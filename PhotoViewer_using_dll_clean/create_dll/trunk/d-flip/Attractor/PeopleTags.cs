using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace dflip
{
    //human tag structure, added by Gengdai
    public struct PeopleTag : IComparable
    {
        public string People;
        public Rectangle Box;
        public PeopleTag(string p,  Rectangle b)
        {
            People = p;
            Box = b;
        }
        public int CompareTo(object obj)
        {
            PeopleTag other =( PeopleTag )obj;

            //sort
            return this.People.CompareTo(other.People);
        }
    }

    //human tag list in one photo, added by Gengdai
    public struct PeopleTags : IComparable
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
            if (pTags.Count!=0)
            {
                pTags.Clear();
            }
        }
        
        public int CompareTo(object obj)
        {
            PeopleTags other = (PeopleTags)obj;

            //sort
            return this.FileName.CompareTo(other.FileName);
        }
    }

}
