using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoViewer.Attractor.Manager;

namespace PhotoViewer.Element.Dock
{
    public class Dock
    {
        private List<Icon> icons_ = new List<Icon>();
        private int iconNumber_ = 0;
        private static readonly int showThreshold_ = 1;
        private static readonly int hideThreshold_ = 100;
        private int dockBoundX_ = 0;
        private Vector2 hidingPoint_ = Vector2.Zero;
        private float iconScale_ = 1.0f;
        private float blankScale_ = 1.0f;
        private DockState state_ = DockState.show;

        public enum DockState
        {
            show,
            opening,
            hide,
            closing,
        }

        public int displayTime
        {
            get;
            set;
        }

        public Dock(int iconNumber)
        {
            iconNumber_ = iconNumber;
            hidingPoint_ = new Vector2(-58f, 42f);
            state_ = DockState.hide;
            displayTime = 0;
            //if (state_ == DockState.show)
            //{
            //    dockBoundX_ = hideThreshold_;
            //    for (int i = 0; i < iconNumber - 1; i++)
            //    {
            //        icons_.Add(new Icon(i, new Vector2(42f, (float)(84 * i + 42)), 1 << i));
            //    }
            //    icons_.Add(new Icon(iconNumber - 1, new Vector2(42f, (float)(84 * iconNumber - 42)), 0));
            //}
            //else
            //{
                dockBoundX_ = 0;
                for (int i = 0; i < iconNumber; i++)
                {
                    icons_.Add(new Icon(i, hidingPoint_, 1 << i));
                }
            //}
        }

        //public void Unload()
        //{
        //    foreach (Icon icon in icons_)
        //    {
        //        icon.Unload();
        //    }
        //}

        public void Render(SpriteBatch batch, int height, Texture2D shadowTexture1, Texture2D shadowTexture2)
        {
            //hidingPoint_.Y = (float)height * 0.5f;
            //if (state_ == DockState.hide)
            //{
            //    foreach (Icon icon in icons_)
            //    {
            //        icon.MoveAt(hidingPoint_);
            //    }
            //}
            //else
            {
                iconScale_ = (float)height / (float)(74 * iconNumber_ + 10);
                if (iconScale_ > 1.0f)
                {
                    iconScale_ = 1.0f;
                    blankScale_ = (float)(height - 64 * iconNumber_) / (float)(10 * iconNumber_ + 10);
                }
                else if (iconScale_ < 1.0f)
                {
                    blankScale_ = iconScale_;
                }

                //if (state_ == DockState.show)
                //{

                //    //foreach (Icon icon in icons_)
                //    //{
                //    //    icon.ScaleAt(iconScale_);
                //    //    icon.MoveAt(new Vector2((float)(10 + 32 * iconScale_), (float)(10 * blankScale_ * (1 + icon.IconID) + 64 * iconScale_ * (icon.IconID + 0.5f))));

                //    //}
                //}

                //float tempWeight = 0f;
                if (state_ == DockState.closing)
                {
                    foreach (Icon icon in icons_)
                    {
                        float tempWeight = 1f / 8f;
                        icon.Vx += (hidingPoint_.X - icon.Position.X) * tempWeight;
                        icon.Vy += (hidingPoint_.Y - icon.Position.Y) * tempWeight;
                        tempWeight = 0.5f;
                        icon.Vx *= tempWeight;
                        icon.Vy *= tempWeight;
                        icon.Move();
                        if (icon.Position.X + 32 * iconScale_ + 26 < dockBoundX_)
                        {
                            dockBoundX_ = (int)Math.Max(0, icon.Position.X + 32 * iconScale_ + 26);
                        }
                    }
                    if ((icons_[icons_.Count - 1].Position.Y - hidingPoint_.Y) < 0.5f)
                    {
                        state_ = DockState.hide;
                    }
                }
                else if (state_ == DockState.opening)
                {
                    foreach (Icon icon in icons_)
                    {
                        float tempWeight = 1f / 256f;
                        icon.Vx += 10 + 32 * iconScale_ - icon.Position.X;
                        icon.Vy += (float)(10 * blankScale_ * (1 + icon.IconID) + 64 * iconScale_ * (icon.IconID + 0.5f) - icon.Position.Y);
                        tempWeight = 0.2f;
                        icon.Vx *= tempWeight;
                        icon.Vy *= tempWeight;
                        icon.Move();
                        if (icon.Position.X + 32 * iconScale_ + 26 > dockBoundX_)
                        {
                            dockBoundX_ = (int)Math.Min(hideThreshold_, icon.Position.X + 32 * iconScale_ + 26);
                        }
                    }
                    if ((10 * blankScale_ * (1 + icons_[iconNumber_ - 1].IconID) + 64 * iconScale_ * (icons_[iconNumber_ - 1].IconID + 0.5f)) - icons_[iconNumber_ - 1].Position.Y < 0.5f)
                    {
                        state_ = DockState.show;
                    }
                }
            }

            //batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (Icon icon in icons_)
            {
                icon.RenderShadow(batch, shadowTexture1);
                if (icon.Attractor != SystemState.FILE_OPEN)
                {
                    icon.RenderLight(batch, shadowTexture2);
                }
                icon.Render(batch, shadowTexture1);
            }
            //batch.End();
        }

        #region 属性封装
        public DockState State
        {
            get
            {
                return state_;
            }
            set
            {
                state_ = value;
            }
        }
        public int ShowThreshold
        {
            get
            {
                return showThreshold_;
            }
        }
        public int HideThreshold
        {
            get
            {
                return hideThreshold_;
            }
        }
        public int DockBoundX
        {
            get
            {
                return dockBoundX_;
            }
        }
        public List<Icon> Icons
        {
            get
            {
                return icons_;
            }
        }
        #endregion
    }
}
