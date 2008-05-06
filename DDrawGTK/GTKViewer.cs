using System;
using System.Collections.Generic;
using Gtk;

using DDrawCairo;

namespace DDraw.GTK
{
    public class GTKViewer : DTkViewer
    {
        GTKViewerControl control;

        protected override void UpdateAutoScroll()
        {
            if (Preview)
                control.SetSize(0, 0);
            else
                control.SetSize((uint)(PgSzX + MARGIN * 2), (uint)(PgSzY + MARGIN * 2));
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
            get { return (int)Math.Round(control.Hadjustment.Value); }
        }
        protected override int VertScroll
        {
            get { return (int)Math.Round(control.Vadjustment.Value); }
        }
        protected override int OffsetX
        {
            get 
            {
                if (preview)
                    return 0;
                int width = control.Allocation.Width;
                if (width > PgSzX + MARGIN * 2) return (width - PgSzX) / 2;
                else return MARGIN;
            }
        }
        protected override int OffsetY
        {
            get 
            {
                if (preview)
                    return 0;
                int height = control.Allocation.Height;
                if (height > PgSzY + MARGIN * 2) return (height - PgSzY) / 2;
                else return MARGIN;
            }
        }

        protected override int Width
        {
            get { return control.Allocation.Width; }
        }
        protected override int Height
        {
            get { return control.Allocation.Height; }
        }
        protected override DPoint CanvasOffset()
        {
            return new DPoint(OffsetX, OffsetY);
        }

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
            control.ScrollEvent += new ScrollEventHandler(control_ScrollEvent);
        }

        void control_Expose(object sender, ExposeEventArgs a)
        {
            // return if window is not BinWindow (Gtk.Layout has two windows)
            if (a.Event.Window != control.BinWindow)
                return;
            // create DGraphics object for the paint routine
            Cairo.Context cr = CairoHelper.CreateContext(a.Event.Window);
            DGraphics dg = new CairoGraphics(cr);
            // call paint events
            DoNeedRepaint(dg);
            // free graphics resources
            ((IDisposable)cr).Dispose();
            cr = null;
            dg = null;
            // dealt withh Expose event
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
        
        DPoint MousePt(double x, double y)
        {
            return new DPoint((x - OffsetX) * (1 / Scale), (y - OffsetY) * (1 / Scale));
        }
        
        void control_ButtonPress(object sender, ButtonPressEventArgs a)
        {
            if (EditFigures)
            {
                if (a.Event.Type == Gdk.EventType.ButtonPress)
                {
                    DoMouseDown(MouseButtonFromGTKEvent(a.Event.Button), MousePt(a.Event.X, a.Event.Y));
                    control.GrabFocus();
                }
                else if (a.Event.Type == Gdk.EventType.TwoButtonPress)
                    DoDoubleClick(MousePt(a.Event.X, a.Event.Y));
            }
        }
        
        void control_MotionNotify(object sender, MotionNotifyEventArgs a)
        {
            if (EditFigures)
                DoMouseMove(MousePt(a.Event.X, a.Event.Y));
        }
        
        void control_ButtonRelease(object sender, ButtonReleaseEventArgs a)
        {
            if (EditFigures)
                DoMouseUp(MouseButtonFromGTKEvent(a.Event.Button), MousePt(a.Event.X, a.Event.Y));
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
                // try to match Gdk.KeyValue DKeys or ascii
                switch (args.Event.Key)
                {
                    case Gdk.Key.Return:
                        DoKeyPress((int)DKeys.Enter);
                        break;
                    case Gdk.Key.KP_Enter:
                        goto case Gdk.Key.Return;
                    case Gdk.Key.Escape:
                        DoKeyPress((int)DKeys.Escape);
                        break;
                    case Gdk.Key.BackSpace:
                        DoKeyPress((int)DKeys.Backspace);
                        break;
                    case Gdk.Key.Delete:
                        DoKeyPress((int)DKeys.Delete);
                        break;
                    case Gdk.Key.Left:
                        DoKeyPress((int)DKeys.Left);
                        break;
                    case Gdk.Key.Right:
                        DoKeyPress((int)DKeys.Right);
                        break;
                    case Gdk.Key.Up:
                        DoKeyPress((int)DKeys.Up);
                        break;
                    case Gdk.Key.Down:
                        DoKeyPress((int)DKeys.Down);
                        break;
                    case Gdk.Key.Home:
                        DoKeyPress((int)DKeys.Home);
                        break;
                    case Gdk.Key.End:
                        DoKeyPress((int)DKeys.End);
                        break;
                    case Gdk.Key.Page_Up:
                        DoKeyPress((int)DKeys.PageUp);
                        break;
                    case Gdk.Key.Page_Down:
                        DoKeyPress((int)DKeys.PageDown);
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
            if (Zoom != Zoom.Custom)
                Zoom = Zoom;
            Update();
        }
        
        void control_ScrollEvent(object sender, ScrollEventArgs a)
        {
            Update();
        }
        
        // Implemented Abstract Methods

        public override void Update()
        {
            control.QueueDraw();
        }

        public override void Update(DRect rect)
        {
            // modify rect to cover weirdness of the renderer, antialiasing and cast to int
            rect = rect.Offset(-1, -1);
            rect = rect.Inflate(3, 3);
            // update rect
            control.QueueDrawArea((int)((rect.X * Scale) - HortScroll + OffsetX),
                                  (int)((rect.Y * Scale) - VertScroll + OffsetY),
                                  (int)(rect.Width * Scale), (int)(rect.Height * Scale));
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
                case DCursor.Hand:
                    control.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
                    break;
            }
        }
    }
    
    public class GTKPrintSettings : DPrintSettings
    {
        public double DpiX;
        public double DpiY;
        public PageSetup PageSetup;

        public GTKPrintSettings(double dpiX, double dpiY, PageSetup pageSetup)
        {
            DpiX = dpiX;
            DpiY = dpiY;
            PageSetup = pageSetup;
        }

        Unit printUnit = Unit.Pixel;
        
        public override double MarginLeft
        {
            get { return PageSetup.GetLeftMargin(printUnit); }
        }
        public override double MarginTop
        {
            get { return PageSetup.GetTopMargin(printUnit); }
        }
        public override double MarginRight
        {
            get { return PageSetup.GetRightMargin(printUnit); }
        }
        public override double MarginBottom
        {
            get { return PageSetup.GetBottomMargin(printUnit); }
        }
        public override double PageWidth
        {
            get { return PageSetup.GetPageWidth(printUnit); }
        }
        public override double PageHeight
        {
            get { return PageSetup.GetPageHeight(printUnit); }
        }
    }
}
