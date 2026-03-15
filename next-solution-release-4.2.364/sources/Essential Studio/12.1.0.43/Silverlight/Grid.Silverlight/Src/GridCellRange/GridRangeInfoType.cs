#region Copyright Syncfusion Inc. 2001 - 2014
// -----------------------------------------------------------------------
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// -----------------------------------------------------------------------
#endregion
#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Identifies the range type for a <see cref="GridRangeInfo"/>.
    /// </summary>
    [Flags]
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public enum GridRangeInfoType
    {
        /// <summary>
        /// Range is empty.
        /// </summary>
        Empty = 0x00,

        /// <summary>
        /// Range of cells.
        /// </summary>
        Cells = 0x01,

        /// <summary>
        /// Range with rows.
        /// </summary>
        Rows = 0x02,

        /// <summary>
        /// Range with columns.
        /// </summary>
        Cols = 0x04,

        /// <summary>
        /// Range is a whole table.
        /// </summary>
        Table = Rows | Cols
    }
}
