namespace Workbook
{
    partial class ExportForm
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
            this.rbPDF = new System.Windows.Forms.RadioButton();
            this.rbImage = new System.Windows.Forms.RadioButton();
            this.rbAllPages = new System.Windows.Forms.RadioButton();
            this.rbSelectPages = new System.Windows.Forms.RadioButton();
            this.lbPageSelect = new System.Windows.Forms.ListBox();
            this.gbExportFormat = new System.Windows.Forms.GroupBox();
            this.gbPages = new System.Windows.Forms.GroupBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbCurrent = new System.Windows.Forms.RadioButton();
            this.gbExportFormat.SuspendLayout();
            this.gbPages.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbPDF
            // 
            this.rbPDF.AutoSize = true;
            this.rbPDF.Location = new System.Drawing.Point(6, 19);
            this.rbPDF.Name = "rbPDF";
            this.rbPDF.Size = new System.Drawing.Size(46, 17);
            this.rbPDF.TabIndex = 0;
            this.rbPDF.TabStop = true;
            this.rbPDF.Text = "PDF";
            this.rbPDF.UseVisualStyleBackColor = true;
            // 
            // rbImage
            // 
            this.rbImage.AutoSize = true;
            this.rbImage.Location = new System.Drawing.Point(97, 19);
            this.rbImage.Name = "rbImage";
            this.rbImage.Size = new System.Drawing.Size(59, 17);
            this.rbImage.TabIndex = 1;
            this.rbImage.TabStop = true;
            this.rbImage.Text = "Images";
            this.rbImage.UseVisualStyleBackColor = true;
            // 
            // rbAllPages
            // 
            this.rbAllPages.AutoSize = true;
            this.rbAllPages.Location = new System.Drawing.Point(6, 42);
            this.rbAllPages.Name = "rbAllPages";
            this.rbAllPages.Size = new System.Drawing.Size(36, 17);
            this.rbAllPages.TabIndex = 2;
            this.rbAllPages.TabStop = true;
            this.rbAllPages.Text = "All";
            this.rbAllPages.UseVisualStyleBackColor = true;
            // 
            // rbSelectPages
            // 
            this.rbSelectPages.AutoSize = true;
            this.rbSelectPages.Location = new System.Drawing.Point(97, 19);
            this.rbSelectPages.Name = "rbSelectPages";
            this.rbSelectPages.Size = new System.Drawing.Size(58, 17);
            this.rbSelectPages.TabIndex = 3;
            this.rbSelectPages.TabStop = true;
            this.rbSelectPages.Text = "Select:";
            this.rbSelectPages.UseVisualStyleBackColor = true;
            this.rbSelectPages.CheckedChanged += new System.EventHandler(this.rbSelectPages_CheckedChanged);
            // 
            // lbPageSelect
            // 
            this.lbPageSelect.FormattingEnabled = true;
            this.lbPageSelect.Location = new System.Drawing.Point(97, 42);
            this.lbPageSelect.Name = "lbPageSelect";
            this.lbPageSelect.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbPageSelect.Size = new System.Drawing.Size(120, 56);
            this.lbPageSelect.TabIndex = 4;
            // 
            // gbExportFormat
            // 
            this.gbExportFormat.Controls.Add(this.rbPDF);
            this.gbExportFormat.Controls.Add(this.rbImage);
            this.gbExportFormat.Location = new System.Drawing.Point(12, 12);
            this.gbExportFormat.Name = "gbExportFormat";
            this.gbExportFormat.Size = new System.Drawing.Size(231, 46);
            this.gbExportFormat.TabIndex = 5;
            this.gbExportFormat.TabStop = false;
            this.gbExportFormat.Text = "Format";
            // 
            // gbPages
            // 
            this.gbPages.Controls.Add(this.rbCurrent);
            this.gbPages.Controls.Add(this.rbAllPages);
            this.gbPages.Controls.Add(this.rbSelectPages);
            this.gbPages.Controls.Add(this.lbPageSelect);
            this.gbPages.Location = new System.Drawing.Point(12, 64);
            this.gbPages.Name = "gbPages";
            this.gbPages.Size = new System.Drawing.Size(231, 108);
            this.gbPages.TabIndex = 6;
            this.gbPages.TabStop = false;
            this.gbPages.Text = "Pages";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(87, 178);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(168, 178);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rbCurrent
            // 
            this.rbCurrent.AutoSize = true;
            this.rbCurrent.Location = new System.Drawing.Point(6, 19);
            this.rbCurrent.Name = "rbCurrent";
            this.rbCurrent.Size = new System.Drawing.Size(59, 17);
            this.rbCurrent.TabIndex = 5;
            this.rbCurrent.TabStop = true;
            this.rbCurrent.Text = "Current";
            this.rbCurrent.UseVisualStyleBackColor = true;
            // 
            // ExportForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(255, 209);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbPages);
            this.Controls.Add(this.gbExportFormat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export";
            this.gbExportFormat.ResumeLayout(false);
            this.gbExportFormat.PerformLayout();
            this.gbPages.ResumeLayout(false);
            this.gbPages.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbPDF;
        private System.Windows.Forms.RadioButton rbImage;
        private System.Windows.Forms.RadioButton rbAllPages;
        private System.Windows.Forms.RadioButton rbSelectPages;
        private System.Windows.Forms.ListBox lbPageSelect;
        private System.Windows.Forms.GroupBox gbExportFormat;
        private System.Windows.Forms.GroupBox gbPages;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbCurrent;
    }
}