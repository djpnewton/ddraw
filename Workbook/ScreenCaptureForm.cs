using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using DDraw;

namespace WinFormsDemo
{
    public partial class ScreenCaptureForm : Form
    {
        AnnotationForm annotationForm = null;

        public event ImportAnnotationsImageHandler CaptureImage;

        public ScreenCaptureForm()
        {
            InitializeComponent();
        }

        private void btnCaptureRect_Click(object sender, EventArgs e)
        {
            ShowAnnotationForm();
            // set annotation DEngine state to SelectMeasure
            annotationForm.De.HsmState = DHsmState.SelectMeasure;
            annotationForm.De.HsmStateChanged += new HsmStateChangedHandler(De_HsmStateChanged);
            annotationForm.De.MeasureRect += new SelectMeasureHandler(De_MeasureRect);
        }

        void ShowAnnotationForm()
        {
            // hide this form
            Hide();
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);
            // create the annotation form (this takes a screen grab)
            annotationForm = new AnnotationForm();
            annotationForm.Show();
            annotationForm.KeyPreview = true;
            annotationForm.KeyDown += new KeyEventHandler(annotationForm_KeyDown);
        }

        void annotationForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CloseAnnotationForm();
        }

        void De_HsmStateChanged(DEngine de, DHsmState state)
        {
            de.HsmStateChanged -= De_HsmStateChanged;
            CloseAnnotationForm();
        }

        void De_MeasureRect(DEngine de, DRect rect)
        {
            de.MeasureRect -= De_MeasureRect;
            if (CaptureImage != null && rect.Width > 0 && rect.Height > 0)
                // call the CaptureImage event passing it the captured bitmap
                CaptureImage(annotationForm.CaptureImage(rect));
            // close the annotation form
            CloseAnnotationForm();
        }

        void CloseAnnotationForm()
        {
            Show();
            if (annotationForm != null)
            {
                annotationForm.Close();
                annotationForm = null;
            }
        }

        private void btnCaptureFull_Click(object sender, EventArgs e)
        {
            ShowAnnotationForm();
            if (CaptureImage != null)
                // call the CaptureImage event passing it the captured bitmap
                CaptureImage(annotationForm.CaptureImage(new DRect(0, 0, annotationForm.Width, annotationForm.Height)));
            CloseAnnotationForm();
        }

        // Capture Window marklar ///////////////////////////////////////////////////

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern int SetCapture(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        IntPtr currentHwnd = IntPtr.Zero;
        int oldLeft;

        private void btnCaptureWindow_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
            currentHwnd = IntPtr.Zero;
            // capture mouse movement
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            SetCapture(Handle);
            MouseMove += new MouseEventHandler(ScreenCaptureForm_MouseMove);
            MouseUp += new MouseEventHandler(ScreenCaptureForm_MouseUp);
            // Hide form
            oldLeft = Left;
            Left = -Width;
        }

        void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            // get selected window
            Point pt = PointToScreen(new Point(e.X, e.Y));
            IntPtr hwnd = WindowFromPoint(new POINT(pt.X, pt.Y));
            RECT r;
            if (hwnd != currentHwnd)
            {
                // undo old window selection rect
                if (currentHwnd != IntPtr.Zero)
                {
                    GetWindowRect(currentHwnd, out r);
                    ControlPaint.DrawReversibleFrame(new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top), Color.AliceBlue, FrameStyle.Thick);
                }
                // show new selected window rect
                GetWindowRect(hwnd, out r);
                ControlPaint.DrawReversibleFrame(new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top), Color.AliceBlue, FrameStyle.Thick);
                // store selected window
                currentHwnd = hwnd;
            }
        }

        void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            // unhook mouse handlers
            MouseMove -= new MouseEventHandler(ScreenCaptureForm_MouseMove);
            MouseUp -= new MouseEventHandler(ScreenCaptureForm_MouseUp);
            if (currentHwnd != IntPtr.Zero)
            {
                // get selected window coords
                RECT r;
                GetWindowRect(currentHwnd, out r);
                // undo selection rect
                ControlPaint.DrawReversibleFrame(new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top), Color.AliceBlue, FrameStyle.Thick);
                // bring selected window forward
                SetForegroundWindow(currentHwnd);
                // capture image of window
                ShowAnnotationForm();
                if (CaptureImage != null)
                    // call the CaptureImage event passing it the captured bitmap
                    CaptureImage(annotationForm.CaptureImage(new DRect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top)));
                CloseAnnotationForm();
            }
            // Show form
            Left = oldLeft;
        }
    }
}
