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
            previewBar1.AddPreview(de, dvEditor);
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
            dvEditor.Update();
            this.de = de;
            de_SelectedFiguresChanged();
            UndoRedoMgr_UndoRedoChanged(false);
        }

        public Form1()
        {
            InitializeComponent();
            // Initialze DGraphics
            WFGraphics.Init();
            // create author properties
            dap = new DAuthorProperties(DColor.Blue, DColor.Red, 3, 1, "Arial");
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
            f = new TextFigure(new DPoint(100, 200), "hello dan", GraphicsHelper.TextExtent, 0);
            de.AddFigure(f);
            // compositing figure
            f = new CompositedExampleFigure();
            f.Rect = new DRect(20, 150, 50, 50);
            de.AddFigure(f);
            // Init controls
            InitPropertyControls();
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
                a4ToolStripMenuItem.Checked = false;
                a5ToolStripMenuItem.Checked = false;
                letterToolStripMenuItem.Checked = false;
                customToolStripMenuItem.Checked = false;
                switch (de.PageFormat)
                {
                    case PageFormat.A4:
                        a4ToolStripMenuItem.Checked = true;
                        break;
                    case PageFormat.A5:
                        a5ToolStripMenuItem.Checked = true;
                        break;
                    case PageFormat.Letter:
                        letterToolStripMenuItem.Checked = true;
                        break;
                    case PageFormat.Custom:
                        customToolStripMenuItem.Checked = true;
                        break;
                }
                cmsBackground.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
            }
        }

        private void InitPropertyControls()
        {
            // disable events
            cbFontName.FontNameChanged -= cbFontName_FontNameChanged;
            // set default (blank) values for property controls
            btnFill.Color = Color.Empty;
            btnStroke.Color = Color.Empty;
            btnStrokeWidth.Value = 1;
            btnAlpha.Value = 1;
            cbFontName.Value = "";
            // deselect controls
            btnFill.Enabled = false;
            btnStroke.Enabled = false;
            btnStrokeWidth.Enabled = false;
            btnAlpha.Enabled = false;
            cbFontName.Enabled = false;
            // update controls based on the state of DEngine
            switch (de.State)
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
                        }
                    foreach (Figure f in figs)
                        if (f is IAlphaBlendable)
                            btnAlpha.Enabled = true;
                    foreach (Figure f in figs)
                        if (f is ITextable)
                            cbFontName.Enabled = true;
                    // set property controls to match selected figure
                    if (figs.Length == 1)
                    {
                        Figure f = figs[0];
                        if (f is IFillable)
                            btnFill.Color = MakeColor(((IFillable)f).Fill);
                        if (f is IStrokeable)
                        {
                            btnStroke.Color = MakeColor(((IStrokeable)f).Stroke);
                            btnStrokeWidth.Value = (int)(((IStrokeable)f).StrokeWidth);
                        }
                        if (f is IAlphaBlendable)
                            btnAlpha.Value = ((IAlphaBlendable)f).Alpha;
                        if (f is ITextable)
                            cbFontName.Value = ((ITextable)f).FontName;
                    }
                    break;
                default:
                    // enable relavant controls
                    btnFill.Enabled = de.State == DEngineState.DrawRect || de.State == DEngineState.DrawEllipse || de.State == DEngineState.DrawText;
                    btnStroke.Enabled = de.State == DEngineState.DrawPolyline || de.State == DEngineState.DrawRect || de.State == DEngineState.DrawEllipse;
                    btnStrokeWidth.Enabled = btnStroke.Enabled;
                    btnAlpha.Enabled = btnFill.Enabled || btnStroke.Enabled;
                    cbFontName.Enabled = de.State == DEngineState.DrawText;
                    // update values to match dap
                    if (btnFill.Enabled)
                        btnFill.Color = MakeColor(dap.Fill);
                    if (btnStroke.Enabled)
                        btnStroke.Color = MakeColor(dap.Stroke);
                    if (btnStrokeWidth.Enabled)
                        btnStrokeWidth.Value = (int)dap.StrokeWidth;
                    if (btnAlpha.Enabled)
                        btnAlpha.Value = dap.Alpha;
                    if (cbFontName.Enabled)
                        cbFontName.Value = dap.FontName;
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

        void de_StateChanged()
        {
            btnSelect.Checked = de.State == DEngineState.Select;
            btnPen.Checked = de.State == DEngineState.DrawPolyline;
            btnRect.Checked = de.State == DEngineState.DrawRect;
            btnEllipse.Checked = de.State == DEngineState.DrawEllipse;
            btnText.Checked = de.State == DEngineState.DrawText;
            InitPropertyControls();
        }

        private void btnAntiAlias_Click(object sender, EventArgs e)
        {
            dvEditor.AntiAlias = !dvEditor.AntiAlias;
            btnAntiAlias.Checked = dvEditor.AntiAlias;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnFill.Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnFill.Color = colorDialog1.Color;
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnFill);
                        break;
                    default:
                        dap.Fill = MakeColor(btnFill.Color);
                        break;
                }
            }
        }

        private void btnStroke_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnStroke.Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnStroke.Color = colorDialog1.Color;
                switch (de.State)
                {
                    case DEngineState.Select:
                        UpdateSelectedFigures(btnStroke);
                        break;
                    default:
                        dap.Stroke = MakeColor(btnStroke.Color);
                        break;
                }
            }
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
                }
                if (sender == btnAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha = btnAlpha.Value;
                if (sender == cbFontName && f is ITextable)
                    ((ITextable)f).FontName = cbFontName.Value;     
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
            de.State = DEngineState.DrawPolyline;
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            de.State = DEngineState.DrawRect;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            de.State = DEngineState.DrawEllipse;
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            de.State = DEngineState.DrawText;
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

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextForm tf = new TextForm();
            if (tf.ShowDialog() == DialogResult.OK)
            {
                de.ClearSelected();
				de.UndoRedoMgr.Start("Add Text");
                de.AddFigure(new TextFigure(new DPoint(10, 10), tf.TextEntered, GraphicsHelper.TextExtent, 0));
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
            if (de.State == DEngineState.TextEdit)
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
    }
}