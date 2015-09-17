//using System;
//using System.Linq;
using System.Collections.Generic;
//using Microsoft.Xna.Framework;
using PhotoViewer.Supplement;
using PhotoViewer.Element;
using PhotoViewer;
using PhotoViewer.Manager;
using PhotoInfo;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorAvoidScale : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;

        // added by Gengdai
        private float realMinScale = 0.0f;
        private float realMaxScale = 0.0f;
        private float aPhotoArea = 0.0f;
        private float bPhotoArea = 0.0f;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {

            float MinPhotoSize = Browser.MinPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count);
            //MinPhotoSize = 0f;// movie用
            float MaxPhotoSize = Browser.MaxPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count);

            weight_ = weight.ScaleWeight;

            // 吸引子选择
            foreach (Photo a in photos)
            {
                // 大规模速度
                float ds = 0;

                // added by Gengdai
                realMinScale = a.GetTexture().Width > a.GetTexture().Height ? MinPhotoSize * Browser.MAXX / a.GetTexture().Width : MinPhotoSize * Browser.MAXY / a.GetTexture().Height;
                realMaxScale = a.GetTexture().Width > a.GetTexture().Height ? MaxPhotoSize * Browser.MAXX / a.GetTexture().Width : MaxPhotoSize * Browser.MAXY / a.GetTexture().Height;
                aPhotoArea = a.Scale * a.GetTexture().Width * a.Scale * a.GetTexture().Height;

                // 避免重叠的约束
                if (a.Adjacency.Count > 0)
                {
                    foreach (AdjacentPhoto b in a)
                    {
                        bPhotoArea = b.Photo.Scale * b.Photo.GetTexture().Width * b.Photo.Scale * b.Photo.GetTexture().Height;
                        // 如果对方小
                        // 为防止重叠，MinPhotoSize会缩小
                        if (bPhotoArea < aPhotoArea)
                        {
                            if (a.IsGazeds && b.Photo.IsGazeds)
                            {
                                ds -= (a.Scale - realMinScale) * 0.1f * weight_;
                            }
                            else
                            {
                                ds -= (a.Scale - realMinScale) * 0.01f * weight_;
                            }
                        }
                    }
                }

                // 防止MinPhotoSize大于MaxPhotoSize
                if (a.Scale < realMinScale)
                {
                    ds += (realMinScale - a.Scale) * 0.02f * weight_;
                }
                else if (a.Scale > realMaxScale)
                {
                    ds -= (a.Scale - realMaxScale) * 0.02f * weight_;
                }

                // 噪声添加
                if (false)
                {
                    float variance = weight.NoiseWeight * 0.2f;
                    float noise = (float)randbm.NextDouble(variance);
                    ds += noise;
                }

                a.AddScale(ds);
            }
        }
    }
}
