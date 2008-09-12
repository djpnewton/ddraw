using System;
using System.IO;
using System.Runtime.InteropServices;

using Cairo;
using FreeImageAPI;
using DDraw;

namespace DDrawCairo
{
    public abstract class CairoBitmap : DBitmap
    {
        // for holding the data sent to cairo_image_surface_create_for_data (http://cairographics.org/manual/cairo-Image-Surfaces.html#cairo-image-surface-create-for-data)
        protected GCHandle _gc_h_surface_data_buffer;

        protected ImageSurface surface
        {
            get { return (ImageSurface)nativeBmp; }
        }

        public CairoBitmap()
        { }

        public CairoBitmap(ImageSurface surf)
        {
            nativeBmp = surf;
        }

        public CairoBitmap(int width, int height) : base(width, height)
        { }

        public CairoBitmap(string filename) : base(filename)
        { }

        public CairoBitmap(Stream s) : base(s)
        { }

        protected override object MakeBitmap(int width, int height)
        {
            return new ImageSurface(Format.Argb32, width, height);
        }
        
        protected override object LoadBitmap(Stream s)
        {
            // init FreeImage
            FreeImage.Initialize(true);
            // copy data to FreeImage memory
            byte[] buf = new byte[s.Length];
            s.Read(buf, 0, (int)s.Length);
            GCHandle gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr bufPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
            uint hmem = FreeImage.OpenMemory(bufPtr, buf.Length);
            gch.Free();
            // copy FreeImage pixels to cairo image surface
            FREE_IMAGE_FORMAT fif = FreeImage.GetFileTypeFromMemory(hmem, buf.Length);
            uint fiBitmap = FreeImage.LoadFromMemory(fif, hmem, 0);
            uint width = FreeImage.GetWidth(fiBitmap);
            uint height = FreeImage.GetHeight(fiBitmap);
            uint bpp = FreeImage.GetBPP(fiBitmap);
            if (bpp != 32)
            {
                uint fiBitmap2 = FreeImage.ConvertTo32Bits(fiBitmap);
                FreeImage.Unload(fiBitmap);
                fiBitmap = fiBitmap2;
            }
            Int32[] imgSurfData = new Int32[width * height];
            for (int y = (int)height - 1; y >= 0; y--)
                for (int x = 0; x < width; x++)
                {
                    // get pixel color
                    RGBQUAD rgb = new RGBQUAD();
                    FreeImage.GetPixelColor(fiBitmap, (uint)x, (uint)(height - 1 - y), rgb);
                    byte[] pixColor = new byte[4];
                    pixColor[0] = rgb.rgbReserved;
                    pixColor[1] = rgb.rgbRed;
                    pixColor[2] = rgb.rgbGreen;
                    pixColor[3] = rgb.rgbBlue;
                    // premultiply alpha -> http://cairographics.org/manual/cairo-Image-Surfaces.html#cairo-format-t
                    if (pixColor[0] != 255)
                    {
                        double a = pixColor[0] / 255.0;
                        pixColor[1] = (byte)(pixColor[1] * a);
                        pixColor[2] = (byte)(pixColor[2] * a);
                        pixColor[3] = (byte)(pixColor[3] * a);
                    }
                    // convert pixColor to Int32 and put in data array
                    if (BitConverter.IsLittleEndian)
                    {
                        byte[] pixColor2 = new byte[4];
                        pixColor2[0] = pixColor[3];
                        pixColor2[1] = pixColor[2];
                        pixColor2[2] = pixColor[1];
                        pixColor2[3] = pixColor[0];
                        imgSurfData[x + y * width] = BitConverter.ToInt32(pixColor2, 0);
                    }
                    else
                        imgSurfData[x + y * width] = BitConverter.ToInt32(pixColor, 0);
                }
            // free FreeImage memory
            FreeImage.CloseMemory(hmem);
            // DeInit FreeImage
            FreeImage.DeInitialize();
            // pin databuffer until imgSurf is destroyed
            _gc_h_surface_data_buffer = GCHandle.Alloc(imgSurfData, GCHandleType.Pinned);
            // create ImageSurface from image data
            IntPtr dataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(imgSurfData, 0);
            ImageSurface imgSurf = new ImageSurface(dataPtr, Format.Argb32, (int)width, (int)height, (int)width * 32 / 8);
            // return surface
            return imgSurf;
        }

        protected override object LoadBitmap(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            object obj = LoadBitmap(fs);
            fs.Close();
            return obj;
        }

        public override void Dispose()
        {
            ((IDisposable)surface).Dispose();
            if (_gc_h_surface_data_buffer.IsAllocated)
                _gc_h_surface_data_buffer.Free();
        }

        public override int Width
        {
            get { return surface.Width; }
        }

        public override int Height
        {
            get { return surface.Height; }
        }

        public override void Save(string filename)
        {
            surface.WriteToPng(filename);
        }

        byte[] GetSurfaceData()
        {
            int length = Height * surface.Stride;
            byte[] data = new byte[length];
            Marshal.Copy(surface.Data, data, 0, length);
            return data;
        }

        public byte[] GetPngData()
        {
            byte[] data = GetSurfaceData();
            // check data format
            System.Diagnostics.Debug.Assert(surface.Format == Format.Argb32, "ERROR: surface.Format is not \"Argb32\".");
            System.Diagnostics.Debug.Assert(data.Length == Width * Height * 4, "ERROR: data is wrong length.");
            // init FreeImage
            FreeImage.Initialize(true);
            // load data into FreeImage
            uint fiBitmap = FreeImage.Allocate(Width, Height, 32, 0x00FF0000, 0x0000FF00, 0x000000FF);
            for (int y = Height - 1; y >= 0; y--)
                for (int x = 0; x < Width; x++)
                {
                    int idx = y * Width * 4 + x * 4;
                    RGBQUAD rgb = new RGBQUAD();
                    rgb.rgbRed = data[idx + 2];
                    rgb.rgbGreen = data[idx + 1];
                    rgb.rgbBlue = data[idx];
                    rgb.rgbReserved = data[idx + 3];
                    FreeImage.SetPixelColor(fiBitmap, (uint)x, (uint)(Height - y - 1), rgb);
                }
            // copy FreeImage data to byte array
            uint hmem = FreeImage.OpenMemory(IntPtr.Zero, 0);
            FreeImage.SaveToMemory(FREE_IMAGE_FORMAT.FIF_PNG, fiBitmap, hmem, 0);
            FreeImage.Unload(fiBitmap);
            IntPtr ptr = IntPtr.Zero;
            int size_in_bytes = 0;
            FreeImage.AcquireMemory(hmem, ref ptr, ref size_in_bytes);
            byte[] pngData = new byte[size_in_bytes];
            Marshal.Copy(ptr, pngData, 0, size_in_bytes);
            FreeImage.CloseMemory(hmem);
            // DeInit FreeImage
            FreeImage.DeInitialize();

            return pngData;
        }
    }

    public class CairoGraphics : DGraphics
    {
        protected Context cr;

        public Surface Target
        {
            get { return cr.Target; }
        }

        public CairoGraphics(Context cr)
        {
            this.cr = cr;
        }

        public CairoGraphics(DBitmap bmp)
        {
            cr = new Context((ImageSurface)bmp.NativeBmp);
        }

        protected CairoGraphics()
        {
        }

        public CairoGraphics(string pdfFile, double width, double height)
        {
            Surface surf = new PdfSurface(pdfFile, width, height);
            cr = new Context(surf);
        }

        public void ShowPage()
        {
            cr.ShowPage();
        }

        // Helper Functions //

        Color MakeColor(DColor color)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        Color MakeColor(DColor color, double alpha)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f * alpha);
        }

        LineJoin MakeLineJoin(DStrokeJoin strokeJoin)
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

        LineCap MakeLineCap(DStrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                case DStrokeCap.Butt:
                    return LineCap.Butt;
                case DStrokeCap.Round:
                    return LineCap.Round;
                case DStrokeCap.Square:
                    return LineCap.Square;
            }
            return LineCap.Butt;
        }

        FillRule MakeFillRule(DFillRule fillRule)
        {
            if (fillRule == DFillRule.EvenOdd)
                return FillRule.EvenOdd;
            return FillRule.Winding;
        }

        void CairoSetPattern(Context cr, DColor color, double alpha, DFillStyle fillStyle)
        {
            switch (fillStyle)
            {
                case DFillStyle.ForwardDiagonalHatch:
                    Surface patSurf = cr.Target.CreateSimilar(Content.ColorAlpha, 7, 7);
                    Context patCr = new Context(patSurf);
                    patCr.SetSource(MakeColor(color, alpha));
                    patCr.LineWidth = 1;
                    patCr.MoveTo(0, 0);
                    patCr.LineTo(7, 7);
                    patCr.Stroke();
                    SurfacePattern pat = new SurfacePattern(patSurf);
                    pat.Extend = Extend.Repeat;
                    cr.SetSource(pat);
                    ((IDisposable)patCr).Dispose();
                    ((IDisposable)patSurf).Dispose();
                    break;
                default:
                    cr.SetSource(MakeColor(color, alpha));
                    break;
            }
        }

        void CairoStrokeStyle(Context cr, DStrokeStyle strokeStyle, double strokeWidth)
        {
            double dot = strokeWidth;
            double space = 2 * strokeWidth;
            double dash = 3 * strokeWidth;
            switch (strokeStyle)
            {
                case DStrokeStyle.Solid:
                    cr.SetDash(new double[] { }, 0);
                    break;
                case DStrokeStyle.Dash:
                    cr.SetDash(new double[] { dash, space }, 0);
                    break;
                case DStrokeStyle.Dot:
                    cr.SetDash(new double[] { dot, space }, 0);
                    break;
                case DStrokeStyle.DashDot:
                    cr.SetDash(new double[] { dash, space, dot, space }, 0);
                    break;
                case DStrokeStyle.DashDotDot:
                    cr.SetDash(new double[] { dash, space, dot, space, dot, space }, 0);
                    break;
            }
        }

        void CairoEllipse(Context cr, double x, double y, double width, double height)
        {
            cr.Save();
            cr.Translate(x + width / 2f, y + height / 2f);
            cr.Scale(width / 2f, height / 2f);
            cr.Arc(0, 0, 1, 0, 2 * Math.PI);
            cr.Restore();
        }

        DMatrix MakeMatrix(Matrix matrix)
        {
            return new DMatrix(matrix.Xx, matrix.Yx, matrix.Xy, matrix.Yy, matrix.X0, matrix.Y0);
        }

        Matrix MakeMatrix(DMatrix matrix)
        {
            return new Matrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
        }

        // Drawing Functions //

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
            FillRect(x, y, width, height, color, alpha, DFillStyle.Solid);
        }

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha, DFillStyle fillStyle)
        {
            CairoSetPattern(cr, color, alpha, fillStyle);
            cr.Rectangle(x, y, width, height);
            cr.Fill();
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin)
        {
            cr.SetSource(MakeColor(color, alpha));
            cr.LineWidth = strokeWidth;
            CairoStrokeStyle(cr, strokeStyle, strokeWidth);
            cr.LineJoin = MakeLineJoin(strokeJoin);
            cr.Rectangle(x, y, width, height);
            cr.Stroke();
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
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, 1, 1);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha, DStrokeStyle strokeStyle)
        {
            cr.SetSource(MakeColor(color, alpha));
            cr.LineWidth = 1;
            CairoStrokeStyle(cr, strokeStyle, 1);
            cr.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            cr.Stroke();
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
            FillEllipse(x, y, width, height, color, 1);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            cr.SetSource(MakeColor(color, alpha));
            cr.LineWidth = 1;
            CairoStrokeStyle(cr, DStrokeStyle.Solid, 1);
            CairoEllipse(cr, x, y, width, height);
            cr.Fill();
        }

        public override void FillEllipse(DRect rect, DColor color)
        {
            FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, 1);
        }

        public override void FillEllipse(DRect rect, DColor color, double alpha)
        {
            FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha);
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color)
        {
            DrawEllipse(x, y, width, height, color, 1, 1, DStrokeStyle.Solid);
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle)
        {
            cr.SetSource(MakeColor(color, alpha));
            cr.LineWidth = strokeWidth;
            CairoStrokeStyle(cr, strokeStyle, strokeWidth);
            CairoEllipse(cr, x, y, width, height);
            cr.Stroke();
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, 1, 1, DStrokeStyle.Solid);
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1, DStrokeStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
            DrawLine(pt1, pt2, color, 1, DStrokeStyle.Solid);
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
            cr.SetSource(MakeColor(color, alpha));
            cr.LineWidth = strokeWidth;
            CairoStrokeStyle(cr, strokeStyle, strokeWidth);
            cr.LineCap = MakeLineCap(strokeCap);
            cr.MoveTo(pt1.X, pt1.Y);
            cr.LineTo(pt2.X, pt2.Y);
            cr.Stroke();
        }

        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round, DStrokeCap.Round);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap)
        {
            if (pts.Count > 1)
            {
                cr.SetSource(MakeColor(color, alpha));
                CairoStrokeStyle(cr, strokeStyle, strokeWidth);
                cr.LineWidth = strokeWidth;
                cr.LineJoin = MakeLineJoin(strokeJoin);
                cr.LineCap = MakeLineCap(strokeCap);
                cr.MoveTo(pts[0].X, pts[0].Y);
                for (int i = 1; i < pts.Count; i++)
                    cr.LineTo(pts[i].X, pts[i].Y);
                cr.Stroke();
            }
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha)
        {
            FillPolygon(pts, color, alpha, DFillRule.EvenOdd);
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha, DFillRule fillRule)
        {
            if (pts.Count > 1)
            {
                cr.SetSource(MakeColor(color, alpha));
                cr.FillRule = MakeFillRule(fillRule);
                cr.MoveTo(pts[0].X, pts[0].Y);
                for (int i = 1; i < pts.Count; i++)
                    cr.LineTo(pts[i].X, pts[i].Y);
                cr.Fill();
            }
        }
        
        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
            DrawBitmap(bitmap, pt, 1);
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt, double alpha)
        {
            cr.SetSource((ImageSurface)bitmap.NativeBmp, pt.X, pt.Y);
            cr.PaintWithAlpha(alpha);
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect)
        {
            DrawBitmap(bitmap, rect, 1);
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect, double alpha)
        {
            ImageSurface surf = (ImageSurface)bitmap.NativeBmp;
            cr.Save();
            cr.Translate(rect.X, rect.Y);
            cr.Scale(rect.Width / surf.Width, rect.Height / surf.Height);
            cr.SetSource(surf, 0, 0);
            cr.PaintWithAlpha(alpha);
            cr.Restore();
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color)
        {
            DrawText(text, fontName, fontSize, pt, color, 1);
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color, double alpha)
        {
            DrawText(text, fontName, fontSize, false, false, false, false, pt, color, alpha);
        }

        public override void DrawText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough, DPoint pt, DColor color, double alpha)
        {
            // set context properties (font, color etc)
            FontWeight fw = FontWeight.Normal;
            if (bold) fw = FontWeight.Bold;
            FontSlant fs = FontSlant.Normal;
            if (italics) fs = FontSlant.Italic;
            cr.SelectFontFace(fontName, fs, fw);
            cr.SetFontSize(fontSize);
            cr.SetSource(MakeColor(color, alpha));
            // split text at new lines
            string[] lines = text.Split('\n');
            // draw each line of text
            FontExtents fe = cr.FontExtents;
            foreach (string line in lines)
            {
                if (line.Length > 0) // cairo doesnt like measuring or drawing empty lines
                {
                    TextExtents te = cr.TextExtents(line);
                    double line_x = pt.X - te.XBearing;
                    double line_y = pt.Y - te.YBearing;
                    cr.MoveTo(line_x, line_y);
                    cr.ShowText(line);
                    if (underline)
                    {
                        cr.MoveTo(line_x, line_y + fe.Descent - 2);
                        cr.LineTo(line_x + te.XAdvance, line_y + fe.Descent - 2);
                        cr.Stroke();
                    }
                    if (strikethrough)
                    {
                        cr.MoveTo(line_x, line_y - fe.Descent);
                        cr.LineTo(line_x + te.XAdvance, line_y - fe.Descent);
                        cr.Stroke();
                    }
                }
                // increment pt.Y
                pt.Y += fe.Height;
            }
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
            return MeasureText(text, fontName, fontSize, false, false, false, false);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough)
        {
            // set context properties
            FontWeight fw = FontWeight.Normal;
            if (bold) fw = FontWeight.Bold;
            FontSlant fs = FontSlant.Normal;
            if (italics) fs = FontSlant.Italic;
            cr.SelectFontFace(fontName, fs, fw);
            cr.SetFontSize(fontSize);
            // split into lines
            string[] lines = text.Split('\n');
            // calc width and height for each line
            double w = 0, h = 0;
            FontExtents fe = cr.FontExtents;
            foreach (string line in lines)
            {
                if (line.Length > 0) // cairo doesnt like measuring or drawing empty lines
                {
                    TextExtents te = cr.TextExtents(line);
                    if (w < te.XAdvance)
                        w = te.XAdvance;
                }
                h += fe.Height;
            }
            // return dimensions
            return new DPoint(w, h - fe.Height + fe.Ascent);
        }

        public override void StartGroup(double x, double y, double width, double height, double offsetX, double offsetY)
        {
            cr.PushGroup();
        }

        public override void DrawGroup(double alpha)
        {
            cr.PopGroupToSource();
            cr.PaintWithAlpha(alpha);
        }

        public override DMatrix SaveTransform()
        {
            return MakeMatrix(cr.Matrix);
        }

        public override void LoadTransform(DMatrix matrix)
        {
            cr.Matrix = MakeMatrix(matrix);
        }

        public override void Scale(double sx, double sy)
        {
            cr.Scale(sx, sy);
        }

        public override void Rotate(double angle, DPoint center)
        {
            cr.Translate(center.X, center.Y);
            cr.Rotate(angle);
            cr.Translate(-center.X, -center.Y);
        }

        public override void Translate(double tx, double ty)
        {
            cr.Translate(tx, ty);
        }

        public override void ResetTransform()
        {
            cr.Matrix = new Matrix(1, 0, 0, 0, 1, 0); // identity matrix
        }

        public override void Clip(DRect r)
        {
            cr.Rectangle(r.X, r.Y, r.Width, r.Height);
            cr.Clip();
        }

        public override void ResetClip()
        {
            cr.ResetClip();
        }

        public override DCompositingMode CompositingMode
        {
            get
            {
                if (cr.Operator == Operator.Source)
                    return DCompositingMode.SourceCopy;
                else
                    return DCompositingMode.SourceOver;
            }
            set
            {
                if (value == DCompositingMode.SourceOver)
                    cr.Operator = Operator.Over;
                else
                    cr.Operator = Operator.Source;
            }
        }

        public override bool AntiAlias
        {
            get { return cr.Antialias == Antialias.Subpixel; }
            set
            {
                if (value)
                    cr.Antialias = Antialias.Subpixel;
                else
                    cr.Antialias = Antialias.None;
            }
        }

        public override void Save()
        {
            cr.Save();
        }

        public override void Restore()
        {
            cr.Restore();
        }

        public override void Dispose()
        {
            ((IDisposable)cr).Dispose();
        }
    }
}
