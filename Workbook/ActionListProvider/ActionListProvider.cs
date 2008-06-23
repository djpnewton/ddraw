using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

namespace Burkovsky.Controls
{
    [ProvideProperty("Action", typeof(Component))]
    public partial class ActionListProvider : Component, IExtenderProvider, ISupportInitialize
    {
        private Dictionary<Component, Action> ActionDictionary = new Dictionary<Component, Action>();
        private ActionCollection FActions = new ActionCollection();

        #region Constructor
        public ActionListProvider()
        {
            InitializeComponent();
            FActions.Provider = this;
        }

        public ActionListProvider(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            FActions.Provider = this;
        }
        #endregion

        #region IExtenderProvider Members

        public bool CanExtend(object extendee)
        {
            return (extendee is ButtonBase) || (extendee is ToolStripItem);
        }

        #endregion

        [Description("Select an action")]
        [Category("Behaviour")]
        public Action GetAction(Component cmp)
        {
            if (ActionDictionary.ContainsKey(cmp))
                return ActionDictionary[cmp];
            else
                return null;
        }

        public void SetAction(Component cmp, Action act)
        {
            ActionDictionary.Remove(cmp);
            if (act != null)
            {
                ActionDictionary.Add(cmp, act);
                UpdateEnabled(act);
                UpdateText(act);
                AddExecuteTarget(act);
            }
        }

        [Category("Actions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(ActionCollectionEditor), typeof(UITypeEditor))]
        public ActionCollection Actions
        {
            get { return FActions; }
        }

        internal void UpdateEnabled(Action Act)
        {
            foreach (KeyValuePair<Component, Action> ActionLink in ActionDictionary)
            {
                if (ActionLink.Value == Act)
                {
                    if (ActionLink.Key is ButtonBase)
                        ((ButtonBase)ActionLink.Key).Enabled = Act.Enabled;
                    if (ActionLink.Key is ToolStripItem)
                        ((ToolStripItem)ActionLink.Key).Enabled = Act.Enabled;
                }
            }
        }

        internal void UpdateText(Action Act)
        {
            foreach (KeyValuePair<Component, Action> ActionLink in ActionDictionary)
            {
                if (ActionLink.Value == Act)
                {
                    if (ActionLink.Key is ButtonBase)
                        ((ButtonBase)ActionLink.Key).Text = Act.Text;
                    if (ActionLink.Key is ToolStripItem)
                        ((ToolStripItem)ActionLink.Key).Text = Act.Text;
                }
            }
        }

        internal void UpdateShortcutKeys(Action Act)
        {
            foreach (KeyValuePair<Component, Action> ActionLink in ActionDictionary)
            {
                if (ActionLink.Value == Act)
                {
                    if (ActionLink.Key is ToolStripMenuItem)
                        ((ToolStripMenuItem)ActionLink.Key).ShortcutKeys = Act.ShortcutKeys;
                }
            }
        }

        internal void AddExecuteTarget(Action Act)
        {
            if (Act.ExecuteHandlers != null)
            {
                RemoveExecuteTarget(Act);
                EventHandler target = (EventHandler)Act.ExecuteHandlers.GetInvocationList()[0];
                foreach (KeyValuePair<Component, Action> ActionLink in ActionDictionary)
                {
                    if (ActionLink.Value == Act)
                    {
                        if (ActionLink.Key is ButtonBase)
                            ((ButtonBase)ActionLink.Key).Click += target;
                        if (ActionLink.Key is ToolStripItem)
                            ((ToolStripItem)ActionLink.Key).Click += target;
                    }
                }
            }
        }

        internal void RemoveExecuteTarget(Action Act)
        {
            if (Act.ExecuteHandlers != null)
            {
                EventHandler target = (EventHandler)Act.ExecuteHandlers.GetInvocationList()[0];
                foreach (KeyValuePair<Component, Action> ActionLink in ActionDictionary)
                {
                    if (ActionLink.Value == Act)
                    {
                        if (ActionLink.Key is ButtonBase)
                            ((ButtonBase)ActionLink.Key).Click -= target;
                        if (ActionLink.Key is ToolStripItem)
                            ((ToolStripItem)ActionLink.Key).Click -= target;
                    }
                }
            }
        }


        #region ISupportInitialize Members

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            foreach (Action act in Actions)
            {
                UpdateEnabled(act);
                UpdateText(act);
                UpdateShortcutKeys(act);
                AddExecuteTarget(act);
            }
        }

        #endregion
    }
}