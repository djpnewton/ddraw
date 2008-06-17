using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

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
                    tsCustomFigureProps.Dap = ((CustomFigureT)value).Dap;
                    tsCustomFigureType.FigureClass = ((CustomFigureT)value).FigureClass;
                    tsCustomFigureProps.FigureClass = ((CustomFigureT)value).FigureClass;
                    ((CustomFigureT)value).Dap.PropertyChanged += new AuthorPropertyChanged(Dap_PropertyChanged);
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
                else if (value is WebLinkT)
                {
                    tbUrl.Text = ((WebLinkT)value).Link;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.WebLink;
                }
            }
            get 
            {
                if (cbType.SelectedIndex == (int)PersonalToolButtonType.CustomFigure)
                {
                    DBitmap bmp = WFHelper.MakeBitmap(vcCustomFigure.Width, vcCustomFigure.Height);
                    de.Copy(de.Figures, out bmp, true, DColor.Clear);
                    return new CustomFigureT(tsCustomFigureProps.FigureClass, tsCustomFigureProps.Dap,
                        Convert.ToBase64String(WFHelper.ToImageData(bmp)));
                }
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.RunCmd)
                    return new RunCmdT(tbRun.Text, tbArgs.Text);
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.ShowDir)
                    return new ShowDirT(tbDir.Text);
                else
                    return new WebLinkT(tbUrl.Text);

            }
        }

        public bool ToolEdit
        {
            set
            {
                if (value)
                {
                    Text = "Tool Edit";
                    tsCustomFigureType.Visible = false;
                    cbType.Visible = false;
                    cbToolEditAddToPersonal.Visible = true;
                }
                else
                {
                    Text = "Personal Toolbutton";
                    cbType.Visible = true;
                    cbToolEditAddToPersonal.Visible = false;
                    tsCustomFigureType.Visible = true;
                }
            }
        }

        public bool ToolEditAddToPersonal
        {
            get { return cbToolEditAddToPersonal.Checked; }
        }

        DEngine de;
        DTkViewer dv;

        public PtButtonForm()
        {
            InitializeComponent();
            dv = new WFViewer(vcCustomFigure);
            dv.EditFigures = false;
            dv.AntiAlias = true;
            dv.Preview = true;
            de = new DEngine(false);
            de.AddViewer(dv);
            // set page height to viewer size
            de.UndoRedoStart("blah");
            de.PageSize = new DPoint(vcCustomFigure.Width, vcCustomFigure.Height);
            de.UndoRedoCommit();
            // default to non tooledit
            ToolEdit = false;
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
                case (int)PersonalToolButtonType.WebLink:
                    pnlWebLink.BringToFront();
                    break;
                default:
                    if (tsCustomFigureProps.Dap == null)
                    {
                        tsCustomFigureType.FigureClass = typeof(PolylineFigure);
                        tsCustomFigureProps.Dap = tsCustomFigureType.Dap.Clone();
                        tsCustomFigureProps.FigureClass = typeof(PolylineFigure);
                        tsCustomFigureProps.Dap.PropertyChanged += new AuthorPropertyChanged(Dap_PropertyChanged);
                    }
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
            // add figure de so it shows on the viewer
            WorkBookUtils.PreviewFigure(de, dv, figureClass, tsCustomFigureProps.Dap, 
                new DPoint(vcCustomFigure.Width, vcCustomFigure.Height));
        }

        void Dap_PropertyChanged(DAuthorProperties dap)
        {
            WorkBookUtils.PreviewFigure(de, dv, tsCustomFigureProps.FigureClass, dap,
                new DPoint(vcCustomFigure.Width, vcCustomFigure.Height));
        }
    }
}