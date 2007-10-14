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
    public partial class Form1 : Form
    {
        DAuthorProperties dap;
        DEngine de = null;

        DViewer dvEditor;

        void CreateDEngine()
        {
            DEngine de = new DEngine(dap);
            previewBar1.AddPreview(de, wfvcEditor);
            // DEngine events
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
        }

        private void SetCurrentDe(DEngine de)
        {
            if (this.de != null)
                this.de.RemoveViewer(dvEditor);
            de.AddViewer(dvEditor);
            dvEditor.Update();
            this.de = de;
            de_SelectedFiguresChanged();
        }

        public Form1()
        {
            InitializeComponent();
            // create author properties
            dap = new DAuthorProperties(DColor.Blue, DColor.Red, 3, 1, "Arial");
            dap.EditModeChanged += new DEditModeChangedHandler(dap_EditModeChanged);
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
            pts.Add(new DPoint(150, 50));
            pts.Add(new DPoint(160, 40));
            pts.Add(new DPoint(170, 70));
            pts.Add(new DPoint(180, 50));
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
            f = new TextFigure(new DPoint(100, 200), "hello dan", new WFTextExtent(), 0);
            de.AddFigure(f);
            TextFigure tf = (TextFigure)f;
            // Init controls
            InitcbFont();
            InitPropertyControls();
        }

        private void InitcbFont()
        {
            InstalledFontCollection ifc = new InstalledFontCollection();
            foreach (FontFamily ff in ifc.Families)
                cbFont.Items.Add(ff.Name);
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
            InitPropertyControls();
        }

        private void InitPropertyControls()
        {
            // set default (blank) values for property controls
            btnFill.BackColor = Color.Empty;
            btnStroke.BackColor = Color.Empty;
            tbStrokeWidth.Value = tbStrokeWidth.Minimum;
            tbAlpha.Value = tbAlpha.Minimum;
            cbFont.SelectedIndex = -1;
            // deselect controls
            btnFill.Enabled = false;
            btnStroke.Enabled = false;
            tbStrokeWidth.Enabled = false;
            tbAlpha.Enabled = false;
            cbFont.Enabled = false;
            // update controls based on the EditMode of DEngine
            switch (dap.EditMode)
            {
                case DEditMode.None:
                    break;
                case DEditMode.Select:
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
                            tbStrokeWidth.Enabled = true;
                        }
                    foreach (Figure f in figs)
                        if (f is IAlphaBlendable)
                            tbAlpha.Enabled = true;
                    foreach (Figure f in figs)
                        if (f is ITextable)
                            cbFont.Enabled = true;
                    // set property controls to match selected figure
                    if (figs.Length == 1)
                    {
                        Figure f = figs[0];
                        if (f is IFillable)
                            btnFill.BackColor = MakeColor(((IFillable)f).Fill);
                        if (f is IStrokeable)
                        {
                            btnStroke.BackColor = MakeColor(((IStrokeable)f).Stroke);
                            tbStrokeWidth.Value = (int)(((IStrokeable)f).StrokeWidth);
                        }
                        if (f is IAlphaBlendable)
                            tbAlpha.Value = (int)(((IAlphaBlendable)f).Alpha * tbAlpha.Maximum);
                        if (f is ITextable)
                            foreach (string item in cbFont.Items)
                                if (item == ((ITextable)f).FontName)
                                {
                                    cbFont.SelectedItem = item;
                                    break;
                                }
                    }
                    break;
                default:
                    // enable relavant controls
                    btnFill.Enabled = dap.EditMode == DEditMode.DrawRect || dap.EditMode == DEditMode.DrawEllipse;
                    btnStroke.Enabled = dap.EditMode == DEditMode.DrawPolyline || dap.EditMode == DEditMode.DrawRect || dap.EditMode == DEditMode.DrawEllipse;
                    tbStrokeWidth.Enabled = btnStroke.Enabled;
                    tbAlpha.Enabled = true;
                    // update values to match dap
                    if (btnFill.Enabled)
                        btnFill.BackColor = MakeColor(dap.Fill);
                    if (btnStroke.Enabled)
                        btnStroke.BackColor = MakeColor(dap.Stroke);
                    if (tbStrokeWidth.Enabled)
                        tbStrokeWidth.Value = (int)dap.StrokeWidth;
                    if (tbAlpha.Enabled)
                        tbAlpha.Value = (int)(dap.Alpha * tbAlpha.Maximum);
                    break;
            }

        }

        void dap_EditModeChanged()
        {
            btnSelect.Checked = dap.EditMode == DEditMode.Select;
            btnPen.Checked = dap.EditMode == DEditMode.DrawPolyline;
            btnRect.Checked = dap.EditMode == DEditMode.DrawRect;
            btnEllipse.Checked = dap.EditMode == DEditMode.DrawEllipse;
            InitPropertyControls();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            previewBar1.ScalePreviewsToViewerControl(wfvcEditor);
        }

        private void btnAntiAlias_Click(object sender, EventArgs e)
        {
            dvEditor.AntiAlias = !dvEditor.AntiAlias;
            btnAntiAlias.Checked = dvEditor.AntiAlias;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnFill.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnFill.BackColor = colorDialog1.Color;
                switch (dap.EditMode)
                {
                    case DEditMode.None:
                        break;
                    case DEditMode.Select:
                        UpdateSelectedFigures(btnFill);
                        break;
                    default:
                        dap.Fill = MakeColor(btnFill.BackColor);
                        break;
                }
            }
        }

        private void btnStroke_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnStroke.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnStroke.BackColor = colorDialog1.Color;
                switch (dap.EditMode)
                {
                    case DEditMode.None:
                        break;
                    case DEditMode.Select:
                        UpdateSelectedFigures(btnStroke);
                        break;
                    default:
                        dap.Stroke = MakeColor(btnStroke.BackColor);
                        break;
                }
            }
        }

        private void tbAlpha_Scroll(object sender, EventArgs e)
        {
            UpdateSelectedFigures(tbAlpha);
            switch (dap.EditMode)
            {
                case DEditMode.None:
                    break;
                case DEditMode.Select:
                    UpdateSelectedFigures(tbAlpha);
                    break;
                default:
                    dap.Alpha = tbAlpha.Value / (float)tbAlpha.Maximum;
                    break;
            }
        }

        private void tsStrokeWidth_Scroll(object sender, EventArgs e)
        {
            switch (dap.EditMode)
            {
                case DEditMode.None:
                    break;
                case DEditMode.Select:
                    UpdateSelectedFigures(tbStrokeWidth);
                    break;
                default:
                    dap.StrokeWidth = tbStrokeWidth.Value;
                    break;
            }
        }

        private void cbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (dap.EditMode)
            {
                case DEditMode.None:
                    break;
                case DEditMode.Select:
                    UpdateSelectedFigures(cbFont);
                    break;
                default:
                    dap.FontName = (string)cbFont.SelectedItem;
                    break;
            }
        }

        private void UpdateSelectedFigures(Control control)
        {
            Figure[] figs = de.SelectedFigures;
            foreach (Figure f in figs)
            {
                if (control == btnFill && f is IFillable)
                    ((IFillable)f).Fill = MakeColor(btnFill.BackColor);
                if (f is IStrokeable)
                {
                    if (control == btnStroke)
                        ((IStrokeable)f).Stroke = MakeColor(btnStroke.BackColor);
                    if (control == tbStrokeWidth)
                        ((IStrokeable)f).StrokeWidth = tbStrokeWidth.Value;
                }
                if (control == tbAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha = tbAlpha.Value / (float)tbAlpha.Maximum;
                if (control == cbFont && f is ITextable)
                    ((ITextable)f).FontName = (string)cbFont.SelectedItem;                    
            }
            dvEditor.Update();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            dap.EditMode = DEditMode.Select;
        }

        private void btnPen_Click(object sender, EventArgs e)
        {
            dap.EditMode = DEditMode.DrawPolyline;
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            dap.EditMode = DEditMode.DrawRect;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            dap.EditMode = DEditMode.DrawEllipse;
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
                de.AddFigure(new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, bmp));
                de.UpdateViewers();
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextForm tf = new TextForm();
            if (tf.ShowDialog() == DialogResult.OK)
            {
                de.ClearSelected();
                de.AddFigure(new TextFigure(new DPoint(10, 10), tf.TextEntered, new WFTextExtent(), 0));
                de.UpdateViewers();
            }
        }
    }
}