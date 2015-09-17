using System;
using System.Collections.Generic;
using dflip.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace dflip
{
    public class PieMenu
    {
        public enum PieMode
        {
            DrawLine,
            DragPhoto,
            DeletePhoto,
            MoveLine,
            Nothing,
            Geotag,
            TimeScroll,
        }

        private PieMode mode_ = PieMode.DragPhoto;
        private bool isShown_ = false;
        private int radius_ = 256;
        private int radiusIn_ = 20;
        private Vector2 position_ = Vector2.Zero;

        #region property
        public PieMode Mode
        {
            get
            {
                return mode_;
            }
            set
            {
                mode_ = value;
            }
        }
        public bool IsShown
        {
            get
            {
                return isShown_;
            }
        }
        #endregion

        public PieMenu()
        {
            radius_ = (int)(Math.Max(ResourceManager.pieTexDef_.Width,ResourceManager.pieTexDef_.Height ) / 2);
        }

        public void Show(Vector2 pos)
        {
            position_ = pos;
            isShown_ = true;
        }

        public void Hide()
        {
            isShown_ = false;
        }

        public ContainmentType BoundingContains(Vector2 pos)
        {
            if (isShown_)
            {
                Vector2 dist = pos - position_;
                if (dist.Length() > radiusIn_ && dist.Length() < radius_)
                {
                    return ContainmentType.Contains;
                }
                else
                {
                    return ContainmentType.Disjoint;
                }
            }
            else
            {
                return ContainmentType.Disjoint;
            }
        }

        public PieMode ChangeMode(Vector2 pos)
        {
            PieMode m = PieMode.Nothing;
            Vector2 dist = pos - position_;
            if (dist.Length() > radiusIn_ && dist.Length() < radius_)
            {
                int mode = ((int)((Math.Atan2((double)dist.Y, (double)dist.X) + Math.PI) * 3d / Math.PI)) % 6;
                switch (mode)
                {
                    case 0:
                        m = PieMode.DrawLine;
                        break;
                    case 1:
                        m = PieMode.DragPhoto;
                        break;
                    case 2:
                        m = PieMode.DeletePhoto;
                        break;
                    case 3:
                        m = PieMode.TimeScroll;
                        break;
                    case 4:
                        m = PieMode.MoveLine;
                        break;
                    case 5:
                        m = PieMode.Geotag;
                        break;
                    default:
                        m = PieMode.Nothing;
                        break;
                }
            }
            return m;
        }

        public void Render(Vector2 pos)
        {
            
            if (isShown_)
            {
                SpriteBatch batch = SystemParameter.batch_;
                Texture2D def = ResourceManager.pieTexDef_;
                List<Texture2D> texs = ResourceManager.pieTexs_;
                Vector2 center = new Vector2(def.Width, def.Height) * 0.5f;
                Vector2 dist = pos - position_;

                //batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (dist.Length() > radiusIn_ && dist.Length() < radius_)
                {
                    int mode = ((int)((Math.Atan2((double)dist.Y, (double)dist.X) + Math.PI) * 3d / Math.PI)) % 6;
                    batch.Draw(texs[mode], position_ - center, Color.White);
                }
                else
                {
                    batch.Draw(def, position_ - center, Color.White);
                }
                //batch.End();
            }
        }
    }
}
