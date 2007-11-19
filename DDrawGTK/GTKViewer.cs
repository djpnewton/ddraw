using System;
using Gtk;

namespace DDraw.GTK
{
    public class GTKViewer : DViewer
    {
        GTKViewerControl control;

        public GTKViewer(GTKViewerControl c)
        {
            control = c;
            control.ExposeEvent += new ExposeEventHandler(control_Expose);
            control.ButtonPressEvent += new ButtonPressEventHandler(control_ButtonPress);
            control.MotionNotifyEvent += new MotionNotifyEventHandler(control_MotionNotify);
            control.ButtonReleaseEvent += new ButtonReleaseEventHandler(control_ButtonRelease);
            control.KeyPressEvent += new KeyPressEventHandler(control_KeyPressEvent);
            control.KeyReleaseEvent += new KeyReleaseEventHandler(control_KeyReleaseEvent);
            control.SizeAllocated += new SizeAllocatedHandler(control_SizeAllocated);
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
                case 3:
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
            {
                if (a.Event.Type == Gdk.EventType.ButtonPress)
                {
                    DoMouseDown(MouseButtonFromGTKEvent(a.Event.Button), new DPoint(a.Event.X, a.Event.Y));
                    control.GrabFocus();
                }
                else if (a.Event.Type == Gdk.EventType.TwoButtonPress)
                    DoDoubleClick(new DPoint(a.Event.X, a.Event.Y));
            }
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

        DKey EventKeyToDKey(Gdk.EventKey ek)
        {
            return new DKey((int)ek.KeyValue,
                (ek.State & Gdk.ModifierType.ShiftMask) != 0,
                (ek.State & Gdk.ModifierType.ControlMask) != 0,
                (ek.State & Gdk.ModifierType.Mod1Mask) != 0);
        }

        void control_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            if (EditFigures)
            {
                DKey k = EventKeyToDKey(args.Event);
                DoKeyDown(k);
                // try to match Gdk.KeyValue to ascii
                switch (args.Event.Key)
                {
                    case Gdk.Key.Return:
                        DoKeyPress('\r');
                        break;
                    case Gdk.Key.KP_Enter:
                        goto case Gdk.Key.Return;
                    case Gdk.Key.Escape:
                        DoKeyPress((char)27);
                        break;
                    case Gdk.Key.BackSpace:
                        DoKeyPress('\b');
                        break;
                    default:
                        // only call DoKeyPress if there is an ascii eqivalent of KeyValue 
                        // (not on modifier keys, arrow keys etc)
                        if (args.Event.KeyValue >= 32 && args.Event.KeyValue <= 126)
                            DoKeyPress((char)args.Event.KeyValue);
                        break;
                }
            }
        }

        void control_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (EditFigures)
                DoKeyUp(EventKeyToDKey(args.Event));
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
                case DCursor.IBeam:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Xterm);
                    break;
            }
        }
    }
}
