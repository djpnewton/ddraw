using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using DejaVu;

namespace DDraw
{
    public enum DMetafileType { Wmf };

    public static class Emf
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt32 Type, Size;
            public RectL Bounds, Frame;
            public UInt32 Signature, Version, Bytes, Records;
            public UInt16 Handles, Reserved;
            public UInt32 nDescription, offDescription, nPalEntries;
            public Wmf.SizeL Device, Millimeters;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HeaderExtension1
        {
            public uint cbPixelFormat, offPixelFormat, bOpenGL;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HeaderExtension2
        {
            public uint MicrometersX, MicrometersY;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MetafileHeaderExtension2
        {
            public Header EmfHeader;
            public HeaderExtension1 EmfHeaderExtension1;
            public HeaderExtension2 EmfHeaderExtension2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RecordHeader
        {
            public UInt32 Type;
            public UInt32 Size;
        }

        public enum FormatSignature
        {
            ENHMETA_SIGNATURE = 0x464D4520,
            EPS_SIGNATURE = 0x46535045
        }

        public enum RecordType
        {
            EMR_HEADER = 0x00000001,
            EMR_POLYBEZIER = 0x00000002,
            EMR_POLYGON = 0x00000003,
            EMR_POLYLINE = 0x00000004,
            EMR_POLYBEZIERTO = 0x00000005,
            EMR_POLYLINETO = 0x00000006,
            EMR_POLYPOLYLINE = 0x00000007,
            EMR_POLYPOLYGON = 0x00000008,
            EMR_SETWINDOWEXTEX = 0x00000009,
            EMR_SETWINDOWORGEX = 0x0000000A,
            EMR_SETVIEWPORTEXTEX = 0x0000000B,
            EMR_SETVIEWPORTORGEX = 0x0000000C,
            EMR_SETBRUSHORGEX = 0x0000000D,
            EMR_EOF = 0x0000000E,
            EMR_SETPIXELV = 0x0000000F,
            EMR_SETMAPPERFLAGS = 0x00000010,
            EMR_SETMAPMODE = 0x00000011,
            EMR_SETBKMODE = 0x00000012,
            EMR_SETPOLYFILLMODE = 0x00000013,
            EMR_SETROP2 = 0x00000014,
            EMR_SETSTRETCHBLTMODE = 0x00000015,
            EMR_SETTEXTALIGN = 0x00000016,
            EMR_SETCOLORADJUSTMENT = 0x00000017,
            EMR_SETTEXTCOLOR = 0x00000018,
            EMR_SETBKCOLOR = 0x00000019,
            EMR_OFFSETCLIPRGN = 0x0000001A,
            EMR_MOVETOEX = 0x0000001B,
            EMR_SETMETARGN = 0x0000001C,
            EMR_EXCLUDECLIPRECT = 0x0000001D,
            EMR_INTERSECTCLIPRECT = 0x0000001E,
            EMR_SCALEVIEWPORTEXTEX = 0x0000001F,
            EMR_SCALEWINDOWEXTEX = 0x00000020,
            EMR_SAVEDC = 0x00000021,
            EMR_RESTOREDC = 0x00000022,
            EMR_SETWORLDTRANSFORM = 0x00000023,
            EMR_MODIFYWORLDTRANSFORM = 0x00000024,
            EMR_SELECTOBJECT = 0x00000025,
            EMR_CREATEPEN = 0x00000026,
            EMR_CREATEBRUSHINDIRECT = 0x00000027,
            EMR_DELETEOBJECT = 0x00000028,
            EMR_ANGLEARC = 0x00000029,
            EMR_ELLIPSE = 0x0000002A,
            EMR_RECTANGLE = 0x0000002B,
            EMR_ROUNDRECT = 0x0000002C,
            EMR_ARC = 0x0000002D,
            EMR_CHORD = 0x0000002E,
            EMR_PIE = 0x0000002F,
            EMR_SELECTPALETTE = 0x00000030,
            EMR_CREATEPALETTE = 0x00000031,
            EMR_SETPALETTEENTRIES = 0x00000032,
            EMR_RESIZEPALETTE = 0x00000033,
            EMR_REALIZEPALETTE = 0x00000034,
            EMR_EXTFLOODFILL = 0x00000035,
            EMR_LINETO = 0x00000036,
            EMR_ARCTO = 0x00000037,
            EMR_POLYDRAW = 0x00000038,
            EMR_SETARCDIRECTION = 0x00000039,
            EMR_SETMITERLIMIT = 0x0000003A,
            EMR_BEGINPATH = 0x0000003B,
            EMR_ENDPATH = 0x0000003C,
            EMR_CLOSEFIGURE = 0x0000003D,
            EMR_FILLPATH = 0x0000003E,
            EMR_STROKEANDFILLPATH = 0x0000003F,
            EMR_STROKEPATH = 0x00000040,
            EMR_FLATTENPATH = 0x00000041,
            EMR_WIDENPATH = 0x00000042,
            EMR_SELECTCLIPPATH = 0x00000043,
            EMR_ABORTPATH = 0x00000044,
            EMR_COMMENT = 0x00000046,
            EMR_FILLRGN = 0x00000047,
            EMR_FRAMERGN = 0x00000048,
            EMR_INVERTRGN = 0x00000049,
            EMR_PAINTRGN = 0x0000004A,
            EMR_EXTSELECTCLIPRGN = 0x0000004B,
            EMR_BITBLT = 0x0000004C,
            EMR_STRETCHBLT = 0x0000004D,
            EMR_MASKBLT = 0x0000004E,
            EMR_PLGBLT = 0x0000004F,
            EMR_SETDIBITSTODEVICE = 0x00000050,
            EMR_STRETCHDIBITS = 0x00000051,
            EMR_EXTCREATEFONTINDIRECTW = 0x00000052,
            EMR_EXTTEXTOUTA = 0x00000053,
            EMR_EXTTEXTOUTW = 0x00000054,
            EMR_POLYBEZIER16 = 0x00000055,
            EMR_POLYGON16 = 0x00000056,
            EMR_POLYLINE16 = 0x00000057,
            EMR_POLYBEZIERTO16 = 0x00000058,
            EMR_POLYLINETO16 = 0x00000059,
            EMR_POLYPOLYLINE16 = 0x0000005A,
            EMR_POLYPOLYGON16 = 0x0000005B,
            EMR_POLYDRAW16 = 0x0000005C,
            EMR_CREATEMONOBRUSH = 0x0000005D,
            EMR_CREATEDIBPATTERNBRUSHPT = 0x0000005E,
            EMR_EXTCREATEPEN = 0x0000005F,
            EMR_POLYTEXTOUTA = 0x00000060,
            EMR_POLYTEXTOUTW = 0x00000061,
            EMR_SETICMMODE = 0x00000062,
            EMR_CREATECOLORSPACE = 0x00000063,
            EMR_SETCOLORSPACE = 0x00000064,
            EMR_DELETECOLORSPACE = 0x00000065,
            EMR_GLSRECORD = 0x00000066,
            EMR_GLSBOUNDEDRECORD = 0x00000067,
            EMR_PIXELFORMAT = 0x00000068,
            EMR_DRAWESCAPE = 0x00000069,
            EMR_EXTESCAPE = 0x0000006A,
            EMR_SMALLTEXTOUT = 0x0000006C,
            EMR_FORCEUFIMAPPING = 0x0000006D,
            EMR_NAMEDESCAPE = 0x0000006E,
            EMR_COLORCORRECTPALETTE = 0x0000006F,
            EMR_SETICMPROFILEA = 0x00000070,
            EMR_SETICMPROFILEW = 0x00000071,
            EMR_ALPHABLEND = 0x00000072,
            EMR_SETLAYOUT = 0x00000073,
            EMR_TRANSPARENTBLT = 0x00000074,
            EMR_GRADIENTFILL = 0x00000076,
            EMR_SETLINKEDUFIS = 0x00000077,
            EMR_SETTEXTJUSTIFICATION = 0x00000078,
            EMR_COLORMATCHTOTARGETW = 0x00000079,
            EMR_CREATECOLORSPACEW = 0x0000007A
        }

        public enum PenStyle
        {
            PS_COSMETIC = 0x00000000,
            PS_ENDCAP_ROUND = 0x00000000,
            PS_JOIN_ROUND = 0x00000000,
            PS_SOLID = 0x00000000,
            PS_DASH = 0x00000001,
            PS_DOT = 0x00000002,
            PS_DASHDOT = 0x00000003,
            PS_DASHDOTDOT = 0x00000004,
            PS_NULL = 0x00000005,
            PS_INSIDEFRAME = 0x00000006,
            PS_USERSTYLE = 0x00000007,
            PS_ALTERNATE = 0x00000008,
            PS_ENDCAP_SQUARE = 0x00000100,
            PS_ENDCAP_FLAT = 0x00000200,
            PS_JOIN_BEVEL = 0x00001000,
            PS_JOIN_MITER = 0x00002000,
            PS_GEOMETRIC = 0x00010000
        }

        public enum StockObject : uint
        {
            WHITE_BRUSH = 0x80000000,
            LTGRAY_BRUSH = 0x80000001,
            GRAY_BRUSH = 0x80000002,
            DKGRAY_BRUSH = 0x80000003,
            BLACK_BRUSH = 0x80000004,
            NULL_BRUSH = 0x80000005,
            WHITE_PEN = 0x80000006,
            BLACK_PEN = 0x80000007,
            NULL_PEN = 0x80000008,
            OEM_FIXED_FONT = 0x8000000A,
            ANSI_FIXED_FONT = 0x8000000B,
            ANSI_VAR_FONT = 0x8000000C,
            SYSTEM_FONT = 0x8000000D,
            DEVICE_DEFAULT_FONT = 0x8000000E,
            DEFAULT_PALETTE = 0x8000000F,
            SYSTEM_FIXED_FONT = 0x80000010,
            DEFAULT_GUI_FONT = 0x80000011,
            DC_BRUSH = 0x80000012,
            DC_PEN = 0x80000013
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RectL
        {
            public Int32 Left, Top, Right, Bottom;
            public RectL(int l, int t, int r, int b)
            {
                Left = l;
                Top = t;
                Right = r;
                Bottom = b;
            }
        }

        public enum ModifyWorldTransformMode
        {
            MWT_IDENTITY = 0x01,
            MWT_LEFTMULTIPLY = 0x02,
            MWT_RIGHTMULTIPLY = 0x03,
            MWT_SET = 0x04
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct XForm
        {
            public float M11, M12, M21, M22, Dx, Dy;
            public XForm(float M11, float M12, float M21, float M22, float Dx, float Dy)
            {
                this.M11 = M11;
                this.M12 = M12;
                this.M21 = M21;
                this.M22 = M22;
                this.Dx = Dx;
                this.Dy = Dy;
            }
            public XForm(DMatrix m)
            {
                this.M11 = (float)m.A;
                this.M12 = (float)m.B;
                this.M21 = (float)m.C;
                this.M22 = (float)m.D;
                this.Dx = (float)m.E;
                this.Dy = (float)m.F;
            }
        }
    }

    public static class Wmf
    {
        public const UInt32 PlaceableKey = 0x9AC6CDD7;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt16 Type, HeaderSize, Version;
            public UInt32 Filesize;
            public UInt16 NumOfObjects;
            public UInt32 MaxRecord;
            public UInt16 NumOfMembers; // not used
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Placeable
        {
            public UInt32 Key;
            public UInt16 HWmf;
            public UInt16 Left, Top, Right, Bottom;
            public UInt16 Inch;
            public UInt32 Reserved;
            public UInt16 Checksum;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FuncHeader
        {
            public UInt32 Size;
            public UInt16 Function;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PointL
        {
            public Int32 x, y;
            public PointL(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SizeL
        {
            public UInt32 cx, cy;
            public SizeL(UInt32 cx, UInt32 cy)
            {
                this.cx = cx; this.cy = cy;
            }
        }

        // Function consts
        public const int EOF = 0x0000;
        public const int SaveDC = 0x001E;
        public const int CreatePalette = 0x00F7;
        public const int SetBkMode = 0x0102;
        public const int SetMapMode = 0x0103;
        public const int SetROP2 = 0x0104;
        public const int SetRelabs = 0x0105;
        public const int SetPolyFillMode = 0x0106;
        public const int SetStretchBltMode = 0x0107;
        public const int DeleteObject = 0x01F0;
        public const int RestoreDC = 0x0127;
        public const int SelectObject = 0x012D;
        public const int SetTextAlign = 0x012E;
        public const int SetBkColor = 0x0201;
        public const int SetTextColor = 0x0209;
        public const int SetWindowOrg = 0x020B;
        public const int SetWindowExt = 0x020C;
        public const int SetViewportOrg = 0x020D;
        public const int SetViewportExt = 0x020E;
        public const int LineTo = 0x0213;
        public const int MoveTo = 0x0214;
        public const int SelectPalette = 0x0234;
        public const int CreatePenIndirect = 0x02FA;
        public const int CreateFontIndirect = 0x02FB;
        public const int CreateBrushIndirect = 0x02FC;
        public const int Polygon = 0x0324;
        public const int Polyline = 0x0325;
        public const int Ellipse = 0x0418;
        public const int Rectangle = 0x041B;
        public const int PolyPolygon = 0x0538;
        public const int Escape = 0x0626;

        // PolyFillMode consts
        public const int PfmAlernate = 0x0001;
        public const int PfmWinding = 0x0002;

        // Brush Style consts
        public const int BS_SOLID = 0x0000;
        public const int BS_NULL = 0x0001;
        public const int BS_HATCHED = 0x0002;
        public const int BS_PATTERN = 0x0003;
        public const int BS_INDEXED = 0x0004;
        public const int BS_DIBPATTERN = 0x0005;
        public const int BS_DIBPATTERNPT = 0x0006;
        public const int BS_PATTERN8X8 = 0x0007;
        public const int BS_DIBPATTERN8X8 = 0x0008;
        public const int BS_MONOPATTERN = 0x0009;

        // Pen Style consts
        public const int PS_COSMETIC = 0x0000;
        public const int PS_ENDCAP_ROUND = 0x0000;
        public const int PS_JOIN_ROUND = 0x0000;
        public const int PS_SOLID = 0x0000;
        public const int PS_DASH = 0x0001;
        public const int PS_DOT = 0x0002;
        public const int PS_DASHDOT = 0x0003;
        public const int PS_DASHDOTDOT = 0x0004;
        public const int PS_NULL = 0x0005;
        public const int PS_INSIDEFRAME = 0x0006;
        public const int PS_USERSTYLE = 0x0007;
        public const int PS_ALTERNATE = 0x0008;
        public const int PS_ENDCAP_SQUARE = 0x0100;
        public const int PS_ENDCAP_FLAT = 0x0200;
        public const int PS_JOIN_BEVEL = 0x1000;
        public const int PS_JOIN_MITER = 0x2000;

        // TextAlignmentMode consts
        public const int TA_NOUPDATECP = 0x0000; 
        public const int TA_LEFT = 0x0000; 
        public const int TA_TOP = 0x0000; 
        public const int TA_UPDATECP = 0x0001; 
        public const int TA_RIGHT = 0x0002; 
        public const int TA_CENTER = 0x0006; 
        public const int TA_BOTTOM = 0x0008; 
        public const int TA_BASELINE = 0x0018;
        public const int TA_RTLREADING = 0x0100;
    }

    public interface IMetafile
    {
        DMetafileType MetafileType
        {
            get;
            set;
        }

        DPoint MetafileSize
        {
            get;
        }
    }

    public class MetafileFigure : ImagebaseFigure, IMetafile
    {
        UndoRedo<DMetafileType> _metafileType = new UndoRedo<DMetafileType>();
        public DMetafileType MetafileType
        {
            get { return _metafileType.Value; }
            set { _metafileType.Value = value; }
        }

        public DPoint MetafileSize
        {
            get
            {
                switch (MetafileType)
                {
                    case DMetafileType.Wmf:
                        return GetWmfSize();
                    default:
                        System.Diagnostics.Debug.Fail("dont know the size :(");
                        return new DPoint(0, 0);
                }
            }
        }            

        protected override void ImageDataChanged()
        {
            //TODO set width and height?
        }

        public MetafileFigure()
        { }

        public MetafileFigure(DPoint pos, double rotation, DMetafileType type, byte[] data, string fileName)
        {
            MetafileType = type;
            ImageData = data;
            FileName = fileName;
            // set coords
            Left = pos.X;
            Top = pos.Y;
            DPoint size = MetafileSize;
            Width = size.X;
            Height = size.Y;
            Rotation = rotation;
        }

        object RawDeserialize(byte[] rawData, int position, Type anyType)
        {
            int rawsize = Marshal.SizeOf(anyType);
            if (rawsize > rawData.Length)
                return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anyType);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        protected override void PaintBody(DGraphics dg)
        {
            if (Alpha != 1)
            {
                dg.StartGroup(X, Y, Width, Height, 0, 0);
                PaintMetafile(dg);
                dg.DrawGroup(Alpha);
            }
            else
                PaintMetafile(dg);
        }

        void PaintMetafile(DGraphics dg)
        {
            if (MetafileType == DMetafileType.Wmf && ImageData.Length >= Marshal.SizeOf(typeof(Wmf.Placeable)))
                PaintWmf(dg);
        }

        #region Wmf Code
        // see http://sk1project.org/index.php for python wmf code
        // see http://msdn.microsoft.com/en-us/library/cc215212.aspx for microsoft documentation
        // see http://wvware.sourceforge.net/libwmf.html for some more wmf code

        Wmf.Header GetWmfHeader()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(Wmf.Header));
            bytesRead += Marshal.SizeOf(typeof(Wmf.Header));
            return (Wmf.Header)obj;
        }

        Wmf.Placeable GetWmfPlaceable()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(Wmf.Placeable));
            bytesRead += Marshal.SizeOf(typeof(Wmf.Placeable));
            return (Wmf.Placeable)obj;
        }

        int WmfFuncHeaderSize = Marshal.SizeOf(typeof(Wmf.FuncHeader)); // cache so we dont have to call a million times
        Wmf.FuncHeader GetWmfFuncHeader()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(Wmf.FuncHeader));
            bytesRead += WmfFuncHeaderSize;
            return (Wmf.FuncHeader)obj;
        }

        int bytesRead;

        byte GetByte()
        {
            byte val = ImageData[bytesRead];
            bytesRead += 1;
            return val;
        }

        int GetInt16()
        {
            int val = BitConverter.ToInt16(ImageData, bytesRead);
            bytesRead += sizeof(UInt16);
            return val;
        }

        DColor GetColor()
        {
            DColor col = new DColor(GetByte(), GetByte(), GetByte());
            byte reserved = GetByte();
            return col;
        }

        DPoint GetPoint()
        {
            // y proceeds x
            double y = GetInt16(), x = GetInt16();
            WmfApplyTransforms(ref x, ref y);
            return new DPoint(x, y);
        }

        DPoints GetPolyPoints(int num, bool join)
        {
            DPoints val = new DPoints();
            for (int i = 0; i < num; i++)
            {
                // x proceeds y
                double x = GetInt16(), y = GetInt16();
                WmfApplyTransforms(ref x, ref y);
                val.Add(new DPoint (x, y));
            }
            // join polygon
            if (join && num > 1 && (val[0].X != val[num-1].X || val[0].Y != val[num-1].Y))
                val.Add(val[0]);
            return val;
        }

        void WmfApplyTransforms(ref double x, ref double y)
        {
            x += -winX;
            y += -winY;
            if (winWidth != 0 && winHeight != 0)
            {
                x *= Width / winWidth;
                y *= Height / winHeight;
            }
            x += X;
            y += Y;
        }

        void WmfApplyTransforms(ref double width)
        {
            if (winWidth != 0 && width != 0)
                width *= Width / winWidth;
        }

        void WmfUpdateMaxtrix(DGraphics dg, DMatrix m)
        {
            // could not get this working... used wmfApplyTransforms to all points instead :(
            /*
            dg.LoadTransform(m);
            dg.Translate(X - winX, Y - winY);
            //int pW = placeable.Right - placeable.Left, pH = placeable.Bottom - placeable.Top;
            //dg.Scale(Width / pW, Height / pH);
            //if (winWidth != 0 && winHeight != 0)
            //    dg.Scale(pW / winWidth, pH / winHeight);
            if (winWidth != 0 && winHeight != 0)
                dg.Scale(Width / winWidth, Height / winHeight);
            */
        }

        // wmf vars
        Wmf.Placeable placeable;
        Wmf.Header header;
        int winX, winY, winWidth, winHeight;
        int viewX, viewY, viewWidth, viewHeight;

        // drawing primatives
        DColor fill = DColor.Empty;
        DFillRule fillRule = DFillRule.EvenOdd;
        DColor stroke = DColor.Empty;
        double strokeWidth = 1;
        DStrokeStyle strokeStyle = DStrokeStyle.Solid;
        DStrokeCap strokeCap = DStrokeCap.Butt;
        DStrokeJoin strokeJoin = DStrokeJoin.Bevel;
        DPoint curPoint = new DPoint(0, 0);

        struct WmfGdiBrush
        {
            public DColor Fill;
            public WmfGdiBrush(DColor fill)
            {
                Fill = fill;
            }
        }
        struct WmfGdiPen
        {
            public DColor Stroke;
            public double StrokeWidth;
            public DStrokeStyle StrokeStyle;
            public DStrokeCap StrokeCap;
            public DStrokeJoin StrokeJoin;
            public WmfGdiPen(DColor stroke, double strokeWidth, DStrokeStyle strokeStyle, DStrokeCap strokeCap, DStrokeJoin strokeJoin)
            {
                Stroke = stroke;
                StrokeWidth = strokeWidth;
                StrokeStyle = strokeStyle;
                StrokeCap = strokeCap;
                StrokeJoin = strokeJoin;
            }
        }

        List<object> WmfGdiObjects = new List<object>();

        void WmfAddGdiObject(object o)
        {
            int idx = WmfGdiObjects.IndexOf(null);
            if (idx == -1)
                WmfGdiObjects.Add(o);
            else
                WmfGdiObjects[idx] = o;
        }

        void WmfSelectGdiObject(int idx)
        {
            // this should not be happening but sometimes does?
            if (idx == WmfGdiObjects.Count)
                idx--;
            // select object and apply properties to our graphics primatives
            object o = WmfGdiObjects[idx];
            if (o is WmfGdiBrush)
                fill = ((WmfGdiBrush)o).Fill;
            else if (o is WmfGdiPen)
            {
                stroke = ((WmfGdiPen)o).Stroke;
                strokeWidth = ((WmfGdiPen)o).StrokeWidth;
                strokeStyle = ((WmfGdiPen)o).StrokeStyle;
                strokeCap = ((WmfGdiPen)o).StrokeCap;
                strokeJoin = ((WmfGdiPen)o).StrokeJoin;
            }
        }

        void WmfDeleteGdiObject(int idx)
        {
            // this should not be happening but sometimes does?
            if (idx == WmfGdiObjects.Count)
                idx--;
            WmfGdiObjects[idx] = null;
        }

        bool WmfIsPlaceable
        {
            get { return BitConverter.ToUInt32(ImageData, 0) == Wmf.PlaceableKey; }
        }

        private void PaintWmf(DGraphics dg)
        {
            dg.Save();
            dg.Clip(Rect);
            // store current matrix
            DMatrix m = dg.SaveTransform();
            // clear gdi objects
            WmfGdiObjects.Clear();
            // check for placeable key
            if (WmfIsPlaceable)
            {
                bytesRead = 0;
                // read placeable header
                placeable = GetWmfPlaceable();
                // checksum
                int sum = 0;
                for (int i = 0; i < (Marshal.SizeOf(typeof(Wmf.Placeable)) - 2 /* WmfPlaceable.Checksum is UInt16 */) / 2; i++)
                    sum = sum ^ BitConverter.ToUInt16(ImageData, i * 2);
                if (sum != placeable.Checksum)
                    System.Diagnostics.Debug.Fail("checksum failed");
                // init matrix
                WmfUpdateMaxtrix(dg, m);
                // read header
                header = GetWmfHeader();
                // iterate draw commands
                bool records = true;
                while (records)
                {
                    int bytesReadPlusFuncHeaderSize = bytesRead;
                    Wmf.FuncHeader fh = GetWmfFuncHeader();
                    bytesReadPlusFuncHeaderSize += (int)fh.Size * 2;
                    bool breakme = false;
                    switch (fh.Function)
                    {
                        case Wmf.SaveDC:
                            break;
                        case Wmf.CreatePalette:
                            break;
                        case Wmf.SetBkMode:
                            int bkMode = GetInt16();
                            break;
                        case Wmf.SetMapMode:
                            int mapMode = GetInt16();
                            break;
                        case Wmf.SetROP2:
                            break;
                        case Wmf.SetRelabs:
                            break;
                        case Wmf.SetPolyFillMode:
                            int polyfillMode = GetInt16();
                            if (polyfillMode == Wmf.PfmAlernate)
                                fillRule = DFillRule.EvenOdd;
                            else
                                fillRule = DFillRule.Winding;
                            break;
                        case Wmf.SetStretchBltMode:
                            break;
                        case Wmf.DeleteObject:
                            WmfDeleteGdiObject(GetInt16());
                            break;
                        case Wmf.RestoreDC:
                            break;
                        case Wmf.SelectObject:
                            WmfSelectGdiObject(GetInt16());
                            break;
                        case Wmf.SetTextAlign:
                            break;
                        case Wmf.SetBkColor:
                            break;
                        case Wmf.SetTextColor:
                            break;
                        case Wmf.SetWindowOrg:
                            winY = GetInt16();
                            winX = GetInt16();
                            WmfUpdateMaxtrix(dg, m);
                            break;
                        case Wmf.SetWindowExt:                            
                            winHeight = GetInt16();
                            winWidth = GetInt16();
                            WmfUpdateMaxtrix(dg, m);
                            break;
                        case Wmf.SetViewportOrg:
                            viewY = GetInt16();
                            viewX = GetInt16();
                            //wmfUpdateMaxtrix(dg, m);
                            break;
                        case Wmf.SetViewportExt:
                            viewHeight = GetInt16();
                            viewWidth = GetInt16();
                            //wmfUpdateMaxtrix(dg, m);
                            break;
                        case Wmf.LineTo:
                            DPoint pt = GetPoint();
                            if (StrokeValid)
                                dg.DrawLine(curPoint, pt, stroke, 1, strokeStyle, strokeWidth, strokeCap);
                            curPoint = pt;
                            break;
                        case Wmf.MoveTo:
                            curPoint = GetPoint();
                            break;
                        case Wmf.SelectPalette:
                            break;
                        case Wmf.CreatePenIndirect:
                            int gdiPenStyle = GetInt16();
                            int widthX = GetInt16();
                            int widthY = GetInt16();
                            DColor penColor = GetColor();
                            double penWidth = widthX;
                            WmfApplyTransforms(ref penWidth);
                            DStrokeStyle penStyle = DStrokeStyle.Solid;
                            DStrokeCap penCap = DStrokeCap.Round;
                            DStrokeJoin penJoin = DStrokeJoin.Round;
                            
                            if ((gdiPenStyle & Wmf.PS_DASHDOTDOT) == Wmf.PS_DASHDOTDOT)
                                penStyle = DStrokeStyle.DashDotDot;
                            else if ((gdiPenStyle & Wmf.PS_DASHDOT) == Wmf.PS_DASHDOT)
                                penStyle = DStrokeStyle.DashDot;
                            else if ((gdiPenStyle & Wmf.PS_DOT) == Wmf.PS_DOT)
                                penStyle = DStrokeStyle.Dot;
                            else if ((gdiPenStyle & Wmf.PS_DASH) == Wmf.PS_DASH)
                                penStyle = DStrokeStyle.Dash;
                            else
                                penStyle = DStrokeStyle.Solid;

                            if ((gdiPenStyle & Wmf.PS_ENDCAP_FLAT) == Wmf.PS_ENDCAP_FLAT)
                                penCap = DStrokeCap.Butt;
                            else if ((gdiPenStyle & Wmf.PS_ENDCAP_SQUARE) == Wmf.PS_ENDCAP_SQUARE)
                                penCap = DStrokeCap.Square;
                            else 
                                penCap = DStrokeCap.Round;

                            if ((gdiPenStyle & Wmf.PS_JOIN_MITER) == Wmf.PS_JOIN_MITER)
                                penJoin = DStrokeJoin.Mitre;
                            else if ((gdiPenStyle & Wmf.PS_JOIN_BEVEL) == Wmf.PS_JOIN_BEVEL)
                                penJoin = DStrokeJoin.Bevel;
                            else
                                penJoin = DStrokeJoin.Round;

                            if ((gdiPenStyle & Wmf.PS_NULL) == Wmf.PS_NULL)
                                WmfAddGdiObject(new WmfGdiPen(DColor.Empty, penWidth, penStyle, penCap, penJoin));
                            else
                                WmfAddGdiObject(new WmfGdiPen(penColor, penWidth, penStyle, penCap, penJoin));
                            break;
                        case Wmf.CreateFontIndirect:
                            WmfAddGdiObject("font");
                            break;
                        case Wmf.CreateBrushIndirect:
                            int brushStyle = GetInt16();
                            DColor brushColor = GetColor();
                            int brushHatch = GetInt16();
                            if ((brushStyle & Wmf.BS_NULL) == Wmf.BS_NULL)
                                WmfAddGdiObject(new WmfGdiBrush(DColor.Empty));
                            else
                                WmfAddGdiObject(new WmfGdiBrush(brushColor));
                            break;
                        case Wmf.Polygon:
                            DPoints polygonPts = GetPolyPoints(GetInt16(), true);
                            if (FillValid)
                                dg.FillPolygon(polygonPts, fill, 1, fillRule);
                            if (StrokeValid)
                                dg.DrawPolyline(polygonPts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case Wmf.Polyline:
                            DPoints polylinePts = GetPolyPoints(GetInt16(), false);
                            if (StrokeValid)
                                dg.DrawPolyline(polylinePts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case Wmf.Ellipse:
                            goto case Wmf.Rectangle;
                        case Wmf.Rectangle:
                            DPoint br = GetPoint();
                            DPoint tl = GetPoint();
                            if (FillValid)
                            {
                                if (fh.Function == Wmf.Rectangle)
                                    dg.FillRect(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, fill, 1);
                                else if (fh.Function == Wmf.Ellipse)
                                    dg.FillEllipse(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, fill, 1);
                            }
                            if (StrokeValid)
                            {
                                if (fh.Function == Wmf.Rectangle)
                                    dg.DrawRect(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, stroke, 1, strokeWidth, strokeStyle, strokeJoin);
                                else if (fh.Function == Wmf.Ellipse)
                                    dg.DrawEllipse(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, stroke, 1, strokeWidth, strokeStyle);
                            }
                            break;
                        case Wmf.PolyPolygon:
                            // find out how many points
                            int numPolygons = GetInt16();
                            int[] numPoints = new int[numPolygons];
                            for (int i = 0; i < numPolygons; i++)
                                numPoints[i] = GetInt16();
                            // join polygons together
                            DPoints polyPolyPoints = new DPoints();
                            for (int i = 0; i < numPolygons; i++)
                                foreach (DPoint polyPolyPt in GetPolyPoints(numPoints[i], true))
                                    polyPolyPoints.Add(polyPolyPt);
                            // draw
                            if (FillValid)
                                dg.FillPolygon(polyPolyPoints, fill, 1, fillRule);
                            if (StrokeValid)
                                dg.DrawPolyline(polyPolyPoints, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case Wmf.Escape:
                            break;
                        default:
                            breakme = true;
                            break;
                    }

                    if (bytesRead != bytesReadPlusFuncHeaderSize)
                        bytesRead = bytesReadPlusFuncHeaderSize;
                    if (breakme)
                        break;
                }
            }
            dg.Restore();
        }

        bool FillValid
        {
            get { return !fill.IsEmpty; }
        }

        bool StrokeValid
        {
            get { return strokeWidth > 0 && !stroke.IsEmpty; }
        }

        DPoint GetWmfSize()
        {
            if (WmfIsPlaceable)
            {
                bytesRead = 0;
                placeable = GetWmfPlaceable();
                header = GetWmfHeader();
                bool records = true;
                while (records)
                {
                    int bytesReadPlusFuncHeaderSize = bytesRead;
                    Wmf.FuncHeader fh = GetWmfFuncHeader();
                    bytesReadPlusFuncHeaderSize += (int)fh.Size * 2;
                    switch (fh.Function)
                    {
                        case Wmf.SetWindowOrg:
                            winY = GetInt16();
                            winX = GetInt16();
                            break;
                        case Wmf.SetWindowExt:
                            winHeight = GetInt16();
                            winWidth = GetInt16();
                            break;
                        case Wmf.EOF:
                            records = false;
                            // this does not always work too well (there are some wacky values in wmf files)
                            double width = Math.Abs(winWidth);
                            if (winWidth != Math.Round((placeable.Right - placeable.Left) * ((double)PageTools.DpiX / placeable.Inch)))
                                width *= (double)PageTools.DpiX / placeable.Inch;
                            double height = Math.Abs(winHeight);
                            if (winHeight != Math.Round((placeable.Bottom - placeable.Top) * ((double)PageTools.DpiY / placeable.Inch)))
                                height *= (double)PageTools.DpiY / placeable.Inch;
                            return new DPoint(width, height);
                    }
                    if (bytesRead != bytesReadPlusFuncHeaderSize)
                        bytesRead = bytesReadPlusFuncHeaderSize;
                }
            }
            return new DPoint(0, 0);
        }
        #endregion
    }
}
