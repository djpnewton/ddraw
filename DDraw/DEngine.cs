using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public enum DEditMode { None, Select, DrawPolyline, DrawRect, DrawEllipse };
    public delegate void DEditModeChangedHandler();
    public delegate void DebugMessageHandler(string msg);
    public delegate void SelectedFiguresHandler();

    public class DAuthorProperties
    {
        public DColor Fill;
        public DColor Stroke;
        public double StrokeWidth;
        public double Alpha;
        public string FontName;

        DEditMode editMode = DEditMode.None;
        public DEditMode EditMode
        {
            get { return editMode; }
            set
            {
                if (value != editMode)
                {
                    editMode = value;
                    if (EditModeChanged != null)
                        EditModeChanged();
                }
            }
        }

        public event DEditModeChangedHandler EditModeChanged;

        public DAuthorProperties(DColor fill, DColor stroke, double strokeWidth, double alpha, string fontName)
        {
            Fill = fill;
            Stroke = stroke;
            StrokeWidth = strokeWidth;
            Alpha = alpha;
            FontName = fontName;
        }

        public void ApplyPropertiesToFigure(Figure f)
        {
            if (f is IFillable)
                ((IFillable)f).Fill = Fill;
            if (f is IStrokeable)
            {
                ((IStrokeable)f).Stroke = Stroke;
                ((IStrokeable)f).StrokeWidth = StrokeWidth;
            }
            if (f is IAlphaBlendable)
                ((IAlphaBlendable)f).Alpha = Alpha;
            if (f is ITextable)
                ((ITextable)f).FontName = FontName;
        }
    }

    public class DEngine
    {
        protected List<Figure> figures = new List<Figure>();
        List<DViewer> viewers = new List<DViewer>();
        List<Figure> selectedFigures = new List<Figure>();
        public Figure[] SelectedFigures
        {
            get
            {
                Figure[] a = new Figure[selectedFigures.Count];
                selectedFigures.CopyTo(a);
                return a;
            }
        }
        Figure currentFigure = null;
        bool drawSelectionRect = false;
        SelectionFigure selectionRect;
        bool mouseDown = false;
        DPoint dragPt;
        double dragRot;
        DHitTest mouseHitTest;

        DAuthorProperties authorProps;

        public event DebugMessageHandler DebugMessage;
        public event SelectedFiguresHandler SelectedFiguresChanged;

        public DEngine(DAuthorProperties ap)
        {
            authorProps = ap;
            authorProps.EditModeChanged += new DEditModeChangedHandler(authorProps_EditModeChanged);
            selectionRect = new SelectionFigure(new DRect(), 0);
        }

        void authorProps_EditModeChanged()
        {
            ClearSelectedFiguresList();
            DoSelectedFiguresChanged();
            foreach (DViewer dv in viewers)
                dv.Update();
        }

        void dv_NeedRepaint(DViewer dv)
        {
            dv.Paint(figures, drawSelectionRect, selectionRect);
        }

        void dv_MouseDown(DViewer dv, DPoint pt)
        {
            MouseDown(dv, pt);
        }

        void dv_MouseMove(DViewer dv, DPoint pt)
        {
            MouseMove(dv, pt);
        }

        void dv_MouseUp(DViewer dv, DPoint pt)
        {
            MouseUp(dv, pt);
            foreach (DViewer dvOther in viewers)
                if (dvOther != dv)
                    dvOther.Update();
        }

        // Public Functions //

        public void AddFigure(Figure f)
        {
            figures.Add(f);
        }

        public void AddViewer(DViewer dv)
        {
            dv.NeedRepaint += new DPaintEventHandler(dv_NeedRepaint);
            dv.MouseDown += new DMouseEventHandler(dv_MouseDown);
            dv.MouseMove += new DMouseEventHandler(dv_MouseMove);
            dv.MouseUp += new DMouseEventHandler(dv_MouseUp);
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

        public void GroupFigures(Figure[] figs)
        {
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
        }

        public void UngroupFigure(GroupFigure gf)
        {
            // apply group properties (rotation etc) to child figures
            DPoint gcpt = gf.Rect.Center;
            foreach (Figure f in gf.Figures)
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
            // add group figures to figure list and selected list
            ClearSelected();
            foreach (Figure f in gf.Figures)
            {
                figures.Add(f);
                AddToSelected(f);
            }
            DoSelectedFiguresChanged();
            // remove group
            figures.Remove(gf);
            // update all viewers
            UpdateViewers();
        }

        // Helper Functions //

        Figure HitTestFigures(DPoint pt, out DHitTest hitTest)
        {
            hitTest = DHitTest.None;
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

        // Mouse Functions //

        protected void MouseDown(DViewer dv, DPoint pt)
        {
            // store mouse state
            mouseDown = true;
            switch (authorProps.EditMode)
            {
                case DEditMode.Select:
                    Figure f = HitTestFigures(pt, out mouseHitTest);
                    // update selected figures
                    if (f != null)
                    {
                        if (!f.Selected)
                        {
                            ClearSelectedFiguresList();
                            AddToSelected(f);
                            DoSelectedFiguresChanged();
                        }
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
                    break;
                case DEditMode.DrawPolyline:
                    // create DPoints object
                    DPoints pts = new DPoints();
                    pts.Add(pt);
                    // create PolylineFigure
                    currentFigure = new PolylineFigure(pts);
                    authorProps.ApplyPropertiesToFigure(currentFigure);
                    // add to list of figures
                    figures.Add(currentFigure);
                    break;
                case DEditMode.DrawRect:
                    // create RectFigure
                    currentFigure = new RectFigure(new DRect(pt.X, pt.Y, 0, 0), 0);
                    authorProps.ApplyPropertiesToFigure(currentFigure);
                    // add to list of figures
                    figures.Add(currentFigure);
                    // store drag pt for reference on mousemove event)
                    dragPt = pt;
                    break;
                case DEditMode.DrawEllipse:
                    // create EllipseFigure
                    currentFigure = new EllipseFigure(new DRect(pt.X, pt.Y, 0, 0), 0);
                    authorProps.ApplyPropertiesToFigure(currentFigure);
                    // add to list of figures
                    figures.Add(currentFigure);
                    // store drag pt for reference on mousemove event)
                    dragPt = pt;
                    break;
            }
            // update drawing
            dv.Update();
        }

        DPoint CalcDragDelta(DPoint pt)
        {
            return new DPoint(pt.X - dragPt.X, pt.Y - dragPt.Y);
        }

        DPoint CalcSizeDelta(DPoint pt, Figure f)
        {
            if (f.LockAspectRatio)
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

        DPoint CalcPosDeltaFromAngle(double angle, DPoint dSize)
        {
            // x/y modification for rotation
            if (angle == 0)
                return new DPoint(0, 0);
            angle = angle / 2;
            double sintheta = Math.Sin(angle);
            double costheta = Math.Cos(angle);
            double r1 = sintheta * -dSize.X;
            double r2 = sintheta * -dSize.Y;
            double modx = r1 * sintheta + r2 * costheta;
            double mody = -r1 * costheta + r2 * sintheta;
            return new DPoint(modx, mody);
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

        protected void MouseMove(DViewer dv, DPoint pt)
        {
            switch (authorProps.EditMode)
            {
                case DEditMode.Select:
                    if (mouseDown)
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
                    else
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
                    }
                    break;
                case DEditMode.DrawPolyline:
                    // set cursor to draw
                    dv.SetCursor(DCursor.Crosshair);
                    if (mouseDown)
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
                    break;
                case DEditMode.DrawRect:
                    // set cursor to draw
                    dv.SetCursor(DCursor.Crosshair);
                    if (mouseDown)
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
                    break;
                case DEditMode.DrawEllipse:
                    goto case DEditMode.DrawRect;
            }
        }

        protected void MouseUp(DViewer dv, DPoint pt)
        {
            mouseDown = false;
            switch (authorProps.EditMode)
            {
                case DEditMode.Select:
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
                    break;
                case DEditMode.DrawPolyline:
                    if (((PolylineFigure)currentFigure).Points.Count <= 1)
                        figures.Remove(currentFigure);
                    break;
                case DEditMode.DrawRect:
                    if (currentFigure.Rect.Width == 0 || currentFigure.Height == 0)
                        figures.Remove(currentFigure);
                    break;
                case DEditMode.DrawEllipse:
                    goto case DEditMode.DrawRect;
            }
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
