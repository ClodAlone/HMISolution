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
  /// Represents a <see cref="ScheduleItem"/> on a display surface (typically a
  /// <see cref="SchedulerCanvasBase"/>).
  /// </summary>
  public class SchedulerElement : ListBoxItem, INotifyPropertyChanged
#if SILVERLIGHT
    , IDoubleClickObserver
#endif
  {
    private readonly ScheduleItem _scheduleItem;
    private DayModel _firstVisibleDay;
    private DayModel _lastVisibleDay;

    internal SchedulerElement(ScheduleItem scheduleItem)
    {
      if (scheduleItem == null)
      {
        throw new ArgumentNullException("scheduleItem");
      }
      IsSelected = scheduleItem.IsSelected;
      _scheduleItem = scheduleItem;
      HookScheduleItemEvents();
#if SILVERLIGHT
      DoubleClickListener.Instance.AddObserver(this);
#else
      _mouseLeftButtonPressed = null; // TODO: remove this when possible
#endif
    }

#if !SILVERLIGHT
    internal event RoutedEventHandler TemplateApplied;

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      RoutedEventHandler handler = TemplateApplied;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
    }
#endif

    internal DayModel FirstVisibleDay
    {
      get { return _firstVisibleDay; }
      set
      {
        if (value != _firstVisibleDay)
        {
          _firstVisibleDay = value;
          if (LastVisibleDay == null)
          {
            LastVisibleDay = value;
          }
          UpdateDisplayState();
        }
      }
    }

    internal DayModel LastVisibleDay
    {
      get { return _lastVisibleDay; }
      set
      {
        _lastVisibleDay = value;
        UpdateDisplayState();
      }
    }

    /// <summary>
    /// Gets whether the start of the <see cref="ScheduleItem"/> represented by this
    /// <see cref="SchedulerElement"/> is on a displayed date.
    /// </summary>
    public bool IsScheduleItemStartVisible
    {
      get
      {
        return FirstVisibleDay == null ? false : DateTimeUtils.IsSameDay(FirstVisibleDay.Date, ScheduleItem.StartTime);
      }
    }

    /// <summary>
    /// Gets whether the start of the <see cref="ScheduleItem"/> represented by this
    /// <see cref="SchedulerElement"/> is on a displayed date.
    /// </summary>
    public bool IsScheduleItemEndVisible
    {
      get
      {
        bool isVisible = false;
        if (LastVisibleDay != null)
        {
          bool isSameDay = DateTimeUtils.IsSameDay(LastVisibleDay.Date, ScheduleItem.EndTime);
          bool isNextDay = DateTimeUtils.IsSameDay(LastVisibleDay.Date.AddDays(1), ScheduleItem.EndTime);
          isVisible = isSameDay || (isNextDay && ScheduleItem.EndTime.Hour == 0 && ScheduleItem.EndTime.Minute == 0);
        }
        return isVisible;
      }
    }

    /// <summary>
    /// Gets display information for the <see cref="SchedulerElement"/>.
    /// </summary>
    public SchedulerElementDisplayState ElementState
    {
      get { return new SchedulerElementDisplayState(this); }
    }

    private void UpdateDisplayState()
    {
      OnPropertyChanged("IsScheduleItemStartVisible");
      OnPropertyChanged("IsScheduleItemEndVisible");
      OnPropertyChanged("ElementState");
      Content = ElementState;
    }

    internal void NotifyRemoved()
    {
      UnhookScheduleItemEvents();
    }

    /// <summary>
    /// Gets the <see cref="ScheduleItem"/> represented by this <see cref="SchedulerElement"/>.
    /// </summary>
    public ScheduleItem ScheduleItem
    {
      get { return _scheduleItem; }
    }

    private void HookScheduleItemEvents()
    {
      _scheduleItem.IsSelectedChanged += ScheduleItem_IsSelectedChanged;
      _scheduleItem.StartTimeChanged += ScheduleItem_StartTimeChanged;
      _scheduleItem.EndTimeChanged += ScheduleItem_EndTimeChanged;
    }

    private void UnhookScheduleItemEvents()
    {
      _scheduleItem.IsSelectedChanged -= ScheduleItem_IsSelectedChanged;
      _scheduleItem.StartTimeChanged -= ScheduleItem_StartTimeChanged;
      _scheduleItem.EndTimeChanged -= ScheduleItem_EndTimeChanged;
    }

    private void ScheduleItem_EndTimeChanged(object sender, EventArgs e)
    {
      UpdateDisplayState();
    }

    private void ScheduleItem_StartTimeChanged(object sender, EventArgs e)
    {
      UpdateDisplayState();
    }

    private void ScheduleItem_IsSelectedChanged(object sender, EventArgs e)
    {
      if (_scheduleItem.IsSelected != IsSelected)
      {
        IsSelected = _scheduleItem.IsSelected;
      }
#if !SILVERLIGHT
      if(IsSelected)
      {
        Dispatcher.BeginInvoke(new Action(EnsureFocus));
      }
#endif
    }

#if !SILVERLIGHT
    private void EnsureFocus()
    {
      Focus();
    }
#endif

    /// <summary>
    /// Called by the framework when the user presses the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      ScheduleItem.IsSelected = true;

      OnMouseLeftButtonPressed(e);
    }

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
    void IDoubleClickObserver.OnDoubleClick()
    {
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
      if (scheduler != null)
      {
        // TODO: problem: this leaves us in a mouse-down state so if the handler takes focus we're
        // left with a sticky SchedulerElement that gets dragged when we move the mouse (even
        // though the mouse is up).
        scheduler.OnItemActivated(ScheduleItem);
        ScheduleItemMover mover = GetTemplateChild("PART_Mover") as ScheduleItemMover;
        if (mover != null)
        {
          mover.CancelDrag();
        }
        // Useful for debuging:
        //ScheduleItemDialog dlg = new ScheduleItemDialog(ScheduleItem);
        //dlg.Show();
      }
    }
#else
    /// <summary>
    /// Called when the mouse is double clicked on this <see cref="SchedulerElement"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
      if (scheduler != null)
      {
        // TODO: problem: this leaves us in a mouse-down state so if the handler takes focus we're
        // left with a sticky SchedulerElement that gets dragged when we move the mouse (even
        // though the mouse is up).
        ScheduleItemMover mover = GetTemplateChild("PART_Mover") as ScheduleItemMover;
        if (mover != null)
        {
          mover.CancelDrag();
        }
        scheduler.OnItemActivated(ScheduleItem);
        
        // Useful for debuging:
        //ScheduleItemDialog dlg = new ScheduleItemDialog(ScheduleItem);
        //dlg.Show();
      }
    }
#endif

    private void OnMouseLeftButtonPressed(MouseButtonEventArgs e)
    {
      var handler = _mouseLeftButtonPressed;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    private MouseButtonEventHandler _mouseLeftButtonPressed;

#if SILVERLIGHT
    event MouseButtonEventHandler IDoubleClickObserver.MouseLeftButtonPressed
    {
      add { _mouseLeftButtonPressed += value; }
      remove { _mouseLeftButtonPressed -= value; }
    }
#endif
  }
}
