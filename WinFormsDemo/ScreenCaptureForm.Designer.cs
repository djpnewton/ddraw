namespace WinFormsDemo
{
    partial class ScreenCaptureForm
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
            this.btnCaptureRect = new System.Windows.Forms.Button();
            this.btnCaptureFull = new System.Windows.Forms.Button();
            this.btnCaptureWindow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCaptureRect
            // 
            this.btnCaptureRect.Location = new System.Drawing.Point(12, 12);
            this.btnCaptureRect.Name = "btnCaptureRect";
            this.btnCaptureRect.Size = new System.Drawing.Size(154, 23);
            this.btnCaptureRect.TabIndex = 0;
            this.btnCaptureRect.Text = "Capture Rectangular Area";
            this.btnCaptureRect.UseVisualStyleBackColor = true;
            this.btnCaptureRect.Click += new System.EventHandler(this.btnCaptureRect_Click);
            // 
            // btnCaptureFull
            // 
            this.btnCaptureFull.Location = new System.Drawing.Point(12, 41);
            this.btnCaptureFull.Name = "btnCaptureFull";
            this.btnCaptureFull.Size = new System.Drawing.Size(154, 23);
            this.btnCaptureFull.TabIndex = 1;
            this.btnCaptureFull.Text = "Capture Full Screen";
            this.btnCaptureFull.UseVisualStyleBackColor = true;
            this.btnCaptureFull.Click += new System.EventHandler(this.btnCaptureFull_Click);
            // 
            // btnCaptureWindow
            // 
            this.btnCaptureWindow.Location = new System.Drawing.Point(12, 70);
            this.btnCaptureWindow.Name = "btnCaptureWindow";
            this.btnCaptureWindow.Size = new System.Drawing.Size(154, 23);
            this.btnCaptureWindow.TabIndex = 2;
            this.btnCaptureWindow.Text = "Capture Window";
            this.btnCaptureWindow.UseVisualStyleBackColor = true;
            this.btnCaptureWindow.Click += new System.EventHandler(this.btnCaptureWindow_Click);
            // 
            // ScreenCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 108);
            this.Controls.Add(this.btnCaptureWindow);
            this.Controls.Add(this.btnCaptureFull);
            this.Controls.Add(this.btnCaptureRect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScreenCaptureForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Screen Capture";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCaptureRect;
        private System.Windows.Forms.Button btnCaptureFull;
        private System.Windows.Forms.Button btnCaptureWindow;
    }
}