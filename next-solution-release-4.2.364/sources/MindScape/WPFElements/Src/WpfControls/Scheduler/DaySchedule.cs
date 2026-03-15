using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Mindscape.WpfElements.Internal;
using System.Windows.Input;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for displaying the schedule items of a single day.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DaySchedule : DayScheduleBase, INotifyPropertyChanged
  {
    private ScrollViewer _scrollViewer;

    static DaySchedule()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DaySchedule),
        new FrameworkPropertyMetadata(typeof(DaySchedule)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DaySchedule"/> class.
    /// </summary>
    public DaySchedule()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      CommandBindings.Add(new CommandBinding(SchedulerCommands.PreviousScheduleItemCommand, PreviousScheduleItem_Executed, PreviousScheduleItem_CanExecute));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.NextScheduleItemCommand, NextScheduleItem_Executed, NextScheduleItem_CanExecute));

      RequestBringIntoView += new RequestBringIntoViewEventHandler(DaySchedule_RequestBringIntoView);
    }

    private void DaySchedule_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      e.Handled = true;
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
      if (_scrollViewer != null)
      {
        _scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
      }
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (e.Source is ScrollViewer)
      {
        VerticalOffset = e.VerticalOffset;
      }
    }

    /// <summary>
    /// Gets or sets the VerticalOffset.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VerticalOffsetProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double VerticalOffset
    {
      get { return (double)GetValue(VerticalOffsetProperty); }
      set { SetValue(VerticalOffsetProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VerticalOffset"/> property.
    /// </summary>
    public static readonly DependencyProperty VerticalOffsetProperty =
      DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DaySchedule),
      new FrameworkPropertyMetadata(OnVerticalOffsetChanged));

    private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DaySchedule)d).OnVerticalOffsetChanged();
    }

    private void OnVerticalOffsetChanged()
    {
      if (_scrollViewer != null)
      {
        _scrollViewer.ScrollToVerticalOffset(VerticalOffset);
      }
    }

    #region wing button support

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

    private void GotoPreviousAppointment()
    {
      ScheduleItem scheduleItem = Schedule.GetPreviousEarliestItem(Date);
      if (scheduleItem != null)
      {
        Date = scheduleItem.StartTime.Date;
        scheduleItem.IsSelected = true;
      }
    }

    private bool CanGotoPreviousAppointment
    {
      get { return IsPreviousWingEnabled; }
    }

    private void GotoNextAppointment()
    {
      ScheduleItem scheduleItem = Schedule.GetNextEarliestItem(Date);
      Date = scheduleItem.StartTime.Date;
      scheduleItem.IsSelected = true;
    }

    private bool CanGotoNextAppointment
    {
      get { return IsNextWingEnabled; }
    }

    /// <summary>
    /// Gets whether the "previous item" wing button is enabled.
    /// </summary>
    public bool IsPreviousWingEnabled
    {
      get { return Schedule.EarliestItemDate < Date.Date; }
    }

    /// <summary>
    /// Gets whether the "next item" wing button is enabled.
    /// </summary>
    public bool IsNextWingEnabled
    {
      get { return Date.Date.AddHours(24) < Schedule.LatestItemDate; }
    }

    /// <summary>
    /// Gets whether the view needs to display the "wing" buttons (previous and next schedule item).
    /// </summary>
    public bool ShowWings
    {
      get
      {
        return (IsPreviousWingEnabled || IsNextWingEnabled)
          && (Day != null && !Day.HasItems());
      }
    }

    /// <summary>
    /// Called when the date changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDateChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnDateChanged(e);

      NotifyDateRangeChanged();
    }

    /// <summary>
    /// Called when a schedule item has been added or removed from the currently displayed day.
    /// </summary>
    protected override void OnDayItemsChanged()
    {
      NotifyDateRangeChanged();
    }

    private void NotifyDateRangeChanged()
    {
      OnPropertyChanged("DateRangeDisplayInfo");
      OnPropertyChanged("ShowWings");
      OnPropertyChanged("IsPreviousWingEnabled");
      OnPropertyChanged("IsNextWingEnabled");
      CommandManager.InvalidateRequerySuggested();
    }

    #endregion

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
