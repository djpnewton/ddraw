using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public class SelectionFigure : RectbaseFigure
    {
        public SelectionFigure(DRect rect, double rotation) : base(rect, rotation) { }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawRect(X, Y, Width, Height, DColor.White, Alpha, Scale);
            dg.DrawRect(X, Y, Width, Height, DColor.Black, Alpha, Scale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
        }
    }

    public class EraserFigure : EllipseFigure
    {
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
