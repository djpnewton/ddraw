using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DDraw
{
    public class WFViewerControl : Panel
    {
        public WFViewerControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
