using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    static class Program
    {
		public static readonly string RegistrySubKey = "Paste Into File";

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
                var v = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                    is_light_mode = false;
            }
            catch { }

            Settings.Default.darkTheme = !is_light_mode;
            Settings.Default.Save();

            if (Settings.Default.firstLaunch)
            {
                Application.Run(new FirstLaunch());
                if (Settings.Default.firstLaunch)
                    return;
            }

            if (!Clipboard.ContainsText() && !Clipboard.ContainsImage())
            {
                DialogResult result = MessageBox.Show(Resources.str_noclip_text, Resources.str_main_window_title, MessageBoxButtons.OK);
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
                else if (args[0] == "/filename")
                {
                    if (args.Length > 1) {
                        RegisterFilename(args[1]);
                    }
                    return;
                }
                Application.Run(new frmMain(args[0]));
            }
            else
            {
                Application.Run(new frmMain());
            }

        }

		public static void RegisterFilename(string filename)
		{
			try
			{
                var key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey(RegistrySubKey);
                key = key.CreateSubKey("filename");
                key.SetValue("", filename);

                MessageBox.Show(Resources.str_message_register_filename_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				//throw;
				MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        public static void UnRegisterApp()
        {
            try
            {
                var key = OpenDirectoryKey().OpenSubKey(@"Background\shell", true);
				key.DeleteSubKeyTree(RegistrySubKey);

                key = OpenDirectoryKey().OpenSubKey("shell", true);
				key.DeleteSubKeyTree(RegistrySubKey);

				MessageBox.Show(Resources.str_message_unregister_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public static void RegisterApp()
        {
            try
            {
				var key = OpenDirectoryKey().CreateSubKey(@"Background\shell").CreateSubKey(RegistrySubKey);
				key.SetValue("", Resources.str_contextentry);
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                key = key.CreateSubKey("command");
				key.SetValue("" , "\"" + Application.ExecutablePath + "\" \"%V\"");

				key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey(RegistrySubKey);
				key.SetValue("", Resources.str_contextentry);
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                key = key.CreateSubKey("command");
				key.SetValue("" , "\"" + Application.ExecutablePath + "\" \"%1\"");
				MessageBox.Show(Resources.str_message_register_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                //throw;
				MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static void ShowBalloon(string title, string message, ushort timeout = 5000)
        {
            var notification = new NotifyIcon()
            {
                Visible = true,
                Icon = System.Drawing.SystemIcons.Information,
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

        static RegistryKey OpenDirectoryKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\Classes\Directory");
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
