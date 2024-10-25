using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile {

    public enum Template {
        FILENAME,
        SUBFOLDER,
    }

    public sealed partial class TemplateEdit : MasterForm {
        public readonly Template template;

        public TemplateEdit(Template template) {
            this.template = template;
            InitializeComponent();


            Icon = Resources.app_icon;
            var preset = "";
            switch (this.template) {
                case Template.FILENAME:
                    Text = Resources.str_edit_template;
                    labelInfo.Text = Resources.str_template_edit_filename_info + @" " + Resources.str_template_edit_info;
                    labelTemplate.Text = Resources.str_filename_template;
                    preset = Settings.Default.filenameTemplate;
                    break;
                case Template.SUBFOLDER:
                    Text = Resources.str_subfolder_template;
                    labelInfo.Text = Resources.str_template_edit_subfolder_info + @" " + Resources.str_template_edit_info;
                    labelTemplate.Text = Resources.str_subfolder_template;
                    preset = Settings.Default.subdirTemplate;
                    break;
            }

            // setup link in info text
            var linkStart = labelInfo.Text.IndexOf(@"<a>", StringComparison.Ordinal);
            var linkEnd = labelInfo.Text.IndexOf(@"</a>", StringComparison.Ordinal);
            if (linkStart >= 0 && linkEnd > linkStart) {
                labelInfo.Text = labelInfo.Text.Remove(linkStart, 3).Remove(linkEnd - 3, 4);
                labelInfo.LinkArea = new LinkArea(linkStart, linkEnd - linkStart - 3);
            }

            // setup predefined templates
            textTemplate.Items.AddRange(new object[]{
                preset,
                "{0:yyyy-MM-dd HH-mm-ss}",
                "{0:yyyyMMdd_HHmmss}",
                "{0:yyyy-jjj}_{1:000}",
                "PasteIntoFile_{1:000}_{0:fffffff}",
                "{0:yyyy-ww}/{0:ddd HHᵸmm´ss´´}",
                "{2:10}",
            });
            textTemplate.Text = preset;

            // Dark theme
            if (RegistryUtil.IsDarkMode()) {
                MakeDarkMode();
            }

        }


        private void buttonAccept_Click(object sender, EventArgs e) {
            switch (template) {
                case Template.FILENAME:
                    Settings.Default.filenameTemplate = textTemplate.Text;
                    break;
                case Template.SUBFOLDER:
                    Settings.Default.subdirTemplate = textTemplate.Text;
                    break;
            }
            Settings.Default.Save();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void textTemplate_Update(object sender = null, EventArgs e = null) {
            try {
                labelPreview.Text = ((Dialog)Owner)?.formatFilenameTemplate(textTemplate.Text);
                labelPreview.ForeColor = TextColor;
                buttonAccept.Enabled = true;

                var invalidChars = Array.Empty<char>();
                switch (template) {
                    case Template.FILENAME:
                        invalidChars = Path.GetInvalidFileNameChars().Except(new[] { '\\', '/' }).ToArray();
                        break;
                    case Template.SUBFOLDER:
                        invalidChars = Path.GetInvalidFileNameChars().Except(new[] { '\\', '/', ':' }).ToArray();
                        break;
                }
                var i = labelPreview.Text?.IndexOfAny(invalidChars);
                if (i is int j && j >= 0)
                    throw new FormatException(string.Format(Resources.str_invalid_character, labelPreview.Text[j]));
            } catch (FormatException ex) {
                labelPreview.Text = ex.Message;
                labelPreview.ForeColor = Color.Red;
                buttonAccept.Enabled = false;
            }
        }

        private void labelInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Resources.str_template_edit_info_url);
        }

        private void TemplateEdit_Shown(object sender, EventArgs e) {
            // Auto size dialog height
            // All tableLayout rows are set to 'autosize' except for the last -> it's height is a measure for leftover space
            Height -= tableLayoutPanel1.GetRowHeights()[tableLayoutPanel1.RowCount - 1];
            MinimumSize = Size;
        }

    }
}
