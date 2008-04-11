using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DDraw;

namespace WinFormsDemo.PersonalToolbar
{
    public partial class PtForm : Form
    {
        PersonalToolStrip personalToolStrip = null;
        public PersonalToolStrip PersonalToolStrip
        {
            set 
            { 
                personalToolStrip = value;
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
            for (int i = 1; i < personalToolStrip.Items.Count; i++)
            {
                if (personalToolStrip.Items[i] is CustomFigureToolButton)
                    listBox1.Items.Add(((CustomFigureToolButton)personalToolStrip.Items[i]).CustomFigureT);
                else if (personalToolStrip.Items[i] is RunCmdToolButton)
                    listBox1.Items.Add(((RunCmdToolButton)personalToolStrip.Items[i]).RunCmdT);
                else if (personalToolStrip.Items[i] is ShowDirToolButton)
                    listBox1.Items.Add(((ShowDirToolButton)personalToolStrip.Items[i]).ShowDirT);
                    
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PtButtonForm pf = new PtButtonForm();
            pf.ToolButtonData = new CustomFigureT(typeof(PolylineFigure), DAuthorProperties.GlobalAP.Clone(), null);
            if (pf.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Add(pf.ToolButtonData);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                PtButtonForm pf = new PtButtonForm();
                pf.ToolButtonData = listBox1.SelectedItem;
                if (pf.ShowDialog() == DialogResult.OK)
                    listBox1.Items[listBox1.SelectedIndex] = pf.ToolButtonData;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                int idx = listBox1.SelectedIndex;
                listBox1.Items.Remove(listBox1.SelectedItem);
                if (idx < listBox1.Items.Count)
                    listBox1.SelectedIndex = idx;
                else
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;

            }
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