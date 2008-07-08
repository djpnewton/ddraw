using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;
using Workbook.PersonalToolbar;

namespace Workbook
{
    public delegate void ImportAnnotationsPageHandler(DEngine de);
    public delegate void ImportAnnotationsImageHandler(DBitmap bmp);

    public partial class AnnoToolsForm : WorkBookForm
    {
        Form prevOwner;
        public Form MainForm;

        public bool Alone;

        AnnotationForm annotationForm = null;

        bool haveImportedAnnotations = false;

        public event ImportAnnotationsPageHandler ImportAnnotationsPage;
        public event ImportAnnotationsImageHandler ImportAnnotationsArea;

        // singleton thingie
        static AnnoToolsForm annoTools = null;
        public static AnnoToolsForm GlobalAtf
        {
            get
            {
                if (annoTools == null)
                    annoTools = new AnnoToolsForm();
                return annoTools;
            }
        }

        public AnnoToolsForm()
        {
            InitializeComponent();
            MouseMode();
            Disposed += new EventHandler(FloatingToolsForm_Disposed);
        }

        void  FloatingToolsForm_Disposed(object sender, EventArgs e)
        {
            // remove reference as the form is disposed
 	        annoTools = null;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            MouseMode();
        }

        bool MouseMode()
        {
            if (annotationForm != null)
            {
                // ask if user wants to cancel
                if (!haveImportedAnnotations && annotationForm.De.CanUndo)
                {
                    if (MessageBox.Show("You have not imported any annotations. This action will erase your annotations, are you sure you want to continue?",
                        "No Annotations Imported", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return false;
                }
                haveImportedAnnotations = false;
                // set ownwer back to the mainform
                Owner = prevOwner;
                // close the annotation form
                annotationForm.Close();
                annotationForm = null;
            }
            // hide annotation tools
            tsAnnotate.Visible = false;
            // hide state tools
            tsEngineState.Mode = FigureToolStripMode.FigureClassSelect;
            tsPropState.Visible = false;
            // personal toolbar
            tsPersonal.Visible = false;
            if (MainForm != null)
            {
                System.Diagnostics.Debug.Assert(MainForm is MainForm, "ERROR: mainForm is not of type MainForm");
                PtUtils.LoadPersonalToolsFromSource(((MainForm)MainForm).PersonalToolStrip,
                    PtUtils.CreatePersonalToolsSource(tsPersonal));
            }
            return true;
        }

        void ScreenAnnotateMode()
        {
            if (annotationForm == null)
            {
                // save owner reference
                prevOwner = Owner;
                // hide this toolbar and the mainform
                Hide();
                Application.DoEvents();
                System.Threading.Thread.Sleep(250);
                // create the annotation form (this takes a screen grab)
                annotationForm = new AnnotationForm();
                annotationForm.Dap = tsEngineState.Dap;
                annotationForm.Show();
                // set the owner to the annotation form
                Owner = annotationForm;
                // show this toolbar
                Show();
                // show annotation tools
                tsAnnotate.Visible = true;
                // show DEngine state tools
                tsEngineState.Mode = FigureToolStripMode.DEngineState;
                tsEngineState.De = annotationForm.De;
                tsPropState.Visible = true;
                tsPropState.Dap = tsEngineState.Dap;
                tsPropState.De = annotationForm.De;
                tsPropState.Dv = annotationForm.Dv;
                // personal toolbar
                tsPersonal.Visible = true;
                System.Diagnostics.Debug.Assert(MainForm is MainForm, "ERROR: mainForm is not of type MainForm");
                PtUtils.LoadPersonalToolsFromSource(tsPersonal,
                    PtUtils.CreatePersonalToolsSource(((MainForm)MainForm).PersonalToolStrip));
                tsPersonal.De = annotationForm.De;
            }
        }

        private void AnnoToolsForm_Shown(object sender, EventArgs e)
        {
            if (MainForm != null)
                MainForm.Hide();
        }

        private void AnnoToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if the annotation form is shown then go back to mousing
            if (annotationForm != null)
                if (!MouseMode())
                {
                    e.Cancel = true;
                    return;
                }
            // if the main form is not visible then close it or show it
            if (MainForm != null && !MainForm.Visible)
            {
                if (Alone)
                    MainForm.Close();
                else
                    MainForm.Show();
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
            if (ImportAnnotationsArea != null && rect.Width > 0 && rect.Height > 0)
            {
                // set haveImportedAnnotations
                haveImportedAnnotations = true;
                // call the ImportAnnotationsArea event passing it the cropped bitmap
                ImportAnnotationsArea(annotationForm.CaptureImage(rect));
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

        private void tsEngineState_FigureClassChanged(object sender, Type figureClass)
        {
            if (figureClass != null)
            {
                ScreenAnnotateMode();
                annotationForm.De.HsmSetStateByFigureClass(figureClass);
            }
        }

        private void tsEngineState_DapChanged(object sender, DAuthorProperties dap)
        {
            tsPropState.Dap = dap;
            tsPersonal.Dap = dap;
            annotationForm.Dap = dap;
        }

        private void tsEngineState_AddToPersonalTools(object sender, CustomFigureTool customFigure)
        {
            tsPersonal.AddCustomFigure(customFigure);
        }

        private void tsPersonal_ItemContext(object sender, EventArgs e)
        {
            annotationForm.De.CheckState();
        }
    }
}