using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PasteIntoFile
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool is_light_mode = true;
            try
            {
                var v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                    is_light_mode = false;
            }
            catch { }

            Properties.Settings.Default.darkTheme = !is_light_mode;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.firstLaunch)
            {
                Application.Run(new FirstLaunch());
                if (Properties.Settings.Default.firstLaunch)
                    return;
            }

            if (!Clipboard.ContainsText() && !Clipboard.ContainsImage())
            {
                DialogResult result = MessageBox.Show("Clipboard is empty", "Warning", MessageBoxButtons.OK);
                return;
            }

            if (args.Length > 0)
            {
                if (args[0] == "/reg")
                {
                    RegisterApp();
                    return;
                }
                else if (args[0] == "/unreg")
                {
                    UnRegisterApp();
                    return;
                }
                Application.Run(new frmMain(args[0]));
            }
            else
            {
                Application.Run(new frmMain());
            }

        }

        public static void UnRegisterApp()
        {
            try
            {
                var key = OpenDirectoryKey().OpenSubKey(@"Background\shell", true);
                key.DeleteSubKeyTree("Paste Into File");

                key = OpenDirectoryKey().OpenSubKey("shell", true);
                key.DeleteSubKeyTree("Paste Into File");

                MessageBox.Show("Application has been Unregistered from your system", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nPlease run the application as Administrator !", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public static void RegisterApp()
        {
            try
            {
                var key = OpenDirectoryKey().CreateSubKey(@"Background\shell").CreateSubKey("Paste Into File");
                key = key.CreateSubKey("command");
                key.SetValue("", Application.ExecutablePath + " \"%V\"");

                key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey("Paste Into File");
                key = key.CreateSubKey("command");
                key.SetValue("", Application.ExecutablePath + " \"%1\"");
                MessageBox.Show("Application has been registered with your system", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message + "\nPlease run the application as Administrator !", "Paste As File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void RestartApp()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Application.ExecutablePath;
            proc.Verb = "runas";

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

        static RegistryKey OpenDirectoryKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\Classes\Directory");
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
