using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// The header control of a <see cref="DataGridColumn"/>.
  /// </summary>
  [TemplatePart(Name = HeaderGripperPartName, Type = typeof(Thumb))]
  public class DataGridColumnHeader : ButtonBase
  {
    private const string HeaderGripperPartName = "PART_HeaderGripper";

    //internal static DataGridColumn ResizingColumn { get; private set; }

    private Thumb _thumb;

    static DataGridColumnHeader()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridColumnHeader), new FrameworkPropertyMetadata(typeof(DataGridColumnHeader)));
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _thumb = GetTemplateChild(HeaderGripperPartName) as Thumb;
      if (_thumb != null)
      {
        _thumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        _thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
        _thumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
        _thumb.MouseDoubleClick += new MouseButtonEventHandler(Thumb_MouseDoubleClick);
      }
    }

    private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Column.IsAutoWidthDirty = true;
      Column.Width = new GridLength(1, GridUnitType.Auto);
    }

    #region Column Property

    /// <summary>
    /// Gets the <see cref="DataGridColumn"/> for this <see cref="DataGridColumnHeader"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridColumn Column
    {
      get { return (DataGridColumn)GetValue(ColumnProperty); }
      internal set
      {
        if (Column != value)
        {
          SetValue(ColumnPropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey ColumnPropertyKey =
        DependencyProperty.RegisterReadOnly("Column", typeof(DataGridColumn), typeof(DataGridColumnHeader), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Column"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnProperty =
        ColumnPropertyKey.DependencyProperty;

    #endregion // Column Property

    #region Role Property

    /// <summary>
    /// Gets whether this <see cref="DataGridColumnHeader"/> control is being used as a normal header, a cursor visual for relocating a column, 
    /// padding at the end of all columns or a grouped column header.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RoleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridColumnHeaderRole Role
    {
      get { return (DataGridColumnHeaderRole)GetValue(RoleProperty); }
      internal set { SetValue(RolePropertyKey, value); }
    }

    private static readonly DependencyPropertyKey RolePropertyKey =
        DependencyProperty.RegisterReadOnly("Role", typeof(DataGridColumnHeaderRole), typeof(DataGridColumnHeader), new UIPropertyMetadata(DataGridColumnHeaderRole.Normal));

    /// <summary>
    /// Identifies the <see cref="Role"/> property.
    /// </summary>
    public static readonly DependencyProperty RoleProperty =
        RolePropertyKey.DependencyProperty;

    #endregion // Role Property

    /// <summary>
    /// Called when the left mouse button is released over this <see cref="DataGridColumnHeader"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      if (Column != null && Column.AllowSort && !_dragLock && IsMouseOver)
      {
        if (Column.SortDirection == SortDirection.None)
        {
          Column.SortDirection = SortDirection.Ascending;
        }
        else if (Column.SortDirection == SortDirection.Ascending)
        {
          Column.SortDirection = SortDirection.Descending;
        }
        else
        {
          Column.SortDirection = SortDirection.None;
        }
      }
    }

    private Point _mouseDownPoint;

    /// <summary>
    /// Called when previewing the left mouse button being pressed over this <see cref="DataGridColumnHeader"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);

      _mouseDownPoint = e.GetPosition(null);
    }

    private bool _dragLock;

    /// <summary>
    /// Called when previewing the mouse moving over this <see cref="DataGridColumnHeader"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      //base.OnPreviewMouseMove(e);

      Point position = e.GetPosition(null);
      double xOffset = Math.Abs(position.X - _mouseDownPoint.X);
      double yOffset = Math.Abs(position.Y - _mouseDownPoint.Y);

      DataGrid dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);

      if (IsPressed && (xOffset > SystemParameters.MinimumHorizontalDragDistance || yOffset > SystemParameters.MinimumVerticalDragDistance)
        && !_dragLock && !_isResizing && Role != DataGridColumnHeaderRole.Padding)
      {
        _dragLock = true;
        
        DataObject dragData = new DataObject(DataFormats.Serializable, new WeakReference(this));
        DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move);

        // This code is only reached after the drag-drop operation is complete:
        _dragLock = false;
        if (_adorner != null && _adornerLayer != null)
        {
          _adornerLayer.Remove(_adorner);
          _adorner = null;
        }
      }
    }

    private SingleElementAdorner _adorner;
    private AdornerLayer _adornerLayer;
    private double _dragOffsetX;
    private double _dragOffsetY;

    private static int _count;

    /// <summary>
    /// Called when a drag and drop operation is giving feedback.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
      _count++;
      
      Window owner = VisualTreeUtils.FindAncestor<Window>(this);
      if (owner != null)
      {
        FrameworkElement content = (FrameworkElement)owner.Content;
        Point mousePosition = MouseUtils.GetPosition(content);
        Thickness margin = content.Margin == null ? new Thickness() : content.Margin;
        mousePosition = new Point(mousePosition.X + margin.Left, mousePosition.Y + margin.Top);
        _adornerLayer = AdornerLayer.GetAdornerLayer(content);
        if (_adorner == null && _adornerLayer != null)
        {
          Point offset = MouseUtils.GetPosition(this);
          _dragOffsetX = offset.X;
          _dragOffsetY = offset.Y;

          DataGridColumnHeader header = new DataGridColumnHeader();
          header.Role = DataGridColumnHeaderRole.Floating;
          header.Column = Column;
          header.Content = Column.Header;
          header.Style = Style;
          header.ContentTemplate = ContentTemplate;
          header.IsHitTestVisible = false;
          header.Width = Column.ActualWidth;
          header.Height = ActualHeight;

          _adorner = new SingleElementAdorner((UIElement)(owner.Content), header);
          _adornerLayer.Add(_adorner);
        }
        if (_adorner != null)
        {
          _adorner.RenderTransform = new TranslateTransform(mousePosition.X - _dragOffsetX, mousePosition.Y - _dragOffsetY);
        }
      }

      e.UseDefaultCursors = false;

      if (e.Effects == DragDropEffects.None)
      {
        Mouse.SetCursor(Cursors.No);
      }
      else
      {
        Mouse.SetCursor(Cursors.Arrow);
      }

      e.Handled = true;
    }

    private bool _isResizing;

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      if (Column.AllowResize)
      {
        _isResizing = true;
        Point position = MouseUtils.GetPosition(_thumb);
        _horizontalMouseDown = position.X;
        _horizontalChangeOffset = 0;
        //ResizingColumn = Column;
      }
    }

    private double _horizontalMouseDown;
    private double _horizontalChangeOffset;

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (Column.AllowResize)
      {
        double newWidth = Math.Max(Column.MinWidth, Column.ActualWidth + e.HorizontalChange - _horizontalChangeOffset);
        if (Column.IsStarSizing && DataGrid != null && DataGrid.DataGridPanel != null)
        {
          UpdateStarSizes(newWidth);
        }
        else if (newWidth <= Column.ActualWidth || _horizontalChangeOffset >= 0)
        {
          Column.Width = new GridLength(Math.Round(newWidth));
          // TODO: Even if it is not a star sizing column, we should scan all the star columns to the right of this column to limit the maximum size of this column.
          //       Currently resizing a fixed column will push star sizing columns off the right edge of the DataGrid which is not right.
        }
        /*DataGridRowPanel panel = VisualTreeUtils.FindAncestor<DataGridRowPanel>(this);
        if (panel != null)
        {
          panel.InvalidateMeasure();
        }*/
      }
      Dispatcher.BeginInvoke(new Action(CalculateHorizontalChangeOffset), DispatcherPriority.ApplicationIdle);
    }

    private void UpdateStarSizes(double newWidth)
    {
      //double totalAvailableWidth = 0;
      //double totalFillWeight = 0;
      //IList<DataGridColumn> starSizingColumns = new List<DataGridColumn>();
      //foreach (DataGridColumn column in DataGrid.EffectiveColumns)
      //{
      //  if (column.IsStarSizing)
      //  {
      //    totalFillWeight += column.FillWeight;
      //    totalAvailableWidth += column == Column ? newWidth : column.ActualWidth;
      //    starSizingColumns.Add(column);
      //  }
      //}
      ////double availableWidth = DataGrid.DataGridPanel.ViewportWidth - totalFinalizedWidth;
      //foreach (DataGridColumn column in starSizingColumns)
      //{
      //  double ratio = totalAvailableWidth <= 0 ? 0 : column.ActualWidth / totalAvailableWidth;
      //  double fillWeight = totalFillWeight * ratio;
      //  column.Width = fillWeight + DataGrid.StarSizingThreshold;
      //  /*double actualWidth = column == Column ? newWidth : column.ActualWidth;
      //  double ratio = availableWidth <= 0 ? 0 : actualWidth / availableWidth;
      //  double fillWeight = totalFillWeight * ratio;
      //  column.Width = fillWeight + DataGrid.StarSizingThreshold;
      //  if (column == Column)
      //  {
      //    break;
      //  }*/
      //}
      //Column.Width = newWidth;
      /*double totalFillWeight = 0;
      double totalAvailableWidth = 0;
      double totalAvailableWidthOfAllColumnsAfterCurrentColumn = Column.ActualWidth;
      double minimumSquashWidth = 0;
      IList<DataGridColumn> remainingColumns = new List<DataGridColumn>();
      bool foundColumn = false;
      foreach (DataGridColumn column in DataGrid.EffectiveColumns)
      {
        if (column == Column)
        {
          foundColumn = true;
        }
        if (foundColumn && column.IsStarSizing)
        {
          totalFillWeight += column.FillWeight;
          totalAvailableWidth += column.ActualWidth;
          if (column != Column)
          {
            remainingColumns.Add(column);
          }
        }
        if (foundColumn && column != Column)
        {
          totalAvailableWidthOfAllColumnsAfterCurrentColumn += column.ActualWidth;
          if (column.IsStarSizing)
          {
            minimumSquashWidth += column.MinWidth;
          }
          else
          {
            minimumSquashWidth += column.ActualWidth;
          }
        }
      }
      totalFillWeight = Math.Round(totalFillWeight);
      double theTotalFillWeight = totalFillWeight;
      newWidth = Math.Min(newWidth, totalAvailableWidthOfAllColumnsAfterCurrentColumn - minimumSquashWidth);

      double oldTotalFillWeight = totalFillWeight - Column.FillWeight;
      double columnRatio = totalAvailableWidth <= 0 ? 0 : newWidth / totalAvailableWidth;
      double columnFillWeight = totalFillWeight * columnRatio;
      //Debug.WriteLine("Ratio: " + columnRatio);
      //Debug.WriteLine("Column fill weight: " + columnFillWeight);
      Column.Width = new GridLength(columnFillWeight, GridUnitType.Star);
      //Column.SetActualWidth(Math.Max(newWidth, Column.MinWidth));
      totalFillWeight -= columnFillWeight;
      double theNewFillWeight = columnFillWeight;
      foreach (DataGridColumn column in remainingColumns)
      {
        double ratio = oldTotalFillWeight <= 0 ? 0 : column.FillWeight / oldTotalFillWeight;
        double newFillWeight = totalFillWeight * ratio;
        column.Width = new GridLength(newFillWeight, GridUnitType.Star);
        theNewFillWeight += newFillWeight;
      }*/

      //Debug.WriteLine("Old fill weight: " + theTotalFillWeight + ", new fill weight: " + theNewFillWeight);

      /*double totalFillWeight = 0;
      double totalAvailableWidth = 0;
      double totalConstantWidth = 0;
      double widthToLeft = 0;
      double minWidthToRight = 0;
      double constantWidthToRight = 0;
      double fillWeightToRight = 0;
      bool passedColumn = false;
      IList<DataGridColumn> starColumnsToRight = new List<DataGridColumn>();

      DataGridColumn lastStarColumn = null;

      foreach (DataGridColumn column in _dataGrid.EffectiveColumns)
      {
        totalAvailableWidth += column.ActualWidth;
        if (column.IsStarSizing)
        {
          totalFillWeight += column.Width.Value;
          lastStarColumn = column;
        }
        else
        {
          totalConstantWidth += column.ActualWidth;
        }
        if (!passedColumn && column != Column)
        {
          widthToLeft = column.ActualWidth;
        }
        if (passedColumn)
        {
          if (column.IsStarSizing)
          {
            if (column.IsVisible)
            {
              starColumnsToRight.Add(column);
              minWidthToRight += column.MinWidth;
              fillWeightToRight += column.Width.Value;
            }
          }
          else
          {
            minWidthToRight += column.ActualWidth;
            constantWidthToRight += column.ActualWidth;
          }
        }
        if (column == Column)
        {
          passedColumn = true;
        }
      }

      if (lastStarColumn == Column)
      {
        return;
      }

      double maxWidth = totalAvailableWidth - widthToLeft - minWidthToRight;
      newWidth = Math.Min(newWidth, maxWidth);

      double totalAvailableWidthForStarColumns = totalAvailableWidth - totalConstantWidth;
      double pixelsPerStar = totalAvailableWidthForStarColumns / totalFillWeight;

      Column.Width = new GridLength(newWidth / pixelsPerStar, GridUnitType.Star);

      double availableStarWidthToRight = totalAvailableWidth - widthToLeft - newWidth - constantWidthToRight;
      double compressedPixelsPerStar = availableStarWidthToRight / fillWeightToRight;

      foreach (DataGridColumn column in starColumnsToRight)
      {
        double width = column.Width.Value * compressedPixelsPerStar;
        column.Width = new GridLength(width / pixelsPerStar, GridUnitType.Star);
      }*/

      double availableWidth = 0;
      double fixedWidth = 0;
      double starCount = 0;
      double minimumWidth = 0;

      int index = DataGrid.EffectiveColumns.IndexOf(Column);
      for (int i = index; i < DataGrid.EffectiveColumns.Count; i++)
      {
        DataGridColumn column = DataGrid.EffectiveColumns[i];
        availableWidth += column.ActualWidth;
        if (column.Width.IsStar)
        {
          starCount += column.Width.Value;
          if (i != index)
          {
            minimumWidth += column.MinWidth;
          }
        }
        else
        {
          fixedWidth += column.ActualWidth;
          minimumWidth += column.ActualWidth;
        }
      }

      double workingWidth = availableWidth - fixedWidth;
      double maxWidth = availableWidth - minimumWidth;
      newWidth = Math.Min(maxWidth, newWidth);

      double starCountToRight = starCount - Column.Width.Value;
      double ratio = newWidth / workingWidth;
      double newStar = starCount * ratio;
      double remainingStarCount = starCount - newStar;
      Column.Width = new GridLength(newStar, GridUnitType.Star);

      for (int i = index + 1; i < DataGrid.EffectiveColumns.Count; i++)
      {
        DataGridColumn column = DataGrid.EffectiveColumns[i];
        if (column.Width.IsStar)
        {
          ratio = column.Width.Value / starCountToRight;
          column.Width = new GridLength(remainingStarCount * ratio, GridUnitType.Star);
        }
      }
    }

    private DataGrid _dataGrid;

    internal DataGrid DataGrid
    {
      get
      {
        if (_dataGrid == null)
        {
          _dataGrid = VisualTreeUtils.FindAncestor<DataGrid>(this);
        }
        return _dataGrid;
      }
    }

    private void CalculateHorizontalChangeOffset()
    {
      Point position = MouseUtils.GetPosition(_thumb);
      _horizontalChangeOffset = position.X - _horizontalMouseDown;
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (Column.AllowResize)
      {
        _isResizing = false;
        //ResizingColumn = null;
      }
      _horizontalChangeOffset = 0;
    }
  }
}
