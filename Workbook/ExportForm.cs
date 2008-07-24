using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DDraw;
using DDraw.WinForms;

namespace Workbook
{
    public partial class ExportForm : Form
    {
        string docFileName;
        IList<DEngine> engines;
        DEngine current;

        public ExportForm(string docFileName, IList<DEngine> engines, DEngine current)
        {
            InitializeComponent();
            // 
            this.docFileName = docFileName;
            this.engines = engines;
            this.current = current;
            // select starting options
            rbPDF.Checked = true;
            rbCurrent.Checked = true;
            // fill lbPageSelect
            lbPageSelect.Items.Clear();
            for (int i = 1; i <= engines.Count; i++)
                lbPageSelect.Items.Add(i);
            lbPageSelect.Enabled = false;
        }

        private void rbSelectPages_CheckedChanged(object sender, EventArgs e)
        {
            lbPageSelect.Enabled = rbSelectPages.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // create list of engines to export
            IList<DEngine> expEngines = new List<DEngine>();
            if (rbCurrent.Checked)
                expEngines.Add(current);
            else if (rbAllPages.Checked)
                expEngines = engines;
            else
                foreach (int item in lbPageSelect.SelectedItems)
                    expEngines.Add(engines[item - 1]);
            // export to selected format
            if (expEngines.Count <= 0)
                MessageBox.Show("No pages selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (rbPDF.Checked)
                {
                    if (ExportPDF(expEngines))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                 }
                else if (rbImage.Checked)
                {
                    if (ExportImage(expEngines))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }

        private bool ExportPDF(IList<DEngine> expEngines)
        {
            bool result = false;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Document|*.pdf";
            sfd.FileName = Path.GetFileNameWithoutExtension(docFileName) + ".pdf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                result = true;
                try
                {
                    /*** no progress form as this requires Application.DoEvents which will 
                    kill rendering (cairo/gdi+ at the same time) ***/

                    WFCairoGraphics dg = WFHelper.MakeCairoPDFGraphics(sfd.FileName, 0, 0);
                    dg.Scale(0.75, 0.75); // TODO figure out why this is needed (gak!)
                    foreach (DEngine de in expEngines)
                    {
                        WFHelper.SetCairoPDFSurfaceSize(dg, PageTools.SizetoSizeMM(de.PageSize));
                        // create cairifyed figures/DEngine
                        DEngine cairoDe = CairifyEngine(de);
                        // print page
                        DPrintViewer dvPrint = new DPrintViewer();
                        //dvPrint.SetPageSize(de.PageSize);
                        dvPrint.Paint(dg, cairoDe.BackgroundFigure, cairoDe.Figures);
                        WFHelper.ShowCairoPDFPage(dg);
                    }
                    dg.Dispose();
                    GC.Collect(); // release all the cairo stuff so the pdf gets written?
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception e)
                {
                    result = false;
                    MessageBox.Show("ERROR", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return result;
        }

        private DEngine CairifyEngine(DEngine de)
        {
            if (!WFHelper.Cairo)
            {
                // set to cairo graphics
                WFCairoGraphics.Init();
                // create new engine
                DEngine cairoDe = new DEngine(null);
                cairoDe.UndoRedo.Start("blah");
                // page size
                cairoDe.PageSize = de.PageSize;
                // figures
                List<Figure> figs = FigureSerialize.FromXml(FigureSerialize.FormatToXml(de.Figures, null));
                foreach (Figure f in figs)
                    cairoDe.AddFigure(f);
                // background figure
                figs = FigureSerialize.FromXml(FigureSerialize.FormatToXml(de.BackgroundFigure, null));
                //CairifyFigures(figs);
                cairoDe.SetBackgroundFigure((BackgroundFigure)figs[0], de.CustomBackgroundFigure);
                cairoDe.UndoRedo.Commit();
                // reset back to GDI graphics
                GDIGraphics.Init();
                return cairoDe;
            }
            return de;
        }

        private bool ExportImage(IList<DEngine> expEngines)
        {
            bool result = false;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                result = true;
                try
                {
                    // setup progress form
                    ProgressForm pf = new ProgressForm();
                    pf.Text = "Exporting to Images";
                    pf.Shown += delegate(object s, EventArgs e)
                    {
                        string fileNameTemplate = Path.GetFileNameWithoutExtension(docFileName) + "{0}.png";
                        DPrintViewer dvPrint = new DPrintViewer();
                        foreach (DEngine de in expEngines)
                        {
                            DBitmap bmp = WFHelper.MakeBitmap((int)de.PageSize.X, (int)de.PageSize.Y);
                            DGraphics dg = WFHelper.MakeGraphics(bmp);
                            dvPrint.Paint(dg, de.BackgroundFigure, de.Figures);
                            bmp.Save(Path.Combine(fbd.SelectedPath, string.Format(fileNameTemplate, engines.IndexOf(de) + 1)));
                            dg.Dispose();
                            bmp.Dispose();
                        }
                        pf.Close();
                        System.Diagnostics.Process.Start(fbd.SelectedPath);
                    };
                    pf.ShowDialog();
                }
                catch (Exception e)
                {
                    result = false;
                    MessageBox.Show("ERROR", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return result;
        }
    }
}