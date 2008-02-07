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
    public delegate void PreviewContextHandler(Preview p, Point pt);
    public delegate void PreviewMoveHandler(Preview p, Preview to);

    public class Preview : UserControl
    {
        bool radioSelect = true;
        public bool RadioSelect
        {
            get { return radioSelect; }
            set { radioSelect = value; }
        }
        public bool Selected
        {
            get { return viewerHolder.BackColor == Color.Red; }
            set
            {
                if (value)
                {
                    viewerHolder.BackColor = Color.Red;
                    // deselect other siblings
                    if (radioSelect)
                        foreach (Control c in Parent.Controls)
                            if (c is Preview && c != this)
                                ((Preview)c).Selected = false;
                    // scroll into view
                    if (Parent is ScrollableControl)
                        ((ScrollableControl)Parent).ScrollControlIntoView(this);
                }
                else
                    viewerHolder.BackColor = Color.Empty;
            }
        }

        public event PreviewContextHandler PreviewContext;
        public event PreviewMoveHandler PreviewMove;

        WFViewerControl viewerControl;
        public WFViewerControl ViewerControl
        {
            get { return viewerControl; }
        }

        DEngine de;
        public DEngine DEngine
        {
            get { return de; }
        }
        DTkViewer dv;
        public DTkViewer DViewer
        {
            get { return dv; }
        }
        
        const int MARGIN = 1;
        Panel viewerHolder;
        PictureBox pbContext;

        public Preview(DEngine de)
        {
            viewerHolder = new Panel();
            viewerHolder.Location = new Point(0, 0);
            viewerHolder.Size = Size;
            Controls.Add(viewerHolder);
            // viewerControl
            viewerControl = new WFViewerControl();
            viewerControl.Location = new Point(MARGIN, MARGIN);
            viewerControl.Size = new Size(Width - MARGIN * 2, Height - MARGIN * 2);
            viewerControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            viewerControl.MouseDown += new MouseEventHandler(viewerControl_MouseDown);
            viewerControl.MouseUp += new MouseEventHandler(viewerControl_MouseUp);
            viewerHolder.Controls.Add(viewerControl);
            // 
            // pbContext
            // 
            pbContext = new PictureBox();
            pbContext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbContext.Image = global::WinFormsDemo.Resource1.arrow;
            pbContext.BackColor = Color.Transparent;
            pbContext.Size = new Size(pbContext.Image.Width, pbContext.Image.Height);
            pbContext.Location = new Point(Width - pbContext.Image.Width - 2, 0);
            pbContext.Click += new EventHandler(pbContext_Click);
            viewerControl.Controls.Add(pbContext);
            // Preview
            AllowDrop = true;
            DragEnter += new DragEventHandler(Preview_DragEnter);
            DragDrop += new DragEventHandler(Preview_DragDrop);
            // DEngine
            this.de = de;
            dv = new WFViewer(viewerControl);
            dv.Preview = true;
            de.PageSizeChanged += new PageSizeChangedHandler(de_PageSizeChanged);
            de.AddViewer(dv);
        }

        void viewerControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                InvokeOnClick(this, e);
                DoDragDrop(this, DragDropEffects.Move);
            }
        }

        void viewerControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && PreviewContext != null)
            {
                InvokeOnClick(this, e);
                PreviewContext(this, new Point(e.X, e.Y));
            }
        }

        private void pbContext_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
            if (PreviewContext != null)
                PreviewContext(this, new Point(pbContext.Left, pbContext.Bottom));
        }

        void Preview_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move && e.Data.GetDataPresent(this.GetType()))
                e.Effect = DragDropEffects.Move;
        }

        void Preview_DragDrop(object sender, DragEventArgs e)
        {
            Preview pDrag = (Preview)e.Data.GetData(this.GetType());
            if (pDrag != this && PreviewMove != null)
                PreviewMove(pDrag, this);
        }

        private void de_PageSizeChanged(DEngine de, DPoint pageSize)
        {
            if (pageSize.X / Width > pageSize.Y / Height)
            {
                viewerHolder.Left = 0;
                viewerHolder.Width = Width;
                viewerHolder.Height = (int)Math.Round(Height * ((Width / pageSize.X) / (Height / pageSize.Y)));
                viewerHolder.Top = Height / 2 - viewerHolder.Height / 2; 
            }
            else
            {
                viewerHolder.Width = (int)Math.Round(Width * ((Height / pageSize.Y) / (Width / pageSize.X)));
                viewerHolder.Left = Width / 2 - viewerHolder.Width / 2;
                viewerHolder.Top = 0;
                viewerHolder.Height = Height;
            }
        }
    }
}
