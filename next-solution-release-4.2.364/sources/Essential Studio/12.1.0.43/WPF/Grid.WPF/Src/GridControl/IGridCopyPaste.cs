#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if !WinRT
using Syncfusion.Windows.Controls.Grid;
namespace Syncfusion.Windows.Controls.Grid
{
#else
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
    /// <summary>
    /// For Custom Copy To Clipboard.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridCopyPaste
    {
        /// <summary>
        /// User use this method to implement Cut Operation
        /// </summary>
        /// <param name="gridCellData">Contain The Currently Selected Cells Style</param>
        /// <param name="rangeList">Contain The Currently Selected Range</param>
        void Copy(GridCellData gridCellData, GridRangeInfoList rangeList);

        /// <summary>
        /// User use this method to implement Copy Operation
        /// </summary>
        /// <param name="gridCellData">Contain The Currently Selected Cells Style</param>
        /// <param name="rangeList">Contain The Currently Selected Range</param>
        void Cut(GridCellData gridCellData, GridRangeInfoList rangeList);

#if (!SILVERLIGHT &&  !WinRT)
        /// <summary>
        /// User use this method to implement Paste Operation
        /// </summary>
        /// <param name="RangeList">Contain The Currently Selected Cells Style</param>
        DataObject Paste(GridRangeInfoList rangeList);
#else

         /// <summary>
         /// User use this method to implement Paste Operation
         /// </summary>
         /// <param name="RangeList">Contain The Currently Selected Cells Style</param>
         string Paste(GridRangeInfoList rangeList);
#endif
    }
}
