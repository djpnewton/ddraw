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
    public partial class CustomPageSizeForm : Form
    {
        public DPoint PageSize
        {
            get { return PageTools.SizeMMtoSize(new DPoint((double)nudWidthMM.Value, (double)nudHeightMM.Value)); }
            set
            {
                DPoint pgSzMM = PageTools.SizetoSizeMM(value);
                nudWidthMM.Value = (decimal)pgSzMM.X;
                nudHeightMM.Value = (decimal)pgSzMM.Y;
            }
        }

        public PageFormat PageFormat
        {
            get
            {
                if (rbA4.Checked)
                    return PageFormat.A4;
                else if (rbA5.Checked)
                    return PageFormat.A5;
                else if (rbLetter.Checked)
                    return PageFormat.Letter;
                else return
                    PageFormat.Custom;
            }
            set { UpdatePageFormat(value); }
        }

        public CustomPageSizeForm()
        {
            InitializeComponent();
        }

        void UpdatePageFormat(PageFormat value)
        {
            rbA4.Checked = value == PageFormat.A4;
            rbA5.Checked = value == PageFormat.A5;
            rbLetter.Checked = value == PageFormat.Letter;
            rbCustom.Checked = value == PageFormat.Custom;
            nudWidthMM.Enabled = rbCustom.Checked;
            nudHeightMM.Enabled = rbCustom.Checked;
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePageFormat(PageFormat);
        }
    }
}