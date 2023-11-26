using System.ComponentModel;
using System.Drawing;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    partial class TemplateEdit {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.labelTemplate = new System.Windows.Forms.Label();
            this.textTemplate = new System.Windows.Forms.ComboBox();
            this.labelInfo = new System.Windows.Forms.LinkLabel();
            this.labelPreview = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelTemplate, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textTemplate, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelInfo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelPreview, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonAccept, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(478, 244);
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // labelTemplate
            //
            this.labelTemplate.AutoSize = true;
            this.labelTemplate.Location = new System.Drawing.Point(4, 43);
            this.labelTemplate.Margin = new System.Windows.Forms.Padding(0);
            this.labelTemplate.Name = "labelTemplate";
            this.labelTemplate.Size = new System.Drawing.Size(167, 20);
            this.labelTemplate.TabIndex = 5;
            this.labelTemplate.Text = Resources.str_filename_template;
            //
            // textTemplate
            //
            this.textTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textTemplate, 2);
            this.textTemplate.FormattingEnabled = true;
            this.textTemplate.Location = new System.Drawing.Point(8, 67);
            this.textTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.textTemplate.Name = "textTemplate";
            this.textTemplate.Size = new System.Drawing.Size(462, 28);
            this.textTemplate.TabIndex = 0;
            this.textTemplate.SelectedIndexChanged += new System.EventHandler(this.textTemplate_Update);
            this.textTemplate.TextUpdate += new System.EventHandler(this.textTemplate_Update);
            //
            // labelInfo
            //
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelInfo, 2);
            this.labelInfo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(8, 8);
            this.labelInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 14);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(462, 21);
            this.labelInfo.TabIndex = 3;
            this.labelInfo.TabStop = true;
            this.labelInfo.Text = Resources.str_template_edit_info;
            this.labelInfo.LinkColor = Color.DodgerBlue;
            this.labelInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelInfo_LinkClicked);
            //
            // labelPreview
            //
            this.labelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPreview.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelPreview, 2);
            this.labelPreview.Location = new System.Drawing.Point(8, 103);
            this.labelPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 14);
            this.labelPreview.Name = "labelPreview";
            this.labelPreview.Size = new System.Drawing.Size(462, 20);
            this.labelPreview.TabIndex = 4;
            this.labelPreview.Text = "...";
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.Location = new System.Drawing.Point(276, 141);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(92, 30);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = Resources.str_cancel;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            //
            // buttonAccept
            //
            this.buttonAccept.AutoSize = true;
            this.buttonAccept.Location = new System.Drawing.Point(376, 141);
            this.buttonAccept.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(94, 30);
            this.buttonAccept.TabIndex = 1;
            this.buttonAccept.Text = Resources.str_accept;
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            //
            // TemplateEdit
            //
            this.AcceptButton = this.buttonAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(478, 244);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateEdit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TemplateEdit";
            this.Load += new System.EventHandler(this.textTemplate_Update);
            this.Shown += new System.EventHandler(this.TemplateEdit_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label labelTemplate;

        private System.Windows.Forms.Button buttonCancel;

        private System.Windows.Forms.Button buttonAccept;

        private System.Windows.Forms.LinkLabel labelInfo;
        private System.Windows.Forms.ComboBox textTemplate;
        private System.Windows.Forms.Label labelPreview;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion
    }
}
