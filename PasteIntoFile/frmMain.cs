using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;

namespace PasteIntoFile
{
    public partial class frmMain : Form
    {
        public string CurrentLocation { get; set; }
        public bool IsText { get; set; }
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain(string location)
        {
            InitializeComponent();
            this.CurrentLocation = location;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.darkTheme)
            {
                BackColor = Color.FromArgb(0, 0, 0);

                foreach (Label lbl in this.Controls.OfType<Label>())
                {
                    lbl.ForeColor = Color.FromArgb(255, 255, 255);
                }

                foreach (TextBox txt in this.Controls.OfType<TextBox>())
                {
                    txt.ForeColor = Color.FromArgb(255, 255, 255);
                    txt.BackColor = Color.FromArgb(43, 43, 43);
                }

                foreach (ComboBox cmb in this.Controls.OfType<ComboBox>())
                {
                    cmb.ForeColor = Color.FromArgb(255, 255, 255);
                    cmb.BackColor = Color.FromArgb(43, 43, 43);
                }

                foreach (LinkLabel lnk in this.Controls.OfType<LinkLabel>())
                {
                    lnk.LinkColor = Color.FromArgb(255, 255, 255);
                }

                foreach (CheckBox chb in this.Controls.OfType<CheckBox>())
                {
                    chb.ForeColor = Color.FromArgb(255, 255, 255);
                }

                foreach (Button btn in this.Controls.OfType<Button>())
                {
                    btn.ForeColor = Color.FromArgb(255, 255, 255);
                    btn.BackColor = Color.FromArgb(43, 43, 43);
                }
            }

            txtFilename.Text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            txtCurrentLocation.Text = CurrentLocation ?? @Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            clrClipboard.Checked = Properties.Settings.Default.clrClipboard;

            /*if (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\Paste Into File\command", "", null) == null)
            {
                if (MessageBox.Show("Seems that you are running this application for the first time,\nDo you want to register it with your system?", "Paste Into File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.RegisterApp();
                }
            }*/

            if (Clipboard.ContainsText())
            {
                lblType.Text = "Text";
                comExt.SelectedItem = "txt";
                IsText = true;
                txtContent.Text = Clipboard.GetText();
                return;
            }

            if (Clipboard.ContainsImage())
            {
                lblType.Text = "Image";
                comExt.SelectedItem = "png";
                imgContent.Image = Clipboard.GetImage();
                return;
            }

            lblType.Text = "Unknown";
            btnSave.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string location = txtCurrentLocation.Text;
            location = location.EndsWith("\\") ? location : location + "\\";
            string filename = txtFilename.Text + "." + comExt.SelectedItem.ToString();
            if (IsText)
            {
                File.WriteAllText(location + filename, txtContent.Text, Encoding.UTF8);
            }
            else
            {
                switch (comExt.SelectedItem.ToString())
                {
                    case "png":
                        imgContent.Image.Save(location + filename, ImageFormat.Png);
                        break;
                    case "ico":
                        imgContent.Image.Save(location + filename, ImageFormat.Icon);
                        break;
                    case "jpg":
                        imgContent.Image.Save(location + filename, ImageFormat.Jpeg);
                        break;
                    case "bmp":
                        imgContent.Image.Save(location + filename, ImageFormat.Bmp);
                        break;
                    case "gif":
                        imgContent.Image.Save(location + filename, ImageFormat.Gif);
                        break;
                    default:
                        imgContent.Image.Save(location + filename, ImageFormat.Png);
                        break;
                }
            }

            if (clrClipboard.Checked)
            {
                Clipboard.Clear();
            }

            Environment.Exit(0);
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e)
        {
            /*FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a folder for saving this file";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtCurrentLocation.Text = fbd.SelectedPath;
            }*/

            BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser();

            betterFolderBrowser.Title = "Select folder...";
            betterFolderBrowser.RootFolder = @Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtCurrentLocation.Text = betterFolderBrowser.SelectedFolder;
            }
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("http://eslamx.com");
        }

        private void lblMe_Click(object sender, EventArgs e)
        {
            Process.Start("http://twitter.com/EslaMx7");
        }

        private void lblHelp_Click(object sender, EventArgs e)
        {
            string msg = "Paste Into File helps you paste any text or images in your system clipboard into a file directly instead of creating new file yourself";
            msg += "\n--------------------\nTo Register the application to your system Context Menu run the program as Administrator with this argument : /reg";
            msg += "\nto Unregister the application use this argument : /unreg\n";
            msg += "\n--------------------\nSend Feedback to : contact@francescosorge.com\n\nThanks :)";
            MessageBox.Show(msg, "Paste As File Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtFilename_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSave_Click(sender, null);
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.francescosorge.com/");
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.RegisterApp();
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.UnRegisterApp();
        }

        private void ClrClipboard_CheckedChanged(object sender, EventArgs e)
        {
            PasteIntoFile.Properties.Settings.Default.clrClipboard = clrClipboard.Checked;
            PasteIntoFile.Properties.Settings.Default.Save();
        }
    }
}
