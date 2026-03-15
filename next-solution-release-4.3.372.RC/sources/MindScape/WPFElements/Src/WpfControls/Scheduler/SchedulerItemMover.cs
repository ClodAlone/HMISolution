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
  /// Provides a user interface for moving a <see cref="ScheduleItem"/> by dragging
  /// with the mouse.
  /// </summary>
  public class ScheduleItemMover : Control
  {
    //TODO: might want to find a better way to do this:
    // need to know whenever an appointment is being dragged.
    // This is to fix the issue with the "Click to add item" button appearing in month view whenever an item is being dragged.
    internal static bool IsDragging;

    private ITimeUIElement _surface;
    private DateTime _lastMouseOverTime;

#if !SILVERLIGHT
    private static ScheduleItem _draggingItem = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleItemMover"/> class.
    /// </summary>
    public ScheduleItemMover()
    {
      if(IsDragging)
      {
        // TODO: Careful, this may cause problems.
        Dispatcher.BeginInvoke(new Action(EnsureMouseCapture));
      }
    }

    private void EnsureMouseCapture()
    {
      if (_draggingItem == ScheduleItem)
      {
        CaptureMouse();
      }
    }
#endif

    /// <summary>
    /// Gets or sets the <see cref="ScheduleItem"/> associated with the mover.
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
      DependencyProperty.Register("ScheduleItem", typeof(ScheduleItem), typeof(ScheduleItemMover),
      new PropertyMetadata(null));

    /// <summary>
    /// Called by the framework when the mouse leaves the control.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      if (IsDragging)
      {
        CaptureMouse();
      }
    }

    /// <summary>
    /// Called by the framework when the user moves the mouse.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

#if !SILVERLIGHT
      if (_surface == null)
      {
        _surface = FindContainingSurface();
        _lastMouseOverTime = _surface.GetDateTime(e.GetPosition((UIElement)_surface), TimeEnd.StartTime);
        if (IsDragging)
        {
          CaptureMouse();
        }
      }
#endif
      UIElement surfaceElement = _surface as UIElement;
      if (IsDragging && surfaceElement != null)
      {
        DateTime mouseOverTime = _surface.GetDateTime(e.GetPosition(surfaceElement), TimeEnd.StartTime);
        int deltaMins = -DateTimeUtils.DurationInMinutes(mouseOverTime, _lastMouseOverTime);
        if (deltaMins != 0)
        {
          ScheduleItem.MoveBy(deltaMins);
          _lastMouseOverTime = mouseOverTime;
        }
      }
      else
      {
        return;
      }
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

      IsDragging = true;
      _surface = FindContainingSurface();
      _lastMouseOverTime = _surface.GetDateTime(e.GetPosition((UIElement)_surface), TimeEnd.StartTime);
      e.Handled = false;

#if !SILVERLIGHT
      _draggingItem = ScheduleItem;
      CaptureMouse();
#endif
    }

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

      e.Handled = false;
    }

    internal void CancelDrag()
    {
      IsDragging = false;
      ReleaseMouseCapture();
    }

    private ITimeUIElement FindContainingSurface()
    {
      return VisualTreeUtils.FindContaining<ITimeUIElement>(this);
    }
  }
}
