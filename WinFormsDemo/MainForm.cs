using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public partial class MainForm : Form
    {
        DEngineManager dem;
        DAuthorProperties dap;
        DEngine de = null;

        DTkViewer dvEditor;

        BitmapGlyph contextGlyph;

        bool beenSaved;
        string fileName;

        const string ProgramName = "WinFormsDemo";
        const string FileExt = ".ddraw";
        const string FileTypeFilter = "DDraw files|*.ddraw";

        void CreateDEngine(DEngine sibling)
        {
            DEngine de = new DEngine(dap, true);
            if (sibling != null)
            {
                int idx = dem.IndexOfEngine(sibling);
                if (idx >= 0)
                    dem.InsertEngine(idx + 1, de);
                else
                    dem.AddEngine(de);
            }
            else
                dem.AddEngine(de);
            de.PageSize = new DPoint(500, 400);
            InitDEngine(de);
        }

        void InitDEngine(DEngine de)
        {
            // DEngine properties
            de.SimplifyPolylines = true;
            de.SimplifyPolylinesTolerance = 0.5;
            de.AutoGroupPolylines = true;
            de.AutoGroupPolylinesTimeout = 2000;
            de.AutoGroupPolylinesXLimit = 100;
            de.AutoGroupPolylinesYLimit = 50;
            // DEngine events
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
            de.ContextClick += new ContextClickHandler(de_ContextClick);
            de.HsmStateChanged += new HsmStateChangedHandler(de_HsmStateChanged);
            de.AddedFigure += new AddedFigureHandler(de_AddedFigure);
            // add glyphs to figures
            foreach (Figure f in de.Figures)
                AddGlyphs(f);
            // show it
            SetCurrentDe(de);
        }

        private void SetCurrentDe(DEngine de)
        {
            if (this.de != null)
            {
                this.de.RemoveViewer(dvEditor);
                if (this.de.CurrentFigureClass != null)
                    de.HsmSetStateByFigureClass(this.de.CurrentFigureClass);
                else
                    de.HsmState = this.de.HsmState;
            }
            de.AddViewer(dvEditor);
            if (dvEditor.Zoom != Zoom.Custom)
                dvEditor.Zoom = dvEditor.Zoom; 
            dvEditor.Update();
            this.de = de;
            de_SelectedFiguresChanged();
            UpdateUndoRedoControls();
        }

        public MainForm()
        {
            InitializeComponent();
            // Initialze DGraphics
            WFGraphics.Init();
            // DEngine Manager
            dem = new DEngineManager();
            dem.UndoRedoChanged += new EventHandler(dem_UndoRedoChanged);
            // create author properties
            dap = new DAuthorProperties(DColor.Blue, DColor.Red, 3, DStrokeStyle.Solid, DMarker.None, DMarker.None, 1, "Arial", false, false, false, false);
            // edit viewer
            dvEditor = new WFViewer(wfvcEditor);
            dvEditor.EditFigures = true;
            dvEditor.DebugMessage += new DebugMessageHandler(DebugMessage);
            // glyphs
            MemoryStream ms = new MemoryStream();
            Resource1.arrow.Save(ms, ImageFormat.Png);
            contextGlyph = new BitmapGlyph(new WFBitmap(ms));
            ms.Dispose();
            contextGlyph.Clicked += new GlyphClickedHandler(contextGlyph_Clicked);
            // figure defaults
            Figure._handleSize = 5;
            Figure._handleBorder = 3;
            Figure._rotateHandleStemLength -= Figure._handleBorder;
            // new document
            New();
            dem.UndoRedoClearHistory();
            // create initial figures
            de.UndoRedoStart("create initial figures");
            // rect figures
            de.AddFigure(new RectFigure(new DRect(10, 10, 50, 50), 0));
            Figure f = new RectFigure(new DRect(40, 40, 50, 50), 0);
            ((RectFigure)f).Fill = new DColor(64, 64, 255, 128);
            de.AddFigure(f);
            // ellipse figure
            f = new EllipseFigure(new DRect(120, 20, 100, 50), 0);
            de.AddFigure(f);
            // polyline figure
            DPoints pts = new DPoints();
            pts.Add(new DPoint(150, 80));
            pts.Add(new DPoint(160, 70));
            pts.Add(new DPoint(170, 100));
            pts.Add(new DPoint(180, 80));
            f = new PolylineFigure(pts);
            de.AddFigure(f);
            // bitmap images
            byte[] imageData = (byte[])TypeDescriptor.GetConverter(Resource1.technocolor).ConvertTo(Resource1.technocolor, typeof(byte[]));
            f = new ImageFigure(new DRect(250, 50, 24, 16), 0, imageData, "technocolor.png");
            de.AddFigure(f);
            f = new ImageFigure(new DRect(150, 150, 39, 50), 0, imageData, "technocolor.png");
            f.LockAspectRatio = true;
            de.AddFigure(f);
            // text figure
            f = new TextFigure(new DPoint(100, 200), "hello\ndan", 0);
            de.AddFigure(f);
            // compositing figure
            f = new CompositedExampleFigure();
            f.Rect = new DRect(20, 150, 50, 50);
            de.AddFigure(f);
            // clock (IEditable) figure
            f = new ClockFigure();
            f.Rect = new DRect(200, 200, 100, 100);
            de.AddFigure(f);
            // triangle polygon
            f = new TriangleFigure();
            f.Rect = new DRect(100, 200, 100, 100);
            de.AddFigure(f);
            // line figure
            f = new LineFigure(new DPoint(100, 100), new DPoint(200, 200));
            ((LineFigure)f).StrokeWidth = 10;
            de.AddFigure(f);
            // commit figures to undo redo manager
            de.UndoRedoCommit();
            de.UndoRedoClearHistory();
            UpdateUndoRedoControls();
            UpdateTitleBar();
            // Init controls
            InitPropertyControls(de.HsmState);
            // read program options
            ReadOptions();
        }

        void ReadOptions()
        {
            ProgramOptions options = new ProgramOptions();
            // MainForm options
            SetBounds(options.FormRect.Left, options.FormRect.Top, options.FormRect.Width, options.FormRect.Height);
            if (options.FormWindowState != FormWindowState.Minimized)
                WindowState = options.FormWindowState;
            // dvEditor options
            dvEditor.AntiAlias = options.AntiAlias;
            if (options.Zoom == Zoom.Custom)
                dvEditor.Scale = options.Scale;
            else
                dvEditor.Zoom = options.Zoom;
        }

        void WriteOptions()
        {
            ProgramOptions options = new ProgramOptions();
            // MainForm options
            if (WindowState != FormWindowState.Normal)
                options.FormRect = RestoreBounds;
            else
                options.FormRect = new Rectangle(Left, Top, Width, Height);
            options.FormWindowState = WindowState;
            // dvEditor options
            options.Zoom = dvEditor.Zoom;
            options.Scale = dvEditor.Scale;
            options.AntiAlias = dvEditor.AntiAlias;
            // write to file
            options.WriteIni();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckDirty())
                e.Cancel = true;
            else
                WriteOptions();
        }

        void DebugMessage(string msg)
        {
            lbInfo.Text = msg;
        }

        void de_SelectedFiguresChanged()
        {
            InitPropertyControls(de.HsmState);
            InitActions();
        }

        void UpdateUndoRedoControls()
        {
            undoToolStripMenuItem.Enabled = dem.CanUndo(de);
            if (undoToolStripMenuItem.Enabled)
                undoToolStripMenuItem.Text = string.Format("Undo \"{0}\"", dem.UndoCaption(de));
            else
                undoToolStripMenuItem.Text = "Undo";
            redoToolStripMenuItem.Enabled = dem.CanRedo(de);
            if (redoToolStripMenuItem.Enabled)
                redoToolStripMenuItem.Text = string.Format("Redo \"{0}\"", dem.RedoCaption(de));
            else
                redoToolStripMenuItem.Text = "Redo";
        }

        void dem_UndoRedoChanged(object sender, EventArgs e)
        {
            UpdateUndoRedoControls();
            UpdateTitleBar();
            if (sender == dem)
                previewBar1.MatchPreviewsToEngines(dem.GetEngines(), dvEditor);
            previewBar1.UpdatePreviewsDirtyProps();
        }

        void de_ContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (clickedFigure != null)
                cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
            else
                cmsCanvas.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
        }

        void contextGlyph_Clicked(IGlyph glyph, Figure figure, DPoint pt)
        {
            cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
        }

        Color GetFillMatch(List<Figure> figs)
        {
            Color fill = Color.Empty;
            foreach (Figure f in figs)
                if (f is IFillable)
                {
                    fill = WFHelper.MakeColor(((IFillable)f).Fill);
                    break;
                }
            if (fill != Color.Empty)
                foreach (Figure f in figs)
                    if (f is IFillable)
                    {
                        if (fill != WFHelper.MakeColor(((IFillable)f).Fill))
                            return Color.Empty;
                    }
            return fill;
        }

        Color GetStrokeMatch(List<Figure> figs)
        {
            Color stroke = Color.Empty;
            foreach (Figure f in figs)
                if (f is IStrokeable)
                {
                    stroke = WFHelper.MakeColor(((IStrokeable)f).Stroke);
                    break;
                }
            if (stroke != Color.Empty)
                foreach (Figure f in figs)
                    if (f is IStrokeable)
                    {
                        if (stroke != WFHelper.MakeColor(((IStrokeable)f).Stroke))
                            return Color.Empty;
                    }
            return stroke;
        }

        double GetStrokeWidthMatch(List<Figure> figs)
        {
            double strokeWidth = ToolStripStrokeWidthButton.Empty;
            foreach (Figure f in figs)
                if (f is IStrokeable)
                {
                    strokeWidth = ((IStrokeable)f).StrokeWidth;
                    break;
                }
            if (strokeWidth != ToolStripStrokeWidthButton.Empty)
                foreach (Figure f in figs)
                    if (f is IStrokeable)
                    {
                        if (strokeWidth != ((IStrokeable)f).StrokeWidth)
                            return ToolStripStrokeWidthButton.Empty;
                    }
            return strokeWidth;
        }

        DStrokeStyle GetStrokeStyleMatch(List<Figure> figs)
        {
            DStrokeStyle strokeStyle = DStrokeStyle.Solid;
            foreach (Figure f in figs)
                if (f is IStrokeable)
                {
                    strokeStyle = ((IStrokeable)f).StrokeStyle;
                    break;
                }
            if (strokeStyle != DStrokeStyle.Solid)
                foreach (Figure f in figs)
                    if (f is IStrokeable)
                    {
                        if (strokeStyle != ((IStrokeable)f).StrokeStyle)
                            return DStrokeStyle.Solid;
                    }
            return strokeStyle;
        }

        DMarker GetMarkerMatch(List<Figure> figs, bool start)
        {
            DMarker marker = DMarker.None;
            foreach (Figure f in figs)
                if (f is IMarkable)
                {
                    if (start)
                        marker = ((IMarkable)f).StartMarker;
                    else
                        marker = ((IMarkable)f).EndMarker;
                    break;
                }
            if (marker != DMarker.None)
                foreach (Figure f in figs)
                    if (f is IMarkable)
                    {
                        if (start)
                        {
                            if (marker != ((IMarkable)f).StartMarker)
                                return DMarker.None;
                        }
                        else
                        {
                            if (marker != ((IMarkable)f).EndMarker)
                                return DMarker.None;
                        }
                    }
            return marker;
        }

        double GetAlphaMatch(List<Figure> figs)
        {
            double alpha = ToolStripAlphaButton.Empty;
            foreach (Figure f in figs)
                if (f is IAlphaBlendable)
                {
                    alpha = ((IAlphaBlendable)f).Alpha;
                    break;
                }
            if (alpha != ToolStripAlphaButton.Empty)
                foreach (Figure f in figs)
                    if (f is IAlphaBlendable)
                    {
                        if (alpha != ((IAlphaBlendable)f).Alpha)
                            return ToolStripAlphaButton.Empty;
                    }
            return alpha;
        }

        string GetFontNameMatch(List<Figure> figs)
        {
            string fontName = null;
            foreach (Figure f in figs)
                if (f is ITextable)
                {
                    fontName = ((ITextable)f).FontName;
                    break;
                }
            if (fontName != null)
                foreach (Figure f in figs)
                    if (f is ITextable)
                    {
                        if (fontName != ((ITextable)f).FontName)
                            return null;
                    }
            return fontName;
        }

        bool GetBoldMatch(List<Figure> figs)
        {
            bool bold = false;
            foreach (Figure f in figs)
                if (f is ITextable)
                {
                    bold = ((ITextable)f).Bold;
                    break;
                }
            if (bold != false)
                foreach (Figure f in figs)
                    if (f is ITextable)
                    {
                        if (bold != ((ITextable)f).Bold)
                            return false;
                    }
            return bold;
        }

        bool GetItalicMatch(List<Figure> figs)
        {
            bool italic = false;
            foreach (Figure f in figs)
                if (f is ITextable)
                {
                    italic = ((ITextable)f).Italics;
                    break;
                }
            if (italic != false)
                foreach (Figure f in figs)
                    if (f is ITextable)
                    {
                        if (italic != ((ITextable)f).Italics)
                            return false;
                    }
            return italic;
        }

        bool GetUnderlineMatch(List<Figure> figs)
        {
            bool underline = false;
            foreach (Figure f in figs)
                if (f is ITextable)
                {
                    underline = ((ITextable)f).Underline;
                    break;
                }
            if (underline != false)
                foreach (Figure f in figs)
                    if (f is ITextable)
                    {
                        if (underline != ((ITextable)f).Underline)
                            return false;
                    }
            return underline;
        }

        bool GetStrikethroughMatch(List<Figure> figs)
        {
            bool strikethrough = false;
            foreach (Figure f in figs)
                if (f is ITextable)
                {
                    strikethrough = ((ITextable)f).Strikethrough;
                    break;
                }
            if (strikethrough != false)
                foreach (Figure f in figs)
                    if (f is ITextable)
                    {
                        if (strikethrough != ((ITextable)f).Strikethrough)
                            return false;
                    }
            return strikethrough;
        }

        private void InitPropertyControls(DHsmState state)
        {
            // disable events
            cbFontName.FontNameChanged -= cbFontName_FontNameChanged;
            // set default (blank) values for property controls
            btnFill.Color = Color.Empty;
            btnStroke.Color = Color.Empty;
            btnStrokeWidth.Value = ToolStripStrokeWidthButton.Empty;
            btnStrokeStyle.Value = DStrokeStyle.Solid;
            btnStartMarker.Value = DMarker.None;
            btnEndMarker.Value = DMarker.None;
            btnAlpha.Value = ToolStripAlphaButton.Empty;
            cbFontName.Value = "";
            btnBold.Checked = false;
            btnItalic.Checked = false;
            btnUnderline.Checked = false;
            btnStrikethrough.Checked = false;
            // deselect controls
            btnFill.Enabled = false;
            btnStroke.Enabled = false;
            btnStrokeWidth.Enabled = false;
            btnStrokeStyle.Enabled = false;
            btnStartMarker.Enabled = false;
            btnEndMarker.Enabled = false;
            btnAlpha.Enabled = false;
            cbFontName.Enabled = false;
            btnBold.Enabled = false;
            btnItalic.Enabled = false;
            btnUnderline.Enabled = false;
            btnStrikethrough.Enabled = false;
            // update controls based on the state of DEngine
            switch (state)
            {
                case DHsmState.Select:
                    // get selected figures
                    List<Figure> figs = de.SelectedFigures;
                    // test to enable property controls
                    foreach (Figure f in figs)
                        if (f is IFillable)
                            btnFill.Enabled = true;
                    foreach (Figure f in figs)
                        if (f is IStrokeable)
                        {
                            btnStroke.Enabled = true;
                            btnStrokeWidth.Enabled = true;
                            btnStrokeStyle.Enabled = true;
                        }
                    foreach (Figure f in figs)
                        if (f is IMarkable)
                        {
                            btnStartMarker.Enabled = true;
                            btnEndMarker.Enabled = true;
                        }
                    foreach (Figure f in figs)
                        if (f is IAlphaBlendable)
                            btnAlpha.Enabled = true;
                    foreach (Figure f in figs)
                        if (f is ITextable)
                        {
                            cbFontName.Enabled = true;
                            btnBold.Enabled = true;
                            btnItalic.Enabled = true;
                            btnUnderline.Enabled = true;
                            btnStrikethrough.Enabled = true;
                        }
                    // set property controls to match selected figure/s
                    if (figs.Count > 0)
                    {
                        btnFill.Color = GetFillMatch(figs);
                        btnStroke.Color = GetStrokeMatch(figs);
                        btnStrokeWidth.Value = (int)Math.Round(GetStrokeWidthMatch(figs));
                        btnStrokeStyle.Value = GetStrokeStyleMatch(figs);
                        btnStartMarker.Value = GetMarkerMatch(figs, true);
                        btnEndMarker.Value = GetMarkerMatch(figs, false);
                        btnAlpha.Value = GetAlphaMatch(figs);
                        cbFontName.Value = GetFontNameMatch(figs);
                        btnBold.Checked = GetBoldMatch(figs);
                        btnItalic.Checked = GetItalicMatch(figs);
                        btnUnderline.Checked = GetUnderlineMatch(figs);
                        btnStrikethrough.Checked = GetStrikethroughMatch(figs);
                    }
                    break;
                default:
                    // enable relavant controls and update values to match dap
                    if (de.HsmCurrentFigClassImpls(typeof(IFillable)))
                    {
                        btnFill.Enabled = true;
                        btnFill.Color = WFHelper.MakeColor(dap.Fill);
                    }
                    if (de.HsmCurrentFigClassImpls(typeof(IStrokeable)))
                    {
                        btnStroke.Enabled = true;
                        btnStroke.Color = WFHelper.MakeColor(dap.Stroke);
                        btnStrokeWidth.Enabled = true;
                        btnStrokeWidth.Value = (int)dap.StrokeWidth;
                        btnStrokeStyle.Enabled = true;
                        btnStrokeStyle.Value = dap.StrokeStyle;
                    }
                    if (de.HsmCurrentFigClassImpls(typeof(IMarkable)))
                    {
                        btnStartMarker.Enabled = true;
                        btnStartMarker.Value = dap.StartMarker;
                        btnEndMarker.Enabled = true;
                        btnEndMarker.Value = dap.EndMarker;
                    }
                    if (de.HsmCurrentFigClassImpls(typeof(IAlphaBlendable)))
                    {
                        btnAlpha.Enabled = true;
                        btnAlpha.Value = dap.Alpha;
                    }
                    if (de.HsmCurrentFigClassImpls(typeof(ITextable)))
                    {
                        cbFontName.Enabled = true;
                        cbFontName.Value = dap.FontName;
                        btnBold.Enabled = true;
                        btnBold.Checked = dap.Bold;
                        btnItalic.Enabled = true;
                        btnItalic.Checked = dap.Italics;
                        btnUnderline.Enabled = true;
                        btnUnderline.Checked = dap.Underline;
                        btnStrikethrough.Enabled = true;
                        btnStrikethrough.Checked = dap.Strikethrough;
                    }                        
                    break;
            }
            // re-enable events
            cbFontName.FontNameChanged += cbFontName_FontNameChanged;
        }

        void InitActions()
        {
            List<Figure> figs = de.SelectedFigures;
            // update group action
            actGroupFigures.Enabled = true;
            if (de.CanUngroupFigures(figs))
                actGroupFigures.Text = "Ungroup";
            else if (de.CanGroupFigures(figs))
                actGroupFigures.Text = "Group";
            else
                actGroupFigures.Enabled = false;
            // update order menu actions
            actSendToBack.Enabled = de.CanSendBackward(figs);
            actBringToFront.Enabled = de.CanBringForward(figs);
            actSendBackward.Enabled = de.CanSendBackward(figs);
            actBringForward.Enabled = de.CanBringForward(figs);
            // update cut/copy/delete actions
            actCut.Enabled = de.CanCopy(figs);
            actCopy.Enabled = de.CanCopy(figs);
            actDelete.Enabled = de.CanDelete(figs);
        }

        void AddGlyphs(Figure fig)
        {
            // make sure fig.Glyphs is assigned
            if (fig.Glyphs == null)
                fig.Glyphs = new List<GlyphPair>();
            // add context glyph if not already there
            foreach (GlyphPair gp in fig.Glyphs)
                if (gp.Glyph == contextGlyph)
                    return;
            GlyphPair contextGlyphPair = new GlyphPair(contextGlyph, DGlyphPosition.TopRight);
            if (fig is LineSegmentbaseFigure) // if line segment then place context glyph at center
                contextGlyphPair.Position = DGlyphPosition.Center;
            fig.Glyphs.Add(contextGlyphPair);
        }

        void de_HsmStateChanged(DEngine de, DHsmState state)
        {
            btnSelect.Checked = state == DHsmState.Select;
            btnPen.Checked = de.HsmCurrentFigClassIs(typeof(PolylineFigure));
            btnRect.Checked = de.HsmCurrentFigClassIs(typeof(RectFigure));
            btnEllipse.Checked = de.HsmCurrentFigClassIs(typeof(EllipseFigure));
            btnText.Checked = state == DHsmState.DrawText;
            btnClock.Checked = de.HsmCurrentFigClassIs(typeof(ClockFigure));
            btnTriangle.Checked = de.HsmCurrentFigClassIs(typeof(TriangleFigure));
            btnRATriangle.Checked = de.HsmCurrentFigClassIs(typeof(RightAngleTriangleFigure));
            btnDiamond.Checked = de.HsmCurrentFigClassIs(typeof(DiamondFigure));
            btnPentagon.Checked = de.HsmCurrentFigClassIs(typeof(PentagonFigure));
            btnLine.Checked = de.HsmCurrentFigClassIs(typeof(LineFigure));
            btnEraser.Checked = state == DHsmState.Eraser;
            InitPropertyControls(state);
        }

        void de_AddedFigure(DEngine de, Figure fig)
        {
            AddGlyphs(fig);
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            Point pt = new Point(btnFill.Bounds.Left, btnFill.Bounds.Bottom);
            pt = btnFill.Owner.PointToScreen(pt);
            ColorPicker f = new ColorPicker(pt.X, pt.Y);
            f.ColorSelected += delegate(object sender2, EventArgs ea)
            {
                btnFill.Color = ((ColorPicker)sender2).SelectedColor;
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnFill);
                        break;
                    default:
                        dap.Fill = WFHelper.MakeColor(btnFill.Color);
                        break;
                }
            };
            f.Show();
        }

        private void btnStroke_Click(object sender, EventArgs e)
        {
            Point pt = new Point(btnStroke.Bounds.Left, btnStroke.Bounds.Bottom);
            pt = btnStroke.Owner.PointToScreen(pt);
            ColorPicker f = new ColorPicker(pt.X, pt.Y);
            f.ColorSelected += delegate(object sender2, EventArgs ea)
            {
                btnStroke.Color = ((ColorPicker)sender2).SelectedColor;
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnStroke);
                        break;
                    default:
                        dap.Stroke = WFHelper.MakeColor(btnStroke.Color);
                        break;
                }
            };
            f.Show();
        }

        private void btnAlpha_AlphaChanged(object sender, double alpha)
        {
            switch (de.HsmState)
            {
                case DHsmState.Select:
                    UpdateSelectedFigures(btnAlpha);
                    break;
                default:
                    dap.Alpha = btnAlpha.Value;
                    break;
            }
        }

        private void btnStrokeWidth_StrokeWidthChanged(object sender, int strokeWidth)
        {
            switch (de.HsmState)
            {
                case DHsmState.Select:
                    UpdateSelectedFigures(btnStrokeWidth);
                    break;
                default:
                    dap.StrokeWidth = btnStrokeWidth.Value;
                    break;
            }
        }

        private void btnStrokeStyle_StrokeStyleChanged(object sender, DStrokeStyle strokeStyle)
        {
            switch (de.HsmState)
            {
                case DHsmState.Select:
                    UpdateSelectedFigures(btnStrokeStyle);
                    break;
                default:
                    dap.StrokeStyle = btnStrokeStyle.Value;
                    break;
            }
        }

        private void btnMarker_MarkerChanged(object sender, DMarker marker)
        {
            if (sender == btnStartMarker)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnStartMarker);
                        break;
                    default:
                        dap.StartMarker = btnStartMarker.Value;
                        break;
                }
            else if (sender == btnEndMarker)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnEndMarker);
                        break;
                    default:
                        dap.EndMarker = btnEndMarker.Value;
                        break;
                }
        }

        private void cbFontName_FontNameChanged(object sender, EventArgs e)
        {
            switch (de.HsmState)
            {
                case DHsmState.Select:
                    UpdateSelectedFigures(cbFontName);
                    break;
                default:
                    dap.FontName = cbFontName.Value;
                    break;
            }
        }

        private void btnFontProp_Changed(object sender, EventArgs e)
        {
            if (sender == btnBold)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnBold);
                        break;
                    default:
                        dap.Bold = btnBold.Checked;
                        break;
                }
            if (sender == btnItalic)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnItalic);
                        break;
                    default:
                        dap.Italics = btnItalic.Checked;
                        break;
                }
            if (sender == btnUnderline)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnUnderline);
                        break;
                    default:
                        dap.Underline = btnUnderline.Checked;
                        break;
                }
            if (sender == btnStrikethrough)
                switch (de.HsmState)
                {
                    case DHsmState.Select:
                        UpdateSelectedFigures(btnStrikethrough);
                        break;
                    default:
                        dap.Strikethrough = btnStrikethrough.Checked;
                        break;
                }
        }

        private void UpdateSelectedFigures(object sender)
        {
            de.UndoRedoStart("Change Property");
            List<Figure> figs = de.SelectedFigures;
            foreach (Figure f in figs)
            {
                if (sender == btnFill && f is IFillable)
                    ((IFillable)f).Fill = WFHelper.MakeColor(btnFill.Color);
                if (f is IStrokeable)
                {
                    if (sender == btnStroke)
                        ((IStrokeable)f).Stroke = WFHelper.MakeColor(btnStroke.Color);
                    if (sender == btnStrokeWidth)
                        ((IStrokeable)f).StrokeWidth = btnStrokeWidth.Value;
                    if (sender == btnStrokeStyle)
                        ((IStrokeable)f).StrokeStyle = btnStrokeStyle.Value;
                }
                if (f is IMarkable)
                {
                    if (sender == btnStartMarker)
                        ((IMarkable)f).StartMarker = btnStartMarker.Value;
                    if (sender == btnEndMarker)
                        ((IMarkable)f).EndMarker = btnEndMarker.Value;
                }
                if (sender == btnAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha = btnAlpha.Value;
                if (f is ITextable)
                {
                    if (sender == cbFontName)
                        ((ITextable)f).FontName = cbFontName.Value;
                    if (sender == btnBold)
                        ((ITextable)f).Bold = btnBold.Checked;
                    if (sender == btnItalic)
                        ((ITextable)f).Italics = btnItalic.Checked;
                    if (sender == btnUnderline)
                        ((ITextable)f).Underline = btnUnderline.Checked;
                    if (sender == btnStrikethrough)
                        ((ITextable)f).Strikethrough = btnStrikethrough.Checked;
                }
            }
            de.UndoRedoCommit();
            dvEditor.Update();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            de.HsmState = DHsmState.Select;
        }

        private void btnPen_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(PolylineFigure));
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(RectFigure));
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(EllipseFigure));
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(TextFigure));
        }

        private void btnClock_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(ClockFigure));
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            if (sender == btnTriangle)
                de.HsmSetStateByFigureClass(typeof(TriangleFigure));
            else if (sender == btnRATriangle)
                de.HsmSetStateByFigureClass(typeof(RightAngleTriangleFigure));
            else if (sender == btnDiamond)
                de.HsmSetStateByFigureClass(typeof(DiamondFigure));
            else if (sender == btnPentagon)
                de.HsmSetStateByFigureClass(typeof(PentagonFigure));
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            de.HsmSetStateByFigureClass(typeof(LineFigure));
        }

        private void btn_Eraser_Click(object sender, EventArgs e)
        {
            de.HsmState = DHsmState.Eraser;
            de.SetEraserSize(25);
        }

        private void previewBar1_PreviewSelected(Preview p)
        {
            SetCurrentDe(p.DEngine);
        }

        private void previewBar1_PreviewAdd(object sender, EventArgs e)
        {
            actNewPage_Execute(sender, e);
        }

        private void previewBar1_PreviewContext(Preview p, Point pt)
        {
            cmsPreview.Show(p, pt);
        }

        private void previewBar1_PreviewMove(Preview p, Preview to)
        {
            dem.UndoRedoStart("Move Page");
            dem.MoveEngine(p.DEngine, to.DEngine);
            dem.UndoRedoCommit();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                de.ClearSelected();
                WFBitmap bmp = new WFBitmap(ofd.FileName);
				de.UndoRedoStart("Add Image");
                byte[] imageData = (byte[])TypeDescriptor.GetConverter(bmp.NativeBmp).ConvertTo(bmp.NativeBmp, typeof(byte[]));
                de.AddFigure(new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, imageData, ofd.FileName));
				de.UndoRedoCommit();
                de.UpdateViewers();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
        }

        private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (de.HsmState != DHsmState.Select)
                de.HsmState = DHsmState.Select;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dem.Undo(de);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dem.Redo(de);
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // zoom
            fitToPageToolStripMenuItem.Checked = dvEditor.Zoom == Zoom.FitToPage;
            fitToWidthToolStripMenuItem.Checked = dvEditor.Zoom == Zoom.FitToWidth;
            _050PcToolStripMenuItem.Checked = dvEditor.Scale == 0.5;
            _100PcToolStripMenuItem.Checked = dvEditor.Scale == 1.0;
            _150PcToolStripMenuItem.Checked = dvEditor.Scale == 1.5;
            // anti alias
            antialiasToolStripMenuItem.Checked = dvEditor.AntiAlias;
        }

        private void ZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == fitToPageToolStripMenuItem)
                dvEditor.Zoom = Zoom.FitToPage;
            else if (sender == fitToWidthToolStripMenuItem)
                dvEditor.Zoom = Zoom.FitToWidth;
            else if (sender == _050PcToolStripMenuItem)
                dvEditor.Scale = 0.5;
            else if (sender == _100PcToolStripMenuItem)
                dvEditor.Scale = 1.0;
            else if (sender == _150PcToolStripMenuItem)
                dvEditor.Scale = 1.5;
        }

        private void antialiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dvEditor.AntiAlias = !dvEditor.AntiAlias;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void wfvcEditor_KeyDown(object sender, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
        }

        private void wfvcEditor_KeyUp(object sender, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
        }

        // action methods //

        private void actGroupFigures_Execute(object sender, EventArgs e)
        {
            List<Figure> figs = new List<Figure>(de.SelectedFigures);
            if (de.CanUngroupFigures(figs))
                de.UngroupFigures(figs);
            else if (de.CanGroupFigures(figs))
                de.GroupFigures(figs);
            InitActions();
        }

        private void actSendToBack_Execute(object sender, EventArgs e)
        {
            de.SendToBack(de.SelectedFigures);
            InitActions();
        }

        private void actBringToFront_Execute(object sender, EventArgs e)
        {
            de.BringToFront(de.SelectedFigures);
            InitActions();
        }

        private void actSendBackward_Execute(object sender, EventArgs e)
        {
            de.SendBackward(de.SelectedFigures);
            InitActions();
        }

        private void actBringForward_Execute(object sender, EventArgs e)
        {
            de.BringForward(de.SelectedFigures);
            InitActions();
        }

        private void actPageSize_Execute(object sender, EventArgs e)
        {
            CustomPageSizeForm f = new CustomPageSizeForm();
            f.PageSize = de.PageSize;
            f.PageFormat = de.PageFormat;
            if (f.ShowDialog() == DialogResult.OK)
            {
                de.UndoRedoStart("Change Page Size");
                if (f.PageFormat == PageFormat.Custom)
                    de.PageSize = f.PageSize;
                else
                    de.PageFormat = f.PageFormat;
                de.UndoRedoCommit();
            }
        }

        private void actBackground_Execute(object sender, EventArgs e)
        {
            BackgroundForm bf = new BackgroundForm();
            bf.BackgroundFigure = de.GetBackgroundFigure();
            de.UndoRedoStart("Set Background");
            if (bf.ShowDialog() == DialogResult.OK)
            {
                de.SetBackgroundFigure(bf.BackgroundFigure);
                de.UndoRedoCommit();
            }
            else
                de.UndoRedoCancel();
        }

        private void actCut_Execute(object sender, EventArgs e)
        {
            List<Figure> figs = de.SelectedFigures;
            DBitmap bmp;
            string data = de.Cut(figs, out bmp, dvEditor.AntiAlias);
            CopyToClipboard(data, (Bitmap)bmp.NativeBmp, figs);
        }

        void CopyToClipboard(string data, Bitmap bmp, List<Figure> figs)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetData(FigureSerialize.DDRAW_FIGURE_XML, data);
            if (figs.Count == 1 && figs[0] is TextFigure)
                dataObject.SetData(DataFormats.Text, ((ITextable)figs[0]).Text);
            dataObject.SetData(DataFormats.Bitmap, true, bmp);
            Clipboard.SetDataObject(dataObject);
        }

        private void actCopy_Execute(object sender, EventArgs e)
        {
            List<Figure> figs = de.SelectedFigures;
            DBitmap bmp;
            string data = de.Copy(figs, out bmp, dvEditor.AntiAlias);
            CopyToClipboard(data, (Bitmap)bmp.NativeBmp, figs);
        }

        private void actPaste_Execute(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(FigureSerialize.DDRAW_FIGURE_XML))
                de.PasteAsSelectedFigures((string)iData.GetData(FigureSerialize.DDRAW_FIGURE_XML));
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                de.UndoRedoStart("Paste Text");
                TextFigure f = new TextFigure(new DPoint(10, 10), (string)iData.GetData(DataFormats.Text), 0);
                dap.ApplyPropertiesToFigure(f);
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                de.UndoRedoCommit();
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                de.UndoRedoStart("Paste Bitmap");
                Bitmap bmp = (Bitmap)iData.GetData(DataFormats.Bitmap, true);
                byte[] imageData = (byte[])TypeDescriptor.GetConverter(bmp).ConvertTo(bmp, typeof(byte[]));
                ImageFigure f = new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, imageData, "Clipboard.bmp");
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                de.UndoRedoCommit();
            }
        }

        void DoDelete()
        {
            if (wfvcEditor.Focused)
                de.Delete(de.SelectedFigures);
            else 
            {
                dem.UndoRedoStart("Delete Page");
                dem.RemoveEngine(de);
                if (dem.EngineCount == 0)
                    CreateDEngine(de);
                dem.UndoRedoCommit();
            }
        }

        private void actDelete_Execute(object sender, EventArgs e)
        {
            DoDelete();   
        }

        void UpdateTitleBar()
        {
            if (dem.Dirty)
                Text = string.Format("{0} - {1}{2}", ProgramName, Path.GetFileNameWithoutExtension(fileName), "*");
            else
                Text = string.Format("{0} - {1}", ProgramName, Path.GetFileNameWithoutExtension(fileName));
        }

        void New()
        {
            fileName = "New Document";
            beenSaved = false;

            dem.UndoRedoStart("New Document");
            dem.Clear();
            CreateDEngine(null);
            dem.UndoRedoCommit();
            dem.Dirty = false;

            UpdateTitleBar();
        }

        void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = FileExt;
            ofd.Filter = FileTypeFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // start undo/redo
                dem.UndoRedoStart("Open Document");
                // load new engines
                fileName = ofd.FileName;
                List<DEngine> engines = FileHelper.Load(fileName, dap, true);
                dem.SetEngines(engines);
                // init new dengines
                foreach (DEngine newDe in dem.GetEngines())
                    InitDEngine(newDe);
                // commit undo/redo
                dem.UndoRedoCommit();
                dem.Dirty = false;
                // update vars
                beenSaved = true;
                UpdateTitleBar();
            }
        }

        void Save()
        {
            FileHelper.Save(fileName, dem.GetEngines());
            dem.Dirty = false;
            beenSaved = true;
            UpdateTitleBar();
        }

        bool SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = FileExt;
            sfd.Filter = FileTypeFilter;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;
                Save();
                return true;
            }
            else
                return false;
        }

        bool CheckDirty()
        {
            if (dem.Dirty)
            {
                switch (MessageBox.Show(string.Format("Save changes to \"{0}\"?", fileName), ProgramName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        if (!beenSaved)
                            return SaveAs();
                        else
                            Save();
                        return true;
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
                return true;
        }

        private void actNew_Execute(object sender, EventArgs e)
        {
            if (CheckDirty())
                New();
        }

        private void actOpen_Execute(object sender, EventArgs e)
        {
            if (CheckDirty())
                Open();
        }

        private void actSave_Execute(object sender, EventArgs e)
        {
            if (!beenSaved)
                SaveAs();
            else
                Save();
        }

        private void actSaveAs_Execute(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void actPrint_Execute(object sender, EventArgs e)
        {
            if (dem.EngineCount > 0)
            {
                PrintDialog pf = new PrintDialog();
                pf.UseEXDialog = true;
                pf.AllowSelection = false;
                if (dem.EngineCount > 1)
                {
                    pf.AllowCurrentPage = true;
                    pf.AllowSomePages = true;
                    pf.PrinterSettings.MinimumPage = 1;
                    pf.PrinterSettings.MaximumPage = dem.EngineCount;
                    pf.PrinterSettings.FromPage = 1;
                    pf.PrinterSettings.ToPage = dem.EngineCount;
                }
                if (pf.ShowDialog() == DialogResult.OK)
                {
                    DPrintViewer dvPrint = new DPrintViewer();
                    // page iteration vars
                    IEnumerator<DEngine> engineEnumerator = dem.GetEngines().GetEnumerator();
                    engineEnumerator.MoveNext();
                    int pageIdx = pf.PrinterSettings.FromPage - 1;
                    // print document settings
                    PrintDocument pd = new PrintDocument();
                    pd.PrinterSettings = pf.PrinterSettings;
                    pd.DocumentName = Path.GetFileNameWithoutExtension(fileName);
                    pd.PrintPage += delegate(object s2, PrintPageEventArgs e2)
                    {
                        // set DEngine/page to print and whether we have more pages to go
                        DEngine de = null;
                        switch (pd.PrinterSettings.PrintRange)
                        {
                            case PrintRange.CurrentPage:
                                e2.HasMorePages = false;
                                de = this.de;
                                break;
                            case PrintRange.SomePages:
                                de = dem.GetEngine(pageIdx);
                                pageIdx += 1;
                                e2.HasMorePages = pageIdx < pd.PrinterSettings.ToPage;
                                break;
                            default: // PrintRange.AllPages
                                de = engineEnumerator.Current;
                                e2.HasMorePages = engineEnumerator.MoveNext();
                                break;
                        }
                        // print the page using the e2.Graphics GDI+ object
                        dvPrint.SetPageSize(de.PageSize);
                        WFPrintSettings dps = new WFPrintSettings(e2.PageSettings);
                        dvPrint.Paint(new WFGraphics(e2.Graphics), dps, de.GetBackgroundFigure(), de.Figures);
                    };
                    // call print operation
                    pd.Print();
                }
            }
        }

        private void actNewPage_Execute(object sender, EventArgs e)
        {
            dem.UndoRedoStart("New Page");
            CreateDEngine(de);
            dem.UndoRedoCommit();
        }

        private void actDeletePage_Execute(object sender, EventArgs e)
        {
            DoDelete();
        }

        private void actClonePage_Execute(object sender, EventArgs e)
        {
            dem.UndoRedoStart("Clone Page");
            // clone data
            string clonedFigures = FigureSerialize.FormatToXml(de.Figures, null);
            string clonedBackground = FigureSerialize.FormatToXml(de.GetBackgroundFigure(), null);
            // create new DEngine
            DPoint sz = de.PageSize;
            CreateDEngine(de);
            de.PageSize = sz;
            // add figures from clone data
            de.SetBackgroundFigure((BackgroundFigure)FigureSerialize.FromXml(clonedBackground)[0]);
            List<Figure> figs = FigureSerialize.FromXml(clonedFigures);
            foreach (Figure f in figs)
                de.AddFigure(f);
            dem.UndoRedoCommit();
        }

        private void actClearPage_Execute(object sender, EventArgs e)
        {
            de.ClearPage();
        }
    }
}