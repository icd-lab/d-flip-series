using System;
using System.Collections.Generic;
//using System.Text;
using Microsoft.Xna.Framework;
using PhotoViewer.Manager;
using PhotoViewer.Element;
using PhotoViewer.Supplement;
using PhotoInfo;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorColor : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private float weight_ = 50;

        private List<Photo> activeOld_ = new List<Photo>();
        private Dictionary<Photo, List<photoDis>> dists_ = new Dictionary<Photo, List<photoDis>>();
        //private List<double> threshold_ = new List<double>();
        private const int attractNum_ = 3;
        
        private class photoDis:IComparable<photoDis>
        {
            public Photo photo;
            public double dis;
            public photoDis(Photo p, double d)
            {
                photo = p;
                dis = d;
            }
            public int CompareTo(photoDis other)
            {
                return dis.CompareTo(other.dis);
            }
        }

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.TagWeight;
            if (photos.Count < 10)
                return;
            
            //while (activeOld_.Count < input.PointingDevices.Count)
            //{
            //    activeOld_.Add(null);
            //}
            //while (dists_.Count < input.PointingDevices.Count)
            //{
            //    dists_.Add(new List<SIntDouble>());
            //}
            //while (threshold_.Count < input.PointingDevices.Count)
            //{
            //    threshold_.Add(0d);
            //}
            //for (int i = 0; i < input.PointingDevices.Count; i++)
            //{
                //Photo activePhoto = null;

                // 注目されている画像を activePhoto とする
            foreach (Photo a in activePhotos)
            {
                //if (!a.activeTag.Contains("Color"))
                //{
                //    continue;
                //}
                if (!dists_.ContainsKey(a))
                {
                    List<photoDis> dis = new List<photoDis>();

                    foreach (Photo p in photos)
                    {
                        if (a != p)
                        {
                            photoDis one = new photoDis(p, p.PhotoDist(a));
                            dis.Add(one);
                        }

                    }
                    // 距離の閾値を 全体の 1/attractNum_ が寄ってくる値にする
                    dis.Sort(); // 按值排序
                    //threshold = dists_[i][dists_[i].Count / attractNum_].value;
                    dists_[a] = dis;
                }
                List<photoDis> distance = dists_[a];
                double threshold = dists_[a][distance.Count / attractNum_].dis;
                for (int i = 0; i < distance.Count / attractNum_; i++)
                {
                    
                    Photo p = distance[i].photo;
                    p.IsFollowing = true;

                    Vector2 v = a.Position - p.Position;
                    v *= (float)(threshold - distance[i].dis);
                    v *= weight_ / 2f;// 10f;

                    // 改变方向的噪音
                    if (v != Vector2.Zero && false)
                    {
                        float noise = (float)((1 - Math.Exp(-rand.NextDouble())) * Math.PI);
                        noise *= (float)Math.Log(a.Adjacency.Count + 1);
                        if (rand.NextDouble() < 0.5)
                        {
                            noise *= -1;
                        }
                        float cnoise = (float)Math.Cos(noise);
                        float snoise = (float)Math.Sin(noise);
                        Vector2 noisyv = new Vector2(v.X * cnoise - v.Y * snoise, v.X * snoise + v.Y * cnoise);
                        v = noisyv;
                    }
                    p.AddPosition(v);

                }
            }
            
        }
    }
}
