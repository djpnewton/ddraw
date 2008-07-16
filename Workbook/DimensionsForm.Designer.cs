namespace Workbook
{
    partial class DimensionsForm
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
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.lbX = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbHeight = new System.Windows.Forms.Label();
            this.numRotation = new System.Windows.Forms.NumericUpDown();
            this.lbRotation = new System.Windows.Forms.Label();
            this.cbGroupRot = new System.Windows.Forms.CheckBox();
            this.cbGroupWidth = new System.Windows.Forms.CheckBox();
            this.cbGroupHeight = new System.Windows.Forms.CheckBox();
            this.cbGroupY = new System.Windows.Forms.CheckBox();
            this.cbGroupX = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbLockAspect = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRotation)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(62, 152);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(143, 152);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // numX
            // 
            this.numX.DecimalPlaces = 4;
            this.numX.Location = new System.Drawing.Point(60, 12);
            this.numX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(72, 20);
            this.numX.TabIndex = 2;
            // 
            // numY
            // 
            this.numY.DecimalPlaces = 4;
            this.numY.Location = new System.Drawing.Point(60, 38);
            this.numY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(72, 20);
            this.numY.TabIndex = 3;
            // 
            // numWidth
            // 
            this.numWidth.DecimalPlaces = 4;
            this.numWidth.Location = new System.Drawing.Point(60, 64);
            this.numWidth.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numWidth.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(72, 20);
            this.numWidth.TabIndex = 4;
            this.numWidth.Validating += new System.ComponentModel.CancelEventHandler(this.numWidth_Validating);
            // 
            // numHeight
            // 
            this.numHeight.DecimalPlaces = 4;
            this.numHeight.Location = new System.Drawing.Point(60, 90);
            this.numHeight.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numHeight.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(72, 20);
            this.numHeight.TabIndex = 5;
            this.numHeight.Validating += new System.ComponentModel.CancelEventHandler(this.numHeight_Validating);
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(10, 14);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(25, 13);
            this.lbX.TabIndex = 6;
            this.lbX.Text = "Left";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(10, 40);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(26, 13);
            this.lbY.TabIndex = 7;
            this.lbY.Text = "Top";
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Location = new System.Drawing.Point(10, 66);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(35, 13);
            this.lbWidth.TabIndex = 8;
            this.lbWidth.Text = "Width";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Location = new System.Drawing.Point(10, 92);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(38, 13);
            this.lbHeight.TabIndex = 9;
            this.lbHeight.Text = "Height";
            // 
            // numRotation
            // 
            this.numRotation.DecimalPlaces = 4;
            this.numRotation.Location = new System.Drawing.Point(60, 116);
            this.numRotation.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numRotation.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numRotation.Name = "numRotation";
            this.numRotation.Size = new System.Drawing.Size(72, 20);
            this.numRotation.TabIndex = 10;
            // 
            // lbRotation
            // 
            this.lbRotation.AutoSize = true;
            this.lbRotation.Location = new System.Drawing.Point(7, 118);
            this.lbRotation.Name = "lbRotation";
            this.lbRotation.Size = new System.Drawing.Size(47, 13);
            this.lbRotation.TabIndex = 11;
            this.lbRotation.Text = "Angle (°)";
            // 
            // cbGroupRot
            // 
            this.cbGroupRot.AutoSize = true;
            this.cbGroupRot.Location = new System.Drawing.Point(138, 117);
            this.cbGroupRot.Name = "cbGroupRot";
            this.cbGroupRot.Size = new System.Drawing.Size(79, 17);
            this.cbGroupRot.TabIndex = 16;
            this.cbGroupRot.Text = "Unify angle";
            this.cbGroupRot.UseVisualStyleBackColor = true;
            this.cbGroupRot.CheckedChanged += new System.EventHandler(this.cbGroupRot_CheckedChanged);
            // 
            // cbGroupWidth
            // 
            this.cbGroupWidth.AutoSize = true;
            this.cbGroupWidth.Location = new System.Drawing.Point(138, 65);
            this.cbGroupWidth.Name = "cbGroupWidth";
            this.cbGroupWidth.Size = new System.Drawing.Size(81, 17);
            this.cbGroupWidth.TabIndex = 17;
            this.cbGroupWidth.Text = "Unify Width";
            this.cbGroupWidth.UseVisualStyleBackColor = true;
            this.cbGroupWidth.CheckedChanged += new System.EventHandler(this.cbGroupWidth_CheckedChanged);
            // 
            // cbGroupHeight
            // 
            this.cbGroupHeight.AutoSize = true;
            this.cbGroupHeight.Location = new System.Drawing.Point(138, 91);
            this.cbGroupHeight.Name = "cbGroupHeight";
            this.cbGroupHeight.Size = new System.Drawing.Size(84, 17);
            this.cbGroupHeight.TabIndex = 18;
            this.cbGroupHeight.Text = "Unify Height";
            this.cbGroupHeight.UseVisualStyleBackColor = true;
            this.cbGroupHeight.CheckedChanged += new System.EventHandler(this.cbGroupHeight_CheckedChanged);
            // 
            // cbGroupY
            // 
            this.cbGroupY.AutoSize = true;
            this.cbGroupY.Location = new System.Drawing.Point(138, 39);
            this.cbGroupY.Name = "cbGroupY";
            this.cbGroupY.Size = new System.Drawing.Size(72, 17);
            this.cbGroupY.TabIndex = 19;
            this.cbGroupY.Text = "Unify Top";
            this.cbGroupY.UseVisualStyleBackColor = true;
            // 
            // cbGroupX
            // 
            this.cbGroupX.AutoSize = true;
            this.cbGroupX.Location = new System.Drawing.Point(138, 13);
            this.cbGroupX.Name = "cbGroupX";
            this.cbGroupX.Size = new System.Drawing.Size(71, 17);
            this.cbGroupX.TabIndex = 20;
            this.cbGroupX.Text = "Unify Left";
            this.cbGroupX.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 142);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 4);
            this.panel1.TabIndex = 21;
            // 
            // cbLockAspect
            // 
            this.cbLockAspect.AutoSize = true;
            this.cbLockAspect.Location = new System.Drawing.Point(138, 78);
            this.cbLockAspect.Name = "cbLockAspect";
            this.cbLockAspect.Size = new System.Drawing.Size(86, 17);
            this.cbLockAspect.TabIndex = 22;
            this.cbLockAspect.Text = "Lock Aspect";
            this.cbLockAspect.UseVisualStyleBackColor = true;
            this.cbLockAspect.CheckedChanged += new System.EventHandler(this.cbLockAspect_CheckedChanged);
            // 
            // DimensionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(230, 184);
            this.Controls.Add(this.cbLockAspect);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbGroupX);
            this.Controls.Add(this.cbGroupY);
            this.Controls.Add(this.cbGroupHeight);
            this.Controls.Add(this.cbGroupWidth);
            this.Controls.Add(this.cbGroupRot);
            this.Controls.Add(this.lbRotation);
            this.Controls.Add(this.numRotation);
            this.Controls.Add(this.lbHeight);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.lbY);
            this.Controls.Add(this.lbX);
            this.Controls.Add(this.numHeight);
            this.Controls.Add(this.numWidth);
            this.Controls.Add(this.numY);
            this.Controls.Add(this.numX);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DimensionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Figure Dimensions";
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRotation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.Label lbY;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbHeight;
        private System.Windows.Forms.NumericUpDown numRotation;
        private System.Windows.Forms.Label lbRotation;
        private System.Windows.Forms.CheckBox cbGroupRot;
        private System.Windows.Forms.CheckBox cbGroupWidth;
        private System.Windows.Forms.CheckBox cbGroupHeight;
        private System.Windows.Forms.CheckBox cbGroupY;
        private System.Windows.Forms.CheckBox cbGroupX;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbLockAspect;
    }
}