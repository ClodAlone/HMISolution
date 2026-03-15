#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

#if WPF
using Syncfusion.Windows;
using Syncfusion.Windows.PdfViewer;
#elif WINDOWS
using Syncfusion.Windows;
using Syncfusion.Windows.Forms.PdfViewer;
#elif MVC
using Syncfusion.PdfViewer.Mvc;
#endif

namespace Syncfusion.PdfViewer.Base
{
    /// <summary>
    /// Base viewer interface for all PdfViewers.
    /// </summary>
    public interface IPdfDocumentView
    {
        /// <summary>
        /// Returns the number of pages.
        /// </summary>
        int PageCount { get; }
        /// <summary>
        /// Returns the current displayed page index.
        /// </summary>
        int CurrentPageIndex { get; }

        /// <summary>
        /// Gets or sets the ZoomMode.
        /// </summary>
        ZoomMode ZoomMode { get; set; }
        /// <summary>
        /// Zooms the document to the specified value.
        /// </summary>
        /// <param name="percentage">Value in Percentage</param>
        void ZoomTo(int percentage);
        /// <summary>
        /// Displays the first page.
        /// </summary>
        void GoToFirstPage();
        /// <summary>
        /// Displays the last page.
        /// </summary>
        void GoToLastPage();
        /// <summary>
        /// Displays the previous page.
        /// </summary>
        void GoToPreviousPage();
        /// <summary>
        /// Displays the next page.
        /// </summary>
        void GoToNextPage();
        /// <summary>
        /// Returns if first page can be displayed.
        /// </summary>
        bool CanGoToFirstPage { get; }
        /// <summary>
        /// Returns if previous page can be displayed.
        /// </summary>
        bool CanGoToPreviousPage { get; }
        /// <summary>
        /// Returns if next page can be displayed.
        /// </summary>
        bool CanGoToNextPage { get; }
        /// <summary>
        /// Returns if last page can be displayed.
        /// </summary>
        bool CanGoToLastPage { get; }
        /// <summary>
        /// Displays the page specified by the index.
        /// </summary>
        /// <param name="index">Index of the page</param>
        void GoToPageAtIndex(int index);
    }
}
