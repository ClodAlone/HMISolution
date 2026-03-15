#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// A provider for covered cells. GridCoveredCellInfoCollection
    /// is a default implementation of this interface. The <see cref="VirtualizingCellsControl"/>
    /// retrieves covered cells through this interface from it <see cref="VirtualizingCellsControl.CoveredCellsProvider"/>
    /// </summary>
    public interface ICoveredCellsProvider
    {
        /// <summary>
        /// Gets a covered cell from the <see cref="ICoveredCellsProvider"/> that includes
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        CoveredCellInfo GetCoveredCell(int rowIndex, int columnIndex);

        /// <summary>
        /// Gets a value indicating if the collection is empty.
        /// </summary>
        bool IsEmpty { get; }
    }
}
