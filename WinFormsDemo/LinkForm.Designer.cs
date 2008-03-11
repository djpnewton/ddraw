namespace WinFormsDemo
{
    partial class LinkForm
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
            this.gbLinkType = new System.Windows.Forms.GroupBox();
            this.rbPage = new System.Windows.Forms.RadioButton();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbWebPage = new System.Windows.Forms.RadioButton();
            this.pnlWebPage = new System.Windows.Forms.Panel();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.lbAddress = new System.Windows.Forms.Label();
            this.pnlFile = new System.Windows.Forms.Panel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.lbFile = new System.Windows.Forms.Label();
            this.pnlPage = new System.Windows.Forms.Panel();
            this.lbPages = new System.Windows.Forms.ListBox();
            this.lbPage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.wfViewerControl1 = new DDraw.WinForms.WFViewerControl();
            this.gbLinkType.SuspendLayout();
            this.pnlWebPage.SuspendLayout();
            this.pnlFile.SuspendLayout();
            this.pnlPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbLinkType
            // 
            this.gbLinkType.Controls.Add(this.rbPage);
            this.gbLinkType.Controls.Add(this.rbFile);
            this.gbLinkType.Controls.Add(this.rbWebPage);
            this.gbLinkType.Location = new System.Drawing.Point(12, 12);
            this.gbLinkType.Name = "gbLinkType";
            this.gbLinkType.Size = new System.Drawing.Size(268, 90);
            this.gbLinkType.TabIndex = 0;
            this.gbLinkType.TabStop = false;
            this.gbLinkType.Text = "Link Type";
            // 
            // rbPage
            // 
            this.rbPage.AutoSize = true;
            this.rbPage.Location = new System.Drawing.Point(6, 65);
            this.rbPage.Name = "rbPage";
            this.rbPage.Size = new System.Drawing.Size(132, 17);
            this.rbPage.TabIndex = 3;
            this.rbPage.TabStop = true;
            this.rbPage.Text = "Page in this Document";
            this.rbPage.UseVisualStyleBackColor = true;
            this.rbPage.CheckedChanged += new System.EventHandler(this.LinkType_Changed);
            // 
            // rbFile
            // 
            this.rbFile.AutoSize = true;
            this.rbFile.Location = new System.Drawing.Point(6, 42);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(123, 17);
            this.rbFile.TabIndex = 2;
            this.rbFile.TabStop = true;
            this.rbFile.Text = "File on this Computer";
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.LinkType_Changed);
            // 
            // rbWebPage
            // 
            this.rbWebPage.AutoSize = true;
            this.rbWebPage.Location = new System.Drawing.Point(6, 19);
            this.rbWebPage.Name = "rbWebPage";
            this.rbWebPage.Size = new System.Drawing.Size(76, 17);
            this.rbWebPage.TabIndex = 1;
            this.rbWebPage.TabStop = true;
            this.rbWebPage.Text = "Web Page";
            this.rbWebPage.UseVisualStyleBackColor = true;
            this.rbWebPage.CheckedChanged += new System.EventHandler(this.LinkType_Changed);
            // 
            // pnlWebPage
            // 
            this.pnlWebPage.Controls.Add(this.tbAddress);
            this.pnlWebPage.Controls.Add(this.lbAddress);
            this.pnlWebPage.Location = new System.Drawing.Point(12, 108);
            this.pnlWebPage.Name = "pnlWebPage";
            this.pnlWebPage.Size = new System.Drawing.Size(268, 102);
            this.pnlWebPage.TabIndex = 1;
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(6, 23);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(259, 20);
            this.tbAddress.TabIndex = 1;
            // 
            // lbAddress
            // 
            this.lbAddress.AutoSize = true;
            this.lbAddress.Location = new System.Drawing.Point(3, 6);
            this.lbAddress.Name = "lbAddress";
            this.lbAddress.Size = new System.Drawing.Size(48, 13);
            this.lbAddress.TabIndex = 0;
            this.lbAddress.Text = "Address:";
            // 
            // pnlFile
            // 
            this.pnlFile.Controls.Add(this.btnBrowse);
            this.pnlFile.Controls.Add(this.tbFile);
            this.pnlFile.Controls.Add(this.lbFile);
            this.pnlFile.Location = new System.Drawing.Point(12, 108);
            this.pnlFile.Name = "pnlFile";
            this.pnlFile.Size = new System.Drawing.Size(268, 102);
            this.pnlFile.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(201, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(64, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbFile
            // 
            this.tbFile.Location = new System.Drawing.Point(6, 22);
            this.tbFile.Name = "tbFile";
            this.tbFile.Size = new System.Drawing.Size(189, 20);
            this.tbFile.TabIndex = 1;
            // 
            // lbFile
            // 
            this.lbFile.AutoSize = true;
            this.lbFile.Location = new System.Drawing.Point(3, 6);
            this.lbFile.Name = "lbFile";
            this.lbFile.Size = new System.Drawing.Size(26, 13);
            this.lbFile.TabIndex = 0;
            this.lbFile.Text = "File:";
            // 
            // pnlPage
            // 
            this.pnlPage.Controls.Add(this.wfViewerControl1);
            this.pnlPage.Controls.Add(this.lbPages);
            this.pnlPage.Controls.Add(this.lbPage);
            this.pnlPage.Location = new System.Drawing.Point(12, 108);
            this.pnlPage.Name = "pnlPage";
            this.pnlPage.Size = new System.Drawing.Size(268, 102);
            this.pnlPage.TabIndex = 2;
            // 
            // lbPages
            // 
            this.lbPages.FormattingEnabled = true;
            this.lbPages.Location = new System.Drawing.Point(6, 23);
            this.lbPages.Name = "lbPages";
            this.lbPages.Size = new System.Drawing.Size(126, 69);
            this.lbPages.TabIndex = 1;
            this.lbPages.SelectedIndexChanged += new System.EventHandler(this.lbPages_SelectedIndexChanged);
            // 
            // lbPage
            // 
            this.lbPage.AutoSize = true;
            this.lbPage.Location = new System.Drawing.Point(3, 6);
            this.lbPage.Name = "lbPage";
            this.lbPage.Size = new System.Drawing.Size(35, 13);
            this.lbPage.TabIndex = 0;
            this.lbPage.Text = "Page:";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 216);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 4);
            this.panel1.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(124, 226);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(205, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnClear.Location = new System.Drawing.Point(12, 226);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(82, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Remove Link";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // wfViewerControl1
            // 
            this.wfViewerControl1.Location = new System.Drawing.Point(138, 23);
            this.wfViewerControl1.Name = "wfViewerControl1";
            this.wfViewerControl1.Size = new System.Drawing.Size(126, 69);
            this.wfViewerControl1.TabIndex = 2;
            // 
            // LinkForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 258);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbLinkType);
            this.Controls.Add(this.pnlPage);
            this.Controls.Add(this.pnlWebPage);
            this.Controls.Add(this.pnlFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Figure Link";
            this.gbLinkType.ResumeLayout(false);
            this.gbLinkType.PerformLayout();
            this.pnlWebPage.ResumeLayout(false);
            this.pnlWebPage.PerformLayout();
            this.pnlFile.ResumeLayout(false);
            this.pnlFile.PerformLayout();
            this.pnlPage.ResumeLayout(false);
            this.pnlPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbLinkType;
        private System.Windows.Forms.RadioButton rbPage;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbWebPage;
        private System.Windows.Forms.Panel pnlWebPage;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label lbAddress;
        private System.Windows.Forms.Panel pnlFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.Label lbFile;
        private System.Windows.Forms.Panel pnlPage;
        private System.Windows.Forms.Label lbPage;
        private System.Windows.Forms.ListBox lbPages;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClear;
        private DDraw.WinForms.WFViewerControl wfViewerControl1;
    }
}