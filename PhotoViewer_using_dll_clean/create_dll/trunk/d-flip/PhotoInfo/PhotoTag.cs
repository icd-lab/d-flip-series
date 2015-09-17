using System;
using dflip;
using System.Linq;
using System.Collections.Generic;
using dflip.Supplement;
using System.Text;
using dflip.Manager;
using Microsoft.Xna.Framework;

namespace PhotoInfo
{
    public class PhotoTag //: IComparable
    {
        Dictionary<String, BoundingBox2D> tagBox = new Dictionary<string,BoundingBox2D>();

        public DateTime CapturedDate;
        public DateTime CreatedDate;

        public List<String> activeTagList
        {
            get;
            private set;
        }

        public int startDate
        {
            get;
            set;
        }

        public int endDate
        {
            get;
            set;
        }

        public PhotoTag()
        {
            activeTagList = new List<String>();
            allTags = new List<String>();
        }

        public PhotoTag(List<String> tags)
        {
            foreach (String t in tags)
                tagBox[t] = new BoundingBox2D();
            allTags = tags;
            activeTagList = new List<String>();
        }

        public List<String> allTags
        {
            get;
            set;
        }
    }
}
