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
        
        public bool Equals(DColor color)
        {
            return R == color.R && G == color.G && B == color.B && A == color.A;
        }

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
