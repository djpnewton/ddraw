using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;

namespace Workbook
{
    public partial class DimensionsForm : Form
    {
        public DRect BoundingRect(IList<Figure> figs)
        {
            DRect r = new DRect(0, 0, 0, 0);
            if (figs.Count == 1)
            {
                r = figs[0].Rect;
                foreach (Figure f in figs)
                    r = r.Union(f.Rect);
            }
            else if (figs.Count > 1)
            {
                r = DGeom.BoundingBoxOfRotatedRect(figs[0].Rect, figs[0].Rotation);
                foreach (Figure f in figs)
                    r = r.Union(DGeom.BoundingBoxOfRotatedRect(f.Rect, f.Rotation));
            }  
            return r;
        }

        IList<Figure> figs = null;
        public IList<Figure> Figures
        {
            set
            {
                figs = value; 
                // set dimensions
                if (figs.Count > 0)
                {
                    DRect r = BoundingRect(figs);
                    numX.Value = Convert.ToDecimal(r.X);
                    cbGroupX.Visible = figs.Count > 1;
                    cbGroupX.Checked = false; 
                    numY.Value = Convert.ToDecimal(r.Y);
                    cbGroupY.Visible = figs.Count > 1;
                    cbGroupY.Checked = false; 
                    numWidth.Value = Convert.ToDecimal(r.Width);
                    cbGroupWidth.Visible = figs.Count > 1;
                    cbGroupWidth.Checked = false; 
                    numHeight.Value = Convert.ToDecimal(r.Height);
                    cbGroupHeight.Visible = figs.Count > 1;
                    cbGroupHeight.Checked = false; 
                    numRotation.Value = Convert.ToDecimal(figs[0].Rotation * 180 / Math.PI);
                    cbGroupRot.Visible = figs.Count > 1;
                    cbGroupRot.Checked = false;
                    if (figs.Count == 1)
                    {
                        numWidth.Minimum = Convert.ToDecimal(figs[0].MinSize);
                        numHeight.Minimum = Convert.ToDecimal(figs[0].MinSize);
                        cbLockAspect.Checked = figs[0].LockAspectRatio;
                    }
                    else
                    {
                        numWidth.Enabled = false;
                        numHeight.Enabled = false;
                        numRotation.Enabled = false;
                    }
                    cbLockAspect.Visible = figs.Count == 1;
                }
                else
                {
                    numX.Enabled = false;
                    numY.Enabled = false;
                    numWidth.Enabled = false;
                    numHeight.Enabled = false;
                }
            }
            get { return figs; }
        }

        public double FigX
        {
            get { return Convert.ToDouble(numX.Value); }
        }
        public bool GroupX
        {
            get { return cbGroupX.Checked; }
        }
        public double FigY
        {
            get { return Convert.ToDouble(numY.Value); }
        }
        public bool GroupY
        {
            get { return cbGroupY.Checked; }
        }
        public double FigWidth
        {
            get { return Convert.ToDouble(numWidth.Value); }
        }
        public bool GroupWidth
        {
            get { return cbGroupWidth.Checked; }
        }
        public double FigHeight
        {
            get { return Convert.ToDouble(numHeight.Value); }
        }
        public bool GroupHeight
        {
            get { return cbGroupHeight.Checked; }
        }
        public double FigRotation
        {
            get { return Convert.ToDouble(numRotation.Value) * Math.PI / 180; }
        }
        public bool GroupRotation
        {
            get { return cbGroupRot.Checked; }
        }

        public DimensionsForm()
        {
            InitializeComponent();
        }

        private void cbGroupWidth_CheckedChanged(object sender, EventArgs e)
        {
            numWidth.Enabled = cbGroupWidth.Checked;
            if (numWidth.Enabled)
                numWidth.Value = Convert.ToDecimal(figs[0].Width);
            else
                numWidth.Value = Convert.ToDecimal(BoundingRect(figs).Width);
        }

        private void cbGroupHeight_CheckedChanged(object sender, EventArgs e)
        {
            numHeight.Enabled = cbGroupHeight.Checked;
            if (numHeight.Enabled)
                numHeight.Value = Convert.ToDecimal(figs[0].Height);
            else
                numHeight.Value = Convert.ToDecimal(BoundingRect(figs).Height);
        }

        private void cbGroupRot_CheckedChanged(object sender, EventArgs e)
        {
            numRotation.Enabled = cbGroupRot.Checked;
        }

        decimal aspectRatio;

        private void cbLockAspect_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLockAspect.Checked)
                aspectRatio = numWidth.Value / numHeight.Value;
        }

        private void numWidth_Validating(object sender, CancelEventArgs e)
        {
            if (cbLockAspect.Checked)
            {
                decimal v = numWidth.Value / aspectRatio;
                if (v < numHeight.Minimum)
                {
                    numWidth.Value = numHeight.Value * aspectRatio;
                    e.Cancel = true;
                }
                else
                    numHeight.Value = v;
            }
        }

        private void numHeight_Validating(object sender, CancelEventArgs e)
        {
            if (cbLockAspect.Checked)
            {
                decimal v = numHeight.Value * aspectRatio;
                if (v < numWidth.Minimum)
                {
                    numHeight.Value = numWidth.Value / aspectRatio;
                    e.Cancel = true;
                }
                else
                    numWidth.Value = v;
            }
        }
    }
}