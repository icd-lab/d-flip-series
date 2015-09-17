using System;
using System.Linq;
using System.Collections.Generic;
using PhotoViewer.Supplement;
using System.Text;
using Microsoft.Xna.Framework;
using PhotoViewer.Manager.Resource;
using System.Drawing;

namespace PhotoViewer.PhotoInfo.Tag
{
    public class PhotoTag //: IComparable
    {
        Dictionary<String, BoundingBox2D> tagBox = new Dictionary<string,BoundingBox2D>();
        int left = 2;
        int right = 2;
        int up = 2;
        int width = 300;
        int addwidth = 0;
        System.Windows.Forms.DataGridView wallLabel = null;

        public List<String> activeTagList
        {
            get;
            private set;
        }

        public int startDate
        {
            get;
            set;
        }

        public int endDate
        {
            get;
            set;
        }

        public Photo relatedPhoto
        {
            private get;
            set;
        }

        public PhotoTag()
        {
            activeTagList = new List<String>();
            allTags = new List<String>();
           
            //allTags.Add("Color");
        }

        public void UnLoad()
        {
            if (wallLabel != null)
            {
                wallLabel.Dispose();
                wallLabel = null;
            }
        }

        //private Dictionary<String, int> parseText(String text, int width)
        //{
        //    String line = String.Empty;
        //    String returnString = String.Empty;
        //    //String[] wordArray = text.Split(' ');
        //    //count records how many rows it has.
        //    int count = 1;
        //    Dictionary<String, int> result = new Dictionary<String, int>();
        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        char word = text[i];
        //        if (ResourceManager.font_.MeasureString(line + word).X > width)
        //        {
        //            int tempAddwidth = 0;
        //            while (i < text.Length && text[i] != ' ' && ((text[i] >= 'a' && text[i] <= 'z') || (text[i] >= 'A' && text[i] <= 'Z')))
        //            {
        //                word = text[i];
        //                line = line + word;
        //                tempAddwidth += (int)ResourceManager.font_.MeasureString(word.ToString()).X;
        //                i++;
        //            }
        //            if (tempAddwidth > addwidth)
        //                addwidth = tempAddwidth;
        //            if (i < text.Length)
        //            {
        //                returnString = returnString + line + '\n';
        //                count++;
        //                line = String.Empty;
        //            }
        //        }
        //        if(i < text.Length)
        //            line = line + text[i];
        //    }
        //    result[returnString + line] = count;
        //    return result;
        //}

        //left 2; right 2; up & down 4; alignment
        public Vector2 drawTags(int startX, int startY, float angleDisplay, int height, float scale)
        {
            //int YOffset = 0;
            //int resultWidth = 0;
            if (allTags.Count > 1 && wallLabel == null)
            {
                //string result = String.Empty;
                wallLabel = new System.Windows.Forms.DataGridView();
                wallLabel.ColumnCount = 1;
                wallLabel.ColumnHeadersVisible = false;
                wallLabel.RowHeadersVisible = false;
                //change line automatically
                wallLabel.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
                wallLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
                wallLabel.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
                wallLabel.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

                //set column to fill the whole width
                wallLabel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                //background
                wallLabel.BackgroundColor = System.Drawing.Color.White;

                //wallLabel.MultiSelect = true;
                //wallLabel.SelectionChanged += new EventHandler(wallLabel_SelectionChanged);
                wallLabel.ReadOnly = true;
                wallLabel.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(wallLabel_CellMouseClick);
                //wallLabel.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(wallLabel_CellBeginEdit);
                wallLabel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
                wallLabel.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
                wallLabel.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Red;
                //wallLabel.EditMode = System.Windows.Forms.DataGridViewEditMode.
                foreach (String tag in allTags)
                {
                    if(tag.Length > 0)
                        wallLabel.Rows.Add(tag);
                    //Dictionary<String, int> result = parseText(tag, width - left - right);
                    //if (result.Values.First() > 1)
                    //    resultWidth = width + addwidth;
                    //else if (resultWidth < width && resultWidth < ResourceManager.font_.MeasureString(tag).Length() + 5)
                    //    resultWidth = (int)ResourceManager.font_.MeasureString(tag).Length() + 5;
                    //if (activeTagList.Contains(tag))
                    //{
                    //    Browser.Instance.batch_.DrawString(ResourceManager.font_, result.Keys.First(), new Vector2(startX + left, startY + YOffset), Color.Red);
                    //}
                    //else
                    //    Browser.Instance.batch_.DrawString(ResourceManager.font_, result.Keys.First(), new Vector2(startX + left, startY + YOffset), Color.Gray);
                    //Vector2 size = ResourceManager.font_.MeasureString(tag);
                    ////create bounding box for every tag
                    //tagBox[tag] = new BoundingBox2D(new Vector2(startX, startY + YOffset), new Vector2(startX + resultWidth, startY + YOffset + size.Y * result.Values.First()), angleDisplay);
                    //YOffset +=(int) size.Y * result.Values.First()+ up;

                }
                wallLabel.Location = new System.Drawing.Point(startX, startY);
                wallLabel.Size = new System.Drawing.Size(width, height);
                Browser.Instance.wall.Controls.Add(wallLabel);
                wallLabel.ClearSelection();
                wallLabel.Focus();
                Browser.Instance.wall.AddLabel(wallLabel);
            }
            else if (wallLabel != null)
            {
                //wallLabel.Update(new System.Drawing.Point(startX + (int)Browser.Instance.clientBounds.Min.X, startY + (int)Browser.Instance.clientBounds.Min.Y), new System.Drawing.Size((int)(width), height));
                wallLabel.Location = new System.Drawing.Point(startX, startY);
                wallLabel.Size = new System.Drawing.Size(width, height);
               
            }
            return new Vector2(width, 0);
        }

        List<int> selectedRows = new List<int>();

        void wallLabel_CellMouseClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (selectedRows.Contains(e.RowIndex))
            {
                selectedRows.Remove(e.RowIndex);
                wallLabel.Rows[e.RowIndex].Selected = false;
                wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = System.Drawing.Color.White;
                wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = System.Drawing.Color.Black;
                activeTagList.Remove(wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            }
            else
            {
                selectedRows.Add(e.RowIndex);
                wallLabel.Rows[e.RowIndex].Selected = true;
                //Console.WriteLine("selected");
                wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = System.Drawing.Color.White;
                wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = System.Drawing.Color.Red;
                activeTagList.Add(wallLabel.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            }
            //how to decide if it is touch or mouse
            //relatedPhoto.touchCount = 1;
        }

        //return 0: nothing; return 1 turn on/off attractor color
        //public bool clickedCheck(Vector2 clickedPosition)
        //{
        //    if (allTags.Count == 1)
        //    {
        //        if (!activeTagList.Contains(allTags[0]))
        //            activeTagList.Add(allTags[0]);
        //        return true;
        //    }
            
        //    foreach (String tag in allTags)
        //        if (tagBox[tag].Contains(clickedPosition) == ContainmentType.Contains)
        //        {
        //            if (activeTagList.Contains(tag))
        //            {
        //                activeTagList.Remove(tag);
        //            }
        //            else
        //            {
        //                activeTagList.Add(tag);
        //            }
        //            if (tag.Equals("Color"))
        //                return true;
        //            return false;
        //        }
        //    return false;
        //}

        public void hideTag()
        {
            //wallLabel.HideLabel();
            if (wallLabel != null)
            {
                foreach (var index in selectedRows)
                {
                    wallLabel.Rows[index].Cells[0].Style.BackColor = System.Drawing.Color.White;
                    wallLabel.Rows[index].Cells[0].Style.ForeColor = System.Drawing.Color.Black;
                }
                wallLabel.ClearSelection();
                wallLabel.Hide();
            }
        }
        public void showTag()
        {
            if (wallLabel != null)
            {
                wallLabel.Show();
                wallLabel.Focus();
            }
        }

        public void clearActiveTagList()
        {
            activeTagList.Clear();
        }

        public PhotoTag(List<String> tags)
        {
            //tags.Add("Color");
            foreach (String t in tags)
                tagBox[t] = new BoundingBox2D();
            allTags = tags;
            activeTagList = new List<String>();
        }

        //public void createBox(float x, float y, float angle)
        //{
        //    int YOffset = 0;
        //    foreach(String t in allTags)
        //    {
        //        Vector2 size = ResourceManager.font_.MeasureString(t);
        //        tagBox[t] = new BoundingBox2D(new Vector2(x, y + YOffset), new Vector2(x + width, y + YOffset + size.Y), angle);
        //        YOffset += (int)size.Y + up;
        //    }
        //}
        public List<String> allTags
        {
            get;
            private set;
        }
    }
}
