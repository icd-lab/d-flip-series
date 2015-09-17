using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace dflip.Element
{
    class FileOpenDialog
    {
        System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        //private readonly object monitor_ = new object();
        public List<String> fileNames
        {
            get;
            private set;
        }
        public FileOpenDialog()
        {
            fileNames = new List<string>();
            this.openFileDialog1.Filter = "対応している画像ファイル(*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|すべてのフ" +
                "ァイル(*.*)|*.*";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "開くファイルを選択してください";
            //this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            openFileDialog1.InitialDirectory = "C:\\PhotoViewer";
            //fileNames.Clear();
            if (!Directory.Exists(openFileDialog1.InitialDirectory))
            {
                openFileDialog1.InitialDirectory = Application.StartupPath;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //lock (monitor_)
                {
                    int length = openFileDialog1.FileNames.GetLength(0);

                    for (int i = 0; i < length; i++)
                    {
                        string fn = openFileDialog1.FileNames[i];
                        fileNames.Add(fn);
                    }

                }
            }
        }
    }
}
