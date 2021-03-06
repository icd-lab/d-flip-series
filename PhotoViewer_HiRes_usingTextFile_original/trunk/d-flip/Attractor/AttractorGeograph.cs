﻿using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using PhotoInfo;
using PhotoViewer;
using PhotoViewer.Supplement;
using PhotoViewer.Element;
using PhotoViewer.Manager;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorGeograph : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private float weight_ = 50;

        private float baseX = (float)SystemParameter.ClientWidth;
        private float baseY = (float)SystemParameter.ClientHeight;
        private const float bx = 1750f;
        private const float by = 1196f;
        //private const float mapDef = 675f;
        //private const float mapX = 1750f;
        private List<SStringIntInt> geotagList_ = new List<SStringIntInt>();

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.NonOverlapWeight;
            baseX = SystemParameter.ClientWidth;
            baseY = SystemParameter.ClientHeight;
            // 从ini文件获取geotag信息
            if (geotagList_.Count < 1)
            {
                string home = "C:\\PhotoViewer";
                string iniName = "geotagList_tohoku.ini";
                if (File.Exists(home + "\\" + iniName))
                {
                    string gtl = File.ReadAllText(home + "\\" + iniName);
                    string[] sep = new string[1];
                    sep[0] = "\r\n";
                    string[] gts = gtl.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0, ilen = gts.Length; i < ilen; ++i)
                    {
                        sep[0] = ":";
                        string[] gt = gts[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        sep[0] = ",";
                        string[] xy = gt[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);
#if JAPANESE_MAP
                        int x = int.Parse(xy[0]);
#else
                        //int x = ((int)(float.Parse(xy[0]) + mapDef * bx / mapX)) % ((int)bx);
                        int x = int.Parse(xy[0]);
#endif
                        int y = int.Parse(xy[1]);
                        geotagList_.Add(new SStringIntInt(gt[0], x, y)); // 地名，xy坐标
                    }
                }
            }

            foreach (Photo a in photos)
            {
                Vector2 v = Vector2.Zero;
                foreach (SStringIntInt gt in geotagList_)
                {
                    if (a.containTag(gt.Name))
                    {
                        Vector2 target = new Vector2((float)gt.X * baseX / bx, (float)gt.Y * baseY / by);
                        v += target - a.Position;
                        break;
                    }
                }

                // 改变噪声方向
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
                a.AddPosition(v);
            }
        }
    }
}
