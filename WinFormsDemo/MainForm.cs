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

        WorkBookArguments cmdArguments = WorkBookArguments.GlobalWbArgs;
        Ipc ipc = Ipc.GlobalIpc;

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
            // DEngine settings
            WorkBookUtils.SetupDEngine(de);
            // DEngine events
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
            de.ContextClick += new ContextClickHandler(de_ContextClick);
            de.DragFigureStart += new DragFigureHandler(de_DragFigureStart);
            de.DragFigureEvt += new DragFigureHandler(de_DragFigureEvt);
            de.DragFigureEnd += new DragFigureHandler(de_DragFigureEnd);
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
            // update toolstrips
            tsEngineState.De = de;
            tsPropState.De = de;
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
            dap = DAuthorProperties.GlobalAP;
            dap.SetProperties(DColor.Blue, DColor.Red, 3, DStrokeStyle.Solid, DMarker.None, DMarker.None, 1, "Arial", false, false, false, false);
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
            // update some controls and titlebar
            UpdateUndoRedoControls();
            UpdateTitleBar();
            // set toolstrip properties
            tsPropState.Dap = dap;
            tsPropState.Dv = dvEditor;
            // read program options
            ReadOptions();
            // get command line arguments
            ActionCommandLine();
            // connect to ipc messages
            ipc.MessageReceived += new MessageReceivedHandler(ipc_MessageReceived);
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

        void ActionCommandLine()
        {
            if (cmdArguments.FloatingTools)
                ShowFloatingTools(true);
            else
                Show();
        }

        void ipc_MessageReceived(IpcMessage msg)
        {
            if (InvokeRequired)
                Invoke(new MessageReceivedHandler(ipc_MessageReceived), new object[] { msg });
            else
                switch (msg)
                {
                    case IpcMessage.Show:
                        if (WindowState == FormWindowState.Minimized)
                            WindowState = FormWindowState.Normal;
                        Show();
                        Focus();
                        if (FloatingToolsForm.GlobalFT.Visible)
                            FloatingToolsForm.GlobalFT.Alone = false;
                        break;
                    case IpcMessage.FloatingTools:
                        ShowFloatingTools(false);
                        break;
                }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckDirty())
                e.Cancel = true;
            else
                WriteOptions();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        void DebugMessage(string msg)
        {
            lbInfo.Text = msg;
        }

        void de_SelectedFiguresChanged()
        {
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
            {
                previewBar1.MatchPreviewsToEngines(dem.GetEngines(), dvEditor);
                foreach (DEngine de in dem.GetEngines())
                    de.UpdateViewers();
            }
            previewBar1.UpdatePreviewsDirtyProps();
        }

        void de_ContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (clickedFigure != null)
                cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
            else
                cmsCanvas.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
        }

        void de_DragFigureStart(DEngine de, Figure dragFigure, DPoint pt)
        {
        }

        void de_DragFigureEvt(DEngine de, Figure dragFigure, DPoint pt)
        {
            if (pt.X < 0 || pt.Y < 0 || pt.X > wfvcEditor.Width || pt.Y > wfvcEditor.Height)
            {
                de.CancelFigureDrag();
                dvEditor.Update();
                wfvcEditor.DoDragDrop(de.SelectedFigures, DragDropEffects.Move);
            }
        }

        void de_DragFigureEnd(DEngine de, Figure dragFigure, DPoint pt)
        {
        }

        void contextGlyph_Clicked(IGlyph glyph, Figure figure, DPoint pt)
        {
            cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
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

        void de_AddedFigure(DEngine de, Figure fig)
        {
            AddGlyphs(fig);
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

        private void previewBar1_PreviewFigureDrop(Preview p, List<Figure> figs)
        {
            if (!p.Selected)
            {
                dem.UndoRedoStart("Drag to New Page");
                foreach (Figure f in figs)
                {
                    de.RemoveFigure(f);
                    p.DEngine.AddFigure(f);
                }
                de.ClearSelected();
                dem.UndoRedoCommit();
                de.UpdateViewers();
                p.DEngine.UpdateViewers();
            }
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
                byte[] imageData = WFHelper.ToImageData((Bitmap)bmp.NativeBmp);
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
            WorkBookUtils.ViewerKeyDown(de, e);
        }

        private void wfvcEditor_KeyUp(object sender, KeyEventArgs e)
        {
            WorkBookUtils.ViewerKeyUp(de, e);
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
                byte[] imageData = WFHelper.ToImageData(bmp);
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
                try
                {
                    // load new engines
                    List<DEngine> engines = FileHelper.Load(ofd.FileName, dap, true);
                    dem.SetEngines(engines);
                    fileName = ofd.FileName;
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
                catch (Exception e)
                {
                    dem.UndoRedoCancel();
                    MessageBox.Show(e.Message, "Error Reading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void Save()
        {
            try
            {
                FileHelper.Save(fileName, dem.GetEngines());
                dem.Dirty = false;
                beenSaved = true;
                UpdateTitleBar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Writing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void actFloatingTools_Execute(object sender, EventArgs e)
        {
            ShowFloatingTools(false);
        }

        void ShowFloatingTools(bool floatingToolsAlone)
        {
            FloatingToolsForm ff = FloatingToolsForm.GlobalFT;
            ff.ImportAnnotationsPage += new ImportAnnotationsPageHandler(FloatingTools_ImportAnnotationsPage);
            ff.ImportAnnotationsArea += new ImportAnnotationsImageHandler(FloatingTools_ImportAnnotationsArea);
            ff.Owner = this;
            ff.Alone = floatingToolsAlone;
            ff.Show();
            ff.Focus();
        }

        void FloatingTools_ImportAnnotationsPage(DEngine de)
        {
            dem.UndoRedoStart("Import Annotations");
            CreateDEngine(null);
            this.de.PageSize = de.PageSize;
            this.de.SetBackgroundFigure(de.GetBackgroundFigure());
            foreach (Figure f in de.Figures)
                this.de.AddFigure(f);
            dem.UndoRedoCommit();
        }

        void FloatingTools_ImportAnnotationsArea(DBitmap bmp)
        {
            de.UndoRedoStart("Import Annotations");
            ImageFigure f = new ImageFigure(new DRect(10, 10, bmp.Width - 1, bmp.Height - 1), 0, WFHelper.ToImageData((Bitmap)bmp.NativeBmp), "annotations.png");
            de.AddFigure(f);
            dvEditor.Update();
            de.UndoRedoCommit();
        }

        private void previewBar1_PreviewFigureDrop(Preview p)
        {

        }
    }
}