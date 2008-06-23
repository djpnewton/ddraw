using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsDemo
{
    public class FontNamePicker : PopupForm
    {
        ListBox lbFonts;

        string selectedFontName = "";
        public string SelectedFontName
        {
            get { return lbFonts.SelectedItem as string; }
            set
            {
                _initSelectedFontName(value);
                if (FontNameSelected != null)
                    FontNameSelected(this, new EventArgs());
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public event EventHandler FontNameSelected;

        public FontNamePicker(int x, int y, string fontName) : base(x, y)
        {
            // create the listbox
            lbFonts = new ListBox();
            lbFonts.Parent = this;
            lbFonts.Dock = DockStyle.Fill;
            // Set the draw mode so we can take over item drawing
            lbFonts.DrawMode = DrawMode.OwnerDrawVariable;
            // Handle the events
            lbFonts.MeasureItem += new MeasureItemEventHandler(MeasureItem);
            lbFonts.DrawItem += new DrawItemEventHandler(DrawItem);
            lbFonts.Click += new EventHandler(lbFonts_Click);
            lbFonts.KeyPress += new KeyPressEventHandler(lbFonts_KeyPress);
            // Create the list of fonts, and populate the ComboBox with that list
            PopulateFonts();
            // set initial fontname
            _initSelectedFontName(fontName);
        }

        private void _initSelectedFontName(string value)
        {
            selectedFontName = value;
            lbFonts.SelectedIndex = lbFonts.FindString(value);
        }

        void lbFonts_Click(object sender, EventArgs e)
        {
            SelectedFontName = lbFonts.SelectedItem as string;
        }

        void lbFonts_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                SelectedFontName = lbFonts.SelectedItem as string;
        }

        protected void MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index > -1)
            {
                string szFont = lbFonts.Items[e.Index].ToString();
                Graphics g = lbFonts.CreateGraphics();
                SizeF size = g.MeasureString(szFont, new Font(szFont, Font.Size));
                // Set the Item's Width
                e.ItemWidth = (int)size.Width;
                // If .NET gives you problems drawing fonts with different heights, set a maximum height
                e.ItemHeight = (int)size.Height;
                if (e.ItemHeight > 20)
                    e.ItemHeight = 20;
            }
        }

        protected void DrawItem(object sender, DrawItemEventArgs e)
        {
            // DrawBackground handles drawing the background (i.e,. hot-tracked v. not)
            // It uses the system colors (Bluish, and and white, by default)
            // same as calling e.Graphics.FillRectangle ( SystemBrushes.Highlight, e.Bounds );
            e.DrawBackground();

            if (e.Index > -1)
            {
                string szFont = lbFonts.Items[e.Index].ToString();
                Font fFont = new Font(szFont, Font.Size);

                Rectangle rectDraw = e.Bounds;

                if ((e.State | DrawItemState.Selected) == e.State)
                    e.Graphics.DrawString(szFont, fFont, SystemBrushes.HighlightText, rectDraw);
                else
                    e.Graphics.DrawString(szFont, fFont, SystemBrushes.WindowText, rectDraw);
            }
            // Uncomment this if you actually like the way the focus rectangle looks
            e.DrawFocusRectangle();
        }

        public void PopulateFonts()
        {
            foreach (FontFamily ff in FontFamily.Families)
                if (ff.IsStyleAvailable(FontStyle.Regular))
                    lbFonts.Items.Add(ff.Name);
            if (lbFonts.Items.Count > 0)
                lbFonts.SelectedIndex = lbFonts.FindString("Times");
        }
    }
}
