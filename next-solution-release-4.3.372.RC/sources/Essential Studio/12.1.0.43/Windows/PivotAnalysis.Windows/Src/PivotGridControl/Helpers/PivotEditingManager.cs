#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.PivotAnalysis;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.PivotAnalysis.Base;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class PivotEditingManager : IDisposable
    {
        PivotGridControlBase tableControl = null;

        bool isWired = false;

        #region Properties
        /// <summary>
        /// Gets or Set whether the expanders are visible.
        /// </summary>
        public bool HideExpanders { get; set; }

        /// <summary>
        /// Gets or sets whether the user can edit total cells.
        /// </summary>
        public bool AllowEditingOfTotalCells { get; set; }

        #endregion

        /// <summary>
        /// Constructor. Intializes a new PivotEditingManager instance
        /// </summary>
        /// <param name="tableControl">PivotGridControlBase</param>
        public PivotEditingManager(PivotGridControlBase tableControl)
        {
            this.tableControl = tableControl;
            HideExpanders = false;
            AllowEditingOfTotalCells = false;

            if (tableControl != null)
            {
                WireEvents();
            }
        }

        /// <summary>
        /// Wire the required events in the TableControl
        /// </summary>
        private void WireEvents()
        {
            if (this.tableControl != null)
            {
                isWired = true;
                this.tableControl.Model.SaveCellInfo += new GridSaveCellInfoEventHandler(Model_SaveCellInfo);
                this.tableControl.LostFocus += new EventHandler(TableControl_LostFocus);
                this.tableControl.KeyDown += new System.Windows.Forms.KeyEventHandler(TableControl_KeyDown);
                this.tableControl.CurrentCellAcceptedChanges += new CancelEventHandler(tableControl_CurrentCellAcceptedChanges);

                this.undoStack = new Stack<PivotValueEditedEventArgs>();
                this.redoStack = new Stack<PivotValueEditedEventArgs>();
            }
        }

        /// <summary>
        /// To Unwire the events from the tableControl
        /// </summary>
        private void UnWireEvents()
        {
            isWired = false;
            this.tableControl.Model.SaveCellInfo -= new GridSaveCellInfoEventHandler(Model_SaveCellInfo);
            this.tableControl.LostFocus -= new EventHandler(TableControl_LostFocus);
            this.tableControl.KeyDown -= new System.Windows.Forms.KeyEventHandler(TableControl_KeyDown);
            this.tableControl.CurrentCellAcceptedChanges -= new CancelEventHandler(tableControl_CurrentCellAcceptedChanges);

            if (this.undoStack != null)
            {
                this.undoStack.Clear();
            }
            if (this.redoStack != null)
            {
                this.redoStack.Clear();
            }

        }

        #region EventHandlers
        /// <summary>
        /// Occurs when a KeyDown is triggered
        /// </summary>
        void TableControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Z && e.Control)
            {
                PivotGridControlBase gridControl = sender as PivotGridControlBase;
                if (gridControl != null && gridControl.CurrentCell != null && gridControl.CurrentCell.IsEditing)
                {
                    gridControl.CurrentCell.CancelEdit();
                }
                else if (this.undoStack != null && this.undoStack.Count > 0)
                {
                    Undo();
                }

                e.Handled = true;
                gridControl.InvalidateCells();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Y && e.Control)
            {
                PivotGridControlBase gridControl = sender as PivotGridControlBase;
                if (gridControl != null && gridControl.CurrentCell != null && gridControl.CurrentCell.IsEditing)
                {
                    gridControl.CurrentCell.CancelEdit();
                }
                else if (this.redoStack != null && this.redoStack.Count > 0)
                {
                    Redo();
                }

                e.Handled = true;
                gridControl.InvalidateCells();
            }
        }

        public event PivotValueEditedEventHandler PivotValueEdited;
        /// <summary>
        /// Occurs when the control lost focus
        /// </summary>
        void TableControl_LostFocus(object sender, EventArgs e)
        {
            PivotGridControlBase gridControl = sender as PivotGridControlBase;
            if (gridControl != null && gridControl.CurrentCell != null && gridControl.CurrentCell.IsEditing)
            {
                gridControl.CurrentCell.Deactivate(true);
            }
        }

        Stack<PivotValueEditedEventArgs> undoStack = null;
        Stack<PivotValueEditedEventArgs> redoStack = null;
        bool fromCommitCellInfo = false;
        /// <summary>
        /// Occurs when the cell is in need to save the changes
        /// </summary>
        void Model_SaveCellInfo(object sender, GridSaveCellInfoEventArgs e)
        {
            if (!tableControl.EnableValueEditing)
                return;

            PivotCellInfo pi = tableControl.PivotEngine[e.RowIndex - 1, e.ColIndex - 1];
            PivotValueEditedEventArgs args = new PivotValueEditedEventArgs()
            {
                Handled = false,
                PivotColumnIndex = e.ColIndex -1,
                PivotRowIndex = e.RowIndex -1,
                NewValue = e.Style.CellValue,
                OldValue = pi.Value,
                PivotCellInfo = pi
            };
            if (!fromCommitCellInfo && pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) 
                || pi.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell) || pi.CellType == PivotCellType.ValueCell)
            {
                undoStack.Push(args);
            }

            if (PivotValueEdited != null)
            {
                PivotValueEdited(this, args);
            }
            if (!args.Handled && pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell)
                || pi.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell) || pi.CellType == PivotCellType.ValueCell)
            {
                string property = GetCalculationNameFromGridRowColumnIndex(e.ColIndex - 1);
                ChangeValue(pi.Value, e.Style.CellValue, e.RowIndex - 1, e.ColIndex - 1, property);
            }
        }

        /// <summary>
        /// Redo the last handled operation 
        /// </summary>
        public void Redo()
        {
            PivotValueEditedEventArgs args = redoStack.Pop();
            if (args != null)
            {
                string property = GetCalculationNameFromGridRowColumnIndex(args.PivotColumnIndex);
                ChangeValue(args.OldValue, args.NewValue, args.PivotRowIndex, args.PivotColumnIndex, property);
                undoStack.Push(args);
                this.tableControl.InvalidateCells(GridRangeInfo.Cell(args.PivotRowIndex, args.PivotColumnIndex));
                tableControl.CurrentCell.MoveTo(args.PivotRowIndex+1, args.PivotColumnIndex+1);
            }
        }

        /// <summary>
        /// Undo the last handled operation
        /// </summary>
        public void Undo()
        {
            PivotValueEditedEventArgs args = undoStack.Pop();
            if (args != null)
            {
                string property = GetCalculationNameFromGridRowColumnIndex(args.PivotColumnIndex);
                ChangeValue(args.NewValue, args.OldValue, args.PivotRowIndex, args.PivotColumnIndex, property);
                redoStack.Push(args);
                this.tableControl.InvalidateCells(GridRangeInfo.Cell(args.PivotRowIndex, args.PivotColumnIndex));
                tableControl.CurrentCell.MoveTo(args.PivotRowIndex+1, args.PivotColumnIndex+1);
            }
        }

        /// <summary>
        /// Occurs after the cell completes the editing
        /// </summary>
        void tableControl_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            this.tableControl.RefreshRange(GridRangeInfo.Table());
        }

       
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disposes the EditingManager
        /// </summary>
        public void Dispose()
        {
            if (isWired)
            {
                UnWireEvents();
            }
        }

        #endregion
        MethodInfo mInfo = null;

        /// <summary>
        /// Gets a list of the row and column pivot values at a given row and column.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        /// <param name="calcFieldName">Returns the name of the calculated field at the given row and column.</param>
        /// <returns>Returns a list of the row and column pivot values.</returns>
        /// <remarks>
        /// The returned list is an ordered list of the pivot values that determine the staring with the row pivot values followed by the column pivot values.
        /// There is a one to one correspondance between the number of items in <see cref="PivotGridControl.PivotRows"/> and then <see cref="PivotGridControl.PivotColumns"/>
        /// to this returned ordered list.
        /// </remarks>
        public List<IComparable> GetRowColumnPivotValuesAt(int row, int column, out string calcFieldName)
        {
            //int col1 = column - pivotGrid.PivotEngine.PivotRows.Count;
            calcFieldName = GetCalculationNameFromGridRowColumnIndex(column);

            if (mInfo == null)
            {
                mInfo = typeof(PivotEngine).GetMethod("GetKeyAt", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            List<IComparable> list = null;
            if (mInfo != null)
            {
                list = mInfo.Invoke(this.tableControl.PivotEngine, new object[] { row, column }) as List<IComparable>;
            }

            return list;
        }

        /// <summary>
        /// Returns the name of the field corresponding to the index
        /// </summary>
        string GetCalculationNameFromGridRowColumnIndex(int index)
        {
            int index1 = index - (tableControl.ShowCalculationsAsColumns ? tableControl.PivotEngine.PivotRows.Count : tableControl.PivotEngine.PivotColumns.Count);
            string name = tableControl.PivotEngine.PivotCalculations.Count > 0 && index1 >= 0 ? tableControl.PivotEngine.PivotCalculations[index1 % tableControl.PivotEngine.PivotCalculations.Count].FieldName : "";

            return name;
        }

        /// <summary>
        /// Updates the cell with new edited/updated value
        /// </summary>
        private void ChangeValue(object oldValue, object newValue, int row, int col, string propertyName)
        {
            int saveRow = row;
            int saveCol = col;

            PivotComputationInfo calc = tableControl.PivotEngine.PivotCalculations.Where(c => c.FieldName == propertyName).First() as PivotComputationInfo;
            int calOffSet = tableControl.PivotEngine.PivotCalculations.IndexOf(calc);
            int colOffSet = (tableControl.PivotEngine.ShowCalculationsAsColumns && tableControl.PivotEngine.PivotCalculations.Count > 1) ? calOffSet : 0;
            int rowOffSet = (!tableControl.PivotEngine.ShowCalculationsAsColumns && tableControl.PivotEngine.PivotCalculations.Count > 1) ? calOffSet : 0;

            PivotCellInfo pci = tableControl.PivotEngine[row, col];

            BinaryList changingRows = new BinaryList();
            BinaryList changingCols = new BinaryList();

            if (pci != null)
            {

                if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                {
                    //process from cell downward, adjusting the summaries

                    int col1 = tableControl.PivotEngine.PivotRows.Count - 1;
                    int row1 = tableControl.PivotEngine.PivotColumns.Count - 1;
                    bool notDone = true;
                    while (notDone && col1 > -1)
                    {
                        col1--;
                        changingRows.AddIfUnique(row);
                        if (col1 > -1)
                        {
                            row = GetNextSummaryDownRowIndex(row, col1) + rowOffSet;
                            if (row == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grandtotal at bottom
                    if (tableControl.ShowGrandTotals)
                    {
                        row = tableControl.PivotEngine.RowCount - 2 - rowOffSet;
                        changingRows.AddIfUnique(row);
                    }

                    //process from right of cell over to the right
                    col = saveCol;
                    row = saveRow;
                    notDone = true;
                    while (notDone && row1 > -1)
                    {
                        row1--;
                        changingCols.AddIfUnique(col);
                        if (row1 > -1)
                        {
                            col = GetNextSummaryOverColIndex(row1, col) + colOffSet;
                            if (col == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grand total on right
                    if (tableControl.ShowGrandTotals)
                    {
                        col = tableControl.PivotEngine.ColumnCount - 2 - (tableControl.PivotEngine.PivotCalculations.Count - colOffSet - 1);
                        changingCols.AddIfUnique(col);
                    }

                    //adjust all necessary summaries to the right and below (and including) of the changed summary
                    foreach (int r in changingRows)
                    {
                        foreach (int c in changingCols)
                        {
                            pci = tableControl.PivotEngine[r, c];
                            if (pci != null)
                            {
                                ChangeValue(oldValue, newValue, r, c, pci);
                                if (r != saveRow || c != saveCol)
                                    tableControl.RefreshRange(GridRangeInfo.Cell(r, c));
                            }
                        }
                    }
                }
                else
                {
                    //process from cell downward, adjusting the summaries

                    int col1 = tableControl.PivotEngine.PivotRows.Count - 1;
                    int row1 = tableControl.PivotEngine.PivotColumns.Count - 1;
                    bool notDone = true;
                    while (notDone && col1 > -1)
                    {
                        col1--;
                        changingRows.AddIfUnique(row);
                        if (col1 > -1)
                        {
                            row = GetNextSummaryDownRowIndex(row, col1) + rowOffSet;
                            if (row == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grandtotal at bottom
                    if (tableControl.ShowGrandTotals)
                    {
                        if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                            row = tableControl.PivotEngine.RowCount - 2 - rowOffSet;
                        else
                            //handle grandtotal at bottom - subract w.r.t Calculation count
                            row = tableControl.PivotEngine.RowCount - 2 - (tableControl.PivotEngine.PivotCalculations.Count - rowOffSet - 1);

                        changingRows.AddIfUnique(row);
                    }

                    //process from right of cell over to the right
                    col = saveCol;
                    row = saveRow;
                    notDone = true;
                    while (notDone && row1 > -1)
                    {
                        row1--;
                        changingCols.AddIfUnique(col);
                        if (row1 > -1)
                        {
                            col = GetNextSummaryOverColIndex(row1, col) + colOffSet;
                            if (col == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grand total on right
                    if (tableControl.ShowGrandTotals)
                    {
                        if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                            col = tableControl.PivotEngine.ColumnCount - 2 - (tableControl.PivotEngine.PivotCalculations.Count - colOffSet - 1);
                        else
                            col = tableControl.PivotEngine.ColumnCount - 2; // Subracted for Calculation column and for indexing

                        changingCols.AddIfUnique(col);
                    }

                    //adjust all necessary summaries to the right and below (and including) of the changed summary
                    foreach (int r in changingRows)
                    {
                        foreach (int c in changingCols)
                        {
                            pci = tableControl.PivotEngine[r, c];
                            if (pci != null)
                            {
                                ChangeValue(oldValue, newValue, r, c, pci);
                                if (r != saveRow || c != saveCol)
                                    tableControl.RefreshRange(GridRangeInfo.Cell(r, c));
                                //    tableControl.InternalGrid.InvalidateCell(GridRangeInfo.Cell(r, c));
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Updates the cell with new edited/updated value
        /// </summary>
        protected virtual void ChangeValue(object oldValue, object newValue, int row1, int col1, PivotCellInfo pi)
        {
            double dNew = 0d;
            double dOld = 0d;
            double dExisting = 0d;

            if (oldValue == null)
                oldValue = 0;
            if (newValue == null)
                newValue = 0;
            if (pi.Value == null)
                pi.Value = 0;

            newValue = newValue.ToString().TrimStart('$');
            bool bNew = double.TryParse(newValue.ToString(), out dNew);
            bool bOld = double.TryParse(oldValue.ToString(), out dOld);
            bool bExisting = double.TryParse(pi.Value.ToString(), out dExisting);

            if (bExisting)
            {
                dExisting += (bNew ? dNew : 0) - (bOld ? dOld : 0);
            }
            else
            {
                dExisting = (bNew ? dNew : 0) - (bOld ? dOld : 0);
            }
            pi.Value = dExisting;
            string _format = string.Empty;
            if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                _format = tableControl.PivotEngine.PivotCalculations[((col1 - tableControl.PivotEngine.PivotRows.Count)) % tableControl.PivotEngine.PivotCalculations.Count].Format;
            else
                _format = tableControl.PivotEngine.PivotCalculations[((row1 - tableControl.PivotEngine.PivotColumns.Count)) % tableControl.PivotEngine.PivotCalculations.Count].Format;

            pi.FormattedText = string.Format("{0:" + _format + "}", pi.Value);
        }

        /// <summary>
        /// To get the index of the next summary row with respect to the provide column index
        /// </summary>
        private int GetNextSummaryOverColIndex(int row1, int col)
        {
            int sumCol = -1;
            PivotCellInfo pci = tableControl.PivotEngine[row1, col];
            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && col > 0)
            {
                col--;
                pci = tableControl.PivotEngine[row1, col];
            }
            if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                sumCol = col + ((tableControl.PivotEngine[row1, col].CellRange != null) ? tableControl.PivotEngine[row1, col].CellRange.Right - tableControl.PivotEngine[row1, col].CellRange.Left + 1 : 1);
            else
                sumCol = col + ((tableControl.PivotEngine[row1, col].CellRange != null) ? tableControl.PivotEngine[row1, col].CellRange.Bottom - tableControl.PivotEngine[row1, col].CellRange.Top + 1 : 1);
            return sumCol;

        }

        /// <summary>
        /// To get the index of the next summary row with respect to the provide Row index
        /// </summary>
        private int GetNextSummaryDownRowIndex(int row, int col1)
        {
            int sumRow = -1;
            PivotCellInfo pci = tableControl.PivotEngine[row, col1];
            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && row > 0)
            {
                row--;
                pci = tableControl.PivotEngine[row, col1];
            }
            if (tableControl.PivotEngine.ShowCalculationsAsColumns)
                sumRow = row + ((tableControl.PivotEngine[row, col1].CellRange != null) ? tableControl.PivotEngine[row, col1].CellRange.Bottom - tableControl.PivotEngine[row, col1].CellRange.Top + 1 : 1);
            else
                sumRow = row + ((tableControl.PivotEngine[row, col1].CellRange != null) ? tableControl.PivotEngine[row, col1].CellRange.Right - tableControl.PivotEngine[row, col1].CellRange.Left + 1 : 1);
            return sumRow;
        }
    }
    #region PivotValueEdited event

    public delegate void PivotValueEditedEventHandler(object sender, PivotValueEditedEventArgs e);

    /// <summary>
    /// Event argumment for the PivotValueEdited event.
    /// </summary>
    public class PivotValueEditedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets whether the EditManager should adjust display values based on the 
        /// edited change. If you set Handled to true, the EditManager will not make any
        /// display changes. If you set Handled to false, the EditManager will adjust the
        /// changed cell and any cell that depends upon it.
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the row index in the grid of the cell that was edited.
        /// </summary>
        public int PivotRowIndex { get; internal set; }
        /// <summary>
        /// Gets the column index in the grid of the cell that was edited.
        /// </summary>
        public int PivotColumnIndex { get; internal set; }
        /// <summary>
        /// Gets the PivotCellInfo of the cell that was edited.
        /// </summary>
        public PivotCellInfo PivotCellInfo { get; internal set; }
        /// <summary>
        /// Gets the value in the cell before the edit occurred.
        /// </summary>
        public object OldValue { get; internal set; }
        /// <summary>
        /// Gets the new value endtered into the cell.
        /// </summary>
        public object NewValue { get; internal set; }
    }

    #endregion
}
