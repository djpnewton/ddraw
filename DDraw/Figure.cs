using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public interface IFillable
    {
        DColor Fill
        {
            get;
            set;
        }
    }

    public interface IStrokeable
    {
        DColor Stroke
        {
            get;
            set;
        }
        double StrokeWidth
        {
            get;
            set;
        }
        DRect RectInclStroke
        {
            get;
        }
    }

    public static class StrokeHelper
    {
        public static DRect RectIncludingStrokeWidth(DRect r, double strokeWidth)
        {
            double sw = strokeWidth / 2;
            return new DRect(r.X - sw, r.Y - sw, r.Width + strokeWidth, r.Height + strokeWidth);
        }

        public static DRect SelectRectIncludingStrokeWidth(DRect r, double strokeWidth)
        {
            double sw = Math.Floor(strokeWidth / 2);
            return new DRect(r.X - sw, r.Y - sw, r.Width + sw + sw, r.Height + sw + sw);
        }
    }

    public interface IAlphaBlendable
    {
        double Alpha
        {
            get;
            set;
        }
    }

    public interface ITextable
    {
        string Text
        {
            get;
            set;
        }
        string FontName
        {
            get;
            set;
        }
        double FontSize
        {
            get;
            set;
        }
    }

    public interface ISelectable
    {
        bool Selected
        {
            get;
            set;
        }
        void PaintSelectionChrome(DViewer dv);
        DRect GetSelectRect();
        DRect GetResizeHandleRect();
        DRect GetRotateHandleRect();
        DRect GetEncompassingRect();
        DHitTest SelectHitTest(DPoint pt);
        DHitTest ResizeHitTest(DPoint pt);
        DHitTest RotateHitTest(DPoint pt);
    }

    public abstract class Figure: ISelectable
    {
        protected double x;
        public virtual double X
        {
            get { return x; }
            set { x = value; }
        }
        protected double y;
        public virtual double Y
        {
            get { return y; }
            set { y = value; }
        }
        protected double width;
        public virtual double Width
        {
            get { return width; }
            set { width = value; }
        }
        protected double height;
        public virtual double Height
        {
            get { return height; }
            set { height = value; }
        }
        public virtual double Left
        {
            get { return x; }
            set { X = value; }
        }
        public virtual double Top
        {
            get { return y; }
            set { Y = value; }
        }
        public virtual double Right
        {
            get { return x + width; }
            set { Width = value - x; }
        }
        public virtual double Bottom
        {
            get { return y + height; }
            set { Height = value - y; }
        }
        public virtual DPoint TopLeft
        {
            get { return new DPoint(x, y); }
            set 
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public virtual DPoint BottomRight
        {
            get { return new DPoint(Right, Bottom); }
            set 
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }
        public DRect Rect
        {
            get { return new DRect(x, y, width, height); }
            set
            {
                x = value.X;
                y = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }
        const double snapAngle = Math.PI / 4;       // 45 degrees
        const double snapRange = Math.PI / (4 * 9); // 5  degrees
        double rotation;
        public double Rotation
        {
            get { return rotation; }
            set
            {
                double r = value % snapAngle;
                if (r < snapRange)
                    rotation = value - r;
                else if (r > snapAngle - snapRange)
                    rotation = value + snapAngle - r;
                else
                    rotation = value;
            }
        }

        public bool LockAspectRatio = false;

        public Figure()
        {
            Rect = new DRect();
            Rotation = 0;
        }

        public Figure(DRect rect, double rotation)
        {
            Rect = rect;
            Rotation = rotation;
        }

        public DPoint RotatePointToFigure(DPoint pt)
        {
            // negative rotation to put point in figure coordinate space
            return DGeom.RotatePoint(pt, Rect.Center, -Rotation);
        }

        public DHitTest HitTest(DPoint pt)
        {
            pt = RotatePointToFigure(pt);
            DHitTest res = RotateHitTest(pt);
            if (res == DHitTest.None)
            {
                res = ResizeHitTest(pt);
                if (res == DHitTest.None)
                {
                    res = SelectHitTest(pt);
                    if (res == DHitTest.None)
                        res = _HitTest(pt);
                }
            }
            return res;
        }

        protected abstract DHitTest _HitTest(DPoint pt);

        public void Paint(DViewer dv)
        {
            DMatrix m = dv.SaveTransform();
            dv.Rotate(Rotation, Rect.Center);
            PaintBody(dv);
            PaintSelectionChrome(dv);
            dv.LoadTransform(m);
        }

        protected abstract void PaintBody(DViewer dv);

        public bool contains(Figure f)
        {
            //TODO rotate
            return Rect.Contains(f.Rect);
        }

        public virtual void BeforeResize(){}
        public virtual void AfterResize(){}

        #region ISelectable Members

        protected const int S_INDENT = 4;
        protected const int HANDLE_SZ = 5;

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public virtual void PaintSelectionChrome(DViewer dv)
        {
            if (Selected && dv.EditFigures)
            {
                // draw selection rectangle
                DRect r = GetSelectRect();
                dv.DrawRect(r, DColor.White);
                dv.DrawRect(r, DColor.Black, 1, DPenStyle.Dot);
                // draw resize handle
                r = GetResizeHandleRect();
                dv.FillEllipse(r, DColor.Red);
                dv.DrawEllipse(r, DColor.Black);
                // draw rotate handle
                r = GetRotateHandleRect();
                DPoint p1 = r.Center;
                DPoint p2 = p1.Offset(0, 3 * HANDLE_SZ - S_INDENT);
                dv.DrawLine(p1, p2, DColor.White);
                dv.DrawLine(p1, p2, DColor.Black, DPenStyle.Dot);
                dv.FillEllipse(r, DColor.Blue);
                dv.DrawEllipse(r, DColor.Black);
                //dv.DrawRect(GetEncompassingRect(), DColor.Black);
            }
        }

        public virtual DRect GetSelectRect()
        {
            return Rect.Offset(-S_INDENT, -S_INDENT).Inflate(S_INDENT + S_INDENT, S_INDENT + S_INDENT);
        }

        public virtual DRect GetResizeHandleRect()
        {
            DRect selectRect = GetSelectRect();
            return new DRect(selectRect.Right - HANDLE_SZ, selectRect.Bottom - HANDLE_SZ, HANDLE_SZ + HANDLE_SZ, HANDLE_SZ + HANDLE_SZ);
        }

        public virtual DRect GetRotateHandleRect()
        {
            DRect selectRect = GetSelectRect();
            return new DRect(selectRect.Center.X - HANDLE_SZ, selectRect.Y - HANDLE_SZ * 3, HANDLE_SZ + HANDLE_SZ, HANDLE_SZ + HANDLE_SZ);
        }

        public virtual DRect GetEncompassingRect()
        {
            if (selected)
                return GetSelectRect().Union(GetResizeHandleRect()).Union(GetRotateHandleRect());
            else
                return Rect;
        }

        public virtual DHitTest SelectHitTest(DPoint pt)
        {
            if (selected && DGeom.PointInRect(pt, GetSelectRect()))
                return DHitTest.SelectRect;
            return DHitTest.None;
        }

        public virtual DHitTest ResizeHitTest(DPoint pt)
        {
            if (selected && DGeom.PointInRect(pt, GetResizeHandleRect()))
                return DHitTest.Resize;
            return DHitTest.None;
        }

        public virtual DHitTest RotateHitTest(DPoint pt)
        {
            if (selected && DGeom.PointInRect(pt, GetRotateHandleRect()))
                return DHitTest.Rotate;
            return DHitTest.None;
        }

        #endregion
    }

    public class RectFigure : Figure, IFillable, IStrokeable, IAlphaBlendable
    {
        public RectFigure()
        {
        }

        public RectFigure(DRect rect, double rotation)
            : base(rect, rotation)
        {
        }

        public override DRect GetSelectRect()
        {
            return StrokeHelper.SelectRectIncludingStrokeWidth(base.GetSelectRect(), strokeWidth);
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInRect(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DViewer dv)
        {
            dv.FillRect(X, Y, Width, Height, fill, alpha);
            dv.DrawRect(X, Y, Width, Height, stroke, alpha, strokeWidth);
        }

        #region IFillable Members
        DColor fill = DColor.Red;
        public DColor Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        #endregion

        #region IStrokeable Members
        DColor stroke = DColor.Blue;
        public DColor Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }
        double strokeWidth = 1;
        public double StrokeWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }  
        }
        public DRect RectInclStroke
        {
            get
            {
                return StrokeHelper.RectIncludingStrokeWidth(Rect, strokeWidth);
            }
        }
        #endregion

        #region IAlphaBlendable Members
        double alpha = 1;
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        #endregion
    }

    public class SelectionFigure : RectFigure
    {
        public SelectionFigure(DRect rect, double rotation)
            : base(rect, rotation)
        {
        }

        protected override void PaintBody(DViewer dv)
        {
            dv.DrawRect(X, Y, Width, Height, DColor.White, Alpha, StrokeWidth);
            dv.DrawRect(X, Y, Width, Height, DColor.Black, Alpha, StrokeWidth, DPenStyle.Dot);
        }
    }

    public class EllipseFigure : RectFigure
    {
        public EllipseFigure(DRect rect, double rotation)
            : base(rect, rotation)
        {
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInEllipse(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;

        }

        protected override void PaintBody(DViewer dv)
        {
            dv.FillEllipse(X, Y, Width, Height, Fill, Alpha);
            dv.DrawEllipse(X, Y, Width, Height, Stroke, Alpha, StrokeWidth);
        }
    }

    public class PolylineFigure : Figure, IStrokeable, IAlphaBlendable
    {
        DPoints points;
        public DPoints Points
        {
            get { return points; }
            set
            {
                points = value;
                Rect = points.Bounds();
            }
        }

        public override double X
        {
            set
            {
                double dX = value - X;
                foreach (DPoint pt in Points)
                    pt.X += dX;
                Points = Points;
            }
        }

        public override double Y
        {
            set
            {
                double dY = value - Y;
                foreach (DPoint pt in Points)
                    pt.Y += dY;
                Points = Points;
            }
        }

        public override double Width
        {
            set
            {
                double scale = value / width;
                foreach (DPoint pt in points)
                    pt.X += (pt.X - x) * (scale - 1);
                Points = Points;
            }
        }

        public override double Height
        {
            set
            {
                double scale = value / height;
                foreach (DPoint pt in points)
                    pt.Y += (pt.Y - y) * (scale - 1);
                Points = Points;
            }
        }

        public PolylineFigure(DPoints points) : base(new DRect(), 0)
        {
            Points = points;
        }

        public override DRect GetSelectRect()
        {
            return StrokeHelper.SelectRectIncludingStrokeWidth(base.GetSelectRect(), strokeWidth);
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInPolyline(pt, points, strokeWidth / 2))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DViewer dv)
        {
            dv.DrawPolyline(points, stroke, alpha, strokeWidth);
        }

        DPoint beforeResizeSize;

        public override void BeforeResize()
        {
            beforeResizeSize = new DPoint(width, height);    
        }

        public override void AfterResize()
        {
            strokeWidth *= ((width / beforeResizeSize.X) + (height / beforeResizeSize.Y)) / 2;
        }

        #region IStrokeable Members
        DColor stroke = DColor.Blue;
        public DColor Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }
        double strokeWidth = 1;
        public double StrokeWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }
        }
        public DRect RectInclStroke
        {
            get
            {
                return StrokeHelper.RectIncludingStrokeWidth(Rect, strokeWidth);
            }
        }
        #endregion

        #region IAlphaBlendable Members
        double alpha = 1;
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        #endregion
    }

    public abstract class RectbaseFigure : Figure, IAlphaBlendable
    {
        public RectbaseFigure()
        {
        }

        public RectbaseFigure(DRect rect, double rotation)
            : base(rect, rotation)
        {
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInRect(pt, Rect))
                return DHitTest.Body;
            return DHitTest.None;
        }

        #region IAlphaBlendable Members
        double alpha = 1;
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        #endregion
    }

    public class ImageFigure : RectbaseFigure
    {
        public DBitmap Bitmap = null;

        public ImageFigure(DRect rect, double rotation, DBitmap bitmap)
            : base(rect, rotation)
        {
            Bitmap = bitmap;
        }

        protected override void PaintBody(DViewer dv)
        {
            dv.DrawBitmap(Bitmap, Rect, Alpha);
        }
    }

    public class TextFigure : RectbaseFigure, IFillable, ITextable
    {
        DTextExtent textExtent;

        string fontName = "Courier New";
        public string FontName
        {
            get { return fontName; }
            set
            {
                fontName = value;
                Text = text;
            }
        }

        double fontSize = 10;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                if (value <= 0)
                    value = 1;
                fontSize = value;
                Text = text;
            }
        }

        private string text = null;
        public string Text
        {
            get { return text; }
            set
            {
                DPoint sz = textExtent.MeasureText(value, fontName, fontSize);
                base.Width = sz.X;
                base.Height = sz.Y;
                text = value;
            }
        }

        public override double Width
        {
            get
            {
                return base.width;
            }
            set
            {
                fontSize *= value / base.width;
                base.Width = value;
            }
        }

        public TextFigure(DPoint pt, string text, DTextExtent textExtent, double rotation)
        {
            this.textExtent = textExtent;
            base.TopLeft = pt;
            Text = text;
            base.Rotation = rotation;
            LockAspectRatio = true;
        }

        public TextFigure(DPoint pt, string text, string fontName, double fontSize, DTextExtent textExtent, double rotation)
        {
            this.textExtent = textExtent;
            base.TopLeft = pt;
            this.fontName = fontName;
            this.fontSize = fontSize;
            Text = text;
            base.Rotation = rotation;
            LockAspectRatio = true;
        }

        protected override void PaintBody(DViewer dv)
        {
            dv.DrawText(Text, fontName, fontSize, Rect, fill, Alpha);
        }

        #region IFillable Members
        DColor fill = DColor.Red;
        public DColor Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        #endregion
    }
}
