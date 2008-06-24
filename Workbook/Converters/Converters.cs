using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using DDraw;

namespace Workbook.Converters
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
            List<string> pageEntries = nbk.GetPageEntries(manifest);
            foreach (string entry in pageEntries)
            {
                DEngine de = new DEngine(true);
                XmlDocument page = nbk.GetPage(entry);
                if (page != null)
                {
                    nbk.AddPageToEngine(page, de);
                    result.Add(de);
                }
            }
            // read attachments
            attachments = new Dictionary<string, byte[]>();
            List<string> attachmentEntries = nbk.GetAttachmentEntries(manifest);
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
