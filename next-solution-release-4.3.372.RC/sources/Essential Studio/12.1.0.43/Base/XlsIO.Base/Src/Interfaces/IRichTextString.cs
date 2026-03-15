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
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a Rich Text String that can be used to apply several styles inside a single cell.
  /// </summary>
  public interface IRichTextString
    : IParentApplication
    , IOptimizedUpdate
  {
    #region Interface Methods
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    IFont GetFont( int iPosition );
    /// <summary>
    /// Sets font for range of characters.
    /// </summary>
    /// <param name="iStartPos">First character of the range.</param>
    /// <param name="iEndPos">Last character of the range.</param>
    /// <param name="font">Font to set.</param>
    void SetFont( int iStartPos, int iEndPos, IFont font );
    /// <summary>
    /// Clears string formatting.
    /// </summary>
    void ClearFormatting();
    /// <summary>
    /// Clears text and formatting.
    /// </summary>
    void Clear();
    /// <summary>
    /// Appends rich text string with specified text and font.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="font">Font to use.</param>
    void Append( string text, IFont font );
    #endregion

    #region Iterface Properties
    /// <summary>
    /// Gets / sets text of the string.
    /// </summary>
    string Text { get; set; }
    /// <summary>
    /// Returns text in rtf format. Read-only.
    /// </summary>
    string RtfText { get; set; }
    /// <summary>
    /// Indicates whether rich text string has formatting runs. Read-only.
    /// </summary>
    bool IsFormatted { get; }
    #endregion
  }
}
