using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DDraw;
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

        static void Write(ZipOutputStream zipOut, string entryName, byte[] data)
        {
            ZipEntry entry = new ZipEntry(entryName);
            entry.DateTime = DateTime.Now;
            entry.Size = data.Length;
            zipOut.PutNextEntry(entry);
            zipOut.Write(data, 0, data.Length);
        }

        public static void Save(string fileName, List<DEngine> dengines)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            ZipOutputStream zipOut = new ZipOutputStream(File.Create(fileName));
            IniConfigSource source = new IniConfigSource();
            // write each page
            int i = 0;
            foreach (DEngine de in dengines)
            {
                IConfig config = source.AddConfig(string.Format("page{0}", i));
                config.Set(PAGESIZE, DPoint.FormatToString(de.PageSize));
                string figureListName = string.Format("figureList{0}.xml", i);
                byte[] data = encoding.GetBytes(FigureSerialize.FormatToXml(de.Figures));
                config.Set(FIGURELIST, figureListName);
                Write(zipOut, figureListName, data);
                string backgroundFigureName = string.Format("backgroundFigure{0}.xml", i);
                config.Set(BACKGROUNDFIGURE, backgroundFigureName);
                data = encoding.GetBytes(FigureSerialize.FormatToXml(de.GetBackgroundFigure()));
                Write(zipOut, backgroundFigureName, data);
                i += 1;
            }
            // write pages ini
            Write(zipOut, PAGES_INI, encoding.GetBytes(source.ToString()));
            // finish
            zipOut.Finish();
            zipOut.Close();
        }

        static byte[] Read(ZipFile zf, string entryName)
        {
            int entryIdx = zf.FindEntry(entryName, true);
            if (entryIdx != -1)
            {
                ZipEntry entry = zf[entryIdx];
                Stream s = zf.GetInputStream(entry);
                return new BinaryReader(s).ReadBytes((int)entry.Size);
            }
            else
                return null;
        }

        public static List<DEngine> Load(string fileName, DAuthorProperties dap)
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
                    DEngine de = new DEngine(dap);
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
                                de.AddFigure(f);
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
                            if (figs.Count == 1 && figs[0] is RectbaseFigure)
                                de.SetBackgroundFigure((RectbaseFigure)figs[0]);
                        }
                    }
                    // get rid of undo history
                    de.UndoRedoCommit();
                    de.UndoRedoClearHistory();
                    // add to list of DEngines
                    res.Add(de);
                }
            }
            return res;
        }
    }
}