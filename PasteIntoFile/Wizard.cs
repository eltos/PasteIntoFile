using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PasteIntoFile.Properties;
using Color = System.Drawing.Color;

namespace PasteIntoFile {
    public sealed partial class Wizard : MasterForm {

        public Wizard() {
            ReloadUi();
        }

        private void ReloadUi() {
            Controls.Clear();
            InitializeComponent();
            Settings.Default.Reload(); // load modifications made from other instance


            Icon = Resources.app_icon;
            Text = Resources.app_title;

            autoSaveCheckBox.Checked = Settings.Default.autoSave;
            autoSaveMayOpenNewCheckBox.Checked = Settings.Default.autoSaveMayOpenNewExplorer;
            contextEntryCheckBoxPaste.Checked = RegistryUtil.ContextMenuPaste.IsRegistered();
            contextEntryCheckBoxCopy.Checked = RegistryUtil.ContextMenuCopy.IsRegistered();
            contextEntryCheckBoxReplace.Checked = RegistryUtil.ContextMenuReplace.IsRegistered();
            autostartCheckBox.Checked = RegistryUtil.IsAutostartRegistered();
            patchingCheckBox.Checked = Settings.Default.trayPatchingEnabled;
            patchingCheckBox.Enabled = autostartCheckBox.Checked;

            // Menu
            UpdateLanguageMenu();

            // Version info
            var versionstr = ProductVersion;
#if PORTABLE
            versionstr += " " + Resources.str_portable;
#endif
#if DEBUG
            versionstr += " (debug build)";
#endif
            version.Text = string.Format(Resources.str_version, versionstr);
            version.Links.Add(0, version.Text.Length, "https://github.com/eltos/PasteIntoFile/releases");
            version.LinkClicked += (sender, args) => Process.Start(args.Link.LinkData.ToString());
            var backgroundTask = CheckForUpdates();

            // Dark theme
            if (RegistryUtil.IsDarkMode()) {
                MakeDarkMode();
            }
        }
        private void UpdateLanguageMenu() {
            var dir = Path.GetDirectoryName(Application.ExecutablePath);
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(culture => culture.Name == "en" || Directory.Exists(Path.Combine(dir, culture.Name)));

            settingsMenuLanguage.DropDownItems.Clear();
            foreach (var culture in cultures) {
                var flag = string.Concat(culture.TwoLetterISOLanguageName.ToUpperInvariant().Select(x => char.ConvertFromUtf32(x + 0x1F1A5))); // + "\ufe0f"
                var description = Equals(culture, CultureInfo.InvariantCulture) ? Resources.str_system_language + @" 💻️"
                    : culture.DisplayName + @" – " + culture.NativeName + @" [" + culture.Name + @"]";

                var item = new ToolStripMenuItem(description, null, (sender, args) => {
                    Settings.Default.language = culture.Name;
                    Settings.Default.Save();
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Settings.Default.language);
                    ReloadUi();
                    Height += 100;
                });

                item.Checked = Settings.Default.language == culture.Name;
                settingsMenuLanguage.DropDownItems.Add(item);

            }

        }

        async Task CheckForUpdates() {
            if (await Program.CheckForUpdates()) {
                // Update available
                version.Links.Clear();
                version.Text += string.Format("\n<a>{0}</a>", string.Format(Resources.str_version_update_available, ProductVersion, Settings.Default.updateLatestVersion));
                var linkStart = version.Text.IndexOf(@"<a>", StringComparison.Ordinal);
                var linkEnd = version.Text.IndexOf(@"</a>", StringComparison.Ordinal);
                if (linkStart >= 0 && linkEnd > linkStart) {
                    version.Text = version.Text.Remove(linkStart, 3).Remove(linkEnd - 3, 4);
                    version.Links.Add(linkStart, linkEnd - linkStart - 3, Settings.Default.updateLatestVersionLink);
                }
            }
        }

        public static void SetAutosaveMode(bool enabled) {
            Settings.Default.autoSave = enabled;
            Settings.Default.Save();
            // update context menu entries with or without ellipsis
            RegistryUtil.ReRegisterContextMenuEntries();
        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.autoSave != autoSaveCheckBox.Checked) {
                SetAutosaveMode(autoSaveCheckBox.Checked);
                SavedAnimation(autoSaveCheckBox);
            }
        }

        private void ChkAutoSaveMayOpenNew_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.autoSaveMayOpenNewExplorer != autoSaveMayOpenNewCheckBox.Checked) {
                Settings.Default.autoSaveMayOpenNewExplorer = autoSaveMayOpenNewCheckBox.Checked;
                Settings.Default.Save();
                SavedAnimation(autoSaveMayOpenNewCheckBox);
            }
        }


        private void ChkContextEntry_CheckedChanged(object sender, EventArgs e) {
            var checkBox = sender as CheckBox;
            RegistryUtil.ContextMenuEntry entry;
            if (sender == contextEntryCheckBoxPaste) {
                entry = RegistryUtil.ContextMenuPaste;
            } else if (sender == contextEntryCheckBoxCopy) {
                entry = RegistryUtil.ContextMenuCopy;
            } else if (sender == contextEntryCheckBoxReplace) {
                entry = RegistryUtil.ContextMenuReplace;
            } else {
                return;
            }
            try {
                if (checkBox.Checked && !entry.IsRegistered()) {
                    entry.Register();
                    SavedAnimation(checkBox);
                } else if (!checkBox.Checked && entry.IsRegistered()) {
                    entry.UnRegister();
                    SavedAnimation(checkBox);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SavedAnimation(Control control) {
            control.Text += @" ✔";
            control.ForeColor = DarkMode ? Color.LawnGreen : Color.Green;
            Task.Delay(1000).ContinueWith(t => {
                control.ForeColor = TextColor;
                control.Text = control.Text.Trim('✔').Trim();
            });
        }

        private void ChkAutostart_CheckedChanged(object sender, EventArgs e) {
            patchingCheckBox.Enabled = autostartCheckBox.Checked;
            try {
                if (autostartCheckBox.Checked && !RegistryUtil.IsAutostartRegistered()) {
                    RegistryUtil.RegisterAutostart();
                    SavedAnimation(autostartCheckBox);
                } else if (!autostartCheckBox.Checked && RegistryUtil.IsAutostartRegistered()) {
                    RegistryUtil.UnRegisterAutostart();
                    patchingCheckBox.Checked = false;
                    SavedAnimation(autostartCheckBox);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkPatching_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.trayPatchingEnabled != patchingCheckBox.Checked) {
                Settings.Default.trayPatchingEnabled = patchingCheckBox.Checked;
                Settings.Default.Save();
                // no SavedAnimation so as not to give the feeling this would take effect immediately
            }
        }

        private void settingsButton_Click(object sender, EventArgs e) {
            // update
            settingsMenuUpdateChecks.Checked = Settings.Default.updateChecksEnabled;
            settingsMenuClearClipboard.Checked = Settings.Default.clrClipboard;
            // show it
            Point ptLowerLeft = new Point(4, settingsButton.Height);
            ptLowerLeft = settingsButton.PointToScreen(ptLowerLeft);
            settingsMenu.Show(ptLowerLeft);
        }

        private void menuUpdateChecks_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.updateChecksEnabled != settingsMenuUpdateChecks.Checked) {
                Settings.Default.updateChecksEnabled = settingsMenuUpdateChecks.Checked;
                Settings.Default.Save();
            }
        }

        private void menuClearClipboard_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.clrClipboard != settingsMenuClearClipboard.Checked) {
                Settings.Default.clrClipboard = settingsMenuClearClipboard.Checked;
                Settings.Default.Save();
            }
        }

        private void finish_Click(object sender, EventArgs e) {
            Settings.Default.firstLaunch = false;
            Settings.Default.Save();
            Close();
        }

        private void Wizard_Shown(object sender, EventArgs e) {
            // Auto size dialog height
            // All tableLayout rows are set to 'autosize' except for the last -> it's height is a measure for leftover space
            Height -= tableLayoutPanel1.GetRowHeights()[tableLayoutPanel1.RowCount - 1];
            MinimumSize = Size;
        }
    }
}
