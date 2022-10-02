using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    public partial class Wizard : MasterForm {
        public Wizard() {
            InitializeComponent();


            foreach (Control element in GetAllChild(this)) {
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_wizard_title, Resources.str_wizard_contextentry_title, Resources.str_wizard_contextentry_info, Resources.str_wizard_contextentry_button, Resources.str_wizard_autosave_title, Resources.str_wizard_autosave_info, Resources.str_wizard_autosave_button, Resources.str_wizard_tray_title, Resources.str_wizard_tray_info, Resources.str_wizard_tray_autostart_button, Resources.str_wizard_tray_patching_button, Resources.str_wizard_finish };
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }

            Icon = Resources.app_icon;
            Text = Resources.app_title;

            autoSaveCheckBox.Checked = Settings.Default.autoSave;
            contextEntryCheckBox.Checked = RegistryUtil.IsContextMenuEntryRegistered();
            autostartCheckBox.Checked = RegistryUtil.IsAutostartRegistered();
            patchingCheckBox.Checked = Settings.Default.trayPatchingEnabled;
            patchingCheckBox.Enabled = autostartCheckBox.Checked;

            // Version info
            version.Text = string.Format(Resources.str_version, ProductVersion);
            version.Links.Add(0, version.Text.Length, "https://github.com/eltos/PasteIntoFile/releases");
            version.LinkClicked += (sender, args) => Process.Start(args.Link.LinkData.ToString());
            CheckForUpdates();

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
            // update context menu entry
            if (RegistryUtil.IsContextMenuEntryRegistered())
                RegistryUtil.RegisterContextMenuEntry(!Settings.Default.autoSave);
        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e) {
            SetAutosaveMode(autoSaveCheckBox.Checked);
        }

        private void ChkContextEntry_CheckedChanged(object sender, EventArgs e) {
            try {
                if (contextEntryCheckBox.Checked && !RegistryUtil.IsContextMenuEntryRegistered()) {
                    RegistryUtil.RegisterContextMenuEntry(!Settings.Default.autoSave);
                    MessageBox.Show(Resources.str_message_register_context_menu_success, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else if (!contextEntryCheckBox.Checked && RegistryUtil.IsContextMenuEntryRegistered()) {
                    RegistryUtil.UnRegisterContextMenuEntry();
                    MessageBox.Show(Resources.str_message_unregister_context_menu_success, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkAutostart_CheckedChanged(object sender, EventArgs e) {
            patchingCheckBox.Enabled = autostartCheckBox.Checked;
            try {
                if (autostartCheckBox.Checked && !RegistryUtil.IsAutostartRegistered()) {
                    RegistryUtil.RegisterAutostart();
                    MessageBox.Show(Resources.str_message_register_autostart_success, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else if (!autostartCheckBox.Checked && RegistryUtil.IsAutostartRegistered()) {
                    RegistryUtil.UnRegisterAutostart();
                    MessageBox.Show(Resources.str_message_unregister_autostart_success, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkPatching_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.trayPatchingEnabled = patchingCheckBox.Checked;
            Settings.Default.Save();
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
