using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using PasteIntoFile.Properties;
using WK.Libraries.BetterFolderBrowserNS;

namespace PasteIntoFile
{
    public partial class frmMain : Form
    {
        public const string DefaultFilenameFormat = "yyyy-MM-dd HH-mm-ss";
        private string text;
        private Image image;
        
        public frmMain(string location = null)
        {
            location = location ?? @Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            // Setup GUI
            InitializeComponent();
            
            foreach (Control element in GetAllChild(this))
            {
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }
            
            Icon = Resources.icon;
            Text = Resources.str_main_window_title;
            infoLabel.Text = string.Format(Resources.str_version, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            

            // Dark theme
            if (Settings.Default.darkTheme)
            {
                foreach (Control element in GetAllChild(this))
                {
                    element.ForeColor = Color.White;
                    element.BackColor = Color.FromArgb(53, 53, 53);
                }
                
                BackColor = Color.FromArgb(24, 24, 24);
                linkRegister.ForeColor = Color.LightBlue;
                linkUnregister.ForeColor = Color.LightBlue;

            }

            //

            var filenameFormat = Settings.Default.filenameTemplate ?? DefaultFilenameFormat;
            txtFilename.Text = DateTime.Now.ToString(filenameFormat);
            txtCurrentLocation.Text = location;
            chkClrClipboard.Checked = Settings.Default.clrClipboard;
            chkAutoSave.Checked = Settings.Default.autoSave;
            
            
            if (Clipboard.ContainsText())
            {
                text = Clipboard.GetText();
                txtContent.Text = text;
                txtContent.Show();
                box.Text = string.Format(Resources.str_preview_text, text.Length, text.Split('\n').Length);
                comExt.Items.AddRange(new object[] {
                    "bat", "java", "js", "json", "cpp", "cs", "css", "csv", "html", "php", "ps1", "py", "txt"
                });
                comExt.Text = Settings.Default.extensionText ?? "txt";

            }
            else if (Clipboard.ContainsImage())
            {
                image = Clipboard.GetImage();
                imgContent.BackgroundImage = image;
                imgContent.Show();
                box.Text = string.Format(Resources.str_preview_image, image.Width, image.Height);
                comExt.Items.AddRange(new object[] {
                    "bpm", "emf", "gif", "ico", "jpg", "png", "tif", "wmf"
                });
                comExt.DropDownStyle = ComboBoxStyle.DropDownList; // prevent custom formats
                comExt.SelectedItem = comExt.Items.Contains(Settings.Default.extensionImage) ? Settings.Default.extensionImage : "png";
                
            }

            // Pressed shift key resets autosave option
            if (ModifierKeys == Keys.Shift)
            {
                // Make sure to bring window to foreground
                WindowState = FormWindowState.Minimized;
                Show();
                BringToFront();
                WindowState = FormWindowState.Normal;
            }
            // otherwise perform autosave if enabled
            else if (chkAutoSave.Checked)
            {
                var file = save();
                if (file != null)
                {
                    var message = string.Format(Resources.str_autosave_balloontext, file);
                    Program.ShowBalloon(Resources.str_autosave_balloontitle, message, 10_000);

                    Environment.Exit(0);
                }
            }
        }
        
        public IEnumerable<Control> GetAllChild(Control control, Type type = null)
        {
            var controls = control.Controls.Cast<Control>();
            var enumerable = controls as Control[] ?? controls.ToArray();
            return enumerable.SelectMany(ctrl => GetAllChild(ctrl, type))
                .Concat(enumerable)
                .Where(c => type == null || type == c.GetType());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (save() != null)
            {
                Environment.Exit(0);
            }
        }
        
        string save()
        {


            string filename = txtFilename.Text + (txtFilename.Text.EndsWith("." + comExt.Text) ? "" : "." + comExt.Text);
            string file = Path.Combine(txtCurrentLocation.Text, filename);
            
            // check if file exists
            if (File.Exists(file))
            {
                var result = MessageBox.Show(string.Format("The file {0} already exists.\nDo you want to overwrite it?", file), "File exists",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result != DialogResult.Yes)
                {
                    return null;
                }
            }
            
            try
            {
                if (text != null)
                {
                    File.WriteAllText(file, text, Encoding.UTF8);
                }
                else if (image != null)
                {
                    ImageFormat format;
                    switch (comExt.Text)
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

                    image.Save(file, format);
                }

                if (chkClrClipboard.Checked)
                {
                    Clipboard.Clear();
                }
                
                return file;

            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.RestartAppElevated(txtCurrentLocation.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return null;
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e)
        {
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

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Environment.Exit(0);
            }
        }

        private void ChkClrClipboard_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.clrClipboard = chkClrClipboard.Checked;
            Settings.Default.Save();
        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoSave.Checked && !Settings.Default.autoSave)
            {
                MessageBox.Show(Resources.str_autosave_infotext, Resources.str_autosave, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            Settings.Default.autoSave = chkAutoSave.Checked;
            Settings.Default.Save();

        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.RegisterApp();
        }

        private void linkUnregister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.UnRegisterApp();
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.str_main_info_url);
        }

        private void comExt_Update(object sender, EventArgs e)
        {
            if (text != null)
            {
                Settings.Default.extensionText = comExt.Text;
            }
            else if (image != null)
            {
                Settings.Default.extensionImage = comExt.Text;
            }
            Settings.Default.Save();
        }
    }
}