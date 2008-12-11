using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Workbook
{
    public partial class GridForm : Form
    {
        public int GridSize
        {
            get { return (int)nudSize.Value; }
            set { nudSize.Value = value; }
        }

        public bool ShowGrid
        {
            get { return cbShow.Checked; }
            set { cbShow.Checked = value; }
        }

        public bool SnapPosition
        {
            get { return cbSnapPosition.Checked; }
            set { cbSnapPosition.Checked = value; }
        }

        public bool SnapResize
        {
            get { return cbSnapResize.Checked; }
            set { cbSnapResize.Checked = value; }
        }

        public bool SnapLines
        {
            get { return cbSnapLines.Checked; }
            set { cbSnapLines.Checked = value; }
        }

        public GridForm()
        {
            InitializeComponent();
            LocalizeUI();
        }

        private void LocalizeUI()
        {
            btnOk.Text = WbLocale.Ok;
            btnCancel.Text = WbLocale.Cancel;
            lbGridSize.Text = WbLocale.GridSize;
            cbShow.Text = WbLocale.ShowGrid;
            cbSnapPosition.Text = WbLocale.SnapPosition;
            cbSnapResize.Text = WbLocale.SnapResize;
            cbSnapLines.Text = WbLocale.SnapLines;
        }
    }
}