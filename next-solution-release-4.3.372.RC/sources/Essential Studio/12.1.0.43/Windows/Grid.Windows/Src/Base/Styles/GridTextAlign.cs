//-------------------------------------------------------------------------------------------------
// <copyright file="GridTextAlign.cs" company="syncfusion">
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
    /// The text alignment for <see cref="GridStyleInfo.TextAlign"/>.
    /// </summary>
    public enum GridTextAlign
    {
        /// <summary>
        /// Default. Use setting defined as default for the cell type.
        /// </summary>
        Default,

        /// <summary>
        /// Align text left of button elements. This is typical for combo boxes.
        /// </summary>
        Left,

        /// <summary>
        /// Align text right of button elements.
        /// </summary>
        Right
    }
}
