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
using System.Diagnostics;
using System.ComponentModel;
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
#if !SILVERLIGHT
using Infralution.Licensing;
using System.Globalization;
using System.Windows.Markup;
using System.Reflection;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// A control which displays a schedule, for example of appointments or tasks,
  /// using a set of calendar views.
  /// </summary>
#if !SILVERLIGHT
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
#endif
  public class Scheduler : Control, INotifyPropertyChanged
  {
    private ScheduleView _currentView;

    private DayView _dayView;
    private WeekView _weekView;
    private MonthView _monthView;

    //TODO: would be nice if we didn't need to do this.
    // The problem is when navagating between views without using the view buttons, the toggle state of the view buttons
    // don't get updated.
    private RadioButton _dayViewButton;
    private RadioButton _weekViewButton;
    private RadioButton _monthViewButton;
    private RadioButton _workWeekRadioButton;
    private RadioButton _fullWeekRadioButton;
    private SchedulerNavigationBar _navBar;

    private readonly Schedule _calendar;

#if SILVERLIGHT
#else
    static Scheduler()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Scheduler),
        new FrameworkPropertyMetadata(typeof(Scheduler)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Scheduler"/> class.
    /// </summary>
    public Scheduler()
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(Scheduler);
      LicensingUtils.NotifyTrialVersion();
#else
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      BindCommands();
      Loaded += new RoutedEventHandler(Scheduler_Loaded);
      Unloaded += new RoutedEventHandler(Scheduler_Unloaded);
#endif
      Formatter = new SchedulerFormatter();
      _calendar = new Schedule();
      _calendar.WorkHours = WorkHours;
      _calendar.SelectedItemChanged += Schedule_SelectedItemChanged;
      _calendar.ItemAdded += new EventHandler<AddScheduleItemEventArgs>(Schedule_ItemAdded);
      _calendar.ItemRemoved += new EventHandler<ScheduleItemEventArgs>(Schedule_ItemRemoved);
      _calendar.RecurrencePatternSkipDatesChanged += new NotifyCollectionChangedEventHandler(Schedule_RecurrencePatternSkipDatesChanged);
      _calendar.RecurrencePatternRemoved += new EventHandler<ScheduleItemEventArgs>(Schedule_RecurrencePatternRemoved);

      _dayView = new DayView(_calendar, DateTime.Now);
      _weekView = new WeekView(_calendar, DateTime.Now.StartOfWeek(FirstDayOfWeek));
      _weekView.WeekViewModeChanged += new EventHandler(WeekView_WeekViewModeChanged);
      _monthView = new MonthView(_calendar);
      CurrentView = _weekView;
      int index = 0;
      foreach (DayModel day in _weekView.Week.Days)
      {
        if (index != 0)
        {
          day.DeselectAll();
        }
        else
        {
          day.SelectedStartTime = day.TimeSlots[0];
          day.SelectedEndTime = day.TimeSlots[0];
        }
        index++;
      }
    }

    /// <summary>
    /// Called when a property value changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);

      if ("Language".Equals(e.Property.Name))
      {
        _monthView.UpdateDayOfWeekNames(FirstDayOfWeek);
        _dayView.OnLanguageChanged();
        _weekView.OnLanguageChanged();
        _monthView.OnLanguageChanged();
        if (_navBar != null)
        {
          _navBar.OnPropertyChanged("DateRange");
        }
      }
    }

    private void WeekView_WeekViewModeChanged(object sender, EventArgs e)
    {
      UpdateWeekRadioButtons();
    }

    private void UpdateWeekRadioButtons()
    {
      if (_fullWeekRadioButton != null && _weekView.WeekViewMode == WeekViewMode.FullWeek)
      {
        _fullWeekRadioButton.IsChecked = true;
      }
      else if (_workWeekRadioButton != null && _weekView.WeekViewMode == WeekViewMode.WorkWeek)
      {
        _workWeekRadioButton.IsChecked = true;
      }
    }

#if !SILVERLIGHT
    private DateTime _currentDay;

    private void Scheduler_Loaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      if (Schedule != null)
      {
        if (!DateTimeUtils.IsSameDay(_currentDay, DateTime.Now))
        {
          Schedule.GetDay(_currentDay).UpdateIsCurrentDay();
          _currentDay = DateTime.Now;
          Schedule.GetDay(_currentDay).UpdateIsCurrentDay();
        }
      }
    }

    private void Scheduler_Unloaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick -= new EventHandler(Timer_Tick);
      if (_weekView != null)
      {
        _weekView.WeekViewModeChanged -= new EventHandler(WeekView_WeekViewModeChanged);
      }
      if (Schedule != null)
      {
        _calendar.SelectedItemChanged -= Schedule_SelectedItemChanged;
        _calendar.ItemAdded -= new EventHandler<AddScheduleItemEventArgs>(Schedule_ItemAdded);
        _calendar.ItemRemoved -= new EventHandler<ScheduleItemEventArgs>(Schedule_ItemRemoved);
        _calendar.RecurrencePatternSkipDatesChanged -= new NotifyCollectionChangedEventHandler(Schedule_RecurrencePatternSkipDatesChanged);
        _calendar.RecurrencePatternRemoved -= new EventHandler<ScheduleItemEventArgs>(Schedule_RecurrencePatternRemoved);
      }
    }
#endif

    /// <summary>
    /// Gets or sets the <see cref="DayOfWeek"/> that is the first day of every week in the <see cref="Scheduler"/>.
    /// This should typically be either Monday or Sunday.
    /// The default is Monday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FirstDayOfWeekProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DayOfWeek FirstDayOfWeek
    {
      get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
      set { SetValue(FirstDayOfWeekProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FirstDayOfWeek"/> property.
    /// </summary>
    public static readonly DependencyProperty FirstDayOfWeekProperty =
      DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek), typeof(Scheduler),
      new FrameworkPropertyMetadata(DayOfWeek.Monday, OnFirstDayOfWeekChanged));

    private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnFirstDayOfWeekChanged();
    }

    private void OnFirstDayOfWeekChanged()
    {
      if (_calendar != null)
      {
        _calendar.FirstDayOfWeek = FirstDayOfWeek;
      }
      if (_weekView != null)
      {
        _weekView.FirstDayOfWeek = FirstDayOfWeek;
      }
      if (_monthView != null)
      {
        _monthView.SetFirstDayOfWeek(FirstDayOfWeek);
      }
    }

#if !SILVERLIGHT

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(SchedulerCommands.DecrementViewCommand, DecrementView_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.IncrementViewCommand, IncrementView_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.ShowWorkWeekCommand, ShowWorkWeek_Executed, ShowWorkWeek_CanExecute));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.ShowFullWeekCommand, ShowFullWeek_Executed, ShowFullWeek_CanExecute));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.SwitchToDayViewCommand, SwitchToDayView_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.SwitchToWeekViewCommand, SwitchToWeekView_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.SwitchToMonthViewCommand, SwitchToMonthView_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.GoToDayCommand, GoToDay_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.GoToWeekCommand, GoToWeek_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.AddScheduleItemCommand, AddScheduleItem_Executed));
    }

    private void IncrementView_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      CurrentView.IncrementView();
    }

    private void DecrementView_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      CurrentView.DecrementView();
    }

    private void ShowWorkWeek_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ShowWorkWeek();
    }

    private void ShowWorkWeek_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanShowWorkWeek;
    }

    private void ShowFullWeek_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ShowFullWeek();
    }

    private void ShowFullWeek_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanShowFullWeek;
    }

    private void SwitchToDayView_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SwitchToDayView();
    }

    private void SwitchToWeekView_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SwitchToWeekView();
    }

    private void SwitchToMonthView_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SwitchToMonthView();
    }

    private void GoToDay_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      DayModel day = e.Parameter as DayModel;
      if (day != null)
      {
        SwitchToDay(day.Date.Date);
      }
    }

    private void GoToWeek_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      WeekModel week = e.Parameter as WeekModel;
      if (week != null)
      {
        SwitchToWeek(week.StartDate.Date);
      }
    }

    private void AddScheduleItem_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      CurrentView.AddScheduleItem();
    }
#endif

    private void Schedule_RecurrencePatternRemoved(object sender, ScheduleItemEventArgs e)
    {
      EventHandler<ScheduleItemEventArgs> handler = RecurrencePatternRemoved;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    private void Schedule_RecurrencePatternSkipDatesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventHandler handler = RecurrencePatternSkipDatesChanged;
      if (handler != null)
      {
        handler(sender, e);
      }
    }

    private void Schedule_ItemAdded(object sender, AddScheduleItemEventArgs e)
    {
      bool cancel;
      bool showDefaultEditor;
      OnItemAdded(e.Item, out cancel, out showDefaultEditor);
      e.Cancel = cancel;
      e.ShowDefaultEditor = showDefaultEditor;
    }

    private void Schedule_ItemRemoved(object sender, ScheduleItemEventArgs e)
    {
      e.Item.PropertyChanged -= new PropertyChangedEventHandler(ScheduleItem_PropertyChanged);
      EventHandler<ScheduleItemEventArgs> handler = ItemRemoved;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    private void ScheduleItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      EventHandler<ScheduleItemPropertyChangedEventArgs> handler = ItemPropertyChanged;
      ScheduleItem item = sender as ScheduleItem;

      if (!item.IsSilent && handler != null)
      {
        handler(this, new ScheduleItemPropertyChangedEventArgs(item, e.PropertyName));
      }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _dayViewButton = GetTemplateChild("PART_DayViewButton") as RadioButton;
      _weekViewButton = GetTemplateChild("PART_WeekViewButton") as RadioButton;
      _monthViewButton = GetTemplateChild("PART_MonthViewButton") as RadioButton;
      _workWeekRadioButton = GetTemplateChild("PART_WorkWeekRadioButton") as RadioButton;
      _fullWeekRadioButton = GetTemplateChild("PART_FullWeekRadioButton") as RadioButton;
      _navBar = GetTemplateChild("PART_SchedulerNavBar") as SchedulerNavigationBar;

#if !SILVERLIGHT
      UpdateViewButtons();
      UpdateWeekRadioButtons();
#endif
    }

    /// <summary>
    /// Gets the <see cref="Schedule"/> associated with this <see cref="Scheduler"/>.
    /// </summary>
    public Schedule Schedule { get { return _calendar; } }

    #region WorkHours property

    /// <summary>
    /// Gets or sets the work hours of the <see cref="Scheduler"/>. The default is 9:00am - 5:00pm.
    /// This is a dependency property.
    /// </summary>
    public WorkHours WorkHours
    {
      get { return (WorkHours)GetValue(WorkHoursProperty); }
      set { SetValue(WorkHoursProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WorkHours"/> property.
    /// </summary>
    public static readonly DependencyProperty WorkHoursProperty =
      DependencyProperty.Register("WorkHours", typeof(WorkHours), typeof(Scheduler),
      new PropertyMetadata(new WorkHours(), new PropertyChangedCallback(OnWorkHoursChanged)));

    private static void OnWorkHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnWorkHoursChanged();
    }

    private void OnWorkHoursChanged()
    {
      if (_calendar != null)
      {
        _calendar.WorkHours = WorkHours;
      }
    }

    #endregion // WorkHours property

    /// <summary>
    /// Gets whether the <see cref="Scheduler"/> is currently displaying a single day view.
    /// </summary>
    public bool IsInDayView { get { return CurrentView is DayView; } }

    /// <summary>
    /// Gets whether the <see cref="Scheduler"/> is currently displaying a weekly view.
    /// </summary>
    public bool IsInWeekView { get { return CurrentView is WeekView; } }

    /// <summary>
    /// Gets whether the <see cref="Scheduler"/> is currently displaying a month view.
    /// </summary>
    public bool IsInMonthView { get { return CurrentView is MonthView; } }

    #region IsReadOnly Property

    /// <summary>
    /// Gets or sets whether or not the <see cref="Scheduler"/> is read only. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsReadOnlyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsReadOnly
    {
      get { return (bool)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> property.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
      DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Scheduler),
      new FrameworkPropertyMetadata(OnIsReadOnlyChanged));

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnIsReadOnlyChanged();
    }

    private void OnIsReadOnlyChanged()
    {
    }

    #endregion // IsReadOnly Property

    #region Styling Properties

    /// <summary>
    /// Gets or sets the DayViewStyle.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DayViewStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style DayViewStyle
    {
      get { return (Style)GetValue(DayViewStyleProperty); }
      set { SetValue(DayViewStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DayViewStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty DayViewStyleProperty =
      DependencyProperty.Register("DayViewStyle", typeof(Style), typeof(Scheduler),
      new PropertyMetadata(OnDayViewStyleChanged));

    private static void OnDayViewStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnDayViewStyleChanged();
    }

    private void OnDayViewStyleChanged()
    {
      if (_dayView != null)
      {
        _dayView.Style = DayViewStyle;
      }
    }

    /// <summary>
    /// Gets or sets the WeekViewStyle.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WeekViewStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style WeekViewStyle
    {
      get { return (Style)GetValue(WeekViewStyleProperty); }
      set { SetValue(WeekViewStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WeekViewStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty WeekViewStyleProperty =
      DependencyProperty.Register("WeekViewStyle", typeof(Style), typeof(Scheduler),
      new PropertyMetadata(OnWeekViewStyleChanged));

    private static void OnWeekViewStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnWeekViewStyleChanged();
    }

    private void OnWeekViewStyleChanged()
    {
      if (_weekView != null)
      {
        _weekView.Style = WeekViewStyle;
      }
    }

    /// <summary>
    /// Gets or sets the MonthViewStyle.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthViewStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style MonthViewStyle
    {
      get { return (Style)GetValue(MonthViewStyleProperty); }
      set { SetValue(MonthViewStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthViewStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthViewStyleProperty =
      DependencyProperty.Register("MonthViewStyle", typeof(Style), typeof(Scheduler),
      new PropertyMetadata(OnMonthViewStyleChanged));

    private static void OnMonthViewStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnMonthViewStyleChanged();
    }

    private void OnMonthViewStyleChanged()
    {
      if (_monthView != null)
      {
        _monthView.Style = MonthViewStyle;
      }
    }

    #endregion

    #region Dialog customization properties

    #region ScheduleItemDialogStyle Property

    /// <summary>
    /// Gets or sets a custom style for the schedule item dialog.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScheduleItemDialogStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style ScheduleItemDialogStyle
    {
      get { return (Style)GetValue(ScheduleItemDialogStyleProperty); }
      set { SetValue(ScheduleItemDialogStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScheduleItemDialogStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty ScheduleItemDialogStyleProperty =
      DependencyProperty.Register("ScheduleItemDialogStyle", typeof(Style), typeof(Scheduler),
      new FrameworkPropertyMetadata(OnScheduleItemDialogStyleChanged));

    private static void OnScheduleItemDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnScheduleItemDialogStyleChanged();
    }

    private void OnScheduleItemDialogStyleChanged()
    {
    }

    #endregion // ScheduleItemDialogStyle Property

    #region RecurrenceDialogStyle Property

    /// <summary>
    /// Gets or set a custom style for the recurrence dialog.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RecurrenceDialogStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style RecurrenceDialogStyle
    {
      get { return (Style)GetValue(RecurrenceDialogStyleProperty); }
      set { SetValue(RecurrenceDialogStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RecurrenceDialogStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty RecurrenceDialogStyleProperty =
      DependencyProperty.Register("RecurrenceDialogStyle", typeof(Style), typeof(Scheduler),
      new FrameworkPropertyMetadata(OnRecurrenceDialogStyleChanged));

    private static void OnRecurrenceDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnRecurrenceDialogStyleChanged();
    }

    private void OnRecurrenceDialogStyleChanged()
    {
    }

    #endregion // RecurrenceDialogStyle Property

    #region DeleteRecurrenceDialogStyle Property

    /// <summary>
    /// Gets or sets a custom style for the recurrence deletion dialog.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DeleteRecurrenceDialogStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style DeleteRecurrenceDialogStyle
    {
      get { return (Style)GetValue(DeleteRecurrenceDialogStyleProperty); }
      set { SetValue(DeleteRecurrenceDialogStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DeleteRecurrenceDialogStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty DeleteRecurrenceDialogStyleProperty =
      DependencyProperty.Register("DeleteRecurrenceDialogStyle", typeof(Style), typeof(Scheduler),
      new FrameworkPropertyMetadata(OnDeleteRecurrenceDialogStyleChanged));

    private static void OnDeleteRecurrenceDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnDeleteRecurrenceDialogStyleChanged();
    }

    private void OnDeleteRecurrenceDialogStyleChanged()
    {
    }

    #endregion // DeleteRecurrenceDialogStyle Property

    #endregion // Dialog customization properties

    #region Formatter property
    
    /// <summary>
    /// Gets or sets the <see cref="SchedulerFormatter"/> used to style various parts of the <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    public SchedulerFormatter Formatter
    {
      get { return (SchedulerFormatter)GetValue(FormatterProperty); }
      set { SetValue(FormatterProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Formatter"/> property.
    /// </summary>
    public static readonly DependencyProperty FormatterProperty =
      DependencyProperty.Register("Formatter", typeof(SchedulerFormatter), typeof(Scheduler),
      new PropertyMetadata(new PropertyChangedCallback(OnFormatterChanged)));

    private static void OnFormatterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnFormatterChanged();
    }

    private void OnFormatterChanged()
    {
    }

    #endregion // Formatter property

    /// <summary>
    /// Gets the currently displayed view.
    /// </summary>
    public ScheduleView CurrentView
    {
      get { return _currentView; }
      private set
      {
        _currentView = value;
        OnPropertyChanged("CurrentView");
        OnPropertyChanged("IsInDayView");
        OnPropertyChanged("IsInWeekView");
        OnPropertyChanged("IsInMonthView");
      }
    }

#if !SILVERLIGHT
    private void UpdateViewButtons()
    {
      if (CurrentView is DayView && _dayViewButton != null)
      {
        _dayViewButton.IsChecked = true;
      }
      else if (CurrentView is WeekView && _weekViewButton != null)
      {
        _weekViewButton.IsChecked = true;
      }
      else if (CurrentView is MonthView && _monthViewButton != null)
      {
        _monthViewButton.IsChecked = true;
      }
    }
#endif

    #region Commands

    /// <summary>
    /// Switches the <see cref="Scheduler"/> into single-day view.
    /// </summary>
    /// <param name="date">The day to display.</param>
    internal void SwitchToDay(DateTime date)
    {
      _dayView.StartDate = date;
      CurrentView = _dayView;
      if (_dayViewButton != null)
      {
        _dayViewButton.IsChecked = true;
      }
    }

    /// <summary>
    /// Switches the <see cref="Scheduler"/> into weekly view.
    /// </summary>
    /// <param name="startDate">The start date of the week.</param>
    internal void SwitchToWeek(DateTime startDate)
    {
      //TODO: make sure given date is the START of a week.
      _weekView.StartDate = startDate;
      CurrentView = _weekView;
      if (_weekViewButton != null)
      {
        _weekViewButton.IsChecked = true;
      }
      int index = 0;
      foreach (DayModel day in _weekView.Week.Days)
      {
        if (index != 0)
        {
          day.DeselectAll();
        }
        else
        {
          day.SelectedStartTime = day.TimeSlots[0];
          day.SelectedEndTime = day.TimeSlots[0];
        }
        index++;
      }
    }

    #region SwitchToDayViewCommand

#if SILVERLIGHT
    ICommand _switchToDayViewCommand;

    /// <summary>
    /// A command that can be sent to the <see cref="Scheduler"/> to switch it into single-day view.
    /// </summary>
    public ICommand SwitchToDayViewCommand
    {
      get
      {
        if (_switchToDayViewCommand == null)
          _switchToDayViewCommand = new DelegateCommand(
              () => this.SwitchToDayView(),
              () => this.CanSwitchToDayView);

        return _switchToDayViewCommand;
      }
    }
#endif

    private void SwitchToDayView()
    {
      if (CurrentView is DayView)
      {
        return;
      }
      if (IsInWeekView)
      {
        DateTime selectedDate = _weekView.Week.Days[0].Date;
        if (_weekView.Week.FirstSelectedDay != null)
        {
          selectedDate = _weekView.Week.FirstSelectedDay.Date;
        }
        else if (_calendar.SelectedItem != null)
        {
          selectedDate = _calendar.SelectedItem.StartTime;
        }
        _dayView.StartDate = selectedDate;
        CurrentView = _dayView;
        DayModel selectedDay = _calendar.GetDay(selectedDate);
        if (_calendar.SelectedItem == null)
        {
          _dayView.Day.SelectedStartTime = selectedDay.SelectedStartTime;
          _dayView.Day.SelectedEndTime = selectedDay.SelectedEndTime;
        }
        else
        {
          _dayView.Day.SelectedStartTime = _dayView.Day.TimeSlots[0];
          _dayView.Day.SelectedEndTime = _dayView.Day.TimeSlots[0];
        }
      }
      else if (IsInMonthView)
      {
        DateTime firstDate = _monthView.Month.SelectedStartDay.Date;
        if (!_monthView.Month.SelectedStartDay.IsSelected)
        {
          ScheduleItem selectedItem = _calendar.SelectedItem;
          if (selectedItem != null)
          {
            firstDate = selectedItem.StartTime;
          }
        }
        _dayView.StartDate = firstDate;// _monthView.StartDate;
        CurrentView = _dayView;
        _dayView.Day.SelectedStartTime = _dayView.Day.TimeSlots[0];
        _dayView.Day.SelectedEndTime = _dayView.Day.TimeSlots[0];
      }
    }

    //private bool CanSwitchToDayView
    //{
    //  get { return true; }
    //}

    #endregion //SwitchToDayViewCommand

    #region SwitchToWeekViewCommand

#if SILVERLIGHT
    ICommand _switchToWeekViewCommand;

    /// <summary>
    /// A command that can be sent to the <see cref="Scheduler"/> to switch it into week view.
    /// </summary>
    public ICommand SwitchToWeekViewCommand
    {
      get
      {
        if (_switchToWeekViewCommand == null)
          _switchToWeekViewCommand = new DelegateCommand(
              () => this.SwitchToWeekView(),
              () => this.CanSwitchToWeekView);

        return _switchToWeekViewCommand;
      }
    }
#endif

    private void SwitchToWeekView()
    {
      if (IsInWeekView)
      {
        return;
      }
      if (IsInDayView)
      {
        DateTime date = _dayView.StartDate;
        DateTime newDate = date.StartOfWeek(FirstDayOfWeek);
        _weekView.Week.DeselectAllDays();
        _weekView.StartDate = newDate;
        CurrentView = _weekView;
        foreach (DayModel day in _weekView.Week.Days)
        {
          day.DeselectAll();
          if (day.Date.Date.Equals(date.Date))
          {
            if (_calendar.SelectedItem == null)
            {
              day.SelectedStartTime = _dayView.Day.SelectedStartTime;
              day.SelectedEndTime = _dayView.Day.SelectedEndTime;
            }
            else
            {
              day.SelectedStartTime = day.TimeSlots[0];
              day.SelectedEndTime = day.TimeSlots[0];
            }
          }
        }
        if (!_weekView.Week.Days.Contains(_dayView.Day))
        {
          _weekView.Week.Days[0].SelectedStartTime = _weekView.Week.Days[0].TimeSlots[0];
          _weekView.Week.Days[0].SelectedEndTime = _weekView.Week.Days[0].TimeSlots[0];
        }
      }
      else if (IsInMonthView)
      {
        DayModel selectedDay = null;
        if (_calendar.SelectedItem != null)
        {
          selectedDay = _calendar.GetDay(_calendar.SelectedItem.StartTime);
          _weekView.StartDate = selectedDay.Date.StartOfWeek(FirstDayOfWeek);
        }
        else
        {
          selectedDay = _calendar.GetDay(_monthView.Month.SelectedStartDay.Date);
          _weekView.StartDate = selectedDay.Date.StartOfWeek(FirstDayOfWeek);
        }
        int index = 0;
        {
          foreach (DayModel day in _weekView.Week.Days)
          {
            if (day != selectedDay)
            {
              day.DeselectAll();
            }
            else
            {
              day.SelectedStartTime = day.TimeSlots[0];
              day.SelectedEndTime = day.TimeSlots[0];
            }
            index++;
          }
        }

        if (!_weekView.Week.Days.Contains(selectedDay))
        {
          _weekView.Week.Days[0].SelectedStartTime = _weekView.Week.Days[0].TimeSlots[0];
          _weekView.Week.Days[0].SelectedEndTime = _weekView.Week.Days[0].TimeSlots[0];
        }
        CurrentView = _weekView;
      }
    }

    //private bool CanSwitchToWeekView
    //{
    //  get { return true; }
    //}

    #endregion //SwitchToWeekViewCommand

    #region SwitchToMonthViewCommand

#if SILVERLIGHT
    ICommand _switchToMonthViewCommand;

    /// <summary>
    /// A command that can be sent to the <see cref="Scheduler"/> to switch it into month view.
    /// </summary>
    public ICommand SwitchToMonthViewCommand
    {
      get
      {
        if (_switchToMonthViewCommand == null)
          _switchToMonthViewCommand = new DelegateCommand(
              () => this.SwitchToMonthView(),
              () => this.CanSwitchToMonthView);

        return _switchToMonthViewCommand;
      }
    }
#endif

    private void SwitchToMonthView()
    {
      if (IsInMonthView)
      {
        return;
      }
      if (IsInDayView)
      {
        _monthView.StartDate = _dayView.StartDate.StartOfMonth();
        _monthView.Month.SelectedStartDay = _dayView.Day;
        _monthView.Month.SelectedEndDay = _dayView.Day;
        CurrentView = _monthView;
      }
      else if (IsInWeekView)
      {
        DayModel firstSelected = _weekView.Week.Days[0];
        if (_weekView.Week.FirstSelectedDay != null)
        {
          firstSelected = _weekView.Week.FirstSelectedDay;
        }
        else if (_calendar.SelectedItem != null)
        {
          firstSelected = _calendar.GetDay(_calendar.SelectedItem.StartTime);
        }
        _monthView.StartDate = firstSelected.Date.StartOfMonth();
        _monthView.Month.SelectedStartDay = firstSelected;
        _monthView.Month.SelectedEndDay = firstSelected;
        CurrentView = _monthView;
      }
    }

    //private bool CanSwitchToMonthView
    //{
    //  get { return true; }
    //}

    #endregion //SwitchToMonthViewCommand

    #region ShowWorkWeekCommand

#if SILVERLIGHT
    ICommand _showWorkWeekCommand;

    /// <summary>
    /// A command that can be sent to the <see cref="Scheduler"/> to switch the week view into
    /// work week only mode.
    /// </summary>
    public ICommand ShowWorkWeekCommand
    {
      get
      {
        if (_showWorkWeekCommand == null)
          _showWorkWeekCommand = new DelegateCommand(
              () => this.ShowWorkWeek(),
              () => this.CanShowWorkWeek);

        return _showWorkWeekCommand;
      }
    }
#endif

    private void ShowWorkWeek()
    {
      _weekView.WeekViewMode = WeekViewMode.WorkWeek;
    }

    private bool CanShowWorkWeek
    {
      get { return IsInWeekView; }
    }

    #endregion //ShowWorkWeekCommand

    #region ShowFullWeekCommand

#if SILVERLIGHT
    ICommand _showFullWeekCommand;

    /// <summary>
    /// A command that can be sent to the <see cref="Scheduler"/> to switch the week view into
    /// full week mode.
    /// </summary>
    public ICommand ShowFullWeekCommand
    {
      get
      {
        if (_showFullWeekCommand == null)
          _showFullWeekCommand = new DelegateCommand(
              () => this.ShowFullWeek(),
              () => this.CanShowFullWeek);

        return _showFullWeekCommand;
      }
    }
#endif

    private void ShowFullWeek()
    {
      _weekView.WeekViewMode = WeekViewMode.FullWeek;
    }

    private bool CanShowFullWeek
    {
      get { return IsInWeekView; }
    }

    #endregion //ShowWorkWeekCommand

    /// <summary>
    /// Called by the framework when the user presses a key.
    /// </summary>
    /// <param name="e">The key event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      //base.OnKeyDown(e);

      if (e.Key == Key.Delete)
      {
        ProcessDeleteKey();
      }
    }

    private void ProcessDeleteKey()
    {
      if (!IsReadOnly)
      {
        ScheduleItem lastSelected = _calendar.SelectedItem;
        if (lastSelected != null)
        {
          if (lastSelected.IsInstanceOfRecurringItem)
          {
#if SILVERLIGHT
          DeleteRecurrenceDialog dialog = new DeleteRecurrenceDialog(lastSelected);
          dialog.Closed += new EventHandler(DeleteRecurrenceDialog_Closed);
          dialog.Show();
#else
            DeleteRecurrenceDialog dlg = new DeleteRecurrenceDialog(lastSelected);
            if (DeleteRecurrenceDialogStyle != null)
            {
              dlg.Style = DeleteRecurrenceDialogStyle;
            }
            dlg.Closed += new RoutedEventHandler(DeleteRecurrenceDialog_Closed);
            DialogHelper.ShowDeleteRecurrenceDialog(dlg, this, Formatter);
#endif
          }
          else
          {
            _calendar.RemoveItem(lastSelected);
            ApplyDefaultSelection(lastSelected);
          }
        }
      }
    }

    // Selects a day or a time slot based on the given ScheduleItem.
    // This is to maintain the existance of a selection after a selected item has been deleted.
    private void ApplyDefaultSelection(ScheduleItem item)
    {
      if (IsInDayView)
      {
        _dayView.Day.SelectedStartTime = _dayView.Day.TimeSlots[0];
        _dayView.Day.SelectedEndTime = _dayView.Day.TimeSlots[0];
      }
      else if (IsInWeekView)
      {
        int index = 0;
        foreach (DayModel day in _weekView.Week.Days)
        {
          if (index != 0)
          {
            day.DeselectAll();
          }
          else
          {
            day.SelectedStartTime = day.TimeSlots[0];
            day.SelectedEndTime = day.TimeSlots[0];
          }
          index++;
        }
      }
      else if (IsInMonthView && item != null)
      {
        DayModel selectedDay = _calendar.GetDay(item.StartTime);
        _monthView.Month.SelectedStartDay = selectedDay;
        _monthView.Month.SelectedEndDay = selectedDay;
      }
    }

    private void DeleteRecurrenceDialog_Closed(object sender, EventArgs e)
    {
      DeleteRecurrenceDialog dialog = sender as DeleteRecurrenceDialog;
      if (dialog != null)
      {
        if (dialog.DialogResult == true)
        {
          _calendar.RemoveItem(dialog.ScheduleItem, dialog.DeleteSeries);
          ApplyDefaultSelection(dialog.ScheduleItem);
        }
      }
    }

    #endregion

    #region ToolBarContent property

    /// <summary>
    /// Gets or sets the ToolBarContent.
    /// This is a dependency property.
    /// </summary>
    public object ToolBarContent
    {
      get { return GetValue(ToolBarContentProperty); }
      set { SetValue(ToolBarContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ToolBarContent"/> property.
    /// </summary>
    public static readonly DependencyProperty ToolBarContentProperty =
      DependencyProperty.Register("ToolBarContent", typeof(object), typeof(Scheduler),
      new PropertyMetadata(new PropertyChangedCallback(OnToolBarContentChanged)));

    private static void OnToolBarContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scheduler)d).OnToolBarContentChanged();
    }

    private void OnToolBarContentChanged()
    {
    }

    #endregion // ToolBarContent property

    /// <summary>
    /// Gets the currently selected <see cref="ScheduleItem"/>.
    /// </summary>
    public ScheduleItem SelectedItem
    {
      get
      {
        return _calendar.SelectedItem;
      }
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

    /// <summary>
    /// Occurs when the user activates an item (for example by double-clicking).
    /// </summary>
    public event EventHandler<ScheduleItemEventArgs> ItemActivated;

    /// <summary>
    /// Raises the <see cref="ItemActivated"/> event.
    /// </summary>
    /// <param name="item">The item being activated.</param>
    protected internal virtual void OnItemActivated(ScheduleItem item)
    {
      //Debug.WriteLine("ITEM COUNT: " + _calendar.Items.Count);
      var handler = ItemActivated;
      if (handler != null)
      {
        handler(this, new ScheduleItemEventArgs(item));
      }
    }

    /// <summary>
    /// Occurs when the user deletes a schedule item from the <see cref="Scheduler"/>.
    /// </summary>
    public event EventHandler<ScheduleItemEventArgs> ItemRemoved;

    /// <summary>
    /// Occurs when the user adds a schedule item to the <see cref="Scheduler"/>.
    /// </summary>
    public event EventHandler<AddScheduleItemEventArgs> ItemAdded;

    /// <summary>
    /// Occurs when a property changes on a <see cref="ScheduleItem"/> such as its name or start time.
    /// </summary>
    public event EventHandler<ScheduleItemPropertyChangedEventArgs> ItemPropertyChanged;

    /// <summary>
    /// Occurs when the SkipDates collection of a <see cref="RecurrencePattern"/> changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler RecurrencePatternSkipDatesChanged;

    /// <summary>
    /// Occurs when a recurrence pattern is removed from the schedule.
    /// </summary>
    public event EventHandler<ScheduleItemEventArgs> RecurrencePatternRemoved;

    /// <summary>
    /// Raises the <see cref="ItemAdded"/> event.
    /// </summary>
    /// <param name="item">The item being added.</param>
    /// <param name="cancel">true to cancel the add (and remove the item again); false to permit the new item.</param>
    /// <param name="showDefaultEditor">true to show the default editor (<see cref="ScheduleItemDialog"/>); false
    /// to suppress the default editor.</param>
    protected internal virtual void OnItemAdded(ScheduleItem item, out bool cancel, out bool showDefaultEditor)
    {
      showDefaultEditor = true;
      cancel = false;
      item.PropertyChanged += new PropertyChangedEventHandler(ScheduleItem_PropertyChanged);

      var handler = ItemAdded;
      if (handler != null)
      {
        AddScheduleItemEventArgs args = new AddScheduleItemEventArgs(item);
        handler(this, args);
        showDefaultEditor = args.ShowDefaultEditor;
        cancel = args.Cancel;
      }
    }

    private void Schedule_SelectedItemChanged(object sender, EventArgs e)
    {
      OnPropertyChanged("SelectedItem");
    }
  }
}
