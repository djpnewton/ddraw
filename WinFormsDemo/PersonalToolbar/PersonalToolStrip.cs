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
                        else if (o is WebLinkT)
                            Items.Add(new WebLinkToolButton((WebLinkT)o));
                    }
                }
            }
            else if (e.ClickedItem is CustomFigureToolButton)
            {
                CustomFigureToolButton b = (CustomFigureToolButton)e.ClickedItem;
                System.Diagnostics.Debug.Assert(de != null, "ERROR: \"de\" is not assigned");
                de.HsmSetStateByFigureClass(b.FigureClass);
                System.Diagnostics.Debug.Assert(dap != null, "ERROR: \"dap\" is not assigned");
                dap.SetProperties(b.FigureClass, b.Dap);
            }
            base.OnItemClicked(e);
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);
            e.Item.MouseDown += new MouseEventHandler(Item_MouseDown);
        }

        void Item_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && sender != btnCustomize)
            {
                ToolStripItem tsItem = (ToolStripItem)sender;
                if (tsItem.Owner != null)
                {
                    // make context menu
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripItem item = menu.Items.Add("Properties");
                    item.Click += delegate(object s, EventArgs e2)
                    {
                        PtButtonForm pf = new PtButtonForm();
                        if (tsItem is CustomFigureToolButton)
                            pf.ToolButtonData = ((CustomFigureToolButton)tsItem).CustomFigureT;
                        else if (tsItem is RunCmdToolButton)
                            pf.ToolButtonData = ((RunCmdToolButton)tsItem).RunCmdT;
                        else if (tsItem is ShowDirToolButton)
                            pf.ToolButtonData = ((ShowDirToolButton)tsItem).ShowDirT;
                        else if (tsItem is WebLinkToolButton)
                            pf.ToolButtonData = ((WebLinkToolButton)tsItem).WebLinkT;
                        if (pf.ShowDialog() == DialogResult.OK)
                        {
                            ToolStripItem newTsItem = null;
                            if (pf.ToolButtonData is CustomFigureT)
                                newTsItem = new CustomFigureToolButton((CustomFigureT)pf.ToolButtonData);
                            else if (pf.ToolButtonData is RunCmdT)
                                newTsItem = new RunCmdToolButton((RunCmdT)pf.ToolButtonData);
                            else if (pf.ToolButtonData is ShowDirT)
                                newTsItem = new ShowDirToolButton((ShowDirT)pf.ToolButtonData);
                            else if (pf.ToolButtonData is WebLinkT)
                                newTsItem = new WebLinkToolButton((WebLinkT)pf.ToolButtonData);
                            Items.Insert(Items.IndexOf(tsItem), newTsItem);
                            Items.Remove(tsItem);
                        }
                    };
                    item = menu.Items.Add("Delete");
                    item.Click += delegate(object s, EventArgs e2)
                    {
                        Items.Remove(tsItem);
                    };
                    menu.Show(tsItem.Owner, tsItem.Bounds.Left + e.X, tsItem.Bounds.Top + e.Y);
                }
            }
        }

        public void Clear()
        {
            for (int i = Items.Count - 1; i > 0; i--)
                Items.RemoveAt(i);
        }

        public void AddCustomFigure(CustomFigureT customFigure)
        {
            Items.Add(new CustomFigureToolButton(customFigure));
        }
    }
}
