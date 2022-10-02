using System.Drawing;
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
            this.autoSaveInfoLabel = new System.Windows.Forms.Label();
            this.autoSaveTitleLabel = new System.Windows.Forms.Label();
            this.autostartCheckBox = new System.Windows.Forms.CheckBox();
            this.autostartInfoLabel = new System.Windows.Forms.Label();
            this.autostartTitleLabel = new System.Windows.Forms.Label();
            this.patchingCheckBox = new System.Windows.Forms.CheckBox();
            this.title = new System.Windows.Forms.Label();
            this.contextEntryTitleLabel = new System.Windows.Forms.Label();
            this.contextEntryInfoLabel = new System.Windows.Forms.Label();
            this.contextEntryCheckBox = new System.Windows.Forms.CheckBox();
            this.finish = new System.Windows.Forms.Button();
            this.version = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.title, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.version, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryTitleLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryInfoLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.contextEntryCheckBox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveTitleLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveInfoLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.autoSaveCheckBox, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.autostartTitleLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.autostartInfoLabel, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.autostartCheckBox, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.patchingCheckBox, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.finish, 0, 12);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 800);
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // autoSaveCheckBox
            //
            this.autoSaveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoSaveCheckBox.AutoSize = true;
            this.autoSaveCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoSaveCheckBox.Location = new System.Drawing.Point(301, 331);
            this.autoSaveCheckBox.Margin = new System.Windows.Forms.Padding(9);
            this.autoSaveCheckBox.Name = "autoSaveCheckBox";
            this.autoSaveCheckBox.Size = new System.Drawing.Size(262, 24);
            this.autoSaveCheckBox.TabIndex = 1;
            this.autoSaveCheckBox.Text = "str_wizard_autosave_button";
            this.autoSaveCheckBox.UseVisualStyleBackColor = true;
            this.autoSaveCheckBox.CheckedChanged += new System.EventHandler(this.ChkAutoSave_CheckedChanged);
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
            this.autoSaveInfoLabel.TabIndex = 6;
            this.autoSaveInfoLabel.Text = "str_wizard_autosave_info";
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
            this.autoSaveTitleLabel.TabIndex = 5;
            this.autoSaveTitleLabel.Text = "str_wizard_autosave_title";
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
            this.title.TabIndex = 0;
            this.title.Text = "str_wizard_title";
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
            this.contextEntryTitleLabel.TabIndex = 1;
            this.contextEntryTitleLabel.Text = "str_wizard_contextentry_title";
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
            this.contextEntryInfoLabel.TabIndex = 2;
            this.contextEntryInfoLabel.Text = "str_wizard_contextentry_info";
            //
            // contextEntryCheckBox
            //
            this.contextEntryCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.contextEntryCheckBox.AutoSize = true;
            this.contextEntryCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextEntryCheckBox.Location = new System.Drawing.Point(275, 192);
            this.contextEntryCheckBox.Margin = new System.Windows.Forms.Padding(9);
            this.contextEntryCheckBox.Name = "contextEntryCheckBox";
            this.contextEntryCheckBox.Size = new System.Drawing.Size(288, 24);
            this.contextEntryCheckBox.TabIndex = 0;
            this.contextEntryCheckBox.Text = "str_wizard_contextentry_button";
            this.contextEntryCheckBox.UseVisualStyleBackColor = true;
            this.contextEntryCheckBox.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
            //
            // autostartTitleLabel
            //
            this.autostartTitleLabel.AutoSize = true;
            this.autostartTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartTitleLabel.Location = new System.Drawing.Point(8, 116);
            this.autostartTitleLabel.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.autostartTitleLabel.Name = "autostartTitleLabel";
            this.autostartTitleLabel.Size = new System.Drawing.Size(343, 32);
            this.autostartTitleLabel.TabIndex = 1;
            this.autostartTitleLabel.Text = "str_wizard_tray_title";
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
            this.autostartInfoLabel.TabIndex = 2;
            this.autostartInfoLabel.Text = "str_wizard_tray_info";
            //
            // autostartCheckBox
            //
            this.autostartCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autostartCheckBox.AutoSize = true;
            this.autostartCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartCheckBox.Location = new System.Drawing.Point(275, 192);
            this.autostartCheckBox.Margin = new System.Windows.Forms.Padding(9, 9, 9, 0);
            this.autostartCheckBox.Name = "autostartCheckBox";
            this.autostartCheckBox.Size = new System.Drawing.Size(288, 24);
            this.autostartCheckBox.TabIndex = 0;
            this.autostartCheckBox.Text = "str_wizard_tray_autostart_button";
            this.autostartCheckBox.UseVisualStyleBackColor = true;
            this.autostartCheckBox.CheckedChanged += new System.EventHandler(this.ChkAutostart_CheckedChanged);
            //
            // patchingCheckBox
            //
            this.patchingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.patchingCheckBox.AutoSize = true;
            this.patchingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patchingCheckBox.Location = new System.Drawing.Point(248, 341);
            this.patchingCheckBox.Margin = new System.Windows.Forms.Padding(9, 0, 9, 9);
            this.patchingCheckBox.Name = "patchingCheckBox";
            this.patchingCheckBox.Size = new System.Drawing.Size(210, 17);
            this.patchingCheckBox.TabIndex = 8;
            this.patchingCheckBox.Text = "str_wizard_tray_patching_button";
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
            this.finish.TabIndex = 2;
            this.finish.Text = "str_wizard_finish";
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
            this.version.TabIndex = 7;
            this.version.Text = "version";
            this.version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // Wizard
            //
            this.AcceptButton = this.finish;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(700, 870);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(700, 870);
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.Wizard_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox patchingCheckBox;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Button finish;
        private System.Windows.Forms.CheckBox contextEntryCheckBox;
        private System.Windows.Forms.Label autoSaveTitleLabel;
        private System.Windows.Forms.Label contextEntryInfoLabel;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.CheckBox autoSaveCheckBox;
        private System.Windows.Forms.Label contextEntryTitleLabel;
        private System.Windows.Forms.Label autostartTitleLabel;
        private System.Windows.Forms.CheckBox autostartCheckBox;
        private System.Windows.Forms.Label autostartInfoLabel;
        private System.Windows.Forms.Label autoSaveInfoLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion
    }
}
