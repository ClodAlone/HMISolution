#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Holds the event argument values for <see cref="GridControlBase.PrepareRenderCell"/> event.
    /// </summary>
    /// <seealso cref="GridPrepareRenderCellEventHandler"/>
    public sealed class GridPrepareRenderCellEventArgs : SyncfusionRoutedEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;

        /// <summary>
        /// Initializes a new <see cref="GridPrepareRenderCellEventArgs"/>.
        /// </summary>
        /// <param name="cell">Cell row column index.</param>
        /// <param name="style">Cell style.</param>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridPrepareRenderCellEventArgs(RowColumnIndex cell, GridStyleInfo style, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.cell = cell;
            this.style = style;
        }

        /// <summary>
        /// Gets the cell as <see cref="RowColumnIndex"/>.
        /// </summary>
        [TraceProperty(true)]
        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }
    }

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.PrepareRenderCell"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">A <see cref="GridPrepareRenderCellEventArgs"/> that contains the event data.</param>
    public delegate void GridPrepareRenderCellEventHandler(object sender, GridPrepareRenderCellEventArgs e);

}
