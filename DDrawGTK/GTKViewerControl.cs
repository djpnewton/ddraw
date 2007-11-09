using System;
using Gtk;

namespace DDrawGTK
{
    public class GTKViewerControl : DrawingArea
    {
        public GTKViewerControl()
        {
            Events = Gdk.EventMask.AllEventsMask;
        }
    }
}
