using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WICR_Estimator
{
    public class DelegateCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        //public event EventHandler CanExecuteChanged;
        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        
        public bool CanExecute(object parameters)
        {
            return _canExecute == null ? true : _canExecute(parameters);
            
        }

        public void RaiseCanExecuteChanged()
        {
            //if (CanExecuteChanged != null)
            //{
            //    CanExecuteChanged(this, EventArgs.Empty);
            //}
            CommandManager.InvalidateRequerySuggested();
        }
       
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { 

                CommandManager.RequerySuggested -= value;
               

            }
        }

        public void Execute(object parameters)
        {
            _execute(parameters);
        }
        
        #endregion // ICommand Members
    }
}

  
