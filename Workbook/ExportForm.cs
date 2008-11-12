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
            LocalizeUI();
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

        private void LocalizeUI()
        {
            Text = WbLocale.Export;
            gbExportFormat.Text = WbLocale.Format;
            gbPages.Text = WbLocale.Pages;
            rbPDF.Text = WbLocale.PDF;
            rbImage.Text = WbLocale.Image;
            rbPng.Text = WbLocale.PNG;
            rbEmf.Text = WbLocale.EMF;
            rbCurrent.Text = WbLocale.Current;
            rbAllPages.Text = WbLocale.All;
            rbSelectPages.Text = WbLocale.Select;
            btnOk.Text = WbLocale.Ok;
            btnCancel.Text = WbLocale.Cancel;
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
                MessageBox.Show(WbLocale.NoPagesSelected, WbLocale.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    WorkBookUtils.RenderPdf(expEngines, sfd.FileName);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception e)
                {
                    result = false;
                    MessageBox.Show(WbLocale.ERROR, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return result;
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
                    pf.Text = WbLocale.ExportingToImages;
                    pf.Shown += delegate(object s, EventArgs e)
                    {
                        string fileNameTemplate;
                        if (rbPng.Checked)
                            fileNameTemplate = Path.GetFileNameWithoutExtension(docFileName) + "{0}.png";
                        else
                            fileNameTemplate = Path.GetFileNameWithoutExtension(docFileName) + "{0}.emf";
                        DPrintViewer dvPrint = new DPrintViewer();
                        foreach (DEngine de in expEngines)
                        {
                            DBitmap bmp = WFHelper.MakeBitmap((int)de.PageSize.X, (int)de.PageSize.Y);
                            DGraphics dg;
                            if (rbPng.Checked)
                                dg = WFHelper.MakeGraphics(bmp);
                            else
                                dg = new EmfGraphics(new DRect(0, 0, de.PageSize.X, de.PageSize.Y), WorkBookUtils.GetScreenMM(), WorkBookUtils.GetScreenRes());
                            dvPrint.Paint(dg, de.BackgroundFigure, de.Figures);
                            if (rbPng.Checked)
                                bmp.Save(Path.Combine(fbd.SelectedPath, string.Format(fileNameTemplate, engines.IndexOf(de) + 1)));
                            else
                                ((EmfGraphics)dg).SaveToFile(Path.Combine(fbd.SelectedPath, string.Format(fileNameTemplate, engines.IndexOf(de) + 1)));
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
                    MessageBox.Show(WbLocale.ERROR, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return result;
        }

        private void rbPDF_CheckedChanged(object sender, EventArgs e)
        {
            rbPng.Enabled = false;
            rbEmf.Enabled = false;
        }

        private void rbImage_CheckedChanged(object sender, EventArgs e)
        {
            rbPng.Enabled = true;
            rbEmf.Enabled = true;
            rbPng.Checked = true;
        }
    }
}