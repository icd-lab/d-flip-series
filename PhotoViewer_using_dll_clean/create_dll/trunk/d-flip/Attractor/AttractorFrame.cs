using System;
using System.Collections.Generic;
using dflip.Manager;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using dflip.Element;
using dflip.Supplement;
using PhotoInfo;
//using PhotoViewer.InputDevice;
//using System.Diagnostics;

namespace Attractor
{
    class AttractorFrame : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;
        // 
        private readonly int INTO_DISPLAY = 20;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            
            weight_ = weight.NonOverlapWeight;
            //List<Stroke> strokes = strokeCol.strokeList;
            foreach (var stroke in strokes)
            {
                if (!stroke.IsClosed)
                    continue;
                
                //foreach (Photo a in photos)
                //{
                    foreach (Photo a in photos)
                    {
                        //Console.WriteLine(a.Position);
                        Vector2 v = Vector2.Zero;
                        // 到最近锚点的矢量
                        Vector2 v2n = Vector2.One * float.MaxValue;
                        foreach (Vector2 s in stroke.Strokes)
                        {
                            Vector2 dist = s - a.Position;
                            if (dist.LengthSquared() < v2n.LengthSquared())
                            {
                                v2n = dist;
                            }
                        }
                        //if(v2n.X == 1f/0)
                            //Console.WriteLine(v2n);
                        int matchedTagCount = 0;
                        foreach (string t in stroke.Tags)
                        {
                            if (a.containTag(t))
                            {
                                ++matchedTagCount;
                                break;
                            }
                        }
                        if (matchedTagCount == 0)
                        {
                            if (stroke.IsInternal(a.Position, stroke.outsideStrokes_))
                            {
                                v += (a.Position - stroke.Center);
                                v *= 0.002f * INTO_DISPLAY * weight_;
                            }
                        }
                        else
                        {
                            if (!stroke.IsInternal(a.Position, stroke.insideStrokes_))
                            {
                                v += (stroke.Center - a.Position);
                                v *= 0.002f * INTO_DISPLAY * weight_;
                            }
                        }
                        /*
                        bool inner = stroke.IsInternal(a.Position);
                        //test four points of each image
#if STRICT
                            bool inner1 = stroke.IsInternal(a.boundingBox_.Min);
                            bool inner2 = stroke.IsInternal(new Vector2(a.boundingBox_.Min.X, a.boundingBox_.Max.Y));
                            bool inner3 = stroke.IsInternal(a.boundingBox_.Max);
                            bool inner4 = stroke.IsInternal(new Vector2(a.boundingBox_.Max.X, a.boundingBox_.Min.Y));
                            if(inner1 || inner2 || inner3 || inner4)
                            {
#else
                        if (inner)
                        {
#endif
                            int matchedTagCount = 0;
                            foreach (string t in stroke.Tags)
                            {
                                if (a.containTag(t))
                                {
                                    ++matchedTagCount;
                                    break;
                                }
                            }
                            if (matchedTagCount == 0)
                            {
                               // v += v2n;
                                v += (a.Position - stroke.Center);
                                v *= 0.002f * INTO_DISPLAY * weight_;
                            }
#if STRICT
                            }
                            if (!inner1 || !inner2 || !inner3 || !inner4)
                            {
#else
                        }
                        else
                        {
#endif
                            int matchedTagCount = 0;
                            foreach (string t in stroke.Tags)
                            {
                                if (a.containTag(t))
                                {
                                    ++matchedTagCount;
                                    break;
                                }
                            }
                            if (matchedTagCount > 0)
                            {
                                //v += v2n;
                                v += (stroke.Center - a.Position);
                                v *= 0.002f * INTO_DISPLAY * weight_;
                                //Console.WriteLine(v);
                            }
                        }
                         * */
                    //bool inner = stroke.IsInternal(a.Position);
                    //Vector2 v = Vector2.Zero;
                    //// vector to the nearest point
                    //Vector2 v2n = Vector2.One * float.MaxValue;

                    ////if (stroke.relatedPhotos.Contains(a))
                    //    //a.IsFollowing = true;
                    //if (stroke.relatedPhotos.Contains(a) && !inner)
                    //{
                    //    foreach (Vector2 s in stroke.Strokes)
                    //    {
                    //        Vector2 dist = s - a.Position;
                    //        if (dist.LengthSquared() < v2n.LengthSquared())
                    //        {
                    //            v2n = dist;
                    //        }
                    //    }
                    //    //v += v2n;
                    //    v += (stroke.Center - a.Position);
                    //    v *= 0.02f * INTO_DISPLAY * weight_;
                    //}
                    //else if (!stroke.relatedPhotos.Contains(a) && inner)
                    //{
                    //    foreach (Vector2 s in stroke.Strokes)
                    //    {
                    //        Vector2 dist = s - a.Position;
                    //        if (dist.LengthSquared() < v2n.LengthSquared())
                    //        {
                    //            v2n = dist;
                    //        }
                    //    }
                    //    //v += v2n;
                    //    v += (a.Position - stroke.Center);
                    //    v *= 0.02f * INTO_DISPLAY * weight_;
                    //}
                    // noise
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
