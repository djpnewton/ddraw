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

        const string FIGURE_ELE = "Figure";
        const string TYPE_ATTR = "Type";
        const string USERATTRS_ELE = "UserAttrs";
        const char   USERATTRS_SEP = ' ';
        const string USERATTRS_SPACE_ENT = "&space;";
        const char   USERATTRS_PART_SEP = ',';
        const string USERATTRS_COMMA_ENT = "&comma;";
        const string USERATTRS_AMP_ENT = "&amp;";
        const string DIMENSION_ELE = "Dimension";
        const string X_ATTR = "X";
        const string Y_ATTR = "Y";
        const string WIDTH_ATTR = "Width";
        const string HEIGHT_ATTR = "Height";
        const string ROTATION_ATTR = "Rotation";
        const string FLIPX_ATTR = "FlipX";
        const string FLIPY_ATTR = "FlipY";
        const string LOCKASPECTRATIO_ATTR = "LockAspectRatio";
        const string LOCKED_ATTR = "Locked";
        const string FILL_ELE = "Fill";
        const string COLOR_ATTR = "Color";
        const string STROKE_ELE = "Stroke";
        const string STROKEWIDTH_ATTR = "StrokeWidth";
        const string STROKESTYLE_ATTR = "StrokeStyle";
        const string STROKECAP_ATTR = "StrokeCap";
        const string STROKEJOIN_ATTR = "StrokeJoin";
        const string ALPHA_ELE = "Alpha";
        const string VALUE_ATTR = "Value";
        const string IMAGE_ELE = "Image";
        const string FILENAME_ATTR = "FileName";
        const string BASE64_VAL = "base64";
        const string BITMAP_ELE = "Bitmap";
        const string BITMAPPOSITION_ATTR = "BitmapPosition";
        const string METAFILE_ELE = "Metafile";
        const string METAFILETYPE_ATTR = "MetafileType";
        const string TEXT_ELE = "Text";
        const string FONTNAME_ATTR = "FontName";
        const string FONTSIZE_ATTR = "FontSize";
        const string BOLD_ATTR = "Bold";
        const string ITALICS_ATTR = "Italics";
        const string UNDERLINE_ATTR = "Underline";
        const string STRIKETHROUGH_ATTR = "Strikethrough";
        const string WRAPTEXT_ATTR = "WrapText";
        const string WRAPTHRESHOLD_ATTR = "WrapThreshold";
        const string WRAPFONTSIZE_ATTR = "WrapFontSize";
        const string CHILDFIGURES_ELE = "ChildFigures";
        const string POLYLINE_ELE = "Polyline";
        const string MARKER_ELE = "Marker";
        const string STARTMARKER_ATTR = "StartMarker";
        const string ENDMARKER_ATTR = "EndMarker";
        const string EDITABLEATTRIBUTES_ELE = "EditableAttributes";
        const string POLYGON_ELE = "Polygon";

        static void FormatToXml(Figure f, XmlTextWriter wr, Dictionary<string, byte[]> images)
        {
            wr.WriteStartElement(FIGURE_ELE);
            wr.WriteAttributeString(TYPE_ATTR, f.GetType().FullName);
            // UserAttrs
            wr.WriteStartElement(USERATTRS_ELE);
            string ua = "";
            foreach (string k in f.UserAttrs.Keys)
            {
                string key = k.Replace("&", USERATTRS_AMP_ENT).
                    Replace(",", USERATTRS_COMMA_ENT).Replace(" ", USERATTRS_SPACE_ENT);
                string value = f.UserAttrs[k].Replace("&", USERATTRS_AMP_ENT).
                    Replace(",", USERATTRS_COMMA_ENT).Replace(" ", USERATTRS_SPACE_ENT);
                ua += string.Format("{0}{1}{2}{3}", key, USERATTRS_PART_SEP, value, USERATTRS_SEP);
            }
            wr.WriteValue(ua);
            wr.WriteEndElement();
            // the rest
            if (f is IDimension)
            {
                wr.WriteStartElement(DIMENSION_ELE);
                IDimension fd = (IDimension)f;
                wr.WriteAttributeString(X_ATTR, fd.X.ToString());
                wr.WriteAttributeString(Y_ATTR, fd.Y.ToString());
                wr.WriteAttributeString(WIDTH_ATTR, fd.Width.ToString());
                wr.WriteAttributeString(HEIGHT_ATTR, fd.Height.ToString());
                wr.WriteAttributeString(ROTATION_ATTR, fd.Rotation.ToString());
                wr.WriteAttributeString(FLIPX_ATTR, fd.FlipX.ToString());
                wr.WriteAttributeString(FLIPY_ATTR, fd.FlipY.ToString());
                wr.WriteAttributeString(LOCKASPECTRATIO_ATTR, fd.LockAspectRatio.ToString());
                wr.WriteAttributeString(LOCKED_ATTR, fd.Locked.ToString());
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
                if (img.ImageData != null)
                {
                    if (images != null && img.FileName != null)
                    {
                        string fileName = FindUniqueFileName(images, Path.GetFileName(img.FileName), img.ImageData);
                        images[fileName] = img.ImageData;
                        wr.WriteAttributeString(FILENAME_ATTR, fileName);
                    }
                    else
                    {
                        if (img.FileName != null)
                            wr.WriteAttributeString(FILENAME_ATTR, Path.GetFileName(img.FileName));
                        wr.WriteAttributeString(TYPE_ATTR, BASE64_VAL);
                        wr.WriteString(Convert.ToBase64String(img.ImageData));
                    }
                }
                wr.WriteEndElement();
            }
            if (f is IBitmap)
            {
                wr.WriteStartElement(BITMAP_ELE);
                wr.WriteAttributeString(BITMAPPOSITION_ATTR, ((IBitmap)f).BitmapPosition.ToString());
                wr.WriteEndElement();
            }
            if (f is IMetafile)
            {
                wr.WriteStartElement(METAFILE_ELE);
                wr.WriteAttributeString(METAFILETYPE_ATTR, ((IMetafile)f).MetafileType.ToString());
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
                wr.WriteAttributeString(WRAPTEXT_ATTR, ft.WrapText.ToString());
                wr.WriteAttributeString(WRAPTHRESHOLD_ATTR, ft.WrapThreshold.ToString());
                wr.WriteAttributeString(WRAPFONTSIZE_ATTR, ft.WrapFontSize.ToString());
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
            if (f is PolygonFigure)
            {
                wr.WriteStartElement(POLYGON_ELE);
                wr.WriteString(DPoints.FormatToString(((PolygonFigure)f).Points));
                wr.WriteEndElement();
            }
            wr.WriteEndElement();
        }

        public static bool BytesSame(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            else
                for (int i = 0; i < a1.Length; i++)
                    if (a1[i] != a2[i])
                        return false;
            return true;
        }

        private static string FindUniqueFileName(Dictionary<string, byte[]> images, string baseName, byte[] imageData)
        {
            // Find a unique filename for the image but if image data is stored in the dictionary
            // with the same image data then use that filename
            string result = baseName;
            int i = 0;
            while (images.ContainsKey(result) && !BytesSame(images[result], imageData))
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

        private static DRect GetBounds(IList<Figure> figures)
        {
            DRect r = GetBoundingRect(figures[0]);
            if (figures.Count > 1)
                foreach (Figure f in figures)
                {
                    DRect r2 = GetBoundingRect(f);
                    if (r2.Left < r.Left)
                        r.Left = r2.Left;
                    if (r2.Top < r.Top)
                        r.Top = r2.Top;
                    if (r2.Right > r.Right)
                        r.Right = r2.Right;
                    if (r2.Bottom > r.Bottom)
                        r.Bottom = r2.Bottom;
                }
            return r;
        }

        public static DBitmap FormatToBmp(IList<Figure> figures, bool antiAlias, DColor backgroundColor)
        {
            if (figures.Count > 0)
            {
                DRect r = GetBounds(figures);
                DBitmap bmp = GraphicsHelper.MakeBitmap(r.Width, r.Height);
                DGraphics dg = GraphicsHelper.MakeGraphics(bmp);
                dg.AntiAlias = antiAlias;
                dg.FillRect(-1, -1, r.Width + 2, r.Height + 2, backgroundColor, 1);
                dg.Translate(-r.Left, -r.Top);
                foreach (Figure f in figures)
                    f.Paint(dg);
                return bmp;
            }
            else
                return null;
        }

        public static byte[] FormatToEmf(IList<Figure> figures, DPoint screenMM, DPoint deviceRes)
        {
            if (figures.Count > 0)
            {
                DRect r =  GetBounds(figures);
                EmfGraphics dg = new EmfGraphics(r, screenMM, deviceRes);
                foreach (Figure f in figures)
                    f.Paint(dg);
                return dg.EmfData;
            }
            return null;
        }

        static void ApplyUserAttrs(XmlReader re, Figure f)
        {
            re.MoveToContent();
            string ua = re.ReadString().Trim();
            string[] attrs = ua.Split(USERATTRS_SEP);
            foreach (string attr in attrs)
            {
                string[] attrParts = attr.Split(USERATTRS_PART_SEP);
                if (attrParts.Length == 2)
                {
                    string key = attrParts[0].Replace(USERATTRS_COMMA_ENT, ",").
                        Replace(USERATTRS_SPACE_ENT, " ").Replace(USERATTRS_AMP_ENT, "&");
                    string value = attrParts[1].Replace(USERATTRS_COMMA_ENT, ",").
                        Replace(USERATTRS_SPACE_ENT, " ").Replace(USERATTRS_AMP_ENT, "&");
                    f.UserAttrs[key] = value;
                }
            }
        }

        static void ApplyDimensions(XmlReader re, IDimension d)
        {
            double x = 0, y = 0, width = 10, height = 10, rot = 0;
            bool flipX = false, flipY = false, lar = false, locked = false;
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
                else if (re.LocalName == FLIPX_ATTR)
                    bool.TryParse(re.Value, out flipX);
                else if (re.LocalName == FLIPY_ATTR)
                    bool.TryParse(re.Value, out flipY);
                else if (re.LocalName == LOCKASPECTRATIO_ATTR)
                    bool.TryParse(re.Value, out lar);
                else if (re.LocalName == LOCKED_ATTR)
                    bool.TryParse(re.Value, out locked);
            }
            d.X = x;
            d.Y = y;
            d.Width = width;
            d.Height = height;
            d.Rotation = rot;
            d.FlipX = flipX;
            d.FlipY = flipY;
            d.LockAspectRatio = lar;
            d.Locked = locked;
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

        static void ApplyBitmap(XmlReader re, IBitmap b)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == BITMAPPOSITION_ATTR)
                    b.BitmapPosition = (DBitmapPosition)Enum.Parse(typeof(DBitmapPosition), re.Value, true);
            }
        }

        static void ApplyMetafile(XmlReader re, IMetafile b)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == METAFILETYPE_ATTR)
                    b.MetafileType = (DMetafileType)Enum.Parse(typeof(DMetafileType), re.Value, true);
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
                else if (re.LocalName == WRAPTEXT_ATTR)
                {
                    bool b = t.WrapText;
                    bool.TryParse(re.Value, out b);
                    t.WrapText = b;
                }
                else if (re.LocalName == WRAPTHRESHOLD_ATTR)
                {
                    double wt = t.WrapThreshold;
                    double.TryParse(re.Value, out wt);
                    t.WrapThreshold = wt;
                }
                else if (re.LocalName == WRAPFONTSIZE_ATTR)
                {
                    double wfs = t.WrapFontSize;
                    double.TryParse(re.Value, out wfs);
                    t.WrapFontSize = wfs;
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

        static void ApplyPolygon(XmlReader re, PolygonFigure f)
        {
            re.MoveToContent();
            f.Points = DPoints.FromString(re.ReadString());
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
                        if (re.LocalName == USERATTRS_ELE)
                            ApplyUserAttrs(re.ReadSubtree(), result);
                        else if (re.LocalName == DIMENSION_ELE && result is IDimension)
                            ApplyDimensions(re.ReadSubtree(), (IDimension)result);
                        else if (re.LocalName == FILL_ELE && result is IFillable)
                            ApplyFill(re.ReadSubtree(), (IFillable)result);
                        else if (re.LocalName == STROKE_ELE && result is IStrokeable)
                            ApplyStroke(re.ReadSubtree(), (IStrokeable)result);
                        else if (re.LocalName == ALPHA_ELE && result is IAlphaBlendable)
                            ApplyAlpha(re.ReadSubtree(), (IAlphaBlendable)result);
                        else if (re.LocalName == IMAGE_ELE && result is IImage)
                            ApplyImage(re.ReadSubtree(), (IImage)result);
                        else if (re.LocalName == BITMAP_ELE && result is IBitmap)
                            ApplyBitmap(re.ReadSubtree(), (IBitmap)result);
                        else if (re.LocalName == METAFILE_ELE && result is IMetafile)
                            ApplyMetafile(re.ReadSubtree(), (IMetafile)result);
                        else if (re.LocalName == TEXT_ELE && result is ITextable)
                            ApplyText(re.ReadSubtree(), (ITextable)result);
                        else if (re.LocalName == CHILDFIGURES_ELE && result is IChildFigureable)
                            ApplyChildren(re.ReadSubtree(), (IChildFigureable)result);
                        else if (re.LocalName == POLYLINE_ELE && result is IPolyline)
                            ApplyPolyline(re.ReadSubtree(), (IPolyline)result);
                        else if (re.LocalName == MARKER_ELE && result is IMarkable)
                            ApplyMarkers(re.ReadSubtree(), (IMarkable)result);
                        else if (re.LocalName == EDITABLEATTRIBUTES_ELE && result is IEditable)
                            ApplyEditableAttributes(re.ReadSubtree(), (IEditable)result);
                        else if (re.LocalName == POLYGON_ELE && result is PolygonFigure)
                            ApplyPolygon(re.ReadSubtree(), (PolygonFigure)result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update any old versions of xml that may have been released
        /// </summary>
        /// <param name="xml">The xml string to be parsed</param>
        /// <returns>An updated xml string</returns>
        static string FormatOld(string xml)
        {            
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);
            // update old BackgroundFigure type
            XmlNodeList nodes = xmldoc.SelectNodes("/Figure[@Type='DDraw.BackgroundFigure']");
            foreach (XmlNode node in nodes)
                UpdateOldIImage(xmldoc, node, null);
            // update old ImageFigure type to BitmapFigure
            nodes = xmldoc.SelectNodes("//Figure[@Type='DDraw.ImageFigure']");
            foreach (XmlNode node in nodes)
                UpdateOldIImage(xmldoc, node, typeof(BitmapFigure).ToString());
            // update old LineFigure type to MultiLineFigure
            nodes = xmldoc.SelectNodes("//Figure[@Type='DDraw.LineFigure']");
            foreach (XmlNode node in nodes)
                UpdateOldLineFigure(xmldoc, node, typeof(LineFigure2).ToString());
            return xmldoc.OuterXml;
        }

        private static void UpdateOldIImage(XmlDocument xmldoc, XmlNode node, string newType)
        {
            if (newType != null)
                node.Attributes["Type"].Value = newType;
            XmlNode bitmapNode = node.SelectSingleNode(BITMAP_ELE);
            if (bitmapNode == null) // no need if bitmapNode already exists
            {
                XmlNode imgNode = node.SelectSingleNode(IMAGE_ELE);
                if (imgNode != null)
                {
                    bitmapNode = xmldoc.CreateElement(BITMAP_ELE);
                    XmlAttribute positionAttr = imgNode.Attributes["Position"];
                    if (positionAttr != null)
                    {
                        XmlAttribute bitmapPositonAttr = xmldoc.CreateAttribute(BITMAPPOSITION_ATTR);
                        bitmapPositonAttr.Value = positionAttr.Value;
                        bitmapNode.Attributes.Append(bitmapPositonAttr);
                    }
                    node.AppendChild(bitmapNode);
                }
            }
        }

        private static void UpdateOldLineFigure(XmlDocument xmldoc, XmlNode node, string newType)
        {
            if (newType != null)
                node.Attributes["Type"].Value = newType;
            XmlNode lineSegmentNode = node.SelectSingleNode("LineSegment");
            if (lineSegmentNode != null)
            {
                XmlAttribute pt1 = lineSegmentNode.Attributes["Pt1"];
                XmlAttribute pt2 = lineSegmentNode.Attributes["Pt2"];
                if (pt1 != null && pt2 != null)
                {
                    XmlElement pointsNode = xmldoc.CreateElement(POLYLINE_ELE);
                    DPoints pts = new DPoints();
                    pts.Add(DPoint.FromString(pt1.Value));
                    pts.Add(DPoint.FromString(pt2.Value));
                    XmlText txt = xmldoc.CreateTextNode(DPoints.FormatToString(pts));
                    pointsNode.AppendChild(txt);
                    lineSegmentNode.AppendChild(pointsNode);
                }
            }
        }

        public static List<Figure> FromXml(string xml)
        {
            // make sure xml string is updated
            xml = FormatOld(xml);

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
