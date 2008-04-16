using System;
using System.Collections.Generic;
using System.Text;

using DejaVu;

namespace DDraw
{
    public class UndoRect
    {
        UndoRedo<double> _x = new UndoRedo<double>(0);
        UndoRedo<double> _y = new UndoRedo<double>(0);
        UndoRedo<double> _width = new UndoRedo<double>(0);
        UndoRedo<double> _height = new UndoRedo<double>(0);
        public double X
        {
            get { return _x.Value; }
            set { _x.Value = value; }
        }
        public double Y
        {
            get { return _y.Value; }
            set { _y.Value = value; }
        }
        public double Width
        {
            get { return _width.Value; }
            set { _width.Value = value; }
        }
        public double Height
        {
            get { return _height.Value; }
            set { _height.Value = value; }
        }
        public DRect Rect
        {
            get { return new DRect(X, Y, Width, Height); }
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }
        public UndoRect(DRect r)
        {
            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
        }
    }
}
