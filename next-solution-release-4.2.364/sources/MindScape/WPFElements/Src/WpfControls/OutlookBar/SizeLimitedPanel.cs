using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a panel which lays out its children in a stack, but truncates the layout
  /// to the available size instead of growing to meet the demands of the children.
  /// </summary>
  public class SizeLimitedPanel : Panel
  {
    private int _startIndex;

    /// <summary>
    /// Gets or sets the index of the first child to display in this panel. Any children
    /// before this index are not displayed.  The default is 0 (display all children up to
    /// the available size).
    /// </summary>
    /// <remarks>Children that are not displayed are still measured and arranged, but with
    /// zero size.</remarks>
    public int StartIndex
    {
      get { return _startIndex; }
      set
      {
        _startIndex = value;
        InvalidateMeasure();
        InvalidateArrange();
      }
    }

    #region Orientation property

    /// <summary>
    /// Gets or sets the Orientation of this <see cref="SizeLimitedPanel"/>.
    /// This is a dependency property.
    /// </summary>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SizeLimitedPanel),
      new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SizeLimitedPanel)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      InvalidateMeasure();
      InvalidateArrange();
    }

    #endregion // Orientation property

    /// <summary>
    /// Measures the child elements of the control.
    /// </summary>
    /// <param name="availableSize">An upper limit size that should not be exceeded.</param>
    /// <returns>The desired size of the element.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      double width = 0;
      double height = 0;
      int index = 0;
      foreach (UIElement element in Children)
      {
        element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        if (index >= StartIndex)
        {
          if (Orientation == Orientation.Vertical && height + element.DesiredSize.Height <= availableSize.Height)
          {
            width = Math.Max(width, element.DesiredSize.Width);
            height += element.DesiredSize.Height;
          }
          else if (Orientation == Orientation.Horizontal && width + element.DesiredSize.Width <= availableSize.Width)
          {
            width += element.DesiredSize.Width;
            height = Math.Max(height, element.DesiredSize.Height);
          }
        }
        index++;
      }
      return new Size(width, height);
    }

    /// <summary>
    /// Arranges the content of the element.
    /// </summary>
    /// <param name="finalSize">The size that this element should use to arrange its
    /// child elements.</param>
    /// <returns>The arranged size of this element and its children.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      double width = 0;
      double height = 0;
      int index = 0;
      foreach (UIElement element in Children)
      {
        if (Orientation == Orientation.Vertical && index >= StartIndex && height + element.DesiredSize.Height <= finalSize.Height)
        {
          height += element.DesiredSize.Height;
          element.Arrange(new Rect(0, height - element.DesiredSize.Height, finalSize.Width, element.DesiredSize.Height));
        }
        else if (Orientation == Orientation.Horizontal && index >= StartIndex && width + element.DesiredSize.Width <= finalSize.Width)
        {
          width += element.DesiredSize.Width;
          element.Arrange(new Rect(width - element.DesiredSize.Width, 0, element.DesiredSize.Width, finalSize.Height));
        }
        else
        {
          element.Arrange(new Rect(0, 0, 0, 0));
        }
        index++;
      }
      Size result = new Size(width, finalSize.Height);
      if (Orientation == Orientation.Vertical)
      {
        result = new Size(finalSize.Width, height);
      }
      return result;
    }
  }
}
