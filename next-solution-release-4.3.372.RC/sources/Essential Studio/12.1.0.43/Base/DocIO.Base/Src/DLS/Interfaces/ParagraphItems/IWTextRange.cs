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
using Syncfusion.DocIO.DLS;
using WCharacterFormat = Syncfusion.DocIO.DLS.WCharacterFormat;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Interface publishes text range functionality
    /// </summary>
    public interface IWTextRange : IParagraphItem
    {
        /// <summary>
        /// Gets / sets text.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Gets text format.
        /// </summary>
        WCharacterFormat CharacterFormat { get; }
        /// <summary>
        /// Applies specified character format for current text range.
        /// </summary>
        /// <param name="charFormat"></param>
        void ApplyCharacterFormat(WCharacterFormat charFormat);
    }
}