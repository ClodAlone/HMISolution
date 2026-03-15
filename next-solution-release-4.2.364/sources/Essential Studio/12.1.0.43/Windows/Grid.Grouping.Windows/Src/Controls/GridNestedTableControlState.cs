//-------------------------------------------------------------------------------------------------
// <copyright file="GridNestedTableControlState.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

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
    internal class GridNestedTableControlState
    {
        bool hasGridBounds;
        Rectangle gridBounds;
////        GridCurrentCell currentCellState;
        GridTable table;
        GridNestedTableControl tableControl;
        int rowIndex = -1;
        int colIndex = -1;
        ChildTable childTable;
        int rowIndexDelta;
        NestedTable nestedTable;
        public bool InMouseMove = false;

        void InitDrawState(GridNestedTableControlState other)
        {
            this.hasGridBounds = other.hasGridBounds;
            this.gridBounds = other.gridBounds;
        }

        public GridNestedTableControl TableControl
        {
            get
            {
                return this.tableControl;
            }

            set
            {
                this.tableControl = value;
            }
        }

        public int RowIndexDelta
        {
            get
            {
                return this.rowIndexDelta;
            }

            set
            {
                this.rowIndexDelta = value;
            }
        }

        public bool HasGridBounds
        {
            get
            {
                return this.hasGridBounds;
            }

            set
            {
                this.hasGridBounds = value;
            }
        }

        public Rectangle GridBounds
        {
            get
            {
                return this.gridBounds;
            }

            set
            {
                this.gridBounds = value;
                this.hasGridBounds = true;
            }
        }

////        public GridCurrentCell CurrentCellState
////        {
////            get
////            {
////                return currentCellState;
////            }
////            set
////            {
////                currentCellState = value;
////            }
////        }

        public GridTable Table
        {
            get
            {
                return this.table;
            }

            set
            {
                this.table = value;
            }
        }

        public ChildTable ChildTable
        {
            get
            {
                return this.childTable;
            }

            set
            {
                this.childTable = value;
            }
        }

        public NestedTable NestedTable
        {
            get
            {
                return this.nestedTable;
            }

            set
            {
                this.nestedTable = value;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }

            set
            {
                this.rowIndex = value;
            }
        }

        public int ColIndex
        {
            get
            {
                return this.colIndex;
            }

            set
            {
                this.colIndex = value;
            }
        }

        public GridNestedTableControlState()
        {
        }

        public GridNestedTableControlState(GridNestedTableControl tableControl)
        {
            this.tableControl = tableControl;
            this.table = tableControl.Table;
            this.childTable = tableControl.Model.FilteredChildTable;
            this.gridBounds = tableControl.GridBounds;
            this.hasGridBounds = tableControl.HasGridBounds;
            this.rowIndexDelta = tableControl.TopRowIndex;
            this.InMouseMove = tableControl.InMouseMove;

            if (tableControl.HideFirstRow)
            {
                this.rowIndexDelta++;
            }
        }

        public GridNestedTableControlState(GridNestedTableControl tableControl, int rowIndex, int colIndex)
        {
            this.tableControl = tableControl;
            this.table = tableControl.Table;
            this.childTable = tableControl.Model.FilteredChildTable;
            this.gridBounds = tableControl.GridBounds;
////            this.currentCellState = tableControl.CurrentCell;
            this.hasGridBounds = tableControl.HasGridBounds;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.rowIndexDelta = tableControl.TopRowIndex;
            this.InMouseMove = tableControl.InMouseMove;

            if (tableControl.HideFirstRow)
            {
                this.rowIndexDelta++;
            }
        }

        public bool ApplyState()
        {
            return this.ApplyState(false);
        }

        public bool ApplyState(bool restore)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose|Switches.CurCellNestedGrid.TraceVerbose, this.table.ToString(), GridBounds, "CC = ", currentCellState.HasCurrentCell, currentCellState.RowIndex, currentCellState.ColIndex);
            this.tableControl.Model.SetTableInternal(this.table);
            this.tableControl.Model.FilteredChildTable = this.childTable;
            ////if (RowIndexDelta == 1 && Control.ModifierKeys == Keys.Control)
                ////Debugger.Break();

            ////SSTraceUtil.TraceCurrentMethodInfo(this.table.ToString(), GridBounds, RowIndexDelta, "CC = ", currentCellState.HasCurrentCell, currentCellState.RowIndex, currentCellState.ColIndex);
            bool changed = SetRenderControlBounds(this.tableControl, this.GridBounds, this.RowIndexDelta);
            this.tableControl.InMouseMove = this.InMouseMove;
            /*if (changed)
            {
                //Trace.WriteLine("********************************");
                string info = this.childTable.Info;
                if (info.Length > 60)
                    info = info.Substring(0,60);
                TraceUtil.TraceCurrentMethodInfo(this.RowIndex, info, GridBounds);
            }      */
            return changed;
            // TODO: Clean UP
            ////if (!restore)
//                {
//                    if (currentCellState != tableControl.CurrentCell
//                        && tableControl.CurrentCell.IsInMoveTo)
//                        //|| tableControl.HasCurrentCell)
//                    {
//                        Trace.WriteLine("*******" + this.ToString() + "/" + currentCellState.ToString());
//                        TraceUtil.TraceCurrentMethodInfo("*****", tableControl.CurrentCell, this, currentCellState);
//                        TraceUtil.TraceCalledFrom(16);
//
//                        deb++;
//
//                        //Debugger.Break();
//                    }
//                }
//            tableControl.InternalSetCurrentCellObject(this.currentCellState);
        }

        ////public static int deb;
        internal static bool SetRenderControlBounds(GridNestedTableControl renderControl, Rectangle childGridClippedBounds, int rowIndexDelta)
        {
            ////TraceUtil.TraceCurrentMethodInfo(childGridClippedBounds, rowIndexDelta);
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

            changed |= renderControl.PrintingMode || renderControl.GridBounds != childGridClippedBounds;
            if (changed)
            {
                renderControl.GridBounds = childGridClippedBounds;
            }

            if (changed)
            {
                renderControl.ViewLayout.Reset();
                renderControl.Model.ResetVolatileData();
                renderControl.Table.DisplayElements.ClearCache();
                renderControl.Table.NestedDisplayElements.ClearCache();
            }

            return changed;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
