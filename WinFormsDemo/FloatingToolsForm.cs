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
    public partial class FloatingToolsForm : Form
    {
        Form mainForm;
        TransparentForm annotationForm = null;

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
 	        floatingTools = null;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            if (annotationForm != null)
            {
                annotationForm.Close();
                annotationForm = null;

                Owner = mainForm;
                Owner.Show();
            }

            btnMouse.Checked = true;
            btnScreenAnnotate.Checked = false;
            toolStripSeparator1.Visible = false;
            btnSelect.Visible = false;
            btnPolyline.Visible = false;

            btnSelect.Checked = false;
            btnPolyline.Checked = false;
        }

        private void btnScreenAnnotate_Click(object sender, EventArgs e)
        {
            if (annotationForm == null)
            {
                mainForm = Owner;

                Hide();
                Owner.Hide();
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);

                annotationForm = new TransparentForm();
                annotationForm.Show();
                Owner = annotationForm;

                Show();


                btnMouse.Checked = false;
                btnScreenAnnotate.Checked = true;
                toolStripSeparator1.Visible = true;
                btnSelect.Visible = true;
                btnPolyline.Visible = true;

                btnSelect.Checked = true;
                btnPolyline.Checked = false;
            }
        }

        private void FloatingToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (annotationForm != null)
            {
                Owner = null;
                btnMouse.PerformClick();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            annotationForm.De.HsmState = DHsmState.Select;
            btnSelect.Checked = true;
            btnPolyline.Checked = false;
        }

        private void btnPolyline_Click(object sender, EventArgs e)
        {
            annotationForm.De.HsmSetStateByFigureClass(typeof(PolylineFigure));
            btnSelect.Checked = false;
            btnPolyline.Checked = true;
        }
    }
}