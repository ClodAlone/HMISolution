#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _command;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> method)
            : this(method, null)
        {
        }

        public DelegateCommand(Action<object> command, Predicate<object> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _command.Invoke(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void ExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }

    public class CommandBase : ICommand
    {
        public SpreadsheetControl AssociatedSpreadsheet;

        internal CommandBase(SpreadsheetControl spreadsheetControl)
        {
            AssociatedSpreadsheet = spreadsheetControl;
        }


        public bool CanExecute(object parameter)
        {
            return CanExcuteCommand(parameter);
        }

        public virtual bool CanExcuteCommand(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void ExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            ExecuteCommand(parameter);
        }

        protected virtual void ExecuteCommand(object parameter)
        {

        }
    }
}
