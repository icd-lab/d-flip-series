﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoInfo;
using PhotoViewer;
using PhotoViewer.Manager;
using PhotoViewer.Element;
using PhotoViewer.Supplement;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorScaleUp : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;

        // added by Gengdai
        private float realMinScale = 0.0f;
        private float realMaxScale = 0.0f;
        private float followMinScale = 0.0f;
        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            float MinPhotoSize = Browser.MinPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count);
            //MinPhotoSize = 0f;// ムービー用
            float MaxPhotoSize = Browser.MaxPhotoScale(Browser.Instance.ClientWidth, Browser.Instance.ClientHeight, Browser.MAXX, Browser.MAXY, photos.Count);

            weight_ = weight.ScaleWeight;

            // アトラクター選択
            foreach (Photo a in photos)
            {
                // スケール速度
                float ds = 0;

                // added by Gengdai
                realMinScale = a.GetTexture().Width > a.GetTexture().Height ? MinPhotoSize * Browser.MAXX / a.GetTexture().Width : MinPhotoSize * Browser.MAXY / a.GetTexture().Height;
                realMaxScale = a.GetTexture().Width > a.GetTexture().Height ? MaxPhotoSize * Browser.MAXX / a.GetTexture().Width : MaxPhotoSize * Browser.MAXY / a.GetTexture().Height;
                followMinScale = realMinScale * 10f;
                if (followMinScale > realMaxScale)
                    followMinScale = realMaxScale;
                // 重ならないように制約
                if (a.Adjacency.Count == 0)
                {
                    // 周りに画像がなければMaxPhotoSizeまで拡大させる
                    ds += (realMaxScale - a.Scale) * 0.01f * weight_;
                }

                // サイズがMinPhotoSize以下もしくはMaxPhotoSize以上になるのを防ぐ制約
                if (a.IsFollowing && a.Scale < followMinScale)
                {
                    ds += (followMinScale - a.Scale) * 0.02f * weight_;
                }
                else if (!a.IsFollowing && a.Scale < realMinScale)
                {
                    ds += (realMinScale - a.Scale) * 0.02f * weight_;
                }
                else if (a.Scale > realMaxScale)
                {
                    ds -= (a.Scale - realMaxScale) * 0.02f * weight_;
                }

                //ノイズを付加する
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
