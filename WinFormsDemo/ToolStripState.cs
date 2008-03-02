using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public class ToolStripDEngineState : ToolStrip
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

        public ToolStripDEngineState()
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
        }

        void btnClick(object sender, EventArgs e)
        {
            if (de != null)
            {
                if (sender == btnSelect)
                    de.HsmState = DHsmState.Select;
                else if (sender == btnPen)
                    de.HsmSetStateByFigureClass(typeof(PolylineFigure));
                else if (sender == btnRect)
                    de.HsmSetStateByFigureClass(typeof(RectFigure));
                else if (sender == btnEllipse)
                    de.HsmSetStateByFigureClass(typeof(EllipseFigure));
                else if (sender == btnText)
                    de.HsmSetStateByFigureClass(typeof(TextFigure));
                else if (sender == btnClock)
                    de.HsmSetStateByFigureClass(typeof(ClockFigure));
                else if (sender == btnTriangle)
                    de.HsmSetStateByFigureClass(typeof(TriangleFigure));
                else if (sender == btnRATriangle)
                    de.HsmSetStateByFigureClass(typeof(RightAngleTriangleFigure));
                else if (sender == btnDiamond)
                    de.HsmSetStateByFigureClass(typeof(DiamondFigure));
                else if (sender == btnPentagon)
                    de.HsmSetStateByFigureClass(typeof(PentagonFigure));
                else if (sender == btnLine)
                    de.HsmSetStateByFigureClass(typeof(LineFigure));
                else if (sender == btnEraser)
                {
                    de.HsmState = DHsmState.Eraser;
                    de.SetEraserSize(25);
                }
            }
        }
    }

    public class ToolStripDAuthorPropsState : ToolStrip
    {
        ToolStripColorButton btnFill;
        ToolStripColorButton btnStroke;
        ToolStripStrokeWidthButton btnStrokeWidth;
        ToolStripStrokeStyleButton btnStrokeStyle;
        ToolStripMarkerButton btnStartMarker;
        ToolStripMarkerButton btnEndMarker;
        ToolStripAlphaButton btnAlpha;
        ToolStripFontNameChooser cbFontName;
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
                    de_HsmStateChanged(de, de.HsmState);
                    de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
                    de_SelectedFiguresChanged();
                }
            }
        }

        DTkViewer dv;
        public DTkViewer Dv
        {
            set { dv = value; }
        }

        public ToolStripDAuthorPropsState()
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
            cbFontName = new ToolStripFontNameChooser();
            cbFontName.Text = "Font Name";
            Items.Add(cbFontName);
            btnBold = new ToolStripButton("Bold", Resource1.text_bold);
            Items.Add(btnBold);
            btnItalic = new ToolStripButton("Italic", Resource1.text_italic);
            Items.Add(btnItalic);
            btnUnderline = new ToolStripButton("Underline", Resource1.text_underline);
            Items.Add(btnUnderline);
            btnStrikethrough = new ToolStripButton("Strikethrough", Resource1.text_strikethrough);
            Items.Add(btnStrikethrough);
            foreach (ToolStripItem b in Items)
            {
                b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                b.ImageScaling = ToolStripItemImageScaling.None;
                b.Click += btnClick;
                if (b is ToolStripButton)
                    ((ToolStripButton)b).CheckOnClick = true;
            }
        }

        void de_HsmStateChanged(DEngine de, DHsmState state)
        {
            InitPropertyControls(state);
        }


        void de_SelectedFiguresChanged()
        {
            InitPropertyControls(de.HsmState);
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

        void InitPropertyControls(DHsmState state)
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

        void UpdateToDap()
        {
            // TODO update controls to dap
        }

        void UpdateToFigures()
        {
            // TODO update controls to figures
        }

        void dap_PropertyChanged(DAuthorProperties dap)
        {
            if (de.HsmState == DHsmState.Select)
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
                        btnFill.Color = ((ColorPicker)sender2).SelectedColor;
                        if (de.HsmState == DHsmState.Select)
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
                        btnStroke.Color = ((ColorPicker)sender2).SelectedColor;
                        if (de.HsmState == DHsmState.Select)
                            UpdateSelectedFigures(btnStroke);
                        else
                            dap.Stroke = WFHelper.MakeColor(btnStroke.Color);

                    };
                    f.Show();
                }
                else if (sender == btnBold)
                {
                    if (de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnBold);
                    else
                        dap.Bold = btnBold.Checked;
                }
                else if (sender == btnItalic)
                {
                    if (de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnItalic);
                    else
                        dap.Italics = btnItalic.Checked;
                }
                else if (sender == btnUnderline)
                {
                    if (de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnUnderline);
                    else
                        dap.Underline = btnUnderline.Checked;
                }
                else if (sender == btnStrikethrough)
                {
                    if (de.HsmState == DHsmState.Select)
                        UpdateSelectedFigures(btnStrikethrough);
                    else
                        dap.Strikethrough = btnStrikethrough.Checked;
                }
            }
        }

        void btnAlpha_AlphaChanged(object sender, double alpha)
        {
            if (de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnAlpha);
            else
                dap.Alpha = btnAlpha.Value;
        }

        void btnStrokeWidth_StrokeWidthChanged(object sender, int strokeWidth)
        {
            if (de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnStrokeWidth);
            else
                dap.StrokeWidth = btnStrokeWidth.Value;
        }

        void btnStrokeStyle_StrokeStyleChanged(object sender, DStrokeStyle strokeStyle)
        {
            if (de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(btnStrokeStyle);
            else
                dap.StrokeStyle = btnStrokeStyle.Value;
        }

        void btnMarker_MarkerChanged(object sender, DMarker marker)
        {
            if (sender == btnStartMarker)
            {
                if (de.HsmState == DHsmState.Select)
                    UpdateSelectedFigures(btnStartMarker);
                else
                    dap.StartMarker = btnStartMarker.Value;
            }
            else if (sender == btnEndMarker)
            {
                if (de.HsmState == DHsmState.Select)
                    UpdateSelectedFigures(btnEndMarker);
                else
                    dap.EndMarker = btnEndMarker.Value;
            }
        }

        private void cbFontName_FontNameChanged(object sender, EventArgs e)
        {
            if (de.HsmState == DHsmState.Select)
                UpdateSelectedFigures(cbFontName);
            else
                dap.FontName = cbFontName.Value;
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
                dv.Update();
            }
        }
    }
}
