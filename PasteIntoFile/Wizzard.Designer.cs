using System.Drawing;
using PasteIntoFile.Properties;

namespace PasteIntoFile
{
    partial class Wizzard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstLaunch));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.first = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.second = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.first.SuspendLayout();
            this.second.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(778, 300);
            this.label1.TabIndex = 0;
            this.label1.Text = Resources.str_firstlaunch_welcome;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(778, 403);
            this.panel1.TabIndex = 1;
            // 
            // first
            // 
            this.first.Controls.Add(this.button1);
            this.first.Controls.Add(this.panel1);
            this.first.Dock = System.Windows.Forms.DockStyle.Fill;
            this.first.Location = new System.Drawing.Point(0, 0);
            this.first.Name = "first";
            this.first.Size = new System.Drawing.Size(778, 544);
            this.first.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackgroundImage = global::PasteIntoFile.Properties.Resources.next;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(339, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 86);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // second
            // 
            this.second.Controls.Add(this.label5);
            this.second.Controls.Add(this.label4);
            this.second.Controls.Add(this.button3);
            this.second.Controls.Add(this.button2);
            this.second.Controls.Add(this.label3);
            this.second.Controls.Add(this.label2);
            this.second.Dock = System.Windows.Forms.DockStyle.Fill;
            this.second.Location = new System.Drawing.Point(0, 0);
            this.second.Name = "second";
            this.second.Size = new System.Drawing.Size(778, 544);
            this.second.TabIndex = 4;
            this.second.Visible = false;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(445, 303);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(192, 87);
            this.button3.TabIndex = 6;
            this.button3.Text = Resources.str_no;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(141, 303);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 87);
            this.button2.TabIndex = 5;
            this.button2.Text = Resources.str_contextentry_register;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 99);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.label3.Size = new System.Drawing.Size(778, 200);
            this.label3.TabIndex = 4;
            this.label3.Text = string.Format(Resources.str_firstlaunch_register_explain, Resources.str_contextentry);
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(6, 10, 15, 10);
            this.label2.Size = new System.Drawing.Size(778, 99);
            this.label2.TabIndex = 1;
            this.label2.Text = Resources.str_firstlaunch_register;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FirstLaunch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 444);
            this.Controls.Add(this.second);
            this.Controls.Add(this.first);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = Resources.icon;
            this.MaximizeBox = false;
            this.Name = "FirstLaunch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Resources.str_firstlaunch_windowtitle;
            this.panel1.ResumeLayout(false);
            this.first.ResumeLayout(false);
            this.second.ResumeLayout(false);
            this.second.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel first;
        private System.Windows.Forms.Panel second;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}