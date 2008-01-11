using System;
using System.Collections.Generic;
using System.Text;

using qf4net;

namespace DDraw
{
    public delegate void DebugMessageHandler(string msg);
    public delegate void SelectedFiguresHandler();
    public delegate void PageSizeChangedHandler(DEngine de, DPoint pageSize);
    public delegate void ContextClickHandler(DEngine de, Figure clickedFigure, DPoint pt);

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

    public partial class DEngine
    {
        protected List<Figure> figures = new List<Figure>();
        List<DViewer> viewers = new List<DViewer>();
        List<Figure> selectedFigures = new List<Figure>();
        public List<Figure> SelectedFigures
        {
            get { return new List<Figure>(selectedFigures.ToArray()); }
        }
        Figure currentFigure = null;
        bool drawSelectionRect = false;
        SelectionFigure selectionRect;
        bool drawEraser = false;
        EraserFigure eraser;
        DPoint dragPt;
        double dragRot;
        DHitTest mouseHitTest;

        bool lockInitialAspectRatio = false;
        double unlockInitalAspectRatioThreshold = 50;

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

        const double figureSnapAngle = Math.PI / 4;        // 45 degrees
        const double figureSnapRange = Math.PI / (4 * 18); // 2.5  degrees (each way)

        DAuthorProperties authorProps;
        UndoRedoManager undoRedoMgr;
        public UndoRedoManager UndoRedoMgr
        {
            get { return undoRedoMgr; }
        }
		
        bool autoUndoRecord = true;
        public bool AutoUndoRecord
        {
            get { return autoUndoRecord; }
            set { autoUndoRecord = value; }
        }
        
        DPoint pageSize = new DPoint(500, 400);
        public DPoint PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                foreach (DViewer dv in viewers)
                    dv.SetPageSize(pageSize);
                if (PageSizeChanged != null)
                    PageSizeChanged(this, value);
            }
        }       
        public PageFormat PageFormat
        {
            get { return PageTools.SizeToFormat(pageSize); }
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

        public DEngine(DAuthorProperties ap)
        {
            authorProps = ap;
            selectionRect = new SelectionFigure(new DRect(), 0);
            eraser = new EraserFigure(10);
            undoRedoMgr = new UndoRedoManager(figures);
            undoRedoMgr.UndoRedoChanged += new UndoRedoChangedDelegate(undoRedoMgr_UndoRedoChanged);

            // QHsm Init
            Init();
        }

        void undoRedoMgr_UndoRedoChanged(bool commitAction)
        {
            if (!commitAction)
            {
                ClearSelected();
                UpdateViewers();
            }
        }

        void dv_NeedRepaint(DViewer dv)
        {
            Figure[] controlFigures = new Figure[0];
            if (drawSelectionRect)
            {
                Array.Resize(ref controlFigures, controlFigures.Length + 1);
                controlFigures[controlFigures.Length - 1] = selectionRect;
            }
            if (drawEraser)
            {
                Array.Resize(ref controlFigures, controlFigures.Length + 1);
                controlFigures[controlFigures.Length - 1] = eraser;
            }
            dv.Paint(figures, controlFigures);
        }

        void dv_MouseDown(DViewer dv, DMouseButton btn, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DEngineSignals.MouseDown, dv, btn, pt));
        }

        void dv_MouseMove(DViewer dv, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DEngineSignals.MouseMove, dv, DMouseButton.NotApplicable, pt));
        }

        void dv_MouseUp(DViewer dv, DMouseButton btn, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DEngineSignals.MouseUp, dv, btn, pt));
            foreach (DViewer dvOther in viewers)
                if (dvOther != dv)
                    dvOther.Update();
        }

        void dv_DoubleClick(DViewer dv, DPoint pt)
        {
            Dispatch(new QMouseEvent((int)DEngineSignals.DoubleClick, dv, DMouseButton.NotApplicable, pt));
        }

        void dv_KeyDown(DViewer dv, DKey k)
        {
            Dispatch(new QKeyEvent((int)DEngineSignals.KeyDown, dv, k));
        }

        void dv_KeyPress(DViewer dv, int k)
        {
            Dispatch(new QKeyPressEvent((int)DEngineSignals.KeyPress, dv, k));
        }

        void dv_KeyUp(DViewer dv, DKey k)
        {
            Dispatch(new QKeyEvent((int)DEngineSignals.KeyUp, dv, k));
        }

        // Public Functions //

        public void AddFigure(Figure f)
        {
            figures.Add(f);
        }

        public void AddViewer(DViewer dv)
        {
            dv.SetPageSize(pageSize);
            dv.NeedRepaint += new DPaintEventHandler(dv_NeedRepaint);
            dv.MouseDown += new DMouseButtonEventHandler(dv_MouseDown);
            dv.MouseMove += new DMouseMoveEventHandler(dv_MouseMove);
            dv.MouseUp += new DMouseButtonEventHandler(dv_MouseUp);
            dv.DoubleClick += new DMouseMoveEventHandler(dv_DoubleClick);
            dv.KeyDown += new DKeyEventHandler(dv_KeyDown);
            dv.KeyPress += new DKeyPressEventHandler(dv_KeyPress);
            dv.KeyUp += new DKeyEventHandler(dv_KeyUp);
            viewers.Add(dv);
        }

        public void RemoveViewer(DViewer dv)
        {
            if (viewers.Contains(dv))
            {
                viewers.Remove(dv);
                dv.NeedRepaint -= dv_NeedRepaint;
                dv.MouseDown -= dv_MouseDown;
                dv.MouseMove -= dv_MouseMove;
                dv.MouseUp -= dv_MouseUp;
                dv.DoubleClick -= dv_DoubleClick;
                dv.KeyDown -= dv_KeyDown;
                dv.KeyPress -= dv_KeyPress;
                dv.KeyUp -= dv_KeyUp;
            }
        }

        public void UpdateViewers()
        {
            foreach (DViewer dv in viewers)
                dv.Update();
        }

        public void ClearSelected()
        {
            ClearSelectedFiguresList();
            DoSelectedFiguresChanged();
        }

        public bool CanGroupFigures(List<Figure> figs)
        {
            return figs.Count > 1;
        }

        public void GroupFigures(List<Figure> figs)
        {
            if (CanGroupFigures(figs))
            {
                // init undoRedo frame
                if (autoUndoRecord)
                    undoRedoMgr.Start("Group");
                // make group
                GroupFigure gf = new GroupFigure(figs);
                figures.Add(gf);
                // change selected figures to the group
                ClearSelected();
                AddToSelected(gf);
                DoSelectedFiguresChanged();
                // remove child figures from figure list
                foreach (Figure f in figs)
                    figures.Remove(f);
                // update all viewers
                UpdateViewers();
                // commit changes to undoRedoMgr
                if (autoUndoRecord)
                    undoRedoMgr.Commit();
            }
        }

        public bool CanUngroupFigures(List<Figure> figs)
        {
            return figs.Count == 1 && figs[0] is GroupFigure;
        }

        public void UngroupFigures(List<Figure> figs)
        {
            if (CanUngroupFigures(figs))
            {
                GroupFigure gf = (GroupFigure)figs[0];
                // init undoRedo frame
                if (autoUndoRecord)
                    undoRedoMgr.Start("Ungroup");
                // perform ungroup
                UngroupFigure(gf);
                // add group figures to selected list
                ClearSelected();
                foreach (Figure f in gf.ChildFigures)
                    AddToSelected(f);
                DoSelectedFiguresChanged();
                // update all viewers
                UpdateViewers();
                // commit changes to undoRedoMgr
                if (autoUndoRecord)
                    undoRedoMgr.Commit();
            }
        }

        public void SendToBack(List<Figure> figs)
        {
            // init undoRedo frame
            if (autoUndoRecord)
                undoRedoMgr.Start("Send to Back");
            // do send to back
            OrderFigures(figs);
            for (int i = figs.Count - 1; i >= 0; i--)
            {
                Figure f = figs[i];
                if (figures.Contains(f))
                {
                    figures.Remove(f);
                    figures.Insert(0, f);
                }
            }
            // update all
            DoSelectedFiguresChanged();
            UpdateViewers();
            // commit changes to undoRedoMgr
            if (autoUndoRecord)
                undoRedoMgr.Commit(); 
        }

        public void BringToFront(List<Figure> figs)
        {
            // init undoRedo frame
            if (autoUndoRecord)
                undoRedoMgr.Start("Bring to Front");
            // do bring to front
            OrderFigures(figs);
            foreach (Figure f in figs)
                if (figures.Contains(f))
                {
                    figures.Remove(f);
                    figures.Add(f);
                }
            // update
            DoSelectedFiguresChanged();
            UpdateViewers();
            // commit changes to undoRedoMgr
            if (autoUndoRecord)
                undoRedoMgr.Commit(); 
        }

        public void SendBackward(List<Figure> figs)
        {            
            // init undoRedo frame
            if (autoUndoRecord)
                undoRedoMgr.Start("Send Backward");
            // do send backward
            OrderFigures(figs);
            foreach (Figure f in figs)
                if (figures.Contains(f))
                {
                    int idx = figures.IndexOf(f);
                    if (idx > 0 && !figs.Contains(figures[idx - 1]))
                    {
                        figures.Remove(f);
                        figures.Insert(idx - 1, f);
                    }
                }
            // update
            DoSelectedFiguresChanged();
            UpdateViewers();
            // commit changes to undoRedoMgr
            if (autoUndoRecord)
                undoRedoMgr.Commit(); 
        }

        public bool CanSendBackward(List<Figure> figs)
        {
            foreach (Figure f in figs)
            {
                int idx = figures.IndexOf(f);
                if (idx > 0 && !figs.Contains(figures[idx - 1]))
                    return true;
            }
            return false;
        }

        public void BringForward(List<Figure> figs)
        {
            // init undoRedo frame
            if (autoUndoRecord)
                undoRedoMgr.Start("Bring Forward");
            // do bring forward
            OrderFigures(figs);
            for (int i = figs.Count - 1; i >= 0; i--)
            {
                Figure f = figs[i];
                if (figures.Contains(f))
                {
                    int idx = figures.IndexOf(f);
                    if (idx < figures.Count - 1 && !figs.Contains(figures[idx + 1]))
                    {
                        figures.Remove(f);
                        figures.Insert(idx + 1, f);
                    }
                }
            }
            // update
            DoSelectedFiguresChanged();
            UpdateViewers();
            // commit changes to undoRedoMgr
            if (autoUndoRecord)
                undoRedoMgr.Commit(); 
        }

        public bool CanBringForward(List<Figure> figs)
        {
            foreach (Figure f in figs)
            {
                int idx = figures.IndexOf(f);
                if (idx < figures.Count - 1 && !figs.Contains(figures[idx + 1]))
                    return true;
            }
            return false;
        }

        public void SetEraserSize(double size)
        {
            eraser.Size = size;
        }

        // Helper Functions //

        Figure HitTestFigures(DPoint pt, out DHitTest hitTest)
        {
            hitTest = DHitTest.None;
            // first hittest for selection chrome
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt);
                if (hitTest != DHitTest.None && hitTest != DHitTest.Body)
                    return f;
            }
            // now hittest for any part of the figure
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt);
                if (hitTest != DHitTest.None)
                    return f;
            }
            return null;
        }

        void ClearSelectedFiguresList()
        {
            foreach (Figure f in selectedFigures)
                f.Selected = false;
            selectedFigures.Clear();
        }

        void AddToSelected(Figure f)
        {
            f.Selected = true;
            selectedFigures.Add(f);
        }

        Figure HitTestSelect(DPoint pt, out DHitTest hitTest)
        {
            Figure f = HitTestFigures(pt, out hitTest);
            // update selected figures
            if (f != null)
            {
                if (!f.Selected)
                {
                    ClearSelectedFiguresList();
                    AddToSelected(f);
                    DoSelectedFiguresChanged();
                }
            }
            return f;
        }

        void OrderFigures(List<Figure> figs)
        {
            int[] idx = new int[figs.Count];
            for(int i = 0; i < figs.Count; i++)
                idx[i] = figures.IndexOf(figs[i]);
            Figure[] figs2 = figs.ToArray();
            Array.Sort(idx, figs2);
            figs = new List<Figure>(figs2);
        }

        void ClearCurrentFigure()
        {
            if (currentFigure != null)
            {
                // remove current figure if it was not sized by a mouse drag
                if (currentFigure.Width == 0 || currentFigure.Height == 0)
                    figures.Remove(currentFigure);
                // null currentfigure
                currentFigure = null;
            }
        }

        void UngroupFigure(GroupFigure gf)
        {
            // apply group properties (rotation etc) to child figures
            DPoint gcpt = gf.Rect.Center;
            foreach (Figure f in gf.ChildFigures)
            {
                // rotation
                DPoint fcpt = f.Rect.Center;
                DPoint rotpt = DGeom.RotatePoint(fcpt, gcpt, gf.Rotation);
                f.X += rotpt.X - fcpt.X;
                f.Y += rotpt.Y - fcpt.Y;
                f.Rotation += gf.Rotation;
                // alpha
                if (gf.UseRealAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha *= gf.Alpha;
            }
            // add group figures to figure list
            foreach (Figure f in gf.ChildFigures)
                figures.Add(f);
            // remove group
            figures.Remove(gf);
        }

        // Mouse Functions //

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
