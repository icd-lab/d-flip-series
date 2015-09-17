using System;
using System.Collections.Generic;
using PhotoViewer;
using PhotoInfo;
//using System.Text;
using Microsoft.Xna.Framework;
using PhotoViewer.Manager;
using PhotoViewer.Element;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace Attractor
{
    class AttractorTime : IAttractorSelection
    {
        private readonly Random rand = new Random();
        private float weight_ = 50;

        public void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            weight_ = weight.NonOverlapWeight;
            // 最も古い写真と新しい写真の撮影日時を取得
            DateTime mindt = DateTime.MaxValue;
            DateTime maxdt = DateTime.MinValue;
            foreach (Photo a in photos)
            {
                if (mindt > a.ptag.CapturedDate)
                {
                    mindt = a.ptag.CapturedDate;
                }
                if (maxdt < a.ptag.CapturedDate)
                {
                    maxdt = a.ptag.CapturedDate;
                }
            }
            sBar.Oldest = mindt;
            sBar.Newest = maxdt;
            // ウインドウ表示範囲内で最も古い写真と新しい写真の撮影日時を指定
            double max = maxdt.Subtract(mindt).TotalSeconds;
            double minw = max * (double)sBar.Min / (double)sBar.Width;
            double maxw = max * (double)sBar.Max / (double)sBar.Width;
            foreach (Photo a in photos)
            {
                Vector2 v = Vector2.Zero;
                DateTime date = a.ptag.CapturedDate;
                //DateTime end = new DateTime(a.ptag.endDate, 12, 31);

                double x = date.Subtract(mindt).TotalSeconds;
                x -= minw;

                x *= (double)sBar.Width / Math.Max((maxw - minw), 1d);
                //if (x + a.Width / 2 > sBar.Width)
                    //v = Vector2.UnitX * (float)(x - a.Position.X + a.Width / 2) * 0.02f * weight_;
                //else if (x - a.Width / 2 < 0)
                    //v = Vector2.UnitX * (float)(x - a.Position.X - a.Width / 2) * 0.02f * weight_;
                //else
                    v = Vector2.UnitX * (float)(x - a.Position.X) * 0.02f * weight_;
                //Vector2 v = Vector2.Zero;

                //double x = a.ptag.CapturedDate.Subtract(mindt).TotalSeconds;
                //x -= minw;
                //x *= (double)sBar.Width / Math.Max((maxw - minw), 1d);
                //v = Vector2.UnitX * (float)(x - a.Position.X) * 0.02f * weight_;

                //// ノイズで方向を変化させる
                //if (v != Vector2.Zero && false)
                //{
                //    float noise = (float)((1 - Math.Exp(-rand.NextDouble())) * Math.PI);
                //    noise *= (float)Math.Log(a.Adjacency.Count + 1);
                //    if (rand.NextDouble() < 0.5)
                //    {
                //        noise *= -1;
                //    }
                //    float cnoise = (float)Math.Cos(noise);
                //    float snoise = (float)Math.Sin(noise);
                //    Vector2 noisyv = new Vector2(v.X * cnoise - v.Y * snoise, v.X * snoise + v.Y * cnoise);
                //    v = noisyv;
                //}
                a.AddPosition(v);
            }
        }
            //{
        //    weight_ = weight.NonOverlapWeight;
        //    // 最も古い写真と新しい写真の撮影日時を取得
        //    DateTime mindt = DateTime.MaxValue;
        //    DateTime maxdt = DateTime.MinValue;
        //    foreach (Photo a in photos)
        //    {
        //        if (a.ptag.startDate == 0)
        //        {
        //            continue;
        //        }
        //        DateTime start = new DateTime(a.ptag.startDate,1,1);
        //        DateTime end = new DateTime(a.ptag.endDate,12,31);
        //        if (mindt > start)
        //        {
        //            mindt = start;
        //        }
        //        if (maxdt < end)
        //        {
        //            maxdt = end;
        //        }
        //    }
        //    sBar.Oldest = mindt;
        //    sBar.Newest = maxdt;
        //    // ウインドウ表示範囲内で最も古い写真と新しい写真の撮影日時を指定
        //    double max = maxdt.Subtract(mindt).TotalSeconds;
        //    double minw = max * (double)sBar.Min / (double)sBar.Width;
        //    double maxw = max * (double)sBar.Max / (double)sBar.Width;
        //    foreach (Photo a in photos)
        //    {
        //        Vector2 v = Vector2.Zero;
        //        if (a.ptag.startDate == 0)
        //            continue;
        //        DateTime start = new DateTime(a.ptag.startDate, 1, 1);
        //        DateTime end = new DateTime(a.ptag.endDate, 12, 31);
                
        //        double x = start.Subtract(mindt).TotalSeconds;
        //        x -= minw;

        //        if (x < 0 && end.Year > start.Year)
        //        {
        //            x = end.Subtract(mindt).TotalSeconds;
        //            x -= minw;
        //        }
        //        x *= (double)sBar.Width / Math.Max((maxw - minw), 1d);
        //        if(x + a.Width/2 > sBar.Width)
        //            v = Vector2.UnitX * (float)(x - a.Position.X - a.Width/2) * 0.02f * weight_;
        //        else if(x - a.Width/2 < 0)
        //            v = Vector2.UnitX * (float)(x - a.Position.X + a.Width/2) * 0.02f * weight_;
        //        else
        //        v = Vector2.UnitX * (float)(x - a.Position.X) * 0.02f * weight_;

        //        // ノイズで方向を変化させる
        //        if (v != Vector2.Zero && false)
        //        {
        //            float noise = (float)((1 - Math.Exp(-rand.NextDouble())) * Math.PI);
        //            noise *= (float)Math.Log(a.Adjacency.Count + 1);
        //            if (rand.NextDouble() < 0.5)
        //            {
        //                noise *= -1;
        //            }
        //            float cnoise = (float)Math.Cos(noise);
        //            float snoise = (float)Math.Sin(noise);
        //            Vector2 noisyv = new Vector2(v.X * cnoise - v.Y * snoise, v.X * snoise + v.Y * cnoise);
        //            v = noisyv;
        //        }
        //        a.AddPosition(v);
        //    }
        //}
    }
}
