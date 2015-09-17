using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;

namespace PhotoViewer.Input.Raw
{
    [Flags()]
    public enum RawMouseFlags
        : ushort
    {
        /// <summary>Relative to the last position.</summary>
        MoveRelative = 0,
        /// <summary>Absolute positioning.</summary>
        MoveAbsolute = 1,
        /// <summary>Coordinate data is mapped to a virtual desktop.</summary>
        VirtualDesktop = 2,
        /// <summary>Attributes for the mouse have changed.</summary>
        AttributesChanged = 4
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct Data
        {
            [FieldOffset(0)]
            public uint Buttons;
            /// <summary>
            /// If the mouse wheel is moved, this will contain the delta amount.
            /// </summary>
            [FieldOffset(2)]
            public short ButtonData;
            /// <summary>
            /// Flags for the event.
            /// </summary>
            [FieldOffset(0)]
            public RawMouseButtons ButtonFlags;
        }
    struct SRawMouse
    {
        public RawMouseFlags Flags;

        public Data Data;

        /// <summary>
        /// Raw button data.
        /// </summary>
        public uint RawButtons;
        /// <summary>
        /// The motion in the X direction. This is signed relative motion or 
        /// absolute motion, depending on the value of usFlags. 
        /// </summary>
        public int LastX;
        /// <summary>
        /// The motion in the Y direction. This is signed relative motion or absolute motion, 
        /// depending on the value of usFlags. 
        /// </summary>
        public int LastY;
        /// <summary>
        /// The device-specific additional information for the event. 
        /// </summary>
        public uint ExtraInformation;

    }

    [Flags()]
    public enum RawMouseButtons
        : ushort
    {
        /// <summary>No button.</summary>
        None = 0,
        /// <summary>Left (button 1) down.</summary>
        LeftDown = 0x0001,
        /// <summary>Left (button 1) up.</summary>
        LeftUp = 0x0002,
        /// <summary>Right (button 2) down.</summary>
        RightDown = 0x0004,
        /// <summary>Right (button 2) up.</summary>
        RightUp = 0x0008,
        /// <summary>Middle (button 3) down.</summary>
        MiddleDown = 0x0010,
        /// <summary>Middle (button 3) up.</summary>
        MiddleUp = 0x0020,
        /// <summary>Button 4 down.</summary>
        Button4Down = 0x0040,
        /// <summary>Button 4 up.</summary>
        Button4Up = 0x0080,
        /// <summary>Button 5 down.</summary>
        Button5Down = 0x0100,
        /// <summary>Button 5 up.</summary>
        Button5Up = 0x0200,
        /// <summary>Mouse wheel moved.</summary>
        MouseWheel = 0x0400
    }
}
