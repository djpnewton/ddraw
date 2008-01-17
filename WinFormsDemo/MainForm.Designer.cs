namespace WinFormsDemo
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pnlPreviews = new System.Windows.Forms.Panel();
            this.tsEditorMode = new System.Windows.Forms.ToolStrip();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnPen = new System.Windows.Forms.ToolStripButton();
            this.btnEraser = new System.Windows.Forms.ToolStripButton();
            this.btnRect = new System.Windows.Forms.ToolStripButton();
            this.btnEllipse = new System.Windows.Forms.ToolStripButton();
            this.btnText = new System.Windows.Forms.ToolStripButton();
            this.btnClock = new System.Windows.Forms.ToolStripButton();
            this.btnTriangle = new System.Windows.Forms.ToolStripButton();
            this.btnRATriangle = new System.Windows.Forms.ToolStripButton();
            this.btnDiamond = new System.Windows.Forms.ToolStripButton();
            this.btnPentagon = new System.Windows.Forms.ToolStripButton();
            this.btnLine = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlViewer = new System.Windows.Forms.Panel();
            this.wfvcEditor = new DDraw.WinForms.WFViewerControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitToPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitToWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._050PcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._100PcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._150PcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.a4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.a5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.letterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFigure = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.previewBar1 = new WinFormsDemo.PreviewBar();
            this.tsFigureProps = new System.Windows.Forms.ToolStrip();
            this.btnFill = new WinFormsDemo.ToolStripColorButton();
            this.btnStroke = new WinFormsDemo.ToolStripColorButton();
            this.btnStrokeWidth = new WinFormsDemo.ToolStripStrokeWidthButton();
            this.btnStrokeStyle = new WinFormsDemo.ToolStripStrokeStyleButton();
            this.btnStartMarker = new WinFormsDemo.ToolStripMarkerButton();
            this.btnEndMarker = new WinFormsDemo.ToolStripMarkerButton();
            this.btnAlpha = new WinFormsDemo.ToolStripAlphaButton();
            this.cbFontName = new WinFormsDemo.ToolStripFontNameChooser();
            this.btnBold = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.btnStrikethrough = new System.Windows.Forms.ToolStripButton();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionListProvider1 = new Burkovsky.Controls.ActionListProvider(this.components);
            this.actSendToBack = new Burkovsky.Controls.Action();
            this.actGroupFigures = new Burkovsky.Controls.Action();
            this.actBringToFront = new Burkovsky.Controls.Action();
            this.actSendBackward = new Burkovsky.Controls.Action();
            this.actBringForward = new Burkovsky.Controls.Action();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.sendToBackToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sendBackwardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bringForwardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlPreviews.SuspendLayout();
            this.tsEditorMode.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlViewer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.cmsFigure.SuspendLayout();
            this.tsFigureProps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionListProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPreviews
            // 
            this.pnlPreviews.Controls.Add(this.previewBar1);
            this.pnlPreviews.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlPreviews.Location = new System.Drawing.Point(459, 0);
            this.pnlPreviews.Name = "pnlPreviews";
            this.pnlPreviews.Size = new System.Drawing.Size(81, 219);
            this.pnlPreviews.TabIndex = 9;
            // 
            // tsEditorMode
            // 
            this.tsEditorMode.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEditorMode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAntiAlias,
            this.toolStripSeparator1,
            this.btnSelect,
            this.btnPen,
            this.btnEraser,
            this.btnRect,
            this.btnEllipse,
            this.btnText,
            this.btnClock,
            this.btnTriangle,
            this.btnRATriangle,
            this.btnDiamond,
            this.btnPentagon,
            this.btnLine});
            this.tsEditorMode.Location = new System.Drawing.Point(3, 24);
            this.tsEditorMode.Name = "tsEditorMode";
            this.tsEditorMode.Size = new System.Drawing.Size(346, 25);
            this.tsEditorMode.TabIndex = 0;
            this.tsEditorMode.Text = "toolStrip1";
            // 
            // btnAntiAlias
            // 
            this.actionListProvider1.SetAction(this.btnAntiAlias, null);
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
            this.actionListProvider1.SetAction(this.toolStripSeparator1, null);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSelect
            // 
            this.actionListProvider1.SetAction(this.btnSelect, null);
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
            this.actionListProvider1.SetAction(this.btnPen, null);
            this.btnPen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPen.Image = ((System.Drawing.Image)(resources.GetObject("btnPen.Image")));
            this.btnPen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPen.Name = "btnPen";
            this.btnPen.Size = new System.Drawing.Size(23, 22);
            this.btnPen.Text = "Pen";
            this.btnPen.Click += new System.EventHandler(this.btnPen_Click);
            // 
            // btnEraser
            // 
            this.actionListProvider1.SetAction(this.btnEraser, null);
            this.btnEraser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEraser.Image = ((System.Drawing.Image)(resources.GetObject("btnEraser.Image")));
            this.btnEraser.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEraser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEraser.Name = "btnEraser";
            this.btnEraser.Size = new System.Drawing.Size(23, 22);
            this.btnEraser.Text = "Eraser";
            this.btnEraser.Click += new System.EventHandler(this.btn_Eraser_Click);
            // 
            // btnRect
            // 
            this.actionListProvider1.SetAction(this.btnRect, null);
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
            this.actionListProvider1.SetAction(this.btnEllipse, null);
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
            this.actionListProvider1.SetAction(this.btnText, null);
            this.btnText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnText.Image = ((System.Drawing.Image)(resources.GetObject("btnText.Image")));
            this.btnText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnText.Name = "btnText";
            this.btnText.Size = new System.Drawing.Size(23, 22);
            this.btnText.Text = "Text";
            this.btnText.Click += new System.EventHandler(this.btnText_Click);
            // 
            // btnClock
            // 
            this.actionListProvider1.SetAction(this.btnClock, null);
            this.btnClock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClock.Image = ((System.Drawing.Image)(resources.GetObject("btnClock.Image")));
            this.btnClock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClock.Name = "btnClock";
            this.btnClock.Size = new System.Drawing.Size(23, 22);
            this.btnClock.Text = "Clock";
            this.btnClock.Click += new System.EventHandler(this.btnClock_Click);
            // 
            // btnTriangle
            // 
            this.actionListProvider1.SetAction(this.btnTriangle, null);
            this.btnTriangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTriangle.Image = ((System.Drawing.Image)(resources.GetObject("btnTriangle.Image")));
            this.btnTriangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnTriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTriangle.Name = "btnTriangle";
            this.btnTriangle.Size = new System.Drawing.Size(23, 22);
            this.btnTriangle.Text = "Triangle";
            this.btnTriangle.Click += new System.EventHandler(this.btnPolygon_Click);
            // 
            // btnRATriangle
            // 
            this.actionListProvider1.SetAction(this.btnRATriangle, null);
            this.btnRATriangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRATriangle.Image = ((System.Drawing.Image)(resources.GetObject("btnRATriangle.Image")));
            this.btnRATriangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRATriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRATriangle.Name = "btnRATriangle";
            this.btnRATriangle.Size = new System.Drawing.Size(23, 22);
            this.btnRATriangle.Text = "RA Triangle";
            this.btnRATriangle.Click += new System.EventHandler(this.btnPolygon_Click);
            // 
            // btnDiamond
            // 
            this.actionListProvider1.SetAction(this.btnDiamond, null);
            this.btnDiamond.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDiamond.Image = ((System.Drawing.Image)(resources.GetObject("btnDiamond.Image")));
            this.btnDiamond.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnDiamond.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDiamond.Name = "btnDiamond";
            this.btnDiamond.Size = new System.Drawing.Size(23, 22);
            this.btnDiamond.Text = "Diamond";
            this.btnDiamond.Click += new System.EventHandler(this.btnPolygon_Click);
            // 
            // btnPentagon
            // 
            this.actionListProvider1.SetAction(this.btnPentagon, null);
            this.btnPentagon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPentagon.Image = ((System.Drawing.Image)(resources.GetObject("btnPentagon.Image")));
            this.btnPentagon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPentagon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPentagon.Name = "btnPentagon";
            this.btnPentagon.Size = new System.Drawing.Size(23, 22);
            this.btnPentagon.Text = "Pentagon";
            this.btnPentagon.Click += new System.EventHandler(this.btnPolygon_Click);
            // 
            // btnLine
            // 
            this.actionListProvider1.SetAction(this.btnLine, null);
            this.btnLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLine.Image = ((System.Drawing.Image)(resources.GetObject("btnLine.Image")));
            this.btnLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(23, 22);
            this.btnLine.Text = "Line";
            this.btnLine.Click += new System.EventHandler(this.btnLine_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pnlMain);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(540, 241);
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
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(540, 219);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlViewer
            // 
            this.pnlViewer.Controls.Add(this.wfvcEditor);
            this.pnlViewer.Controls.Add(this.pnlPreviews);
            this.pnlViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewer.Location = new System.Drawing.Point(0, 0);
            this.pnlViewer.Name = "pnlViewer";
            this.pnlViewer.Size = new System.Drawing.Size(540, 219);
            this.pnlViewer.TabIndex = 10;
            // 
            // wfvcEditor
            // 
            this.wfvcEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wfvcEditor.Location = new System.Drawing.Point(0, 0);
            this.wfvcEditor.Name = "wfvcEditor";
            this.wfvcEditor.Size = new System.Drawing.Size(459, 219);
            this.wfvcEditor.TabIndex = 0;
            this.wfvcEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.wfvcEditor_KeyUp);
            this.wfvcEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.wfvcEditor_KeyDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 219);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(540, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbInfo
            // 
            this.actionListProvider1.SetAction(this.lbInfo, null);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.formatToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(540, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.fileToolStripMenuItem, null);
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.newToolStripMenuItem, null);
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // openToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.openToolStripMenuItem, null);
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.saveToolStripMenuItem, null);
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.saveAsToolStripMenuItem, null);
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // toolStripSeparator3
            // 
            this.actionListProvider1.SetAction(this.toolStripSeparator3, null);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.exitToolStripMenuItem, null);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.editToolStripMenuItem, null);
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpened += new System.EventHandler(this.editToolStripMenuItem_DropDownOpened);
            // 
            // undoToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.undoToolStripMenuItem, null);
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.redoToolStripMenuItem, null);
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.actionListProvider1.SetAction(this.toolStripSeparator4, null);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.cutToolStripMenuItem, null);
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.copyToolStripMenuItem, null);
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.pasteToolStripMenuItem, null);
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.notImplemented_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.viewToolStripMenuItem, null);
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // zoomToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.zoomToolStripMenuItem, null);
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fitToPageToolStripMenuItem,
            this.fitToWidthToolStripMenuItem,
            this._050PcToolStripMenuItem,
            this._100PcToolStripMenuItem,
            this._150PcToolStripMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            this.zoomToolStripMenuItem.DropDownOpening += new System.EventHandler(this.zoomToolStripMenuItem_DropDownOpening);
            // 
            // fitToPageToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.fitToPageToolStripMenuItem, null);
            this.fitToPageToolStripMenuItem.Name = "fitToPageToolStripMenuItem";
            this.fitToPageToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.fitToPageToolStripMenuItem.Text = "Fit to Page";
            this.fitToPageToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // fitToWidthToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.fitToWidthToolStripMenuItem, null);
            this.fitToWidthToolStripMenuItem.Name = "fitToWidthToolStripMenuItem";
            this.fitToWidthToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.fitToWidthToolStripMenuItem.Text = "Fit to Width";
            this.fitToWidthToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // _050PcToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this._050PcToolStripMenuItem, null);
            this._050PcToolStripMenuItem.Name = "_050PcToolStripMenuItem";
            this._050PcToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this._050PcToolStripMenuItem.Text = "50%";
            this._050PcToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // _100PcToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this._100PcToolStripMenuItem, null);
            this._100PcToolStripMenuItem.Name = "_100PcToolStripMenuItem";
            this._100PcToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this._100PcToolStripMenuItem.Text = "100%";
            this._100PcToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // _150PcToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this._150PcToolStripMenuItem, null);
            this._150PcToolStripMenuItem.Name = "_150PcToolStripMenuItem";
            this._150PcToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this._150PcToolStripMenuItem.Text = "150%";
            this._150PcToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.insertToolStripMenuItem, null);
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // imageToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.imageToolStripMenuItem, null);
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.imageToolStripMenuItem.Text = "Image";
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // formatToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.formatToolStripMenuItem, null);
            this.formatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageSizeToolStripMenuItem,
            this.toolStripSeparator5,
            this.groupToolStripMenuItem1,
            this.toolStripSeparator6,
            this.sendToBackToolStripMenuItem1,
            this.bringToFrontToolStripMenuItem1,
            this.sendBackwardToolStripMenuItem1,
            this.bringForwardToolStripMenuItem1});
            this.formatToolStripMenuItem.Name = "formatToolStripMenuItem";
            this.formatToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.formatToolStripMenuItem.Text = "Format";
            // 
            // pageSizeToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.pageSizeToolStripMenuItem, null);
            this.pageSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.a4ToolStripMenuItem,
            this.a5ToolStripMenuItem,
            this.letterToolStripMenuItem,
            this.customToolStripMenuItem});
            this.pageSizeToolStripMenuItem.Name = "pageSizeToolStripMenuItem";
            this.pageSizeToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.pageSizeToolStripMenuItem.Text = "Page Size";
            this.pageSizeToolStripMenuItem.DropDownOpening += new System.EventHandler(this.pageSizeToolStripMenuItem_DropDownOpening);
            // 
            // a4ToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.a4ToolStripMenuItem, null);
            this.a4ToolStripMenuItem.Name = "a4ToolStripMenuItem";
            this.a4ToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.a4ToolStripMenuItem.Text = "A4";
            this.a4ToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // a5ToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.a5ToolStripMenuItem, null);
            this.a5ToolStripMenuItem.Name = "a5ToolStripMenuItem";
            this.a5ToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.a5ToolStripMenuItem.Text = "A5";
            this.a5ToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // letterToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.letterToolStripMenuItem, null);
            this.letterToolStripMenuItem.Name = "letterToolStripMenuItem";
            this.letterToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.letterToolStripMenuItem.Text = "Letter";
            this.letterToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.customToolStripMenuItem, null);
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.PageSizeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.helpToolStripMenuItem, null);
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.aboutToolStripMenuItem, null);
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.cmsFigure.Size = new System.Drawing.Size(239, 120);
            // 
            // toolStripSeparator2
            // 
            this.actionListProvider1.SetAction(this.toolStripSeparator2, null);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(235, 6);
            // 
            // previewBar1
            // 
            this.previewBar1.AutoScroll = true;
            this.previewBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBar1.Location = new System.Drawing.Point(0, 0);
            this.previewBar1.Name = "previewBar1";
            this.previewBar1.Size = new System.Drawing.Size(81, 219);
            this.previewBar1.TabIndex = 0;
            this.previewBar1.PreviewSelected += new WinFormsDemo.PreviewSelectedHandler(this.previewBar1_PreviewSelected);
            this.previewBar1.PreviewAdd += new System.EventHandler(this.previewBar1_PreviewAdd);
            // 
            // tsFigureProps
            // 
            this.tsFigureProps.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFigureProps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFill,
            this.btnStroke,
            this.btnStrokeWidth,
            this.btnStrokeStyle,
            this.btnStartMarker,
            this.btnEndMarker,
            this.btnAlpha,
            this.cbFontName,
            this.btnBold,
            this.btnItalic,
            this.btnUnderline,
            this.btnStrikethrough});
            this.tsFigureProps.Location = new System.Drawing.Point(3, 49);
            this.tsFigureProps.Name = "tsFigureProps";
            this.tsFigureProps.Size = new System.Drawing.Size(371, 25);
            this.tsFigureProps.TabIndex = 2;
            // 
            // btnFill
            // 
            this.actionListProvider1.SetAction(this.btnFill, null);
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
            this.actionListProvider1.SetAction(this.btnStroke, null);
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
            this.actionListProvider1.SetAction(this.btnStrokeWidth, null);
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
            // btnStrokeStyle
            // 
            this.actionListProvider1.SetAction(this.btnStrokeStyle, null);
            this.btnStrokeStyle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStrokeStyle.Image = ((System.Drawing.Image)(resources.GetObject("btnStrokeStyle.Image")));
            this.btnStrokeStyle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrokeStyle.Name = "btnStrokeStyle";
            this.btnStrokeStyle.ShowDropDownArrow = false;
            this.btnStrokeStyle.Size = new System.Drawing.Size(20, 22);
            this.btnStrokeStyle.Text = "Stroke Style";
            this.btnStrokeStyle.Value = DDraw.DStrokeStyle.DashDotDot;
            this.btnStrokeStyle.StrokeStyleChanged += new WinFormsDemo.StrokeStyleChangedHandler(this.btnStrokeStyle_StrokeStyleChanged);
            // 
            // btnStartMarker
            // 
            this.actionListProvider1.SetAction(this.btnStartMarker, null);
            this.btnStartMarker.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStartMarker.Image = ((System.Drawing.Image)(resources.GetObject("btnStartMarker.Image")));
            this.btnStartMarker.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStartMarker.Name = "btnStartMarker";
            this.btnStartMarker.ShowDropDownArrow = false;
            this.btnStartMarker.Size = new System.Drawing.Size(20, 22);
            this.btnStartMarker.Start = true;
            this.btnStartMarker.Text = "toolStripMarkerButton1";
            this.btnStartMarker.Value = DDraw.DMarker.None;
            this.btnStartMarker.MarkerChanged += new WinFormsDemo.MarkerChangedHandler(this.btnMarker_MarkerChanged);
            // 
            // btnEndMarker
            // 
            this.actionListProvider1.SetAction(this.btnEndMarker, null);
            this.btnEndMarker.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEndMarker.Image = ((System.Drawing.Image)(resources.GetObject("btnEndMarker.Image")));
            this.btnEndMarker.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEndMarker.Name = "btnEndMarker";
            this.btnEndMarker.ShowDropDownArrow = false;
            this.btnEndMarker.Size = new System.Drawing.Size(20, 22);
            this.btnEndMarker.Start = false;
            this.btnEndMarker.Text = "toolStripMarkerButton2";
            this.btnEndMarker.Value = DDraw.DMarker.None;
            this.btnEndMarker.MarkerChanged += new WinFormsDemo.MarkerChangedHandler(this.btnMarker_MarkerChanged);
            // 
            // btnAlpha
            // 
            this.actionListProvider1.SetAction(this.btnAlpha, null);
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
            this.actionListProvider1.SetAction(this.cbFontName, null);
            this.cbFontName.Name = "cbFontName";
            this.cbFontName.Size = new System.Drawing.Size(121, 22);
            this.cbFontName.Value = null;
            this.cbFontName.FontNameChanged += new System.EventHandler(this.cbFontName_FontNameChanged);
            // 
            // btnBold
            // 
            this.actionListProvider1.SetAction(this.btnBold, null);
            this.btnBold.CheckOnClick = true;
            this.btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBold.Image = ((System.Drawing.Image)(resources.GetObject("btnBold.Image")));
            this.btnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(23, 22);
            this.btnBold.Text = "Bold";
            this.btnBold.Click += new System.EventHandler(this.btnFontProp_Changed);
            // 
            // btnItalic
            // 
            this.actionListProvider1.SetAction(this.btnItalic, null);
            this.btnItalic.CheckOnClick = true;
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("btnItalic.Image")));
            this.btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(23, 22);
            this.btnItalic.Text = "Italic";
            this.btnItalic.Click += new System.EventHandler(this.btnFontProp_Changed);
            // 
            // btnUnderline
            // 
            this.actionListProvider1.SetAction(this.btnUnderline, null);
            this.btnUnderline.CheckOnClick = true;
            this.btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUnderline.Image = ((System.Drawing.Image)(resources.GetObject("btnUnderline.Image")));
            this.btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(23, 22);
            this.btnUnderline.Text = "Underline";
            this.btnUnderline.Click += new System.EventHandler(this.btnFontProp_Changed);
            // 
            // btnStrikethrough
            // 
            this.actionListProvider1.SetAction(this.btnStrikethrough, null);
            this.btnStrikethrough.CheckOnClick = true;
            this.btnStrikethrough.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStrikethrough.Image = ((System.Drawing.Image)(resources.GetObject("btnStrikethrough.Image")));
            this.btnStrikethrough.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrikethrough.Name = "btnStrikethrough";
            this.btnStrikethrough.Size = new System.Drawing.Size(23, 22);
            this.btnStrikethrough.Text = "Strikethrough";
            this.btnStrikethrough.Click += new System.EventHandler(this.btnFontProp_Changed);
            // 
            // groupToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.groupToolStripMenuItem, this.actGroupFigures);
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            this.groupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.groupToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.groupToolStripMenuItem.Text = "Group";
            // 
            // sendToBackToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.sendToBackToolStripMenuItem, this.actSendToBack);
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            this.sendToBackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Next)));
            this.sendToBackToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.sendToBackToolStripMenuItem.Text = "Send to Back";
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.bringToFrontToolStripMenuItem, this.actBringToFront);
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            this.bringToFrontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.PageUp)));
            this.bringToFrontToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.bringToFrontToolStripMenuItem.Text = "Bring to Front";
            // 
            // sendBackwardToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.sendBackwardToolStripMenuItem, this.actSendBackward);
            this.sendBackwardToolStripMenuItem.Name = "sendBackwardToolStripMenuItem";
            this.sendBackwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Next)));
            this.sendBackwardToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.sendBackwardToolStripMenuItem.Text = "Send Backward";
            // 
            // bringForwardToolStripMenuItem
            // 
            this.actionListProvider1.SetAction(this.bringForwardToolStripMenuItem, this.actBringForward);
            this.bringForwardToolStripMenuItem.Name = "bringForwardToolStripMenuItem";
            this.bringForwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.PageUp)));
            this.bringForwardToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.bringForwardToolStripMenuItem.Text = "Bring Forward";
            // 
            // actionListProvider1
            // 
            this.actionListProvider1.Actions.Add(this.actGroupFigures);
            this.actionListProvider1.Actions.Add(this.actSendToBack);
            this.actionListProvider1.Actions.Add(this.actBringToFront);
            this.actionListProvider1.Actions.Add(this.actSendBackward);
            this.actionListProvider1.Actions.Add(this.actBringForward);
            // 
            // actSendToBack
            // 
            this.actSendToBack.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Next)));
            this.actSendToBack.Text = "Send to Back";
            this.actSendToBack.Execute += new System.EventHandler(this.actSendToBack_Execute);
            // 
            // actGroupFigures
            // 
            this.actGroupFigures.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.actGroupFigures.Text = "Group";
            this.actGroupFigures.Execute += new System.EventHandler(this.actGroupFigures_Execute);
            // 
            // actBringToFront
            // 
            this.actBringToFront.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.PageUp)));
            this.actBringToFront.Text = "Bring to Front";
            this.actBringToFront.Execute += new System.EventHandler(this.actBringToFront_Execute);
            // 
            // actSendBackward
            // 
            this.actSendBackward.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Next)));
            this.actSendBackward.Text = "Send Backward";
            this.actSendBackward.Execute += new System.EventHandler(this.actSendBackward_Execute);
            // 
            // actBringForward
            // 
            this.actBringForward.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.PageUp)));
            this.actBringForward.Text = "Bring Forward";
            this.actBringForward.Execute += new System.EventHandler(this.actBringForward_Execute);
            // 
            // toolStripSeparator5
            // 
            this.actionListProvider1.SetAction(this.toolStripSeparator5, null);
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(155, 6);
            // 
            // groupToolStripMenuItem1
            // 
            this.actionListProvider1.SetAction(this.groupToolStripMenuItem1, this.actGroupFigures);
            this.groupToolStripMenuItem1.Name = "groupToolStripMenuItem1";
            this.groupToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.groupToolStripMenuItem1.Text = "Group";
            // 
            // toolStripSeparator6
            // 
            this.actionListProvider1.SetAction(this.toolStripSeparator6, null);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(155, 6);
            // 
            // sendToBackToolStripMenuItem1
            // 
            this.actionListProvider1.SetAction(this.sendToBackToolStripMenuItem1, this.actSendToBack);
            this.sendToBackToolStripMenuItem1.Name = "sendToBackToolStripMenuItem1";
            this.sendToBackToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.sendToBackToolStripMenuItem1.Text = "Send to Back";
            // 
            // bringToFrontToolStripMenuItem1
            // 
            this.actionListProvider1.SetAction(this.bringToFrontToolStripMenuItem1, this.actBringToFront);
            this.bringToFrontToolStripMenuItem1.Name = "bringToFrontToolStripMenuItem1";
            this.bringToFrontToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.bringToFrontToolStripMenuItem1.Text = "Bring to Front";
            // 
            // sendBackwardToolStripMenuItem1
            // 
            this.actionListProvider1.SetAction(this.sendBackwardToolStripMenuItem1, this.actSendBackward);
            this.sendBackwardToolStripMenuItem1.Name = "sendBackwardToolStripMenuItem1";
            this.sendBackwardToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.sendBackwardToolStripMenuItem1.Text = "Send Backward";
            // 
            // bringForwardToolStripMenuItem1
            // 
            this.actionListProvider1.SetAction(this.bringForwardToolStripMenuItem1, this.actBringForward);
            this.bringForwardToolStripMenuItem1.Name = "bringForwardToolStripMenuItem1";
            this.bringForwardToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.bringForwardToolStripMenuItem1.Text = "Bring Forward";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 315);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "WinFormsDemo";
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
            this.cmsFigure.ResumeLayout(false);
            this.tsFigureProps.ResumeLayout(false);
            this.tsFigureProps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionListProvider1)).EndInit();
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
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnPen;
        private PreviewBar previewBar1;
        private System.Windows.Forms.ToolStripButton btnRect;
        private System.Windows.Forms.ToolStripButton btnEllipse;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
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
        private System.Windows.Forms.ToolStrip tsFigureProps;
        private ToolStripColorButton btnFill;
        private ToolStripColorButton btnStroke;
        private ToolStripStrokeWidthButton btnStrokeWidth;
        private ToolStripAlphaButton btnAlpha;
        private ToolStripFontNameChooser cbFontName;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnClock;
        private System.Windows.Forms.ToolStripButton btnTriangle;
        private System.Windows.Forms.ToolStripButton btnRATriangle;
        private System.Windows.Forms.ToolStripButton btnDiamond;
        private System.Windows.Forms.ToolStripButton btnPentagon;
        private System.Windows.Forms.ToolStripButton btnLine;
        private ToolStripStrokeStyleButton btnStrokeStyle;
        private ToolStripMarkerButton btnStartMarker;
        private ToolStripMarkerButton btnEndMarker;
        private System.Windows.Forms.ToolStripButton btnBold;
        private System.Windows.Forms.ToolStripButton btnItalic;
        private System.Windows.Forms.ToolStripButton btnUnderline;
        private System.Windows.Forms.ToolStripButton btnStrikethrough;
        private System.Windows.Forms.ToolStripButton btnEraser;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitToPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitToWidthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _050PcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _100PcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _150PcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pageSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem a4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem a5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem letterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private Burkovsky.Controls.ActionListProvider actionListProvider1;
        private Burkovsky.Controls.Action actGroupFigures;
        private Burkovsky.Controls.Action actSendToBack;
        private Burkovsky.Controls.Action actBringToFront;
        private Burkovsky.Controls.Action actSendBackward;
        private Burkovsky.Controls.Action actBringForward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem sendToBackToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bringToFrontToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sendBackwardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bringForwardToolStripMenuItem1;
    }
}

