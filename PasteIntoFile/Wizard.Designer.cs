using System.Drawing;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    partial class Wizard
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.subtitle1 = new System.Windows.Forms.Label();
            this.text1 = new System.Windows.Forms.Label();
            this.chkContextEntry = new System.Windows.Forms.CheckBox();
            this.finish = new System.Windows.Forms.Button();
            this.version = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkAutoSave, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.title, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.subtitle1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.text1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkContextEntry, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.finish, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.version, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.RowCount = 10;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(576, 542);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkAutoSave.Location = new System.Drawing.Point(301, 331);
            this.chkAutoSave.Margin = new System.Windows.Forms.Padding(9);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(262, 24);
            this.chkAutoSave.TabIndex = 1;
            this.chkAutoSave.Text = "str_wizard_autosave_button";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            this.chkAutoSave.CheckedChanged += new System.EventHandler(this.ChkAutoSave_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label2.Location = new System.Drawing.Point(8, 295);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(560, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "str_wizard_autosave_info";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label1.Location = new System.Drawing.Point(8, 255);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(560, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "str_wizard_autosave_title";
            // 
            // title
            // 
            this.title.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.title.Location = new System.Drawing.Point(8, 8);
            this.title.Margin = new System.Windows.Forms.Padding(4);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(560, 54);
            this.title.TabIndex = 0;
            this.title.Text = "str_wizard_title";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // subtitle1
            // 
            this.subtitle1.AutoSize = true;
            this.subtitle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.subtitle1.Location = new System.Drawing.Point(8, 116);
            this.subtitle1.Margin = new System.Windows.Forms.Padding(4, 30, 4, 4);
            this.subtitle1.Name = "subtitle1";
            this.subtitle1.Size = new System.Drawing.Size(343, 32);
            this.subtitle1.TabIndex = 1;
            this.subtitle1.Text = "str_wizard_contextentry_title";
            // 
            // text1
            // 
            this.text1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.text1.AutoSize = true;
            this.text1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.text1.Location = new System.Drawing.Point(8, 156);
            this.text1.Margin = new System.Windows.Forms.Padding(4);
            this.text1.Name = "text1";
            this.text1.Size = new System.Drawing.Size(560, 23);
            this.text1.TabIndex = 2;
            this.text1.Text = "str_wizard_contextentry_info";
            // 
            // chkContextEntry
            // 
            this.chkContextEntry.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkContextEntry.AutoSize = true;
            this.chkContextEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkContextEntry.Location = new System.Drawing.Point(275, 192);
            this.chkContextEntry.Margin = new System.Windows.Forms.Padding(9);
            this.chkContextEntry.Name = "chkContextEntry";
            this.chkContextEntry.Size = new System.Drawing.Size(288, 24);
            this.chkContextEntry.TabIndex = 0;
            this.chkContextEntry.Text = "str_wizard_contextentry_button";
            this.chkContextEntry.UseVisualStyleBackColor = true;
            this.chkContextEntry.CheckedChanged += new System.EventHandler(this.ChkContextEntry_CheckedChanged);
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
            this.ClientSize = new System.Drawing.Size(576, 542);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(589, 572);
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.Wizard_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Button finish;
        private System.Windows.Forms.CheckBox chkContextEntry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label text1;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.Label subtitle1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion
    }
}