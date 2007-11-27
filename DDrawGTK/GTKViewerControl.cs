using System;
using Gtk;

namespace DDraw.GTK
{
    public class GTKViewerControl : Layout
    {
        public GTKViewerControl() : base(null, null)
        {
            CanFocus = true;
            Events = Gdk.EventMask.AllEventsMask;
        }
    }
}
