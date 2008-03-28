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
    public delegate void DragFigureHandler(DEngine de, Figure dragFigure, DPoint pt);
    public delegate void SelectMeasureHandler(DEngine de, DRect rect);
    public delegate void AddedFigureHandler(DEngine de, Figure fig);

    public delegate void AuthorPropertyChanged(DAuthorProperties dap);

    public class DAuthorProperties
    {
        DColor fill;
        public DColor Fill
        {
            get { return fill; }
            set { fill = value; DoPropertyChanged(); }
        }
        DColor stroke;
        public DColor Stroke
        {
            get { return stroke; }
            set { stroke = value; DoPropertyChanged(); }
        }
        double strokeWidth;
        public double StrokeWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; DoPropertyChanged(); }
        }
        DStrokeStyle strokeStyle;
        public DStrokeStyle StrokeStyle
        {
            get { return strokeStyle; }
            set { strokeStyle = value; DoPropertyChanged(); }
        }
        DMarker startMarker;
        public DMarker StartMarker
        {
            get { return startMarker; }
            set { startMarker = value; DoPropertyChanged(); }
        }
        DMarker endMarker;
        public DMarker EndMarker
        {
            get { return endMarker; }
            set { endMarker = value; DoPropertyChanged(); }
        }
        double alpha;
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; DoPropertyChanged(); }
        }
        string fontName;
        public string FontName
        {
            get { return fontName; }
            set { fontName = value; DoPropertyChanged(); }
        }
        bool bold;
        public bool Bold
        {
            get { return bold; }
            set { bold = value; DoPropertyChanged(); }
        }
        bool italics;
        public bool Italics
        {
            get { return italics; }
            set { italics = value; DoPropertyChanged(); }
        }
        bool underline;
        public bool Underline
        {
            get { return underline; }
            set { underline = value; DoPropertyChanged(); }
        }
        bool strikethrough;
        public bool Strikethrough
        {
            get { return strikethrough; }
            set { strikethrough = value; DoPropertyChanged(); }
        }

        public event AuthorPropertyChanged PropertyChanged;
        
        public DAuthorProperties()
        {
        	fill = DColor.Red;
        	stroke = DColor.Blue;
        	strokeWidth = 1;
            strokeStyle = DStrokeStyle.Solid;
            startMarker = DMarker.None;
            endMarker = DMarker.None;
        	alpha = 1;
        	fontName = "Arial";
            bold = false;
            italics = false;
            underline = false;
            strikethrough = false;
        }

        public void SetProperties(DColor fill, DColor stroke, double strokeWidth, DStrokeStyle strokeStyle, 
            DMarker startMarker, DMarker endMarker, double alpha, string fontName, bool bold, bool italics,
            bool underline, bool strikethrough)
        {
            this.fill = fill;
            this.stroke = stroke;
            this.strokeWidth = strokeWidth;
            this.strokeStyle = strokeStyle;
            this.startMarker = startMarker;
            this.endMarker = endMarker;
            this.alpha = alpha;
            this.fontName = fontName;
            this.bold = bold;
            this.italics = italics;
            this.underline = underline;
            this.strikethrough = strikethrough;
            DoPropertyChanged();
        }

        void DoPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this);
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

        static DAuthorProperties ap = null;
        public static DAuthorProperties GlobalAP
        {
            get
            {
                if (ap == null)
                    ap = new DAuthorProperties();
                return ap;
            }
        }
    }

    public class DEngine
    {
        UndoRedoArea undoRedoArea;
        public bool CanUndo
        {
            get { return undoRedoArea.CanUndo; }
        }
        public bool CanRedo
        {
            get { return undoRedoArea.CanRedo; }
        }
        public IEnumerable<CommandId> UndoCommands
        {
            get { return undoRedoArea.UndoCommands; }
        }
        public IEnumerable<CommandId> RedoCommands
        {
            get { return undoRedoArea.RedoCommands; }
        }
        public void UndoRedoStart(string name)
        {
            undoRedoArea.Start(name);
        }
        public void UndoRedoCommit()
        {
            undoRedoArea.Commit();
        }
        public void UndoRedoCancel()
        {
            undoRedoArea.Cancel();
        }
        public void UndoRedoClearHistory()
        {
            undoRedoArea.ClearHistory();
        }
        public void UndoRedoClearRedos()
        {
            undoRedoArea.ClearRedos();
        }
        public void Undo()
        {
            undoRedoArea.Undo();
        }
        public void Redo()
        {
            undoRedoArea.Redo();
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
        public Type CurrentFigureClass
        {
            get { return hsm.CurrentFigureClass; }
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
        public bool FigureSelectAddToSelection
        {
            get { return hsm.FigureSelectAddToSelection; }
            set { hsm.FigureSelectAddToSelection = value; }
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
        public bool AutoGroupPolylines
        {
            get { return hsm.AutoGroupPolylines; }
            set { hsm.AutoGroupPolylines = value; }
        }
        public int AutoGroupPolylinesTimeout
        {
            get { return hsm.AutoGroupPolylinesTimeout; }
            set { hsm.AutoGroupPolylinesTimeout = value; }
        }
        public int AutoGroupPolylinesXLimit
        {
            get { return hsm.AutoGroupPolylinesXLimit; }
            set { hsm.AutoGroupPolylinesXLimit = value; }
        }
        public int AutoGroupPolylinesYLimit
        {
            get { return hsm.AutoGroupPolylinesYLimit; }
            set { hsm.AutoGroupPolylinesYLimit = value; }
        }
        public bool UsePolylineDots
        {
            get { return hsm.UsePolylineDots; }
            set { hsm.UsePolylineDots = value; }
        }
        public bool FiguresBoundToPage
        {
            get { return hsm.FiguresBoundToPage; }
            set { hsm.FiguresBoundToPage = value; }
        }
        public int KeyMovementRate
        {
            get { return hsm.KeyMovementRate; }
            set { hsm.KeyMovementRate = value; }
        }

        UndoRedo<DPoint> _pageSize = new UndoRedo<DPoint>(new DPoint(PageTools.DefaultPageWidth, PageTools.DefaultPageHeight));
        public DPoint PageSize
        {
            get { return _pageSize.Value; }
            set
            {
                if (!value.Equals(_pageSize.Value))
                {
                    _pageSize.Value = value;
                    figureHandler.SetBackgroundFigureSize(value);
                }
                if (PageSizeChanged != null)
                    PageSizeChanged(this, value);
                viewerHandler.SetPageSize(value);
                hsm.SetPageSize(value);
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
        public event DragFigureHandler DragFigureStart;
        public event DragFigureHandler DragFigureEvt;
        public event DragFigureHandler DragFigureEnd;
        public event DMouseButtonEventHandler MouseDown;
        public event PageSizeChangedHandler PageSizeChanged;
        public event EventHandler UndoRedoChanged;
        public event EventHandler<CommandDoneEventArgs> UndoRedoCommandDone;
        public event HsmStateChangedHandler HsmStateChanged;
        public event AddedFigureHandler AddedFigure;
        public event SelectMeasureHandler MeasureRect;

        static int instanceNumber = 0;

        public DEngine(DAuthorProperties authorProps, bool usingEngineManager)
        {
            // create the undo/redo manager
            undoRedoArea = new UndoRedoArea(string.Format("area #{0}", instanceNumber));
            undoRedoArea.CommandDone += new EventHandler<CommandDoneEventArgs>(undoRedoArea_CommandDone);
            // create viewer handler
            viewerHandler = new DViewerHandler();
            viewerHandler.MouseDown += new DMouseButtonEventHandler(viewerHandler_MouseDown);
            // create figure handler
            if (!usingEngineManager)
                undoRedoArea.Start("create figure handler");
            figureHandler = new DFigureHandler();
            figureHandler.SelectedFiguresChanged += new SelectedFiguresHandler(DoSelectedFiguresChanged);
            figureHandler.AddedFigure += delegate(DEngine de, Figure fig)
            {
                if (AddedFigure != null)
                    AddedFigure(this, fig);
            };
            if (!usingEngineManager)
            {
                undoRedoArea.Commit();
                undoRedoArea.ClearHistory();
            }
            // create state machine
            hsm = new DHsm(undoRedoArea, viewerHandler, figureHandler, authorProps);
            hsm.DebugMessage += new DebugMessageHandler(hsm_DebugMessage);
            hsm.ContextClick += new ContextClickHandler(hsm_ContextClick);
            hsm.DragFigureStart += new DragFigureHandler(hsm_DragFigureStart);
            hsm.DragFigureEvt += new DragFigureHandler(hsm_DragFigureEvt);
            hsm.DragFigureEnd += new DragFigureHandler(hsm_DragFigureEnd);
            hsm.MeasureRect += new SelectMeasureHandler(hsm_MeasureRect);
            hsm.StateChanged += new HsmStateChangedHandler(hsm_StateChanged);
            // update instance number
            instanceNumber++;
        }

        void viewerHandler_MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (MouseDown != null)
                MouseDown(dv, btn, pt);
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
            if (UndoRedoCommandDone != null)
                UndoRedoCommandDone(this, e);
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

        void hsm_DragFigureStart(DEngine de, Figure dragFigure, DPoint pt)
        {
            if (DragFigureStart != null)
                DragFigureStart(this, dragFigure, pt);
        }

        void hsm_DragFigureEvt(DEngine de, Figure dragFigure, DPoint pt)
        {
            if (DragFigureEvt != null)
                DragFigureEvt(this, dragFigure, pt);
        }

        void hsm_DragFigureEnd(DEngine de, Figure dragFigure, DPoint pt)
        {
            if (DragFigureEnd != null)
                DragFigureEnd(this, dragFigure, pt);
        }

        void hsm_MeasureRect(DEngine de, DRect rect)
        {
            if (MeasureRect != null)
                MeasureRect(this, rect);
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

        public bool CanDelete(IList<Figure> figs)
        {
            return figs.Count > 0;
        }

        public void Delete(IList<Figure> figs)
        {
            if (CanDelete(figs))
            {
                UndoRedoStart("Delete Figures");
                foreach (Figure f in figs)
                    RemoveFigure(f);
                UndoRedoCommit();
                ClearSelected();
                UpdateViewers();
            }
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

        public void HsmTextEdit(TextFigure tf)
        {
            hsm.ToTextEdit(tf);
        }

        public void ClearPage()
        {
            UndoRedoStart("Clear Page");
            figureHandler.Figures.Clear();
            UndoRedoCommit();
            DoSelectedFiguresChanged();
            UpdateViewers();
        }

        public void CancelFigureDrag()
        {
            hsm.CancelFigureDrag();
        }

        public void SelectAll()
        {
            figureHandler.Select(figureHandler.Figures);
            UpdateViewers();
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
