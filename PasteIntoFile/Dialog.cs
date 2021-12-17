using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PasteIntoFile.Properties;
using WK.Libraries.BetterFolderBrowserNS;

namespace PasteIntoFile
{
    public partial class Dialog : MasterForm
    {
        public const string DefaultFilenameFormat = "yyyy-MM-dd HH-mm-ss";
        private string text;
        private Image image;
        
        public Dialog(string location, string filename = null, bool forceShowDialog = false)
        {
            // always show GUI if shift pressed during start
            forceShowDialog |= ModifierKeys == Keys.Shift;
            
            // Setup GUI
            InitializeComponent();
            
            foreach (Control element in GetAllChild(this))
            {
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_filename, Resources.str_extension, Resources.str_location, Resources.str_clear_clipboard, Resources.str_save, Resources.str_preview, Resources.str_main_info, Resources.str_autosave_checkbox, Resources.str_contextentry_checkbox };
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

            }

            //

            var filenameFormat = string.IsNullOrWhiteSpace(Settings.Default.filenameTemplate) ? DefaultFilenameFormat : Settings.Default.filenameTemplate;
            txtFilename.Text = DateTime.Now.ToString(filenameFormat);
            txtCurrentLocation.Text = Path.GetFullPath(location);
            chkClrClipboard.Checked = Settings.Default.clrClipboard;
            chkAutoSave.Checked = Settings.Default.autoSave;
            chkContextEntry.Checked = Program.IsAppRegistered();
            
            
            if (Clipboard.ContainsText())
            {
                text = Clipboard.GetText();
                txtContent.Text = text;
                txtContent.Show();
                box.Text = string.Format(Resources.str_preview_text, text.Length, text.Split('\n').Length);
                comExt.Items.AddRange(new object[] {
                    "bat", "java", "js", "json", "cpp", "cs", "css", "csv", "html", "php", "ps1", "py", "txt"
                });
                comExt.Text = Settings.Default.extensionText == null ? "txt" : Settings.Default.extensionText;

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

            // second parameter can overwrite filename and -type
            if (filename != null)
            {
                var i = filename.LastIndexOf('.');
                if (i < 0)
                {
                    txtFilename.Text = filename;
                }
                else
                {
                    txtFilename.Text = filename.Substring(0, i);
                    comExt.Text = filename.Substring(i+1);
                }
            }
            

            // Pressed shift key resets autosave option
            if (forceShowDialog)
            {
                // Make sure to bring window to foreground (holding shift will open window in background)
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
                    ExplorerUtil.RequestFilenameEdit(file);
                    
                    var message = string.Format(Resources.str_autosave_balloontext, file);
                    Program.ShowBalloon(Resources.str_autosave_balloontitle, message, 10_000);

                    Environment.Exit(0);
                }
            }
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
            string dirname = Path.GetFullPath(txtCurrentLocation.Text);
            string filename = txtFilename.Text + (txtFilename.Text.EndsWith("." + comExt.Text) ? "" : "." + comExt.Text);
            string file = Path.Combine(dirname, filename);
            
            // check if file exists
            if (File.Exists(file))
            {
                var result = MessageBox.Show(string.Format(Resources.str_file_exists, file), Resources.str_main_window_title,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return null;
                }
            } else if (Directory.Exists(file))
            {
                MessageBox.Show(string.Format(Resources.str_file_exists_directory, file), Resources.str_main_window_title,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            
            
            try
            {
                // create folders if required
                Directory.CreateDirectory(dirname);

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
                Program.RestartAppElevated(dirname);
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
            betterFolderBrowser.RootFolder = txtCurrentLocation.Text ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog(this) == DialogResult.OK)
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
                MessageBox.Show(Resources.str_autosave_infotext, Resources.str_autosave_checkbox, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            Settings.Default.autoSave = chkAutoSave.Checked;
            Settings.Default.Save();

        }

        private void ChkContextEntry_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContextEntry.Checked && !Program.IsAppRegistered())
            {
                Program.RegisterApp();
            }
            else if (!chkContextEntry.Checked && Program.IsAppRegistered())
            {
                Program.UnRegisterApp();
            }
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