﻿namespace dflip
{
    partial class FloatTextBox
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            
            this.textBox1.Size = new System.Drawing.Size(width, height);
            this.Size = new System.Drawing.Size(width, height);
            this.TabIndex = 0;
            this.Font = new System.Drawing.Font(this.Font.FontFamily, 30);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox1_KeyDown);
            // 
            // FloatTextBox
            // 
            this.Controls.Add(this.textBox1);
            this.textBox1.Multiline = false;
            this.textBox1.ShortcutsEnabled = true;
            this.KeyPreview = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.Name = "FloatTextBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            //this.Text = "Input Some key words.";
            this.TopMost = true;
            //this.TransparencyKey = System.Drawing.Color.Red;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        #endregion

        private System.Windows.Forms.TextBox textBox1;
    }
}