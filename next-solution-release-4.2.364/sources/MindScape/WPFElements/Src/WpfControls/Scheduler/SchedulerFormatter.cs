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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides formatting information for a <see cref="Scheduler"/> control.
  /// </summary>
  public class SchedulerFormatter : DependencyObject
  {
    /// <summary>
    /// Gets or sets the <see cref="StyleSelector"/> for selecting the style of a <see cref="ScheduleItem"/> in day or week view.
    /// If this property is set to null, then the default schedule item style will be used.
    /// </summary>
    public StyleSelector ScheduleItemStyleSelector { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> for selecting the template for a <see cref="ScheduleItem"/> in day or week view.
    /// If this property is set to null, then the default schedule item template will be used.
    /// </summary>
    public DataTemplateSelector ScheduleItemTemplateSelector { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="StyleSelector"/> for selecting the style of a short <see cref="ScheduleItem"/>.
    /// If this property is set to null, then the default schedule item style will be used.
    /// </summary>
    public StyleSelector ShortItemStyleSelector { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> for selecting the template of a short <see cref="ScheduleItem"/>.
    /// If this property is set to null, then the default schedule item template will be used.
    /// </summary>
    public DataTemplateSelector ShortItemTemplateSelector { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="StyleSelector"/> for selecting the style of a long <see cref="ScheduleItem"/>.
    /// If this property is set to null, then the default schedule item style will be used.
    /// </summary>
    public StyleSelector LongItemStyleSelector { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> for selecting the template of a long <see cref="ScheduleItem"/>.
    /// If this property is set to null, then the default schedule item template will be used.
    /// </summary>
    public DataTemplateSelector LongItemTemplateSelector { get; set; }

    #region content properties

    /// <summary>
    /// Gets or sets the content to be displayed in the 'Month' button of the <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MonthButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object MonthButtonContent
    {
      get { return GetValue(MonthButtonContentProperty); }
      set { SetValue(MonthButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MonthButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty MonthButtonContentProperty =
      DependencyProperty.Register("MonthButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Month"));

    /// <summary>
    /// Gets or sets the content to be displayed in the 'Week' button of the <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WeekButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object WeekButtonContent
    {
      get { return GetValue(WeekButtonContentProperty); }
      set { SetValue(WeekButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WeekButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty WeekButtonContentProperty =
      DependencyProperty.Register("WeekButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Week"));

    /// <summary>
    /// Gets or sets the content to be displayed in the 'Day' button of the <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DayButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object DayButtonContent
    {
      get { return GetValue(DayButtonContentProperty); }
      set { SetValue(DayButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DayButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty DayButtonContentProperty =
      DependencyProperty.Register("DayButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Day"));

    /// <summary>
    /// Gets or sets the content to be displayed in the 'Add Appointment' button of the <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AddAppointmentButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object AddAppointmentButtonContent
    {
      get { return GetValue(AddAppointmentButtonContentProperty); }
      set { SetValue(AddAppointmentButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AddAppointmentButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty AddAppointmentButtonContentProperty =
      DependencyProperty.Register("AddAppointmentButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Add Appointment"));

    /// <summary>
    /// Gets or sets the content of the show-work-week label.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowWorkWeekLabelContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object ShowWorkWeekLabelContent
    {
      get { return GetValue(ShowWorkWeekLabelContentProperty); }
      set { SetValue(ShowWorkWeekLabelContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowWorkWeekLabelContent"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowWorkWeekLabelContentProperty =
      DependencyProperty.Register("ShowWorkWeekLabelContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Show work week"));

    /// <summary>
    /// Gets or sets the content of the show-full-week label.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowFullWeekLabelContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object ShowFullWeekLabelContent
    {
      get { return GetValue(ShowFullWeekLabelContentProperty); }
      set { SetValue(ShowFullWeekLabelContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowFullWeekLabelContent"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowFullWeekLabelContentProperty =
      DependencyProperty.Register("ShowFullWeekLabelContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Show full week"));

    /// <summary>
    /// Gets or sets the content of the next-item wing button.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NextItemButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object NextItemButtonContent
    {
      get { return GetValue(NextItemButtonContentProperty); }
      set { SetValue(NextItemButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NextItemButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty NextItemButtonContentProperty =
      DependencyProperty.Register("NextItemButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Next Appointment"));

    /// <summary>
    /// Gets or sets the content of the previous-item wing button.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PreviousItemButtonContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object PreviousItemButtonContent
    {
      get { return GetValue(PreviousItemButtonContentProperty); }
      set { SetValue(PreviousItemButtonContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PreviousItemButtonContent"/> property.
    /// </summary>
    public static readonly DependencyProperty PreviousItemButtonContentProperty =
      DependencyProperty.Register("PreviousItemButtonContent", typeof(object), typeof(SchedulerFormatter),
      new FrameworkPropertyMetadata("Previous Appointment"));

    #endregion // content properties

    #region Dialog customization properties

    /// <summary>
    /// Gets or sets a custom style for the schedule item dialog.
    /// </summary>
    public Style ScheduleItemDialogStyle { get; set; }

    /// <summary>
    /// Gets or set a custom style for the recurrence dialog.
    /// </summary>
    public Style RecurrenceDialogStyle { get; set; }

    /// <summary>
    /// Gets or sets a custom style for the recurrence deletion dialog.
    /// </summary>
    public Style DeleteRecurrenceDialogStyle { get; set; }

    #endregion // Dialog customization properties

    /// <summary>
    /// Gets or sets the default name for newly created schedule items.
    /// </summary>
    public string DefaultScheduleItemName { get; set; }
  }
}
