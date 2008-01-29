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

        public PreviewBar()
        {
            InitializeComponent();
        }

        public Preview AddPreview(DEngine de, DViewer dv)
        {
            Preview p = new Preview(de);
            p.Width = ClientSize.Width;
            p.Height = 65;
            if (Controls.Count > 0)
                p.Top = Controls[Controls.Count - 1].Bottom;
            else
                p.Top = 0;
            p.Parent = this;
            p.Click += new EventHandler(p_Click);
            previews.Add(p);
            InvokeOnClick(p, new EventArgs());
            return p;
        }

        public void Clear()
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
                if (Controls[i] is Preview)
                    Controls.Remove(Controls[i]);
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
    }
}
