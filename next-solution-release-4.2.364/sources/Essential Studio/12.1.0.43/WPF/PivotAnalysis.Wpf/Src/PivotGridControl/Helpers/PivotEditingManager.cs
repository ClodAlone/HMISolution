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
using Syncfusion.Windows.Controls.PivotGrid;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.PivotAnalysis.Base;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using System.Globalization;
using Syncfusion.Windows.Controls.Cells;
#if SILVERLIGHT
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Silverlight.Controls.PivotGrid;
using Syncfusion.Linq;
#endif
namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// PivotEditingManager provides editing support for value cells in a PivotGridControl. You enable this support
    /// by setting PivotGridControl.EnableValueEditing to true.
    /// </summary>
    public class PivotEditingManager : IDisposable
    {
        PivotGridControl pivotGrid = null;
        internal bool _isHookEventsCalled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pivotGrid"></param>
        public PivotEditingManager(PivotGridControl pivotGrid)
        {
            this.pivotGrid = pivotGrid;
            HideExpanders = false;
            AllowEditingOfTotalCells = false;



#if !SILVERLIGHT
            if (pivotGrid.IsLoaded)
            {
                HookEvents();
            }
            else if (pivotGrid.InternalGrid != null)
#else   

            if (pivotGrid.InternalGrid != null)
#endif
            {
                HookEvents();
            }
            else
            {
                this.pivotGrid.Loaded += (s, e) =>
                    {
                        if (!_isHookEventsCalled)
                        {
                            HookEvents();
                            _isHookEventsCalled = true;
                        }
                    };
            }
            this.pivotGrid.Unloaded += (s, e) =>
                {
                    UnhookEvents();
                };
        }

        #region public methods
        /// <summary>
        /// Clears Undo/Redo lists and unsubscribes to events.
        /// </summary>
        public void Dispose()
        {
            if (isHooked)
            {
                UnhookEvents();
            }
        }

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
                mInfo = typeof(PivotEngine).GetMethod("GetKeyAt",  System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            List<IComparable> list = null;
            if (mInfo != null)
            {
               list =  mInfo.Invoke(this.pivotGrid.PivotEngine, new object[] {row, column}) as List<IComparable>;
            }

            return list;
        }
#if SILVERLIGHT
        private object[] ProcessList(List<PivotItem> pivotItems)
        {
            int count = pivotItems.Count;
            object[] pds = new object[count];

            for (int i = 0; i < count; i++)
            {

                if (pivotGrid.PivotEngine.ItemProperties != null && pivotGrid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] is PropertyInfo)
                    pds[i] = pivotGrid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] as PropertyInfo;
                else if (pivotGrid.PivotEngine.ItemProperties != null && pivotGrid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] is ExpressionPropertyDescriptor)
                    pds[i] = pivotGrid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] as ExpressionPropertyDescriptor;

            }

            return pds;
        }
#endif
        /// <summary>
        /// Gets a list of the raw data items that go into computing the value displayed in the given row and column.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        /// <returns>The list of data items associated with the given cell.</returns>
        /// <remarks>
        /// If the row and column point to any cell other than a Value cell, an list containing zero items is returned. If you are using a DataView as the
        /// underlying data source, the returned list contain DataRowView objects. If you are using an IList&lt;T&gt; object, then the return list will
        /// contain T objects."/>
        /// </remarks>
        public List<object> GetRawItemsFor(int row, int col)
        {
            if (this.pivotGrid.PivotEngine != null)
                return pivotGrid.PivotEngine.GetRawItemsFor(row, col);
            else
                return null;
        }
        /// <summary>
        /// Reverts the last editing change.
        /// </summary>
        /// <remarks>
        /// If a cell is actively being edited when this method is called, then the editing of that cell is cancelled. If there
        /// is no actively editing cell when this method is called, then the last changed cell is reverted to its previous value.
        /// </remarks>
        public void Undo()
        {
            PivotValueEditedArgs args = undoStack.Pop();
            if (args != null)
            {
                string property = GetCalculationNameFromGridRowColumnIndex(args.PivotColumnIndex);
                ChangeValue(args.NewValue, args.OldValue, args.PivotRowIndex, args.PivotColumnIndex, property);
                redoStack.Push(args);
            //    pivotGrid.InternalGrid.InvalidateCell(GridRangeInfo.Cell(args.PivotRowIndex, args.PivotColumnIndex));
                pivotGrid.InternalGrid.CurrentCell.MoveTo(args.PivotRowIndex, args.PivotColumnIndex);
            }
        }

        /// <summary>
        /// Redoes the previously undone editing change.
        /// </summary>
        /// <remarks>
        /// If a cell is actively being edited when this method is called, then the editing of that cell is cancelled. If there
        /// is no actively editing cell when this method is called, then the last undone editing action is restored.
        /// </remarks>
        public void Redo()
        {
            PivotValueEditedArgs args = redoStack.Pop();
            if (args != null)
            {
                string property = GetCalculationNameFromGridRowColumnIndex(args.PivotColumnIndex);
                ChangeValue(args.OldValue, args.NewValue, args.PivotRowIndex, args.PivotColumnIndex, property);
                undoStack.Push(args);
               //pivotGrid.InternalGrid.InvalidateCell(GridRangeInfo.Cell(args.PivotRowIndex, args.PivotColumnIndex));
                pivotGrid.InternalGrid.CurrentCell.MoveTo(args.PivotRowIndex, args.PivotColumnIndex);
            }
        }

        /// <summary>
        /// Event raised as the user completes the editing of a cell.
        /// </summary>
        public event PivotValueEditedEvent PivotValueEdited;

        #endregion

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

        #region private methods

        bool isHooked = false;
        MethodInfo mInfo = null;
       
        private void HookEvents()
        {
            if (this.pivotGrid.InternalGrid != null)
            {
                this.pivotGrid.InternalGrid.QueryCellInfo += new GridQueryCellInfoEventHandler(InternalGrid_QueryCellInfo);
                this.pivotGrid.InternalGrid.CommitCellInfo += new GridCommitCellInfoEventHandler(InternalGrid_CommitCellInfo);
                this.pivotGrid.InternalGrid.CurrentCellStartEditing += InternalGrid_CurrentCellStartEditing;
                this.pivotGrid.InternalGrid.PrepareRenderCell += new GridPrepareRenderCellEventHandler(InternalGrid_PrepareRenderCell);
                this.pivotGrid.InternalGrid.LostFocus += new System.Windows.RoutedEventHandler(InternalGrid_LostFocus);
               
#if !SILVERLIGHT
                this.pivotGrid.InternalGrid.PreviewKeyDown += new KeyEventHandler(InternalGrid_PreviewKeyDown);
#else
                this.pivotGrid.InternalGrid.KeyDown += new KeyEventHandler(InternalGrid_PreviewKeyDown);
               
#endif
                this.undoStack = new Stack<PivotValueEditedArgs>();
                this.redoStack = new Stack<PivotValueEditedArgs>();

                isHooked = true;

                //need to refresh the display so QCI is hit to set the TextBox celltype
                this.pivotGrid.InternalGrid.InvalidateCells();
            }
        }

        void InternalGrid_CurrentCellStartEditing(object sender, ComponentModel.SyncfusionCancelRoutedEventArgs args)
        {
            args.Cancel = (pivotGrid.EnableSpecificColumnEditing || pivotGrid.EnableValueEditing) ? false : true;
            args.Handled = (pivotGrid.EnableSpecificColumnEditing || pivotGrid.EnableValueEditing) ? false : true;
        }
    
        private void UnhookEvents()
        {
            this.pivotGrid.InternalGrid.QueryCellInfo -= new GridQueryCellInfoEventHandler(InternalGrid_QueryCellInfo);
            this.pivotGrid.InternalGrid.CommitCellInfo -= new GridCommitCellInfoEventHandler(InternalGrid_CommitCellInfo);
            this.pivotGrid.InternalGrid.PrepareRenderCell -= new GridPrepareRenderCellEventHandler(InternalGrid_PrepareRenderCell);
            this.pivotGrid.InternalGrid.LostFocus -= new System.Windows.RoutedEventHandler(InternalGrid_LostFocus);
#if !SILVERLIGHT
                this.pivotGrid.InternalGrid.PreviewKeyDown += new KeyEventHandler(InternalGrid_PreviewKeyDown);
#else
            this.pivotGrid.InternalGrid.KeyDown += new KeyEventHandler(InternalGrid_PreviewKeyDown);

#endif
            if (this.undoStack != null)
            {
                this.undoStack.Clear();
            }
            if (this.redoStack != null)
            {
                this.redoStack.Clear();
            }
            isHooked = false;
        }

         private void ChangeValue(object oldValue, object newValue, int row, int col, string propertyName)
        {
            int saveRow = row;
            int saveCol = col;

            PivotComputationInfo calc = pivotGrid.PivotEngine.PivotCalculations.Where(c => c.FieldName == propertyName).First() as PivotComputationInfo;
            int calOffSet = pivotGrid.PivotEngine.PivotCalculations.IndexOf(calc);
            int colOffSet = (pivotGrid.PivotEngine.ShowCalculationsAsColumns && pivotGrid.PivotEngine.PivotCalculations.Count > 1) ? calOffSet : 0;
            int rowOffSet = (!pivotGrid.PivotEngine.ShowCalculationsAsColumns && pivotGrid.PivotEngine.PivotCalculations.Count > 1) ? calOffSet : 0;

            PivotCellInfo pci = pivotGrid.PivotEngine[row, col];

            BinaryList changingRows = new BinaryList();
            BinaryList changingCols = new BinaryList();

            if (pci != null)
            {

                if (pivotGrid.PivotEngine.ShowCalculationsAsColumns)
                {
                    //process from cell downward, adjusting the summaries

                    int col1 = pivotGrid.PivotEngine.PivotRows.Count - 1;
                    int row1 = pivotGrid.PivotEngine.PivotColumns.Count - 1;
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
                    if (pivotGrid.ShowGrandTotals)
                    {
                        row = pivotGrid.PivotEngine.RowCount - 2 - rowOffSet;
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
                            col = GetNextSummaryOverColIndex(row1, col);
                            if (col == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grand total on right
                    if (pivotGrid.ShowGrandTotals)
                    {
                        col = pivotGrid.PivotEngine.ColumnCount - 2 - (pivotGrid.PivotEngine.PivotCalculations.Count - colOffSet - 1);
                        changingCols.AddIfUnique(col);
                    }

                    //adjust all necessary summaries to the right and below (and including) of the changed summary
                    foreach (int r in changingRows)
                    {
                        foreach (int c in changingCols)
                        {
                            pci = pivotGrid.PivotEngine[r, c];
                            if (pci != null)
                            {
                                ChangeValue(oldValue, newValue, r, c, pci);
                                if (r != saveRow || c != saveCol)
                                    pivotGrid.InternalGrid.InvalidateCell(GridRangeInfo.Cell(r, c));
                            }
                        }
                    }
                }
                else
                {
                    //process from cell downward, adjusting the summaries

                    int col1 = pivotGrid.PivotEngine.PivotRows.Count - 1;
                    int row1 = pivotGrid.PivotEngine.PivotColumns.Count - 1;
                    bool notDone = true;
                    while (notDone && col1 > -1)
                    {
                        col1--;
                        changingRows.AddIfUnique(row);
                        if (col1 > -1)
                        {
                            row = GetNextSummaryDownRowIndex(row, col1);
                            if (row == -1)
                            {
                                notDone = false;
                            }
                        }
                    }
                    //handle grandtotal at bottom
                    if (pivotGrid.ShowGrandTotals)
                    {
                        row = pivotGrid.PivotEngine.RowCount - (pivotGrid.PivotEngine.PivotColumns.Count + (!pivotGrid.PivotEngine.ShowCalculationsAsColumns || pivotGrid.PivotCalculations.Count <= 1 ? 0 : 1));
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
                    if (pivotGrid.ShowGrandTotals)
                    {
                        if (pivotGrid.PivotEngine.ShowCalculationsAsColumns)
                            col = pivotGrid.PivotEngine.ColumnCount - 2 - (pivotGrid.PivotEngine.PivotCalculations.Count - colOffSet - 1);
                        else
                            col = pivotGrid.PivotEngine.ColumnCount - 2; // Subracted for Calculation column and for indexing

                        changingCols.AddIfUnique(col);
                    }

                    //adjust all necessary summaries to the right and below (and including) of the changed summary
                    foreach (int r in changingRows)
                    {
                        foreach (int c in changingCols)
                        {
                            pci = pivotGrid.PivotEngine[r, c];
                            if (pci != null)
                            {
                                ChangeValue(oldValue, newValue, r, c, pci);
                                if (r != saveRow || c != saveCol)
                                    pivotGrid.InternalGrid.InvalidateCell(GridRangeInfo.Cell(r, c));
                            }
                        }
                    }
                 
                }
            }
        }

         private int GetNextSummaryOverColIndex(int row1, int col)
         {
             int sumCol = -1, column = col;
             PivotCellInfo pci = pivotGrid.PivotEngine[row1, col];
             while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && col > 0)
             {
                 col--;
                 pci = pivotGrid.PivotEngine[row1, col];
             }
             if (pivotGrid.PivotEngine[row1, col] != null)
             {
                 if (pivotGrid.PivotEngine.ShowCalculationsAsColumns)
                     sumCol = col + ((column - pivotGrid.PivotEngine.PivotRows.Count) % pivotGrid.PivotEngine.PivotCalculations.Count) + ((pivotGrid.PivotEngine[row1, col].CellRange != null) ? pivotGrid.PivotEngine[row1, col].CellRange.Right - pivotGrid.PivotEngine[row1, col].CellRange.Left + 1 : 1);
                 else
                     sumCol = col + (pivotGrid.PivotEngine[row1, col].CellRange != null ? pivotGrid.PivotEngine[row1, col].CellRange.Right - pivotGrid.PivotEngine[row1, col].CellRange.Left : 1) + 1;
             }
             return sumCol;

         }

         private int GetNextSummaryDownRowIndex(int row, int col1)
         {
             int sumRow = -1, tempRow = row;
             PivotCellInfo pci = pivotGrid.PivotEngine[row, col1];
             while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && row > 0)
             {
                 row--;
                 pci = pivotGrid.PivotEngine[row, col1];
             }
             if (pivotGrid.PivotEngine[row, col1] != null)
             {
                 if (pivotGrid.PivotEngine.ShowCalculationsAsColumns)
                 {
                     sumRow = row + ((pivotGrid.PivotEngine[row, col1].CellRange != null) ? pivotGrid.PivotEngine[row, col1].CellRange.Bottom - pivotGrid.PivotEngine[row, col1].CellRange.Top + 1 : 1);
                 }
                 else
                 {
                     sumRow = row + ((tempRow - pivotGrid.PivotEngine.PivotColumns.Count) % pivotGrid.PivotEngine.PivotCalculations.Count) + ((pivotGrid.PivotEngine[row, col1].CellRange != null) ? pivotGrid.PivotEngine[row, col1].CellRange.Bottom - pivotGrid.PivotEngine[row, col1].CellRange.Top + 1 : 1);
                 }
             }
             return sumRow;
         }

        /// <summary>
        /// Override this method to affect the change represented in the method arguments.
        /// </summary>
        /// <param name="oldValue">The old value from the cell that was edited.</param>
        /// <param name="newValue">The new value from the cell that was edited.</param>
        /// <param name="row1">Points to the row index of cell being adjusted. This cell may be different from the cell that was edited.</param>
        /// <param name="col1">Points to the column index of cell being adjusted. This cell may be different from the cell that was edited.</param>
        /// <param name="pi">The <see cref="PivotCellInfo"/> for the cell being adjusted.</param>
        /// <remarks>
        /// The default implementation treats the adjustment as if the underlying calculation were a summation. This means the oldValue is subtracted from the
        /// cell being adjusted, and the new value is added. The adjustment is made by setting pi.Value and pi.FormattedText. 
        /// </remarks>
        /// <example>
        ///   <para>The following code shows the default implementation.</para>
        ///   <code lang="C#">
        ///   protected virtual void ChangeValue(object oldValue, object newValue, int row1, int col1, PivotCellInfo pi)
        ///   {
        ///       double dNew = 0d;
        ///       double dOld = 0d;
        ///       double dExisting = 0d;
        ///   
        ///       if (oldValue == null)
        ///           oldValue = 0;
        ///       if (newValue == null)
        ///           newValue = 0;
        ///       if (pi.Value == null)
        ///           pi.Value = 0;
        ///   
        ///       bool bNew = double.TryParse(newValue.ToString(), out dNew);
        ///       bool bOld = double.TryParse(oldValue.ToString(), out dOld);
        ///       bool bExisting = double.TryParse(pi.Value.ToString(), out dExisting);
        ///   
        ///       if (bExisting)
        ///       {
        ///           dExisting += (bNew ? dNew : 0) - (bOld ? dOld : 0);
        ///       }
        ///       else  
        ///       {
        ///           dExisting = (bNew ? dNew : 0) - (bOld ? dOld : 0);
        ///       }
        ///       pi.Value = dExisting;
        ///       pi.FormattedText = string.Format("{0:" + pivotGrid.PivotEngine.PivotCalculations[col1 % pivotGrid.PivotEngine.PivotCalculations.Count].Format + "}", pi.Value);
        ///    }
        ///   </code>
        /// </example>
        protected virtual void ChangeValue(object oldValue, object newValue, int row1, int col1, PivotCellInfo pi)
        {
            double dNew = 0d;
            double dOld = 0d;
            
            string dNewString = "";
            string dOldString = "";
            double dExisting = 0d;
            string dExistingString = "";
            if (oldValue == null)
                oldValue = 0;
            if (newValue == null)
                newValue = 0;
            if (pi.Value == null)
                pi.Value = (pi.Summary.GetType() == typeof(DisplayIfDiscreteValuesEqual)) ? (object)"" : 0;
            bool bNew = false, bOld = false;
            bool bExisting = false;
            if (pi.Value != null && pi.Value.GetType() != typeof(string))
            {
                bNew = double.TryParse(newValue.ToString(), out dNew);
                bOld = double.TryParse(oldValue.ToString(), out dOld);
                bExisting = double.TryParse(pi.Value.ToString(), out dExisting);

                if (bExisting)
                {
                    dExisting += (bNew ? dNew : 0) - (bOld ? dOld : 0);
                }
                else
                {
                    dExisting = (bNew ? dNew : 0) - (bOld ? dOld : 0);
                }
                if (pi.CellType == PivotCellType.ValueCell
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell)
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell)
                    )
                    pi.Value = dExisting;
            }
            else
            {
                bExisting = ((newValue.ToString()) != null)? true : false;
                if (bExisting)
                {
                    dExistingString = newValue.ToString();
                }
                else
                {
                    dExistingString = oldValue.ToString();
                }
                if (pi.CellType == PivotCellType.ValueCell
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell)
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)
                    || pi.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell)
                    )
                    pi.Value = dExistingString;
            }
            //pi.FormattedText = string.Format("{0:" + pivotGrid.PivotEngine.PivotCalculations[col1 % pivotGrid.PivotEngine.PivotCalculations.Count].Format + "}", pi.Value);
            string _format = string.Empty;
            if (pivotGrid.PivotEngine.ShowCalculationsAsColumns)
                _format = pivotGrid.PivotEngine.PivotCalculations[((col1 - pivotGrid.PivotEngine.PivotRows.Count)) % pivotGrid.PivotEngine.PivotCalculations.Count].Format;
            else
                _format = pivotGrid.PivotEngine.PivotCalculations[((row1 - pivotGrid.PivotEngine.PivotColumns.Count)) % pivotGrid.PivotEngine.PivotCalculations.Count].Format;

            pi.FormattedText = string.Format("{0:" + _format + "}", pi.Value);
         }
        #endregion

        #region Event Handlers

        //used to handle ctl+Z/ctl+Y to undo/redo the latest change...
        void InternalGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
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
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
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
            }
        }

        //use to avoid defect of currentcell staying alive through changing schema through D&D.
        void InternalGrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PivotGridControlBase gridControl = sender as PivotGridControlBase;
#if !SILVERLIGHT
            if (gridControl != null && !gridControl.IsKeyboardFocusWithin && gridControl.CurrentCell != null)
            
#else

            if (gridControl != null && gridControl.CurrentCell != null)
            {
                if (!PivotGridControl.IsFocusIn(pivotGrid))

#endif
                {
                    gridControl.CurrentCell.Deactivate();
                }
            }
        
#if SILVERLIGHT
        }
#endif

        //use this event to hide the expanders...
        void InternalGrid_PrepareRenderCell(object sender, GridPrepareRenderCellEventArgs e)
        {
            if (HideExpanders)
            {
                if (e.Cell.RowIndex < pivotGrid.PivotEngine.RowCount && e.Cell.ColumnIndex < pivotGrid.PivotEngine.ColumnCount)
                {
                    PivotCellInfo pi = pivotGrid.PivotEngine[e.Cell.RowIndex, e.Cell.ColumnIndex];
                    if ((pi.CellType & PivotCellType.ExpanderCell) != 0)
                    {
                        e.Style.CellType = "Static";
                    };
                }
            }
        }
        /// <summary>
        /// Gets or Sets the edited cell's row/column index.
        /// </summary>
        public RowColumnIndex EditedCell { get; set; }

        Stack<PivotValueEditedArgs> undoStack = null;
        Stack<PivotValueEditedArgs> redoStack = null;
        bool fromCommitCellInfo = false;
        //use this event to raise the PivotValueEdited event...
        void InternalGrid_CommitCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            if ((!pivotGrid.EnableValueEditing && !pivotGrid.EnableSpecificColumnEditing) || (pivotGrid.InternalGrid.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell && !this.pivotGrid.InternalGrid.CurrentCell.Renderer.IsModified))
                return;
            EditedCell = e.Cell;
            PivotCellInfo pi = pivotGrid.PivotEngine[e.Cell.RowIndex, e.Cell.ColumnIndex];
            PivotValueEditedArgs args = new PivotValueEditedArgs()
            {
                Handled = false,
                PivotColumnIndex = e.Cell.ColumnIndex,
                PivotRowIndex = e.Cell.RowIndex,
                NewValue = pi != null ? ((pi.CellType != (PivotCellType.ValueCell | PivotCellType.TotalCell) && pivotGrid.InternalGrid.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell) ? this.pivotGrid.InternalGrid.CurrentCell.Renderer.ControlValue : e.Style.CellValue) : null,
                OldValue = pi != null ? pi.Value : null,
                PivotCellInfo = pi
            };
            if ((args.PivotCellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell)) || (args.PivotCellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell))
                || (args.PivotCellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)) || (args.PivotCellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell)) || (args.PivotCellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell))
                || (args.PivotCellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell)))
                return;
            if (!fromCommitCellInfo)
            {
                undoStack.Push(args);
            }

            if (PivotValueEdited != null)
            {
                PivotValueEdited(this, args);
            }
            if (!args.Handled)
            {
                 if (args.PivotCellInfo != null && (args.PivotCellInfo.Format != "#.##" && args.PivotCellInfo.Format != null || (args.PivotCellInfo.Format == null && args.PivotCellInfo.Value != null)))
                {
                    if (args.NewValue == null || args.NewValue.ToString() == string.Empty)
                        args.NewValue = 0.0;
                    else if (args.NewValue != null)
                    {
                        double d = 0d;
                        if (Double.TryParse(args.NewValue.ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentUICulture, out d))
                        {
                            args.NewValue = d;
                        }
                    }
                }
                string property = GetCalculationNameFromGridRowColumnIndex(pivotGrid.ShowCalculationsAsColumns ? args.PivotColumnIndex : args.PivotRowIndex);
                ChangeValue(args.OldValue, args.NewValue, args.PivotRowIndex, args.PivotColumnIndex, property);
            }
        }

        string GetCalculationNameFromGridRowColumnIndex(int index)
        {
            int index1 = index - (pivotGrid.ShowCalculationsAsColumns ? pivotGrid.PivotEngine.PivotRows.Count : pivotGrid.PivotEngine.PivotColumns.Count);
            string name = pivotGrid.PivotEngine.PivotCalculations.Count > 0 && index1 >= 0 ? pivotGrid.PivotEngine.PivotCalculations[index1 % pivotGrid.PivotEngine.PivotCalculations.Count].FieldName : "";
    
            return name;
        }
        
        //use this event to make certain cells editable
        void InternalGrid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (!pivotGrid.EnableValueEditing && !pivotGrid.EnableSpecificColumnEditing)
                return;

            if (e.Cell.RowIndex < pivotGrid.PivotEngine.RowCount && e.Cell.ColumnIndex < pivotGrid.PivotEngine.ColumnCount && (pivotGrid.PivotEngine.ColumnCount > 1 && pivotGrid.PivotEngine.RowCount > 1))
            {
                if ((pivotGrid.EnableSpecificColumnEditing && pivotGrid.EditColumnList != null && pivotGrid.EditColumnList.Count > 0 && pivotGrid.EditColumnList.Any(o => o.ToString() == pivotGrid.PivotEngine.GetFieldNameAtIndex(e.Cell.ColumnIndex).ToString())) || (pivotGrid.EnableValueEditing))
                {
                    PivotCellInfo pi = pivotGrid.PivotEngine[e.Cell.RowIndex, e.Cell.ColumnIndex];
                    if (pi != null && (pi.CellType & PivotCellType.ValueCell) != 0
                        && (AllowEditingOfTotalCells || (pi.CellType & PivotCellType.TotalCell) == 0
                         && (pi.CellType & PivotCellType.GrandTotalCell) == 0)
                        )
                    {
                        e.Style.CellType = "TextBox";
                    }
                }
            }
        }

       #endregion     
    }

    #region PivotValueEdited event
    /// <summary>
    /// Event handler for the <see cref="PivotValueEditedEvent"/> event.
    /// </summary>
    /// <param name="sender">The EditManager raising the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void PivotValueEditedEvent(object sender, PivotValueEditedArgs e);

    /// <summary>
    /// Event argumment for the PivotValueEdited event.
    /// </summary>
    public class PivotValueEditedArgs : EventArgs
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
