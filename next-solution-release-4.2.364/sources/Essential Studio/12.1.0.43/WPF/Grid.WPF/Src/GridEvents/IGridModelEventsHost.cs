#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.ComponentModel;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Grid;
namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Defines the workflow for grid model events.
    /// </summary>
    public interface IGridModelEventsHost
    {
        /// <summary>
        /// Occurs when the grid model is being disposed.
        /// </summary>
        /// <param name="disposing">True if the component is being disposed.</param>
        void OnDisposing(bool disposing);
        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellInfoEventArgs"/> containing event data.</param>
        void OnQueryCellInfo(GridQueryCellInfoEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardCanPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardCanPaste(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardCanCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardCanCopy(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardCanCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardCanCut(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardPaste(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardPasted(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardCut(GridCutPasteEventArgs e);
        /// <summary>
        /// Raises the <see cref="ClipboardCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs"/> containing event data.</param>
        void OnClipboardCopy(GridCutPasteEventArgs e);
        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        /// <param name="e">A <see cref="GridCommitCellInfoEventArgs"/> containing event data.</param>
        void OnCommitCellInfo(GridCommitCellInfoEventArgs e);
        /// <summary>
        /// Occurs when the model has saved style information about a specific cell.        
        /// </summary>
        /// <param name="e">A <see cref="GridCommitCellInfoEventArgs"/> containing event data.</param>
        void OnCommittedCellInfo(GridCommitCellInfoEventArgs e);
        /// <summary>
        /// Occurs when the model queries information about base styles at a specific cell.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryBaseStylesEventArgs"/> containing event data.</param>
        void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e);
        /// <summary>
        /// Occurs after a range of rows has been inserted.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs"/> containing event data.</param>
        void OnRowsInserted(GridRangeInsertedEventArgs e);
        /// <summary>
        /// Occurs after a range of rows has been removed.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs"/> containing event data.</param>
        void OnRowsRemoved(GridRangeRemovedEventArgs e);
        /// <summary>
        /// Occurs after a range of rows has been moved.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs"/> containing event data.</param>
        void OnRowsMoved(GridRangeMovedEventArgs e);
        /// <summary>
        /// Occurs after a range of columns has been inserted.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs"/> containing event data.</param>
        void OnColumnsInserted(GridRangeInsertedEventArgs e);
        /// <summary>
        /// Occurs after a range of columns has been removed.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs"/> containing event data.</param>
        void OnColumnsRemoved(GridRangeRemovedEventArgs e);
        /// <summary>
        /// Occurs after a range of columns has been moved.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs"/> containing event data.</param>
        void OnColumnsMoved(GridRangeMovedEventArgs e);
        /// <summary>
        /// Occurs when the GridModel.BaseStylesMap is changed.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> containing event data.</param>
        void OnBaseStyleMapsChanged(EventArgs e);
        /// <summary>
        /// Occurs when the CellModels collection is changed.
        /// </summary>
        /// <param name="e">A <see cref="CollectionChangeEventArgs"/> containing event data.</param>
        void OnCellModelsChanged(CollectionChangeEventArgs e);
        /// <summary>
        /// Occurs when querying for a cell type.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellModelEventArgs"/> containing event data.</param>
        void OnQueryCellModel(GridQueryCellModelEventArgs e);
        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to get the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/>.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs"/> containing event data.</param>
        void OnQueryCellFormattedText(GridCellTextEventArgs e);
        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to parse the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs"/> containing event data.</param>
        void OnSaveCellFormattedText(GridCellTextEventArgs e);
        /// <summary>
        /// Use this event to provide support for parsing the formatted string and convert
        /// it into the the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs"/> containing event data.</param>
        void OnParseCommonFormats(GridCellTextEventArgs e);
        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to get the raw string that represents the underlying cell's value.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs"/> containing event data.</param>
        void OnQueryCellText(GridCellTextEventArgs e);
        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to set the unformatted string that represents the underlying cell's value.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs"/> containing event data.</param>
        void OnSaveCellText(GridCellTextEventArgs e);
        /// <summary>
        /// Occurs when the model queries information about covered cells at a specific cell.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCoveredRangeEventArgs"/> containing event data.</param>
        void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e);
        /// <summary>
        /// Occurs when the model queries information about a cell spanned range at a specific cell.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellSpanBackgroundsEventArgs"/> containing event data.</param>
        void OnQueryCellSpansBackground(GridQueryCellSpanBackgroundsEventArgs e);
        /// <summary>
        /// Occurs after the model updates its internal data structures when the model is in the process of selecting
        /// a range of cells.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> containing event data.</param>
        void OnSelectionChanged(GridSelectionChangedEventArgs e);
        /// <summary>
        /// Occurs before the model updates internal data structures when the model is in the process of selecting
        /// a range of cells.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangingEventArgs"/> containing event data.</param>
        void OnSelectionChanging(GridSelectionChangingEventArgs e);
    }
}
