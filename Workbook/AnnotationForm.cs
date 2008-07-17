using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace Workbook
{
    public partial class AnnotationForm : Form
    {
        DEngine de;
        public DEngine De
        {
            get { return de; }
        }

        DAuthorProperties dap;
        public DAuthorProperties Dap
        {
            set { dap = value; }
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
            de = new DEngine(null);
            de.AddedFigure += new AddedFigureHandler(de_AddedFigure);
            de.AddViewer(dv);
            WorkBookUtils.SetupDEngine(de);
            // setup undo/redo sensitive stuff
            de.UndoRedoStart("initial setup");
            de.PageSize = new DPoint(screenSize.Width, screenSize.Height);
            BackgroundFigure bf = new BackgroundFigure(); // background figure
            bf.ImageData = WFHelper.ToImageData(bmp);
            bf.FileName = "screen_capture.png";
            bf.Position = DImagePosition.Normal;
            de.SetBackgroundFigure(bf, true);
            de.UndoRedoCommit();
            de.UndoRedoClearHistory();
            // set form to full screen
            Location = new Point(0, 0);
            Size = screenSize;
        }

        void de_AddedFigure(DEngine de, Figure fig, bool fromHsm)
        {
            if (fromHsm)
                dap.ApplyPropertiesToFigure(fig);
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

        public DBitmap CaptureImage(DRect rect)
        {
            System.Diagnostics.Debug.Assert(rect.Width > 0, "ERROR: rect.Width <= 0");
            System.Diagnostics.Debug.Assert(rect.Height > 0, "ERROR: rect.Height <= 0");
            //  create list of figures to format to bitmap
            List<Figure> figs = new List<Figure>();
            figs.Add(de.BackgroundFigure);
            foreach (Figure f in de.Figures)
                figs.Add(f);
            // format the figures to bitmap
            DBitmap initialBmp = FigureSerialize.FormatToBmp(figs, dv.AntiAlias, DColor.White);
            // crop the bitmap to the rect
            DBitmap croppedBmp = WFHelper.MakeBitmap((int)rect.Width, (int)rect.Height);
            DGraphics dg = WFHelper.MakeGraphics(croppedBmp);
            dg.DrawBitmap(initialBmp, new DPoint(-rect.X, -rect.Y));
            dg.Dispose();
            initialBmp.Dispose();
            return croppedBmp;
        }

        private void wfViewerControl1_MouseEnter(object sender, EventArgs e)
        {
            PopupForm.HidePopups();
        }
    }
}