using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using PhotoViewer.Manager.Resource;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Input.PointingDev;
using PhotoViewer.Supplement;

namespace PhotoViewer.Element.StrokeTextbox
{
    public class Stroke
    {
        private List<Vector2> strokes_ = new List<Vector2>();
        private List<string> tags_ = new List<string>();
        private Vector2 center_ = Vector2.Zero;
        private bool isClosed_ = false;
        private bool isDragged_ = false;
        private Color color_ = Color.Black;
        private PointingDevice pointingDevice = null;                  // only one pointing device can connect to stroke at one time

        // stroke的间隔
        public static readonly int StrokeTh = 32;
        private bool error = false;

        #region 属性封装
        public List<Vector2> Strokes
        {
            get
            {
                return strokes_;
            }
        }
        public List<string> Tags
        {
            get
            {
                return tags_;
            }
            set
            {
                tags_ = value;
                error = false;
            }
        }
        public Vector2 Center
        {
            get
            {
                return center_;
            }
        }
        public bool IsClosed
        {
            get
            {
                return isClosed_;
            }
        }
        public bool IsDragged
        {
            get
            {
                return isDragged_;
            }
        }
        public Vector2 Last
        {
            get
            {
                return strokes_[strokes_.Count - 1];
            }
        }
        public Color Color
        {
            get
            {
                return color_;
            }
            set
            {
                color_ = value;
            }
        }
        public List<Photo> photos
        {
            private get;
            set;
        }
        public List<Photo> relatedPhotos
        {
            get;
            private set;
        }
        #endregion

        public Stroke(Vector2 root, List<Photo> p)
        {
            strokes_.Add(root);
            photos = p;
            relatedPhotos = new List<Photo>();
        }

        public void AddStroke(Vector2 ap)
        {
            strokes_.Add(ap);
        }

        public void photoCal()
        {
            relatedPhotos.Clear();
            foreach (var p in photos)
            {
                foreach (string t in Tags)
                {
                    if (p.containTag(t))
                    {
                        //++matchedTagCount;
                        relatedPhotos.Add(p);
                        break;
                    }
                }
            }
        }

        public bool touchDelete(PointingDevice pd)
        {
            Vector2 ndist = Vector2.One * float.MaxValue;
            for (int k = 0, klen = Strokes.Count - 1; k < klen; ++k)
            {
                Vector2 dist = Strokes[k] - pd.GamePosition;
                if (dist.LengthSquared() < ndist.LengthSquared())
                {
                    enlargeIndex = k;
                    ndist = dist;
                }
            }
            if (ndist.Length() <= dragTh_)
            {
                return true;
            }
            return false;
        }

        public void EnlargeStroke(int index, Vector2 v)
        {
            int sc = strokes_.Count - 1;
            Vector2 basev = strokes_[index] - center_;
            float baseLen = basev.Length();
            {
                basev.Normalize();
                float length = basev.X * v.X + basev.Y * v.Y;
                //
                float lengthh = basev.X * v.Y - basev.Y * v.X;
                Vector2 basevv = new Vector2(-basev.Y, basev.X);
                basevv *= lengthh;
                //
                for (int i = 0; i < sc; ++i)
                {
                    Vector2 dir = strokes_[i] - center_;
                    float dirLen = dir.Length();
                    dir.Normalize();
                    strokes_[i] += dir * length * dirLen / baseLen;
                    //
                    strokes_[i] += basevv;
                    //
                }
                strokes_[sc] = strokes_[0];
                //
                center_ += basevv;
                //
            }
        }

        public void MoveStroke(int index, Vector2 v)
        {
            int sc = strokes_.Count - 1;
            strokes_[index] += v;
            center_ += v / (float)sc;
            if (index == 0)
            {
                strokes_[sc] += v;
            }
        }

        public void MoveStroke(Vector2 v)
        {
            for(int i = 0, len = strokes_.Count; i < len; ++i)
            {
                strokes_[i] += v;
            }
            center_ += v;
        }

        public void BeginMove()
        {
            isDragged_ = true;
        }

        public void EndMove()
        {
            isDragged_ = false;
        }

        public void End()
        {
            foreach (Vector2 s in strokes_)
            {
                center_ += s;
            }
            center_ /= (float)strokes_.Count;
            strokes_.Add(strokes_[0]);
            isClosed_ = true;
        }

        public void AddTags(List<string> ts)
        {
            foreach (string t in ts)
            {
                if (!tags_.Contains(t))
                {
                    tags_.Add(t);
                }
            }
            tags_.Sort();
        }

        public bool IsInternal(Vector2 target)
        {
            int trueCount = 0;
            List<Vector2> roots = new List<Vector2>();
            roots.Add(-1024 * Vector2.UnitX - 1024 * Vector2.UnitY);
            roots.Add(-1024 * Vector2.UnitX + (768 + 1024) * Vector2.UnitY);
            roots.Add((1024 + 1024) * Vector2.UnitX + (768 + 1024) * Vector2.UnitY);
            roots.Add((1024 + 1024) * Vector2.UnitX - 1024 * Vector2.UnitY);
            foreach (Vector2 r in roots)
            {
                int crossCount = 0;
                for (int i = 0, len = strokes_.Count; i < len - 1; ++i)
                {
                    Vector2 now = strokes_[i];
                    Vector2 next = strokes_[i + 1];
                    Vector2 a = target - r;
                    Vector2 b = now - r;
                    int s1 = Math.Sign(a.X * b.Y - a.Y * b.X);
                    b = next - r;
                    int s2 = Math.Sign(a.X * b.Y - a.Y * b.X);
                    if (s1 != s2)
                    {
                        a = next - now;
                        b = r - now;
                        s1 = Math.Sign(a.X * b.Y - a.Y * b.X);
                        b = target - now;
                        s2 = Math.Sign(a.X * b.Y - a.Y * b.X);
                        if (s1 != s2)
                        {
                            ++crossCount;
                        }
                    }
                }
                if (crossCount % 2 == 1)
                {
                    ++trueCount;
                }
            }
            if (trueCount > (roots.Count + 1) / 2 - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // 拖拽锚点
        private int dragTh_ = 50;
        public enum StrokeState
        {
            DragStroke,
            ExpansionStroke,
            NOTHING
        }

        private int state = (int)StrokeState.NOTHING;
        private int enlargeIndex;
        public int Interact(PointingDevice pd)
        {
            if (pointingDevice == null)
            {
                if (pd.LeftButton == ButtonState.Pressed && pd.oldLeftButton == ButtonState.Released)
                {
                    if (!IsClosed)
                    {
                        state = (int)StrokeState.NOTHING;
                        return state;
                    }
                    Vector2 ndist = Vector2.One * float.MaxValue;
                    for (int k = 0, klen = Strokes.Count - 1; k < klen; ++k)
                    {
                        Vector2 dist = Strokes[k] - pd.GamePosition;
                        if (dist.LengthSquared() < ndist.LengthSquared())
                        {
                            //dragSIndexes_[i][j] = k;
                            enlargeIndex = k;
                            ndist = dist;
                            //s[j].BeginMove();
                        }
                    }
                    if (ndist.Length() > dragTh_ && IsInternal(pd.GamePosition))
                    {
                        BeginMove();
                        state = (int)Stroke.StrokeState.DragStroke;
                        pointingDevice = pd;
                    }
                    else if (ndist.Length() <= dragTh_)
                    {
                        state = (int)Stroke.StrokeState.ExpansionStroke;
                        pointingDevice = pd;
                    }
                }
            }
            else if (pointingDevice == pd)
            {
                // 左键移动锚点
                if (pd.LeftButton == ButtonState.Pressed)
                {
                    Vector2 mov = pd.GamePosition - pd.OldGamePosition;
                    if (state == (int)Stroke.StrokeState.DragStroke)
                    {

                        MoveStroke(mov);

                    }
                    else if (state == (int)Stroke.StrokeState.ExpansionStroke)
                    {
                        EnlargeStroke(enlargeIndex, mov);

                    }
                }
                else if (pd.LeftButton == ButtonState.Released && pd.oldLeftButton == ButtonState.Pressed)
                {
                    // 拖动结束
                    if (state == (int)Stroke.StrokeState.DragStroke)
                        EndMove();
                    pointingDevice = null;
                    state = (int)Stroke.StrokeState.NOTHING;
                }
            }
            return state;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public Vector2 renderTag()
        {
            if (error)
                return Vector2.Zero;
            if (this.tags_.Count > 0)
            {
                try
                {
                    int width = 5;
                    int height = (int)ResourceManager.font_.MeasureString(tags_[0]).Y;
                    foreach (var tag in tags_)
                    {
                        width += (int)ResourceManager.font_.MeasureString(tag).X + 5;
                    }
                    Browser.Instance.batch_.Draw(ResourceManager.frameSquare_, new Rectangle((int)this.Strokes[strokes_.Count - 2].X, (int)this.Strokes[strokes_.Count - 2].Y, width, height), Color.White);
                    width = 5;
                    foreach (var tag in tags_)
                    {
                        Browser.Instance.batch_.DrawString(ResourceManager.font_, tag, new Vector2(this.Strokes[strokes_.Count - 2].X + width, strokes_[strokes_.Count - 2].Y), this.Color);
                        width += (int)ResourceManager.font_.MeasureString(tag).X + 5;
                    }
                    return new Vector2(width, ResourceManager.font_.MeasureString(tags_[0]).Y);
                }
                catch (Exception e)
                {
                    error = true;
                    MessageBox(new IntPtr(0), "System can't display these characters", "Error Displaying Characters", 0);
                }
            }
            return Vector2.Zero;
        }

        public BoundingBox2D boundingbox
        {
            get;
            set;
        }

        public void Render()
        {
            if (strokes_.Count > 0)
            {
                //b.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend);
                for (int i = 0, len = strokes_.Count; i < len - 1; ++i)
                {
                    Vector2 v = strokes_[i + 1] - strokes_[i];
                    int vlen = 1 + (int)(v.Length());
                    v.Normalize();
                    for (int j = 0; j < vlen; ++j)
                    {
                        Browser.Instance.batch_.Draw(ResourceManager.stroke_, strokes_[i] - ((float)ResourceManager.stroke_.Width * 0.5f) * Vector2.One + v * (float)j, color_);
                    }
                }
                if (!isClosed_)
                {
                    int last = strokes_.Count - 1;
                    Vector2 v = strokes_[0] - strokes_[last];
                    int vlen = 1 + (int)(v.Length());
                    v.Normalize();
                    for (int j = 0; j < vlen; j += 10)
                    {
                        Browser.Instance.batch_.Draw(ResourceManager.stroke_, strokes_[last] - ((float)ResourceManager.stroke_.Width * 0.5f) * Vector2.One + v * (float)j, color_);
                        Browser.Instance.batch_.Draw(ResourceManager.stroke_, strokes_[last] - ((float)ResourceManager.stroke_.Width * 0.5f) * Vector2.One + v * (float)(j + 2), color_);
                        Browser.Instance.batch_.Draw(ResourceManager.stroke_, strokes_[last] - ((float)ResourceManager.stroke_.Width * 0.5f) * Vector2.One + v * (float)(j + 4), color_);
                    }
                }
                //b.End();
            }
        }

        public void RenderBezier()
        {
            if (strokes_.Count > 0)
            {
                //b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                for (int i = 0, len = strokes_.Count - 1; i < len; ++i)
                {
                    double smoothness = 10d;
                    double x1 = strokes_[i].X;
                    double y1 = strokes_[i].Y;
                    double x4 = strokes_[(i + 1) % len].X;
                    double y4 = strokes_[(i + 1) % len].Y;
                    double x2 = x4 - strokes_[(i - 1 + len) % len].X;
                    x2 /= smoothness;
                    x2 += x1;
                    double y2 = y4 - strokes_[(i - 1 + len) % len].Y;
                    y2 /= smoothness;
                    y2 += y1;
                    double x3 = x1 - strokes_[(i + 2) % len].X;
                    x3 /= smoothness;
                    x3 += x4;
                    double y3 = y1 - strokes_[(i + 2) % len].Y;
                    y3 /= smoothness;
                    y3 += y4;
                    int jlen = (int)(Math.Max(32, (strokes_[i + 1] - strokes_[i]).Length()));
                    for (int j = 0; j < jlen; ++j)
                    {
                        double dj = (double)j;
                        double djlen = (double)jlen;
                        double x = x1 * (1d - dj / djlen) * (1d - dj / djlen) * (1d - dj / djlen) + 3d * x2 * (1d - dj / djlen) * (1d - dj / djlen) * dj / djlen + 3d * x3 * (1d - dj / djlen) * dj * dj / (djlen * djlen) + x4 * dj * dj * dj / (djlen * djlen * djlen);
                        double y = y1 * (1d - dj / djlen) * (1d - dj / djlen) * (1d - dj / djlen) + 3d * y2 * (1d - dj / djlen) * (1d - dj / djlen) * dj / djlen + 3d * y3 * (1d - dj / djlen) * dj * dj / (djlen * djlen) + y4 * dj * dj * dj / (djlen * djlen * djlen);
                        Browser.Instance.batch_.Draw(ResourceManager.stroke_, new Vector2((float)x, (float)y) - ((float)ResourceManager.stroke_.Width * 0.5f) * Vector2.One, color_);
                    }
                }
                //b.End();
            }
        }
    }
}
