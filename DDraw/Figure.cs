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
        bool FlipX
        {
            get;
            set;
        }
        bool FlipY
        {
            get;
            set;
        }
        bool LockAspectRatio
        {
            get;
            set;
        }
        bool Locked
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
        DPoint TextOffset
        {
            get;
        }

        bool WrapText
        {
            get;
            set;
        }
        double WrapThreshold
        {
            get;
            set;
        }
        double WrapFontSize
        {
            get;
            set;
        }
        double WrapLength
        {
            get;
        }
        string WrappedText
        {
            get;
        }
    }

    public static class TextHelper
    {
        static string WrapLine(DGraphics dg, string s, ITextable itext, out string remainder)
        {
            string result;
            remainder = null;
            if (itext.WrapLength > 0)
            {
                result = s;
                int n = s.Length - 1;
                DPoint sz = MeasureText(dg, s, itext);
                while (sz.X > itext.WrapLength)
                {
                    if (char.IsWhiteSpace(s[n]))
                    {
                        result = s.Substring(0, n);
                        remainder = s.Substring(n);
                    }
                    else
                    {
                        bool foundWord = false;
                        for (int j = n - 1; j >= 0; j--)
                            if (char.IsWhiteSpace(s[j]))
                            {
                                result = s.Substring(0, j + 1);
                                remainder = s.Substring(j + 1);
                                n = j + 1;
                                foundWord = true;
                                break;
                            }
                        if (!foundWord)
                        {
                            result = s.Substring(0, n);
                            remainder = s.Substring(n);
                        }
                    }
                    sz = MeasureText(dg, result, itext);
                    n--;
                }
            }
            else
                result = "";
            if (result.Length == 0)
            {
                if (s.Length > 0)
                    result = s.Substring(0, 1);
                if (s.Length > 1)
                    remainder = s.Substring(1);
                else
                    remainder = null;
            }
            return result;
        }

        public static string MakeWrappedText(string text, ITextable itext)
        {
            DBitmap bmp = GraphicsHelper.MakeBitmap(10, 10);
            DGraphics dg = GraphicsHelper.MakeGraphics(bmp);

            string result = "";
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                // wrap line
                string remainder = line;
                string wrappedLine;
                string wrappedText = null;
                do
                {
                    wrappedLine = WrapLine(dg, remainder, itext, out remainder);
                    if (wrappedText == null)
                        wrappedText = wrappedLine;
                    else
                        wrappedText = string.Concat(wrappedText, '\n', wrappedLine);
                }
                while (remainder != null);
                // join lines
                if (i < lines.Length - 1)
                    result = string.Concat(result, wrappedText, '\n');
                else
                    result = string.Concat(result, wrappedText);
            }

            dg.Dispose();
            bmp.Dispose();

            return result;
        }

        public static DPoint MeasureText(DGraphics dg, string text, ITextable itext)
        {
            return GraphicsHelper.MeasureText(dg, text, itext.FontName, itext.FontSize, itext.Bold, itext.Italics, itext.Underline, itext.Strikethrough);
        }

        public static DPoint MeasureText(string text, ITextable itext)
        {
            return GraphicsHelper.MeasureText(text, itext.FontName, itext.FontSize, itext.Bold, itext.Italics, itext.Underline, itext.Strikethrough);
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
        bool ContextHandle
        {
            get;
            set;
        }
        void PaintSelectionChrome(DGraphics dg);
        DRect GetSelectRect();
        DRect GetResizeHandleRect();
        DRect GetRotateHandleRect();
        DRect GetContextHandleRect();
        DRect GetEncompassingRect();
        DHitTest SelectHitTest(DPoint pt);
        DHitTest ResizeHitTest(DPoint pt);
        DHitTest RotateHitTest(DPoint pt);
        DHitTest ContextHitTest(DPoint pt);
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

        bool HasGlyphs
        {
            get;
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
        bool mouseOver = false;
        public virtual bool MouseOver
        {
            get { return mouseOver; }
            set { mouseOver = value; }
        }

        bool contextHandle = false;
        public bool ContextHandle
        {
            get { return contextHandle; }
            set { contextHandle = value; }
        }

        UndoRedo<bool> _locked = new UndoRedo<bool>(false);
        public bool Locked
        {
            get { return _locked.Value; }
            set { _locked.Value = value; }
        }

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
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }
        UndoRedo<double> _rotation = new UndoRedo<double>(0);
        public virtual double Rotation
        {
            get { return _rotation.Value; }
            set { if (value != _rotation.Value) _rotation.Value = value; }
        }

        UndoRedo<bool> _flipX = new UndoRedo<bool>(false);
        public virtual bool FlipX
        {
            get { return _flipX.Value; }
            set { if (value != _flipX.Value) _flipX.Value = value; }
        }
        UndoRedo<bool> _flipY = new UndoRedo<bool>(false);
        public virtual bool FlipY
        {
            get { return _flipY.Value; }
            set { if (value != _flipY.Value) _flipY.Value = value; }
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

        public DPoint FlipPointToFigure(DPoint pt)
        {
            // account for flip
            DPoint ctr = Rect.Center;
            DPoint res = new DPoint(pt.X, pt.Y);
            if (FlipX)
                res.X -= (res.X - ctr.X) * 2;
            if (FlipY)
                res.Y -= (res.Y - ctr.Y) * 2;
            return res;
        }

        public DPoint TransformPointToFigure(DPoint pt)
        {
            DPoint res = RotatePointToFigure(pt);
            return FlipPointToFigure(res);
        }

        protected DHitTest SelectAndBodyHitTest(DPoint pt, List<Figure> children)
        {
            DHitTest res = SelectHitTest(pt);
            if (res == DHitTest.None)
                res = BodyHitTest(FlipPointToFigure(pt), children);
            return res;
        }

        public virtual DHitTest HitTest(DPoint pt, List<Figure> children, out IGlyph glyph)
        {
            pt = RotatePointToFigure(pt);
            DHitTest res = GlyphHitTest(pt, out glyph);
            if (res == DHitTest.None)
            {
                if (Locked)
                {
                    res = LockHitTest(pt);
                    if (res == DHitTest.None)
                        res = SelectAndBodyHitTest(pt, children);
                }
                else
                {
                    res = RotateHitTest(pt);
                    if (res == DHitTest.None)
                    {
                        res = ResizeHitTest(pt);
                        if (res == DHitTest.None)
                        {
                            res = ContextHitTest(pt);
                            if (res == DHitTest.None)
                                res = SelectAndBodyHitTest(pt, children);
                        }
                    }
                }
            }
            return res;
        }

        protected abstract DHitTest BodyHitTest(DPoint pt, List<Figure> children);

        protected void ApplyTransforms(DGraphics dg, bool doFlips)
        {
            DPoint ctr = Rect.Center;
            dg.Rotate(Rotation, ctr);
            if (doFlips)
            {
                if (FlipX)
                {
                    dg.Translate(ctr.X * 2, 0);
                    dg.Scale(-1, 1);
                }
                if (FlipY)
                {
                    dg.Translate(0, ctr.Y * 2);
                    dg.Scale(1, -1);
                }
            }
        }

        void UnFlipTransform(DGraphics dg)
        {
            DPoint ctr = Rect.Center;
            if (FlipX)
            {
                dg.Scale(-1, 1);
                dg.Translate(-ctr.X * 2, 0);
            }
            if (FlipY)
            {
                dg.Scale(1, -1);
                dg.Translate(0, -ctr.Y * 2);
            }
        }

        public void Paint(DGraphics dg)
        {
            dg.Save();
            ApplyTransforms(dg, true);
            PaintBody(dg);
            if (glyphsVisible && HasGlyphs)
            {
                UnFlipTransform(dg);
                PaintGlyphs(dg);
            }
            dg.Restore();
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

        protected virtual void PaintContextHandle(DGraphics dg)
        {
            if (contextHandle)
            {
                double hb = _handleBorder * _controlScale;
                double hb2 = hb + hb;
                DRect r = GetContextHandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                dg.FillEllipse(r, DColor.White);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                double qW = r.Width / 4;
                double tH = r.Height / 3;
                DPoints pts = new DPoints();
                pts.Add(new DPoint(r.X + qW, r.Y + tH));
                pts.Add(new DPoint(r.X + qW * 2, r.Y + tH * 2));
                pts.Add(new DPoint(r.X + qW * 3, r.Y + tH));
                dg.DrawPolyline(pts, DColor.Black);
            }
        }

        protected virtual void PaintLockHandle(DGraphics dg)
        {
            if (contextHandle)
            {
                double hb = _handleBorder * _controlScale;
                double hb2 = hb + hb;
                DRect r = GetContextHandleRect();
                if (hb != 0)
                    r = r.Resize(hb, hb, -hb2, -hb2);
                dg.FillEllipse(r, DColor.White);
                dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Solid);
                double qW = r.Width / 4;
                double qH = r.Height / 4;
                dg.DrawLine(new DPoint(r.X + qW, r.Y + qW), new DPoint(r.X + qW * 3, r.Y + qW * 3), DColor.Red);
                dg.DrawLine(new DPoint(r.X + qW, r.Y + qW * 3), new DPoint(r.X + qW * 3, r.Y + qW), DColor.Red);
            }
        }

        public virtual void PaintSelectionChrome(DGraphics dg)
        {
            if (Selected)
            {
                // save current transform
                DMatrix m = dg.SaveTransform();
                // apply transform
                ApplyTransforms(dg, false);
                // draw selection rectangle
                DRect r = GetSelectRect();
                double selectRectY = r.Y;
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.White, 1, _controlScale);
                dg.DrawRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, _controlScale, DStrokeStyle.Dot, DStrokeJoin.Mitre);
                if (Locked)
                    PaintLockHandle(dg);
                else
                {
                    // draw context handle
                    PaintContextHandle(dg);
                    // draw resize handle
                    double hb = _handleBorder * _controlScale;
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
                }
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

        public virtual DRect GetContextHandleRect()
        {
            DRect selectRect = GetSelectRect();
            double hs = HandleSize * _controlScale;
            return new DRect(selectRect.Right - hs, selectRect.Y - hs, hs + hs, hs + hs);
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

        public virtual DHitTest ContextHitTest(DPoint pt)
        {
            if (selected && contextHandle && DGeom.PointInRect(pt, GetContextHandleRect()))
                return DHitTest.Context;
            return DHitTest.None;
        }

        public virtual DHitTest LockHitTest(DPoint pt)
        {
            if (selected && DGeom.PointInRect(pt, GetContextHandleRect()))
                return DHitTest.Lock;
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

        public bool HasGlyphs
        {
            get { return glyphs != null && glyphs.Count > 0; }
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
#if BEHAVIOURS
        , IBehaviours
#endif    
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

#if BEHAVIOURS
        #region IBehaviours Members
        DBehaviour mouseOverBehaviour;
        public DBehaviour MouseOverBehaviour
        {
            get { return mouseOverBehaviour; }
            set { mouseOverBehaviour = value; }
        }
        #endregion
#endif
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

        public override DRect GetEncompassingRect()
        {
            return StrokeHelper.RectIncludingStrokeWidth(base.GetEncompassingRect(), StrokeWidth);
        }

        protected const int noFillThresh = 3;
        protected const int noFillThresh2 = noFillThresh + noFillThresh;

        protected void NoFillRects(out DRect r, out DRect r2)
        {
             r = new DRect(X - strokeWidthHalf - noFillThresh, 
                 Y - strokeWidthHalf - noFillThresh, 
                 Width + StrokeWidth + noFillThresh2, 
                 Height + StrokeWidth + noFillThresh2);
             r2 = new DRect(X + strokeWidthHalf + noFillThresh, 
                 Y + strokeWidthHalf + noFillThresh, 
                 Width - StrokeWidth - noFillThresh2, 
                 Height - StrokeWidth - noFillThresh2);
        }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
        {
            if (Fill.IsEmpty)
            {
                DRect r, r2;
                NoFillRects(out r, out r2);
                if (DGeom.PointInRect(pt, r) && !DGeom.PointInRect(pt, r2))
                    return DHitTest.Body;
            }
            else if (DGeom.PointInRect(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
#if BEHAVIOURS
            // select paint properties
            DColor Fill = this.Fill;
            DColor Stroke = this.Stroke;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetFill)
                    Fill = MouseOverBehaviour.Fill;
                if (MouseOverBehaviour.SetStroke)
                    Stroke = MouseOverBehaviour.Stroke;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
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
            if (Fill.IsEmpty)
            {
                DRect r, r2;
                NoFillRects(out r, out r2);
                if (DGeom.PointInEllipse(pt, r) && !DGeom.PointInEllipse(pt, r2))
                    return DHitTest.Body;
            }
            else if (DGeom.PointInEllipse(pt, RectInclStroke))
                return DHitTest.Body;
            return DHitTest.None;
        }

        protected override void PaintBody(DGraphics dg)
        {
#if BEHAVIOURS
            // select paint properties
            DColor Fill = this.Fill;
            DColor Stroke = this.Stroke;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetFill)
                    Fill = MouseOverBehaviour.Fill;
                if (MouseOverBehaviour.SetStroke)
                    Stroke = MouseOverBehaviour.Stroke;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
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
#if BEHAVIOURS
        , IBehaviours
#endif
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
            return StrokeHelper.RectIncludingStrokeWidth(base.GetEncompassingRect(), StrokeWidth)
                .Union(GetStartMarkerRect()).Union(GetEndMarkerRect());
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

#if BEHAVIOURS
        #region IBehaviours Members
        DBehaviour mouseOverBehaviour;
        public DBehaviour MouseOverBehaviour
        {
            get { return mouseOverBehaviour; }
            set { mouseOverBehaviour = value; }
        }
        #endregion
#endif
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

        public override bool FlipX
        {
            get { return false; }
            set
            {
                if (value)
                {
                    DPoint tmp = Pt1;
                    Pt1 = new DPoint(Pt2.X, Pt1.Y);
                    Pt2 = new DPoint(tmp.X, Pt2.Y);
                }
            }
        }

        public override bool FlipY
        {
            get { return false; }
            set
            {
                if (value)
                {
                    DPoint tmp = Pt1;
                    Pt1 = new DPoint(Pt1.X, Pt2.Y);
                    Pt2 = new DPoint(Pt2.X, tmp.Y);
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

        public override DRect GetContextHandleRect()
        {
            DPoint ctr = GetSelectRect().Center;
            double hs = HandleSize * _controlScale;
            return new DRect(ctr.X - hs, ctr.Y - hs, hs + hs, hs + hs);
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
                ApplyTransforms(dg, true);
                if (Locked)
                    PaintLockHandle(dg);
                else
                {
                    // draw context handle
                    PaintContextHandle(dg);
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
        }

        public override void PaintGlyphs(DGraphics dg)
        {
            if (Glyphs != null && Glyphs.Count > 0)
            {
                DMatrix m = dg.SaveTransform();
                ApplyTransforms(dg, false);
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
#if BEHAVIOURS
            // select paint properties
            DColor Stroke = this.Stroke;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetStroke)
                    Stroke = MouseOverBehaviour.Stroke;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
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
        UndoRect boundingBox = new UndoRect(new DRect());

        UndoRedo<DPoints> _points = new UndoRedo<DPoints>(new DPoints());
        public DPoints Points
        {
            get { return _points.Value; }
            set
            {
                _points.Value = value;
                DRect r = _points.Value.Bounds();
                boundingBox.X = r.X;
                boundingBox.Y = r.Y;
                boundingBox.Width = r.Width;
                boundingBox.Height = r.Height;
            }
        }

        public void AddPoint(DPoint pt)
        {
            DPoints pts = Points;
            if (pts == null)
                pts = new DPoints();
            pts.Add(pt);
            Points = pts;
        }

        void MovePoints(double dX, double dY)
        {
            DPoints newPts = new DPoints();
            foreach (DPoint pt in Points)
                newPts.Add(new DPoint(pt.X + dX, pt.Y + dY));
            Points = newPts;
        }

        public override double X
        {
            get { return boundingBox.X; }
            set
            {
                if (value != X)
                    MovePoints(value - X, 0);
            }
        }

        public override double Y
        {
            get { return boundingBox.Y; }
            set
            {
                if (value != Y)
                    MovePoints(0, value - Y);
            }
        }

        void ResizePoints(double sX, double sY)
        {
            DPoints newPts = new DPoints();
            foreach (DPoint pt in Points)
            {
                DPoint newPt = new DPoint(0, 0);
                if (sX == 1)
                    newPt.X = pt.X;
                else
                    newPt.X = pt.X + (pt.X - X) * (sX - 1);
                if (sY == 1)
                    newPt.Y = pt.Y;
                else
                    newPt.Y = pt.Y + (pt.Y - Y) * (sY - 1);
                newPts.Add(newPt);
            }
            Points = newPts;
        }

        public override double Width
        {
            get { return boundingBox.Width; }
            set
            {
                if (value != Width && Points.Count > 1)
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
                        ResizePoints(value / Width, 1);
                }
            }
        }

        public override double Height
        {
            get { return boundingBox.Height; }
            set
            {
                if (value != Height && Points.Count > 1)
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
                        ResizePoints(1, value / Height);
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
#if BEHAVIOURS
            // select paint properties
            DColor Stroke = this.Stroke;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetStroke)
                    Stroke = MouseOverBehaviour.Stroke;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
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
#if BEHAVIOURS
        , IBehaviours
#endif
    {
        public override double MinSize
        {
            get { return 5; }
        }

        const int defaultFontSize = 10;
        const int minWrapThreshold = 5;

        #region ITextable members
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

        UndoRedo<double> _fontSize = new UndoRedo<double>(defaultFontSize);
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
                    UpdateWrappedText();
                    UpdateSize();
                }
            }
        }

        public bool HasText
        {
            get { return Text != null && Text.Length > 0; }
        }

        public DPoint TextOffset
        {
            get { return new DPoint(0, 0); }
        }

        UndoRedo<bool> _wrapText = new UndoRedo<bool>(false);
        public bool WrapText
        {
            get { return _wrapText.Value; }
            set
            {
                if (value != _wrapText.Value)
                {
                    _wrapText.Value = value;
                    UpdateWrappedText();
                    UpdateSize();
                }
            }
        }

        UndoRedo<double> _wrapThreshold = new UndoRedo<double>(200);
        public double WrapThreshold
        {
            get { return _wrapThreshold.Value; }
            set
            {
                if (value < minWrapThreshold)
                    value = minWrapThreshold;
                if (value != _wrapThreshold.Value)
                {
                    _wrapThreshold.Value = value;
                    UpdateWrappedText();
                    UpdateSize();
                }
            }
        }

        UndoRedo<double> _wrapFontSize = new UndoRedo<double>(defaultFontSize);
        public double WrapFontSize
        {
            get { return _wrapFontSize.Value; }
            set
            {
                if (value != _wrapFontSize.Value)
                {
                    _wrapFontSize.Value = value;
                    UpdateWrappedText();
                    UpdateSize();
                }
            }
        }

        public double WrapLength
        {
            get { return FontSize / WrapFontSize * WrapThreshold; }
        }

        UndoRedo<string> __wrappedText = new UndoRedo<string>(null);
        string _wrappedText
        {
            get { return __wrappedText.Value; }
            set { __wrappedText.Value = value; }
        }

        public string WrappedText
        {
            get
            {
                if (WrapText)
                    return _wrappedText;
                else
                    return Text;
            }
        }
        #endregion

        public override double Width
        {
            get { return base.Width; }
            set 
            {
                if (base.Width != 0)
                    FontSize *= value / base.Width;
                else if (FontSize < 1)
                    FontSize = 1;
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

        void UpdateWrappedText()
        {
            if (WrapText)
                _wrappedText = TextHelper.MakeWrappedText(Text, this);
            else
                _wrappedText = null;
        }

        void UpdateSize()
        {
            DPoint sz = TextHelper.MeasureText(WrappedText, this);
            base.Width = sz.X;
            base.Height = sz.Y;
        }

        protected override void PaintBody(DGraphics dg)
        {
#if BEHAVIOURS
            // select paint properties
            DColor Fill = this.Fill;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetFill)
                    Fill = MouseOverBehaviour.Fill;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
            dg.DrawText(WrappedText, FontName, FontSize, Bold, Italics, Underline, Strikethrough, Rect.TopLeft, Fill, Alpha);
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
        const int border = 8;
        public int BorderWidth
        {
            get { return border; }
        }

        ITextable itext;
        public ITextable IText
        {
            get { return itext; }
        }

        Figure f;
        public Figure Figure
        {
            get { return f; }
        }

        public string Text
        {
            get { return itext.Text; }
            set { itext.Text = value; }
        }
        public bool HasText
        {
            get { return itext.HasText; }
        }
        public string FontName
        {
            get { return itext.FontName; }
            set { itext.FontName = value; }
        }
        public double FontSize
        {
            get { return itext.FontSize; }
            set { itext.FontSize = value; }
        }
        public bool Bold
        {
            get { return itext.Bold; }
            set { itext.Bold = value; }
        }
        public bool Italics
        {
            get { return itext.Italics; }
            set { itext.Italics = value; }
        }
        public bool Underline
        {
            get { return itext.Underline; }
            set { itext.Underline = value; }
        }
        public bool Strikethrough
        {
            get { return itext.Strikethrough; }
            set { itext.Strikethrough = value; }
        }
        public DPoint TextOffset
        {
            get { return itext.TextOffset.Offset(border, border); }
        }
        public bool WrapText
        {
            get { return itext.WrapText; }
            set { itext.WrapText = value; }
        }
        public double WrapThreshold
        {
            get { return itext.WrapThreshold; }
            set { itext.WrapThreshold = value; }
        }
        public double WrapFontSize
        {
            get { return itext.WrapFontSize; }
            set { itext.WrapFontSize = value; }
        }
        public double WrapLength
        {
            get { return itext.WrapLength; }
        }
        public string WrappedText
        {
            get { return itext.WrappedText; }
        }

        public override double X
        {
            get { return f.Left - border; }
            set { f.Left = value + border; }
        }
        public override double Y
        {
            get { return f.Top - border; }
            set { f.Top = value + border; }
        }
        public override double Width
        {
            get 
            {
                if (WrapText)
                    return Math.Max(WrapLength + border + border, f.Width + border + border);
                return f.Width + border + border; 
            }
            set { f.Width = value - border - border; }
        }
        public override double Height
        {
            get { return f.Height + border + border; }
            set { f.Height = value - border - border; }
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
            get { return WrappedText.Split('\n'); }
        }

        public TextEditFigure(Figure f, ITextable itext)
        {
            this.f = f;
            this.itext = itext;
            SetCursorPos(Text.Length, false);
        }

        public TextEditFigure(DPoint pt, Figure f, ITextable itext) : this(f, itext)
        {
            TopLeft = pt;
        }

        public DRect TextWrapHandleRect
        {
            get { return new DRect(Right - border, Y + Height / 2 - border / 2, border, border); }
        }

        protected override void PaintBody(DGraphics dg)
        {
            // paint border
            DRect r = Rect;
            dg.FillRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, DFillStyle.ForwardDiagonalHatch);
            dg.FillRect(r.X + border, r.Y + border, r.Width - border - border, r.Height - border - border, DColor.White, 1);
            // paint text
            double alpha = 1;
            if (f is IAlphaBlendable)
            {
                alpha = ((IAlphaBlendable)f).Alpha;
                ((IAlphaBlendable)f).Alpha = 1;
            }
            double rot = f.Rotation;
            f.Rotation = 0;
            bool flipX = f.FlipX, flipY = f.FlipY;
            f.FlipX = false;
            f.FlipY = false;
            f.Paint(dg);
            f.FlipX = flipX;
            f.FlipY = flipY;
            if (f is IAlphaBlendable)
                ((IAlphaBlendable)f).Alpha = alpha;
            f.Rotation = rot;
            // paint text wrap handle
            r = TextWrapHandleRect;
            if (WrapText)
                dg.FillEllipse(r, DColor.White);
            else
                dg.FillEllipse(r, DColor.LightGray);
            dg.DrawEllipse(r, DColor.Black);
            // paint selection & cursor
            string[] lines = Lines;
            DPoint offset = itext.TextOffset;
            DPoint pt = f.Rect.TopLeft.Offset(offset.X, offset.Y);
            double height = LineHeight(dg, lines);
            DPoint cpt = MeasureCursorPosition(lines, height);
            DRect[] selRects = MeasureSelectionRects(dg, lines, height);

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

        double LineHeight(DGraphics dg, string[] lines)
        {
            return TextHelper.MeasureText(dg, WrappedText, this).Y / lines.Length;
        }

        double LineHeight(string[] lines)
        {
            return TextHelper.MeasureText(WrappedText, this).Y / lines.Length;
        }

        DPoint MeasureCursorPosition(string[] lines, double height)
        {
            // find the x,y position of the cursor
            int pos = WrapPos(WrappedText, cursorPosition);
            if (pos >= 0)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (pos <= line.Length)
                    {
                        DPoint sz = TextHelper.MeasureText(line.Substring(0, pos), this);
                        return new DPoint(sz.X, height * i);
                    }
                    pos -= line.Length + 1;
                }
            }
            return new DPoint(0, 0);
        }

        DRect[] MeasureSelectionRects(DGraphics dg, string[] lines, double lineHeight)
        {
            DRect[] res = new DRect[0];
            if (selectionLength != 0)
            {
                // init selection variables
                int selStart;
                int selEnd;
                string wt = WrappedText; 
                if (selectionLength < 0)
                {
                    selStart = WrapPos(wt, cursorPosition + selectionLength);
                    selEnd = WrapPos(wt, cursorPosition);
                }
                else
                {
                    selStart = WrapPos(wt, cursorPosition);
                    selEnd = WrapPos(wt, cursorPosition + selectionLength);
                }
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
                            x = TextHelper.MeasureText(dg, line.Substring(0, selIdx), this).X;
                        else
                            x = 0;
                        // width of selection rect for this line
                        double w = TextHelper.MeasureText(dg, selText, this).X;
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
                newXPos = TextHelper.MeasureText(newLine.Substring(0, n), this).X;
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

        int WrapPos(string wrappedText, int pos)
        {
            // find the equvalent cursor position in wrapped text to one given from the base text
            if (!WrapText)
                return pos;
            int diff = 0;
            for (int i = 0; i < pos; i++)
                if (Text[i] != wrappedText[i + diff])
                    diff++;
            return pos + diff;
        }

        int UnwrapPos(string wrappedText, int pos)
        {
            // find the equvalent cursor position in base text to one given from the wrapped text
            if (!WrapText)
                return pos;
            int diff = 0;
            for (int i = 0; i < pos; i++)
                if (wrappedText[i] != Text[i + diff])
                    diff--;
            return pos + diff;
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
                    SetCursorPos(UnwrapPos(WrappedText,
                        cursorPosition + CharNumberChange(lines, lineNo, newLineNo, currentlineCharNo, newLineCharNo)
                        ), select);
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

        public void SetCursorPoint(DPoint pt, bool select)
        {
            DPoint offset = TextOffset;
            pt = new DPoint(pt.X - X - offset.X, pt.Y - Y - offset.Y);
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
                        double width = TextHelper.MeasureText(substr, this).X;
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
            SetCursorPos(UnwrapPos(WrappedText, pos), select);
        }

        public bool HitTestBorder(DPoint pt)
        {
            if (DGeom.PointInRect(pt, Rect))
            {
                DRect innerBorder = new DRect(X + border, Y + border, Width - border * 2, Height - border * 2);
                if (!DGeom.PointInRect(pt, innerBorder))
                    return true;
            }
            return false;
        }

        public bool HitTestTextWrapHandle(DPoint pt)
        {
            return DGeom.PointInRect(pt, TextWrapHandleRect);
        }

        delegate int FindCharDelegate(bool findWhiteSpace, bool iterForward, int offset);
        public void DoubleClick(DPoint pt)
        {
            FindCharDelegate findChar = delegate(bool findWhiteSpace, bool iterForward, int offset)
            {
                if (iterForward)
                    while (cursorPosition + offset < Text.Length - 1 && Char.IsWhiteSpace(Text[cursorPosition + offset + 1]) == findWhiteSpace)
                        offset++;
                else
                    while (cursorPosition + offset > 0 && Char.IsWhiteSpace(Text[cursorPosition + offset - 1]) == findWhiteSpace)
                        offset--;
                return offset;
            };
            // set initial cursor point
            SetCursorPoint(pt, false);
            // check if any text
            if (Text.Length > 0)
            {
                // make sure cursor not at end
                if (cursorPosition == Text.Length)
                    SetCursorPos(cursorPosition - 1, false);
                // find word & whitespace pair
                int end = 0, start = 0;
                if (Char.IsWhiteSpace(Text[cursorPosition]))
                {
                    end = findChar(true, true, 0) + 1;
                    start = findChar(true, false, 0);
                    start = findChar(false, false, start);
                }
                else
                {
                    end = findChar(false, true, 0);
                    end = findChar(true, true, end) + 1;
                    start = findChar(false, false, 0);
                }
                // select chars
                SetCursorPos(cursorPosition + start, false);
                SetCursorPos(cursorPosition - start + end, true);
            }
        }
    }

    public class GroupFigure : RectbaseFigure, IChildFigureable
    {
        public override double MinSize
        {
            get { return 10; }
        }

        public override bool MouseOver
        {
            set
            {
                base.MouseOver = value;
                foreach (Figure f in childFigs)
                    f.MouseOver = value;
            }
        }
        
        public override double X
        {
            get { return boundingBox.X; }
            set
            {
                if (value != X)
                {
                    double dX = value - X;
                    originalRect.X += dX;
                    int i = 0;
                    foreach (Figure f in childFigs)
                    {
                        DRect r = originalChildRects[i];
                        r.X += dX;
                        originalChildRects[i] = r;
                        f.X += dX;
                        i += 1;
                    }
                    CreateBoundingBox();
                }
            }
        }

        public override double Y
        {
            get { return boundingBox.Y; }
            set
            {
                if (value != Y)
                {
                    double dY = value - Y;
                    originalRect.Y += dY;
                    int i = 0;
                    foreach (Figure f in childFigs)
                    {
                        DRect r = originalChildRects[i];
                        r.Y += dY;
                        originalChildRects[i] = r;
                        f.Y += dY;
                        i += 1;
                    }
                    CreateBoundingBox();
                }
            }
        }

        public override double Width
        {
            get { return boundingBox.Width; }
            set
            {
                if (value != Width)
                {
                    double sx = value / originalRect.Width;
                    int i = 0;
                    foreach (Figure f in childFigs)
                    {
                        DRect ocr = originalChildRects[i];
                        f.X = originalRect.X + (sx * (ocr.X - originalRect.X));
                        f.Width = sx * ocr.Width;
                        i++;
                    }
                    CreateBoundingBox();
                }
            }
        }

        public override double Height
        {
            get { return boundingBox.Height; }
            set
            {
                if (value != Height)
                {
                    double sy = value / originalRect.Height;
                    int i = 0;
                    foreach (Figure f in childFigs)
                    {
                        DRect ocr = originalChildRects[i];
                        f.Y = originalRect.Y + (sy * (ocr.Y - originalRect.Y));
                        f.Height = sy * ocr.Height;
                        i++;
                    }
                    CreateBoundingBox();
                }
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

        UndoRedoList<DRect> originalChildRects = new UndoRedoList<DRect>();
        UndoRect originalRect = new UndoRect(new DRect());
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
            originalChildRects.Clear();
            foreach (Figure f in childFigs)
                originalChildRects.Add(f.Rect);
            CreateBoundingBox();
            originalRect.Rect = boundingBox.Rect;
        }

        protected DHitTest SelectAndBodyHitTest(DPoint pt, List<Figure> children, out IGlyph glyph)
        {
            DHitTest res = DHitTest.None;
            glyph = null;
            for (int i = childFigs.Count - 1; i >= 0; i--)
            {
                Figure f = childFigs[i];
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
            return res;
        }

        public override DHitTest HitTest(DPoint pt, List<Figure> children, out IGlyph glyph)
        {
            pt = RotatePointToFigure(pt);
            DHitTest res = GlyphHitTest(pt, out glyph);
            if (res == DHitTest.None)
            {
                if (Locked)
                {
                    res = LockHitTest(pt);
                    if (res == DHitTest.None)
                        res = SelectAndBodyHitTest(FlipPointToFigure(pt), children, out glyph);
                }
                else
                {
                    res = RotateHitTest(pt);
                    if (res == DHitTest.None)
                    {
                        res = ResizeHitTest(pt);
                        if (res == DHitTest.None)
                        {
                            res = ContextHitTest(pt);
                            if (res == DHitTest.None)
                                res = SelectAndBodyHitTest(FlipPointToFigure(pt), children, out glyph);
                        }
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
            DPoint ctr = Rect.Center;
            DRect r = DGeom.BoundingBoxOfRotatedRect(childFigs[0].Rect, childFigs[0].Rotation);
            foreach (Figure f in childFigs)
            {
                DRect r2 = DGeom.BoundingBoxOfRotatedRect(f.GetSelectRect(), f.Rotation);
                // flip
                if (FlipX)
                    r2.X -= (r2.Center.X - ctr.X) * 2;
                if (FlipY)
                    r2.Y -= (r2.Center.Y - ctr.Y) * 2;
                // merge
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
