// /home/daniel/Projects/ddraw/GTKDemo/MainWindow.cs created with MonoDevelop
// User: daniel at 4:50 p 6/11/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;

using DDraw;
using DDraw.GTK;

namespace GTKDemo
{
	public class MainWindow : Window
	{		
		Label l;
		
		public MainWindow(): base("MainWindow")
		{          
            // create the table and pack into the window
            Table table = new Table(2, 3, false);
            Add(table);
            // Initialze DGraphics
            GTKGraphics.Init();
			// create DViewerControl and attach to table
			GTKViewerControl dvc = new GTKViewerControl();
            table.Attach(dvc, 0, 1, 0, 1, AttachOptions.Fill | AttachOptions.Expand, 
                         AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            // create the scrollbars and pack into the table
            VScrollbar vsb = new VScrollbar(null);
            table.Attach(vsb, 1, 2, 0, 1, AttachOptions.Fill|AttachOptions.Shrink,
                         AttachOptions.Fill|AttachOptions.Shrink, 0, 0);
            HScrollbar hsb = new HScrollbar(null);
			table.Attach(hsb, 0, 1, 1, 2, AttachOptions.Fill|AttachOptions.Shrink,
                         AttachOptions.Fill|AttachOptions.Shrink, 0, 0); 
            // tell the scrollbars to use the DViewerControl widget's adjustments
            vsb.Adjustment = dvc.Vadjustment;
            hsb.Adjustment = dvc.Hadjustment;
            // create debuging label        
			l = new Label("debug");
            table.Attach(l, 0, 1, 2, 3, AttachOptions.Fill|AttachOptions.Shrink, 
                         AttachOptions.Fill|AttachOptions.Shrink, 0, 0);
			// create DViewer and DEngine			
			GTKViewer dv = new GTKViewer(dvc);
			dv.EditFigures = true;
			dv.DebugMessage += new DebugMessageHandler(DebugMessage);
			DEngine de = new DEngine(new DAuthorProperties());
			de.AddViewer(dv);
			de.State = DEngineState.Select;
			de.DebugMessage += new DebugMessageHandler(DebugMessage);
			// add figures
			de.AddFigure(new EllipseFigure(new DRect(20, 30, 100, 100), 0));
			RectFigure rf = new RectFigure(new DRect(10, 20, 100, 100), 0);
			rf.Alpha = 0.7;
			rf.Fill = new DColor(80, 80, 80);
			de.AddFigure(rf);
			de.AddFigure(new TextFigure(new DPoint(150, 30), "hello", GraphicsHelper.TextExtent, 0));
            // resize window			
			Resize(400, 300);
		}
		
		void DebugMessage(string msg)
		{
			l.Text = msg;
		}
		
		protected override bool OnDeleteEvent (Gdk.Event evnt)
		{
			Application.Quit();
			return true;
		}

	}
}
