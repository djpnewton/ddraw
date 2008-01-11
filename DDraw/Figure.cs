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
        DStrokeStyle StrokeStyle
        {
            get;
            set;
        }
        DStrokeJoin StrokeJoin
        {
            get;
            set;
        }
        DStrokeCap StrokeCap
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
        bool Bold
        {
            get;
            set;
        }
        bool Italics
        {
            get;
            set;
        }
        bool Underline
        {
            get;
            set;
        }
        bool Strikethrough
        {
           get;
           set;
        }
    }

    public interface IChildFigureable
    {
        List<Figure> ChildFigures
        {
            get;
            set;
        }
    }

    public interface ILineSegment
    {
        DPoint Pt1
        {
            get;
            set;
        }
        DPoint Pt2
        {
            get;
            set;
        }
    }

    public interface IMarkable
    {
        double MarkerSize
        {
            get;
        }
        DMarker StartMarker
        {
            get;
            set;
        }
        DMarker EndMarker
        {
            get;
            set;
        }
        DPoints GetStartMarkerPoints();
        DPoints GetEndMarkerPoints();
        DRect GetStartMarkerRect();
        DRect GetEndMarkerRect();
    }

    public static class MarkerHelper
    {
        public static DPoints MarkerPoints(DMarker marker, DPoint center, double angle, double size)
        {
            double hsz = size / 2;
            DPoints pts = new DPoints();
            DPoint pt;
            switch (marker)
            {
                case DMarker.Arrow:
                    pt = DGeom.PointFromAngle(center, angle, hsz);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle + DGeom.HalfPi, hsz);
                    pt = DGeom.PointFromAngle(pt, angle + Math.PI, size);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle - DGeom.HalfPi, size);
                    pts.Add(pt);
                    break;
                case DMarker.Dot:
                    int n = (int)Math.Round(size);
                    if (n < 4)
                        n = 4;
                    double angleSegment = (Math.PI * 2) / n;
                    for (int i = 0; i < n; i++)
                        pts.Add(DGeom.PointFromAngle(center, angle + angleSegment * i, hsz));
                    break;
                case DMarker.Square:
                    pt = DGeom.PointFromAngle(center, angle, hsz);
                    pt = DGeom.PointFromAngle(pt, angle + DGeom.HalfPi, hsz);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle + Math.PI, size);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle - DGeom.HalfPi, size);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle, size);
                    pts.Add(pt);
                    break;
                case DMarker.Diamond:
                    pt = DGeom.PointFromAngle(center, angle, hsz);
                    pts.Add(pt); 
                    pt = DGeom.PointFromAngle(pt, angle + DGeom.HalfPi, hsz);
                    pt = DGeom.PointFromAngle(pt, angle + Math.PI, hsz);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle + Math.PI, hsz);
                    pt = DGeom.PointFromAngle(pt, angle - DGeom.HalfPi, hsz);
                    pts.Add(pt);
                    pt = DGeom.PointFromAngle(pt, angle - DGeom.HalfPi, hsz);
                    pt = DGeom.PointFromAngle(pt, angle, hsz);
                    pts.Add(pt);
                    break;
            }
            return pts;
        }

        public static DPoints MarkerPoints(DMarker marker, DPoint center, DPoint fromPoint, double size)
        {
            return MarkerPoints(marker, center, DGeom.AngleBetweenPoints(fromPoint, center) - DGeom.HalfPi, size);
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
        double rotation;
        public virtual double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
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

        protected void ApplyTransforms(DGraphics dg)
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
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
                // draw resize handle
                r = GetResizeHandleRect();
                dg.FillEllipse(r, DColor.Red);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DStrokeStyle.Solid);
                // draw rotate handle
                r = GetRotateHandleRect();
                DPoint p1 = r.Center;
                DPoint p2 = p1.Offset(0, 3 * HANDLE_SZ * Scale - S_INDENT * Scale);
                dg.DrawLine(p1, p2, DColor.White, 1, DStrokeStyle.Solid, Scale, DStrokeCap.Butt);
                dg.DrawLine(p1, p2, DColor.Black, 1, DStrokeStyle.Dot, Scale, DStrokeCap.Butt);
                dg.FillEllipse(r, DColor.Blue);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DStrokeStyle.Solid);
                //r = GetEncompassingRect();
                //dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale);
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

    public abstract class RectbaseFigure : Figure, IAlphaBlendable
    {
        public RectbaseFigure()
        { }

        public RectbaseFigure(DRect rect, double rotation) : base(rect, rotation)
        { }

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

    public class RectFigure : RectbaseFigure, IFillable, IStrokeable
    {
        public RectFigure()
        { }

        public RectFigure(DRect rect, double rotation) : base(rect, rotation)
        { }

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
            dg.FillRect(X, Y, Width, Height, fill, Alpha);
            dg.DrawRect(X, Y, Width, Height, stroke, Alpha, strokeWidth, strokeStyle, strokeJoin);
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
        DStrokeStyle strokeStyle = DStrokeStyle.Solid;
        public DStrokeStyle StrokeStyle
        {
            get { return strokeStyle; }
            set { strokeStyle = value; }
        }
        DStrokeJoin strokeJoin = DStrokeJoin.Mitre;
        public DStrokeJoin StrokeJoin
        {
            get { return strokeJoin; }
            set { strokeJoin = value; }
        }
        DStrokeCap strokeCap = DStrokeCap.Butt;
        public DStrokeCap StrokeCap
        {
            get { return strokeCap; }
            set { strokeCap = value; }
        }
        public DRect RectInclStroke
        {
            get { return StrokeHelper.RectIncludingStrokeWidth(Rect, strokeWidth); }
        }
        #endregion
    }

    public class EllipseFigure : RectFigure
    {
        public EllipseFigure()
        { }

        public EllipseFigure(DRect rect, double rotation) : base(rect, rotation)
        { }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInEllipse(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
            dg.FillEllipse(X, Y, Width, Height, Fill, Alpha);
            dg.DrawEllipse(X, Y, Width, Height, Stroke, Alpha, StrokeWidth, StrokeStyle);
        }
    }

    public abstract class LinebaseFigure : Figure, IStrokeable, IMarkable, IAlphaBlendable
    {
        public abstract void AddPoint(DPoint pt);

        public abstract DPoints Points
        {
            get;
        }

        public override DRect GetSelectRect()
        {
            // add in stroke width spacing
            DRect r = StrokeHelper.SelectRectIncludingStrokeWidth(base.GetSelectRect(), StrokeWidth);
            // add in marker spacing
            double i = S_INDENT * Scale;
            if (startMarker != DMarker.None)
                r = r.Union(GetStartMarkerRect().Offset(-i, -i).Inflate(i + i, i + i));
            if (endMarker != DMarker.None)
                r = r.Union(GetEndMarkerRect().Offset(-i, -i).Inflate(i + i, i + i));
            // return rect
            return r;
        }

        public override DRect GetEncompassingRect()
        {
            return base.GetEncompassingRect().Union(GetStartMarkerRect()).Union(GetEndMarkerRect());
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (DGeom.PointInPolyline(pt, Points, StrokeWidth / 2))
                return DHitTest.Body;
            else if (DGeom.PointInPolygon(pt, GetStartMarkerPoints()) || DGeom.PointInPolygon(pt, GetEndMarkerPoints()))
                return DHitTest.Body;
            return DHitTest.None;
        }

        /*protected override DHitTest _HitTest(DPoint pt)
        {
            DHitTest ht = base._HitTest(pt);
            if (ht == DHitTest.None)
            {
                if (DGeom.PointInPolygon(pt, GetStartMarkerPoints()) || DGeom.PointInPolygon(pt, GetEndMarkerPoints()))
                    return DHitTest.Body;
            }
            return ht;
        }*/

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
        DStrokeStyle strokeStyle = DStrokeStyle.Solid;
        public DStrokeStyle StrokeStyle
        {
            get { return strokeStyle; }
            set { strokeStyle = value; }
        }
        DStrokeJoin strokeJoin = DStrokeJoin.Round;
        public DStrokeJoin StrokeJoin
        {
            get { return strokeJoin; }
            set { strokeJoin = value; }
        }
        DStrokeCap strokeCap = DStrokeCap.Round;
        public DStrokeCap StrokeCap
        {
            get { return strokeCap; }
            set { strokeCap = value; }
        }
        public DRect RectInclStroke
        {
            get { return StrokeHelper.RectIncludingStrokeWidth(Rect, strokeWidth); }
        }
        #endregion

        #region IMarkable Members
        public double MarkerSize
        {
            get { return strokeWidth * 2.5; }
        }
        DMarker startMarker = DMarker.None;
        public DMarker StartMarker
        {
            get { return startMarker; }
            set { startMarker = value; }
        }
        DMarker endMarker = DMarker.None;
        public DMarker EndMarker
        {
            get { return endMarker; }
            set { endMarker = value; }
        }
        public abstract DPoints GetStartMarkerPoints();
        public abstract DPoints GetEndMarkerPoints();
        public DRect GetStartMarkerRect()
        {
            return GetStartMarkerPoints().Bounds();
        }
        public DRect GetEndMarkerRect()
        {
            return GetEndMarkerPoints().Bounds();
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

    public abstract class LineSegmentbaseFigure : LinebaseFigure, ILineSegment
    {
        DPoint pt1, pt2;
        public DPoint Pt1
        {
            get { return pt1; }
            set { pt1 = value; }
        }
        public DPoint Pt2
        {
            get { return pt2; }
            set { pt2 = value; }
        }

        public override double X
        {
            get
            {
                if (pt1 != null && pt2 != null)
                    return Math.Min(pt1.X, pt2.X);
                else
                    return 0;
            }
            set
            {
                double dX = value - X;
                pt1.X += dX;
                pt2.X += dX;
            }
        }

        public override double Y
        {
            get
            {
                if (pt1 != null && pt2 != null)
                    return Math.Min(pt1.Y, pt2.Y);
                else return 0;
            }
            set
            {
                double dY = value - Y;
                pt1.Y += dY;
                pt2.Y += dY;
            }
        }

        public override double Width
        {
            get
            {
                if (pt1 != null && pt2 != null)
                    return Math.Abs(pt1.X - pt2.X);
                else
                    return 0;
            }
            set
            {
                if (pt1.X > pt2.X)
                    pt1.X = pt2.X + value;
                else
                    pt2.X = pt1.X + value;
            }
        }

        public override double Height
        {
            get
            {
                if (pt1 != null && pt2 != null)
                    return Math.Abs(pt1.Y - pt2.Y);
                else
                    return 0;
            }
            set
            {
                if (pt1.Y > pt2.Y)
                    pt1.Y = pt2.Y + value;
                else
                    pt2.Y = pt1.Y + value;
            }
        }

        public override double Rotation
        {
            get { return 0; }
            set
            {
                DPoint c = Rect.Center;
                pt1 = DGeom.RotatePoint(pt1, c, value);
                pt2 = DGeom.RotatePoint(pt2, c, value);
            }
        }

        public override void AddPoint(DPoint pt)
        {
            if (pt1 == null)
                pt1 = pt;
            else
                pt2 = pt;
        }

        public override DPoints Points
        {
            get 
            {
                DPoints pts = new DPoints();
                if (pt1 != null)
                    pts.Add(pt1);
                if (pt2 != null)
                    pts.Add(pt2);
                return pts;
            }
        }

        public DRect GetPt1HandleRect()
        {
            double hs = HANDLE_SZ * Scale;
            return new DRect(pt1.X - hs, pt1.Y - hs, hs + hs, hs + hs);
        }

        public DRect GetPt2HandleRect()
        {
            double hs = HANDLE_SZ * Scale;
            return new DRect(pt2.X - hs, pt2.Y - hs, hs + hs, hs + hs);
        }
        
        public override DRect GetEncompassingRect()
        {
            if (Selected)
                return base.GetEncompassingRect().Union(GetPt1HandleRect().Union(GetPt2HandleRect()));
            else
                return base.GetEncompassingRect();
        }

        public override void PaintSelectionChrome(DGraphics dg)
        {
            if (Selected)
            {
                // save current transform
                DMatrix m = dg.SaveTransform();
                // apply transform
                ApplyTransforms(dg);
                // draw pt1 handle
                DRect r = GetPt1HandleRect();
                dg.FillEllipse(r.X, r.Y, r.Width, r.Height, DColor.Red, 1);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DStrokeStyle.Solid);
                // draw pt2 handle
                r = GetPt2HandleRect();
                dg.FillEllipse(r.X, r.Y, r.Width, r.Height, DColor.Red, 1);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale, DStrokeStyle.Solid);
                // view outline
                //dg.DrawRect(GetEncompassingRect(), DColor.Black);
                // load previous transform
                dg.LoadTransform(m);
            }
        }

        public override DHitTest SelectHitTest(DPoint pt)
        {
            if (Selected && DGeom.PointInLine(pt, pt1, pt2, HANDLE_SZ * Scale))
                return DHitTest.SelectRect;
            return DHitTest.None;
        }

        public override DHitTest ResizeHitTest(DPoint pt)
        {
            if (Selected)
            {
                if (DGeom.PointInRect(pt, GetPt1HandleRect()))
                    return DHitTest.ReposLinePt1;
                else if (DGeom.PointInRect(pt, GetPt2HandleRect()))
                    return DHitTest.ReposLinePt2;
                return DHitTest.None;
            }
            return DHitTest.None;
        }

        public override DHitTest RotateHitTest(DPoint pt)
        {
            return DHitTest.None;
        }
    }

    public class LineFigure : LineSegmentbaseFigure
    {
        public override DPoints GetStartMarkerPoints()
        {
            if (Pt1 != null && Pt2 != null)
                return MarkerHelper.MarkerPoints(StartMarker, Pt1, Pt2, MarkerSize);
            else
                return new DPoints();
        }
        public override DPoints GetEndMarkerPoints()
        {
            if (Pt1 != null && Pt2 != null)
                return MarkerHelper.MarkerPoints(EndMarker, Pt2, Pt1, MarkerSize);
            else
                return new DPoints();
        }

        public LineFigure()
        { }

        public LineFigure(DPoint pt1, DPoint pt2)
        {
            Pt1 = pt1;
            Pt2 = pt2;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (Pt1 != null && Pt2 != null)
            {
                dg.DrawLine(Pt1, Pt2, Stroke, Alpha, StrokeStyle, StrokeWidth, StrokeCap);
                if (StartMarker != DMarker.None)
                    dg.FillPolygon(GetStartMarkerPoints(), Stroke, Alpha);
                if (EndMarker != DMarker.None)
                    dg.FillPolygon(GetEndMarkerPoints(), Stroke, Alpha);
            }
        }
    }

    public abstract class PolylinebaseFigure : LinebaseFigure
    {
        DPoints points;
        public override DPoints Points
        {
            get { return points; }
        }

        public void SetPoints(DPoints pts)
        {
                points = pts;
                Rect = points.Bounds();
        }

        public override void AddPoint(DPoint pt)
        {
            if (points == null)
                points = new DPoints();
            points.Add(pt);
            SetPoints(points); // to set the bounds (maybe should fix this)
        }

        public override double X
        {
            set
            {
                double dX = value - X;
                foreach (DPoint pt in Points)
                    pt.X += dX;
                SetPoints(Points);
            }
        }

        public override double Y
        {
            set
            {
                double dY = value - Y;
                foreach (DPoint pt in Points)
                    pt.Y += dY;
                SetPoints(Points);
            }
        }

        public override double Width
        {
            set
            {
                double scale = value / Width;
                foreach (DPoint pt in points)
                    pt.X += (pt.X - X) * (scale - 1);
                SetPoints(Points);
            }
        }

        public override double Height
        {
            set
            {
                double scale = value / Height;
                foreach (DPoint pt in points)
                    pt.Y += (pt.Y - Y) * (scale - 1);
                SetPoints(Points);
            }
        }

        DPoint beforeResizeSize;

        public override void BeforeResize()
        {
            beforeResizeSize = new DPoint(Width, Height);
        }

        public override void AfterResize()
        {
            StrokeWidth *= ((Width / beforeResizeSize.X) + (Height / beforeResizeSize.Y)) / 2;
        }
    }

    public class PolylineFigure : PolylinebaseFigure
    {
        public override DPoints GetStartMarkerPoints()
        {
            if (Points.Count > 1)
                return MarkerHelper.MarkerPoints(StartMarker, Points[0], Points[1], MarkerSize);
            else
                return new DPoints();
        }
        public override DPoints GetEndMarkerPoints()
        {
            if (Points.Count > 1)
                return MarkerHelper.MarkerPoints(EndMarker, Points[Points.Count - 1], Points[Points.Count - 2], MarkerSize);
            else
                return new DPoints();
        }

        public PolylineFigure(DPoints points)
        {
            SetPoints(points);
        }

        public PolylineFigure()
        { }

        protected override void PaintBody(DGraphics dg)
        {
            if (Points.Count > 1)
            {
                dg.DrawPolyline(Points, Stroke, Alpha, StrokeWidth, StrokeStyle, StrokeJoin, StrokeCap);
                if (StartMarker != DMarker.None)
                    dg.FillPolygon(GetStartMarkerPoints(), Stroke, Alpha);
                if (EndMarker != DMarker.None)
                    dg.FillPolygon(GetEndMarkerPoints(), Stroke, Alpha);
            }
        }
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

        bool bold = false;
        public bool Bold
        {
            get { return bold; }
            set
            {
                bold = value;
                Text = text;
            }
        }
        bool italics = false;
        public bool Italics
        {
            get { return italics; }
            set
            {
                italics = value; 
                Text = text;
            }
        }
        bool underline = false;
        public bool Underline
        {
            get { return underline; }
            set
            {
                underline = value; 
                Text = text;
            }
        }
        bool strikethrough = false;
        public bool Strikethrough
        {
            get { return strikethrough; }
            set
            {
                strikethrough = value; 
                Text = text;
            }
        }

        private string text = null;
        public string Text
        {
            get { return text; }
            set
            {
                DPoint sz = GraphicsHelper.MeasureText(value, fontName, FontSize, bold, italics, underline, strikethrough);
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

        public TextFigure(DPoint pt, string text, double rotation)
        {
            TopLeft = pt;
            Text = text;
            Rotation = rotation;
        }

        public TextFigure(DPoint pt, string text, string fontName, double fontSize, double rotation)
        {
            TopLeft = pt;
            this.fontName = fontName;
            this.fontSize = fontSize;
            Text = text;
            Rotation = rotation;
        }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawText(Text, fontName, fontSize, bold, italics, underline, strikethrough, Rect.TopLeft, fill, Alpha);
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
            set { tf.FontSize = value; }
        }

        public bool Bold
        {
            get { return tf.Bold; }
            set { tf.Bold = value; }
        }
        public bool Italics
        {
            get { return tf.Italics; }
            set { tf.Italics = value; }
        }
        public bool Underline
        {
            get { return tf.Underline; }
            set { tf.Underline = value; }
        }
        public bool Strikethrough
        {
            get { return tf.Strikethrough; }
            set { tf.Strikethrough = value; }
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

        // cursor position can range from 0 to Text.Length
        int cursorPosition;

        string[] Lines
        {
            get { return Text.Split('\n'); }
        }

        public TextEditFigure(TextFigure tf)
        {
            this.tf = tf;
            cursorPosition = Text.Length;
        }

        public TextEditFigure(DPoint pt, TextFigure tf) : this(tf)
        {
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
            // paint cursor
            string[] lines = Lines;
            DPoint pt = tf.Rect.TopLeft;
            double height;
            DPoint cpt = MeasureCursorPosition(lines, out height);
            dg.DrawLine(new DPoint(pt.X + cpt.X, pt.Y + cpt.Y), new DPoint(pt.X + cpt.X, pt.Y + cpt.Y + height), DColor.Black, 1, DStrokeStyle.Solid, 2, DStrokeCap.Butt);
        }

        DPoint MeasureCursorPosition(string[] lines, out double height)
        {
            // find the x,y position of the cursor
            int n = cursorPosition;
            height = GraphicsHelper.MeasureText(Text, FontName, FontSize, Bold, Italics, Underline, Strikethrough).Y / lines.Length;
            if (cursorPosition >= 0)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (n <= line.Length)
                    {
                        DPoint sz = GraphicsHelper.MeasureText(line.Substring(0, n), FontName, FontSize, Bold, Italics, Underline, Strikethrough);
                        return new DPoint(sz.X, height * i);
                    }
                    n -= line.Length + 1;
                }
            }
            return new DPoint(0, 0);
        }

        int FindLineByCharacter(string[] lines, int textChar, out int lineChar)
        {
            // find the line a character is on based on the index (also find the character index on the line)
            int n = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (n + line.Length >= textChar)
                {
                    lineChar = textChar - n;
                    return i;
                }
                n += line.Length;
                n += 1; // take new line char into account
            }
            lineChar = 0;
            return 0;
        }

        int FindEquivCharacterPosition(string newLine, int currentChar, double currentXPos)
        {
            // given a character index and x position along a line, 
            // find the eqvivalent character index on a different line
            if (currentChar < 1)
                return 0;

            double newXPos = 0;
            int n = 1;
            while (newXPos < currentXPos)
            {
                if (n >= newLine.Length)
                    return newLine.Length;
                newXPos = GraphicsHelper.MeasureText(newLine.Substring(0, n), FontName, FontSize, Bold, Italics, Underline, Strikethrough).X;
                n += 1;
            }
            return n - 1;
        }

        int CharNumberChange(string[] lines, int currentLine, int newLine, int currentLineChar, int newLineChar)
        {
            // given a set of lines and a current character line and position,
            // calculate the delta of the index compared to a new character (new line and index on that line)
            if (newLine < currentLine)
                return -(currentLineChar + 1 + lines[newLine].Length - newLineChar);
            else if (newLine > currentLine)
                return lines[currentLine].Length - currentLineChar + 1 + newLineChar;
            else
                return newLineChar - currentLineChar;
        }

        public void MoveCursor(DKeys k)
        {
            switch (k)
            {
                case DKeys.Left:
                    if (cursorPosition > 0) cursorPosition -= 1;
                    break;
                case DKeys.Right:
                    if (cursorPosition < Text.Length) cursorPosition += 1;
                    break;
                case DKeys.Up:
                    string[] lines = Lines;
                    int currentlineCharNo;
                    int lineNo = FindLineByCharacter(lines, cursorPosition, out currentlineCharNo);
                    int newLineNo;
                    if (lineNo > 0 && k == DKeys.Up)
                        newLineNo = lineNo - 1;
                    else if (lineNo < lines.Length - 1 && k == DKeys.Down)
                        newLineNo = lineNo + 1;
                    else
                        break;
                    double height;
                    DPoint cpt = MeasureCursorPosition(lines, out height);
                    int newLineCharNo = FindEquivCharacterPosition(lines[newLineNo], currentlineCharNo, cpt.X);
                    cursorPosition += CharNumberChange(lines, lineNo, newLineNo, currentlineCharNo, newLineCharNo);                    
                    break;
                case DKeys.Down:
                    goto case DKeys.Up;
            }
        }

        public void InsertAtCursor(char c)
        {
            if (cursorPosition < Text.Length)
                Text = Text.Insert(cursorPosition, c.ToString());
            else
                Text = string.Concat(Text, c);
            MoveCursor(DKeys.Right);
        }

        public void BackspaceAtCursor()
        {
            if (cursorPosition > 0)
            {
                Text = Text.Remove(cursorPosition - 1, 1);
                MoveCursor(DKeys.Left);
            }
        }

        public void DeleteAtCursor()
        {
            if (cursorPosition < Text.Length)
                Text = Text.Remove(cursorPosition, 1);
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

        List<Figure> childFigs;
        public List<Figure> ChildFigures
        {
            get  { return new List<Figure>(childFigs.ToArray()); }
            set
            {
                childFigs = value;
                CreateOriginalRects();
            }
        }

        DRect[] originalChildRects;
        DRect originalRect;

        public GroupFigure(List<Figure> figs)
        {
            System.Diagnostics.Debug.Assert(figs != null, "figs is not assigned");
            System.Diagnostics.Debug.Assert(figs.Count > 1, "figs.Length is less than 2");
            // make new figure list
            childFigs = new List<Figure>();
            foreach (Figure f in figs)
                childFigs.Add(f);
            // store starting dimensions for scaling later on
            CreateOriginalRects();
        }

        void CreateOriginalRects()
        {
            originalChildRects = new DRect[childFigs.Count];
            int i = 0;
            foreach (Figure f in childFigs)
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

        public DRect GetChildBoundingBox(Figure f)
        {
            return DGeom.BoundingBoxOfRotatedRect(f.GetEncompassingRect(), Rotation, Rect.Center);
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
