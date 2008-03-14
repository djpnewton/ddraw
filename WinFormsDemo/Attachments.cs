using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DejaVu.Collections.Generic;
using DDraw;

namespace WinFormsDemo
{
    public class AttachmentView : ListView
    {
        public const string ATTACHMENTS_DIR = "attachments";

        UndoRedoDictionary<string, byte[]> attachmentDict = new UndoRedoDictionary<string, byte[]>();

        DEngineManager dem = null;
        public DEngineManager EngineManager
        {
            set { dem = value; }
        }

        public AttachmentView()
        {
            // columns
            ColumnHeader FileName = new ColumnHeader();
            FileName.Text = "File Name";
            FileName.Width = 100;
            ColumnHeader Size = new ColumnHeader();
            Size.Text = "Size";
            Size.Width = 40;
            Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { FileName, Size});
            // details view
            View = View.Details;
            // drag n drop
            AllowDrop = true;
            DragEnter += new DragEventHandler(AttachmentView_DragEnter);
            DragDrop += new DragEventHandler(AttachmentView_DragDrop);
            ItemDrag += new ItemDragEventHandler(AttachmentView_ItemDrag);
        }

        void AttachmentView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void AttachmentView_DragDrop(object sender, DragEventArgs e)
        {
            if (dem != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dem.UndoRedoStart("Add Attachments");
                string[] paths = ((string[])e.Data.GetData(DataFormats.FileDrop));
                foreach (string path in paths)
                    if (CheckAttachmentExists(path))
                        AddAttachment(path);
                dem.UndoRedoCommit();
            }
        }

        void AttachmentView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Copy);
        }

        byte[] ReadFileData(string path)
        {
            byte[] buf;
            using (FileStream fs = File.OpenRead(path))
            {
                buf = new byte[fs.Length];
                fs.Read(buf, 0, (int)fs.Length);
            }
            return buf;
        }

        public bool CheckAttachmentExists(string path)
        {
            string name = Path.GetFileName(path);
            if (HasAttachment(name))
            {
                // read file data
                byte[] buf = ReadFileData(path);
                // check if same
                if (!FigureSerialize.BytesSame(buf, attachmentDict[name]))
                {
                    if (MessageBox.Show(string.Format("An attachment already exists with the name \"{0}\"\n\nDo you want to overwrite it?", name),
                        "Attachment Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return false;
                }
                return true;
            }
            return true;
        }

        public string AddAttachment(string fileName)
        {
            if (File.Exists(fileName))
            {
                // read file data
                byte[] buf = ReadFileData(fileName);
                // add attachment
                string name = Path.GetFileName(fileName);
                AddAttachment(name, buf);
                // return attachment name
                return name;
            }
            return null;
        }

        public void AddAttachment(string name, byte[] buf)
        {
            attachmentDict[name] = buf;
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add(buf.Length.ToString());
            Items.Add(item);
        }

        public void RemoveAttachment(ListViewItem item)
        {
            if (attachmentDict.ContainsKey(item.Text))
            {
                attachmentDict.Remove(item.Text);
                Items.Remove(item);
            }
        }

        public void ClearAttachments()
        {
            attachmentDict.Clear();
            Items.Clear();
        }

        public bool HasAttachment(string name)
        {
            return attachmentDict.ContainsKey(name);
        }

        public byte[] GetAttachment(string name)
        {
            return attachmentDict[name];
        }

        public bool ExecuteAttachment(string name)
        {
            if (attachmentDict.ContainsKey(name))
            {
                // find unique temp file name
                string tempDir = Path.GetTempPath();
                string ext = Path.GetExtension(name);
                string namePart = Path.GetFileNameWithoutExtension(name);
                int n = 0;
                string fileName;
                do
                {
                    fileName = string.Format("{0}{1}[{2}]{3}", tempDir, namePart, n, ext);
                    n++;
                }
                while (File.Exists(fileName));
                // copy attachment to temp file
                using (FileStream fs = File.Create(fileName))
                {
                    byte[] buf = attachmentDict[name];
                    fs.Write(buf, 0, buf.Length);
                }
                // execute file
                System.Diagnostics.Process.Start(fileName);
                return true;
            }
            return false;
        }

        public List<string> GetAttachmentNames()
        {
            List<string> result = new List<string>();
            foreach (string name in attachmentDict.Keys)
                result.Add(name);
            return result;
        }

        public void UpdateAttachmentView()
        {
            if (attachmentDict.Count != Items.Count)
            {
                Items.Clear();
                foreach (string key in attachmentDict.Keys)
                {
                    ListViewItem item = new ListViewItem(key);
                    item.SubItems.Add(attachmentDict[key].Length.ToString());
                    Items.Add(item);
                }
            }
        }
    }
}
