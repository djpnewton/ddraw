using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.IO;

namespace WinFormsDemo
{
    public static class ToolStripHelper
    {
        public static Image BlankImage
        {
            // single pixel transparent png
            get { return Image.FromStream(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAtJREFUGFdjYAACAAAFAAGq1chRAAAAAElFTkSuQmCC"))); }
        }

        public static void DrawStrokeWidthIcon(Graphics g, Rectangle r, Color color, int strokeWidth)
        {
            PointF pt1 = new PointF(r.X, r.Height / 2 + 1);
            PointF pt2 = new PointF(r.Right, r.Height / 2 + 1);
            g.DrawLine(new Pen(color, strokeWidth), pt1, pt2);
        }

        public static void DrawAlphaIcon(Graphics g, Rectangle r, Color fill, Color outline, double alpha)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Brush b = new SolidBrush(Color.FromArgb((int)(alpha * 255), fill));
            Pen p = new Pen(Color.FromArgb((int)(alpha * 255), outline));
            float w = r.Width * 0.75f;
            g.FillEllipse(b, new RectangleF(r.X, r.Y, w, r.Height));
            g.DrawEllipse(p, new RectangleF(r.X, r.Y, w, r.Height));
            float l = r.Left + r.Width * 0.25f;
            g.FillEllipse(b, new RectangleF(l, r.Y, w, r.Height));
            g.DrawEllipse(p, new RectangleF(l, r.Y, w, r.Height));
        }
    }

    public enum ColorType { Fill, Stroke };

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripColorButton : ToolStripButton
    {
        ColorType colorType = ColorType.Fill;
        public ColorType ColorType
        {
            get { return colorType; }
            set
            {
                colorType = value;
                Invalidate();
            }
        }

        Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                Invalidate();
            }
        }

        public ToolStripColorButton() : base ()
        {
        }

        const int INDENT = 2;

        protected override void OnPaint(PaintEventArgs e)
        {
            DisplayStyle = ToolStripItemDisplayStyle.None;
            base.OnPaint(e);
            Rectangle r = ContentRectangle;
            r.Inflate(-INDENT, -INDENT);
            r.Offset(-1, -1);
            Color outline, fill;
            if (Enabled)
            {
                outline = Color.Black;
                fill = color;
            }
            else
            {
                outline = Color.DarkGray;
                fill = Color.LightGray;
            }
            if (colorType == ColorType.Fill)
            {
                e.Graphics.FillRectangle(new SolidBrush(fill), r);
                e.Graphics.DrawRectangle(new Pen(outline), r);
            }
            else if (colorType == ColorType.Stroke)
            {
                e.Graphics.DrawRectangle(new Pen(outline), r);
                r.Inflate(-2, -2);
                e.Graphics.DrawRectangle(new Pen(fill, 3), r);
                r.Inflate(-2, -2);
                e.Graphics.DrawRectangle(new Pen(outline), r);
            }
        }
    }

    public class StrokeWidthMenuItem : ToolStripMenuItem
    {
        int value;
        public int Value
        {
            get { return value; }
        }

        public StrokeWidthMenuItem(int strokeWidth, EventHandler onClick)
            : base()
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            Text = "XXXXXXX";
            Click += onClick;
            this.value = strokeWidth;
        }
    }

    public class AlphaMenuItem : ToolStripMenuItem
    {
        double value;
        public double Value
        {
            get { return value; }
        }

        public AlphaMenuItem(double alpha, EventHandler onClick)
            : base()
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            Text = "XXXXXXX";
            Click += onClick;
            this.value = alpha;
        }
    }

    public class MySpecialRenderer : ToolStripProfessionalRenderer {

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) 
        {
            //RadioButtonRenderer.DrawRadioButton(e.Graphics, e.ImageRectangle.Location, System.Windows.Forms.VisualStyles.RadioButtonState.CheckedNormal);          
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Point pt = new Point(e.ImageRectangle.X + e.ImageRectangle.Width / 2, e.ImageRectangle.Y + e.ImageRectangle.Height / 2);
            Rectangle r = new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
            e.Graphics.FillEllipse(Brushes.Black, r);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is StrokeWidthMenuItem)
                ToolStripHelper.DrawStrokeWidthIcon(e.Graphics, e.TextRectangle, Color.Black, ((StrokeWidthMenuItem)e.Item).Value);
            else if (e.Item is AlphaMenuItem)
                ToolStripHelper.DrawAlphaIcon(e.Graphics, 
                    new Rectangle(e.TextRectangle.Location, new Size(e.TextRectangle.Height, e.TextRectangle.Height)), 
                    Color.Blue, Color.Black,
                    ((AlphaMenuItem)e.Item).Value);
            else
                base.OnRenderItemText(e);
        }
    }

    public delegate void StrokeWidthChangedHandler(object sender, int strokeWidth);

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripStrokeWidthButton : ToolStripDropDownButton
    {
        public event StrokeWidthChangedHandler StrokeWidthChanged;

        public int Value
        {
            get
            {
                foreach (StrokeWidthMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return 0;
            }
            set
            {
                foreach (StrokeWidthMenuItem item in DropDown.Items)
                    item.Checked = false;
                for (int i = 0; i < DropDown.Items.Count; i++)
                {
                    StrokeWidthMenuItem item = (StrokeWidthMenuItem)DropDown.Items[i];
                    if (value == item.Value)
                    {
                        item.Checked = true;
                        break;
                    }
                    if (value < item.Value)
                    {
                        if (i == 0)
                        {
                            item.Checked = true;
                            break;
                        }
                        else 
                        {
                            StrokeWidthMenuItem item2 = (StrokeWidthMenuItem)DropDown.Items[i - 1];
                            if (Math.Abs(value - item.Value) < Math.Abs(value - item2.Value))
                                item.Checked = true;
                            else
                                item2.Checked = true;
                            break;
                        }
                    }
                    else
                    {
                        if (i == DropDown.Items.Count-1)
                        {
                            item.Checked = true;
                            break;
                        }
                        else 
                        {
                            StrokeWidthMenuItem item2 = (StrokeWidthMenuItem)DropDown.Items[i + 1];
                            if (value <= item2.Value)
                            {
                                if (Math.Abs(value - item.Value) < Math.Abs(value - item2.Value))
                                    item.Checked = true;
                                else
                                    item2.Checked = true;
                                break;
                            }
                        }
                    }
                }
                Image = ToolStripHelper.BlankImage;
                Invalidate();
            }
        }

        public ToolStripStrokeWidthButton() : base()
        {
            ShowDropDownArrow = false;
            DropDown.Items.Add(new StrokeWidthMenuItem(1, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeWidthMenuItem(3, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeWidthMenuItem(5, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeWidthMenuItem(7, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeWidthMenuItem(9, new EventHandler(MenuItem_OnClick)));
            DropDown.ItemAdded += new ToolStripItemEventHandler(DropDown_ItemAdded);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image;
            base.OnPaint(e);
            Color outline;
            if (Enabled)
                outline = Color.Black;
            else
                outline = Color.DarkGray;
            ToolStripHelper.DrawStrokeWidthIcon(e.Graphics, ContentRectangle, outline, Value);
        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (newParent != null)
                newParent.Renderer = new MySpecialRenderer();
        }

        void DropDown_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (!(e.Item is StrokeWidthMenuItem))
                DropDown.Items.Remove(e.Item);
        }

        void MenuItem_OnClick(object sender, EventArgs e)
        {
            Value = ((StrokeWidthMenuItem)sender).Value;
            if (StrokeWidthChanged != null)
                StrokeWidthChanged(this, ((StrokeWidthMenuItem)sender).Value);
        }
    } 

    public delegate void AlphaChangedHandler(object sender, double alpha);

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripAlphaButton : ToolStripDropDownButton
    {
        public event AlphaChangedHandler AlphaChanged;

        public double Value
        {
            get
            {
                foreach (AlphaMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return 0;
            }
            set
            {
                foreach (AlphaMenuItem item in DropDown.Items)
                    item.Checked = false;
                for (int i = 0; i < DropDown.Items.Count; i++)
                {
                    AlphaMenuItem item = (AlphaMenuItem)DropDown.Items[i];
                    if (value == item.Value)
                    {
                        item.Checked = true;
                        break;
                    }
                    if (value > item.Value)
                    {
                        if (i == 0)
                        {
                            item.Checked = true;
                            break;
                        }
                        else
                        {
                            AlphaMenuItem item2 = (AlphaMenuItem)DropDown.Items[i - 1];
                            if (Math.Abs(value - item.Value) < Math.Abs(value - item2.Value))
                                item.Checked = true;
                            else
                                item2.Checked = true;
                            break;
                        }
                    }
                    else
                    {
                        if (i == DropDown.Items.Count - 1)
                        {
                            item.Checked = true;
                            break;
                        }
                        else
                        {
                            AlphaMenuItem item2 = (AlphaMenuItem)DropDown.Items[i + 1];
                            if (value >= item2.Value)
                            {
                                if (Math.Abs(value - item.Value) < Math.Abs(value - item2.Value))
                                    item.Checked = true;
                                else
                                    item2.Checked = true;
                                break;
                            }
                        }
                    }
                }
                Image = ToolStripHelper.BlankImage;
                Invalidate();
            }
        }

        public ToolStripAlphaButton() : base()
        {
            ShowDropDownArrow = false;
            DropDown.Items.Add(new AlphaMenuItem(1.0, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new AlphaMenuItem(0.5, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new AlphaMenuItem(0.4, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new AlphaMenuItem(0.3, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new AlphaMenuItem(0.2, new EventHandler(MenuItem_OnClick)));
            DropDown.ItemAdded += new ToolStripItemEventHandler(DropDown_ItemAdded);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image;
            base.OnPaint(e);
            Color outline, fill;
            if (Enabled)
            {
                outline = Color.Black;
                fill = Color.Blue;
            }
            else
            {
                outline = Color.DarkGray;
                fill = Color.LightGray;
            }
            Rectangle r = ContentRectangle;
            r.Inflate(-2, -2);
            r.Offset(-1, -1);
            ToolStripHelper.DrawAlphaIcon(e.Graphics, r, fill, outline, Value);
        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (newParent != null)
                newParent.Renderer = new MySpecialRenderer();
        }

        void DropDown_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (!(e.Item is AlphaMenuItem))
                DropDown.Items.Remove(e.Item);
        }

        void MenuItem_OnClick(object sender, EventArgs e)
        {
            Value = ((AlphaMenuItem)sender).Value;
            if (AlphaChanged != null)
                AlphaChanged(this, ((AlphaMenuItem)sender).Value);
        }
    }

    public class FontNameChooser : ComboBox
    {
        public string Value
        {
            get { return SelectedItem as string; }
            set { SelectedItem = value; }
        }

        public FontNameChooser() : base()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            System.Drawing.Text.InstalledFontCollection ifc = new System.Drawing.Text.InstalledFontCollection();
            foreach (FontFamily ff in ifc.Families)
                Items.Add(ff.Name);
        }

    }

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripFontNameChooser : ToolStripControlHost
    {
        public string Value
        {
            get { return FontNameChooser.Value; }
            set { FontNameChooser.Value = value; }
        }

        public FontNameChooser FontNameChooser
        {
            get { return Control as FontNameChooser; }
        }

        private static Control CreateControlInstance()
        {
            return new FontNameChooser();
        }

        public ToolStripFontNameChooser() : base(CreateControlInstance()) { }

        protected override void OnSubscribeControlEvents(Control c)
        {
            base.OnSubscribeControlEvents(c);
            FontNameChooser.SelectedIndexChanged += new EventHandler(FontNameChooser_SelectedIndexChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control c)
        {
            base.OnUnsubscribeControlEvents(c);
            FontNameChooser.SelectedIndexChanged -= new EventHandler(FontNameChooser_SelectedIndexChanged);
        }

        public event EventHandler FontNameChanged;

        void FontNameChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FontNameChanged != null)
                FontNameChanged(this, e);
        }
    }
}
