using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using DejaVu;

namespace DDraw
{
    public enum DMetafileType { Wmf };

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
            if (MetafileType == DMetafileType.Wmf && ImageData.Length >= Marshal.SizeOf(typeof(WmfPlaceable)))
                PaintWmf(dg);
        }

        #region Wmf Code
        // see http://sk1project.org/index.php for python wmf code
        // see http://msdn.microsoft.com/en-us/library/cc215212.aspx for microsoft documentation
        // see http://wvware.sourceforge.net/libwmf.html for some more wmf code

        const UInt32 WmfPlaceableKey = 0x9AC6CDD7;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WmfHeader
        {
            public UInt16 Type, HeaderSize, Version;
            public UInt32 Filesize;
            public UInt16 NumOfObjects;
            public UInt32 MaxRecord;
            public UInt16 NumOfMembers; // not used
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WmfPlaceable
        {
            public UInt32 Key;
            public UInt16 HWmf;
            public UInt16 Left, Top, Right, Bottom;
            public UInt16 Inch;
            public UInt32 Reserved;
            public UInt16 Checksum;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WmfFuncHeader
        {
            public UInt32 Size;
            public UInt16 Function;
        }

        // Function consts
        const int WmfSaveDC = 0x001E;
        const int WmfCreatePalette = 0x00F7;
        const int WmfSetBkMode = 0x0102;
        const int WmfSetMapMode = 0x0103;
        const int WmfSetROP2 = 0x0104;
        const int WmfSetRelabs = 0x0105;
        const int WmfSetPolyFillMode = 0x0106;
        const int WmfSetStretchBltMode = 0x0107;
        const int WmfDeleteObject = 0x01F0;
        const int WmfRestoreDC = 0x0127;
        const int WmfSelectObject = 0x012D;
        const int WmfSetTextAlign = 0x012E;
        const int WmfSetBkColor = 0x0201;
        const int WmfSetTextColor = 0x0209;
        const int WmfSetWindowOrg = 0x020B;
        const int WmfSetWindowExt = 0x020C;
        const int WmfSetViewportOrg = 0x020D;
        const int WmfSetViewportExt = 0x020E;
        const int WmfLineTo = 0x0213;
        const int WmfMoveTo = 0x0214;
        const int WmfSelectPalette = 0x0234;
        const int WmfCreatePenIndirect = 0x02FA;
        const int WmfCreateFontIndirect = 0x02FB;
        const int WmfCreateBrushIndirect = 0x02FC;
        const int WmfPolygon = 0x0324;
        const int WmfPolyline = 0x0325;
        const int WmfRectangle = 0x041B;
        const int WmfPolyPolygon = 0x0538;
        const int WmfEscape = 0x0626;

        // Brush Style consts
        const int WMF_BS_SOLID = 0x0000;
        const int WMF_BS_NULL = 0x0001;
        const int WMF_BS_HATCHED = 0x0002;
        const int WMF_BS_PATTERN = 0x0003;
        const int WMF_BS_INDEXED = 0x0004;
        const int WMF_BS_DIBPATTERN = 0x0005;
        const int WMF_BS_DIBPATTERNPT = 0x0006;
        const int WMF_BS_PATTERN8X8 = 0x0007;
        const int WMF_BS_DIBPATTERN8X8 = 0x0008;
        const int WMF_BS_MONOPATTERN = 0x0009;

        // Pen Style consts
        const int WMF_PS_COSMETIC = 0x0000;
        const int WMF_PS_ENDCAP_ROUND = 0x0000;
        const int WMF_PS_JOIN_ROUND = 0x0000;
        const int WMF_PS_SOLID = 0x0000;
        const int WMF_PS_DASH = 0x0001;
        const int WMF_PS_DOT = 0x0002;
        const int WMF_PS_DASHDOT = 0x0003;
        const int WMF_PS_DASHDOTDOT = 0x0004;
        const int WMF_PS_NULL = 0x0005;
        const int WMF_PS_INSIDEFRAME = 0x0006;
        const int WMF_PS_USERSTYLE = 0x0007;
        const int WMF_PS_ALTERNATE = 0x0008;
        const int WMF_PS_ENDCAP_SQUARE = 0x0100;
        const int WMF_PS_ENDCAP_FLAT = 0x0200;
        const int WMF_PS_JOIN_BEVEL = 0x1000;
        const int WMF_PS_JOIN_MITER = 0x2000;

        WmfHeader GetWmfHeader()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(WmfHeader));
            bytesRead += Marshal.SizeOf(typeof(WmfHeader));
            return (WmfHeader)obj;
        }

        WmfPlaceable GetWmfPlaceable()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(WmfPlaceable));
            bytesRead += Marshal.SizeOf(typeof(WmfPlaceable));
            return (WmfPlaceable)obj;
        }

        int WmfFuncHeaderSize = Marshal.SizeOf(typeof(WmfFuncHeader)); // cache so we dont have to call a million times
        WmfFuncHeader GetWmfFuncHeader()
        {
            object obj = RawDeserialize(ImageData, bytesRead, typeof(WmfFuncHeader));
            bytesRead += WmfFuncHeaderSize;
            return (WmfFuncHeader)obj;
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

        WmfPlaceable placeable;
        WmfHeader header;
        int winX, winY, winWidth, winHeight;
        int viewX, viewY, viewWidth, viewHeight;

        bool IsPlaceable
        {
            get { return BitConverter.ToUInt32(ImageData, 0) == WmfPlaceableKey; }
        }

        private void PaintWmf(DGraphics dg)
        {
            dg.Save();
            dg.Clip(Rect);
            // store current matrix
            DMatrix m = dg.SaveTransform();
            // check for placeable key
            if (IsPlaceable)
            {
                bytesRead = 0;
                // read placeable header
                placeable = GetWmfPlaceable();
                // checksum
                int sum = 0;
                for (int i = 0; i < (Marshal.SizeOf(typeof(WmfPlaceable)) - 2 /* WmfPlaceable.Checksum is UInt16 */) / 2; i++)
                    sum = sum ^ BitConverter.ToUInt16(ImageData, i * 2);
                if (sum != placeable.Checksum)
                    System.Diagnostics.Debug.Fail("checksum failed");
                // init matrix
                WmfUpdateMaxtrix(dg, m);
                // read header
                header = GetWmfHeader();
                // drawing primatives
                DColor fill = DColor.Empty;
                DColor stroke = DColor.Empty;
                double strokeWidth = 1;
                DStrokeStyle strokeStyle = DStrokeStyle.Solid;
                DStrokeCap strokeCap = DStrokeCap.Butt;
                DStrokeJoin strokeJoin = DStrokeJoin.Bevel;
                DPoint curPoint = new DPoint(0, 0);
                // iterate draw commands
                bool records = true;
                while (records)
                {
                    int bytesReadPlusFuncHeaderSize = bytesRead;
                    WmfFuncHeader fh = GetWmfFuncHeader();
                    bytesReadPlusFuncHeaderSize += (int)fh.Size * 2;
                    bool breakme = false;
                    switch (fh.Function)
                    {
                        case WmfSaveDC:
                            break;
                        case WmfCreatePalette:
                            break;
                        case WmfSetBkMode:
                            int bkMode = GetInt16();
                            break;
                        case WmfSetMapMode:
                            int mapMode = GetInt16();
                            break;
                        case WmfSetROP2:
                            break;
                        case WmfSetRelabs:
                            break;
                        case WmfSetPolyFillMode:
                            int polyfillMode = GetInt16();
                            break;
                        case WmfSetStretchBltMode:
                            break;
                        case WmfDeleteObject:
                            int index = GetInt16();
                            break;
                        case WmfRestoreDC:
                            break;
                        case WmfSelectObject:
                            int index2 = GetInt16();
                            break;
                        case WmfSetTextAlign:
                            break;
                        case WmfSetBkColor:
                            break;
                        case WmfSetTextColor:
                            break;
                        case WmfSetWindowOrg:
                            winY = GetInt16();
                            winX = GetInt16();
                            WmfUpdateMaxtrix(dg, m);
                            break;
                        case WmfSetWindowExt:                            
                            winHeight = GetInt16();
                            winWidth = GetInt16();
                            WmfUpdateMaxtrix(dg, m);
                            break;
                        case WmfSetViewportOrg:
                            viewY = GetInt16();
                            viewX = GetInt16();
                            //wmfUpdateMaxtrix(dg, m);
                            break;
                        case WmfSetViewportExt:
                            viewHeight = GetInt16();
                            viewWidth = GetInt16();
                            //wmfUpdateMaxtrix(dg, m);
                            break;
                        case WmfLineTo:
                            DPoint pt = GetPoint();
                            dg.DrawLine(curPoint, pt, stroke, 1, strokeStyle, strokeWidth, strokeCap);
                            curPoint = pt;
                            break;
                        case WmfMoveTo:
                            curPoint = GetPoint();
                            break;
                        case WmfSelectPalette:
                            break;
                        case WmfCreatePenIndirect:
                            int penStyle = GetInt16();
                            int widthX = GetInt16();
                            int widthY = GetInt16();
                            DColor penColor = GetColor();

                            if ((penStyle & WMF_PS_SOLID) == WMF_PS_SOLID)
                                strokeStyle = DStrokeStyle.Solid;
                            else if ((penStyle & WMF_PS_DASH) == WMF_PS_DASH)
                                strokeStyle = DStrokeStyle.Dash;
                            else if ((penStyle & WMF_PS_DOT) == WMF_PS_DOT)
                                strokeStyle = DStrokeStyle.Dot;
                            else if ((penStyle & WMF_PS_DASHDOT) == WMF_PS_DASHDOT)
                                strokeStyle = DStrokeStyle.DashDot;
                            else if ((penStyle & WMF_PS_DASHDOTDOT) == WMF_PS_DASHDOTDOT)
                                strokeStyle = DStrokeStyle.DashDotDot;

                            if ((penStyle & WMF_PS_ENDCAP_FLAT) == WMF_PS_ENDCAP_FLAT)
                                strokeCap = DStrokeCap.Butt;
                            else if ((penStyle & WMF_PS_ENDCAP_ROUND) == WMF_PS_ENDCAP_ROUND)
                                strokeCap = DStrokeCap.Round;
                            else if ((penStyle & WMF_PS_ENDCAP_SQUARE) == WMF_PS_ENDCAP_SQUARE)
                                strokeCap = DStrokeCap.Square;

                            if ((penStyle & WMF_PS_JOIN_BEVEL) == WMF_PS_JOIN_BEVEL)
                                strokeJoin = DStrokeJoin.Bevel;
                            else if ((penStyle & WMF_PS_JOIN_MITER) == WMF_PS_JOIN_MITER)
                                strokeJoin = DStrokeJoin.Mitre;
                            else if ((penStyle & WMF_PS_JOIN_ROUND) == WMF_PS_JOIN_ROUND)
                                strokeJoin = DStrokeJoin.Round;

                            if ((penStyle & WMF_PS_NULL) == WMF_PS_NULL)
                                stroke = DColor.Empty;
                            else
                                stroke = penColor;
                            strokeWidth = widthX;
                            break;
                        case WmfCreateFontIndirect:
                            break;
                        case WmfCreateBrushIndirect:
                            int brushStyle = GetInt16();
                            DColor brushColor = GetColor();
                            int brushHatch = GetInt16();
                            if ((brushStyle & WMF_BS_NULL) == WMF_BS_NULL)
                                fill = DColor.Empty;
                            else
                                fill = brushColor;
                            break;
                        case WmfPolygon:
                            DPoints polygonPts = GetPolyPoints(GetInt16(), true);
                            dg.FillPolygon(polygonPts, fill, 1);
                            dg.DrawPolyline(polygonPts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case WmfPolyline:
                            DPoints polylinePts = GetPolyPoints(GetInt16(), false);
                            dg.DrawPolyline(polylinePts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case WmfRectangle:
                            DPoint br = GetPoint();
                            DPoint tl = GetPoint();
                            dg.DrawRect(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, fill, 1, strokeWidth, strokeStyle, strokeJoin);
                            break;
                        case WmfPolyPolygon:
                            int numPolygons = GetInt16();
                            int[] numPoints = new int[numPolygons];
                            for (int i = 0; i < numPolygons; i++)
                                numPoints[i] = GetInt16();
                            for (int i = 0; i < numPolygons; i++)
                            {
                                DPoints polygonPts2 = GetPolyPoints(numPoints[i], true);
                                dg.FillPolygon(polygonPts2, fill, 1);
                                dg.DrawPolyline(polygonPts2, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            }
                            break;
                        case WmfEscape:
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

        DPoint GetWmfSize()
        {
            if (IsPlaceable)
            {
                bytesRead = 0;
                placeable = GetWmfPlaceable();
                header = GetWmfHeader();
                bool records = true;
                while (records)
                {
                    int bytesReadPlusFuncHeaderSize = bytesRead;
                    WmfFuncHeader fh = GetWmfFuncHeader();
                    bytesReadPlusFuncHeaderSize += (int)fh.Size * 2;
                    switch (fh.Function)
                    {
                        case WmfSetWindowOrg:
                            winY = GetInt16();
                            winX = GetInt16();
                            break;
                        case WmfSetWindowExt:
                            winHeight = GetInt16();
                            winWidth = GetInt16();
                            break;
                        case 0:
                            records = false;
                            // this does not always work too well (there are some wacky values in wmf files)
                            return new DPoint(Math.Abs(winWidth) - Math.Max(0, winX), 
                                Math.Abs(winHeight) - Math.Max(0, winY));
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
