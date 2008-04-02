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
            dg.DrawRect(X, Y, Width, Height, DColor.White, Alpha, _controlScale);
            dg.DrawRect(X, Y, Width, Height, DColor.Black, Alpha, _controlScale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
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

        // reimplement fill, stroke & strokewidth to escape the UndoRedo properties
        public override DColor Fill
        {
            get { return DColor.White; }
            set { }
        }
        public override DColor Stroke
        {
            get { return DColor.Black; }
            set { }
        }
        public override double StrokeWidth
        {
            get { return 2; }
            set { }
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
        }
    }

    public enum DGlyphPosition { TopRight, BottomLeft, Center, CenterStack };

    public enum DGlyphVisiblity { Never, WhenFigureSelected, Always };

    public delegate void GlyphClickedHandler(IGlyph glyph, Figure figure, DPoint pt);

    public interface IGlyph
    {
        DGlyphPosition Position
        {
            get;
            set;
        }

        void CenterStack(int total, int index);

        DGlyphVisiblity Visiblility
        {
            get;
            set;
        }

        bool IsVisible(bool figureSelected);

        double Width
        {
            get;
        }

        double Height
        {
            get;
        }

        void Paint(DGraphics dg, Figure f, double scale);

        DHitTest HitTest(DPoint pt, Figure hitTestFigure, double scale);

        DCursor Cursor
        {
            get;
            set;
        }

        event GlyphClickedHandler Clicked;
        void CallClicked(Figure f, DPoint pt);
    }

    public abstract class BasicGlyph : IGlyph
    {
        DGlyphPosition pos = DGlyphPosition.TopRight;
        public DGlyphPosition Position
        {
            get { return pos; }
            set 
            {
                System.Diagnostics.Debug.Assert(value != DGlyphPosition.CenterStack, "Must use \"CenterStack\" method instead");
                pos = value; 
            }
        }

        protected int centerStackTotal;
        protected int centerStackIndex;
        public void CenterStack(int total, int index)
        {
            if (total == 1)
                pos = DGlyphPosition.Center;
            else
            {
                centerStackTotal = total;
                centerStackIndex = index;
                pos = DGlyphPosition.CenterStack;
            }
        }

        DGlyphVisiblity visiblility = DGlyphVisiblity.WhenFigureSelected;
        public DGlyphVisiblity Visiblility
        {
            get { return visiblility; }
            set { visiblility = value; }
        }

        public abstract double Width
        {
            get;
        }

        public abstract double Height
        {
            get;
        }

        protected DPoint GetXYPosition(DGlyphPosition pos, DRect figureRect, double scale)
        {
            double x = 0, y = 0;
            switch (pos)
            {
                case DGlyphPosition.TopRight:
                    x = figureRect.Right - Width * scale;
                    y = figureRect.Top;
                    break;
                case DGlyphPosition.BottomLeft:
                    x = figureRect.Left;
                    y = figureRect.Bottom - Height * scale;
                    break;
                case DGlyphPosition.Center:
                    x = figureRect.Center.X - Width / 2 * scale;
                    y = figureRect.Center.Y - Height / 2 * scale;
                    break;
                case DGlyphPosition.CenterStack:
                    x = figureRect.Center.X - Width / 2 * scale;
                    y = figureRect.Center.Y - Height / 2 * scale;
                    double stackX = centerStackIndex * Width * scale;
                    x += stackX;
                    break;
            }
            return new DPoint(x, y);
        }

        protected DRect GetRect(DGlyphPosition pos, DRect figureRect, double scale)
        {
            DPoint pt = GetXYPosition(pos, figureRect, scale);
            return new DRect(pt.X, pt.Y, Width * scale, Height * scale);
        }

        public abstract void Paint(DGraphics dg, Figure f, double scale);

        public bool IsVisible(bool figureSelected)
        {
            return visiblility == DGlyphVisiblity.Always || (visiblility == DGlyphVisiblity.WhenFigureSelected && figureSelected);
        }

        public DHitTest HitTest(DPoint pt, Figure f, double scale)
        {
            if (IsVisible(f.Selected))
            {
                if (DGeom.PointInRect(pt, GetRect(pos, f.GetSelectRect(), scale)))
                    return DHitTest.Glyph;
            }
            return DHitTest.None;
        }

        DCursor cursor = DCursor.Default;
        public DCursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        public event GlyphClickedHandler Clicked;
        public void CallClicked(Figure f, DPoint pt)
        {
            if (Clicked != null)
                Clicked(this, f, pt);
        }
    }

    public class BitmapGlyph: BasicGlyph
    {
        public override double Width
        {
            get { return bmp.Width; }
        }

        public override double Height
        {
            get { return bmp.Height; }
        }

        DBitmap bmp;
        public DBitmap Bitmap
        {
            get { return bmp; }
            set { bmp = value; }
        }

        public BitmapGlyph(DBitmap bmp, DGlyphPosition pos)
        {
            this.bmp = bmp;
            Position = pos;
        }

        public override void Paint(DGraphics dg, Figure f, double scale)
        {
            dg.DrawBitmap(bmp, GetRect(Position, f.GetSelectRect(), scale));
        }
    }
}
