using System;
using System.Collections.Generic;
using System.Text;

using qf4net;

namespace DDraw
{
    public enum DEngineSignals : int
    {
        //enum values must start at UserSig value or greater
        GSelect = QSignals.UserSig, GDrawPolyline, GDrawText, GDrawFigure,
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

    public enum DEngineState { Select, DrawPolyline, DrawText, TextEdit, DrawFigure, FigureEdit };

    public partial class DEngine : QHsm
    {
        Type currentFigureClass;
        public Type CurrentFigureClass
        {
            get { return currentFigureClass; }
        }

        // state variables (showing state hierachy here)
        QState Main;
            QState Select;
                QState SelectDefault;
                QState DragFigure;
            QState DrawPolyline;
                QState DrawPolyDefault;
                QState DrawingPoly;
            QState DrawText;
            QState TextEdit;
            QState DrawFigure;
                QState DrawFigureDefault;
                QState DrawingFigure;
            QState FigureEdit;

        public delegate void DEngineStateChangedHandler(DEngine de, DEngineState state);
        public event DEngineStateChangedHandler StateChanged;
        public DEngineState State
        {
            get
            {
                if (IsInState(Select))
                    return DEngineState.Select;
                if (IsInState(DrawPolyline))
                    return DEngineState.DrawPolyline;
                if (IsInState(DrawText))
                    return DEngineState.DrawText;
                if (IsInState(TextEdit))
                    return DEngineState.TextEdit;
                if (IsInState(DrawFigure))
                    return DEngineState.DrawFigure;
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
                    case DEngineState.DrawPolyline:
                        Dispatch(new QEvent((int)DEngineSignals.GDrawPolyline));
                        break;
                    case DEngineState.DrawText:
                        Dispatch(new QEvent((int)DEngineSignals.GDrawText));
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, String.Format("Sorry, cant set directly to '{0}' :(", value));
                        break;
                }
            }
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
                case (int)DEngineSignals.GDrawPolyline:
                    TransitionTo(DrawPolyline);
                    return null;
                case (int)DEngineSignals.GDrawText:
                    TransitionTo(DrawText);
                    return null;
                case (int)DEngineSignals.GDrawFigure:
                    currentFigureClass = ((QGDrawFigureEvent)qevent).FigureClass;
                    TransitionTo(DrawFigure);
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
                            dragPt = CalcSizeDelta(f.RotatePointToFigure(pt), f);
                            break;
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
                    DPoint dSize = CalcSizeDelta(pt, currentFigure);
                    if (currentFigure.Width + dSize.X < MIN_SIZE)
                    {
                        dSize.X = MIN_SIZE - currentFigure.Width;
                        if (currentFigure.LockAspectRatio)
                            dSize.Y = (currentFigure.Height / currentFigure.Width) * dSize.X;
                    }
                    if (currentFigure.Height + dSize.Y < MIN_SIZE)
                    {
                        dSize.Y = MIN_SIZE - currentFigure.Height;
                        if (currentFigure.LockAspectRatio)
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
                case DHitTest.Rotate:
                    System.Diagnostics.Trace.Assert(currentFigure != null, "currentFigure is null");
                    // initial update rect
                    updateRect = GetBoundingBox(currentFigure);
                    // apply rotation to figure
                    currentFigure.Rotation = GetRotationOfPointComparedToFigure(currentFigure, pt) - dragRot;
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
                currentFigure = new TextEditFigure((TextFigure)f);
                figures.Insert(figures.IndexOf(f), currentFigure);
                figures.Remove(f);
                dv.Update(currentFigure.Rect);
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

        QState DoDrawPolyline(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(DrawPolyDefault);
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.DrawPolyline);
                    return null;
            }
            return this.Main;
        }

        void DoDrawPolyDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Polyline");
                // create DPoints object
                DPoints pts = new DPoints();
                pts.Add(pt);
                // create PolylineFigure
                currentFigure = new PolylineFigure(pts);
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figures.Add(currentFigure);
                // transition
                TransitionTo(DrawingPoly);
            }
        }

        void DoDrawPolyDefaultMouseMove(DViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawPolyDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoDrawPolyDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawPolyDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawPolyline;
        }

        void DoDrawingPolyMouseMove(DViewer dv, DPoint pt)
        {
            // initial update rect
            DRect updateRect = currentFigure.GetSelectRect();
            // add point
            DPoints pts = ((PolylineFigure)currentFigure).Points;
            pts.Add(pt);
            ((PolylineFigure)currentFigure).Points = pts;
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingPolyMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawPolyDefault);
        }

        QState DoDrawingPoly(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseMove:
                    DoDrawingPolyMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawingPolyMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawPolyline;
        }

        void DoDrawTextMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                ClearCurrentFigure();
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Text");
                // create TextFigure
                currentFigure = new TextEditFigure(pt, new TextFigure(pt, "", GraphicsHelper.TextExtent, 0));
                authorProps.ApplyPropertiesToFigure(((TextEditFigure)currentFigure).TextFigure);
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

        QState DoDrawFigure(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    InitializeState(DrawFigureDefault);
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
                    DoStateChanged(DEngineState.DrawFigure);
                    return null;
            }
            return this.Main;
        }

        void DoDrawFigureDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
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
                TransitionTo(DrawingFigure);
            }
        }

        void DoDrawFigureDefaultMouseMove(DViewer dv, DPoint pt)
        {
            // set cursor to draw
            dv.SetCursor(DCursor.Crosshair);
        }

        QState DoDrawFigureDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoDrawFigureDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawFigureDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawFigure;
        }

        void DoDrawingFigureMouseMove(DViewer dv, DPoint pt)
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
            if (pt.Y >= dragPt.Y)
                currentFigure.Bottom = pt.Y;
            else
            {
                currentFigure.Top = pt.Y;
                currentFigure.Bottom = dragPt.Y;
            }
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingFigureMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawFigureDefault);
        }

        QState DoDrawingFigure(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseMove:
                    DoDrawingFigureMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawingFigureMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawFigure;
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
            DrawPolyline = new QState(this.DoDrawPolyline);
            DrawPolyDefault = new QState(this.DoDrawPolyDefault);
            DrawingPoly = new QState(this.DoDrawingPoly);
            DrawText = new QState(this.DoDrawText);
            TextEdit = new QState(this.DoTextEdit);
            DrawFigure = new QState(this.DoDrawFigure);
            DrawFigureDefault = new QState(this.DoDrawFigureDefault);
            DrawingFigure = new QState(this.DoDrawingFigure);
            FigureEdit = new QState(this.DoFigureEdit);
			InitializeState(Main); // initial transition			
		}
    }
}
