using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public delegate void DPaintEventHandler(DTkViewer dv, DGraphics dg);
    public delegate void DMouseButtonEventHandler(DTkViewer dv, DMouseButton btn, DPoint pt);
    public delegate void DMouseMoveEventHandler(DTkViewer dv, DPoint pt);
    public delegate void DKeyEventHandler(DTkViewer dv, DKey k);
    public delegate void DKeyPressEventHandler(DTkViewer dv, int k);

    abstract public class DViewer
    {
        abstract public void SetPageSize(DPoint pageSize);
        abstract protected DPoint PageSize
        {
            get;
        }

        public event DebugMessageHandler DebugMessage;

        public DViewer()
        {

        }

        public void Paint(DGraphics dg, Figure backgroundFigure, IList<Figure> figures)
        {
            // paint figures
            if (backgroundFigure != null)
                backgroundFigure.Paint(dg);
            foreach (Figure figure in figures)
                figure.Paint(dg);
        }

        // Other //

        protected void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
    }

    /// <summary>
    /// base toolkit viewer/renderer class
    /// </summary>
    public abstract class DTkViewer : DViewer
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

        double scale = 1;
        public virtual double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                zoom = Zoom.Custom;
                UpdateAutoScroll();
            }
        }
        Zoom zoom = Zoom.Custom;
        public Zoom Zoom
        {
            get { return zoom; }
            set
            {
                switch (value)
                {
                    case Zoom.FitToPage:
                        if (PageSize.X / Width > PageSize.Y / Height)
                            goto case Zoom.FitToWidth;
                        else
                            Scale = (Height - MARGIN * 2) / PageSize.Y;
                        break;
                    case Zoom.FitToWidth:
                        Scale = (Width - MARGIN * 2) / PageSize.X;
                        break;
                    case Zoom.Custom:
                        System.Diagnostics.Debug.Assert(false, "ERROR: to use a custom zoom use the ZoomPercent property");
                        break;
                }
                zoom = value;
            }
        }

        abstract protected void UpdateAutoScroll();

        abstract public bool Preview
        {
            get;
            set;
        }

        protected const int SHADOW_OFFSET = 5;
        protected const int MARGIN = SHADOW_OFFSET * 2;

        protected int PgSzX
        {
            get { return (int)Math.Round(PageSize.X * scale); }
        }
        protected int PgSzY
        {
            get { return (int)Math.Round(PageSize.Y * scale); }
        }

        abstract protected int HortScroll { get; }
        abstract protected int VertScroll { get; }
        abstract protected int OffsetX { get; }
        abstract protected int OffsetY { get; }
        public DPoint EngineToClient(DPoint pt)
        {
            pt = new DPoint(pt.X * scale, pt.Y * scale);
            return pt.Offset(-HortScroll + OffsetX, -VertScroll + OffsetY);
        }

        abstract protected int Width { get; }
        abstract protected int Height { get; }
        abstract protected DPoint CanvasOffset();

        public event DPaintEventHandler NeedRepaint;
        public event DMouseButtonEventHandler MouseDown;
        public event DMouseMoveEventHandler MouseMove;
        public event DMouseButtonEventHandler MouseUp;
        public event DMouseMoveEventHandler DoubleClick;
        public event DKeyEventHandler KeyDown;
        public event DKeyPressEventHandler KeyPress;
        public event DKeyEventHandler KeyUp;

        public DTkViewer(): base()
        {

        }

        protected void DoNeedRepaint(DGraphics dg)
        {
            if (NeedRepaint != null)
                NeedRepaint(this, dg);
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

        protected void DoKeyPress(int k)
        {
            if (KeyPress != null)
                KeyPress(this, k);
        }

        protected void DoKeyUp(DKey k)
        {
            if (KeyUp != null)
                KeyUp(this, k);
        }

        public void Paint(DGraphics dg, Figure backgroundFigure, IList<Figure> figures, Figure[] controlFigures)
        {
            // set antialias value
            dg.AntiAlias = AntiAlias;
            // draw backround and transform canvas accordind to the pagesize
            if (Preview)
                dg.Scale(Width / PageSize.X, Height / PageSize.Y); // scale to width & height as this is a preview viewer
            else
            {
                dg.FillRect(0, 0, Width, Height, new DColor(200, 200, 200), 1); // gray background
                dg.Translate(CanvasOffset()); // center drawing
                dg.Scale(scale, scale); // scale canvas
                dg.FillRect(SHADOW_OFFSET, SHADOW_OFFSET, PageSize.X, PageSize.Y, DColor.Black, 1); // draw black canvas shadow
            }
            base.Paint(dg, backgroundFigure, figures);
            if (editFigures)
            {
                double invScale = 1 / scale;
                if (figures != null)
                    foreach (Figure figure in figures)
                    {
                        figure.Scale = invScale;
                        figure.PaintSelectionChrome(dg);
                        figure.PaintGlyphs(dg);
                    }
                if (controlFigures != null)
                    foreach (Figure figure in controlFigures)
                    {
                        figure.Scale = invScale;
                        figure.Paint(dg);
                    }
            }
        }

        // Abstract Methods //

        abstract public void Update();
        abstract public void Update(DRect rect);
        abstract public void SetCursor(DCursor cursor);
    }

    public abstract class DPrinterSettings
    {
    }

    public abstract class DPrintViewer : DViewer
    {
        DPoint pageSize;
        public override void SetPageSize(DPoint pageSize)
        {
            this.pageSize = pageSize;
        }
        protected override DPoint PageSize
        {
            get { return pageSize; }
        }
    }
}
