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
        static Type _textExtentClass;
        static DTextExtent te = null;

        public static DBitmap MakeBitmap(double width, double height)
        {
            return (DBitmap)Activator.CreateInstance(_bitmapClass, new object[] { (int)Math.Ceiling(width + 1), (int)Math.Ceiling(height + 1) });
        }

        public static DGraphics MakeGraphics(DBitmap bmp)
        {
            return (DGraphics)Activator.CreateInstance(_graphicsClass, new object[] { bmp });
        }

        public static DTextExtent TextExtent
        {
            get
            {
                if (te == null)
                    te = (DTextExtent)Activator.CreateInstance(_textExtentClass);
                return te;
            }
        }

        public static void Init(Type bitmapClass, Type graphicsClass, Type textExtentClass)
        {
            // bitmapClass needs to be a desendant of DBitmap
            _bitmapClass = bitmapClass;
            // graphicsClass needs to be a desendant of DGraphics
            _graphicsClass = graphicsClass;
            // textExtentClass needs to be a desendant of DTextExtent
            _textExtentClass = textExtentClass;
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

    public abstract class DTextExtent
    {
        public abstract DPoint MeasureText(string text, string fontName, double fontSize);
    }

    public enum DCompositingMode { SourceOver, SourceCopy };

    public abstract class DGraphics
    {
        public static void Init(Type bitmapClass, Type graphicsClass, Type textExtentClass)
        {
            GraphicsHelper.Init(bitmapClass, graphicsClass, textExtentClass);
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
