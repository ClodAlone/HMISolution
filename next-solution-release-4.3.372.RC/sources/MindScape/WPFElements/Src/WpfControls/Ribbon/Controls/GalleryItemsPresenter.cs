using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Displays items within a <see cref="Gallery"/> control.
  /// </summary>
  public class GalleryItemsPresenter : ItemsControl
  {
    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is GalleryItem;
    }

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>The element that is used to display the given item.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new GalleryItem();
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">The element used to display the specified item.</param>
    /// <param name="item">The item to display.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      Visual visual = item as Visual;
      Gallery gallery = VisualTreeUtils.FindAncestor<Gallery>(this);
      GalleryItem galleryItem = element as GalleryItem;
      if (visual != null)
      {
        Rectangle rectangle = new Rectangle();
        VisualBrush brush = new VisualBrush(visual);
        rectangle.Fill = brush;
        FrameworkElement e = visual as FrameworkElement;
        if (e != null)
        {
          if (gallery != null)
          {
            if (Double.IsNaN(gallery.ItemWidth) || Double.IsNaN(gallery.ItemHeight))
            {
              e.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            }
            else
            {
              e.Measure(new Size(gallery.ItemWidth, gallery.ItemHeight));
            }
            rectangle.Width = e.DesiredSize.Width;
            rectangle.Height = e.DesiredSize.Height;
          }
        }
        galleryItem.Content = rectangle;
        galleryItem.DataContext = item;
      }
      else
      {
        base.PrepareContainerForItemOverride(element, item);
      }

      if (gallery != null && galleryItem != null)
      {
        gallery.UpdateActualItemSize(galleryItem);
        if (gallery.SelectedValue != null && (gallery.SelectedValue == item || gallery.SelectedValue.Equals(item)))
        {
          galleryItem.IsSelected = true;
        }
      }
    }
  }
}
