// originally from http://www.c-sharpcorner.com/UploadFile/jwallroth/ColorPicker12022005021718AM/ColorPicker.aspx

// created on 06.09.2002 at 09:16
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Workbook
{
    public class ColorPicker : PopupForm
    {
        byte max = 40;
        Panel[] panel = new Panel[40];

        Color[] color = new Color[40]
	    {
		    //row 1
		    Color.FromArgb(0,0,0), Color.FromArgb(153,51,0), Color.FromArgb(51,51,0), Color.FromArgb(0,51,0),
		    Color.FromArgb(0,51,102), Color.FromArgb(0,0,128), Color.FromArgb(51,51,153), Color.FromArgb(51,51,51),
    		
		    //row 2
		    Color.FromArgb(128,0,0), Color.FromArgb(255,102,0), Color.FromArgb(128,128,0), Color.FromArgb(0,128,0),
		    Color.FromArgb(0,128,128), Color.FromArgb(0,0,255), Color.FromArgb(102,102,153), Color.FromArgb(128,128,128),
    		
		    //row 3
		    Color.FromArgb(255,0,0), Color.FromArgb(255,153,0), Color.FromArgb(153,204,0), Color.FromArgb(51,153,102),
		    Color.FromArgb(51,204,204), Color.FromArgb(51,102,255), Color.FromArgb(128,0,128), Color.FromArgb(153,153,153),
    		
		    //row 4
		    Color.FromArgb(255,0,255), Color.FromArgb(255,204,0), Color.FromArgb(255,255,0), Color.FromArgb(0,255,0),
		    Color.FromArgb(0,255,255), Color.FromArgb(0,204,255), Color.FromArgb(153,51,102), Color.FromArgb(192,192,192),
    		
		    //row 5
		    Color.FromArgb(255,153,204), Color.FromArgb(255,204,153), Color.FromArgb(255,255,153), Color.FromArgb(204,255,204),
		    Color.FromArgb(204,255,255), Color.FromArgb(153,204,255), Color.FromArgb(204,153,255), Color.FromArgb(255,255,255)						
	    };

        string[] colorName = new string[40]
	    {
		    "Black", "Brown", "Olive Green", "Dark Green", "Dark Teal", "Dark Blue", "Indigo", "Gray-80%",
		    "Dark Red", "Orange", "Dark Yellow", "Green", "Teal", "Blue", "Blue-Gray", "Gray-50%",
		    "Red", "Light Orange", "Lime", "Sea Green", "Aqua", "Light Blue", "Violet", "Gray-40%",
		    "Pink", "Gold", "Yellow", "Bright Green", "Turquoise", "Sky Blue", "Plum", "Gray-25%",
		    "Rose", "Tan", "Light Yellow", "Light Green", "Light Turquoise", "Pale Blue", "Lavender", "White"
	    };

        Button noneButton = new Button();
        Button moreColorsButton = new Button();

        Color selectedColor;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set
            {
                selectedColor = value;
                if (ColorSelected != null)
                    ColorSelected(this, new EventArgs());
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public event EventHandler ColorSelected;

        public ColorPicker(int x, int y, bool canSelectNone) : base(x, y)
        {
            BuildPalette();

            if (canSelectNone)
            {
                noneButton.Text = "None";
                noneButton.Size = new Size(69, 22);
                noneButton.Location = new Point(5, 99);
                noneButton.Click += new EventHandler(noneButton_Click);
                noneButton.FlatStyle = FlatStyle.Popup;
                Controls.Add(noneButton);
            }

            moreColorsButton.Text = "More colors ...";
            if (canSelectNone)
            {
                moreColorsButton.Size = new Size(69, 22);
                moreColorsButton.Location = new Point(80, 99);
            }
            else
            {
                moreColorsButton.Size = new Size(142, 22);
                moreColorsButton.Location = new Point(5, 99);
            }
            moreColorsButton.Click += new EventHandler(moreColorsButton_Click);
            moreColorsButton.FlatStyle = FlatStyle.Popup;
            Controls.Add(moreColorsButton);
        }

        void BuildPalette()
        {
            byte pwidth = 16;
            byte pheight = 16;
            byte pdistance = 2;
            byte border = 5;
            int x = border, y = border;
            ToolTip toolTip = new ToolTip();

            for (int i = 0; i < max; i++)
            {
                panel[i] = new Panel();
                panel[i].Height = pwidth;
                panel[i].Width = pheight;
                panel[i].Location = new Point(x, y);
                toolTip.SetToolTip(panel[i], colorName[i]);

                this.Controls.Add(panel[i]);

                if (x < (7 * (pwidth + pdistance)))
                    x += pwidth + pdistance;
                else
                {
                    x = border;
                    y += pheight + pdistance;
                }

                panel[i].BackColor = color[i];
                panel[i].MouseEnter += new EventHandler(OnMouseEnterPanel);
                panel[i].MouseLeave += new EventHandler(OnMouseLeavePanel);
                panel[i].MouseDown += new MouseEventHandler(OnMouseDownPanel);
                panel[i].MouseUp += new MouseEventHandler(OnMouseUpPanel);
                panel[i].Paint += new PaintEventHandler(OnPanelPaint);
            }
        }

        void noneButton_Click(object sender, System.EventArgs e)
        {
            SelectedColor = Color.Empty;
        }

        void moreColorsButton_Click(object sender, System.EventArgs e)
        {
            UseDeactivate = false;

            ColorDialog colDialog = new ColorDialog();
            colDialog.FullOpen = true;
            if (colDialog.ShowDialog() == DialogResult.OK)
                SelectedColor = colDialog.Color;
            colDialog.Dispose();

            UseDeactivate = true;
        }

        void OnMouseEnterPanel(object sender, EventArgs e)
        {
            DrawPanel(sender, 1);
        }

        void OnMouseLeavePanel(object sender, EventArgs e)
        {
            DrawPanel(sender, 0);
        }

        void OnMouseDownPanel(object sender, MouseEventArgs e)
        {
            DrawPanel(sender, 2);
        }

        void OnMouseUpPanel(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            SelectedColor = panel.BackColor;
        }

        void DrawPanel(object sender, byte state)
        {
            Panel panel = (Panel)sender;

            Graphics g = panel.CreateGraphics();

            Pen pen1, pen2;

            if (state == 1) 		//mouse over
            {
                pen1 = new Pen(SystemColors.ControlLightLight);
                pen2 = new Pen(SystemColors.ControlDarkDark);
            }
            else if (state == 2)	//clicked
            {
                pen1 = new Pen(SystemColors.ControlDarkDark);
                pen2 = new Pen(SystemColors.ControlLightLight);
            }
            else				//neutral
            {
                pen1 = new Pen(SystemColors.ControlDark);
                pen2 = new Pen(SystemColors.ControlDark);

            }

            Rectangle r = panel.ClientRectangle;
            Point p1 = new Point(r.Left, r.Top); 				//top left
            Point p2 = new Point(r.Right - 1, r.Top);			//top right
            Point p3 = new Point(r.Left, r.Bottom - 1);		    //bottom left
            Point p4 = new Point(r.Right - 1, r.Bottom - 1);	//bottom right

            g.DrawLine(pen1, p1, p2);
            g.DrawLine(pen1, p1, p3);
            g.DrawLine(pen2, p2, p4);
            g.DrawLine(pen2, p3, p4);
        }

        void OnPanelPaint(Object sender, PaintEventArgs e)
        {
            DrawPanel(sender, 0);
        }
    }
}
