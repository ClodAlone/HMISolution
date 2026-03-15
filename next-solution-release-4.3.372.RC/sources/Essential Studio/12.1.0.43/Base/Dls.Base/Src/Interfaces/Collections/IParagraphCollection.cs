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
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Documents collection interface.
  /// </summary>
  public interface IParagraphCollection
    : ICollectionBase
  {
    /// <summary>
    /// Get document by index
    /// </summary>
    new IParagraph this[ int index ] { get; }
    /// <summary>
    /// Adds paragraph to end of section
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    int Add( IParagraph paragraph );
    /// <summary>
    /// Insert paragraph at spesified position
    /// </summary>
    /// <param name="index"></param>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    void Insert( int index, IParagraph paragraph );
  }
}