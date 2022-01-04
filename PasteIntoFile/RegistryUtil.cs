using System.Collections.Generic;
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

        private static string PRIMARY_KEY_NAME = "PasteIntoFile";
        private static IEnumerable<RegistryKey> OpenClassKeys(string type = null) {
	        if (type == null) // return all class type keys (dirs and files)
	        {
		        return OpenClassKeys("Directory").Concat(OpenClassKeys("*"));
	        }
	        var node = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + type);
	        return new[] {
		        node.CreateSubKey(@"Background\shell"),
		        node.CreateSubKey(@"shell"),
            } ;
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
	        foreach (var classKey in OpenClassKeys())
	        {
		        if (classKey == null || !classKey.GetSubKeyNames().Contains(PRIMARY_KEY_NAME)) return false;
	        }
	        return true;
        }
        
        /// <summary>
        /// Remove context menu entry
        /// </summary>
        public static void UnRegisterApp()
        {
	        foreach (var classKey in OpenClassKeys())
	        {
		        classKey.DeleteSubKeyTree(PRIMARY_KEY_NAME);
	        }
        }

        /// <summary>
        /// Create context menu entry
        /// </summary>
        public static void RegisterApp(bool silent = false)
        {
	        // register "paste into file" for directory context menu
	        foreach (var classKey in OpenClassKeys("Directory"))
	        {
		        var key = classKey.CreateSubKey(PRIMARY_KEY_NAME);
		        key.SetValue("", Resources.str_contextentry);
		        key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
		        key = key.CreateSubKey("command");
		        key.SetValue("", "\"" + Application.ExecutablePath + "\" paste \"%V\"");

	        }

	        // register "copy from file" for file context menu (any extension)
	        foreach (var classKey in OpenClassKeys("*"))
	        {
		        var key = classKey.CreateSubKey(PRIMARY_KEY_NAME);
		        key.SetValue("", Resources.str_contextentry_copyfromfile);
		        key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
		        key = key.CreateSubKey("command");
		        key.SetValue("", "\"" + Application.ExecutablePath + "\" copy \"%V\"");

	        }

        }

    }
}