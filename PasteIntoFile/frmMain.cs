using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class frmMain : Form
    {
        public const string DEFAULT_FILENAME_FORMAT = "yyyy-MM-dd HH-mm-ss";
        public string CurrentLocation { get; set; }
        public bool IsText { get; set; }
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain(string location)
        {
            InitializeComponent();
            CurrentLocation = location;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Settings.Default.darkTheme)
            {
                BackColor = Color.FromArgb(0, 0, 0);

                foreach (Label lbl in Controls.OfType<Label>())
                {
                    lbl.ForeColor = Color.FromArgb(255, 255, 255);
                }

                foreach (TextBox txt in Controls.OfType<TextBox>())
                {
                    txt.ForeColor = Color.FromArgb(255, 255, 255);
                    txt.BackColor = Color.FromArgb(43, 43, 43);
                }

                foreach (ComboBox cmb in Controls.OfType<ComboBox>())
                {
                    cmb.ForeColor = Color.FromArgb(255, 255, 255);
                    cmb.BackColor = Color.FromArgb(43, 43, 43);
                }

                foreach (LinkLabel lnk in Controls.OfType<LinkLabel>())
                {
                    lnk.LinkColor = Color.FromArgb(255, 255, 255);
                }

                foreach (CheckBox chb in Controls.OfType<CheckBox>())
                {
                    chb.ForeColor = Color.FromArgb(255, 255, 255);
                }

                foreach (Button btn in Controls.OfType<Button>())
                {
                    btn.ForeColor = Color.FromArgb(255, 255, 255);
                    btn.BackColor = Color.FromArgb(43, 43, 43);
                }
            }

            string filename = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\shell\"+Program.RegistrySubKey+@"\filename", "", null) ?? DEFAULT_FILENAME_FORMAT;
            txtFilename.Text = DateTime.Now.ToString(filename);
            txtCurrentLocation.Text = CurrentLocation ?? @"C:\";
            txtCurrentLocation.Text = CurrentLocation ?? @Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            clrClipboard.Checked = Settings.Default.clrClipboard;
            autoSave.Checked = Settings.Default.autoSave;

            /*if (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\"+Program.RegistrySubKey+@"\command", "", null) == null)
            {
                if (MessageBox.Show(Resources.str_message_first_time, Resources.window_title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.RegisterApp();
                }
            }*/

            if (Clipboard.ContainsText())
            {
                lblType.Text = Resources.str_type_txt;
                comExt.SelectedItem = "txt";
                IsText = true;
                txtContent.Text = Clipboard.GetText();
            }
            else if (Clipboard.ContainsImage())
            {
                lblType.Text = Resources.str_type_img;
                comExt.SelectedItem = "png";
                imgContent.Show();
                imgContent.BackgroundImage = Clipboard.GetImage();
                if (imgContent.BackgroundImage.Width*1.0/imgContent.BackgroundImage.Height > imgContent.Width*1.0/imgContent.Height)
                {
                    imgContent.Height = imgContent.Width * imgContent.BackgroundImage.Height / imgContent.BackgroundImage.Width;
                }
                else
                {
                    var newWidth = imgContent.Height * imgContent.BackgroundImage.Width / imgContent.BackgroundImage.Height;
                    imgContent.Left += (imgContent.Width - newWidth) / 2;
                    imgContent.Width = newWidth;
                }
                Height += imgContent.Height;
                CenterToScreen();
            }
            else
            {
                lblType.Text = Resources.str_type_unknown;
                btnSave.Enabled = false;
            }

            if (autoSave.Checked && btnSave.Enabled)
            {
                btnSave.PerformClick();
            }
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
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Png);
                        break;
                    case "ico":
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Icon);
                        break;
                    case "jpg":
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Jpeg);
                        break;
                    case "bmp":
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Bmp);
                        break;
                    case "gif":
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Gif);
                        break;
                    default:
                        imgContent.BackgroundImage.Save(location + filename, ImageFormat.Png);
                        break;
                }
            }

            if (clrClipboard.Checked)
            {
                Clipboard.Clear();
            }


            if (autoSave.Checked)
            {
                Program.ShowBalloon(Resources.str_autosave_balloontitle, string.Format(Resources.str_autosave_balloontext, txtCurrentLocation.Text + @"\" + txtFilename.Text + "." + comExt.Text));
                Thread.Sleep(5000);
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

            betterFolderBrowser.Title = Resources.str_select_folder;
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
            MessageBox.Show(Resources.str_message_help, Resources.str_main_window_title, MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Environment.Exit(0);
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://" + Resources.str_main_info_url);
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
            Settings.Default.clrClipboard = clrClipboard.Checked;
            Settings.Default.Save();
        }

        private void AutoSave_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.autoSave = autoSave.Checked;
            Settings.Default.Save();
        }

        private void AutoSave_Click(object sender, EventArgs e)
        {
            if (autoSave.Checked)
            {
                MessageBox.Show(string.Format(Resources.str_autosave_infotext, ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath), Resources.str_autosave, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
