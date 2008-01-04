using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDraw
{
    public static class GraphicsHelper
    {
        static Type _bitmapClass;
        static Type _graphicsClass;

        public static DBitmap MakeBitmap(double width, double height)
        {
            return (DBitmap)Activator.CreateInstance(_bitmapClass, new object[] { (int)Math.Ceiling(width + 1), (int)Math.Ceiling(height + 1) });
        }

        public static DGraphics MakeGraphics(DBitmap bmp)
        {
            return (DGraphics)Activator.CreateInstance(_graphicsClass, new object[] { bmp });
        }

        public static DPoint MeasureText(string text, string fontName, double fontSize)
        {
            return MeasureText(text, fontName, fontSize, false, false, false, false);
        }

        public static DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough)
        {
            DBitmap bmp = MakeBitmap(10, 10);
            DGraphics dg = MakeGraphics(bmp);
            DPoint sz = dg.MeasureText(text, fontName, fontSize, bold, italics, underline, strikethrough);
            dg.Dispose();
            bmp.Dispose();
            return sz;
        }

        public static void Init(Type bitmapClass, Type graphicsClass)
        {
            // bitmapClass needs to be a desendant of DBitmap
            _bitmapClass = bitmapClass;
            // graphicsClass needs to be a desendant of DGraphics
            _graphicsClass = graphicsClass;
        }
    }

    public abstract class DBitmap
    {
        protected object nativeBmp;
        public object NativeBmp
        {
            get { return nativeBmp; }
        }

        public DBitmap()
        {
        }

        public DBitmap(int width, int height)
        {
            nativeBmp = MakeBitmap(width, height);
        }

        public DBitmap(string filename)
        {
            nativeBmp = LoadBitmap(filename);
        }

        public DBitmap(Stream s)
        {
            nativeBmp = LoadBitmap(s);
        }

        ~DBitmap()
        {
            Dispose();
        }
        
        public bool Equals(DBitmap bitmap)
        {
            return nativeBmp == bitmap.NativeBmp;
        }

        protected abstract object MakeBitmap(int width, int height);
        protected abstract object LoadBitmap(string filename);
        protected abstract object LoadBitmap(Stream s);
        public abstract void Dispose();
        public abstract int Width
        {
            get;
        }
        public abstract int Height
        {
            get;
        }
        public abstract void Save(string filename);
    }

    public enum DCompositingMode { SourceOver, SourceCopy };

    public abstract class DGraphics
    {
        public static void Init(Type bitmapClass, Type graphicsClass)
        {
            GraphicsHelper.Init(bitmapClass, graphicsClass);
        }

        // Abstract Drawing Methods //

        abstract public void FillRect(double x, double y, double width, double height, DColor color, double alpha);
        abstract public void FillRect(double x, double y, double width, double height, DColor color, double alpha, DFillStyle fillStyle);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin);
        abstract public void DrawRect(DRect rect, DColor color);
        abstract public void DrawRect(DRect rect, DColor color, double alpha);
        abstract public void DrawRect(DRect rect, DColor color, double alpha, DStrokeStyle strokeStyle);
        abstract public void FillEllipse(double x, double y, double width, double height, DColor color);
        abstract public void FillEllipse(double x, double y, double width, double height, DColor color, double alpha);
        abstract public void FillEllipse(DRect rect, DColor color);
        abstract public void FillEllipse(DRect rect, DColor color, double alpha);
        abstract public void DrawEllipse(double x, double y, double width, double height, DColor color);
        abstract public void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle);
        abstract public void DrawEllipse(DRect rect, DColor color);
        abstract public void DrawEllipse(DRect rect, DColor color, double alpha);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, DStrokeStyle strokeStyle);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle, double strokeWidth, DStrokeCap strokeCap);
        abstract public void DrawPolyline(DPoints pts, DColor color);
        abstract public void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap);
        abstract public void FillPolygon(DPoints pts, DColor color, double alpha);
        abstract public void DrawBitmap(DBitmap bitmap, DPoint pt);
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect);
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect, double alpha);
        abstract public void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color);
        abstract public void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color, double alpha);
        abstract public void DrawText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough, DPoint pt, DColor color, double alpha);
        abstract public DPoint MeasureText(string text, string fontName, double fontSize);
        abstract public DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough);

        abstract public DMatrix SaveTransform();
        abstract public void LoadTransform(DMatrix matrix);
        abstract public void Scale(double sx, double sy);
        abstract public void Rotate(double angle, DPoint center);
        abstract public void Translate(DPoint offset);
        abstract public void ResetTransform();

        abstract public DCompositingMode CompositingMode
        {
            get;
            set;
        }
        abstract public bool AntiAlias
        {
            get;
            set;
        }

        public abstract void Dispose();
    }
}
