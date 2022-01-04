using System;
using System.Diagnostics;
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
            [Option('f', "filename", HelpText = "Filename template with optional date format variable such as {0:yyyyMMdd HHmmSS}")]
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
            public string HiddenDirectory { get; set; } // for backwards compatibility: directory as first value argument
            
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
            // redirect console output to parent process, for command line help etc.
            // not perfect, but probably as good as it can be: https://stackoverflow.com/a/11058118
            AttachConsole( ATTACH_PARENT_PROCESS );
            
            if (!Settings.Default.upgradePerformed)
            {
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
                .ParseArguments<ArgsMain, ArgsConfig, ArgsWizard>(args);
            return parseResult.MapResult(
                (ArgsMain opts) => RunMain(opts),
                (ArgsConfig opts) => RunConfig(opts),
                (ArgsWizard opts) => RunWizard(opts),
                errs => DisplayHelp(parseResult));
            
        }
        
        static int DisplayHelp<T>(ParserResult<T> result)
        {  
            var helpText = CommandLine.Text.HelpText.AutoBuild(result, h =>
            {
                // customized help text
                h.AdditionalNewLineAfterOption = false;
                h.AddPostOptionsLine(Resources.str_main_info_url);
                return CommandLine.Text.HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine("\n\n" + helpText);
            return 1;
        }

        /// <summary>
        /// Run main program in default mode to save clipboard contents
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code</returns>
        static int RunMain(ArgsMain args)
        {
            ApplyCommonArgs(args);
            var forceShowDialog = args.Directory == null;
            
            if (Settings.Default.firstLaunch)
            {
                RunWizard();
                forceShowDialog = true;
            }


            var location = (args.Directory?? args.HiddenDirectory??
                    ExplorerUtil.GetActiveExplorerPath()??
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                .Trim().Trim("\"".ToCharArray()); // remove trailing " fixes paste in root dir
            
            Application.Run(new Dialog(location, forceShowDialog));
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

        static void ApplyCommonArgs(ArgsCommon args)
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
            ApplyCommonArgs(args);
            try
            {
                if (args.Register)
                    RegistryUtil.RegisterApp();
                if (args.Unregister)
                    RegistryUtil.UnRegisterApp();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message + "\n" + Resources.str_message_run_as_admin);
                return 1;
            }
            return 0;
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
            var notification = new NotifyIcon
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

        [System.Runtime.InteropServices.DllImport( "kernel32.dll" )]
        static extern bool AttachConsole( int dwProcessId );
        private const int ATTACH_PARENT_PROCESS = -1;

    }
}
