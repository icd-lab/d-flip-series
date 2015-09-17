using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dflip.Manager;

namespace PhotoInfo
{
    class PhotoLog//: IComparable
    {

        public string FilePath;
        public string FileName;
        public string[] CapturedTimeStamp;
        public string[] CreateTimeStamp;
        public string[] Tags = new string[]{};
        public string[] Feature;
        public string Variance;

        public PhotoLog(PhotoLog pl)
        {
            FilePath = pl.FilePath;
            FileName = pl.FileName;
            CapturedTimeStamp = pl.CapturedTimeStamp;
            CreateTimeStamp = pl.CreateTimeStamp;
            Tags = pl.Tags;
            Feature = pl.Feature;
            Variance = pl.Variance;
        }

        public PhotoLog()
        { }

        public PhotoLog(string fp)
        {
            FilePath = fp;
            string[] fpsplit = fp.Split('\\');
            FileName = fpsplit[fpsplit.Length - 1];
            CapturedTimeStamp = null;
            CreateTimeStamp = null;
            //Tags = null;
            Feature = null;
            Variance = "0.0";
        }

        public static string[] FormatTimeStamp(string[] str)
        {
            if (str != null)
            {
                if (str[0].Length <= 4)
                    return str;
                else
                {
                    string[] strr = str[0].Split('-');
                    int len = strr.Length;
                    string[] temp = new string[len + str.Length - 1];
                    for (int i = 0; i < len + str.Length - 1; i++)
                    {
                        if (i < len)
                            temp[i] = strr[i];
                        else
                            temp[i] = str[i - len + 1];
                    }

                    return temp;
                }
            }
            else
                return null;
        }

        public static DateTime String2DateTime(string[] ss)
        {
            // on winxp
            //return new DateTime(int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[2]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));
            // on win7
            if (int.Parse(ss[0]) < 1980)
                return new DateTime(int.Parse(ss[2]), int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));
            else
                return new DateTime(int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[2]), int.Parse(ss[3]), int.Parse(ss[4]), int.Parse(ss[5]));

        }

        public void SetTag(string[] ts)
        {
            int count = ts.Length;
            if (count > 0)
            {
                Tags = new string[count];
            }
            Tags = ts;
        }

        //public int CompareTo(object obj)
        //{
        //    PhotoLog other = (PhotoLog)obj;

        //    return this.FileName.CompareTo(other.FileName);
        //}

        
    }
}
