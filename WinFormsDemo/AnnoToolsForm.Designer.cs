namespace WinFormsDemo
{
    partial class AnnoToolsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnoToolsForm));
            this.toolStripPanel1 = new System.Windows.Forms.ToolStripPanel();
            this.tsAnnotate = new WinFormsDemo.ToolStripEx();
            this.btnMouse = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnImportArea = new System.Windows.Forms.ToolStripButton();
            this.btnImportPage = new System.Windows.Forms.ToolStripButton();
            this.tsPersonal = new WinFormsDemo.PersonalToolbar.PersonalToolStrip();
            this.tsEngineState = new WinFormsDemo.FigureToolStrip();
            this.tsPropState = new WinFormsDemo.FigurePropertiesToolStrip();
            this.toolStripPanel1.SuspendLayout();
            this.tsAnnotate.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripPanel1
            // 
            this.toolStripPanel1.Controls.Add(this.tsAnnotate);
            this.toolStripPanel1.Controls.Add(this.tsPersonal);
            this.toolStripPanel1.Controls.Add(this.tsEngineState);
            this.toolStripPanel1.Controls.Add(this.tsPropState);
            this.toolStripPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.toolStripPanel1.Name = "toolStripPanel1";
            this.toolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolStripPanel1.Size = new System.Drawing.Size(317, 74);
            // 
            // tsAnnotate
            // 
            this.tsAnnotate.ClickThrough = true;
            this.tsAnnotate.Dock = System.Windows.Forms.DockStyle.None;
            this.tsAnnotate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMouse,
            this.toolStripSeparator1,
            this.btnUndo,
            this.btnImportArea,
            this.btnImportPage});
            this.tsAnnotate.Location = new System.Drawing.Point(3, 0);
            this.tsAnnotate.Name = "tsAnnotate";
            this.tsAnnotate.Size = new System.Drawing.Size(110, 25);
            this.tsAnnotate.TabIndex = 0;
            this.tsAnnotate.Text = "toolStrip1";
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
            // tsPersonal
            // 
            this.tsPersonal.ClickThrough = true;
            this.tsPersonal.Dap = null;
            this.tsPersonal.De = null;
            this.tsPersonal.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPersonal.Location = new System.Drawing.Point(113, 0);
            this.tsPersonal.Name = "tsPersonal";
            this.tsPersonal.Size = new System.Drawing.Size(35, 25);
            this.tsPersonal.TabIndex = 4;
            // 
            // tsEngineState
            // 
            this.tsEngineState.ClickThrough = true;
            this.tsEngineState.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEngineState.FigureClass = null;
            this.tsEngineState.Location = new System.Drawing.Point(3, 25);
            this.tsEngineState.Mode = WinFormsDemo.FigureToolStripMode.DEngineState;
            this.tsEngineState.Name = "tsEngineState";
            this.tsEngineState.Size = new System.Drawing.Size(314, 25);
            this.tsEngineState.TabIndex = 2;
            this.tsEngineState.Text = "tsEngineState";
            this.tsEngineState.AddToPersonalTools += new WinFormsDemo.FigureStyleEvent(this.tsEngineState_AddToPersonalTools);
            this.tsEngineState.FigureClassChanged += new WinFormsDemo.FigureClassChangedHandler(this.tsEngineState_FigureClassChanged);
            this.tsEngineState.DapChanged += new WinFormsDemo.DapChangedHandler(this.tsEngineState_DapChanged);
            // 
            // tsPropState
            // 
            this.tsPropState.ClickThrough = true;
            this.tsPropState.Dap = null;
            this.tsPropState.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPropState.FigureClass = null;
            this.tsPropState.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsPropState.Location = new System.Drawing.Point(3, 50);
            this.tsPropState.Name = "tsPropState";
            this.tsPropState.Size = new System.Drawing.Size(273, 25);
            this.tsPropState.TabIndex = 3;
            this.tsPropState.Text = "tsPropState";
            // 
            // AnnoToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(317, 74);
            this.Controls.Add(this.toolStripPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnnoToolsForm";
            this.ShowInTaskbar = false;
            this.Text = "Screen Annotate";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.AnnoToolsForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnnoToolsForm_FormClosing);
            this.toolStripPanel1.ResumeLayout(false);
            this.toolStripPanel1.PerformLayout();
            this.tsAnnotate.ResumeLayout(false);
            this.tsAnnotate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStripEx tsAnnotate;
        private System.Windows.Forms.ToolStripButton btnMouse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnImportArea;
        private System.Windows.Forms.ToolStripButton btnImportPage;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private FigureToolStrip tsEngineState;
        private FigurePropertiesToolStrip tsPropState;
        private System.Windows.Forms.ToolStripPanel toolStripPanel1;
        private WinFormsDemo.PersonalToolbar.PersonalToolStrip tsPersonal;
    }
}