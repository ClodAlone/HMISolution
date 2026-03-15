#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Documents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;
    using System.Windows;
    using Syncfusion.Windows.Controls.Grid;
using System.Windows.Controls;
    using System.Windows.Printing;

    /// <summary>
    /// Provides properties and methods for implementing a print paginator for grid.
    /// </summary>
    public interface IGridPrintPaginator
    {
        /// <summary>
        /// Returns the rows per page based on the UIElement's available space
        /// </summary>
        /// <param name="printSize">Print size.</param>
        /// <returns>Rows per page.</returns>
        int GetPrintTotalPageCount(Size printSize);

        /// <summary>
        /// Sets the print page size based on the print size.
        /// </summary>
        /// <param name="printSize">Print size.</param>
        void SetPrintPageSize(Size printSize);

        /// <summary>
        /// Gets the visual of the given page.
        /// </summary>
        /// <param name="pageNumber">The page no. for which to return the visual.</param>
        /// <returns>The visual of the given page.</returns>
       // FrameworkElement GetPrintVisualAt(int pageNumber);

        System.Windows.Controls.StackPanel GetPrintVisualAt(int pageNumber); 
       // System.Windows.Controls.Grid GetPrintVisualAt1(int pageNumber, GridControlBase pe); 

        /// <summary>
        /// Gets or sets the range of grid cells to be printed.
        /// </summary>
        GridRangeInfo PrintRange { get; set; }

        /// <summary>
        /// Gets or sets the height of the print header.
        /// </summary>
        double PrintHeaderHeight { get; set; }

        /// <summary>
        /// Gets or sets the height of the print footer.
        /// </summary>
        double PrintFooterHeight { get; set; }

        /// <summary>
        /// Gets or sets the print header template.
        /// </summary>
        DataTemplate PrintHeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the print footer template.
        /// </summary>
        DataTemplate PrintFooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the print content.
        /// </summary>
        string PrintDescription { get; set; }



        
    }
}
