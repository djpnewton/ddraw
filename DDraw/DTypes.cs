using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public enum DStrokeStyle { Solid, Dash, Dot, DashDot, DashDotDot };

    public enum DStrokeJoin { Mitre, Round, Bevel };

    public enum DStrokeCap { Butt, Round, Square };

    public enum DFillStyle { Solid, ForwardDiagonalHatch };

    public enum DMarker { None, Arrow, Dot, Square, Diamond };

    public enum DCursor { Default, MoveAll, MoveNS, MoveWE, MoveNESW, MoveNWSE, Rotate, Crosshair, IBeam, Hand };

    public enum DHitTest { None, Body, SelectRect, Resize, Rotate, Context, ReposLinePt1, ReposLinePt2, Glyph, Lock };

    public enum DMouseButton { Left, Right, Middle, NotApplicable };

    public enum DKeys { Backspace = (int)'\b', Tab = (int)'\t', Enter = (int)'\r', Escape = 27, Delete = 127, 
        Left = 100000, Right = 100001, Up = 1000002, Down = 100003,
        Home = 100004, End = 100005, PageUp = 100006, PageDown = 100007 };

    public struct DKey
    {
        public int Code;
        public bool Shift;
        public bool Ctrl;
        public bool Alt;

        public DKey(int code, bool shift, bool ctrl, bool alt)
        {
            Code = code;
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
        }
    }

    public struct DMatrix
    {
        public double A, B, C, D, E, F;

        public DMatrix(double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }
    }

    public class DPoint
    {
        public double X, Y;

        public DPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public DPoint Offset(double x, double y)
        {
            return new DPoint(X + x, Y + y);
        }

        public bool Equals(DPoint pt)
        {
            return pt.X == X && pt.Y == Y;
        }

        public DPoint Clone()
        {
            return new DPoint(X, Y);
        }

        public static string FormatToString(DPoint pt)
        {
            return string.Format("{0},{1}", pt.X, pt.Y);
        }

        public static DPoint FromString(string s)
        {
            string[] parts = s.Split(',');
            if (parts.Length == 2)
            {
                double x, y;
                double.TryParse(parts[0], out x);
                double.TryParse(parts[1], out y);
                return new DPoint(x, y);
            }
            else
                return new DPoint(0, 0);
        }
    }

    public class DPoints : List<DPoint>
    {
        public DRect Bounds()
        {
            DRect result = new DRect(0, 0, 0, 0);
            if (Count > 0)
            {
                result.TopLeft = this[0];
                foreach (DPoint pt in this)
                {
                    if (pt.X < result.Left)
                        result.Left = pt.X;
                    else if (pt.X > result.Right)
                        result.Right = pt.X;
                    if (pt.Y < result.Top)
                        result.Top = pt.Y;
                    else if (pt.Y > result.Bottom)
                        result.Bottom = pt.Y;
                }
            }
            return result;    
        }

        public static DPoints FromArray(DPoint[] pts)
        {
            DPoints points = new DPoints();
            foreach (DPoint pt in pts)
                points.Add(pt);
            return points;
        }

        public static string FormatToString(DPoints pts)
        {
            string res = "";
            foreach (DPoint pt in pts)
                res = string.Concat(res, DPoint.FormatToString(pt), " ");
            return res;
        }

        public static DPoints FromString(string s)
        {
            DPoints res = new DPoints();
            string[] parts = s.Trim().Split(' ');
            foreach (string part in parts)
                res.Add(DPoint.FromString(part));
            return res;
        }
    }

    public struct DRect
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double Left
        {
            get { return X; }
            set
            {
                Width = Right - value;
                X = value;
            }
        }
        public double Top
        {
            get { return Y; }
            set
            {
                Height = Bottom - value;
                Y = value;
            }
        }
        public double Right
        {
            get { return X + Width; }
            set { Width = value - X; }
        }
        public double Bottom
        {
            get { return Y + Height; }
            set { Height = value - Y; }
        }
        public DPoint TopLeft
        {
            get { return new DPoint(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public DPoint TopRight
        {
            get { return new DPoint(Right, Y); }
            set
            {
                Right = value.X;
                Y = value.Y;
            }
        }
        public DPoint BottomLeft
        {
            get { return new DPoint(X, Bottom); }
            set
            {
                X = value.X;
                Bottom = value.Y;
            }
        }
        public DPoint BottomRight
        {
            get { return new DPoint(Right, Bottom); }
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }
        public DPoint Size
        {
            get { return new DPoint(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }
        public DPoint Center
        {
            get { return new DPoint(X + Width / 2, Y + Height / 2); }
        }

        public DRect Offset(double dX, double dY)
        {
            return new DRect(X + dX, Y + dY, Width, Height);
        }

        public DRect Inflate(double dX, double dY)
        {
            return new DRect(X, Y, Width + dX, Height + dY);
        }

        public DRect Resize(double dX, double dY, double dWidth, double dHeight)
        {
            return new DRect(X + dX, Y + dY, Width + dWidth, Height + dHeight);
        }

        public DRect Union(DRect r2)
        {
            if (r2.Width > 0 || r2.Height > 0)
                return new DRect(Math.Min(X, r2.X), Math.Min(Y, r2.Y),
                    Math.Max(Right, r2.Right), Math.Max(Bottom, r2.Bottom), 0);
            else
                return this;
        }

        public DRect Intersect(DRect r2)
        {
            if (IntersectsWith(r2))
                return new DRect(Math.Max(X, r2.X), Math.Max(Y, r2.Y),
                    Math.Min(Right, r2.Right), Math.Min(Bottom, r2.Bottom), 0);
            else
                return new DRect();
        }

        public bool Contains(DRect rect)
        {
            return (rect.X >= X && rect.Right <= Right && rect.Y >= Y && rect.Bottom <= Bottom);
        }

        public bool IntersectsWith(DRect rect)
        {
            bool xIn = rect.X >= X && rect.X <= Right;
            bool rightIn = rect.Right >= X && rect.Right <= Right;
            bool yIn = rect.Y >= Y && rect.Y <= Bottom;
            bool bottomIn = rect.Bottom >= Y && rect.Bottom <= Bottom;
            return (xIn && yIn) || (xIn && bottomIn) || (yIn && rightIn) || (bottomIn && rightIn);
        }
        
        public bool Equals(DRect rect)
        {
            return (X == rect.X && Y == rect.Y & Width == rect.Width && Height == rect.Height);
        }


        public DRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public DRect(double x, double y, double right, double bottom, double zz)
        {
            X = x;
            Y = y;
            Width = 0;
            Height = 0;
            Right = right;
            Bottom = bottom;
        }
    }

    public enum PageFormat { A4, A5, Letter, Custom };

    public enum Zoom { FitToPage, FitToWidth, Custom };

    public static class PageTools
    {
        public const int DefaultPageWidth = 500;
        public const int DefaultPageHeight = 400;

        // TODO: find these out cross platform
        public const int DpiX = 96;
        public const int DpiY = 96;

        const int A4W = 210, A4H = 297;
        const int A5W = 148, A5H = 210;
        const int LtW = 216, LtH = 279;

        public const float MMPerInch = 25.4f;

        public static DPoint FormatToSizeMM(PageFormat pf)
        {
            switch (pf)
            {
                case PageFormat.A4:
                    return new DPoint(A4W, A4H);
                case PageFormat.A5:
                    return new DPoint(A5W, A5H);
                case PageFormat.Letter:
                    return new DPoint(LtW, LtH);
                default:
                    System.Diagnostics.Debug.Assert(false, "ERROR: PageFormat.Custom has no size");
                    return new DPoint(A4W, A4H);
            }
        }

        public static DPoint FormatToSize(PageFormat pf)
        {
            DPoint szMM = FormatToSizeMM(pf);
            return new DPoint(szMM.X * DpiX / MMPerInch, szMM.Y * DpiY / MMPerInch);
        }

        public static PageFormat SizeMMToFormat(DPoint pgSzMM)
        {
            if ((int)pgSzMM.X == A4W && (int)pgSzMM.Y == A4H)
                return PageFormat.A4;
            else if ((int)pgSzMM.X == A5W && (int)pgSzMM.Y == A5H)
                return PageFormat.A5;
            else if ((int)pgSzMM.X == LtW && (int)pgSzMM.Y == LtH)
                return PageFormat.Letter;
            else
                return PageFormat.Custom;
        }

        public static PageFormat SizeToFormat(DPoint pgSz)
        {
            return SizeMMToFormat(SizetoSizeMM(pgSz));
        }

        public static DPoint SizeMMtoSize(DPoint pgSzMM)
        {
            return new DPoint(pgSzMM.X * DpiX / MMPerInch, pgSzMM.Y * DpiY / MMPerInch);
        }

        public static DPoint SizetoSizeMM(DPoint pgSz)
        {
            return new DPoint(pgSz.X * MMPerInch / DpiX, pgSz.Y * MMPerInch / DpiY);
        }
    }
}
