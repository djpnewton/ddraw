using System;
using System.Collections.Generic;
using System.Text;

using DejaVu;
using DejaVu.Collections.Generic;

namespace DDraw
{
    public class DEngineManager
    {
        UndoRedoArea undoRedoArea = new UndoRedoArea("DEngineManager");
        UndoRedoList<DEngine> engines = new UndoRedoList<DEngine>();

        public int EngineCount
        {
            get { return engines.Count; }
        }
        public event EventHandler UndoRedoChanged;

        public bool Dirty
        {
            set
            {
                if (value == false)
                {
                    foreach (DEngine de in engines)
                        de.UndoRedoClearHistory();
                    undoRedoArea.ClearHistory();
                    // no more undo/redo history
                    DoUndoRedoChanged(this, new EventArgs());
                }
            }
            get
            {
                foreach (DEngine de in engines)
                    if (de.CanUndo)
                        return true;
                if (undoRedoArea.CanUndo)
                    return true;
                return false;
            }
        }

        UndoRedo<BackgroundFigure> _backgroundFigure = new UndoRedo<BackgroundFigure>();
        public BackgroundFigure BackgroundFigure
        {
            get { return _backgroundFigure.Value; }
            set
            {
                if (_backgroundFigure.Value != value)
                    _backgroundFigure.Value = value;
            }
        }

        public DEngineManager()
        {
            undoRedoArea.CommandDone += new EventHandler<CommandDoneEventArgs>(undoRedoArea_CommandDone);
        }

        void undoRedoArea_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (e.CommandDoneType == CommandDoneType.Commit)
                // clear all redos for engines
                foreach (DEngine de in engines)
                    de.UndoRedoClearRedos();
            DoUndoRedoChanged(this, e);
        }

        void de_UndoRedoCommandDone(object sender, CommandDoneEventArgs e)
        {
            if (e.CommandDoneType == CommandDoneType.Commit)
                // clear all redos for undoRedoArea
                undoRedoArea.ClearRedos();
            DoUndoRedoChanged(sender, e);
        }

        void DoUndoRedoChanged(object sender, EventArgs e)
        {
            if (UndoRedoChanged != null)
                UndoRedoChanged(sender, e);
        }

        public void AddEngine(DEngine de)
        {
            InsertEngine(engines.Count, de);
        }

        public void InsertEngine(int idx, DEngine de)
        {
            de.UndoRedoCommandDone += new EventHandler<CommandDoneEventArgs>(de_UndoRedoCommandDone);
            engines.Insert(idx, de);
        }

        public int IndexOfEngine(DEngine de)
        {
            return engines.IndexOf(de);
        }

        public void RemoveEngine(DEngine de)
        {
            engines.Remove(de);
            de.UndoRedoCommandDone -= undoRedoArea_CommandDone;
        }

        public void MoveEngine(DEngine de, DEngine to)
        {
            int idx = engines.IndexOf(to);
            engines.Remove(de);
            engines.Insert(idx, de);
        }

        public void Clear()
        {
            engines.Clear();
            BackgroundFigure = new BackgroundFigure();
        }

        public DEngine GetEngine(int idx)
        {
            return engines[idx];
        }

        public List<DEngine> GetEngines()
        {
            return new List<DEngine>(engines.ToArray());
        }

        public void SetEngines(List<DEngine> engines)
        {
            this.engines.Clear();
            foreach (DEngine de in engines)
                AddEngine(de);
        }

        public void UndoRedoStart(string name)
        {
            undoRedoArea.Start(name);
        }
        public void UndoRedoCommit()
        {
            undoRedoArea.Commit();
        }
        public void UndoRedoCancel()
        {
            undoRedoArea.Cancel();
        }
        public void UndoRedoClearHistory()
        {
            undoRedoArea.ClearHistory();
        }

        public bool CanUndo(DEngine de)
        {
            return (de != null && de.CanUndo) || undoRedoArea.CanUndo;
        }

        public bool CanRedo(DEngine de)
        {
            return (de != null && de.CanRedo) || undoRedoArea.CanRedo;
        }

        CommandId GetCommand(IEnumerator<CommandId> en)
        {
            CommandId commandId = new CommandId(-1, null);
            if (en.MoveNext())
                commandId = en.Current;
            return commandId;
        }

        void FindMostRecentCommand(IEnumerator<CommandId> idA, out bool aMostRecent,
            IEnumerator<CommandId> idB, out bool bMostRecent, out string mostRecentCaption)
        {
            CommandId aCommandId = GetCommand(idA);
            CommandId bCommandId = GetCommand(idB);
            aMostRecent = false;
            bMostRecent = false;
            if (aCommandId.Id > bCommandId.Id)
            {
                aMostRecent = true;
                mostRecentCaption = aCommandId.Caption;
            }
            else
            {
                bMostRecent = true;
                mostRecentCaption = bCommandId.Caption;
            }
        }

        void FindEarlyestCommand(IEnumerator<CommandId> idA, out bool aEarlyest,
            IEnumerator<CommandId> idB, out bool bEarlyest, out string earlyestCaption)
        {
            CommandId aCommandId = GetCommand(idA);
            if (aCommandId.Id == -1)
                aCommandId.Id = int.MaxValue;
            CommandId bCommandId = GetCommand(idB);
            if (bCommandId.Id == -1)
                bCommandId.Id = int.MaxValue;
            aEarlyest = false;
            bEarlyest = false;
            if (aCommandId.Id < bCommandId.Id)
            {
                aEarlyest = true;
                earlyestCaption = aCommandId.Caption;
            }
            else
            {
                bEarlyest = true;
                earlyestCaption = bCommandId.Caption;
            }
            if (earlyestCaption == null)
                System.Diagnostics.Debug.Assert(false);
        }

        public string UndoCaption(DEngine de)
        {
            if (de != null)
            {
                bool a, b;
                string caption;
                FindMostRecentCommand(de.UndoCommands.GetEnumerator(), out a, undoRedoArea.UndoCommands.GetEnumerator(), out b, out caption);
                return caption;
            }
            else
                return GetCommand(undoRedoArea.UndoCommands.GetEnumerator()).Caption;
        }

        public string RedoCaption(DEngine de)
        {
            if (de != null)
            {
                bool a, b;
                string caption;
                FindEarlyestCommand(de.RedoCommands.GetEnumerator(), out a, undoRedoArea.RedoCommands.GetEnumerator(), out b, out caption);
                return caption;
            }
            else
                return GetCommand(undoRedoArea.RedoCommands.GetEnumerator()).Caption;
        }

        public void Undo(DEngine de)
        {
            bool deUndo, thisUndo;
            string caption;
            FindMostRecentCommand(de.UndoCommands.GetEnumerator(), out deUndo, undoRedoArea.UndoCommands.GetEnumerator(), out thisUndo, out caption);
            if (deUndo)
                de.Undo();
            else
                undoRedoArea.Undo();
        }

        public void Redo(DEngine de)
        {
            bool deRedo, thisRedo;
            string caption;
            FindEarlyestCommand(de.RedoCommands.GetEnumerator(), out deRedo, undoRedoArea.RedoCommands.GetEnumerator(), out thisRedo, out caption);
            if (deRedo)
                de.Redo();
            else
                undoRedoArea.Redo();
        }
    }
}
