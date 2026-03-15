#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Used by GridSelectCellsMouseController.
    /// </summary>
    public delegate bool GridCurrentCellMoveDelegateHandler(GridDirectionType direction, int num, bool extendSelection);

    /// <summary>
    /// Manages the current cell for a grid. Provides methods for activating, deactivating, and
    /// moving the current cell.
    /// </summary>
    public class GridCurrentCell : IDisposable
    {
        #region Fields
        // Fields
        GridControlBase grid;
        private int rowIndex = -1;
        private int columnIndex = -1;
        private IGridCellRenderer cellRenderer;
        private bool isModified;
        private bool isEditing;
        private bool inAcceptedChanges;
        private bool inActivate;
        private bool inActivated;
        private bool inActivateFailed;
        private bool inBeginEdit;
        private bool inCancelEdit;
        private bool inChanging;
        private bool inChanged;
        private bool inConfirmChanges;
        private bool inDeactivate;
        private bool inDeactivated;
        private bool inDeactivateFailed;
        private bool inEndEdit;
        private bool inMoveTo;
        private bool inMoved;
        private bool inMoveFailed;
        private bool inRejectChanges;
        // private bool inShowDropDown;
        private int keyMoveRowIndex = -1;
        private int keyMoveColumnIndex = -1;
        private int moveToRowIndex = -1;

        public int MoveToRowIndex
        {
            get
            {
                return moveToRowIndex;
            }
        }

        private int moveToColumnIndex = -1;

        internal int MoveToColumnIndex
        {
            get
            {
                return moveToColumnIndex;
            }
        }
        private GridActivateCurrentCellOptions activateOptions;

        public GridActivateCurrentCellOptions ActivateOptions
        {
            get { return this.activateOptions; }
        }

        private bool moveFromActiveState;
        /// <summary>
        /// Gets a value indicating whether there was an activated current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo"/> was called.
        /// </summary>
        public bool MoveFromActiveState
        {
            get { return moveFromActiveState; }
        }
        private int moveFromRowIndex = -1;

        /// <summary>
        /// Gets the saved row index information about the previous position of the current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo"/> was called.
        /// </summary>
        public int MoveFromRowIndex
        {
            get { return moveFromRowIndex; }
        }
        private int moveFromColumnIndex = -1;

        /// <summary>
        /// Gets the saved column index information about the previous position of the current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo"/> was called.
        /// </summary>
        public int MoveFromColIndex
        {
            get { return moveFromColumnIndex; }
        }
        private bool moveToDone;
        /// <summary>
        /// Gets the state if the MoveTo method is complete.
        /// </summary>
        public bool MoveToDone
        {
            get
            {
                return this.moveToDone;
            }
        }

        private bool notifyChangingCalled;
        private Exception exception;
        private bool isValid;
        private int locked = 0;
        private bool savedModified;
        bool shouldUnloadInCancelEdit = false;
        #endregion

        GridModel gridModel { get { return grid.Model; } }

        #region Ctor
        /// <summary>
        /// Initializes the object, attaches it to a grid, and subscribes to events.
        /// </summary>
        /// <param name="grid">The parent grid for this object.</param>
        public GridCurrentCell(GridControlBase grid)
        {
            this.grid = grid;
        }
        #endregion

        #region Lock
        /// <summary>
        /// Lets you temporarily "lock" the current cell. While a current cell
        /// is locked, any attempts to move, deactivate, save, or activate the current
        /// cell will fail.
        /// </summary>
        public void Lock()
        {
            locked++;
        }

        /// <summary>
        /// Unlocks a temporarily "locked" current cell with <see cref="Lock"/>.
        /// </summary>
        public void Unlock()
        {
            if (locked > 0)
                locked--;
        }

        /// <summary>
        /// Determines if the current cell's state is "locked". If the current cell
        /// is locked, any attempts to move, deactivate, save, or activate the current
        /// cell will fail.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return locked > 0;
            }
        }
        #endregion

        #region HasCurrentCell, CellRowColumnIndex, Renderer, Grid

        /// <summary>
        /// The row and column index of the current cell as a <see cref="GridRangeInfo"/>.
        /// </summary>
        /// <remarks>
        /// Changing this property will trigger a call to <see cref="MoveTo"/>. When the
        /// value is <see cref="GridRangeInfo.Empty"/>, the current cell will be deactivated.
        /// </remarks>
        public GridRangeInfo RangeInfo
        {
            get
            {
                int rowIndex, columnIndex;
                if (GetCurrentCell(out rowIndex, out columnIndex))
                    return GridRangeInfo.Cell(rowIndex, columnIndex);
                return GridRangeInfo.Empty;
            }
            set
            {
                if (value.IsEmpty)
                    MoveTo(-1, -1);
                else if (value.Width == 1 && value.Height == 1)
                    MoveTo(value.Top, value.Left);
                else
                    throw new ArgumentException("value");
            }
        }

        /// <summary>
        /// The row index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }
        /// <summary>
        /// The column index.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                return columnIndex;
            }
        }

        /// <summary>
        /// The row and column index of the current cell as a <see cref="RowColumnIndex"/>.
        /// </summary>
        /// <remarks>
        /// Changing this property will trigger a call to <see cref="MoveTo"/>. When the
        /// value is <see cref="RowColumnIndex.Empty"/>, the current cell will be deactivated.
        /// </remarks>
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                if (Renderer == null)
                    return RowColumnIndex.Empty;
                return Renderer.CellRowColumnIndex;
            }
            set
            {
                if (value.IsEmpty)
                    MoveTo(-1, -1);
                else
                    MoveTo(value.RowIndex, value.ColumnIndex);
            }
        }

        /// <summary>
        /// Updates the cell's row and column indices with the given value.
        /// </summary>
        /// <param name="cellRowColumnIndex">The new row and column indices as <see cref="RowColumnIndex"/>.</param>
        public void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            if (cellRowColumnIndex.IsEmpty)
                cellRenderer = null;
            else if (Renderer != null)
            {
                Renderer.UpdateCellRowColumnIndex(cellRowColumnIndex);
                rowIndex = cellRowColumnIndex.RowIndex;
                columnIndex = cellRowColumnIndex.ColumnIndex;
            }
        }

        /// <summary>
        /// Checks if the specified row and column indices are valid.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>True if it is valid; False otherwise.</returns>
        public bool IsValidCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            if (grid == null || grid.Model == null || grid.Model.RowHeights == null || grid.Model.ColumnWidths == null || cellRowColumnIndex.RowIndex < 0 || cellRowColumnIndex.ColumnIndex < 0 ||
                            cellRowColumnIndex.RowIndex > grid.Model.RowCount || cellRowColumnIndex.ColumnIndex > grid.Model.ColumnCount)
                return false;
            return true;
        }

        /// <summary>
        /// Returns position of the current cell together with a boolean if the current cell is active or not.
        /// </summary>
        /// <param name="rowIndex">The row index of the current cell.</param>
        /// <param name="columnIndex">The column index of the current cell.</param>
        /// <returns>True if grid has active current cell; False otherwise.</returns>
        public bool GetCurrentCell(out int rowIndex, out int columnIndex)
        {
            rowIndex = columnIndex = -1;
            if (HasCurrentCell)
            {
                rowIndex = this.RowIndex;
                columnIndex = this.ColumnIndex;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns position of the current cell together with a boolean if the current cell is active or not.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column indices of the current cell as <see cref="RowColumnIndex"/>.</param>
        /// <returns>True if grid has active current cell; False otherwise.</returns>
        public bool GetCurrentCell(out RowColumnIndex cellRowColumnIndex)
        {
            cellRowColumnIndex = RowColumnIndex.Empty;
            if (HasCurrentCell)
            {
                cellRowColumnIndex = this.CellRowColumnIndex;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the grid has an active current cell.
        /// </summary> 
        public bool HasCurrentCell
        {
            get
            {
                return cellRenderer != null;
            }
        }

        /// <summary>
        /// Checks if the current cell is active and at the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>True if current cell was found at row and column indices; False otherwise.</returns>
        public bool HasCurrentCellAt(int rowIndex, int columnIndex)
        {
            return HasCurrentCell && rowIndex == this.RowIndex && columnIndex == this.ColumnIndex;
        }

        /// <summary>
        /// Checks if the current cell is active and at the specific row index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>True if current cell was found at row index; False otherwise.</returns>
        public bool HasCurrentCellAt(int rowIndex)
        {
            return HasCurrentCell && rowIndex == this.RowIndex;
        }

        /// <summary>
        /// Checks if the current cell is active at the specified <see cref="RowColumnIndex"/>.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column index as <see cref="RowColumnIndex"/>.</param>
        /// <returns>True if current cell was found at the given <see cref="RowColumnIndex"/>; False otherwise.</returns>
        public bool HasCurrentCellAt(RowColumnIndex cellRowColumnIndex)
        {
            return HasCurrentCell && cellRowColumnIndex == this.CellRowColumnIndex;
        }

        /// <summary>
        /// Checks if the current cell is active at the specific range.
        /// </summary>
        /// <param name="range">The cell range.</param>
        /// <returns>True if current cell was found at the given range; False otherwise.</returns>
        public bool HasCurrentCellAt(GridRangeInfo range)
        {
            return HasCurrentCell && range.Contains(RangeInfo);
        }

        /// <summary>
        /// The parent grid.
        /// </summary>
        public GridControlBase Grid
        {
            get { return grid; }
        }

        /// <summary>
        /// The cell renderer.
        /// </summary>
        public IGridCellRenderer Renderer
        {
            get { return cellRenderer; }
        }

        #endregion

        #region Activate
        /// <summary>
        /// Returns whether <see cref="GridCurrentCell.Deactivate"/> or <see cref="GridCurrentCell.Activate"/>
        /// is in progress.
        /// </summary>
        public bool IsInActivateOrDeactivate
        {
            get
            {
                return inDeactivate || inActivate || inActivateFailed || inDeactivateFailed;
            }
        }

        /// <summary>
        /// Returns whether <see cref="GridCurrentCell.Activate"/>
        /// is in progress.
        /// </summary>
        public bool IsInActivate
        {
            get
            {
                return inActivate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInActivated.
        /// </summary>
        public bool IsInActivated
        {
            get
            {
                return inActivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInActivateFailed.
        /// </summary>
        public bool IsInActivateFailed
        {
            get
            {
                return inActivateFailed;
            }
        }

        /// <summary>
        /// Activates the current cell at the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>True if activating the current cell was successful; False otherwise.</returns>
        public bool Activate(int rowIndex, int columnIndex)
        {
            return Activate(rowIndex, columnIndex, new GridActivateCurrentCellOptions());
        }

        /// <summary>
        /// Activates the current cell at the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="activateOptions">A <see cref="GridActivateCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if activating the current cell was successful; False otherwise.</returns>
        public bool Activate(int rowIndex, int columnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
            return Activate(cellRowColumnIndex, activateOptions);
        }

        /// <summary>
        /// Activates the current cell at the specified position.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell's row column index as <see cref="RowColumnIndex"/>.</param>
        /// <param name="activateOptions">A <see cref="GridActivateCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if activating the current cell was successful; False otherwise.</returns>
        public bool Activate(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (IsLocked) return false;
            if (HasCurrentCell && !(this.Renderer is GridDataDataTemplateCellRenderer)) return false;

            if (!(activateOptions.IsActivateTriggeredByMouseDownIntoUIElement || activateOptions.IsExternalMove))
                inDeactivated = false;
                
            if (inDeactivated)
                throw new Exception("Activate is being called from within Deactivaed call.");

            if (HasCurrentCellAt(rowIndex, columnIndex))
            {
                BeginEditHelper(activateOptions);
                return true;
            }

            bool success = false;
            Exception = null;
            try
            {
                if (!IsValidCellRowColumnIndex(cellRowColumnIndex)) return false;

                // Raise Activating event
                inActivate = true;
                GridRenderStyleInfo style = grid.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
				
				  //if ModelStyle Empty  means it will take default render, It may cause of exception. To avoid this we have return false if ModelStyle is Empty.
                var renderEditorType = ((GridCellRendererBase)(style.CellRenderer)).EditorType;
                bool skipActivate=(renderEditorType!=null && activateOptions.Element!=null&& !activateOptions.Element.GetType().Equals(renderEditorType));
                if (skipActivate)
                {
                    return false;
                }
				
                IGridCellRenderer targetRenderer = style.CellRenderer;
                if (!targetRenderer.RaiseActivating(this.grid, cellRowColumnIndex, activateOptions))
                    return false;

                if (!Grid.RaiseCurrentCellActivating(ref cellRowColumnIndex, activateOptions))
                    return false;

                // update internal state
                this.rowIndex = cellRowColumnIndex.RowIndex;
                this.columnIndex = cellRowColumnIndex.ColumnIndex;
                if (!this.inMoveTo)
                {
                    this.keyMoveRowIndex = cellRowColumnIndex.RowIndex;
                    this.keyMoveColumnIndex = cellRowColumnIndex.ColumnIndex;
                }
                this.cellRenderer = targetRenderer;
                success = true;

                BeginEditHelper(activateOptions);
                inActivate = false;

                // Raise Activated event
                inActivated = true;
                //CellRenderer is null while changing itemsource on cell activate
                if (this.cellRenderer == null)
                    return false;
                this.cellRenderer.RaiseActivated();
                Grid.RaiseCurrentCellActivated();
                inActivated = false;

                // Render current cell border into a DrawingVisual of the ScrollControl.ForegroundFrame.
                // This will not trigger any InvalidateVisual or InvalidateArrange calls.
                grid.RenderCurrentCellBorder();
                return true;
            }
            finally
            {
                inActivate = false;
                inActivated = false;
                if (!success)
                {
                    // Raise ActivateFailed event
                    inActivateFailed = true;
                    try
                    {
                        if (cellRenderer != null)
                            cellRenderer.RaiseActivateFailed();
                        Grid.RaiseCurrentCellActivateFailed(cellRowColumnIndex);
                    }
                    finally
                    {
                        inActivateFailed = false;
                    }
                }
            }
        }

        private void BeginEditHelper(GridActivateCurrentCellOptions activateOptions)
        {
            if (IsEditing)
                return;

            shouldUnloadInCancelEdit = false;

            // Initialize renderer and optionally edit mode
            //if ((grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.ClickOnCell | GridCellActivateAction.SetCurrent)) != 0)
            //    shouldUnloadInCancelEdit = CreateCurrentCellUIElements();

            cellRenderer.RaiseInitialize(activateOptions);
            //Initialize(activateOptions);

            if (activateOptions.ShouldBeginEdit || activateOptions.IsActivateTriggeredByGotFocus  )
                BeginEdit(true);
        }

        //private void Initialize(GridActivateCurrentCellOptions options)
        //{
        //}
        #endregion

        #region Deactivate
        /// <summary>
        /// Returns whether IsInDeactivateFailed.
        /// </summary>
        public bool IsInDeactivateFailed
        {
            get
            {
                return inDeactivateFailed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInDeactivated.
        /// </summary>
        public bool IsInDeactivated
        {
            get
            {
                return inDeactivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.Deactivate"/> is in progress.
        /// </summary>
        public bool IsInDeactivate
        {
            get
            {
                return inDeactivate;
            }
        }

        /// <summary>
        /// Deactivates the current cell and confirms or rejects changes made to the current cell.
        /// </summary>
        /// <returns>True if current cell can be deactivated; False otherwise.</returns>
        public bool Deactivate()
        {
            return Deactivate(false);
        }

        /// <summary>
        /// Deactivates the current cell and confirms or rejects changes made to the current cell.
        /// </summary>
        /// <param name="discardChangesIfCommitFails">True if changes can be discarded; False otherwise.</param>
        /// <returns>True if current cell can be deactivated; False otherwise.</returns>
        public bool Deactivate(bool discardChangesIfCommitFails)
        {
            if (!HasCurrentCell)
            {
                isEditing = false;
                return true;
            }

            RowColumnIndex savedCellRowColumnIndex = CellRowColumnIndex;
            IGridCellRenderer savedCellRenderer = this.cellRenderer;
            bool success = false;

            this.savedModified = isModified;
            Exception = null;
            inDeactivate = true;
            try
            {
                // Raise Deactivating event
                if (!cellRenderer.RaiseDeactivating()) return false;
                if (!Grid.RaiseCurrentCellDeactivating()) return false;
                if (!HasCurrentCell) return true;

                // Save or discard user changes
                if (IsModified)
                {
                    if (!discardChangesIfCommitFails)
                    {
                        // Fires NotifyValidate, should CloseDropDown() possibly fires ControlLostFocus event.
                        GridDataControl GDC = this.Grid.FindParentElementOfType<GridDataControl>();
                        if (GDC != null)
                        {
                            var currentStyle = this.Grid.Model[this.RowIndex, this.ColumnIndex] as GridDataStyleInfo;
                            if ((GDC.UpdateMode != UpdateMode.PropertyChanged)||(currentStyle != null && (currentStyle.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell|| currentStyle.CellIdentity.TableCellType== GridDataTableCellType.RecordCell||currentStyle.CellIdentity.TableCellType== GridDataTableCellType.UnboundRecordCell)))
                            {
                                if (!this.ConfirmChanges())
                                    return false;
                            }                            
                        }
                        else
                        {
                            GridTreeControl GTC = this.Grid.FindParentElementOfType<GridTreeControl>();
                            if (GTC != null && GTC.UpdateMode != UpdateMode.RowCachedMode)
                            {
                                if (!this.ConfirmChanges())
                                    return false;
                            }
                            else
                            {
                                if (!this.ConfirmChanges())
                                    return false;
                            }

                        }
                    }
                    else
                        // Should CloseDropDown() possibly fires ControlLostFocus event.
                        this.RejectChanges();
                }
                success = true;

                // Reset editing mode, reset cell editor
                if (IsEditing)
                    CancelEdit(true);//!activateOptions.IsActivateTriggeredByMouseDownIntoUIElement);
                //since the below code breaks the default behavior, this is removed.
                //else //Else condition to move the Focus out of the DataTemplate renderer when it is not in EditMode and Pressing Tab.
                //{
                //    if (cellRenderer != null && cellRenderer.AllowGridToFocus)
                //        grid.Focus();
                //}
                // Update internal state

                if (inDeactivate)
                {
                    if (!HasCurrentCell)
                        return true;

                    this.cellRenderer = null;

                    // At this time there is no current cell anymore (HasCurrentCell = false).
                    // Raise Deactivated event
                    inDeactivate = false;
                    inDeactivated = true;
                    savedCellRenderer.RaiseDeactivated();
                    Grid.RaiseCurrentCellDeactivated(savedCellRowColumnIndex);
                    inDeactivated = false;
                }

                // Remove current cell border from DrawingVisual of the ScrollControl.ForegroundFrame.
                // This will not trigger any InvalidateVisual or InvalidateArrange calls.
                grid.RenderCurrentCellBorder();

                return true;
            }
            catch (Exception ex)
            {
                success = false;
                Exception = ex;
                return false;
            }
            finally
            {
                inDeactivate = false;
                inDeactivated = false;
                savedModified = false;

                if (!success)
                {
                    // Raise DeactivateFailed event
                    inDeactivateFailed = true;
                    try
                    {
                        if (cellRenderer != null)
                            cellRenderer.RaiseDeactivateFailed();
                        Grid.RaiseCurrentCellDeactivateFailed();
                    }
                    finally
                    {
                        inDeactivateFailed = false;
                    }
                }
            }
        }
        #endregion

        #region ConfirmChanges, RejectChanges
        /// <summary>
        /// Gets a value indicating whether IsInConfirmChanges.
        /// </summary>
        public bool IsInConfirmChanges
        {
            get
            {
                return inConfirmChanges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInAcceptedChanges.
        /// </summary>
        public bool IsInAcceptedChanges
        {
            get
            {
                return inAcceptedChanges;
            }
        }

        /// <summary>
        /// Confirms any pending changes for the current cell and closes any open drop-down windows.
        /// </summary>
        /// <returns>True if changes could be saved or if current cell was not modified; False if saving the changes
        /// failed.</returns>
        public bool ConfirmChanges()
        {
            if (!HasCurrentCell || IsLocked) return false;

            inConfirmChanges = true;
            try
            {
                if (cellRenderer != null)
                {
                    //if (closeDropDown && IsDroppedDown)
                    //    CloseDropDown(PopupCloseType.Done);

                    if (IsModified)
                    {
                        Validate();
                        if (!IsValid)
                        {
                            cellRenderer.RaiseConfirmChangesFailed();
                            Grid.RaiseCurrentCellConfirmChangesFailed();
                            return false;
                        }

                        if (cellRenderer == null || !cellRenderer.RaiseSaveChanges())
                            return false;

                        IsModified = false;
                        inAcceptedChanges = true;
                        try
                        {
                            if (!Grid.RaiseCurrentCellAcceptedChanges())
                            {
                                inAcceptedChanges = false;
                                IsModified = true;
                                cellRenderer.RaiseConfirmChangesFailed();
                                Grid.RaiseCurrentCellConfirmChangesFailed();
                                return false;
                            }
                        }
                        finally
                        {
                            inAcceptedChanges = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cellRenderer.RaiseConfirmChangesFailed();
                Grid.RaiseCurrentCellConfirmChangesFailed();
                this.Exception = ex;
                return false;
            }
            finally
            {
                inConfirmChanges = false;
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether IsInRejectChanges.
        /// </summary>
        public bool IsInRejectChanges
        {
            get
            {
                return inRejectChanges;
            }
        }

        /// <summary>
        /// Discards any changes for the current cell.
        /// </summary>
        public void RejectChanges()
        {
            if (!HasCurrentCell || IsLocked) return;

            inRejectChanges = true;
            try
            {
                //if (IsDroppedDown)
                //    CloseDropDown(PopupCloseType.Canceled);
                if (cellRenderer != null)
                    cellRenderer.RaiseRejectChanges();
                IsModified = false;
                Grid.RaiseCurrentCellRejectedChanges();
            }
            finally
            {
                inRejectChanges = false;
            }
        }

        #endregion

        #region CurrentCellUIElement - create and unload

        /// <summary>
        /// Creates the UI elements of current cell.
        /// </summary>
        /// <returns>True if the operation was successful; False otherwise.</returns>
        public bool CreateCurrentCellUIElements()
        {
            CellUIElements cellUI = Grid.GetCellUIElements(CellRowColumnIndex);
            if (cellUI == null)
            {
                Grid.DelayedCreateCellUIElements(CellRowColumnIndex);
                cellUI = Grid.GetCellUIElements(CellRowColumnIndex);
                if (cellUI != null && cellUI.UIElements.Count > 0)
                {
                    activateOptions.Element = cellUI.UIElements[0];
                    cellRenderer.RefreshContent();
                    Grid.InvalidateCell(CellRowColumnIndex);
                    Grid.InvalidateVisual(false);
                    if (cellRenderer.IsFocused)
                        activateOptions.Element.Focus();
                    return true;
                }
            }
            else if (!this.cellRenderer.SupportsRenderOptimization && this.IsEditing)
            {
                if (cellUI != null && cellUI.UIElements.Count > 0)
                {
                    activateOptions.Element = cellUI.UIElements[0];
                    cellRenderer.RefreshContent();
                    Grid.InvalidateCell(CellRowColumnIndex);
                    Grid.InvalidateVisual(false);
                    if (cellRenderer.IsFocused && !activateOptions.Element.IsKeyboardFocusWithin)
                        activateOptions.Element.Focus();
                    cellUI.IsDirty = false;
                    return false;
                }
            }

            else
                cellUI.IsDirty = false;

            return false;
        }

        /// <summary>
        /// Disposes the UI element of current cell.
        /// </summary>
        public void UnloadCurrentCellUIElement()
        {
            Grid.ArrangedCellUIElements.Unload(CellRowColumnIndex);
            Grid.InvalidateCell(CellRowColumnIndex);
            Grid.InvalidateVisual(false);
        }

        bool inRefresh = false;

        /// <summary>
        /// Gets a value indicating whether IsInRefresh.
        /// </summary>
        public bool IsInRefresh { get { return inRefresh; } }

        /// <summary>
        /// Refreshes the current cell.
        /// </summary>
        public void Refresh()
        {
            inRefresh = true;
            try
            {
                if (HasCurrentCell && Renderer != null)
                    Renderer.RefreshContent();
            }
            finally
            {
                inRefresh = false;
            }
        }

        #endregion

        #region BeginEdit, EndEdit, CancelEdit
        /// <summary>
        /// Gets a value indicating whether the current cell is in editing mode.
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
        }

        /// <summary>
        /// Returns true when <see cref="BeginEdit"/> was called; False after method returned.
        /// </summary>
        public bool IsInBeginEdit
        {
            get
            {
                return inBeginEdit;
            }
        }

        /// <summary>
        /// Starts editing mode for the current cell.
        /// </summary>
        /// <returns>True if current cell supports editing; False otherwise.</returns>
        public bool BeginEdit()
        {
            return BeginEdit(grid.IsKeyboardFocusWithin);
        }

        /// <summary>
        /// Starts editing mode for the current cell and allows setting the focus to the cell editor.
        /// </summary>
        /// <param name="focusCellUIElement">Specifies if focus can be set to the cell editor.</param>
        /// <returns>True if current cell supports editing; False otherwise.</returns>
        public bool BeginEdit(bool focusCellUIElement)
        {
            if (IsLocked || !HasCurrentCell) return false;
            if (isEditing) return true;
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(cellRenderer.CellRowColumnIndex);
            if (style != null && style.ReadOnly && !gridModel.Options.AllowTextSelectionOnReadOnly)
                return false;

            inBeginEdit = true;
            try
            {
                if (IsValidCellRowColumnIndex(CellRowColumnIndex))
                {
                    if (cellRenderer != null && !IsEditing && cellRenderer.IsEditable)
                    {
                        notifyChangingCalled = false;

                        if (cellRenderer == null || !cellRenderer.RaiseStartEditing())
                            return false;

                        if (!Grid.RaiseCurrentCellStartEditing())
                        {
                            EndEdit();
                            return false;
                        }

                        isEditing = true;

                        shouldUnloadInCancelEdit = CreateCurrentCellUIElements();
                        //CellRenderer is null while changing itemsource on cell activate
                        if (this.cellRenderer == null)
                            return false;
                        cellRenderer.RaiseBeginEdit();
                        if (cellRenderer != null)
                            if (cellRenderer.IsFocusable && focusCellUIElement)
                                cellRenderer.IsFocused = true;
						// When Current cell enter edit mode the entire cell should be in view. 
                        if (!(cellRenderer is GridDataCellNestedGridRenderer))
                            ScrollInView();
                        //CellRenderer is null while changing itemsource on cell activate
                        if (this.cellRenderer == null)
                            return false;
                        if (cellRenderer.CurrentCellUIElement != null)
                            Grid.RaiseCurrenctCellLoaded(cellRenderer.CellRowColumnIndex, cellRenderer.CurrentCellUIElement);
                    }
                    return true;
                }
            }
            finally
            {
                inBeginEdit = false;
            }
            return false;

        }

        /// <summary>
        /// Returns true when <see cref="CancelEdit"/> was called; False after method returned.
        /// </summary>
        public bool IsInCancelEdit
        {
            get
            {
                return inCancelEdit;
            }
        }

        internal void ResetCache()
        {
            if (this.cellRenderer != null)
            {
                IGridCellRenderer savedCellRenderer = this.cellRenderer;
                cellRenderer = null;
                inDeactivated = true;
                savedCellRenderer.RaiseDeactivated();
                inDeactivated = false;
            }
        }

        /// <summary>
        /// Cancels editing for the current cell and discards any changes.
        /// </summary>
        public void CancelEdit()
        {
            CancelEdit(true);
        }

        /// <summary>
        /// Cancels editing for the current cell and discards any changes.
        /// </summary>
        /// <param name="setGridFocus">When true, the focus will be set to the grid.</param>
        public void CancelEdit(bool setGridFocus)
        {
            if (IsLocked || !HasCurrentCell || inCancelEdit) return;

            inCancelEdit = true;
            try
            {
                if (IsModified)
                    cellRenderer.RaiseRejectChanges();

                //if (cellRenderer != null)
                //    cellRenderer.RaiseEndEdit();//This code is commented Because Before committing value Control values are reset so move this to last of this method

                IsModified = false;
                isEditing = false;
                if (setGridFocus && cellRenderer!=null && cellRenderer.AllowGridToFocus)
                    grid.Focus();

                if (shouldUnloadInCancelEdit)
                    //&& (Grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.ClickOnCell | GridCellActivateAction.SetCurrent)) == 0)
                    UnloadCurrentCellUIElement();

                if (cellRenderer != null)
                    cellRenderer.RaiseEditingComplete();
                Grid.RaiseCurrentCellEditingComplete();

                if (cellRenderer != null)
                    cellRenderer.RaiseEndEdit();
            }
            finally
            {
                inCancelEdit = false;
            }
        }

        
#if !SILVERLIGHT
        /// <summary>
        /// This property used to Check whether CurrentCell is in FilterBarCell
        /// </summary>

        internal bool IsInDropDownFilterCell
        {
            get;
            set;
        }
#endif
       

        /// <summary>
        /// Gets a value indicating whether IsInEndEdit.
        /// </summary>
        public bool IsInEndEdit
        {
            get
            {
                return inEndEdit;
            }
        }

        /// <summary>
        /// Finished up editing mode for the current cell.
        /// </summary>
        public void EndEdit()
        {
            if (IsLocked || !HasCurrentCell) return;

            inEndEdit = true;
            try
            {
                if (IsModified)
                {
                    // Fires NotifyValidate, should CloseDropDown() possibly fires ControlLostFocus event.
                    ConfirmChanges();
                }
                CancelEdit();
            }
            finally
            {
                inEndEdit = false;
            }
        }


        #endregion

        #region NotifyChanging, NotifyChanged
        /// <summary>
        /// Gets or sets a value indicating whether there are pending changes in the current cell.
        /// </summary>
        public bool IsModified
        {
            get { return isModified; }
            private set { isModified = value; }
        }

        /// <summary>
        /// Indicates that the current cell is being changed.
        /// </summary>
        /// <returns>The boolean value NotifyChanging</returns>
        public bool NotifyChanging()
        {
            // only raise event first time cell contents are changed.                               // this should be skipped for Property changed update mode. 
            GridDataControl gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl != null)
            {
                if (gridDataControl.Model.TableProperties.UpdateMode != UpdateMode.PropertyChanged)
                {
                    if (notifyChangingCalled) return true;              // In property changed mode every time when we changing the cell value NotifyCurrentCellChanging() should be fired. 
                }
            }

            inChanging = true;
            try
            {
                if (IsInActivateOrDeactivate)
                    notifyChangingCalled = true;
                else
                    notifyChangingCalled = Grid.RaiseCurrentCellChanging();
                return notifyChangingCalled;
            }
            finally
            {
                inChanging = false;
            }
        }

        /// <summary>
        /// Signifies the current cell change.
        /// </summary>
        public void NotifyChanged()
        {
            if (!IsInActivateOrDeactivate)
            {
                inChanged = true;
                try
                {
                    IsModified = true;
                    Grid.RaiseCurrentCellChanged();
                }
                finally
                {
                    inChanged = false;
                }
            }
        }

        /// <summary>
        /// Indicates if the current cell's <see cref="GridControlBase.CurrentCellChanging"/> event
        /// is being handled.
        /// </summary>
        public bool IsInChanging
        {
            get
            {
                return inChanging;
            }
        }

        /// <summary>
        /// Indicates if the current cell's <see cref="GridControlBase.CurrentCellChanged"/> event
        /// is being handled.
        /// </summary>
        public bool IsInChanged
        {
            get
            {
                return inChanged;
            }
        }

        #endregion

        #region MoveTo

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.MoveTo"/> is in progress. MoveTo saves information
        /// about the current cell state and its target cell so that events can more easily compare
        /// previous and new states of the current cell during <see cref="GridCurrentCell.MoveTo"/> calls.
        /// </summary>
        public bool IsInMoveTo
        {
            get
            {
                return inMoveTo;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInMoved.
        /// </summary>
        public bool IsInMoved
        {
            get
            {
                return inMoved;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsInMoveFailed.
        /// </summary>
        public bool IsInMoveFailed
        {
            get
            {
                return inMoveFailed;
            }
        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        public bool MoveTo(int rowIndex, int columnIndex)
        {
            return MoveTo(rowIndex, columnIndex, new GridActivateCurrentCellOptions());
        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="activateOptions">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        public bool MoveTo(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            return MoveTo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, activateOptions);
        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        public bool MoveTo(RowColumnIndex cellRowColumnIndex)
        {
            return MoveTo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, new GridActivateCurrentCellOptions());
        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="activateOptions">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        public bool MoveTo(int rowIndex, int columnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (IsLocked) return false;

            if (!this.IsValidCellRowColumnIndex(new RowColumnIndex(rowIndex, columnIndex)))
                return false;
            if (HasCurrentCellAt(rowIndex, columnIndex) && this.cellRenderer.CurrentStyle.Store != null)
            {
                BeginEditHelper(activateOptions);
                this.cellRenderer.RaiseActivated();
                return true;
            }

            //GridCellRendererBase savedCellRenderer = this.cellRenderer;
            int savedRowIndex = this.rowIndex;
            int savedColumnIndex = this.columnIndex;
            bool success = false;
            //bool focused = grid.HasControlFocus;

            keyMoveRowIndex = -1;
            keyMoveColumnIndex = -1;

            GridRenderStyleInfo style = grid.GetRenderStyleInfo(rowIndex, columnIndex);
            if (!style.Enabled)// && !Grid.IsDesignMode())
                return false;

            this.moveFromColumnIndex = savedColumnIndex;
            this.moveFromRowIndex = savedRowIndex;
            this.moveFromActiveState = this.HasCurrentCell;
            this.activateOptions = activateOptions;
            this.moveToRowIndex = rowIndex;
            this.moveToColumnIndex = columnIndex;
            this.inMoveTo = true;

            bool noActivate = false;

            try
            {
                {
                    RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                    if (!grid.RaiseCurrentCellMoving(ref cellRowColumnIndex, activateOptions))
                        return false;
                    rowIndex = cellRowColumnIndex.RowIndex;
                    columnIndex = cellRowColumnIndex.ColumnIndex;
                }

                // Update cached information in case Moving event changes parameters.
                this.activateOptions = activateOptions;
                this.moveToRowIndex = rowIndex;
                this.moveToColumnIndex = columnIndex;

                AdjustRowColIfCoveredCell(ref rowIndex, ref columnIndex);
                if (activateOptions.SetCurrentCellOptions== GridSetCurrentCellOptions.ScrollInView && columnIndex < this.Grid.NavigateWithArrowKeysCellsRange.Left)
                    columnIndex = this.Grid.NavigateWithArrowKeysCellsRange.Left;

                // Update cached information in case AdjustRowColIfCoveredCell changes parameters.
                this.moveToRowIndex = rowIndex;
                this.moveToColumnIndex = columnIndex;
                //GridOrientation movement = GetMovement(rowIndex, columnIndex);

                if (!Deactivate(activateOptions.DiscardChanges))
                    return false;

                IsModified = false;
                rowIndex = moveToRowIndex;
                columnIndex = moveToColumnIndex;

                if (!noActivate && !Activate(rowIndex, columnIndex, activateOptions))
                    return false;

                this.moveToRowIndex = this.rowIndex;
                this.moveToColumnIndex = this.columnIndex;

                this.moveToDone = true;

                success = true;

                this.inMoved = true;
                Grid.RaiseCurrentCellMoved(activateOptions);
                this.inMoved = false;

                return true;
            }
            finally
            {
                this.inMoveTo = false;
                this.inMoved = false;

                if (!success)
                {
                    this.inMoveFailed = true;
                    try
                    {
                        Grid.RaiseCurrentCellMoveFailed(new RowColumnIndex(rowIndex, columnIndex), activateOptions);
                        if (Exception != null)
                        {
                            //this.DisplayWarningText(this.ErrorMessage);
                        }
                    }
                    finally
                    {
                        this.inMoveFailed = false;
                    }
                }

            }
        }

        #endregion

        #region Validate, Exception

        /// <summary>
        /// Validates the current cell.
        /// </summary>
        public void Validate()
        {
            if (!HasCurrentCell || IsLocked) return;

            try
            {
                var style = gridModel[RowIndex, ColumnIndex];
                var oldValue = style.CellValue;
                var newValue = cellRenderer != null ? cellRenderer.ControlValue : null;
                object modifiedValue = null;
                
                // Possibly shows a dialog box (but user can also set CurrentCell.ErrorMessage).
                IsValid = Grid.RaiseCurrentCellValidating(style, oldValue,newValue,out modifiedValue) && (cellRenderer == null || cellRenderer.RaiseValidate());
                if (cellRenderer != null && cellRenderer.ControlValue !=null && !cellRenderer.ControlValue.Equals(modifiedValue))
                {
                    cellRenderer.ControlValue = modifiedValue;
                }
                if (isValid)
                {
                    if (cellRenderer != null)
                        cellRenderer.RaiseValidated();
                    Grid.RaiseCurrentCellValidated();
                }
                else
                {
                    this.gridModel.Selections.Clear();
                    this.gridModel.Selections.Add(GridRangeInfo.Cell(this.RowIndex, this.ColumnIndex));
                }
            }
            catch (Exception ex)
            {
                IsValid = false;
                Exception = ex;
            }
        }

        /// <summary>
        /// Returns success of the latest <see cref="Validate"/> method call.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
            private set { isValid = value; }
        }

        /// <summary>
        /// Holds the details of any exceptin that has been caught during current cell operations.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
            private set { exception = value; }
        }

        #endregion

        #region DropDown

        /*public void CloseDropDown()
        {
        }

        public void ShowDropDown()
        {
        } */

        /// <summary>
        /// Gets a value indicating whether the drop-down window of a current cell is dropped-down.
        /// </summary>
        public bool IsDroppedDown
        {
            get;
            internal set;
        }

        #endregion

        #region Move Aliases

        /// <overload>
        /// Moves the current cell down and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveDown()
        {
            MoveDown(1, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down to the next enabled row after skipping a specified number of rows.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <genoverload/>
        public void MoveDown(int num)
        {
            MoveDown(num, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down to the next enabled row after skipping a specified number of rows and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveDown(int num, bool extendSelection)
        {
            Move(GridDirectionType.Down, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell up and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell up to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveUp()
        {
            MoveUp(1, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell up to the next enabled row after skipping a specified number of rows.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <genoverload/>
        public void MoveUp(int num)
        {
            MoveUp(num, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell up to the next enabled row after skipping a specified number of rows and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveUp(int num, bool extendSelection)
        {
            Move(GridDirectionType.Up, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell left and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell left to the next enabled column.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveLeft()
        {
            bool extenselection = (Keyboard.Modifiers & ModifierKeys.Shift) != 0 ? (Keyboard.IsKeyDown(Key.Tab) ? false : true) : false;
            MoveLeft(1, extenselection);
        }
        /// <summary>
        /// Moves the current cell left to the next enabled column after skipping a specified number of columns.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <genoverload/>
        public void MoveLeft(int num)
        {
            MoveLeft(num, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell left to the next enabled column after skipping a specified number of columns and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveLeft(int num, bool extendSelection)
        {
            Move(GridDirectionType.Left, num, extendSelection);
        }


        /// <overload>
        /// Moves the current cell right and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell right to the next enabled column.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveRight()
        {
            MoveRight(1, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell right to the next enabled column after skipping a specified number of columns.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <genoverload/>
        public void MoveRight(int num)
        {
            MoveRight(num, (Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell right to the next enabled column after skipping a specified number of columns and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveRight(int num, bool extendSelection)
        {
            Move(GridDirectionType.Right, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell up one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell up one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void PageUp()
        {
            PageUp((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell up one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void PageUp(bool extendSelection)
        {
            Move(GridDirectionType.PageUp, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void PageDown()
        {
            PageDown((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void PageDown(bool extendSelection)
        {
            Move(GridDirectionType.PageDown, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell left one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell left one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void PageLeft()
        {
            PageLeft((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell left one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void PageLeft(bool extendSelection)
        {
            Move(GridDirectionType.PageLeft, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell right one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell right one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void PageRight()
        {
            PageRight((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell right one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void PageRight(bool extendSelection)
        {
            Move(GridDirectionType.PageRight, 1, extendSelection);
        }



        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveToRightEnd()
        {
            MoveToRightEnd((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveToRightEnd(bool extendSelection)
        {
            Move(GridDirectionType.MostRight, 1, extendSelection);
        }



        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveToBottom()
        {
            MoveToBottom((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveToBottom(bool extendSelection)
        {
            Move(GridDirectionType.Bottom, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveToLeftEnd()
        {
            MoveToLeftEnd((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }

        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveToLeftEnd(bool extendSelection)
        {
            Move(GridDirectionType.MostLeft, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveToTop()
        {
            MoveToTop((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <genoverload/>
        public void MoveToTop(bool extendSelection)
        {
            Move(GridDirectionType.Top, 1, extendSelection);
        }

        public void MoveToTopLeft()
        {
            MoveToTopLeft((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }

        public void MoveToTopLeft(bool extendSelection)
        {
            Move(GridDirectionType.TopLeft, 1, extendSelection);
        }

        public void MoveToBottomRight()
        {
            MoveToBottomRight((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
        }

        public void MoveToBottomRight(bool extendSelection)
        {
            Move(GridDirectionType.BottomRight, 1, extendSelection);
        }

        #endregion

        #region Move Implementation

        bool inMove;

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.Move"/> is in progress.
        /// </summary>
        public bool IsInMove
        {
            get { return inMove; }
        }

        /// <overload>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <param name="wrapCell">Indicates if grid should move to beginning of next row when at end of row or vice versa.</param>
        /// <returns>True if the current cell was moved to a new position; False otherwise (e.g. if current cell is at first row
        /// and you tried to move up).</returns>
        public bool Move(GridDirectionType direction, int num, bool extendSelection, bool wrapCell)
        {
            if (IsLocked) return false;

            inMove = true;
            try
            {

                GridMoveCurrentCellDirectionEventArgs qme = new GridMoveCurrentCellDirectionEventArgs(direction, num, extendSelection, this.RowIndex, this.ColumnIndex, GridControlBase.MoveCurrentCellDirectionEvent, this.Grid);
                Grid.RaiseMoveCurrentCellDirection(qme);
                if (qme.Handled)
                {
                    if (!qme.Result)
                    {
                        // Move was canceled. Display error if any.
                        //if (this.Exception != null)
                        //{
                        //    Grid.CancelUpdate();
                        //    this.DisplayWarningText(this.ErrorMessage);
                        //}
                    }
                    return qme.Result;
                }

                if (wrapCell && HasCurrentCell)
                {
                    GridRangeInfo gridCells = grid.NavigateWithArrowKeysCellsRange;
                    GridWrapCellBehavior wrapFlags = Grid.Model.Options.WrapCellBehavior;
                    int ri = this.RowIndex;
                    int ci = this.ColumnIndex;
                    if (direction == GridDirectionType.Right)
                    {
                        bool isLastCol = (ci == gridCells.Right);
                        if (!isLastCol)
                        {
                            ci++;
                            isLastCol = !Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref ri, ref ci);
                        }

                        if (isLastCol)
                        {
                            bool isLastRow = (ri == gridCells.Bottom);
                            if (!isLastRow)
                            {
                                ri++;
                                ci = this.ColumnIndex;
                                // Move to next row, set isLastRow if subsequent rows are disabled.
                                isLastRow = !Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref ri, ref ci);
                            }

                            if (isLastRow)
                            {
                                if (wrapFlags == GridWrapCellBehavior.WrapGrid)
                                    return Move(GridDirectionType.TopLeft, 1, false, false);

                                else if (wrapFlags == GridWrapCellBehavior.NextControlInForm)
                                {
                                    GridWrapCellNextControlInFormEventArgs e = new GridWrapCellNextControlInFormEventArgs(true, false, GridControlBase.WrapCellNextControlInFormEvent, this.Grid);
                                    Grid.RaiseWrapCellNextControlInForm(e);
                                    if (!e.Handled &&
                                        (!e.MoveTopLeft || Move(GridDirectionType.TopLeft, 1, false, false)))
                                    {
                                        // TODO: Select next control in parent form.
                                        return true;
                                    }
                                }
                                else if (this.IsEditing)
                                {
                                    this.EndEdit();
                                    return true;
                                }
                                return false;
                            }
                            else
                            {
                                // Move to first column in next row.
                                if (Move(GridDirectionType.Down, 1, false, false))
                                {
                                    Move(GridDirectionType.MostLeft, 1, false, false);
                                    this.ScrollInView();
                                }
                                return true;
                            }
                        }
                    }
                    else if (direction == GridDirectionType.Left)
                    {
                        bool isFirstCol = (ci == gridCells.Left);
                        if (!isFirstCol)
                        {
                            ci--;
                            // Move to previous column, set isFirstCol if previous columns are disabled.
                            isFirstCol = !Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref ri, ref ci);
                        }

                        if (isFirstCol)
                        {
                            bool isFirstRow = (ri == gridCells.Top);
                            if (!isFirstRow)
                            {
                                ri--;
                                ci = this.ColumnIndex;
                                isFirstRow = !Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref ri, ref ci);
                            }

                            if (isFirstRow)
                            {
                                if (wrapFlags == GridWrapCellBehavior.WrapGrid)
                                    return Move(GridDirectionType.BottomRight, 1, false, false);
                                else if (wrapFlags == GridWrapCellBehavior.NextControlInForm)
                                {
                                    GridWrapCellNextControlInFormEventArgs e = new GridWrapCellNextControlInFormEventArgs(false, false, GridControlBase.WrapCellNextControlInFormEvent, this.Grid);
                                    Grid.RaiseWrapCellNextControlInForm(e);
                                    if (!e.Handled)
                                    {
                                        // TODO: Select previous control in parent form.
                                        return true;
                                    }
                                }
                                return false;
                            }
                            else
                            {
                                // Move to last column in previous row.
                                if (Move(GridDirectionType.Up, 1, false, false))
                                {                                                                     
                                    Move(GridDirectionType.MostRight, 1, false, false);
                                    this.ScrollInView();                                    
                                }
                                return true;
                            }
                        }
                    }
                }

                int savedRowIndex = rowIndex;
                int savedColumnIndex = columnIndex;

                GridActivateCurrentCellOptions ao = new GridActivateCurrentCellOptions();
                if (ExternalMove != null)
                    ExternalMove(direction, num, extendSelection);
                else
                    InternalMove(direction, num, ao);
                return rowIndex != savedRowIndex || columnIndex != savedColumnIndex;
            }
            finally
            {
                inMove = false;
            }
        }

        /// <summary>
        /// Used by GridSelectCellsMouseController.
        /// </summary>
        public GridCurrentCellMoveDelegateHandler ExternalMove
        {
            get
            {
                return grid.externalMove;
            }
            set
            {
                grid.externalMove = value;
            }
        }

        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="extendSelection">When true, extends the current selection.</param>
        /// <returns>True if the current cell was moved to a new position; False otherwise (e.g. if current cell is at first row
        /// and you tried to move up).</returns>
        public bool Move(GridDirectionType direction, int num, bool extendSelection)
        {
            return Move(direction, num, extendSelection, gridModel.Options.WrapCell);
        }

        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// without selecting the cells. <see cref="GridCurrentCell.Move"/> for a method that
        /// also selects cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="options">The <see cref="GridSetCurrentCellOptions"/> that specifies the options for current cell movement.</param>
        /// <returns>True if this operation successfully completes.</returns>
        public bool InternalMove(GridDirectionType direction, int num, GridActivateCurrentCellOptions options)
        {
            if (IsLocked)
                return false;

            if (this.rowIndex >= 0)
            {
                GridRangeInfo regularCells = grid.NavigateWithArrowKeysCellsRange;
                GridRangeInfo notFrozenCells = grid.ScrollCellsRange;
                bool querySuccess = false;
                bool hasCurrentCell = HasCurrentCell;
                bool allowSelectRange = (options.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.None;
                bool scrollFrozen = gridModel.Options.ScrollFrozen;
                int currentRow = this.rowIndex;
                int currentCol = this.columnIndex;
                int targetRowIndex = currentRow;
                int targetColumnIndex = currentCol;
                int vscrollup = 0;
                int hscrollleft = 0;
                bool vert = false;
                bool horz = false;

                switch (direction)
                {
                    case GridDirectionType.Up:
                    case GridDirectionType.Down:
                    case GridDirectionType.PageDown:
                    case GridDirectionType.PageUp:
                    case GridDirectionType.Top:
                    case GridDirectionType.Bottom:
                        vert = true;
                        break;

                    case GridDirectionType.PageRight:
                    case GridDirectionType.PageLeft:
                    case GridDirectionType.Left:
                    case GridDirectionType.Right:
                    case GridDirectionType.MostLeft:
                    case GridDirectionType.MostRight:
                        horz = true;
                        break;

                    case GridDirectionType.TopLeft:
                    case GridDirectionType.BottomRight:
                        break;
                }


                num = Math.Max(num, 1);
                if (horz && keyMoveRowIndex != -1)
                    currentRow = keyMoveRowIndex;
                if (vert && keyMoveColumnIndex != -1)
                    currentCol = keyMoveColumnIndex;

                int newRowIndex = currentRow;
                int newColumnIndex = currentCol;

                // Range spanned by covered cell
                GridRangeInfo coveredRange;
                int nRows = 0, nCols = 0;
                if (gridModel.CoveredRanges.Find(currentRow, currentCol, out coveredRange))
                {
                    nRows = coveredRange.Bottom - coveredRange.Top;
                    nCols = coveredRange.Right - coveredRange.Left;
                }

                switch (direction)
                {
                    #region GridDirectionType.Up
                    case GridDirectionType.Up:
                        vert = true;
                        while (num > 0)
                        {
                            if (scrollFrozen && (newRowIndex == grid.TopRowIndex || vscrollup > 0))
                            {
                                // If current cell is at the topmost nonfrozen row, scroll the view.
                                vscrollup++;
                                if (!grid.ScrollGridGetPrevRowIndex(ref newRowIndex))
                                    break;
                            }
                            else if (newRowIndex < regularCells.Top || vscrollup > 0)
                            {
                                // If there are frozen rows, move up one visible cell;
                                // if current cell is at the top row, scroll the view.
                                vscrollup++;
                                if (currentRow >= notFrozenCells.Top && notFrozenCells.Top > 0)
                                {
                                    newRowIndex = grid.GetRow(grid.GetClientRow(notFrozenCells.Top - 1)) - vscrollup;
                                }
                            }
                            else if (notFrozenCells.Top > regularCells.Top
                                && newRowIndex > grid.TopRowIndex
                                && newRowIndex <= grid.GetRow(grid.GetClientRow(notFrozenCells.Top - 1) + 1))
                            {
                                newRowIndex = grid.GetRow(grid.GetClientRow(newRowIndex) - 1);
                            }
                            else
                                newRowIndex = Math.Max(regularCells.Top, newRowIndex - 1);

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex))
                                break;

                            // Scrolling necessary when new row is not visible
                            // because it is between the frozen row and the
                            // top row.
                            if (newRowIndex >= notFrozenCells.Top && newRowIndex < grid.TopRowIndex)
                                vscrollup += grid.TopRowIndex - newRowIndex;

                            num--;
                            querySuccess = true;
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }

                        break;
                    #endregion

                    #region GridDirectionType.Down
                    case GridDirectionType.Down:
                        vert = true;
                        //gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                        while (num > 0)
                        {
                            // If there are frozen rows, move down one visible cell.
                            if (newRowIndex < notFrozenCells.Top)
                            {
                                newRowIndex += nRows;
                                grid.ScrollGridGetNextRowIndex(ref newRowIndex, false);
                                if (newRowIndex > notFrozenCells.Top)
                                    newRowIndex = grid.TopRowIndex;
                            }
                            else
                                newRowIndex = Math.Min(regularCells.Bottom, newRowIndex + 1);

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex))
                                break;

                            num--;
                            querySuccess = true;
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.Left
                    case GridDirectionType.Left:
                        while (num > 0)
                        {
                            if (scrollFrozen && (newColumnIndex == grid.LeftColumnIndex || hscrollleft > 0))
                            {
                                // If current cell is at the topmost nonfrozen Column, scroll the view.
                                hscrollleft++;
                                if (!grid.ScrollGridGetPrevColIndex(ref newColumnIndex))
                                    break;
                            }
                            else if (newColumnIndex <= regularCells.Left || hscrollleft > 0)
                            {
                                // If there are frozen Columns, move up one visible cell;
                                // if current cell is at the Left Column, scroll the view.
                                hscrollleft++;
                                if (currentCol >= notFrozenCells.Left && notFrozenCells.Left > 0)
                                {
                                    var colIdx = grid.GetClientCol(notFrozenCells.Left - 1);
                                    if (colIdx > -1)
                                    {
                                        newColumnIndex = grid.GetCol(colIdx) - hscrollleft;
                                    }
                                    else
                                    {
                                        newColumnIndex = colIdx;
                                    }
                                }
                            }
                            else if (notFrozenCells.Left > regularCells.Left
                                && newColumnIndex >= grid.LeftColumnIndex
                                && newColumnIndex < grid.GetCol(grid.GetClientCol(notFrozenCells.Left - 1) + 1))
                            {
                                newColumnIndex = grid.GetCol(grid.GetClientCol(newColumnIndex) - 1);
                            }
                            else
                                newColumnIndex = Math.Max(regularCells.Left, newColumnIndex - 1);

                            if (newColumnIndex < 0 || newRowIndex < 0)
                                break;

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex))
                                break;

                            // Scrolling becomes necessary when new column is not visible
                            // because it is between the frozen columns and the
                            // left column.
                            if (newColumnIndex >= notFrozenCells.Left && newRowIndex < grid.LeftColumnIndex)
                                hscrollleft += grid.LeftColumnIndex - newColumnIndex;

                            num--;
                            querySuccess = true;
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.Right
                    case GridDirectionType.Right:
                        //gridModel.RaiseQueryMaximumRowCol(0, gridModel.ColCount);
                        while (num > 0)
                        {
                            // If there are frozen rows, move one visible cell to the right.
                            if (newColumnIndex < notFrozenCells.Left)
                            {
                                newColumnIndex += nCols;
                                grid.ScrollGridGetNextColIndex(ref newColumnIndex, false);
                                if (newColumnIndex > notFrozenCells.Left)
                                    newColumnIndex = grid.LeftColumnIndex;
                            }
                            else
                            {
                                newColumnIndex = Math.Min(regularCells.Right, newColumnIndex + nCols + 1);
                            }

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex))
                                break;

                            num--;
                            querySuccess = true;
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.PageDown
                    case GridDirectionType.PageDown:
                        vert = true;
                        //gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);

                        if (grid.TopRowIndex < regularCells.Bottom)
                        {
                            newRowIndex = grid.ScrollRows.GetNextPage(newRowIndex);
                            newRowIndex = Math.Min(regularCells.Bottom, newRowIndex);

                            // Find the nearest possible cell (first down. then up).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                            if (!querySuccess)
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);

                            if (querySuccess)
                            {
                                if (newRowIndex > grid.ScrollCellsRange.Bottom && targetRowIndex <= grid.ScrollCellsRange.Bottom)
                                    grid.ScrollToBottom();

                                targetRowIndex = newRowIndex;
                                targetColumnIndex = newColumnIndex;
                            }
                        }
                        break;
                    #endregion

                    #region GridDirectionType.PageUp

                    case GridDirectionType.PageUp:
                        vert = true;
                        if (grid.TopRowIndex >= regularCells.Top)
                        {
                            newRowIndex = grid.ScrollRows.GetPreviousPage(newRowIndex);
                            newRowIndex = Math.Max(regularCells.Top, newRowIndex);

                            // Find the nearest possible cell (first up. then down).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);

                            if (!querySuccess)
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);

                            if (querySuccess)
                            {
                                if (newRowIndex < grid.ScrollCellsRange.Top && targetRowIndex >= grid.ScrollCellsRange.Top)
                                    grid.ScrollToTop();

                                targetRowIndex = newRowIndex;
                                targetColumnIndex = newColumnIndex;
                            }
                        }
                        break;
                    #endregion

                    #region GridDirectionType.PageRight
                    case GridDirectionType.PageRight:
                        vert = true;
                        //gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                        if (grid.LeftColumnIndex < regularCells.Right)
                        {
                            newColumnIndex = grid.ScrollColumns.GetNextPage(newColumnIndex);
                            newColumnIndex = Math.Min(regularCells.Right, newColumnIndex);

                            // Find the nearest possible cell (first right. then up).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                            if (!querySuccess)
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);

                            if (querySuccess)
                            {
                                if (newColumnIndex > grid.ScrollCellsRange.Right && targetColumnIndex <= grid.ScrollCellsRange.Right)
                                    grid.ScrollToRightEnd();

                                targetRowIndex = newRowIndex;
                                targetColumnIndex = newColumnIndex;
                            }
                        }
                        break;
                    #endregion

                    #region GridDirectionType.PageLeft
                    case GridDirectionType.PageLeft:
                        vert = true;
                        if (grid.LeftColumnIndex > regularCells.Left)
                        {
                            newColumnIndex = grid.ScrollColumns.GetPreviousPage(newColumnIndex);
                            newColumnIndex = Math.Max(regularCells.Left, newColumnIndex);

                            // Find the nearest possible cell (first left. then down).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newColumnIndex, ref newColumnIndex);

                            if (!querySuccess)
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newColumnIndex, ref newColumnIndex);

                            if (querySuccess)
                            {
                                if (newColumnIndex < grid.ScrollCellsRange.Left && targetColumnIndex >= grid.ScrollCellsRange.Left)
                                    grid.ScrollToLeftEnd();

                                targetRowIndex = newRowIndex;
                                targetColumnIndex = newColumnIndex;
                            }
                        }
                        break;
                    #endregion

                    #region GridDirectionType.MostLeft
                    case GridDirectionType.MostLeft:
                        hscrollleft = 1;
                        newColumnIndex = regularCells.Left;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToLeftEnd();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.MostRight
                    case GridDirectionType.MostRight:
                        newColumnIndex = regularCells.Right;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToRightEnd();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.TopLeft
                    case GridDirectionType.TopLeft:
                        newColumnIndex = regularCells.Left;
                        newRowIndex = regularCells.Top;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToTop();
                            grid.ScrollToLeftEnd();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    #region GridDirectionType.BottomRight

                    case GridDirectionType.BottomRight:
                        newColumnIndex = regularCells.Right;
                        newRowIndex = regularCells.Bottom;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);
                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToBottom();
                            grid.ScrollToRightEnd();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;

                    #endregion

                    #region GridDirectionType.Top
                    case GridDirectionType.Top:
                        vert = true;
                        vscrollup = 1;
                        newRowIndex = regularCells.Top;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToTop();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;

                    #endregion

                    #region GridDirectionType.Bottom
                    case GridDirectionType.Bottom:
                        vert = true;
                        newRowIndex = regularCells.Bottom;
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);
                        if (querySuccess)
                        {
                            grid.ScrollToBottom();
                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                        break;
                    #endregion

                    default:
                        throw new InvalidEnumArgumentException("direction");
                }

                bool success = false;

                bool isShiftKey = ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None);
                bool isCtrlKey = ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None);
                // position edit cell
                if (querySuccess &&
                    (targetRowIndex != currentRow && targetRowIndex >= regularCells.Top
                    || targetColumnIndex != currentCol && targetColumnIndex >= regularCells.Left
                    || !hasCurrentCell))
                {
                    try
                    {
                        // EXCELCURCELL
                        if (gridModel.Options.ExcelLikeCurrentCell
                            && allowSelectRange
                            )
                        {
                            if (!horz && gridModel.Selections.Ranges.Count > 0
                                && (gridModel.Selections.Ranges.AnyRangeContains(GridRangeInfo.Cell(targetRowIndex, targetColumnIndex)) 
                                || (gridModel.Options.ListBoxSelectionMode != GridSelectionMode.None && 
                                gridModel.Options.ListBoxSelectionMode != GridSelectionMode.MultiExtended)) && (direction!= GridDirectionType.Left && (direction!= GridDirectionType.Right)))
                                grid.Model.Selections.Clear();
                        }
                        else if ((isShiftKey || isCtrlKey) && (gridModel.Selections.Ranges.AnyRangeContains(GridRangeInfo.Cell(targetRowIndex, targetColumnIndex))))
                            grid.Model.Selections.Clear();

                        bool savedActive = this.HasCurrentCell;

                        int _targetRowIndex = targetRowIndex;
                        int _targetColumnIndex = targetColumnIndex;
                        this.AdjustRowColIfCoveredCell(ref _targetRowIndex, ref _targetColumnIndex);

                        if (_targetColumnIndex < regularCells.Left)
                            _targetColumnIndex = regularCells.Left;

                        if (this.MoveTo(_targetRowIndex, _targetColumnIndex, options))
                        {
                            //if (vscrollleft > 0 && _targetColumnIndex < grid.LeftColumnIndex)
                            //    grid.LeftColumnIndex = Math.Max(_targetColumnIndex, grid.GetFirstScrollableCol());

                            //if (vscrollup > 0 && _targetRowIndex < grid.TopRowIndex)
                            //    grid.TopRowIndex = Math.Max(_targetRowIndex, grid.GetFirstScrollableRow());

                            // Need to be aware here of two special cases:
                            // 1) User could have overriden CurrentCellMoving
                            // and redirected the targetRowIndex and targetColumnIndex.
                            // 2) End-User could be stepping through a covered range.
                            // Then targetRowIndex and targetColumnIndex will point to
                            // upper-left corner but next time you hit arrow-key the
                            // current cell should stay in same column or row as it
                            // was before.
                            if (!savedActive || grid.Model.CoveredRanges.FindRange(rowIndex, columnIndex).IsEmpty)
                            {
                                keyMoveRowIndex = this.rowIndex;
                                keyMoveColumnIndex = this.columnIndex;
                            }
                            // next two handle CoveredCells
                            else if (vert)
                            {
                                keyMoveRowIndex = this.rowIndex;
                                keyMoveColumnIndex = targetColumnIndex;
                            }
                            else
                            {
                                keyMoveRowIndex = targetRowIndex;
                                keyMoveColumnIndex = this.columnIndex;
                            }
                            success = true;
                        }
                    }
                    finally
                    {
                    }
                }
                else
                {
                    while (hscrollleft-- > 0)
                        grid.LineLeft();

                    while (vscrollup-- > 0)
                        grid.LineUp();
                }

                return success;
            }

            return false;
        }

        private GridOrientation GetMovement(int targetRowIndex, int targetColumnIndex)
        {
            GridOrientation direction = GridOrientation.None;
            if (targetRowIndex < 0 || targetColumnIndex < 0)
                direction = HasCurrentCell ? GridOrientation.None : GridOrientation.Both;
            else if (IsNotNavigateColumn(targetColumnIndex) || IsNotNavigateRow(targetRowIndex))
                direction = GridOrientation.None;
            else
            {
                if (!HasCurrentCell)
                    direction = GridOrientation.Both;
                else
                {
                    if (rowIndex != targetRowIndex)
                        direction |= GridOrientation.Vertical;
                    if (columnIndex != targetColumnIndex)
                        direction |= GridOrientation.Horizontal;
                }
            }
            return direction;
        }


        public bool QueryNextEnabledCellCheck(GridDirectionType direction, ref int row, ref int col)
        {
            GridRangeInfo gridCells = grid.NavigateWithArrowKeysCellsRange;
            int oldrow = row;
            int oldcol = col;
            int lineSize;
            bool foundEnabled = false;
            if (direction == GridDirectionType.Right)
            {
                while (!foundEnabled && row > -1 && row <= this.gridModel.RowCount)
                {
                    while (!foundEnabled && col > -1 && col < this.gridModel.ColumnCount)
                    {
                        col++;
                        if (row < this.gridModel.RowCount && col < this.gridModel.ColumnCount)
                            foundEnabled = this.gridModel[row, col].Enabled;

                        if (foundEnabled && (this.gridModel.RowHeights.GetHidden(row, out lineSize) || this.gridModel.ColumnWidths.GetHidden(col, out lineSize)))   // Sd 7297 - Checking enabled cells is hidden. 
                        {
                            foundEnabled = false;
                        }
                    }
                    if (!foundEnabled)
                    {
                        col = gridCells.Left;
                        row++;

                        if (row < this.gridModel.RowCount && col < this.gridModel.ColumnCount)
                            foundEnabled = this.gridModel[row, col].Enabled;

                        if (foundEnabled && (this.gridModel.RowHeights.GetHidden(row, out lineSize) || this.gridModel.ColumnWidths.GetHidden(col, out lineSize)))   // Sd 7297 - Checking enabled cells is hidden. 
                        {
                            foundEnabled = false;
                        }
                    }
                }
            }
            else if (direction == GridDirectionType.Left)
            {
                while (!foundEnabled && row > gridCells.Top && row <= this.gridModel.RowCount)
                {
                    while (!foundEnabled && col > gridCells.Left && col < this.gridModel.ColumnCount)
                    {
                        col--;
                        if (row < this.gridModel.RowCount && col < this.gridModel.ColumnCount)
                            foundEnabled = this.gridModel[row, col].Enabled;

                        if (foundEnabled && (this.gridModel.RowHeights.GetHidden(row, out lineSize) || this.gridModel.ColumnWidths.GetHidden(col, out lineSize)))   // Sd 7297 - Checking enabled cells is hidden. 
                        {
                            foundEnabled = false;
                        }
                    }
                    if (!foundEnabled)
                    {
                        col = gridCells.Right;
                        row--;
                    }
                }
            }
            if (!foundEnabled)
            {
                row = oldrow;
                col = oldcol;
            }
            return foundEnabled;

        }

        /// <summary>
        /// Determines the next enabled cell when current cell wants to move into a given direction. Cells that are not
        /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped. The search in a given
        /// direction is aborted when the row or column index hits the boundaries of the
        /// <see cref="GridControlBase.NavigateWithArrowKeysCellsRange"/> range.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>
        /// True if an enabled cell was found; False otherwise.
        /// </returns>
        /// <remarks>
        /// This method will not raise the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event. Instead
        /// you can call this method from your QueryNextMoveCell event handler to find out about the next
        /// enabled cell and then decide on further criteria if the suggested cell is good.
        /// <para/>
        /// You should call <see cref="GridControlBase.GetNextCurrentCellPosition"/> instead if you want
        /// the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event to be raised.
        /// </remarks>
        public bool QueryNextEnabledCell(GridDirectionType direction, ref int rowIndex, ref int columnIndex)
        {
            GridRangeInfo gridCells = grid.NavigateWithArrowKeysCellsRange;
            int targetRow = rowIndex;
            int targetCol = columnIndex;
            bool enabled = false;

            GridRangeInfo coveredRange = null;
            GridRenderStyleInfo style;

            switch (direction)
            {
                case GridDirectionType.Up:
                    while (targetRow >= gridCells.Top && !enabled)
                    {
                        // Skip invisible and covered cells.
                        while (targetRow >= gridCells.Top
                            && (grid.GetRowHeight(targetRow) == 0
                            || gridModel.CoveredCells.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Top != targetRow
                            )
                            )
                            targetRow = grid.ScrollRows.GetPreviousScrollLineIndex(targetRow);

                        if (targetRow >= gridCells.Top)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            // Not enabled, continue search.
                            if (!enabled)
                                targetRow = grid.ScrollRows.GetPreviousScrollLineIndex(targetRow);
                        }
                    }
                    break;

                case GridDirectionType.Down:
                    while (targetRow <= gridCells.Bottom && !enabled)
                    {
                        // Skip invisible and covered cells.
                        while (targetRow <= gridCells.Bottom
                            && targetRow > -1
                            && (grid.GetRowHeight(targetRow) == 0
                            || gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Top != targetRow
                            )
                            )
                            targetRow = grid.ScrollRows.GetNextScrollLineIndex(targetRow);

                        if (targetRow <= gridCells.Bottom && targetRow > -1)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            // Not enabled, continue search.
                            if (!enabled)
                                targetRow = grid.ScrollRows.GetNextScrollLineIndex(targetRow);
                        }

                        if (targetRow == -1)
                        {
                            break;
                        }
                    }
                    break;

                case GridDirectionType.Left:
                    while (targetCol >= gridCells.Left && !enabled)
                    {
                        // Skip invisible and covered cells.
                        while (targetCol >= gridCells.Left
                            && (grid.GetColWidth(targetCol) == 0
                            || gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Left != targetCol))
                            targetCol = grid.ScrollColumns.GetPreviousScrollLineIndex(targetCol);

                        if (targetCol < gridCells.Left)
                            targetCol = gridCells.Left;

                        if (targetCol >= gridCells.Left)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                            int rc;
                            var isHidden = gridModel.RowHeights.GetHidden(rowIndex, out rc);
                            if (isHidden)
                                targetRow = grid.ScrollRows.GetPreviousScrollLineIndex(rowIndex);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            // Not enabled, continue search.
                            if (!enabled)
                                targetCol = grid.ScrollColumns.GetPreviousScrollLineIndex(targetCol);
                        }
                    }
                    break;

                case GridDirectionType.Right:
                    var moveNextRow = false;
                    while (targetCol <= gridCells.Right && !enabled)
                    {
                        // Skip invisible and covered cells.
                        while (targetCol <= gridCells.Right
                            && (grid.GetColWidth(targetCol) == 0
                            || gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Left != targetCol && !moveNextRow))
                        {
                            targetCol = grid.ScrollColumns.GetNextScrollLineIndex(targetCol);
                            if (targetCol == -1 && targetRow < grid.ScrollRows.LineCount - 1)
                            {
                                // we reached the end move to next row
                                targetRow = grid.ScrollRows.GetNextScrollLineIndex(targetRow);
                                targetCol = gridCells.Left;
                                moveNextRow = true;
                            }
                            else if (coveredRange !=null && (coveredRange.Width == grid.Model.ColumnCount - 1) && (targetRow < grid.ScrollRows.LineCount - 1))
                                return false;
                            else
                            {
                                break;
                            }
                        }

                        if (moveNextRow)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();
                            break;
                        }

                        if (targetCol <= gridCells.Right)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            // Not enabled, continue search.
                            if (!enabled || this.CanMove(style))
                                targetCol = grid.ScrollColumns.GetNextScrollLineIndex(targetCol);
                        }
                    }
                    break;
            }

            if (enabled)
            {
                rowIndex = targetRow;
                columnIndex = targetCol;
            }
            else if (targetCol <= gridModel.ColumnCount)
            {
                //return QueryNextEnabledCellCheck(GridDirectionType.Right, ref rowIndex, ref columnIndex);

                var result = QueryNextEnabledCellCheck(direction, ref targetRow, ref targetCol);                
                if (result && this.grid.Model.RowHeights[targetRow] == 0)
                {
                    rowIndex++;
                    columnIndex = 0;
                    return QueryNextEnabledCell(direction, ref  targetRow, ref  targetCol);
                }
                else if (result)
                {
                    rowIndex = targetRow;
                    columnIndex = targetCol;
                    return true;
                }
                else
                {
                    return result;
                }
            }

            return enabled;
        }

        private bool CanMove(GridRenderStyleInfo style)
        {
            GridDataStyleInfo GridDataStyle = style.ModelStyle as GridDataStyleInfo;
            if (GridDataStyle != null && GridDataStyle.CellIdentity != null && GridDataStyle.CellIdentity.TableCellType == GridDataTableCellType.EmptyCell)
                return true;
            return false;
        }

        private bool IsNotNavigateRow(int targetRowIndex)
        {
            GridRangeInfo gridCells = grid.NavigateWithArrowKeysCellsRange;
            return targetRowIndex < gridCells.Top
                || targetRowIndex > gridCells.Bottom;
        }

        private bool IsNotNavigateColumn(int targetColumnIndex)
        {
            GridRangeInfo gridCells = grid.NavigateWithArrowKeysCellsRange;
            return targetColumnIndex < gridCells.Left
                || targetColumnIndex > gridCells.Right;
        }

        #endregion

        /// <summary>
        /// Adjusts the row index and column index if the cell belongs to a covered range. In that case, the top
        /// left cell coordinates are returned.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        internal void AdjustRowColIfCoveredCell(ref int rowIndex, ref int columnIndex)
        {
            CoveredCellInfo ci = grid.GetCoveredCell(rowIndex, columnIndex);
            if (ci != null)
            {
                rowIndex = ci.Top;
                columnIndex = ci.Left;
            }
        }

        /// <summary>
        /// Brings the current cell into view.
        /// </summary>
        public void ScrollInView()
        {
            if (HasCurrentCell)
            {
                grid.ScrollInView(CellRowColumnIndex);
            }
        }

        #region Insert and Remove Rows

        /// <summary>
        /// Insert a specified number of rows at a specified row index.
        /// </summary>
        /// <param name="insertAtRowIndex">The starting row index where new rows should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        /// <param name="moveState">The <see cref="GridCurrentCellMoveState"/>.</param>
        public void InsertRows(int insertAtRowIndex, int count, GridCurrentCellMoveState moveState)
        {
            if (moveFromRowIndex >= insertAtRowIndex)
                moveFromRowIndex += count;
            if (moveToRowIndex >= insertAtRowIndex)
                moveToRowIndex += count;
            if (keyMoveRowIndex >= insertAtRowIndex)
                keyMoveRowIndex += count;
            if (rowIndex >= insertAtRowIndex)
                rowIndex += count;

            if (moveState != null)
            {
                if (moveState.MoveFrom != -1)
                    moveFromRowIndex = moveState.MoveFrom + insertAtRowIndex;
                if (moveState.MoveTo != -1)
                    moveToRowIndex = moveState.MoveTo + insertAtRowIndex;
                if (moveState.Index != -1)
                    rowIndex = moveState.Index + insertAtRowIndex;
                if (moveState.KeyMove != -1)
                    keyMoveRowIndex = moveState.KeyMove + insertAtRowIndex;

            }
            if (Renderer != null)
                UpdateCellRowColumnIndex(new RowColumnIndex(rowIndex, columnIndex));
        }

        /// <summary>
        /// Removes a specified number of rows at a specified row index.
        /// </summary>
        /// <param name="removeAtRowIndex">The starting row index where columns should be removed.</param>
        /// <param name="count">The number of rows to remove.</param>
        /// <param name="moveState">The <see cref="GridCurrentCellMoveState"/>.</param>
        public void RemoveRows(int removeAtRowIndex, int count, GridCurrentCellMoveState moveState)
        {
            if (moveFromRowIndex >= removeAtRowIndex)
            {
                if (moveFromRowIndex >= removeAtRowIndex + count)
                    moveFromRowIndex -= count;
                else if (moveState != null)
                    moveState.MoveFrom = moveFromRowIndex - removeAtRowIndex;
                else
                    moveFromRowIndex = -1;
            }
            if (moveToRowIndex >= removeAtRowIndex)
            {
                if (moveToRowIndex >= removeAtRowIndex + count)
                    moveToRowIndex -= count;
                else if (moveState != null)
                    moveState.MoveTo = moveToRowIndex - removeAtRowIndex;
                else
                    moveToRowIndex = -1;
            }
            if (keyMoveRowIndex >= removeAtRowIndex)
            {
                if (keyMoveRowIndex >= removeAtRowIndex + count)
                    keyMoveRowIndex -= count;
                else if (moveState != null)
                    moveState.KeyMove = keyMoveRowIndex - removeAtRowIndex;
                else
                    keyMoveRowIndex = -1;
            }
            if (rowIndex >= removeAtRowIndex)
            {
                if (rowIndex >= removeAtRowIndex + count)
                {
                    rowIndex -= count;
                    if (Renderer != null)
                        UpdateCellRowColumnIndex(new RowColumnIndex(rowIndex, columnIndex));
                }
                else if (moveState != null)
                    moveState.Index = rowIndex - removeAtRowIndex;
                else
                    rowIndex = -1;
            }
            if (rowIndex == -1)
                this.ResetCache();
        }

        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Insert a specified number of columns at a specified column index.
        /// </summary>
        /// <param name="insertAtColumnIndex">The starting column index where new columns should be inserted.</param>
        /// <param name="count">The number of columns to insert.</param>
        /// <param name="moveState">The <see cref="GridCurrentCellMoveState"/>.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, GridCurrentCellMoveState moveState)
        {
            if (moveFromColumnIndex >= insertAtColumnIndex)
                moveFromColumnIndex += count;
            if (moveToColumnIndex >= insertAtColumnIndex)
                moveToColumnIndex += count;
            if (keyMoveColumnIndex >= insertAtColumnIndex)
                keyMoveColumnIndex += count;
            if (columnIndex >= insertAtColumnIndex)
                columnIndex += count;

            if (moveState != null)
            {
                if (moveState.MoveFrom != -1)
                    moveFromColumnIndex = moveState.MoveFrom + insertAtColumnIndex;
                if (moveState.MoveTo != -1)
                    moveToColumnIndex = moveState.MoveTo + insertAtColumnIndex;
                if (moveState.Index != -1)
                    columnIndex = moveState.Index + insertAtColumnIndex;
                if (moveState.KeyMove != -1)
                    keyMoveColumnIndex = moveState.KeyMove + insertAtColumnIndex;

            }
            if (Renderer != null)
                UpdateCellRowColumnIndex(new RowColumnIndex(rowIndex, columnIndex));
        }

        /// <summary>
        /// Removes a specified number of columns at a specified column index.
        /// </summary>
        /// <param name="removeAtColumnIndex">The starting column index where columns should be removed.</param>
        /// <param name="count">The number of columns to remove.</param>
        /// <param name="moveState">The <see cref="GridCurrentCellMoveState"/>.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, GridCurrentCellMoveState moveState)
        {
            if (moveFromColumnIndex >= removeAtColumnIndex)
            {
                if (moveFromColumnIndex >= removeAtColumnIndex + count)
                    moveFromColumnIndex -= count;
                else if (moveState != null)
                    moveState.MoveFrom = moveFromColumnIndex - removeAtColumnIndex;
                else
                    moveFromColumnIndex = -1;
            }
            if (moveToColumnIndex >= removeAtColumnIndex)
            {
                if (moveToColumnIndex >= removeAtColumnIndex + count)
                    moveToColumnIndex -= count;
                else if (moveState != null)
                    moveState.MoveTo = moveToColumnIndex - removeAtColumnIndex;
                else
                    moveToColumnIndex = -1;
            }
            if (keyMoveColumnIndex >= removeAtColumnIndex)
            {
                if (keyMoveColumnIndex >= removeAtColumnIndex + count)
                    keyMoveColumnIndex -= count;
                else if (moveState != null)
                    moveState.KeyMove = keyMoveColumnIndex - removeAtColumnIndex;
                else
                    keyMoveColumnIndex = -1;
            }
            if (columnIndex >= removeAtColumnIndex)
            {
                if (columnIndex >= removeAtColumnIndex + count)
                {
                    columnIndex -= count;
                    if (Renderer != null)
                        UpdateCellRowColumnIndex(new RowColumnIndex(rowIndex, columnIndex));
                }
                else if (moveState != null)
                    moveState.Index = columnIndex - removeAtColumnIndex;
                else
                {
                    rowIndex = -1;
                    columnIndex = -1;
                }
            }
            if (columnIndex == -1)
                cellRenderer = null;
        }
        #endregion


        private int suspendEvents = 0;

        /// <summary>
        /// Suspends raising events.
        /// </summary>
        public void SuspendEvents()
        {
            suspendEvents++;
        }


        /// <summary>
        /// Resumes raising events.
        /// </summary>
        public void ResumeEvents()
        {
            if (suspendEvents > 0)
                suspendEvents--;
        }

        /// <summary>
        /// Checks if raising events is temporarily disabled.
        /// </summary>
        public bool IsSuspendEvents
        {
            get
            {
                return suspendEvents > 0;
            }
        }

        public void Dispose()
        {
            this.grid = null;
        }
    }

    /// <summary>
    /// Represents the state of the current cell's movement.
    /// </summary>
    public class GridCurrentCellMoveState
    {
        /// <summary>
        /// Index from which the current cell is moved.
        /// </summary>
        public int MoveFrom = -1;
        /// <summary>
        /// Index to which the current cell is moved.
        /// </summary>
        public int MoveTo = -1;
        /// <summary>
        /// Current cell index.
        /// </summary>
        public int Index = -1;
        /// <summary>
        /// Specifies if the current cell's movement is caused by a key press.
        /// </summary>
        public int KeyMove = -1;
    }


}
