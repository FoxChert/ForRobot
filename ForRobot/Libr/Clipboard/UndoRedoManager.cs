using System;
//using System.Collections.Specialized;
using ForRobot.Libr.Clipboard.UndoRedo;

namespace ForRobot.Libr.Clipboard
{
    public class UndoRedoManager : IUndoRedo
    {
        #region Private variables

        private readonly CacheClipboardProvider _clipboardProvider;
        private readonly string _fileKey;
        private readonly UndoRedoStacks _stacks;

        #endregion Privae variables

        #region Public variables
        
        public bool CanUndo => this._stacks.UndoStack.Count > 0;
        public bool CanRedo => this._stacks.RedoStack.Count > 0;

        public event EventHandler UndoRedoStateChanged;
        //public event NotifyCollectionChangedEventHandler UndoRedoStateChanged;

        #endregion Public variables

        public UndoRedoManager(CacheClipboardProvider clipboardProvider, string fileKey)
        {
            this._clipboardProvider = clipboardProvider;
            this._fileKey = fileKey;
            this._stacks = _clipboardProvider.GetOrAddStacks(_fileKey);
        }

        #region Public functions
        
        public void Undo()
        {
            if (!this.CanUndo) return;

            var command = _stacks.UndoStack.Pop();
            command.Unexecute();
            this._stacks.RedoStack.Push(command);
            this.OnUndoRedoStateChanged();
        }

        public void Redo()
        {
            if (!this.CanRedo) return;

            var command = _stacks.RedoStack.Pop();
            command.Execute();
            this._stacks.UndoStack.Push(command);
            this.OnUndoRedoStateChanged();
        }

        public void AddUndoCommand(IUndoableCommand command)
        {
            this._stacks.UndoStack.Push(command);
            this._stacks.RedoStack.Clear();
            this.OnUndoRedoStateChanged();
        }

        public void ClearUndoRedoHistory()
        {
            this._stacks.UndoStack.Clear();
            this._stacks.RedoStack.Clear();
            this.OnUndoRedoStateChanged();
        }

        protected virtual void OnUndoRedoStateChanged() => this.UndoRedoStateChanged?.Invoke(this, EventArgs.Empty);

        #endregion Public functions
    }
}
