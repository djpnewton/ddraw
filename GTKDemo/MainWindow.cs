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
			Box b = new VBox();
			Add(b);

            // Initialze DGraphics
            GTKGraphics.Init();
			
			GTKViewerControl dvc = new GTKViewerControl();
			b.Add(dvc);
			
			l = new Label("debug");
			b.PackEnd(l, false, false, 0);
			
			GTKViewer dv = new GTKViewer(dvc);
			dv.DebugMessage += new DebugMessageHandler(DebugMessage);
			DAuthorProperties dap = new DAuthorProperties();
			DEngine de = new DEngine(dap);
			de.AddViewer(dv);
			de.DebugMessage += new DebugMessageHandler(DebugMessage);
			
			de.AddFigure(new EllipseFigure(new DRect(20, 30, 100, 100), 0));
			RectFigure rf = new RectFigure(new DRect(10, 20, 100, 100), 0);
			rf.Alpha = 0.7;
			rf.Fill = new DColor(80, 80, 80);
			de.AddFigure(rf);
			de.AddFigure(new TextFigure(new DPoint(150, 30), "hello", new GTKTextExtent(), 0));

			dv.EditFigures = true;
			dap.EditMode = DEditMode.Select;
			
			
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
