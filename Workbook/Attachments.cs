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

namespace Workbook
{
    public delegate void FileDropHandler(object sender, string[] filePaths);

    public class AttachmentView : ListView
    {
        public const string ATTACHMENTS_DIR = "attachments";

        UndoRedoDictionary<string, byte[]> attachmentDict = new UndoRedoDictionary<string, byte[]>();

        public event FileDropHandler FileDrop;
        public event EventHandler Insert;
        public event EventHandler Delete;

        public string TempDir = null;

        MenuItem deleteMenuItem;

        public AttachmentView()
        {
            // columns
            ColumnHeader FileName = new ColumnHeader();
            FileName.Text = WbLocale.FileName;
            FileName.Width = 100;
            ColumnHeader Size = new ColumnHeader();
            Size.Text = WbLocale.Size;
            Size.Width = 40;
            Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { FileName, Size});
            // details view
            View = View.Details;
            // drag n drop
            AllowDrop = true;
            DragEnter += new DragEventHandler(AttachmentView_DragEnter);
            DragDrop += new DragEventHandler(AttachmentView_DragDrop);
            ItemDrag += new ItemDragEventHandler(AttachmentView_ItemDrag);
            // item activate
            Activation = ItemActivation.Standard;
            ItemActivate += new EventHandler(AttachmentView_ItemActivate);
            // context menu
            MenuItem ins = new MenuItem(WbLocale.Insert);
            ins.Click += new EventHandler(insert_Click);
            deleteMenuItem = new MenuItem(WbLocale.Delete);
            deleteMenuItem.Click += new EventHandler(delete_Click);
            ContextMenu = new ContextMenu(new MenuItem[] { ins, deleteMenuItem });
            ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
        }

        void AttachmentView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void AttachmentView_DragDrop(object sender, DragEventArgs e)
        {
            if (FileDrop != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                FileDrop(this, ((string[])e.Data.GetData(DataFormats.FileDrop)));
        }

        void AttachmentView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Copy);
        }

        void AttachmentView_ItemActivate(object sender, EventArgs e)
        {
            if (SelectedItems.Count == 1)
                try
                { ExecuteAttachment(SelectedItems[0].Text); }
                catch (Exception ex)
                { MessageBox.Show(ex.Message, WbLocale.AttachmentExecuteError); }
        }

        void insert_Click(object sender, EventArgs e)
        {
            if (Insert != null)
                Insert(this, new EventArgs());
        }

        void delete_Click(object sender, EventArgs e)
        {
            if (Delete != null)
                Delete(this, new EventArgs());
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            deleteMenuItem.Enabled = SelectedItems.Count > 0;
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

        // from http://www.binaryartist.com/post/C-File-size-in-human-terms2c-GB2c-MB2c-KB2c-Bytes.aspx
        string GetFileSize(long bytes)
        {
            if (bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(bytes, 1073741824);
                return String.Format("{0:##.##} GB", size);
            }
            else if (bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(bytes, 1048576);
                return String.Format("{0:##.##} MB", size);
            }
            else if (bytes >= 1024)
            {
                Decimal size = Decimal.Divide(bytes, 1024);
                return String.Format("{0:##.##} KB", size);
            }
            else if (bytes > 0 & bytes < 1024)
            {
                Decimal size = bytes;
                return String.Format("{0:##.##} Bytes", size);
            }
            else
                return "0 Bytes";
        } 

        public void AddAttachment(string name, byte[] buf)
        {
            name = Path.GetFileName(name);
            attachmentDict[name] = buf;
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add(GetFileSize(buf.Length));
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
                string tempDir = TempDir;
                if (tempDir == null || tempDir.Length == 0)
                    tempDir = Path.GetTempPath();
                string fileName = WorkBookUtils.GetTempFileName(name, tempDir);
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
