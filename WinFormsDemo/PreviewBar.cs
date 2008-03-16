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
        public event PreivewFigureDropHandler PreviewFigureDrop;

        int IdealPreviewWidth
        {
            get { return pnlPreviews.Width - SystemInformation.VerticalScrollBarWidth; }
        }

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

        public void SetPreviewSelected(DEngine de)
        {
            if (de != null)
            {
                Preview pToSelect = (Preview)pnlPreviews.Controls[GetPreviewIndex(de)];
                pToSelect.Selected = true;
                DoPreviewSelected(pToSelect);
            }
        }

        public Preview AddPreview(DEngine de, DViewer dv, DEngine sibling)
        {
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
            p.Width = IdealPreviewWidth;
            p.Height = 65;
            p.Left = 0;
            SetPreviewTops(idx); 
            p.Click += new EventHandler(p_Click);
            p.PreviewContext += new PreviewContextHandler(p_PreviewContext);
            p.PreviewMove += new PreviewMoveHandler(p_PreviewMove);
            p.PreviewFigureDrop += new PreivewFigureDropHandler(p_PreviewFigureDrop);
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
                    pnlPreviews.Controls[idx].Top = 0 - pnlPreviews.VerticalScroll.Value;
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

        void p_PreviewFigureDrop(Preview p, List<Figure> figs)
        {
            if (PreviewFigureDrop != null)
                PreviewFigureDrop(p, figs);
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

        public void MatchPreviewsToEngines(List<DEngine> engines, DViewer dv)
        {
            // Remove previews that dont have a matching preview
            for (int i = pnlPreviews.Controls.Count-1; i >= 0; i--)
            {
                Preview p = (Preview)pnlPreviews.Controls[i];

                if (engines.IndexOf(p.DEngine) == -1)
                    RemovePreview(p.DEngine);
            }
            // Add previews for engines that dont have them
            for (int i = 0; i < engines.Count; i++)
            {
                if (GetPreviewIndex(engines[i]) == -1)
                {
                    if (i > 0)
                        AddPreview(engines[i], dv, engines[i - 1]);
                    else
                        AddPreview(engines[i], dv, null);                  
                }
            }
            // Move previews that dont match engine order
            bool reorder = false;
            for (int i = 0; i < engines.Count; i++)
            {
                int idx = GetPreviewIndex(engines[i]);
                if (idx != i)
                {
                    pnlPreviews.Controls.SetChildIndex(pnlPreviews.Controls[idx], i);
                    reorder = true;
                }
            }
            if (reorder)
                SetPreviewTops(0);
        }

        public void UpdatePreviewsDirtyProps()
        {
            foreach (Preview p in pnlPreviews.Controls)
                p.Dirty = p.DEngine.CanUndo;
        }

        private void pnlPreviews_SizeChanged(object sender, EventArgs e)
        {
            if (pnlPreviews.Controls.Count > 0)
            {
                if (pnlPreviews.Controls[0].Width != IdealPreviewWidth)
                    foreach (Preview p in pnlPreviews.Controls)
                        p.Width = IdealPreviewWidth;
            }
        }
    }
}
