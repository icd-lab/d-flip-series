using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using PhotoInfo;
using dflip;
using dflip.Supplement;
using dflip.Element;
using dflip.Manager;

namespace Attractor
{
    class AttractorGeograph : IAttractorSelection
    {
        private readonly Random rand = new Random();
        //private float weight_ = 50;

        private float baseX = (float)SystemParameter.ClientWidth;
        private float baseY = (float)SystemParameter.ClientHeight;
        private float bx = 1022f;
        private float by = 748f;
        private const float mapDef = 675f;
        private const float mapX = 1750f;
        private List<SStringIntInt> geotagList_ = new List<SStringIntInt>();
        private List<SStringIntInt> geotagList_tohoku = new List<SStringIntInt>();

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            //weight_ = weight.NonOverlapWeight;
            baseX = SystemParameter.ClientWidth;
            baseY = SystemParameter.ClientHeight;
            // get geotag info from ini file
            if (geotagList_.Count < 1)
            {
                string iniName;
                if (ResourceManager.homeDirectory_ == null)
                    iniName = "geotagList.ini";
                else iniName = ResourceManager.homeDirectory_ + "\\" + "geotagList.ini"; 
                if (File.Exists(iniName))
                {
                    string gtl = File.ReadAllText(iniName);
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
                        int x = ((int)(float.Parse(xy[0]) + mapDef * bx / mapX)) % ((int)bx);
                        //int x = int.Parse(xy[0]);
#endif
                        int y = int.Parse(xy[1]);
                        geotagList_.Add(new SStringIntInt(gt[0], x, y)); //place，coordinate xy
                    }
                }
            }

            if (ResourceManager.IfTohoku)
            {
                bx = 1750f;
                by = 1196f;

                //read tohoku geotag list
                if (geotagList_tohoku.Count < 1)
                {
                    string iniName;
                    if (ResourceManager.homeDirectory_ == null)
                        iniName = "geotagList_tohoku.ini";
                    else iniName = ResourceManager.homeDirectory_ + "\\" + "geotagList_tohoku.ini";
                    if (File.Exists(iniName))
                    {
                        string gtl = File.ReadAllText(iniName);
                        string[] sep = new string[1];
                        sep[0] = "\r\n";
                        string[] gts = gtl.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0, ilen = gts.Length; i < ilen; ++i)
                        {
                            sep[0] = ":";
                            string[] gt = gts[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            sep[0] = ",";
                            string[] xy = gt[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            int x = int.Parse(xy[0]);
                            int y = int.Parse(xy[1]);
                            geotagList_tohoku.Add(new SStringIntInt(gt[0], x, y)); // 地名，xy坐标
                        }
                    }
                }
            }

            else
            {
                bx = 1022f;
                by = 748f;
            }            

            foreach (Photo a in photos)
            {
                bool flag = false;
                foreach (Stroke s in strokes)
                {
                    if (s.relatedPhotos.Contains(a))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    continue;
                Vector2 v = Vector2.Zero;
                if (ResourceManager.IfTohoku)
                {
                    foreach (SStringIntInt gt in geotagList_tohoku)
                    {
                        if (a.containTag(gt.Name))
                        {
                            Vector2 target = new Vector2((float)gt.X * baseX / bx, (float)gt.Y * baseY / by);
                            v += target - a.Position;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (SStringIntInt gt in geotagList_)
                    {
                        if (a.containTag(gt.Name))
                        {
                            Vector2 target = new Vector2((float)gt.X * baseX / bx, (float)gt.Y * baseY / by);
                            v += target - a.Position;
                            break;
                        }
                    }
                }
                // noise
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
