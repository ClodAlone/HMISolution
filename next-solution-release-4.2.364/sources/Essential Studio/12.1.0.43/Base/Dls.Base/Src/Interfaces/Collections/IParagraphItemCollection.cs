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

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Paragraph item collection interface
  /// </summary>
  public interface IParagraphItemCollection
    : ICollectionBase
  {
    /// <summary>
    /// Gets item by index
    /// </summary>
    new IParagraphItem this[ int index ] { get; }
    /// <summary>
    /// Adds paragraph item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    int Add( IParagraphItem pItem );
    /// <summary>
    /// Gets index of specified paragraph item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf( IParagraphItem item );
    /// <summary>
    /// Inserts an paragraph item to collection
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pItem"></param>
    void Insert( int index, IParagraphItem pItem );
    /// <summary>
    /// Removes an paragraph item from collection
    /// </summary>
    /// <param name="item"></param>
    void Remove( IParagraphItem item );
  } 
}