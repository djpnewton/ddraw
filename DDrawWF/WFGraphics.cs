using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;

namespace DDraw.WinForms
{
    public static class WFHelper
    {
        public static DColor MakeColor(Color color)
        {
            return new DColor(color.R, color.G, color.B, color.A);
        }

        public static Color MakeColor(DColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color MakeColor(DColor color, double alpha)
        {
            return Color.FromArgb((int)(color.A * alpha), color.R, color.G, color.B);
        }
    }

    public class WFBitmap : DBitmap
    {
        Bitmap bmp
        {
            get { return (Bitmap)nativeBmp; }
        }

        public WFBitmap(Bitmap bmp)
        {
            nativeBmp = bmp;
        }

        public WFBitmap(int width, int height)
            : base(width, height)
        { }

        public WFBitmap(string filename)
            : base(filename)
        { }

        public WFBitmap(Stream s)
            : base(s)
        { }

        protected override object MakeBitmap(int width, int height)
        {
            return new Bitmap(width, height);
        }

        protected override object LoadBitmap(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            object b = LoadBitmap(fs);
            fs.Dispose();
            return b;
        }

        protected override object LoadBitmap(Stream s)
        {
            // speed up bitmaps by converting to 32bppPArgb format (apparently this can 
            // then be accelerated by GDI). 
            // See - http://objectmix.com/dotnet/102271-most-common-gdi-question-2.html 
            //     - http://www.vgdotnet.com/forums/viewtopic.php?t=365
            Bitmap orignalBmp = new Bitmap(Bitmap.FromStream(s));
            Bitmap newBmp = new Bitmap(orignalBmp.Width, orignalBmp.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(newBmp))
                g.DrawImage(orignalBmp, new Rectangle(0, 0, orignalBmp.Width, orignalBmp.Height));
            orignalBmp.Dispose();
            return newBmp;
        }

        public override void Dispose()
        {
            if (nativeBmp != null)
                ((Bitmap)nativeBmp).Dispose();
        }

        public override int Width
        {
            get { return bmp.Width; }
        }

        public override int Height
        {
            get { return bmp.Height; }
        }

        public override void Save(string filename)
        {
            bmp.Save(filename);
        }
    }

    public class WFGraphics : DGraphics
    {
        public static void Init()
        {
            GraphicsHelper.Init(typeof(WFBitmap), typeof(WFGraphics));
        }

        Graphics g;

        public WFGraphics(Graphics g)
        {
            this.g = g;    
        }

        public WFGraphics(DBitmap bmp)
        {
            g = Graphics.FromImage((Bitmap)bmp.NativeBmp);
        }

        // Helper Functions //

        Brush MakeBrush(DFillStyle fillStyle, DColor color, double alpha)
        {
            switch (fillStyle)
            {
                case DFillStyle.ForwardDiagonalHatch:
                    return new HatchBrush(HatchStyle.ForwardDiagonal, WFHelper.MakeColor(color, alpha), Color.FromArgb(0, Color.Red));
                default:
                    return new SolidBrush(WFHelper.MakeColor(color, alpha)); ;
            }
        }

        Rectangle MakeRect(double x, double y, double width, double height)
        {
            return new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(width), Convert.ToInt32(height));
        }

        Rectangle MakeRect(DRect rect)
        {
            return new Rectangle(Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y), Convert.ToInt32(rect.Width), Convert.ToInt32(rect.Height));
        }

        PointF[] MakePoints(DPoints pts)
        {
            PointF[] result = new PointF[pts.Count];
            for (int i = 0; i < pts.Count; i++)
                result[i] = new PointF((float)pts[i].X, (float)pts[i].Y);
            return result;
        }

        private Pen MakePen(Color color, DStrokeStyle strokeStyle)
        {
            Pen p = new Pen(color);
            p.DashStyle = MakeDashStyle(strokeStyle);
            return p;
        }

        private Pen MakePen(Color color, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap)
        {
            Pen p = new Pen(color, (float)strokeWidth);
            p.DashStyle = MakeDashStyle(strokeStyle);
            p.LineJoin = MakeLineJoin(strokeJoin);
            p.SetLineCap(MakeLineCap(strokeCap), MakeLineCap(strokeCap), DashCap.Flat);
            return p;
        }

        private LineCap MakeLineCap(DStrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                case DStrokeCap.Butt:
                    return LineCap.Flat;
                case DStrokeCap.Round:
                    return LineCap.Round;
                case DStrokeCap.Square:
                    return LineCap.Square;
            }
            return LineCap.Flat;
        }

        private DashCap MakeDashCap(DStrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                case DStrokeCap.Butt:
                    return DashCap.Flat;
                case DStrokeCap.Round:
                    return DashCap.Round;
                case DStrokeCap.Square:
                    return DashCap.Flat;
            }
            return DashCap.Flat;
        }

        private LineJoin MakeLineJoin(DStrokeJoin strokeJoin)
        {
            switch (strokeJoin)
            {
                case DStrokeJoin.Mitre:
                    return LineJoin.Miter;
                case DStrokeJoin.Round:
                    return LineJoin.Round;
                case DStrokeJoin.Bevel:
                    return LineJoin.Bevel;
            }
            return LineJoin.Miter;
        }

        private DashStyle MakeDashStyle(DStrokeStyle strokeStyle)
        {
            switch (strokeStyle)
            {
                case DStrokeStyle.Solid:
                    return DashStyle.Solid;
                case DStrokeStyle.Dash:
                    return DashStyle.Dash;
                case DStrokeStyle.Dot:
                    return DashStyle.Dot;
                case DStrokeStyle.DashDot:
                    return DashStyle.DashDot;
                case DStrokeStyle.DashDotDot:
                    return DashStyle.DashDotDot;
            }
            return DashStyle.Solid;
        }

        private Matrix MakeMatrix(DMatrix matrix)
        {
            return new Matrix((float)matrix.A, (float)matrix.B, (float)matrix.C,
                (float)matrix.D, (float)matrix.E, (float)matrix.F);
        }
		
		public FontStyle MakeFontStyle(bool bold, bool italics, bool underline, bool strikethrough)
		{
			FontStyle res = FontStyle.Regular;
			if (bold)
				res = res | FontStyle.Bold;
			if (italics)
				res = res | FontStyle.Italic;
			if (underline)
				res = res | FontStyle.Underline;
			if (strikethrough)
				res = res | FontStyle.Strikeout;
			return res;
		}
        
        // Drawing Functions //

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
            g.FillRectangle(new SolidBrush(WFHelper.MakeColor(color, alpha)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha, DFillStyle fillStyle)
        {
            g.FillRectangle(MakeBrush(fillStyle, color, alpha), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin)
        {
            // see:
            // http://groups.google.com/group/microsoft.public.dotnet.framework.drawing/browse_thread/thread/c52a43702fccaab8/838a26535bf6e2e6?lnk=st&q=drawline+outofmemoryexception#838a26535bf6e2e6
            // http://www.codeprof.com/dev-archive/123/2-8-1234065.shtm
            try
            {
                g.DrawRectangle(MakePen(WFHelper.MakeColor(color, alpha), strokeWidth, strokeStyle, strokeJoin, DStrokeCap.Butt), (float)x, (float)y, (float)width, (float)height);
            }
            catch { }
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color)
        {
            DrawRect(x, y, width, height, color, 1, 1);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            DrawRect(x, y, width, height, color, alpha, strokeWidth, DStrokeStyle.Solid, DStrokeJoin.Mitre);
        }

        public override void DrawRect(DRect rect, DColor color)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha, DStrokeStyle strokeStyle)
        {
            g.DrawRectangle(MakePen(WFHelper.MakeColor(color, alpha), strokeStyle), MakeRect(rect));
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
            g.FillEllipse(new SolidBrush(WFHelper.MakeColor(color)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            g.FillEllipse(new SolidBrush(WFHelper.MakeColor(color, alpha)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void FillEllipse(DRect rect, DColor color)
        {
            g.FillEllipse(new SolidBrush(WFHelper.MakeColor(color)), MakeRect(rect));
        }

        public override void FillEllipse(DRect rect, DColor color, double alpha)
        {
            g.FillEllipse(new SolidBrush(WFHelper.MakeColor(color, alpha)), MakeRect(rect));
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color)
        {
            g.DrawEllipse(new Pen(WFHelper.MakeColor(color)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle)
        {
            g.DrawEllipse(MakePen(WFHelper.MakeColor(color, alpha), strokeWidth, strokeStyle, DStrokeJoin.Mitre, DStrokeCap.Butt), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
            g.DrawEllipse(new Pen(WFHelper.MakeColor(color)), MakeRect(rect));
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
            g.DrawEllipse(new Pen(WFHelper.MakeColor(color, alpha)), MakeRect(rect));
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
            DrawLine(pt1, pt2, color, 1);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha)
        {
            DrawLine(pt1, pt2, color, alpha, DStrokeStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, 1, strokeStyle);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, alpha, strokeStyle, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle, double strokeWidth, DStrokeCap strokeCap)
        {
            // see:
            // http://groups.google.com/group/microsoft.public.dotnet.framework.drawing/browse_thread/thread/c52a43702fccaab8/838a26535bf6e2e6?lnk=st&q=drawline+outofmemoryexception#838a26535bf6e2e6
            // http://www.codeprof.com/dev-archive/123/2-8-1234065.shtm
            try
            {
                g.DrawLine(MakePen(WFHelper.MakeColor(color, alpha), strokeWidth, strokeStyle, DStrokeJoin.Mitre, strokeCap), (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
            }
            catch { }
        }

        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round, DStrokeCap.Round);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap)
        {
            if (pts.Count > 1)
                g.DrawLines(MakePen(WFHelper.MakeColor(color, alpha), strokeWidth, strokeStyle, strokeJoin, strokeCap), MakePoints(pts));
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha)
        {
            if (pts.Count > 1)
                g.FillPolygon(MakeBrush(DFillStyle.Solid, color, alpha), MakePoints(pts));
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
            g.DrawImage((Bitmap)bitmap.NativeBmp, new PointF((float)pt.X, (float)pt.Y));
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt, double alpha)
        {
            DrawBitmap(bitmap, new DRect(pt.X, pt.Y, bitmap.Width, bitmap.Height), alpha);
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect)
        {
            g.DrawImage((Bitmap)bitmap.NativeBmp, MakeRect(rect.Inflate(1, 1)));
        }

        ImageAttributes MakeImageAttributesWithAlpha(double alpha)
        {
            // from http://www.codeproject.com/vcpp/gdiplus/AlphaBlending.asp
            ImageAttributes ia = new ImageAttributes();
            float[][] colorMatrixElements = { 
                new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, (float)alpha, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return ia;
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect, double alpha)
        {
            if (bitmap != null)
            {
                Bitmap bmp = (Bitmap)bitmap.NativeBmp;
                g.DrawImage(bmp, MakeRect(rect.Inflate(1, 1)),
                    0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, MakeImageAttributesWithAlpha(alpha));
            }
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color)
        {
            DrawText(text, fontName, fontSize, false, false, false, false, pt, color, 1);
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color, double alpha)
        {
            DrawText(text, fontName, fontSize, false, false, false, false, pt, color, alpha);
        }

        public override void DrawText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough, DPoint pt, DColor color, double alpha)
        {
            g.DrawString(text, new Font(fontName, (float)fontSize, MakeFontStyle(bold, italics, underline, strikethrough)), new SolidBrush(WFHelper.MakeColor(color, alpha)), new PointF((float)pt.X, (float)pt.Y), StringFormat.GenericDefault);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
            return MeasureText(text, fontName, fontSize, false, false, false, false);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough)
        {
            // create the font
            Font f = new Font(fontName, (float)fontSize, MakeFontStyle(bold, italics, underline, strikethrough));
            // measure the actual size of the text
            StringFormat sf = StringFormat.GenericDefault;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            SizeF sz = g.MeasureString(text, f, new PointF(0, 0), sf);
            sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, text.Length) });
            Region[] regs = g.MeasureCharacterRanges(text, f, new RectangleF(0, 0, sz.Width, sz.Height), sf);
            if (regs.Length > 0)
            {
                RectangleF r = regs[0].GetBounds(g);
                sz.Width = r.Left + r.Width;
            }
            // measure a new or empty line as if it had text on it
            if (text.EndsWith("\n") || text == "")
                sz.Height = g.MeasureString(string.Concat(text, "."), f).Height;
            return new DPoint(sz.Width, sz.Height);
        }

        public override DMatrix SaveTransform()
        {
            return new DMatrix(g.Transform.Elements[0], g.Transform.Elements[1], g.Transform.Elements[2],
                g.Transform.Elements[3], g.Transform.Elements[4], g.Transform.Elements[5]);
        }

        public override void LoadTransform(DMatrix matrix)
        {
            g.Transform = MakeMatrix(matrix);
        }

        public override void Scale(double sx, double sy)
        {
            g.ScaleTransform((float)sx, (float)sy);
        }

        public override void Rotate(double angle, DPoint center)
        {
            g.TranslateTransform((float)center.X, (float)center.Y);
            g.RotateTransform((float)(angle * 180 / Math.PI)); // convert from radians to degrees
            g.TranslateTransform((float)-center.X, (float)-center.Y);
        }

        public override void Translate(double tx, double ty)
        {
            g.TranslateTransform((float)tx, (float)ty);
        }

        public override void ResetTransform()
        {
            g.ResetTransform();
        }

        public override void Clip(DRect r)
        {
            g.SetClip(MakeRect(r));
        }

        public override void ResetClip()
        {
            g.ResetClip();
        }

        public override DCompositingMode CompositingMode
        {
            get
            {
                if (g.CompositingMode == System.Drawing.Drawing2D.CompositingMode.SourceCopy)
                    return DCompositingMode.SourceCopy;
                else
                    return DCompositingMode.SourceOver;
            }
            set
            {
                if (value == DCompositingMode.SourceOver)
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                else
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            }
        }

        public override bool AntiAlias
        {
            get { return g.SmoothingMode == SmoothingMode.AntiAlias; }
            set
            {
                if (value)
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                else
                    g.SmoothingMode = SmoothingMode.None;
            }
        }

        Stack<GraphicsState> gsStack = new Stack<GraphicsState>();

        public override void Save()
        {
            gsStack.Push(g.Save());
        }

        public override void Restore()
        {
            g.Restore(gsStack.Pop());
        }

        public override void Dispose()
        {
            g.Dispose();
        }
    }
}
