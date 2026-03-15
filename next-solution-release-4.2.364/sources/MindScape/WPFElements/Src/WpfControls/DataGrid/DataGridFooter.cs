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
  /// A footer row for a <see cref="DataGrid"/> that can be used to display collection aggregates.
  /// </summary>
  public class DataGridFooter : ItemsControl
  {
    static DataGridFooter()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridFooter), new FrameworkPropertyMetadata(typeof(DataGridFooter)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridFooter"/> class.
    /// </summary>
    public DataGridFooter()
    {
      Loaded += new RoutedEventHandler(DataGridFooter_Loaded);
      Unloaded += new RoutedEventHandler(DataGridFooter_Unloaded);
    }

    private void DataGridFooter_Loaded(object sender, RoutedEventArgs e)
    {
      if (Columns != null)
      {
        Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
        Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      }
    }

    private void DataGridFooter_Unloaded(object sender, RoutedEventArgs e)
    {
      if (Columns != null)
      {
        Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      }
    }

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      ObservableCollection<DataGridFooterCell> groupHeaderCells = ItemsSource as ObservableCollection<DataGridFooterCell>;
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

    private void UpdateItemsSource()
    {
      if (Columns != null)
      {
        ObservableCollection<DataGridFooterCell> groupHeaderCells = new ObservableCollection<DataGridFooterCell>();
        foreach (DataGridColumn column in Columns)
        {
          groupHeaderCells.Add(BuildItem(column));
        }
        ItemsSource = groupHeaderCells;
      }
    }

    private DataGridFooterCell BuildItem(DataGridColumn column)
    {
      return new DataGridFooterCell(column);
    }

    #region Columns Property

    // TODO: Remove this property and use an attached property to sync the EffectiveColumns property to the ItemsSource property.
    //       While synching, the footer cells can be generated.
    //       This same technique could be used for group header cells too.

    /// <summary>
    /// Gets or sets the collection of columns to generate the items source.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<DataGridColumn> Columns
    {
      get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Columns"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnsProperty =
      DependencyProperty.Register("Columns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGridFooter),
      new FrameworkPropertyMetadata(OnColumnsChanged));

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridFooter)d).OnColumnsChanged(e);
    }

    #endregion // Columns Property

    private void OnColumnsChanged(DependencyPropertyChangedEventArgs e)
    {
      ObservableCollection<DataGridColumn> columns = e.OldValue as ObservableCollection<DataGridColumn>;
      if (columns != null)
      {
        columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      }
      if (Columns != null)
      {
        Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      }
      UpdateItemsSource();
    }
  }
}
