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
    public delegate void PreviewContextHandler(Preview p, Point pt);
    public delegate void PreviewMoveHandler(Preview p, Preview to);
    public delegate void PreviewFigureDropHandler(Preview p, List<Figure> figs);
    public delegate void PreviewNameChangedHandler(Preview p, string name);

    public class Preview : UserControl
    {
        public bool Selected
        {
            get { return viewerHolder.BackColor == Color.Red; }
            set
            {
                if (value)
                {
                    viewerHolder.BackColor = Color.Red;
                    // deselect other siblings
                    foreach (Control c in Parent.Controls)
                        if (c is Preview && c != this)
                            ((Preview)c).Selected = false;
                    // scroll into view
                    if (Parent is ScrollableControl)
                        ((ScrollableControl)Parent).ScrollControlIntoView(this);
                }
                else
                    viewerHolder.BackColor = Color.LightGray;
            }
        }

        public event PreviewContextHandler PreviewContext;
        public event PreviewMoveHandler PreviewMove;
        public event PreviewFigureDropHandler PreviewFigureDrop;
        public event PreviewNameChangedHandler PreviewNameChanged;

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
        Label label;
        PictureBox pbContext;

        const int labelFontSize = 7;
        const int labelHeight = 13;

        public Preview(DEngine de)
        {
            viewerHolder = new Panel();
            viewerHolder.Location = new Point(0, 0);
            viewerHolder.Size = new Size(Width, Height - labelHeight);
            Controls.Add(viewerHolder);
            // label
            label = new Label();
            label.Font = new Font(Font.FontFamily, labelFontSize);
            label.TextAlign = ContentAlignment.TopCenter;
            label.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            label.Location = new Point(MARGIN, Height - labelHeight - MARGIN * 2);
            label.Size = new Size(Width - MARGIN * 2, labelHeight);
            label.DoubleClick += new EventHandler(label_DoubleClick);
            Controls.Add(label);
            // viewerControl
            viewerControl = new WFViewerControl();
            viewerControl.Location = new Point(MARGIN, MARGIN);
            viewerControl.Size = new Size(viewerHolder.Width - MARGIN * 2, viewerHolder.Height - MARGIN * 2);
            viewerControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            viewerControl.MouseDown += new MouseEventHandler(viewerControl_MouseDown);
            viewerControl.MouseUp += new MouseEventHandler(viewerControl_MouseUp);
            viewerHolder.Controls.Add(viewerControl);
            // pbContext
            pbContext = new PictureBox();
            pbContext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbContext.Image = global::Workbook.Resource1.context;
            pbContext.BackColor = Color.Transparent;
            pbContext.Size = new Size(pbContext.Image.Width, pbContext.Image.Height);
            pbContext.Location = new Point(Width - pbContext.Image.Width - 2, 0);
            pbContext.Click += new EventHandler(pbContext_Click);
            viewerControl.Controls.Add(pbContext);
            // Preview
            AllowDrop = true;
            DragEnter += new DragEventHandler(Preview_DragEnter);
            DragDrop += new DragEventHandler(Preview_DragDrop);
            SizeChanged += new EventHandler(Preview_SizeChanged);
            // DEngine
            this.de = de;
            dv = new WFViewer(viewerControl);
            dv.Preview = true;
            de.PageSizeChanged += new PageSizeChangedHandler(de_PageSizeChanged);
            de.AddViewer(dv);
            UpdateScale();
            UpdateName();
        }

        void label_DoubleClick(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
            Rename();
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
            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move && 
                (e.Data.GetDataPresent(this.GetType()) || e.Data.GetDataPresent(typeof(List<Figure>))))
                e.Effect = DragDropEffects.Move;
        }

        void Preview_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(this.GetType()))
            {
                Preview pDrag = (Preview)e.Data.GetData(this.GetType());
                if (pDrag != this && PreviewMove != null)
                    PreviewMove(pDrag, this);
            }
            else if (e.Data.GetDataPresent(typeof(List<Figure>)))
            {
                if (PreviewFigureDrop != null)
                    PreviewFigureDrop(this, (List<Figure>)e.Data.GetData(typeof(List<Figure>)));
            }
        }

        void Preview_SizeChanged(object sender, EventArgs e)
        {
            UpdateScale();
        }

        void UpdateScale()
        {
            int h = Height - labelHeight;
            if (de.PageSize.X / Width > de.PageSize.Y / h)
            {
                viewerHolder.Left = 0;
                viewerHolder.Width = Width;
                viewerHolder.Height = (int)Math.Round(h * ((Width / de.PageSize.X) / (Height / de.PageSize.Y)));
                viewerHolder.Top = h / 2 - viewerHolder.Height / 2;
            }
            else
            {
                viewerHolder.Width = (int)Math.Round(Width * ((h / de.PageSize.Y) / (Width / de.PageSize.X)));
                viewerHolder.Left = Width / 2 - viewerHolder.Width / 2;
                viewerHolder.Top = 0;
                viewerHolder.Height = h;
            }
            label.Top = viewerHolder.Bottom - 2;
        }

        private void de_PageSizeChanged(DEngine de, DPoint pageSize)
        {
            UpdateScale();
        }

        public void Rename()
        {
            Point p = viewerControl.PointToScreen(new Point(0, 0));
            TextPopup f = new TextPopup(p.X, p.Y);
            f.Text = label.Text;
            f.FormClosed += new FormClosedEventHandler(textPopup_FormClosed);
            f.Show();
        }

        void textPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            label.Text = ((TextPopup)sender).Text;
            if (PreviewNameChanged != null)
                PreviewNameChanged(this, label.Text);
        }

        public void UpdateName()
        {
            label.Text = de.PageName;
        }
    }
}
