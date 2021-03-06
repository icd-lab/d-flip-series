﻿namespace PhotoViewer.Element.ProgressBar
{
    partial class ProgressBarForm
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
            this.progressTextureBar = new System.Windows.Forms.ProgressBar();
            this.progressNameBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // progressTextureBar
            // 
            this.progressTextureBar.BackColor = System.Drawing.Color.White;
            this.progressTextureBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressTextureBar.ForeColor = System.Drawing.Color.LawnGreen;
            this.progressTextureBar.Location = new System.Drawing.Point(0, 24);
            this.progressTextureBar.Name = "progressTextureBar";
            this.progressTextureBar.Size = new System.Drawing.Size(352, 23);
            this.progressTextureBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressTextureBar.TabIndex = 0;
            // 
            // progressNameBar
            // 
            this.progressNameBar.BackColor = System.Drawing.Color.White;
            this.progressNameBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressNameBar.ForeColor = System.Drawing.Color.LawnGreen;
            this.progressNameBar.Location = new System.Drawing.Point(0, 0);
            this.progressNameBar.Name = "progressNameBar";
            this.progressNameBar.Size = new System.Drawing.Size(352, 23);
            this.progressNameBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressNameBar.TabIndex = 1;
            // 
            // ProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(352, 47);
            this.ControlBox = false;
            this.Controls.Add(this.progressNameBar);
            this.Controls.Add(this.progressTextureBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(129, 151);
            this.Name = "ProgressBarForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Now Loading...";
            this.SizeChanged += new System.EventHandler(this.ProgressBarForm_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressTextureBar;
        private System.Windows.Forms.ProgressBar progressNameBar;
    }
}