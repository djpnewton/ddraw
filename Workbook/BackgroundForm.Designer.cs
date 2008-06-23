namespace WinFormsDemo
{
    partial class BackgroundForm
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
            this.btnColor = new System.Windows.Forms.Button();
            this.rbColor = new System.Windows.Forms.RadioButton();
            this.rbImage = new System.Windows.Forms.RadioButton();
            this.btnImageBrowse = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbImagePos = new System.Windows.Forms.ComboBox();
            this.cbApplyAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnColor
            // 
            this.btnColor.Location = new System.Drawing.Point(12, 35);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(59, 23);
            this.btnColor.TabIndex = 0;
            this.btnColor.Text = "Color";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // rbColor
            // 
            this.rbColor.AutoSize = true;
            this.rbColor.Location = new System.Drawing.Point(12, 12);
            this.rbColor.Name = "rbColor";
            this.rbColor.Size = new System.Drawing.Size(75, 17);
            this.rbColor.TabIndex = 1;
            this.rbColor.TabStop = true;
            this.rbColor.Text = "Solid Color";
            this.rbColor.UseVisualStyleBackColor = true;
            this.rbColor.CheckedChanged += new System.EventHandler(this.rbColor_CheckedChanged);
            // 
            // rbImage
            // 
            this.rbImage.AutoSize = true;
            this.rbImage.Location = new System.Drawing.Point(162, 12);
            this.rbImage.Name = "rbImage";
            this.rbImage.Size = new System.Drawing.Size(54, 17);
            this.rbImage.TabIndex = 2;
            this.rbImage.TabStop = true;
            this.rbImage.Text = "Image";
            this.rbImage.UseVisualStyleBackColor = true;
            this.rbImage.CheckedChanged += new System.EventHandler(this.rbImage_CheckedChanged);
            // 
            // btnImageBrowse
            // 
            this.btnImageBrowse.Location = new System.Drawing.Point(162, 35);
            this.btnImageBrowse.Name = "btnImageBrowse";
            this.btnImageBrowse.Size = new System.Drawing.Size(59, 23);
            this.btnImageBrowse.TabIndex = 3;
            this.btnImageBrowse.Text = "Browse..";
            this.btnImageBrowse.UseVisualStyleBackColor = true;
            this.btnImageBrowse.Click += new System.EventHandler(this.btnImageBrowse_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(162, 64);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 89);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(150, 186);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(231, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 116);
            this.panel1.TabIndex = 7;
            // 
            // cbImagePos
            // 
            this.cbImagePos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImagePos.FormattingEnabled = true;
            this.cbImagePos.Items.AddRange(new object[] {
            "Stretch",
            "Top Left",
            "Center",
            "Tile",
            "Stretch (Keep Aspect Ratio)"});
            this.cbImagePos.Location = new System.Drawing.Point(162, 159);
            this.cbImagePos.Name = "cbImagePos";
            this.cbImagePos.Size = new System.Drawing.Size(144, 21);
            this.cbImagePos.TabIndex = 8;
            // 
            // cbApplyAll
            // 
            this.cbApplyAll.AutoSize = true;
            this.cbApplyAll.Location = new System.Drawing.Point(35, 190);
            this.cbApplyAll.Name = "cbApplyAll";
            this.cbApplyAll.Size = new System.Drawing.Size(109, 17);
            this.cbApplyAll.TabIndex = 9;
            this.cbApplyAll.Text = "Apply to all pages";
            this.cbApplyAll.UseVisualStyleBackColor = true;
            // 
            // BackgroundForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(318, 218);
            this.Controls.Add(this.cbApplyAll);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnImageBrowse);
            this.Controls.Add(this.cbImagePos);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.rbImage);
            this.Controls.Add(this.rbColor);
            this.Controls.Add(this.btnColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackgroundForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Background";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.RadioButton rbColor;
        private System.Windows.Forms.RadioButton rbImage;
        private System.Windows.Forms.Button btnImageBrowse;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbImagePos;
        private System.Windows.Forms.CheckBox cbApplyAll;
    }
}