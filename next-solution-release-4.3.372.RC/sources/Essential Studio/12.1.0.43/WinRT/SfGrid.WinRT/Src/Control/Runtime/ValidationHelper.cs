#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !WP
using System.ComponentModel.DataAnnotations;
#endif
using System.Reflection;
using System.Windows;
#if WinRT
using Windows.UI.Xaml;
using Syncfusion.Data;
#else
using System.Windows.Controls;
using System.Windows.Data;
using Syncfusion.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class ValidationHelper : IDisposable
    {
        #region fields

        private static bool isCurrentCellValidated = true;
        private static bool isCurrentRowValidated = true;
        private SfDataGrid dataGrid;
        private static bool isFocusSetBack = false;
#if !WP
        private Dictionary<string, string> errorMessages;
        int rowIndex = -1;
#endif
        #endregion

        #region Ctor

        public ValidationHelper(SfDataGrid datagrid)
        {
            dataGrid = datagrid;
        }

        #endregion

        #region Properties

        public static bool IsCurrentCellValidated
        {
            get { return isCurrentCellValidated; }
            internal set { isCurrentCellValidated = value; }
        }

        public static bool IsCurrentRowValidated
        {
            get { return isCurrentRowValidated; }
            internal set { isCurrentRowValidated = value; }
        }

        public static bool IsFocusSetBack
        {
            get { return isFocusSetBack; }
            internal set { isFocusSetBack = value; }
        }
        #endregion

        #region Methods
#if !WP

        internal void ResetValidations(bool canResetRow)
        {
            ResetValidation(dataGrid, canResetRow);
        }

        private void ResetValidation(SfDataGrid grid, bool canResetRow)
        {
            var rowColumnIndex = grid.SelectionController.CurrentCellManager.CurrentCellIndex;
            var dataRow = grid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowColumnIndex.RowIndex);
            if (dataRow != null)
            {
                if (dataRow is DetailsViewDataRow)
                {
                    ResetValidation((dataRow as DetailsViewDataRow).DetailsViewDataGrid, canResetRow);
                    return;
                }
                var dataColumn = dataRow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == rowColumnIndex.ColumnIndex);
                if (dataColumn != null && dataColumn.ColumnElement != null)
                {
                    var cell = (dataColumn.ColumnElement as GridCell);
                    cell.eventErrorMessage = string.Empty;
                    IsCurrentCellValidated = true;
                    if (canResetRow)
                        isCurrentRowValidated = true;
                    cell.ApplyValidationVisualState();
                }
            }
        }


        internal bool RaiseRowValidate(RowColumnIndex currentCellIndex)
        {
            if (IsCurrentRowValidated)
                return true;

            var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == currentCellIndex.RowIndex);
            if (dataRow == null) return true;
            errorMessages = new Dictionary<string, string>();
            rowIndex = dataRow.RowIndex;
            var validatingArgs = new RowValidatingEventArgs(dataRow.WholeRowElement.DataContext, currentCellIndex.RowIndex, errorMessages);

            if (!dataGrid.RaiseRowValidatingEvent(validatingArgs))
            {
                foreach (var item in validatingArgs.ErrorMessages)
                {
                    var columnBase = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn!= null && x.GridColumn.MappingName.Equals(item.Key));
                    if (columnBase != null && columnBase.ColumnElement is GridCell)
                    {
                        var rowColIndex = new RowColumnIndex(dataRow.RowIndex, columnBase.ColumnIndex);
                        var currentCell = ((GridCell)columnBase.ColumnElement);
                        if (currentCell != null)
                            currentCell.SetError(item.Value, false);
                    }
                }

                dataRow.WholeRowElement.SetError();
                if (!dataRow.IsEditing)
                    dataGrid.SelectionController.CurrentCellManager.BeginEdit();
                IsCurrentCellValidated = false;
                return false;
            }
            else
            {
                RemoveError(dataRow, false);
            }

            var args = new RowValidatedEventArgs(validatingArgs.RowData, validatingArgs.RowIndex, validatingArgs.ErrorMessages);
            dataGrid.RaiseRowValidatedEvent(args);
            IsCurrentRowValidated = true;
            //this.dataGrid.VisualContainer.SuspendManipulationScroll = false;
            errorMessages = null;
            rowIndex = -1;
            return true;
        }

        internal void RemoveError(DataRowBase dataRow, bool removeAll)
        {
            if (dataRow == null)
                return;
            foreach (DataColumnBase column in dataRow.VisibleColumns)
            {
                if (column.ColumnElement is GridCell)
                {
                    if (removeAll)
                        (column.ColumnElement as GridCell).RemoveAll();
                    else
                        (column.ColumnElement as GridCell).RemoveError(false);
                }
            }
            dataRow.WholeRowElement.RemoveError();
        }

        internal bool RaiseCellValidate(RowColumnIndex currentCellIndex, IGridCellRenderer renderer, bool allowattributeValidation)
        {
            if (IsCurrentCellValidated)
                return true;

             var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == currentCellIndex.RowIndex);
            if (dataRow == null) 
                return false;
            var columnBase = dataRow.VisibleColumns.FirstOrDefault(x => x.ColumnIndex == currentCellIndex.ColumnIndex);
            if(columnBase == null)
                return false;
            renderer = renderer == null ? columnBase.Renderer : renderer;

            if (renderer == null)
                return false;
            if (IsFocusSetBack)
                return false;

            object oldValue = null;
            object changedNewValue;
            string errorMessage;
            if(this.dataGrid.SelectionController is GridSelectionController)
                oldValue = (this.dataGrid.SelectionController as GridSelectionController).CurrentCellManager.oldCellValue;
            var newCellValue = renderer.GetControlValue();
            if (this.RaiseCurrentCellValidatingEvent(oldValue, newCellValue , columnBase.GridColumn,out changedNewValue, currentCellIndex,columnBase.ColumnElement as UIElement,out errorMessage,dataRow.RowData))
            {
                bool isValid = true;
                if (newCellValue != changedNewValue)
                    renderer.SetControlValue(changedNewValue);
                if (allowattributeValidation)
                {
#if !WinRT
                    if ((columnBase.ColumnElement is GridCell) && (columnBase.ColumnElement as GridCell).Content != null && (columnBase.ColumnElement as GridCell).Content is UIElement)
                        renderer.UpdateSource((columnBase.ColumnElement as GridCell).Content as UIElement);
#endif
                    if (this.dataGrid.GridValidationMode != GridValidationMode.None)
                        isValid = this.dataGrid.Validations.ValidateColumn(dataRow.RowData, columnBase.GridColumn.MappingName, (columnBase.ColumnElement as GridCell), currentCellIndex)
                            && DataValidation.Validate((columnBase.ColumnElement as GridCell), columnBase.GridColumn.MappingName, columnBase.ColumnElement.DataContext);
#if !WinRT
                    if (!isValid && this.dataGrid.GridValidationMode == GridValidationMode.InEdit)
                        return false;
#endif
                }
                this.RaiseCurrentCellValidatedEvent(oldValue, columnBase.Renderer.GetControlValue(), columnBase.GridColumn, errorMessage, dataRow.RowData);
                IsCurrentCellValidated = true;
                return true;
            }
            return false;
        }

        internal bool RaiseCellValidate(RowColumnIndex currentCellIndex)
        {
            return this.RaiseCellValidate(currentCellIndex, null, true);
        }


        internal bool RaiseCurrentCellValidatingEvent(object oldValue, object newValue, GridColumn column, out object changedNewValue, RowColumnIndex currentCellIndex, UIElement currentCell, out string errorMessage, object rowData)
        {
            bool isValid;
            var e = new CurrentCellValidatingEventArgs(dataGrid)
            {
                OldValue = oldValue,
                NewValue = newValue,
                Column = column,
                IsValid = true,
                RowData = rowData
            };
            isValid = dataGrid.RaiseCurrentCellValidatingEvent(e);
            changedNewValue = e.NewValue;

            if (currentCell is GridCell)
            {
                var cell = currentCell as GridCell;
                if (!isValid)
                    cell.SetError(e.ErrorMessage, false);
                else if (errorMessages == null || !(errorMessages.Keys.Any(x => x == column.MappingName)))
                    cell.RemoveError(false);
            }

            errorMessage = e.ErrorMessage;
            return isValid;
        }

        internal void RaiseCurrentCellValidatedEvent(object oldValue, object newValue, GridColumn column, string errorMessage, object rowData)
        {

            var e = new CurrentCellValidatedEventArgs(dataGrid)
            {
                OldValue = oldValue,
                NewValue = newValue,
                Column = column,
                ErrorMessage = errorMessage,
                RowData = rowData
            };
            dataGrid.RaiseCurrentCellValidatedEvent(e);
        }

        internal bool ValidateColumn(object rowData, string columnName, GridCell currentCell, RowColumnIndex currentCellIndex)
        {
            bool isValid = true;
            var errorMessage = string.Empty;
            bool isAttributeError = false;
            if (rowData == null || string.IsNullOrEmpty(columnName) || currentCell == null || currentCellIndex == RowColumnIndex.Empty)
                return isValid;
            var itemproperties = this.dataGrid.View.GetItemProperties();
            if (columnName.Contains("."))
            {
                string[] propertyNameList = columnName.Split('.');
                int complexPropertyCount = propertyNameList.Count();
                for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                {
                    var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                    if (tempProperyDescriptor != null)
                    {
                        rowData = tempProperyDescriptor.GetValue(rowData);
#if WPF
                        itemproperties = TypeDescriptor.GetProperties(rowData);
#else
                        itemproperties = new PropertyInfoCollection(rowData.GetType());
#endif
                    }
                    columnName = propertyNameList[complexPropertyCount - 1];
                }
            }
#if !WinRT
            var propertyinfo = rowData.GetType().GetProperty(columnName);
            var validationContext = new ValidationContext(rowData, null, null) { MemberName = columnName };
#else
            
           
            
            var propertyinfo = rowData.GetType().GetRuntimeProperties().FirstOrDefault(x => x.Name == columnName);
            
            var validationContext = new ValidationContext(rowData) { MemberName = columnName };
#endif

            if (errorMessages != null && rowIndex == currentCellIndex.RowIndex && errorMessages.Keys.Contains(columnName))
                errorMessage = errorMessages[columnName];

            if (propertyinfo != null && this.dataGrid.GridValidationMode != GridValidationMode.None)
            {
                var validationAttribute = propertyinfo.GetCustomAttributes(false).OfType<ValidationAttribute>();
                var value = propertyinfo.GetValue(rowData, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                if (!Validator.TryValidateValue(value, validationContext, results, validationAttribute))
                {
                    foreach (var result in results)
                        errorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage + string.Format("\n") + result.ErrorMessage : errorMessage + result.ErrorMessage;
                                        isValid = false;
                    isAttributeError = true;
                }
            }

            if (currentCell != null)
            {
                if (!isValid || !string.IsNullOrEmpty(errorMessage))
                    currentCell.SetError(errorMessage, isAttributeError);
                else 
                    currentCell.RemoveError(true);
            }
#if SyncfusionFramework4_5
            if (rowData is INotifyDataErrorInfo)
                isValid = DataValidation.ValidateINotifyDataErrorInfo(currentCell, columnName, rowData);
#endif
            return isValid;
        }

        

        internal void ValidateColumns(DataRowBase dr)
        {
            foreach (var column in dr.VisibleColumns)
                if (column.GridColumn != null)
                    this.ValidateColumn(dr.RowData, column.GridColumn.MappingName, column.ColumnElement as GridCell, new RowColumnIndex(column.RowIndex, column.ColumnIndex));
        }
#endif
        internal bool CheckForValidation(bool canRaiseEvent) 
        {
            if (!canRaiseEvent)
                return IsCurrentCellValidated && IsCurrentRowValidated;
#if !WP
            if (!IsCurrentCellValidated || !IsCurrentRowValidated)
                if (this.RaiseCellValidate(dataGrid.SelectionController.CurrentCellManager.CurrentCellIndex))
                    return this.RaiseRowValidate(dataGrid.SelectionController.CurrentCellManager.CurrentCellIndex);
                else
                    return false;

#endif
            return true;
        }



        #endregion

        public void Dispose()
        {
#if !WP
            if (errorMessages != null)
            {
                errorMessages.Clear();
                errorMessages = null;
            }
#endif
            dataGrid = null;
        }
    }
}
