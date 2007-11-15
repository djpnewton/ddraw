using System;
using Gtk;

namespace DDraw.GTK
{
    public class GTKViewer : DViewer
    {
        GTKViewerControl control;
        //Cursor RotateCursor;

        public GTKViewer(GTKViewerControl c)
        {
            control = c;
            control.ExposeEvent += new ExposeEventHandler(control_Expose);
            control.ButtonPressEvent += new ButtonPressEventHandler(control_ButtonPress);
            control.MotionNotifyEvent += new MotionNotifyEventHandler(control_MotionNotify);
            control.ButtonReleaseEvent += new ButtonReleaseEventHandler(control_ButtonRelease);
            control.SizeAllocated += new SizeAllocatedHandler(control_SizeAllocated);
            /*


            RotateCursor = new Cursor(Res.Resource.RotateIcon.GetHicon());*/
        }
        
        void control_Expose(object sender, ExposeEventArgs a)
        {
            Cairo.Context cr = Gdk.CairoHelper.Create(a.Event.Window);
                        
            dg = new GTKGraphics(cr);
            dg.AntiAlias = AntiAlias;

            cr.Color = new Cairo.Color(1, 1, 1);
            Gdk.CairoHelper.Rectangle(cr, a.Event.Area);
            cr.Fill();

            DoNeedRepaint();
            
            a.RetVal = true;
        }

        DMouseButton MouseButtonFromGTKEvent(uint button)
        {
            switch (button)
            {
                case 1:
                    return DMouseButton.Right;
                case 2:
                    return DMouseButton.Middle;
                default:
                    return DMouseButton.Left;
            }
        }
        
        void control_ButtonPress(object sender, ButtonPressEventArgs a)
        {
            if (EditFigures)
                DoMouseDown(MouseButtonFromGTKEvent(a.Event.Button), new DPoint(a.Event.X, a.Event.Y));
        }
        
        void control_MotionNotify(object sender, MotionNotifyEventArgs a)
        {
            if (EditFigures)
                DoMouseMove(new DPoint(a.Event.X, a.Event.Y));
        }
        
        void control_ButtonRelease(object sender, ButtonReleaseEventArgs a)
        {
            if (EditFigures)
                DoMouseUp(MouseButtonFromGTKEvent(a.Event.Button), new DPoint(a.Event.X, a.Event.Y));
        }
        
        void control_SizeAllocated(object sender, SizeAllocatedArgs a)
        {
        }
        
        // Implemented Abstract Methods

        public override void Update()
        {
            control.QueueDraw();
        }

        public override void Update(DRect rect)
        {
            rect = rect.Offset(-1, -1);
            rect = rect.Inflate(2, 2);
            control.QueueDrawArea((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public override void SetCursor(DCursor cursor)
        {
            switch (cursor)
            {
                case DCursor.Default:
                    control.GdkWindow.Cursor = null;
                    break;
                case DCursor.MoveAll:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Fleur);
                    break;
                case DCursor.MoveNS:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.SbVDoubleArrow);
                    break;
                case DCursor.MoveWE:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.SbHDoubleArrow);
                    break;
                case DCursor.MoveNWSE:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.BottomRightCorner);
                    break;
                case DCursor.MoveNESW:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.BottomLeftCorner);
                    break;
                case DCursor.Rotate:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Exchange);
                    break;
                case DCursor.Crosshair:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Crosshair);
                    break;
            }
        }
    }
}
