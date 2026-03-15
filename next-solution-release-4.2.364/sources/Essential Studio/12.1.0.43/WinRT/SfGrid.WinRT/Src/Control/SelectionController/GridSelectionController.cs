#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
#if WinRT
using System.Globalization;
using Windows.Devices.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Syncfusion.UI.Xaml.Utility;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
#if !WP
using Syncfusion.Windows.Shared;
using System.ComponentModel.DataAnnotations;
#endif

#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
    using DoubleTappedEventArgs = Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs;
    using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
    using TappedEventArgs = Windows.UI.Xaml.Input.TappedRoutedEventArgs;
    using Windows.UI.Xaml.Data;
#elif WPF
    using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif SILVERLIGHT
    using DoubleTappedEventArgs = MouseButtonEventArgs;
    using TappedEventArgs = MouseButtonEventArgs;
#elif WP
    using DoubleTappedEventArgs = System.Windows.Input.GestureEventArgs;
    using TappedEventArgs = System.Windows.Input.GestureEventArgs;
#endif

    public class GridSelectionController : IGridSelectionController, IDisposable
    {
        #region Fields

        bool isSuspended;
        SfDataGrid dataGrid;
        List<int> selectedRows;
        Brush rowHoverBackgroundBrush;
        private Brush rowSelectionBrush;
        private Brush groupRowSelectionBrush;
        GridCurrentCellManager currentCellManager;
        //Behaviour: To hold Selection Changed Event that is raised from SourceCollectionChanged,
        //When deleting Multiple Rows
        protected bool cancelSelectionChangedEvent;

        int pressedIndex = -1;
        Point pressedPosition;
        Key lastPressedKey = Key.None;
        Key currentKey = Key.None;

        #endregion

        #region Properties

        public SfDataGrid DataGrid
        {
            get
            {
                return dataGrid;
            }
        }

        public bool IsSuspended
        {
            get { return isSuspended; }
        }

        protected bool SuspendAutoScrolling { get; set; }

        #endregion

        #region Ctor

        public GridSelectionController(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
            selectedRows = new List<int>();
            currentCellManager = CreateCurrentCellManager();
            InitializeSelectionProperties();
        }

        #endregion

        #region ISelectionController

        /// <summary>
        /// Property which contains the collection of Selected RowIndexes in SfDataGrid.
        /// </summary>
        public List<int> SelectedRows
        {
            get { return selectedRows; }
        }

        /// <summary>
        /// Property which Get or set the Brush value for Selection in DataRow
        /// </summary>
        public Brush RowSelectionBrush
        {
            get
            {
                return rowSelectionBrush;
            }
            set
            {
                rowSelectionBrush = value;
                OnPropertyChanged("RowSelectionBrush");
            }
        }

        /// <summary>
        /// Property which Get or set the Brush value for Selection in Group Header and Summaries.
        /// </summary>
        public Brush GroupRowSelectionBrush
        {
            get
            {
                return groupRowSelectionBrush;
            }
            set
            {
                groupRowSelectionBrush = value;
                OnPropertyChanged("GroupRowSelectionBrush");
            }
        }

        /// <summary>
        /// Property which Get or Set the Brush value for RowHover Highliting.
        /// </summary>
        public Brush RowHoverBackgroundBrush
        {
            get
            {
                return rowHoverBackgroundBrush;
            }
            set
            {
                rowHoverBackgroundBrush = value;
                OnPropertyChanged("RowHoverBackgroundBrush");
            }
        }

        /// <summary>
        /// Property which helps to Get the CurrentCell Manager of SfDataGrid
        /// </summary>
        public GridCurrentCellManager CurrentCellManager
        {
            get { return currentCellManager; }
        }

        /// <summary>
        /// Method which helps to select all the rows.
        /// </summary>
        public void SelectAll()
        {
            if (this.DataGrid.View is VirtualizingCollectionView)
                return;
            this.SuspendUpdates();

            if (this.DataGrid.SelectionMode != GridSelectionMode.None && this.DataGrid.SelectionMode != GridSelectionMode.Single)
            {
                this.ShowAllRowSelectionBorder();
                if (this.DataGrid.SelectedItems.Count > 0)
                {
                    this.DataGrid.SelectedItems.Clear();
                    this.SelectedRows.Clear();
                }
                int rowIndex = this.DataGrid.HeaderLineCount;
                var rowCount = this.DataGrid.VisualContainer.RowCount - this.DataGrid.VisualContainer.FooterRows;

#if !WP
                while (rowIndex < rowCount)
                {
                    if (!this.DataGrid.DetailsViewManager.HasDetailsView)
                        this.SelectedRows.Add(rowIndex);
                    else
                    {
                        if (!this.DataGrid.IsInDetailsViewIndex(rowIndex))
                            this.SelectedRows.Add(rowIndex);
                    }
                    rowIndex++;
                }
#endif

                if (this.DataGrid.GridModel.HasGroup)
                {
                    this.DataGrid.View.TopLevelGroup.DisplayElements.ToList().ForEach(record =>
                        { if (record is RecordEntry) this.DataGrid.SelectedItems.Add((record as RecordEntry).Data); });
                }

                else
                {
                    foreach (RecordEntry record in this.DataGrid.View.Records)
                    {
                        this.DataGrid.SelectedItems.Add(record.Data);
                    }
                }

                var currentIndex = pressedIndex > -1 ? pressedIndex : this.SelectedRows.LastOrDefault();

                if (this.SelectedRows.Count > 0 && this.DataGrid.SelectedItems.Count > 0)
                {
                    this.DataGrid.SelectedItem = pressedIndex > -1 ? this.GetRecordAtRowIndex(pressedIndex) : this.DataGrid.SelectedItems.FirstOrDefault();
                    this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(currentIndex);
                    CurrentCellManager.SetCurrentRowIndex(currentIndex);
                    this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.SelectedIndex);
                }

                var args = new GridSelectionChangedEventArgs(this.DataGrid) { AddedItems = this.DataGrid.SelectedItems.ToList(), AddedIndexs = this.SelectedRows };
                this.DataGrid.RaiseSelectionChangedEvent(args);
            }

            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which helps to clear the selection in Grid.
        /// </summary>
        /// <param name="exceptCurrentRow">Flag which decides whether Current row selection should remove or not</param>
        public void ClearSelections(bool exceptCurrentRow)
        {
            this.ClearSelections(exceptCurrentRow, true);
        }

#if !WP
        /// <summary>
        /// Method whcih helps to move the Current row selection to certain Row Column index.
        /// </summary>
        /// <param name="rowColumnIndex">Corresponding Row and Column index where the selection to move.</param>
        public void MoveCurrentCell(RowColumnIndex rowColumnIndex)
        {
            this.SuspendUpdates();

            if (this.dataGrid.SelectionMode== GridSelectionMode.None || this.dataGrid.NavigationMode== NavigationMode.Row|| !CurrentCellManager.CanMoveCurrentCell(rowColumnIndex))
            {
                this.ResumeUpdates();
                return;
            }
            CurrentCellManager.EndEdit();
            ClearSelections(false);

            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Program))
            {
                this.ResumeUpdates();
                return;
            }            
            this.SelectedRows.Add(rowColumnIndex.RowIndex);
            this.dataGrid.SelectedIndex =this.DataGrid.ResolveToRecordIndex(rowColumnIndex.RowIndex);
            var rowData = this.GetRecordAtRowIndex(rowColumnIndex.RowIndex);
            if (rowData != null)
            {
                this.dataGrid.SelectedItems.Add(rowData);
                this.dataGrid.SelectedItem = rowData;
            }
            this.dataGrid.View.MoveCurrentToPosition(this.dataGrid.SelectedIndex);
            this.CurrentCellManager.SetCurrentRowColumnIndex(rowColumnIndex);
            this.ShowRowSelectionBorder(rowColumnIndex.RowIndex);
            this.ResumeUpdates();
        }
#endif
        /// <summary>
        /// Property which contains the collection of Selected RowIndexes in SfDataGrid.
        /// </summary>
        public virtual void SelectRows(int startRowIndex, int endRowIndex)
        {
            if (startRowIndex < 0 || endRowIndex < 0)
                return;

            if (startRowIndex > endRowIndex)
            {
                var temp = startRowIndex;
                startRowIndex = endRowIndex;
                endRowIndex = temp;
            }

            if (this.DataGrid.SelectionMode != GridSelectionMode.None && this.DataGrid.SelectionMode != GridSelectionMode.Single)
            {
                this.SuspendUpdates();
                var addedItem = new List<object>();
                while (startRowIndex <= endRowIndex)
                {
                    object rowData = this.GetRecordAtRowIndex(startRowIndex);
                    addedItem.Add(rowData);
                    if (!this.SelectedRows.Contains(startRowIndex))
                        this.SelectedRows.Add(startRowIndex);
                    if (!this.DataGrid.SelectedItems.Contains(rowData))
                        this.DataGrid.SelectedItems.Add(rowData);
                    this.ShowRowSelectionBorder(startRowIndex);
                    startRowIndex++;
                }
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(endRowIndex));

                this.ResumeUpdates();
            }
        }

        

        /// <summary>
        /// Method which called when the collections like SelectedItems, Columns and DataSource changed.
        /// </summary>
        /// <param name="e">Collection Changed Event Args</param>
        /// <param name="reason">Which collection has changed</param>
        public virtual void HandleCollectionChanged(NotifyCollectionChangedEventArgs e, CollectionChangedReason reason)
        {
            switch (reason)
            {
                case CollectionChangedReason.SelectedItemsCollection:
                    ProcessSelectedItemsChanged(e);
                    break;
#if !WP
                case CollectionChangedReason.ColumnsCollection:
                    CurrentCellManager.HandleColumnsCollectionChanged(e);
                    break;
#endif
                case CollectionChangedReason.DataReorder:
                    ProcessDataReorder(e.OldItems[0], e.Action);
                    break;

                default:
                    ProcessSourceCollectionChanged(e, reason);
                    break;
            }
        }

#if !WP
        /// <summary>
        /// Method which handles the DetailsViewGrid key down.
        /// </summary>
        /// <param name="args">Corresponding KeyEvent argument</param>
        /// <returns>return whether the key down handled by grid or not.</returns>
        public virtual bool HandleDetailsViewGridKeyDown(KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Home:
                case Key.End:
                case Key.Escape:
                    {
                        this.HandleKeyDown(args);
                        return true;
                    }
                case Key.Enter:
                    {
                        if (pressedIndex < 0)
                            this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                        var lastRowIndex = this.GetLastRowIndex();
                        var nextRowIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex < this.DataGrid.HeaderLineCount
                                               ? this.DataGrid.HeaderLineCount
                                               : this.GetNextRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if ((nextRowIndex == lastRowIndex && nextRowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex) || nextRowIndex == -1 || nextRowIndex > lastRowIndex)
                        {
                            this.ClearSelections(false);
                            pressedIndex = -1;
                            return false;
                        }
                        
                        return this.HandleKeyDown(args); ;
                    }
                case Key.F2:
                    {
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            this.HandleKeyDown(args);
                            return true;
                        }
                        return false;
                    }
                case Key.A:
                case Key.C:
                case Key.V:
                case Key.X:
                case Key.Insert:
                    {
                        if (CheckControlKeyPressed())
                        {
                            this.HandleKeyDown(args);
                            return true;
                        }
                        return false;
                    }
                case Key.Up:
                    {
                        var firstRowIndex = this.GetFirstRowIndex();
                        if (this.CurrentCellManager.CurrentCellIndex.IsEmpty || this.CurrentCellManager.CurrentCellIndex.RowIndex == -1)
                        {
                            this.CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(this.GetLastRowIndex() + 1, this.CurrentCellManager.GetFirstCellIndex()));
                            if (pressedIndex < 0)
                                pressedIndex = this.GetLastRowIndex() + 1;
                        }
                        var previousRowIndex = this.GetPreviousRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if (previousRowIndex == firstRowIndex && previousRowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex)
                        {
                            this.ClearSelections(false);
                            pressedIndex = -1;
                            return false;
                        }
                        this.HandleKeyDown(args);
                        return true;
                    }
                case Key.Down:
                    {
                        if (pressedIndex < 0)
                            this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                        var lastRowIndex = this.GetLastRowIndex();
                        var nextRowIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex < this.DataGrid.HeaderLineCount
                                               ? this.DataGrid.HeaderLineCount
                                               : this.GetNextRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if (((nextRowIndex == lastRowIndex && nextRowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex) || nextRowIndex == -1 || nextRowIndex > lastRowIndex) && !this.DataGrid.IsInDetailsViewIndex(nextRowIndex))
                        {
                            if (this.DataGrid.NotifyListener != null && CheckIsLastRow(this.DataGrid))
                                    return false;
                            this.ClearSelections(false);
                            pressedIndex = -1;
                            return false;
                        }

                        return this.HandleKeyDown(args);
                    }
                case Key.Tab:
                    {
                        if (CheckShiftKeyPressed())
                        {
                            if (this.CurrentCellManager.CurrentCellIndex.ColumnIndex < 0)
                                this.CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.GetLastCellIndex() + 1);
                            var columnIndex = CurrentCellManager.GetPreviousCellIndex();
                            if (columnIndex == CurrentCellManager.GetFirstCellIndex() && this.CurrentCellManager.CurrentCellIndex.ColumnIndex == columnIndex)
                            {
                                var rowIndex = this.GetPreviousRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                if (rowIndex <= GetFirstRowIndex() && rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex)
                                {
                                    this.ClearSelections(false);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (this.CurrentCellManager.CurrentCellIndex.ColumnIndex < 0)
                                this.CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.GetFirstCellIndex());
                            var columnIndex = CurrentCellManager.GetNextCellIndex();
                            if (columnIndex == CurrentCellManager.GetLastCellIndex() && this.CurrentCellManager.CurrentCellIndex.ColumnIndex == columnIndex)
                            {
                                var rowIndex = this.GetNextRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                if (rowIndex >= GetLastRowIndex() && rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex)
                                {
                                    this.ClearSelections(false);
                                    return false;
                                }
                            }
                        }
                        return this.HandleKeyDown(args);
                    }
                default:
                    return false;

            }
        }

        private bool CheckIsLastRow(SfDataGrid dataGrid)
        {
            var parentGrid = dataGrid.NotifyListener.GetParentDataGrid();
            if (parentGrid != null && parentGrid.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex == parentGrid.VisualContainer.RowCount - 1)
            {
                if (parentGrid.NotifyListener != null)
                    return CheckIsLastRow(parentGrid);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method which helps to handle the selection in DataGrid.
        /// </summary>
        /// <param name="args">Corresponding Key Event Args</param>
        /// <returns>return whether the key down handled by grid or not.</returns>
        public virtual bool HandleKeyDown(KeyEventArgs args)
        {
            if (this.DataGrid.SelectionMode == GridSelectionMode.None)
                return args.Handled;
            if (this.dataGrid.NavigationMode == NavigationMode.Cell && !this.DataGrid.IsInDetailsViewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
            {
                if (CurrentCellManager.HandleKeyDown(args))
                {
                    ProcessKeyDown(args);
                    if (args.Key == Key.Up || args.Key == Key.Down || args.Key == Key.Left || args.Key == Key.Right ||
                            args.Key == Key.PageUp || args.Key == Key.PageDown || args.Key == Key.Tab)
                        args.Handled = true;
                    return args.Handled;
                }
            }
            else
            {
                ProcessKeyDown(args);
                return args.Handled;
            }
            return false;
        }
#endif

        /// <summary>
        /// Method which is called when doing Pointer operations like Pressed,Released,Tapped and Double Tapped.
        /// </summary>
        /// <param name="args">args containd the Pointer operation type, Pointer event args</param>
        /// <param name="rowColumnIndex">Row column index of the cell which has been clicked.</param>
        public virtual void HandlePointerOperations(GridPointerEventArgs args, RowColumnIndex rowColumnIndex)
        {
            switch (args.Operation)
            {
                case PointerOperation.Pressed:
                    this.ProcessPointerPressed(args.OriginalEventArgs as MouseButtonEventArgs, rowColumnIndex);
                    break;
                case PointerOperation.Released:
                     this.ProcessPointerReleased(args.OriginalEventArgs as MouseButtonEventArgs, rowColumnIndex);
                    break;
#if !WP
                case PointerOperation.Tapped:
                    this.ProcessOnTapped(args.OriginalEventArgs as TappedEventArgs, rowColumnIndex);
                    break;
                case PointerOperation.DoubleTapped:
                    this.ProcessOnDoubleTapped(rowColumnIndex);
                    break;
#endif
#if !WP && !WinRT
                case PointerOperation.Wheel:
                    this.ProcessPointerWheel(args.OriginalEventArgs as MouseWheelEventArgs, rowColumnIndex);
                    break;
#endif
            }
        }

        /// <summary>
        /// Method which is called while doing Sorting,Filtering,Grouping,Paging and Pasting operations.
        /// </summary>
        /// <param name="args">argument contains the Operation type and Operation arguments</param>
        public virtual void HandleGridOperations(GridOperationsHandle args)
        {
            switch (args.Operation)
            {
                case GridOperation.Sorting:
                    ProcessSortChanged();
                    break;
                case GridOperation.Filtering:
                    ProcessFilterApplied();
                    break;
                    case GridOperation.FilterPopupOpening:
                    ProcessFilterPopupOpened();
                    break;
                case GridOperation.Paging:
                    ProcessPageChanged();
                    break;
                case GridOperation.Grouping:
                    ProcessGroupChanged(args.OperationArgs as NotifyCollectionChangedEventArgs);
                    break;
                case GridOperation.TableSummary:
                    {
                        if (!this.DataGrid.SelectedItems.Any())
                        {
                            this.SuspendUpdates();
                            this.ResetSelectedRows();
                            this.ResumeUpdates();
                            CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(this.DataGrid.HeaderLineCount, CurrentCellManager.GetFirstCellIndex()));
                            this.DataGrid.SelectedIndex = 0;
                        }
                        else
                            ResetSelectedRows();
                        break;
                    }
#if !WP
                case GridOperation.Paste:
                    ProcessOnPaste(args.OperationArgs as List<object>);
                    break;
                case GridOperation.AddNewRow:
                    ProcessAddNewRow((AddNewRowOperationHandle)args.OperationArgs);
                    break;
#endif
            }
        }

        /// <summary>
        /// Method which called when Selection properties like SelectedIndex,SelectedItem and SelectionMode property changed.
        /// </summary>
        /// <param name="handle">Handle which contains the values of changed property</param>
        public virtual void HandleSelectionPropertyChanges(SelectionPropertyChangeHandle handle)
        {
            switch (handle.PropertyName)
            {
                case "SelectedItem":
                    ProcessSelectedItemChanged(handle);
                    break;
                case "SelectedIndex":
                    ProcessSelectedIndexChanged(handle);
                    break;
                case "SelectionMode":
                    ProcessSelectionModeChanged(handle);
                    break;
                case "NavigationMode":
                    ProcessNavigationModeChanged(handle);
                    break;
                case "CurrentItem":
                    ProcessCurrentItemChanged(handle);
                    break;
            }
        }

        /// <summary>
        /// Method which called when the Group Expand or Collapsed
        /// </summary>
        /// <param name="index">Group Row Index</param>
        /// <param name="count">Number of rows collapsed or Expanded</param>
        /// <param name="isExpanded">Whether Expanded or collapsed</param>
        public virtual void HandleGroupExpandCollapse(int index, int count, bool isExpanded)
        {
            if (isExpanded)
                ProcessGroupExpanded(index, count);
            else
                ProcessGroupCollapsed(index, count);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Virtual Method

        #region Pointer Operation Methods

        /// <summary>
        /// Method which handle the selection on PointerPressed operation.
        /// </summary>
        /// <param name="args">Pointer event arguments.</param>
        /// <param name="rowColumnIndex">Cell row and column index.</param>
        protected virtual void ProcessPointerPressed(MouseButtonEventArgs args, RowColumnIndex rowColumnIndex)
        {
#if !WP
            if (this.DataGrid.SelectionMode == GridSelectionMode.None || rowColumnIndex.RowIndex <= this.DataGrid.GetHeaderIndex())
                return;
#endif
#if WPF
            pressedPosition = args == null ? new Point() : GetPointPosition(args, this.DataGrid);
#else
            pressedPosition = args == null ? new Point() : GetPointPosition(args, null);
#endif
#if !WP
            if (rowColumnIndex.ColumnIndex < CurrentCellManager.GetFirstCellIndex() && this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(rowColumnIndex.RowIndex))
                return;
            if (this.DataGrid is DetailsViewDataGrid && this.DataGrid.NotifyListener != null)
            {
                this.ProcessDetailsViewGridPointerPressed(args, rowColumnIndex);
            }

#endif
            if (!CheckShiftKeyPressed())
                this.pressedIndex = rowColumnIndex.RowIndex;

            if (this.DataGrid.AllowSelectionOnPointerPressed)
            {
#if !WP
                if (!CurrentCellManager.HandlePointerOperation(args, rowColumnIndex))
                    return;
                var oldRowIndex = CurrentCellManager.CurrentCellIndex.RowIndex;

                if (this.DataGrid.SelectionMode == GridSelectionMode.Extended && CheckShiftKeyPressed() && this.SelectedRows.Count > 0)
                {
                    this.ProcessShiftSelection(rowColumnIndex.RowIndex);
                    return;
                }
#endif
                this.ProcessSelection(rowColumnIndex.RowIndex, SelectionReason.PointerPressed);
#if !WP
                if (DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                    this.dataGrid.GridModel.addNewRowController.SetAddNewMode(true);

                if (oldRowIndex != CurrentCellManager.CurrentCellIndex.RowIndex && DataGrid.IsAddNewIndex(oldRowIndex))
                    this.DataGrid.GridModel.addNewRowController.CommitAddNew();
#endif
            }
            else
            {
                // ResumeUpdates();
            }
        }

#if !WinRT && !WP
        /// <summary>
        /// Methods which handles Scrolling on Pointer Wheel
        /// </summary>
        /// <param name="mouseButtonEventArgs">An <see cref="T:System.Windows.Input.MouseButtonEventArgs">MouseButtonEventArgs</see> that contains the event data.</param>
        /// <param name="rowColumnIndex"></param>
        /// <remarks></remarks>
        protected virtual void ProcessPointerWheel(MouseWheelEventArgs args, RowColumnIndex rowColumnIndex)
        {
            var currentCell = CurrentCellManager.CurrentCell;
            if (currentCell != null && CurrentCellManager.CurrentCellIndex.RowIndex == rowColumnIndex.RowIndex && (currentCell.GridColumn is GridEditorColumn && ((GridEditorColumn) currentCell.GridColumn).AllowScrollingOnCircle))
                DataGrid.VisualContainer.SuspendManipulationScroll = currentCell.IsEditing;
            else
                DataGrid.VisualContainer.SuspendManipulationScroll = false;
        }
#endif

        /// <summary>
        /// Method which handles the selection in pointer released operation.
        /// </summary>
        /// <param name="args">Corresponding pointer event argument</param>
        /// <param name="rowColumnIndex">Corrensponding cell rowcolumn index.</param>
        protected virtual void ProcessPointerReleased(MouseButtonEventArgs args, RowColumnIndex rowColumnIndex)
        {
            if (IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None || this.DataGrid.AllowSelectionOnPointerPressed || rowColumnIndex.RowIndex <= this.DataGrid.GetHeaderIndex())
            {
                //ResumeUpdates();
                return;
            }

#if WPF
            Point pointerReleasedRowPosition = args == null ? new Point() : GetPointPosition(args, this.DataGrid);
#else
            Point pointerReleasedRowPosition = args == null ? new Point() : GetPointPosition(args, null);
#endif

            double xPosChange = Math.Abs(pointerReleasedRowPosition.X - pressedPosition.X);
            double yPosChange = Math.Abs(pointerReleasedRowPosition.Y - pressedPosition.Y);

            //Here we checking the pointer pressed position and pointer released position. Because we don't selec the row while manipulate scrolling.
            if (xPosChange < 20 && yPosChange < 20)
            {
#if WPF
                if (CheckControlKeyPressed() && (args!=null && args.ChangedButton == MouseButton.Right))
                    return;
#endif

#if !WP

                if (rowColumnIndex.ColumnIndex < CurrentCellManager.GetFirstCellIndex() && this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(rowColumnIndex.RowIndex))
                    return;

                if (CurrentCellManager.CurrentCellIndex != rowColumnIndex)
                {
                    if (this.DataGrid is DetailsViewDataGrid && this.DataGrid.NotifyListener != null)
                    {
                        this.ProcessDetailsViewGridPointerReleased(args, rowColumnIndex);
                    }

                    if (this.DataGrid.SelectedDetailsViewGrid != null)
                    {
                        if (!this.ClearDetailsViewGridSelections(this.DataGrid.SelectedDetailsViewGrid as DetailsViewDataGrid))
                            return;
                        this.DataGrid.SelectedDetailsViewGrid = null;
                    }
                }

                if (!CurrentCellManager.HandlePointerOperation(args, rowColumnIndex))
                    return;

                var oldRowIndex = CurrentCellManager.CurrentCellIndex.RowIndex;

                if (this.DataGrid.SelectionMode == GridSelectionMode.Extended && CheckShiftKeyPressed() && this.SelectedRows.Count > 0)
                {
                    if (ValidationHelper.IsCurrentCellValidated)
                        this.ProcessShiftSelection(rowColumnIndex.RowIndex);
                }
                else
                {
#endif

                    if (ValidationHelper.IsCurrentCellValidated)
                        this.ProcessSelection(rowColumnIndex.RowIndex, SelectionReason.PointerReleased);
#if !WP
                }

                if (DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                    this.dataGrid.GridModel.addNewRowController.SetAddNewMode(true);

                if (oldRowIndex != CurrentCellManager.CurrentCellIndex.RowIndex && DataGrid.IsAddNewIndex(oldRowIndex))
                {
                    this.DataGrid.GridModel.addNewRowController.CommitAddNew();
                }
#endif
            }
        }

#if !WP
        /// <summary>
        /// Method which handles the editing operation when Edit Trigger is set as OnTapped
        /// </summary>
        /// <param name="e">Tapped event args</param>
        /// <param name="currentRowColumnIndex">Corresponding cell row column index.</param>
        protected virtual void ProcessOnTapped(TappedEventArgs e, RowColumnIndex currentRowColumnIndex)
        {
            if (!ValidationHelper.IsCurrentCellValidated || IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None || (this.DataGrid.NavigationMode == NavigationMode.Row && !DataGrid.IsAddNewIndex(currentRowColumnIndex.RowIndex)))
                return;
#if WPF
            Point pointerReleasedRowPosition = e == null ? new Point() : GetPointPosition(e, this.DataGrid);
#elif WinRT
            Point pointerReleasedRowPosition = e == null ? new Point() : e.GetPosition(null);
#else
            Point pointerReleasedRowPosition = e == null ? new Point() : e.GetPosition(null);
#endif

            double xPosChange = Math.Abs(pointerReleasedRowPosition.X - pressedPosition.X);
            double yPosChange = Math.Abs(pointerReleasedRowPosition.Y - pressedPosition.Y);
            //Here we checking the pointer pressed position and pointer released position. Because we don't selec the row while manipulate scrolling.
            if (xPosChange < 20 && yPosChange < 20)
            {
                CurrentCellManager.ProcessOnTapped(e, currentRowColumnIndex);
            }
        }

        /// <summary>
        /// Method which handels the Editing operation when the EditTrigger is set as OnDoubleTapped.
        /// </summary>
        /// <param name="currentRowColumnIndex">Current cell row column index.</param>
        protected virtual void ProcessOnDoubleTapped(RowColumnIndex currentRowColumnIndex)
        {
            if (!ValidationHelper.IsCurrentCellValidated || IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None || this.CurrentCellManager.CurrentCellIndex.RowIndex!=currentRowColumnIndex.RowIndex || (this.DataGrid.NavigationMode == NavigationMode.Row && !this.dataGrid.IsAddNewIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex)))
                return;

            if (this.DataGrid.EditTrigger == EditTrigger.OnDoubleTap)
            {
                if (this.DataGrid.SelectionMode == GridSelectionMode.Multiple)
                    this.AddSelection(CurrentCellManager.CurrentCellIndex.RowIndex, null, SelectionReason.PointerReleased);
                CurrentCellManager.ProcessOnDoubleTapped();
            }
        }
#endif

        #endregion

#if !WP
        #region Key Navigation Methods

        /// <summary>
        /// Method which handles all the Key navigation operations.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void ProcessKeyDown(KeyEventArgs args)
        {
            currentKey = args.Key;
            bool needMove = false;
            if (this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex))
            {
                var dataRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex);
                if (dataRow is DetailsViewDataRow)
                {
                    var detailsViewDataRow = dataRow as DetailsViewDataRow;
                    this.DataGrid.SelectedDetailsViewGrid = detailsViewDataRow.DetailsViewDataGrid;
                    if (detailsViewDataRow.DetailsViewDataGrid.SelectionController.HandleDetailsViewGridKeyDown(args))
                    {
                        if (currentKey == Key.Up)
                            this.ScrollInViewFromTop(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if (currentKey == Key.Down)
                            this.ScrollInViewFromBottom(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        return;
                    }
                    else
                    {
                        if ((currentKey == Key.Tab && !CheckShiftKeyPressed()) || currentKey == Key.Down || currentKey == Key.Enter)
                        {
                            //var nextRowIndex = GetNextRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            if (this.CurrentCellManager.CurrentCellIndex.RowIndex >= GetLastRowIndex())
                            {
                                //this.ScrollInViewFromBottom(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                return;
                            }
                        }

                        if (currentKey == Key.Tab)
                        {
                            var colIndex = CheckShiftKeyPressed() ? (dataGrid.FlowDirection == FlowDirection.LeftToRight ? CurrentCellManager.GetFirstCellIndex() : CurrentCellManager.GetLastCellIndex()) : (dataGrid.FlowDirection == FlowDirection.LeftToRight ? CurrentCellManager.GetLastCellIndex() : CurrentCellManager.GetFirstCellIndex());
                            this.CurrentCellManager.SetCurrentColumnIndex(colIndex);
                        }
                    }
                }
            }

            if (DataGrid.FlowDirection == FlowDirection.RightToLeft)
            {
                switch (args.Key)
                {
                    case Key.Right:
                        currentKey = Key.Left;
                        break;
                    case Key.Left:
                        currentKey = Key.Right;
                        break;
                    case Key.Home:
                        {
                            if (!CheckControlKeyPressed())
                                currentKey = Key.End;
                        }
                        break;
                    case Key.End:
                        {
                            if (!CheckControlKeyPressed())
                                currentKey = Key.Home;
                        }
                        break;
                }
            }

            ValidationHelper.IsFocusSetBack = false;
            int rowIndex, columnIndex;
            switch (currentKey)
            {
                case Key.Escape:
                    {
                        if (CurrentCellManager.HandleKeyNavigation(args))
                            args.Handled = true;
                    }
                    break;
                case Key.F2:
                    {
                        if (CurrentCellManager.HandleKeyNavigation(args))
                            args.Handled = true;
                    }
                    break;
                case Key.C:
                    {
                        if (CheckControlKeyPressed())
                        {
                            this.DataGrid.GridCopyPaste.Copy();
                            args.Handled = true;
                        }
                    }
                    break;
                case Key.V:
                    {
                        if (CheckControlKeyPressed())
                        {
                            this.DataGrid.GridCopyPaste.Paste();
                            args.Handled = true;
                        }
                    }
                    break;
                case Key.X:
                    {
                        if (CheckControlKeyPressed())
                        {
                            this.DataGrid.GridCopyPaste.Cut();
                            args.Handled = true;
                        }
                    }
                    break;
                case Key.Insert:
                    {
                        if (CheckShiftKeyPressed())
                        {
                            this.DataGrid.GridCopyPaste.Paste();
                            args.Handled = true;
                        }
                    }
                    break;
                case Key.Down:
                    {
                        int lastRowIndex = this.GetLastRowIndex();
                        rowIndex = CheckControlKeyPressed() ? lastRowIndex : this.GetNextRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            if (!CurrentCellManager.CheckValidationAndEndEdit(true))
                                return;
                        }
                        int actualRowIndex = rowIndex;

                        if (dataGrid.AddNewRowPosition == AddNewRowPosition.Top && dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(false);
                            if (dataGrid.View.IsAddingNew)
                            {
                                dataGrid.GridModel.addNewRowController.CommitAddNew();
                            }
                            if (this.CurrentCellManager.CurrentCell != null && this.DataGrid.SelectionMode == GridSelectionMode.Multiple)
                            {
                                this.CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                                RemoveSelection(this.CurrentCellManager.CurrentCellIndex.RowIndex, new List<object>(), SelectionReason.KeyPressed);
                            }
                        }

                        if (this.CurrentCellManager.CurrentCellIndex.ColumnIndex == -1)
                            this.CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.GetFirstCellIndex());

#if !WPF
                        if (this.DataGrid is DetailsViewDataGrid && this.DataGrid.NotifyListener != null)
                        {
                            if (rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == lastRowIndex)
                            {
                                var parentGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
#if WinRT
                                parentGrid.Focus(FocusState.Programmatic);
#else
                                parentGrid.Focus();
#endif
                                parentGrid.SelectionController.HandleKeyDown(args);

                                return;
                            }
                        }
#endif
                        this.ProcessDetailsViewKeyDown(args, rowIndex, Key.Down);

                        if ((rowIndex > lastRowIndex || (rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == lastRowIndex)) && (!this.dataGrid.IsAddNewIndex(rowIndex) || (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom && this.dataGrid.IsAddNewIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex))))
                            return;

                        if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended && this.CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex)
                            actualRowIndex++;

                        var rowColumnIndex = new RowColumnIndex(actualRowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }

                        if (this.DataGrid.SelectionMode != GridSelectionMode.None)
                        {
                            this.HideRowFocusBorder();
                            if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                            {
                                if (CheckShiftKeyPressed() && CheckControlKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                                {
                                    this.ProcessShiftSelection(rowIndex);
                                }
                                else
                                {
                                    this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                }
                            }
                            else
                            {
                                this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                                this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                needMove = true;
                            }
                        }

                        if (dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex) && dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom && !dataGrid.View.IsAddingNew)
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(true);
                        }

                        this.ScrollInViewFromBottom(rowIndex);


                        if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            this.lastPressedKey = Key.Down;
                        else
                        {
                            this.lastPressedKey = Key.None;
                            pressedIndex = rowIndex;
                        }
                        args.Handled = true;
                    }
                    break;
                case Key.Up:
                    {
                        rowIndex = (CheckControlKeyPressed()) ? this.GetFirstRowIndex() : this.GetPreviousRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            if (!CurrentCellManager.CheckValidationAndEndEdit(true))
                                return;
                        }

                        if (this.CurrentCellManager.CurrentCellIndex.RowIndex < this.DataGrid.HeaderLineCount)
                            return;
                        int actualRowIndex = rowIndex;

                        if (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom && dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(false);
                            if (dataGrid.View.IsAddingNew)
                            {
                                dataGrid.GridModel.addNewRowController.CommitAddNew();
                            }
                        }

                        this.ProcessDetailsViewKeyDown(args, rowIndex, Key.Up);

#if !WPF
                        if (this.DataGrid is DetailsViewDataGrid && rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == this.DataGrid.HeaderLineCount)
                        {
                            var parentGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
#if WinRT
                            parentGrid.Focus(FocusState.Programmatic);
#else
                            parentGrid.Focus();
#endif
                            parentGrid.SelectionController.HandleKeyDown(args);

                            return;
                        }
#endif

                        if (rowIndex == this.DataGrid.HeaderLineCount && rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex)
                            return;

                        if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended &&
                            this.CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex)
                            actualRowIndex--;

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            var rowColumnIndex = new RowColumnIndex(actualRowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex);
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }
                        this.HideRowFocusBorder();
                        if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                        {
                            if (CheckShiftKeyPressed() && CheckControlKeyPressed() &&
                               this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            {
                                this.ProcessShiftSelection(rowIndex);
                            }
                            else
                            {
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                            }
                        }
                        else
                        {
                            this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                            this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            needMove = true;
                        }

                        if (dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex) && dataGrid.AddNewRowPosition == AddNewRowPosition.Top && !dataGrid.View.IsAddingNew)
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(true);
                        }

                        this.ScrollInViewFromTop(this.CurrentCellManager.CurrentCellIndex.RowIndex);

                        if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            this.lastPressedKey = Key.Up;
                        else
                        {
                            this.lastPressedKey = Key.None;
                            pressedIndex = rowIndex;
                        }
                        args.Handled = true;
                    }
                    break;
                case Key.Right:
                    {
                        if (this.CurrentCellManager.CurrentCellIndex.RowIndex <= this.dataGrid.GetHeaderIndex())
                            return;

                        Group group = null;
                        if (this.DataGrid.GridModel.HasGroup)
                        {
                            group = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex)] as Group;
                        }
                        if (group != null)
                        {
                            this.ExpandOrCollapseGroup(group, true);
                        }
                        else if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (!CurrentCellManager.HandleKeyNavigation(args))
                                return;
                        }
                        if (this.SelectedRows.Count > 1 && this.DataGrid.SelectionMode == GridSelectionMode.Extended && !CheckShiftKeyPressed())
                        {
                            var currentRowIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                            this.ClearSelections(true);
                            if (group != null)
                            {
                                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(currentRowIndex);
                                this.CurrentCellManager.SetCurrentRowIndex(this.DataGrid.ResolveToRowIndex(this.DataGrid.SelectedIndex));
                                this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                                this.ShowRowSelectionBorder(currentRowIndex);
                            }
                        }

                        this.lastPressedKey = Key.Right;
                        args.Handled = true;
                    }
                    break;

                case Key.Left:
                    {
                        if (this.CurrentCellManager.CurrentCellIndex.RowIndex <= this.dataGrid.GetHeaderIndex())
                            return;
                        Group group = null;

                        if (this.DataGrid.GridModel.HasGroup)
                        {
                            group = this.DataGrid.View.TopLevelGroup.DisplayElements[
                                this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex)] as Group;
                        }
                        if (group != null)
                        {
                            this.ExpandOrCollapseGroup(group, false);
                        }
                        else if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (!CurrentCellManager.HandleKeyNavigation(args))
                                return;
                        }

                        if (this.SelectedRows.Count > 1 && this.DataGrid.SelectionMode == GridSelectionMode.Extended && !CheckShiftKeyPressed())
                        {
                            var currentRowIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                            this.ClearSelections(true);
                            if (group != null)
                            {
                                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(currentRowIndex);
                                this.CurrentCellManager.SetCurrentRowIndex(this.DataGrid.ResolveToRowIndex(this.DataGrid.SelectedIndex));
                                this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                                this.ShowRowSelectionBorder(currentRowIndex);
                            }
                        }
                        this.lastPressedKey = Key.Left;
                        args.Handled = true;
                    }
                    break;

                case Key.PageDown:
                    {
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            if (!CurrentCellManager.CheckValidationAndEndEdit(true))
                                return;
                        }

                        if (dataGrid.AddNewRowPosition == AddNewRowPosition.Top && dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(false);
                            if (dataGrid.View.IsAddingNew)
                                dataGrid.GridModel.addNewRowController.CommitAddNew();
                        }

                        var lastRowIndex = this.GetLastRowIndex();
                        rowIndex = this.GetNextPageIndex();

                        if ((rowIndex > lastRowIndex || (rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == lastRowIndex)) && !this.dataGrid.IsAddNewIndex(rowIndex))
                            return;

                        var rowColumnIndex = new RowColumnIndex(rowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }
                        this.HideRowFocusBorder();
                        if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                        {
                            if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            {
                                this.ProcessShiftSelection(rowIndex);
                            }
                            else
                            {
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                pressedIndex = rowIndex;
                            }
                        }
                        else
                        {
                            this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                            this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            needMove = true;
                        }

                        this.ScrollInViewFromBottom(rowIndex);
                        this.lastPressedKey = Key.PageDown;
                        args.Handled = true;
                    }
                    break;
                case Key.PageUp:
                    {
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            if (!CurrentCellManager.CheckValidationAndEndEdit(true))
                                return;
                        }

                        if (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom && dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            dataGrid.GridModel.addNewRowController.SetAddNewMode(false);
                            if (dataGrid.View.IsAddingNew)
                                dataGrid.GridModel.addNewRowController.CommitAddNew();
                        }

                        rowIndex = this.GetPreviousPageIndex();

                        if (rowIndex == this.DataGrid.HeaderLineCount && rowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex)
                            return;

                        var rowColumnIndex = new RowColumnIndex(rowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex);

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }
                        this.HideRowFocusBorder();
                        if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                        {
                            if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            {
                                this.ProcessShiftSelection(rowIndex);
                            }
                            else
                            {
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                pressedIndex = rowIndex;
                            }
                        }
                        else
                        {
                            this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                            this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            needMove = true;
                        }

                        this.ScrollInViewFromTop(rowIndex);
                        this.lastPressedKey = Key.PageUp;
                        args.Handled = true;
                    }
                    break;
                case Key.Home:
                    {
                        if (this.CurrentCellManager.CurrentCellIndex.RowIndex <= this.dataGrid.GetHeaderIndex())
                            return;
                        rowIndex = CheckControlKeyPressed() ? this.GetFirstRowIndex() : this.CurrentCellManager.CurrentCellIndex.RowIndex;

                        if (this.DataGrid.NavigationMode == NavigationMode.Row)
                            rowIndex = this.GetFirstRowIndex();

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            var validationResult = rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex ? CurrentCellManager.RaiseValidationAndEndEdit() : CurrentCellManager.CheckValidationAndEndEdit(true);
                            if (!validationResult)
                                return;
                        }


                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                            columnIndex = CurrentCellManager.GetFirstCellIndex();
                        else
                            columnIndex = this.CurrentCellManager.CurrentCellIndex.ColumnIndex;

                        if (this.CurrentCellManager.CurrentCellIndex.ColumnIndex == CurrentCellManager.GetFirstCellIndex() && this.CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex)
                            return;

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (dataGrid.FlowDirection == FlowDirection.RightToLeft && CheckControlKeyPressed())
                                columnIndex = CurrentCellManager.GetLastCellIndex();
                            var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }
                        this.HideRowFocusBorder();
                        if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                        {
                            if (CheckShiftKeyPressed() && CheckControlKeyPressed() &&
                                this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            {
                                this.ProcessShiftSelection(rowIndex);
                            }
                            else
                            {
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                pressedIndex = rowIndex;
                            }
                        }
                        else
                        {
                            this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                            this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            needMove = true;
                        }

                        this.ScrollInViewFromTop(rowIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                            this.CurrentCellManager.ScrollInViewFromLeft(columnIndex);
                        this.lastPressedKey = Key.Home;
                        args.Handled = true;
                    }
                    break;
                case Key.End:
                    {
                        rowIndex = CheckControlKeyPressed() ? this.GetLastRowIndex() : CurrentCellManager.CurrentCellIndex.RowIndex;
                        if (this.DataGrid.NavigationMode == NavigationMode.Row)
                            rowIndex = this.GetLastRowIndex();
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            var validationResult = rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex ? CurrentCellManager.RaiseValidationAndEndEdit() : CurrentCellManager.CheckValidationAndEndEdit(true);
                            if (!validationResult)
                                return;
                        }

                        if (this.CurrentCellManager.CurrentCellIndex.RowIndex <= this.dataGrid.GetHeaderIndex())
                            return;
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                            columnIndex = CurrentCellManager.GetLastCellIndex();
                        else
                            columnIndex = CurrentCellManager.CurrentCellIndex.ColumnIndex;

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                        {
                            if (dataGrid.FlowDirection == FlowDirection.RightToLeft && CheckControlKeyPressed())
                                columnIndex = CurrentCellManager.GetFirstCellIndex();
                            var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }
                        this.HideRowFocusBorder();
                        if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                        {
                            if (CheckShiftKeyPressed() && CheckControlKeyPressed() &&
                                this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                                this.ProcessShiftSelection(rowIndex);
                            else
                            {
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                pressedIndex = rowIndex;
                            }
                        }
                        else
                        {
                            this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                            this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                            needMove = true;
                        }

                        this.ScrollInViewFromBottom(rowIndex);
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                            this.CurrentCellManager.ScrollInViewFromRight(columnIndex);

                        this.lastPressedKey = Key.End;
                        args.Handled = true;
                    }
                    break;
#if !WP
                case Key.Delete:
                    {
                        args.Handled = RemoveRows();
                    }
                    break;
#endif
                case Key.Enter:
                    {
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell || DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
#if WinRT
                            if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                CurrentCellManager.CommitCellValue(true);
#endif
                            if (!CurrentCellManager.CheckValidationAndEndEdit(false))
                                return;
                        }
                        int lastRowIndex = this.GetLastRowIndex();
                        rowIndex = this.GetNextRowIndex(CurrentCellManager.CurrentCellIndex.RowIndex);

                        if (DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            DataGrid.GridModel.addNewRowController.CommitAddNew();
                            if (DataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                                return;
                        }

                        if (CurrentCellManager.CurrentCellIndex.ColumnIndex == -1)
                            CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.GetFirstCellIndex());

#if !WPF
                        if (this.DataGrid is DetailsViewDataGrid && this.DataGrid.NotifyListener != null)
                        {
                            if (rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == lastRowIndex)
                            {
                                var parentGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
#if WinRT
                                parentGrid.Focus(FocusState.Programmatic);
#else
                                parentGrid.Focus();
#endif
                                parentGrid.SelectionController.HandleKeyDown(args);
                                return;
                            }
                        }
#endif

                        this.ProcessDetailsViewKeyDown(args, rowIndex, Key.Down);

                        if (rowIndex >= lastRowIndex && CurrentCellManager.CurrentCellIndex.RowIndex == lastRowIndex)
                            return;

                        if (this.DataGrid.NavigationMode == NavigationMode.Cell || dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            var rowColumnIndex = new RowColumnIndex(rowIndex, CurrentCellManager.CurrentCellIndex.ColumnIndex);
                            if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                return;
                        }

                        if (this.DataGrid.SelectionMode != GridSelectionMode.None)
                        {
                            this.HideRowFocusBorder();
                            if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                                this.ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                            else
                            {
                                this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                                this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                needMove = true;
                            }
                        }

                        this.ScrollInViewFromBottom(rowIndex);

                        if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                            this.lastPressedKey = Key.Down;
                        else
                        {
                            this.lastPressedKey = Key.None;
                            pressedIndex = rowIndex;
                        }
                        args.Handled = true;
                    }
                    break;

                case Key.A:
                    {
                        if (CheckControlKeyPressed())
                        {
                            this.SelectAll();
                            this.lastPressedKey = Key.A;
                            args.Handled = true;
                        }
                    }
                    break;

                case Key.Tab:
                    {
                        if (CurrentCellManager.CurrentCellIndex.RowIndex <= this.DataGrid.GetHeaderIndex() && !(this.DataGrid is DetailsViewDataGrid))
                            return;
                        this.lastPressedKey = Key.Tab;
                        if (this.DataGrid.NavigationMode == NavigationMode.Cell || DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            if (!CurrentCellManager.HandleKeyNavigation(args))
                            {
                                if (args.Handled)
                                    return;
                                rowIndex = CurrentCellManager.CurrentCellIndex.RowIndex;
                                if (CheckShiftKeyPressed())
                                {
                                    columnIndex = this.dataGrid.FlowDirection == FlowDirection.LeftToRight ? CurrentCellManager.GetLastCellIndex() : CurrentCellManager.GetFirstCellIndex();
                                    if (CurrentCellManager.CurrentCellIndex.RowIndex < 0)
                                        rowIndex = this.GetLastRowIndex();
                                    else
                                        rowIndex = this.GetPreviousRowIndex(rowIndex);

                                    if (rowIndex <= GetFirstRowIndex() && CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex && !(this.DataGrid is DetailsViewDataGrid))
                                        return;

                                    var dataRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowIndex && item.RowType != RowType.DefaultRow);
                                    if (dataRow != null)
                                    {
                                        columnIndex = this.dataGrid.FlowDirection == FlowDirection.LeftToRight ? CurrentCellManager.GetFirstCellIndex() : CurrentCellManager.GetLastCellIndex();
                                    }
#if WinRT
                                    if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                                        CurrentCellManager.CommitCellValue(true);
#endif
                                    if (!this.DataGrid.Validations.RaiseRowValidate(CurrentCellManager.CurrentCellIndex))
                                        return;
                                    if (DataGrid.View.IsEditingItem)
                                        DataGrid.View.CommitEdit();
                                    if (DataGrid.AddNewRowPosition == AddNewRowPosition.Bottom && dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                                        dataGrid.GridModel.addNewRowController.CommitAddNew();

                                    this.ProcessDetailsViewKeyDown(args, rowIndex, Key.Tab);

#if !WPF
                                    if (this.DataGrid is DetailsViewDataGrid && rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == this.DataGrid.HeaderLineCount)
                                    {
                                        var parentGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
                                        parentGrid.SelectionController.HandleKeyDown(args);
#if WinRT
                                        parentGrid.Focus(FocusState.Programmatic);
#else
                                        parentGrid.Focus();
#endif
                                        return;
                                    }
#endif

                                }
                                else
                                {
                                    if (CurrentCellManager.CurrentCellIndex.RowIndex < 0)
                                        rowIndex = this.GetFirstRowIndex();
                                    else
                                        rowIndex = this.GetNextRowIndex(rowIndex);
                                    columnIndex = dataGrid.FlowDirection == FlowDirection.LeftToRight ? CurrentCellManager.GetFirstCellIndex() : CurrentCellManager.GetLastCellIndex();
                                    if (!this.DataGrid.Validations.RaiseRowValidate(CurrentCellManager.CurrentCellIndex))
                                        return;
                                    if (DataGrid.View.IsEditingItem)
                                        DataGrid.View.CommitEdit();
                                    if (dataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                                    {
                                        dataGrid.GridModel.addNewRowController.CommitAddNew();
                                        if (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                                            rowIndex = dataGrid.GridModel.addNewRowController.GetAddNewRowIndex();
                                    }
                                    if (!this.DataGrid.IsAddNewIndex(rowIndex) && rowIndex >= GetLastRowIndex() && CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex && !(this.DataGrid is DetailsViewDataGrid))
                                        return;

#if !WPF
                                    if (this.DataGrid is DetailsViewDataGrid && rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex && rowIndex == GetLastRowIndex())
                                    {
                                        var parentGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
#if WinRT
                                        parentGrid.Focus(FocusState.Programmatic);
#else
                                        parentGrid.Focus();
#endif
                                        parentGrid.SelectionController.HandleKeyDown(args);
                                        return;
                                    }
#endif

                                    this.ProcessDetailsViewKeyDown(args, rowIndex, Key.Tab);

                                }

                                var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);

                                if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                                {
                                    if (!CurrentCellManager.ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                                        return;
                                }

                                this.HideRowFocusBorder();
                                if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                                {
                                    ProcessSelection(rowIndex, SelectionReason.KeyPressed);
                                    pressedIndex = rowIndex;
                                }
                                else
                                {
                                    this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                                    this.ShowRowFocusBorder(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                    needMove = true;
                                }

                                if (dataGrid.IsAddNewIndex(rowIndex))
                                    dataGrid.GridModel.addNewRowController.SetAddNewMode(true);

                                if (CheckShiftKeyPressed())
                                {
                                    if (columnIndex == CurrentCellManager.GetLastCellIndex())
                                        this.CurrentCellManager.ScrollInViewFromRight(columnIndex);
                                    else
                                        this.CurrentCellManager.ScrollInViewFromLeft(columnIndex);
                                    this.ScrollInViewFromTop(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                }
                                else
                                {
                                    if (columnIndex == CurrentCellManager.GetFirstCellIndex())
                                        CurrentCellManager.ScrollInViewFromLeft(columnIndex);
                                    else
                                        CurrentCellManager.ScrollInViewFromRight(columnIndex);
                                    this.ScrollInViewFromBottom(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                                }
                            }
                            this.lastPressedKey = Key.None;
                            args.Handled = true;
                        }
                        break;


                    }
                case Key.Space:
                    {
                        if (this.DataGrid.SelectionMode == GridSelectionMode.Multiple && this.CurrentCellManager.CurrentCellIndex.RowIndex > this.DataGrid.GetHeaderIndex())
                        {
                            var addedItems = new List<object>();
                            var removedItems = new List<object>();
                            var addedIndexes = new List<int>();
                            var removedIndexes = new List<int>();
                            var currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsCurrentRow);
                            if(currentRow!=null)
                            {
                                if (currentRow.IsSelectedRow)
                                {
                                    removedItems.Add(currentRow.RowData);
                                    removedIndexes.Add(currentRow.RowIndex);
                                    if (!RaiseSelectionChanging(addedItems, removedItems, addedIndexes, removedIndexes))
                                    {
                                        if (this.DataGrid.NavigationMode == NavigationMode.Row && DataGrid.IsAddNewIndex(currentRow.RowIndex))
                                            CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                                        RemoveSelection(currentRow.RowIndex, removedItems, SelectionReason.KeyPressed);
                                        ShowRowFocusBorder(CurrentCellManager.CurrentCellIndex.RowIndex);
                                    }
                                    else
                                        return;
                                }
                                else
                                {
                                    if (!(currentRow.RowData is Group) && !(currentRow.RowData is SummaryRecordEntry))
                                        addedItems.Add(currentRow.RowData);
                                    addedIndexes.Add(currentRow.RowIndex);
                                    if (!RaiseSelectionChanging(addedItems, removedItems, addedIndexes, removedIndexes))
                                    {
                                        if (this.DataGrid.NavigationMode == NavigationMode.Row && DataGrid.IsAddNewIndex(currentRow.RowIndex))
                                            CurrentCellManager.SelectCurrentCell(new RowColumnIndex(currentRow.RowIndex,CurrentCellManager.GetFirstCellIndex()));
                                        AddSelection(currentRow.RowIndex, addedItems, SelectionReason.KeyPressed);
                                        HideRowFocusBorder();
                                    }
                                    else
                                        return;
                                }
                                RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
                                args.Handled = true;
                            }
                        }
                        break;
                    }

            }
            if (needMove)
            {
                this.SuspendUpdates();
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
                UpdateCurrentRow();
                this.ResumeUpdates();
            }
        }

        #endregion
#endif

        #region Grid Operations Methods

        /// <summary>
        /// Method which handles the selection when Page Changed.
        /// </summary>
        protected virtual void ProcessPageChanged()
        {
            this.ResetSelectedRows();
            this.RefreshSelectedItems();
            UpdateCurrentRowIndex();
        }

        /// <summary>
        /// Method which handles the selection while filter applied.
        /// </summary>
        protected virtual void ProcessFilterApplied()
        {
#if !WP
            if (this.DataGrid is DetailsViewDataGrid && this.SelectedRows.Count == 0)
                return;
            if (!dataGrid.View.IsAddingNew)
                this.DataGrid.GridModel.addNewRowController.SetAddNewMode(false);
#endif
            this.SuspendUpdates();
            ResetSelectedRows();
            RefreshSelectedItems();
            this.ResumeUpdates();
            this.CurrentCellManager.isCollectionChanged = true;
            if (this.SelectedRows.Count == 0 || this.DataGrid.CurrentItem == null)
            {
                this.CurrentCellManager.SetCurrentColumnIndex(this.CurrentCellManager.GetFirstCellIndex());
                this.DataGrid.SelectedIndex = 0;
            }

            UpdateCurrentRowIndex();
            this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
        }

        protected virtual void ProcessFilterPopupOpened()
        {
            int rowIndex = CurrentCellManager.CurrentCellIndex.RowIndex;
#if !WP
            if (this.dataGrid.NavigationMode == NavigationMode.Cell || dataGrid.IsAddNewIndex(rowIndex))
            {
#if WinRT
                if (CurrentCellManager.CurrentCell != null && CurrentCellManager.CurrentCell.IsEditing)
                            CurrentCellManager.CommitCellValue(true);
#endif
                if (!this.currentCellManager.CheckValidationAndEndEdit(false))
                    return;
            }
            if (this.dataGrid.View.IsAddingNew && dataGrid.IsAddNewIndex(rowIndex))
            {
                dataGrid.GridModel.addNewRowController.CommitAddNew(false);
            }
            else
                return;

            if (this.dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
            {
                rowIndex = this.GetLastRowIndex();
                rowIndex += 1;
            }
#endif
            if (this.DataGrid.SelectionMode != GridSelectionMode.None)
            {
                if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple)
                    this.ProcessSelection(rowIndex, SelectionReason.PointerPressed);
            }
            if (this.dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                this.dataGrid.ScrollInView(new RowColumnIndex(rowIndex, CurrentCellManager.CurrentCellIndex.ColumnIndex));

        }

        /// <summary>
        /// Method which handles the selection when Sort the columns.
        /// </summary>
        protected virtual void ProcessSortChanged()
        {
            this.SuspendUpdates();
#if !WP
            if (this.DataGrid.GridModel.HasGroup || this.DataGrid.View is Syncfusion.Data.PagedCollectionView)
#else
            if(this.DataGrid.GridModel.HasGroup)
#endif
            {
                this.ResetSelectedRows();
                this.RefreshSelectedItems();
            }
            else
            {
                this.ResetSelectedRows();
            }
            UpdateCurrentRowIndex();
            this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
            if (this.DataGrid.SelectedItem != null && this.DataGrid.View != null && this.DataGrid.CurrentItem == null)
            {
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
            }
            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which handles the selection when the column grouped.
        /// </summary>
        /// <param name="args">Group column collection changed event argument.</param>
        protected virtual void ProcessGroupChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.DataGrid.SelectionMode != GridSelectionMode.None)
            {
                this.SuspendUpdates();
                this.ResetSelectedRows();
                this.RefreshSelectedItems();
                this.ResumeUpdates();
                if (this.SelectedRows.Count == 0 && this.DataGrid.CurrentItem==null)
                {
                    CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(this.DataGrid.HeaderLineCount, CurrentCellManager.GetFirstCellIndex()));
                    this.DataGrid.SelectedIndex = 0;
                }
                else
                {
                    if (args == null)
                        return;

                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.CurrentCellIndex.ColumnIndex + args.NewItems.Count);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            CurrentCellManager.SetCurrentColumnIndex(CurrentCellManager.CurrentCellIndex.ColumnIndex - args.OldItems.Count);
                            break;
                    }
                }
                UpdateCurrentRowIndex();
            }
        }

#if !WP
        /// <summary>
        /// Method which handles the selection while pasting the records in DataGrid.
        /// </summary>
        /// <param name="records"></param>
        protected virtual void ProcessOnPaste(List<object> records)
        {
            if (this.DataGrid.SelectionMode == GridSelectionMode.None)
                return;
            if (this.DataGrid.SelectionMode != GridSelectionMode.Single)
            {
                this.DataGrid.SelectedItems.Clear();
                records.ForEach(rec =>
                {
                    if (this.DataGrid.View.FilterRecord(rec))
                        this.DataGrid.SelectedItems.Add(rec);
                });
            }
            else
            {
                if (this.DataGrid.View.FilterRecord(records.FirstOrDefault()))
                    this.DataGrid.SelectedItem = records.FirstOrDefault();
            }
        }
#endif

        #endregion

        #region Handle Selection PropertyChanges

        /// <summary>
        /// Method which handles the selection when Selection Mode property changed
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessSelectionModeChanged(SelectionPropertyChangeHandle handle)
        {
            if (handle.NewValue == null)
                return;

            this.SuspendUpdates();
#if !WP
            if (this.CurrentCellManager.HasCurrentCell && this.CurrentCellManager.CurrentCell.IsEditing)
                CurrentCellManager.EndEdit();
#endif

            HideRowFocusBorder();

            switch ((GridSelectionMode)handle.NewValue)
            {
                case GridSelectionMode.None:
                    this.ClearSelections(false);
                    CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                    UpdateCurrentRow();
                    if (this.DataGrid.CurrentItem != null)
                        this.DataGrid.View.MoveCurrentToPosition(-1);
                    break;
                case GridSelectionMode.Single:
                    this.ClearSelections(true);
                    break;
                case GridSelectionMode.Extended:
                    {
                        var row = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsCurrentRow);
                        if (row != null && !row.IsSelectedRow)
                        {
                            row.IsSelectedRow = true;
                            this.SelectedRows.Add(row.RowIndex);
                            if (row.RowData != null && !(row.RowData is Group) && !(row.RowData is SummaryRecordEntry))
                                this.DataGrid.SelectedItems.Add(row.RowData);
                        }
                    }
                    break;
            }
            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which handles the selection when NavigationMode changed.
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessNavigationModeChanged(SelectionPropertyChangeHandle handle)
        {
            this.ClearSelections(false);
            this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
            this.UpdateCurrentRow();
        }

        /// <summary>
        /// Method which helps to do the Selection operation when CurrentItem changed.
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessCurrentItemChanged(SelectionPropertyChangeHandle handle)
        {
            if (IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None)
                return;
            bool needMove = false;
            
            var addedItems = new List<object>();
            var removedItems = new List<object>();
            var addedIndexes = new List<int>();
            var removedIndexes = new List<int>();
            var oldCurrentCellColumnIndex = CurrentCellManager.CurrentCellIndex.ColumnIndex > -1 ? CurrentCellManager.CurrentCellIndex.ColumnIndex : CurrentCellManager.GetFirstCellIndex();
            if(this.dataGrid.SelectionMode== GridSelectionMode.Single)
            {
                removedIndexes.Add(this.SelectedRows.FirstOrDefault());
                removedItems.Add(this.DataGrid.SelectedItems.FirstOrDefault());
                this.CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                RemoveSelection(CurrentCellManager.CurrentCellIndex.RowIndex, removedItems, SelectionReason.CollectionChanged);
                
                var rowIndex = this.DataGrid.ResolveToRowIndex(handle.NewValue);
                if (rowIndex > this.DataGrid.GetHeaderIndex())
                {
                    addedIndexes.Add(rowIndex);
                    addedItems.Add(handle.NewValue);
                    this.CurrentCellManager.SelectCurrentCell(new RowColumnIndex(rowIndex, oldCurrentCellColumnIndex));
                    AddSelection(rowIndex, addedItems, SelectionReason.CollectionChanged);
                    this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                    needMove = true;
                }
                this.RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
            }
            else
            {
                var rowIndex = this.DataGrid.ResolveToRowIndex(handle.NewValue);
#if !WP
                if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                {
                    this.CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                    if (rowIndex > this.DataGrid.GetHeaderIndex())
                    {
                        this.CurrentCellManager.SelectCurrentCell(new RowColumnIndex(rowIndex, oldCurrentCellColumnIndex));
                        this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                        needMove = true;
                    }
                }
                else
#endif
                {
                    if (rowIndex > this.DataGrid.GetHeaderIndex())
                    {
                        HideRowFocusBorder();
                        var row = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowIndex);
                        if (row != null)
                        {
                            row.IsFocusedRow = true;
                        }
                        this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                        needMove = true;
                    }
                }
            }

            if (this.dataGrid.CurrentItem!=null && this.dataGrid.CurrentItem.Equals(this.dataGrid.View.CurrentItem))
                needMove = false;
            if (needMove)
            {
                this.SuspendUpdates();
                this.DataGrid.View.MoveCurrentTo(handle.NewValue);
                this.ResumeUpdates();
            }
            UpdateCurrentRow();
        }

        /// <summary>
        /// Method which handles the selection when Selection Index property changed
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessSelectedIndexChanged(SelectionPropertyChangeHandle handle)
        {
            var newValue = (int)handle.NewValue;
            var oldValue = (int)handle.OldValue;

            if (IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None || !ValidationHelper.IsCurrentCellValidated)
                return;
            bool needToMove = true;
            this.SuspendUpdates();
            int rowIndex = this.dataGrid.ResolveToRowIndex(newValue);
            if (!this.SelectedRows.Contains(rowIndex))
            {
                object rowData = this.GetRecordAtRowIndex(rowIndex);
                var addedItems = new List<object>();
                var removedItems = new List<object>();
                var addedIndexes = new List<int>();
                var removedIndexes = new List<int>();

                if (this.SelectedRows.Count > 1)
                {
                    removedIndexes = this.SelectedRows.ToList();
                    removedItems = this.DataGrid.SelectedItems.ToList();
                    ClearSelections(false);
                }
                else
                {
                    var removeIndex = this.SelectedRows.FirstOrDefault();
                    this.SelectedRows.Remove(removeIndex);
                    var removedData = this.DataGrid.SelectedItems.LastOrDefault();
                    this.DataGrid.SelectedItems.Remove(removedData);
                    this.HideRowSelectionBorder(removeIndex);
                    removedItems.Add(removedData);
                    removedIndexes.Add(removeIndex);
                }

                if (rowData != null)
                    this.DataGrid.SelectedItems.Add(rowData);
                else
                    needToMove = false;
                HideRowFocusBorder();
                this.SelectedRows.Add(rowIndex);
                this.ShowRowSelectionBorder(rowIndex);

                CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                this.CurrentCellManager.SetCurrentRowIndex(SelectedRows.LastOrDefault());
#if !WP
                if(DataGrid.NavigationMode==NavigationMode.Cell)
#endif
                CurrentCellManager.SelectCurrentCell(this.CurrentCellManager.CurrentCellIndex);
                UpdateCurrentRow();
                this.DataGrid.SelectedItem = rowData;

                if (this.DataGrid.GridModel.HasGroup)
                {
                    var record =
                        this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(rowIndex)];
                    if (!(record is RecordEntry))
                        needToMove = false;
                }

                if (needToMove)
                    this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(rowIndex));
                else
                {
                    this.DataGrid.View.MoveCurrentToPosition(-1);
                }

                if (!cancelSelectionChangedEvent)
                {
                    addedIndexes.Add(rowIndex);
                    addedItems.Add(rowData);
                    this.RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
                }
            }

            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which handles the selection when Selection Item property changed
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessSelectedItemChanged(SelectionPropertyChangeHandle handle)
        {
            if (IsSuspended || (handle.NewValue == null && handle.OldValue == null) || this.DataGrid.SelectionMode == GridSelectionMode.None || !ValidationHelper.IsCurrentCellValidated)
                return;
            this.SuspendUpdates();

            var addedItems = new List<object>();
            var removedItems = new List<object>();
            var addedIndexes = new List<int>();
            var removedIndexes = new List<int>();

            if (handle.NewValue == null)
            {
                removedItems = this.DataGrid.SelectedItems.ToList();
                removedIndexes = this.SelectedRows.ToList();
                this.ClearSelections(false);
                this.RaiseSelectionChanged(null, removedItems, null, removedIndexes);
                this.ResumeUpdates();
                return;
            }

            if (!this.DataGrid.SelectedItems.Contains(handle.NewValue))
            {
                int rowIndex = this.DataGrid.ResolveToRowIndex(handle.NewValue);
                HideRowFocusBorder();
                if (this.SelectedRows.Count > 1)
                {
                    removedIndexes = this.SelectedRows.ToList();
                    removedItems = this.DataGrid.SelectedItems.ToList();
                    ClearSelections(false);
                }
                else if (this.SelectedRows.Count > 0)
                {
                    var removedData = this.DataGrid.SelectedItems.LastOrDefault();
                    if (removedData != null)
                    {
                        this.DataGrid.SelectedItems.Remove(removedData);
                        removedItems.Add(removedData);
                    }
                    var removeIndex = this.SelectedRows.FirstOrDefault();
                    this.SelectedRows.Remove(removeIndex);
                    this.HideRowSelectionBorder(removeIndex);
                    CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                    removedIndexes.Add(removeIndex);
                }

                this.DataGrid.SelectedItems.Add(handle.NewValue);
                if (rowIndex >= this.dataGrid.HeaderLineCount)
                {
                    this.SelectedRows.Add(rowIndex);
                    this.ShowRowSelectionBorder(rowIndex);
                    addedIndexes.Add(rowIndex);
                }
                this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
#if !WP
                if (this.DataGrid.NavigationMode == NavigationMode.Cell)
                    CurrentCellManager.SelectCurrentCell(this.CurrentCellManager.CurrentCellIndex);
#endif
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));

                addedItems.Add(handle.NewValue);
                UpdateCurrentRow();
                this.RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
            }

            this.ResumeUpdates();
        }

        #endregion

        #region Collection Changed Operation

        /// <summary>
        /// Method which handles the selection when DataGrid ItemsSource collection changed.
        /// </summary>
        /// <param name="e">Collection changed event argument.</param>
        /// <param name="reason">Collection changed reason.</param>
        protected virtual void ProcessSourceCollectionChanged(NotifyCollectionChangedEventArgs e, CollectionChangedReason reason)
        {
            this.SuspendUpdates();
            var removedItems = new List<object>();
            var removedIndexes = new List<int>();
#if !WP
            if (reason == CollectionChangedReason.SourceCollectionChanged)
            {
                if (DataGrid.View.IsAddingNew)
                    DataGrid.GridModel.addNewRowController.CommitAddNew();
            }
#endif
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (reason == CollectionChangedReason.RecordCollectionChanged)
                        {
                            if (e.NewItems.Contains(this.dataGrid.SelectedItem))
                            {
                                this.CurrentCellManager.isCollectionChanged = true;
                                this.ResetSelectedRows();
                                if (dataGrid.GridModel.HasGroup || dataGrid.View.SortDescriptions.Count > 0 || this.HasFilter())
                                {
                                    this.RefreshSelectedItems();
                                }
                                UpdateCurrentRowIndex();
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (reason == CollectionChangedReason.RecordCollectionChanged)
                        {
                            if (e.OldItems.Contains(this.dataGrid.SelectedItem))
                            {
                                this.CurrentCellManager.isCollectionChanged = true;
                                this.ResetSelectedRows();
                                UpdateCurrentRowIndex();
                            }
                        }
                        else
                        {

                            object removedData = e.OldItems[0] is RecordEntry ? (e.OldItems[0] as RecordEntry).Data : e.OldItems[0];
                            if (this.DataGrid.SelectedItems.Contains(removedData))
                            {
                                int removedIndex = this.DataGrid.ResolveToRowIndex(removedData);
                                removedItems.Add(removedData);
                                removedIndexes.Add(removedIndex);
                                CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                                this.RemoveSelection(removedIndex, removedItems, SelectionReason.CollectionChanged);
                                if (!cancelSelectionChangedEvent)
                                    this.RaiseSelectionChanged(null, removedItems, null, removedIndexes);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        if (reason == CollectionChangedReason.SourceCollectionChanged)
                        {
                            if (DataGrid.SelectedItems.Contains(e.OldItems[0]))
                            {
                                var index = DataGrid.SelectedItems.IndexOf(e.OldItems[0]);
                                DataGrid.SelectedItems[index] = e.NewItems[0];
                                if (DataGrid.SelectedItem == e.OldItems[0])
                                    DataGrid.SelectedItem = e.NewItems[0];
                            }
                          
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    {
                        if (reason == CollectionChangedReason.SourceCollectionChanged)
                            this.RefreshSelectedItems();
                    }
                    break;
            }
            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which handles the selection when SelectedItems collection changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessSelectedItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsSuspended || this.DataGrid.SelectionMode == GridSelectionMode.None)
                return;
            this.SuspendUpdates();
            var addedItems = new List<object>();
            var removedItems = new List<object>();
            var addedIndexes = new List<int>();
            var removedIndexes = new List<int>();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (this.DataGrid.SelectionMode == GridSelectionMode.Single && this.DataGrid.SelectedItems.Count > 1)
                            throw new InvalidOperationException("Cannot able to Add more than one item in SelectedItems collection when SelectionMode is 'Single'");

                        int rowIndex = this.DataGrid.ResolveToRowIndex(e.NewItems[0]);
                        addedItems.Add(e.NewItems[0]);
                        addedIndexes.Add(rowIndex);
                        this.AddSelection(rowIndex, addedItems, SelectionReason.SelectedItemsChanged);
                        this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(rowIndex));
                        this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                        this.RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        int rowIndex = this.DataGrid.ResolveToRowIndex(e.OldItems[0]);
                        removedIndexes.Add(rowIndex);
                        removedItems.Add(e.OldItems[0]);
                        this.RemoveSelection(rowIndex, removedItems, SelectionReason.SelectedItemsChanged);
                        this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
                        this.RaiseSelectionChanged(null, removedItems, null, removedIndexes);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.SelectedRows.Clear();
                        HideAllRowSelectionBorder(false);
                        this.DataGrid.SelectedIndex = -1;
                        this.CurrentCellManager.SetCurrentRowIndex(-1);
                        this.DataGrid.SelectedItem = null;
                        this.CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                        this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
                        this.RaiseSelectionChanged(null, null, null, null);
                    }
                    break;
            }
            HideRowFocusBorder();
            UpdateCurrentRow();
            this.ResumeUpdates();
        }

        protected virtual void ProcessDataReorder(object value, NotifyCollectionChangedAction action)
        {
            this.SuspendUpdates();

            if (action == NotifyCollectionChangedAction.Remove)
            {
                if (this.DataGrid.SelectedItems.Contains(value))
                {
                    var removedIndex = DataGrid.ResolveToRowIndex(value);
                    this.DataGrid.SelectedItems.Remove(value);
                    if (this.DataGrid.SelectedItem == value)
                    {
                        this.DataGrid.SelectedItem = this.DataGrid.SelectedItems.FirstOrDefault();
                    }
                    RaiseSelectionChanged(null, new List<object>() { value }, null, new List<int>() { removedIndex });
                }
            }

            this.ResumeUpdates();
        }

        protected virtual void ClearSelections(bool exceptCurrentRow, bool removeCurrentCellSelection = true)
        {
            this.SuspendUpdates();
            if (this.SelectedRows.Count > 0 && !exceptCurrentRow)
            {
                this.SelectedRows.Clear();
                if (removeCurrentCellSelection)
                    CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                this.HideAllRowSelectionBorder(exceptCurrentRow);
                this.DataGrid.SelectedItems.Clear();
                this.DataGrid.View.MoveCurrentToPosition(-1);
                this.DataGrid.SelectedItem = null;
                this.DataGrid.SelectedIndex = -1;
                
                CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(-1, -1));
                this.CurrentCellManager.previousCellIndex = new RowColumnIndex(-1, -1);
            }
            else if (this.SelectedRows.Count > 0)
            {
                object currentData = this.DataGrid.View.CurrentItem != null ? ((RecordEntry)this.DataGrid.View.CurrentItem).Data : null;

                this.SelectedRows.Clear();
                this.DataGrid.SelectedItems.Clear();
                this.HideAllRowSelectionBorder(exceptCurrentRow);

                int currentRowIndex = this.DataGrid.ResolveToRowIndex(this.DataGrid.View.CurrentPosition);
                this.SelectedRows.Add(currentRowIndex);
                if (currentData != null)
                {
                    this.DataGrid.SelectedItems.Add(currentData);
                }
                this.DataGrid.SelectedItem = currentData;
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(currentRowIndex);
                this.CurrentCellManager.SetCurrentRowIndex(this.DataGrid.ResolveToRowIndex(this.DataGrid.SelectedIndex));
                this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                this.ShowRowSelectionBorder(currentRowIndex);
            }
            else
            {
                if (CurrentCellManager.CurrentCell != null && removeCurrentCellSelection)
                {
                    CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                    this.DataGrid.SelectedIndex = -1;
                    CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(-1, -1));
                }
#if !WP
                else if (DataGrid.NavigationMode == NavigationMode.Row)
                    CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(-1, -1));
#endif
            }
            HideRowFocusBorder();
            this.ResumeUpdates();
        }

        #endregion

        #region Group Expand/Collapse

        /// <summary>
        /// Method which handles the selection when the Group expanded.
        /// </summary>
        /// <param name="insertIndex">Expanded group row header index.</param>
        /// <param name="count">Expanded row count.</param>
        protected virtual void ProcessGroupExpanded(int insertIndex, int count)
        {
            if (this.DataGrid.SelectionMode == GridSelectionMode.Multiple)
            {
                int index = this.SelectedRows.Count - 1;
                while (index >= 0)
                {
                    if (SelectedRows[index] > insertIndex - 1)
                    {
                        SelectedRows[index] = SelectedRows[index] + count;
                    }
                    index--;
                }
            }
        }

        /// <summary>
        /// Method which handles the selection when the Group collapsed.
        /// </summary>
        /// <param name="removeAtIndex">Expanded group row header index.</param>
        /// <param name="count">Collapsedrow count.</param>
        protected virtual void ProcessGroupCollapsed(int removeAtIndex, int count)
        {
            if (this.DataGrid.SelectionMode == GridSelectionMode.Multiple)
            {
                this.SuspendUpdates();
                var group = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(removeAtIndex - 1)] as Group;
                if (group != null && group.Records != null)
                {
                    int index = group.Records.Count - 1;
                    while (index >= 0)
                    {
                        var recordIndex = this.DataGrid.View.TopLevelGroup.DisplayElements.IndexOf(group.Records[index]);
                        this.SelectedRows.Remove(this.DataGrid.ResolveToRowIndex(recordIndex));
                        this.DataGrid.SelectedItems.Remove(group.Records[index].Data);
                        index--;
                    }
                }

                int inx = this.SelectedRows.Count - 1;
                while (inx >= 0)
                {
                    if (SelectedRows[inx] > removeAtIndex - 1)
                    {
                        SelectedRows[inx] = SelectedRows[inx] - count;
                    }
                    inx--;
                }

                this.DataGrid.SelectedItem = this.DataGrid.SelectedItems.FirstOrDefault();
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.SelectedRows.LastOrDefault());
                this.ResumeUpdates();
            }
        }

        #endregion

        #region CurrentCell Manager

        /// <summary>
        /// Method which creates the current cell manager for SfDataGrid.
        /// </summary>
        /// <returns></returns>
        protected virtual GridCurrentCellManager CreateCurrentCellManager()
        {
            return new GridCurrentCellManager(this.dataGrid);
        }

        #endregion

        #region Add New Row

#if !WP
        /// <summary>
        /// Method which handles the selection when the operation done related to AddNewRow.
        /// </summary>
        /// <param name="handle"></param>
        protected virtual void ProcessAddNewRow(AddNewRowOperationHandle handle)
        {
            switch (handle.AddNewRowOperation)
            {
                case AddNewRowOperation.CommitNew:
                    {
                        if (DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex))
                        {
                            this.CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                            RemoveSelection(CurrentCellManager.CurrentCellIndex.RowIndex, new List<object>() { dataGrid.View.CurrentAddItem }, SelectionReason.KeyPressed);
                        }
                    }
                    break;
                case AddNewRowOperation.PlacementChange:
                    {
                        var addNewRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
                        if (addNewRow != null && addNewRow.IsCurrentRow)
                        {
                            this.CurrentCellManager.RemoveCurrentCellSelection(CurrentCellManager.CurrentCellIndex);
                            RemoveSelection(CurrentCellManager.CurrentCellIndex.RowIndex, new List<object>() { dataGrid.View.CurrentAddItem }, SelectionReason.KeyPressed);
                            if (addNewRow.IsSelectedRow)
                            {
                                addNewRow.IsSelectedRow = false;
                                addNewRow.IsCurrentRow = false;
                            }
                            (addNewRow as GridDataRow).ApplyRowHeaderVisualState();
                        }
                        this.DataGrid.GridModel.addNewRowController.SetAddNewMode(false);
                        ResetSelectedRows();
                        UpdateCurrentRowIndex();
                    }
                    break;
            }
        }
#endif
        #endregion

        #endregion

        #region Private Methods

        #region General Helper Methods

        /// <summary>
        /// Method which initiate the Selection Properties in the selection controller.
        /// </summary>
        private void InitializeSelectionProperties()
        {
            GroupRowSelectionBrush = dataGrid.GroupRowSelectionBrush;
            RowSelectionBrush = dataGrid.RowSelectionBrush;
            RowHoverBackgroundBrush = dataGrid.RowHoverHighlightingBrush;
        }

#if !WP
        /// <summary>
        /// Method to remove the Selected Rows from Source Collection in the DataGrid.
        /// </summary>
        /// <remarks></remarks>
        protected bool RemoveRows()
        {
            var sfDataGrid = DataGrid.SelectedDetailsViewGrid ?? DataGrid;
            var currentCell = sfDataGrid.SelectionController.CurrentCellManager.CurrentCell;
            bool canUserDeleteRows;
            if (sfDataGrid.NavigationMode == NavigationMode.Cell)
                canUserDeleteRows = (sfDataGrid.AllowDeleting) && (currentCell != null && !currentCell.IsEditing);
            else
            {
                var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == sfDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex);
                canUserDeleteRows = (sfDataGrid.AllowDeleting) && (dataRow!=null && dataRow.RowType == RowType.DefaultRow);
            }

            if (!canUserDeleteRows)
                return false;

            var recordsToRemove = sfDataGrid.SelectedItems.ToList();
            var indexesOfItemsToRemove = sfDataGrid.SelectionController.SelectedRows.ToList();
            var currentCellIndex = sfDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex;

            var deletingEventArgs = new RecordDeletingEventArgs(sfDataGrid) { Items = sfDataGrid.SelectedItems.ToList() };
            if (sfDataGrid.RaiseRecordDeletingEvent(deletingEventArgs))
                return false;

            var itemsToRemove = deletingEventArgs.Items;
            cancelSelectionChangedEvent = true;

            foreach (var item in itemsToRemove.Where(item => sfDataGrid.View.Contains(item)))
            {
                sfDataGrid.View.Remove(item);
            }

            var deletedEventArgs = new RecordDeletedEventArgs(sfDataGrid) 
            {
                Items = itemsToRemove,
                SelectedIndex = currentCellIndex.RowIndex
            };
            sfDataGrid.RaiseRecordDeletedEvent(deletedEventArgs);

            if (itemsToRemove.Count != recordsToRemove.Count)
            {
                if (itemsToRemove.Count == 0)
                    return false;
                var rowIndex = sfDataGrid.SelectionController.SelectedRows.FirstOrDefault();
                currentCellIndex = new RowColumnIndex(rowIndex, currentCell.ColumnIndex);
            }
            else
                sfDataGrid.SelectionController.ClearSelections(false);
            
            currentCellIndex.RowIndex = deletedEventArgs.SelectedIndex;
            sfDataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(currentCellIndex.RowIndex);
            ((GridSelectionController)sfDataGrid.SelectionController).pressedIndex = currentCellIndex.RowIndex;
            sfDataGrid.SelectionController.CurrentCellManager.SetCurrentRowColumnIndex(currentCellIndex);
            cancelSelectionChangedEvent = false;

            var e = new GridSelectionChangedEventArgs(sfDataGrid)
            {
                AddedItems = sfDataGrid.SelectedItems,
                RemovedItems = itemsToRemove,
                AddedIndexs = sfDataGrid.SelectionController.SelectedRows,
                RemovedIndexs = indexesOfItemsToRemove
            };
            sfDataGrid.RaiseSelectionChangedEvent(e);
            sfDataGrid.ScrollInView(currentCellIndex);
            return true;
        }
#endif

        /// <summary>
        /// Method which Expand or Collapse the group when press Right and Left key.
        /// </summary>
        /// <param name="group">Corresponding group</param>
        /// <param name="isExpanded"></param>
        private void ExpandOrCollapseGroup(Group group, bool isExpanded)
        {
            var captionRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == CurrentCellManager.CurrentCellIndex.RowIndex && (item.RowType == RowType.CaptionCoveredRow || item.RowType == RowType.CaptionRow));
            if (captionRow != null)
            {
                var captionRowControl = captionRow.WholeRowElement as CaptionSummaryRowControl;
                if (isExpanded)
                {
                    if (!group.IsExpanded)
                    {
                        this.DataGrid.GridModel.ExpandGroup(group);
                        captionRowControl.IsExpanded = true;
                    }
                }
                else
                {
                    if (group.IsExpanded)
                    {
                        this.DataGrid.GridModel.CollapseGroup(group);
                        captionRowControl.IsExpanded = false;
                    }
                }
                this.DataGrid.UpdateRowCountAndScrollBars();
                this.DataGrid.GridModel.RefreshDataRow(CurrentCellManager.CurrentCellIndex.RowIndex, true);
            }
        }

        /// <summary>
        /// Method which perform the selection when we select the rows using Shift key.
        /// </summary>
        /// <param name="rowIndex"></param>
        private void ProcessShiftSelection(int rowIndex)
        {
            if (CurrentCellManager.CurrentCellIndex.RowIndex == rowIndex)
                return;
            object rowData = null;
            object pressedRowData = this.GetRecordAtRowIndex(this.pressedIndex);

            var addedItems = new List<object>();
            var removedItems = this.DataGrid.SelectedItems.ToList();
            var addedIndexes = new List<int>();
            var removedIndexes = this.SelectedRows.ToList();
            var columnIndex = CurrentCellManager.CurrentCellIndex.ColumnIndex;
            int currentIndex = this.pressedIndex;
            if (this.pressedIndex < rowIndex)
            {
                while (currentIndex <= rowIndex)
                {
                    rowData = this.GetRecordAtRowIndex(currentIndex);
                    addedItems.Add(rowData);
                    addedIndexes.Add(currentIndex);
                    currentIndex++;
                }
            }
            else
            {
                while (currentIndex >= rowIndex)
                {
                    rowData = this.GetRecordAtRowIndex(currentIndex);
                    addedItems.Add(rowData);
                    addedIndexes.Add(currentIndex);
                    currentIndex--;
                }
            }

            var commonItems = addedItems.Intersect(removedItems).ToList();
            var commonIndexes = addedIndexes.Intersect(removedIndexes).ToList();
            var addedItem = addedItems.Except(commonItems).ToList();
            var removedItem = removedItems.Except(commonItems).ToList();
            var addedIndex = addedIndexes.Except(commonIndexes).ToList();
            var removedIndex = removedIndexes.Except(commonIndexes).ToList();

            if (this.RaiseSelectionChanging(addedItem, removedItem, addedIndex, removedIndex))
                return;

            this.ClearSelections(false,false);

            this.SuspendUpdates();

            currentIndex = 0;
            while (currentIndex < addedItems.Count)
            {
                var data = addedItems[currentIndex];
                var index = addedIndexes[currentIndex];
                if (!this.SelectedRows.Contains(index))
                {
#if !WP
                    if (this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(index))
                    {
                        var dataRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == index);
                        if (dataRow is DetailsViewDataRow)
                        {
                            var detailsViewDataRow = dataRow as DetailsViewDataRow;
                            this.DataGrid.SelectedDetailsViewGrid = detailsViewDataRow.DetailsViewDataGrid;
                            detailsViewDataRow.DetailsViewDataGrid.SelectionController.SelectAll();
                        }
                    }
#endif
                    this.SelectedRows.Add(index);
                }
                if (!this.DataGrid.SelectedItems.Contains(data) && data != null)
                    this.DataGrid.SelectedItems.Add(data);
                this.ShowRowSelectionBorder(index);
                currentIndex++;
            }

            this.DataGrid.SelectedItem = pressedRowData;
            this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(pressedIndex);
            CurrentCellManager.SetCurrentRowColumnIndex(new RowColumnIndex(rowIndex,columnIndex));
            this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(rowIndex));
            UpdateCurrentRow();
            RaiseSelectionChanged(addedItem, removedItem, addedIndex, removedIndex);
            this.ResumeUpdates();
        }

        /// <summary>
        /// Check whether the Shift key is pressed
        /// </summary>
        /// <returns></returns>
        private bool CheckShiftKeyPressed()
        {
#if WinRT
            if (Window.Current.CoreWindow.GetAsyncKeyState(Key.Shift).HasFlag(CoreVirtualKeyStates.Down))
#elif WPF
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
#else
            if (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
#endif
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method which decides whether we can remove the selection in same row.
        /// </summary>
        /// <returns></returns>
        private bool CheckCanRemoveSameRow()
        {
#if !WP
            if (this.DataGrid.SelectionMode == GridSelectionMode.Extended)
            {
#if WinRT
                if ((Window.Current.CoreWindow.GetAsyncKeyState(Key.Shift).HasFlag(CoreVirtualKeyStates.Down)) && (Window.Current.CoreWindow.GetAsyncKeyState(Key.Home).HasFlag(CoreVirtualKeyStates.Down) || Window.Current.CoreWindow.GetAsyncKeyState(Key.End).HasFlag(CoreVirtualKeyStates.Down)))
#elif WPF
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.Home) || Keyboard.IsKeyDown(Key.End)))
#else
                if (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) &&(currentKey==Key.Home || currentKey==Key.End))
#endif
                    return false;
            }
#endif
            return true;
        }

        /// <summary>
        /// Check whether the Control key is pressed.
        /// </summary>
        /// <returns></returns>
        private bool CheckControlKeyPressed()
        {
#if WinRT
            if (Window.Current.CoreWindow.GetAsyncKeyState(Key.Control).HasFlag(CoreVirtualKeyStates.Down))
#elif WPF
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
#else
            if (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
#endif
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method whcih returns the First row index in DataGrid.
        /// </summary>
        /// <returns></returns>
        private int GetFirstRowIndex()
        {
            return this.DataGrid.HeaderLineCount;
        }

        /// <summary>
        /// Method which return the mouse position in DataGrid.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        private Point GetPointPosition(MouseButtonEventArgs args, UIElement relativeTo)
        {
#if WinRT
            return args.GetCurrentPoint(relativeTo).Position;
#else
            return args.GetPosition(relativeTo);
#endif
        }
#if !WP
        /// <summary>
        /// Method which returns the Last row index in DataGrid. This method was used in Detail View.
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        private int GetLastRowIndex(SfDataGrid dataGrid)
        {
            int count;
            var lastRowIndex = dataGrid.VisualContainer.RowCount - dataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom) - 1;
            for (var start = lastRowIndex; start >= 0; start--)
            {
                if (dataGrid.VisualContainer.RowHeights.GetHidden(start, out count)) continue;
                lastRowIndex = start;
                break;
            }
            return lastRowIndex;
        }

       

        /// <summary>
        /// Method which returns the next row index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetNextRowIndex(int index)
        {
            if (index < this.DataGrid.HeaderLineCount)
            {
                if (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended && index!=pressedIndex && dataGrid.AddNewRowPosition == AddNewRowPosition.Top)
                    return this.DataGrid.HeaderLineCount - 1;
                return this.DataGrid.HeaderLineCount;
            }
            if (index >= this.DataGrid.VisualContainer.ScrollRows.LineCount)
                return -1;
            var nextIndex = this.DataGrid.VisualContainer.ScrollRows.GetNextScrollLineIndex(index);// (this.CurrentCellIndex.RowIndex + 1);
            if (nextIndex == -1 || nextIndex >= (this.DataGrid.VisualContainer.RowCount - this.DataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom)))
                nextIndex = index;
            //nextIndex = nextIndex < (this.DataGrid.VisualContainer.RowCount - this.DataGrid.TableSummaryRows.Count) ? nextIndex : (this.DataGrid.VisualContainer.RowCount - this.DataGrid.TableSummaryRows.Count - 1);
            nextIndex = (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended && nextIndex <= pressedIndex && nextIndex != GetLastRowIndex()) ? this.DataGrid.VisualContainer.ScrollRows.GetPreviousScrollLineIndex(nextIndex) : nextIndex;
            nextIndex = ((this.DataGrid is DetailsViewDataGrid) && CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended && nextIndex <= pressedIndex && nextIndex > index) ? this.DataGrid.VisualContainer.ScrollRows.GetPreviousScrollLineIndex(nextIndex) : nextIndex;
            return nextIndex;
        }

        /// <summary>
        /// Method which return the previous index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetPreviousRowIndex(int index)
        {
            if (index <= this.DataGrid.HeaderLineCount)
            {
                if (dataGrid.AddNewRowPosition == AddNewRowPosition.Top)
                    return this.DataGrid.HeaderLineCount - 1;
                return this.DataGrid.HeaderLineCount;
            }
            var previousIndex = this.DataGrid.VisualContainer.ScrollRows.GetPreviousScrollLineIndex(index);
            previousIndex = (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended) ? index : previousIndex;
            previousIndex = (CheckShiftKeyPressed() && (previousIndex <= pressedIndex || this.lastPressedKey == Key.Tab) && this.DataGrid.SelectionMode == GridSelectionMode.Extended) ? this.DataGrid.VisualContainer.ScrollRows.GetPreviousScrollLineIndex(previousIndex) : previousIndex;
            return previousIndex;
        }

        /// <summary>
        /// Method which returns the Next page index.
        /// </summary>
        /// <returns></returns>
        private int GetNextPageIndex()
        {
            var rowIndex = this.DataGrid.IsAddNewIndex(CurrentCellManager.CurrentCellIndex.RowIndex) ? CurrentCellManager.CurrentCellIndex.RowIndex + 1 : CurrentCellManager.CurrentCellIndex.RowIndex;
            if (rowIndex < this.DataGrid.HeaderLineCount)
                return this.DataGrid.VisualContainer.ScrollRows.GetNextPage(0);
            var nextPageIndex = this.DataGrid.VisualContainer.ScrollRows.GetNextPage(rowIndex);
            nextPageIndex = nextPageIndex < (this.DataGrid.VisualContainer.RowCount - this.DataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom)) ? nextPageIndex : this.GetLastRowIndex();
            return nextPageIndex;
        }

        /// <summary>
        /// Method which returen the previous page index.
        /// </summary>
        /// <returns></returns>
        private int GetPreviousPageIndex()
        {
            int previousPageIndex = this.DataGrid.VisualContainer.ScrollRows.GetPreviousPage(CurrentCellManager.CurrentCellIndex.RowIndex);
            previousPageIndex = previousPageIndex < this.DataGrid.HeaderLineCount ? this.DataGrid.HeaderLineCount : previousPageIndex;
            return previousPageIndex;
        }

        /// <summary>
        /// Method whcih returns the last row index in DataGrid.
        /// </summary>
        /// <returns></returns>
        private int GetLastRowIndex()
        {
            int count;
            int index = this.DataGrid.VisualContainer.RowCount - this.dataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom) - 1;
            if (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                index -= 1;
            for (int start = index; start >= 0; start--)
            {
                if (!this.DataGrid.VisualContainer.RowHeights.GetHidden(start, out count))
                    return start;
            }
            return index;
        }
#endif
        /// <summary>
        /// Method which helps to find whether the group is expanded or not.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool CheckGroupExpanded(Group group)
        {
            bool isExpanded = false;
            if (group != null && group.IsExpanded)
            {
                if (!group.IsTopLevelGroup)
                {
                    isExpanded = this.CheckGroupExpanded(group.Parent as Group);
                }
                else
                {
                    isExpanded = true;
                }
            }
            return isExpanded;
        }

        private bool HasFilter()
        {
            bool hasFilter=false;
            if(DataGrid.View is QueryableCollectionView)
                hasFilter= (DataGrid.View as QueryableCollectionView).RowFilter!=null;
#if !WP
            else if(DataGrid.View is Syncfusion.Data.PagedCollectionView)
                hasFilter= (DataGrid.View as Syncfusion.Data.PagedCollectionView).RowFilter!=null;
#endif
#if WPF
            else if(DataGrid.View is DataTableCollectionView)
                hasFilter= (DataGrid.View as DataTableCollectionView).Filter!=null;
#endif
            else if(DataGrid.View is VirtualizingCollectionView)
                hasFilter=(DataGrid.View as VirtualizingCollectionView).RowFilter!=null;
            else
                hasFilter= DataGrid.View.Filter!=null;
            return hasFilter;
        }

        #endregion

        #endregion

        #region Protected Methods
        private int suspendcount = -1;
        /// <summary>
        /// Method which helps to suspend the other updates when doing selection operation.
        /// </summary>
        protected void SuspendUpdates()
        {
            suspendcount++;
            this.isSuspended = true;
        }

        /// <summary>
        /// Method which resumes the selection updates.
        /// </summary>
        protected void ResumeUpdates()
        {
            if (suspendcount == 0)
                this.isSuspended = false;
            suspendcount--;
        }

        /// <summary>
        /// Method which helps to add the selection in corresponding row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="addedItems"></param>
        /// <param name="reason"></param>
        protected void AddSelection(int rowIndex, List<object> addedItems, SelectionReason reason)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return;
            this.SuspendUpdates();
            if (!this.SelectedRows.Contains(rowIndex) && rowIndex >= 0)
            {
                this.SelectedRows.Add(rowIndex);
            }
            this.ShowRowSelectionBorder(rowIndex);
            if (addedItems != null)
            {
                addedItems.ForEach(item =>
                {
                    if (!this.DataGrid.SelectedItems.Contains(item) && item != null)
                        this.DataGrid.SelectedItems.Add(item);
                });
            }
            if (this.dataGrid.SelectedItems.Count > 0)
                this.DataGrid.SelectedItem = this.DataGrid.SelectedItems[0];
            
            if (this.SelectedRows.Count > 0)
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.SelectedRows[0]);

            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which helps to remove the selection in corresponding row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="removedItems"></param>
        /// <param name="reason"></param>
        protected void RemoveSelection(int rowIndex, List<object> removedItems, SelectionReason reason, bool needToAddSelection = false)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return;
            this.SuspendUpdates();
            if (this.SelectedRows.Contains(rowIndex))
            {
                this.SelectedRows.Remove(rowIndex);
            }
            this.HideRowSelectionBorder(rowIndex);
            removedItems.ForEach(item =>
            {
                if (this.DataGrid.SelectedItems.Contains(item))
                    this.DataGrid.SelectedItems.Remove(item);
            });

            if (this.SelectedRows.Count > 0)
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.SelectedRows[0]);
            else if (!needToAddSelection)
                this.DataGrid.SelectedIndex = -1;

            if (this.DataGrid.SelectedItems.Count > 0)
                this.DataGrid.SelectedItem = this.DataGrid.SelectedItems[0];
            else if(!needToAddSelection)
                this.DataGrid.SelectedItem = null;
           
            if (this.DataGrid.SelectionMode != GridSelectionMode.Multiple || reason != SelectionReason.KeyPressed)
            {
                if (SelectedRows.Count > 0)
                {
                    int index = this.SelectedRows.LastOrDefault();
#if !WP
                    if (this.DataGrid.SelectionMode == GridSelectionMode.Extended && CheckShiftKeyPressed() && reason == SelectionReason.KeyPressed && this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(index))
                    {
                        var dataRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == index);
                        if (dataRow is DetailsViewDataRow)
                            CurrentCellManager.SetCurrentRowIndex(index);
                        else
                        {
                            this.SelectedRows.Remove(index);
                            CurrentCellManager.SetCurrentRowIndex(this.SelectedRows.LastOrDefault());
                        }

                    }
                    else
#endif
                        CurrentCellManager.SetCurrentRowIndex(index);
                    
                }
                else
                    CurrentCellManager.SetCurrentRowIndex(-1);
            }
            this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(CurrentCellManager.CurrentCellIndex.RowIndex));
            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which helps to raise the property cchanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method which helps to raise the selection changing event.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <param name="removedItems"></param>
        /// <param name="addedIndexes"></param>
        /// <param name="removedIndexes"></param>
        /// <returns></returns>
        protected bool RaiseSelectionChanging(List<object> addedItems, List<object> removedItems, List<int> addedIndexes, List<int> removedIndexes)
        {
            var args = new GridSelectionChangingEventArgs(this.DataGrid)
                {
                    AddedItems = addedItems,
                    RemovedItems = removedItems,
                    AddedIndexs = addedIndexes,
                    RemovedIndexs = removedIndexes
                };
            return this.DataGrid.RaiseSelectionChagingEvent(args);
        }

        /// <summary>
        /// Method which helps to Raise the SelectionChanged event.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <param name="removedItems"></param>
        /// <param name="addedIndexes"></param>
        /// <param name="removedIndexes"></param>
        protected void RaiseSelectionChanged(List<object> addedItems, List<object> removedItems, List<int> addedIndexes, List<int> removedIndexes)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return;
            var args = new GridSelectionChangedEventArgs(this.DataGrid)
                {
                    AddedItems = addedItems,
                    RemovedItems = removedItems,
                    AddedIndexs = addedIndexes,
                    RemovedIndexs = removedIndexes
                };
            this.DataGrid.RaiseSelectionChangedEvent(args);
        }

        /// <summary>
        /// Method which returns the record in the row index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected object GetRecordAtRowIndex(int index)
        {
            if (DataGrid.IsAddNewIndex(index))
            {
                return DataGrid.View.IsAddingNew ? DataGrid.View.CurrentAddItem : null;
            }
            else
            {
                if (this.DataGrid.GridModel.HasGroup)
                {
                    var newObj = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(index)];
                    if (newObj is RecordEntry)
                    {
                        return ((RecordEntry)newObj).Data;
                    }
                }
                else
                {
                    var resolvedIndex = this.DataGrid.ResolveToRecordIndex(index);
                    var recordsCount = this.dataGrid.View.Records.Count;
                    return (recordsCount > 0 && resolvedIndex >= 0 && resolvedIndex < recordsCount)
                               ? this.DataGrid.View.Records[resolvedIndex].Data
                               : null;
                }
            }
            return null;
        }

        /// <summary>
        /// Method which helps to show the Focus borderin corresponding row.
        /// </summary>
        /// <param name="rowIndex"></param>
        protected void ShowRowFocusBorder(int rowIndex)
        {
#if !WP
             DataRowBase row = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowIndex);
             if (row != null && !row.IsSelectedRow && ((row.RowType != RowType.DefaultRow && this.DataGrid.NavigationMode == NavigationMode.Cell) || this.DataGrid.NavigationMode == NavigationMode.Row))
             {
                 row.IsFocusedRow = true;
             }
#endif
        }

        /// <summary>
        /// Method which helps to hide the focus border.
        /// </summary>
        protected void HideRowFocusBorder()
        {
             DataRowBase row = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsFocusedRow);
             if (row != null)
             {
                 row.IsFocusedRow = false;
             }
        }

        /// <summary>
        /// Method which shows the Selection border in corresponding row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        protected void ShowRowSelectionBorder(int rowIndex)
        {
            DataRowBase row = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowIndex);
            if (row != null)
            {
                row.IsSelectedRow = true;
            }
        }

        /// <summary>
        /// Method which hides the selection border in corresponding row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        protected void HideRowSelectionBorder(int rowIndex)
        {
            DataRowBase row = this.DataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowIndex);
            if (row != null)
            {
                row.IsSelectedRow = false;
            }
        }

        /// <summary>
        /// Method which hides the selection border in all rows.
        /// </summary>
        /// <param name="exceptCurrentRow"></param>
        protected void HideAllRowSelectionBorder(bool exceptCurrentRow)
        {
            if (!exceptCurrentRow)
                this.DataGrid.RowGenerator.Items.ForEach(item => { if (item.IsSelectedRow) item.IsSelectedRow = false; });
            else
                this.DataGrid.RowGenerator.Items.ForEach(item => { if (item.RowData != this.DataGrid.View.CurrentItem && item.IsSelectedRow) item.IsSelectedRow = false; });
        }

        /// <summary>
        /// Method which shows the selection border in all rows.
        /// </summary>
        protected void ShowAllRowSelectionBorder()
        {
            this.DataGrid.RowGenerator.Items.ForEach(item => { if (!item.IsSelectedRow) item.IsSelectedRow = true; });
        }
       
        /// <summary>
        /// Method which helps to scroll the row in View from bottom.
        /// </summary>
        /// <param name="rowIndex"></param>
        protected void ScrollInViewFromBottom(int rowIndex)
        {
            if (rowIndex < 0)
                return;
            int firstBodyVisibleIndex=-1;
            VisibleLineInfo lineInfo = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
            if (lineInfo == null)
            {
                var visibleLines = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLines();
                if (visibleLines.FirstBodyVisibleIndex < visibleLines.Count)
                    firstBodyVisibleIndex = visibleLines[visibleLines.FirstBodyVisibleIndex].LineIndex;
            }
            if (rowIndex > this.DataGrid.VisualContainer.ScrollRows.LastBodyVisibleLineIndex ||(firstBodyVisibleIndex>=0 && rowIndex<firstBodyVisibleIndex) ||(lineInfo!=null && lineInfo.IsClipped))
            {
#if !WP
                if (this.dataGrid.IsInDetailsViewIndex(rowIndex) && this.dataGrid.SelectedDetailsViewGrid != null)
                {
                    var detailsViewGrid = this.dataGrid.SelectedDetailsViewGrid;
                    var detailsViewSelectedIndex = detailsViewGrid.ResolveToRowIndex(detailsViewGrid.SelectedIndex);
                    var line = detailsViewGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(detailsViewSelectedIndex);
                    if (line == null)
                    {
                        var firstRowIndex = detailsViewGrid.VisualContainer.ScrollRows.HeaderLineCount;
                        if (detailsViewSelectedIndex >= 0)
                            detailsViewGrid.VisualContainer.ScrollRows.ScrollInView(detailsViewSelectedIndex);
                        var delta = 0.0;
                        if (detailsViewSelectedIndex == firstRowIndex)
                        {
                            delta = detailsViewGrid.VisualContainer.ScrollRows.HeaderExtent +
                                    detailsViewGrid.VisualContainer.ScrollRows.FooterExtent +
                                    this.dataGrid.DetailsViewPadding.Top +
                                    this.dataGrid.DetailsViewPadding.Bottom +
                                    this.dataGrid.DetailsViewPadding.Bottom;
                        }
                        delta += detailsViewGrid.VisualContainer.ScrollRows.GetLineSize(detailsViewSelectedIndex);
                        this.dataGrid.VisualContainer.VScrollBar.Value += delta;
                        this.DataGrid.VisualContainer.InvalidateMeasureInfo();
                        return;
                    }
                    else
                    {                        
                        if (detailsViewSelectedIndex >= 0)
                            detailsViewGrid.VisualContainer.ScrollRows.ScrollInView(detailsViewSelectedIndex);
                        var parentLine = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
                        if (parentLine == null)
                        {
                            this.DataGrid.VisualContainer.ScrollRows.ScrollInView(rowIndex);
                            this.DataGrid.VisualContainer.InvalidateMeasureInfo();
                        }
                        return;
                    }
                }
#endif
                this.DataGrid.VisualContainer.ScrollRows.ScrollInView(rowIndex);
                this.DataGrid.VisualContainer.InvalidateMeasureInfo();
            }
        }

        /// <summary>
        /// Method which helps to view the row from top.
        /// </summary>
        /// <param name="rowIndex"></param>
        protected void ScrollInViewFromTop(int rowIndex)
        {
            if (this.dataGrid.AllowFrozenGroupHeaders && dataGrid.View != null)
            {
                rowIndex = rowIndex - this.dataGrid.View.GroupDescriptions.Count;
            }
            if (rowIndex < 0 || rowIndex >= this.DataGrid.VisualContainer.ScrollRows.LineCount)
                return;
            
#if !WP
            if (this.dataGrid.IsInDetailsViewIndex(rowIndex) && this.dataGrid.SelectedDetailsViewGrid != null)
            {
                var detailsViewGrid = this.dataGrid.SelectedDetailsViewGrid;
                var detailsViewSelectedIndex = detailsViewGrid.ResolveToRowIndex(detailsViewGrid.SelectedIndex);
                var lastRowIndex = GetLastRowIndex(detailsViewGrid);
                if (detailsViewSelectedIndex >= 0)
                    detailsViewGrid.VisualContainer.ScrollRows.ScrollInView(detailsViewSelectedIndex);
                var delta = 0.0;
                if (detailsViewSelectedIndex == lastRowIndex)
                {
                    delta = detailsViewGrid.VisualContainer.ScrollRows.HeaderExtent +
                            detailsViewGrid.VisualContainer.ScrollRows.FooterExtent +
                            this.dataGrid.DetailsViewPadding.Top +
                            this.dataGrid.DetailsViewPadding.Bottom +
                            this.dataGrid.DetailsViewPadding.Bottom;
                }
                delta += detailsViewGrid.VisualContainer.ScrollRows.GetLineSize(detailsViewSelectedIndex);
                this.dataGrid.VisualContainer.VScrollBar.Value -= delta;
                this.DataGrid.VisualContainer.InvalidateMeasureInfo();
                return;
            }
#endif
            VisibleLinesCollection lineCollection = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLines();
            VisibleLineInfo lineInfo = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(rowIndex);

            if ((lineCollection.FirstBodyVisibleIndex < lineCollection.Count) && (rowIndex < lineCollection[lineCollection.firstBodyVisibleIndex].LineIndex || rowIndex > this.DataGrid.VisualContainer.ScrollRows.LastBodyVisibleLineIndex || (lineInfo != null && lineInfo.IsClipped)))
            {
                this.DataGrid.VisualContainer.ScrollRows.ScrollInView(rowIndex);
                this.DataGrid.VisualContainer.InvalidateMeasureInfo();
            }
        }

        /// <summary>
        /// Method which is used to reset the Selected Row index when the records reseted while Sorting, Filtering and Grouping.
        /// </summary>
        /// <remarks></remarks>
        protected void ResetSelectedRows()
        {
            if (this.DataGrid.SelectedItems.Count > 0 || (this.CurrentCellManager.CurrentCellIndex != default(RowColumnIndex) && this.dataGrid.SelectionMode == GridSelectionMode.Multiple))
            {
                int previousCurrentCellIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;

                if (this.DataGrid.SelectedItems.Count > this.SelectedRows.Count)
                {
                    this.SelectedRows.Clear();
                    foreach (var item in this.DataGrid.SelectedItems)
                    {
                        this.SelectedRows.Add(this.DataGrid.ResolveToRowIndex(item));
                    }
                }
                else
                {
                    int index = 0;
                    while (index < this.DataGrid.SelectedItems.Count)
                    {
                        this.SelectedRows[index] = this.DataGrid.ResolveToRowIndex(this.DataGrid.SelectedItems[index]);
                        index++;
                    }
                }
#if !WP
                if (this.DataGrid.View != null && this.DataGrid.View.CurrentEditItem != null)
                    CurrentCellManager.EndEdit();
#endif
                if (SelectedRows.Count > 0)
                {
                    this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.SelectedRows.FirstOrDefault());
                }
            }
        }


        protected void UpdateCurrentRowIndex()
        {
            bool isAvailable= false;
            if (this.DataGrid.CurrentItem != null)
            {
                var record = this.DataGrid.View.Records.GetRecord(this.DataGrid.CurrentItem);
                if (record != null)
                {
                    if ((this.DataGrid.View as CollectionViewAdv).IsGrouping)
                    {
                        var group = record.Parent as Group;
                        isAvailable = CheckGroupExpanded(group);
                    }
                    else
                        isAvailable = true;
                }
            }
            var rowIndex = -1;
            if (isAvailable)
            {
                rowIndex = this.dataGrid.ResolveToRowIndex(this.DataGrid.CurrentItem);
            }
            else
            {
                if (this.DataGrid.CurrentItem != null)
            {
                if (this.DataGrid.CurrentItem != null)
                {
                    var currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.IsCurrentRow);
                    if (currentRow != null)
                    {
                        currentRow.IsCurrentRow = false;
                        (currentRow as GridDataRow).ApplyRowHeaderVisualState();
                    }
                }
                }
                if (this.SelectedRows.Count > 0)
                    rowIndex = this.SelectedRows.LastOrDefault();
            }

            if (this.CurrentCellManager.CurrentCellIndex.RowIndex != rowIndex)
            {
                CurrentCellManager.RemoveCurrentCellSelection(this.CurrentCellManager.CurrentCellIndex);
                this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                if (this.CurrentCellManager.CurrentCellIndex.RowIndex != -1)
                {
                    CurrentCellManager.SelectCurrentCell(this.CurrentCellManager.CurrentCellIndex);
                    if (!SuspendAutoScrolling)
                        CurrentCellManager.ScrollInView(CurrentCellManager.CurrentCellIndex);
                }
            }
            if (!isAvailable && this.DataGrid.View!=null)
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
        }

        /// <summary>
        /// Method which is used to reset the selected items when we Filter the records.
        /// </summary>
        /// <remarks></remarks>
        protected void RefreshSelectedItems()
        {
            this.SuspendUpdates();
            int index = this.DataGrid.SelectedItems.Count - 1;
            while (index >= 0)
            {
                object item = this.DataGrid.SelectedItems[index];
                bool isAvailable = false;
                var recordentry = this.DataGrid.View.Records.GetRecord(item);
                if (recordentry != null)
                {
                    if (this.DataGrid.GridModel.HasGroup)
                    {
                        isAvailable = this.CheckGroupExpanded(recordentry.Parent as Group);
                    }
                    else
                    {
                        isAvailable = true;
                    }
                }
                if (!isAvailable)
                {
                    this.DataGrid.SelectedItems.Remove(item);
                }
                index--;
            }

            if (this.DataGrid.SelectedItems.Count < this.SelectedRows.Count)
            {
                if (this.DataGrid.SelectedItems.Count > 0)
                    this.SelectedRows.Clear();
                index = this.DataGrid.SelectedItems.Count - 1;
                while (index >= 0)
                {
                    this.SelectedRows.Add(this.DataGrid.ResolveToRowIndex(this.DataGrid.SelectedItems[index]));
                    index--;
                }
            }

            if (this.DataGrid.SelectedItems.Count > 0)
            {
                this.DataGrid.SelectedItem = this.DataGrid.SelectedItems[0];
                this.DataGrid.SelectedIndex = this.DataGrid.ResolveToRecordIndex(this.SelectedRows[0]);
            }
            else
            {
                if (this.SelectedRows.Count > 0)
                    this.SelectedRows.Clear();
                this.DataGrid.SelectedItem = null;
                this.DataGrid.SelectedIndex = -1;
            }
            this.ResumeUpdates();
        }

        /// <summary>
        /// Method which helps to process the selection on corresponding row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected bool ProcessSelection(int rowIndex, SelectionReason reason)
        {
            bool needToMove = true;
            int currentColumnIndex = this.CurrentCellManager.CurrentCellIndex.ColumnIndex;
            bool needToRemove = this.DataGrid.SelectionMode == GridSelectionMode.Single || (this.DataGrid.SelectionMode == GridSelectionMode.Extended && ((!CheckControlKeyPressed() && (reason == SelectionReason.PointerReleased || reason == SelectionReason.PointerPressed)) || (reason == SelectionReason.KeyPressed && !CheckShiftKeyPressed()) || (CheckShiftKeyPressed() && (this.lastPressedKey == Key.Tab || this.lastPressedKey == Key.A)))) || (reason == SelectionReason.KeyPressed && this.DataGrid.SelectionMode == GridSelectionMode.Multiple);
            bool needToRemoveSameRow = ((this.DataGrid.SelectionMode == GridSelectionMode.Extended && ((CheckControlKeyPressed() && reason != SelectionReason.KeyPressed) || (reason == SelectionReason.KeyPressed && CheckShiftKeyPressed() && this.lastPressedKey != Key.Tab && this.lastPressedKey != Key.A))) || this.DataGrid.SelectionMode == GridSelectionMode.Multiple) && this.SelectedRows.Contains(rowIndex);

            if (needToRemoveSameRow)
                needToRemoveSameRow = CheckCanRemoveSameRow();

            var data = this.GetRecordAtRowIndex(rowIndex);
            if (needToRemoveSameRow && (CheckShiftKeyPressed() && this.DataGrid.SelectionMode == GridSelectionMode.Extended && ((this.pressedIndex < rowIndex && this.CurrentCellManager.CurrentCellIndex.RowIndex < rowIndex) || (this.pressedIndex > rowIndex && this.CurrentCellManager.CurrentCellIndex.RowIndex > rowIndex))))
            {
                needToRemoveSameRow = !needToRemoveSameRow;
                this.SelectedRows.Remove(rowIndex);
                this.SuspendUpdates();
                this.DataGrid.SelectedItems.Remove(data);
                this.ResumeUpdates();
            }

            var addedItems = new List<object>();
            var removedItems = new List<object>();
            var addedIndexes = new List<int>();
            var removedIndexes = new List<int>();
            addedIndexes.Add(rowIndex);
            if (data != null)
                addedItems.Add(data);
            if (this.CurrentCellManager.CurrentCellIndex.RowIndex > this.DataGrid.GetHeaderIndex() && (this.CurrentCellManager.CurrentCellIndex.RowIndex != rowIndex || (this.DataGrid.SelectionMode == GridSelectionMode.Extended && this.SelectedRows.Count > 0)))//&& needToRemove)
            {
                if (this.SelectedRows.Count > 0)
                {
                    removedItems = this.DataGrid.SelectedItems.ToList();
                    removedIndexes = this.SelectedRows.ToList();
                }
                else
                {
                    data = this.GetRecordAtRowIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex);
                    if (data != null)
                        removedItems.Add(data);
                }
            }

            if (needToRemoveSameRow)
            {
                removedItems.Clear();
                if (addedItems.Count > 0)
                    removedItems.Add(addedItems.FirstOrDefault());
                if (this.RaiseSelectionChanging(null, removedItems, null, removedIndexes))
                {
                    CurrentCellManager.RemoveCurrentCellSelection(new RowColumnIndex(rowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex));
                    CurrentCellManager.SelectCurrentCell(CurrentCellManager.previousCellIndex);
                    UpdateCurrentRow();
                    return false;
                }
#if !WP
                if (this.DataGrid.SelectionMode == GridSelectionMode.Extended)
                {
                    CurrentCellManager.EndEdit();
                    CurrentCellManager.RemoveCurrentCellSelection(new RowColumnIndex(rowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex));
                }
#endif

                this.HideRowFocusBorder();
                this.RemoveSelection(rowIndex, removedItems, reason);
#if !WP
                if (this.DataGrid.SelectionMode == GridSelectionMode.Extended && this.DataGrid.NavigationMode != NavigationMode.Row)
                {
                    CurrentCellManager.SelectCurrentCell(this.CurrentCellManager.CurrentCellIndex);
                }
#endif

                if (this.DataGrid.SelectionMode == GridSelectionMode.Multiple)
                {
                    this.CurrentCellManager.SetCurrentRowIndex(rowIndex);
                    this.ShowRowFocusBorder(rowIndex);
                }

                this.SuspendUpdates();
                this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(this.CurrentCellManager.CurrentCellIndex.RowIndex));
                this.ResumeUpdates();

                UpdateCurrentRow();
                this.RaiseSelectionChanged(null, removedItems, null, removedIndexes);
                return true;
            }

            if (this.CurrentCellManager.CurrentCellIndex.RowIndex != rowIndex || (this.DataGrid.SelectionMode == GridSelectionMode.Extended && this.SelectedRows.Count > 1 && !(CheckShiftKeyPressed() || CheckControlKeyPressed())) || CurrentCellManager.CanSelectRow(rowIndex) || (this.DataGrid.SelectionMode == GridSelectionMode.Multiple))
            {
                if (!needToRemove)
                {
                    removedIndexes.Clear();
                    removedItems.Clear();
                }
                if (this.RaiseSelectionChanging(addedItems, removedItems, addedIndexes, removedIndexes))
                {
                    CurrentCellManager.RemoveCurrentCellSelection(new RowColumnIndex(rowIndex, this.CurrentCellManager.CurrentCellIndex.ColumnIndex));
                    CurrentCellManager.SelectCurrentCell(CurrentCellManager.previousCellIndex);
                    UpdateCurrentRow();
                    return false;
                }
                if (needToRemove)
                {
                    if (this.SelectedRows.Count > 1)
                    {
                        if (lastPressedKey == Key.A && CheckShiftKeyPressed())
                        {
                            this.ClearSelections(true);
                            this.pressedIndex = this.CurrentCellManager.CurrentCellIndex.RowIndex;
                        }
                        else
                        {
                            this.ClearSelections(false, false);
                            this.CurrentCellManager.SetCurrentColumnIndex(currentColumnIndex);
                        }
                        lastPressedKey = lastPressedKey == Key.A ? Key.None : lastPressedKey;
                    }
                    else
                        this.RemoveSelection(this.SelectedRows.FirstOrDefault(), removedItems, reason, addedItems.Count > 0);
                }


                this.AddSelection(rowIndex, addedItems, reason);

                if (this.DataGrid.GridModel.HasGroup)
                {
                    var record =
                        this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(rowIndex)];
                    if (!(record is RecordEntry))
                        needToMove = false;
                }

                this.SuspendUpdates();

                if (needToMove)
                    this.DataGrid.View.MoveCurrentToPosition(this.DataGrid.ResolveToRecordIndex(rowIndex));
                else
                {
                    this.DataGrid.View.MoveCurrentToPosition(-1);
                }

                this.ResumeUpdates();

                this.HideRowFocusBorder();

                this.CurrentCellManager.SetCurrentRowIndex(rowIndex);

                var currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.IsCurrentRow);
                if (currentRow != null)
                {
                    currentRow.IsCurrentRow = false;
                    (currentRow as GridDataRow).ApplyRowHeaderVisualState();
                }

                currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex);
                if (currentRow != null)
                {
                    currentRow.IsCurrentRow = true;
                    (currentRow as GridDataRow).ApplyRowHeaderVisualState();
                }
                this.RaiseSelectionChanged(addedItems, removedItems, addedIndexes, removedIndexes);
                return true;
            }

            return false;
        }

        private void UpdateCurrentRow()
        {
            if (this.DataGrid.RowGenerator.Items.All(row => row.RowRegion == RowRegion.Header || row.RowIndex == -1))
                return;
            var currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.IsCurrentRow);
            if (currentRow != null)
            {
                currentRow.IsCurrentRow = false;
                (currentRow as GridDataRow).ApplyRowHeaderVisualState();
            }
            currentRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.CurrentCellManager.CurrentCellIndex.RowIndex);
            if (currentRow != null)
            {
                currentRow.IsCurrentRow = true;
                (currentRow as GridDataRow).ApplyRowHeaderVisualState();
            }
        }
       
        #endregion

#if !WP

        #region Details View

        /// <summary>
        /// Method which handles the Key navigation in DetailsViewGrid.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rowIndex"></param>
        /// <param name="processKey"></param>
        /// <param name="returnIndex"></param>
        /// <returns></returns>
        private bool ProcessDetailsViewIndex(KeyEventArgs args, int rowIndex, Key processKey, out int returnIndex)
        {
            if (this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(rowIndex))
            {
                var dataRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowIndex && row.IsEnsured);
                if (dataRow == null)
                {
                    this.DataGrid.DetailsViewManager.BringIntoView(rowIndex);
                    dataRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
                }
                if (dataRow is DetailsViewDataRow)
                {
                    var detailsViewDataRow = dataRow as DetailsViewDataRow;
                    var result =
                        detailsViewDataRow.DetailsViewDataGrid.SelectionController.HandleDetailsViewGridKeyDown(args);
                    if (result)
                    {
                        returnIndex = rowIndex;
                        this.DataGrid.SelectedDetailsViewGrid = detailsViewDataRow.DetailsViewDataGrid;
                        return true;
                    }
                    else
                    {
                        if (processKey == Key.Down || processKey == Key.Tab)
                            rowIndex = this.GetNextRowIndex(rowIndex);
                        else if (processKey == Key.Up || (processKey == Key.Tab && CheckControlKeyPressed()))
                            rowIndex = this.GetPreviousRowIndex(rowIndex);
                        if (ProcessDetailsViewIndex(args, rowIndex, processKey, out returnIndex))
                        {
                            returnIndex = rowIndex;
                            return true;
                        }
                        else
                            this.DataGrid.SelectedDetailsViewGrid = null;
                    }
                }
            }
            returnIndex = rowIndex;
            return false;
        }

        /// <summary>
        /// Method which decides whether the index is in Details view grid. and proceed the Key navigation operation in Details view grid.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rowIndex"></param>
        /// <param name="processKey"></param>
        private void ProcessDetailsViewKeyDown(KeyEventArgs args, int rowIndex, Key processKey)
        {
            if (this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.IsInDetailsViewIndex(rowIndex) && !(rowIndex == CurrentCellManager.CurrentCellIndex.RowIndex && this.DataGrid.SelectionMode == GridSelectionMode.Extended && CheckShiftKeyPressed()))
            {
                int outRowIndex;
                if (!ProcessDetailsViewIndex(args, rowIndex, processKey, out outRowIndex))
                {
                    rowIndex = outRowIndex;
                    this.DataGrid.SelectedDetailsViewGrid = null;
                }
            }
            else if (this.DataGrid.SelectedDetailsViewGrid != null)
            {
                this.ClearDetailsViewGridSelections(this.DataGrid.SelectedDetailsViewGrid as DetailsViewDataGrid);
                this.DataGrid.SelectedDetailsViewGrid = null;
            }
        }

        /// <summary>
        /// Method whhich helps to handle the selection in DetailsView grid when perform Pointer pressed operations.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rowColumnIndex"></param>
        protected virtual void ProcessDetailsViewGridPointerPressed(MouseButtonEventArgs args, RowColumnIndex rowColumnIndex)
        {
            var parentDataGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
            var detailsViewDataRow = parentDataGrid.RowGenerator.Items.FirstOrDefault(row => (row is DetailsViewDataRow) && (row as DetailsViewDataRow).DetailsViewDataGrid == this.DataGrid);
            var colIndex = parentDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex < 0
                               ? parentDataGrid.SelectionController.CurrentCellManager.GetFirstCellIndex()
                               : parentDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex;
            var rowcolIndex = new RowColumnIndex(detailsViewDataRow.RowIndex, colIndex);
            parentDataGrid.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Pressed, args), rowcolIndex);
        }

        /// <summary>
        /// Method whhich helps to handle the selection in DetailsView grid when perform Pointer pressed operations.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rowColumnIndex"></param>
        protected virtual void ProcessDetailsViewGridPointerReleased(MouseButtonEventArgs args, RowColumnIndex rowColumnIndex)
        {
            var parentDataGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
            var detailsViewDataRow = parentDataGrid.RowGenerator.Items.FirstOrDefault(row => (row is DetailsViewDataRow) && (row as DetailsViewDataRow).DetailsViewDataGrid == this.DataGrid);
            var colIndex = parentDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex < 0
                               ? parentDataGrid.SelectionController.CurrentCellManager.GetFirstCellIndex()
                               : parentDataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex;
            var rowcolIndex = new RowColumnIndex(detailsViewDataRow.RowIndex, colIndex);
            parentDataGrid.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Released, args), rowcolIndex);
            parentDataGrid.SelectedDetailsViewGrid = this.DataGrid;
        }

        /// <summary>
        /// Method which helps to clear the selection in Details view grid.
        /// </summary>
        /// <param name="detailsViewDataGrid"></param>
        protected virtual bool ClearDetailsViewGridSelections(DetailsViewDataGrid detailsViewDataGrid)
        {
            if (!detailsViewDataGrid.Validations.CheckForValidation(true))
                return false;
            if (detailsViewDataGrid.SelectedDetailsViewGrid is DetailsViewDataGrid)
                ClearDetailsViewGridSelections(detailsViewDataGrid.SelectedDetailsViewGrid as DetailsViewDataGrid);
            detailsViewDataGrid.SelectionController.ClearSelections(false);
            (detailsViewDataGrid.SelectionController as GridSelectionController).pressedIndex = -1;
            detailsViewDataGrid.RowGenerator.Items.ForEach(row =>
            {
                if (row.IsCurrentRow)
                {
                    row.IsCurrentRow = false;
                    (row as GridDataRow).ApplyRowHeaderVisualState();
                }
            });
            return true;
        }

        #endregion

#endif

        #region Dispose 

        public void Dispose()
        {
            this.dataGrid = null;
        }

        #endregion        
    
    }
}
