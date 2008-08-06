namespace Workbook
{
    partial class PropertiesForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.vc = new DDraw.WinForms.WFViewerControl();
            this.tsFigureProps = new Workbook.FigurePropertiesToolStrip();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(6, 228);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(290, 4);
            this.panel1.TabIndex = 12;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 238);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(134, 238);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // vc
            // 
            this.vc.Location = new System.Drawing.Point(9, 37);
            this.vc.Name = "vc";
            this.vc.Size = new System.Drawing.Size(284, 185);
            this.vc.TabIndex = 22;
            // 
            // tsFigureProps
            // 
            this.tsFigureProps.ClickThrough = false;
            this.tsFigureProps.Dap = null;
            this.tsFigureProps.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFigureProps.FigureClass = null;
            this.tsFigureProps.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsFigureProps.Location = new System.Drawing.Point(9, 9);
            this.tsFigureProps.Name = "tsFigureProps";
            this.tsFigureProps.Size = new System.Drawing.Size(284, 25);
            this.tsFigureProps.TabIndex = 0;
            this.tsFigureProps.UseDecentToolTips = false;
            // 
            // PropertiesForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(302, 269);
            this.Controls.Add(this.vc);
            this.Controls.Add(this.tsFigureProps);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Figure Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private DDraw.WinForms.WFViewerControl vc;
        private FigurePropertiesToolStrip tsFigureProps;
    }
}