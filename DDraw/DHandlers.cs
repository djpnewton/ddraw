using System;
using System.Collections.Generic;
using System.Text;

using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public delegate void SelectedFiguresHandler();

    public class DFigureHandler
    {
        UndoRedo<BackgroundFigure> _backgroundFigure = new UndoRedo<BackgroundFigure>();
        public BackgroundFigure BackgroundFigure
        {
            get { return _backgroundFigure.Value; }
            set 
            { 
                if (_backgroundFigure.Value != value)
                    _backgroundFigure.Value = value; 
            }
        }
        SelectionFigure selectionFigure = new SelectionFigure(new DRect(), 0);
        public SelectionFigure SelectionFigure
        {
            get { return selectionFigure; }
        }
        EraserFigure eraserFigure = new EraserFigure(10);
        public EraserFigure EraserFigure
        {
            get { return eraserFigure; }
        }

        protected UndoRedoList<Figure> figures = new UndoRedoList<Figure>();
        public UndoRedoList<Figure> Figures
        {
            get { return figures; }
        }

        public event SelectedFiguresHandler SelectedFiguresChanged;
        List<Figure> selectedFigures = new List<Figure>();
        public List<Figure> SelectedFigures
        {
            get { return selectedFigures; }
        }

        public DFigureHandler()
        {
            BackgroundFigure = new BackgroundFigure();
            BackgroundFigure.Fill = DColor.White;
            BackgroundFigure.Width = PageTools.DefaultPageWidth;
            BackgroundFigure.Height = PageTools.DefaultPageHeight;
        }

        // public methods //

        public event AddedFigureHandler AddedFigure;

        public void Add(Figure f)
        {
            figures.Add(f);
            if (AddedFigure != null)
                AddedFigure(null, f);
        }

        public void Insert(Figure f, Figure before)
        {
            figures.Insert(figures.IndexOf(before), f);
        }

        public void Remove(Figure f)
        {
            figures.Remove(f);
        }

        public void Select(IList<Figure> figs)
        {
            ClearSelectedFiguresList();
            foreach (Figure f in figs)
                AddToSelected(f);
            DoSelectedFiguresChanged();
        }

        public void ClearSelected()
        {
            ClearSelectedFiguresList();
            DoSelectedFiguresChanged();
        }

        public void SelectFigures(List<Figure> figs)
        {
            ClearSelectedFiguresList();
            foreach (Figure f in figs)
                AddToSelected(f);
            DoSelectedFiguresChanged();
        }

        public void SendToBack(List<Figure> figs)
        {
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
        }

        public void BringToFront(List<Figure> figs)
        {
            // do bring to front
            OrderFigures(figs);
            foreach (Figure f in figs)
                if (figures.Contains(f))
                {
                    figures.Remove(f);
                    figures.Add(f);
                }
        }

        public void SendBackward(List<Figure> figs)
        {
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

        public bool CanGroupFigures(List<Figure> figs)
        {
            return figs.Count > 1;
        }

        public void GroupFigures(List<Figure> figs)
        {
            if (CanGroupFigures(figs))
            {
                // make group
                GroupFigure gf = new GroupFigure(figs);
                Add(gf);
                // change selected figures to the group
                ClearSelectedFiguresList();
                AddToSelected(gf);
                DoSelectedFiguresChanged();
                // remove child figures from figure list
                foreach (Figure f in figs)
                    figures.Remove(f);
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
                // perform ungroup
                UngroupFigure(gf);
                // add group figures to selected list
                ClearSelected();
                foreach (Figure f in gf.ChildFigures)
                    AddToSelected(f);
                DoSelectedFiguresChanged();
            }
        }

        public Figure HitTestFigures(DPoint pt, out DHitTest hitTest, out IGlyph glyph)
        {
            glyph = null;
            hitTest = DHitTest.None;
            // first hittest for selection chrome
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt, out glyph);
                if (hitTest != DHitTest.None && hitTest != DHitTest.Body)
                    return f;
            }
            // now hittest for any part of the figure
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt, out glyph);
                if (hitTest != DHitTest.None)
                    return f;
            }
            return null;
        }

        public Figure HitTestSelect(DPoint pt, out DHitTest hitTest, out IGlyph glyph)
        {
            Figure f = HitTestFigures(pt, out hitTest, out glyph);
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

        public void UngroupFigure(GroupFigure gf)
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
                if (gf.UseGroupAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha *= gf.Alpha;
            }
            // add group figures to figure list
            foreach (Figure f in gf.ChildFigures)
                Add(f);
            // remove group
            figures.Remove(gf);
        }

        public void SetBackgroundFigureSize(DPoint size)
        {
            BackgroundFigure.Width = size.X;
            BackgroundFigure.Height = size.Y;
        }

        public void SetEraserSize(double size)
        {
            eraserFigure.Size = size;
        }

        // helper methods //

        void OrderFigures(List<Figure> figs)
        {
            int[] idx = new int[figs.Count];
            for (int i = 0; i < figs.Count; i++)
                idx[i] = figures.IndexOf(figs[i]);
            Figure[] figs2 = figs.ToArray();
            Array.Sort(idx, figs2);
            figs = new List<Figure>(figs2);
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

        void DoSelectedFiguresChanged()
        {
            if (SelectedFiguresChanged != null)
                SelectedFiguresChanged();
        }
    }

    public class DViewerHandler
    {
        List<DTkViewer> viewers = new List<DTkViewer>();

        public event DPaintEventHandler NeedRepaint;
        public event DMouseButtonEventHandler MouseDown;
        public event DMouseMoveEventHandler MouseMove;
        public event DMouseButtonEventHandler MouseUp;
        public event DMouseMoveEventHandler DoubleClick;
        public event DKeyEventHandler KeyDown;
        public event DKeyPressEventHandler KeyPress;
        public event DKeyEventHandler KeyUp;

        // public methods //

        public void Add(DTkViewer dv)
        {
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

        public void Remove(DTkViewer dv)
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

        public void Update()
        {
            foreach (DTkViewer dv in viewers)
                dv.Update();
        }

        public void SetPageSize(DPoint size)
        {
            foreach (DTkViewer dv in viewers)
                dv.SetPageSize(size);
        }

        // viewer events

        void dv_NeedRepaint(DTkViewer dv, DGraphics dg)
        {
            if (NeedRepaint != null)
                NeedRepaint(dv, dg);
        }

        void dv_MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (MouseDown != null)
                MouseDown(dv, btn, pt);
        }

        void dv_MouseMove(DTkViewer dv, DPoint pt)
        {
            if (MouseMove != null)
                MouseMove(dv, pt);
        }

        void dv_MouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            if (MouseUp != null)
                MouseUp(dv, btn, pt);
            foreach (DTkViewer dvOther in viewers)
                if (dvOther != dv)
                    dvOther.Update();
        }

        void dv_DoubleClick(DTkViewer dv, DPoint pt)
        {
            if (DoubleClick != null)
                DoubleClick(dv, pt);
        }

        void dv_KeyDown(DTkViewer dv, DKey k)
        {
            if (KeyDown != null)
                KeyDown(dv, k);
        }

        void dv_KeyPress(DTkViewer dv, int k)
        {
            if (KeyPress != null)
                KeyPress(dv, k);
        }

        void dv_KeyUp(DTkViewer dv, DKey k)
        {
            if (KeyUp != null)
                KeyUp(dv, k);
        }
    }
}


