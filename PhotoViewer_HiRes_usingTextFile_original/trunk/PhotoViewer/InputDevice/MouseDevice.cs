using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhotoViewer.InputDevice
{
    class MouseDevice: PointingDevice
    {
        public MouseDevice(int header, Vector2 position)
            : base(header, position)
        { }
        
    }
}
