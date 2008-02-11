using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DDraw;
using DejaVu;
using ICSharpCode.SharpZipLib.Zip;
using Nini.Config;

namespace WinFormsDemo
{
    public static class FileHelper
    {
        const string PAGES_INI = "pages.ini";
        const string PAGESIZE = "PageSize";
        const string FIGURELIST = "figureList";
        const string BACKGROUNDFIGURE = "backgroundFigure";
        const string IMAGES_DIR = "images";

        static void Write(ZipOutputStream zipOut, string entryName, byte[] data)
        {
            ZipEntry entry = new ZipEntry(entryName);
            entry.DateTime = DateTime.Now;
            entry.Size = data.Length;
            zipOut.PutNextEntry(entry);
            zipOut.Write(data, 0, data.Length);
        }

        public static void Save(string fileName, List<DEngine> engines)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            FileStream fs = File.Create(fileName);
            ZipOutputStream zipOut = new ZipOutputStream(fs);
            IniConfigSource source = new IniConfigSource();
            // write each page
            int i = 0;
            Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();
            foreach (DEngine de in engines)
            {
                IConfig config = source.AddConfig(string.Format("page{0}", i));
                config.Set(PAGESIZE, DPoint.FormatToString(de.PageSize));
                string figureListName = string.Format("figureList{0}.xml", i);
                byte[] data = encoding.GetBytes(FigureSerialize.FormatToXml(de.Figures, images));
                config.Set(FIGURELIST, figureListName);
                Write(zipOut, figureListName, data);
                string backgroundFigureName = string.Format("backgroundFigure{0}.xml", i);
                config.Set(BACKGROUNDFIGURE, backgroundFigureName);
                data = encoding.GetBytes(FigureSerialize.FormatToXml(de.GetBackgroundFigure(), images));
                Write(zipOut, backgroundFigureName, data);
                i += 1;
            }
            // write images
            foreach (KeyValuePair<string, byte[]> kvp in images)
                if (kvp.Key != null && kvp.Key.Length > 0)
                    Write(zipOut, IMAGES_DIR + Path.DirectorySeparatorChar + Path.GetFileName(kvp.Key), kvp.Value);
            // write pages ini
            Write(zipOut, PAGES_INI, encoding.GetBytes(source.ToString()));
            // finish
            zipOut.Close();
            zipOut.Dispose();
            fs.Close();
            fs.Dispose();
        }

        static byte[] Read(ZipFile zf, string entryName)
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

        public static List<DEngine> Load(string fileName, DAuthorProperties dap, bool usingEngineManager)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            List<DEngine> res = new List<DEngine>();
            // load zipfile
            ZipFile zf = new ZipFile(fileName);
            // find pages ini file entry
            byte[] data = Read(zf, PAGES_INI);
            if (data != null)
            {
                // create Nini config source from pages ini entry stream
                IniConfigSource source = new IniConfigSource(new MemoryStream(data));
                // load each page info mentioned in ini entry
                foreach (IConfig config in source.Configs)
                {
                    // create new DEngine for page
                    DEngine de = new DEngine(dap, usingEngineManager);
                    if (!usingEngineManager)
                        // start recording undo history (it is a pain that we have to do this)
                        de.UndoRedoStart("loading file");
                    // set page size
                    if (config.Contains(PAGESIZE))
                        de.PageSize = DPoint.FromString(config.Get(PAGESIZE));
                    // set the figures
                    if (config.Contains(FIGURELIST))
                    {
                        string figureListEntryName = config.Get(FIGURELIST);
                        data = Read(zf, figureListEntryName);
                        if (data != null)
                        {
                            List<Figure> figs = FigureSerialize.FromXml(encoding.GetString(data));
                            foreach (Figure f in figs)
                            {
                                de.AddFigure(f);
                                LoadImage(zf, f);
                            }
                        }
                    }
                    // set the background figure
                    if (config.Contains(BACKGROUNDFIGURE))
                    {
                        string backgroundFigureEntryName = config.Get(BACKGROUNDFIGURE);
                        data = Read(zf, backgroundFigureEntryName);
                        if (data != null)
                        {
                            List<Figure> figs = FigureSerialize.FromXml(encoding.GetString(data));
                            if (figs.Count == 1 && figs[0] is BackgroundFigure)
                            {
                                LoadImage(zf, figs[0]);
                                de.SetBackgroundFigure((BackgroundFigure)figs[0]);
                            }
                        }
                    }
                    if (!usingEngineManager)
                    {
                        // get rid of undo history
                        de.UndoRedoCommit();
                        de.UndoRedoClearHistory();
                    }
                    // add to list of DEngines
                    res.Add(de);
                }
            }
            return res;
        }

        private static void LoadImage(ZipFile zf, Figure f)
        {
            if (f is IImage && ((IImage)f).FileName != null)
                ((IImage)f).ImageData = Read(zf, string.Concat(IMAGES_DIR, Path.DirectorySeparatorChar, ((IImage)f).FileName));
            if (f is IChildFigureable)
                foreach (Figure child in ((IChildFigureable)f).ChildFigures)
                    LoadImage(zf, child);
        }
    }
}
