#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Gantt
{
#if !SILVERLIGHT

    //public class DelegateCommand : ICommand
    //{
    //    private Predicate<object> _canExecute;
    //    private Action<object> _method;
    //    public event EventHandler CanExecuteChanged;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    //    /// </summary>
    //    /// <param name="method">The method.</param>
    //    public DelegateCommand(Action<object> method)
    //        : this(method, null)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    //    /// </summary>
    //    /// <param name="method">The method.</param>
    //    /// <param name="canExecute">The can execute.</param>
    //    public DelegateCommand(Action<object> method, Predicate<object> canExecute)
    //    {
    //        _method = method;
    //        _canExecute = canExecute;
    //    }

    //    /// <summary>
    //    /// Defines the method that determines whether the command can execute in its current state.
    //    /// </summary>
    //    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    //    /// <returns>
    //    /// true if this command can be executed; otherwise, false.
    //    /// </returns>
    //    public bool CanExecute(object parameter)
    //    {
    //        if (_canExecute == null)
    //        {
    //            return true;
    //        }

    //        return _canExecute(parameter);
    //    }

    //    /// <summary>
    //    /// Defines the method to be called when the command is invoked.
    //    /// </summary>
    //    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    //    public void Execute(object parameter)
    //    {
    //        _method.Invoke(parameter);
    //    }
    //}
#endif
}
