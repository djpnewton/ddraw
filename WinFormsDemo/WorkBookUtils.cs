using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using DDraw;

namespace WinFormsDemo
{
    public static class WorkBookUtils
    {
        public static void SetupDEngine(DEngine de)
        {
            // Workbook DEngine Settings
            de.SimplifyPolylines = true;
            de.SimplifyPolylinesTolerance = 0.5;
            de.AutoGroupPolylines = true;
            de.AutoGroupPolylinesTimeout = 2000;
            de.AutoGroupPolylinesXLimit = 100;
            de.AutoGroupPolylinesYLimit = 50;
            de.UsePolylineDots = true;
            de.FiguresBoundToPage = true;
        }

        public static void ViewerKeyDown(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
            de.FigureSelectAddToSelection = e.Shift;
        }

        public static void ViewerKeyUp(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
            de.FigureSelectAddToSelection = e.Shift;
        }

        const uint MA_ACTIVATE = 1;
        const uint MA_ACTIVATEANDEAT = 2;
        const uint MA_NOACTIVATE = 3;
        const uint MA_NOACTIVATEANDEAT = 4;

        const uint WM_MOUSEACTIVATE = 0x21;

        /// <summary>
        /// This WndProc implements click-through functionality. Some controls (MenuStrip, ToolStrip) will not
        /// recognize a click unless the form they are hosted in is active. So the first click will activate the
        /// form and then a second is required to actually make the click happen.
        /// </summary>
        /// <param name="m">The Message that was passed to your WndProc.</param>
        /// <returns>true if the message was processed, false if it was not</returns>
        /// <remarks>
        /// You should first call base.WndProc(), and then call this method. This method is only intended to
        /// change a return value, not to change actual processing before that.
        /// </remarks>
        public static bool ClickThroughWndProc(ref Message m)
        {
            bool returnVal = false;
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                if (m.Result == (IntPtr)MA_ACTIVATEANDEAT)
                {
                    m.Result = (IntPtr)MA_ACTIVATE;
                    returnVal = true;
                }
            }
            return returnVal;
        }

        public static bool IsOurAppActive
        {
            get
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form == Form.ActiveForm)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    public class WorkBookForm : Form
    {
        const int WM_ACTIVATE = 0x006;
        const int WM_ACTIVATEAPP = 0x01C;
        const int WM_PAINT = 0x000f;
        const int WM_NCPAINT = 0x0085;
        const int WM_NCACTIVATE = 0x086;

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr SendMessageW(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static bool PostMessageW(
            IntPtr handle,
            uint msg,
            IntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// Gets or sets the titlebar rendering behavior for when the form is deactivated.
        /// </summary>
        /// <remarks>
        /// If this property is false, the titlebar will be rendered in a different color when the form
        /// is inactive as opposed to active. If this property is true, it will always render with the
        /// active style. If the whole application is deactivated, the title bar will still be drawn in
        /// an inactive state.
        /// </remarks>
        bool forceActiveTitleBar = true;
        public bool ForceActiveTitleBar
        {
            get { return forceActiveTitleBar; }
            set { forceActiveTitleBar = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            HandleParentWndProc(ref m);
        }

        private int ignoreNcActivate = 0;

        /// <summary>
        /// Manages some special handling of window messages.
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if the message was handled, false if the caller should handle the message.</returns>
        public bool HandleParentWndProc(ref Message m)
        {
            bool returnVal = true;

            switch (m.Msg)
            {
                case WM_NCPAINT:
                    goto default;

                case WM_NCACTIVATE:
                    if (forceActiveTitleBar && m.WParam == IntPtr.Zero)
                    {
                        if (ignoreNcActivate > 0)
                        {
                            --ignoreNcActivate;
                            goto default;
                        }
                        else if (Form.ActiveForm != this ||  // Gets rid of: if you have the form active, then click on the desktop --> desktop refreshes
                                 !Visible)                   // Gets rid of: desktop refresh on exit
                        {
                            goto default;
                        }
                        else
                        {
                            // Only 'lock' for the topmost form in the application. Otherwise you get the whole system
                            // refreshing (i.e. the dreaded "repaint the whole desktop 5 times" glitch) when you do things
                            // like minimize the window
                            // And only lock if we aren't minimized. Otherwise the desktop refreshes.
                            bool locked = false;
                            if (Owner == null &&
                                WindowState != FormWindowState.Minimized)
                            {
                                locked = true;
                            }

                            //this.realParentWndProc(ref m);


                            SendMessageW(Handle, WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);

                            if (locked)
                            {
                                Invalidate(true);
                            }

                            break;
                        }
                    }
                    else
                    {
                        goto default;
                    }

                case WM_ACTIVATE:
                    goto default;

                case WM_ACTIVATEAPP:

                    //this.realParentWndProc(ref m);

                    // Check if the app is being deactivated
                    if (forceActiveTitleBar && m.WParam == IntPtr.Zero)
                    {
                        // If so, put our titlebar in the inactive state
                        PostMessageW(Handle, WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero);

                        ++ignoreNcActivate;
                    }

                    if (m.WParam == new IntPtr(1))
                    {
                        foreach (Form childForm in OwnedForms)
                        {
                            if (childForm is WorkBookForm && ((WorkBookForm)childForm).ForceActiveTitleBar && 
                                childForm.IsHandleCreated)
                            {
                                PostMessageW(childForm.Handle, WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                            }
                        }

                        if (Owner != null)
                        {
                            if (Owner is WorkBookForm && ((WorkBookForm)Owner).ForceActiveTitleBar && 
                                Owner.IsHandleCreated)
                            {
                                PostMessageW(Owner.Handle, WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                            }
                        }
                    }

                    break;

                default:
                    returnVal = false;
                    break;
            }

            return returnVal;
        }
    }
}
