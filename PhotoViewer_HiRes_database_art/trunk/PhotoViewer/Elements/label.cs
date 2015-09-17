using System.Windows.Forms;
using System.Drawing;

namespace PhotoViewer.Element.Label
{
    public partial class Wall : Form
    {
        public Wall()
        {
            InitializeComponent();
            this.Show();
        }

        public void UpdateWall()
        {
            this.Location = new Point((int)Browser.Instance.clientBounds.Min.X, (int)Browser.Instance.clientBounds.Min.Y);
            this.Size = new Size(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight);
        }

        public void AddLabel(System.Windows.Forms.DataGridView label)
        {
            labelList.Add(label);
            
        }

        public bool IfFocused()
        {
            foreach (var la in labelList)
            {
                if (la.Focused)
                    return true;
            }
            return false;
        }
    }

    
}
