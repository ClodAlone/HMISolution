using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Implements a selectable item in a <see cref="MulticolumnTreeView"/> control.
  /// </summary>
  public class MulticolumnTreeViewItem : TreeViewItem
  {
    static MulticolumnTreeViewItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MulticolumnTreeViewItem),
        new FrameworkPropertyMetadata(typeof(MulticolumnTreeViewItem)));
    }

    /// <summary>
    /// Gets the level of the <see cref="MulticolumnTreeViewItem"/> in the hierarchy.
    /// </summary>
    public int Level
    {
      get
      {
        if (_level == -1)
        {
          MulticolumnTreeViewItem parent = ItemsControlFromItemContainer(this) as MulticolumnTreeViewItem;
          _level = (parent != null) ? parent.Level + 1 : 0;
        }
        return _level;
      }
    }

    /// <summary>
    /// Creates a new <see cref="MulticolumnTreeViewItem"/> to display a child item.
    /// </summary>
    /// <returns>A new TreeListViewItem.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new MulticolumnTreeViewItem();
    }

    /// <summary>
    /// Determines whether an object is a <see cref="MulticolumnTreeViewItem"/>.
    /// </summary>
    /// <param name="item">The object to evaluate.</param>
    /// <returns>true if item is a MulticolumnTreeViewItem; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is MulticolumnTreeViewItem;
    }

    private int _level = -1;
  }
}
