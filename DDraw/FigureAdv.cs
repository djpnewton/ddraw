using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using DejaVu;

namespace DDraw
{
    public delegate void EditFinishedHandler(IEditable sender);
    
    public interface IEditable
    {
        void StartEdit();
        void EndEdit();
        event EditFinishedHandler EditFinished; 
        void MouseDown(DViewer dv, DMouseButton btn, DPoint pt);
        void MouseMove(DViewer dv, DPoint pt);
        void MouseUp(DViewer dv, DMouseButton btn, DPoint pt);
        void DoubleClick(DViewer dv, DPoint pt);
        void KeyPress(DViewer dv, int k);
        string EditAttrsToString();
        void SetEditAttrsFromString(string s);
    }
    
    public class ClockFigure : RectbaseFigure, IEditable
    {
        bool editing = false;
        double origRotation;
        
        UndoRedo<double> firstHandAngle = new UndoRedo<double>(0);
        bool editingFirstHand = false;
        UndoRedo<double> secondHandAngle = new UndoRedo<double>(Math.PI / 4);
        bool editingSecondHand = false;

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            if (editing && DGeom.PointInRect(pt, GetClockRect()))
                return DHitTest.Body;
            else if (DGeom.PointInEllipse(pt, GetClockRect()))
                return DHitTest.Body;
            return DHitTest.None;
        }
        
        public void StartEdit ()
        {
            editing = true;
            origRotation = Rotation;
            Rotation = 0;
        }
        
        public void EndEdit ()
        {
            editing = false;
            Rotation = origRotation;
        }

        public event EditFinishedHandler EditFinished;
        
        public void MouseDown (DViewer dv, DMouseButton btn, DPoint pt)
        {
            if (editing && HitTest(pt) == DHitTest.Body)
            {
                DRect r = GetClockRect();
                DPoint fhp = FirstHandPoint(r);
                DPoint shp = SecondHandPoint(r);
                if (DGeom.DistBetweenTwoPts(pt, fhp) <= DGeom.DistBetweenTwoPts(pt, shp))
                {
                    editingFirstHand = true;
                    firstHandAngle.Value = DGeom.AngleBetweenPoints(r.Center, pt);
                }
                else
                {
                    editingSecondHand = true;
                    secondHandAngle.Value = DGeom.AngleBetweenPoints(r.Center, pt);
                }
                dv.Update(r);                          
            }
            else
                DoEditFinished();
        }

        public void MouseMove (DViewer dv, DPoint pt)
        {
            if (editingFirstHand)
            {
                firstHandAngle.Value = DGeom.AngleBetweenPoints(GetClockRect().Center, pt);
                dv.Update(GetClockRect());
            }
            else if (editingSecondHand)
            {
                secondHandAngle.Value = DGeom.AngleBetweenPoints(GetClockRect().Center, pt);
                dv.Update(GetClockRect());
            }
            else if (editing && HitTest(pt) == DHitTest.Body) 
                dv.SetCursor(DCursor.Crosshair);
            else
                dv.SetCursor(DCursor.Default);
        }

        public void MouseUp (DViewer dv, DMouseButton btn, DPoint pt)
        {
            editingFirstHand = false;
            editingSecondHand = false;
        }

        public void DoubleClick (DViewer dv, DPoint pt)
        {
            
        }
        
        public void KeyPress (DViewer dv, int k)
        {
            switch ((DKeys)k)
            {
                case DKeys.Enter:
                    DoEditFinished();
                    break;
                case DKeys.Escape:
                    goto case DKeys.Enter;
            }
        }

        DRect GetClockRect()
        {
            DRect r = Rect;
            if (r.Width == r.Height)
                return r;
            if (r.Width > r.Height)
                return new DRect(r.X + r.Width / 2 - r.Height / 2, r.Y, r.Height, r.Height);
            else
                return new DRect(r.X, r.Y + r.Height / 2 - r.Width / 2, r.Width, r.Width);
        }
            
        DPoint FirstHandPoint(DRect r)
        { 
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y), r.Center, firstHandAngle.Value);    
        }
            
        DPoint SecondHandPoint(DRect r)
        {
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y + r.Height / 6), r.Center, secondHandAngle.Value);
        }

        protected override void PaintBody (DGraphics dg)
        {
            DRect r = GetClockRect();
            double alpha = Alpha;
            if (editing)
            {
                dg.FillRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, DFillStyle.ForwardDiagonalHatch);
                alpha = 1;
            }
            dg.FillEllipse(r, DColor.White, alpha);
            dg.DrawEllipse(r, DColor.Black, alpha);
            dg.DrawText("12", "Arial", 8, new DPoint(r.Center.X - 8, r.Y), DColor.Black, alpha);
            dg.DrawLine(r.Center, FirstHandPoint(r), DColor.Red, alpha);
            dg.DrawLine(r.Center, SecondHandPoint(r), DColor.Blue, alpha);
        }
        
        void DoEditFinished()
        {
            if (EditFinished != null)
                EditFinished(this);
        }

        public string EditAttrsToString()
        {
            return string.Format("{0},{1}", firstHandAngle.Value, secondHandAngle.Value);
        }

        public void SetEditAttrsFromString(string s)
        {
            string[] parts = s.Split(',');
            if (parts.Length == 2)
            {
                double first, second;
                double.TryParse(parts[0], out first);
                double.TryParse(parts[1], out second);
                firstHandAngle.Value = first;
                secondHandAngle.Value = second;
            }
        }
    }
}