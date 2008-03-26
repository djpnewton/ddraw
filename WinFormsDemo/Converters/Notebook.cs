using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using DDraw;
using ICSharpCode.SharpZipLib.Zip;
using SvgNet;
using SvgNet.SvgElements;
using SvgNet.SvgTypes;

namespace WinFormsDemo.Converters
{
    public class Notebook
    {
        ZipFile zf;

        public Notebook(string fileName)
        {
            zf = new ZipFile(fileName);
        }

        ~Notebook()
        {
            zf.Close();
        }

        byte[] Read(string entryName)
        {
            // search for entry name with forwardslash or backslash path seperators
            entryName = entryName.Replace("/", @"\");
            int entryIdx = zf.FindEntry(entryName, true);
            if (entryIdx == -1)
            {
                entryName = entryName.Replace(@"\", "/");
                entryIdx = zf.FindEntry(entryName, true);
            }
            // read bytes if entry found
            if (entryIdx != -1)
            {
                ZipEntry entry = zf[entryIdx];
                Stream s = zf.GetInputStream(entry);
                return new BinaryReader(s).ReadBytes((int)entry.Size);
            }
            else
                return null;
        }

        public XmlDocument GetXmlEntry(string entryName)
        {
            byte[] data = Read(entryName);
            if (data != null)
            {
                XmlDocument xmldoc = new XmlDocument();
                using (MemoryStream ms = new MemoryStream(data))
                    xmldoc.Load(ms);
                return xmldoc;
            }
            else
                return null;
        }

        public XmlDocument GetManifest()
        {
            return GetXmlEntry("imsmanifest.xml");
        }

        public List<string> GetPageEntries(XmlDocument manifest)
        {
            List<string> pageEntries = new List<string>();
            // add default namespace the the manifest uses
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifest.NameTable);
            nsmgr.AddNamespace("ims", "http://www.imsglobal.org/xsd/imscp_v1p1");
            // find page nodes using the ims manifest
            XmlNode doc = manifest.DocumentElement;
            XmlNodeList pages = doc.SelectNodes("/ims:manifest/ims:resources/ims:resource[@identifier='pages']/*", nsmgr);
            // add the page entries to the result
            foreach (XmlNode n in pages)
                if (n.LocalName == "file" && n.Attributes.GetNamedItem("href") != null)
                    pageEntries.Add(n.Attributes.GetNamedItem("href").Value);
            return pageEntries;
        }

        public XmlDocument GetPage(string entryName)
        {
            return GetXmlEntry(entryName);
        }

        const string svgNs = "http://www.w3.org/2000/svg";

        bool HasSvgNamespace(XmlDocument page)
        {
            return page.DocumentElement.GetAttribute("xmlns") == svgNs;
        }

        public void AddPageToEngine(XmlDocument page, DEngine de)
        {
            // svg namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(page.NameTable);
            nsmgr.AddNamespace("svg", svgNs);
            bool hasSvgNs = HasSvgNamespace(page);
            // set de page size
            int width;
            int height;
            int.TryParse(page.DocumentElement.Attributes.GetNamedItem("width").Value, out width);
            int.TryParse(page.DocumentElement.Attributes.GetNamedItem("height").Value, out height);
            if (width == 0 || height == 0)
                de.PageSize = new DPoint(800, 600);
            else
                de.PageSize = new DPoint(width, height);
            // node list
            XmlNodeList nl;
            // find background
            if (hasSvgNs)
                nl = page.SelectNodes("/svg:svg/svg:rect", nsmgr);
            else
                nl = page.SelectNodes("/svg/rect");
            if (nl.Count == 1)
            {
                SvgElement rect = SvgFactory.LoadFromXML(page, (XmlElement)nl[0]);
                Figure f = SvgElementToFigure(rect);
                if (f is RectFigure)
                {
                    BackgroundFigure bf = new BackgroundFigure();
                    bf.Fill = ((RectFigure)f).Fill;
                    de.SetBackgroundFigure(bf);
                }
            }
            // try to find foreground group
            if (hasSvgNs)
                nl = page.SelectNodes("/svg:svg/svg:g[@class='foreground']", nsmgr);
            else
                nl = page.SelectNodes("/svg/g[@class='foreground']");
            // convert svg elements to figures and add them to DEngine
            if (nl.Count == 1)
            {
                SvgElement root = SvgFactory.LoadFromXML(page, (XmlElement)nl[0]);
                foreach (SvgElement e in root.Children)
                {
                    Figure f = SvgElementToFigure(e);
                    if (f != null)
                        de.AddFigure(f);
                }
            }
        }

        double GetSvgElementRotation(SvgStyledTransformedElement e)
        {
            if (e.Transform.Count == 1 && e.Transform[0].Type == SvgTransformType.SVG_TRANSFORM_ROTATE)
                return e.Transform[0].Angle * Math.PI / 180;
            return 0;
        }

        Figure SvgElementToFigure(SvgElement e)
        {
            Figure f = null;

            if (e is SvgRectElement || e is SvgImageElement)
            {
                if (e.Attributes.ContainsKey("x") && e.Attributes.ContainsKey("y") &&
                    e.Attributes.ContainsKey("width") && e.Attributes.ContainsKey("height"))
                {
                    SvgLength X = new SvgLength((string)e.Attributes["x"]);
                    SvgLength Y = new SvgLength((string)e.Attributes["y"]);
                    SvgLength Width = new SvgLength((string)e.Attributes["width"]);
                    SvgLength Height = new SvgLength((string)e.Attributes["height"]);
                    DRect r = new DRect(X.Value, Y.Value, Width.Value, Height.Value);
                    if (e is SvgRectElement)
                        f = new RectFigure(r, 0);
                    else if (e is SvgImageElement)
                    {
                        SvgImageElement e2 = (SvgImageElement)e;
                        byte[] imgData = Read(e2.Href);
                        if (imgData != null)
                            f = new ImageFigure(r, 0, imgData, Path.GetFileName(e2.Href));
                    }
                }
            }
            else if (e is SvgEllipseElement)
            {
                SvgEllipseElement e2 = (SvgEllipseElement)e;
                if (e2.Attributes.ContainsKey("cx") && e2.Attributes.ContainsKey("cy") &&
                     e2.Attributes.ContainsKey("rx") && e2.Attributes.ContainsKey("ry"))
                {
                    e2.CX = new SvgLength((string)e2.Attributes["cx"]);
                    e2.CY = new SvgLength((string)e2.Attributes["cy"]);
                    e2.RX = new SvgLength((string)e2.Attributes["rx"]);
                    e2.RY = new SvgLength((string)e2.Attributes["ry"]);
                    f = new EllipseFigure(new DRect(e2.CX.Value - e2.RX.Value, e2.CY.Value - e2.RY.Value,
                        e2.RX.Value * 2, e2.RY.Value * 2), 0);
                }
            }
            else if (e is SvgPathElement)
            {
                SvgPathElement e2 = (SvgPathElement)e;
                if (e2.Attributes.ContainsKey("d"))
                {
                    e2.D = new SvgPath((string)e2.Attributes["d"]);
                    // treat all paths as polygons for the moment
                    DPoints pts = new DPoints();
                    for (int i = 0; i < e2.D.Count; i++)
                    {
                        PathSeg s = e2.D[i];
                        if ((s.Type == SvgPathSegType.SVG_SEGTYPE_MOVETO || s.Type == SvgPathSegType.SVG_SEGTYPE_LINETO) &&
                            s.Abs)
                            pts.Add(new DPoint(s.Data[0], s.Data[1]));
                    }
                    if (pts.Count >= 3)
                    {
                        DRect r = pts.Bounds();
                        foreach (DPoint pt in pts)
                        {
                            pt.X = (pt.X - r.Left) / r.Width;
                            pt.Y = (pt.Y - r.Top) / r.Height;
                        }
                        f = new PolygonFigure(pts);
                        f.Rect = r;
                    }
                }
            }
            else if (e is SvgPolylineElement)
            {
                SvgPolylineElement e2 = (SvgPolylineElement)e;
                if (e2.Attributes.ContainsKey("points"))
                    f = new PolylineFigure(DPoints.FromString((string)e2.Attributes["points"]));
            }
            else if (e is SvgLineElement)
            {
                SvgLineElement e2 = (SvgLineElement)e;
                if (e2.Attributes.ContainsKey("x1") && e2.Attributes.ContainsKey("y1") &&
                     e2.Attributes.ContainsKey("x2") && e2.Attributes.ContainsKey("y2"))
                {
                    e2.X1 = new SvgLength((string)e2.Attributes["x1"]);
                    e2.Y1 = new SvgLength((string)e2.Attributes["y1"]);
                    e2.X2 = new SvgLength((string)e2.Attributes["x2"]);
                    e2.Y2 = new SvgLength((string)e2.Attributes["y2"]);

                    f = new LineFigure(new DPoint(e2.X1.Value, e2.Y1.Value),
                        new DPoint(e2.X2.Value, e2.Y2.Value));
                }
            }
            else if (e is SvgGroupElement)
            {
                SvgGroupElement e2 = (SvgGroupElement)e;
                f = new GroupFigure();
                GroupFigure gf = (GroupFigure)f;
                foreach (SvgElement childEle in e2.Children)
                {
                    Figure childFig = SvgElementToFigure(childEle);
                    if (childFig != null)
                        gf.ChildFigures.Add(childFig);
                }
                if (gf.ChildFigures.Count > 0)
                    gf.ChildFigures = gf.ChildFigures;
                else
                    f = null;
            }

            if (f != null)
            {
                if (e is SvgStyledTransformedElement)
                    f.Rotation = GetSvgElementRotation((SvgStyledTransformedElement)e);
                if (f is IFillable && e.Attributes.ContainsKey("fill"))
                    ((IFillable)f).Fill = DColor.FromHtml((string)e.Attributes["fill"]);
                if (f is IStrokeable)
                {
                    if (e.Attributes.ContainsKey("stroke"))
                        ((IStrokeable)f).Stroke = DColor.FromHtml((string)e.Attributes["stroke"]);
                    if (e.Attributes.ContainsKey("stroke-width"))
                        ((IStrokeable)f).StrokeWidth = double.Parse((string)e.Attributes["stroke-width"]);
                    if (e.Attributes.ContainsKey("stroke-dasharray"))
                        ((IStrokeable)f).StrokeStyle = NotebookDashArrayToStrokeStyle((string)e.Attributes["stroke-dasharray"]);
                }
                if (f is IMarkable)
                {
                    if (e.Attributes.ContainsKey("marker-start"))
                        ((IMarkable)f).StartMarker = NotebookMarkerToDMarker((string)e.Attributes["marker-start"]);
                    if (e.Attributes.ContainsKey("marker-end"))
                        ((IMarkable)f).EndMarker = NotebookMarkerToDMarker((string)e.Attributes["marker-end"]);
                }
                if (f is IAlphaBlendable && e.Attributes.ContainsKey("opacity"))
                    ((IAlphaBlendable)f).Alpha = double.Parse((string)e.Attributes["opacity"]);
            }

            return f;
        }

        DStrokeStyle NotebookDashArrayToStrokeStyle(string s)
        {
            string[] parts = s.Split(',');
            double[] dashes = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                double d;
                double.TryParse(part, out d);
                dashes[i] = d;
            }
            if (dashes.Length == 2 && dashes[0] > dashes[1])
                return DStrokeStyle.Dash;
            if (dashes.Length == 2 && dashes[0] == dashes[1])
                return DStrokeStyle.Dot;
            if (dashes.Length == 4 && dashes[0] > dashes[1] && dashes[1] == dashes[2] && dashes[2] == dashes[3])
                return DStrokeStyle.DashDot;
            if (dashes.Length == 6 && dashes[0] > dashes[1] && dashes[1] == dashes[2] && dashes[2] == dashes[3] &&
                dashes[3] == dashes[4] && dashes[4] == dashes[5])
                return DStrokeStyle.DashDotDot;
            return DStrokeStyle.Solid;
        }

        DMarker NotebookMarkerToDMarker(string s)
        {
            if (s.StartsWith("url(#Arrow"))
                return DMarker.Arrow;
            if (s.StartsWith("url(#Square"))
                return DMarker.Square;
            if (s.StartsWith("url(#Circle"))
                return DMarker.Dot;
            if (s.StartsWith("url(#Diamond"))
                return DMarker.Diamond;
            return DMarker.None;
        }
    }
}
