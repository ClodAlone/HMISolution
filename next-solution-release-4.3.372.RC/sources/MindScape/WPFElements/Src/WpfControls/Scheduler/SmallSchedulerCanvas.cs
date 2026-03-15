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
  /// <summary>
  /// Represents a canvas on which schedule items can be laid out in a summary view.
  /// </summary>
  public class SmallSchedulerCanvas : SchedulerCanvasBase, ITimeUIElement
#if SILVERLIGHT
      , IMouseWheelObserver
#endif
  {
    private IList<MonthViewDayElement> _dayElements;
    private IList<SchedulerElement> _schedulerElements;
    private Dictionary<DayModel, IList<bool>> _usedSpaces;
    private IList<ScheduleItem> _addedLongItems;
    //private SchedulerElement _selectedElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallSchedulerCanvas"/> class.
    /// </summary>
    public SmallSchedulerCanvas()
    {
      _dayElements = new List<MonthViewDayElement>();
      _schedulerElements = new List<SchedulerElement>();
      SizeChanged += new SizeChangedEventHandler(SchedulerCanvas_SizeChanged);
      MouseLeave += new MouseEventHandler(SchedulerCanvas_MouseLeave);
      MouseEnter += new MouseEventHandler(SmallSchedulerCanvas_MouseEnter);
      MouseMove += new MouseEventHandler(SmallAppointmentSurface_MouseMove);
      MouseLeftButtonDown += new MouseButtonEventHandler(SmallAppointmentSurface_MouseLeftButtonDown);

#if SILVERLIGHT
      Mindscape.SilverlightElements.MouseWheel.Instance.AddObserver(this);
#endif
    }

    internal override SchedulerElement FindSchedulerElement(ScheduleItem item)
    {
      foreach (SchedulerElement element in _schedulerElements)
      {
        if (element.ScheduleItem == item)
        {
          return element;
        }
      }
      return null;
    }

    private void SmallSchedulerCanvas_MouseEnter(object sender, MouseEventArgs e)
    {
      Point p = e.GetPosition(this);
      int dayIndex = (int)(p.X / (ActualWidth / Days.Count));
      if (dayIndex < 0 || dayIndex > Days.Count - 1)
      {
        return;
      }
      DayModel day = Days[dayIndex];
      if (CreateHereDayIndex != dayIndex)
      {
        DateTime addAppointmentTime = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day, 0, 0, 0);
        RelayoutCreateHereButton(dayIndex, -1, addAppointmentTime);
      }
    }

    private void SmallAppointmentSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      RemoveCreateHereButton();
    }

    private void SmallAppointmentSurface_MouseMove(object sender, MouseEventArgs e)
    {
      if (EnlargeWhenFull)
      {
        //return;
      }
      if (IsMouseDown)
      {
        return;
      }

      Point p = e.GetPosition(this);
      int dayIndex = (int)(p.X / (ActualWidth / Days.Count));
      if (dayIndex < 0 || dayIndex > Days.Count - 1)
      {
        return;
      }
      DayModel day = Days[dayIndex];
      if (CreateHereDayIndex != dayIndex)
      {
        DateTime addAppointmentTime = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day, 0, 0, 0);
        RelayoutCreateHereButton(dayIndex, -1, addAppointmentTime);
      }
    }

    internal override ButtonLayout GetCreateHereButtonLayout(int dayIndex, int timeSlotIndex)
    {
      return new ButtonLayout
      (
        25,
        ActualWidth / Days.Count,
        new Thickness(dayIndex * (ActualWidth / Days.Count) + 1.5, ActualHeight - 26, 0, 0)
      );
    }

    private void SchedulerCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
      RemoveCreateHereButton();
    }

    private void SchedulerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ConfigureElements();
    }

    /// <summary>
    /// Gets the time corresponding to a location on the canvas.
    /// </summary>
    /// <param name="x">The x-coordinate of the location.</param>
    /// <param name="y">The y-coordinate of the location.</param>
    /// <param name="timeEnd">Whether to adjust the time for the start or end of a <see cref="ScheduleItem"/>.</param>
    /// <returns>The date and time corresponding to x and y.</returns>
    protected override DateTime GetDateTime(double x, double y, TimeEnd timeEnd)
    {
      double dayWidth = ActualWidth / Days.Count;
      int index = (int)(x / dayWidth);
      //int hour = (int)((x - index * dayWidth) / dayWidth * 24);
      if (index < 0 || x < 0)
      {
        index = 0;
        //hour = 0;
      }
      if (index >= Days.Count)
      {
        index = Days.Count - 1;
        //hour = 23;
      }
      DayModel day = Days[index];
      DateTime date = day.Date;
      DateTime time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
      if (timeEnd == TimeEnd.EndTime)
      {
        time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).AddHours(24);
      }
      return time;
    }


    /// <summary>
    /// Gets or sets whether to show single-day items.  This should be set to true
    /// for summary views such as month views, and false if the single-day items are
    /// shown elsewhere as in day and week views.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowShortItemsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowShortItems
    {
      get { return (bool)GetValue(ShowShortItemsProperty); }
      set { SetValue(ShowShortItemsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowShortItems"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowShortItemsProperty =
      DependencyProperty.Register("ShowShortItems", typeof(bool), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether the canvas should auto-enlarge to display its contents.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EnlargeWhenFullProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool EnlargeWhenFull
    {
      get { return (bool)GetValue(EnlargeWhenFullProperty); }
      set { SetValue(EnlargeWhenFullProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EnlargeWhenFull"/> property.
    /// </summary>
    public static readonly DependencyProperty EnlargeWhenFullProperty =
      DependencyProperty.Register("EnlargeWhenFull", typeof(bool), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(false));
    
    /// <summary>
    /// Gets or sets the minimum amount of space to be available underneath the elements within this <see cref="SmallSchedulerCanvas"/>.
    /// This is a dependency property.
    /// </summary>
    public float BottomBuffer
    {
      get { return (float)GetValue(BottomBufferProperty); }
      set { SetValue(BottomBufferProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BottomBuffer"/> property.
    /// </summary>
    public static readonly DependencyProperty BottomBufferProperty =
      DependencyProperty.Register("BottomBuffer", typeof(float), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(27f));

    private void OnLongItemAdded(DayModel day, ScheduleItem item)
    {
      if (_addedLongItems.Contains(item))
      {
        SchedulerElement existingElement = null;
        foreach (SchedulerElement elem in _schedulerElements)
        {
          if (elem.ScheduleItem == item)
          {
            existingElement = elem;
            break;
          }
        }
        if (existingElement == null)
        {
          throw new Exception("This should never be thrown. A long appointment has already been added, but its AppointmentElement can not be found!!");
        }
        bool found = false;
        foreach (DayModel d in Days)
        {
          if (d.LongItems.Contains(item))
          {
            if (!found)
            {
              existingElement.FirstVisibleDay = d;
            }
            found = true;
            existingElement.LastVisibleDay = d;
          }
        }
        ConfigureElementPositions();
        return;
      }
      _addedLongItems.Add(item);
      SchedulerElement element = new SchedulerElement(item) { FirstVisibleDay = day, LastVisibleDay = day };
      element.Style = SelectLongItemStyle(element);
      element.ContentTemplate = SelectLongItemTemplate(element);
      Children.Add(element);
      _schedulerElements.Add(element);
      double dayWidth = ActualWidth / Days.Count;
      ConfigureElementPosition(element, dayWidth);
      HookSchedulerItemEvents(item);
    }

    private void ScheduleItem_IsSelectedChanged(object sender, EventArgs e)
    {
      ScheduleItem scheduleItem = sender as ScheduleItem;
      RemoveCreateHereButton();
      if (!scheduleItem.IsSelected)
      {
        return;
      }
      IList<DayModel> days = Days;
      if (TargetMonth != null)
      {
        days = TargetMonth.Days;
      }
      if (days != null)
      {
        foreach (DayModel day in days)
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

    //private void element_IsSelectedChanged(object sender, PropertyChangedEventArgs e)
    //{
    //  SchedulerElement element = (SchedulerElement)sender;
    //  if (_selectedElement != null && !_selectedElement.IsSelected)
    //  {
    //    _selectedElement = null;
    //  }
    //  if (_selectedElement != null && element.ScheduleItem == _selectedElement.ScheduleItem)
    //  {
    //    return;
    //  }
    //  if (!element.IsSelected)
    //  {
    //    return;
    //  }
    //  foreach (DayModel day in Days)
    //  {
    //    List<ScheduleItem> selectedAppointments = new List<ScheduleItem>(day.SelectedItems);
    //    foreach (ScheduleItem appointment in selectedAppointments)
    //    {
    //      appointment.IsSelected = false;
    //    }
    //  }
    //  _selectedElement = element;
    //  _selectedElement.IsSelected = true;
    //}

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      DayModel day = (DayModel)sender;

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          if (e.IsLong)
          {
            OnLongItemAdded(day, e.Item);
          }
          else
          {
            OnShortItemAdded(day, e.Item);
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          if (e.IsLong)
          {
            OnLongItemRemoved(day, e.Item);
          }
          else
          {
            OnShortItemRemoved(e.Item);
          }
          break;
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
          day.IsSelectedChanged -= Day_IsSelectedChanged;
          foreach (ScheduleItem item in day.ScheduleItems)
          {
            UnhookSchedulerItemEvents(item);
          }
        }
      }
      int index = 0;
      if (Days != null)
      {
        foreach (DayModel day in Days)
        {
          day.ItemsChanged += Day_ItemsChanged;
          day.IsSelectedChanged += Day_IsSelectedChanged;
          index++;
        }
      }
      ConfigureElements();
    }

    private void OnShortItemRemoved(ScheduleItem item)
    {
      IList<SchedulerElement> toRemove = new List<SchedulerElement>();
      foreach (SchedulerElement element in _schedulerElements)
      {
        if (element.ScheduleItem == item)
        {
          //_schedulerElements.Remove(element);
          toRemove.Add(element);
          UnhookSchedulerItemEvents(item);
          element.NotifyRemoved();
          Children.Remove(element);
          ConfigureElements();
          //return;
        }
      }
      foreach (SchedulerElement element in toRemove)
      {
        _schedulerElements.Remove(element);
      }
    }

    private void OnShortItemAdded(DayModel day, ScheduleItem item)
    {
      if (!ShowShortItems)
      {
        return;
      }

      foreach (SchedulerElement elem in _schedulerElements)
      {
        if (elem.ScheduleItem == item && elem.FirstVisibleDay == day)
        {
          return;
        }
      }

      SchedulerElement element = new SchedulerElement(item)
      {
        FirstVisibleDay = day,
        LastVisibleDay = day
      };
      element.Style = SelectShortItemStyle(element);
      element.ContentTemplate = SelectShortItemTemplate(element);
      Children.Add(element);
      _schedulerElements.Add(element);
      double dayWidth = ActualWidth / Days.Count;
      ConfigureElementPosition(element, dayWidth);
      HookSchedulerItemEvents(item);
      ConfigureElementPositions();
    }

    private void Day_IsSelectedChanged(object sender, EventArgs e)
    {
      DayModel day = (DayModel)sender;
      if (!day.IsSelected)
      {
        return;
      }
      IList<DayModel> days = Days;
      if (TargetMonth != null)
      {
        days = TargetMonth.Days;
      }
      foreach (DayModel d in days)
      {
        List<ScheduleItem> selectedAppointments = new List<ScheduleItem>(d.SelectedItems);
        foreach (ScheduleItem appointment in selectedAppointments)
        {
          appointment.IsSelected = false;
        }
      }
    }

    private void OnLongItemRemoved(DayModel day, ScheduleItem item)
    {
      foreach (SchedulerElement element in _schedulerElements)
      {
        if (element.ScheduleItem == item)
        {
          bool found = false;
          foreach (DayModel d in Days)
          {
            if (d != day && d.LongItems.Contains(item))
            {
              if (!found)
              {
                element.FirstVisibleDay = d;
              }
              found = true;
              element.LastVisibleDay = d;
            }
          }
          if (found)
          {
            return;
          }
          _schedulerElements.Remove(element);
          _addedLongItems.Remove(element.ScheduleItem);
          UnhookSchedulerItemEvents(item);
          element.NotifyRemoved();
          Children.Remove(element);
          ConfigureElements();
          return;
        }
      }
    }

    /// <summary>
    /// Gets or sets the Style applies to <see cref="MonthViewDayElement"/> objects
    /// displayed on the canvas.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DayElementStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style DayElementStyle
    {
      get { return (Style)GetValue(DayElementStyleProperty); }
      set { SetValue(DayElementStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DayElementStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty DayElementStyleProperty =
      DependencyProperty.Register("DayElementStyle", typeof(Style), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(OnDayElementStyleChanged));

    private static void OnDayElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SmallSchedulerCanvas)d).OnDayElementStyleChanged();
    }

    private void OnDayElementStyleChanged()
    {
      Style style = DayElementStyle;
      foreach (MonthViewDayElement element in _dayElements)
      {
        element.Style = style;
      }
    }

    /// <summary>
    /// Gets or sets the Style applied to <see cref="SchedulerElement"/> controls
    /// representing multi-day items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LongItemStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style LongItemStyle
    {
      get { return (Style)GetValue(LongItemStyleProperty); }
      set { SetValue(LongItemStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LongItemStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty LongItemStyleProperty =
      DependencyProperty.Register("LongItemStyle", typeof(Style), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(null));

    #region LongItemTemplate property

    /// <summary>
    /// Gets or sets the LongItemTemplate.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate LongItemTemplate
    {
      get { return (DataTemplate)GetValue(LongItemTemplateProperty); }
      set { SetValue(LongItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LongItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty LongItemTemplateProperty =
      DependencyProperty.Register("LongItemTemplate", typeof(DataTemplate), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(null));

    #endregion // LongItemTemplate property

    /// <summary>
    /// Gets or sets the Style applied to <see cref="SchedulerElement"/> controls
    /// representing single-day items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShortItemStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style ShortItemStyle
    {
      get { return (Style)GetValue(ShortItemStyleProperty); }
      set { SetValue(ShortItemStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShortItemStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty ShortItemStyleProperty =
      DependencyProperty.Register("ShortItemStyle", typeof(Style), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(null));

    #region ShortItemTemplate property

    /// <summary>
    /// Gets or sets the ShortItemTemplate.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate ShortItemTemplate
    {
      get { return (DataTemplate)GetValue(ShortItemTemplateProperty); }
      set { SetValue(ShortItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShortItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ShortItemTemplateProperty =
      DependencyProperty.Register("ShortItemTemplate", typeof(DataTemplate), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(null));

    #endregion // ShortItemTemplate property

    /// <summary>
    /// Changes the style and template of all the schedule elements based on the new formatter.
    /// </summary>
    /// <param name="e">Event args.</param>
    protected override void OnFormatterChangedCore(DependencyPropertyChangedEventArgs e)
    {
      foreach (UIElement child in Children)
      {
        SchedulerElement element = child as SchedulerElement;
        if (element != null)
        {
          if (_addedLongItems.Contains(element.ScheduleItem))
          {
            element.Style = SelectLongItemStyle(element);
            element.ContentTemplate = SelectLongItemTemplate(element);
          }
          else
          {
            element.Style = SelectShortItemStyle(element);
            element.ContentTemplate = SelectShortItemTemplate(element);
          }
        }
      }
    }

    private Style SelectShortItemStyle(SchedulerElement element)
    {
      return Formatter == null ? ShortItemStyle : SelectStyle(element, Formatter.ShortItemStyleSelector, ShortItemStyle);
    }

    private DataTemplate SelectShortItemTemplate(SchedulerElement element)
    {
      return Formatter == null ? ShortItemTemplate : SelectTemplate(element, Formatter.ShortItemTemplateSelector, ShortItemTemplate);
    }

    private Style SelectLongItemStyle(SchedulerElement element)
    {
      return Formatter == null ? LongItemStyle : SelectStyle(element, Formatter.LongItemStyleSelector, LongItemStyle);
    }

    private DataTemplate SelectLongItemTemplate(SchedulerElement element)
    {
      return Formatter == null ? LongItemTemplate : SelectTemplate(element, Formatter.LongItemTemplateSelector, LongItemTemplate);
    }

    /// <summary>
    /// Gets or sets the month in whose view the canvas appears.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TargetMonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public MonthModel TargetMonth
    {
      get { return (MonthModel)GetValue(TargetMonthProperty); }
      set { SetValue(TargetMonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TargetMonth"/> property.
    /// </summary>
    public static readonly DependencyProperty TargetMonthProperty =
      DependencyProperty.Register("TargetMonth", typeof(MonthModel), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the TargetWeek.
    /// This is a dependency property.
    /// </summary>
    public WeekModel TargetWeek
    {
      get { return (WeekModel)GetValue(TargetWeekProperty); }
      set { SetValue(TargetWeekProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TargetWeek"/> property.
    /// </summary>
    public static readonly DependencyProperty TargetWeekProperty =
      DependencyProperty.Register("TargetWeek", typeof(WeekModel), typeof(SmallSchedulerCanvas),
      new PropertyMetadata(new PropertyChangedCallback(OnTargetWeekChanged)));

    private static void OnTargetWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SmallSchedulerCanvas)d).OnTargetWeekChanged();
    }

    private void OnTargetWeekChanged()
    {
      ConfigureElements();
    }

    private void ScheduleItem_EndTimeChanged(object sender, EventArgs e)
    {
      ConfigureElementPositions();
    }

    private void ScheduleItem_StartTimeChanged(object sender, EventArgs e)
    {
      ConfigureElementPositions();
    }

    private void ConfigureElements()
    {
      if (Days == null) { return; }
      double width = ActualWidth;
      double dayWidth = width / Days.Count;
      ClearChildren();
      _dayElements = new List<MonthViewDayElement>();
      double x = 0;
      _addedLongItems = new List<ScheduleItem>();
      foreach (DayModel day in Days)
      {
        //bool belongsToTargetMonth = TargetMonth != null && day.Date.Month == TargetMonth.StartDate.Month;
        MonthViewDayElement dayElement = new MonthViewDayElement(day, TargetMonth, TargetWeek);
        _dayElements.Add(dayElement);
        dayElement.Style = DayElementStyle;
        Children.Add(dayElement);
        Canvas.SetLeft(dayElement, x);
        dayElement.Width = dayWidth;
        dayElement.Height = ActualHeight;
        x += dayWidth;
        if (x > ActualWidth - dayWidth)
        {
          dayElement.Width += ActualWidth - x;
        }
      }
      _schedulerElements = new List<SchedulerElement>();
      foreach (DayModel day in Days)
      {
        foreach (ScheduleItem scheduleItem in day.LongItems)
        {
          if (!_addedLongItems.Contains(scheduleItem))
          {
            _addedLongItems.Add(scheduleItem);
            SchedulerElement element = new SchedulerElement(scheduleItem);
            element.FirstVisibleDay = day;
            element.LastVisibleDay = day;
            element.Style = SelectLongItemStyle(element);
            element.ContentTemplate = SelectLongItemTemplate(element);
            Children.Add(element);
            _schedulerElements.Add(element);
            //scheduleItem.IsSelected = false;
            HookSchedulerItemEvents(scheduleItem);
          }
          else
          {
            SchedulerElement element = null;
            foreach (SchedulerElement elem in _schedulerElements)
            {
              if (elem.ScheduleItem == scheduleItem)
              {
                element = elem;
                break;
              }
            }
            if (element == null)
            {
              throw new Exception("This should never be thrown. A long appointment has already been added, but its AppointmentElement can not be found!!");
            }
            element.LastVisibleDay = day;
            element.Style = SelectLongItemStyle(element);
            element.ContentTemplate = SelectLongItemTemplate(element);
          }
        }
        if (ShowShortItems)
        {
          foreach (ScheduleItem item in day.ShortItems)
          {
            SchedulerElement element = new SchedulerElement(item) { FirstVisibleDay = day, LastVisibleDay = day };
            element.Style = SelectShortItemStyle(element);
            element.ContentTemplate = SelectShortItemTemplate(element);
            Children.Add(element);
            _schedulerElements.Add(element);
            if (DateTimeUtils.IsSameDay(item.StartTime, day.Date))
            {
              HookSchedulerItemEvents(item);
            }
          }
        }
      }
      ConfigureElementPositions();
    }

    private void HookSchedulerItemEvents(ScheduleItem item)
    {
      item.StartTimeChanged += ScheduleItem_StartTimeChanged;
      item.EndTimeChanged += ScheduleItem_EndTimeChanged;
      item.IsSelectedChanged += ScheduleItem_IsSelectedChanged;
    }

    private void UnhookSchedulerItemEvents(ScheduleItem item)
    {
      item.StartTimeChanged -= ScheduleItem_StartTimeChanged;
      item.EndTimeChanged -= ScheduleItem_EndTimeChanged;
      item.IsSelectedChanged -= ScheduleItem_IsSelectedChanged;
    }

    private void ConfigureElementPositions()
    {
      if (Days != null)
      {
        double dayWidth = ActualWidth / Days.Count;
        _usedSpaces = new Dictionary<DayModel, IList<bool>>();
        foreach (DayModel day in Days)
        {
          _usedSpaces[day] = new List<bool>();
        }
        foreach (SchedulerElement element in _schedulerElements)
        {
          ConfigureElementPosition(element, dayWidth);
        }
      }
    }

    private void ConfigureElementPosition(SchedulerElement element, double dayWidth)
    {
      int firstIndex;
      IList<DayModel> knownDays = new List<DayModel>();
      if (element.FirstVisibleDay == element.LastVisibleDay)
      {
        knownDays.Add(element.FirstVisibleDay);
        firstIndex = Days.IndexOf(element.FirstVisibleDay);
      }
      else
      {
        knownDays = GetKnownDays(element.ScheduleItem, out firstIndex);
      }
      double x = firstIndex * dayWidth;
      Canvas.SetLeft(element, x);
      int y = 1;
      int usedIndex = 0;
      bool found = false;
      while (!found)
      {
        bool clear = true;
        foreach (DayModel day in knownDays)
        {
          IList<bool> usedSpaces = _usedSpaces[day];
          for (int i = usedSpaces.Count; i <= usedIndex; i++)
          {
            usedSpaces.Add(false);
          }
          if (usedSpaces[usedIndex])
          {
            clear = false;
            break;
          }
        }
        if (clear)
        {
          found = true;
          if (y + 20 + BottomBuffer < ActualHeight || EnlargeWhenFull)  // TODO: 44 = height of appointment (20) + bottom buffer space (24)
          {
            Canvas.SetTop(element, y);
            foreach (DayModel day in knownDays)
            {
              _usedSpaces[day][usedIndex] = true;
            }
            //Could probably improve this loop:
            foreach (MonthViewDayElement dayElement in _dayElements)
            {
              if (knownDays.Contains(dayElement.Day))
              {
                dayElement.HasOverflowItems = false;
              }
            }
          }
          else
          {
            element.Width = 0;
            element.Height = 0;
            element.Visibility = Visibility.Collapsed;
            //Could probably impove this loop:
            foreach (MonthViewDayElement dayElement in _dayElements)
            {
              if (knownDays.Contains(dayElement.Day))
              {
                dayElement.HasOverflowItems = true;
              }
            }
            return;
          }
        }
        y += 20; //TODO: this is the height of an AppointmentElement. Should put this somewhere else.
        usedIndex++;
      }
      if (EnlargeWhenFull && ActualHeight < y + BottomBuffer)
      {
        Height = y + BottomBuffer;
      }
      element.Height = 20;
      element.Width = dayWidth * knownDays.Count;
    }

    private IList<DayModel> GetKnownDays(ScheduleItem item, out int firstIndex)
    {
      List<DayModel> days = new List<DayModel>();
      firstIndex = -1;
      int index = 0;
      foreach (DayModel day in Days)
      {
        if (DateTimeUtils.Contains(item.StartTime, item.EndTime.AddMilliseconds(-1), day.Date))
        {
          if (firstIndex == -1)
          {
            firstIndex = index;
          }
          days.Add(day);
        }
        index++;
      }
      if (days.Count == 0) //Just a precaution
      {
        throw new InvalidOperationException("For some reason there is an appointment here that does not fit into any of the days of this surface.");
      }
      return days;
    }

#if SILVERLIGHT
    /// <summary>
    /// Gets the amount to scroll the <see cref="SchedulerCanvasBase"/> in response to the mouse wheel.
    /// </summary>
    protected override double MouseWheelScrollFactor
    {
      get { return 20; } //TODO: 20 is the height of an element, need to turn into property
    }
#endif
  }
}
