using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoViewer.Input.Raw
{
    struct SRawInputDevice
    {
        public short UsagePage;
        public short Usage;
        public int Flags;
        public IntPtr Target;
    }
}
