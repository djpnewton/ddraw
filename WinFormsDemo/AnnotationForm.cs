using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public partial class AnnotationForm : Form
    {
        DEngine de;
        public DEngine De
        {
            get { return de; }
        }

        DTkViewer dv;
        public DTkViewer Dv
        {
            get { return dv; }
        }

        public AnnotationForm()
        {
            InitializeComponent();
            // screen size
            Size screenSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            // get screen cap
            Bitmap bmp = new Bitmap(screenSize.Width, screenSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, screenSize);
            g.Dispose();
            // create dv & de
            dv = new WFViewer(wfViewerControl1);
            dv.Preview = true;
            dv.EditFigures = true;
            dv.AntiAlias = true;
            DAuthorProperties ap = new DAuthorProperties();
            de = new DEngine(ap, false);
            de.AddViewer(dv);
            // setup author props
            ap.StrokeWidth = 2;
            // setup undo/redo sensitive stuff
            de.UndoRedoStart("initial setup");
            de.PageSize = new DPoint(screenSize.Width + 1, screenSize.Height + 1);
            de.AddFigure(new RectFigure(new DRect(screenSize.Width / 2 - 50, screenSize.Height / 2 - 50, 100, 100), Math.PI / 4)); // rect figure
            de.AddFigure(new TextFigure(new DPoint(screenSize.Width / 2 - 50, screenSize.Height / 4), "Drag ME", 0)); // text figure
            BackgroundFigure bf = new BackgroundFigure(); // background figure
            bf.ImageData = WFHelper.ToImageData(bmp);
            bf.FileName = "screen_capture.png";
            bf.Position = DImagePosition.Normal;
            de.SetBackgroundFigure(bf);
            de.UndoRedoCommit();
            de.UndoRedoClearHistory();
            // set form to full screen
            Location = new Point(0, 0);
            Size = screenSize;
        }
    }
}