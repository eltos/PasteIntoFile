using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public class RegistryUtil
    {
        // Please note that registry keys are also created by installer
        // and removed upon uninstall

        public static RegistryKey OpenDirectoryKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\Classes\Directory");
        }


        public static bool IsDarkMode()
        {
	        try
	        {
		        var v = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
		        return v != null && v.ToString() == "0";
	        }
	        catch
	        {
		        // ignored
	        }

	        return false;
        }
        
        
        /// <summary>
        /// Checks if context menu entry is registered
        /// </summary>
        /// <returns>app registration status (true/false)</returns>
        public static bool IsAppRegistered()
        {
            var key = OpenDirectoryKey().OpenSubKey("shell");
            if (key == null || !key.GetSubKeyNames().Contains("PasteIntoFile")) return false;
            
            key = OpenDirectoryKey().OpenSubKey(@"Background\shell");
            if (key == null || !key.GetSubKeyNames().Contains("PasteIntoFile")) return false;
            
            return true;
        }
        
        /// <summary>
        /// Remove context menu entry
        /// </summary>
        public static void UnRegisterApp()
        {
            var key = OpenDirectoryKey().OpenSubKey(@"Background\shell", true);
			key.DeleteSubKeyTree("PasteIntoFile");

            key = OpenDirectoryKey().OpenSubKey("shell", true);
			key.DeleteSubKeyTree("PasteIntoFile");
			
        }

        /// <summary>
        /// Create context menu entry
        /// </summary>
        public static void RegisterApp(bool silent = false)
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
			key.SetValue("" , "\"" + Application.ExecutablePath + "\" \"%V\"");

        }

    }
}