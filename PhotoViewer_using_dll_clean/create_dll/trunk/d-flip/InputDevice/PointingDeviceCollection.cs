using System;
using System.Collections.Generic;
using PhotoInfo;
using dflip.Manager;
using Microsoft.Xna.Framework;

namespace dflip.InputDevice
{
    public class PointingDeviceCollection
    {
        List<PointingDevice> pointingDevices = new List<PointingDevice>();
        int pos = 0;

        public void update()
        {
            foreach (PointingDevice pd in pointingDevices)
            {
                pd.update();
                if (pd.Type == PointingDevice.DeviceType.Touch)
                    pd.getPieMenu().Mode = touchState;
            }
            //oldShowTouchPie = showTouchPie;
        }


        //public void checkTouchRight()
        //{
        //    PointingDevice pd = null;
        //    //if showTouchPie property is true, then don't count its left button's state
        //    for (int i = 0; i < pointingDevices.Count; i++)
        //    {
        //        //maybe this constitudes part of the touch right menu
        //        if (pointingDevices[i].showTouchPie)
        //            continue;
        //        if (pointingDevices[i].Type == PointingDevice.DeviceType.Touch)
        //        {
        //            pd = pointingDevices[i];
        //            bool flag = false;
        //            int j = i + 1;
        //            for (; j < pointingDevices.Count; j++)
        //            {
        //                float x = pd.GamePosition.X - pointingDevices[j].GamePosition.X;
        //                float y = pd.GamePosition.Y - pointingDevices[j].GamePosition.Y;
        //                if (x < 200 && x > -200 && y < 200 && y > -200)
        //                {
        //                    flag = true;
        //                    break;
        //                }
        //            }
        //            if (flag)
        //            {
        //                pointingDevices[i].rightTouchCount++;
        //                if (!pd.showTouchPie && pointingDevices[i].rightTouchCount > 100)
        //                {
        //                    pd.showTouchPie = true;
        //                    pointingDevices[j].showTouchPie = true;
        //                    pd.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        //                    pd.RightDownPosition = pd.Position;
        //                }
        //                else if (pd.rightTouchCount <= 100)
        //                    pd.showTouchPie = false;
        //                //if right menu appears for a long time without change, just cancel its appearance
        //                else if (pd.showTouchPie)
        //                {
        //                    pd.rightMenuAppearCount++;
        //                    if (pd.rightMenuAppearCount > 300)
        //                    {
        //                        pd.rightMenuAppearCount = 0;
        //                        pd.showTouchPie = false;
        //                        pointingDevices[j].showTouchPie = false;
        //                        pd.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
        //                    }
        //                }
        //            }
        //            else {
        //                pd.rightTouchCount = 0;
        //                pd.showTouchPie = false;
        //            }
        //        }
        //    }
        //}

        public PieMenu.PieMode touchState = PieMenu.PieMode.DragPhoto;
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
                //if(pd.countTouchTime < 50 && pd.getPieMenu().Mode == PieMenu.PieMode.MoveLine)
                    
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
