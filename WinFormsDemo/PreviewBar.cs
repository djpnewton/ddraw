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
        public event PreviewContextHandler PreviewContext;

        int baseWidth = -1;

        public PreviewBar()
        {
            InitializeComponent();
        }

        int GetPreviewIndex(DEngine de)
        {
            if (de != null)
                for (int i = previews.Count - 1; i >= 0; i--)
                    if (previews[i].DEngine == de)
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
            int idx = GetPreviewIndex(sibling) + 1;
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
            // add to preview list
            previews.Insert(idx, p);
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
                Preview p = previews[idx];
                // remove from preview list and pnlPreview.Controls
                previews.Remove(p);
                pnlPreviews.Controls.Remove(p);
                // set the preview positions
                SetPreviewTops(idx);
                // select a new preview
                if (p.Selected)
                {
                    if (idx < previews.Count)
                    {
                        previews[idx].Selected = true;
                        DoPreviewSelected(previews[idx]);
                    }
                    else if (idx > 0)
                    {
                        previews[idx - 1].Selected = true;
                        DoPreviewSelected(previews[idx - 1]);
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
            previews.Clear();
        }

        void p_Click(object sender, EventArgs e)
        {
            ((Preview)sender).Selected = true;
            DoPreviewSelected((Preview)sender);
        }

        void p_PreviewContext(Preview p, Point pt)
        {
            if (PreviewContext != null)
                PreviewContext(p, pt);
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
                    DoPreviewSelected(pToSelect);
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
