using System;
using System.Collections.Generic;
using System.Text;

using qf4net;
using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public enum DHsmSignals : int
    {
        //enum values must start at UserSig value or greater
        GSelect = QSignals.UserSig, GSelectMeasure, GDrawLine, GDrawText, GDrawRect, GEraser, 
        GCancelFigureDrag, GTextEdit, GCheckState, GCutText, GCopyText, GPasteText,
        TextEdit, FigureEdit,
        MouseDown, MouseMove, MouseUp, DoubleClick,
        KeyDown, KeyPress, KeyUp
    }

    public class QViewerEvent : QEvent
    {
        DTkViewer dv;
        public DTkViewer Dv
        {
            get { return dv; }
        }

        public QViewerEvent(int qSignal, DTkViewer dv) : base(qSignal)
        {
            this.dv = dv;
        }
    }

    public class QMouseEvent : QViewerEvent
    {
        DMouseButton button;
        public DMouseButton Button
        {
            get { return button; }
        }
        DPoint pt;
        public DPoint Pt
        {
            get { return pt; } 
        }

        public QMouseEvent(int qSignal, DTkViewer dv, DMouseButton button, DPoint pt) : base(qSignal, dv)
        {
            this.button = button;
            this.pt = pt;
        }
    }

    public class QKeyEvent : QViewerEvent
    {
        DKey key;
        public DKey Key
        {
            get { return key; }
        }

        public QKeyEvent(int qSignal, DTkViewer dv, DKey key) : base(qSignal, dv)
        {
            this.key = key;
        }
    }

    public class QKeyPressEvent : QViewerEvent
    {
        int key;
        public int Key
        {
            get { return key; }
        }

        public QKeyPressEvent(int qSignal, DTkViewer dv, int key) : base(qSignal, dv)
        {
            this.key = key;
        }
    }

    public class QGDrawFigureEvent : QEvent
    {
        Type figureClass;
        public Type FigureClass
        {
            get { return figureClass; }
        }

        public QGDrawFigureEvent(int qSignal, Type figureClass) : base(qSignal)
        {
            this.figureClass = figureClass;
        }
    }

    public class QGTextEditEvent : QEvent
    {
        TextFigure tf;
        public TextFigure TextFigure
        {
            get { return tf; }
        }

        public QGTextEditEvent(int qSignal, TextFigure tf) : base(qSignal)
        {
            this.tf = tf;
        }
    }

    public class QGPasteTextEvent : QEvent
    {
        string text;
        public string Text
        {
            get { return text; }
        }
        public QGPasteTextEvent(int qSignal, string text)
            : base(qSignal)
        {
            this.text = text;
        }
    }

    public enum DHsmState { Select, SelectMeasure, DrawLine, DrawText, TextEdit, DrawRect, FigureEdit, Eraser };
    public enum DHsmSnapAngleMode { Default, Always, Never };
    public enum DHsmLockAspectRatioMode { Default, Always, Never };

    public delegate void HsmStateChangedHandler(DEngine de, DHsmState state);
    public delegate void HsmTextHandler(DEngine de, string text);
    public class DHsm : QHsm
    {
        // private variables
        UndoRedoArea undoRedoArea;
        DViewerHandler viewerHandler;
        DFigureHandler figureHandler;

        Type currentFigureClass = null;
        Figure currentFigure = null;
        DPoint dragPt;
        double dragRot;
        DHitTest mouseHitTest;
        bool cancelledFigureDrag;
        DKey textEditKey;
        bool textEditMouseDown, textEditChangeTextWrap, textEditMove;
        double textEditDragWrapOffset;

        bool lockInitialAspectRatio = false;
        double unlockInitalAspectRatioThreshold = 50;

        const double figureSnapAngle = Math.PI / 4;        // 45 degrees
        const double figureSnapRange = Math.PI / (4 * 18); // 2.5  degrees (each way)

        Figure autoGroupPolylineFigure = null;
        bool autoGroupPolylineTimeoutMet;
        bool autoGroupPolylineXLimitMet;
        bool autoGroupPolylineYLimitMet;
        int autoGroupPolylineStart;

        // properties
        public Type CurrentFigureClass
        {
            get { return currentFigureClass; }
        }

        DHsmLockAspectRatioMode figureLockAspectRatioMode = DHsmLockAspectRatioMode.Default;
        public DHsmLockAspectRatioMode FigureLockAspectRatioMode
        {
            get { return figureLockAspectRatioMode; }
            set { figureLockAspectRatioMode = value; }
        }
        public bool LockingAspectRatio
        {
            get { return lockInitialAspectRatio || (figureLockAspectRatioMode == DHsmLockAspectRatioMode.Always); }
        }

        DHsmSnapAngleMode figureSnapAngleMode = DHsmSnapAngleMode.Default;
        public DHsmSnapAngleMode FigureSnapAngleMode
        {
            get { return figureSnapAngleMode; }
            set { figureSnapAngleMode = value; }
        }

        bool figureSelectToggleToSelection = false;
        public bool FigureSelectToggleToSelection
        {
            get { return figureSelectToggleToSelection; }
            set { figureSelectToggleToSelection = value; }
        }

        bool figuresDeselectOnSingleClick = true;
        public bool FiguresDeselectOnSingleClick
        {
            get { return figuresDeselectOnSingleClick; }
            set { figuresDeselectOnSingleClick = value; }
        }

        bool simplifyPolylines = false;
        public bool SimplifyPolylines
        {
            get { return simplifyPolylines; }
            set { simplifyPolylines = value; }
        }
        double simplifyPolylinesTolerance = 5;
        public double SimplifyPolylinesTolerance
        {
            get { return simplifyPolylinesTolerance; }
            set { simplifyPolylinesTolerance = value; }
        }

        bool autoGroupPolylines = false;
        public bool AutoGroupPolylines
        {
            get { return autoGroupPolylines; }
            set { autoGroupPolylines = value; }
        }
        int autoGroupPolylinesTimeout = 1000;
        public int AutoGroupPolylinesTimeout
        {
            get { return autoGroupPolylinesTimeout; }
            set { autoGroupPolylinesTimeout = value; }
        }
        int autoGroupPolylinesXLimit = 100;
        public int AutoGroupPolylinesXLimit
        {
            get { return autoGroupPolylinesXLimit; }
            set { autoGroupPolylinesXLimit = value; }
        }
        int autoGroupPolylinesYLimit = 50;
        public int AutoGroupPolylinesYLimit
        {
            get { return autoGroupPolylinesYLimit; }
            set { autoGroupPolylinesYLimit = value; }
        }

        bool usePolylineDots = true;
        public bool UsePolylineDots
        {
            get { return usePolylineDots; }
            set { usePolylineDots = value; }
        }

        int keyMovementRate = 1;
        public int KeyMovementRate
        {
            get { return keyMovementRate; }
            set { keyMovementRate = value; }
        }

        DPoint pageSize = PageTools.FormatToSize(PageFormat.Default);
        public void SetPageSize(DPoint pageSize)
        {
            this.pageSize = pageSize;
        }
        bool figuresBoundToPage = false;
        public bool FiguresBoundToPage
        {
            get { return figuresBoundToPage; }
            set { figuresBoundToPage = value; }
        }

        string selectOperationName = "Select Operation";
        public string SelectOperationName
        {
            get { return selectOperationName; }
            set { selectOperationName = value; }
        }
        string addLineName = "addLine";
        public string AddLineName
        {
            get { return addLineName; }
            set { addLineName = value; }
        }
        string addTextName = "Add Text";
        public string AddTextName
        {
            get { return addTextName; }
            set { addTextName = value; }
        }
        string addName = "Add";
        public string AddName
        {
            get { return addName; }
            set { addName = value; }
        }
        string textEditName = "Text Edit";
        public string TextEditName
        {
            get { return textEditName; }
            set { textEditName = value; }
        }
        string figureEditName = "Figure Edit";
        public string FigureEditName
        {
            get { return figureEditName; }
            set { figureEditName = value; }
        }
        string eraseOperationName = "Erase Operation";
        public string EraseOperationName
        {
            get { return eraseOperationName; }
            set { eraseOperationName = value; }
        }
        string moveName = "Move";
        public string MoveName
        {
            get { return moveName; }
            set { moveName = value; }
        }

        double grid = 20;
        public double Grid
        {
            get { return grid; }
            set { grid = value; }
        }
        bool gridSnapPosition = true;
        public bool GridSnapPosition
        {
            get { return gridSnapPosition; }
            set { gridSnapPosition = value; }
        }
        bool gridSnapResize = true;
        public bool GridSnapResize
        {
            get { return gridSnapResize; }
            set { gridSnapResize = value; }
        }
        bool gridSnapLines = true;
        public bool GridSnapLines
        {
            get { return gridSnapLines; }
            set { gridSnapLines = value; }
        }

        bool drawSelection = false;
        bool drawEraser = false;

        public event HsmStateChangedHandler StateChanged;
        public DHsmState State
        {
            get
            {
                if (IsInState(Select))
                    return DHsmState.Select;
                if (IsInState(SelectMeasure))
                    return DHsmState.SelectMeasure;
                if (IsInState(DrawLine))
                    return DHsmState.DrawLine;
                if (IsInState(DrawText))
                    return DHsmState.DrawText;
                if (IsInState(TextEdit))
                    return DHsmState.TextEdit;
                if (IsInState(DrawRect))
                    return DHsmState.DrawRect;
                if (IsInState(FigureEdit))
                    return DHsmState.FigureEdit;
                if (IsInState(Eraser))
                    return DHsmState.Eraser;
                System.Diagnostics.Debug.Assert(false, "Logic Error :(");
                return DHsmState.Select;
            }
            set
            {
                switch (value)
                {
                    case DHsmState.Select:
                        Dispatch(new QEvent((int)DHsmSignals.GSelect));
                        break;
                    case DHsmState.SelectMeasure:
                        Dispatch(new QEvent((int)DHsmSignals.GSelectMeasure));
                        break;
                    case DHsmState.Eraser:
                        Dispatch(new QEvent((int)DHsmSignals.GEraser));
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set directly to '{0}' :(", value));
                        break;
                }
            }
        }

#if DEBUG
        public event DebugMessageHandler DebugMessage;
#endif
        public event ClickHandler FigureClick;
        public event ClickHandler FigureContextClick;
        public event ClickHandler FigureLockClick;
        public event ClickHandler ContextClick;
        public event DragFigureHandler DragFigureStart;
        public event DragFigureHandler DragFigureEvt;
        public event DragFigureHandler DragFigureEnd;
        public event SelectMeasureHandler MeasureRect;

        public event HsmTextHandler TextCut;
        public event HsmTextHandler TextCopy;

        // state variables (showing state hierachy here)
        QState Main;
            QState Select;
                QState SelectDefault;
                QState DragFigure;
                QState DragSelect;
            QState SelectMeasure;
                QState SelectMeasureDefault;
                QState SelectMeasuring;
            QState DrawLine;
                QState DrawLineDefault;
                QState DrawingLine;
            QState DrawText;
            QState TextEdit;
            QState DrawRect;
                QState DrawRectDefault;
                QState DrawingRect;
            QState FigureEdit;
            QState Eraser;
                QState EraserDefault;
                QState Erasing;

        public DHsm(UndoRedoArea undoRedoArea, DViewerHandler viewerHandler, DFigureHandler figureHandler) : base()
        {
            // undo redo manager
            this.undoRedoArea = undoRedoArea;
            // viewer handler
            this.viewerHandler = viewerHandler;
            viewerHandler.NeedRepaint += new DPaintEventHandler(dv_NeedRepaint);
            viewerHandler.MouseDown += new DMouseButtonEventHandler(dv_MouseDown);
            viewerHandler.MouseMove += new DMouseMoveEventHandler(dv_MouseMove);
            viewerHandler.MouseUp += new DMouseButtonEventHandler(dv_MouseUp);
            viewerHandler.DoubleClick += new DMouseMoveEventHandler(dv_DoubleClick);
            viewerHandler.KeyDown += new DKeyEventHandler(dv_KeyDown);
            viewerHandler.KeyPress += new DKeyPressEventHandler(dv_KeyPress);
            viewerHandler.KeyUp += new DKeyEventHandler(dv_KeyUp);
            // figure handler
            this.figureHandler = figureHandler;
            // QHsm Init
            Init();
        }

        // viewer events

        void dv_NeedRepaint(DTkViewer dv,DGraphics dg)
        {
            Figure[] controlFigures = null;
            if (drawSelection || drawEraser)
            {
                controlFigures = new Figure[0];
                if (drawSelection)
                {
                    Array.Resize(ref controlFigures, controlFigures.Length + 1);
                    controlFigures[controlFigures.Length - 1] = figureHandler.SelectionFigure;
                }
                if (drawEraser)
                {
                    Array.Resize(ref controlFigures, controlFigures.Length + 1);
                    controlFigures[controlFigures.Length - 1] = figureHandler.EraserFigure;
                }
            }
            dv.Paint(dg, figureHandler.BackgroundFigure, figureHandler.Figures, controlFigures);
        }

        void dv_MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DHsmSignals.MouseDown, dv, btn, pt));
        }

        void dv_MouseMove(DTkViewer dv, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DHsmSignals.MouseMove, dv, DMouseButton.NotApplicable, pt));
        }

        void dv_MouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DHsmSignals.MouseUp, dv, btn, pt));
        }

        void dv_DoubleClick(DTkViewer dv, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DHsmSignals.DoubleClick, dv, DMouseButton.NotApplicable, pt));
        }

        void dv_KeyDown(DTkViewer dv, DKey k)
        {
            Dispatch(new QKeyEvent((int)DHsmSignals.KeyDown, dv, k));
        }

        void dv_KeyPress(DTkViewer dv, int k)
        {
            Dispatch(new QKeyPressEvent((int)DHsmSignals.KeyPress, dv, k));
        }

        void dv_KeyUp(DTkViewer dv, DKey k)
        {
            Dispatch(new QKeyEvent((int)DHsmSignals.KeyUp, dv, k));
        }

        // public methods

        public void SetStateByFigureClass(Type figureClass)
        {
            if (figureClass.Equals(typeof(TextFigure))) // do TextFigure first as it is inherits from RectbaseFigure
                Dispatch(new QGDrawFigureEvent((int)DHsmSignals.GDrawText, figureClass));
            else if (figureClass.IsSubclassOf(typeof(RectbaseFigure)))
                Dispatch(new QGDrawFigureEvent((int)DHsmSignals.GDrawRect, figureClass));
            else if (figureClass.IsSubclassOf(typeof(LinebaseFigure)))
                Dispatch(new QGDrawFigureEvent((int)DHsmSignals.GDrawLine, figureClass));
            else
                System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set state using '{0}' :(", figureClass.Name));
        }

        public void ToTextEdit(TextFigure tf)
        {
            Dispatch(new QGTextEditEvent((int)DHsmSignals.GTextEdit, tf));
        }

        public bool CurrentFigClassImpls(Type _interface)
        {
            if (currentFigureClass != null)
                return _interface.IsAssignableFrom(currentFigureClass);
            else
                return false;
        }

        public bool CurrentFigClassIs(Type _class)
        {
            if (currentFigureClass != null)
                return _class.Equals(currentFigureClass);
            else
                return false;
        }

        public void CancelFigureDrag()
        {
            Dispatch(new QEvent((int)DHsmSignals.GCancelFigureDrag));
        }

        public void CheckState()
        {
            Dispatch(new QEvent((int)DHsmSignals.GCheckState));
        }

        public void CutText()
        {
            Dispatch(new QEvent((int)DHsmSignals.GCutText));
        }

        public void CopyText()
        {
            Dispatch(new QEvent((int)DHsmSignals.GCopyText));
        }

        public void PasteText(string text)
        {
            Dispatch(new QGPasteTextEvent((int)DHsmSignals.GPasteText, text));
        }

        // private methods

        void CommitOrRollback(bool dontAllowZeroArea)
        {
            // roll back if current figure has zero areasize
            if (dontAllowZeroArea && (currentFigure.Width == 0 || currentFigure.Height == 0))
                undoRedoArea.Cancel();
            else
                // commit to undo/redo
                undoRedoArea.Commit();
            // null currentfigure
            currentFigure = null;
        }

        void BoundPtToPage(DPoint pt)
        {
            if (figuresBoundToPage)
            {
                if (pt.X < 0)
                    pt.X = 0;
                else if (pt.X > pageSize.X)
                    pt.X = pageSize.X;
                if (pt.Y < 0)
                    pt.Y = 0;
                else if (pt.Y > pageSize.Y)
                    pt.Y = pageSize.Y;
            }
        }

        DPoint CalcDragDelta(DPoint pt)
        {
            return new DPoint(pt.X - dragPt.X, pt.Y - dragPt.Y);
        }

        DPoint CalcSizeDelta(DPoint pt, Figure f, bool lockAspectRatio)
        {
            if (lockAspectRatio && f.Width > 0)
            {
                pt = pt.Offset(-dragPt.X, -dragPt.Y);
                double m = f.Height / f.Width;
                DPoint intersectionPt = DGeom.IntersectionOfTwoLines(m, f.BottomRight, -1, pt);
                // using soh/cah/toa
                double h = DGeom.DistBetweenTwoPts(f.BottomRight, intersectionPt);
                double A = Math.Atan(m);
                // cos(A) = a/h
                double dX = Math.Cos(A) * h;
                // sin(A) = o/h
                double dY = Math.Sin(A) * h;
                // find out the angle of the line between the mouse pt and the botton left of the figure
                double angle = -(Math.Atan2(pt.Y - f.BottomRight.Y, pt.X - f.BottomRight.X) - Math.PI);
                // if the angle is on the topleft side (225 deg to 45 deg) then we are resizing smaller
                if (angle < Math.PI / 4 || angle > Math.PI + Math.PI / 4)
                    return new DPoint(-dX, -dY);
                else
                    return new DPoint(dX, dY);
            }
            else
                return new DPoint((pt.X - f.X) - f.Width - dragPt.X, (pt.Y - f.Y) - f.Height - dragPt.Y);
        }

        DRect GetBoundingBox(Figure f)
        {
            return DGeom.BoundingBoxOfRotatedRect(f.GetEncompassingRect(), f.Rotation, f.Rect.Center);
        }

        double GetRotationOfPointComparedToFigure(Figure f, DPoint pt)
        {
            return DGeom.AngleBetweenPoints(f.GetSelectRect().Center, pt);
        }

#if DEBUG
        void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }
#endif

        // state methods

        QState DoMain(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(Select);
                    return null;
                case (int)DHsmSignals.GSelect:
                    TransitionTo(Select);
                    return null;
                case (int)DHsmSignals.GSelectMeasure:
                    TransitionTo(SelectMeasure);
                    return null;
                case (int)DHsmSignals.GDrawLine:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawLine);
                    return null;
                case (int)DHsmSignals.GDrawText:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawText);
                    return null;
                case (int)DHsmSignals.GDrawRect:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawRect);
                    return null;
                case (int)DHsmSignals.GEraser:
                    TransitionTo(Eraser);
                    return null;
                case (int)DHsmSignals.GTextEdit:
                    currentFigure = ((QGTextEditEvent)qevent).TextFigure;
                    TransitionTo(TextEdit);
                    return null;
            }
            return this.TopState;
        }

        QState DoSelect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(SelectDefault);
                    return null;
                case (int)QSignals.Entry:
                    // clear currentFigureClass
                    currentFigureClass = null;
                    viewerHandler.Update();
                    return null;
                case (int)DHsmSignals.GCheckState:
                    TransitionTo(SelectDefault);
                    return null;
            }
            return this.Main;
        }

        void DoSelectDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                // find and select clicked figure
                List<Figure> children = new List<Figure>();
                IGlyph glyph;
                Figure f = figureHandler.HitTestSelect(pt, out mouseHitTest, children, out glyph, figureSelectToggleToSelection);
                // update selected figures
                if (glyph != null)
                {

                }
                else if (f != null)
                {
                    // set drag infomation
                    currentFigure = f;
                    switch (mouseHitTest)
                    {
                        case DHitTest.Body:
                            // store drag point
                            dragPt = pt;
                            if (!f.Locked)
                            {
                                // drag figure start event
                                if (DragFigureStart != null)
                                    DragFigureStart(null, f, dv.EngineToClient(pt));
                            }
                            break;
                        case DHitTest.SelectRect:
                            goto case DHitTest.Body;
                        case DHitTest.Resize:
                            dragPt = new DPoint(0, 0);
                            dragPt = CalcSizeDelta(f.RotatePointToFigure(pt), f, LockingAspectRatio || f.LockAspectRatio);
                            lockInitialAspectRatio = figureLockAspectRatioMode == DHsmLockAspectRatioMode.Default;
                            break;
                        case DHitTest.RepositionPoint:
                            break;
                        case DHitTest.Rotate:
                            dragRot = GetRotationOfPointComparedToFigure(f, pt) - f.Rotation;
                            if (dragRot > Math.PI)
                                dragRot = dragRot - (Math.PI * 2);
                            break;
                        case DHitTest.Context:
                            if (FigureContextClick != null)
                                FigureContextClick(null, f, dv.EngineToClient(pt));
                            break;
                        case DHitTest.Lock:
                            if (FigureLockClick != null)
                                FigureLockClick(null, f, dv.EngineToClient(pt));
                            break;
                    }
                }
                else
                {
                    if (!figureSelectToggleToSelection)
                        figureHandler.ClearSelected();
                    dragPt = pt; // mouseHitTest = DHitTest.None
                    // transition
                    TransitionTo(DragSelect);
                }
                // update drawing
                dv.Update();
            }
        }

        void DoSelectDefaultMouseMove(DTkViewer dv, DPoint pt)
        {
            // set cursor
            DHitTest hitTest;
            List<Figure> children = new List<Figure>();
            IGlyph glyph;
            Figure f = figureHandler.HitTestFigures(pt, out hitTest, children, out glyph);
            // set mouseover
            if (f != null && !f.MouseOver)
            {
                f.MouseOver = true;
                dv.Update(GetBoundingBox(f));
            }
            foreach (Figure otherFig in figureHandler.Figures)
                if (otherFig != f && otherFig.MouseOver)
                {
                    otherFig.MouseOver = false;
                    dv.Update(GetBoundingBox(otherFig));
                }
            // set cursor
            switch (hitTest)
            {
                case DHitTest.None:
                    dv.SetCursor(DCursor.Default);
                    break;
                case DHitTest.Body:
                    if (f.Locked)
                        dv.SetCursor(DCursor.Default);
                    else
                        dv.SetCursor(DCursor.MoveAll);
                    if (f.ClickEvent)
                        dv.SetCursor(DCursor.Hand);
                    else
                        foreach (Figure child in children)
                            if (child.ClickEvent)
                            {
                                dv.SetCursor(DCursor.Hand);
                                break;
                            }
                    break;
                case DHitTest.SelectRect:
                    goto case DHitTest.Body;
                case DHitTest.Resize:
                    dv.SetCursor(DCursor.MoveNWSE);
                    break;
                case DHitTest.RepositionPoint:
                    goto case DHitTest.Resize;
                case DHitTest.Rotate:
                    dv.SetCursor(DCursor.Rotate);
                    break;
                case DHitTest.Context:
                    dv.SetCursor(DCursor.Hand);
                    break;
                case DHitTest.Lock:
                    goto case DHitTest.Context;
                case DHitTest.Glyph:
                    dv.SetCursor(glyph.Cursor);
                    break;
            }
#if DEBUG
            DoDebugMessage(string.Format("{0}, {1}", pt.X, pt.Y));
#endif
        }

        void DoSelectDefaultMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            DHitTest hitTest;
            List<Figure> children = new List<Figure>();
            IGlyph glyph;
            if (btn == DMouseButton.Left)
            {
                Figure f = figureHandler.HitTestFigures(pt, out hitTest, children, out glyph);
                if (hitTest == DHitTest.Glyph)
                {
                    if (children.Count > 0)
                        glyph.CallClicked(children[0], dv.EngineToClient(pt));
                    else
                        glyph.CallClicked(f, dv.EngineToClient(pt));
                }
                else if ((hitTest == DHitTest.Body || hitTest == DHitTest.SelectRect) && FigureClick != null)
                {
                    foreach (Figure child in children)
                        if (child.ClickEvent)
                            FigureClick(null, child, dv.EngineToClient(pt));
                    if (f.ClickEvent)
                        FigureClick(null, f, dv.EngineToClient(pt));
                    if (FiguresDeselectOnSingleClick && !FigureSelectToggleToSelection)
                    {
                        figureHandler.SelectFigures(new List<Figure>(new Figure[] { f }), false);
                        dv.Update();
                    }
                }
            }
            else if (btn == DMouseButton.Right)
            {
                Figure f = figureHandler.HitTestSelect(pt, out hitTest, null, out glyph, figureSelectToggleToSelection);
                dv.SetCursor(DCursor.Default);
                dv.Update();
                if (ContextClick != null)
                    ContextClick(null, f, dv.EngineToClient(pt));
            }
            // nullify current figure for DoSelectDefault -> DHsmSignals.MouseMove:
            currentFigure = null;
        }

        void DoSelectDefaultDoubleClick(DTkViewer dv, DPoint pt)
        {
            DHitTest ht;
            IGlyph glyph;
            Figure f = figureHandler.HitTestFigures(pt, out ht, null, out glyph);
            if (f is ITextable)
            {
                currentFigure = f;
                TransitionTo(TextEdit);
            }
            else if (f is IEditable)
            {
                currentFigure = f;
                TransitionTo(FigureEdit);
            }
        }

        void DoSelectDefaultKeyPress(DTkViewer dv, int k)
        {
            int dx = 0, dy = 0;
            switch (k)
            {
                case (int)DKeys.Left:
                    dx = -keyMovementRate;
                    break;
                case (int)DKeys.Right:
                    dx = keyMovementRate;
                    break;
                case (int)DKeys.Up:
                    dy = -keyMovementRate;
                    break;
                case (int)DKeys.Down:
                    dy = keyMovementRate;
                    break;
            }
            if (dx != 0 || dy != 0)
            {
                undoRedoArea.Start(moveName, this);
                foreach (Figure f in figureHandler.SelectedFigures)
                {
                    f.X += dx;
                    f.Y += dy;
                }
                undoRedoArea.Commit();
                dv.Update();
            }
        }

        QState DoSelectDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseDown:
                    DoSelectDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    if (currentFigure != null && !currentFigure.Locked &&
                        (((QMouseEvent)qevent).Pt.X != dragPt.X || ((QMouseEvent)qevent).Pt.Y != dragPt.Y))
                    {
                        TransitionTo(DragFigure);
                        DoDragFigureMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    }
                    else
                        DoSelectDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoSelectDefaultMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.DoubleClick:
                    DoSelectDefaultDoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.KeyPress:
                    DoSelectDefaultKeyPress(((QKeyPressEvent)qevent).Dv, ((QKeyPressEvent)qevent).Key);
                    return null;
            }
            return this.Select;
        }

        DPoint GridSnapOffset(double x, double y)
        {
            if (grid > 0)
            {
                double rX = x % grid;
                if (rX > grid / 2)
                    rX = grid - rX;
                else
                    rX = -rX;
                double rY = y % grid;
                if (rY > grid / 2)
                    rY = grid - rY;
                else
                    rY = -rY;
                return new DPoint(rX, rY);
            }
            return new DPoint(0, 0);
        }

        delegate void SetPointDelegate(DPoint pt);
        delegate double GetRotationalSnapDelegate(double ar);
        void DoDragFigureMouseMove(DTkViewer dv, DPoint pt)
        {
            // rectangular area to update with paint event
            DRect updateRect = new DRect();
            // move selected figures
            switch (mouseHitTest)
            {
                case DHitTest.Body:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // drag figure event
                    if (DragFigureEvt != null)
                        DragFigureEvt(null, currentFigure, dv.EngineToClient(pt));
                    // if figure drag op is cancelled then quit this function
                    if (cancelledFigureDrag)
                    {
                        cancelledFigureDrag = false;
                        return;
                    }
                    // bound pt to canvas
                    BoundPtToPage(pt);
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    foreach (Figure f in figureHandler.SelectedFigures)
                        updateRect = updateRect.Union(GetBoundingBox(f));
                    // apply x/y delta to figures
                    DPoint dPos = CalcDragDelta(pt);
                    if (gridSnapPosition && grid > 0)
                    {
                        DPoint o = GridSnapOffset(currentFigure.X + dPos.X, currentFigure.Y + dPos.Y);
                        dPos.X += o.X;
                        dPos.Y += o.Y;
                        pt.X += o.X;
                        pt.Y += o.Y;
                    }
                    if (dPos.X != 0 || dPos.Y != 0)
                        foreach (Figure f in figureHandler.SelectedFigures)
                            if (!f.Locked)
                            {
                                f.X += dPos.X;
                                f.Y += dPos.Y;
                            }
                    // store drag pt for reference later (eg. next mousemove event)
                    dragPt = pt;
                    // final update rect
                    foreach (Figure f in figureHandler.SelectedFigures)
                        updateRect = updateRect.Union(GetBoundingBox(f));
                    break;
                case DHitTest.SelectRect:
                    goto case DHitTest.Body;
                case DHitTest.Resize:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // bound pt to canvas
                    BoundPtToPage(pt);
                    // alert figure we are going to resize it
                    currentFigure.BeforeResize();
                    // inital update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // translate point onto the same rotated plane as the figure
                    pt = currentFigure.RotatePointToFigure(pt);
                    // apply width/height delta to figure
                    DPoint dSize = CalcSizeDelta(pt, currentFigure, LockingAspectRatio || currentFigure.LockAspectRatio);
                    if (lockInitialAspectRatio && !(figureLockAspectRatioMode == DHsmLockAspectRatioMode.Always || currentFigure.LockAspectRatio))
                    {
                        DPoint dSizeUnlocked = CalcSizeDelta(pt, currentFigure, false);
                        if (figureLockAspectRatioMode == DHsmLockAspectRatioMode.Never ||
                            Math.Abs(dSizeUnlocked.X - dSize.X) >= unlockInitalAspectRatioThreshold ||
                            Math.Abs(dSizeUnlocked.Y - dSize.Y) >= unlockInitalAspectRatioThreshold)
                        {
                             lockInitialAspectRatio = false;
                            dSize = dSizeUnlocked;
                        }
                    }
                    if (currentFigure.Width > 0 && currentFigure.Width + dSize.X < currentFigure.MinSize)
                    {
                        dSize.X = currentFigure.MinSize - currentFigure.Width;
                        if (LockingAspectRatio || currentFigure.LockAspectRatio)
                            dSize.Y = (currentFigure.Height / currentFigure.Width) * dSize.X;
                    }
                    if (currentFigure.Height > 0 && currentFigure.Height + dSize.Y < currentFigure.MinSize)
                    {
                        dSize.Y = currentFigure.MinSize - currentFigure.Height;
                        if (LockingAspectRatio || currentFigure.LockAspectRatio)
                            dSize.X = (currentFigure.Width / currentFigure.Height) * dSize.Y;
                    }
                    DRect oldRect = currentFigure.Rect;
                    currentFigure.Width += dSize.X;
                    currentFigure.Height += dSize.Y;
                    // snap resize
                    if (gridSnapResize && grid > 0)
                    {
                        DPoint o2 = GridSnapOffset(currentFigure.Width, currentFigure.Height);
                        currentFigure.Width += o2.X;
                        currentFigure.Height += o2.Y;
                    }
                    DGeom.UpdateRotationPosition(currentFigure, oldRect, currentFigure.Rect); 
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    // alert figure we have finished resizing
                    currentFigure.AfterResize();
                    // debug message
#if DEBUG
                    DoDebugMessage(string.Format("{0} {1}", dSize.X, dSize.Y));
#endif
                    break;
                case DHitTest.RepositionPoint:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // bound pt to canvas
                    BoundPtToPage(pt);
                    // inital update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // get our reposition point interface
                    IRepositionPoint rp = (IRepositionPoint)currentFigure;
                    // setup points
                    DPoint newPoint;
                    newPoint = new DPoint(pt.X, pt.Y);
                    SetPointDelegate setPoint = delegate(DPoint point)
                    {
                        // snap point to grid
                        if (gridSnapLines && grid > 0)
                        {
                            DPoint o3 = GridSnapOffset(point.X, point.Y);
                            point.X += o3.X;
                            point.Y += o3.Y;
                        }
                        // now set point
                        rp.RepositionPoint = point;
                    };
                    GetRotationalSnapDelegate getRotationalSnap = delegate(double angleRemainder)
                    {
                        if (angleRemainder < figureSnapRange)
                            return -angleRemainder;
                        else if (angleRemainder > figureSnapAngle - figureSnapRange)
                            return figureSnapAngle - angleRemainder;
                        else
                            return 0;
                    };
                    if (rp.AnglePoint != null)
                    {
                        // find the current angle of the line and the remainder when divided by the snap angle
                        double currentAngle = DGeom.AngleBetweenPoints(rp.RepositionPoint, rp.AnglePoint);
                        double ar = currentAngle % figureSnapAngle;
                        // reposition line
                        double newAngle;
                        switch (FigureSnapAngleMode)
                        {
                            case DHsmSnapAngleMode.Always:
                                // slide point along snap angle
                                newAngle = DGeom.AngleBetweenPoints(newPoint, rp.AnglePoint);
                                ar = newAngle % figureSnapAngle;
                                if (ar < figureSnapAngle / 2)
                                    setPoint(DGeom.RotatePoint(newPoint, rp.AnglePoint, -ar));
                                else
                                    setPoint(DGeom.RotatePoint(newPoint, rp.AnglePoint, figureSnapAngle - ar));
                                break;
                            case DHsmSnapAngleMode.Default:
                                if (ar == 0)
                                {
                                    // line is snapped, test if new angle will unsnap the line
                                    newAngle = DGeom.AngleBetweenPoints(newPoint, rp.AnglePoint);
                                    ar = newAngle % figureSnapAngle;
                                    if (ar > figureSnapRange && ar < figureSnapAngle - figureSnapRange)
                                        // unsnapped, set new point
                                        setPoint(newPoint);
                                    else
                                    {
                                        // slide point along snap angle
                                        newPoint = DGeom.RotatePoint(newPoint, rp.AnglePoint, getRotationalSnap(ar));
                                        setPoint(newPoint);
                                    }
                                }
                                else
                                {
                                    // set new point
                                    setPoint(newPoint);
                                    // test whether to snap our line
                                    newAngle = DGeom.AngleBetweenPoints(newPoint, rp.AnglePoint);
                                    ar = newAngle % figureSnapAngle;
                                    double rotationalSnap = getRotationalSnap(ar);
                                    // snap it
                                    if (rotationalSnap != 0)
                                        setPoint(DGeom.RotatePoint(newPoint, rp.AnglePoint, rotationalSnap));
                                }
                                break;
                            case DHsmSnapAngleMode.Never:
                                // set new point
                                setPoint(newPoint);
                                break;
                        }
                    }
                    else
                        setPoint(newPoint);
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    break;
                case DHitTest.Rotate:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // apply rotation to figure
                    double newRot = GetRotationOfPointComparedToFigure(currentFigure, pt) - dragRot;
                    double r = newRot % figureSnapAngle;
                    switch (figureSnapAngleMode)
                    {
                        case DHsmSnapAngleMode.Always:
                            if (r < figureSnapAngle / 2)
                                currentFigure.Rotation = newRot - r;
                            else
                                currentFigure.Rotation = newRot + figureSnapAngle - r;
                            break;
                        case DHsmSnapAngleMode.Default:
                            if (r < figureSnapRange)
                                currentFigure.Rotation = newRot - r;
                            else if (r > figureSnapAngle - figureSnapRange)
                                currentFigure.Rotation = newRot + figureSnapAngle - r;
                            else
                                currentFigure.Rotation = newRot;
                            break;
                        case DHsmSnapAngleMode.Never:
                            currentFigure.Rotation = newRot;
                            break;
                    }                      
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
#if DEBUG
                    // debug message
                    DoDebugMessage((currentFigure.Rotation * 180 / Math.PI).ToString());
#endif
                    break;
            }
            // update drawing
            dv.Update(updateRect);
        }

        void DoDragFigureMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                // drag figure end event
                if ((mouseHitTest == DHitTest.Body || mouseHitTest == DHitTest.SelectRect) && 
                    !currentFigure.Locked && DragFigureEnd != null)
                    DragFigureEnd(null, currentFigure, dv.EngineToClient(pt));
                // nullify currentFigure
                currentFigure = null;
                // transition
                TransitionTo(SelectDefault);
            }
        }

        QState DoDragFigure(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // record state for undo/redo manager
                    undoRedoArea.Start(selectOperationName);
                    return null;
                case (int)QSignals.Exit:
                    // commit undo changes
                    if (undoRedoArea.IsCommandStarted)
                        undoRedoArea.Commit();
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDragFigureMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoDragFigureMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.GCancelFigureDrag:
                    // tell DoDragFigureMouseMove to quit
                    cancelledFigureDrag = true;
                    // nullify currentFigure
                    currentFigure = null;
                    // cancel changes
                    undoRedoArea.Cancel();
                    // transition
                    TransitionTo(SelectDefault);
                    return null;
            }
            return this.Select;
        }

        void DoDragSelectMouseMove(DTkViewer dv, DPoint pt)
        {
            // rectangular area to update with paint event
            DRect updateRect = new DRect();
            // initial update rect
            updateRect = figureHandler.SelectionFigure.Rect;
            // drag select figure
            figureHandler.SelectionFigure.TopLeft = dragPt;
            figureHandler.SelectionFigure.BottomRight = pt;
            if (figureHandler.SelectionFigure.Width < 0)
            {
                figureHandler.SelectionFigure.X += figureHandler.SelectionFigure.Width;
                figureHandler.SelectionFigure.Width = -figureHandler.SelectionFigure.Width;
            }
            if (figureHandler.SelectionFigure.Height < 0)
            {
                figureHandler.SelectionFigure.Y += figureHandler.SelectionFigure.Height;
                figureHandler.SelectionFigure.Height = -figureHandler.SelectionFigure.Height;
            }
            // final update rect
            updateRect = updateRect.Union(figureHandler.SelectionFigure.Rect);
            // update drawing
            dv.Update(updateRect);
        }

        QState DoDragSelect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // move selection figure out of view
                    figureHandler.SelectionFigure.Left = int.MinValue;
                    figureHandler.SelectionFigure.Width = 0;
                    // enable selection figure drawing
                    drawSelection = true;
                    return null;
                case (int)QSignals.Exit:
                    drawSelection = false;
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDragSelectMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DRect updateRect2 = figureHandler.SelectionFigure.Rect;
                    List<Figure> selectFigs = new List<Figure>();
                    foreach (Figure f in figureHandler.Figures)
                        if (DGeom.PointInRect(f.Rect.Center, figureHandler.SelectionFigure.Rect))
                            selectFigs.Add(f);
                    figureHandler.SelectFigures(selectFigs, figureSelectToggleToSelection);
                    foreach (Figure f in selectFigs)
                        updateRect2 = updateRect2.Union(GetBoundingBox(f));
                    // update drawing
                    ((QMouseEvent)qevent).Dv.Update(updateRect2);
                    // transition back
                    TransitionTo(SelectDefault);
                    return null;
            }
            return this.Select;
        }

        QState DoSelectMeasure(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // clear currentFigureClass
                    currentFigureClass = null;
                    // clear selected
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)QSignals.Init:
                    InitializeState(SelectMeasureDefault);
                    return null;
            }
            return this.Main;
        }

        QState DoSelectMeasureDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseMove:
                    ((QMouseEvent)qevent).Dv.SetCursor(DCursor.Crosshair);
                    return null;
                case (int)DHsmSignals.MouseDown:
                    // store drag point
                    dragPt = ((QMouseEvent)qevent).Pt;
                    // show the selection figure
                    drawSelection = true;
                    // transistion
                    TransitionTo(SelectMeasuring);
                    return null;
            }
            return this.SelectMeasure;
        }

        QState DoSelectMeasuring(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseMove:
                    DoDragSelectMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    // hide the selection figure
                    drawSelection = false;
                    ((QMouseEvent)qevent).Dv.Update();
                    // transition
                    TransitionTo(SelectMeasureDefault);
                    // call SelectMeasure event
                    if (MeasureRect != null)
                        MeasureRect(null, figureHandler.SelectionFigure.Rect);
                    break;
            }
            return this.SelectMeasure;
        }

        QState DoDrawLine(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(DrawLineDefault);
                    return null;
                case (int)QSignals.Entry:
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)DHsmSignals.GCheckState:
                    // commit to undo/redo
                    if (IsInState(DrawingLine))
                    CommitOrRollback(false);
                    // transition
                    TransitionTo(DrawLineDefault);
                    // nullify autoGroupPolylineFigure
                    autoGroupPolylineFigure = null;
                    return null;
            }
            return this.Main;
        }

        void DoDrawLineDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {                
                undoRedoArea.Start(addLineName);
                // bound pt to canvas
                BoundPtToPage(pt);
                // create line figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                if (currentFigure is IPolyline)
                {
                    ((IPolyline)currentFigure).AddPoint(pt);
                    if (!(currentFigure is IRepositionPoint))
                    {
                        // auto grouping stuff
                        if (AutoGroupPolylines)
                        {
                            if (autoGroupPolylineFigure == null)
                            {
                                autoGroupPolylineFigure = currentFigure;
                                autoGroupPolylineTimeoutMet = false;
                            }
                            else
                            {
                                if (Environment.TickCount <= autoGroupPolylineStart + AutoGroupPolylinesTimeout)
                                    autoGroupPolylineTimeoutMet = true;
                                else
                                {
                                    autoGroupPolylineTimeoutMet = false;
                                    autoGroupPolylineFigure = currentFigure;
                                }
                            }
                        }
                    }
                    else
                        ((IPolyline)currentFigure).AddPoint(pt);
                }
                // add to list of figures
                figureHandler.Add(currentFigure, true);
                // transition
                TransitionTo(DrawingLine);
            }
            else if (btn == DMouseButton.Right)
                TransitionTo(Select);
        }

        void DoDrawLineDefaultMouseMove(DTkViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawLineDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseDown:
                    DoDrawLineDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDrawLineDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawLine;
        }

        void DoDrawingLineMouseMove(DTkViewer dv, DPoint pt)
        {
            // initial update rect
            DRect updateRect = currentFigure.GetSelectRect();
            // bound pt to canvas
            BoundPtToPage(pt);
            // add point
            if (currentFigure is IPolyline)
            {
                if (!(currentFigure is IRepositionPoint))
                    ((IPolyline)currentFigure).AddPoint(pt);
                else
                    ((IPolyline)currentFigure).LastPoint = pt;
            }
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingLineMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (currentFigure is IPolyline)
            {
                if (!(currentFigure is IRepositionPoint))
                {
                    // test for finished line
                    bool lineNotFinished = false;
                    if (((IPolyline)currentFigure).Points.Count < 2)
                    {
                        if (UsePolylineDots)
                        {
                            DPoint currentPt = ((IPolyline)currentFigure).Points[0];
                            DPoint newPt = new DPoint(currentPt.X + 0.01, currentPt.Y + 0.01);
                            ((IPolyline)currentFigure).Points.Add(newPt);
                        }
                        else
                            lineNotFinished = true;
                    }
                    if (!lineNotFinished)
                    {
                        // simplify polyline
                        if (SimplifyPolylines && currentFigure is IPolyline)
                        {
                            ((IPolyline)currentFigure).Points = DGeom.SimplifyPolyline(((IPolyline)currentFigure).Points, simplifyPolylinesTolerance);
                            dv.Update(currentFigure.GetSelectRect());
                        }
                        // auto group
                        if (autoGroupPolylineTimeoutMet)
                        {
                            autoGroupPolylineXLimitMet = DGeom.DistXBetweenRects(autoGroupPolylineFigure.GetEncompassingRect(),
                                currentFigure.GetEncompassingRect()) < autoGroupPolylinesXLimit;
                            autoGroupPolylineYLimitMet = DGeom.DistYBetweenRects(autoGroupPolylineFigure.GetEncompassingRect(),
                                currentFigure.GetEncompassingRect()) < autoGroupPolylinesYLimit;
                            if (autoGroupPolylineXLimitMet && autoGroupPolylineYLimitMet)
                            {
                                if (autoGroupPolylineFigure is GroupFigure)
                                {
                                    figureHandler.Remove(currentFigure);
                                    IChildFigureable cf = (IChildFigureable)autoGroupPolylineFigure;
                                    cf.ChildFigures.Add(currentFigure);
                                    cf.ChildFigures = cf.ChildFigures;
                                }
                                else if (autoGroupPolylineFigure is IPolyline)
                                {
                                    figureHandler.Remove(autoGroupPolylineFigure);
                                    figureHandler.Remove(currentFigure);
                                    GroupFigure gf = new GroupFigure(new List<Figure>(new Figure[] { autoGroupPolylineFigure, currentFigure }));
                                    figureHandler.Add(gf, false);
                                    autoGroupPolylineFigure = gf;
                                }
                            }
                            else
                                autoGroupPolylineFigure = currentFigure;
                        }
                    }
                    autoGroupPolylineStart = Environment.TickCount;
                }
            }
            // commit to undo/redo
            CommitOrRollback(false);
            // transition
            TransitionTo(DrawLineDefault);
        }

        QState DoDrawingLine(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseMove:
                    DoDrawingLineMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoDrawingLineMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.KeyDown:
                    if (((QKeyEvent)qevent).Key.Ctrl && 
                        currentFigure is IRepositionPoint && 
                        currentFigure is IPolyline)
                    {
                        ((IPolyline)currentFigure).AddPoint(((IPolyline)currentFigure).LastPoint);
                        ((QKeyEvent)qevent).Dv.Update(currentFigure.GetSelectRect());
                    };
                    return null;
            }
            return this.DrawLine;
        }

        void DoDrawTextMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                undoRedoArea.Start(addTextName);
                // bound pt to canvas
                BoundPtToPage(pt);
                // create TextFigure
                currentFigure = new TextFigure(pt, "", 0);
                // add to list of figures
                figureHandler.Add(currentFigure, true);
                // update DViewer
                dv.Update();
                // transition
                TransitionTo(TextEdit);
            }
        }

        void DoDrawTextMouseMove(DTkViewer dv, DPoint pt)
        {

        }

        void DoDrawTextMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {

        }

        QState DoDrawText(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)DHsmSignals.MouseDown:
                    DoDrawTextMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDrawTextMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoDrawTextMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Main;
        }

        void DoTextEditMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                TextEditFigure tef = (TextEditFigure)currentFigure;
                IGlyph glyph;
                if (currentFigure.HitTest(pt, null, out glyph) == DHitTest.Body)
                {
                    textEditMouseDown = true;
                    textEditChangeTextWrap = tef.HitTestTextWrapHandle(pt);
                    textEditMove = tef.HitTestBorder(pt);
                    if (textEditChangeTextWrap)
                        textEditDragWrapOffset = tef.X + tef.BorderWidth + tef.WrapLength - pt.X;
                    if (textEditMove)
                        dragPt = pt;
                    else
                    {
                        tef.SetCursorPoint(pt, false);
                        dv.Update(currentFigure.Rect);
                    }
                }
                else
                {
                    // find and select clicked figure
                    Figure f = figureHandler.HitTestSelect(pt, out mouseHitTest, null, out glyph, false);
                    // select the TextFigure from the TextEditFigure
                    if (f == tef)
                    {
                        if (tef.HasText)
                        {
                            f = tef.Figure;
                            figureHandler.SelectFigures(new List<Figure>(new Figure[] { f }), false);
                        }
                    }
                    // setup for select mouse move
                    dragPt = pt;
                    // transition to select state
                    TransitionTo(DragFigure);
                    currentFigure = f;
                }
            }
            else if (btn == DMouseButton.Right)
                TransitionTo(Select);
        }

        void DoTextEditMouseMove(DTkViewer dv, DPoint pt)
        {
            TextEditFigure tef = (TextEditFigure)currentFigure;
            if (textEditMouseDown)
            {
                if (textEditChangeTextWrap)
                {
                    dv.Update(GetBoundingBox(tef));
                    if (!tef.WrapText)
                    {
                        tef.WrapFontSize = tef.FontSize;
                        tef.WrapThreshold = tef.Width - tef.BorderWidth * 2;
                        tef.WrapText = true;
                        textEditDragWrapOffset = tef.X + tef.BorderWidth + tef.WrapLength - pt.X;
                    }
                    const int threshBase = 200;
                    tef.WrapThreshold = threshBase * ((pt.X - (tef.X + tef.BorderWidth) + textEditDragWrapOffset) /
                        (threshBase * tef.FontSize / tef.WrapFontSize));
                    dv.Update(GetBoundingBox(tef));
                }
                else if (textEditMove)
                {
                    // bound pt to canvas
                    BoundPtToPage(pt);
                    // initial update rect
                    DRect updateRect = GetBoundingBox(tef);
                    // move text edit figure
                    tef.X += pt.X - dragPt.X;
                    tef.Y += pt.Y - dragPt.Y;
                    dragPt = pt;
                    // update
                    dv.Update(updateRect.Union(GetBoundingBox(tef)));
                }
                else
                {
                    tef.SetCursorPoint(pt, true);
                    dv.Update(currentFigure.Rect);
                }
            }
            else
            {
                IGlyph glyph;
                if (currentFigure.HitTest(pt, null, out glyph) == DHitTest.Body)
                {
                    if (tef.HitTestTextWrapHandle(pt))
                        dv.SetCursor(DCursor.MoveWE);
                    else if (tef.HitTestBorder(pt))
                        dv.SetCursor(DCursor.MoveAll);
                    else
                        dv.SetCursor(DCursor.IBeam);
                }
                else
                    DoSelectDefaultMouseMove(dv, pt);
            }
        }

        void DoTextEditMouseUp(DTkViewer dv, DMouseButton b, DPoint pt)
        {
            textEditMouseDown = false;
        }

        void DoTextEditDoubleClick(DTkViewer dv, DMouseButton b, DPoint pt)
        {
            ((TextEditFigure)currentFigure).DoubleClick(pt);
            dv.Update(currentFigure.Rect);
        }

        void DoTextEditKeyDown(DTkViewer dv, DKey k)
        {
            textEditKey = k;
        }

        void DoTextEditKeyPress(DTkViewer dv, int k)
        {
            if (currentFigure != null && currentFigure is TextEditFigure)
            {
                TextEditFigure te = (TextEditFigure)currentFigure;
                DRect updateRect = te.Rect;
                switch ((DKeys)k)
                {
                    case DKeys.Backspace:
                        te.BackspaceAtCursor();
                        break;
                    case DKeys.Enter:
                        te.InsertAtCursor('\n');
                        break;
                    case DKeys.Escape:
                        TransitionTo(SelectDefault);
                        return;
                    case DKeys.Delete:
                        te.DeleteAtCursor();
                        break;
                    case DKeys.Left:
                        te.MoveCursor((DKeys)k, textEditKey.Ctrl, textEditKey.Shift);
                        break;
                    case DKeys.Right: goto case DKeys.Left;
                    case DKeys.Up: goto case DKeys.Left;
                    case DKeys.Down: goto case DKeys.Left;
                    case DKeys.Home: goto case DKeys.Left;
                    case DKeys.End: goto case DKeys.Left;
                    case DKeys.PageUp: break;
                    case DKeys.PageDown: break;
                    default:
                        te.InsertAtCursor((char)k);
                        break;
                }
                dv.Update(updateRect.Union(te.Rect));
            }
        }

        void DoTextEditKeyUp(DTkViewer dv, DKey k)
        {
            textEditKey = k;
        }

        QState DoTextEdit(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // unmouseover figure
                    currentFigure.MouseOver = false;
                    // reset textEditKey
                    textEditKey = new DKey();
                    // reset textEditMouseDown
                    textEditMouseDown = false;
                    // start undo record
                    if (!undoRedoArea.IsCommandStarted) // can be started from other means
                        undoRedoArea.Start(textEditName);
                    // add TextEditFigure
                    Figure tf = currentFigure;
                    currentFigure = new TextEditFigure(tf, (ITextable)tf);
                    figureHandler.Insert(currentFigure, tf);
                    figureHandler.Remove(tf);
                    // update view
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)QSignals.Exit:
                    // replace text edit figure with the textfigure
                    if (currentFigure is TextEditFigure)
                    {
                        TextEditFigure tef = (TextEditFigure)currentFigure;
                        if (tef.HasText || !(tef.Figure is TextFigure))
                            figureHandler.Insert(tef.Figure, currentFigure);
                        figureHandler.Remove(currentFigure);
                        // nullify currentfigure
                        currentFigure = null;
                    }
                    // record text edit to undo manager
                    undoRedoArea.Commit();
                    return null;
                case (int)DHsmSignals.MouseDown:
                    DoTextEditMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoTextEditMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoTextEditMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.DoubleClick:
                    DoTextEditDoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.KeyDown:
                    DoTextEditKeyDown(((QKeyEvent)qevent).Dv, ((QKeyEvent)qevent).Key);
                    return null;
                case (int)DHsmSignals.KeyPress:
                    DoTextEditKeyPress(((QKeyPressEvent)qevent).Dv, ((QKeyPressEvent)qevent).Key);
                    return null;
                case (int)DHsmSignals.KeyUp:
                    DoTextEditKeyUp(((QKeyEvent)qevent).Dv, ((QKeyEvent)qevent).Key);
                    return null;
                case (int)DHsmSignals.GCutText:
                    string text = ((TextEditFigure)currentFigure).Cut();
                    if (TextCut != null)
                        TextCut(null, text);
                    return null;
                case (int)DHsmSignals.GCopyText:
                    string text2 = ((TextEditFigure)currentFigure).Copy();
                    if (TextCopy != null)
                        TextCopy(null, text2);
                    return null;
                case (int)DHsmSignals.GPasteText:
                    ((TextEditFigure)currentFigure).Paste(((QGPasteTextEvent)qevent).Text);
                    return null;
                case (int)DHsmSignals.GCheckState:
                    TransitionTo(Select);
                    return null;
            }
            return this.Main;
        }

        QState DoDrawRect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(DrawRectDefault);
                    return null;
                case (int)QSignals.Entry:
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)DHsmSignals.GCheckState:
                    // commit to undo/redo
                    if (IsInState(DrawingRect))
                        CommitOrRollback(false);
                    // transition
                    TransitionTo(DrawRectDefault);
                    return null;
            }
            return this.Main;
        }

        void DoDrawRectDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                undoRedoArea.Start(string.Format("{0} {1}", addName, currentFigureClass.Name));
                // bound pt to canvas
                BoundPtToPage(pt);
                // create Figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                currentFigure.TopLeft = pt;
                // add to list of figures
                figureHandler.Add(currentFigure, true);
                // store drag pt for reference on mousemove event)
                dragPt = pt;
                // transition
                TransitionTo(DrawingRect);
            }
            else if (btn == DMouseButton.Right)
                TransitionTo(Select);
        }

        void DoDrawRectDefaultMouseMove(DTkViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawRectDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseDown:
                    DoDrawRectDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDrawRectDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawRect;
        }

        void DoDrawingRectMouseMove(DTkViewer dv, DPoint pt)
        {
            // initial update rect
            DRect updateRect = currentFigure.GetSelectRect();
            // bound pt to canvas
            BoundPtToPage(pt);
            // change dimensions
            if (pt.X >= dragPt.X)
                currentFigure.Right = pt.X;
            else
            {
                currentFigure.Left = pt.X;
                currentFigure.Right = dragPt.X;
            }
            if ((figureLockAspectRatioMode == DHsmLockAspectRatioMode.Always) || currentFigure.LockAspectRatio)
            {
                currentFigure.Height = currentFigure.Width;
                if (pt.Y >= dragPt.Y)
                    currentFigure.Top = dragPt.Y;
                else
                    currentFigure.Top = dragPt.Y - currentFigure.Height;
            }
            else
            {
                if (pt.Y >= dragPt.Y)
                    currentFigure.Bottom = pt.Y;
                else
                {
                    currentFigure.Top = pt.Y;
                    currentFigure.Bottom = dragPt.Y;
                }
            }
            // set selection rectangle
            figureHandler.SelectionFigure.Rect = currentFigure.GetSelectRect();
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingRectMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            CommitOrRollback(true);
            // transition
            TransitionTo(DrawRectDefault);
            // update drawing
            dv.Update(figureHandler.SelectionFigure.Rect);
        }

        QState DoDrawingRect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;
                case (int)QSignals.Exit:
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoDrawingRectMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoDrawingRectMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawRect;
        }
        
        void currentFigure_EditFinished(IEditable sender)
        {
            // tranistion
            TransitionTo(Select);
        }
        
        QState DoFigureEdit(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // unmouseover figure
                    currentFigure.MouseOver = false;
                    // start undo record
                    undoRedoArea.Start(figureEditName);
                    // set editing and connect to edit finished event
                    ((IEditable)currentFigure).StartEdit();
                    ((IEditable)currentFigure).EditFinished += new EditFinishedHandler(currentFigure_EditFinished);
                    // update view
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    return null;
                case (int)QSignals.Exit:
                    // not editing any more
                    ((IEditable)currentFigure).EndEdit();
                    ((IEditable)currentFigure).EditFinished -= currentFigure_EditFinished;                
                    // record figure edit to undo manager
                    undoRedoArea.Commit();
                    // clear current figure
                    currentFigure = null;
                    return null;
                case (int)DHsmSignals.MouseDown:
                    if (((QMouseEvent)qevent).Button == DMouseButton.Left)
                        ((IEditable)currentFigure).MouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    else if (((QMouseEvent)qevent).Button == DMouseButton.Right)
                        TransitionTo(Select);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    ((IEditable)currentFigure).MouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    ((IEditable)currentFigure).MouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.DoubleClick:
                    ((IEditable)currentFigure).DoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.KeyPress:
                    ((IEditable)currentFigure).KeyPress(((QKeyPressEvent)qevent).Dv, ((QKeyPressEvent)qevent).Key);
                    return null;
                case (int)DHsmSignals.GCheckState:
                    TransitionTo(Select);
                    return null;
            }
            return this.Main;
        }

        QState DoEraser(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(EraserDefault);
                    return null;
                case (int)QSignals.Entry:
                    // clear currentFigureClass
                    currentFigureClass = null;
                    // clear selected
                    figureHandler.ClearSelected();
                    // update
                    viewerHandler.Update();
                    return null;
                case (int)DHsmSignals.GCheckState:
                    TransitionTo(EraserDefault);
                    return null;
            }
            return this.Main;
        }

        QState DoEraserDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseDown:
                    if (((QMouseEvent)qevent).Button == DMouseButton.Left)
                        TransitionTo(Erasing);
                    else if (((QMouseEvent)qevent).Button == DMouseButton.Right)
                        TransitionTo(Select);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    ((QMouseEvent)qevent).Dv.SetCursor(DCursor.Crosshair);
                    return null;
            }
            return this.Eraser;
        }

        enum LineSegPos { Start, Middle, End, All };
        void ClonePolyProps(Figure from, Figure to, LineSegPos linepos)
        {
            to.Rotation = from.Rotation;
            DGeom.UpdateRotationPosition(to, from.Rect, to.Rect);
            if (from.GetType() == to.GetType())
            {
                if (from is IFillable)
                    ((IFillable)to).Fill = ((IFillable)from).Fill;
                if (from is IStrokeable)
                {
                    ((IStrokeable)to).Stroke = ((IStrokeable)from).Stroke;
                    ((IStrokeable)to).StrokeWidth = ((IStrokeable)from).StrokeWidth;
                    ((IStrokeable)to).StrokeStyle = ((IStrokeable)from).StrokeStyle;
                }
                if (from is IMarkable)
                {
                    if (linepos == LineSegPos.Start || linepos == LineSegPos.All)
                        ((IMarkable)to).StartMarker = ((IMarkable)from).StartMarker;
                    if (linepos == LineSegPos.End || linepos == LineSegPos.All)
                        ((IMarkable)to).EndMarker = ((IMarkable)from).EndMarker;
                }
                if (from is IAlphaBlendable)
                    ((IAlphaBlendable)to).Alpha = ((IAlphaBlendable)from).Alpha;
                if (from is ITextable)
                {
                    ((ITextable)to).FontName = ((ITextable)from).FontName;
                    ((ITextable)to).Bold = ((ITextable)from).Bold;
                    ((ITextable)to).Italics = ((ITextable)from).Italics;
                    ((ITextable)to).Underline = ((ITextable)from).Underline;
                    ((ITextable)to).Strikethrough = ((ITextable)from).Strikethrough;
                }
            }
        }

        delegate void Method();
        void ErasePolyline(DPoints ptsToRemove, PolylinebaseFigure f, GroupFigure parent)
        {
            // set figure list to use
            UndoRedoList<Figure> figs;
            if (parent != null)
                figs = parent.ChildFigures;
            else
                figs = figureHandler.Figures;
            // make new polylines
            List<Figure> newPolys = new List<Figure>();
            DPoints newPts = null;
            Method createNewPoly = delegate()
            {
                if (newPts != null && newPts.Count >= 2)
                {
                    PolylinebaseFigure nf = (PolylinebaseFigure)Activator.CreateInstance(f.GetType());
                    nf.Points = newPts;
                    newPolys.Add(nf);
                }
                newPts = null;
            };
            foreach (DPoint pt in f.Points)
            {
                if (!ptsToRemove.Contains(pt))
                {
                    if (newPts == null)
                        newPts = new DPoints();
                    newPts.Add(new DPoint(pt.X, pt.Y));
                }
                else
                    createNewPoly();
            }
            createNewPoly();
            // apply old poly properties to new polys and add to "figs" figure list
            for (int i = 0; i < newPolys.Count; i++)
            {
                Figure nf = newPolys[i];
                if (newPolys.Count == 1)
                    ClonePolyProps(f, nf, LineSegPos.All);
                else if (i == 0)
                    ClonePolyProps(f, nf, LineSegPos.Start);
                else if (i == newPolys.Count - 1)
                    ClonePolyProps(f, nf, LineSegPos.End);
                else
                    ClonePolyProps(f, nf, LineSegPos.Middle);
                // add poly to figs
                figs.Insert(figs.IndexOf(f), nf);
            }
            // remove original polyline
            figs.Remove(f);
            // reset GroupFigure parent figure list so its scalling boxes are reset
            if (parent != null)
                parent.ChildFigures = figs;
        }

        void ErasePolylines(DPoint eraserPt, IList<Figure> figures, ref DRect updateRect, GroupFigure parent)
        {
            for (int i = figures.Count - 1; i >= 0; i--)
                if (figures[i] is PolylinebaseFigure)
                {
                    PolylinebaseFigure f = (PolylinebaseFigure)figures[i];
                    DPoint rotPt = f.TransformPointToFigure(eraserPt);
                    DPoints ptsToRemove = new DPoints();
                    if (f.Points != null)
                        foreach (DPoint pt in f.Points)
                            if (DGeom.PointInCircle(pt, rotPt, figureHandler.EraserFigure.Size / 2))
                                ptsToRemove.Add(pt);
                    if (ptsToRemove.Count > 0)
                    {
                        // add polyline figure bounding box to updateRect
                        updateRect = updateRect.Union(GetBoundingBox(f));
                        if (parent != null)
                        {
                            // remove child figure and account for rect changes
                            DRect oldR = parent.Rect;
                            ErasePolyline(ptsToRemove, f, parent);
                            DRect newR = parent.Rect;
                            DGeom.UpdateRotationPosition(f, oldR, newR);
                        }
                        else
                            ErasePolyline(ptsToRemove, f, parent);
                    }
                }
                else if (figures[i] is GroupFigure)
                {
                    GroupFigure f = (GroupFigure)figures[i];
                    // recurse into group figure and also update rotation position
                    DRect oldR = f.Rect;
                    ErasePolylines(f.TransformPointToFigure(eraserPt), f.ChildFigures, ref updateRect, f);
                    DRect newR = f.Rect;
                    DGeom.UpdateRotationPosition(f, oldR, newR);
                    // clean up group figure if no longer needed
                    if (f.ChildFigures.Count == 1)
                        figureHandler.UngroupFigure(f);
                    else if (f.ChildFigures.Count < 1)
                        figures.Remove(f);
                }
        }

        QState DoErasing(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // record state for undo/redo manager
                    undoRedoArea.Start(eraseOperationName);
                    return null;
                case (int)QSignals.Exit:
                    // commit undo changes
                    undoRedoArea.Commit();
                    // hide eraser
                    drawEraser = false;
                    viewerHandler.Update(); // and show updated polylines in other viewers too
                    return null;
                case (int)DHsmSignals.MouseMove:
                    QMouseEvent me = (QMouseEvent)qevent;
                    DRect updateRect = GetBoundingBox(figureHandler.EraserFigure);
                    // show & move eraser
                    drawEraser = true;
                    figureHandler.EraserFigure.TopLeft = new DPoint(me.Pt.X - figureHandler.EraserFigure.Size / 2, me.Pt.Y - figureHandler.EraserFigure.Size / 2);
                    // erase stuff
                    ErasePolylines(me.Pt, figureHandler.Figures, ref updateRect, null);
                    // update
                    me.Dv.Update(updateRect.Union(GetBoundingBox(figureHandler.EraserFigure)));
                    return null;
                case (int)DHsmSignals.MouseUp:
                    // transition
                    TransitionTo(EraserDefault);
                    return null;
            }
            return this.Eraser;
        }

        protected override void InitializeStateMachine()
		{
            Main = new QState(this.DoMain);
            Select = new QState(this.DoSelect);
            SelectDefault = new QState(this.DoSelectDefault);
            DragFigure = new QState(this.DoDragFigure);
            DragSelect = new QState(this.DoDragSelect);
            SelectMeasure = new QState(this.DoSelectMeasure);
            SelectMeasureDefault = new QState(this.DoSelectMeasureDefault);
            SelectMeasuring = new QState(this.DoSelectMeasuring);
            DrawLine = new QState(this.DoDrawLine);
            DrawLineDefault = new QState(this.DoDrawLineDefault);
            DrawingLine = new QState(this.DoDrawingLine);
            DrawText = new QState(this.DoDrawText);
            TextEdit = new QState(this.DoTextEdit);
            DrawRect = new QState(this.DoDrawRect);
            DrawRectDefault = new QState(this.DoDrawRectDefault);
            DrawingRect = new QState(this.DoDrawingRect);
            FigureEdit = new QState(this.DoFigureEdit);
            Eraser = new QState(this.DoEraser);
            EraserDefault = new QState(this.DoEraserDefault);
            Erasing = new QState(this.DoErasing);
            InitializeState(Main); // initial transition			
		}

        new protected void TransitionTo(QState targetState)
        {
            base.TransitionTo(targetState);
            if (StateChanged != null)
                StateChanged(null, State);
        }
    }
}
