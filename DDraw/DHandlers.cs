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
        UndoRedo<bool> _customBackgroundFigure = new UndoRedo<bool>(false);
        public bool CustomBackgroundFigure
        {
            get { return _customBackgroundFigure.Value; }
            set
            {
                if (_customBackgroundFigure.Value != value)
                    _customBackgroundFigure.Value = value;
            }
        }

        UndoRedo<BackgroundFigure> _backgroundFigure = new UndoRedo<BackgroundFigure>(new BackgroundFigure());
        public BackgroundFigure BackgroundFigure
        {
            get { return _backgroundFigure.Value; }
            set 
            { 
                if (_backgroundFigure.Value != value)
                    _backgroundFigure.Value = value; 
            }
        }
        SelectionFigure selectionFigure = new SelectionFigure(new DRect());
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
            figures.ItemAdded += new ItemAddedDelegate<Figure>(figures_ItemAdded);

            BackgroundFigure.Fill = DColor.White;
            DPoint sz = PageTools.FormatToSize(PageFormat.Default);
            BackgroundFigure.Width = sz.X;
            BackgroundFigure.Height = sz.Y;
        }

        void figures_ItemAdded(Figure item)
        {
            if (AddedFigure != null)
                AddedFigure(null, item, addedFromHsm);
            addedFromHsm = false;
        }

        // public methods //

        public event AddedFigureHandler AddedFigure;
        bool addedFromHsm = false;

        public void Add(Figure f, bool fromHsm)
        {
            addedFromHsm = fromHsm;
            figures.Add(f);
        }

        public void Insert(Figure f, Figure before)
        {
            figures.Insert(figures.IndexOf(before), f);
        }

        public void Remove(Figure f)
        {
            figures.Remove(f);
        }

        public void ClearSelected()
        {
            if (selectedFigures.Count > 0)
            {
                ClearSelectedFiguresList();
                DoSelectedFiguresChanged();
            }
        }

        public void SelectFigures(IList<Figure> figs, bool add)
        {
            // check that there is any change to selected figures
            bool changes = false;
            if (add)
            {
                foreach (Figure f in figs)
                    if (!selectedFigures.Contains(f))
                    {
                        changes = true;
                        break;
                    }
            }
            else
            {
                if (figs.Count != selectedFigures.Count)
                    changes = true;
                else
                    foreach (Figure f in selectedFigures)
                        if (!figs.Contains(f))
                        {
                            changes = true;
                            break;
                        }
            }
            if (changes)
            {
                // clear if replacing selected figures
                if (!add)
                    ClearSelectedFiguresList();
                // add new selected figures
                foreach (Figure f in figs)
                    AddToSelected(f);
                DoSelectedFiguresChanged();
            }
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
                Add(gf, false);
                // change selected figures to the group
                ClearSelectedFiguresList();
                AddToSelected(gf);
                DoSelectedFiguresChanged();
                // remove child figures from figure list
                foreach (Figure f in figs)
                {
                    figures.Remove(f);
                    f.MouseOver = false;
                }
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
                {
                    AddToSelected(f);
                    f.MouseOver = false;
                }
                DoSelectedFiguresChanged();
            }
        }

        public Figure HitTestFigures(DPoint pt, out DHitTest hitTest, List<Figure> children, out IGlyph glyph)
        {
            glyph = null;
            hitTest = DHitTest.None;
            // first hittest for selection chrome
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt, children, out glyph);
                if (hitTest != DHitTest.None && hitTest != DHitTest.Body /*&& hitTest != DHitTest.Glyph*/)
                    return f;
            }
            // now hittest for any part of the figure
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                Figure f = figures[i];
                hitTest = f.HitTest(pt, null /* only send children param once */, out glyph);
                if (hitTest != DHitTest.None)
                    return f;
            }
            return null;
        }

        public Figure HitTestSelect(DPoint pt, out DHitTest hitTest, List<Figure> children, out IGlyph glyph, bool addToggle)
        {
            Figure f = HitTestFigures(pt, out hitTest, children, out glyph);
            // update selected figures
            if (f != null)
            {
                if (!f.Selected)
                {
                    if (!addToggle)
                        ClearSelectedFiguresList();
                    AddToSelected(f);
                    DoSelectedFiguresChanged();
                }
                else if (addToggle)
                {
                    RemoveFromSelected(f);
                    DoSelectedFiguresChanged();
                }
            }
            return f;
        }

        public void UngroupFigure(GroupFigure gf)
        {
            // apply group properties (rotation etc) to child figures
            DPoint grpCtr = gf.Rect.Center;
            foreach (Figure f in gf.ChildFigures)
            {
                // flip
                DPoint ctr = f.Rect.Center;
                if (gf.FlipX)
                {
                    f.FlipX = !f.FlipX;
                    f.X -= (ctr.X - grpCtr.X) * 2;
                    // flip rotation
                    if (f.Rotation != 0) f.Rotation = 2 * Math.PI - f.Rotation;
                }
                if (gf.FlipY)
                {
                    f.FlipY = !f.FlipY;
                    f.Y -= (ctr.Y - grpCtr.Y) * 2;
                    // flip rotation
                    if (f.Rotation != 0) f.Rotation = 2 * Math.PI - f.Rotation;
                }
                // rotation
                if (gf.Rotation != 0)
                {
                    ctr = f.Rect.Center;
                    DPoint rotpt = DGeom.RotatePoint(ctr, grpCtr, gf.Rotation);
                    f.X += rotpt.X - ctr.X;
                    f.Y += rotpt.Y - ctr.Y;
                    f.Rotation += gf.Rotation;
                }
                // alpha
                if (gf.UseRealAlpha && f is IAlphaBlendable)
                    ((IAlphaBlendable)f).Alpha *= gf.Alpha;
            }
            // add group figures to figure list
            foreach (Figure f in gf.ChildFigures)
                Add(f, false);
            // remove group
            figures.Remove(gf);
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
            if (!selectedFigures.Contains(f))
                selectedFigures.Add(f);
        }

        void RemoveFromSelected(Figure f)
        {
            f.Selected = false;
            if (selectedFigures.Contains(f))
                selectedFigures.Remove(f);
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


