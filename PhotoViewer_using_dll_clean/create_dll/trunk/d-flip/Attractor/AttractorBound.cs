//using System;
using System.Collections.Generic;
using PhotoInfo;
using dflip.Supplement;
using dflip.Element;
using dflip.Manager;
using dflip;
using Microsoft.Xna.Framework;

namespace Attractor
{
    class AttractorBound : IAttractorSelection
    {
        private readonly RandomBoxMuller randbm = new RandomBoxMuller();
        private int weight_ = 50;
        // avoid photo surpassing constraint power
        private readonly int INTO_DISPLAY = 20;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.NonOverlapWeight;

            foreach (Photo a in photos)
            {
                //velocity
                Vector2 v = Vector2.Zero;

                //avoid photo going out of the window(strong constraint)
#if NO_ROTATION
                if (systemState.curState != SystemState.ATTRACTOR_TIME)
                {
                    if (a.BoundingBox.Min.X < dock.DockBoundX)
                    {
                        v.X -= (a.BoundingBox.Min.X - dock.DockBoundX);
                    }
                    if (a.BoundingBox.Max.X > SystemParameter.ClientWidth)
                    {
                        v.X -= (a.BoundingBox.Max.X - SystemParameter.ClientWidth);
                    }
                }
                if (a.BoundingBox.Min.Y < 0)
                {
                    v.Y -= (a.BoundingBox.Min.Y);
                }
                if (a.BoundingBox.Max.Y > SystemParameter.ClientHeight)
                {
                    v.Y -= (a.BoundingBox.Max.Y - SystemParameter.ClientHeight);
                }
                v *= 0.02f * INTO_DISPLAY * weight_;
#else
                float va = 0f;
                for (int i = 0; i < 4; ++i)
                {
                    Vector2 v1 = a.Position - a.BoudingBox.Vertex[i];
                    v1.Normalize();
                    Vector2 v2 = Vector2.Zero;
                    float dist = 0f;
                    if (a.BoudingBox.Vertex[i].X < input.DockBound)
                    {
                        v2 = Vector2.UnitX;
                        dist = input.DockBound - a.BoudingBox.Vertex[i].X ;
                    }
                    if (a.BoudingBox.Vertex[i].X > input.WindowWidth)
                    {
                        v2 = -Vector2.UnitX;
                        dist = a.BoudingBox.Vertex[i].X - input.WindowWidth;
                    }
                    if (a.BoudingBox.Vertex[i].Y < 0)
                    {
                        v2 = Vector2.UnitY;
                        dist = -a.BoudingBox.Vertex[i].Y;
                    }
                    if (a.BoudingBox.Vertex[i].Y > input.WindowHeight)
                    {
                        v2 = -Vector2.UnitY;
                        dist = a.BoudingBox.Vertex[i].Y - input.WindowHeight;
                    }
                    //v += v1 * (float)(dist * Math.Abs(v1.X * v2.X + v1.Y * v2.Y));
                    v += dist * v2;
                    va += -(v1.X * v2.Y - v1.Y * v2.X) * INTO_DISPLAY;
                }
                v *= 0.02f * INTO_DISPLAY * weight_;
#endif

                // add noise
                if (true)
                {
                    float variance = weight.NoiseWeight * 0.5f;
                    Vector2 noise = new Vector2((float)randbm.NextDouble(variance), (float)randbm.NextDouble(variance));
                    v += noise;
                }
                a.AddPosition(v);
#if NO_ROTATION
#else
                if (input.EnabledNoise)
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
