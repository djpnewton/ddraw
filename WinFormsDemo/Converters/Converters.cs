using System;
using System.Collections.Generic;
using System.Text;

using DDraw;

namespace WinFormsDemo.Converters
{
    public static class Converters
    {
        public static List<DEngine> FromNotebook(string fileName)
        {
            List<DEngine> result = new List<DEngine>();
            // create notebook converter
            Notebook nbk = new Notebook(fileName);
            // create engine for each page
            List<string> pageEntries = nbk.GetPageEntries(nbk.GetManifest());
            foreach (string entry in pageEntries)
            {
                DEngine de = new DEngine(DAuthorProperties.GlobalAP, true);
                nbk.AddPageToEngine(nbk.GetPage(entry), de);
                result.Add(de);
            }
            // return engine/pages
            return result;
        }
    }
}
