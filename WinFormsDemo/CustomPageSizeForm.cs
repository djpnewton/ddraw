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

        public CustomPageSizeForm()
        {
            InitializeComponent();
        }
    }
}