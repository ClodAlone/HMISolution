using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains commands used with the <see cref="MonthCalendar"/> control.
  /// </summary>
  public static class CalendarCommands
  {
    /// <summary>
    /// A command for moving the selection to one year earlier.
    /// </summary>
    public static readonly RoutedCommand YearBack = new RoutedCommand("YearBack", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one year later.
    /// </summary>
    public static readonly RoutedCommand YearForward = new RoutedCommand("YearForward", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one month earlier.
    /// </summary>
    public static readonly RoutedCommand MonthBack = new RoutedCommand("MonthBack", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one month later.
    /// </summary>
    public static readonly RoutedCommand MonthForward = new RoutedCommand("MonthForward", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one week earlier.
    /// </summary>
    public static readonly RoutedCommand WeekBack = new RoutedCommand("WeekBack", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one week later.
    /// </summary>
    public static readonly RoutedCommand WeekForward = new RoutedCommand("WeekForward", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one day earlier.
    /// </summary>
    public static readonly RoutedCommand DayBack = new RoutedCommand("DayBack", typeof(MonthCalendar));

    /// <summary>
    /// A command for moving the selection to one day later.
    /// </summary>
    public static readonly RoutedCommand DayForward = new RoutedCommand("DayForward", typeof(MonthCalendar));

    /// <summary>
    /// A command for selecting the current date.
    /// </summary>
    public static readonly RoutedCommand SelectToday = new RoutedCommand("SelectToday", typeof(MonthCalendar));
  }
}
