using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A panel that provides layout logic for items within a <see cref="PopupMenu"/>.
  /// </summary>
  public class PopupMenuPanel : Panel
  {
    private readonly Dictionary<Gallery, double> _galleryHeights = new Dictionary<Gallery,double>();

    /// <summary>
    /// Measures the elements within this <see cref="PopupMenuPanel"/> and returns the desirted size.
    /// </summary>
    /// <param name="availableSize">The available size of this <see cref="PopupMenuPanel"/>.</param>
    /// <returns>The desired size of this <see cref="PopupMenuPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      double width = 0;
      double height = 0;
      IList<Gallery> galleries = new List<Gallery>();
      foreach (UIElement element in Children)
      {
        Gallery gallery = element as Gallery;
        if (gallery != null)
        {
          galleries.Add(gallery);
        }
        else
        {
          element.Measure(availableSize);
          height += element.DesiredSize.Height;
          width = Math.Max(width, element.DesiredSize.Width);
        }
      }
      MinWidth = width;
      double finalWidth = width;
      _galleryHeights.Clear();
      foreach (Gallery gallery in galleries)
      {
        //double galleryWidth = (availableSize.Width == Double.MaxValue || width == 0) ? availableSize.Width : Double.IsInfinity(availableSize.Width) ? width + gallery.ItemWidth : availableSize.Width;
        double galleryWidth = Double.IsInfinity(availableSize.Width) ? Math.Max(gallery.ActualItemWidth * gallery.MinColumnCount, (width == 0 ? 150 : width)) + 24 : availableSize.Width;
        double galleryHeight = Double.IsInfinity(availableSize.Height) ? 500 : Math.Max(0, availableSize.Height - height);
        gallery.Measure(new Size(galleryWidth, galleryHeight));
        if (galleryHeight == 500)
        {
          galleryHeight = Math.Min(galleryHeight, gallery.DesiredSize.Height);
        }
        _galleryHeights[gallery] = galleryHeight;
        height += gallery.DesiredSize.Height;
        finalWidth = Math.Max(finalWidth, gallery.DesiredSize.Width);
      }
      return new Size(finalWidth, height);
    }

    /// <summary>
    /// Arranges the elements within this <see cref="PopupMenuPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="PopupMenuPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      double y = 0;
      PopupResizer resizer = null;
      foreach (UIElement element in Children)
      {
        PopupResizer popupResizer = element as PopupResizer;
        if (popupResizer != null)
        {
          resizer = popupResizer;
          if (resizer.IsTop)
          {
            resizer.Arrange(new Rect(0, 0, finalSize.Width, resizer.DesiredSize.Height));
            y += resizer.DesiredSize.Height;
          }
          break;
        }
      }
      foreach (UIElement element in Children)
      {
        if(element != resizer)
        {
          double height = element.DesiredSize.Height;
          Gallery gallery = element as Gallery;
          if (gallery != null)
          {
            double h;
            _galleryHeights.TryGetValue(gallery, out h);
            if (h < finalSize.Height + 20) // TODO: this is uber kludgy. Need to find a better way to work out how much unused space is available for gallery control(s) to fill.
            {
              height = h;
            }
          }
          element.Arrange(new Rect(0, y, finalSize.Width, height));
          y += height;
        }
      }
      if (resizer != null && !resizer.IsTop)
      {
        resizer.Arrange(new Rect(0, finalSize.Height - resizer.DesiredSize.Height, finalSize.Width, resizer.DesiredSize.Height));
      }
      return finalSize;
    }
  }
}
