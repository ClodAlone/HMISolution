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

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotSchemaDesigner
#else
namespace Syncfusion.Silverlight.Controls.PivotSchemaDesigner
#endif
{
    /// <summary>
    /// Represents the class for PivotSchemaDesigner to define its operations through routed UI commands.
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#if !SILVERLIGHT
    public static class PivotCommands
    {
        /// <summary>
        /// A routed UI command to delete pivot item.
        /// </summary>
        public static RoutedUICommand DeletePivotItem = new RoutedUICommand("DeltePivotitem",
                                                                   "DeltePivotitem",
                                                                   typeof(PivotCommands));
        /// <summary>
        /// A routed UI command to remove filter applied.
        /// </summary>
        public static RoutedUICommand DeleteFilter = new RoutedUICommand("DeleteFilter", "DeleteFilter",
                                                        typeof(PivotCommands));
        /// <summary>
        /// A routed UI command to show filter popup window.
        /// </summary>
        public static RoutedUICommand ShowFilter = new RoutedUICommand("ShowFilter", "ShowFilter",
                                                typeof(PivotCommands));
    }
#else

    /// <summary>
    /// Command to Delete PivotItems
    /// </summary>
    public class DeletePivotItemCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePivotItemCommand"/> class.
        /// </summary>
        public DeletePivotItemCommand() {}

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)  { return true;  }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) { CanExecuteChanged(parameter, new EventArgs()); }

    }

    /// <summary>
    /// Command to Delete Filters
    /// </summary>
    public class DeleteFilterCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFilterCommand"/> class.
        /// </summary>
        public DeleteFilterCommand() {}

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)  { return true;  }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) { CanExecuteChanged(parameter, new EventArgs()); }
    }

    /// <summary>
    /// Command to show Filter
    /// </summary>
    public class ShowFilterCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowFilterCommand"/> class.
        /// </summary>
        public ShowFilterCommand() { }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)  { return true;  }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) { CanExecuteChanged(parameter, new EventArgs()); }
    }

    /// <summary>
    /// Command to show Calculation window
    /// </summary>
    public class ShowCalculationCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCalculationCommand"/> class.
        /// </summary>
        public ShowCalculationCommand() { }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter) { return true; }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) { CanExecuteChanged(parameter, new EventArgs()); }
    }

#endif
}
