#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.IO;

using Syncfusion.HTMLUI.Base;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface for HTML elements which contain CSS information.
    /// </summary>
    internal interface IElementHasCss
    {
        /// <summary>
        /// Gets or sets the array of formats created from this element.
        /// </summary>
        IHTMLFormat[] CreatedFormats
        { 
            get;
            set; 
        }

        /// <summary>
        /// Returns the CSS data from the HTML element.
        /// </summary>
        /// <returns>Null if element does not contains any CSS; 
        /// TokenStream with position set to the CSS data otherwise.</returns>
        TokenStream GetCssStream();
    }
}