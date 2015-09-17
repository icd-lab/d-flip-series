using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.IO;

namespace PhotoInfo
{
    public class ArtworksTag : PhotoTag
    {
        public class TagEntity
        {
            public string Name
            {
                get;
                set;
            }
            //public int StartYear
            //{
            //    get;
            //    set;
            //}
            //public int EndYear
            //{
            //    get;
            //    set;
            //}
            public Vector3[] feature
            {
                get;
                set;
            }
            public double variance
            {
                set;
                get;
            }
            public TagEntity()
            {
            }
            public TagEntity(string from, Photo.colorFeature color)
            {
                Name = from;
                feature = color.feature_;
                variance = color.variance_;
            }
        }

        public static Dictionary<string, Photo.colorFeature> FromXml(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TagEntity[]));
            var tags = (TagEntity[])ser.Deserialize(new StringReader(xml));
            Dictionary<string, Photo.colorFeature> colorFeatures = new Dictionary<string,Photo.colorFeature>();
            foreach (var tag in tags)
            {
                colorFeatures[tag.Name] = new Photo.colorFeature(tag.feature, tag.variance);
                
            }
            return colorFeatures;
        }

        public static string ExportXml(Dictionary<string, Photo.colorFeature> colorFeatures)
        {
            List<TagEntity> list = new List<TagEntity>();
            foreach (var t in colorFeatures.Keys)
            {
                list.Add(new TagEntity(t, colorFeatures[t]));
            }
            XmlSerializer ser = new XmlSerializer(typeof(TagEntity[]));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, list.ToArray());
            return writer.ToString();
        }
        
        //public string[] CapturedTimeStamp;
        //public string[] CreateTimeStamp;
        //public string[] Tags;
        //public string[] Feature;
        //public string Variance;
        
        /*public string artistE = "";
        public string artistJ = "";
        public string titleE = "";
        public string titleJ = "";
        public string birthplace = "";
        public string country = "";
        public string museum = "";*/
        
        // color info
        public Vector3[] feature = null;
        public double variance = 0d;

        //public ArtworksTag()
        //{ }
        public ArtworksTag(List<String> tags, int start, int end)//: base(tags)
        {
            startDate = start;
            endDate = end;
        }

        /*public PhotoLog(string fp)
        {
            //FilePath = fp;
            //string[] fpsplit = fp.Split('\\');
            //FileName = fpsplit[fpsplit.Length - 1];
            //CapturedTimeStamp = null;
            //CreateTimeStamp = null;
            //Tags = null;
            //Feature = null;
            //Variance = "0.0";
        }*/

        /*public static DateTime String2DateTime(string[] ss)
        {
            // on winxp
            //return new DateTime(int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[2]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));
            // on win7
            if (int.Parse(ss[0]) < 1980)
                return new DateTime(int.Parse(ss[2]), int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));
            else
                return new DateTime(int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[2]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));

        }*/

        //public void SetTag(string[] ts)
        //{
        //    int count = ts.Length;
        //    if (count > 0)
        //    {
        //        Tags = new string[count];
        //    }
        //    Tags = ts;
        //}
        /*#region resizeTexture
        public static byte[] ReSizeTexture(GraphicsDevice gd, Texture2D fromt, ref Texture2D tot)
        {
            int mod = 0; // -1:缩小，0:直接（不动），1:扩大
            int w = fromt.Width;
            int h = fromt.Height;
            int xm = Browser.MAXX;
            int ym = Browser.MAXY;
            double xr = (double)w / (double)xm;
            double yr = (double)h / (double)ym;
            if (xr > 1 || yr > 1)
            {
                mod = -1;
            }
            else if (xr < 1 && yr < 1)
            {
                mod = 1;
            }
            int wn = xm;
            int hn = ym;
            if (xr > yr)
            {
                // 高
                hn = (int)((double)h / (double)xr);
                yr = xr;
            }
            else
            {
                // 宽
                wn = (int)((double)w / (double)yr);
                xr = yr;
            }
            double wr = (double)(w - 1) / (double)(wn - 1);
            double hr = (double)(h - 1) / (double)(hn - 1);
            // 画素Data（BGRA顺序）
            int dlen = w * h * 4;
            int dnlen = wn * hn * 4;
            byte[] data = new byte[dlen];
            fromt.GetData(data);
            double[] datat = new double[dnlen];
            byte[] datan = new byte[dnlen];
            int[] num = new int[dnlen];
            for (int i = 0, len = dnlen; i < len; i += 4)
            {
                datat[i] = 0d;
                datat[i + 1] = 0d;
                datat[i + 2] = 0d;
                datat[i + 3] = 255d;
            }
            for (int i = 0, len = dnlen; i < len; ++i)
            {
                num[i] = 0;
            }

            if (mod < 0)
            {
                // 缩小
                for (int i = 0, len = h; i < len; ++i)
                {
                    for (int j = 0, jlen = w; j < jlen; ++j)
                    {
                        double xi = (double)j / wr;
                        double yi = (double)i / hr;
                        int index = (int)(Math.Min(Photo.cod((int)(xi + 0.5d), (int)(yi + 0.5d), wn), dnlen - 1));
                        num[index]++;
                        for (int k = 0, klen = 3; k < klen; ++k)
                        {
                            datat[index + k] += (double)data[Photo.cod(j, i, w) + k];
                        }
                    }
                }
                for (int i = 0, len = dnlen; i < len; ++i)
                {
                    if (num[i] > 0)
                    {
                        for (int k = 0, klen = 3; k < klen; ++k)
                        {
                            datan[i + k] = (byte)(datat[i + k] / (double)num[i]);
                        }
                        datan[i + 3] = (byte)255;
                    }
                }
            }
            else if (mod > 0)
            {
                // 扩大
                for (int i = 0, len = hn; i < len; ++i)
                {
                    for (int j = 0, jlen = wn; j < jlen; ++j)
                    {
                        double x = (double)j * wr;
                        double y = (double)i * hr;
                        int ox = (int)Math.Min(w - 2, (int)x);
                        int oy = (int)Math.Min(h - 2, (int)y);
                        double dx0 = x - ox;
                        double dy0 = y - oy;
                        double dx1 = 1 + ox - x;
                        double dy1 = 1 + oy - y;
                        for (int k = 0, klen = 3; k < klen; ++k)
                        {
                            double datax0 = (data[Photo.cod(ox, oy, w) + k] * dx1 + data[Photo.cod(ox + 1, oy, w) + k] * dx0) / (dx0 + dx1);
                            double data0y = (data[Photo.cod(ox, oy, w) + k] * dy1 + data[Photo.cod(ox, oy + 1, w) + k] * dy0) / (dy0 + dy1);
                            double datax1 = (data[Photo.cod(ox, oy + 1, w) + k] * dx1 + data[Photo.cod(ox + 1, oy + 1, w) + k] * dx0) / (dx0 + dx1);
                            double data1y = (data[Photo.cod(ox + 1, oy, w) + k] * dy1 + data[Photo.cod(ox + 1, oy + 1, w) + k] * dy0) / (dy0 + dy1);
                            datat[Photo.cod(j, i, wn) + k] = ((datax0 * dy1 + datax1 * dy0) / (dy0 + dy1) + (data0y * dx1 + data1y * dx0) / (dx0 + dx1)) * 0.5d;
                            datan[Photo.cod(j, i, wn) + k] = (byte)datat[Photo.cod(j, i, wn) + k];
                        }
                        datan[Photo.cod(j, i, wn) + 3] = (byte)255;
                    }
                }
            }
            else
            {
                datan = data;
            }

            // 应用纹理
            tot = new Texture2D(gd, wn, hn);
            tot.SetData(datan);
            return datan;
        }
        #endregion*/

        // added by Liu Gengdai, 2011-11-16
        //public static string[] FormatTimeStamp(string[] str)
        //{
        //    if (str != null)
        //    {
        //        if (str[0].Length <= 4)
        //            return str;
        //        else
        //        {
        //            string[] strr = str[0].Split('-');                    
        //            int len = strr.Length;
        //            string[] temp = new string[len + str.Length - 1];
        //            for (int i = 0; i < len + str.Length -1; i++ )
        //            {
        //                if (i < len)
        //                    temp[i] = strr[i];
        //                else
        //                    temp[i] = str[i - len +1];
        //            }
                    
        //            return temp;
        //        }
        //    }
        //    else
        //        return null;
        //}

        //public int CompareTo(object obj)
        //{
        //    PhotoLog other = (PhotoLog)obj;

        //    // 根据FileName排序
        //    return this.FileName.CompareTo(other.FileName);
        //}
    }
}
