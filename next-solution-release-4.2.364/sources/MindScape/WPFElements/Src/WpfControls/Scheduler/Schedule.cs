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
using System.Diagnostics;
using System.Collections.Specialized;
using System.Linq;
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
  /// Contains schedule data for display in a <see cref="Scheduler"/> control.
  /// </summary>
  public class Schedule
  {
#if TRIAL
    internal const int MaxItems = 8;
#else
    internal const int MaxItems = Int32.MaxValue;
#endif

    private static DateTime _lastAlertedLicenseExceeded;

    internal static void AlertLicenseExceeded(bool immediate)
    {
      bool recentlyNotified = (DateTime.UtcNow - _lastAlertedLicenseExceeded).TotalMinutes <= 5;

      if (immediate || !recentlyNotified)
      {
        MessageBox.Show("The trial edition of Mindscape Silverlight Elements limits the scheduler to eight items.  You cannot currently add a new item.");
        _lastAlertedLicenseExceeded = DateTime.UtcNow;
      }
    }

    private Dictionary<long, DayModel> _days;
    private Dictionary<ScheduleItem, IList<DayModel>> _itemDayMap;

    //private ScheduleItem _lastAddedItem;
    private readonly List<ScheduleItem> _recurringItems = new List<ScheduleItem>();  // TODO: Yuck
    private ScheduleItem _lastSelectedItem;

    private ScheduleItem _earliestItem;
    private ScheduleItem _latestItem;

    
    private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;

    /// <summary>
    /// Initializes a new instance of the <see cref="Schedule"/> class.
    /// </summary>
    public Schedule()
    {
      _days = new Dictionary<long, DayModel>();
      _itemDayMap = new Dictionary<ScheduleItem, IList<DayModel>>();
      WorkHours = new WorkHours();
#if SILVERLIGHT
      _currentDay = DateTime.Now;
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
#endif
    }

    internal DayOfWeek FirstDayOfWeek
    {
      get { return _firstDayOfWeek; }
      set
      {
        _firstDayOfWeek = value;
      }
    }

#if SILVERLIGHT
    private DateTime _currentDay;

    private void Timer_Tick(object sender, EventArgs e)
    {
      if (!DateTimeUtils.IsSameDay(_currentDay, DateTime.Now))
      {
        GetDay(_currentDay).UpdateIsCurrentDay();
        _currentDay = DateTime.Now;
        GetDay(_currentDay).UpdateIsCurrentDay();
      }
    }
#endif

    /// <summary>
    /// Gets or sets an <see cref="IScheduleItemBuilder"/> for creating and adding custom schedule items to the <see cref="Schedule"/>.
    /// </summary>
    public IScheduleItemBuilder ScheduleItemBuilder { get; set; }

    /// <summary>
    /// Adds a new item to the schedule.
    /// </summary>
    /// <param name="scheduleItem">The item to be added.</param>
    public void AddItem(ScheduleItem scheduleItem)
    {
      if (Items.Count > MaxItems)
      {
        AlertLicenseExceeded(false);
        return;
      }

      if (scheduleItem.IsRecurring)
      {
        AddRecurrence(scheduleItem);
      }
      else
      {
        DateTime date = scheduleItem.StartTime.Date;
        while (date < scheduleItem.EndTime)
        {
          DayModel day = GetDay(date);
          day.AddScheduleItem(scheduleItem);
          date = date.AddDays(1);
        }
        OnItemAdded(scheduleItem);
      }
    }

    internal void SilentlyAddItem(ScheduleItem scheduleItem)
    {
      if (Items.Count > MaxItems)
      {
        AlertLicenseExceeded(false);
        return;
      }

      DateTime date = scheduleItem.StartTime.Date;
      while (date < scheduleItem.EndTime)
      {
        DayModel day = GetDay(date);
        day.AddScheduleItem(scheduleItem);
        date = date.AddDays(1);
      }
    }

    internal AddScheduleItemEventArgs AddItemReturnArgs(ScheduleItem item)
    {
      AddItem(item);
      return _lastAddItemArgs;
    }

    /// <summary>
    /// Gets the collection of items in the <see cref="Schedule"/>.
    /// </summary>
    public ReadOnlyCollection<ScheduleItem> Items
    {
      get
      {
        return _itemDayMap.Keys.Where(si => !si.IsInstanceOfRecurringItem).Concat(_recurringItems).ToList().AsReadOnly();
      }
    }

    internal DateTime EarliestItemDate
    {
      get
      {
        if (_earliestItem == null)
        {
          return DateTime.MaxValue;
        }
        return _earliestItem.StartTime;
      }
    }

    private ScheduleItem EarliestItem
    {
      get { return _earliestItem; }
      set
      {
        _earliestItem = value;
      }
    }

    internal DateTime LatestItemDate
    {
      get
      {
        if (_latestItem == null)
        {
          return DateTime.MinValue;
        }
        return _latestItem.EndTime;
      }
    }

    private ScheduleItem LatestItem
    {
      get { return _latestItem; }
      set
      {
        _latestItem = value;
      }
    }

    private bool HasItems
    {
      get { return _itemDayMap.Count > 0; }
    }

    internal ScheduleItem GetNextEarliestItem(DateTime time)
    {
      if (time < EarliestItemDate)
      {
        time = EarliestItemDate;
      }
      DayModel day = GetDay(time);
      ScheduleItem result = day.EarliestItem;
      while (result == null)
      {
        if (day.Date.Year == 9999)
        {
          return null;
        }
        day = GetDay(day.Date.AddHours(24));
        result = day.EarliestItem;
      }
      return result;
    }

    internal ScheduleItem GetPreviousEarliestItem(DateTime time)
    {
      if (LatestItemDate < time)
      {
        time = LatestItemDate;
      }
      DayModel day = GetDay(time);
      ScheduleItem result = day.EarliestItem;
      while (result == null)
      {
        if (day.Date.Year == 1)
        {
          return null;
        }
        day = GetDay(day.Date.AddHours(-24));
        result = day.EarliestItem;
      }
      return result;
    }

    internal ScheduleItem GetPreviousLatestItem(DateTime time)
    {
      if (LatestItemDate < time)
      {
        time = LatestItemDate;
      }
      DayModel day = GetDay(time);
      ScheduleItem result = day.LatestItem;
      while (result == null)
      {
        if (day.Date.Year == 1)
        {
          return null;
        }
        day = GetDay(day.Date.AddHours(-24));
        result = day.LatestItem;
      }
      return result;
    }

    private void AddRecurrence(ScheduleItem item)
    {
      Debug.Assert(item.IsRecurring);

      _recurringItems.Add(item);
      item.RecurrenceInfo.RecurrencePatternSkipDatesChanged += new NotifyCollectionChangedEventHandler(RecurrencePattern_SkipDatesChanged);

      RecurrenceInfo info = item.RecurrenceInfo;

      if (info.EndType == RecurrenceEndType.NoEndDate)
      {
        //TODO: make a dummy appointment that represents an appointment that is infinte days in the future.
      }

      List<DayModel> days = new List<DayModel>(_days.Values);
      foreach (DayModel day in days)
      {
        AddRecurringItemInstanceToDay(item, day);
      }
    }

    /// <summary>
    /// Occurs when the contents of the skip dates collection of any of the recurrence patterns change.
    /// </summary>
    public event NotifyCollectionChangedEventHandler RecurrencePatternSkipDatesChanged;

    private void RecurrencePattern_SkipDatesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // If the info uses the max recurrences, and the number of skip dates now equals the max recurrences of the info,
      // then the recurrence pattern is now expended and should be removed. this is done here:
      RecurrenceInfo info = sender as RecurrenceInfo;
      if (info.EndType == RecurrenceEndType.EndAfter && info.RecurrencePattern.SkipDates.Count == info.MaxOccurrences)
      {
        info.RecurrencePatternSkipDatesChanged -= new NotifyCollectionChangedEventHandler(RecurrencePattern_SkipDatesChanged);
        ScheduleItem recurrenceMasterToRemove = null;
        foreach (ScheduleItem scheduleItem in _recurringItems)
        {
          if (scheduleItem.RecurrenceInfo == info)
          {
            recurrenceMasterToRemove = scheduleItem;
            break;
          }
        }
        Debug.Assert(recurrenceMasterToRemove != null);
        _recurringItems.Remove(recurrenceMasterToRemove);
        OnRecurrencePatternRemoved(recurrenceMasterToRemove);
      } // TODO: need to remove the recurrence pattern in the case where it ends by a particular date and all possible items have been individually removed.
      else
      {
        NotifyCollectionChangedEventHandler handler = RecurrencePatternSkipDatesChanged;
        if (handler != null)
        {
          handler(sender, e);
        }
      }
    }

    private void AddRecurringItemInstanceToDay(ScheduleItem item, DayModel day)
    {
      Debug.Assert(item.IsRecurring);

      RecurrenceInfo info = item.RecurrenceInfo;

      if (info.Includes(day))
      {
        ScheduleItem instanceOfRecurrence = item.CreateInstance(day.Date);
        DateTime date = instanceOfRecurrence.StartTime.Date;
        while (date < instanceOfRecurrence.EndTime)
        {
          DayModel d = GetDay(date);

          if (!d.ScheduleItems.Contains(instanceOfRecurrence))
          {
            d.AddScheduleItem(instanceOfRecurrence);
          }
          date = date.AddDays(1);
        }
        OnItemAdded(instanceOfRecurrence);
      }
    }

    private WorkHours _workHours;

    /// <summary>
    /// Gets or sets the work hours of the <see cref="Schedule"/>. The default is 9:00am - 5:00pm
    /// </summary>
    public WorkHours WorkHours
    {
      get { return _workHours; }
      set
      {
        _workHours = value;
        foreach (DayModel day in _days.Values)
        {
          day.UpdateTimeSlots(WorkHours);
        }
      }
    }

    /// <summary>
    /// Gets a <see cref="DayModel"/> for the given date.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> for selecting the <see cref="DayModel"/>.</param>
    /// <returns>The <see cref="DayModel"/> for the given date.</returns>
    public DayModel GetDay(DateTime date)
    {
      DateTime dateTime = date.Date;
      DayModel day;
      _days.TryGetValue(dateTime.Ticks, out day);
      if (day == null)
      {
        day = new DayModel(dateTime, WorkHours);
        day.ItemsChanged += Day_ItemsChanged;
        _days[day.Date.Ticks] = day;

        foreach (ScheduleItem recurringItem in _recurringItems)
        {
          AddRecurringItemInstanceToDay(recurringItem, day);
        }
      }
      return day;
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        NotifyScheduleItemAdded((DayModel)sender, e.Item, e.IsLong);

        if (Items.Count > MaxItems)
        {
          AlertLicenseExceeded(false);
          System.Windows.Threading.DispatcherSynchronizationContext.Current.Post(o => ((DayModel)sender).RemoveScheduleItem(e.Item), null);
        }
      }
    }

    //internal event EventHandler<DayModelEventArgs> DayCreated;

    //private void OnDayCreated(DayModel day)
    //{
    //  var handler = DayCreated;
    //  if (handler != null)
    //  {
    //    handler(this, new DayModelEventArgs(day));
    //  }
    //}

    /// <summary>
    /// Removes an item from the schedule.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    public void RemoveItem(ScheduleItem item)
    {
      RemoveItem(item, false);
    }

    /// <summary>
    /// Removes an item from the schedule.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <param name="removeAllOccurrences">true to remove all occurrences of a recurring item;
    /// false to remove only the specified item.</param>
    public void RemoveItem(ScheduleItem item, bool removeAllOccurrences)
    {
      if (removeAllOccurrences && item.IsInstanceOfRecurringItem)
      {
        item.InstanceOf.RecurrenceInfo.RecurrencePatternSkipDatesChanged -= new NotifyCollectionChangedEventHandler(RecurrencePattern_SkipDatesChanged);
      }

      JustRemoveAppointment(item);

      if (removeAllOccurrences && item.IsInstanceOfRecurringItem)
      {
        //TODO: find a better way to know what appointments belong to a particular recurrence info, rather than hunting them all down.
        List<ScheduleItem> toRemove = new List<ScheduleItem>();
        foreach (ScheduleItem app in _itemDayMap.Keys)
        {
          if (app.IsInstanceOfRecurringItem && app.InstanceOf.RecurrenceInfo == item.InstanceOf.RecurrenceInfo)
          {
            toRemove.Add(app);
          }
        }
        foreach (ScheduleItem app in toRemove)
        {
          JustRemoveAppointment(app);
        }
        _recurringItems.Remove(item.InstanceOf);
        OnRecurrencePatternRemoved(item.InstanceOf);
      }
    }

    /// <summary>
    /// Occurs when a recurrence pattern is removed from the schedule.
    /// </summary>
    public event EventHandler<ScheduleItemEventArgs> RecurrencePatternRemoved;

    private void OnRecurrencePatternRemoved(ScheduleItem item)
    {
      Debug.Assert(item.IsRecurring);
      EventHandler<ScheduleItemEventArgs> handler = RecurrencePatternRemoved;
      if (handler != null)
      {
        handler(this, new ScheduleItemEventArgs(item));
      }
    }

    /// <summary>
    /// Removes all items from the schedule.
    /// </summary>
    public void Clear()
    {
      EarliestItem = null;
      LatestItem = null;
      while (_recurringItems.Count > 0)
      {
        RemoveItem(_recurringItems[0], true);
      }
      while (_itemDayMap.Keys.Count > 0)
      {
        RemoveItem(_itemDayMap.Keys.First(), true);
      }
    }

    //Ignores removing all appointments in a recurrence if neccessary. That is, just performs the appointment removal.
    private void JustRemoveAppointment(ScheduleItem item)
    {
      if (SelectedItem == item)
      {
        SelectedItem = null;
      }
      IList<DayModel> days = GetDays(item.StartTime, item.EndTime);
      foreach (DayModel day in days)
      {
        day.RemoveScheduleItem(item);
      }
      UnhookScheduleItemEvents(item);
      _itemDayMap.Remove(item);
      _recurringItems.Remove(item);

      if (item.IsInstanceOfRecurringItem)
      {
        item.InstanceOf.RecurrenceInfo.RecurrencePattern.SkipDates.Add(item.StartTime);
      }

      if (item == EarliestItem)
      {
        if (!HasItems)
        {
          EarliestItem = null;
          LatestItem = null;
          OnItemRemoved(item);
          return;
        }
        EarliestItem = GetNextEarliestItem(EarliestItemDate);
      }
      if (item == LatestItem)
      {
        if (!HasItems)
        {
          EarliestItem = null;
          LatestItem = null;
          OnItemRemoved(item);
          return;
        }
        LatestItem = GetPreviousLatestItem(LatestItemDate);
      }
      OnItemRemoved(item);
    }

    internal void SilentlyRemoveItem(ScheduleItem item)
    {
      if (SelectedItem == item)
      {
        SelectedItem = null;
      }
      IList<DayModel> days = GetDays(item.StartTime, item.EndTime);
      foreach (DayModel day in days)
      {
        day.RemoveScheduleItem(item);
      }
      UnhookScheduleItemEvents(item);
      _itemDayMap.Remove(item);
      _recurringItems.Remove(item);

      if (item.IsInstanceOfRecurringItem)
      {
        item.InstanceOf.RecurrenceInfo.RecurrencePattern.SkipDates.Add(item.StartTime);
      }

      if (item == EarliestItem)
      {
        if (!HasItems)
        {
          EarliestItem = null;
          LatestItem = null;
          OnItemRemoved(item);
          return;
        }
        EarliestItem = GetNextEarliestItem(EarliestItemDate);
      }
      if (item == LatestItem)
      {
        if (!HasItems)
        {
          EarliestItem = null;
          LatestItem = null;
          OnItemRemoved(item);
          return;
        }
        LatestItem = GetPreviousLatestItem(LatestItemDate);
      }
    }

    /// <summary>
    /// Occurs when the user deletes a schedule item from the <see cref="Schedule"/>.
    /// </summary>
    public event EventHandler<ScheduleItemEventArgs> ItemRemoved;

    private void OnItemRemoved(ScheduleItem item)
    {
      EventHandler<ScheduleItemEventArgs> handler = ItemRemoved;
      if (handler != null)
      {
        handler(this, new ScheduleItemEventArgs(item));
      }
    }

    /// <summary>
    /// Occurs when the user adds a schedule item to the <see cref="Schedule"/>.
    /// </summary>
    public event EventHandler<AddScheduleItemEventArgs> ItemAdded;

    private AddScheduleItemEventArgs _lastAddItemArgs;

    private void OnItemAdded(ScheduleItem item)
    {
      AddScheduleItemEventArgs args = new AddScheduleItemEventArgs(item);
      args.ShowDefaultEditor = true;
      _lastAddItemArgs = args;
      EventHandler<AddScheduleItemEventArgs> handler = ItemAdded;
      if (handler != null)
      {
        args.ShowDefaultEditor = false;
        handler(this, args);
        if (args.Cancel)
        {
          RemoveItem(item);
        }
      }
    }

    private void HookScheduleItemEvents(ScheduleItem item)
    {
      item.StartTimeChanged += ScheduleItem_StartTimeChanged;
      item.EndTimeChanged += ScheduleItem_EndTimeChanged;
      item.IsSelectedChanged += ScheduleItem_IsSelectedChanged;
      item.RecurrenceChanged += ScheduleItem_RecurrenceChanged;
    }

    private void UnhookScheduleItemEvents(ScheduleItem item)
    {
      item.StartTimeChanged -= ScheduleItem_StartTimeChanged;
      item.EndTimeChanged -= ScheduleItem_EndTimeChanged;
      item.IsSelectedChanged -= ScheduleItem_IsSelectedChanged;
      item.RecurrenceChanged -= ScheduleItem_RecurrenceChanged;
    }

    private void ScheduleItem_IsSelectedChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = (ScheduleItem)sender;
      if (scheduleItem.IsSilent && scheduleItem.IsSelected)
      {
        SelectedItem = null;
      }
      else
      {
        if (scheduleItem.IsSelected)
        {
          SelectedItem = scheduleItem;
        }
        else if (SelectedItem == scheduleItem)
        {
          SelectedItem = null;
        }
      }
    }

    private void ScheduleItem_RecurrenceChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = (ScheduleItem)sender;
      _itemDayMap.Remove(scheduleItem);
      _recurringItems.Remove(scheduleItem);
      OnItemRemoved(scheduleItem);

      NotifyScheduleItemAdded(null, scheduleItem, false);
    }

    private void NotifyScheduleItemAdded(DayModel dayModel, ScheduleItem item, bool reconfigureDays)
    {
      if (item.IsRecurring)
      {
        if (dayModel != null)
        {
          dayModel.RemoveScheduleItem(item);
        }
        AddRecurrence(item);
        return;
      }

      if (!_itemDayMap.Keys.Contains(item))
      {
        HookScheduleItemEvents(item);
        if (!reconfigureDays)
        {
          _itemDayMap[item] = GetDays(item.StartTime, item.EndTime);
        }
      }

      //_lastAddedItem = item;

      if (reconfigureDays)
      {
        ReconfigureDays(item);
      }

      UpdateEarliestAndLatestItems(item);
    }

    private void ScheduleItem_StartTimeChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = (ScheduleItem)sender;
      ReconfigureDays(scheduleItem);
      if (scheduleItem == EarliestItem)
      {
        EarliestItem = GetPreviousEarliestItem(EarliestItemDate);
      }
      else
      {
        if (EarliestItemDate == null)
        {
          EarliestItem = scheduleItem;
        }
        if (scheduleItem.StartTime < EarliestItemDate)
        {
          EarliestItem = scheduleItem;
        }
      }
    }

    private void ScheduleItem_EndTimeChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = (ScheduleItem)sender;
      ReconfigureDays(scheduleItem);
      if (scheduleItem == LatestItem)
      {
        LatestItem = GetPreviousLatestItem(LatestItemDate);
      }
      else
      {
        if (LatestItemDate == null)
        {
          LatestItem = scheduleItem;
        }
        if (LatestItemDate < scheduleItem.EndTime)
        {
          LatestItem = scheduleItem;
        }
      }
    }

    /// <summary>
    /// Gets the currently selected schedule item.
    /// </summary>
    public ScheduleItem SelectedItem
    {
      get { return _lastSelectedItem; }
      private set
      {
        _lastSelectedItem = value;
        OnSelectedItemChanged();
      }
    }

    private void UpdateEarliestAndLatestItems(ScheduleItem addedItem)
    {
      if (LatestItemDate == null || LatestItemDate < addedItem.EndTime)
      {
        LatestItem = addedItem;
      }
      if (EarliestItemDate == null || addedItem.StartTime < EarliestItemDate)
      {
        EarliestItem = addedItem;
      }
    }

    private void ReconfigureDays(ScheduleItem scheduleItem)
    {
      IList<DayModel> days = GetDays(scheduleItem.StartTime, scheduleItem.EndTime);
      IList<DayModel> oldDays;
      _itemDayMap.TryGetValue(scheduleItem, out oldDays);
      IList<DayModel> newDays = new List<DayModel>();
      if (oldDays != null)
      {
        foreach (DayModel day in days)
        {
          if (oldDays.Contains(day))
          {
            oldDays.Remove(day);
          }
          else
          {
            newDays.Add(day);
          }
        }
        foreach (DayModel day in oldDays)
        {
          day.RemoveScheduleItem(scheduleItem);
        }
        foreach (DayModel day in newDays)
        {
          day.AddScheduleItem(scheduleItem);
          //_lastAddedItem = scheduleItem;
        }
      }
      _itemDayMap[scheduleItem] = days;
    }

    private IList<DayModel> GetDays(DateTime startDate, DateTime endDate)
    {
      List<DayModel> days = new List<DayModel>();
      DateTime day = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

      do
      {
        DayModel d = GetDay(day);
        days.Add(d);
        day = day.AddDays(1);
      }
      while (day < endDate);

      return days;
    }

    internal event EventHandler SelectedItemChanged;

    internal void OnSelectedItemChanged()
    {
      var handler = SelectedItemChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }
  }
}
