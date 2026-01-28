using System;
using ForRobot.Libr.Collections;

namespace ForRobot.Libr.Clipboard.UndoRedo
{
    public class UndoRedoStacks
    {
        public const int MAX_STACK_SIZE = 50;

        public LimitedStack<IUndoableCommand> UndoStack { get; } = new LimitedStack<IUndoableCommand>(MAX_STACK_SIZE);
        public LimitedStack<IUndoableCommand> RedoStack { get; } = new LimitedStack<IUndoableCommand>(MAX_STACK_SIZE);
    }
}
