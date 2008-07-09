using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.IO;
using DDraw;

namespace Workbook
{
    public static class ToolStripHelper
    {
        public static Image BlankImage
        {
            // 16x16 transparent png
            get { return Image.FromStream(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAABVJREFUOE9jYBgFoyEwGgKjIQAJAQAEEAABcqUTdgAAAABJRU5ErkJggg=="))); }
        }

        public static void DrawStrokeWidthIcon(Graphics g, Rectangle r, Color color, int strokeWidth)
        {
            if (strokeWidth == ToolStripStrokeWidthButton.Empty)
                strokeWidth = 1;
            PointF pt1 = new PointF(r.X, r.Height / 2 + 1);
            PointF pt2 = new PointF(r.Right, r.Height / 2 + 1);
            g.DrawLine(new Pen(color, strokeWidth), pt1, pt2);
        }

        public static void DrawStrokeStyleIcon(Graphics g, Rectangle r, Color color, DStrokeStyle strokeStyle)
        {
            Pen p = new Pen(color, 2);
            switch (strokeStyle)
            {
                case DStrokeStyle.Solid:
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    break;
                case DStrokeStyle.Dash:
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    break;
                case DStrokeStyle.Dot:
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    break;
                case DStrokeStyle.DashDot:
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    break;
                case DStrokeStyle.DashDotDot:
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    break;
            }
            PointF pt1 = new PointF(r.X, r.Height / 2 + 1);
            PointF pt2 = new PointF(r.Right, r.Height / 2 + 1);
            g.DrawLine(p, pt1, pt2);
        }

        public static void DrawMarkerIcon(Graphics g, Rectangle r, Color color, DMarker marker, bool start)
        {
            const int sz = 8;
            const int hsz = sz / 2;
            int mid = r.Height / 2 + 1;
            Pen p = new Pen(color, sz / 2);
            Brush b = new SolidBrush(color);
            switch (marker)
            {
                case DMarker.None:
                    break;
                case DMarker.Arrow:
                    if (start)
                        g.FillPolygon(b, new Point[3] { new Point(r.Left, mid), new Point(r.Left + sz, mid - hsz), 
                                                        new Point(r.Left + sz, mid + hsz)});
                    else
                        g.FillPolygon(b, new Point[3] { new Point(r.Right, mid), new Point(r.Right - sz, mid - hsz), 
                                                        new Point(r.Right - sz, mid + hsz)});
                break;
                case DMarker.Dot:
                    if (start)
                        g.FillEllipse(b, new Rectangle(r.Left, mid - hsz, sz, sz));
                    else
                        g.FillEllipse(b, new Rectangle(r.Right - sz, mid - hsz, sz, sz));
                    break;
                case DMarker.Square:
                    if (start)
                        g.FillRectangle(b, new Rectangle(r.Left, mid - hsz, sz, sz)); 
                    else
                        g.FillRectangle(b, new Rectangle(r.Right - sz, mid - hsz, sz, sz));
                    break;
                case DMarker.Diamond:
                    if (start)
                        g.FillPolygon(b, new Point[4] { new Point(r.Left + hsz, mid - hsz), new Point(r.Left + sz, mid), 
                                                        new Point(r.Left + hsz, mid + hsz), new Point(r.Left, mid)});
                    else
                        g.FillPolygon(b, new Point[4] { new Point(r.Right - hsz, mid - hsz), new Point(r.Right - sz, mid), 
                                                        new Point(r.Right - hsz, mid + hsz), new Point(r.Right, mid)});
                    break;
            }
            PointF pt1, pt2;
            if (start)
            {
                pt1 = new PointF(r.X + hsz, mid);
                pt2 = new PointF(r.Right, mid);
            }
            else
            {
                pt1 = new PointF(r.Right - hsz, mid);
                pt2 = new PointF(r.Left, mid);
            }
            g.DrawLine(p, pt1, pt2);
        }

        public static void DrawAlphaIcon(Graphics g, Rectangle r, Color fill, Color outline, double alpha)
        {
            if (alpha == ToolStripAlphaButton.Empty)
                alpha = 1;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Brush b = new SolidBrush(Color.FromArgb((int)(alpha * 255), fill));
            Pen p = new Pen(Color.FromArgb((int)(alpha * 255), outline));
            float w = r.Width * 0.75f;
            float h = r.Height * 0.75f;
            float l = r.Left + r.Width * 0.125f;
            g.FillEllipse(b, new RectangleF(l, r.Y, w, h));
            g.DrawEllipse(p, new RectangleF(l, r.Y, w, h));
            float t = r.Top + r.Height * 0.25f;
            g.FillEllipse(b, new RectangleF(r.X, t, w, h));
            g.DrawEllipse(p, new RectangleF(r.X, t, w, h));
            l = r.Left + r.Width * 0.25f;
            g.FillEllipse(b, new RectangleF(l, t, w, h));
            g.DrawEllipse(p, new RectangleF(l, t, w, h));
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

    public class StrokeStyleMenuItem : ToolStripMenuItem
    {
        DStrokeStyle value;
        public DStrokeStyle Value
        {
            get { return value; }
        }

        public StrokeStyleMenuItem(DStrokeStyle strokeStyle, EventHandler onClick)
            : base()
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            Text = "XXXXXXX";
            Click += onClick;
            this.value = strokeStyle;
        }
    }

    public class MarkerMenuItem : ToolStripMenuItem
    {
        DMarker value;
        public DMarker Value
        {
            get { return value; }
        }

        public bool Start = true;

        public MarkerMenuItem(DMarker marker, EventHandler onClick)
            : base()
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            Text = "XXXXXXX";
            Click += onClick;
            this.value = marker;
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
            System.Drawing.Drawing2D.GraphicsState gs = e.Graphics.Save();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Point pt = new Point(e.ImageRectangle.X + e.ImageRectangle.Width / 2, e.ImageRectangle.Y + e.ImageRectangle.Height / 2);
            Rectangle r = new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
            e.Graphics.FillEllipse(Brushes.Black, r);
            e.Graphics.Restore(gs);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is StrokeWidthMenuItem)
                ToolStripHelper.DrawStrokeWidthIcon(e.Graphics, e.TextRectangle, Color.Black, ((StrokeWidthMenuItem)e.Item).Value);
            else if (e.Item is StrokeStyleMenuItem)
                ToolStripHelper.DrawStrokeStyleIcon(e.Graphics, e.TextRectangle, Color.Black, ((StrokeStyleMenuItem)e.Item).Value);
            else if (e.Item is MarkerMenuItem)
                ToolStripHelper.DrawMarkerIcon(e.Graphics, e.TextRectangle, Color.Black, ((MarkerMenuItem)e.Item).Value, ((MarkerMenuItem)e.Item).Start);
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

        public static int Empty = -1;
        
        public int Value
        {
            get
            {
                foreach (StrokeWidthMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return Empty;
            }
            set
            {
                foreach (StrokeWidthMenuItem item in DropDown.Items)
                    item.Checked = false;
                if (value != Empty)
                {
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
                            if (i == DropDown.Items.Count - 1)
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

    public delegate void StrokeStyleChangedHandler(object sender, DStrokeStyle strokeStyle);

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripStrokeStyleButton : ToolStripDropDownButton
    {
        public event StrokeStyleChangedHandler StrokeStyleChanged;

        public static int Empty = -1;

        public DStrokeStyle Value
        {
            get
            {
                foreach (StrokeStyleMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return DStrokeStyle.Solid;
            }
            set
            {
                foreach (StrokeStyleMenuItem item in DropDown.Items)
                {
                    if (item.Value == value)
                        item.Checked = true;
                    else
                    item.Checked = false;
                }
                Image = ToolStripHelper.BlankImage;
                Invalidate();
            }
        }

        public ToolStripStrokeStyleButton()
            : base()
        {
            ShowDropDownArrow = false;
            DropDown.Items.Add(new StrokeStyleMenuItem(DStrokeStyle.Solid, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeStyleMenuItem(DStrokeStyle.Dash, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeStyleMenuItem(DStrokeStyle.Dot, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeStyleMenuItem(DStrokeStyle.DashDot, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new StrokeStyleMenuItem(DStrokeStyle.DashDotDot, new EventHandler(MenuItem_OnClick)));
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
            ToolStripHelper.DrawStrokeStyleIcon(e.Graphics, ContentRectangle, outline, Value);
        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (newParent != null)
                newParent.Renderer = new MySpecialRenderer();
        }

        void DropDown_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (!(e.Item is StrokeStyleMenuItem))
                DropDown.Items.Remove(e.Item);
        }

        void MenuItem_OnClick(object sender, EventArgs e)
        {
            Value = ((StrokeStyleMenuItem)sender).Value;
            if (StrokeStyleChanged != null)
                StrokeStyleChanged(this, ((StrokeStyleMenuItem)sender).Value);
        }
    }

    public delegate void MarkerChangedHandler(object sender, DMarker marker);

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripMarkerButton : ToolStripDropDownButton
    {
        public event MarkerChangedHandler MarkerChanged;

        public static int Empty = -1;
     
        public DMarker Value
        {
            get
            {
                foreach (MarkerMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return DMarker.None;
            }
            set
            {
                foreach (MarkerMenuItem item in DropDown.Items)
                {
                    if (item.Value == value)
                        item.Checked = true;
                    else
                        item.Checked = false;
                }
                Image = ToolStripHelper.BlankImage;
                Invalidate();
            }
        }
        
        bool start = true;
        public bool Start
        {
            get { return start; }
            set
            {
                foreach (MarkerMenuItem item in DropDown.Items)
                    item.Start = value;
                start = value;
            }
        }
        
        public ToolStripMarkerButton() : base()
        {
            ShowDropDownArrow = false;
            DropDown.Items.Add(new MarkerMenuItem(DMarker.None, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new MarkerMenuItem(DMarker.Arrow, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new MarkerMenuItem(DMarker.Dot, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new MarkerMenuItem(DMarker.Square, new EventHandler(MenuItem_OnClick)));
            DropDown.Items.Add(new MarkerMenuItem(DMarker.Diamond, new EventHandler(MenuItem_OnClick)));
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
            ToolStripHelper.DrawMarkerIcon(e.Graphics, ContentRectangle, outline, Value, start);
        }
        
        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (newParent != null)
                newParent.Renderer = new MySpecialRenderer();
        }
        
        void DropDown_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (!(e.Item is MarkerMenuItem))
                DropDown.Items.Remove(e.Item);
        }

        void MenuItem_OnClick(object sender, EventArgs e)
        {
            Value = ((MarkerMenuItem)sender).Value;
            if (MarkerChanged != null)
                MarkerChanged(this, ((MarkerMenuItem)sender).Value);
        }
    } 

    public delegate void AlphaChangedHandler(object sender, double alpha);

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripAlphaButton : ToolStripDropDownButton
    {
        public event AlphaChangedHandler AlphaChanged;

        public static int Empty = -1;

        public double Value
        {
            get
            {
                foreach (AlphaMenuItem item in DropDown.Items)
                    if (item.Checked)
                        return item.Value;
                return Empty;
            }
            set
            {
                foreach (AlphaMenuItem item in DropDown.Items)
                    item.Checked = false;
                if (value != Empty)
                {
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

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripFontNameButton : ToolStripButton
    {
        string fontName = "";
        public string Value
        {
            get { return fontName; }
            set  { fontName = value; }
        }

        public ToolStripFontNameButton() : base()
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image;
            Image = Resource1.font;
            ToolTipText = "Font";
        }
    }

    // based off code in Paint.Net for mono (http://code.google.com/p/paint-mono/)
    public class ToolStripEx : ToolStrip
    {
        bool clickThrough = true;

        /// <summary>
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
        /// not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is true, which is the opposite of the behavior provided by the base
        /// ToolStrip class.
        /// </remarks>
        public bool ClickThrough
        {
            get { return clickThrough; }
            set { clickThrough = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.clickThrough)
                WorkBookUtils.ClickThroughWndProc(ref m);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            PopupForm.HidePopups();
        }

        /// <summary>
        /// To fix up the gastly default tooltips on ToolStripItems.
        /// Otherwise they blink in and out of focus if the ToolStripItem is close to the bottom of the screens working area.
        /// </summary>
        public bool UseDecentToolTips
        {
            get { return !ShowItemToolTips; }
            set { ShowItemToolTips = !value; }
        }

        ToolTip ToolTip = new ToolTip();
        ToolStripItem itemEntered = null;

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            base.OnMouseMove(mea);
            if (UseDecentToolTips)
            {
                // find new item entered
                ToolStripItem newItemEntered = null;
                foreach (ToolStripItem item in Items)
                    if (mea.X >= item.Bounds.X && mea.X <= item.Bounds.Right &&
                        mea.Y >= item.Bounds.Y && mea.Y <= item.Bounds.Bottom)
                    {
                        newItemEntered = item;
                        break;
                    }
                // hide tooltip if different item to last time
                if (newItemEntered != itemEntered)
                    ToolTip.Hide(this);
                // set tooltip based on item entered
                itemEntered = newItemEntered;
                if (itemEntered != null)
                {
                    string toolTipText = itemEntered.ToolTipText;
                    if (toolTipText == null || toolTipText.Length == 0)
                        toolTipText = itemEntered.Text;
                    if (toolTipText != null && toolTipText.Length != 0)
                    {
                        if (ToolTip.GetToolTip(this) != toolTipText)
                            ToolTip.SetToolTip(this, toolTipText);
                    }
                    else
                        ToolTip.SetToolTip(this, null);
                }
                else
                    ToolTip.SetToolTip(this, null);

                if (itemEntered != null)
                {
                    if (ToolTip.GetToolTip(this) != itemEntered.Text && ToolTip.GetToolTip(this) != itemEntered.ToolTipText)
                    {
                        ToolTip.SetToolTip(this, null);
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (UseDecentToolTips)
            {
                ToolTip.Hide(this);
                ToolTip.SetToolTip(this, null);
            }
        }
    }
}
