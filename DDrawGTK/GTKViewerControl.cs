using System;
using Gtk;

namespace DDraw.GTK
{
    public class GTKViewerControl : DrawingArea
    {
        public GTKViewerControl()
        {
            CanFocus = true;
            Events = Gdk.EventMask.AllEventsMask;
        }
    }
}
