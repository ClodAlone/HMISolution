#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
    using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
    using DoubleTappedEventArgs = Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs;
    using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
    using TappedEventArgs = Windows.UI.Xaml.Input.TappedRoutedEventArgs;
    using Windows.UI.Xaml.Data;
    using Windows.Devices.Input;
    using Syncfusion.UI.Xaml.Grid.Cells;
    using Windows.UI.Xaml;
    using Windows.UI.Core;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using Syncfusion.Data;
    using Syncfusion.Data.Extensions;
#elif WPF
    using Syncfusion.UI.Xaml.Grid.Cells;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif SILVERLIGHT
    using Syncfusion.UI.Xaml.Grid.Cells;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif WP
    using System.Collections.Specialized;
    using System.Windows.Input;
#endif
    public class GridCurrentCellManager
    {
        #region Fields

        private RowColumnIndex currentCellIndex;
        private SfDataGrid dataGrid;
        internal object oldCellValue;
        internal RowColumnIndex previousCellIndex;
        internal bool isCollectionChanged = false;
        Key lastPressedKey = Key.Space;

        #endregion

        #region Ctor

        public GridCurrentCellManager(SfDataGrid grid)
        {
            dataGrid = grid;
            CurrentCellIndex = new RowColumnIndex(-1, -1);
            previousCellIndex = new RowColumnIndex(-1, -1);
        }

        #endregion

        #region Public Properties

        public RowColumnIndex CurrentCellIndex
        {
            get { return currentCellIndex; }
            private set { currentCellIndex = value; }
        }

        public DataColumnBase CurrentCell { get; set; }

        public bool HasCurrentCell { get { return CurrentCell != null; } }

        #endregion

        #region Internal Methods

        internal void SetCurrentRowIndex(int rowIndex)
        {
            currentCellIndex.RowIndex = rowIndex;
        }

        internal void SetCurrentColumnIndex(int columnIndex)
        {
            currentCellIndex.ColumnIndex = columnIndex;
        }

        internal void SetCurrentColumnBase(DataColumnBase column, bool setFocus)
        {
            CurrentCell = column;

            if (CurrentCell.Renderer != null && !CurrentCell.Renderer.HasCurrentCellState && !CurrentCell.IsEditing)
                CurrentCell.Renderer.SetCurrentCellState(dataGrid.SelectionController.CurrentCellManager.CurrentCellIndex, CurrentCell.ColumnElement, CurrentCell.IsEditing, isCollectionChanged ? false : true);
#if !WP
            if (dataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex) && dataGrid.View.IsAddingNew && !dataGrid.View.IsEditingItem && lastPressedKey == Key.Tab)
                this.BeginEdit();
#endif
            isCollectionChanged = false;
        }

        internal void SetCurrentRowColumnIndex(RowColumnIndex rowColumnIndex)
        {
            CurrentCellIndex = rowColumnIndex;
        }

        #region CommitValue
#if WinRT
        internal void CommitCellValue(bool isNewValue)
        {
            var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == this.CurrentCellIndex.RowIndex);
            if (!dataRow.IsEditing) return;
            var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.ColumnIndex == this.CurrentCellIndex.ColumnIndex);
            if (dataColumn == null || !dataColumn.IsEditing) return;
            object cellvalue;
            if (isNewValue)
                cellvalue = dataColumn.Renderer.GetControlValue();
            else
                cellvalue = oldCellValue;

            if (cellvalue != null)
            {
                var propertyAccessProvider = dataGrid.View.GetPropertyAccessProvider();
                var propertyinfo = dataGrid.View.GetItemProperties().Find(dataColumn.GridColumn.MappingName, false);
                if (propertyinfo == null)
                {
                    string actualproperty = dataColumn.GridColumn.MappingName;
                    string[] propertyNameList = dataColumn.GridColumn.MappingName.Split('.');
                    int complexPropertyCount = propertyNameList.Count();
                    var record = dataRow.RowData;
                    for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                    {
                        var tempProperyDescriptor = dataGrid.View.GetItemProperties().Find(propertyNameList[iterator], true);
                        if (tempProperyDescriptor != null)
                            record = tempProperyDescriptor.GetValue(record);
                    }
                    actualproperty = propertyNameList[complexPropertyCount - 1];
                    propertyinfo = record.GetType().GetProperty(actualproperty);
                }
                if (propertyinfo == null || dataColumn.GridColumn is GridTemplateColumn || dataColumn.GridColumn is GridUnBoundColumn)
                    return;
                if (dataColumn.GridColumn is GridNumericColumn)
                {
                    var numericColumn = (GridNumericColumn)dataColumn.GridColumn;
                    switch (numericColumn.FormatString)
                    {
                        case "p":
                        case "P":
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator, "");
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                            break;
                        case "c":
                        case "C":
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator, "");
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");
                            break;
                        case "n":
                        case "N":
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "");
                            break;
                        default:
                            cellvalue = cellvalue.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "");
                            break;
                    }
                }
                var value = Syncfusion.Data.Helper.ValueConvert.ChangeType(cellvalue, propertyinfo.PropertyType, null);
                if (value != null)
                    propertyAccessProvider.SetValue(dataRow.RowData, dataColumn.GridColumn.MappingName, value);
            }
        }
#endif
        #endregion

#if !WP
        internal bool RaiseValidationAndEndEdit()
        {
            if (!dataGrid.Validations.RaiseCellValidate(CurrentCellIndex))
                return false;
            EndEdit(false);
            return true;
        }

        internal bool CheckValidationAndEndEdit(bool canRaiseEvent)
        {
            if (!dataGrid.Validations.CheckForValidation(true))
                return false;
            EndEdit();
            return true;
        }
#endif



        /// <summary>
        /// Returns the DataColumn for the given RowColumnIndex
        /// </summary>
        /// <param name="rowColumnIndex">Corresponding RowColumnIndex Value</param>
        /// <remarks></remarks>
        internal protected DataColumnBase GetDataColumnBase(RowColumnIndex rowColumnIndex)
        {
            var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowColumnIndex.RowIndex);
            if (dataRow != null)
            {
                var dataColumn = dataRow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == rowColumnIndex.ColumnIndex);
                return dataColumn;
            }
            return null;
        }
#if !WP
        internal bool CanMoveCurrentCell(RowColumnIndex rowColumnIndex)
        {
            if (!AllowFocus(rowColumnIndex) || dataGrid.NavigationMode == NavigationMode.Row)
                return false;
            if (CurrentCellIndex != rowColumnIndex && !dataGrid.Validations.RaiseCellValidate(CurrentCellIndex) || CurrentCellIndex.RowIndex > -1 && CurrentCellIndex.RowIndex != rowColumnIndex.RowIndex && !dataGrid.Validations.RaiseRowValidate(CurrentCellIndex))
                return false;
            return true;
        }
#endif

        /// <summary>
        /// Method which helps to decide whether we can select the current row or not. 
        /// This method used when we need to select the focused row.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        internal bool CanSelectRow(int rowIndex)
        {
            if (CurrentCellIndex.RowIndex == rowIndex)
            {
                var row = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsCurrentRow);
                if (row != null)
                    return !row.IsSelectedRow;
            }
            return false;
        }

        #endregion

        #region Public Methods

        #region Virtual Methods

        public virtual bool HandlePointerOperation(MouseButtonEventArgs args, RowColumnIndex rowColumnIndex)
        {
#if !WP
            if (CurrentCellIndex != rowColumnIndex && AllowFocus(rowColumnIndex) && (dataGrid.NavigationMode != NavigationMode.Row || dataGrid.IsAddNewIndex(rowColumnIndex.RowIndex)))
            {
                if (!dataGrid.Validations.RaiseCellValidate(CurrentCellIndex) || CurrentCellIndex.RowIndex > -1 && CurrentCellIndex.RowIndex != rowColumnIndex.RowIndex && !dataGrid.Validations.RaiseRowValidate(CurrentCellIndex))
                {
                    return false;
                }
                EndEdit(CurrentCellIndex.RowIndex != rowColumnIndex.RowIndex);
                bool cancellRowSelection = CurrentCellIndex.RowIndex == rowColumnIndex.RowIndex && this.dataGrid.IsAddNewIndex(rowColumnIndex.RowIndex);

#if WinRT
                var activationTrigger = ActivationTrigger.Mouse;
                if (args != null)
                    activationTrigger = ConvertPointerDeviceTypeToActivationTrigger(args.Pointer.PointerDeviceType);
                if ((dataGrid.NavigationMode != NavigationMode.Row || dataGrid.IsAddNewIndex(rowColumnIndex.RowIndex)) && !ProcessCurrentCellSelection(rowColumnIndex, activationTrigger))
#else
                if ((dataGrid.NavigationMode != NavigationMode.Row || dataGrid.IsAddNewIndex(rowColumnIndex.RowIndex)) && !ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Mouse))
#endif
                    return false;
                return !cancellRowSelection;
            }

            if (dataGrid.NavigationMode == NavigationMode.Row)
                return true;

            if (CurrentCellIndex == rowColumnIndex && ((dataGrid.SelectionMode == GridSelectionMode.Extended && (CheckControlKeyPressed() || this.dataGrid.SelectionController.SelectedRows.Count > 1) && !this.dataGrid.IsInDetailsViewIndex(rowColumnIndex.RowIndex)) || CanSelectRow(rowColumnIndex.RowIndex) || dataGrid.SelectionMode == GridSelectionMode.Multiple))
                return true;
            return false;
#else
            return true;
#endif
        }
#if !WP
        public virtual bool HandleKeyDown(KeyEventArgs args)
        {
            if (HasCurrentCell)
            {
                if (CurrentCell.Renderer != null && !CurrentCell.Renderer.ShouldGridTryToHandleKeyDown(args))
                {
#if WinRT
                    if (char.IsLetterOrDigit(args.Key.ToString(), 0) && this.dataGrid.AllowEditing)
                    {
                        if (CheckControlKeyPressed())
                            return false;
                        if (!(args.Key >= Key.A && args.Key <= Key.Z) && !(args.Key >= Key.Number0 && args.Key <= Key.Number9) && !(args.Key >= Key.NumberPad0 && args.Key <= Key.NumberPad9))
                            return false;
                        if (CurrentCell.Renderer != null && !(CurrentCell.Renderer is GridCellTemplateRenderer))
                        {
                            if (!CurrentCell.IsEditing)
                            {
                                if (BeginEdit())
                                    CurrentCell.Renderer.PreviewTextInput(args);
                            }
                        }
                    }
#endif
                    return false;
                }
            }
            return true;
        }

        public virtual bool HandleKeyNavigation(KeyEventArgs args)
        {
            Key currentKey = args.Key;
            if (dataGrid.FlowDirection == FlowDirection.RightToLeft)
            {
                switch (args.Key)
                {
                    case Key.Right:
                        currentKey = Key.Left;
                        break;
                    case Key.Left:
                        currentKey = Key.Right;
                        break;
                }
            }
            int rowIndex, columnIndex;
            switch (currentKey)
            {
                case Key.F2:
                    {
                        if (CurrentCell != null && CurrentCell.IsEditing)
                        {
                            if (CheckShiftKeyPressed() || !RaiseValidationAndEndEdit())
                                return false;
                        }
                        else if (CurrentCell != null)
                        {
                            BeginEdit();
                        }
                        args.Handled = true;
                    }
                    break;
                case Key.Right:
                    {
                        if (!RaiseValidationAndEndEdit())
                            return false;
                        rowIndex = CurrentCellIndex.RowIndex;
                        columnIndex = CheckControlKeyPressed() ? this.GetLastCellIndex() : this.GetNextCellIndex();

                        if (CurrentCellIndex.ColumnIndex == columnIndex)
                            return false;

                        if (columnIndex != -1)
                        {
                            var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                            if (ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                            {
                                this.SetCurrentColumnIndex(columnIndex);
                                this.ScrollInViewFromRight(columnIndex);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        lastPressedKey = Key.Right;
                        args.Handled = true;
                    }
                    break;
                case Key.Left:
                    {
                        if (!RaiseValidationAndEndEdit())
                            return false;
                        rowIndex = CurrentCellIndex.RowIndex;
                        columnIndex = (CheckControlKeyPressed())
                                          ? this.GetFirstCellIndex()
                                          : this.GetPreviousCellIndex();

                        if (CurrentCellIndex.ColumnIndex == columnIndex)
                            return false;

                        if (columnIndex != -1)
                        {
                            var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                            if (ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                            {
                                this.ScrollInViewFromLeft(columnIndex);
                            }
                            else
                                return false;
                        }
                        args.Handled = true;
                        lastPressedKey = Key.Right;
                    }
                    break;
                case Key.Escape:
                    if (dataGrid.NavigationMode == NavigationMode.Cell || dataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex))
                    {
                        dataGrid.Validations.ResetValidations(false);
                        if (HasCurrentCell && CurrentCell.IsEditing)
                        {
#if WinRT
                            CommitCellValue(false);
                            var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == CurrentCellIndex.RowIndex);
                            this.dataGrid.Validations.RemoveError(dataRow, false);
#endif
                            EndEdit(false);
                        }
                        else
                        {
                            if (this.dataGrid.View.IsEditingItem)
                            {
                                dataGrid.View.CancelEdit();
                                ValidationHelper.IsCurrentCellValidated = true;
                                ValidationHelper.IsCurrentRowValidated = true;
#if WPF
                                if (dataGrid.View.IsLegacyDataTable)
                                {
                                    var currentRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsCurrentRow);
                                    if (currentRow != null)
                                    {
                                        foreach (var col in currentRow.VisibleColumns)
                                            col.UpdateBinding(currentRow.RowData);
                                    }
                                }
#endif
                                if (dataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex) && dataGrid.View.IsAddingNew)
                                    dataGrid.GridModel.addNewRowController.CancelAddNew();
                                args.Handled = true;
                            }
                        }
                    }
                    break;
                case Key.Tab:
                    {
                        var previousCellEditStatus = CurrentCell != null ? CurrentCell.IsEditing : false;
                        if (!RaiseValidationAndEndEdit())
                        {
                            args.Handled = true;
                            return false;
                        }

                        rowIndex = CurrentCellIndex.RowIndex;
                        var getPrevious = (CheckShiftKeyPressed() && dataGrid.FlowDirection == FlowDirection.LeftToRight) || (!CheckShiftKeyPressed() && dataGrid.FlowDirection == FlowDirection.RightToLeft);
                        lastPressedKey = Key.Tab;
                        var row = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == CurrentCellIndex.RowIndex);
                        if (getPrevious)
                        {
                            columnIndex = GetPreviousCellIndex();
                            if ((columnIndex == GetFirstCellIndex() && CurrentCellIndex.ColumnIndex == columnIndex) || (row != null && row.RowType != RowType.DefaultRow) || (dataGrid is DetailsViewDataGrid && rowIndex < 0))
                                return false;
                        }
                        else
                        {
                            columnIndex = GetNextCellIndex();
                            if ((columnIndex == GetLastCellIndex() && CurrentCellIndex.ColumnIndex == columnIndex) || (row != null && row.RowType != RowType.DefaultRow) || (dataGrid is DetailsViewDataGrid && rowIndex < 0))
                            {
                                return false;
                            }
                        }
                        var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                        if (!ProcessCurrentCellSelection(rowColumnIndex, ActivationTrigger.Keyboard))
                        {
                            args.Handled = true;
                            return false;
                        }
                        if (previousCellEditStatus && dataGrid.IsAddNewIndex(rowColumnIndex.RowIndex))
                            BeginEdit();
                        if (getPrevious)
                            this.ScrollInViewFromRight(columnIndex);
                        else
                            this.ScrollInViewFromLeft(columnIndex);

                        //ScrollInView(rowColumnIndex);
                        args.Handled = true;
                    }
                    break;
            }
            return args.Handled;
        }

        public virtual void ProcessOnTapped(TappedEventArgs e, RowColumnIndex currentRowColumnIndex)
        {
            if ((dataGrid.EditTrigger == EditTrigger.OnTap && this.AllowFocus(currentRowColumnIndex)))
            {
                if (dataGrid.View.IsEditingItem)
                {
                    if (CurrentCellIndex.RowIndex != currentRowColumnIndex.RowIndex)
                        CheckValidationAndEndEdit(true);
                    else
                        RaiseValidationAndEndEdit();
                }
                this.BeginEdit();
            }
            ScrollInView(currentRowColumnIndex);
        }

        public virtual void ProcessOnDoubleTapped()
        {
            if (CurrentCell != null && CurrentCell.Renderer != null)
            {
                if (CurrentCell.Renderer.HasCurrentCellState && !CurrentCell.IsEditing)
                {
                    this.BeginEdit();
                }
            }
        }
#endif
        public virtual void HandleColumnsCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CurrentCell != null)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        ProcessColumnRemoveAndInsert(args.NewItems.Count > 0 && args.NewItems[0] != null ? (GridColumn)args.NewItems[0] : null, dataGrid.ResolveToScrollColumnIndex(args.NewStartingIndex), NotifyCollectionChangedAction.Add);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ProcessColumnRemoveAndInsert(args.OldItems.Count > 0 && args.OldItems[0] != null ? (GridColumn)args.OldItems[0] : null, dataGrid.ResolveToScrollColumnIndex(args.OldStartingIndex), NotifyCollectionChangedAction.Remove);
                        break;
#if !SILVERLIGHT && !WP
                    case NotifyCollectionChangedAction.Move:
                        {
                            int nextCellIndex = -1;
                            if (dataGrid.ResolveToScrollColumnIndex(args.OldStartingIndex) > CurrentCellIndex.ColumnIndex && dataGrid.ResolveToScrollColumnIndex(args.NewStartingIndex) <= CurrentCellIndex.ColumnIndex)
                                nextCellIndex = CurrentCellIndex.ColumnIndex + 1;
                            else if (dataGrid.ResolveToScrollColumnIndex(args.NewStartingIndex) >= CurrentCellIndex.ColumnIndex && dataGrid.ResolveToScrollColumnIndex(args.OldStartingIndex) < CurrentCellIndex.ColumnIndex)
                                nextCellIndex = CurrentCellIndex.ColumnIndex - 1;
                            else if (((GridColumn)args.OldItems[0]).MappingName == CurrentCell.GridColumn.MappingName)
                            {
                                nextCellIndex = dataGrid.ResolveToScrollColumnIndex(args.NewStartingIndex);
                                RemoveCurrentCellSelection(CurrentCellIndex);
                                SelectCurrentCell(new RowColumnIndex(CurrentCellIndex.RowIndex, nextCellIndex));
                            }
                            if (nextCellIndex != -1)
                                SetCurrentColumnIndex(nextCellIndex);
                        }
                        break;
#endif
                }
            }
        }

        #endregion
#if !WP
        #region Editing Methods

        /// <summary>
        /// Triggers BeginEdit, and causes the CellRenderer to load its actual Renderer for Editing
        /// </summary>
        /// <remarks></remarks>
        public bool BeginEdit()
        {
            if (!HasCurrentCell) return false;
            var currentCell = CurrentCell.ColumnElement as GridCell;
            if ((currentCell == null) || (currentCell is GridCaptionSummaryCell) || (currentCell is GridGroupSummaryCell) || (currentCell is GridTableSummaryCell) || (CurrentCell.GridColumn is GridTemplateColumn && ((GridTemplateColumn)CurrentCell.GridColumn).EditTemplate == null))
                return false;

            if (CurrentCell.GridColumn != null && (CurrentCell.GridColumn.AllowEditing || dataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex)))
            {
                if (CurrentCell.Renderer != null && CurrentCell.Renderer.IsEditable)
                {
                    if (dataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex) && !dataGrid.View.IsAddingNew)
                    {
                        if (!dataGrid.GridModel.addNewRowController.AddNew())
                            return false;
                    }

                    var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == CurrentCellIndex.RowIndex);
                    if (dataRow == null)
                        return false;

                    if (ValidationHelper.IsCurrentCellValidated && !RaiseCurrentCellBeginEditEvent(CurrentCellIndex, CurrentCell.GridColumn))
                    {
                        var isinedit = CurrentCell.Renderer.BeginEdit(CurrentCellIndex, currentCell, CurrentCell.GridColumn, currentCell.DataContext);
                        if (isinedit)
                        {
                            CurrentCell.IsEditing = isinedit;
                            dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == CurrentCellIndex.RowIndex);
                            if (dataRow != null)
                            {
                                if (!dataGrid.View.IsEditingItem || !dataGrid.View.CurrentEditItem.Equals(dataRow.RowData))
                                    dataGrid.View.EditItem(dataRow.RowData);
                                dataRow.IsEditing = isinedit;
                                if (!(CurrentCell.Renderer is GridCellTemplateRenderer || CurrentCell.Renderer is GridUnBoundCellRenderer || CurrentCell.Renderer is GridCellMultiColumnDropDownRenderer))
                                {
                                    ValidationHelper.IsCurrentCellValidated = !isinedit;
                                    ValidationHelper.IsCurrentRowValidated = !isinedit;
                                }
                                (dataRow as GridDataRow).ApplyRowHeaderVisualState();
                            }
                        }
                        return isinedit;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ends Edit for the corresponding Renderer and loads TextBlocl
        /// </summary>
        /// <param name="previousRowColumnIndex">RowColumnIndex for the Cell to End Edit</param>
        /// <remarks></remarks>
        public bool EndEdit(bool canCommit = true)
        {
            var datarow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsEditing);

            if (!HasCurrentCell) return true;
            var currentCell = CurrentCell.ColumnElement as GridCell;
            if ((currentCell == null) || (currentCell is GridCaptionSummaryCell) || (currentCell is GridGroupSummaryCell) || (currentCell is GridTableSummaryCell))
                return true;
            if (CurrentCell.Renderer != null && CurrentCell.Renderer.IsEditable)
            {
                if (CurrentCell.IsEditing)
                {
                    var isinedit = CurrentCell.Renderer.EndEdit(CurrentCellIndex, currentCell, CurrentCell.GridColumn, currentCell.DataContext);
                    if (isinedit)
                    {
                        if (canCommit && dataGrid.View.IsEditingItem)
                            dataGrid.View.CommitEdit();
                        datarow.IsEditing = !isinedit;
                        if (CurrentCell != null)
                            CurrentCell.IsEditing = !isinedit;
                        RaiseCurrentCellEndEditEvent(CurrentCellIndex);
                        (datarow as GridDataRow).ApplyRowHeaderVisualState();
                    }
                }
                else
                {
                    if (canCommit && dataGrid.View.IsEditingItem)
                        dataGrid.View.CommitEdit();
                }
            }
            else
            {
                if (canCommit && dataGrid.View.IsEditingItem)
                    dataGrid.View.CommitEdit();
            }
            return true;
        }

        #endregion

#endif
        #endregion

        #region Private Methods

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

        private void ProcessColumnRemoveAndInsert(GridColumn changedColumn, int changedIndex, NotifyCollectionChangedAction action)
        {
            int nextCellIndex = -1;
            if (changedColumn != null && CurrentCell.GridColumn != null && changedColumn.MappingName == CurrentCell.GridColumn.MappingName)
            {
                if (action == NotifyCollectionChangedAction.Remove)
                {
                    RemoveCurrentCellSelection(CurrentCellIndex);
                    var nextColumn = this.GetNextFocusGridColumn(0, MoveDirection.Right);
                    if (nextColumn != null)
                    {
                        nextCellIndex = dataGrid.ResolveToScrollColumnIndex(dataGrid.Columns.IndexOf(nextColumn));
                        SelectCurrentCell(new RowColumnIndex(CurrentCellIndex.RowIndex, nextCellIndex));
                    }
                }
#if !WinRT
                else if (action == NotifyCollectionChangedAction.Add)
                {
                    nextCellIndex = dataGrid.ResolveToScrollColumnIndex(changedIndex);
                    RemoveCurrentCellSelection(CurrentCellIndex);
                    SelectCurrentCell(new RowColumnIndex(CurrentCellIndex.RowIndex, nextCellIndex));
                }
#endif
            }
            else
            {
                if (changedIndex <= CurrentCell.ColumnIndex)
                    nextCellIndex = action == NotifyCollectionChangedAction.Add ? CurrentCellIndex.ColumnIndex + 1 : CurrentCellIndex.ColumnIndex - 1;
            }
            if (nextCellIndex != -1)
                this.SetCurrentColumnIndex(nextCellIndex);
        }

#if WinRT
        private ActivationTrigger ConvertPointerDeviceTypeToActivationTrigger(PointerDeviceType pointerDeviceType)
        {
            var activationTrigger = ActivationTrigger.Mouse;
            if (pointerDeviceType == PointerDeviceType.Pen)
                activationTrigger = ActivationTrigger.Pen;
            else if (pointerDeviceType == PointerDeviceType.Touch)
                activationTrigger = ActivationTrigger.Touch;
            return activationTrigger;
        }
#endif

        #endregion

        #region Protected Methods

        protected internal void RemoveCurrentCellSelection(RowColumnIndex rowColumnIndex)
        {
            if (CurrentCell != null)
            {
                CurrentCell.IsSelectedColumn = false;
                if (CurrentCell.Renderer != null)
                {
                    if (CurrentCell.IsEditing)
                    {
                        CurrentCell.Renderer.EndEdit(rowColumnIndex, CurrentCell.ColumnElement, CurrentCell.GridColumn,
                                                    CurrentCell.ColumnElement.DataContext);
#if !WP
                        dataGrid.Validations.ResetValidations(true);
#endif
                        dataGrid.View.CommitEdit();
                        CurrentCell.IsEditing = false;
                        var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsEditing);
                        dataRow.IsEditing = false;
                        oldCellValue = null;
                    }
                    CurrentCell.Renderer.ResetCurrentCellState();
                }
                CurrentCell = null;
            }
        }

        protected internal void ScrollInView(RowColumnIndex rowColumnIndex)
        {
            if (rowColumnIndex.RowIndex < 0)
                return;
            var visibleRowLines = this.dataGrid.VisualContainer.ScrollRows.GetVisibleLines();
            var rowLineInfo = this.dataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(rowColumnIndex.RowIndex);
            bool isScrolled = false;

            if (visibleRowLines.FirstBodyVisibleIndex < visibleRowLines.Count)
            {
                if (rowColumnIndex.RowIndex < visibleRowLines[visibleRowLines.FirstBodyVisibleIndex].LineIndex || rowColumnIndex.RowIndex > visibleRowLines[visibleRowLines.LastBodyVisibleIndex].LineIndex || (rowLineInfo != null && rowLineInfo.IsClipped))
                {
                    this.dataGrid.VisualContainer.ScrollRows.ScrollInView(rowColumnIndex.RowIndex);
                    isScrolled = true;
                }
            }

            if (rowColumnIndex.ColumnIndex < 0)
                return;

            var visibleColumnLines = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            var columnLineInfo = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(rowColumnIndex.ColumnIndex);

            if (visibleColumnLines.FirstBodyVisibleIndex < visibleColumnLines.Count)
            {
                if (rowColumnIndex.ColumnIndex < visibleColumnLines[visibleColumnLines.FirstBodyVisibleIndex].LineIndex || rowColumnIndex.ColumnIndex > visibleColumnLines[visibleColumnLines.LastBodyVisibleIndex].LineIndex || (columnLineInfo != null && columnLineInfo.IsClipped))
                {
                    this.dataGrid.VisualContainer.ScrollColumns.ScrollInView(rowColumnIndex.ColumnIndex);
                    isScrolled = true;
                }
            }

            if (isScrolled)
                this.dataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        protected internal void SelectCurrentCell(RowColumnIndex rowColumnIndex)
        {
            var dataColumn = GetDataColumnBase(rowColumnIndex);
            if (dataColumn != null && dataColumn is DataColumn)
            {
                dataColumn.IsSelectedColumn = true;
                if (dataColumn.Renderer != null)
                {
                    dataColumn.Renderer.SetCurrentCellState(rowColumnIndex, dataColumn.ColumnElement, dataColumn.IsEditing, isCollectionChanged ? false : true);
                    var dataRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowColumnIndex.RowIndex);
                    //Get the value based on ValueBinding
                    if (dataRow != null && dataColumn.GridColumn.MappingName != null)
                        oldCellValue = this.dataGrid.View.GetPropertyAccessProvider().GetValue(dataRow.RowData, dataColumn.GridColumn.MappingName, true);
                    else
                        oldCellValue = dataColumn.Renderer.GetControlValue();
                    isCollectionChanged = false;
                }
                this.CurrentCell = dataColumn;
            }
            SetCurrentColumnIndex(rowColumnIndex.ColumnIndex);
        }

#if !WP
        /// <summary>
        /// Method which call the Invalidate measure and Scroll in view if the column is not present in Visible region.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <remarks></remarks>
        protected internal void ScrollInViewFromRight(int columnIndex)
        {
            VisibleLineInfo lineInfo = dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);

            //Here we checking whether the column is not in visible or clipped
            if (columnIndex > dataGrid.VisualContainer.ScrollColumns.LastBodyVisibleLineIndex || lineInfo == null || (lineInfo != null && lineInfo.IsClipped))
            {
                dataGrid.VisualContainer.ScrollColumns.ScrollInView(columnIndex);
                lineInfo = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
                if (lineInfo != null && dataGrid is DetailsViewDataGrid && dataGrid.NotifyListener != null)
                    DetailsViewManager.ScrollInViewAllDetailsViewParent(dataGrid, lineInfo);
                dataGrid.VisualContainer.InvalidateMeasureInfo();
            }
        }

        /// <summary>
        /// Method which call the Invalidate measure and Scroll in view if the column is not present in Visible region.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <remarks></remarks>
        protected internal void ScrollInViewFromLeft(int columnIndex)
        {
            VisibleLinesCollection lineCollection = dataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            VisibleLineInfo lineInfo = dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);

            //Here we checking whether the column is not in visible or clipped
            if (columnIndex >= 0 && columnIndex < lineCollection[lineCollection.firstBodyVisibleIndex].LineIndex || lineInfo == null || (lineInfo != null && lineInfo.IsClipped))
            {
                if (columnIndex == GetFirstCellIndex() && this.dataGrid.DetailsViewManager.HasDetailsView)
                    columnIndex -= 1;
                dataGrid.VisualContainer.ScrollColumns.ScrollInView(columnIndex);
                lineInfo = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
                if (lineInfo != null && dataGrid is DetailsViewDataGrid && dataGrid.NotifyListener != null)
                    DetailsViewManager.ScrollInViewAllDetailsViewParent(dataGrid, lineInfo);
                dataGrid.VisualContainer.InvalidateMeasureInfo();
            }
        }

        protected bool AllowFocus(RowColumnIndex rowColumnIndex)
        {
            var dataColumn = GetDataColumnBase(rowColumnIndex);
            if (dataColumn != null && dataColumn.GridColumn != null)
                return (dataColumn.GridColumn.AllowFocus && !dataColumn.GridColumn.IsHidden);
            return true;
        }

        protected bool AllowCurrentCellSelection(RowColumnIndex currentCellIndex)
        {
            var dataColumn = GetDataColumnBase(currentCellIndex);
            if (dataColumn != null && dataColumn.Renderer != null && dataColumn.Renderer.IsEditable)
                return true;
            return false;
        }

#endif



        protected internal int GetLastCellIndex()
        {
#if !WP
            int lastIndex = dataGrid.Columns.IndexOf(dataGrid.Columns.LastOrDefault(col => (col.AllowFocus && !col.IsHidden && col.ActualWidth != 0d)));
            lastIndex += dataGrid.View.GroupDescriptions.Count;
            if (dataGrid.DetailsViewManager.HasDetailsView)
                lastIndex += 1;
#else
            int lastIndex = dataGrid.Columns.IndexOf(dataGrid.Columns.LastOrDefault(col => (!col.IsHidden && col.ActualWidth != 0d)));
#endif
            if (dataGrid.ShowRowHeader)
                lastIndex += 1;
            return lastIndex;
        }

        protected internal int GetNextCellIndex()
        {
            int nextCellIndex = CurrentCellIndex.ColumnIndex;
            int lastCellIndex = this.GetLastCellIndex();

            GridColumn column = this.GetNextFocusGridColumn(nextCellIndex + 1, MoveDirection.Right);
            if (column != null)
            {
                nextCellIndex = dataGrid.ResolveToScrollColumnIndex(dataGrid.Columns.IndexOf(column));
            }

            nextCellIndex = (nextCellIndex > lastCellIndex) ? lastCellIndex : nextCellIndex;

            return nextCellIndex;
        }

        protected internal int GetPreviousCellIndex()
        {
            int previousCellIndex = CurrentCellIndex.ColumnIndex;

            GridColumn column = GetNextFocusGridColumn(previousCellIndex - 1, MoveDirection.Left);
            if (column != null)
            {
                previousCellIndex = dataGrid.ResolveToScrollColumnIndex(dataGrid.Columns.IndexOf(column));
            }

            previousCellIndex = previousCellIndex < dataGrid.View.GroupDescriptions.Count
                                        ? CurrentCellIndex.ColumnIndex
                                        : previousCellIndex;

            return previousCellIndex;
        }

        protected internal GridColumn GetNextFocusGridColumn(int columnIndex, MoveDirection direction)
        {
            var resolvedIndex = dataGrid.ResolveToGridVisibleColumnIndex(columnIndex);
            if (resolvedIndex < 0 || resolvedIndex >= dataGrid.Columns.Count)
                return null;

            var gridColumn = dataGrid.Columns[resolvedIndex];

#if !WP
            if (gridColumn == null || !gridColumn.AllowFocus || gridColumn.ActualWidth == 0.0 || gridColumn.IsHidden)
#else
            if (gridColumn == null || gridColumn.ActualWidth == 0.0||gridColumn.IsHidden)
#endif
            {
                gridColumn = this.GetNextFocusGridColumn(direction == MoveDirection.Right ? columnIndex + 1 : columnIndex - 1, direction);
            }
            return gridColumn;
        }

        protected internal int GetFirstCellIndex()
        {
#if !WP
            int firstColumnIndex = dataGrid.Columns.IndexOf(dataGrid.Columns.FirstOrDefault(col => (col.AllowFocus && !col.IsHidden && col.ActualWidth != 0d)));

            if (dataGrid.DetailsViewManager.HasDetailsView)
                firstColumnIndex += 1;
#else
            int firstColumnIndex = dataGrid.Columns.IndexOf(dataGrid.Columns.FirstOrDefault(col => ( !col.IsHidden && col.ActualWidth != 0d)));
#endif
            firstColumnIndex += dataGrid.View.GroupDescriptions.Count;
            if (dataGrid.ShowRowHeader)
                firstColumnIndex += 1;
            return firstColumnIndex;
        }

#if !WP

        #region VirtualMethods

        protected internal virtual bool ProcessCurrentCellSelection(RowColumnIndex newRowColumnIndex, ActivationTrigger activationTriggger)
        {
            if (newRowColumnIndex.ColumnIndex < GetFirstCellIndex())
            {
                var nextFocusGridColumn = GetNextFocusGridColumn(GetFirstCellIndex(), MoveDirection.Right);
                var nextCellIndex = CurrentCellIndex.ColumnIndex < 0 ? dataGrid.ResolveToScrollColumnIndex(dataGrid.Columns.IndexOf(nextFocusGridColumn)) : CurrentCellIndex.ColumnIndex;
                newRowColumnIndex.ColumnIndex = nextCellIndex;

                if (newRowColumnIndex.RowIndex == CurrentCellIndex.RowIndex && newRowColumnIndex.ColumnIndex == CurrentCellIndex.ColumnIndex && this.dataGrid.SelectionMode != GridSelectionMode.Multiple)
                    return false;
            }
            previousCellIndex = CurrentCellIndex;
            bool isRecordCell = AllowCurrentCellSelection(newRowColumnIndex);
            if (isRecordCell && RaiseCurrentCellActivatingEvent(CurrentCellIndex, newRowColumnIndex, activationTriggger))
                return false;
            var currentRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.IsCurrentRow);
            if (currentRow != null)
            {
                currentRow.IsCurrentRow = false;
                (currentRow as GridDataRow).ApplyRowHeaderVisualState();
            }
            RemoveCurrentCellSelection(CurrentCellIndex);
            SelectCurrentCell(newRowColumnIndex);
            currentRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == newRowColumnIndex.RowIndex);
            if (currentRow != null)
            {
                currentRow.IsCurrentRow = true;
                (currentRow as GridDataRow).ApplyRowHeaderVisualState();
            }
            if (isRecordCell)
                RaiseCurrentCellActivatedEvent(newRowColumnIndex, previousCellIndex, activationTriggger);
            return true;
        }

        #endregion
#endif
        #endregion
#if !WP
        #region Events & Event Raising Methods

        protected bool RaiseCurrentCellActivatingEvent(RowColumnIndex previousRowColumnIndex, RowColumnIndex currentRowColumnIndex, ActivationTrigger activationTrigger)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return false;

            if (previousRowColumnIndex.RowIndex < 0 || previousRowColumnIndex.ColumnIndex < 0)
                previousRowColumnIndex = new RowColumnIndex(0, 0);

            var args = new CurrentCellActivatingEventArgs(dataGrid)
            {
                PreviousRowColumnIndex = previousRowColumnIndex,
                CurrentRowColumnIndex = currentRowColumnIndex,
                ActivationTrigger = activationTrigger
            };
            return dataGrid.RaiseCurrentCellActivatingEvent(args);
        }

        protected void RaiseCurrentCellActivatedEvent(RowColumnIndex rowColumnIndex, RowColumnIndex previousRowColumnIndex, ActivationTrigger activationTrigger)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return;

            if (previousRowColumnIndex.RowIndex < 0 || previousRowColumnIndex.ColumnIndex < 0)
                previousRowColumnIndex = new RowColumnIndex(0, 0);

            var e = new CurrentCellActivatedEventArgs(dataGrid)
            {
                CurrentRowColumnIndex = rowColumnIndex,
                PreviousRowColumnIndex = previousRowColumnIndex,
                ActivationTrigger = activationTrigger
            };
            dataGrid.RaiseCurrentCellActivatedEvent(e);
        }

        protected void RaiseCurrentCellEndEditEvent(RowColumnIndex rowColumnIndex)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return;

            var args = new CurrentCellEndEditEventArgs(dataGrid)
            {
                RowColumnIndex = rowColumnIndex
            };
            dataGrid.RaiseCurrentCellEndEditEvent(args);
        }

        protected bool RaiseCurrentCellBeginEditEvent(RowColumnIndex rowColumnIndex, GridColumn gridColumn)
        {
            if (!ValidationHelper.IsCurrentCellValidated)
                return false;

            var args = new CurrentCellBeginEditEventArgs(dataGrid)
            {
                RowColumnIndex = rowColumnIndex,
                Column = gridColumn
            };
            return dataGrid.RaiseCurrentCellBeginEditEvent(args);
        }

        #endregion
#endif
    }
}
