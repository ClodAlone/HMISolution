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
    /// Represents the style that can be used to format a Table.
    /// </summary>
    internal interface IWTableStyle : IStyle
    {
        /// <summary>
        /// Gets paragraph format.
        /// </summary>
        WParagraphFormat ParagraphFormat { get; }
        /// <summary>
        /// Gets the list format.
        /// </summary>
        WListFormat ListFormat { get; }
        /// <summary>
        /// Gets character format.
        /// </summary>
        WCharacterFormat CharacterFormat { get; }
        /// <summary>
        /// Gets cell properties.
        /// </summary>
        TableStyleCellProperties CellProperties { get; }
        /// <summary>
        /// Gets row properties.
        /// </summary>
        TableStyleRowProperties RowProperties { get; }
        /// <summary>
        /// Gets table properties.
        /// </summary>
        TableStyleTableProperties TableProperties { get; }
    }
}
