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
            this.components = new System.ComponentModel.Container();
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
            this.btnText = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlViewer = new System.Windows.Forms.Panel();
            this.wfvcEditor = new DDraw.WinForms.WFViewerControl();
            this.pnlFigureProps = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFigureProps = new System.Windows.Forms.ToolStrip();
            this.btnFill = new WinFormsDemo.ToolStripColorButton();
            this.btnStroke = new WinFormsDemo.ToolStripColorButton();
            this.btnStrokeWidth = new WinFormsDemo.ToolStripStrokeWidthButton();
            this.btnAlpha = new WinFormsDemo.ToolStripAlphaButton();
            this.cbFontName = new WinFormsDemo.ToolStripFontNameChooser();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.cmsFigure = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.sendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsBackground = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pageSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.a4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.a5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.letterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlPreviews.SuspendLayout();
            this.tsEditorMode.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlViewer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tsFigureProps.SuspendLayout();
            this.cmsFigure.SuspendLayout();
            this.cmsBackground.SuspendLayout();
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
            this.btnEllipse,
            this.btnText});
            this.tsEditorMode.Location = new System.Drawing.Point(3, 24);
            this.tsEditorMode.Name = "tsEditorMode";
            this.tsEditorMode.Size = new System.Drawing.Size(185, 25);
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
            this.btnRect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRect.Image = ((System.Drawing.Image)(resources.GetObject("btnRect.Image")));
            this.btnRect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRect.Name = "btnRect";
            this.btnRect.Size = new System.Drawing.Size(23, 22);
            this.btnRect.Text = "Rectangle";
            this.btnRect.Click += new System.EventHandler(this.btnRect_Click);
            // 
            // btnEllipse
            // 
            this.btnEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEllipse.Image = ((System.Drawing.Image)(resources.GetObject("btnEllipse.Image")));
            this.btnEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(23, 22);
            this.btnEllipse.Text = "Ellipse";
            this.btnEllipse.Click += new System.EventHandler(this.btnEllipse_Click);
            // 
            // btnText
            // 
            this.btnText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnText.Image = ((System.Drawing.Image)(resources.GetObject("btnText.Image")));
            this.btnText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnText.Name = "btnText";
            this.btnText.Size = new System.Drawing.Size(23, 22);
            this.btnText.Text = "Text";
            this.btnText.Click += new System.EventHandler(this.btnText_Click);
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
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsFigureProps);
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
            this.pnlFigureProps.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlFigureProps.Location = new System.Drawing.Point(0, 0);
            this.pnlFigureProps.Name = "pnlFigureProps";
            this.pnlFigureProps.Size = new System.Drawing.Size(101, 244);
            this.pnlFigureProps.TabIndex = 0;
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
            this.editToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(540, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpened += new System.EventHandler(this.editToolStripMenuItem_DropDownOpened);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
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
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tsFigureProps
            // 
            this.tsFigureProps.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFigureProps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFill,
            this.btnStroke,
            this.btnStrokeWidth,
            this.btnAlpha,
            this.cbFontName});
            this.tsFigureProps.Location = new System.Drawing.Point(188, 24);
            this.tsFigureProps.Name = "tsFigureProps";
            this.tsFigureProps.Size = new System.Drawing.Size(250, 25);
            this.tsFigureProps.TabIndex = 2;
            // 
            // btnFill
            // 
            this.btnFill.Color = System.Drawing.Color.Empty;
            this.btnFill.ColorType = WinFormsDemo.ColorType.Fill;
            this.btnFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.btnFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(23, 22);
            this.btnFill.Text = "Fill";
            this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
            // 
            // btnStroke
            // 
            this.btnStroke.Color = System.Drawing.Color.Empty;
            this.btnStroke.ColorType = WinFormsDemo.ColorType.Stroke;
            this.btnStroke.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.btnStroke.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStroke.Name = "btnStroke";
            this.btnStroke.Size = new System.Drawing.Size(23, 22);
            this.btnStroke.Text = "Stroke";
            this.btnStroke.Click += new System.EventHandler(this.btnStroke_Click);
            // 
            // btnStrokeWidth
            // 
            this.btnStrokeWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStrokeWidth.Image = ((System.Drawing.Image)(resources.GetObject("btnStrokeWidth.Image")));
            this.btnStrokeWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrokeWidth.Name = "btnStrokeWidth";
            this.btnStrokeWidth.ShowDropDownArrow = false;
            this.btnStrokeWidth.Size = new System.Drawing.Size(20, 22);
            this.btnStrokeWidth.Text = "Stroke Width";
            this.btnStrokeWidth.Value = 1;
            this.btnStrokeWidth.StrokeWidthChanged += new WinFormsDemo.StrokeWidthChangedHandler(this.btnStrokeWidth_StrokeWidthChanged);
            // 
            // btnAlpha
            // 
            this.btnAlpha.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAlpha.Image = ((System.Drawing.Image)(resources.GetObject("btnAlpha.Image")));
            this.btnAlpha.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAlpha.Name = "btnAlpha";
            this.btnAlpha.ShowDropDownArrow = false;
            this.btnAlpha.Size = new System.Drawing.Size(20, 22);
            this.btnAlpha.Text = "Alpha";
            this.btnAlpha.Value = 1;
            this.btnAlpha.AlphaChanged += new WinFormsDemo.AlphaChangedHandler(this.btnAlpha_AlphaChanged);
            // 
            // cbFontName
            // 
            this.cbFontName.Name = "cbFontName";
            this.cbFontName.Size = new System.Drawing.Size(121, 22);
            this.cbFontName.Value = null;
            this.cbFontName.FontNameChanged += new System.EventHandler(this.cbFontName_FontNameChanged);
            // 
            // cmsFigure
            // 
            this.cmsFigure.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupToolStripMenuItem,
            this.toolStripSeparator2,
            this.sendToBackToolStripMenuItem,
            this.bringToFrontToolStripMenuItem,
            this.sendBackwardToolStripMenuItem,
            this.bringForwardToolStripMenuItem});
            this.cmsFigure.Name = "cmsFigure";
            this.cmsFigure.Size = new System.Drawing.Size(159, 120);
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            this.groupToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.groupToolStripMenuItem.Text = "Group";
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.groupToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
            // 
            // sendToBackToolStripMenuItem
            // 
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            this.sendToBackToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.sendToBackToolStripMenuItem.Text = "Send to Back";
            this.sendToBackToolStripMenuItem.Click += new System.EventHandler(this.orderToolStripMenuItem_Click);
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            this.bringToFrontToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.bringToFrontToolStripMenuItem.Text = "Bring to Front";
            this.bringToFrontToolStripMenuItem.Click += new System.EventHandler(this.orderToolStripMenuItem_Click);
            // 
            // sendBackwardToolStripMenuItem
            // 
            this.sendBackwardToolStripMenuItem.Name = "sendBackwardToolStripMenuItem";
            this.sendBackwardToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.sendBackwardToolStripMenuItem.Text = "Send Backward";
            this.sendBackwardToolStripMenuItem.Click += new System.EventHandler(this.orderToolStripMenuItem_Click);
            // 
            // bringForwardToolStripMenuItem
            // 
            this.bringForwardToolStripMenuItem.Name = "bringForwardToolStripMenuItem";
            this.bringForwardToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.bringForwardToolStripMenuItem.Text = "Bring Forward";
            this.bringForwardToolStripMenuItem.Click += new System.EventHandler(this.orderToolStripMenuItem_Click);
            // 
            // cmsBackground
            // 
            this.cmsBackground.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageSizeToolStripMenuItem});
            this.cmsBackground.Name = "cmsBackground";
            this.cmsBackground.Size = new System.Drawing.Size(132, 26);
            // 
            // pageSizeToolStripMenuItem
            // 
            this.pageSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.a4ToolStripMenuItem,
            this.a5ToolStripMenuItem,
            this.letterToolStripMenuItem,
            this.customToolStripMenuItem});
            this.pageSizeToolStripMenuItem.Name = "pageSizeToolStripMenuItem";
            this.pageSizeToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.pageSizeToolStripMenuItem.Text = "Page Size";
            // 
            // a4ToolStripMenuItem
            // 
            this.a4ToolStripMenuItem.Name = "a4ToolStripMenuItem";
            this.a4ToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.a4ToolStripMenuItem.Text = "A4";
            this.a4ToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // a5ToolStripMenuItem
            // 
            this.a5ToolStripMenuItem.Name = "a5ToolStripMenuItem";
            this.a5ToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.a5ToolStripMenuItem.Text = "A5";
            this.a5ToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // letterToolStripMenuItem
            // 
            this.letterToolStripMenuItem.Name = "letterToolStripMenuItem";
            this.letterToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.letterToolStripMenuItem.Text = "Letter";
            this.letterToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tsFigureProps.ResumeLayout(false);
            this.tsFigureProps.PerformLayout();
            this.cmsFigure.ResumeLayout(false);
            this.cmsBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DDraw.WinForms.WFViewerControl wfvcEditor;
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
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnPen;
        private PreviewBar previewBar1;
        private System.Windows.Forms.ToolStripButton btnRect;
        private System.Windows.Forms.ToolStripButton btnEllipse;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsFigure;
        private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem sendToBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringToFrontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendBackwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringForwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnText;
        private System.Windows.Forms.ContextMenuStrip cmsBackground;
        private System.Windows.Forms.ToolStripMenuItem pageSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem a4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem a5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem letterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStrip tsFigureProps;
        private ToolStripColorButton btnFill;
        private ToolStripColorButton btnStroke;
        private ToolStripStrokeWidthButton btnStrokeWidth;
        private ToolStripAlphaButton btnAlpha;
        private ToolStripFontNameChooser cbFontName;
    }
}

