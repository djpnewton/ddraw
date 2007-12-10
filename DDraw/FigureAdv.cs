using System;
using System.Collections.Generic;
using System.Text;

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
        void KeyPress(DViewer dv, char k);
    }
    
    public class ClockFigure : RectbaseFigure, IEditable
    {
        bool editing = false;
        double origRotation;
        
        double firstHandAngle = 0;
        bool editingFirstHand = false;
        double secondHandAngle = Math.PI / 4;
        bool editingSecondHand = false;
        
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
                DRect r = Rect;
                DPoint fhp = FirstHandPoint(r);
                DPoint shp = SecondHandPoint(r);
                if (DGeom.DistBetweenTwoPts(pt, fhp) <= DGeom.DistBetweenTwoPts(pt, shp))
                {
                    editingFirstHand = true;
                    firstHandAngle = DGeom.AngleBetweenPoints(r.Center, pt);
                }
                else
                {
                    editingSecondHand = true;
                    secondHandAngle = DGeom.AngleBetweenPoints(r.Center, pt);
                }
                dv.Update(Rect);                          
            }
            else
                DoEditFinished();
        }

        public void MouseMove (DViewer dv, DPoint pt)
        {
            if (editingFirstHand)
            {
                firstHandAngle = DGeom.AngleBetweenPoints(Rect.Center, pt);
                dv.Update(Rect);
            }
            else if (editingSecondHand)
            {
                secondHandAngle = DGeom.AngleBetweenPoints(Rect.Center, pt);
                dv.Update(Rect);
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
        
        public void KeyPress (DViewer dv, char k)
        {
            switch (k)
            {
                case '\r':
                    DoEditFinished();
                    break;
                case (char)27: // esc
                    goto case '\r';
            }
        }
            
        DPoint FirstHandPoint(DRect r)
        { 
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y), r.Center, firstHandAngle);    
        }
            
        DPoint SecondHandPoint(DRect r)
        {
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y + r.Height / 6), r.Center, secondHandAngle);
        }

        protected override void PaintBody (DGraphics dg)
        {   
            DRect r = Rect;
            if (editing)
                dg.FillRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, DFillStyle.ForwardDiagonalHatch);
            dg.FillEllipse(r, DColor.White, Alpha);
            dg.DrawEllipse(r, DColor.Black, Alpha);
            dg.DrawText("12", "Arial", 8, new DPoint(r.Center.X - 8, r.Y), DColor.Black);
            dg.DrawLine(r.Center, FirstHandPoint(r), DColor.Red);
            dg.DrawLine(r.Center, SecondHandPoint(r), DColor.Blue);
        }
        
        void DoEditFinished()
        {
            if (EditFinished != null)
                EditFinished(this);
        }
    }
}