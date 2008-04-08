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
using WinFormsDemo.PersonalToolbar;

namespace WinFormsDemo
{
    public partial class MainForm : WorkBookForm
    {
        DEngineManager dem;
        DAuthorProperties dap;
        DEngine de = null;

        DTkViewer dvEditor;

        BitmapGlyph contextGlyph;
        BitmapGlyph linkGlyph;
        DPoint textInsertionPoint;
        bool nonTextInsertionKey;

        bool previewBar1Focused;

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
            InitDEngine(de, true);
        }

        void InitDEngine(DEngine de, bool showIt)
        {
            // DEngine settings
            WorkBookUtils.SetupDEngine(de);
            // DEngine events
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
            de.FigureClick += new ClickHandler(de_FigureClick);
            de.ContextClick += new ClickHandler(de_ContextClick);
            de.DragFigureStart += new DragFigureHandler(de_DragFigureStart);
            de.DragFigureEvt += new DragFigureHandler(de_DragFigureEvt);
            de.DragFigureEnd += new DragFigureHandler(de_DragFigureEnd);
            de.MouseDown += new DMouseButtonEventHandler(de_MouseDown);
            de.AddedFigure += new AddedFigureHandler(de_AddedFigure);
            // add glyphs to figures
            foreach (Figure f in de.Figures)
                AddDefaultGlyphs(f);
            // show it
            if (showIt)
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
                    de.HsmState = DHsmState.Select;
            }
            de.AddViewer(dvEditor);
            if (dvEditor.Zoom != Zoom.Custom)
                dvEditor.Zoom = dvEditor.Zoom;
            dvEditor.Update();
            this.de = de;
            de_SelectedFiguresChanged();
            UpdateUndoRedoActions();
            // update toolstrips
            tsEngineState.De = de;
            tsPropState.De = de;
            // null textInsertionPoint
            textInsertionPoint = null;
        }

        public MainForm()
        {
            InitializeComponent();
            // Initialze DGraphics
            WFGraphics.Init();
            // DEngine Manager
            dem = new DEngineManager();
            dem.UndoRedoChanged += new EventHandler(dem_UndoRedoChanged);
            attachmentView1.EngineManager = dem;
            // create author properties
            dap = DAuthorProperties.GlobalAP;
            dap.SetProperties(DColor.Blue, DColor.Red, 3, DStrokeStyle.Solid, DMarker.None, DMarker.None, 1, "Arial", false, false, false, false);
            // edit viewer
            dvEditor = new WFViewer(wfvcEditor);
            dvEditor.EditFigures = true;
            dvEditor.DebugMessage += new DebugMessageHandler(DebugMessage);
            // glyphs
            contextGlyph = new BitmapGlyph(new WFBitmap(Resource1.arrow), DGlyphPosition.TopRight);
            contextGlyph.Cursor = DCursor.Hand;
            contextGlyph.Clicked += new GlyphClickedHandler(contextGlyph_Clicked);
            linkGlyph = new BitmapGlyph(new WFBitmap(Resource1.link), DGlyphPosition.BottomLeft);
            linkGlyph.Visiblility = DGlyphVisiblity.Always;
            linkGlyph.Cursor = DCursor.Hand;
            linkGlyph.Clicked += new GlyphClickedHandler(linkGlyph_Clicked);
            // figure defaults
            Figure._handleSize = 6;
            Figure._handleBorder = 4;
            Figure._rotateHandleStemLength = Figure._handleSize - Figure._handleBorder;
            LinebaseFigure._hitTestExtension = 5;
            // new document
            New();
            dem.UndoRedoClearHistory();
            // update some controls and titlebar
            UpdateUndoRedoActions();
            UpdateTitleBar();
            // set toolstrip properties
            tsPropState.Dap = dap;
            tsPropState.Dv = dvEditor;
            // connect to ipc messages
            ipc.MessageReceived += new MessageReceivedHandler(ipc_MessageReceived);
            // get command line arguments
            ActionCommandLine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // read program options
            ReadOptions();
            // read toolstrip settings
            ToolStripManager.SaveSettings(this, "reset");
            ToolStripManager.LoadSettings(this);
        }

        void ReadOptions()
        {
            ProgramOptions options = new ProgramOptions();
            // MainForm options
            SetBounds(options.FormRect.Left, options.FormRect.Top, options.FormRect.Width, options.FormRect.Height);
            if (options.FormWindowState != FormWindowState.Minimized)
                WindowState = options.FormWindowState;
            SidebarSide = options.SidebarSide;
            SidebarWidth = options.SidebarWidth;
            // dvEditor options
            dvEditor.AntiAlias = options.AntiAlias;
            if (options.Zoom == Zoom.Custom)
                dvEditor.Scale = options.Scale;
            else
                dvEditor.Zoom = options.Zoom;
            // load personal toolbar
            PtUtils.LoadPersonalTools(tsPersonal);
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
            options.SidebarSide = SidebarSide;
            options.SidebarWidth = SidebarWidth;
            // dvEditor options
            options.Zoom = dvEditor.Zoom;
            options.Scale = dvEditor.Scale;
            options.AntiAlias = dvEditor.AntiAlias;
            // write to file
            options.WriteIni();
            // save personal toolbar
            PtUtils.SavePersonalTools(tsPersonal);
        }

        void ActionCommandLine()
        {
            if (cmdArguments.FloatingTools)
                ShowFloatingTools(true);
            else
            {
                // if second unnamed cmd line argument is a file open it (first is this executable)
                if (cmdArguments.UnnamedParamCount > 1 && File.Exists(cmdArguments[1]))
                    OpenFile(cmdArguments[1]);
                // show form
                Show();
            }
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
            {
                // write user options
                WriteOptions();
                // write toolstrip settings
                ToolStripManager.SaveSettings(this);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        void DebugMessage(string msg)
        {
            lbInfo.Text = msg;
        }

        void CheckState()
        {
            // Change to the 'Select' state if we are in a volatile state like text editing
            if (de.HsmState == DHsmState.TextEdit)
                de.HsmState = DHsmState.Select;
        }

        void de_SelectedFiguresChanged()
        {
            InitActions();
        }

        void UpdateUndoRedoActions()
        {
            actUndo.Enabled = dem.CanUndo(de);
            if (actUndo.Enabled)
                actUndo.Text = string.Format("Undo \"{0}\"", dem.UndoCaption(de));
            else
                actUndo.Text = "Undo";
            actRedo.Enabled = dem.CanRedo(de);
            if (actRedo.Enabled)
                actRedo.Text = string.Format("Redo \"{0}\"", dem.RedoCaption(de));
            else
                actRedo.Text = "Redo";
        }

        void dem_UndoRedoChanged(object sender, EventArgs e)
        {
            UpdateUndoRedoActions();
            UpdateTitleBar();
            if (sender == dem)
            {
                // check if engines have changed order
                previewBar1.MatchPreviewsToEngines(dem.GetEngines(), dvEditor);
                foreach (DEngine de in dem.GetEngines())
                    de.UpdateViewers();
            }
            else
                // check if UserAttrs have changed and update glyphs
                foreach (Figure f in de.Figures)
                    CheckLinkGlyph(f);
            // check if attachments have changed
            attachmentView1.UpdateAttachmentView();
            // update previews dirty states
            previewBar1.UpdatePreviewsDirtyProps();
        }

        void de_FigureClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (clickedFigure.UserAttrs.ContainsKey(Links.LinkBody))
                ExecLink(clickedFigure);
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

        void de_MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            textInsertionPoint = pt;
        }

        void contextGlyph_Clicked(IGlyph glyph, Figure figure, DPoint pt)
        {
            cmsFigure.Show(wfvcEditor, new Point((int)pt.X, (int)pt.Y));
        }

        void ExecLink(Figure figure)
        {
            if (figure.UserAttrs.ContainsKey(Links.Link) && figure.UserAttrs.ContainsKey(Links.LinkType))
            {
                string link = figure.UserAttrs[Links.Link];
                LinkType linkType = Links.StringToLinkType(figure.UserAttrs[Links.LinkType]);
                switch (linkType)
                {
                    case LinkType.WebPage:
                        System.Diagnostics.Process.Start(link);
                        break;
                    case LinkType.File:
                        if (File.Exists(link))
                            System.Diagnostics.Process.Start(link);
                        else
                            MessageBox.Show(string.Format("Could not find file \"{0}\"", link), "File link error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case LinkType.Page:
                        LinkPage lp = (LinkPage)Enum.Parse(typeof(LinkPage), link, true);
                        switch (lp)
                        {
                            case LinkPage.First:
                                previewBar1.SetPreviewSelected(dem.GetEngine(0));
                                break;
                            case LinkPage.Last:
                                previewBar1.SetPreviewSelected(dem.GetEngine(dem.EngineCount - 1));
                                break;
                            case LinkPage.Next:
                                int idx = dem.IndexOfEngine(de) + 1;
                                if (idx < dem.EngineCount)
                                    previewBar1.SetPreviewSelected(dem.GetEngine(idx));
                                else
                                    goto case LinkPage.First;
                                break;
                            case LinkPage.Previous:
                                int idx2 = dem.IndexOfEngine(de) - 1;
                                if (idx2 > 0)
                                    previewBar1.SetPreviewSelected(dem.GetEngine(idx2));
                                else
                                    goto case LinkPage.Last;
                                break;
                            default:
                                int n = 0;
                                int.TryParse(link, out n);
                                if (n >= 0 && n < dem.EngineCount)
                                    previewBar1.SetPreviewSelected(dem.GetEngine(n));
                                else
                                    MessageBox.Show(string.Format("Page \"{0}\" does not exist", n + 1), "Page link error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                        break;
                    case LinkType.Attachment:
                        attachmentView1.ExecuteAttachment(link);
                        break;
                }
            }
        }

        void linkGlyph_Clicked(IGlyph glyph, Figure figure, DPoint pt)
        {
            ExecLink(figure);
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
            // update link action
            actLink.Enabled = figs.Count == 1;
        }

        void AddDefaultGlyphs(Figure fig)
        {
            // make sure fig.Glyphs is assigned
            if (fig.Glyphs == null)
                fig.Glyphs = new List<IGlyph>();
            // add context glyph if not already there
            foreach (IGlyph g in fig.Glyphs)
                if (g == contextGlyph)
                    return;
            fig.Glyphs.Add(contextGlyph);
            CheckLinkGlyph(fig);
            // recurse into child figures
            if (fig is GroupFigure)
                foreach (Figure child in ((GroupFigure)fig).ChildFigures)
                    AddDefaultGlyphs(child);
        }

        void CheckLinkGlyph(Figure fig)
        {
            if (fig.UserAttrs.ContainsKey(Links.Link) && !fig.UserAttrs.ContainsKey(Links.LinkBody))
            {
                if (!fig.Glyphs.Contains(linkGlyph))
                    fig.Glyphs.Insert(0, linkGlyph);
            }
            else
            {
                if (fig.Glyphs.Contains(linkGlyph))
                    fig.Glyphs.Remove(linkGlyph);
            }
            fig.ClickEvent = fig.UserAttrs.ContainsKey(Links.LinkBody);
        }

        void de_AddedFigure(DEngine de, Figure fig)
        {
            AddDefaultGlyphs(fig);
        }

        private void previewBar1_Enter(object sender, EventArgs e)
        {
            // previewBar1.Focused does not seem to be working :(
            previewBar1Focused = true;
        }

        private void previewBar1_Leave(object sender, EventArgs e)
        {
            // previewBar1.Focused does not seem to be working :(
            previewBar1Focused = false;
        }

        private void previewBar1_PreviewSelected(Preview p)
        {
            SetCurrentDe(p.DEngine);
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
                CheckState();
                de.ClearSelected();
                WFBitmap bmp = new WFBitmap(ofd.FileName);
                de.UndoRedoStart("Add Image");
                byte[] imageData = WFHelper.ToImageData((Bitmap)bmp.NativeBmp);
                de.AddFigure(new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, imageData, ofd.FileName));
                de.UndoRedoCommit();
                de.UpdateViewers();
            }
        }

        private void attachmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                dem.UndoRedoStart("Add Attachment");
                if (attachmentView1.CheckAttachmentExists(ofd.FileName))
                    attachmentView1.AddAttachment(ofd.FileName);
                dem.UndoRedoCommit();
            }
        }

        private void resetToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripManager.LoadSettings(this, "reset");
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
            nonTextInsertionKey = e.Alt || e.Control || 
                e.KeyValue == (int)Keys.Escape || e.KeyValue == (int)Keys.Enter ||
                e.KeyValue == (int)Keys.Space;
        }

        private void wfvcEditor_KeyUp(object sender, KeyEventArgs e)
        {
            WorkBookUtils.ViewerKeyUp(de, e);
            if (e.KeyValue == (int)Keys.Escape)
                de.HsmState = DHsmState.Select;
        }

        private void wfvcEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!nonTextInsertionKey && textInsertionPoint != null && de.HsmState != DHsmState.TextEdit)
            {
                de.UndoRedoStart("Text Edit");
                TextFigure tf = new TextFigure(textInsertionPoint, "", 0);
                dap.ApplyPropertiesToFigure(tf);
                de.AddFigure(tf);
                de.HsmTextEdit(tf);
                dvEditor.Update();
            }
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

        void PasteDataObject(IDataObject iData, string opPrefix, double objX, double objY)
        {
            if (iData.GetDataPresent(FigureSerialize.DDRAW_FIGURE_XML))
                de.PasteAsSelectedFigures((string)iData.GetData(FigureSerialize.DDRAW_FIGURE_XML));
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                de.UndoRedoStart(string.Format("{0} Text", opPrefix));
                TextFigure f = new TextFigure(new DPoint(objX, objY), (string)iData.GetData(DataFormats.Text), 0);
                dap.ApplyPropertiesToFigure(f);
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                de.UndoRedoCommit();
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                de.UndoRedoStart(string.Format("{0} Bitmap", opPrefix));
                Bitmap bmp = (Bitmap)iData.GetData(DataFormats.Bitmap, true);
                byte[] imageData = WFHelper.ToImageData(bmp);
                ImageFigure f = new ImageFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0, imageData, "Clipboard.bmp");
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                de.UndoRedoCommit();
            }
            else if (iData.GetDataPresent(DataFormats.FileDrop))
            {
                de.UndoRedoStart(string.Format("{0} File", opPrefix));
                string path = ((string[])iData.GetData(DataFormats.FileDrop))[0];
                if (IsImageFilePath(path))
                {
                    Bitmap bmp = (Bitmap)Bitmap.FromFile(path);
                    ImageFigure f = new ImageFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0,
                        WFHelper.ToImageData(bmp), path);
                    de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                }
                else if (attachmentView1.CheckAttachmentExists(path))
                {
                    TextFigure f = new TextFigure(new DPoint(objX, objY), Path.GetFileName(path), 0);
                    f.UserAttrs[Links.Link] = attachmentView1.AddAttachment(path);
                    f.UserAttrs[Links.LinkType] = LinkType.Attachment.ToString();
                    dap.ApplyPropertiesToFigure(f);
                    de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                }
                de.UndoRedoCommit();
            }
            else if (iData.GetDataPresent(attachmentView1.GetType()))
            {
                de.UndoRedoStart(string.Format("{0} Attachment", opPrefix));
                foreach (ListViewItem item in attachmentView1.SelectedItems)
                {
                    if (IsImageFilePath(item.Text))
                    {
                        using (MemoryStream ms = new MemoryStream(attachmentView1.GetAttachment(item.Text)))
                        {
                            Bitmap bmp = (Bitmap)Bitmap.FromStream(ms);
                            ImageFigure f = new ImageFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0,
                                WFHelper.ToImageData(bmp), item.Text);
                            de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                        }
                    }
                    else
                    {
                        TextFigure f = new TextFigure(new DPoint(objX, objY), item.Text, 0);
                        f.UserAttrs[Links.Link] = item.Text;
                        f.UserAttrs[Links.LinkType] = LinkType.Attachment.ToString();
                        dap.ApplyPropertiesToFigure(f);
                        de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                    }
                }
                de.UndoRedoCommit();
            }
        }

        private void actPaste_Execute(object sender, EventArgs e)
        {
            CheckState();
            PasteDataObject(Clipboard.GetDataObject(), "Paste", 10, 10);
        }

        bool AttachmentLinked(string name)
        {
            foreach (DEngine de in dem.GetEngines())
                foreach (Figure f in de.Figures)
                {
                    if (f.UserAttrs.ContainsKey(Links.Link) && f.UserAttrs.ContainsKey(Links.LinkType))
                    {
                        if (f.UserAttrs[Links.LinkType] == LinkType.Attachment.ToString() && f.UserAttrs[Links.Link] == name)
                            return true;
                    }
                }
            return false;
        }

        void DoDelete()
        {
            if (wfvcEditor.Focused)
                de.Delete(de.SelectedFigures);
            else if (previewBar1Focused)
            {
                CheckState();
                dem.UndoRedoStart("Delete Page");
                dem.RemoveEngine(de);
                if (dem.EngineCount == 0)
                    CreateDEngine(de);
                dem.UndoRedoCommit();
            }
            else if (attachmentView1.Focused)
            {
                foreach (ListViewItem item in attachmentView1.SelectedItems)
                    if (AttachmentLinked(item.Text))
                    {
                        if (MessageBox.Show("One of the attachments is linked by a figure. Are you sure you want to delete it?",
                            "Attachment is Linked", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            break;
                        else
                            return;
                    }
                dem.UndoRedoStart("Delete Attachments");
                foreach (ListViewItem item in attachmentView1.SelectedItems)
                    attachmentView1.RemoveAttachment(item);
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
            attachmentView1.ClearAttachments();
            dem.UndoRedoCommit();
            dem.Dirty = false;

            UpdateTitleBar();
        }

        void OpenFile(string fileName)
        {
            // create progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = "Opening File";
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // start undo/redo
                dem.UndoRedoStart("Open Document");
                try
                {
                    // load new engines
                    Dictionary<string, byte[]> extraEntries;
                    List<DEngine> engines = FileHelper.Load(fileName, dap, true,
                        new string[] { AttachmentView.ATTACHMENTS_DIR }, out extraEntries);
                    dem.Clear();
                    // init new dengines
                    foreach (DEngine newDe in engines)
                    {
                        InitDEngine(newDe, false);
                        dem.AddEngine(newDe);
                        Application.DoEvents(); // update progress form
                    }
                    // set attachments
                    attachmentView1.ClearAttachments();
                    if (extraEntries != null)
                        foreach (string name in extraEntries.Keys)
                            if (name.IndexOf(AttachmentView.ATTACHMENTS_DIR) == 0)
                                attachmentView1.AddAttachment(name, extraEntries[name]);
                    // commit undo/redo
                    dem.UndoRedoCommit();
                    dem.Dirty = false;
                    // update vars
                    this.fileName = fileName;
                    beenSaved = true;
                    UpdateTitleBar();
                    // show first page
                    ShowFirstPage();
                }
                catch (Exception e2)
                {
                    dem.UndoRedoCancel();
                    MessageBox.Show(e2.Message, "Error Reading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                pf.Close();
            };
            pf.ShowDialog();
        }

        void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = FileExt;
            ofd.Filter = FileTypeFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenFile(ofd.FileName);
        }

        void Save()
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = "Saving File";
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                try
                {
                    // make extra enties dict (from attachments)
                    Dictionary<string, byte[]> extraEntries = new Dictionary<string, byte[]>();
                    foreach (string name in attachmentView1.GetAttachmentNames())
                        extraEntries.Add(AttachmentView.ATTACHMENTS_DIR + Path.DirectorySeparatorChar + name,
                            attachmentView1.GetAttachment(name));
                    // save
                    FileHelper.Save(fileName, dem.GetEngines(), extraEntries);
                    dem.Dirty = false;
                    beenSaved = true;
                    UpdateTitleBar();
                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2.Message, "Error Writing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                pf.Close();
            };
            pf.ShowDialog();
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
            CheckState();
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
            CheckState();
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
            CheckState();
            de.ClearPage();
        }

        private void actFloatingTools_Execute(object sender, EventArgs e)
        {
            ShowFloatingTools(false);
        }

        private void actLink_Execute(object sender, EventArgs e)
        {
            List<Figure> figs = de.SelectedFigures;
            if (figs.Count == 1)
            {
                Figure f = figs[0];
                LinkForm lf = new LinkForm();
                lf.CurrentEngine = de;
                lf.Engines = dem.GetEngines();
                lf.Attachments = attachmentView1.GetAttachmentNames();
                if (f.UserAttrs.ContainsKey(Links.LinkType) && f.UserAttrs.ContainsKey(Links.Link))
                {
                    lf.LinkType = Links.StringToLinkType(f.UserAttrs[Links.LinkType]);
                    switch (lf.LinkType)
                    {
                        case LinkType.WebPage:
                            lf.WebPage = f.UserAttrs[Links.Link];
                            break;
                        case LinkType.File:
                            lf.File = f.UserAttrs[Links.Link];
                            lf.CopyFileToAttachments = false;
                            break;
                        case LinkType.Page:
                            string link = f.UserAttrs[Links.Link];
                            lf.LinkPage = (LinkPage)Enum.Parse(typeof(LinkPage), link, true); ;
                            if (lf.LinkPage == LinkPage.None)
                            {
                                int n = 0;
                                int.TryParse(link, out n);
                                lf.PageNum = n;
                            }
                            break;
                        case LinkType.Attachment:
                            lf.Attachment = f.UserAttrs[Links.Link];
                            break;
                    }
                }
                lf.LinkBody = f.UserAttrs.ContainsKey(Links.LinkBody);
                switch (lf.ShowDialog())
                {
                    case DialogResult.OK:
                        de.UndoRedoStart("Change Link");
                        f.UserAttrs[Links.LinkType] = lf.LinkType.ToString();
                        switch (lf.LinkType)
                        {
                            case LinkType.WebPage:
                                f.UserAttrs[Links.Link] = lf.WebPage;
                                break;
                            case LinkType.File:
                                if (lf.CopyFileToAttachments && attachmentView1.CheckAttachmentExists(lf.File))
                                {
                                    f.UserAttrs[Links.LinkType] = LinkType.Attachment.ToString();
                                    string name = attachmentView1.AddAttachment(lf.File);
                                    f.UserAttrs[Links.Link] = name;
                                }
                                else
                                    f.UserAttrs[Links.Link] = lf.File;
                                break;
                            case LinkType.Page:
                                if (lf.LinkPage == LinkPage.None)
                                    f.UserAttrs[Links.Link] = lf.PageNum.ToString();
                                else
                                    f.UserAttrs[Links.Link] = lf.LinkPage.ToString();
                                break;
                            case LinkType.Attachment:
                                f.UserAttrs[Links.Link] = lf.Attachment;
                                break;
                        }
                        if (lf.LinkBody)
                            f.UserAttrs[Links.LinkBody] = "";
                        else
                            f.UserAttrs.Remove(Links.LinkBody);
                        // remove dead enties
                        if (f.UserAttrs[Links.Link] == null || f.UserAttrs[Links.Link].Length == 0)
                        {
                            f.UserAttrs.Remove(Links.Link);
                            f.UserAttrs.Remove(Links.LinkType);
                            f.UserAttrs.Remove(Links.LinkBody);
                        }
                        de.UndoRedoCommit();
                        CheckLinkGlyph(f);
                        break;
                    case DialogResult.Abort:
                        de.UndoRedoStart("Remove Link");
                        f.UserAttrs.Remove(Links.Link);
                        f.UserAttrs.Remove(Links.LinkType);
                        f.UserAttrs.Remove(Links.LinkBody);
                        de.UndoRedoCommit();
                        CheckLinkGlyph(f);
                        break;
                }
                // update editor view
                dvEditor.Update();
            }
        }

        private void actUndo_Execute(object sender, EventArgs e)
        {
            dem.Undo(de);
        }

        private void actRedo_Execute(object sender, EventArgs e)
        {
            dem.Redo(de);
        }

        private void actSelectAll_Execute(object sender, EventArgs e)
        {
            if (de.HsmState == DHsmState.Select)
                de.SelectAll();
        }

        void ShowFloatingTools(bool floatingToolsAlone)
        {
            FloatingToolsForm ff = FloatingToolsForm.GlobalFT;
            if (!ff.Visible)
            {
                ff.ImportAnnotationsPage += new ImportAnnotationsPageHandler(FloatingTools_ImportAnnotationsPage);
                ff.ImportAnnotationsArea += new ImportAnnotationsImageHandler(FloatingTools_ImportAnnotationsArea);
                ff.Owner = this;
                ff.Alone = floatingToolsAlone;
                ff.Show();
            }
            ff.Focus();
        }

        void FloatingTools_ImportAnnotationsPage(DEngine de)
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = "Importing Screen as Page";
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // import annotations
                dem.UndoRedoStart("Import Annotations");
                CreateDEngine(null);
                this.de.PageSize = de.PageSize;
                this.de.SetBackgroundFigure(de.GetBackgroundFigure());
                foreach (Figure f in de.Figures)
                    this.de.AddFigure(f);
                dem.UndoRedoCommit();
                // close dialog
                pf.Close();
            };
            pf.ShowDialog();
        }

        void FloatingTools_ImportAnnotationsArea(DBitmap bmp)
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = "Importing Area as Image";
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // import annotations
                de.UndoRedoStart("Import Annotations");
                ImageFigure f = new ImageFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, WFHelper.ToImageData((Bitmap)bmp.NativeBmp), "annotations.png");
                de.AddFigure(f);
                dvEditor.Update();
                de.UndoRedoCommit();
                // close dialog
                pf.Close();
            };
            pf.ShowDialog();
        }

        // wfvcEditor drag & drop

        private void wfvcEditor_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.Bitmap) ||
                    e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(attachmentView1.GetType()))
                    e.Effect = DragDropEffects.Copy;
            }
        }

        bool IsImageFilePath(string path)
        {
            string ext = Path.GetExtension(path);
            if (string.Compare(ext, ".png", true) == 0 ||
                string.Compare(ext, ".gif", true) == 0 ||
                string.Compare(ext, ".jpg", true) == 0 ||
                string.Compare(ext, ".jpeg", true) == 0 ||
                string.Compare(ext, ".bmp", true) == 0)
                return true;
            return false;
        }

        private void wfvcEditor_DragDrop(object sender, DragEventArgs e)
        {
            CheckState();
            Point cpt = wfvcEditor.PointToClient(new Point(e.X, e.Y));
            DPoint pt = dvEditor.ClientToEngine(new DPoint(cpt.X, cpt.Y));
            PasteDataObject(e.Data, "Copy", pt.X, pt.Y);
        }

        // Sidebar //////////////////////////////////////////////////////

        int SidebarWidth
        {
            get
            {
                if (SidebarSide == SidebarSide.Left)
                    return splitContainer1.SplitterDistance;
                else
                    return splitContainer1.Width - splitContainer1.SplitterDistance;
            }
            set
            {
                if (SidebarSide == SidebarSide.Left)
                    splitContainer1.SplitterDistance = value;
                else
                    splitContainer1.SplitterDistance = splitContainer1.Width - value;
            }
        }

        SidebarSide SidebarSide
        {
            get
            {
                if (tsSidebarPanel.Parent == splitContainer1.Panel2)
                    return SidebarSide.Right;
                else
                    return SidebarSide.Left;
            }
            set
            {
                if (value != SidebarSide)
                {
                    SuspendLayout();
                    if (value == SidebarSide.Left)
                    {
                        previewBar1.Parent = splitContainer1.Panel1;
                        attachmentView1.Parent = splitContainer1.Panel1;
                        tsSidebarPanel.Parent = splitContainer1.Panel1;
                        wfvcEditor.Parent = splitContainer1.Panel2;
                        btnSwitchSidebar.Image = Resource1.arrow_right;
                        tsSidebar.Items.Insert(tsSidebar.Items.Count - 1, tsSidebarSep);
                        tsSidebar.Items.Insert(tsSidebar.Items.Count - 1, btnSwitchSidebar);
                        splitContainer1.FixedPanel = FixedPanel.Panel1;
                    }
                    else
                    {
                        previewBar1.Parent = splitContainer1.Panel2;
                        attachmentView1.Parent = splitContainer1.Panel2;
                        tsSidebarPanel.Parent = splitContainer1.Panel2;
                        wfvcEditor.Parent = splitContainer1.Panel1;
                        btnSwitchSidebar.Image = Resource1.arrow_left;
                        tsSidebar.Items.Insert(0, tsSidebarSep);
                        tsSidebar.Items.Insert(0, btnSwitchSidebar);
                        splitContainer1.FixedPanel = FixedPanel.Panel2;
                    }
                    if (btnPages.Checked)
                        btnPages.PerformClick();
                    else
                        btnAttachments.PerformClick();
                    int tmp = splitContainer1.Panel1MinSize;
                    splitContainer1.Panel1MinSize = splitContainer1.Panel2MinSize;
                    splitContainer1.Panel2MinSize = tmp;
                    splitContainer1.SplitterDistance = splitContainer1.Width - splitContainer1.SplitterDistance;
                    ResumeLayout();
                }
            }
        }

        private void btnSwitchSidebar_Click(object sender, EventArgs e)
        {
            if (SidebarSide == SidebarSide.Right)
                SidebarSide = SidebarSide.Left;
            else
                SidebarSide = SidebarSide.Right;
        }

        private void btnPages_Click(object sender, EventArgs e)
        {
            previewBar1.BringToFront();
            btnPages.Checked = true;
            btnAttachments.Checked = false;
            previewBar1.Focus();
        }

        private void btnAttachments_Click(object sender, EventArgs e)
        {
            attachmentView1.BringToFront();
            btnPages.Checked = false;
            btnAttachments.Checked = true;
        }

        // Page Management toolbar

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            previewBar1.Previous();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            previewBar1.Next();
        }

        private void importNotebookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckDirty())
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Smart Notebook|*.notebook";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // setup progress form
                    ProgressForm pf = new ProgressForm();
                    pf.Text = "Importing Notebook File";
                    pf.Shown += delegate(object s, EventArgs e2)
                    {
                        Application.DoEvents();
                        // start undo/redo
                        dem.UndoRedoStart("Import Notebook");
                        try
                        {
                            // clear pages & attachments
                            dem.Clear();
                            attachmentView1.ClearAttachments();
                            // load notebook file
                            Dictionary<string, byte[]> attachments;
                            List<DEngine> engines = Converters.Converters.FromNotebook(ofd.FileName, out attachments);
                            // add new pages
                            foreach (DEngine de in engines)
                            {
                                InitDEngine(de, false);
                                dem.AddEngine(de);
                                Application.DoEvents(); // update progress form
                            }
                            // add new attachments
                            foreach (string name in attachments.Keys)
                                attachmentView1.AddAttachment(name, attachments[name]);
                            // clear undos
                            dem.UndoRedoCommit();
                            dem.UndoRedoClearHistory();
                            // update vars
                            fileName = Path.GetFileNameWithoutExtension(ofd.FileName);
                            beenSaved = false;
                            UpdateTitleBar();
                            // show first page
                            ShowFirstPage();
                        }
                        catch (Exception e3)
                        {
                            dem.UndoRedoCancel();
                            MessageBox.Show(e3.Message, "Error Reading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        pf.Close();
                    };
                    pf.ShowDialog();
                }
            }
        }

        void ShowFirstPage()
        {
            if (dem.EngineCount > 0)
                previewBar1.SetPreviewSelected(dem.GetEngine(0));
        }

        private void btnEditPersonalToolbar_Click(object sender, EventArgs e)
        {
            PtForm pf = new PtForm();
            pf.PersonalToolstrip = tsPersonal;
            if (pf.ShowDialog() == DialogResult.OK)
            {
                for (int i = tsPersonal.Items.Count - 1; i > 0; i--)
                    tsPersonal.Items.RemoveAt(i);
                foreach (object o in pf.ToolItems)
                {
                    if (o is CustomFigureT)
                        tsPersonal.Items.Add(new CustomFigureToolButton(((CustomFigureT)o).FigureClass,
                            ((CustomFigureT)o).Dap));
                    else if (o is RunCmdT)
                        tsPersonal.Items.Add(new RunCmdToolButton(((RunCmdT)o).Command, ((RunCmdT)o).Arguments));
                    else if (o is ShowDirT)
                        tsPersonal.Items.Add(new ShowDirToolButton(((ShowDirT)o).Dir));
                }
            }
        }

        private void tsPersonal_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is CustomFigureToolButton)
            {
                CustomFigureToolButton b = (CustomFigureToolButton)e.ClickedItem;
                de.HsmSetStateByFigureClass(b.FigureClass);
                dap.SetProperties(b.Dap);
            }
        }
    }
}