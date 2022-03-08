using System;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class Wizard : MasterForm
    {
        public Wizard()
        {
            InitializeComponent();
            
            foreach (Control element in GetAllChild(this))
            {
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_wizard_title, Resources.str_wizard_contextentry_title, Resources.str_wizard_contextentry_info, Resources.str_wizard_contextentry_button, Resources.str_wizard_autosave_title, Resources.str_wizard_autosave_info, Resources.str_wizard_autosave_button, Resources.str_wizard_finish };
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }
            
            Icon = Resources.icon;
            Text = Resources.str_main_window_title;

            version.Text = string.Format(Resources.str_version, ProductVersion);

            autoSaveCheckBox.Checked = Settings.Default.autoSave;
            contextEntryCheckBox.Checked = RegistryUtil.IsContextMenuEntryRegistered();
            autostartCheckBox.Checked = RegistryUtil.IsAutostartRegistered();
        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.autoSave = autoSaveCheckBox.Checked;
            Settings.Default.Save();

        }

        private void ChkContextEntry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (contextEntryCheckBox.Checked && !RegistryUtil.IsContextMenuEntryRegistered())
                {
                    RegistryUtil.RegisterContextMenuEntry();
                    MessageBox.Show(Resources.str_message_register_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (!contextEntryCheckBox.Checked && RegistryUtil.IsContextMenuEntryRegistered())
                {
                    RegistryUtil.UnRegisterContextMenuEntry();
                    MessageBox.Show(Resources.str_message_unregister_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkAutostart_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (autostartCheckBox.Checked && !RegistryUtil.IsAutostartRegistered())
                {
                    RegistryUtil.RegisterAutostart();
                    MessageBox.Show(Resources.str_message_register_autostart_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (!autostartCheckBox.Checked && RegistryUtil.IsAutostartRegistered())
                {
                    RegistryUtil.UnRegisterAutostart();
                    MessageBox.Show(Resources.str_message_unregister_autostart_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void finish_Click(object sender, EventArgs e)
        {
            Settings.Default.firstLaunch = false;
            Settings.Default.Save();
            Close();
        }

        private void Wizard_Shown(object sender, EventArgs e)
        {
            // Auto size dialog height
            // All tableLayout rows are set to 'autosize' except for the last -> it's height is a measure for leftover space 
            Height -= tableLayoutPanel1.GetRowHeights()[tableLayoutPanel1.RowCount - 1];
            MinimumSize = Size;
        }
    }
}
