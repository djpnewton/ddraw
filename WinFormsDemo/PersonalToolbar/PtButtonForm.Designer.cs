namespace WinFormsDemo.PersonalToolbar
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
            this.pnlRunCmd.SuspendLayout();
            this.pnlShowDir.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(434, 4);
            this.panel1.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(370, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(289, 110);
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
            "Show Directory"});
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
            this.pnlRunCmd.Size = new System.Drawing.Size(433, 55);
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
            this.tbArgs.Size = new System.Drawing.Size(341, 20);
            this.tbArgs.TabIndex = 16;
            // 
            // tbRun
            // 
            this.tbRun.Location = new System.Drawing.Point(89, 3);
            this.tbRun.Name = "tbRun";
            this.tbRun.Size = new System.Drawing.Size(260, 20);
            this.tbRun.TabIndex = 15;
            // 
            // btnRunBrowse
            // 
            this.btnRunBrowse.Location = new System.Drawing.Point(355, 1);
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
            this.pnlShowDir.Size = new System.Drawing.Size(433, 55);
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
            this.tbDir.Size = new System.Drawing.Size(288, 20);
            this.tbDir.TabIndex = 13;
            // 
            // btnDirBrowse
            // 
            this.btnDirBrowse.Location = new System.Drawing.Point(355, 3);
            this.btnDirBrowse.Name = "btnDirBrowse";
            this.btnDirBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnDirBrowse.TabIndex = 12;
            this.btnDirBrowse.Text = "Browse";
            this.btnDirBrowse.UseVisualStyleBackColor = true;
            this.btnDirBrowse.Click += new System.EventHandler(this.btnDirBrowse_Click);
            // 
            // pnlCustomFigure
            // 
            this.pnlCustomFigure.Location = new System.Drawing.Point(12, 39);
            this.pnlCustomFigure.Name = "pnlCustomFigure";
            this.pnlCustomFigure.Size = new System.Drawing.Size(433, 55);
            this.pnlCustomFigure.TabIndex = 20;
            // 
            // PtButtonForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(457, 141);
            this.Controls.Add(this.pnlCustomFigure);
            this.Controls.Add(this.pnlShowDir);
            this.Controls.Add(this.pnlRunCmd);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PtButtonForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Personal Toolbutton";
            this.pnlRunCmd.ResumeLayout(false);
            this.pnlRunCmd.PerformLayout();
            this.pnlShowDir.ResumeLayout(false);
            this.pnlShowDir.PerformLayout();
            this.ResumeLayout(false);

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
    }
}