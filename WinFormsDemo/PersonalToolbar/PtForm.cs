using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WinFormsDemo.PersonalToolbar
{
    public partial class PtForm : Form
    {
        ToolStripEx personalToolstrip = null;
        public ToolStripEx PersonalToolstrip
        {
            set 
            { 
                personalToolstrip = value;
                UpdateListbox();
            }
        }

        public IEnumerable<object> ToolItems
        {
            get
            {
                foreach (object o in listBox1.Items)
                    yield return o;
            }
        }

        public PtForm()
        {
            InitializeComponent();
        }

        void UpdateListbox()
        {
            listBox1.Items.Clear();
            for (int i = 1; i < personalToolstrip.Items.Count; i++)
            {
                if (personalToolstrip.Items[i] is RunCmdToolButton)
                    listBox1.Items.Add(((RunCmdToolButton)personalToolstrip.Items[i]).RunCmdT);
                else if (personalToolstrip.Items[i] is ShowDirToolButton)
                    listBox1.Items.Add(((ShowDirToolButton)personalToolstrip.Items[i]).ShowDirT);
                    
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PtButtonForm pf = new PtButtonForm();
            pf.ToolbuttonData = new RunCmdT();
            if (pf.ShowDialog() == DialogResult.OK)
                listBox1.Items.Add(pf.ToolbuttonData);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                PtButtonForm pf = new PtButtonForm();
                pf.ToolbuttonData = listBox1.SelectedItem;
                if (pf.ShowDialog() == DialogResult.OK)
                    listBox1.Items[listBox1.SelectedIndex] = pf.ToolbuttonData;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
            {
                int idx = listBox1.SelectedIndex;
                object item = listBox1.SelectedItem;
                listBox1.Items.Remove(listBox1.SelectedItem);
                listBox1.Items.Insert(idx - 1, item);
                listBox1.SelectedItem = item;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox1.SelectedIndex < listBox1.Items.Count - 1)
            {
                int idx = listBox1.SelectedIndex;
                object item = listBox1.SelectedItem;
                listBox1.Items.Remove(listBox1.SelectedItem);
                listBox1.Items.Insert(idx + 1, item);
                listBox1.SelectedItem = item;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = listBox1.SelectedIndex != -1;
            btnDelete.Enabled = listBox1.SelectedIndex != -1;
            btnMoveUp.Enabled = listBox1.SelectedIndex != -1;
            btnMoveDown.Enabled = listBox1.SelectedIndex != -1;
        }
    }
}