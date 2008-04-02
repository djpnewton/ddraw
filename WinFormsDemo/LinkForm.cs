using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public partial class LinkForm : Form
    {
        public LinkType LinkType
        {
            get
            {
                if (rbWebPage.Checked)
                    return LinkType.WebPage;
                if (rbFile.Checked)
                    return LinkType.File;
                if (rbPage.Checked)
                    return LinkType.Page;
                return LinkType.Attachment;
            }
            set
            {
                switch (value)
                {
                    case LinkType.WebPage:
                        rbWebPage.Checked = true;
                        break;
                    case LinkType.File:
                        rbFile.Checked = true;
                        break;
                    case LinkType.Page:
                        rbPage.Checked = true;
                        break;
                    case LinkType.Attachment:
                        rbAttachment.Checked = true;
                        break;
                }
            }
        }

        public string WebPage
        {
            get { return tbAddress.Text; }
            set { tbAddress.Text = value; }
        }

        public string File
        {
            get { return tbFile.Text; }
            set { tbFile.Text = value; }
        }

        public bool CopyFileToAttachments
        {
            get { return cbCopyFileToAttachments.Checked; }
            set { cbCopyFileToAttachments.Checked = value; }
        }

        public LinkPage LinkPage
        {
            get
            {
                if (rbPageFirst.Checked)
                    return LinkPage.First;
                else if (rbPageLast.Checked)
                    return LinkPage.Last;
                else if (rbPageNext.Checked)
                    return LinkPage.Next;
                else if (rbPagePrevious.Checked)
                    return LinkPage.Previous;
                else
                    return LinkPage.None;
            }
            set
            {
                switch (value)
                {
                    case LinkPage.None:
                        lbPages.SelectedIndex = 0;
                        break;
                    case LinkPage.First:
                        rbPageFirst.Checked = true;
                        break;
                    case LinkPage.Last:
                        rbPageLast.Checked = true;
                        break;
                    case LinkPage.Next:
                        rbPageNext.Checked = true;
                        break;
                    case LinkPage.Previous:
                        rbPagePrevious.Checked = true;
                        break;
                }
            }
        }

        public int PageNum
        {
            get { return lbPages.SelectedIndex; }
            set 
            { 
                if (value >= 0 && value < lbPages.Items.Count)
                    lbPages.SelectedIndex = value; 
            }
        }

        public string Attachment
        {
            get
            {
                if (lbAttachments.SelectedIndex != -1)
                    return (string)lbAttachments.Items[lbAttachments.SelectedIndex];
                return null;
            }
            set { lbAttachments.SelectedItem = value; }
        }

        public bool LinkBody
        {
            get { return rbLinkBody.Checked; }
            set
            {
                if (value) rbLinkBody.Checked = true;
                else rbLinkIcon.Checked = true;
            }
        }

        List<DEngine> engines;
        public List<DEngine> Engines
        {
            get { return engines; }
            set 
            { 
                engines = value;
                lbPages.Items.Clear();
                if (engines.Count > 0)
                {
                    for (int i = 1; i <= engines.Count; i++)
                        lbPages.Items.Add(i);
                    lbPages.SelectedIndex = 0;
                }
            }
        }

        DEngine currentEngine;
        public DEngine CurrentEngine
        {
            set { currentEngine = value; }
        }

        List<string> attachments;
        public List<string> Attachments
        {
            get { return attachments; }
            set
            {
                attachments = value;
                lbAttachments.Items.Clear();
                if (attachments.Count > 0)
                {
                    foreach (string s in attachments)
                        lbAttachments.Items.Add(s);
                    lbAttachments.SelectedIndex = 0;
                }
            }
        }

        DEngine currentPreviewEngine = null;
        DTkViewer currentPreviewViewer = null;

        public LinkForm()
        {
            InitializeComponent();
        }

        private void LinkType_Changed(object sender, EventArgs e)
        {
            if (rbWebPage.Checked)
                pnlWebPage.BringToFront();
            if (rbFile.Checked)
                pnlFile.BringToFront();
            if (rbPage.Checked)
                pnlPage.BringToFront();
            if (rbAttachment.Checked)
                pnlAttachment.BringToFront();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                tbFile.Text = ofd.FileName;
        }

        void ShowPagePreview(int engineIdx)
        {
            // bound engine index to 0 <-> engines.Count - 1
            if (engineIdx >= engines.Count)
                engineIdx = 0;
            if (engineIdx < 0)
                engineIdx = engines.Count - 1;
            // setup new preview
            if (currentPreviewEngine != null && currentPreviewViewer != null)
                currentPreviewEngine.RemoveViewer(currentPreviewViewer);
            currentPreviewViewer = new WFViewer(wfViewerControl1);
            currentPreviewViewer.AntiAlias = true;
            currentPreviewViewer.EditFigures = false;
            currentPreviewViewer.Preview = true;
            currentPreviewEngine = engines[engineIdx];
            currentPreviewEngine.AddViewer(currentPreviewViewer);
        }

        private void lbPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPages.SelectedIndex != -1)
            {
                // deselect page radio buttons
                rbPageFirst.Checked = false;
                rbPageLast.Checked = false;
                rbPageNext.Checked = false;
                rbPagePrevious.Checked = false;
                // show preview
                ShowPagePreview(lbPages.SelectedIndex);
            }
        }

        private void rbPage_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // deselect lbPages
                lbPages.SelectedIndex = -1;
                // show preview
                if (sender == rbPageFirst)
                    ShowPagePreview(0);
                if (sender == rbPageLast)
                    ShowPagePreview(engines.Count - 1);
                if (sender == rbPageNext)
                    ShowPagePreview(engines.IndexOf(currentEngine) + 1);
                else if (sender == rbPagePrevious)
                    ShowPagePreview(engines.IndexOf(currentEngine) - 1);
            }
        }
    }

    public enum LinkType { WebPage, File, Attachment, Page };

    public enum LinkPage { None, First = -1, Last = -2, Next = -3, Previous = -4 };

    public static class Links
    {
        public const string Link = "Link";
        public const string LinkType = "LinkType";
        public const string LinkBody = "LinkBody";

        public static LinkType StringToLinkType(string p)
        {
            return (LinkType)Enum.Parse(typeof(LinkType), p, true);
        }
    }
}