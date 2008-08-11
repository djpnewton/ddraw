using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public struct DColor
    {
        byte r;
        public byte R
        {
            get { return r; }
            set { r = value; SetEmpty(); }
        }
        byte g;
        public byte G
        {
            get { return g; }
            set { g = value; SetEmpty(); }
        }
        byte b;
        public byte B
        {
            get { return b; }
            set { b = value; SetEmpty(); }
        }
        byte a;
        public byte A
        {
            get { return a; }
            set { a = value; SetEmpty(); }
        }

        bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
        }

        void SetEmpty()
        {
            isEmpty = r == 0 && g == 0 && b == 0 && a == 0;
        }
        
        public bool Equals(DColor color)
        {
            return R == color.R && G == color.G && B == color.B && A == color.A;
        }

        public DColor(byte red, byte green, byte blue, byte alpha)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
            isEmpty = false;
            SetEmpty();
        }

        public DColor(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            a = 255;
            isEmpty = false;
        }

        public static string FormatToString(DColor color)
        {
            return string.Format("{0},{1},{2},{3}", color.R, color.G, color.B, color.A);
        }

        public static DColor FromString(string s)
        {
            string[] parts = s.Split(',');
            if (parts.Length == 3 || parts.Length == 4)
            {
                byte r, g, b, a = 0;;
                byte.TryParse(parts[0], out r);
                byte.TryParse(parts[1], out g);
                byte.TryParse(parts[2], out b);
                if (parts.Length == 4)
                    byte.TryParse(parts[3], out a);
                return new DColor(r, g, b, a);
            }
            else
                return DColor.Black;                
        }

        public static DColor FromHtml(string s)
        {
            if (s.Length == 7 && s[0] == '#')
            {
                byte r = Convert.ToByte(string.Concat("0x" + s.Substring(1, 2)), 16);
                byte g = Convert.ToByte(string.Concat("0x" + s.Substring(3, 2)), 16);
                byte b = Convert.ToByte(string.Concat("0x" + s.Substring(5, 2)), 16);
                return new DColor(r, g, b);
            }
            else if (s == "none")
                return new DColor(0, 0, 0, 0);
            return DColor.Black;
        }

        // Predefined colors

        public static DColor Empty
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

        public static DColor Blue50Pc
        {
            get { return new DColor(0, 0, 255, 128); }
        }

        public static DColor Black
        {
            get { return new DColor(0, 0, 0); }
        }

        public static DColor White
        {
            get { return new DColor(255, 255, 255); }
        }

        public static DColor LightGray
        {
            get { return new DColor(192, 192, 192); }
        }
    }
}
