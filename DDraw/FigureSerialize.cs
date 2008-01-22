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

        static void FormatToXml(Figure f, XmlTextWriter wr)
        {
            wr.WriteStartElement("Figure");
            wr.WriteAttributeString("Type", f.GetType().FullName);
            if (f is IDimension)
            {
                wr.WriteStartElement("Dimension");
                IDimension fd = (IDimension)f;
                wr.WriteAttributeString("X", fd.X.ToString());
                wr.WriteAttributeString("Y", fd.Y.ToString());
                wr.WriteAttributeString("Width", fd.Width.ToString());
                wr.WriteAttributeString("Height", fd.Height.ToString());
                wr.WriteAttributeString("Rotation", fd.Rotation.ToString());
                wr.WriteEndElement();
            }
            if (f is IFillable)
            {
                wr.WriteStartElement("Fill");
                wr.WriteAttributeString("Color", DColor.FormatToString(((IFillable)f).Fill));
                wr.WriteEndElement();
            }
            if (f is IStrokeable)
            {
                wr.WriteStartElement("Stroke");
                IStrokeable fs = (IStrokeable)f;
                wr.WriteAttributeString("Color", DColor.FormatToString(fs.Stroke));
                wr.WriteAttributeString("StrokeWidth", fs.StrokeWidth.ToString());
                wr.WriteAttributeString("StrokeStyle", fs.StrokeStyle.ToString());
                wr.WriteAttributeString("StrokeCap", fs.StrokeCap.ToString());
                wr.WriteAttributeString("StrokeJoin", fs.StrokeJoin.ToString());
                wr.WriteEndElement();
            }
            if (f is IAlphaBlendable)
            {
                wr.WriteStartElement("Alpha");
                wr.WriteAttributeString("Value", ((IAlphaBlendable)f).Alpha.ToString());
                wr.WriteEndElement();
            }
            if (f is IBitmapable)
            {
                wr.WriteStartElement("Bitmap");
                wr.WriteAttributeString("Type", "base64");
                wr.WriteString(Convert.ToBase64String(((IBitmapable)f).Bitmap.GetData()));
                wr.WriteEndElement();
            }
            if (f is ITextable)
            {
                wr.WriteStartElement("Text");
                ITextable ft = (ITextable)f;
                wr.WriteAttributeString("FontName", ft.FontName);
                wr.WriteAttributeString("FontSize", ft.FontSize.ToString());
                wr.WriteAttributeString("Bold", ft.Bold.ToString());
                wr.WriteAttributeString("Italics", ft.Italics.ToString());
                wr.WriteAttributeString("Underline", ft.Underline.ToString());
                wr.WriteAttributeString("Strikethrough", ft.Strikethrough.ToString());
                wr.WriteString(ft.Text);
                wr.WriteEndElement();
            }
            if (f is IChildFigureable)
            {
                wr.WriteStartElement("ChildFigures");
                foreach (Figure childFigure in ((IChildFigureable)f).ChildFigures)
                    FormatToXml(childFigure, wr);
                wr.WriteEndElement();
            }
            if (f is ILineSegment)
            {
                wr.WriteStartElement("LineSegment");
                ILineSegment lf = (ILineSegment)f;
                wr.WriteAttributeString("Pt1", DPoint.FormatToString(lf.Pt1));
                wr.WriteAttributeString("Pt2", DPoint.FormatToString(lf.Pt2));
                wr.WriteEndElement();
            }
            if (f is IPolyline)
            {
                wr.WriteStartElement("Polyline");
                wr.WriteString(DPoints.FormatToString(((IPolyline)f).Points));
                wr.WriteEndElement();
            }
            if (f is IMarkable)
            {
                wr.WriteStartElement("Marker");
                wr.WriteAttributeString("StartMarker", ((IMarkable)f).StartMarker.ToString());
                wr.WriteAttributeString("EndMarker", ((IMarkable)f).EndMarker.ToString());
                wr.WriteEndElement();
            }
            if (f is IEditable)
            {
                wr.WriteStartElement("EditableAttributes");
                wr.WriteString(((IEditable)f).EditAttrsToString());
                wr.WriteEndElement();
            }
            wr.WriteEndElement();
        }

        public static string FormatToXml(IList<Figure> figures)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = new XmlTextWriter(ms, null);
            wr.Formatting = Formatting.Indented;
            wr.Indentation = 2;
            wr.Namespaces = false;
            wr.WriteStartDocument();
            wr.WriteStartElement("FigureList");
            foreach (Figure f in figures)
                FormatToXml(f, wr);
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
                dg.Translate(new DPoint(-left, -top));
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
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == "X")
                    double.TryParse(re.Value, out x);
                else if (re.LocalName == "Y")
                    double.TryParse(re.Value, out y);
                else if (re.LocalName == "Width")
                    double.TryParse(re.Value, out width);
                else if (re.LocalName == "Height")
                    double.TryParse(re.Value, out height);
                else if (re.LocalName == "Rotation")
                    double.TryParse(re.Value, out rot);
            }
            d.X = x;
            d.Y = y;
            d.Width = width;
            d.Height = height;
            d.Rotation = rot;
        }

        static void ApplyFill(XmlReader re, IFillable f)
        {
            re.MoveToContent();
            re.MoveToAttribute("Color");
            if (re.LocalName == "Color")
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
                    if (re.LocalName == "Color")
                        s.Stroke = DColor.FromString(re.Value);
                    else if (re.LocalName == "StrokeWidth")
                    {
                        double sw = 1;
                        double.TryParse(re.Value, out sw);
                        s.StrokeWidth = sw;
                    }
                    else if (re.LocalName == "StrokeStyle")
                        s.StrokeStyle = (DStrokeStyle)Enum.Parse(typeof(DStrokeStyle), re.Value, true);
                    else if (re.LocalName == "StrokeCap")
                        s.StrokeCap = (DStrokeCap)Enum.Parse(typeof(DStrokeCap), re.Value, true);
                    else if (re.LocalName == "StrokeJoin")
                        s.StrokeJoin = (DStrokeJoin)Enum.Parse(typeof(DStrokeJoin), re.Value, true);
                }
                catch { }
            }
        }

        static void ApplyAlpha(XmlReader re, IAlphaBlendable a)
        {
            re.MoveToContent();
            re.MoveToAttribute("Value");
            if (re.LocalName == "Value")
            {
                double v = 1;
                double.TryParse(re.Value, out v);
                a.Alpha = v;
            }
        }

        static void ApplyBitmap(XmlReader re, IBitmapable b)
        {
            re.MoveToContent();
            re.MoveToAttribute("Type");
            if (re.LocalName == "Type" && re.Value == "base64")
            {
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(re.ReadString()));
                b.Bitmap = GraphicsHelper.MakeBitmap(ms);
            }
        }

        static void ApplyText(XmlReader re, ITextable t)
        {
            re.MoveToContent();
            for (int i = 0; i < re.AttributeCount; i++)
            {
                re.MoveToAttribute(i);
                if (re.LocalName == "FontName")
                    t.FontName = re.Value;
                else if (re.LocalName == "FontSize")
                {
                    double sz = 1;
                    double.TryParse(re.Value, out sz);
                    t.FontSize = sz;
                }
                else if (re.LocalName == "Bold")
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Bold = b;
                }
                else if (re.LocalName == "Italics")
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Italics = b;
                }
                else if (re.LocalName == "Underline")
                {
                    bool b = false;
                    bool.TryParse(re.Value, out b);
                    t.Underline = b;
                }
                else if (re.LocalName == "Strikethrough")
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
                if (re.NodeType == XmlNodeType.Element && re.LocalName == "Figure")
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
                if (re.LocalName == "Pt1")
                    ls.Pt1 = DPoint.FromString(re.Value);
                else if (re.LocalName == "Pt2")
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
                    if (re.LocalName == "StartMarker")
                        m.StartMarker = (DMarker)Enum.Parse(typeof(DMarker), re.Value, true);
                    else if (re.LocalName == "EndMarker")
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
            re.MoveToAttribute("Type");
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
                        if (re.LocalName == "Dimension" && result is IDimension)
                            ApplyDimensions(re.ReadSubtree(), (IDimension)result);
                        else if (re.LocalName == "Fill" && result is IFillable)
                            ApplyFill(re.ReadSubtree(), (IFillable)result);
                        else if (re.LocalName == "Stroke" && result is IStrokeable)
                            ApplyStroke(re.ReadSubtree(), (IStrokeable)result);
                        else if (re.LocalName == "Alpha" && result is IAlphaBlendable)
                            ApplyAlpha(re.ReadSubtree(), (IAlphaBlendable)result);
                        else if (re.LocalName == "Bitmap" && result is IBitmapable)
                            ApplyBitmap(re.ReadSubtree(), (IBitmapable)result);
                        else if (re.LocalName == "Text" && result is ITextable)
                            ApplyText(re.ReadSubtree(), (ITextable)result);
                        else if (re.LocalName == "ChildFigures" && result is IChildFigureable)
                            ApplyChildren(re.ReadSubtree(), (IChildFigureable)result);
                        else if (re.LocalName == "LineSegment" && result is ILineSegment)
                            ApplyLine(re.ReadSubtree(), (ILineSegment)result);
                        else if (re.LocalName == "Polyline" && result is IPolyline)
                            ApplyPolyline(re.ReadSubtree(), (IPolyline)result);
                        else if (re.LocalName == "Marker" && result is IMarkable)
                            ApplyMarkers(re.ReadSubtree(), (IMarkable)result);
                        else if (re.LocalName == "EditableAttributes" && result is IEditable)
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
                if (re.NodeType == XmlNodeType.Element && re.LocalName == "Figure")
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
