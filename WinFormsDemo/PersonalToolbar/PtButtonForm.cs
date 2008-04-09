using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;

namespace WinFormsDemo.PersonalToolbar
{
    public partial class PtButtonForm : Form
    {
        public object ToolButtonData
        {
            set
            {
                if (value is CustomFigureT)
                {
                    tsCustomFigureType.FigureClass = ((CustomFigureT)value).FigureClass;
                    tsCustomFigureProps.FigureClass = ((CustomFigureT)value).FigureClass;
                    tsCustomFigureProps.Dap = ((CustomFigureT)value).Dap;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.CustomFigure;
                }
                else if (value is RunCmdT)
                {
                    tbRun.Text = ((RunCmdT)value).Command;
                    tbArgs.Text = ((RunCmdT)value).Arguments;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.RunCmd;
                }
                else if (value is ShowDirT)
                {
                    tbDir.Text = ((ShowDirT)value).Dir;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.ShowDir;
                }
            }
            get 
            {
                if (cbType.SelectedIndex == (int)PersonalToolButtonType.CustomFigure)
                    return new CustomFigureT(tsCustomFigureProps.FigureClass, tsCustomFigureProps.Dap);
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.RunCmd)
                    return new RunCmdT(tbRun.Text, tbArgs.Text);
                else
                    return new ShowDirT(tbDir.Text);
            }
        }

        public PtButtonForm()
        {
            InitializeComponent();
            tsCustomFigureProps.Dap = DAuthorProperties.GlobalAP.Clone();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbType.SelectedIndex)
            {
                case (int)PersonalToolButtonType.RunCmd:
                    pnlRunCmd.BringToFront();
                    break;
                case (int)PersonalToolButtonType.ShowDir:
                    pnlShowDir.BringToFront();
                    break;
                default:
                    pnlCustomFigure.BringToFront();
                    break;
            }
        }

        private void btnDirBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                tbDir.Text = fd.SelectedPath;
        }

        private void btnRunBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "All Files|*.*";
            if (fd.ShowDialog() == DialogResult.OK)
                tbRun.Text = fd.FileName;
        }

        private void tsCustomFigureType_FigureClassChanged(object sender, Type figureClass)
        {
            tsCustomFigureProps.FigureClass = figureClass;
        }
    }
}