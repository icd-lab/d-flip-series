using System;
using System.Collections.Generic;
using PhotoInfo;
using PhotoViewer.Manager;
using Microsoft.Xna.Framework;

namespace PhotoViewer.InputDevice
{
    public class PointingDeviceCollection
    {
        List<PointingDevice> pointingDevices = new List<PointingDevice>();
        //Dictionary<PointingDevice, PieMenu> mouseMenu = new Dictionary<PointingDevice,PieMenu>();
        //Dictionary<PointingDevice, Photo> mousePhoto = new Dictionary<PointingDevice,Photo>();
        int pos = 0;

        public void update()
        {
            foreach (PointingDevice pd in pointingDevices)
                pd.update();
        }

        public void initialize()
        {
            //foreach (PointingDevice pd in pointingDevices)
            //{
            //    //pd.PositionAdd(-Browser.Instance.clientBounds.Min);
            //    pd.RightDownPosition -= Browser.Instance.clientBounds.Min;
            //    pd.LeftDownPosition -= Browser.Instance.clientBounds.Min;
            //    pd.MiddleDownPosition -= Browser.Instance.clientBounds.Min;
            //}
        }
        public void add(PointingDevice pd)
        {
            pointingDevices.Add(pd);
            //mouseMenu[pd] = new PieMenu();
        }

        

        public void remove(PointingDevice pd)
        {
            pointingDevices.Remove(pd);
        }
        public PointingDevice next()
        {
            if (++pos < pointingDevices.Count)
                return pointingDevices[pos];
            return null;
        }

        public PointingDevice first()
        {
            pos = 0;
            if (pointingDevices.Count == 0)
                return null;
            return pointingDevices[0];
        }
        public int count
        {
            get
            {
                return pointingDevices.Count;
            }
        }

        public void drawMouse(Color color)
        {
            foreach (PointingDevice pointingDevice in pointingDevices)
            {
                if (pointingDevice.state == (int)PointingDevice.State.Curosr)
                {
                    SystemParameter.batch_.Draw(ResourceManager.cursor_, pointingDevice.GamePosition - 24 * Vector2.One, color);
                }
                else
                {
                    SystemParameter.batch_.Draw(ResourceManager.batsuTex_, pointingDevice.GamePosition - 24 * Vector2.One, Color.White);
                }
            }
        }

        public void drawPieMenu()
        {
            foreach (PointingDevice pointingDevice in pointingDevices)
                pointingDevice.getPieMenu().Render(pointingDevice.GamePosition);
        }
        //public void update()
        //{
        //    foreach (PointingDevice pd in pointingDevices)
        //    {
        //        pd.PositionAdd(-Browser.Instance.clientBounds.Min);
        //        pd.RightDownPosition -= Browser.Instance.clientBounds.Min;
        //        pd.LeftDownPosition -= Browser.Instance.clientBounds.Min;
        //        pd.MiddleDownPosition -= Browser.Instance.clientBounds.Min;
        //    }
        //}

        public PointingDevice this[int index]
        {
            get 
            {
                if(index < pointingDevices.Count)
                    return pointingDevices[index];
                else return null;
            }
        }


    }
}
