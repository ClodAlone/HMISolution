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


    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridModel.SelectionChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">An <see cref="GridSelectionChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridSelectionChangingEventHandler(object sender, GridSelectionChangingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridModel.SelectionChanging"/> event.
    /// </summary>
    /// <remarks>
    /// The GridSelectionChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.SelectionChanging"/> event
    /// when the model is in the process of selecting a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>
    /// method call.
    /// <para/>
    /// You can disallow the selection of specific cells at run-time when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also modify the <see cref="GridSelectionChangingEventArgs.Range"/> to include additional cells.
    /// <para/>
    /// The <see cref="GridModel"/> will raise a <see cref="GridModel.SelectionChanging"/> event before
    /// it updates its internal data structures and a <see cref="GridModel.SelectionChanged"/> event
    /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
    /// the selected range of cells.
    /// </remarks>
    /// <seealso cref="GridSelectionChangingEventHandler"/>
    /// <seealso cref="GridModel.SelectionChanging"/>
    /// <seealso cref="GridSelectionChangedEventArgs"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridSelectionChangingEventArgs : SyncfusionCancelEventArgs
    {
        private GridRangeInfo range;
        private GridSelectionReason reason;
        private GridRangeInfo clickRange = GridRangeInfo.Empty;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="reason">The current state of the user action and reason for this event (mouse, keyboard or programmatic).</param>
        /// <param name="clickRange">The range of cells to be selected when the previous range is reset. <para/>
        /// Will be set only if reason is GridSelectionReason.SetCurrentCell, GridSelectionReason.MouseDown, GridSelectionReason.MouseMove.
        /// </param>
        public GridSelectionChangingEventArgs(GridRangeInfo range, GridSelectionReason reason, GridRangeInfo clickRange)
            : this(range, reason)
        {
            this.clickRange = clickRange;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="reason">The current state of the user action and reason for this event (mouse, keyboard, or programmatic).</param>
        public GridSelectionChangingEventArgs(GridRangeInfo range, GridSelectionReason reason)
        {
            this.range = range;
            this.reason = reason;
        }

        /// <summary>
        /// The range of cells to be selected.
        /// </summary>
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        /// <summary>
        /// The range of cells to be selected when the previous range is reset. <para/>
        /// Will be set only if reason is GridSelectionReason.SetCurrentCell, GridSelectionReason.MouseDown, GridSelectionReason.MouseMove.
        /// </summary>
        public GridRangeInfo ClickRange
        {
            get
            {
                return clickRange;
            }
        }

        /// <summary>
        /// The current state of the user action and reason for this event (mouse, keyboard, or programmatic).
        /// </summary>
        public GridSelectionReason Reason
        {
            get
            {
                return reason;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.SelectionChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridSelectionChangedEventHandler(object sender, GridSelectionChangedEventArgs e);


    /// <summary>
    /// Provides data about the <see cref="GridModel.SelectionChanged"/> event.
    /// </summary>
    /// <remarks>
    /// The GridSelectionChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.SelectionChanged"/> event 
    /// when the model in the process of selecting a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>
    /// method call.
    /// <para/>
    /// The <see cref="GridModel"/> will raise a <see cref="GridModel.SelectionChanging"/> event before
    /// it updates its internal data structures and a <see cref="GridModel.SelectionChanged"/> event
    /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
    /// the selected range of cells.
    /// </remarks>
    /// <seealso cref="GridSelectionChangedEventHandler"/>
    /// <seealso cref="GridModel.SelectionChanged"/>
    /// <seealso cref="GridSelectionChangingEventArgs"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridSelectionChangedEventArgs : SyncfusionRoutedEventArgs
    {
        private GridRangeInfo range;
        private GridSelectionReason reason;
        private GridRangeInfoList oldRanges;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="oldRanges">A <see cref="GridRangeInfoList"/> that holds all selected ranges before this user action.</param>
        /// <param name="reason">The origin source for this event (mouse, keyboard, or programmatic).</param>
        public GridSelectionChangedEventArgs(GridRangeInfo range, GridRangeInfoList oldRanges, GridSelectionReason reason)
        {
            this.range = range;
            this.reason = reason;
            this.oldRanges = oldRanges;
        }

        /// <summary>
        /// The range of cells to be selected.
        /// </summary>
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }

        /// <summary>
        /// The origin source for this event (mouse, keyboard, or programmatic).
        /// </summary>
        public GridSelectionReason Reason
        {
            get
            {
                return reason;
            }
        }
        /// <summary>
        /// A <see cref="GridRangeInfoList"/> that holds all selected ranges before this user action.
        /// </summary>
        public GridRangeInfoList OldRanges
        {
            get
            {
                return oldRanges;
            }
        }
    }


}
