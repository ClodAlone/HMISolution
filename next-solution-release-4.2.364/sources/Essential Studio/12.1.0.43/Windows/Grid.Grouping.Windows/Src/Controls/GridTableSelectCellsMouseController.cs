//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableSelectCellsMouseController.cs" company="syncfusion">
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
using System.IO;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <internalonly/>
    /// <summary>
    /// Implements the cell selection behavior of a grid control.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridTableSelectCellsMouseController : GridSelectCellsMouseController
    {
        GridTableControl gridWindow;

        /// <internalonly/>
        /// <summary>Constructor for GridTableSelectCellsMouseController.</summary>
        /// <param name="grid">The grid table control.</param>
        public GridTableSelectCellsMouseController(GridTableControl grid)
            : base(grid)
        {
            gridWindow = (GridTableControl) grid.GetGridWindow();
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context. Used internally.
        /// </summary>
        /// <param name="e">Provides data for the System.Windows.Forms.Control.MouseUp, System.Windows.Forms.Control.MouseDown, and System.Windows.Forms.Control.MouseMove events.</param>
        /// <param name="controller">IMouseController defines the interface for mouse controllers to be used with MouseControllerDispatcher.</param>
        /// <returns>returns HitTest value</returns>
        /// <override/>
        /// <internalonly/>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            gridWindow.Model.CoveredRanges.ResetCache();

            if (gridWindow.Table.TableOptions.AllowSelection == GridSelectionFlags.None
                && gridWindow.Table.TableOptions.ListBoxSelectionMode != SelectionMode.None)
            {
                return 0;
            }

            return base.HitTest(e, controller);
        }

        /// <override/>
        /// <internalonly/>
        protected override bool GridCurrentCellExternalMove(GridDirectionType direction, int num, bool extendSelection)
        {
            if (gridWindow.Table.TableOptions.AllowSelection == GridSelectionFlags.None
                && gridWindow.Table.TableOptions.ListBoxSelectionMode != SelectionMode.None)
            {
                return false;
            }

            return base.GridCurrentCellExternalMove(direction, num, extendSelection);
            ////return Grid.CurrentCell.InternalMove(direction, num, GridSetCurrentCellOptions.ScrollInView);
        }

        /// <internalonly/>
        protected override void ProcessSetCurrentCell(int rowIndex, int colIndex, GridSetCurrentCellOptions flags)
        {
            if (gridWindow.Table.TableOptions.AllowSelection == GridSelectionFlags.None
                && gridWindow.Table.TableOptions.ListBoxSelectionMode != SelectionMode.None)
            {
                return;
            }

            base.ProcessSetCurrentCell(rowIndex, colIndex, flags);
        }
    }
}
