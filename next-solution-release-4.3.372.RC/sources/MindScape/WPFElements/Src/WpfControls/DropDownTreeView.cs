using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A TreeView that can be dropped down like a combo box.
  /// </summary>
  public class DropDownTreeView : Control
  {
    static DropDownTreeView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownTreeView), new FrameworkPropertyMetadata(typeof(DropDownTreeView)));
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SelectedItem
    {
      get { return GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof(object), typeof(DropDownTreeView),
      new FrameworkPropertyMetadata(OnSelectedItemChanged));

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownTreeView)d).OnSelectedItemChanged();
    }

    private void OnSelectedItemChanged()
    {
      if (_treeView != null)
      {
        var container = FindContainer(_treeView, SelectedItem) as TreeViewItem;
        if (container != null)
        {
          container.IsSelected = true;
        }
      }
    }

    /// <summary>
    /// Gets or sets the items source.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemsSourceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DropDownTreeView));

    /// <summary>
    /// Gets or sets the item template.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ItemTemplate
    {
      get { return (DataTemplate)GetValue(ItemTemplateProperty); }
      set { SetValue(ItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
      DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DropDownTreeView));


    /// <summary>
    /// Gets or sets the item template for the selected item (when shown collapsed).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate SelectedItemTemplate
    {
      get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
      set { SetValue(SelectedItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemTemplateProperty =
      DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(DropDownTreeView));

    /// <summary>
    /// Gets or sets a value which indicates whether users may select only leaf nodes,
    /// or can select any nodes.  The default is false (select any nodes).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectLeafOnlyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool SelectLeafOnly
    {
      get { return (bool)GetValue(SelectLeafOnlyProperty); }
      set { SetValue(SelectLeafOnlyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectLeafOnly"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectLeafOnlyProperty =
      DependencyProperty.Register("SelectLeafOnly", typeof(bool), typeof(DropDownTreeView),
      new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets the value of the NotifySelectionChange attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the NotifySelectionChange attached property.</returns>
    public static DropDownTreeView GetNotifySelectionChange(DependencyObject obj)
    {
      return (DropDownTreeView)obj.GetValue(NotifySelectionChangeProperty);
    }

    /// <summary>
    /// Sets the value of the NotifySelectionChange attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetNotifySelectionChange(DependencyObject obj, DropDownTreeView value)
    {
      obj.SetValue(NotifySelectionChangeProperty, value);
    }

    /// <summary>
    /// Identifies the NotifySelectionChange attached property.
    /// </summary>
    public static readonly DependencyProperty NotifySelectionChangeProperty =
      DependencyProperty.RegisterAttached("NotifySelectionChange", typeof(DropDownTreeView), typeof(DropDownTreeView),
      new FrameworkPropertyMetadata(OnNotifySelectionChangeChanged));

    private static void OnNotifySelectionChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeView source = (TreeView)d;
      DropDownTreeView target = (DropDownTreeView)(e.NewValue);
      if (source != null && target != null)
      {
        source.SelectedItemChanged += target.OnNotifierSelectedItemChanged;
        target._treeView = source;
      }
    }

    private TreeView _treeView;

    private void OnNotifierSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      TreeView source = (TreeView)sender;
      object selection = e.NewValue;

      if (SelectLeafOnly)
      {
        var container = FindContainer(source, selection);
        if (container != null && container.Items.Count > 0)
        {
          return;
        }
      }

      SelectedItem = selection;
      source.RaiseEvent(new RoutedEventArgs(DropDownEditBox.DropDownCloseRequestedEvent));
    }

    private static ItemsControl FindContainer(ItemsControl startingFrom, object item)
    {
      var icg = startingFrom.ItemContainerGenerator;
      if (icg == null)
      {
        return null;
      }

      var container = icg.ContainerFromItem(item) as ItemsControl;
      if (container != null)
      {
        return container;
      }

      foreach (var child in startingFrom.Items)
      {
        ItemsControl hic = icg.ContainerFromItem(child) as ItemsControl;
        if (hic != null)
        {
          var findresult = FindContainer(hic, item);
          if (findresult != null)
          {
            return findresult;
          }
        }
      }

      return null;
    }
  }
}
