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

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public partial class MainForm : Form
    {
        DAuthorProperties dap;
        DEngine de = null;

        DViewer dvEditor;

        void CreateDEngine()
        {
            DEngine de = new DEngine(dap);
            previewBar1.AddPreview(de, dvEditor);
            de.PageSize = new DPoint(500, 400);
            // DEngine events
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
            de.UndoRedoMgr.UndoRedoChanged += new UndoRedoChangedDelegate(UndoRedoMgr_UndoRedoChanged);
            de.ContextClick += new ContextClickHandler(de_ContextClick);
            de.StateChanged += new DEngine.DEngineStateChangedHandler(de_StateChanged);

            SetCurrentDe(de);
        }

        private void SetCurrentDe(DEngine de)
        {
            if (this.de != null)
            {
                this.de.RemoveViewer(dvEditor);
                de.State = this.de.State;
            }
            de.AddViewer(dvEditor);
            if (dvEditor.Zoom != Zoom.Custom)
                dvEditor.Zoom = dvEditor.Zoom; 
            dvEditor.Update();
            this.de = de;
            de_SelectedFiguresChanged();
            UndoRedoMgr_UndoRedoChanged(false);
        }

        public MainForm()
        {
            InitializeComponent();
            // Initialze DGraphics
            WFGraphics.Init();
            // create author properties
            dap = new DAuthorProperties(DColor.Blue, DColor.Red, 3, DStrokeStyle.Solid, DMarker.None, DMarker.None, 1, "Arial", false, false, false, false);
            // edit viewer
            dvEditor = new WFViewer(wfvcEditor);
            dvEditor.EditFigures = true;
            dvEditor.DebugMessage += new DebugMessageHandler(DebugMessage);
            // create ddraw engine 1
            CreateDEngine();
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
            MemoryStream ms = new MemoryStream();
            Resource1.technocolor.Save(ms, ImageFormat.Bmp);
            f = new ImageFigure(new DRect(250, 50, 24, 16), 0, new WFBitmap(ms));
            de.AddFigure(f);
            f = new ImageFigure(new DRect(150, 150, 39, 50), 0, new WFBitmap(ms));
            f.LockAspectRatio = true;
            de.AddFigure(f);
            ms.Dispose();
            // text figure
            f = new TextFigure(new DPoint(100, 200), "hello\ndan", GraphicsHelper.TextExtent, 0);
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
            // Init controls
            InitPropertyControls(de.State);
        }

        void DebugMessage(string msg)
        {
            lbInfo.Text = msg;
        }

        Color MakeColor(DColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        DColor MakeColor(Color color)
        {
            return new DColor(color.R, color.G, color.B, color.A);
        }

        void de_SelectedFiguresChanged()
        {
            InitPropertyControls(de.State);
            InitMenus();
        }

        void UndoRedoMgr_UndoRedoChanged(bool commitAction)
        {
            undoToolStripMenuItem.Enabled = de.UndoRedoMgr.CanUndo;
            if (undoToolStripMenuItem.Enabled)
                undoToolStripMenuItem.Text = string.Format("Undo \"{0}\"", de.UndoRedoMgr.UndoName);
            else
                undoToolStripMenuItem.Text = "Undo";
            redoToolStripMenuItem.Enabled = de.UndoRedoMgr.CanRedo;
            if (redoToolStripMenuItem.Enabled)
                redoToolStripMenuItem.Text = string.Format("Redo \"{0}\"", de.UndoRedoMgr.RedoName);
            else
                redoToolStripMenuItem.Text = "Redo";
        }

        void de_ContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (clickedFigure != null)
                cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
            else
            {
                a4ToolStripMenuItem.Checked = de.PageFormat == PageFormat.A4;
                a5ToolStripMenuItem.Checked = de.PageFormat == PageFormat.A5;
                letterToolStripMenuItem.Checked = de.PageFormat == PageFormat.Letter;
                customToolStripMenuItem.Checked = de.PageFormat == PageFormat.Custom;
                fitToPageToolStripMenuItem.Checked = dvEditor.Zoom == Zoom.FitToPage;
                fitToWidthToolStripMenuItem.Checked = dvEditor.Zoom == Zoom.FitToWidth;
                _050PcToolStripMenuItem.Checked = dvEditor.Scale == 0.5;
                _100PcToolStripMenuItem.Checked = dvEditor.Scale == 1.0;
                _150PcToolStripMenuItem.Checked = dvEditor.Scale == 1.5;
                cmsBackground.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
            }
        }

        Color GetFillMatch(Figure[] figs)
        {
            Color fill = Color.Empty;
            foreach (Figure f in figs)
                if (f is IFillable)
                {
                    fill = MakeColor(((IFillable)f).Fill);
                    break;
                }
            if (fill != Color.Empty)
                foreach (Figure f in figs)
                    if (f is IFillable)
                    {
                        if (fill != MakeColor(((IFillable)f).Fill))
                            return Color.Empty;
                    }
            return fill;
        }

        Color GetStrokeMatch(Figure[] figs)
        {
            Color stroke = Color.Empty;
            foreach (Figure f in figs)
                if (f is IStrokeable)
                {
                    stroke = MakeColor(((IStrokeable)f).Stroke);
                    break;
                }
            if (stroke != Color.Empty)
                foreach (Figure f in figs)
                    if (f is IStrokeable)
                    {
                        if (stroke != MakeColor(((IStrokeable)f).Stroke))
                            return Color.Empty;
                    }
            return stroke;
        }

        double GetStrokeWidthMatch(Figure[] figs)
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

        DStrokeStyle GetStrokeStyleMatch(Figure[] figs)
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

        DMarker GetMarkerMatch(Figure[] figs, bool start)
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

        double GetAlphaMatch(Figure[] figs)
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

        string GetFontNameMatch(Figure[] figs)
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

        bool GetBoldMatch(Figure[] figs)
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

        bool GetItalicMatch(Figure[] figs)
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

        bool GetUnderlineMatch(Figure[] figs)
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

        bool GetStrikethroughMatch(Figure[] figs)
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

        private void InitPropertyControls(DEngineState state)
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
                case DEngineState.Select:
                    // get selected figures
                    Figure[] figs = de.SelectedFigures;
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
                    if (figs.Length > 0)
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
                    if (de.CurrentFigClassImpls(typeof(IFillable)))
                    {
                        btnFill.Enabled = true;
                        btnFill.Color = MakeColor(dap.Fill);
                    }
                    if (de.CurrentFigClassImpls(typeof(IStrokeable)))
                    {
                        btnStroke.Enabled = true;
                        btnStroke.Color = MakeColor(dap.Stroke);
                        btnStrokeWidth.Enabled = true;
                        btnStrokeWidth.Value = (int)dap.StrokeWidth;
                        btnStrokeStyle.Enabled = true;
                        btnStrokeStyle.Value = dap.StrokeStyle;
                    }
                    if (de.CurrentFigClassImpls(typeof(IMarkable)))
                    {
                        btnStartMarker.Enabled = true;
                        btnStartMarker.Value = dap.StartMarker;
                        btnEndMarker.Enabled = true;
                        btnEndMarker.Value = dap.EndMarker;
                    }
                    if (de.CurrentFigClassImpls(typeof(IAlphaBlendable)))
                    {
                        btnAlpha.Enabled = true;
                        btnAlpha.Value = dap.Alpha;
                    }
                    if (de.CurrentFigClassImpls(typeof(ITextable)))
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

        void InitMenus()
        {
            Figure[] figs = de.SelectedFigures;
            // update group menu item
            groupToolStripMenuItem.Enabled = true;
            if (figs.Length == 1 && figs[0] is GroupFigure)
                groupToolStripMenuItem.Text = "Ungroup";
            else if (figs.Length > 1)
                groupToolStripMenuItem.Text = "Group";
            else
                groupToolStripMenuItem.Enabled = false;
            // update order menu items
            sendToBackToolStripMenuItem.Enabled = de.CanSendBackward(figs);
            bringToFrontToolStripMenuItem.Enabled = de.CanBringForward(figs);
            sendBackwardToolStripMenuItem.Enabled = de.CanSendBackward(figs);
            bringForwardToolStripMenuItem.Enabled = de.CanBringForward(figs);
        }

        void de_StateChanged(DEngine de, DEngineState state)
        {
            btnSelect.Checked = state == DEngineState.Select;
            btnPen.Checked = de.CurrentFigClassIs(typeof(PolylineFigure));
            btnRect.Checked = de.CurrentFigClassIs(typeof(RectFigure));
            btnEllipse.Checked = de.CurrentFigClassIs(typeof(EllipseFigure));
            btnText.Checked = state == DEngineState.DrawText;
            btnClock.Checked = de.CurrentFigClassIs(typeof(ClockFigure));
            btnTriangle.Checked = de.CurrentFigClassIs(typeof(TriangleFigure));
            btnRATriangle.Checked = de.CurrentFigClassIs(typeof(RightAngleTriangleFigure));
            btnDiamond.Checked = de.CurrentFigClassIs(typeof(DiamondFigure));
            btnPentagon.Checked = de.CurrentFigClassIs(typeof(PentagonFigure));
            InitPropertyControls(state);
        }

        private void btnAntiAlias_Click(object sender, EventArgs e)
        {
            dvEditor.AntiAlias = !dvEditor.AntiAlias;
            btnAntiAlias.Checked = dvEditor.AntiAlias;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            Point pt = new Point(btnFill.Bounds.Left, btnFill.Bounds.Bottom);
            pt = btnFill.Owner.PointToScreen(pt);
            ColorPicker f = new ColorPicker(pt.X, pt.Y);
            f.ColorSelected += delegate(object sender2, EventArgs ea)
            {
                btnFill.Color = ((ColorPicker)sender2).SelectedColor;
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnFill);
                        break;
                    default:
                        dap.Fill = MakeColor(btnFill.Color);
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
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnStroke);
                        break;
                    default:
                        dap.Stroke = MakeColor(btnStroke.Color);
                        break;
                }
            };
            f.Show();
        }

        private void btnAlpha_AlphaChanged(object sender, double alpha)
        {
            UpdateSelectedFigures(btnAlpha);
            switch (de.State)
            {
                case DEngineState.Select:
                    UpdateSelectedFigures(btnAlpha);
                    break;
                default:
                    dap.Alpha = btnAlpha.Value;
                    break;
            }
        }

        private void btnStrokeWidth_StrokeWidthChanged(object sender, int strokeWidth)
        {
            switch (de.State)
            {
                case DEngineState.Select:
                    UpdateSelectedFigures(btnStrokeWidth);
                    break;
                default:
                    dap.StrokeWidth = btnStrokeWidth.Value;
                    break;
            }
        }

        private void btnStrokeStyle_StrokeStyleChanged(object sender, DStrokeStyle strokeStyle)
        {
            switch (de.State)
            {
                case DEngineState.Select:
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
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnStartMarker);
                        break;
                    default:
                        dap.StartMarker = btnStartMarker.Value;
                        break;
                }
            else if (sender == btnEndMarker)
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnEndMarker);
                        break;
                    default:
                        dap.EndMarker = btnEndMarker.Value;
                        break;
                }
        }

        private void cbFontName_FontNameChanged(object sender, EventArgs e)
        {
            switch (de.State)
            {
                case DEngineState.Select:
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
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnBold);
                        break;
                    default:
                        dap.Bold = btnBold.Checked;
                        break;
                }
            if (sender == btnItalic)
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnItalic);
                        break;
                    default:
                        dap.Italics = btnItalic.Checked;
                        break;
                }
            if (sender == btnUnderline)
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnUnderline);
                        break;
                    default:
                        dap.Underline = btnUnderline.Checked;
                        break;
                }
            if (sender == btnStrikethrough)
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnStrikethrough);
                        break;
                    default:
                        dap.Strikethrough = btnStrikethrough.Checked;
                        break;
                }
        }

        private void UpdateSelectedFigures(object sender)
        {
            de.UndoRedoMgr.Start("Change Property"); // TODO: make this work better with the slider controls
            Figure[] figs = de.SelectedFigures;
            foreach (Figure f in figs)
            {
                if (sender == btnFill && f is IFillable)
                    ((IFillable)f).Fill = MakeColor(btnFill.Color);
                if (f is IStrokeable)
                {
                    if (sender == btnStroke)
                        ((IStrokeable)f).Stroke = MakeColor(btnStroke.Color);
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
            de.UndoRedoMgr.Commit();
            dvEditor.Update();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            de.State = DEngineState.Select;
        }

        private void btnPen_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(PolylineFigure));
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(RectFigure));
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(EllipseFigure));
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(TextFigure));
        }

        private void btnClock_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(ClockFigure));
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            if (sender == btnTriangle)
                de.SetStateByFigureClass(typeof(TriangleFigure));
            else if (sender == btnRATriangle)
                de.SetStateByFigureClass(typeof(RightAngleTriangleFigure));
            else if (sender == btnDiamond)
                de.SetStateByFigureClass(typeof(DiamondFigure));
            else if (sender == btnPentagon)
                de.SetStateByFigureClass(typeof(PentagonFigure));
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            de.SetStateByFigureClass(typeof(LineFigure));
        }

        private void previewBar1_PreviewSelected(Preview p)
        {
            SetCurrentDe(p.DEngine);
        }

        private void previewBar1_PreviewAdd(object sender, EventArgs e)
        {
            CreateDEngine();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                de.ClearSelected();
                WFBitmap bmp = new WFBitmap(ofd.FileName);
				de.UndoRedoMgr.Start("Add Image");
                de.AddFigure(new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, bmp));
				de.UndoRedoMgr.Commit();
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
            if (de.State != DEngineState.Select)
                de.State = DEngineState.Select;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            de.UndoRedoMgr.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            de.UndoRedoMgr.Redo();
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Figure[] figs = de.SelectedFigures;
            if (figs.Length == 1 && figs[0] is GroupFigure)
                de.UngroupFigure((GroupFigure)figs[0]);
            else if (figs.Length > 1)
                de.GroupFigures(figs);
        }

        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == sendToBackToolStripMenuItem)
                de.SendToBack(de.SelectedFigures);
            else if (sender == bringToFrontToolStripMenuItem)
                de.BringToFront(de.SelectedFigures);
            else if (sender == sendBackwardToolStripMenuItem)
                de.SendBackward(de.SelectedFigures);
            else if (sender == bringForwardToolStripMenuItem)
                de.BringForward(de.SelectedFigures);
        }

        private void PageSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == a4ToolStripMenuItem)
                de.PageFormat = PageFormat.A4;
            else if (sender == a5ToolStripMenuItem)
                de.PageFormat = PageFormat.A5;
            else if (sender == letterToolStripMenuItem)
                de.PageFormat = PageFormat.Letter;
            else if (sender == customToolStripMenuItem)
            {
                CustomPageSizeForm f = new CustomPageSizeForm();
                f.PageSize = de.PageSize;
                if (f.ShowDialog() == DialogResult.OK)
                    de.PageSize = f.PageSize;
            }
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

        private void notImplemented_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void wfvcEditor_KeyDown(object sender, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
        }

        private void wfvcEditor_KeyUp(object sender, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
        }
    }
}