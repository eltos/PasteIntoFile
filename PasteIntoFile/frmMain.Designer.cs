using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblType = new System.Windows.Forms.Label();
            this.imgContent = new System.Windows.Forms.PictureBox();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.lblExt = new System.Windows.Forms.Label();
            this.comExt = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtCurrentLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseForFolder = new System.Windows.Forms.Button();
            this.infoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.linkUnregister = new System.Windows.Forms.LinkLabel();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            this.clrClipboard = new System.Windows.Forms.CheckBox();
            this.autoSave = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgContent)).BeginInit();
            this.SuspendLayout();
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(20, 200);
            this.lblType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(43, 20);
            this.lblType.TabIndex = 6;
            this.lblType.Text = "Type";
            this.lblType.Visible = false;
            // 
            // imgContent
            // 
            this.imgContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imgContent.Location = new System.Drawing.Point(24, 210);
            this.imgContent.Margin = new System.Windows.Forms.Padding(4, 05, 4, 5);
            this.imgContent.Name = "imgContent";
            this.imgContent.Size = new System.Drawing.Size(728, 500);
            this.imgContent.TabIndex = 2;
            this.imgContent.TabStop = false;
            this.imgContent.Visible = false;
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(18, 362);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(280, 176);
            this.txtContent.TabIndex = 3;
            this.txtContent.Visible = false;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(19, 20);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(78, 20);
            this.lblFileName.TabIndex = 4;
            this.lblFileName.Text = Resources.str_filename;
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(24, 46);
            this.txtFilename.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(612, 26);
            this.txtFilename.TabIndex = 1;
            // 
            // lblExt
            // 
            this.lblExt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(639, 20);
            this.lblExt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(83, 20);
            this.lblExt.TabIndex = 6;
            this.lblExt.Text = Resources.str_extension;
            // 
            // comExt
            // 
            this.comExt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comExt.FormattingEnabled = true;
            this.comExt.Location = new System.Drawing.Point(644, 46);
            this.comExt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comExt.Name = "comExt";
            this.comExt.Size = new System.Drawing.Size(108, 28);
            this.comExt.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(276, 148);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(476, 45);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = Resources.str_save;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtCurrentLocation
            // 
            this.txtCurrentLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentLocation.Location = new System.Drawing.Point(24, 111);
            this.txtCurrentLocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCurrentLocation.Name = "txtCurrentLocation";
            this.txtCurrentLocation.Size = new System.Drawing.Size(670, 26);
            this.txtCurrentLocation.TabIndex = 3;
            this.txtCurrentLocation.Text = "C:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = Resources.str_location;
            // 
            // btnBrowseForFolder
            // 
            this.btnBrowseForFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseForFolder.Location = new System.Drawing.Point(702, 110);
            this.btnBrowseForFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBrowseForFolder.Name = "btnBrowseForFolder";
            this.btnBrowseForFolder.Size = new System.Drawing.Size(50, 32);
            this.btnBrowseForFolder.TabIndex = 4;
            this.btnBrowseForFolder.Text = Resources.str_ellipsis;
            this.btnBrowseForFolder.UseVisualStyleBackColor = true;
            this.btnBrowseForFolder.Click += new System.EventHandler(this.btnBrowseForFolder_Click);
            // 
            // linkRegister
            // 
            this.linkRegister.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.linkRegister.Location = new System.Drawing.Point(18, 217);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.Size = new System.Drawing.Size(300, 20);
            this.linkRegister.TabIndex = 14;
            this.linkRegister.TabStop = true;
            this.linkRegister.Text = Resources.str_contextentry_register;
            this.linkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler((sender, e) => Program.RegisterApp());
            // 
            // linkUnregister
            // 
            this.linkUnregister.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.linkUnregister.Location = new System.Drawing.Point(18, 251);
            this.linkUnregister.Name = "linkUnregister";
            this.linkUnregister.Size = new System.Drawing.Size(300, 20);
            this.linkUnregister.TabIndex = 15;
            this.linkUnregister.TabStop = true;
            this.linkUnregister.Text = Resources.str_contextentry_unregister;
            this.linkUnregister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler((sender, e) => Program.UnRegisterApp());
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.Location = new System.Drawing.Point(357, 217);
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(400, 20);
            this.infoLabel.ForeColor = System.Drawing.Color.Gray;
            this.infoLabel.Text = string.Format(Resources.str_version, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            // 
            // infoLinkLabel
            // 
            this.infoLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLinkLabel.Location = new System.Drawing.Point(357, 251);
            this.infoLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.infoLinkLabel.Name = "infoLinkLabel";
            this.infoLinkLabel.Size = new System.Drawing.Size(400, 20);
            this.infoLinkLabel.LinkColor = System.Drawing.Color.Gray;
            this.infoLinkLabel.TabIndex = 17;
            this.infoLinkLabel.TabStop = true;
            this.infoLinkLabel.Text = Resources.str_main_info;
            this.infoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler((sender, e) => Process.Start(Resources.str_main_info_url));
            
            // 
            // clrClipboard
            // 
            this.clrClipboard.AutoSize = true;
            this.clrClipboard.Location = new System.Drawing.Point(22, 148);
            this.clrClipboard.Name = "clrClipboard";
            this.clrClipboard.Size = new System.Drawing.Size(140, 24);
            this.clrClipboard.TabIndex = 10;
            this.clrClipboard.Text = Resources.str_clear_clipboard;
            this.clrClipboard.UseVisualStyleBackColor = true;
            this.clrClipboard.CheckedChanged += new System.EventHandler(this.ClrClipboard_CheckedChanged);
            // 
            // autoSave
            // 
            this.autoSave.AutoSize = true;
            this.autoSave.Location = new System.Drawing.Point(22, 173);
            this.autoSave.Name = "autoSave";
            this.autoSave.Size = new System.Drawing.Size(102, 24);
            this.autoSave.TabIndex = 11;
            this.autoSave.Text = Resources.str_autosave;
            this.autoSave.UseVisualStyleBackColor = true;
            this.autoSave.CheckedChanged += new System.EventHandler(this.AutoSave_CheckedChanged);
            this.autoSave.Click += new System.EventHandler(this.AutoSave_Click);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(775, 287);
            this.Controls.Add(this.autoSave);
            this.Controls.Add(this.clrClipboard);
            this.Controls.Add(this.linkRegister);
            this.Controls.Add(this.linkUnregister);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.infoLinkLabel);
            this.Controls.Add(this.btnBrowseForFolder);
            this.Controls.Add(this.txtCurrentLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.comExt);
            this.Controls.Add(this.lblExt);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.imgContent);
            this.Controls.Add(this.lblType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = Resources.icon;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Resources.str_main_window_title;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.imgContent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.PictureBox imgContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.ComboBox comExt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtCurrentLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseForFolder;
        private System.Windows.Forms.LinkLabel infoLinkLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.LinkLabel linkUnregister;
        private System.Windows.Forms.LinkLabel linkRegister;
        private System.Windows.Forms.CheckBox clrClipboard;
        private System.Windows.Forms.CheckBox autoSave;
    }
}

