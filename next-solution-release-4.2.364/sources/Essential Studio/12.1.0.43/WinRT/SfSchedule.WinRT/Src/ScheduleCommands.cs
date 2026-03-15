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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Syncfusion.UI.Xaml.Schedule
{
    public static class ScheduleCommands
    {
        #region AddNew Command

        static ICommand addNewCommand;
        public static ICommand AddNewCommand
        {
            get
            {
                if (addNewCommand == null)
                    addNewCommand = new ScheduleCommand(OnAddNewCommand, CanAddNewExecute);
                return addNewCommand;
            }
        }

        static void OnAddNewCommand(object parameter)
        {
            if (CanAddNewExecute(parameter))
            {
                (parameter as SfSchedule).AddNewAppointment();
            }
        }

        static bool CanAddNewExecute(object parameter)
        {
            return (parameter is SfSchedule);
        }

        #endregion

        #region Edit Command

        static ICommand editCommand;
        public static ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                    editCommand = new ScheduleCommand(OnEditCommand, CanEditExecute);
                return editCommand;
            }
        }

        static void OnEditCommand(object parameter)
        {
            if (CanEditExecute(parameter))
            {
                (parameter as SfSchedule).EditAppointment();
            }
        }

        static bool CanEditExecute(object parameter)
        {
            return (parameter is SfSchedule && (parameter as SfSchedule).SelectedAppointment != null);
        }

        #endregion

        #region Delete Command

        static ICommand deleteCommand;
        public static ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new ScheduleCommand(OnDeleteCommand, CanDeleteExecute);
                return deleteCommand;
            }
        }

        static void OnDeleteCommand(object parameter)
        {
            if (CanDeleteExecute(parameter))
            {
                (parameter as SfSchedule).DeleteAppointment();
            }
        }

        static bool CanDeleteExecute(object parameter)
        {
            return (parameter is SfSchedule && (parameter as SfSchedule).SelectedAppointment != null);
        }

        #endregion

        #region DragAndDrop Command

        static ICommand dragAndDropCommand;
        public static ICommand DragAndDropCommand
        {
            get
            {
                if (dragAndDropCommand == null)
                    dragAndDropCommand = new ScheduleCommand(OnDragAndDropCommand, CanDragAndDropExecute);
                return dragAndDropCommand;
            }
        }

        static void OnDragAndDropCommand(object parameter)
        {
            if (CanDragAndDropExecute(parameter))
            {
                (parameter as SfSchedule).ResizeAppointment();
            }
        }

        static bool CanDragAndDropExecute(object parameter)
        {
            return (parameter is SfSchedule && (parameter as SfSchedule).SelectedAppointment != null);
        }

        #endregion

        #region Copy Command

        static ICommand copyCommand;
        public static ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null)
                    copyCommand = new ScheduleCommand(OnCopyCommand, CanCopyExecute);
                return copyCommand;
            }
        }

        static void OnCopyCommand(object parameter)
        {
            if (CanCopyExecute(parameter))
            {
                (parameter as SfSchedule).CopyAppointment();
            }
        }

        static bool CanCopyExecute(object parameter)
        {
            return (parameter is SfSchedule && (parameter as SfSchedule).SelectedAppointment != null);
        }

        #endregion

        #region Paste Command

        static ICommand pasteCommand;
        public static ICommand PasteCommand
        {
            get
            {
                if (pasteCommand == null)
                    pasteCommand = new ScheduleCommand(OnPasteCommand, CanPasteExecute);
                return pasteCommand;
            }
        }

        static void OnPasteCommand(object parameter)
        {
            if (CanPasteExecute(parameter))
            {
                (parameter as SfSchedule).PasteAppointment();
            }
        }

        static bool CanPasteExecute(object parameter)
        {
            return (parameter is SfSchedule && (parameter as SfSchedule).CopiedAppointment != null);
        }

        #endregion
    }

    internal class ScheduleCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        Func<object, bool> canExecute;
        Action<object> executeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCommand"/> class.
        /// </summary>
        /// <param name="executeAction">The action to be executed.</param>
        /// <param name="canExecute">The can execute.</param>
        public ScheduleCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
            return canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// </param>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }

}
