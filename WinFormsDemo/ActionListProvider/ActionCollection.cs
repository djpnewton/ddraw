using System;
using System.Collections;
using System.Text;

namespace Burkovsky.Controls
{
    public class ActionAlreadyExistsException : ArgumentException { }

    public class ActionCollection : CollectionBase
    {
        private ActionListProvider FProvider;

        internal ActionListProvider Provider
        {
            get { return FProvider; }
            set { FProvider = value; }
        }

        public Action Add(Action act)
        {
            this.InnerList.Add(act);
            act.Provider = FProvider;

            return act;
        }

        public void Remove(Action Act)
        {
            InnerList.Remove(Act);
        }

        public Action this[Int32 i]
        {
            get { return (Action)this.InnerList[i]; }
        }
    }
}
