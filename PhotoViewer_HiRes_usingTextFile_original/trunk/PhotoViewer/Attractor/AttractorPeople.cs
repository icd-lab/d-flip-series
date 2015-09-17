using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoViewer.Element;
using PhotoViewer.Manager;
using PhotoInfo;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    // added by Gengdai
    class AttractorPeople : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private int weight_1 = 20;
        private int weight_2 = 5;
        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            //Photo clickedPhoto = null;

            // 找到被点击的图片
            foreach (Photo a in activePhotos)
            {
                if (a.IsClicked == false)
                {
                    continue;
                }
                //if one picture is clickes
                List<PeopleTag> currentTagList = a.PTags.pTags;
                string selectedPeopleName = null;
                // 如果被选中的图片没有人物标签则返回，不予处理
                if (currentTagList == null)
                    continue;
                foreach (PeopleTag p in currentTagList)
                {
                    Rectangle box = p.Box;
                    Rectangle newbox = new Rectangle((int)((float)box.X * a.ScaleDisplay), (int)((float)box.Y * a.ScaleDisplay)
                        , (int)((float)box.Width * a.ScaleDisplay), (int)((float)box.Height * a.ScaleDisplay));

                    if (newbox.Contains((int)a.ClickedPoint.X, (int)a.ClickedPoint.Y))
                    {
                        selectedPeopleName = p.People;
                        break;
                    }
                }

                // 如果选中了某人
                if (selectedPeopleName != null)
                {
                    foreach (Photo p in photos)
                    {
                        Vector2 v = Vector2.Zero;
                        if (p.ptag.allTags.Contains(selectedPeopleName)) // 对于那些被吸引的图像
                        {
                            v = a.Position - p.Position; // 吸引
                            v *= weight_1 / 2f;// 10f;  
                        }
                        else // 为被吸引的图像
                        {
                            v = p.Position - a.Position; // 排斥
                            v *= weight_2 / 3.0f;// 10f;  
                        }

                        // 改变噪声方向
                        if (v != Vector2.Zero && false)
                        {
                            float noise = (float)((1 - Math.Exp(-rand.NextDouble())) * Math.PI);
                            noise *= (float)Math.Log(p.Adjacency.Count + 1);
                            if (rand.NextDouble() < 0.5)
                            {
                                noise *= -1;
                            }
                            float cnoise = (float)Math.Cos(noise) / 3f;
                            float snoise = (float)Math.Sin(noise) / 3f;
                            Vector2 noisyv = new Vector2(v.X * cnoise - v.Y * snoise, v.X * snoise + v.Y * cnoise);
                            v = noisyv;
                        }

                        // 将改变的向量v作用于图像
                        p.AddPosition(v);
                    }
                }
            }
        }
    }
}
