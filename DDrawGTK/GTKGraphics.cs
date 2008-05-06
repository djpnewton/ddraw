using System;
using System.IO;

using Cairo;
using DDraw;
using DDrawCairo;

namespace DDraw.GTK
{
    public static class GTKHelper
    {
        public static void InitGraphics()
        {
            CairoGraphics.Init(typeof(GTKBitmap), typeof(GTKGraphics));
        }
    }

    public class GTKBitmap : CairoBitmap
    {
        public GTKBitmap(int width, int height) : base(width, height)
        {
        }

        public GTKBitmap(Stream s): base(s)
        {
        }
    }

    public class GTKGraphics : CairoGraphics
    {
        public GTKGraphics(Context cr) : base(cr)
        {
        }

        public GTKGraphics(DBitmap bmp) : base(bmp)
        {
        }
    }
}
