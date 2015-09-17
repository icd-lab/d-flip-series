using System;
using PhotoViewer;
using System.Linq;
using System.Collections.Generic;
using PhotoViewer.Supplement;
using System.Text;
using PhotoViewer.Manager;
using Microsoft.Xna.Framework;

namespace PhotoInfo
{
    public class PhotoTag //: IComparable
    {
        Dictionary<String, BoundingBox2D> tagBox = new Dictionary<string,BoundingBox2D>();
        int left = 2;
        int right = 2;
        int up = 4;
        int width = 250;

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
            allTags.Add("Color");
        }

        private Dictionary<String, int> parseText(String text, int width)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');
            int count = 1;
            Dictionary<String, int> result = new Dictionary<String, int>();
            foreach (String word in wordArray)
            {
                if (ResourceManager.font_.MeasureString(line + word).Length() > width)
                {
                    returnString = returnString + line + '\n';
                    count++;
                    line = String.Empty;
                }

                line = line + word + ' ';
            }
            result[returnString + line] = count;
            return result;
        }

        //left 2; right 2; up & down 4; alignment
        public List<int> drawTags(int startX, int startY, float angleDisplay)
        {
            int YOffset = 0;
            int resultWidth = 0;
            if (allTags.Count > 1)
            {
                foreach (String tag in allTags)
                {
                    Dictionary<String, int> result = parseText(tag, width - left - right);
                    if (result.Values.First() > 1)
                        resultWidth = 250;
                    else if (resultWidth < 250 && resultWidth < ResourceManager.font_.MeasureString(tag).Length() + 5)
                        resultWidth = (int)ResourceManager.font_.MeasureString(tag).Length() + 5;
                    if (activeTagList.Contains(tag))
                    {
                        SystemParameter.batch_.DrawString(ResourceManager.font_, result.Keys.First(), new Vector2(startX + left, startY + YOffset), Color.Red);
                    }
                    else
                        SystemParameter.batch_.DrawString(ResourceManager.font_, result.Keys.First(), new Vector2(startX + left, startY + YOffset), Color.Gray);
                    Vector2 size = ResourceManager.font_.MeasureString(tag);
                    //create bounding box for every tag
                    tagBox[tag] = new BoundingBox2D(new Vector2(startX, startY + YOffset), new Vector2(startX + resultWidth, startY + YOffset + size.Y * result.Values.First()), angleDisplay);
                    YOffset +=(int) size.Y * result.Values.First()+ up;
                }

                //createBox(startX, startY, angleDisplay);
            }
            List<int> maxSize = new List<int>();
            maxSize.Add(resultWidth); maxSize.Add(YOffset);
            return maxSize;
        }
        //return 0: nothing; return 1 turn on/off atractor color
        public bool clickedCheck(Vector2 clickedPosition)
        {
            if (allTags.Count == 1)
            {
                if(!activeTagList.Contains(allTags[0]))
                    activeTagList.Add(allTags[0]);
                return true;
            }
            foreach(String tag in allTags)
                if (tagBox[tag].Contains(clickedPosition) == ContainmentType.Contains)
                {
                    if (activeTagList.Contains(tag))
                    {
                        activeTagList.Remove(tag);
                    }
                    else
                    {
                        activeTagList.Add(tag);
                    }
                    if(tag.Equals("Color"))
                        return true;
                    return false;
                }
            return false;
        }

        public void clearActiveTagList()
        {
            activeTagList.Clear();
        }

        public PhotoTag(List<String> tags)
        {
            tags.Add("Color");
            foreach (String t in tags)
                tagBox[t] = new BoundingBox2D();
            allTags = tags;
            activeTagList = new List<String>();
        }

        //public void createBox(float x, float y, float angle)
        //{
        //    int YOffset = 0;
        //    foreach(String t in allTags)
        //    {
        //        Vector2 size = ResourceManager.font_.MeasureString(t);
        //        tagBox[t] = new BoundingBox2D(new Vector2(x, y + YOffset), new Vector2(x + width, y + YOffset + size.Y), angle);
        //        YOffset += (int)size.Y + up;
        //    }
        //}
        public List<String> allTags
        {
            get;
            set;
        }
    }
}
