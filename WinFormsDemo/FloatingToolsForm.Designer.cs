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
            this.toolStripPanel1 = new System.Windows.Forms.ToolStripPanel();
            this.toolStrip1 = new WinFormsDemo.ToolStripEx();
            this.btnMouse = new System.Windows.Forms.ToolStripButton();
            this.btnScreenAnnotate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnImportArea = new System.Windows.Forms.ToolStripButton();
            this.btnImportPage = new System.Windows.Forms.ToolStripButton();
            this.tsEngineState = new WinFormsDemo.ToolStripDEngineState();
            this.tsPropState = new WinFormsDemo.ToolStripDAuthorPropsState();
            this.toolStripPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripPanel1
            // 
            this.toolStripPanel1.Controls.Add(this.toolStrip1);
            this.toolStripPanel1.Controls.Add(this.tsEngineState);
            this.toolStripPanel1.Controls.Add(this.tsPropState);
            this.toolStripPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.toolStripPanel1.Name = "toolStripPanel1";
            this.toolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolStripPanel1.Size = new System.Drawing.Size(317, 74);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ClickThrough = true;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMouse,
            this.btnScreenAnnotate,
            this.toolStripSeparator1,
            this.btnUndo,
            this.btnImportArea,
            this.btnImportPage});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(133, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnMouse
            // 
            this.btnMouse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMouse.Image = ((System.Drawing.Image)(resources.GetObject("btnMouse.Image")));
            this.btnMouse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMouse.Name = "btnMouse";
            this.btnMouse.Size = new System.Drawing.Size(23, 22);
            this.btnMouse.Text = "Mouse";
            this.btnMouse.Click += new System.EventHandler(this.btnMouse_Click);
            // 
            // btnScreenAnnotate
            // 
            this.btnScreenAnnotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScreenAnnotate.Image = ((System.Drawing.Image)(resources.GetObject("btnScreenAnnotate.Image")));
            this.btnScreenAnnotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScreenAnnotate.Name = "btnScreenAnnotate";
            this.btnScreenAnnotate.Size = new System.Drawing.Size(23, 22);
            this.btnScreenAnnotate.Text = "Screen Annotate";
            this.btnScreenAnnotate.Click += new System.EventHandler(this.btnScreenAnnotate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // btnImportArea
            // 
            this.btnImportArea.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImportArea.Image = ((System.Drawing.Image)(resources.GetObject("btnImportArea.Image")));
            this.btnImportArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportArea.Name = "btnImportArea";
            this.btnImportArea.Size = new System.Drawing.Size(23, 22);
            this.btnImportArea.Text = "Import Area as Image";
            this.btnImportArea.Click += new System.EventHandler(this.btnImportArea_Click);
            // 
            // btnImportPage
            // 
            this.btnImportPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImportPage.Image = ((System.Drawing.Image)(resources.GetObject("btnImportPage.Image")));
            this.btnImportPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportPage.Name = "btnImportPage";
            this.btnImportPage.Size = new System.Drawing.Size(23, 22);
            this.btnImportPage.Text = "Import Screen as Page";
            this.btnImportPage.Click += new System.EventHandler(this.btnImportPage_Click);
            // 
            // tsEngineState
            // 
            this.tsEngineState.ClickThrough = true;
            this.tsEngineState.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEngineState.Location = new System.Drawing.Point(3, 25);
            this.tsEngineState.Name = "tsEngineState";
            this.tsEngineState.Size = new System.Drawing.Size(288, 25);
            this.tsEngineState.TabIndex = 2;
            this.tsEngineState.Text = "tsEngineState";
            // 
            // tsPropState
            // 
            this.tsPropState.ClickThrough = true;
            this.tsPropState.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPropState.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsPropState.Location = new System.Drawing.Point(3, 50);
            this.tsPropState.Name = "tsPropState";
            this.tsPropState.Size = new System.Drawing.Size(291, 25);
            this.tsPropState.TabIndex = 3;
            this.tsPropState.Text = "tsPropState";
            // 
            // FloatingToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(317, 74);
            this.Controls.Add(this.toolStripPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FloatingToolsForm";
            this.Text = "Floating Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FloatingToolsForm_FormClosing);
            this.toolStripPanel1.ResumeLayout(false);
            this.toolStripPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStripEx toolStrip1;
        private System.Windows.Forms.ToolStripButton btnScreenAnnotate;
        private System.Windows.Forms.ToolStripButton btnMouse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnImportArea;
        private System.Windows.Forms.ToolStripButton btnImportPage;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private ToolStripDEngineState tsEngineState;
        private ToolStripDAuthorPropsState tsPropState;
        private System.Windows.Forms.ToolStripPanel toolStripPanel1;
    }
}