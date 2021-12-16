using System;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class Wizzard : Form
    {
        public Wizzard()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            first.Hide();
            second.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Program.RegisterApp();
            Settings.Default.firstLaunch = false;
            Settings.Default.Save();
            Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Settings.Default.firstLaunch = false;
            Settings.Default.Save();
            Close();
        }
    }
}
