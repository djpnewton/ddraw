using System;
using Gtk;

namespace DDraw.GTK
{
	public class GTKViewerControl : DrawingArea
	{
		public GTKViewerControl()
		{
			Events = Gdk.EventMask.AllEventsMask;
		}
	}
}
