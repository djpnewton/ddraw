using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;

using DejaVu.Collections.Generic;

namespace DDraw
{
    public static class FigureSerialize
    {
        public const string DDRAW_FIGURE_XML = "DDrawFigureXml";

        public const string FIGURE_ELE = "Figure";
        public const string TYPE_ATTR = "Type";
        public const string DIMENSION_ELE = "Dimension";
        public const string X_ATTR = "X";
        public const string Y_ATTR = "Y";
        public const string WIDTH_ATTR = "Width";
        public const string HEIGHT_ATTR = "Height";
        public const string ROTATION_ATTR = "Rotation";
        public const string LOCKASPECTRATIO_ATTR = "LockAspectRatio";
        public const string FILL_ELE = "Fill";
        public const string COLOR_ATTR = "Color";
        public const string STROKE_ELE = "Stroke";
        public const string STROKEWIDTH_ATTR = "StrokeWidth";
        public const string STROKESTYLE_ATTR = "StrokeStyle";
        public const string STROKECAP_ATTR = "StrokeCap";
        public const string STROKEJOIN_ATTR = "StrokeJoin";
        public const string ALPHA_ELE = "Alpha";
        public const string VALUE_ATTR = "Value";
        public const string IMAGE_ELE = "Image";
        public const string POSITION_ATTR = "Position";
        public const string FILENAME_ATTR = "FileName";
        public const string BASE64_VAL = "base64";
        public const string TEXT_ELE = "Text";
        public const string FONTNAME_ATTR = "FontName";
        public const string FONTSIZE_ATTR = "FontSize";
        public const string BOLD_ATTR = "Bold";
        public const string ITALICS_ATTR = "Italics";
        public const string UNDERLINE_ATTR = "Underline";
        public const string STRIKETHROUGH_ATTR = "Strikethrough";
        public const string CHILDFIGURES_ELE = "ChildFigures";
        public const string LINESEGMENT_ELE = "LineSegment";
        public const string PT1_ATTR = "Pt1";
        public const string PT2_ATTR = "Pt2";
        public const string POLYLINE_ELE = "Polyline";
        public const string MARKER_ELE = "Marker";
        public const string STARTMARKER_ATTR = "StartMarker";
        public const string ENDMARKER_ATTR = "EndMarker";
        public const string EDITABLEATTRIBUTES_ELE = "EditableAttributes";

        static void FormatToXml(Figure f, XmlTextWriter wr, Dictionary<string, byte[]> images)
        {
            wr.WriteStartElement(FIGURE_ELE);
            wr.WriteAttributeString(TYPE_ATTR, f.GetType().FullName);
            if (f is IDimension)
            {
                wr.WriteStartElement(DIMENSION_ELE);
                IDimension fd = (IDimension)f;
                wr.WriteAttributeString(X_ATTR, fd.X.ToString());
                wr.WriteAttributeString(Y_ATTR, fd.Y.ToString());
                wr.WriteAttributeString(WIDTH_ATTR, fd.Width.ToString());
                wr.WriteAttributeString(HEIGHT_ATTR, fd.Height.ToString());
                wr.WriteAttributeString(ROTATION_ATTR, fd.Rotation.ToString());
                wr.WriteAttributeString(LOCKASPECTRATIO_ATTR, fd.LockAspectRatio.ToString());
                wr.WriteEndElement();
            }
            if (f is IFillable)
            {
                wr.WriteStartElement(FILL_ELE);
                wr.WriteAttributeString(COLOR_ATTR, DColor.FormatToString(((IFillable)f).Fill));
                wr.WriteEndElement();
            }
            if (f is IStrokeable)
            {
                wr.WriteStartElement(STROKE_ELE);
                IStrokeable fs = (IStrokeable)f;
                wr.WriteAttributeString(COLOR_ATTR, DColor.FormatToString(fs.Stroke));
                wr.WriteAttributeString(STROKEWIDTH_ATTR, fs.StrokeWidth.ToString());
                wr.WriteAttributeString(STROKESTYLE_ATTR, fs.StrokeStyle.ToString());
                wr.WriteAttributeString(STROKECAP_ATTR, fs.StrokeCap.ToString());
                wr.WriteAttributeString(STROKEJOIN_ATTR, fs.StrokeJoin.ToString());
                wr.WriteEndElement();
            }
            if (f is IAlphaBlendable)
            {
                wr.WriteStartElement(ALPHA_ELE);
                wr.WriteAttributeString(VALUE_ATTR, ((IAlphaBlendable)f).Alpha.ToString());
                wr.WriteEndElement();
            }
            if (f is IImage)
            {
                wr.WriteStartElement(IMAGE_ELE);
                IImage img = (IImage)f;
                wr.WriteAttributeString(POSITION_ATTR, img.Position.ToString());
                if (img.ImageData != null)
                {
                    if (images != null)
                    {
                        if (images.ContainsKey(img.FileName) && !BytesSame(images[img.FileName], img.ImageData))
                        {
                            string fileName = FindUniqueFileName(images, Path.GetFileName(img.FileName));
                            images[fileName] = img.ImageData;
                            wr.WriteAttributeString(FILENAME_ATTR, fileName);
                        }
                        else
                        {
                            images[img.FileName] = img.ImageData;
                            wr.WriteAttributeString(FILENAME_ATTR, Path.GetFileName(img.FileName));
                        }
                    }
                    else
                    {
                        wr.WriteAttributeString(FILENAME_ATTR, Path.GetFileName(img.FileName));
                        wr.WriteAttributeString(TYPE_ATTR, BASE64_VAL);
                        wr.WriteString(Convert.ToBase64String(img.ImageData));
                    }
                }
                wr.WriteEndElement();
            }
            if (f is ITextable)
            {
                wr.WriteStartElement(TEXT_ELE);
                ITextable ft = (ITextable)f;
                wr.WriteAttributeString(FONTNAME_ATTR, ft.FontName);
                wr.WriteAttributeString(FONTSIZE_ATTR, ft.FontSize.ToString());
                wr.WriteAttributeString(BOLD_ATTR, ft.Bold.ToString());
                wr.WriteAttributeString(ITALICS_ATTR, ft.Italics.ToString());
                wr.WriteAttributeString(UNDERLINE_ATTR, ft.Underline.ToString());
                wr.WriteAttributeString(STRIKETHROUGH_ATTR, ft.Strikethrough.ToString());
                wr.WriteString(ft.Text);
                wr.WriteEndElement();
            }
            if (f is IChildFigureable)
            {
                wr.WriteStartElement(CHILDFIGURES_ELE);
                foreach (Figure childFigure in ((IChildFigureable)f).ChildFigures)
                    FormatToXml(childFigure, wr, images);
                wr.WriteEndElement();
            }
            if (f is ILineSegment)
            {
                wr.WriteStartElement(LINESEGMENT_ELE);
                ILineSegment lf = (ILineSegment)f;
                wr.WriteAttributeString(PT1_ATTR, DPoint.FormatToString(lf.Pt1));
                wr.WriteAttributeString(PT2_ATTR, DPoint.FormatToString(lf.Pt2));
                wr.WriteEndElement();
            }
            if (f is IPolyline)
            {
                wr.WriteStartElement(POLYLINE_ELE);
                wr.WriteString(DPoints.FormatToString(((IPolyline)f).Points));
                wr.WriteEndElement();
            }
            if (f is IMarkable)
            {
                wr.WriteStartElement(MARKER_ELE);
                wr.WriteAttributeString(STARTMARKER_ATTR, ((IMarkable)f).StartMarker.ToString());
                wr.WriteAttributeString(ENDMARKER_ATTR, ((IMarkable)f).EndMarker.ToString());
                wr.WriteEndElement();
            }
            if (f is IEditable)
            {
                wr.WriteStartElement(EDITABLEATTRIBUTES_ELE);
                wr.WriteString(((IEditable)f).EditAttrsToString());
                wr.WriteEndElement();
            }
            wr.WriteEndElement();
        }

        private static bool BytesSame(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            else
                for (int i = 0; i < a1.Length; i++)
                    if (a1[i] != a2[i])
                        return false;
            return true;
        }

        private static string FindUniqueFileName(Dictionary<string, byte[]> images, string baseName)
        {
            string result = baseName;
            int i = 0;
            while (images.ContainsKey(result))
            {
                result = string.Concat(Path.GetFileNameWithoutExtension(baseName), i.ToString(), Path.GetExtension(baseName));
                i += 1;
            }
            return result;
        }

        static XmlTextWriter CreateXmlWriter(MemoryStream ms)
        {
            XmlTextWriter wr = new XmlTextWriter(ms, null);
            wr.Formatting = Formatting.Indented;
            wr.Indentation = 2;
            wr.Namespaces = false;
            return wr;
        }

        public static string FormatToXml(Figure f, Dictionary<string, byte[]> images)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = CreateXmlWriter(ms);
            wr.WriteStartDocument();
            FormatToXml(f, wr, images);
            wr.WriteEndDocument();
            wr.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static string FormatToXml(IList<Figure> figures, Dictionary<string, byte[]> images)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = CreateXmlWriter(ms);
            wr.WriteStartDocument();
            wr.WriteStartElement("FigureList");
            foreach (Figure f in figures)
                FormatToXml(f, wr, images);
            wr.WriteEndElement();
            wr.WriteEndDocument();
            wr.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        static DRect GetBoundingRect(Figure f)
        {
            if (f is IStrokeable)
                return DGeom.BoundingBoxOfRotatedRect(((IStrokeable)f).RectInclStroke, f.Rotation);
            else
                return DGeom.BoundingBoxOfRotatedRect(f.Rect, f.Rotation);
        }

        public static DBitmap FormatToBmp(IList<Figure> figures, bool antiAlias)
        {
            double left, top, right, bottom;
            if (figures.Count > 0)
            {
                DRect r = GetBoundingRect(figures[0]);
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
                if (figures.Count > 1)
                    foreach (Figure f in figures)
                    {
                        r = GetBoundingRect(f);
                        if (r.Left < left)
                            left = r.Left;
                        if (r.Top < top)
                            top = r.Top;
                        if (r.Right > right)
                            right = r.Right;
                        if (r.Bottom > bottom)
                            bottom = r.Bottom;
                    }
                DBitmap bmp = GraphicsHelper.MakeBitmap(right - left, bottom - top);
                DGraphics dg = GraphicsHelper.MakeGraphics(bmp);
                dg.AntiAlias = antiAlias;
                dg.FillRect(-1, -1, right - left + 2, bottom - top + 2, DColor.White, 1);
                dg.Translate(-left, -top);
                foreach (Figure f in figures)
                    f.Paint(dg);
                return bmp;
            }
            else
                return null;
        }

        static void ApplyDimensions(XmlReader re, IDimension d)
        {
            double x = 0, y = 0, width = 10, height = 10, rot = 0;
            bool lar = false;
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == X_ATTR)
                    double.TryParse(re.Value, out x);
                else if (re.LocalName == Y_ATTR)
                    double.TryParse(re.Value, out y);
                else if (re.LocalName == WIDTH_ATTR)
                    double.TryParse(re.Value, out width);
                else if (re.LocalName == HEIGHT_ATTR)
                    double.TryParse(re.Value, out height);
                else if (re.LocalName == ROTATION_ATTR)
                    double.TryParse(re.Value, out rot);
                else if (re.LocalName == LOCKASPECTRATIO_ATTR)
                    bool.TryParse(re.Value, out lar);
            }
            d.X = x;
            d.Y = y;
            d.Width = width;
            d.Height = height;
            d.Rotation = rot;
            d.LockAspectRatio = lar;
        }

        static void ApplyFill(XmlReader re, IFillable f)
        {
            re.MoveToContent();
            re.MoveToAttribute(COLOR_ATTR);
            if (re.LocalName == COLOR_ATTR)
                f.Fill = DColor.FromString(re.Value);
        }

        static void ApplyStroke(XmlReader re, IStrokeable s)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                try
                {
                    if (re.LocalName == COLOR_ATTR)
                        s.Stroke = DColor.FromString(re.Value);
                    else if (re.LocalName == STROKEWIDTH_ATTR)
                    {
                        double sw = 1;
                        double.TryParse(re.Value, out sw);
                        s.StrokeWidth = sw;
                    }
                    else if (re.LocalName == STROKESTYLE_ATTR)
                        s.StrokeStyle = (DStrokeStyle)Enum.Parse(typeof(DStrokeStyle), re.Value, true);
                    else if (re.LocalName == STROKECAP_ATTR)
                        s.StrokeCap = (DStrokeCap)Enum.Parse(typeof(DStrokeCap), re.Value, true);
                    else if (re.LocalName == STROKEJOIN_ATTR)
                        s.StrokeJoin = (DStrokeJoin)Enum.Parse(typeof(DStrokeJoin), re.Value, true);
                }
                catch { }
            }
        }

        static void ApplyAlpha(XmlReader re, IAlphaBlendable a)
        {
            re.MoveToContent();
            re.MoveToAttribute(VALUE_ATTR);
            if (re.LocalName == VALUE_ATTR)
            {
                double v = 1;
                double.TryParse(re.Value, out v);
                a.Alpha = v;
            }
        }

        static void ApplyImage(XmlReader re, IImage b)
        {
            re.MoveToContent();
            bool base64Data = false;
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == TYPE_ATTR && re.Value == BASE64_VAL)
                    base64Data = true;
                else if (re.LocalName == POSITION_ATTR)
                    b.Position = (DImagePosition)Enum.Parse(typeof(DImagePosition), re.Value, true);
                else if (re.LocalName == FILENAME_ATTR)
                    b.FileName = re.Value;
            }
            if (base64Data)
            {
                string data = re.ReadString();
                if (data.Length > 0)
                    b.ImageData = Convert.FromBase64String(data);
            }
        }

        static void ApplyText(XmlReader re, ITextable t)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == FONTNAME_ATTR)
                    t.FontName = re.Value;
                else if (re.LocalName == FONTSIZE_ATTR)
                {
                    double sz = 1;
                    double.TryParse(re.Value, out sz);
                    t.FontSize = sz;
                }
                else if (re.LocalName == BOLD_ATTR)
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Bold = b;
                }
                else if (re.LocalName == ITALICS_ATTR)
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Italics = b;
                }
                else if (re.LocalName == UNDERLINE_ATTR)
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Underline = b;
                }
                else if (re.LocalName == STRIKETHROUGH_ATTR)
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Strikethrough = b;
                }
            }
            t.Text = re.ReadString();
        }

        static void ApplyChildren(XmlReader re, IChildFigureable c)
        {
            UndoRedoList<Figure> figs = new UndoRedoList<Figure>();
            while (re.Read())
            {
                if (re.NodeType == XmlNodeType.Element && re.LocalName == FIGURE_ELE)
                {
                    Figure f = FromXml(re.ReadSubtree());
                    if (f != null)
                        figs.Add(f);
                }
            }
            c.ChildFigures = figs;
        }

        static void ApplyLine(XmlReader re, ILineSegment ls)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == PT1_ATTR)
                    ls.Pt1 = DPoint.FromString(re.Value);
                else if (re.LocalName == PT2_ATTR)
                    ls.Pt2 = DPoint.FromString(re.Value);
            }
        }

        static void ApplyPolyline(XmlReader re, IPolyline pl)
        {
            re.MoveToContent();
            pl.Points = DPoints.FromString(re.ReadString());
        }

        static void ApplyMarkers(XmlReader re, IMarkable m)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                try
                {
                    re.MoveToAttribute(i);
                    if (re.LocalName == STARTMARKER_ATTR)
                        m.StartMarker = (DMarker)Enum.Parse(typeof(DMarker), re.Value, true);
                    else if (re.LocalName == ENDMARKER_ATTR)
                        m.EndMarker = (DMarker)Enum.Parse(typeof(DMarker), re.Value, true);
                }
                catch { }
            }
        }

        static void ApplyEditableAttributes(XmlReader re, IEditable e)
        {
            re.MoveToContent();
            e.SetEditAttrsFromString(re.ReadString());
        }

        static Figure FromXml(XmlReader re)
        {
            Figure result = null;
            re.MoveToContent();
            re.MoveToAttribute(TYPE_ATTR);
            while (re.ReadAttributeValue())
            {
                if (re.NodeType == XmlNodeType.Text)
                {
                    Type figureType = Type.GetType(re.Value);
                    if (figureType != null)
                        result = (Figure)Activator.CreateInstance(figureType);
                }
            }
            // apply properties to figure
            if (result != null)
            {
                while (re.Read())
                {
                    if (re.NodeType == XmlNodeType.Element)
                    {
                        if (re.LocalName == DIMENSION_ELE && result is IDimension)
                            ApplyDimensions(re.ReadSubtree(), (IDimension)result);
                        else if (re.LocalName == FILL_ELE && result is IFillable)
                            ApplyFill(re.ReadSubtree(), (IFillable)result);
                        else if (re.LocalName == STROKE_ELE && result is IStrokeable)
                            ApplyStroke(re.ReadSubtree(), (IStrokeable)result);
                        else if (re.LocalName == ALPHA_ELE && result is IAlphaBlendable)
                            ApplyAlpha(re.ReadSubtree(), (IAlphaBlendable)result);
                        else if (re.LocalName == IMAGE_ELE && result is IImage)
                            ApplyImage(re.ReadSubtree(), (IImage)result);
                        else if (re.LocalName == TEXT_ELE && result is ITextable)
                            ApplyText(re.ReadSubtree(), (ITextable)result);
                        else if (re.LocalName == CHILDFIGURES_ELE && result is IChildFigureable)
                            ApplyChildren(re.ReadSubtree(), (IChildFigureable)result);
                        else if (re.LocalName == LINESEGMENT_ELE && result is ILineSegment)
                            ApplyLine(re.ReadSubtree(), (ILineSegment)result);
                        else if (re.LocalName == POLYLINE_ELE && result is IPolyline)
                            ApplyPolyline(re.ReadSubtree(), (IPolyline)result);
                        else if (re.LocalName == MARKER_ELE && result is IMarkable)
                            ApplyMarkers(re.ReadSubtree(), (IMarkable)result);
                        else if (re.LocalName == EDITABLEATTRIBUTES_ELE && result is IEditable)
                            ApplyEditableAttributes(re.ReadSubtree(), (IEditable)result);
                    }
                }
            }
            return result;
        }

        public static List<Figure> FromXml(string xml)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(xml);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            XmlTextReader re = new XmlTextReader(ms);
            List<Figure> result = new List<Figure>();
            Figure currentFig;
            while (re.Read())
            {
                if (re.NodeType == XmlNodeType.Element && re.LocalName == FIGURE_ELE)
                {
                    currentFig = FromXml(re.ReadSubtree());
                    if (currentFig != null)
                        result.Add(currentFig);
                }
            }
            return result;
        }
    }
}
