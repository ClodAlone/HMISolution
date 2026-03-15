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
using Syncfusion.DLS.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publish section functionality
  /// </summary>
  public interface ITextBody : IEntityBase
  {
    /// <summary>
    /// Gets inner paragraphs.
    /// </summary>
    IParagraphCollection Paragraphs { get; }
    /// <summary>
    /// Adds paragraph at the end of section.
    /// </summary>
    /// <returns></returns>
    IParagraph AddParagraph();
    /// <summary>
    /// Inserts html at end of text body.
    /// </summary>
    void InsertHTML( string html );
    /// <summary>
    /// Inserts html begins from paragraph specified by paragraphIndex.
    /// </summary>
    void InsertHTML( string html, int paragraphIndex );
    /// <summary>
    /// Inserts html begining from paragraph specified by paragraphIndex, 
    /// and after paragraph item specified by paragraphItemIndex.
    /// </summary>
    void InsertHTML( string html, int paragraphIndex, int paragraphItemIndex );
  }
}