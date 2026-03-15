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
  /// Interface publishes section functionality
  /// </summary>
  public interface ISection : ITextBody
  {
    /// <summary>
    /// Gets headers/footers of current section.
    /// </summary>
    HeadersFooters HeadersFooters { get; }
    /// <summary>
    /// Gets page Setup of current section.
    /// </summary>
    PageSetup PageSetup { get; }
    /// <summary>
    /// Get collection of columns which logically divide page on many 
    /// printing/publishing areas.
    /// </summary>
    ColumnCollection Columns { get; }
    /// <summary>
    /// Gets / sets break code.
    /// </summary>
    SectionBreakCode BreakCode { get; set; }
    /// <summary>
    /// Adds new column to the section.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="spacing"></param>
    /// <returns></returns>
    Column AddColumn( float width, float spacing );
    /// <summary>
    /// Clones section and sets new owner document.
    /// </summary>
    /// <param name="document">New owner document</param>
    /// <returns></returns>
    ISection Clone( IDocument document );
    /// <summary>
    /// Makes all columns in current section to be of equal width.
    /// </summary>
    void MakeColumnsEqual();
  }
}
