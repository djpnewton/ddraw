using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public class SelectionFigure : RectbaseFigure
    {
        // reimplement x,y,width & height to escape the UndoRedo properties
        double x;
        public override double X
        {
            get { return x; }
            set { x = value; }
        }
        double y;
        public override double Y
        {
            get { return y; }
            set { y = value; }
        }
        double width;
        public override double Width
        {
            get { return width; }
            set { width = value; }
        }
        double height;
        public override double Height
        {
            get { return height; }
            set { height = value; }
        }

        public SelectionFigure(DRect rect, double rotation) : base(rect, rotation) { }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawRect(X, Y, Width, Height, DColor.White, Alpha, Scale);
            dg.DrawRect(X, Y, Width, Height, DColor.Black, Alpha, Scale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
        }
    }

    public class EraserFigure : EllipseFigure
    {
        // reimplement x,y,width & height to escape the UndoRedo properties
        double x;
        public override double X
        {
            get { return x; }
            set { x = value; }
        }
        double y;
        public override double Y
        {
            get { return y; }
            set { y = value; }
        }
        double width;
        public override double Width
        {
            get { return width; }
            set { width = value; }
        }
        double height;
        public override double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Size
        {
            get { return Width; }
            set
            {
                Width = value;
                Height = value;
            }
        }

        public EraserFigure(double size)
        {
            Size = size;
            Fill = DColor.White;
            Stroke = DColor.Black;
            StrokeWidth = 2;
        }
    }
}
