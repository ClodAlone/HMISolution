using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a text area that is read-only normally, but can be clicked to
  /// enable editing.
  /// </summary>
  public class EditableTextBlock : TextBox
  {
    static EditableTextBlock()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock),
        new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextBlock"/> class.
    /// </summary>
    public EditableTextBlock()
    {
      //LicensingUtils.NotifyTrialVersion();
      PreviewMouseLeftButtonDown += new MouseButtonEventHandler(EditableTextBlock_PreviewMouseLeftButtonDown);
    }

    private void EditableTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _lastMouseDown = e.GetPosition(this);

      e.Handled = false;

      base.OnMouseLeftButtonDown(e);
    }

    Point _lastMouseDown = new Point(Double.MinValue, Double.MaxValue);

    /// <summary>
    /// Called by the framework when the user releases the left mouse button.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      Point position = e.GetPosition(this);
      double distanceMagnitude = GeometryUtils.DistanceMagnitude(position, _lastMouseDown);

      if (distanceMagnitude <= 16)
      {
        Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this); // TODO remove this.
        if (scheduler != null && scheduler.IsReadOnly)
        {
          return;
        }
        IsInEditMode = true;
      }
      else
      {
        e.Handled = false;
      }

      base.OnMouseLeftButtonUp(e);
    }

    /// <summary>
    /// Called by the framework when the user presses a key.
    /// </summary>
    /// <param name="e">The key event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        IsInEditMode = false;
      }

      // This fixes a bug where if the caret is at the end of the EditableTextBlock on a SchedulerElement, and the user hits delete, 
      // Then the event gets routed to the Scheduler, and the element gets deleted.
      // But is it fine to do this? :
      if (e.Key == Key.Delete)
      {
        e.Handled = true;
      }
    }

    /// <summary>
    /// Called by the framework when the control loses focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);

      IsInEditMode = false;
    }

    /// <summary>
    /// Gets or sets whether the <see cref="EditableTextBlock"/> is in edit mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsInEditModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsInEditMode
    {
      get { return (bool)GetValue(IsInEditModeProperty); }
      set { SetValue(IsInEditModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsInEditMode"/> property.
    /// </summary>
    public static readonly DependencyProperty IsInEditModeProperty =
      DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock),
      new PropertyMetadata(false));
  }
}
