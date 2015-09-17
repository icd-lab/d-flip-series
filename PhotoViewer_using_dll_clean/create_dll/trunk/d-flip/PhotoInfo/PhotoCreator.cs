using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using database;
using dflip.Supplement;
using dflip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using dflip.Element;
using dflip.Manager;
using System.Runtime.InteropServices;

namespace PhotoInfo
{
    class PhotoCreator
    {
        //TableProcessor table;
        //ColorTable colorTable = new ColorTable();
        //public PhotoCreator(TableProcessor t)
        //{
        //    table = t;
        //    photos = new List<Photo>();
        //}

        //加载照片的日志
        private Dictionary<string, PhotoLog> photoLog = new Dictionary<string,PhotoLog>();
        private Dictionary<string, PeopleTags> peopleTags = new Dictionary<string, PeopleTags>();
        private string profilePath;
        public PhotoCreator()
        {
            //ReadPhotoLogs();
            ReadPeopleLogs();
            photos = new List<Photo>();
        }
        // added by Gengdai
        private void ReadPeopleLogs()
        {
            string fullFileName = "people.cfg";
            if (ResourceManager.homeDirectory_ != null)
                fullFileName = ResourceManager.homeDirectory_ + "\\" + fullFileName; 
            //string fullFileName = ResourceManager.homeDirectory_ + tagFileName;
            if (File.Exists(fullFileName))
            {
                //ptags.Clear();

                string photoname;
                string people;
                Microsoft.Xna.Framework.Rectangle box;
                StreamReader sr = new StreamReader(fullFileName, Encoding.GetEncoding("Shift_JIS"));
                string[] sep = new string[1];
                sep[0] = "\r\n";
                string[] tags = (sr.ReadToEnd()).Split(sep, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, len = tags.Length; i < len; i++)
                {
                    List<PeopleTag> list = new List<PeopleTag>();
                    //list.Clear();
                    sep[0] = "|";
                    string[] tag = tags[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    photoname = tag[0];
                    for (int j = 1; j < tag.Length; j++)
                    {
                        sep[0] = ":";
                        string[] subtag = tag[j].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        people = subtag[0];
                        box = new Microsoft.Xna.Framework.Rectangle(int.Parse(subtag[1]), int.Parse(subtag[2]), int.Parse(subtag[3]), int.Parse(subtag[4]));
                        list.Add(new PeopleTag(people, box));
                    }
                    peopleTags[photoname] = new PeopleTags(photoname, list);
                }
            }
        }

        private void ReadPhotoLogs()
        {
            //string exePath = ResourceManager.homeDirectory_;
            //if (System.IO.File.Exists(exePath + "\\profile.ini"))
            if (profilePath == null)
                return;
            if (!System.IO.File.Exists(profilePath))
            {
                MessageBox(new IntPtr(0), "File doesn't exist!", "File Read Error", 0);
                return;
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(profilePath, System.Text.Encoding.GetEncoding("Shift_JIS"));
            string[] sep = new string[1];
            sep[0] = "\r\n";
            string[] logs = (sr.ReadToEnd()).Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (logs.Length > 0)
                ResourceManager.homeDirectory_ = logs[0];
            for (int i = 1, len = logs.Length; i < len; ++i)
            {
                sep[0] = "|";
                PhotoLog p = new PhotoLog();
                string[] log = logs[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                if (log.Length > 0)
                    p.FilePath = log[0];
                if (log.Length > 1)
                    p.FileName = log[1];

                sep[0] = ":";
                //sep[1] = " ";
                bool error = false;
                if (log.Length > 2)
                {
                    string[] seperate = new string[2];
                    seperate[0] = ":";
                    seperate[1] = " ";
                    p.CapturedTimeStamp = log[2].Split(seperate, StringSplitOptions.RemoveEmptyEntries);
                    if (File.Exists(p.FilePath))
                    {
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(p.FilePath);
                        int[] pidList = bmp.PropertyIdList;
                        int index = Array.IndexOf(pidList, 0x9003);
                        if (index > 0)
                        {
                            try
                            {
                                System.Drawing.Imaging.PropertyItem pi = bmp.PropertyItems[index];
                                string cts = Encoding.ASCII.GetString(pi.Value, 0, 19);
                                p.CapturedTimeStamp = cts.Split(seperate, StringSplitOptions.RemoveEmptyEntries);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("photo shotting time read error");
                                MessageBox(new IntPtr(0), "shotting time error!", "Shotting Time Error", 0);
                                error = true;
                            }
                        }
                        bmp.Dispose();
                    }
                }
                if (log.Length > 3)
                {
                    p.CreateTimeStamp = log[3].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    if (error)
                        p.CapturedTimeStamp = p.CreateTimeStamp;
                }
                if (log.Length > 4)
                    p.Tags = log[4].Split(sep, StringSplitOptions.RemoveEmptyEntries);

                if (log.Length > 5)
                {
                    p.Feature = log[5].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    p.Feature = null;
                }
                if (log.Length > 6)
                {
                    p.Variance = log[6];
                }
                else
                {
                    p.Variance = "0.0";
                }
                photoLog[p.FilePath] = p;
            }
            sr.Close();
            //photoLog.Sort();
            
        }

        public void InitialOpen(string profilePath)
        {
            this.profilePath = profilePath;
            if (profilePath == null)
                return;
            ReadPhotoLogs();

            //if start with exe, profilePath has more than one part
            string[] profile = profilePath.Split('\\');
            if (profile.Length > 1)
            {
                createPhoto(photoLog.Keys.ToList(), 0);
            }
            else ResourceManager.homeDirectory_ = null;
        }

        public List<Photo> photos
        {
            get;
            private set;
        }
        private Random random_ = new Random();
        private ProgressBarForm progressBar;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public void createPhoto(List<string> filename, int time)
        {
			if (filename.Count == 0)
				return;

			string[] profile = profilePath.Split ('\\');
            string[] root = null;
            if(ResourceManager.homeDirectory_ != null)
               root  = ResourceManager.homeDirectory_.Split('\\');
			//もしコマンドラインから起きたら、profile.Length > 1。
            if (profile.Length > 1 && time > 0)
            {
                //List<string> paths = new List<string>();
                string[] parts = filename[0].Split('\\');

                if (parts.Length <= root.Length)
                {
                    MessageBox(new IntPtr(0), string.Format("連携モード時は、{0}配下のファイルのみ指定できます。", ResourceManager.homeDirectory_), "Error Reading Photo", 0);
                    return;
                }
                bool flag = false;
                for (int i = 0; i < root.Length; i++)
                {
                    if (!string.Equals(parts[i], root[i], StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    MessageBox(new IntPtr(0), string.Format("連携モード時は、{0}配下のファイルのみ指定できます。", ResourceManager.homeDirectory_), "Error Reading Photo", 0);
                    return;
                }
            }

            //photos = new List<Photo>();
            progressBar = new ProgressBarForm();
            progressBar.Location = new System.Drawing.Point((int)SystemParameter.clientBounds.Min.X, (int)SystemParameter.clientBounds.Min.Y);
           
            progressBar.Begin(filename.Count);
            for(int i = 0; i < filename.Count; i++)
            {

                PhotoTag ptag = new PhotoTag();
                Photo.colorFeature color = new Photo.colorFeature();

                PhotoLog nowLog = new PhotoLog(filename[i]);
                
                string[] namePart = filename[i].Split('\\');
                Texture2D t;
                if (time == 0)
                {
                    root = ResourceManager.homeDirectory_.Split('\\');
                    if (namePart.Length <= root.Length)
                    {
                        MessageBox(new IntPtr(0), string.Format("連携モード時は、{0}配下のファイルのみ指定できます。", ResourceManager.homeDirectory_), "Error Reading Photo", 0);
                        continue;
                    }
                    bool flag = false;
                    for (int j = 0; j < root.Length; j++)
                    {
                        if (!string.Equals(namePart[j], root[j], StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        MessageBox(new IntPtr(0), string.Format("連携モード時は、{0}配下のファイルのみ指定できます。", ResourceManager.homeDirectory_), "Error Reading Photo", 0);
                        continue;
                    }
                }
                if (File.Exists(filename[i]))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(filename[i], FileMode.Open))
                        {
                            t = Texture2D.FromStream(SystemParameter.graphicsDevice, fileStream);
                            //t = new Texture2D(SystemParameter.graphicsDevice, SystemParameter.graphicsDevice.PresentationParameters.BackBufferHeight, SystemParameter.graphicsDevice.PresentationParameters.BackBufferWidth, true, SystemParameter.graphicsDevice.PresentationParameters.BackBufferFormat);
                            //t.SetData<FileStream>(2,
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox(new IntPtr(0), e.ToString(), "Error Reading Photo", 0);
                        continue;
                    }
                }
                else continue;
                Photo newP = null;

                //if tag exists
                if (photoLog.ContainsKey(filename[i]))
                {
                    nowLog = photoLog[filename[i]];
                    
                    ptag.CapturedDate = PhotoLog.String2DateTime(PhotoLog.FormatTimeStamp(nowLog.CapturedTimeStamp));
                    ptag.CreatedDate = PhotoLog.String2DateTime(PhotoLog.FormatTimeStamp(nowLog.CreateTimeStamp));


                    //read tag
                        string[] sepName = new string[1];
                        sepName[0] = "\\";
                        string[] names = filename[i].Split(sepName, StringSplitOptions.RemoveEmptyEntries);
                        string name = names[names.Length - 1];
                        sepName[0] = ",";
                        string[] nameTags = name.Split(sepName, StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 1; j < nameTags.Length - 1; j++)
                        {
                            List<string> tagList;
                            tagList = nowLog.Tags.ToList();
                            if (nowLog.Tags == null || !nowLog.Tags.Contains(nameTags[j]))
                            {
                                tagList.Add(nameTags[j]);
                            }
                            nowLog.Tags = tagList.ToArray();
                        }

                        if (nowLog.Tags != null && nowLog.Tags.Length > 0)
                            ptag.allTags = nowLog.Tags.ToList();
                    
                    if (nowLog.Feature != null)
                    {
                        List<Vector3> f = new List<Vector3>();
                        for (int j = 0, len = nowLog.Feature.Length; j < len; j += 3)
                        {
                            f.Add(new Vector3(float.Parse(nowLog.Feature[j]), float.Parse(nowLog.Feature[j + 1]), float.Parse(nowLog.Feature[j + 2])));
                        }
                        color.feature_ = f.ToArray();
                        color.variance_ = double.Parse(nowLog.Variance);
                    }
                    else
                    {
                        color = Photo.CalcFeature(t);
                        List<string> f = new List<string>();
                        for (int j = 0, len = color.feature_.Length; j < len; ++j)
                        {
                            f.Add(color.feature_[j].X.ToString());
                            f.Add(color.feature_[j].Y.ToString());
                            f.Add(color.feature_[j].Z.ToString());
                        }
                        nowLog.Feature = f.ToArray();
                        nowLog.Variance = color.variance_.ToString();
                        photoLog[nowLog.FilePath] = nowLog;
                    }
                    
                }

                else
                {
                    //file name to tag
                    string[] sepName = new string[1];
                    sepName[0] = "\\";
                    string[] names = filename[i].Split(sepName, StringSplitOptions.RemoveEmptyEntries);
                    string name = names[names.Length - 1];
                    sepName[0] = ",";
                    string[] nameTags = name.Split(sepName, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 1; j < nameTags.Length - 1; j++)
                    {
                        List<string> tagList;
                        tagList = nowLog.Tags.ToList();
                        if (nowLog.Tags == null || !nowLog.Tags.Contains(nameTags[j]))
                        {
                            tagList.Add(nameTags[j]);
                        }
                            nowLog.Tags = tagList.ToArray();
                    }

                    if (nowLog.Tags != null && nowLog.Tags.Length > 0)
                        ptag.allTags = nowLog.Tags.ToList();

                    //char[] sep = { '/', ' ', ':'};
                    char[] sep = { '/', ' ', ':', '-' }; // modified by Gengdai, 2011-11-9
                    string nowtime = DateTime.Now.ToString();
                    nowLog.CreateTimeStamp = nowtime.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename[i]);
                    int[] pidList = bmp.PropertyIdList;
                    int index = Array.IndexOf(pidList, 0x9003);
                    if (index == -1)
                    {
                        DateTime WriteTimeStamp = File.GetLastWriteTime(filename[i]);
                        if (DateTime.Compare(WriteTimeStamp, PhotoLog.String2DateTime(nowLog.CreateTimeStamp)) < 0)
                        {
                            nowLog.CapturedTimeStamp = (WriteTimeStamp.ToString()).Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            nowLog.CapturedTimeStamp = nowLog.CreateTimeStamp;
                        }
                    }
                    else
                    {
                        try
                        {
                            System.Drawing.Imaging.PropertyItem pi = bmp.PropertyItems[index];
                            string cts = Encoding.ASCII.GetString(pi.Value, 0, 19);
                            nowLog.CapturedTimeStamp = cts.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("read capture time error");
                            MessageBox(new IntPtr(0), "capture time read error!", "Capture Time Error", 0);
                            nowLog.CapturedTimeStamp = nowLog.CreateTimeStamp;
                        }
                    }
                    bmp.Dispose();

                    ptag.CapturedDate = PhotoLog.String2DateTime(nowLog.CapturedTimeStamp);
                    ptag.CreatedDate = PhotoLog.String2DateTime(nowLog.CreateTimeStamp);

                    //calculate color feature
                    color = Photo.CalcFeature(t);
                    List<string> f = new List<string>();
                    for (int j = 0, len = color.feature_.Length; j < len; ++j)
                    {
                        f.Add(color.feature_[j].X.ToString());
                        f.Add(color.feature_[j].Y.ToString());
                        f.Add(color.feature_[j].Z.ToString());
                    }
                    nowLog.Feature = f.ToArray();
                    nowLog.Variance = color.variance_.ToString();
                    photoLog[nowLog.FilePath] = nowLog;

                }

                
                //problem: two images cannot have the same filename
                
                newP = new Photo(i, filename[i], new Vector2(
                        (float)(random_.NextDouble() * SystemParameter.ClientWidth),
                        (float)(random_.NextDouble() * SystemParameter.ClientHeight)), (float)(random_.NextDouble() * 0.1) + 0.25f, 0f, ptag, t, color);
                
                photos.Add(newP);

                // people tag: added by Gengdai
                if (peopleTags.ContainsKey(filename[i]))
                {
                    var p = peopleTags[filename[i]];
                    newP.PTags = p;

                    // add tags directly
                    foreach (PeopleTag pt in p.pTags)
                    {
                        newP.ptag.allTags.Add(pt.People);
                    }

                }
                
                //photos[photos.Count - 1].SetTexture(t);

                newP.Center = new Vector2(t.Width, t.Height) * 0.5f;
                newP.BoundingBox = new BoundingBox2D(newP.Position - newP.Center * newP.Scale - Vector2.One * (float)ResourceManager.MAR, newP.Position + newP.Center * newP.Scale + Vector2.One * (float)ResourceManager.MAR, newP.Angle);

                progressBar.ProgressName();
            }
            progressBar.End();

        }

        public void UnloadPhoto()
        {
            foreach (Photo photo in photos)
            {
                photo.Unload();
            }
        }

        public void UnloadPeopleTag()
        {
            //foreach (PeopleTags pt in peopleTags)
            //{
            //    pt.Release();
            //}
        }

        ~PhotoCreator()
        {
            savePhotoLogs();
        }

        public void addTag(string filePath, List<string> tags)
        {
            photoLog[filePath].Tags = photoLog[filePath].Tags.Concat(tags).ToArray();
        }

        public void removeTag(string filePath, List<string> tags)
        {
            photoLog[filePath].Tags = photoLog[filePath].Tags.Except(tags).ToArray();
        }

        private void savePhotoLogs()
        {
            if (profilePath == null)
                return;
            //string profilePath = ResourceManager.homeDirectory_;
            if (System.IO.File.Exists(profilePath))// + "\\profile.ini"))
            {
                System.IO.File.Delete(profilePath);// + "\\profile.ini");
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(profilePath, false, System.Text.Encoding.GetEncoding("Shift_JIS"));
            sw.Write(ResourceManager.homeDirectory_ + "\r\n");
            foreach (PhotoLog p in photoLog.Values)
            {
                sw.Write(p.FilePath + "|");
                sw.Write(p.FileName + "|");
                for (int i = 0, len = p.CapturedTimeStamp.Length; i < len; ++i)
                {
                    sw.Write(p.CapturedTimeStamp[i] + ":");
                }
                sw.Write("|");
                for (int i = 0, len = p.CreateTimeStamp.Length; i < len; ++i)
                {
                    sw.Write(p.CreateTimeStamp[i] + ":");
                }
                sw.Write("|");
                if (p.Tags != null && p.Tags.Length > 0)
                {
                    for (int i = 0, len = p.Tags.Length; i < len; ++i)
                    {
                        sw.Write(p.Tags[i] + ":");
                    }
                }
                else
                {
                    sw.Write("null:");
                }
                sw.Write("|");
                if (p.Feature != null)
                {
                    for (int i = 0, len = p.Feature.Length; i < len; ++i)
                    {
                        sw.Write(p.Feature[i] + ":");
                    }
                    sw.Write("|" + p.Variance.ToString());
                }
               
                sw.Write("\r\n");
            }
            sw.Close();

        }

    }
    
                
                
}
