using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides a user interface for editing the recurrence pattern of a recurring
  /// <see cref="ScheduleItem"/>.
  /// </summary>
  public partial class RecurrenceDialog :
#if SILVERLIGHT
    ChildWindow
#else
    UserControl
#endif
  {
    private ScheduleItem _item;
    private bool _changingDuration;
    private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;
    private bool _canRemoveRecurrence;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurrenceDialog"/> class.
    /// </summary>
    /// <param name="item">The item to be edited.</param>
    /// <param name="currentInfo">The current recurrence settings for the item.</param>
    public RecurrenceDialog(ScheduleItem item, RecurrenceInfo currentInfo)
      : this(item, currentInfo, DayOfWeek.Monday)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurrenceDialog"/> class.
    /// </summary>
    /// <param name="item">The item to be edited.</param>
    /// <param name="currentInfo">The current recurrence settings for the item.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    public RecurrenceDialog(ScheduleItem item, RecurrenceInfo currentInfo, DayOfWeek firstDayOfWeek)
    {
      InitializeComponent();

      BindCommands();

      _firstDayOfWeek = firstDayOfWeek;
      _item = item;
      RecurrenceInfo = currentInfo;
#if !SILVERLIGHT
      _changingDuration = false;
#endif
      _canRemoveRecurrence = currentInfo != null;

      SetValue(TimeListPropertyKey, BuildTimeList(0, 0));
      SetValue(DurationListPropertyKey, BuildDurationList());
      SetValue(MonthListPropertyKey, BuildMonthList());

      StartTime = new TimeOfDay(_item.StartTime.Hour, _item.StartTime.Minute);
      Duration = _item.EndTime - _item.StartTime;

      SetValue(OccurrenceListPropertyKey, BuildOccurrenceList());
      SetValue(DayOfRecurrenceListPropertyKey, BuildDayOfRecurrenceList());

      ResetWeeklyInfo();
      ResetMonthlyInfo();
      ResetYearlyInfo();

      if (currentInfo != null)
      {
        switch (currentInfo.EndType)
        {
          case RecurrenceEndType.NoEndDate:
            IsInfinite = true;
            break;
          case RecurrenceEndType.EndAfter:
            IsLimitedByCount = true;
            break;
          case RecurrenceEndType.EndBy:
            IsLimitedByDate = true;
            break;
        }
      }

#if SILVERLIGHT
      StartDateEditor.SelectedDate = _item.StartTime;
      EndDateEditor.SelectedDate = _item.StartTime.AddMonths(1);
#else
      StartDate = _item.StartTime;
      EndDate = _item.StartTime.AddMonths(1);
#endif

      IsLimitedByCount = true;

      Loaded += new RoutedEventHandler(RecurrenceDialog_Loaded);

      DataContext = this;
    }

    private void RecurrenceDialog_Loaded(object sender, RoutedEventArgs e)
    {
      Window wnd = Parent as Window;
      if (wnd != null)
      {
        wnd.Title = Title;
      }
    }

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(SchedulerCommands.OkCommand, Ok_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.CancelCommand, Cancel_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.RemoveRecurrenceCommand, RemoveRecurrence_Executed, RemoveRecurrence_CanExecute));
    }

    private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      RecurrenceInfo = BuildRecurrenceInfo();
      this.DialogResult = true;
    }

    private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    private void RemoveRecurrence_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      RecurrenceInfo = null;
      this.DialogResult = false;
    }

    private void RemoveRecurrence_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = _canRemoveRecurrence;
    }

    #region Title Property

    /// <summary>
    /// Gets or sets the Title.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TitleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata("Appointment Recurrence", OnTitleChanged));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
      Window wnd = Parent as Window;
      if (wnd != null)
      {
        wnd.Title = Title;
      }
    }

    #endregion // Title Property

    #region StartTime Property

    /// <summary>
    /// Gets or sets the time of day that each schedule item starts at.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeOfDay StartTime
    {
      get { return (TimeOfDay)GetValue(StartTimeProperty); }
      set { SetValue(StartTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartTime"/> property.
    /// </summary>
    public static readonly DependencyProperty StartTimeProperty =
      DependencyProperty.Register("StartTime", typeof(TimeOfDay), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnStartTimeChanged));

    private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnStartTimeChanged();
    }

    private void OnStartTimeChanged()
    {
      if (!_changingDuration)
      {
        UpdateDuration();
      }
    }

    #endregion // StartTime Property

    #region EndTime Property

    /// <summary>
    /// Gets or sets the time of day that each schedule item ends at.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EndTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeOfDay EndTime
    {
      get { return (TimeOfDay)GetValue(EndTimeProperty); }
      set { SetValue(EndTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndTime"/> property.
    /// </summary>
    public static readonly DependencyProperty EndTimeProperty =
      DependencyProperty.Register("EndTime", typeof(TimeOfDay), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnEndTimeChanged));

    private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnEndTimeChanged();
    }

    private void OnEndTimeChanged()
    {
      if (!_changingDuration)
      {
        UpdateDuration();
      }
    }

    #endregion // EndTime Property

    #region Duration Property

    /// <summary>
    /// Gets or sets the duration of a single recurrence item.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DurationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeSpan Duration
    {
      get { return (TimeSpan)GetValue(DurationProperty); }
      set { SetValue(DurationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Duration"/> property.
    /// </summary>
    public static readonly DependencyProperty DurationProperty =
      DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnDurationChanged));

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnDurationChanged();
    }

    private void OnDurationChanged()
    {
      if (!_changingDuration)
      {
        _changingDuration = true;
        DateTime startTime = new DateTime(2000, 1, 1, StartTime.Hour, StartTime.Minute, 0);
        DateTime endTime = startTime + Duration;
        EndTime = new TimeOfDay(endTime.Hour, endTime.Minute);
        _changingDuration = false;
      }
    }

    private void UpdateDuration()
    {
      DateTime startTime = new DateTime(2000, 1, 1, StartTime.Hour, StartTime.Minute, 0);
      DateTime endTime = new DateTime(2000, 1, 1, EndTime.Hour, EndTime.Minute, 0);
      if (StartTime >= EndTime)
      {
        endTime = new DateTime(2000, 1, 2, EndTime.Hour, EndTime.Minute, 0);
      }
      Duration = endTime - startTime;
    }

    #endregion // Duration Property

    #region TimeList Property

    /// <summary>
    /// Gets the list of times displayed by the time pickers.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TimeListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<TimeOfDay> TimeList
    {
      get { return (ReadOnlyCollection<TimeOfDay>)GetValue(TimeListProperty); }
    }

    private static readonly DependencyPropertyKey TimeListPropertyKey =
        DependencyProperty.RegisterReadOnly("TimeList", typeof(ReadOnlyCollection<TimeOfDay>), typeof(RecurrenceDialog), new UIPropertyMetadata(new List<TimeOfDay>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="TimeList"/> property.
    /// </summary>
    public static readonly DependencyProperty TimeListProperty =
        TimeListPropertyKey.DependencyProperty;

    #endregion // TimeList Property

    #region DurationList Property

    /// <summary>
    /// Gets the list of durations displayed in the duration picker.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DurationListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<TimeSpan> DurationList
    {
      get { return (ReadOnlyCollection<TimeSpan>)GetValue(DurationListProperty); }
    }

    private static readonly DependencyPropertyKey DurationListPropertyKey =
        DependencyProperty.RegisterReadOnly("DurationList", typeof(ReadOnlyCollection<TimeSpan>), typeof(RecurrenceDialog), new UIPropertyMetadata(new List<TimeSpan>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="DurationList"/> property.
    /// </summary>
    public static readonly DependencyProperty DurationListProperty =
        DurationListPropertyKey.DependencyProperty;

    #endregion // DurationList Property

    #region StartDate Property

    /// <summary>
    /// Gets or sets the StartDate.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartDateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime StartDate
    {
      get { return (DateTime)GetValue(StartDateProperty); }
      set { SetValue(StartDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartDate"/> property.
    /// </summary>
    public static readonly DependencyProperty StartDateProperty =
      DependencyProperty.Register("StartDate", typeof(DateTime), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnStartDateChanged));

    private static void OnStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnStartDateChanged();
    }

    private void OnStartDateChanged()
    {
    }

    #endregion // StartDate Property

    #region EndDate Property

    /// <summary>
    /// Gets or sets the EndDate.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EndDateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime EndDate
    {
      get { return (DateTime)GetValue(EndDateProperty); }
      set { SetValue(EndDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndDate"/> property.
    /// </summary>
    public static readonly DependencyProperty EndDateProperty =
      DependencyProperty.Register("EndDate", typeof(DateTime), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnEndDateChanged));

    private static void OnEndDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnEndDateChanged();
    }

    private void OnEndDateChanged()
    {
      IsLimitedByDate = true;
    }

    #endregion // EndDate Property

    #region MaxOccurrences Property

    /// <summary>
    /// Gets or sets the maximum number of occurrences when using a recurrence pattern that is limited by an occurrence count.
    /// The default is 10.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxOccurrencesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MaxOccurrences
    {
      get { return (int)GetValue(MaxOccurrencesProperty); }
      set { SetValue(MaxOccurrencesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxOccurrences"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxOccurrencesProperty =
      DependencyProperty.Register("MaxOccurrences", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(10, OnMaxOccurrencesChanged));

    private static void OnMaxOccurrencesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMaxOccurrencesChanged();
    }

    private void OnMaxOccurrencesChanged()
    {
      IsLimitedByCount = true;
    }

    #endregion // MaxOccurrences Property

    #region OccurrenceList Property

    /// <summary>
    /// Gets the <see cref="Occurrence"/> list.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OccurrenceListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<Occurrence> OccurrenceList
    {
      get { return (ReadOnlyCollection<Occurrence>)GetValue(OccurrenceListProperty); }
    }

    private static readonly DependencyPropertyKey OccurrenceListPropertyKey =
        DependencyProperty.RegisterReadOnly("OccurrenceList", typeof(ReadOnlyCollection<Occurrence>), typeof(RecurrenceDialog), new UIPropertyMetadata(new List<Occurrence>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="OccurrenceList"/> property.
    /// </summary>
    public static readonly DependencyProperty OccurrenceListProperty =
        OccurrenceListPropertyKey.DependencyProperty;

    #endregion // OccurrenceList Property

    #region DayOfRecurrenceList Property

    /// <summary>
    /// Gets the <see cref="DayOfRecurrence"/> list.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DayOfRecurrenceListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<DayOfRecurrence> DayOfRecurrenceList
    {
      get { return (ReadOnlyCollection<DayOfRecurrence>)GetValue(DayOfRecurrenceListProperty); }
    }

    private static readonly DependencyPropertyKey DayOfRecurrenceListPropertyKey =
        DependencyProperty.RegisterReadOnly("DayOfRecurrenceList", typeof(ReadOnlyCollection<DayOfRecurrence>), typeof(RecurrenceDialog), new UIPropertyMetadata(new List<DayOfRecurrence>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="DayOfRecurrenceList"/> property.
    /// </summary>
    public static readonly DependencyProperty DayOfRecurrenceListProperty =
        DayOfRecurrenceListPropertyKey.DependencyProperty;

    #endregion // DayOfRecurrenceList Property

    #region MonthList Property

    /// <summary>
    /// Gets a list of integers from 0 to 11. This is for the month picker.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<int> MonthList
    {
      get { return (ReadOnlyCollection<int>)GetValue(MonthListProperty); }
    }

    private static readonly DependencyPropertyKey MonthListPropertyKey =
        DependencyProperty.RegisterReadOnly("MonthList", typeof(ReadOnlyCollection<int>), typeof(RecurrenceDialog), new UIPropertyMetadata(new List<int>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="MonthList"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthListProperty =
        MonthListPropertyKey.DependencyProperty;

    #endregion // MonthList Property

    #region End type properties

    #region IsInfinite Property

    /// <summary>
    /// Gets or sets whether or not the recurrence pattern will recur forever.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsInfiniteProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsInfinite
    {
      get { return (bool)GetValue(IsInfiniteProperty); }
      set { SetValue(IsInfiniteProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsInfinite"/> property.
    /// </summary>
    public static readonly DependencyProperty IsInfiniteProperty =
      DependencyProperty.Register("IsInfinite", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsInfiniteChanged));

    private static void OnIsInfiniteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsInfiniteChanged();
    }

    private void OnIsInfiniteChanged()
    {
      if (IsInfinite)
      {
        IsLimitedByCount = false;
        IsLimitedByDate = false;
      }
    }

    #endregion // IsInfinite Property

    #region IsLimitedByCount Property

    /// <summary>
    /// Gets or sets whether or not the recurrence pattern will be limited by a maximum occurrence count.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsLimitedByCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsLimitedByCount
    {
      get { return (bool)GetValue(IsLimitedByCountProperty); }
      set { SetValue(IsLimitedByCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsLimitedByCount"/> property.
    /// </summary>
    public static readonly DependencyProperty IsLimitedByCountProperty =
      DependencyProperty.Register("IsLimitedByCount", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnIsLimitedByCountChanged));

    private static void OnIsLimitedByCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsLimitedByCountChanged();
    }

    private void OnIsLimitedByCountChanged()
    {
      if (IsLimitedByCount)
      {
        IsInfinite = false;
        IsLimitedByDate = false;
      }
    }

    #endregion // IsLimitedByCount Property

    #region IsLimitedByDate Property

    /// <summary>
    /// Gets or sets whether or not the recurrence pattern will end on a particular date.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsLimitedByDateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsLimitedByDate
    {
      get { return (bool)GetValue(IsLimitedByDateProperty); }
      set { SetValue(IsLimitedByDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsLimitedByDate"/> property.
    /// </summary>
    public static readonly DependencyProperty IsLimitedByDateProperty =
      DependencyProperty.Register("IsLimitedByDate", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsLimitedByDateChanged));

    private static void OnIsLimitedByDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsLimitedByDateChanged();
    }

    private void OnIsLimitedByDateChanged()
    {
      if (IsLimitedByDate)
      {
        IsInfinite = false;
        IsLimitedByCount = false;
      }
    }

    #endregion // IsLimitedByDate Property

    #endregion // End type properties

    #region Pattern selection properties

    #region IsDailyPattern Property

    /// <summary>
    /// Gets or sets whether or not to create a daily pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsDailyPatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsDailyPattern
    {
      get { return (bool)GetValue(IsDailyPatternProperty); }
      set { SetValue(IsDailyPatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDailyPattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDailyPatternProperty =
      DependencyProperty.Register("IsDailyPattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsDailyPatternChanged));

    private static void OnIsDailyPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsDailyPatternChanged();
    }

    private void OnIsDailyPatternChanged()
    {
      if (IsDailyPattern)
      {
        IsWeeklyPattern = false;
        IsMonthlyPattern = false;
        IsYearlyPattern = false;
      }
    }

    #endregion // IsDailyPattern Property

    #region IsWeeklyPattern Property

    /// <summary>
    /// Gets or sets whether or not to create a weekly pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsWeeklyPatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsWeeklyPattern
    {
      get { return (bool)GetValue(IsWeeklyPatternProperty); }
      set { SetValue(IsWeeklyPatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsWeeklyPattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsWeeklyPatternProperty =
      DependencyProperty.Register("IsWeeklyPattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnIsWeeklyPatternChanged));

    private static void OnIsWeeklyPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsWeeklyPatternChanged();
    }

    private void OnIsWeeklyPatternChanged()
    {
      if (IsWeeklyPattern)
      {
        IsDailyPattern = false;
        IsMonthlyPattern = false;
        IsYearlyPattern = false;
      }
    }

    #endregion // IsWeeklyPattern Property

    #region IsMonthlyPattern Property

    /// <summary>
    /// Gets or sets whether or not to create a monthly pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMonthlyPatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMonthlyPattern
    {
      get { return (bool)GetValue(IsMonthlyPatternProperty); }
      set { SetValue(IsMonthlyPatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMonthlyPattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMonthlyPatternProperty =
      DependencyProperty.Register("IsMonthlyPattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsMonthlyPatternChanged));

    private static void OnIsMonthlyPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsMonthlyPatternChanged();
    }

    private void OnIsMonthlyPatternChanged()
    {
      if (IsMonthlyPattern)
      {
        IsDailyPattern = false;
        IsWeeklyPattern = false;
        IsYearlyPattern = false;
      }
    }

    #endregion // IsMonthlyPattern Property

    #region IsYearlyPattern Property

    /// <summary>
    /// Gets or sets whether or not to create a yearly pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsYearlyPatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsYearlyPattern
    {
      get { return (bool)GetValue(IsYearlyPatternProperty); }
      set { SetValue(IsYearlyPatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsYearlyPattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsYearlyPatternProperty =
      DependencyProperty.Register("IsYearlyPattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsYearlyPatternChanged));

    private static void OnIsYearlyPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsYearlyPatternChanged();
    }

    private void OnIsYearlyPatternChanged()
    {
      if (IsYearlyPattern)
      {
        IsDailyPattern = false;
        IsWeeklyPattern = false;
        IsMonthlyPattern = false;
      }
    }

    #endregion // IsYearlyPattern Property

    #endregion // Pattern selection properties

    #region Daily pattern properties

    #region IsEveryNDays Property

    /// <summary>
    /// Gets or sets whether or not to use the every-Nth-day pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEveryNDaysProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEveryNDays
    {
      get { return (bool)GetValue(IsEveryNDaysProperty); }
      set { SetValue(IsEveryNDaysProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsEveryNDays"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEveryNDaysProperty =
      DependencyProperty.Register("IsEveryNDays", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnIsEveryNDaysChanged));

    private static void OnIsEveryNDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsEveryNDaysChanged();
    }

    private void OnIsEveryNDaysChanged()
    {
      if (IsEveryNDays)
      {
        IsEveryWeekday = false;
      }
    }

    #endregion // IsEveryNDays Property

    #region IsEveryWeekday Property

    /// <summary>
    /// Gets or sets whether or not to use the every-weekday pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEveryWeekdayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEveryWeekday
    {
      get { return (bool)GetValue(IsEveryWeekdayProperty); }
      set { SetValue(IsEveryWeekdayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsEveryWeekday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEveryWeekdayProperty =
      DependencyProperty.Register("IsEveryWeekday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsEveryWeekdayChanged));

    private static void OnIsEveryWeekdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsEveryWeekdayChanged();
    }

    private void OnIsEveryWeekdayChanged()
    {
      if (IsEveryWeekday)
      {
        IsEveryNDays = false;
      }
    }

    #endregion // IsEveryWeekday Property

    #region DailyInterval Property

    /// <summary>
    /// Gets or sets the daily interval.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DailyIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int DailyInterval
    {
      get { return (int)GetValue(DailyIntervalProperty); }
      set { SetValue(DailyIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DailyInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty DailyIntervalProperty =
      DependencyProperty.Register("DailyInterval", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnDailyIntervalChanged));

    private static void OnDailyIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnDailyIntervalChanged();
    }

    private void OnDailyIntervalChanged()
    {
      IsEveryNDays = true;
    }

    #endregion // DailyInterval Property

    #endregion // Daily pattern properties

    #region Weekly pattern properties

    #region WeeklyInterval Property

    /// <summary>
    /// Gets or sets the weekly interval.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WeeklyIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int WeeklyInterval
    {
      get { return (int)GetValue(WeeklyIntervalProperty); }
      set { SetValue(WeeklyIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WeeklyInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty WeeklyIntervalProperty =
      DependencyProperty.Register("WeeklyInterval", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnWeeklyIntervalChanged));

    private static void OnWeeklyIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnWeeklyIntervalChanged();
    }

    private void OnWeeklyIntervalChanged()
    {
    }

    #endregion // WeeklyInterval Property

    #region IsOnMonday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Monday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnMondayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnMonday
    {
      get { return (bool)GetValue(IsOnMondayProperty); }
      set { SetValue(IsOnMondayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnMonday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnMondayProperty =
      DependencyProperty.Register("IsOnMonday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnMondayChanged));

    private static void OnIsOnMondayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnMondayChanged();
    }

    private void OnIsOnMondayChanged()
    {
    }

    #endregion // IsOnMonday Property

    #region IsOnTuesday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Tuesday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnTuesdayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnTuesday
    {
      get { return (bool)GetValue(IsOnTuesdayProperty); }
      set { SetValue(IsOnTuesdayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnTuesday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnTuesdayProperty =
      DependencyProperty.Register("IsOnTuesday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnTuesdayChanged));

    private static void OnIsOnTuesdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnTuesdayChanged();
    }

    private void OnIsOnTuesdayChanged()
    {
    }

    #endregion // IsOnTuesday Property

    #region IsOnWednesday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Wednesday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnWednesdayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnWednesday
    {
      get { return (bool)GetValue(IsOnWednesdayProperty); }
      set { SetValue(IsOnWednesdayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnWednesday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnWednesdayProperty =
      DependencyProperty.Register("IsOnWednesday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnWednesdayChanged));

    private static void OnIsOnWednesdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnWednesdayChanged();
    }

    private void OnIsOnWednesdayChanged()
    {
    }

    #endregion // IsOnWednesday Property

    #region IsOnThursday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Thursday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnThursdayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnThursday
    {
      get { return (bool)GetValue(IsOnThursdayProperty); }
      set { SetValue(IsOnThursdayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnThursday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnThursdayProperty =
      DependencyProperty.Register("IsOnThursday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnThursdayChanged));

    private static void OnIsOnThursdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnThursdayChanged();
    }

    private void OnIsOnThursdayChanged()
    {
    }

    #endregion // IsOnThursday Property

    #region IsOnFriday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Friday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnFridayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnFriday
    {
      get { return (bool)GetValue(IsOnFridayProperty); }
      set { SetValue(IsOnFridayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnFriday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnFridayProperty =
      DependencyProperty.Register("IsOnFriday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnFridayChanged));

    private static void OnIsOnFridayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnFridayChanged();
    }

    private void OnIsOnFridayChanged()
    {
    }

    #endregion // IsOnFriday Property

    #region IsOnSaturday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Saturday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnSaturdayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnSaturday
    {
      get { return (bool)GetValue(IsOnSaturdayProperty); }
      set { SetValue(IsOnSaturdayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnSaturday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnSaturdayProperty =
      DependencyProperty.Register("IsOnSaturday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnSaturdayChanged));

    private static void OnIsOnSaturdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnSaturdayChanged();
    }

    private void OnIsOnSaturdayChanged()
    {
    }

    #endregion // IsOnSaturday Property

    #region IsOnSunday Property

    /// <summary>
    /// Gets or sets whether or not the weekly pattern occurs on Sunday.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsOnSundayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsOnSunday
    {
      get { return (bool)GetValue(IsOnSundayProperty); }
      set { SetValue(IsOnSundayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsOnSunday"/> property.
    /// </summary>
    public static readonly DependencyProperty IsOnSundayProperty =
      DependencyProperty.Register("IsOnSunday", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsOnSundayChanged));

    private static void OnIsOnSundayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsOnSundayChanged();
    }

    private void OnIsOnSundayChanged()
    {
    }

    #endregion // IsOnSunday Property

    #endregion // Weekly pattern properties

    #region Monthly pattern properties

    #region IsNthDayOfMonth Property

    /// <summary>
    /// Gets or sets whether or not to use the Nth-day-of-month pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsNthDayOfMonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsNthDayOfMonth
    {
      get { return (bool)GetValue(IsNthDayOfMonthProperty); }
      set { SetValue(IsNthDayOfMonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsNthDayOfMonth"/> property.
    /// </summary>
    public static readonly DependencyProperty IsNthDayOfMonthProperty =
      DependencyProperty.Register("IsNthDayOfMonth", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnIsNthDayOfMonthChanged));

    private static void OnIsNthDayOfMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsNthDayOfMonthChanged();
    }

    private void OnIsNthDayOfMonthChanged()
    {
      if (IsNthDayOfMonth)
      {
        IsMonthlyRecurrencePattern = false;
      }
    }

    #endregion // IsNthDayOfMonth Property

    #region IsMonthlyRecurrencePattern Property

    /// <summary>
    /// Gets or sets whether or not to use the monthly-pattern-recurrence-pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMonthlyRecurrencePatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMonthlyRecurrencePattern
    {
      get { return (bool)GetValue(IsMonthlyRecurrencePatternProperty); }
      set { SetValue(IsMonthlyRecurrencePatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMonthlyRecurrencePattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMonthlyRecurrencePatternProperty =
      DependencyProperty.Register("IsMonthlyRecurrencePattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsMonthlyRecurrencePatternChanged));

    private static void OnIsMonthlyRecurrencePatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsMonthlyRecurrencePatternChanged();
    }

    private void OnIsMonthlyRecurrencePatternChanged()
    {
      if (IsMonthlyRecurrencePattern)
      {
        IsNthDayOfMonth = false;
      }
    }

    #endregion // IsMonthlyRecurrencePattern Property

    #region DayOfTheMonth Property

    /// <summary>
    /// Gets or sets the specific day of the month.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DayOfTheMonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int DayOfTheMonth
    {
      get { return (int)GetValue(DayOfTheMonthProperty); }
      set { SetValue(DayOfTheMonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DayOfTheMonth"/> property.
    /// </summary>
    public static readonly DependencyProperty DayOfTheMonthProperty =
      DependencyProperty.Register("DayOfTheMonth", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnDayOfTheMonthChanged));

    private static void OnDayOfTheMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnDayOfTheMonthChanged();
    }

    private void OnDayOfTheMonthChanged()
    {
      IsNthDayOfMonth = true;
    }

    #endregion // DayOfTheMonth Property

    #region MonthlyOcurrence Property

    /// <summary>
    /// Gets or sets the monthly <see cref="Occurrence"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthlyOcurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Occurrence MonthlyOcurrence
    {
      get { return (Occurrence)GetValue(MonthlyOcurrenceProperty); }
      set { SetValue(MonthlyOcurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthlyOcurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthlyOcurrenceProperty =
      DependencyProperty.Register("MonthlyOcurrence", typeof(Occurrence), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnMonthlyOcurrenceChanged));

    private static void OnMonthlyOcurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthlyOcurrenceChanged();
    }

    private void OnMonthlyOcurrenceChanged()
    {
      IsMonthlyRecurrencePattern = true;
    }

    #endregion // MonthlyOcurrence Property

    #region MonthlyDayOfRecurrence Property

    /// <summary>
    /// Gets or sets the monthly <see cref="DayOfRecurrence"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthlyDayOfRecurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DayOfRecurrence MonthlyDayOfRecurrence
    {
      get { return (DayOfRecurrence)GetValue(MonthlyDayOfRecurrenceProperty); }
      set { SetValue(MonthlyDayOfRecurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthlyDayOfRecurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthlyDayOfRecurrenceProperty =
      DependencyProperty.Register("MonthlyDayOfRecurrence", typeof(DayOfRecurrence), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnMonthlyDayOfRecurrenceChanged));

    private static void OnMonthlyDayOfRecurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthlyDayOfRecurrenceChanged();
    }

    private void OnMonthlyDayOfRecurrenceChanged()
    {
      IsMonthlyRecurrencePattern = true;
    }

    #endregion // MonthlyDayOfRecurrence Property

    #region MonthlyInterval Property

    /// <summary>
    /// Gets or sets the monthly interval.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthlyIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MonthlyInterval
    {
      get { return (int)GetValue(MonthlyIntervalProperty); }
      set { SetValue(MonthlyIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthlyInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthlyIntervalProperty =
      DependencyProperty.Register("MonthlyInterval", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnMonthlyIntervalChanged));

    private static void OnMonthlyIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthlyIntervalChanged();
    }

    private void OnMonthlyIntervalChanged()
    {
      IsNthDayOfMonth = true;
    }

    #endregion // MonthlyInterval Property

    #region MonthlyRecurrenceInterval Property

    /// <summary>
    /// Gets or sets the MonthlyRecurrenceInterval.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthlyRecurrenceIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MonthlyRecurrenceInterval
    {
      get { return (int)GetValue(MonthlyRecurrenceIntervalProperty); }
      set { SetValue(MonthlyRecurrenceIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthlyRecurrenceInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthlyRecurrenceIntervalProperty =
      DependencyProperty.Register("MonthlyRecurrenceInterval", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnMonthlyRecurrenceIntervalChanged));

    private static void OnMonthlyRecurrenceIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthlyRecurrenceIntervalChanged();
    }

    private void OnMonthlyRecurrenceIntervalChanged()
    {
      IsMonthlyRecurrencePattern = true;
    }

    #endregion // MonthlyRecurrenceInterval Property

    #endregion // Monthly pattern properties

    #region Yearly pattern properties

    #region IsSpecificDateOfYear Property

    /// <summary>
    /// Gets or sets whether or not to use the specific-date-yearly-recurrence.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSpecificDateOfYearProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSpecificDateOfYear
    {
      get { return (bool)GetValue(IsSpecificDateOfYearProperty); }
      set { SetValue(IsSpecificDateOfYearProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSpecificDateOfYear"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSpecificDateOfYearProperty =
      DependencyProperty.Register("IsSpecificDateOfYear", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnIsSpecificDateOfYearChanged));

    private static void OnIsSpecificDateOfYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsSpecificDateOfYearChanged();
    }

    private void OnIsSpecificDateOfYearChanged()
    {
      if (IsSpecificDateOfYear)
      {
        IsYearlyRecurrencePattern = false;
      }
    }

    #endregion // IsSpecificDateOfYear Property

    #region IsYearlyRecurrencePattern Property

    /// <summary>
    /// Gets or sets whether or not to use the yearly-pattern-recurrence-pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsYearlyRecurrencePatternProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsYearlyRecurrencePattern
    {
      get { return (bool)GetValue(IsYearlyRecurrencePatternProperty); }
      set { SetValue(IsYearlyRecurrencePatternProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsYearlyRecurrencePattern"/> property.
    /// </summary>
    public static readonly DependencyProperty IsYearlyRecurrencePatternProperty =
      DependencyProperty.Register("IsYearlyRecurrencePattern", typeof(bool), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnIsYearlyRecurrencePatternChanged));

    private static void OnIsYearlyRecurrencePatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnIsYearlyRecurrencePatternChanged();
    }

    private void OnIsYearlyRecurrencePatternChanged()
    {
      if (IsYearlyRecurrencePattern)
      {
        IsSpecificDateOfYear = false;
      }
    }

    #endregion // IsYearlyRecurrencePattern Property

    #region YearlyInterval Property

    /// <summary>
    /// Gets or sets the yearly interval.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="YearlyIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int YearlyInterval
    {
      get { return (int)GetValue(YearlyIntervalProperty); }
      set { SetValue(YearlyIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YearlyInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty YearlyIntervalProperty =
      DependencyProperty.Register("YearlyInterval", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnYearlyIntervalChanged));

    private static void OnYearlyIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnYearlyIntervalChanged();
    }

    private void OnYearlyIntervalChanged()
    {
    }

    #endregion // YearlyInterval Property

    #region Month Property

    /// <summary>
    /// Gets or sets the month to be used by a specific-date-yearly-recurrence-pattern.
    /// The month is an integer from 0 to 11.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Month
    {
      get { return (int)GetValue(MonthProperty); }
      set { SetValue(MonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Month"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthProperty =
      DependencyProperty.Register("Month", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnMonthChanged));

    private static void OnMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthChanged();
    }

    private void OnMonthChanged()
    {
      IsSpecificDateOfYear = true;
    }

    #endregion // Month Property

    #region YearlyDayOfTheMonth Property

    /// <summary>
    /// Gets or sets the specific day of the month.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="YearlyDayOfTheMonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int YearlyDayOfTheMonth
    {
      get { return (int)GetValue(YearlyDayOfTheMonthProperty); }
      set { SetValue(YearlyDayOfTheMonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YearlyDayOfTheMonth"/> property.
    /// </summary>
    public static readonly DependencyProperty YearlyDayOfTheMonthProperty =
      DependencyProperty.Register("YearlyDayOfTheMonth", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnYearlyDayOfTheMonthChanged));

    private static void OnYearlyDayOfTheMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnYearlyDayOfTheMonthChanged();
    }

    private void OnYearlyDayOfTheMonthChanged()
    {
      IsSpecificDateOfYear = true;
    }

    #endregion // YearlyDayOfTheMonth Property

    #region YearlyOccurrence Property

    /// <summary>
    /// Gets or sets the yearly <see cref="Occurrence"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="YearlyOccurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Occurrence YearlyOccurrence
    {
      get { return (Occurrence)GetValue(YearlyOccurrenceProperty); }
      set { SetValue(YearlyOccurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YearlyOccurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty YearlyOccurrenceProperty =
      DependencyProperty.Register("YearlyOccurrence", typeof(Occurrence), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnYearlyOccurrenceChanged));

    private static void OnYearlyOccurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnYearlyOccurrenceChanged();
    }

    private void OnYearlyOccurrenceChanged()
    {
      IsYearlyRecurrencePattern = true;
    }

    #endregion // YearlyOccurrence Property

    #region YearlyDayOfRecurrence Property

    /// <summary>
    /// Gets or sets the yearly <see cref="DayOfRecurrence"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="YearlyDayOfRecurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DayOfRecurrence YearlyDayOfRecurrence
    {
      get { return (DayOfRecurrence)GetValue(YearlyDayOfRecurrenceProperty); }
      set { SetValue(YearlyDayOfRecurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YearlyDayOfRecurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty YearlyDayOfRecurrenceProperty =
      DependencyProperty.Register("YearlyDayOfRecurrence", typeof(DayOfRecurrence), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(OnYearlyDayOfRecurrenceChanged));

    private static void OnYearlyDayOfRecurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnYearlyDayOfRecurrenceChanged();
    }

    private void OnYearlyDayOfRecurrenceChanged()
    {
      IsYearlyRecurrencePattern = true;
    }

    #endregion // YearlyDayOfRecurrence Property

    #region MonthOfRecurrence Property

    /// <summary>
    /// Gets or sets the month to be used by a yearly-pattern-recurrence-pattern.
    /// The month is an integer from 0 to 11.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthOfRecurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MonthOfRecurrence
    {
      get { return (int)GetValue(MonthOfRecurrenceProperty); }
      set { SetValue(MonthOfRecurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthOfRecurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthOfRecurrenceProperty =
      DependencyProperty.Register("MonthOfRecurrence", typeof(int), typeof(RecurrenceDialog),
      new FrameworkPropertyMetadata(1, OnMonthOfRecurrenceChanged));

    private static void OnMonthOfRecurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RecurrenceDialog)d).OnMonthOfRecurrenceChanged();
    }

    private void OnMonthOfRecurrenceChanged()
    {
      IsYearlyRecurrencePattern = true;
    }

    #endregion // MonthOfRecurrence Property

    #endregion // Yearly pattern properties

    #region List creation methods

    private static IList<TimeOfDay> BuildTimeList(int startHour, int startMinute)
    {
      List<TimeOfDay> times = new List<TimeOfDay>();
      int hour = startHour;
      int min = startMinute;
      for (int i = 0; i < 48; i++)
      {
        times.Add(new TimeOfDay(hour, min));
        min += 30;
        if (min >= 60)
        {
          min -= 60;
          hour++;
          if (hour >= 24)
          {
            hour -= 24;
          }
        }
      }

      return times.AsReadOnly();
    }

    private static IList<TimeSpan> BuildDurationList()
    {
      List<TimeSpan> timeSpans = new List<TimeSpan>();
      timeSpans.Add(new TimeSpan(0, 0, 0));
      timeSpans.Add(new TimeSpan(0, 5, 0));
      timeSpans.Add(new TimeSpan(0, 10, 0));
      timeSpans.Add(new TimeSpan(0, 15, 0));
      timeSpans.Add(new TimeSpan(0, 30, 0));
      for (int i = 1; i < 13; i++)
      {
        timeSpans.Add(new TimeSpan(i, 0, 0));
      }
      timeSpans.Add(new TimeSpan(18, 0, 0));
      for (int i = 1; i < 5; i++)
      {
        timeSpans.Add(new TimeSpan(i, 0, 0, 0));
      }
      timeSpans.Add(new TimeSpan(7, 0, 0, 0));
      timeSpans.Add(new TimeSpan(14, 0, 0, 0));
      return timeSpans.AsReadOnly();
    }

    private static IList<Occurrence> BuildOccurrenceList()
    {
      List<Occurrence> occurrences = new List<Occurrence>();
      occurrences.Add(Occurrence.First);
      occurrences.Add(Occurrence.Second);
      occurrences.Add(Occurrence.Third);
      occurrences.Add(Occurrence.Fourth);
      occurrences.Add(Occurrence.Last);
      return occurrences.AsReadOnly();
    }

    private static IList<DayOfRecurrence> BuildDayOfRecurrenceList()
    {
      List<DayOfRecurrence> list = new List<DayOfRecurrence>();
      list.Add(DayOfRecurrence.Day);
      list.Add(DayOfRecurrence.Weekday);
      list.Add(DayOfRecurrence.WeekendDay);
      list.Add(DayOfRecurrence.Monday);
      list.Add(DayOfRecurrence.Tuesday);
      list.Add(DayOfRecurrence.Wednesday);
      list.Add(DayOfRecurrence.Thursday);
      list.Add(DayOfRecurrence.Friday);
      list.Add(DayOfRecurrence.Saturday);
      list.Add(DayOfRecurrence.Sunday);
      return list.AsReadOnly();
    }

    private static IList<int> BuildMonthList()
    {
      List<int> months = new List<int>();
      for (int i = 0; i < 12; i++)
      {
        months.Add(i);
      }
      return months.AsReadOnly();
    }

    #endregion // List creation methods

    /// <summary>
    /// Gets or sets the recurrence information for the <see cref="ScheduleItem"/>.
    /// </summary>
    public RecurrenceInfo RecurrenceInfo { get; private set; }

    #region Reset methods

    private void ResetWeeklyInfo()
    {
      DayOfWeek dayOfWeek = _item.StartTime.DayOfWeek;
      SelectWeek(dayOfWeek);
    }

    private void SelectWeek(DayOfWeek dayOfWeek)
    {
      switch(dayOfWeek)
      {
        case DayOfWeek.Monday:
          IsOnMonday = true;
          break;
        case DayOfWeek.Tuesday:
          IsOnTuesday = true;
          break;
        case DayOfWeek.Wednesday:
          IsOnWednesday = true;
          break;
        case DayOfWeek.Thursday:
          IsOnThursday = true;
          break;
        case DayOfWeek.Friday:
          IsOnFriday = true;
          break;
        case DayOfWeek.Saturday:
          IsOnSaturday = true;
          break;
        case DayOfWeek.Sunday:
          IsOnSunday = true;
          break;
        default:
          throw new InvalidOperationException("Failed to find the property for " + dayOfWeek);
      }
    }

    private void ResetMonthlyInfo()
    {
      DateTime start = _item.StartTime;
      DayOfTheMonth = start.Day;

      MonthlyDayOfRecurrence = ConvertDayOfWeek(start.DayOfWeek);
      int weekCount = (int)Math.Ceiling(start.Day / 7f);
      MonthlyOcurrence = ConvertToOccurrence(weekCount);
      IsNthDayOfMonth = true;
    }

    private void ResetYearlyInfo()
    {
      DateTime start = _item.StartTime;
      Month = start.Month;
      YearlyDayOfTheMonth = start.Day;

      YearlyDayOfRecurrence = ConvertDayOfWeek(start.DayOfWeek);
      int weekCount = (int)Math.Ceiling(start.Day / 7f);
      YearlyOccurrence = ConvertToOccurrence(weekCount);
      MonthOfRecurrence = start.Month;
      IsSpecificDateOfYear = true;
    }

    private Occurrence ConvertToOccurrence(int number)
    {
      switch(number)
      {
        case 1:
          return Occurrence.First;
        case 2:
          return Occurrence.Second;
        case 3:
          return Occurrence.Third;
        case 4:
          return Occurrence.Fourth;
        default:
          return Occurrence.Last;
      }
    }

    private DayOfRecurrence ConvertDayOfWeek(DayOfWeek dayOfWeek)
    {
      switch(dayOfWeek)
      {
        case DayOfWeek.Monday:
          return DayOfRecurrence.Monday;
        case DayOfWeek.Tuesday:
          return DayOfRecurrence.Tuesday;
        case DayOfWeek.Wednesday:
          return DayOfRecurrence.Wednesday;
        case DayOfWeek.Thursday:
          return DayOfRecurrence.Thursday;
        case DayOfWeek.Friday:
          return DayOfRecurrence.Friday;
        case DayOfWeek.Saturday:
          return DayOfRecurrence.Saturday;
        case DayOfWeek.Sunday:
          return DayOfRecurrence.Sunday;
        default:
          throw new InvalidOperationException("Failed to convert " + dayOfWeek + " to a DayOfRecurrence");
      }
    }

    #endregion // Reset methods

    #region Pattern construction methods

    private RecurrenceInfo BuildRecurrenceInfo()
    {
      IRecurrencePattern pattern = null;
      if (IsDailyPattern == true)
      {
        pattern = BuildDailyPattern();
      }
      else if (IsWeeklyPattern == true)
      {
        pattern = BuildWeeklyPattern();
      }
      else if (IsMonthlyPattern == true)
      {
        pattern = BuildMonthlyPattern();
      }
      else if (IsYearlyPattern == true)
      {
        pattern = BuildYearlyPattern();
      }

      DateTime startTime = StartDate;
      TimeSpan duration = Duration;

      if (IsInfinite == true)
      {
        return RecurrenceInfo.Forever(startTime, duration, pattern, _firstDayOfWeek);
      }
      if (IsLimitedByCount == true)
      {
        int maxOccurrences = MaxOccurrences;
        return RecurrenceInfo.ToMaxOccurrences(startTime, duration, maxOccurrences, pattern, _firstDayOfWeek);
      }
      if (IsLimitedByDate == true)
      {
        DateTime endDate = EndDate;
        return RecurrenceInfo.ToEndDate(startTime, duration, endDate, pattern, _firstDayOfWeek);
      }
      return null;
    }

    private IRecurrencePattern BuildDailyPattern()
    {
      if (IsEveryNDays)
      {
        int dailyInterval = DailyInterval;
        return new EveryNDaysRecurrencePattern(dailyInterval);
      }
      return new EveryWeekdayRecurrencePattern();
    }

    private IRecurrencePattern BuildWeeklyPattern()
    {
      int weeklyInterval = WeeklyInterval;
      IList<DayOfWeek> days = new List<DayOfWeek>();
      if (IsOnMonday)
      {
        days.Add(DayOfWeek.Monday);
      }
      if (IsOnTuesday)
      {
        days.Add(DayOfWeek.Tuesday);
      }
      if (IsOnWednesday)
      {
        days.Add(DayOfWeek.Wednesday);
      }
      if (IsOnThursday)
      {
        days.Add(DayOfWeek.Thursday);
      }
      if (IsOnFriday)
      {
        days.Add(DayOfWeek.Friday);
      }
      if (IsOnSaturday)
      {
        days.Add(DayOfWeek.Saturday);
      }
      if (IsOnSunday)
      {
        days.Add(DayOfWeek.Sunday);
      }
      if (days.Count == 0)
      {
        days.Add(_item.StartTime.DayOfWeek);
      }
      DayOfWeek[] daysOfWeek = new DayOfWeek[days.Count];
      int index = 0;
      foreach (DayOfWeek day in days)
      {
        daysOfWeek[index] = day;
        index++;
      }
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(weeklyInterval, daysOfWeek);
      return pattern;
    }

    private IRecurrencePattern BuildMonthlyPattern()
    {
      int monthlyInterval = 0;
      if (IsNthDayOfMonth)
      {
        int dayOfMonth = DayOfTheMonth;
        monthlyInterval = MonthlyInterval;
        return new NthDayOfMonthRecurrencePattern(monthlyInterval, dayOfMonth);
      }
      Occurrence occurrence = MonthlyOcurrence;
      DayOfRecurrence dayOfRecurrence = MonthlyDayOfRecurrence;
      monthlyInterval = MonthlyRecurrenceInterval;
      return new MonthlyPatternRecurrencePattern(monthlyInterval, occurrence, dayOfRecurrence);
    }

    private IRecurrencePattern BuildYearlyPattern()
    {
      int month = 0;
      int yearlyInterval = YearlyInterval;
      if (IsSpecificDateOfYear)
      {
        month = Month;
        int day = YearlyDayOfTheMonth;
        return new SpecificDateYearlyRecurrencePattern(yearlyInterval, month, day);
      }
      Occurrence occurrence = YearlyOccurrence;
      DayOfRecurrence dayOfRecurrence = YearlyDayOfRecurrence;
      month = MonthOfRecurrence;
      return new YearlyPatternRecurrencePattern(yearlyInterval, month, occurrence, dayOfRecurrence);
    }

    #endregion // Pattern construction methods

#if !SILVERLIGHT
    private bool _dialogResult = false;

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    public bool DialogResult
    {
      get { return _dialogResult; }
      private set
      {
        _dialogResult = value;
        Window wnd = Parent as Window;// VisualTreeUtils.FindAncestor<Window>(this);
        if (wnd != null)
        {
          wnd.DialogResult = DialogResult;
        }
        OnClosed();
      }
    }

    /// <summary>
    /// Raised when this <see cref="RecurrenceDialog"/> is closed.
    /// </summary>
    public event RoutedEventHandler Closed;

    internal void OnClosed()
    {
      RoutedEventHandler handler = Closed;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
    }
#endif
  }
}

