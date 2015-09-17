using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoViewer.InputDevice
{
    struct SRawInputHeader
    {
        public int Type;
        public int Size;
        public IntPtr Device;
        public IntPtr WParam;
    }
}
