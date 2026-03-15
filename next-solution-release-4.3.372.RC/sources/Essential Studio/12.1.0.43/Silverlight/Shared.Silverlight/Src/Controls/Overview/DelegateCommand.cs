#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    //internal class DelegateCommand : ICommand
    //{
    //    public event EventHandler CanExecuteChanged;

    //    Func<object, bool> canExecute;
    //    Action<object> executeAction;
    //    bool canExecuteCache;

    //    public DelegateCommand(Action<object> executeAction,
    //                           Func<object, bool> canExecute)
    //    {
    //        this.executeAction = executeAction;
    //        this.canExecute = canExecute;
    //    }

    //    #region ICommand Members

    //    public bool CanExecute(object parameter)
    //    {
    //        bool tempCanExecute = canExecute(parameter);

    //        if (canExecuteCache != tempCanExecute)
    //        {
    //            canExecuteCache = tempCanExecute;
    //            if (CanExecuteChanged != null)
    //            {
    //                CanExecuteChanged(this, new EventArgs());
    //            }
    //        }

    //        return canExecuteCache;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        executeAction(parameter);
    //    }

    //    #endregion

    //}
}
