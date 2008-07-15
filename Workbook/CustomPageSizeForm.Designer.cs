namespace Workbook
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
            this.rbA4 = new System.Windows.Forms.RadioButton();
            this.rbA5 = new System.Windows.Forms.RadioButton();
            this.rbLetter = new System.Windows.Forms.RadioButton();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.cbApplyAll = new System.Windows.Forms.CheckBox();
            this.rbDefault = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidthMM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeightMM)).BeginInit();
            this.SuspendLayout();
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Location = new System.Drawing.Point(28, 124);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(60, 13);
            this.lbWidth.TabIndex = 0;
            this.lbWidth.Text = "Width (mm)";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Location = new System.Drawing.Point(28, 150);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(63, 13);
            this.lbHeight.TabIndex = 1;
            this.lbHeight.Text = "Height (mm)";
            // 
            // nudWidthMM
            // 
            this.nudWidthMM.Location = new System.Drawing.Point(94, 122);
            this.nudWidthMM.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidthMM.Name = "nudWidthMM";
            this.nudWidthMM.Size = new System.Drawing.Size(71, 20);
            this.nudWidthMM.TabIndex = 5;
            // 
            // nudHeightMM
            // 
            this.nudHeightMM.Location = new System.Drawing.Point(94, 148);
            this.nudHeightMM.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeightMM.Name = "nudHeightMM";
            this.nudHeightMM.Size = new System.Drawing.Size(71, 20);
            this.nudHeightMM.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(127, 176);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(209, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rbA4
            // 
            this.rbA4.AutoSize = true;
            this.rbA4.Location = new System.Drawing.Point(12, 35);
            this.rbA4.Name = "rbA4";
            this.rbA4.Size = new System.Drawing.Size(38, 17);
            this.rbA4.TabIndex = 1;
            this.rbA4.TabStop = true;
            this.rbA4.Text = "A4";
            this.rbA4.UseVisualStyleBackColor = true;
            this.rbA4.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbA5
            // 
            this.rbA5.AutoSize = true;
            this.rbA5.Location = new System.Drawing.Point(12, 58);
            this.rbA5.Name = "rbA5";
            this.rbA5.Size = new System.Drawing.Size(38, 17);
            this.rbA5.TabIndex = 2;
            this.rbA5.TabStop = true;
            this.rbA5.Text = "A5";
            this.rbA5.UseVisualStyleBackColor = true;
            this.rbA5.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbLetter
            // 
            this.rbLetter.AutoSize = true;
            this.rbLetter.Location = new System.Drawing.Point(12, 81);
            this.rbLetter.Name = "rbLetter";
            this.rbLetter.Size = new System.Drawing.Size(52, 17);
            this.rbLetter.TabIndex = 3;
            this.rbLetter.TabStop = true;
            this.rbLetter.Text = "Letter";
            this.rbLetter.UseVisualStyleBackColor = true;
            this.rbLetter.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(12, 104);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(60, 17);
            this.rbCustom.TabIndex = 4;
            this.rbCustom.TabStop = true;
            this.rbCustom.Text = "Custom";
            this.rbCustom.UseVisualStyleBackColor = true;
            this.rbCustom.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // cbApplyAll
            // 
            this.cbApplyAll.AutoSize = true;
            this.cbApplyAll.Location = new System.Drawing.Point(12, 180);
            this.cbApplyAll.Name = "cbApplyAll";
            this.cbApplyAll.Size = new System.Drawing.Size(109, 17);
            this.cbApplyAll.TabIndex = 7;
            this.cbApplyAll.Text = "Apply to all pages";
            this.cbApplyAll.UseVisualStyleBackColor = true;
            // 
            // rbDefault
            // 
            this.rbDefault.AutoSize = true;
            this.rbDefault.Location = new System.Drawing.Point(12, 12);
            this.rbDefault.Name = "rbDefault";
            this.rbDefault.Size = new System.Drawing.Size(59, 17);
            this.rbDefault.TabIndex = 0;
            this.rbDefault.TabStop = true;
            this.rbDefault.Text = "Default";
            this.rbDefault.UseVisualStyleBackColor = true;
            // 
            // CustomPageSizeForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(293, 210);
            this.Controls.Add(this.rbDefault);
            this.Controls.Add(this.cbApplyAll);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.rbLetter);
            this.Controls.Add(this.rbA5);
            this.Controls.Add(this.rbA4);
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
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Page Size";
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
        private System.Windows.Forms.RadioButton rbA4;
        private System.Windows.Forms.RadioButton rbA5;
        private System.Windows.Forms.RadioButton rbLetter;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.CheckBox cbApplyAll;
        private System.Windows.Forms.RadioButton rbDefault;
    }
}