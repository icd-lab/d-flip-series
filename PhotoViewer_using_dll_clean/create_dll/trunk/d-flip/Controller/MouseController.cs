using System;
using System.Collections.Generic;
using dflip.InputDevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoInfo;
using System.Linq;
using System.Diagnostics;
using dflip.Element;
using dflip.Manager;

namespace dflip.InputDevice
{
    class MouseController
    {
        private Dock dock;
        private ScrollBar sBar;
        PointingDeviceCollection pdCollection;
        private Dictionary<int, ScrollBar.DragRegion> barDrag = new Dictionary<int, ScrollBar.DragRegion>();
        private Dictionary<int, Stroke> strokeMouse = new Dictionary<int, Stroke>();

        private List<Photo> photos = new List<Photo>();
        
        PhotoCreator createPhoto = new PhotoCreator();
        SystemState systemState;
        PhotoDisplay photoDisplayManager;
        StrokeBoxCollection strokeGroup;
        KeyboardDevice keyboard;
        //List<Stroke> strokes;
        Dictionary<int, Photo> mousePhotoConnection = new Dictionary<int, Photo>();
        //public void Dispose()
        //{
            
        //}
        public MouseController(PointingDeviceCollection pd, Dock d, ScrollBar sB, SystemState st, PhotoDisplay photoDisplay, StrokeBoxCollection s, KeyboardDevice key)
        {
            dock = d;
            sBar = sB;
            //pdCollection = pdc;
            strokeGroup = s;
            systemState = st;
            photoDisplayManager = photoDisplay;
            //rawInputForm_ = new RawInputForm();
            pdCollection = pd;
            keyboard = key;
        }

        bool mouseOnMenu(PointingDevice pd)
        {
            // icon, piemenu, controlpanel, tagform 这些， 鼠标是否在其上
            foreach (Icon icon in dock.Icons)
            {
                icon.tooltip.IsDel = true;
                if (icon.BoundingBox.Contains(pd.GamePosition) == ContainmentType.Contains)
                {
                    iconMouse(icon, pd);
                    return true;
                }
            }
            
            if (systemState.curState == SystemState.ATTRACTOR_TIME)
            {
                if ((barDrag.ContainsKey(pd.Header) && barDrag[pd.Header] != ScrollBar.DragRegion.NONE) || sBar.BoundingBox.Contains(pd.GamePosition) == ContainmentType.Contains)
                {
                    mouseBar(pd);
                    return true;
                }
            }

            if(strokeGroup.underMouse(pd))
                return true;
            //foreach (var tb in ftBoxes)
            //{
            //    if (tb.boundingBox.Contains(pd.GamePosition) == ContainmentType.Contains)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        public void InitialOpen(string profilePath)
        {
            createPhoto.InitialOpen(profilePath);
            photos = createPhoto.photos;
            photoDisplayManager.PhotosToShow(photos);
            strokeGroup.photos = photos;
            strokeGroup.recalPhoto();
        }

        private void mouseBar(PointingDevice pd)
        {
            if (pd.LeftButton == ButtonState.Pressed)
            {
                Vector2 pos = pd.GamePosition;
                Vector2 posOld = pd.OldGamePosition;
                Vector2 v = pos - posOld;
                
                if (pd.oldLeftButton == ButtonState.Released)
                {
                    if (sBar.LeftContains(pos) == ContainmentType.Contains)
                    {
                        barDrag[pd.Header] = ScrollBar.DragRegion.LEFT;
                        
                    }
                    else if (sBar.RightContains(pos) == ContainmentType.Contains)
                    {
                        barDrag[pd.Header] = ScrollBar.DragRegion.RIGHT;
                        
                    }
                    else if (sBar.CenterContains(pos) == ContainmentType.Contains)
                    {
                        barDrag[pd.Header] = ScrollBar.DragRegion.CENTER;
                        
                    }
                    else
                    {
                        barDrag[pd.Header] = ScrollBar.DragRegion.NONE;
                    }
                }
                else
                {
                    if(barDrag.ContainsKey(pd.Header))
                    {
                        ScrollBar.DragRegion region = barDrag[pd.Header];
                        if(region == ScrollBar.DragRegion.LEFT)
                            sBar.MoveBar((int)pos.X, sBar.Max);
                        else if(region == ScrollBar.DragRegion.RIGHT)
                            sBar.MoveBar(sBar.Min, (int)pos.X);
                        else if (region == ScrollBar.DragRegion.CENTER)
                        {
                            if ((pos.X > sBar.Min && pos.X < sBar.Max) || (posOld.X > sBar.Min && posOld.X < sBar.Max))
                            {
                                sBar.MoveBar(sBar.Min + (int)v.X, sBar.Max + (int)v.X);
                            }
                        }
                    }
                }
                
            }
            else barDrag[pd.Header] = ScrollBar.DragRegion.NONE;
        }

        private void mousePhoto(PointingDevice pd)
        {
            if (pd.LeftButton == ButtonState.Released && pd.oldLeftButton == ButtonState.Pressed)
            {
                //Console.WriteLine("ds");
                if (mousePhotoConnection.ContainsKey(pd.Header))
                {
                    //Console.WriteLine(pd.oldLeftButton + i++);
                    Photo photo = mousePhotoConnection[pd.Header];
                    photo.underMouse = true;
                    photo.PositionDisplay = pd.GamePosition + photo.StartDragPosition * photo.ScaleDisplay;
                    photo.Position = photo.PositionDisplay;
                    //Console.WriteLine(photo.Position);
                    photo.SetDisplayTarget();
                    mousePhotoConnection.Remove(pd.Header);
                    List<string> newTags = strokeGroup.checkInside(photo.Position, photo);
                    if (newTags.Count > 0)
                    {
                        photo.ptag.allTags.AddRange(newTags);
                        createPhoto.addTag(photo.FileName, newTags);
                    }
                    List<string> removeTags = strokeGroup.checkOutside(photo.Position, photo);
                    if (removeTags.Count > 0)
                    {
                        photo.ptag.allTags = new List<string>(photo.ptag.allTags.Except(removeTags));
                        createPhoto.removeTag(photo.FileName, removeTags);
                    }
                }
            }

            else if (pd.LeftButton == ButtonState.Pressed && mousePhotoConnection.ContainsKey(pd.Header))
            {
                Photo photo = mousePhotoConnection[pd.Header];
                
                photo.underMouse = true;
                photo.IsClicked = true;
                photo.PositionDisplay = pd.GamePosition + photo.StartDragPosition * photo.ScaleDisplay;
                photo.Position = photo.PositionDisplay;
                //Console.WriteLine(photo.Position);
                photo.SetDisplayTarget();
                if(pd.Type == PointingDevice.DeviceType.Touch)
                    photo.touchCount = 1;
            }

            //指点到图像上并拽动鼠标
            else
            foreach (Photo photo in photos)
            {
                if (photo.BoundingBoxDisplay.Contains(pd.GamePosition) == ContainmentType.Contains)
                {
                    photo.underMouse = true;
                    //photo.DistanceToMouse = pd.GamePosition - photo.Position ;
                    //Console.WriteLine(photo.IsGazeds);
                    if (pd.LeftButton == ButtonState.Pressed && pd.oldLeftButton == ButtonState.Released)
                    {
                        photo.KeepGazed();
                        photo.IsClicked = true;
                        photo.ClickedPoint = photo.BoundingBoxDisplay.Offset(pd.GamePosition);
                        Console.WriteLine(photo.ClickedPoint);
                        Console.WriteLine(pd.GamePosition);
                        photo.StartDragPosition = (photo.PositionDisplay - pd.GamePosition)/photo.ScaleDisplay;
                        photo.Position = photo.PositionDisplay;
                        //photo.tagClickedCheck(pd.GamePosition);
                            //systemState.SwapColor();
                        mousePhotoConnection[pd.Header] = photo;
                        if (pd.Type == PointingDevice.DeviceType.Touch)
                        {
                            photo.touchCount = 1;
                        }
                    }
                    
                    break;
                }

            }
        }

        public void check()
        {
            foreach (Photo p in photos)
                p.prepare();
            //PointingDevice touchPie = 
                //pdCollection.checkTouchRight();
            for (PointingDevice pointingDevice = pdCollection.first(); pointingDevice != null; pointingDevice = pdCollection.next())
            {
                // first check if mouse on menus
                //second check if right button clicked so under change mouse state mode. only right pressed
                //third different mode invoke different method.
                if (mouseOnMenu(pointingDevice))
                    continue;
                PieMenu pm = pointingDevice.getPieMenu();
                if (pointingDevice.RightButton == ButtonState.Pressed && pointingDevice.oldRightButton == ButtonState.Released && !keyboard.ctrlKey)
                {
                    pm.Show(pointingDevice.GameRightDownPosition);
                }
                //else if(pointingDevice.RightButton == ButtonState.Pressed)
                //{
                //    //pm.Render(pointingDevice.GamePosition);
                //}
                else if (pointingDevice.RightButton == ButtonState.Released && pointingDevice.oldRightButton == ButtonState.Pressed && !keyboard.ctrlKey)
                {
                    pointingDevice.state = (int)PointingDevice.State.Curosr;
                    if (pm.IsShown)
                    {
                        PieMenu.PieMode m = pm.ChangeMode(pointingDevice.GamePosition);
                        switch (m)
                        {
                            case PieMenu.PieMode.DragPhoto:
                            case PieMenu.PieMode.DeletePhoto:
                            case PieMenu.PieMode.DrawLine:
                            case PieMenu.PieMode.MoveLine:
                                pm.Mode = m;
                                if (pointingDevice.Type == PointingDevice.DeviceType.Touch)
                                    pdCollection.touchState = m;
                                break;
                            case PieMenu.PieMode.TimeScroll:
                                //pm.Mode = m;
                                if (systemState.curState == SystemState.ATTRACTOR_GEOGRAPH)
                                {
                                    systemState.SwapGeograph();
                                    systemState.SwapScaleUp();
                                    foreach (Icon ic in dock.Icons)
                                    {
                                        if (ic.Attractor == SystemState.ATTRACTOR_GEOGRAPH)
                                        {
                                            ic.IsOn = !ic.IsOn;
                                            break;
                                        }
                                    }

                                }
                                systemState.SwapTime();
                                foreach (Icon ic in dock.Icons)
                                {
                                    if (ic.Attractor == SystemState.ATTRACTOR_TIME)
                                    {
                                        ic.IsOn = !ic.IsOn;
                                        break;
                                    }
                                }
                                break;
                            case PieMenu.PieMode.Geotag:
                                //pm.Mode = m;
                                if (systemState.curState == SystemState.ATTRACTOR_TIME)
                                {
                                    systemState.SwapTime();
                                    foreach (Icon ic in dock.Icons)
                                    {
                                        if (ic.Attractor == SystemState.ATTRACTOR_TIME)
                                        {
                                            ic.IsOn = !ic.IsOn;
                                            break;
                                        }
                                    }
                                }
                                //if it isn't map mode, turn on, if it is and under tohoku mode, turn to worldmap
                                if (systemState.curState != SystemState.ATTRACTOR_GEOGRAPH)
                                {
                                    systemState.SwapGeograph();
                                    systemState.SwapScaleUp();
                                    ResourceManager.IfTohoku = false;
                                }
                                else
                                {
                                    if (ResourceManager.IfTohoku)
                                    {
                                        foreach (Icon ic in dock.Icons)
                                        {
                                            if (ic.Attractor == SystemState.TOHOKU_MAP)
                                            {
                                                ic.IsOn = !ic.IsOn;
                                                break;
                                            }
                                        }
                                        ResourceManager.IfTohoku = false;
                                    }
                                    else
                                    {
                                        systemState.SwapGeograph();
                                        systemState.SwapScaleUp();
                                    }
                                }

                                foreach (Icon ic in dock.Icons)
                                {
                                    if (ic.Attractor == SystemState.ATTRACTOR_GEOGRAPH)
                                    {
                                        ic.IsOn = !ic.IsOn;
                                    }
                                }
                                break;
                            default: break;
                        }
                        pm.Hide();
                    }
                }
                else if (pointingDevice.showTouchPie)
                {
                    if (strokeUnderDrawing.ContainsKey(pointingDevice))
                    {
                        strokeGroup.remove(strokeUnderDrawing[pointingDevice]);
                        strokeUnderDrawing.Remove(pointingDevice);
                    }
                    continue;
                }
                else
                    switch (pm.Mode)
                    {
                        case PieMenu.PieMode.DeletePhoto:
                            deletePhoto(pointingDevice); break;
                        case PieMenu.PieMode.DragPhoto:
                            mousePhoto(pointingDevice); break;
                        case PieMenu.PieMode.DrawLine:
                            drawLine(pointingDevice); break;
                        case PieMenu.PieMode.MoveLine:
                            moveLine(pointingDevice); break;
                        default: break;
                    }
            }
            foreach (Photo p in photos)
                p.postProcess();
            pdCollection.update();
        }

        private void deletePhoto(PointingDevice pd)
        {
            if (pd.LeftButton == ButtonState.Pressed && pd.oldLeftButton == ButtonState.Released)
            {
                foreach (Photo p in photos)
                {
                    if (p.boundingBox_.Contains(pd.GamePosition) == ContainmentType.Contains)
                    {
                        p.IsDel = true;
                        break;
                    }
                }
                //if (pd.Type == PointingDevice.DeviceType.Touch)
                //{
                //    List<Stroke> s = strokeGroup.strokeList;
                //    for (int j = 0; j < s.Count; ++j)
                //    {
                //        if (s[j].touchDelete(pd))
                //        {
                //            pd.state = (int)PointingDevice.State.Cross;
                //            strokeGroup.remove(s[j]);
                //        }
                //        else
                //        {
                //            pd.state = (int)PointingDevice.State.Curosr;
                //        }
                //    }
                //}
            }
        }

        Dictionary<PointingDevice, Stroke> strokeUnderDrawing = new Dictionary<PointingDevice, Stroke>();

        void drawLine(PointingDevice pd)
        {
            if (photos.Count == 0)
                return;
            if (pd.showTouchPie && strokeUnderDrawing.ContainsKey(pd))
            {
                strokeUnderDrawing.Remove(pd);
                return;
            }
            if (pd.RightButton == ButtonState.Pressed && pd.oldRightButton == ButtonState.Released && keyboard.ctrlKey)
            {
                pd.state = (int)PointingDevice.State.Cross;
                List<Stroke> s = strokeGroup.strokeList;
                for (int j = 0; j < s.Count; ++j)
                {
                    if (s[j].IsInternal(pd.GamePosition, s[j].Strokes))
                    {
                        strokeGroup.remove(s[j]);
                    }
                }
                return;
            }

            if (pd.RightButton == ButtonState.Released && pd.oldRightButton == ButtonState.Pressed)
            {
                pd.state = (int)PointingDevice.State.Curosr;
                return;
            }

            if (pd.LeftButton == ButtonState.Pressed && pd.oldLeftButton == ButtonState.Released)
            {
                //press left button, add stroke
                if (strokeUnderDrawing.ContainsKey(pd))
                {
                    strokeGroup.remove(strokeUnderDrawing[pd]);
                    strokeUnderDrawing.Remove(pd);
                }
                strokeUnderDrawing[pd] = strokeGroup.createStroke(pd.GamePosition);
                
            }
            else if (pd.LeftButton == ButtonState.Pressed && strokeUnderDrawing.ContainsKey(pd))
            {
                Stroke s = strokeUnderDrawing[pd];
                if ((s.Last - pd.GamePosition).Length() > Stroke.StrokeTh)
                {
                    // drag and draw
                    s.AddStroke(pd.GamePosition);
                }
            }

            else if(pd.LeftButton == ButtonState.Released && pd.oldLeftButton == ButtonState.Pressed)
            {
                if (strokeUnderDrawing.ContainsKey(pd))
                {
                    Stroke s = strokeUnderDrawing[pd];
                    if (!s.IsClosed)
                    {
                        if (s.Strokes.Count > 2)
                        {
                            //if >= 3 points
                            s.End();
                            //pdCollection.touchState = PieMenu.PieMode.MoveLine;
                            //create text box
                            strokeGroup.createTextBox(pd.GamePosition, s);
                        }
                        else
                        {
                            //less than 3 points
                            strokeGroup.remove(strokeUnderDrawing[pd]);
                        }
                    }
                    strokeUnderDrawing.Remove(pd);
                }
                
            }
        }
        void moveLine(PointingDevice pd)
        {
            if (pd.RightButton == ButtonState.Pressed && pd.oldRightButton == ButtonState.Released && keyboard.ctrlKey)
            {
                pd.state = (int)PointingDevice.State.Cross;
                List<Stroke> s = strokeGroup.strokeList;
                for (int j = 0; j < s.Count; ++j)
                {
                    if (s[j].IsInternal(pd.GamePosition, s[j].Strokes))
                    {
                        strokeGroup.remove(s[j]);
                    }
                }
                return;
            }
            if (pd.RightButton == ButtonState.Released && pd.oldRightButton == ButtonState.Pressed)
            {
                pd.state = (int)PointingDevice.State.Curosr;
                return;
            }
            if (strokeMouse.ContainsKey(pd.Header))
            {
                if (strokeMouse[pd.Header].Interact(pd) == (int)Stroke.StrokeState.NOTHING)
                {
                    strokeGroup.moveTextBox(strokeMouse[pd.Header]);
                    strokeMouse.Remove(pd.Header);
                }
            }
            else
            {
                List<Stroke> s = strokeGroup.strokeList;
                for (int j = 0; j < s.Count; ++j)
                {
                    if (s[j].Interact(pd) != (int)Stroke.StrokeState.NOTHING)
                    {
                        strokeMouse[pd.Header] = s[j];
                        break;
                    }
                }
            }
            
        }

        void iconMouse(Icon icon, PointingDevice pd)
        {
            //show tips
            icon.tooltip.IsDel = false;

            if (pd.LeftButton == ButtonState.Pressed && pd.oldLeftButton == ButtonState.Released)
            {
                if (icon.Attractor == SystemState.ATTRACTOR_BOUND)
                {
                    systemState.SwapBound();
                    icon.IsOn = !icon.IsOn;
                }
                //else if (icon.Attractor == SystemState.ATTRACTOR_SCALEUP)
                //{
                //    systemState.SwapScaleUp();
                //    icon.IsOn = !icon.IsOn;
                //}
                else if (icon.Attractor == SystemState.ATTRACTOR_TIME)
                {
                    if (systemState.curState == SystemState.ATTRACTOR_GEOGRAPH)
                    {
                        systemState.SwapGeograph();
                        systemState.SwapScaleUp();
                        foreach (Icon ic in dock.Icons)
                        {
                            if (ic.Attractor == SystemState.ATTRACTOR_GEOGRAPH && ic.IsOn)
                            {
                                ic.IsOn = !ic.IsOn;
                                break;
                            }
                            if (ic.Attractor == SystemState.TOHOKU_MAP && ic.IsOn)
                            {
                                ic.IsOn = !ic.IsOn;
                                break;
                            }
                        }

                    }
                    systemState.SwapTime();
                    icon.IsOn = !icon.IsOn;
                }
                else if (icon.Attractor == SystemState.ATTRACTOR_GEOGRAPH)
                {
                    if (systemState.curState == SystemState.ATTRACTOR_TIME)
                    {
                        systemState.SwapTime();
                        foreach (Icon ic in dock.Icons)
                        {
                            if (ic.Attractor == SystemState.ATTRACTOR_TIME)
                            {
                                ic.IsOn = !ic.IsOn;
                                break;
                            }
                        }
                    }
                    icon.IsOn = !icon.IsOn;
                    if (icon.IsOn)
                    {
                        ResourceManager.IfTohoku = false;
                        if (systemState.curState != SystemState.ATTRACTOR_GEOGRAPH)
                        {
                            systemState.SwapGeograph();
                            systemState.SwapScaleUp();
                        }
                        else
                        {
                            foreach (Icon ic in dock.Icons)
                            {
                                if (ic.Attractor == SystemState.TOHOKU_MAP)
                                {
                                    ic.IsOn = !ic.IsOn;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        systemState.SwapGeograph();
                        systemState.SwapScaleUp();
                    }
                    
                    
                }
                //tohoku map
                else if (icon.Attractor == SystemState.TOHOKU_MAP)
                {
                    if (systemState.curState == SystemState.ATTRACTOR_TIME)
                    {
                        systemState.SwapTime();
                        foreach (Icon ic in dock.Icons)
                        {
                            if (ic.Attractor == SystemState.ATTRACTOR_TIME)
                            {
                                ic.IsOn = !ic.IsOn;
                                break;
                            }
                        }
                    }
                    icon.IsOn = !icon.IsOn;
                    if (icon.IsOn)
                    {
                        ResourceManager.IfTohoku = true;
                        if (systemState.curState != SystemState.ATTRACTOR_GEOGRAPH)
                        {
                            systemState.SwapGeograph();
                            systemState.SwapScaleUp();
                        }
                        else
                        {
                            foreach (Icon ic in dock.Icons)
                            {
                                if (ic.Attractor == SystemState.ATTRACTOR_GEOGRAPH)
                                {
                                    ic.IsOn = !ic.IsOn;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        systemState.SwapGeograph();
                        systemState.SwapScaleUp();
                    }
                    
                }
                else if (icon.Attractor == SystemState.FILE_OPEN)
                {
                    FileOpenDialog dialog = new FileOpenDialog();
                    if (dialog.fileNames.Count > 0)
                    {
                        //photos_.Clear();
                        createPhoto.createPhoto(dialog.fileNames, 1);
                        //foreach(Photo p in createPhoto.photos)
                        //    photos[p.ID] = p;
                        photos = createPhoto.photos;
                        photoDisplayManager.PhotosToShow(photos);
                        strokeGroup.photos = photos;
                        strokeGroup.recalPhoto();
                    }
                    //icon.IsOn = !icon.IsOn;

                }
                
                else if (icon.Attractor == SystemState.MOVE)
                {
                    pdCollection.touchState = PieMenu.PieMode.DragPhoto;
                    dock.touchState = (pdCollection.touchState);
                }
                else if (icon.Attractor == SystemState.QUERY)
                {
                    pdCollection.touchState = PieMenu.PieMode.DrawLine;
                    dock.touchState = (pdCollection.touchState);
                }
                else if (icon.Attractor == SystemState.TRANSFORM)
                {
                    pdCollection.touchState = PieMenu.PieMode.MoveLine;
                    dock.touchState = (pdCollection.touchState);
                }
                else if (icon.Attractor == SystemState.DELETE)
                {
                    pdCollection.touchState = PieMenu.PieMode.DeletePhoto;
                    dock.touchState = (pdCollection.touchState);
                }
            }
        }

        public void resetDock()
        {
            dock.State = Dock.DockState.closing;
        }

        public void trigerDock()
        {
            if (Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                if (dock.State == Dock.DockState.hide)
                    dock.State = Dock.DockState.opening;
            }
            else
            {
                bool flag = false;
                for (PointingDevice pointingDevice = pdCollection.first(); pointingDevice != null; pointingDevice = pdCollection.next())
                {
                    if (pointingDevice.GamePosition.X <= dock.ShowThreshold + 15)
                    {
                        //if (pointingDevice.LeftButton == ButtonState.Released && pointingDevice.RightButton == ButtonState.Released)
                        {
                            if (dock.State == Dock.DockState.hide || dock.State == Dock.DockState.closing)
                                dock.State = Dock.DockState.opening;
                            //flag = true;
                        }
                        dock.displayTime = 0;
                    }
                    if (pointingDevice.GamePosition.X < dock.HideThreshold)
                    {
                        //if (dock.State == Dock.DockState.show || dock.State == Dock.DockState.opening)
                        //{
                        //    dock.State = Dock.DockState.closing;
                        //}
                        flag = true;
                    }
                }
                if (!flag)
                    if (dock.State == Dock.DockState.show || dock.State == Dock.DockState.opening)
                    {
                        dock.displayTime++;
                        if (dock.displayTime > 200)
                            dock.State = Dock.DockState.closing;
                    }

            }
        }
    }
}
