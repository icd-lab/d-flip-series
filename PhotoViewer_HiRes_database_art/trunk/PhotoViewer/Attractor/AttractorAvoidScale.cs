using System.Collections.Generic;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Element.Dock;
using PhotoViewer.Element.ScrollBar;
using PhotoViewer.Element.StrokeTextbox;
using PhotoViewer.Attractor.Manager;
using PhotoViewer.Supplement;

namespace PhotoViewer.Attractor
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
                //realMinScale = a.Width > a.Height ? MinPhotoSize * Browser.MAXX / a.Width : MinPhotoSize * Browser.MAXY / a.Height;
                //realMaxScale = a.Width > a.Height ? MaxPhotoSize * Browser.MAXX / a.Width : MaxPhotoSize * Browser.MAXY / a.Height;
                realMinScale = a.Width > a.Height ? MinPhotoSize / a.Width : MinPhotoSize / a.Height;
                realMaxScale = a.Width > a.Height ? MaxPhotoSize / a.Width : MaxPhotoSize / a.Height;
                aPhotoArea = a.Scale * a.Width * a.Scale * a.Height;

                // 避免重叠的约束
                if (a.Adjacency.Count > 0)
                {
                    foreach (AdjacentPhoto b in a)
                    {
                        bPhotoArea = b.Photo.Scale * b.Photo.Width * b.Photo.Scale * b.Photo.Height;
                        // 如果对方小
                        // 为防止重叠，MinPhotoSize会缩小
                        //if (a.IsGazeds) continue;
                        if (bPhotoArea < aPhotoArea)
                        {
                            //if (b.Photo.IsGazeds)
                            //{
                            //    ds -= (a.Scale - realMinScale) * 0.1f * weight_;
                            //}
                            if (a.IsGazeds) continue;
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
