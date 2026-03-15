using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Mindscape.WpfElements.Internal;
using System.Windows.Input;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  ///  A base class for day schedule controls.
  /// </summary>
  public abstract class DayScheduleBase : Control, IScheduleProvider
  {
    private SchedulerCanvasBase _schedulerCanvas;
    private DayModel _day;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaySchedule"/> class.
    /// </summary>
    public DayScheduleBase()
    {
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _schedulerCanvas = GetTemplateChild("PART_SchedulerCanvas") as SchedulerCanvasBase;
      SetCanvasDays();
    }

    private void SetCanvasDays()
    {
      if (_schedulerCanvas != null && Schedule != null)
      {
        IList<DayModel> days = new List<DayModel>();
        DayModel day = Schedule.GetDay(Date);
        Day = day;
        days.Add(day);
        _schedulerCanvas.Days = days;
      }
    }

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
      DependencyProperty.Register("ScheduleItemDialogStyle", typeof(Style), typeof(DayScheduleBase),
      new FrameworkPropertyMetadata(OnScheduleItemDialogStyleChanged));

    private static void OnScheduleItemDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DayScheduleBase)d).OnScheduleItemDialogStyleChanged();
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
      DependencyProperty.Register("RecurrenceDialogStyle", typeof(Style), typeof(DayScheduleBase),
      new FrameworkPropertyMetadata(OnRecurrenceDialogStyleChanged));

    private static void OnRecurrenceDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DayScheduleBase)d).OnRecurrenceDialogStyleChanged();
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
      DependencyProperty.Register("DeleteRecurrenceDialogStyle", typeof(Style), typeof(DayScheduleBase),
      new FrameworkPropertyMetadata(OnDeleteRecurrenceDialogStyleChanged));

    private static void OnDeleteRecurrenceDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DayScheduleBase)d).OnDeleteRecurrenceDialogStyleChanged();
    }

    private void OnDeleteRecurrenceDialogStyleChanged()
    {
    }

    #endregion // DeleteRecurrenceDialogStyle Property

    #endregion // Dialog customization properties

    /// <summary>
    /// Gets or sets the schedule displayed on this control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScheduleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Schedule Schedule
    {
      get { return (Schedule)GetValue(ScheduleProperty); }
      set { SetValue(ScheduleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Schedule"/> property.
    /// </summary>
    public static readonly DependencyProperty ScheduleProperty =
      DependencyProperty.Register("Schedule", typeof(Schedule), typeof(DayScheduleBase),
      new FrameworkPropertyMetadata(new Schedule(), OnScheduleChanged));

    private static void OnScheduleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DayScheduleBase)d).OnScheduleChanged();
    }

    private void OnScheduleChanged()
    {
      SetCanvasDays();
    }

    /// <summary>
    /// Gets or sets the date for which to display schedule items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime Date
    {
      get { return (DateTime)GetValue(DateProperty); }
      set { SetValue(DateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Date"/> property.
    /// </summary>
    public static readonly DependencyProperty DateProperty =
      DependencyProperty.Register("Date", typeof(DateTime), typeof(DayScheduleBase),
      new FrameworkPropertyMetadata(DateTime.Now, OnDateChanged));

    private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DayScheduleBase)d).OnDateChanged(e);
    }

    /// <summary>
    /// Called when the date changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnDateChanged(DependencyPropertyChangedEventArgs e)
    {
      SetCanvasDays();
      OnDateChanged();
    }

    /// <summary>
    /// Raised when the date of this <see cref="DayScheduleBase"/> changes.
    /// </summary>
    public event EventHandler DateChanged;

    private void OnDateChanged()
    {
      EventHandler handler = DateChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the <see cref="DayModel"/> displayed by this <see cref="DayScheduleBase"/>.
    /// </summary>
    protected DayModel Day
    {
      get { return _day; }
      private set
      {
        if (_day != null)
        {
          _day.ItemsChanged -= new EventHandler<ScheduleItemCollectionChangedEventArgs>(Day_ItemsChanged);
        }
        _day = value;
        if (_day != null)
        {
          _day.ItemsChanged += new EventHandler<ScheduleItemCollectionChangedEventArgs>(Day_ItemsChanged);
        }
      }
    }

    /// <summary>
    /// Deselects all time slots and schedule items.
    /// </summary>
    public void DeselectAll()
    {
      Day.DeselectAll();
      if (Schedule.SelectedItem != null)
      {
        Schedule.SelectedItem.IsSelected = false;
      }
    }

    private void Day_ItemsChanged(object sender, ScheduleItemCollectionChangedEventArgs e)
    {
      OnDayItemsChanged();
    }

    /// <summary>
    /// Called when schedule items are added or removed from the currently displayed day.
    /// </summary>
    protected virtual void OnDayItemsChanged()
    {
    }

    /// <summary>
    /// Gets the range currently selected in the control.
    /// </summary>
    public DateRange SelectedDateRange
    {
      get
      {
        DateTime start;
        DateTime end;
        if (Schedule.SelectedItem != null)
        {
          start = Schedule.SelectedItem.StartTime;
          end = Schedule.SelectedItem.EndTime;
        }
        else
        {
          DateTime date = Day.Date;
          TimeSlot selectedStartTime = Day.SelectedStartTime;
          TimeSlot selectedEndTime = Day.SelectedEndTime;
          start = new DateTime(date.Year, date.Month, date.Day, selectedStartTime.Hour, selectedStartTime.Minute, 0);
          end = new DateTime(date.Year, date.Month, date.Day, selectedEndTime.Hour, selectedEndTime.Minute, 0).AddMinutes(30);
          if (start.Ticks > end.Ticks)
          {
            start = new DateTime(date.Year, date.Month, date.Day, selectedEndTime.Hour, selectedEndTime.Minute, 0);
            end = new DateTime(date.Year, date.Month, date.Day, selectedStartTime.Hour, selectedStartTime.Minute, 0).AddMinutes(30);
          }
        }
        return new DateRange(start, end);
      }
    }

    SchedulerElement IScheduleProvider.FindSchedulerElement(ScheduleItem item)
    {
      SchedulerElement element = null;
      if (_schedulerCanvas != null)
      {
        element = _schedulerCanvas.FindSchedulerElement(item);
      }
      return element;
    }

    /// <summary>
    /// Called when a key is pressed while this <see cref="DayScheduleBase"/> has focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Delete)
      {
        ProcessDeleteKey();
      }
    }

    private void ProcessDeleteKey()
    {
      ScheduleItem lastSelected = Schedule.SelectedItem;
      if (lastSelected != null)
      {
        if (lastSelected.IsInstanceOfRecurringItem)
        {
#if SILVERLIGHT
          DeleteRecurrenceDialog dialog = new DeleteRecurrenceDialog(lastSelected);
          dialog.Closed += new EventHandler(DeleteRecurrenceDialog_Closed);
          dialog.Show();
#else
          Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
          DeleteRecurrenceDialog dlg = new DeleteRecurrenceDialog(lastSelected);
          if (scheduler != null && scheduler.DeleteRecurrenceDialogStyle != null)
          {
            dlg.Style = scheduler.DeleteRecurrenceDialogStyle;
          }
          else if (DeleteRecurrenceDialogStyle != null)
          {
            dlg.Style = DeleteRecurrenceDialogStyle;
          }
          dlg.Closed += new RoutedEventHandler(DeleteRecurrenceDialog_Closed);
          DialogHelper.ShowDeleteRecurrenceDialog(dlg, this, scheduler == null ? null : scheduler.Formatter);
#endif
        }
        else
        {
          Schedule.RemoveItem(lastSelected);
          //ApplyDefaultSelection(lastSelected);
        }
      }
    }

    private void DeleteRecurrenceDialog_Closed(object sender, EventArgs e)
    {
      DeleteRecurrenceDialog dialog = sender as DeleteRecurrenceDialog;
      if (dialog != null)
      {
        if (dialog.DialogResult == true)
        {
          Schedule.RemoveItem(dialog.ScheduleItem, dialog.DeleteSeries);
          //ApplyDefaultSelection(dialog.ScheduleItem);
        }
      }
    }
  }
}
