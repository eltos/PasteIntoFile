using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    public class RegistryUtil {

        public class Command {
            public delegate bool HasDialog();

            public readonly string Title;
            public readonly string Args;
            public readonly HasDialog Dialog;

            public Command(string title, string args, HasDialog dialog = null) {
                Title = title;
                Args = args;
                Dialog = dialog ?? (() => false);
            }

            public void Register(RegistryKey key) {
                key.SetValue("MUIVerb", Title + (Dialog() ? "…" : ""));
                var cmdkey = key.CreateSubKey("command");
                cmdkey.SetValue("", "\"" + Application.ExecutablePath + "\" " + Args);
            }
        }

        // Please note that registry keys are also created by installer and removed upon uninstall
        // Always keep the "Installer/PasteIntoFile.wxs" up to date with the keys used below!

        public static readonly ContextMenuEntryPaste ContextMenuPaste = new ContextMenuEntryPaste();
        public class ContextMenuEntryPaste : ContextMenuEntry {
            public ContextMenuEntryPaste() : base("Directory", "PasteIntoFile") { }
            protected override Command Command => new Command(Resources.str_contextentry, "paste \"%V\"", () => !Settings.Default.autoSave);
        }

        public static readonly ContextMenuEntryCopy ContextMenuCopy = new ContextMenuEntryCopy();
        public class ContextMenuEntryCopy : ContextMenuEntry {
            public ContextMenuEntryCopy() : base("*", "PasteIntoFile") { }
            protected override Command Command => new Command(Resources.str_contextentry_copyfromfile, "copy \"%V\"");
        }

        public static readonly ContextMenuEntryReplace ContextMenuReplace = new ContextMenuEntryReplace();
        public class ContextMenuEntryReplace : ContextMenuEntry {
            public ContextMenuEntryReplace() : base("*", "PasteIntoFile_replace") { }
            protected override string AppliesTo() => APPEND_NOT_SUPPORTED;
            protected override Command Command => new Command(
                Resources.str_contextentry_paste_into_existing_file + " (" + Resources.str_replace + ")",
                "paste --overwrite=true --directory=\"%w\" --filename=\"%V\" --autosave=true"
            );
            public override void Register() {
                base.Register();
                new ContextMenuEntryReplaceAppend().Register();
            }
            public override void UnRegister() {
                base.UnRegister();
                new ContextMenuEntryReplaceAppend().UnRegister();
            }
        }
        private class ContextMenuEntryReplaceAppend : ContextMenuEntry {
            public ContextMenuEntryReplaceAppend() : base("*", "PasteIntoFile_replace_append") { }
            protected override string AppliesTo() => "NOT ( " + APPEND_NOT_SUPPORTED + " )";
            protected override string Title => Resources.str_contextentry_paste_into_existing_file;
            protected override Command[] SubCommands => new[]{
                new Command(Resources.str_replace, "paste --overwrite=true --directory=\"%w\" --filename=\"%V\" --autosave=true"),
                new Command(Resources.str_append, "paste --append --directory=\"%w\" --filename=\"%V\" --autosave=true")
            };
        }
        // for file types that are known not to support appending, show an entry without subcommands instead
        private static string APPEND_NOT_SUPPORTED = "System.FileExtension:=." + string.Join(" OR System.FileExtension:=.",
            new ImageContent(null).Extensions.Concat(
                new VectorImageContent(null).Extensions.Concat(
                    new SvgContent("").Extensions.Concat(
                        new UrlContent(null).Extensions))));



        public static readonly ContextMenuEntry[] AllContextMenu = { ContextMenuPaste, ContextMenuCopy, ContextMenuReplace };



        /// <summary>
        /// Base class for context menu entries
        ///
        /// These must have a type (file extension or "Directory"), key and
        /// either a command, or a title and subcommands.
        /// </summary>
        public abstract class ContextMenuEntry {
            // Documentation:
            // https://docs.microsoft.com/en-us/windows/win32/shell/context
            // https://docs.microsoft.com/en-us/windows/win32/shell/context-menu

            protected readonly string Type;
            protected readonly string Key;

            protected ContextMenuEntry(string type, string key) {
                Type = type;
                Key = key;
            }

            protected virtual string AppliesTo() => null;

            // single command for regular entries
            protected virtual Command Command => null;

            // title and subcommands for submenu entries
            protected virtual string Title => null;
            protected virtual Command[] SubCommands => null;



            private static RegistryKey RootKey => Registry.CurrentUser.CreateSubKey(@"Software\Classes");

            /// <summary>
            /// Opens a number of class sub keys according to the type.
            /// </summary>
            /// <returns>List of registry keys</returns>
            private IEnumerable<RegistryKey> OpenClassKeys() {
                if (Type == "Directory")
                    return new[] { RootKey.CreateSubKey(@"Directory\shell"), RootKey.CreateSubKey(@"Directory\Background\shell") };
                return new[] { RootKey.CreateSubKey(Type + @"\shell") };
            }

            /// <summary>
            /// Checks if context menu entry is registered
            /// </summary>
            /// <returns>context menu entry registration status (true/false)</returns>
            public bool IsRegistered() {
                foreach (var classKey in OpenClassKeys()) {
                    if (classKey == null || !classKey.GetSubKeyNames().Contains(Key)) return false;
                }

                return true;
            }

            /// <summary>
            /// Remove context menu entry
            /// </summary>
            public virtual void UnRegister() {
                foreach (var classKey in OpenClassKeys()) {
                    classKey.DeleteSubKeyTree(Key);
                }
            }

            /// <summary>
            /// Create context menu entry
            /// </summary>
            public virtual void Register() {
                foreach (var classKey in OpenClassKeys()) {
                    var key = classKey.CreateSubKey(Key);
                    key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
                    if (AppliesTo() is string appliesTo) key.SetValue("AppliesTo", appliesTo);
                    Command?.Register(key);
                    if (SubCommands?.Length > 0) {
                        key.SetValue("MUIVerb", Title);
                        key.SetValue("ExtendedSubCommandsKey", key.Name.Substring(RootKey.Name.Length + 1));
                        var subkey = key.CreateSubKey("shell");
                        for (var i = 0; i < SubCommands.Length; i++) {
                            SubCommands[i].Register(subkey.CreateSubKey("cmd" + i));
                        }
                    }
                }
            }

        }




        /// <summary>
        /// Re-registers all registered context menu entries (with correct localisation)
        /// </summary>
        public static void ReRegisterContextMenuEntries() {
            foreach (var entry in AllContextMenu) {
                if (entry.IsRegistered()) {
                    entry.UnRegister();
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
