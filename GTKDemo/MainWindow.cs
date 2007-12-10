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

        DEngine de;
        DViewer dv;
		
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
			dv = new GTKViewer(dvc);
			dv.EditFigures = true;
			dv.DebugMessage += new DebugMessageHandler(DebugMessage);
			de = new DEngine(new DAuthorProperties());
			de.AddViewer(dv);
			de.State = DEngineState.Select;
			de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.ContextClick += new ContextClickHandler(de_ContextClick);
			// add figures
			de.AddFigure(new EllipseFigure(new DRect(20, 30, 100, 100), 0));
			RectFigure rf = new RectFigure(new DRect(10, 20, 100, 100), 0);
			rf.Alpha = 0.7;
			rf.Fill = new DColor(80, 80, 80);
			de.AddFigure(rf);
			de.AddFigure(new TextFigure(new DPoint(150, 30), "hello", GraphicsHelper.TextExtent, 0));
            // compositing figure
            Figure f = new CompositedExampleFigure();
            f.Rect = new DRect(20, 150, 50, 50);
            de.AddFigure(f);
            // clock (IEditable) figure
            f = new ClockFigure();
            f.Rect = new DRect(200, 200, 100, 100);
            de.AddFigure(f);
            // resize window			
			Resize(400, 300);
		}

        void de_ContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            // create popup
            Menu pop = new Menu();
            MenuItem mi;
            if (clickedFigure == null)
            {
                // page zoom menu items
                mi = new MenuItem("Fit to Page");
                mi.ButtonPressEvent += new ButtonPressEventHandler(miFitToPage_ButtonPressEvent);
                pop.Append(mi);
                mi = new MenuItem("Fit to Width");
                mi.ButtonPressEvent += new ButtonPressEventHandler(miFitToWidth_ButtonPressEvent);
                pop.Append(mi);
                mi = new MenuItem("50%");
                mi.ButtonPressEvent += new ButtonPressEventHandler(mi050pc_ButtonPressEvent);
                pop.Append(mi);
                mi = new MenuItem("100%");
                mi.ButtonPressEvent += new ButtonPressEventHandler(mi100pc_ButtonPressEvent);
                pop.Append(mi);
                mi = new MenuItem("150%");
                mi.ButtonPressEvent += new ButtonPressEventHandler(mi150pc_ButtonPressEvent);
                pop.Append(mi);
            }
            else
            {
                // group menu items
                Figure[] figs = de.SelectedFigures;
                if (figs.Length > 1)
                {
                    mi = new MenuItem("Group");
                    mi.ButtonPressEvent += new ButtonPressEventHandler(miGroup_ButtonPressEvent);
                    pop.Append(mi);
                }
                else if (figs.Length == 1 && figs[0] is GroupFigure)
                {
                    mi = new MenuItem("Ungroup");
                    mi.ButtonPressEvent += new ButtonPressEventHandler(miUngroup_ButtonPressEvent);
                    pop.Append(mi);
                }
            }
            // show popup
            if (pop.Children.Length > 0)
            {
                pop.Popup();
                pop.ShowAll();
            }
        }
                
        void miFitToPage_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            dv.Zoom = Zoom.FitToPage;
        }
		
        void miFitToWidth_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            dv.Zoom = Zoom.FitToWidth;
        }
		
        void mi050pc_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            dv.Scale = 0.5;
        }
		
        void mi100pc_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            dv.Scale = 1.0;
        }
		
        void mi150pc_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            dv.Scale = 1.5;
        }
		
        void miGroup_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            de.GroupFigures(de.SelectedFigures);
        }
            
        void miUngroup_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            de.UngroupFigure((GroupFigure)de.SelectedFigures[0]);
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
