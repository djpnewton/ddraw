using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Workbook
{
    public class TextPopup : PopupForm
    {
        TextBox tb = new TextBox();

        public override string Text
        {
            get { return tb.Text; }
            set { tb.Text = value; }
        }

        public TextPopup(int x, int y) : base(x, y)
        {
            ClientSize = new System.Drawing.Size(ClientSize.Width, tb.Height * 2);
            tb.Multiline = true;
            tb.Dock = DockStyle.Fill;
            Controls.Add(tb);

            KeyPreview = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            tb.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
                Close();
        }
    }
}
