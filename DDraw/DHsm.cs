using System;
using System.Collections.Generic;
using System.Text;

using qf4net;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public enum DHsmSignals : int
    {
        //enum values must start at UserSig value or greater
        GSelect = QSignals.UserSig, GDrawLine, GDrawText, GDrawRect, GEraser,
        TextEdit, FigureEdit,
        MouseDown, MouseMove, MouseUp, DoubleClick,
        KeyDown, KeyPress, KeyUp
    }

    public class QMouseEvent : QEvent
    {
        DTkViewer dv;
        public DTkViewer Dv
        {
            get { return dv; }
        }
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

        public QMouseEvent(int qSignal, DTkViewer dv, DMouseButton button, DPoint pt) : base(qSignal)
        {
            this.dv = dv;
            this.button = button;
            this.pt = pt;
        }
    }

    public class QKeyEvent : QEvent
    {
        DViewer dv;
        public DViewer Dv
        {
            get { return dv; }
        }
        DKey key;
        public DKey Key
        {
            get { return key; }
        }

        public QKeyEvent(int qSignal, DViewer dv, DKey key) : base(qSignal)
        {
            this.dv = dv;
            this.key = key;
        }
    }

    public class QKeyPressEvent : QEvent
    {
        DTkViewer dv;
        public DTkViewer Dv
        {
            get { return dv; }
        }
        int key;
        public int Key
        {
            get { return key; }
        }

        public QKeyPressEvent(int qSignal, DTkViewer dv, int key)
            : base(qSignal)
        {
            this.dv = dv;
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

    public enum DHsmState { Select, DrawLine, DrawText, TextEdit, DrawRect, FigureEdit, Eraser };

    public delegate void HsmStateChangedHandler(DEngine de, DHsmState state);

    public class DHsm : QHsm
    {
        // private variables
        UndoRedoManager undoRedoManager;
        DViewerHandler viewerHandler;
        DFigureHandler figureHandler;
        DAuthorProperties authorProps;

        Type currentFigureClass = null;
        Figure currentFigure = null;
        DPoint dragPt;
        double dragRot;
        DHitTest mouseHitTest;

        bool lockInitialAspectRatio = false;
        double unlockInitalAspectRatioThreshold = 50;

        const double figureSnapAngle = Math.PI / 4;        // 45 degrees
        const double figureSnapRange = Math.PI / (4 * 18); // 2.5  degrees (each way)

        // properties
        bool figureLockAspectRatio = false;
        public bool FigureLockAspectRatio
        {
            get { return figureLockAspectRatio; }
            set { figureLockAspectRatio = value; }
        }
        public bool LockingAspectRatio
        {
            get { return lockInitialAspectRatio || figureLockAspectRatio; }
        }

        bool figureAlwaysSnapAngle = false;
        public bool FigureAlwaysSnapAngle
        {
            get { return figureAlwaysSnapAngle; }
            set { figureAlwaysSnapAngle = value; }
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
                    case DHsmState.Eraser:
                        Dispatch(new QEvent((int)DHsmSignals.GEraser));
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set directly to '{0}' :(", value));
                        break;
                }
            }
        }

        public event DebugMessageHandler DebugMessage;
        public event ContextClickHandler ContextClick;

        // state variables (showing state hierachy here)
        QState Main;
            QState Select;
                QState SelectDefault;
                QState DragFigure;
                QState DragSelect;
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

        public DHsm(UndoRedoManager undoRedoManager, DViewerHandler viewerHandler, DFigureHandler figureHandler, DAuthorProperties authorProps) : base()
        {
            // undo redo manager
            this.undoRedoManager = undoRedoManager;
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
            // author properties
            this.authorProps = authorProps;
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

        void DoStateChanged(DHsmState state)
        {
            if (StateChanged != null)
                StateChanged(null, state);
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

        // private methods

        void ClearCurrentFigure()
        {
            if (currentFigure != null)
            {
                // remove current figure if it was not sized by a mouse drag
                if (currentFigure.Width == 0 || currentFigure.Height == 0)
                    figureHandler.Remove(currentFigure);
                // null currentfigure
                currentFigure = null;
            }
        }

        DPoint CalcDragDelta(DPoint pt)
        {
            return new DPoint(pt.X - dragPt.X, pt.Y - dragPt.Y);
        }

        DPoint CalcSizeDelta(DPoint pt, Figure f, bool lockAspectRatio)
        {
            if (lockAspectRatio)
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

        protected const int MIN_SIZE = 5;

        void DoDebugMessage(string msg)
        {
            if (DebugMessage != null)
                DebugMessage(msg);
        }

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
            }
            return this.TopState;
        }

        QState DoSelect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(SelectDefault);
                    return Main;
                case (int)QSignals.Entry:
                    // clear currentFigureClass
                    currentFigureClass = null;
                    // dont clear currentfigure and selected if we have transitioned from TextEdit state
                    if (!IsInState(TextEdit))
                    {
                        ClearCurrentFigure();
                        figureHandler.ClearSelected();
                    }
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.Select);
                    return null;
            }
            return this.Main;
        }

        void DoSelectDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                // find and select clicked figure
                IGlyph glyph;
                Figure f = figureHandler.HitTestSelect(pt, out mouseHitTest, out glyph);
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
                            dragPt = pt;
                            break;
                        case DHitTest.SelectRect:
                            goto case DHitTest.Body;
                        case DHitTest.Resize:
                            dragPt = new DPoint(0, 0);
                            dragPt = CalcSizeDelta(f.RotatePointToFigure(pt), f, LockingAspectRatio || f.LockAspectRatio);
                            lockInitialAspectRatio = true;
                            break;
                        case DHitTest.ReposLinePt1:
                            break;
                        case DHitTest.ReposLinePt2:
                            goto case DHitTest.ReposLinePt1;
                        case DHitTest.Rotate:
                            dragRot = GetRotationOfPointComparedToFigure(f, pt) - f.Rotation;
                            if (dragRot > Math.PI)
                                dragRot = dragRot - (Math.PI * 2);
                            break;
                    }
                    // transition
                    TransitionTo(DragFigure);
                }
                else
                {
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
            IGlyph glyph;
            figureHandler.HitTestFigures(pt, out hitTest, out glyph);
            switch (hitTest)
            {
                case DHitTest.None:
                    dv.SetCursor(DCursor.Default);
                    break;
                case DHitTest.Body:
                    dv.SetCursor(DCursor.MoveAll);
                    break;
                case DHitTest.SelectRect:
                    goto case DHitTest.Body;
                case DHitTest.Resize:
                    dv.SetCursor(DCursor.MoveNWSE);
                    break;
                case DHitTest.ReposLinePt1:
                    goto case DHitTest.Resize;
                case DHitTest.ReposLinePt2:
                    goto case DHitTest.Resize;
                case DHitTest.Rotate:
                    dv.SetCursor(DCursor.Rotate);
                    break;
                case DHitTest.Glyph:
                    dv.SetCursor(glyph.Cursor);
                    break;
            }
            DoDebugMessage(string.Format("{0}, {1}", pt.X, pt.Y));
        }

        void DoSelectDefaultMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            DHitTest hitTest;
            IGlyph glyph;
            if (btn == DMouseButton.Left)
            {
                Figure f = figureHandler.HitTestFigures(pt, out hitTest, out glyph);
                if (hitTest == DHitTest.Glyph)
                    glyph.CallClicked(f, dv.EngineToClient(pt));
            }
            else if (btn == DMouseButton.Right)
            {
                Figure f = figureHandler.HitTestSelect(pt, out hitTest, out glyph);
                dv.SetCursor(DCursor.Default);
                dv.Update();
                if (ContextClick != null)
                    ContextClick(null, f, dv.EngineToClient(pt));
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
                    DoSelectDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoSelectDefaultMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Select;
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
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    foreach (Figure f in figureHandler.SelectedFigures)
                        updateRect = updateRect.Union(GetBoundingBox(f));
                    // apply x/y delta to figures
                    DPoint dPos = CalcDragDelta(pt);
                    if (dPos.X != 0 || dPos.Y != 0)
                        foreach (Figure f in figureHandler.SelectedFigures)
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
                    // alert figure we are going to resize it
                    currentFigure.BeforeResize();
                    // inital update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // translate point onto the same rotated plane as the figure
                    pt = currentFigure.RotatePointToFigure(pt);
                    // apply width/height delta to figure
                    DPoint dSize = CalcSizeDelta(pt, currentFigure, LockingAspectRatio || currentFigure.LockAspectRatio);
                    if (lockInitialAspectRatio && !(figureLockAspectRatio || currentFigure.LockAspectRatio))
                    {
                        DPoint dSizeUnlocked = CalcSizeDelta(pt, currentFigure, false);
                        if (Math.Abs(dSizeUnlocked.X - dSize.X) >= unlockInitalAspectRatioThreshold ||
                            Math.Abs(dSizeUnlocked.Y - dSize.Y) >= unlockInitalAspectRatioThreshold)
                        {
                            lockInitialAspectRatio = false;
                            dSize = dSizeUnlocked;
                        }
                    }
                    if (currentFigure.Width + dSize.X < MIN_SIZE)
                    {
                        dSize.X = MIN_SIZE - currentFigure.Width;
                        if (LockingAspectRatio || currentFigure.LockAspectRatio)
                            dSize.Y = (currentFigure.Height / currentFigure.Width) * dSize.X;
                    }
                    if (currentFigure.Height + dSize.Y < MIN_SIZE)
                    {
                        dSize.Y = MIN_SIZE - currentFigure.Height;
                        if (LockingAspectRatio || currentFigure.LockAspectRatio)
                            dSize.X = (currentFigure.Width / currentFigure.Height) * dSize.Y;
                    }
                    DRect oldRect = currentFigure.Rect;
                    currentFigure.Width += dSize.X;
                    currentFigure.Height += dSize.Y;
                    DGeom.UpdateRotationPosition(currentFigure, oldRect, currentFigure.Rect); 
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    // alert figure we have finished resizing
                    currentFigure.AfterResize();
                    // debug message
                    DoDebugMessage(string.Format("{0} {1}", dSize.X, dSize.Y));
                    break;
                case DHitTest.ReposLinePt1:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // inital update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // get our line segment interface
                    ILineSegment ls = (ILineSegment)currentFigure;
                    // setup points
                    DPoint oldPoint, newPoint, otherPoint;
                    if (mouseHitTest == DHitTest.ReposLinePt1)
                    {
                        oldPoint = ls.Pt1;
                        newPoint = new DPoint(pt.X, pt.Y);
                        otherPoint = ls.Pt2;
                    }
                    else
                    {
                        oldPoint = ls.Pt2;
                        newPoint = new DPoint(pt.X, pt.Y);
                        otherPoint = ls.Pt1;
                    }
                    SetPointDelegate setPoint = delegate(DPoint point)
                    {
                        if (mouseHitTest == DHitTest.ReposLinePt1)
                            ls.Pt1 = point;
                        else
                            ls.Pt2 = point;
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
                    // find the current angle of the line and the remainder when divided by the snap angle
                    double currentAngle = DGeom.AngleBetweenPoints(oldPoint, otherPoint);
                    double ar = currentAngle % figureSnapAngle;
                    // reposition line
                    if (figureAlwaysSnapAngle)
                    {
                        // slide point along snap angle
                        double newAngle = DGeom.AngleBetweenPoints(newPoint, otherPoint);
                        ar = newAngle % figureSnapAngle;
                        if (ar < figureSnapAngle / 2) 
                            setPoint(DGeom.RotatePoint(newPoint, otherPoint, -ar));
                        else
                            setPoint(DGeom.RotatePoint(newPoint, otherPoint, figureSnapAngle - ar));
                    }
                    else if (ar == 0)
                    {
                        // line is snapped, test if new angle will unsnap the line
                        double newAngle = DGeom.AngleBetweenPoints(newPoint, otherPoint);
                        ar = newAngle % figureSnapAngle;
                        if (ar > figureSnapRange && ar < figureSnapAngle - figureSnapRange)
                            // unsnapped, set new point
                            setPoint(newPoint);
                        else
                        {
                            // slide point along snap angle
                            newPoint = DGeom.RotatePoint(newPoint, otherPoint, getRotationalSnap(ar));
                            setPoint(newPoint);
                        }
                    }
                    else
                    {
                        // set new point
                        setPoint(newPoint);
                        // test whether to snap our line
                        double newAngle = DGeom.AngleBetweenPoints(newPoint, otherPoint);
                        ar = newAngle % figureSnapAngle;
                        double rotationalSnap = getRotationalSnap(ar);
                        // snap it
                        if (rotationalSnap != 0)
                            setPoint(DGeom.RotatePoint(newPoint, otherPoint, rotationalSnap));
                    }                       
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    break;
                case DHitTest.ReposLinePt2:
                    goto case DHitTest.ReposLinePt1;
                case DHitTest.Rotate:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // apply rotation to figure
                    double newRot = GetRotationOfPointComparedToFigure(currentFigure, pt) - dragRot;
                    double r = newRot % figureSnapAngle;
                    if (figureAlwaysSnapAngle)
                    {
                        if (r < figureSnapAngle / 2)
                            currentFigure.Rotation = newRot - r;
                        else
                            currentFigure.Rotation = newRot + figureSnapAngle - r;
                    }
                    else if (r < figureSnapRange)
                        currentFigure.Rotation = newRot - r;
                    else if (r > figureSnapAngle - figureSnapRange)
                        currentFigure.Rotation = newRot + figureSnapAngle - r;
                    else
                        currentFigure.Rotation = newRot;
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    // debug message
                    DoDebugMessage((currentFigure.Rotation * 180 / Math.PI).ToString());
                    break;
            }
            // update drawing
            dv.Update(updateRect);
        }

        void DoDragFigureMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                currentFigure = null;
                // transition
                TransitionTo(SelectDefault);
            }
        }

        void DoDragFigureDoubleClick(DTkViewer dv, DPoint pt)
        {
            DHitTest ht;
            IGlyph glyph;
            Figure f = figureHandler.HitTestFigures(pt, out ht, out glyph);
            if (f is TextFigure)
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

        QState DoDragFigure(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    // record state for undo/redo manager
                    undoRedoManager.Start("Select Operation");
                    break;
                case (int)QSignals.Exit:
                    // commit undo changes
                    undoRedoManager.Commit();
                    break;
                case (int)DHsmSignals.MouseMove:
                    DoDragFigureMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseUp:
                    DoDragFigureMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.DoubleClick:
                    DoDragFigureDoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Select;
        }

        QState DoDragSelect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    drawSelection = true;
                    break;
                case (int)QSignals.Exit:
                    drawSelection = false;
                    break;
                case (int)DHsmSignals.MouseMove:
                    // rectangular area to update with paint event
                    DRect updateRect = new DRect();
                    // initial update rect
                    updateRect = figureHandler.SelectionFigure.Rect;
                    // drag select figure
                    figureHandler.SelectionFigure.TopLeft = dragPt;
                    figureHandler.SelectionFigure.BottomRight = ((QMouseEvent)qevent).Pt;
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
                    ((QMouseEvent)qevent).Dv.Update(updateRect);
                    return null;
                case (int)DHsmSignals.MouseUp:
                        DRect updateRect2 = figureHandler.SelectionFigure.Rect;
                        List<Figure> selectFigs = new List<Figure>();
                        foreach (Figure f in figureHandler.Figures)
                            if (DGeom.PointInRect(f.Rect.Center, figureHandler.SelectionFigure.Rect))
                                selectFigs.Add(f);
                        figureHandler.SelectFigures(selectFigs);
                        foreach (Figure f in selectFigs)
                            updateRect2 = updateRect2.Union(GetBoundingBox(f));
                        // update drawing
                        ((QMouseEvent)qevent).Dv.Update(updateRect2);
                        // transition back
                        TransitionTo(SelectDefault);
                    return null;
                case (int)DHsmSignals.DoubleClick:
                    DoDragFigureDoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Select;
        }

        QState DoDrawLine(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(DrawLineDefault);
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.DrawLine);
                    return null;
            }
            return this.Main;
        }

        void DoDrawLineDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                undoRedoManager.Start("Add Line");
                // create line figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                if (currentFigure is ILineSegment)
                {
                    if (((ILineSegment)currentFigure).Pt1 == null)
                        ((ILineSegment)currentFigure).Pt1 = pt;
                    else
                        ((ILineSegment)currentFigure).Pt2 = pt;
                }
                else if (currentFigure is IPolyline)
                    ((IPolyline)currentFigure).AddPoint(pt);
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figureHandler.Add(currentFigure);
                // transition
                TransitionTo(DrawingLine);
            }
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
            // add point
            if (figureAlwaysSnapAngle && currentFigure is ILineSegment)
            {
                ILineSegment ls = ((ILineSegment)currentFigure);
                // slide point along snap angle
                double newAngle = DGeom.AngleBetweenPoints(pt, ls.Pt1);
                double r = newAngle % figureSnapAngle;
                if (r < figureSnapAngle / 2)
                    pt = DGeom.RotatePoint(pt, ls.Pt1, -r);
                else
                    pt = DGeom.RotatePoint(pt, ls.Pt1, figureSnapAngle - r);
            }
            if (currentFigure is ILineSegment)
                ((ILineSegment)currentFigure).Pt2 = pt;
            else if (currentFigure is IPolyline)
                ((IPolyline)currentFigure).AddPoint(pt);
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingLineMouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            undoRedoManager.Commit();
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
            }
            return this.DrawLine;
        }

        void DoDrawTextMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                ClearCurrentFigure();
                undoRedoManager.Start("Add Text");
                // create TextFigure
                currentFigure = new TextFigure(pt, "", 0);
                authorProps.ApplyPropertiesToFigure((TextFigure)currentFigure);
                // add to list of figures
                figureHandler.Add(currentFigure);
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
                case (int)QSignals.Init:
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.DrawText);
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
                // find and select clicked figure
                IGlyph glyph;
                Figure f = figureHandler.HitTestSelect(pt, out mouseHitTest, out glyph);
                // select the TextFigure from the TextEditFigure
                TextEditFigure tef = (TextEditFigure)currentFigure;
                if (f == tef)
                {
                    if (tef.HasText)
                    {
                        f = tef.TextFigure;
                        figureHandler.SelectFigures(new List<Figure>(new Figure[] { f }));
                    }
                }
                // setup for select mouse move
                dragPt = pt;
                // transition to select state
                TransitionTo(DragFigure);
            }
        }

        void DoTextEditMouseMove(DTkViewer dv, DPoint pt)
        {
            DoSelectDefaultMouseMove(dv, pt);
        }

        void DoTextEditKeyPress(DTkViewer dv, int k)
        {
            if (currentFigure != null && currentFigure is TextEditFigure)
            {
                TextEditFigure te = (TextEditFigure)currentFigure;
                DRect updateRect = currentFigure.Rect;
                switch ((DKeys)k)
                {
                    case DKeys.Backspace:
                        te.BackspaceAtCursor();
                        break;
                    case DKeys.Enter:
                        te.InsertAtCursor('\n');
                        break;
                    case DKeys.Escape:
                        State = DHsmState.Select;
                        break;
                    case DKeys.Delete:
                        te.DeleteAtCursor();
                        break;
                    case DKeys.Left:
                        te.MoveCursor((DKeys)k);
                        break;
                    case DKeys.Right: goto case DKeys.Left;
                    case DKeys.Up: goto case DKeys.Left;
                    case DKeys.Down: goto case DKeys.Left;
                    default:
                        te.InsertAtCursor((char)k);
                        break;
                }
                dv.Update(updateRect.Union(currentFigure.Rect));
            }
        }

        QState DoTextEdit(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    return Main;
                case (int)QSignals.Entry:
                    // start undo record
                    undoRedoManager.Start("Text Edit");
                    // add TextEditFigure
                    Figure tf = currentFigure;
                    currentFigure = new TextEditFigure((TextFigure)tf);
                    figureHandler.Insert(currentFigure, tf);
                    figureHandler.Remove(tf);
                    // update view
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.TextEdit);
                    return null;
                case (int)QSignals.Exit:
                    // replace text edit figure with the textfigure
                    if (currentFigure is TextEditFigure)
                    {
                        if (((TextEditFigure)currentFigure).HasText)
                            figureHandler.Insert(((TextEditFigure)currentFigure).TextFigure, currentFigure);
                        figureHandler.Remove(currentFigure);
                    }
                    // record text edit to undo manager
                    undoRedoManager.Commit();
                    return null;
                case (int)DHsmSignals.MouseDown:
                    DoTextEditMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.MouseMove:
                    DoTextEditMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DHsmSignals.KeyPress:
                    DoTextEditKeyPress(((QKeyPressEvent)qevent).Dv, ((QKeyPressEvent)qevent).Key);
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
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    figureHandler.ClearSelected();
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.DrawRect);
                    return null;
            }
            return this.Main;
        }

        void DoDrawRectDefaultMouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                undoRedoManager.Start(string.Format("Add {0}", currentFigureClass.Name));
                // create Figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                currentFigure.TopLeft = pt;
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figureHandler.Add(currentFigure);
                // store drag pt for reference on mousemove event)
                dragPt = pt;
                // transition
                TransitionTo(DrawingRect);
            }
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
            // change dimensions
            if (pt.X >= dragPt.X)
                currentFigure.Right = pt.X;
            else
            {
                currentFigure.Left = pt.X;
                currentFigure.Right = dragPt.X;
            }
            if (figureLockAspectRatio || currentFigure.LockAspectRatio)
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
            undoRedoManager.Commit();
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
                case (int)QSignals.Init:
                    return Main;
                case (int)QSignals.Entry:
                    DoStateChanged(DHsmState.FigureEdit);
                    // start undo record
                    undoRedoManager.Start("Figure Edit");
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
                    undoRedoManager.Commit();
                    return null;
                case (int)DHsmSignals.MouseDown:
                    ((IEditable)currentFigure).MouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
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
            }
            return this.Main;
        }

        QState DoEraser(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(EraserDefault);
                    return Main;
                case (int)QSignals.Entry:
                    // clear currentFigureClass
                    currentFigureClass = null;
                    // clear currentfigure and selected
                    ClearCurrentFigure();
                    figureHandler.ClearSelected();
                    // update
                    viewerHandler.Update();
                    DoStateChanged(DHsmState.Eraser);
                    return null;
            }
            return this.Main;
        }

        QState DoEraserDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DHsmSignals.MouseDown:
                    TransitionTo(Erasing);
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
                    DPoint rotPt = f.RotatePointToFigure(eraserPt);
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
                    ErasePolylines(f.RotatePointToFigure(eraserPt), f.ChildFigures, ref updateRect, f);
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
                    undoRedoManager.Start("Erase Operation");
                    break;
                case (int)QSignals.Exit:
                    // commit undo changes
                    undoRedoManager.Commit();
                    // hide eraser
                    drawEraser = false;
                    viewerHandler.Update(); // and show updated polylines in other viewers too
                    break;
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
    }
}
