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
                }
                else
                    viewerHolder.BackColor = Color.Empty;
            }
        }

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
        DViewer dv;
        public DViewer DViewer
        {
            get { return dv; }
        }
        
        const int MARGIN = 1;
        Panel viewerHolder;

        public Preview(DEngine de)
        {
            viewerHolder = new Panel();
            viewerHolder.Location = new Point(0, 0);
            viewerHolder.Size = Size;
            viewerHolder.Click += new EventHandler(viewerControl_Click);
            Controls.Add(viewerHolder);
            // viewerControl
            viewerControl = new WFViewerControl();
            viewerControl.Location = new Point(MARGIN, MARGIN);
            viewerControl.Size = new Size(Width - MARGIN * 2, Height - MARGIN * 2);
            viewerControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            viewerControl.Click += new EventHandler(viewerControl_Click);
            viewerHolder.Controls.Add(viewerControl);
            // Preview
            this.de = de;
            dv = new WFViewer(viewerControl);
            dv.Preview = true;
            de.PageSizeChanged += new PageSizeChangedHandler(de_PageSizeChanged);
            de.AddViewer(dv);
        }

        private void viewerControl_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
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
