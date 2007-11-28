namespace WinFormsDemo
{
    partial class CustomPageSizeForm
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
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbHeight = new System.Windows.Forms.Label();
            this.nudWidthMM = new System.Windows.Forms.NumericUpDown();
            this.nudHeightMM = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidthMM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeightMM)).BeginInit();
            this.SuspendLayout();
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Location = new System.Drawing.Point(12, 9);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(60, 13);
            this.lbWidth.TabIndex = 0;
            this.lbWidth.Text = "Width (mm)";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Location = new System.Drawing.Point(12, 37);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(63, 13);
            this.lbHeight.TabIndex = 1;
            this.lbHeight.Text = "Height (mm)";
            // 
            // nudWidthMM
            // 
            this.nudWidthMM.Location = new System.Drawing.Point(81, 7);
            this.nudWidthMM.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidthMM.Name = "nudWidthMM";
            this.nudWidthMM.Size = new System.Drawing.Size(90, 20);
            this.nudWidthMM.TabIndex = 2;
            // 
            // nudHeightMM
            // 
            this.nudHeightMM.Location = new System.Drawing.Point(81, 35);
            this.nudHeightMM.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeightMM.Name = "nudHeightMM";
            this.nudHeightMM.Size = new System.Drawing.Size(90, 20);
            this.nudHeightMM.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(15, 61);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(96, 61);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // CustomPageSizeForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(184, 95);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.nudHeightMM);
            this.Controls.Add(this.nudWidthMM);
            this.Controls.Add(this.lbHeight);
            this.Controls.Add(this.lbWidth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomPageSizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Page Size";
            ((System.ComponentModel.ISupportInitialize)(this.nudWidthMM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeightMM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbHeight;
        private System.Windows.Forms.NumericUpDown nudWidthMM;
        private System.Windows.Forms.NumericUpDown nudHeightMM;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}