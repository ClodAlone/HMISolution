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
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif
using System.Collections.Specialized;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Specifies the recurrence schedule for a <see cref="ScheduleItem"/>.
  /// </summary>
  public class RecurrenceInfo
  {
    private readonly TimeSpan _startTime;
    private readonly TimeSpan _duration;

    private readonly DateTime _startDate;
    private readonly int _maxOccurrences;
    private readonly DateTime _endDate;
    private readonly RecurrenceEndType _endType;

    private readonly IRecurrencePattern _recurrencePattern;
    private readonly DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs until a specified date.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="endDate">The date on which the recurrence is to end.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo ToEndDate(DateTime start, TimeSpan duration, DateTime endDate, IRecurrencePattern recurrencePattern)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.EndBy, endDate, Int32.MaxValue);
    }

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs for a specified number of occurrences.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="maxOccurrences">The number of occurrences.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo ToMaxOccurrences(DateTime start, TimeSpan duration, int maxOccurrences, IRecurrencePattern recurrencePattern)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.EndAfter, DateTime.MaxValue, maxOccurrences);
    }

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs indefinitely.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo Forever(DateTime start, TimeSpan duration, IRecurrencePattern recurrencePattern)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.NoEndDate, DateTime.MaxValue, Int32.MaxValue);
    }

    #region Factory methods that have "firstDayOfWeek" parameters

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs until a specified date.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="endDate">The date on which the recurrence is to end.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo ToEndDate(DateTime start, TimeSpan duration, DateTime endDate, IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.EndBy, endDate, Int32.MaxValue, firstDayOfWeek);
    }

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs for a specified number of occurrences.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="maxOccurrences">The number of occurrences.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo ToMaxOccurrences(DateTime start, TimeSpan duration, int maxOccurrences, IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.EndAfter, DateTime.MaxValue, maxOccurrences, firstDayOfWeek);
    }

    /// <summary>
    /// Creates a <see cref="RecurrenceInfo"/> that runs indefinitely.
    /// </summary>
    /// <param name="start">The start date and time of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>A <see cref="RecurrenceInfo"/> representing the specified recurrence schedule.</returns>
    public static RecurrenceInfo Forever(DateTime start, TimeSpan duration, IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      return new RecurrenceInfo(start.Date, start.TimeOfDay, duration, recurrencePattern, RecurrenceEndType.NoEndDate, DateTime.MaxValue, Int32.MaxValue, firstDayOfWeek);
    }

    #endregion // Factory methods that have "firstDayOfWeek" parameters

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurrenceInfo"/> class.
    /// </summary>
    /// <param name="startDate">The start date of the recurrence.</param>
    /// <param name="startTime">The start time of day of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="endType">The end criterion for the recurrence.</param>
    /// <param name="endDate">The date on which the recurrence is to end (for RecurrenceEndType.EndBy).</param>
    /// <param name="maxOccurrences">The number of occurrences (for RecurrenceEndType.EndAfter).</param>
    /// <remarks>This constructor is primarily for deserialisation purposes.  The factory methods
    /// "ToEndDate", "ToMaxOccurrences" and "Forever" provide a simpler
    /// API for creating specific types of RecurrenceInfo.</remarks>
    public RecurrenceInfo(DateTime startDate, TimeSpan startTime, TimeSpan duration, IRecurrencePattern recurrencePattern, RecurrenceEndType endType, DateTime endDate, int maxOccurrences)
      :this(startDate, startTime, duration, recurrencePattern, endType, endDate, maxOccurrences, DayOfWeek.Monday)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurrenceInfo"/> class.
    /// </summary>
    /// <param name="startDate">The start date of the recurrence.</param>
    /// <param name="startTime">The start time of day of the recurrence.</param>
    /// <param name="duration">The duration of the <see cref="ScheduleItem"/>.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="endType">The end criterion for the recurrence.</param>
    /// <param name="endDate">The date on which the recurrence is to end (for RecurrenceEndType.EndBy).</param>
    /// <param name="maxOccurrences">The number of occurrences (for RecurrenceEndType.EndAfter).</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <remarks>This constructor is primarily for deserialisation purposes.  The factory methods
    /// "ToEndDate", "ToMaxOccurrences" and "Forever" provide a simpler
    /// API for creating specific types of RecurrenceInfo.</remarks>
    public RecurrenceInfo(DateTime startDate, TimeSpan startTime, TimeSpan duration, IRecurrencePattern recurrencePattern, RecurrenceEndType endType, DateTime endDate, int maxOccurrences, DayOfWeek firstDayOfWeek)
    {
      _startDate = startDate;
      _startTime = startTime;
      _duration = duration;

      _endType = endType;
      _endDate = endDate;
      _maxOccurrences = maxOccurrences;
      _recurrencePattern = recurrencePattern;
      _recurrencePattern.SkipDatesChanged += new NotifyCollectionChangedEventHandler(RecurrencePattern_SkipDatesChanged);
      _firstDayOfWeek = firstDayOfWeek;
    }

    /// <summary>
    /// Occurs when the contents of the skip dates collection of the <see cref="RecurrencePattern"/> changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler RecurrencePatternSkipDatesChanged;

    private void RecurrencePattern_SkipDatesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      foreach (DateTime dateTime in e.NewItems)
      {
        if (!IncludesIgnoreSkipDates(dateTime))
        {
          throw new InvalidOperationException(dateTime + " can not be added as a skip date because the recurrence pattern does not include it");
        }
      }
      NotifyCollectionChangedEventHandler handler = RecurrencePatternSkipDatesChanged;
      if(handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Gets the start date of the recurrence.
    /// </summary>
    public DateTime StartDate
    {
      get { return _startDate; }
    }

    /// <summary>
    /// Gets the start time of day of the recurrence.
    /// </summary>
    public TimeSpan StartTime
    {
      get { return _startTime; }
    }

    /// <summary>
    /// Gets the duration of the recurring item.
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
    }

    /// <summary>
    /// Gets the maximum number of occurrences.
    /// </summary>
    public int MaxOccurrences
    {
      get { return _maxOccurrences; }
    }

    /// <summary>
    /// Gets the end date of the recurrence.
    /// </summary>
    public DateTime EndDate
    {
      get { return _endDate; }
    }

    /// <summary>
    /// Gets the end criterion for the recurrence.
    /// </summary>
    public RecurrenceEndType EndType
    {
      get { return _endType; }
    }

    /// <summary>
    /// Gets the recurrence pattern.
    /// </summary>
    public IRecurrencePattern RecurrencePattern
    {
      get { return _recurrencePattern; }
    }

    internal bool Includes(DayModel day)
    {
      DateTime date = day.Date;
      switch (_endType)
      {
        case RecurrenceEndType.NoEndDate:
          return _recurrencePattern.Includes(_startDate, date, _firstDayOfWeek);
        case RecurrenceEndType.EndAfter:
          return _recurrencePattern.Includes(_startDate, date, _maxOccurrences, _firstDayOfWeek);
        case RecurrenceEndType.EndBy:
          return _recurrencePattern.Includes(_startDate, date, _endDate, _firstDayOfWeek);
        default:
          throw new InvalidOperationException("Unknown EndType in RecurrenceInfo");
      }
    }

    private bool IncludesIgnoreSkipDates(DateTime dateTime)
    {
      RecurrencePattern pattern = _recurrencePattern as RecurrencePattern;
      switch (_endType)
      {
        case RecurrenceEndType.NoEndDate:
          return pattern.IncludesIgnoreSkipDates(_startDate, dateTime, _firstDayOfWeek);
        case RecurrenceEndType.EndAfter:
          return pattern.IncludesIgnoreSkipDates(_startDate, dateTime, _maxOccurrences, _firstDayOfWeek);
        case RecurrenceEndType.EndBy:
          return pattern.IncludesIgnoreSkipDates(_startDate, dateTime, _endDate, _firstDayOfWeek);
        default:
          throw new InvalidOperationException("Unknown EndType in RecurrenceInfo");
      }
    }
  }
}
