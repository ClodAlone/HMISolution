//-------------------------------------------------------------------------------------------------
// <copyright file="GridNestedTableControl.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion

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
    /// A windowless grid control that displays rows with <see cref="Syncfusion.Grouping.Table.DisplayElements"/> of a nested
    /// <see cref="GridTable"/> inside a parent table control. One GridNestedTableControl is created for every nested relation.
    /// A GridNestedTableControl is shared among all <see cref="ChildTable"/> tables for a relation.
    /// </summary>
    public class GridNestedTableControl : GridTableControl
    {
        GridTableControl parentGrid;
        GridNestedTableControlCellRenderer parentRenderer;
        internal int id = 0;
        static internal int globalId = 0;

        /// <summary>
        /// Returns the GridNestedTableControlCellRenderer that is the parent cell of this nested table control.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridNestedTableControlCellRenderer ParentRenderer
        {
            get
            {
                return this.parentRenderer;
            }
        }

        /// <summary>
        /// Initializes the control with its model and the parent control that it will be displayed in.
        /// </summary>
        /// <param name="model">The table model.</param>
        /// <param name="parentGrid">The parent control.</param>
        /// <param name="parentRenderer">The <see cref="GridNestedTableControlCellRenderer"/> which hosts this control.</param>
        public GridNestedTableControl(GridTableModel model, GridTableControl parentGrid, GridNestedTableControlCellRenderer parentRenderer)
            : base(model)
        {
            this.parentGrid = parentGrid;
            this.parentRenderer = parentRenderer;
            this.IsWindowless = true;
            model.QueryRowHeight += new GridRowColSizeEventHandler(this.model_QueryRowHeight);
            ////this.HScrollPixel = false;
            ////this.VScrollPixel = false;
            this.MouseControllerDispatcher.AllowDoubleClickTimer = false;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.sn = parentRenderer.Model.RelatedTable.TableDescriptor.Name);
            }

            this.groupingControl = parentGrid.GroupingControl;
            this.id = globalId++;
        }

        string sn;

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.sn);
            }

            if (disposing)
            {
                this.parentGrid = null;
                GridTableModel model = (GridTableModel) Model;
                if (model != null)
                {
                    model.QueryRowHeight -= new GridRowColSizeEventHandler(this.model_QueryRowHeight);
                }
            }

            base.Dispose(disposing);
        }

        /// <override/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        /// <override/>
        /// <summary>
        /// Triggers when the currentrecord context changes.
        /// </summary>
        protected override void Table_CurrentRecordContextChange(object sender, CurrentRecordContextChangeEventArgs e)
        {
            GridNestedTable nt = null;
            if (e.Record != null)
            {
                ChildTable ct = e.Record.ParentChildTable;
                if (ct != null)
                {
                    if ((ct.ParentNestedTable as GridNestedTable).ParentChildTable != null)
                        nt = ct.ParentNestedTable as GridNestedTable;
                }
            }

            using (this.parentRenderer.SwitchNestedTableAndRestore(nt))
            {
                base.Table_CurrentRecordContextChange(sender, e);
            }
        }

        /// <override/>
        /// <summary>Updates the control.</summary>
        public override void Update()
        {
            ////            if (SharedBaseAssembly.DumpFilteredChildTable)
            ////                Debugger.Break();
            using (this.parentRenderer.SaveCurrentNestedTableAndRestore())
            {
                base.Update();
            }
        }

        /// <override/>
        /// <summary>The current cell for this nested table.</summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override GridCurrentCell CurrentCell
        {
            get
            {
                if (Model.HasTable && !this.IsDisposed && !this.IsDisposing)
                {
                    GridChildTable childTable = Model.Table.FilteredChildTableOrTopLevelGroup as GridChildTable;
                    if (childTable != null)
                    {
                        // SH 2/23 - Added check for childTable.CurrentCell.Grid != this to prevent
                        // problem that sometimes in EmployeeTerrotoryOrder sample click inside orders
                        // was not taken after sorting employees.
                        if (childTable.CurrentCell == null)
                        {
                            childTable.CurrentCell = new GridCurrentCell(this);
                            childTable.CurrentCell.defaultUpdateFlag = false;
                        }
                        else
                        {
                            GridNestedTableControl gg = (GridNestedTableControl) childTable.CurrentCell.Grid;
                            ////int id1 = gg.id;
                            ////Console.WriteLine(this.id.ToString() + " --- " + id1.ToString());
                            if (!Object.ReferenceEquals(gg, this))
                            {
                                ////Console.WriteLine(Object.ReferenceEquals(childTable.CurrentCell.Grid, this));
                                childTable.CurrentCell = new GridCurrentCell(this);
                                childTable.CurrentCell.defaultUpdateFlag = false;
                            }
                        }

                        return childTable.CurrentCell;
                    }
                }

                return base.CurrentCell;
            }
        }

        /*
         *  Usage of HideFirstRow in GridNestedTableControlCellRenderer and GridNestedTableControlState
         *
            Rectangle GetDrawCellBounds(int rowIndex, int colIndex, Rectangle clipBounds, out int rowIndexDelta)
            {
                // Complete bounds of the cell to be drawn (can be pretty large ...)
                GridRangeInfo rgCell = Grid.Model.CoveredRanges.FindRange(rowIndex, colIndex);

                // Get the row delta between visible ranges top row and full ranges top row
                rowIndexDelta = rowIndex - rgCell.Top;

            internal static void SetRenderControlBounds(GridNestedTableControl renderControl, Rectangle childGridClippedBounds, int rowIndexDelta)
            {
                bool changed;
                if (rowIndexDelta == 0)
                {
                    changed = renderControl.TopRowIndex != 1 || renderControl.HideFirstRow;
                    if (changed)
                    {
                        renderControl.InternalSetTopRow(1);
                        renderControl.HideFirstRow = false;
                    }
                }
                else
                {
                    changed = renderControl.TopRowIndex != rowIndexDelta || !renderControl.HideFirstRow;
                    if (changed)
                    {
                        renderControl.InternalSetTopRow(rowIndexDelta);
                        renderControl.HideFirstRow = true;
                    }
                }
        */

        bool hideFirstRow = false;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool HideFirstRow
        {
            get
            {
                return this.hideFirstRow;
            }

            set
            {
                if (this.hideFirstRow != value)
                {
                    this.hideFirstRow = value;
                    ViewLayout.Reset();
                }
            }
        }

        internal bool RaiseProcessKeyEventArgs(ref Message m)
        {
            return ProcessKeyEventArgs(ref m);
        }

        /// <override/>
        /// <summary>
        /// Draws the grid to the specified graphics canvas.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="shouldClip">Specifies if the clipping region should be saved and restored after the grid is drawn.</param>
        public override void DrawGrid(Graphics g, bool shouldClip)
        {
            this.ThemesEnabled = this.parentGrid.ThemesEnabled;
            this.RightToLeft = this.parentGrid.RightToLeft;
            base.DrawGrid(g, shouldClip);
        }

        /// <override/>
        /// <summary>
        /// Occurs when the grid drawing engine wants to draw the specified range of visible cells that need repainting.
        /// </summary>
        /// <param name="topRow">Top row index.</param>
        /// <param name="leftCol">Left column index.</param>
        /// <param name="bottomRow">Bottom row index.</param>
        /// <param name="rightCol">Right column index.</param>
        /// <param name="g">Graphics context.</param>
        /// <param name="rectClip">Clipping rectangle.</param>
        public override void OnDrawClientRowCol(int topRow, int leftCol, int bottomRow, int rightCol, Graphics g, Rectangle rectClip)
        {
            this.ThemesEnabled = this.parentGrid.ThemesEnabled;
            this.RightToLeft = this.parentGrid.RightToLeft;
            this.Model.EnableLegacyStyle = this.parentGrid.Model.EnableLegacyStyle;
            base.OnDrawClientRowCol(topRow, leftCol, bottomRow, rightCol, g, rectClip);
        }

        internal GridTableControl GetParentTableControl()
        {
            if (this.parentGrid is GridNestedTableControl)
            {
                return ((GridNestedTableControl)this.parentGrid).GetParentTableControl();
            }

            return this.parentGrid;
        }

        /// <override/>
        /// <summary>
        /// Called to scroll the specified cell into view.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="dontScroll">Specifies if the top row index and left column index should be changed
        /// without raising scroll events and without updating the screen (calling DoScroll).</param>
        /// <param name="reason">The reason for scrolling the current cell into view (e.g. KeyPress, GridFocus etc.)</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already
        /// in the visible area.</returns>
        public override bool ProcessScrollCellInView(int rowIndex, int colIndex, bool dontScroll, GridScrollCurrentCellReason reason)
        {
            GridTableControl tableControl = this.GetParentTableControl();

            Element el = Table.DisplayElements[rowIndex];
            int parentRowIndex = tableControl.Table.NestedDisplayElements.IndexOf(el);

            // Use tableControl.ProcessScrollCellInView for scrolling rows into view
            // and use tableControl.HScrollPixelScrollInView to scroll column into view.
            int parentColIndex = 0;
            bool scrolled = tableControl.ProcessScrollCellInView(parentRowIndex, parentColIndex, dontScroll, reason);

            if (colIndex > this.InternalGetFrozenCols())
            {
            Rectangle cellBounds = RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells|GridRangeOptions.CalculateNonClientArea);
            scrolled |= tableControl.HScrollPixelScrollInView(cellBounds);
            }

            return scrolled;
        }

        /// <override/>
        protected override void OnTopRowChanging(GridRowColIndexChangingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <override/>
        protected override void OnLeftColChanging(GridRowColIndexChangingEventArgs e)
        {
            e.Cancel = true;
        }

        private void model_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            if (e.Index == 0 && this.HideFirstRow)
            {
                e.Size = 0;
                e.Handled = true;
            }
        }
    }
}
