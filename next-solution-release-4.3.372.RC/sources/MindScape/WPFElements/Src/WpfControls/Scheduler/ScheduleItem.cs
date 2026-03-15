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
using System.ComponentModel;
using System.Collections.Generic;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents an item such as an appointment or task in a <see cref="Schedule"/>.
  /// </summary>
  public class ScheduleItem : ViewModelBase
  {
    private string _name;
    private DateTime _startTime, _endTime;
    private bool _isSelected;

    private RecurrenceInfo _recurrenceInfo;
    private ScheduleItem _instanceOf;

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    public String Name
    {
      get { return _name; }
      set
      {
        RemoveFromRecurrencePattern();
        Set(ref _name, value, "Name");
      }
    }

    /// <summary>
    /// Gets whether the item is recurring.
    /// </summary>
    public bool IsRecurring
    {
      get { return _recurrenceInfo != null; }
    }

    /// <summary>
    /// Gets or sets the item's recurrence behavior.  This will be
    /// null if the item is not recurring.
    /// </summary>
    public RecurrenceInfo RecurrenceInfo
    {
      get { return _recurrenceInfo; }
      set
      {
        if (IsInstanceOfRecurringItem)
        {
          // TODO: this is a weasel-out: we don't yet give people a way to find the thing we're an instance of, and
          // we have bugs with changing the recurrence even if they do manage to find it.
          throw new InvalidOperationException("Cannot change recurrence on an instance of a recurring item.  Find the original item and set recurrence on that instead.");
        }

        _recurrenceInfo = value;
        OnPropertyChanged("RecurrenceInfo");
        OnPropertyChanged("IsRecurring");
        OnRecurrenceChanged();
      }
    }

    internal ScheduleItem CreateInstance(DateTime date)
    {
      if (!IsRecurring)
      {
        throw new InvalidOperationException("CreateInstance can be called only on a recurring ScheduleItem.");
      }

      ScheduleItem instanceOfRecurrence = CreateInstanceCore();
      instanceOfRecurrence.Name = Name;
      instanceOfRecurrence._instanceOf = this;

      instanceOfRecurrence.StartTime = new DateTime(date.Year, date.Month, date.Day).Add(RecurrenceInfo.StartTime);
      instanceOfRecurrence.EndTime = instanceOfRecurrence.StartTime + RecurrenceInfo.Duration;

      return instanceOfRecurrence;
    }

    /// <summary>
    /// Creates a new instance of a recurring schedule item.
    /// Custom schedule item implementations can override this method to create an instance of the custom schedule item.
    /// </summary>
    /// <returns></returns>
    protected virtual ScheduleItem CreateInstanceCore()
    {
      return new ScheduleItem();
    }

    /// <summary>
    /// Gets whether the item is an instance of a recurring <see cref="ScheduleItem"/>.
    /// </summary>
    public bool IsInstanceOfRecurringItem
    {
      get { return _instanceOf != null; }
    }

    internal ScheduleItem InstanceOf
    {
      get { return _instanceOf; }
    }

    internal void RemoveFromRecurrencePattern()
    {
      if (IsInstanceOfRecurringItem && _endTime.Year > 1 && _startTime.Year > 1)
      {
        _instanceOf.RecurrenceInfo.RecurrencePattern.SkipDates.Add(StartTime);
        _instanceOf = null;
        OnPropertyChanged("IsInstanceOfRecurringItem");
      }
    }

    //TODO: why do these 6 methods exist? we don't seem to be using them...

    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur until the specified date.
    /// </summary>
    /// <param name="until">The date until which the item should recur.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    public void RecurUntil(DateTime until, IRecurrencePattern recurrencePattern)
    {
      RecurrenceInfo = RecurrenceInfo.ToEndDate(StartTime, EndTime - StartTime, until, recurrencePattern, DayOfWeek.Monday);
    }


    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur for the specified number of occurrences.
    /// </summary>
    /// <param name="maxOccurrences">The number of times the item should recur.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    public void RecurTimes(int maxOccurrences, IRecurrencePattern recurrencePattern)
    {
      RecurrenceInfo = RecurrenceInfo.ToMaxOccurrences(StartTime, EndTime - StartTime, maxOccurrences, recurrencePattern, DayOfWeek.Monday);
    }


    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur indefinitely.
    /// </summary>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    public void RecurForever(IRecurrencePattern recurrencePattern)
    {
      RecurrenceInfo = RecurrenceInfo.Forever(StartTime, EndTime - StartTime, recurrencePattern, DayOfWeek.Monday);
    }

    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur until the specified date.
    /// </summary>
    /// <param name="until">The date until which the item should recur.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    public void RecurUntil(DateTime until, IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      RecurrenceInfo = RecurrenceInfo.ToEndDate(StartTime, EndTime - StartTime, until, recurrencePattern, firstDayOfWeek);
    }


    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur for the specified number of occurrences.
    /// </summary>
    /// <param name="maxOccurrences">The number of times the item should recur.</param>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    public void RecurTimes(int maxOccurrences, IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      RecurrenceInfo = RecurrenceInfo.ToMaxOccurrences(StartTime, EndTime - StartTime, maxOccurrences, recurrencePattern, firstDayOfWeek);
    }


    /// <summary>
    /// Specifies that the <see cref="ScheduleItem"/> should recur indefinitely.
    /// </summary>
    /// <param name="recurrencePattern">The recurrence pattern.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    public void RecurForever(IRecurrencePattern recurrencePattern, DayOfWeek firstDayOfWeek)
    {
      RecurrenceInfo = RecurrenceInfo.Forever(StartTime, EndTime - StartTime, recurrencePattern, firstDayOfWeek);
    }

    /// <summary>
    /// Gets or sets when the item starts.
    /// </summary>
    public DateTime StartTime
    {
      get { return _startTime; }
      set
      {
        RemoveFromRecurrencePattern();
        _startTime = value;
        OnStartTimeChanged();
      }
    }

    /// <summary>
    /// Gets or sets when the item ends.
    /// </summary>
    public DateTime EndTime
    {
      get { return _endTime; }
      set
      {
        RemoveFromRecurrencePattern();
        _endTime = value;
        OnEndTimeChanged();
      }
    }

    /// <summary>
    /// Moves the item by the specified time.
    /// </summary>
    /// <param name="deltaMinutes">The time to offset the item, in minutes.</param>
    public void MoveBy(int deltaMinutes)
    {
      RemoveFromRecurrencePattern();
      _endTime = _endTime.AddMinutes(deltaMinutes);
      _startTime = _startTime.AddMinutes(deltaMinutes);
      OnStartTimeChanged();
      OnEndTimeChanged();
    }

    /// <summary>
    /// Gets or sets whether the item is selected.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          OnIsSelectedChanged();
          OnPropertyChanged("IsSelected");
        }
      }
    }

    /// <summary>
    /// Occurs when the item's selection status changes.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    private void OnIsSelectedChanged()
    {
      EventHandler handler = IsSelectedChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Occurs when the item's <see cref="StartTime"/> changes.
    /// </summary>
    public event EventHandler StartTimeChanged;

    /// <summary>
    /// Occurs when the item's <see cref="EndTime"/> changes.
    /// </summary>
    public event EventHandler EndTimeChanged;

    private void OnStartTimeChanged()
    {
      OnPropertyChanged("StartTime");
      EventHandler handler = StartTimeChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void OnEndTimeChanged()
    {
      OnPropertyChanged("EndTime");
      EventHandler handler = EndTimeChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal bool IsSilent { get; set; }

    /// <summary>
    /// Occurs when the item's recurrence information changes.
    /// </summary>
    public event EventHandler RecurrenceChanged;

    /// <summary>
    /// Raises the <see cref="RecurrenceChanged"/> event.
    /// </summary>
    protected virtual void OnRecurrenceChanged()
    {
      var handler = RecurrenceChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }
  }
}
