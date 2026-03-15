using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Infralution.Licensing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Arranges child elements into a single line and sizes them according to requested
  /// proportions.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class ProportionalStackPanel : StackPanel
  {
    /// <summary>
    /// Measures the child elements of the control.
    /// </summary>
    /// <param name="constraint">An upper limit size that should not be exceeded.</param>
    /// <returns>The desired size of the element.</returns>
    protected override Size MeasureOverride(Size constraint)
    {
      double total = 0;
      foreach (UIElement child in InternalChildren)
      {
        total += GetProportion(child);
      }

      Size size = new Size();
      bool horizontal = (Orientation == Orientation.Horizontal);
      foreach (UIElement child in InternalChildren)
      {
        Size offeredSize = constraint;
        if (horizontal)
        {
          offeredSize.Width = constraint.Width * (GetProportion(child) / total);
        }
        else
        {
          offeredSize.Height = constraint.Height * (GetProportion(child) / total);
        }
        child.Measure(offeredSize);
        Size desiredSize = child.DesiredSize;
        if (horizontal)
        {
          size.Width += desiredSize.Width;
          size.Height = Math.Max(size.Height, desiredSize.Height);
        }
        else
        {
          size.Height += desiredSize.Height;
          size.Width = Math.Max(size.Width, desiredSize.Width);
        }
      }

      return size;
    }

    /// <summary>
    /// Arranges the content of the element.
    /// </summary>
    /// <param name="arrangeSize">The size that this element should use to arrange its
    /// child elements.</param>
    /// <returns>The arranged size of this element and its children.</returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
      double total = 0;
      foreach (UIElement child in InternalChildren)
      {
        total += GetProportion(child);
      }

      bool horizontal = (Orientation == Orientation.Horizontal);
      Rect rect = new Rect(arrangeSize);
      double lastOffset = 0;
      foreach (UIElement child in InternalChildren)
      {
        if (horizontal)
        {
          rect.X += lastOffset;
          lastOffset = arrangeSize.Width * (GetProportion(child) / total);
          rect.Width = lastOffset;
          rect.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
        }
        else
        {
          rect.Y += lastOffset;
          lastOffset = arrangeSize.Height * (GetProportion(child) / total);
          rect.Height = lastOffset;
          rect.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
        }

        child.Arrange(rect);
      }

      return arrangeSize;
    }


    /// <summary>
    /// Gets the value of the Proportion attached property for a specified element.
    /// </summary>
    /// <param name="obj">The element from which the property value is read.</param>
    /// <returns>The property value for the element.</returns>
    public static int GetProportion(DependencyObject obj)
    {
      return (int)obj.GetValue(ProportionProperty);
    }

    /// <summary>
    /// Sets the value of the Proportion attached property for a specified element.
    /// </summary>
    /// <param name="obj">The element on which the property value is set.</param>
    /// <param name="value">The value to which the property is set.</param>
    public static void SetProportion(DependencyObject obj, int value)
    {
      obj.SetValue(ProportionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the proportional size of a child element
    /// within a <see cref="ProportionalStackPanel"/>.
    /// </summary>
    public static readonly DependencyProperty ProportionProperty =
        DependencyProperty.RegisterAttached("Proportion", typeof(int), 
        typeof(ProportionalStackPanel),
        new FrameworkPropertyMetadata(
          1,
          FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));


  }
}
