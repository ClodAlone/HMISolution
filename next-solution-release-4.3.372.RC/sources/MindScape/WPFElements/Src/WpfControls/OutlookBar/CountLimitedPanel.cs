using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a panel which lays out its children in a stack, but truncates the layout
  /// after a specified number of children.
  /// </summary>
  public class CountLimitedPanel : Panel
  {
    private int _visibleCount;

    /// <summary>
    /// Gets or sets the number of children that will be displayed in this panel. All other children
    /// still belong to this panel, but are not displayed.
    /// </summary>
    /// <remarks>Children that are not displayed are still measured and arranged, but with
    /// zero size.</remarks>
    public int VisibleItemCount
    {
      get { return _visibleCount; }
      set
      {
        _visibleCount = value;
        InvalidateMeasure();
        InvalidateArrange();
      }
    }

    #region Orientation property

    /// <summary>
    /// Gets or sets the Orientation of this <see cref="CountLimitedPanel"/>.
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
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CountLimitedPanel),
      new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CountLimitedPanel)d).OnOrientationChanged();
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
      Size size = new Size(availableSize.Width, Double.PositiveInfinity);
      if (Orientation == Orientation.Horizontal)
      {
        size = new Size(Double.PositiveInfinity, availableSize.Height);
      }
      double height = 0;
      double width = 0;
      int index = 0;
      foreach (UIElement element in Children)
      {
        element.Measure(size);
        if (index < VisibleItemCount)
        {
          height += element.DesiredSize.Height;
          width += element.DesiredSize.Width;
        }
        index++;
      }
      Size result = new Size(availableSize.Width, height);
      if (Orientation == Orientation.Horizontal)
      {
        result = new Size(width, availableSize.Height);
      }
      return result;
    }

    /// <summary>
    /// Arranges the content of the element.
    /// </summary>
    /// <param name="finalSize">The size that this element should use to arrange its
    /// child elements.</param>
    /// <returns>The arranged size of this element and its children.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      Size size = new Size(finalSize.Width, Double.PositiveInfinity);
      if (Orientation == Orientation.Horizontal)
      {
        size = new Size(Double.PositiveInfinity, finalSize.Height);
      }
      double height = 0;
      double width = 0;
      int index = 0;
      foreach (UIElement element in Children)
      {
        element.Measure(size);
        if (index < VisibleItemCount)
        {
          height += element.DesiredSize.Height;
          width += element.DesiredSize.Width;
          if (Orientation == Orientation.Vertical)
          {
            element.Arrange(new Rect(0, height - element.DesiredSize.Height, finalSize.Width, element.DesiredSize.Height));
          }
          else if (Orientation == Orientation.Horizontal)
          {
            element.Arrange(new Rect(width - element.DesiredSize.Width, 0, element.DesiredSize.Width, finalSize.Height));
          }
        }
        else
        {
          element.Arrange(new Rect(0, 0, 0, 0));
        }
        index++;
      }
      Size result = new Size(finalSize.Width, height);
      if (Orientation == Orientation.Horizontal)
      {
        result = new Size(width, finalSize.Height);
      }
      return result;
    }
  }
}
