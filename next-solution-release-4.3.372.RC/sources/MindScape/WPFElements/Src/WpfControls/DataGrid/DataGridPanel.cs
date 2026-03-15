using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Collections;
using System.Windows.Threading;
using System.Diagnostics;
using System.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides row virtualization for a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridPanel : VirtualizingPanel, IScrollInfo
  {
    // Scroll fields:
    private bool _canHorizontallyScroll;
    private bool _canVerticallyScroll;
    private Vector _scrollOffset = new Vector(0.0, 0.0);
    private Vector _internalOffset;
    private Size _extent;
    private Size _viewport;
    private Size _maxDesiredSize;
    private ScrollViewer _scrollOwner;

    // DataGrid fields:
    private DataGrid _dataGrid;
    private double _totalColumnsWidth;

    // Virtualization fields:
    private readonly Dictionary<object, UIElement> _containerMap = new Dictionary<object, UIElement>();
    private HashSet<object> _previousItems = new HashSet<object>();
    private readonly Queue<UIElement> _recycleBin = new Queue<UIElement>();
    private IList<UIElement> _realizedChildren;
    private IList<UIElement> _backLog;

    private bool _unloaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridPanel"/> class.
    /// </summary>
    public DataGridPanel()
    {
      Loaded += new RoutedEventHandler(DataGridPanel_Loaded);
      Unloaded += new RoutedEventHandler(DataGridPanel_Unloaded);
    }

    private void DataGridPanel_Unloaded(object sender, RoutedEventArgs e)
    {
      if (_itemsSource != null)
      {
        _itemsSource.CollectionUpdated -= new EventHandler(ItemsSource_CollectionUpdated);
      }
      if (_dataGrid != null)
      {
        if (_dataGrid.EffectiveColumns != null)
        {
          _dataGrid.EffectiveColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(EffectiveColumns_CollectionChanged);
        }
        if (_dataGrid.GroupedColumns != null)
        {
          _dataGrid.GroupedColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
        }
      }
      _unloaded = true;
    }

    private void DataGridPanel_Loaded(object sender, RoutedEventArgs e)
    {
      _requiresMeasure = true;
      if (_unloaded)
      {
        if (_itemsSource != null)
        {
          _itemsSource.CollectionUpdated += new EventHandler(ItemsSource_CollectionUpdated);
        }
        if (_dataGrid != null)
        {
          if (_dataGrid.EffectiveColumns != null)
          {
            _dataGrid.EffectiveColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(EffectiveColumns_CollectionChanged);
          }
          if (_dataGrid.GroupedColumns != null)
          {
            _dataGrid.GroupedColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
          }
        }
        _unloaded = false;
      }

      ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
      if (itemsOwner != null)
      {
        itemsOwner.SizeChanged += new SizeChangedEventHandler(DataGridPanel_SizeChanged);
      }
      if (DataGrid != null)
      {
        DataGrid.DataGridPanel = this;
      }
    }

    private void DataGridPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      SetHorizontalOffset(HorizontalOffset);
      UpdateFirstVisibleColumnIndex();
      _requiresMeasure = true;
      InvalidateMeasure();
    }

    private bool _requiresMeasure = true;

    internal void InvalidateRequiresMeasure()
    {
      _requiresMeasure = true;
      InvalidateMeasure();
    }

    /// <summary>
    /// Measures the elements within this <see cref="DataGridPanel"/> and returns the desired size.
    /// </summary>
    /// <param name="availableSize">The available size for this <see cref="DataGridPanel"/>.</param>
    /// <returns>The desired size of this <see cref="DataGridPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      //Debug.WriteLine("Measuring Grid");

      if (!_requiresMeasure)
      {
        return DesiredSize;
      }

      // Reset the AutoWidth properties and determine if an auto-sizing column exists:
      if (DataGrid != null) // NOTE: do not put a HasAutoColumn check on this condition because this is where we calculate the HasAutoColumn value.
      {
        bool hasAutoColumn = false;
        bool hasStarSizingColumn = false;
        foreach (DataGridColumn column in DataGrid.EffectiveColumns)
        {
          column.AutoWidth = column.MinWidth;
          hasAutoColumn = hasAutoColumn || (column.Width.IsAuto && column.IsAutoWidthDirty);
          hasStarSizingColumn = hasStarSizingColumn || column.IsStarSizing;
        }
        DataGrid.HasAutoColumn = hasAutoColumn;
        DataGrid.HasStarSizingColumn = hasStarSizingColumn;
        if (hasAutoColumn)
        {
          // Force the header presenter to be measured first:
          DataGrid.MeasureHeaderRowPresenter();
        }
      }

      double width = Double.IsInfinity(availableSize.Width) ? 500 : availableSize.Width;
      double height = Double.IsInfinity(availableSize.Height) ? 500 : availableSize.Height;
      availableSize = new Size(width, height);

      _realizedChildren = new List<UIElement>();
      _backLog = new List<UIElement>();
      
      if (DataGrid == null || !IsLoaded)
      {
        _requiresMeasure = false;
        return availableSize;
      }

      //ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
      DataGridItemsSource items = GetItemsSource();
      if (items == null)
      {
        _requiresMeasure = false;
        return availableSize;
      }

      Size stackDesiredSize = new Size();
      Size layoutSlotSize = new Size(availableSize.Width, Double.PositiveInfinity);
      int lastViewport = -1;
      int itemCount = items.CountOnCurrentPage;
      double remainingSpace = availableSize.Height;
      int firstViewport = ComputeIndexOfFirstVisibleItem(itemCount);

      HashSet<object> nextCache = new HashSet<object>();
      bool fillRemainingSpace = true;

      // Frozen rows
      double frozenRowEdge = 0;
      int currentIndex = 0;

      while (currentIndex < Math.Min(FrozenRowCount, itemCount))
      {
        object item = items.GetItemOnCurrentPageAt(currentIndex);
        UIElement child = GetContainer(item, currentIndex);

        _previousItems.Remove(item);
        nextCache.Add(item);

        DataGridRow row = child as DataGridRow;
        if (row != null)
        {
          row.SetValue(DataGridRow.IndexPropertyKey, currentIndex);
          if (_dataGrid != null)
          {
            row.Level = _dataGrid.GroupedColumns.Count;
          }
          if (row.Panel != null)
          {
            row.Panel.InvalidateMeasure();
          }
          row.ItemWrapper = items != null ? items.GetHierarchyWrapper(item) : null;
        }
        DataGridGroupingRow groupingRow = child as DataGridGroupingRow;
        if (groupingRow != null)
        {
          groupingRow.SetValue(DataGridGroupingRow.IndexPropertyKey, currentIndex);
        }

        _realizedChildren.Add(child);

        if (row != null && row.Panel != null)
        {
          row.Panel.ForceMeasure(layoutSlotSize);
        }
        else
        {
          child.Measure(layoutSlotSize);
        }

        Size desiredSize = child.DesiredSize;

        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, desiredSize.Width);
        stackDesiredSize.Height += Math.Round(desiredSize.Height);
        frozenRowEdge += desiredSize.Height;

        if (ScrollOwnerExists && lastViewport == -1)
        {
          remainingSpace -= Math.Round(desiredSize.Height);
          if (NumericalUtils.LessThanOrClose(remainingSpace, 0.0))
          {
            lastViewport = currentIndex;
          }
        }
        if (stackDesiredSize.Height > availableSize.Height)
        {
          fillRemainingSpace = false;
          break;
        }
        currentIndex++;
      }
      // End frozen rows

      currentIndex = Math.Max(0, firstViewport + FrozenRowCount);
      while (currentIndex < itemCount)
      {
        object item = items.GetItemOnCurrentPageAt(currentIndex);
        UIElement child = GetContainer(item, currentIndex);

        _previousItems.Remove(item);
        nextCache.Add(item);

        DataGridRow row = child as DataGridRow;
        if (row != null)
        {
          row.SetValue(DataGridRow.IndexPropertyKey, currentIndex);
          if (_dataGrid != null)
          {
            row.Level = _dataGrid.GroupedColumns.Count;
          }
          if (row.Panel != null)
          {
            row.Panel.InvalidateMeasure();
          }
          row.ItemWrapper = items != null ? items.GetHierarchyWrapper(item) : null;
        }
        DataGridGroupingRow groupingRow = child as DataGridGroupingRow;
        if (groupingRow != null)
        {
          groupingRow.SetValue(DataGridGroupingRow.IndexPropertyKey, currentIndex);
        }

        _realizedChildren.Add(child);

        if (row != null && row.Panel != null)
        {
          row.Panel.ForceMeasure(layoutSlotSize);
        }
        else
        {
          child.Measure(layoutSlotSize);
        }

        Size desiredSize = child.DesiredSize;
        //desiredSize.Height = 24;

        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, desiredSize.Width);
        stackDesiredSize.Height += Math.Round(desiredSize.Height);

        if (ScrollOwnerExists && lastViewport == -1 && currentIndex >= firstViewport)
        {
          remainingSpace -= Math.Round(desiredSize.Height);
          if (NumericalUtils.LessThanOrClose(remainingSpace, 0.0))
          {
            lastViewport = currentIndex;
          }
        }
        if (stackDesiredSize.Height > availableSize.Height)
        {
          fillRemainingSpace = false;
          break;
        }
        currentIndex++;
      }

      // Update the ActualWidth of the columns that are auto-sizing:
      double totalColumnsWidth = 0;
      if (DataGrid != null && DataGrid.HasAutoColumn)
      {
        if (DataGrid != null)
        {
          totalColumnsWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
          foreach (DataGridColumn column in DataGrid.EffectiveColumns)
          {
            if (column.Width.IsAuto && column.IsAutoWidthDirty)
            {
              double oldActualWidth = column.ActualWidth;
              column.SetActualWidth(column.AutoWidth == 0 ? 100 : column.AutoWidth);
              if (oldActualWidth != column.ActualWidth && _dataGrid != null)
              {
                _requiresMeasure = true;
                InvalidateMeasure();
              }
            }
            totalColumnsWidth += column.ActualWidth;
          }
        }
        /*if (ScrollOwnerExists)
        {
          SetAndVerifyScrollingData(_viewport, new Size(totalColumnWidth, _extent.Height), _scrollOffset);
          SetHorizontalOffset(HorizontalOffset);
          OnScrollChange();
        }*/
      }
      if (totalColumnsWidth != 0)
      {
        _totalColumnsWidth = totalColumnsWidth;
      }
      _totalColumnsWidth = Math.Max(0, _totalColumnsWidth);
      // End auto sizing logic

      // TODO: calculate star sizing first? Although auto size columns needs to be done first.
      double viewportWidth = availableSize.Width - (DataGrid == null ? 0 : DataGrid.RowHeaderWidth);
      CalculateStarSizingWidths(viewportWidth);

      if (ScrollOwnerExists)
      {
        Size extent = new Size(_totalColumnsWidth, itemCount);
        if (fillRemainingSpace)
        {
          FillRemainingSpace(ref firstViewport, ref remainingSpace, ref stackDesiredSize, layoutSlotSize);
        }
        stackDesiredSize = UpdateLogicalScrollData(stackDesiredSize, availableSize, remainingSpace, extent, firstViewport, lastViewport, itemCount);
      }

      foreach (object o in _previousItems)
      {
        UIElement element;
        _containerMap.TryGetValue(o, out element);
        if (element != null)
        {
          element.Visibility = Visibility.Hidden;
          ClearContainer(element, o);
          if (element is DataGridRow)
          {
            _recycleBin.Enqueue(element);
          }
        }
        _containerMap.Remove(o);
      }
      _previousItems = nextCache;
      
      if (DataGrid != null)
      {
        DataGrid.FrozenRowEdge = frozenRowEdge;
      }

      _requiresMeasure = false;
      return stackDesiredSize;
    }

    private int FrozenRowCount
    {
      get
      {
        return _dataGrid == null ? 0 : _dataGrid.FrozenRowCount;
      }
    }

    /// <summary>
    /// Arranges all the elements within this <see cref="DataGridPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="DataGridPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      //Debug.WriteLine("Arranging");
      Dispatcher.BeginInvoke(new Action(EnsureFrozenEdge), DispatcherPriority.ApplicationIdle);

      Rect finalRect = new Rect(finalSize);
      finalRect.Y = 0;
      double height = 0.0;
      for (int i = _backLog.Count - 1; i >= 0; i--)
      {
        UIElement container = _backLog[i];
        Size desiredSize = container.DesiredSize;
        finalRect.Y += height;
        height = Math.Round(desiredSize.Height);
        finalRect.Height = height;
        finalRect.Width = Math.Max(finalSize.Width, desiredSize.Width);
        if (container != null)
        {
          container.Arrange(finalRect);
        }
      }
      IList<UIElement> items = _realizedChildren;
      for (int i = 0; i < items.Count; i++)
      {
        UIElement container = items[i];
        Size desiredSize = container.DesiredSize;
        finalRect.Y += height;
        height = Math.Round(desiredSize.Height);
        finalRect.Height = height;
        finalRect.Width = Math.Max(finalSize.Width, desiredSize.Width);
        if (container != null)
        {
          container.Arrange(finalRect);
        }
      }
      if (_failedToBringIntoView != null)
      {
        BringCellIntoView(_failedToBringIntoView);
        _failedToBringIntoView = null;
      }

      OnScrollChange();

      if (DataGrid.AutoColumnWidthBehavior == AutoColumnWidthBehavior.OneTime)
      {
        foreach (DataGridColumn column in DataGrid.EffectiveColumns)
        {
          column.IsAutoWidthDirty = false;
        }
      }

      return finalSize;
    }

    private void CalculateStarSizingWidths(double viewportWidth)
    {
      double totalFinalizedWidth = 0;
      if (_dataGrid != null)
      {
        totalFinalizedWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
      }
      double totalFillWeight = 0;
      List<DataGridColumn> starSizingColumns = new List<DataGridColumn>();
      foreach (DataGridColumn column in DataGrid.EffectiveColumns)
      {
        if (!column.IsStarSizing || !column.IsVisible)
        {
          totalFinalizedWidth += column.ActualWidth;
        }
        else
        {
          totalFillWeight += column.FillWeight;
          starSizingColumns.Add(column);
        }
      }
      totalFillWeight = Math.Round(totalFillWeight);

      /*DataGridColumn resizingColumn = DataGridColumnHeader.ResizingColumn;
      if (resizingColumn != null)
      {
        starSizingColumns.Remove(resizingColumn);
        //totalFinalizedWidth += resizingColumn.ActualWidth;
      }*/

      starSizingColumns.Sort(new DataGridColumn_SortByFillWidth());
      /*double groupingIndent = 0;
      if (_dataGrid != null)
      {
        groupingIndent = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
      }*/
      double availableWidth = viewportWidth - totalFinalizedWidth; // -groupingIndent;
      double pixelsPerUnit = availableWidth / totalFillWeight;
      foreach (DataGridColumn column in starSizingColumns)
      {
        //double ratio = totalFillWeight <= 0 ? 0 : column.FillWeight / totalFillWeight;
        //double actualWidth = Math.Max(column.MinWidth, availableWidth * ratio);
        double actualWidth = Math.Max(column.MinWidth, column.FillWeight * pixelsPerUnit);
        // TODO: respect the MinWidth:
        /*if (actualWidth < column.MinWidth)
        {
          double diff = column.MinWidth - actualWidth;
          availableWidth -= diff;
          actualWidth = column.MinWidth;
        }*/
        column.SetActualWidth(actualWidth);
        totalFinalizedWidth += actualWidth;
      }
      if (Math.Abs(totalFinalizedWidth - viewportWidth) < 2)
      {
        _totalColumnsWidth = viewportWidth;
      }

      if (DataGrid.EffectiveColumns.Count > 0)
      {
        DataGridColumn lastColumn = DataGrid.EffectiveColumns[DataGrid.EffectiveColumns.Count - 1];
        if (lastColumn.IsStarSizing)
        {
          lastColumn.AllowResize = false;
        }
      }

      if (DataGrid != null)
      {
        DataGrid.InvalidateHeaderRowMeasure();
        //DataGrid.MeasureHeaderRowPresenter();
      }
    }

    private class DataGridColumn_SortByFillWidth : IComparer<DataGridColumn>
    {
      public int Compare(DataGridColumn column1, DataGridColumn column2)
      {
        return (int)((column1.FillWeight - column2.FillWeight) * 100);
      }
    }

    // The frozen column feature does not behave nicely with auto-size columns and a few other features, so we use a dispatcher to force the FrozenEdge to be updated.
    // TODO: it would be great if we could remove this terrible work around.
    private void EnsureFrozenEdge()
    {
      if (DataGrid != null)
      {
        DataGrid.FrozenEdge = FrozenEdge + 0.0001;
        Dispatcher.BeginInvoke(new Action(RestoreFrozenEdge), DispatcherPriority.ApplicationIdle);
      }
    }

    private void RestoreFrozenEdge()
    {
      if (DataGrid != null)
      {
        DataGrid.FrozenEdge = FrozenEdge;
      }
    }

    /// <summary>
    /// Called when the visual children collection is cleared.
    /// </summary>
    protected override void OnClearChildren()
    {
      base.OnClearChildren();
      _containerMap.Clear();
      _previousItems.Clear();
      _realizedChildren.Clear();
      _recycleBin.Clear();
      _backLog.Clear();
    }

    private DataGridItemsSource _itemsSource;

    private DataGridItemsSource GetItemsSource()
    {
      ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
      DataGridItemsSource source = itemsOwner.ItemsSource as DataGridItemsSource;
      if (source != _itemsSource && _itemsSource != null)
      {
        _itemsSource.CollectionUpdated -= new EventHandler(ItemsSource_CollectionUpdated);
        _itemsSource = null;
      }
      if (_itemsSource == null && source != null)
      {
        _itemsSource = source;
        _itemsSource.CollectionUpdated += new EventHandler(ItemsSource_CollectionUpdated);
      }
      if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && source == null)
      {
        IList data = new List<object>();
        data.Add(new SampleObject() { Property1 = "Value 1", Property2 = "Value 2" });
        data.Add(new SampleObject() { Property1 = "Value 1", Property2 = "Value 2" });
        source = new DataGridItemsSource(data);
      }
      return source;
    }

    private void ItemsSource_CollectionUpdated(object sender, EventArgs e)
    {
      _requiresMeasure = true;
      if (VerticalOffset <= ExtentHeight - ViewportHeight && VerticalOffset >= ExtentHeight - ViewportHeight - 1)
      {
        foreach (UIElement element in Children)
        {
          element.Visibility = Visibility.Hidden;
        }
      }
    }

    internal IList<UIElement> RealizedChildren
    {
      get { return _realizedChildren; }
    }

    private void FillRemainingSpace(ref int firstViewport, ref double remainingSpace, ref Size stackDesiredSize, Size layoutSlotSize)
    {
      ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
      DataGridItemsSource items = itemsOwner.ItemsSource as DataGridItemsSource;
      if (items == null)
      {
        return;
      }
      int index = firstViewport + FrozenRowCount;
      while (index > FrozenRowCount)
      {
        object item = items.GetItemOnCurrentPageAt(index - 1);
        _previousItems.Remove(item);
        UIElement container = GetContainer(item, index);

        DataGridRow row = container as DataGridRow;
        if (row != null)
        {
          row.SetValue(DataGridRow.IndexPropertyKey, index - 1);
        }
        DataGridGroupingRow groupingRow = container as DataGridGroupingRow;
        if (groupingRow != null)
        {
          groupingRow.SetValue(DataGridGroupingRow.IndexPropertyKey, index - 1);
        }

        container.Measure(layoutSlotSize);
        double currentRemainingSpace = remainingSpace;
        Size desiredSize = container.DesiredSize;
        currentRemainingSpace -= desiredSize.Height;
        if (NumericalUtils.LessThan(currentRemainingSpace, 0.0))
        {
          container.Visibility = Visibility.Hidden;
          break;
        }
        _backLog.Add(container);
        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, desiredSize.Width);
        stackDesiredSize.Height += desiredSize.Height;
        firstViewport--;
        index--;
        remainingSpace = currentRemainingSpace;
      }
    }

    private Size UpdateLogicalScrollData(Size stackDesiredSize, Size constraint, double remainingSpace, Size extent, int firstViewport, int lastViewport, int itemCount)
    {
      Size viewport = constraint;
      if (DataGrid != null && DataGrid.ShowRowHeaders)
      {
        viewport.Width -= DataGrid.RowHeaderWidth;
      }
      if (lastViewport == -1)
      {
        lastViewport = itemCount - 1;
      }
      int range = lastViewport - firstViewport;
      if (range == 0 || NumericalUtils.GreaterThanOrClose(remainingSpace, 0.0))
      {
        range++;
      }
      viewport.Height = Math.Max(0, range);

      Vector offset = _internalOffset;
      offset.Y = firstViewport;
      offset.X = Math.Max(0.0, Math.Min(offset.X, extent.Width - viewport.Width));
      if (itemCount > range && !Double.IsPositiveInfinity(constraint.Height))
      {
        stackDesiredSize.Height = constraint.Height;
      }
      stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
      stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
      _maxDesiredSize.Width = Math.Max(stackDesiredSize.Width, _maxDesiredSize.Width);
      _maxDesiredSize.Height = Math.Max(stackDesiredSize.Height, _maxDesiredSize.Height);
      stackDesiredSize = _maxDesiredSize;
      SetAndVerifyScrollingData(viewport, extent, offset);
      return stackDesiredSize;
    }

    private void SetAndVerifyScrollingData(Size viewport, Size extent, Vector offset)
    {
      bool viewportChanged = !NumericalUtils.AreClose(viewport, _viewport);
      bool extentChanged = !NumericalUtils.AreClose(extent, _extent);
      bool offsetChanged = !NumericalUtils.AreClose(offset, _scrollOffset);
      _internalOffset = offset;
      if (viewportChanged || extentChanged || offsetChanged)
      {
        _viewport = viewport;
        _extent = extent;
        _scrollOffset = offset;
        //Debug.WriteLine("Extent: " + ExtentWidth + ", Viewport: " + ViewportWidth);
        OnScrollChange();
      }
    }

    private UIElement GetContainer(object item, int index)
    {
      UIElement child;
      _containerMap.TryGetValue(item, out child);
      if (child == null)
      {
        /*if (DataGrid != null && DataGrid.CheckContainer(item))
        {
          child = item as UIElement;
          // TODO: This try catch block is used because after a DataGridGroupingRow has been added, scrolling down and then back up can cause it to be added again.
          //       It would be better to have some kind of recycling cache for grouping rows to avoid this issue.
          try
          {
            AddInternalChild(child);
          }
          catch (Exception) { }
        }
        else*/
        {
          if (!(item is DataGridGroup) && _recycleBin.Count != 0)
          {
            child = _recycleBin.Dequeue();
            DataGridRow row = child as DataGridRow;
            if (row != null)
            {
              row.RequiresRePrepare = true;
            }
          }
          else
          {
            child = GetContainerFromDataGrid(item) as UIElement;
            AddInternalChild(child);
          }

          PrepareContainer(child, item);
        }
        _containerMap[item] = child;
      }

      DataGridRow dataGridRow = child as DataGridRow;
      if (dataGridRow != null)
      {
        dataGridRow.SetValue(DataGridRow.IndexPropertyKey, index);
      }
      DataGridGroupingRow groupingRow = child as DataGridGroupingRow;
      if (groupingRow != null)
      {
        groupingRow.SetValue(DataGridGroupingRow.IndexPropertyKey, index);
      }
      if (DataGrid != null)
      {
        DataGrid.PrepareContainerBackground(child);
      }

      child.Visibility = Visibility.Visible;
      return child;
    }

    private DependencyObject GetContainerFromDataGrid(object item)
    {
      if (DataGrid != null)
      {
        return DataGrid.GetContainer(item);
      }
      return null;
    }

    private void PrepareContainer(DependencyObject element, object item)
    {
      if (DataGrid != null)
      {
        DataGrid.PrepareContainer(element, item);
      }
    }

    private void ClearContainer(DependencyObject element, object item)
    {
      if (DataGrid != null)
      {
        DataGrid.ClearContainer(element, item);
        // Special logic for DataTable/DataView support: Binding exceptions occur on DataRowView when rows are removed and then recycled.
        // So when a row is removed here, we clear the Content property of all the cells to avoid this issue.
        DataGridRow row = element as DataGridRow;
        if (row != null && row.Content is DataRowView)
        {
          row.ClearCells();
        }
      }
    }

    private int ComputeIndexOfFirstVisibleItem(int itemsCount)
    {
      if (ScrollOwnerExists)
      {
        return Math.Max(0, NumericalUtils.CoerceIndexToInteger(_internalOffset.Y, itemsCount));
      }
      return 0;
    }

    private void OnScrollChange()
    {
      if (ScrollOwner != null)
      {
        ScrollOwner.InvalidateScrollInfo();
      }
    }

    internal int FirstVisibleColumnIndex { get; set; }
    internal double FirstVisibleColumnOffset { get; set; }
    internal double FrozenEdge { get; set; }

    internal DataGridRow GetRow(object item)
    {
      UIElement element;
      _containerMap.TryGetValue(item, out element);
      return element as DataGridRow;
    }

    internal DataGridRow GetDataGridRow(int index)
    {
      if (_realizedChildren.Count > 0)
      {
        DataGridRow row = null;
        IDataGridRow firstRow = _realizedChildren[0] as IDataGridRow;
        int firstIndex = firstRow.Index;
        int rowIndex = index - firstIndex;
        if (rowIndex >= 0 && rowIndex < _realizedChildren.Count)
        {
          row = _realizedChildren[rowIndex] as DataGridRow;
        }
        if (row != null && row.Index == index)
        {
          return row;
        }
        // TODO: keep an eye on this backlog code. It has not been fully tested.
        // Check the backlog:
        if (_backLog.Count > 0)
        {
          firstRow = _backLog[_backLog.Count - 1] as IDataGridRow;
          firstIndex = firstRow.Index;
          rowIndex = _backLog.Count - (index - firstIndex) - 1;
          if (rowIndex >= 0 && rowIndex < _backLog.Count)
          {
            row = _backLog[rowIndex] as DataGridRow;
          }
          if (row != null && row.Index == index)
          {
            return row;
          }
          foreach (IDataGridRow r in _backLog)
          {
            if (r.Index == index)
            {
              return r as DataGridRow;
            }
          }
        }

        foreach (IDataGridRow r in _realizedChildren)
        {
          if (r.Index == index)
          {
            return r as DataGridRow;
          }
        }
      }
      return null;
    }

    internal DataGridRow GetDataGridRow(object rowContent)
    {
      foreach (UIElement element in _realizedChildren)
      {
        DataGridRow row = element as DataGridRow;
        if (row != null && row.Content == rowContent)
        {
          return row;
        }
      }
      return null;
    }

    internal void ForceReRender()
    {
      foreach (UIElement element in _realizedChildren)
      {
        DataGridRow row = element as DataGridRow;
        if (row != null)
        {
          row.RequiresRePrepare = true;
        }
      }
      _requiresMeasure = true;
      InvalidateMeasure();
    }

    internal void BringCellIntoView(DataGridCell cellModel)
    {
      if (DataGrid != null)
      {
        // Horizontal offset:
        double x = 0;
        int count = 0;
        foreach (DataGridColumn column in DataGrid.EffectiveColumns)
        {
          count++;
          if (column == cellModel.Column)
          {
            break;
          }
          x += column.ActualWidth;
        }
        if (count > DataGrid.FrozenColumnCount)
        {
          if (x < HorizontalOffset + FrozenEdge || cellModel.Column.ActualWidth > ViewportWidth - FrozenEdge)
          {
            SetHorizontalOffset(x - FrozenEdge);
          }
          else if (x + cellModel.Column.ActualWidth > HorizontalOffset + ViewportWidth)
          {
            SetHorizontalOffset(x + cellModel.Column.ActualWidth - ViewportWidth);
          }
        }
        // Vertical offset:
        double y = cellModel.RowIndex;
        // TODO: this doesn't consider the hight of each row. This will be a bit tricky.
        if (y < VerticalOffset + FrozenRowCount && y >= FrozenRowCount)
        {
          SetVerticalOffset(y - FrozenRowCount);
        }
        else if (y > VerticalOffset + ViewportHeight - 1)
        {
          if (y >= ExtentHeight)
          {
            // If this condition is reached, it means we are trying to bring a cell into view that hasn't been rendered yet.
            // Thus we can not sucessfully bring it into view at this point.
            // So lets store the cell that failed to be brought into view, and try once more in the arrange pass.
            _failedToBringIntoView = cellModel;
          }
          SetVerticalOffset(y - ViewportHeight + 1);
        }
      }
    }

    internal void BringRowIntoView(object rowContent)
    {
      if (DataGrid != null)
      {
        // Vertical offset:
        ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
        if (itemsOwner != null)
        {
          DataGridItemsSource source = itemsOwner.ItemsSource as DataGridItemsSource;
          if (source != null)
          {
            double y = source.IndexOfOnCurrentPage(rowContent);
            if (y >= 0 && y < source.CountOnCurrentPage)
            {
              // TODO: this doesn't consider the hight of each row. This will be a bit tricky.
              if (y < VerticalOffset + FrozenRowCount && y >= FrozenRowCount)
              {
                SetVerticalOffset(y - FrozenRowCount);
              }
              else if (y > VerticalOffset + ViewportHeight - 1)
              {
                /*if (y >= ExtentHeight && DataGrid != null && DataGrid.EffectiveColumns.Count > 0)
                {
                  _failedToBringIntoView = new DataGridCell(rowContent, DataGrid.EffectiveColumns[0]);
                }*/
                _extent.Height = source.CountOnCurrentPage;
                int estimatedViewportHeight = (int)Math.Min(ViewportHeight, Math.Min((ActualHeight / 25), _extent.Height)); // TODO: this is a problem: if the viewport isn't full of items, how to know how the viewport height will be affected by the new items?
                SetVerticalOffset(y - estimatedViewportHeight + 1);
              }
            }
          }
        }
      }
    }

    private DataGridCell _failedToBringIntoView;

    internal void UpdateRowHeaderWidth(double width)
    {
      _viewport.Width = ActualWidth - width;
      SetHorizontalOffset(HorizontalOffset);
    }

    internal void UpdateGroupAggregates()
    {
      foreach (UIElement element in _realizedChildren)
      {
        DataGridGroupingRow groupingRow = element as DataGridGroupingRow;
        if (groupingRow != null)
        {
          groupingRow.UpdateItemsSource();
        }
      }
    }

    #region DataGrid property

    private DataGrid DataGrid
    {
      get
      {
        if (_dataGrid == null)
        {
          DataGrid = VisualTreeUtils.FindAncestor<DataGrid>(this);
        }
        return _dataGrid;
      }
      set
      {
        if (_dataGrid != value)
        {
          if (_dataGrid != null)
          {
            _dataGrid.EffectiveColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(EffectiveColumns_CollectionChanged);
            _dataGrid.GroupedColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
            foreach (DataGridColumn column in _dataGrid.EffectiveColumns)
            {
              column.WidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_WidthChanged);
              column.ActualWidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_ActualWidthChanged);
            }
          }
          _dataGrid = value;
          if (_dataGrid != null)
          {
            _dataGrid.DataGridPanel = this;
            _dataGrid.EffectiveColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(EffectiveColumns_CollectionChanged);
            _dataGrid.GroupedColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
            _totalColumnsWidth = 0;
            if (_dataGrid != null)
            {
              _totalColumnsWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
            }
            foreach (DataGridColumn column in _dataGrid.EffectiveColumns)
            {
              column.WidthChanged += new EventHandler<ColumnWidthChangedEventArgs>(Column_WidthChanged);
              column.ActualWidthChanged += new EventHandler<ColumnWidthChangedEventArgs>(Column_ActualWidthChanged);
              _totalColumnsWidth += column.ActualWidth;
            }
            _extent.Width = _totalColumnsWidth;
            OnScrollChange();
            OnTotalColumnsWidthChanged();
            UpdateFirstVisibleColumnIndex();
          }
        }
      }
    }

    private void GroupedColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _totalColumnsWidth = 0;
      if (_dataGrid != null)
      {
        _totalColumnsWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
      }
      foreach (DataGridColumn column in DataGrid.EffectiveColumns)
      {
        _totalColumnsWidth += column.ActualWidth;
      }
      _extent.Width = _totalColumnsWidth;
      UpdateFirstVisibleColumnIndex();
      OnTotalColumnsWidthChanged();
      SetHorizontalOffset(HorizontalOffset);
      OnScrollChange();
    }

    private void EffectiveColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateFirstVisibleColumnIndex();
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        _totalColumnsWidth = 0;
        if (_dataGrid != null)
        {
          _totalColumnsWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
        }
      }
      if (e.OldItems != null)
      {
        foreach (DataGridColumn column in e.OldItems)
        {
          column.WidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_WidthChanged);
          column.ActualWidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_ActualWidthChanged);
          _totalColumnsWidth -= column.ActualWidth;
        }
      }
      if (e.NewItems != null)
      {
        foreach (DataGridColumn column in e.NewItems)
        {
          column.WidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_WidthChanged);
          column.WidthChanged += new EventHandler<ColumnWidthChangedEventArgs>(Column_WidthChanged);
          column.ActualWidthChanged -= new EventHandler<ColumnWidthChangedEventArgs>(Column_ActualWidthChanged);
          column.ActualWidthChanged += new EventHandler<ColumnWidthChangedEventArgs>(Column_ActualWidthChanged);
          _totalColumnsWidth += column.ActualWidth;
        }
      }
      _totalColumnsWidth = Math.Max(0, _totalColumnsWidth);
      _extent.Width = _totalColumnsWidth;
      UpdateFirstVisibleColumnIndex();
      OnTotalColumnsWidthChanged();
      SetHorizontalOffset(HorizontalOffset);
      OnScrollChange();
    }

    private void Column_ActualWidthChanged(object sender, ColumnWidthChangedEventArgs e)
    {
      //_totalColumnsWidth -= e.OldWidth;
      //_totalColumnsWidth += e.NewWidth;
      //_totalColumnsWidth = Math.Max(0, _totalColumnsWidth);
      _totalColumnsWidth = 0;
      if (_dataGrid != null)
      {
        _totalColumnsWidth = _dataGrid.GroupingIndent * _dataGrid.GroupedColumns.Count;
      }
      foreach (DataGridColumn column in DataGrid.EffectiveColumns)
      {
        _totalColumnsWidth += column.ActualWidth;
      }
      _totalColumnsWidth = Math.Max(0, _totalColumnsWidth);
      _extent.Width = _totalColumnsWidth;
      UpdateFirstVisibleColumnIndex();
      OnTotalColumnsWidthChanged();
      SetHorizontalOffset(HorizontalOffset);
      OnScrollChange();
      _requiresMeasure = true;
      //InvalidateMeasure();
    }

    private void Column_WidthChanged(object sender, ColumnWidthChangedEventArgs e)
    {
      if (DataGrid != null)
      {
        //double totalColumnWidth = 0;
        bool hasAutoColumn = false;
        bool hasStarSizingColumn = false;
        foreach (DataGridColumn column in DataGrid.EffectiveColumns)
        {
          //totalColumnWidth += column.ActualWidth;
          hasAutoColumn = hasAutoColumn || (column.Width.IsAuto && column.IsAutoWidthDirty);
          hasStarSizingColumn = hasStarSizingColumn || column.IsStarSizing;
        }
        DataGrid.HasAutoColumn = hasAutoColumn;
        DataGrid.HasStarSizingColumn = hasStarSizingColumn;
        //_totalColumnsWidth = Math.Max(0, totalColumnWidth);
        //_extent.Width = _totalColumnsWidth;
        UpdateFirstVisibleColumnIndex();
        //OnTotalColumnsWidthChanged();
      }
      SetHorizontalOffset(HorizontalOffset);
      OnScrollChange();
      _requiresMeasure = true;
      InvalidateMeasure();
    }

    internal double TotalColumnsWidth
    {
      get { return _totalColumnsWidth; }
    }

    internal event EventHandler TotalColumnsWidthChanged;

    private void OnTotalColumnsWidthChanged()
    {
      EventHandler handler = TotalColumnsWidthChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // DataGrid property

    #region IScrollInfo members

    /// <summary>
    /// Scrolls up by one line.
    /// </summary>
    public virtual void LineUp()
    {
      SetVerticalOffset(VerticalOffset - 1.0);
    }

    /// <summary>
    /// Scrolls down by one line.
    /// </summary>
    public virtual void LineDown()
    {
      SetVerticalOffset(VerticalOffset + 1.0);
    }

    /// <summary>
    /// Scrolls left by 16 pixels.
    /// </summary>
    public virtual void LineLeft()
    {
      SetHorizontalOffset(HorizontalOffset - 16.0);
    }

    /// <summary>
    /// Scroll right by 16 pixels.
    /// </summary>
    public virtual void LineRight()
    {
      SetHorizontalOffset(HorizontalOffset + 16.0);
    }

    /// <summary>
    /// Scrolls up by the hight of the viewport.
    /// </summary>
    public virtual void PageUp()
    {
      SetVerticalOffset(VerticalOffset - ViewportHeight);
    }

    /// <summary>
    /// Scrolls down by the height of the viewport.
    /// </summary>
    public virtual void PageDown()
    {
      SetVerticalOffset(VerticalOffset + ViewportHeight);
    }

    /// <summary>
    /// Scrolls left by the width of the viewport.
    /// </summary>
    public virtual void PageLeft()
    {
      SetHorizontalOffset(HorizontalOffset - ViewportWidth);
    }

    /// <summary>
    /// Scrolls right by the width of the viewport.
    /// </summary>
    public virtual void PageRight()
    {
      SetHorizontalOffset(HorizontalOffset + ViewportWidth);
    }

    /// <summary>
    /// Scrolls up.
    /// </summary>
    public virtual void MouseWheelUp()
    {
      SetVerticalOffset(VerticalOffset - SystemParameters.WheelScrollLines);
    }

    /// <summary>
    /// Scrolls down.
    /// </summary>
    public virtual void MouseWheelDown()
    {
      SetVerticalOffset(VerticalOffset + SystemParameters.WheelScrollLines);
    }

    /// <summary>
    /// Scrolls left.
    /// </summary>
    public virtual void MouseWheelLeft()
    {
      SetHorizontalOffset(HorizontalOffset - 48);
    }

    /// <summary>
    /// Scrolls right.
    /// </summary>
    public virtual void MouseWheelRight()
    {
      SetHorizontalOffset(HorizontalOffset + 48);
    }

    /// <summary>
    /// Sets the horizontal scroll offset.
    /// </summary>
    /// <param name="offset">The desired horizontal scroll offset.</param>
    public void SetHorizontalOffset(double offset)
    {
      if (Double.IsNaN(offset))
      {
        throw new ArgumentOutOfRangeException("offset", "Horizontal offset can not be NaN.");
      }
      offset = Math.Max(0.0, Math.Min(offset, ExtentWidth - ViewportWidth));
      if (!NumericalUtils.AreClose(offset, _internalOffset.X))
      {
        _requiresMeasure = true;
        _internalOffset.X = offset;
        UpdateFirstVisibleColumnIndex();
        InvalidateMeasure();
      }
    }

    /// <summary>
    /// Sets the vertical scroll offset.
    /// </summary>
    /// <param name="offset">The desired vertical scroll offset.</param>
    public void SetVerticalOffset(double offset)
    {
      if (Double.IsNaN(offset))
      {
        throw new ArgumentOutOfRangeException("offset", "Vertical offset can not be NaN.");
      }
      offset = Math.Max(0.0, Math.Min(offset, ExtentHeight - ViewportHeight));
      if (!NumericalUtils.AreClose(offset, _internalOffset.Y))
      {
        _requiresMeasure = true;
        _internalOffset.Y = offset;
        InvalidateMeasure();
      }
    }

    internal void UpdateFirstVisibleColumnIndex()
    {
      if (DataGrid != null)
      {
        double totalWidth = 0;
        FrozenEdge = 0;
        int count = 0;

        foreach (DataGridColumn column in DataGrid.EffectiveColumns)
        {
          if (count < DataGrid.FrozenColumnCount)
          {
            FrozenEdge += column.ActualWidth;
            totalWidth += column.ActualWidth;
          }
          else
          {
            // Column virtualization is disabled when there exists an auto-sizing column:
            FirstVisibleColumnOffset = DataGrid.HasAutoColumn || DataGrid.HasStarSizingColumn ? FrozenEdge - _internalOffset.X : totalWidth - _internalOffset.X;
            totalWidth += column.ActualWidth;
            if (totalWidth > _internalOffset.X + FrozenEdge)
            {
              // Column virtualization is disabled when there exists an auto-sizing column:
              FirstVisibleColumnIndex = DataGrid.HasAutoColumn || DataGrid.HasStarSizingColumn ? DataGrid.FrozenColumnCount : count;
              break;
            }
          }
          count++;
        }
        DataGrid.FrozenEdge = FrozenEdge;
      }
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <param name="visual">Not used.</param>
    /// <param name="rectangle">Not used.</param>
    /// <returns>A new <see cref="Rect"/>.</returns>
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
      return new Rect();
    }

    /// <summary>
    /// Gets or sets whether horizontal scrolling is supported.
    /// </summary>
    public bool CanHorizontallyScroll
    {
      get { return _canHorizontallyScroll; }
      set
      {
        if (_canHorizontallyScroll != value)
        {
          _canHorizontallyScroll = value;
          _requiresMeasure = true;
          InvalidateMeasure();
        }
      }
    }

    /// <summary>
    /// Gets or sets whether vertical scrolling is supported.
    /// </summary>
    public bool CanVerticallyScroll
    {
      get { return _canVerticallyScroll; }
      set
      {
        if (_canVerticallyScroll != value)
        {
          _canVerticallyScroll = value;
          _requiresMeasure = true;
          InvalidateMeasure();
        }
      }
    }

    /// <summary>
    /// Gets the width of the scroll extent.
    /// </summary>
    public double ExtentWidth { get { return _extent.Width; } }

    /// <summary>
    /// Gets the height of the scroll extent.
    /// </summary>
    public double ExtentHeight { get { return _extent.Height; } }

    /// <summary>
    /// Gets the width of the viewport.
    /// </summary>
    public double ViewportWidth { get { return _viewport.Width; } }

    /// <summary>
    /// Gets the height of the viewport.
    /// </summary>
    public double ViewportHeight { get { return _viewport.Height; } }

    /// <summary>
    /// Gets the horizontal offset.
    /// </summary>
    public double HorizontalOffset { get { return _scrollOffset.X; } }

    /// <summary>
    /// Gets the vertical offset.
    /// </summary>
    public double VerticalOffset { get { return _scrollOffset.Y; } }

    /// <summary>
    /// Gets or sets the scroll owner.
    /// </summary>
    public ScrollViewer ScrollOwner
    {
      get
      {
        return _scrollOwner;
      }
      set
      {
        if (value != _scrollOwner)
        {
          _requiresMeasure = true;
          InvalidateMeasure();
          if (ScrollOwnerExists)
          {
            _internalOffset = new Vector();
            Size size = new Size();
            _maxDesiredSize = size;
            _viewport = size;
            _extent = size;
          }
          _scrollOwner = value;
        }
      }
    }

    private bool ScrollOwnerExists { get { return _scrollOwner != null; } }

    #endregion // IScrollInfo members
  }
}
