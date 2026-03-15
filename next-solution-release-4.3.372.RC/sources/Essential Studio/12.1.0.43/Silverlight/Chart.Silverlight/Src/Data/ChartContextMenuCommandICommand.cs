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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartContextMenucommand
    /// </summary>
    public class ChartContextMenuCommand : ICommand
    {

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public bool CanExecute(object parameter)
        {
            return true;
        }
#pragma warning disable 0067
        /// <summary>
        /// Event for CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Class implementation for ContextMenuItemCollection
    /// </summary>
    public class ContextMenuItemsCollection : ObservableCollection<ContextMenuItemAdv>
    {
    }
}
