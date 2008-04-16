using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace DDraw.WinForms
{
    public class WFViewer : DTkViewer
    {
        WFViewerControl control;
        Cursor RotateCursor;
        Point mousePt;
		System.Diagnostics.Stopwatch stopWatch;

        protected override void UpdateAutoScroll()
        {
            if (Preview)
                control.AutoScrollMinSize = new Size(0, 0);
            else
                control.AutoScrollMinSize = new Size(PgSzX + MARGIN * 2, PgSzY + MARGIN * 2);
            Update();
        }
        
        bool preview = false;
        public override bool Preview {
            get { return preview; }
            set 
            {
                preview = value;
                UpdateAutoScroll();
            }
        }

        DPoint pageSize = new DPoint(100, 100);
        public override void SetPageSize(DPoint pageSize)
        {
            this.pageSize = pageSize;
            UpdateAutoScroll();
        }
        protected override DPoint PageSize {
            get { return pageSize; }
        }


        protected override int HortScroll
        {
            get 
            { 
                if (control.HorizontalScroll.Visible) return control.HorizontalScroll.Value;
                else return 0;
            }
        }
        protected override int VertScroll
        {
            get 
            {
                if (control.VerticalScroll.Visible) return control.VerticalScroll.Value;
                else return 0;
            }
        }
        protected override int OffsetX
        {
            get 
            {
                if (preview)
                    return 0;
                if (control.Width > PgSzX + MARGIN * 2) return (control.Width - PgSzX) / 2;
                else return MARGIN;
            }
        }
        protected override int OffsetY
        {
            get 
            {
                if (preview)
                    return 0;
                if (control.Height > PgSzY + MARGIN * 2) return (control.Height - PgSzY) / 2;
                else return MARGIN;
            }
        }

        protected override int Width
        {
            get { return control.Width; }
        }
        protected override int Height
        {
            get { return control.Height; }
        }
        protected override DPoint CanvasOffset()
        {
            return new DPoint(-HortScroll + OffsetX, -VertScroll + OffsetY);
        }

        public WFViewer(WFViewerControl c)
        {
            control = c;
            control.Paint += new PaintEventHandler(control_Paint);
            control.MouseDown += new MouseEventHandler(control_MouseDown);
            control.MouseMove += new MouseEventHandler(control_MouseMove);
            control.MouseUp += new MouseEventHandler(control_MouseUp);
            control.DoubleClick += new EventHandler(control_DoubleClick);
            control.KeyDown += new KeyEventHandler(control_KeyDown);
            control.KeyPress += new KeyPressEventHandler(control_KeyPress);
            control.KeyUp += new KeyEventHandler(control_KeyUp);
            control.SizeChanged += new EventHandler(control_SizeChanged);
            control.Scroll += new ScrollEventHandler(control_Scroll);

            RotateCursor = new Cursor(Resource1.RotateIcon.GetHicon());

            stopWatch = new System.Diagnostics.Stopwatch();
        }

        ~WFViewer()
        {

        }

        void control_Paint(object sender, PaintEventArgs e)
        {
            // start stopwatch
			stopWatch.Start();
            // create DGraphics object for the paint routine
            DGraphics dg = new WFGraphics(e.Graphics);
            // call paint events
            DoNeedRepaint(dg);
            // clear DGraphics
            dg = null; // no need to dispose (we did not create the base GDI graphics object)
            // stop stopwatch and report duration
            stopWatch.Stop();
			DoDebugMessage("control_Paint duration: " + stopWatch.ElapsedMilliseconds.ToString());
			stopWatch.Reset();
        }

        DMouseButton MouseButtonFromWFEvent(MouseButtons btns)
        {
            switch (btns)
            {
                case MouseButtons.Right: return DMouseButton.Right;
                case MouseButtons.Middle: return DMouseButton.Middle;
                default: return DMouseButton.Left;
            }
        }
        
        DPoint MousePt(double x, double y)
        {
            return new DPoint((x + HortScroll - OffsetX) * (1 / Scale), (y + VertScroll - OffsetY) * (1 / Scale));
        }
            
        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (EditFigures)
            {
                DoMouseDown(MouseButtonFromWFEvent(e.Button), MousePt(e.X, e.Y));
                control.Focus();
            }
        }

        void control_MouseMove(object sender, MouseEventArgs e)
        {
            mousePt = e.Location;
            if (EditFigures)
                DoMouseMove(MousePt(e.X, e.Y));
        }

        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (EditFigures)
                DoMouseUp(MouseButtonFromWFEvent(e.Button), MousePt(e.X, e.Y));
        }

        void control_DoubleClick(object sender, EventArgs e)
        {
            if (EditFigures)
                DoDoubleClick(MousePt(mousePt.X, mousePt.Y));
        }

        DKey KeyFromWFEvent(KeyEventArgs e)
        {
            return new DKey(e.KeyValue, e.Shift, e.Control, e.Alt);
        }

        void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (EditFigures)
            {
                DoKeyDown(KeyFromWFEvent(e));
                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        DoKeyPress((int)DKeys.Delete);
                        break;
                    case Keys.Left:
                        DoKeyPress((int)DKeys.Left);
                        break;
                    case Keys.Right:
                        DoKeyPress((int)DKeys.Right);
                        break;
                    case Keys.Up:
                        DoKeyPress((int)DKeys.Up);
                        break;
                    case Keys.Down:
                        DoKeyPress((int)DKeys.Down);
                        break;
                    case Keys.Home:
                        DoKeyPress((int)DKeys.Home);
                        break;
                    case Keys.End:
                        DoKeyPress((int)DKeys.End);
                        break;
                    case Keys.PageUp:
                        DoKeyPress((int)DKeys.PageUp);
                        break;
                    case Keys.PageDown:
                        DoKeyPress((int)DKeys.PageDown);
                        break;
                }
            }
        }

        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (EditFigures)
                DoKeyPress(e.KeyChar);
        }

        void control_KeyUp(object sender, KeyEventArgs e)
        {
            if (EditFigures)
            {
                DoKeyUp(KeyFromWFEvent(e));
            }
        }

        void control_SizeChanged(object sender, EventArgs e)
        {
            if (Zoom != Zoom.Custom)
                Zoom = Zoom;
            Update();
        }
        
        void control_Scroll(object sender, ScrollEventArgs e)
        {
            Update();
        }

        // Implemented Abstract Methods

        public override void Update()
        {
            control.Invalidate();
        }

        public override void Update(DRect rect)
        {
            Rectangle r = new Rectangle((int)((rect.X * Scale) - HortScroll + OffsetX),
                                        (int)((rect.Y * Scale) - VertScroll + OffsetY), 
                                        (int)(rect.Width * Scale), (int)(rect.Height * Scale));
            r.Inflate(3, 3); // one for cast to int, one for antialiasing and one for GDI+
            control.Invalidate(r);
        }

        public override void SetCursor(DCursor cursor)
        {
            switch (cursor)
            {
                case DCursor.Default:
                    control.Cursor = Cursors.Default;
                    break;
                case DCursor.MoveAll:
                    control.Cursor = Cursors.SizeAll;
                    break;
                case DCursor.MoveNS:
                    control.Cursor = Cursors.SizeNS;
                    break;
                case DCursor.MoveWE:
                    control.Cursor = Cursors.SizeWE;
                    break;
                case DCursor.MoveNWSE:
                    control.Cursor = Cursors.SizeNWSE;
                    break;
                case DCursor.MoveNESW:
                    control.Cursor = Cursors.SizeNESW;
                    break;
                case DCursor.Rotate:
                    control.Cursor = RotateCursor;
                    break;
                case DCursor.Crosshair:
                    control.Cursor = Cursors.Cross;
                    break;
                case DCursor.IBeam:
                    control.Cursor = Cursors.IBeam;
                    break;
                case DCursor.Hand:
                    control.Cursor = Cursors.Hand;
                    break;
            }
        }
    }
    
    public class WFPrintSettings : DPrintSettings
    {
        public PageSettings PageSettings;

        public WFPrintSettings(PageSettings pageSettings)
        {
            PageSettings = pageSettings;
        }
        
        public override double MarginLeft
        {
            get { return PageSettings.Margins.Left; }
        }
        public override double MarginTop
        {
            get { return PageSettings.Margins.Top; }
        }
        public override double MarginRight
        {
            get { return PageSettings.Margins.Right; }
        }
        public override double MarginBottom
        {
            get { return PageSettings.Margins.Bottom; }
        }
        public override double PageWidth
        {
            get { return PageSettings.PaperSize.Width; }
        }
        public override double PageHeight
        {
            get { return PageSettings.PaperSize.Height; }
        }
    }
}
