using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Element.Dock;
using PhotoViewer.Element.ScrollBar;
using PhotoViewer.Element.StrokeTextbox;
using PhotoViewer.Attractor.Manager;

namespace PhotoViewer.Attractor
{
    class AttractorGeograph : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private float weight_ = 50;

        private float baseX = (float)Browser.Instance.ClientWidth;
        private float baseY = (float)Browser.Instance.ClientHeight;
        private List<float> leftup = new List<float>();
        private List<float> rightDown = new List<float>();
        private float dx, dy;
        private Dictionary<string, List<float>> countryInfo = new Dictionary<string, List<float>>();
        //private List<SStringIntInt> geotagList_ = new List<SStringIntInt>();

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.NonOverlapWeight;
            baseX = Browser.Instance.ClientWidth;
            baseY = Browser.Instance.ClientHeight;
            // 从ini文件获取geotag信息
            if (countryInfo.Count < 1)
            {//84.034319, 179.947926-51.289405, -148.059888
                //83.842130, 170.423871
                leftup.Add(-160.210936f);
                leftup.Add(88.780861f);
                rightDown.Add(170.976559f);
                rightDown.Add(-79.319496f);
                //leftup.Add(-179.947926f);
                //leftup.Add(84.034319f);
                //rightDown.Add(148.059888f);
                //rightDown.Add(-51.289405f);
                
                dx = rightDown[0] - leftup[0];
                dy = rightDown[1] - leftup[1];
                string home = "C:\\PhotoViewer";
                string iniName = "geotagList_AA.ini";
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
                        List<float> degree = new List<float>();
                        degree.Add((float.Parse(xy[1]) - leftup[0])/dx);
                        degree.Add((float.Parse(xy[0]) - leftup[1])/dy);
                        countryInfo[gt[0]] = degree; // 地名，xy坐标
                    }
                }
            }

            foreach (Photo a in photos)
            {
                Vector2 v = Vector2.Zero;
                bool flag = false;
                //foreach (var str in strokes)
                //{
                //    if (str.relatedPhotos.Contains(a))
                //    {
                //        flag = true;
                //        break;
                //    }
                //}
                //if (flag)
                //    continue;
                foreach (string country in countryInfo.Keys)
                {
                    if (a.containTag(country))
                    {
                        Vector2 target = new Vector2(countryInfo[country][0] * Browser.Instance.ClientWidth, countryInfo[country][1] * Browser.Instance.ClientHeight);
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
