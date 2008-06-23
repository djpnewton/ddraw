using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Burkovsky.Controls
{
    class ActionCollectionEditor: CollectionEditor
    {        
        public ActionCollectionEditor(Type type): base(type) {}

        protected override object SetItems(object editValue, object[] value)
        {
            object Result;
            ActionCollection Collection = (ActionCollection)editValue;
            Result = base.SetItems(editValue, value);

            foreach (Action act in value)
            {
                act.Provider = Collection.Provider;
            }

            return Result;
        }
    }
}
