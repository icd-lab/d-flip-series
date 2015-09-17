using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PhotoInfo;
using Attractor;
using System.Diagnostics;
using dflip.Element;

namespace dflip.Manager
{
    class PhotoDisplay
    {
        private List<Photo> activePhotos = new List<Photo>();
        List<Photo> photos = new List<Photo>();
        SystemState systemState;
        Dock dock;
        AttractorWeight weight;
        ScrollBar sBar;

        public int AttractToMouseWeight
        {
            get
            {
                //return trackBarAttractToMouse.Value;
                return 5;
            }
        }

        public bool EnabledNoise
        {
            get
            {
                //return noiseToolStripMenuItem.Checked;
                return true;
            }
        }

        public int ScaleUpMouseWeight
        {
            get
            {
                //return trackBarScaleUpMouse.Value;
                return 100;
            }
        }
        public int TagWeight
        {
            get
            {
                //return trackBarTag.Value;
                return 100;
            }
        }
        public int TagFlagWeight
        {
            get
            {
                // とりあえず仮に100で．
                return 100;
            }
        }
        public int NoiseWeight
        {
            get
            {
                //return trackBarNoise.Value;
                return 0;
            }
        }
        public int NonOverlapWeight
        {
            get
            {
                //return trackBarNonOverlap.Value;
                return 100;
            }
        }
        public int ScaleWeight
        {
            get
            {
                //return trackBarScale.Value;
                return 100;
            }
        }
        StrokeBoxCollection strokes;
        public PhotoDisplay(SystemState st, Dock d, ScrollBar s, StrokeBoxCollection str)
        {
            systemState = st;
            dock = d;
            sBar = s;
            strokes = str;
            //photos = p;
            weight = new AttractorWeight(
                NonOverlapWeight,
                ScaleWeight,
                AttractToMouseWeight,
                ScaleUpMouseWeight,
                TagWeight,
                NoiseWeight);
        }

        public bool IsFiltering
        {
            get
            {
                //return filteringToolStripMenuItem.Checked;
                return true;
            }
            /*set
            {
                filteringToolStripMenuItem.Checked = value;
            }*/
        }
        bool IsShadowing
        {
            get
            {
                //return shadowingToolStripMenuItem.Checked;
                return true;
            }
            /*set
            {
                shadowingToolStripMenuItem.Checked = value;
            }*/
        }
        bool IsWhiteframe
        {
            get
            {
                //return whiteFrameToolStripMenuItem.Checked;
                return true;
            }
            /*set
            {
                whiteFrameToolStripMenuItem.Checked = value;
            }*/
        }
        //AttractorWeight weight;


        public void PhotosToShow(List<Photo> p)
        {
            photos = p;
        }

        public void photoInitialize()
        {
            // 
            foreach (Photo photo in photos)
            {
                photo.Begin();
            }
            // adjacent photos
            Vector2 dir = Vector2.Zero;
            float dira = 0f;
            for (int i = 0, count = photos.Count; i < count - 1; ++i)
            {
                for (int j = i + 1; j < count; ++j)
                {
                    if (photos[i].BoundingBox.Overrap(photos[j].BoundingBox, ref dir, ref dira))
                    {
                        photos[i].AddAdjacentPhoto(photos[j], dir, dira);
                        photos[j].AddAdjacentPhoto(photos[i], -dir, -dira);
                        
                        //if (photos[i].LayerDepth - photos[j].LayerDepth < 1e-9 && photos[i].LayerDepth - photos[j].LayerDepth > -1e-9)
                        //{
                        //    if (photos[j].LayerDepth + 0.001f > 1f)
                        //        photos[j].LayerDepth = 1f;
                        //    else photos[j].LayerDepth += 0.001f;
                        //}
                    }
                }
            }
        }

        public void photoBehavior()
        {
            activePhotos.Clear();
            foreach (Photo p in photos)
                if (p.IsGazeds)
                    activePhotos.Add(p);

            #region control photos' position and scale
            if (photos.Count > 0) // 如果图像已加载
            {
                systemState.invokeAttractorSelection(dock, sBar, weight, photos, activePhotos, strokes.strokeList, systemState);
            }
            #endregion

            for (int i = 0; i < photos.Count; i++)
            {
                Vector2 v = Vector2.Zero;

                #region deleted & selected photo
                if (photos[i].IsDel)
                {
                    photos.Remove(photos[i]);
                    i--;
                    continue;
                }

                //selected photo's position stable
                if (photos[i].IsGazeds)
                {
                    photos[i].AddPosition(-photos[i].Velocity);

                    // 画面外に出ないように制約(強い制約)
#if NO_ROTATION
                    if (photos[i].BoundingBox.Min.X < dock.DockBoundX)
                    {
                        v.X -= (photos[i].BoundingBox.Min.X - dock.DockBoundX) * 20f;
                    }
                    if (photos[i].BoundingBox.Max.X > SystemParameter.ClientWidth)
                    {
                        v.X -= (photos[i].BoundingBox.Max.X - SystemParameter.ClientWidth) * 20f;
                    }
                    if (photos[i].BoundingBox.Min.Y < 0)
                    {
                        v.Y -= (photos[i].BoundingBox.Min.Y) * 20f;
                    }
                    if (photos[i].BoundingBox.Max.Y > SystemParameter.ClientHeight)
                    {
                        v.Y -= (photos[i].BoundingBox.Max.Y - SystemParameter.ClientHeight) * 20f;
                    }
#else
                        for (int i = 0; i < 4; ++i)
                        {
                            Vector2 v1 = photo.Position - photo.BoudingBox.Vertex[i];
                            v1.Normalize();
                            Vector2 v2 = Vector2.Zero;
                            float dist = 0;
                            if (photo.BoudingBox.Vertex[i].X < dock_.DockBoundX)
                            {
                                v2 = Vector2.UnitX;
                                dist = dock_.DockBoundX - photo.BoudingBox.Vertex[i].X;
                            }
                            if (photo.BoudingBox.Vertex[i].X > input_.WindowWidth)
                            {
                                v2 = -Vector2.UnitX;
                                dist = photo.BoudingBox.Vertex[i].X - input_.WindowWidth;
                            }
                            if (photo.BoudingBox.Vertex[i].Y < 0)
                            {
                                v2 = Vector2.UnitY;
                                dist = -photo.BoudingBox.Vertex[i].Y;
                            }
                            if (photo.BoudingBox.Vertex[i].Y > input_.WindowHeight)
                            {
                                v2 = -Vector2.UnitY;
                                dist = photo.BoudingBox.Vertex[i].Y - input_.WindowHeight;
                            }
                            //v += v1 * (float)(dist * Math.Abs(v1.X * v2.X + v1.Y * v2.Y)) * 20f;
                            v += dist * v2 * 20f;
                            va += -(v1.X * v2.Y - v1.Y * v2.X) * 10f;
                        }
#endif
                }
                #endregion

                photos[i].AddPosition(v);
                photos[i].End();
                photos[i].SetDisplayTarget();
            }
            
        }

        public void drawPhoto()
        {
            foreach (Photo photo in photos)
            {
                bool isDragged = false;
                bool isFavorite = false;
                
                photo.Render(SystemParameter.batch_, IsFiltering, IsShadowing, ResourceManager.shadowSquare_, IsWhiteframe, isDragged, ResourceManager.frameSquare_, isFavorite);
            }
            //batch_.End();
        }
    }
}
