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
using System.ComponentModel;
using System.Collections.Specialized;
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
  //TODO: do some fancy OnMouseLeave stuff - if I can work out how.
  /// <summary>
  /// Represents a canvas on which schedule items can be laid out in a detail view.
  /// </summary>
  public partial class SchedulerCanvas : SchedulerCanvasBase
#if SILVERLIGHT
    , IDoubleClickObserver
#endif
    , INotifyPropertyChanged
  {
    private Dictionary<DayModel, SchedulerElementManager> _schedulerElementManagers;
    private int _firstSelectedDay; // The day selected by the mouse click
    private int _lastSelectedDay;  // The day the mouse was last dragged over after selecting the first day.

    private ScheduleItem _newItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerCanvas"/> class.
    /// </summary>
    public SchedulerCanvas()
    {
      _schedulerElementManagers = new Dictionary<DayModel, SchedulerElementManager>();
      SizeChanged += new SizeChangedEventHandler(SchedulerCanvas_SizeChanged);
      MouseLeftButtonDown += new MouseButtonEventHandler(SchedulerCanvas_MouseLeftButtonDown);
      MouseMove += new MouseEventHandler(SchedulerCanvas_MouseMove);
      MouseLeftButtonUp += new MouseButtonEventHandler(SchedulerCanvas_MouseLeftButtonUp);
      MouseLeave += new MouseEventHandler(SchedulerCanvas_MouseLeave);

#if SILVERLIGHT
      Mindscape.SilverlightElements.MouseWheel.Instance.AddObserver(this);
#else
      RequestBringIntoView += new RequestBringIntoViewEventHandler(SchedulerCanvas_RequestBringIntoView);
#endif

      HourSlotHeight = 60; //TODO: need to find a way to set this in XAML...Why wont it work?
      RightAreaBuffer = 20;

#if SILVERLIGHT
      DoubleClickListener.Instance.AddObserver(this);
#endif
    }

#if !SILVERLIGHT
    private void SchedulerCanvas_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      e.Handled = true;
    }
#endif

    private void SchedulerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ConfigureSchedulerCanvas();
      //TODO: there might be a better way to reconfigure this control on a resize, rather than emptying the children
      //and re filling it again aswell as applying the positioning algorithm. maybe a simple resizing can be done instead.
      //ALTHOUGH, configuring the surface on re-size like it does now is to solve the problem of the Width property being
      //0 when it is created and configured.
    }

    /// <summary>
    /// Gets or sets the height of each hourly slot.
    /// </summary>
    public double HourSlotHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the empty area left to the right of each <see cref="SchedulerElement"/>.
    /// </summary>
    public double RightAreaBuffer { get; set; }

    /// <summary>
    /// Gets the time corresponding to a location on the canvas.
    /// </summary>
    /// <param name="x">The x-coordinate of the location.</param>
    /// <param name="y">The y-coordinate of the location.</param>
    /// <param name="timeEnd">Whether to adjust the time for the start or end of a <see cref="ScheduleItem"/>.</param>
    /// <returns>The date and time corresponding to x and y.</returns>
    protected override DateTime GetDateTime(double x, double y, TimeEnd timeEnd)
    {
      double timeConversion = y / HourSlotHeight;
      int hour = (int)timeConversion;
      double min = timeConversion - hour;
      int minute = (int)(min * 60);
      minute = (minute / 30) * 30;
      if (y < 0)
      {
        hour = 0;
        minute = 0;
      }
      if (hour > 23)
      {
        hour = 24;
        minute = 0;
      }
      if (minute > 59)
      {
        minute = 0;
      }
      DayModel day = GetDay(x);
      DateTime baseTime = day.Date;
      DateTime time = new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, hour == 24 ? 0 : hour, minute, 0);
      if (hour == 24)
      {
        time = time.AddHours(24);
      }
      else if (timeEnd == TimeEnd.EndTime)
      {
        time = time.AddMinutes(30);
      }//TODO: this is somewhat temporary. the value should be replaced by the time changing snap size in minutes.
      return time;
    }

    private DayModel GetDay(double x)
    {
      double dayWidth = ActualWidth / Days.Count;
      int index = (int)(x / dayWidth);
      if (x < 2)
      {
        index = 0;
      }
      if (x > ActualWidth - 2)
      {
        index = Days.Count - 1;
      }
      return Days[index];
    }

    internal override SchedulerElement FindSchedulerElement(ScheduleItem item)
    {
      foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
      {
        SchedulerElement element = manager.FindSchedulerElement(item);
        if(element != null)
        {
          return element;
        }
      }
      return null;
    }

    private void OnScheduleItemAdded(DayModel day, ScheduleItem scheduleItem)
    {
      SchedulerElement element = new SchedulerElement(scheduleItem) { FirstVisibleDay = day };
      element.Style = SelectScheduleItemStyle(element);
      element.ContentTemplate = SelectScheduleItemTemplate(element);
      SchedulerElementManager manager = _schedulerElementManagers[day];
      Children.Add(element);
      manager.AddElement(element);
      HookScheduleItemEvents(scheduleItem);
      ConfigureElementPositions();
    }

    private Style SelectScheduleItemStyle(SchedulerElement element)
    {
      return Formatter == null ? ScheduleItemStyle : SelectStyle(element, Formatter.ScheduleItemStyleSelector, ScheduleItemStyle);
    }

    private DataTemplate SelectScheduleItemTemplate(SchedulerElement element)
    {
      return Formatter == null ? ScheduleItemTemplate : SelectTemplate(element, Formatter.ScheduleItemTemplateSelector, ScheduleItemTemplate);
    }

    private void ScheduleItem_IsSelectedChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = sender as ScheduleItem;
      if (!scheduleItem.IsSelected)
      {
        return;
      }
      if (Days != null)
      {
        foreach (DayModel day in Days)
        {
          List<ScheduleItem> selectedItems = new List<ScheduleItem>(day.SelectedItems);
          foreach (ScheduleItem item in selectedItems)
          {
            if (item != scheduleItem)
            {
              item.IsSelected = false;
            }
          }
          day.DeselectAll();
          day.IsSelected = false;
        }
      }
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      if (!e.IsLong)
      {
        DayModel day = (DayModel)sender;
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            OnScheduleItemAdded(day, e.Item);
            break;
          case NotifyCollectionChangedAction.Remove:
            OnScheduleItemRemoved(day, e.Item);
            break;
        }
      }
    }

    /// <summary>
    /// Updates the user interface in response to changes to the <see cref="SchedulerCanvasBase.Days"/> property.
    /// </summary>
    /// <param name="e">Contains information about changes to the Days property.</param>
    protected override void OnDaysChanged(DependencyPropertyChangedEventArgs e)
    {
      IList<DayModel> oldDays = (IList<DayModel>)e.OldValue;
      if (oldDays != null)
      {
        foreach (DayModel day in oldDays)
        {
          day.ItemsChanged -= Day_ItemsChanged;
          day.HasSelectedTimeSlotChanged -= Day_HasSelectedTimeSlotChanged;
          foreach (ScheduleItem item in day.ScheduleItems)
          {
            UnhookScheduleItemEvents(item);
          }
        }
      }
      int index = 0;
      if (Days != null)
      {
        foreach (DayModel day in Days)
        {
          day.ItemsChanged += Day_ItemsChanged;
          day.HasSelectedTimeSlotChanged += Day_HasSelectedTimeSlotChanged;
          day.IsSelected = false;
          index++;
        }
        ConfigureSchedulerCanvas();
      }
    }

    private void Day_HasSelectedTimeSlotChanged(object sender, EventArgs e)
    {
      DayModel day = sender as DayModel;
      if (day.HasSelectedTimeSlot)
      {
        foreach (DayModel d in Days)
        {
          List<ScheduleItem> selectedItems = new List<ScheduleItem>(d.SelectedItems);
          foreach (ScheduleItem item in selectedItems)
          {
            item.IsSelected = false;
          }
        }
      }
    }

    private void OnScheduleItemRemoved(DayModel day, ScheduleItem scheduleItem)
    {
      UnhookScheduleItemEvents(scheduleItem);
      SchedulerElementManager manager = _schedulerElementManagers[day];
      SchedulerElement element = manager.RemoveItem(scheduleItem);
      if (element != null)
      {
        element.NotifyRemoved();
        Children.Remove(element);
      }
    }

    /// <summary>
    /// Gets or sets the Style for schedule items displayed on the canvas.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScheduleItemStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style ScheduleItemStyle
    {
      get { return (Style)GetValue(ScheduleItemStyleProperty); }
      set { SetValue(ScheduleItemStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScheduleItemStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty ScheduleItemStyleProperty =
      DependencyProperty.Register("ScheduleItemStyle", typeof(Style), typeof(SchedulerCanvas),
      new PropertyMetadata(OnScheduleItemStyleChanged));

    private static void OnScheduleItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvas)d).OnScheduleItemStyleChanged();
    }

    private void OnScheduleItemStyleChanged()
    {
      if (Formatter != null)
      {
        foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
        {
          manager.FormatScheduleItems(Formatter.ScheduleItemStyleSelector, ScheduleItemStyle);
        }
      }
    }

    #region ScheduleItemTemplate property

    /// <summary>
    /// Gets or sets the DataTemplate. for schedule items displayed on the canvas.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate ScheduleItemTemplate
    {
      get { return (DataTemplate)GetValue(ScheduleItemTemplateProperty); }
      set { SetValue(ScheduleItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScheduleItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ScheduleItemTemplateProperty =
      DependencyProperty.Register("ScheduleItemTemplate", typeof(DataTemplate), typeof(SchedulerCanvas),
      new PropertyMetadata(new PropertyChangedCallback(OnScheduleItemTemplateChanged)));

    private static void OnScheduleItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvas)d).OnScheduleItemTemplateChanged();
    }

    private void OnScheduleItemTemplateChanged()
    {
      if (Formatter != null)
      {
        foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
        {
          manager.FormatScheduleItems(Formatter.ScheduleItemTemplateSelector, ScheduleItemTemplate);
        }
      }
    }

    #endregion // ScheduleItemTemplate property

    /// <summary>
    /// Changes the style and template of all the schedule elements based on the new formatter.
    /// </summary>
    /// <param name="e">Event args.</param>
    protected override void OnFormatterChangedCore(DependencyPropertyChangedEventArgs e)
    {
      foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
      {
        manager.FormatScheduleItems(Formatter == null ? null : Formatter.ScheduleItemStyleSelector, ScheduleItemStyle);
      }
      foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
      {
        manager.FormatScheduleItems(Formatter == null ? null : Formatter.ScheduleItemTemplateSelector, ScheduleItemTemplate);
      }
    }

    /// <summary>
    /// Gets or sets the DataTemplate for presenting <see cref="TimeSlot"/> objects on the canvas.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TimeSlotTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate TimeSlotTemplate
    {
      get { return (DataTemplate)GetValue(TimeSlotTemplateProperty); }
      set { SetValue(TimeSlotTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TimeSlotTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty TimeSlotTemplateProperty =
      DependencyProperty.Register("TimeSlotTemplate", typeof(DataTemplate), typeof(SchedulerCanvas),
      new PropertyMetadata(OnTimeSlotTemplateChanged));

    private static void OnTimeSlotTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvas)d).OnTimeSlotTemplateChanged();
    }

    private void OnTimeSlotTemplateChanged()
    {
      foreach (FrameworkElement element in Children)
      {
        ContentPresenter cp = element as ContentPresenter;
        if (cp != null)
        {
          if (cp.Content is TimeOfDay)
          {
            //TimeOfDay time = (TimeOfDay)(cp.Content);
            cp.ContentTemplate = TimeSlotTemplate;
            cp.Width = ActualWidth + 20;
          }
        }
      }
    }

    private void ConfigureSchedulerCanvas()
    {
      if (ActualWidth == 0)
      {
        return;
      }
      double left = 0;
      int dayCount = Days.Count;
      double width = ActualWidth / dayCount;
      ClearChildren();
      _schedulerElementManagers = new Dictionary<DayModel, SchedulerElementManager>();
      foreach (DayModel day in Days)
      {
        SchedulerElementManager manager = new SchedulerElementManager() { HourSlotHeight = HourSlotHeight };
        _schedulerElementManagers[day] = manager;
        manager.Left = left + 2;
        manager.Right = left + width - RightAreaBuffer;
        double top = 0;
        if (day == Days[dayCount - 1])
        {
          width = ActualWidth - left;
        }
        foreach (TimeSlot timeSlot in day.TimeSlots)
        {
          ContentPresenter cp = new ContentPresenter();
          cp.Content = timeSlot;
          cp.ContentTemplate = TimeSlotTemplate;
          Canvas.SetTop(cp, top);
          Canvas.SetLeft(cp, left);
          cp.Width = width;
          top += HourSlotHeight / 2; //TODO: divide this according to how long the timeSlot is.
          Children.Add(cp);
        }
        left += width;

        foreach (ScheduleItem scheduleItem in day.ShortItems) //TODO: factor this out with the OnAppointmentAdded.
        {
          SchedulerElement element = new SchedulerElement(scheduleItem) { FirstVisibleDay = day };
          element.Style = SelectScheduleItemStyle(element);
          element.ContentTemplate = SelectScheduleItemTemplate(element);
          Children.Add(element);
          manager.AddElement(element);
          scheduleItem.IsSelected = false;
          HookScheduleItemEvents(scheduleItem);
        }
        ConfigureElementPositions();
      }
    }

    private void HookScheduleItemEvents(ScheduleItem scheduleItem)
    {
      scheduleItem.StartTimeChanged += ScheduleItem_StartTimeChanged;
      scheduleItem.EndTimeChanged += ScheduleItem_EndTimeChanged;
      scheduleItem.IsSelectedChanged += ScheduleItem_IsSelectedChanged;
    }

    private void UnhookScheduleItemEvents(ScheduleItem scheduleItem)
    {
      scheduleItem.StartTimeChanged -= ScheduleItem_StartTimeChanged;
      scheduleItem.EndTimeChanged -= ScheduleItem_EndTimeChanged;
      scheduleItem.IsSelectedChanged -= ScheduleItem_IsSelectedChanged;
    }

    private void ScheduleItem_EndTimeChanged(object sender, EventArgs e)
    {
      ConfigureElementPositions(); //TODO: it would be great to find a way to only update the AppointmentElementManager that holds the appointment that changed.
    }

    private void ScheduleItem_StartTimeChanged(object sender, EventArgs e)
    {
      ConfigureElementPositions();
    }

    private void ConfigureElementPositions()
    {
      foreach (SchedulerElementManager manager in _schedulerElementManagers.Values)
      {
        manager.ConfigureElementPositions();
      }
    }

    #region Mouse logic

    private void SchedulerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      CaptureMouse();
      Point p = e.GetPosition(this);

      int dayIndex = (int)Math.Max(0, Math.Min(Days.Count - 1, p.X / (ActualWidth / Days.Count)));
      DayModel day = Days[dayIndex];

      int index = (int)Math.Max(0, Math.Min(day.TimeSlots.Count - 1, p.Y / (HourSlotHeight / 2)));
      TimeSlot time = day.TimeSlots[index];

      foreach (DayModel d in Days)
      {
        d.DeselectAll();
        d.IsSelected = false;
      }
      day.SelectedStartTime = time;
      day.SelectedEndTime = time;
      _firstSelectedDay = dayIndex;
      _lastSelectedDay = dayIndex;

      RemoveCreateHereButton();

#if SILVERLIGHT
      MouseButtonEventHandler handler = MouseLeftButtonPressed;
      if (handler != null)
      {
        handler(sender, e);
      }
#endif
    }

    private void SchedulerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ReleaseMouseCapture();
    }

    //TODO refactor this method
    private void SchedulerCanvas_MouseMove(object sender, MouseEventArgs e)
    {
      Point p = e.GetPosition(this);
      int index = (int)(p.Y / (HourSlotHeight / 2));
      if (index < 0)
      {
        index = 0;
      }
      int dayIndex = (int)(p.X / (ActualWidth / Days.Count));
      if (dayIndex < 0 || dayIndex > Days.Count - 1)
      {
        return;
      }
      DayModel day = Days[dayIndex];
      index = Math.Min(index, day.TimeSlots.Count - 1);
      TimeSlot time = day.TimeSlots[index];
      if (IsMouseDown)
      {
        _lastSelectedDay = dayIndex;
        int firstDay = Math.Min(_lastSelectedDay, _firstSelectedDay);
        int lastDay = Math.Max(_lastSelectedDay, _firstSelectedDay);
        for (int i = 0; i < Days.Count; i++)
        {
          if (i < firstDay || i > lastDay)
          {
            Days[i].DeselectAll();
          }
          else if (i != _firstSelectedDay)
          {
            Days[i].SelectedStartTime = Days[i].TimeSlots[0]; //TODO provide easy access to getting first / last time slot
            Days[i].SelectedEndTime = Days[i].TimeSlots[Days[i].TimeSlots.Count - 1];
          }
        }
        if (_lastSelectedDay > _firstSelectedDay)
        {
          day.SelectedEndTime = time;
          Days[_firstSelectedDay].SelectedEndTime = Days[_firstSelectedDay].TimeSlots[Days[_firstSelectedDay].TimeSlots.Count - 1];
        }
        else
        {
          day.SelectedEndTime = time;
          if (_firstSelectedDay != _lastSelectedDay)
          {
            day.SelectedStartTime = day.TimeSlots[day.TimeSlots.Count - 1];
            Days[_firstSelectedDay].SelectedEndTime = Days[_firstSelectedDay].TimeSlots[0];
          }
        }
      }
      if (SlotContainsItem(day, time.DateTime))
      {
        RemoveCreateHereButton();
        return;
      }
      if (CreateHereDayIndex != dayIndex || !CreateHereTime.Equals(time.DateTime))
      {
        RelayoutCreateHereButton(dayIndex, index, time.DateTime);
      }
    }

    internal override ButtonLayout GetCreateHereButtonLayout(int dayIndex, int timeSlotIndex)
    {
      return new ButtonLayout
      (
        HourSlotHeight / 2,
        ActualWidth / Days.Count,
        new Thickness(dayIndex * (ActualWidth / Days.Count) + 1.5, timeSlotIndex * (HourSlotHeight / 2), 0, 0)
      );
    }

    private static readonly TimeSpan SlotSize = TimeSpan.FromMinutes(30);

    //Finds out if the given day has any appointments within the given time + 30 mins
    private bool SlotContainsItem(DayModel day, DateTime time)
    {
      DateTime endTime = time + SlotSize;
      foreach (ScheduleItem item in day.ShortItems)
      {
        if (item.StartTime < endTime && time < item.EndTime)
        {
          return true;
        }
      }
      return false;
    }

    private void SchedulerCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
      RemoveCreateHereButton();
    }

    #endregion

#if SILVERLIGHT
    /// <summary>
    /// Gets the amount to scroll the <see cref="SchedulerCanvasBase"/> in response to the mouse wheel.
    /// </summary>
    protected override double MouseWheelScrollFactor
    {
      get { return HourSlotHeight / 2; }
    }
#endif

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="name">The name of the property that changed.</param>
    protected void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }

#if SILVERLIGHT
    #region IDoubleClickObserver Members

    void IDoubleClickObserver.OnDoubleClick()
    {
      OnDoubleClick();
    }

    /// <summary>
    /// Raised when the user clicks the mouse button.
    /// </summary>
    public event MouseButtonEventHandler MouseLeftButtonPressed;

    #endregion
#else
    /// <summary>
    /// Called when the mouse button is pressed over this <see cref="SchedulerCanvas"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
      {
        OnDoubleClick();
      }
    }

    private void EnsureDisableDragging()
    {
      ReleaseMouseCapture();
      IsMouseDown = false;
    }
#endif

    private void OnDoubleClick()
    {
      if (_firstSelectedDay == _lastSelectedDay)
      {
        DayModel day = Days[_firstSelectedDay];
        if (day.SelectedStartTime == day.SelectedEndTime && day.SelectedStartTime != null)
        {
          TimeSlot timeSlot = day.SelectedStartTime;
          _newItem = new ScheduleItem();
          DateTime d = day.Date;
          DateTime firstDate = new DateTime(d.Year, d.Month, d.Day, timeSlot.Hour, timeSlot.Minute, 0);
          DateTime lastDate = firstDate.AddMinutes(30);
          IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
          Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
          Schedule schedule = view.Schedule;
          bool showDefaultEditor = true;
          ReleaseMouseCapture();
          IsMouseDown = false;
          string name = (scheduler == null || scheduler.Formatter == null || scheduler.Formatter.DefaultScheduleItemName == null) ? "New Appointment" : scheduler.Formatter.DefaultScheduleItemName;
          if (view != null && schedule != null && schedule.ScheduleItemBuilder != null)
          {
            CreateScheduleItemArgs args = new CreateScheduleItemArgs(schedule, ScheduleItemCreationType.DoubleClick, firstDate, lastDate, name);
            CreateScheduleItemResult result = schedule.ScheduleItemBuilder.CreateScheduleItem(args);
            showDefaultEditor = result.AddDefaultItem && result.Item == null;
          }
          if (showDefaultEditor)
          {
            _newItem.Name = name;
            _newItem.StartTime = firstDate;
            _newItem.EndTime = lastDate;
            if (view != null)
            {
              AddScheduleItemEventArgs addItemArgs = schedule.AddItemReturnArgs(_newItem);
              showDefaultEditor = !addItemArgs.Cancel && addItemArgs.ShowDefaultEditor;
            }
            else
            {
              day.AddScheduleItem(_newItem);
            }
            if (showDefaultEditor)
            {
#if SILVERLIGHT
              ScheduleItemDialog dlg = new ScheduleItemDialog(_newItem);
              dlg.Closed += new EventHandler(ScheduleItemDialog_Closed);
              dlg.Show();
#else
              if (view != null)
              {
                ScheduleItemDialog dlg = new ScheduleItemDialog(_newItem, scheduler == null ? DayOfWeek.Monday : scheduler.FirstDayOfWeek);
                if (scheduler != null && scheduler.ScheduleItemDialogStyle != null)
                {
                  dlg.Style = scheduler.ScheduleItemDialogStyle;
                }
                else if (view is DayScheduleBase)
                {
                  DayScheduleBase dayScheduleBase = view as DayScheduleBase;
                  if (dayScheduleBase != null && dayScheduleBase.ScheduleItemDialogStyle != null)
                  {
                    dlg.Style = dayScheduleBase.ScheduleItemDialogStyle;
                  }
                }
                dlg.Scheduler = scheduler ?? (view as UIElement);
                dlg.Closed += new RoutedEventHandler(ScheduleItemDialog_Closed);
                DialogHelper.ShowItemDialog(dlg, this, scheduler == null ? null : scheduler.Formatter);
              }
#endif
            }
          }
#if !SILVERLIGHT
          Dispatcher.BeginInvoke(new Action(EnsureDisableDragging));
#endif
        }
      }
    }

    private void ScheduleItemDialog_Closed(object sender, EventArgs e)
    {
      ScheduleItemDialog dialog = sender as ScheduleItemDialog;
      if (dialog != null)
      {
        if (dialog.DialogResult == false)
        {
          IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
          if (view != null && _newItem != null)
          {
            view.Schedule.RemoveItem(_newItem);
          }
        }
      }
      _newItem = null;
    }
  }
}
