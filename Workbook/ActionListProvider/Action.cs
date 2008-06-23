using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Burkovsky.Controls
{
    [DefaultEventAttribute("Execute")]
    [ToolboxItem(false)]
    public class Action : Component
    {
        internal ActionListProvider Provider = null;
        private bool FEnabled = true;
        private string FText = "";
        private Keys FShortcutKeys;

        public Action()
        {
            Disposed += new EventHandler(this.OnDisposed);
        }

        private void OnDisposed(Object sender, EventArgs e)
        {
            if (Provider != null)
            {
                Provider.Actions.Remove(this);
            }
        }

        [Category("Action")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return FEnabled; }
            set
            {
                FEnabled = value;
                if (Provider != null)
                    Provider.UpdateEnabled(this);
            }
        }

        [Category("Action")]
        public string Text
        {
            get { return FText; }
            set
            {
                FText = value;
                if (Provider != null)
                    Provider.UpdateText(this);
            }
        }

        [Category("Action")]
        public Keys ShortcutKeys
        {
            get { return FShortcutKeys; }
            set
            {
                FShortcutKeys = value;
                if (Provider != null)
                    Provider.UpdateShortcutKeys(this);
            }
        }

        internal EventHandler ExecuteHandlers;
        public event EventHandler Execute
        {
            add
            {
                ExecuteHandlers += value;
                if (Provider != null)
                    Provider.AddExecuteTarget(this);
            }
            remove
            {
                ExecuteHandlers -= value;
                if (Provider != null)
                    Provider.AddExecuteTarget(this);
            }

        }
    }
}