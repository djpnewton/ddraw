using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace Workbook.PersonalToolbar
{
    public partial class PtButtonForm : Form
    {
        public PersonalTool PersonalTool
        {
            set
            {
                tbLabel.Text = value.Label;
                cbLabel.Checked = value.ShowLabel;
                if (value is CustomFigureTool)
                {
                    tsCustomFigureProps.Dap = ((CustomFigureTool)value).Dap;
                    tsCustomFigureType.FigureClass = ((CustomFigureTool)value).FigureClass;
                    tsCustomFigureProps.FigureClass = ((CustomFigureTool)value).FigureClass;
                    ((CustomFigureTool)value).Dap.PropertyChanged += new AuthorPropertyChanged(Dap_PropertyChanged);
                    cbType.SelectedIndex = (int)PersonalToolButtonType.CustomFigure;
                }
                else if (value is RunCmdTool)
                {
                    tbRun.Text = ((RunCmdTool)value).Command;
                    tbArgs.Text = ((RunCmdTool)value).Arguments;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.RunCmd;
                }
                else if (value is ShowDirTool)
                {
                    tbDir.Text = ((ShowDirTool)value).Dir;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.ShowDir;
                }
                else if (value is WebLinkTool)
                {
                    tbUrl.Text = ((WebLinkTool)value).Link;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.WebLink;
                }
                else if (value is ModeSelectTool)
                {
                    if (((ModeSelectTool)value).ModeSelectType == ModeSelectType.Select)
                        rbSelect.Checked = true;
                    else
                        rbEraser.Checked = true;
                    cbType.SelectedIndex = (int)PersonalToolButtonType.ModeSelect;
                }
            }
            get 
            {
                if (cbType.SelectedIndex == (int)PersonalToolButtonType.CustomFigure)
                {
                    DBitmap bmp = WFHelper.MakeBitmap(vcCustomFigure.Width, vcCustomFigure.Height);
                    de.Copy(de.Figures, out bmp, true, DColor.Empty);
                    return new CustomFigureTool(tbLabel.Text, cbLabel.Checked,
                        tsCustomFigureProps.FigureClass, tsCustomFigureProps.Dap,
                        Convert.ToBase64String(WFHelper.ToImageData(bmp)));
                }
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.RunCmd)
                    return new RunCmdTool(tbLabel.Text, cbLabel.Checked, tbRun.Text, tbArgs.Text);
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.ShowDir)
                    return new ShowDirTool(tbLabel.Text, cbLabel.Checked, tbDir.Text);
                else if (cbType.SelectedIndex == (int)PersonalToolButtonType.WebLink)
                    return new WebLinkTool(tbLabel.Text, cbLabel.Checked, tbUrl.Text);
                else
                    return new ModeSelectTool(tbLabel.Text, cbLabel.Checked, ModeSelect);
            }
        }

        public void SetupToolEdit()
        {
            Text = "Tool Edit";
            tsCustomFigureType.Visible = false;
            cbType.Visible = false;
            cbToolEditAddToPersonal.Visible = true;
            pnlLabel.Visible = false;
        }

        public void SetupToolButtonEdit()
        {
            Text = "Personal Toolbutton";
            cbType.Visible = true;
            cbToolEditAddToPersonal.Visible = false;
            tsCustomFigureType.Visible = true;
            pnlLabel.Visible = true;
        }

        public bool ToolEditAddToPersonal
        {
            get { return cbToolEditAddToPersonal.Checked; }
        }

        public ModeSelectType ModeSelect
        {
            get
            {
                if (rbSelect.Checked)
                    return ModeSelectType.Select;
                return ModeSelectType.Eraser;
            }
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
            // default to toolbutton edit
            SetupToolButtonEdit();
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
                case (int)PersonalToolButtonType.ModeSelect:
                    pnlModeSelect.BringToFront();
                    break;
                default:
                    if (tsCustomFigureProps.Dap == null)
                    {
                        tsCustomFigureProps.Dap = new DAuthorProperties();
                        tsCustomFigureType.FigureClass = typeof(PolylineFigure);
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