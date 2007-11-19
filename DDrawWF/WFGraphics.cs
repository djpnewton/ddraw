using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DDraw.WinForms
{
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

        protected override void DisposeBitmap()
        {
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

    public class WFTextExtent : DTextExtent
    {
        public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
            Size sz = TextRenderer.MeasureText(text, new Font(fontName, (float)fontSize));
            return new DPoint(sz.Width, sz.Height);
        }
    }

    public class WFGraphics : DGraphics
    {
        public static void Init()
        {
            GraphicsHelper.Init(typeof(WFBitmap), typeof(WFGraphics), typeof(WFTextExtent));
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

        Color MakeColor(DColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        Color MakeColor(DColor color, double alpha)
        {
            return Color.FromArgb((int)(color.A * alpha), color.R, color.G, color.B);
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

        private Pen MakePen(Color color, DPenStyle penStyle)
        {
            Pen p = new Pen(color);
            p.DashStyle = MakeDashStyle(penStyle);
            return p;
        }

        private Pen MakePen(Color color, double strokeWidth, DPenStyle penStyle)
        {
            Pen p = new Pen(color, (float)strokeWidth);
            p.DashStyle = MakeDashStyle(penStyle);
            return p;
        }

        private DashStyle MakeDashStyle(DPenStyle penStyle)
        {
            switch (penStyle)
            {
                case DPenStyle.Solid:
                    return DashStyle.Solid;
                case DPenStyle.Dash:
                    return DashStyle.Dash;
                case DPenStyle.Dot:
                    return DashStyle.Dot;
            }
            return DashStyle.Solid;
        }

        private Matrix MakeMatrix(DMatrix matrix)
        {
            return new Matrix((float)matrix.A, (float)matrix.B, (float)matrix.C,
                (float)matrix.D, (float)matrix.E, (float)matrix.F);
        }
        
        // Drawing Functions //

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
            g.FillRectangle(new SolidBrush(MakeColor(color, alpha)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DPenStyle penStyle)
        {
            g.DrawRectangle(MakePen(MakeColor(color, alpha), strokeWidth, penStyle), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color)
        {
            g.DrawRectangle(new Pen(MakeColor(color)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            g.DrawRectangle(new Pen(MakeColor(color, alpha), (float)strokeWidth), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawRect(DRect rect, DColor color)
        {
            g.DrawRectangle(new Pen(MakeColor(color)), MakeRect(rect));
        }

        public override void DrawRect(DRect rect, DColor color, double alpha)
        {
            g.DrawRectangle(new Pen(MakeColor(color, alpha)), MakeRect(rect));
        }

        public override void DrawRect(DRect rect, DColor color, double alpha, DPenStyle penStyle)
        {
            g.DrawRectangle(MakePen(MakeColor(color, alpha), penStyle), MakeRect(rect));
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
            g.FillEllipse(new SolidBrush(MakeColor(color)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            g.FillEllipse(new SolidBrush(MakeColor(color, alpha)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void FillEllipse(DRect rect, DColor color)
        {
            g.FillEllipse(new SolidBrush(MakeColor(color)), MakeRect(rect));
        }

        public override void FillEllipse(DRect rect, DColor color, double alpha)
        {
            g.FillEllipse(new SolidBrush(MakeColor(color, alpha)), MakeRect(rect));
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color)
        {
            g.DrawEllipse(new Pen(MakeColor(color)), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            g.DrawEllipse(new Pen(MakeColor(color, alpha), (float)strokeWidth), (float)x, (float)y, (float)width, (float)height);
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
            g.DrawEllipse(new Pen(MakeColor(color)), MakeRect(rect));
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
            g.DrawEllipse(new Pen(MakeColor(color, alpha)), MakeRect(rect));
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
            g.DrawLine(new Pen(MakeColor(color)), (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha)
        {
            g.DrawLine(new Pen(MakeColor(color, alpha)), (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, DPenStyle penStyle)
        {
            g.DrawLine(MakePen(MakeColor(color), penStyle), (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DPenStyle penStyle)
        {
            g.DrawLine(MakePen(MakeColor(color, alpha), penStyle), (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
        }

        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth)
        {
            if (pts.Count > 1)
            {
                Pen p = new Pen(MakeColor(color, alpha), (float)strokeWidth);
                p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Flat);
                p.LineJoin = LineJoin.Round;
                g.DrawLines(p, MakePoints(pts));
            }
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
            g.DrawImage((Bitmap)bitmap.NativeBmp, new PointF((float)pt.X, (float)pt.Y));
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
            Bitmap bmp = (Bitmap)bitmap.NativeBmp;
            g.DrawImage(bmp, MakeRect(rect.Inflate(1, 1)),
                0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, MakeImageAttributesWithAlpha(alpha));
        }

        public override void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color)
        {
            g.DrawString(text, new Font(fontName, (float)fontSize), new SolidBrush(MakeColor(color)), new PointF((float)rect.X, (float)rect.Y));
        }

        public override void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color, double alpha)
        {
            g.DrawString(text, new Font(fontName, (float)fontSize), new SolidBrush(MakeColor(color, alpha)), new PointF((float)rect.X, (float)rect.Y));
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

        public override void Translate(DPoint offset)
        {
            g.TranslateTransform((float)offset.X, (float)offset.Y);
        }

        public override void ResetTransform()
        {
            g.ResetTransform();
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
    }
}
