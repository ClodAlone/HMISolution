//-------------------------------------------------------------------------------------------------
// <copyright file="GridModel.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using System.Globalization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Returns a graphics context when needed and raises a
    /// <see cref="GridModel.PrepareGraphics"/>  event to initialize the graphics object.
    /// </summary>
    public class GridModelGraphicsProvider : Disposable, IGraphicsProvider
    {
        GridModel model;
        Graphics g = null;
        private DisplayDCGraphicsProvider dgp = null;
        private bool ownedGraphics = false;

        /// <summary>
        /// Called after a new <see cref="Graphics"/> object was created and gives a handler
        /// a chance to initialize the graphics context.
        /// </summary>
        public event GraphicsEventHandler PrepareGraphics;

        /// <overload>
        /// Initializes a new empty <see cref="GraphicsProvider"/>
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GraphicsProvider"/> with a <see cref="Control"/> object.
        /// </summary>
        /// <param name="model">The control that will be used for creating the graphics object..</param>
        public GridModelGraphicsProvider(GridModel model)
        {
            this.model = model;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.ownedGraphics)
                {
                    if (g != null)
                    {
                        g.Dispose();
                        g = null;
                    }
                }
                // Just removing the reference will cause the DisplayDCGraphicsProvider to cleanup itself.
                this.dgp = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or creates a cached graphics object.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                if (g == null)
                {
                    GridControlBase control = model.ActiveGridView;
                    if (control != null)
                    {
                        control = control.GetGridWindow();
                    }

                    if (control != null && control.IsHandleCreated)
                    {
                        g = control.CreateGraphics();
                        this.ownedGraphics = true;
                        OnPrepareGraphics(new GraphicsEventArgs(g));
                    }
                }

                if (g == null)
                {
                    //// It's important that we hold a reference to this singleton
                    //// as long as this instance is alive.
                    if (this.dgp == null)
                    {
                        this.dgp = DisplayDCGraphicsProvider.Singleton;
                        OnPrepareGraphics(new GraphicsEventArgs(g));
                    }

                    return this.dgp.Graphics;
                }

                return g;
            }
        }

        /// <summary>
        /// Raises the <see cref="GraphicsProvider.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareGraphics(GraphicsEventArgs e)
        {
            if (model != null)
            {
                model.DoPrepareGraphics(e.Graphics);
            }

            if (PrepareGraphics != null)
            {
                PrepareGraphics(this, e);
            }
        }
    }

    /// <summary>
    /// Defines an interface for an object that handles events raised by <see cref="GridModel"/> objects.
    /// </summary>
    public interface IGridModelEventsTarget
    {
        /// <copyfrom cref="GridModel.BanneredRangesChanged"/>
        /// <summary>See <see cref="GridModel.BanneredRangesChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnBanneredRangesChanged(GridBanneredRangesChangedEventArgs e);

        /// <copyfrom cref="GridModel.BanneredRangesChanging"/>
        /// <summary>See <see cref="GridModel.BanneredRangesChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnBanneredRangesChanging(GridBanneredRangesChangingEventArgs e);

        /// <copyfrom cref="GridModel.BaseStylesMapChanged"/>
        /// <summary>See <see cref="GridModel.BaseStylesMapChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnBaseStylesMapChanged(EventArgs e);

        /// <copyfrom cref="GridModel.BeginUpdateRequest"/>
        /// <summary>See <see cref="GridModel.BeginUpdateRequest"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnBeginUpdateRequest(EventArgs e);

        /// <copyfrom cref="GridModel.CellModelsChanged"/>
        /// <summary>See <see cref="GridModel.CellModelsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCellModelsChanged(CollectionChangeEventArgs e);

        /// <copyfrom cref="GridModel.CellsChanged"/>
        /// <summary>See <see cref="GridModel.CellsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCellsChanged(GridCellsChangedEventArgs e);

        /// <copyfrom cref="GridModel.CellsChanging"/>
        /// <summary>See <see cref="GridModel.CellsChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCellsChanging(GridCellsChangingEventArgs e);

        /// <copyfrom cref="GridModel.ClearingCells"/>
        /// <summary>See <see cref="GridModel.ClearingCells"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClearingCells(GridClearingCellsEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCopy"/>
        /// <summary>See <see cref="GridModel.ClipboardCopy"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCopyToBuffer(ClipboardCopyToBufferEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCanCopy"/>
        /// <summary>See <see cref="GridModel.ClipboardCanCopy"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCanCopy(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCanCut"/>
        /// <summary>See <see cref="GridModel.ClipboardCanCut"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCanCut(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCanPaste"/>
        /// <summary>See <see cref="GridModel.ClipboardCanPaste"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCanPaste(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCopy"/>
        /// <summary>See <see cref="GridModel.ClipboardCopy"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCopy(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardCut"/>
        /// <summary>See <see cref="GridModel.ClipboardCut"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardCut(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardPaste"/>
        /// <summary>See <see cref="GridModel.ClipboardPaste"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardPaste(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ClipboardPasted"/>
        /// <summary>See <see cref="GridModel.ClipboardPasted"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnClipboardPasted(GridCutPasteEventArgs e);

        /// <copyfrom cref="GridModel.ColsHidden"/>
        /// <summary>See <see cref="GridModel.ColsHidden"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsHidden(GridRowColHiddenEventArgs e);

        /// <copyfrom cref="GridModel.ColsHiding"/>
        /// <summary>See <see cref="GridModel.ColsHiding"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsHiding(GridRowColHidingEventArgs e);

        /// <copyfrom cref="GridModel.ColsInserted"/>
        /// <summary>See <see cref="GridModel.ColsInserted"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsInserted(GridRangeInsertedEventArgs e);

        /// <copyfrom cref="GridModel.ColsInserting"/>
        /// <summary>See <see cref="GridModel.ColsInserting"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsInserting(GridRangeInsertingEventArgs e);

        /// <copyfrom cref="GridModel.ColsMoved"/>
        /// <summary>See <see cref="GridModel.ColsMoved"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsMoved(GridRangeMovedEventArgs e);

        /// <copyfrom cref="GridModel.ColsMoving"/>
        /// <summary>See <see cref="GridModel.ColsMoving"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsMoving(GridRangeMovingEventArgs e);

        /// <copyfrom cref="GridModel.ColsRemoved"/>
        /// <summary>See <see cref="GridModel.ColsRemoved"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsRemoved(GridRangeRemovedEventArgs e);

        /// <copyfrom cref="GridModel.ColsRemoving"/>
        /// <summary>See <see cref="GridModel.ColsRemoving"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColsRemoving(GridRangeRemovingEventArgs e);

        /// <copyfrom cref="GridModel.ColWidthsChanged"/>
        /// <summary>See <see cref="GridModel.ColWidthsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColWidthsChanged(GridRowColSizeChangedEventArgs e);

        /// <copyfrom cref="GridModel.ColWidthsChanging"/>
        /// <summary>See <see cref="GridModel.ColWidthsChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnColWidthsChanging(GridRowColSizeChangingEventArgs e);

        /// <copyfrom cref="GridModel.ConfirmingPendingChanges"/>
        /// <summary>See <see cref="GridModel.ConfirmingPendingChanges"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnConfirmingPendingChanges(CancelEventArgs e);

        /// <copyfrom cref="GridModel.CoveredRangesChanged"/>
        /// <summary>See <see cref="GridModel.CoveredRangesChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCoveredRangesChanged(GridCoveredRangesChangedEventArgs e);

        /// <copyfrom cref="GridModel.CoveredRangesChanging"/>
        /// <summary>See <see cref="GridModel.CoveredRangesChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCoveredRangesChanging(GridCoveredRangesChangingEventArgs e);

        /// <copyfrom cref="GridModel.DataChanged"/>
        /// <summary>See <see cref="GridModel.DataChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDataChanged(EventArgs e);

        /// <copyfrom cref="GridModel.DataProviderChanged"/>
        /// <summary>See <see cref="GridModel.DataProviderChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDataProviderChanged(EventArgs e);

        /// <copyfrom cref="GridModel.DataProviderQueryCellInfo"/>
        /// <summary>See <see cref="GridModel.DataProviderQueryCellInfo"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDataProviderQueryCellInfo(GridQueryCellInfoEventArgs e);

        /// <copyfrom cref="GridModel.DataProviderSaveCellInfo"/>
        /// <summary>See <see cref="GridModel.DataProviderSaveCellInfo"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDataProviderSaveCellInfo(GridSaveCellInfoEventArgs e);

        /// <copyfrom cref="GridModel.DefaultColWidthChanged"/>
        /// <summary>See <see cref="GridModel.DefaultColWidthChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDefaultColWidthChanged(GridDefaultSizeChangedEventArgs e);

        /// <copyfrom cref="GridModel.DefaultColWidthChanging"/>
        /// <summary>See <see cref="GridModel.DefaultColWidthChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDefaultColWidthChanging(GridDefaultSizeChangingEventArgs e);

        /// <copyfrom cref="GridModel.DefaultRowHeightChanged"/>
        /// <summary>See <see cref="GridModel.DefaultRowHeightChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDefaultRowHeightChanged(GridDefaultSizeChangedEventArgs e);

        /// <copyfrom cref="GridModel.DefaultRowHeightChanging"/>
        /// <summary>See <see cref="GridModel.DefaultRowHeightChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnDefaultRowHeightChanging(GridDefaultSizeChangingEventArgs e);

        /// <copyfrom cref="GridModel.EndUpdateRequest"/>
        /// <summary>See <see cref="GridModel.EndUpdateRequest"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnEndUpdateRequest(GridEndUpdateRequestEventArgs e);

        /// <copyfrom cref="GridModel.FloatingCellsChanged"/>
        /// <summary>See <see cref="GridModel.FloatingCellsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnFloatingCellsChanged(GridFloatingCellsChangedEventArgs e);

        /// <copyfrom cref="GridModel.FrozenColCountChanged"/>
        /// <summary>See <see cref="GridModel.FrozenColCountChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnFrozenColCountChanged(GridCountChangedEventArgs e);

        /// <copyfrom cref="GridModel.FrozenColCountChanging"/>
        /// <summary>See <see cref="GridModel.FrozenColCountChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnFrozenColCountChanging(GridCountChangingEventArgs e);

        /// <copyfrom cref="GridModel.FrozenRowCountChanged"/>
        /// <summary>See <see cref="GridModel.FrozenRowCountChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnFrozenRowCountChanged(GridCountChangedEventArgs e);

        /// <copyfrom cref="GridModel.FrozenRowCountChanging"/>
        /// <summary>See <see cref="GridModel.FrozenRowCountChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnFrozenRowCountChanging(GridCountChangingEventArgs e);

        /// <copyfrom cref="GridModel.HeaderColCountChanged"/>
        /// <summary>See <see cref="GridModel.HeaderColCountChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnHeaderColCountChanged(GridCountChangedEventArgs e);

        /// <copyfrom cref="GridModel.HeaderColCountChanging"/>
        /// <summary>See <see cref="GridModel.HeaderColCountChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnHeaderColCountChanging(GridCountChangingEventArgs e);

        /// <copyfrom cref="GridModel.HeaderRowCountChanged"/>
        /// <summary>See <see cref="GridModel.HeaderRowCountChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnHeaderRowCountChanged(GridCountChangedEventArgs e);

        /// <copyfrom cref="GridModel.HeaderRowCountChanging"/>
        /// <summary>See <see cref="GridModel.HeaderRowCountChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnHeaderRowCountChanging(GridCountChangingEventArgs e);

        /// <copyfrom cref="GridModel.InvalidateRangeRequest"/>
        /// <summary>See <see cref="GridModel.InvalidateRangeRequest"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnInvalidateRangeRequest(GridInvalidateRangeRequestEventArgs e);

        /// <copyfrom cref="GridModel.MergeCellsChanged"/>
        /// <summary>See <see cref="GridModel.MergeCellsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnMergeCellsChanged(GridMergeCellsChangedEventArgs e);

        /// <copyfrom cref="GridModel.PasteCellText"/>
        /// <summary>See <see cref="GridModel.PasteCellText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnPasteCellText(GridPasteCellTextEventArgs e);

        /// <copyfrom cref="GridModel.PrepareChangeSelection"/>
        /// <summary>See <see cref="GridModel.PrepareChangeSelection"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnPrepareChangeSelection(GridPrepareChangeSelectionEventArgs e);

        /// <copyfrom cref="GridModel.PrepareClearSelection"/>
        /// <summary>See <see cref="GridModel.PrepareClearSelection"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnPrepareClearSelection(EventArgs e);

        /// <copyfrom cref="GridModel.PrepareGraphics"/>
        /// <summary>See <see cref="GridModel.PrepareGraphics"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnPrepareGraphics(GraphicsEventArgs e);

        /// <copyfrom cref="GridModel.QueryBanneredRange"/>
        /// <summary>See <see cref="GridModel.QueryBanneredRange"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryBanneredRange(GridQueryBanneredRangeEventArgs e);

        /// <copyfrom cref="GridModel.QueryCanMergeCells"/>
        /// <summary>See <see cref="GridModel.QueryCanMergeCells"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCanMergeCells(GridQueryCanMergeCellsEventArgs e);

        /// <copyfrom cref="GridModel.QueryCellFormattedText"/>
        /// <summary>See <see cref="GridModel.QueryCellFormattedText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCellFormattedText(GridCellTextEventArgs e);

        /// <copyfrom cref="GridModel.QueryCellInfo"/>
        /// <summary>See <see cref="GridModel.QueryCellInfo"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCellInfo(GridQueryCellInfoEventArgs e);

        /// <copyfrom cref="GridModel.QueryCellModel"/>
        /// <summary>See <see cref="GridModel.QueryCellModel"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCellModel(GridQueryCellModelEventArgs e);

        /// <copyfrom cref="GridModel.QueryCellText"/>
        /// <summary>See <see cref="GridModel.QueryCellText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCellText(GridCellTextEventArgs e);

        /// <copyfrom cref="GridModel.QueryColCount"/>
        /// <summary>See <see cref="GridModel.QueryColCount"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryColCount(GridRowColCountEventArgs e);

        /// <copyfrom cref="GridModel.QueryColWidth"/>
        /// <summary>See <see cref="GridModel.QueryColWidth"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryColWidth(GridRowColSizeEventArgs e);

        /// <copyfrom cref="GridModel.QueryCoveredRange"/>
        /// <summary>See <see cref="GridModel.QueryCoveredRange"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e);

        /// <copyfrom cref="GridModel.QueryDragDropMoveClearCells"/>
        /// <summary>See <see cref="GridModel.QueryDragDropMoveClearCells"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryDragDropMoveClearCells(CancelEventArgs e);

        /// <copyfrom cref="GridModel.QueryHideCol"/>
        /// <summary>See <see cref="GridModel.QueryHideCol"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryHideCol(GridRowColHideEventArgs e);

        /// <copyfrom cref="GridModel.QueryHideRow"/>
        /// <summary>See <see cref="GridModel.QueryHideRow"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryHideRow(GridRowColHideEventArgs e);

        /// <copyfrom cref="GridModel.QueryOleDataSourceData"/>
        /// <summary>See <see cref="GridModel.QueryOleDataSourceData"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e);

        /// <copyfrom cref="GridModel.QueryRowCount"/>
        /// <summary>See <see cref="GridModel.QueryRowCount"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryRowCount(GridRowColCountEventArgs e);

        /// <copyfrom cref="GridModel.QueryRowHeight"/>
        /// <summary>See <see cref="GridModel.QueryRowHeight"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryRowHeight(GridRowColSizeEventArgs e);

        /// <copyfrom cref="GridModel.QueryRowHeightTotal"/>
        /// <summary>See <see cref="GridModel.QueryRowHeightTotal"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnQueryRowHeightTotal(GridRowColSizeTotalEventArgs e);

        /// <copyfrom cref="GridModel.RowHeightsChanged"/>
        /// <summary>See <see cref="GridModel.RowHeightsChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowHeightsChanged(GridRowColSizeChangedEventArgs e);

        /// <copyfrom cref="GridModel.RowHeightsChanging"/>
        /// <summary>See <see cref="GridModel.RowHeightsChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowHeightsChanging(GridRowColSizeChangingEventArgs e);

        /// <copyfrom cref="GridModel.RowsHidden"/>
        /// <summary>See <see cref="GridModel.RowsHidden"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsHidden(GridRowColHiddenEventArgs e);

        /// <copyfrom cref="GridModel.RowsHiding"/>
        /// <summary>See <see cref="GridModel.RowsHiding"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsHiding(GridRowColHidingEventArgs e);

        /// <copyfrom cref="GridModel.RowsInserted"/>
        /// <summary>See <see cref="GridModel.RowsInserted"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsInserted(GridRangeInsertedEventArgs e);

        /// <copyfrom cref="GridModel.RowsInserting"/>
        /// <summary>See <see cref="GridModel.RowsInserting"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsInserting(GridRangeInsertingEventArgs e);

        /// <copyfrom cref="GridModel.RowsMoved"/>
        /// <summary>See <see cref="GridModel.RowsMoved"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsMoved(GridRangeMovedEventArgs e);

        /// <copyfrom cref="GridModel.RowsMoving"/>
        /// <summary>See <see cref="GridModel.RowsMoving"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsMoving(GridRangeMovingEventArgs e);

        /// <copyfrom cref="GridModel.RowsRemoved"/>
        /// <summary>See <see cref="GridModel.RowsRemoved"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsRemoved(GridRangeRemovedEventArgs e);

        /// <copyfrom cref="GridModel.RowsRemoving"/>
        /// <summary>See <see cref="GridModel.RowsRemoving"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnRowsRemoving(GridRangeRemovingEventArgs e);

        /// <copyfrom cref="GridModel.SaveCellFormattedText"/>
        /// <summary>See <see cref="GridModel.SaveCellFormattedText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveCellFormattedText(GridCellTextEventArgs e);

        /// <copyfrom cref="GridModel.SaveCellInfo"/>
        /// <summary>See <see cref="GridModel.SaveCellInfo"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveCellInfo(GridSaveCellInfoEventArgs e);

        /// <copyfrom cref="GridModel.SaveCellText"/>
        /// <summary>See <see cref="GridModel.SaveCellText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveCellText(GridCellTextEventArgs e);

        /// <copyfrom cref="GridModel.ParseCommonFormats"/>
        /// <summary>See <see cref="GridModel.ParseCommonFormats"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnParseCommonFormats(GridCellTextEventArgs e);

        /// <copyfrom cref="GridModel.SaveColCount"/>
        /// <summary>See <see cref="GridModel.SaveColCount"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveColCount(GridRowColCountEventArgs e);

        /// <copyfrom cref="GridModel.SaveColWidth"/>
        /// <summary>See <see cref="GridModel.SaveColWidth"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveColWidth(GridRowColSizeEventArgs e);

        /// <copyfrom cref="GridModel.SaveHideCol"/>
        /// <summary>See <see cref="GridModel.SaveHideCol"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveHideCol(GridRowColHideEventArgs e);

        /// <copyfrom cref="GridModel.SaveHideRow"/>
        /// <summary>See <see cref="GridModel.SaveHideRow"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveHideRow(GridRowColHideEventArgs e);

        /// <copyfrom cref="GridModel.SaveRowCount"/>
        /// <summary>See <see cref="GridModel.SaveRowCount"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveRowCount(GridRowColCountEventArgs e);

        /// <copyfrom cref="GridModel.SaveRowHeight"/>
        /// <summary>See <see cref="GridModel.SaveRowHeight"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveRowHeight(GridRowColSizeEventArgs e);

        /// <copyfrom cref="GridModel.SelectionChanged"/>
        /// <summary>See <see cref="GridModel.SelectionChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSelectionChanged(GridSelectionChangedEventArgs e);

        /// <copyfrom cref="GridModel.SelectionChanging"/>
        /// <summary>See <see cref="GridModel.SelectionChanging"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSelectionChanging(GridSelectionChangingEventArgs e);

        /// <copyfrom cref="GridModel.SaveCellFormattedText"/>
        /// <summary>See <see cref="GridModel.SaveCellFormattedText"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnSaveCellFormattedText(GridOleDropAtRowColEventArgs e);

        /// <copyfrom cref="GridModel.OleDroppedData"/>
        /// <summary>See <see cref="GridModel.OleDroppedData"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnOleDroppedData(EventArgs e);

        /// <copyfrom cref="GridModel.CommandStackChanged"/>
        /// <summary>See <see cref="GridModel.CommandStackChanged"/> in the <see cref="GridModel"/> class for information.</summary>
        void OnCommandStackChanged(EventArgs e);
    }

    /// <summary>
    /// This is the <see cref="GridModel"/> class that holds all data information about a grid and provides methods to completely initialize a grid
    /// and attach it later to a <see cref="GridControlBase"/> so that its contents can be rendered to the screen.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridModel"/> holds all data for a grid. You can initialize a <see cref="GridModel"/> and then attach it to one or more <see cref="GridControlBase"/>
    /// controls. If you share the same model among several <see cref="GridControlBase"/> controls, changes in the model will be automatically reflected in
    /// all associated controls.
    /// <para/>
    /// If the user makes changes in one control, e.g. row heights or cell contents, the changes will be stored in the GridModel and events are
    /// raised that notify all associated controls about changes in the model so that each view can update its contents. This follows the
    /// Model-View-Controller pattern where data is separated from view.
    /// <para/>
    /// <see cref="GridModel"/> offers many events that you can subscribe to and modify the default behavior of the grid. Typically, methods
    /// that are sent before the action is carried out allows you to adjust certain parameters or cancel the operation. These events usually
    /// end with an "ing" suffix, like "Changing". Events that are raised after the data in the model have been changed inform associated controls
    /// about the success of the operation. For example, if a <see cref="GridControlBase"/> receives a <see cref="GridModel.CellsChanged"/> event,
    /// it will redraw the affected cells. Most of these events have a <see cref="SyncfusionSuccessEventArgs.Success"/> property that indicates
    /// if the operation finished successfully or failed to complete.<para/>
    /// A <see cref="GridModel"/> holds a <see cref="GridCellModelCollection"/> that holds models for all cell types used in the grid.
    /// If a cell queries for a new cell type that is not found in the cell model collection, it tries to instantiate a <see cref="GridCellModelBase"/>
    /// object for the specific cell type. A <see cref="GridModel.QueryCellModel"/> event is raised to allow you to create custom cell
    /// types on demand. But you can also instantiate cell models for custom cell types at initialization and add them to <see cref="GridModel.CellModels"/>.
    /// <para/>
    /// There is also a <see cref="GridControl"/> class that combines both <see cref="GridModel"/> and <see cref="GridControlBase"/> into one class. This
    /// gives you easier access to all methods of <see cref="GridModel"/> if you are working on a grid that does not need to support several views
    /// sharing the same model.
    /// </remarks>
    /// <seealso cref="GridControlBase"/>
    /// <seealso cref="GridControl"/>
    [Serializable]
    [ToolboxItem(false)]
    public class GridModel :
        Component,
        IGridVolatileDataContainer,
        IOperationFeedbackProvider,
        ICreateControl,
        IGridModelSource,
        ISerializable,
        IDisposable,
        IDeserializationCallback
    ////, ISupportUpdating
    {
        //// Fields
        internal GridData data;
        internal GridBaseStylesMap styleInfoMap = null;
        [NonSerialized]
        internal IGridData volatileData;
        [NonSerialized]
        internal IGridVolatileData gridVolatileData;
        [NonSerialized]
        internal string lastSyncText = string.Empty;
        internal GridNamespaceGroupItemCollection ngic;
        static bool allowNewGridVolatileData = false;

        /// <summary>
        /// Gets or sets <see cref="IGridData"/> that defines an interface that <see cref="GridStyleInfoIdentity"/>
        /// utilizes to query cell contents and base styles, look up cell types, and save changes back to the
        /// grid.
        /// </summary>
        public IGridData VolatileData
        {
            get
            {
                return volatileData;
            }

            set
            {
                volatileData = value;
                gridVolatileData = volatileData as IGridVolatileData;
            }
        }

        /// <exclude/>
        /// <summary>Gets or sets NamespaceCollection. Used internally.</summary>
        public GridNamespaceGroupItemCollection NamespaceCollection
        {
            get
            {
                return ngic;
            }

            set
            {
                ngic = value;
            }
        }

        bool inDispose = false;
        bool isDisposed = false;

        /// <override/>
        /// <summary>Releases all the resources used by this component.</summary>
        public new void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            inDispose = true;
            ((Component)this).Dispose();
            inDispose = false;
            isDisposed = true;
        }

        /// <summary>
        /// Gets a value indicating whether true if object is executing <see cref="Dispose()"/> method call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposing
        {
            get
            {
                return inDispose;
            }
        }

        /// <summary>
        /// Gets a value indicating whether object has been disposed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (feedbackStack != null)
            {
                feedbackStack.Clear();
                feedbackStack = null;
            }

            if (savedGridModelGraphicsProvider != null)
            {
                savedGridModelGraphicsProvider.Dispose();
                savedGridModelGraphicsProvider = null;
            }

            if (this.volatileData is IDisposable)
            {
                ((IDisposable)volatileData).Dispose();
            }

            this.volatileData = null;

            this.activeGridView = null;
            this.banneredRangesObject = null;

            if (cellModels != null)
            {
                this.cellModels.Dispose();
            }

            this.cellModels = null;

            if (colHidden != null)
            {
                this.colHidden.Dispose();
            }

            this.colHidden = null;

            if (cols != null)
            {
                this.cols.Dispose();
            }

            this.cols = null;

            if (colStyles != null)
            {
                this.colStyles.Dispose();
            }

            this.colStyles = null;

            if (colWidths != null)
            {
                this.colWidths.Dispose();
            }

            this.colWidths = null;

            if (commandStack != null)
            {
                this.commandStack.Dispose();
            }

            this.commandStack = null;

            this.coveredRangesObject = null;

            this.currentCellInfo = null;

            if (cutPaste != null)
            {
                this.cutPaste.Dispose();
            }

            this.cutPaste = null;

            this.data = null;

            this.DataProvider = null;
            this.deserializeList = null;
            this.dragDropModel = null;
            this.eventsTarget = null;
            this.floatingCellsObject = null;
            this.graphicsProviderRef = null;
            this.mergeCellsObject = null;
            this.operationFeedbackListener = null;

            if (ownOptions && options != null)
            {
                this.options.Dispose();
            }

            ////this.options = null;
            this.properties = null;

            if (rowHeights != null)
            {
                this.rowHeights.Dispose();
            }

            this.rowHeights = null;

            if (rowHidden != null)
            {
                this.rowHidden.Dispose();
            }

            this.rowHidden = null;

            if (rows != null)
            {
                this.rows.Dispose();
            }

            this.rows = null;

            if (rowStyles != null)
            {
                this.rowStyles.Dispose();
            }

            this.rowStyles = null;

            this.savedFloatingCellsObject = null;
            this.selectedRanges = null;

            if (selections != null)
            {
                this.selections.Dispose();
            }

            this.selections = null;

            if (styleInfoMap != null)
            {
                this.styleInfoMap.Dispose();
            }

            this.styleInfoMap = null;

            if (textDataExchange != null)
            {
                this.textDataExchange.Dispose();
            }

            this.textDataExchange = null;

            this.userData = null;
            this.volatileData = null;

            base.Dispose(disposing);
            isDisposed = true;
        }

        private IGridModelEventsTarget eventsTarget;

        /// <summary>
        /// Gets or sets GridModelEventsTarget. Redirects events defined in <see cref="IGridModelEventsTarget"/> to the specified object.
        /// Each event will first be called on <see cref="GridModelEventsTarget"/> before the actual
        /// event handler in this object is called.
        /// </summary>
        public IGridModelEventsTarget GridModelEventsTarget
        {
            get
            {
                return eventsTarget;
            }

            set
            {
                eventsTarget = value;
            }
        }

        GridRangeInfoList selectedRanges = null;
        ////GridRangeInfoList coveredRangesObject = null;

        [NonSerialized]
        internal bool modified = false;

        [NonSerialized]
        internal string fileName = string.Empty;

        [NonSerialized]
        internal int updateCount = 0;

        [NonSerialized]
        internal bool updatePending = false;

        //// TODO: [NonSerialized] GridRangeInfo updateRange = GridRangeInfo.Empty;
        ////[NonSerialized] GridRangeInfo updateRange = GridRangeInfo.Empty;
        [NonSerialized]
        internal int currentRow;

        [NonSerialized]
        internal int currentCol;

        [NonSerialized]
        internal bool selectionStateChanged = false;

        [NonSerialized]
        internal GridControlBase activeGridView = null;

        [NonSerialized]
        internal bool inInit;

        [NonSerialized]
        internal string updateCommand;

        ////        static GridModel emptyModel = null;
        ////        internal static GridModel Empty
        ////        {
        ////            get
        ////            {
        ////                if (emptyModel == null)
        ////                    emptyModel = new GridModel();
        ////                return emptyModel;
        ////            }
        ////        }

        GridCellInfoCollection gridCells = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridCellInfo"/> objects. This collection is a wrapper collection
        /// for cells in the <see cref="GridData"/> object. It provides support for code serialization at design-time.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridCellInfoCollection GridCells
        {
            get
            {
                if (gridCells == null)
                {
                    gridCells = new GridCellInfoCollection(this.Data.sfTable);
                }

                return gridCells;
            }

            set
            {
                if (!Object.ReferenceEquals(gridCells, value))
                {
                    GridCells.WriteToData(value);

                    ////null out the gridCells member so that a fresh pull is retrieved from the data object
                    gridCells = null;
                    this.rangeStyles = null;
                }
            }
        }

        /// <summary>
        /// Clears all cell formatting.   
        /// </summary>
        public void ResetGridCells()
        {
            gridCells = null;
        }

        GridRangeStyleCollection rangeStyles = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridRangeStyle"/> objects. This collection is a wrapper collection
        /// for cells in the <see cref="GridData"/> object. It provides support for modifying
        /// cells through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRangeStyleCollection RangeStyles
        {
            get
            {
                if (rangeStyles == null)
                {
                    rangeStyles = new GridRangeStyleCollection(this.Data.sfTable);
                }

                ResetGridCells();
                return rangeStyles;
            }

            set
            {
                if (!Object.ReferenceEquals(rangeStyles, value))
                {
                    RangeStyles.WriteToData(this.Data.sfTable);
                }
            }
        }

        /// <summary>
        /// Clears all cell formatting in the <see cref="GridData"/> object.
        /// </summary>
        public void ResetRangeStyles()
        {
            RangeStyles.Clear();
            RangeStyles.WriteToData();
        }

        /// <summary>
        ///   <para> Gets / sets the user-definable data for the current object.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDictionary UserData
        {
            get
            {
                if (this.userData == null)
                {
                    this.userData = new ListDictionary();
                }

                return this.userData;
            }
        }

        [NonSerialized]
        ListDictionary userData = null;

        GridModelOptions options = new GridModelOptions();
        bool ownOptions = true;

        /// <summary>
        /// Gets a <see cref="GridModelOptions"/> that allows you to adjust behavior and appearance of the grid.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelOptions Options
        {
            [DebuggerStepThrough()]
            get
            {
                return options;
            }
        }

        /// <exclude/>
        /// <summary>This method is supposed to be called only from the GridGroupingControl. 
        /// When used with regular grids there might be event listeners setup by the GridControlBase
        /// object that is attached to the GridModel and replace the GridModelOptions might 
        /// cause memory leaks in such case.
        /// </summary>
        /// <param name="options">A <see cref="GridModelOptions"/> object that provides properties to adjust the look and feel of grid.</param>
        public void SetOptions(GridModelOptions options)
        {
            if (options != this.options)
            {
                if (ownOptions && options != null)
                {
                    options.Dispose();
                }

                this.options = options;
                if (options.gridModel == null)
                {
                    options.gridModel = this;
                }

                ownOptions = false;
            }
        }

        GridModel IGridModelSource.Model
        {
            get { return this; }
        }

        /// <summary>
        /// Gets IGridModelSource interface. Returns this.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModel Model
        {
            [DebuggerStepThrough()]
            get { return this; }
        }

        OperationFeedbackListener operationFeedbackListener;

        /// <summary>
        /// Gets or sets a feedback handler for the grid model to give visual feedback about time-consuming operations.
        /// Default handler for visual feedback are <see cref="Syncfusion.Windows.Forms.DelayedStatusDialog"/> and
        /// <see cref="Syncfusion.Windows.Forms.DelayedWaitCursor"/>.
        /// </summary>
        /// <remarks>
        /// If you want to provide visual feedback about progress of time-consuming operations you should assign
        /// feedback listener to <see cref="GridModel.OperationFeedbackListener"/>. This could be done in your forms constructor
        /// or your derived classes constructor.
        /// <para/>
        /// See <see cref="Syncfusion.ComponentModel.OperationFeedback"/> for more in-depth discussion about operation feedback.
        /// </remarks>
        /// <example>
        /// The following example initializes a <see cref="DelayedStatusDialog"/> as listener.
        /// <code lang="C#">
        /// Syncfusion.Windows.Forms.DelayedStatusDialog handler = new Syncfusion.Windows.Forms.DelayedStatusDialog();
        /// handler.Delay = 1000;
        /// handler.ShowDialogPercentRule = 25; //default
        /// handler.ShowMousePercentRule = 75; //default values
        /// this.gridControl1.Model.OperationFeedbackListener = handler;
        /// </code>
        /// </example>
        /// <seealso cref="Syncfusion.ComponentModel.OperationFeedbackListener"/>
        /// <seealso cref="Syncfusion.ComponentModel.OperationFeedback"/>
        public OperationFeedbackListener OperationFeedbackListener
        {
            get
            {
                return operationFeedbackListener;
            }

            set
            {
                if (operationFeedbackListener != value)
                {
                    if (operationFeedbackListener != null)
                    {
                        operationFeedbackListener.RemoveProvider(this);
                    }

                    operationFeedbackListener = value;
                    if (operationFeedbackListener != null)
                    {
                        operationFeedbackListener.AddProvider(this);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the reference for <see cref="GridModel.BaseStylesMap"/> in <see cref="GridModel"/> has changed.
        /// </summary>
        [Description("Occurs when the reference to the BaseStylesMap has changed."),
        Category("Behavior")]
        public event EventHandler BaseStylesMapChanged;

        /// <summary>
        /// Occurs when <see cref="GridModel.Data"/> has changed.
        /// </summary>
        [Description("Occurs when the reference to the GridData has changed."),
        Category("Behavior")]
        public event EventHandler DataChanged;

        /// <summary>
        /// Occurs when <see cref="GridModel.Modified"/> has changed.
        /// </summary>
        [Description("Occurs when the modified flag has changed."),
        Category("Behavior")]
        public event EventHandler ModifiedChanged;

        /// <summary>
        /// Occurs when <see cref="GridModel.FileName"/> has changed.
        /// </summary>
        [Description("Occurs when FileName has changed."),
        Category("Behavior")]
        public event EventHandler FileNameChanged;

        /// <summary>
        /// Occurs after the contents of a specified range of cells have changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCellsChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the contents of a specified range of cells have changed."),
        Category("Behavior")]
        public event GridCellsChangedEventHandler CellsChanged;

        /// <summary>
        /// Occurs before the contents of a specified range of cells are being changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description(" Occurs before the contents of a specified range of cells are being changed."),
        Category("Behavior")]
        public event GridCellsChangingEventHandler CellsChanging;

        /// <summary>
        /// Occurs when any changes in the grid should be confirmed.
        /// </summary>
        /// <remarks>
        /// This event is raised as a result of a <see cref="GridModel.ConfirmPendingChanges"/> method call.
        /// <para/>
        /// Set <see cref="CancelEventArgs.Cancel"/> True if you cannot commit pending changes.
        /// </remarks>
        /// <see cref="GridModel.ConfirmPendingChanges"/>
        [Description("Occurs when any changes in the grid should be confirmed."),
        Category("Behavior")]
        public event CancelEventHandler ConfirmingPendingChanges;

        /// <summary>
        /// Occurs when Refresh is called.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridModel.Refresh"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs when Refresh was called."),
        Category("Behavior")]
        public event EventHandler RefreshRequest;

        /// <summary>
        /// Occurs when the first BeginUpdate was called for the grid model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs when the first BeginUpdate was called for the grid model."),
        Category("Behavior")]
        public event EventHandler BeginUpdateRequest;

        /// <summary>
        /// Occurs when EndUpdate was called for the grid model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridEndUpdateRequestEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs when the last EndUpdate was called for the grid model."),
        Category("Behavior")]
        public event GridEndUpdateRequestEventHandler EndUpdateRequest;

        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        /// <remarks>
        /// If you made changes to <see cref="GridSaveCellInfoEventArgs.Style"/> you should also
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether the style has been changed from its original settings.
        /// <para/>
        /// See DataBoundGrid source code for example.
        /// <note type="note">The intention of this event is to store data. See
        /// <see cref="GridCellsChangedEventArgs"/> for the related UI event after changes were made to the
        /// data store.</note>
        /// <para/>
        /// See <seea cref="GridSaveCellInfoEventArgs"/> for further discussion about this event.
        /// </remarks>
        /// <seealso cref="GridSaveCellInfoEventHandler"/>
        /// <seealso cref="GridModel.SaveCellInfo"/>
        /// <seealso cref="IGridModelDataProvider"/>
        /// <seealso cref="GridQueryCellInfoEventArgs"/>
        [Description("Occurs when the model is about to save style information about at specific cell."),
        Category("Data")]
        public event GridSaveCellInfoEventHandler SaveCellInfo;

        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell and a <see cref="DataProvider"/> (e.g. <see cref="GridModelDataBinder"/>)
        /// has been specified.
        /// </summary>
        /// <remarks>
        /// This is similar to the <see cref="SaveCellInfo"/> event. <para/>
        /// Is called before GridModelDataBinder.SaveCellInfo is called and gives you a chance to modify
        /// style data before they are saved in underlying data source. <para/>
        /// If you made changes to <see cref="GridSaveCellInfoEventArgs.Style"/>, you should also
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether the style has been changed from its original settings.
        /// <para/>
        /// <note type="note">The intention of this event is to store data. See
        /// <see cref="GridCellsChangedEventArgs"/> for the related UI event after changes were made to the
        /// data store.</note>
        /// <para/>
        /// See <see cref="GridSaveCellInfoEventArgs"/> and <see cref="IGridModelDataProvider.SaveCellInfo"/>
        /// for further discussion about this event.
        /// </remarks>
        /// <seealso cref="GridSaveCellInfoEventArgs"/>
        /// <seealso cref="GridModel.SaveCellInfo"/>
        /// <seealso cref="IGridModelDataProvider"/>
        /// <seealso cref="GridModelDataBinder"/>
        /// <seealso cref="GridSaveCellInfoEventArgs"/>
        [Description("Occurs when the model queries for style information about a specific cell."),
        Category("Data")]
        public event GridSaveCellInfoEventHandler DataProviderSaveCellInfo;

        /// <summary>
        /// Occurs for each cell when text is pasted from a buffer into several cells.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to handle text pasted into a cell at run-time on demand.
        /// <para/>
        /// If you do not wish the grid to make any changes to the cell,
        /// set <see cref="CancelEventArgs.Cancel"/> to True. The grid will check this
        /// flag to see whether it should make changes to the cell.
        /// <para/>
        /// If you do wish the grid to abort the current paste operation (in case several cells are pasted),
        /// set the <see cref="GridPasteCellTextEventArgs.Abort"/> flag to True.
        /// <para/>
        /// The GridPasteCellTextEventArgs members, e.ColIndex and e.RowIndex, specify column and row of the cell. The e.Style member holds the
        /// GridStyleInfo object for the cell.
        /// </remarks>
        /// <seealso cref="GridPasteCellTextEventHandler"/>
        /// <seealso cref="GridPasteCellTextEventArgs"/>
        /// <seealso cref="GridModel.PasteCellText"/>
        [Description("Occurs for each cell when text is pasted from a buffer into several cells."),
        Category("Clipboard")]
        public event GridPasteCellTextEventHandler PasteCellText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to get the raw string that represents the underlying cell's value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to represent a cell's value as string at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default conversion, you should assign the result string
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should return <see cref="GridCellTextEventArgs.Text"/>
        /// or use a default conversion.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellText"/>
        /// <seealso cref="GridModel.QueryCellText"/>
        /// <seealso cref="GridCellModelBase.GetText"/>
        /// <seealso cref="GridStyleInfo.Text"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to set the unformatted string that represents the underlying cell's value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to parse the unformatted text into a cell value at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default parsing behavior, you should assign the resulting value
        /// to the <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridStyleInfo"/> object
        /// and set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should accept your modification
        /// or use a default parsing routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, query the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// <para/>
        ///    See the <see cref="SaveCellFormattedText"/> event for further discussion since these two events
        ///    are very similar. Often you will need to handle both events in your code in the same way.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellText"/>
        /// <seealso cref="GridModel.QueryCellText"/>
        /// <seealso cref="GridCellModelBase.ApplyText"/>
        /// <seealso cref="GridStyleInfo.Text"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellText;

        /// <summary>
        /// Occurs when an undo or redo command has been added to the command stack. Check GridModelCommandManager.UndoStack
        /// or GridModelCommandManager.RedoStack to see the list of commands on the stack.
        /// </summary>
        [Description("Occurs when an undo or redo command has been added to the command stack."),
        Category("Behavior")]
        public event EventHandler CommandStackChanged;

        /// <summary>
        /// Raises the  <see cref="CommandStackChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCommandStackChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCommandStackChanged(e);
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (CommandStackChanged != null)
            {
                CommandStackChanged(this, e);
            }
        }

        internal void RaiseCommandStackChanged(EventArgs e)
        {
            OnCommandStackChanged(e);
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to get the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to format a cell's value as string at run-time on demand based on <see cref="GridStyleInfo.Format"/>.
        /// <para/>
        /// If you do want to customize the grid's default formatting, you should assign the resulting string
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should return <see cref="GridCellTextEventArgs.Text"/>
        /// or use a default formatting routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellFormattedText"/>
        /// <seealso cref="GridModel.QueryCellFormattedText"/>
        /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
        /// <seealso cref="GridStyleInfo.FormattedText"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellFormattedText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to parse the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to parse the formatted text into a cell value at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default parsing behavior, you should assign the resulting value
        /// to the <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridStyleInfo"/> object
        /// and set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should accept your modification
        /// or use a default parsing routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// <para/>
        /// This event is normally called from within <see cref="GridStyleInfo.ApplyFormattedText(string)"/>, which is called
        /// when the user enters text into a text box or when text is assigned to <see cref="GridStyleInfo.FormattedText"/>.
        /// ApplyFormattedText method checks if there are event handlers for <see cref="GridModel.SaveCellFormattedText"/> and
        /// if the <see cref="SyncfusionHandledEventArgs.Handled"/> is not set, they try to convert the input text into
        /// the type specified with <see cref="GridStyleInfo.CellValueType"/>.
        /// <para/>
        /// If this conversion fails, <see cref="GridStyleInfo.ApplyFormattedText(string)"/> will check <see cref="GridStyleInfo.StrictValueType"/>. If it
        /// is True, an exception is thrown which itself results in a warning message displayed to the user at the
        /// time from <see cref="GridControlBase.CurrentCellValidating"/>.
        /// <para/>
        /// If you set <see cref="GridStyleInfo.StrictValueType"/> to False, <see cref="GridStyleInfo.ApplyFormattedText(string)"/> will not throw
        /// an exception and simply store the text as <see cref="GridStyleInfo.CellValue"/>.
        /// <para/>
        /// If you need a more specialized customization of this behavior, you should handle the
        /// <see cref="GridModel.SaveCellFormattedText"/> event. This lets you parse the text input
        /// and change the cells <see cref="GridStyleInfo.CellValueType"/> at run-time. See the attached example.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellFormattedText"/>
        /// <seealso cref="GridModel.QueryCellFormattedText"/>
        /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
        /// <seealso cref="GridStyleInfo.FormattedText"/>
        /// <seealso cref="GridStyleInfo.CellValueType"/>
        /// <seealso cref="GridStyleInfo.StrictValueType"/>
        /// <example>
        /// This example parses the text input and changes the cell's CellValueType at run-time if the input does not match the current CellValueType.
        /// <code lang="C#">
        /// void InitializeComponent()
        /// {
        ///     // initialize code
        ///     // ...
        ///     this.gridControl1.SaveCellText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_SaveCellText);
        ///     this.gridControl1.QueryCellFormattedText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_QueryCellFormattedText);
        ///     this.gridControl1.SaveCellFormattedText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_SaveCellFormattedText);
        ///     this.gridControl1.QueryCellText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_QueryCellText);
        /// }
        /// <para/>
        /// private void gridControl1_QueryCellFormattedText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        /// <para/>
        /// }
        /// <para/>
        /// private void gridControl1_QueryCellText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        /// <para/>
        /// }
        /// <para/>
        /// private void gridControl1_SaveCellText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        ///     ParseText(e);
        /// }
        /// <para/>
        /// private void gridControl1_SaveCellFormattedText(object sender, GridCellTextEventArgs e)
        /// {
        ///     ParseText(e);
        /// }
        /// <para/>
        /// void ParseText(GridCellTextEventArgs e)
        /// {
        ///     // By default, the grid will display a warning message box informing the user
        ///     // the entered value is not valid and the user will have to change the value.
        ///     //
        ///     // In this event handler, we change the grid default's behavior such that
        ///     // when the user enters a value that does not fit the cell's CellValueType,
        ///     // the input text is accepted and no warning message is shown.
        ///     if (e.Handled)
        ///         return;
        /// <para/>
        ///     System.Globalization.CultureInfo ci = e.Style.CultureInfo;
        ///     System.Globalization.NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
        ///     try
        ///     {
        ///         e.Style.CellValue = GridCellValueConvert.Parse(e.Text, e.Style.CellValueType, nfi, e.Style.Format);
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         if (ex is FormatException || ex.InnerException is FormatException)
        ///         {
        ///             e.Style.CellValue = e.Text;
        ///             // possibly could also change CellValueType here
        ///             e.Style.CellValueType = typeof(string);
        ///             // - or -
        ///             // you could also further analyze the input text and assign a type
        ///             // that fits the input text, e.g.
        ///             // e.Style.CellValueType = typeof(datetime);
        ///             // - or -
        ///             // e.Style.CellValueType = typeof(decimal);
        ///             // etc.
        ///         }
        ///         else
        ///             throw;
        ///     }
        ///     e.Handled = true;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Private Sub InitializeComponent()
        ///     ' Initalize code
        ///     ' ...
        ///     AddHandler Me.gridControl1.SaveCellText, AddressOf Me.gridControl1_SaveCellText
        ///     AddHandler Me.gridControl1.QueryCellFormattedText, AddressOf Me.gridControl1_QueryCellFormattedText
        ///     AddHandler Me.gridControl1.SaveCellFormattedText, AddressOf Me.gridControl1_SaveCellFormattedText
        ///     AddHandler Me.gridControl1.QueryCellText, AddressOf Me.gridControl1_QueryCellText
        /// End Sub 'InitializeComponent
        /// <para/>
        /// Private Sub gridControl1_QueryCellFormattedText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        /// End Sub 'gridControl1_QueryCellFormattedText
        /// <para/>
        /// Private Sub gridControl1_QueryCellText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        /// End Sub 'gridControl1_QueryCellText
        /// <para/>
        /// Private Sub gridControl1_SaveCellText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        ///     ParseText(e)
        /// End Sub 'gridControl1_SaveCellText
        /// <para/>
        /// Private Sub gridControl1_SaveCellFormattedText(sender As Object, e As GridCellTextEventArgs)
        ///     ParseText(e)
        /// End Sub 'gridControl1_SaveCellFormattedText
        /// <para/>
        /// Sub ParseText(e As GridCellTextEventArgs)
        ///     ' By default, the grid will display a warning message box informing the user
        ///     ' the entered value is not valid and the user will have to change the value.
        ///     '
        ///     ' In this event handler we change the grid default's behavior such that
        ///     ' when the user enters a value that does not fit the cell's CellValueType,
        ///     ' the input text is accepted and no warning message is shown.
        ///     If e.Handled Then
        ///         Return
        ///     End If
        ///     Dim ci As System.Globalization.CultureInfo = e.Style.CultureInfo
        ///     Dim nfi As System.Globalization.NumberFormatInfo = Nothing
        ///     If (Not (ci Is Nothing)) Then nfi = ci.NumberFormat
        ///     Try
        ///         e.Style.CellValue = GridCellValueConvert.Parse(e.Text, e.Style.CellValueType, nfi, e.Style.Format)
        ///     Catch ex As Exception
        ///         If TypeOf ex Is FormatException OrElse TypeOf ex.InnerException Is FormatException Then
        ///             e.Style.CellValue = e.Text
        ///         ' possibly could also change CellValueType here
        ///         ' e.Style.CellValueType = typeof(string);
        ///         ' - or -
        ///         ' you could also further analyze the input text and assign a type
        ///         ' that fits the input text, e.g.
        ///         ' e.Style.CellValueType = typeof(datetime);
        ///         ' - or -
        ///         ' e.Style.CellValueType = typeof(decimal);
        ///         ' etc.
        ///         Else
        ///             Throw
        ///         End If
        ///     End Try
        ///     e.Handled = True
        /// End Sub 'ParseText
        /// </code>
        /// </example>
        [Description("Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellFormattedText;

        /// <summary>
        /// Use this event to provide support for parsing the formatted string and convert
        /// it into the the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// <para/>
        /// This event is raised from GridCellModelBase.ApplyFormattedText after 
        /// <see cref="SaveCellFormattedText"/> was raised. The event is raised only 
        /// if the SaveCellFormattedText did not set e.Handled. 
        /// </summary>
        /// <remarks>
        /// The grid has built-in support for parsing the Percent format (Format = "P") and Hexadecimal
        /// format (Format = "X"). You should handle this event if you want to add support
        /// for other formats. <para/>
        /// GridCellTextEventArgs has information about the style settings of the cell. You can
        /// inspect that style to get information about Format and CellValueType of the cell.
        /// </remarks>
        [Description("Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler ParseCommonFormats;

        /// <summary>
        /// Raises the <see cref="QueryCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCellText(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (QueryCellText != null)
            {
                QueryCellText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryCellText(GridCellTextEventArgs e)
        {
            OnQueryCellText(e);
        }

        /// <summary>
        /// Raises the <see cref="SaveCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellText(GridCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveCellText(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (SaveCellText != null)
            {
                SaveCellText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSaveCellText(GridCellTextEventArgs e)
        {
            OnSaveCellText(e);
        }

        /// <summary>
        /// Raises the <see cref="QueryCellFormattedText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCellFormattedText(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (QueryCellFormattedText != null)
            {
                QueryCellFormattedText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryCellFormattedText(GridCellTextEventArgs e)
        {
            OnQueryCellFormattedText(e);
        }

        /// <summary>
        /// Raises the <see cref="SaveCellFormattedText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveCellFormattedText(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (SaveCellFormattedText != null)
            {
                SaveCellFormattedText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSaveCellFormattedText(GridCellTextEventArgs e)
        {
            OnSaveCellFormattedText(e);
        }

        /// <summary>
        /// Raises the <see cref="ParseCommonFormats"/> event. 
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnParseCommonFormats(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ParseCommonFormats != null)
            {
                ParseCommonFormats(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseParseCommonFormats(GridCellTextEventArgs e)
        {
            OnParseCommonFormats(e);

            DefaultParseCommonFormats(e);
        }

        internal void DefaultParseCommonFormats(GridCellTextEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Text == string.Empty || e.Text == null)
                {
                    return;
                }
                ////If this is a numeric type with a common format that is not automatically handled by the Parse
                ////routine, we'll take care of it here.

                switch (e.Style.Format)
                {
                    case "P":
                        if (!e.Text.EndsWith("%"))
                        {
                            return;
                        }

                        string s = e.Text;
                        s = s.TrimEnd('%');
                        decimal d = decimal.Parse(s, System.Globalization.NumberStyles.Any, e.Style.GetCulture(true)) * (decimal)0.01;
                        e.Style.CellValue = Convert.ChangeType(d, e.Style.CellValueType);
                        e.Handled = true;
                        break;
                    case "X":
                        Int64 i = Int64.Parse(e.Text, System.Globalization.NumberStyles.AllowHexSpecifier, e.Style.GetCulture(true));
                        e.Style.CellValue = Convert.ChangeType(i, e.Style.CellValueType);
                        e.Handled = true;
                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanPaste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you want the standard Paste method to ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's CanPaste method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardCanPaste;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Paste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you want the standard Paste to method ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's Paste method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardPaste;

        /// <summary>
        /// Occurs after the <see cref="GridModelCutPaste.Paste"/> was called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>
        /// and text was pasted into the grid.
        /// </summary>
        /// <remarks>
        /// This event is called to inform you that text or cells have been pasted.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs after called text or cells have been pasted."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardPasted;

        // eventr GridOleDropAtRowCol OleDropAtRowCol

        /// <summary>
        /// Occurs when the when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
        /// before the data are applied to the grid.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste data behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will check <see cref="GridOleDropAtRowColEventArgs.Result"/> as indication if the
        /// operation was successful.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// </remarks>
        /// <seealso cref="GridOleDropAtRowColEventHandler"/>
        [Description("Occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and before the data are applied to the grid."),
        Category("Behavior")]
        public event GridOleDropAtRowColEventHandler OleDropAtRowCol;

        /// <summary>
        /// Raises the  <see cref="OleDropAtRowCol"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridOleDropAtRowColEventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveCellFormattedText(e);
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, this, e);
            if (OleDropAtRowCol != null)
            {
                OleDropAtRowCol(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            OnOleDropAtRowCol(e);
        }

        /// <summary>
        /// Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
        /// the data were applied to the grid.
        /// </summary>
        [Description("Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and the data were applied to the grid."),
        Category("Behavior")]
        public event EventHandler OleDroppedData;

        /// <summary>
        /// Raises the <see cref="OleDroppedData"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDroppedData(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnOleDroppedData(e);
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (OleDroppedData != null)
            {
                OleDroppedData(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseOleDroppedData(EventArgs e)
        {
            OnOleDroppedData(e);
        }

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanCut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you want the standard Cut method to ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's CanCut method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardCanCut;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Cut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can then specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you want the standard Cut method to ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's Cut method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardCut;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can then specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you do want the standard Copy method to ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's Copy method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardCopy;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized paste behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridCutPasteEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can then specify <see cref="GridCutPasteEventArgs.IgnoreCurrentCell"/> if you want the standard Copy method to ignore the current cell.
        /// <para/>
        /// If the user has selected a range of cells and paste is called, you can force the clipboard contents to be pasted into the
        /// selected range and not at the current cell's position.
        /// </remarks>
        /// <seealso cref="GridCutPasteEventHandler"/>
        [Description("Occurs when the grid's CanCopy method is called."),
        Category("Behavior")]
        public event GridCutPasteEventHandler ClipboardCanCopy;

        /// <summary>
        /// Occurs when the text copies to buffer
        /// </summary>
        /// <remarks>
        /// This event lets you to modify the text which is being copied to the buffer.
        /// </remarks>
        /// <seealso cref="ClipboardCopyToBufferEventHandler"/>
        [Description("Occurs when the grid's data copied to the buffer is called."),
        Category("Behavior")]
        public event ClipboardCopyToBufferEventHandler ClipboardCopyToBuffer;

        /// <summary>
        /// Occurs when the <see cref="GridModel.ClearCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,bool)"/> is called on a <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// This event lets you provide your own customized clear cells behavior.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridClearingCellsEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// </remarks>
        /// <seealso cref="GridClearingCellsEventHandler"/>
        [Description("Occurs when the grid's ClearCells method is called."),
        Category("Behavior")]
        public event GridClearingCellsEventHandler ClearingCells;

        /// <summary>
        /// Occurs when a user starts dragging a range of selected cells
        /// using OLE drag-and-drop.
        /// </summary>
        /// <remarks>
        /// This event lets you supply your own clipboard formats or add support for pasting additional clipboard content.
        /// <para/>
        /// If you do not wish the grid to proceed with default behavior for this method,
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether it should proceed. If you set it to True, the calling method
        /// will return <see cref="GridQueryOleDataSourceDataEventArgs.Result"/> as return value.
        /// <para/>
        /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
        /// You can specify <see cref="GridQueryOleDataSourceDataEventArgs.IgnoreCurrentCell"/> if you want the standard Copy method to ignore the current cell
        /// or change <see cref="GridQueryOleDataSourceDataEventArgs.DragDropFlags"/> for more advanced options.
        /// <para/>
        /// This event lets you customize the OLE Data Source behavior of an OLE drag-and-drop operation. See the
        /// <see cref="IGridDataObjectConsumer"/> interface for customizing the OLE DropTarget part of an OLE drag-and-drop operation.
        /// </remarks>
        /// <seealso cref="GridQueryOleDataSourceDataEventHandler"/>
        [Description("Occurs when a user starts dragging a range of selected cells using OLE drag-and-drop."),
        Category("Behavior")]
        public event GridQueryOleDataSourceDataEventHandler QueryOleDataSourceData;

        /// <summary>
        /// Raises the <see cref="QueryOleDataSourceData"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryOleDataSourceDataEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryOleDataSourceData(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (QueryOleDataSourceData != null)
            {
                QueryOleDataSourceData(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            OnQueryOleDataSourceData(e);
        }

        /// <summary>
        /// Occurs when the user drops data onto another control using OLE drag-and-drop
        /// and does not press the Control Key. Set e.Cancel = True for this event if you do not
        /// want the grid to clear cell contents of the dragged cells.
        /// </summary>
        [Description("Occurs when a user drops data onto another control using OLE drag-and-drop and does not press the Control Key."),
        Category("Behavior")]
        public event CancelEventHandler QueryDragDropMoveClearCells;

        /// <summary>
        /// Raises the <see cref="QueryDragDropMoveClearCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryDragDropMoveClearCells(e);
            }

            if (QueryDragDropMoveClearCells != null)
            {
                QueryDragDropMoveClearCells(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            OnQueryDragDropMoveClearCells(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCanPaste(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (ClipboardCanPaste != null)
            {
                ClipboardCanPaste(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCanPaste(GridCutPasteEventArgs e)
        {
            OnClipboardCanPaste(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardPaste(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardPaste(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ClipboardPaste != null)
            {
                ClipboardPaste(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardPaste(GridCutPasteEventArgs e)
        {
            OnClipboardPaste(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardPasted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardPasted(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardPasted(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ClipboardPasted != null)
            {
                ClipboardPasted(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardPasted(GridCutPasteEventArgs e)
        {
            OnClipboardPasted(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCut(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCanCut(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ClipboardCanCut != null)
            {
                ClipboardCanCut(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCanCut(GridCutPasteEventArgs e)
        {
            OnClipboardCanCut(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCut(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCut(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ClipboardCut != null)
            {
                ClipboardCut(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCut(GridCutPasteEventArgs e)
        {
            OnClipboardCut(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCopy(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCopy(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ClipboardCopy != null)
            {
                ClipboardCopy(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCopy(GridCutPasteEventArgs e)
        {
            OnClipboardCopy(e);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCanCopy(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ClipboardCanCopy != null)
            {
                ClipboardCanCopy(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCanCopy(GridCutPasteEventArgs e)
        {
            OnClipboardCanCopy(e);
        }

         /// <summary>
        /// Raises the <see cref="OnClipBoardCopyToBuffer"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ClipboardCopyToBufferEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipBoardCopyToBuffer(ClipboardCopyToBufferEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClipboardCopyToBuffer(e);
            }
            if (this.ClipboardCopyToBuffer != null)
            {
                this.ClipboardCopyToBuffer(this, e);
            }

        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClipboardCopyToBuffer(ClipboardCopyToBufferEventArgs e)
        {
            OnClipBoardCopyToBuffer(e);
        }
        /// <summary>
        /// Raises the <see cref="ClearingCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridClearingCellsEventArgs" /> that contains the event data.</param>
        protected virtual void OnClearingCells(GridClearingCellsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnClearingCells(e);
            }

            if (ClearingCells != null)
            {
                ClearingCells(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseClearingCells(GridClearingCellsEventArgs e)
        {
            OnClearingCells(e);
        }

        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// </summary>
        /// <remarks>
        /// This event allows you to customize cell contents at run-time on demand, just before
        /// the cell is drawn or programmatically accessed through <see cref="GridModel.this[int,int]"/>,
        /// <see cref="GridModel.ColStyles"/>, <see cref="GridModel.RowStyles"/>,
        /// or <see cref="GridModel.TableStyle"/>.
        /// <para/>
        /// If you made changes to <see cref="GridQueryCellInfoEventArgs.Style"/> you should also
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to true. The grid will check this
        /// flag to see whether the style has been initialized. If the event has been marked as
        /// handled, the grid will not access cell information from its own data store
        /// <see cref="GridModel.Data"/>. In the default case when the event is not marked as handled,
        /// the grid will locate cell information by calling <see cref="GridData.this"/>.
        /// <para/>
        /// See <seea cref="GridQueryCellInfoEventArgs"/> for further discussion about this event.
        /// </remarks>
        /// <seealso cref="GridQueryCellInfoEventArgs"/>
        /// <seealso cref="GridModel.SaveCellInfo"/>
        /// <seealso cref="IGridModelDataProvider"/>
        /// <seealso cref="GridSaveCellInfoEventArgs"/>
        [Description("Occurs when the model queries for style information about a specific cell."),
        Category("Behavior")]
        public event GridQueryCellInfoEventHandler QueryCellInfo;

        /// <summary>
        /// Occurs when the model queries for style information about a specific cell and a <see cref="DataProvider"/> (e.g. <see cref="GridModelDataBinder"/>)
        /// has been specified.
        /// </summary>
        /// <remarks>
        /// This event allows you to customize cell contents at run-time on demand, just before
        /// the <see cref="IGridModelDataProvider.QueryCellInfo"/> of the <see cref="DataProvider"/>
        /// is called.
        /// <para/>
        /// If you make changes to <see cref="GridQueryCellInfoEventArgs.Style"/> you should also
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
        /// flag to see whether the style has been initialized. If the event has been marked as
        /// handled, the grid will not access cell information from its own data store
        /// or from <see cref="DataProvider"/>. In the default case when the event is not marked as handled,
        /// the grid will locate cell information by calling <see cref="IGridModelDataProvider.QueryCellInfo"/>
        /// and <see cref="GridData.this"/>.
        /// <para/>
        /// See <seea cref="GridQueryCellInfoEventArgs"/> and <see cref="IGridModelDataProvider.QueryCellInfo"/>
        /// for further discussion about this event.
        /// </remarks>
        /// <seealso cref="GridQueryCellInfoEventArgs"/>
        /// <seealso cref="GridModel.SaveCellInfo"/>
        /// <seealso cref="IGridModelDataProvider"/>
        /// <seealso cref="GridModelDataBinder"/>
        /// <seealso cref="GridSaveCellInfoEventArgs"/>
        [Description("Occurs when the model queries for style information about a specific cell."),
        Category("Behavior")]
        public event GridQueryCellInfoEventHandler DataProviderQueryCellInfo;

        /// <summary>
        /// Occurs when the model queries information about covered cells at a specific cell.
        /// </summary>
        /// <remarks>
        /// This event allows you to specify covered ranges at run-time, e.g when you have
        /// a large grid with repeating patterns of covered ranges. If the specified row and
        /// column index is part of a covered cells range, you should assign the coordinates
        /// of the covered cell to <see cref="GridQueryCoveredRangeEventArgs.Range"/> and
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
        /// <para/>
        /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
        /// from your event handler and no further querying for data about covered range information
        /// for this cell is necessary.
        /// <para/>
        /// See the VirtualGrid sample for an example how to use this event.
        /// </remarks>
        /// <seealso cref="GridQueryCoveredRangeEventHandler"/>
        [Description("Occurs when the model queries information about covered cells at a specific cell."),
        Category("Behavior")]
        public event GridQueryCoveredRangeEventHandler QueryCoveredRange;

        /// <summary>
        /// Occurs when the model queries information about a bannered range at a specific cell.
        /// </summary>
        /// <remarks>
        /// This event allows you to specify bannered ranges at run-time, e.g when you have
        /// a large grid with repeating patterns of bannered ranges. If the specified row and
        /// column index is part of a bannered cells range you should assign the coordinates
        /// of the bannered cell to <see cref="GridQueryBanneredRangeEventArgs.Range"/> and
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
        /// <para/>
        /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
        /// from your event handler and no further querying for data about bannered range information
        /// for this cell is necessary.
        /// <para/>
        /// See the BannerCells sample for an example how to use this event.
        /// </remarks>
        /// <seealso cref="GridQueryBanneredRangeEventHandler"/>
        [Description("Occurs when the model queries information about a bannered range at a specific cell."),
        Category("Behavior")]
        public event GridQueryBanneredRangeEventHandler QueryBanneredRange;

        /// <summary>
        /// Occurs before the column count is returned from the model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColCountEventArgs"/> for a more detailed discussion.
        /// </remarks>
        [Description("Occurs before the column count is returned from the model."),
        Category("Behavior")]
        public event GridRowColCountEventHandler QueryColCount;

        /// <summary>
        /// Occurs before the row count is returned from the model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColCountEventArgs"/> for a more detailed discussion.
        /// </remarks>
        [Description("Occurs before the row count is returned from the model."),
        Category("Behavior")]
        public event GridRowColCountEventHandler QueryRowCount;

        /// <summary>
        /// Occurs before the column count is changed in the model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColCountEventArgs"/> for a more detailed discussion.
        /// </remarks>
        [Description("Occurs before the column count is changed in the model."),
        Category("Behavior")]
        public event GridRowColCountEventHandler SaveColCount;

        /// <summary>
        /// Occurs before the row count is changed in the model.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColCountEventArgs"/> for a more detailed discussion.
        /// </remarks>
        [Description("Occurs before the row count is changed in the model."),
        Category("Behavior")]
        public event GridRowColCountEventHandler SaveRowCount;

        /// <summary>
        /// Occurs when the <see cref="GridModel.QueryCellModel"/> is querying for the <see cref="GridCellModelBase"/>
        /// and the cell type is not found in the GridCellModelCollection.
        /// </summary>
        /// <remarks>
        /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
        /// a new cell type that it cannot find in the table it will raise a <see cref="GridModel.QueryCellModel"/> event.
        /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The
        /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
        /// associated cell object. This object will be stored in the table together with its name and
        /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
        /// <para/>
        /// You should process this event if you want to add custom cell types and initialize these
        /// cell types on demand when associated cells are accessed the first time.
        /// </remarks>
        /// <seealso cref="GridQueryCellModelEventHandler"/>
        [Description("Occurs is querying for a cell type."),
        Category("Behavior")]
        public event GridQueryCellModelEventHandler QueryCellModel;

        /// <summary>
        /// Raises the <see cref="GridModel.QueryCellModel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellModelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCellModel(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (QueryCellModel != null)
            {
                QueryCellModel(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryCellModel(GridQueryCellModelEventArgs e)
        {
            OnQueryCellModel(e);

            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellObjectFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellObjectFactory != null)
                {
                    e.CellModel = pGridCellObjectFactory.CreateCellModel(e.CellType, this);
                }
                else
                {
                    pGridCellObjectFactory = new GridBaseCellModelFactory(true);
                    GridFactoryProvider.Init(pGridCellObjectFactory);
                    e.CellModel = pGridCellObjectFactory.CreateCellModel(e.CellType, this);
                }
            }
        }

        /// <summary>
        /// Occurs when the CellModels collection is changed.
        /// </summary>
        [Description("Occurs when the CellModels collection is changed"),
        Category("Behavior")]
        public event CollectionChangeEventHandler CellModelsChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.CellModelsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CollectionChangeEventArgs" /> that contains the event data.</param>// Events
        protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellModelsChanged(e);
            }

            if (this.ActiveGridView != null)
            {
                ActiveGridView.CurrentCell.gridModel_CellModelsChanged(this, e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e.Action, e.Element);
            }
#else

            ;
#endif

            if (CellModelsChanged != null)
            {
                CellModelsChanged(this, e);
            }
        }

        internal bool ignoreCellModelsChanged = false;

        internal void RaiseCellModelsChanged(CollectionChangeEventArgs e)
        {
            if (!ignoreCellModelsChanged)
            {
                OnCellModelsChanged(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCoveredRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCoveredRange(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            try
            {
                if (QueryCoveredRange != null)
                {
                    QueryCoveredRange(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            OnQueryCoveredRange(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryBanneredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryBanneredRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryBanneredRange(GridQueryBanneredRangeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryBanneredRange(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            try
            {
                if (QueryBanneredRange != null)
                {
                    QueryBanneredRange(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseQueryBanneredRange(GridQueryBanneredRangeEventArgs e)
        {
            OnQueryBanneredRange(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCellInfo(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (QueryCellInfo != null)
                {
                    QueryCellInfo(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Triggers a call to the <see cref="OnQueryCellInfo"/> method which raises <see cref="QueryCellInfo"/> event
        /// </summary>
        /// <param name="e">Event data.</param>
        public void RaiseQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            OnQueryCellInfo(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DataProviderQueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellInfoEventArgs"/> that contains the event data.</param>
        protected virtual void OnDataProviderQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDataProviderQueryCellInfo(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (DataProviderQueryCellInfo != null)
                {
                    DataProviderQueryCellInfo(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseDataProviderQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            OnDataProviderQueryCellInfo(e);
        }

        /// <summary>
        /// Gets or sets the active grid view.
        /// </summary>
        /// <remarks>
        /// If there are several views associated with this model, only one <see cref="GridControlBase"/> can be active.
        /// <para/>
        /// Changing the active view will result in calls to <see cref="GridCurrentCell.Deactivate"/>
        /// and <see cref="GridCurrentCell.Activate(int, int)"/> for the involved controls.
        /// <para/>
        /// </remarks>
        public GridControlBase ActiveGridView
        {
            get
            {
                return activeGridView;
            }

            set
            {
                if (activeGridView != value)
                {
#if DEBUG
                    if (Switches.Development.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(value, "Old View:", activeGridView);
                    }
#else
                    ;
#endif
                    if (activeGridView != null)
                    {
                        activeGridView.NotifyDeactivate();
                    }

                    activeGridView = value;
                    int row, col;
                    if (activeGridView != null)
                    {
                        activeGridView.NotifyActivate();
                    }

                    if (value.CurrentCell.GetCurrentCell(out row, out col))
                    {
                        SetActiveCurrentCell(row, col);
                    }
                }
            }
        }

        /// <summary>
        /// This is used in GridCurrentCell to record the current cell movement for undo and redo playback.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        internal void SetActiveCurrentCell(int row, int col)
        {
            // TODO: see SynchronizeCurrentCell
            if (row != currentRow || col != currentCol)
            {
                currentRow = row;
                currentCol = col;
                selectionStateChanged = true;
            }
        }

        #region Updating
        /// <overload>
        /// Suspends the painting of associated grid controls until the <see cref="EndUpdate()"/> method is called.
        /// </overload>
        /// <summary>
        /// Suspends the painting of associated grid controls until the <see cref="EndUpdate()"/> method is called.
        /// </summary>
        /// <remarks>
        /// <para>When many paints are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.</para>
        /// <para>
        /// Pass BeginUpdateOptions if you do not want to do a complete Refresh of the control and instead
        /// want to have certain regions of your control be invalidated or scroll the contents of control.</para>
        /// If you call BeginUpdate() and then later EndUpdate(), the control will know if a paint is pending and only
        /// refresh the control if a paint is pending. A call to ShouldPrepareUpdate, Invalidate, or a WM_PAINT message during
        /// the BeginUpdate EndUpdate block will signal the control that a paint is pending.
        /// <para/>
        /// This method will raise a <see cref="GridModel.BeginUpdateRequest"/> event.
        /// Each attached <see cref="GridControlBase"/> listens to this event and calls <see cref="ScrollControl.BeginUpdate()"/>
        /// for the current control.
        /// </remarks>
        /// <seealso cref="GridModel.EndUpdate()"/>
        public void BeginUpdate()
        {
            BeginUpdate(BeginUpdateOptions.None, string.Empty);
        }

        /// <summary>
        /// Suspends the painting of associated grid controls until the <see cref="EndUpdate()"/> method is called.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <genoverload/>
        public void BeginUpdate(BeginUpdateOptions options)
        {
            BeginUpdate(options, string.Empty);
        }

        /// <summary>
        /// Suspends the painting of associated grid controls until the <see cref="EndUpdate()"/> method is called and records a command description
        /// why painting is suspended.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <param name="commandDesc">A description of the command.</param>
        /// <genoverload/>
        public virtual void BeginUpdate(BeginUpdateOptions options, string commandDesc)
        {
            if (!inInit)
            {
                if (!inBeginUpdate)
                {
                    ////inBeginUpdate = true;
#if DEBUG
                    if (Switches.BeginEndUpdate.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo("Level " + this.updateCount.ToString());
                    }
#else
                    ;
#endif

                    Trace.Indent();
                    if (this.updateCount++ == 0)
                    {
                        this.updateOptions = options;
                        this.updateCommand = commandDesc;
                    }
                    else
                    {
                        this.updateOptions &= options;
                    }

                    OnBeginUpdateRequest(EventArgs.Empty);
                    ////inBeginUpdate = false;
                }
            }
        }

        [NonSerialized]
        BeginUpdateOptions updateOptions = BeginUpdateOptions.None;
        [NonSerialized]
        bool inBeginUpdate = false;
        [NonSerialized]
        bool inEndUpdate = false;

        /// <summary>
        /// Gets the painting support during the BeginUpdate, EndUpdate batch.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeginUpdateOptions UpdateOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return updateOptions;
            }
        }

        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </summary>
        /// <remarks>
        /// When many paint are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.
        /// <para/>
        /// This method will raise a <see cref="GridModel.EndUpdateRequest"/> event.
        /// Each attached <see cref="GridControlBase"/> listens to this event and call <see cref="ScrollControl.EndUpdate()"/>
        /// for the current control.
        /// </remarks>
        public void EndUpdate()
        {
            EndUpdate(true);
        }

        /// <summary>
        /// Cancel any pending BeginUpdate calls.
        /// </summary>
        public void CancelUpdate()
        {
            while (updateCount > 0)
            {
                EndUpdate(false);
            }
        }

        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method and allows you
        /// to specify whether current pending paint operations should be discarded.
        /// </summary>
        /// <param name="update">Specifies whether current pending paint operations should be discarded.</param>
        public virtual void EndUpdate(bool update)
        {
            if (!inInit)
            {
                if (!inEndUpdate)
                {
                    ////inEndUpdate = true;
#if DEBUG
                    if (Switches.BeginEndUpdate.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo();
                    }
#endif
                    ////                    if (updateCount == 0)
                    ////                        Trace.WriteLine("XXX");
                    OnEndUpdateRequest(new GridEndUpdateRequestEventArgs(update));
                    if (updateCount > 0)
                    {
                        if (--updateCount == 0)
                        {
                            updateCommand = string.Empty;
                        }
                    }

                    Trace.Unindent();
#if DEBUG
                    if (Switches.BeginEndUpdate.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo("Level " + this.updateCount.ToString());
                    }
#else
                    ;
#endif
                    ////inEndUpdate = false;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.BeginUpdateRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnBeginUpdateRequest(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnBeginUpdateRequest(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.UpdateInfo, this.UpdateOptions);
            }
#else

            ;
#endif
            if (BeginUpdateRequest != null)
            {
                BeginUpdateRequest(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.EndUpdateRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridEndUpdateRequestEventArgs" /> that contains the event data.</param>
        protected virtual void OnEndUpdateRequest(GridEndUpdateRequestEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnEndUpdateRequest(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (EndUpdateRequest != null)
            {
                EndUpdateRequest(this, e);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.BeginUpdate()"/> has been called.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Updating
        {
            [DebuggerStepThrough()]
            get
            {
                return updateCount > 0;
            }
        }

        internal string UpdateInfo
        {
            get
            {
                return updateCommand;
            }
        }

        [NonSerialized]
        int suspendChangeEvents = 0;

        /// <summary>
        /// Suspends raising change events.
        /// </summary>
        [DebuggerStepThrough()]
        public void SuspendChangeEvents()
        {
            suspendChangeEvents++;
        }

        /// <summary>
        /// Resumes raising change events.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResumeChangeEvents()
        {
            if (suspendChangeEvents > 0)
            {
                suspendChangeEvents--;
            }
        }

        /// <exclude/>
        [Obsolete("Use CanGridRaiseEvents instead. There was a naming conflict with Whidbeys Component.CanRaiseEvents property.")]
#if SyncfusionFramework2_0
        protected new bool CanRaiseEvents
#else
        protected bool CanRaiseEvents
#endif
        {
            get
            {
                return CanGridRaiseEvents;
            }
        }

        /// <summary>
        /// Gets a value indicating whether raising events has been suspended.
        /// </summary>
        protected virtual bool CanGridRaiseEvents
        {
            get
            {
                return suspendChangeEvents == 0;
            }
        }

        [NonSerialized]
        int suspendRecordUndo = 0;

        /// <summary>
        /// Suspend logging undo information.
        /// </summary>
        [DebuggerStepThrough()]
        public void SuspendRecordUndo()
        {
            suspendRecordUndo++;
        }

        /// <summary>
        /// Resume logging undo information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResumeRecordUndo()
        {
            if (suspendRecordUndo > 0)
            {
                suspendRecordUndo--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether undo information should be logged.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ShouldRecordUndo
        {
            [DebuggerStepThrough()]
            get
            {
                return !inInit && suspendRecordUndo == 0;
            }
        }

        #endregion
        #region ChangeLayoutCells

        //// It is still discussed if ChangingLayoutCells and ChangingLayoutCells events are needed.
        //// They may go away ... The Conditional flag ensures that they don't get compiled into the binaries.

        event GridChangeLayoutCellsEventHandler ChangingLayoutCells;
        event GridChangeLayoutCellsEventHandler ChangedLayoutCells;

        [Conditional("Changelayout")]
        internal void NotifyChangingLayoutCells(GridRangeInfo range)
        {
            OnChangingLayoutCells(new GridChangeLayoutCellsEventArgs(range));
        }

        [Conditional("Changelayout")]
        internal void OnChangingLayoutCells(GridChangeLayoutCellsEventArgs e)
        {
            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ChangingLayoutCells != null)
            {
                try
                {
                    ChangingLayoutCells(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }

        [Conditional("Changelayout")]
        internal void NotifyChangedLayoutCells(GridRangeInfo range)
        {
            if (!inInit)
            {
                OnChangedLayoutCells(new GridChangeLayoutCellsEventArgs(range));
            }
        }

        [Conditional("Changelayout")]
        internal void OnChangedLayoutCells(GridChangeLayoutCellsEventArgs e)
        {
            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ChangingLayoutCells != null)
            {
                try
                {
                    ChangedLayoutCells(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }
        #endregion
        #region ISupportInitialize

        /// <summary>
        /// Implements <see cref="ISupportInitialize.BeginInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void BeginInit()
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (inInit)
            {
                throw new System.Exception("BeginInit called twice.");
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);

            inInit = true;
        }

        /// <summary>
        /// Implements <see cref="ISupportInitialize.EndInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void EndInit()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            inInit = false;
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.BeginInit"/> was called.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initializing
        {
            [DebuggerStepThrough()]
            get
            {
                return this.inInit;
            }
        }
        #endregion

        /// <implement/>
        /// <summary>Creates the control.</summary>
        /// <returns>The control.</returns>
        public virtual Control CreateControl()
        {
            GridControlBase grid = new GridControlBase(this);
            grid.FillSplitterPane = true;
            return grid;
        }

        /// <summary>
        /// Force all views to be refreshed.
        /// </summary>
        public virtual void Refresh()
        {
            if (RefreshRequest != null)
            {
                RefreshRequest(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Force all views to invalidate a specified range of cells.
        /// </summary>
        /// <param name="range">The range of cells to be repainted.</param>
        /// <param name="options">Options that indicate if method should enlarge the affected range of cells to include covered and floating cells.</param>
        public virtual void InvalidateRange(GridRangeInfo range, GridRangeOptions options)
        {
            if (InvalidateRangeRequest != null)
            {
                InvalidateRangeRequest(this, new GridInvalidateRangeRequestEventArgs(range, options));
            }
        }

        /// <summary>
        /// Occurs when <see cref="InvalidateRange"/> is called for the <see cref="GridModel"/>.
        /// </summary>
        public event GridInvalidateRangeRequestEventHandler InvalidateRangeRequest;

        /// <summary>
        /// Raises the <see cref="InvalidateRangeRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridInvalidateRangeRequestEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidateRangeRequest(GridInvalidateRangeRequestEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnInvalidateRangeRequest(e);
            }

            if (InvalidateRangeRequest != null)
            {
                InvalidateRangeRequest(this, e);
            }
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfo"/> with all cells in the grid, excluding row and column headers.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRangeInfo GridCellsRange
        {
            get
            {
                if (RowCount >= Rows.HeaderCount + 1 && ColCount >= Cols.HeaderCount + 1)
                {
                    return GridRangeInfo.Cells(Rows.HeaderCount + 1, Cols.HeaderCount + 1, RowCount, ColCount);
                }
                else
                {
                    return GridRangeInfo.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfo"/> with all cells in the grid, excluding frozen rows and columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRangeInfo ScrollableGridRangeInfo
        {
            get
            {
                if (RowCount >= Rows.FrozenCount + 1 && ColCount >= Cols.FrozenCount + 1)
                {
                    return GridRangeInfo.Cells(Rows.FrozenCount + 1, Cols.FrozenCount + 1, RowCount, ColCount);
                }
                else
                {
                    return GridRangeInfo.Empty;
                }
            }
        }

        #region GridDC
        /// <summary>
        /// Raises the <see cref="GridModel.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// Override this event if you want to customize the graphics context before a grid control draws to it.
        /// </remarks>
        /// <example>The following example changes the <see cref="Graphics.TextRenderingHint"/> of a <see cref="Graphics"/>
        /// object to <see cref="System.Drawing.Text.TextRenderingHint.ClearTypeGridFit"/>:
        /// <code lang="C#">
        /// e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        /// </code>
        /// </example>
        protected virtual void OnPrepareGraphics(GraphicsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPrepareGraphics(e);
            }

            // TODO: Make a technical note for customers how to change e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (PrepareGraphics != null)
            {
                PrepareGraphics(this, e);
            }
        }

        /// <summary>
        /// Raise a <see cref="GridModel.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        public void DoPrepareGraphics(Graphics g)
        {
            OnPrepareGraphics(new GraphicsEventArgs(g));
        }

        /// <summary>
        /// Occurs before the grid draws to or uses a <see cref="Graphics"/> context.
        /// </summary>
        /// <remarks>
        /// Typical places where this event is raised are: <see cref="GridControlBase.OnPaint"/> of the <see cref="GridControlBase"/>
        /// and <see cref="GridModelRowColSizeIndexer.ResizeToFit(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/>.
        /// <para/>
        /// Raises a <see cref="GridModel.PrepareGraphics"/> event.
        /// </remarks>
        [Description("Occurs before the grid draws to or uses a Graphics context."),
        Category("Behavior")]
        public event GraphicsEventHandler PrepareGraphics;

        /// <summary>
        /// Returns a display <see cref="Graphics"/> object for this grid. You must dispose this object after use.
        /// The method will return null if <see cref="ActiveGridView"/> is null. In such case the <see cref="GetGraphicsProvider"/>
        /// method is recommended.
        /// </summary>
        /// <returns>A <see cref="Graphics"/> object.</returns>
        /// <remarks>
        /// Raises a <see cref="GridModel.PrepareGraphics"/> event.
        /// </remarks>
        /// <example>
        /// The following shows how to use this method.
        /// <code lang="C#">
        /// IGraphicsProvider graphicsProvider = this.GetGraphicsProvider();  // It is important to hold onto this object as long ad Graphics context is needed!
        /// Graphics g = graphicsProvider.Graphics;   // Do not dispose this object! It is a cached Display Device context.
        /// // If nested method calls do call GetGraphicsProvider and get a graphics context the
        /// // same cached graphics context can be returned.
        /// </code>
        /// <code lang="VB">
        /// Dim graphicsProvider as IGraphicsProvider = this.GetGraphicsProvider()  ' It is important to hold onto this object as long ad Graphics context is needed!
        /// Dim g as Graphics = graphicsProvider.Graphics   ' Do not dispose this object! It is a cached Display Device context.
        /// ' If nested method calls do call GetGraphicsProvider and get a graphics context the
        /// ' same cached graphics context can be returned.
        /// </code>
        /// </example>
        [Obsolete("It is recommended to use GetGraphicsProvider instead since this method will keep a Display Graphics context in memory if no active grid view is attached.")]
        public Graphics CreateGraphics()
        {
            if (savedGridModelGraphicsProvider == null)
            {
                if (this.ActiveGridView != null)
                {
                    return this.ActiveGridView.CreateGridGraphics();
                }

                savedGridModelGraphicsProvider = new GridModelGraphicsProvider(this);
            }

            return savedGridModelGraphicsProvider.Graphics;
        }

        GridModelGraphicsProvider savedGridModelGraphicsProvider = null;

        /// <summary>
        /// Returns a temporarily cached <see cref="IGraphicsProvider"/> object. The object
        /// creates a Graphics object on demand and raises a <see cref="PrepareGraphics"/> event. The graphics object
        /// gets automatically disposed once the IGraphicsProvider itself goes
        /// out of scope. Do not dispose this object yourself.
        /// </summary>
        /// <returns>Graphics provider.</returns>
        public IGraphicsProvider GetGraphicsProvider()
        {
            GridModelGraphicsProvider graphicsProvider = null;

            if (graphicsProviderRef != null)
            {
                graphicsProvider = graphicsProviderRef.Target as GridModelGraphicsProvider;
            }

            if (graphicsProvider == null)
            {
                graphicsProvider = new GridModelGraphicsProvider(this);
                graphicsProviderRef = new WeakReference(graphicsProvider);
            }

            return graphicsProvider;
        }

        [NonSerialized]
        WeakReference graphicsProviderRef = null;

        #endregion
        #region OperationFeedback

        /// <summary>
        /// Occurs when an operation takes a longer time and the user should be notified
        /// about its status and have a chance to abort.
        /// </summary>
        /// <remarks>
        /// See <see cref="OperationFeedbackEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs when an operation takes a longer time and the user should be notified about its status."),
        Category("Behavior")]
        public event OperationFeedbackEventHandler OperationFeedback;

        [NonSerialized]
        Stack feedbackStack = new Stack();
        void IOperationFeedbackProvider.RaiseOperationFeedbackEvent(OperationFeedbackEventArgs e)
        {
            if (OperationFeedback != null)
            {
                OperationFeedback(this, e);
            }
        }

        Stack IOperationFeedbackProvider.FeedbackStack
        {
            get { return feedbackStack; }
        }
        #endregion

        // Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="GridModel"/>.
        /// </summary>
        /// <param name="type">The Type value.</param>
        public GridModel(Type type)
        {
            CreateGridVolatileData();
            options.gridModel = this;
        }

        /// <overload>
        /// Initializes a new instance of <see cref="GridModel"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="GridModel"/>.
        /// </summary>
        public GridModel()
            : this(typeof(GridModel))
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether GridModel should use the newer <see cref="GridNewVolatileData"/>
        /// as the default volatile data store. This works only with .NET Framework 2.0 or higher
        /// and offers improved performance especially with virtual mode scenarios.
        /// </summary>
        public static bool AllowNewGridVolatileData
        {
            get { return allowNewGridVolatileData; }
            set { allowNewGridVolatileData = value; }
        }

        private void CreateGridVolatileData()
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (AllowNewGridVolatileData)
            {
                volatileData = gridVolatileData = new GridNewVolatileData(this);
            }
            else
            {
#endif
                volatileData = gridVolatileData = new GridVolatileData(this);
            }
        }

        ArrayList deserializeList = null;

        /// <summary>
        /// Initializes a new <see cref="GridModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModel(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            data = (GridData)info.GetValue("Data", typeof(GridData));
            rowHeights = (GridModelRowColSizeIndexer)info.GetValue("RowHeights", typeof(GridModelRowColSizeIndexer));
            colWidths = (GridModelRowColSizeIndexer)info.GetValue("ColWidths", typeof(GridModelRowColSizeIndexer));
            rows = (GridModelRowColOperations)info.GetValue("Rows", typeof(GridModelRowColOperations));
            cols = (GridModelRowColOperations)info.GetValue("Cols", typeof(GridModelRowColOperations));
            styleInfoMap = (GridBaseStylesMap)info.GetValue("BaseStylesMap", typeof(GridBaseStylesMap));
            selectedRanges = (GridRangeInfoList)info.GetValue("SelectedRanges", typeof(GridRangeInfoList));
            rowHidden = (GridModelHideRowColsIndexer)info.GetValue("HideRows", typeof(GridModelHideRowColsIndexer));
            colHidden = (GridModelHideRowColsIndexer)info.GetValue("HideCols", typeof(GridModelHideRowColsIndexer));
            coveredRangesObject = (GridModelCoveredRanges)info.GetValue("CoveredRanges", typeof(GridModelCoveredRanges));
            cellModels = (GridCellModelCollection)info.GetValue("CellModels", typeof(GridCellModelCollection));
            floatingCellsObject = (GridModelFloatingCells)info.GetValue("FloatingCells", typeof(GridModelFloatingCells));
            options = (GridModelOptions)info.GetValue("Options", typeof(GridModelOptions));
            options.gridModel = this;

            deserializeList = new ArrayList();
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Value is GridModelBound)
                {
                    deserializeList.Add(sie.Value);
                }

                if (sie.Name == "Properties")
                {
                    properties = (GridProperties)info.GetValue("Properties", typeof(GridProperties));
                }
                else if (sie.Name == "BanneredRanges")
                {
                    banneredRangesObject = (GridModelBanneredRanges)info.GetValue("BanneredRanges", typeof(GridModelBanneredRanges));
                }
            }

            CreateGridVolatileData();
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            IDeserializationCallback dc = Data as IDeserializationCallback;
            if (dc != null)
            {
                dc.OnDeserialization(sender);
            }

            foreach (GridModelBound mb in this.deserializeList)
            {
                mb.RaiseModelDeserialization(sender, this);
            }

            floatingCellsObject.SetFloatCellsMode(options.FloatCellsMode);
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModel"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the data needed to serialize the <see cref="GridModel"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", data); // GridData
            info.AddValue("RowHeights", rowHeights); // GridModelRowColSizeIndexer
            info.AddValue("ColWidths", colWidths); // GridModelRowColSizeIndexer
            info.AddValue("Rows", rows); // GridModelRowColOperations
            info.AddValue("Cols", cols); // GridModelRowColOperations
            info.AddValue("BaseStylesMap", styleInfoMap); // GridBaseStylesMap
            info.AddValue("SelectedRanges", selectedRanges); // GridRangeInfoList
            info.AddValue("HideRows", rowHidden); // GridModelHideRowColsIndexer
            info.AddValue("HideCols", colHidden); // GridModelHideRowColsIndexer

            info.AddValue("CoveredRanges", coveredRangesObject); // GridModelCoveredRanges
            info.AddValue("CellModels", cellModels); // GridCellModelCollection
            info.AddValue("FloatingCells", floatingCellsObject); // GridModelFloatingCells
            info.AddValue("Options", options); // GridModelOptions
            info.AddValue("Properties", properties); // GridProperties
            info.AddValue("BanneredRanges", banneredRangesObject); // GridModelBanneredRanges
        }

        /// <summary>
        /// Gets or sets the <see cref="GridBaseStylesMap"/> that is associated with this <see cref="GridModel"/>.
        /// </summary>
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                if (styleInfoMap == null)
                {
                    styleInfoMap = OnCreateBaseStylesMap();
                    OnBaseStylesMapChanged(EventArgs.Empty);
                }

                return styleInfoMap;
            }

            set
            {
                styleInfoMap = value;
                OnBaseStylesMapChanged(EventArgs.Empty);
            }
        }

        private bool inRichTextBoxEditMode =false;
        /// <summary>
        /// Gets or Sets whether the RichtextBox is in EditMode.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or Sets whether the RichtextBox is in EditMode.")]
        public bool InRichTextEditMode
        {
            get
            {
                return inRichTextBoxEditMode;
            }
            set
            {
                inRichTextBoxEditMode = value;
            }
        }
        int richTextStyleRow = 0;
        internal int RichTextStyleRow
        {
            get
            {
                return richTextStyleRow;
            }
            set
            {
                richTextStyleRow = value;
            }
        }
        int richTextStyleCol = 0;
        internal int RichTextStyleCol
        {
            get
            {
                return richTextStyleCol;
            }
            set
            {
                richTextStyleCol = value;
            }
        }
        private RichTextBox richTextControl = null;
        /// <summary>
        /// Used Inyternally.
        /// </summary>
        internal RichTextBox RichTextControl
        {
            get
            {
                return richTextControl;
            }
            set
            {
                richTextControl = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.BaseStylesMap"/> has been associated with this <see cref="GridModel"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBaseStylesMap
        {
            get
            {
                return styleInfoMap != null;
            }
        }

        /// <summary>
        /// This method is called the first time <see cref="GridModel.BaseStylesMap"/> and no
        /// <see cref="GridBaseStylesMap"/> has been associated with the <see cref="GridModel"/> before.
        /// </summary>
        /// <returns>A <see cref="GridBaseStylesMap"/> object.</returns>
        protected virtual GridBaseStylesMap OnCreateBaseStylesMap()
        {
            GridBaseStylesMap styleInfoMap = new GridBaseStylesMap();
            styleInfoMap.RegisterStandardStyles();
            return styleInfoMap;
        }

        string Name
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Raises the BaseStylesMapChanged event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data. </param>
        protected virtual void OnBaseStylesMapChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnBaseStylesMapChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, BaseStylesMap);
            }
#else

            ;
#endif
            try
            {
                if (BaseStylesMapChanged != null)
                {
                    BaseStylesMapChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseBaseStylesMapChanged(EventArgs e)
        {
            OnBaseStylesMapChanged(e);
        }

        /// <summary>
        /// Gets or sets a <see cref="GridData"/> object that saves cell contents and row and column headers for the <see cref="GridModel"/>.
        /// </summary>
        /// <remarks>
        /// A <see cref="GridModel.DataChanged"/> event is raised if you replace the <see cref="GridModel.Data"/>.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public GridData Data
        {
            get
            {
                if (data == null)
                {
                    data = OnCreateData();
                    OnDataChanged(EventArgs.Empty);
                }

                return data;
            }

            set
            {
                data = value;
                if (gridVolatileData != null)
                {
                    gridVolatileData.Clear();
                }

                OnDataChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.Data"/> has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasData
        {
            get
            {
                return data != null;
            }
        }

        /// <summary>
        /// This method is called the first time <see cref="GridModel.BaseStylesMap"/> and no
        /// <see cref="GridBaseStylesMap"/> has been associated with the <see cref="GridModel"/> before.
        /// </summary>
        /// <returns>A <see cref="GridBaseStylesMap"/> object.</returns>
        public virtual GridData OnCreateData()
        {
            return new GridData();
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DataChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnDataChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDataChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, Data);
            }
#else

            ;
#endif

            try
            {
                if (DataChanged != null)
                {
                    DataChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseDataChanged(EventArgs e)
        {
            OnDataChanged(e);
        }

        /// <summary>
        /// Recreates a <see cref="GridModel"/> object from a file that holds data in SOAP format.
        /// </summary>
        /// <param name="fileName">The full pathname of the file.</param>
        /// <returns>The recreated <see cref="GridModel"/> object.</returns>
        public static GridModel LoadSoap(string fileName)
        {
            GridModel t = null;
            Stream s = File.OpenRead(fileName);
            try
            {
                t = LoadSoap(s);
                t.fileName = fileName;
            }
            finally
            {
                s.Close();
            }

            return t;
        }

        /// <summary>
        /// Recreates a <see cref="GridModel"/> object from a <see cref="Stream"/> with data in SOAP format.
        /// </summary>
        /// <overload>
        /// Recreates a <see cref="GridModel"/> object from a <see cref="Stream"/> with data in SOAP format.
        /// </overload>
        /// <param name="s">A <see cref="Stream"/> with data in SOAP format.</param>
        /// <returns>The recreated <see cref="GridModel"/> object.</returns>
        public static GridModel LoadSoap(Stream s)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);

                return LoadSoapStream(s);
            }
            catch
            {
                // Fixes isses reported in incident 21508. In earlier versions the GridIndexDictionary used
                // by row heights and column widths and hidden rows did write out type information. BaseStylesMap
                // (GridBaseStyle class) also had a similar problem.
                s.Position = 0;
                TextReader rs = new StreamReader(s);
                string content = rs.ReadToEnd();
                content = System.Text.RegularExpressions.Regex.Replace(content, @", Version=.+\</AssemblyName\>", "</AssemblyName>");

                System.Text.UTF8Encoding Unicode = new System.Text.UTF8Encoding();
                byte[] bytes = Unicode.GetBytes(content);
                s = new MemoryStream(bytes);

                return LoadSoapStream(s);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        static GridModel LoadSoapStream(Stream s)
        {
            SoapFormatter b = new SoapFormatter();
            b.AssemblyFormat = FormatterAssemblyStyle.Simple;
            b.Binder = new Binder();
            GridModel t = b.Deserialize(s) as GridModel;
            if (t != null && t.BaseStylesMap != null && t.BaseStylesMap.Modified)
            {
                t.BaseStylesMap = (GridBaseStylesMap)t.BaseStylesMap.Clone(); // fixes a problem with BaseStylesMap when deserialized from template.
            }

            t.Modified = false;
            return t;
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a file in SOAP format. The filename can be specified with <see cref="GridModel.FileName"/>.
        /// </summary>
        public void SaveSoap()
        {
            SaveSoap(fileName);
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object in SOAP format to a file with the specified filename.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void SaveSoap(string fileName)
        {
            this.fileName = fileName;
            Stream s = File.CreateText(fileName).BaseStream;
            SaveSoap(s);
            s.Close();
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a stream in SOAP format.
        /// </summary>
        /// <param name="s">Stream object.</param>
        public void SaveSoap(Stream s)
        {
            SoapFormatter b = new SoapFormatter();
            b.AssemblyFormat = FormatterAssemblyStyle.Simple;
            b.Serialize(s, this);
            Modified = false;
        }

        /// <overload>
        /// Recreates a <see cref="GridModel"/> object from a file that holds data in binary format.
        /// </overload>
        /// <summary>
        /// Recreates a <see cref="GridModel"/> object from a file that holds data in binary format.
        /// </summary>
        /// <param name="fileName">The full pathname of the file.</param>
        /// <returns>The recreated <see cref="GridModel"/> object.</returns>
        public static GridModel LoadBinary(string fileName)
        {
            GridModel t = null;
            Stream s = File.OpenRead(fileName);
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                BinaryFormatter b = new BinaryFormatter();
                b.AssemblyFormat = FormatterAssemblyStyle.Simple;
                b.Binder = new Binder();
                object obj = b.Deserialize(s);
                t = obj as GridModel;
                if (t != null && t.BaseStylesMap != null && t.BaseStylesMap.Modified)
                {
                    t.BaseStylesMap = (GridBaseStylesMap)t.BaseStylesMap.Clone(); // fixes a problem with BaseStylesMap when deserialized from template.
                }

                t.Modified = false;
                t.fileName = fileName;
            }
            finally
            {
                s.Close();
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            return t;
        }

        /// <summary>
        /// Recreates a <see cref="GridModel"/> object from a <see cref="Stream"/> with data in binary format.
        /// </summary>
        /// <param name="s">A <see cref="Stream"/> with data in binary format.</param>
        /// <returns>The recreated <see cref="GridModel"/> object.</returns>
        public static GridModel LoadBinary(Stream s)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                BinaryFormatter b = new BinaryFormatter();
                b.AssemblyFormat = FormatterAssemblyStyle.Simple;
                b.Binder = new Binder();
                GridModel t = b.Deserialize(s) as GridModel;
                if (t != null && t.BaseStylesMap != null && t.BaseStylesMap.Modified)
                {
                    t.BaseStylesMap = (GridBaseStylesMap)t.BaseStylesMap.Clone(); // fixes a problem with BaseStylesMap when deserialized from template.
                }

                t.Modified = false;
                return t;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a file in binary format. The filename can be specified with <see cref="GridModel.FileName"/>.
        /// </summary>
        public void SaveBinary()
        {
            SaveBinary(fileName);
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object in binary format to a file with the specified filename.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void SaveBinary(string fileName)
        {
            this.fileName = fileName;
            Stream s = File.Create(fileName);
            SaveBinary(s);
            s.Close();
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a stream in binary format.
        /// </summary>
        /// <param name="s">Stream object.</param>
        public void SaveBinary(Stream s)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.AssemblyFormat = FormatterAssemblyStyle.Simple;
            b.Serialize(s, this);
            Modified = false;
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.FileName"/> was specified.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUntitled
        {
            get
            {
                return GridUtil.IsEmpty(fileName);
            }
        }

        internal bool shouldResetDesignTimeCollection = true;

        /// <summary>
        /// Gets or sets a value indicating whether the modified state of the grid. A <see cref="GridModel.ModifiedChanged"/> event is raised
        /// when this property is changed.
        /// </summary>
        public bool Modified
        {
            get
            {
                return modified;
            }

            set
            {
                if (value && shouldResetDesignTimeCollection)
                {
                    if (this.DesignMode)
                    {
                        TraceUtil.TraceCurrentMethodInfo(value);
                    }
                    ////this.rangeStyles = null;
                    ////                    this.rowHeightEntries = null;
                    ////                    this.colWidthEntries = null;
                    ////                    this.rowHiddenEntries = null;
                    ////                    this.colHiddenEntries = null;
                }

                if (modified != value)
                {
                    modified = value;
                    if (ModifiedChanged != null)
                    {
                        ModifiedChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        // GridName (e.g. TabName in work sheet)

        /// <summary>
        /// Gets or sets the filename for saving the <see cref="GridModel"/> next time you call <see cref="GridModel.SaveBinary()"/>.
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    if (FileNameChanged != null)
                    {
                        FileNameChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>Gets Selected Ranges. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual GridRangeInfoList SelectedRanges
        {
            get
            {
                if (selectedRanges == null)
                {
                    selectedRanges = new GridRangeInfoList();
                }

                return selectedRanges;
            }
        }

        /// <summary>
        /// Returns the column index for a column that can be identified by the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The column index; or -1 if column could not be resolved.</returns>
        /// <remarks>
        /// The default behavior is that the name is recognized if in format C# (e.g. C10).<para/>
        /// If a <see cref="DataProvider"/> is attached to the grid (e.g. a <see cref="GridModelDataBinder"/>
        /// when using a <see cref="GridDataBoundGrid"/>), the name can be the fieldname in the underlying table.<para/>
        /// The following collections allow you to pass in names as identifiers:
        /// <list type="bullet">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term><see cref="GridModelRowColSizeIndexer.this[int]"/> of the <see cref="GridModel.RowHeights"/> and <see cref="GridModel.ColWidths"/> collection.</term></item>
        /// <item><term><see cref="GridModelHideRowColsIndexer.this[int]"/> of <see cref="GridModel.HideRows"/> and <see cref="GridModel.HideCols"/> collection.</term></item>
        /// <item><term><see cref="GridModelRowStylesIndexer.this[int]"/> of <see cref="GridModel.RowStyles"/> and
        /// <see cref="GridModelColStylesIndexer.this[int]"/> of <see cref="GridModel.ColStyles"/> collection.</term></item>
        /// </list>
        /// </remarks>
        public virtual int NameToColIndex(string name)
        {
            int colIndex = -1;
            try
            {
                if (this.dataProvider == null)
                {
                    colIndex = GridRangeInfo.Parse(name).Left;
                }
                else
                {
                    colIndex = this.dataProvider.NameToColIndex(name);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            return colIndex;
        }

        /// <summary>
        /// Returns the row index for a row that can be identified by the specified name.
        /// </summary>
        /// <param name="name">The name of the row.</param>
        /// <returns>The row index; or -1 if row could not be resolved.</returns>
        /// <remarks>
        /// The default behavior is that the name is recognized if in format R# (e.g. R10).<para/>
        /// The following collections allow you to pass in names as identifiers:
        /// <list type="bullet">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term><see cref="GridModelRowColSizeIndexer.this[int]"/> of the <see cref="GridModel.RowHeights"/> and <see cref="GridModel.ColWidths"/> collection.</term></item>
        /// <item><term><see cref="GridModelHideRowColsIndexer.this[int]"/> of <see cref="GridModel.HideRows"/> and <see cref="GridModel.HideCols"/> collection.</term></item>
        /// <item><term><see cref="GridModelRowStylesIndexer.this[int]"/> of <see cref="GridModel.RowStyles"/> and
        /// <see cref="GridModelColStylesIndexer.this[int]"/> of <see cref="GridModel.ColStyles"/> collection.</term></item>
        /// </list>
        /// </remarks>
        public virtual int NameToRowIndex(string name)
        {
            int rowIndex = -1;
            try
            {
                if (this.dataProvider == null)
                {
                    if (name.Length > 0 && Char.IsDigit(name[0]))
                    {
                        rowIndex = int.Parse(name);
                    }
                    else
                    {
                        GridRangeInfo r = GridRangeInfo.Parse(name);
                        if (r != null)
                        {
                            rowIndex = r.Top;
                        }
                    }
                }
                else
                {
                    rowIndex = this.dataProvider.NameToRowIndex(name);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            return rowIndex;
        }

        /// <summary>
        /// Occurs after row heights for a specified range of rows have been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after row heights for a specified range of rows has changed."),
        Category("Behavior")]
        public event GridRowColSizeChangedEventHandler RowHeightsChanged;

        /// <summary>
        /// Occurs before row heights for a specified range of rows are changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before row heights for a specified range of rows are changed."),
        Category("Behavior")]
        public event GridRowColSizeChangingEventHandler RowHeightsChanging;

        /// <summary>
        /// Occurs after column widths for a specified range of columns have been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after column widths for a specified range of columns have been changed."),
        Category("Behavior")]
        public event GridRowColSizeChangedEventHandler ColWidthsChanged;

        /// <summary>
        /// Occurs before column widths for a specified range of columns are changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before column widths for a specified range of columns are changed."),
        Category("Behavior")]
        public event GridRowColSizeChangingEventHandler ColWidthsChanging;

        /// <summary>
        /// Occurs before the size of a column is returned from the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the size of a column is returned from the dictionary."),
        Category("Behavior")]
        public event GridRowColSizeEventHandler QueryColWidth;

        /// <summary>
        /// Occurs when the size of a row is retrieved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the size of a row is returned from the dictionary."),
        Category("Behavior")]
        public event GridRowColSizeEventHandler QueryRowHeight;

        /// <summary>
        /// Occurs when the total size of several rows is retrieved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the size of a row is returned from the dictionary."),
        Category("Behavior")]
        public event GridRowColSizeTotalEventHandler QueryRowHeightTotal;

        /// <summary>
        /// Occurs before the size of a column is stored in the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the size of a column is stored in the dictionary."),
        Category("Behavior")]
        public event GridRowColSizeEventHandler SaveColWidth;

        /// <summary>
        /// Occurs before the size of a row is stored in the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColSizeEventHandler"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the size of a row is stored in the dictionary."),
        Category("Behavior")]
        public event GridRowColSizeEventHandler SaveRowHeight;

        /// <summary>
        /// Raises the RowHeightsChanged event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangedEventArgs that contains the event data. </param>
        protected virtual void OnRowHeightsChanged(GridRowColSizeChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowHeightsChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (RowHeightsChanged != null)
                {
                    RowHeightsChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseRowHeightsChanged(GridRowColSizeChangedEventArgs e)
        {
            OnRowHeightsChanged(e);
        }

        /// <summary>
        /// Raises the RowHeightsChanging event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangingEventArgs that contains the event data. </param>
        protected virtual void OnRowHeightsChanging(GridRowColSizeChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowHeightsChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (RowHeightsChanging != null)
                {
                    RowHeightsChanging(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        internal void RaiseRowHeightsChanging(GridRowColSizeChangingEventArgs e)
        {
            OnRowHeightsChanging(e);
        }

        /// <summary>
        /// Raises the ColWidthsChanged event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangedEventArgs that contains the event data. </param>
        protected virtual void OnColWidthsChanged(GridRowColSizeChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColWidthsChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (ColWidthsChanged != null)
                {
                    ColWidthsChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseColWidthsChanged(GridRowColSizeChangedEventArgs e)
        {
            OnColWidthsChanged(e);
        }

        /// <summary>
        /// Raises the ColWidthsChanging event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangingEventArgs that contains the event data. </param>
        protected virtual void OnColWidthsChanging(GridRowColSizeChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColWidthsChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (ColWidthsChanging != null)
                {
                    ColWidthsChanging(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        internal void RaiseColWidthsChanging(GridRowColSizeChangingEventArgs e)
        {
            OnColWidthsChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveRowHeight" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveRowHeight(GridRowColSizeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveRowHeight(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (SaveRowHeight != null)
            {
                SaveRowHeight(this, e);
            }
        }

        internal void RaiseSaveRowHeight(GridRowColSizeEventArgs e)
        {
            OnSaveRowHeight(e);
        }

        /// <summary>
        /// Raise the QueryRowHeight event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryRowHeight(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (QueryRowHeight != null)
            {
                QueryRowHeight(this, e);
            }
        }

        internal void RaiseQueryRowHeight(GridRowColSizeEventArgs e)
        {
            OnQueryRowHeight(e);
        }

        /// <summary>
        /// Raise the QueryRowHeight event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeTotalEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRowHeightTotal(GridRowColSizeTotalEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryRowHeightTotal(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (QueryRowHeightTotal != null)
            {
                QueryRowHeightTotal(this, e);
            }
        }

        internal void RaiseQueryRowHeightTotal(GridRowColSizeTotalEventArgs e)
        {
            OnQueryRowHeightTotal(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveColWidth" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveColWidth(GridRowColSizeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveColWidth(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (SaveColWidth != null)
            {
                SaveColWidth(this, e);
            }
        }

        internal void RaiseSaveColWidth(GridRowColSizeEventArgs e)
        {
            OnSaveColWidth(e);
        }

        /// <summary>
        /// Raise the QueryColWidth event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryColWidth(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (QueryColWidth != null)
            {
                QueryColWidth(this, e);
            }
        }

        internal void RaiseQueryColWidth(GridRowColSizeEventArgs e)
        {
            OnQueryColWidth(e);
        }

        [NonSerialized]
        internal GridModelRowColSizeIndexer rowHeights = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetRowHeightsInt(GridModelRowColSizeIndexer rowHeights)
        {
            this.rowHeights = rowHeights;
        }

        /// <summary>
        /// Gets row heights for the grid.
        /// </summary>
        [Category("Behavior-CellDimensions")]
        [Description("Set columns widths interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(ColWidthUITypeEditor), typeof(UITypeEditor))]
        public GridModelRowColSizeIndexer RowHeights
        {
            get
            {
                if (rowHeights == null)
                {
                    rowHeights = new GridModelRowHeightsIndexer(this);
                }

                return rowHeights;
            }
        }

        internal GridRowHeightCollection rowHeightEntries = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridRowHeight"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="RowHeights"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Behavior-CellDimensions")]
        [Description("Set columns widths interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridRowHeightCollection RowHeightEntries
        {
            get
            {
                if (rowHeightEntries == null)
                {
                    rowHeightEntries = new GridRowHeightCollection(RowHeights);
                }

                return rowHeightEntries;
            }

            set
            {
                if (!Object.ReferenceEquals(this.rowHeightEntries, value))
                {
                    this.rowHeightEntries = value;
                    RowHeightEntries.WriteToDict(this.RowHeights);
                }
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="RowHeights"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeRowHeightEntries()
        {
            return RowHeights.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="RowHeights"/> collection.
        /// </summary>
        public void ResetRowHeightEntries()
        {
            GridRowColSizeDictionary rowColSizeDictionary = RowHeights.Dictionary as GridRowColSizeDictionary;
            if (rowColSizeDictionary != null)
            {
                GridIndexDictionary dict = rowColSizeDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                this.RowHeights.ResetModified();
                Model.rowHeightEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }

        internal GridModelRowColSizeIndexer colWidths = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetColWidthsInt(GridModelRowColSizeIndexer colWidths)
        {
            this.colWidths = colWidths;
        }

        /// <summary>
        /// Gets column widths for the grid.
        /// </summary>
        [Category("Behavior-CellDimensions")]
        [System.Xml.Serialization.XmlIgnore]
        [Description("Set columns widths interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(ColWidthUITypeEditor), typeof(UITypeEditor))]
        public GridModelRowColSizeIndexer ColWidths
        {
            get
            {
                if (colWidths == null)
                {
                    colWidths = new GridModelColWidthsIndexer(this);
                }

                return colWidths;
            }
        }

        internal GridColWidthCollection colWidthEntries = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridColWidth"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="ColWidths"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Behavior-CellDimensions")]
        [Description("Set columns widths interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridColWidthCollection ColWidthEntries
        {
            get
            {
                if (colWidthEntries == null)
                {
                    colWidthEntries = new GridColWidthCollection(ColWidths);
                }

                return colWidthEntries;
            }

            set
            {
                if (!Object.ReferenceEquals(this.colWidthEntries, value))
                {
                    this.colWidthEntries = value;
                    ColWidthEntries.WriteToDict(this.ColWidths);
                }
            }
        }

        /// <summary>
        /// Resets values in the <see cref="ColWidths"/> collection.
        /// </summary>
        public void ResetColWidthEntries()
        {
            GridRowColSizeDictionary rowColSizeDictionary = ColWidths.Dictionary as GridRowColSizeDictionary;
            if (rowColSizeDictionary != null)
            {
                GridIndexDictionary dict = rowColSizeDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                this.ColWidths.ResetModified();
                Model.colWidthEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="ColWidths"/> collection were modified.
        /// </summary>
        /// <returns>true if modified; false otherwise.</returns>
        public bool ShouldSerializeColWidthEntries()
        {
            return ColWidths.Modified;
        }

        internal GridRowHiddenCollection rowHiddenEntries = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridRowHidden"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="HideRows"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Behavior-Visibility")]
        [Description("Hide rows interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridRowHiddenCollection RowHiddenEntries
        {
            get
            {
                if (rowHiddenEntries == null)
                {
                    rowHiddenEntries = new GridRowHiddenCollection(HideRows);
                }

                return rowHiddenEntries;
            }

            set
            {
                if (!Object.ReferenceEquals(this.rowHiddenEntries, value))
                {
                    this.rowHiddenEntries = value;
                    RowHiddenEntries.WriteToDict(this.HideRows);
                }
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="HideRows"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeRowHiddenEntries()
        {
            return HideRows.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="HideRows"/> collection.
        /// </summary>
        public void ResetRowHiddenEntries()
        {
            GridRowColHideDictionary rowColHiddenDictionary = HideRows.Dictionary as GridRowColHideDictionary;
            if (rowColHiddenDictionary != null)
            {
                GridIndexDictionary dict = rowColHiddenDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                Model.rowHiddenEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }

        internal GridColHiddenCollection colHiddenEntries = null;

        /// <summary>
        /// Gets or sets a collection of <see cref="GridColHidden"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="HideCols"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Behavior-Visibility")]
        [Description("Hide columns interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridColHiddenCollection ColHiddenEntries
        {
            get
            {
                if (colHiddenEntries == null)
                {
                    colHiddenEntries = new GridColHiddenCollection(HideCols);
                }

                return colHiddenEntries;
            }

            set
            {
                if (!Object.ReferenceEquals(this.colHiddenEntries, value))
                {
                    this.colHiddenEntries = value;
                    ColHiddenEntries.WriteToDict(this.HideCols);
                }
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="HideCols"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeColHiddenEntries()
        {
            return HideCols.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="HideCols"/> collection.
        /// </summary>
        public void ResetColHiddenEntries()
        {
            GridRowColHideDictionary rowColHiddenDictionary = HideCols.Dictionary as GridRowColHideDictionary;
            if (rowColHiddenDictionary != null)
            {
                GridIndexDictionary dict = rowColHiddenDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                Model.colHiddenEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal.</param>
        /// <returns>The optimal size of the cell.</returns>
        public Size CalculatePreferredCellSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            GridCellModelBase cellModel = style.CellModel;
            Size size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, queryBounds);
#if DEBUG
            if (Switches.GridModel.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(size, rowIndex, colIndex, queryBounds, style);
            }
#else
            ;
#endif
            if (GetGridLineBorder().Style != GridBorderStyle.None)
            {
                size.Width = size.Width + 1;
            }

            return size;
        }

        internal GridBorder gridLineBorder;
        internal GridBorder fixedLineBorder;

        /// <summary>
        /// Creates a GridBorder object generated using Options.DefaultGridBorderStyle
        /// and Properties.GridLineColor
        /// </summary>
        /// <returns>Grid border.</returns>
        public virtual GridBorder GetGridLineBorder()
        {
            if (gridLineBorder == null)
            {
                gridLineBorder = new GridBorder(Options.DefaultGridBorderStyle, Properties.GridLineColor);
            }

            return gridLineBorder;
        }

        /// <summary>
        /// Creates a GridBorder object generated using Options.DefaultGridBorderStyle
        /// and Properties.FixedLinesColor
        /// </summary>
        /// <returns>Grid border.</returns>
        public virtual GridBorder GetFixedLineBorder()
        {
            if (fixedLineBorder == null)
            {
                fixedLineBorder = new GridBorder(Options.DefaultGridBorderStyle, Properties.FixedLinesColor, GridBorderWeight.Medium);
            }

            return fixedLineBorder;
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="queryBounds">Vertical or horizontal.</param>
        /// <returns>The optimal size of the cell.</returns>
        public Size CalculatePreferredCellSize(Graphics g, int rowIndex, int colIndex, GridQueryBounds queryBounds)
        {
            GridStyleInfo style = this[rowIndex, colIndex];
            return CalculatePreferredCellSize(g, rowIndex, colIndex, style, queryBounds);
        }

        /// <summary>
        /// Occurs after a range of columns was hidden.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHidingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of columns was hidden."),
        Category("Behavior")]
        public event GridRowColHiddenEventHandler ColsHidden;

        /// <summary>
        /// Occurs before a range of columns is hidden.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHidingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of columns is hidden."),
        Category("Behavior")]
        public event GridRowColHidingEventHandler ColsHiding;

        /// <summary>
        /// Occurs after a range of rows was hidden.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHidingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of rows was hidden."),
        Category("Behavior")]
        public event GridRowColHiddenEventHandler RowsHidden;

        /// <summary>
        /// Occurs before a range of rows is hidden.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHidingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of rows is hidden."),
        Category("Behavior")]
        public event GridRowColHidingEventHandler RowsHiding;

        /// <summary>
        /// Occurs before the hidden state of a column is returned from the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHideEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the hidden state of a column is returned from the dictionary."),
        Category("Behavior")]
        internal event GridRowColHideEventHandler QueryHideCol;

        /// <summary>
        /// Occurs before the hidden state of a column is stored in the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHideEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the hidden state of a column is stored in the dictionary."),
        Category("Behavior")]
        internal event GridRowColHideEventHandler SaveHideCol;

        /// <summary>
        /// Occurs before the hidden state of a row is returned from the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHideEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the hidden state of a row is returned from the dictionary."),
        Category("Behavior")]
        internal event GridRowColHideEventHandler QueryHideRow; ////not currently used - so marked as internal

        /// <summary>
        /// Occurs before the hidden state of a row is stored in the dictionary.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRowColHideEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the hidden state of a row is stored in the dictionary."),
        Category("Behavior")]
        internal event GridRowColHideEventHandler SaveHideRow; ////not currently used - so marked as internal

        /// <summary>
        /// Raises the <see cref="GridModel.RowsHidden" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridRowColHiddenEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsHidden(GridRowColHiddenEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsHidden(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (RowsHidden != null)
                {
                    RowsHidden(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseRowsHidden(GridRowColHiddenEventArgs e)
        {
            OnRowsHidden(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsHiding" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHidingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsHiding(GridRowColHidingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsHiding(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (RowsHiding != null)
                {
                    RowsHiding(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        internal void RaiseRowsHiding(GridRowColHidingEventArgs e)
        {
            if (GridControlBase.UseOldHiddenScrollLogic && e.From <= this.Rows.FrozenCount)
            {
                this.Rows.FrozenCount = Math.Max(0, e.From - 1);
            }

            OnRowsHiding(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsHidden" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHiddenEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsHidden(GridRowColHiddenEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsHidden(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (ColsHidden != null)
                {
                    ColsHidden(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        internal void RaiseColsHidden(GridRowColHiddenEventArgs e)
        {
            OnColsHidden(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsHiding" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHidingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsHiding(GridRowColHidingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsHiding(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (ColsHiding != null)
                {
                    ColsHiding(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        internal void RaiseColsHiding(GridRowColHidingEventArgs e)
        {
            if (GridControlBase.UseOldHiddenScrollLogic && e.From <= this.Cols.FrozenCount)
            {
                this.Cols.FrozenCount = Math.Max(0, e.From - 1);
            }

            OnColsHiding(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveHideRow" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveHideRow(GridRowColHideEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveHideRow(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (SaveHideRow != null)
            {
                SaveHideRow(this, e);
            }
        }

        internal void RaiseSaveHideRow(GridRowColHideEventArgs e)
        {
            OnSaveHideRow(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryHideRow" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryHideRow(GridRowColHideEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryHideRow(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (QueryHideRow != null)
            {
                QueryHideRow(this, e);
            }
        }

        internal void RaiseQueryHideRow(GridRowColHideEventArgs e)
        {
            OnQueryHideRow(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveHideCol" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveHideCol(GridRowColHideEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveHideCol(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (SaveHideCol != null)
            {
                SaveHideCol(this, e);
            }
        }

        internal void RaiseSaveHideCol(GridRowColHideEventArgs e)
        {
            OnSaveHideCol(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryHideCol" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryHideCol(GridRowColHideEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryHideCol(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (QueryHideCol != null)
            {
                QueryHideCol(this, e);
            }
        }

        internal void RaiseQueryHideCol(GridRowColHideEventArgs e)
        {
            OnQueryHideCol(e);
        }

        internal GridModelHideRowColsIndexer rowHidden = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetHideRowsInt(GridModelHideRowColsIndexer rowHidden)
        {
            this.rowHidden = rowHidden;
        }

        /// <summary>
        /// Gets hidden rows in the grid.
        /// </summary>
        [Category("Behavior-Visibility")]
        [Description("Hide rows interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        ////        [Editor(typeof(RowHideUITypeEditor), typeof(UITypeEditor))]
        public GridModelHideRowColsIndexer HideRows
        {
            get
            {
                if (rowHidden == null)
                {
                    rowHidden = new GridModelHideRowsIndexer(this);
                }

                return rowHidden;
            }
        }

        internal GridModelHideRowColsIndexer colHidden = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetHideColsInt(GridModelHideRowColsIndexer colHidden)
        {
            this.colHidden = colHidden;
        }

        /// <summary>
        /// Gets hidden columns in the grid.
        /// </summary>
        [Category("Behavior-Visibility")]
        [Description("Hide columns interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(RowHideUITypeEditor), typeof(UITypeEditor))]
        public GridModelHideRowColsIndexer HideCols
        {
            get
            {
                if (colHidden == null)
                {
                    colHidden = new GridModelHideColsIndexer(this);
                }

                return colHidden;
            }
        }

        [NonSerialized]
        GridModelCommandManager commandStack = null;

        /// <summary>
        /// Gets undo and redo in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelCommandManager CommandStack
        {
            get
            {
                if (commandStack == null)
                {
                    commandStack = new GridModelCommandManager(this);
                }

                return commandStack;
            }
        }

        /// <summary>
        /// Occurs before the default row height is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDefaultSizeChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the default row height is changed."),
        Category("Behavior")]
        public event GridDefaultSizeChangingEventHandler DefaultRowHeightChanging;

        /// <summary>
        /// Occurs after the default row height has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDefaultSizeChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the default row height has been changed."),
        Category("Behavior")]
        public event GridDefaultSizeChangedEventHandler DefaultRowHeightChanged;

        /// <summary>
        /// Occurs before the default column width is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDefaultSizeChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the default column width is changed."),
        Category("Behavior")]
        public event GridDefaultSizeChangingEventHandler DefaultColWidthChanging;

        /// <summary>
        /// Occurs after the default column width has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDefaultSizeChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the default column width has been changed."),
        Category("Behavior")]
        public event GridDefaultSizeChangedEventHandler DefaultColWidthChanged;

        /// <summary>
        /// Occurs after the header row count has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the header row count has been changed."),
        Category("Behavior")]
        public event GridCountChangedEventHandler HeaderRowCountChanged;

        /// <summary>
        /// Occurs before the header row count is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the header row count is changed."),
        Category("Behavior")]
        public event GridCountChangingEventHandler HeaderRowCountChanging;

        /// <summary>
        /// Occurs after the frozen row count has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the frozen row count has been changed."),
        Category("Behavior")]
        public event GridCountChangedEventHandler FrozenRowCountChanged;

        /// <summary>
        /// Occurs before the frozen row count is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the frozen row count is changed."),
        Category("Behavior")]
        public event GridCountChangingEventHandler FrozenRowCountChanging;

        /// <summary>
        /// Occurs after a range of rows is moved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeMovedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of rows is moved."),
        Category("Behavior")]
        public event GridRangeMovedEventHandler RowsMoved;

        /// <summary>
        /// Occurs before a range of rows is moved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeMovingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of rows is removed."),
        Category("Behavior")]
        public event GridRangeMovingEventHandler RowsMoving;

        /// <summary>
        /// Occurs after a range of rows has been removed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeRemovedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of rows has been removed."),
        Category("Behavior")]
        public event GridRangeRemovedEventHandler RowsRemoved;

        /// <summary>
        /// Occurs before a range of rows is removed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeRemovingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of rows is removed."),
        Category("Behavior")]
        public event GridRangeRemovingEventHandler RowsRemoving;

        /// <summary>
        /// Occurs before a range of rows is inserted.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeInsertingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of rows is inserted."),
        Category("Behavior")]
        public event GridRangeInsertingEventHandler RowsInserting;

        /// <summary>
        /// Occurs after a range of rows has been inserted.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeInsertedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of rows has been inserted."),
        Category("Behavior")]
        public event GridRangeInsertedEventHandler RowsInserted;

        /// <summary>
        /// Occurs after the header column count has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the header column count has been changed."),
        Category("Behavior")]
        public event GridCountChangedEventHandler HeaderColCountChanged;

        /// <summary>
        /// Occurs before the header column count is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the header column count is changed."),
        Category("Behavior")]
        public event GridCountChangingEventHandler HeaderColCountChanging;

        /// <summary>
        /// Occurs after the frozen column count has been changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after the frozen column count has been changed."),
        Category("Behavior")]
        public event GridCountChangedEventHandler FrozenColCountChanged;

        /// <summary>
        /// Occurs before the frozen column count is changed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCountChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before the frozen column count is changed."),
        Category("Behavior")]
        public event GridCountChangingEventHandler FrozenColCountChanging;

        /// <summary>
        /// Occurs after a range of columns is moved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeMovedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of columns is moved."),
        Category("Behavior")]
        public event GridRangeMovedEventHandler ColsMoved;

        /// <summary>
        /// Occurs before a range of columns is moved.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeMovingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of columns is removed."),
        Category("Behavior")]
        public event GridRangeMovingEventHandler ColsMoving;

        /// <summary>
        /// Occurs after a range of columns has been inserted.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeRemovedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of columns has been inserted."),
        Category("Behavior")]
        public event GridRangeRemovedEventHandler ColsRemoved;

        /// <summary>
        /// Occurs before a range of columns is removed.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeRemovingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of columns is removed."),
        Category("Behavior")]
        public event GridRangeRemovingEventHandler ColsRemoving;

        /// <summary>
        /// Occurs before a range of columns is inserted.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeInsertingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before a range of columns is inserted."),
        Category("Behavior")]
        public event GridRangeInsertingEventHandler ColsInserting;

        /// <summary>
        /// Occurs after a range of columns has been inserted.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridRangeInsertedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after a range of columns has been inserted."),
        Category("Behavior")]
        public event GridRangeInsertedEventHandler ColsInserted;

        /// <summary>
        /// Raises the <see cref="GridModel.DefaultRowHeightChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultRowHeightChanging(GridDefaultSizeChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDefaultRowHeightChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (DefaultRowHeightChanging != null)
            {
                DefaultRowHeightChanging(this, e);
            }
        }

        internal void RaiseDefaultRowHeightChanging(GridDefaultSizeChangingEventArgs e)
        {
            OnDefaultRowHeightChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DefaultRowHeightChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultRowHeightChanged(GridDefaultSizeChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDefaultRowHeightChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (DefaultRowHeightChanged != null)
            {
                DefaultRowHeightChanged(this, e);
            }
        }

        internal void RaiseDefaultRowHeightChanged(GridDefaultSizeChangedEventArgs e)
        {
            OnDefaultRowHeightChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.HeaderRowCountChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderRowCountChanged(GridCountChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHeaderRowCountChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (HeaderRowCountChanged != null)
            {
                HeaderRowCountChanged(this, e);
            }
        }

        internal void RaiseHeaderRowCountChanged(GridCountChangedEventArgs e)
        {
            OnHeaderRowCountChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.HeaderRowCountChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderRowCountChanging(GridCountChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHeaderRowCountChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (HeaderRowCountChanging != null)
            {
                HeaderRowCountChanging(this, e);
            }
        }

        internal void RaiseHeaderRowCountChanging(GridCountChangingEventArgs e)
        {
            OnHeaderRowCountChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.FrozenRowCountChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenRowCountChanged(GridCountChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnFrozenRowCountChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (FrozenRowCountChanged != null)
            {
                FrozenRowCountChanged(this, e);
            }
        }

        internal void RaiseFrozenRowCountChanged(GridCountChangedEventArgs e)
        {
            OnFrozenRowCountChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.FrozenRowCountChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenRowCountChanging(GridCountChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnFrozenRowCountChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (FrozenRowCountChanging != null)
            {
                FrozenRowCountChanging(this, e);
            }
        }

        internal void RaiseFrozenRowCountChanging(GridCountChangingEventArgs e)
        {
            OnFrozenRowCountChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsMoved" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsMoved(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (RowsMoved != null)
            {
                RowsMoved(this, e);
            }
        }

        internal void RaiseRowsMoved(GridRangeMovedEventArgs e)
        {
            OnRowsMoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsMoving" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsMoving(GridRangeMovingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsMoving(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (RowsMoving != null)
            {
                RowsMoving(this, e);
            }
        }

        internal void RaiseRowsMoving(GridRangeMovingEventArgs e)
        {
            OnRowsMoving(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsRemoved" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsRemoved(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (RowsRemoved != null)
            {
                RowsRemoved(this, e);
            }

            this.gridCells = null;
            this.rangeStyles = null;
        }

        internal void RaiseRowsRemoved(GridRangeRemovedEventArgs e)
        {
            OnRowsRemoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsRemoving" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsRemoving(GridRangeRemovingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsRemoving(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (RowsRemoving != null)
            {
                RowsRemoving(this, e);
            }
        }

        internal void RaiseRowsRemoving(GridRangeRemovingEventArgs e)
        {
            OnRowsRemoving(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsInserting" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsInserting(GridRangeInsertingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsInserting(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (RowsInserting != null)
            {
                RowsInserting(this, e);
            }
        }

        internal void RaiseRowsInserting(GridRangeInsertingEventArgs e)
        {
            OnRowsInserting(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.RowsInserted" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRowsInserted(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
               

            ;
#endif
            if (RowsInserted != null)
            {
                RowsInserted(this, e);
            }
        }

        internal void RaiseRowsInserted(GridRangeInsertedEventArgs e)
        {
            OnRowsInserted(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DefaultColWidthChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultColWidthChanging(GridDefaultSizeChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDefaultColWidthChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (DefaultColWidthChanging != null)
            {
                DefaultColWidthChanging(this, e);
            }
        }

        internal void RaiseDefaultColWidthChanging(GridDefaultSizeChangingEventArgs e)
        {
            OnDefaultColWidthChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DefaultColWidthChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultColWidthChanged(GridDefaultSizeChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDefaultColWidthChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (DefaultColWidthChanged != null)
            {
                DefaultColWidthChanged(this, e);
            }
        }

        internal void RaiseDefaultColWidthChanged(GridDefaultSizeChangedEventArgs e)
        {
            OnDefaultColWidthChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.HeaderColCountChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderColCountChanged(GridCountChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHeaderColCountChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif

            if (HeaderColCountChanged != null)
            {
                HeaderColCountChanged(this, e);
            }
        }

        internal void RaiseHeaderColCountChanged(GridCountChangedEventArgs e)
        {
            OnHeaderColCountChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.HeaderColCountChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderColCountChanging(GridCountChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHeaderColCountChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (HeaderColCountChanging != null)
            {
                HeaderColCountChanging(this, e);
            }
        }

        internal void RaiseHeaderColCountChanging(GridCountChangingEventArgs e)
        {
            OnHeaderColCountChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.FrozenColCountChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenColCountChanged(GridCountChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnFrozenColCountChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (FrozenColCountChanged != null)
            {
                FrozenColCountChanged(this, e);
            }
        }

        internal void RaiseFrozenColCountChanged(GridCountChangedEventArgs e)
        {
            OnFrozenColCountChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.FrozenColCountChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenColCountChanging(GridCountChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnFrozenColCountChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (FrozenColCountChanging != null)
            {
                FrozenColCountChanging(this, e);
            }
        }

        internal void RaiseFrozenColCountChanging(GridCountChangingEventArgs e)
        {
            OnFrozenColCountChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsMoved" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsMoved(GridRangeMovedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsMoved(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ColsMoved != null)
            {
                ColsMoved(this, e);
            }
        }

        internal void RaiseColsMoved(GridRangeMovedEventArgs e)
        {
            OnColsMoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsMoving" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsMoving(GridRangeMovingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsMoving(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ColsMoving != null)
            {
                ColsMoving(this, e);
            }
        }

        internal void RaiseColsMoving(GridRangeMovingEventArgs e)
        {
            OnColsMoving(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsRemoved" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsRemoved(GridRangeRemovedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsRemoved(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (ColsRemoved != null)
            {
                ColsRemoved(this, e);
            }

            this.gridCells = null;
            this.rangeStyles = null;
        }

        internal void RaiseColsRemoved(GridRangeRemovedEventArgs e)
        {
            OnColsRemoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsRemoving" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsRemoving(GridRangeRemovingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsRemoving(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ColsRemoving != null)
            {
                ColsRemoving(this, e);
            }
        }

        internal void RaiseColsRemoving(GridRangeRemovingEventArgs e)
        {
            OnColsRemoving(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsInserting" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsInserting(GridRangeInsertingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsInserting(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ColsInserting != null)
            {
                ColsInserting(this, e);
            }
        }

        internal void RaiseColsInserting(GridRangeInsertingEventArgs e)
        {
            OnColsInserting(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsInserted" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsInserted(GridRangeInsertedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnColsInserted(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (ColsInserted != null)
            {
                ColsInserted(this, e);
            }
        }

        internal void RaiseColsInserted(GridRangeInsertedEventArgs e)
        {
            OnColsInserted(e);
        }

        /// <summary>
        /// Gets a table that represents a range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <returns>A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</returns>
        /// <seealso cref="GridModel.SetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/>
        public GridStyleInfoStoreTable GetCells(GridRangeInfo range)
        {
            GridStyleInfoStoreTable data = null;

            if (range.IsTable)
            {
                data = new GridStyleInfoStoreTable(0, 0);
                data[-1, -1] = Data[-1, -1];
                ////throw new NotImplementedException("GetTableData");
            }
            else if (range.IsRows)
            {
                data = Rows.GetCells(range.Top, range.Bottom, true, false);
            }
            else if (range.IsCols)
            {
                data = Cols.GetCells(range.Left, range.Right, true, false);
            }
            else
            {
                using (OperationFeedback op = new OperationFeedback(this))
                {
                    //// TODO: op.Description
                    int rowOffset = range.Top;
                    int colOffset = range.Left;
                    int rowCount = range.Height;
                    int colCount = range.Width;
                    int total = rowCount * colCount;
                    data = new GridStyleInfoStoreTable(rowCount, colCount);
                    int count = 0;
                    if (total > 0)
                    {
                        for (int r = 0; r < rowCount; r++)
                        {
                            for (int c = 0; c < colCount; c++)
                            {
                                GridStyleInfo style = new GridStyleInfo();
                                this.GetCellInfo(r + rowOffset, c + colOffset, style);
                                data[r, c] = (GridStyleInfoStore)style.Store;
                                op.PercentComplete = (int)((++count) * 100 / total);
                                if (op.ShouldCancel)
                                {
                                    throw new GridUserCanceledException();
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }

        /// <overload>
        /// Changes the contents for a range of cells in one batch.
        /// </overload>
        /// <summary>
        /// Changes the contents for a range of cells in one batch.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="data">A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</param>
        /// <seealso cref="GridModel.GetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/>
        public void SetCells(GridRangeInfo range, GridStyleInfoStoreTable data)
        {
            SetCells(range, data, false, false);
        }

        /// <summary>
        /// Changes the contents for a range of cells in one batch.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="data">A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</param>
        /// <param name="dontRaiseSaveCellInfoEvent">If True, the method will not raise the <see cref="GridModel.SaveCellInfo"/> event.</param>
        /// <param name="copyReferenceOnly">If True, the method will try to assign the style object directly. This is only possible if the
        /// existing cell was empty before or if modifyType is <see cref="StyleModifyType.Copy"/>. Otherwise it will apply the object as
        /// specified in modifyType.</param>
        /// <seealso cref="GridModel.GetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/>
        public void SetCells(GridRangeInfo range, GridStyleInfoStoreTable data, bool dontRaiseSaveCellInfoEvent, bool copyReferenceOnly)
        {
            NotifyChangingLayoutCells(range);
            this.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "GridModel.SetCells");
            try
            {
                //// TODO: undo?
                if (range.IsTable)
                {
                    throw new NotImplementedException("WriteTableData");
                    //// WriteTableData(data);
                }
                else if (range.IsRows)
                {
                    if (data.RowCount != range.Height)
                    {
                        throw new InvalidOperationException(SR.GetString("ExceptionDataRangeDiffRowCount"));
                    }

                    Rows.SetCells(range.Top, data);
                }
                else if (range.IsCols)
                {
                    if (data.ColCount != range.Width)
                    {
                        throw new InvalidOperationException(SR.GetString("ExceptionDataRangeDiffColCount"));
                    }

                    Cols.SetCells(range.Left, data);
                }
                else
                {
                    if (data.RowCount != range.Height)
                    {
                        throw new InvalidOperationException(SR.GetString("ExceptionDataRangeDiffRowCount"));
                    }

                    if (data.ColCount != range.Width)
                    {
                        throw new InvalidOperationException(SR.GetString("ExceptionDataRangeDiffColCount"));
                    }

                    using (OperationFeedback op = new OperationFeedback(this))
                    {
                        int rowOffset = range.Top;
                        int colOffset = range.Left;
                        int rowCount = range.Height;
                        int colCount = range.Width;
                        int total = rowCount * colCount;
                        int count = 0;
                        if (total > 0)
                        {
                            for (int r = 0; r < rowCount; r++)
                            {
                                for (int c = 0; c < colCount; c++)
                                {
                                    int rowIndex = r + range.Top;
                                    int colIndex = c + range.Left;
                                    GridStyleInfoStore store = data[r, c];
                                    if (store != null)
                                    {
                                        GridStyleInfo style = new GridStyleInfo(store);
                                        SetCellInfo(rowIndex, colIndex, style, StyleModifyType.Copy, dontRaiseSaveCellInfoEvent, copyReferenceOnly);
                                    }
                                    else
                                    {
                                        //// TODO: option to skip null cells
                                        SetCellInfo(rowIndex, colIndex, null, StyleModifyType.Copy, dontRaiseSaveCellInfoEvent, copyReferenceOnly);
                                    }

                                    op.PercentComplete = (int)((++count) * 100 / total);
                                    if (op.ShouldCancel)
                                    {
                                        throw new GridUserCanceledException();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                this.ResetVolatileData();
                this.EndUpdate();
                NotifyChangedLayoutCells(range);
            }
        }

        private GridModelRowColOperations rows = null;

        /// <summary>
        /// Gets row operations for the grid. Allows you to insert, move, remove rows, and more.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelRowColOperations Rows
        {
            get
            {
                if (rows == null)
                {
                    rows = new GridModelRowOperations(this);
                }

                return rows;
            }
        }

        private GridModelRowColOperations cols = null;

        /// <summary>
        /// Gets column operations for the grid. Allows you to insert, move, remove columns, and more.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelRowColOperations Cols
        {
            get
            {
                if (cols == null)
                {
                    cols = new GridModelColOperations(this);
                }

                return cols;
            }
        }

        GridBaseStylesMap IGridVolatileDataContainer.BaseStylesMap
        {
            get { return this.BaseStylesMap; }
        }

        int IGridVolatileDataContainer.HeaderRowCount
        {
            get { return this.Rows.HeaderCount; }
        }

        int IGridVolatileDataContainer.HeaderColCount
        {
            get { return this.Cols.HeaderCount; }
        }

        void IGridVolatileDataContainer.ChangeCell(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridRangeInfo range = GridRangeInfo.Auto(rowIndex, colIndex);
            ChangeCells(range, new GridStyleInfo[] { style }, StyleModifyType.Changes);
        }

        void IGridVolatileDataContainer.GetCellInfo(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GetCellInfo(rowIndex, colIndex, style);
        }

        IGridModelDataProvider dataProvider;

        /// <summary>
        /// Gets or sets a one-stop place to subscribe to row count, column count, and QueryCellInfo events.
        /// </summary>
        /// <remarks>
        /// You should implement <see cref="IGridModelDataProvider"/> if you want to receive
        /// <see cref="GridModel.QueryRowCount"/>, <see cref="GridModel.QueryColCount"/>,
        /// <see cref="GridModel.QueryCellInfo"/>, and <see cref="GridModel.SaveCellInfo"/> events.
        /// <para/>
        /// The methods in this interface are called before the named events are raised and thus
        /// give you a chance to control the event's behavior before other subscribers can handle it.
        /// <para/>
        /// You should assign a reference of your object to <see cref="GridModel.DataProvider"/> in order
        /// to receive the method calls.
        /// </remarks>
        public IGridModelDataProvider DataProvider
        {
            get
            {
                return dataProvider;
            }

            set
            {
                if (dataProvider != value)
                {
                    dataProvider = value;
                    OnDataProviderChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="GridModel.DataProvider"/> property has changed.
        /// </summary>
        public event EventHandler DataProviderChanged;

        /// <summary>
        /// Raises the <see cref="DataProviderChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataProviderChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDataProviderChanged(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (DataProviderChanged != null)
            {
                DataProviderChanged(this, e);
            }
        }

        [NonSerialized]
        int inGetCellInfo = 0;

        [NonSerialized]
        static internal int warnOnRecursionLevel = 6;

        /// <summary>
        /// Gets or sets RecursionLevel. If you handle the <see cref="QueryCellInfo"/> event and access other cells of the grid,
        /// there is a danger that you start an infinite recursion when you access the same
        /// cell in the grid for which <see cref="OnQueryCellInfo"/> was called. <para/>
        /// <see cref="GetCellInfo"/> checks the recursion level. If the level is greater
        /// than <see cref="WarnOnRecursionLevel"/>, an exception is thrown. The default is 6.
        /// </summary>
        [Browsable(false)]
        public static int WarnOnRecursionLevel
        {
            get
            {
                return warnOnRecursionLevel;
            }

            set
            {
                warnOnRecursionLevel = value;
            }
        }

        /// <summary>
        /// GetCellInfo raises a <see cref="QueryCellInfo"/> event to fill style contents and
        /// gets style data (the <see cref="GridStyleInfoStore"/>) from GridData.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">The style object that will receive data.</param>
        /// <returns>Always true.</returns>
        /// <remarks>
        /// If <see cref="QueryCellInfo"/> did not set e.Handled, the method gets the
        /// style data (the <see cref="GridStyleInfoStore"/>) from GridData and applies its data to
        /// the style object. Existing data of the style parameter previously initialized
        /// with <see cref="QueryCellInfo"/> will be preserved.
        /// <para/>
        /// IGridVolatileDataContainer.GetCellInfo calls GetCellInfo to return style information
        /// for a cell to the GridVolatileData cache.
        /// </remarks>
        public bool GetCellInfo(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridQueryCellInfoEventArgs e = new GridQueryCellInfoEventArgs(rowIndex, colIndex, style);
            try
            {
                inGetCellInfo++;
                if (inGetCellInfo > warnOnRecursionLevel)
                {
                    throw new InvalidOperationException("Recursive call detected. When accessing grid cell objects from a QueryCellInfo event handler make sure you do not recursively access the same cell.");
                }

                if (dataProvider != null)
                {
                    OnDataProviderQueryCellInfo(e);
                    if (e.Handled)
                    {
                        return true;
                    }

                    dataProvider.QueryCellInfo(e);
                }

                OnQueryCellInfo(e);
            }
            finally
            {
                inGetCellInfo--;
            }

            if (e.Handled)
            {
                return true;
            }

            GridStyleInfoStore store;
            store = Data[rowIndex, colIndex];

            if (store != null)
            {
                style.Store.ModifyStyle(store, StyleModifyType.ApplyNew);
            }

            return true;
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveCellInfo" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSaveCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveCellInfo(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (SaveCellInfo != null)
                {
                    SaveCellInfo(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                throw;
            }
        }

        internal void RaiseSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            OnSaveCellInfo(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.DataProviderSaveCellInfo" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSaveCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnDataProviderSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDataProviderSaveCellInfo(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            try
            {
                if (DataProviderSaveCellInfo != null)
                {
                    DataProviderSaveCellInfo(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                throw;
            }
        }

        internal void RaiseDataProviderSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            OnDataProviderSaveCellInfo(e);
        }

        /// <summary>
        /// Raises the <see cref="PasteCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridPasteCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnPasteCellText(GridPasteCellTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPasteCellText(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (PasteCellText != null)
            {
                PasteCellText(this, e);
            }
        }

        internal void RaisePasteCellText(GridPasteCellTextEventArgs e)
        {
            OnPasteCellText(e);
        }

        /// <overload>
        /// Changes the cell contents at a specific row and column index.
        /// </overload>
        /// <summary>
        /// Changes the cell contents at a specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <remarks>
        /// <see cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/> is more low-level than using <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> or <see cref="GridModel.this[int,int]"/>.
        /// <para/>
        /// It provides a faster solution to change cell contents compared to changing cells
        /// with <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> or <see cref="GridModel.this[int,int]"/>.
        /// It will not repaint any cells and also not try to generate undo information. <para/>
        /// </remarks>
        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType)
        {
            return SetCellInfo(rowIndex, colIndex, style, modifyType, false, false);
        }

        /// <summary>
        /// Changes the cell contents at a specific row and column index and allows you to suppress raising <see cref="GridModel.SaveCellInfo"/> events
        /// and also avoid copying the style objects.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <param name="dontRaiseSaveCellInfoEvent">If True, the method will not raise the <see cref="GridModel.SaveCellInfo"/> event.</param>
        /// <param name="copyReferenceOnly">If True, the method will try to assign the style object directly. This is only possible if the
        /// existing cell was empty before or if modifyType is <see cref="StyleModifyType.Copy"/>. Otherwise it will apply the object as
        /// specified in modifyType.</param>
        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType, bool dontRaiseSaveCellInfoEvent, bool copyReferenceOnly)
        {
            if (!dontRaiseSaveCellInfoEvent)
            {
                if (style != null)
                {
                    GridStyleInfoIdentity identity = style.Identity as GridStyleInfoIdentity;

                    if (identity == null || identity.Data == null || !identity.OffLine)
                    {
                        // Avoid recursive calls when style is changed within OnSaveCellInfo.
                        // Otherwise, if identity is not offline, changing a property will
                        // result in OnStyleChanged firing again and a call to ChangeCells.
                        // GridStyleInfo.OnStyleChanged checks for offline flag and will not
                        // call SetCellInfo if the style is offline.
                        identity = new GridStyleInfoIdentity(this.volatileData, rowIndex, colIndex, true);
                        GridStyleInfoStore store = (GridStyleInfoStore)style.Store;
                        if (style.Identity == null)
                        {
                            style.Identity = identity;
                        }
                        else
                        {
                            style = new GridStyleInfo(identity, store);
                        }
                    }
                }

                GridSaveCellInfoEventArgs e = new GridSaveCellInfoEventArgs(rowIndex, colIndex, style, modifyType);
                if (dataProvider != null)
                {
                    OnDataProviderSaveCellInfo(e);
                    if (e.Handled)
                    {
                        return true;
                    }

                    dataProvider.SaveCellInfo(e);
                }

                OnSaveCellInfo(e);
                if (e.Handled)
                {
                    return true;
                }
            }

            bool canChange = DiscardReadOnly || !readOnly;
            if (HasData && canChange)
            {
                try
                {
                    int r = rowIndex;
                    int c = colIndex;

                    GridStyleInfoStore store = null;
                    if (modifyType == StyleModifyType.Remove || (style == null && modifyType == StyleModifyType.Copy))
                    {
                        if (!DiscardReadOnly)
                        {
                            store = Data[r, c];
                        }

                        if (DiscardReadOnly || store == null || 0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                        {
                            Data[r, c] = null;
                        }
                    }
                    else
                    {
                        if (style == null)
                        {
                            throw new ArgumentNullException("style");
                        }

                        if (modifyType != StyleModifyType.Copy)
                        {
                            store = Data[r, c];
                        }

                        if (store != null)
                        {
                            if (DiscardReadOnly || 0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                            {
                                if (AllowAdjustCellValue)
                                {
                                    if ((modifyType == StyleModifyType.Changes && style.Store.IsValueModified(GridStyleInfoStore.CellValueTypeProperty))
                                        || ((modifyType == StyleModifyType.Override
                                            || modifyType == StyleModifyType.Copy
                                            || modifyType == StyleModifyType.ApplyNew)
                                        && style.Store.IsValueModified(GridStyleInfoStore.CellValueTypeProperty)))
                                    {
                                        FixCellValueType(style, style.CellValueType);
                                    }
                                }

                                store.ModifyStyle(style.Store, modifyType);
                            }
                        }
                        else
                        {
                            GridStyleInfoStore store2 = (GridStyleInfoStore)style.Store;
                            if (r <= Data.RowCount && c <= Data.ColCount)
                            {
                                if (copyReferenceOnly || store2 == null)
                                {
                                    Data[r, c] = store2;
                                }
                                else
                                {
                                    Data[r, c] = (GridStyleInfoStore)store2.Clone();
                                }
                            }
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    throw;
                }
            }

            return false;
        }

        bool allowAdjustCellValue = true;

        /// <summary>
        /// Gets or sets a value indicating whether the the CellValue should be converted to a specified CellValueType at
        /// the time CellValueType is set if CellValue is assigned to a cell before the CellValueType.
        /// Default is true for 4.1 and later. 
        /// If this flag is not set the CellValueType will not be enforced on a previously set CellValue.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowAdjustCellValue
        {
            get { return allowAdjustCellValue; }
            set { allowAdjustCellValue = value; }
        }

        void FixCellValueType(GridStyleInfo style, Type cellValueType)
        {
            if (style.HasCellValueType && style.HasCellValue)
            {
                object cellValue = style.CellValue;

                if (cellValue != null)
                {
                    if (cellValue.GetType() != cellValueType)
                    {
                        if (cellValue is string)
                        {
                            style.ApplyText((string)cellValue);
                        }
                        else
                        {
                            style.CellValue = GridCellValueConvert.ChangeType(cellValue, (Type)cellValueType, style.GetCulture(true), true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the volatile data cache. Call this method if you want to refresh the grid and force grid cells that
        /// are visible at the moment to reload all their data from data source. Also, the row and column count will be
        /// requeried.
        /// </summary>
        public void ResetVolatileData()
        {
            lastSyncText = null;
            if (gridVolatileData != null)
            {
                gridVolatileData.Clear();
            }

            if (this.NotifyResetVolatileData != null)
            {
                this.NotifyResetVolatileData(this, EventArgs.Empty);
            }

            foreach (GridBaseStyle gbs in BaseStylesMap)
            {
                if (gbs.StyleInfo.HasFont)
                {
                    gbs.StyleInfo.Font.ResetGdipFont();
                }
            }

            CoveredRanges.ResetCache();
            BanneredRanges.ResetCache();
        }

        internal event EventHandler NotifyResetVolatileData;

        /// <summary>
        /// Gives you access to the style information of a cell.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change cell contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        ///             model[2, 2].Text = "Grid Demo";
        ///             model[2, 2].Font.Bold = true;
        ///             model[2, 2].Font.Size = 16;
        ///             model[2, 2].HorizontalAlignment = GridHorizontalAlignment.Center;
        ///             model[2, 2].VerticalAlignment = GridVerticalAlignment.Middle;
        ///             model[2, 2].CellType = "Static";
        ///             model[2, 2].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
        ///             model[2, 2].Interior = new BrushInfo(GradientStyle.PathEllipse, Color.FromArgb(100, 57, 73, 122), Color.FromArgb(237, 240, 247));
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set for the cell,
        /// the <see cref="GridStyleInfo"/> object that is returned by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        ///             GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
        ///             standard.TextColor = Color.FromArgb(0, 21, 84);
        ///                 Color color = model[1, 1].TextColor;
        ///                 // model[1, 1].TextColor will return Color.FromArgb(0, 21, 84));
        /// </code>
        /// </example>
        public GridStyleInfo this[int rowIndex, int colIndex]
        {
            ////[DebuggerStepThrough()]
            get
            {
                if(volatileData != null)
                     return volatileData[rowIndex, colIndex];
                return gridVolatileData[rowIndex,colIndex];
            }

            ////[DebuggerStepThrough()]
            set
            {
                Data[rowIndex, colIndex] = (GridStyleInfoStore)value.Store.Clone();
            }
        }

        private bool PropertyDescriptorIsARelation(PropertyDescriptor prop)
        {
            if (typeof(IList).IsAssignableFrom(prop.PropertyType))
            {
                return !typeof(Array).IsAssignableFrom(prop.PropertyType);
            }

            return false;
        }

        /// <summary>
        /// Gives you an easy way to copy data from any given datasource that implements the IList interface
        /// or is an Array to a specified range of cells in the grid.
        /// </summary>
        /// <param name="range">The destination range.</param>
        /// <param name="dataSource">A data source that implements the IList interface or is an Array.</param>
        /// <remarks>
        /// PopulateValues uses the low-level <see cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/> method to populate date from
        /// the given datasource. This makes it a much faster way to fill grid cells instead of using <see cref="GridModel.this[int,int]"/>
        /// or <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>. It will not generate undo information. <para/>
        /// The grid display will be refreshed after the method finishes.<para/>
        /// If you want to fill the grid with large amount of data and give the user a chance to abort the operation, you should
        /// assign a <see cref="DelayedStatusDialog"/> to the model's <see cref="GridModel.OperationFeedbackListener"/>.<para/>
        /// </remarks>
        /// <seealso cref="GridModel.PopulateHeaders"/>.
        public void PopulateValues(GridRangeInfo range, object dataSource)
        {
            OperationFeedback op = new OperationFeedback(this);
            op.Description = "PopulateValues";
            op.AllowCancel = true;
            op.AllowNestedProgress = false;
            op.AllowNestedFeedback = false;
            int total = 0;
            int progress = 0;
            try
            {
                BeginUpdate(BeginUpdateOptions.None);
                IListSource listSource = dataSource as IListSource;
                if (listSource != null)
                {
                    dataSource = listSource.GetList();
                }

                Array array = dataSource as Array;
                if (array != null && array.Rank > 2)
                {
                    throw new ArgumentException("Invalid Array - Rank greater 2 not allowed.", "dataSource");
                }
                else if (array != null && array.Rank == 2)
                {
                    int length = Math.Min(array.GetLength(0), range.Height);
                    int width = Math.Min(array.GetLength(1), range.Width);
                    total = length * width;
                    if (total > 0)
                    {
                        for (int n = 0; n < length; n++)
                        {
                            for (int k = 0; k < width; k++)
                            {
                                int rowIndex = range.Top + n;
                                int colIndex = range.Left + k;
                                object item = array.GetValue(n, k);
                                GridStyleInfo style = new GridStyleInfo();
                                ////style.CellValueType = pdc[n].PropertyType;
                                style.CellValue = item;
                                SetCellInfo(rowIndex, colIndex, style, StyleModifyType.Override, true, true);
                            }

                            progress += width;
                            op.PercentComplete = (int)(progress * 100 / total);
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }
                else
                {
                    IList list = dataSource as IList;
                    if (list != null)
                    {
                        int count = Math.Min(list.Count, range.Height);
                        if (count > 0 && list[0] != null)
                        {
                            ITypedList typedList = list as ITypedList;
                            if (typedList != null)
                            {
                                PropertyDescriptorCollection pdc = typedList.GetItemProperties(null);
                                for (int n = 0; n < count; n++)
                                {
                                    int rowIndex = range.Top + n;
                                    object item = list[n];
                                    // Later:
                                    //                                PropertyDescriptor pd = pdc[index];
                                    //                                if (pd.IsBrowsable)
                                    //                                    if (this.PropertyDescriptorIsARelation(pd))
                                    PopulateItem(GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, range.Right), pdc, item);
                                    op.PercentComplete = (int)(n * 100 / count);
                                    if (op.ShouldCancel)
                                    {
                                        throw new GridUserCanceledException();
                                    }
                                }
                            }
                            else
                            {
                                Type type = list[0].GetType();
                                if (type.IsPrimitive || type == typeof(string))
                                {
                                    for (int n = 0; n < count; n++)
                                    {
                                        int rowIndex = range.Top + n;
                                        PopulateItem(rowIndex, range.Left, list[n]);
                                        op.PercentComplete = (int)(n * 100 / count);
                                        if (op.ShouldCancel)
                                        {
                                            throw new GridUserCanceledException();
                                        }
                                    }
                                }
                                else
                                {
                                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
                                    for (int n = 0; n < count; n++)
                                    {
                                        int rowIndex = range.Top + n;
                                        PopulateItem(GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, range.Right), pdc, list[n]);
                                        op.PercentComplete = (int)(n * 100 / count);
                                        if (op.ShouldCancel)
                                        {
                                            throw new GridUserCanceledException();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Type type = dataSource.GetType();
                        if (type.IsPrimitive || type == typeof(string))
                        {
                            PopulateItem(range.Top, range.Left, dataSource);
                        }
                        else
                        {
                            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
                            PopulateItem(range, pdc, dataSource);
                        }
                    }
                }

                InvalidateRange(range, GridRangeOptions.None);
            }
            finally
            {
                ResetVolatileData();
                EndUpdate();
                op.Close();
            }
        }

        void PopulateItem(GridRangeInfo range, PropertyDescriptorCollection pdc, object item)
        {
            if (item != null)
            {
                int count = Math.Min(range.Width, pdc.Count);
                for (int n = 0; n < count; n++)
                {
                    GridStyleInfo style = new GridStyleInfo();
                    ////style.CellValueType = pdc[n].PropertyType;
                    style.CellValue = pdc[n].GetValue(item);
                    SetCellInfo(range.Top, range.Left + n, style, StyleModifyType.Override, true, true);
                }
            }
        }

        void PopulateItem(int rowIndex, int colIndex, object item)
        {
            if (item != null)
            {
                GridStyleInfo style = new GridStyleInfo();
                ////style.CellValueType = item.GetType();
                style.CellValue = item;
                SetCellInfo(rowIndex, colIndex, style, StyleModifyType.Override, true, true);
            }
        }

        /// <summary>
        /// Gives you an easy way to copy column headers / names from any given datasource that implements the IList interface
        /// or is an Array to a specified range of cells in the grid.
        /// </summary>
        /// <param name="range">The destination range.</param>
        /// <param name="dataSource">A data source that implements the IList interface or is an Array.</param>
        /// <seealso cref="GridModel.PopulateValues"/>.
        public void PopulateHeaders(GridRangeInfo range, object dataSource)
        {
            BeginUpdate(BeginUpdateOptions.None);
            IListSource listSource = dataSource as IListSource;
            if (listSource != null)
            {
                dataSource = listSource.GetList();
            }

            IList list = dataSource as IList;
            if (list != null)
            {
                int count = Math.Min(list.Count, range.Height);
                if (count > 0 && list[0] != null)
                {
                    ITypedList typedList = list as ITypedList;
                    if (typedList != null)
                    {
                        PropertyDescriptorCollection pdc = typedList.GetItemProperties(null);
                        _PopulateHeaders(range, pdc);
                    }
                    else
                    {
                        Type type = list[0].GetType();
                        if (type.IsPrimitive || type == typeof(string))
                        {
                            _PopulateHeaders(range.Top, range.Left, "Value");
                        }
                        else
                        {
                            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
                            _PopulateHeaders(range, pdc);
                        }
                    }
                }
            }
            else
            {
                Type type = dataSource.GetType();
                if (type.IsPrimitive || type == typeof(string))
                {
                    _PopulateHeaders(range.Top, range.Left, "Value");
                }
                else
                {
                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
                    _PopulateHeaders(range, pdc);
                }
            }

            ResetVolatileData();
            EndUpdate();
        }

        void _PopulateHeaders(GridRangeInfo range, PropertyDescriptorCollection pdc)
        {
            int count = Math.Min(range.Width, pdc.Count);
            for (int n = 0; n < count; n++)
            {
                GridStyleInfo style = new GridStyleInfo();
                ////style.CellValueType = pdc[n].PropertyType;
                style.CellValue = pdc[n].DisplayName;
                SetCellInfo(range.Top, range.Left + n, style, StyleModifyType.Override, true, true);
            }
        }

        void _PopulateHeaders(int rowIndex, int colIndex, string item)
        {
            if (item != null)
            {
                GridStyleInfo style = new GridStyleInfo();
                ////style.CellValueType = item.GetType();
                style.CellValue = item;
                SetCellInfo(rowIndex, colIndex, style, StyleModifyType.Override, true, true);
            }
        }

        [NonSerialized]
        private GridModelColStylesIndexer colStyles = null;

        /// <summary>
        /// Gets column styles. Individual cells will inherit attributes from the corresponding column style.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelColStylesIndexer ColStyles
        {
            [DebuggerStepThrough()]
            get
            {
                if (colStyles == null)
                {
                    colStyles = new GridModelColStylesIndexer(this);
                }

                return colStyles;
            }
        }

        [NonSerialized]
        private GridModelRowStylesIndexer rowStyles = null;

        /// <summary>
        /// Gets row styles. Individual cells will inherit attributes from the corresponding row style.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelRowStylesIndexer RowStyles
        {
            [DebuggerStepThrough()]
            get
            {
                if (rowStyles == null)
                {
                    rowStyles = new GridModelRowStylesIndexer(this);
                }

                return rowStyles;
            }
        }

        /// <summary>
        /// Gets or sets the table style. Individual cells will inherit attributes from the table style.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfo TableStyle
        {
            ////[DebuggerStepThrough()]
            get
            {
                return this[-1, -1];
            }

            ////[DebuggerStepThrough()]
            set
            {
                this[-1, -1] = value;
            }
        }

        /// <overload>
        /// Analyzes specified cells for attributes that are equal among the cells.
        /// </overload>
        /// <summary>
        /// Analyzes specified cells for attributes that are equal among the cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <returns>The <see cref="GridStyleInfo"/> object that holds all attributes that were equal among cells.</returns>
        /// <example>
        /// The following example determines if <see cref="GridFontInfo.Bold"/> is enabled for a range of cells:
        /// <code lang="C#">
        ///         GridStyleInfo style = GetCombinedStyle(GridRangeInfo.Cells(2, 2, 4, 8));
        ///         bool isBold = style.Font.Bold;
        /// </code>
        /// </example>
        public GridStyleInfo GetCombinedStyle(GridRangeInfo range)
        {
            GridRangeInfoList ranges = new GridRangeInfoList();
            ranges.Add(range);
            return GetCombinedStyle(ranges);
        }

        /// <summary>
        /// Analyzes specified cells for attributes that are equal among the cells.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> that specifies the ranges of cells.</param>
        /// <returns>The <see cref="GridStyleInfo"/> object that holds all attributes that were equal among cells.</returns>
        public GridStyleInfo GetCombinedStyle(GridRangeInfoList ranges)
        {
            GridStyleInfo style = null;
            GridModel model = this;
            foreach (GridRangeInfo range in ranges)
            {
                if (range.IsCells)
                {
                    GridRangeInfo intRange = range.ExpandRange(-1, -1, -1, -1);
                    //// model[-1, -1] will give me table style, [r, -1] row style etc.
                    for (int r = intRange.Top; r <= intRange.Bottom; r++)
                    {
                        for (int c = intRange.Left; c <= intRange.Right; c++)
                        {
                            if (style == null)
                            {
                                style = model[r, c].GetOffLineCopy();
                            }
                            else
                            {
                                style.MergeStyle(model[r, c]);
                            }
                        }
                    }
                }
                else if (range.IsTable)
                {
                    style = model.TableStyle.GetOffLineCopy();
                }
                else if (range.IsRows)
                {
                    for (int r = range.Top; r <= range.Bottom; r++)
                    {
                        if (style == null)
                        {
                            style = model.RowStyles[r].GetOffLineCopy();
                        }
                        else
                        {
                            style.MergeStyle(model.RowStyles[r]);
                        }
                    }
                }
                else if (range.IsCols)
                {
                    for (int c = range.Left; c <= range.Right; c++)
                    {
                        if (style == null)
                        {
                            style = model.ColStyles[c].GetOffLineCopy();
                        }
                        else
                        {
                            style.MergeStyle(model.ColStyles[c]);
                        }
                    }
                }
            }

            return style;
        }

        /// <summary>
        /// Records current selection state - current cell and selected ranges. Will be used for restoring selections when performing undo / redo,
        /// </summary>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="ranges">The current list of selected ranges.</param>
        public void ChangeSelectionState(int currentRow, int currentCol, GridRangeInfo[] ranges)
        {
            // called from CommandStack when doing an undo or redo
            this.Selections.Clear(true);
            if (activeGridView != null)
            {
                activeGridView.CurrentCell.MoveTo(currentRow, currentCol, GridSetCurrentCellOptions.NoSelectRange);
            }

            foreach (GridRangeInfo range in ranges)
            {
                if (range != null && !range.IsEmpty)
                {
                    this.Selections.Add(range);
                }
            }
        }

        /// <summary>
        /// Scrolls the specifies range into view of the active grid view.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> to be made visible.</param>
        /// <param name="reason">The reason for scrolling the current cell into view (e.g. KeyPress, GridFocus etc.)</param>
        /// <remarks>
        /// This will only affect the active grid view. See <see cref="GridModel.ActiveGridView"/>.
        /// </remarks>
        public void ScrollCellInView(GridRangeInfo range, GridScrollCurrentCellReason reason)
        {
            if (activeGridView != null)
            {
                activeGridView.ScrollCellInView(range, reason);
            }
        }

        /// <overload>
        /// Applies a text to the specified range of cells.
        /// </overload>
        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <remarks>
        /// <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> will reset volatile data cache,
        /// generate undo information, force recalculation of floating cells, and
        /// raise <see cref="GridModel.CellsChanging"/> and <see cref="GridModel.CellsChanged"/> method.
        /// <para/>
        /// When you change cells directly with an indexer, this results in a call to <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>
        /// with modifyType set to <see cref="StyleModifyType.Changes"/>.
        /// <para/>
        /// </remarks>
        /// <example>
        /// The following example assigns a previously create style with a bold font to a cell:
        /// <code lang="C#">
        ///             GridStyleInfo boldFontStyle = new GridStyleInfo();
        ///             boldFontStyle.TextColor =  Color.FromArgb(238, 122, 3);
        ///             boldFontStyle.Font = boldFont;
        ///             model[rowIndex, 1].Text = "Interior";
        ///             model.ChangeCells(GridRangeInfo.Cell(rowIndex, 1), boldFontStyle);
        ///             </code>
        /// </example>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
        {
            bool success = false;
            GridStyleInfo[] savedCellsInfo = null;    // will be filled with style setting
            //            NotifyChangingLayoutCells(range);
            this.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "GridModel.ChangedCells " + range.ToString());
            if (NotifyCellsChanging(new GridCellsChangingEventArgs(range, cellsInfo, modifyType)))
            {
                //// start op, generate undo info
                OperationFeedback op = new OperationFeedback(this);
                op.Name = "ChangeCells";
                op.Description = SR.GetString("DescriptionChangeCells", range);
                op.AllowCancel = CommandStack.IsRecording || !CommandStack.Enabled;
                op.AllowRollback = CommandStack.IsRecording;

                try
                {
                    //// bool bIsMouseAction = GetHitState() > 0;  // op has a AllowProgress setting
                    GridRangeInfo intRange = range.ExpandRange(-1, -1, -1, -1);
                    int dwSize = intRange.Width * intRange.Height;

                    if (CommandStack.ShouldGenerateUndoInfo)
                    {
                        try
                        {
                            savedCellsInfo = GetCellsInfo(intRange);
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            {
                                throw;
                            }

                            return false;
                        }
                    }

                    GridRangeInfo restoreRange = GridRangeInfo.Empty;
                    ResetVolatileData();
                    try
                    {
                        int cellIndex = 0;
                        int counter = 0;
                        for (int rowIndex = intRange.Top; rowIndex <= intRange.Bottom; rowIndex++)
                        {
                            for (int colIndex = intRange.Left; colIndex <= intRange.Right; colIndex++)
                            {
                                if (cellsInfo.Length > 1)
                                {
                                    cellIndex = ((rowIndex - intRange.Top) * intRange.Width) + (colIndex - intRange.Left);
                                }

                                if (cellIndex >= cellsInfo.Length)
                                {
                                    break;
                                }

                                if (gridVolatileData != null)
                                {
                                    if (colIndex >= 0 && rowIndex >= 0)
                                    {
                                        gridVolatileData.ResetItem(new GridCellPos(rowIndex, colIndex));
                                    }
                                }

                                GridStyleInfo cellInfo = cellsInfo[cellIndex];
                                if (cellInfo != null)
                                {
                                    success |= SetCellInfo(rowIndex, colIndex, cellInfo, modifyType);
                                }
                                else
                                {
                                    success |= SetCellInfo(rowIndex, colIndex, null, StyleModifyType.Remove);
                                }

                                ////if (slowdown)
                                ////    System.Threading.Thread.Sleep(100);

                                op.PercentComplete = (int)((++counter) * 100 / dwSize);
                                if (op.ShouldCancel)
                                {
                                    if (rowIndex == range.Top)
                                    {
                                        restoreRange = GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, colIndex);
                                    }
                                    else
                                    {
                                        restoreRange = GridRangeInfo.Cells(range.Top, range.Left, rowIndex, range.Right);
                                    }

                                    throw new GridUserCanceledException();
                                }
                            }
                        }
                    }
                    catch (GridUserCanceledException ucex)
                    {
                        TraceUtil.TraceExceptionCatched(ucex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ucex))
                        {
                            throw;
                        }

                        if (success && savedCellsInfo != null)
                        {
                            if (CommandStack.IsRecording)
                            {
                                CommandStack.Mode = GridCommandMode.Rollback;
                                ChangeCells(restoreRange, savedCellsInfo, StyleModifyType.Copy);
                                CommandStack.Mode = GridCommandMode.Recording;
                                savedCellsInfo = null;
                            }

                            success = false;
                        }
                    }

                    ////                    if (CommandList.ShouldRecordCommandInfo)
                    ////                        CommandList.Add(new GridChangeCellsCommands(this, range, cellsInfo, modifyType));

                    if (CommandStack.ShouldGenerateUndoInfo && savedCellsInfo != null)
                    {
                        CommandStack.Push(new GridChangeCellsCommand(this, range, savedCellsInfo, StyleModifyType.Copy));
                    }

                    if (success)
                    {
                        Modified = true;
                        this.gridCells = null;
                        this.rangeStyles = null;
                    }
                }
                finally
                {
                    FloatingCells.DelayFloatCells(range);
                    MergeCells.DelayMergeCells(range);
                    OnCellsChanged(new GridCellsChangedEventArgs(range, savedCellsInfo, success));
                    op.Close();
                    this.EndUpdate();
                    NotifyChangedLayoutCells(range);
                }
            }

            return success;
        }

        /// <summary>
        /// Applies a text to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="textValue">The text to be saved in cells.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, string textValue)
        {
            GridStyleInfo cellInfo = new GridStyleInfo();
            cellInfo.Text = textValue;
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo, StyleModifyType modifyType)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, modifyType);
        }

        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo)
        {
            return ChangeCells(range, cellsInfo, StyleModifyType.Changes);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.CellsChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellsChanging(GridCellsChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellsChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (CellsChanging != null)
            {
                CellsChanging(this, e);
            }
        }

        internal void RaiseCellsChanging(GridCellsChangingEventArgs e)
        {
            OnCellsChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.CellsChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangingEventArgs" /> that contains the event data.</param>
        /// <returns>True if the operation is successful.</returns>
        public bool NotifyCellsChanging(GridCellsChangingEventArgs e)
        {
            try
            {
                OnCellsChanging(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }

            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridModel.CellsChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangedEventArgs" /> that contains the event data.</param>
        public void NotifyCellsChanged(GridCellsChangedEventArgs e)
        {
            try
            {
                OnCellsChanged(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.CellsChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellsChanged(GridCellsChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellsChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            try
            {
                if (CellsChanged != null)
                {
                    CellsChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <overload>
        /// Returns the cell contents for a range of cells and stores them in an array.
        /// </overload>
        /// <summary>
        /// Returns the cell contents for a range of cells and stores them in an array.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <returns>An array of <see cref="GridStyleInfo"/> object with cell information.</returns>
        public GridStyleInfo[] GetCellsInfo(GridRangeInfo range)
        {
            return GetCellsInfo(range, null);
        }

        /// <summary>
        /// Returns the cell contents for a range of cells and stores them in an array.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="op">A <see cref="OperationFeedback"/> to provide feedback during time-consuming operations or to abort an operation.</param>
        /// <returns>An array of <see cref="OperationFeedback"/> that allows progress feedback. May be NULL.</returns>
        public GridStyleInfo[] GetCellsInfo(GridRangeInfo range, OperationFeedback op)
        {
            try
            {
                int size = range.Width * range.Height;
                GridStyleInfo[] cellsInfo = new GridStyleInfo[size];
                for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                {
                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        GridStyleInfo styleInfo = new GridStyleInfo();
                        int dwIndex = ((rowIndex - range.Top) * range.Width) + (colIndex - range.Left);
                        if (GetCellInfo(rowIndex, colIndex, styleInfo))
                        {
                            cellsInfo[dwIndex] = styleInfo;
                        }

                        if (op != null && op.ShouldCancel)
                        {
                            throw new GridUserCanceledException();
                        }
                    }
                }

                return cellsInfo;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return null;
            }
        }

        /// <summary>
        /// Checks if a range of cells is selected or if the grid has a current cell which contents can be cleared.
        /// </summary>
        /// <returns>True if clearing cells with <see cref="GridModel.Clear(bool)"/> is possible; False otherwise.</returns>
        /// <remarks>
        /// Use this to enable a "Clear" menu item or gray it out.
        /// </remarks>
        public bool CanClearSelection()
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            return SelectedRanges.Count > 0 || HasCurrentCellInfo;
        }

        /// <summary>
        /// Clears all ranges in the current user selection.
        /// </summary>
        /// <param name="clearStyle">True if you want to clear all cell information; False if you only want to clear text.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        public bool Clear(bool clearStyle)
        {
            bool sucess = false;
            GridRangeInfoList rangeList;
            if (Selections.GetSelectedRanges(out rangeList, true))
            {
                sucess = ClearCells(rangeList, clearStyle);
            }

            return sucess;
        }

        /// <overload>
        /// Clears a specified range of cells.
        /// </overload>
        /// <summary>
        /// Clears a specified range of cells.
        /// </summary>
        /// <param name="range">The GridRangeInfo</param>
        /// <param name="clearStyle">True if you want to clear all cell information; False if you only want to clear text</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        public bool ClearCells(GridRangeInfo range, bool clearStyle /*=true*/)
        {
            GridRangeInfoList rangeList = new GridRangeInfoList();
            rangeList.Add(range);
            return ClearCells(rangeList, clearStyle);
        }

        /// <summary>
        /// Clears specified ranges of cells.
        /// </summary>
        /// <param name="rangeList">A <see cref="GridRangeInfoList"/> that holds cells to be cleared.</param>
        /// <param name="clearStyle">True if you want to clear all cell information; False if you only want to clear text.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        public bool ClearCells(GridRangeInfoList rangeList, bool clearStyle)
        {
            GridClearingCellsEventArgs e = new GridClearingCellsEventArgs(rangeList, clearStyle, false);
            Model.OnClearingCells(e);
            rangeList = e.RangeList;
            clearStyle = e.ClearStyle;
            if (e.Handled)
            {
                return e.Result;
            }

            bool canceled = false;

            int firstRow = 0;
            int firstCol = 0;
            int rowCount = RowCount;
            int colCount = ColCount;

            firstRow = Rows.HeaderCount + 1;
            firstCol = Cols.HeaderCount + 1;

            BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "GridModel.ClearCells");
            CommandStack.BeginTrans(SR.GetString("GRID_IDM_CLEARDATA"));
            OperationFeedback op = new OperationFeedback(this);
            op.Description = SR.GetString("GRID_IDM_CLEARDATA");
            op.AllowRollback = true;
            op.AllowNestedProgress = false;
            op.AllowCancel = true;

            try
            {
                foreach (GridRangeInfo range in rangeList)
                {
                    // SetStyleRange possibly throws an User Exception.
                    if (clearStyle)
                    {
                        op.AllowNestedProgress = true;

                        // Remove all style information.
                        canceled = !ChangeCells(range, GridStyleInfo.Empty, StyleModifyType.Remove);

                        if (canceled || op.ShouldCancel)
                        {
                            throw new GridUserCanceledException();
                        }

                        if (!range.IsCells)
                        {
                            // Clear both cell styles in range and also the row or column base styles.
                            GridRangeInfo expandedRange = range.ExpandRange(Rows.HeaderCount, Cols.HeaderCount, RowCount, ColCount);

                            if (range.IsTable)
                            {
                                canceled |= !ChangeCells(GridRangeInfo.Rows(Rows.HeaderCount, RowCount), GridStyleInfo.Empty, StyleModifyType.Remove);
                                if (!canceled)
                                {
                                    canceled |= !ChangeCells(GridRangeInfo.Cols(Cols.HeaderCount, ColCount), GridStyleInfo.Empty, StyleModifyType.Remove);
                                }
                            }

                            if (!canceled)
                            {
                                canceled = !ChangeCells(expandedRange, GridStyleInfo.Empty, StyleModifyType.Remove);
                            }

                            if (canceled || op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }

                        foreach (GridRangeInfo rc in BanneredRanges.Ranges.GetRangesContained(range))
                        {
                            BanneredRanges.Remove(rc);
                        }

                        foreach (GridRangeInfo rc in CoveredRanges.Ranges.GetRangesContained(range))
                        {
                            CoveredRanges.Remove(rc);
                        }
                    }
                    else
                    {
                        GridRangeInfo expandedRange = range.ExpandRange(Rows.HeaderCount, Cols.HeaderCount, RowCount, ColCount);

                        int dwSize = expandedRange.Width * expandedRange.Height;
                        int counter = 0;

                        // Only the value.
                        for (int rowIndex = expandedRange.Top; !canceled && rowIndex <= expandedRange.Bottom; rowIndex++)
                        {
                            for (int colIndex = expandedRange.Left; !canceled && colIndex <= expandedRange.Right; colIndex++)
                            {
                                op.PercentComplete = (int)((++counter) * 100 / dwSize);

                                GridStyleInfo style = this[rowIndex, colIndex];

                                if ((Model.IsReadOnly || style.ReadOnly) && !Model.IgnoreReadOnly)
                                {
                                    continue;
                                }

                                // Give the control the chance to validate.
                                // and change the pasted text
                                if (style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.ClearCells).Length > 0)
                                {
                                    canceled = !style.ApplyFormattedText(string.Empty, GridCellBaseTextInfo.ClearCells);
                                }

                                if (canceled || op.ShouldCancel)
                                {
                                    throw new GridUserCanceledException();
                                }
                            }
                        }
                    }
                }

                CommandStack.CommitTrans();
                return true;
            }
            catch (GridUserCanceledException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                if (op.RollbackConfirmed)
                {
                    CommandStack.Rollback();
                }
                else
                {
                    CommandStack.CommitTrans();
                }

                return false;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                CommandStack.Rollback();

                return false;
            }
            finally
            {
                op.Close();
                EndUpdate();
            }
        }

        [NonSerialized]
        GridCurrentCellInfo currentCellInfo = null;

        /// <summary>
        /// Gets or sets information about position of current cell, current cell renderer, and last active grid control.
        /// </summary>
        public virtual GridCurrentCellInfo CurrentCellInfo
        {
            get
            {
                return currentCellInfo;
            }

            set
            {
                currentCellInfo = value;
            }
        }

        /// <summary>
        /// Resets information about position of current cell, current cell renderer, and last active grid control.
        /// </summary>
        public void ResetCurrentCellInfo()
        {
            currentCellInfo = null;
        }

        /// <summary>
        /// Gets a value indicating whether information about current cell is available.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrentCellInfo
        {
            get
            {
                return currentCellInfo != null;
            }
        }

        /// <summary>
        /// Confirm pending changes in current cell.
        /// </summary>
        public void ConfirmChanges()
        {
            if (HasCurrentCellInfo)
            {
                CurrentCellInfo.GridView.CurrentCell.ConfirmChanges();
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ConfirmingPendingChanges" /> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// Set <see cref="CancelEventArgs.Cancel"/> to True if you can't confirm pending changes and want
        /// to abort the current operation.<para/>
        /// Some operations cannot be aborted, however.
        /// </remarks>
        public virtual void OnConfirmingPendingChanges(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnConfirmingPendingChanges(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            try
            {
                if (ConfirmingPendingChanges != null)
                {
                    ConfirmingPendingChanges(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        /// <summary>
        /// Confirms any pending changes, raises the <see cref="GridModel.ConfirmingPendingChanges" /> event,
        /// and calls the current cells <see cref="GridCurrentCell.ConfirmChanges()"/> method.
        /// </summary>
        /// <returns>True if this action is successfully completed; False otherwise.</returns>
        public bool ConfirmPendingChanges()
        {
            CancelEventArgs e = new CancelEventArgs();
            OnConfirmingPendingChanges(e);
            ConfirmChanges();
            return !e.Cancel;
        }

        /// <summary>
        /// Calls <see cref="GridCurrentCell.EndEdit"/> for the current cell.
        /// </summary>
        public void EndEdit()
        {
            if (HasCurrentCellInfo)
            {
                CurrentCellInfo.GridView.CurrentCell.EndEdit();
            }
        }

        /// <summary>
        /// Gets the <see cref="GridCellRendererBase"/> for the current cell.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellRendererBase CurrentCellRenderer
        {
            get
            {
                GridCurrentCellInfo cci = CurrentCellInfo;
                if (cci != null)
                {
                    return cci.CellView;
                }

                return null;
            }
        }

        [NonSerialized]
        GridModelCutPaste cutPaste;

        /// <summary>
        /// Gets clipboard operations for the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelCutPaste CutPaste
        {
            get
            {
                if (cutPaste == null)
                {
                    cutPaste = new GridModelCutPaste(this);
                    OnCreatedCutPaste();
                }

                return cutPaste;
            }
        }

        /// <summary>
        /// Called after the <see cref="GridModelCutPaste"/> object was created.
        /// </summary>
        protected virtual void OnCreatedCutPaste()
        {
        }

        [NonSerialized]
        GridModelStyleDataExchange dataExchange;

        /// <summary>
        /// Gets style data exchange for the grid. Lets you copy style information to a stream or clipboard and recreate the
        /// styles at a later time.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelStyleDataExchange DataExchange
        {
            get
            {
                if (dataExchange == null)
                {
                    dataExchange = new GridModelStyleDataExchange(this);
                }

                return dataExchange;
            }
        }

        [NonSerialized]
        GridModelTextDataExchange textDataExchange;

        /// <summary>
        /// Gets text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
        /// cell text at a later time.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelTextDataExchange TextDataExchange
        {
            get
            {
                if (textDataExchange == null)
                {
                    textDataExchange = new GridModelTextDataExchange(this);
                }

                return textDataExchange;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryColCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryColCount(GridRowColCountEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryColCount(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (QueryColCount != null)
            {
                QueryColCount(this, e);
            }
        }

        /// <summary>
        /// Triggers a call to the <see cref="OnQueryColCount"/> method which raises <see cref="QueryColCount"/> event
        /// </summary>
        /// <param name="e">Event data.</param>
        public void RaiseQueryColCount(GridRowColCountEventArgs e)
        {
            OnQueryColCount(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.QueryRowCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRowCount(GridRowColCountEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryRowCount(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (QueryRowCount != null)
            {
                QueryRowCount(this, e);
            }
        }

        /// <summary>
        /// Triggers a call to the <see cref="OnQueryRowCount"/> method which raises <see cref="QueryRowCount"/> event
        /// </summary>
        /// <param name="e">Event data.</param>
        public void RaiseQueryRowCount(GridRowColCountEventArgs e)
        {
            OnQueryRowCount(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveColCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveColCount(GridRowColCountEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveColCount(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (SaveColCount != null)
            {
                SaveColCount(this, e);
            }
        }

        internal void RaiseSaveColCount(GridRowColCountEventArgs e)
        {
            OnSaveColCount(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SaveRowCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveRowCount(GridRowColCountEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSaveRowCount(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else
            ;
#endif
            if (SaveRowCount != null)
            {
                SaveRowCount(this, e);
            }
        }

        internal void RaiseSaveRowCount(GridRowColCountEventArgs e)
        {
            OnSaveRowCount(e);
        }

        /// <summary>
        /// Gets or sets the number of rows in the grid. You can provide this value at run-time if you handle the <see cref="GridModel.QueryRowCount"/>
        /// or <see cref="GridModel.SaveRowCount"/> events.
        /// </summary>
        public int RowCount
        {
            get
            {
                if (gridVolatileData != null)
                {
                    if (!this.gridVolatileData.HasRowCount)
                    {
                        GridRowColCountEventArgs e = new GridRowColCountEventArgs();
                        if (dataProvider != null)
                        {
                            dataProvider.QueryRowCount(e);
                        }

                        OnQueryRowCount(e);
                        if (e.Handled)
                        {
                            gridVolatileData.RowCount = e.Count;
                        }
                        else
                        {
                            gridVolatileData.RowCount = Data.RowCount;
                        }
                    }

                    return gridVolatileData.RowCount;
                }
                else
                {
                    GridRowColCountEventArgs e = new GridRowColCountEventArgs();
                    if (dataProvider != null)
                    {
                        dataProvider.QueryRowCount(e);
                    }

                    OnQueryRowCount(e);
                    if (!e.Handled)
                    {
                        return Data.RowCount;
                    }

                    return e.Count;
                }
            }

            set
            {
                GridRowColCountEventArgs e = new GridRowColCountEventArgs(value);
                OnSaveRowCount(e);
                if (!e.Handled)
                {
                    int savedRowCount = this.Initializing ? Data.RowCount : RowCount;
                    if (value < savedRowCount)
                    {
                        Rows.RemoveRange(value + 1, savedRowCount);
                    }
                    else if (value > savedRowCount)
                    {
                        Rows.InsertRange(savedRowCount + 1, value - savedRowCount);
                    }
                }

                if (gridVolatileData != null)
                {
                    gridVolatileData.RowCount = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of columns in the grid.
        /// You can provide this value at run-time if you handle the <see cref="GridModel.QueryColCount"/>
        /// or <see cref="GridModel.SaveColCount"/> events.
        /// </summary>
        public int ColCount
        {
            get
            {
                if (gridVolatileData != null)
                {
                    if (!this.gridVolatileData.HasColCount)
                    {
                        GridRowColCountEventArgs e = new GridRowColCountEventArgs();
                        if (dataProvider != null)
                        {
                            dataProvider.QueryColCount(e);
                        }

                        OnQueryColCount(e);
                        if (e.Handled)
                        {
                            gridVolatileData.ColCount = e.Count;
                        }
                        else
                        {
                            gridVolatileData.ColCount = Data.ColCount;
                        }
                    }

                    return gridVolatileData.ColCount;
                }
                else
                {
                    GridRowColCountEventArgs e = new GridRowColCountEventArgs();
                    if (dataProvider != null)
                    {
                        dataProvider.QueryColCount(e);
                    }

                    OnQueryColCount(e);
                    if (!e.Handled)
                    {
                        return Data.ColCount;
                    }

                    return e.Count;
                }
            }

            set
            {
                GridRowColCountEventArgs e = new GridRowColCountEventArgs(value);
                OnSaveColCount(e);
                if (!e.Handled)
                {
                    int savedColCount = this.Initializing ? Data.ColCount : ColCount;
                    if (value < savedColCount)
                    {
                        Cols.RemoveRange(value + 1, savedColCount);
                    }
                    else if (value > savedColCount)
                    {
                        Cols.InsertRange(savedColCount + 1, value - savedColCount);
                    }
                }

                if (gridVolatileData != null)
                {
                    gridVolatileData.ColCount = value;
                }
            }
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="nLastRow">The Last row.</param>
        /// <param name="nLastCol">The Last column.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryMaximumRowCol(int nLastRow, int nLastCol)
        {
        }

        [NonSerialized]
        private bool discardReadOnly = false;
        private bool readOnly = false;

        /// <summary>
        /// Gets or sets a value indicating whether to DiscardReadOnly. Use IgnoreReadOnly instead.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool DiscardReadOnly
        {
            get { return discardReadOnly || IsDesign; }
            set { discardReadOnly = value; }
        }

        bool IsDesign
        {
            get
            {
                if (this.DesignMode)
                {
                    return true;
                }
                else if (this.ActiveGridView != null)
                {
                    ISite site = ((IComponent)ActiveGridView).Site;
                    if (site != null)
                    {
                        return site.DesignMode;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Occurs when <see cref="GridModel.ReadOnly"/> has changed.
        /// </summary>
        [Description("Occurs when Read-only mode has changed."),
        Category("Behavior")]
        public event EventHandler ReadOnlyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the grid is in Read-only state.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }

            set
            {
                if (value != readOnly)
                {
                    readOnly = value;
                    if (ReadOnlyChanged != null)
                    {
                        ReadOnlyChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        bool browseOnly = false;
        /// <summary>
        /// Gets or sets a value indicating whether the grid is in Browse-only state.
        /// To check the BrowseOnly state of Grid internally
        /// </summary>
        internal bool BrowseOnly
        {
            get
            {
                return browseOnly;
            }

            set
            {
                if (value != browseOnly)
                {
                    browseOnly = value;
                    readOnly = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsReadOnly. Internal only. just for compatibility...
        /// </summary>
        internal bool IsReadOnly
        {
            get
            {
                return ReadOnly;
            }

            set
            {
                ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to make changes to Read-only cells. Set this True if you want to be able make changes to Read-only cells.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IgnoreReadOnly
        {
            get
            {
                return DiscardReadOnly;
            }

            set
            {
                discardReadOnly = value;
            }
        }

        private GridModelSelections selections = null;

        /// <summary>
        /// Gets range selections in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelSelections Selections
        {
            get
            {
                if (selections == null)
                {
                    selections = new GridModelSelections(this);
                }

                return selections;
            }
        }

        /// <summary>
        /// Occurs before the model updates internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
        /// the selected range of cells.
        /// <para/>
        /// You can disallow the selection of specific cells at run-time when
        /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can also modify the <see cref="GridSelectionChangingEventArgs.Range"/> to include additional cells.
        /// </remarks>
        /// <seealso cref="GridSelectionChangingEventHandler"/>
        /// <seealso cref="GridSelectionChangedEventArgs"/>
        [Description("Occurs before internal data structures are updated with new selection state from a SelectRange command."),
        Category("Behavior")]
        public event GridSelectionChangingEventHandler SelectionChanging;

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event after
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outline
        /// the selected range of cells.
        /// </remarks>
        [Description("Occurs after internal data structures were updated with new selection state from a SelectRange command."),
        Category("Behavior")]
        public event GridSelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs before the grid model will clear its list of selected ranges when the user selects a new
        /// range of cells or when <see cref="GridModelSelections.Clear()"/> was called.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a <see cref="GridModel.PrepareClearSelection"/> event before
        /// it updates its internal data structures. A <see cref="GridControlBase"/> grid listens to this event and
        /// repaints the selected range of cells.
        /// </remarks>
        [Description("Occurs before the grid will clear its list of selected ranges."),
        Category("Behavior")]
        public event EventHandler PrepareClearSelection;

        /// <summary>
        /// Occurs before the model changes the current selection.
        /// </summary>
        /// <remarks>
        /// This event is raised by the model
        /// to notify all associated views that there has been a change to the current selection
        /// in the grid and all associated views should redraw affected display contents. The change can
        /// be originated by a mouse or keyboard input or programmatically.
        /// <para/>
        /// See <see cref="GridPrepareChangeSelectionEventArgs"/> for further discussion about this event.
        /// </remarks>
        [Description("Occurs before the grid will change the active selection range."),
        Category("Behavior")]
        public event GridPrepareChangeSelectionEventHandler PrepareChangeSelection;

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (SelectionChanging != null)
            {
                SelectionChanging(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (e.Range != null && e.Range.IsCells)
            {
                e.Range = Model.CoveredRanges.Ranges.GetOuterRange(e.Range);
            }

            try
            {
                OnSelectionChanging(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSelectionChanged(GridSelectionChangedEventArgs e)
        {
            // fix some painting gltich with ExcelLikeSelectionFrame when starting selection with frozen rows or cols.
            if (!e.Range.IsEmpty && Model.Options.ExcelLikeSelectionFrame && (e.Range.Top <= Model.Rows.FrozenCount || e.Range.Left <= Model.Cols.FrozenCount))
            {
                this.Model.InvalidateRange(e.Range, GridRangeOptions.None);
            }

            OnSelectionChanged(e);
            selectionStateChanged = true;
        }

        /// <summary>
        /// Raises the <see cref="GridModel.PrepareClearSelection" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareClearSelection(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPrepareClearSelection(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (PrepareClearSelection != null)
            {
                PrepareClearSelection(this, e);
            }
        }

        internal void RaisePrepareClearSelection(EventArgs e)
        {
            OnPrepareClearSelection(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.PrepareChangeSelection" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridPrepareChangeSelectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareChangeSelection(GridPrepareChangeSelectionEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPrepareChangeSelection(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (PrepareChangeSelection != null)
            {
                PrepareChangeSelection(this, e);
            }
        }

        internal void RaisePrepareChangeSelection(GridPrepareChangeSelectionEventArgs e)
        {
            OnPrepareChangeSelection(e);
        }

        private GridModelBanneredRanges banneredRangesObject = null;

        /// <summary>
        /// Gets bannered ranges in the grid.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridModelBanneredRanges BanneredRanges
        {
            get
            {
                if (banneredRangesObject == null)
                {
                    banneredRangesObject = new GridModelBanneredRanges(this);
                }

                return banneredRangesObject;
            }
        }

        /// <summary>
        /// Determines if bannered ranges have been added.
        /// </summary>
        /// <returns>returns boolean value to determines if bannered ranges have been added</returns>
        bool ShouldSerializeBanneredRanges()
        {
            return BanneredRanges.Count > 0;
        }

        /// <summary>
        /// Clears all bannered ranges.
        /// </summary>
        public void ResetBanneredRanges()
        {
            if (BanneredRanges.Count > 0)
            {
                BanneredRanges.Clear();
            }
        }

        private GridModelCoveredRanges coveredRangesObject = null;

        /// <summary>
        /// Gets covered ranges in the grid.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridModelCoveredRanges CoveredRanges
        {
            get
            {
                if (coveredRangesObject == null)
                {
                    coveredRangesObject = new GridModelCoveredRanges(this);
                }

                return coveredRangesObject;
            }
        }

        /// <summary>
        /// Determines if covered ranges have been added.
        /// </summary>
        /// <returns>returns boolean value to determines if covered ranges have been added.</returns>
        bool ShouldSerializeCoveredRanges()
        {
            return CoveredRanges.Count > 0;
        }

        /// <summary>
        /// Clears all covered ranges.
        /// </summary>
        public void ResetCoveredRanges()
        {
            if (CoveredRanges.Count > 0)
            {
                CoveredRanges.Clear();
            }
        }

        /// <summary>
        /// Determines spanned cell information for a given row and column. Spanned cells can be covered cells or floated cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if a spanned range existed at the specified cell.</returns>
        public virtual bool GetSpannedRangeInfo(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            if (CoveredRanges.Find(rowIndex, colIndex, out range))
            {
                return true;
            }

            if (MergeCells.Find(GridMergeCellDirection.Both, rowIndex, colIndex, out range))
            {
                return true;
            }

            if (FloatingCells.Find(rowIndex, colIndex, out range))
            {
                return true;
            }

            return false;
        }

        private bool enableGridListControlInComboBox = true;

        /// <summary>
        /// Gets or sets whether the grid's combobox controls should contain GridListControl. The default value is false.
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Show combobox with GridListControl."),
        DefaultValue(true)]
        public bool EnableGridListControlInComboBox
        {
            get { return enableGridListControlInComboBox; }
            set
            {
                enableGridListControlInComboBox = value;
            }
        }

        private bool enableLegacyStyle = true;
        /// <summary>
        /// Gets / sets the Legacy styles
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Legacy styles."),
        DefaultValue(true)]
        public virtual bool EnableLegacyStyle
        {
            get
            {
                return enableLegacyStyle;
            }
            set
            {
                if (enableLegacyStyle != value)
                {
                    enableLegacyStyle = value;

                    if (!value && GridStyleInfo.Default != null)
                    {
                        GridStyleInfo.Default.VerticalAlignment = GridVerticalAlignment.Middle;
                    }
                   
                    if (this.CellModels.ContainsKey("ComboBox"))
                    {
                        if (this.EnableGridListControlInComboBox && !this.EnableLegacyStyle)
                        {
                            this.CellModels["ComboBox"] = new GridDropDownGridListControlCellModel(this, true);
                        }
                        else
                        {
                            this.CellModels["ComboBox"] = new GridComboBoxCellModel(this);
                        }
                    }
                    this.Options.OnOptionsChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Occurs before covering is applied or reset for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCoveredRangesChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before covering is applied or reset for a range of cells."),
        Category("Behavior")]
        public event GridCoveredRangesChangingEventHandler CoveredRangesChanging;

        /// <summary>
        /// Occurs after covering was applied or reset for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCoveredRangesChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after covering was applied or reset for a range of cells."),
        Category("Behavior")]
        public event GridCoveredRangesChangedEventHandler CoveredRangesChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.CoveredRangesChanging" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCoveredRangesChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCoveredRangesChanging(GridCoveredRangesChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCoveredRangesChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (CoveredRangesChanging != null)
            {
                CoveredRangesChanging(this, e);
            }
        }

        internal void RaiseCoveredRangesChanging(GridCoveredRangesChangingEventArgs e)
        {
            OnCoveredRangesChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.CoveredRangesChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCoveredRangesChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCoveredRangesChanged(GridCoveredRangesChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCoveredRangesChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (CoveredRangesChanged != null)
            {
                CoveredRangesChanged(this, e);
            }
        }

        internal void RaiseCoveredRangesChanged(GridCoveredRangesChangedEventArgs e)
        {
            OnCoveredRangesChanged(e);
        }

        /// <summary>
        /// Occurs before bannered range is applied or reset for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridBanneredRangesChangingEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs before bannering is applied or reset for a range of cells."),
        Category("Behavior")]
        public event GridBanneredRangesChangingEventHandler BanneredRangesChanging;

        /// <summary>
        /// Occurs after bannered range was applied or reset for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridBanneredRangesChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after bannering was applied or reset for a range of cells."),
        Category("Behavior")]
        public event GridBanneredRangesChangedEventHandler BanneredRangesChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.BanneredRangesChanging" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridBanneredRangesChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnBanneredRangesChanging(GridBanneredRangesChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnBanneredRangesChanging(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (BanneredRangesChanging != null)
            {
                BanneredRangesChanging(this, e);
            }
        }

        internal void RaiseBanneredRangesChanging(GridBanneredRangesChangingEventArgs e)
        {
            OnBanneredRangesChanging(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.BanneredRangesChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridBanneredRangesChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnBanneredRangesChanged(GridBanneredRangesChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnBanneredRangesChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (BanneredRangesChanged != null)
            {
                BanneredRangesChanged(this, e);
            }
        }

        internal void RaiseBanneredRangesChanged(GridBanneredRangesChangedEventArgs e)
        {
            OnBanneredRangesChanged(e);
        }

        [NonSerialized]
        private GridModelFloatingCells floatingCellsObject = null;

        /// <summary>
        /// Gets floating cells in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelFloatingCells FloatingCells
        {
            get
            {
                if (floatingCellsObject == null)
                {
                    floatingCellsObject = new GridModelFloatingCells(this);
                }

                return floatingCellsObject;
            }
        }

        GridModelFloatingCells savedFloatingCellsObject = null;

        /// <summary>
        /// Temporarily saves the state of floating cells, called from GridPrintDocument.OnBeginPrint before printing.
        /// </summary>
        public void PushPrintFloatingCells()
        {
            savedFloatingCellsObject = floatingCellsObject;
            floatingCellsObject = new GridModelFloatingCells(this);
            floatingCellsObject.DelayFloatCells(GridRangeInfo.Table());
        }

        /// <summary>
        /// Restores the state of floating cells that has been saved before with <see cref="PushPrintFloatingCells"/>. Called from GridPrintDocument.OnEndPrint after printing.
        /// </summary>
        public void ResetPrintFloatingCells()
        {
            floatingCellsObject = savedFloatingCellsObject;
        }

        /// <summary>
        /// Occurs after floating state was changed for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridFloatingCellsChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after floating state was changed for a range of cells."),
        Category("Behavior")]
        public event GridFloatingCellsChangedEventHandler FloatingCellsChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.FloatingCellsChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridFloatingCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFloatingCellsChanged(GridFloatingCellsChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnFloatingCellsChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (FloatingCellsChanged != null)
            {
                FloatingCellsChanged(this, e);
            }
        }

        internal void RaiseFloatingCellsChanged(GridFloatingCellsChangedEventArgs e)
        {
            OnFloatingCellsChanged(e);
        }

        [NonSerialized]
        private GridModelMergeCells mergeCellsObject = null;

        /// <summary>
        /// Gets merge cells in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelMergeCells MergeCells
        {
            get
            {
                if (mergeCellsObject == null)
                {
                    mergeCellsObject = new GridModelMergeCells(this);
                }

                return mergeCellsObject;
            }
        }

        /// <summary>
        /// Occurs after merge state was changed for a range of cells.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridMergeCellsChangedEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs after merge state was changed for a range of cells."),
        Category("Behavior")]
        public event GridMergeCellsChangedEventHandler MergeCellsChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.MergeCellsChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridMergeCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnMergeCellsChanged(GridMergeCellsChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMergeCellsChanged(e);
            }

            if (!CanGridRaiseEvents)
            {
                return;
            }
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif

            if (MergeCellsChanged != null)
            {
                MergeCellsChanged(this, e);
            }
        }

        internal void RaiseMergeCellsChanged(GridMergeCellsChangedEventArgs e)
        {
            OnMergeCellsChanged(e);
        }

        /// <summary>
        /// Occurs when grid compares the contents of two cells to determine if they should be merged. Set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True if you want to provide a customized comparison for cell contents.
        /// </summary>
        [Description("Occurs when grid compares the contents of two cells to determine if they should be merged."),
        Category("Behavior")]
        public event GridQueryCanMergeCellsEventHandler QueryCanMergeCells;

        /// <summary>
        /// Raises the <see cref="QueryCanMergeCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCanMergeCellsEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCanMergeCells(GridQueryCanMergeCellsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCanMergeCells(e);
            }
#if DEBUG

            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e);
            }
#else

            ;
#endif
            if (QueryCanMergeCells != null)
            {
                QueryCanMergeCells(this, e);
            }
        }

        internal void RaiseQueryCanMergeCells(GridQueryCanMergeCellsEventArgs e)
        {
            OnQueryCanMergeCells(e);
        }

        #region Borders
        /// <overload>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </overload>
        /// <summary>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="cellBounds">The <see cref="System.Drawing.Rectangle"/> with the cell bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the cell bounds excluding its borders.</returns>
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style, bool isRightToLeft)
        {
            GridMargins margins = StyleInfoBordersToMargins(style);
            if (isRightToLeft)
            {
                margins = margins.SwapRightToLeft();
            }

            return GridMargins.RemoveMargins(cellBounds, margins);
        }

        /// <summary>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="cellBounds">The <see cref="System.Drawing.Rectangle"/> with the cell bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the cell bounds excluding its borders.</returns>
        [Obsolete("It is recommended to specify isRightToLeft parameter, default for isRightToLeft is False.")]
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style)
        {
            return SubtractBorders(cellBounds, style, false);
        }

        /// <summary>
        /// Adds border margins to given cell client area size. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="size">The <see cref="System.Drawing.Size"/> with the cell size.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="System.Drawing.Size"/> with the cell size including its borders.</returns>
        public Size AddBorders(Size size, GridStyleInfo style)
        {
            return GridMargins.AddMargins(size, StyleInfoBordersToMargins(style));
        }

        /// <summary>
        /// Extracts <see cref="GridMargins"/> information from a <see cref="GridStyleInfo"/>.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="GridMargins"/> object with border margins.</returns>
        public GridMargins StyleInfoBordersToMargins(GridStyleInfo style)
        {
            bool showHorizontalLines = Properties.DisplayHorzLines;
            bool showVerticalLines = Properties.DisplayVertLines;
            GridBorderStyle defaultBorder = this.GetGridLineBorder().Style;

            GridMargins margins = style.BorderMargins.ToMargins();

            if (style.CellAppearance != GridCellAppearance.Flat)
            {
                margins._top++;
                margins._bottom++;
                margins._left++;
                margins._right++;
            }

            GridBordersInfo borders = style.ReadOnlyBorders;
            if (borders.Top.Style != GridBorderStyle.Standard
                && borders.Top.Style != GridBorderStyle.None)
            {
                margins._top += borders.Top.Width;
            }

            if (borders.Bottom.Style == GridBorderStyle.Standard)
            {
                if (showHorizontalLines && defaultBorder != GridBorderStyle.None)
                {
                    margins._bottom++;
                }
            }
            else if (borders.Bottom.Style != GridBorderStyle.None)
            {
                margins._bottom += borders.Bottom.Width;
            }

            if (borders.Left.Style != GridBorderStyle.Standard
                && borders.Left.Style != GridBorderStyle.None)
            {
                margins._left += borders.Left.Width;
            }

            if (borders.Right.Style == GridBorderStyle.Standard && defaultBorder != GridBorderStyle.None)
            {
                if (showVerticalLines && defaultBorder != GridBorderStyle.None)
                {
                    margins._right++;
                }
            }
            else if (borders.Right.Style != GridBorderStyle.None)
            {
                margins._right += borders.Right.Width;
            }

            return margins;
        }
        #endregion
        #region ControlPool
        GridCellModelCollection cellModels = null;

        /// <summary>
        /// Gets cell types for the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellModelCollection CellModels
        {
            get
            {
                if (cellModels == null)
                {
                    cellModels = new GridCellModelCollection(this);
                }

                return cellModels;
            }
        }

        GridCellModelBase IGridVolatileDataContainer.LookupCellModel(string id)
        {
            return CellModels[id];
        }

        #endregion

        internal event GridCellEventHandler SynchronizingCurrentCell;

        /// <summary>
        /// Raises the <see cref="SynchronizingCurrentCell"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs" /> that contains the event data.</param>
        internal void OnSynchronizingCurrentCell(GridCellEventArgs e)
        {
            if (this.ActiveGridView != null)
            {
                ActiveGridView.CurrentCell.gridModel_SynchronizingCurrentCell(this, e);
            }

            if (SynchronizingCurrentCell != null)
            {
                SynchronizingCurrentCell(this, e);
            }
        }

        /// <summary>
        /// Calls <see cref="SetActiveCurrentCell"/> and raises a <see cref="SynchronizingCurrentCell"/> event. Good for synchronizing
        /// current cell movements in splitter panes.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public void SynchronizeCurrentCell(int rowIndex, int colIndex)
        {
            lastSyncEditRow = rowIndex;
            lastSyncEditCol = colIndex;
            SetActiveCurrentCell(rowIndex, colIndex);
            OnSynchronizingCurrentCell(new GridCellEventArgs(rowIndex, colIndex));
        }

        [NonSerialized]
        private int lastSyncEditRow;

        [NonSerialized]
        private int lastSyncEditCol;

        /// <summary>
        /// Temporary data during OLE drag-and-drop operation. Implementation specific and will change in future versions.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public class InternalGridDragDropData
        {
            // Temporary attributes for drag-and-drop.

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public bool dndSource = false;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndStartRow = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndStartCol = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public GridRangeInfoList dndSelList = null;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndForceDropCol = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndForceDropRow = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndRowOffset = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public int dndColOffset = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public bool dndCurrentCellText = false;       //// True if selected text from current cell is dragged

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public GridCellRendererBase dndCurrentCellControl = null;

            // OLE data source.

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            public static bool dndGridSource = false;   //// True if grid is a data source.

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            public static int dndRowsCopied = 0;   ///// Number of rows / cols copied in OnDndCacheGlobalData.

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            public static int dndColsCopied = 0;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            public static bool dndGridTargetStyle = false;   //// True if grid was a target and style info was copied.

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            [Syncfusion.Documentation.DocumentationExclude()]
            [NonSerialized]
            public bool directDragDrop = false;

            /// <summary>
            /// Default Constructor.
            /// </summary>
            public InternalGridDragDropData()
                : base()
            {
            }
        }

        InternalGridDragDropData dragDropModel;

        /// <internalonly/>
        /// <summary>Gets DragDrop Data. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InternalGridDragDropData DragDropData
        {
            get
            {
                if (dragDropModel == null)
                {
                    dragDropModel = new InternalGridDragDropData();
                }

                return dragDropModel;
            }
        }

        /// <summary>
        /// Gets or sets more options for the grid. Printing related. Also manages colors for grid background, grid lines, and more.
        /// </summary>
        public GridProperties Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new GridProperties();
                }

                return properties;
            }

            set
            {
                if (value != this.properties)
                {
                    properties = value;
                }
            }
        }

        // Property-Object
        internal GridProperties properties = null;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        sealed class Binder : SerializationBinder
        {
            public override Type BindToType(
                string assemblyName, string typeName)
            {
                Type t = Type.GetType(typeName);
                if (t != null)
                {
                    return t;
                }
                else
                {
#if SINGLE_DLL_BUILD
                    t = Syncfusion.SharedBaseAssembly.Assembly.GetType(typeName);
#else
                    t = Syncfusion.SharedBaseAssembly.Assembly.GetType(typeName);
#endif
                }

                if (t != null)
                {
                    return t;
                }
                else
                {
                    t = Syncfusion.AssemblyInfo.Assembly.GetType(typeName);
                }

                return t;
            }
        }

        /// <summary>
        /// This is called from GridDropDownGridListControlCellModel to initialize datasource on demand.
        /// Override this method to calculate the datasource on demand
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.DataSource is returned.
        /// </summary>
        /// <param name="style">The style object that holds the binding information.</param>
        /// <returns>Style datasource.</returns>
        public virtual object GetStyleDataSource(GridStyleInfo style)
        {
            object dataSource = null;
            Type valueType = style.CellValueType;

            //// What to do with type converters? Should I still support them?
            ////            if (valueType != null)
            ////            {
            ////                TypeConverter tc = TypeDescriptor.GetConverter(valueType);
            ////                if (tc != null && tc.GetStandardValuesSupported())
            ////                {
            ////                    dataSource = tc.GetStandardValues();
            ////                    ICollection collection = tc.GetStandardValues();
            ////                    foreach (object item in collection)
            ////                        this.listBoxPart.Items.Add(item.ToString());
            ////                }
            ////                if (dataSource == null)
            ////                    return dataSource;
            ////            }

            if (style.ChoiceList != null && style.ChoiceList.Count > 0)
            {
                dataSource = style.ChoiceList;
            }
            else
            {
                dataSource = style.DataSource;
            }
            ////            if (dataSource == null)
            ////            {
            ////                Type valueType = style.CellValueType;
            ////                if (valueType != null)
            ////                    dataSource = tc.GetStandardValues();
            ////            }

            if (dataSource == null)
            {
                PropertyDescriptor pd = GetPropertyDescriptor(style);
                TypeConverter tc = GetTypeConverter(style);
                if (tc != null && tc.CanConvertTo(typeof(string)) && tc.GetStandardValuesSupported())
                {
                    return this.GetCachedStandardValues(tc, pd != null ? pd.PropertyType : style.CellValueType);
                }
            }

            return dataSource;
        }

        /// <summary>
        /// Returns GridStyleInfo.PropertyDescriptor.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A PropertyDescriptor</returns>
        internal PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }

        /// <summary>
        /// Returns a TypeConverter with type information about the style.CellValue.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A TypeConverter</returns>
        internal TypeConverter GetTypeConverter(GridStyleInfo style)
        {
            PropertyDescriptor pd = GetPropertyDescriptor(style);
            if (pd != null)
            {
                return pd.Converter;
            }

            Type type = style.CellValueType;
            if (type != null)
            {
                return TypeDescriptor.GetConverter(type);
            }

            return null;
        }

        Hashtable collectionToIListTable = new Hashtable();

        /// <internalonly/>
        /// <summary>
        /// Returns a list with standard values / possible choices for the specified TyepConverter and Type. Helper routined for <see cref="GetStyleDataSource"/>.
        /// </summary>
        /// <param name="converter">The TypeConverter</param>
        /// <param name="propertyType">The Type property</param>
        /// <returns>A list with standard values / possible choices.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridPropertyStandardValuesList GetCachedStandardValues(TypeConverter converter, Type propertyType)
        {
            if (converter != null && converter.GetStandardValuesSupported())
            {
                string name = converter.GetType().FullName + "@" + propertyType.FullName;
                if (collectionToIListTable.Contains(name))
                {
                    return collectionToIListTable[name] as GridPropertyStandardValuesList;
                }
                else
                {
                    ICollection list = converter.GetStandardValues();
                    GridPropertyStandardValuesList sl = new GridPropertyStandardValuesList("Value", propertyType);
                    sl.AddRange(list);
                    collectionToIListTable[name] = sl;
                    return sl;
                }
            }

            return null;
        }
    }

    /// <internalonly/>
    /// <summary>Internal only.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridPropertyStandardValuesList : ArrayList, ITypedList
    {
        string name;
        Type elementType;
        PropertyDescriptorCollection pdc;
        int maxLength = -1;
        internal string format = string.Empty;
        internal CultureInfo ci;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public GridPropertyStandardValuesList(string name, Type elementType)
        {
            this.name = name;
            this.elementType = elementType;
        }

        #region ITypedList Members

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects to find in the collection as bindable. This can be null.</param>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </returns>
        /// <internalonly/>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (pdc == null)
            {
                PropertyDescriptor pd = new GridPropertyStandardValuesSelfPropertyDescriptor(this, name, elementType);
                pdc = new PropertyDescriptorCollection(new PropertyDescriptor[] { pd });
            }

            return pdc;
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        /// <internalonly/>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return name;
        }
        #endregion

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="ci">The CultureInfo.</param>
        /// <returns>returns max length</returns>
        /// <internalonly/>
        public int GetMaxLength(string format, CultureInfo ci)
        {
            if (this.maxLength == -1 || format != this.format || ci != this.ci)
            {
                this.maxLength = -1;
                this.format = format;
                this.ci = ci;
                for (int n = 0; n < Count; n++)
                {
                    maxLength = Math.Max(GridCellValueConvert.FormatValue(this[n], elementType, format, ci, null).Length, maxLength);
                }
            }

            return maxLength;
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to be added to the end of the <see cref="T:System.Collections.ArrayList"/>. The value can be null.</param>
        /// <returns>
        /// The <see cref="T:System.Collections.ArrayList"/> index at which the <paramref name="value"/> has been added.
        /// </returns>
        /// <internalonly/>
        public override int Add(object value)
        {
            return base.Add(value);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void AddRange(ICollection c)
        {
            base.AddRange(c);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void Clear()
        {
            base.Clear();
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void Insert(int index, object value)
        {
            base.Insert(index, value);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void InsertRange(int index, ICollection c)
        {
            base.InsertRange(index, c);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void Remove(object obj)
        {
            base.Remove(obj);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public override object this[int index]
        {
            get
            {
                return base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }

    /// <internalonly/>
    /// <summary>Internal only.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridPropertyStandardValuesSelfPropertyDescriptor : PropertyDescriptor
    {
        Type type;
        GridPropertyStandardValuesList parent;

        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a FieldDescriptor.
        /// </summary>
        /// <param name="parent">A reference to <see cref="GridPropertyStandardValuesList"/>.</param>
        /// <param name="name">Proprety name.</param>
        /// <param name="type">Property type.</param>
        public GridPropertyStandardValuesSelfPropertyDescriptor(GridPropertyStandardValuesList parent, string name, Type type)
            : base(name, null)
        {
            this.parent = parent;
            this.type = type;
        }

        /// <override/>
        /// <summary>
        /// Determines a value indicating whether the
        /// value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for
        /// persistence. </param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Sets the value of the component to a
        /// different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.
        /// <param name="value">The new value. </param>        
        /// </param>
        public override void SetValue(object component, object value)
        {
        }

        /// <override/>
        /// <summary>
        /// Resets the value for this property of the
        /// component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be
        /// reset to the default value. </param>
        public override void ResetValue(object component)
        {
        }

        /// <override/>
        /// <summary>
        /// Gets the current value of the property on a
        /// component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve
        /// the value. </param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            return GridCellValueConvert.FormatValue(component, type, parent.format, parent.ci, null);
        }

        /// <override/>
        /// <summary>
        /// Returns whether resetting an object changes
        /// its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability. </param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return type;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets a value indicating whether this
        /// property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the type of the component this property
        /// is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return type;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets a value indicating whether the member is browsable.
        /// </summary>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <override/>
        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <override/>
        /// <summary>
        /// Compares this to another object to see if they are equivalent.
        /// </summary>
        /// <param name="other">The object to compare. </param>
        /// <returns>
        /// true if the values are equivalent; otherwise, false.
        /// </returns>
        public override bool Equals(object other)
        {
            return Object.ReferenceEquals(this, other);
        }
    }
}
