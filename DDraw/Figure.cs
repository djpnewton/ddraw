using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public interface IDimension
    {
        double X
        {
            get;
            set;
        }
        double Y
        {
            get;
            set;
        }
        double Width
        {
            get;
            set;
        }
        double Height
        {
            get;
            set;
        }
        double Rotation
        {
            get;
            set;
        }
        bool LockAspectRatio
        {
            get;
            set;
        }
    }

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
        double SwHalf
        {
            get;
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

    public enum DImagePosition { Stretch, Normal, Center, Tile, StretchWithAspectRatio }

    public interface IImage
    {
        DBitmap Bitmap
        {
            get;
        }

        byte[] ImageData
        {
            get;
            set;
        }

        DImagePosition Position
        {
            get;
            set;
        }

        string FileName
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
        UndoRedoList<Figure> ChildFigures
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

    public interface IPolyline
    {
        DPoints Points
        {
            get;
            set;
        }

        void AddPoint(DPoint pt);
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

    public interface IGlyphable
    {
        List<IGlyph> Glyphs
        {
            get;
            set;
        }

        bool GlyphsVisible
        {
            get;
            set;
        }

        void PaintGlyphs(DGraphics dg);

        DHitTest GlyphHitTest(DPoint pt, out IGlyph glyph);
    }

    public abstract class Figure: IDimension, ISelectable, IGlyphable
    {
        public double _controlScale = 1;
        public virtual double MinSize
        {
            get { return 0.1; }
        }
        public bool ClickEvent = false;

        UndoRedo<double> _x = new UndoRedo<double>(0);
        public virtual double X
        {
            get { return _x.Value; }
            set { if (value != _x.Value) _x.Value = value; }
        }
        UndoRedo<double> _y = new UndoRedo<double>(0);
        public virtual double Y
        {
            get { return _y.Value; }
            set { if (value != _y.Value) _y.Value = value; }
        }
        UndoRedo<double> _width = new UndoRedo<double>(0);
        public virtual double Width
        {
            get { return _width.Value; }
            set { if (value != _width.Value) _width.Value = value; }
        }
        UndoRedo<double> _height = new UndoRedo<double>(0);
        public virtual double Height
        {
            get { return _height.Value; }
            set { if (value != _height.Value) _height.Value = value; }
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
            set { Width = value - X; }
        }
        public virtual double Bottom
        {
            get { return Y + Height; }
            set { Height = value - Y; }
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
                if (value.X != _x.Value) _x.Value = value.X;
                if (value.Y != _y.Value) _y.Value = value.Y;
                if (value.Width != _width.Value) _width.Value = value.Width;
                if (value.Height != _height.Value) _height.Value = value.Height;
            }
        }
        UndoRedo<double> _rotation = new UndoRedo<double>(0);
        public virtual double Rotation
        {
            get { return _rotation.Value; }
            set { if (value != _rotation.Value) _rotation.Value = value; }
        }

        bool lockAspectRatio = false;
        public virtual bool LockAspectRatio
        {
            get { return lockAspectRatio; }
            set { lockAspectRatio = value; }
        }

        bool useRealAlpha = true;
        public virtual bool UseRealAlpha
        {
            get { return useRealAlpha; }
            set { useRealAlpha = value; }
        }

        UndoRedoDictionary<string, string> _userAttrs = new UndoRedoDictionary<string, string>();
        public UndoRedoDictionary<string, string> UserAttrs
        {
            get { return _userAttrs; }
        }

        public Figure()
        { }

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

        public virtual DHitTest HitTest(DPoint pt, List<Figure> children, out IGlyph glyph)
        {
            pt = RotatePointToFigure(pt);
            DHitTest res = GlyphHitTest(pt, out glyph);
            if (res == DHitTest.None)
            {
                res = RotateHitTest(pt);
                if (res == DHitTest.None)
                {
                    res = ResizeHitTest(pt);
                    if (res == DHitTest.None)
                    {
                        res = SelectHitTest(pt);
                        if (res == DHitTest.None)
                            res = BodyHitTest(pt, children);
                    }
                }
            }
            return res;
        }

        protected abstract DHitTest BodyHitTest(DPoint pt, List<Figure> children);

        protected void ApplyTransforms(DGraphics dg)
        {
            dg.Rotate(Rotation, Rect.Center);
        }

        public void Paint(DGraphics dg)
        {
            DMatrix m = dg.SaveTransform();
            ApplyTransforms(dg);
            PaintBody(dg);
            if (glyphsVisible)
                PaintGlyphs(dg);
            dg.LoadTransform(m);
        }

        protected abstract void PaintBody(DGraphics dg);

        public bool Contains(Figure childFigure)
        {
            // rotate childFigure's rect by its rotation
            DRect r = DGeom.BoundingBoxOfRotatedRect(childFigure.Rect, childFigure.Rotation, childFigure.Rect.Center);
            // rotate result by the reverse rotation of this figures rect
            r = DGeom.BoundingBoxOfRotatedRect(r, -Rotation, Rect.Center);
            // return whether result lies within this figures rect
            return Rect.Contains(r);
        }

        public virtual void BeforeResize(){}
        public virtual void AfterResize(){}

        #region ISelectable Members

        public static int _selectIndent = 4;
        public static int _handleSize = 5;
        public static int _handleBorder = 0;
        public static int _rotateHandleStemLength = 5;
        public static int HandleSize
        {
            get { return _handleSize + _handleBorder; }
        }

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
                double selectRectY = r.Y;
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.White, 1, _controlScale);
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
                // draw resize handle
                double hb = _handleBorder *_controlScale;
                double hb2 = hb + hb;
                r = GetResizeHandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                dg.FillEllipse(r, DColor.Red);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                // draw rotate handle
                r = GetRotateHandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                DPoint p1 = r.Center;
                DPoint p2 = new DPoint(p1.X, selectRectY);
                dg.DrawLine(p1, p2, DColor.White, 1, DStrokeStyle.Solid, _controlScale, DStrokeCap.Butt);
                dg.DrawLine(p1, p2, DColor.Black, 1, DStrokeStyle.Dot, _controlScale, DStrokeCap.Butt);
                dg.FillEllipse(r, DColor.Blue);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                //r = GetEncompassingRect();
                //dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, Scale);
                // load previous transform
                dg.LoadTransform(m);
            }
        }

        public virtual DRect GetSelectRect()
        {
            double i = _selectIndent * _controlScale;
            return Rect.Offset(-i, -i).Inflate(i + i, i + i);
        }

        public virtual DRect GetResizeHandleRect()
        {
            DRect selectRect = GetSelectRect();
            double hs = HandleSize * _controlScale;
            return new DRect(selectRect.Right - hs, selectRect.Bottom - hs, hs + hs, hs + hs);
        }

        public virtual DRect GetRotateHandleRect()
        {
            DRect selectRect = GetSelectRect();
            double hs = HandleSize * _controlScale;
            return new DRect(selectRect.Center.X - hs, selectRect.Y - hs - hs - _rotateHandleStemLength, hs + hs, hs + hs);
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

        #region IGlyphable Members

        List<IGlyph> glyphs = null;
        public List<IGlyph> Glyphs
        {
            get { return glyphs; }
            set { glyphs = value; }
        }

        bool glyphsVisible = false;
        public bool GlyphsVisible
        {
            get { return glyphsVisible; }
            set { glyphsVisible = value; }
        }

        public virtual void PaintGlyphs(DGraphics dg)
        {
            if (glyphs != null && glyphs.Count > 0)
                foreach (IGlyph g in glyphs)
                    if (g.IsVisible(selected))
                        g.Paint(dg, this, _controlScale);
        }

        public virtual DHitTest GlyphHitTest(DPoint pt, out IGlyph glyph)
        {
            if (glyphs != null && glyphs.Count > 0)
                foreach (IGlyph g in glyphs)
                {
                    DHitTest ht = g.HitTest(pt, this, _controlScale);
                    if (ht != DHitTest.None)
                    {
                        glyph = g;
                        return ht;
                    }
                }
            glyph = null;
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

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (DGeom.PointInRect(pt, Rect))
                return DHitTest.Body;
            return DHitTest.None;
        }

        #region IAlphaBlendable Members
        UndoRedo<double> _alpha = new UndoRedo<double>(1);
        public virtual double Alpha
        {
            get { return _alpha.Value; }
            set { if (value != _alpha.Value) _alpha.Value = value; }
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
            return StrokeHelper.SelectRectIncludingStrokeWidth(base.GetSelectRect(), StrokeWidth);
        }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (DGeom.PointInRect(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (UseRealAlpha && Alpha != 1 && StrokeWidth > 0)
            {
                dg.StartGroup(X, Y, Width + StrokeWidth, Height + StrokeWidth, SwHalf, SwHalf);
                dg.FillRect(X, Y, Width, Height, Fill, 1);
                dg.DrawRect(X, Y, Width, Height, Stroke, 1, StrokeWidth, StrokeStyle, StrokeJoin);
                dg.DrawGroup(Alpha);
            }
            else
            {
                dg.FillRect(X, Y, Width, Height, Fill, Alpha);
                dg.DrawRect(X, Y, Width, Height, Stroke, Alpha, StrokeWidth, StrokeStyle, StrokeJoin);
            }
        }

        #region IFillable Members
        UndoRedo<DColor> _fill = new UndoRedo<DColor>(DColor.Red);
        public virtual DColor Fill
        {
            get { return _fill.Value; }
            set { if (!value.Equals(_fill.Value)) _fill.Value = value; }
        }
        #endregion

        #region IStrokeable Members
        UndoRedo<DColor> _stroke = new UndoRedo<DColor>(DColor.Blue);
        public virtual DColor Stroke
        {
            get { return _stroke.Value; }
            set { if (!value.Equals(_stroke.Value)) _stroke.Value = value; }
        }
        UndoRedo<double> _strokeWidth = new UndoRedo<double>(1);
        public virtual double StrokeWidth
        {
            get { return _strokeWidth.Value; }
            set
            {
                if (value != _strokeWidth.Value)
                {
                    _strokeWidth.Value = value;
                    strokeWidthHalf = value / 2;
                }
            }
        }
        double strokeWidthHalf = 0.5;
        public double SwHalf
        {
            get { return strokeWidthHalf; }
        }
        UndoRedo<DStrokeStyle> _strokeStyle = new UndoRedo<DStrokeStyle>(DStrokeStyle.Solid);
        public virtual DStrokeStyle StrokeStyle
        {
            get { return _strokeStyle.Value; }
            set { if (value != _strokeStyle.Value) _strokeStyle.Value = value; }
        }
        UndoRedo<DStrokeJoin> _strokeJoin = new UndoRedo<DStrokeJoin>(DStrokeJoin.Mitre);
        public virtual DStrokeJoin StrokeJoin
        {
            get { return _strokeJoin.Value; }
            set { if (value != _strokeJoin.Value) _strokeJoin.Value = value; }
        }
        UndoRedo<DStrokeCap> _strokeCap = new UndoRedo<DStrokeCap>(DStrokeCap.Butt);
        public virtual DStrokeCap StrokeCap
        {
            get { return _strokeCap.Value; }
            set { if (value != _strokeCap.Value) _strokeCap.Value = value; }
        }
        public DRect RectInclStroke
        {
            get { return StrokeHelper.RectIncludingStrokeWidth(Rect, StrokeWidth); }
        }
        #endregion
    }

    public class EllipseFigure : RectFigure
    {
        public EllipseFigure()
        { }

        public EllipseFigure(DRect rect, double rotation) : base(rect, rotation)
        { }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (DGeom.PointInEllipse(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (UseRealAlpha && Alpha != 1 && StrokeWidth > 0)
            {
                dg.StartGroup(X, Y, Width + StrokeWidth, Height + StrokeWidth, SwHalf, SwHalf);
                dg.FillEllipse(X, Y, Width, Height, Fill);
                dg.DrawEllipse(X, Y, Width, Height, Stroke, 1, StrokeWidth, StrokeStyle);
                dg.DrawGroup(Alpha);
            }
            else
            {
                dg.FillEllipse(X, Y, Width, Height, Fill, Alpha);
                dg.DrawEllipse(X, Y, Width, Height, Stroke, Alpha, StrokeWidth, StrokeStyle);
            }
        }
    }

    public abstract class LinebaseFigure : Figure, IStrokeable, IMarkable, IAlphaBlendable
    {
        public static int _hitTestExtension = 0;

        public override DRect GetSelectRect()
        {
            // add in stroke width spacing
            DRect r = StrokeHelper.SelectRectIncludingStrokeWidth(base.GetSelectRect(), StrokeWidth);
            // add in marker spacing
            double i = _selectIndent * _controlScale;
            if (StartMarker != DMarker.None)
                r = r.Union(GetStartMarkerRect().Offset(-i, -i).Inflate(i + i, i + i));
            if (EndMarker != DMarker.None)
                r = r.Union(GetEndMarkerRect().Offset(-i, -i).Inflate(i + i, i + i));
            // return rect
            return r;
        }

        public override DRect GetEncompassingRect()
        {
            return base.GetEncompassingRect().Union(GetStartMarkerRect()).Union(GetEndMarkerRect());
        }

        #region IStrokeable Members
        UndoRedo<DColor> _stroke = new UndoRedo<DColor>(DColor.Blue);
        public virtual DColor Stroke
        {
            get { return _stroke.Value; }
            set { if (!value.Equals(_stroke.Value)) _stroke.Value = value; }
        }
        UndoRedo<double> _strokeWidth = new UndoRedo<double>(1);
        public virtual double StrokeWidth
        {
            get { return _strokeWidth.Value; }
            set
            {
                if (value != _strokeWidth.Value)
                {
                    _strokeWidth.Value = value;
                    strokeWidthHalf = value / 2;
                }
            }
        }
        double strokeWidthHalf = 0.5;
        public double SwHalf
        {
            get { return strokeWidthHalf; }
        }
        UndoRedo<DStrokeStyle> _strokeStyle = new UndoRedo<DStrokeStyle>(DStrokeStyle.Solid);
        public virtual DStrokeStyle StrokeStyle
        {
            get { return _strokeStyle.Value; }
            set { if (value != _strokeStyle.Value) _strokeStyle.Value = value; }
        }
        UndoRedo<DStrokeJoin> _strokeJoin = new UndoRedo<DStrokeJoin>(DStrokeJoin.Round);
        public virtual DStrokeJoin StrokeJoin
        {
            get { return _strokeJoin.Value; }
            set { if (value != _strokeJoin.Value) _strokeJoin.Value = value; }
        }
        UndoRedo<DStrokeCap> _strokeCap = new UndoRedo<DStrokeCap>(DStrokeCap.Round);
        public virtual DStrokeCap StrokeCap
        {
            get { return _strokeCap.Value; }
            set { if (value != _strokeCap.Value) _strokeCap.Value = value; }
        }
        public DRect RectInclStroke
        {
            get { return StrokeHelper.RectIncludingStrokeWidth(Rect, StrokeWidth); }
        }
        #endregion

        #region IMarkable Members
        public double MarkerSize
        {
            get { return StrokeWidth * 2.5; }
        }
        UndoRedo<DMarker> _startMarker = new UndoRedo<DMarker>(DMarker.None);
        public DMarker StartMarker
        {
            get { return _startMarker.Value; }
            set { if (value != _startMarker.Value) _startMarker.Value = value; }
        }
        UndoRedo<DMarker> _endMarker = new UndoRedo<DMarker>(DMarker.None);
        public DMarker EndMarker
        {
            get { return _endMarker.Value; }
            set { if (value != _endMarker.Value) _endMarker.Value = value; }
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
        UndoRedo<double> _alpha = new UndoRedo<double>(1);
        public virtual double Alpha
        {
            get { return _alpha.Value; }
            set { if (value != _alpha.Value) _alpha.Value = value; }
        }
        #endregion
    }

    public abstract class LineSegmentbaseFigure : LinebaseFigure, ILineSegment
    {
        UndoRedo<double> _pt1x = new UndoRedo<double>();
        UndoRedo<double> _pt1y = new UndoRedo<double>();
        public DPoint Pt1
        {
            get { return new DPoint(_pt1x.Value, _pt1y.Value); }
            set
            {
                DPoint pt = Pt1;
                if (value.X != pt.X || value.Y != pt.Y)
                {
                    _pt1x.Value = value.X;
                    _pt1y.Value = value.Y;
                }
            }
        }
        UndoRedo<double> _pt2x = new UndoRedo<double>();
        UndoRedo<double> _pt2y = new UndoRedo<double>();
        public DPoint Pt2
        {
            get { return new DPoint(_pt2x.Value, _pt2y.Value); }
            set
            {
                DPoint pt = Pt2;
                if (value.X != pt.X || value.Y != pt.Y)
                {
                    _pt2x.Value = value.X;
                    _pt2y.Value = value.Y;
                }
            }
        }

        public override double X
        {
            get { return Math.Min(Pt1.X, Pt2.X); }
            set
            {
                double dX = value - X;
                Pt1 = new DPoint(Pt1.X + dX, Pt1.Y);
                Pt2 = new DPoint(Pt2.X + dX, Pt2.Y);
            }
        }

        public override double Y
        {
            get { return Math.Min(Pt1.Y, Pt2.Y); }
            set
            {
                double dY = value - Y;
                Pt1 = new DPoint(Pt1.X, Pt1.Y + dY);
                Pt2 = new DPoint(Pt2.X, Pt2.Y + dY);
            }
        }

        public override double Width
        {
            get { return Math.Abs(Pt1.X - Pt2.X); }
            set
            {
                if (Pt1.X > Pt2.X)
                    Pt1 = new DPoint(Pt2.X + value, Pt1.Y);
                else
                    Pt2 = new DPoint(Pt1.X + value, Pt2.Y);
            }
        }

        public override double Height
        {
            get { return Math.Abs(Pt1.Y - Pt2.Y); }
            set
            {
                if (Pt1.Y > Pt2.Y)
                    Pt1 = new DPoint(Pt1.X, Pt2.Y + value);
                else
                    Pt2 = new DPoint(Pt2.X, Pt1.Y + value);
            }
        }

        public override double Rotation
        {
            get { return 0; }
            set
            {
                if (Pt1 != null && Pt2 != null)
                {
                    DPoint c = Rect.Center;
                    Pt1 = DGeom.RotatePoint(Pt1, c, value);
                    Pt2 = DGeom.RotatePoint(Pt2, c, value);
                }
            }
        }

        public DRect GetPt1HandleRect()
        {
            double hs = HandleSize * _controlScale;
            return new DRect(Pt1.X - hs, Pt1.Y - hs, hs + hs, hs + hs);
        }

        public DRect GetPt2HandleRect()
        {
            double hs = HandleSize * _controlScale;
            return new DRect(Pt2.X - hs, Pt2.Y - hs, hs + hs, hs + hs);
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
                double hb = _handleBorder * _controlScale;
                double hb2 = hb + hb;
                DRect r = GetPt1HandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                dg.FillEllipse(r.X, r.Y, r.Width, r.Height, DColor.Red, 1);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                // draw pt2 handle
                r = GetPt2HandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                dg.FillEllipse(r.X, r.Y, r.Width, r.Height, DColor.Red, 1);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                // view outline
                //dg.DrawRect(GetEncompassingRect(), DColor.Black);
                // load previous transform
                dg.LoadTransform(m);
            }
        }

        public override void PaintGlyphs(DGraphics dg)
        {
            if (Glyphs != null && Glyphs.Count > 0)
            {
                DMatrix m = dg.SaveTransform();
                ApplyTransforms(dg);
                int n = 0;
                foreach (IGlyph g in Glyphs)
                {
                    // set glyph position to "center stack"
                    DGlyphPosition temp = g.Position;
                    g.CenterStack(Glyphs.Count, n);
                    n++;
                    // do paint op
                    if (g.IsVisible(Selected))
                        g.Paint(dg, this, _controlScale);
                    // set gylph position back to original
                    g.Position = temp;
                }
                dg.LoadTransform(m);
            }
        }

        public override DHitTest GlyphHitTest(DPoint pt, out IGlyph glyph)
        {
            if (Glyphs != null && Glyphs.Count > 0)
            {
                int n = 0;
                foreach (IGlyph g in Glyphs)
                {
                    // set glyph position to "center stack"
                    DGlyphPosition temp = g.Position;
                    g.CenterStack(Glyphs.Count, n);
                    n++;
                    // do hit test
                    DHitTest ht = g.HitTest(pt, this, _controlScale);
                    // set gylph position back to original
                    g.Position = temp;
                    // return if positive result
                    if (ht != DHitTest.None)
                    {
                        glyph = g;
                        return ht;
                    }
                }
            }
            glyph = null;
            return DHitTest.None;
        }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (Pt1 != null && Pt2 != null)
            {
                if (DGeom.PointInLine(pt, Pt1, Pt2, StrokeWidth / 2 + _hitTestExtension))
                    return DHitTest.Body;
                else if (DGeom.PointInPolygon(pt, GetStartMarkerPoints()) || DGeom.PointInPolygon(pt, GetEndMarkerPoints()))
                    return DHitTest.Body;
            }
            return DHitTest.None;
        }

        public override DHitTest SelectHitTest(DPoint pt)
        {
            if (Pt1 != null && Pt2 != null)
            {
                if (Selected && DGeom.PointInLine(pt, Pt1, Pt2, HandleSize * _controlScale))
                    return DHitTest.SelectRect;
            }
            return DHitTest.None;
        }

        public override DHitTest ResizeHitTest(DPoint pt)
        {
            if (Selected)
            {
                if (Pt1 != null && Pt2 != null)
                {
                    if (DGeom.PointInRect(pt, GetPt1HandleRect()))
                        return DHitTest.ReposLinePt1;
                    else if (DGeom.PointInRect(pt, GetPt2HandleRect()))
                        return DHitTest.ReposLinePt2;
                }
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
            return MarkerHelper.MarkerPoints(StartMarker, Pt1, Pt2, MarkerSize);
        }
        public override DPoints GetEndMarkerPoints()
        {
            return MarkerHelper.MarkerPoints(EndMarker, Pt2, Pt1, MarkerSize);
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
            if (UseRealAlpha && Alpha != 1 && StrokeWidth > 0 && (StartMarker != DMarker.None || EndMarker != DMarker.None))
            {
                DPoints smp;
                if (StartMarker != DMarker.None)
                    smp = GetStartMarkerPoints();
                else
                    smp = MarkerHelper.MarkerPoints(DMarker.Square, Pt1, 0, MarkerSize);
                DPoints emp;
                if (EndMarker != DMarker.None)
                    emp = GetEndMarkerPoints();
                else
                    emp = MarkerHelper.MarkerPoints(DMarker.Square, Pt2, 0, MarkerSize);
                DRect R = GetEncompassingRect().Union(smp.Bounds()).Union(emp.Bounds());

                dg.StartGroup(X, Y, R.Width, R.Height, X - R.X, Y - R.Y);
                dg.DrawLine(Pt1, Pt2, Stroke, 1, StrokeStyle, StrokeWidth, StrokeCap);
                if (StartMarker != DMarker.None)
                    dg.FillPolygon(GetStartMarkerPoints(), Stroke, 1);
                if (EndMarker != DMarker.None)
                    dg.FillPolygon(GetEndMarkerPoints(), Stroke, 1);
                dg.DrawGroup(Alpha);
            }
            else
            {
                dg.DrawLine(Pt1, Pt2, Stroke, Alpha, StrokeStyle, StrokeWidth, StrokeCap);
                if (StartMarker != DMarker.None)
                    dg.FillPolygon(GetStartMarkerPoints(), Stroke, Alpha);
                if (EndMarker != DMarker.None)
                    dg.FillPolygon(GetEndMarkerPoints(), Stroke, Alpha);
            }
        }
    }

    public abstract class PolylinebaseFigure : LinebaseFigure, IPolyline
    {
        UndoRedo<DPoints> _points = new UndoRedo<DPoints>();
        public DPoints Points
        {
            get { return _points.Value; }
            set
            {
                _points.Value = new DPoints();
                foreach (DPoint pt in value)
                    _points.Value.Add(new DPoint(pt.X, pt.Y));
                Rect = _points.Value.Bounds();
            }
        }

        public void AddPoint(DPoint pt)
        {
            DPoints pts = Points;
            if (pts == null)
                pts = new DPoints();
            pts.Add(pt);
            Points = pts; // to set the bounds (maybe should fix this)
        }

        public override double X
        {
            set
            {
                if (Points != null)
                {
                    double dX = value - X;
                    foreach (DPoint pt in Points)
                        pt.X += dX;
                    Points = Points;
                }
            }
        }

        public override double Y
        {
            set
            {
                if (Points != null)
                {
                    double dY = value - Y;
                    foreach (DPoint pt in Points)
                        pt.Y += dY;
                    Points = Points;
                }
            }
        }

        public override double Width
        {
            set
            {
                if (Points != null && Points.Count > 1)
                {
                    bool flat = true;
                    DPoint lastPt = Points[0];
                    foreach (DPoint pt in Points)
                    {
                        if (pt.X != lastPt.X)
                        {
                            flat = false;
                            break;
                        }
                        lastPt = pt;
                    }
                    if (!flat)
                    {
                        double scale = value / Width;
                        foreach (DPoint pt in Points)
                            pt.X += (pt.X - X) * (scale - 1);
                        Points = Points;
                    }
                }
            }
        }

        public override double Height
        {
            set
            {
                if (Points != null && Points.Count > 1)
                {
                    bool flat = true;
                    DPoint lastPt = Points[0];
                    foreach (DPoint pt in Points)
                    {
                        if (pt.Y != lastPt.Y)
                        {
                            flat = false;
                            break;
                        }
                        lastPt = pt;
                    }
                    if (!flat)
                    {
                        double scale = value / Height;
                        foreach (DPoint pt in Points)
                            pt.Y += (pt.Y - Y) * (scale - 1);
                        Points = Points;
                    }
                }
            }
        }

        const int StrokeWidthFactorNull = -1;
        void CalcStrokeWidthFactor()
        {
            if (Width == 0 && Height == 0)
                strokeWidthFactor = StrokeWidthFactorNull;
            else
                strokeWidthFactor = StrokeWidth / (Width + Height);
        }
        double strokeWidthFactor = StrokeWidthFactorNull;

        public override double StrokeWidth
        {
            get { return base.StrokeWidth; }
            set 
            {
                base.StrokeWidth = value;
                CalcStrokeWidthFactor();
            }
        }

        DPoint beforeResizeSize;

        public override void BeforeResize()
        {
            beforeResizeSize = new DPoint(Width, Height);
            if (strokeWidthFactor == StrokeWidthFactorNull)
                CalcStrokeWidthFactor();
        }

        public override void AfterResize()
        {
            StrokeWidth = strokeWidthFactor * (Width + Height);
        }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (Points != null)
            {
                if (DGeom.PointInPolyline(pt, Points, StrokeWidth / 2 + _hitTestExtension))
                    return DHitTest.Body;
                else if (DGeom.PointInPolygon(pt, GetStartMarkerPoints()) || DGeom.PointInPolygon(pt, GetEndMarkerPoints()))
                    return DHitTest.Body;
            }
            return DHitTest.None;
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
            Points = points; 
        }

        public PolylineFigure()
        { }

        protected override void PaintBody(DGraphics dg)
        {
            if (Points != null && Points.Count > 1)
            {
                if (UseRealAlpha && Alpha != 1 && StrokeWidth > 0 && (StartMarker != DMarker.None || EndMarker != DMarker.None))
                {
                    DPoints smp;
                    if (StartMarker != DMarker.None)
                        smp = GetStartMarkerPoints();
                    else
                    {
                        if (Points.Count > 1)
                            smp = MarkerHelper.MarkerPoints(DMarker.Square, Points[0], Points[1], MarkerSize);
                        else
                            smp = new DPoints();
                    }
                    DPoints emp;
                    if (EndMarker != DMarker.None)
                        emp = GetEndMarkerPoints();
                    else
                    {
                        if (Points.Count > 1)
                            emp = MarkerHelper.MarkerPoints(DMarker.Square, Points[Points.Count - 1], Points[Points.Count - 2], MarkerSize);
                        else
                            emp = new DPoints();
                    }
                    DRect R = StrokeHelper.RectIncludingStrokeWidth(GetEncompassingRect(), StrokeWidth)
                        .Union(smp.Bounds()).Union(emp.Bounds());

                    dg.StartGroup(X, Y, R.Width, R.Height, X - R.X, Y - R.Y);
                    dg.DrawPolyline(Points, Stroke, 1, StrokeWidth, StrokeStyle, StrokeJoin, StrokeCap);
                    if (StartMarker != DMarker.None)
                        dg.FillPolygon(GetStartMarkerPoints(), Stroke, 1);
                    if (EndMarker != DMarker.None)
                        dg.FillPolygon(GetEndMarkerPoints(), Stroke, 1);
                    dg.DrawGroup(Alpha);
                }
                else
                {
                    dg.DrawPolyline(Points, Stroke, Alpha, StrokeWidth, StrokeStyle, StrokeJoin, StrokeCap);
                    if (StartMarker != DMarker.None)
                        dg.FillPolygon(GetStartMarkerPoints(), Stroke, Alpha);
                    if (EndMarker != DMarker.None)
                        dg.FillPolygon(GetEndMarkerPoints(), Stroke, Alpha);
                }
            }
        }
    }

    public class ImageFigure : RectbaseFigure, IImage
    {
        UndoRedo<DBitmap> _bitmap = new UndoRedo<DBitmap>(null);
        public DBitmap Bitmap
        {
            get { return _bitmap.Value; }
        }

        UndoRedo<byte[]> _imageData = new UndoRedo<byte[]>(null);
        public Byte[] ImageData
        {
            get { return _imageData.Value; }
            set
            {
                if (value != _imageData.Value)
                {
                    _imageData.Value = value;
                    if (value != null)
                        _bitmap.Value = GraphicsHelper.MakeBitmap(new MemoryStream(value));
                    else
                        _bitmap.Value = null;
                }
            }
        }

        UndoRedo<DImagePosition> _pos = new UndoRedo<DImagePosition>(DImagePosition.Stretch);
        public DImagePosition Position
        {
            get { return _pos.Value; }
            set
            {
                if (value != _pos.Value)
                    _pos.Value = value;
            }
        }

        string filename = null;
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        public ImageFigure()
        { }

        public ImageFigure(DRect rect, double rotation, byte[] imageData, string FileName) : base(rect, rotation)
        {
            ImageData = imageData;
            filename = FileName;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (Bitmap != null)
            {
                dg.Save();
                dg.Clip(Rect);
                switch (Position)
                {
                    case DImagePosition.Stretch:
                        dg.DrawBitmap(Bitmap, Rect, Alpha);
                        break;
                    case DImagePosition.Normal:
                        dg.DrawBitmap(Bitmap, TopLeft, Alpha);
                        break;
                    case DImagePosition.Center:
                        dg.DrawBitmap(Bitmap, new DPoint(X + Width / 2 - Bitmap.Width / 2,
                                                         Y + Height / 2 - Bitmap.Height / 2), Alpha);
                        break;
                    case DImagePosition.Tile:
                        int xTimes = (int)Math.Ceiling(Width / Bitmap.Width);
                        int YTimes = (int)Math.Ceiling(Height / Bitmap.Height);
                        for (int i = 0; i < xTimes; i++)
                            for (int j = 0; j < YTimes; j++)
                                dg.DrawBitmap(Bitmap, new DPoint(X + i * Bitmap.Width, Y + j * Bitmap.Height), Alpha);
                        break;
                    case DImagePosition.StretchWithAspectRatio:
                        double sx = Width / Bitmap.Width;
                        double sy = Height / Bitmap.Height;
                        DRect bmpRect;
                        if (sx > sy)
                        {
                            double w = sy * Bitmap.Width;
                            bmpRect = new DRect(X + Width / 2 - w / 2, Y, w, Height);
                        }
                        else
                        {
                            double h = sx * Bitmap.Height;
                            bmpRect = new DRect(X, Y + Height / 2 - h / 2, Width, h);
                        }
                        dg.DrawBitmap(Bitmap, bmpRect, Alpha);
                        break;
                }
                dg.Restore();
            }
        }
    }

    public class TextFigure : RectbaseFigure, IFillable, ITextable
    {
        public override double MinSize
        {
            get { return 5; }
        }

        UndoRedo<string> _fontName = new UndoRedo<string>("Courier New");
        public string FontName
        {
            get { return _fontName.Value; }
            set
            {
                if (value != _fontName.Value)
                {
                    _fontName.Value = value;
                    UpdateSize();
                }
            }
        }

        UndoRedo<double> _fontSize = new UndoRedo<double>(10);
        public double FontSize
        {
            get { return _fontSize.Value; }
            set
            {
                if (value != _fontSize.Value)
                {
                    if (value <= 0)
                        value = 1;
                    _fontSize.Value = value;
                    UpdateSize();
                }
            }
        }

        UndoRedo<bool> _bold = new UndoRedo<bool>(false);
        public bool Bold
        {
            get { return _bold.Value; }
            set
            {
                if (value != _bold.Value)
                {
                    _bold.Value = value;
                    UpdateSize();
                }
            }
        }
        UndoRedo<bool> _italics = new UndoRedo<bool>(false);
        public bool Italics
        {
            get { return _italics.Value; }
            set
            {
                if (value != _italics.Value)
                {
                    _italics.Value = value;
                    UpdateSize();
                }
            }
        }
        UndoRedo<bool> _underline = new UndoRedo<bool>(false);
        public bool Underline
        {
            get { return _underline.Value; }
            set
            {
                if (value != _underline.Value)
                {
                    _underline.Value = value;
                    UpdateSize();
                }
            }
        }
        UndoRedo<bool> _strikethrough = new UndoRedo<bool>(false);
        public bool Strikethrough
        {
            get { return _strikethrough.Value; }
            set
            {
                if (value != _strikethrough.Value)
                {
                    _strikethrough.Value = value;
                    UpdateSize();
                }
            }
        }

        UndoRedo<string> _text = new UndoRedo<string>(null);
        public string Text
        {
            get { return _text.Value; }
            set
            {
                if (value != _text.Value)
                {
                    _text.Value = value;
                    UpdateSize();
                }
            }
        }

        public bool HasText
        {
            get { return Text != null && Text.Length > 0; }
        }

        public override double Width
        {
            get { return base.Width; }
            set 
            {
                if (base.Width != 0)
                    FontSize *= value / base.Width; 
            }
        }

        public override double Height
        {
            get { return base.Height; }
            set { /* do nothing :) */ }
        }

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        public TextFigure() : this(new DPoint(0, 0), "", 0)
        { }

        public TextFigure(DPoint pt, string text, double rotation)
        {
            TopLeft = pt;
            Text = text;
            Rotation = rotation;
        }

        public TextFigure(DPoint pt, string text, string fontName, double fontSize, double rotation)
        {
            TopLeft = pt;
            FontName = fontName;
            FontSize = fontSize;
            Text = text;
            Rotation = rotation;
        }

        void UpdateSize()
        {
            DPoint sz = GraphicsHelper.MeasureText(Text, FontName, FontSize, Bold, Italics, Underline, Strikethrough);
            base.Width = sz.X;
            base.Height = sz.Y;
        }

        protected override void PaintBody(DGraphics dg)
        {
            dg.DrawText(Text, FontName, FontSize, Bold, Italics, Underline, Strikethrough, Rect.TopLeft, Fill, Alpha);
        }

        #region IFillable Members
        UndoRedo<DColor> _fill = new UndoRedo<DColor>(DColor.Red);
        public DColor Fill
        {
            get { return _fill.Value; }
            set { if (!value.Equals(_fill.Value)) _fill.Value = value; }
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
        int _cursorPos;
        int cursorPosition
        {
            get { return _cursorPos; }
        }
        // selection length is the number of chars selected after the cursor
        int selectionLength;

        string[] Lines
        {
            get { return Text.Split('\n'); }
        }

        public TextEditFigure(TextFigure tf)
        {
            this.tf = tf;
            SetCursorPos(Text.Length, false);
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
            // paint selection & cursor
            string[] lines = Lines;
            DPoint pt = tf.Rect.TopLeft;
            double height = LineHeight(lines);
            DPoint cpt = MeasureCursorPosition(lines, height);
            DRect[] selRects = MeasureSelectionRects(lines, height);

            foreach (DRect sr in selRects)
                dg.FillRect(pt.X + sr.X, pt.Y + sr.Y, sr.Width, sr.Height, DColor.LightGray, 0.5);

            dg.DrawLine(new DPoint(pt.X + cpt.X, pt.Y + cpt.Y), new DPoint(pt.X + cpt.X, pt.Y + cpt.Y + height), DColor.Black, 1, DStrokeStyle.Solid, 2, DStrokeCap.Butt);
        }

        int FindPrevWord()
        {
            int i = cursorPosition;
            if (i > 0)
            {
                bool startAtWhitespace = Char.IsWhiteSpace(Text[i - 1]);
                while (i > 0)
                {
                    i--;
                    if (i == 0)
                        break;
                    if (startAtWhitespace)
                    {
                        if (!Char.IsWhiteSpace(Text[i - 1]))
                            startAtWhitespace = false;
                    }
                    else if (Char.IsWhiteSpace(Text[i - 1]))
                        break;
                }
            }
            return i;
        }

        int FindNextWord()
        {
            int i = cursorPosition;
            if (i < Text.Length)
            {
                bool startAtWord = !Char.IsWhiteSpace(Text[i]);
                while (i <= Text.Length)
                {
                    i++;
                    if (i == Text.Length)
                        break;
                    if (startAtWord)
                    {
                        if (Char.IsWhiteSpace(Text[i]))
                            startAtWord = false;
                    }
                    else if (!Char.IsWhiteSpace(Text[i]))
                        break;
                }
            }
            return i;
        }

        double LineHeight(string[] lines)
        {
            return GraphicsHelper.MeasureText(Text, FontName, FontSize, Bold, Italics, Underline, Strikethrough).Y / lines.Length;
        }

        DPoint MeasureCursorPosition(string[] lines, double height)
        {
            // find the x,y position of the cursor
            int n = cursorPosition;
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

        DRect[] MeasureSelectionRects(string[] lines, double lineHeight)
        {
            DRect[] res = new DRect[0];
            if (selectionLength != 0)
            {
                // init selection variables
                int selStart = cursorPosition;
                int selEnd;
                if (selectionLength < 0)
                {
                    selStart += selectionLength;
                    selEnd = cursorPosition;
                }
                else
                    selEnd = cursorPosition + selectionLength;
                // start measuring selection rectangles line by line
                int n = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string selText = null;
                    int selIdx = 0;
                    if (selStart >= n && selStart < n + line.Length) // start line
                    {
                        selIdx = selStart - n;
                        selText = line.Substring(selIdx, Math.Min(selEnd - selStart, line.Length - selIdx));
                    }
                    else if (selEnd > n && selEnd <= n + line.Length) // end line
                    {
                        selIdx = 0;
                        selText = line.Substring(selIdx, selEnd - n);
                    }
                    else if (n > selStart && n < selEnd) // in between line
                    {
                        selIdx = 0;
                        selText = line;
                    }
                    if (selText != null)
                    {
                        // start of selection rect for this line
                        double x;
                        if (selIdx != 0)
                            x = GraphicsHelper.MeasureText(line.Substring(0, selIdx), FontName, FontSize, Bold, Italics, Underline, Strikethrough).X;
                        else
                            x = 0;
                        // width of selection rect for this line
                        double w = GraphicsHelper.MeasureText(selText, FontName, FontSize, Bold, Italics, Underline, Strikethrough).X;
                        if (selIdx != 0)
                        {
                            // take into account the space that is put infront of the first character
                            double w2 = GraphicsHelper.MeasureText(string.Concat(selText, selText),
                            FontName, FontSize, Bold, Italics, Underline, Strikethrough).X;
                            w = w2 - w;
                        }
                        // add selection rect to function result
                        Array.Resize(ref res, res.Length + 1);
                        res[res.Length - 1] = new DRect(x, lineHeight * i, w, lineHeight);

                    }
                    n += line.Length + 1;
                }
            }
            return res;
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

        void SetCursorPos(int pos, bool select)
        {
            if (select)
            {
                selectionLength += _cursorPos - pos;
                _cursorPos = pos;
            }
            else
            {
                _cursorPos = pos;
                selectionLength = 0;
            }
        }

        public void MoveCursor(DKeys k, bool moveWord, bool select)
        {
            switch (k)
            {
                case DKeys.Left:
                    if (moveWord)
                        SetCursorPos(FindPrevWord(), select);
                    else if (cursorPosition > 0) 
                        SetCursorPos(cursorPosition - 1, select);
                    break;
                case DKeys.Right:
                    if (moveWord)
                        SetCursorPos(FindNextWord(), select);
                    else if (cursorPosition < Text.Length)
                        SetCursorPos(cursorPosition + 1, select);
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
                    DPoint cpt = MeasureCursorPosition(lines, LineHeight(lines));
                    int newLineCharNo = FindEquivCharacterPosition(lines[newLineNo], currentlineCharNo, cpt.X);
                    SetCursorPos(cursorPosition + CharNumberChange(lines, lineNo, newLineNo, currentlineCharNo, newLineCharNo),
                        select);
                    break;
                case DKeys.Down:
                    goto case DKeys.Up;
                case DKeys.Home:
                    while (cursorPosition > 0 && Text[cursorPosition - 1] != '\n')
                        SetCursorPos(cursorPosition - 1, select);
                    break;
                case DKeys.End:
                    while (cursorPosition < Text.Length && Text[cursorPosition] != '\n')
                        SetCursorPos(cursorPosition + 1, select);
                    break;
            }
        }

        void DeleteSelection()
        {
            if (selectionLength < 0)
            {
                Text = Text.Remove(cursorPosition + selectionLength, Math.Abs(selectionLength));
                SetCursorPos(cursorPosition - Math.Abs(selectionLength), false);
            }
            else
                Text = Text.Remove(cursorPosition, selectionLength);
            selectionLength = 0;
        }

        public void InsertAtCursor(char c)
        {
            if (selectionLength != 0)
                DeleteSelection();
            if (cursorPosition < Text.Length)
                Text = Text.Insert(cursorPosition, c.ToString());
            else
                Text = string.Concat(Text, c);
            MoveCursor(DKeys.Right, false, false);
        }

        public void BackspaceAtCursor()
        {
            if (selectionLength == 0)
            {
                if (cursorPosition > 0)
                {
                    Text = Text.Remove(cursorPosition - 1, 1);
                    MoveCursor(DKeys.Left, false, false);
                }
            }
            else
                DeleteSelection();
        }

        public void DeleteAtCursor()
        {
            if (selectionLength == 0)
            {
                if (cursorPosition < Text.Length)
                    Text = Text.Remove(cursorPosition, 1);
            }
            else
                DeleteSelection();
        }

        public void SetCursor(DPoint pt)
        {
            pt = new DPoint(pt.X - X - border, pt.Y - Y - border);
            string[] lines = Lines;
            double lineHeight = LineHeight(lines);
            int pos = 0;
            // find line that pt is on
            for (int i = 0; i < lines.Length; i++)
            {
                pos += lines[i].Length;
                if (lineHeight * (i + 1) >= pt.Y || i == lines.Length - 1)
                {
                    double lastWidth = 0;
                    // find character in line that pt is on
                    for (int j = 1; j <= lines[i].Length; j++)
                    {
                        string substr = lines[i].Substring(0, j);
                        double width = GraphicsHelper.MeasureText(substr, FontName, FontSize, Bold, Italics, Underline, Strikethrough).X;
                        if (width >= pt.X)
                        {
                            double middleCharX;
                            if (j == 1)
                                middleCharX = width / 2;
                            else
                                middleCharX = width - ((width - lastWidth) / 2);
                            if (middleCharX > pt.X)
                                pos -= lines[i].Length - (j - 1);
                            else
                                pos -= lines[i].Length - j;
                            break;
                        }
                        lastWidth = width;
                    }
                    break;
                }
                else
                    pos++;
            }
            // set new cursor position
            SetCursorPos(pos, false);
        }
    }

    public class GroupFigure : RectbaseFigure, IChildFigureable
    {
        public override double MinSize
        {
            get { return 10; }
        }

        public override double X
        {
            get { return boundingBox.X; }
            set
            {
                double dX = value - X;
                originalRect.X += dX;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    originalChildRects[i].X += dX;
                    f.X += dX;
                    i += 1;
                }
                CreateBoundingBox();
            }
        }

        public override double Y
        {
            get { return boundingBox.Y; }
            set
            {
                double dY = value - Y;
                originalRect.Y += dY;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    originalChildRects[i].Y += dY;
                    f.Y += dY;
                    i += 1;
                }
                CreateBoundingBox();
            }
        }

        public override double Width
        {
            get { return Right - X; }
            set
            {
                double sx = value / originalRect.Width;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    UndoRect ocr = originalChildRects[i];
                    f.X = originalRect.X + (sx * (ocr.X - originalRect.X));
                    f.Width = sx * ocr.Width;
                    i++;
                }
                CreateBoundingBox(); 
            }
        }

        public override double Height
        {
            get { return Bottom - Y; }
            set
            {
                double sy = value / originalRect.Height;
                int i = 0;
                foreach (Figure f in childFigs)
                {
                    UndoRect ocr = originalChildRects[i];
                    f.Y = originalRect.Y + (sy * (ocr.Y - originalRect.Y));
                    f.Height = sy * ocr.Height;
                    i++;
                }
                CreateBoundingBox(); 
            }
        }

        public override double Right
        {
            get { return boundingBox.X + boundingBox.Width; }
            set { Width = value - X; }
        }

        public override double Bottom
        {
            get { return boundingBox.Y + boundingBox.Height; }
            set { Height = value - Y; }
        }

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        UndoRedoList<Figure> childFigs;
        public UndoRedoList<Figure> ChildFigures
        {
            get { return childFigs; }
            set
            {
                childFigs = value;
                CreateOriginalRects();
            }
        }

        UndoRect[] originalChildRects;
        UndoRect originalRect;
        UndoRect boundingBox = new UndoRect(new DRect());

        public GroupFigure() : this(new List<Figure>())
        { }

        public GroupFigure(IList<Figure> figs)
        {
            System.Diagnostics.Debug.Assert(figs != null, "figs is not assigned");
            // make new figure list
            childFigs = new UndoRedoList<Figure>();
            foreach (Figure f in figs)
                childFigs.Add(f);
            // store starting dimensions for scaling later on
            CreateOriginalRects();
            // use individual alpha for each child
            UseRealAlpha = false;
        }

        void CreateOriginalRects()
        {
            originalChildRects = new UndoRect[childFigs.Count];
            int i = 0;
            foreach (Figure f in childFigs)
            {
                originalChildRects[i] = new UndoRect(f.Rect);
                i++;
            }
            CreateBoundingBox();
            originalRect = new UndoRect(boundingBox.Rect);
        }

        public override DHitTest HitTest(DPoint pt, List<Figure> children, out IGlyph glyph)
        {
            pt = RotatePointToFigure(pt);
            DHitTest res = GlyphHitTest(pt, out glyph);
            if (res == DHitTest.None)
            {
                res = RotateHitTest(pt);
                if (res == DHitTest.None)
                {
                    res = ResizeHitTest(pt);
                    if (res == DHitTest.None)
                    {
                        foreach (Figure f in childFigs)
                        {
                            res = f.HitTest(pt, children, out glyph);
                            if (res != DHitTest.None)
                            {
                                if (children != null)
                                    children.Add(f);
                                if (res == DHitTest.Glyph)
                                    return res;
                                break;
                            }
                        }
                        DHitTest temp = res;
                        res = SelectHitTest(pt);
                        if (res == DHitTest.None)
                            res = temp;
                    }
                }
            }
            return res;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (UseRealAlpha)
            {
                if (Width > 0 && Height > 0)
                {
                    DRect rS = GetSelectRect();
                    dg.StartGroup(rS.X, rS.Y, rS.Width, rS.Height, 0, 0);
                    foreach (Figure f in childFigs)
                    {
                        f._controlScale = _controlScale;
                        f.GlyphsVisible = GlyphsVisible;
                        f.Paint(dg);
                    }
                    dg.DrawGroup(Alpha);
                }
            }
            else
                foreach (Figure f in childFigs)
                {
                    f._controlScale = _controlScale;
                    f.GlyphsVisible = GlyphsVisible;
                    f.Paint(dg);
                }
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

        void CreateBoundingBox()
        {
            if (childFigs.Count > 0)
            {
                boundingBox.Rect = DGeom.BoundingBoxOfRotatedRect(childFigs[0].Rect, childFigs[0].Rotation);
                foreach (Figure f in childFigs)
                {
                    DRect r2 = DGeom.BoundingBoxOfRotatedRect(f.Rect, f.Rotation);
                    boundingBox.Rect = boundingBox.Rect.Union(r2);
                }
            }
            else
                boundingBox.Rect = new DRect();
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

    public class BackgroundFigure : ImageFigure, IFillable
    {
        protected override void PaintBody(DGraphics dg)
        {
            dg.FillRect(X, Y, Width, Height, Fill, Alpha);
            base.PaintBody(dg);
        }

        #region IFillable Members
        UndoRedo<DColor> _fill = new UndoRedo<DColor>(DColor.White);
        public virtual DColor Fill
        {
            get { return _fill.Value; }
            set { if (!value.Equals(_fill.Value)) _fill.Value = value; }
        }
        #endregion
    }
}
