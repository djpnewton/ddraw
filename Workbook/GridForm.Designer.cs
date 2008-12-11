namespace Workbook
{
    partial class GridForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.nudSize = new System.Windows.Forms.NumericUpDown();
            this.lbGridSize = new System.Windows.Forms.Label();
            this.cbShow = new System.Windows.Forms.CheckBox();
            this.cbSnapPosition = new System.Windows.Forms.CheckBox();
            this.cbSnapResize = new System.Windows.Forms.CheckBox();
            this.cbSnapLines = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(58, 127);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(139, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // nudSize
            // 
            this.nudSize.Location = new System.Drawing.Point(67, 7);
            this.nudSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudSize.Name = "nudSize";
            this.nudSize.Size = new System.Drawing.Size(71, 20);
            this.nudSize.TabIndex = 6;
            this.nudSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lbGridSize
            // 
            this.lbGridSize.AutoSize = true;
            this.lbGridSize.Location = new System.Drawing.Point(12, 9);
            this.lbGridSize.Name = "lbGridSize";
            this.lbGridSize.Size = new System.Drawing.Size(49, 13);
            this.lbGridSize.TabIndex = 7;
            this.lbGridSize.Text = "Grid Size";
            // 
            // cbShow
            // 
            this.cbShow.AutoSize = true;
            this.cbShow.Location = new System.Drawing.Point(15, 33);
            this.cbShow.Name = "cbShow";
            this.cbShow.Size = new System.Drawing.Size(75, 17);
            this.cbShow.TabIndex = 8;
            this.cbShow.Text = "Show Grid";
            this.cbShow.UseVisualStyleBackColor = true;
            // 
            // cbSnapPosition
            // 
            this.cbSnapPosition.AutoSize = true;
            this.cbSnapPosition.Location = new System.Drawing.Point(15, 56);
            this.cbSnapPosition.Name = "cbSnapPosition";
            this.cbSnapPosition.Size = new System.Drawing.Size(91, 17);
            this.cbSnapPosition.TabIndex = 9;
            this.cbSnapPosition.Text = "Snap Position";
            this.cbSnapPosition.UseVisualStyleBackColor = true;
            // 
            // cbSnapResize
            // 
            this.cbSnapResize.AutoSize = true;
            this.cbSnapResize.Location = new System.Drawing.Point(15, 79);
            this.cbSnapResize.Name = "cbSnapResize";
            this.cbSnapResize.Size = new System.Drawing.Size(86, 17);
            this.cbSnapResize.TabIndex = 10;
            this.cbSnapResize.Text = "Snap Resize";
            this.cbSnapResize.UseVisualStyleBackColor = true;
            // 
            // cbSnapLines
            // 
            this.cbSnapLines.AutoSize = true;
            this.cbSnapLines.Location = new System.Drawing.Point(15, 102);
            this.cbSnapLines.Name = "cbSnapLines";
            this.cbSnapLines.Size = new System.Drawing.Size(79, 17);
            this.cbSnapLines.TabIndex = 11;
            this.cbSnapLines.Text = "Snap Lines";
            this.cbSnapLines.UseVisualStyleBackColor = true;
            // 
            // GridForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(226, 162);
            this.Controls.Add(this.cbSnapLines);
            this.Controls.Add(this.cbSnapResize);
            this.Controls.Add(this.cbSnapPosition);
            this.Controls.Add(this.cbShow);
            this.Controls.Add(this.lbGridSize);
            this.Controls.Add(this.nudSize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grid";
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown nudSize;
        private System.Windows.Forms.Label lbGridSize;
        private System.Windows.Forms.CheckBox cbShow;
        private System.Windows.Forms.CheckBox cbSnapPosition;
        private System.Windows.Forms.CheckBox cbSnapResize;
        private System.Windows.Forms.CheckBox cbSnapLines;
    }
}