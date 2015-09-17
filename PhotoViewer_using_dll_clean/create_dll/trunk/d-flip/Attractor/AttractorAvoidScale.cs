//using System;
//using System.Linq;
using System.Collections.Generic;
//using Microsoft.Xna.Framework;
using dflip.Supplement;
using dflip.Element;
using dflip;
using dflip.Manager;
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

            float MinPhotoSize = SystemParameter.MinPhotoScale(SystemParameter.ClientWidth, SystemParameter.ClientHeight, ResourceManager.MAXX, ResourceManager.MAXY, photos.Count);
            //MinPhotoSize = 0f;// for movie
            float MaxPhotoSize = SystemParameter.MaxPhotoScale(SystemParameter.ClientWidth, SystemParameter.ClientHeight, ResourceManager.MAXX, ResourceManager.MAXY, photos.Count);

            weight_ = weight.ScaleWeight;

            foreach (Photo a in photos)
            {
                // big scale velocity
                float ds = 0;

                // added by Gengdai
                realMinScale = a.GetTexture().Width > a.GetTexture().Height ? MinPhotoSize * ResourceManager.MAXX / a.GetTexture().Width : MinPhotoSize * ResourceManager.MAXY / a.GetTexture().Height;
                realMaxScale = a.GetTexture().Width > a.GetTexture().Height ? MaxPhotoSize * ResourceManager.MAXX / a.GetTexture().Width : MaxPhotoSize * ResourceManager.MAXY / a.GetTexture().Height;
                aPhotoArea = a.Scale * a.GetTexture().Width * a.Scale * a.GetTexture().Height;

                // restraint to avoid overlapping
                if (a.Adjacency.Count > 0)
                {
                    foreach (AdjacentPhoto b in a)
                    {
                        bPhotoArea = b.Photo.Scale * b.Photo.GetTexture().Width * b.Photo.Scale * b.Photo.GetTexture().Height;
                        
                        // avoid overlapping, decrease MinPhotoSize
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

                // avoid MinPhotoSize getting bigger than MaxPhotoSize
                if (a.Scale < realMinScale)
                {
                    ds += (realMinScale - a.Scale) * 0.02f * weight_;
                }
                else if (a.Scale > realMaxScale)
                {
                    ds -= (a.Scale - realMaxScale) * 0.02f * weight_;
                }

                // add noise
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
