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

        public BackgroundFigure BackgroundFigure
        {
            get 
            {
                BackgroundFigure bg = new BackgroundFigure();
                bg.Fill = WFHelper.MakeColor(panel1.BackColor);
                if (rbColor.Checked)
                    bg.ImageData = null;
                else if (rbImage.Checked)
                {
                    byte[] imgData = WFHelper.ToImageData((Bitmap)pictureBox1.Image);
                    if (bg.Bitmap == null || bg.ImageData != imgData || bg.Position != ImagePosition)
                    {
                        bg.ImageData = imgData;
                        bg.Position = ImagePosition;
                        bg.FileName = imageFileName;
                    }
                }
                return bg;
            }
            set 
            {
                panel1.BackColor = WFHelper.MakeColor(value.Fill);
                if (value.Bitmap == null)
                {
                    rbColor.Checked = true;
                    pictureBox1.Image = pictureBox1.ErrorImage;
                }
                else
                {
                    rbImage.Checked = true;
                    pictureBox1.Image = WFHelper.FromImageData(value.ImageData);
                    ImagePosition = value.Position;
                    imageFileName = value.FileName;
                }
                UpdateControls();
            }
        }

        DImagePosition ImagePosition
        {
            get { return (DImagePosition)cbImagePos.SelectedIndex; }
            set { cbImagePos.SelectedIndex = (int)value; }
        }

        public bool ApplyAll
        {
            get { return cbApplyAll.Checked; }
        }

        public BackgroundForm()
        {
            InitializeComponent();
            ImagePosition = DImagePosition.Stretch;
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
    }
}