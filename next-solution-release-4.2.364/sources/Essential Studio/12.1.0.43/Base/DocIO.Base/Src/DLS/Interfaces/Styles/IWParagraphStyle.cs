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

using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the style that can be used to format a Paragraph.
    /// </summary>
    public interface IWParagraphStyle : IStyle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary style.
        /// </summary>
        /// <value>
        /// 	if this instance is primary style, set to <c>true</c>.
        /// </value>
        bool IsPrimaryStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets formatting of paragraph.
        /// </summary>
        WParagraphFormat ParagraphFormat { get; }
        /// <summary>
        /// Gets formatting of characters inside paragraph.
        /// </summary>
        WCharacterFormat CharacterFormat { get; }
    }
}
