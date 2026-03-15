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
using System.Collections.Specialized;
using System.ComponentModel;

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
  public class DayModel : DateRangeViewModelBase, IScheduleViewModel
  {
    private readonly DateTime _date;
    private readonly ReadOnlyCollection<TimeSlot> _timeSlots;

    private TimeSlot _selectedStartTime;
    private TimeSlot _selectedEndTime;
    private IList<ScheduleItem> _shortItems;
    private IList<ScheduleItem> _longItems;
    private IList<ScheduleItem> _selectedItems;
    private bool _hasSelectedTimeSlot;
    private bool _isSelected;

    public DayModel(DateTime date, WorkHours workHours)
    {
      _shortItems = new List<ScheduleItem>();
      _longItems = new List<ScheduleItem>();
      _selectedItems = new List<ScheduleItem>();
      _date = date;

#if SILVERLIGHT
      _goToDayCommand = new DelegateCommand<DependencyObject>(OnGoToDay);
#endif

      DateTime time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
      List<TimeSlot> timeSlots = new List<TimeSlot>();
      for (int i = 0; i < 48; i++)
      {
        timeSlots.Add(new TimeSlot(time, workHours));
        time = time.AddMinutes(30);
      }
      _timeSlots = timeSlots.AsReadOnly();

      _selectedStartTime = _timeSlots[0];
      _selectedStartTime.IsSelected = true;
      _selectedEndTime = _timeSlots[0];
      _hasSelectedTimeSlot = true;
    }

    internal void UpdateTimeSlots(WorkHours workHours)
    {
      foreach (TimeSlot timeSlot in _timeSlots)
      {
        timeSlot.UpdateState(workHours);
      }
    }

    public DateTime Date
    {
      get { return _date; }
    }

    public bool IsCurrentDay
    {
      get
      {
        return DateTimeUtils.IsSameDay(Date, DateTime.Now);
      }
    }

    internal void UpdateIsCurrentDay()
    {
      OnPropertyChanged("IsCurrentDay");
    }

    //TODO: cache these?
    internal ScheduleItem EarliestItem
    {
      get
      {
        ScheduleItem earliest = null;
        if (_shortItems.Count != 0)
        {
          earliest = _shortItems[0];
        }
        else if (_longItems.Count != 0)
        {
          earliest = _longItems[0];
        }
        else
        {
          return null;
        }
        foreach (ScheduleItem item in ScheduleItems)
        {
          if (item.StartTime < earliest.StartTime)
          {
            earliest = item;
          }
        }
        return earliest;
      }
    }

    internal ScheduleItem LatestItem
    {
      get
      {
        ScheduleItem latest = null;
        if (_shortItems.Count != 0)
        {
          latest = _shortItems[0];
        }
        else if (_longItems.Count != 0)
        {
          latest = _longItems[0];
        }
        else
        {
          return null;
        }
        foreach (ScheduleItem item in ScheduleItems)
        {
          if (latest.EndTime < item.EndTime)
          {
            latest = item;
          }
        }
        return latest;
      }
    }

    public void AddScheduleItem(ScheduleItem item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }
      if (_shortItems.Contains(item))
      {
        return;
      }
      if (_longItems.Contains(item))
      {
        return;
      }
      HookScheduleItemEvents(item);
      double hourlyDuration = DateTimeUtils.DurationInHours(item.StartTime, item.EndTime);
      //TODO: see if we can just change this value to 24:
      if (hourlyDuration < 23.98) // So that items that fill up a whole day can be considered as long items.
      {
        _shortItems.Add(item);
        OnShortItemAdded(item);
      }
      else
      {
        _longItems.Add(item);
        OnLongItemAdded(item);
      }
    }

    private void ScheduleItem_RecurrenceChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = sender as ScheduleItem;
      if (scheduleItem.IsRecurring)
      {
        RemoveScheduleItem(scheduleItem);
      }

      // TODO: we are not handling the case where the user changes an item from recurring to non-recurring
    }

    public void RemoveScheduleItem(ScheduleItem itemToRemove)
    {
      if (itemToRemove == null)
      {
        throw new ArgumentNullException("itemToRemove", "Can not remove a null Appointment from a Day.");
      }
      UnhookScheduleItemEvents(itemToRemove);

      //int index = 0;
      foreach (ScheduleItem item in _shortItems)
      {
        if (item == itemToRemove)
        {
          _shortItems.Remove(item);
          OnShortItemRemoved(itemToRemove);
          return;
        }
        //index++;
      }
      //index = 0;
      foreach (ScheduleItem item in _longItems)
      {
        if (item == itemToRemove)
        {
          _longItems.Remove(item);
          OnLongItemRemoved(itemToRemove);
          return;
        }
        //index++;
      }
    }

    private void HookScheduleItemEvents(ScheduleItem item)
    {
      item.IsSelectedChanged += ScheduleItem_IsSelectedChanged;
      item.StartTimeChanged += ScheduleItem_TimeChanged;
      item.EndTimeChanged += ScheduleItem_TimeChanged;
      item.RecurrenceChanged += ScheduleItem_RecurrenceChanged;
    }

    private void UnhookScheduleItemEvents(ScheduleItem item)
    {
      item.IsSelectedChanged -= ScheduleItem_IsSelectedChanged;
      item.StartTimeChanged -= ScheduleItem_TimeChanged;
      item.EndTimeChanged -= ScheduleItem_TimeChanged;
      item.RecurrenceChanged -= ScheduleItem_RecurrenceChanged;
    }

    private void ScheduleItem_IsSelectedChanged(object sender, EventArgs e)
    {
      ScheduleItem appointment = (ScheduleItem)sender;
      if (appointment.IsSelected && !_selectedItems.Contains(appointment))
      {
        _selectedItems.Add(appointment);
      }
      if (!appointment.IsSelected)
      {
        _selectedItems.Remove(appointment);
      }
    }

    private void ScheduleItem_TimeChanged(object sender, EventArgs e)
    {
      AssignToShortOrLongCollection((ScheduleItem)sender);
    }

    private void AssignToShortOrLongCollection(ScheduleItem item)
    {
      double hourlyDuration = DateTimeUtils.DurationInHours(item.StartTime, item.EndTime);
      if (hourlyDuration >= 23.98 && _shortItems.Contains(item))
      {
        //int index = _shortItems.IndexOf(item);
        _shortItems.Remove(item);
        _longItems.Add(item);
        OnShortItemRemoved(item);
        OnLongItemAdded(item);
      }
      else if (hourlyDuration < 23.98 && _longItems.Contains(item))
      {
        //int index = _longItems.IndexOf(item);
        _longItems.Remove(item);
        _shortItems.Add(item);
        OnLongItemRemoved(item);
        OnShortItemAdded(item);
      }
    }

    public ReadOnlyCollection<ScheduleItem> SelectedItems
    {
      get { return new ReadOnlyCollection<ScheduleItem>(_selectedItems); }
    }

    public ReadOnlyCollection<ScheduleItem> ScheduleItems
    {
      get
      {
        List<ScheduleItem> items = new List<ScheduleItem>(_shortItems);
        items.AddRange(_longItems);
        return items.AsReadOnly();
      }
    }

    public event EventHandler<ScheduleItemCollectionChangedEventArgs> ItemsChanged;

    protected virtual void OnItemsChanged(ScheduleItem item, NotifyCollectionChangedAction action, bool isLong)
    {
      var handler = ItemsChanged;
      if (handler != null)
      {
        handler(this, new ScheduleItemCollectionChangedEventArgs(item, action, isLong));
      }
    }

    internal ReadOnlyCollection<ScheduleItem> ShortItems
    {
      get { return new ReadOnlyCollection<ScheduleItem>(_shortItems); }
    }

    private void OnShortItemAdded(ScheduleItem item)
    {
      OnItemsChanged(item, NotifyCollectionChangedAction.Add, false);
    }

    private void OnShortItemRemoved(ScheduleItem item)
    {
      OnItemsChanged(item, NotifyCollectionChangedAction.Remove, false);
    }

    internal ReadOnlyCollection<ScheduleItem> LongItems
    {
      get { return new ReadOnlyCollection<ScheduleItem>(_longItems); }
    }

    private void OnLongItemAdded(ScheduleItem item)
    {
      OnItemsChanged(item, NotifyCollectionChangedAction.Add, true);
    }

    private void OnLongItemRemoved(ScheduleItem item)
    {
      OnItemsChanged(item, NotifyCollectionChangedAction.Remove, true);
    }

    #region IsSelected Property

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          OnIsSelectedChanged();
        }
      }
    }

    public event EventHandler IsSelectedChanged;

    private void OnIsSelectedChanged()
    {
      EventHandler handler = IsSelectedChanged;
      if (handler != null)
      {
        IsSelectedChanged(this, EventArgs.Empty);
      }
    }

    #endregion

    public void DeselectAll()
    {
      foreach (TimeSlot slot in _timeSlots)
      {
        slot.IsSelected = false;
      }
      HasSelectedTimeSlot = false;
    }

    public TimeSlot SelectedStartTime
    {
      get { return _selectedStartTime; }
      set
      {
        //TODO: should DayView be responsible for this logic?
        if (value == null)
        {
          throw new NullReferenceException("The selected start time can not be null");
        }

        int selectedEndTimeIndex = _timeSlots.IndexOf(_selectedEndTime);
        int currentSelectedStartTimeIndex = _timeSlots.IndexOf(_selectedStartTime);
        int newSelectedStartTimeIndex = 0;
        if (value != null)
        {
          newSelectedStartTimeIndex = _timeSlots.IndexOf(value);
        }
        int topIndex = Math.Min(selectedEndTimeIndex, Math.Min(currentSelectedStartTimeIndex, newSelectedStartTimeIndex));
        int bottomIndex = Math.Max(selectedEndTimeIndex, Math.Max(currentSelectedStartTimeIndex, newSelectedStartTimeIndex));
        int topSelection = Math.Min(selectedEndTimeIndex, newSelectedStartTimeIndex);
        int bottomSelection = Math.Max(selectedEndTimeIndex, newSelectedStartTimeIndex);

        for (int i = topIndex; i <= bottomIndex; i++)
        {
          if (i < topSelection)
          {
            _timeSlots[i].IsSelected = false;
          }
          else if (i > bottomSelection)
          {
            _timeSlots[i].IsSelected = false;
          }
          else
          {
            _timeSlots[i].IsSelected = true;
          }
        }
        _selectedStartTime = value;
        HasSelectedTimeSlot = true;
      }
    }

    public TimeSlot SelectedEndTime
    {
      get { return _selectedEndTime; }
      set
      {
        int selectedStartTimeIndex = _timeSlots.IndexOf(_selectedStartTime);
        int currentSelectedEndTimeIndex = _timeSlots.IndexOf(_selectedEndTime);
        int newSelectedEndTimeIndex = _timeSlots.IndexOf(value);
        int topIndex = Math.Min(selectedStartTimeIndex, Math.Min(currentSelectedEndTimeIndex, newSelectedEndTimeIndex));
        int bottomIndex = Math.Max(selectedStartTimeIndex, Math.Max(currentSelectedEndTimeIndex, newSelectedEndTimeIndex));
        int topSelection = Math.Min(selectedStartTimeIndex, newSelectedEndTimeIndex);
        int bottomSelection = Math.Max(selectedStartTimeIndex, newSelectedEndTimeIndex);

        for (int i = topIndex; i <= bottomIndex; i++)
        {
          _timeSlots[i].IsSelected = (i >= topSelection) && (i <= bottomSelection);
        }
        _selectedEndTime = value;
        HasSelectedTimeSlot = true;
      }
    }

    public bool HasSelectedTimeSlot
    {
      get { return _hasSelectedTimeSlot; }
      private set
      {
        if (value != _hasSelectedTimeSlot)
        {
          _hasSelectedTimeSlot = value;
          OnHasSelectedTimeSlotChanged();
        }
      }
    }

    public event EventHandler HasSelectedTimeSlotChanged;

    private void OnHasSelectedTimeSlotChanged()
    {
      EventHandler handler = HasSelectedTimeSlotChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    public ReadOnlyCollection<TimeSlot> TimeSlots
    {
      get { return _timeSlots; }
    }

    public override bool HasItems()
    {
      return ShortItems.Count > 0 || LongItems.Count > 0;
    }

#if SILVERLIGHT
    // TODO: horrible horrible hack

    private ICommand _goToDayCommand;

    public ICommand GoToDayCommand
    {
      get { return _goToDayCommand; }
    }

    private void OnGoToDay(DependencyObject sender)
    {
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(sender);
      if (scheduler != null)
      {
        scheduler.SwitchToDay(Date);
      }
    }
#endif

    protected override DateTime StartDateCore
    {
      get { return Date; }
    }

    protected override DateTime EndDateCore
    {
      get { return Date; }
    }

    protected override DateDisplayMode DateRangeDisplayMode
    {
      get { return DateDisplayMode.Day; }
    }
  }
}
