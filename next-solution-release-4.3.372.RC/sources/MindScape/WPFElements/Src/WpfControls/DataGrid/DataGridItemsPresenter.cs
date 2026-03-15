using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  //TODO: is this class still needed? The DataGridPanel seems to bypass this and acess the DataGrid directly.
  /// <summary>
  /// A control for displaying the contents of a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridItemsPresenter : ItemsControl
  {
    private DataGrid _dataGrid;

    static DataGridItemsPresenter()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridItemsPresenter), new FrameworkPropertyMetadata(typeof(DataGridItemsPresenter)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridItemsPresenter"/>.
    /// </summary>
    public DataGridItemsPresenter()
    {
      Loaded += new RoutedEventHandler(DataGridItemsPresenter_Loaded);
    }

    private void DataGridItemsPresenter_Loaded(object sender, RoutedEventArgs e)
    {
      _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
    }

    /// <summary>
    /// Gets a container for displaying an item in the <see cref="DataGrid"/>.
    /// </summary>
    /// <returns>A new <see cref="DataGridRow"/>.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      _dataGrid = GetDataGrid();
      if (_dataGrid != null)
      {
        //return _dataGrid.GetContainer();
      }
      return base.GetContainerForItemOverride();
    }

    /// <summary>
    /// Prepares the given container for displaying the given item.
    /// </summary>
    /// <param name="element">The <see cref="DataGridRow"/> to prepare.</param>
    /// <param name="item">The item to be displayed by the given row.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      _dataGrid = GetDataGrid();
      if (_dataGrid != null)
      {
        _dataGrid.PrepareContainer(element, item);
        return;
      }
      base.PrepareContainerForItemOverride(element, item);
    }

    /// <summary>
    /// Returns whether or not the given item can be used as its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the given item can be used as its own container. False otherwise.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      _dataGrid = GetDataGrid();
      if (_dataGrid != null)
      {
        return _dataGrid.CheckContainer(item);
      }
      return base.IsItemItsOwnContainerOverride(item);
    }

    private DataGrid GetDataGrid()
    {
      if (_dataGrid == null)
      {
        _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
      }
      return _dataGrid;
    }
  }
}
