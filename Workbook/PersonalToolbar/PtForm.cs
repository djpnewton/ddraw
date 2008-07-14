using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DDraw;

namespace Workbook.PersonalToolbar
{
    public partial class PtForm : Form
    {
        PersonalToolStrip personalToolStrip = null;
        public PersonalToolStrip PersonalToolStrip
        {
            set 
            { 
                personalToolStrip = value;
                UpdateListView();
            }
        }

        public IEnumerable<PersonalTool> ToolItems
        {
            get
            {
                foreach (ListViewItem o in listView1.Items)
                    yield return (PersonalTool)o.Tag;
            }
        }

        const string RunCmdImage = "RunCmdImage";
        const string ShowDirImage = "ShowDirImage";
        const string WebLinkImage = "WebLink";
        const string SelectModeImage = "SelectModeImage";
        const string EraserModeImage = "EraserModeImage";          

        public PtForm()
        {
            InitializeComponent();

            imageList1.Images.Add(RunCmdImage, Resource1.cog);
            imageList1.Images.Add(ShowDirImage, Resource1.folder);
            imageList1.Images.Add(WebLinkImage, Resource1.world_link);
            imageList1.Images.Add(SelectModeImage, Resource1.cursor);
            imageList1.Images.Add(EraserModeImage, Resource1.eraser);
        }

        ListViewItem CreateListViewItem(PersonalTool item)
        {
            ListViewItem li = new ListViewItem();
            if (item is PersonalTool)
            {
                li.Tag = item;
                li.Text = li.Tag.ToString();
                li.ToolTipText = li.Tag.ToString();
            }
            return li;
        }

        void SetImage(ListViewItem li)
        {
            if (li.Tag is CustomFigureTool)
            {
                imageList1.Images.Add(WorkBookUtils.Base64ToBitmap(((CustomFigureTool)li.Tag).Base64Icon));
                li.ImageIndex = imageList1.Images.Count - 1;
            }
            else if (li.Tag is RunCmdTool)
                li.ImageKey = RunCmdImage;
            else if (li.Tag is ShowDirTool)
                li.ImageKey = ShowDirImage;
            else if (li.Tag is WebLinkTool)
                li.ImageKey = WebLinkImage;
            else if (li.Tag is ModeSelectTool)
            {
                if (((ModeSelectTool)li.Tag).ModeSelectType == ModeSelectType.Select)
                    li.ImageKey = SelectModeImage;
                else
                    li.ImageKey = EraserModeImage;
            }
        }

        void AddToListView(PersonalTool item, int idx)
        {
            ListViewItem li = CreateListViewItem(item);
            SetImage(li);
            listView1.Items.Insert(idx, li);
        }

        void AddToListView(PersonalTool item)
        {
            AddToListView(item, listView1.Items.Count);
        }

        void UpdateListView()
        {
            listView1.Items.Clear();
            for (int i = 1; i < personalToolStrip.Items.Count; i++)
            {
                if (personalToolStrip.Items[i] is CustomFigureToolButton)
                    AddToListView(((CustomFigureToolButton)personalToolStrip.Items[i]).CustomFigure);
                else if (personalToolStrip.Items[i] is RunCmdToolButton)
                    AddToListView(((RunCmdToolButton)personalToolStrip.Items[i]).RunCmd);
                else if (personalToolStrip.Items[i] is ShowDirToolButton)
                    AddToListView(((ShowDirToolButton)personalToolStrip.Items[i]).ShowDir);
                else if (personalToolStrip.Items[i] is WebLinkToolButton)
                    AddToListView(((WebLinkToolButton)personalToolStrip.Items[i]).WebLink);
                else if (personalToolStrip.Items[i] is ModeSelectToolButton)
                    AddToListView(((ModeSelectToolButton)personalToolStrip.Items[i]).ModeSelect);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PtButtonForm pf = new PtButtonForm();
            pf.PersonalTool = new CustomFigureTool(null, false, typeof(PolylineFigure), new DAuthorProperties(), null);
            if (pf.ShowDialog() == DialogResult.OK)
            {
                AddToListView(pf.PersonalTool);
                listView1.SelectedIndices.Clear();
                listView1.SelectedIndices.Add(listView1.Items.Count - 1);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                PtButtonForm pf = new PtButtonForm();
                pf.PersonalTool = (PersonalTool)listView1.SelectedItems[0].Tag;
                if (pf.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem li = CreateListViewItem(pf.PersonalTool);
                    SetImage(li);
                    listView1.Items[listView1.SelectedIndices[0]] = li;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int idx = listView1.SelectedIndices[0];
                listView1.Items.Remove(listView1.SelectedItems[0]);
                listView1.SelectedIndices.Clear();
                if (idx < listView1.Items.Count)
                    listView1.SelectedIndices.Add(idx);
                else if (listView1.Items.Count > 0)
                    listView1.SelectedIndices.Add(listView1.Items.Count - 1);
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && listView1.SelectedIndices[0] > 0)
            {
                int idx = listView1.SelectedIndices[0];
                ListViewItem item = listView1.SelectedItems[0];
                listView1.Items.Remove(listView1.SelectedItems[0]);
                listView1.Items.Insert(idx - 1, item);
                listView1.SelectedItems.Clear();
                listView1.SelectedIndices.Add(idx - 1);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && listView1.SelectedIndices[0] < listView1.Items.Count - 1)
            {
                int idx = listView1.SelectedIndices[0];
                ListViewItem item = listView1.SelectedItems[0];
                listView1.Items.Remove(listView1.SelectedItems[0]);
                listView1.Items.Insert(idx + 1, item);
                listView1.SelectedIndices.Clear();
                listView1.SelectedIndices.Add(idx + 1);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = listView1.SelectedItems.Count == 1;
            btnDelete.Enabled = listView1.SelectedItems.Count == 1;
            btnMoveUp.Enabled = listView1.SelectedItems.Count == 1;
            btnMoveDown.Enabled = listView1.SelectedItems.Count == 1;
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            btnEdit_Click(sender, e);
        }
    }
}