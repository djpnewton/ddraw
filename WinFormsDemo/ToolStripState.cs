using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Nini.Config;
using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public enum FigureToolStripMode { DEngineState, FigureClassSelect };

    public delegate void FigureClassChangedHandler(object sender, Type figureClass);
    public delegate void DapChangedHandler(object sender, DAuthorProperties dap);

    public class FigureToolStrip : ToolStripEx
    {
        ToolStripButton btnSelect;
        ToolStripButton btnPen;
        ToolStripButton btnRect;
        ToolStripButton btnEllipse;
        ToolStripButton btnText;
        ToolStripButton btnClock;
        ToolStripButton btnTriangle;
        ToolStripButton btnRATriangle;
        ToolStripButton btnDiamond;
        ToolStripButton btnPentagon;
        ToolStripButton btnLine;
        ToolStripButton btnEraser;

        FigureToolStripMode mode = FigureToolStripMode.DEngineState;
        public FigureToolStripMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                btnSelect.Visible = mode == FigureToolStripMode.DEngineState;
                btnEraser.Visible = mode == FigureToolStripMode.DEngineState;
                FigureClass = null;
            }
        }

        DEngine de;
        public DEngine De
        {
            set
            {
                if (de != null)
                    de.HsmStateChanged -= de_HsmStateChanged;
                de = value;
                if (de != null)
                {
                    de.HsmStateChanged += new HsmStateChangedHandler(de_HsmStateChanged);
                    de_HsmStateChanged(de, de.HsmState);
                }
            }
        }

        Type figureClass;
        public Type FigureClass
        {
            get { return figureClass; }
            set
            {
                figureClass = value;
                UpdateToFigureClass();
                DoFigureClassChanged();
            }
        }

        static DAuthorProperties dapPolyline = new DAuthorProperties();
        static DAuthorProperties dapRect = new DAuthorProperties();
        static DAuthorProperties dapEllipse = new DAuthorProperties();
        static DAuthorProperties dapText = new DAuthorProperties();
        static DAuthorProperties dapClock = new DAuthorProperties();
        static DAuthorProperties dapTriangle = new DAuthorProperties();
        static DAuthorProperties dapRightAngleTriangle = new DAuthorProperties();
        static DAuthorProperties dapDiamond = new DAuthorProperties();
        static DAuthorProperties dapPentagon = new DAuthorProperties();
        static DAuthorProperties dapLine = new DAuthorProperties();

        const string _INIFILE = "FigureTools.ini";
        const string POLYLINE_SECT = "Polyline";
        const string RECT_SECT = "Rect";
        const string ELLIPSE_SECT = "Ellipse";
        const string TEXT_SECT = "Text";
        const string CLOCK_SECT = "Clock";
        const string TRIANGLE_SECT = "Triangle";
        const string RIGHTANGLETRIANGLE_SECT = "RightAngleTriangle";
        const string DIAMOND_SECT = "Diamond";
        const string PENTAGON_SECT = "Pentagon";
        const string LINE_SECT = "Line";

        static string IniFile
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Application.ExecutablePath) +
                    System.IO.Path.DirectorySeparatorChar + _INIFILE;
            }
        }

        public static void LoadFigureTools()
        {
            if (System.IO.File.Exists(IniFile))
            {
                IConfigSource source = new IniConfigSource(IniFile);
                foreach (IConfig config in source.Configs)
                {
                    if (config.Name == POLYLINE_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapPolyline);
                    else if (config.Name == ELLIPSE_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapEllipse);
                    else if (config.Name == TEXT_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapText);
                    else if (config.Name == CLOCK_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapClock);
                    else if (config.Name == TRIANGLE_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapTriangle);
                    else if (config.Name == RIGHTANGLETRIANGLE_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapRightAngleTriangle);
                    else if (config.Name == DIAMOND_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapDiamond);
                    else if (config.Name == PENTAGON_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapPentagon);
                    else if (config.Name == LINE_SECT)
                        WorkBookUtils.ReadConfigToDap(config, dapLine);
                }
            }
        }

        public static void SaveFigureTools()
        {
            IniConfigSource source = new IniConfigSource();
            WorkBookUtils.WriteDapToConfig(dapPolyline, source.AddConfig(POLYLINE_SECT));
            WorkBookUtils.WriteDapToConfig(dapRect, source.AddConfig(RECT_SECT));
            WorkBookUtils.WriteDapToConfig(dapEllipse, source.AddConfig(ELLIPSE_SECT));
            WorkBookUtils.WriteDapToConfig(dapText, source.AddConfig(TEXT_SECT));
            WorkBookUtils.WriteDapToConfig(dapClock, source.AddConfig(CLOCK_SECT));
            WorkBookUtils.WriteDapToConfig(dapTriangle, source.AddConfig(TRIANGLE_SECT));
            WorkBookUtils.WriteDapToConfig(dapRightAngleTriangle, source.AddConfig(RIGHTANGLETRIANGLE_SECT));
            WorkBookUtils.WriteDapToConfig(dapDiamond, source.AddConfig(DIAMOND_SECT));
            WorkBookUtils.WriteDapToConfig(dapPentagon, source.AddConfig(PENTAGON_SECT));
            WorkBookUtils.WriteDapToConfig(dapLine, source.AddConfig(LINE_SECT));
            source.Save(IniFile);
        }

        DAuthorProperties dap = null;
        public DAuthorProperties Dap
        {
            get { return dap; }
        }

        public event FigureClassChangedHandler FigureClassChanged;
        public event DapChangedHandler DapChanged;

        public FigureToolStrip()
        {
            btnSelect = new ToolStripButton("Select", Resource1.cursor, btnClick);
            Items.Add(btnSelect);
            btnPen = new ToolStripButton("Pen", Resource1.pencil, btnClick);
            Items.Add(btnPen);
            btnEraser = new ToolStripButton("Eraser", Resource1.eraser, btnClick);
            Items.Add(btnEraser);
            btnRect = new ToolStripButton("Rectangle", Resource1.rect, btnClick);
            Items.Add(btnRect);
            btnEllipse = new ToolStripButton("Ellipse", Resource1.ellipse, btnClick);
            Items.Add(btnEllipse);
            btnText = new ToolStripButton("Text", Resource1.style, btnClick);
            Items.Add(btnText);
            btnClock = new ToolStripButton("Clock", Resource1.clock, btnClick);
            Items.Add(btnClock);
            btnTriangle = new ToolStripButton("Trangle", Resource1.tri, btnClick);
            Items.Add(btnTriangle);
            btnRATriangle = new ToolStripButton("Right Angled Triangle", Resource1.tri2, btnClick);
            Items.Add(btnRATriangle);
            btnDiamond = new ToolStripButton("Diamond", Resource1.dia, btnClick);
            Items.Add(btnDiamond);
            btnPentagon = new ToolStripButton("Pentagon", Resource1.pent, btnClick);
            Items.Add(btnPentagon);
            btnLine = new ToolStripButton("Line", Resource1.line, btnClick);
            Items.Add(btnLine);
            foreach (ToolStripItem b in Items)
            {
                b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                b.ImageScaling = ToolStripItemImageScaling.None;
            }
        }

        void de_HsmStateChanged(DEngine de, DHsmState state)
        {
            // set button checked
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
            // update dap
            foreach (ToolStripButton btn in Items)
                if (btn.Checked)
                {
                    dap = DapFromButton(btn);
                    if (DapChanged != null)
                        DapChanged(this, dap);
                    break;
                }
        }

        DAuthorProperties DapFromButton(object btn)
        {
            if (btn == btnPen)
                return dapPolyline;
            else if (btn == btnRect)
                return dapRect;
            else if (btn == btnEllipse)
                return dapEllipse;
            else if (btn == btnText)
                return dapText;
            else if (btn == btnClock)
                return dapClock;
            else if (btn == btnTriangle)
                return dapTriangle;
            else if (btn == btnRATriangle)
                return dapRightAngleTriangle;
            else if (btn == btnDiamond)
                return dapDiamond;
            else if (btn == btnPentagon)
                return dapPentagon;
            else if (btn == btnLine)
                return dapLine;
            return null;
        }

        Type FigureClassFromButton(object btn)
        {
            if (btn == btnPen)
                return typeof(PolylineFigure);
            else if (btn == btnRect)
                return typeof(RectFigure);
            else if (btn == btnEllipse)
                return typeof(EllipseFigure);
            else if (btn == btnText)
                return typeof(TextFigure);
            else if (btn == btnClock)
                return typeof(ClockFigure);
            else if (btn == btnTriangle)
                return typeof(TriangleFigure);
            else if (btn == btnRATriangle)
                return typeof(RightAngleTriangleFigure);
            else if (btn == btnDiamond)
                return typeof(DiamondFigure);
            else if (btn == btnPentagon)
                return typeof(PentagonFigure);
            else if (btn == btnLine)
                return typeof(LineFigure);
            return null;
        }

        void btnClick(object sender, EventArgs e)
        {
            if (mode == FigureToolStripMode.DEngineState)
            {
                System.Diagnostics.Debug.Assert(de != null, "ERROR: \"de\" property needs to be set");
                if (sender == btnSelect)
                    de.HsmState = DHsmState.Select;
                else if (sender == btnEraser)
                {
                    de.HsmState = DHsmState.Eraser;
                    de.SetEraserSize(25);
                }
                else
                {
                    Type figureClass = FigureClassFromButton(sender);
                    if (figureClass != null)
                    {
                        de.HsmSetStateByFigureClass(figureClass);
                        ShowFigureStylePopup((ToolStripButton)sender, figureClass, dap);
                    }
                }
            }
            else if (mode == FigureToolStripMode.FigureClassSelect)
            {
                if (sender == btnPen)
                    figureClass = typeof(PolylineFigure);
                else if (sender == btnRect)
                    figureClass = typeof(RectFigure);
                else if (sender == btnEllipse)
                    figureClass = typeof(EllipseFigure);
                else if (sender == btnText)
                    figureClass = typeof(TextFigure);
                else if (sender == btnClock)
                    figureClass = typeof(ClockFigure);
                else if (sender == btnTriangle)
                    figureClass = typeof(TriangleFigure);
                else if (sender == btnRATriangle)
                    figureClass = typeof(RightAngleTriangleFigure);
                else if (sender == btnDiamond)
                    figureClass = typeof(DiamondFigure);
                else if (sender == btnPentagon)
                    figureClass = typeof(PentagonFigure);
                else if (sender == btnLine)
                    figureClass = typeof(LineFigure);
                else
                    figureClass = null;
                UpdateToFigureClass();
                DoFigureClassChanged();
            }
        }

        private void ShowFigureStylePopup(ToolStripItem item, Type figureClass, DAuthorProperties dap)
        {
            Point pt = new Point(item.Bounds.Left, item.Bounds.Bottom);
            pt = item.Owner.PointToScreen(pt);
            FigureStylePopup pf = new FigureStylePopup(pt.X, pt.Y, figureClass, dap, true);
            pf.AutoHideTimeout = 2500;
            pf.AddToPersonalTools += new FigureStyleEvent(pf_AddToPersonalTools);
            pf.Show();
        }

        public event FigureStyleEvent AddToPersonalTools;

        void pf_AddToPersonalTools(object sender, PersonalToolbar.CustomFigureT customFigure)
        {
            if (AddToPersonalTools != null)
                AddToPersonalTools(sender, customFigure);
        }

        void UpdateToFigureClass()
        {
            btnPen.Checked = typeof(PolylineFigure).Equals(figureClass);
            btnRect.Checked = typeof(RectFigure).Equals(figureClass);
            btnEllipse.Checked = typeof(EllipseFigure).Equals(figureClass);
            btnText.Checked = typeof(TextFigure).Equals(figureClass);
            btnClock.Checked = typeof(ClockFigure).Equals(figureClass);
            btnTriangle.Checked = typeof(TriangleFigure).Equals(figureClass);
            btnRATriangle.Checked = typeof(RightAngleTriangleFigure).Equals(figureClass);
            btnDiamond.Checked = typeof(DiamondFigure).Equals(figureClass);
            btnPentagon.Checked = typeof(PentagonFigure).Equals(figureClass);
            btnLine.Checked = typeof(LineFigure).Equals(figureClass);
        }

        void DoFigureClassChanged()
        {
            if (FigureClassChanged != null)
                FigureClassChanged(this, figureClass);
        }
    }

    public class FigurePropertiesToolStrip : ToolStripEx
    {
        ToolStripColorButton btnFill;
        ToolStripColorButton btnStroke;
        ToolStripStrokeWidthButton btnStrokeWidth;
        ToolStripStrokeStyleButton btnStrokeStyle;
        ToolStripMarkerButton btnStartMarker;
        ToolStripMarkerButton btnEndMarker;
        ToolStripAlphaButton btnAlpha;
        ToolStripFontNameButton btnFontName;
        ToolStripButton btnBold;
        ToolStripButton btnItalic;
        ToolStripButton btnUnderline;
        ToolStripButton btnStrikethrough;

        DAuthorProperties dap;
        public DAuthorProperties Dap
        {
            set
            {
                if (dap != null)
                    dap.PropertyChanged -= dap_PropertyChanged;
                dap = value;
                if (dap != null)
                {
                    dap.PropertyChanged += new AuthorPropertyChanged(dap_PropertyChanged);
                    UpdateToDap();
                }
            }
            get { return dap; }
        }

        Type figureClass;
        public Type FigureClass
        {
            get { return figureClass; }
            set 
            {
                figureClass = value;
                InitPropertyControlsToFigureClass();
                UpdateToDap();
            }
        }

        DEngine de;
        public DEngine De
        {
            set
            {
                if (de != null)
                {
                    de.HsmStateChanged -= de_HsmStateChanged;
                    de.SelectedFiguresChanged -= de_SelectedFiguresChanged;
                }
                de = value;
                if (de != null)
                {
                    de.HsmStateChanged += new HsmStateChangedHandler(de_HsmStateChanged);
                    de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
                    InitPropertyControlsToDEngine(de.HsmState);
                }
            }
        }

        DTkViewer dv;
        public DTkViewer Dv
        {
            set { dv = value; }
        }

        public FigurePropertiesToolStrip()
        {
            btnFill = new ToolStripColorButton();
            btnFill.ColorType = ColorType.Fill;
            btnFill.Text = "Fill";
            Items.Add(btnFill);
            btnStroke = new ToolStripColorButton();
            btnStroke.ColorType = ColorType.Stroke;
            btnStroke.Text = "Stroke";
            Items.Add(btnStroke);
            btnStrokeWidth = new ToolStripStrokeWidthButton();
            btnStrokeWidth.Text = "Stroke Width";
            btnStrokeWidth.StrokeWidthChanged += new StrokeWidthChangedHandler(btnStrokeWidth_StrokeWidthChanged);
            Items.Add(btnStrokeWidth);
            btnStrokeStyle = new ToolStripStrokeStyleButton();
            btnStrokeStyle.Text = "Stroke Style";
            btnStrokeStyle.StrokeStyleChanged += new StrokeStyleChangedHandler(btnStrokeStyle_StrokeStyleChanged);
            Items.Add(btnStrokeStyle);
            btnStartMarker = new ToolStripMarkerButton();
            btnStartMarker.Start = true;
            btnStartMarker.Text = "Start Marker";
            btnStartMarker.MarkerChanged += new MarkerChangedHandler(btnMarker_MarkerChanged);
            Items.Add(btnStartMarker);
            btnEndMarker = new ToolStripMarkerButton();
            btnEndMarker.Start = false;
            btnEndMarker.Text = "End Marker";
            btnEndMarker.MarkerChanged += new MarkerChangedHandler(btnMarker_MarkerChanged);
            Items.Add(btnEndMarker);
            btnAlpha = new ToolStripAlphaButton();
            btnAlpha.Text = "Opacity";
            btnAlpha.AlphaChanged += new AlphaChangedHandler(btnAlpha_AlphaChanged);
            Items.Add(btnAlpha);
            btnFontName = new ToolStripFontNameButton();
            Items.Add(btnFontName);
            btnBold = new ToolStripButton("Bold", Resource1.text_bold);
            btnBold.CheckOnClick = true;
            Items.Add(btnBold);
            btnItalic = new ToolStripButton("Italic", Resource1.text_italic);
            btnItalic.CheckOnClick = true;
            Items.Add(btnItalic);
            btnUnderline = new ToolStripButton("Underline", Resource1.text_underline);
            btnUnderline.CheckOnClick = true;
            Items.Add(btnUnderline);
            btnStrikethrough = new ToolStripButton("Strikethrough", Resource1.text_strikethrough);
            btnStrikethrough.CheckOnClick = true;
            Items.Add(btnStrikethrough);
            foreach (ToolStripItem b in Items)
            {
                b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                b.ImageScaling = ToolStripItemImageScaling.None;
                b.Click += btnClick;
            }
        }

        void de_HsmStateChanged(DEngine de, DHsmState state)
        {
            InitPropertyControlsToDEngine(state);
        }

        void de_SelectedFiguresChanged()
        {
            InitPropertyControlsToDEngine(de.HsmState);
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

        void DefaultControlProperties()
        {
            btnFill.Color = Color.Empty;
            btnStroke.Color = Color.Empty;
            btnStrokeWidth.Value = ToolStripStrokeWidthButton.Empty;
            btnStrokeStyle.Value = DStrokeStyle.Solid;
            btnStartMarker.Value = DMarker.None;
            btnEndMarker.Value = DMarker.None;
            btnAlpha.Value = ToolStripAlphaButton.Empty;
            btnFontName.Value = "";
            btnBold.Checked = false;
            btnItalic.Checked = false;
            btnUnderline.Checked = false;
            btnStrikethrough.Checked = false;
        }

        void DeselectControls()
        {
            btnFill.Enabled = false;
            btnStroke.Enabled = false;
            btnStrokeWidth.Enabled = false;
            btnStrokeStyle.Enabled = false;
            btnStartMarker.Enabled = false;
            btnEndMarker.Enabled = false;
            btnAlpha.Enabled = false;
            btnFontName.Enabled = false;
            btnBold.Enabled = false;
            btnItalic.Enabled = false;
            btnUnderline.Enabled = false;
            btnStrikethrough.Enabled = false;
        }

        void InitPropertyControlsToDEngine(DHsmState state)
        {
            // set default (blank) values for property controls
            DefaultControlProperties();
            // deselect controls
            DeselectControls();
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
                            btnFontName.Enabled = true;
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
                        btnFontName.Value = GetFontNameMatch(figs);
                        btnBold.Checked = GetBoldMatch(figs);
                        btnItalic.Checked = GetItalicMatch(figs);
                        btnUnderline.Checked = GetUnderlineMatch(figs);
                        btnStrikethrough.Checked = GetStrikethroughMatch(figs);
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(dap != null, "ERROR: \"dap\" is not assigned");
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
                        btnFontName.Enabled = true;
                        btnFontName.Value = dap.FontName;
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
        }

        void UpdateToDap()
        {
            if (btnFill.Enabled)
                btnFill.Color = WFHelper.MakeColor(dap.Fill);
            if (btnStroke.Enabled)
                btnStroke.Color = WFHelper.MakeColor(dap.Stroke);
            if (btnStrokeWidth.Enabled)
                btnStrokeWidth.Value = (int)Math.Round(dap.StrokeWidth);
            if (btnStrokeStyle.Enabled)
                btnStrokeStyle.Value = dap.StrokeStyle;
            if (btnStartMarker.Enabled)
                btnStartMarker.Value = dap.StartMarker;
            if (btnEndMarker.Enabled)
                btnEndMarker.Value = dap.EndMarker;
            if (btnAlpha.Enabled)
                btnAlpha.Value = dap.Alpha;
            if (btnFontName.Enabled)
                btnFontName.Value = dap.FontName;
            if (btnBold.Enabled)
                btnBold.Checked = dap.Bold;
            if (btnItalic.Enabled)
                btnItalic.Checked = dap.Italics;
            if (btnUnderline.Enabled)
                btnUnderline.Checked = dap.Underline;
            if (btnStrikethrough.Enabled)
                btnStrikethrough.Checked = dap.Strikethrough;
        }

        void InitPropertyControlsToFigureClass()
        {
            DefaultControlProperties();
            DeselectControls();
            if (typeof(IFillable).IsAssignableFrom(figureClass))
                btnFill.Enabled = true;
            if (typeof(IStrokeable).IsAssignableFrom(figureClass))
            {
                btnStroke.Enabled = true;
                btnStrokeWidth.Enabled = true;
                btnStrokeStyle.Enabled = true;
            }
            if (typeof(IMarkable).IsAssignableFrom(figureClass))
            {
                btnStartMarker.Enabled = true;
                btnEndMarker.Enabled = true;
            }
            if (typeof(IAlphaBlendable).IsAssignableFrom(figureClass))
                btnAlpha.Enabled = true;
            if (typeof(ITextable).IsAssignableFrom(figureClass))
            {
                btnFontName.Enabled = true;
                btnBold.Enabled = true;
                btnItalic.Enabled = true;
                btnUnderline.Enabled = true;
                btnStrikethrough.Enabled = true;
            }
        }

        void dap_PropertyChanged(DAuthorProperties dap)
        {
            UpdateToDap();
        }

        void btnClick(object sender, EventArgs e)
        {
            if (dap != null || de != null)
            {
                if (sender == btnFill)
                {
                    Point pt = new Point(btnFill.Bounds.Left, btnFill.Bounds.Bottom);
                    pt = btnFill.Owner.PointToScreen(pt);
                    ColorPicker f = new ColorPicker(pt.X, pt.Y);
                    f.ColorSelected += delegate(object sender2, EventArgs ea)
                    {
                        btnFill.Color = f.SelectedColor;
                        if (de != null && de.HsmState == DHsmState.Select)
                            UpdateSelectedFigures(btnFill);
                        else
                            dap.Fill = WFHelper.MakeColor(btnFill.Color);
                    };
                    f.Show();
                }
                else if (sender == btnStroke)
                {
                    Point pt = new Point(btnStroke.Bounds.Left, btnStroke.Bounds.Bottom);
                    pt = btnStroke.Owner.PointToScreen(pt);
                    ColorPicker f = new ColorPicker(pt.X, pt.Y);
                    f.ColorSelected += delegate(object sender2, EventArgs ea)
                    {
                        btnStroke.Color = f.SelectedColor;
                        if (de != null && de.HsmState == DHsmState.Select)
                            UpdateSelectedFigures(btnStroke);
                        else
                            dap.Stroke = WFHelper.MakeColor(btnStroke.Color);

                    };
                    f.Show();
                }
                else if (sender == btnFontName)
                {
                    Point pt = new Point(btnFontName.Bounds.Left, btnFontName.Bounds.Bottom);
                    pt = btnFontName.Owner.PointToScreen(pt);
                    FontNamePicker f = new FontNamePicker(pt.X, pt.Y, btnFontName.Value);
                    f.FontNameSelected += delegate(object sender2, EventArgs ea)
                    {
                        btnFontName.Value = f.SelectedFontName;
                        if (de != null && de.HsmState == DHsmState.Select)
                            UpdateSelectedFigures(btnFontName);
                        else
                            dap.FontName = btnFontName.Value;

                    };
                    f.Show();
                }
                else if (sender == btnBold)
                {
                    if (de != null && de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnBold);
                    else
                        dap.Bold = btnBold.Checked;
                }
                else if (sender == btnItalic)
                {
                    if (de != null && de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnItalic);
                    else
                        dap.Italics = btnItalic.Checked;
                }
                else if (sender == btnUnderline)
                {
                    if (de != null && de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnUnderline);
                    else
                        dap.Underline = btnUnderline.Checked;
                }
                else if (sender == btnStrikethrough)
                {
                    if (de != null && de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnStrikethrough);
                    else
                        dap.Strikethrough = btnStrikethrough.Checked;
                }
            }
        }

        void btnAlpha_AlphaChanged(object sender, double alpha)
        {
            if (de != null && de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnAlpha);
            else
                dap.Alpha = btnAlpha.Value;
        }

        void btnStrokeWidth_StrokeWidthChanged(object sender, int strokeWidth)
        {
            if (de != null && de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnStrokeWidth);
            else
                dap.StrokeWidth = btnStrokeWidth.Value;
        }

        void btnStrokeStyle_StrokeStyleChanged(object sender, DStrokeStyle strokeStyle)
        {
            if (de != null && de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnStrokeStyle);
            else
                dap.StrokeStyle = btnStrokeStyle.Value;
        }

        void btnMarker_MarkerChanged(object sender, DMarker marker)
        {
            if (sender == btnStartMarker)
            {
                if (de != null && de.HsmState == DHsmState.Select)
                    UpdateSelectedFigures(btnStartMarker);
                else
                    dap.StartMarker = btnStartMarker.Value;
            }
            else if (sender == btnEndMarker)
            {
                if (de != null && de.HsmState == DHsmState.Select)
                    UpdateSelectedFigures(btnEndMarker);
                else
                    dap.EndMarker = btnEndMarker.Value;
            }
        }

        private void UpdateSelectedFigures(object sender)
        {
            if (de != null && dv != null)
            {
                de.UndoRedoStart("Change Property");
                foreach (Figure f in de.SelectedFigures)
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
                        if (sender == btnFontName)
                            ((ITextable)f).FontName = btnFontName.Value;
                        if (sender == btnFontName)
                            ((ITextable)f).FontName = btnFontName.Value;
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
                dv.Update();
            }
        }
    }
}
