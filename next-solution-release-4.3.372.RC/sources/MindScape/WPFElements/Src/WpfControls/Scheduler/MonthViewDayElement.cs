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
  /// Displays a single day in the month view of a <see cref="Scheduler"/>.
  /// </summary>
  public class MonthViewDayElement : ListBoxItem, INotifyPropertyChanged
#if SILVERLIGHT
    , IDoubleClickObserver
#endif
  {
    internal static bool _dragging;    //TODO: should try to find a way to make this non-static.

    private ScheduleItem _newItem;

    internal MonthViewDayElement(DayModel day, MonthModel month, WeekModel week)
    {
      _day = day;
      day.IsSelectedChanged += Day_IsSelectedChanged;
      _month = month;
      _week = week;
      _belongsToTargetMonth = (month != null && _month.Contains(day));
#if SILVERLIGHT
      _viewDetailsCommand = new DelegateCommand(OnViewDetails);
      DoubleClickListener.Instance.AddObserver(this);
#else
      CommandBindings.Add(new CommandBinding(SchedulerCommands.ViewDetailsCommand, ViewDetails_Executed));
#endif
    }

#if SILVERLIGHT
    private readonly ICommand _viewDetailsCommand;

    /// <summary>
    /// Gets a command for viewing the details of the day that is represented.
    /// </summary>
    public ICommand ViewDetailsCommand
    {
      get { return _viewDetailsCommand; }
    }
#else
    private void ViewDetails_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      OnViewDetails();
    }
#endif

    private void OnViewDetails()
    {
      VisualTreeUtils.FindContaining<Scheduler>(this).SwitchToDay(_day.Date);
    }

    private void Day_IsSelectedChanged(object sender, EventArgs e)
    {
      IsSelected = ((DayModel)sender).IsSelected;
      OnPropertyChanged("ViewStatus");
    }

    /// <summary>
    /// Called by the framework when the mouse leaves the control.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
#if SILVERLIGHT
      IsMouseOver = false;
#else
      OnPropertyChanged("ViewStatus");
#endif
    }

    /// <summary>
    /// Called by the framework when the mouse enters the control.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
#if SILVERLIGHT
      IsMouseOver = true;
#else
      OnPropertyChanged("ViewStatus");
#endif
      if (_dragging)
      {
        if (_month != null)
        {
          _month.SelectedEndDay = _day;
        }
        if (_week != null)
        {
          _week.SelectedEndDay = _day;
        }
      }
    }

    /// <summary>
    /// Called by the framework when the user presses the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      _dragging = true;
      if (_month != null)
      {
        _month.SelectedStartDay = _day;
        _month.SelectedEndDay = _day;
      }
      if (_week != null)
      {
        _week.SelectedStartDay = _day;
        _week.SelectedEndDay = _day;
        _week.DeselectAllDays();
      }
      e.Handled = false;

#if SILVERLIGHT
      MouseButtonEventHandler handler = MouseLeftButtonPressed;
      if (handler != null)
      {
        handler(this, e);
      }
#endif
    }

    /// <summary>
    /// Called by the framework when the user releases the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      _dragging = false;
      e.Handled = false;
    }

    private readonly DayModel _day;
    private readonly MonthModel _month;
    private readonly WeekModel _week;
#if SILVERLIGHT
    private bool _isMouseOver;
#endif

    /// <summary>
    /// Gets the day represented by this <see cref="MonthViewDayElement"/>.
    /// </summary>
    public DayModel Day
    {
      get { return _day; }
    }

    private bool _hasOverflowItems;

    /// <summary>
    /// Gets whether the control contains more <see cref="ScheduleItem"/> objects than
    /// it can render.
    /// </summary>
    public bool HasOverflowItems
    {
      get { return _hasOverflowItems; }
      internal set
      {
        _hasOverflowItems = value;
        OnPropertyChanged("HasOverflowItems");
      }
    }

    private readonly bool _belongsToTargetMonth;

    /// <summary>
    /// Gets whether the represented day belongs to the month being displayed.
    /// </summary>
    public bool BelongsToTargetMonth
    {
      get { return _belongsToTargetMonth; }
    }

    /// <summary>
    /// Gets display information for the <see cref="MonthViewDayElement"/>.
    /// </summary>
    public ElementViewStatus ViewStatus
    {
      get
      {
        if (_day.IsSelected)
        {
          return ElementViewStatus.Selected;
        }
        if (IsMouseOver)
        {
          return BelongsToTargetMonth ? ElementViewStatus.MouseOver : ElementViewStatus.MouseOverPadding;
        }
        if (!BelongsToTargetMonth)
        {
          return ElementViewStatus.Padding;
        }
        return ElementViewStatus.Normal;
      }
    }

#if SILVERLIGHT
    private bool IsMouseOver
    {
      get { return _isMouseOver; }
      set
      {
        if (value != _isMouseOver)
        {
          _isMouseOver = value;
          OnPropertyChanged("ViewStatus");
        }
      }
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
    /// Called when the mouse is double click over this <see cref="MonthViewDayElement"/>.
    /// </summary>
    /// <param name="e">Event data.</param>
    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      base.OnMouseDoubleClick(e);

      OnDoubleClick();
    }

    private void EnsureDisableDragging()
    {
      _dragging = false;
    }
#endif

    private void OnDoubleClick()
    {
      _newItem = new ScheduleItem();
      IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
      Schedule schedule = view.Schedule;
      bool showDefaultEditor = true;
      DateTime d = _day.Date;
      DateTime firstDate = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
      DateTime lastDate = firstDate.AddHours(24);
      _dragging = false;
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
        if (schedule != null)
        {
          AddScheduleItemEventArgs addItemArgs = schedule.AddItemReturnArgs(_newItem);
          showDefaultEditor = !addItemArgs.Cancel && addItemArgs.ShowDefaultEditor;
        }
        else
        {
          _day.AddScheduleItem(_newItem);
        }
        if (showDefaultEditor)
        {
#if SILVERLIGHT
          ScheduleItemDialog dlg = new ScheduleItemDialog(_newItem);
          dlg.Closed += new EventHandler(ScheduleItemDialog_Closed);
          dlg.Show();
#else
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
#endif
        }
      }
#if !SILVERLIGHT
      Dispatcher.BeginInvoke(new Action(EnsureDisableDragging));
#endif
    }

    private void ScheduleItemDialog_Closed(object sender, EventArgs e)
    {
      ScheduleItemDialog dialog = sender as ScheduleItemDialog;
      if (dialog != null)
      {
        if (dialog.DialogResult == false)
        {
          IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
          if (view != null && view.Schedule != null)
          {
            view.Schedule.RemoveItem(_newItem);
          }
        }
      }
      _newItem = null;
    }
  }
}
