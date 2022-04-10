using System;
using System.Diagnostics;
using System.Drawing;
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
        private readonly SharpClipboard clipMonitor = new SharpClipboard();
        private bool continuousMode = false;
        private int saveCount = 0;



        public Dialog(string location = null, bool forceShowDialog = false) {
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

            foreach (Control element in GetAllChild(this)) {
                if (element is WebBrowser) continue;
                // ReSharper disable once UnusedVariable (to convince IDE that these resource strings are actually used)
                string[] usedResourceStrings = { Resources.str_filename, Resources.str_extension, Resources.str_location, Resources.str_clear_clipboard, Resources.str_save, Resources.str_preview, Resources.str_main_info, Resources.str_autosave_checkbox, Resources.str_contextentry_checkbox, Resources.str_continuous_mode };
                element.Text = Resources.ResourceManager.GetString(element.Text) ?? element.Text;
            }

            Icon = Resources.icon;
            Text = Resources.str_main_window_title;
            infoLabel.Text = string.Format(Resources.str_version, ProductVersion);


            // Dark theme
            if (RegistryUtil.IsDarkMode()) {
                foreach (Control element in GetAllChild(this)) {
                    element.ForeColor = Color.White;
                    element.BackColor = Color.FromArgb(53, 53, 53);
                }

                BackColor = Color.FromArgb(24, 24, 24);

            }

            // read clipboard and populate GUI
            comExt.Text = "*"; // force to use extension based on format type
            var clipRead = readClipboard();

            updateFilename();
            if (saveIntoSubdir) location += @"\" + formatFilenameTemplate(Settings.Default.subdirTemplate);
            txtCurrentLocation.Text = location;
            chkClrClipboard.Checked = Settings.Default.clrClipboard;
            chkContinuousMode.Checked = continuousMode;
            updateSavebutton();
            chkAutoSave.Checked = Settings.Default.autoSave;


            txtFilename.Select();

            // show dialog or perform autosave
            bool showDialog = forceShowDialog || !(Settings.Default.autoSave ^ invertAutosave);
            if (showDialog) {
                // Make sure to bring window to foreground (holding shift will open window in background)
                WindowState = FormWindowState.Minimized;
                Show();
                BringToFront();
                WindowState = FormWindowState.Normal;

                // register clipboard monitor
                clipMonitor.ClipboardChanged += ClipboardChanged;

            } else {
                // directly save without showing a dialog
                Opacity = 0; // prevent dialog from showing up for a fraction of a second

                var file = clipRead ? save() : null;
                if (file != null) {

                    if (!saveIntoSubdir) {
                        // select file in explorer for rename and exit afterwards
                        ExplorerUtil.FilenameEditComplete += (sender, args) => {
                            Program.ShowBalloon(Resources.str_autosave_balloontitle,
                                new[] { file, Resources.str_autosave_balloontext }, 10);
                            Environment.ExitCode = 0;
                            Close();
                        };

                        ExplorerUtil.AsyncRequestFilenameEdit(file);

                        // Timeout in case filename edit fails
                        Task.Delay(new TimeSpan(0, 0, 0, 10)).ContinueWith(o => Close());

                    } else {
                        // exit immediately
                        Program.ShowBalloon(Resources.str_autosave_balloontitle,
                            new[] { file, Resources.str_autosave_balloontext }, 10);
                        Environment.ExitCode = 0;
                        Close();
                    }

                } else {
                    // save failed, exit with error code
                    Environment.ExitCode = 1;
                    Close();
                }

            }

        }

        public string formatFilenameTemplate(string template) {
            return String.Format(template, clipData.Timestamp, saveCount);
        }
        public void updateFilename() {
            try {
                txtFilename.Text = formatFilenameTemplate(Settings.Default.filenameTemplate);
            } catch (FormatException) {
                txtFilename.Text = "filename_template_invalid";
            }
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

            // if selected extension does not match available contents, adjust it
            if (comExt.Text == "*" || comExt.Text == null || clipData.ForExtension(comExt.Text) == null) {
                // chose file extension based on available contents in this order
                BaseContent content = clipData.PrimaryContent;
                if (content is ImageContent)
                    comExt.Text = content.Extensions.Contains(Settings.Default.extensionImage) ? Settings.Default.extensionImage : content.DefaultExtension;
                else if (content is TextContent)
                    comExt.Text = Settings.Default.extensionText == null ? content.DefaultExtension : Settings.Default.extensionText;
                else if (content != null)
                    comExt.Text = content.DefaultExtension;
                else
                    comExt.Text = "";
            }

            updateContentPreview();

            if (comExt.Items.Count > 0) {
                return true;
            }

            MessageBox.Show(Resources.str_noclip_text, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;

        }



        private void ClipboardChanged(Object sender, SharpClipboard.ClipboardChangedEventArgs e) {
            var previousClipboardTimestamp = clipData.Timestamp;
            readClipboard();

            // ignore duplicate clipboard content updates within 100ms
            if (continuousMode && (clipData.Timestamp - previousClipboardTimestamp).TotalMilliseconds > 100) {
                updateFilename();
                save();
                updateSavebutton();
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

            BaseContent content = clipData.ForExtension(comExt.Text);

            if (content != null) {
                box.Text = content.Description;

                if (content is ImageContent imageContent) {
                    imagePreview.BackgroundImage = imageContent.Image;
                    imagePreview.Show();
                    return;
                }

                if (content is HtmlContent htmlContent) {
                    htmlPreview.DocumentText = htmlContent.Text;
                    htmlPreview.Show();
                    return;
                }

                if (content is TextLikeContent textLikeContent) {
                    if (content is RtfContent)
                        textPreview.Rtf = textLikeContent.Text;
                    else
                        textPreview.Text = textLikeContent.Text;
                    textPreview.Show();
                    return;
                }

                if (content is FilesContent filesContent) {
                    treePreview.BeginUpdate();
                    treePreview.Nodes.Clear();
                    foreach (var file in filesContent.FileList) {
                        treePreview.Nodes.Add(file);
                    }
                    treePreview.EndUpdate();
                    treePreview.Show();
                    return;
                }
            }

            // no matching data found
            box.Text = Resources.str_error_cliboard_format_missmatch;

        }


        private void updateSavebutton() {
            btnSave.Enabled = txtFilename.Enabled = !continuousMode;
            btnSave.Text = continuousMode ? string.Format(Resources.str_n_saved, saveCount) : Resources.str_save;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (save() != null) {
                Environment.ExitCode = 0;
                Close();
            }
        }

        string save() {
            try {
                string dirname = Path.GetFullPath(txtCurrentLocation.Text);
                string ext = comExt.Text.ToLowerInvariant().Trim();
                if (ext.StartsWith(".")) ext = ext.Substring(1);
                string filename = txtFilename.Text;
                if (!string.IsNullOrWhiteSpace(ext) && !filename.EndsWith("." + ext))
                    filename += "." + ext;
                string file = Path.Combine(dirname, filename);

                // check if file exists
                if (File.Exists(file)) {
                    var result = MessageBox.Show(string.Format(Resources.str_file_exists, file), Resources.str_main_window_title,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) {
                        return null;
                    }
                } else if (Directory.Exists(file)) {
                    MessageBox.Show(string.Format(Resources.str_file_exists_directory, file), Resources.str_main_window_title,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }


                // create folders if required
                Directory.CreateDirectory(dirname);

                BaseContent contentToSave = clipData.ForExtension(comExt.Text);

                if (contentToSave != null) {
                    contentToSave.SaveAs(file, ext);
                } else {
                    return null;
                }

                if (Settings.Default.clrClipboard) {
                    Clipboard.Clear();
                }

                saveCount++;
                return file;

            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message + "\n" + Resources.str_message_run_as_admin, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.RestartAppElevated(txtCurrentLocation.Text);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, Resources.str_main_window_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
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

        private void ChkClrClipboard_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.clrClipboard = chkClrClipboard.Checked;
            Settings.Default.Save();
        }

        private void chkContinuousMode_CheckedChanged(object sender, EventArgs e) {
            if (chkContinuousMode.Checked) {
                var saveNow = MessageBox.Show(Resources.str_continuous_mode_enabled_ask_savenow, Resources.str_continuous_mode, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (saveNow == DialogResult.Yes) // save current clipboard now
                {
                    updateFilename();
                    save();
                } else if (saveNow != DialogResult.No)
                    chkContinuousMode.Checked = false;
            }

            continuousMode = chkContinuousMode.Checked;
            updateSavebutton();

        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e) {
            if (chkAutoSave.Checked && !Settings.Default.autoSave) {
                MessageBox.Show(Resources.str_autosave_infotext, Resources.str_autosave_checkbox, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Wizard.SetAutosaveMode(chkAutoSave.Checked);

        }

        private void settingsLinkLabel_LinkClicked(object sender, EventArgs e) {
            new Wizard().ShowDialog(this);
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Resources.str_main_info_url);
        }

        private void comExt_Update(object sender, EventArgs e) {

            updateContentPreview();

            // remember user selected defaults for certain content types
            BaseContent content = clipData.ForExtension(comExt.Text);
            if (content is TextLikeContent)
                Settings.Default.extensionText = comExt.Text;
            if (content is ImageContent)
                Settings.Default.extensionImage = comExt.Text;

            Settings.Default.Save();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var dialog = new TemplateEdit();
            dialog.FormClosed += DialogOnFormClosed;
            dialog.ShowDialog(this);
        }

        private void DialogOnFormClosed(object sender, FormClosedEventArgs e) {
            updateFilename();
        }
    }
}
