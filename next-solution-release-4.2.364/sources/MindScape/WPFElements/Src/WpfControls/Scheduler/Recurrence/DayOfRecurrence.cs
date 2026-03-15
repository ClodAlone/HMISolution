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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// The day on which a recurring schedule item recurs.  This may be a day of the
  /// week, or it may match multiple days of the week (as in "last day of the month"
  /// or "first weekday in January").  It is used in conjunction with the <see cref="Occurrence"/>
  /// type.
  /// </summary>
  public enum DayOfRecurrence
  {
    /// <summary>
    /// The Occurrence counts all days.
    /// </summary>
    Day,

    /// <summary>
    /// The Occurrence counts all weekdays (Monday to Friday).
    /// </summary>
    Weekday,

    /// <summary>
    /// The Occurrence counts weekend days (Saturday and Sunday);
    /// </summary>
    WeekendDay,

    /// <summary>
    /// The Occurrence counts Sundays.
    /// </summary>
    Sunday,

    /// <summary>
    /// The Occurrence counts Mondays.
    /// </summary>
    Monday,

    /// <summary>
    /// The Occurrence counts Tuesdays.
    /// </summary>
    Tuesday,

    /// <summary>
    /// The Occurrence counts Wednesdays.
    /// </summary>
    Wednesday,

    /// <summary>
    /// The Occurrence counts Thursdays.
    /// </summary>
    Thursday,

    /// <summary>
    /// The Occurrence counts Fridays.
    /// </summary>
    Friday,

    /// <summary>
    /// The Occurrence counts Saturdays.
    /// </summary>
    Saturday
  }
}
