using System;
using System.Collections.Generic;
using System.ComponentModel;
using dflip.Supplement;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using dflip.Element;

namespace dflip
{
    public partial class FloatTextBox : Form
    {
        const int width = 268;
        const int height = 30;
        Stroke s;
        public List<string> tags
        {
            get;
            private set;
        }
        #region property
        public bool IsShown
        {
            get;
            private set;
        }
        #endregion

        public FloatTextBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            
        }

        public Vector2 ShowTextBox(Vector2 pos, Stroke st)
        {
            this.Location = new System.Drawing.Point((int)(pos.X + SystemParameter.clientBounds.Min.X), (int)(pos.Y + SystemParameter.clientBounds.Min.Y));
            this.Show();
            IsShown = true;
            boundingBox = new BoundingBox2D(new Vector2(pos.X , pos.Y), new Vector2(pos.X + width, pos.Y + height), 0f);
            s = st;
            return new Vector2(pos.X + this.Width, pos.Y);
        }

        public void PostTags()
        {
            char[] spritChar = { ' ', '　', ',', '，', '、' };
            string[] tempText = this.textBox1.Text.Split(spritChar, StringSplitOptions.RemoveEmptyEntries);

            if (tempText.Length == 0)
            {
                s.Tags = new List<string>();
                s.photoCal();
                return;
            }
            IsShown = false;
            this.Hide();

            s.Tags = tempText.ToList<string>();
            s.photoCal();
        }

        public Vector2 showAgain(Vector2 pos)
        {
            this.Location = new System.Drawing.Point((int)(pos.X + SystemParameter.clientBounds.Min.X), (int)(pos.Y + SystemParameter.clientBounds.Min.Y));
            this.Show();
            IsShown = true;
            boundingBox = new BoundingBox2D(new Vector2(pos.X, pos.Y), new Vector2(pos.X + width, pos.Y + height), 0f);
            return new Vector2(pos.X + this.Width, pos.Y);
        }

        public BoundingBox2D boundingBox
        {
            get;
            private set;
        }

        void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new System.NotImplementedException();
            if (e.Control & e.KeyCode == Keys.A)
            {
                textBox1.SelectAll();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                this.PostTags();
            }
        }

    }
}
