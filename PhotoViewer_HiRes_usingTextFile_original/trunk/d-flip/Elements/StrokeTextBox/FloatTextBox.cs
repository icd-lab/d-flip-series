using System;
using System.Collections.Generic;
using System.ComponentModel;
using PhotoViewer.Supplement;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using PhotoViewer.Element;

namespace PhotoViewer
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
        #region 属性封装
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

        public void ShowTextBox(Vector2 pos, Stroke st)
        {
            this.Location = new System.Drawing.Point((int)(pos.X + SystemParameter.clientBounds.Min.X), (int)(pos.Y + SystemParameter.clientBounds.Min.Y));
            this.Show();
            IsShown = true;
            boundingBox = new BoundingBox2D(new Vector2(pos.X , pos.Y), new Vector2(pos.X + width, pos.Y + height), 0f);
            s = st;
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

        public void showAgain(Vector2 pos)
        {
            this.Location = new System.Drawing.Point((int)(pos.X + SystemParameter.clientBounds.Min.X), (int)(pos.Y + SystemParameter.clientBounds.Min.Y));
            this.Show();
            IsShown = true;
            boundingBox = new BoundingBox2D(new Vector2(pos.X, pos.Y), new Vector2(pos.X + width, pos.Y + height), 0f);
        }

        public BoundingBox2D boundingBox
        {
            get;
            private set;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //base.OnKeyPress(e);

            if ((Keys)e.KeyChar == Keys.Enter)
            {
                this.PostTags();
            }
        }

    }
}
