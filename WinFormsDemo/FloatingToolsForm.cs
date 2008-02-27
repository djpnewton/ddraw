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
    public delegate void ImportAnnotationsHandler(DEngine de);

    public partial class FloatingToolsForm : Form
    {
        Form mainForm;
        AnnotationForm annotationForm = null;

        public event ImportAnnotationsHandler ImportAnnotations;

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
                // ask user if they want to import the annotations
                switch (MessageBox.Show("Would you like to import the annotations?",
                    "Import Annotations", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        if (ImportAnnotations != null)
                            ImportAnnotations(annotationForm.De);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                }
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
        }

        private void btnPolyline_Click(object sender, EventArgs e)
        {
            // set annotation DEngine state to DrawLine[Polyline]
            annotationForm.De.HsmSetStateByFigureClass(typeof(PolylineFigure));
            btnSelect.Checked = false;
            btnPolyline.Checked = true;
        }
    }
}