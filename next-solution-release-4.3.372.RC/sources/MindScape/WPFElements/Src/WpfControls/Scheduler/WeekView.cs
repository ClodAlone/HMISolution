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
using System.Collections.ObjectModel;
using System.Collections.Generic;
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a week view in a <see cref="Scheduler"/> control.
  /// </summary>
  public class WeekView : ScheduleView
  {
    private WeekModel _week;
    private WeekViewMode _mode;
    private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeekView"/> class.
    /// </summary>
    public WeekView()
      : this(new Schedule(), DateTime.Now)
    {
    }

#if !SILVERLIGHT
    static WeekView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekView),
          new FrameworkPropertyMetadata(typeof(WeekView)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="WeekView"/> class.
    /// </summary>
    /// <param name="schedule">The <see cref="Schedule"/> to be displayed.</param>
    /// <param name="startDate">The start date for the display.</param>
    public WeekView(Schedule schedule, DateTime startDate)
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(WeekView);
#endif

      _mode = WeekViewMode.FullWeek;
      Schedule = schedule;
      Week = new WeekModel(Schedule, startDate);
      Week.DayCreated += Week_DayCreated;
      UpdateContainsToday();
#if SILVERLIGHT
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
#else
      Loaded += new RoutedEventHandler(WeekView_Loaded);
      Unloaded += new RoutedEventHandler(WeekView_Unloaded);
#endif
    }

#if !SILVERLIGHT
    private void WeekView_Loaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
    }

    private void WeekView_Unloaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick -= new EventHandler(Timer_Tick);
    }

    internal override void OnLanguageChanged()
    {
      base.OnLanguageChanged();

      OnPropertyChanged("Week");
    }
#endif

    private void Week_DayCreated(object sender, EventArgs e)
    {
      OnPropertyChanged("Week");
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      OnPropertyChanged("Now");
      UpdateContainsToday();
    }

    /// <summary>
    /// Gets whether the view contains the current day.
    /// </summary>
    public bool ContainsToday
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets whether to display the full week or the work week only.
    /// </summary>
    public WeekViewMode WeekViewMode
    {
      get { return _mode; }
      set
      {
        if (_mode == value) { return; }
        _mode = value;
        DayModel lastSelectedDay = Week.LastSelectedDay;
        Week = _week; //This will remove all the event handlers from all the days currently being displayed, and then sets the day count of the showing week.
        if (lastSelectedDay == null || lastSelectedDay.Date.DayOfWeek == DayOfWeek.Saturday || lastSelectedDay.Date.DayOfWeek == DayOfWeek.Sunday)
        {
          Week.DeselectAllDays();
          Week.Days[0].SelectedStartTime = Week.Days[0].TimeSlots[0];
          Week.Days[0].SelectedEndTime = Week.Days[0].TimeSlots[0];
        }
        EventHandler handler = WeekViewModeChanged;
        if (handler != null)
        {
          handler(this, EventArgs.Empty);
        }
      }
    }

    internal event EventHandler WeekViewModeChanged;

    private void UpdateDayCount()
    {
      switch (_mode)
      {
        case WeekViewMode.FullWeek:
          Week.StartDate = DateTimeUtils.StartOfWeek(Week.StartDate, _firstDayOfWeek);
          Week.DayCount = 7;
          foreach (DayModel day in Week.Days)
          {
            if (day.Date.DayOfWeek == DayOfWeek.Saturday || day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
              day.DeselectAll();
            }
          }
          //Week.Days[5].DeselectAll();
          //Week.Days[6].DeselectAll();
          //Week.DeselectAllDays();
          break;
        case WeekViewMode.WorkWeek:
          DateTime startDate = Week.StartDate;
          while (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
          {
            startDate = startDate.AddDays(1);
          }
          Week.StartDate = startDate;
          Week.DayCount = 5;
          break;
      }
    }

    internal DayOfWeek FirstDayOfWeek
    {
      get { return _firstDayOfWeek; }
      set
      {
        if (_firstDayOfWeek != value)
        {
          _firstDayOfWeek = value;
          UpdateDayCount();
          NotifyDateRangeChanged();
          Week.DeselectAllDays();
          Week.Days[0].SelectedStartTime = Week.Days[0].TimeSlots[0];
          Week.Days[0].SelectedEndTime = Week.Days[0].TimeSlots[0];
        }
      }
    }

    //TODO: should just check to see if the DateTime.Now fits between the start and end days of the week.
    //That would be faster.
    //make sure to remember that this WeekView may not always be showing all the days of the week.
    private void UpdateContainsToday()
    {
      bool nowContainsToday = false;
      foreach (DayModel day in Week.Days)
      {
        if (DateTimeUtils.IsSameDay(day.Date, DateTime.Now))
        {
          nowContainsToday = true;
          break;
        }
      }
      bool containedToday = ContainsToday;
      ContainsToday = nowContainsToday;
      if (containedToday != ContainsToday)
      {
        OnPropertyChanged("ContainsToday");
      }
    }

    /// <summary>
    /// Moves the view to the next week.
    /// </summary>
    public override void IncrementView()
    {
      if (Schedule.SelectedItem != null)
      {
        Schedule.SelectedItem.IsSelected = false;
      }
      MoveView(7);
    }

    /// <summary>
    /// Moves the view to the previous week.
    /// </summary>
    public override void DecrementView()
    {
      if (Schedule.SelectedItem != null)
      {
        Schedule.SelectedItem.IsSelected = false;
      }
      MoveView(-7);
    }

    private void MoveView(int delta)
    {
      Week = new WeekModel(Schedule, _week.StartDate.AddDays(delta));
      int index = 0;
      foreach (DayModel day in Week.Days)
      {
        if (index == 0)
        {
          day.SelectedStartTime = day.TimeSlots[0];
          day.SelectedEndTime = day.TimeSlots[0];
        }
        else
        {
          day.DeselectAll();
        }
        index++;
      }
    }

    internal override IScheduleViewModel ViewModel
    {
      get { return _week; }
    }

    /// <summary>
    /// Gets the end date of the view.
    /// </summary>
    protected override DateTime EndDate
    {
      get { return StartDate.AddDays(6); }
    }

    /// <summary>
    /// Gets a suitable start date for the control to display the requested date.
    /// </summary>
    /// <param name="date">The date to be displayed.</param>
    /// <returns>A start date for the view that will cause this date to be displayed.</returns>
    protected override DateTime GetStartDateContaining(DateTime date)
    {
      return date.StartOfWeek(_firstDayOfWeek);
    }

    /// <summary>
    /// Gets or sets the start date of the view.
    /// </summary>
    public override DateTime StartDate
    {
      get { return _week.StartDate; }
      set
      {
        Week = new WeekModel(Schedule, value);
      }
    }

    /// <summary>
    /// Called when the schedule of this <see cref="WeekView"/> changes.
    /// </summary>
    protected override void OnScheduleChanged()
    {
      if (_week != null)
      {
        Week = new WeekModel(Schedule, StartDate);
      }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

#if SILVERLIGHT
      Dispatcher.BeginInvoke(ScrollToStartOfDay);
#else
      Dispatcher.BeginInvoke(new Action(ScrollToStartOfDay));
#endif
    }

    private void ScrollToStartOfDay()
    {
      ScrollViewer sv = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
      if (sv != null)
      {
        sv.ScrollToVerticalOffset(480);  // 8 * HourSlotHeight  // TODO: de-constantise
      }
    }

    /// <summary>
    /// Gets the week being displayed.
    /// </summary>
    public WeekModel Week
    {
      get { return _week; }
      private set
      {
        if (_week != null)
        {
          foreach (DayModel day in _week.Days)
          {
            day.ItemsChanged -= Day_ItemsChanged;
          }
        }
        _week = value;
        UpdateDayCount();

        foreach (DayModel day in _week.Days)
        {
          day.ItemsChanged += Day_ItemsChanged;
        }

        UpdateContainsToday();
        OnPropertyChanged("Week");
        NotifyDateRangeChanged();
      }
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      NotifyDateRangeChanged();
    }

    /// <summary>
    /// Gets a <see cref="DateRange"/> containing the selected start and end dates of this <see cref="WeekView"/>.
    /// If a <see cref="ScheduleItem"/> is selected, then the start and end times of the item will be returned instead.
    /// </summary>
    public override DateRange SelectedDateRange
    {
      get
      {
        return _week.SelectedDateRange;
      }
    }

    /// <summary>
    /// Adds a new schedule item at the current selection.
    /// </summary>
    /// <returns>The new item.</returns>
    internal override ScheduleItem CreateScheduleItemForAdd()
    {
      ScheduleItem item = new ScheduleItem { Name = "New Appointment" };
      Week.AddScheduleItem(item);
      return item;
    }

    /// <summary>
    /// Cancels the addition of the specified item.
    /// </summary>
    /// <param name="item">The item to cancel.</param>
    protected override void CancelAdd(ScheduleItem item)
    {
      //Week.RemoveScheduleItem(item);
      Schedule.RemoveItem(item);
    }

    /// <summary>
    /// Gets how to format dates in the view title.
    /// </summary>
    protected override DateDisplayMode DateRangeDisplayMode
    {
      get { return DateDisplayMode.Day; }
    }
  }
}
