using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public delegate void DPaintEventHandler(DViewer dv);
    public delegate void DMouseButtonEventHandler(DViewer dv, DMouseButton btn, DPoint pt);
    public delegate void DMouseMoveEventHandler(DViewer dv, DPoint pt);
    public delegate void DKeyEventHandler(DViewer dv, DKey k);
    public delegate void DKeyPressEventHandler(DViewer dv, char k);

    abstract public class DViewer
    {
        protected DGraphics dg = null;

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
        public event DMouseButtonEventHandler MouseDown;
        public event DMouseMoveEventHandler MouseMove;
        public event DMouseButtonEventHandler MouseUp;
        public event DMouseMoveEventHandler DoubleClick;
        public event DKeyEventHandler KeyDown;
        public event DKeyPressEventHandler KeyPress;
        public event DKeyEventHandler KeyUp;
        public event DebugMessageHandler DebugMessage;

        public DViewer()
        {

        }

        protected void DoNeedRepaint()
        {
            if (NeedRepaint != null)
                NeedRepaint(this);
        }

        protected void DoMouseDown(DMouseButton btn, DPoint pt)
        {
            if (MouseDown != null)
                MouseDown(this, btn, pt);
        }

        protected void DoMouseMove(DPoint pt)
        {
            if (MouseMove != null)
                MouseMove(this, pt);
        }

        protected void DoMouseUp(DMouseButton btn, DPoint pt)
        {
            if (MouseUp != null)
                MouseUp(this, btn, pt);
        }

        protected void DoDoubleClick(DPoint pt)
        {
            if (DoubleClick != null)
                DoubleClick(this, pt);
        }

        protected void DoKeyDown(DKey k)
        {
            if (KeyDown != null)
                KeyDown(this, k);
        }

        protected void DoKeyPress(char k)
        {
            if (KeyPress != null)
                KeyPress(this, k);
        }

        protected void DoKeyUp(DKey k)
        {
            if (KeyUp != null)
                KeyUp(this, k);
        }

        public void Paint(List<Figure> figures, bool drawSelectionRect, Figure selectionRect)
        {
            dg.Scale(ScaleX, ScaleY);

            // paint figures
            foreach (Figure figure in figures)
                figure.Paint(dg);
            if (editFigures)
            {
                foreach (Figure figure in figures)
                    figure.PaintSelectionChrome(dg);
                if (drawSelectionRect)
                    selectionRect.Paint(dg);
            }
        }

        // Abstract Methods //

        abstract public void Update();
        abstract public void Update(DRect rect);
        abstract public void SetCursor(DCursor cursor);

        // Other //

        protected void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
    }
}
