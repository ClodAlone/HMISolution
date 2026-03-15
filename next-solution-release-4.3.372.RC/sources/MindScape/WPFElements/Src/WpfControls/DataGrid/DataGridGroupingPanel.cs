using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A panel that allows the user to drag and drop column headers to group by that column.
  /// </summary>
  public class DataGridGroupingPanel : ItemsControl
  {
    static DataGridGroupingPanel()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridGroupingPanel), new FrameworkPropertyMetadata(typeof(DataGridGroupingPanel)));
    }

    private readonly Dictionary<object, UIElement> _containerMap = new Dictionary<object, UIElement>();
    private IList<double> _dropPositions;
    private int _dropPositionIndex;
    private DataGrid _dataGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridGroupingPanel"/> class.
    /// </summary>
    public DataGridGroupingPanel()
    {
      AllowDrop = true;
      Loaded += new RoutedEventHandler(DataGridGroupingPanel_Loaded);
    }

    private void DataGridGroupingPanel_Loaded(object sender, RoutedEventArgs e)
    {
      _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
    }

    /// <summary>
    /// Called when a drag operation enters this <see cref="DataGridGroupingPanel"/>.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);

      _dropPositions = GetDropPositions();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _dataGrid = VisualTreeUtils.FindContaining<DataGrid>(this);
    }

    /// <summary>
    /// Called when a drag operation is moving over this <see cref="DataGridGroupingPanel"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      
      Point position = e.GetPosition(this);
      WeakReference reference = e.Data.GetData(DataFormats.Serializable, false) as WeakReference;
      if (reference != null && reference.IsAlive)
      {
        DataGridColumnHeader columnHeader = reference.Target as DataGridColumnHeader;
        if (columnHeader != null && columnHeader.Column.AllowGrouping)
        {
          if (columnHeader.DataGrid == _dataGrid)
          {
            IsRequestingColumnDropPosition = true;
            ColumnDropRequestPosition = 8;
            _dropPositionIndex = 0;

            // Find the closest drop position to the mouse:
            if (_dropPositions != null && _dropPositions.Count != 0)
            {
              double smallestDiff = Double.MaxValue;
              double closestPosition = 0;
              int index = 0;
              foreach (double x in _dropPositions)
              {
                double diff = Math.Abs(x - position.X);
                if (diff < smallestDiff)
                {
                  smallestDiff = diff;
                  closestPosition = x;
                  _dropPositionIndex = index;
                }
                index++;
              }
              ColumnDropRequestPosition = closestPosition;
            }
          }
          else
          {
            e.Handled = true;
            e.Effects = DragDropEffects.None;
          }
        }
      }
    }

    /// <summary>
    /// Called when a drag operation leaves this <see cref="DataGridGroupingPanel"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragLeave(DragEventArgs e)
    {
      base.OnDragLeave(e);
      // Even if the mouse is still within the bounds of the grouping panel, this method is still called every now and then.
      // Although the OnDragOver method is called instantly after this method in this situation, the drop indicator still flickers due to setting the IsRequestingDropPosition.
      // So this logic checks to see if the drag action really has left the grouping panel.
      Point position = MouseUtils.GetPosition(this);
      if (position.X < 1 || position.Y < 1 || position.X > ActualWidth - 1 || position.Y > ActualHeight - 1)
      {
        IsRequestingColumnDropPosition = false;
      }
    }

    /// <summary>
    /// Called when a drag operation is dropped over this <see cref="DataGridHeaderRowPresenter"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDrop(DragEventArgs e)
    {
      base.OnDrop(e);

      WeakReference reference = e.Data.GetData(DataFormats.Serializable, false) as WeakReference;
      if (reference != null && reference.IsAlive)
      {
        DataGridColumnHeader columnHeader = reference.Target as DataGridColumnHeader;
        if (columnHeader != null && IsRequestingColumnDropPosition)
        {
          ObservableCollection<DataGridColumn> collection = ItemsSource as ObservableCollection<DataGridColumn>;
          if (collection != null)
          {
            if (collection.Contains(columnHeader.Column))
            {
              int currentIndex = collection.IndexOf(columnHeader.Column);
              collection.Move(currentIndex, currentIndex < _dropPositionIndex ? _dropPositionIndex - 1 : _dropPositionIndex);
            }
            else
            {
              if (_dropPositionIndex == _dropPositions.Count)
              {
                collection.Add(columnHeader.Column);
              }
              else
              {
                collection.Insert(_dropPositionIndex, columnHeader.Column);
              }
            }
          }
        }
      }
      IsRequestingColumnDropPosition = false;
    }

    private IList<double> GetDropPositions()
    {
      IList<double> dropPositions = new List<double>();

      double previousHeaderPosition = 0;
      double half = 6;
      double firstPosition = 0;
      foreach (object o in ItemsSource)
      {
        DataGridColumn column = o as DataGridColumn;
        DataGridColumnHeader header = _containerMap[column] as DataGridColumnHeader;
        Point position = header.TranslatePoint(new Point(0, 0), this);

        if (previousHeaderPosition != 0)
        {
          half = (position.X - previousHeaderPosition) / 2.0;
          dropPositions.Add(previousHeaderPosition + half);
        }
        else
        {
          firstPosition = position.X;
        }
        previousHeaderPosition = position.X + header.ActualWidth;
      }
      if (HasItems)
      {
        dropPositions.Insert(0, firstPosition - half);
        dropPositions.Add(previousHeaderPosition + half);
      }

      return dropPositions;
    }

    #region ColumnDropRequestPosition Property

    /// <summary>
    /// Gets the position of the column drop request from the left edge of this <see cref="DataGridGroupingPanel"/>.
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
      private set { SetValue(DataGridGroupingPanel.ColumnDropRequestPositionPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey ColumnDropRequestPositionPropertyKey =
        DependencyProperty.RegisterReadOnly("ColumnDropRequestPosition", typeof(double), typeof(DataGridGroupingPanel), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="ColumnDropRequestPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnDropRequestPositionProperty =
        ColumnDropRequestPositionPropertyKey.DependencyProperty;

    #endregion // ColumnDropRequestPosition Property

    #region IsRequestingColumnDropPosition Property

    /// <summary>
    /// Gets whether a column is requesting to be dropped onto this <see cref="DataGridGroupingPanel"/>.
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
      private set { SetValue(DataGridGroupingPanel.IsRequestingColumnDropPositionPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey IsRequestingColumnDropPositionPropertyKey =
        DependencyProperty.RegisterReadOnly("IsRequestingColumnDropPosition", typeof(bool), typeof(DataGridGroupingPanel), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsRequestingColumnDropPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty IsRequestingColumnDropPositionProperty =
        IsRequestingColumnDropPositionPropertyKey.DependencyProperty;

    #endregion // IsRequestingColumnDropPosition Property

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
    /// Clears the given container.
    /// </summary>
    /// <param name="element">The container to clear.</param>
    /// <param name="item">The item held by the given container.</param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      _containerMap.Remove(item);
    }

    /// <summary>
    /// Prepares the given container to display the given item.
    /// </summary>
    /// <param name="element">The <see cref="DataGridColumnHeader"/> to prepare.</param>
    /// <param name="item">The <see cref="DataGridColumn"/> to be displayed by the given header.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridColumn column = item as DataGridColumn;
      if (column != null)
      {
        DataGridColumnHeader header = element as DataGridColumnHeader;
        header.Role = DataGridColumnHeaderRole.Grouping;
        header.Style = ItemContainerStyle;
        header.ContentTemplate = ItemTemplate;
        header.Content = column.Header;
        header.Column = column;
        _containerMap[column] = header;
      }
      else
      {
        throw new InvalidOperationException("DataGridGroupingPanel only supports DataGridColumn objects");
      }

      //column.SortDirectionChanged += new EventHandler(DataGridColumn_SortDirectionChanged);

      //BindingOperations.SetBinding(header, DataGridCellContainer.WidthProperty, new Binding { Source = column, Path = new PropertyPath("Width"), Mode = BindingMode.TwoWay });
    }
  }
}
