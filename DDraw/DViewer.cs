using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public delegate void DPaintEventHandler(DViewer dv);
    public delegate void DMouseEventHandler(DViewer dv, DPoint pt);

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
            dg.Scale(ScaleX, ScaleY);

            // paint figures
            foreach (Figure figure in figures)
                figure.Paint(dg, editFigures);
            if (drawSelectionRect && editFigures)
                selectionRect.Paint(dg, editFigures);
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
