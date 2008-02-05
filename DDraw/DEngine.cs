using System;
using System.Collections.Generic;
using System.Text;

using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public delegate void DebugMessageHandler(string msg);
    public delegate void PageSizeChangedHandler(DEngine de, DPoint pageSize);
    public delegate void ContextClickHandler(DEngine de, Figure clickedFigure, DPoint pt);
    public delegate void AddedFigureHandler(DEngine de, Figure fig);

    public class DAuthorProperties
    {
        public DColor Fill;
        public DColor Stroke;
        public double StrokeWidth;
        public DStrokeStyle StrokeStyle;
        public DMarker StartMarker;
        public DMarker EndMarker;
        public double Alpha;
        public string FontName;
        public bool Bold, Italics, Underline, Strikethrough;
        
        public DAuthorProperties()
        {
        	Fill = DColor.Red;
        	Stroke = DColor.Blue;
        	StrokeWidth = 1;
            StrokeStyle = DStrokeStyle.Solid;
            StartMarker = DMarker.None;
            EndMarker = DMarker.None;
        	Alpha = 1;
        	FontName = "Arial";
            Bold = false;
            Italics = false;
            Underline = false;
            Strikethrough = false;
        }

        public DAuthorProperties(DColor fill, DColor stroke, double strokeWidth, DStrokeStyle strokeStyle, 
            DMarker startMarker, DMarker endMarker, double alpha, string fontName, bool bold, bool italics,
            bool underline, bool strikethrough)
        {
            Fill = fill;
            Stroke = stroke;
            StrokeWidth = strokeWidth;
            StrokeStyle = strokeStyle;
            StartMarker = startMarker;
            EndMarker = endMarker;
            Alpha = alpha;
            FontName = fontName;
            Bold = bold;
            Italics = italics;
            Underline = underline;
            Strikethrough = strikethrough;
        }

        public void ApplyPropertiesToFigure(Figure f)
        {
            if (f is IFillable)
                ((IFillable)f).Fill = Fill;
            if (f is IStrokeable)
            {
                ((IStrokeable)f).Stroke = Stroke;
                ((IStrokeable)f).StrokeWidth = StrokeWidth;
                ((IStrokeable)f).StrokeStyle = StrokeStyle;
            }
            if (f is IMarkable)
            {
                ((IMarkable)f).StartMarker = StartMarker;
                ((IMarkable)f).EndMarker = EndMarker;
            }
            if (f is IAlphaBlendable)
                ((IAlphaBlendable)f).Alpha = Alpha;
            if (f is ITextable)
            {
                ((ITextable)f).FontName = FontName;
                ((ITextable)f).Bold = Bold;
                ((ITextable)f).Italics = Italics;
                ((ITextable)f).Underline = Underline;
                ((ITextable)f).Strikethrough = Strikethrough;
            }
        }
    }

    public class UndoRedoManager : UndoRedoArea
    {
        static int instanceNumber = 0;

        public UndoRedoManager() : base("UndoRedoArea #" + instanceNumber.ToString())
        {
            // increment static instanceNumber variable
            instanceNumber += 1;
        }

        public new IDisposable Start(string name)
        {
            if (!IsCommandStarted)
                return base.Start(name);
            else
                return null;
        }
    }

    public class DEngine
    {
        UndoRedoManager undoRedoManager;
        public bool CanUndo
        {
            get { return undoRedoManager.CanUndo; }
        }
        public bool CanRedo
        {
            get { return undoRedoManager.CanRedo; }
        }
        public IEnumerable<string> UndoCommands
        {
            get { return undoRedoManager.UndoCommands; }
        }
        public IEnumerable<string> RedoCommands
        {
            get { return undoRedoManager.RedoCommands; }
        }
        public void UndoRedoStart(string name)
        {
            undoRedoManager.Start(name);
        }
        public void UndoRedoCommit()
        {
            undoRedoManager.Commit();
        }
        public void UndoRedoCancel()
        {
            undoRedoManager.Cancel();
        }
        public void UndoRedoClearHistory()
        {
            undoRedoManager.ClearHistory();
        }
        public void Undo()
        {
            undoRedoManager.Undo();
        }
        public void Redo()
        {
            undoRedoManager.Redo();
        }

        DViewerHandler viewerHandler;

        DFigureHandler figureHandler;
        public List<Figure> SelectedFigures
        {
            get { return figureHandler.SelectedFigures; }
        }
        public List<Figure> Figures
        {
            get { return new List<Figure>(figureHandler.Figures.ToArray()); }
        }

        DHsm hsm;
        public DHsmState HsmState
        {
            get { return hsm.State; }
            set { hsm.State = value; }
        }
        public bool FigureLockAspectRatio
        {
            get { return hsm.FigureLockAspectRatio; }
            set { hsm.FigureLockAspectRatio = value; }
        }
        public bool FigureAlwaysSnapAngle
        {
            get { return hsm.FigureAlwaysSnapAngle; }
            set { hsm.FigureAlwaysSnapAngle = value; }
        }
        public bool SimplifyPolylines
        {
            get { return hsm.SimplifyPolylines; }
            set { hsm.SimplifyPolylines = value; }
        }
        public double SimplifyPolylinesTolerance
        {
            get { return hsm.SimplifyPolylinesTolerance; }
            set { hsm.SimplifyPolylinesTolerance = value; }
        }

        UndoRedo<DPoint> _pageSize = new UndoRedo<DPoint>(new DPoint(PageTools.DefaultPageWidth, PageTools.DefaultPageHeight));
        public DPoint PageSize
        {
            get { return _pageSize.Value; }
            set
            {
                if (!value.Equals(_pageSize.Value))
                {
                    bool meStartCommand = !undoRedoManager.IsCommandStarted;
                    if (meStartCommand)
                        undoRedoManager.Start("Set Page Size");
                    _pageSize.Value = value;
                    figureHandler.SetBackgroundFigureSize(value);
                    if (meStartCommand)
                        undoRedoManager.Commit();
                }
                if (PageSizeChanged != null)
                    PageSizeChanged(this, value);
                viewerHandler.SetPageSize(value);
            }
        }       
        public PageFormat PageFormat
        {
            get { return PageTools.SizeToFormat(PageSize); }
            set 
            {
                if (value != PageFormat.Custom)
                    PageSize = PageTools.FormatToSize(value); 
            }
        }

        public event DebugMessageHandler DebugMessage;
        public event SelectedFiguresHandler SelectedFiguresChanged;
        public event ContextClickHandler ContextClick;
        public event PageSizeChangedHandler PageSizeChanged;
        public event EventHandler UndoRedoChanged;
        public event HsmStateChangedHandler HsmStateChanged;
        public event AddedFigureHandler AddedFigure;

        public DEngine(DAuthorProperties authorProps)
        {
            // create the undo/redo manager
            undoRedoManager = new UndoRedoManager();
            undoRedoManager.CommandDone += new EventHandler<CommandDoneEventArgs>(undoRedoArea_CommandDone);
            // create viewer handler
            viewerHandler = new DViewerHandler();
            // create figure handler
            undoRedoManager.Start("create figure handler");
            figureHandler = new DFigureHandler();
            figureHandler.SelectedFiguresChanged += new SelectedFiguresHandler(DoSelectedFiguresChanged);
            figureHandler.AddedFigure += delegate(DEngine de, Figure fig)
            {
                if (AddedFigure != null)
                    AddedFigure(this, fig);
            };
            undoRedoManager.Commit();
            undoRedoManager.ClearHistory();
            // create state machine
            hsm = new DHsm(undoRedoManager, viewerHandler, figureHandler, authorProps);
            hsm.DebugMessage += new DebugMessageHandler(hsm_DebugMessage);
            hsm.ContextClick += new ContextClickHandler(hsm_ContextClick);
            hsm.StateChanged += new HsmStateChangedHandler(hsm_StateChanged);
        }

        void undoRedoArea_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (e.CommandDoneType == CommandDoneType.Undo || e.CommandDoneType == CommandDoneType.Redo)
            {
                ClearSelected();
                UpdateViewers();
                // in case the page size was undooed
                PageSize = PageSize;
            }
            if (UndoRedoChanged != null)
                UndoRedoChanged(this, e);
        }

        void hsm_StateChanged(DEngine de, DHsmState state)
        {
            if (HsmStateChanged != null)
                HsmStateChanged(this, state);
        }

        void hsm_ContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (ContextClick != null)
                ContextClick(this, clickedFigure, pt);
        }

        void hsm_DebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }

        // Public Functions //

        public void AddFigure(Figure f)
        {
            figureHandler.Add(f);
        }

        public void RemoveFigure(Figure f)
        {
            figureHandler.Remove(f);
        }

        public void SelectFigures(IList<Figure> figs)
        {
            figureHandler.Select(figs);
        }

        public void AddViewer(DTkViewer dv)
        {
            dv.SetPageSize(PageSize);
            viewerHandler.Add(dv);
        }

        public void RemoveViewer(DTkViewer dv)
        {
            viewerHandler.Remove(dv);
        }

        public void UpdateViewers()
        {
            viewerHandler.Update();
        }

        public void ClearSelected()
        {
            figureHandler.ClearSelected();
        }

        public bool CanGroupFigures(List<Figure> figs)
        {
            return figureHandler.CanGroupFigures(figs);
        }

        public void GroupFigures(List<Figure> figs)
        {
            if (CanGroupFigures(figs))
            {
                // init undoRedo frame
                UndoRedoStart("Group");
                // make group
                figureHandler.GroupFigures(figs);
                // commit changes to undoRedoArea
                UndoRedoCommit();
                // update all viewers
                UpdateViewers();
            }
        }

        public bool CanUngroupFigures(List<Figure> figs)
        {
            return figureHandler.CanUngroupFigures(figs);
        }

        public void UngroupFigures(List<Figure> figs)
        {
            if (CanUngroupFigures(figs))
            {
                // init undoRedo frame
                UndoRedoStart("Ungroup");
                // perform ungroup
                figureHandler.UngroupFigures(figs);
                // commit changes to undoRedoArea
                UndoRedoCommit();
                // update all viewers
                UpdateViewers();
            }
        }

        public void SendToBack(List<Figure> figs)
        {
            // init undoRedo frame
            UndoRedoStart("Send to Back");
            // send to back
            figureHandler.SendToBack(figs);
            // update viewers
            UpdateViewers();
            // commit changes to undoRedoArea
            UndoRedoCommit(); 
        }

        public void BringToFront(List<Figure> figs)
        {
            // init undoRedo frame
            UndoRedoStart("Bring to Front");
            // bring to front
            figureHandler.BringToFront(figs);
            // update viewers
            UpdateViewers();
            // commit changes to undoRedoArea
            UndoRedoCommit(); 
        }

        public void SendBackward(List<Figure> figs)
        {            
            // init undoRedo frame
            UndoRedoStart("Send Backward");
            // send backward
            figureHandler.SendBackward(figs);
            // update
            UpdateViewers();
            // commit changes to undoRedoArea
            UndoRedoCommit(); 
        }

        public bool CanSendBackward(List<Figure> figs)
        {
            return figureHandler.CanSendBackward(figs);
        }

        public void BringForward(List<Figure> figs)
        {
            // init undoRedo frame
            UndoRedoStart("Bring Forward");
            // bring forward
            figureHandler.BringForward(figs);
            // update viewers
            UpdateViewers();
            // commit changes to undoRedoArea
            UndoRedoCommit();
        }

        public bool CanBringForward(List<Figure> figs)
        {
            return figureHandler.CanBringForward(figs);
        }

        public string Cut(List<Figure> figs, out DBitmap bmp, bool bmpAntiAlias)
        {
            if (CanCopy(figs))
            {
                string data = Copy(figs, out bmp, bmpAntiAlias);
                UndoRedoStart("Cut");
                foreach (Figure f in figs)
                    RemoveFigure(f);
                UndoRedoCommit();
                ClearSelected();
                UpdateViewers();
                return data;
            }
            bmp = null;
            return null;
        }

        public bool CanCopy(List<Figure> figs)
        {
            return figs.Count > 0;
        }

        public string Copy(List<Figure> figs, out DBitmap bmp, bool bmpAntiAlias)
        {
            bmp = FigureSerialize.FormatToBmp(figs, bmpAntiAlias);
            return FigureSerialize.FormatToXml(figs, null);
        }

        public void PasteAsSelectedFigures(string data)
        {
            UndoRedoStart("Paste");
            PasteAsSelectedFigures(FigureSerialize.FromXml(data));
            UndoRedoCommit();
        }

        public void PasteAsSelectedFigures(IList<Figure> figs)
        {
            foreach (Figure f in figs)
                AddFigure(f);
            SelectFigures(figs);
            UpdateViewers();
        }

        public void Delete(List<Figure> figs)
        {
            UndoRedoStart("Delete Figures");
            foreach (Figure f in figs)
                RemoveFigure(f);
            UndoRedoCommit();
            ClearSelected();
            UpdateViewers();
        }

        public BackgroundFigure GetBackgroundFigure()
        {
            return figureHandler.BackgroundFigure;
        }

        public void SetBackgroundFigure(BackgroundFigure f)
        {
            f.Rect = new DRect(0, 0, PageSize.X, PageSize.Y);
            figureHandler.BackgroundFigure = f;
            viewerHandler.Update();
        }

        public void SetEraserSize(double size)
        {
            figureHandler.SetEraserSize(size);
        }

        public bool HsmCurrentFigClassImpls(Type _interface)
        {
            return hsm.CurrentFigClassImpls(_interface);
        }

        public bool HsmCurrentFigClassIs(Type _class)
        {
            return hsm.CurrentFigClassIs(_class);
        }

        public void HsmSetStateByFigureClass(Type figureClass)
        {
            hsm.SetStateByFigureClass(figureClass);
        }

        // Other //

        void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }

        void DoSelectedFiguresChanged()
        {
            if (SelectedFiguresChanged != null)
                SelectedFiguresChanged();
        }
    }
}
