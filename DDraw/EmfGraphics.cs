using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace DDraw
{
    public class EmfGraphics : DGraphics
    {
        MemoryStream ms;
        uint numRecords = 0;
        ushort maxNumOfObjects = 0;

        DMatrix currentMatrix = DMatrix.Identity();
        Stack<DMatrix> matrixStack = new Stack<DMatrix>();

        public EmfGraphics(DRect bounds, DPoint screenMM, DPoint deviceRes)
        {
            screenMM.X = 320;
            screenMM.Y = 200;

            // create memory stream 
            ms = new MemoryStream();
            // write header
            Emf.MetafileHeaderExtension2 h = new Emf.MetafileHeaderExtension2();
            h.EmfHeader.Type = 1;
            h.EmfHeader.Size = (uint)Marshal.SizeOf(typeof(Emf.MetafileHeaderExtension2));
            h.EmfHeader.Bounds = new Emf.RectL((int)bounds.X, (int)bounds.Y, (int)bounds.Right, (int)bounds.Bottom);
            double pixelWidth = screenMM.X / deviceRes.X;
            double pixelHeight = screenMM.Y / deviceRes.Y;
            h.EmfHeader.Frame = new Emf.RectL((int)(bounds.X * 100 * pixelWidth), 
                (int)(bounds.Y * 100 * pixelHeight), 
                (int)(bounds.Right * 100 * pixelWidth), 
                (int)(bounds.Bottom * 100 * pixelHeight));
            h.EmfHeader.Signature = (uint)Emf.FormatSignature.ENHMETA_SIGNATURE;
            h.EmfHeader.Version = 0x00010000;
            h.EmfHeader.Bytes = 0; // size of metafile (set later on)
            h.EmfHeader.Records = 0; // num of records in metafile (set later on)
            h.EmfHeader.Handles = 0; // max number of gdi objects used at one time (set later on)
            h.EmfHeader.Reserved = 0;
            h.EmfHeader.nDescription = 0;
            h.EmfHeader.offDescription = 0;
            h.EmfHeader.nPalEntries = 0; // set later on
            h.EmfHeader.Device = new Wmf.SizeL((uint)deviceRes.X, (uint)deviceRes.Y);
            h.EmfHeader.Millimeters = new Wmf.SizeL((uint)screenMM.X, (uint)screenMM.Y);
            h.EmfHeaderExtension1.cbPixelFormat = 0;
            h.EmfHeaderExtension1.offPixelFormat = 0;
            h.EmfHeaderExtension1.bOpenGL = 0;
            h.EmfHeaderExtension2.MicrometersX = (uint)(screenMM.X * 1000);
            h.EmfHeaderExtension2.MicrometersY = (uint)(screenMM.Y * 1000);
            byte[] data = RawSerialize(h);
            ms.Write(data, 0, data.Length);

            WriteRecordHeader(Emf.RecordType.EMR_SETMAPMODE, 12);
            WriteUInt(0x08); // MM_ANISOTROPIC

            WriteRecordHeader(Emf.RecordType.EMR_SETWINDOWORGEX, 16);
            WritePointL(new Wmf.PointL(h.EmfHeader.Bounds.Left, h.EmfHeader.Bounds.Top));
            WriteRecordHeader(Emf.RecordType.EMR_SETWINDOWEXTEX, 16);
            WriteSizeL(new Wmf.SizeL((uint)(h.EmfHeader.Bounds.Right - h.EmfHeader.Bounds.Left),
                (uint)(h.EmfHeader.Bounds.Bottom - h.EmfHeader.Bounds.Top)));
            WriteRecordHeader(Emf.RecordType.EMR_SETVIEWPORTORGEX, 16);
            WritePointL(new Wmf.PointL(h.EmfHeader.Bounds.Left, h.EmfHeader.Bounds.Top));
            WriteRecordHeader(Emf.RecordType.EMR_SETVIEWPORTEXTEX, 16);
            WriteSizeL(new Wmf.SizeL((uint)(h.EmfHeader.Bounds.Right - h.EmfHeader.Bounds.Left),
                (uint)(h.EmfHeader.Bounds.Bottom - h.EmfHeader.Bounds.Top)));
        }

        public void SaveToFile(string fileName)
        {
            byte[] data = EmfData;
            // write data to disk
            if (File.Exists(fileName))
                File.Delete(fileName);
            FileStream fs = File.Create(fileName);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public byte[] EmfData
        {
            get
            {
                // write end of file wmf func to memory stream
                WriteRecordHeader(Emf.RecordType.EMR_EOF, 20);
                WriteUInt(0);
                WriteUInt(0);
                WriteUInt(0);
                // get data from memory stream
                byte[] data = ms.ToArray();
                // modify Emf.Header structure in data
                GCHandle hData = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr p = hData.AddrOfPinnedObject();
                Emf.Header h = (Emf.Header)Marshal.PtrToStructure(p, typeof(Emf.Header));
                System.Diagnostics.Debug.Assert(h.Signature == (uint)Emf.FormatSignature.ENHMETA_SIGNATURE, "ERROR: Emf Header signature is wrong :(");
                h.Bytes = (uint)(data.Length);         
                h.Records = numRecords;                
                h.Handles = (ushort)(maxNumOfObjects + 1);           
                Marshal.StructureToPtr(h, p, false);
                hData.Free();
                // return data
                return data;
            }
        }

        #region Helper Functions

        byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }

        void WriteRecordHeader(Emf.RecordType record, uint size)
        {
            Emf.RecordHeader r;
            r.Type = (uint)record;
            r.Size = size;
            byte[] buf = RawSerialize(r);
            ms.Write(buf, 0, buf.Length);
            // need this for Emf.Header
            numRecords++;
        }

        void WriteUInt(uint num)
        {
            byte[] buf = BitConverter.GetBytes(num);
            ms.Write(buf, 0, buf.Length);
        }

        void WriteFloat(float num)
        {
            byte[] buf = BitConverter.GetBytes(num);
            ms.Write(buf, 0, buf.Length);
        }

        void WriteSizeL(Wmf.SizeL sz)
        {
            byte[] buf = RawSerialize(sz);
            ms.Write(buf, 0, buf.Length);
        }

        void WriteColor(DColor col)
        {
            byte[] buf = new byte[] { col.R, col.G, col.B, 0 };
            ms.Write(buf, 0, buf.Length);
        }

        uint StrokePenStyle(DStrokeStyle ss, DStrokeJoin sj, DStrokeCap sc)
        {
            Emf.PenStyle _ss = 0;
            switch (ss)
            {
                case DStrokeStyle.Dash:
                    _ss = Emf.PenStyle.PS_DASH;
                    break;
                case DStrokeStyle.DashDot:
                    _ss = Emf.PenStyle.PS_DASHDOT;
                    break;
                case DStrokeStyle.DashDotDot:
                    _ss = Emf.PenStyle.PS_DASHDOTDOT;
                    break;
                case DStrokeStyle.Dot:
                    _ss = Emf.PenStyle.PS_DOT;
                    break;
                case DStrokeStyle.Solid:
                    _ss = Emf.PenStyle.PS_SOLID;
                    break;
            }
            Emf.PenStyle _sj = 0;
            switch (sj)
            {
                case DStrokeJoin.Bevel:
                    _sj = Emf.PenStyle.PS_JOIN_BEVEL;
                    break;
                case DStrokeJoin.Mitre:
                    _sj = Emf.PenStyle.PS_JOIN_MITER;
                    break;
                case DStrokeJoin.Round:
                    _sj = Emf.PenStyle.PS_JOIN_ROUND;
                    break;
            }
            Emf.PenStyle _sc = 0;
            switch (sc)
            {
                case DStrokeCap.Butt:
                    _sc = Emf.PenStyle.PS_ENDCAP_FLAT;
                    break;
                case DStrokeCap.Round:
                    _sc = Emf.PenStyle.PS_ENDCAP_ROUND;
                    break;
                case DStrokeCap.Square:
                    _sc = Emf.PenStyle.PS_ENDCAP_SQUARE;
                    break;
            }
            return (uint)(_ss | _sj | _sc);
        }

        void CreatePen(uint objectIndex, double sw, DColor col, DStrokeStyle ss, DStrokeJoin sj, DStrokeCap sc)
        {
            WriteRecordHeader(Emf.RecordType.EMR_CREATEPEN, 28);
            WriteUInt(objectIndex);
            WriteUInt(StrokePenStyle(ss, sj, sc));
            WriteUInt((uint)sw);
            WriteUInt((uint)sw);
            WriteColor(col);
        }

        void ExtCreatePen(uint objectIndex, double sw, DColor col, DStrokeStyle ss, DStrokeJoin sj, DStrokeCap sc)
        {
            WriteRecordHeader(Emf.RecordType.EMR_EXTCREATEPEN, 52);
            WriteUInt(objectIndex);
            WriteUInt(0);
            WriteUInt(0);
            WriteUInt(0);
            WriteUInt(0);
            // LogPenEx
            WriteUInt(StrokePenStyle(ss, sj, sc));
            WriteUInt((uint)sw);
            WriteUInt(Wmf.BS_SOLID);
            WriteColor(col);
            WriteUInt(0);
            WriteUInt(0);
        }

        void CreateBrushIndirect(uint objectIndex, DColor col)
        {
            WriteRecordHeader(Emf.RecordType.EMR_CREATEBRUSHINDIRECT, 24);
            WriteUInt(objectIndex);
            WriteUInt(Wmf.BS_SOLID);
            WriteColor(col);
            WriteUInt(0); // BrushHatch not supported
        }

        void ExtCreateFontIndirectW(uint objectIndex, string fontName, double fontSize, bool bold, bool italic, bool underline, bool strikethrough)
        {
            WriteRecordHeader(Emf.RecordType.EMR_EXTCREATEFONTINDIRECTW, 104);
            WriteUInt(objectIndex);
            // LogFont
            WriteUInt((uint)fontSize); //Height?
            WriteUInt(0); //Width?
            WriteUInt(0); //Escapement?
            WriteUInt(0); //Orientation?
            if (bold)
            WriteUInt(700);
            else
                WriteUInt(400);
            if (italic)
                ms.WriteByte(1);
            else
                ms.WriteByte(0);
            if (underline)
                ms.WriteByte(1);
            else
                ms.WriteByte(0);
            if (strikethrough)
                ms.WriteByte(1);
            else
                ms.WriteByte(0);
            ms.WriteByte(1); // default character set
            ms.WriteByte(0); // default out precision
            ms.WriteByte(0); // default clip precision
            ms.WriteByte(0); // default quality
            ms.WriteByte(0); // default pitch and family
            UnicodeEncoding enc = new UnicodeEncoding();
            byte[] strBytes = enc.GetBytes(fontName);
            if (strBytes.Length < 64)
            {
                ms.Write(strBytes, 0, strBytes.Length);
                byte[] pad = new byte[64 - strBytes.Length];
                ms.Write(pad, 0, pad.Length);
            }
            else
                ms.Write(strBytes, 0, 64);
        }

        void SelectObject(uint num)
        {
            WriteRecordHeader(Emf.RecordType.EMR_SELECTOBJECT, 12);
            WriteUInt(num);
        }

        void DeleteObject(uint num)
        {
            WriteRecordHeader(Emf.RecordType.EMR_DELETEOBJECT, 12);
            WriteUInt(num);
        }

        void Rectangle(double x, double y, double width, double height)
        {
            WriteRecordHeader(Emf.RecordType.EMR_RECTANGLE, 24);
            WriteRectL(x, y, width, height);
        }

        void Ellipse(double x, double y, double width, double height)
        {
            WriteRecordHeader(Emf.RecordType.EMR_ELLIPSE, 24);
            WriteRectL(x, y, width, height);
        }

        void ExtTextOutW(string text, DPoint pt)
        {
            // encode text to utf16 and make padding
            UnicodeEncoding enc = new UnicodeEncoding();
            byte[] textBytes = enc.GetBytes(text);
            int padSize = 0;
            if (textBytes.Length % 8 != 0)
                padSize = 8 - textBytes.Length % 8;
            byte[] padBytes = new byte[padSize];
            // write TextOut record
            WriteRecordHeader(Emf.RecordType.EMR_EXTTEXTOUTW, (uint)(76 + textBytes.Length + padBytes.Length));
            WriteRectL(0, 0, -1,-1); // bounds
            WriteUInt(1); // GM_COMPATIBLE
            WriteFloat(1); // exScale
            WriteFloat(1); // eyScale
            // EmrText object
            WritePointL(pt); // Reference
            WriteUInt((uint)(text.Length)); // Chars
            WriteUInt(76); // offString
            WriteUInt(0); // Options
            WriteRectL(0, 0, -1, -1); // Rectangle
            WriteUInt(0); // offDx
            ms.Write(textBytes, 0, textBytes.Length); // OutputString
            ms.Write(padBytes, 0, padBytes.Length); // pad
        }

        void WriteRectL(double x, double y, double width, double height)
        {
            Emf.RectL r = new Emf.RectL();
            r.Left = (int)x;
            r.Top = (int)y;
            r.Right = (int)(x + width);
            r.Bottom = (int)(y + height);
            WriteRectL(r);
        }

        void WriteRectL(Emf.RectL r)
        {
            byte[] data = RawSerialize(r);
            ms.Write(data, 0, data.Length);
        }

        void MoveTo(DPoint pt)
        {
            WriteRecordHeader(Emf.RecordType.EMR_MOVETOEX, 16);
            WritePointL(pt);
        }

        void LineTo(DPoint pt)
        {
            WriteRecordHeader(Emf.RecordType.EMR_LINETO, 16);
            WritePointL(pt);
        }

        void WritePointL(DPoint pt)
        {
            Wmf.PointL ptl = new Wmf.PointL();
            ptl.x = (int)pt.X;
            ptl.y = (int)pt.Y;
            WritePointL(ptl);
        }

        void WritePointL(Wmf.PointL pt)
        {
            byte[] data = RawSerialize(pt);
            ms.Write(data, 0, data.Length);
        }

        void Polyline(DPoints pts)
        {
            WriteRecordHeader(Emf.RecordType.EMR_POLYLINE, (uint)(28 + pts.Count * 8));
            WriteRectL(CalcBounds(pts));
            WriteUInt((uint)pts.Count);
            foreach (DPoint pt in pts)
                WritePointL(pt);
        }

        void Polygon(DPoints pts)
        {
            WriteRecordHeader(Emf.RecordType.EMR_POLYGON, (uint)(28 + pts.Count * 8));
            WriteRectL(CalcBounds(pts));
            WriteUInt((uint)pts.Count);
            foreach (DPoint pt in pts)
                WritePointL(pt);
        }

        Emf.RectL CalcBounds(DPoints pts)
        {
            Emf.RectL r = new Emf.RectL(0, 0, 0, 0);
            if (pts.Count > 0)
            {
                r.Left = (int)pts[0].X;
                r.Top = (int)pts[0].Y;
                r.Right = (int)pts[0].X;
                r.Bottom = (int)pts[0].Y;
            }
            foreach (DPoint pt in pts)
            {
                if (pt.X < r.Left)
                    r.Left = (int)pt.X;
                if (pt.Y < r.Top)
                    r.Top = (int)pt.Y;
                if (pt.X > r.Right)
                    r.Right = (int)pt.X;
                if (pt.Y > r.Bottom)
                    r.Bottom = (int)pt.Y;
            }
            return r;
        }

        void ModifyWorldTransform(DMatrix m)
        {
            WriteRecordHeader(Emf.RecordType.EMR_MODIFYWORLDTRANSFORM, 36);
            Emf.XForm x = new Emf.XForm(m);
            byte[] data = RawSerialize(x);
            ms.Write(data, 0, data.Length);
            WriteUInt((uint)Emf.ModifyWorldTransformMode.MWT_RIGHTMULTIPLY);
        }

        void ResetWorldTransform()
        {
            WriteRecordHeader(Emf.RecordType.EMR_MODIFYWORLDTRANSFORM, 36);
            Emf.XForm x = new Emf.XForm();
            byte[] data = RawSerialize(x);
            ms.Write(data, 0, data.Length);
            WriteUInt((uint)Emf.ModifyWorldTransformMode.MWT_IDENTITY);
        }

        void SetWorldTransform(DMatrix m)
        {
            WriteRecordHeader(Emf.RecordType.EMR_SETWORLDTRANSFORM, 32);
            Emf.XForm x = new Emf.XForm(m);
            byte[] data = RawSerialize(x);
            ms.Write(data, 0, data.Length);
        }

        void UpdateMaxNumOfObjects(ushort num)
        {
            if (num > maxNumOfObjects)
                maxNumOfObjects = num;
        }

        #endregion

        #region Drawing Functions

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
            FillRect(x, y, width, height, color, alpha, DFillStyle.Solid);
        }

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha, DFillStyle fillStyle)
        {
            UpdateMaxNumOfObjects(1);
            CreateBrushIndirect(1, color);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_PEN);
            Rectangle(x, y, width, height);
            DeleteObject(1);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin)
        {
            UpdateMaxNumOfObjects(1);
            CreatePen(1, strokeWidth, color, strokeStyle, DStrokeJoin.Round, DStrokeCap.Round);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_BRUSH);
            Rectangle(x, y, width, height);
            DeleteObject(1);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color)
        {
            DrawRect(x, y, width, height, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            DrawRect(x, y, width, height, color, alpha, strokeWidth, DStrokeStyle.Solid, DStrokeJoin.Round);
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
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1, strokeStyle, DStrokeJoin.Round);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
            FillEllipse(x, y, width, height, color, 1);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            UpdateMaxNumOfObjects(1);
            CreateBrushIndirect(1, color);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_PEN);
            Ellipse(x, y, width, height);
            DeleteObject(1);
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
            UpdateMaxNumOfObjects(1);
            CreatePen(1, strokeWidth, color, strokeStyle, DStrokeJoin.Round, DStrokeCap.Round);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_BRUSH);
            Ellipse(x, y, width, height);
            DeleteObject(1);
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1, DStrokeStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
            DrawLine(pt1, pt2, color, 1, DStrokeStyle.Solid, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha)
        {
            DrawLine(pt1, pt2, color, alpha, DStrokeStyle.Solid, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, 1, strokeStyle, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, alpha, strokeStyle, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle, double strokeWidth, DStrokeCap strokeCap)
        {
            UpdateMaxNumOfObjects(1);
            CreatePen(1, strokeWidth, color, strokeStyle, DStrokeJoin.Round, strokeCap);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_BRUSH);
            MoveTo(pt1);
            LineTo(pt2);
            DeleteObject(1);
        }

        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round, DStrokeCap.Round);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap)
        {
            UpdateMaxNumOfObjects(1);
            CreatePen(1, strokeWidth, color, strokeStyle, strokeJoin, strokeCap);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_BRUSH);
            Polyline(pts);
            DeleteObject(1);
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha)
        {
            FillPolygon(pts, color, alpha, DFillRule.EvenOdd);
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha, DFillRule fillRule)
        {
            UpdateMaxNumOfObjects(1);
            CreateBrushIndirect(1, color);
            SelectObject(1);
            SelectObject((uint)Emf.StockObject.NULL_PEN);
            Polygon(pts);
            DeleteObject(1);
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt, double alpha)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect, double alpha)
        {
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
            UpdateMaxNumOfObjects(2);
            CreateBrushIndirect(1, color);
            SelectObject(1);
            ExtCreateFontIndirectW(2, fontName, fontSize, bold, italics, underline, strikethrough);
            SelectObject(2);
            ExtTextOutW(text, pt);
            DeleteObject(2);
            DeleteObject(1);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
            return new DPoint(100, 100);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough)
        {
            return new DPoint(100, 100);
        }

        public override void StartGroup(double x, double y, double width, double height, double offsetX, double offsetY)
        {
        }

        public override void DrawGroup(double alpha)
        {
        }

        public override DMatrix SaveTransform()
        {
            return currentMatrix;
        }

        public override void LoadTransform(DMatrix matrix)
        {
            if (!currentMatrix.Equals(matrix))
            {
                SetWorldTransform(matrix);
                currentMatrix = matrix;
            }
        }

        public override void Scale(double sx, double sy)
        {
            if (sx != 1 || sy != 1)
            {
                DMatrix m = DMatrix.InitScaleMatrix(sx, sy);
                ModifyWorldTransform(m);
                currentMatrix = currentMatrix.Multiply(m);
            }
        }

        public override void Rotate(double angle, DPoint center)
        {
            if (angle != 0)
            {
                Translate(-center.X, -center.Y);
                DMatrix m = DMatrix.InitRotateMatrix(angle);
                ModifyWorldTransform(m);
                currentMatrix = currentMatrix.Multiply(m);
                Translate(center.X, center.Y);
            }
        }

        public override void Translate(double tx, double ty)
        {
            if (tx != 0 || ty != 0)
            {
                DMatrix m = DMatrix.InitTranslateMatrix(tx, ty);
                ModifyWorldTransform(m);
                currentMatrix = currentMatrix.Multiply(m);
            }
        }

        public override void ResetTransform()
        {
            ResetWorldTransform();
            currentMatrix = DMatrix.Identity();
        }

        public override void Clip(DRect r)
        {
        }

        public override void ResetClip()
        {
        }

        public override DCompositingMode CompositingMode
        {
            get { return DCompositingMode.SourceCopy; }
            set { }
        }

        public override bool AntiAlias
        {
            get { return false; }
            set { }
        }

        public override void Save()
        {
            matrixStack.Push(currentMatrix);
        }

        public override void Restore()
        {
            LoadTransform(matrixStack.Pop());
        }

        #endregion

        public override void Dispose()
        {
        }
    }

    public class WmfGraphics : DGraphics
    {
        MemoryStream ms;
        ushort numOfObjects = 0;
        uint maxRecord = 0;

        double winX, winY, winWidth, winHeight;

        public WmfGraphics(double x1, double y1, double x2, double y2)
        {
            winX = 0;
            winY = 0;
            winWidth = x2 - x1;
            winHeight = y2 - y1;
            // create memory stream 
            ms = new MemoryStream();
            // write placeable
            Wmf.Placeable p = new Wmf.Placeable();
            p.Key = Wmf.PlaceableKey;
            p.HWmf = 0;
            p.Left = (ushort)winX;
            p.Top = (ushort)winY;
            p.Right = (ushort)winWidth;
            p.Bottom = (ushort)winHeight;
            p.Inch = 96;
            p.Reserved = 0;
            // checksum
            int sum = 0;
            byte[] cd = RawSerialize(p);
            for (int i = 0; i < (Marshal.SizeOf(typeof(Wmf.Placeable)) - 2 /* WmfPlaceable.Checksum is UInt16 */) / 2; i++)
                sum = sum ^ BitConverter.ToUInt16(cd, i * 2);
            p.Checksum = (ushort)sum;
            byte[] buf = RawSerialize(p);
            ms.Write(buf, 0, buf.Length);
            // write header 
            Wmf.Header h = new Wmf.Header();
            h.Type = 0x0002;
            h.HeaderSize = (ushort)(Marshal.SizeOf(typeof(Wmf.Header)) / 2);
            h.Version = 0x0300;
            h.Filesize = 0;     // see SaveToFile
            h.NumOfObjects = 0; // see SaveToFile
            h.MaxRecord = 0;    // see SaveToFile
            h.NumOfMembers = 0; // not used
            buf = RawSerialize(h);
            ms.Write(buf, 0, buf.Length);

            // set window space
            WriteFuncHeader(5, Wmf.SetWindowOrg);
            WriteUShort((ushort)winX);
            WriteUShort((ushort)winY);
            WriteFuncHeader(5, Wmf.SetWindowExt);
            WriteUShort((ushort)winWidth);
            WriteUShort((ushort)winHeight);

            /*
            // draw stuff
            DrawEllipse(2, 2, 96, 96, DColor.Blue, 1, 2, DStrokeStyle.Dot);
            FillRect(10, 10, 80, 80, DColor.Red, 1);
            DrawRect(20, 20, 60, 60, DColor.LightGray, 1, 2, DStrokeStyle.Solid, DStrokeJoin.Bevel);
            DrawEllipse(20, 20, 60, 60, DColor.Blue, 1, 2, DStrokeStyle.DashDotDot);
            DrawLine(new DPoint(15, 50), new DPoint(85, 50), DColor.White, 1, DStrokeStyle.Solid, 1, DStrokeCap.Butt);
            DPoints pts = new DPoints();
            for (int i = 0; i < 50; i += 5)
                for (int j = 0; j < 50; j += 5)
                    pts.Add(new DPoint(i, j + i));
            DrawPolyline(pts, DColor.Black);
            pts.Clear();
            pts.Add(new DPoint(50, 50));
            pts.Add(new DPoint(75, 75));
            pts.Add(new DPoint(50, 75));
            FillPolygon(pts, DColor.Blue, 1);
            */
        }

        public void SaveToFile(string fileName)
        {
            byte[] data = WmfData;
            // write data to disk
            FileStream fs = File.OpenWrite(fileName);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public byte[] WmfData
        {
            get
            {
                // write end of file wmf func to memory stream
                WriteFuncHeader(3, Wmf.EOF);
                // get data from memory stream
                byte[] data = ms.ToArray();
                // modify Wmf.Header structure in data
                GCHandle hData = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr p = new IntPtr(hData.AddrOfPinnedObject().ToInt64() + Marshal.SizeOf(typeof(Wmf.Placeable)));
                Wmf.Header header = (Wmf.Header)Marshal.PtrToStructure(p, typeof(Wmf.Header));
                header.Filesize = (uint)(data.Length - Marshal.SizeOf(typeof(Wmf.Placeable)));  // size of file - sizeof(wmf.placeable)
                header.NumOfObjects = numOfObjects;                                             // number of objects created
                header.MaxRecord = maxRecord;                                                   // largest record size
                Marshal.StructureToPtr(header, p, false);
                hData.Free();
                // return data
                return data;
            }
        }

        #region Helper Functions

        byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }

        void WriteFuncHeader(uint size, ushort func)
        {
            Wmf.FuncHeader f;
            f.Size = size;
            f.Function = func;
            byte[] buf = RawSerialize(f);
            ms.Write(buf, 0, buf.Length);
            // need this for Wmf.Header
            if (size > maxRecord)
                maxRecord = size;
        }

        void WriteUShort(ushort num)
        {
            byte[] buf = BitConverter.GetBytes(num);
            ms.Write(buf, 0, buf.Length);
        }

        void WriteColor(DColor col)
        {
            byte[] buf = new byte[] { col.R, col.G, col.B, 0 };
            ms.Write(buf, 0, buf.Length);
        }

        ushort StrokePenStyle(DStrokeStyle ss, DStrokeJoin sj, DStrokeCap sc)
        {
            int _ss = 0;
            switch (ss)
            {
                case DStrokeStyle.Dash:
                    _ss = Wmf.PS_DASH;
                    break;
                case DStrokeStyle.DashDot:
                    _ss = Wmf.PS_DASHDOT;
                    break;
                case DStrokeStyle.DashDotDot:
                    _ss = Wmf.PS_DASHDOTDOT;
                    break;
                case DStrokeStyle.Dot:
                    _ss = Wmf.PS_DOT;
                    break;
                case DStrokeStyle.Solid:
                    _ss = Wmf.PS_SOLID;
                    break;
            }
            int _sj = 0;
            switch (sj)
            {
                case DStrokeJoin.Bevel:
                    _sj = Wmf.PS_JOIN_BEVEL;
                    break;
                case DStrokeJoin.Mitre:
                    _sj = Wmf.PS_JOIN_MITER;
                    break;
                case DStrokeJoin.Round:
                    _sj = Wmf.PS_JOIN_ROUND;
                    break;
            }
            int _sc = 0;
            switch (sc)
            {
                case DStrokeCap.Butt:
                    _sc = Wmf.PS_ENDCAP_FLAT;
                    break;
                case DStrokeCap.Round:
                    _sc = Wmf.PS_ENDCAP_ROUND;
                    break;
                case DStrokeCap.Square:
                    _sc = Wmf.PS_ENDCAP_SQUARE;
                    break;
            }
            return (ushort)(_ss | _sj | _sc);
        }

        void CreatePenIndirect(double sw, DColor col, DStrokeStyle ss, DStrokeJoin sj, DStrokeCap sc)
        {
            WriteFuncHeader(8, Wmf.CreatePenIndirect);
            WriteUShort(StrokePenStyle(ss, sj, sc));
            WriteUShort((ushort)sw);
            WriteUShort((ushort)sw);
            WriteColor(col);
            // need this for Wmf.Header
            numOfObjects++;
        }

        void CreatePenIndirect()
        {
            WriteFuncHeader(8, Wmf.CreatePenIndirect);
            WriteUShort(Wmf.PS_NULL);
            WriteUShort(0);
            WriteUShort(0);
            WriteColor(DColor.Empty);
            // need this for Wmf.Header
            numOfObjects++;
        }

        void CreateBrushIndirect(DColor col)
        {
            WriteFuncHeader(7, Wmf.CreateBrushIndirect);
            WriteUShort(Wmf.BS_SOLID);
            WriteColor(col);
            WriteUShort(0); // BrushHatch not supported
            // need this for Wmf.Header
            numOfObjects++;
        }

        void CreateBrushIndirect()
        {
            WriteFuncHeader(7, Wmf.CreateBrushIndirect);
            WriteUShort(Wmf.BS_NULL);
            WriteColor(DColor.Empty);
            WriteUShort(0);
            // need this for Wmf.Header
            numOfObjects++;
        }

        void SelectObject(ushort num)
        {
            WriteFuncHeader(4, Wmf.SelectObject);
            WriteUShort(num);
        }

        void DeleteObject(ushort num)
        {
            WriteFuncHeader(4, Wmf.DeleteObject);
            WriteUShort(num);
        }

        void RectShape(ushort func, ushort x, ushort y, ushort width, ushort height)
        {
            WriteFuncHeader(7, func);
            WriteUShort((ushort)(y + height));
            WriteUShort((ushort)(x + width));
            WriteUShort(y);
            WriteUShort(x);
        }

        void MoveTo(DPoint pt)
        {
            WriteFuncHeader(5, Wmf.MoveTo);
            WriteUShort((ushort)pt.Y);
            WriteUShort((ushort)pt.X);
        }

        void LineTo(DPoint pt)
        {
            WriteFuncHeader(5, Wmf.LineTo);
            WriteUShort((ushort)pt.Y);
            WriteUShort((ushort)pt.X);
        }

        void Polyline(DPoints pts)
        {
            WriteFuncHeader((uint)(4 + pts.Count * 2), Wmf.Polyline);
            WriteUShort((ushort)pts.Count);
            foreach (DPoint pt in pts)
            {
                WriteUShort((ushort)pt.X);
                WriteUShort((ushort)pt.Y);
            }
        }

        void Polygon(DPoints pts)
        {
            WriteFuncHeader((uint)(4 + pts.Count * 2), Wmf.Polygon);
            WriteUShort((ushort)pts.Count);
            foreach (DPoint pt in pts)
            {
                WriteUShort((ushort)pt.X);
                WriteUShort((ushort)pt.Y);
            }
        }

        #endregion

        #region Drawing Functions

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
            FillRect(x, y, width, height, color, alpha, DFillStyle.Solid);
        }

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha, DFillStyle fillStyle)
        {
            CreateBrushIndirect(color);
            SelectObject(0);
            CreatePenIndirect();
            SelectObject(1);
            RectShape(Wmf.Rectangle, (ushort)x, (ushort)y, (ushort)width, (ushort)height);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin)
        {
            CreatePenIndirect(strokeWidth, color, strokeStyle, DStrokeJoin.Round, DStrokeCap.Round);
            SelectObject(0);
            CreateBrushIndirect();
            SelectObject(1);
            RectShape(Wmf.Rectangle, (ushort)x, (ushort)y, (ushort)width, (ushort)height);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color)
        {
            DrawRect(x, y, width, height, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            DrawRect(x, y, width, height, color, alpha, strokeWidth, DStrokeStyle.Solid, DStrokeJoin.Round);
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
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1, strokeStyle, DStrokeJoin.Round);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
            FillEllipse(x, y, width, height, color, 1);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            CreateBrushIndirect(color);
            SelectObject(0);
            CreatePenIndirect();
            SelectObject(1);
            RectShape(Wmf.Ellipse, (ushort)x, (ushort)y, (ushort)width, (ushort)height);
            DeleteObject(1);
            DeleteObject(0);
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
            CreatePenIndirect(strokeWidth, color, strokeStyle, DStrokeJoin.Round, DStrokeCap.Round);
            SelectObject(0);
            CreateBrushIndirect();
            SelectObject(1);
            RectShape(Wmf.Ellipse, (ushort)x, (ushort)y, (ushort)width, (ushort)height);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
            DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1, DStrokeStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
            DrawLine(pt1, pt2, color, 1, DStrokeStyle.Solid, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha)
        {
            DrawLine(pt1, pt2, color, alpha, DStrokeStyle.Solid, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, 1, strokeStyle, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle)
        {
            DrawLine(pt1, pt2, color, alpha, strokeStyle, 1, DStrokeCap.Round);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DStrokeStyle strokeStyle, double strokeWidth, DStrokeCap strokeCap)
        {
            CreatePenIndirect(strokeWidth, color, strokeStyle, DStrokeJoin.Round, strokeCap);
            SelectObject(0);
            CreateBrushIndirect();
            SelectObject(1);
            MoveTo(pt1);
            LineTo(pt2);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1, DStrokeStyle.Solid, DStrokeJoin.Round, DStrokeCap.Round);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth, DStrokeStyle strokeStyle, DStrokeJoin strokeJoin, DStrokeCap strokeCap)
        {
            CreatePenIndirect(strokeWidth, color, strokeStyle, strokeJoin, strokeCap);
            SelectObject(0);
            CreateBrushIndirect();
            SelectObject(1);
            Polyline(pts);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha)
        {
            FillPolygon(pts, color, alpha, DFillRule.EvenOdd);
        }

        public override void FillPolygon(DPoints pts, DColor color, double alpha, DFillRule fillRule)
        {
            CreateBrushIndirect(color);
            SelectObject(0);
            CreatePenIndirect();
            SelectObject(1);
            Polygon(pts);
            DeleteObject(1);
            DeleteObject(0);
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DPoint pt, double alpha)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect)
        {
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect, double alpha)
        {
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color)
        {
        }

        public override void DrawText(string text, string fontName, double fontSize, DPoint pt, DColor color, double alpha)
        {
        }

        public override void DrawText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough, DPoint pt, DColor color, double alpha)
        {
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
            return new DPoint(100, 100);
        }

        public override DPoint MeasureText(string text, string fontName, double fontSize, bool bold, bool italics, bool underline, bool strikethrough)
        {
            return new DPoint(100, 100);
        }

        public override void StartGroup(double x, double y, double width, double height, double offsetX, double offsetY)
        {
        }

        public override void DrawGroup(double alpha)
        {
        }

        public override DMatrix SaveTransform()
        {
            return new DMatrix();
        }

        public override void LoadTransform(DMatrix matrix)
        {
        }

        public override void Scale(double sx, double sy)
        {
        }

        public override void Rotate(double angle, DPoint center)
        {
        }

        public override void Translate(double tx, double ty)
        {
        }

        public override void ResetTransform()
        {
        }

        public override void Clip(DRect r)
        {
        }

        public override void ResetClip()
        {
        }

        public override DCompositingMode CompositingMode
        {
            get { return DCompositingMode.SourceCopy; }
            set { }
        }

        public override bool AntiAlias
        {
            get { return false; }
            set { }
        }

        public override void Save()
        {
        }

        public override void Restore()
        {
        }

        #endregion

        public override void Dispose()
        {
        }
    }
}
