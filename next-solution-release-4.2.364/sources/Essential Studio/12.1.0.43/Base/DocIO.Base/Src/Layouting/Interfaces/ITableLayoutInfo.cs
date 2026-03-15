#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

using System;
using Syncfusion.DocIO.DLS;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for TableLayoutInfo.
    /// </summary>
    internal interface ITableLayoutInfo
    {

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        float Width { get; set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        float Height { get; }

        /// <summary>
        /// Gets or sets the width of the cells.
        /// </summary>
        /// <value>The width of the cells.</value>
        float[] CellsWidth { get; set; }

        /// <summary>
        /// Gets the headers row count.
        /// </summary>
        /// <value>The headers row count.</value>
        int HeadersRowCount { get; }

        /// <summary>
        /// Gets the is default cells.
        /// </summary>
        /// <value>The is default cells.</value>
        bool[] IsDefaultCells { get; }

        /// <summary>
        /// Gets or sets if the table is splitted
        /// </summary>
        bool IsSplittedTable { get; set; }

        /// <summary>
        /// Gets the cell spacings.
        /// </summary>
        /// <value>The cell spacings.</value>
        double CellSpacings { get; }

        /// <summary>
        /// Gets the cell paddings.
        /// </summary>
        /// <value>The cell paddings.</value>
        double CellPaddings { get; }
    }
}

#endif