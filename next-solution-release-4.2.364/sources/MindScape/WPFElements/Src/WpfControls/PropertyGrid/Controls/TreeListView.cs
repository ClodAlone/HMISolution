using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// A control with the expand/collapse functionality of a TreeView, but supporting
  /// multiple columns like a ListView.
  /// </summary>
  [TemplatePart(Name=ScrollViewerPartName, Type=typeof(ScrollViewer))]
  public class TreeListView : TreeView
  {
    private const string ScrollViewerPartName = "PART_ScrollViewer";
    private const double MinVariableColumnWidth = 40;

    static TreeListView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), 
        new FrameworkPropertyMetadata(typeof(TreeListView)));
    }

    #region Attached properties for list view columns and helper methods

    /// <summary>
    /// Gets the value of the IsFillColumn attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the IsFillColumn attached property.</returns>
    /// <remarks>If you set the IsFillColumn attached property on a GridViewColumn,
    /// this column will resize itself to use up all space not allocated to other columns.</remarks>
    public static bool GetIsFillColumn(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsFillColumnProperty);
    }

    /// <summary>
    /// Sets the value of the IsFillColumn attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    /// <remarks>If you set the IsFillColumn attached property on a GridViewColumn,
    /// this column will resize itself to use up all space not allocated to other columns.</remarks>
    public static void SetIsFillColumn(DependencyObject obj, bool value)
    {
      obj.SetValue(IsFillColumnProperty, value);
    }

    /// <summary>
    /// Identifies the IsFillColumn attached property.
    /// </summary>
    public static readonly DependencyProperty IsFillColumnProperty =
        DependencyProperty.RegisterAttached("IsFillColumn", typeof(bool),
        typeof(TreeListView), new UIPropertyMetadata(false));


    /// <summary>
    /// Gets or sets the minimum width of the resize column (the column
    /// whose IsResizeColumn property is set to true).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ResizeColumnMinWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ResizeColumnMinWidth
    {
      get { return (double)GetValue(ResizeColumnMinWidthProperty); }
      set { SetValue(ResizeColumnMinWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ResizeColumnMinWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty ResizeColumnMinWidthProperty =
      DependencyProperty.Register("ResizeColumnMinWidth", typeof(double), typeof(TreeListView),
      new FrameworkPropertyMetadata(MinVariableColumnWidth));

    /// <summary>
    /// Gets the value of the IsResizeColumn attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the IsResizeColumn attached property.</returns>
    /// <remarks>If you set the IsResizeColumn attached property on a GridViewColumn,
    /// you can resize the column by dragging its right-hand border.</remarks>
    public static bool GetIsResizeColumn(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsResizeColumnProperty);
    }

    /// <summary>
    /// Sets the value of the IsResizeColumn attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    /// <remarks>If you set the IsResizeColumn attached property on a GridViewColumn,
    /// you can resize the column by dragging its right-hand border.</remarks>
    public static void SetIsResizeColumn(DependencyObject obj, bool value)
    {
      obj.SetValue(IsResizeColumnProperty, value);
    }

    /// <summary>
    /// Identifies the IsResizeColumn attached property.
    /// </summary>
    public static readonly DependencyProperty IsResizeColumnProperty =
        DependencyProperty.RegisterAttached("IsResizeColumn", typeof(bool),
        typeof(TreeListView), new UIPropertyMetadata(false));

    private static GridViewColumn FindColumnWithAttachedProperty(GridViewColumnCollection columns, DependencyProperty dp, int? defaultIndex)
    {
      if (columns != null)
      {
        foreach (GridViewColumn column in columns)
        {
          if ((bool)(column.GetValue(dp)))
          {
            return column;
          }
        }

        if (defaultIndex.HasValue)
        {
          int defaultIndexValue = defaultIndex.Value;
          if (defaultIndexValue < columns.Count)
          {
            return columns[defaultIndexValue];
          }
        }
      }

      return null;
    }

    //private static bool HasPropertyValue(DependencyObject obj, DependencyProperty dp)
    //{
    //  object value = obj.ReadLocalValue(dp);
    //  if (value != null && value != DependencyProperty.UnsetValue && dp.PropertyType.IsAssignableFrom(value.GetType()))
    //  {
    //    return true;
    //  }

    //  return false;
    //}

    private static GridViewColumn FindFillColumn(GridViewColumnCollection columns)
    {
      return FindColumnWithAttachedProperty(columns, IsFillColumnProperty, 1);
    }

    private static GridViewColumn FindResizeColumn(GridViewColumnCollection columns)
    {
      return FindColumnWithAttachedProperty(columns, IsResizeColumnProperty, 0);
    }

    private static IEnumerable<GridViewColumn> FindNonFillColumns(GridViewColumnCollection columns)
    {
      GridViewColumn fillColumn = FindFillColumn(columns);
      foreach (GridViewColumn column in columns)
      {
        if (column != fillColumn)
        {
          yield return column;
        }
      }
    }

    #endregion

    /// <summary>
    /// Gets or sets the columns in the <see cref="TreeListView"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="Following the normal pattern for dependency properties")]
    public GridViewColumnCollection Columns
    {
      get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Columns"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), 
        typeof(TreeListView), new UIPropertyMetadata(null));

    private bool AllowColumnResizing
    {
      get { return GetAllowColumnResizing(this); }
    }

    /// <summary>
    /// Gets the value of the AllowColumnResizing attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the AllowColumnResizing attached property.</returns>
    public static bool GetAllowColumnResizing(DependencyObject obj)
    {
      return (bool)obj.GetValue(AllowColumnResizingProperty);
    }

    /// <summary>
    /// Sets the value of the AllowColumnResizing attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetAllowColumnResizing(DependencyObject obj, bool value)
    {
      obj.SetValue(AllowColumnResizingProperty, value);
    }

    /// <summary>
    /// Identifies the AllowColumnResizing attached property.
    /// </summary>
    public static readonly DependencyProperty AllowColumnResizingProperty =
      DependencyProperty.RegisterAttached("AllowColumnResizing", typeof(bool), 
      typeof(TreeListView), 
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets the value of the ResizeColumnWidth attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the ResizeColumnWidth attached property.</returns>
    public static double GetResizeColumnWidth(DependencyObject obj)
    {
      return (double)obj.GetValue(ResizeColumnWidthProperty);
    }

    /// <summary>
    /// Sets the value of the ResizeColumnWidth attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetResizeColumnWidth(DependencyObject obj, double value)
    {
      obj.SetValue(ResizeColumnWidthProperty, value);
    }

    /// <summary>
    /// Identifies the ResizeColumnWidth attached property.
    /// </summary>
    public static readonly DependencyProperty ResizeColumnWidthProperty =
      DependencyProperty.RegisterAttached("ResizeColumnWidth", typeof(double), typeof(TreeListView),
      new FrameworkPropertyMetadata(Double.NaN));


    /// <summary>
    /// Gets the value of the ShowSubitemExpanders attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the ShowSubitemExpanders attached property.</returns>
    public static bool GetShowSubitemExpanders(DependencyObject obj)
    {
      return (bool)obj.GetValue(ShowSubitemExpandersProperty);
    }

    /// <summary>
    /// Sets the value of the ShowSubitemExpanders attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetShowSubitemExpanders(DependencyObject obj, bool value)
    {
      obj.SetValue(ShowSubitemExpandersProperty, value);
    }

    /// <summary>
    /// Identifies the ShowSubitemExpanders attached property.
    /// </summary>
    public static readonly DependencyProperty ShowSubitemExpandersProperty =
      DependencyProperty.RegisterAttached("ShowSubitemExpanders", typeof(bool), typeof(TreeListView),
      new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets the value of the ShowGroupExpanders attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the ShowGroupExpanders attached property.</returns>
    public static bool GetShowGroupExpanders(DependencyObject obj)
    {
      return (bool)obj.GetValue(ShowGroupExpandersProperty);
    }

    /// <summary>
    /// Sets the value of the ShowGroupExpanders attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetShowGroupExpanders(DependencyObject obj, bool value)
    {
      obj.SetValue(ShowGroupExpandersProperty, value);
    }

    /// <summary>
    /// Identifies the ShowGroupExpanders attached property.
    /// </summary>
    public static readonly DependencyProperty ShowGroupExpandersProperty =
      DependencyProperty.RegisterAttached("ShowGroupExpanders", typeof(bool), typeof(TreeListView),
      new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the default <see cref="Style"/> of an
    /// expand/collapse toggle button.
    /// </summary>
    public static object ExpandCollapseToggleStyleKey
    {
      get { return new ComponentResourceKey(typeof(TreeListView), "ExpandCollapseToggleStyle"); }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="TreeListView"/> class.
    /// </summary>
    public TreeListView()
    {
      if (DesignerProperties.GetIsInDesignMode(this))
      {
        Columns = new GridViewColumnCollection();
      }
      SetUpColumnResizingOnControlResize();
    }

    /// <summary>
    /// Called when the ItemsSource property changes.
    /// </summary>
    /// <param name="oldValue">The old items source.</param>
    /// <param name="newValue">The new items source.</param>
    protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
    {
      INotifyCollectionChanged notifier = oldValue as INotifyCollectionChanged;
      if (notifier != null)
      {
        notifier.CollectionChanged -= ItemsSource_CollectionChanged;
      }

      base.OnItemsSourceChanged(oldValue, newValue);

      notifier = ItemsSource as INotifyCollectionChanged;
      if (notifier != null)
      {
        notifier.CollectionChanged += ItemsSource_CollectionChanged;
      }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      Dispatcher.BeginInvoke(new Action(ResizeFillColumn), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
    }

    #region Column resizing

    private void SetUpColumnResizingOnControlResize()
    {
      SizeChanged += delegate(object sender, SizeChangedEventArgs e)
        {
          ResizeFillColumn();
        };
    }

    private Point _dragStartPoint;

    /// <summary>
    /// Called by the Windows Presentation Foundation infrastructure when
    /// a template is applied to the control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      SetUpColumnResizing();
    }

    private void SetUpColumnResizing()
    {
      ScrollViewer scrollViewer = GetTemplateChild(ScrollViewerPartName) as ScrollViewer;
      if (scrollViewer != null)
      {
        scrollViewer.ScrollChanged += delegate(object sender, ScrollChangedEventArgs e)
          {
            ResizeFillColumn();
          };
      }

      AddHandler(PreviewMouseMoveEvent, new MouseEventHandler(OnPreviewMouseMove), true);

      PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
      PreviewMouseMove += OnPreviewMouseMove;
      PreviewMouseUp += OnPreviewMouseUp;

      // For some reason the GridViewRowPresenter is not available immediately after applying
      // the template.  Would like to understand now, but for now just call ourselves back
      // when we're ready.
      //ResizeValueColumn();
      Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
        new ThreadStart(ResizeFillColumn));
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (Equals(Cursor, Cursors.SizeWE) && AllowColumnResizing)
      {
        _dragStartPoint = e.GetPosition(this);

        CaptureMouse();

        e.Handled = true;
      }
    }

    private static double TotalWidthOfFixedColumns(GridViewRowPresenterBase grid)
    {
      double width = grid.ActualWidth;

      GridViewColumn resizeColumn = FindResizeColumn(grid.Columns);
      if (resizeColumn != null)
      {
        width -= resizeColumn.ActualWidth;
      }

      GridViewColumn fillColumn = FindFillColumn(grid.Columns);
      if (fillColumn != null)
      {
        width -= fillColumn.ActualWidth;
      }

      return width;
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
      Point point = e.GetPosition(this);

      GridViewRowPresenter gvrp = GetGridPresenter();

      if (gvrp != null && AllowColumnResizing)
      {
        GridViewColumn resizeColumn = FindResizeColumn(gvrp.Columns);
        if (resizeColumn != null)
        {
          if (IsMouseCaptured)
          {
            double delta = point.X - _dragStartPoint.X;

            double availableControlWidth = GetAvailableWidth();
            double variableColumnsMaxTotalWidthAllowed = availableControlWidth - TotalWidthOfFixedColumns(gvrp);
            double resizeColumnDesiredWidth = resizeColumn.ActualWidth + delta;
            //double resizeColumnMinWidth = HasPropertyValue(this, TreeListView.ResizeColumnMinWidthProperty) ? ResizeColumnMinWidth : MinVariableColumnWidth;
            double resizeColumnMaxWidth = variableColumnsMaxTotalWidthAllowed - ResizeColumnMinWidth;  // so as to leave MinVariableColumnWidth over for the FillColumn

            if (resizeColumnDesiredWidth > resizeColumnMaxWidth)
            {
              resizeColumnDesiredWidth = resizeColumnMaxWidth;
            }

            if (resizeColumnDesiredWidth < ResizeColumnMinWidth)
            {
              resizeColumnDesiredWidth = ResizeColumnMinWidth;
            }

            resizeColumn.Width = resizeColumnDesiredWidth;

            _dragStartPoint = point;

            ResizeFillColumn();

            e.Handled = true;
          }
          else
          {
            double resizeColumnWidth = resizeColumn.ActualWidth;
            if (point.X > resizeColumnWidth - 6 && point.X < resizeColumnWidth + 3)
            {
              Cursor = Cursors.SizeWE;
              e.Handled = true;
            }
            else
            {
              Cursor = Cursors.Arrow;
            }
          }
        }
      }
    }

    private double GetAvailableWidth()
    {
      double viewportWidth = Double.PositiveInfinity;
      ScrollViewer sv = GetTemplateChild(ScrollViewerPartName) as ScrollViewer;
      if (sv != null)
      {
        viewportWidth = sv.ViewportWidth;
      }

      GridViewRowPresenter gvrp = GetGridPresenter();
      if (gvrp == null)
      {
        return ActualWidth;
      }

      return Math.Min(viewportWidth, Math.Min(ActualWidth, gvrp.ActualWidth - 2));
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (IsMouseCaptured && AllowColumnResizing)
      {
        e.Handled = true;

        ReleaseMouseCapture();

        Cursor = Cursors.Arrow;
      }
    }

    private void ResizeFillColumn()
    {
      GridViewRowPresenter grid = GetGridPresenter();
      if (grid != null && grid.Columns != null)
      {
        GridViewColumn fillColumn = FindFillColumn(grid.Columns);
        if (fillColumn != null)
        {
          double usedWidth = 0;
          foreach (GridViewColumn column in FindNonFillColumns(grid.Columns))
          {
            usedWidth += column.ActualWidth;
          }
          fillColumn.Width = Math.Max(0, GetAvailableWidth() - usedWidth - 2);
        }
      }
      //else
      //{
      //  Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
      //    new ThreadStart(ResizeFillColumn));
      //}
    }

    #endregion

    #region Default template helpers

    private GridViewRowPresenter GetGridPresenter()
    {
      return FindChildFrameworkElement(this, delegate(FrameworkElement element)
      {
        return element is GridViewRowPresenter;
      }) as GridViewRowPresenter;
    }

    private static FrameworkElement FindChildFrameworkElement(
         DependencyObject dependencyObject, Predicate<FrameworkElement> where)
    {
      FrameworkElement result = null;

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        DependencyObject childDependencyObject
          = VisualTreeHelper.GetChild(dependencyObject, i);

        result = childDependencyObject as FrameworkElement;

        if ((result != null) && where(result))
        {
          break;
        }

        result = FindChildFrameworkElement(childDependencyObject, where);

        if (result != null)
        {
          break;
        }
      }

      return result;
    }

    #endregion

    #region Override item type

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>The element that is used to display the given item.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TreeListViewItem();
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is TreeListViewItem;
    }

    /// <summary>
    /// Prepares the container.
    /// </summary>
    /// <param name="element">The container to prepare.</param>
    /// <param name="item">The item that will be hosted by the container.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      PropertyGrid grid = VisualTreeUtils.FindAncestor<PropertyGrid>(this);
      if (grid != null && grid.SelectedObject != null)
      {
        PropertyGridRow row = item as PropertyGridRow;
        TreeListViewItem treeViewItem = element as TreeListViewItem;
        if (row != null && item != null)
        {
          bool isExpanded = false;

          if (grid.ExpandedStates != null && grid.IsExpandedStatePersistent)
          {
            Dictionary<string, bool> expandedStates;
            grid.ExpandedStates.TryGetValue(grid.SelectedObject.GetType(), out expandedStates);
            if (expandedStates != null)
            {
              expandedStates.TryGetValue(row.Node.Property.DeclaringType.ToString() + " " + row.Node.Property.DisplayName + " 0", out isExpanded);
            }
          }
          else
          {
            isExpanded = grid.ExpanderMode == ExpanderMode.Expanded;
          }

          treeViewItem.IsExpanded = isExpanded;
        }
      }
    }

    #endregion
  }

  /// <summary>
  /// Implements a selectable item in a <see cref="TreeListView"/> control.
  /// </summary>
  public class TreeListViewItem : TreeViewItem
  {
    static TreeListViewItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem),
        new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeListViewItem"/> class.
    /// </summary>
    public TreeListViewItem()
    {
      AddHandler(Control.PreviewMouseDownEvent, new MouseButtonEventHandler(TreeListViewItem_MouseDown), true);
    }

    private void TreeListViewItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      IsSelected = true;
    }

    /// <summary>
    /// Gets the level of the <see cref="TreeListViewItem"/> in the hierarchy.
    /// </summary>
    public int Level
    {
      get
      {
        if (_level == -1)
        {
          TreeListViewItem parent = ItemsControlFromItemContainer(this) as TreeListViewItem;
          _level = (parent != null) ? parent.Level + 1 : 0;
        }
        return _level;
      }
    }

    /// <summary>
    /// Creates a new <see cref="TreeListViewItem"/> to display a child item.
    /// </summary>
    /// <returns>A new TreeListViewItem.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TreeListViewItem();
    }

    /// <summary>
    /// Determines whether an object is a <see cref="TreeListViewItem"/>.
    /// </summary>
    /// <param name="item">The object to evaluate.</param>
    /// <returns>true if item is a TreeListViewItem; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is TreeListViewItem;
    }

    /// <summary>
    /// Prepares the container.
    /// </summary>
    /// <param name="element">The container to prepare.</param>
    /// <param name="item">The item that will be hosted by the container.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      PropertyGrid grid = VisualTreeUtils.FindAncestor<PropertyGrid>(this);
      if (grid != null && grid.SelectedObject != null)
      {
        PropertyGridRow row = item as PropertyGridRow;
        TreeListViewItem treeViewItem = element as TreeListViewItem;
        if (row != null && item != null)
        {
          bool isExpanded = false;
          if (grid.ExpandedStates != null && grid.IsExpandedStatePersistent)
          {
            Dictionary<string, bool> expandedStates;
            grid.ExpandedStates.TryGetValue(grid.SelectedObject.GetType(), out expandedStates);
            if (expandedStates != null)
            {
              expandedStates.TryGetValue(row.Node.Property.DeclaringType.ToString() + " " + row.Node.Property.DisplayName + " " + (Level + 1), out isExpanded);
            }
          }
          else if (Level < 10) // Kludge for avoiding neverending model structures.
          {
            isExpanded = grid.ExpanderMode == ExpanderMode.Expanded;
          }
          treeViewItem.IsExpanded = isExpanded;
        }
      }
    }

    private int _level = -1;

    /// <summary>
    /// Identifies the <see cref="Expanding"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ExpandingEvent =
      EventManager.RegisterRoutedEvent("Expanding", RoutingStrategy.Direct,
        typeof(RoutedEventHandler), typeof(TreeListViewItem));

    /// <summary>
    /// Occurs when the <see cref="TreeListViewItem"/> is expanded after it is loaded.
    /// </summary>
    public event RoutedEventHandler Expanding
    {
      add { AddHandler(ExpandingEvent, value); }
      remove { RemoveHandler(ExpandingEvent, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Collapsing"/> routed event.
    /// </summary>
    public static readonly RoutedEvent CollapsingEvent =
      EventManager.RegisterRoutedEvent("Collapsing", RoutingStrategy.Direct,
        typeof(RoutedEventHandler), typeof(TreeListViewItem));

    /// <summary>
    /// Occurs when the <see cref="TreeListViewItem"/> is collapsed after it is loaded.
    /// </summary>
    public event RoutedEventHandler Collapsing
    {
      add { AddHandler(CollapsingEvent, value); }
      remove { RemoveHandler(CollapsingEvent, value); }
    }

    /// <summary>
    /// Called when the <see cref="TreeListViewItem"/> is expanded.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnExpanded(RoutedEventArgs e)
    {
      base.OnExpanded(e);
      if (IsLoaded)
      {
        RaiseEvent(new RoutedEventArgs(ExpandingEvent));
      }
    }

    /// <summary>
    /// Called when the <see cref="TreeListViewItem"/> is collapsed.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnCollapsed(RoutedEventArgs e)
    {
      base.OnCollapsed(e);
      if (IsLoaded)
      {
        RaiseEvent(new RoutedEventArgs(CollapsingEvent));
      }
    }
  }
}
