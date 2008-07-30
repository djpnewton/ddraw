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
    public partial class CustomPageSizeForm : Form
    {
        public DPoint PageSize
        {
            get 
            {
                if (PageFormat == PageFormat.Default || PageFormat == PageFormat.A4 ||
                    PageFormat == PageFormat.A5 || PageFormat == PageFormat.Letter)
                    return PageTools.FormatToSize(PageFormat);
                else
                    return PageTools.SizeMMtoSize(new DPoint((double)nudWidthMM.Value, (double)nudHeightMM.Value)); 
            }
            set
            {
                DPoint pgSzMM = PageTools.SizetoSizeMM(value);
                nudWidthMM.Value = (decimal)pgSzMM.X;
                nudHeightMM.Value = (decimal)pgSzMM.Y;
                PageFormat = PageTools.SizeMMToFormat(pgSzMM);
            }
        }

        public PageFormat PageFormat
        {
            get
            {
                if (rbDefault.Checked)
                    return PageFormat.Default;
                else if (rbA4.Checked)
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

        public bool ApplyAll
        {
            get { return cbApplyAll.Checked; }
        }

        public CustomPageSizeForm()
        {
            InitializeComponent();
            LocalizeUI();
        }

        private void LocalizeUI()
        {
            Text = WbLocale.PageSize;
            rbDefault.Text = WbLocale.Default;
            rbCustom.Text = WbLocale.Custom;
            lbWidth.Text = WbLocale.WidthMM;
            lbHeight.Text = WbLocale.HeightMM;
            cbApplyAll.Text = WbLocale.ApplyToAllPages;
            btnOk.Text = WbLocale.Ok;
            btnCancel.Text = WbLocale.Cancel;
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