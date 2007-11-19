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

            e.Graphics.FillRectangle(Brushes.White, control.ClientRectangle);

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

        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (EditFigures)
            {
                DoMouseDown(MouseButtonFromWFEvent(e.Button), new DPoint(e.X, e.Y));
                control.Focus();
            }
        }

        void control_MouseMove(object sender, MouseEventArgs e)
        {
            mousePt = e.Location;
            if (EditFigures)
                DoMouseMove(new DPoint(e.X, e.Y));
        }

        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (EditFigures)
                DoMouseUp(MouseButtonFromWFEvent(e.Button), new DPoint(e.X, e.Y));
        }

        void control_DoubleClick(object sender, EventArgs e)
        {
            if (EditFigures)
                DoDoubleClick(new DPoint(mousePt.X, mousePt.Y));
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

        }

        // Implemented Abstract Methods

        public override void Update()
        {
            control.Invalidate();
        }

        public override void Update(DRect rect)
        {
            Rectangle r = new Rectangle((int)(rect.X), (int)(rect.Y), (int)(rect.Width), (int)(rect.Height));
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
