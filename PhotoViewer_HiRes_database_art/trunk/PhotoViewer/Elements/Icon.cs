using PhotoViewer.Attractor.Manager;
using PhotoViewer.Element.Tips;
using PhotoViewer.Manager.Resource;
using PhotoViewer.Supplement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoViewer.Element.Dock
{
    public class Icon
    {
        private int iconID_ = -1;
        private bool isOn_ = false;
        private Vector2 position_ = Vector2.Zero;
        private Vector2 velocity_ = Vector2.Zero;
        private float scale_ = 1.0f;
        private float lightScale_ = 1.0f;
        private float lsv_ = 0.0f;
        private int attractor_ = -1;
        private BoundingBox2D boundingBox_ = new BoundingBox2D();
        private Vector2 center_ = Vector2.Zero;
        public Tip tooltip
        {
            get;
            private set;
        }

        public Icon(int iconID, Vector2 position, int attractor)
        {
            iconID_ = iconID;
            position_ = position;
            attractor_ = attractor;
            if (attractor == SystemState.ATTRACTOR_BOUND)
            {
                isOn_ = true;
            }
            tooltip = new Tip(attractor);
            tooltip.IsDel = true;
            this.Load();
        }

        public void Load()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //if (null == texture_)
            {
                center_ = new Vector2(ResourceManager.texture_[iconID_].Width, ResourceManager.texture_[iconID_].Height) * 0.5f;
                boundingBox_ = new BoundingBox2D(position_ - center_, position_ + center_, 0f);
            }
        }

        //public void Unload()
        //{
        //    if (texture_ != null)
        //    {
        //        texture_.Dispose();
        //        texture_ = null;
        //    }
        //}

        public void Move()
        {
            position_ += velocity_;
            boundingBox_ = new BoundingBox2D(position_ - center_ * scale_, position_ + center_ * scale_, 0f);
        }
        public void MoveAt(Vector2 tPos)
        {
            position_ = tPos;
            boundingBox_ = new BoundingBox2D(position_ - center_ * scale_, position_ + center_ * scale_, 0f);
        }
        public void ScaleAt(float tScale)
        {
            scale_ = tScale;
            boundingBox_ = new BoundingBox2D(position_ - center_ * scale_, position_ + center_ * scale_, 0f);
        }
        private bool tooltipOldState = true;
        public void Render(SpriteBatch batch, Texture2D texture1)
        {
            //if (texture_ != null)
            {
                batch.Draw(ResourceManager.texture_[iconID_], position_, null, Color.White, 0f, center_, scale_, SpriteEffects.None, 0f);
            }
            //Console.WriteLine(tooltip.IsDel);
            if (tooltip.IsDel == false)
            {
                if (tooltipOldState)
                {
                    tooltip.Position = position_ + Vector2.UnitY * 0.5f * (boundingBox_.Max.Y - boundingBox_.Min.Y);
                    tooltip.Reset();
                }
                if (tooltip.Left < 0)
                {
                    tooltip.MoveAtH((float)tooltip.Right / 2f);
                }
                tooltip.Render(Browser.Instance.batch_, ResourceManager.font_, ResourceManager.fukiTex_);
                
            }
            tooltipOldState = tooltip.IsDel; 
        }
        public void RenderLight(SpriteBatch batch, Texture2D textureLight)
        {
            if (IsOn && textureLight != null)
            {
                float tempWeight = 1f / 256f;
                lsv_ += scale_ - lightScale_;
                tempWeight = 0.2f;
                lsv_ *= tempWeight;
                lightScale_ += lsv_;

                batch.Draw(textureLight, position_, null, Color.White, 0f, center_, lightScale_, SpriteEffects.None, 0f);
            }
            else
            {
                lightScale_ = 0f;
            }
        }
        public void RenderShadow(SpriteBatch batch, Texture2D textureShadow)
        {
            if (textureShadow != null)
            {
                batch.Draw(textureShadow, position_, null, new Color(255,255,255, (byte)(159)), 0f, center_ * 0.75f, scale_ * 2f - lightScale_ * 0.7f, SpriteEffects.None, 0f);
            }
        }

        #region プロパティ
        public int IconID
        {
            get
            {
                return iconID_;
            }
        }
        public bool IsOn
        {
            get
            {
                return isOn_;
            }
            set
            {
                isOn_ = value;
            }
        }
        public int Attractor
        {
            get
            {
                return attractor_;
            }
            set
            {
                attractor_ = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position_;
            }
        }
        public float Vx
        {
            get
            {
                return velocity_.X;
            }
            set
            {
                velocity_.X = value;
            }
        }
        public float Vy
        {
            get
            {
                return velocity_.Y;
            }
            set
            {
                velocity_.Y = value;
            }
        }
        public BoundingBox2D BoundingBox
        {
            get
            {
                return boundingBox_;
            }
        }
        #endregion
    }
}
