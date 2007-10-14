using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDraw
{
    public abstract class DBitmap
    {
        protected IntPtr handle;
        public IntPtr Handle
        {
            get { return handle; }
        }

        public DBitmap(string filename)
        {
            handle = LoadBitmap(filename);
        }

        public DBitmap(Stream s)
        {
            handle = LoadBitmap(s);
        }

        ~DBitmap()
        {
            DisposeBitmap();
        }

        protected abstract IntPtr LoadBitmap(string filename);
        protected abstract IntPtr LoadBitmap(Stream s);
        protected abstract void DisposeBitmap();
        public abstract int Width
        {
            get;
        }
        public abstract int Height
        {
            get;
        }
    }

    public abstract class DTextExtent
    {
        public abstract DPoint MeasureText(string text, string fontName, double fontSize);
    }

    public delegate void DPaintEventHandler(DViewer dv);
    public delegate void DMouseEventHandler(DViewer dv, DPoint pt);

    abstract public class DViewer
    {
        bool antiAlias = false;
        public bool AntiAlias
        {
            get { return antiAlias; }
            set
            {
                antiAlias = value;
                Update();
            }
        }

        bool editFigures = false;
        public bool EditFigures
        {
            get { return editFigures; }
            set 
            { 
                editFigures = value;
                Update();
            }
        }

        public double ScaleX = 1;
        public double ScaleY = 1;

        public event DPaintEventHandler NeedRepaint;
        public event DMouseEventHandler MouseDown;
        public event DMouseEventHandler MouseMove;
        public event DMouseEventHandler MouseUp;
        public event DebugMessageHandler DebugMessage;

        public DViewer()
        {

        }

        protected void DoNeedRepaint()
        {
            if (NeedRepaint != null)
                NeedRepaint(this);
        }

        protected void DoMouseDown(DPoint pt)
        {
            if (MouseDown != null)
                MouseDown(this, pt);
        }

        protected void DoMouseMove(DPoint pt)
        {
            if (MouseMove != null)
                MouseMove(this, pt);
        }

        protected void DoMouseUp(DPoint pt)
        {
            if (MouseUp != null)
                MouseUp(this, pt);
        }

        public void Paint(List<Figure> figures, bool drawSelectionRect, Figure selectionRect)
        {
            Scale(ScaleX, ScaleY);

            // paint figures
            foreach (Figure figure in figures)
                figure.Paint(this);
            if (drawSelectionRect && editFigures)
                selectionRect.Paint(this);
        }

        // Abstract Drawing Methods //

        abstract public void Update();
        abstract public void Update(DRect rect);
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
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect);
        abstract public void DrawBitmap(DBitmap bitmap, DRect rect, double alpha);
        abstract public void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color);
        abstract public void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color, double alpha);

        abstract public DMatrix SaveTransform();
        abstract public void LoadTransform(DMatrix matrix);
        abstract public void Scale(double sx, double sy);
        abstract public void Rotate(double angle, DPoint center);
        abstract public void ResetTransform();

        abstract public void SetCursor(DCursor cursor);

        // Other //

        protected void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
    }
}
