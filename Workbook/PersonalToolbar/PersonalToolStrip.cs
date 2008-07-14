using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using DDraw;

namespace Workbook.PersonalToolbar
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
                    foreach (PersonalTool pt in pf.ToolItems)
                    {
                        if (pt is CustomFigureTool)
                            Items.Add(new CustomFigureToolButton((CustomFigureTool)pt));
                        else if (pt is RunCmdTool)
                            Items.Add(new RunCmdToolButton((RunCmdTool)pt));
                        else if (pt is ShowDirTool)
                            Items.Add(new ShowDirToolButton((ShowDirTool)pt));
                        else if (pt is WebLinkTool)
                            Items.Add(new WebLinkToolButton((WebLinkTool)pt));
                        else if (pt is ModeSelectTool)
                            Items.Add(new ModeSelectToolButton((ModeSelectTool)pt));
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
            else if (e.ClickedItem is ModeSelectToolButton)
            {
                ModeSelectToolButton b = (ModeSelectToolButton)e.ClickedItem;
                System.Diagnostics.Debug.Assert(de != null, "ERROR: \"de\" is not assigned");
                if (b.ModeSelectType == ModeSelectType.Select)
                    de.HsmState = DHsmState.Select;
                else if (b.ModeSelectType == ModeSelectType.Eraser)
                    de.HsmState = DHsmState.Eraser;
            }
            base.OnItemClicked(e);
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);
            e.Item.MouseDown += new MouseEventHandler(Item_MouseDown);
        }

        public event EventHandler ItemContext;

        void Item_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && sender != btnCustomize)
            {
                if (ItemContext != null)
                    ItemContext(sender, new EventArgs());
                ToolStripItem tsItem = (ToolStripItem)sender;
                if (tsItem.Owner != null)
                {
                    // make context menu
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripItem item = menu.Items.Add("Properties");
                    item.Click += delegate(object s, EventArgs e2)
                    {
                        PtButtonForm pf = new PtButtonForm();
                        pf.SetupToolButtonEdit();
                        if (tsItem is CustomFigureToolButton)
                            pf.PersonalTool = ((CustomFigureToolButton)tsItem).CustomFigure;
                        else if (tsItem is RunCmdToolButton)
                            pf.PersonalTool = ((RunCmdToolButton)tsItem).RunCmd;
                        else if (tsItem is ShowDirToolButton)
                            pf.PersonalTool = ((ShowDirToolButton)tsItem).ShowDir;
                        else if (tsItem is WebLinkToolButton)
                            pf.PersonalTool = ((WebLinkToolButton)tsItem).WebLink;
                        else if (tsItem is ModeSelectToolButton)
                            pf.PersonalTool = ((ModeSelectToolButton)tsItem).ModeSelect;
                        if (pf.ShowDialog() == DialogResult.OK)
                        {
                            ToolStripItem newTsItem = null;
                            if (pf.PersonalTool is CustomFigureTool)
                                newTsItem = new CustomFigureToolButton((CustomFigureTool)pf.PersonalTool);
                            else if (pf.PersonalTool is RunCmdTool)
                                newTsItem = new RunCmdToolButton((RunCmdTool)pf.PersonalTool);
                            else if (pf.PersonalTool is ShowDirTool)
                                newTsItem = new ShowDirToolButton((ShowDirTool)pf.PersonalTool);
                            else if (pf.PersonalTool is WebLinkTool)
                                newTsItem = new WebLinkToolButton((WebLinkTool)pf.PersonalTool);
                            else if (pf.PersonalTool is ModeSelectTool)
                                newTsItem = new ModeSelectToolButton((ModeSelectTool)pf.PersonalTool);
                            Items.Insert(Items.IndexOf(tsItem), newTsItem);
                            Items.Remove(tsItem);
                            // click it
                            if (newTsItem is CustomFigureToolButton)
                                newTsItem.PerformClick();
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

        public void AddCustomFigure(CustomFigureTool customFigure)
        {
            Items.Add(new CustomFigureToolButton(customFigure));
        }
    }
}
