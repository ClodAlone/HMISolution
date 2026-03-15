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
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a month view in a <see cref="Scheduler"/> control.
  /// </summary>
  public class MonthView : ScheduleView
  {
    private MonthModel _month;
    private ReadOnlyCollection<string> _dayOfWeekNames;

#if !SILVERLIGHT
    static MonthView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthView),
        new FrameworkPropertyMetadata(typeof(MonthView)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="MonthView"/> class.
    /// </summary>
    public MonthView()
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(MonthView);
#endif
      Dispatcher.BeginInvoke(new Action<DayOfWeek>(UpdateDayOfWeekNames), DayOfWeek.Monday);
      
      //TODO: set the start date for the default constructor...somehow
    }

    internal override void OnLanguageChanged()
    {
      base.OnLanguageChanged();

      OnPropertyChanged("Month");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MonthView"/> class.
    /// </summary>
    /// <param name="schedule">The <see cref="Schedule"/> to be displayed.</param>
    public MonthView(Schedule schedule)
      : this()
    {
      Schedule = schedule;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MonthView"/> class.
    /// </summary>
    /// <param name="schedule">The <see cref="Schedule"/> to be displayed.</param>
    /// <param name="startDate">The start date for the display.</param>
    public MonthView(Schedule schedule, DateTime startDate)
      : this(schedule)
    {
      StartDate = startDate;
      //TODO: check to see if the given day is the start of a month.
    }

    /// <summary>
    /// Gets an ordered list of "day of week" names starting from the specified first day of week.
    /// </summary>
    public ReadOnlyCollection<string> DayOfWeekNames
    {
      get { return _dayOfWeekNames; }
      private set
      {
        _dayOfWeekNames = value;
        OnPropertyChanged("DayOfWeekNames");
      }
    }

    internal void UpdateDayOfWeekNames(DayOfWeek firstDayOfWeek)
    {
      IList<string> names = new List<string>();
      DateTime date = DateTimeUtils.StartOfWeek(DateTime.Now, firstDayOfWeek);
      //CultureInfo culture = Language.GetEquivalentCulture();
      CultureInfo culture = CultureInfo.CurrentCulture;
      for (int i = 0; i < 7; i++)
      {
        //names.Add(date.DayOfWeek.ToString());
        names.Add(date.ToString("dddd", culture));
        date = date.AddDays(1);
      }
      DayOfWeekNames = new ReadOnlyCollection<string>(names);
    }

    internal void SetFirstDayOfWeek(DayOfWeek firstDayOfWeek)
    {
      UpdateDayOfWeekNames(firstDayOfWeek);
      if (_month != null)
      {
        _month.FirstDayOfWeek = firstDayOfWeek;
      }
      NotifyDateRangeChanged();
    }

    /// <summary>
    /// Moves the view to the next month.
    /// </summary>
    public override void IncrementView()
    {
      Month = new MonthModel(Schedule, _month.StartDate.AddMonths(1));
    }

    /// <summary>
    /// Moves the view to the previous month.
    /// </summary>
    public override void DecrementView()
    {
      Month = new MonthModel(Schedule, _month.StartDate.AddMonths(-1));
    }

    internal override IScheduleViewModel ViewModel
    {
      get { return _month; }
    }

    /// <summary>
    /// Gets the end date of the view.
    /// </summary>
    protected override DateTime EndDate
    {
      get { return new DateTime(StartDate.Year, StartDate.Month, DateTime.DaysInMonth(StartDate.Year, StartDate.Month), 23, 59, 59); }
    }

    /// <summary>
    /// Gets the month being displayed.
    /// </summary>
    public MonthModel Month
    {
      get { return _month; }
      private set
      {
        if (_month != null)
        {
          foreach (DayModel day in _month.Days)
          {
            day.ItemsChanged -= Day_ItemsChanged;
          }
        }
        _month = value;
        foreach (DayModel day in _month.Days)
        {
          day.ItemsChanged += Day_ItemsChanged;
        }
        OnPropertyChanged("StartDate");
        OnPropertyChanged("Month");
        NotifyDateRangeChanged();
      }
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      NotifyDateRangeChanged();
    }

    /// <summary>
    /// Gets a suitable start date for the control to display the requested date.
    /// </summary>
    /// <param name="date">The date to be displayed.</param>
    /// <returns>A start date for the view that will cause this date to be displayed.</returns>
    protected override DateTime GetStartDateContaining(DateTime date)
    {
      return date.StartOfMonth();
    }

    /// <summary>
    /// Gets or sets the start date of the view.
    /// </summary>
    public override DateTime StartDate
    {
      get { return _month.StartDate; }
      set
      {
        //TODO: make sure given DateTime is at the start of a month!
        Month = new MonthModel(Schedule, value);
      }
    }

    /// <summary>
    /// Called when the schedule of this <see cref="MonthView"/> changes.
    /// </summary>
    protected override void OnScheduleChanged()
    {
      if (_month != null)
      {
        Month = new MonthModel(Schedule, StartDate);
      }
    }

    /// <summary>
    /// Adds a new schedule item at the current selection.
    /// </summary>
    /// <returns>The new item.</returns>
    internal override ScheduleItem CreateScheduleItemForAdd()
    {
      ScheduleItem item = new ScheduleItem { Name = "New Appointment" };
      DateRange range = SelectedDateRange;
      item.StartTime = range.StartDate;
      item.EndTime = range.EndDate;
      Schedule.AddItem(item);
      return item;
    }

    /// <summary>
    /// Gets a <see cref="DateRange"/> containing the selected start and end dates of this <see cref="MonthView"/>.
    /// If a <see cref="ScheduleItem"/> is selected, then the start and end times of the item will be returned instead.
    /// </summary>
    public override DateRange SelectedDateRange
    {
      get
      {
        DateTime start = new DateTime();
        DateTime end = new DateTime();
        if (Schedule.SelectedItem != null)
        {
          start = Schedule.SelectedItem.StartTime;
          end = Schedule.SelectedItem.EndTime;
        }
        else
        {
          if (Month.SelectedDays.Count == 0)
          {
            DayModel firstDay = Month.Days[0];
            foreach (DayModel dayModel in Month.Days)
            {
              if (dayModel.Date.Day == 1 && dayModel.Date.Month == Month.StartDate.Month)
              {
                firstDay = dayModel;
                break;
              }
            }
            Month.SelectedStartDay = firstDay;
            Month.SelectedEndDay = firstDay;
          }
          start = Month.SelectedStartDay.Date;
          end = Month.SelectedEndDay.Date;
          if (start > end)
          {
            start = end;
            end = Month.SelectedStartDay.Date;
          }
          end = end.AddHours(24);
        }
        return new DateRange(start, end);
      }
    }

    /// <summary>
    /// Cancels tha addition of the specified item.
    /// </summary>
    /// <param name="item">The item to cancel.</param>
    protected override void CancelAdd(ScheduleItem item)
    {
      /*foreach (var day in Month.Days)
      {
        day.RemoveScheduleItem(item);
      }*/
      Schedule.RemoveItem(item);
    }

    /// <summary>
    /// Gets how to format dates in the view title.
    /// </summary>
    protected override DateDisplayMode DateRangeDisplayMode
    {
      get { return DateDisplayMode.Month; }
    }
  }
}
