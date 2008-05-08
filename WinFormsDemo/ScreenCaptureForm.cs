using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;

namespace WinFormsDemo
{
    public partial class ScreenCaptureForm : Form
    {
        AnnotationForm annotationForm = null;

        public event ImportAnnotationsImageHandler CaptureImage;

        public ScreenCaptureForm()
        {
            InitializeComponent();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            // hide this form
            Hide();
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);
            // create the annotation form (this takes a screen grab)
            annotationForm = new AnnotationForm();
            annotationForm.Show();
            annotationForm.KeyPreview = true;
            annotationForm.KeyDown += new KeyEventHandler(annotationForm_KeyDown);
            // set annotation DEngine state to SelectMeasure
            annotationForm.De.HsmState = DHsmState.SelectMeasure;
            annotationForm.De.HsmStateChanged += new HsmStateChangedHandler(De_HsmStateChanged);
            annotationForm.De.MeasureRect += new SelectMeasureHandler(De_MeasureRect);
        }

        void annotationForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CloseAnnotationForm();
        }

        void De_HsmStateChanged(DEngine de, DHsmState state)
        {
            de.HsmStateChanged -= De_HsmStateChanged;
            CloseAnnotationForm();
        }

        void De_MeasureRect(DEngine de, DRect rect)
        {
            de.MeasureRect -= De_MeasureRect;
            if (CaptureImage != null && rect.Width > 0 && rect.Height > 0)
                // call the CaptureImage event passing it the captured bitmap
                CaptureImage(annotationForm.CaptureImage(rect));
            // close the annotation form
            CloseAnnotationForm();
        }

        void CloseAnnotationForm()
        {
            Show();
            if (annotationForm != null)
            {
                annotationForm.Close();
                annotationForm = null;
            }
        }
    }
}
