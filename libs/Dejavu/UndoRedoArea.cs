// This source is under LGPL license. Sergei Arhipenko (c) 2006-2007. email: sbs-arhipenko@yandex.ru. This notice may not be removed.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DejaVu
{
	/// <summary>
	/// Provides undo/redo commands with isolation. 
	/// Changes made during a command are isolated inside the given area.
	/// </summary>
	/// <remarks>
	/// Developer is responsinle to guarantie that every data instance is changed always in same area.
	/// Data consistency can be unpredictably tampered if an instance is changed in one area but then changed in another one.
	/// </remarks>
	[DebuggerDisplay("{Name}")]
	public class UndoRedoArea
	{
		/// <summary>This field serves primarily for debugging purposes</summary>
		public readonly string Name;
		/// <summary>
		/// Initializes new area
		/// </summary>
		/// <param name="name">Name of the area (for debugging and tracing purposes)</param>
		public UndoRedoArea(string name)
		{
			Name = name;
		}
		[ThreadStatic]
		private static UndoRedoArea currentArea = null;
		internal static UndoRedoArea CurrentArea
		{
			get { return currentArea; }
		}

		private List<Command> history = new List<Command>();
		private int currentPosition = -1;

		private Command currentCommand = null;
		internal Command CurrentCommand
		{
			get { return currentCommand; }
		}

		#region Undo/Redo stuff
		/// <summary>Returns true if history has command that can be undone</summary>
		public bool CanUndo
		{
			get { return currentPosition >= 0; }
		}
		/// <summary>Returns true if history has command that can be redone</summary>
		public bool CanRedo
		{
			get { return currentPosition < history.Count - 1; }
		}
		/// <summary>Undo last command from history list</summary>
		public void Undo()
		{
			AssertNoCommand();
			if (CanUndo)
			{
				Command command = history[currentPosition--];
				command.Undo();
				OnCommandDone(CommandDoneType.Undo);
			}
		}
		/// <summary>Repeats command that was undone before</summary>
		public void Redo()
		{
			AssertNoCommand();
			if (CanRedo)
			{
				Command command = history[++currentPosition];
				command.Redo();
				OnCommandDone(CommandDoneType.Redo);
			}
		}
		#endregion

		/// <summary>Start command. Any data changes must be done within a command.</summary>
		/// <param name="commandCaption"></param>
		/// <returns>Interface that allows properly finish the command with 'using' statement</returns>
		public IDisposable Start(string commandCaption)
		{
			AssertNoCommand();
			currentArea = this;
			currentCommand = new Command(commandCaption, this);
			return currentCommand;
		}
		/// <summary>Commits current command and saves changes into history</summary>
        public void Commit()
        {
            AssertCurrentCommand();
            if (currentCommand.HasChanges)
            {
                currentCommand.Commit();

                // add command to history (all redo records will be removed)
                int count = history.Count - currentPosition - 1;
                history.RemoveRange(currentPosition + 1, count);

                history.Add(currentCommand);
                currentPosition++;
                TruncateHistory();

                OnCommandDone(CommandDoneType.Commit);
            }
            currentCommand = null;
        }
		/// <summary>
		/// Rollback current command. It does not saves any changes done in current command.
		/// </summary>
		public void Cancel()
		{
			AssertCurrentCommand();
			currentCommand.Undo();
			currentCommand = null;
		}

		/// <summary>
		/// Clears all history. It does not affect current data but history only. 
		/// It is usefull after any data initialization if you want forbid user to undo this initialization.
		/// </summary>
		public void ClearHistory()
		{
			currentCommand = null;
			currentPosition = -1;
			history.Clear();
		}

        /// <summary>
        /// Clears all redo history. It does not affect current data but redos only. 
        /// </summary>
        public void ClearRedos()
        {
            currentCommand = null;
            int count = history.Count - currentPosition - 1;
            history.RemoveRange(currentPosition + 1, count);
        }

		/// <summary>Checks that there is no command started in current thread</summary>
		internal void AssertNoCommand()
		{
			// check command in this area
			if (currentCommand != null)
				throw new InvalidOperationException("Previous command is not completed. Use UndoRedoManager.Commit() to complete current command.");
			// check command in area that is current now 
			if (currentArea != null && currentArea.currentCommand != null)
				throw new InvalidOperationException("A command of another area has already started in current thread.");
		}

		/// <summary>Checks that command had been started</summary>
		internal static void AssertCommand()
		{
			if (currentArea == null || currentArea.currentCommand == null)
				throw new InvalidOperationException("Command is not started.");
		}
		/// <summary>Checks that command had been started in given area</summary>
		internal void AssertCurrentCommand()
		{
			if (currentCommand == null)
				throw new InvalidOperationException("Command in given area is not started.");
		}

		public bool IsCommandStarted
		{
			get { return currentCommand != null; }
		}

		#region Commands Lists
		/// <summary>Gets an enumeration of commands captions that can be undone.</summary>
		/// <remarks>The first command in the enumeration will be undone first</remarks>
		public IEnumerable<CommandId> UndoCommands
		{
			get
			{
				for (int i = currentPosition; i >= 0; i--)
					yield return history[i].CommandId;
			}
		}
		/// <summary>Gets an enumeration of commands captions that can be redone.</summary>
		/// <remarks>The first command in the enumeration will be redone first</remarks>
        public IEnumerable<CommandId> RedoCommands
		{
			get
			{
				for (int i = currentPosition + 1; i < history.Count; i++)
                    yield return history[i].CommandId;
			}
		}
		#endregion

		#region History Size

		private int maxHistorySize = 0;

		/// <summary>
		/// Gets/sets max commands stored in history. 
		/// Zero value (default) sets unlimited history size.
		/// </summary>
		public int MaxHistorySize
		{
			get { return maxHistorySize; }
			set
			{
				if (IsCommandStarted)
					throw new InvalidOperationException("Max size may not be set while command is run.");
				if (value < 0)
					throw new ArgumentOutOfRangeException("Value may not be less than 0");
				maxHistorySize = value;
				TruncateHistory();
			}
		}

		private void TruncateHistory()
		{
			if (maxHistorySize > 0)
				if (history.Count > maxHistorySize)
				{
					int count = history.Count - maxHistorySize;
					history.RemoveRange(0, count);
					currentPosition -= count;
				}
		}
		#endregion

		public event EventHandler<CommandDoneEventArgs> CommandDone;
		void OnCommandDone(CommandDoneType type)
		{
			if (CommandDone != null)
				CommandDone(null, new CommandDoneEventArgs(type));
		}
	}
}
