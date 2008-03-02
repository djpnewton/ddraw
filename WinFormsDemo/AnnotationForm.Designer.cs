namespace WinFormsDemo
{
    partial class AnnotationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wfViewerControl1 = new DDraw.WinForms.WFViewerControl();
            this.SuspendLayout();
            // 
            // wfViewerControl1
            // 
            this.wfViewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wfViewerControl1.Location = new System.Drawing.Point(0, 0);
            this.wfViewerControl1.Name = "wfViewerControl1";
            this.wfViewerControl1.Size = new System.Drawing.Size(292, 266);
            this.wfViewerControl1.TabIndex = 0;
            this.wfViewerControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.wfViewerControl1_KeyUp);
            this.wfViewerControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.wfViewerControl1_KeyDown);
            // 
            // AnnotationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.wfViewerControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AnnotationForm";
            this.Text = "TransparentForm";
            this.ResumeLayout(false);

        }

        #endregion

        private DDraw.WinForms.WFViewerControl wfViewerControl1;
    }
}