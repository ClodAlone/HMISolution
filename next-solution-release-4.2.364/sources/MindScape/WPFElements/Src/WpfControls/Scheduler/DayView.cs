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
using System.ComponentModel;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a single-day view in a <see cref="Scheduler"/> control.
  /// </summary>
  public class DayView : ScheduleView
  {
    private DayModel _day;

#if !SILVERLIGHT
    static DayView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DayView),
        new FrameworkPropertyMetadata(typeof(DayView)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="DayView"/> class.
    /// </summary>
    public DayView()
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(DayView);
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
#else
      Loaded += new RoutedEventHandler(DayView_Loaded);
      Unloaded += new RoutedEventHandler(DayView_Unloaded);
#endif

      Schedule = new Schedule();
      StartDate = DateTime.Now;
    }

#if !SILVERLIGHT
    private void DayView_Loaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
    }

    private void DayView_Unloaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick -= new EventHandler(Timer_Tick);
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="DayView"/> class.
    /// </summary>
    /// <param name="schedule">The <see cref="Schedule"/> to be displayed.</param>
    /// <param name="startDate">The start date for the display.</param>
    public DayView(Schedule schedule, DateTime startDate)
      : this()
    {
      Schedule = schedule;
      StartDate = startDate;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      OnPropertyChanged("Now");
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
    /// Gets a list containing the day being represented.
    /// </summary>
    /// <remarks>This member is used for binding.</remarks>
    public IList<DayModel> Days
    {
      get
      {
        IList<DayModel> list = new List<DayModel>();
        list.Add(Day);
        return list;
      }
    }

    /// <summary>
    /// Gets the day being displayed.
    /// </summary>
    public DayModel Day
    {
      get { return _day; }
      private set
      {
        if (_day != null)
        {
          _day.ItemsChanged -= Day_ItemsChanged;
        }
        _day = value;

        _day.ItemsChanged += Day_ItemsChanged;

        //TODO: yeah, these should be moved out:
        OnPropertyChanged("Day");
        OnPropertyChanged("Days");
        OnPropertyChanged("StartDate");
        OnPropertyChanged("TimeSlots");
        NotifyDateRangeChanged();
      }
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      NotifyDateRangeChanged();
    }

    /// <summary>
    /// Gets the collection of timeslots to be displayed in the view.
    /// </summary>
    public ReadOnlyCollection<TimeSlot> TimeSlots
    {
      get
      {
        return Day.TimeSlots;
      }
    }

    /// <summary>
    /// Moves the view to the next day.
    /// </summary>
    public override void IncrementView()
    {
      StartDate = StartDate.AddDays(1);
      Day.SelectedStartTime = Day.TimeSlots[0];
      Day.SelectedEndTime = Day.TimeSlots[0];
    }

    /// <summary>
    /// Moves the view to the previous day.
    /// </summary>
    public override void DecrementView()
    {
      StartDate = StartDate.AddDays(-1);
      Day.SelectedStartTime = Day.TimeSlots[0];
      Day.SelectedEndTime = Day.TimeSlots[0];
    }

    internal override IScheduleViewModel ViewModel
    {
      get { return _day; }
    }

    /// <summary>
    /// Gets the end date of the view.
    /// </summary>
    protected override DateTime EndDate
    {
      get { return StartDate.AddHours(24); }
    }

    /// <summary>
    /// Gets a suitable start date for the control to display the requested date.
    /// </summary>
    /// <param name="date">The date to be displayed.</param>
    /// <returns>A start date for the view that will cause this date to be displayed.</returns>
    protected override DateTime GetStartDateContaining(DateTime date)
    {
      return date.Date;
    }

    /// <summary>
    /// Gets or sets the start date of the view.
    /// </summary>
    public override DateTime StartDate
    {
      get { return _day.Date; }
      set
      {
        Day = Schedule.GetDay(value);
        PropertyChangedEventHandler handler = StartDateChanged;
        if (handler != null)
        {
          handler(this, new PropertyChangedEventArgs("StartDate"));
        }
      }
    }

    /// <summary>
    /// Raised whenever the start date of this <see cref="DayView"/> changes.
    /// </summary>
    public event PropertyChangedEventHandler StartDateChanged;

    /// <summary>
    /// Called when the schedule of this <see cref="DayView"/> changes.
    /// </summary>
    protected override void OnScheduleChanged()
    {
      if (_day != null)
      {
        Day = Schedule.GetDay(StartDate);
      }
    }

    /// <summary>
    /// Adds a new schedule item at the current selection.
    /// </summary>
    /// <returns>The new item.</returns>
    internal override ScheduleItem CreateScheduleItemForAdd()
    {
      ScheduleItem scheduleItem = new ScheduleItem { Name = "New Appointment" };
      DateRange range = SelectedDateRange;
      scheduleItem.StartTime = range.StartDate;
      scheduleItem.EndTime = range.EndDate;
      Schedule.AddItem(scheduleItem);
      return scheduleItem;
    }

    /// <summary>
    /// Gets a <see cref="DateRange"/> containing the selected start and end dates of this <see cref="DayView"/>.
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
          DateTime date = Day.Date;
          TimeSlot selectedStartTime = Day.SelectedStartTime;
          TimeSlot selectedEndTime = Day.SelectedEndTime;
          start = new DateTime(date.Year, date.Month, date.Day, selectedStartTime.Hour, selectedStartTime.Minute, 0);
          end = new DateTime(date.Year, date.Month, date.Day, selectedEndTime.Hour, selectedEndTime.Minute, 0).AddMinutes(30);
          if (start.Ticks > end.Ticks)
          {
            start = new DateTime(date.Year, date.Month, date.Day, selectedEndTime.Hour, selectedEndTime.Minute, 0);
            end = new DateTime(date.Year, date.Month, date.Day, selectedStartTime.Hour, selectedStartTime.Minute, 0).AddMinutes(30);
          }
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
      //Day.RemoveScheduleItem(item);
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
