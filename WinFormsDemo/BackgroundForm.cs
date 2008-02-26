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
        string imageFileName;

        BackgroundFigure backgroundFigure;
        public BackgroundFigure BackgroundFigure
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
                if (backgroundFigure.ImageData == null)
                    return BackgroundType.Color;
                else
                    return BackgroundType.Image;
            }
        }

        DImagePosition ImagePosition
        {
            get { return (DImagePosition)cbImagePos.SelectedIndex; }
            set { cbImagePos.SelectedIndex = (int)value; }
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
                imageFileName = ofd.FileName;
            }
        }

        private void BackgroundForm_Shown(object sender, EventArgs e)
        {
            panel1.BackColor = WFHelper.MakeColor(backgroundFigure.Fill);
            pictureBox1.Image = pictureBox1.ErrorImage;
            ImagePosition = DImagePosition.Stretch;
            if (BackgroundType == BackgroundType.Color)
                rbColor.Checked = true;
            else if (BackgroundType == BackgroundType.Image)
            {
                if (backgroundFigure.Bitmap != null)
                    pictureBox1.Image = (Bitmap)backgroundFigure.Bitmap.NativeBmp;
                ImagePosition = backgroundFigure.Position;
                imageFileName = backgroundFigure.FileName;
                rbImage.Checked = true;
            }
            UpdateControls();
        }

        void UpdateControls()
        {
            panel1.Enabled = rbColor.Checked;
            btnImageBrowse.Enabled = rbImage.Checked;
            pictureBox1.Enabled = rbImage.Checked;
            cbImagePos.Enabled = rbImage.Checked;
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
                backgroundFigure.Fill = WFHelper.MakeColor(panel1.BackColor);
                if (rbColor.Checked)
                    backgroundFigure.ImageData = null;
                else if (rbImage.Checked)
                {
                    if ((backgroundFigure.Bitmap == null) || 
                        backgroundFigure.Bitmap.NativeBmp != (Bitmap)pictureBox1.Image ||
                        backgroundFigure.Position != ImagePosition)
                    {
                        backgroundFigure.ImageData = WFHelper.ToImageData((Bitmap)pictureBox1.Image);
                        backgroundFigure.Position = ImagePosition;
                        backgroundFigure.FileName = imageFileName;
                    }
                }
            }
        }
    }
}