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
                    de.Copy(de.Figures, out bmp, false, DColor.Clear);
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

        DEngine de;
        DTkViewer dv;

        public PtButtonForm()
        {
            InitializeComponent();
            dv = new WFViewer(vcCustomFigure);
            dv.EditFigures = false;
            dv.AntiAlias = true;
            dv.Preview = true;
            de = new DEngine(tsCustomFigureProps.Dap, false);
            de.AddViewer(dv);
            // set page height to viewer size
            de.UndoRedoStart("blah");
            de.PageSize = new DPoint(vcCustomFigure.Width, vcCustomFigure.Height);
            de.UndoRedoCommit();
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
                        tsCustomFigureProps.Dap = DAuthorProperties.GlobalAP.Clone();
                        tsCustomFigureProps.FigureClass = typeof(PolylineFigure);
                        tsCustomFigureType.FigureClass = typeof(PolylineFigure);
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
            de.ClearPage();
            de.UndoRedoStart("blah");
            Figure f = (Figure)Activator.CreateInstance(figureClass);
            tsCustomFigureProps.Dap.ApplyPropertiesToFigure(f);
            if (f is PolylineFigure || f is LineFigure)
            {
                DPoint pt1 = new DPoint(vcCustomFigure.Width / 4.0, vcCustomFigure.Height / 4.0);
                DPoint pt2 = new DPoint(vcCustomFigure.Width * 3 / 4.0, vcCustomFigure.Height * 3 / 4.0);
                if (f is PolylineFigure)
                {
                    DPoints pts = new DPoints();
                    pts.Add(pt1);
                    pts.Add(new DPoint(vcCustomFigure.Width * 1.25 / 4.0, vcCustomFigure.Height * 1.10 / 4.0));
                    pts.Add(new DPoint(vcCustomFigure.Width * 1.50 / 4.0, vcCustomFigure.Height * 1.25 / 4.0));
                    pts.Add(new DPoint(vcCustomFigure.Width * 1.75 / 4.0, vcCustomFigure.Height * 1.50 / 4.0));
                    pts.Add(new DPoint(vcCustomFigure.Width * 2.00 / 4.0, vcCustomFigure.Height * 1.75 / 4.0)); 
                    pts.Add(new DPoint(vcCustomFigure.Width * 2.25 / 4.0, vcCustomFigure.Height * 2.00 / 4.0));
                    pts.Add(new DPoint(vcCustomFigure.Width * 2.50 / 4.0, vcCustomFigure.Height * 2.25 / 4.0));
                    pts.Add(new DPoint(vcCustomFigure.Width * 2.75 / 4.0, vcCustomFigure.Height * 2.50 / 4.0));
                    pts.Add(pt2);
                    ((PolylineFigure)f).Points = pts;
                }
                else if (f is LineFigure)
                {
                    ((LineFigure)f).Pt1 = pt1;
                    ((LineFigure)f).Pt2 = pt2;
                }
            }
            else if (f is TextFigure)
                ((TextFigure)f).Text = "AaBbCc";
            f.Left = vcCustomFigure.Width / 4.0;
            f.Top = vcCustomFigure.Height / 4.0;
            f.Width = vcCustomFigure.Width / 2.0;
            f.Height = vcCustomFigure.Height / 2.0;
            SetTextSize(f);
            de.AddFigure(f);
            de.UndoRedoCommit();
            dv.Update();
        }

        private void SetTextSize(Figure f)
        {
            if (f is TextFigure)
            {
                f.Left = vcCustomFigure.Width / 8.0;
                f.Width = vcCustomFigure.Width * 3 / 4.0;
            }
        }

        void Dap_PropertyChanged(DAuthorProperties dap)
        {
            de.UndoRedoStart("blah");
            foreach (Figure f in de.Figures)
            {
                dap.ApplyPropertiesToFigure(f);
                SetTextSize(f); // if the font has changed we need to update the text size
            }
            de.UndoRedoCommit();
            dv.Update();
        }
    }
}