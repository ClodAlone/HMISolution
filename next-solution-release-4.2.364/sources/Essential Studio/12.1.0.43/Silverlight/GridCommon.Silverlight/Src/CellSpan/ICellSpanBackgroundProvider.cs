#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// A provider for cell span backgrounds. GridCellSpanBackgroundInfoCollection
    /// is a default implementation of this interface. The <see cref="VirtualizingCellsControl"/>
    /// retrieves cell span backgrounds through this interface from it <see cref="VirtualizingCellsControl.CellSpanBackgroundsProvider"/>
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface ICellSpanBackgroundsProvider
    {
        /// <summary>
        /// Gets the cell span backgrounds that include
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        List<CellSpanBackgroundInfo> GetCellSpanBackgrounds(int rowIndex, int columnIndex);

        /// <summary>
        /// Gets a value indicating if the collection is empty.
        /// </summary>
        bool IsEmpty { get; }
    }
}
