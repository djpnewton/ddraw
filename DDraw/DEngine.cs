using System;
using System.Collections.Generic;
using System.Text;

using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
#if DEBUG
    public delegate void DebugMessageHandler(string msg);
#endif
    public delegate void PageSizeChangedHandler(DEngine de, DPoint pageSize);
    public delegate void ClickHandler(DEngine de, Figure clickedFigure, DPoint pt);
    public delegate void DragFigureHandler(DEngine de, Figure dragFigure, DPoint pt);
    public delegate void SelectMeasureHandler(DEngine de, DRect rect);
    public delegate void AddedFigureHandler(DEngine de, Figure fig, bool fromHsm);

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
        double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; DoPropertyChanged(); }
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
        	fill = DColor.White;
        	stroke = DColor.Black;
        	strokeWidth = 1;
            strokeStyle = DStrokeStyle.Solid;
            startMarker = DMarker.None;
            endMarker = DMarker.None;
        	alpha = 1;
        	fontName = "Arial";
            fontSize = 14;
            bold = false;
            italics = false;
            underline = false;
            strikethrough = false;
        }

        public void SetProperties(DColor fill, DColor stroke, double strokeWidth, DStrokeStyle strokeStyle, 
            DMarker startMarker, DMarker endMarker, double alpha, string fontName, double fontSize, bool bold, 
            bool italics, bool underline, bool strikethrough)
        {
            this.fill = fill;
            this.stroke = stroke;
            this.strokeWidth = strokeWidth;
            this.strokeStyle = strokeStyle;
            this.startMarker = startMarker;
            this.endMarker = endMarker;
            this.alpha = alpha;
            this.fontName = fontName;
            this.fontSize = fontSize;
            this.bold = bold;
            this.italics = italics;
            this.underline = underline;
            this.strikethrough = strikethrough;
            DoPropertyChanged();
        }

        public void SetProperties(DAuthorProperties dap)
        {
            this.fill = dap.Fill;
            this.stroke = dap.Stroke;
            this.strokeWidth = dap.StrokeWidth;
            this.strokeStyle = dap.StrokeStyle;
            this.startMarker = dap.StartMarker;
            this.endMarker = dap.EndMarker;
            this.alpha = dap.Alpha;
            this.fontName = dap.FontName;
            this.fontSize = dap.FontSize;
            this.bold = dap.Bold;
            this.italics = dap.Italics;
            this.underline = dap.Underline;
            this.strikethrough = dap.Strikethrough;
            DoPropertyChanged();
        }

        public void SetProperties(Type figureClass, DAuthorProperties dap)
        {
            if (typeof(IFillable).IsAssignableFrom(figureClass))
                this.fill = dap.Fill;
            if (typeof(IStrokeable).IsAssignableFrom(figureClass))
            {
                this.stroke = dap.Stroke;
                this.strokeWidth = dap.StrokeWidth;
                this.strokeStyle = dap.StrokeStyle;
            }
            if (typeof(IMarkable).IsAssignableFrom(figureClass))
            {
                this.startMarker = dap.StartMarker;
                this.endMarker = dap.EndMarker;
            }
            if (typeof(IAlphaBlendable).IsAssignableFrom(figureClass))
                this.alpha = dap.Alpha;
            if (typeof(ITextable).IsAssignableFrom(figureClass))
            {
                this.fontName = dap.FontName;
                this.fontSize = dap.FontSize;
                this.bold = dap.Bold;
                this.italics = dap.Italics;
                this.underline = dap.Underline;
                this.strikethrough = dap.Strikethrough;
            }
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
                ((ITextable)f).FontSize = FontSize;
                ((ITextable)f).Bold = Bold;
                ((ITextable)f).Italics = Italics;
                ((ITextable)f).Underline = Underline;
                ((ITextable)f).Strikethrough = Strikethrough;
            }
        }

        public DAuthorProperties Clone()
        {
            DAuthorProperties dap = new DAuthorProperties();
            dap.SetProperties(this);
            return dap;
        }

        public static DAuthorProperties FromFigure(Figure f)
        {
            DAuthorProperties dap = new DAuthorProperties();
            if (f is IFillable)
                dap.Fill = ((IFillable)f).Fill;
            if (f is IStrokeable)
            {
                dap.Stroke = ((IStrokeable)f).Stroke;
                dap.StrokeWidth = ((IStrokeable)f).StrokeWidth;
                dap.StrokeStyle = ((IStrokeable)f).StrokeStyle;
            }
            if (f is IMarkable)
            {
                dap.StartMarker = ((IMarkable)f).StartMarker;
                dap.EndMarker = ((IMarkable)f).EndMarker;
            }
            if (f is IAlphaBlendable)
                dap.Alpha = ((IAlphaBlendable)f).Alpha;
            if (f is ITextable)
            {
                dap.FontName = ((ITextable)f).FontName;
                dap.FontSize = ((ITextable)f).FontSize;
                dap.Bold = ((ITextable)f).Bold;
                dap.Italics = ((ITextable)f).Italics;
                dap.Underline = ((ITextable)f).Underline;
                dap.Strikethrough = ((ITextable)f).Strikethrough;
            }
            return dap;
        }
    }

    public class DEngine
    {
        UndoRedoArea undoRedoArea;
        public UndoRedoArea UndoRedo
        {
            get { return undoRedoArea; }
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
        public DHsmLockAspectRatioMode FigureLockAspectRatioMode
        {
            get { return hsm.FigureLockAspectRatioMode; }
            set { hsm.FigureLockAspectRatioMode = value; }
        }
        public DHsmSnapAngleMode FigureSnapAngleMode
        {
            get { return hsm.FigureSnapAngleMode; }
            set { hsm.FigureSnapAngleMode = value; }
        }
        public bool FigureSelectToggleToSelection
        {
            get { return hsm.FigureSelectToggleToSelection; }
            set { hsm.FigureSelectToggleToSelection = value; }
        }
        public bool FiguresDeselectOnSingleClick
        {
            get { return hsm.FiguresDeselectOnSingleClick; }
            set { hsm.FiguresDeselectOnSingleClick = value; }
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

        public string SelectOperationName
        {
            get { return hsm.SelectOperationName; }
            set { hsm.SelectOperationName = value; }
        }
        public string AddLineName
        {
            get { return hsm.AddLineName; }
            set { hsm.AddLineName = value; }
        }
        public string AddTextName
        {
            get { return hsm.AddTextName; }
            set { hsm.AddTextName = value; }
        }
        public string AddName
        {
            get { return hsm.AddName; }
            set { hsm.AddName = value; }
        }
        public string TextEditName
        {
            get { return hsm.TextEditName; }
            set { hsm.TextEditName = value; }
        }
        public string FigureEditName
        {
            get { return hsm.FigureEditName; }
            set { hsm.FigureEditName = value; }
        }
        public string EraseOperationName
        {
            get { return hsm.EraseOperationName; }
            set { hsm.EraseOperationName = value; }
        }
        public string MoveName
        {
            get { return hsm.MoveName; }
            set { hsm.MoveName = value; }
        }

        UndoRedo<DPoint> _pageSize = new UndoRedo<DPoint>(PageTools.FormatToSize(PageFormat.Default));
        public DPoint PageSize
        {
            get { return _pageSize.Value; }
            set
            {
                if (!value.Equals(_pageSize.Value))
                    _pageSize.Value = value;
                if (PageSizeChanged != null)
                    PageSizeChanged(this, value);
                viewerHandler.SetPageSize(value);
                hsm.SetPageSize(value);
            }
        }       

        UndoRedo<string> _pageName = new UndoRedo<string>(null);
        public string PageName
        {
            get { return _pageName.Value; }
            set
            {
                if (!value.Equals(_pageName.Value))
                    _pageName.Value = value;
            }
        }
#if DEBUG
        public event DebugMessageHandler DebugMessage;
#endif
        public event SelectedFiguresHandler SelectedFiguresChanged;
        public event ClickHandler FigureClick;
        public event ClickHandler FigureContextClick;
        public event ClickHandler FigureLockClick;
        public event ClickHandler ContextClick;
        public event DragFigureHandler DragFigureStart;
        public event DragFigureHandler DragFigureEvt;
        public event DragFigureHandler DragFigureEnd;
        public event DMouseMoveEventHandler MouseMove;
        public event DMouseButtonEventHandler MouseDown;
        public event DMouseButtonEventHandler MouseUp;
        public event PageSizeChangedHandler PageSizeChanged;
        public event EventHandler UndoRedoChanged;
        public event EventHandler<CommandDoneEventArgs> UndoRedoCommandDone;
        public event HsmStateChangedHandler HsmStateChanged;
        public event AddedFigureHandler AddedFigure;
        public event SelectMeasureHandler MeasureRect;

        public event HsmTextHandler TextCut;
        public event HsmTextHandler TextCopy;

        public DEngine(UndoRedoArea area)
        {
            // setup undo/redo manager
            if (area == null)
                undoRedoArea = new UndoRedoArea("");
            else
                undoRedoArea = area;
            undoRedoArea.CommandDone += new EventHandler<CommandDoneEventArgs>(undoRedoArea_CommandDone);
            // create viewer handler
            viewerHandler = new DViewerHandler();
            viewerHandler.MouseMove += new DMouseMoveEventHandler(viewerHandler_MouseMove);
            viewerHandler.MouseDown += new DMouseButtonEventHandler(viewerHandler_MouseDown);
            viewerHandler.MouseUp += new DMouseButtonEventHandler(viewerHandler_MouseUp);
            // create figure handler
            figureHandler = new DFigureHandler();
            figureHandler.SelectedFiguresChanged += new SelectedFiguresHandler(DoSelectedFiguresChanged);
            figureHandler.AddedFigure += delegate(DEngine de, Figure fig, bool fromHsm)
            {
                if (AddedFigure != null)
                    AddedFigure(this, fig, fromHsm);
            };
            // create state machine
            hsm = new DHsm(undoRedoArea, viewerHandler, figureHandler);
#if DEBUG
            hsm.DebugMessage += new DebugMessageHandler(hsm_DebugMessage);
#endif
            hsm.FigureClick += new ClickHandler(hsm_FigureClick);
            hsm.FigureContextClick += new ClickHandler(hsm_FigureContextClick);
            hsm.FigureLockClick += new ClickHandler(hsm_FigureLockClick);
            hsm.ContextClick += new ClickHandler(hsm_ContextClick);
            hsm.DragFigureStart += new DragFigureHandler(hsm_DragFigureStart);
            hsm.DragFigureEvt += new DragFigureHandler(hsm_DragFigureEvt);
            hsm.DragFigureEnd += new DragFigureHandler(hsm_DragFigureEnd);
            hsm.MeasureRect += new SelectMeasureHandler(hsm_MeasureRect);
            hsm.StateChanged += new HsmStateChangedHandler(hsm_StateChanged);
            hsm.TextCut += new HsmTextHandler(hsm_TextCut);
            hsm.TextCopy += new HsmTextHandler(hsm_TextCopy);
        }

        void viewerHandler_MouseMove(DTkViewer dv, DPoint pt)
        {
            if (MouseMove != null)
                MouseMove(dv, pt);
        }

        void viewerHandler_MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (MouseDown != null)
                MouseDown(dv, btn, pt);
        }

        void viewerHandler_MouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (MouseUp != null)
                MouseUp(dv, btn, pt);
        }

        void undoRedoArea_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (e.CommandDoneType == CommandDoneType.Undo || e.CommandDoneType == CommandDoneType.Redo)
            {
                ClearSelected();
                UpdateViewers();
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

        void hsm_FigureClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (FigureClick != null)
                FigureClick(this, clickedFigure, pt);
        }

        void hsm_FigureContextClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (FigureContextClick != null)
                FigureContextClick(this, clickedFigure, pt);
        }

        void hsm_FigureLockClick(DEngine de, Figure clickedFigure, DPoint pt)
        {
            if (FigureLockClick != null)
                FigureLockClick(this, clickedFigure, pt);
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

#if DEBUG
        void hsm_DebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
#endif

        void hsm_TextCut(DEngine de, string text)
        {
            if (TextCut != null)
                TextCut(this, text);
        }

        void hsm_TextCopy(DEngine de, string text)
        {
            if (TextCopy != null)
                TextCopy(this, text);
        }

        // Public Functions //

        public void AddFigure(Figure f)
        {
            figureHandler.Add(f, false);
        }

        public void RemoveFigure(Figure f)
        {
            figureHandler.Remove(f);
        }

        public void SelectFigures(IList<Figure> figs)
        {
            figureHandler.SelectFigures(figs, false);
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
                // make group
                figureHandler.GroupFigures(figs);
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
                // perform ungroup
                figureHandler.UngroupFigures(figs);
                // update all viewers
                UpdateViewers();
            }
        }

        public void SendToBack(List<Figure> figs)
        {
            // send to back
            figureHandler.SendToBack(figs);
            // update viewers
            UpdateViewers();
        }

        public void BringToFront(List<Figure> figs)
        {
            // bring to front
            figureHandler.BringToFront(figs);
            // update viewers
            UpdateViewers();
        }

        public void SendBackward(List<Figure> figs)
        {            
            // send backward
            figureHandler.SendBackward(figs);
            // update
            UpdateViewers();
        }

        public bool CanSendBackward(List<Figure> figs)
        {
            return figureHandler.CanSendBackward(figs);
        }

        public void BringForward(List<Figure> figs)
        {
            // bring forward
            figureHandler.BringForward(figs);
            // update viewers
            UpdateViewers();
        }

        public bool CanBringForward(List<Figure> figs)
        {
            return figureHandler.CanBringForward(figs);
        }

        public string Cut(List<Figure> figs, out DBitmap bmp, bool bmpAntiAlias)
        {
            if (CanCopy(figs))
            {
                string data = Copy(figs, out bmp, bmpAntiAlias, DColor.White);
                foreach (Figure f in figs)
                    RemoveFigure(f);
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

        public string Copy(List<Figure> figs, out DBitmap bmp, bool bmpAntiAlias, DColor bmpBackgroundColor)
        {
            bmp = FigureSerialize.FormatToBmp(figs, bmpAntiAlias, bmpBackgroundColor);
            return FigureSerialize.FormatToXml(figs, null);
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
                foreach (Figure f in figs)
                    RemoveFigure(f);
                ClearSelected();
                UpdateViewers();
            }
        }

        public bool CustomBackgroundFigure
        {
            get { return figureHandler.CustomBackgroundFigure; }
        }

        public BackgroundFigure BackgroundFigure
        {
            get { return figureHandler.BackgroundFigure; }
        }

        public void SetBackgroundFigure(BackgroundFigure f, bool isCustom)
        {
            f.Rect = new DRect(0, 0, PageSize.X, PageSize.Y);
            figureHandler.BackgroundFigure = f;
            viewerHandler.Update();
            figureHandler.CustomBackgroundFigure = isCustom;
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
            figureHandler.Figures.Clear();
            DoSelectedFiguresChanged();
            UpdateViewers();
        }

        public void CancelFigureDrag()
        {
            hsm.CancelFigureDrag();
        }

        public void SelectAll()
        {
            figureHandler.SelectFigures(figureHandler.Figures, false);
            UpdateViewers();
        }

        public void CheckState()
        {
            hsm.CheckState();
        }

        public void CutText()
        {
            hsm.CutText();
            UpdateViewers();
        }

        public void CopyText()
        {
            hsm.CopyText();
        }

        public void PasteText(string text)
        {
            hsm.PasteText(text);
            UpdateViewers();
        }

        // Other //

#if DEBUG
       void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
#endif

        void DoSelectedFiguresChanged()
        {
            if (SelectedFiguresChanged != null)
                SelectedFiguresChanged();
        }
    }
}
