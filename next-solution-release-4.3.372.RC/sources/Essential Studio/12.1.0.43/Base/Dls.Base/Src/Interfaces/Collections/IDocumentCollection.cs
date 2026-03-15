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
  /// Document collection interface.
  /// </summary>
  public interface IDocumentCollection
    : ICollectionBase
  {
    /// <summary>
    /// Get document by index
    /// </summary>
    new IDocument this[ int index ] { get; }
  }
}