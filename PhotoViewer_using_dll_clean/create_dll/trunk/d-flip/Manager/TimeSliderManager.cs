﻿using System.Collections.Generic;
using dflip.Element;
using dflip;
using Microsoft.Xna.Framework;

namespace dflip.Manager
{
    class TimeSliderManager
    {
        // 
        public ScrollBar sBar
        {
            get;
            private set;
        }
        //
        private List<Tip> timeSign = new List<Tip>(2);

        public TimeSliderManager()
        {
            sBar = new ScrollBar(1022);
            timeSign.Add(new Tip(sBar.MinDT, new Vector2(sBar.Min, (float)ScrollBar.Height * 1.45f)));
            timeSign.Add(new Tip(sBar.MaxDT, new Vector2(sBar.Max, (float)ScrollBar.Height * 1.45f)));
        }
        public void render()
        {
            Tip f1 = timeSign[0];
            Tip f2 = timeSign[1];
            f1.ChangeDT(sBar.MinDT);
            f2.ChangeDT(sBar.MaxDT);
            if (f1.Left != sBar.Min)
            {
                f1.MoveAtH(sBar.Min + (f1.Right - f1.Left) / 2f);
            }

            if (f2.Right != sBar.Max)
            {
                f2.MoveAtH(sBar.Max - (f2.Right - f2.Left) / 2);
            }

            f1.Render(SystemParameter.batch_, ResourceManager.font_, ResourceManager.fukiTex_);
            f2.Render(SystemParameter.batch_, ResourceManager.font_, ResourceManager.fukiTex_);
            //}
            // 
            sBar.Render(SystemParameter.batch_, SystemParameter.ClientWidth, ResourceManager.sBarTex1_, ResourceManager.sBarTex2_);
        }
    }
}
