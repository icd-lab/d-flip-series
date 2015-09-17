using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Element.Dock;
using PhotoViewer.Element.ScrollBar;
using PhotoViewer.Element.StrokeTextbox;
using PhotoViewer.Attractor.Manager;
using PhotoViewer.Supplement;

namespace PhotoViewer.Attractor
{
    class AttractorScaleUpMouse : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;
        // added by Gengdai
        private float realMinScale = 0.0f;
        private float realMaxScale = 0.0f;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            float MinPhotoSize = Browser.MinPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count) * 5;
            float MaxPhotoSize = Browser.MaxPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count) * 2;

            weight_ = weight.ScaleUpMouseWeight;
            // アトラクター選択
            foreach (Photo a in activePhotos)
            {
                float ds = 0; // スケール

                // added by Gengdai
                //realMinScale = a.Width > a.Height ? MinPhotoSize * Browser.MAXX / a.Width : MinPhotoSize * Browser.MAXY / a.Height;
                //realMaxScale = a.Width > a.Height ? MaxPhotoSize * Browser.MAXX / a.Width : MaxPhotoSize * Browser.MAXY / a.Height;
                realMinScale = a.Width > a.Height ? MinPhotoSize / a.Width : MinPhotoSize / a.Height;
                realMaxScale = a.Width > a.Height ? MaxPhotoSize / a.Width : MaxPhotoSize / a.Height;

                // マウスに重なっているほど大きくしたい
                //if (a.IsGazeds)
                {
                    ds += (realMaxScale - a.Scale) * weight_ * 0.05f;
                    // サイズが最小値以下もしくは最大値以上になるのを防ぐ制約
                    if (a.Scale < realMinScale)
                    {
                        ds += (realMinScale - a.Scale) * weight_ * 0.01f;
                    }
                    else if (a.Scale > realMaxScale)
                    {
                        ds -= (a.Scale - realMaxScale) * weight_ * 0.01f;
                    }
                    // ノイズを付加する
                    if (false)
                    {
                        float variance = weight.NoiseWeight * 0.2f;
                        float noise = (float)randbm.NextDouble(variance);
                        ds += noise;
                    }
                }

                a.AddScale(ds);
            }
        }
    }
}
