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
            get { return BackColor == Color.Red; }
            set
            {
                if (value)
                {
                    BackColor = Color.Red;
                    // deselect other siblings
                    if (radioSelect)
                        foreach (Control c in Parent.Controls)
                            if (c is Preview && c != this)
                                ((Preview)c).Selected = false;
                }
                else
                    BackColor = Color.Empty;
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

        public Preview(DEngine de)
        {
            viewerControl = new WFViewerControl();
            SuspendLayout();
            // 
            // wfViewerControl
            // 
            viewerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            viewerControl.Location = new System.Drawing.Point(3, 3);
            viewerControl.Name = "wfViewerControl";
            viewerControl.Size = new System.Drawing.Size(144, 144);
            viewerControl.Click += new System.EventHandler(viewerControl_Click);
            // 
            // Preview
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(viewerControl);
            Name = "Preview";
            ResumeLayout(false);

            this.de = de;
            dv = new WFViewer(viewerControl);
            dv.Preview = true;
            de.AddViewer(dv);
        }

        private void viewerControl_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
        }
    }
}
