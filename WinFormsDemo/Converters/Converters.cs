using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using DDraw;

namespace WinFormsDemo.Converters
{
    public static class Converters
    {
        public static List<DEngine> FromNotebook(string fileName, out Dictionary<string, byte[]> attachments)
        {
            List<DEngine> result = new List<DEngine>();
            // create notebook converter
            Notebook nbk = new Notebook(fileName);
            XmlDocument manifest = nbk.GetManifest();
            // create engine for each page
            List<string> pageEntries = nbk.GetResourceEntries(manifest, "pages");
            foreach (string entry in pageEntries)
            {
                DEngine de = new DEngine(DAuthorProperties.GlobalAP, true);
                nbk.AddPageToEngine(nbk.GetPage(entry), de);
                result.Add(de);
            }
            // read attachments
            attachments = new Dictionary<string, byte[]>();
            List<string> attachmentEntries = nbk.GetResourceEntries(manifest, "attachments");
            foreach (string entry in attachmentEntries)
            {
                byte[] attachmentData = nbk.GetAttachment(entry);
                if (attachmentData != null)
                    attachments.Add(entry, attachmentData);
            }
            // free notebook
            nbk = null;
            GC.Collect();
            // return engine/pages
            return result;
        }
    }
}
