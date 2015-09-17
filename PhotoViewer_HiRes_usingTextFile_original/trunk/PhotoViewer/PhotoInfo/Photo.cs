using System;
using System.Collections;
using System.Collections.Generic;
using PhotoViewer;
using PhotoViewer.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoViewer.Supplement;

namespace PhotoInfo
{
    public class Photo : IEnumerable
    {
        public class colorFeature
        {
            /// <summary>
            /// 画像特徴量（5x5の25分割した画像 + 分散）
            /// </summary>
            public Vector3[] feature_ = new Vector3[25];
            public double variance_ = 0d;
            public colorFeature()
            { }
            public colorFeature(Vector3[] f, double v)
            {
                feature_ = f;
                variance_ = v;
            }
        }

        private static readonly int GAZE_TIME = 90;
        private static readonly int NO_GAZE_TIME = 10;//30
        private colorFeature color = null;
        /// <summary>
        /// ファイルパス
        /// </summary>
        private string filename_ = string.Empty;
        
        /// <summary>
        /// 画像ID
        /// </summary>
        private int ID_ = -1;

        /// <summary>
        /// 画像タグ
        /// </summary>
        //private List<string> tag_ = new List<string>();
        //private DateTime capturedDate_ = DateTime.MinValue;
        //private DateTime createdDate_ = DateTime.MinValue;
        //private List<string> comment_ = new List<string>();
        //private PeopleTags pTags = new PeopleTags();
        public PhotoTag ptag;// = new PhotoLog();

        public bool containTag(String tag)
        {
            foreach (string s in ptag.allTags)
                if (string.Equals(tag, s, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        public bool tagClickedCheck(Vector2 clickedPosition)
        {
           //return ptag.clickedCheck(clickedPosition);
            return true;
        }

        public List<String> activeTag
        {
            get { return ptag.activeTagList; }
        }

        public PeopleTags PTags
        {
            get;
            set;
        }

        

        /// <summary>
        /// 画像のテクスチャ
        /// </summary>
        private Texture2D texture_ = null;

        /// <summary>
        /// 位置
        /// </summary>
        private Vector2 position_ = Vector2.Zero;
        private Vector2 positionDisplay_ = Vector2.Zero;
        private Vector2 positionTarget_ = Vector2.Zero;

        /// <summary>
        /// 向き
        /// </summary>
        private float angle_ = 0;
        private float angleDisplay_ = 0;
        private float angleTarget_ = 0;

        /// <summary>
        /// 回転中心
        /// </summary>
        private Vector2 center_ = Vector2.Zero;

        /// <summary>
        /// スケール
        /// </summary>
        private float scale_ = 1;
        private float scaleDisplay_ = 1;
        private float scaleTarget_ = 1;

        List<string> tags;

        /// <summary>
        /// 速度
        /// </summary>
        private Vector2 velocity_ = Vector2.Zero;
        private Vector2 velocityDisplay_ = Vector2.Zero;

        /// <summary>
        /// 角速度(omega)
        /// </summary>
        private float vangle_ = 0;
        private float vangleDisplay_ = 0;

        /// <summary>
        /// スケール速度
        /// </summary>
        private float vscale_ = 0;
        private float vscaleDisplay_ = 0;

        /// <summary>
        /// 画像レイヤの深さ
        /// </summary>
        private float layerDepth_ = 0.9999f;

        public float LayerDepth
        {
            get
            {
                return layerDepth_;
            }
            set
            {
                layerDepth_ = value;
            }
        }
        /// <summary>
        /// マウスまでの距離
        /// </summary>
        //private Vector2 distanceToMouse;// = new List<Vector2>();

        /// <summary>
        /// マウスと重なっているか？
        /// </summary>
        //private List<bool> isOverlapMouses_ = new List<bool>();
        // 前回マウスと重なっていたか？
        //private List<bool> isOverlapMousesOld_ = new List<bool>();
        // マウスと重なっている時間[frame]
        private int overlapMouseTime;

        public bool IsFollowing
        {
            get;
            set;
        }

        public bool IsClicked
        {
            get;
            set;
        }
        public bool underMouse
        {
            get;
            set;
        }
        private Vector2 clickedPoint = Vector2.Zero;
        public bool IsGazeds
        {
            get
            {
                if (overlapMouseTime > GAZE_TIME)
                    return true;
                else return false;
            }
        }
        public void KeepGazed()
        {
            overlapMouseTime = GAZE_TIME + 1;
            layerDepth_ = 0f;
        }

        // 消去フラグ
        private bool isDel_ = false;

        /// <summary>
        /// バウンディングボックス
        /// </summary>
        public BoundingBox2D boundingBox_ = new BoundingBox2D();
        public BoundingBox2D boundingBoxDisplay_ = new BoundingBox2D();

        /// <summary>
        /// 近傍の重なっている写真の集合とそれらとのめり込み距離
        /// </summary>
        private List<AdjacentPhoto> adjacency_ = new List<AdjacentPhoto>();
        private List<AdjacentPhoto> adjacencyDisplay_ = new List<AdjacentPhoto>();

        

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="filename"></param>
        public Photo(int photoID, string filename, Vector2 position, float scale, float angle, PhotoTag p, Texture2D t, colorFeature f)
        {
            
            ID_ = photoID;
            filename_ = filename;
            position_ = position;
            positionDisplay_ = position;
            positionTarget_ = position;
            scale_ = scale;
            scaleDisplay_ = 0f;
            scaleTarget_ = scale;
            angle_ = angle;
            angleDisplay_ = angle;
            angleTarget_ = angle;
            ptag = p;
            texture_ = t;
            color = f;
            //this.LoadTag();
        }

        //0: not touched; 1: begin count
        public int touchCount
        {
            get;
            set;
        }

        List<int> maxTagSize = new List<int>();

        
        public void ShowMetadata()
        {
            //maxTagSize = ptag.drawTags((int)(this.PositionDisplay.X + this.WidthDisplay / 2 + Browser.MAR), (int)(this.PositionDisplay.Y - this.HeightDisplay / 2), angleDisplay_);
            
        }

        public void Unload()
        {
            if (texture_ != null)
            {
                texture_.Dispose();
                texture_ = null;
            }
        }

        public static int FeatureSplit = 5;
        // add by Liu Gengdai
        public Texture2D GetTexture()
        {
            return texture_;
        }

        private int cod(int x, int y, int width)
        {
            return 4 * (x + y * width);
        }

        public double PhotoDist(Photo p)
        {
            double distMax = (double)(Photo.FeatureSplit * Photo.FeatureSplit) * 2d;// Math.Sqrt(1.25d);
            double dist = 0.0;
            for (int j = 0; j < Photo.FeatureSplit * Photo.FeatureSplit; ++j)
            {
                dist += ResourceManager.HsvDist(this.color.feature_[j], p.color.feature_[j]);
            }
                                    // 距离正规化
            dist /= distMax;
            dist += (this.color.variance_ - p.color.variance_) * (this.color.variance_ - p.color.variance_);
            dist /= 2d;
            return dist;
        }

        
        public static colorFeature CalcFeature(Texture2D texture_)
        {
            Photo.colorFeature color = new colorFeature();
            int w = texture_.Width;
            int h = texture_.Height;
            int dlen = w * h * 4;
            byte[] d = new byte[dlen];
            texture_.GetData(d);
            List<Vector3> f = new List<Vector3>();
            Vector3 rgbave = Vector3.Zero;
            Vector3 hsvave = Vector3.Zero;
            for (int i = 0; i < FeatureSplit; ++i)
            {
                for (int j = 0; j < FeatureSplit; ++j)
                {
                    int count = 0;
                    Vector3 rgb = Vector3.Zero;
                    Vector3 hsv = Vector3.Zero;
                    for (int ii = h * i / FeatureSplit; ii < h * (i + 1) / FeatureSplit; ++ii)
                    {
                        for (int jj = w * j / FeatureSplit; jj < w * (j + 1) / FeatureSplit; ++jj)
                        {
                            int index = 4 * (jj + ii * w);
                            rgb.X += d[index + 2];
                            rgb.Y += d[index + 1];
                            rgb.Z += d[index];
                            ++count;
                        }
                    }
                    rgb /= (float)(count * 255);
                    rgbave += rgb;
                    ResourceManager.rgb2hsv(ref rgb, out hsv);
                    f.Add(hsv);
                }
            }
            color.feature_ = f.ToArray();

            rgbave /= (float)(FeatureSplit * FeatureSplit);
            ResourceManager.rgb2hsv(ref rgbave, out hsvave);
            color.variance_ = 0d;
            foreach (Vector3 ff in f)
            {
                color.variance_ += ResourceManager.HsvDist(ff, hsvave);
            }
            color.variance_ /= (double)(FeatureSplit * FeatureSplit);

            return color;
        }

        #region property
        public int ID
        {
            get
            {
                return ID_;
            }
        }
        public string FileName
        {
            get
            {
                return filename_;
            }
        }
        public bool IsLoadedTexture
        {
            get
            {
                return (texture_ != null);
            }
        }
        public bool IsDel
        {
            get
            {
                return isDel_;
            }
            set
            {
                isDel_ = value;
            }
        }
        public float Width
        {
            get
            {
                return (texture_ != null) ? texture_.Width : 0;
            }
        }
        public float Height
        {
            get
            {
                return (texture_ != null) ? texture_.Height : 0;
            }
        }
        public float WidthDisplay
        {
            get
            {
                return (texture_ != null) ? texture_.Width * scaleDisplay_ : 0;
            }
        }
        public float HeightDisplay
        {
            get
            {
                return (texture_ != null) ? texture_.Height * scaleDisplay_ : 0;
            }
        }
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
        //display position: center of photos
        public Vector2 PositionDisplay
        {
            get
            {
                return positionDisplay_;
            }
            set
            {
                positionDisplay_ = value;
            }
        }
        public float Scale
        {
            get
            {
                return scale_;
            }
            set
            {
                scale_ = value;
            }
        }
        public float ScaleDisplay
        {
            get
            {
                return scaleDisplay_;
            }
        }
        public float Angle
        {
            get
            {
                return angle_;
            }
            set
            {
                angle_ = value;
            }
        }
        public float AngleDisplay
        {
            get
            {
                return angleDisplay_;
            }
            set
            {
                angleDisplay_ = value;
            }
        }
        public Vector2 ClickedPoint
        {
            get { return clickedPoint; }
            set { clickedPoint = value; }
        }

        
#endregion
        #region property
        /*public List<string> Tag
        {
            get
            {
                return tag_;
            }
            set
            {
                tag_ = value;
            }
        }*/
        //public DateTime CreatedDate
        //{
        //    get
        //    {
        //        return createdDate_;
        //    }
        //    set
        //    {
        //        createdDate_ = value;
        //    }
        //}
        //public DateTime CapturedDate
        //{
        //    get
        //    {
        //        return capturedDate_;
        //    }
        //    set
        //    {
        //        capturedDate_ = value;
        //    }
        //}
        /*public List<string> Comments
        {
            get
            {
                return comment_;
            }
            set
            {
                comment_ = value;
            }
        }*/
        //public Vector3[] Feature
        //{
        //    get
        //    {
        //        return feature_;
        //    }
        //    set
        //    {
        //        feature_ = value;
        //    }
        //}
        //public double Variance
        //{
        //    get
        //    {
        //        return variance_;
        //    }
        //    set
        //    {
        //        variance_ = value;
        //    }
        //}
        #endregion
        #region ｷ籏ｰﾊﾔ(ﾍｼﾏﾋｶｯﾏ犹ﾘ｣ｺﾋﾙｶﾈ ｽﾇｶﾈ ﾋﾅ)
        public Vector2 Velocity
        {
            get
            {
                return velocity_;
            }
            set
            {
                velocity_ = value;
            }
        }

        public float Vangle
        {
            get
            {
                return vangle_;
            }
            set
            {
                vangle_ = value;
            }
        }

        public float Vscale
        {
            get
            {
                return vscale_;
            }
            set
            {
                vscale_ = value;
            }
        }
        #endregion
        #region ﾐﾞｸﾄﾍｼﾆｬﾔﾋｶｯ
        public void AddPosition(Vector2 dx)
        {
            velocity_ += dx;
        }
        public void AddAngle(float dx)
        {
            vangle_ += dx;
        }
        public void AddScale(float dx)
        {
            vscale_ += dx;
        }
        #endregion
        #region ｷ籏ｰﾊﾔ
        //public Vector2 DistanceToMouse
        //{
        //    get;
        //    set;
        //}

        public Vector2 StartDragPosition
        {
            get;
            set;
        }

        
        public Vector2 Center
        {
            get
            {
                return center_;
            }
            set
            {
                center_ = value;
            }
        }
        public BoundingBox2D BoundingBox
        {
            get
            {
                return boundingBox_;
            }
            set
            {
                boundingBox_ = value;
            }
        }
        public BoundingBox2D BoundingBoxDisplay
        {
            get
            {
                return boundingBoxDisplay_;
            }
        }
        public void AddAdjacentPhoto(Photo photo, Vector2 dir, float dira)
        {
            adjacency_.Add(AdjacentPhoto.newInstance(photo, dir, dira));
        }
        public void AddAdjacentPhotoDisplay(Photo photo, Vector2 dir, float dira)
        {
            adjacencyDisplay_.Add(AdjacentPhoto.newInstance(photo, dir, dira));
        }
        public IEnumerator GetEnumerator()
        {
            return adjacency_.GetEnumerator();
        }
        public IEnumerator GetEnumeratorDisplay()
        {
            return adjacencyDisplay_.GetEnumerator();
        }
        internal System.Collections.ObjectModel.ReadOnlyCollection<AdjacentPhoto> Adjacency
        {
            get
            {
                return adjacency_.AsReadOnly();
            }
        }
        internal System.Collections.ObjectModel.ReadOnlyCollection<AdjacentPhoto> AdjacencyDisplay
        {
            get
            {
                return adjacencyDisplay_.AsReadOnly();
            }
        }
        public void ClearAdjacentPhoto()
        {
            foreach (AdjacentPhoto ap in adjacency_)
            {
                ap.Dispose();
            }
            adjacency_.Clear();
        }
        public void ClearAdjacentPhotoDisplay()
        {
            foreach (AdjacentPhoto ap in adjacencyDisplay_)
            {
                ap.Dispose();
            }
            adjacencyDisplay_.Clear();
        }
        #endregion

        /// <summary>
        /// アトラクター選択前の処理
        /// </summary>
        public void Begin()
        {
            velocity_ = Vector2.Zero;
            vangle_ = 0;
            vscale_ = 0;

            ClearAdjacentPhoto();
            ClearAdjacentPhotoDisplay();
            IsFollowing = false;
        }

        public void prepare()
        {
            // マウスの状態
            underMouse = false;
            IsClicked = false;
        }

        /// <summary>
        /// アトラクター選択後の処理
        /// </summary>
        public void End()
        {
            if (isDel_)
            {
                scale_ -= 0.5f;
            }
            else
            {
                position_ += velocity_ * (1.0f / 60.0f);
                scale_ += vscale_ * (1.0f / 60.0f);
#if NO_ROTATION
#else
                angle_ += (float)(Math.Sign(vangle_) * Math.PI) * (1.0f / 60.0f);
                while (angle_ > Math.PI)
                {
                    angle_ -= (float)Math.PI * 2f;
                }
                while (angle_ < -Math.PI)
                {
                    angle_ += (float)Math.PI * 2f;
                }
#endif
            }

            //bounding box
            if (IsGazeds && ptag.allTags != null && maxTagSize.Count == 2 && ptag.allTags.Count > 0)
            {
                //ptag.createBox(this.PositionDisplay.X + this.WidthDisplay / 2, this.PositionDisplay.Y - this.HeightDisplay / 2, angleDisplay_);
                int positionLarger = (int)Math.Max(position_.Y - center_.Y * scale_ - (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d) + maxTagSize[1], position_.Y + center_.Y * scale_ + (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d));
                int positionDisplayLarger = (int)Math.Max(positionDisplay_.Y - center_.Y * scaleDisplay_ - (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d) + maxTagSize[1], positionDisplay_.Y + center_.Y * scaleDisplay_ + (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d));
                
                boundingBox_ = new BoundingBox2D(
                  position_ - center_ * scale_ - Vector2.One * (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d),
                  new Vector2(position_.X + center_.X * scale_ + (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d) + maxTagSize[0], positionLarger), angle_);
                boundingBoxDisplay_ = new BoundingBox2D(
                    positionDisplay_ - center_ * scaleDisplay_ - Vector2.One * (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d),
                    new Vector2(positionDisplay_.X + center_.X * scaleDisplay_ - (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d) + maxTagSize[0], positionDisplayLarger), angleDisplay_);

            }
            else
            {
                boundingBox_ = new BoundingBox2D(
                  position_ - center_ * scale_ - Vector2.One * (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d),
                  position_ + center_ * scale_ + Vector2.One * (float)((1d - Math.Exp(-(double)scale_)) * (double)Browser.MAR + 2d), angle_);
                boundingBoxDisplay_ = new BoundingBox2D(
                    positionDisplay_ - center_ * scaleDisplay_ - Vector2.One * (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d),
                    positionDisplay_ + center_ * scaleDisplay_ - Vector2.One * (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR + 2d), angleDisplay_);
            }
        }

        public void postProcess()
        {
            if (touchCount > 0 && touchCount < 300)
            {
                touchCount++;
                underMouse = true;
                this.KeepGazed();
            }
            if (touchCount >= 300)
            {
                touchCount = 0;
                //ptag.clearActiveTagList();
                //overlapMouseTime = 0;
            }
            //mouse
            if (underMouse == false)
            {
                //if(IsGazeds)
                    //ptag.clearActiveTagList();
                overlapMouseTime = 0;
            }
            else if (underMouse && IsGazeds == false)
                overlapMouseTime++;
            if (IsGazeds)
            {
                layerDepth_ = 0f;
            }
        }

        // 表示用パラメータを更新
        public void SetDisplayTarget()
        {
            positionTarget_ = position_;
            scaleTarget_ = scale_;
            angleTarget_ = angle_;
        }

        // 画像を描画
        public void Render(SpriteBatch batch, bool isFiltering, bool isShadowing, Texture2D textureShadow,bool isWhiteframe, bool isDragged, Texture2D textureFrame, bool isFavorite)
        {
            if (isFiltering)
            {
                float tempWeight = 1f / 300f;
                velocityDisplay_ += (positionTarget_ - positionDisplay_) * tempWeight;
                vscaleDisplay_ += (scaleTarget_ - scaleDisplay_) * tempWeight;
                while(angleTarget_ - angleDisplay_ > Math.PI)
                {
                    angleTarget_ -= (float)(Math.PI * 2d);
                }
                while (angleTarget_ - angleDisplay_ < -Math.PI)
                {
                    angleTarget_ += (float)(Math.PI * 2d);
                }
                vangleDisplay_ += (angleTarget_ - angleDisplay_) * tempWeight;
                tempWeight = 0.9f;
                velocityDisplay_ *= tempWeight;
                vscaleDisplay_ *= tempWeight;
                vangleDisplay_ *= tempWeight;
                positionDisplay_ += velocityDisplay_;
                scaleDisplay_ += vscaleDisplay_;
                angleDisplay_ += vangleDisplay_;
            }
            else
            {
                positionDisplay_ = position_;
                scaleDisplay_ = scale_;
                angleDisplay_ = angle_;
            }

            if (scaleDisplay_ > 0)
            {
                if (texture_ != null)
                {
                    //if (IsGazeds)
                    //{
                    //    layerDepth_ = 0f;
                    //}
                    ///*else if (isOverlapMouses_.Contains(true))
                    //{
                    //    layerDepth_ = float.Epsilon;
                    //}*/
                    if(!IsGazeds)
                    {
                        layerDepth_ = Math.Max(0, Math.Min(1, 1 / (scaleDisplay_ + 1.0f)));
                    }

                    float ep = 0.0000001f;

                    if (isShadowing)
                    {
                        if (textureShadow != null)
                        {
                            float scalex = (Width * scaleDisplay_ + (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR * 2d + 2d)) * 1.5f / 100f;
                            float scaley = (Height * scaleDisplay_ + (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR * 2d + 2d)) * 1.5f / 100f;
                            Vector2 scaleShadow = new Vector2(scalex, scaley);
                            if (isFavorite)
                            {
                                batch.Draw(textureShadow, positionDisplay_, null, Microsoft.Xna.Framework.Color.Red,
                                  angleDisplay_, 50f * Vector2.One, scaleShadow, SpriteEffects.None, layerDepth_ + ep * 2f);//Math.Min(1.0f, layerDepth_ + ep * 2f)
                            }
                            else
                            {
                                batch.Draw(textureShadow, positionDisplay_, null, Microsoft.Xna.Framework.Color.Black,
                                  angleDisplay_, 50f * Vector2.One, scaleShadow, SpriteEffects.None, layerDepth_);//Math.Min(1.0f, layerDepth_ + ep * 2f)
                            }
                        }
                    }

                    if (isWhiteframe)
                    {
                        if (textureFrame != null)
                        {
                            float scalex = (Width * scaleDisplay_ + (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR * 2d + 2d)) / 2f;
                            float scaley = (Height * scaleDisplay_ + (float)((1d - Math.Exp(-(double)scaleDisplay_)) * (double)Browser.MAR * 2d + 2d)) / 2f;
                            Vector2 scaleFrame = new Vector2(scalex, scaley);
                            if (isDragged)
                            {
                                batch.Draw(textureFrame, positionDisplay_, null, Microsoft.Xna.Framework.Color.LightGray,
                                  angleDisplay_, Vector2.One, scaleFrame, SpriteEffects.None, layerDepth_);//Math.Min(1.0f, layerDepth_ + ep)
                            }
                            else if(IsFollowing)
                                batch.Draw(textureFrame, positionDisplay_, null, Microsoft.Xna.Framework.Color.White,
                                  angleDisplay_, Vector2.One, scaleFrame, SpriteEffects.None, layerDepth_);//Math.Min(1.0f, layerDepth_ + ep)
                            
                            else
                            {
                                batch.Draw(textureFrame, positionDisplay_, null, Microsoft.Xna.Framework.Color.White,
                                  angleDisplay_, Vector2.One, scaleFrame, SpriteEffects.None, layerDepth_);//Math.Min(1.0f, layerDepth_ + ep)
                            }
                        }
                    }

                    batch.Draw(texture_, positionDisplay_, null, Microsoft.Xna.Framework.Color.White,
                      angleDisplay_, center_, scaleDisplay_, SpriteEffects.None, layerDepth_);
                }
            }
            if (IsGazeds)
            {
                ShowMetadata();
            }
        }
    }
}
