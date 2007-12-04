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
	
	public interface IBitmapable
	{
		DBitmap Bitmap
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
        bool HasText
        {
            get;
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

    public interface IChildFigureable
    {
        Figure[] ChildFigures
        {
            get;
        }
    }

    public interface ISelectable
    {
        bool Selected
        {
            get;
            set;
        }
        void PaintSelectionChrome(DGraphics dg);
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
        public double Scale = 1;

        double x;
        public virtual double X
        {
            get { return x; }
            set { x = value; }
        }
        double y;
        public virtual double Y
        {
            get { return y; }
            set { y = value; }
        }
        double width;
        public virtual double Width
        {
            get { return width; }
            set { width = value; }
        }
        double height;
        public virtual double Height
        {
            get { return height; }
            set { height = value; }
        }
        public virtual double Left
        {
            get { return X; }
            set { X = value; }
        }
        public virtual double Top
        {
            get { return Y; }
            set { Y = value; }
        }
        public virtual double Right
        {
            get { return X + Width; }
            set { Width = value - x; }
        }
        public virtual double Bottom
        {
            get { return Y + Height; }
            set { Height = value - y; }
        }
        public virtual DPoint TopLeft
        {
            get { return new DPoint(X, Y); }
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
            get { return new DRect(X, Y, Width, Height); }
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

        bool lockAspectRatio = false;
        public virtual bool LockAspectRatio
        {
            get { return lockAspectRatio; }
            set { lockAspectRatio = value; }
        }

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

        void ApplyTransforms(DGraphics dg)
        {
            dg.Rotate(Rotation, Rect.Center);
        }

        public void Paint(DGraphics dg)
        {
            DMatrix m = dg.SaveTransform();
            ApplyTransforms(dg);
            PaintBody(dg);
            dg.LoadTransform(m);
        }

        protected abstract void PaintBody(DGraphics dg);

        public bool Contains(Figure childFigure)
        {
            // rotate childFigure's rect by its rotation
            DRect r = DGeom.BoundingBoxOfRotatedRect(childFigure.Rect, childFigure.Rotation, childFigure.Rect.Center);
            // rotate result by the reverse rotation of this figures rect
            r = DGeom.BoundingBoxOfRotatedRect(r, -rotation, Rect.Center);
            // return whether result lies within this figures rect
            return Rect.Contains(r);
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

        public virtual void PaintSelectionChrome(DGraphics dg)
        {
            if (Selected)
            {
                // save current transform
                DMatrix m = dg.SaveTransform();
                // apply transform
                ApplyTransforms(dg);
                // draw selection rectangle
                DRect r = GetSelectRect();
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.White, 1, Scale);
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DPenStyle.Dot);
                // draw resize handle
                r = GetResizeHandleRect();
                dg.FillEllipse(r, DColor.Red);
                dg.DrawEllipse(r, DColor.Black);
                // draw rotate handle
                r = GetRotateHandleRect();
                DPoint p1 = r.Center;
                DPoint p2 = p1.Offset(0, 3 * HANDLE_SZ * Scale - S_INDENT * Scale);
                dg.DrawLine(p1, p2, DColor.White, 1, DPenStyle.Solid, Scale);
                dg.DrawLine(p1, p2, DColor.Black, 1, DPenStyle.Dot, Scale);
                dg.FillEllipse(r, DColor.Blue);
                dg.DrawEllipse(r, DColor.Black);
			    //dg.DrawRect(GetEncompassingRect(), DColor.Black);
                // load previous transform
                dg.LoadTransform(m);
            }
        }

        public virtual DRect GetSelectRect()
        {
            double i = S_INDENT * Scale;
            return Rect.Offset(-i, -i).Inflate(i + i, i + i);
        }

        public virtual DRect GetResizeHandleRect()
        {
            DRect selectRect = GetSelectRect();
            double hs = HANDLE_SZ * Scale;
            return new DRect(selectRect.Right - hs, selectRect.Bottom - hs, hs + hs, hs + hs);
        }

        public virtual DRect GetRotateHandleRect()
        {
            DRect selectRect = GetSelectRect();
            double hs = HANDLE_SZ * Scale;
            return new DRect(selectRect.Center.X - hs, selectRect.Y - hs * 3, hs + hs, hs + hs);
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

        protected override void PaintBody(DGraphics dg)
        {
            dg.FillRect(X, Y, Width, Height, fill, alpha);
            dg.DrawRect(X, Y, Width, Height, stroke, alpha, strokeWidth);
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
        public SelectionFigure(DRect rect, double rotation) : base(rect, rotation) { }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawRect(X, Y, Width, Height, DColor.White, Alpha, StrokeWidth * Scale);
            dg.DrawRect(X, Y, Width, Height, DColor.Black, Alpha, StrokeWidth * Scale, DPenStyle.Dot);
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

        protected override void PaintBody(DGraphics dg)
        {
            dg.FillEllipse(X, Y, Width, Height, Fill, Alpha);
            dg.DrawEllipse(X, Y, Width, Height, Stroke, Alpha, StrokeWidth);
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
                double scale = value / Width;
                foreach (DPoint pt in points)
                    pt.X += (pt.X - X) * (scale - 1);
                Points = Points;
            }
        }

        public override double Height
        {
            set
            {
                double scale = value / Height;
                foreach (DPoint pt in points)
                    pt.Y += (pt.Y - Y) * (scale - 1);
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

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawPolyline(points, stroke, alpha, strokeWidth);
        }

        DPoint beforeResizeSize;

        public override void BeforeResize()
        {
            beforeResizeSize = new DPoint(Width, Height);    
        }

        public override void AfterResize()
        {
            strokeWidth *= ((Width / beforeResizeSize.X) + (Height / beforeResizeSize.Y)) / 2;
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
        public virtual double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        #endregion
    }

    public class ImageFigure : RectbaseFigure, IBitmapable
    {
		DBitmap bitmap = null;
        public DBitmap Bitmap
		{
			get { return bitmap; }
			set { bitmap = value; }
		}

        public ImageFigure(DRect rect, double rotation, DBitmap bitmap)
            : base(rect, rotation)
        {
            this.bitmap = bitmap;
        }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawBitmap(Bitmap, Rect, Alpha);
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

        public bool HasText
        {
            get { return text != null && text.Length > 0; }
        }

        public override double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                fontSize *= value / base.Width;
                base.Width = value;
            }
        }

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        public TextFigure(DPoint pt, string text, DTextExtent textExtent, double rotation)
        {
            this.textExtent = textExtent;
            TopLeft = pt;
            Text = text;
            Rotation = rotation;
        }

        public TextFigure(DPoint pt, string text, string fontName, double fontSize, DTextExtent textExtent, double rotation)
        {
            this.textExtent = textExtent;
            TopLeft = pt;
            this.fontName = fontName;
            this.fontSize = fontSize;
            Text = text;
            Rotation = rotation;
        }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawText(Text, fontName, fontSize, Rect.TopLeft, fill, Alpha);
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

    public class TextEditFigure : RectbaseFigure, ITextable
    {
        const int border = 6;

        TextFigure tf;
        public TextFigure TextFigure
        {
            get { return tf; }
        }

        public string Text
        {
            get { return tf.Text; }
            set { tf.Text = value; }
        }

        public bool HasText
        {
            get { return tf.HasText; }
        }

        public string FontName
        {
            get { return tf.FontName; }
            set { tf.FontName = value; }
        }

        public double FontSize
        {
            get { return tf.FontSize; }
            set
            { tf.FontSize = value; }
        }

        public override double X
        {
            get { return tf.Left - border; }
            set { tf.Left = value + border; }
        }

        public override double Y
        {
            get { return tf.Top - border; }
            set { tf.Top = value + border; }
        }

        public override double Width
        {
            get { return tf.Width + border + border; }
            set { tf.Width = value - border - border; }
        }

        public override double Height
        {
            get { return tf.Height + border + border; }
            set { tf.Height = value - border - border; }
        }

        public TextEditFigure(TextFigure tf)
        {
            this.tf = tf;
        }

        public TextEditFigure(DPoint pt, TextFigure tf)
        {
            this.tf = tf;
            TopLeft = pt;
        }

        protected override void PaintBody(DGraphics dg)
        {
            // paint border
            DRect r = Rect;
            dg.FillRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, DFillStyle.ForwardDiagonalHatch);
            dg.FillRect(tf.X, tf.Y, tf.Width, tf.Height, DColor.White, 1);
            // paint text
            double alpha = tf.Alpha;
            tf.Alpha = 1;
            double rot = tf.Rotation;
            tf.Rotation = 0;
            tf.Paint(dg);
            tf.Alpha = alpha;
            tf.Rotation = rot;
        }
    }

    public class GroupFigure : RectbaseFigure, IChildFigureable
    {
        public override double X
        {
            get { return GetBoundingBox().X; }
            set
            {
                double dX = value - X;
                foreach (Figure f in childFigs)
                    f.X += dX;
            }
        }

        public override double Y
        {
            get { return GetBoundingBox().Y; }
            set
            {
                double dY = value - Y;
                foreach (Figure f in childFigs)
                    f.Y += dY;
            }
        }

        public override double Width
        {
            get { return Right - X; }
            set
            {
                double sx = value / originalRect.Width;
                double dx = X - originalRect.X;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    DRect ocr = originalChildRects[i];
                    f.X = originalRect.X + (sx * (ocr.X - originalRect.X)) + dx;
                    f.Width = sx * ocr.Width;
                    i++;
                }
            }
        }

        public override double Height
        {
            get { return Bottom - Y; }
            set
            {
                double sy = value / originalRect.Height;
                double dy = Y - originalRect.Y;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    DRect ocr = originalChildRects[i];
                    f.Y = originalRect.Y + (sy * (ocr.Y - originalRect.Y)) + dy;
                    f.Height = sy * ocr.Height;
                    i++;
                }
            }
        }

        public override double Right
        {
            get { return GetBoundingBox().Right; }
            set
            { Width = value - X; }
        }

        public override double Bottom
        {
            get { return GetBoundingBox().Bottom; }
            set { Height = value - Y; }
        }

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        public bool UseRealAlpha = true;

        Figure[] childFigs;
        public Figure[] ChildFigures
        {
            get { return childFigs; }
        }

        DRect[] originalChildRects;
        DRect originalRect;

        public GroupFigure(Figure[] figs)
        {
            System.Diagnostics.Debug.Assert(figs != null, "figs is not assigned");
            System.Diagnostics.Debug.Assert(figs.Length > 1, "figs.Length is less than 2");
            childFigs = figs;
            // store starting dimensions for scaling later on
            originalChildRects = new DRect[figs.Length];
            int i = 0;
            foreach (Figure f in figs)
            {
                originalChildRects[i] = f.Rect;
                i++;
            }
            originalRect = Rect;
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            DHitTest ht;
            foreach (Figure f in childFigs)
            {
                ht = f.HitTest(pt);
                if (ht == DHitTest.Body)
                    return ht;
            }
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (UseRealAlpha)
            {
                if (Width > 0 && Height > 0)
                {
                    DRect rS = GetSelectRect();
                    DBitmap bmp = GraphicsHelper.MakeBitmap(rS.Width, rS.Height);
                    DGraphics bmpGfx = GraphicsHelper.MakeGraphics(bmp);
                    bmpGfx.AntiAlias = dg.AntiAlias;
                    bmpGfx.Translate(new DPoint(-rS.X , -rS.Y));
                    foreach (Figure f in childFigs)
                        f.Paint(bmpGfx);
                    dg.DrawBitmap(bmp, rS, Alpha);
                    bmpGfx.Dispose();
                    bmp.Dispose();
                }
            }
            else
                foreach(Figure f in childFigs)
                    f.Paint(dg);
        }

        public override double Alpha
        {
            get
            {
                if (UseRealAlpha)
                    return base.Alpha;
                else
                {
                    List<IAlphaBlendable> figuresWithAlpha = new List<IAlphaBlendable>();
                    foreach (Figure f in childFigs)
                        if (f is IAlphaBlendable)
                            figuresWithAlpha.Add((IAlphaBlendable)f);
                    if (figuresWithAlpha.Count > 0)
                    {
                        for (int i = 1; i < figuresWithAlpha.Count; i++)
                            if (figuresWithAlpha[i].Alpha != figuresWithAlpha[i - 1].Alpha)
                                return -1;
                        return figuresWithAlpha[0].Alpha;
                    }
                    else
                        return -1;
                }
            }
            set
            {
                if (UseRealAlpha)
                    base.Alpha = value;
                else
                {
                    foreach (Figure f in childFigs)
                        if (f is IAlphaBlendable)
                            ((IAlphaBlendable)f).Alpha = value;
                }
            }
        }

        DRect GetBoundingBox()
        {
            DRect r = DGeom.BoundingBoxOfRotatedRect(childFigs[0].Rect, childFigs[0].Rotation);
            foreach (Figure f in childFigs)
            {
                DRect r2 = DGeom.BoundingBoxOfRotatedRect(f.Rect, f.Rotation);
                r = r.Union(r2);
            }
            return r;
        }
        
        public override DRect GetSelectRect()
        {
            DRect r = DGeom.BoundingBoxOfRotatedRect(childFigs[0].Rect, childFigs[0].Rotation);
            foreach (Figure f in childFigs)
            {
                DRect r2 = DGeom.BoundingBoxOfRotatedRect(f.GetSelectRect(), f.Rotation);               
                r = r.Union(r2);
            }
            return r;
        }
        
        public override void BeforeResize()
        {
            foreach (Figure f in childFigs)
                f.BeforeResize();
        }

        public override void AfterResize()
        {
            foreach (Figure f in childFigs)
                f.AfterResize();
        }
    }

    public class CompositedExampleFigure : RectbaseFigure
    {
        /* An example figure that show how one can use the CompositingMode
         * to do a decent alpha blend.
         * However this is a bit slower and does not blend the edges of the 
         * bitmap with the canvas when AntiAlias is turned on.
         * 
         * Needs more work/thinking...
         */

        protected override void PaintBody(DGraphics dg)
        {
            if (Width > 0 && Height > 0)
            {
                DBitmap bmp = GraphicsHelper.MakeBitmap(Width, Height);
                DGraphics bmpGfx = GraphicsHelper.MakeGraphics(bmp);
                bmpGfx.AntiAlias = dg.AntiAlias;
                bmpGfx.CompositingMode = DCompositingMode.SourceCopy;
                bmpGfx.FillRect(1, 1, Width - 2, Height - 2, DColor.Blue, Alpha);
                bmpGfx.FillRect(1, 1, 2 * Width / 3, 2 * Height / 3, DColor.Red, Alpha);
                bmpGfx.FillRect(Width / 3, Height / 3, 2 * Width / 3 - 1, 2 * Height / 3 - 1, DColor.Green, Alpha);
                dg.DrawBitmap(bmp, Rect, Alpha);
                bmpGfx.Dispose();
                bmp.Dispose();
            }
        }
    }
}
