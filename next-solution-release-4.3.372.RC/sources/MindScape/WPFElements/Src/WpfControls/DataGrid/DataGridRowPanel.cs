using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides column virtualization for a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridRowPanel : VirtualizingPanel
  {
    // DataGrid fields:
    private DataGrid _dataGrid; // TODO: try to remove these DataGrid fields.
    private DataGridRow _row;
    private DataGridHeaderRowPresenter _presenter;

    // Virtualization fields:
    private Dictionary<object, UIElement> _containerMap = new Dictionary<object, UIElement>();
    private HashSet<object> _previousItems = new HashSet<object>();
    private Queue<UIElement> _recycleBin = new Queue<UIElement>();
    private IList<UIElement> _realizedChildren = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridRowPanel"/>.
    /// </summary>
    public DataGridRowPanel()
    {
      Loaded += new RoutedEventHandler(DataGridRowPanel_Loaded);
    }

    private void DataGridRowPanel_Loaded(object sender, RoutedEventArgs e)
    {
      _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
      _row = VisualTreeUtils.FindContaining<DataGridRow>(this);
      _presenter = VisualTreeUtils.FindContaining<DataGridHeaderRowPresenter>(this);
      if (_dataGrid != null)
      {
        foreach (DataGridColumn column in _dataGrid.EffectiveColumns)
        {
          column.IsAutoWidthDirty = true;
        }
      }

      ScrollViewer scrollViewer = VisualTreeUtils.FindContaining<ScrollViewer>(this);
      if (scrollViewer != null && _row == null)
      {
        scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
      }
      InvalidateMeasure();
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      InvalidateMeasure();
    }

    /// <summary>
    /// Measures the elements within this <see cref="DataGridRowPanel"/> and returns the desired size.
    /// </summary>
    /// <param name="availableSize">The available size for this <see cref="DataGridRowPanel"/>.</param>
    /// <returns>The desired size of this <see cref="DataGridRowPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      if (_dataGrid != null && !Double.IsNaN(_dataGrid.ActualWidth))
      {
        availableSize.Width = _dataGrid.ActualWidth;
      }
      double width = Double.IsInfinity(availableSize.Width) ? 500 : availableSize.Width;
      double height = Double.IsInfinity(availableSize.Height) ? 500 : availableSize.Height;
      availableSize = new Size(width, height);
      _realizedChildren = new List<UIElement>();
      
      Size stackDesiredSize = new Size();
      ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
      ItemCollection items = itemsOwner.Items;
      int itemCount = items.Count;
      int firstVisibleIndex = Math.Max(FrozenColumnCount, FirstVisibleItemIndex);
      HashSet<object> nextCache = new HashSet<object>();

      bool columnOverflow = FillItems(0, Math.Min(FrozenColumnCount, itemCount), items, nextCache, availableSize.Width, ref stackDesiredSize);
      if (!columnOverflow && firstVisibleIndex < itemCount)
      {
        FillItems(firstVisibleIndex, itemCount, items, nextCache, availableSize.Width, ref stackDesiredSize);
      }

      foreach (object o in _previousItems)
      {
        UIElement element;
        _containerMap.TryGetValue(o, out element);
        if (element != null)
        {
          element.Visibility = Visibility.Hidden;
          ClearContainer(element, o);
          _recycleBin.Enqueue(element);
        }
        _containerMap.Remove(o);
      }
      _previousItems = nextCache;

      if (_row != null)
      {
        _row.RequiresRePrepare = false;
      }

      return stackDesiredSize;
    }

    internal Size ForceMeasure(Size availableSize)
    {
      return MeasureOverride(availableSize);
    }

    /// <summary>
    /// Arranges all the elements within this <see cref="DataGridRowPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="DataGridRowPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      Rect finalRect = new Rect(finalSize);
      double frozenX = 0.0;
      finalRect.X = Math.Round(FirstItemOffset - FrozenEdge);
      double width = 0.0;
      IList<UIElement> items = _realizedChildren;
      for (int i = 0; i < items.Count; i++)
      {
        UIElement container = items[i];
        if (container != null && container.IsVisible)
        {
          Size desiredSize = container.DesiredSize;
          finalRect.X += width;
          width = Math.Round(desiredSize.Width);

          // Get the actual width of the current column:
          DataGridColumn column = null;
          DataGridCellContainer cellContainer = container as DataGridCellContainer;
          if (cellContainer != null)
          {
            column = cellContainer.Column;
          }
          else
          {
            DataGridColumnHeader header = container as DataGridColumnHeader;
            if (header != null)
            {
              column = header.Column;
            }
          }
          if (column != null)
          {
            width = Math.Max(column.MinWidth, column.ActualWidth);
          }
          // End auto width logic

          finalRect.Width = width;
          finalRect.Height = Math.Max(finalSize.Height, desiredSize.Height);
          if (container != null)
          {
            double x = finalRect.X;
            if (i < FrozenColumnCount)
            {
              finalRect.X = frozenX;
              frozenX += finalRect.Width;
            }
            if (x < FrozenEdge && i >= FrozenColumnCount)
            {
              RectangleGeometry clip = new RectangleGeometry(new Rect(FrozenEdge - x, 0, Math.Max(0, width - (FrozenEdge - x)), finalSize.Height));
              container.Clip = clip;
            }
            else
            {
              container.Clip = null;
            }
            container.Arrange(finalRect);

            finalRect.X = x;
          }
        }
      }
      return finalSize;
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
    }

    internal IList<UIElement> RealizedChildren
    {
      get { return _realizedChildren; }
    }

    private bool FillItems(int startIndex, int maxCount, ItemCollection items, HashSet<object> nextCache, double availableWidth, ref Size stackDesiredSize)
    {
      bool columnOverflow = false;
      Size layoutSlotSize = new Size(availableWidth, Double.PositiveInfinity);
      if (_row != null && !Double.IsNaN(_row.Height))
      {
        layoutSlotSize.Height = _row.Height;
      }
      int currentIndex = startIndex;
      while (currentIndex < maxCount)
      {
        object item = items[currentIndex];
        
        currentIndex++;
        UIElement child = GetContainer(item);

        // Since we don't bind the width property anymore (in order to implement auto-sizing), we set the width of the cell here:
        DataGridColumn column = item as DataGridColumn;

        if (column == null)
        {
          GroupHeaderCell groupHeaderCell = item as GroupHeaderCell;
          if (groupHeaderCell != null)
          {
            column = groupHeaderCell.Column;
          }
          else
          {
            DataGridFooterCell footerCell = item as DataGridFooterCell;
            if (footerCell != null)
            {
              column = footerCell.Column;
            }
          }
        }

        if (column != null && !column.IsVisible)
        {
          child.Visibility = Visibility.Hidden;
          _realizedChildren.Add(child);
          continue;
        }
        _previousItems.Remove(item);

        child.Visibility = Visibility.Visible;
        nextCache.Add(item);

        FrameworkElement element = child as FrameworkElement;
        if (element != null && column != null)
        {
          if (column.IsStarSizing)
          {
            element.Width = Math.Max(column.MinWidth, column.ActualWidth);
            element.InvalidateMeasure();
          }
          else if (column.Width.IsAuto)
          {
            if (column.IsAutoWidthDirty)
            {
              element.Width = double.NaN;
            }
            else
            {
              element.Width = Math.Min(column.MaxAutoWidth, column.ActualWidth);
            }
          }
          else
          {
            element.Width = column.Width.Value;
          }

          if (element is DataGridColumnHeader && column.Width.IsAuto && _dataGrid != null && !_dataGrid.CanAutoSizeColumnHeaders)
          {
            element.Width = column.ActualWidth;
          }
        }

        _realizedChildren.Add(child);

        // Reset the width of the container to avoid issues with recycling auto-sizing columns:
        if (column.Width.IsAuto && column.IsAutoWidthDirty)
        {
          child.Measure(new Size(0, 0));
        }

        if (column.Width.IsAuto)
        {
          child.Measure(new Size(Math.Min(column.MaxAutoWidth, layoutSlotSize.Width), layoutSlotSize.Height));
        }
        else
        {
          // TODO: performance can be improved if we can find a way to not measure every cell.
          child.Measure(layoutSlotSize);
        }
        Size desiredSize = child.DesiredSize;

        double width = desiredSize.Width;

        // Update the auto-width for the current column:
        if (column.Width.IsAuto)
        {
          if (column.IsAutoWidthDirty && (!(element is DataGridColumnHeader) || (_dataGrid != null && _dataGrid.CanAutoSizeColumnHeaders)))
          {
            width = Math.Ceiling(Math.Max(column.AutoWidth, Math.Ceiling(width + 2)));
            column.AutoWidth = width;
          }
        }
        
        stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, desiredSize.Height);
        stackDesiredSize.Width += Math.Round(width);

        // This is part of column virtualization. This is disabled if there is an auto-sizing column.
        if ((_dataGrid == null || (!_dataGrid.HasAutoColumn && !_dataGrid.HasStarSizingColumn)))
        {
          if (stackDesiredSize.Width + FirstItemOffset - FrozenEdge > availableWidth)
          {
            columnOverflow = true;
            break;
          }
        }
      }
      return columnOverflow;
    }

    private UIElement GetContainer(object item)
    {
      UIElement child;
      _containerMap.TryGetValue(item, out child);
      if (child == null)
      {
        if (_recycleBin.Count != 0)
        {
          child = _recycleBin.Dequeue();
          child.Visibility = Visibility.Visible;
        }
        else
        {
          child = GetContainerForItem(item) as UIElement;
          AddInternalChild(child);
        }
        PrepareContainer(child, item);
        _containerMap[item] = child;
      }
      else
      {
        if (_row != null && _row.RequiresRePrepare)
        {
          DataGridCellContainer cell = child as DataGridCellContainer;
          if (cell != null)
          {
            _row.RePrepareContainer(cell);
          }
        }

        DataGridCellContainer cellContainer = child as DataGridCellContainer;
        if (cellContainer != null)
        {
          bool isExpandable = _dataGrid != null && _dataGrid.DisplayedItemsSource != null && _dataGrid.DisplayedItemsSource.HasHierarchy && _dataGrid.EffectiveColumns.Count > 0 && _dataGrid.EffectiveColumns[0] == item;
          cellContainer.SetIsExpandableCell(isExpandable);
          /*if (_row != null && _row.ItemWrapper != null)
          {
            cellContainer.SetHasChildren(_row.ItemWrapper.HasChildren);
            cellContainer.IsExpanded = _row.ItemWrapper.IsExpanded;
            cellContainer.SetLevel(_row.ItemWrapper.Level);
          }
          else
          {
            cellContainer.SetHasChildren(false);
            cellContainer.IsExpanded = false;
            cellContainer.SetLevel(0);
          }*/
        }
      }
      return child;
    }

    private DependencyObject GetContainerForItem(object item)
    {
      if (_row == null)
      {
        _row = VisualTreeUtils.FindContaining<DataGridRow>(this);
      }
      if (_presenter == null)
      {
        _presenter = VisualTreeUtils.FindContaining<DataGridHeaderRowPresenter>(this);
      }
      if (_row != null)
      {
        return _row.GetContainer(item);
      }
      else if (_presenter != null)
      {
        return _presenter.GetContainer();
      }
      return new ContentControl();
    }

    private void PrepareContainer(DependencyObject element, object item)
    {
      if (_row != null)
      {
        _row.PrepareContainer(element, item);
      }
      else if (_presenter != null)
      {
        _presenter.PrepareContainer(element, item);
      }
      else
      {
        ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
        ContentControl cc = element as ContentControl;
        DataGridColumn column = item as DataGridColumn;
        if (column == null)
        {
          GroupHeaderCell groupHeaderCell = item as GroupHeaderCell;
          if (groupHeaderCell != null)
          {
            column = groupHeaderCell.Column;
          }
          else
          {
            DataGridFooterCell footerCell = item as DataGridFooterCell;
            if (footerCell != null)
            {
              column = footerCell.Column;
            }
          }
        }
        if (cc != null && itemsOwner != null && column != null)
        {
          cc.Content = item;
          cc.ContentTemplate = itemsOwner.ItemTemplate;
          cc.ContentTemplateSelector = itemsOwner.ItemTemplateSelector;
          cc.Style = itemsOwner.ItemContainerStyle;
          cc.Width = column.ActualWidth;
        }
      }
    }

    private void ClearContainer(DependencyObject element, object item)
    {
      if (_row != null)
      {
        _row.ClearContainer(element, item);
      }
      else if (_presenter != null)
      {
        _presenter.ClearContainer(element, item);
      }
    }

    private int FirstVisibleItemIndex
    {
      get
      {
        return (_dataGrid == null || _dataGrid.DataGridPanel == null) ? 0 : _dataGrid.DataGridPanel.FirstVisibleColumnIndex;
      }
    }

    private double FirstItemOffset
    {
      get
      {
        return (_dataGrid == null || _dataGrid.DataGridPanel == null) ? 0 : _dataGrid.DataGridPanel.FirstVisibleColumnOffset;
      }
    }

    private double FrozenEdge
    {
      get
      {
        return (_dataGrid == null || _dataGrid.DataGridPanel == null) ? 0 : _dataGrid.DataGridPanel.FrozenEdge;
      }
    }

    private int FrozenColumnCount
    {
      get
      {
        return _dataGrid == null ? 0 : _dataGrid.FrozenColumnCount;
      }
    }

    internal UIElement GetElement(object item)
    {
      UIElement element;
      _containerMap.TryGetValue(item, out element);
      return element;
    }
  }
}
