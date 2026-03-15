using Infralution.Licensing;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfDataGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.Data;
using System.Reflection;

using DataGridColumn = Mindscape.WpfElements.WpfDataGrid.DataGridColumn;
using DataGridRow = Mindscape.WpfElements.WpfDataGrid.DataGridRow;
using DataGridCell = Mindscape.WpfElements.WpfDataGrid.DataGridCell;
using DataGridRowEventArgs = Mindscape.WpfElements.WpfDataGrid.DataGridRowEventArgs;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that displays a grid of data that can be edited by a user.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DataGrid : ItemsControl, IExtendInPlaceEditors, IProvideEditorHosting
  {
    private DataGridPanel _dataGridPanel;

    private bool _unloaded;
    private readonly DispatcherTimer _smartScrollTimer = new DispatcherTimer();

    static DataGrid()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGrid"/> class.
    /// </summary>
    public DataGrid()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      _editors = new EditorCollection(this);
      _editorSelector = new EditorSelector(this);

      //ObservableCollection<object> selectedItems = new ObservableCollection<object>();
      //SetValue(SelectedItemsPropertyKey, selectedItems);
      INotifyCollectionChanged notify = _selectedItems as INotifyCollectionChanged;
      if (notify != null)
      {
        notify.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
      }

      _selectedCells.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedCells_CollectionChanged);

      BindCommands();

      Columns = new ObservableCollection<DataGridColumn>();
      Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      Loaded += OnLoaded;
      Unloaded += new RoutedEventHandler(DataGrid_Unloaded);

      ObservableCollection<DataGridColumn> groupedColumns = new ObservableCollection<DataGridColumn>();
      groupedColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
      SetValue(GroupedColumnsPropertyKey, groupedColumns);

      /*if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        IList data = new List<object>();
        data.Add(new SampleObject() { Property1 = "Value 1", Property2 = "Value 2" });
        data.Add(new SampleObject() { Property1 = "Value 1", Property2 = "Value 2" });
        ItemsSource = data;
      }*/
      AddHandler(Control.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
      SizeChanged += new SizeChangedEventHandler(DataGrid_SizeChanged);
      IsTabStop = false;

      _smartScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
      _smartScrollTimer.Tick += new EventHandler(SmartScrollTimer_Tick);
    }

    private void SmartScrollTimer_Tick(object sender, EventArgs e)
    {
      if (DataGridPanel != null && Mouse.LeftButton == MouseButtonState.Pressed)
      {
        Point position = Mouse.GetPosition(this);
        bool scrolled = false;
        if (position.Y > ActualHeight)
        {
          double diff = position.Y - ActualHeight;
          int speed = (int)Math.Ceiling(diff / 10.0);
          DataGridPanel.SetVerticalOffset(DataGridPanel.VerticalOffset + speed);
          scrolled = true;
        }
        else if (position.Y < 0)
        {
          double diff = -position.Y;
          int speed = (int)Math.Ceiling(diff / 10.0);
          DataGridPanel.SetVerticalOffset(DataGridPanel.VerticalOffset - speed);
          scrolled = true;
        }

        if (position.X < 0)
        {
          double diff = -position.X;
          int speed = (int)Math.Ceiling(diff / 10.0) * 20;
          DataGridPanel.SetHorizontalOffset(DataGridPanel.HorizontalOffset - speed);
          scrolled = true;
        }
        else if (position.X > ActualWidth)
        {
          double diff = position.X - ActualWidth;
          int speed = (int)Math.Ceiling(diff / 10.0) * 20;
          DataGridPanel.SetHorizontalOffset(DataGridPanel.HorizontalOffset + speed);
          scrolled = true;
        }

        if (scrolled)
        {
          if (_previousMouseEventArgs != null)
          {
            OnMouseMove(_previousMouseEventArgs);
          }
        }
      }
    }

    private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.InvalidateRequiresMeasure();
        // TODO: When frozen columns are present, scrolling to the far right, and then maximizing causes a space to appear after the last column. Using a Dispatcher here solves this problem.
        //       Need to find a way to do this better. The filter row is still broken in this scenario.
        Dispatcher.BeginInvoke(new Action(InvalidateRemeasure), DispatcherPriority.ApplicationIdle);
      }
    }

    private void InvalidateRemeasure()
    {
      DataGridPanel.UpdateFirstVisibleColumnIndex();
      DataGridPanel.InvalidateRequiresMeasure();
    }

    private void DataGrid_Unloaded(object sender, RoutedEventArgs e)
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.PageIndexChanged -= new EventHandler(DataGridItemsSource_PageIndexChanged);
        DisplayedItemsSource.CollectionUpdated -= new EventHandler(DataGridItemsSource_CollectionUpdated);
        DisplayedItemsSource.RowIsExpandedChanged -= new EventHandler<RowIsExpandedChangedEventArgs>(DataGridItemsSource_RowIsExpandedChanged);
        DisplayedItemsSource.Destroy();
      }
      INotifyCollectionChanged notifyer = ItemsSource as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = ItemsSource as IBindingList;
        if (bindingList != null)
        {
          bindingList.ListChanged -= new ListChangedEventHandler(BindingList_ListChanged);
        }
      }

      CollectionView view = ItemsSource as CollectionView;
      if (view != null)
      {
        view.CurrentChanged -= new EventHandler(CollectionView_CurrentChanged);
      }

      foreach (DataGridColumn column in EffectiveColumns)
      {
        column.SortDirectionChanged -= new EventHandler(DataGridColumn_SortDirectionChanged);
        column.FilterChanged -= new EventHandler(DataGridColumn_FilterChanged);
        column.AllowEditingChanged -= new EventHandler(DataGridColumn_AllowEditingChanged);
        column.BackgroundChanged -= new EventHandler(DataGridColumn_BackgroundChanged);
        column.ForegroundChanged -= new EventHandler(DataGridColumn_ForegroundChanged);
        column.IsVisibleChanged -= new EventHandler(DataGridColumn_IsVisibleChanged);
        column.TemplateChanged -= new EventHandler(DataGridColumn_TemplateChanged);
      }
      _unloaded = true;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      //Debug.WriteLine("Key Down");
      if (IsEditModeArrowKeyNavigationEnabled)
      {
        if (HighlightedCellContainer != null && HighlightedCellContainer.IsEditing && !HighlightedCellContainer.WasDoubleClicked && !HighlightedCellContainer.SelectionChanged)
        {
          //Debug.WriteLine("Passed");
          switch (e.Key)
          {
            case Key.Up:
              if (CanHighlightUp)
              {
                HighlightUp();
              }
              break;
            case Key.Down:
              if (CanHighlightDown)
              {
                HighlightDown();
              }
              break;
            case Key.Left:
              if (CanHighlightLeft)
              {
                HighlightPrevious(false);
              }
              break;
            case Key.Right:
              if (CanHighlightRight)
              {
                HighlightNext(false);
              }
              break;
          }
        }
      }
    }

    private readonly Dictionary<GroupDescription, DataGridColumn> _groupDescriptionMap = new Dictionary<GroupDescription, DataGridColumn>();

    private void GroupedColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateGrouping();
    }

    private void UpdateGrouping()
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.Begin();
        DisplayedItemsSource.GroupDescriptions.Clear();
        _groupDescriptionMap.Clear();
        foreach (DataGridColumn column in GroupedColumns)
        {
          GroupDescription groupDescription = new PropertyGroupDescription(column.PropertyInfo.Name);
          if (column.PropertyInfo is DataTablePropertyInfoAdapter)
          {
            groupDescription = new PropertyGroupDescription(column.PropertyInfo.Name);
          }
          _groupDescriptionMap[groupDescription] = column;
          DisplayedItemsSource.GroupDescriptions.Add(groupDescription);
        }
        DisplayedItemsSource.End();
      }
    }

    /// <summary>
    /// Called when a dependancy property is changed.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);

      if ("IsVisible".Equals(e.Property.Name))
      {
        InvalidateHeaderRowMeasure();
        if (DataGridPanel != null)
        {
          DataGridPanel.InvalidateRequiresMeasure();
        }
      }
    }

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      RecalculateEffectiveColumns();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (_unloaded)
      {
        DataGridItemsSource itemsSource = new DataGridItemsSource(ItemsSource ?? new List<object>(), ItemTemplate, HierarchyMode);
        itemsSource.ObjectBuilder = ObjectBuilder;
        itemsSource.PageSize = PageSize;
        itemsSource.GroupMap = _groupDescriptionMap;
        if (ItemsSource != null)
        {
          itemsSource.PageIndexChanged += new EventHandler(DataGridItemsSource_PageIndexChanged);
          itemsSource.CollectionUpdated += new EventHandler(DataGridItemsSource_CollectionUpdated);
          itemsSource.RowIsExpandedChanged += new EventHandler<RowIsExpandedChangedEventArgs>(DataGridItemsSource_RowIsExpandedChanged);
        }
        itemsSource.AllowUserToAddRows = AllowUserToAddRows;
        SetValue(DisplayedItemsSourcePropertyKey, itemsSource);

        INotifyCollectionChanged notifyer = ItemsSource as INotifyCollectionChanged;
        if (notifyer != null)
        {
          notifyer.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
        }
        else
        {
          IBindingList bindingList = ItemsSource as IBindingList;
          if (bindingList != null)
          {
            bindingList.ListChanged += new ListChangedEventHandler(BindingList_ListChanged);
          }
        }

        CollectionView view = ItemsSource as CollectionView;
        if (view != null)
        {
          view.CurrentChanged += new EventHandler(CollectionView_CurrentChanged);
        }
        _unloaded = false;
      }

      if (SelectedItems == _selectedItems && BindingOperations.GetBinding(this, SelectedItemsProperty) == null)
      {
        SelectedItems = _selectedItems;
      }

      RecalculateEffectiveColumns();

      _dataGridPanel = VisualTreeUtils.GetChild<DataGridPanel>(this);
      _headerRowPresenter = VisualTreeUtils.GetChild<DataGridHeaderRowPresenter>(this);

      UpdateGrouping();

      /*if (_dataGridPanel != null)
      {
        _dataGridPanel.DataGrid = this;
      }*/
    }

    /// <summary>
    /// Called when the <see cref="DataGrid"/> loses keyboard focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnLostKeyboardFocus(e);
      if (!IsKeyboardFocusWithin)
      {
        DataGridCellContainer cell = HighlightedCellContainer;
        if (cell != null && cell.IsEditing)
        {
          cell.SetIsEditing(false);
        }
      }
    }

    private DataGridHeaderRowPresenter _headerRowPresenter;

    internal void MeasureHeaderRowPresenter()
    {
      if (_headerRowPresenter != null)
      {
        DataGridRowPanel panel = VisualTreeUtils.GetChild<DataGridRowPanel>(_headerRowPresenter);
        if (panel != null)
        {
          panel.InvalidateMeasure();
          panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        }
      }
    }

    internal void InvalidateHeaderRowMeasure()
    {
      if (_headerRowPresenter != null)
      {
        DataGridRowPanel panel = VisualTreeUtils.GetChild<DataGridRowPanel>(_headerRowPresenter);
        if (panel != null)
        {
          panel.InvalidateMeasure();
        }
      }
    }

    #region Commands

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(DataGridCommands.NextPage, NextPage_Executed, NextPage_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.PreviousPage, PreviousPage_Executed, PreviousPage_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.FirstPage, FirstPage_Executed, FirstPage_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.LastPage, LastPage_Executed, LastPage_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightUp, HighlightUp_Executed, HighlightUp_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightDown, HighlightDown_Executed, HighlightDown_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightRight, HighlightRight_Executed, HighlightRight_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightLeft, HighlightLeft_Executed, HighlightLeft_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightLeftWithWrapping, HighlightLeftWithWrapping_Executed, HighlightLeftWithWrapping_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.HighlightRightWithWrapping, HighlightRightWithWrapping_Executed, HighlightRightWithWrapping_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.SelectUp, SelectUp_Executed, SelectUp_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.SelectDown, SelectDown_Executed, SelectDown_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.StartOfRow, StartOfRow_Executed, StartOfRow_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.StartOfGrid, StartOfGrid_Executed, StartOfGrid_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.EndOfRow, EndOfRow_Executed, EndOfRow_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.EndOfGrid, EndOfGrid_Executed, EndOfGrid_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.PageUp, PageUp_Executed, PageUp_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.PageDown, PageDown_Executed, PageDown_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.UngroupColumn, UngroupColumn_Executed, UngroupColumn_CanExecute));

      CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, SelectAll_Executed, SelectAll_CanExecute));
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.ToggleSelectAll, ToggleSelectAll_Executed, ToggleSelectAll_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.EnterEditMode, EnterEditMode_Executed, EnterEditMode_CanExecute));
      CommandBindings.Add(new CommandBinding(DataGridCommands.CancelEditMode, CancelEditMode_Executed, CancelEditMode_CanExecute));

      CommandBindings.Add(new CommandBinding(DataGridCommands.RemoveFilter, RemoveFilter_Executed, RemoveFilter_CanExecute));

      InputBindings.Add(new InputBinding(ApplicationCommands.SelectAll, new KeyGesture(Key.A, ModifierKeys.Control)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightUp, new KeyGesture(Key.Up)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightDown, new KeyGesture(Key.Down)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightUp, new KeyGesture(Key.Enter, ModifierKeys.Shift)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightDown, new KeyGesture(Key.Enter)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightRight, new KeyGesture(Key.Right)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightLeft, new KeyGesture(Key.Left)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightLeftWithWrapping, new KeyGesture(Key.Tab, ModifierKeys.Shift)));
      InputBindings.Add(new InputBinding(DataGridCommands.HighlightRightWithWrapping, new KeyGesture(Key.Tab)));
      
      InputBindings.Add(new InputBinding(DataGridCommands.SelectUp, new KeyGesture(Key.Up, ModifierKeys.Shift)));
      InputBindings.Add(new InputBinding(DataGridCommands.SelectDown, new KeyGesture(Key.Down, ModifierKeys.Shift)));

      InputBindings.Add(new InputBinding(DataGridCommands.StartOfRow, new KeyGesture(Key.Home)));
      InputBindings.Add(new InputBinding(DataGridCommands.StartOfGrid, new KeyGesture(Key.Home, ModifierKeys.Control)));
      InputBindings.Add(new InputBinding(DataGridCommands.EndOfRow, new KeyGesture(Key.End)));
      InputBindings.Add(new InputBinding(DataGridCommands.EndOfGrid, new KeyGesture(Key.End, ModifierKeys.Control)));
      InputBindings.Add(new InputBinding(DataGridCommands.PageUp, new KeyGesture(Key.PageUp)));
      InputBindings.Add(new InputBinding(DataGridCommands.PageDown, new KeyGesture(Key.PageDown)));

      InputBindings.Add(new InputBinding(DataGridCommands.EnterEditMode, new KeyGesture(Key.F2)));
      InputBindings.Add(new InputBinding(DataGridCommands.CancelEditMode, new KeyGesture(Key.Escape)));

      InputBindings.Add(new InputBinding(ApplicationCommands.Copy, new KeyGesture(Key.C, ModifierKeys.Control)));
    }

    // TODO: can write a test for this
    // TODO: it's possible to have multiple gestures for a single command. e.g. HighlightUp. This method should remove them all for the given command.
    /// <summary>
    /// Removes the input binding for the given command.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> for finding the input binding to remove.</param>
    public void RemoveInputBinding(ICommand command)
    {
      int index = 0;
      foreach (InputBinding binding in InputBindings)
      {
        if (binding.Command == command)
        {
          InputBindings.RemoveAt(index);
          return;
        }
        index++;
      }
    }

    /// <summary>
    /// Removes the input binding for the given input gesture.
    /// </summary>
    /// <param name="gesture">The <see cref="InputGesture"/> for finding the input binding to remove.</param>
    public void RemoveInputBinding(InputGesture gesture)
    {
      int index = 0;
      foreach (InputBinding binding in InputBindings)
      {
        if (binding.Gesture.Equals(gesture))
        {
          InputBindings.RemoveAt(index);
          return;
        }
        index++;
      }
    }

    #region NextPage Command

    private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanGoToNextPage)
      {
        PageIndex++;
      }
    }

    private void NextPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGoToNextPage;
    }

    private bool CanGoToNextPage { get { return DisplayedItemsSource != null && PageIndex != DisplayedItemsSource.PageCount - 1; } }

    #endregion // NextPage Command

    #region PreviousPage Command

    private void PreviousPage_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanGoToPreviousPage)
      {
        PageIndex--;
      }
    }

    private void PreviousPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGoToPreviousPage;
    }

    private bool CanGoToPreviousPage { get { return DisplayedItemsSource != null && PageIndex != 0; } }

    #endregion // PreviousPage Command

    #region FirstPage Command

    private void FirstPage_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanGoToFirstPage)
      {
        PageIndex = 0;
      }
    }

    private void FirstPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGoToFirstPage;
    }

    private bool CanGoToFirstPage { get { return DisplayedItemsSource != null && PageIndex != 0; } }

    #endregion // FirstPage Command

    #region LastPage Command

    private void LastPage_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanGoToLastPage)
      {
        PageIndex = DisplayedItemsSource.PageCount - 1;
      }
    }

    private void LastPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanGoToLastPage;
    }

    private bool CanGoToLastPage { get { return DisplayedItemsSource != null && PageIndex != DisplayedItemsSource.PageCount - 1; } }

    #endregion // LastPage Command

    #region HighlightUp Command

    private void HighlightUp_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanHighlightUp)
      {
        HighlightUp();
      }
    }

    private void HighlightUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightUp;
    }

    private bool CanHighlightUp
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && (HighlightedCell.RowIndex > 0 || (HighlightedCellContainer != null && HighlightedCellContainer.IsEditing));
      }
    }

    #endregion // HighlightUp Command

    #region HighlightDown Command

    private void HighlightDown_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanHighlightDown)
      {
        HighlightDown();
      }
    }

    private void HighlightDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightDown;
    }

    private bool CanHighlightDown
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && (HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1 || (HighlightedCellContainer != null && HighlightedCellContainer.IsEditing));
      }
    }

    #endregion // HighlightDown Command

    #region HighlightRight Command

    private void HighlightRight_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanHighlightRight)
      {
        HighlightNext(false);
      }
    }

    private void HighlightRight_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightRight;
    }

    private bool CanHighlightRight
    {
      get
      {
        if (HighlightedCell == null || HighlightedCellContainer == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && highlightedColumnIndex < EffectiveColumns.Count - 1;
      }
    }

    #endregion // HighlightRight Command

    #region HighlightLeft Command

    private void HighlightLeft_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanHighlightLeft)
      {
        HighlightPrevious(false);
      }
    }

    private void HighlightLeft_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightLeft;
    }

    private bool CanHighlightLeft
    {
      get
      {
        if (HighlightedCell == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && highlightedColumnIndex > 0;
      }
    }

    #endregion // HighlightLeft Command

    #region HighlightLeftWithWrapping Command

    private void HighlightLeftWithWrapping_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (HighlightedCell != null && HighlightedCellContainer != null)
      {
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        if (!(highlightedColumnIndex > 0 || HighlightedCell.RowIndex > 0))
        {
          HighlightedCellContainer.SetIsEditing(false);
          return;
        }
      }

      if (CanHighlightLeftWithWrapping)
      {
        HighlightPrevious(true);
      }
    }

    private void HighlightLeftWithWrapping_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightLeftWithWrapping;
      e.ContinueRouting = !e.CanExecute;
    }

    private bool CanHighlightLeftWithWrapping
    {
      get
      {
        if (HighlightedCell == null || HighlightedCellContainer == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && (highlightedColumnIndex > 0 || HighlightedCell.RowIndex > 0 || HighlightedCellContainer.IsEditing);
      }
    }

    #endregion // HighlightLeftWithWrapping Command

    #region HighlightRightWithWrapping Command

    private void HighlightRightWithWrapping_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (HighlightedCell != null && HighlightedCellContainer != null)
      {
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        if (!(highlightedColumnIndex < EffectiveColumns.Count - 1 || HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1))
        {
          HighlightedCellContainer.SetIsEditing(false);
          return;
        }
      }

      if (CanHighlightRightWithWrapping)
      {
        HighlightNext(true);
      }
    }

    private void HighlightRightWithWrapping_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanHighlightRightWithWrapping;
      e.ContinueRouting = !e.CanExecute;
    }

    private bool CanHighlightRightWithWrapping
    {
      get
      {
        if (HighlightedCell == null || HighlightedCellContainer == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && (highlightedColumnIndex < EffectiveColumns.Count - 1 || HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1 || HighlightedCellContainer.IsEditing);
      }
    }

    #endregion // HighlightRight Command

    #region StartOfRow Command

    private void StartOfRow_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanStartOfRow)
      {
        NavigateToStartOfRow();
      }
    }

    private void StartOfRow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanStartOfRow;
    }

    private bool CanStartOfRow
    {
      get
      {
        if (HighlightedCell == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && highlightedColumnIndex > 0;
      }
    }

    #endregion // StartOfRow Command

    #region StartOfGrid Command

    private void StartOfGrid_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanStartOfGrid)
      {
        NavigateToStartOfGrid();
      }
    }

    private void StartOfGrid_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanStartOfGrid;
    }

    private bool CanStartOfGrid
    {
      get
      {
        if (HighlightedCell == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && (highlightedColumnIndex > 0 || HighlightedCell.RowIndex != 0);
      }
    }

    #endregion // StartOfGrid Command

    #region EndOfRow Command

    private void EndOfRow_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanEndOfRow)
      {
        NavigateToEndOfRow();
      }
    }

    private void EndOfRow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanEndOfRow;
    }

    private bool CanEndOfRow
    {
      get
      {
        if (HighlightedCell == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && highlightedColumnIndex < EffectiveColumns.Count - 1;
      }
    }

    #endregion // EndOfRow Command

    #region EndOfGrid Command

    private void EndOfGrid_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanEndOfGrid)
      {
        NavigateToEndOfGrid();
      }
    }

    private void EndOfGrid_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanEndOfGrid;
    }

    private bool CanEndOfGrid
    {
      get
      {
        if (HighlightedCell == null)
        {
          return false;
        }
        int highlightedColumnIndex = EffectiveColumns.IndexOf(HighlightedCell.Column); // TODO: this can be improved if columns store their index.
        return DisplayedItemsSource != null && (highlightedColumnIndex < EffectiveColumns.Count - 1 || HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1);
      }
    }

    #endregion // EndOfGrid Command

    #region PageUp Command

    private void PageUp_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanPageUp)
      {
        PageUp();
      }
    }

    private void PageUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanPageUp;
    }

    private bool CanPageUp
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && HighlightedCell.RowIndex > 0;
      }
    }

    #endregion // PageUp Command

    #region PageDown Command

    private void PageDown_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanPageDown)
      {
        PageDown();
      }
    }

    private void PageDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanPageDown;
    }

    private bool CanPageDown
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1;
      }
    }

    #endregion // PageDown Command

    #region Highlight command logic

    /*private bool IsHighlightedCellValid
    {
      get
      {
        DataGridCellContainer highlightedCellContainer = HighlightedCellContainer;
        if (HighlightedCell == null || (highlightedCellContainer != null && !highlightedCellContainer.IsValid))
        {
          return false;
        }
        return true;
      }
    }*/

    private void AttemptToSwitchHighlightedCellToEditMode()
    {
      DataGridCellContainer highlightedCellContainer = HighlightedCellContainer;
      if (highlightedCellContainer != null)
      {
        highlightedCellContainer.SetIsEditing(true);
      }
      else
      {
        SwitchHighlightedCellToEditModeLater();
      }
    }

    // TODO: need to skip invisible columns when checking if the highlighted cell is at the first or last column.
    // TODO: need to skip invisible columns when putting cell at start/end of the row or grid.

    private int GetNextVisibleColumn(int current, bool checkTabStop)
    {
      current++;
      while (current < EffectiveColumns.Count)
      {
        DataGridColumn column = EffectiveColumns[current];
        if (column.IsVisible && (!checkTabStop || KeyboardNavigation.GetIsTabStop(column)))
        {
          return current;
        }
        current++;
      }
      return EffectiveColumns.Count;
    }

    private int GetPreviousVisibleColumn(int current, bool checkTabStop)
    {
      current--;
      while (current >= 0)
      {
        DataGridColumn column = EffectiveColumns[current];
        if (column.IsVisible && (!checkTabStop || KeyboardNavigation.GetIsTabStop(column)))
        {
          return current;
        }
        current--;
      }
      return -1;
    }

    private void HighlightNext(bool allowWrap)
    {
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      // TODO: bug in the isEditing logic. It will only be maintained if the cell being edited is currently within the viewport.
      // Infact the IsEdited property of a cell also needs to be maintained in and out of virtualization.
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      int highlightedColumnIndex = EffectiveColumns.IndexOf(highlightedCellModel.Column); // TODO: this can be improved if columns store their index.

      highlightedColumnIndex = GetNextVisibleColumn(highlightedColumnIndex, allowWrap);

      if (highlightedColumnIndex < EffectiveColumns.Count)
      {
        //highlightedColumnIndex++;
        //highlightedColumnIndex = GetNextVisibleColumn(highlightedColumnIndex, allowWrap);
        DataGridCell cellModel = new DataGridCell(highlightedCellModel.RowContent, EffectiveColumns[highlightedColumnIndex]) { RowIndex = highlightedCellModel.RowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
      else if (allowWrap)
      {
        int highlightedRowIndex = highlightedCellModel.RowIndex;
        if (highlightedRowIndex < DisplayedItemsSource.CountOnCurrentPage - 1)
        {
          highlightedRowIndex++;
          highlightedColumnIndex = GetNextVisibleColumn(-1, allowWrap);
          DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(highlightedRowIndex), EffectiveColumns[highlightedColumnIndex]) { RowIndex = highlightedRowIndex };
          //HighlightedCell = cellModel;
          ChangeHighlightedCell(cellModel);
        }
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation && HighlightedCellContainer != null && HighlightedCellContainer.Column != null && HighlightedCellContainer.Column.AllowEditing)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
    }

    private void HighlightPrevious(bool allowWrap)
    {
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      int highlightedColumnIndex = EffectiveColumns.IndexOf(highlightedCellModel.Column); // TODO: this can be improved if columns store their index.

      highlightedColumnIndex = GetPreviousVisibleColumn(highlightedColumnIndex, allowWrap);

      if (highlightedColumnIndex >= 0)
      {
        //highlightedColumnIndex--;
        //highlightedColumnIndex = GetPreviousVisibleColumn(highlightedColumnIndex, allowWrap);
        DataGridCell cellModel = new DataGridCell(highlightedCellModel.RowContent, EffectiveColumns[highlightedColumnIndex]) { RowIndex = highlightedCellModel.RowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
      else if (allowWrap)
      {
        int highlightedRowIndex = highlightedCellModel.RowIndex;
        if (highlightedRowIndex > 0)
        {
          highlightedRowIndex--;
          highlightedColumnIndex = GetPreviousVisibleColumn(EffectiveColumns.Count, allowWrap);
          DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(highlightedRowIndex), EffectiveColumns[highlightedColumnIndex]) { RowIndex = highlightedRowIndex };
          //HighlightedCell = cellModel;
          ChangeHighlightedCell(cellModel);
        }
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation && HighlightedCellContainer != null && HighlightedCellContainer.Column != null && HighlightedCellContainer.Column.AllowEditing)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
    }

    private void HighlightDown()
    {
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      if (isEditing && HighlightedCell.RowIndex >= DisplayedItemsSource.CountOnCurrentPage - 1)
      {
        highlightedCell.SetIsEditing(false);
        Focus();
        return;
      }
      int highlightedRowIndex = highlightedCellModel.RowIndex;
      int newHighlightedRowIndex = Math.Max(0, Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, highlightedRowIndex));
      if (newHighlightedRowIndex < DisplayedItemsSource.CountOnCurrentPage - 1)
      {
        newHighlightedRowIndex++;
        if (SkipGroupHeadersWhenNavigating)
        {
          while (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup && newHighlightedRowIndex < DisplayedItemsSource.CountOnCurrentPage - 1)
          {
            newHighlightedRowIndex++;
          }
        }
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex), highlightedCellModel.Column) { RowIndex = newHighlightedRowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation && HighlightedCellContainer != null && HighlightedCellContainer.Column != null && HighlightedCellContainer.Column.AllowEditing)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
    }

    private void HighlightUp()
    {
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      if (isEditing && HighlightedCell.RowIndex <= 0)
      {
        highlightedCell.SetIsEditing(false);
        Focus();
        return;
      }
      int highlightedRowIndex = highlightedCellModel.RowIndex;
      int currentHighlightedRowIndex = highlightedRowIndex;
      if (highlightedRowIndex > 0)
      {
        highlightedRowIndex--;

        int newHighlightedRowIndex = Math.Max(0, Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, highlightedRowIndex));
        if (SkipGroupHeadersWhenNavigating)
        {
          while (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup && newHighlightedRowIndex > 0)
          {
            newHighlightedRowIndex--;
          }
          if (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup)
          {
            newHighlightedRowIndex = currentHighlightedRowIndex;
          }
        }
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex), highlightedCellModel.Column) { RowIndex = newHighlightedRowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation && HighlightedCellContainer != null && HighlightedCellContainer.Column != null && HighlightedCellContainer.Column.AllowEditing)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
    }

    // TODO: Shift+Arrow key selection logic:

    private object _selectedItem;
    private bool _selectedItemLock = false;

    private void SelectDown()
    {
      _selectedItemLock = true;
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      int highlightedRowIndex = highlightedCellModel.RowIndex;
      int newHighlightedRowIndex = Math.Max(0, Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, highlightedRowIndex));
      if (newHighlightedRowIndex < DisplayedItemsSource.CountOnCurrentPage - 1)
      {
        newHighlightedRowIndex++;
        if (SkipGroupHeadersWhenNavigating)
        {
          while (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup && newHighlightedRowIndex < DisplayedItemsSource.CountOnCurrentPage - 1)
          {
            newHighlightedRowIndex++;
          }
        }
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex), highlightedCellModel.Column) { RowIndex = newHighlightedRowIndex };

        int rowIndex = -1;
        DataGridColumn column = null;
        DataGridCell selectedCell = SelectedCell;
        if (selectedCell != null && _selectedItem != null)
        {
          rowIndex = DisplayedItemsSource.IndexOf(_selectedItem);
          column = selectedCell.Column;
        }
        else if (_selectedItem != null && EffectiveColumns != null && EffectiveColumns.Count > 0)
        {
          rowIndex = DisplayedItemsSource.IndexOf(_selectedItem);
          column = EffectiveColumns[0];
        }
        if (column == null || rowIndex < 0)
        {
          _selectedItemLock = false;
          return;
        }
        int startRowIndex = rowIndex;
        int startColumnIndex = EffectiveColumns.IndexOf(column);
        if (startRowIndex >= 0)
        {
          if (!KeyboardUtils.IsHoldingCtrl)
          {
            SelectedCells.Clear();
            SelectedItem = null;
            SelectedItems.Clear();
          }
          int endRowIndex = cellModel.RowIndex;
          if (endRowIndex < startRowIndex)
          {
            int temp = startRowIndex;
            startRowIndex = endRowIndex;
            endRowIndex = temp;
          }
          int endColumnIndex = EffectiveColumns.IndexOf(column);
          if (endColumnIndex < startColumnIndex)
          {
            int temp = startColumnIndex;
            startColumnIndex = endColumnIndex;
            endColumnIndex = temp;
          }
          for (int i = startRowIndex; i <= endRowIndex; i++)
          {
            /*for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
            {
              SelectedCells.Add(new DataGridCell(DisplayedItemsSource.GetItemAt_SortedItemsOnly(i), EffectiveColumns[columnIndex]));
            }*/
            object o = DisplayedItemsSource.GetItemAt(i);
            if (o != null)
            {
              SelectedItems.Add(o);
            }
          }
          //SelectedCell = selectedCell;
        }

        _ignoreSelectionUpdateForHighlightedCell = true;
        ChangeHighlightedCell(cellModel);
        _ignoreSelectionUpdateForHighlightedCell = false;
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
      _selectedItemLock = false;
    }

    private void SelectUp()
    {
      _selectedItemLock = true;
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      DataGridCell highlightedCellModel = HighlightedCell;
      bool isEditing = highlightedCell != null && highlightedCell.IsEditing; // TODO: this may be replaced with a data grid property.
      int highlightedRowIndex = highlightedCellModel.RowIndex;
      int currentHighlightedRowIndex = highlightedRowIndex;
      if (highlightedRowIndex > 0)
      {
        highlightedRowIndex--;

        int newHighlightedRowIndex = Math.Max(0, Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, highlightedRowIndex));
        if (SkipGroupHeadersWhenNavigating)
        {
          while (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup && newHighlightedRowIndex > 0)
          {
            newHighlightedRowIndex--;
          }
          if (DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex) is DataGridGroup)
          {
            newHighlightedRowIndex = currentHighlightedRowIndex;
          }
        }
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(newHighlightedRowIndex), highlightedCellModel.Column) { RowIndex = newHighlightedRowIndex };

        int rowIndex = -1;
        DataGridColumn column = null;
        DataGridCell selectedCell = SelectedCell;
        if (selectedCell != null && _selectedItem != null)
        {
          rowIndex = DisplayedItemsSource.IndexOf(_selectedItem);
          column = selectedCell.Column;
        }
        else if (_selectedItem != null && EffectiveColumns != null && EffectiveColumns.Count > 0)
        {
          rowIndex = DisplayedItemsSource.IndexOf(_selectedItem);
          column = EffectiveColumns[0];
        }
        if (column == null || rowIndex < 0)
        {
          _selectedItemLock = false;
          return;
        }
        int startRowIndex = rowIndex;
        int startColumnIndex = EffectiveColumns.IndexOf(column);
        if (startRowIndex >= 0)
        {
          if (!KeyboardUtils.IsHoldingCtrl)
          {
            SelectedCells.Clear();
            SelectedItem = null;
            SelectedItems.Clear();
          }
          int endRowIndex = cellModel.RowIndex;
          if (endRowIndex > startRowIndex)
          {
            int temp = startRowIndex;
            startRowIndex = endRowIndex;
            endRowIndex = temp;
          }
          int endColumnIndex = EffectiveColumns.IndexOf(column);
          if (endColumnIndex < startColumnIndex)
          {
            int temp = startColumnIndex;
            startColumnIndex = endColumnIndex;
            endColumnIndex = temp;
          }
          for (int i = startRowIndex; i >= endRowIndex; i--)
          {
            /*for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
            {
              SelectedCells.Add(new DataGridCell(DisplayedItemsSource.GetItemAt_SortedItemsOnly(i), EffectiveColumns[columnIndex]));
            }*/
            object o = DisplayedItemsSource.GetItemAt(i);
            if (o != null)
            {
              SelectedItems.Add(o);
            }
          }
          //SelectedCell = selectedCell;
        }

        _ignoreSelectionUpdateForHighlightedCell = true;
        ChangeHighlightedCell(cellModel);
        _ignoreSelectionUpdateForHighlightedCell = false;
      }
      if (isEditing && IsEditModeMaintainedDuringNavigation)
      {
        AttemptToSwitchHighlightedCellToEditMode();
      }
      else
      {
        Focus();
      }
      _selectedItemLock = false;
    }

    private void NavigateToStartOfRow()
    {
      if (EffectiveColumns.Count > 0)
      {
        DataGridCell highlightedCellModel = HighlightedCell;
        DataGridCell cellModel = new DataGridCell(highlightedCellModel.RowContent, EffectiveColumns[0]) { RowIndex = highlightedCellModel.RowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
    }

    private void NavigateToStartOfGrid()
    {
      if (EffectiveColumns.Count > 0 && DisplayedItemsSource.CountOnCurrentPage > 0)
      {
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(0), EffectiveColumns[0]) { RowIndex = 0 };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
    }

    private void NavigateToEndOfRow()
    {
      if (EffectiveColumns.Count > 0)
      {
        DataGridCell highlightedCellModel = HighlightedCell;
        DataGridCell cellModel = new DataGridCell(highlightedCellModel.RowContent, EffectiveColumns[EffectiveColumns.Count - 1]) { RowIndex = highlightedCellModel.RowIndex };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
    }

    private void NavigateToEndOfGrid()
    {
      if (EffectiveColumns.Count > 0 && DisplayedItemsSource.CountOnCurrentPage > 0)
      {
        DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(DisplayedItemsSource.CountOnCurrentPage - 1), EffectiveColumns[EffectiveColumns.Count - 1]) { RowIndex = DisplayedItemsSource.CountOnCurrentPage - 1 };
        //HighlightedCell = cellModel;
        ChangeHighlightedCell(cellModel);
      }
    }

    private void PageUp()
    {
      DataGridPanel.PageUp();
      DataGridCell highlightedCellModel = HighlightedCell;
      int highlightedRowIndex = highlightedCellModel.RowIndex;

      highlightedRowIndex = Math.Max(0, highlightedRowIndex - (int)DataGridPanel.ViewportHeight);
      DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(highlightedRowIndex), highlightedCellModel.Column) { RowIndex = highlightedRowIndex };
      _ignoreBringHighlightedCellIntoView = true;
      //HighlightedCell = cellModel;
      ChangeHighlightedCell(cellModel);
      _ignoreBringHighlightedCellIntoView = false;
      // TODO: might want to use a dispatcher to now bring cell into view for edge cases involving different sized rows.
    }

    private void PageDown()
    {
      DataGridPanel.PageDown();
      DataGridCell highlightedCellModel = HighlightedCell;
      int highlightedRowIndex = highlightedCellModel.RowIndex;

      highlightedRowIndex = Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, highlightedRowIndex + (int)DataGridPanel.ViewportHeight);
      DataGridCell cellModel = new DataGridCell(DisplayedItemsSource.GetItemOnCurrentPageAt(highlightedRowIndex), highlightedCellModel.Column) { RowIndex = highlightedRowIndex };
      _ignoreBringHighlightedCellIntoView = true;
      //HighlightedCell = cellModel;
      ChangeHighlightedCell(cellModel);
      _ignoreBringHighlightedCellIntoView = false;
      // TODO: might want to use a dispatcher to now bring cell into view for edge cases involving different sized rows.
    }

    private void UpdateSelectionAfterNavigatingHighlightedCell()
    {
      if (SelectionMode == SelectionMode.Extended || SelectionMode == SelectionMode.Single)
      {
        DataGridCell highlightedCellModel = HighlightedCell;
        if (highlightedCellModel != null)
        {
          if (SelectionType == DataGridSelectionType.Row || (SelectedItem != null && SelectionType == DataGridSelectionType.RowAndCell))
          {
            if (SelectionMode == SelectionMode.Extended)
            {
              SelectedItems.Clear();
            }
            SelectedItem = highlightedCellModel.RowContent;
          }
          else if (SelectionType == DataGridSelectionType.Cell || (SelectedCell != null && SelectionType == DataGridSelectionType.RowAndCell))
          {
            if (SelectionMode == SelectionMode.Extended)
            {
              SelectedCells.Clear();
            }
            SelectedCell = highlightedCellModel;
          }
        }
      }
    }

    private void SwitchHighlightedCellToEditModeLater()
    {
      Dispatcher.BeginInvoke(new Action(SwitchHighlightedCellToEditMode), DispatcherPriority.Render);
    }

    private void SwitchHighlightedCellToEditMode()
    {
      DataGridCellContainer highlightedCell = HighlightedCellContainer;
      if (highlightedCell != null)
      {
        highlightedCell.SetIsEditing(true);
      }
    }

    #endregion // Highlight command logic

    #region SelectAll Command

    private void UpdateCanSelectAll()
    {
      SetValue(CanSelectAllPropertyKey, ItemsSource != null && (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended));
    }

    private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectAll();
    }

    private void SelectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanSelectAll;
    }

    private bool _selectionEventLock;

    /// <summary>
    /// Selects all the items in this <see cref="DataGrid"/>.
    /// </summary>
    public void SelectAll()
    {
      if (CanSelectAll)
      {
        if (SelectionType == DataGridSelectionType.Cell)
        {
          int rowCount = DisplayedItemsSource.CountOnTotalActiveCollection;
          for (int i = 0; i < rowCount; i++)
          {
            object o = DisplayedItemsSource.GetItemAt(i);
            if (o != null)
            {
              foreach (DataGridColumn column in EffectiveColumns)
              {
                SelectedCells.Add(new DataGridCell(o, column));
              }
            }
          }
        }
        else
        {
          _selectionEventLock = true;
          SelectedItems.Clear();
          foreach (object o in ItemsSource)
          {
            SelectedItems.Add(o);
          }
          _selectionEventLock = false;
          RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, new List<object>(), SelectedItems));
        }
      }
    }

    /// <summary>
    /// Gets whether or not all the items in this <see cref="DataGrid"/> can be selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CanSelectAllProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CanSelectAll
    {
      get { return (bool)GetValue(CanSelectAllProperty); }
    }

    private static readonly DependencyPropertyKey CanSelectAllPropertyKey =
        DependencyProperty.RegisterReadOnly("CanSelectAll", typeof(bool), typeof(DataGrid), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="CanSelectAll"/> property.
    /// </summary>
    public static readonly DependencyProperty CanSelectAllProperty =
        CanSelectAllPropertyKey.DependencyProperty;

    #endregion // SelectAll Command

    #region ToggleSelectAll Command

    private void ToggleSelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_selectedItems.Count == DisplayedItemsSource.Count)
      {
        SelectedItem = null; // This also empties the SelectedItems list.
      }
      else
      {
        SelectAll();
      }
    }

    private void ToggleSelectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = IsToggleSelectAllButtonEnabled && ItemsSource != null && DisplayedItemsSource != null && (SelectionType == DataGridSelectionType.Row || SelectionType == DataGridSelectionType.RowAndCell) && (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended);
    }

    #endregion // ToggleSelectAll Command

    #region UngroupColumn Command

    private void UngroupColumn_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      DataGridColumn column = e.Parameter as DataGridColumn;
      if (column != null)
      {
        GroupedColumns.Remove(column);
      }
    }

    private void UngroupColumn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    #endregion // UngroupColumn Command

    #region SelectUp Command

    private void SelectUp_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanSelectUp)
      {
        SelectUp();
      }
    }

    private void SelectUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanSelectUp;
    }

    private bool CanSelectUp
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && HighlightedCell.RowIndex > 0 && SelectionMode != SelectionMode.Single;
      }
    }

    #endregion // SelectUp Command

    #region SelectDown Command

    private void SelectDown_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanSelectDown)
      {
        SelectDown();
      }
    }

    private void SelectDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanSelectDown;
    }

    private bool CanSelectDown
    {
      get
      {
        return HighlightedCell != null && DisplayedItemsSource != null && HighlightedCell.RowIndex < DisplayedItemsSource.CountOnCurrentPage - 1 && SelectionMode != SelectionMode.Single;
      }
    }

    #endregion // SelectDown Command

    #region EnterEditMode Command

    private void EnterEditMode_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanEnterEditMode)
      {
        HighlightedCellContainer.SelectionChanged = true;
        HighlightedCellContainer.SetIsEditing(true);
      }
    }

    private void EnterEditMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanEnterEditMode;
    }

    private bool CanEnterEditMode
    {
      get
      {
        return HighlightedCellContainer != null && !HighlightedCellContainer.IsEditing;
      }
    }

    #endregion // EnterEditMode Command

    #region CancelEditMode Command

    private void CancelEditMode_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (CanCancelEditMode)
      {
        HighlightedCellContainer.CancelEditMode();
        Focus();
      }
    }

    private void CancelEditMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = CanCancelEditMode;
    }

    private bool CanCancelEditMode
    {
      get
      {
        return HighlightedCellContainer != null && HighlightedCellContainer.IsEditing;
      }
    }

    #endregion // CancelEditMode Command

    #region RemoveFilter Command

    private void RemoveFilter_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      DataGridColumn column = e.Parameter as DataGridColumn;
      if (column != null)
      {
        column.Filter = null;
      }
    }

    private void RemoveFilter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      DataGridColumn column = e.Parameter as DataGridColumn;
      e.CanExecute = column != null && column.Filter != null;
    }

    #endregion // RemoveFilter Command

    #region Copy Command

    private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      string text = "";
      bool newLine = true;

      // Create column index map:
      Dictionary<DataGridColumn, int> _columnIndexMap = new Dictionary<DataGridColumn, int>(); // TODO: maybe columns can store their index.
      int index = 0;
      foreach (DataGridColumn column in EffectiveColumns)
      {
        _columnIndexMap[column] = index;
        index++;
      }

      // Keep a sorted list of the columns that will be included
      SortedList<int, int> selectedColumns = new SortedList<int,int>();
      // If at least 1 row is selected, then put all columns in the selected columns list.
      if (_selectedItems.Count > 0)
      {
        for (int columnIndex = 0; columnIndex < EffectiveColumns.Count; columnIndex++)
        {
          selectedColumns.Add(columnIndex, columnIndex);
        }
      }

      // Create an arry to store all the selected cell values in the order they are displayed.
      // The row count must make sure all rows and cells fit.
      object[,] copyArray = new object[EffectiveColumns.Count, _selectedCellMap.Count + _selectedItems.Count];
      int rowCount = DisplayedItemsSource.CountOnTotalActiveCollection;
      int copyArrayRowIndex = 0;
      // Iterate all rows after filtering and sorting etc
      for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
      {
        object rowObject = DisplayedItemsSource.GetItemAt(rowIndex);
        if (rowObject != null && _selectedItems.Contains(rowObject))
        {
          // The whole row is selected:
          int columnIndex = 0;
          foreach (DataGridColumn column in EffectiveColumns)
          {
            DataGridCell cell = new DataGridCell(rowObject, column);
            copyArray[columnIndex, copyArrayRowIndex] = cell.Value;
            columnIndex++;
          }
          copyArrayRowIndex++;
        }
        else if (rowObject != null && _selectedCellMap.ContainsKey(rowObject))
        {
          // Some of the cells in the row are selected:
          IList<DataGridCell> cells = _selectedCellMap[rowObject];
          foreach (DataGridCell cell in cells)
          {
            int columnIndex = _columnIndexMap[cell.Column];
            if (!selectedColumns.ContainsKey(columnIndex))
            {
              // Make sure the column is included:
              selectedColumns.Add(columnIndex, columnIndex);
            }
            copyArray[columnIndex, copyArrayRowIndex] = cell.Value;
          }
          copyArrayRowIndex++;
        }
      }

      // Iterate the array and copy to the clipboard:
      for (int row = 0; row < copyArrayRowIndex; row++)
      {
        newLine = true;
        foreach (int column in selectedColumns.Values)
        {
          if (!newLine)
          {
            text += "\t";
          }
          object o = copyArray[column, row];
          text += o == null ? "" : o.ToString();
          newLine = false;
        }
        if (row < copyArrayRowIndex - 1)
        {
          text += "\n";
        }
      }

      try
      {
        Clipboard.SetText(text);
      }
      catch (Exception) { }
    }

    private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    #endregion // Copy Command

    #endregion // Commands

    internal DataGridPanel DataGridPanel
    {
      get { return _dataGridPanel; }
      set { _dataGridPanel = value; }
    }

    internal bool HasAutoColumn { get; set; }

    internal bool HasStarSizingColumn { get; set; }

    /// <summary>
    /// Called when the ItemsSource property changes.
    /// </summary>
    /// <param name="oldValue">The old items source.</param>
    /// <param name="newValue">The new items source.</param>
    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      base.OnItemsSourceChanged(oldValue, newValue);

      if (AutoColumnWidthBehavior == AutoColumnWidthBehavior.OneTime && EffectiveColumns != null)
      {
        foreach (DataGridColumn column in EffectiveColumns)
        {
          column.IsAutoWidthDirty = true;
        }
      }

      /*DataGridCellContainer cell = HighlightedCellContainer;
      if (cell != null && cell.IsEditing)
      {
        cell.SetIsEditing(false);
      }*/

      INotifyCollectionChanged notifyer = oldValue as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = oldValue as IBindingList;
        if (bindingList != null)
        {
          bindingList.ListChanged -= new ListChangedEventHandler(BindingList_ListChanged);
        }
      }

      CollectionView view = oldValue as CollectionView;
      if (view != null)
      {
        view.CurrentChanged -= new EventHandler(CollectionView_CurrentChanged);
      }

      IComparer<object> sortComparer = null;
      SortDirection sortDirection = SortDirection.None;
      IFilter filter = null;

      if (DisplayedItemsSource != null)
      {
        filter = DisplayedItemsSource.FilterExpression;
        sortComparer = DisplayedItemsSource.SortComparer;
        sortDirection = DisplayedItemsSource.SortDirection;
        DisplayedItemsSource.PageIndexChanged -= new EventHandler(DataGridItemsSource_PageIndexChanged);
        DisplayedItemsSource.CollectionUpdated -= new EventHandler(DataGridItemsSource_CollectionUpdated);
        DisplayedItemsSource.RowIsExpandedChanged -= new EventHandler<RowIsExpandedChangedEventArgs>(DataGridItemsSource_RowIsExpandedChanged);
        DisplayedItemsSource.Destroy();
      }

      SelectedCell = null;
      SelectedItem = null;

      DataGridItemsSource itemsSource = new DataGridItemsSource(ItemsSource ?? new List<object>(), ItemTemplate, HierarchyMode);

      itemsSource.ObjectBuilder = ObjectBuilder;
      itemsSource.PageSize = PageSize;
      itemsSource.GroupMap = _groupDescriptionMap;
      if (ItemsSource != null)
      {
        itemsSource.PageIndexChanged += new EventHandler(DataGridItemsSource_PageIndexChanged);
        itemsSource.CollectionUpdated += new EventHandler(DataGridItemsSource_CollectionUpdated);
        itemsSource.RowIsExpandedChanged += new EventHandler<RowIsExpandedChangedEventArgs>(DataGridItemsSource_RowIsExpandedChanged);
      }
      itemsSource.AllowUserToAddRows = AllowUserToAddRows;
      SetValue(DisplayedItemsSourcePropertyKey, itemsSource);
      UpdateCanSelectAll();
      RecalculateEffectiveColumns();
      if (DataGridPanel != null)
      {
        DataGridPanel.InvalidateRequiresMeasure();
      }

      notifyer = ItemsSource as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = ItemsSource as IBindingList;
        if (bindingList != null)
        {
          bindingList.ListChanged += new ListChangedEventHandler(BindingList_ListChanged);
        }
      }

      view = ItemsSource as CollectionView;
      if (view != null)
      {
        view.CurrentChanged += new EventHandler(CollectionView_CurrentChanged);
        if (IsSynchronizedWithCurrentItem)
        {
          SelectedItem = view.CurrentItem;
        }
      }

      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.SortComparer = sortComparer;
        DisplayedItemsSource.SortDirection = sortDirection;
        DisplayedItemsSource.FilterExpression = filter;
      }
    }

    private void DataGridItemsSource_RowIsExpandedChanged(object sender, RowIsExpandedChangedEventArgs e)
    {
      EventHandler<RowIsExpandedChangedEventArgs> handler = RowIsExpandedChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Raised when a row is expanded or collapsed.
    /// </summary>
    public event EventHandler<RowIsExpandedChangedEventArgs> RowIsExpandedChanged;

    private void CollectionView_CurrentChanged(object sender, EventArgs e)
    {
      CollectionView view = sender as CollectionView;
      if (SelectedItem != view.CurrentItem && IsSynchronizedWithCurrentItem)
      {
        SelectedItem = view.CurrentItem;
      }
    }

    /// <summary>
    /// Called when the items property changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.Source_CollectionChanged(null, e);
      }
    }

    private void BindingList_ListChanged(object sender, ListChangedEventArgs e)
    {
      if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemMoved || e.ListChangedType == ListChangedType.Reset)
      {
        HandleCollectionChanged(e.NewIndex);
      }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      HandleCollectionChanged(e.NewStartingIndex);
    }

    private void HandleCollectionChanged(int newIndex)
    {
      if (IsViewportFixedWhenAddingAndRemovingRows && DataGridPanel != null)
      {
        double currentViewport = DataGridPanel.VerticalOffset;
        if (currentViewport >= newIndex)
        {
          DataGridPanel.SetVerticalOffset(currentViewport + 1);
        }
      }
      foreach (DataGridColumn column in EffectiveColumns)
      {
        column.SortDirection = SortDirection.None;
      }
    }

    private void DataGridItemsSource_CollectionUpdated(object sender, EventArgs e)
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.InvalidateRequiresMeasure();
      }
      UpdateAggregates();
    }

    private void UpdateAggregates()
    {
      foreach (DataGridColumn column in EffectiveColumns)
      {
        AggregateBase aggregate = column.FooterAggregate as AggregateBase;
        if (aggregate != null)
        {
          aggregate.PropertyInfo = column.PropertyInfo;
          aggregate.Calculate(FilterDisplayedItemsSourceForAggregates());
        }
      }
    }

    internal void UpdateAggregate(DataGridColumn column)
    {
      AggregateBase aggregate = column.FooterAggregate as AggregateBase;
      if (aggregate != null)
      {
        aggregate.PropertyInfo = column.PropertyInfo;
        aggregate.Calculate(FilterDisplayedItemsSourceForAggregates());
      }

      if (DataGridPanel != null)
      {
        DataGridPanel.UpdateGroupAggregates();
      }
    }

    private IEnumerable FilterDisplayedItemsSourceForAggregates()
    {
      foreach (object item in DisplayedItemsSource.GetSortedCollection())
      {
        if (!(item is DataGridGroup))
        {
          if (item is DataGridItemWrapper)
          {
            DataGridItemWrapper wrapper = (DataGridItemWrapper)item;
            yield return wrapper.Object;
          }
          else
          {
            yield return item;
          }
        }
      }
    }

    private void DataGridItemsSource_PageIndexChanged(object sender, EventArgs e)
    {
      PageIndex = DisplayedItemsSource.PageIndex;
    }

    private DataGridRow GetDataGridRow(int index)
    {
      return _dataGridPanel.GetDataGridRow(index);
    }

    /*/// <summary>
    /// Returns a new UI element for displaying a row in the data grid.
    /// </summary>
    /// <returns>A new instance of a <see cref="DataGridRow"/>.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      DataGridRow row = new DataGridRow(this);
      BindingOperations.SetBinding(row, DataGridRow.ItemsSourceProperty, new Binding { Source = this, Path = new PropertyPath("EffectiveColumns") });
      AttachDataGridRowEventHandlers(row);
      return row;
    }

    /// <summary>
    /// Prepares a <see cref="DataGridRow"/> for displaying an object.
    /// </summary>
    /// <param name="element">The data grid row to be displayed.</param>
    /// <param name="item">The object that the data grid row will display.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      DataGridRow row = element as DataGridRow;
      row.Content = item;

      // Restoring state information that was destroyed by virtualization:
      if (SelectedItems.Contains(item))
      {
        row.IsSelected = true;
      }
      else
      {
        row.IsSelected = false;
      }
    }

    /// <summary>
    /// Returns whether or not the given object can be used as a row container.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <returns>True if the given item is a <see cref="DataGridRow"/>. False otherwise.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is DataGridRow || item is DataGridGroupingRow;
    }*/

    /*internal bool IsItemItsOwnContainer(object item)
    {
      //return IsItemItsOwnContainerOverride(item);
      return item is DataGridRow || item is DataGridGroupingRow;
    }*/

    internal DependencyObject GetContainer(object item)
    {
      //return GetContainerForItemOverride();
      DataGridGroup group = item as DataGridGroup;
      if (group != null)
      {
        DataGridGroupingRow groupingRow = new DataGridGroupingRow(group, EffectiveColumns);
        DataGridColumn column = GetGroupedColumnFromDescription(group.GroupFrom);
        if (column != null)
        {
          groupingRow.HeaderTemplate = column.GroupRowHeaderTemplate;
          groupingRow.HeaderTemplateSelector = column.GroupRowHeaderTemplateSelector;
        }
        return groupingRow;
      }
      DataGridRow row = new DataGridRow(this);
      BindingOperations.SetBinding(row, DataGridRow.ItemsSourceProperty, new Binding { Source = this, Path = new PropertyPath("EffectiveColumns") });
      AttachDataGridRowEventHandlers(row);
      return row;
    }

    internal DataGridColumn GetGroupedColumnFromDescription(GroupDescription groupDescription)
    {
      DataGridColumn column = null;
      _groupDescriptionMap.TryGetValue(groupDescription, out column);
      return column;
    }

    internal void PrepareContainer(DependencyObject element, object item)
    {
      //PrepareContainerForItemOverride(element, item);
      DataGridRow row = element as DataGridRow;

      if (row != null)
      {
        row.Content = item;
        row.ItemWrapper = DisplayedItemsSource != null ? DisplayedItemsSource.GetHierarchyWrapper(item) : null;

        // Restoring state information that was destroyed by virtualization:
        if (SelectedItems.Contains(item))
        {
          row.IsSelected = true;
        }
        else
        {
          row.IsSelected = false;
        }
      }
    }

    internal void PrepareContainerBackground(DependencyObject element)
    {
      DataGridRow row = element as DataGridRow;
      if (row != null)
      {
        row.Background = GetRowBackground(row);
      }
    }

    internal void ClearContainer(DependencyObject element, object item)
    {
      //ClearContainerForItemOverride(element, item);
    }

    internal bool CheckContainer(object item)
    {
      //return IsItemItsOwnContainerOverride(item);
      return item is DataGridRow || item is DataGridGroupingRow;
    }

    //private object _mouseDownRow;
    private int _mouseDownRowIndex = -1;
    private int _previousSelectionDragRowIndex;

    private int _mouseDownColumnIndex = -1;
    private int _previousSelectionDragColumnIndex;


    /// <summary>
    /// Called when a mouse button is released over this <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      if (e.ButtonState == MouseButtonState.Released) // Because calling Window.DragMove somehow calls this method with e.ButtonState = Pressed. Seems like a bug.
      {
        _smartScrollTimer.Stop();
        ReleaseMouseCapture();
        //_mouseDownRow = null;
        _mouseDownRowIndex = -1;
        _mouseDownColumnIndex = -1;

        if (CellEditModeBehavior == DataGridCellEditModeBehavior.OnClick)
        {
          DependencyObject mouseOverElement = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
          DataGridCellContainer mouseOverCell = VisualTreeUtils.FindContaining<DataGridCellContainer>(mouseOverElement);
          if (mouseOverCell != null && mouseOverCell.IsHighlighted == true)
          {
            mouseOverCell.SetIsEditing(true);
          }
        }
      }
    }

    /// <summary>
    /// Called when the mouse leaves this <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      SetValue(MouseOverCellPropertyKey, null);
    }

    /// <summary>
    /// Called when the mouse is moved over this <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      _previousMouseEventArgs = e;
      if ((Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed || (IsRightClickSelectionEnabled && Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed)) && SelectionMode == SelectionMode.Extended && DisplayedItemsSource != null && DisplayedItemsSource.CountOnCurrentPage > 0)
      {
        // Row selection:
        if ((SelectionType == DataGridSelectionType.Row || (SelectionType == DataGridSelectionType.RowAndCell && _mouseDownType == DataGridSelectionType.Row)) && _mouseDownRowIndex != -1)
        {
          int mouseOverRowIndex = -1;
          DependencyObject mouseOverElement = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
          if (mouseOverElement != null)
          {
            DataGridRow mouseOverRow = VisualTreeUtils.FindContaining<DataGridRow>(mouseOverElement);
            if (mouseOverRow != null)
            {
              mouseOverRowIndex = mouseOverRow.Index;
            }
          }
          else
          {
            return; // If the mouseOverElement is null, it should mean this method could have been called through Window.DragMove, so we don't want to effect the data grid selection.
          }
          if (mouseOverRowIndex == -1 && DataGridPanel != null && DataGridPanel.RealizedChildren != null && DataGridPanel.RealizedChildren.Count > 0)
          {
            Point position = e.GetPosition(DataGridPanel);
            if (DataGridPanel.RealizedChildren.Count > 0)
            {
              if (position.Y < 0)
              {
                mouseOverRowIndex = (DataGridPanel.RealizedChildren[0] as IDataGridRow).Index;
              }
              else if (position.Y > DataGridPanel.ActualHeight)
              {
                mouseOverRowIndex = (DataGridPanel.RealizedChildren[DataGridPanel.RealizedChildren.Count - 1] as IDataGridRow).Index;
              }
            }
          }
          if (mouseOverRowIndex != -1)
          {
            SelectionUpdateInfo info = SelectionRangeUtils.GetSelectionUpdateInfo(_mouseDownRowIndex, _previousSelectionDragRowIndex, mouseOverRowIndex);
            for (int i = info.StartEmptyIndex; i <= info.EndEmptyIndex; i++)
            {
              object toRemove = DisplayedItemsSource.GetItemOnCurrentPageAt_NullGroupHeaders(i);
              if (toRemove != null)
              {
                SelectedItems.Remove(toRemove);
              }
            }
            for (int i = info.StartFillIndex; i <= info.EndFillIndex; i++)
            {
              object o = DisplayedItemsSource.GetItemOnCurrentPageAt_NullGroupHeaders(i);
              if (o != null && !SelectedItems.Contains(o))
              {
                SelectedItems.Add(o);
              }
            }
            _previousSelectionDragRowIndex = mouseOverRowIndex;
          }
        }
        // Cell selection:
        else if ((SelectionType == DataGridSelectionType.Cell || SelectionType == DataGridSelectionType.RowAndCell) && _mouseDownRowIndex != -1 && _mouseDownColumnIndex != -1)
        {
          int mouseOverRowIndex = -1;
          int mouseOverColumnIndex = -1;
          DependencyObject mouseOverElement = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
          if (mouseOverElement != null)
          {
            DataGridCellContainer mouseOverCell = VisualTreeUtils.FindContaining<DataGridCellContainer>(mouseOverElement);
            if (mouseOverCell != null)
            {
              mouseOverRowIndex = mouseOverCell.Row.Index;
              mouseOverColumnIndex = EffectiveColumns.IndexOf(mouseOverCell.Column); // TODO: try improve performance here.
            }
          }
          if (mouseOverRowIndex == -1 && DataGridPanel != null && DataGridPanel.RealizedChildren != null && DataGridPanel.RealizedChildren.Count > 0)
          {
            Point position = e.GetPosition(DataGridPanel);
            if (position.Y < 0)
            {
              mouseOverRowIndex = (DataGridPanel.RealizedChildren[0] as IDataGridRow).Index;
            }
            else if (position.Y > DataGridPanel.ActualHeight)
            {
              mouseOverRowIndex = (DataGridPanel.RealizedChildren[DataGridPanel.RealizedChildren.Count - 1] as IDataGridRow).Index;
            }
          }
          if (mouseOverColumnIndex == -1 && DataGridPanel != null && DataGridPanel.RealizedChildren != null && DataGridPanel.RealizedChildren.Count > 0)
          {
            Point position = e.GetPosition(DataGridPanel);
            DataGridRow firstRow = DataGridPanel.RealizedChildren[0] as DataGridRow;
            if (firstRow != null && firstRow.Panel != null && firstRow.Panel.RealizedChildren != null && firstRow.Panel.RealizedChildren.Count > 0)
            {
              // TODO: would be good to work around the IndexOf operators here:
              if (position.X < 0)
              {
                mouseOverColumnIndex = EffectiveColumns.IndexOf((firstRow.Panel.RealizedChildren[0] as DataGridCellContainer).Column);
              }
              else if (position.X > DataGridPanel.ActualWidth)
              {
                mouseOverColumnIndex = EffectiveColumns.IndexOf((firstRow.Panel.RealizedChildren[firstRow.Panel.RealizedChildren.Count - 1] as DataGridCellContainer).Column);
              }
            }
          }
          if (mouseOverColumnIndex != -1)
          {
            int currentFirstSelectedRow = Math.Min(_mouseDownRowIndex, _previousSelectionDragRowIndex);
            int currentLastSelectedRow = Math.Max(_mouseDownRowIndex, _previousSelectionDragRowIndex);

            SelectionUpdateInfo columnInfo = SelectionRangeUtils.GetSelectionUpdateInfo(_mouseDownColumnIndex, _previousSelectionDragColumnIndex, mouseOverColumnIndex);

            for (int row = currentFirstSelectedRow; row <= currentLastSelectedRow; row++)
            {
              for (int i = columnInfo.StartEmptyIndex; i <= columnInfo.EndEmptyIndex; i++)
              {
                RemoveSelectedCell(GetCellOnCurrentPage(row, i));
              }
            }
            for (int row = currentFirstSelectedRow; row <= currentLastSelectedRow; row++)
            {
              for (int i = columnInfo.StartFillIndex; i <= columnInfo.EndFillIndex; i++)
              {
                DataGridCell cell = GetCellOnCurrentPage(row, i);
                if (!ContainsSelectedCell(cell))
                {
                  SelectedCells.Add(cell);
                }
              }
            }

            _previousSelectionDragColumnIndex = mouseOverColumnIndex;
          }
          if (mouseOverRowIndex != -1)
          {
            int newFirstSelectedColumn = Math.Min(_mouseDownColumnIndex, mouseOverColumnIndex == -1 ? _previousSelectionDragColumnIndex : mouseOverColumnIndex);
            int newLastSelectedColumn = Math.Max(_mouseDownColumnIndex, mouseOverColumnIndex == -1 ? _previousSelectionDragColumnIndex : mouseOverColumnIndex);

            SelectionUpdateInfo rowInfo = SelectionRangeUtils.GetSelectionUpdateInfo(_mouseDownRowIndex, _previousSelectionDragRowIndex, mouseOverRowIndex);

            for (int column = newFirstSelectedColumn; column <= newLastSelectedColumn; column++)
            {
              for (int i = rowInfo.StartEmptyIndex; i <= rowInfo.EndEmptyIndex; i++)
              {
                RemoveSelectedCell(GetCellOnCurrentPage(i, column));
              }
            }
            for (int column = newFirstSelectedColumn; column <= newLastSelectedColumn; column++)
            {
              for (int i = rowInfo.StartFillIndex; i <= rowInfo.EndFillIndex; i++)
              {
                DataGridCell cell = GetCellOnCurrentPage(i, column);
                if (!ContainsSelectedCell(cell))
                {
                  SelectedCells.Add(cell);
                }
              }
            }

            _previousSelectionDragRowIndex = mouseOverRowIndex;
          }
        }
      }

      // Mouse over cell logic
      DependencyObject mouseOverObject = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
      if (mouseOverObject != null)
      {
        DataGridCellContainer mouseOverCell = VisualTreeUtils.FindContaining<DataGridCellContainer>(mouseOverObject);
        if (mouseOverCell != null && mouseOverCell.Column != null && mouseOverCell.Row != null && mouseOverCell.Row.Content != null)
        {
          SetValue(MouseOverCellPropertyKey, new DataGridCell(mouseOverCell.Row.Content, mouseOverCell.Column));
        }
        else
        {
          SetValue(MouseOverCellPropertyKey, null);
        }
      }
      else
      {
        SetValue(MouseOverCellPropertyKey, null);
      }
    }

    private MouseEventArgs _previousMouseEventArgs;

    /// <summary>
    /// Called when text input occurs while this <see cref="DataGrid"/> has focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      if (HighlightedCellContainer != null)
      {
        HighlightedCellContainer.SetIsEditing(true, e.Text);
      }
    }

    // TODO: The performance on this method is a bit slow.
    private void RemoveSelectedCell(DataGridCell cell)
    {
      for (int i = SelectedCells.Count - 1; i >= 0; i--)
      {
        if (SelectedCells[i].RowContent == cell.RowContent && SelectedCells[i].Column == cell.Column)
        {
          SelectedCells.RemoveAt(i);
          break;
        }
      }
    }

    // TODO: The performance on this method is a bit slow.
    private bool ContainsSelectedCell(DataGridCell cell)
    {
      for (int i = SelectedCells.Count - 1; i >= 0; i--)
      {
        if (SelectedCells[i].RowContent == cell.RowContent && SelectedCells[i].Column == cell.Column)
        {
          return true;
        }
      }
      return false;
    }

    private void AttachDataGridRowEventHandlers(DataGridRow row)
    {
      //row.AddHandler(DataGridRow.MouseLeftButtonDownEvent, new MouseButtonEventHandler(DataGridRow_MouseLeftButtonDown), true);
      row.MouseLeftButtonDown += new MouseButtonEventHandler(DataGridRow_MouseLeftButtonDown);
      row.MouseRightButtonDown += new MouseButtonEventHandler(DataGridRow_MouseRightButtonDown);

      row.MouseDoubleClick += new MouseButtonEventHandler(Row_MouseDoubleClick);
    }

    private void DataGridRow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (IsRightClickSelectionEnabled)
      {
        DataGridRow_MouseLeftButtonDown(sender, e);
      }
    }

    private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      DataGridRow row = sender as DataGridRow;
      if (row.ItemWrapper != null)
      {
        DependencyObject mouseOverElement = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
        DataGridCellContainer mouseOverCell = VisualTreeUtils.FindContaining<DataGridCellContainer>(mouseOverElement);
        if (mouseOverCell == null)
        {
          if (row.ItemWrapper.HasChildren && OnHierarchicalStateChanging(row.Content, !row.ItemWrapper.IsExpanded))
          {
            row.ItemWrapper.IsExpanded = !row.ItemWrapper.IsExpanded;
          }
        }
      }
    }

    private DataGridSelectionType _mouseDownType = DataGridSelectionType.Cell;

    private void DataGridRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Focus();
      _smartScrollTimer.Start();
      if (!CanChangeHighlightedCell(null))
      {
        return;
      }

      CaptureMouse();
      bool isValidRowAndCellMode = false;
      _mouseDownType = DataGridSelectionType.Cell;
      if (SelectionType == DataGridSelectionType.RowAndCell)
      {
        DependencyObject mouseOverElement = InputHitTest(Mouse.GetPosition(this)) as DependencyObject;
        DataGridCellContainer mouseOverCell = VisualTreeUtils.FindContaining<DataGridCellContainer>(mouseOverElement);
        if (mouseOverCell == null)
        {
          isValidRowAndCellMode = true;
          _mouseDownType = DataGridSelectionType.Row;
        }
      }
      if (SelectionType == DataGridSelectionType.Row || isValidRowAndCellMode)
      {
        DataGridRow row = sender as DataGridRow;
        //_mouseDownRow = row.Content;
        _mouseDownRowIndex = row.Index;
        _previousSelectionDragRowIndex = _mouseDownRowIndex;
        if (SelectionMode == SelectionMode.Single && !row.IsSelected)
        {
          row.IsSelected = true;
          SelectedItem = row.Content;
        }
        else if (SelectionMode == SelectionMode.Multiple)
        {
          if (!row.IsEditing)
          {
            if (row.IsSelected)
            {
              row.IsSelected = false;
              SelectedItems.Remove(row.Content);
            }
            else
            {
              row.IsSelected = true;
              SelectedItem = row.Content;
            }
          }
          else
          {
            row.IsSelected = true;
            SelectedItem = row.Content;
          }
        }
        else if (SelectionMode == SelectionMode.Extended)
        {
          if (KeyboardUtils.IsHoldingShift && SelectedItem != null)
          {
            int startIndex = DisplayedItemsSource.IndexOf(SelectedItem);
            int selectedItemIndex = startIndex;
            if (startIndex >= 0)
            {
              if (!KeyboardUtils.IsHoldingCtrl)
              {
                SelectedItems.Clear();
                SelectedCell = null;
                SelectedCells.Clear();
              }
              int endIndex = row.Index + (PageIndex * PageSize);
              if (endIndex < startIndex)
              {
                int temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
              }
              for (int i = startIndex; i <= endIndex; i++)
              {
                Object o = DisplayedItemsSource.GetItemAt(i);
                if (o != null && !SelectedItems.Contains(o)) // TODO: This contains check is a bit slow.
                {
                  SelectedItems.Add(o);
                }
              }
              SelectedItem = DisplayedItemsSource.GetItemAt(selectedItemIndex);
            }
          }
          else if (KeyboardUtils.IsHoldingCtrl)
          {
            if (!row.IsEditing)
            {
              if (row.IsSelected)
              {
                row.IsSelected = false;
                SelectedItems.Remove(row.Content);
              }
              else
              {
                row.IsSelected = true;
                SelectedItem = row.Content;
              }
            }
            else
            {
              row.IsSelected = true;
              SelectedItem = row.Content;
            }
          }
          else
          {
            row.IsSelected = true;
            SelectedItems.Clear();
            SelectedCell = null;
            SelectedCells.Clear();
            SelectedItem = row.Content;
          }
        }

        if (HighlightedCell == null || (HighlightedCell.RowContent != row.Content)) // && HighlightedCellContainer.IsValid))
        {
          if (EffectiveColumns.Count > 0)
          {
            DataGridCellContainer firstCell = row.GetCell(EffectiveColumns[0]);
            if (firstCell != null)
            {
              _ignoreSelectionUpdateForHighlightedCell = true;
              HighlightedCell = new DataGridCell(firstCell.Row.Content, firstCell.Column) { RowIndex = firstCell.Row.Index };
              _ignoreSelectionUpdateForHighlightedCell = false;
            }
          }
        }
      }
    }

    internal bool IsHighlightedCell(DataGridCellContainer cell)
    {
      return HighlightedCell != null && cell.Content == HighlightedCell.RowContent && cell.Column == HighlightedCell.Column;
    }

    private DataGridCellContainer GetCellContainer(DataGridCell cell)
    {
      if (cell != null && IsLoaded && DisplayedItemsSource != null)
      {
        int index = cell.RowIndex;
        if (index >= DisplayedItemsSource.CountOnCurrentPage || index < 0 || DisplayedItemsSource.GetItemOnCurrentPageAt(index) != cell.RowContent)
        {
          // Potential performance issue here:
          index = DisplayedItemsSource.IndexOfOnCurrentPage(cell.RowContent);
        }
        if (index >= 0 && index < DisplayedItemsSource.CountOnCurrentPage)
        {
          DataGridRow row = GetDataGridRow(index);
          if (row != null)
          {
            DataGridCellContainer cellContainer = row.GetCell(cell.Column);
            return cellContainer;
          }
        }
      }
      return null;
    }

    internal bool CanEdit(DataGridCell cellModel)
    {
      bool canEdit = AllowEditing && cellModel.Column.AllowEditing && !_uneditableRows.Contains(cellModel.RowContent);
      if (canEdit)
      {
        HashSet<object> rows;
        _uneditableCells.TryGetValue(cellModel.Column, out rows);
        if (rows != null)
        {
          canEdit = !rows.Contains(cellModel.RowContent);
        }
      }
      return canEdit;
    }

    #region MouseOverCell Property

    /// <summary>
    /// Gets the MouseOverCell.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MouseOverCellProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridCell MouseOverCell
    {
      get { return (DataGridCell)GetValue(MouseOverCellProperty); }
    }

    private static readonly DependencyPropertyKey MouseOverCellPropertyKey =
        DependencyProperty.RegisterReadOnly("MouseOverCell", typeof(DataGridCell), typeof(DataGrid), new UIPropertyMetadata(null, OnMouseOverCellChanged));

    /// <summary>
    /// Identifies the <see cref="MouseOverCell"/> property.
    /// </summary>
    public static readonly DependencyProperty MouseOverCellProperty =
        MouseOverCellPropertyKey.DependencyProperty;

    private static void OnMouseOverCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DataGridCell oldCell = e.OldValue as DataGridCell;
      DataGridCell newCell = e.NewValue as DataGridCell;
      if (oldCell != null && newCell != null && (oldCell.Column != newCell.Column || oldCell.RowContent != newCell.RowContent)) // neither cell is null, and one of their properties are different.
      {
        ((DataGrid)d).OnMouseOverCellChanged(new DataGridCellEventArgs((DataGridCell)e.NewValue));
      }
      else if (oldCell != newCell && (oldCell == null || newCell == null)) // One and only one of the cells is null.
      {
        ((DataGrid)d).OnMouseOverCellChanged(new DataGridCellEventArgs((DataGridCell)e.NewValue));
      }
    }

    #endregion // MouseOverCell Property

    private void DisableHighlightedCellEditing()
    {
      DataGridCellContainer cell = HighlightedCellContainer;
      if (cell != null)
      {
        cell.SetIsEditing(false);
      }
    }

    private DataGridCellContainer HighlightedCellContainer
    {
      get { return GetCellContainer(HighlightedCell); }
    }

    #region HighlightedCell Property

    /// <summary>
    /// Gets or sets the highlighted <see cref="DataGridCell"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HighlightedCellProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridCell HighlightedCell
    {
      get { return (DataGridCell)GetValue(HighlightedCellProperty); }
      set { SetValue(HighlightedCellProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HighlightedCell"/> property.
    /// </summary>
    public static readonly DependencyProperty HighlightedCellProperty =
      DependencyProperty.Register("HighlightedCell", typeof(DataGridCell), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnHighlightedCellChanged));

    private static void OnHighlightedCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnHighlightedCellChanged(e);
    }

    private bool _ignoreBringHighlightedCellIntoView = false;

    private void OnHighlightedCellChanged(DependencyPropertyChangedEventArgs e)
    {
      // Un-highlight currently highlighted cell.
      DataGridCellContainer currentHighlightedCell = GetCellContainer(e.OldValue as DataGridCell);
      if (currentHighlightedCell != null)
      {
        currentHighlightedCell.SetIsHighlighted(false);
      }
      // highlight the new cell based on the given cell model.
      DataGridCellContainer cell = HighlightedCellContainer;
      if (cell != null)
      {
        cell.SetIsHighlighted(true);
      }
      // Ensure the correct row index.
      if (HighlightedCell != null)
      {
        if (!_selectedItemLock)
        {
          _selectedItem = HighlightedCell.RowContent; // For use by the Shift + Up/Down function.
        }
        SetValue(HighlightedItemPropertyKey, HighlightedCell.RowContent);
        int index = HighlightedCell.RowIndex;
        if (index >= DisplayedItemsSource.CountOnCurrentPage || index < 0 || DisplayedItemsSource.GetItemOnCurrentPageAt(index) != HighlightedCell.RowContent)
        {
          // TODO: Potential performance issue here:
          index = DisplayedItemsSource.IndexOfOnCurrentPage(HighlightedCell.RowContent);
          if (index < 0)
          {
            index = 0;
          }
        }
        HighlightedCell.RowIndex = index;
      }
      // Bring the new highlighted cell into view.
      if (HighlightedCell != null && !_ignoreBringHighlightedCellIntoView)
      {
        BringCellIntoView(HighlightedCell);
      }
      if (!_ignoreSelectionUpdateForHighlightedCell)
      {
        UpdateSelectionAfterNavigatingHighlightedCell();
      }
    }

    #endregion // HighlightedCell Property

    /// <summary>
    /// Raised whenever the user has performed an action that will cause the Highlighted cell to change.
    /// This can be used to cancel the operation.
    /// </summary>
    public event EventHandler<HighlightedCellChangingEventArgs> HighlightedCellChanging;

    private void OnHighlightedCellChanging(HighlightedCellChangingEventArgs args)
    {
      EventHandler<HighlightedCellChangingEventArgs> handler = HighlightedCellChanging;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void ChangeHighlightedCell(DataGridCell cell)
    {
      DataGridCellContainer container = HighlightedCellContainer;
      bool isValid = container == null || container.IsValid;

      HighlightedCellChangingEventArgs args = new HighlightedCellChangingEventArgs(isValid);
      OnHighlightedCellChanging(args);
      if (!args.Cancel)
      {
        HighlightedCell = cell;
      }
    }

    #region HighlightedItem Property

    /// <summary>
    /// Gets the highlighted item belonging to the row that contains the highlighted cell.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HighlightedItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object HighlightedItem
    {
      get { return GetValue(HighlightedItemProperty); }
    }

    private static readonly DependencyPropertyKey HighlightedItemPropertyKey =
        DependencyProperty.RegisterReadOnly("HighlightedItem", typeof(object), typeof(DataGrid), new UIPropertyMetadata(null, OnHighlightedItemChanged));

    /// <summary>
    /// Identifies the <see cref="HighlightedItem"/> property.
    /// </summary>
    public static readonly DependencyProperty HighlightedItemProperty =
        HighlightedItemPropertyKey.DependencyProperty;

    private static void OnHighlightedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnHighlightedItemChanged();
    }

    private void OnHighlightedItemChanged()
    {
      OnHighlightedItemChanged(new DataGridRowEventArgs(HighlightedItem));
    }

    /// <summary>
    /// Raised when the HighlightedItem property value changes.
    /// </summary>
    public event DataGridRowEventHandler HighlightedItemChanged;

    /// <summary>
    /// Raises the HighlightedItemChanged event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnHighlightedItemChanged(DataGridRowEventArgs e)
    {
      DataGridRowEventHandler handler = HighlightedItemChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    #endregion // HighlightedItem Property

    /// <summary>
    /// Changes the vertical and horizontal scroll offsets to include the given <see cref="DataGridCell"/> within the viewport.
    /// </summary>
    /// <param name="cellModel">The <see cref="DataGridCell"/> to bring into view.</param>
    public void BringCellIntoView(DataGridCell cellModel)
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.BringCellIntoView(cellModel);
      }
    }

    /// <summary>
    /// Changes the vertical scroll offset to bring the given row into the viewport.
    /// </summary>
    /// <param name="row">The data context of the row to bring into view.</param>
    public void BringRowIntoView(object row)
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.BringRowIntoView(row);
      }
    }

    #region Cell methods

    /// <summary>
    /// Returns a <see cref="DataGridCell"/> model object for the row and column at the given indicies.
    /// If the data grid is displaying data across multiple pages, a row index of zero will be the first item on the first page.
    /// </summary>
    /// <param name="rowIndex">The index of the row that the cell is in.</param>
    /// <param name="columnIndex">The index of the column that the cell is in.</param>
    /// <returns>The <see cref="DataGridCell"/> at the given row and column indicies.</returns>
    public DataGridCell GetCell(int rowIndex, int columnIndex)
    {
      // This first condition is for the scenario where this method is called before the DataGridItemsSource has had a chance to update
      // its collection when an item has been added.
      if (DisplayedItemsSource != null && DisplayedItemsSource.Count <= rowIndex)
      {
        // TODO: this will slightly affect the performance. Revise in the future.
        DisplayedItemsSource.ClearSorting();
      }

      if (rowIndex < 0 || DisplayedItemsSource == null || DisplayedItemsSource.Count <= rowIndex)
      {
        throw new ArgumentOutOfRangeException("rowIndex", "Row index is out of range");
      }
      if (columnIndex < 0 || EffectiveColumns == null || EffectiveColumns.Count <= columnIndex)
      {
        throw new ArgumentOutOfRangeException("columnIndex", "Column index is out of range");
      }
      object rowContent = DisplayedItemsSource.GetItemAt_SortedItemsOnly(rowIndex);
      DataGridColumn column = EffectiveColumns[columnIndex];
      return new DataGridCell(rowContent, column);
    }

    /// <summary>
    /// Returns the <see cref="DataGridCell"/> model object for the row and column indicies on the current page.
    /// A row index of zero will be the first item on the current page.
    /// </summary>
    /// <param name="rowIndex">The index of the row on the current page that the cell is in.</param>
    /// <param name="columnIndex">The index of the column that the cell is in.</param>
    /// <returns>The <see cref="DataGridCell"/> at the given row and column indicies.</returns>
    public DataGridCell GetCellOnCurrentPage(int rowIndex, int columnIndex)
    {
      if (DisplayedItemsSource == null)
      {
        throw new NullReferenceException("DisplayedItemsSource has not been initialized.");
      }
      rowIndex = Math.Max(0, Math.Min(DisplayedItemsSource.CountOnCurrentPage - 1, rowIndex));
      columnIndex = Math.Max(0, Math.Min(EffectiveColumns.Count - 1, columnIndex));

      if (rowIndex < 0 || DisplayedItemsSource == null || DisplayedItemsSource.CountOnCurrentPage <= rowIndex)
      {
        throw new ArgumentOutOfRangeException("rowIndex", "Row index is out of range on the current page");
      }
      if (columnIndex < 0 || EffectiveColumns == null || EffectiveColumns.Count <= columnIndex)
      {
        throw new ArgumentOutOfRangeException("columnIndex", "Column index is out of range");
      }
      object rowContent = DisplayedItemsSource.GetItemOnCurrentPageAt(rowIndex);
      DataGridColumn column = EffectiveColumns[columnIndex];
      return new DataGridCell(rowContent, column);
    }

    #endregion // Cell methods

    #region CellEditModeBehavior Property

    /// <summary>
    /// Gets or sets the <see cref="DataGridCellEditModeBehavior"/> which specifies how the user can cause a cell to go into edit mode using the mouse.
    /// The default is OnClickHighlightedCell.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CellEditModeBehaviorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridCellEditModeBehavior CellEditModeBehavior
    {
      get { return (DataGridCellEditModeBehavior)GetValue(CellEditModeBehaviorProperty); }
      set { SetValue(CellEditModeBehaviorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CellEditModeBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty CellEditModeBehaviorProperty =
      DependencyProperty.Register("CellEditModeBehavior", typeof(DataGridCellEditModeBehavior), typeof(DataGrid),
      new FrameworkPropertyMetadata(DataGridCellEditModeBehavior.OnClickHighlightedCell, OnCellEditModeBehaviorChanged));

    private static void OnCellEditModeBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnCellEditModeBehaviorChanged();
    }

    private void OnCellEditModeBehaviorChanged()
    {
    }

    #endregion // CellEditModeBehavior Property

    #region IsEditModeArrowKeyNavigationEnabled Property

    /// <summary>
    /// Gets or sets whether or not the arrow keys can be used to navigate the <see cref="DataGrid"/> while a cell is in edit mode.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEditModeArrowKeyNavigationEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEditModeArrowKeyNavigationEnabled
    {
      get { return (bool)GetValue(IsEditModeArrowKeyNavigationEnabledProperty); }
      set { SetValue(IsEditModeArrowKeyNavigationEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsEditModeArrowKeyNavigationEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEditModeArrowKeyNavigationEnabledProperty =
      DependencyProperty.Register("IsEditModeArrowKeyNavigationEnabled", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnIsEditModeArrowKeyNavigationEnabledChanged));

    private static void OnIsEditModeArrowKeyNavigationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsEditModeArrowKeyNavigationEnabledChanged();
    }

    private void OnIsEditModeArrowKeyNavigationEnabledChanged()
    {
    }

    #endregion // IsEditModeArrowKeyNavigationEnabled Property

    #region IsEditModeMaintainedDuringNavigation Property

    /// <summary>
    /// Gets or sets whether or not the DataGrid stays in edit mode when navigating cells. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEditModeMaintainedDuringNavigationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEditModeMaintainedDuringNavigation
    {
      get { return (bool)GetValue(IsEditModeMaintainedDuringNavigationProperty); }
      set { SetValue(IsEditModeMaintainedDuringNavigationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsEditModeMaintainedDuringNavigation"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEditModeMaintainedDuringNavigationProperty =
      DependencyProperty.Register("IsEditModeMaintainedDuringNavigation", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnIsEditModeMaintainedDuringNavigationChanged));

    private static void OnIsEditModeMaintainedDuringNavigationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsEditModeMaintainedDuringNavigationChanged();
    }

    private void OnIsEditModeMaintainedDuringNavigationChanged()
    {
    }

    #endregion // IsEditModeMaintainedDuringNavigation Property

    #region AllowUserToAddRows Property

    /// <summary>
    /// Gets or sets whether or not the user can add data grid rows. When set to true, an extra row is displayed at the end
    /// of the data. If the user edits a value in this row, the content of the row will be added to the items source.
    /// Then another empty row will be added at the end of the data.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowUserToAddRowsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowUserToAddRows
    {
      get { return (bool)GetValue(AllowUserToAddRowsProperty); }
      set { SetValue(AllowUserToAddRowsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowUserToAddRows"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowUserToAddRowsProperty =
      DependencyProperty.Register("AllowUserToAddRows", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnAllowUserToAddRowsChanged));

    private static void OnAllowUserToAddRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnAllowUserToAddRowsChanged();
    }

    private void OnAllowUserToAddRowsChanged()
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.AllowUserToAddRows = AllowUserToAddRows;
      }
    }

    #endregion // AllowUserToAddRows Property

    #region IsSynchronizedWithCurrentItem Property

    /// <summary>
    /// Gets or sets a value that indicates whether a <see cref="DataGrid"/> should keep the SelectedItem synchronized with the current item in the Items property.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSynchronizedWithCurrentItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSynchronizedWithCurrentItem
    {
      get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
      set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSynchronizedWithCurrentItem"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
      DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnIsSynchronizedWithCurrentItemChanged));

    private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsSynchronizedWithCurrentItemChanged();
    }

    private void OnIsSynchronizedWithCurrentItemChanged()
    {
      CollectionView view = ItemsSource as CollectionView;
      if (view != null && IsSynchronizedWithCurrentItem)
      {
        SelectedItem = view.CurrentItem;
      }
    }

    #endregion // IsSynchronizedWithCurrentItem Property

    #region IsToggleSelectAllButtonEnabled Property

    /// <summary>
    /// Gets or sets whether or not the toggle-select-all button is enabled.
    /// This is a button at the top left corner of the grid that selects or deselects all items.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsToggleSelectAllButtonEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsToggleSelectAllButtonEnabled
    {
      get { return (bool)GetValue(IsToggleSelectAllButtonEnabledProperty); }
      set { SetValue(IsToggleSelectAllButtonEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsToggleSelectAllButtonEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsToggleSelectAllButtonEnabledProperty =
      DependencyProperty.Register("IsToggleSelectAllButtonEnabled", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false));

    #endregion // IsToggleSelectAllButtonEnabled Property

    #region HorizontalGridLineBrush Property

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> used to render the horizontal grid lines.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HorizontalGridLineBrushProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush HorizontalGridLineBrush
    {
      get { return (Brush)GetValue(HorizontalGridLineBrushProperty); }
      set { SetValue(HorizontalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HorizontalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty HorizontalGridLineBrushProperty =
      DependencyProperty.Register("HorizontalGridLineBrush", typeof(Brush), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnHorizontalGridLineBrushChanged));

    private static void OnHorizontalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnHorizontalGridLineBrushChanged();
    }

    private void OnHorizontalGridLineBrushChanged()
    {
    }

    #endregion // HorizontalGridLineBrush Property

    #region VerticalGridLineBrush Property

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> used to render the verical grid lines.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VerticalGridLineBrushProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush VerticalGridLineBrush
    {
      get { return (Brush)GetValue(VerticalGridLineBrushProperty); }
      set { SetValue(VerticalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VerticalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty VerticalGridLineBrushProperty =
      DependencyProperty.Register("VerticalGridLineBrush", typeof(Brush), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnVerticalGridLineBrushChanged));

    private static void OnVerticalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnVerticalGridLineBrushChanged();
    }

    private void OnVerticalGridLineBrushChanged()
    {
    }

    #endregion // VerticalGridLineBrush Property

    #region AlternatingRowBackground Property

    /// <summary>
    /// Gets or sets the AlternatingRowBackground.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AlternatingRowBackgroundProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush AlternatingRowBackground
    {
      get { return (Brush)GetValue(AlternatingRowBackgroundProperty); }
      set { SetValue(AlternatingRowBackgroundProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AlternatingRowBackground"/> property.
    /// </summary>
    public static readonly DependencyProperty AlternatingRowBackgroundProperty =
      DependencyProperty.Register("AlternatingRowBackground", typeof(Brush), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnAlternatingRowBackgroundChanged));

    private static void OnAlternatingRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnAlternatingRowBackgroundChanged();
    }

    private void OnAlternatingRowBackgroundChanged()
    {
    }

    #endregion // AlternatingRowBackground Property

    #region IsViewportFixedWhenAddingAndRemovingRows Property

    /// <summary>
    /// Gets or sets whether or not the vieport is fixed to the currently displayed rows when new rows are added or removed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsViewportFixedWhenAddingAndRemovingRowsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsViewportFixedWhenAddingAndRemovingRows
    {
      get { return (bool)GetValue(IsViewportFixedWhenAddingAndRemovingRowsProperty); }
      set { SetValue(IsViewportFixedWhenAddingAndRemovingRowsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsViewportFixedWhenAddingAndRemovingRows"/> property.
    /// </summary>
    public static readonly DependencyProperty IsViewportFixedWhenAddingAndRemovingRowsProperty =
      DependencyProperty.Register("IsViewportFixedWhenAddingAndRemovingRows", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnIsViewportFixedWhenAddingAndRemovingRowsChanged));

    private static void OnIsViewportFixedWhenAddingAndRemovingRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsViewportFixedWhenAddingAndRemovingRowsChanged(e);
    }

    private void OnIsViewportFixedWhenAddingAndRemovingRowsChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // IsViewportFixedWhenAddingAndRemovingRows Property

    #region ObjectBuilder Property

    /// <summary>
    /// Gets or sets the <see cref="IObjectBuilder"/> used for the user to add new rows.
    /// This is useful for building objects that do not have public default constructors.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ObjectBuilderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IObjectBuilder ObjectBuilder
    {
      get { return (IObjectBuilder)GetValue(ObjectBuilderProperty); }
      set { SetValue(ObjectBuilderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ObjectBuilder"/> property.
    /// </summary>
    public static readonly DependencyProperty ObjectBuilderProperty =
      DependencyProperty.Register("ObjectBuilder", typeof(IObjectBuilder), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnObjectBuilderChanged));

    private static void OnObjectBuilderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnObjectBuilderChanged();
    }

    private void OnObjectBuilderChanged()
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.ObjectBuilder = ObjectBuilder;
      }
    }

    #endregion // ObjectBuilder Property

    private bool CanChangeHighlightedCell(DataGridCell cell)
    {
      DataGridCellContainer container = HighlightedCellContainer;
      bool isValid = container == null || container.IsValid;
      HighlightedCellChangingEventArgs args = new HighlightedCellChangingEventArgs(isValid);
      OnHighlightedCellChanging(args);
      return !args.Cancel;
    }

    internal void OnCellMouseOver(DataGridCellContainer cell)
    {
      if (MouseOverCell == null || (!MouseOverCell.RowIndex.Equals(cell.Row.Index) || !MouseOverCell.Column.Equals(cell.Column)))
      {
        SetValue(MouseOverCellPropertyKey, new DataGridCell(cell.Row.Content, cell.Column));
        MouseOverCell.RowIndex = cell.Row.Index;
      }
    }
     
    internal void OnCellRightMouseDown(DataGridCellContainer cell)
    {
      if (IsRightClickSelectionEnabled)
      {
        OnCellMouseDown(cell);
      }
    }

    private bool IsMouseOverCheckBox()
    {
      DependencyObject element = Mouse.DirectlyOver as DependencyObject;
      if (element != null)
      {
        CheckBox checkBox = VisualTreeUtils.FindAncestor<CheckBox>(element);
        if (checkBox != null)
        {
          return true;
        }
      }
      return false;
    }

    internal void OnCellMouseDown(DataGridCellContainer cell)
    {
      //DataGridCellContainer highlightedCell = HighlightedCellContainer;
      //if (highlightedCell == null || highlightedCell.IsValid)
      {
        if (cell.Row != null)
        {
          _mouseDownRowIndex = cell.Row.Index;
          _previousSelectionDragRowIndex = _mouseDownRowIndex;
        }

        if (cell.IsHighlighted)
        {
          if (!IsMouseOverCheckBox())
          {
            cell.SetIsEditing(true);
          }
        }
        else
        {
          if (!CanChangeHighlightedCell(cell.DataGridCell))
          {
            return;
          }

          DataGridCellContainer currentEditingCell = HighlightedCellContainer;
          if (currentEditingCell != null && currentEditingCell.IsEditing && currentEditingCell != cell)
          {
            currentEditingCell.SetIsEditing(false);
          }

          if (cell.Row.IsEditing)
          {
            cell.SetIsEditing(true);
          }
          if (SelectionType == DataGridSelectionType.Cell || SelectionType == DataGridSelectionType.RowAndCell)
          {
            if (cell.Row != null)
            {
              _mouseDownRowIndex = cell.Row.Index;
              _previousSelectionDragRowIndex = _mouseDownRowIndex;
            }
            if (cell.Column != null)
            {
              _mouseDownColumnIndex = EffectiveColumns.IndexOf(cell.Column); // TODO: would be good to improve the performance here.
              _previousSelectionDragColumnIndex = _mouseDownColumnIndex;
            }
            if (SelectionMode == SelectionMode.Single && !cell.IsSelected)
            {
              // If a cell is selected in single mode and RowAndCell type, then make sure there are no selected rows anymore:
              SelectedItem = null;
              SelectedItems.Clear();

              cell.IsSelected = true;
              SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
            }
            else if (SelectionMode == SelectionMode.Multiple)
            {
              if (!cell.IsEditing)
              {
                if (cell.IsSelected)
                {
                  cell.IsSelected = false;
                  // TODO: need to improve the performance here:
                  for (int i = 0; i < SelectedCells.Count; i++)
                  {
                    if (SelectedCells[i].RowContent == cell.Row.Content && SelectedCells[i].Column == cell.Column)
                    {
                      SelectedCells.RemoveAt(i);
                      break;
                    }
                  }
                }
                else
                {
                  cell.IsSelected = true;
                  SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
                }
              }
              else
              {
                cell.IsSelected = true;
                SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
              }
            }
            else if (SelectionMode == SelectionMode.Extended)
            {
              if (KeyboardUtils.IsHoldingShift && SelectedCell != null)
              {
                DataGridCell selectedCell = SelectedCell;
                int startRowIndex = DisplayedItemsSource.IndexOf(SelectedCell.RowContent);
                int startColumnIndex = EffectiveColumns.IndexOf(SelectedCell.Column);
                if (startRowIndex >= 0)
                {
                  if (!KeyboardUtils.IsHoldingCtrl)
                  {
                    SelectedCells.Clear();
                    SelectedItem = null;
                    SelectedItems.Clear();
                  }
                  int endRowIndex = cell.Row.Index;
                  if (endRowIndex < startRowIndex)
                  {
                    int temp = startRowIndex;
                    startRowIndex = endRowIndex;
                    endRowIndex = temp;
                  }
                  int endColumnIndex = EffectiveColumns.IndexOf(cell.Column);
                  if (endColumnIndex < startColumnIndex)
                  {
                    int temp = startColumnIndex;
                    startColumnIndex = endColumnIndex;
                    endColumnIndex = temp;
                  }
                  for (int i = startRowIndex; i <= endRowIndex; i++)
                  {
                    for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
                    {
                      if (columnIndex == startColumnIndex && i == startRowIndex && startColumnIndex != endColumnIndex && startRowIndex != endRowIndex && KeyboardUtils.IsHoldingCtrl)
                      {
                        continue;
                      }
                      object rowContent = DisplayedItemsSource.GetItemAt(i);
                      if (rowContent != null)
                      {
                        SelectedCells.Add(new DataGridCell(rowContent, EffectiveColumns[columnIndex]));
                      }
                    }
                  }
                  SelectedCell = selectedCell;
                }
              }
              else if (KeyboardUtils.IsHoldingCtrl)
              {
                if (!cell.IsEditing)
                {
                  if (cell.IsSelected)
                  {
                    cell.IsSelected = false;
                    // TODO: need to improve the performance here:
                    for (int i = 0; i < SelectedCells.Count; i++)
                    {
                      if (SelectedCells[i].RowContent == cell.Row.Content && SelectedCells[i].Column == cell.Column)
                      {
                        SelectedCells.RemoveAt(i);
                        break;
                      }
                    }
                  }
                  else
                  {
                    cell.IsSelected = true;
                    SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
                  }
                }
                else
                {
                  cell.IsSelected = true;
                  SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
                }
              }
              else
              {
                cell.IsSelected = true;
                SelectedCells.Clear();
                SelectedItem = null;
                SelectedItems.Clear();
                SelectedCell = new DataGridCell(cell.Row.Content, cell.Column);
              }
            }
          }

          _ignoreSelectionUpdateForHighlightedCell = true;
          HighlightedCell = DataGridCell.GetCell(cell);
          _ignoreSelectionUpdateForHighlightedCell = false;
        }
      }
      //SetValue(SelectedDataPropertyKey, SelectedCell != null ? SelectedCell.Value : null);
    }

    private bool _ignoreSelectionUpdateForHighlightedCell;

    internal DataGridValidateCellEventArgs OnCellValueChanged(DataGridCellContainer cell)
    {
      FinalizeNewRowIfNecessary(cell);
      DataGridValidateCellEventArgs args = new DataGridValidateCellEventArgs(DataGridCell.GetCell(cell));
      OnValidateCell(args);
      return args;
    }

    private void FinalizeNewRowIfNecessary(DataGridCellContainer cell)
    {
      if (cell != null)
      {
        if (cell.Row != null)
        {
          if (cell.Row.Content != null && DisplayedItemsSource != null && DisplayedItemsSource.NewRowObject != null)
          {
            if (cell.Content == DisplayedItemsSource.NewRowObject)
            {
              IList itemsSource = ItemsSource as IList;
              if (itemsSource != null)
              {
                object newRowObject = DisplayedItemsSource.NewRowObject;
                itemsSource.Add(newRowObject);
                OnUserAddedRow(new DataGridRowEventArgs(newRowObject));
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Raised whenever the user has finished adding a row to the <see cref="DataGrid"/>.
    /// </summary>
    public event DataGridRowEventHandler UserAddedRow;

    /// <summary>
    /// Raises the UserAddedRow event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnUserAddedRow(DataGridRowEventArgs e)
    {
      DataGridRowEventHandler handler = UserAddedRow;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="ValidateCell"/> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnValidateCell(DataGridValidateCellEventArgs e)
    {
      EventHandler<DataGridValidateCellEventArgs> handler = ValidateCell;
      if (handler != null)
      {
        handler(this, e);
        e.Handled = true;
      }
    }

    /// <summary>
    /// Allows an application to validate the contents of a cell.  This is raised when the user changes the value of a cell.
    /// </summary>
    public event EventHandler<DataGridValidateCellEventArgs> ValidateCell;

    /// <summary>
    /// Raises the <see cref="MouseOverCellChanged"/> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnMouseOverCellChanged(DataGridCellEventArgs e)
    {
      DataGridCellEventHandler handler = MouseOverCellChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Raised whenever the mouse is moved over a different cell.
    /// </summary>
    public event DataGridCellEventHandler MouseOverCellChanged;

    #region IsRightClickSelectionEnabled Property

    /// <summary>
    /// Gets or sets whether or not the right mouse button can be used to select items.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsRightClickSelectionEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsRightClickSelectionEnabled
    {
      get { return (bool)GetValue(IsRightClickSelectionEnabledProperty); }
      set { SetValue(IsRightClickSelectionEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsRightClickSelectionEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsRightClickSelectionEnabledProperty =
      DependencyProperty.Register("IsRightClickSelectionEnabled", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnIsRightClickSelectionEnabledChanged));

    private static void OnIsRightClickSelectionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsRightClickSelectionEnabledChanged();
    }

    private void OnIsRightClickSelectionEnabledChanged()
    {
    }

    #endregion // IsRightClickSelectionEnabled Property

    #region SelectionMode Property

    /// <summary>
    /// Gets or sets the selection mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectionModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public SelectionMode SelectionMode
    {
      get { return (SelectionMode)GetValue(SelectionModeProperty); }
      set { SetValue(SelectionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(DataGrid),
      new FrameworkPropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      if (SelectionType == DataGridSelectionType.Row && SelectionMode == SelectionMode.Single)
      {
        object selectedItem = SelectedItem;
        if (SelectedItems != null)
        {
          SelectedItems.Clear();
        }
        SelectedItem = selectedItem;
      }
      else if (SelectionType == DataGridSelectionType.Cell && SelectionMode == SelectionMode.Single)
      {
        DataGridCell selectedCell = SelectedCell;
        _selectedCells.Clear();
        SelectedCell = selectedCell;
      }
      UpdateCanSelectAll();
    }

    #endregion // SelectionMode Property

    #region SelectionType Property

    /// <summary>
    /// Gets or sets whether the user can select rows or cells.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectionTypeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridSelectionType SelectionType
    {
      get { return (DataGridSelectionType)GetValue(SelectionTypeProperty); }
      set { SetValue(SelectionTypeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionType"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionTypeProperty =
      DependencyProperty.Register("SelectionType", typeof(DataGridSelectionType), typeof(DataGrid),
      new FrameworkPropertyMetadata(DataGridSelectionType.Row, OnSelectionTypeChanged));

    private static void OnSelectionTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSelectionTypeChanged();
    }

    private void OnSelectionTypeChanged()
    {
      if (SelectionType == DataGridSelectionType.Row)
      {
        SelectedCells.Clear();
        if (HighlightedCell != null)
        {
          SelectedItems.Add(HighlightedCell.RowContent);
        }
      }
      else if (SelectionType == DataGridSelectionType.Cell)
      {
        SelectedItems.Clear();
        if (HighlightedCell != null)
        {
          SelectedCells.Add(HighlightedCell);
        }
      }
      UpdateCanSelectAll();
    }

    #endregion // SelectionType Property

    #region DisplayedItemsSource Property

    /// <summary>
    /// Gets the collection of items being displayed by this <see cref="DataGrid"/>. This collection manages sorting and paging.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DisplayedItemsSourceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridItemsSource DisplayedItemsSource
    {
      get { return (DataGridItemsSource)GetValue(DisplayedItemsSourceProperty); }
    }

    private static readonly DependencyPropertyKey DisplayedItemsSourcePropertyKey =
        DependencyProperty.RegisterReadOnly("DisplayedItemsSource", typeof(DataGridItemsSource), typeof(DataGrid), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="DisplayedItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty DisplayedItemsSourceProperty =
        DisplayedItemsSourcePropertyKey.DependencyProperty;

    #endregion // DisplayedItemsSource Property

    #region PageSize Property

    /// <summary>
    /// Gets or sets the number of items per page.
    /// By default this value is zero which indicates all items are on one page.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PageSizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int PageSize
    {
      get { return (int)GetValue(PageSizeProperty); }
      set { SetValue(PageSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PageSize"/> property.
    /// </summary>
    public static readonly DependencyProperty PageSizeProperty =
      DependencyProperty.Register("PageSize", typeof(int), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnPageSizeChanged));

    private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnPageSizeChanged();
    }

    private void OnPageSizeChanged()
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.PageSize = PageSize;
      }
    }

    #endregion // PageSize Property

    #region PageIndex Property

    /// <summary>
    /// Gets or sets the index of the currently displayed page.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PageIndexProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int PageIndex
    {
      get { return (int)GetValue(PageIndexProperty); }
      set { SetValue(PageIndexProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PageIndex"/> property.
    /// </summary>
    public static readonly DependencyProperty PageIndexProperty =
      DependencyProperty.Register("PageIndex", typeof(int), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnPageIndexChanged));

    private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnPageIndexChanged();
    }

    private void OnPageIndexChanged()
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.PageIndex = PageIndex;
      }
    }

    #endregion // PageIndex Property

    #region IsPagerVisible Property

    // TODO: change this to Visibility type rather than boolean?

    /// <summary>
    /// Gets or sets whether the built in pager control is visible. If set to null, the pager will be visible only if there is more than 1 page.
    /// The default is null.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsPagerVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool? IsPagerVisible
    {
      get { return (bool?)GetValue(IsPagerVisibleProperty); }
      set { SetValue(IsPagerVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsPagerVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsPagerVisibleProperty =
      DependencyProperty.Register("IsPagerVisible", typeof(bool?), typeof(DataGrid), new PropertyMetadata(null));

    #endregion // IsPagerVisible Property

    #region PagerStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> for the built in <see cref="DataGridPager"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PagerStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style PagerStyle
    {
      get { return (Style)GetValue(PagerStyleProperty); }
      set { SetValue(PagerStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PagerStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty PagerStyleProperty =
      DependencyProperty.Register("PagerStyle", typeof(Style), typeof(DataGrid),
      new FrameworkPropertyMetadata());

    #endregion // PagerStyle Property

    #region MaxPagerButtonCount Property

    /// <summary>
    /// Gets or sets the maximum number of pager buttons that the built-in <see cref="DataGridPager"/> can display.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxPagerButtonCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MaxPagerButtonCount
    {
      get { return (int)GetValue(MaxPagerButtonCountProperty); }
      set { SetValue(MaxPagerButtonCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxPagerButtonCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxPagerButtonCountProperty =
      DependencyProperty.Register("MaxPagerButtonCount", typeof(int), typeof(DataGrid),
      new FrameworkPropertyMetadata(9, OnMaxPagerButtonCountChanged));

    private static void OnMaxPagerButtonCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnMaxPagerButtonCountChanged();
    }

    private void OnMaxPagerButtonCountChanged()
    {
    }

    #endregion // MaxPagerButtonCount Property

    #region EllipsisMode Property

    /// <summary>
    /// Gets or sets the <see cref="EllipsisMode"/> of the built-in <see cref="DataGridPager"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EllipsisModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public EllipsisMode EllipsisMode
    {
      get { return (EllipsisMode)GetValue(EllipsisModeProperty); }
      set { SetValue(EllipsisModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EllipsisMode"/> property.
    /// </summary>
    public static readonly DependencyProperty EllipsisModeProperty =
      DependencyProperty.Register("EllipsisMode", typeof(EllipsisMode), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnEllipsisModeChanged));

    private static void OnEllipsisModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnEllipsisModeChanged();
    }

    private void OnEllipsisModeChanged()
    {
    }

    #endregion // EllipsisMode Property

    #region IsFilterRowVisible Property

    /// <summary>
    /// Gets or sets whether or not the filter row is visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsFilterRowVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsFilterRowVisible
    {
      get { return (bool)GetValue(IsFilterRowVisibleProperty); }
      set { SetValue(IsFilterRowVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsFilterRowVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsFilterRowVisibleProperty =
      DependencyProperty.Register("IsFilterRowVisible", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnIsFilterRowVisibleChanged));

    private static void OnIsFilterRowVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsFilterRowVisibleChanged();
    }

    private void OnIsFilterRowVisibleChanged()
    {
    }

    #endregion // IsFilterRowVisible Property

    #region IsColumnHeaderFilterVisible Property

    /// <summary>
    /// Gets or sets whether or not the column header filter buttons are visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsColumnHeaderFilterVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsColumnHeaderFilterVisible
    {
      get { return (bool)GetValue(IsColumnHeaderFilterVisibleProperty); }
      set { SetValue(IsColumnHeaderFilterVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsColumnHeaderFilterVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsColumnHeaderFilterVisibleProperty =
      DependencyProperty.Register("IsColumnHeaderFilterVisible", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnIsColumnHeaderFilterVisibleChanged));

    private static void OnIsColumnHeaderFilterVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsColumnHeaderFilterVisibleChanged();
    }

    private void OnIsColumnHeaderFilterVisibleChanged()
    {
    }

    #endregion // IsColumnHeaderFilterVisible Property

    #region IsFooterVisible Property

    /// <summary>
    /// Gets or sets whether or not the footer is visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsFooterVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsFooterVisible
    {
      get { return (bool)GetValue(IsFooterVisibleProperty); }
      set { SetValue(IsFooterVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsFooterVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsFooterVisibleProperty =
      DependencyProperty.Register("IsFooterVisible", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnIsFooterVisibleChanged));

    private static void OnIsFooterVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnIsFooterVisibleChanged();
    }

    private void OnIsFooterVisibleChanged()
    {
    }

    #endregion // IsFooterVisible Property

    #region IsGroupingPanelVisible Property

    /// <summary>
    /// Gets or sets whether the built in grouping panel is visible. The grouping panel allows users to group the data by columns.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsGroupingPanelVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsGroupingPanelVisible
    {
      get { return (bool)GetValue(IsGroupingPanelVisibleProperty); }
      set { SetValue(IsGroupingPanelVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsGroupingPanelVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsGroupingPanelVisibleProperty =
      DependencyProperty.Register("IsGroupingPanelVisible", typeof(bool), typeof(DataGrid));

    #endregion // IsGroupingPanelVisible Property

    #region GroupdColumns Property

    /// <summary>
    /// Gets the collection of grouped columns in the order of their grouping.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupedColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<DataGridColumn> GroupedColumns
    {
      get { return (ObservableCollection<DataGridColumn>)GetValue(GroupedColumnsProperty); }
    }

    private static readonly DependencyPropertyKey GroupedColumnsPropertyKey =
        DependencyProperty.RegisterReadOnly("GroupedColumns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="GroupedColumns"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupedColumnsProperty = GroupedColumnsPropertyKey.DependencyProperty;

    #endregion // GroupdColumns Property

    #region GroupingIndent Property

    /// <summary>
    /// Gets or sets the number of pixels to indent a single level in the grouping tree.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupingIndentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double GroupingIndent
    {
      get { return (double)GetValue(GroupingIndentProperty); }
      set { SetValue(GroupingIndentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupingIndent"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupingIndentProperty =
      DependencyProperty.Register("GroupingIndent", typeof(double), typeof(DataGrid),
      new FrameworkPropertyMetadata(20.0, OnGroupingIndentChanged));

    private static void OnGroupingIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnGroupingIndentChanged();
    }

    private void OnGroupingIndentChanged()
    {
    }

    #endregion // GroupingIndent Property

    // TODO: should we rename this to 'AllowsColumnReorder' to be consistant with the MultiColumnTreeView? (Notice the 's' on 'Allows')
    // TODO: should this property prevent reordering the columns in the data model?
    // TODO: AllowColumnReorder tests

    #region AllowColumnReorder Property

    /// <summary>
    /// Gets or sets whether the user can reorder the columns.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowColumnReorderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowColumnReorder
    {
      get { return (bool)GetValue(AllowColumnReorderProperty); }
      set { SetValue(AllowColumnReorderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowColumnReorder"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowColumnReorderProperty =
      DependencyProperty.Register("AllowColumnReorder", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true));

    #endregion // AllowColumnReorder Property

    // TODO: do we really need a ShowColumnHeaders property? Should this be replaced with a ColumnHeaderRowStyle property where a null style doesn't display anything?

    #region ShowColumnHeaders Property

    /// <summary>
    /// Gets or sets whether the column headers are visible.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowColumnHeadersProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowColumnHeaders
    {
      get { return (bool)GetValue(ShowColumnHeadersProperty); }
      set { SetValue(ShowColumnHeadersProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowColumnHeaders"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowColumnHeadersProperty =
      DependencyProperty.Register("ShowColumnHeaders", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true));

    #endregion // ShowColumnHeaders Property

    #region ShowRowHeaders Property

    /// <summary>
    /// Gets or sets whether the row headers are visible.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowRowHeadersProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowRowHeaders
    {
      get { return (bool)GetValue(ShowRowHeadersProperty); }
      set { SetValue(ShowRowHeadersProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowRowHeaders"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowRowHeadersProperty =
      DependencyProperty.Register("ShowRowHeaders", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnShowRowHeadersChanged));

    private static void OnShowRowHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnShowRowHeadersChanged();
    }

    private void OnShowRowHeadersChanged()
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.UpdateRowHeaderWidth(ShowRowHeaders ? RowHeaderWidth : 0);
      }
    }

    #endregion // ShowRowHeaders Property

    #region RowHeaderWidth Property

    /// <summary>
    /// Gets or sets the width of the row header block.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RowHeaderWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double RowHeaderWidth
    {
      get { return (double)GetValue(RowHeaderWidthProperty); }
      set { SetValue(RowHeaderWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RowHeaderWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty RowHeaderWidthProperty =
      DependencyProperty.Register("RowHeaderWidth", typeof(double), typeof(DataGrid),
      new FrameworkPropertyMetadata(22.0, OnRowHeaderWidthChanged));

    private static void OnRowHeaderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnRowHeaderWidthChanged();
    }

    private void OnRowHeaderWidthChanged()
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.UpdateRowHeaderWidth(ShowRowHeaders ? RowHeaderWidth : 0);
      }
    }

    #endregion // RowHeaderWidth Property

    #region RowHeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display custom content within row headers.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RowHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate RowHeaderTemplate
    {
      get { return (DataTemplate)GetValue(RowHeaderTemplateProperty); }
      set { SetValue(RowHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RowHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty RowHeaderTemplateProperty =
      DependencyProperty.Register("RowHeaderTemplate", typeof(DataTemplate), typeof(DataGrid),
      new FrameworkPropertyMetadata(new DataTemplate(), OnRowHeaderTemplateChanged));

    private static void OnRowHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnRowHeaderTemplateChanged();
    }

    private void OnRowHeaderTemplateChanged()
    {
    }

    #endregion // RowHeaderTemplate Property

    #region GroupRowHeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display custom content within group row headers;
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupRowHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate GroupRowHeaderTemplate
    {
      get { return (DataTemplate)GetValue(GroupRowHeaderTemplateProperty); }
      set { SetValue(GroupRowHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupRowHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupRowHeaderTemplateProperty =
      DependencyProperty.Register("GroupRowHeaderTemplate", typeof(DataTemplate), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnGroupRowHeaderTemplateChanged));

    private static void OnGroupRowHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnGroupRowHeaderTemplateChanged();
    }

    private void OnGroupRowHeaderTemplateChanged()
    {
    }

    #endregion // GroupRowHeaderTemplate Property

    #region ColumnHeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to visualize column header content.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ColumnHeaderTemplate
    {
      get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
      set { SetValue(ColumnHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderTemplateProperty =
      DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnColumnHeaderTemplateChanged));

    private static void OnColumnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnColumnHeaderTemplateChanged();
    }

    private void OnColumnHeaderTemplateChanged()
    {
    }

    #endregion // ColumnHeaderTemplate Property

    #region AllowEditing Property

    /// <summary>
    /// Gets or sets whether the user can edit any of the cells in this <see cref="DataGrid"/>.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowEditingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowEditing
    {
      get { return (bool)GetValue(AllowEditingProperty); }
      set { SetValue(AllowEditingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowEditing"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowEditingProperty =
      DependencyProperty.Register("AllowEditing", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnAllowEditingChanged));

    private static void OnAllowEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnAllowEditingChanged();
    }

    private void OnAllowEditingChanged()
    {
      DisableHighlightedCellEditing();
    }

    #endregion // AllowEditing Property

    #region HierarchyMode Property

    /// <summary>
    /// Gets or sets how to display hierarchical data when it is loaded. The default is collapsed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HierarchyModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridHierarchyMode HierarchyMode
    {
      get { return (DataGridHierarchyMode)GetValue(HierarchyModeProperty); }
      set { SetValue(HierarchyModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HierarchyMode"/> property.
    /// </summary>
    public static readonly DependencyProperty HierarchyModeProperty =
      DependencyProperty.Register("HierarchyMode", typeof(DataGridHierarchyMode), typeof(DataGrid),
      new FrameworkPropertyMetadata(DataGridHierarchyMode.Collapsed, OnHierarchyModeChanged));

    private static void OnHierarchyModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnHierarchyModeChanged();
    }

    private void OnHierarchyModeChanged()
    {
    }

    #endregion // HierarchyMode Property

    /// <summary>
    /// Resets all sorting, grouping and filtering on the displayed items source.
    /// </summary>
    public void ResetDisplayedItemsSource()
    {
      if (DisplayedItemsSource != null)
      {
        GroupedColumns.Clear();
        foreach (DataGridColumn column in EffectiveColumns)
        {
          column.SortDirection = SortDirection.None;
          column.Filter = null;
        }
      }
    }

    /// <summary>
    /// Expands the given data object if it has children.
    /// </summary>
    /// <param name="item">The hierarchical parent to expand.</param>
    public void Expand(object item)
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.Expand(item);
      }
    }

    /// <summary>
    /// Collapses the given data object if it has children.
    /// </summary>
    /// <param name="item">The hierarchical parent to collapse.</param>
    public void Collapse(object item)
    {
      if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.Collapse(item);
      }
    }

    internal bool OnHierarchicalStateChanging(object item, bool isExpanding)
    {
      EventHandler<DataGridHierarchicalItemStateChangingEventArgs> handler = HierarchicalItemStateChanging;
      DataGridHierarchicalItemStateChangingEventArgs eventArgs = new DataGridHierarchicalItemStateChangingEventArgs(item, isExpanding);
      if (handler != null)
      {
        handler(this, eventArgs);
      }
      return eventArgs.CanChangeState;
    }

    /// <summary>
    /// Rasied when a hierarchical data item is requesting to be expanded or collapsed.
    /// </summary>
    public event EventHandler<DataGridHierarchicalItemStateChangingEventArgs> HierarchicalItemStateChanging;

    // TODO: how to provide API for seeing what cells/rows can or can not be edited?
    //       Answer: maybe provide an event or command that the developer can hook into to return the editability of the given row/cell. This should have good performance for complex situations.

    // TODO what to do if rows are removed?
    private HashSet<object> _uneditableRows = new HashSet<object>();

    // TODO: this API isn't very discoverable. Is it?
    /// <summary>
    /// Sets whether or not the given object can be edited by the user.
    /// </summary>
    /// <param name="rowContent">A data object displayed by the data grid.</param>
    /// <param name="allowEditing">Whether or not the user can edit the given object.</param>
    public void SetAllowEditing(object rowContent, bool allowEditing)
    {
      if (!allowEditing && !_uneditableRows.Contains(rowContent))
      {
        _uneditableRows.Add(rowContent);
        if (HighlightedCell != null && HighlightedCell.RowContent == rowContent)
        {
          DisableHighlightedCellEditing();
        }
      }
      else if (allowEditing)
      {
        _uneditableRows.Remove(rowContent);
      }
    }

    private Dictionary<object, Brush> _rowBackgrounds = new Dictionary<object, Brush>();

    /// <summary>
    /// Sets the background brush of the given row.
    /// </summary>
    /// <param name="rowContent">The row content to identify the desired row.</param>
    /// <param name="background">The background brush to apply to the row.</param>
    public void SetRowBackground(object rowContent, Brush background)
    {
      _rowBackgrounds[rowContent] = background;
      if (_dataGridPanel != null)
      {
        DataGridRow row = _dataGridPanel.GetDataGridRow(rowContent);
        if (row != null)
        {
          row.Background = GetRowBackground(row);
        }
      }
    }

    internal Brush GetRowBackground(DataGridRow row)
    {
      Brush background;
      _rowBackgrounds.TryGetValue(row.Content, out background);
      if (background != null)
      {
        return background;
      }
      if (AlternatingRowBackground != null && row.Index % 2 != 0)
      {
        return AlternatingRowBackground;
      }
      return Brushes.Transparent;
    }

    // TODO: what to do if rows/columns are removed?
    private Dictionary<DataGridColumn, HashSet<object>> _uneditableCells = new Dictionary<DataGridColumn, HashSet<object>>();

    // TODO: this API isn't very discoverable. Is it?
    /// <summary>
    /// Sets whether or not the given cell can be edited by the user.
    /// </summary>
    /// <param name="cellModel">A <see cref="DataGridCell"/> model object.</param>
    /// <param name="allowEditing">Whether or not the user can edit the value in the given cell.</param>
    public void SetAllowEditing(DataGridCell cellModel, bool allowEditing)
    {
      if (!allowEditing)
      {
        HashSet<object> rows;
        _uneditableCells.TryGetValue(cellModel.Column, out rows);
        if (rows == null)
        {
          rows = new HashSet<object>();
          _uneditableCells[cellModel.Column] = rows;
        }
        if (!rows.Contains(cellModel.RowContent))
        {
          rows.Add(cellModel.RowContent);
          if (HighlightedCell != null && HighlightedCell.Column == cellModel.Column && HighlightedCell.RowContent == cellModel.RowContent)
          {
            DisableHighlightedCellEditing();
          }
        }
      }
      else
      {
        HashSet<object> rows;
        _uneditableCells.TryGetValue(cellModel.Column, out rows);
        if (rows != null)
        {
          rows.Remove(cellModel.RowContent);
          if (rows.Count == 0)
          {
            _uneditableCells.Remove(cellModel.Column);
          }
        }
      }
    }

    // TODO: maybe this should map rows first and then columns.
    private readonly Dictionary<DataGridColumn, Dictionary<object, Brush>> _cellBackgrounds = new Dictionary<DataGridColumn, Dictionary<object, Brush>>();

    /// <summary>
    /// Sets the background brush of the gievn cell.
    /// </summary>
    /// <param name="cellModel">A <see cref="DataGridCell"/> model object.</param>
    /// <param name="background">The background brush for the given cell.</param>
    public void SetCellBackground(DataGridCell cellModel, Brush background)
    {
      Dictionary<object, Brush> rows;
      _cellBackgrounds.TryGetValue(cellModel.Column, out rows);
      if (rows == null)
      {
        rows = new Dictionary<object, Brush>();
        _cellBackgrounds[cellModel.Column] = rows;
      }
      rows[cellModel.RowContent] = background;
      DataGridCellContainer container = GetCellContainer(cellModel);
      if (container != null)
      {
        container.Background = GetCellBackground(cellModel);
      }
    }

    internal Brush GetCellBackground(DataGridCell cellModel)
    {
      Dictionary<object, Brush> rows;
      _cellBackgrounds.TryGetValue(cellModel.Column, out rows);
      if (rows != null)
      {
        Brush background;
        rows.TryGetValue(cellModel.RowContent, out background);
        if (background != null)
        {
          return background;
        }
      }
      return cellModel.Column == null ? Brushes.Transparent : cellModel.Column.Background;
    }

    internal static readonly GridLength DefaultColumnWidth = new GridLength(100);

    private readonly IList<DataGridColumn> _oldColumns = new List<DataGridColumn>();

    // TODO: on startup this method seems to be called twice. Find a way to prevent this.
    private void RecalculateEffectiveColumns()
    {
      _oldColumns.Clear();
      if (EffectiveColumns != null)
      {
        foreach (DataGridColumn column in EffectiveColumns)
        {
          column.SortDirectionChanged -= new EventHandler(DataGridColumn_SortDirectionChanged);
          column.FilterChanged -= new EventHandler(DataGridColumn_FilterChanged);
          column.AllowEditingChanged -= new EventHandler(DataGridColumn_AllowEditingChanged);
          column.BackgroundChanged -= new EventHandler(DataGridColumn_BackgroundChanged);
          column.ForegroundChanged -= new EventHandler(DataGridColumn_ForegroundChanged);
          column.IsVisibleChanged -= new EventHandler(DataGridColumn_IsVisibleChanged);
          column.TemplateChanged -= new EventHandler(DataGridColumn_TemplateChanged);
          _oldColumns.Add(column);
        }
      }
      if (EffectiveColumns == null)
      {
        SetValue(EffectiveColumnsPropertyKey, new ObservableCollection<DataGridColumn>());
      }
      ObservableCollection<DataGridColumn> columns = EffectiveColumns;
      //List<DataGridColumn> newColumns = new List<DataGridColumn>();
      //if (!_loadedColumns && IsLoaded)
      {
        columns.Clear();
        //_loadedColumns = true;
      }
      foreach (var columnInfo in GetColumnsToDisplay())
      {
        //column.Bind(GridViewColumn.WidthProperty, columnInfo.Column, DataGridColumn.WidthProperty, DefaultColumnWidth);
        //column.Bind(GridViewColumn.HeaderProperty, columnInfo.Column, DataGridColumn.HeaderProperty, columnInfo.SafePropertyDisplayName);
        if (columnInfo.Column.Header == null || columnInfo.Column.IsHeaderAutoSet)
        {
          columnInfo.Column.Header = columnInfo.SafePropertyDisplayName;
          columnInfo.Column.IsHeaderAutoSet = true;
        }
        DataGridColumn column = columnInfo.Column;
        if (columnInfo.Column != null)
        {
          if (column.DisplayMemberBinding != null)
          {
            if (column.DisplayTemplate == null)
            {
              column.UseDisplayTemplate = false;
            }
            if (column.EditorTemplateSelector == null)
            {
              column.UseEditorTemplateSelector = false;
            }
          }
          // Here we check the DisplayMemberBinding for scenarios where the grid starts out with a null ItemsSource which gets set later.
          // Without this check, the next pass of this method finds that the DisplayTemplate property is already set.
          // It has been set from the previous pass which automated the template based on the DisplayTemplate.
          // Thus without this check, the DisplayTemplate and EditorTemplateSelector don't get re-automated. This would crash the application when a user tries to edit a cell in the column.
          if (columnInfo.Column.DisplayTemplate != null && column.UseDisplayTemplate)
          {
            column.DisplayTemplate = columnInfo.Column.DisplayTemplate;
          }
          else if (columnInfo.Column.DisplayTemplateSelector != null)
          {
            column.DisplayTemplateSelector = columnInfo.Column.DisplayTemplateSelector;
          }
          else if (columnInfo.PropertyInfo != null)
          {
            column.DisplayTemplate = BuildTemplate(columnInfo.PropertyInfo);
          }
          if (columnInfo.Column.EditorTemplate != null)
          {
            column.EditorTemplate = columnInfo.Column.EditorTemplate;
          }
          else if (columnInfo.Column.EditorTemplateSelector != null && column.UseEditorTemplateSelector)
          {
            column.EditorTemplateSelector = columnInfo.Column.EditorTemplateSelector;
          }
          else if (columnInfo.PropertyInfo != null)
          {
            column.EditorTemplateSelector = new DataGridEditorSelector(columnInfo.PropertyInfo, EditorSelector);
          }
        }
        else if (columnInfo.PropertyInfo != null)
        {
          column.DisplayTemplate = BuildTemplate(columnInfo.PropertyInfo);
          column.EditorTemplateSelector = new DataGridEditorSelector(columnInfo.PropertyInfo, EditorSelector);
        }

        column.PropertyInfo = columnInfo.PropertyInfo;

        //if (column.Foreground == null && IsLoaded)
        //{
        //  column.Foreground = Foreground;
        //}

        column.SortDirectionChanged += new EventHandler(DataGridColumn_SortDirectionChanged);
        column.FilterChanged += new EventHandler(DataGridColumn_FilterChanged);
        column.AllowEditingChanged += new EventHandler(DataGridColumn_AllowEditingChanged);
        column.BackgroundChanged += new EventHandler(DataGridColumn_BackgroundChanged);
        column.ForegroundChanged += new EventHandler(DataGridColumn_ForegroundChanged);
        column.IsVisibleChanged += new EventHandler(DataGridColumn_IsVisibleChanged);
        column.TemplateChanged += new EventHandler(DataGridColumn_TemplateChanged);

        column.IsLoaded = true;
        //if (!columns.Contains(column))
        {
          columns.Add(column);
        }
        //newColumns.Add(column);
      }

      /*for (int i = columns.Count - 1; i >= 0; i--)
      {
        if (!newColumns.Contains(columns[i]))
        {
          columns.RemoveAt(i);
        }
      }*/
    }

    private void DataGridColumn_FilterChanged(object sender, EventArgs e)
    {
      if (DisplayedItemsSource != null)
      {
        AndFilter expression = new AndFilter();
        int count = 0;
        foreach (DataGridColumn column in EffectiveColumns)
        {
          if (column.Filter != null)
          {
            expression.Add(new PropertyFilter(column.PropertyInfo.AsPropertyInfo, column.Filter));
            count++;
          }
        }
        DisplayedItemsSource.FilterExpression = count > 0 ? expression : null;
      }
    }

    private void DataGridColumn_TemplateChanged(object sender, EventArgs e)
    {
      RecalculateEffectiveColumns();
    }

    private void DataGridColumn_IsVisibleChanged(object sender, EventArgs e)
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.InvalidateRequiresMeasure();        
      }
    }

    private void DataGridColumn_ForegroundChanged(object sender, EventArgs e)
    {
      if (_dataGridPanel != null)
      {
        _dataGridPanel.ForceReRender();
      }
    }

    private void DataGridColumn_BackgroundChanged(object sender, EventArgs e)
    {
      if (_dataGridPanel != null)
      {
        _dataGridPanel.ForceReRender();
      }
    }

    private void DataGridColumn_AllowEditingChanged(object sender, EventArgs e)
    {
      if (HighlightedCell != null)
      {
        DataGridColumn column = sender as DataGridColumn;
        if (!column.AllowEditing && column == HighlightedCell.Column)
        {
          DisableHighlightedCellEditing();
        }
      }
    }

    private void DataGridColumn_SortDirectionChanged(object sender, EventArgs e)
    {
      if (!IgnoreSortDirectionChanged)
      {
        DataGridColumn column = sender as DataGridColumn;
        Dispatcher.BeginInvoke(new UpdateSortDelegate(UpdateSort), column);
      }
    }

    private delegate void UpdateSortDelegate(DataGridColumn column);

    /// <summary>
    /// Gets or sets a custom sorting function. This is to provide more flexibility than custom sort comparers.
    /// </summary>
    public Action<List<object>, DataGridColumn> CustomSort { get; set; }

    private void UpdateSort(DataGridColumn column)
    {
      var comparer = column.SortComparer;
      if (comparer == null)
      {
        IPropertyInfo info = column.PropertyInfo;
        if (info != null && DisplayedItemsSource != null)
        {
          DisplayedItemsSource.CustomSort = CustomSort;
          DisplayedItemsSource.SortColumn = column;
          DisplayedItemsSource.Sort(info.AsPropertyInfo, column.SortDirection);
        }
      }
      else if (DisplayedItemsSource != null)
      {
        DisplayedItemsSource.CustomSort = CustomSort;
        DisplayedItemsSource.SortColumn = column;
        DisplayedItemsSource.Sort(comparer, column.SortDirection);
      }
    }

    internal bool IgnoreSortDirectionChanged { get; set; }

    private IEnumerable<DataGridColumnInfo> GetColumnsToDisplay()
    {
      if (ItemsSource is DataTableWrapper)
      {
        // DataTable support
        DataTableWrapper wrapper = ItemsSource as DataTableWrapper;
        if (AutoGenerateColumns)
        {
          foreach (DataColumn dataColumn in wrapper.DataTable.Columns)
          {
            DataGridColumn column = FindExistingColumn(dataColumn.ColumnName);
            if (column == null)
            {
              column = new DataGridColumn() { PropertyName = dataColumn.ColumnName };
            }
            yield return new DataGridColumnInfo() { Column = column, PropertyInfo = new DataTablePropertyInfoAdapter(dataColumn) };
          }
        }
        else
        {
          foreach (var column in Columns)
          {
            var columnInfo = new DataGridColumnInfo { Column = column };
            if (column.PropertyName != null)
            {
              var dataColumn = wrapper.DataTable.Columns[column.PropertyName];
              if (dataColumn != null)
              {
                IPropertyInfo propertyInfo = new DataTablePropertyInfoAdapter(dataColumn);
                columnInfo.PropertyInfo = propertyInfo;
              }
            }
            yield return columnInfo;
          }
        }
        // End DataTable support
      }
      else if (ItemsSource is DataView)
      {
        // DataView support
        DataView view = ItemsSource as DataView;
        if (AutoGenerateColumns)
        {
          foreach (DataColumn dataColumn in view.Table.Columns)
          {
            DataGridColumn column = FindExistingColumn(dataColumn.ColumnName);
            if (column == null)
            {
              column = new DataGridColumn() { PropertyName = dataColumn.ColumnName };
            }
            yield return new DataGridColumnInfo() { Column = column, PropertyInfo = new DataTablePropertyInfoAdapter(dataColumn) };
          }
        }
        else
        {
          foreach (var column in Columns)
          {
            var columnInfo = new DataGridColumnInfo { Column = column };
            if (column.PropertyName != null)
            {
              var dataColumn = view.Table.Columns[column.PropertyName];
              if (dataColumn != null)
              {
                IPropertyInfo propertyInfo = new DataTablePropertyInfoAdapter(dataColumn);
                columnInfo.PropertyInfo = propertyInfo;
              }
            }
            yield return columnInfo;
          }
        }
        // End DataView support
      }
      else
      {
        PropertyDescriptorCollection pds;

        if (DesignerProperties.GetIsInDesignMode(this))
        {
          // Design time logic
          pds = TypeDescriptor.GetProperties(typeof(SampleObject));
        }
        else if (ItemsSource == null)
        {
          pds = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
        }
        else if (ItemsSource is ITypedList)
        {
          pds = ((ITypedList)ItemsSource).GetItemProperties(null);
        }
        else
        {
          Type t = CollectionUtilities.GetCollectionValueType(ItemsSource);
          pds = TypeDescriptor.GetProperties(t);
        }

        if (AutoGenerateColumns)
        {
          foreach (PropertyDescriptor propertyDescriptor in pds)
          {
            IPropertyInfo propertyInfo = new DescriptorPropertyInfoAdapter(propertyDescriptor);
            if (propertyInfo.CanRead && propertyInfo.IsBrowsable)
            {
              DataGridColumn column = FindExistingColumn(propertyDescriptor.Name);
              if (column == null)
              {
                column = new DataGridColumn() { PropertyName = propertyDescriptor.Name };
              }
              if (IsLoaded && column.DisplayMemberBinding != null)
              {
                Type rowType = ItemsSource == null ? null : CollectionUtilities.GetCollectionValueType(ItemsSource);
                IPropertyInfo bindingAdapter = new BindingPropertyInfoAdapter(column.DisplayMemberBinding, rowType);
                if (bindingAdapter.CanRead)
                {
                  propertyInfo = bindingAdapter;
                }
              }
              yield return new DataGridColumnInfo { Column = column, PropertyInfo = propertyInfo };
            }
          }
        }
        else if (Columns != null)
        {
          Type rowType = ItemsSource == null ? null : CollectionUtilities.GetCollectionValueType(ItemsSource);
          foreach (var column in Columns)
          {
            var columnInfo = new DataGridColumnInfo { Column = column };
            if (IsLoaded && column.DisplayMemberBinding != null)
            {
              IPropertyInfo propertyInfo = new BindingPropertyInfoAdapter(column.DisplayMemberBinding, rowType);
              if (propertyInfo.CanRead)
              {
                columnInfo.PropertyInfo = propertyInfo;
              }
            }
            else if (column.PropertyName != null)
            {
              var descriptor = pds[column.PropertyName];
              if (descriptor != null)
              {
                IPropertyInfo propertyInfo = new DescriptorPropertyInfoAdapter(descriptor);
                if (propertyInfo.CanRead)
                {
                  columnInfo.PropertyInfo = propertyInfo;
                }
              }
            }
            yield return columnInfo;
          }
        }
      }
    }

    private DataGridColumn FindExistingColumn(string name)
    {
      DataGridColumn column = null;
      if (name != null)
      {
        column = Columns.FirstOrDefault(c => name.Equals(c.PropertyName));
        if (column == null)
        {
          column = _oldColumns.FirstOrDefault(c => name.Equals(c.PropertyName));
        }
        if (column != null)
        {
          column.SortDirection = SortDirection.None;
        }
      }
      return column;
    }

    #region EffectiveColumns Property

    /// <summary>
    /// Gets the actual columns displayed by this <see cref="DataGrid"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EffectiveColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<DataGridColumn> EffectiveColumns
    {
      get { return (ObservableCollection<DataGridColumn>)GetValue(EffectiveColumnsProperty); }
    }

    private static readonly DependencyPropertyKey EffectiveColumnsPropertyKey =
        DependencyProperty.RegisterReadOnly("EffectiveColumns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="EffectiveColumns"/> property.
    /// </summary>
    public static readonly DependencyProperty EffectiveColumnsProperty =
        EffectiveColumnsPropertyKey.DependencyProperty;

    #endregion // EffectiveColumns Property

    /// <summary>
    /// Gets the resource key for the <see cref="ScrollViewer"/> style used by this <see cref="DataGrid"/>.
    /// </summary>
    public static ComponentResourceKey ScrollViewerStyleKey
    {
      get { return new ComponentResourceKey(typeof(DataGrid), "ScrollViewerStyle"); }
    }

    #region DefaultMargin Property

    /// <summary>
    /// Gets or sets the default margin around each control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DefaultMarginProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Thickness DefaultMargin
    {
      get { return (Thickness)GetValue(DefaultMarginProperty); }
      set { SetValue(DefaultMarginProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty DefaultMarginProperty =
      DependencyProperty.Register("DefaultMargin", typeof(Thickness), typeof(DataGrid));

    #endregion // DefaultMargin Property

    /// <summary>
    /// Gets whether or not default margin compensation is required.
    /// </summary>
    public bool DefaultMarginCompensationRequired
    {
      get { return false; }
    }

    #region AutoGenerateColumns Property

    /// <summary>
    /// Gets or sets whether the effective columns collection should be automatically be generated.
    /// Generated columns will be based on the gettable properties on the objects added to this <see cref="DataGrid"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AutoGenerateColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AutoGenerateColumns
    {
      get { return (bool)GetValue(AutoGenerateColumnsProperty); }
      set { SetValue(AutoGenerateColumnsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoGenerateColumns"/> property.
    /// </summary>
    public static readonly DependencyProperty AutoGenerateColumnsProperty =
      DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnAutoGenerateColumnsChanged));

    private static void OnAutoGenerateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnAutoGenerateColumnsChanged();
    }

    private void OnAutoGenerateColumnsChanged()
    {
      RecalculateEffectiveColumns();
    }

    #endregion // AutoGenerateColumns Property

    //private readonly ObservableCollection<DataGridColumn> _columns = new ObservableCollection<DataGridColumn>();

    /*/// <summary>
    /// Gets the collection of column definitions. Custom columns can be added to the data grid via this collection.
    /// </summary>
    public Collection<DataGridColumn> Columns
    {
      get { return _columns; }
    }*/

    #region Columns Property

    /// <summary>
    /// Gets the collection of column definitions. Custom columns can be added to the data grid via this collection.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<Mindscape.WpfElements.WpfDataGrid.DataGridColumn> Columns
    {
      get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Columns"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnsProperty =
      DependencyProperty.Register("Columns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnColumnsChanged));

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnColumnsChanged();
    }

    private void OnColumnsChanged()
    {
      RecalculateEffectiveColumns();
    }

    #endregion // Columns Property

    #region Editor extensibility

    private EditorSelector _editorSelector;

    /// <summary>
    /// Gets the template selector used to create value editors.
    /// </summary>
    public DataTemplateSelector EditorSelector
    {
      get { return _editorSelector; }
    }

    private readonly EditorCollection _editors;
    private readonly EditorDecorationCollection _editorDecorations = new EditorDecorationCollection();

    /// <summary>
    /// Gets the collection of custom editors for this <see cref="DataGrid"/> instance.
    /// </summary>
    /// <remarks>Add editors to this collection to define how to edit custom types or
    /// to customise the display of specific properties.</remarks>
    public EditorCollection Editors
    {
      get { return _editors; }
    }

    EditorDecorationCollection IExtendInPlaceEditors.EditorDecorations
    {
      get { return _editorDecorations; }
    }

    /// <summary>
    /// Gets or sets the styles applied to built-in editors.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BuiltInEditorStylesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public BuiltInEditorStyleCollection BuiltInEditorStyles
    {
      get { return (BuiltInEditorStyleCollection)GetValue(BuiltInEditorStylesProperty); }
      set { SetValue(BuiltInEditorStylesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BuiltInEditorStyles"/> property.
    /// </summary>
    public static readonly DependencyProperty BuiltInEditorStylesProperty =
      DependencyProperty.Register("BuiltInEditorStyles", typeof(BuiltInEditorStyleCollection), typeof(DataGrid));

    #endregion

    #region BuiltInDisplayTemplates Property

    /// <summary>
    /// Gets or sets the templates for displaying cell values.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BuiltInDisplayTemplatesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public BuiltInDisplayTemplateCollection BuiltInDisplayTemplates
    {
      get { return (BuiltInDisplayTemplateCollection)GetValue(BuiltInDisplayTemplatesProperty); }
      set { SetValue(BuiltInDisplayTemplatesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BuiltInDisplayTemplates"/> property.
    /// </summary>
    public static readonly DependencyProperty BuiltInDisplayTemplatesProperty =
      DependencyProperty.Register("BuiltInDisplayTemplates", typeof(BuiltInDisplayTemplateCollection), typeof(DataGrid));

    #endregion // BuiltInDisplayTemplates Property

    #region SelectedItem Property

    /// <summary>
    /// Gets or sets the selected item. This property is only used when the data grid is in row selection mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SelectedItem
    {
      get { return GetValue(SelectedItemProperty); }
      set
      {
        SetValue(SelectedItemProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnSelectedItemChanged));

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSelectedItemChanged();
    }

    private void OnSelectedItemChanged()
    {
      _updatingSelectedItemsCollectionInternal = true;
      if (!_isSettingSelectedItemInternal)
      {
        if (SelectionMode == SelectionMode.Single)
        {
          _selectionEventLock = true;
          SelectedItems.Clear();
          _selectionEventLock = false;
          if (SelectedItem != null)
          {
            // If a row is selected in single mode and RowAndCell type, make sure there is no cells selected anymore.
            SelectedCell = null;
            SelectedCells.Clear();

            SelectedItems.Add(SelectedItem);
          }
        }
        else if (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended)
        {
          if (SelectedItem == null)
          {
            if (SelectedItems.Count > 0)
            {
              SelectedItems.Clear();
            }
          }
          else
          {
            if (!SelectedItems.Contains(SelectedItem))
            {
              SelectedItems.Add(SelectedItem);
            }
          }
        }
      }
      _updatingSelectedItemsCollectionInternal = false;

      CollectionView view = ItemsSource as CollectionView;
      if (view != null && IsSynchronizedWithCurrentItem)
      {
        view.MoveCurrentTo(SelectedItem);
      }
    }

    private bool _updatingSelectedItemsCollectionInternal;
    private bool _isSettingSelectedItemInternal;

    #endregion // SelectedItem Property

    #region SkipGroupHeadersWhenNavigating Property

    /// <summary>
    /// Gets or sets whether or not navigating the cells will skip the group header rows. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SkipGroupHeadersWhenNavigatingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool SkipGroupHeadersWhenNavigating
    {
      get { return (bool)GetValue(SkipGroupHeadersWhenNavigatingProperty); }
      set { SetValue(SkipGroupHeadersWhenNavigatingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SkipGroupHeadersWhenNavigating"/> property.
    /// </summary>
    public static readonly DependencyProperty SkipGroupHeadersWhenNavigatingProperty =
      DependencyProperty.Register("SkipGroupHeadersWhenNavigating", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnSkipGroupHeadersWhenNavigatingChanged));

    private static void OnSkipGroupHeadersWhenNavigatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSkipGroupHeadersWhenNavigatingChanged(e);
    }

    private void OnSkipGroupHeadersWhenNavigatingChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // SkipGroupHeadersWhenNavigating Property

    private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _isSettingSelectedItemInternal = true;
      if (!_updatingSelectedItemsCollectionInternal)
      {
        if (e.OldItems != null)
        {
          if (e.Action == NotifyCollectionChangedAction.Replace)
          {
            foreach (object o in e.OldItems)
            {
              if (o == SelectedItem)
              {
                SelectedItem = null;
                break;
              }
            }
          }
          else if (e.Action == NotifyCollectionChangedAction.Remove)
          {
            if (SelectedItems.Count > 0)
            {
              foreach (object o in e.OldItems)
              {
                if (o == SelectedItem)
                {
                  SelectedItem = SelectedItems[0];
                  break;
                }
              }
            }
          }
        }
        if (e.NewItems != null)
        {
          if (SelectionMode == SelectionMode.Single)
          {
            // If a row is selected in single mode and RowAndCell type, make sure there is no cells selected anymore.
            SelectedCell = null;
            SelectedCells.Clear();

            SelectedItem = e.NewItems[0];
            // TODO: using a dispatcher here is not an elegant solution.
            // The issue here is that we want to clear the collection so that it only contains a single item in Single mode.
            // But we can't modify the collection while inside a collection changed event handler.
            // The only other solution may be to use a custom collection implementation for selected items like we do in the Silverlight MultiCalendar.
            Dispatcher.BeginInvoke(new Action(EnsureOnlyOneSelectedItem));
          }
          else if (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended)
          {
            if (SelectedItem == null)
            {
              SelectedItem = e.NewItems[0];
            }
          }
        }
        if (SelectedItems.Count == 0)
        {
          SelectedItem = null;
        }
      }

      SetValue(SelectedDataPropertyKey, SelectedItem);

      // UI update for the selection logic:
      if (_dataGridPanel != null)
      {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
          foreach (UIElement element in _dataGridPanel.Children)
          {
            DataGridRow row = element as DataGridRow;
            if (row != null)
            {
              row.IsSelected = false;
            }
          }
        }
        if (e.OldItems != null)
        {
          foreach (object o in e.OldItems)
          {
            SetRowSelection(o, false);
          }
        }
        if (e.NewItems != null)
        {
          foreach (object o in e.NewItems)
          {
            SetRowSelection(o, true);
          }
        }
      }
      _isSettingSelectedItemInternal = false;

      if (!_selectionEventLock)
      {
        RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.OldItems ?? new List<object>(), e.NewItems ?? new List<object>()));
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionChanged"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SelectionChangedEvent =
      EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble,
        typeof(SelectionChangedEventHandler), typeof(DataGrid));

    /// <summary>
    /// Raised when the selected items changed.
    /// </summary>
    public event SelectionChangedEventHandler SelectionChanged
    {
      add { AddHandler(SelectionChangedEvent, value); }
      remove { RemoveHandler(SelectionChangedEvent, value); }
    }

    private void EnsureOnlyOneSelectedItem()
    {
      int skip = 1;
      while (SelectedItems.Count > 1)
      {
        if (SelectedItems[SelectedItems.Count - skip] != SelectedItem)
        {
          SelectedItems.RemoveAt(SelectedItems.Count - skip);
        }
        else
        {
          skip++;
          if (skip > SelectedItems.Count)
          {
            return;
          }
        }
      }
    }

    private void SetRowSelection(object item, bool isSelected)
    {
      DataGridRow row = _dataGridPanel.GetRow(item);
      if (row != null)
      {
        row.IsSelected = isSelected;
      }
    }

    private IList _selectedItems = new ObservableCollection<object>();

    #region SelectedItems Property

    /// <summary>
    /// Gets or sets the collection of selected items. This property is only used when the data grid is in row selection mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IList SelectedItems
    {
      get
      {
        IList list = (IList)GetValue(SelectedItemsProperty);
        return list ?? _selectedItems;
      }
      set { SetValue(SelectedItemsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItems"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty =
      DependencyProperty.Register("SelectedItems", typeof(IList), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnSelectedItemsChanged));

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSelectedItemsChanged(e);
    }

    private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
    {
      //TODO: now that this can be set, we may need null checks around the place.
      INotifyCollectionChanged notify = _selectedItems as INotifyCollectionChanged;
      if (notify != null)
      {
        notify.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
      }
      _selectedItems = SelectedItems;
      notify = SelectedItems as INotifyCollectionChanged;
      if (notify != null)
      {
        notify.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
      }
      SelectedCells_CollectionChanged(SelectedItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    #endregion // SelectedItems Property



    /*#region SelectedItems Property

    /// <summary>
    /// Gets the SelectedItems.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IList SelectedItems
    {
      get { return (IList)GetValue(SelectedItemsProperty); }
    }

    private static readonly DependencyPropertyKey SelectedItemsPropertyKey =
        DependencyProperty.RegisterReadOnly("SelectedItems", typeof(IList), typeof(DataGrid), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SelectedItems"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

    #endregion // SelectedItems Property*/

    #region SelectedCell Property

    /// <summary>
    /// Gets or sets the selected cell. This property is only used when the data grid is in cell selection mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedCellProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridCell SelectedCell
    {
      get { return (DataGridCell)GetValue(SelectedCellProperty); }
      set
      {
        SetValue(SelectedCellProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedCell"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedCellProperty =
      DependencyProperty.Register("SelectedCell", typeof(DataGridCell), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnSelectedCellChanged));

    private static void OnSelectedCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnSelectedCellChanged();
    }

    private void OnSelectedCellChanged()
    {
      _updatingSelectedCellsCollectionInternal = true;
      if (SelectionMode == SelectionMode.Single)
      {
        SelectedCells.Clear();
        if (SelectedCell != null)
        {
          SelectedCells.Add(SelectedCell);
        }
      }
      else if (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended)
      {
        if (SelectedCell == null)
        {
          if (SelectedCells.Count > 0)
          {
            SelectedCells.Clear();
          }
        }
        else
        {
          // TODO: could implement this "Contains" look up more efficiently by using the cell selection map
          if (!SelectedCells.Contains(SelectedCell))
          {
            SelectedCells.Add(SelectedCell);
          }
        }
      }
      _updatingSelectedCellsCollectionInternal = false;
    }

    #endregion // SelectedCell Property

    #region SelectedData Property

    /// <summary>
    /// Gets the selected data. If a cell is selected, then this property holds the content of that cell.
    /// If a row is selected, then this property holds the content of that row.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedDataProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SelectedData
    {
      get { return GetValue(SelectedDataProperty); }
    }

    private static readonly DependencyPropertyKey SelectedDataPropertyKey =
        DependencyProperty.RegisterReadOnly("SelectedData", typeof(object), typeof(DataGrid), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SelectedData"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedDataProperty =
        SelectedDataPropertyKey.DependencyProperty;

    #endregion // SelectedData Property

    private bool _updatingSelectedCellsCollectionInternal;

    private void SelectedCells_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!_updatingSelectedCellsCollectionInternal)
      {
        if (e.OldItems != null)
        {
          if (e.Action == NotifyCollectionChangedAction.Replace)
          {
            foreach (object o in e.OldItems)
            {
              if (o == SelectedCell)
              {
                SelectedCell = null;
                break;
              }
            }
          }
          else if (e.Action == NotifyCollectionChangedAction.Remove)
          {
            if (SelectedCells.Count > 0)
            {
              foreach (object o in e.OldItems)
              {
                if (o == SelectedCell)
                {
                  SelectedCell = SelectedCells[0];
                  break;
                }
              }
            }
          }
        }
        if (e.NewItems != null)
        {
          if (SelectionMode == SelectionMode.Single)
          {
            // If a cell is selected in single mode and RowAndCell type, then make sure there are no rows selected:
            SelectedItem = null;
            SelectedItems.Clear();

            SelectedCell = e.NewItems[0] as DataGridCell;
          }
          else if (SelectionMode == SelectionMode.Multiple || SelectionMode == SelectionMode.Extended)
          {
            if (SelectedCell == null)
            {
              SelectedCell = e.NewItems[0] as DataGridCell;
            }
          }
        }
        if (_selectedCells.Count == 0)
        {
          SelectedCell = null;
        }
      }

      SetValue(SelectedDataPropertyKey, SelectedCell != null ? SelectedCell.Value : null);

      // Updating the selection UI states and the cell selection map:
      if (e.OldItems != null)
      {
        foreach (DataGridCell cell in e.OldItems)
        {
          IList<DataGridCell> cells;
          _selectedCellMap.TryGetValue(cell.RowContent, out cells);
          if (cells != null)
          {
            cells.Remove(cell);
            if (cells.Count == 0)
            {
              _selectedCellMap.Remove(cell.RowContent);
            }
            SetCellSelection(cell, false);
          }
        }
      }
      if (e.NewItems != null)
      {
        foreach (DataGridCell cell in e.NewItems)
        {
          IList<DataGridCell> cells;
          _selectedCellMap.TryGetValue(cell.RowContent, out cells);
          if (cells == null)
          {
            cells = new List<DataGridCell>();
            cells.Add(cell);
            _selectedCellMap.Add(cell.RowContent, cells);
          }
          else
          {
            cells.Add(cell);
          }
          SetCellSelection(cell, true);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        // Here we deselct all the selected cells in the UI
        // TODO: need to find a smater/faster way of doing this.
        // We could check to see if the number of cells on the screen is smaller than some factor of the number of cells in the selection map.
        //  - if so, iterate through the cells on the screen and deselect them. Any cells not on the screen will be virtualized and so don't matter.
        // Or we could have some kind of cache that holds all cell UI objects on the screen. This would need to be updated every time cells are selected, deselected, or the view port is scrolled.
        foreach (IList<DataGridCell> cells in _selectedCellMap.Values)
        {
          foreach (DataGridCell cell in cells)
          {
            SetCellSelection(cell, false);
          }
        }
        _selectedCellMap.Clear();
      }
    }

    private void SetCellSelection(DataGridCell cell, bool isSelected)
    {
      if (_dataGridPanel != null)
      {
        DataGridRow row = _dataGridPanel.GetRow(cell.RowContent);
        if (row != null)
        {
          DataGridCellContainer cellElement = row.GetCell(cell.Column);
          if (cellElement != null)
          {
            cellElement.IsSelected = isSelected;
          }
        }
      }
    }

    // TODO: it would be neat to merge these 2 collections into some kind of specialized collection implementation:
    // The dictionary is for fast state recovery out of UI virtualization. The observable collection is for a clean API.
    private Dictionary<object, IList<DataGridCell>> _selectedCellMap = new Dictionary<object, IList<DataGridCell>>();

    internal Dictionary<object, IList<DataGridCell>> SelectedCellMap
    {
      get { return _selectedCellMap; }
    }

    private ObservableCollection<DataGridCell> _selectedCells = new ObservableCollection<DataGridCell>();

    /// <summary>
    /// Gets the collection of selected cells. This property is only used when the data grid is in cell selection mode.
    /// </summary>
    public IList<DataGridCell> SelectedCells
    {
      get { return _selectedCells; }
    }

    #region FrozenColumnCount Property

    /// <summary>
    /// Gets or sets the number of frozen columns.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FrozenColumnCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int FrozenColumnCount
    {
      get { return (int)GetValue(FrozenColumnCountProperty); }
      set { SetValue(FrozenColumnCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FrozenColumnCount"/> property.
    /// </summary>
    public static readonly DependencyProperty FrozenColumnCountProperty =
      DependencyProperty.Register("FrozenColumnCount", typeof(int), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnFrozenColumnCountChanged));

    private static void OnFrozenColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnFrozenColumnCountChanged();
    }

    private void OnFrozenColumnCountChanged()
    {
      if (DataGridPanel != null)
      {        
        DataGridPanel.UpdateFirstVisibleColumnIndex();
        DataGridPanel.InvalidateRequiresMeasure();        
      }
      UpdateIsFrozenLineVisible();
      EventHandler handler = FrozenColumnCountChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler FrozenColumnCountChanged;

    #endregion // FrozenColumnCount Property    

    #region FrozenRowCount Property

    /// <summary>
    /// Gets or sets the FrozenRowCount.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FrozenRowCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int FrozenRowCount
    {
      get { return (int)GetValue(FrozenRowCountProperty); }
      set { SetValue(FrozenRowCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FrozenRowCount"/> property.
    /// </summary>
    public static readonly DependencyProperty FrozenRowCountProperty =
      DependencyProperty.Register("FrozenRowCount", typeof(int), typeof(DataGrid),
      new FrameworkPropertyMetadata(OnFrozenRowCountChanged));

    private static void OnFrozenRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnFrozenRowCountChanged();
    }

    private void OnFrozenRowCountChanged()
    {
      if (DataGridPanel != null)
      {
        DataGridPanel.InvalidateRequiresMeasure();
      }
      UpdateIsFrozenLineVisible();
      /*EventHandler handler = FrozenRowCountChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }*/
    }

    //internal event EventHandler FrozenRowCountChanged;

    #endregion // FrozenRowCount Property

    #region FrozenEdge Property

    /// <summary>
    /// Gets the combined width of the frozen columns.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FrozenEdgeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double FrozenEdge
    {
      get { return (double)GetValue(FrozenEdgeProperty); }
      internal set
      {
        if (FrozenEdge != value)
        {
          SetValue(FrozenEdgePropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey FrozenEdgePropertyKey =
        DependencyProperty.RegisterReadOnly("FrozenEdge", typeof(double), typeof(DataGrid), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="FrozenEdge"/> property.
    /// </summary>
    public static readonly DependencyProperty FrozenEdgeProperty =
        FrozenEdgePropertyKey.DependencyProperty;

    #endregion // FrozenEdge Property

    #region FrozenRowEdge Property

    /// <summary>
    /// Gets the FrozenRowEdge.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FrozenRowEdgeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double FrozenRowEdge
    {
      get { return (double)GetValue(FrozenRowEdgeProperty); }
      internal set
      {
        if (FrozenRowEdge != value)
        {
          SetValue(FrozenRowEdgePropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey FrozenRowEdgePropertyKey =
        DependencyProperty.RegisterReadOnly("FrozenRowEdge", typeof(double), typeof(DataGrid), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="FrozenRowEdge"/> property.
    /// </summary>
    public static readonly DependencyProperty FrozenRowEdgeProperty =
        FrozenRowEdgePropertyKey.DependencyProperty;

    #endregion // FrozenRowEdge Property

    #region ShowFirstFrozenLine property

    /// <summary>
    /// Gets or sets whether the frozen line should be displayed when there are no frozen columns.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowFirstFrozenLineProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowFirstFrozenLine
    {
      get { return (bool)GetValue(ShowFirstFrozenLineProperty); }
      set { SetValue(ShowFirstFrozenLineProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowFirstFrozenLine"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowFirstFrozenLineProperty =
      DependencyProperty.Register("ShowFirstFrozenLine", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(false, OnShowFirstFrozenLineChanged));

    private static void OnShowFirstFrozenLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnShowFirstFrozenLineChanged();
    }

    private void OnShowFirstFrozenLineChanged()
    {
      UpdateIsFrozenLineVisible();
    }

    #endregion // ShowFirstFrozenLine property

    #region ShowFrozenLine Property

    /// <summary>
    /// Gets or sets whether or not any frozen line are displayed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowFrozenLineProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowFrozenLine
    {
      get { return (bool)GetValue(ShowFrozenLineProperty); }
      set { SetValue(ShowFrozenLineProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowFrozenLine"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowFrozenLineProperty =
      DependencyProperty.Register("ShowFrozenLine", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnShowFrozenLineChanged));

    private static void OnShowFrozenLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnShowFrozenLineChanged();
    }

    private void OnShowFrozenLineChanged()
    {
      UpdateIsFrozenLineVisible();
    }

    #endregion // ShowFrozenLine Property

    #region IsFrozenLineVisible Property

    /// <summary>
    /// Gets whether the frozen line is currently visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsFrozenLineVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsFrozenLineVisible
    {
      get { return (bool)GetValue(IsFrozenLineVisibleProperty); }
    }

    private static readonly DependencyPropertyKey IsFrozenLineVisiblePropertyKey =
        DependencyProperty.RegisterReadOnly("IsFrozenLineVisible", typeof(bool), typeof(DataGrid), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsFrozenLineVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsFrozenLineVisibleProperty =
        IsFrozenLineVisiblePropertyKey.DependencyProperty;

    #endregion // IsFrozenLineVisible Property

    #region IsFrozenRowLineVisible Property

    /// <summary>
    /// Gets the IsFrozenRowLineVisible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsFrozenRowLineVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsFrozenRowLineVisible
    {
      get { return (bool)GetValue(IsFrozenRowLineVisibleProperty); }
    }

    private static readonly DependencyPropertyKey IsFrozenRowLineVisiblePropertyKey =
        DependencyProperty.RegisterReadOnly("IsFrozenRowLineVisible", typeof(bool), typeof(DataGrid), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsFrozenRowLineVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsFrozenRowLineVisibleProperty =
        IsFrozenRowLineVisiblePropertyKey.DependencyProperty;

    #endregion // IsFrozenRowLineVisible Property

    #region AutoColumnWidthBehavior Property

    /// <summary>
    /// Gets or sets the auto column width behavior.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AutoColumnWidthBehaviorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public AutoColumnWidthBehavior AutoColumnWidthBehavior
    {
      get { return (AutoColumnWidthBehavior)GetValue(AutoColumnWidthBehaviorProperty); }
      set { SetValue(AutoColumnWidthBehaviorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoColumnWidthBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty AutoColumnWidthBehaviorProperty =
      DependencyProperty.Register("AutoColumnWidthBehavior", typeof(AutoColumnWidthBehavior), typeof(DataGrid),
      new FrameworkPropertyMetadata(AutoColumnWidthBehavior.Dynamic, OnAutoColumnWidthBehaviorChanged));

    private static void OnAutoColumnWidthBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnAutoColumnWidthBehaviorChanged();
    }

    private void OnAutoColumnWidthBehaviorChanged()
    {
    }

    #endregion // AutoColumnWidthBehavior Property

    #region CanAutoSizeColumnHeaders Property

    /// <summary>
    /// Gets or sets whether or not column headers are considered when auto sizing a column. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CanAutoSizeColumnHeadersProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CanAutoSizeColumnHeaders
    {
      get { return (bool)GetValue(CanAutoSizeColumnHeadersProperty); }
      set { SetValue(CanAutoSizeColumnHeadersProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanAutoSizeColumnHeaders"/> property.
    /// </summary>
    public static readonly DependencyProperty CanAutoSizeColumnHeadersProperty =
      DependencyProperty.Register("CanAutoSizeColumnHeaders", typeof(bool), typeof(DataGrid),
      new FrameworkPropertyMetadata(true, OnCanAutoSizeColumnHeadersChanged));

    private static void OnCanAutoSizeColumnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGrid)d).OnCanAutoSizeColumnHeadersChanged();
    }

    private void OnCanAutoSizeColumnHeadersChanged()
    {
    }

    #endregion // CanAutoSizeColumnHeaders Property

    private void UpdateIsFrozenLineVisible()
    {
      if ((FrozenColumnCount == 0 && !ShowFirstFrozenLine) || !ShowFrozenLine)
      {
        SetValue(IsFrozenLineVisiblePropertyKey, false);
      }
      else
      {
        SetValue(IsFrozenLineVisiblePropertyKey, true);
      }

      if ((FrozenRowCount == 0 && !ShowFirstFrozenLine) || !ShowFrozenLine)
      {
        SetValue(IsFrozenRowLineVisiblePropertyKey, false);
      }
      else
      {
        SetValue(IsFrozenRowLineVisiblePropertyKey, true);
      }
    }

    private Binding CloneBinding(Binding binding)
    {
      Binding b = new Binding()
      {
        AsyncState = binding.AsyncState,
        BindingGroupName = binding.BindingGroupName,
        BindsDirectlyToSource = binding.BindsDirectlyToSource,
        Converter = binding.Converter,
        ConverterCulture = binding.ConverterCulture,
        ConverterParameter = binding.ConverterParameter,
        ElementName = binding.ElementName,
        FallbackValue = binding.FallbackValue,
        IsAsync = binding.IsAsync,
        Mode = binding.Mode,
        NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
        NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
        NotifyOnValidationError = binding.NotifyOnValidationError,
        Path = binding.Path,
        //RelativeSource = binding.RelativeSource,
        //Source = binding.Source,
        //StringFormat = binding.StringFormat,
        TargetNullValue = binding.TargetNullValue,
        UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
        //UpdateSourceTrigger = binding.UpdateSourceTrigger,
        ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
        ValidatesOnExceptions = binding.ValidatesOnExceptions,
        XPath = binding.XPath
      };
      return b;
    }

    private DataTemplate BuildTemplate(IPropertyInfo info)
    {
      if (info == null)
      {
        return null;
      }

      BindingPropertyInfoAdapter bindingAdapter = info as BindingPropertyInfoAdapter;
      BindingBase binding;// = bindingAdapter == null ? new Binding(info.Name) : new Binding() { Converter = new BindingAdapterConverter(bindingAdapter) };// bindingAdapter.Binding;
      if (bindingAdapter != null)
      {
        MultiBinding b = new MultiBinding();
        b.StringFormat = bindingAdapter.Binding.StringFormat;
        b.Bindings.Add(new Binding());
        b.Bindings.Add(CloneBinding(bindingAdapter.Binding));
        b.Converter = new BindingAdapterConverter(bindingAdapter);
        binding = b;

        if (bindingAdapter.Binding.StringFormat != null)
        {
          FrameworkElementFactory formattedRootFactory = new FrameworkElementFactory(typeof(TextBlock));
          formattedRootFactory.SetValue(TextBlock.TextProperty, binding);
          formattedRootFactory.SetValue(TextBlock.PaddingProperty, new Thickness(3, 4, 0, 4));
          formattedRootFactory.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 2, 0));
          formattedRootFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
          DataTemplate formattedTemplate = new DataTemplate();
          formattedTemplate.VisualTree = formattedRootFactory;
          return formattedTemplate;
        }

      }
      else
      {
        binding = new Binding(info.Name);
      }

      FrameworkElementFactory rootFactory = new FrameworkElementFactory(typeof(ContentPresenter));
      if (info.CanWrite)
      {
        if (bindingAdapter == null)
        {
          binding = new Binding(info.Name) { Mode = BindingMode.TwoWay };
        }
        rootFactory.SetValue(ContentPresenter.ContentProperty, binding);
      }
      else
      {
        rootFactory.SetValue(ContentPresenter.ContentProperty, binding);
      }

      object editorKey = GetEditorKey(info, true);
      foreach (Editor editor in Editors)
      {
        PropertyEditor propertyEditor = editor as PropertyEditor;
        if (propertyEditor != null && propertyEditor.PropertyName.Equals(info.Name))
        {
          if (FindResource(PropertyGrid.CheckBoxEditorKey) == propertyEditor.EditorTemplate)
          {
            editorKey = PropertyGrid.CheckBoxEditorKey;
            break;
          }
        }
        TypeEditor typeEditor = editor as TypeEditor;
        if (typeEditor != null && typeEditor.EditedType.Equals(info.PropertyType))
        {
          if (FindResource(PropertyGrid.IntegerEditorKey) == typeEditor.EditorTemplate)
          {
            editorKey = PropertyGrid.IntegerEditorKey;
            break;
          }
          if (FindResource(PropertyGrid.DoubleEditorKey) == typeEditor.EditorTemplate)
          {
            editorKey = PropertyGrid.DoubleEditorKey;
            break;
          }
        }
      }
      if (BuiltInDisplayTemplates != null)
      {
        rootFactory.SetValue(ContentPresenter.ContentTemplateProperty, BuiltInDisplayTemplates.FindTemplate(editorKey));

        if (PropertyGrid.ListSelectEditorKey.Equals(editorKey) || PropertyGrid.ListSelectNoTextEntryEditorKey.Equals(editorKey))
        {
          CellDisplayConverter converter = new CellDisplayConverter() { TypeConverter = info.Converter };
          rootFactory.SetValue(ContentPresenter.ContentProperty, new Binding(info.Name) { Converter = converter });
        }
      }
      
      // TODO: this was a successful performance improvement. Continue this for all display templates.
      /*rootFactory = new FrameworkElementFactory(typeof(TextBlock));
      rootFactory.SetValue(TextBlock.PaddingProperty, new Thickness(3, 4, 0, 4));
      rootFactory.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 2, 0));
      rootFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
      rootFactory.SetValue(TextBlock.TextProperty, "TEXTEXTEXTEXTEXT");
      //rootFactory.SetBinding(TextBlock.TextProperty, binding);*/

      /*rootFactory = new FrameworkElementFactory(typeof(TextBlock));
      rootFactory.SetValue(TextBlock.PaddingProperty, new Thickness(3, 4, 0, 4));
      rootFactory.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 2, 0));
      rootFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
      rootFactory.SetValue(TextBlock.TextProperty, "TEXTEXTEXTEXTEXT");
      //rootFactory.SetBinding(TextBlock.TextProperty, binding);*/

      DataTemplate template = new DataTemplate();
      template.VisualTree = rootFactory;
      return template;
    }

    // TODO: it would be good to merge this with the BuiltInEditor.GetEditorKey method.
    private static object GetEditorKey(IPropertyInfo info, bool editable)
    {
      Type propertyType = info.PropertyType;
      //object propertyValue = node.Value;

      /*if (propertyValue != null && propertyValue is Many)
      {
        return PropertyGrid.ManyEditorKey;
      }*/

      if (IsCollectionType(propertyType))
      {
        return PropertyGrid.CollectionDisplayKey;
      }

      if (!editable)
      {
        return PropertyGrid.ReadOnlyDisplayKey;
      }

      // If there is an explicit TypeConverter on a property that would normally
      // get a default list-style TypeConverter, we want the explicit TypeConverter
      // to take precedence.
      if (propertyType != null && !info.IsDefined(typeof(TypeConverterAttribute), true))
      {
        if (propertyType.IsEnum || propertyType == typeof(bool))
        {
          return PropertyGrid.ListSelectEditorKey;
        }
      }

      if (info != null)
      {
        TypeConverter converter = info.Converter; //.Property.Converter;
        /*if (converter == null && node.Value != null)
        {
          converter = TypeDescriptor.GetConverter(node.Value);
        }*/
        if (ReflectionUtilities.ShouldUseStandardValues(converter, propertyType))
        {
          return PropertyGrid.ListSelectNoTextEntryEditorKey;
        }
      }

      if (propertyType == typeof(DateTime))
      {
        // At this level, we do not distinguish date/time or time properties.
        // This cannot be done on type and would therefore need the user
        // to provide a PropertyEditor.
        return PropertyGrid.DateEditorKey;
      }

      if (propertyType == typeof(TimeSpan))
      {
        return PropertyGrid.TimeSpanEditorKey;
      }

      if (propertyType == typeof(Color))
      {
        return PropertyGrid.ColorEditorKey;
      }

      if (_textEditableTypes.Contains(propertyType))
      {
        return PropertyGrid.SimpleTextEditorKey;
      }

      return PropertyGrid.ReadOnlyDisplayKey;
    }

    // TODO: this is also seen in BuiltInEditor. May want to pull out into a utils class or make it internal static somewhere.
    private static bool IsCollectionType(Type type)
    {
      return typeof(ICollection).IsAssignableFrom(type)
        || TypeUtilities.IsGenericCollection(type);
    }

    // TODO: this is also seen in BuiltInEditor. May want to pull out into a utils class or make it internal static somewhere.
    private static readonly List<Type> _textEditableTypes = new List<Type>(new Type[]
    {
      typeof(int),
      typeof(uint),
      typeof(long),
      typeof(ulong),
      typeof(short),
      typeof(ushort),
      typeof(byte),
      typeof(sbyte),
      typeof(float),
      typeof(double),
      typeof(decimal),
      typeof(string)
    });

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Style"/> of DataGrid expander toggle button.
    /// </summary>
    public static object ExpanderToggleButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(DataGrid), "ExpanderToggleButtonStyle"); }
    }
  }

  internal class SampleObject
  {
    public string Property1 { get; set; }

    public string Property2 { get; set; }
  }

  /// <summary>
  /// Represents the method that will handle row-related events of a <see cref="DataGrid"/>.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void DataGridRowEventHandler(object sender, DataGridRowEventArgs e);

  /// <summary>
  /// Represents the method that will handle cell-related events of a <see cref="DataGrid"/>.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void DataGridCellEventHandler(object sender, DataGridCellEventArgs e);
}
