using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    static class Program
    {

        class ArgsCommon
        {
            [Option('f', "filename", HelpText = "Filename template (may contain format variables such as {0:yyyyMMdd HHmmSS})")]
            public string Filename { get; set; }

            [Option("text-extension", HelpText = "File extension for text contents")]
            public string TextExtension { get; set; }
        
            [Option("image-extension", HelpText = "File extension for image contents")]
            public string ImageExtension { get; set; }
            
            [Option('c', "clear", HelpText = "Clear clipboard after save (true/false)")]
            public bool? ClearClipboard { get; set; }
            
            [Option('a', "autosave", HelpText = "Autosave file without prompt (true/false)")]
            public bool? Autosave { get; set; }

        }
        
        [Verb("save", true, HelpText = "Save clipboard contents")]
        class ArgsMain : ArgsCommon
        {
            [Option('d', "directory", HelpText = "Path of directory to save file into")]
            public string Directory { get; set; }

            [Value(0, Hidden = true)]
            public string HiddenDirectory { get; set; }
            
        }

        [Verb("config", HelpText = "Change configuration (without saving clipboard)")]
        class ArgsConfig : ArgsCommon
        {
            [Option("register", HelpText = "Register context menu entry", SetName = "register")]
            public bool Register { get; set; }

            [Option("unregister", HelpText = "Unregister context menu entry", SetName = "register")]
            public bool Unregister { get; set; }
            
        }

        [Verb("wizard", HelpText = "Open the first-launch wizard")]
        class ArgsWizard 
        {
            
        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (!Settings.Default.upgradePerformed)
            {
                // New app version was installed
                Settings.Default.Upgrade();
                Settings.Default.upgradePerformed = true;
                Settings.Default.Save();
            }

            return Parser.Default.ParseArguments<ArgsMain, ArgsConfig, ArgsWizard>(args)
                .MapResult(
                    (ArgsMain opts) => RunMain(opts),
                    (ArgsConfig opts) => RunConfig(opts),
                    (ArgsWizard opts) => RunWizard(opts),
                    errs => 1);

        }

        /// <summary>
        /// Run main program in default mode to save clipboard contents
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunMain(ArgsMain args)
        {
            ApplyConfig(args);
            
            if (Settings.Default.firstLaunch)
            {
                RunWizard();
            }



            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool isLightMode = true;
            try
            {
                var v = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                    isLightMode = false;
            }
            catch
            {
                // ignored
            }

            Settings.Default.darkTheme = !isLightMode;
            Settings.Default.Save();

            
            var location = (args.Directory?? args.HiddenDirectory??
                           ExplorerUtil.GetActiveExplorerPath()??
                           Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                .Trim().Trim("\"".ToCharArray()); // remove trailing " fixes paste in root dir
            
            Application.Run(new Dialog(location, default, args.Directory == null));
            return 0;
        }

        /// <summary>
        /// Run wizard
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunWizard(ArgsWizard args = null)
        {
            Application.Run(new Wizard());
            return 0;
        }

        static void ApplyConfig(ArgsCommon args)
        {
            if (args.Filename != null)
                Settings.Default.filenameTemplate = args.Filename;
            if (args.TextExtension != null)
                Settings.Default.extensionText = args.TextExtension;
            if (args.ImageExtension != null)
                Settings.Default.extensionImage = args.ImageExtension;
            if (args.ClearClipboard != null)
                Settings.Default.clrClipboard = (bool) args.ClearClipboard;
            if (args.Autosave != null)
                Settings.Default.autoSave = (bool) args.Autosave;

            Settings.Default.Save();
        }
        
        /// <summary>
        /// Run only config update
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunConfig(ArgsConfig args)
        {
            ApplyConfig(args);
            if (args.Register)
                return RegisterApp() ? 0 : 1;
            if (args.Unregister)
                return UnRegisterApp() ? 0 : 1;
            return 0;
        }
        
        // Context Menu integration
        //
        // Please note that registry keys are also created by installer
        // and removed upon uninstall

        public static RegistryKey OpenDirectoryKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\Classes\Directory");
        }
        
        /// <summary>
        /// Checks if context menu entry is registered
        /// </summary>
        /// <returns>app registration status (true/false)</returns>
        public static bool IsAppRegistered()
        {
            var key = OpenDirectoryKey().OpenSubKey("shell");
            return key != null && key.GetSubKeyNames().Contains("PasteIntoFile");
        }
        
        /// <summary>
        /// Remove context menu entry
        /// </summary>
        public static bool UnRegisterApp(bool silent = false)
        {
            try
            {
                var key = OpenDirectoryKey().OpenSubKey(@"Background\shell", true);
				key.DeleteSubKeyTree("PasteIntoFile");

                key = OpenDirectoryKey().OpenSubKey("shell", true);
				key.DeleteSubKeyTree("PasteIntoFile");

				MessageBox.Show(Resources.str_message_unregister_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return false;
        }

        /// <summary>
        /// Create context menu entry
        /// </summary>
        public static bool RegisterApp()
        {
            try
            {
				var key = OpenDirectoryKey().CreateSubKey(@"Background\shell").CreateSubKey("PasteIntoFile");
				key.SetValue("", Resources.str_contextentry);
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                key = key.CreateSubKey("command");
				key.SetValue("" , "\"" + Application.ExecutablePath + "\" \"%V\"");

				key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey("PasteIntoFile");
				key.SetValue("", Resources.str_contextentry);
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                key = key.CreateSubKey("command");
				key.SetValue("" , "\"" + Application.ExecutablePath + "\" \"%1\"");
				MessageBox.Show(Resources.str_message_register_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                //throw;
				MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// Restart app in admin mode
        /// </summary>
        /// <param name="location">File location to be passed to new instance</param>
        public static void RestartAppElevated(string location)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Application.ExecutablePath;
            proc.Verb = "runas";
            proc.Arguments = "\"" + location + "\"";

            try
            {
                Process.Start(proc);
            }
            catch
            {
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
        /// <param name="timeout">Duration after which message is dismissed</param>
        public static void ShowBalloon(string title, string message, ushort timeout = 5000)
        {
            var notification = new NotifyIcon()
            {
                Visible = true,
                Icon = Resources.icon,
                // optional - BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                BalloonTipTitle = title,
                BalloonTipText = message,
            };

            // Display for 5 seconds.
            notification.ShowBalloonTip(timeout);

            // This will let the balloon close after it's 5 second timeout
            // for demonstration purposes. Comment this out to see what happens
            // when dispose is called while a balloon is still visible.
            Thread.Sleep(timeout);

            // The notification should be disposed when you don't need it anymore,
            // but doing so will immediately close the balloon if it's visible.
            notification.Dispose();
        }

        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
