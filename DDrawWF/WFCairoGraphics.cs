using System;
using System.IO;

using Cairo;
using DDraw;
using DDrawCairo;

namespace DDraw.WinForms
{
    public class WFCairoBitmap : CairoBitmap
    {
        public WFCairoBitmap(int width, int height): base(width, height)
        {}
        
        public WFCairoBitmap(string filename): base(filename)
        { }

        public WFCairoBitmap(Stream s) : base(s)
        {}

        public WFCairoBitmap(System.Drawing.Bitmap bmp)
        {
            nativeBmp = BitmapToSurface(bmp);
        }

        ImageSurface BitmapToSurface(System.Drawing.Bitmap bmp)
        {
            Int32[] data = new Int32[bmp.Width * bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    System.Drawing.Color col = bmp.GetPixel(x, y);
                    data[x + y * bmp.Width] = col.ToArgb();
                }

            // pin databuffer until imgSurf is destroyed
            _gc_h_surface_data_buffer = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);

            // create image surface
            IntPtr dataPtr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            ImageSurface imgSurf = new ImageSurface(dataPtr, Format.Argb32, bmp.Width, bmp.Height, bmp.Width * 32 / 8);

            return imgSurf; 
        }
    }

    public class WFCairoGraphics : CairoGraphics
    {
        public static void Init()
        {
            GraphicsHelper.Init(typeof(WFCairoBitmap), typeof(WFCairoGraphics));
        }

        public WFCairoGraphics(Context cr) : base(cr)
        {
        }

        public WFCairoGraphics(DBitmap bmp) : base(bmp)
        {
        }

        System.Drawing.Graphics g = null;
        Win32Surface surf = null;

        public WFCairoGraphics(System.Drawing.Graphics g)
        {
            this.g = g;
            surf = new Cairo.Win32Surface(g.GetHdc());
            cr = new Cairo.Context(surf);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (surf != null)
                surf.Destroy();
            if (g != null)
                g.ReleaseHdc();
        }
    }
}
