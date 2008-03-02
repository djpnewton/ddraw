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
    }
}
