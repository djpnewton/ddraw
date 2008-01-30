using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public delegate void PreviewSelectedHandler(Preview p);

    public partial class PreviewBar : UserControl
    {
        List<Preview> previews = new List<Preview>();

        public event PreviewSelectedHandler PreviewSelected;
        public event EventHandler PreviewAdd;

        int baseWidth = -1;

        public PreviewBar()
        {
            InitializeComponent();
        }

        public Preview AddPreview(DEngine de, DViewer dv)
        {
            if (baseWidth == -1)
            {
                if (pnlPreviews.VerticalScroll.Visible)
                    baseWidth = Width - SystemInformation.VerticalScrollBarWidth;
                else
                    baseWidth = Width;
            }

            Preview p = new Preview(de);
            p.Width = pnlPreviews.Width;
            p.Height = 65;
            p.Left = 0;
            if (pnlPreviews.Controls.Count > 0)
                p.Top = pnlPreviews.Controls[pnlPreviews.Controls.Count - 1].Bottom;
            else
                p.Top = 0;
            p.Parent = pnlPreviews;
            p.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            p.Click += new EventHandler(p_Click);
            previews.Add(p);
            InvokeOnClick(p, new EventArgs());

            return p;
        }

        public void Clear()
        {
            pnlPreviews.Controls.Clear();
        }

        void p_Click(object sender, EventArgs e)
        {
            ((Preview)sender).Selected = true;
            if (PreviewSelected != null)
                PreviewSelected((Preview)sender);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (PreviewAdd != null)
                PreviewAdd(this, new EventArgs());
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            foreach (Preview p in previews)
                if (p.Selected)
                {
                    int idx = previews.IndexOf(p);
                    Preview pToSelect;
                    if (idx > 0)
                        pToSelect = previews[idx - 1];
                    else
                        pToSelect = previews[previews.Count - 1];
                    pToSelect.Selected = true;
                    if (PreviewSelected != null)
                        PreviewSelected(pToSelect);
                    break;
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            foreach (Preview p in previews)
                if (p.Selected)
                {
                    int idx = previews.IndexOf(p);
                    Preview pToSelect;
                    if (idx < previews.Count-1)
                        pToSelect = previews[idx + 1];
                    else
                        pToSelect = previews[0];
                    pToSelect.Selected = true;
                    if (PreviewSelected != null)
                        PreviewSelected(pToSelect);
                    break;
                }
        }

        private void pnlPreviews_Resize(object sender, EventArgs e)
        {
            if (baseWidth != -1)
            {
                if (pnlPreviews.VerticalScroll.Visible)
                    Width = baseWidth + SystemInformation.VerticalScrollBarWidth;
                else
                    Width = baseWidth;
            }
        }
    }
}
