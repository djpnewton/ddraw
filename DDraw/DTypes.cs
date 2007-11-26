using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public enum DPenStyle { Solid, Dash, Dot };

    public enum DFillStyle { Solid, ForwardDiagonalHatch };

    public enum DCursor { Default, MoveAll, MoveNS, MoveWE, MoveNESW, MoveNWSE, Rotate, Crosshair, IBeam };

    public enum DHitTest { None, Body, SelectRect, Resize, Rotate };

    public enum DMouseButton { Left, Right, Middle, NotApplicable };

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

        public DRect Union(DRect r2)
        {
            return new DRect(Math.Min(X, r2.X), Math.Min(Y, r2.Y),
                Math.Max(Right, r2.Right), Math.Max(Bottom, r2.Bottom), 0);
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
}
