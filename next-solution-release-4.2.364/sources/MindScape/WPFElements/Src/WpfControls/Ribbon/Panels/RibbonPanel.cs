using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A panel that provides layout logic for items within a <see cref="RibbonTab"/>.
  /// </summary>
  public class RibbonTabPanel : Panel
  {
    /// <summary>
    /// Measures the items within this <see cref="RibbonTabPanel"/> and returns the desired size.
    /// </summary>
    /// <param name="availableSize">The available size of this <see cref="RibbonTabPanel"/>.</param>
    /// <returns>The desired size of this <see cref="RibbonTabPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      double availableWidth = availableSize.Width;
      if (Double.IsInfinity(availableSize.Width) || Double.IsNaN(availableSize.Width))
      {
        availableWidth = 100000000;
      }

      double width = Double.MaxValue;
      double height = 0;
      if (!Double.IsInfinity(availableSize.Height) && !Double.IsNaN(availableSize.Height))
      {
        height = availableSize.Height;
      }

      Dictionary<UIElement, double> sizeCache = new Dictionary<UIElement,double>();
      while (width > availableWidth && Children.Count > 0)
      {
        UIElement largestElement = null;
        double largestWidth = 0;
        double secondLargestWidth = 0;
        width = 0;
        foreach (UIElement element in Children)
        {
          RibbonGroup group = element as RibbonGroup;
          if (!sizeCache.ContainsKey(element))
          {
            if (group != null)
            {
              group.IsReadyToCollapse = false;
              group.SetIsExpanded(true);
            }
            sizeCache[element] = Double.PositiveInfinity;
          }
          element.Measure(new Size(sizeCache[element], availableSize.Height));
          if (group != null && group.IsReadyToCollapse)
          {
            group.SetIsExpanded(false);
            element.Measure(new Size(sizeCache[element], availableSize.Height));
          }
          height = Math.Max(height, element.DesiredSize.Height);
          width += element.DesiredSize.Width;
          sizeCache[element] = Math.Min(element.DesiredSize.Width, sizeCache[element]);

          if (sizeCache[element] > largestWidth && (group == null || group.IsExpanded))
          {
            secondLargestWidth = largestWidth;
            largestWidth = sizeCache[element];
            largestElement = element;
          }
          else if (sizeCache[element] > secondLargestWidth && (group == null || group.IsExpanded))
          {
            secondLargestWidth = sizeCache[element];
          }
        }

        if (width > availableWidth && largestElement != null)
        {
          double deltaFromAvailable = Math.Ceiling(width - availableWidth);
          double deltaFromSecondLargest = Math.Ceiling(largestWidth - secondLargestWidth);
          double delta = deltaFromSecondLargest == 0 ? deltaFromAvailable : Math.Min(deltaFromAvailable, deltaFromSecondLargest);
          RibbonGroup ribbonGroup = largestElement as RibbonGroup;
          if (ribbonGroup != null)
          {
            if (!ribbonGroup.CanShrink)
            {
              ribbonGroup.SetIsExpanded(false);
            }
            else
            {
              sizeCache[largestElement] = Math.Max(0, sizeCache[largestElement] - delta);
            }
          }
          else
          {
            sizeCache[largestElement] = Math.Max(0, sizeCache[largestElement] - delta);
          }
        }
        else
        {
          break;
        }
      }
      return new Size(width, height);
    }

    /// <summary>
    /// Arranges the items within this <see cref="RibbonTabPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="RibbonTabPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      double x = 0;
      foreach (UIElement element in Children)
      {
        element.Arrange(new Rect(x, 0, element.DesiredSize.Width, finalSize.Height));
        x += element.DesiredSize.Width;
      }

      return finalSize;
    }
  }
}
