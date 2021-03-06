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
using DejaVu;
using DejaVu.Collections.Generic;
using Workbook.PersonalToolbar;

namespace Workbook
{
    public partial class MainForm : WorkBookForm
    {
        UndoRedoArea undoRedoArea = new UndoRedoArea("Workbook Area");
        UndoRedoList<DEngine> engines = new UndoRedoList<DEngine>();

        public bool Dirty
        {
            set
            {
                if (value == false)
                {
                    undoRedoArea.ClearHistory();
                    undoRedoArea_UndoRedoChanged(this, new CommandDoneEventArgs(CommandDoneType.Commit));
                }
            }
            get { return undoRedoArea.CanUndo; }
        }

        UndoRedo<DPoint> _newPageSize = new UndoRedo<DPoint>(PageTools.FormatToSize(PageFormat.Default));
        public DPoint NewPageSize
        {
            get { return _newPageSize.Value; }
            set
            {
                if (!value.Equals(_newPageSize.Value))
                    _newPageSize.Value = value;
            }
        }

        UndoRedo<BackgroundFigure> _backgroundFigure = new UndoRedo<BackgroundFigure>();
        public BackgroundFigure BackgroundFigure
        {
            get { return _backgroundFigure.Value; }
            set
            {
                if (_backgroundFigure.Value != value)
                    _backgroundFigure.Value = value;
            }
        }

        DEngine de = null;
        DTkViewer dvEditor;

        BitmapGlyph linkGlyph;
        DPoint textInsertionPoint;
        bool nonTextInsertionKey;

        bool previewBar1Focused;
        bool highlightSelection;

        bool beenSaved;
        bool needSave;
        string fileName;

        WorkBookArguments cmdArguments = WorkBookArguments.GlobalWbArgs;
        Ipc ipc = Ipc.GlobalIpc;
#if PREVIEW_BUILD
        string ProgramName = "Workbook (Preview Build:" + WorkBookUtils.RetrieveLinkerTimestamp().ToShortDateString() + ")";
#else
        const string ProgramName = "Workbook";
#endif
        const string FileExt = ".wbook";
        const string OpenFileTypeFilter = "Workbook files|*.ddraw;*.wbook";
        const string SaveFileTypeFilter = "Workbook files|*.wbook";
        string TempDir = Path.Combine(Path.GetTempPath(), "2Touch Workbook");
        string tempFilesDir;
        Timer autoSaveTimer;
        string autoSaveDir;
        string autoSaveFileName;
        string autoSaveServerFileName;
        const string autoSaveServerExt = ".servername";
        bool autoSaveOnSelectState = false;

        int gridSize = 20;
        bool showGrid = false;
        bool gridSnapPosition = false;
        bool gridSnapResize = false;
        bool gridSnapLines = false;

        public PersonalToolStrip PersonalToolStrip
        {
            get { return tsPersonal; }
        }

        void CreateDEngine(DEngine sibling)
        {
            DEngine de = new DEngine(undoRedoArea);
            if (sibling != null)
            {
                int idx = engines.IndexOf(sibling);
                if (idx >= 0)
                    engines.Insert(idx + 1, de);
                else
                    engines.Add(de);
            }
            else
                engines.Add(de);
            // page size and name
            de.PageSize = NewPageSize;
            de.PageName = engines.Count.ToString();
            // background figure
            if (BackgroundFigure != null && !de.CustomBackgroundFigure)
                de.SetBackgroundFigure(BackgroundFigure, false);
            // 
            InitDEngine(de, true);
        }

        void InitDEngine(DEngine de, bool showIt)
        {
            // DEngine settings
            WorkBookUtils.SetupDEngine(de);
            // DEngine events
#if DEBUG
            de.DebugMessage += new DebugMessageHandler(DebugMessage);
#endif
            de.HsmStateChanged += new HsmStateChangedHandler(de_HsmStateChanged);
            de.SelectedFiguresChanged += new SelectedFiguresHandler(de_SelectedFiguresChanged);
            de.FigureClick += new ClickHandler(de_FigureClick);
            de.FigureContextClick += new ClickHandler(de_ContextClick);
            de.FigureLockClick += new ClickHandler(de_FigureLockClick);
            de.ContextClick += new ClickHandler(de_ContextClick);
            de.DragFigureStart += new DragFigureHandler(de_DragFigureStart);
            de.DragFigureEvt += new DragFigureHandler(de_DragFigureEvt);
            de.DragFigureEnd += new DragFigureHandler(de_DragFigureEnd);
            de.MouseDown += new DMouseButtonEventHandler(de_MouseDown);
            de.AddedFigure += new AddedFigureHandler(de_AddedFigure);
            de.TextCopy += new HsmTextHandler(de_TextCopy);
            de.TextCut += new HsmTextHandler(de_TextCut);
            // add default properties to figures
            foreach (Figure f in de.Figures)
                AddDefaultProperties(f);
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
            // personal toolbar
            tsPersonal.De = de;
            // set grid options
            SetGridOptionsForCurrentDe();
        }

        private void SetGridOptionsForCurrentDe()
        {
            de.Grid = gridSize;
            de.GridSnapPosition = gridSnapPosition;
            de.GridSnapResize = gridSnapResize;
            de.GridSnapLines = gridSnapLines;
            if (showGrid)
                dvEditor.Grid = gridSize;
            else
                dvEditor.Grid = 0;
            dvEditor.Update();
        }

        public MainForm()
        {
            // Init localization
            WbLocale.Init(System.Globalization.CultureInfo.CurrentUICulture);
            // Init Component
            InitializeComponent();
            LocalizeUI();
            // create temp paths
            tempFilesDir = WorkBookUtils.GetTempFileName("tempfiles", TempDir);
            autoSaveDir = Path.Combine(TempDir, "autosave");
            try
            {
                if (!Directory.Exists(TempDir))
                    Directory.CreateDirectory(TempDir);
                if (!Directory.Exists(tempFilesDir))
                    Directory.CreateDirectory(tempFilesDir);
                if (!Directory.Exists(autoSaveDir))
                    Directory.CreateDirectory(autoSaveDir);
            }
            catch { }
            attachmentView1.TempDir = tempFilesDir;
            // set icon
            Icon = Resource1._2touch;
            // Create Handle (so we can respond to ipc events from other threads without showing the form)
            IntPtr h = Handle;
            // Initialze DGraphics
            WFHelper.InitGraphics();
            // undo redo
            undoRedoArea.CommandDone += new EventHandler<CommandDoneEventArgs>(undoRedoArea_UndoRedoChanged);
            // edit viewer
            dvEditor = new WFViewer(wfvcEditor);
            dvEditor.EditFigures = true;
#if DEBUG
            dvEditor.DebugMessage += new DebugMessageHandler(DebugMessage);
#else
            statusStrip1.Parent = null;
#endif
            // highlight selection
#if !BEHAVIOURS
            highlightSelectionToolStripMenuItem.Visible = false;
#endif
            // glyphs
            linkGlyph = new BitmapGlyph(WFHelper.MakeBitmap(Resource1.link), DGlyphPosition.BottomLeft);
            linkGlyph.Visiblility = DGlyphVisiblity.Always;
            linkGlyph.Cursor = DCursor.Hand;
            linkGlyph.Clicked += new GlyphClickedHandler(linkGlyph_Clicked);
            // figure defaults
            Figure._handleSize = 6;
            Figure._handleBorder = 4;
            Figure._rotateHandleStemLength = Figure._handleSize - Figure._handleBorder;
            LinebaseFigure._hitTestExtension = 5;
            // setup autosave
            autoSaveTimer = new Timer();
            autoSaveTimer.Tick += new EventHandler(autoSaveTimer_Tick);
            // set toolstrip properties
            tsPropState.Dv = dvEditor;
            // connect to ipc messages
            ipc.MessageReceived += new MessageReceivedHandler(ipc_MessageReceived);
            ipc.P2pMessageRecieved += new P2pMessageReceivedHandler(ipc_P2pMessageRecieved);
            // get command line arguments
            ActionCommandLine();
        }

        private void LocalizeUI()
        {
            // Menu Items
            fileToolStripMenuItem.Text = WbLocale.File;
            editToolStripMenuItem.Text = WbLocale.Edit;
            viewToolStripMenuItem.Text = WbLocale.View;
            insertToolStripMenuItem.Text = WbLocale.Insert;
            formatToolStripMenuItem.Text = WbLocale.Format;
            toolsToolStripMenuItem.Text = WbLocale.Tools;
            recentDocumentsToolStripMenuItem.Text = WbLocale.RecentDocuments;
            importNotebookToolStripMenuItem.Text = WbLocale.ImportNotebook;
            exportToolStripMenuItem.Text = WbLocale.Export;
            sendToToolStripMenuItem.Text = WbLocale.SendTo;
            mailRecipientToolStripMenuItem.Text = WbLocale.MailRecipient;
            mailRecipientasPDFToolStripMenuItem.Text = WbLocale.MailRecipientPDF;
            exitToolStripMenuItem.Text = WbLocale.Exit;
            zoomToolStripMenuItem.Text = WbLocale.Zoom;
            fitToPageToolStripMenuItem.Text = WbLocale.FitToPage;
            fitToWidthToolStripMenuItem.Text = WbLocale.FitToWidth;
            _050PcToolStripMenuItem.Text = WbLocale._050Percent;
            _100PcToolStripMenuItem.Text = WbLocale._100Percent;
            _150PcToolStripMenuItem.Text = WbLocale._150Percent;
            antialiasToolStripMenuItem.Text = WbLocale.Antialias;
            toolbarsToolStripMenuItem.Text = WbLocale.Toolbars;
            editToolStripMenuItem1.Text = WbLocale.Edit;
            personalToolStripMenuItem.Text = WbLocale.Personal;
            modeSelectToolStripMenuItem.Text = WbLocale.ModeSelect;
            propertySelectToolStripMenuItem.Text = WbLocale.PropertySelect;
            pageNavigationToolStripMenuItem.Text = WbLocale.PageNavigation;
            toolsToolStripMenuItem1.Text = WbLocale.Tools;
            imageToolStripMenuItem.Text = WbLocale.Image;
            attachmentToolStripMenuItem.Text = WbLocale.Attachment;
            orderToolStripMenuItem.Text = WbLocale.Order;
            resetToolbarsToolStripMenuItem.Text = WbLocale.ResetToolbars;
            aboutToolStripMenuItem.Text = WbLocale.About;
            flipXToolStripMenuItem.Text = WbLocale.FlipLeftRight;
            flipYToolStripMenuItem.Text = WbLocale.FlipUpDown;
            orderStripMenuItem.Text = WbLocale.Order;
            highlightSelectionToolStripMenuItem.Text = WbLocale.HighlightSelection;
            gridToolStripMenuItem.Text = WbLocale.Grid;
            // Actions
            actAnnoTools.Text = WbLocale.ScreenAnnotate;
            actBackground.Text = WbLocale.Background;
            actBringForward.Text = WbLocale.BringForward;
            actBringToFront.Text = WbLocale.BringToFront;
            actClearPage.Text = WbLocale.ClearPage;
            actClonePage.Text = WbLocale.ClonePage;
            actCopy.Text = WbLocale.Copy;
            actCut.Text = WbLocale.Cut;
            actDelete.Text = WbLocale.Delete;
            actDeletePage.Text = WbLocale.DeletePage;
            actDimensions.Text = WbLocale.Dimensions;
            actExportSelectionToImage.Text = WbLocale.ExportSelectionToImage;
            actExportSelectionToPng.Text = WbLocale.PNG;
            actExportSelectionToEmf.Text = WbLocale.EMF;
            actGroupFigures.Text = WbLocale.Group;
            actLink.Text = WbLocale.Link;
            actLockFigure.Text = WbLocale.Lock;
            actNew.Text = WbLocale.New;
            actNewPage.Text = WbLocale.NewPage;
            actOpen.Text = WbLocale.Open;
            actPageSize.Text = WbLocale.PageSize;
            actPaste.Text = WbLocale.Paste;
            actPrint.Text = WbLocale.Print;
            actProperties.Text = WbLocale.Properties;
            actRedo.Text = WbLocale.Redo;
            actRenamePage.Text = WbLocale.RenamePage;
            actSave.Text = WbLocale.Save;
            actSaveAs.Text = WbLocale.SaveAs;
            actScreenCapture.Text = WbLocale.ScreenCapture;
            actSelectAll.Text = WbLocale.SelectAll;
            actSendBackward.Text = WbLocale.SendBackward;
            actSendToBack.Text = WbLocale.SendToBack;
            actUndo.Text = WbLocale.Undo;
            actUnwrapText.Text = WbLocale.RemoveTextWrap;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // read program options
            ReadOptions();
            // read toolstrip settings
            ToolStripManager.SaveSettings(this, "reset");
            ToolStripManager.LoadSettings(this);
            // ensure menustrip is visible
            menuStrip1.Visible = true;
            // make sure previews line up properly
            previewBar1.ResetPreviewPositions();
            // autosave stuff
            CheckPreviousAutosaves();
            CreateAutosave();
            // set grid
            SetGridOptionsForCurrentDe();
        }

        void TryDelete(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch { }
        }

        void NewIfNoFileOpen()
        {
            if (de == null)
                New();
        }

        void CheckPreviousAutosaves()
        {
            // check for old autosave files
            string[] autosaves = Directory.GetFiles(autoSaveDir, "*" + FileExt);
            string orphanedAutosave = null;
            string autosaveServerInfoFile = null;
            foreach (string autosave in autosaves)
            {
                autosaveServerInfoFile = Path.ChangeExtension(autosave, autoSaveServerExt);
                if (File.Exists(autosaveServerInfoFile))
                {
                    using (StreamReader sr = new StreamReader(autosaveServerInfoFile))
                    {
                        if (ipc.SendP2pMessage(sr.ReadToEnd(), IpcP2pMessage.QueryAutoSaveFile, autosave) !=
                            IpcP2pMessage.ConfirmAutoSaveFile)
                        {
                            orphanedAutosave = autosave;
                            break;
                        }
                    }
                }
                else
                {
                    orphanedAutosave = autosave;
                    break;
                }
            }
            //choose whether to load autosave file or initialize new document
            if (orphanedAutosave != null)
            {
                FileInfo fi = new FileInfo(orphanedAutosave);
                if (fi.Length == 0)
                {
                    TryDelete(autosaveServerInfoFile);
                    TryDelete(orphanedAutosave);
                    NewIfNoFileOpen();
                }
                else if (MessageBox.Show("Autosave file found, would you like to restore this file?",
                    ProgramName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (OpenFile(orphanedAutosave))
                    {
                        fileName = "Restored Autosave";
                        beenSaved = false;
                        needSave = true;
                        TryDelete(autosaveServerInfoFile);
                        TryDelete(orphanedAutosave);
                    }
                    else
                        NewIfNoFileOpen();
                }
                else
                    NewIfNoFileOpen();
            }
            else
                NewIfNoFileOpen();
            Dirty = false;
        }

        void CreateAutosave()
        {
            // create autosave file and ipc server info file
            autoSaveFileName = WorkBookUtils.GetTempFileName("autosave" + FileExt, autoSaveDir);
            autoSaveServerFileName = Path.ChangeExtension(autoSaveFileName, autoSaveServerExt);
            try
            {
                File.Create(autoSaveFileName).Close();
                StreamWriter sw = new StreamWriter(autoSaveServerFileName);
                sw.Write(ipc.channelP2pServerName);
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        void Autosave(DHsmState state)
        {
            if (state == DHsmState.Select)
            {
                autoSaveOnSelectState = false;
                _Save(autoSaveFileName);
                autoSaveTimer.Start();
            }
            else
            {
                autoSaveOnSelectState = true;
                autoSaveTimer.Stop();
            }
        }

        void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            Autosave(de.HsmState);
        }

        void ReadOptions()
        {
            // start undo/redo transaction
            undoRedoArea.Start("set options");

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
            // toolbar options
            editToolStripMenuItem1.Checked = options.EditToolbar;
            personalToolStripMenuItem.Checked = options.PersonalToolbar;
            modeSelectToolStripMenuItem.Checked = options.EngineStateToolbar;
            propertySelectToolStripMenuItem.Checked = options.PropertyStateToolbar;
            pageNavigationToolStripMenuItem.Checked = options.PageNavigationToolbar;
            toolsToolStripMenuItem1.Checked = options.ToolsToolbar;
            UpdateToolbars();
            // misc
            highlightSelection = options.HighlightSelection;
            PageTools.SetDefaultSizeMM(options.DefaultPageSize);
            NewPageSize = PageTools.FormatToSize(PageFormat.Default);
            if (options.AutoSaveInterval >= 1)
                autoSaveTimer.Interval = options.AutoSaveInterval;
            else
                autoSaveTimer.Tick -= autoSaveTimer_Tick;
            WorkBookUtils.AutoRotateSnap = options.AutoRotateSnap;
            WorkBookUtils.AutoLockAspectRatio = options.AutoLockAspectRatio;
            // grid options
            gridSize = options.GridSize;
            showGrid = options.ShowGrid;
            gridSnapPosition = options.GridSnapPosition;
            gridSnapResize = options.GridSnapResize;
            gridSnapLines = options.GridSnapLines;
            // load recent docs
            LoadRecentDocuments(options);
            // load personal toolbar
            PtUtils.LoadPersonalTools(tsPersonal);
            // load figure toolbar
            FigureToolStrip.LoadFigureTools();

            // commit undo redo
            undoRedoArea.Commit();
            undoRedoArea.ClearHistory();
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
            // toolbar options
            options.EditToolbar = tsEdit.Visible;
            options.PersonalToolbar = tsPersonal.Visible;
            options.EngineStateToolbar = tsEngineState.Visible;
            options.PropertyStateToolbar = tsPropState.Visible;
            options.PageNavigationToolbar = tsPageManage.Visible;
            options.ToolsToolbar = tsTools.Visible;
            // misc
            options.HighlightSelection = highlightSelection;
            // grid options
            options.GridSize = gridSize;
            options.ShowGrid = showGrid;
            options.GridSnapPosition = gridSnapPosition;
            options.GridSnapResize = gridSnapResize;
            options.GridSnapLines = gridSnapLines;
            // write to file
            options.WriteIni();
            // save personal toolbar
            PtUtils.SavePersonalTools(tsPersonal);
            // save figure toolbar
            FigureToolStrip.SaveFigureTools();
        }

        void ActionCommandLine()
        {
            if (cmdArguments.ScreenAnnotate)
                ShowAnnoTools(true);
            else
            {
                // if second unnamed cmd line argument is a file open it (first is this executable)
                if (cmdArguments.UnnamedParamCount > 1 && File.Exists(cmdArguments[1]))
                    OpenFile(cmdArguments[1]);
                // show form
                Show();
            }
        }

        void AddRecentDocument(string fileName)
        {
            new ProgramOptions().AddRecentDocument(fileName);
            LoadRecentDocuments(new ProgramOptions());
        }

        void LoadRecentDocuments(ProgramOptions options)
        {
            recentDocumentsToolStripMenuItem.DropDown.Items.Clear();
            List<string> recentDocs = options.GetRecentDocuments();
            if (recentDocs.Count > 0)
            {
                recentDocumentsToolStripMenuItem.Enabled = true;
                int n = 1;
                foreach (string doc in recentDocs)
                {
                    ToolStripItem t = recentDocumentsToolStripMenuItem.DropDown.Items.Add(
                        string.Format("{0}: {1}", n, Path.GetFileNameWithoutExtension(doc)));
                    n++;
                    t.Tag = doc;
                    t.Click += new EventHandler(recentDoc_Click);
                }
            }
            else
                recentDocumentsToolStripMenuItem.Enabled = false;
        }

        void recentDoc_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem && ((ToolStripItem)sender).Tag is string &&
                File.Exists((string)((ToolStripItem)sender).Tag) && CheckDirty())
                OpenFile((string)((ToolStripItem)sender).Tag);
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
                        Activate();
                        break;
                    case IpcMessage.ScreenAnnotate:
                        ShowAnnoTools(false);
                        break;
                }
        }

        IpcP2pMessage ipc_P2pMessageRecieved(IpcP2pMessage msg, string data)
        {
            if (msg == IpcP2pMessage.QueryAutoSaveFile)
            {
                if (data == autoSaveFileName)
                    return IpcP2pMessage.ConfirmAutoSaveFile;
                else
                    return IpcP2pMessage.RejectAutoSaveFile;
            }
            return IpcP2pMessage.Bork;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckDirty())
                e.Cancel = true;
            else if (Visible)
            {
                // write user options
                WriteOptions();
                // write toolstrip settings
                ToolStripManager.SaveSettings(this);
                // remove temp files
                try
                {
                    if (File.Exists(autoSaveFileName))
                        File.Delete(autoSaveFileName);
                    if (File.Exists(autoSaveServerFileName))
                        File.Delete(autoSaveServerFileName);
                    if (Directory.Exists(tempFilesDir))
                        Directory.Delete(tempFilesDir, true);
                }
                catch { }
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
            // Change to a safe state if we are in a volatile state like text editing or resizing etc
            if (de != null)
                de.CheckState();
        }

        void de_HsmStateChanged(DEngine de, DHsmState state)
        {
            if (state == DHsmState.TextEdit)
            {
                actCut.Enabled = true;
                actCopy.Enabled = true;
            }
            if (autoSaveOnSelectState && state == DHsmState.Select)
                Autosave(state);
        }

        void de_SelectedFiguresChanged()
        {
            InitActions();
        }

        void UpdateUndoRedoActions()
        {
            actUndo.Enabled = undoRedoArea.CanUndo;
            actUndo.Text = WbLocale.Undo;
            if (actUndo.Enabled)
            {
                IEnumerator<string> en = undoRedoArea.UndoCommands.GetEnumerator();
                if (en.MoveNext())
                    actUndo.Text = string.Format("{0} \"{1}\"", WbLocale.Undo, en.Current);
            };
            actRedo.Enabled = undoRedoArea.CanRedo;
            actRedo.Text = WbLocale.Redo;
            if (actRedo.Enabled)
            {
                IEnumerator<string> en = undoRedoArea.RedoCommands.GetEnumerator();
                if (en.MoveNext())
                    actRedo.Text = string.Format("{0} \"{1}\"", WbLocale.Redo, en.Current);
            }
                
        }

        void undoRedoArea_UndoRedoChanged(object sender, CommandDoneEventArgs e)
        {
            UpdateUndoRedoActions();
            UpdateTitleBar();
            if (e.CommandDoneType != CommandDoneType.Commit)
            {
                // in case the page size was undooed
                foreach (DEngine en in engines)
                    en.PageSize = en.PageSize;
                foreach (DEngine de in engines)
                    de.UpdateViewers();
                // check if UserAttrs have changed and update glyphs
                foreach (Figure f in de.Figures)
                    CheckLinkGlyph(f);
                // check if a page is renamed
                previewBar1.UpdatePreviewNames();
                // check if attachments have changed
                attachmentView1.UpdateAttachmentView();
            }
            // check if engines have changed order
            previewBar1.MatchPreviewsToEngines(engines, dvEditor);
        }

        void de_FigureClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (clickedFigure.UserAttrs.ContainsKey(Links.LinkBody))
                ExecLink(clickedFigure);
        }

        void de_FigureLockClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            undoRedoArea.Start(WbLocale.UnlockFigure);
            clickedFigure.Locked = false;
            undoRedoArea.Commit();
            dvEditor.Update();
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

        void ExecLink(Figure figure)
        {
            if (figure.UserAttrs.ContainsKey(Links.Link) && figure.UserAttrs.ContainsKey(Links.LinkType))
            {
                string link = figure.UserAttrs[Links.Link];
                LinkType linkType = Links.StringToLinkType(figure.UserAttrs[Links.LinkType]);
                switch (linkType)
                {
                    case LinkType.WebPage:
                        try
                        {
                            UriBuilder ub = new UriBuilder(link);
                            System.Diagnostics.Process.Start(ub.Uri.AbsoluteUri);
                        }
                        catch (Exception e)
                        { MessageBox.Show(e.Message, WbLocale.WebLinkError, MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        break;
                    case LinkType.File:
                        if (File.Exists(link))
                            try
                            {
                                System.Diagnostics.Process.Start(link);
                            }
                            catch (Exception e)
                            { MessageBox.Show(e.Message, WbLocale.FileLinkError, MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                            MessageBox.Show(string.Format("{0} \"{1}\"", WbLocale.CouldNotFindFile, link), WbLocale.FileLinkError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case LinkType.Page:
                        LinkPage lp = (LinkPage)Enum.Parse(typeof(LinkPage), link, true);
                        switch (lp)
                        {
                            case LinkPage.First:
                                previewBar1.SetPreviewSelected(engines[0]);
                                break;
                            case LinkPage.Last:
                                previewBar1.SetPreviewSelected(engines[engines.Count - 1]);
                                break;
                            case LinkPage.Next:
                                int idx = engines.IndexOf(de) + 1;
                                if (idx < engines.Count)
                                    previewBar1.SetPreviewSelected(engines[idx]);
                                else
                                    goto case LinkPage.First;
                                break;
                            case LinkPage.Previous:
                                int idx2 = engines.IndexOf(de) - 1;
                                if (idx2 > 0)
                                    previewBar1.SetPreviewSelected(engines[idx2]);
                                else
                                    goto case LinkPage.Last;
                                break;
                            default:
                                int n = 0;
                                int.TryParse(link, out n);
                                if (n >= 0 && n < engines.Count)
                                    previewBar1.SetPreviewSelected(engines[n]);
                                else
                                    MessageBox.Show(string.Format("{0} \"{1}\" {2}", WbLocale.Page, n + 1, WbLocale.DoesNotExist), WbLocale.PageLinkError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                        break;
                    case LinkType.Attachment:
                        try
                        {
                            attachmentView1.ExecuteAttachment(link);
                        }
                        catch (Exception e)
                        { MessageBox.Show(e.Message, WbLocale.AttachmentLinkError, MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
                actGroupFigures.Text = WbLocale.Ungroup;
            else if (de.CanGroupFigures(figs))
                actGroupFigures.Text = WbLocale.Group;
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
            // update link, lock & properties action
            actLink.Enabled = figs.Count == 1;
            actLockFigure.Enabled = figs.Count > 0;
            actDimensions.Enabled = figs.Count > 0;
            actProperties.Enabled = figs.Count > 0;
            // update export selection action
            actExportSelectionToImage.Enabled = figs.Count > 0;
            actExportSelectionToPng.Enabled = figs.Count > 0;
            actExportSelectionToEmf.Enabled = figs.Count > 0;
            // update unwrap text action
            actUnwrapText.Enabled = figs.Count == 1 && figs[0] is ITextable && ((ITextable)figs[0]).WrapText;
            removeTextWrapToolStripMenuItem.Visible = actUnwrapText.Enabled;
        }

        void AddDefaultProperties(Figure fig)
        {
            // show context handle
            fig.ContextHandle = true;
            // make sure fig.Glyphs is assigned
            if (fig.Glyphs == null)
                fig.Glyphs = new List<IGlyph>();
            // add link glyph if needed
            CheckLinkGlyph(fig);
            // add behavoiurs
            AddBehaviours(fig);
            // recurse into child figures
            if (fig is GroupFigure)
                foreach (Figure child in ((GroupFigure)fig).ChildFigures)
                    AddDefaultProperties(child);
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

        private void AddBehaviours(Figure fig)
        {
#if BEHAVIOURS
            if (fig is IBehaviours)
            {
                DBehaviour b = new DBehaviour();
                if (highlightSelection)
                {
                    b.SetFill = true;
                    b.SetStroke = true;
                    b.SetAlpha = true;
                    if (fig is IFillable)
                    {
                        b.Fill = DColor.Blue50Pc;
                        b.Stroke = DColor.Blue;
                    }
                    else
                        b.Stroke = DColor.Blue50Pc;
                    b.Alpha = 1;
                }
                ((IBehaviours)fig).MouseOverBehaviour = b;
            }
#endif
        }

        void de_AddedFigure(DEngine de, Figure fig, bool fromHsm)
        {
            if (fromHsm)
                tsEngineState.Dap.ApplyPropertiesToFigure(fig);
            AddDefaultProperties(fig);
        }

        void de_TextCut(DEngine de, string text)
        {
            if (text != null)
                Clipboard.SetText(text);
        }

        void de_TextCopy(DEngine de, string text)
        {
            if (text != null)
                Clipboard.SetText(text);
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
            CheckState();
            SetCurrentDe(p.DEngine);
        }

        private void previewBar1_PreviewContext(Preview p, Point pt)
        {
            cmsPreview.Show(p, pt);
        }

        private void previewBar1_PreviewMove(Preview p, Preview to)
        {
            CheckState();
            undoRedoArea.Start(WbLocale.MovePage);
            int idx = engines.IndexOf(to.DEngine);
            engines.Remove(p.DEngine);
            engines.Insert(idx, p.DEngine);
            undoRedoArea.Commit();
        }

        private void previewBar1_PreviewFigureDrop(Preview p, List<Figure> figs)
        {
            if (!p.Selected)
            {
                CheckState();
                undoRedoArea.Start(WbLocale.DragFigureToNewPage);
                foreach (Figure f in figs)
                {
                    WorkBookUtils.PutInBounds(p.DEngine, f);
                    de.RemoveFigure(f);
                    p.DEngine.AddFigure(f);
                }
                de.ClearSelected();
                undoRedoArea.Commit();
                de.UpdateViewers();
                p.DEngine.UpdateViewers();
            }
        }

        private void previewBar1_PreviewNameChanged(Preview p, string name)
        {
            CheckState();
            undoRedoArea.Start(WbLocale.ChangePageName);
            de.PageName = name;
            undoRedoArea.Commit();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.PNG;*.WMF";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CheckState();
                de.ClearSelected();
                undoRedoArea.Start(WbLocale.AddImage);
                const int left = 10, top = 10;
                byte[] imageData = WorkBookUtils.GetBytesFromFile(ofd.FileName);
                if (IsWmfFilePath(ofd.FileName))
                    de.AddFigure(new MetafileFigure(new DPoint(left, top), 0, DDraw.DMetafileType.Wmf, imageData, ofd.FileName));
                else
                {
                    DBitmap bmp = WFHelper.MakeBitmap(ofd.FileName);
                    de.AddFigure(new BitmapFigure(new DRect(left, top, bmp.Width, bmp.Height), 0, imageData, ofd.FileName));
                }
                undoRedoArea.Commit();
                de.UpdateViewers();
            }
        }

        private void attachmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAttachment();
        }

        private void InsertAttachment()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                undoRedoArea.Start(WbLocale.AddAttachment);
                if (attachmentView1.CheckAttachmentExists(ofd.FileName))
                    attachmentView1.AddAttachment(ofd.FileName);
                undoRedoArea.Commit();
            }
        }

        private void resetToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripManager.LoadSettings(this, "reset");
            UpdateToolbars();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
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
            // highlight selection
            highlightSelectionToolStripMenuItem.Checked = highlightSelection;
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

        private void highlightSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightSelection = !highlightSelection;
            foreach (DEngine de in engines)
                SetFigureBehaviours(de.Figures);
        }

        private void SetFigureBehaviours(IList<Figure> list)
        {
            foreach (Figure f in list)
            {
                AddBehaviours(f);
                if (f is GroupFigure)
                    SetFigureBehaviours(((GroupFigure)f).ChildFigures);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void wfvcEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // stop the Del shortcut from absorbing the key input when in textedit state
            if (de.HsmState == DHsmState.TextEdit)
            {
                if (e.KeyCode == Keys.Delete)
                    e.IsInputKey = true;
                else if (e.Shift)
                    e.IsInputKey = true;
            }
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
                de.UndoRedo.Start(WbLocale.TextEdit);
                TextFigure tf = new TextFigure(textInsertionPoint, "", 0);
                tsEngineState.DapText.ApplyPropertiesToFigure(tf);
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
            {
                de.UndoRedo.Start(WbLocale.Ungroup);
                de.UngroupFigures(figs);
                de.UndoRedo.Commit();
            }
            else if (de.CanGroupFigures(figs))
            {
                de.UndoRedo.Start(WbLocale.Group);
                de.GroupFigures(figs);
                de.UndoRedo.Commit();
            }
            InitActions();
        }

        private void actSendToBack_Execute(object sender, EventArgs e)
        {
            de.UndoRedo.Start(WbLocale.SendToBack);
            de.SendToBack(de.SelectedFigures);
            de.UndoRedo.Commit();
            InitActions();
        }

        private void actBringToFront_Execute(object sender, EventArgs e)
        {
            de.UndoRedo.Start(WbLocale.BringToFront);
            de.BringToFront(de.SelectedFigures);
            de.UndoRedo.Commit();
            InitActions();
        }

        private void actSendBackward_Execute(object sender, EventArgs e)
        {
            de.UndoRedo.Start(WbLocale.SendBackward);
            de.SendBackward(de.SelectedFigures);
            de.UndoRedo.Commit();
            InitActions();
        }

        private void actBringForward_Execute(object sender, EventArgs e)
        {
            de.UndoRedo.Start(WbLocale.BringForward);
            de.BringForward(de.SelectedFigures);
            de.UndoRedo.Commit();
            InitActions();
        }

        private void actPageSize_Execute(object sender, EventArgs e)
        {
            CustomPageSizeForm f = new CustomPageSizeForm();
            f.PageSize = de.PageSize;
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.ApplyAll)
                {
                    undoRedoArea.Start(WbLocale.SetGlobalPageSize);
                    NewPageSize = f.PageSize;
                    foreach (DEngine en in engines)
                        SetEnginePageSize(en, f.PageSize);
                    undoRedoArea.Commit();
                }
                else
                {
                    undoRedoArea.Start(WbLocale.ChangePageSize);
                    SetEnginePageSize(de, f.PageSize);
                    undoRedoArea.Commit();
                }
            }
        }

        private void SetEnginePageSize(DEngine de, DPoint pageSize)
        {
            de.PageSize = pageSize;
            foreach (Figure fig in de.Figures)
                WorkBookUtils.PutInBounds(de, fig);
        }

        private void actBackground_Execute(object sender, EventArgs e)
        {
            BackgroundForm bf = new BackgroundForm();
            bf.BackgroundFigure = de.BackgroundFigure;
            if (bf.ShowDialog() == DialogResult.OK)
            {
                if (bf.ApplyAll)
                {
                    undoRedoArea.Start(WbLocale.SetGlobalBackground);
                    BackgroundFigure = bf.BackgroundFigure;
                    foreach (DEngine en in engines)
                        en.SetBackgroundFigure(BackgroundFigure, false);
                    undoRedoArea.Commit();
                }
                else
                {
                    undoRedoArea.Start(WbLocale.SetBackground);
                    de.SetBackgroundFigure(bf.BackgroundFigure, true);
                    undoRedoArea.Commit();
                }
            }
        }

        private void actCut_Execute(object sender, EventArgs e)
        {
            if (de.HsmState == DHsmState.TextEdit)
                de.CutText();
            else
            {
                CheckState();
                List<Figure> figs = de.SelectedFigures;
                DBitmap bmp;
                de.UndoRedo.Start(WbLocale.Cut);
                string data = de.Cut(figs, out bmp, dvEditor.AntiAlias);
                de.UndoRedo.Commit();
                CopyToClipboard(data, WFHelper.FromImageData(WFHelper.ToImageData(bmp)), figs);
            }
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
            if (de.HsmState == DHsmState.TextEdit)
                de.CopyText();
            else
            {
                CheckState();
                List<Figure> figs = de.SelectedFigures;
                DBitmap bmp;
                string data = de.Copy(figs, out bmp, dvEditor.AntiAlias, DColor.White);
                CopyToClipboard(data, WFHelper.FromImageData(WFHelper.ToImageData(bmp)), figs);
            }
        }

        void PasteDataObject(IDataObject iData, string opPrefix, double objX, double objY)
        {
            if (iData.GetDataPresent(FigureSerialize.DDRAW_FIGURE_XML))
            {
                undoRedoArea.Start(string.Format("{0} {1}", opPrefix, WbLocale.Figures));
                List<Figure> figs = FigureSerialize.FromXml((string)iData.GetData(FigureSerialize.DDRAW_FIGURE_XML));
                foreach (Figure f in figs)
                    WorkBookUtils.PutInBounds(de, f);
                de.PasteAsSelectedFigures(figs);
                undoRedoArea.Commit();
            }
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                undoRedoArea.Start(string.Format("{0} {1}", opPrefix, WbLocale.Text));
                TextFigure f = new TextFigure(new DPoint(objX, objY), (string)iData.GetData(DataFormats.Text), 0);
                tsEngineState.DapText.ApplyPropertiesToFigure(f);
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                undoRedoArea.Commit();
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                undoRedoArea.Start(string.Format("{0} {1}", opPrefix, WbLocale.Bitmap));
                Bitmap bmp = (Bitmap)iData.GetData(DataFormats.Bitmap, true);
                byte[] imageData = WFHelper.ToImageData(bmp);
                BitmapFigure f = new BitmapFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0, imageData, "Clipboard.bmp");
                de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                undoRedoArea.Commit();
            }
            else if (iData.GetDataPresent(DataFormats.FileDrop))
            {
                undoRedoArea.Start(string.Format("{0} {1}", opPrefix, WbLocale.File));
                string path = ((string[])iData.GetData(DataFormats.FileDrop))[0];
                if (IsImageFilePath(path))
                {
                    Figure f;
                    if (IsWmfFilePath(path))
                        f = new MetafileFigure(new DPoint(objX, objY), 0,
                            DDraw.DMetafileType.Wmf, WorkBookUtils.GetBytesFromFile(path), path);
                    else
                    {
                        Bitmap bmp = (Bitmap)Bitmap.FromFile(path);
                        f = new BitmapFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0,
                            WFHelper.ToImageData(bmp), path);
                    }
                    de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                }
                else if (attachmentView1.CheckAttachmentExists(path))
                {
                    TextFigure f = new TextFigure(new DPoint(objX, objY), Path.GetFileName(path), 0);
                    f.UserAttrs[Links.Link] = attachmentView1.AddAttachment(path);
                    f.UserAttrs[Links.LinkType] = LinkType.Attachment.ToString();
                    tsEngineState.DapText.ApplyPropertiesToFigure(f);
                    de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                }
                undoRedoArea.Commit();
            }
            else if (iData.GetDataPresent(attachmentView1.GetType()))
            {
                undoRedoArea.Start(string.Format("{0} {1}", opPrefix, WbLocale.Attachment));
                foreach (ListViewItem item in attachmentView1.SelectedItems)
                {
                    if (IsImageFilePath(item.Text))
                    {
                        using (MemoryStream ms = new MemoryStream(attachmentView1.GetAttachment(item.Text)))
                        {
                            Figure f ;
                            if (IsWmfFilePath(item.Text))
                                f = new MetafileFigure(new DPoint(objX, objY), 0, 
                                    DDraw.DMetafileType.Wmf, ms.ToArray(), item.Text);
                            else
                            {
                            Bitmap bmp = (Bitmap)Bitmap.FromStream(ms);
                            f = new BitmapFigure(new DRect(objX, objY, bmp.Width, bmp.Height), 0,
                                WFHelper.ToImageData(bmp), item.Text);
                            }
                            de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                        }
                    }
                    else
                    {
                        TextFigure f = new TextFigure(new DPoint(objX, objY), item.Text, 0);
                        f.UserAttrs[Links.Link] = item.Text;
                        f.UserAttrs[Links.LinkType] = LinkType.Attachment.ToString();
                        tsEngineState.DapText.ApplyPropertiesToFigure(f);
                        de.PasteAsSelectedFigures(new List<Figure>(new Figure[] { f }));
                    }
                }
                undoRedoArea.Commit();
            }
        }

        private void actPaste_Execute(object sender, EventArgs e)
        {
            if (de.HsmState == DHsmState.TextEdit)
            {
                if (Clipboard.GetText() != null)
                    de.PasteText(Clipboard.GetText());
            }
            else
            {
                CheckState();
                PasteDataObject(Clipboard.GetDataObject(), WbLocale.Paste, 10, 10);
            }
        }

        bool AttachmentLinked(string name)
        {
            foreach (DEngine de in engines)
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
            {
                CheckState();
                de.UndoRedo.Start(WbLocale.DeleteFigures);
                de.Delete(de.SelectedFigures);
                de.UndoRedo.Commit();
            }
            else if (previewBar1Focused)
            {
                CheckState();
                undoRedoArea.Start(WbLocale.DeletePage);
                engines.Remove(de);
                if (engines.Count == 0)
                    CreateDEngine(de);
                undoRedoArea.Commit();
            }
            else if (attachmentView1.Focused)
                DeleteAttachment();
        }

        private void DeleteAttachment()
        {
            foreach (ListViewItem item in attachmentView1.SelectedItems)
                if (AttachmentLinked(item.Text))
                {
                    if (MessageBox.Show(WbLocale.AttachmentIsLinkedMsg, WbLocale.AttachmentIsLinked, 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        break;
                    else
                        return;
                }
            CheckState();
            undoRedoArea.Start(WbLocale.DeleteAttachments);
            foreach (ListViewItem item in attachmentView1.SelectedItems)
                attachmentView1.RemoveAttachment(item);
            undoRedoArea.Commit();
        }

        private void actDelete_Execute(object sender, EventArgs e)
        {
            DoDelete();
        }

        void UpdateTitleBar()
        {
            if (Dirty)
                Text = string.Format("{0} - {1}{2}", ProgramName, Path.GetFileNameWithoutExtension(fileName), "*");
            else
                Text = string.Format("{0} - {1}", ProgramName, Path.GetFileNameWithoutExtension(fileName));
        }

        void StartAutoSaveTimer()
        {
            autoSaveTimer.Stop();
            autoSaveTimer.Start();
        }

        void New()
        {
            fileName = WbLocale.NewDocument;
            beenSaved = false;
            needSave = false;

            undoRedoArea.Start(WbLocale.NewDocument);
            ClearDocument();
            CreateDEngine(null);
            attachmentView1.ClearAttachments();
            undoRedoArea.Commit();
            Dirty = false;

            UpdateTitleBar();
            // start autosaving
            StartAutoSaveTimer();
        }

        void ClearDocument()
        {
            engines.Clear();
            BackgroundFigure = new BackgroundFigure();
        }

        bool OpenFile(string fileName)
        {
            bool result = false;
            CheckState();
            // create progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = WbLocale.OpeningFile;
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // start undo/redo
                undoRedoArea.Start(WbLocale.OpenDocument);
                try
                {
                    // load new engines
                    ClearDocument();
                    DPoint pageSize;
                    BackgroundFigure bf;
                    Dictionary<string, byte[]> extraEntries;
                    List<DEngine> engines = FileHelper.Load(fileName, undoRedoArea, out pageSize, out bf,
                        new string[] { AttachmentView.ATTACHMENTS_DIR }, out extraEntries);
                    if (pageSize != null)
                        NewPageSize = pageSize;
                    else
                        NewPageSize = PageTools.FormatToSize(PageFormat.Default);
                    if (bf != null)
                        BackgroundFigure = bf;
                    else
                        BackgroundFigure = new BackgroundFigure();
                    // init new dengines
                    foreach (DEngine newDe in engines)
                    {
                        InitDEngine(newDe, false);
                        this.engines.Add(newDe);
                        Application.DoEvents(); // update progress form
                    }
                    // set attachments
                    attachmentView1.ClearAttachments();
                    if (extraEntries != null)
                        foreach (string name in extraEntries.Keys)
                            if (name.IndexOf(AttachmentView.ATTACHMENTS_DIR) == 0)
                                attachmentView1.AddAttachment(name, extraEntries[name]);
                    // commit undo/redo
                    undoRedoArea.Commit();
                    Dirty = false;
                    // update vars
                    this.fileName = fileName;
                    beenSaved = true;
                    needSave = false;
                    UpdateTitleBar();
                    // show first page
                    ShowFirstPage();
                    // add to recent documents
                    AddRecentDocument(fileName);
                    // start autosaving
                    StartAutoSaveTimer();
                    // result true!
                    result = true;
                }
                catch (Exception e2)
                {
                    undoRedoArea.Cancel();
                    MessageBox.Show(e2.Message, WbLocale.ErrorReadingFile, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                pf.Close();
            };
            pf.ShowDialog();
            return result;
        }

        void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = FileExt;
            ofd.Filter = OpenFileTypeFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenFile(ofd.FileName);
        }

        bool _Save(string backupFile)
        {
            CheckState();
            bool retval = false;
            try
            {
                // make extra enties dict (from attachments)
                Dictionary<string, byte[]> extraEntries = new Dictionary<string, byte[]>();
                foreach (string name in attachmentView1.GetAttachmentNames())
                    extraEntries.Add(AttachmentView.ATTACHMENTS_DIR + Path.DirectorySeparatorChar + name,
                        attachmentView1.GetAttachment(name));
                // save
                if (backupFile != null)
                    FileHelper.Save(backupFile, engines, NewPageSize, BackgroundFigure, extraEntries);
                else
                {
                    FileHelper.Save(fileName, engines, NewPageSize, BackgroundFigure, extraEntries);
                    Dirty = false;
                    beenSaved = true;
                    needSave = false;
                    UpdateTitleBar();
                    // add to recent documents
                    AddRecentDocument(fileName);
                }
                // set retval to true
                retval = true;
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message, WbLocale.ErrorWritingFile, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retval;
        }

        bool Save()
        {
            bool retval = false;
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = WbLocale.SavingFile;
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                retval = _Save(null);
                pf.Close();
            };
            pf.ShowDialog();
            return retval;
        }

        bool SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = FileExt;
            sfd.Filter = SaveFileTypeFilter;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;
                return Save();
            }
            else
                return false;
        }

        bool CheckDirty()
        {
            if (Dirty || needSave)
            {
                switch (MessageBox.Show(string.Format("{0} \"{1}\"?", WbLocale.SaveChangesTo, fileName), ProgramName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        if (!beenSaved)
                            return SaveAs();
                        else
                            return Save();
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
            if (engines.Count > 0)
            {
                PrintDialog pf = new PrintDialog();
                pf.UseEXDialog = true;
                pf.AllowSelection = false;
                if (engines.Count > 1)
                {
                    pf.AllowCurrentPage = true;
                    pf.AllowSomePages = true;
                    pf.PrinterSettings.MinimumPage = 1;
                    pf.PrinterSettings.MaximumPage = engines.Count;
                    pf.PrinterSettings.FromPage = 1;
                    pf.PrinterSettings.ToPage = engines.Count;
                }
                if (pf.ShowDialog() == DialogResult.OK)
                {
                    DPrintViewer dvPrint = new DPrintViewer();
                    // page iteration vars
                    IEnumerator<DEngine> engineEnumerator = engines.GetEnumerator();
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
                                de = engines[pageIdx];
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
                        DGraphics dg = WFHelper.MakePrintGraphics(e2.Graphics);
                        dvPrint.Paint(dg, dps, de.BackgroundFigure, de.Figures);
                        dg.Dispose();
                    };
                    // call print operation
                    pd.Print();
                }
            }
        }

        private void actNewPage_Execute(object sender, EventArgs e)
        {
            CheckState();
            undoRedoArea.Start(WbLocale.NewPage);
            CreateDEngine(de);
            undoRedoArea.Commit();
        }

        private void actDeletePage_Execute(object sender, EventArgs e)
        {
            DoDelete();
        }

        private void actClonePage_Execute(object sender, EventArgs e)
        {
            CheckState();
            undoRedoArea.Start(WbLocale.ClonePage);
            // clone data
            string clonedFigures = FigureSerialize.FormatToXml(de.Figures, null);
            string clonedBackground = null;
            if (de.CustomBackgroundFigure)
                clonedBackground = FigureSerialize.FormatToXml(de.BackgroundFigure, null);
            // create new DEngine
            DPoint sz = de.PageSize;
            CreateDEngine(de);
            de.PageSize = sz;
            // add figures from clone data
            if (clonedBackground != null)
                de.SetBackgroundFigure((BackgroundFigure)FigureSerialize.FromXml(clonedBackground)[0], true);
            List<Figure> figs = FigureSerialize.FromXml(clonedFigures);
            foreach (Figure f in figs)
                de.AddFigure(f);
            undoRedoArea.Commit();
        }

        private void actClearPage_Execute(object sender, EventArgs e)
        {
            CheckState();
            de.UndoRedo.Start(WbLocale.ClearPage);
            de.ClearPage();
            de.UndoRedo.Commit();
        }

        private void actRenamePage_Execute(object sender, EventArgs e)
        {
            previewBar1.RenameCurrentPreview();
        }

        private void actAnnoTools_Execute(object sender, EventArgs e)
        {
            ShowAnnoTools(false);
        }

        private void actScreenCapture_Execute(object sender, EventArgs e)
        {
            ScreenCaptureForm screenCaptureForm = new ScreenCaptureForm();
            screenCaptureForm.CaptureImage += new ImportAnnotationsImageHandler(screenCaptureForm_CaptureImage);
            screenCaptureForm.Show();
        }

        void screenCaptureForm_CaptureImage(DBitmap bmp)
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = WbLocale.ImportingScreenCapture;
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // import annotations
                undoRedoArea.Start(WbLocale.ImportScreenCapture);
                BitmapFigure f = new BitmapFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, WFHelper.ToImageData(bmp), "screencap.png");
                de.AddFigure(f);
                dvEditor.Update();
                undoRedoArea.Commit();
                // close dialog
                pf.Close();
            };
            pf.ShowDialog();
        }

        private void actLink_Execute(object sender, EventArgs e)
        {
            List<Figure> figs = de.SelectedFigures;
            if (figs.Count == 1)
            {
                Figure f = figs[0];
                LinkForm lf = new LinkForm();
                lf.CurrentEngine = de;
                lf.Engines = engines;
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
                        undoRedoArea.Start(WbLocale.ChangeLink);
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
                        undoRedoArea.Commit();
                        CheckLinkGlyph(f);
                        break;
                    case DialogResult.Abort:
                        undoRedoArea.Start(WbLocale.RemoveLink);
                        f.UserAttrs.Remove(Links.Link);
                        f.UserAttrs.Remove(Links.LinkType);
                        f.UserAttrs.Remove(Links.LinkBody);
                        undoRedoArea.Commit();
                        CheckLinkGlyph(f);
                        break;
                }
                // update editor view
                dvEditor.Update();
            }
        }

        private void actLockFigure_Execute(object sender, EventArgs e)
        {
            if (de.SelectedFigures.Count > 0)
            {
                undoRedoArea.Start(WbLocale.ChangeFigureLock);
                foreach (Figure f in de.SelectedFigures)
                    f.Locked = !f.Locked;
                undoRedoArea.Commit();
                dvEditor.Update();
            }
        }

        private void actUndo_Execute(object sender, EventArgs e)
        {
            CheckState();
            undoRedoArea.Undo();
        }

        private void actRedo_Execute(object sender, EventArgs e)
        {
            CheckState();
            undoRedoArea.Redo();
        }

        private void actSelectAll_Execute(object sender, EventArgs e)
        {
            if (de.HsmState == DHsmState.Select)
                de.SelectAll();
        }

        private void actDimensions_Execute(object sender, EventArgs e)
        {
            DimensionsForm df = new DimensionsForm();
            df.Figures = de.SelectedFigures;
            if (df.ShowDialog() == DialogResult.OK)
            {
                undoRedoArea.Start(WbLocale.ChangeFigureDimensions);
                DRect origRect = df.BoundingRect(df.Figures);
                foreach (Figure f in df.Figures)
                {
                    if (df.GroupX)
                        f.X = df.FigX;
                    else
                        f.X += df.FigX - origRect.X;
                    if (df.GroupY)
                        f.Y = df.FigY;
                    else
                        f.Y += df.FigY - origRect.Y;
                    f.BeforeResize();
                    if (df.GroupWidth)
                        f.Width = df.FigWidth;
                    if (df.GroupHeight)
                        f.Height = df.FigHeight;
                    f.AfterResize();
                    if (df.GroupRotation)
                        f.Rotation = df.FigRotation;
                }
                if (df.Figures.Count == 1)
                {
                    df.Figures[0].BeforeResize();
                    df.Figures[0].Width = df.FigWidth;
                    df.Figures[0].Height = df.FigHeight;
                    df.Figures[0].AfterResize();
                    df.Figures[0].Rotation = df.FigRotation;
                }
                undoRedoArea.Commit();
                de.UpdateViewers();
            }
        }

        private void actProperties_Execute(object sender, EventArgs e)
        {
            if (de.SelectedFigures.Count > 0)
            {
                PropertiesForm f = new PropertiesForm();
                f.Figures = de.SelectedFigures;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    // update all selected figures to have the same properties as the PropertiesForm figures
                    undoRedoArea.Start(WbLocale.ChangeFigureProperties);
                    List<Figure> propFigs = WorkBookUtils.FlatFigureList(f.Figures);
                    List<Figure> deFigs = WorkBookUtils.FlatFigureList(de.SelectedFigures);
                    for (int i = 0; i < propFigs.Count; i++)
                        DAuthorProperties.FromFigure(propFigs[i]).ApplyPropertiesToFigure(deFigs[i]);
                    undoRedoArea.Commit();
                    de.UpdateViewers();
                    // update property state toolstrip
                    tsPropState.De = de;
                }
            }
        }

        private void actExportSelectionToImage_Execute(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(de.SelectedFigures.Count > 0, "ERROR: No figures selected");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG File|*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                CheckState();
                DBitmap bmp = FigureSerialize.FormatToBmp(de.SelectedFigures, dvEditor.AntiAlias, DColor.Empty);
                bmp.Save(sfd.FileName);
            }        
        }

        private void actExportSelectionToEmf_Execute(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(de.SelectedFigures.Count > 0, "ERROR: No figures selected");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "EMF File|*.emf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                CheckState();
                byte[] data = FigureSerialize.FormatToEmf(de.SelectedFigures, WorkBookUtils.GetScreenMM(), WorkBookUtils.GetScreenRes());
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);
                FileStream fs = File.Create(sfd.FileName);
                fs.Write(data, 0, data.Length);
                fs.Close();
            }             
        }

        private void actUnwrapText_Execute(object sender, EventArgs e)
        {
            if (de.SelectedFigures.Count == 1 && de.SelectedFigures[0] is ITextable)
            {
                undoRedoArea.Start(WbLocale.RemoveTextWrap);
                ((ITextable)de.SelectedFigures[0]).WrapText = false;
                undoRedoArea.Commit();
                dvEditor.Update();
            }
        }

        void ShowAnnoTools(bool fromCmdLine)
        {
            CheckState();
            AnnoToolsForm atf = AnnoToolsForm.GlobalAtf;
            if (!atf.Visible)
            {
                atf.ImportAnnotationsPage += new ImportAnnotationsPageHandler(AnnoTools_ImportAnnotationsPage);
                atf.ImportAnnotationsArea += new ImportAnnotationsImageHandler(AnnoTools_ImportAnnotationsArea);
                atf.MainForm = this;
                atf.FromCmdLine = fromCmdLine;
                atf.Show();
            }
            atf.Focus();
        }

        void AnnoTools_ImportAnnotationsPage(DEngine de)
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = WbLocale.ImportingScreenAsPage;
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // import annotations
                undoRedoArea.Start(WbLocale.ImportAnnotations);
                CreateDEngine(this.de);
                this.de.PageSize = de.PageSize;
                this.de.SetBackgroundFigure(de.BackgroundFigure, true);
                foreach (Figure f in de.Figures)
                    this.de.AddFigure(f);
                undoRedoArea.Commit();
                // make sure previews line up properly
                previewBar1.ResetPreviewPositions();
                // close dialog
                pf.Close();
            };
            pf.ShowDialog();
        }

        void AnnoTools_ImportAnnotationsArea(DBitmap bmp)
        {
            // progress form
            ProgressForm pf = new ProgressForm();
            pf.Text = WbLocale.ImportingAreaAsImage;
            pf.Shown += delegate(object s, EventArgs e)
            {
                Application.DoEvents();
                // import annotations
                undoRedoArea.Start(WbLocale.ImportAnnotations);
                BitmapFigure f = new BitmapFigure(new DRect(10, 10, bmp.Width, bmp.Height), 0, WFHelper.ToImageData(bmp), "annotations.png");
                de.AddFigure(f);
                dvEditor.Update();
                undoRedoArea.Commit();
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
                string.Compare(ext, ".bmp", true) == 0 ||
                string.Compare(ext, ".wmf", true) == 0)
                return true;
            return false;
        }

        bool IsWmfFilePath(string path)
        {
            return string.Compare(Path.GetExtension(path), ".wmf", true) == 0;
        }

        private void wfvcEditor_DragDrop(object sender, DragEventArgs e)
        {
            CheckState();
            Point cpt = wfvcEditor.PointToClient(new Point(e.X, e.Y));
            DPoint pt = dvEditor.ClientToEngine(new DPoint(cpt.X, cpt.Y));
            PasteDataObject(e.Data, WbLocale.Copy, pt.X, pt.Y);
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

        private void attachmentView1_FileDrop(object sender, string[] filePaths)
        {
            CheckState();
            undoRedoArea.Start(WbLocale.AddAttachments);
            foreach (string path in filePaths)
                if (attachmentView1.CheckAttachmentExists(path))
                    attachmentView1.AddAttachment(path);
            undoRedoArea.Commit();
        }

        private void attachmentView1_Insert(object sender, EventArgs e)
        {
            InsertAttachment();
        }

        private void attachmentView1_Delete(object sender, EventArgs e)
        {
            DeleteAttachment();
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
                    CheckState();
                    // setup progress form
                    ProgressForm pf = new ProgressForm();
                    pf.Text = WbLocale.ImportingNotebookFile;
                    pf.Shown += delegate(object s, EventArgs e2)
                    {
                        Application.DoEvents();
                        // start undo/redo
                        undoRedoArea.Start(WbLocale.ImportNotebook);
                        try
                        {
                            // clear pages & attachments
                            ClearDocument();
                            attachmentView1.ClearAttachments();
                            // load notebook file
                            Dictionary<string, byte[]> attachments;
                            List<DEngine> engines = Converters.Converters.FromNotebook(ofd.FileName, undoRedoArea, out attachments);
                            // add new pages
                            foreach (DEngine de in engines)
                            {
                                InitDEngine(de, false);
                                this.engines.Add(de);
                                Application.DoEvents(); // update progress form
                            }
                            // add new attachments
                            foreach (string name in attachments.Keys)
                                attachmentView1.AddAttachment(name, attachments[name]);
                            // clear undos
                            undoRedoArea.Commit();
                            undoRedoArea.ClearHistory();
                            // update vars
                            fileName = Path.GetFileNameWithoutExtension(ofd.FileName);
                            beenSaved = false;
                            needSave = false;
                            UpdateTitleBar();
                            // show first page
                            ShowFirstPage();
                            // start autosaving
                            StartAutoSaveTimer();
                        }
                        catch (Exception e3)
                        {
                            undoRedoArea.Cancel();
                            MessageBox.Show(e3.Message, WbLocale.ErrorReadingFile, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        pf.Close();
                    };
                    pf.ShowDialog();
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckState();
            new ExportForm(fileName, engines, de).ShowDialog();
        }

        void ShowFirstPage()
        {
            if (engines.Count > 0)
                previewBar1.SetPreviewSelected(engines[0]);
        }

        private void Toolbars_MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = ((ToolStripMenuItem)sender);
            mi.Checked = !mi.Checked;
            UpdateToolbars();
        }

        void UpdateToolbars()
        {
            tsEdit.Visible = editToolStripMenuItem1.Checked;
            tsPersonal.Visible = personalToolStripMenuItem.Checked;
            tsEngineState.Visible = modeSelectToolStripMenuItem.Checked;
            tsPropState.Visible = propertySelectToolStripMenuItem.Checked;
            tsPageManage.Visible = pageNavigationToolStripMenuItem.Checked;
            tsTools.Visible = toolsToolStripMenuItem1.Checked;
        }

        private void flipXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRedoArea.Start(WbLocale.FlipLeftRight);
            foreach (Figure fig in de.SelectedFigures)
                fig.FlipX = !fig.FlipX;
            undoRedoArea.Commit();
            dvEditor.Update();
        }

        private void flipYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRedoArea.Start(WbLocale.FlipUpDown);
            foreach (Figure fig in de.SelectedFigures)
                fig.FlipY = !fig.FlipY;
            undoRedoArea.Commit();
            dvEditor.Update();
        }

        private void tsEngineState_AddToPersonalTools(object sender, PersonalToolbar.CustomFigureTool customFigure)
        {
            tsPersonal.AddCustomFigure(customFigure);
        }

        private void tsEngineState_DapChanged(object sender, DAuthorProperties dap)
        {
            tsPropState.Dap = dap;
            tsPersonal.Dap = dap;
        }

        private void wfvcEditor_MouseEnter(object sender, EventArgs e)
        {
            PopupForm.HidePopups();
        }

        private void tsPersonal_ItemContext(object sender, EventArgs e)
        {
            de.CheckState();
        }

        private void mailRecipientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // find filename
            string tempFileName = WorkBookUtils.GetTempFileName(Path.GetFileName(fileName), tempFilesDir, FileExt);
            // save file
            if (_Save(tempFileName))
            {
                // load mail client
                MAPI mapi = new MAPI();
                mapi.AddAttachment(tempFileName);
                mapi.SendMailPopup(string.Format("{0}: {1}", WbLocale.Emailing, Path.GetFileNameWithoutExtension(fileName)), "");
            }
        }

        private void mailRecipientasPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // find filename
            string tempFileName = WorkBookUtils.GetTempFileName(Path.GetFileName(fileName), tempFilesDir, ".pdf");
            try
            {
                WorkBookUtils.RenderPdf(engines, tempFileName);
                // load mail client
                MAPI mapi = new MAPI();
                mapi.AddAttachment(tempFileName);
                mapi.SendMailPopup(string.Format("{0}: {1}", WbLocale.Emailing, Path.GetFileNameWithoutExtension(fileName)), "");
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message, WbLocale.ErrorMailingPDF, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridForm f = new GridForm();
            f.GridSize = gridSize;
            f.ShowGrid = showGrid;
            f.SnapPosition = gridSnapPosition;
            f.SnapResize = gridSnapResize;
            f.SnapLines = gridSnapLines;
            if (f.ShowDialog() == DialogResult.OK)
            {
                gridSize = f.GridSize;
                showGrid = f.ShowGrid;
                gridSnapPosition = f.SnapPosition;
                gridSnapResize = f.SnapResize;
                gridSnapLines = f.SnapLines;
                SetGridOptionsForCurrentDe();
            }
        }
    }
}