using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a group of items within a <see cref="Gallery"/> control.
  /// </summary>
  public class GalleryGroup : HeaderedItemsControl
  {
    private Gallery _gallery;

    static GalleryGroup()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroup),
        new FrameworkPropertyMetadata(typeof(GalleryGroup)));
    }

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
    protected override System.Windows.DependencyObject GetContainerForItemOverride()
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
      base.PrepareContainerForItemOverride(element, item);
      Gallery gallery = Gallery;
      if (gallery != null && gallery.SelectedValue != null && (gallery.SelectedValue == item || gallery.SelectedValue.Equals(item)))
      {
        GalleryItem galleryItem = element as GalleryItem;
        if (galleryItem != null)
        {
          galleryItem.IsSelected = true;
        }
      }

      if (gallery != null)
      {
        GalleryItem galleryItem = element as GalleryItem;
        gallery.UpdateActualItemSize(galleryItem);
      }
    }

    private Gallery Gallery
    {
      get
      {
        if (_gallery == null)
        {
          _gallery = VisualTreeUtils.FindAncestor<Gallery>(this);
        }
        return _gallery;
      }
    }
  }
}
