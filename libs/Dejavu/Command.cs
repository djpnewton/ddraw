// This source is under LGPL license. Sergei Arhipenko (c) 2006-2007. email: sbs-arhipenko@yandex.ru. This notice may not be removed.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DejaVu
{
    public struct CommandId
    {
        public int Id;
        public string Caption;
        public CommandId(int id, string caption)
        {
            Id = id;
            Caption = caption;
        }
    }

    class Command : IDisposable
    {
		readonly UndoRedoArea parentArea;
        public readonly CommandId CommandId;

        static int _currentId = 0;

		public Command(string caption, UndoRedoArea parentArea)
        {
            CommandId = new CommandId(_currentId++, caption);
			this.parentArea = parentArea;
        }

		Dictionary<IUndoRedoMember, object> changes = new Dictionary<IUndoRedoMember, object>();
		public bool IsEnlisted(IUndoRedoMember member)
		{
			// if command suspended, it will always return true to prevent changes registration
			return changes.ContainsKey(member);
		}

		public object this[IUndoRedoMember member]
		{
			get 
			{
				return changes[member];
			}
			set 
			{
				changes[member] = value;
			}
			
		}

        public bool HasChanges
        {
            get { return changes.Count > 0; }
        }

		internal void Commit()
		{
			foreach (IUndoRedoMember member in changes.Keys)
				member.OnCommit(changes[member]);
		}
		internal void Undo()
		{
			foreach (IUndoRedoMember member in changes.Keys)
				member.OnUndo(changes[member]);
		}
		internal void Redo()
		{
			foreach (IUndoRedoMember member in changes.Keys)
				member.OnRedo(changes[member]);
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (parentArea.IsCommandStarted)
			{
				Debug.Assert(parentArea.CurrentCommand == this, "Another command had been started within disposed command");
				parentArea.Cancel();
			}
		}

		#endregion
	}
}
