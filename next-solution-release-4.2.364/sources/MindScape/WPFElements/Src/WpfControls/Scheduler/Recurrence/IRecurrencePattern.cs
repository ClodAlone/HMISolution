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
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents the recurrence pattern of a recurring <see cref="ScheduleItem"/>.
  /// </summary>
  public interface IRecurrencePattern
  {
    /// <summary>
    /// Gets a list of all the dates where schedule items have been deleted from the recurrence pattern.
    /// </summary>
    ObservableCollection<DateTime> SkipDates { get; }

    /// <summary>
    /// Raised when the SkipDates collection is changed.
    /// </summary>
    event NotifyCollectionChangedEventHandler SkipDatesChanged;

#if SILVERLIGHT

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day);

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day, int occurrenceCount);

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start and end date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="endDate">The end date of recurrence.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day, DateTime endDate);

#endif

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day, DayOfWeek firstDayOfWeek);

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day, int occurrenceCount, DayOfWeek firstDayOfWeek);

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start and end date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="endDate">The end date of recurrence.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    bool Includes(DateTime startDate, DateTime day, DateTime endDate, DayOfWeek firstDayOfWeek);
  }
}
