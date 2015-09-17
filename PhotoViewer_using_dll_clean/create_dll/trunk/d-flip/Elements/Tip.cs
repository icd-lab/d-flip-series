using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using dflip.Manager;
using dflip.Supplement;
using System;

namespace dflip.Element
{
    public class Tip
    {
        // テクスチャ fuki.png のピクセル数 227x190 (170x140)
        public static int Width = 227;
        public static int Height = 190;
        public static int InWidth = 170;
        public static int InHeight = 140;

        private Vector2 position_ = Vector2.Zero;
        private int ID_ = -1;
        private Vector2 scale_ = Vector2.Zero;
        private Vector2 vScale_ = Vector2.Zero;
        private Vector2 scaleTarget_ = 0.5f * Vector2.One;
        //private bool isDel_ = false;
        private BoundingBox2D boundingBox_ = new BoundingBox2D();
        private string text_ = string.Empty;
        private FukiType type_ = FukiType.None;

        public enum FukiType
        {
            None,
            DateTime,
            ToolTip,
            PhotoTag,
        }

        #region プロパティ
        public Vector2 Position
        {
            get
            {
                return position_;
            }
            set 
            {
                position_ = value;
            }
        }
        public int ID
        {
            get
            {
                return ID_;
            }
            set
            {
                ID_ = value;
            }
        }
        public bool IsDel
        {
            get;
            set;
        }
        public FukiType Type
        {
            get
            {
                return type_;
            }
        }
        public Vector2 Scale
        {
            get
            {
                return scale_;
            }
        }
        public int Left
        {
            get
            {
                return (int)(position_.X - Width * scale_.X * 0.5f);
            }
        }
        public int Right
        {
            get
            {
                return (int)(position_.X + Width * scale_.X * 0.5f);
            }
        }
        public int Top
        {
            get
            {
                return (int)(position_.Y - Height * scale_.Y * 0.5f);
            }
        }
        public int Bottom
        {
            get
            {
                return (int)(position_.Y + Height * scale_.Y * 0.5f);
            }
        }
        #endregion

        public void Reset()
        {
            scale_ = Vector2.Zero;
        }

        public Tip(DateTime dt, Vector2 pos)
        {
            text_ = dt.ToShortDateString();
            position_ = pos;
            type_ = FukiType.DateTime;
        }
        public Tip(int attractor)//, Vector2 pos)
        {
            ID_ = attractor;
            switch (attractor)
            {
                case SystemState.ATTRACTOR_BOUND:
                        text_ = "Molding";
                        break;
                /*case ControlPanel.ATTRACTOR_AVOID:
                        text_ = "Translation (Avoidance)";
                    break;
                case ControlPanel.ATTRACTOR_AVOIDSCALE:
                    text_ = "Shrink (Avoidance)";
                    break;*/
                case SystemState.TOHOKU_MAP:
                    text_ = "Tohoku Map";
                    break;
                /*case ControlPanel.ATTRACTOR_SCALEUPMOUSE:
                    text_ = "Enlarge (Point)";
                    break;
                case ControlPanel.ATTRACTOR_ANCHOR:
                    text_ = "Anchor (Point)";
                    break;
                //case ControlPanel.ATTRACTOR_COLOR:
                case ControlPanel.ATTRACTOR_TAG:
                    text_ = "Attraction (Point)";
                    break;*/
                case SystemState.ATTRACTOR_TIME:
                        text_ = "Time";
                        break;
                //case SystemState.ATTRACTOR_FRAME:
                //        text_ = "Stroke";
                //        break;
                case SystemState.ATTRACTOR_GEOGRAPH:
                    text_ = "Geograph";
                    break;
                case SystemState.FILE_OPEN:
                    text_ = "Load Photos";
                    break;
                case SystemState.MOVE:
                    text_ = "Drag Photo";
                    break;
                case SystemState.QUERY:
                    text_ = "Draw Circle";
                    break;
                case SystemState.TRANSFORM:
                    text_ = "Move Circle";
                    break;
                case SystemState.DELETE:
                    text_ = "Delete Photo";
                    break;
                default:
                    text_ = "Unknown";
                    break;
            }
            //position_ = pos;
            type_ = FukiType.ToolTip;
        }

        //public ContainmentType BoundingContains(Vector2 pos)
        //{
        //    if (scale_.X > 0 && scale_.Y > 0)
        //    {
        //        return boundingBox_.Contains(pos);
        //    }
        //    else
        //    {
        //        return ContainmentType.Disjoint;
        //    }
        //}

        public void MoveAtH(float x)
        {
            position_.X = x;
            Vector2 scaledCenter = new Vector2(scale_.X * (float)Width / 2f, scale_.Y * (float)Height / 2f);
            boundingBox_ = new BoundingBox2D(position_ - scaledCenter, position_ + scaledCenter, 0f);
        }
        public void MoveAtV(float y)
        {
            position_.Y = y;
            Vector2 scaledCenter = new Vector2(scaleTarget_.X * (float)Width / 2f, scaleTarget_.Y * (float)Height / 2f);
            boundingBox_ = new BoundingBox2D(position_ - scaledCenter, position_ + scaledCenter, 0f);
        }

        public void ChangeDT(DateTime dt)
        {
            text_ = dt.ToShortDateString();
        }

        public void Render(SpriteBatch batch, SpriteFont font, Texture2D tex)
        {
            // フォントの大きさを基準に吹き出しの大きさを変える
            int a = this.Left;
            int b = this.Right;
            Vector2 fontMeasure = font.MeasureString(text_);
            //if (isDel_)
            //{
            //    scaleTarget_ = -Vector2.One;
            //}
            //else
            {
                scaleTarget_.X = fontMeasure.X / (float)InWidth;
                scaleTarget_.Y = fontMeasure.Y / (float)InHeight;
                if (type_ == FukiType.ToolTip)
                {
                    scaleTarget_ *= 0.7f;
                }
            }

            // 出てくるときに揺らす
            //if (type_ != FukiType.DateTime)
            {
                float tempWeight = 1f / 5f;
                vScale_.X += (scaleTarget_.X - scale_.X) * tempWeight;
                vScale_.Y += (scaleTarget_.Y - scale_.Y) * tempWeight;
                tempWeight = 0.5f;
                vScale_ *= tempWeight;
                scale_ += vScale_;
            }
            
            // boundingBox を更新
            Vector2 center = new Vector2((float)Width / 2f, (float)Height / 2f);
            boundingBox_ = new BoundingBox2D(position_ - center * scale_, position_ + center * scale_, 0f);

            if (scale_.X >= 0 && scale_.Y >= 0)
            {
                float depth = Math.Min(1f, 0.5f * (float)Math.Exp(scale_.X - scaleTarget_.X));
                float ep = 0.0000001f;
                depth = Math.Max(ep, depth);
                batch.Draw(tex, position_, null, Color.White, 0f, center, scale_, SpriteEffects.None, depth);
                center = fontMeasure * 0.5f;
                Vector2 stringScale = new Vector2(scale_.X * InWidth / fontMeasure.X, scale_.Y * InHeight / fontMeasure.Y);
                batch.DrawString(font, text_, position_, Color.Black, 0f, center, stringScale, SpriteEffects.None, depth - ep);
            }
        }
    }
}
