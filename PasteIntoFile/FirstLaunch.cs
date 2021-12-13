using System;
using System.Diagnostics;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class FirstLaunch : Form
    {
        public FirstLaunch()
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
            Properties.Settings.Default.firstLaunch = false;
            Properties.Settings.Default.Save();
            Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.firstLaunch = false;
            Properties.Settings.Default.Save();
            Close();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.str_firstlaunch_creator_link);
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.str_firstlaunch_maintainer_link);
        }
    }
}
