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
                return LinkType.Page;
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

        public int Page
        {
            get { return lbPages.SelectedIndex; }
            set 
            { 
                if (value >= 0 && value < lbPages.Items.Count)
                    lbPages.SelectedIndex = value; 
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

        DEngine de = null;
        DTkViewer dv = null;

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
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                tbFile.Text = ofd.FileName;
        }

        private void lbPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (de != null && dv != null)
                de.RemoveViewer(dv);
            dv = new WFViewer(wfViewerControl1);
            dv.AntiAlias = true;
            dv.EditFigures = false;
            dv.Preview = true;
            de = engines[lbPages.SelectedIndex];
            de.AddViewer(dv);
        }
    }

    public enum LinkType { WebPage, File, Page };

    public static class Links
    {
        public const string Link = "Link";
        public const string LinkType = "LinkType";

        public static LinkType StringToLinkType(string p)
        {
            return (LinkType)Enum.Parse(typeof(LinkType), p, true);
        }
    }
}