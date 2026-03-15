using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for modifying the saturation and brightness of a color.
  /// </summary>
  public class ColorSquare : NotifyingColorPickerBase
  {
    private bool _isMouseDown;

    static ColorSquare()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSquare),
        new FrameworkPropertyMetadata(typeof(ColorSquare)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorSquare"/> class.
    /// </summary>
    public ColorSquare()
    {
      SizeChanged += new SizeChangedEventHandler(ColorSquare_SizeChanged);
    }

    private void ColorSquare_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateCursorPosition();
    }

    /// <summary>
    /// Called when the left mouse button is pressed over this <see cref="ColorSquare"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      _isMouseDown = true;
      Point position = e.GetPosition(this);
      SetCursorPosition(position);
      CaptureMouse();
    }

    /// <summary>
    /// Called when the mouse is moved over this <see cref="ColorSquare"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (_isMouseDown)
      {
        Point position = e.GetPosition(this);
        SetCursorPosition(position);
      }
    }

    /// <summary>
    /// Called when the left mouse button is released over this <see cref="ColorSquare"/> .
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      _isMouseDown = false;
      ReleaseMouseCapture();
    }

    /// <summary>
    /// Called when this <see cref="ColorSquare"/> loses mouse capture.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);

      if (_isMouseDown)
      {
        CaptureMouse();
      }
    }

    private void SetCursorPosition(Point position)
    {
      double x = Math.Max(0, Math.Min(ActualWidth, position.X));
      double y = Math.Max(0, Math.Min(ActualHeight, position.Y));
      position = new Point(x, y);
      SetValue(CursorPositionPropertyKey, position);
      UpdateSV(position);
    }

    private void UpdateSV(Point point)
    {
      if (ActualWidth != 0 && ActualHeight != 0)
      {
        NotifyingColor.S = Math.Max(0, Math.Min(1, point.X / ActualWidth));
        NotifyingColor.V = Math.Max(0, Math.Min(1, 1 - (point.Y / ActualHeight)));
      }
    }

    private void UpdateCursorPosition()
    {
      double x = NotifyingColor.S * ActualWidth;
      double y = ActualHeight - (NotifyingColor.V * ActualHeight);
      SetCursorPosition(new Point(x, y));
    }

    #region CursorPosition Property

    /// <summary>
    /// Gets the position of the color cursor.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CursorPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Point CursorPosition
    {
      get { return (Point)GetValue(CursorPositionProperty); }
    }

    private static readonly DependencyPropertyKey CursorPositionPropertyKey =
        DependencyProperty.RegisterReadOnly("CursorPosition", typeof(Point), typeof(ColorSquare), new UIPropertyMetadata(new Point()));

    /// <summary>
    /// Identifies the <see cref="CursorPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty CursorPositionProperty =
        CursorPositionPropertyKey.DependencyProperty;

    #endregion // CursorPosition Property

    /// <summary>
    /// Called when the selected color changes.
    /// </summary>
    protected override void OnSelectedColorChanged()
    {
      base.OnSelectedColorChanged();
      UpdateCursorPosition();
    }
  }
}
