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
using System.Windows.Controls.Primitives;
using System.ComponentModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides a user interface for navigating through a <see cref="Schedule"/> displayed in
  /// a <see cref="Scheduler"/> control.
  /// </summary>
  public class SchedulerNavigationBar : Control, INotifyPropertyChanged
  {
#if !SILVERLIGHT
    static SchedulerNavigationBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SchedulerNavigationBar),
        new FrameworkPropertyMetadata(typeof(SchedulerNavigationBar)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerNavigationBar"/> class.
    /// </summary>
    public SchedulerNavigationBar()
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(SchedulerNavigationBar);
#endif
    }

#if SILVERLIGHT
    /// <summary>
    /// Gets or sets a command to go to the previous view (e.g. previous week).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PreviousCommandProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ICommand PreviousCommand
    {
      get { return (ICommand)GetValue(PreviousCommandProperty); }
      set { SetValue(PreviousCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PreviousCommand"/> property.
    /// </summary>
    public static readonly DependencyProperty PreviousCommandProperty =
      DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(SchedulerNavigationBar),
      new PropertyMetadata(null));


    /// <summary>
    /// Gets or sets a command to go to the next view (e.g. next week).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NextCommandProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ICommand NextCommand
    {
      get { return (ICommand)GetValue(NextCommandProperty); }
      set { SetValue(NextCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NextCommand"/> property.
    /// </summary>
    public static readonly DependencyProperty NextCommandProperty =
      DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(SchedulerNavigationBar),
      new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a command to add a new item to the schedule.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AddScheduleItemCommandProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ICommand AddScheduleItemCommand
    {
      get { return (ICommand)GetValue(AddScheduleItemCommandProperty); }
      set { SetValue(AddScheduleItemCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AddScheduleItemCommand"/> property.
    /// </summary>
    public static readonly DependencyProperty AddScheduleItemCommandProperty =
      DependencyProperty.Register("AddScheduleItemCommand", typeof(ICommand), typeof(SchedulerNavigationBar),
      new PropertyMetadata(null));
#endif

    /// <summary>
    /// Gets or sets the date range to be displayed in the title.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DateRangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateRangeDisplayInfo DateRange
    {
      get { return (DateRangeDisplayInfo)GetValue(DateRangeProperty); }
      set { SetValue(DateRangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DateRange"/> property.
    /// </summary>
    public static readonly DependencyProperty DateRangeProperty =
      DependencyProperty.Register("DateRange", typeof(DateRangeDisplayInfo), typeof(SchedulerNavigationBar),
      new PropertyMetadata(null));

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
      DependencyProperty.Register("ToolBarContent", typeof(object), typeof(SchedulerNavigationBar),
      new PropertyMetadata(null));

    #endregion // ToolBarContent property

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }
}
