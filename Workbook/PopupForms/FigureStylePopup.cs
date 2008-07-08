using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using DDraw;
using DDraw.WinForms;

namespace Workbook
{
    public delegate void FigureStyleEvent(object sender, PersonalToolbar.CustomFigureTool customFigure);

    public class FigureStylePopup : PopupForm
    {
        WFViewerControl vc;
        DTkViewer dv;
        DEngine de;

        Type figureClass;
        DAuthorProperties dap;

        public int AutoHideTimeout = 0;

        public event FigureStyleEvent AddToPersonalTools;

        public FigureStylePopup(int x, int y, Type figureClass, DAuthorProperties dap, bool antiAlias) : base(x, y)
        {
            this.figureClass = figureClass;
            this.dap = dap;
            // create the viewer
            vc = new WFViewerControl();
            vc.Parent = this;
            vc.Dock = DockStyle.Fill;
            dv = new WFViewer(vc);
            dv.Preview = true;
            dv.AntiAlias = antiAlias;
            // create the DEngine
            de = new DEngine(false);
            de.AddViewer(dv);
            // set page height to viewer size
            de.UndoRedoStart("blah");
            de.PageSize = new DPoint(vc.Width, vc.Height);
            de.UndoRedoCommit();
            // add the figure
            WorkBookUtils.PreviewFigure(de, dv, figureClass, dap, new DPoint(vc.Width, vc.Height));
            // buttons
            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 25;
            pnlTop.Parent = this;
            Button btnEdit = new Button();
            btnEdit.FlatStyle = FlatStyle.Popup;
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(1, 1);
            btnEdit.Parent = pnlTop;
            btnEdit.Click += new EventHandler(btnEdit_Click);
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            UseDeactivate = false;
            tmrShown.Stop();

            PersonalToolbar.PtButtonForm f = new PersonalToolbar.PtButtonForm();
            f.PersonalTool = new PersonalToolbar.CustomFigureTool(null, false, figureClass, dap.Clone(), null);
            f.SetupToolEdit();
            if (f.ShowDialog() == DialogResult.OK)
            {
                dap.SetProperties(((PersonalToolbar.CustomFigureTool)f.PersonalTool).Dap);
                if (f.ToolEditAddToPersonal && AddToPersonalTools != null)
                    AddToPersonalTools(this, (PersonalToolbar.CustomFigureTool)f.PersonalTool);
            }

            Close();
        }

        Timer tmrShown = new Timer();

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // time out
            if (AutoHideTimeout > 0)
            {
                tmrShown.Interval = AutoHideTimeout;
                tmrShown.Tick += new EventHandler(timer_Tick);
                tmrShown.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
