using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace WinFormsDemo
{
    public enum BackgroundType { None, Color, Image };

    public partial class BackgroundForm : Form
    {
        RectbaseFigure backgroundFigure;
        public RectbaseFigure BackgroundFigure
        {
            get { return backgroundFigure; }
            set 
            { 
                backgroundFigure = value;
                UpdateControls();
            }
        }

        BackgroundType BackgroundType
        {
            get
            {
                if (backgroundFigure is RectFigure)
                    return BackgroundType.Color;
                else if (backgroundFigure is ImageFigure)
                    return BackgroundType.Image;
                else
                    return BackgroundType.None;
            }
        }

        public BackgroundForm()
        {
            InitializeComponent();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Point pt = PointToScreen(new Point(btnColor.Left, btnColor.Bottom));
            ColorPicker f = new ColorPicker(pt.X, pt.Y);
            f.ColorSelected += delegate(object sender2, EventArgs ea)
            {
                panel1.BackColor = ((ColorPicker)sender2).SelectedColor;
            };
            f.Show();
        }

        private void btnImageBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void BackgroundForm_Shown(object sender, EventArgs e)
        {
            pictureBox1.Image = pictureBox1.ErrorImage;
            if (BackgroundType == BackgroundType.Color)
            {
                panel1.BackColor = WFHelper.MakeColor(((RectFigure)backgroundFigure).Fill);
                rbColor.Checked = true;
            }
            else if (BackgroundType == BackgroundType.Image)
            {
                if (((ImageFigure)backgroundFigure).Bitmap != null)
                    pictureBox1.Image = (Bitmap)((ImageFigure)backgroundFigure).Bitmap.NativeBmp;
                rbImage.Checked = true;
            }
            UpdateControls();
        }

        void UpdateControls()
        {
            btnColor.Enabled = rbColor.Checked;
            panel1.Enabled = rbColor.Checked;
            btnImageBrowse.Enabled = rbImage.Checked;
            pictureBox1.Enabled = rbImage.Checked;
        }

        private void rbColor_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void rbImage_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void BackgroundForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (rbColor.Checked)
                {
                    if (!(backgroundFigure is RectFigure) ||
                        WFHelper.MakeColor(((RectFigure)backgroundFigure).Fill) != panel1.BackColor)
                    {
                        backgroundFigure = new RectFigure();
                        ((RectFigure)backgroundFigure).Fill = WFHelper.MakeColor(panel1.BackColor);
                        ((RectFigure)backgroundFigure).StrokeWidth = 0;
                    }
                }
                else if (rbImage.Checked)
                {
                    if (!(backgroundFigure is ImageFigure) ||
                        ((ImageFigure)backgroundFigure).Bitmap == null ||
                        ((ImageFigure)backgroundFigure).Bitmap.NativeBmp != pictureBox1.Image)
                    {
                        byte[] imageData = (byte[])TypeDescriptor.GetConverter((Bitmap)pictureBox1.Image).
                            ConvertTo((Bitmap)pictureBox1.Image, typeof(byte[]));
                        backgroundFigure = new ImageFigure(new DRect(), 0, imageData);
                    }
                }
            }
        }
    }
}