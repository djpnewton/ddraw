namespace Workbook.PersonalToolbar
{
    partial class PtButtonForm
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
            this.cbType = new System.Windows.Forms.ComboBox();
            this.pnlRunCmd = new System.Windows.Forms.Panel();
            this.lbArgs = new System.Windows.Forms.Label();
            this.lbRunCmd = new System.Windows.Forms.Label();
            this.tbArgs = new System.Windows.Forms.TextBox();
            this.tbRun = new System.Windows.Forms.TextBox();
            this.btnRunBrowse = new System.Windows.Forms.Button();
            this.pnlShowDir = new System.Windows.Forms.Panel();
            this.lbDir = new System.Windows.Forms.Label();
            this.tbDir = new System.Windows.Forms.TextBox();
            this.btnDirBrowse = new System.Windows.Forms.Button();
            this.pnlCustomFigure = new System.Windows.Forms.Panel();
            this.vcCustomFigure = new DDraw.WinForms.WFViewerControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStripPanel1 = new System.Windows.Forms.ToolStripPanel();
            this.tsCustomFigureType = new Workbook.FigureToolStrip();
            this.tsCustomFigureProps = new Workbook.FigurePropertiesToolStrip();
            this.pnlWebLink = new System.Windows.Forms.Panel();
            this.lbWebLink = new System.Windows.Forms.Label();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.cbToolEditAddToPersonal = new System.Windows.Forms.CheckBox();
            this.pnlRunCmd.SuspendLayout();
            this.pnlShowDir.SuspendLayout();
            this.pnlCustomFigure.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStripPanel1.SuspendLayout();
            this.pnlWebLink.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 4);
            this.panel1.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(401, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(320, 110);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "Custom Figure",
            "Run Command",
            "Show Directory",
            "Web Link"});
            this.cbType.Location = new System.Drawing.Point(12, 12);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(121, 21);
            this.cbType.TabIndex = 10;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // pnlRunCmd
            // 
            this.pnlRunCmd.Controls.Add(this.lbArgs);
            this.pnlRunCmd.Controls.Add(this.lbRunCmd);
            this.pnlRunCmd.Controls.Add(this.tbArgs);
            this.pnlRunCmd.Controls.Add(this.tbRun);
            this.pnlRunCmd.Controls.Add(this.btnRunBrowse);
            this.pnlRunCmd.Location = new System.Drawing.Point(12, 39);
            this.pnlRunCmd.Name = "pnlRunCmd";
            this.pnlRunCmd.Size = new System.Drawing.Size(463, 55);
            this.pnlRunCmd.TabIndex = 11;
            // 
            // lbArgs
            // 
            this.lbArgs.AutoSize = true;
            this.lbArgs.Location = new System.Drawing.Point(3, 32);
            this.lbArgs.Name = "lbArgs";
            this.lbArgs.Size = new System.Drawing.Size(60, 13);
            this.lbArgs.TabIndex = 18;
            this.lbArgs.Text = "Arguments:";
            // 
            // lbRunCmd
            // 
            this.lbRunCmd.AutoSize = true;
            this.lbRunCmd.Location = new System.Drawing.Point(3, 6);
            this.lbRunCmd.Name = "lbRunCmd";
            this.lbRunCmd.Size = new System.Drawing.Size(80, 13);
            this.lbRunCmd.TabIndex = 17;
            this.lbRunCmd.Text = "Run Command:";
            // 
            // tbArgs
            // 
            this.tbArgs.Location = new System.Drawing.Point(89, 29);
            this.tbArgs.Name = "tbArgs";
            this.tbArgs.Size = new System.Drawing.Size(371, 20);
            this.tbArgs.TabIndex = 16;
            // 
            // tbRun
            // 
            this.tbRun.Location = new System.Drawing.Point(89, 3);
            this.tbRun.Name = "tbRun";
            this.tbRun.Size = new System.Drawing.Size(294, 20);
            this.tbRun.TabIndex = 15;
            // 
            // btnRunBrowse
            // 
            this.btnRunBrowse.Location = new System.Drawing.Point(385, 1);
            this.btnRunBrowse.Name = "btnRunBrowse";
            this.btnRunBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnRunBrowse.TabIndex = 14;
            this.btnRunBrowse.Text = "Browse";
            this.btnRunBrowse.UseVisualStyleBackColor = true;
            this.btnRunBrowse.Click += new System.EventHandler(this.btnRunBrowse_Click);
            // 
            // pnlShowDir
            // 
            this.pnlShowDir.Controls.Add(this.lbDir);
            this.pnlShowDir.Controls.Add(this.tbDir);
            this.pnlShowDir.Controls.Add(this.btnDirBrowse);
            this.pnlShowDir.Location = new System.Drawing.Point(12, 39);
            this.pnlShowDir.Name = "pnlShowDir";
            this.pnlShowDir.Size = new System.Drawing.Size(463, 55);
            this.pnlShowDir.TabIndex = 12;
            // 
            // lbDir
            // 
            this.lbDir.AutoSize = true;
            this.lbDir.Location = new System.Drawing.Point(3, 8);
            this.lbDir.Name = "lbDir";
            this.lbDir.Size = new System.Drawing.Size(52, 13);
            this.lbDir.TabIndex = 19;
            this.lbDir.Text = "Directory:";
            // 
            // tbDir
            // 
            this.tbDir.Location = new System.Drawing.Point(61, 5);
            this.tbDir.Name = "tbDir";
            this.tbDir.Size = new System.Drawing.Size(322, 20);
            this.tbDir.TabIndex = 13;
            // 
            // btnDirBrowse
            // 
            this.btnDirBrowse.Location = new System.Drawing.Point(385, 3);
            this.btnDirBrowse.Name = "btnDirBrowse";
            this.btnDirBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnDirBrowse.TabIndex = 12;
            this.btnDirBrowse.Text = "Browse";
            this.btnDirBrowse.UseVisualStyleBackColor = true;
            this.btnDirBrowse.Click += new System.EventHandler(this.btnDirBrowse_Click);
            // 
            // pnlCustomFigure
            // 
            this.pnlCustomFigure.Controls.Add(this.vcCustomFigure);
            this.pnlCustomFigure.Controls.Add(this.panel2);
            this.pnlCustomFigure.Location = new System.Drawing.Point(12, 39);
            this.pnlCustomFigure.Name = "pnlCustomFigure";
            this.pnlCustomFigure.Size = new System.Drawing.Size(463, 55);
            this.pnlCustomFigure.TabIndex = 20;
            // 
            // vcCustomFigure
            // 
            this.vcCustomFigure.Dock = System.Windows.Forms.DockStyle.Right;
            this.vcCustomFigure.Location = new System.Drawing.Point(408, 0);
            this.vcCustomFigure.Name = "vcCustomFigure";
            this.vcCustomFigure.Size = new System.Drawing.Size(55, 55);
            this.vcCustomFigure.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.toolStripPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(463, 55);
            this.panel2.TabIndex = 20;
            // 
            // toolStripPanel1
            // 
            this.toolStripPanel1.Controls.Add(this.tsCustomFigureType);
            this.toolStripPanel1.Controls.Add(this.tsCustomFigureProps);
            this.toolStripPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.toolStripPanel1.Name = "toolStripPanel1";
            this.toolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolStripPanel1.Size = new System.Drawing.Size(463, 55);
            // 
            // tsCustomFigureType
            // 
            this.tsCustomFigureType.ClickThrough = false;
            this.tsCustomFigureType.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCustomFigureType.FigureClass = null;
            this.tsCustomFigureType.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCustomFigureType.Location = new System.Drawing.Point(3, 0);
            this.tsCustomFigureType.Mode = Workbook.FigureToolStripMode.FigureClassSelect;
            this.tsCustomFigureType.Name = "tsCustomFigureType";
            this.tsCustomFigureType.Size = new System.Drawing.Size(233, 25);
            this.tsCustomFigureType.TabIndex = 1;
            this.tsCustomFigureType.FigureClassChanged += new Workbook.FigureClassChangedHandler(this.tsCustomFigureType_FigureClassChanged);
            // 
            // tsCustomFigureProps
            // 
            this.tsCustomFigureProps.ClickThrough = false;
            this.tsCustomFigureProps.Dap = null;
            this.tsCustomFigureProps.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCustomFigureProps.FigureClass = null;
            this.tsCustomFigureProps.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCustomFigureProps.Location = new System.Drawing.Point(3, 25);
            this.tsCustomFigureProps.Name = "tsCustomFigureProps";
            this.tsCustomFigureProps.Size = new System.Drawing.Size(264, 25);
            this.tsCustomFigureProps.TabIndex = 0;
            // 
            // pnlWebLink
            // 
            this.pnlWebLink.Controls.Add(this.lbWebLink);
            this.pnlWebLink.Controls.Add(this.tbUrl);
            this.pnlWebLink.Location = new System.Drawing.Point(12, 39);
            this.pnlWebLink.Name = "pnlWebLink";
            this.pnlWebLink.Size = new System.Drawing.Size(463, 55);
            this.pnlWebLink.TabIndex = 20;
            // 
            // lbWebLink
            // 
            this.lbWebLink.AutoSize = true;
            this.lbWebLink.Location = new System.Drawing.Point(3, 8);
            this.lbWebLink.Name = "lbWebLink";
            this.lbWebLink.Size = new System.Drawing.Size(30, 13);
            this.lbWebLink.TabIndex = 19;
            this.lbWebLink.Text = "Link:";
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(39, 5);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(421, 20);
            this.tbUrl.TabIndex = 13;
            // 
            // cbToolEditAddToPersonal
            // 
            this.cbToolEditAddToPersonal.AutoSize = true;
            this.cbToolEditAddToPersonal.Location = new System.Drawing.Point(12, 14);
            this.cbToolEditAddToPersonal.Name = "cbToolEditAddToPersonal";
            this.cbToolEditAddToPersonal.Size = new System.Drawing.Size(140, 17);
            this.cbToolEditAddToPersonal.TabIndex = 21;
            this.cbToolEditAddToPersonal.Text = "Add to Personal Toolbar";
            this.cbToolEditAddToPersonal.UseVisualStyleBackColor = true;
            // 
            // PtButtonForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(487, 141);
            this.Controls.Add(this.cbToolEditAddToPersonal);
            this.Controls.Add(this.pnlCustomFigure);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pnlRunCmd);
            this.Controls.Add(this.pnlShowDir);
            this.Controls.Add(this.pnlWebLink);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PtButtonForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PtButtonForm";
            this.pnlRunCmd.ResumeLayout(false);
            this.pnlRunCmd.PerformLayout();
            this.pnlShowDir.ResumeLayout(false);
            this.pnlShowDir.PerformLayout();
            this.pnlCustomFigure.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStripPanel1.ResumeLayout(false);
            this.toolStripPanel1.PerformLayout();
            this.pnlWebLink.ResumeLayout(false);
            this.pnlWebLink.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Panel pnlRunCmd;
        private System.Windows.Forms.TextBox tbRun;
        private System.Windows.Forms.Button btnRunBrowse;
        private System.Windows.Forms.Panel pnlShowDir;
        private System.Windows.Forms.TextBox tbDir;
        private System.Windows.Forms.Button btnDirBrowse;
        private System.Windows.Forms.Label lbArgs;
        private System.Windows.Forms.Label lbRunCmd;
        private System.Windows.Forms.TextBox tbArgs;
        private System.Windows.Forms.Label lbDir;
        private System.Windows.Forms.Panel pnlCustomFigure;
        private System.Windows.Forms.ToolStripPanel toolStripPanel1;
        private FigurePropertiesToolStrip tsCustomFigureProps;
        private FigureToolStrip tsCustomFigureType;
        private DDraw.WinForms.WFViewerControl vcCustomFigure;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlWebLink;
        private System.Windows.Forms.Label lbWebLink;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.CheckBox cbToolEditAddToPersonal;
    }
}