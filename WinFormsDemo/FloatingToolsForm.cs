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
    public delegate void ImportAnnotationsPageHandler(DEngine de);
    public delegate void ImportAnnotationsImageHandler(DBitmap bmp);

    public partial class FloatingToolsForm : Form
    {
        Form mainForm;
        AnnotationForm annotationForm = null;

        public event ImportAnnotationsPageHandler ImportAnnotationsPage;
        public event ImportAnnotationsImageHandler ImportAnnotationsArea;

        // singleton thingie
        static FloatingToolsForm floatingTools = null;
        public static FloatingToolsForm FloatingTools
        {
            get
            {
                if (floatingTools == null)
                    floatingTools = new FloatingToolsForm();
                return floatingTools;
            }
        }

        public FloatingToolsForm()
        {
            InitializeComponent();
            btnMouse.PerformClick();
            Disposed += new EventHandler(FloatingToolsForm_Disposed);
        }

        void  FloatingToolsForm_Disposed(object sender, EventArgs e)
        {
            // remove reference as the form is disposed
 	        floatingTools = null;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            if (annotationForm != null)
            {
                // set ownwer back to the mainform
                Owner = mainForm;
                // close the annotation form
                annotationForm.Close();
                annotationForm = null;
                // show the mainform
                Owner.Show();
            }
            // check mouse button
            btnMouse.Checked = true;
            btnScreenAnnotate.Checked = false;
            // hide annotation tools
            toolStripSeparator1.Visible = false;
            btnSelect.Visible = false;
            btnPolyline.Visible = false;
            btnUndo.Visible = false;
            toolStripSeparator2.Visible = false;
            btnImportArea.Visible = false;
            btnImportPage.Visible = false;
            // uncheck annotation tools
            btnSelect.Checked = false;
            btnPolyline.Checked = false;
        }

        private void btnScreenAnnotate_Click(object sender, EventArgs e)
        {
            if (annotationForm == null)
            {
                // save mainform/owner reference
                mainForm = Owner;
                // hide this toolbar and the mainform
                Hide();
                Owner.Hide();
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
                // create the annotation form (this takes a screen grab)
                annotationForm = new AnnotationForm();
                annotationForm.Show();
                // set the owner to the annotation form
                Owner = annotationForm;
                // show this toolbar
                Show();
                // check the screen annotation button
                btnMouse.Checked = false;
                btnScreenAnnotate.Checked = true;
                // show annotation tools
                toolStripSeparator1.Visible = true;
                btnSelect.Visible = true;
                btnPolyline.Visible = true;
                btnUndo.Visible = true;
                toolStripSeparator2.Visible = true;
                btnImportArea.Visible = true;
                btnImportPage.Visible = true;
                // check the selection tool
                btnSelect.Checked = true;
                btnPolyline.Checked = false;
            }
        }

        private void FloatingToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if the annotation form is shown then go back to mousing
            if (annotationForm != null)
            {
                Owner = null;
                btnMouse.PerformClick();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            // set annotation DEngine state to select
            annotationForm.De.HsmState = DHsmState.Select;
            btnSelect.Checked = true;
            btnPolyline.Checked = false;
            btnImportArea.Checked = false;
        }

        private void btnPolyline_Click(object sender, EventArgs e)
        {
            // set annotation DEngine state to DrawLine[Polyline]
            annotationForm.De.HsmSetStateByFigureClass(typeof(PolylineFigure));
            btnSelect.Checked = false;
            btnPolyline.Checked = true;
            btnImportArea.Checked = false;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            annotationForm.De.Undo();
        }

        private void btnImportArea_Click(object sender, EventArgs e)
        {
            // set annotation DEngine state to SelectMeasure
            annotationForm.De.HsmState = DHsmState.SelectMeasure;
            btnSelect.Checked = false;
            btnPolyline.Checked = false;
            btnImportArea.Checked = true;
            annotationForm.De.MeasureRect += new SelectMeasureHandler(De_MeasureRect);
        }

        void De_MeasureRect(DEngine de, DRect rect)
        {
            de.MeasureRect -= De_MeasureRect;
            if (ImportAnnotationsArea != null)
            {
                //  create list of figures to format to bitmap
                List<Figure> figs = new List<Figure>();
                figs.Add(annotationForm.De.GetBackgroundFigure());
                foreach (Figure f in annotationForm.De.Figures)
                    figs.Add(f);
                // format the figures to bitmap
                DBitmap initialBmp = FigureSerialize.FormatToBmp(figs, annotationForm.Dv.AntiAlias);
                // crop the bitmap to the rect
                Bitmap croppedBmp = new Bitmap((int)rect.Width, (int)rect.Height);
                Graphics g = Graphics.FromImage(croppedBmp);
                g.DrawImage((Bitmap)initialBmp.NativeBmp, (int)-rect.X, (int)-rect.Y);
                g.Dispose();
                initialBmp.Dispose();
                // call the ImportAnnotationsArea event passing it the cropped bitmap
                ImportAnnotationsArea(new WFBitmap(croppedBmp));
            }
            // select the selection tool
            btnSelect.PerformClick();
        }

        private void btnImportPage_Click(object sender, EventArgs e)
        {
            if (ImportAnnotationsPage != null)
                ImportAnnotationsPage(annotationForm.De);
        }
    }
}