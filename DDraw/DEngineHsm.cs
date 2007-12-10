using System;
using System.Collections.Generic;
using System.Text;

using qf4net;

namespace DDraw
{
    public enum DEngineSignals : int
    {
        //enum values must start at UserSig value or greater
        GSelect = QSignals.UserSig, GDrawPolyline, GDrawRect, GDrawEllipse, GDrawText,
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

    public enum DEngineState { Select, DrawPolyline, DrawRect, DrawEllipse, DrawText, TextEdit };

    public partial class DEngine : QHsm
    {
        // state variables (showing state hierachy here)
        QState Main;
            QState Select;
                QState SelectDefault;
                QState DragFigure;
            QState DrawPolyline;
                QState DrawPolyDefault;
                QState DrawingPoly;
            QState DrawRect;
                QState DrawRectDefault;
                QState DrawingRect;
            QState DrawEllipse;
                QState DrawEllipseDefault;
                QState DrawingEllipse;
            QState DrawText;
            QState TextEdit;
            QState FigureEdit;

        public delegate void DEngineStateChangedHandler();
        public event DEngineStateChangedHandler StateChanged;
        public DEngineState State
        {
            get
            {
                if (IsInState(Select))
                    return DEngineState.Select;
                if (IsInState(DrawPolyline))
                    return DEngineState.DrawPolyline;
                if (IsInState(DrawRect))
                    return DEngineState.DrawRect;
                if (IsInState(DrawEllipse))
                    return DEngineState.DrawEllipse;
                if (IsInState(DrawText))
                    return DEngineState.DrawText;
                if (IsInState(TextEdit))
                    return DEngineState.TextEdit;
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
                    case DEngineState.DrawRect:
                        Dispatch(new QEvent((int)DEngineSignals.GDrawRect));
                        break;
                    case DEngineState.DrawEllipse:
                        Dispatch(new QEvent((int)DEngineSignals.GDrawEllipse));
                        break;
                    case DEngineState.DrawText:
                        Dispatch(new QEvent((int)DEngineSignals.GDrawText));
                        break;
                }
            }
        }

        void DoStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
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
                case (int)DEngineSignals.GDrawRect:
                    TransitionTo(DrawRect);
                    return null;
                case (int)DEngineSignals.GDrawEllipse:
                    TransitionTo(DrawEllipse);
                    return null;
                case (int)DEngineSignals.GDrawText:
                    TransitionTo(DrawText);
                    return null;
            }
            return this.TopState;
        }

        QState DoSelect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    DoStateChanged();
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
                    DoStateChanged();
                    InitializeState(DrawPolyDefault);
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
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

        QState DoDrawRect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    DoStateChanged();
                    InitializeState(DrawRectDefault);
                   return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
                    return null;
            }
            return this.Main;
        }
        
        void DoDrawRectDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Rect");
                // create RectFigure
                currentFigure = new RectFigure(new DRect(pt.X, pt.Y, 0, 0), 0);
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
                ((RectFigure)currentFigure).Right = pt.X;
            else
            {
                ((RectFigure)currentFigure).Left = pt.X;
                ((RectFigure)currentFigure).Right = dragPt.X;
            }
            if (pt.Y >= dragPt.Y)
                ((RectFigure)currentFigure).Bottom = pt.Y;
            else
            {
                ((RectFigure)currentFigure).Top = pt.Y;
                ((RectFigure)currentFigure).Bottom = dragPt.Y;
            }
            // update drawing
            dv.Update(updateRect.Union(currentFigure.GetSelectRect()));
        }

        void DoDrawingRectMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawRectDefault);
        }
        
        QState DoDrawingRect(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseMove:
                    DoDrawingRectMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawingRectMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawRect;
        }

        QState DoDrawEllipse(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)QSignals.Init:
                    DoStateChanged();
                    InitializeState(DrawEllipseDefault);
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
                    return null;
            }
            return this.Main;
        }
        
        void DoDrawEllipseDefaultMouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (btn == DMouseButton.Left)
            {
                if (autoUndoRecord)
                    undoRedoMgr.Start("Add Ellipse");
                // create EllipseFigure
                currentFigure = new EllipseFigure(new DRect(pt.X, pt.Y, 0, 0), 0);
                authorProps.ApplyPropertiesToFigure(currentFigure);
                // add to list of figures
                figures.Add(currentFigure);
                // store drag pt for reference on mousemove event)
                dragPt = pt;
                // transition
                TransitionTo(DrawingEllipse);
            }
        }

        void DoDrawEllipseDefaultMouseMove(DViewer dv, DPoint pt)
        {
            DoDrawRectDefaultMouseMove(dv, pt);
        }
        
        QState DoDrawEllipseDefault(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseDown:
                    DoDrawEllipseDefaultMouseDown(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                                       return null;
                case (int)DEngineSignals.MouseMove:
                    DoDrawEllipseDefaultMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawEllipse;
        }


        void DoDrawingEllipseMouseMove(DViewer dv, DPoint pt)
        {
            DoDrawingRectMouseMove(dv, pt);
        }
            
        void DoDrawingEllipseMouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (autoUndoRecord)
                undoRedoMgr.Commit();
            // transition
            TransitionTo(DrawEllipseDefault);
        }

        QState DoDrawingEllipse(IQEvent qevent)
        {
            switch (qevent.QSignal)
            {
                case (int)DEngineSignals.MouseMove:
                    DoDrawingEllipseMouseMove(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Pt);
                    return null;
                case (int)DEngineSignals.MouseUp:
                    DoDrawingEllipseMouseUp(((QMouseEvent)qevent).Dv, ((QMouseEvent)qevent).Button, ((QMouseEvent)qevent).Pt);
                    return null;
            }
            return this.DrawEllipse;
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
                    DoStateChanged();
                    return Main;
                case (int)QSignals.Entry:
                    ClearCurrentFigure();
                    ClearSelected();
                    UpdateViewers();
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
                    DoStateChanged();
                    return Main;
                case (int)QSignals.Entry:
                    // start undo record
                    if (autoUndoRecord)
                        undoRedoMgr.Start("Text Edit");
                    // update view
                    ClearSelected();
                    UpdateViewers();
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
                    DoStateChanged();
                    return Main;
                case (int)QSignals.Entry:
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
            DrawRect = new QState(this.DoDrawRect);
            DrawRectDefault = new QState(this.DoDrawRectDefault);
            DrawingRect = new QState(this.DoDrawingRect);
            DrawEllipse = new QState(this.DoDrawEllipse);
            DrawEllipseDefault = new QState(this.DoDrawEllipseDefault);
            DrawingEllipse = new QState(this.DoDrawingEllipse);
            DrawText = new QState(this.DoDrawText);
            TextEdit = new QState(this.DoTextEdit);
            FigureEdit = new QState(this.DoFigureEdit);
			InitializeState(Main); // initial transition			
		}
    }
}
