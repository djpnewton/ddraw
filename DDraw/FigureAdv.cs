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
        void MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt);
        void MouseMove(DTkViewer dv, DPoint pt);
        void MouseUp(DTkViewer dv, DMouseButton btn, DPoint pt);
        void DoubleClick(DTkViewer dv, DPoint pt);
        void KeyPress(DTkViewer dv, int k);
        string EditAttrsToString();
        void SetEditAttrsFromString(string s);
    }

#if BEHAVIOURS
    public interface IBehaviours
    {
        DBehaviour MouseOverBehaviour
        {
            get;
            set;
        }
    }

    public struct DBehaviour
    {
        public bool SetFill;
        public DColor Fill;
        public bool SetStroke;
        public DColor Stroke;
        public bool SetAlpha;
        public double Alpha; 
    }
#endif
    
    public class ClockFigure : RectFigure, IEditable
    {
        bool editing = false;
        double origRotation, origAlpha;
        bool origFlipX, origFlipY;
        
        UndoRedo<double> firstHandAngle = new UndoRedo<double>(0);
        bool editingFirstHand = false;
        UndoRedo<double> secondHandAngle = new UndoRedo<double>(Math.PI / 4);
        bool editingSecondHand = false;

        public override bool LockAspectRatio
        {
            get { return true; }
        }

        protected override DHitTest BodyHitTest(DPoint pt, List<Figure> children)
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
            origAlpha = Alpha;
            Alpha = 1;
            origFlipX = FlipX;
            origFlipY = FlipY;
            FlipX = false;
            FlipY = false;
        }
        
        public void EndEdit ()
        {
            editing = false;
            Rotation = origRotation;
            Alpha = origAlpha;
            FlipX = origFlipX;
            FlipY = origFlipY;
        }

        public event EditFinishedHandler EditFinished;

        public void MouseDown(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            IGlyph glyph;
            if (editing && HitTest(pt, null, out glyph) == DHitTest.Body)
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

        public void MouseMove(DTkViewer dv, DPoint pt)
        {
            IGlyph glyph;
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
            else if (editing && HitTest(pt, null, out glyph) == DHitTest.Body) 
                dv.SetCursor(DCursor.Crosshair);
            else
                dv.SetCursor(DCursor.Default);
        }

        public void MouseUp(DTkViewer dv, DMouseButton btn, DPoint pt)
        {
            editingFirstHand = false;
            editingSecondHand = false;
        }

        public void DoubleClick(DTkViewer dv, DPoint pt)
        {
            
        }

        public void KeyPress(DTkViewer dv, int k)
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
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y + r.Height / 12 + SwHalf), r.Center, firstHandAngle.Value);    
        }
            
        DPoint SecondHandPoint(DRect r)
        {
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y + r.Height / 6 + SwHalf), r.Center, secondHandAngle.Value);
        }

        DPoint TextPoint(DRect r, int num)
        {
            return DGeom.RotatePoint(new DPoint(r.Center.X, r.Y + r.Height / 24 + SwHalf), r.Center, (num / 12.0) * (2 * Math.PI));
        }

        protected override void PaintBody (DGraphics dg)
        {
#if BEHAVIOURS
            // select paint properties
            DColor Fill = this.Fill; ;
            DColor Stroke = this.Stroke;
            double Alpha = this.Alpha;
            if (MouseOver)
            {
                if (MouseOverBehaviour.SetFill)
                    Fill = MouseOverBehaviour.Fill;
                if (MouseOverBehaviour.SetStroke)
                    Stroke = MouseOverBehaviour.Stroke;
                if (MouseOverBehaviour.SetAlpha)
                    Alpha = MouseOverBehaviour.Alpha;
            }
#endif
            // do painting
            DRect r = GetClockRect();
            const int baseSize = 100;
            if (editing)
                dg.FillRect(r.X, r.Y, r.Width, r.Height, DColor.Black, 1, DFillStyle.ForwardDiagonalHatch);
            dg.FillEllipse(r, Fill, Alpha);
            dg.DrawEllipse(r.X, r.Y, r.Width, r.Height, Stroke, Alpha, StrokeWidth, StrokeStyle);
            double offset = 0.5 * Width / baseSize;
            string[] nums = new string[] { "12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
            string font = "Arial"; 
            double fontSz = 6 * Width / baseSize;
            if (fontSz <= 0) fontSz = 6;
            for (int i = 0; i < nums.Length; i++)
            {
                string text = nums[i];
                DPoint textPos = TextPoint(r, i);
                DPoint textSz = dg.MeasureText(text, font, fontSz);
                dg.Save();
                dg.Translate(offset, offset);
                dg.DrawText(text, font, fontSz, new DPoint(textPos.X - textSz.X / 2, textPos.Y - textSz.Y / 2), DColor.Black, Alpha);
                dg.Restore();
                dg.DrawText(text, font, fontSz, new DPoint(textPos.X - textSz.X / 2, textPos.Y - textSz.Y / 2), DColor.White, Alpha);
            }
            double handWidth = 3 * Width / baseSize;
            dg.Save();
            dg.Translate(offset, offset);
            dg.DrawLine(r.Center, FirstHandPoint(r), DColor.Black, Alpha, DStrokeStyle.Solid, handWidth, DStrokeCap.Round);
            dg.DrawLine(r.Center, SecondHandPoint(r), DColor.Black, Alpha, DStrokeStyle.Solid, handWidth, DStrokeCap.Round);
            dg.Restore();
            dg.DrawLine(r.Center, FirstHandPoint(r), DColor.Red, Alpha, DStrokeStyle.Solid, handWidth, DStrokeCap.Round);
            dg.DrawLine(r.Center, SecondHandPoint(r), DColor.Blue, Alpha, DStrokeStyle.Solid, handWidth, DStrokeCap.Round);
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

    public class TextBoxFigure : RectFigure, ITextable
    {
        public override double MinSize
        {
            get { return 5; }
        }

        TextFigure tf;

        public TextBoxFigure()
        {
            tf = new TextFigure();
            tf.Text = "hello";
            WrapText = true;
            tf.Fill = DColor.Black;
        }

        protected override void PaintBody(DGraphics dg)
        {
            base.PaintBody(dg);
            dg.Save();
            dg.Clip(Rect);
            DPoint offset = TextOffset;
            dg.Translate(X + offset.X, Y + offset.Y);
            tf.Paint(dg);
            dg.Restore();
        }

        public override double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                tf.WrapThreshold = value;
            }
        }

        #region ITextable members
        public string Text
        {
            get { return tf.Text; }
            set { tf.Text = value; }
        }
        public bool HasText
        {
            get { return tf.HasText; }
        }
        public string FontName
        {
            get { return tf.FontName; }
            set { tf.FontName = value; }
        }
        public double FontSize
        {
            get { return tf.FontSize; }
            set 
            { 
                tf.FontSize = value;
                tf.WrapFontSize = value;
            }
        }
        public bool Bold
        {
            get { return tf.Bold; }
            set { tf.Bold = value; }
        }
        public bool Italics
        {
            get { return tf.Italics; }
            set { tf.Italics = value; }
        }
        public bool Underline
        {
            get { return tf.Underline; }
            set { tf.Underline = value; }
        }
        public bool Strikethrough
        {
            get { return tf.Strikethrough; }
            set { tf.Strikethrough = value; }
        }
        public DPoint TextOffset
        {
            get { return new DPoint(Width / 2 - tf.Width / 2, Height / 2 - tf.Height / 2); }
        }
        public bool WrapText
        {
            get { return tf.WrapText; }
            set { tf.WrapText = value; }
        }
        public double WrapThreshold
        {
            get { return tf.WrapThreshold; }
            set
            {
                if (value >= MinSize)
                    Width = value;
                else
                    Width = MinSize;
            }
        }
        public double WrapFontSize
        {
            get { return tf.WrapFontSize; }
            set { }
        }
        public double WrapLength
        {
            get { return tf.WrapLength; }
        }
        public string WrappedText
        {
            get { return tf.WrappedText; }
        }
        #endregion

        void UpdateSize()
        { }
    }
}