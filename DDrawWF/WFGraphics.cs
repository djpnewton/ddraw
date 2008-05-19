using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace DDraw.WinForms
{
    public static class WFHelper
    {
        public static bool Cairo
        {
            get 
            { 
#if CAIRO
                return true; 
#else
                return false;
#endif
            }
        }

        public static void InitGraphics()
        {
#if CAIRO
            WFCairoGraphics.Init();
#else
            GDIGraphics.Init();
#endif
        }

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

        public static byte[] ToImageData(DBitmap bmp)
        {
#if CAIRO
            return ((WFCairoBitmap)bmp).GetPngData();
#else
            return ToImageData((Bitmap)bmp.NativeBmp);
#endif
        }

        public static byte[] ToImageData(Bitmap bmp)
        {
            return (byte[])TypeDescriptor.GetConverter(bmp).ConvertTo(bmp, typeof(byte[]));
        }

        public static Bitmap FromImageData(byte[] data)
        {
            return new Bitmap(new MemoryStream(data));
        }
        
        public static DBitmap MakeBitmap(Bitmap bmp)
        {
#if CAIRO
            return new WFCairoBitmap(bmp);
#else
            return new GDIBitmap(bmp);
#endif
        }

        public static DBitmap MakeBitmap(int width, int height)
        {
#if CAIRO
            return new WFCairoBitmap(width, height);
#else
            return new GDIBitmap(width, height);
#endif
        }

        public static DBitmap MakeBitmap(string filename)
        {
#if CAIRO
            return new WFCairoBitmap(filename);
#else
            return new GDIBitmap(filename);
#endif
        }

        public static DGraphics MakeGraphics(DBitmap bmp)
        {
#if CAIRO
            return new WFCairoGraphics(bmp);
#else
            return new GDIGraphics(bmp);
#endif
        }

        public static DGraphics MakeGraphics(Graphics g)
        {
#if CAIRO
            return new WFCairoGraphics(g);
#else
            return new GDIGraphics(g);
#endif
        }

        public static WFCairoGraphics MakePDFCairoGraphics(string pdfFile, double width, double height)
        {
            return new WFCairoGraphics(pdfFile, width, height);
        }

        public static void SetCairoPDFSurfaceSize(WFCairoGraphics dg, DPoint pgSzMM)
        {
            System.Diagnostics.Debug.Assert(dg.Target is Cairo.PdfSurface);
            //  Convert page size in MM to cairo pdf points (1/72 inch , http://cairographics.org/manual/cairo-PDF-Surfaces.html#cairo-pdf-surface-set-size)
            double width = (pgSzMM.X / PageTools.MMPerInch) * 72;
            double height = (pgSzMM.Y / PageTools.MMPerInch) * 72;
            ((Cairo.PdfSurface)dg.Target).SetSize(width, height);
        }

        public static void ShowCairoPDFPage(WFCairoGraphics dg)
        {
            System.Diagnostics.Debug.Assert(dg.Target is Cairo.PdfSurface);
            dg.ShowPage();
        }
    }
}
