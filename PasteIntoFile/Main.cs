using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using CommandLine;
using CommandLine.Text;
using Microsoft.Toolkit.Uwp.Notifications;
using PasteIntoFile.Properties;
using WK.Libraries.SharpClipboardNS;

namespace PasteIntoFile {
    static class Program {
        public const string PATCHED_CLIPBOARD_MAGIC = "PasteIntoFileHasPatchedTheClipboard";
        private static int saveCount = 0;

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

            [Option("enable-patching", HelpText = "Enables clipboard patching while running in system tray", SetName = "patching")]
            public bool RegisterPatching { get; set; }

            [Option("disable-patching", HelpText = "Disables clipboard patching", SetName = "patching")]
            public bool UnregisterPatching { get; set; }

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
                // New version installed with default settings (upgradePerformed == false)
                // https://stackoverflow.com/a/534335/13324744
                Settings.Default.Upgrade(); // Migrate settings from previous version (if any)
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

        /// <summary>
        /// Run program to copy file contents to clipboard
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunCopy(ArgsCopy args) {
            try {
                string path = Path.GetFullPath(args.FilePath);

                var contents = ClipboardContents.FromFile(path);

                if (contents != null) {
                    contents.CopyToClipboard(path);
                    return 0;
                }

                MessageBox.Show(String.Format(Resources.str_copy_failed_unknown_format, path),
                    Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 1;
        }

        /// <summary>
        /// Run setup wizard with config settings gui
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
        /// If enabled, also monitor and patch clipboard with file drop list
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
                    MessageBox.Show(Resources.str_copy_failed_not_single_file, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            copy.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.C);

            // Register clipboard observer for patching
            SharpClipboard clipMonitor = null;
            if (Settings.Default.trayPatchingEnabled) {
                bool skipFirst = true;
                clipMonitor = new SharpClipboard();
                clipMonitor.ClipboardChanged += (s, e) => {
                    if (skipFirst) { skipFirst = false; return; }
                    if (PatchedClipboardContents() is IDataObject data) {
                        clipMonitor.MonitorClipboard = false; // to prevent infinite callback
                        Clipboard.SetDataObject(data, false);
                        clipMonitor.MonitorClipboard = true;
                    }
                };
            }

            // Tray icon
            NotifyIcon icon = new NotifyIcon();
            icon.Icon = Resources.app_icon;
            icon.Text = Resources.app_title;
            icon.ContextMenu = new ContextMenu(new[] {
                new MenuItem(Resources.str_open_paste_into_file, (s, e) => new Dialog(forceShowDialog: true).Show()),
                new MenuItem(Resources.str_settings, (s, e) => new Wizard().Show()),
                new MenuItem(Resources.str_exit, (s, e) => { Application.Exit(); }),
            });
            icon.Visible = true;

            // Check for updates (async)
            CheckForUpdates();

            Application.Run();

            // leave the clipboard monitoring chain in a clean way, otherwise the chain will break when the program exits
            clipMonitor?.StopMonitoring();

            icon.Visible = false;
            return 0;
        }


        /// <summary>
        /// Return patched clipboard by creating a temporary file and adding it to the clipboard if necessary
        /// </summary>
        /// <returns>Patched clipboard data or null if patching not necessary</returns>
        public static IDataObject PatchedClipboardContents() {
            // Analyze clipboard data
            if (Clipboard.ContainsFileDropList()) return null;
            var clipData = ClipboardContents.FromClipboard();
            var filename = Dialog.formatFilenameTemplate(Settings.Default.filenameTemplate, clipData.Timestamp, saveCount);
            var ext = Dialog.determineExtension(clipData.PrimaryContent);
            var contentToSave = clipData.ForExtension(ext);
            if (contentToSave == null) return null;

            // Save clipboard content to temporary file
            var dirname = Path.Combine(Path.GetTempPath(), "PasteIntoFile");
            try { Directory.Delete(dirname, true); } catch { /* try to keep tmp dir clean, ignore errors */ }
            Directory.CreateDirectory(dirname);
            if (!string.IsNullOrWhiteSpace(ext) && !filename.EndsWith("." + ext)) filename += "." + ext;
            var file = Path.Combine(dirname, filename);
            contentToSave.SaveAs(file, ext);
            saveCount++;

            // Patch clipboard with temporary file
            IDataObject data = new DataObject();
            if (Clipboard.GetDataObject()?.GetFormats(false) is string[] formats) {
                // Mirror clipboard (directly using data = Clipboard.GetDataObject() does not work)
                foreach (var format in formats) {
                    data.SetData(format, false, Clipboard.GetData(format));
                }
            }
            data.SetData(DataFormats.FileDrop, new[] { file });
            data.SetData(PATCHED_CLIPBOARD_MAGIC, true); // tag used to distinguish original and patched clipboard
            return data;
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
                if (args.RegisterPatching)
                    Settings.Default.trayPatchingEnabled = true;
                if (args.UnregisterPatching)
                    Settings.Default.trayPatchingEnabled = false;

                Settings.Default.Save();
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
        /// <param name="link">Optional link to visit when clicking the balloon</param>
        /// <param name="silent">If true, make a silent balloon (default)</param>
        public static void ShowBalloon(string title, string[] message, ushort expire = 5, string link = null, bool silent = true) {
            var builder = new ToastContentBuilder().AddText(title);
            foreach (var s in message) {
                builder.AddText(s);
            }
            if (silent)
                builder.AddAudio(null, null, true);

            if (link != null)
                builder.AddButton(Resources.str_open, ToastActivationType.Protocol, link);

            builder.Show(toast => {
                if (expire > 0) {
                    toast.ExpirationTime = DateTime.Now.AddSeconds(expire);
                }
            });
        }


        /// <summary>
        /// Checks for updates
        /// To reduce server load, results are cached and frequent queries skipped
        /// </summary>
        /// <returns>If an update is available</returns>
        public static async Task<bool> CheckForUpdates() {
            bool newReleaseFound = false;
            if ((DateTime.Now - Settings.Default.updateLatestVersionLastCheck).TotalDays > 7) {
                // Last check outdated, check again
                Settings.Default.updateLatestVersionLastCheck = DateTime.Now;
                Settings.Default.Save();
                try {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("PasteIntoFile", Application.ProductVersion));
                    var data = await client.GetStringAsync(new Uri("https://api.github.com/repos/eltos/PasteIntoFile/releases/latest"));
                    var match = Regex.Match(data, "\"(https://github.com/eltos/PasteIntoFile/releases/tag/v(\\d+(\\.\\d+)*))\"");
                    if (match.Success && match.Groups[2].Value != Settings.Default.updateLatestVersion) {
                        newReleaseFound = true;
                        Settings.Default.updateLatestVersion = match.Groups[2].Value;
                        Settings.Default.updateLatestVersionLink = match.Groups[1].Value;
                        Settings.Default.Save();
                    }
                } catch {
                    Console.Error.WriteLine("Failed to check for updates");
                }
            }

            try {
                var thisVersion = Version.Parse(Application.ProductVersion);
                var latestVersion = Version.Parse(Settings.Default.updateLatestVersion);
                if (latestVersion.CompareTo(thisVersion) > 0) {
                    // Update available
                    if (newReleaseFound) {
                        ShowBalloon(string.Format(Resources.str_version_update_available, Application.ProductVersion, Settings.Default.updateLatestVersion),
                            new[] { Settings.Default.updateLatestVersionLink }, 0, Settings.Default.updateLatestVersionLink, false);
                    }
                    return true;
                }
            } catch { /* ignore errors due to parsing of fetched version */ }
            return false;

        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

    }
}
