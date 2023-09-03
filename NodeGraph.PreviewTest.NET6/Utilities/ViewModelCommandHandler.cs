using Livet.Commands;
using System;

namespace NodeGraph.PreviewTest.NET6.Utilities
{
    public class ViewModelCommandHandler
    {
        public ViewModelCommand Get(Action execute, Func<bool> canExecute = null!)
        {
            if (_Command == null)
            {
                _Command = new ViewModelCommand(execute, canExecute);
            }

            return _Command;
        }

        ViewModelCommand? _Command;
    }

    public class ViewModelCommandHandler<T>
    {
        public ListenerCommand<T> Get(Action<T> execute, Func<bool> canExecute = null!)
        {
            if (_Command == null)
            {
                _Command = new ListenerCommand<T>(execute);
            }

            return _Command;
        }

        ListenerCommand<T>? _Command;
    }
}
