using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo
{
    public partial class TextForm : Form
    {
        public string TextEntered
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        public TextForm()
        {
            InitializeComponent();
        }
    }
}