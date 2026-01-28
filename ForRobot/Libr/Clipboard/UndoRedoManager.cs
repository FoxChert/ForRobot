using System;

using ForRobot.Libr.Clipboard.UndoRedo;
using ForRobot.Libr.Collections;

namespace ForRobot.Libr.Clipboard
{
    public class UndoRedoManager : IUndoRedo
    {
        #region Private variables

        private readonly CacheClipboardProvider _clipboardProvider;
        private readonly string _cacheKey;
        private readonly UndoRedoStacks _stacks;
        private readonly object _syncRoot = new object();

        #endregion Private variables

        #region Public variables

        /// <summary>
        /// Можно ли отменить изменения
        /// </summary>
        public bool CanUndo
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._stacks.UndoStack.Count > 0;
                }
            }
        }
        /// <summary>
        /// Можно ли вернуть изменения
        /// </summary>
        public bool CanRedo
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._stacks.RedoStack.Count > 0;
                }
            }
        }

        /// <summary>
        /// Событие изменения состояний стеков
        /// </summary>
        public event EventHandler UndoRedoStateChanged;

        #endregion Public variables

        public UndoRedoManager(CacheClipboardProvider clipboardProvider, string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey)) throw new ArgumentException("Key cannot be null or empty", nameof(cacheKey));

            this._clipboardProvider = clipboardProvider ?? throw new ArgumentNullException(nameof(clipboardProvider));
            this._cacheKey = cacheKey;
            this._stacks = _clipboardProvider.GetOrAddStacks(_cacheKey);
        }

        private void ExecuteCommand(LimitedStack<IUndoableCommand> fromStack, LimitedStack<IUndoableCommand> toStack, Action<IUndoableCommand> action)
        {
            lock (this._syncRoot)
            {
                if (fromStack.Count == 0) return;

                var command = fromStack.Pop();
                action(command);
                toStack.Push(command);
                this.OnUndoRedoStateChanged();
            }
        }

        #region Public functions

        public void Undo() => ExecuteCommand(this._stacks.UndoStack, this._stacks.RedoStack, cmd => cmd.Unexecute());
        public void Redo() => ExecuteCommand(this._stacks.RedoStack, this._stacks.UndoStack, cmd => cmd.Execute());

        public void AddUndoCommand(IUndoableCommand command)
        {
            lock (this._syncRoot)
            {
                this._stacks.UndoStack.Push(command);
                this._stacks.RedoStack.Clear();
                this.OnUndoRedoStateChanged();
            }
        }

        /// <summary>
        /// Очистка стеков
        /// </summary>
        public void ClearUndoRedoHistory()
        {
            lock (this._syncRoot)
            {
                this._stacks.UndoStack.Clear();
                this._stacks.RedoStack.Clear();
                this.OnUndoRedoStateChanged();
            }
        }

        protected virtual void OnUndoRedoStateChanged() => this.UndoRedoStateChanged?.Invoke(this, EventArgs.Empty);

        #endregion Public functions
    }
}
