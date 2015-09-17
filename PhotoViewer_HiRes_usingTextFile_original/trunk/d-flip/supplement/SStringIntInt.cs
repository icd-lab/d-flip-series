using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoViewer.Supplement
{
    public struct SStringIntInt
    {
        public string Name;
        public int X;
        public int Y;

        public SStringIntInt(string n, int x, int y)
        {
            Name = n;
            X = x;
            Y = y;
        }
    }
}
