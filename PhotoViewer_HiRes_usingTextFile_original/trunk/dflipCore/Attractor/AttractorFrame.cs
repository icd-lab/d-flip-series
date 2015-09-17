using System;
using System.Collections.Generic;
using PhotoViewer.Manager;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PhotoViewer.Element;
using PhotoViewer.Supplement;
using PhotoInfo;
//using PhotoViewer.InputDevice;
//using System.Diagnostics;

namespace Attractor
{
    class AttractorFrame : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;
        // 画面离开屏幕的制约力量
        private readonly int INTO_DISPLAY = 10;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            
            weight_ = weight.NonOverlapWeight;
            //List<Stroke> strokes = strokeCol.strokeList;
            foreach (var stroke in strokes)
            {
                if (!stroke.IsClosed)
                    continue;
                
                foreach (Photo a in photos)
                {
                    bool inner = stroke.IsInternal(a.Position);
                    Vector2 v = Vector2.Zero;
                    // 到最近锚点的矢量
                    Vector2 v2n = Vector2.One * float.MaxValue;

                    if (stroke.relatedPhotos.Contains(a))
                        a.IsFollowing = true;
                    if (stroke.relatedPhotos.Contains(a) && !inner)
                    {
                        foreach (Vector2 s in stroke.Strokes)
                        {
                            Vector2 dist = s - a.Position;
                            if (dist.LengthSquared() < v2n.LengthSquared())
                            {
                                v2n = dist;
                            }
                        }
                        v += v2n;
                        //v += (stroke.Center - a.Position);
                        v *= 0.02f * INTO_DISPLAY * weight_;
                    }

#if STRICT
                            bool inner1 = strokes[i][j].IsInternal(a.BoudingBox.Min);
                            bool inner2 = strokes[i][j].IsInternal(new Vector2(a.BoudingBox.Min.X, a.BoudingBox.Max.Y));
                            bool inner3 = strokes[i][j].IsInternal(a.boudingBox_.Max);
                            bool inner4 = strokes[i][j].IsInternal(new Vector2(a.BoudingBox.Max.X, a.BoudingBox.Min.Y));
                            if(inner || inner1 || inner2 || inner3 || inner4)
                            {
#else
                    else if (!stroke.relatedPhotos.Contains(a) && inner)
                    {
#endif
                        foreach (Vector2 s in stroke.Strokes)
                        {
                            Vector2 dist = s - a.Position;
                            if (dist.LengthSquared() < v2n.LengthSquared())
                            {
                                v2n = dist;
                            }
                        }
                        //v -= v2n;
                        v += (a.Position - stroke.Center);
                        v *= 0.02f * INTO_DISPLAY * weight_;
                    }
                    // 添加噪音
                    if (false)
                    {
                        if (v != Vector2.Zero)
                        {
                            float variance = weight.NoiseWeight * 0.5f;
                            Vector2 noise = new Vector2((float)randbm.NextDouble(variance), (float)randbm.NextDouble(variance));
                            v += noise;
                        }
                    }
                    a.AddPosition(v);
                }
            
                
            }
        }
    }
}
