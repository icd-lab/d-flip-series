using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace PhotoViewer.Element.ProgressBar
{
    public partial class ProgressBarForm : Form
    {
        int loadCount_ = 0;
        int loadedName_ = 0;
        int loadedTexture_ = 0;

#region プロパティ
        public bool isBegin
        {
            get
            {
                if (loadCount_ > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
#endregion

        public ProgressBarForm()
        {
            InitializeComponent();
        }

        public void Begin(int load)
        {
            loadCount_ = load;
            loadedName_ = 0;
            progressNameBar.Value = 0;
            loadedTexture_ = 0;
            progressTextureBar.Value = 0;
            this.Show();
        }
        public void Add(int added)
        {
            loadCount_ += added;
        }
        public void End()
        {
            this.Hide();
            loadCount_ = 0;
            loadedName_ = 0;
            loadedTexture_ = 0;
        }

        public void ProgressName()
        {
            ++loadedName_;
            progressNameBar.Value = (int)((float)(loadedName_) * 100f / (float)(loadCount_));
            Text = "Loading Metadata ...";
        }
        public void ProgressName(int x)
        {
            loadedName_ += x;
            progressNameBar.Value = (int)((float)(loadedName_) * 100f / (float)(loadCount_));
            Text = "Loading Metadata ...";
        }
        public void ProgressTexture()
        {
            ++loadedTexture_;
            progressTextureBar.Value = (int)((float)(loadedTexture_) * 100f / (float)(loadCount_));
            Text = "Loading Pixel Data ...";
        }
        public void ProgressTexture(int x)
        {
            loadedTexture_ += x;
            progressTextureBar.Value = (int)((float)(loadedTexture_) * 100f / (float)(loadCount_));
            Text = "Loading Pixel Data ...";
        }

        private void ProgressBarForm_SizeChanged(object sender, EventArgs e)
        {
            this.Size = new Size(358, 79);
        }
    }
}
