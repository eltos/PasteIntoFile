using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CommandLine;
using CommandLine.Text;
using Microsoft.Toolkit.Uwp.Notifications;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    static class Program {

        class ArgsCommon {
            [Option('f', "filename", HelpText = "Filename template with optional format variables such as\n" +
                                                "{0:yyyyMMdd HHmmSS} for current date and time\n" +
                                                "{1:000} for batch-mode save counter")]
            public string Filename { get; set; }

            [Option("text-extension", HelpText = "File extension for text contents")]
            public string TextExtension { get; set; }

            [Option("image-extension", HelpText = "File extension for image contents")]
            public string ImageExtension { get; set; }

            [Option("subdir", HelpText = "Template for name of subfolder to create when holding CTRL (see filename for format variables)")]
            public string Subdir { get; set; }

            [Option('c', "clear", HelpText = "Clear clipboard after save (true/false)")]
            public bool? ClearClipboard { get; set; }

            [Option('a', "autosave", HelpText = "Autosave file without prompt (true/false)")]
            public bool? Autosave { get; set; }

        }

        [Verb("paste", true, HelpText = "Paste clipboard contents into file")]
        class ArgsPaste : ArgsCommon {
            [Option('d', "directory", HelpText = "Path of directory to save file into")]
            public string Directory { get; set; }

            [Value(0, Hidden = true)]
            public string DirectoryFallback { get; set; } // alternative: directory as first value argument

        }

        [Verb("copy", HelpText = "Copy file contents to clipboard")]
        class ArgsCopy {
            [Value(0, HelpText = "Path of file to copy from")]
            public string FilePath { get; set; }

        }

        [Verb("config", HelpText = "Change configuration (without saving clipboard)")]
        class ArgsConfig : ArgsCommon {
            [Option("register", HelpText = "Register context menu entry", SetName = "register")]
            public bool RegisterContextMenu { get; set; }

            [Option("unregister", HelpText = "Unregister context menu entry", SetName = "register")]
            public bool UnregisterContextMenu { get; set; }

            [Option("enable-autostart", HelpText = "Register program to run in system tray on windows startup", SetName = "autostart")]
            public bool RegisterAutostart { get; set; }

            [Option("disable-autostart", HelpText = "Remove autostart on windows startup registration", SetName = "autostart")]
            public bool UnregisterAutostart { get; set; }

        }

        [Verb("wizard", HelpText = "Open the first-launch wizard")]
        class ArgsWizard {

        }

        [Verb("tray", HelpText = "Open in tray and wait for hotkey Win + Alt + V")]
        class ArgsTray {

        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args) {
            // redirect console output to parent process, for command line help etc.
            // not perfect, but probably as good as it can be: https://stackoverflow.com/a/11058118
            AttachConsole(ATTACH_PARENT_PROCESS);

            if (!Settings.Default.upgradePerformed) {
                // New app version was installed
                Settings.Default.Upgrade();
                Settings.Default.upgradePerformed = true;
                Settings.Default.Save();
            }

            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // parse command line arguments
            var parseResult = new Parser(with => with.HelpWriter = null)
                .ParseArguments<ArgsPaste, ArgsCopy, ArgsConfig, ArgsWizard, ArgsTray>(args);
            return parseResult.MapResult(
                (ArgsPaste opts) => RunPaste(opts),
                (ArgsCopy opts) => RunCopy(opts),
                (ArgsConfig opts) => RunConfig(opts),
                (ArgsWizard opts) => RunWizard(opts),
                (ArgsTray opts) => RunTray(opts),
                errs => DisplayHelp(parseResult, errs));

        }

        static int DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs) {
            HelpText helpText;
            if (errs.IsVersion())
                helpText = HelpText.AutoBuild(result);
            else {
                helpText = HelpText.AutoBuild(result, h => {
                    // customize help text
                    h.AdditionalNewLineAfterOption = false;
                    h.AddPostOptionsLine(Resources.str_main_info_url);
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                });
            }
            Console.WriteLine("\n\n" + helpText);
            return 1;
        }

        /// <summary>
        /// Run main program in default mode to save clipboard contents
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunPaste(ArgsPaste args) {
            ApplyCommonArgs(args);

            var directory = args.Directory ?? args.DirectoryFallback;
            var forceShowDialog = directory == null;

            if (Settings.Default.firstLaunch) {
                RunWizard();
                forceShowDialog = true;
            }


            Application.Run(new Dialog(directory, forceShowDialog));
            return Environment.ExitCode;
        }

        static int RunCopy(ArgsCopy args) {
            try {
                string path = Path.GetFullPath(args.FilePath);

                var contents = ClipboardContents.FromFile(path);

                if (contents != null) {
                    contents.CopyToClipboard(path);
                    return 0;
                }

                MessageBox.Show(String.Format(Resources.str_copy_failed_unknown_format, path),
                    Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 1;
        }

        /// <summary>
        /// Run wizard
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunWizard(ArgsWizard args = null) {
            if (RegistryUtil.IsContextMenuEntryRegistered())
                RegistryUtil.RegisterContextMenuEntry(!Settings.Default.autoSave); // overwrites default entry with localized strings

            var wizard = new Wizard();
            // Make sure to bring window to foreground (installer will open window in background)
            wizard.BringToFrontForced();
            Application.Run(wizard);
            return 0;
        }

        /// <summary>
        /// Run program in system tray and wait for hotkey press
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunTray(ArgsTray args = null) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Register hotkeys
            KeyboardHook paste = new KeyboardHook();
            paste.KeyPressed += (s, e) => {
                var arg = new ArgsPaste();
                arg.Directory = ExplorerUtil.GetActiveExplorerPath();
                RunPaste(arg);
            };
            paste.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.V);
            paste.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt | ModifierKeys.Shift, Keys.V);
            paste.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt | ModifierKeys.Control, Keys.V);
            paste.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt | ModifierKeys.Shift | ModifierKeys.Control, Keys.V);

            KeyboardHook copy = new KeyboardHook();
            copy.KeyPressed += (s, e) => {
                var files = ExplorerUtil.GetActiveExplorerSelectedFiles();
                if (files.Count == 1) {
                    var arg = new ArgsCopy();
                    arg.FilePath = files.Item(0).Path;
                    RunCopy(arg);
                } else {
                    MessageBox.Show(Resources.str_copy_failed_not_single_file, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            copy.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.C);

            // Tray icon
            NotifyIcon icon = new NotifyIcon();
            icon.Icon = Resources.icon;
            icon.Text = Resources.str_main_window_title;
            icon.ContextMenu = new ContextMenu(new[] {
                new MenuItem(Resources.str_open_paste_into_file, (s, e) => new Dialog(forceShowDialog: true).Show()),
                new MenuItem(Resources.str_settings, (s, e) => new Wizard().Show()),
                new MenuItem(Resources.str_exit, (s, e) => { Application.Exit(); }),
            });
            icon.Visible = true;

            Application.Run();

            icon.Visible = false;
            return 0;
        }


        static void ApplyCommonArgs(ArgsCommon args) {
            if (args.Filename != null)
                Settings.Default.filenameTemplate = args.Filename;
            if (args.TextExtension != null)
                Settings.Default.extensionText = args.TextExtension;
            if (args.ImageExtension != null)
                Settings.Default.extensionImage = args.ImageExtension;
            if (args.Subdir != null)
                Settings.Default.subdirTemplate = args.Subdir;
            if (args.ClearClipboard != null)
                Settings.Default.clrClipboard = (bool)args.ClearClipboard;
            if (args.Autosave != null)
                Wizard.SetAutosaveMode((bool)args.Autosave);

            Settings.Default.Save();
        }

        /// <summary>
        /// Run only config update
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunConfig(ArgsConfig args) {
            ApplyCommonArgs(args);
            try {
                if (args.RegisterContextMenu)
                    RegistryUtil.RegisterContextMenuEntry(!Settings.Default.autoSave);
                if (args.UnregisterContextMenu)
                    RegistryUtil.UnRegisterContextMenuEntry();
                if (args.RegisterAutostart)
                    RegistryUtil.RegisterAutostart();
                if (args.UnregisterAutostart)
                    RegistryUtil.UnRegisterAutostart();
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message + "\n" + Resources.str_message_run_as_admin);
                return 1;
            }
            return 0;
        }


        /// <summary>
        /// Restart app in admin mode
        /// </summary>
        /// <param name="location">File location to be passed to new instance</param>
        public static void RestartAppElevated(string location) {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Application.ExecutablePath;
            proc.Verb = "runas";
            proc.Arguments = "\"" + location + "\"";

            try {
                Process.Start(proc);
            } catch {
                // The user refused the elevation.
                // Do nothing and return directly ...
                return;
            }
            Application.Exit();
        }

        /// <summary>
        /// Shows a balloon message in the windows notification bar
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="message">Body of the message</param>
        /// <param name="expire">Duration after which message is dismissed in second</param>
        public static void ShowBalloon(string title, string[] message, ushort expire = 5) {
            var builder = new ToastContentBuilder().AddText(title);
            foreach (var s in message) {
                builder.AddText(s);
            }
            builder.AddAudio(null, null, true); // silent

            builder.Show(toast => {
                toast.ExpirationTime = DateTime.Now.AddSeconds(expire);
            });
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

    }
}
