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
using System.Collections.ObjectModel;
using System.Linq;
#if SILVERLIGHT
namespace Mindscape.SilverlightElements.Internal
#else
namespace Mindscape.WpfElements.Internal
#endif
{
#pragma warning disable 1591  // XML documentation comments

  /// <summary>
  /// This class supports the <see cref="Scheduler"/> control and is not intended for use from your code.
  /// </summary>
  public class WeekModel : DateRangeViewModelBase, IScheduleViewModel
  {
    private DateTime _startDate;
    private IList<DayModel> _days;
    private Schedule _schedule;
    private DayModel _selectedStartDay, _selectedEndDay;

    private int _dayCount = 7;

    public WeekModel(Schedule schedule, DateTime startDate)
    {
      _schedule = schedule;
      //_schedule.DayCreated += new EventHandler<DayModelEventArgs>(Schedule_DayCreated);
      StartDate = startDate;

#if SILVERLIGHT
      _goToWeekCommand = new DelegateCommand<DependencyObject>(OnGoToWeek);
#endif
    }

    public WeekModel(Schedule calendar, DateTime startDate, MonthModel month)
      : this(calendar, startDate)
    {
      Month = month;
    }

    public DayModel SelectedStartDay
    {
      get { return _selectedStartDay; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("The selected start day can not be null");
        }
        _selectedStartDay = value;
        UpdateSelectionRange();
      }
    }

    public DayModel SelectedEndDay
    {
      get { return _selectedEndDay; }
      set
      {
        _selectedEndDay = value;
        UpdateSelectionRange();
      }
    }

    private void DeselectDays()
    {
      foreach (DayModel day in Days)
      {
        day.IsSelected = false;
      }
    }

    private void UpdateSelectionRange()
    {
      DeselectDays();
      if (SelectedStartDay != null && SelectedEndDay != null)
      {
        int startIndex = Days.IndexOf(SelectedStartDay);
        int endIndex = Days.IndexOf(SelectedEndDay);
        if (startIndex > -1 && endIndex > -1)
        {
          if (startIndex > endIndex)
          {
            int temp = startIndex;
            startIndex = endIndex;
            endIndex = temp;
          }
          for (int i = startIndex; i <= endIndex; i++)
          {
            Days[i].IsSelected = true;
          }
        }
      }
    }

    public event EventHandler DayCreated;

    protected virtual void OnDayCreated()
    {
      var handler = DayCreated;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /*private void Schedule_DayCreated(object sender, DayModelEventArgs e)
    {
      DayModel addedDay = e.Day;
      int index = 0;
      foreach (DayModel day in _days)
      {
        if (day.Date.Equals(addedDay.Date))
        {
          _days[index] = addedDay;
          OnDayCreated();
          OnPropertyChanged("Days");
          return;
        }
        index++;
      }
    }*/

    internal int DayCount
    {
      get { return _dayCount; }
      set
      {
        _dayCount = value;
        StartDate = _startDate; //To trigger recreation of the _day list.
        OnPropertyChanged("Days");
      }
    }

    public MonthModel Month
    {
      get;
      private set;
    }

    private IList<DayModel> SelectedDays
    {
      get
      {
        List<DayModel> days = new List<DayModel>();
        foreach (DayModel day in Days)
        {
          if (day.HasSelectedTimeSlot || day.IsSelected)
          {
            days.Add(day);
          }
        }
        return days;
      }
    }

    public DayModel FirstSelectedDay
    {
      get
      {
        foreach (DayModel day in _days)
        {
          if (day.HasSelectedTimeSlot)
          {
            return day;
          }
        }
        return null;
      }
    }

    public DayModel LastSelectedDay
    {
      get
      {
        for (int i = _days.Count - 1; i >= 0; i--)
        {
          if (_days[i].HasSelectedTimeSlot)
          {
            return _days[i];
          }
        }
        return null;
      }
    }

    public void DeselectAllDays()
    {
      foreach (DayModel day in _days)
      {
        day.DeselectAll();
      }
    }

    public void AddScheduleItem(ScheduleItem item)
    {
      DateRange range = SelectedDateRange;
      item.StartTime = range.StartDate;
      item.EndTime = range.EndDate;
      _schedule.AddItem(item);
    }

    internal DateRange SelectedDateRange
    {
      get
      {
        IList<DayModel> selectedDays = SelectedDays;
        if (selectedDays.Count == 0 && _schedule.SelectedItem != null)
        {
          return new DateRange(_schedule.SelectedItem.StartTime, _schedule.SelectedItem.EndTime);
        }

        if (selectedDays.Count == 0) // This is a safe guard, just in case there are no selected schedule items or days at all.
        {
          Days[0].SelectedStartTime = Days[0].TimeSlots[0];
          Days[0].SelectedEndTime = Days[0].TimeSlots[0];
          selectedDays.Add(Days[0]);
        }
        DayModel day = selectedDays[0];
        DateTime d = day.Date;
        DateTime start = new DateTime(d.Year, d.Month, d.Day, day.SelectedStartTime.Hour, day.SelectedStartTime.Minute, 0);
        if (!day.HasSelectedTimeSlot)
        {
          day.SelectedStartTime = day.TimeSlots[0];
          day.SelectedEndTime = day.TimeSlots.Last();
          start = new DateTime(d.Year, d.Month, d.Day, day.SelectedStartTime.Hour, day.SelectedStartTime.Minute, 0);
          day.DeselectAll();
        }
        else if (day.SelectedStartTime.IsLaterThan(day.SelectedEndTime))
        {
          start = new DateTime(d.Year, d.Month, d.Day, day.SelectedEndTime.Hour, day.SelectedEndTime.Minute, 0);
        }


        day = selectedDays[selectedDays.Count - 1];
        d = day.Date;
        DateTime end = end = new DateTime(d.Year, d.Month, d.Day, day.SelectedEndTime.Hour, day.SelectedEndTime.Minute, 0).AddMinutes(30);
        if (!day.HasSelectedTimeSlot)
        {
          day.SelectedStartTime = day.TimeSlots[0];
          day.SelectedEndTime = day.TimeSlots.Last();
          end = end = new DateTime(d.Year, d.Month, d.Day, day.SelectedEndTime.Hour, day.SelectedEndTime.Minute, 0).AddMinutes(30);
          day.DeselectAll();
        }
        if (day.SelectedStartTime.IsLaterThan(day.SelectedEndTime))
        {
          end = new DateTime(d.Year, d.Month, d.Day, day.SelectedStartTime.Hour, day.SelectedStartTime.Minute, 0).AddMinutes(30);
        }
        return new DateRange(start, end);
      }
    }

    internal void RemoveScheduleItem(ScheduleItem item)
    {
      foreach (DayModel day in Days)
      {
        day.RemoveScheduleItem(item);
      }
    }

    public DateTime StartDate
    {
      get { return _startDate; }
      internal set
      {
        _startDate = value;
        _days = new List<DayModel>();
        DateTime day = new DateTime(_startDate.Ticks);
        for (int i = 0; i < _dayCount; i++)
        {
          DayModel newDay = _schedule.GetDay(day);
          _days.Add(newDay);
          day = new DateTime(day.Ticks);
          day = day.AddDays(1);
        }
      }
    }

    public DateTime EndDate
    {
      get
      {
        return _startDate.AddDays(6);
      }
    }

    public ReadOnlyCollection<DayModel> Days
    {
      get
      {
        return new ReadOnlyCollection<DayModel>(_days);
      }
    }

    public override bool HasItems()
    {
      return _days.Any(d => d.HasItems());
    }

#if SILVERLIGHT
    // TODO: horrible horrible hack

    private ICommand _goToWeekCommand;

    public ICommand GoToWeekCommand
    {
      get { return _goToWeekCommand; }
    }

    private void OnGoToWeek(DependencyObject sender)
    {
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(sender);
      if (scheduler != null)
      {
        scheduler.SwitchToWeek(StartDate);
      }
    }
#endif

    protected override DateTime StartDateCore
    {
      get { return StartDate; }
    }

    protected override DateTime EndDateCore
    {
      get { return EndDate; }
    }

    protected override DateDisplayMode DateRangeDisplayMode
    {
      get { return DateDisplayMode.Day; }
    }
  }
}
