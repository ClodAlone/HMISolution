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
  /// Provides a user interface for changing the start or end time of a
  /// <see cref="ScheduleItem"/> by dragging with the mouse.
  /// </summary>
  public class ScheduleItemTimeChanger : Control
  {
    internal static bool IsDragging;
    //private double _yPressed;
    private ITimeUIElement _surface;

#if !SILVERLIGHT
    private static TimeEnd _draggingTimeEnd;
    private static ScheduleItem _draggingItem = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleItemTimeChanger"/> class.
    /// </summary>
    public ScheduleItemTimeChanger()
    {
      if (IsDragging)
      {
        // TODO: Careful, this may cause problems.
        Dispatcher.BeginInvoke(new Action(EnsureMouseCapture));
      }
    }

    private void EnsureMouseCapture()
    {
      if (_draggingTimeEnd == TimeEnd && _draggingItem == ScheduleItem)
      {
        CaptureMouse();
      }
    }
#endif

    /// <summary>
    /// Called by the framework when the user releases the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      IsDragging = false;

#if !SILVERLIGHT
      ReleaseMouseCapture();
#endif
    }

    /// <summary>
    /// Called by the framework when the user presses the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
      if (scheduler != null && scheduler.IsReadOnly)
      {
        e.Handled = false;
        return;
      }

      BeginDrag();
    }

    /// <summary>
    /// Called by the framework when the user moves the mouse.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
#if !SILVERLIGHT
      if (IsDragging && _draggingTimeEnd == TimeEnd && _draggingItem == ScheduleItem)
      {
        CaptureMouse();
        if (_surface == null)
        {
          _surface = FindContainingSurface();
          //_yPressed = e.GetPosition((UIElement)_surface).Y;
        }
      }
#endif

      if (IsDragging && (_surface as UIElement) != null)
      {
        ApplyDrag(e);
      }
    }

    private void ApplyDrag(MouseEventArgs e)
    {
      UIElement surfaceElement = _surface as UIElement;
      var dragPosition = e.GetPosition(surfaceElement);
      //double y = dragPosition.Y;
      DateTime newTime = _surface.GetDateTime(dragPosition, TimeEnd);

      if (TimeEnd == TimeEnd.StartTime && newTime < ScheduleItem.EndTime && !ScheduleItem.StartTime.Equals(newTime))
      {
        ScheduleItem.StartTime = newTime;
      }
      else if (TimeEnd == TimeEnd.EndTime && newTime > ScheduleItem.StartTime && !ScheduleItem.EndTime.Equals(newTime))
      {
        ScheduleItem.EndTime = newTime;
      }
    }

    private void BeginDrag()
    {
      IsDragging = true;
      CaptureMouse();
      _surface = FindContainingSurface();
      //_yPressed = e.GetPosition((UIElement)_surface).Y;
#if !SILVERLIGHT
      _draggingTimeEnd = TimeEnd;
      _draggingItem = ScheduleItem;
#endif
    }

    //private static DateTime ConvertToDateTime(DateTime baseTime, double y) //TODO: latter, some evolutionary form of this method should be placed in AppointmentSurface.
    //{
    //  double timeConversion = y / 60; //This value 60 is the height of an hour. TODO: move somewhere else!!
    //  int hour = (int)timeConversion;
    //  double min = timeConversion - hour;
    //  int minute = (int)(min * 60);
    //  minute = (minute / 30) * 30;
    //  if (y < 0)
    //  {
    //    hour = 0;
    //    minute = 0;
    //  }
    //  if (y > 60 * 24)
    //  {
    //    hour = 0;
    //    minute = 0;
    //  }
    //  DateTime time = new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, hour, minute, 0);
    //  if (y > 60 * 24)
    //  {
    //    time = time.AddHours(24);
    //  }
    //  return time;
    //}

    /// <summary>
    /// Gets or sets the <see cref="ScheduleItem"/> associated with the changer.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScheduleItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ScheduleItem ScheduleItem
    {
      get { return (ScheduleItem)GetValue(ScheduleItemProperty); }
      set { SetValue(ScheduleItemProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScheduleItem"/> property.
    /// </summary>
    public static readonly DependencyProperty ScheduleItemProperty =
      DependencyProperty.Register("ScheduleItem", typeof(ScheduleItem), typeof(ScheduleItemTimeChanger),
      null);


    /// <summary>
    /// Gets or sets the end (start or finish) of the <see cref="ScheduleItem"/> will
    /// be changed by dragging this <see cref="ScheduleItemTimeChanger"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TimeEndProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeEnd TimeEnd
    {
      get { return (TimeEnd)GetValue(TimeEndProperty); }
      set { SetValue(TimeEndProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TimeEnd"/> property.
    /// </summary>
    public static readonly DependencyProperty TimeEndProperty =
      DependencyProperty.Register("TimeEnd", typeof(TimeEnd), typeof(ScheduleItemTimeChanger),
      new PropertyMetadata(TimeEnd.StartTime));

    private ITimeUIElement FindContainingSurface()
    {
      return VisualTreeUtils.FindContaining<ITimeUIElement>(this);
    }
  }
}
