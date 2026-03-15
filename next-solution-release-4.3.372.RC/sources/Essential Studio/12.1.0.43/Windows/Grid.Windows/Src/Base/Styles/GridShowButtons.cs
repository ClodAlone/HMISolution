//-------------------------------------------------------------------------------------------------
// <copyright file="GridShowButtons.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Specifies when to show or display cell button elements.
    /// </summary>
    public enum GridShowButtons
    {
        /// <summary>
        /// Hide them always.
        /// </summary>
        Hide,

        /// <summary>
        /// Show them always.
        /// </summary>
        Show,

        /// <summary>
        /// Show buttons for current row. You should also set GridRefreshCurrentCellBehavior.RefreshRow.
        /// </summary>
        ShowCurrentRow,

        /// <summary>
        /// Show buttons only for the current cell.
        /// </summary>
        ShowCurrentCell,

        /// <summary>
        /// Show buttons only for the current cell when it is being edited.
        /// </summary>
        ShowCurrentCellEditing,
    }
}
