using System;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsDemo
{
    public class PopupForm : Form
    {
        Button cancelButton = new Button();
        public bool UseDeactivate = true;

        public PopupForm(int x, int y)
        {
            TopMost = true;
            Size = new Size(158, 132);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = MaximizeBox = ControlBox = false;
            ShowInTaskbar = false;
            CenterToScreen();

            // make sure popup is within working area of screen
            Rectangle workingArea = Screen.GetWorkingArea(new Point(x, y));
            if (x + Width > workingArea.Width)
                x = workingArea.Width - Width;
            if (y + Height > workingArea.Height)
                y = workingArea.Height - Height;
            Location = new Point(x, y);

            //"invisible" button to cancel at Escape
            cancelButton.Size = new Size(5, 5);
            cancelButton.Location = new Point(-10, -10);
            cancelButton.Click += new EventHandler(cancelButton_Click);
            Controls.Add(cancelButton);
            cancelButton.TabIndex = 0;
            cancelButton.DialogResult = DialogResult.Cancel;
            this.CancelButton = cancelButton;
        }

        void cancelButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            if (UseDeactivate && !Modal)
                Close();
            base.OnDeactivate(e);
        }
    }
}
