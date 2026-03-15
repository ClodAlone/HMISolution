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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a slot on the display surface of a <see cref="Scheduler"/> control.
  /// </summary>
  public class TimeSlot : ViewModelBase, IComparable<TimeSlot>
  {
    private DateTime _dateTime;
    private bool _isSelected;
    private TimeSlotState _timeSlotState;

    internal TimeSlot(DateTime dateTime, WorkHours workHours)
    {
      _dateTime = dateTime;

      //TODO: this stuff should be moved out into some kind of policy class so that its not so hard-coded like this.
      _timeSlotState = TimeSlotState.WorkTime;
      if (_dateTime.DayOfWeek == DayOfWeek.Saturday || _dateTime.DayOfWeek == DayOfWeek.Sunday)
      {
        _timeSlotState = TimeSlotState.NotWorkTime;
      }
      else if (_dateTime < new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, workHours.StartTime.Hour, workHours.StartTime.Minute, 0)
        || _dateTime.AddMinutes(30) > new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, workHours.EndTime.Hour, workHours.EndTime.Minute, 0))
      {
        _timeSlotState = TimeSlotState.NotWorkTime;
      }
    }

    internal void UpdateState(WorkHours workHours)
    {
      TimeSlotState timeSlotState = IsSelected ? TimeSlotState.SelectedWorkTime : TimeSlotState.WorkTime;
      if (_dateTime.DayOfWeek == DayOfWeek.Saturday || _dateTime.DayOfWeek == DayOfWeek.Sunday)
      {
        timeSlotState = IsSelected ? TimeSlotState.SelectedNotWorkTime : TimeSlotState.NotWorkTime;
      }
      else if (_dateTime < new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, workHours.StartTime.Hour, workHours.StartTime.Minute, 0)
        || _dateTime.AddMinutes(30) > new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, workHours.EndTime.Hour, workHours.EndTime.Minute, 0))
      {
        timeSlotState = IsSelected ? TimeSlotState.SelectedNotWorkTime : TimeSlotState.NotWorkTime;
      }
      TimeSlotState = timeSlotState;
    }

    /// <summary>
    /// Gets whether the <see cref="TimeSlot"/> is selected.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      internal set
      {
        if (_isSelected == value)
        {
          return;
        }
        _isSelected = value;
        if (_isSelected)
        {
          if (_timeSlotState == TimeSlotState.WorkTime)
          {
            TimeSlotState = TimeSlotState.SelectedWorkTime;
          }
          else
          {
            TimeSlotState = TimeSlotState.SelectedNotWorkTime;
          }
        }
        else
        {
          if (_timeSlotState == TimeSlotState.SelectedWorkTime)
          {
            TimeSlotState = TimeSlotState.WorkTime;
          }
          else
          {
            TimeSlotState = TimeSlotState.NotWorkTime;
          }
        }
        OnPropertyChanged("IsSelected");
      }
    }

    /// <summary>
    /// Gets the time of the <see cref="TimeSlot"/>.
    /// </summary>
    public DateTime DateTime { get { return _dateTime; } }

    /// <summary>
    /// Gets the hour of the <see cref="TimeSlot"/>.
    /// </summary>
    public int Hour { get { return _dateTime.Hour; } }

    /// <summary>
    /// Gets the minute of the <see cref="TimeSlot"/>.
    /// </summary>
    public int Minute { get { return _dateTime.Minute; } }

    /// <summary>
    /// Gets the display status of the <see cref="TimeSlot"/>.
    /// </summary>
    public TimeSlotState TimeSlotState
    {
      get { return _timeSlotState; }
      private set { Set(ref _timeSlotState, value, "TimeSlotState"); }
    }

    internal bool IsLaterThan(TimeSlot other)
    {
      return CompareTo(other) > 0;
    }

    /// <summary>
    /// Compares this <see cref="TimeSlot"/> to another.
    /// </summary>
    /// <param name="other">The TimeSlot to compare against.</param>
    /// <returns>A negative value if this TimeSlot occurs before the other; a positive
    /// value if this TimeSlot occurs after the other; 0 if the two TimeSlots coincide.</returns>
    public int CompareTo(TimeSlot other)
    {
      return (int)((DateTime - other.DateTime).TotalSeconds);
    }
  }
}
