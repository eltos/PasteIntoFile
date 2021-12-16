using System;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class Wizzard : MasterForm
    {
        public Wizzard()
        {
            InitializeComponent();
            
            foreach (Control element in GetAllChild(this))
            {
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_wizzard_title, Resources.str_wizzard_contextentry_title, Resources.str_wizzard_contextentry_info, Resources.str_wizzard_contextentry_button, Resources.str_wizzard_autosave_title, Resources.str_wizzard_autosave_info, Resources.str_wizzard_autosave_button, Resources.str_wizzard_finish };
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }
            
            Icon = Resources.icon;
            Text = Resources.str_main_window_title;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Program.RegisterApp())
            {
                button1.Text += " ✓";
                button1.Enabled = false;
            }
        }

        private void finish_Click(object sender, EventArgs e)
        {
            Settings.Default.firstLaunch = false;
            Settings.Default.Save();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.autoSave = true;
            Settings.Default.Save();
            button2.Text += " ✓";
            button2.Enabled = false;
        }

        private void Wizzard_Shown(object sender, EventArgs e)
        {
            // Auto size dialog height
            // All tableLayout rows are set to 'autosize' except for the last -> it's height is a measure for leftover space 
            Height -= tableLayoutPanel1.GetRowHeights()[tableLayoutPanel1.RowCount - 1];
            MinimumSize = Size;
        }
    }
}
