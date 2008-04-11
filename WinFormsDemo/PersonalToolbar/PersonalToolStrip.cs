using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using DDraw;

namespace WinFormsDemo.PersonalToolbar
{
    public class PersonalToolStrip : ToolStripEx
    {
        ToolStripButton btnCustomize;

        DEngine de;
        public DEngine De
        {
            get { return de; }
            set { de = value; }
        }
        DAuthorProperties dap;
        public DAuthorProperties Dap
        {
            get { return dap; }
            set { dap = value; }
        }

        public PersonalToolStrip()
        {
            btnCustomize = new ToolStripButton(Resource1.user_add);
            btnCustomize.ToolTipText = "Customize Personal Toolbar";
            Items.Add(btnCustomize);
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == btnCustomize)
            {
                PtForm pf = new PtForm();
                pf.PersonalToolStrip = this;
                if (pf.ShowDialog() == DialogResult.OK)
                {
                    Clear();
                    foreach (object o in pf.ToolItems)
                    {
                        if (o is CustomFigureT)
                            Items.Add(new CustomFigureToolButton((CustomFigureT)o));
                        else if (o is RunCmdT)
                            Items.Add(new RunCmdToolButton((RunCmdT)o));
                        else if (o is ShowDirT)
                            Items.Add(new ShowDirToolButton((ShowDirT)o));
                    }
                }
            }
            else if (e.ClickedItem is CustomFigureToolButton)
            {
                System.Diagnostics.Debug.Assert(de != null, "ERROR: \"de\" is not assigned");
                System.Diagnostics.Debug.Assert(dap != null, "ERROR: \"dap\" is not assigned");
                CustomFigureToolButton b = (CustomFigureToolButton)e.ClickedItem;
                de.HsmSetStateByFigureClass(b.FigureClass);
                dap.SetProperties(b.FigureClass, b.Dap);
            }
            base.OnItemClicked(e);
        }

        public void Clear()
        {
            for (int i = Items.Count - 1; i > 0; i--)
                Items.RemoveAt(i);
        }
    }
}
