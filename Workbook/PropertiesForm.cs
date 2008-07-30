using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDraw;
using DDraw.WinForms;

namespace Workbook
{
    public partial class PropertiesForm : Form
    {
        public IList<Figure> Figures
        {
            set
            {
                // copy figures to our DEngine
                string xml = FigureSerialize.FormatToXml(value, null);
                de.UndoRedo.Start("blah");
                List<Figure> figs = FigureSerialize.FromXml(xml);
                foreach (Figure f in figs)
                    de.AddFigure(f);
                // put figures in center of page and adjust for aspect ratios
                if (figs.Count > 1)         // group figures
                    de.GroupFigures(figs);
                if (de.Figures.Count == 1)  // position single (or grouped) figure and adjust page size
                {
                    Figure f = de.Figures[0];
                    const int space = 5;
                    double vcAr = vc.Width / (double)vc.Height;
                    DRect r = DGeom.BoundingBoxOfRotatedRect(f.GetEncompassingRect(), f.Rotation);
                    double fAr = r.Width / r.Height;
                    if (vcAr > fAr)
                    {
                        de.PageSize = new DPoint(r.Width / fAr * vcAr + space * 2, r.Height + space * 2);
                        f.Left = f.Left - r.Left + (de.PageSize.X / 2 - r.Width / 2);
                        f.Top = f.Top - r.Top + space;
                    }
                    else
                    {
                        de.PageSize = new DPoint(r.Width + space * 2, r.Height * fAr / vcAr + space * 2);
                        f.Left = f.Left - r.Left + space;
                        f.Top = f.Top - r.Top + (de.PageSize.Y / 2 - r.Height / 2);
                    }
                    if (figs.Count > 1)     // ungroup figures
                        de.UngroupFigures(de.Figures);
                }
                de.UndoRedo.Commit();
                // select all figures for property adjustment
                de.SelectAll();
            }
            get { return de.Figures; }
        }

        DEngine de;
        DTkViewer dv;

        public PropertiesForm()
        {
            InitializeComponent();
            LocalizeUI();
            dv = new WFViewer(vc);
            dv.EditFigures = false;
            dv.AntiAlias = true;
            dv.Preview = true;
            de = new DEngine(null);
            de.AddViewer(dv);
            // set page height to viewer size
            de.UndoRedo.Start("blah");
            de.PageSize = new DPoint(vc.Width, vc.Height);
            de.UndoRedo.Commit();

            tsFigureProps.De = de;
            tsFigureProps.Dv = dv;
        }

        private void LocalizeUI()
        {
            Text = WbLocale.FigureProperties;
            btnOk.Text = WbLocale.Ok;
            btnCancel.Text = WbLocale.Cancel;
        }
    }
}