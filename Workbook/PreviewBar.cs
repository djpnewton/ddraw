using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace Workbook
{
    public delegate void PreviewSelectedHandler(Preview p);

    public partial class PreviewBar : UserControl
    {
        public event PreviewSelectedHandler PreviewSelected;
        public event PreviewContextHandler PreviewContext;
        public event PreviewMoveHandler PreviewMove;
        public event PreviewFigureDropHandler PreviewFigureDrop;
        public event PreviewNameChangedHandler PreviewNameChanged;

        int IdealPreviewWidth
        {
            get { return Width - SystemInformation.VerticalScrollBarWidth; }
        }

        public PreviewBar()
        {
            InitializeComponent();
        }

        int GetPreviewIndex(DEngine de)
        {
            if (de != null)
                for (int i = Controls.Count - 1; i >= 0; i--)
                    if (((Preview)Controls[i]).DEngine == de)
                        return i;
            return -1;
        }

        public void SetPreviewSelected(DEngine de)
        {
            if (de != null)
            {
                Preview pToSelect = (Preview)Controls[GetPreviewIndex(de)];
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
                idx = Controls.Count;
            // create preview
            Preview p = new Preview(de);
            p.Parent = this;
            Controls.SetChildIndex(p, idx);
            // set preview properties
            p.Width = IdealPreviewWidth;
            p.Height = 75;
            p.Left = 0;
            SetPreviewTops(idx); 
            p.Click += new EventHandler(p_Click);
            p.PreviewContext += new PreviewContextHandler(p_PreviewContext);
            p.PreviewMove += new PreviewMoveHandler(p_PreviewMove);
            p.PreviewFigureDrop += new PreviewFigureDropHandler(p_PreviewFigureDrop);
            p.PreviewNameChanged += new PreviewNameChangedHandler(p_PreviewNameChanged);
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
                Preview p = (Preview)Controls[idx];
                // remove from pnlPreview.Controls
                Controls.Remove(p);
                // set the preview positions
                SetPreviewTops(idx);
                // select a new preview
                if (p.Selected)
                {
                    if (idx < Controls.Count)
                    {
                        ((Preview)Controls[idx]).Selected = true;
                        DoPreviewSelected((Preview)Controls[idx]);
                    }
                    else if (idx > 0)
                    {
                        ((Preview)Controls[idx - 1]).Selected = true;
                        DoPreviewSelected((Preview)Controls[idx - 1]);
                    }
                }
            }
        }
        
        void SetPreviewTops(int idx)
        {
            while (idx < Controls.Count)
            {
                if (idx == 0)
                    Controls[idx].Top = 0 - VerticalScroll.Value;
                else
                    Controls[idx].Top = Controls[idx - 1].Bottom;
                idx++;
            }
        }

        public void Clear()
        {
            Controls.Clear();
        }

        void p_Click(object sender, EventArgs e)
        {
            Focus();
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
            Controls.SetChildIndex(p, Controls.IndexOf(to));
            SetPreviewTops(0);
            if (PreviewMove != null)
                PreviewMove(p, to);
        }

        void p_PreviewFigureDrop(Preview p, List<Figure> figs)
        {
            if (PreviewFigureDrop != null)
                PreviewFigureDrop(p, figs);
        }

        void p_PreviewNameChanged(Preview p, string name)
        {
            if (PreviewNameChanged != null)
                PreviewNameChanged(p, name);
        }

        public void Previous()
        {
            foreach (Preview p in Controls)
                if (p.Selected)
                {
                    int idx = Controls.IndexOf(p);
                    Preview pToSelect;
                    if (idx > 0)
                        pToSelect = (Preview)Controls[idx - 1];
                    else
                        pToSelect = (Preview)Controls[Controls.Count - 1];
                    pToSelect.Selected = true;
                    DoPreviewSelected(pToSelect);
                    break;
                }
        }

        public void Next()
        {
            foreach (Preview p in Controls)
                if (p.Selected)
                {
                    int idx = Controls.IndexOf(p);
                    Preview pToSelect;
                    if (idx < Controls.Count - 1)
                        pToSelect = (Preview)Controls[idx + 1];
                    else
                        pToSelect = (Preview)Controls[0];
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
            for (int i = Controls.Count-1; i >= 0; i--)
            {
                Preview p = (Preview)Controls[i];

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
                    Controls.SetChildIndex(Controls[idx], i);
                    reorder = true;
                }
            }
            if (reorder)
                SetPreviewTops(0);
        }

        public void UpdatePreviewsDirtyProps()
        {
            foreach (Preview p in Controls)
                p.Dirty = p.DEngine.CanUndo;
        }

        private void PreviewBar_SizeChanged(object sender, EventArgs e)
        {
            if (Controls.Count > 0)
            {
                if (Controls[0].Width != IdealPreviewWidth)
                    foreach (Preview p in Controls)
                        p.Width = IdealPreviewWidth;
            }
        }

        public void RenameCurrentPreview()
        {
            foreach (Preview p in Controls)
                if (p.Selected)
                {
                    p.Rename();
                    break;
                }
        }

        public void UpdatePreviewNames()
        {
            foreach (Preview p in Controls)
                p.UpdateName();
        }
    }
}
