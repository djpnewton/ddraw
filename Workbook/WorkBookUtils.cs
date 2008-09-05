using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

using Nini.Config;
using DDraw;
using DDraw.WinForms;

namespace Workbook
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
            de.FiguresDeselectOnSingleClick = true;
            // localize undo/redo commands
            de.SelectOperationName = WbLocale.SelectOperation;
            de.AddLineName = WbLocale.AddLine;
            de.AddTextName = WbLocale.AddText;
            de.AddName = WbLocale.Add;
            de.TextEditName = WbLocale.TextEdit;
            de.FigureEditName = WbLocale.FigureEdit;
            de.EraseOperationName = WbLocale.EraseOperation;
            de.MoveName = WbLocale.Move;
        }

        public static void ViewerKeyDown(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
            de.FigureSelectToggleToSelection = e.Shift;
            SetKeyMovementRate(de, e);
        }

        public static void ViewerKeyUp(DEngine de, KeyEventArgs e)
        {
            de.FigureLockAspectRatio = e.Shift;
            de.FigureAlwaysSnapAngle = e.Shift;
            de.FigureSelectToggleToSelection = e.Shift;
            SetKeyMovementRate(de, e);
        }

        static void SetKeyMovementRate(DEngine de, KeyEventArgs e)
        {
            if (e.Shift)
                de.KeyMovementRate = 5;
            else
                de.KeyMovementRate = 1;
        }

        #region ClickThroughWndProc
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
        #endregion

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

        public static string BitmapToBase64(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            string base64 = Convert.ToBase64String(ms.ToArray());
            ms.Close();
            return base64;
        }

        public static Bitmap Base64ToBitmap(string base64)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64));
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            return bmp;
        }

        public static void PutInBounds(DEngine de, Figure f)
        {
            // make sure figure is within page bounds
            if (f.Left < 0)
                f.Left = 0;
            else if (f.Right > de.PageSize.X)
                f.Left = de.PageSize.X - f.Width;
            if (f.Top < 0)
                f.Top = 0;
            else if (f.Bottom > de.PageSize.Y)
                f.Top = de.PageSize.Y - f.Height;
        }

        public static void PreviewFigure(DEngine de, DTkViewer dv, Type figureClass, DAuthorProperties dap, DPoint viewerSize)
        {
            // add figure de so it shows on the viewer
            de.UndoRedo.Start("blah");
            de.ClearPage();
            Figure f = (Figure)Activator.CreateInstance(figureClass);
            dap.ApplyPropertiesToFigure(f);
            if (f is PolylineFigure || f is LineFigure)
            {
                DPoint pt1 = new DPoint(viewerSize.X / 4.0, viewerSize.Y / 4.0);
                DPoint pt2 = new DPoint(viewerSize.X * 3 / 4.0, viewerSize.Y * 3 / 4.0);
                if (f is PolylineFigure)
                {
                    DPoints pts = new DPoints();
                    pts.Add(pt1);
                    pts.Add(new DPoint(viewerSize.X * 1.25 / 4.0, viewerSize.Y * 1.10 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 1.50 / 4.0, viewerSize.Y * 1.25 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 1.75 / 4.0, viewerSize.Y * 1.50 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 2.00 / 4.0, viewerSize.Y * 1.75 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 2.25 / 4.0, viewerSize.Y * 2.00 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 2.50 / 4.0, viewerSize.Y * 2.25 / 4.0));
                    pts.Add(new DPoint(viewerSize.X * 2.75 / 4.0, viewerSize.Y * 2.50 / 4.0));
                    pts.Add(pt2);
                    ((PolylineFigure)f).Points = pts;
                }
                else if (f is LineFigure)
                {
                    ((LineFigure)f).Pt1 = pt1;
                    ((LineFigure)f).Pt2 = pt2;
                }
            }
            else if (f is ITextable)
                ((ITextable)f).Text = "Aa";
            f.Left = viewerSize.X / 4.0;
            f.Top = viewerSize.Y / 4.0;
            f.Width = viewerSize.X / 2.0;
            f.Height = viewerSize.Y / 2.0;
            if (f is TextFigure)
            {
                f.Left = viewerSize.X / 8.0;
                f.Width = viewerSize.X * 3 / 4.0;
            }
            de.AddFigure(f);
            de.UndoRedo.Commit();
            dv.Update();
        }

        public static byte[] GetBytesFromFile(string fileName)
        {
            FileStream fs = File.OpenRead(fileName);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return bytes;
        }

        public static string GetTempFileName(string proposedName, string tempDir)
        {
            return GetTempFileName(proposedName, tempDir, Path.GetExtension(proposedName));
        }

        public static string GetTempFileName(string proposedName, string tempDir, string fileExt)
        {
            string leaf = Path.GetFileNameWithoutExtension(proposedName);
            string tempFileName;
            int n = 1;
            do
            {
                tempFileName = string.Format("{0}[{1}]{2}", Path.Combine(tempDir, leaf), n, fileExt);
                n++;
            }
            while (File.Exists(tempFileName));
            return tempFileName;
        }

        #region DAuthorProperties Config
        const string FILL_OPT = "Fill";
        const string STROKE_OPT = "Stroke";
        const string STROKEWIDTH_OPT = "StrokeWidth";
        const string STROKESTYLE_OPT = "StrokeStyle";
        const string ALPHA_OPT = "Alpha";
        const string STARTMARKER_OPT = "StartMarker";
        const string ENDMARKER_OPT = "EndMarker";
        const string FONTNAME_OPT = "FontName";
        const string BOLD_OPT = "Bold";
        const string ITALICS_OPT = "Italics";
        const string UNDERLINE_OPT = "Underline";
        const string STRIKETHROUGH_OPT = "Strikethrough";

        public static void WriteDapToConfig(DAuthorProperties dap, IConfig config)
        {
            config.Set(FILL_OPT, DColor.FormatToString(dap.Fill));
            config.Set(STROKE_OPT, DColor.FormatToString(dap.Stroke));
            config.Set(STROKEWIDTH_OPT, dap.StrokeWidth);
            config.Set(STROKESTYLE_OPT, dap.StrokeStyle.ToString());
            config.Set(ALPHA_OPT, dap.Alpha);
            config.Set(STARTMARKER_OPT, dap.StartMarker);
            config.Set(ENDMARKER_OPT, dap.EndMarker);
            config.Set(FONTNAME_OPT, dap.FontName);
            config.Set(BOLD_OPT, dap.Bold);
            config.Set(ITALICS_OPT, dap.Italics);
            config.Set(UNDERLINE_OPT, dap.Underline);
            config.Set(STRIKETHROUGH_OPT, dap.Strikethrough);
        }

        public static void ReadConfigToDap(IConfig config, DAuthorProperties dap)
        {
            dap.Fill = DColor.FromString(config.Get(FILL_OPT));
            dap.Stroke = DColor.FromString(config.Get(STROKE_OPT));
            dap.StrokeWidth = config.GetInt(STROKEWIDTH_OPT, 1);
            dap.StrokeStyle = (DStrokeStyle)Enum.Parse(typeof(DStrokeStyle), 
                config.Get(STROKESTYLE_OPT, DStrokeStyle.Solid.ToString()), true);
            dap.Alpha = config.GetDouble(ALPHA_OPT, 1);
            dap.StartMarker = (DMarker)Enum.Parse(typeof(DMarker), 
                config.Get(STARTMARKER_OPT, DMarker.None.ToString()), true);
            dap.EndMarker = (DMarker)Enum.Parse(typeof(DMarker),
                config.Get(ENDMARKER_OPT, DMarker.None.ToString()), true);
            dap.FontName = config.Get(FONTNAME_OPT, "Arial");
            dap.Bold = config.GetBoolean(BOLD_OPT, false);
            dap.Italics = config.GetBoolean(ITALICS_OPT, false);
            dap.Underline = config.GetBoolean(UNDERLINE_OPT, false);
            dap.Strikethrough = config.GetBoolean(STRIKETHROUGH_OPT, false);
        }
        #endregion

        #region Render DEngine to PDF
        public static void RenderPdf(IList<DEngine> expEngines, string fileName)
        {
            /*** no progress form as this requires Application.DoEvents which will 
            kill rendering (cairo/gdi+ at the same time) ***/

            WFCairoGraphics dg = WFHelper.MakeCairoPDFGraphics(fileName, 0, 0);
            dg.Scale(0.75, 0.75); // TODO figure out why this is needed (gak!)
            foreach (DEngine de in expEngines)
            {
                WFHelper.SetCairoPDFSurfaceSize(dg, PageTools.SizetoSizeMM(de.PageSize));
                // create cairifyed figures/DEngine
                DEngine cairoDe = CairifyEngine(de);
                // print page
                DPrintViewer dvPrint = new DPrintViewer();
                //dvPrint.SetPageSize(de.PageSize);
                dvPrint.Paint(dg, cairoDe.BackgroundFigure, cairoDe.Figures);
                WFHelper.ShowCairoPDFPage(dg);
            }
            dg.Dispose();
            GC.Collect(); // release all the cairo stuff so the pdf gets written?
        }

        private static DEngine CairifyEngine(DEngine de)
        {
            if (!WFHelper.Cairo)
            {
                // set to cairo graphics
                WFCairoGraphics.Init();
                // create new engine
                DEngine cairoDe = new DEngine(null);
                cairoDe.UndoRedo.Start("blah");
                // page size
                cairoDe.PageSize = de.PageSize;
                // figures
                List<Figure> figs = FigureSerialize.FromXml(FigureSerialize.FormatToXml(de.Figures, null));
                foreach (Figure f in figs)
                    cairoDe.AddFigure(f);
                // background figure
                figs = FigureSerialize.FromXml(FigureSerialize.FormatToXml(de.BackgroundFigure, null));
                //CairifyFigures(figs);
                cairoDe.SetBackgroundFigure((BackgroundFigure)figs[0], de.CustomBackgroundFigure);
                cairoDe.UndoRedo.Commit();
                // reset back to GDI graphics
                GDIGraphics.Init();
                return cairoDe;
            }
            return de;
        }
        #endregion

        public static List<Figure> FlatFigureList(IList<Figure> figs)
        {
            List<Figure> result = new List<Figure>();
            foreach (Figure f in figs)
            {
                result.Add(f);
                if (f is IChildFigureable)
                {
                    List<Figure> children = FlatFigureList(((IChildFigureable)f).ChildFigures);
                    foreach (Figure f2 in children)
                        result.Add(f2);
                }
            }
            return result;
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
