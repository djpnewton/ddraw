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
            de = new DEngine(DAuthorProperties.GlobalAP, false);
            de.AddViewer(dv);
            WorkBookUtils.SetupDEngine(de);
            // setup undo/redo sensitive stuff
            de.UndoRedoStart("initial setup");
            de.PageSize = new DPoint(screenSize.Width, screenSize.Height);
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

        private void wfViewerControl1_KeyDown(object sender, KeyEventArgs e)
        {
            WorkBookUtils.ViewerKeyDown(de, e);
            // delete
            if (e.KeyCode == Keys.Delete && !e.Control && !e.Alt && !e.Shift)
                de.Delete(de.SelectedFigures);
        }

        private void wfViewerControl1_KeyUp(object sender, KeyEventArgs e)
        {
            WorkBookUtils.ViewerKeyUp(de, e);
        }
    }
}