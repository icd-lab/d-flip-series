using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoViewer.Input.PointingDev;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Supplement;
using PhotoViewer.Manager.Resource;

namespace PhotoViewer.Element.StrokeTextbox
{
    public class StrokeBoxCollection
    {
        Dictionary<Stroke, FloatTextBox> StrokeBox = new Dictionary<Stroke, FloatTextBox>();

        public List<Photo> photos
        {
            get;
            set;
        }

        public StrokeBoxCollection()
        {
            photos = new List<Photo>();
        }

        public Stroke createStroke(Vector2 pos)
        {
            Stroke s = new Stroke(pos, photos);
            StrokeBox[s] = null;
            setColor(s);
            return s;
        }

        //public void remove(Stroke s)
        //{
        //    if (StrokeBox.ContainsKey(s))
        //    {
        //        if (StrokeBox[s] != null)
        //            StrokeBox[s].Dispose();
        //        StrokeBox.Remove(s);
        //    }

        //}

        public int Count
        {
            get
            {
                return StrokeBox.Count;
            }
        }

        float hue = 0f;
        Vector3 tempColor;

        private void setColor(Stroke s)
        {
            tempColor = new Vector3(hue, 1f, 1f);
            Vector3 strokeColor = Vector3.Zero;
            ResourceManager.hsv2rgb(ref tempColor, out strokeColor);
            //Color sColor = new Color(strokeColor);
            s.Color = new Color(strokeColor);
            hue += 0.3f;
            if (hue > 1f)
                hue -= (int)hue;
        }

        public void render()
        {
            foreach (Stroke s in StrokeBox.Keys)
            {
                if (s.IsDragged)
                {
                    s.RenderBezier();
                }
                else
                {
                    s.Render();
                }
            }
        }

        public List<FloatTextBox> textboxList
        {
            get
            {
                return new List<FloatTextBox>(StrokeBox.Values);
            }
        }

        public List<Stroke> strokeList
        {
            get
            {
                return new List<Stroke>(StrokeBox.Keys);
            }
        }

        public void remove(Stroke s)
        {
            if (StrokeBox.ContainsKey(s))
            {
                if (StrokeBox[s] != null)
                    StrokeBox[s].Dispose();
                StrokeBox.Remove(s);
                //photoInStroke.Remove(s);
                //strokeBoundingbox.Remove(s);
            }

        }

        public void renderStatic()
        {
            foreach (Stroke s in StrokeBox.Keys)
            {
                if (s.IsClosed && !s.IsDragged)
                {
                    s.Render();
                }
            }
        }

        public void renderDynamic()
        {


            foreach (Stroke s in StrokeBox.Keys)
            {
                if (!s.IsClosed)
                {
                    s.Render();
                }
                else if (s.IsDragged)
                {
                    s.RenderBezier();
                }
            }
        }

        public void renderTags()
        {
            foreach (Stroke s in StrokeBox.Keys)
            {
                if (s.Tags.Count > 0 && StrokeBox[s].IsShown == false)
                {
                    Vector2 size = s.renderTag();
                    if (size == Vector2.Zero)
                    {
                        StrokeBox[s].showAgain(s.Strokes[s.Strokes.Count - 2]);
                        
                    }
                    s.boundingbox = new BoundingBox2D(s.Strokes[s.Strokes.Count - 2], s.Strokes[s.Strokes.Count - 2] + size, 0);
                }
            }
        }

        public void createTextBox(Vector2 pos, Stroke s)
        {
            FloatTextBox box = new FloatTextBox();
            //textBoxes.Add(box);
            StrokeBox[s] = box;
            box.ShowTextBox(pos, s);
        
        }

        public bool underMouse(PointingDevice pd)
        {
            foreach (var tb in StrokeBox.Values)
            {
                if (tb != null && tb.boundingBox.Contains(pd.GamePosition) == ContainmentType.Contains && tb.IsShown)
                {
                    return true;
                }
            }
            foreach (var s in StrokeBox.Keys)
            {
                var box = s.boundingbox;
                if (box.Contains(pd.GamePosition) == ContainmentType.Contains)
                {
                    if (pd.oldLeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && pd.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        StrokeBox[s].showAgain(s.Strokes[s.Strokes.Count - 2]);
                    return true;
                }
            }
            return false;
        }

        public void PositionUpdate()
        {
            foreach (var s in StrokeBox.Keys)
            {
                var box = StrokeBox[s];
                if(box != null)
                box.Location = new System.Drawing.Point((int)(s.Strokes[s.Strokes.Count - 2].X + Browser.Instance.clientBounds.Min.X), (int)(s.Strokes[s.Strokes.Count - 2].Y + Browser.Instance.clientBounds.Min.Y));
            }
        }
    }

}
