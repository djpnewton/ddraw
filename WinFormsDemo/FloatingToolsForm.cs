using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;
using WinFormsDemo.PersonalToolbar;

namespace WinFormsDemo
{
    public delegate void ImportAnnotationsPageHandler(DEngine de);
    public delegate void ImportAnnotationsImageHandler(DBitmap bmp);

    public partial class FloatingToolsForm : WorkBookForm
    {
        Form mainForm;

        AnnotationForm annotationForm = null;

        bool haveImportedAnnotations = false;
        public bool Alone
        {
            get { return TopMost; }
            set { TopMost = value; }
        }

        public event ImportAnnotationsPageHandler ImportAnnotationsPage;
        public event ImportAnnotationsImageHandler ImportAnnotationsArea;

        // singleton thingie
        static FloatingToolsForm floatingTools = null;
        public static FloatingToolsForm GlobalFT
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
            MouseMode();
            Disposed += new EventHandler(FloatingToolsForm_Disposed);
        }

        void  FloatingToolsForm_Disposed(object sender, EventArgs e)
        {
            // remove reference as the form is disposed
 	        floatingTools = null;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            MouseMode();
        }

        private void btnScreenAnnotate_Click(object sender, EventArgs e)
        {
            ScreenAnnotateMode();
        }

        bool MouseMode()
        {
            if (annotationForm != null)
            {
                // ask if user wants to cancel
                if (!haveImportedAnnotations && annotationForm.De.CanUndo)
                {
                    if (MessageBox.Show("You have not imported any annotations. Going into mouse mode will erase your annotations, do you want to cancel this action?",
                        "Annotations Import Question", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        return false;
                }
                // set ownwer back to the mainform
                Owner = mainForm;
                // close the annotation form
                annotationForm.Close();
                annotationForm = null;
                // show the mainform
                if (!Alone || haveImportedAnnotations)
                {
                    mainForm.Show();
                    haveImportedAnnotations = false;
                }
            }
            // check mouse button
            btnMouse.Checked = true;
            btnScreenAnnotate.Checked = false;
            // hide annotation tools
            toolStripSeparator1.Visible = false;
            btnUndo.Visible = false;
            btnImportArea.Visible = false;
            btnImportPage.Visible = false;
            // hide state tools
            tsEngineState.Visible = false;
            tsPropState.Visible = false;
            // personal toolbar
            tsPersonal.Visible = false;
            if (mainForm != null)
            {
                System.Diagnostics.Debug.Assert(mainForm is MainForm, "ERROR: mainForm is not of type MainForm");
                PtUtils.LoadPersonalToolsFromSource(((MainForm)mainForm).PersonalToolStrip,
                    PtUtils.CreatePersonalToolsSource(tsPersonal));
            }
            return true;
        }

        void ScreenAnnotateMode()
        {
            if (annotationForm == null)
            {
                // save mainform/owner reference
                mainForm = Owner;
                // hide this toolbar and the mainform
                Hide();
                mainForm.Hide();
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
                btnUndo.Visible = true;
                toolStripSeparator1.Visible = true;
                btnImportArea.Visible = true;
                btnImportPage.Visible = true;
                // show DEngine state tools
                tsEngineState.Visible = true;
                tsEngineState.De = annotationForm.De;
                tsPropState.Visible = true;
                tsPropState.Dap = DAuthorProperties.GlobalAP;
                tsPropState.De = annotationForm.De;
                tsPropState.Dv = annotationForm.Dv;
                // personal toolbar
                tsPersonal.Visible = true;
                System.Diagnostics.Debug.Assert(mainForm is MainForm, "ERROR: mainForm is not of type MainForm");
                PtUtils.LoadPersonalToolsFromSource(tsPersonal,
                    PtUtils.CreatePersonalToolsSource(((MainForm)mainForm).PersonalToolStrip));
                tsPersonal.De = annotationForm.De;
                tsPersonal.Dap = DAuthorProperties.GlobalAP;
            }
        }

        private void FloatingToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if the annotation form is shown then go back to mousing
            if (annotationForm != null)
                if (!MouseMode())
                {
                    e.Cancel = true;
                    return;
                }
            // if the main form is not visible then close it too
            if (Owner != null && !Owner.Visible)
            {
                mainForm = Owner;
                Owner = null; // watch out for recursion
                mainForm.Close();
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            annotationForm.De.Undo();
        }

        private void btnImportArea_Click(object sender, EventArgs e)
        {
            // set annotation DEngine state to SelectMeasure
            if (annotationForm.De.HsmState != DHsmState.SelectMeasure)
            {
                annotationForm.De.HsmState = DHsmState.SelectMeasure;
                btnImportArea.Checked = true;
                annotationForm.De.HsmStateChanged += new HsmStateChangedHandler(De_HsmStateChanged);
                annotationForm.De.MeasureRect += new SelectMeasureHandler(De_MeasureRect);
            }
        }

        void De_HsmStateChanged(DEngine de, DHsmState state)
        {
            de.HsmStateChanged -= De_HsmStateChanged;
            btnImportArea.Checked = false;
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
                DBitmap initialBmp = FigureSerialize.FormatToBmp(figs, annotationForm.Dv.AntiAlias, DColor.White);
                // crop the bitmap to the rect
                Bitmap croppedBmp = new Bitmap((int)rect.Width, (int)rect.Height);
                Graphics g = Graphics.FromImage(croppedBmp);
                g.DrawImage((Bitmap)initialBmp.NativeBmp, (int)-rect.X, (int)-rect.Y);
                g.Dispose();
                initialBmp.Dispose();
                // set haveImportedAnnotations
                haveImportedAnnotations = true;
                // call the ImportAnnotationsArea event passing it the cropped bitmap
                ImportAnnotationsArea(new WFBitmap(croppedBmp));
            }
            // select the selection tool
            annotationForm.De.HsmState = DHsmState.Select;
        }

        private void btnImportPage_Click(object sender, EventArgs e)
        {
            if (ImportAnnotationsPage != null)
            {
                haveImportedAnnotations = true;
                ImportAnnotationsPage(annotationForm.De);
            }
        }
    }
}