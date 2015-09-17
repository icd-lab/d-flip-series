using System;
using System.Collections.Generic;
using PhotoInfo;
using dflip.Manager;
using dflip.Element;
//using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;


namespace Attractor
{
    class AttractorTag : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private float weight_ = 50;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.TagWeight;

            // íçñ⁄Ç≥ÇÍÇƒÇ¢ÇÈâÊëúÇtemActivePhotoÇ∆Ç∑ÇÈ
            foreach (Photo a in activePhotos)
            {
                if (a.activeTag.Count == 0 || (a.activeTag.Count == 1 && a.activeTag.Contains("Color")))
                    continue;
                                        // äeâÊëúÇÃà⁄ìÆ
                foreach (Photo photo in photos)
                {
                    if (photo.ID == a.ID)
                        continue;
                    bool matched = false;
                    foreach (String tag in a.activeTag)
                    {
                        if (tag.Equals("Color"))
                            continue;
                        if (photo.containTag(tag))
                        {
                            matched = true;
                            break;
                        }
                    }
                    Vector2 v = a.Position - photo.Position;
                    if (matched)
                    {
                        photo.IsFollowing = true;
                        Vector2 tempdir = Vector2.Zero;
                        float tempdira = 0f;
                        if (a.boundingBox_.Overrap(photo.boundingBox_, ref tempdir, ref tempdira))
                        {
                            v *= -1f;
                        }
                        v *= (float)3f;
                    }
                    else
                    {
                        v *= -1f;
                    }
                    v *= weight_ / 128f;

                    // noise
                    if (v != Vector2.Zero && false) // noise enabled
                    {
                        float noise = (float)((1 - Math.Exp(-rand.NextDouble())) * Math.PI);
                        noise *= (float)Math.Log(photo.Adjacency.Count + 1);
                        if (rand.NextDouble() < 0.5)
                        {
                            noise *= -1;
                        }
                        float cnoise = (float)Math.Cos(noise);
                        float snoise = (float)Math.Sin(noise);
                        Vector2 noisyv = new Vector2(v.X * cnoise - v.Y * snoise, v.X * snoise + v.Y * cnoise);
                        v = noisyv;
                    }

                    photo.AddPosition(v);
                }
            }


        }
    }
}
