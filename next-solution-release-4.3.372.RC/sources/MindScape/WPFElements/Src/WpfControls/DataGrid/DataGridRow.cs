using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A control representing a row in a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridRow : ItemsControl, IDataGridRow, INotifyPropertyChanged
  {
    static DataGridRow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridRow), new FrameworkPropertyMetadata(typeof(DataGridRow)));
    }

    private readonly DataGrid _parent;

    internal DataGridRow(DataGrid parent)
    {
      _parent = parent;
      Loaded += new RoutedEventHandler(DataGridRow_Loaded);
    }

    internal DataGrid DataGrid { get { return _parent; } }

    private void DataGridRow_Loaded(object sender, RoutedEventArgs e)
    {
      Panel = VisualTreeUtils.GetChild<DataGridRowPanel>(this);
      if (_parent != null && _parent.DataGridPanel != null)
      {
        // TODO: would be great if we didn't need to do this.
        // The problem is that after the row has loaded (after the measurment pass), the row suddenly decides that the height is a couple of pixels wrong.
        // Without this, there can be an empty gap at the bottom of the grid where some rows should be.
        // Need to find what is causing the size of the row to change after the measure pass.
        _parent.DataGridPanel.InvalidateRequiresMeasure();
      }
    }

    internal DataGridRowPanel Panel { get; private set; }

    #region Content Property

    /// <summary>
    /// Gets or sets the content of this <see cref="DataGridRow"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
      DependencyProperty.Register("Content", typeof(object), typeof(DataGridRow));

    #endregion // Content Property

    internal bool RequiresRePrepare { get; set; }

    internal void RePrepareContainer(DataGridCellContainer cell)
    {
      cell.Content = Content;
      if (cell.Column != null && cell.Column.DisplayMemberBinding != null)
      {
        cell.DataContext = Content; // TODO: now that we set DataContext, we might not need to set the Content property anymore
      }

      // Restore state information that may have been destroyed by virtualization:
      RestoreState(cell);
    }

    #region IsSelected Property

    /// <summary>
    /// Gets whether this <see cref="DataGridRow"/> is selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      internal set
      {
        if (IsSelected != value)
        {
          SetValue(IsSelectedPropertyKey, value);
          EventHandler handler = IsSelectedChanged;
          if (handler != null)
          {
            handler(this, EventArgs.Empty);
          }
        }
      }
    }

    private static readonly DependencyPropertyKey IsSelectedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(DataGridRow), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        IsSelectedPropertyKey.DependencyProperty;

    /// <summary>
    /// Raised when the IsSelected property changes.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    #endregion // IsSelected Property

    #region IsEditing Property

    /// <summary>
    /// Gets whether or not one of the cells in this <see cref="DataGridRow"/> is in edit mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEditingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEditing
    {
      get { return (bool)GetValue(IsEditingProperty); }
    }

    internal static readonly DependencyPropertyKey IsEditingPropertyKey =
        DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(DataGridRow), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsEditing"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEditingProperty =
        IsEditingPropertyKey.DependencyProperty;

    #endregion // IsEditing Property

    #region Index Property

    /// <summary>
    /// Gets the display index of this <see cref="DataGridRow"/> on the current page.
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
        DependencyProperty.RegisterReadOnly("Index", typeof(int), typeof(DataGridRow), new UIPropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="Index"/> property.
    /// </summary>
    public static readonly DependencyProperty IndexProperty =
        IndexPropertyKey.DependencyProperty;

    #endregion // Index Property

    #region Level Property

    /// <summary>
    /// Gets the level of this <see cref="DataGridRow"/> within the grouping tree.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LevelProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Level
    {
      get { return (int)GetValue(LevelProperty); }
      internal set
      {
        SetValue(LevelPropertyKey, value);
      }
    }

    private static readonly DependencyPropertyKey LevelPropertyKey =
        DependencyProperty.RegisterReadOnly("Level", typeof(int), typeof(DataGridRow), new UIPropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="Level"/> property.
    /// </summary>
    public static readonly DependencyProperty LevelProperty =
        LevelPropertyKey.DependencyProperty;

    #endregion // Level Property

    private bool _hasChildren;

    /// <summary>
    /// Gets whether or not this row has hierarchical children.
    /// </summary>
    public bool HasChildren
    {
      get { return _hasChildren; }
      private set
      {
        if (_hasChildren != value)
        {
          _hasChildren = value;
          OnPropertyChanged("HasChildren");
        }
      }
    }

    private int _hierarchyLevel;

    /// <summary>
    /// Gets the level of this row within the hierarchy tree.
    /// </summary>
    public int HierarchyLevel
    {
      get { return _hierarchyLevel; }
      private set
      {
        if (_hierarchyLevel != value)
        {
          _hierarchyLevel = value;
          OnPropertyChanged("HierarchyLevel");
        }
      }
    }

    private bool _isExpanded;

    /// <summary>
    /// Gets or sets whether or not this row is expanded to show it's children.
    /// </summary>
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded != value)
        {
          _isExpanded = value;
          if (ItemWrapper != null)
          {
            ItemWrapper.IsExpanded = _isExpanded;
          }
          OnPropertyChanged("IsExpanded");
        }
      }
    }

    // TODO: There only really needs to be one of these per DataGrid control.
    // Also, may change this so that the row listens to the cells, and change the IsEditing property key back to private.
    private DataGridCellContainer _editingCell; // The cell being edited.

    internal DataGridCellContainer EditingCell
    {
      get { return _editingCell; }
      set { _editingCell = value; }
    }

    internal DependencyObject GetContainer(object item)
    {
      //return GetContainerForItemOverride();
      // TODO: I don't think this recycling index thing is used anymore.
      if (_recycleIndex < _cells.Count)
      {
        DataGridCellContainer cell = _cells[_recycleIndex];
        _recycleIndex++;
        return cell;
      }
      //bool isExpandable = _parent != null && _parent.DisplayedItemsSource.HasHierarchy && _parent.EffectiveColumns.Count > 0 && _parent.EffectiveColumns[0] == item;
      DataGridCellContainer c = new DataGridCellContainer();
      c.Row = this;
      _cells.Add(c);
      _recycleIndex++;
      return c;
    }

    internal void PrepareContainer(DependencyObject element, object item)
    {
      PrepareContainerForItemOverride(element, item);
    }

    internal void ClearContainer(DependencyObject element, object item)
    {
      ClearContainerForItemOverride(element, item);
    }

    /*/// <summary>
    /// Gets an item container that can be displayed by this <see cref="DataGridRow"/>.
    /// </summary>
    /// <returns>A new <see cref="DataGridCellContainer"/>.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      // TODO: I don't think this recycling index thing is used anymore.
      if (_recycleIndex < _cells.Count)
      {
        DataGridCellContainer cell = _cells[_recycleIndex];
        _recycleIndex++;
        return cell;
      }
      DataGridCellContainer c = new DataGridCellContainer();
      c.Row = this;
      _cells.Add(c);
      _recycleIndex++;
      return c;
    }*/

    private DataGridItemWrapper _itemWrapper;

    internal DataGridItemWrapper ItemWrapper
    {
      get { return _itemWrapper; }
      set
      {
        _itemWrapper = value;
        if (_itemWrapper == null)
        {
          IsExpanded = false;
          HierarchyLevel = 0;
          HasChildren = false;
        }
        else
        {
          IsExpanded = _itemWrapper.IsExpanded;
          HierarchyLevel = _itemWrapper.Level;
          HasChildren = _itemWrapper.HasChildren;
        }
      }
    }

    /// <summary>
    /// Prepares the given container for displaying the given item.
    /// </summary>
    /// <param name="element">The <see cref="DataGridCellContainer"/> to prepare.</param>
    /// <param name="item">The item to be displayed by the given cell container.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridCellContainer cell = element as DataGridCellContainer;
      DataGridColumn column = item as DataGridColumn;

      if (ItemContainerStyle != null && cell != null)
      {
        cell.Style = ItemContainerStyle;
      }

      cell.Content = Content; // TODO: Binding?
      if (column.DisplayMemberBinding != null)
      {
        cell.DataContext = Content;
      }
      cell.Column = column; // TODO: Binding?

      bool isExpandable = _parent != null && _parent.DisplayedItemsSource != null && _parent.DisplayedItemsSource.HasHierarchy && _parent.EffectiveColumns.Count > 0 && _parent.EffectiveColumns[0] == item;
      cell.SetIsExpandableCell(isExpandable);
      /*if (ItemWrapper != null)
      {
        cell.SetHasChildren(ItemWrapper.HasChildren);
        cell.IsExpanded = ItemWrapper.IsExpanded;
        cell.SetLevel(ItemWrapper.Level);
      }
      else
      {
        cell.SetHasChildren(false);
        cell.IsExpanded = false;
        cell.SetLevel(0);
      }*/

      //BindingOperations.SetBinding(cell, DataGridCellContainer.WidthProperty, new Binding { Source = column, Path = new PropertyPath("Width") });
      // TODO: these should probably be bindings:
      if (column.DisplayTemplateSelector != null)
      {
        cell.ContentTemplate = null;
        cell.ContentTemplateSelector = column.DisplayTemplateSelector;
      }
      else
      {
        cell.ContentTemplateSelector = null;
        cell.ContentTemplate = column.DisplayTemplate;
      }

      cell.SetIsEditing(column.IsAlwaysInEditMode);

      // Restore state information that may have been destroyed by virtualization:
      RestoreState(cell);
    }

    private void RestoreState(DataGridCellContainer cell)
    {
      cell.SetIsHighlighted(_parent.IsHighlightedCell(cell));
      cell.IsValid = true; // TODO: restore validation state properly.
      // Restore selection state using the SelectedCellMap:
      IList<DataGridCell> cells;
      _parent.SelectedCellMap.TryGetValue(Content, out cells);
      cell.IsSelected = false;
      if (cells != null)
      {
        foreach (DataGridCell selectedCell in cells) // TODO: experiment with using a hash set to store the selected cells on a row rather than a list.
        {
          if (selectedCell.Column == cell.Column)
          {
            cell.IsSelected = true;
            break;
          }
        }
      }
      if (_parent != null)
      {
        DataGridCell cellModel = cell.DataGridCell;
        cell.Background = _parent.GetCellBackground(cellModel);
        //cell.Foreground = cellModel.Column == null ? Brushes.Black : cellModel.Column.Foreground;
        if (cellModel.Column != null && cellModel.Column.Foreground != null)
        {
          cell.Foreground = cellModel.Column.Foreground;
        }
      }
    }

    /// <summary>
    /// Clears the given item container.
    /// </summary>
    /// <param name="element">The item container to clear.</param>
    /// <param name="item">The item being displayed by the given item container.</param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridCellContainer cell = element as DataGridCellContainer;
      BindingOperations.ClearBinding(cell, DataGridCellContainer.WidthProperty);
    }

    internal void ClearCells()
    {
      if (Panel != null)
      {
        foreach (UIElement element in Panel.Children)
        {
          DataGridCellContainer cell = element as DataGridCellContainer;
          if (cell != null)
          {
            cell.Content = null;
          }
        }
      }
    }

    /// <summary>
    /// Returns whether or not the given item can be used as its own item container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the given item can be used as its own item container. False otherwise.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is DataGridCellContainer;
    }

    private IList<DataGridCellContainer> _cells = new List<DataGridCellContainer>();
    private int _recycleIndex;

    internal void Recycle()
    {
      _recycleIndex = 0;
    }

    internal DataGridCellContainer GetCell(DataGridColumn column)
    {
      return Panel == null ? null : Panel.GetElement(column) as DataGridCellContainer;
    }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }
}
