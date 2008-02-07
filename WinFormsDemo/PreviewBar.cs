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
        public event PreviewSelectedHandler PreviewSelected;
        public event EventHandler PreviewAdd;
        public event PreviewContextHandler PreviewContext;
        public event PreviewMoveHandler PreviewMove;

        int baseWidth = -1;

        public PreviewBar()
        {
            InitializeComponent();
        }

        int GetPreviewIndex(DEngine de)
        {
            if (de != null)
                for (int i = pnlPreviews.Controls.Count - 1; i >= 0; i--)
                    if (((Preview)pnlPreviews.Controls[i]).DEngine == de)
                        return i;
            return -1;
        }

        public Preview AddPreview(DEngine de, DViewer dv, DEngine sibling)
        {
            if (baseWidth == -1)
            {
                if (pnlPreviews.VerticalScroll.Visible)
                    baseWidth = Width - SystemInformation.VerticalScrollBarWidth;
                else
                    baseWidth = Width;
            }
            // index of new preview
            int idx;
            if (sibling != null)
                idx = GetPreviewIndex(sibling) + 1;
            else
                idx = pnlPreviews.Controls.Count;
            // create preview
            Preview p = new Preview(de);
            p.Parent = pnlPreviews;
            pnlPreviews.Controls.SetChildIndex(p, idx);
            // set preview properties
            p.Width = pnlPreviews.Width;
            p.Height = 65;
            p.Left = 0;
            SetPreviewTops(idx); 
            p.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            p.Click += new EventHandler(p_Click);
            p.PreviewContext += new PreviewContextHandler(p_PreviewContext);
            p.PreviewMove += new PreviewMoveHandler(p_PreviewMove);
            // select it
            p.Selected = true;
            DoPreviewSelected(p);
            // return p
            return p;
        }

        public void RemovePreview(DEngine de)
        {
            int idx = GetPreviewIndex(de);
            if (idx > -1)
            {
                Preview p = (Preview)pnlPreviews.Controls[idx];
                // remove from pnlPreview.Controls
                pnlPreviews.Controls.Remove(p);
                // set the preview positions
                SetPreviewTops(idx);
                // select a new preview
                if (p.Selected)
                {
                    if (idx < pnlPreviews.Controls.Count)
                    {
                        ((Preview)pnlPreviews.Controls[idx]).Selected = true;
                        DoPreviewSelected((Preview)pnlPreviews.Controls[idx]);
                    }
                    else if (idx > 0)
                    {
                        ((Preview)pnlPreviews.Controls[idx - 1]).Selected = true;
                        DoPreviewSelected((Preview)pnlPreviews.Controls[idx - 1]);
                    }
                }
            }
        }
        
        void SetPreviewTops(int idx)
        {
            while (idx < pnlPreviews.Controls.Count)
            {
                if (idx == 0)
                    pnlPreviews.Controls[idx].Top = 0;
                else
                    pnlPreviews.Controls[idx].Top = pnlPreviews.Controls[idx - 1].Bottom;
                idx++;
            }
        }

        public void Clear()
        {
            pnlPreviews.Controls.Clear();
        }

        void p_Click(object sender, EventArgs e)
        {
            if (!((Preview)sender).Selected)
            {
                ((Preview)sender).Selected = true;
                DoPreviewSelected((Preview)sender);
            }
        }

        void p_PreviewContext(Preview p, Point pt)
        {
            if (PreviewContext != null)
                PreviewContext(p, pt);
        }

        void p_PreviewMove(Preview p, Preview to)
        {
            pnlPreviews.Controls.SetChildIndex(p, pnlPreviews.Controls.IndexOf(to));
            SetPreviewTops(0);
            if (PreviewMove != null)
                PreviewMove(p, to);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (PreviewAdd != null)
                PreviewAdd(this, new EventArgs());
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            foreach (Preview p in pnlPreviews.Controls)
                if (p.Selected)
                {
                    int idx = pnlPreviews.Controls.IndexOf(p);
                    Preview pToSelect;
                    if (idx > 0)
                        pToSelect = (Preview)pnlPreviews.Controls[idx - 1];
                    else
                        pToSelect = (Preview)pnlPreviews.Controls[pnlPreviews.Controls.Count - 1];
                    pToSelect.Selected = true;
                    DoPreviewSelected(pToSelect);
                    break;
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            foreach (Preview p in pnlPreviews.Controls)
                if (p.Selected)
                {
                    int idx = pnlPreviews.Controls.IndexOf(p);
                    Preview pToSelect;
                    if (idx < pnlPreviews.Controls.Count - 1)
                        pToSelect = (Preview)pnlPreviews.Controls[idx + 1];
                    else
                        pToSelect = (Preview)pnlPreviews.Controls[0];
                    pToSelect.Selected = true;
                    DoPreviewSelected(pToSelect);
                    break;
                }
        }

        void DoPreviewSelected(Preview p)
        {
            if (PreviewSelected != null)
                PreviewSelected(p);
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
