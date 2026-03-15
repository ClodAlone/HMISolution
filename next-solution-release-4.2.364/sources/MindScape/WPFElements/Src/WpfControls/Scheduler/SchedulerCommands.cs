using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains commands used within the <see cref="Scheduler"/> control.
  /// </summary>
  public static class SchedulerCommands
  {
    /// <summary>
    /// Moves the view backward to the previous block (e.g. previous week);
    /// </summary>
    public static readonly RoutedCommand DecrementViewCommand = new RoutedCommand("DecrementViewCommand", typeof(Scheduler));

    /// <summary>
    /// Moves the view forward to the next block (e.g. next week).
    /// </summary>
    public static readonly RoutedCommand IncrementViewCommand = new RoutedCommand("IncrementViewCommand", typeof(Scheduler));

    /// <summary>
    /// A command for adding a schedule item in a <see cref="Scheduler"/>.
    /// </summary>
    public static readonly RoutedCommand AddScheduleItemCommand = new RoutedCommand("AddScheduleItemCommand", typeof(Scheduler));

    /// <summary>
    /// A command for only displaying the 5 working days in <see cref="WeekView"/>.
    /// </summary>
    public static readonly RoutedCommand ShowWorkWeekCommand = new RoutedCommand("ShowWorkWeekCommand", typeof(Scheduler));

    /// <summary>
    /// A command for showing all 7 days of the week in <see cref="WeekView"/>.
    /// </summary>
    public static readonly RoutedCommand ShowFullWeekCommand = new RoutedCommand("ShowFullWeekCommand", typeof(Scheduler));

    /// <summary>
    /// A command for switching to <see cref="DayView"/>.
    /// </summary>
    public static readonly RoutedCommand SwitchToDayViewCommand = new RoutedCommand("SwitchToDayViewCommand", typeof(Scheduler));

    /// <summary>
    /// A command for switching to <see cref="WeekView"/>.
    /// </summary>
    public static readonly RoutedCommand SwitchToWeekViewCommand = new RoutedCommand("SwitchToWeekViewCommand", typeof(Scheduler));

    /// <summary>
    /// A command for switching to <see cref="MonthView"/>.
    /// </summary>
    public static readonly RoutedCommand SwitchToMonthViewCommand = new RoutedCommand("SwitchToMonthViewCommand", typeof(Scheduler));

    /// <summary>
    /// A command for bringing a specific day into view.
    /// </summary>
    public static readonly RoutedCommand GoToDayCommand = new RoutedCommand("GoToDayCommand", typeof(Scheduler));

    /// <summary>
    /// A command for bringing a specific week into view.
    /// </summary>
    public static readonly RoutedCommand GoToWeekCommand = new RoutedCommand("GoToWeekCommand", typeof(Scheduler));

    /// <summary>
    /// A command for bringing the previous schedule item into view.
    /// </summary>
    public static readonly RoutedCommand PreviousScheduleItemCommand = new RoutedCommand("PreviousAppointmentCommand", typeof(ScheduleView));

    /// <summary>
    /// A command for bringing the next schedule item into view.
    /// </summary>
    public static readonly RoutedCommand NextScheduleItemCommand = new RoutedCommand("NextAppointmentCommand", typeof(ScheduleView));

    /// <summary>
    /// A command for switching to day view from month view for a day that has more items than can be displayed.
    /// </summary>
    public static readonly RoutedCommand ViewDetailsCommand = new RoutedCommand("ViewDetailsCommand", typeof(MonthViewDayElement));

    /// <summary>
    /// A command for confirming dialogs.
    /// </summary>
    public static readonly RoutedCommand OkCommand = new RoutedCommand("OkCommand", typeof(UserControl));

    /// <summary>
    /// A command for canceling dialogs.
    /// </summary>
    public static readonly RoutedCommand CancelCommand = new RoutedCommand("CancelCommand", typeof(UserControl));

    /// <summary>
    /// A command for opening the recurrence dialog for a <see cref="ScheduleItem"/>.
    /// </summary>
    public static readonly RoutedCommand EditRecurrenceCommand = new RoutedCommand("EditRecurrenceCommand", typeof(UserControl));

    /// <summary>
    /// A command for removing the recurrence pattern from a <see cref="ScheduleItem"/>.
    /// </summary>
    public static readonly RoutedCommand RemoveRecurrenceCommand = new RoutedCommand("RemoveRecurrenceCommand", typeof(UserControl));
  }
}
