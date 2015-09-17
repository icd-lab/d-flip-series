using System.Collections.Generic;
using PhotoViewer.PhotoInfo;
using PhotoViewer.Element.Dock;
using PhotoViewer.Element.ScrollBar;
using PhotoViewer.Element.StrokeTextbox;

namespace PhotoViewer.Attractor.Manager
{
    public class SystemState
    {
        private static readonly IAttractorSelection[] Attractors =
        {
            new AttractorBound(),
            new AttractorTime(),            
            new AttractorGeograph(),
            new AttractorFrame(),
            new AttractorAvoid(),
            new AttractorAvoidScale(),
            new AttractorTag(),
            new AttractorColor(),
            
            //new AttractorAnchor(),

            new AttractorPeople(),
            new AttractorScaleUpMouse(),
            new AttractorScaleUp(),
            
        };
        

        public const int ATTRACTOR_NONE = 0;
        public const int ATTRACTOR_BOUND = 1;
        public const int ATTRACTOR_TIME = 2;
        public const int ATTRACTOR_GEOGRAPH = 4;
        public const int ATTRACTOR_FRAME = 8;
        public const int ATTRACTOR_AVOID = 16;
        public const int ATTRACTOR_AVOIDSCALE = 32;
        public const int ATTRACTOR_TAG = 64;
        public const int ATTRACTOR_COLOR = 128;
        public const int ATTRACTOR_PEOPLE = 256;
        public const int ATTRACTOR_SCALEUPMOUSE = 512;
        public const int ATTRACTOR_SCALEUP = 1024;
        public const int FILE_OPEN = 8;
        public int curState
        {
            get;
            set;
        }

        // 現在使用しているアトラクターのフラグ
        private int attractor_ = ATTRACTOR_NONE;

        public int Attractor
        {
            get
            {
                return attractor_;
            }
            private set
            {
                attractor_ = value;
            }
        }
        public SystemState()
        {

            attractor_ |= ATTRACTOR_BOUND;
            attractor_ |= ATTRACTOR_AVOID;
            attractor_ |= ATTRACTOR_AVOIDSCALE;
            attractor_ |= ATTRACTOR_SCALEUP;
            //attractor_ |= ATTRACTOR_ANCHOR;
            //selected by mouse, so enlarge
            attractor_ |= ATTRACTOR_SCALEUPMOUSE;
            attractor_ |= ATTRACTOR_TAG;
            attractor_ |= ATTRACTOR_COLOR;
            //attractor_ |= ATTRACTOR_GEOGRAPH;
            attractor_ |= ATTRACTOR_FRAME;
            //attractor_ |= ATTRACTOR_TIME;
            //attractor_ |= ATTRACTOR_PEOPLE;

        }

        public void invokeAttractorSelection(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState)
        {
            for (int i = 0, size = Attractors.Length; i < size; ++i)
            {
                if ((attractor_ & (1 << i)) != 0)
                {
                    Attractors[i].select(dock, sBar, weight, photos, activePhotos, strokes, systemState);
                }
            }
        }

        // 各アトラクター選択の有無と強さ
        public void SwapBound()
        {
            //boundToolStripMenuItem.Checked = !boundToolStripMenuItem.Checked;
            if ((attractor_ & ATTRACTOR_BOUND) == 0)
            {
                attractor_ |= ATTRACTOR_BOUND;
            }
            else
            {
                attractor_ &= ~ATTRACTOR_BOUND;
            }
        }

        public void SwapScaleUp()
        {
            if ((attractor_ & ATTRACTOR_SCALEUP) == 0)
            {
                attractor_ |= ATTRACTOR_SCALEUP;
            }
            else
            {
                attractor_ &= ~ATTRACTOR_SCALEUP;
            }
        }

        public void SwapFrame()
        {
            //frameToolStripMenuItem.Checked = !frameToolStripMenuItem.Checked;
            //if (frameToolStripMenuItem.Checked)
            if((attractor_ & ATTRACTOR_FRAME) == 0)
            {
                attractor_ |= ATTRACTOR_FRAME;
            }
            else
            {
                attractor_ &= ~ATTRACTOR_FRAME;
            }
        }
        
        //private void frameToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    SwapFrame();
        //}

        public void SwapTime()
        {
            //timeToolStripMenuItem.Checked = !timeToolStripMenuItem.Checked;
            //if (timeToolStripMenuItem.Checked)
            if((attractor_ & ATTRACTOR_TIME) == 0)
            {
                attractor_ |= ATTRACTOR_TIME;
                curState = ATTRACTOR_TIME;
            }
            else
            {
                attractor_ &= ~ATTRACTOR_TIME;
                curState = ATTRACTOR_NONE;
            }
        }

        /*public void SwapPeople()
        {
            peopleToolStripMenuItem.Checked = !peopleToolStripMenuItem.Checked;
            if (peopleToolStripMenuItem.Checked)
            {
                attractor_ |= ATTRACTOR_PEOPLE;
            }
            else
                attractor_ &= ~ATTRACTOR_PEOPLE;
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwapTime();
        }*/

        public void SwapGeograph()
        {
            //geographToolStripMenuItem.Checked = !geographToolStripMenuItem.Checked;
            if ((attractor_ & ATTRACTOR_GEOGRAPH) == 0)
            {
                attractor_ |= ATTRACTOR_GEOGRAPH;
                curState = ATTRACTOR_GEOGRAPH;
            }
            else
            {
                attractor_ &= ~ATTRACTOR_GEOGRAPH;
                curState = ATTRACTOR_NONE;
            }
        }

        #region プロパティ群
        /*public MouseMode Mode
        {
            get
            {
                return mode_;
            }
            set
            {
                mode_ = value;
            }
        }
        public List<float> ImagePositionVariations
        {
            get
            {
                return imagesPositionVariations_;
            }
            set
            {
                imagesPositionVariations_ = value;
            }
        }
        public List<float> ImageScaleVariations
        {
            get
            {
                return imagesScaleVariatioins_;
            }
            set
            {
                imagesScaleVariatioins_ = value;
            }
        }*/

        
        
        public bool IsBound
        {
            get
            {
                int temp = attractor_ & SystemState.ATTRACTOR_BOUND;
                return temp == 0 ? false : true;
            }
        }

        public bool IsFrame
        {
            get
            {
                int temp = attractor_ & SystemState.ATTRACTOR_FRAME;
                return temp == 0 ? false : true;
            }
        }
        public bool IsTime
        {
            get
            {
                int temp = attractor_ & SystemState.ATTRACTOR_TIME;
                return temp == 0 ? false : true;
            }
        }
        public bool IsGeograph
        {
            get
            {
                //return geographToolStripMenuItem.Checked;
                int temp = attractor_ & SystemState.ATTRACTOR_GEOGRAPH;
                return temp == 0? false : true;
            }
        }
        
        #endregion
    }
}