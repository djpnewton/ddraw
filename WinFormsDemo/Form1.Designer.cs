namespace WinFormsDemo
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pnlPreviews = new System.Windows.Forms.Panel();
            this.previewBar1 = new WinFormsDemo.PreviewBar();
            this.tsEditorMode = new System.Windows.Forms.ToolStrip();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnPen = new System.Windows.Forms.ToolStripButton();
            this.btnRect = new System.Windows.Forms.ToolStripButton();
            this.btnEllipse = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlViewer = new System.Windows.Forms.Panel();
            this.wfvcEditor = new DDraw.WFViewerControl();
            this.pnlFigureProps = new System.Windows.Forms.Panel();
            this.cbFont = new System.Windows.Forms.ComboBox();
            this.tbStrokeWidth = new System.Windows.Forms.TrackBar();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.btnStroke = new System.Windows.Forms.Button();
            this.btnFill = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnGroup = new System.Windows.Forms.Button();
            this.pnlPreviews.SuspendLayout();
            this.tsEditorMode.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlViewer.SuspendLayout();
            this.pnlFigureProps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbStrokeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPreviews
            // 
            this.pnlPreviews.Controls.Add(this.previewBar1);
            this.pnlPreviews.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlPreviews.Location = new System.Drawing.Point(358, 0);
            this.pnlPreviews.Name = "pnlPreviews";
            this.pnlPreviews.Size = new System.Drawing.Size(81, 244);
            this.pnlPreviews.TabIndex = 9;
            // 
            // previewBar1
            // 
            this.previewBar1.AutoScroll = true;
            this.previewBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBar1.Location = new System.Drawing.Point(0, 0);
            this.previewBar1.Name = "previewBar1";
            this.previewBar1.Size = new System.Drawing.Size(81, 244);
            this.previewBar1.TabIndex = 0;
            this.previewBar1.PreviewSelected += new WinFormsDemo.PreviewSelectedHandler(this.previewBar1_PreviewSelected);
            this.previewBar1.PreviewAdd += new System.EventHandler(this.previewBar1_PreviewAdd);
            // 
            // tsEditorMode
            // 
            this.tsEditorMode.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEditorMode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAntiAlias,
            this.toolStripSeparator1,
            this.btnSelect,
            this.btnPen,
            this.btnRect,
            this.btnEllipse});
            this.tsEditorMode.Location = new System.Drawing.Point(3, 24);
            this.tsEditorMode.Name = "tsEditorMode";
            this.tsEditorMode.Size = new System.Drawing.Size(215, 25);
            this.tsEditorMode.TabIndex = 0;
            this.tsEditorMode.Text = "toolStrip1";
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAntiAlias.Image = ((System.Drawing.Image)(resources.GetObject("btnAntiAlias.Image")));
            this.btnAntiAlias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(52, 22);
            this.btnAntiAlias.Text = "AntiAlias";
            this.btnAntiAlias.Click += new System.EventHandler(this.btnAntiAlias_Click);
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
            // btnPen
            // 
            this.btnPen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPen.Image = ((System.Drawing.Image)(resources.GetObject("btnPen.Image")));
            this.btnPen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPen.Name = "btnPen";
            this.btnPen.Size = new System.Drawing.Size(23, 22);
            this.btnPen.Text = "Pen";
            this.btnPen.Click += new System.EventHandler(this.btnPen_Click);
            // 
            // btnRect
            // 
            this.btnRect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRect.Image = ((System.Drawing.Image)(resources.GetObject("btnRect.Image")));
            this.btnRect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRect.Name = "btnRect";
            this.btnRect.Size = new System.Drawing.Size(59, 22);
            this.btnRect.Text = "Rectangle";
            this.btnRect.Click += new System.EventHandler(this.btnRect_Click);
            // 
            // btnEllipse
            // 
            this.btnEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEllipse.Image = ((System.Drawing.Image)(resources.GetObject("btnEllipse.Image")));
            this.btnEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(40, 22);
            this.btnEllipse.Text = "Ellipse";
            this.btnEllipse.Click += new System.EventHandler(this.btnEllipse_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pnlMain);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(540, 266);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(540, 315);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsEditorMode);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlViewer);
            this.pnlMain.Controls.Add(this.pnlFigureProps);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(540, 244);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlViewer
            // 
            this.pnlViewer.Controls.Add(this.wfvcEditor);
            this.pnlViewer.Controls.Add(this.pnlPreviews);
            this.pnlViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewer.Location = new System.Drawing.Point(101, 0);
            this.pnlViewer.Name = "pnlViewer";
            this.pnlViewer.Size = new System.Drawing.Size(439, 244);
            this.pnlViewer.TabIndex = 10;
            // 
            // wfvcEditor
            // 
            this.wfvcEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wfvcEditor.Location = new System.Drawing.Point(0, 0);
            this.wfvcEditor.Name = "wfvcEditor";
            this.wfvcEditor.Size = new System.Drawing.Size(358, 244);
            this.wfvcEditor.TabIndex = 0;
            // 
            // pnlFigureProps
            // 
            this.pnlFigureProps.Controls.Add(this.btnGroup);
            this.pnlFigureProps.Controls.Add(this.cbFont);
            this.pnlFigureProps.Controls.Add(this.tbStrokeWidth);
            this.pnlFigureProps.Controls.Add(this.tbAlpha);
            this.pnlFigureProps.Controls.Add(this.btnStroke);
            this.pnlFigureProps.Controls.Add(this.btnFill);
            this.pnlFigureProps.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlFigureProps.Location = new System.Drawing.Point(0, 0);
            this.pnlFigureProps.Name = "pnlFigureProps";
            this.pnlFigureProps.Size = new System.Drawing.Size(101, 244);
            this.pnlFigureProps.TabIndex = 0;
            // 
            // cbFont
            // 
            this.cbFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFont.FormattingEnabled = true;
            this.cbFont.Location = new System.Drawing.Point(3, 163);
            this.cbFont.Name = "cbFont";
            this.cbFont.Size = new System.Drawing.Size(92, 21);
            this.cbFont.TabIndex = 9;
            this.cbFont.SelectedIndexChanged += new System.EventHandler(this.cbFont_SelectedIndexChanged);
            // 
            // tbStrokeWidth
            // 
            this.tbStrokeWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStrokeWidth.Location = new System.Drawing.Point(3, 61);
            this.tbStrokeWidth.Minimum = 1;
            this.tbStrokeWidth.Name = "tbStrokeWidth";
            this.tbStrokeWidth.Size = new System.Drawing.Size(92, 45);
            this.tbStrokeWidth.TabIndex = 10;
            this.tbStrokeWidth.Value = 3;
            this.tbStrokeWidth.Scroll += new System.EventHandler(this.tsStrokeWidth_Scroll);
            // 
            // tbAlpha
            // 
            this.tbAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAlpha.Location = new System.Drawing.Point(3, 112);
            this.tbAlpha.Maximum = 5;
            this.tbAlpha.Minimum = 1;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(92, 45);
            this.tbAlpha.TabIndex = 9;
            this.tbAlpha.Value = 3;
            this.tbAlpha.Scroll += new System.EventHandler(this.tbAlpha_Scroll);
            // 
            // btnStroke
            // 
            this.btnStroke.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStroke.Location = new System.Drawing.Point(3, 32);
            this.btnStroke.Name = "btnStroke";
            this.btnStroke.Size = new System.Drawing.Size(92, 23);
            this.btnStroke.TabIndex = 9;
            this.btnStroke.Text = "Stroke";
            this.btnStroke.UseVisualStyleBackColor = true;
            this.btnStroke.Click += new System.EventHandler(this.btnStroke_Click);
            // 
            // btnFill
            // 
            this.btnFill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFill.Location = new System.Drawing.Point(3, 3);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(92, 23);
            this.btnFill.TabIndex = 8;
            this.btnFill.Text = "Fill";
            this.btnFill.UseVisualStyleBackColor = true;
            this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 244);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(540, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbInfo
            // 
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(540, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageToolStripMenuItem,
            this.textToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.imageToolStripMenuItem.Text = "Image";
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.textToolStripMenuItem.Text = "Text";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // btnGroup
            // 
            this.btnGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroup.Location = new System.Drawing.Point(3, 190);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(92, 23);
            this.btnGroup.TabIndex = 11;
            this.btnGroup.Text = "Group";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 315);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.pnlPreviews.ResumeLayout(false);
            this.tsEditorMode.ResumeLayout(false);
            this.tsEditorMode.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlViewer.ResumeLayout(false);
            this.pnlFigureProps.ResumeLayout(false);
            this.pnlFigureProps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbStrokeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DDraw.WFViewerControl wfvcEditor;
        private System.Windows.Forms.Panel pnlPreviews;
        private System.Windows.Forms.ToolStrip tsEditorMode;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel pnlViewer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ToolStripStatusLabel lbInfo;
        private System.Windows.Forms.Panel pnlFigureProps;
        private System.Windows.Forms.Button btnStroke;
        private System.Windows.Forms.Button btnFill;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.TrackBar tbStrokeWidth;
        private System.Windows.Forms.ComboBox cbFont;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnPen;
        private PreviewBar previewBar1;
        private System.Windows.Forms.ToolStripButton btnRect;
        private System.Windows.Forms.ToolStripButton btnEllipse;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.Button btnGroup;

    }
}

