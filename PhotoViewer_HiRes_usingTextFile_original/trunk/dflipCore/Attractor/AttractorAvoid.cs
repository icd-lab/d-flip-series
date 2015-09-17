//using System;
using System.Collections.Generic;
//using System.Text;
using Microsoft.Xna.Framework;
using PhotoViewer;
using PhotoViewer.Supplement;
using PhotoViewer.Element;
using PhotoViewer.Manager;
using PhotoInfo;

//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorAvoid : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.NonOverlapWeight * (ResourceManager.MAXX + ResourceManager.MAXY);

            // 吸引子选择
            foreach (Photo a in photos)
            {
                // 速度
                Vector2 v = Vector2.Zero;

                // 避免相邻图像重叠的移动方向
                foreach (AdjacentPhoto b in a)
                {
                    if (a.IsGazeds && b.Photo.IsGazeds)
                    {
                        v += b.Direction * 0.2f * weight_ / 150f;
                        if (a.touchCount != 0 && b.Photo.touchCount != 0)
                        {
                            if (a.touchCount <= b.Photo.touchCount && a.LayerDepth >= b.Photo.LayerDepth)
                            {
                                b.Photo.LayerDepth = a.LayerDepth + 0.001f;
                                if (b.Photo.LayerDepth > 1f)
                                    b.Photo.LayerDepth = 1f;
                            }
                            else if (b.Photo.touchCount < a.touchCount && b.Photo.LayerDepth >= a.LayerDepth)
                            {
                                a.LayerDepth = b.Photo.LayerDepth + 0.001f;
                                if (a.LayerDepth > 1f)
                                    a.LayerDepth = 1f;
                            }
                        }
                    }
                    else
                    {
                        v += b.Direction * 0.02f * weight_/ 150f;
                    }
                }

                // 添加噪声
                if (false)
                {
                    float variance = weight.NoiseWeight * 0.5f;
                    Vector2 noise = new Vector2((float)randbm.NextDouble(variance), (float)randbm.NextDouble(variance));
                    v += noise;
                }

                a.AddPosition(v);

#if NO_ROTATION
#else
                // 回転角
                float va = 0f;
                foreach (AdjacentPhoto b in a)
                {
                    va += b.AngleDirection;
                }

                //
                //

                float at = 0f;
                //// 1点を中心に回転させる
                //Vector2 spinCenter = new Vector2(input.WindowWidth / 2f, input.WindowHeight / 2f);
                ////spinCenter.Y = input.WindowHeight;
                //at = (float)(-Math.Atan2((double)(a.Position.X - (double)spinCenter.X), (double)(a.Position.Y - (double)spinCenter.Y)));
                //if (at > 0)
                //{
                //    at -= (float)Math.PI;
                //}
                //else
                //{
                //    at += (float)Math.PI;
                //}
                // できるだけ下を向かせる
                at = 0f;

                va += (at - a.Angle);

                //
                //

                // ノイズを付加する
                if (false)
                {
                    float variance = weight.NoiseWeight * 0.1f;
                    float noise = (float)randbm.NextDouble(variance);
                    va += noise;
                }
                a.AddAngle(va);
#endif
            }
        }
    }
}
