using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A panel that provides layout logic for items within an in-Ribbon <see cref="Gallery"/>.
  /// </summary>
  public class GalleryPanel : Panel
  {
    private Dictionary<UIElement, Point> _positionCache = new Dictionary<UIElement,Point>();

    private int _rowIndex;
    private int _totalRowCount;
    private readonly Storyboard _storyboard;
    private readonly DoubleAnimation _indexAnimation;
    private int _visibleRowCount = 1;
    private int _visibleColumnCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryPanel"/> class.
    /// </summary>
    public GalleryPanel()
    {
      _storyboard = new Storyboard();
      _indexAnimation = new DoubleAnimation() { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)) };
      Storyboard.SetTargetProperty(_indexAnimation, new PropertyPath("CurrentRowIndex"));
      Storyboard.SetTarget(_indexAnimation, this);
      _storyboard.Children.Add(_indexAnimation);
    }

    /// <summary>
    /// Measures the elements within this <see cref="GalleryPanel"/> and returns the desired size.
    /// </summary>
    /// <param name="availableSize">The available size of this <see cref="GalleryPanel"/>.</param>
    /// <returns>The desired size of this <see cref="GalleryPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      _positionCache = new Dictionary<UIElement, Point>();

      // Aquire constants
      int maxColumnCount = 10;
      double itemWidth = 56;
      double itemHeight = 56;
      ItemsControl itemsHost = ItemsControl.GetItemsOwner(this);
      Gallery gallery = VisualTreeUtils.FindAncestor<Gallery>(itemsHost);
      if (gallery != null)
      {
        maxColumnCount = gallery.MaxColumnCount;
        itemWidth = gallery.ActualItemWidth;
        itemHeight = gallery.ActualItemHeight;
      }
      maxColumnCount = Math.Max(1, Math.Min(maxColumnCount, Children.Count));

      // Calculate final width
      double width = maxColumnCount * itemWidth;
      if (!Double.IsPositiveInfinity(availableSize.Width) && !Double.IsNaN(availableSize.Width))
      {
        int columnCount = Math.Max(gallery.MinColumnCount, (int)(availableSize.Width / itemWidth));
        if (columnCount < maxColumnCount)
        {
          maxColumnCount = columnCount;
          width = maxColumnCount * itemWidth;
        }
      }
      _visibleColumnCount = maxColumnCount;
      // Calculate final height
      double totalHeight = itemHeight;
      _visibleRowCount = 1;
      if (!Double.IsPositiveInfinity(availableSize.Height) && !Double.IsNaN(availableSize.Height))
      {
        _visibleRowCount = (int)(availableSize.Height / itemHeight);
        totalHeight = _visibleRowCount * itemHeight;
      }

      _totalRowCount = (int)Math.Ceiling(Children.Count / (double)maxColumnCount);

      double height = 0;
      double x = 0;
      double y = -itemHeight * CurrentRowIndex;
      int columnIndex = -1;
      int rowIndex = 0;
      foreach (UIElement element in Children)
      {
        columnIndex++;

        if (columnIndex == maxColumnCount)
        {
          columnIndex = 0;
          rowIndex++;
          x = 0;
          y += height;
        }

        _positionCache[element] = new Point(x, y);
        element.Measure(new Size(itemWidth, itemHeight));
        x += itemWidth;
        height = itemHeight;
      }

      return new Size(width, totalHeight);
    }

    /// <summary>
    /// Arranges the elements within this <see cref="GalleryPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="GalleryPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      double itemWidth = 56;
      double itemHeight = 56;
      ItemsControl itemsHost = ItemsControl.GetItemsOwner(this);
      Gallery gallery = VisualTreeUtils.FindAncestor<Gallery>(itemsHost);
      if (gallery != null)
      {
        itemWidth = gallery.ActualItemWidth;
        itemHeight = gallery.ActualItemHeight;
      }
      foreach (UIElement element in _positionCache.Keys)
      {
        Point point = new Point();
        _positionCache.TryGetValue(element, out point);
        element.Arrange(new Rect(point.X, point.Y, itemWidth, itemHeight));
        if (point.Y + itemHeight < 1)
        {
          element.Visibility = Visibility.Hidden;
        }
        else
        {
          element.Visibility = Visibility.Visible;
        }
      }

      return finalSize;
    }

    internal bool CanNavigateUp
    {
      get { return _rowIndex > 0; }
    }

    internal bool CanNavigateDown
    {
      get { return _rowIndex < _totalRowCount - _visibleRowCount; }
    }

    internal void NavigateUp()
    {
      if (CanNavigateUp)
      {
        RowIndex--;
        InvalidateMeasure();
      }
    }

    internal void NavigateDown()
    {
      if (CanNavigateDown)
      {
        RowIndex++;
        InvalidateMeasure();
      }
    }

    private int RowIndex
    {
      get { return _rowIndex; }
      set
      {
        _rowIndex = value;
        _indexAnimation.To = _rowIndex;
        _storyboard.Begin();
      }
    }

    internal void BringIntoView(object item)
    {
      ItemsControl itemsHost = ItemsControl.GetItemsOwner(this);
      if (itemsHost != null)
      {
        int index = itemsHost.Items.IndexOf(item);
        int row = index / _visibleColumnCount;
        if (row < CurrentRowIndex)
        {
          RowIndex = row;
          _storyboard.SkipToFill();
        }
        else if (row >= CurrentRowIndex + _visibleRowCount)
        {
          RowIndex = row - (_visibleRowCount - 1);
          _storyboard.SkipToFill();
        }
      }
    }

    #region CurrentRowIndex Property

    private double CurrentRowIndex
    {
      get { return (double)GetValue(CurrentRowIndexProperty); }
      set { SetValue(CurrentRowIndexProperty, value); }
    }

    private static readonly DependencyProperty CurrentRowIndexProperty =
      DependencyProperty.Register("CurrentRowIndex", typeof(double), typeof(GalleryPanel),
      new FrameworkPropertyMetadata(OnCurrentRowIndexChanged));

    private static void OnCurrentRowIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((GalleryPanel)d).OnCurrentRowIndexChanged();
    }

    private void OnCurrentRowIndexChanged()
    {
      InvalidateMeasure();
    }

    #endregion // CurrentRowIndex Property
  }
}
