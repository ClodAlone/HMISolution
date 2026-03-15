using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for displaying a monthly calendar.
  /// </summary>
  [TemplatePart(Name = DaysListPartName, Type = typeof(ListBox))]
  [TemplatePart(Name = DaysOfWeekListPartName, Type = typeof(ItemsControl))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class MonthCalendar : Control
  {
    /// <summary>
    /// Identifies the template part responsible for displaying the days of the month.
    /// </summary>
    public const string DaysListPartName = "PART_DaysList";

    /// <summary>
    /// Identifies the template part responsible for displaying the days of the week.
    /// </summary>
    public const string DaysOfWeekListPartName = "PART_DaysOfWeekList";

    /// <summary>
    /// Gets or sets the selected date.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedDateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td><see cref="FrameworkPropertyMetadata.BindsTwoWayByDefault"/></td></tr>
    /// </table>
    /// </remarks>
    public DateTime SelectedDate
    {
      get { return (DateTime)GetValue(SelectedDateProperty); }
      set { SetValue(SelectedDateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedDate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(MonthCalendar),
        new FrameworkPropertyMetadata(
          DateTime.Now, 
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
          OnSelectedDateChanged));

    /// <summary>
    /// Gets or sets the month which is currently shown on the control.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VisibleMonthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime VisibleMonth
    {
      get { return (DateTime)GetValue(VisibleMonthProperty); }
      set { SetValue(VisibleMonthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VisibleMonth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VisibleMonthProperty =
        DependencyProperty.Register("VisibleMonth", typeof(DateTime), typeof(MonthCalendar),
        new FrameworkPropertyMetadata(
          DateTime.Now, 
          OnVisibleMonthChanged));

    /// <summary>
    /// Gets or sets the number of weeks to display.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WeeksToDisplayProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int WeeksToDisplay
    {
      get { return (int)GetValue(WeeksToDisplayProperty); }
      set { SetValue(WeeksToDisplayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WeeksToDisplay"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WeeksToDisplayProperty =
        DependencyProperty.Register("WeeksToDisplay", typeof(int), typeof(MonthCalendar), 
        new FrameworkPropertyMetadata(DefaultWeeksToDisplay));


    /// <summary>
    /// Gets or sets the culture.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CultureProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public CultureInfo Culture
    {
      get { return (CultureInfo)GetValue(CultureProperty); }
      set { SetValue(CultureProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Culture"/> property.
    /// </summary>
    public static readonly DependencyProperty CultureProperty =
        DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(MonthCalendar),
        new FrameworkPropertyMetadata(
          CultureInfo.CurrentCulture,
          OnCultureChanged));


    /// <summary>
    /// Gets or sets the text to display the month name.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthTextProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string MonthText
    {
      get { return (string)GetValue(MonthTextProperty); }
    }

    private static readonly DependencyPropertyKey MonthTextPropertyKey =
      DependencyProperty.RegisterReadOnly("MonthText", typeof(string), typeof(MonthCalendar),
      new FrameworkPropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(DateTime.Now.Month)));

    /// <summary>
    /// Identifies the <see cref="MonthText"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthTextProperty =
        MonthTextPropertyKey.DependencyProperty;


    /// <summary>
    /// Gets or sets how the time of day is set when the user selects the
    /// Today button.  The default is <see cref="F:TodayButtonTimeAction.Zero"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TodayButtonTimeActionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TodayButtonTimeAction TodayButtonTimeAction
    {
      get { return (TodayButtonTimeAction)GetValue(TodayButtonTimeActionProperty); }
      set { SetValue(TodayButtonTimeActionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TodayButtonTimeAction"/> property.
    /// </summary>
    public static readonly DependencyProperty TodayButtonTimeActionProperty =
      DependencyProperty.Register("TodayButtonTimeAction", typeof(TodayButtonTimeAction), typeof(MonthCalendar),
      new FrameworkPropertyMetadata(TodayButtonTimeAction.Zero));


    private const int DefaultWeeksToDisplay = 6;

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MonthCalendar)d).OnSelectedDateChanged();
    }

    private void OnSelectedDateChanged()
    {
      try
      {
        SelectedDate = Constrained(SelectedDate);
        if (!DateTimeUtils.SameMonth(VisibleMonth, SelectedDate) || VisibleMonth.TimeOfDay != SelectedDate.TimeOfDay)
        {
          VisibleMonth = SelectedDate;
        }

        ListBox daysList = GetTemplateChild(DaysListPartName) as ListBox;

        if (daysList != null)
        {
          if (daysList.SelectedValue == null || (DateTime)(daysList.SelectedValue) != SelectedDate)
          {
            try
            {
              daysList.SelectedValue = SelectedDate;
            }
            catch (NullReferenceException)
            {
              // Working around crash in WPF Selector control
            }
          }
        }
      }
      catch (NullReferenceException)
      {
        // Bug 1021 - scrolling through months very quickly can cause null reference exception.
        // The exception occurs deep in the WPF binding infrastructure, so I don't think there's anything
        // we can do to prevent it; the best we can do is queue it up for a retry.
        Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(OnSelectedDateChanged));
      }
    }

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MonthCalendar)d).OnCultureChanged();
    }

    private void OnCultureChanged()
    {
      string monthText = Culture.DateTimeFormat.GetAbbreviatedMonthName(VisibleMonth.Month);
      SetValue(MonthTextPropertyKey, monthText);
      ShowDays();
    }

    private static void OnVisibleMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MonthCalendar)d).OnVisibleMonthChanged();
    }

    private void OnVisibleMonthChanged()
    {
      string monthText = Culture.DateTimeFormat.GetAbbreviatedMonthName(VisibleMonth.Month);
      SetValue(MonthTextPropertyKey, monthText);
      ShowDays();
    }

    static MonthCalendar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendar), 
        new FrameworkPropertyMetadata(typeof(MonthCalendar)));
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="MonthCalendar"/> class.
    /// </summary>
    public MonthCalendar()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      BindCommands();
      BindKeyGestures(this);
    }

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(CalendarCommands.YearBack, YearBack_Executed, YearBack_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.YearForward, YearForward_Executed, YearForward_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.MonthBack, MonthBack_Executed, MonthBack_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.MonthForward, MonthForward_Executed, MonthForward_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.WeekBack, WeekBack_Executed, WeekBack_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.WeekForward, WeekForward_Executed, WeekForward_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.DayBack, DayBack_Executed, DayBack_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.DayForward, DayForward_Executed, DayForward_CanExecute));
      CommandBindings.Add(new CommandBinding(CalendarCommands.SelectToday, SelectToday_Executed));
    }

    private static void BindKeyGestures(UIElement inputElement)
    {
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.YearBack, new KeyGesture(Key.Prior, ModifierKeys.Control)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.YearForward, new KeyGesture(Key.Next, ModifierKeys.Control)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.MonthBack, new KeyGesture(Key.Prior)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.MonthForward, new KeyGesture(Key.Next)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.WeekBack, new KeyGesture(Key.Up)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.WeekForward, new KeyGesture(Key.Down)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.DayBack, new KeyGesture(Key.Left)));
      inputElement.InputBindings.Add(new InputBinding(CalendarCommands.DayForward, new KeyGesture(Key.Right)));
    }

    private const int DaysPerWeek = 7;

    private void SelectToday_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      DateTime selectedDate = SelectedDate;
      switch (TodayButtonTimeAction)
      {
        case TodayButtonTimeAction.Preserve:
          selectedDate = DateTime.Now.Date.Add(SelectedDate.TimeOfDay);
          break;
        case TodayButtonTimeAction.Now:
          selectedDate = DateTime.Now;
          break;
        default:
          selectedDate = DateTime.Now.Date;
          break;
      }
      SelectedDate = DateTime.SpecifyKind(selectedDate, SelectedDate.Kind);
      DropDownEditBox.NotifyPossibleCloseAction(this);
    }

    private void YearBack_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddYears(-1));
    }

    private void YearBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate >= DateTime.MinValue.AddYears(1);
    }

    private void YearForward_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddYears(1));
    }

    private void YearForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate <= DateTime.MaxValue.AddYears(-1);
    }

    private void MonthBack_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddMonths(-1));
    }

    private void MonthBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate >= DateTime.MinValue.AddMonths(1);
    }

    private void MonthForward_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddMonths(1));
    }

    private void MonthForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate <= DateTime.MaxValue.AddMonths(-1);
    }

    private void WeekBack_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddDays(-DaysPerWeek));
    }

    private void WeekBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate >= DateTime.MinValue.AddDays(DaysPerWeek);
    }

    private void WeekForward_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddDays(DaysPerWeek));
    }

    private void WeekForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate <= DateTime.MaxValue.AddDays(-DaysPerWeek);
    }

    private void DayBack_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddDays(-1));
    }

    private void DayBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate >= DateTime.MinValue.AddDays(1);
    }

    private void DayForward_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectedDate = Constrained(SelectedDate.AddDays(1));
    }

    private void DayForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = SelectedDate <= DateTime.MaxValue.AddDays(-1);
    }

    /// <summary>
    /// Called when a template is applied to the <see cref="MonthCalendar"/> control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ListBox daysList = GetTemplateChild(DaysListPartName) as ListBox;
      if (daysList != null)
      {
        daysList.SelectionChanged += DaysList_SelectionChanged;
        daysList.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DaysList_PreviewMouseLeftButtonUp);
        BindKeyGestures(daysList);
      }
      ItemsControl daysOfWeekList = GetTemplateChild(DaysOfWeekListPartName) as ItemsControl;
      if (daysOfWeekList != null)
      {
        daysOfWeekList.ItemsSource = Culture.DateTimeFormat.AbbreviatedDayNames;
      }

      ShowDays();
    }

    private void DaysList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      DropDownEditBox.NotifyPossibleCloseAction(this);
    }

    private void DaysList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        SelectedDate = (DateTime)(e.AddedItems[0]);
        e.Handled = true;
      }
    }

    private void ShowDays()
    {
      ListBox daysList = GetTemplateChild(DaysListPartName) as ListBox;
      if (daysList != null)
      {
        try
        {
          daysList.SelectionChanged -= DaysList_SelectionChanged;

          int cellCount = DaysPerWeek * WeeksToDisplay;
          List<DateTime?> visibleDays = new List<DateTime?>(cellCount);

          DateTime firstDayOfMonth = VisibleMonth.AddDays(-VisibleMonth.Day + 1);
          DateTime date = firstDayOfMonth;
          int i = 0;

          try
          {
            date = firstDayOfMonth.AddDays(-((int)firstDayOfMonth.DayOfWeek));
          }
          catch (ArgumentOutOfRangeException)
          {
            for (; i < ((int)firstDayOfMonth.DayOfWeek); ++i)
            {
              visibleDays.Add(null);
            }
          }

          for (; i < cellCount; ++i)
          {
            visibleDays.Add(date);
            try
            {
              date = date.AddDays(1);
            }
            catch (ArgumentOutOfRangeException)
            {
              break;
            }
          }

          for (; i < cellCount; ++i)
          {
            visibleDays.Add(null);
          }

          try
          {
            daysList.ItemsSource = visibleDays;
          }
          catch (InvalidOperationException)
          {
            // This can happen if an animation gets a huff on.  Reschedule
            // for once the animation has finished.
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(ShowDays));
          }

          try
          {
            daysList.SelectedValue = SelectedDate;
          }
          catch (NullReferenceException)
          {
            // Working around crash somewhere in the WPF Selector control
          }
        }
        finally
        {
          daysList.SelectionChanged += DaysList_SelectionChanged;
        }
      }
    }

    /// <summary>
    /// Gets or sets the minimum date that can be selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime Minimum
    {
      get { return (DateTime)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(DateTime), typeof(MonthCalendar),
        new FrameworkPropertyMetadata(DateTime.MinValue, OnRangeConstraintChanged));


    /// <summary>
    /// Gets or sets the maximum date that can be selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime Maximum
    {
      get { return (DateTime)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(DateTime), typeof(MonthCalendar),
        new FrameworkPropertyMetadata(DateTime.MaxValue, OnRangeConstraintChanged));

    private static void OnRangeConstraintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      MonthCalendar calendar = (MonthCalendar)d;
      calendar.SelectedDate = calendar.Constrained(calendar.SelectedDate);
    }

    private DateTime Constrained(DateTime date)
    {
      if (date < Minimum)
      {
        return Minimum;
      }
      else if (date > Maximum)
      {
        return Maximum;
      }
      return date;
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Style"/> for calendar
    /// buttons.
    /// </summary>
    public static object SelectButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(MonthCalendar), "SelectButtonStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the content of the "Today" button.
    /// </summary>
    public static ComponentResourceKey TodayButtonContentKey
    {
      get { return new ComponentResourceKey(typeof(MonthCalendar), "TodayButtonContent"); }
    }
  }

  /// <summary>
  /// Specifies the time of day associated with the Today button on
  /// a calendar control.
  /// </summary>
  public enum TodayButtonTimeAction
  {
    /// <summary>
    /// When the user selects the Today button, only the current date is used, and
    /// the time of day is zeroed.
    /// </summary>
    Zero,

    /// <summary>
    /// When the user selects the Today button, the current date is used, and the
    /// previously selected time of day is retained.
    /// </summary>
    Preserve,

    /// <summary>
    /// When the user selects the Today button, the current time of day is used.
    /// </summary>
    Now
  }
}
