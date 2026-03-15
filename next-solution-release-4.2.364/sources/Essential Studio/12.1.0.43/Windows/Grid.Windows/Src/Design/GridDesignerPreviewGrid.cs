//-------------------------------------------------------------------------------------------------
// <copyright file="GridDesignerPreviewGrid.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <summary>
    ///    A derived grid component class.
    /// </summary>
    [ToolboxItem(false)]
    internal class GridDesignerPreviewGrid : GridControl
    {
        GridFrame gridFrame;

        public GridDesignerPreviewGrid(GridModel model, GridFrame gridFrame)
            : base(model)
        {
            this.gridFrame = gridFrame;
        }

        //// Find and replace dialog

        GridFindReplaceDialogSink findReplaceDialogSink;

        public GridFindReplaceDialogSink GridFindReplaceDialogSink
        {
            get
            {
                if (findReplaceDialogSink == null)
                {
                    findReplaceDialogSink = new GridFindReplaceDialogSink(this);
                }

                return findReplaceDialogSink;
            }
        }

        protected override void OnCurrentCellActivated(EventArgs e)
        {
            GridFindReplaceDialog.ResetFindLocation();
            gridFrame.OnPreviewGridCurrentCellActivated(this, e);

            base.OnCurrentCellActivated(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            gridFrame.OnPreviewGridMouseDown(this, e);
            base.OnMouseDown(e);
        }

        protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            gridFrame.OnPreviewGridPrepareViewStyleInfo(this, e);

            // Fixes defect 2316 (Setting Enabled property in baseStyle collection, should not interact with the designer) 
            e.Style.Enabled = true;

            base.OnPrepareViewStyleInfo(e);
        }

        protected override void OnControlGotFocus()
        {
            GridFindReplaceDialog.SetActiveSinkIfVisible(GridFindReplaceDialogSink);
            base.OnControlGotFocus();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.TopRowIndex = InternalGetHeaderRows() + 1;
            this.LeftColIndex = InternalGetHeaderCols() + 1;
            this.AllowDrop = true;
            this.IgnoreReadOnly = true;
        }

        public static void SetupGridModel(GridModel model)
        {
            model.CommandStack.Enabled = true;
        }

        static void ModelQueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellObjectFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellObjectFactory != null)
                {
                    e.CellModel = pGridCellObjectFactory.CreateCellModel(e.CellType, (GridModel)sender);
                }
            }
        }

        protected override bool ShouldDeactivateCurrentCell()
        {
            return CausesValidation && base.ShouldDeactivateCurrentCell();
        }

        protected override void WireModel()
        {
            base.WireModel();
            WireGridModel(Model);
        }

        protected override void UnwireModel()
        {
            base.UnwireModel();
            UnwireGridModel(Model);
        }

        internal void WireGridModel(GridModel grid)
        {
            grid.CellsChanged += new GridCellsChangedEventHandler(grid1_CellsChanged);
            grid.RowHeightsChanged += new GridRowColSizeChangedEventHandler(grid1_RowHeightsChanged);
            grid.ColWidthsChanged += new GridRowColSizeChangedEventHandler(grid1_ColWidthsChanged);
            grid.Properties.Changed += new EventHandler(Properties_Changed);
            grid.ColsRemoved += new GridRangeRemovedEventHandler(grid1_ColsRemoved);
            grid.ColsInserted += new GridRangeInsertedEventHandler(grid1_ColsInserted);
            grid.RowsRemoved += new GridRangeRemovedEventHandler(grid1_RowsRemoved);
            grid.RowsInserted += new GridRangeInsertedEventHandler(grid1_RowsInserted);
            grid.SelectionChanged += new GridSelectionChangedEventHandler(grid1_SelectionChanged);
            grid.PrepareGraphics += new Syncfusion.Drawing.GraphicsEventHandler(grid1_PrepareGraphics);
        }

        internal void UnwireGridModel(GridModel grid)
        {
            grid.CellsChanged -= new GridCellsChangedEventHandler(grid1_CellsChanged);
            grid.RowHeightsChanged -= new GridRowColSizeChangedEventHandler(grid1_RowHeightsChanged);
            grid.ColWidthsChanged -= new GridRowColSizeChangedEventHandler(grid1_ColWidthsChanged);
            grid.Properties.Changed -= new EventHandler(Properties_Changed);
            grid.ColsRemoved -= new GridRangeRemovedEventHandler(grid1_ColsRemoved);
            grid.ColsInserted -= new GridRangeInsertedEventHandler(grid1_ColsInserted);
            grid.RowsRemoved -= new GridRangeRemovedEventHandler(grid1_RowsRemoved);
            grid.RowsInserted -= new GridRangeInsertedEventHandler(grid1_RowsInserted);
            grid.SelectionChanged -= new GridSelectionChangedEventHandler(grid1_SelectionChanged);
            grid.PrepareGraphics -= new Syncfusion.Drawing.GraphicsEventHandler(grid1_PrepareGraphics);
        }

        private void grid1_ColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            gridFrame.OnPreviewGridColWidthsChanged(sender, e);
        }

        private void grid1_RowHeightsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            gridFrame.OnPreviewGridRowHeightsChanged(sender, e);
        }

        private void grid1_CellsChanged(object sender, GridCellsChangedEventArgs e)
        {
            gridFrame.OnPreviewGridCellsChanged(sender, e);
        }

        private void Properties_Changed(object sender, EventArgs e)
        {
            gridFrame.OnPreviewGridPropertiesChanged(sender, e);
        }

        private void grid1_ColsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            gridFrame.OnPreviewGridColsRemoved(sender, e);
        }

        private void grid1_ColsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            gridFrame.OnPreviewGridColsInserted(sender, e);
        }

        private void grid1_RowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            gridFrame.OnPreviewGridRowsRemoved(sender, e);
        }

        private void grid1_RowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            gridFrame.OnPreviewGridRowsInserted(sender, e);
        }

        private void grid1_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            gridFrame.OnPreviewGridSelectionChanged(sender, e);
        }

        private void grid1_PrepareGraphics(object sender, Syncfusion.Drawing.GraphicsEventArgs e)
        {
            gridFrame.OnPreviewGridPrepareGraphics(sender, e);
        }

        protected override void OnModelChanged(EventArgs e)
        {
            gridFrame.OnPreviewGridModelChanged(this, e);
            base.OnModelChanged(e);
        }
    }
}
