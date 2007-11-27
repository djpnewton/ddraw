using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DDraw.WinForms
{
    public class WFViewer : DViewer
    {
        WFViewerControl control;
        Cursor RotateCursor;
        Point mousePt;
		System.Diagnostics.Stopwatch stopWatch;
        
        void UpdateAutoScroll()
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

        DPoint pageSize;
        public override void SetPageSize(DPoint pageSize)
        {
            this.pageSize = pageSize;
            UpdateAutoScroll();
        }
        protected override DPoint PageSize {
            get { return pageSize; }
        }

        
        int HortScroll
        {
            get 
            { 
                if (control.HorizontalScroll.Visible) return control.HorizontalScroll.Value;
                else return 0;
            }
        }
        int VertScroll
        {
            get 
            {
                if (control.VerticalScroll.Visible) return control.VerticalScroll.Value;
                else return 0;
            }
        }
        
        int OffsetX
        {
            get 
            {
                if (control.Width > PgSzX + MARGIN * 2) return (control.Width - PgSzX) / 2;
                else return MARGIN;
            }
        }   
        int OffsetY
        {
            get 
            {
                if (control.Height > PgSzY + MARGIN * 2) return (control.Height - PgSzY) / 2;
                else return MARGIN;
            }
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
			stopWatch.Start();

            dg = new WFGraphics(e.Graphics);
            dg.AntiAlias = AntiAlias;
                
            if (preview)
            {
                e.Graphics.FillRectangle(Brushes.White, control.ClientRectangle);
                e.Graphics.ScaleTransform(control.Width / (float)pageSize.X, control.Height / (float)pageSize.Y);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.LightGray, control.ClientRectangle);
                e.Graphics.TranslateTransform(-HortScroll + OffsetX, -VertScroll + OffsetY);
                e.Graphics.FillRectangle(Brushes.Black, SHADOW_OFFSET, SHADOW_OFFSET, (float)pageSize.X, (float)pageSize.Y);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, (float)pageSize.X, (float)pageSize.Y);
            }

            DoNeedRepaint();

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
            return new DPoint(x + HortScroll - OffsetX, y + VertScroll - OffsetY);
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
                DoKeyDown(KeyFromWFEvent(e));
        }

        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (EditFigures)
                DoKeyPress(e.KeyChar);
        }

        void control_KeyUp(object sender, KeyEventArgs e)
        {
            if (EditFigures)
                DoKeyUp(KeyFromWFEvent(e));
        }

        void control_SizeChanged(object sender, EventArgs e)
        {
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
            Rectangle r = new Rectangle((int)(rect.X) - HortScroll + OffsetX,
                                        (int)(rect.Y) - VertScroll + OffsetY, 
                                        (int)(rect.Width), (int)(rect.Height));
            r.Inflate(1, 1);
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
            }
        }
    }
}
