using System;

namespace ForRobot.Libr.AttachedProperties
{
    public class AttemptSelectedCommand : RelayCommand
    {
        private readonly Func<object, bool> _shouldBlock;

        public bool CommandWasBlock { get; private set; }

        public AttemptSelectedCommand(Action<object> execute,
                                     Func<object, bool> canExecute = null,
                                     Func<object, bool> shouldBlock = null) : base(execute, canExecute ?? (_ => true))
        {
            _shouldBlock = shouldBlock ?? (_ => false);
        }

        public override void Execute(object parameter)
        {
            if (this.ShouldBlock(parameter))
                return;

            base.Execute(parameter);
        }

        public bool ShouldBlock(object parameter)
        {
            bool shouldBlock = _shouldBlock(parameter);
            CommandWasBlock = shouldBlock;
            return shouldBlock;
        }
    }
}
