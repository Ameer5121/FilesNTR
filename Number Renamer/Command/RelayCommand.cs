using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Number_Renamer.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;

        private readonly Func<bool> canExecute;
        public RelayCommand(Action<object> execute) : this(execute, canExecute: null) // Sometimes you don't need canExecute method. So you can create a command without it like; SomeCommand = new RelayCommand(UpdateName);
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
