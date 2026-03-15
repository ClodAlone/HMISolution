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
using Syncfusion.Data.Extensions;
using Syncfusion.Data;
#if !SILVERLIGHT && !WP
using System.Threading.Tasks;
#endif
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Collections;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridAddNewRowController: IDisposable
    {
        #region Fields

        SfDataGrid dataGrid;

        #endregion

        #region Ctor

        public GridAddNewRowController(SfDataGrid grid)
        {
            dataGrid = grid;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Method which initiates the CurrentAddItem through View.AddNew() method.
        /// Method will called when the cell begin editing.
        /// </summary>
        /// <returns></returns>
        internal bool AddNew()
        {
            if (!dataGrid.View.CanAddNew)
                return false;
            if (dataGrid.View.IsAddingNew)
                throw new InvalidOperationException("CurrentAddItem should be null when AddNew calls");
            if (dataGrid.AddNewRowPosition != AddNewRowPosition.None)
            {
                dataGrid.View.AddNew();
                var addNewRowEventArgs = new AddNewRowInitiatingEventArgs(this.dataGrid) { NewObject = dataGrid.View.CurrentAddItem };
                var addNewObject = dataGrid.RaiseAddNewRowInitiatingEvent(addNewRowEventArgs);
                var addNewRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
                if (addNewRow != null)
                {
                    VisualStateManager.GoToState(addNewRow.WholeRowElement, "Edit", true);
                    addNewRow.RowData = addNewObject;
                }
            }
            dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.AddNewRow, new AddNewRowOperationHandle(AddNewRowOperation.AddNew,null)));
            return true;
        }

        /// <summary>
        /// Method which helps to Cancell the CurrentAddItem, adding to the sourcecollection.
        /// Method will be called when press esc key twice.
        /// </summary>
        internal void CancelAddNew()
        {
            dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.AddNewRow, new AddNewRowOperationHandle(AddNewRowOperation.CancelNew, null)));
            this.dataGrid.View.CancelNew();
            this.dataGrid.Validations.ResetValidations(true);
            ResetAddNewRow(false);
        }

        /// <summary>
        /// Method which helps to Commits the CurrentAddItem to the sourcecollection.
        /// </summary>
        internal void CommitAddNew(bool changeState = true)
        {
            dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.AddNewRow, new AddNewRowOperationHandle(AddNewRowOperation.CommitNew, null)));
            if (this.dataGrid.View.IsAddingNew)
                this.dataGrid.View.CommitNew();
            if (this.dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                dataGrid.ScrollInView(new RowColumnIndex(GetAddNewRowIndex(), 0));
            ResetAddNewRow(changeState);
        }

        /// <summary>
        /// Method which helps to Set the mode for AddNewRow water mark.
        /// </summary>
        /// <param name="inEdit"></param>
        internal void SetAddNewMode(bool inEdit)
        {
            var addNewRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
            if (addNewRow != null)
            {
                if (inEdit)
                    VisualStateManager.GoToState(addNewRow.WholeRowElement, "Edit", true);
                else
                    VisualStateManager.GoToState(addNewRow.WholeRowElement, "Normal", true);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method which helps to reset the AddNewRow after commiting or canceling.
        /// </summary>
        private void ResetAddNewRow(bool changeState)
        {
            var addNewRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
            if (addNewRow != null)
            {
                addNewRow.RowData = this.dataGrid.View.CurrentAddItem;
                (addNewRow.WholeRowElement as AddNewRowControl).UpdateTextBorder();
                (addNewRow as GridDataRow).ApplyRowHeaderVisualState();
                this.dataGrid.Validations.RemoveError(addNewRow, true);
                if (changeState)
                    VisualStateManager.GoToState(addNewRow.WholeRowElement, "Normal", true);
            }
        }

        /// <summary>
        /// Method which helps to get the AddNewROw index in DataGrid.
        /// </summary>
        /// <returns></returns>
        internal int GetAddNewRowIndex()
        {
            if (dataGrid.AddNewRowPosition == AddNewRowPosition.None)
                return -1;
            else if (dataGrid.AddNewRowPosition == AddNewRowPosition.Top)
                return dataGrid.HeaderLineCount - 1;
            else
                return dataGrid.VisualContainer.RowCount - (dataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom) + 1);
        }

        #endregion

        public void Dispose()
        {
            this.dataGrid = null;
        }
    }

    /// <summary>
    /// Class which used as argument in SelectionController when performing AddNewRow operation.
    /// </summary>
    public class AddNewRowOperationHandle
    {
        #region Ctor

        public AddNewRowOperationHandle(AddNewRowOperation operation, object operationArgs)
        {
            AddNewRowOperation = operation;
            OperationArgs = operationArgs;
        }

        #endregion

        /// <summary>
        /// Get the AddNewRow position.
        /// </summary>
        public AddNewRowOperation AddNewRowOperation { get; private set; }

        /// <summary>
        /// Get the event argument regarding AddNewRow operation.
        /// </summary>
        public object OperationArgs { get; private set; }
    }

    /// <summary>
    /// Enum which descripes the Operation done in AddNewRow
    /// </summary>
    public enum AddNewRowOperation
    {
        AddNew,
        CancelNew,
        CommitNew,
        PlacementChange
    }
}
