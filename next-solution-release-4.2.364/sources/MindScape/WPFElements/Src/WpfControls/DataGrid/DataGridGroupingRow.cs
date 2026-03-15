using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A collapsable row in a data grid that appears above a group of rows.
  /// </summary>
  public class DataGridGroupingRow : ItemsControl, IDataGridRow
  {
    // TODO: need to adjust DataGrid slightly so that the Effective columns property never changes.
    private ObservableCollection<DataGridColumn> _columns;

    static DataGridGroupingRow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridGroupingRow), new FrameworkPropertyMetadata(typeof(DataGridGroupingRow)));
    }

    internal DataGridGroupingRow(DataGridGroup group, ObservableCollection<DataGridColumn> columns)
    {
      Group = group;
      _columns = columns;

      UpdateItemsSource();

      Loaded += new RoutedEventHandler(DataGridGroupingRow_Loaded);
      Unloaded += new RoutedEventHandler(DataGridGroupingRow_Unloaded);
    }

    private void DataGridGroupingRow_Loaded(object sender, RoutedEventArgs e)
    {
      _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
    }

    private void DataGridGroupingRow_Unloaded(object sender, RoutedEventArgs e)
    {
      _columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
    }

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      ObservableCollection<GroupHeaderCell> groupHeaderCells = ItemsSource as ObservableCollection<GroupHeaderCell>;
      if (groupHeaderCells == null)
      {
        UpdateItemsSource();
        return;
      }
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          int i = e.NewStartingIndex;
          foreach (DataGridColumn column in e.NewItems)
          {
            groupHeaderCells.Insert(i, BuildItem(column));
            i++;
          }
          break;
        case NotifyCollectionChangedAction.Move:
          groupHeaderCells.Move(e.OldStartingIndex, e.NewStartingIndex);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (DataGridColumn column in e.OldItems)
          {
            groupHeaderCells.RemoveAt(e.OldStartingIndex);
          }
          break;
        case NotifyCollectionChangedAction.Replace:
          // TODO
          break;
        case NotifyCollectionChangedAction.Reset:
          UpdateItemsSource();
          break;
      }
    }

    internal void UpdateItemsSource()
    {
      ObservableCollection<GroupHeaderCell> groupHeaderCells = new ObservableCollection<GroupHeaderCell>();
      foreach (DataGridColumn column in _columns)
      {
        groupHeaderCells.Add(BuildItem(column));
      }
      ItemsSource = groupHeaderCells;
    }

    private GroupHeaderCell BuildItem(DataGridColumn column)
    {
      AggregateBase aggregate = null;
      if (column.GroupAggregate != null)
      {
        aggregate = Activator.CreateInstance(column.GroupAggregate.GetType()) as AggregateBase;
        aggregate.PropertyInfo = column.PropertyInfo;
        aggregate.Calculate(Group.Children);
      }
      return new GroupHeaderCell(Group, column, aggregate);
    }

    /// <summary>
    /// Gets the <see cref="DataGridGroup"/> of this <see cref="DataGridGroupingRow"/>.
    /// </summary>
    public DataGridGroup Group { get; private set; }

    #region HeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display the header of this <see cref="DataGridGroupingRow"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate HeaderTemplate
    {
      get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
      set { SetValue(HeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
      DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGridGroupingRow));

    #endregion // HeaderTemplate Property

    #region HeaderTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to display the header of this <see cref="DataGridGroupingRow"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector HeaderTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
      set { SetValue(HeaderTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
      DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridGroupingRow));

    #endregion // HeaderTemplateSelector Property

    #region Index Property

    /// <summary>
    /// Gets the display index of this <see cref="DataGridGroupingRow"/> on the current page.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IndexProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Index
    {
      get { return (int)GetValue(IndexProperty); }
    }

    internal static readonly DependencyPropertyKey IndexPropertyKey =
        DependencyProperty.RegisterReadOnly("Index", typeof(int), typeof(DataGridGroupingRow), new UIPropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="Index"/> property.
    /// </summary>
    public static readonly DependencyProperty IndexProperty = IndexPropertyKey.DependencyProperty;

    #endregion // Index Property
  }
}
