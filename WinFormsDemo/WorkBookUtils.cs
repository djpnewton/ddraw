using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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
            de.FiguresBoundToPage = true;
        }

        public static void ViewerKeyDown(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
        }

        public static void ViewerKeyUp(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
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
}
