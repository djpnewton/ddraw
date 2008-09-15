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
        const int WmfEllipse = 0x0418;
        const int WmfRectangle = 0x041B;
        const int WmfPolyPolygon = 0x0538;
        const int WmfEscape = 0x0626;

        // PolyFillMode consts
        const int WmfPfmAlernate = 0x0001;
        const int WmfPfmWinding = 0x0002;

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
        WmfPlaceable placeable;
        WmfHeader header;
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
            get { return BitConverter.ToUInt32(ImageData, 0) == WmfPlaceableKey; }
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
                for (int i = 0; i < (Marshal.SizeOf(typeof(WmfPlaceable)) - 2 /* WmfPlaceable.Checksum is UInt16 */) / 2; i++)
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
                            if (polyfillMode == WmfPfmAlernate)
                                fillRule = DFillRule.EvenOdd;
                            else
                                fillRule = DFillRule.Winding;
                            break;
                        case WmfSetStretchBltMode:
                            break;
                        case WmfDeleteObject:
                            WmfDeleteGdiObject(GetInt16());
                            break;
                        case WmfRestoreDC:
                            break;
                        case WmfSelectObject:
                            WmfSelectGdiObject(GetInt16());
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
                            if (StrokeValid)
                                dg.DrawLine(curPoint, pt, stroke, 1, strokeStyle, strokeWidth, strokeCap);
                            curPoint = pt;
                            break;
                        case WmfMoveTo:
                            curPoint = GetPoint();
                            break;
                        case WmfSelectPalette:
                            break;
                        case WmfCreatePenIndirect:
                            int gdiPenStyle = GetInt16();
                            int widthX = GetInt16();
                            int widthY = GetInt16();
                            DColor penColor = GetColor();
                            double penWidth = widthX;
                            WmfApplyTransforms(ref penWidth);
                            DStrokeStyle penStyle;
                            DStrokeCap penCap;
                            DStrokeJoin penJoin;

                            if ((gdiPenStyle & WMF_PS_SOLID) == WMF_PS_SOLID)
                                penStyle = DStrokeStyle.Solid;
                            else if ((gdiPenStyle & WMF_PS_DASH) == WMF_PS_DASH)
                                penStyle = DStrokeStyle.Dash;
                            else if ((gdiPenStyle & WMF_PS_DOT) == WMF_PS_DOT)
                                penStyle = DStrokeStyle.Dot;
                            else if ((gdiPenStyle & WMF_PS_DASHDOT) == WMF_PS_DASHDOT)
                                penStyle = DStrokeStyle.DashDot;
                            else if ((gdiPenStyle & WMF_PS_DASHDOTDOT) == WMF_PS_DASHDOTDOT)
                                penStyle = DStrokeStyle.DashDotDot;

                            if ((gdiPenStyle & WMF_PS_ENDCAP_FLAT) == WMF_PS_ENDCAP_FLAT)
                                penCap = DStrokeCap.Butt;
                            else if ((gdiPenStyle & WMF_PS_ENDCAP_ROUND) == WMF_PS_ENDCAP_ROUND)
                                penCap = DStrokeCap.Round;
                            else if ((gdiPenStyle & WMF_PS_ENDCAP_SQUARE) == WMF_PS_ENDCAP_SQUARE)
                                penCap = DStrokeCap.Square;

                            if ((gdiPenStyle & WMF_PS_JOIN_BEVEL) == WMF_PS_JOIN_BEVEL)
                                penJoin = DStrokeJoin.Bevel;
                            else if ((gdiPenStyle & WMF_PS_JOIN_MITER) == WMF_PS_JOIN_MITER)
                                penJoin = DStrokeJoin.Mitre;
                            else if ((gdiPenStyle & WMF_PS_JOIN_ROUND) == WMF_PS_JOIN_ROUND)
                                penJoin = DStrokeJoin.Round;

                            if ((gdiPenStyle & WMF_PS_NULL) == WMF_PS_NULL)
                                WmfAddGdiObject(new WmfGdiPen(DColor.Empty, penWidth, penStyle, penCap, penJoin));
                            else
                                WmfAddGdiObject(new WmfGdiPen(penColor, penWidth, penStyle, penCap, penJoin));
                            break;
                        case WmfCreateFontIndirect:
                            WmfAddGdiObject("font");
                            break;
                        case WmfCreateBrushIndirect:
                            int brushStyle = GetInt16();
                            DColor brushColor = GetColor();
                            int brushHatch = GetInt16();
                            if ((brushStyle & WMF_BS_NULL) == WMF_BS_NULL)
                                WmfAddGdiObject(new WmfGdiBrush(DColor.Empty));
                            else
                                WmfAddGdiObject(new WmfGdiBrush(brushColor));
                            break;
                        case WmfPolygon:
                            DPoints polygonPts = GetPolyPoints(GetInt16(), true);
                            if (FillValid)
                                dg.FillPolygon(polygonPts, fill, 1, fillRule);
                            if (StrokeValid)
                                dg.DrawPolyline(polygonPts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case WmfPolyline:
                            DPoints polylinePts = GetPolyPoints(GetInt16(), false);
                            if (StrokeValid)
                                dg.DrawPolyline(polylinePts, stroke, 1, strokeWidth, strokeStyle, strokeJoin, strokeCap);
                            break;
                        case WmfEllipse:
                            goto case WmfRectangle;
                        case WmfRectangle:
                            DPoint br = GetPoint();
                            DPoint tl = GetPoint();
                            if (StrokeValid)
                            {
                                if (fh.Function == WmfRectangle)
                                    dg.DrawRect(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, stroke, 1, strokeWidth, strokeStyle, strokeJoin);
                                else if (fh.Function == WmfEllipse)
                                    dg.DrawEllipse(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y, stroke, 1, strokeWidth, strokeStyle);
                            }
                            break;
                        case WmfPolyPolygon:
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
