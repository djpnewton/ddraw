using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo.PersonalToolbar
{
    public partial class PtButtonForm : Form
    {
        public object ToolbuttonData
        {
            set
            {
                if (value is RunCmdT)
                {
                    tbRun.Text = ((RunCmdT)value).Command;
                    tbArgs.Text = ((RunCmdT)value).Arguments;
                    cbType.SelectedIndex = (int)PersonalToolbuttonType.RunCmd;
                }
                else
                {
                    tbDir.Text = ((ShowDirT)value).Dir;
                    cbType.SelectedIndex = (int)PersonalToolbuttonType.ShowDir;
                }
            }
            get 
            {
                if (cbType.SelectedIndex == (int)PersonalToolbuttonType.RunCmd)
                    return new RunCmdT(tbRun.Text, tbArgs.Text);
                else
                    return new ShowDirT(tbDir.Text);
            }
        }

        public PtButtonForm()
        {
            InitializeComponent();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbType.SelectedIndex == (int)PersonalToolbuttonType.RunCmd)
                pnlRunCmd.BringToFront();
            else
                pnlShowDir.BringToFront();
        }

        private void btnDirBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                tbDir.Text = fd.SelectedPath;
        }

        private void btnRunBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "All Files|*.*";
            if (fd.ShowDialog() == DialogResult.OK)
                tbRun.Text = fd.FileName;
        }
    }
}