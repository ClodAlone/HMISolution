#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Allows to choose outline text style.
    /// </summary>
    [Flags()]
    public enum PdfTextStyle
    {
        /// <summary>
        /// Regular text style.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Italic text style.
        /// </summary>
        Italic = 1,

        /// <summary>
        /// Bold text style.
        /// </summary>
        Bold = 2,
    }
}
