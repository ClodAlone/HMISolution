using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A control for presenting the column headers of a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridHeaderRowPresenter : ItemsControl
  {
    private ScrollViewer _scrollViewer;
    private double _previousHorizontalOffset;
    private double _previousExtentWidth;
    private DataGridColumn _sortedColumn;
    private DataGrid _dataGrid;

    static DataGridHeaderRowPresenter()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridHeaderRowPresenter), new FrameworkPropertyMetadata(typeof(DataGridHeaderRowPresenter)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    public DataGridHeaderRowPresenter()
    {
      AllowDrop = true;
      Loaded += new RoutedEventHandler(DataGridHeaderRowPresenter_Loaded);
      PaddingColumnHeader = new DataGridColumnHeader() { Role = DataGridColumnHeaderRole.Padding };
      PaddingColumnHeader.Style = ItemContainerStyle;
      SizeChanged += new SizeChangedEventHandler(DataGridHeaderRowPresenter_SizeChanged);

      if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        MinHeight = 22;
        /*IList columns = new List<DataGridColumn>();
        columns.Add(new DataGridColumn() { Header = "Column 1" });
        columns.Add(new DataGridColumn() { Header = "Column 2" });
        ItemsSource = columns;*/
      }
    }

    private void DataGridHeaderRowPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      DataGrid dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (dataGrid != null && dataGrid.DataGridPanel != null && PaddingColumnHeader != null)
      {
        //PaddingColumnHeader.Width = Math.Max(0, ActualWidth - dataGrid.DataGridPanel.TotalColumnsWidth);
      }
    }

    private void DataGridHeaderRowPresenter_Loaded(object sender, RoutedEventArgs e)
    {
      _scrollViewer = VisualTreeUtils.FindContaining<ScrollViewer>(this);
      if (_scrollViewer != null)
      {
        _scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
      }

      _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
      DataGridRowPanel panel = VisualTreeUtils.GetChild<DataGridRowPanel>(this);
      if (panel != null)
      {
        if (_dataGrid != null)
        {
          _dataGrid.FrozenColumnCountChanged += new EventHandler(DataGrid_FrozenColumnCountChanged);
        }
        if (_dataGrid != null && _dataGrid.DataGridPanel != null)
        {
          panel.InvalidateMeasure();
          _dataGrid.DataGridPanel.TotalColumnsWidthChanged += new EventHandler(DataGridPanel_TotalColumnsWidthChanged);
          if (PaddingColumnHeader != null)
          {
            //PaddingColumnHeader.Width = Math.Max(0, ActualWidth - _dataGrid.DataGridPanel.TotalColumnsWidth);
          }
        }
      }
    }

    private void DataGridPanel_TotalColumnsWidthChanged(object sender, EventArgs e)
    {
      //DataGridPanel panel = sender as DataGridPanel;
      if (PaddingColumnHeader != null)
      {
        //PaddingColumnHeader.Width = Math.Max(0, ActualWidth - panel.TotalColumnsWidth);
      }
    }

    /// <summary>
    /// Called when the item container style changes.
    /// </summary>
    /// <param name="oldItemContainerStyle">The old item container style.</param>
    /// <param name="newItemContainerStyle">The new item container style.</param>
    protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
    {
      base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);
      if (PaddingColumnHeader != null)
      {
        PaddingColumnHeader.Style = ItemContainerStyle;
      }
    }

    /// <summary>
    /// Called when a drag operation enters this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);

      if (_dataGrid != null)
      {
        WeakReference reference = e.Data.GetData(DataFormats.Serializable, false) as WeakReference;
        if (reference != null && reference.IsAlive)
        {
          DataGridColumnHeader source = reference.Target as DataGridColumnHeader;
          if (source != null)
          {
            if (source.DataGrid != _dataGrid)
            {
              e.Handled = true;
              e.Effects = DragDropEffects.None;
              return;
            }
            DataGridColumn sourceColumn = source.Column;
            // TODO: we should be able to store the effective column index within the column header during the virtualization process (simalar to what we are doing for rows)
            // This will allow us to remove the IndexOf call here:
            _sourceIndex = _dataGrid.EffectiveColumns.IndexOf(sourceColumn);
          }
        }
      }
    }

    /// <summary>
    /// Called when a drag operation is moving over this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);

      WeakReference reference = e.Data.GetData(DataFormats.Serializable, false) as WeakReference;
      if (reference != null && reference.IsAlive)
      {
        DataGridColumnHeader source = reference.Target as DataGridColumnHeader;
        if (source != null && source.DataGrid != _dataGrid)
        {
          e.Effects = DragDropEffects.None;
          IsRequestingColumnDropPosition = false;
          e.Handled = true;
          return;
        }
      }

      Point position = e.GetPosition(this);
      if (_dataGrid != null)
      {
        position = new Point(position.X - _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count, position.Y);
      }
      if (_dataGrid != null && _dataGrid.AllowColumnReorder)
      {
        IsRequestingColumnDropPosition = true;
        object originalSource = e.OriginalSource;
        DependencyObject element = originalSource as DependencyObject;
        if (element != null)
        {
          DataGridColumnHeader header = VisualTreeUtils.FindAncestor<DataGridColumnHeader>(element);
          if (header != null)
          {
            DataGridColumn dragOverColumn = header.Column;
            if (_dataGrid != null)
            {
              // TODO: again, we should be able to work around calling IndexOf here:
              _dropIndex = _dataGrid.EffectiveColumns.IndexOf(dragOverColumn);
              Point mouseFromHeader = e.GetPosition(header);

              // Check to see if the mouse is closest to the left side or the right side of the column header.
              // This will determine where to place the indicator, and what the actual drop index is.
              if (mouseFromHeader.X < header.ActualWidth / 2.0 || header.Role == DataGridColumnHeaderRole.Padding)
              {
                ColumnDropRequestPosition = position.X - mouseFromHeader.X;
              }
              else
              {
                ColumnDropRequestPosition = position.X - mouseFromHeader.X + header.ActualWidth;
                _dropIndex++;
              }

              // If the drop target is the padding column, then the drop index is the last index of the effective columns collection.
              if (header.Role == DataGridColumnHeaderRole.Padding)
              {
                _dropIndex = _dataGrid.EffectiveColumns.Count;
              }

              // Due to the behavior of the ObservableCollection.Move method, we need to decrement the actual drop index if it is greater than the source index.
              if (_sourceIndex < _dropIndex)
              {
                _dropIndex--;
              }
              // If the drop index is the same as the source index, then no change will be made if the item is dropped, so no need to display the drop indicator.
              // (This may be an option later on)
              if (_dropIndex == _sourceIndex)
              {
                IsRequestingColumnDropPosition = false;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Called when a drag operation leaves this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragLeave(DragEventArgs e)
    {
      base.OnDragLeave(e);
      // Even if the mouse is still within the bounds of the header row presenter, this method can still be called if the mouse moves over a column resizer.
      // Although the OnDragOver method is called instantly after this method in this situation, the drop indicator still flickers due to setting the IsRequestingDropPosition.
      // So this logic checks to see if the drag action really has left the header row presenter.
      Point position = MouseUtils.GetPosition(this);
      if (position.X < 1 || position.Y < 1 || position.X > ActualWidth - 5 || position.Y > ActualHeight - 1)
      {
        IsRequestingColumnDropPosition = false;
      }
    }

    private int _sourceIndex;
    private int _dropIndex;

    /// <summary>
    /// Called when a drag operation is dropped over this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDrop(DragEventArgs e)
    {
      base.OnDrop(e);

      // Dragging a column header from the grouping panel back to the header row presenter will ungroup that column:
      WeakReference reference = e.Data.GetData(DataFormats.Serializable, false) as WeakReference;
      if (reference != null && reference.IsAlive)
      {
        DataGridColumnHeader header = reference.Target as DataGridColumnHeader;
        if (header != null && _dataGrid != null && header.Role == DataGridColumnHeaderRole.Grouping)
        {
          _dataGrid.GroupedColumns.Remove(header.Column);
        }
      }

      // Reordering columns:
      if (IsRequestingColumnDropPosition)
      {
        if (_dataGrid != null)
        {
          _dataGrid.EffectiveColumns.Move(_sourceIndex, _dropIndex);
        }
      }
      IsRequestingColumnDropPosition = false;
    }

    #region ColumnDropRequestPosition Property

    /// <summary>
    /// Gets the position of the column drop request from the left edge of this <see cref="DataGridHeaderRowPresenter"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnDropRequestPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ColumnDropRequestPosition
    {
      get { return (double)GetValue(ColumnDropRequestPositionProperty); }
      private set { SetValue(DataGridHeaderRowPresenter.ColumnDropRequestPositionPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey ColumnDropRequestPositionPropertyKey =
        DependencyProperty.RegisterReadOnly("ColumnDropRequestPosition", typeof(double), typeof(DataGridHeaderRowPresenter), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="ColumnDropRequestPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnDropRequestPositionProperty =
        ColumnDropRequestPositionPropertyKey.DependencyProperty;

    #endregion // ColumnDropRequestPosition Property

    #region IsRequestingColumnDropPosition Property

    /// <summary>
    /// Gets whether a column is requesting to be dropped onto this <see cref="DataGridHeaderRowPresenter"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsRequestingColumnDropPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsRequestingColumnDropPosition
    {
      get { return (bool)GetValue(IsRequestingColumnDropPositionProperty); }
      private set { SetValue(DataGridHeaderRowPresenter.IsRequestingColumnDropPositionPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey IsRequestingColumnDropPositionPropertyKey =
        DependencyProperty.RegisterReadOnly("IsRequestingColumnDropPosition", typeof(bool), typeof(DataGridHeaderRowPresenter), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsRequestingColumnDropPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty IsRequestingColumnDropPositionProperty =
        IsRequestingColumnDropPositionPropertyKey.DependencyProperty;

    #endregion // IsRequestingColumnDropPosition Property

    #region PaddingColumnHeader Property

    /// <summary>
    /// Gets the <see cref="DataGridColumnHeader"/> used as padding at the end of all columns.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PaddingColumnHeaderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridColumnHeader PaddingColumnHeader
    {
      get { return (DataGridColumnHeader)GetValue(PaddingColumnHeaderProperty); }
      private set { SetValue(PaddingColumnHeaderPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey PaddingColumnHeaderPropertyKey =
        DependencyProperty.RegisterReadOnly("PaddingColumnHeader", typeof(DataGridColumnHeader), typeof(DataGridHeaderRowPresenter), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="PaddingColumnHeader"/> property.
    /// </summary>
    public static readonly DependencyProperty PaddingColumnHeaderProperty =
        PaddingColumnHeaderPropertyKey.DependencyProperty;

    #endregion // PaddingColumnHeader Property

    private void DataGrid_FrozenColumnCountChanged(object sender, EventArgs e)
    {
      DataGridRowPanel panel = VisualTreeUtils.GetChild<DataGridRowPanel>(this);
      if (panel != null)
      {
        panel.InvalidateMeasure();
      }
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (_scrollViewer.HorizontalOffset != _previousHorizontalOffset || _scrollViewer.ExtentWidth != _previousExtentWidth)
      {
        DataGridRowPanel panel = VisualTreeUtils.GetChild<DataGridRowPanel>(this);
        if (panel != null)
        {
          DataGrid dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
          if (dataGrid != null && dataGrid.DataGridPanel != null)
          {
            panel.InvalidateMeasure();
          }
        }
        _previousHorizontalOffset = _scrollViewer.HorizontalOffset;
        _previousExtentWidth = _scrollViewer.ExtentWidth;
      }
    }

    internal DependencyObject GetContainer()
    {
      return GetContainerForItemOverride();
    }

    internal void PrepareContainer(DependencyObject element, object item)
    {
      PrepareContainerForItemOverride(element, item);
    }

    internal void ClearContainer(DependencyObject element, object item)
    {
      ClearContainerForItemOverride(element, item);
    }

    /// <summary>
    /// Returns whether or not the given object can be used as its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item can be used as its own container. False otherwise.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is DataGridColumnHeader;
    }

    /// <summary>
    /// Gets a container that can be displayed in this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <returns>A new <see cref="DataGridColumnHeader"/> control.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new DataGridColumnHeader();
    }

    /// <summary>
    /// Prepares the given container to display the given item.
    /// </summary>
    /// <param name="element">The <see cref="DataGridColumnHeader"/> to prepare.</param>
    /// <param name="item">The <see cref="DataGridColumn"/> to be displayed by the given header.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridColumn column = item as DataGridColumn;
      DataGridColumnHeader header = element as DataGridColumnHeader;
      header.Style = ItemContainerStyle;
      header.ContentTemplate = ItemTemplate;
      Binding headerBinding = new Binding("Header") { Source = column };
      BindingOperations.SetBinding(header, DataGridColumnHeader.ContentProperty, headerBinding);
      header.Column = column;

      column.SortDirectionChanged += new EventHandler(DataGridColumn_SortDirectionChanged);
      
      //BindingOperations.SetBinding(header, DataGridCellContainer.WidthProperty, new Binding { Source = column, Path = new PropertyPath("Width"), Mode = BindingMode.TwoWay });
    }

    /// <summary>
    /// Clears the given container.
    /// </summary>
    /// <param name="element">The <see cref="DataGridColumnHeader"/> to clear.</param>
    /// <param name="item">The <see cref="DataGridColumn"/> being displayed by the given header.</param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridColumn column = item as DataGridColumn;
      DataGridColumnHeader header = element as DataGridColumnHeader;
      column.SortDirectionChanged -= new EventHandler(DataGridColumn_SortDirectionChanged);
      BindingOperations.ClearBinding(header, DataGridCellContainer.WidthProperty);
    }

    private void DataGridColumn_SortDirectionChanged(object sender, EventArgs e)
    {
      DataGridColumn column = sender as DataGridColumn;
      if (column.SortDirection != SortDirection.None)
      {
        if (_sortedColumn != null && _sortedColumn != column)
        {
          DataGridColumn previousColumn = _sortedColumn;
          _sortedColumn = column;
          if (_dataGrid != null)
          {
            _dataGrid.IgnoreSortDirectionChanged = true;
          }
          previousColumn.SortDirection = SortDirection.None;
          if (_dataGrid != null)
          {
            _dataGrid.IgnoreSortDirectionChanged = false;
          }
        }
        else if (_sortedColumn == null)
        {
          _sortedColumn = column;
        }
      }
      else if (_sortedColumn == column)
      {
        _sortedColumn = null;
      }
    }
  }
}
