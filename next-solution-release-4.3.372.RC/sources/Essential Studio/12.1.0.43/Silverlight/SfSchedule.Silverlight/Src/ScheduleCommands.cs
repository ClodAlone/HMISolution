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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

#if WPF
        private static readonly RoutedUICommand c_delete = new RoutedUICommand("Delete", "Delete", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_create = new RoutedUICommand("Create", "Create", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_copy = new RoutedUICommand("Copy", "Copy", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_paste = new RoutedUICommand("Paste", "Paste", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_up = new RoutedUICommand("Up", "Up", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_down = new RoutedUICommand("Down", "Down", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_right = new RoutedUICommand("Right", "Right", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_left = new RoutedUICommand("Left", "Left", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_home = new RoutedUICommand("Home", "Home", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_end = new RoutedUICommand("End", "End", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_pgUp = new RoutedUICommand("PgUp", "PgUp", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_pgDown = new RoutedUICommand("PgDown", "PgDown", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_firstDayCurrentWeek = new RoutedUICommand("FirstDayCurrentWeek", "FirstDayCurrentWeek", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_lastDayCurrentWeek = new RoutedUICommand("LastDayCurrentWeek", "LastDayCurrentWeek", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_sameDayPreviousWeek = new RoutedUICommand("SameDayPreviousWeek", "SameDayPreviousWeek", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_SameDayNextWeek = new RoutedUICommand("SameDayNextWeek", "SameDayNextWeek", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_zoomin = new RoutedUICommand("ZoomIn", "ZoomIn", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_zoomout = new RoutedUICommand("ZoomOut", "ZoomOut", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_previousAppointment = new RoutedUICommand("PreviousAppointment", "PreviousAppointment", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_nextAppointment = new RoutedUICommand("NextAppointment", "NextAppointment", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_resizeAppointmentUp = new RoutedUICommand("ResizeAppointmentUp", "ResizeAppointmentUp", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_resizeAppointmentDown = new RoutedUICommand("ResizeAppointmentDown", "ResizeAppointmentDown", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_timeSelectionUp = new RoutedUICommand("TimeSelectionUp", "TimeSelectionUp", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_timeSelectionDown = new RoutedUICommand("TimeSelectionDown", "TimeSelectionDown", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_createAppOnMultiSelection = new RoutedUICommand("CreateAppOnMultiSelection", "CreateAppOnMultiSelection", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_timeSelectionRight = new RoutedUICommand("TimeSelectionRight", "TimeSelectionRight", typeof(ScheduleCommands));
        private static readonly RoutedUICommand c_timeSelectionLeft = new RoutedUICommand("TimeSelectionLeft", "TimeSelectionLeft", typeof(ScheduleCommands));


        public static RoutedUICommand TimeSelectionRight
        {
            get
            {
                return c_timeSelectionRight;
            }
        }

        public static RoutedUICommand TimeSelectionLeft
        {
            get
            {
                return c_timeSelectionLeft;
            }
        }
        public static RoutedUICommand CreateAppOnMultiSelection
        {
            get
            {
                return c_createAppOnMultiSelection;
            }
        }

        public static RoutedUICommand TimeSelectionUp
        {
            get
            {
                return c_timeSelectionUp;
            }
        }

        public static RoutedUICommand TimeSelectionDown
        {
            get
            {
                return c_timeSelectionDown;
            }
        }
        public static RoutedUICommand ResizeAppointmentUp
        {
            get
            {
                return c_resizeAppointmentUp;
            }
        }
        public static RoutedUICommand ResizeAppointmentDown
        {
            get
            {
                return c_resizeAppointmentDown;
            }
        }

        public static RoutedUICommand PreviousAppointment
        {
            get
            {
                return c_previousAppointment;
            }
        }
        public static RoutedUICommand NextAppointment
        {
            get
            {
                return c_nextAppointment;
            }
        }
        public static RoutedUICommand ZoomIn
        {
            get
            {
                return c_zoomin;
            }
        }
        public static RoutedUICommand ZoomOut
        {
            get
            {
                return c_zoomout;
            }
        }

        public static RoutedUICommand SameDayPreviousWeek
        {
            get
            {
                return c_sameDayPreviousWeek;
            }
        }
        public static RoutedUICommand SameDayNextWeek
        {
            get
            {
                return c_SameDayNextWeek;
            }
        }
        public static RoutedUICommand FirstDayCurrentWeek
        {
            get
            {
                return c_firstDayCurrentWeek;
            }
        }
        public static RoutedUICommand LastDayCurrentWeek
        {
            get
            {
                return c_lastDayCurrentWeek;
            }
        }
        public static RoutedUICommand Delete
        {
            get
            {
                return c_delete;
            }
        }
        public static RoutedUICommand Create
        {
            get
            {
                return c_create;
            }
        }
        public static RoutedUICommand Copy
        {
            get
            {
                return c_copy;
            }
        }
        public static RoutedUICommand Paste
        {
            get
            {
                return c_paste;
            }
        }
        public static RoutedUICommand Up
        {
            get
            {
                return c_up;
            }
        }
        public static RoutedUICommand Down
        {
            get
            {
                return c_down;
            }
        }
        public static RoutedUICommand Right
        {
            get
            {
                return c_right;
            }
        }
        public static RoutedUICommand Left
        {
            get
            {
                return c_left;
            }
        }
        public static RoutedUICommand PgUp
        {
            get
            {
                return c_pgUp;
            }
        }
        public static RoutedUICommand PgDown
        {
            get
            {
                return c_pgDown;
            }
        }
        public static RoutedUICommand Home
        {
            get
            {
                return c_home;
            }
        }
        public static RoutedUICommand End
        {
            get
            {
                return c_end;
            }
        }
#endif
    }

#if SILVERLIGHT

    public class Command : ICommand
    {
        public delegate void SimpleDelegate();
        public event SimpleDelegate Execute;
        //public event CancelEventHandler CanExecute;

        void ICommand.Execute(object parameter)
        {
            if (Execute != null)
                Execute();
        }

        
        bool ICommand.CanExecute(object parameter)
        {
            // not necessary for this application, and CancelEventArgs doesn't exist on Silverlight
            //CancelEventArgs args = new CancelEventArgs(false);
            //if (CanExecute != null)
            //    CanExecute(this, args);
            //return !args.Cancel;
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        public Key Key = Key.None;
        public string DisplayKey;
        public ModifierKeys ModifierKeys = ModifierKeys.None;
        public string Text = "";
        public bool HasMenuItem = true;
        public Button Button = null; // hooks up the command to the button
    }

    public class CommandHelper
    {
        private UIElement owner;
        public List<Command> commands = new List<Command>();

        public CommandHelper(UIElement owner)
        {
            this.owner = owner;
            owner.KeyDown += new KeyEventHandler(keyDown);
        }

        
        private void keyDown(object sender, KeyEventArgs e)
        {
            foreach (Command command in commands)
            {
                // Intentionally ignore modifier keys
                bool shiftKeyMatches = (command.ModifierKeys & ModifierKeys.Shift) == (Keyboard.Modifiers & ModifierKeys.Shift);
                if (command.Key == e.Key && shiftKeyMatches)
                {
                    (command as ICommand).Execute(null);
                }
            }
        }



#if WPF
        public void AddBinding(Command command, RoutedCommand applicationCommand)
        {
            CommandBinding binding = new CommandBinding(applicationCommand);
            binding.Executed += delegate(object sender, ExecutedRoutedEventArgs e)
            {
                ((ICommand)command).Execute(null);
            };
            owner.CommandBindings.Add(binding);
        }

        public ContextMenu contextmenu;
#endif

        public void AddMenuSeparator()
        {
#if WPF
            var item = new Separator();
            contextmenu.Items.Add(item);
#endif
        }

        public void AddCommand(Command command)
        {
            commands.Add(command);

            // KeyBinding insists that ModifierKeys != 0 for alphabetic keys,
            // so we have to roll our own
            //this.CommandBindings.Add(new CommandBinding(command));
            //KeyGesture gesture = new KeyGesture(command.Key, command.ModifierKeys);
            //this.InputBindings.Add(new KeyBinding(command, gesture));

#if WPF
            if (command.HasMenuItem) {
                MenuItem item = new MenuItem();
                string text = command.Text + ShortcutText(command);
                item.Header = text;
                item.Command = command;
                contextmenu.Items.Add(item);
            }
#endif
            if (command.Button != null)
            {
                string text = command.Text + ShortcutText(command);
                ToolTip tooltip = new ToolTip();
                tooltip.Content = text;
                tooltip.Background = (Brush)Application.Current.Resources["menuBackground"];
                tooltip.Foreground = (Brush)Application.Current.Resources["menuForeground"];
                tooltip.BorderBrush = (Brush)Application.Current.Resources["shotclockBrush"];
                command.Button.Click += (object sender, RoutedEventArgs e) =>
                {
                    (command as ICommand).Execute(null);
                };
#if WPF
                command.Button.ToolTip = tooltip;
                //command.Button.Command = command;
#endif
            }
        }

        private static string ShortcutText(Command command)
        {
            string text = "";
            string keyText = null;
            if (command.DisplayKey != null)
                keyText = command.DisplayKey;
            else if (command.Key != Key.None)
            {
                keyText = command.Key.ToString();
                if ((command.ModifierKeys & ModifierKeys.Shift) != 0)
                    keyText = "shift+" + keyText;
            }

            if (keyText != null)
                text += " (" + keyText + ")";
            return text;
        }
    }  
#endif

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
