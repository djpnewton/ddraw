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
    public partial class TransparentForm : Form
    {
        public TransparentForm()
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
            DTkViewer dv = new WFViewer(wfViewerControl1);
            dv.Preview = true;
            dv.EditFigures = true;
            DEngine de = new DEngine(new DAuthorProperties(), false);
            de.AddViewer(dv);
            // setup undo/redo sensitive stuff
            de.UndoRedoStart("initial setup");
            de.PageSize = new DPoint(screenSize.Width, screenSize.Height);
            de.AddFigure(new RectFigure(new DRect(screenSize.Width / 2 - 50, screenSize.Height / 2 - 50, 100, 100), Math.PI / 4)); // rect figure
            de.AddFigure(new TextFigure(new DPoint(screenSize.Width / 2 - 50, screenSize.Height / 4), "Drag ME", 0)); // text figure
            BackgroundFigure bf = new BackgroundFigure(); // background figure
            bf.ImageData = WFHelper.ToImageData(bmp);
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