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

#region file using directives
using System;
using WTextBoxFormat = Syncfusion.DocIO.DLS.WTextBoxFormat;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for ITextBox.
    /// </summary>
    public interface IWTextBox : IParagraphItem, ICompositeEntity
    {
        /// <summary>
        /// Gets the text box body.
        /// </summary>
        /// <value>The text box body.</value>
        WTextBody TextBoxBody { get; }
        /// <summary>
        /// Gets or sets the text box format.
        /// </summary>
        /// <value>The text box format.</value>
        WTextBoxFormat TextBoxFormat { get; set; }
    }
}
