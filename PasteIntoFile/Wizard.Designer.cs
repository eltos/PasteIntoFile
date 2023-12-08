using System;
using System.Drawing;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    partial class Wizard {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.autoSaveCheckBox = new System.Windows.Forms.CheckBox();
            this.autoSaveMayOpenNewCheckBox = new System.Windows.Forms.CheckBox();
            this.autoSaveInfoLabel = new System.Windows.Forms.Label();
            this.autoSaveTitleLabel = new System.Windows.Forms.Label();
            this.autostartCheckBox = new System.Windows.Forms.CheckBox();
            this.autostartInfoLabel = new System.Windows.Forms.Label();
            this.autostartTitleLabel = new System.Windows.Forms.Label();
            this.patchingCheckBox = new System.Windows.Forms.CheckBox();
            this.title = new System.Windows.Forms.Label();
            this.contextEntryTitleLabel = new System.Windows.Forms.Label();
            this.contextEntryInfoLabel = new System.Windows.Forms.Label();
            this.contextEntryCheckBoxPaste = new System.Windows.Forms.CheckBox();
            this.contextEntryCheckBoxCopy = new System.Windows.Forms.CheckBox();
            this.contextEntryCheckBoxReplace = new System.Windows.Forms.CheckBox();
            this.finish = new System.Windows.Forms.Button();
            this.version = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.title, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.version, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryTitleLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryInfoLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryCheckBoxPaste, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryCheckBoxCopy, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryCheckBoxReplace, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveTitleLabel, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveInfoLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveCheckBox, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveMayOpenNewCheckBox, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.autostartTitleLabel, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.autostartInfoLabel, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.autostartCheckBox, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.patchingCheckBox, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.finish, 0, 15);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.RowCount = 17;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // autoSaveCheckBox
            //
            this.autoSaveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.autoSaveCheckBox.AutoSize = true;
            this.autoSaveCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoSaveCheckBox.Location = new System.Drawing.Point(301, 331);
            this.autoSaveCheckBox.Margin = new System.Windows.Forms.Padding(9, 9, 9, 0);
            this.autoSaveCheckBox.Name = "autoSaveCheckBox";
            this.autoSaveCheckBox.Size = new System.Drawing.Size(262, 24);
            this.autoSaveCheckBox.TabIndex = 4;
            this.autoSaveCheckBox.Text = Resources.str_wizard_autosave_button;
            this.autoSaveCheckBox.UseVisualStyleBackColor = true;
            this.autoSaveCheckBox.CheckedChanged += new System.EventHandler(this.ChkAutoSave_CheckedChanged);
            //
            // autostartMayOpenNewCheckBox
            //
            this.autoSaveMayOpenNewCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.autoSaveMayOpenNewCheckBox.AutoSize = true;
            this.autoSaveMayOpenNewCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoSaveMayOpenNewCheckBox.Location = new System.Drawing.Point(275, 192);
            this.autoSaveMayOpenNewCheckBox.Margin = new System.Windows.Forms.Padding(9, 0, 9, 9);
            this.autoSaveMayOpenNewCheckBox.Name = "autoSaveMayOpenNewCheckBox";
            this.autoSaveMayOpenNewCheckBox.Size = new System.Drawing.Size(288, 24);
            this.autoSaveMayOpenNewCheckBox.TabIndex = 5;
            this.autoSaveMayOpenNewCheckBox.Text = Resources.str_wizard_autosave_may_open_new_explorer;
            this.autoSaveMayOpenNewCheckBox.UseVisualStyleBackColor = true;
            this.autoSaveMayOpenNewCheckBox.CheckedChanged += new System.EventHandler(this.ChkAutoSaveMayOpenNew_CheckedChanged);
            //
            // autoSaveInfoLabel
            //
            this.autoSaveInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.autoSaveInfoLabel.AutoSize = true;
            this.autoSaveInfoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoSaveInfoLabel.Location = new System.Drawing.Point(8, 295);
            this.autoSaveInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.autoSaveInfoLabel.Name = "autoSaveInfoLabel";
            this.autoSaveInfoLabel.Size = new System.Drawing.Size(560, 23);
            this.autoSaveInfoLabel.Text = Resources.str_wizard_autosave_info;
            //
            // autoSaveTitleLabel
            //
            this.autoSaveTitleLabel.AutoSize = true;
            this.autoSaveTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoSaveTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoSaveTitleLabel.Location = new System.Drawing.Point(8, 255);
            this.autoSaveTitleLabel.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.autoSaveTitleLabel.Name = "autoSaveTitleLabel";
            this.autoSaveTitleLabel.Size = new System.Drawing.Size(560, 32);
            this.autoSaveTitleLabel.Text = Resources.str_wizard_autosave_title;
            //
            // title
            //
            this.title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(8, 8);
            this.title.Margin = new System.Windows.Forms.Padding(4);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(560, 54);
            this.title.Text = Resources.str_wizard_title;
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // contextEntryTitleLabel
            //
            this.contextEntryTitleLabel.AutoSize = true;
            this.contextEntryTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryTitleLabel.Location = new System.Drawing.Point(8, 116);
            this.contextEntryTitleLabel.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.contextEntryTitleLabel.Name = "contextEntryTitleLabel";
            this.contextEntryTitleLabel.Size = new System.Drawing.Size(343, 32);
            this.contextEntryTitleLabel.Text = Resources.str_wizard_contextentry_title;
            //
            // contextEntryInfoLabel
            //
            this.contextEntryInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.contextEntryInfoLabel.AutoSize = true;
            this.contextEntryInfoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryInfoLabel.Location = new System.Drawing.Point(8, 156);
            this.contextEntryInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.contextEntryInfoLabel.Name = "contextEntryInfoLabel";
            this.contextEntryInfoLabel.Size = new System.Drawing.Size(560, 23);
            this.contextEntryInfoLabel.Text = Resources.str_wizard_contextentry_info;
            //
            // contextEntryCheckBoxPaste
            //
            this.contextEntryCheckBoxPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.contextEntryCheckBoxPaste.AutoSize = true;
            this.contextEntryCheckBoxPaste.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryCheckBoxPaste.Location = new System.Drawing.Point(275, 192);
            this.contextEntryCheckBoxPaste.Margin = new System.Windows.Forms.Padding(9, 9, 9, 0);
            this.contextEntryCheckBoxPaste.Name = "contextEntryCheckBoxPaste";
            this.contextEntryCheckBoxPaste.Size = new System.Drawing.Size(288, 24);
            this.contextEntryCheckBoxPaste.TabIndex = 1;
            this.contextEntryCheckBoxPaste.Text = Resources.str_contextentry;
            this.contextEntryCheckBoxPaste.UseVisualStyleBackColor = true;
            this.contextEntryCheckBoxPaste.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
            //
            // contextEntryCheckBoxCopy
            //
            this.contextEntryCheckBoxCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.contextEntryCheckBoxCopy.AutoSize = true;
            this.contextEntryCheckBoxCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryCheckBoxCopy.Location = new System.Drawing.Point(275, 192);
            this.contextEntryCheckBoxCopy.Margin = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.contextEntryCheckBoxCopy.Name = "contextEntryCheckBoxCopy";
            this.contextEntryCheckBoxCopy.Size = new System.Drawing.Size(288, 24);
            this.contextEntryCheckBoxCopy.TabIndex = 2;
            this.contextEntryCheckBoxCopy.Text = Resources.str_contextentry_copyfromfile;
            this.contextEntryCheckBoxCopy.UseVisualStyleBackColor = true;
            this.contextEntryCheckBoxCopy.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
            //
            // contextEntryCheckBoxReplace
            //
            this.contextEntryCheckBoxReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.contextEntryCheckBoxReplace.AutoSize = true;
            this.contextEntryCheckBoxReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryCheckBoxReplace.Location = new System.Drawing.Point(275, 192);
            this.contextEntryCheckBoxReplace.Margin = new System.Windows.Forms.Padding(9, 0, 9, 9);
            this.contextEntryCheckBoxReplace.Name = "contextEntryCheckBoxReplace";
            this.contextEntryCheckBoxReplace.Size = new System.Drawing.Size(288, 24);
            this.contextEntryCheckBoxReplace.TabIndex = 3;
            this.contextEntryCheckBoxReplace.Text = Resources.str_contextentry_replaceintofile;
            this.contextEntryCheckBoxReplace.UseVisualStyleBackColor = true;
            this.contextEntryCheckBoxReplace.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
            //
            // autostartTitleLabel
            //
            this.autostartTitleLabel.AutoSize = true;
            this.autostartTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartTitleLabel.Location = new System.Drawing.Point(8, 116);
            this.autostartTitleLabel.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.autostartTitleLabel.Name = "autostartTitleLabel";
            this.autostartTitleLabel.Size = new System.Drawing.Size(343, 32);
            this.autostartTitleLabel.Text = Resources.str_wizard_tray_title;
            //
            // autostartInfoLabel
            //
            this.autostartInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.autostartInfoLabel.AutoSize = true;
            this.autostartInfoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartInfoLabel.Location = new System.Drawing.Point(8, 156);
            this.autostartInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.autostartInfoLabel.Name = "autostartInfoLabel";
            this.autostartInfoLabel.Size = new System.Drawing.Size(560, 23);
            this.autostartInfoLabel.Text = Resources.str_wizard_tray_info;
            //
            // autostartCheckBox
            //
            this.autostartCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.autostartCheckBox.AutoSize = true;
            this.autostartCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartCheckBox.Location = new System.Drawing.Point(275, 192);
            this.autostartCheckBox.Margin = new System.Windows.Forms.Padding(9, 9, 9, 0);
            this.autostartCheckBox.Name = "autostartCheckBox";
            this.autostartCheckBox.Size = new System.Drawing.Size(288, 24);
            this.autostartCheckBox.TabIndex = 5;
            this.autostartCheckBox.Text = Resources.str_wizard_tray_autostart_button;
            this.autostartCheckBox.UseVisualStyleBackColor = true;
            this.autostartCheckBox.CheckedChanged += new System.EventHandler(this.ChkAutostart_CheckedChanged);
            //
            // patchingCheckBox
            //
            this.patchingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.patchingCheckBox.AutoSize = true;
            this.patchingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patchingCheckBox.Location = new System.Drawing.Point(248, 341);
            this.patchingCheckBox.Margin = new System.Windows.Forms.Padding(9, 0, 9, 9);
            this.patchingCheckBox.Name = "patchingCheckBox";
            this.patchingCheckBox.Size = new System.Drawing.Size(210, 17);
            this.patchingCheckBox.TabIndex = 6;
            this.patchingCheckBox.Text = Resources.str_wizard_tray_patching_button;
            this.patchingCheckBox.UseVisualStyleBackColor = true;
            this.patchingCheckBox.CheckedChanged += new System.EventHandler(this.ChkPatching_CheckedChanged);
            //
            // finish
            //
            this.finish.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.finish.AutoSize = true;
            this.finish.Location = new System.Drawing.Point(185, 394);
            this.finish.Margin = new System.Windows.Forms.Padding(4, 30, 4, 15);
            this.finish.Name = "finish";
            this.finish.Size = new System.Drawing.Size(206, 30);
            this.finish.TabIndex = 7;
            this.finish.Text = Resources.str_wizard_finish;
            this.finish.UseVisualStyleBackColor = true;
            this.finish.Click += new System.EventHandler(this.finish_Click);
            //
            // version
            //
            this.version.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.version.AutoSize = true;
            this.version.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.version.Location = new System.Drawing.Point(258, 66);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(59, 20);
            this.version.TabIndex = 9;
            this.version.Text = "version";
            this.version.LinkColor = Color.DodgerBlue;
            this.version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // settings menu
            //
            this.settingsMenu = new ContextMenuStrip();
            this.settingsMenu.AutoSize = true;
            this.settingsMenuUpdateChecks = new ToolStripMenuItem();
            this.settingsMenuUpdateChecks.CheckOnClick = true;
            this.settingsMenuUpdateChecks.CheckedChanged += new EventHandler(this.menuUpdateChecks_CheckedChanged);
            this.settingsMenuUpdateChecks.Text = Resources.str_wizard_notify_on_updates;
            this.settingsMenu.Items.Add(this.settingsMenuUpdateChecks);
            //
            // settings button
            //
            this.settingsButton = new Label();
            this.settingsButton.Text = "⚙️";
            this.settingsButton.Font = new Font("Microsoft Sans Serif", 16F);
            this.settingsButton.Location = new Point(0, 4);
            this.settingsButton.Margin = new Padding(0);
            this.settingsButton.Name = "settings";
            this.settingsButton.TabIndex = 8;
            this.settingsButton.AutoSize = true;
            this.settingsButton.Cursor = Cursors.Hand;
            this.settingsButton.Click += new EventHandler(this.settingsButton_Click);
            this.Controls.Add(this.settingsButton);
            //
            // Wizard
            //
            this.AcceptButton = this.finish;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 900);
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.Wizard_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox patchingCheckBox;
        private System.Windows.Forms.Label settingsButton;
        private System.Windows.Forms.ContextMenuStrip settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuUpdateChecks;
        private System.Windows.Forms.LinkLabel version;
        private System.Windows.Forms.Button finish;
        private System.Windows.Forms.CheckBox contextEntryCheckBoxPaste;
        private System.Windows.Forms.CheckBox contextEntryCheckBoxCopy;
        private System.Windows.Forms.CheckBox contextEntryCheckBoxReplace;
        private System.Windows.Forms.Label autoSaveTitleLabel;
        private System.Windows.Forms.Label contextEntryInfoLabel;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.CheckBox autoSaveCheckBox;
        private System.Windows.Forms.CheckBox autoSaveMayOpenNewCheckBox;
        private System.Windows.Forms.Label contextEntryTitleLabel;
        private System.Windows.Forms.Label autostartTitleLabel;
        private System.Windows.Forms.CheckBox autostartCheckBox;
        private System.Windows.Forms.Label autostartInfoLabel;
        private System.Windows.Forms.Label autoSaveInfoLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion
    }
}
