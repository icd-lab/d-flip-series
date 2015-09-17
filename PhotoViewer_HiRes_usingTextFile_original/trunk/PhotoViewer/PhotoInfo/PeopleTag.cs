using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhotoInfo
{
    // 人物标记数据结构, added by Gengdai
    public struct PeopleTag //: IComparable
    {
        public string People;
        public Rectangle Box;
        public PeopleTag(string p, Rectangle b)
        {
            People = p;
            Box = b;
        }
        //public int CompareTo(object obj)
        //{
        //    PeopleTag other = (PeopleTag)obj;

        //    // 根据People排序
        //    return this.People.CompareTo(other.People);
        //}
    }

    // 每一幅图片的人物标记列表, added by Gengdai
    public struct PeopleTags //: IComparable
    {
        public string FileName;
        public List<PeopleTag> pTags;
        public PeopleTags(string f, List<PeopleTag> l)
        {
            FileName = f;
            pTags = l;
        }
        public PeopleTags(string f)
        {
            FileName = f;
            pTags = new List<PeopleTag>();
        }
        public void Release()
        {
            if (pTags.Count != 0)
            {
                pTags.Clear();
            }
        }

        //public int CompareTo(object obj)
        //{
        //    PeopleTags other = (PeopleTags)obj;

        //    // 根据FileName排序
        //    return this.FileName.CompareTo(other.FileName);
        //}
    }
}
