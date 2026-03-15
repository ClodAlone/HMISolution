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
  public class MonthModel : DateRangeViewModelBase, IScheduleViewModel
  {
    private IList<WeekModel> _weeks;
    private DateTime _startDate;
    private Schedule _calendar;
    private DayModel _selectedStartDay, _selectedEndDay;
    private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;
    
    public MonthModel(Schedule calendar, DateTime startDate)
    {
      _weeks = new List<WeekModel>();
      _calendar = calendar;
      StartDate = startDate;
      FirstDayOfWeek = _calendar.FirstDayOfWeek;

      SelectFirstDayOfMonth();
    }

    public bool Contains(DayModel day)
    {
      return day.Date.Month == StartDate.Month && day.Date.Year == StartDate.Year;
    }

    public DateTime StartDate
    {
      get { return _startDate; }
      private set
      {
        //TODO: check to make sure the given DateTime is at the start of the month.
        _startDate = value;
        UpdateWeeks();
      }
    }

    private void UpdateWeeks()
    {
      _weeks = new List<WeekModel>();
      DateTime weekStart = new DateTime(_startDate.Ticks).StartOfWeek(FirstDayOfWeek);
      do
      {
        _weeks.Add(new WeekModel(_calendar, weekStart, this));
        weekStart = new DateTime(weekStart.Ticks);
        weekStart = weekStart.AddDays(7);
      } while (weekStart.Month == _startDate.Month);
      OnPropertyChanged("Weeks");
    }

    internal DayOfWeek FirstDayOfWeek
    {
      get { return _firstDayOfWeek; }
      set
      {
        _firstDayOfWeek = value;
        UpdateWeeks();
      }
    }

    //TODO: maybe make a method that deselects all days. (probably won't be public though)

    public DayModel SelectedStartDay
    {
      get { return _selectedStartDay; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("The selected start day can not be null");
        }
        IList<DayModel> days = DayList;
        int selectedEndTimeIndex = days.IndexOf(_selectedEndDay);
        int currentSelectedStartTimeIndex = days.IndexOf(_selectedStartDay);
        int newSelectedStartTimeIndex = 0;
        if (value != null)
        {
          newSelectedStartTimeIndex = days.IndexOf(value);
        }
        int topIndex = Math.Min(selectedEndTimeIndex, Math.Min(currentSelectedStartTimeIndex, newSelectedStartTimeIndex));
        int bottomIndex = Math.Max(selectedEndTimeIndex, Math.Max(currentSelectedStartTimeIndex, newSelectedStartTimeIndex));
        int topSelection = Math.Min(selectedEndTimeIndex, newSelectedStartTimeIndex);
        int bottomSelection = Math.Max(selectedEndTimeIndex, newSelectedStartTimeIndex);

        for (int i = topIndex; i <= bottomIndex; i++)
        {
          days[i].IsSelected = i >= topSelection && i <= bottomSelection;
        }
        _selectedStartDay = value;
      }
    }

    public DayModel SelectedEndDay
    {
      get { return _selectedEndDay; }
      set
      {
        IList<DayModel> days = DayList;
        int selectedStartTimeIndex = days.IndexOf(_selectedStartDay);
        int currentSelectedEndTimeIndex = days.IndexOf(_selectedEndDay);
        int newSelectedEndTimeIndex = days.IndexOf(value);
        int topIndex = Math.Min(selectedStartTimeIndex, Math.Min(currentSelectedEndTimeIndex, newSelectedEndTimeIndex));
        int bottomIndex = Math.Max(selectedStartTimeIndex, Math.Max(currentSelectedEndTimeIndex, newSelectedEndTimeIndex));
        int topSelection = Math.Min(selectedStartTimeIndex, newSelectedEndTimeIndex);
        int bottomSelection = Math.Max(selectedStartTimeIndex, newSelectedEndTimeIndex);

        for (int i = topIndex; i <= bottomIndex; i++)
        {
          days[i].IsSelected = (i >= topSelection && i <= bottomSelection);
        }
        _selectedEndDay = value;
      }
    }

    public IList<DayModel> SelectedDays
    {
      get
      {
        IList<DayModel> days = new List<DayModel>();
        foreach (WeekModel week in _weeks)
        {
          foreach (DayModel day in week.Days)
          {
            if (day.IsSelected)
            {
              days.Add(day);
            }
          }
        }
        return days;
      }
    }

    public ReadOnlyCollection<WeekModel> Weeks
    {
      get { return new ReadOnlyCollection<WeekModel>(_weeks); }
    }

    public IList<DayModel> Days
    {
      get
      {
        IList<DayModel> days = new List<DayModel>();
        foreach (WeekModel week in _weeks)
        {
          foreach (DayModel day in week.Days)
          {
            days.Add(day);
          }
        }
        return days;
      }
    }

    private void SelectFirstDayOfMonth()
    {
      foreach (WeekModel week in _weeks)
      {
        foreach (DayModel d in week.Days) //TODO: null pointer checks? just in case...
        {
          if (DateTimeUtils.IsSameDay(d.Date, StartDate))
          {
            if (_selectedStartDay == null)
            {
              _selectedStartDay = d;
            }
            if (_selectedEndDay == null)
            {
              _selectedEndDay = d;
            }
            SelectedStartDay = d;
            SelectedEndDay = d;
          }
          else
          {
            d.IsSelected = false;
          }
        }
      }
    }

    private IList<DayModel> DayList
    {
      get
      {
        List<DayModel> days = new List<DayModel>();
        foreach (WeekModel week in _weeks)
        {
          foreach (DayModel day in week.Days)
          {
            days.Add(day);
          }
        }
        return days;
      }
    }

    #region IScheduleViewModel Members

    public override bool HasItems()
    {
      return _weeks.Any(w => w.HasItems());
    }

    #endregion

    protected override DateTime StartDateCore
    {
      get { return StartDate; }
    }

    protected override DateTime EndDateCore
    {
      get { return StartDate.EndOfMonth(); }
    }

    protected override DateDisplayMode DateRangeDisplayMode
    {
      get { return DateDisplayMode.Month; }
    }
  }
}
