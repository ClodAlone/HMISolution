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
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// The base class for specific views in the <see cref="Scheduler"/> (day,
  /// week and month views).
  /// </summary>
  public abstract class ScheduleView : Control, INotifyPropertyChanged, IScheduleProvider
  {
    private static readonly ReadOnlyCollection<TimeOfDay> _hoursOfTheDay;
    private Schedule _schedule;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleView"/> class.
    /// </summary>
    protected ScheduleView()
    {
#if SILVERLIGHT
      _previousCommand = new DelegateCommand(DecrementView);
      _nextCommand = new DelegateCommand(IncrementView);
      _addScheduleItemCommand = new DelegateCommand(AddScheduleItem);
#else
      BindCommands();
#endif
    }

#if !SILVERLIGHT
    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(SchedulerCommands.PreviousScheduleItemCommand, PreviousScheduleItem_Executed, PreviousScheduleItem_CanExecute));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.NextScheduleItemCommand, NextScheduleItem_Executed, NextScheduleItem_CanExecute));
    }

    private void PreviousScheduleItem_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      GotoPreviousAppointment();
    }

    private void PreviousScheduleItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGotoPreviousAppointment;
    }

    private void NextScheduleItem_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      GotoNextAppointment();
    }

    private void NextScheduleItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGotoNextAppointment;
    }
#endif

    SchedulerElement IScheduleProvider.FindSchedulerElement(ScheduleItem item)
    {
      SchedulerElement element = null;
      foreach (SchedulerCanvasBase canvas in VisualTreeUtils.GetChildren<SchedulerCanvasBase>(this))
      {
        element = canvas.FindSchedulerElement(item);
        if (element != null)
        {
          break;
        }
      }
      return element;
    }

    static ScheduleView()
    {
      List<TimeOfDay> hours = new List<TimeOfDay>();
      for (int i = 0; i < 24; i++)
      {
        hours.Add(new TimeOfDay(i));
      }
      _hoursOfTheDay = hours.AsReadOnly();
    }

    /// <summary>
    /// Gets the hours of the day.
    /// </summary>
    public ReadOnlyCollection<TimeOfDay> Times
    {
      get { return _hoursOfTheDay; }
    }

    // TODO: This offends mine sight.  But it's used for moving the now highlight on the timeslot
    // track.  Is there a nicer way?
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    /// <remarks>This is used for binding, to provide change notifications as time passes.
    /// It returns the same value as DateTime.Now.</remarks>
    public DateTime Now
    {
      get { return DateTime.Now; }
    }

    /// <summary>
    /// Moves the view forward to the next block (e.g. next week).
    /// </summary>
    public abstract void IncrementView();

    /// <summary>
    /// Moves the view backward to the previous block (e.g. previous week).
    /// </summary>
    public abstract void DecrementView();

    /// <summary>
    /// Gets or sets the start date of the view.
    /// </summary>
    public abstract DateTime StartDate { get; set; }

    /// <summary>
    /// Gets the end date of the view.
    /// </summary>
    protected abstract DateTime EndDate { get; }

    /// <summary>
    /// Gets how the title should be formatted in the view.
    /// </summary>
    protected abstract DateDisplayMode DateRangeDisplayMode { get; }

    /// <summary>
    /// Gets display information for the view title.
    /// </summary>
    public DateRangeDisplayInfo DateRangeDisplayInfo
    {
      get
      {
        return new DateRangeDisplayInfo(StartDate, EndDate, DateRangeDisplayMode);
      }
    }

    internal virtual void OnLanguageChanged()
    {
      OnPropertyChanged("DateRangeDisplayInfo");
      OnPropertyChanged("StartDate");
    }

    /// <summary>
    /// When overridden in a derived class, gets a start date that is appropriate
    /// for that type of view to display the given date.
    /// </summary>
    /// <param name="date">The date to be displayed.</param>
    /// <returns>A date which is suitable as a start date for displaying the given date.</returns>
    protected abstract DateTime GetStartDateContaining(DateTime date);

    /// <summary>
    /// Gets or sets the <see cref="Schedule"/> being displayed.
    /// </summary>
    public Schedule Schedule
    {
      get { return _schedule; }
      protected set
      {
        _schedule = value;
        OnScheduleChanged();
      }
    }

    /// <summary>
    /// Called when the schedule of this <see cref="ScheduleView"/> changes.
    /// </summary>
    protected abstract void OnScheduleChanged();

    /// <summary>
    /// Gets whether the view needs to display the "wing" buttons (previous and next schedule item).
    /// </summary>
    public bool ShowWings
    {
      get
      {
        return (IsPreviousWingEnabled || IsNextWingEnabled)
          && !ViewModel.HasItems();
      }
    }

    internal abstract IScheduleViewModel ViewModel { get; }

    /// <summary>
    /// Gets whether the "previous item" wing button is enabled.
    /// </summary>
    public bool IsPreviousWingEnabled
    {
      get { return Schedule.EarliestItemDate < StartDate; }
    }

    /// <summary>
    /// Gets whether the "next item" wing button is enabled.
    /// </summary>
    public bool IsNextWingEnabled
    {
      get { return EndDate < Schedule.LatestItemDate; }
    }

    /// <summary>
    /// Gets a <see cref="DateRange"/> containing the selected start and end dates.
    /// If a <see cref="ScheduleItem"/> is selected, then the start and end times of the item will be returned instead.
    /// </summary>
    public abstract DateRange SelectedDateRange { get; }

    /// <summary>
    /// When overridden in a derived class, creates a <see cref="ScheduleItem"/>
    /// at the current selection.
    /// </summary>
    /// <returns>The new schedule item.</returns>
    internal abstract ScheduleItem CreateScheduleItemForAdd();

    /// <summary>
    /// When overridden in a derived class, cancels the addition of the specified
    /// <see cref="ScheduleItem"/>.
    /// </summary>
    /// <param name="item">The item to be cancelled.</param>
    protected abstract void CancelAdd(ScheduleItem item);

#if SILVERLIGHT
    private void AddScheduleItem() // TODO: Just make it internal like in WPF?
#else
    internal void AddScheduleItem()
#endif
    {
      DateRange range = SelectedDateRange;
      bool showDefaultEditor = true;
      IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
      Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
      Schedule schedule = view.Schedule;
      string name = (scheduler == null || scheduler.Formatter == null || scheduler.Formatter.DefaultScheduleItemName == null) ? "New Appointment" : scheduler.Formatter.DefaultScheduleItemName;
      if (view != null && schedule != null && schedule.ScheduleItemBuilder != null)
      {
        CreateScheduleItemArgs args = new CreateScheduleItemArgs(schedule, ScheduleItemCreationType.AddItemToolBarButton, range.StartDate, range.EndDate, name);
        CreateScheduleItemResult result = schedule.ScheduleItemBuilder.CreateScheduleItem(args);
        showDefaultEditor = result.AddDefaultItem && result.Item == null;
      }
      if (showDefaultEditor)
      {
        ScheduleItem item = new ScheduleItem() { Name = name };
        item.StartTime = range.StartDate;
        item.EndTime = range.EndDate;
        AddScheduleItemEventArgs addItemArgs = schedule.AddItemReturnArgs(item);
        showDefaultEditor = !addItemArgs.Cancel && addItemArgs.ShowDefaultEditor;
        if (showDefaultEditor)
        {
#if SILVERLIGHT
          ScheduleItemDialog dlg = new ScheduleItemDialog(item);
          dlg.Closed += new EventHandler(ScheduleItemDialog_Closed);
          dlg.Show();
#else
          ScheduleItemDialog dlg = new ScheduleItemDialog(item, scheduler == null ? DayOfWeek.Monday : scheduler.FirstDayOfWeek);
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
    }

    private void ScheduleItemDialog_Closed(object sender, EventArgs e)
    {
      ScheduleItemDialog dialog = sender as ScheduleItemDialog;
      if (dialog != null)
      {
        dialog.Closed -= new RoutedEventHandler(ScheduleItemDialog_Closed);
        if (dialog.DialogResult == false)
        {
          CancelAdd(dialog.ScheduleItem);
        }
      }
#if !SILVERLIGHT
      Dispatcher.BeginInvoke(new Action(EnsureFocus));
#endif
    }

#if !SILVERLIGHT
    private void EnsureFocus()
    {
      Focus();
    }
#endif

    #region Commands

#if SILVERLIGHT
    private readonly ICommand _previousCommand;
    private readonly ICommand _nextCommand;
    private readonly ICommand _addScheduleItemCommand;

    /// <summary>
    /// Gets a command for moving to the previous block (e.g. previous week).
    /// </summary>
    public ICommand PreviousCommand
    {
      get { return _previousCommand; }
    }

    /// <summary>
    /// Gets a command for moving to the next block (e.g. next week).
    /// </summary>
    public ICommand NextCommand
    {
      get { return _nextCommand; }
    }

    /// <summary>
    /// Gets a command for adding an item to the <see cref="Schedule"/> at the current
    /// selection.
    /// </summary>
    public ICommand AddScheduleItemCommand
    {
      get { return _addScheduleItemCommand; }
    }
#endif

#if SILVERLIGHT
    ICommand _gotoPreviousAppointmentCommand;

    /// <summary>
    /// Gets a command for moving to the first <see cref="ScheduleItem"/> prior
    /// to the start of this view.
    /// </summary>
    public ICommand PreviousItemCommand
    {
      get
      {
        if (_gotoPreviousAppointmentCommand == null)
          _gotoPreviousAppointmentCommand = new DelegateCommand(
              () => this.GotoPreviousAppointment(),
              () => this.CanGotoPreviousAppointment);

        return _gotoPreviousAppointmentCommand;
      }
    }
#endif

    private void GotoPreviousAppointment()
    {
      ScheduleItem scheduleItem = Schedule.GetPreviousEarliestItem(StartDate);
      if (scheduleItem != null)
      {
        StartDate = GetStartDateContaining(scheduleItem.StartTime);
        scheduleItem.IsSelected = true;
      }
    }

    private bool CanGotoPreviousAppointment
    {
      get { return IsPreviousWingEnabled; }
    }

#if SILVERLIGHT
    ICommand _gotoNextAppointmentCommand;

    /// <summary>
    /// Gets a command for moving to the next <see cref="ScheduleItem"/> after
    /// the end of this view.
    /// </summary>
    public ICommand NextItemCommand
    {
      get
      {
        if (_gotoNextAppointmentCommand == null)
          _gotoNextAppointmentCommand = new DelegateCommand(
              () => this.GotoNextAppointment(),
              () => this.CanGotoNextAppointment);

        return _gotoNextAppointmentCommand;
      }
    }
#endif

    private void GotoNextAppointment()
    {
      ScheduleItem scheduleItem = Schedule.GetNextEarliestItem(StartDate);
      StartDate = GetStartDateContaining(scheduleItem.StartTime);
      scheduleItem.IsSelected = true;
    }

    private bool CanGotoNextAppointment
    {
      get { return IsNextWingEnabled; }
    }

    #endregion //Commands

    /// <summary>
    /// Raises change notification events required when the view changes to
    /// displaying a different date range.
    /// </summary>
    protected void NotifyDateRangeChanged()
    {
#if SILVERLIGHT
      Dispatcher.BeginInvoke(FinalNotifyDateRangeChanged);
#else
      Dispatcher.BeginInvoke(new Action(FinalNotifyDateRangeChanged));
#endif
    }

    private void FinalNotifyDateRangeChanged()
    {
      OnPropertyChanged("DateRangeDisplayInfo");
      OnPropertyChanged("ShowWings");
      OnPropertyChanged("IsPreviousWingEnabled");
      OnPropertyChanged("IsNextWingEnabled");
#if SILVERLIGHT
      ((DelegateCommand)NextItemCommand).RaiseCanExecuteChanged();
      ((DelegateCommand)PreviousItemCommand).RaiseCanExecuteChanged();
#else
      CommandManager.InvalidateRequerySuggested();
#endif
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
  }
}
