#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Specifies type of paginating.
    /// </summary>
    public enum PdfLayoutType
    {
        /// <summary>
        /// If the element exceeds the page, proceed it on the next page.
        /// </summary>
        Paginate,

        /// <summary>
        /// Draw the element on the one page only.
        /// </summary>
        OnePage
    }

    /// <summary>
    /// Specifies how the element should be contained on the page.
    /// </summary>
    public enum PdfLayoutBreakType
    {
        /// <summary>
        /// Fit the element according to the bounds specified or the page bounds.
        /// </summary>
        FitPage,

        /// <summary>
        /// If the element doesn't fit at the first page, don't draw it on this page.
        /// </summary>
        FitElement
    }
}
