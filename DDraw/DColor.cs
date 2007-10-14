using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public struct DColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public DColor(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public DColor(byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
            A = 255;
        }

        // Predefined colors

        public static DColor Clear
        {
            get { return new DColor(0, 0, 0, 0); }
        }

        public static DColor Red
        {
            get { return new DColor(255, 0, 0); }
        }

        public static DColor Green
        {
            get { return new DColor(0, 255, 0); }
        }

        public static DColor Blue
        {
            get { return new DColor(0, 0, 255); }
        }

        public static DColor Black
        {
            get { return new DColor(0, 0, 0); }
        }

        public static DColor White
        {
            get { return new DColor(255, 255, 255); }
        }
    }
}
