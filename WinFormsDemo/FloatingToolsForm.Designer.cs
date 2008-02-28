namespace WinFormsDemo
{
    partial class FloatingToolsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FloatingToolsForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnMouse = new System.Windows.Forms.ToolStripButton();
            this.btnScreenAnnotate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnPolyline = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnImportArea = new System.Windows.Forms.ToolStripButton();
            this.btnImportPage = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMouse,
            this.btnScreenAnnotate,
            this.toolStripSeparator1,
            this.btnSelect,
            this.btnPolyline,
            this.btnUndo,
            this.toolStripSeparator2,
            this.btnImportArea,
            this.btnImportPage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(361, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnMouse
            // 
            this.btnMouse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMouse.Image = ((System.Drawing.Image)(resources.GetObject("btnMouse.Image")));
            this.btnMouse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMouse.Name = "btnMouse";
            this.btnMouse.Size = new System.Drawing.Size(42, 22);
            this.btnMouse.Text = "Mouse";
            this.btnMouse.Click += new System.EventHandler(this.btnMouse_Click);
            // 
            // btnScreenAnnotate
            // 
            this.btnScreenAnnotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScreenAnnotate.Image = ((System.Drawing.Image)(resources.GetObject("btnScreenAnnotate.Image")));
            this.btnScreenAnnotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScreenAnnotate.Name = "btnScreenAnnotate";
            this.btnScreenAnnotate.Size = new System.Drawing.Size(92, 22);
            this.btnScreenAnnotate.Text = "Screen Annotate";
            this.btnScreenAnnotate.Click += new System.EventHandler(this.btnScreenAnnotate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSelect
            // 
            this.btnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(23, 22);
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnPolyline
            // 
            this.btnPolyline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPolyline.Image = ((System.Drawing.Image)(resources.GetObject("btnPolyline.Image")));
            this.btnPolyline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPolyline.Name = "btnPolyline";
            this.btnPolyline.Size = new System.Drawing.Size(23, 22);
            this.btnPolyline.Text = "Pen";
            this.btnPolyline.Click += new System.EventHandler(this.btnPolyline_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnImportArea
            // 
            this.btnImportArea.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportArea.Image = ((System.Drawing.Image)(resources.GetObject("btnImportArea.Image")));
            this.btnImportArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportArea.Name = "btnImportArea";
            this.btnImportArea.Size = new System.Drawing.Size(69, 22);
            this.btnImportArea.Text = "Import Area";
            this.btnImportArea.Click += new System.EventHandler(this.btnImportArea_Click);
            // 
            // btnImportPage
            // 
            this.btnImportPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportPage.Image = ((System.Drawing.Image)(resources.GetObject("btnImportPage.Image")));
            this.btnImportPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportPage.Name = "btnImportPage";
            this.btnImportPage.Size = new System.Drawing.Size(70, 17);
            this.btnImportPage.Text = "Import Page";
            this.btnImportPage.Click += new System.EventHandler(this.btnImportPage_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 22);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // FloatingToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 25);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FloatingToolsForm";
            this.Text = "Floating Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FloatingToolsForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnScreenAnnotate;
        private System.Windows.Forms.ToolStripButton btnMouse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnPolyline;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnImportArea;
        private System.Windows.Forms.ToolStripButton btnImportPage;
        private System.Windows.Forms.ToolStripButton btnUndo;
    }
}