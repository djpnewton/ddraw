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

namespace WinFormsDemo
{
    public partial class ExportForm : Form
    {
        string docFileName;
        DEngineManager dem;
        DEngine current;

        public ExportForm(string docFileName, DEngineManager dem, DEngine current)
        {
            InitializeComponent();
            // 
            this.docFileName = docFileName;
            this.dem = dem;
            this.current = current;
            // select starting options
            rbPDF.Enabled = WFHelper.Cairo;
            if (WFHelper.Cairo)
                rbPDF.Checked = true;
            else
                rbImage.Checked = true;
            rbCurrent.Checked = true;
            // fill lbPageSelect
            lbPageSelect.Items.Clear();
            for (int i = 1; i <= dem.GetEngines().Count; i++)
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
            List<DEngine> expEngines = new List<DEngine>();
            if (rbCurrent.Checked)
                expEngines.Add(current);
            else if (rbAllPages.Checked)
                expEngines = dem.GetEngines();
            else
                foreach (int item in lbPageSelect.SelectedItems)
                    expEngines.Add(dem.GetEngine(item - 1));
            // export to selected format
            if (expEngines.Count <= 0)
                MessageBox.Show("No pages selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (rbPDF.Checked)
                {
                    if (WFHelper.Cairo)
                    {
                        if (ExportPDF(expEngines))
                        {
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }
                    else
                        MessageBox.Show("Export to PDF only works with Cairo enabled builds", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool ExportPDF(List<DEngine> expEngines)
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
                    // setup progress form
                    ProgressForm pf = new ProgressForm();
                    pf.Text = "Exporting to PDF Document";
                    pf.Shown += delegate(object s, EventArgs e)
                    {
                        WFCairoGraphics dg = WFHelper.MakePDFCairoGraphics(sfd.FileName, 0, 0);
                        dg.Scale(0.75, 0.75); // TODO figure out why this is needed (gak!)
                        foreach (DEngine de in expEngines)
                        {
                            Application.DoEvents();
                            WFHelper.SetCairoPDFSurfaceSize(dg, PageTools.SizetoSizeMM(de.PageSize));
                            DPrintViewer dvPrint = new DPrintViewer();
                            //dvPrint.SetPageSize(de.PageSize);
                            dvPrint.Paint(dg, de.GetBackgroundFigure(), de.Figures);
                            WFHelper.ShowCairoPDFPage(dg);
                        }
                        dg.Dispose();
                        pf.Close();
                        System.Diagnostics.Process.Start(sfd.FileName);
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

        private bool ExportImage(List<DEngine> expEngines)
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
                            dvPrint.Paint(dg, de.GetBackgroundFigure(), de.Figures);
                            bmp.Save(Path.Combine(fbd.SelectedPath, string.Format(fileNameTemplate, dem.IndexOfEngine(de) + 1)));
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