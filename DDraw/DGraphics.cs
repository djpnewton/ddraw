using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DDraw.WinForms;

namespace DDraw
{
    public enum DGraphicsSystem { WinForms };

    public static class GraphicsHelper
    {
        public static DGraphicsSystem GraphicsSystem = DGraphicsSystem.WinForms;

        public static DBitmap MakeBitmap(double width, double height)
        {
            switch (GraphicsSystem)
            {
                case DGraphicsSystem.WinForms:
                    return new WFBitmap((int)Math.Ceiling(width + 1), (int)Math.Ceiling(height + 1));
                default:
                    return null;
            }
        }

        public static DGraphics MakeGraphics(DBitmap bmp)
        {
            switch (GraphicsSystem)
            {
                case DGraphicsSystem.WinForms:
                    return new WFGraphics(bmp);
                default:
                    return null;
            }
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
            DisposeBitmap();
        }

        protected abstract object MakeBitmap(int width, int height);
        protected abstract object LoadBitmap(string filename);
        protected abstract object LoadBitmap(Stream s);
        protected abstract void DisposeBitmap();
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
        public static void Init(DGraphicsSystem gs)
        {
            GraphicsHelper.GraphicsSystem = gs;
        }

        // Abstract Drawing Methods //

        abstract public void FillRect(double x, double y, double width, double height, DColor color, double alpha);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth);
        abstract public void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DPenStyle penStyle);
        abstract public void DrawRect(DRect rect, DColor color);
        abstract public void DrawRect(DRect rect, DColor color, double alpha);
        abstract public void DrawRect(DRect rect, DColor color, double alpha, DPenStyle penStyle);
        abstract public void FillEllipse(double x, double y, double width, double height, DColor color);
        abstract public void FillEllipse(double x, double y, double width, double height, DColor color, double alpha);
        abstract public void FillEllipse(DRect rect, DColor color);
        abstract public void FillEllipse(DRect rect, DColor color, double alpha);
        abstract public void DrawEllipse(double x, double y, double width, double height, DColor color);
        abstract public void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth);
        abstract public void DrawEllipse(DRect rect, DColor color);
        abstract public void DrawEllipse(DRect rect, DColor color, double alpha);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, DPenStyle penStyle);
        abstract public void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DPenStyle penStyle);
        abstract public void DrawPolyline(DPoints pts, DColor color);
        abstract public void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth);
        abstract public void DrawBitmap(DBitmap bitmap, DPoint pt);
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect);
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect, double alpha);
        abstract public void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color);
        abstract public void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color, double alpha);

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
    }
}
