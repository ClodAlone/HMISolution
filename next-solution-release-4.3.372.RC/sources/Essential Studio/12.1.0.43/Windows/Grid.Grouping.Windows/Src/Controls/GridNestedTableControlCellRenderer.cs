//-------------------------------------------------------------------------------------------------
// <copyright file="GridNestedTableControlCellRenderer.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#define optimizeAggressive
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// GridNestedTableControlCellRenderer creates a GridTableControl as a view for the
    /// GridNestedTableControlCellModel.TableModel. The GridTableControl is drawn static
    /// using the GridTableControl.DrawGrid method. GridTableControl is being used in
    /// a windowless mode. All mouse and keyboard interaction is forwarded from a
    /// parent grid control to the nested table control. The table control has no
    /// window handle.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// You typically access cell models through the <see cref="GridControlBase.CellRenderers"/>
    /// property of the <see cref="GridControlBase"/> class. A nested table control cell renderer
    /// is identified through its parent relations name with an "RT" prefix.
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    ///             string cellType = "RT" + relatedTable.TableDescriptor.Name;
    ///             GridNestedTableControlCellRenderer cm = this.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
    /// </code>
    /// <code lang="VB">
    /// <para/>
    ///         Dim cellType As String = "RT" + relatedTable.TableDescriptor.Name
    ///         Dim cm As GridNestedTableControlCellRenderer = Me.CellRenderers(cellType)
    /// </code>
    /// </example>
    public class GridNestedTableControlCellRenderer : GridStaticCellRenderer
    {
        static MouseEventArgs emptyMouseEventArgs = new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0);

        GridNestedTableControlCellModel cellTableModel;
        GridNestedTableControl renderControl;
        bool forceNestedTableAndRestore = true;
        bool scrolled = false;

        internal static bool tracing = false;

        /// <summary>
        /// Initializes a new <see cref="GridNestedTableControlCellRenderer"/> object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer.</param>
        /// <remarks>
        /// References to GridControlBase
        /// and GridCellModelBase will be saved.
        /// <para/>
        /// You typically access cell models through the <see cref="GridControlBase.CellRenderers"/>
        /// property of the <see cref="GridControlBase"/> class. A nested table control cell renderer
        /// is identified through its parent relations name with an "RT" prefix.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///             string cellType = "RT" + relatedTable.TableDescriptor.Name;
        ///             GridNestedTableControlCellRenderer cm = this.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
        /// </code>
        /// <code lang="VB">
        /// <para/>
        ///         Dim cellType As String = "RT" + relatedTable.TableDescriptor.Name
        ///         Dim cm As GridNestedTableControlCellRenderer = Me.CellRenderers(cellType)
        /// </code>
        /// </example>
        public GridNestedTableControlCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.cellTableModel = (GridNestedTableControlCellModel)Model;
            this.SupportsFocusControl = true;

            this.renderControl = Grid.GroupingControl.CreateNestedTableControl(cellTableModel.RelatedTableModel, Grid, this);
            this.renderControl.IsWindowless = true;
            this.renderControl.ParentSite = this.Grid;
            this.renderControl.Initialize();
            this.renderControl.ThemesEnabled = true;
            /*            renderControl.CurrentCellChanging += new CancelEventHandler(renderControl_CurrentCellChanging);
            ////            renderControl.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(renderControl_CurrentCellActivating);
            ////            renderControl.CurrentCellActivated += new EventHandler(renderControl_CurrentCellActivated);
            ////            renderControl.CurrentCellStartEditing += new CancelEventHandler(renderControl_CurrentCellStartEditing);
            ////            renderControl.CurrentCellMoving += new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
            ////            renderControl.MouseActivating += new CancelEventHandler(renderControl_MouseActivating);
            ////            renderControl.CurrentCellMoved += new GridCurrentCellMovedEventHandler(renderControl_CurrentCellMoved);
            ////            renderControl.Model.ColWidthsChanged += new GridRowColSizeChangedEventHandler(renderControl_ColWidthsChanged);*/
            this.SetControl(this.renderControl);
            grid.GetGridWindow().WindowScrolling += new ScrollWindowEventHandler(this.GridNestedTableControlCellRenderer_WindowScrolling);

            GridTable t = this.Model.RelatedTable;

            this.renderControl.GridControlBaseEventsTarget = new GridGroupingControl.GridControlBaseEventsTarget(this.renderControl, t.Engine.ParentControl as GridGroupingControl);

            this.Model.RelatedTableModel.ActiveGridView = this.renderControl;
            this.tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Model.Grid.Table.ToString(), this.Model.ToString(), tableId);
            }
        }

        void GridNestedTableControlCellRenderer_WindowScrolling(object sender, ScrollWindowEventArgs e)
        {
            this.scrolled = true;
        }

        static int tableCounter = 0;
        int tableId = 0;

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            return GetType().Name + " { " + (this.Model != null ? this.Model.ToString() : string.Empty) + isdisposed + " }";
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.tableId);
            }

            if (disposing)
            {
                this.SetControl(null);
                ////                renderControl.CurrentCellChanging -= new CancelEventHandler(renderControl_CurrentCellChanging);
                ////                renderControl.CurrentCellActivating -= new GridCurrentCellActivatingEventHandler(renderControl_CurrentCellActivating);
                ////                renderControl.CurrentCellActivated -= new EventHandler(renderControl_CurrentCellActivated);
                ////                renderControl.CurrentCellStartEditing -= new CancelEventHandler(renderControl_CurrentCellStartEditing);
                ////                renderControl.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
                ////                renderControl.MouseActivating -= new CancelEventHandler(renderControl_MouseActivating);
                ////                renderControl.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(renderControl_CurrentCellMoved);
                ////                renderControl.Model.ColWidthsChanged -= new GridRowColSizeChangedEventHandler(renderControl_ColWidthsChanged);
                if (this.Grid != null)
                {
                    this.Grid.GetGridWindow().WindowScrolling -= new ScrollWindowEventHandler(GridNestedTableControlCellRenderer_WindowScrolling);
                }

                if (this.renderControl != null)
                {
                    this.renderControl.GridControlBaseEventsTarget.Dispose();
                    this.renderControl.Dispose();
                    this.renderControl = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Strong Typed Property Overrides
        /// <summary>
        /// The inner <see cref="GridNestedTableControl"/> that draws the nested table.
        /// </summary>
        public new GridNestedTableControl Control
        {
            get
            {
                return this.renderControl;
            }
        }

        /// <summary>
        /// The parent <see cref="GridTableControl"/> that this cell renderer belongs to.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }

        /// <summary>
        /// The <see cref="GridNestedTableControlCellModel"/> that this cell renderer belongs to.
        /// </summary>
        public new GridNestedTableControlCellModel Model
        {
            get
            {
                return (GridNestedTableControlCellModel)base.Model;
            }
        }

        #endregion

        /// <override/>
        /// <summary>Called from OnVScroll, OnHScroll before grid is scrolled.</summary>
        /// <param name="pMsg">The Message.</param>
        public override void OnNotifyMsg(ref Message pMsg)
        {
            if (this.Grid.InSynchronizeGridWithEngine)
            {
                return;
            }

            switch (pMsg.Msg)
            {
                case 0x0115: //// WM_VSCROLL
                case 0x0114: //// WM_HSCROLL:
                    this.Grid.GetNestedCurrentCell().CloseDropDown(PopupCloseType.Canceled);
                    this.scrolled = true;
                    break;
            }

            base.OnNotifyMsg(ref pMsg);
        }

        /// <override/>
        /// <summary>
        /// Returns a nested current cell if this cell type hosts a GridControl by itself.
        /// </summary>
        /// <returns>Current cell.</returns>
        public override GridCurrentCell GetNestedCurrentCell()
        {
            GridNestedTable nestedTable = this.Grid.Table.CurrentElement as GridNestedTable;
            GridChildTable childTable = null;
            while (nestedTable != null)
            {
                childTable = nestedTable.ChildTable;
                nestedTable = childTable.ParentTable.CurrentElement as GridNestedTable;
            }

            if (childTable != null)
            {
                return childTable.CurrentCell;
            }

            return null;
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <returns>returns the GridCurrentCell.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridCurrentCell GetInnerCurrentCell()
        {
            GridNestedTable nestedTable = this.Grid.Table.CurrentElement as GridNestedTable;
            GridChildTable childTable = null;
            if (nestedTable != null)
            {
                childTable = nestedTable.ChildTable;
            }

            if (childTable != null)
            {
                return childTable.CurrentCell;
            }

            return null;
        }

        #region NestedTable Context

        internal GridSwitchNestedTableAndRestoreHelper SaveCurrentNestedTableAndRestore()
        {
            if (this.renderControl == null)
            {
                return null;
            }

            ChildTable filteredChildTable = this.renderControl.Table.FilteredChildTableOrTopLevelGroup;
            GridNestedTable nt = null;

            if (filteredChildTable != null)
            {
                nt = (GridNestedTable)filteredChildTable.ParentNestedTable;
                if ((this.Grid.Model != null && this.Grid.Model.ActiveGridView != null) && (this.Grid.MarkRowHeader || this.Grid.MarkColHeader))
                {
                    this.Grid.Model.ActiveGridView.BeginUpdate();
                    this.Grid.Model.ActiveGridView.UpdateStyles();
                    this.Grid.Model.ActiveGridView.EndUpdate(true);
                    if (nt.ChildTableGroupLevel > 0)
                    {
                        this.Grid.CurrentCell.Refresh();
                    }
                }
               
            }

            return new GridSwitchNestedTableAndRestoreHelper(this, nt);
        }

        /// <internalonly/>
        /// <summary>
        /// Lets you switch the context of the nested control. GridBounds, Table.FilteredChildTable etc.
        /// will be adjusted.
        /// </summary>
        /// <param name="nestedTable">The nested table that the nested control should operate on.</param>
        /// <returns>A GridSwitchNestedTableAndRestoreHelper. When you call Dispose on this member
        /// the previous state will be restored.</returns>
        public GridSwitchNestedTableAndRestoreHelper SwitchNestedTableAndRestore(GridNestedTable nestedTable)
        {
            return new GridSwitchNestedTableAndRestoreHelper(this, this.SwitchNestedTable(nestedTable));
        }

        GridSwitchNestedTableAndRestoreHelper SwitchNestedTableAndRestore(int rowIndex, int colIndex)
        {
            return new GridSwitchNestedTableAndRestoreHelper(this, this.SwitchNestedTable(rowIndex, colIndex));
        }

        GridSwitchNestedTableAndRestoreHelper SwitchNestedTableAndRestore(int rowIndex, int colIndex, bool dontRestore)
        {
            GridNestedTable nestedTable = this.SwitchNestedTable(rowIndex, colIndex);
            if (dontRestore)
            {
                return null;
            }

            return new GridSwitchNestedTableAndRestoreHelper(this, nestedTable);
        }

        internal GridSwitchNestedTableAndRestoreHelper SwitchNestedTableAndRestore(GridNestedTable nestedTable, int rowIndex, int colIndex)
        {
            return new GridSwitchNestedTableAndRestoreHelper(this, this.SwitchNestedTable(nestedTable, rowIndex, colIndex));
        }

        GridNestedTable SwitchNestedTable(GridNestedTable nestedTable)
        {
            if (nestedTable == null)
            {
                return null;
            }
            if (nestedTable.ParentChildTable == null)
            {
                return null;
            }
            int rowIndex = nestedTable.ParentChildTable.DisplayElements.IndexOf(nestedTable);
            if (rowIndex == -1)
            {
                return null;
            }

            int colIndex = this.Grid.TableDescriptor.GetColumnIndentCount();
            return this.SwitchNestedTable(nestedTable, rowIndex, colIndex);
        }

        GridNestedTable SwitchNestedTable(int rowIndex, int colIndex)
        {
            // TODO: This should not happen, occurs when called from
            // GridTableClickCellsMouseController.MouseHover, reproduce with D:\Syncfusion\QaIssues\Grouping\14237\DSGrid\
            if (rowIndex >= this.Grid.Table.DisplayElements.Count)
            {
                Debug.WriteLine("Cached out of range issue in SwitchNestedTable.");
                return null;
            }

            GridNestedTable nestedTable = this.Grid.Table.DisplayElements[rowIndex] as GridNestedTable;
            if (nestedTable == null)
            {
                return null;
            }

            return this.SwitchNestedTable(nestedTable, rowIndex, colIndex);
        }

        GridNestedTable SwitchNestedTable(GridNestedTable nestedTable, int rowIndex, int colIndex)
        {
            /*
             * Correct: this happens when Draw is called on a different nested table than the current nested table
             * in the parent. That's why SwitchNestedTable above calls
             * nestedTable.ParentChildTable.DisplayElements.IndexOf(nestedTable)
             * and nestedTable.ParentChildTable.DisplayElements.Count is not necessarily the same as
             * Grid.Table.DisplayElements.Count (FilteredChildTable might be different)
            if (rowIndex >= Grid.Table.DisplayElements.Count)
            {
                Debug.WriteLine("Catched out of range issue in SwitchNestedTable");
                return null;
            }
            */

            if (nestedTable == null || rowIndex < 0 || colIndex < 0)
            {
                return null;
            }

            if (this.renderControl == null)
            {
                return null;
            }

            GridChildTable childTable = nestedTable.ChildTable;
            if (childTable.IsDisposed)
            {
                return null;
            }

            Table childParentTable = childTable.ParentTable;
            ////LL
            ChildTable filteredChildTable = this.Model.RelatedTable.FilteredChildTableOrTopLevelGroup;

            GridNestedTableControlState newContext = this.GetNestedTableContext(nestedTable);

            if ((newContext == null || !newContext.ApplyState()) && childTable == filteredChildTable)
            {
                return null;
            }

            if (this.forceNestedTableAndRestore && filteredChildTable != null)
            {
                return (GridNestedTable)filteredChildTable.ParentNestedTable;
            }

            return null;
        }

        GridNestedTableControlState GetNestedTableContext(int rowIndex, int colIndex)
        {
            GridNestedTable nestedTable = this.Grid.Table.DisplayElements[rowIndex] as GridNestedTable;
            if (rowIndex == -1)
            {
                return null;
            }

            return this.GetNestedTableContext(nestedTable, rowIndex, colIndex);
        }

        GridNestedTableControlState GetNestedTableContext(GridNestedTable nestedTable)
        {
            int rowIndex = nestedTable.ParentChildTable.DisplayElements.IndexOf(nestedTable);
            int colIndex = this.Grid.TableDescriptor.GetColumnIndentCount();
            return this.GetNestedTableContext(nestedTable, rowIndex, colIndex);
        }

        GridNestedTableControlState GetNestedTableContext(GridNestedTable nestedTable, int rowIndex, int colIndex)
        {
            if (nestedTable == null || rowIndex < 0 || colIndex < 0)
            {
                return null;
            }

            ////            if (nestedTable == null)
            ////                throw new ArgumentNullException("nestedTable");
            ////
            ////            if (rowIndex < 0 || rowIndex >= Grid.Table.DisplayElements.Count)
            ////                throw new ArgumentOutOfRangeException("rowIndex", String.Format("rowIndex {0} valid range: 0 to {1}", rowIndex, Grid.Table.DisplayElements.Count-1));
            ////
            ////            if (colIndex != Grid.TableDescriptor.GetColumnIndentCount())
            ////                throw new ArgumentOutOfRangeException("colIndex");
            if (!nestedTable.GetVisibleInHierarchy())
            {
                return null;
            }

            // Bounds of the tables visible scrollable cells
            GridRangeInfo scrollRange = GridRangeInfo.Rows(rowIndex, this.Grid.ViewLayout.LastVisibleRow + 1);
            Rectangle clipBounds = this.Grid.RangeInfoToRectangle(scrollRange, GridRangeOptions.None);

            int rowIndexDelta;
            ////Rectangle childGridClippedBounds = this.GetDrawCellBounds(Math.Max(Grid.TopRowIndex, rowIndex), colIndex, clipBounds, out rowIndexDelta);
            Rectangle childGridClippedBounds = this.GetDrawCellBounds(nestedTable, Math.Max(this.Grid.TopRowIndex, rowIndex), colIndex, clipBounds, out rowIndexDelta);

            GridNestedTableControlState drawCellState = new GridNestedTableControlState();
            drawCellState.RowIndex = rowIndex;
            drawCellState.ColIndex = colIndex;
            drawCellState.TableControl = this.renderControl;
            drawCellState.NestedTable = nestedTable;
            drawCellState.ChildTable = nestedTable.ChildTable;
            if (nestedTable.ChildTable != null)
            {
                drawCellState.Table = (GridTable)nestedTable.ChildTable.ParentTable;
            }

            drawCellState.RowIndexDelta = rowIndexDelta;
            drawCellState.GridBounds = childGridClippedBounds;

            return drawCellState;
        }

        ////Rectangle GetCurrentCellBounds(out int rowIndexDelta)
        ////{
        ////    // Bounds of the tables visible scrollable cells
        ////    GridRangeInfo scrollRange = Grid.ViewLayout.VisibleCellsRange;
        ////    Rectangle clipBounds = Grid.RangeInfoToRectangle(scrollRange, GridRangeOptions.None);

        ////    return GetDrawCellBounds(Math.Max(Grid.TopRowIndex, RowIndex), ColIndex, clipBounds, out rowIndexDelta);
        ////}

        ////Rectangle GetDrawCellBounds(int rowIndex, int colIndex, Rectangle clipBounds, out int rowIndexDelta)
        Rectangle GetDrawCellBounds(GridNestedTable nestedTable, int rowIndex, int colIndex, Rectangle clipBounds, out int rowIndexDelta)
        {
            // Complete bounds of the cell to be drawn (can be pretty large ...)
            GridRangeInfo rgCell = this.Grid.Model.CoveredRanges.FindRange(rowIndex, colIndex);
            Rectangle fullBounds = this.Grid.RangeInfoToRectangle(rgCell, GridRangeOptions.CalculateNonClientArea);

            // Fixes issue with scrolling of nested tables when customer did manually set Model.Cols.FrozenCount on parent table.
            ////if (colIndex <= Grid.Model.Cols.FrozenCount)
            if (colIndex <= this.Grid.InternalGetFrozenCols())
            {
                int scrollPos = 0;
                if (this.Grid.ShouldScrollNestedTable(nestedTable))
                {
                    GridTableControl tc = this.Grid.GetTableControlWindow();
                    int indent = tc.GetHScrollPixelMinimum();
                    int scrollPosition = tc.GetCurrentHScrollPixelPos();
                    scrollPos = scrollPosition - indent + this.renderControl.GetHScrollPixelMinimum();
                    this.SetCurrentHScrollPixelPosition(scrollPos);
                }
                else
                {
                    if (this.Grid.HScrollPixel)
                    {
                        scrollPos = this.Grid.HScrollBar.Minimum - this.Grid.HScrollBar.Value;
                    }
                    else
                    {
                        int c1 = this.Grid.InternalGetFrozenCols() + 1;
                        int c2 = this.Grid.LeftColIndex - 1;
                        if (c2 >= c1)
                        {
                            scrollPos = -Grid.ViewLayout.GetColRangeWidth(c1, c2, GridCellSizeKind.ActualSize);
                        }
                    }

                    GridUtil.OffsetLeft(ref fullBounds, scrollPos);
                    this.renderControl.InternalSetCurrentHScrollPixelPos(this.renderControl.GetColWidth(0));
                }
                this.renderControl.HScrollPixel = false;
            }
            // These are the bounds that should be assigned to render control. We change the top row and
            // last visible row, but leave horizontal coordinates as is.
            int clipBoundsTop = clipBounds.Top;

            // When at TopRowIndex and VPixelScroll is enabled then we need to adjust the
            // top edge to be above the coordinates of the top row (overlapping with frozen area section).
            // The grip will correctly clip the drawing at the frozen area.
            if (rowIndex == this.Grid.TopRowIndex)
            {
                clipBoundsTop -= this.Grid.GetCurrentVScrollPixelDelta();
            }

            Rectangle childGridClippedBounds = Rectangle.FromLTRB(
                fullBounds.Left,
                Math.Max(clipBoundsTop, fullBounds.Top),
                fullBounds.Right,
                Math.Min(clipBounds.Bottom, fullBounds.Bottom));

            //// Get the row delta between visible ranges top row and full ranges top row.
            rowIndexDelta = rowIndex - rgCell.Top;
            return childGridClippedBounds;
        }

        /// <summary>
        /// To handle scrolling of nested table with frozen column without any painting issue.
        /// </summary>
        /// <param name="pixelPos">Current Scroll Pixel Position</param>
        private void SetCurrentHScrollPixelPosition(int pixelPos)
        {
            int colIndex;
            int pixelDelta;
            this.renderControl.HScrollPixelPosToColIndex(pixelPos, out colIndex, out pixelDelta);
            if (colIndex != 0)
                this.renderControl.InternalSetLeftCol(colIndex);
            else
                this.renderControl.InternalSetLeftCol(this.renderControl.Model.ColCount - 1);

        }
        #endregion

        internal bool OnDeactivateCurrentCell(bool allowCancel)
        {
            using (this.SwitchNestedTableAndRestore(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                if (tracing)
                {
                    TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.Info, allowCancel);
                }

                return this.Control.DeactivateCurrentCell(allowCancel);
            }
        }

        internal void OnControlEndEdit(bool synchronizeCurrentCellAfterEndEditFailed)
        {
            using (this.SwitchNestedTableAndRestore(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                if (tracing)
                {
                    TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
                }

                this.Control.ControlEndEdit(synchronizeCurrentCellAfterEndEditFailed);
            }
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            if (tracing)
            {
                TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
            }

            base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (tracing)
            {
                TraceUtil.TraceCurrentMethodInfo("Begin", clientRectangle, rowIndex, colIndex, g.ClipBounds, this.renderControl.Table.Info);
            }

            if (this.scrolled)
            {
                this.renderControl.ViewLayout.Reset();
                this.renderControl.Model.ResetVolatileData();
                this.scrolled = false;
            }

            ////if (SharedBaseAssembly.DumpFilteredChildTable && System.Windows.Forms.Control.ModifierKeys == Keys.Shift)
            //    Debugger.Break();
            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex))
            {
                try
                {
                    if (tracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
                    }

                    if (this.renderControl != null)
                    {
                        if (this.Grid.PrintingMode)
                        {
                            // Within GridNestedTableControlState.SetRenderControlBounds the bounds
                            // will be changed when renderControl.GridBounds = childGridClippedBounds;
                            // is executed (happens always when in PrintingMode).
                            // We need to restore this value after printing occured.
                            this.renderControl.InitPrintInfo();
                            this.renderControl.DrawGrid(g, false);
                            this.renderControl.RestoreBoundsAfterPrint();
                        }
                        else
                        {
                            this.renderControl.DrawGrid(g, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                }
            }

            if (tracing)
            {
                TraceUtil.TraceCurrentMethodInfo("End", this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
            }
        }

        #region ParentGrid Activation events
        /// <summary>
        /// Called from GridCurrentCell.Deactivate after GridCurrentCell.Deactivating event
        /// and before the current cell is deactivated.
        /// </summary>
        /// <returns>
        /// True if renderer can be deactivated; False if deactivation should be aborted.
        /// </returns>
        /// <override/>
        protected override bool OnDeactivating()
        {
            if (this.renderControl.Table == null)
            {
                this.renderControl = null;
                return true;
            }
            ////if (!Grid.IsWindowless)
            //// Debugger.Break();
            using (this.SwitchNestedTableAndRestore(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                if (this.renderControl != null)
                {
                    if (tracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
                    }

                    this.Control.CurrentCell.Deactivate(false);
                    if (this.Control.CurrentCell.HasCurrentCell)
                    {
                        return false;
                    }
                }

                return base.OnDeactivating();
            }
        }

        /// <override/>
        protected override void OnDeactived(int rowIndex, int colIndex)
        {
            using (this.SwitchNestedTableAndRestore(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                if (this.renderControl != null)
                {
                    if (tracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
                    }
                }

                base.OnDeactived(rowIndex, colIndex);
            }
        }

        /// <summary>
        /// This is called from GridCurrentCell.Activate after the activating event has been raised
        /// and allows interception of cell activation.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>
        /// True is cell can be activated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnActivating(int rowIndex, int colIndex)
        {
            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex))
            {
                if (this.renderControl != null)
                {
                    if (tracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo(CurrentCell, this.renderControl.Table.FilteredChildTableOrTopLevelGroup);
                    }
                }

                if (!base.OnActivating(rowIndex, colIndex))
                {
                    return false;
                }
            }

            this.SwitchNestedTable(rowIndex, colIndex);
            return true;
        }

        /// <override/>
        protected override void OnActivated()
        {
            base.OnActivated();
        }

        #endregion

        #region ParentGrid Misc Events
        /// <override/>
        protected override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
        }

        /// <override/>
        /// <summary>Determines if the cell should be scrolled into view when GridCurrentCell.ScrollInView is called.</summary>
        /// <param name="reason">Defines the reason for scrolling current cell into view.</param>
        /// <returns>returns boolean value False.</returns>
        public override bool OnScrollInView(GridScrollCurrentCellReason reason)
        {
            /*           if (this.renderControl.CurrentCell.HasCurrentCell)
            ////            {
            ////                               if (this.renderControl.CurrentCell.Renderer.OnScrollInView())
            ////                                {
            ////                                    return renderControl.CurrentCell.Renderer.GetNestedCurrentCell();
            ////                                }
            ////            }*/
            return false;
        }

        /// <override/>
        /// <summary>Determines whether the cell needs to be repainted when it becomes the current cell.</summary>
        /// <returns>returns the boolean value False.</returns>
        public override bool ShouldRefreshCurrentCell()
        {
            return false;
        }

        /// <override/>
        protected override void OnHasFocusControlChanged()
        {
            using (this.SwitchNestedTableAndRestore(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                this.SwitchNestedTable(CurrentCell.RowIndex, CurrentCell.ColIndex);
                base.OnHasFocusControlChanged();

                // TODO: what about and the focus taken away
                // just by clicking on scrollbar ...
                if (!this.HasFocusControl)
                {
                    this.Control.CurrentCell.HasControlFocus = false;
                }
                else
                {
                    if (this.Control.CurrentCell.IsEditing)
                    {
                        this.Control.CurrentCell.HasControlFocus = true;
                    }
                }
            }
        }

        #endregion

        #region MouseController Forwarding

        /// <summary>
        /// This method is called to determine whether the cell renderer wants to receive mouse events
        /// for the give cell at the given coordinates.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>
        /// Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.
        /// </returns>
        /// <override/>
        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            GridTableControl tableControl = (GridTableControl)this.Grid.GetGridWindow();

            bool dontRestore = (tableControl.InMouseMove || e.Button != MouseButtons.None)
                && (this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex));

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl == null)
                {
                    return 0;
                }

                this.forceNestedTableAndRestore = true;
                return this.renderControl.HitTest(new Point(e.X, e.Y), e.Button);
            }
        }

        int mRowIndex;
        int mColIndex;
        GridNestedTable mTable;

        /// <override/>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    this.mRowIndex = rowIndex;
                    this.mColIndex = colIndex;
                    if (rowIndex < this.Grid.Table.DisplayElements.Count)
                    {
                        this.mTable = Grid.Table.DisplayElements[rowIndex] as GridNestedTable;
                        if (this.mTable != null)
                        {
                            this.renderControl.CurrentCellMoving += new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
                            this.renderControl.MouseControllerDispatcher.ProcessMouseDown(e);
                            this.renderControl.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
                            this.mTable = null;
                        }
                    }
                }
            }
        }

        private void renderControl_CurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
        {
            int r = this.Grid.Table.DisplayElements.IndexOf(mTable);
            if (r != -1)
            {
                e.Cancel = !CurrentCell.MoveTo(r, this.mColIndex);
            }
        }

        /// <override/>
        protected override void OnMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    this.renderControl.OnCellTipsMouseMove(e);
                    this.renderControl.MouseOperationChildTable = this.renderControl.Table.FilteredChildTable;
                    this.renderControl.MouseControllerDispatcher.ProcessMouseMove(e);
                }
            }
        }

        /// <override/>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    Point pt = this.Grid.GetGridWindow().LastMousePosition;
                    pt = this.renderControl.GridPointToClient(pt);
                    this.renderControl.MouseControllerDispatcher.ProcessMouseMove(new MouseEventArgs(MouseButtons.None, 1, pt.X, pt.Y, 0));
                }
            }
        }

        /// <override/>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    this.renderControl.MouseControllerDispatcher.ProcessMouseMove(new MouseEventArgs(MouseButtons.None, 1, int.MaxValue, int.MaxValue, 0));
                }
            }
        }

        /// <override/>
        protected override void OnMouseMove(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    this.renderControl.MouseControllerDispatcher.ProcessMouseMove(e);
                }
            }
        }

        /// <override/>
        protected override void OnMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            ////Console.WriteLine(rowIndex.ToString() + ": " + Grid.Table.DisplayElements[rowIndex].ToString());
            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                ////Console.WriteLine(renderControl.Table.FilteredChildTable.ToString());
                if (this.renderControl != null)
                {
                    this.mRowIndex = rowIndex;
                    this.mColIndex = colIndex;
                    if (rowIndex < this.Grid.Table.DisplayElements.Count)
                    {
                        this.mTable = Grid.Table.DisplayElements[rowIndex] as GridNestedTable;
                        if (this.mTable != null)
                        {
                            this.renderControl.CurrentCellMoving += new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
                            this.renderControl.MouseControllerDispatcher.ProcessMouseUp(e);
                            this.renderControl.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(renderControl_CurrentCellMoving);
                            this.mTable = null;
                        }
                    }
                }
            }
        }

        /// <override/>
        protected override void OnCancelMode(int rowIndex, int colIndex)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl != null)
                {
                    this.renderControl.MouseControllerDispatcher.ProcessCancelMode();
                }
            }
        }

        /// <summary>
        /// Override this method if you want to change the cursor for this cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Forms.Cursor"/> to be displayed.
        /// </returns>
        /// <override/>
        protected override Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            bool dontRestore = this != this.Grid.CurrentCell.Renderer || this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            using (this.SwitchNestedTableAndRestore(rowIndex, colIndex, dontRestore))
            {
                if (this.renderControl == null)
                {
                    return null;
                }

                return this.renderControl.MouseControllerDispatcher.DisplayCursor;
            }
        }

        #endregion

        #region Keyboard Forwarding

        /// <summary>
        /// This is called from GridControlBase.ProcessKeyEventArgs and allows your customized cell renderer
        /// to process keyboard events before the GridControlBase gets the actual KeyDown / KeyUp event.
        /// </summary>
        /// <param name="m">The <see cref="T:System.Windows.Forms.Message"/> with data of the keyboard event.</param>
        /// <returns>
        /// True if key was handled; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            bool b = false;
            this.SwitchNestedTable(CurrentCell.RowIndex, CurrentCell.ColIndex);
            if (this.renderControl != null)
            {
                b = this.renderControl.RaiseProcessKeyEventArgs(ref m);
            }

            return b;
        }
        #endregion

        #region renderControl Events
        /*
                bool ActivateMouseCell()
                {
                    TraceUtil.TraceCurrentMethodInfo(Control.Table.Info);
                    //            if (tracing)
        //                TraceUtil.TraceCurrentMethodInfo();
        //
        //            inActivateMouseCell = true;
        //
        //            GridCurrentCell gcc = CurrentCell; // CurrentCell of this renderers parent grid ...
        //            bool hasFocus = gcc.HasCurrentCellAt(RowIndex, ColIndex);
        //
        //            if (this.lastMouseCellState != null)
        //            {
        //                return this.CurrentCell.MoveTo(
        //                    lastMouseCellState.RowIndex,
        //                    lastMouseCellState.ColIndex,
        //                    GridSetCurrentCellOptions.NoSetFocus|GridSetCurrentCellOptions.NoSyncCurrentCell);
        //            }
        //            else if (!hasFocus)
        //                return false;
        //
        //            lastMouseCellState = GetCurrentCellState();
        //            inActivateMouseCell = false;
        //            hitActivateMouseCell = true;
                    return true;
                }

                private void renderControl_ColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
                {
                    Grid.Invalidate();
                }

                private void renderControl_CurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs e)
                {
                    TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);
        //            hitRenderControl_CurrentCellActivating = true;
                }
                bool tracing = true;

                private void renderControl_CurrentCellActivated(object sender, EventArgs e)
                {
                    TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);
        //            if (this.activatedCurrentCellState != null)
        //                this.activatedCurrentCellState.CurrentCellState = Control.CurrentCell;

                }

                private void renderControl_CurrentCellStartEditing(object sender, CancelEventArgs e)
                {
        //            if (tracing)
        //                TraceUtil.TraceCurrentMethodInfo(e);
        //            if (CurrentCell.HasCurrentCellAt(RowIndex, ColIndex))
        //            {
        //                if (!Grid.InSynchronizeGridWithEngine)
        //                {
        //                    GridCurrentCell innerCurrentCell = this.GetInnerCurrentCell();
        //                    if (innerCurrentCell != null)
        //                    {
        //                        using(new GridCurrentCellState(this))
        //                        {
        //                            innerCurrentCell.ScrollInView(GridScrollCurrentCellReason.BeginEdit);
        //                        }
        //                    }
        //                }
        //                // Set IsMousePressed = true. Otherwise the will scroll first row of parent nested table into view.
        //                bool v= Grid.IsMousePressed;
        //                Grid.IsMousePressed = true; // TODO: LockScroll or so?
        //                CurrentCell.BeginEdit();
        //                Grid.IsMousePressed = v;
        //
        ////                if (renderControl.IsMousePressed)
        ////                {
        ////                    int delta;
        ////                    Rectangle bounds = this.GetCurrentCellBounds(out delta);
        ////                    Trace.WriteLine(bounds);
        ////                }
        //            }
        //            e.Cancel = !CurrentCell.IsEditing;
                }

                private void renderControl_CurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
                {
                    if (tracing)
                        TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);
                }

                private void renderControl_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
                {
                    if (tracing)
                        TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);
                    //TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose|Switches.CurCellNestedGrid.TraceVerbose, this.renderControl.Table, e);
                    //this.hitRenderControl_CurrentCellActivated = true;
        //            if (this.activatedCurrentCellState != null)
        //                this.activatedCurrentCellState.CurrentCellState = Control.CurrentCell;
                }

                private void renderControl_MouseActivating(object sender, CancelEventArgs e)
                {
                    TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);
                    //            if (tracing)
        //                TraceUtil.TraceCurrentMethodInfo(e);
        //
        //            // bubble up
        //            if (!Grid.RaiseMouseActivating())
        //            {
        //                e.Cancel = true;
        //                return;
        //            }
        //
        //            ActivateMouseCell();
        //            if (tracing)
        //                TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose|Switches.CurCellNestedGrid.TraceVerbose, this.ToString());
                }

                private void relatedTable_CategorizingRecords(object sender, TableEventArgs e)
                {
        //                this.tableDescriptorVersion = -1;
        //                this.MouseDownTick = -1;
        //                this.MouseDownPoint = Point.Empty;
        //                this.lastRowIndex = -1;
        //                this.lastMouseCellState = null;
        //                this.lastColIndex = -1;
        //                this.inOnDraw = false;
        //                this.inMouseOp = false;
        //                this.inActivateMouseCell = false;
        //                this.hitRenderControl_CurrentCellActivating = false;
        //                this.hitRenderControl_CurrentCellActivated = false;
        //                this.hasCurrentCellLayoutChanged = false;
        //                this.currentCellLayoutVersion = -1;
        //                this.activatedCurrentCellState = null;
        //                this.activated = false;
        //                this._layoutVersion = -1;
                }

                private void renderControl_CurrentCellChanging(object sender, CancelEventArgs e)
                {
                    TraceUtil.TraceCurrentMethodInfo(Control.Table.Info, e);

                }

            */
        #endregion

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public class GridSwitchNestedTableAndRestoreHelper : IDisposable
        {
            internal GridNestedTable oldNestedTable;
            GridNestedTableControlCellRenderer owner;

            /// <internalonly/>
            /// <summary>Internal only.</summary>
            public GridSwitchNestedTableAndRestoreHelper(GridNestedTableControlCellRenderer owner, GridNestedTable oldNestedTable)
            {
                this.oldNestedTable = oldNestedTable;
                this.owner = owner;
            }

            /// <internalonly/>
            /// <summary>Internal only.</summary>
            public void Dispose()
            {
                ////if (SharedBaseAssembly.DumpFilteredChildTable)
                //    TraceUtil.TraceCurrentMethodInfo();
                if (this.oldNestedTable != null && this.oldNestedTable.GetVisibleInHierarchy())
                {
                    ////&& oldNestedTable.ParentChildTable == oldNestedTable.ParentTable.FilteredChildTable
                    if (GridNestedTableControlCellRenderer.tracing)
                    {
                        ////int rowIndex = oldNestedTable.GetRowIndex();
                        TraceUtil.TraceCurrentMethodInfo(this.oldNestedTable);
                        TraceUtil.TraceCalledFrom(2);
                    }
                    ////                    if (SharedBaseAssembly.DumpFilteredChildTable)
                    ////                        Debugger.Break();
                    this.owner.SwitchNestedTable(oldNestedTable);
                }
            }
        }
    }
}
