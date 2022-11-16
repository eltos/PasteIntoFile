using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    public class RegistryUtil {

        // Please note that registry keys are also created by installer and removed upon uninstall
        // Always keep the "Installer/PasteIntoFile.wxs" up to date with the keys used below!
        public static readonly ContextMenuEntry ContextMenuPaste = new ContextMenuEntry(
            "Directory", "PasteIntoFile", Resources.str_contextentry,
            "paste \"%V\"", () => !Settings.Default.autoSave);
        public static readonly ContextMenuEntry ContextMenuCopy = new ContextMenuEntry(
            "*", "PasteIntoFile", Resources.str_contextentry_copyfromfile,
            "copy \"%V\"", () => false);
        public static readonly ContextMenuEntry ContextMenuReplace = new ContextMenuEntry(
            "*", "PasteIntoFile_replace", Resources.str_contextentry_replaceintofile,
            "paste --directory=\"%w\" --filename=\"%V\" --autosave=true --overwrite=true", () => false);

        public static readonly ContextMenuEntry[] AllContextMenu = { ContextMenuPaste, ContextMenuCopy, ContextMenuReplace };


        public class ContextMenuEntry {
            // Documentation:
            // https://docs.microsoft.com/en-us/windows/win32/shell/context
            // https://docs.microsoft.com/en-us/windows/win32/shell/context-menu

            private readonly string _type;
            private readonly string _key;
            private readonly string _title;
            private readonly string _args;
            private readonly HasDialog _hasDialog;

            public delegate bool HasDialog();

            /// <summary>
            ///
            /// </summary>
            /// <param name="type">File type or "Directory" or "*"</param>
            /// <param name="key"></param>
            /// <param name="title">Title to show in context menu entry</param>
            /// <param name="args">Arguments to pass to binary</param>
            /// <param name="hasDialog"></param>
            public ContextMenuEntry(string type, string key, string title, string args, HasDialog hasDialog) {
                _type = type;
                _key = key;
                _title = title;
                _args = args;
                _hasDialog = hasDialog;
            }

            /// <summary>
            /// Opens a number of class sub keys according to the type.
            /// </summary>
            /// <returns>List of registry keys</returns>
            private IEnumerable<RegistryKey> OpenClassKeys() {
                var classes = Registry.CurrentUser.CreateSubKey(@"Software\Classes");
                if (_type == "Directory")
                    return new[] { classes.CreateSubKey(@"Directory\shell"), classes.CreateSubKey(@"Directory\Background\shell") };
                return new[] { classes.CreateSubKey(_type + @"\shell") };
            }

            /// <summary>
            /// Checks if context menu entry is registered
            /// </summary>
            /// <returns>context menu entry registration status (true/false)</returns>
            public bool IsRegistered() {
                foreach (var classKey in OpenClassKeys()) {
                    if (classKey == null || !classKey.GetSubKeyNames().Contains(_key)) return false;
                }

                return true;
            }

            /// <summary>
            /// Remove context menu entry
            /// </summary>
            public void UnRegister() {
                foreach (var classKey in OpenClassKeys()) {
                    classKey.DeleteSubKeyTree(_key);
                }
            }

            /// <summary>
            /// Create context menu entry
            /// </summary>
            public void Register() {
                foreach (var classKey in OpenClassKeys()) {
                    var key = classKey.CreateSubKey(_key);
                    key.SetValue("", _title + (_hasDialog() ? "…" : ""));
                    key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                    key = key.CreateSubKey("command");
                    key.SetValue("", "\"" + Application.ExecutablePath + "\" " + _args);
                }
            }

        }




        /// <summary>
        /// Re-registers all registered context menu entries (with correct localisation)
        /// </summary>
        public static void ReRegisterContextMenuEntries() {
            foreach (var entry in AllContextMenu) {
                if (entry.IsRegistered()) {
                    entry.Register();
                }
            }
        }


        public static bool IsDarkMode() {
            try {
                var v = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                return v != null && v.ToString() == "0";
            } catch {
                // ignored
            }

            return false;
        }


        /// <summary>
        /// The path to the key where Windows looks for startup applications
        /// </summary>
        private static RegistryKey AutostartSubKey => Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

        /// <summary>
        /// Checks if autostart is registered
        /// </summary>
        /// <returns>autostart registration status (true/false)</returns>
        public static bool IsAutostartRegistered() {
            return AutostartSubKey?.GetValue("PasteIntoFile") != null;
        }

        /// <summary>
        /// Register autostart
        /// </summary>
        public static void RegisterAutostart() {
            AutostartSubKey?.SetValue("PasteIntoFile", "\"" + Application.ExecutablePath + "\" tray");
        }

        /// <summary>
        /// Remove autostart registration
        /// </summary>
        public static void UnRegisterAutostart() {
            AutostartSubKey?.DeleteValue("PasteIntoFile", false);
        }

    }
}
