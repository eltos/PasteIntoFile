using System.ComponentModel;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    partial class Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgContent = new System.Windows.Forms.PictureBox();
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.lblExt = new System.Windows.Forms.Label();
            this.comExt = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtCurrentLocation = new System.Windows.Forms.TextBox();
            this.lblCurrentLocation = new System.Windows.Forms.Label();
            this.btnBrowseForFolder = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.infoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.chkClrClipboard = new System.Windows.Forms.CheckBox();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.box = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkContinuousMode = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.chkContextEntry = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize) (this.imgContent)).BeginInit();
            this.box.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgContent
            // 
            this.imgContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imgContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgContent.Location = new System.Drawing.Point(8, 27);
            this.imgContent.Margin = new System.Windows.Forms.Padding(4);
            this.imgContent.Name = "imgContent";
            this.imgContent.Size = new System.Drawing.Size(522, 236);
            this.imgContent.TabIndex = 13;
            this.imgContent.TabStop = false;
            this.imgContent.Visible = false;
            // 
            // txtContent
            // 
            this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.txtContent.Location = new System.Drawing.Point(8, 27);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4);
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.Size = new System.Drawing.Size(522, 236);
            this.txtContent.TabIndex = 0;
            this.txtContent.TabStop = false;
            this.txtContent.Text = "";
            this.txtContent.Visible = false;
            this.txtContent.WordWrap = false;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(0, 0);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(96, 20);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "str_filename";
            // 
            // txtFilename
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.txtFilename, 2);
            this.txtFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilename.Location = new System.Drawing.Point(4, 24);
            this.txtFilename.Margin = new System.Windows.Forms.Padding(4);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(376, 26);
            this.txtFilename.TabIndex = 0;
            this.txtFilename.Text = "xxx";
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(388, 0);
            this.lblExt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(104, 20);
            this.lblExt.TabIndex = 2;
            this.lblExt.Text = "str_extension";
            // 
            // comExt
            // 
            this.comExt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comExt.FormattingEnabled = true;
            this.comExt.Location = new System.Drawing.Point(388, 24);
            this.comExt.Margin = new System.Windows.Forms.Padding(4);
            this.comExt.Name = "comExt";
            this.comExt.Size = new System.Drawing.Size(154, 28);
            this.comExt.TabIndex = 1;
            this.comExt.SelectedIndexChanged += new System.EventHandler(this.comExt_Update);
            this.comExt.TextUpdate += new System.EventHandler(this.comExt_Update);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.Location = new System.Drawing.Point(277, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.tableLayoutPanel2.SetRowSpan(this.btnSave, 2);
            this.btnSave.Size = new System.Drawing.Size(265, 46);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "str_save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtCurrentLocation
            // 
            this.txtCurrentLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentLocation.Location = new System.Drawing.Point(4, 24);
            this.txtCurrentLocation.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrentLocation.Name = "txtCurrentLocation";
            this.txtCurrentLocation.Size = new System.Drawing.Size(455, 26);
            this.txtCurrentLocation.TabIndex = 2;
            // 
            // lblCurrentLocation
            // 
            this.lblCurrentLocation.AutoSize = true;
            this.lblCurrentLocation.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentLocation.Margin = new System.Windows.Forms.Padding(0);
            this.lblCurrentLocation.Name = "lblCurrentLocation";
            this.lblCurrentLocation.Size = new System.Drawing.Size(91, 20);
            this.lblCurrentLocation.TabIndex = 1;
            this.lblCurrentLocation.Text = "str_location";
            // 
            // btnBrowseForFolder
            // 
            this.btnBrowseForFolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowseForFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseForFolder.Location = new System.Drawing.Point(467, 24);
            this.btnBrowseForFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseForFolder.Name = "btnBrowseForFolder";
            this.btnBrowseForFolder.Size = new System.Drawing.Size(75, 39);
            this.btnBrowseForFolder.TabIndex = 3;
            this.btnBrowseForFolder.Text = "...";
            this.btnBrowseForFolder.UseVisualStyleBackColor = true;
            this.btnBrowseForFolder.Click += new System.EventHandler(this.btnBrowseForFolder_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.infoLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.infoLabel.Location = new System.Drawing.Point(482, 4);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(64, 20);
            this.infoLabel.TabIndex = 10;
            this.infoLabel.Text = "v0.0.0.0";
            // 
            // infoLinkLabel
            // 
            this.infoLinkLabel.AutoSize = true;
            this.infoLinkLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.infoLinkLabel.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.infoLinkLabel.Location = new System.Drawing.Point(441, 32);
            this.infoLinkLabel.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.infoLinkLabel.Name = "infoLinkLabel";
            this.infoLinkLabel.Size = new System.Drawing.Size(105, 20);
            this.infoLinkLabel.TabIndex = 9;
            this.infoLinkLabel.TabStop = true;
            this.infoLinkLabel.Text = "str_main_info";
            this.infoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.infoLinkLabel_LinkClicked);
            // 
            // chkClrClipboard
            // 
            this.chkClrClipboard.AutoSize = true;
            this.chkClrClipboard.Location = new System.Drawing.Point(4, 0);
            this.chkClrClipboard.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chkClrClipboard.Name = "chkClrClipboard";
            this.chkClrClipboard.Size = new System.Drawing.Size(169, 24);
            this.chkClrClipboard.TabIndex = 6;
            this.chkClrClipboard.Text = "str_clear_clipboard";
            this.chkClrClipboard.UseVisualStyleBackColor = true;
            this.chkClrClipboard.CheckedChanged += new System.EventHandler(this.ChkClrClipboard_CheckedChanged);
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Location = new System.Drawing.Point(4, 0);
            this.chkAutoSave.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(203, 24);
            this.chkAutoSave.TabIndex = 5;
            this.chkAutoSave.Text = "str_autosave_checkbox";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            this.chkAutoSave.CheckedChanged += new System.EventHandler(this.ChkAutoSave_CheckedChanged);
            // 
            // box
            // 
            this.box.AutoSize = true;
            this.box.Controls.Add(this.txtContent);
            this.box.Controls.Add(this.imgContent);
            this.box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.box.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.box.Location = new System.Drawing.Point(19, 196);
            this.box.Margin = new System.Windows.Forms.Padding(4);
            this.box.Name = "box";
            this.box.Padding = new System.Windows.Forms.Padding(8);
            this.box.Size = new System.Drawing.Size(538, 271);
            this.box.TabIndex = 5;
            this.box.TabStop = false;
            this.box.Text = "str_preview";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.box, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(15);
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(576, 542);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.txtFilename, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblFileName, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblExt, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.comExt, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.linkLabel1, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(15, 15);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(546, 56);
            this.tableLayoutPanel3.TabIndex = 11;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.linkLabel1.Location = new System.Drawing.Point(251, 0);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(133, 20);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "str_edit_template";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.txtCurrentLocation, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lblCurrentLocation, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnBrowseForFolder, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(15, 71);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(546, 67);
            this.tableLayoutPanel4.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnSave, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkClrClipboard, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkContinuousMode, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(15, 138);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(546, 54);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // chkContinuousMode
            // 
            this.chkContinuousMode.AutoSize = true;
            this.chkContinuousMode.Location = new System.Drawing.Point(4, 24);
            this.chkContinuousMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chkContinuousMode.Name = "chkContinuousMode";
            this.chkContinuousMode.Size = new System.Drawing.Size(189, 24);
            this.chkContinuousMode.TabIndex = 6;
            this.chkContinuousMode.Text = "str_continuous_mode";
            this.chkContinuousMode.UseVisualStyleBackColor = true;
            this.chkContinuousMode.CheckedChanged += new System.EventHandler(this.chkContinuousMode_CheckedChanged);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.infoLinkLabel, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.chkAutoSave, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.infoLabel, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.chkContextEntry, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(15, 471);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(546, 56);
            this.tableLayoutPanel5.TabIndex = 14;
            // 
            // chkContextEntry
            // 
            this.chkContextEntry.AutoSize = true;
            this.chkContextEntry.Location = new System.Drawing.Point(4, 28);
            this.chkContextEntry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chkContextEntry.Name = "chkContextEntry";
            this.chkContextEntry.Size = new System.Drawing.Size(225, 24);
            this.chkContextEntry.TabIndex = 5;
            this.chkContextEntry.Text = "str_contextentry_checkbox";
            this.chkContextEntry.UseVisualStyleBackColor = true;
            this.chkContextEntry.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
            // 
            // Dialog
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(576, 542);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(589, 422);
            this.Name = "Dialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Main_KeyPress);
            ((System.ComponentModel.ISupportInitialize) (this.imgContent)).EndInit();
            this.box.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.LinkLabel linkLabel1;

        private System.Windows.Forms.CheckBox chkContinuousMode;
        private System.Windows.Forms.CheckBox chkContextEntry;
        private System.Windows.Forms.PictureBox imgContent;
        private System.Windows.Forms.RichTextBox txtContent;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.ComboBox comExt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtCurrentLocation;
        private System.Windows.Forms.Label lblCurrentLocation;
        private System.Windows.Forms.Button btnBrowseForFolder;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.LinkLabel infoLinkLabel;
        private System.Windows.Forms.CheckBox chkClrClipboard;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.GroupBox box;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion

    }
}