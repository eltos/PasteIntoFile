using Microsoft.Win32;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    public partial class frmMain : Form
    {
        public const string DefaultFilenameFormat = "yyyy-MM-dd HH-mm-ss";
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
                var dark1 = Color.FromArgb(24, 24, 24);
                var dark2 = Color.FromArgb(53, 53, 53);

                BackColor = dark1;

                foreach (Label lbl in Controls.OfType<Label>())
                {
                    lbl.ForeColor = Color.White;
                }

                foreach (TextBox txt in Controls.OfType<TextBox>())
                {
                    txt.ForeColor = Color.White;
                    txt.BackColor = dark2;
                }

                foreach (ComboBox cmb in Controls.OfType<ComboBox>())
                {
                    cmb.ForeColor = Color.White;
                    cmb.BackColor = dark2;
                }

                foreach (LinkLabel lnk in Controls.OfType<LinkLabel>())
                {
                    lnk.LinkColor = Color.LightBlue;
                }

                foreach (CheckBox chb in Controls.OfType<CheckBox>())
                {
                    chb.ForeColor = Color.White;
                }

                foreach (Button btn in Controls.OfType<Button>())
                {
                    btn.ForeColor = Color.White;
                    btn.BackColor = dark2;
                }
            }

            string filename = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\shell\"+Program.RegistrySubKey+@"\filename", "", null) ?? DefaultFilenameFormat;
            txtFilename.Text = DateTime.Now.ToString(filename);
            txtCurrentLocation.Text = CurrentLocation ?? @"C:\";
            txtCurrentLocation.Text = CurrentLocation ?? @Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            clrClipboard.Checked = Settings.Default.clrClipboard;
            autoSave.Checked = Settings.Default.autoSave;
            
            // Pressed shift key resets autosave option
            if (ModifierKeys == Keys.Shift)
            {
                autoSave.Checked = false;
                // Make sure to bring window to foreground
                WindowState = FormWindowState.Minimized;
                Show();
                WindowState = FormWindowState.Normal;
            }
            

            if (Clipboard.ContainsText())
            {
                txtContent.Text = Clipboard.GetText();
                IsText = true;
                comExt.Items.AddRange(new object[] {
                    "txt", "html", "js", "css", "csv", "json", "cs", "cpp", "java", "php", "py"
                });
                // heuristic to suggest filetype
                if (Regex.IsMatch(txtContent.Text, "<html.*>(.|\n)*</html>")) {
                    comExt.SelectedItem = "html";
                } else if (Regex.IsMatch(txtContent.Text, "^\\{(.|\n)*:(.|\n)*\\}$")) {
                    comExt.SelectedItem = "json";
                } else {
                    comExt.SelectedItem = "txt";
                }
                lblType.Text = Resources.str_type_txt;
            }
            else if (Clipboard.ContainsImage())
            {
                comExt.Items.AddRange(new object[] {
                    "bpm", "emf", "gif", "ico", "jpg", "png", "tif", "wmf"
                });
                comExt.DropDownStyle = ComboBoxStyle.DropDownList; // prevent custom formats
                comExt.SelectedItem = "png";
                lblType.Text = Resources.str_type_img;
                imgContent.Show();
                imgContent.BackgroundImage = Clipboard.GetImage();
                
                if (imgContent.BackgroundImage != null)
                {
                    if (imgContent.BackgroundImage.Width * 1.0 / imgContent.BackgroundImage.Height >
                        imgContent.Width * 1.0 / imgContent.Height)
                    {
                        imgContent.Height = imgContent.Width * imgContent.BackgroundImage.Height /
                                            imgContent.BackgroundImage.Width;
                    }
                    else
                    {
                        var newWidth = imgContent.Height * imgContent.BackgroundImage.Width /
                                       imgContent.BackgroundImage.Height;
                        imgContent.Left += (imgContent.Width - newWidth) / 2;
                        imgContent.Width = newWidth;
                    }
                    Height += imgContent.Height;
                }
                
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
            string filename = txtFilename.Text + "." + comExt.SelectedItem;
            try
            {
                if (IsText)
                {
                    File.WriteAllText(location + filename, txtContent.Text, Encoding.UTF8);
                }
                else
                {
                    ImageFormat format;
                    switch (comExt.SelectedItem.ToString())
                    {
                        case "bpm": format = ImageFormat.Bmp; break;
                        case "emf": format = ImageFormat.Emf; break;
                        case "gif": format = ImageFormat.Gif; break;
                        case "ico": format = ImageFormat.Icon; break;
                        case "jpg": format = ImageFormat.Jpeg; break;
                        case "tif": format = ImageFormat.Tiff; break;
                        case "wmf": format = ImageFormat.Wmf; break;
                        default: format = ImageFormat.Png; break;
                    }

                    imgContent.BackgroundImage.Save(location + filename, format);
                }

                if (clrClipboard.Checked)
                {
                    Clipboard.Clear();
                }


                if (autoSave.Checked && e == EventArgs.Empty)
                {
                    Program.ShowBalloon(Resources.str_autosave_balloontitle, string.Format(Resources.str_autosave_balloontext, txtCurrentLocation.Text + @"\" + txtFilename.Text + "." + comExt.Text), 
                        10_000);
                }

                Environment.Exit(0);
                
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            
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
            betterFolderBrowser.RootFolder = @Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtCurrentLocation.Text = betterFolderBrowser.SelectedFolder;
            }
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Environment.Exit(0);
            }
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
                MessageBox.Show(Resources.str_autosave_infotext, Resources.str_autosave, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
