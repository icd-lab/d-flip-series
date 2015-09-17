using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoViewer.Element;
using PhotoViewer.InputDevice;
using PhotoInfo;
using PhotoViewer.Supplement;

namespace PhotoViewer.Manager
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

        public List<string> checkInside(Vector2 pos, Photo p)
        {
            List<string> oldTags = p.ptag.allTags;
            List<string> tags = new List<string>();
            foreach (var s in StrokeBox.Keys)
            {
                if (s.IsInternal(pos))
                {
                    foreach(var tag in s.Tags)
                        if (!oldTags.Contains(tag))
                        {
                            tags.Add(tag);
                            s.addPhoto(p);
                        }
                }
            }
            return tags;
        }

        public List<string> checkOutside(Vector2 pos, Photo p)
        {
            List<string> oldTags = p.ptag.allTags;
            List<string> tags = new List<string>();
            foreach (var s in StrokeBox.Keys)
            {
                if (!s.IsInternal(pos))
                {
                    foreach (var tag in s.Tags)
                        if (oldTags.Contains(tag))
                        {
                            tags.Add(tag);
                            s.removePhoto(p);
                        }
                }
            }
            return tags;
        }

        public Stroke createStroke(Vector2 pos)
        {
            Stroke s = new Stroke(pos, photos);
            StrokeBox[s] = null;
            setColor(s);
            return s;
        }

        public void remove(Stroke s)
        {
            if (StrokeBox.ContainsKey(s))
            {
                if (StrokeBox[s] != null)
                    StrokeBox[s].Dispose();
                StrokeBox.Remove(s);
            }

        }

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

        public void renderTags()
        {
            foreach (Stroke s in StrokeBox.Keys)
            {
                if (s.Tags.Count > 0 && StrokeBox[s].IsShown == false)
                {
                    Vector2 size = s.renderTag();
                    s.boundingbox = new BoundingBox2D(s.Strokes[s.Strokes.Count - 2], s.Strokes[s.Strokes.Count - 2] + size, 0);
                }
            }
        }

        public void recalPhoto()
        {
            foreach (var s in StrokeBox.Keys)
            {
                s.photos = photos;
                s.photoCal();

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
                else if(s.IsDragged)
                {
                    s.RenderBezier();
                }
            }
        }

        public List<Stroke> strokeList
        {
            get
            {
                return new List<Stroke>(StrokeBox.Keys);
            }
        }
        
        //public void remove()
        //{
        //    List<Stroke> strokes = new List<Stroke>(StrokeBox.Keys);
        //    for (int i = 0; i < strokes.Count; i++)
        //    {
        //        FloatTextBox box = StrokeBox[strokes[i]];
        //        if (box != null && box.IsShown == false)
        //        {
        //            strokes[i].AddTags(box.tags);
        //            box.Dispose();
        //            StrokeBox[strokes[i]] = null;
        //        }
        //    }
        //}
        
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
    }


}
