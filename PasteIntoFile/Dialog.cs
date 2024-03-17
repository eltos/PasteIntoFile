using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PasteIntoFile.Properties;
using WK.Libraries.BetterFolderBrowserNS;
using WK.Libraries.SharpClipboardNS;

namespace PasteIntoFile {
    public partial class Dialog : MasterForm {
        private ClipboardContents clipData = new ClipboardContents();
        private int saveCount = 0;
        private bool _formLoaded = false;

        private SharpClipboard _clipMonitor;
        private bool disableUiEvents = false;

        public SharpClipboard clipMonitor {
            get {
                if (_clipMonitor == null) _clipMonitor = new SharpClipboard();
                return _clipMonitor;
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e) {
            // leave the clipboard monitoring chain in a clean way, otherwise the chain will break when the program exits
            clipMonitor?.StopMonitoring();
            base.OnFormClosed(e);
        }



        public Dialog(
            string location = null,
            string filename = null,
            bool? showDialogOverwrite = null,
            bool? clearClipboardOverwrite = null,
            bool overwriteIfExists = false,
            bool append = false
        ) {
            Settings.Default.Reload(); // load modifications made from other instance
            Settings.Default.continuousMode = false; // always start in normal mode
            Settings.Default.Save();

            // Fallback to default path
            location = (location ?? ExplorerUtil.GetActiveExplorerPath() ??
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                .Trim().Trim("\"".ToCharArray()); // remove trailing " fixes paste in root dir
            try {
                location = Path.GetFullPath(location);
            } catch { // ignored
            }

            // flag if key is pressed during start
            bool invertAutosave = (ModifierKeys & Keys.Shift) == Keys.Shift;
            bool saveIntoSubdir = (ModifierKeys & Keys.Control) == Keys.Control;

            // Setup GUI
            InitializeComponent();
            AllowAlwaysOnTop();

            Icon = Resources.app_icon;
            Text = Resources.app_title;
            var versionstr = ProductVersion;
#if DEBUG
            versionstr += "D";
#endif
#if PORTABLE
            versionstr += " " + Resources.str_portable;
#endif
            versionInfoLabel.Text = string.Format(Resources.str_version, versionstr);
            versionInfoLabel.LinkArea = new LinkArea(0, 0);
            var backgroundTask = CheckForUpdates();


            // Dark theme
            if (RegistryUtil.IsDarkMode()) {
                MakeDarkMode();
            }

            // read clipboard and populate GUI
            if (!string.IsNullOrWhiteSpace(Path.GetExtension(filename))) {
                // filename contains extension, so use that
                comExt.Text = Path.GetExtension(filename).Trim('.');
                filename = Path.GetFileNameWithoutExtension(filename);
            } else {
                // use extension based on format type
                comExt.Text = "";
            }

            var clipRead = readClipboard(); // will set extension automatically if empty

            updateFilename(filename);

            if (saveIntoSubdir) location += @"\" + formatFilenameTemplate(Settings.Default.subdirTemplate);
            txtCurrentLocation.Text = location;

            chkAppend.Checked = append;  // non-persistent setting
            updateUiFromSettings();
            Settings.Default.PropertyChanged += (sender, args) => updateUiFromSettings();


            txtFilename.Select();

            // show dialog or perform autosave
            var showDialog = !(Settings.Default.autoSave ^ invertAutosave);
            if (showDialogOverwrite != null) showDialog = (bool)showDialogOverwrite;
            if (showDialog) {
                // Make sure to bring window to foreground (holding shift will open window in background)
                BringToFrontForced();

                // register clipboard monitor
                clipMonitor.ClipboardChanged += ClipboardChanged;
                FormClosing += (s, e) => clipMonitor.ClipboardChanged -= ClipboardChanged;


            } else {
                // directly save without showing a dialog
                Opacity = 0; // prevent dialog from showing up for a fraction of a second

                var file = clipRead ? save(overwriteIfExists, clearClipboardOverwrite) : null;
                if (file != null) {
                    Environment.ExitCode = 0;

                    if (!chkAppend.Checked) {
                        // select file in explorer for rename and exit afterwards
                        ExplorerUtil.FilenameEditComplete += (sender, args) => {
                            CloseAsSoonAsPossible();
                        };
                        if (ExplorerUtil.AsyncRequestFilenameEdit(file, Settings.Default.autoSaveMayOpenNewExplorer)) {
                            // Wait for FilenameEditComplete event, but timeout after 3s in case it fails
                            Task.Delay(new TimeSpan(0, 0, 3)).ContinueWith(o => CloseAsSoonAsPossible());
                        } else {
                            // No event expected, close immediately (e.g. if explorer may not be opened)
                            CloseAsSoonAsPossible();
                        }

                    } else {
                        CloseAsSoonAsPossible();
                    }

                } else {
                    // save failed, exit with error code
                    Environment.ExitCode = 1;
                    CloseAsSoonAsPossible();
                }

            }

        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            _formLoaded = true;
        }

        /// <summary>
        /// Close the form as soon as possible.
        /// Unless <code>Close()</code> this method is save to call from within the constructor.
        /// See https://stackoverflow.com/questions/3067901
        /// </summary>
        private void CloseAsSoonAsPossible() {
            if (_formLoaded) { // Already loaded, save to call Close
                Close();
            } else { // Close once loading completed
                Load += (sender, args) => Close();
            }
        }

        private void updateUiFromSettings() {
            disableUiEvents = true;
            chkContinuousMode.Checked = Settings.Default.continuousMode;
            chkAutoSave.Checked = Settings.Default.autoSave;
            updateSavebutton();
            disableUiEvents = false;
        }

        public static string formatFilenameTemplate(string template, DateTime timestamp, int count) {
            return String.Format(template, timestamp, count);
        }
        public string formatFilenameTemplate(string template) {
            return formatFilenameTemplate(template, clipData.Timestamp, saveCount);
        }
        public void updateFilename(string filenameTemplate = null) {
            try {
                txtFilename.Text = formatFilenameTemplate(filenameTemplate ?? Settings.Default.filenameTemplate);
            } catch (FormatException) {
                txtFilename.Text = "filename_template_invalid";
            }
        }

        /// <summary>
        /// Determine the extension to use based on user settings and defaults
        /// </summary>
        /// <param name="content">Clipboard content for which to determine extension</param>
        /// <returns>Extension</returns>
        public static string determineExtension(ClipboardContents clipData) {
            // Determines primary data in clipboard according to a custom prioritisation order
            BaseContent content = clipData.ForContentType(typeof(ImageLikeContent)) ??
                                  clipData.ForContentType(typeof(TextContent)) ??
                                  clipData.ForContentType(typeof(BaseContent));

            // chose file extension based on user preference if available
            if (content is ImageContent && content.Extensions.Contains(Settings.Default.extensionImage))
                return Settings.Default.extensionImage;
            if (content is TextContent && Settings.Default.extensionText != null)
                return Settings.Default.extensionText;
            if (content != null)
                return content.DefaultExtension;
            return "";
        }

        /// <summary>
        /// Read the clipboard and update the UI
        /// </summary>
        /// <returns></returns>
        private bool readClipboard() {

            clipData = ClipboardContents.FromClipboard();

            // Update extension dropdown list
            comExt.Items.Clear();
            foreach (var content in clipData.Contents) {
                comExt.AddWithSeparator(content.Extensions.Except(comExt.ItemArray()));
            }
            comExt.AddWithSeparator(new[] { "*" });

            // if no extension selected, update from available contents
            if (string.IsNullOrWhiteSpace(comExt.Text)) {
                comExt.Text = determineExtension(clipData);
            }

            updateContentPreview();

            if (comExt.Items.Count > 0) {
                return true;
            }

            MessageBox.Show(Resources.str_noclip_text, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;

        }



        private void ClipboardChanged(Object sender, SharpClipboard.ClipboardChangedEventArgs e) {
            var previousClipboardTimestamp = clipData.Timestamp;
            readClipboard();

            // continuous batch mode
            if (chkContinuousMode.Checked) {
                var ignore = false;
                // ignore duplicate updates within 100ms
                ignore |= (clipData.Timestamp - previousClipboardTimestamp).TotalMilliseconds <= 500;
                // ignore internal updates due to clipboard patching
                ignore |= Clipboard.ContainsData(Program.PATCHED_CLIPBOARD_MAGIC);

                if (!ignore) {
                    if (!chkAppend.Checked) updateFilename();
                    save();
                    updateSavebutton();
                }
            }
        }


        /// <summary>
        /// Update content preview depending on available clipboard data and selected file extension
        /// </summary>
        private void updateContentPreview() {

            textPreview.Hide();
            htmlPreview.Hide();
            imagePreview.Hide();
            treePreview.Hide();

            var (content, ext) = contentToSave();
            if (content == null) {
                // no matching data found
                box.Text = String.Format(Resources.str_error_cliboard_format_missmatch, comExt.Text);
                return;
            }

            box.Text = content.Description;

            if (content is ImageLikeContent imageContent) {
                var img = imageContent.ImagePreview(ext);
                if (img != null) {
                    imagePreview.Image = img;

                    // Checkerboard background in case image is transparent
                    Bitmap bg = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bg);
                    Brush brush = new SolidBrush(Color.LightGray);
                    float d = Math.Max(10, Math.Max(bg.Width, bg.Height) / 50f);
                    for (int x = 0; x < bg.Width / d; x++) {
                        for (int y = 0; y < bg.Height / d; y += 2) {
                            g.FillRectangle(brush, x * d, d * (y + x % 2), d, d);
                        }
                    }
                    imagePreview.BackgroundImage = bg;

                    imagePreview.Show();
                } else {
                    // conversion failed
                    box.Text = String.Format(Resources.str_error_cliboard_format_missmatch, comExt.Text);
                }

            } else if (content is HtmlContent htmlContent) {
                htmlPreview.DocumentText = htmlContent.Text;
                htmlPreview.Show();

            } else if (content is SvgContent svgContent) {
                htmlPreview.DocumentText = svgContent.Xml;
                htmlPreview.Show();

            } else if (content is TextLikeContent textLikeContent) {
                if (content.Extensions.FirstOrDefault() == "rtf")
                    textPreview.Rtf = textLikeContent.TextPreview(ext);
                else
                    textPreview.Text = textLikeContent.TextPreview(ext);
                textPreview.Show();

            } else if (content is FilesContent filesContent) {
                if (filesContent.TextPreview(ext) is string preview) {
                    textPreview.Text = preview;
                    textPreview.Show();
                } else {
                    treePreview.BeginUpdate();
                    treePreview.Nodes.Clear();
                    foreach (var file in filesContent.FileList) {
                        treePreview.Nodes.Add(file);
                    }
                    treePreview.EndUpdate();
                    treePreview.Show();
                }

            }


        }


        private void updateSavebutton() {
            txtFilename.Enabled = !chkContinuousMode.Checked || chkAppend.Checked;
            btnSave.Enabled = !chkContinuousMode.Checked;
            btnSave.Text = chkContinuousMode.Checked ? string.Format(Resources.str_n_saved, saveCount) : Resources.str_save;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (save() != null) {
                Environment.ExitCode = 0;
                Close();
            }
        }

        /// <summary>
        /// Return the content to be saved and the file extension to use
        /// </summary>
        /// <returns>(content to be saved, file extension)</returns>
        (BaseContent content, string ext) contentToSave() {
            var ext = comExt.Text != "*" ? comExt.Text : determineExtension(clipData);
            ext = ext.ToLowerInvariant().Trim();
            var content = clipData.ForExtension(ext);
            if (ext.StartsWith(".")) ext = ext.Substring(1);
            return (content, ext);
        }

        /// <summary>
        /// Saves the clipboard content according to the current UI settings
        /// </summary>
        /// <param name="overwriteIfExists"></param>
        /// <param name="clearClipboardOverwrite"></param>
        /// <returns></returns>
        string save(bool overwriteIfExists = false, bool? clearClipboardOverwrite = false) {
            try {
                var (content, ext) = contentToSave();

                if (content == null) {
                    MessageBox.Show(string.Format(Resources.str_error_cliboard_format_missmatch, comExt.Text), Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                // build path to save
                string dirname = Path.GetFullPath(txtCurrentLocation.Text);
                string filename = txtFilename.Text;
                if (!string.IsNullOrWhiteSpace(ext) && !filename.EndsWith("." + ext))
                    filename += "." + ext;
                string file = Path.Combine(dirname, filename);

                // check if file exists
                if (File.Exists(file)) {
                    if (overwriteIfExists) {
                        // Move old file to recycle bin and proceed
                        ExplorerUtil.MoveToRecycleBin(file);
                    } else if (!chkAppend.Checked) {
                        // Ask user for confirmation
                        var result = MessageBox.Show(string.Format(Resources.str_file_exists, file), Resources.app_title,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result != DialogResult.Yes) {
                            return null;
                        }
                    }
                } else if (Directory.Exists(file)) {
                    MessageBox.Show(string.Format(Resources.str_file_exists_directory, file), Resources.app_title,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }


                // create folders if required
                Directory.CreateDirectory(dirname);

                try {
                    content.SaveAs(file, ext, chkAppend.Checked);
                } catch (AppendNotSupportedException) {
                    // So ask user if we should replace instead
                    var msg = string.Format(Resources.str_append_not_supported, ext) + "\n\n" +
                              string.Format(Resources.str_file_exists, file);
                    var result = MessageBox.Show(msg, Resources.app_title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes) {
                        content.SaveAs(file, ext);
                    } else {
                        return null;
                    }
                }

                if (clearClipboardOverwrite ?? Settings.Default.clrClipboard) {
                    clipMonitor.MonitorClipboard = false; // to prevent callback during batch mode
                    Clipboard.Clear();
                    clipMonitor.MonitorClipboard = true;
                }

                // remember user preferences for certain content types
                if (!string.IsNullOrWhiteSpace(comExt.Text) && comExt.Text != "*") {
                    if (content is TextContent)
                        Settings.Default.extensionText = comExt.Text;
                    if (content is ImageContent)
                        Settings.Default.extensionImage = comExt.Text;
                    Settings.Default.Save();
                }

                saveCount++;
                return file;

            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.RestartAppElevated(txtCurrentLocation.Text);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, Resources.app_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        async Task CheckForUpdates() {
            if (await Program.CheckForUpdates()) {
                // Update available
                versionInfoLabel.Text = string.Format(Resources.str_version_update_available, ProductVersion, Settings.Default.updateLatestVersion);
                versionInfoLabel.LinkArea = new LinkArea(0, versionInfoLabel.Text.Length);
                versionInfoLabel.LinkClicked += (sender, args) => Process.Start(Settings.Default.updateLatestVersionLink);
            }
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e) {
            BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser();

            betterFolderBrowser.Title = Resources.str_select_folder;
            betterFolderBrowser.RootFolder = txtCurrentLocation.Text ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog(this) == DialogResult.OK) {
                txtCurrentLocation.Text = betterFolderBrowser.SelectedFolder;
            }
        }

        private void Main_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape) {
                Environment.ExitCode = 0;
                Close();
            }
        }

        private void chkAppend_CheckedChanged(object sender, EventArgs e) {
            if (disableUiEvents) return;
            updateSavebutton();
        }

        private void chkContinuousMode_CheckedChanged(object sender, EventArgs e) {
            if (disableUiEvents) return;
            if (chkContinuousMode.Checked) {
                var saveNow = MessageBox.Show(Resources.str_continuous_mode_enabled_ask_savenow, Resources.str_continuous_mode, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (saveNow == DialogResult.Yes) // save current clipboard now
                {
                    // for first time save, keep current filename
                    save();
                } else if (saveNow != DialogResult.No)
                    chkContinuousMode.Checked = false;
            }

            Settings.Default.continuousMode = chkContinuousMode.Checked;
            Settings.Default.Save();
            updateSavebutton();

        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e) {
            if (disableUiEvents) return;
            if (chkAutoSave.Checked && !Settings.Default.autoSave) {
                MessageBox.Show(Resources.str_autosave_infotext, Resources.str_autosave_checkbox, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Wizard.SetAutosaveMode(chkAutoSave.Checked);

        }

        private void settingsLinkLabel_LinkClicked(object sender, EventArgs e) {
            var dialog = new Wizard();
            dialog.TopMost = TopMost; // https://github.com/dotnet/winforms/issues/6190
            dialog.ShowDialog(this);
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Resources.str_main_info_url);
        }

        private void comExt_Update(object sender, EventArgs e) {
            updateContentPreview();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var dialog = new TemplateEdit();
            dialog.TopMost = TopMost; // https://github.com/dotnet/winforms/issues/6190
            dialog.FormClosed += DialogOnFormClosed;
            dialog.ShowDialog(this);
        }

        private void DialogOnFormClosed(object sender, FormClosedEventArgs e) {
            updateFilename();
        }
    }
}
