using System;
using System.Collections.Generic;
using System.Text;

using qf4net;

namespace DDraw
{
    public enum DEngineSignals : int
    {
        //enum values must start at UserSig value or greater
        GSelect = QSignals.UserSig, GDrawLine, GDrawText, GDrawRect,
        TextEdit, FigureEdit,
        MouseDown, MouseMove, MouseUp, DoubleClick,
        KeyDown, KeyPress, KeyUp
    }

    public class QMouseEvent : QEvent
    {
        DViewer dv;
        public DViewer Dv
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

        public QMouseEvent(int qSignal, DViewer dv, DMouseButton button, DPoint pt) : base(qSignal)
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
        DViewer dv;
        public DViewer Dv
        {
            get { return dv; }
        }
        char key;
        public char Key
        {
            get { return key; }
        }

        public QKeyPressEvent(int qSignal, DViewer dv, char key) : base(qSignal)
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

    public enum DEngineState { Select, DrawLine, DrawText, TextEdit, DrawRect, FigureEdit };

    public partial class DEngine : QHsm
    {
        Type currentFigureClass;
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

        // state variables (showing state hierachy here)
        QState Main;
            QState Select;
                QState SelectDefault;
                QState DragFigure;
            QState DrawLine;
                QState DrawLineDefault;
                QState DrawingLine;
            QState DrawText;
            QState TextEdit;
            QState DrawRect;
                QState DrawRectDefault;
                QState DrawingRect;
            QState FigureEdit;

        public delegate void DEngineStateChangedHandler(DEngine de, DEngineState state);
        public event DEngineStateChangedHandler StateChanged;
        public DEngineState State
        {
            get
            {
                if (IsInState(Select))
                    return DEngineState.Select;
                if (IsInState(DrawLine))
                    return DEngineState.DrawLine;
                if (IsInState(DrawText))
                    return DEngineState.DrawText;
                if (IsInState(TextEdit))
                    return DEngineState.TextEdit;
                if (IsInState(DrawRect))
                    return DEngineState.DrawRect;
                if (IsInState(FigureEdit))
                    return DEngineState.FigureEdit;
                System.Diagnostics.Debug.Assert(false, "Logic Error :(");
                return DEngineState.Select;
            }
            set
            {
                switch (value)
                {
                    case DEngineState.Select:
                        Dispatch(new QEvent((int)DEngineSignals.GSelect));
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set directly to '{0}' :(", value));
                        break;
                }
            }
        }

        public void SetStateByFigureClass(Type FigureClass)
        {
            if (FigureClass.Equals(typeof(TextFigure))) // do TextFigure first as it is inherits from RectbaseFigure
                Dispatch(new QGDrawFigureEvent((int)DEngineSignals.GDrawText, FigureClass));
            else if (FigureClass.IsSubclassOf(typeof(RectbaseFigure)))
                Dispatch(new QGDrawFigureEvent((int)DEngineSignals.GDrawRect, FigureClass));
            else if (FigureClass.IsSubclassOf(typeof(LinebaseFigure)))
                Dispatch(new QGDrawFigureEvent((int)DEngineSignals.GDrawLine, FigureClass));
            else
                System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set state using '{0}' :(", FigureClass.Name));
        }

        void DoStateChanged(DEngineState state)
        {
            if (StateChanged != null)
                StateChanged(this, state);
        }

        QState DoMain(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(Select);
                    return null;
                case (int)DEngineSignals.GSelect:
                    TransitionTo(Select);
                    return null;
                case (int)DEngineSignals.GDrawLine:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawLine);
                    return null;
                case (int)DEngineSignals.GDrawText:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawText);
                    return null;
                case (int)DEngineSignals.GDrawRect:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawRect);
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
                        ClearSelected();
                    }
                    UpdateViewers();
                    DoStateChanged(DEngineState.Select);
                    return null;
            }
            return this.Main;
        }

        void DoSelectDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                // find and select clicked figure
                Figure f = HitTestSelect(pt, out mouseHitTest);
                // update selected figures
                if (f != null)
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
                            dragPt = pt;
                            break;
                        case DHitTest.ReposLinePt2:
                            goto case DHitTest.ReposLinePt1;
                        case DHitTest.Rotate:
                            dragRot = GetRotationOfPointComparedToFigure(f, pt) - f.Rotation;
                            if (dragRot > Math.PI)
                                dragRot = dragRot - (Math.PI * 2);
                            break;
                    }
                }
                else
                {
                    ClearSelectedFiguresList();
                    DoSelectedFiguresChanged();
                    dragPt = pt; // mouseHitTest = DHitTest.None
                }
                // update drawing
                dv.Update();
                // transition
                TransitionTo(DragFigure);
            }
        }

        void DoSelectDefaultMouseMove(DViewer dv, DPoint pt)
        {
            // set cursor
            DHitTest hitTest;
            HitTestFigures(pt, out hitTest);
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
            }
            DoDebugMessage(string.Format("{0}, {1}", pt.X, pt.Y));
        }

        void DoSelectDefaultMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Right)
            {
                DHitTest hitTest;
                Figure f = HitTestSelect(pt, out hitTest);
                dv.SetCursor(DCursor.Default);
                dv.Update();
                if (ContextClick != null)
                    ContextClick(this, f, dv.EngineToClient(pt));
            }
        }

        QState DoSelectDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoSelectDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoSelectDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoSelectDefaultMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Select;
        }

        void DoDragFigureMouseMove(DViewer dv, DPoint pt)
        {
            // rectangular area to update with paint event
            DRect updateRect = new DRect();
            // move selected figures
            switch (mouseHitTest)
            {
                case DHitTest.None:
                    // initial update rect
                    updateRect = selectionRect.Rect;
                    // drag select figure
                    selectionRect.TopLeft = dragPt;
                    selectionRect.BottomRight = pt;
                    if (selectionRect.Width < 0)
                    {
                        selectionRect.X += selectionRect.Width;
                        selectionRect.Width = -selectionRect.Width;
                    }
                    if (selectionRect.Height < 0)
                    {
                        selectionRect.Y += selectionRect.Height;
                        selectionRect.Height = -selectionRect.Height;
                    }
                    drawSelectionRect = true;
                    // final update rect
                    updateRect = updateRect.Union(selectionRect.Rect);
                    break;
                case DHitTest.Body:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    foreach (Figure f in selectedFigures)
                        updateRect = updateRect.Union(GetBoundingBox(f));
                    // apply x/y delta to figures
                    DPoint dPos = CalcDragDelta(pt);
                    foreach (Figure f in selectedFigures)
                    {
                        f.X += dPos.X;
                        f.Y += dPos.Y;
                    }
                    // store drag pt for reference later (eg. next mousemove event)
                    dragPt = pt;
                    // final update rect
                    foreach (Figure f in selectedFigures)
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
                    currentFigure.Width += dSize.X;
                    currentFigure.Height += dSize.Y;
                    // apply x/y delta to figure to account for rotation
                    dPos = CalcPosDeltaFromAngle(currentFigure.Rotation, dSize);
                    currentFigure.X += dPos.X;
                    currentFigure.Y += dPos.Y;
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    // alert figure we have finished resizing
                    currentFigure.AfterResize();
                    // debug message
                    DoDebugMessage(string.Format("{0} {1}", dSize.X, dSize.Y));
                    break;
                case DHitTest.ReposLinePt1:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // alert figure we are going to resize it
                    currentFigure.BeforeResize();
                    // inital update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // resize
                    LineSegmentbaseFigure lf = (LineSegmentbaseFigure)currentFigure;
                    if (mouseHitTest == DHitTest.ReposLinePt1)
                    {
                        lf.pt1.X += pt.X - dragPt.X;
                        lf.pt1.Y += pt.Y - dragPt.Y;
                    }
                    else if (mouseHitTest == DHitTest.ReposLinePt2)
                    {
                        lf.pt2.X += pt.X - dragPt.X;
                        lf.pt2.Y += pt.Y - dragPt.Y;
                    }
                    dragPt = pt;
                    // final update rect
                    updateRect = updateRect.Union(GetBoundingBox(currentFigure));
                    // alert figure we have finished resizing
                    currentFigure.AfterResize();
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
                    if (r < figureSnapRange)
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

        void DoDragFigureMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                currentFigure = null;
                if (drawSelectionRect)
                {
                    DRect updateRect = selectionRect.Rect;
                    foreach (Figure f in figures)
                        if (selectionRect.Contains(f))
                        {
                            AddToSelected(f);
                            updateRect = updateRect.Union(GetBoundingBox(f));
                        }
                    DoSelectedFiguresChanged();
                    drawSelectionRect = false;
                    // update drawing
                    dv.Update(updateRect);
                }
                // transition
                TransitionTo(SelectDefault);
            }
        }

        void DoDragFigureDoubleClick(DViewer dv, DPoint pt)
        {
            DHitTest ht;
            Figure f = HitTestFigures(pt, out ht);
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
                    if (autoUndoRecord)
                        undoRedoMgr.Start("Select Operation");
                    break;
                case (int)QSignals.Exit:
                    // commit undo changes
                    if (autoUndoRecord)
                        undoRedoMgr.Commit();
                    break;
                case (int)DEngineSignals.MouseMove:
                    DoDragFigureMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDragFigureMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.DoubleClick:
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
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.DrawLine);
                    return null;
            }
            return this.Main;
        }

        void DoDrawLineDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Line");
                // create line figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                ((LinebaseFigure)currentFigure).AddPoint(pt);
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figures.Add(currentFigure);
                // transition
                TransitionTo(DrawingLine);
            }
        }

        void DoDrawLineDefaultMouseMove(DViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawLineDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoDrawLineDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawLineDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawLine;
        }

        void DoDrawingLineMouseMove(DViewer dv, DPoint pt)
        {
            // initial update rect
            DRect updateRect = currentFigure.GetSelectRect();
            // add point
            ((LinebaseFigure)currentFigure).AddPoint(pt);
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingLineMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawLineDefault);
        }

        QState DoDrawingLine(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseMove:
                    DoDrawingLineMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawingLineMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawLine;
        }

        void DoDrawTextMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                ClearCurrentFigure();
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Text");
                // create TextFigure
                currentFigure = new TextFigure(pt, "", GraphicsHelper.TextExtent, 0);
                authorProps.ApplyPropertiesToFigure((TextFigure)currentFigure);
                // add to list of figures
                figures.Add(currentFigure);
                // update DViewer
                dv.Update();
                // transition
                TransitionTo(TextEdit);
            }
        }

        void DoDrawTextMouseMove(DViewer dv, DPoint pt)
        {

        }

        void DoDrawTextMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
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
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.DrawText);
                    return null;
                case (int)DEngineSignals.MouseDown:
                    DoDrawTextMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawTextMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawTextMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.Main;
        }

        void DoTextEditMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                // find and select clicked figure
                Figure f = HitTestSelect(pt, out mouseHitTest);
                // select the TextFigure from the TextEditFigure
                TextEditFigure tef = (TextEditFigure)currentFigure;
                if (f == tef)
                {
                    if (tef.HasText)
                    {
                        f = tef.TextFigure;
                        ClearSelectedFiguresList();
                        AddToSelected(f);
                        DoSelectedFiguresChanged();
                    }
                }
                // setup for select mouse move
                dragPt = pt;
                // transition to select state
                TransitionTo(DragFigure);
                // select op started
                if (autoUndoRecord)
                    undoRedoMgr.Start("Select Operation");
            }
        }

        void DoTextEditMouseMove(DViewer dv, DPoint pt)
        {
            DoSelectDefaultMouseMove(dv, pt);
        }

        void DoTextEditKeyPress(DViewer dv, char k)
        {
            if (currentFigure != null && currentFigure is ITextable)
            {
                ITextable tf = (ITextable)currentFigure;
                switch (k)
                {
                    case '\b': // backspace
                        if (tf.HasText)
                        {
                            DRect r = currentFigure.Rect;
                            tf.Text = tf.Text.Substring(0, tf.Text.Length - 1);
                            dv.Update(r);
                        }
                        break;
                    case '\r': // enter
                        State = DEngineState.Select;
                        break;
                    case (char)27: // esc
                        goto case '\r';
                    default:
                        tf.Text = string.Concat(tf.Text, k);
                        dv.Update(currentFigure.Rect);
                        break;
                }
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
                    if (autoUndoRecord)
                        undoRedoMgr.Start("Text Edit");
                    // add TextEditFigure
                    Figure tf = currentFigure;
                    currentFigure = new TextEditFigure((TextFigure)tf);
                    figures.Insert(figures.IndexOf(tf), currentFigure);
                    figures.Remove(tf);
                    // update view
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.TextEdit);
                    return null;
                case (int)QSignals.Exit:
                    // replace text edit figure with the textfigure
                    if (currentFigure is TextEditFigure)
                    {
                        if (((TextEditFigure)currentFigure).HasText)
                            figures.Insert(figures.IndexOf(currentFigure), ((TextEditFigure)currentFigure).TextFigure);
                        figures.Remove(currentFigure);
                    }
                    // record text edit to undo manager
                    if (autoUndoRecord)
                        undoRedoMgr.Commit();
                    return null;
                case (int)DEngineSignals.MouseDown:
                    DoTextEditMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoTextEditMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.KeyPress:
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
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.DrawRect);
                    return null;
            }
            return this.Main;
        }

        void DoDrawRectDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                if (autoUndoRecord)
                    undoRedoMgr.Start(string.Format("Add {0}", currentFigureClass.Name));
                // create Figure
                currentFigure = (Figure)Activator.CreateInstance(currentFigureClass);
                currentFigure.TopLeft = pt;
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figures.Add(currentFigure);
                // store drag pt for reference on mousemove event)
                dragPt = pt;
                // transition
                TransitionTo(DrawingRect);
            }
        }

        void DoDrawRectDefaultMouseMove(DViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawRectDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoDrawRectDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawRectDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawRect;
        }

        void DoDrawingRectMouseMove(DViewer dv, DPoint pt)
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
            selectionRect.Rect = currentFigure.GetSelectRect();
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingRectMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawRectDefault);
            // update drawing
            dv.Update(selectionRect.Rect);
        }

        QState DoDrawingRect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Entry:
                    drawSelectionRect = true;
                    return null;
                case (int)QSignals.Exit:
                    drawSelectionRect = false;
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawingRectMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
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
                    DoStateChanged(DEngineState.FigureEdit);
                    // start undo record
                    if (autoUndoRecord)
                        undoRedoMgr.Start("Figure Edit");
                    // set editing and connect to edit finished event
                    ((IEditable)currentFigure).StartEdit();
                    ((IEditable)currentFigure).EditFinished += new EditFinishedHandler(currentFigure_EditFinished);
                    // update view
                    ClearSelected();
                    UpdateViewers();
                    return null;
                case (int)QSignals.Exit:
                    // not editing any more
                    ((IEditable)currentFigure).EndEdit();
                    ((IEditable)currentFigure).EditFinished -= currentFigure_EditFinished;                
                    // record figure edit to undo manager
                    if (autoUndoRecord)
                        undoRedoMgr.Commit();
                    return null;
                case (int)DEngineSignals.MouseDown:
                    ((IEditable)currentFigure).MouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    ((IEditable)currentFigure).MouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    ((IEditable)currentFigure).MouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.DoubleClick:
                    ((IEditable)currentFigure).DoubleClick(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.KeyPress:
                    ((IEditable)currentFigure).KeyPress(((QKeyPressEvent)qevent).Dv, ((QKeyPressEvent)qevent).Key);
                    return null;
            }
            return this.Main;
        }

        protected override void InitializeStateMachine()
		{
            Main = new QState(this.DoMain);
            Select = new QState(this.DoSelect);
            SelectDefault = new QState(this.DoSelectDefault);
            DragFigure = new QState(this.DoDragFigure);
            DrawLine = new QState(this.DoDrawLine);
            DrawLineDefault = new QState(this.DoDrawLineDefault);
            DrawingLine = new QState(this.DoDrawingLine);
            DrawText = new QState(this.DoDrawText);
            TextEdit = new QState(this.DoTextEdit);
            DrawRect = new QState(this.DoDrawRect);
            DrawRectDefault = new QState(this.DoDrawRectDefault);
            DrawingRect = new QState(this.DoDrawingRect);
            FigureEdit = new QState(this.DoFigureEdit);
			InitializeState(Main); // initial transition			
		}
    }
}
