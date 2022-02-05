using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PasteIntoFile.Properties;
using WK.Libraries.BetterFolderBrowserNS;
using WK.Libraries.SharpClipboardNS;

namespace PasteIntoFile
{
    public partial class Dialog : MasterForm
    {
        private ClipboardDataContainer clipData = new ClipboardDataContainer();
        private readonly SharpClipboard clipMonitor = new SharpClipboard();
        private bool continuousMode = false;
        private int saveCount = 0;
        
        
        private static readonly string[] IMAGE_FORMATS = {
            "png", "bpm", "emf", "gif", "ico", "jpg", "tif", "wmf"
        };

        private static readonly string[] HTML_FORMATS = {
            "html", "htm"
        };

        private static readonly string[] URL_FORMATS = {
            "url"
        };

        private static readonly string[] DEFAULT_TEXT_FORMATS = {
            "txt", "bat", "java", "js", "json", "cpp", "cs", "css", "csv", "md", "php", "ps1", "py"
        };


        public Dialog(string location, bool forceShowDialog = false)
        {
            // always show GUI if shift pressed during start
            forceShowDialog |= ModifierKeys == Keys.Shift;
            
            // Setup GUI
            InitializeComponent();
            
            foreach (Control element in GetAllChild(this))
            {
                if (element is WebBrowser) continue;
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_filename, Resources.str_extension, Resources.str_location, Resources.str_clear_clipboard, Resources.str_save, Resources.str_preview, Resources.str_main_info, Resources.str_autosave_checkbox, Resources.str_contextentry_checkbox, Resources.str_continuous_mode };
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }
            
            Icon = Resources.icon;
            Text = Resources.str_main_window_title;
            infoLabel.Text = string.Format(Resources.str_version, ProductVersion);
            

            // Dark theme
            if (RegistryUtil.IsDarkMode())
            {
                foreach (Control element in GetAllChild(this))
                {
                    element.ForeColor = Color.White;
                    element.BackColor = Color.FromArgb(53, 53, 53);
                }
                
                BackColor = Color.FromArgb(24, 24, 24);

            }

            // read clipboard and populate GUI
            comExt.Text = "*"; // force to use extension based on format type
            var clipRead = readClipboard();
            
            updateFilename();
            txtCurrentLocation.Text = Path.GetFullPath(location);
            chkClrClipboard.Checked = Settings.Default.clrClipboard;
            chkContinuousMode.Checked = continuousMode;
            updateSavebutton(); 
            chkAutoSave.Checked = Settings.Default.autoSave;
            chkContextEntry.Checked = RegistryUtil.IsAppRegistered();
            

            txtFilename.Select();

            // show dialog or autosave option
            if (forceShowDialog)
            {
                // Make sure to bring window to foreground (holding shift will open window in background)
                WindowState = FormWindowState.Minimized;
                Show();
                BringToFront();
                WindowState = FormWindowState.Normal;
            }
            // otherwise perform autosave if enabled
            else if (Settings.Default.autoSave)
            {
                var file = clipRead ? save() : null;
                if (file != null)
                {
                    ExplorerUtil.RequestFilenameEdit(file);
                    
                    var message = string.Format(Resources.str_autosave_balloontext, file);
                    Program.ShowBalloon(Resources.str_autosave_balloontitle, message, 10_000);

                    Environment.Exit(0);
                }
                else
                {
                    Environment.Exit(1);
                }

            }
            
            // register clipboard monitor
            clipMonitor.ClipboardChanged += ClipboardChanged;
            
        }

        public string formatFilenameTemplate(string template)
        {
            return String.Format(template, clipData.timestamp, saveCount);
        }
        public void updateFilename()
        {
            try
            {
                txtFilename.Text = formatFilenameTemplate(Settings.Default.filenameTemplate);
            }
            catch (FormatException)
            {
                txtFilename.Text = "filename_template_invalid";
            }
        }
        
        private bool readClipboard() {
            
            clipData = ClipboardDataContainer.fromCliboardData();

            // Update GUI elements
            comExt.Items.Clear();
            
            if (clipData.hasImage) {
                imgContent.BackgroundImage = clipData.image;
                comExt.Items.AddRange(IMAGE_FORMATS);
                comExt.Items.Add(""); // separator
            }
            
            if (clipData.hasHtml) {
                htmlContent.DocumentText = clipData.html;
                comExt.Items.AddRange(HTML_FORMATS);
                comExt.Items.Add(""); // separator
            }

            if (clipData.hasFiles && !clipData.hasText) {
                // use list of file paths as text
                clipData.text = string.Join("\n", clipData.files.Cast<string>().ToList());
            }
            
            if (clipData.hasText) {
                txtContent.Text = clipData.text;
                if (clipData.hasTextUrl) {
                    comExt.Items.AddRange(URL_FORMATS); 
                    comExt.Items.Add(""); // separator
                }
                comExt.Items.AddRange(DEFAULT_TEXT_FORMATS);
                comExt.Items.Add(""); // separator
            }
            
            
            
            // if selected extension does not match available contents, adjust it
            if (comExt.Text == "*" || comExt.Text == null ||
                saveAsImage() && !clipData.hasImage ||
                saveAsHtml() && !clipData.hasHtml && !clipData.hasText || // allow to save text as html file
                saveAsUrl() && !clipData.hasTextUrl ||
                saveAsText() && !clipData.hasText) 
            {
                // chose file extension based on available contents in this order
                if (clipData.hasImage)
                    comExt.Text = IMAGE_FORMATS.Contains(Settings.Default.extensionImage) ? Settings.Default.extensionImage : IMAGE_FORMATS[0];
                else if (clipData.hasText)
                    comExt.Text = Settings.Default.extensionText == null ? DEFAULT_TEXT_FORMATS[0] : Settings.Default.extensionText;
                else if (clipData.hasHtml)
                    comExt.Text = HTML_FORMATS[0];
                else
                    comExt.Text = "";
            }

            if (comExt.Items.Count > 0)
            {
                updateContents();
                return true;
            }
            
            MessageBox.Show(Resources.str_noclip_text, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
            
        }

        
        
        private void ClipboardChanged(Object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            var previousClipboardTimestamp = clipData.timestamp;
            readClipboard();

            // ignore duplicate clipboard content updates within 100ms
            if (continuousMode && (clipData.timestamp - previousClipboardTimestamp).TotalMilliseconds > 100)
            {
                updateFilename();
                save();
                updateSavebutton();
            }
        }

        /// <summary>
        /// Update content preview depending on available clipboard data and selected file extension
        /// </summary>
        private void updateContents() {
            
            txtContent.Hide();
            htmlContent.Hide();
            imgContent.Hide();

            if (saveAsImage() && clipData.hasImage) {
                imgContent.Show();
                box.Text = string.Format(Resources.str_preview_image, clipData.image.Width, clipData.image.Height);
            }
            else if (saveAsHtml() && clipData.hasHtml) {
                htmlContent.Show();
                box.Text = Resources.str_preview_html;
            }
            else if (saveAsUrl() && clipData.hasTextUrl) {
                txtContent.Show();
                box.Text = Resources.str_preview_url;
            }
            else if (saveAsTextLike() && clipData.hasText) { // text like, e.g. allow to save text as html file
                txtContent.Show();
                box.Text = string.Format(Resources.str_preview_text, clipData.text.Length, clipData.text.Split('\n').Length);
            }
            else {
                box.Text = Resources.str_error_cliboard_format_missmatch;
            }
            
        }
        private void updateSavebutton()
        {
            btnSave.Enabled = txtFilename.Enabled = !continuousMode;
            btnSave.Text = continuousMode ? string.Format(Resources.str_n_saved, saveCount) : Resources.str_save;
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
            try {
                string dirname = Path.GetFullPath(txtCurrentLocation.Text);
                string ext = comExt.Text.ToLowerInvariant();
                if (ext.StartsWith(".")) ext = ext.Substring(1);
                string filename = txtFilename.Text;
                if (!string.IsNullOrWhiteSpace(ext) && !filename.EndsWith("." + ext))
                    filename += "." + ext;
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
            
            
                // create folders if required
                Directory.CreateDirectory(dirname);

                if (saveAsImage()) {
                    if (!clipData.hasImage) {
                        MessageBox.Show(string.Format(Resources.str_error_save_no_image_data, ext), Resources.str_main_window_title,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    
                    ImageFormat imageFormat;
                    switch (ext)
                    {
                        case "bpm": imageFormat = ImageFormat.Bmp; break;
                        case "emf": imageFormat = ImageFormat.Emf; break;
                        case "gif": imageFormat = ImageFormat.Gif; break;
                        case "ico": imageFormat = ImageFormat.Icon; break;
                        case "jpg": imageFormat = ImageFormat.Jpeg; break;
                        case "tif": imageFormat = ImageFormat.Tiff; break;
                        case "wmf": imageFormat = ImageFormat.Wmf; break;
                        default: imageFormat = ImageFormat.Png; break;
                    }
                    clipData.image.Save(file, imageFormat);                        
                    
                }
                else if (saveAsUrl()) {
                    if (!clipData.hasTextUrl) {
                        MessageBox.Show(Resources.str_error_save_no_uri, Resources.str_main_window_title,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                            
                    File.WriteAllLines(file, new[] {
                        @"[InternetShortcut]",
                        @"URL=" + clipData.textUrl
                    }, Encoding.UTF8);
                    
                }
                else if (saveAsHtml() && clipData.hasHtml) {
                    var data = clipData.html;
                    if (!data.StartsWith("<!DOCTYPE html>"))
                        data = "<!DOCTYPE html>\n" + data;
                    File.WriteAllText(file, data, Encoding.UTF8);
                    
                }
                else if (saveAsTextLike() && clipData.hasText) // text like, e.g. allow to save text as html file
                {
                    File.WriteAllText(file, clipData.text, Encoding.UTF8);
                }
                else
                {
                    return null;
                }
                
                if (Settings.Default.clrClipboard)
                {
                    Clipboard.Clear();
                }

                saveCount++;
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
            betterFolderBrowser.RootFolder = txtCurrentLocation.Text ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                txtCurrentLocation.Text = betterFolderBrowser.SelectedFolder;
            }
        }

        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Escape)
            {
                Environment.Exit(1);
            }
        }

        private void ChkClrClipboard_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.clrClipboard = chkClrClipboard.Checked;
            Settings.Default.Save();
        }

        private void chkContinuousMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContinuousMode.Checked)
            {
                var saveNow = MessageBox.Show(Resources.str_continuous_mode_enabled_ask_savenow, Resources.str_continuous_mode, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (saveNow == DialogResult.Yes) // save current clipboard now
                {
                    updateFilename();
                    save();
                }
                else if (saveNow != DialogResult.No)
                    chkContinuousMode.Checked = false;
            } 
            
            continuousMode = chkContinuousMode.Checked;
            updateSavebutton();
                
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
            try
            {
                if (chkContextEntry.Checked && !RegistryUtil.IsAppRegistered())
                {
                    RegistryUtil.RegisterApp();
                    MessageBox.Show(Resources.str_message_register_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (!chkContextEntry.Checked && RegistryUtil.IsAppRegistered())
                {
                    RegistryUtil.UnRegisterApp();
                    MessageBox.Show(Resources.str_message_unregister_context_menu_success, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.str_main_info_url);
        }

        private void comExt_Update(object sender, EventArgs e) {

            updateContents();

            // remember user selected defaults for certain content types
            if (saveAsTextLike() && clipData.hasText)
                Settings.Default.extensionText = comExt.Text;
            if (saveAsImage() && clipData.hasImage)
                Settings.Default.extensionImage = comExt.Text;
            
            Settings.Default.Save();
        }
        
        private bool saveAsImage() => IMAGE_FORMATS.Contains(comExt.Text);

        private bool saveAsHtml() => HTML_FORMATS.Contains(comExt.Text);

        private bool saveAsUrl() => URL_FORMATS.Contains(comExt.Text);

        /// <summary>
        /// Checks for strict text extensions, i.e. no other known formats
        /// </summary>
        private bool saveAsText() {
            return saveAsTextLike() && !saveAsHtml() ;
        }
        /// <summary>
        /// Checks for text like extensions, i.e. including html formats, which can also contain text
        /// </summary>
        private bool saveAsTextLike() {
            return comExt.Text != null && comExt.Text != "*" && !saveAsImage() && !saveAsUrl();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dialog = new TemplateEdit();
            dialog.FormClosed += DialogOnFormClosed;
            dialog.ShowDialog(this);
        }

        private void DialogOnFormClosed(object sender, FormClosedEventArgs e)
        {
            updateFilename();
        }
    }
}