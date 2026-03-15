#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
#if !WinRT
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridModelEventsHost
    {
        void OnDisposing(bool disposing);
        void OnQueryCellInfo(GridQueryCellInfoEventArgs e);
        void OnCommitCellInfo(GridCommitCellInfoEventArgs e);
        void OnCommittedCellInfo(GridCommitCellInfoEventArgs e);
        void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e);
        void OnRowsInserted(GridRangeInsertedEventArgs e);
        void OnRowsRemoved(GridRangeRemovedEventArgs e);
        void OnRowsMoved(GridRangeMovedEventArgs e);
        void OnColumnsInserted(GridRangeInsertedEventArgs e);
        void OnColumnsRemoved(GridRangeRemovedEventArgs e);
        void OnColumnsMoved(GridRangeMovedEventArgs e);
        void OnBaseStyleMapsChanged(EventArgs e);
        //void OnCellModelsChanged(CollectionChangeEventArgs e);
        void OnQueryCellModel(GridQueryCellModelEventArgs e);
        void OnQueryCellFormattedText(GridCellTextEventArgs e);
        void OnSaveCellFormattedText(GridCellTextEventArgs e);
        void OnParseCommonFormats(GridCellTextEventArgs e);
        void OnQueryCellText(GridCellTextEventArgs e);
        void OnSaveCellText(GridCellTextEventArgs e);
        void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e);
        void OnQueryCellSpansBackground(GridQueryCellSpanBackgroundsEventArgs e);
        void OnSelectionChanged(GridSelectionChangedEventArgs e);
        void OnSelectionChanging(GridSelectionChangingEventArgs e);

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
        /// Raises the <see cref="ClipboardPasted"/> event.
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
    }

}
