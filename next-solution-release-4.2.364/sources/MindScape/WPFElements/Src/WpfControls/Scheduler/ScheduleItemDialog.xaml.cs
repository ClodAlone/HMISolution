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
using System.Globalization;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides a dialog-based interface for editing a <see cref="ScheduleItem"/>.
  /// </summary>
  public partial class ScheduleItemDialog :
#if SILVERLIGHT
    ChildWindow
#else
    UserControl
#endif
  {
    private ScheduleItem _item;
    private RecurrenceInfo _info;
    private bool _ignoreOverlaps;
    private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;

    //private TimePicker _startTimeEditor;
    //private TimePicker _endTimeEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleItemDialog"/> class.
    /// </summary>
    /// <param name="item">The item to be edited.</param>
    public ScheduleItemDialog(ScheduleItem item)
      : this(item, DayOfWeek.Monday)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleItemDialog"/> class.
    /// </summary>
    /// <param name="item">The item to be edited.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    public ScheduleItemDialog(ScheduleItem item, DayOfWeek firstDayOfWeek)
    {
      InitializeComponent();

      BindCommands();

      SetValue(StartTimeListPropertyKey, CreateTimeList(0, 0));

      _firstDayOfWeek = firstDayOfWeek;
      _item = item;
      Subject = item.Name;
      _ignoreOverlaps = true;
      StartTime = new TimeOfDay(item.StartTime.Hour, item.StartTime.Minute);
      StartDate = item.StartTime;
      
      SetValue(EndTimeListPropertyKey, CreateTimeList(StartTime.Hour, StartTime.Minute));
      EndTime = new TimeOfDay(item.EndTime.Hour, item.EndTime.Minute);
      
      EndDate = item.EndTime;
      _ignoreOverlaps = false;

      Loaded += new RoutedEventHandler(ScheduleItemDialog_Loaded);

      DataContext = this;
    }

    private void ScheduleItemDialog_Loaded(object sender, RoutedEventArgs e)
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
      CommandBindings.Add(new CommandBinding(SchedulerCommands.EditRecurrenceCommand, EditRecurrence_Executed, EditRecurrence_CanExecute));
    }

    private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      _item.Name = Subject;
      _item.StartTime = CreateDateTime(StartDate, StartTime);
      _item.EndTime = CreateDateTime(EndDate, EndTime);
      if (_info != null)
      {
        _item.RecurrenceInfo = _info;
      }
      this.DialogResult = true;
    }

    private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    private void EditRecurrence_Executed(object sender, ExecutedRoutedEventArgs e)
    {
#if SILVERLIGHT
      RecurrenceDialog dlg = new RecurrenceDialog(_item, _info);
      dlg.Closed += new EventHandler(RecurrenceDialog_Closed);
      dlg.Show();
#else
      RecurrenceDialog dlg = new RecurrenceDialog(_item, _info, _firstDayOfWeek);
      dlg.Closed += new RoutedEventHandler(RecurrenceDialog_Closed);
      Scheduler scheduler = Scheduler as Scheduler;
      if (scheduler != null && scheduler.RecurrenceDialogStyle != null)
      {
        dlg.Style = scheduler.RecurrenceDialogStyle;
      }
      else if (Scheduler is DayScheduleBase)
      {
        DayScheduleBase dayScheduleBase = Scheduler as DayScheduleBase;
        if (dayScheduleBase != null && dayScheduleBase.RecurrenceDialogStyle != null)
        {
          dlg.Style = dayScheduleBase.RecurrenceDialogStyle;
        }
      }
      DialogHelper.ShowRecurrenceDialog(dlg, Scheduler, scheduler == null ? null : scheduler.Formatter);
#endif
    }

    private void EditRecurrence_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (_item.IsInstanceOfRecurringItem)
      {
        e.CanExecute = false;
      }
      else
      {
        e.CanExecute = true;
      }
    }

    private IList<TimeOfDay> CreateTimeList(int startHour, int startMinute)
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

    private DateTime CreateDateTime(DateTime date, TimeOfDay time)
    {
      DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
      return dateTime;
    }

    /// <summary>
    /// Gets the <see cref="ScheduleItem" /> being edited.
    /// </summary>
    public ScheduleItem ScheduleItem
    {
      get { return _item; }
    }

    /// <summary>
    /// Gets or sets the Subject.
    /// This is a dependency property.
    /// </summary>
    public string Subject
    {
      get { return (string)GetValue(SubjectProperty); }
      set { SetValue(SubjectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Subject"/> property.
    /// </summary>
    public static readonly DependencyProperty SubjectProperty =
      DependencyProperty.Register("Subject", typeof(string), typeof(ScheduleItemDialog),
      new PropertyMetadata(""));

    private void CheckOverlapOnStartChange()
    {
      if (!_ignoreOverlaps)
      {
        DateTime start = CreateDateTime(StartDate, StartTime);
        DateTime end = CreateDateTime(EndDate, EndTime);
        if (start > end)
        {
          end = start.AddMinutes(30);
          EndDate = end;
          EndTime = new TimeOfDay(end.Hour, end.Minute);
        }
      }
    }

    private void CheckOverlapOnEndChange()
    {
      if (!_ignoreOverlaps)
      {
        DateTime start = CreateDateTime(StartDate, StartTime);
        DateTime end = CreateDateTime(EndDate, EndTime);
        if (start > end && end.Year > 1)
        {
          start = end.AddMinutes(-30);
          StartDate = start;
          StartTime = new TimeOfDay(start.Hour, start.Minute);
        }
      }
    }

    #region StartTime property

    /// <summary>
    /// Gets or sets the StartTime.
    /// This is a dependency property.
    /// </summary>
    public TimeOfDay StartTime
    {
      get { return (TimeOfDay)GetValue(StartTimeProperty); }
      set { SetValue(StartTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartTime"/> property.
    /// </summary>
    public static readonly DependencyProperty StartTimeProperty =
      DependencyProperty.Register("StartTime", typeof(TimeOfDay), typeof(ScheduleItemDialog),
      new PropertyMetadata(new TimeOfDay(), new PropertyChangedCallback(OnStartTimeChanged)));

    private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ScheduleItemDialog)d).OnStartTimeChanged();
    }

    private void OnStartTimeChanged()
    {
      /*TimeOfDay startTime = (TimeOfDay)StartTimeEditor.SelectedTime;
      TimeOfDay selectedEndTime = (TimeOfDay)EndTimeEditor.SelectedTime;
      EndTimeEditor.ItemsSource = CreateTimeList(startTime.Hour, startTime.Minute);
      EndTime = selectedEndTime;*/

      CheckOverlapOnStartChange();
    }

    #endregion // StartTime property

    #region EndTime property

    /// <summary>
    /// Gets or sets the EndTime.
    /// This is a dependency property.
    /// </summary>
    public TimeOfDay EndTime
    {
      get { return (TimeOfDay)GetValue(EndTimeProperty); }
      set { SetValue(EndTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndTime"/> property.
    /// </summary>
    public static readonly DependencyProperty EndTimeProperty =
      DependencyProperty.Register("EndTime", typeof(TimeOfDay), typeof(ScheduleItemDialog),
      new PropertyMetadata(new TimeOfDay(), new PropertyChangedCallback(OnEndTimeChanged)));

    private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ScheduleItemDialog)d).OnEndTimeChanged();
    }

    private void OnEndTimeChanged()
    {
      CheckOverlapOnEndChange();
    }

    #endregion // EndTime property

    #region StartDate property

    /// <summary>
    /// Gets or sets the StartDate.
    /// This is a dependency property.
    /// </summary>
    public DateTime StartDate
    {
      get { return (DateTime)GetValue(StartDateProperty); }
      set { SetValue(StartDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartDate"/> property.
    /// </summary>
    public static readonly DependencyProperty StartDateProperty =
      DependencyProperty.Register("StartDate", typeof(DateTime), typeof(ScheduleItemDialog),
      new PropertyMetadata(new PropertyChangedCallback(OnStartDateChanged)));

    private static void OnStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ScheduleItemDialog)d).OnStartDateChanged();
    }

    private void OnStartDateChanged()
    {
      CheckOverlapOnStartChange();
    }

    #endregion // StartDate property

    #region EndDate property

    /// <summary>
    /// Gets or sets the EndDate.
    /// This is a dependency property.
    /// </summary>
    public DateTime EndDate
    {
      get { return (DateTime)GetValue(EndDateProperty); }
      set { SetValue(EndDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndDate"/> property.
    /// </summary>
    public static readonly DependencyProperty EndDateProperty =
      DependencyProperty.Register("EndDate", typeof(DateTime), typeof(ScheduleItemDialog),
      new PropertyMetadata(new PropertyChangedCallback(OnEndDateChanged)));

    private static void OnEndDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ScheduleItemDialog)d).OnEndDateChanged();
    }

    private void OnEndDateChanged()
    {
      CheckOverlapOnEndChange();
    }

    #endregion // EndDate property

    #region HasRecurrence property

    /// <summary>
    /// Gets whether or not the schedule item currently has a recurrence pattern.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HasRecurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool HasRecurrence
    {
      get { return (bool)GetValue(HasRecurrenceProperty); }
    }

    private static readonly DependencyPropertyKey HasRecurrencePropertyKey =
        DependencyProperty.RegisterReadOnly("HasRecurrence", typeof(bool), typeof(ScheduleItemDialog), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="HasRecurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty HasRecurrenceProperty =
        HasRecurrencePropertyKey.DependencyProperty;

    #endregion // HasRecurrence property

    #region StartTimeList property

    /// <summary>
    /// Gets the list of times to be displayed by the start time picker.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartTimeListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<TimeOfDay> StartTimeList
    {
      get { return (ReadOnlyCollection<TimeOfDay>)GetValue(StartTimeListProperty); }
    }

    private static readonly DependencyPropertyKey StartTimeListPropertyKey =
        DependencyProperty.RegisterReadOnly("StartTimeList", typeof(ReadOnlyCollection<TimeOfDay>), typeof(ScheduleItemDialog), new UIPropertyMetadata(new List<TimeOfDay>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="StartTimeList"/> property.
    /// </summary>
    public static readonly DependencyProperty StartTimeListProperty =
        StartTimeListPropertyKey.DependencyProperty;

    #endregion // StartTimeList property

    #region EndTimeList property

    /// <summary>
    /// Gets the list of times to be displayed by the end time picker.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EndTimeListProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<TimeOfDay> EndTimeList
    {
      get { return (ReadOnlyCollection<TimeOfDay>)GetValue(EndTimeListProperty); }
    }

    private static readonly DependencyPropertyKey EndTimeListPropertyKey =
        DependencyProperty.RegisterReadOnly("EndTimeList", typeof(ReadOnlyCollection<TimeOfDay>), typeof(ScheduleItemDialog), new UIPropertyMetadata(new List<TimeOfDay>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="EndTimeList"/> property.
    /// </summary>
    public static readonly DependencyProperty EndTimeListProperty =
        EndTimeListPropertyKey.DependencyProperty;

    #endregion // EndTimeList property

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
      DependencyProperty.Register("Title", typeof(string), typeof(ScheduleItemDialog),
      new FrameworkPropertyMetadata("New Appointment", OnTitleChanged));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ScheduleItemDialog)d).OnTitleChanged();
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

    private void RecurrenceDialog_Closed(object sender, EventArgs e)
    {
      RecurrenceDialog dlg = sender as RecurrenceDialog;
      if (dlg != null)
      {
        _info = dlg.RecurrenceInfo;
        if (_info == null)
        {
          SetValue(HasRecurrencePropertyKey, false);
          //RecurrenceButton.Content = "Add Recurrence";
        }
        else
        {
          SetValue(HasRecurrencePropertyKey, true);
          //RecurrenceButton.Content = "Edit Recurrence";
        }
      }
    }

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
    /// Raised when this <see cref="ScheduleItemDialog"/> is closed.
    /// </summary>
    public event RoutedEventHandler Closed;

    internal UIElement Scheduler { get; set; }

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
