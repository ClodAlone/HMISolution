using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that displays a list of times.
  /// </summary>
  public class TimeOfDayBar : Control, INotifyPropertyChanged
  {
    private readonly ReadOnlyCollection<TimeOfDay> _hoursOfTheDay;
    private ScrollViewer _scrollViewer;

    static TimeOfDayBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeOfDayBar),
        new FrameworkPropertyMetadata(typeof(TimeOfDayBar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeOfDayBar"/> class.
    /// </summary>
    public TimeOfDayBar()
    {
      List<TimeOfDay> hours = new List<TimeOfDay>();
      for (int i = 0; i < 24; i++)
      {
        hours.Add(new TimeOfDay(i));
      }
      _hoursOfTheDay = hours.AsReadOnly();
      SchedulerUpdateTimer.MinuteTimer.Tick += new EventHandler(Timer_Tick);
#if !SILVERLIGHT
      Unloaded += new RoutedEventHandler(TimeOfDayBar_Unloaded);
#endif
    }

#if !SILVERLIGHT
    private void TimeOfDayBar_Unloaded(object sender, RoutedEventArgs e)
    {
      SchedulerUpdateTimer.MinuteTimer.Tick -= new EventHandler(Timer_Tick);
    }
#endif

    private void Timer_Tick(object sender, EventArgs e)
    {
      OnPropertyChanged("Now");
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
      VerticalOffset = e.VerticalOffset;
    }

    /// <summary>
    /// Gets the times to display.
    /// </summary>
    public ReadOnlyCollection<TimeOfDay> Times
    {
      get { return _hoursOfTheDay; }
    }

    /// <summary>
    /// Gets or sets the VerticalOffset of the scroll bar.
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
      DependencyProperty.Register("VerticalOffset", typeof(double), typeof(TimeOfDayBar),
      new FrameworkPropertyMetadata(OnVerticalOffsetChanged));

    private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeOfDayBar)d).OnVerticalOffsetChanged();
    }

    private void OnVerticalOffsetChanged()
    {
      if (_scrollViewer != null)
      {
        _scrollViewer.ScrollToVerticalOffset(VerticalOffset);
      }
    }

    /// <summary>
    /// Gets or sets the whether or not to display the time pointer.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsTimePointerVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsTimePointerVisible
    {
      get { return (bool)GetValue(IsTimePointerVisibleProperty); }
      set { SetValue(IsTimePointerVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsTimePointerVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsTimePointerVisibleProperty =
      DependencyProperty.Register("IsTimePointerVisible", typeof(bool), typeof(TimeOfDayBar),
      new FrameworkPropertyMetadata());

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
