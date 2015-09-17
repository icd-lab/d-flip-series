using System;
using System.Collections.Generic;
using System.Text;

namespace dflip.InputDevice
{
    struct SRawInputDevice
    {
        public short UsagePage;
        public short Usage;
        public int Flags;
        public IntPtr Target;
    }
}
