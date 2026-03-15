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
using System.Collections.Generic;
using System.Collections.Specialized;
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
  public abstract class RecurrencePattern : IRecurrencePattern
  {
    private ObservableCollection<DateTime> _skipDates;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurrencePattern"/> class.
    /// </summary>
    public RecurrencePattern()
    {
      _skipDates = new ObservableCollection<DateTime>();
      _skipDates.CollectionChanged += new NotifyCollectionChangedEventHandler(SkipDates_CollectionChanged);
    }

    /// <summary>
    /// Occurs when the contents of the <see cref="SkipDates"/> collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler SkipDatesChanged;

    private void SkipDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventHandler handler = SkipDatesChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Gets a collection containing all the dates that are included in this <see cref="RecurrencePattern"/> but have been skipped.
    /// Adding a date to this collection will prevent schedule items from occuring on that date based on this pattern.
    /// </summary>
    public ObservableCollection<DateTime> SkipDates
    {
      get
      {
        return _skipDates;
      }
    }

#if SILVERLIGHT

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day)
    {
      return IncludesUsingSkipDates(startDate, day, false, null);
    }

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start and end date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="endDate">The end date of recurrence.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day, DateTime endDate)
    {
      DateTime date = day.Date;
      if (!DateTimeUtils.IsSameDay(date, endDate) && date > endDate)
      {
        return false;
      }

      return Includes(startDate, day);
    }

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day, int occurrenceCount)
    {
      return IncludesUsingSkipDates(startDate, day, true, occurrenceCount);
    }

#endif

    #region Methods that use a specified first day of week

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day, DayOfWeek firstDayOfWeek)
    {
      return IncludesUsingSkipDates(startDate, day, false, null, firstDayOfWeek);
    }

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start and end date.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="endDate">The end date of recurrence.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day, DateTime endDate, DayOfWeek firstDayOfWeek)
    {
      DateTime date = day.Date;
      if (!DateTimeUtils.IsSameDay(date, endDate) && date > endDate)
      {
        return false;
      }

      return Includes(startDate, day, firstDayOfWeek);
    }

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    public bool Includes(DateTime startDate, DateTime day, int occurrenceCount, DayOfWeek firstDayOfWeek)
    {
      return IncludesUsingSkipDates(startDate, day, true, occurrenceCount, firstDayOfWeek);
    }

    #endregion // Methods that use a specified first day of week

#if SILVERLIGHT

    private bool IncludesUsingSkipDates(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount)
    {
      return IncludesUsingSkipDates(startDate, day, limitOccurrences, occurrenceCount, DayOfWeek.Monday);
    }

#endif

    private bool IncludesUsingSkipDates(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount, DayOfWeek firstDayOfWeek)
    {
      foreach (DateTime time in _skipDates)
      {
        if (time.Date == day.Date)
        {
          return false;
        }
      }
      return Includes(startDate, day, limitOccurrences, occurrenceCount, firstDayOfWeek);
    }

    /// <summary>
    /// When overridden in a derived class, gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="limitOccurrences">Whether to limit the number of occurrences considered.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider, if <paramref name="limitOccurrences"/> is true.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    protected abstract bool Includes(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount);

    /// <summary>
    /// When overridden in a derived class, gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="limitOccurrences">Whether to limit the number of occurrences considered.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider, if <paramref name="limitOccurrences"/> is true.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    protected virtual bool Includes(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount, DayOfWeek firstDayOfWeek)
    {
      return Includes(startDate, day, limitOccurrences, occurrenceCount);
    }

#if SILVERLIGHT

    // Methods for testing if a DateTime is included in this pattern without using the skip dates.
    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day)
    {
      return Includes(startDate, day, false, null);
    }

    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day, DateTime endDate)
    {
      DateTime date = day.Date;
      if (!DateTimeUtils.IsSameDay(date, endDate) && date > endDate)
      {
        return false;
      }

      return IncludesIgnoreSkipDates(startDate, day);
    }

    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day, int occurrenceCount)
    {
      return Includes(startDate, day, true, occurrenceCount);
    }

#endif

    #region Methods for testing if a DateTime is included in this pattern without using the skip dates AND with using a specified start of week.

    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day, DayOfWeek firstDayOfWeek)
    {
      return Includes(startDate, day, false, null, firstDayOfWeek);
    }

    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day, DateTime endDate, DayOfWeek firstDayOfWeek)
    {
      DateTime date = day.Date;
      if (!DateTimeUtils.IsSameDay(date, endDate) && date > endDate)
      {
        return false;
      }

      return IncludesIgnoreSkipDates(startDate, day, firstDayOfWeek);
    }

    internal bool IncludesIgnoreSkipDates(DateTime startDate, DateTime day, int occurrenceCount, DayOfWeek firstDayOfWeek)
    {
      return Includes(startDate, day, true, occurrenceCount, firstDayOfWeek);
    }

    #endregion // Methods for testing if a DateTime is included in this pattern without using the skip dates AND with using a specified start of week.
  }
}
