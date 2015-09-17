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
        bool showTouchPie = false;
        bool oldShowTouchPie = false;
        int touchCount;

        public void update()
        {
            foreach (PointingDevice pd in pointingDevices)
            {
                pd.update();
                if (pd.Type == PointingDevice.DeviceType.Touch)
                    pd.getPieMenu().Mode = touchState;
            }
            oldShowTouchPie = showTouchPie;
        }

        //public enum TouchStateMenu
        //{
        //    DrawLine,
        //    DragPhoto,
        //    DeletePhoto,
        //    MoveLine,
        //    Nothing,
        //}

        public PieMenu.PieMode touchState = PieMenu.PieMode.DragPhoto;
        //{
        //    get;
        //    private set;
        //}
        public void clear()
        {
            pointingDevices.Clear();
        }

        public PointingDevice checkTouchRight()
        {
            touchCount = 0;
            PointingDevice pd = null;
            foreach (PointingDevice pointingDevice in pointingDevices)
            {
                if (pointingDevice.Type == PointingDevice.DeviceType.Touch)
                {
                    pd = pointingDevice;
                    touchCount++;
                }
            }
            if (touchCount > 1)
            {
                if (oldShowTouchPie == false)
                {
                    showTouchPie = true;
                    pd.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    pd.RightDownPosition = pd.Position;
                    return pd;
                }
                else return null;
            }
            else
            {
                showTouchPie = false;
                return null;
            }
        }

        public void initialize()
        {
            foreach (PointingDevice pd in pointingDevices)
            {
                if (pd.Type == PointingDevice.DeviceType.Touch)
                    pd.getPieMenu().Mode = touchState;
            }
        }
        public void add(PointingDevice pd)
        {
            pointingDevices.Add(pd);
            //mouseMenu[pd] = new PieMenu();
        }

        

        public bool remove(PointingDevice pd)
        {
            if (pd.Type == PointingDevice.DeviceType.Touch)
            {
                PieMenu pm = pd.getPieMenu();
                if (pm.IsShown)
                {
                    pd.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    return false;
                }
            }

            pointingDevices.Remove(pd);
            return true;
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
                    Browser.Instance.batch_.Draw(ResourceManager.cursor_, pointingDevice.GamePosition - 24 * Vector2.One, color);
                }
                else
                {
                    Browser.Instance.batch_.Draw(ResourceManager.batsuTex_, pointingDevice.GamePosition - 24 * Vector2.One, Color.White);
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

        //public void removeTouchMap(List<int> touchRemoveList)
        //{
        //    for (int i = 0; i < touchRemoveList.Count; i++)
        //    {
        //        if()
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
