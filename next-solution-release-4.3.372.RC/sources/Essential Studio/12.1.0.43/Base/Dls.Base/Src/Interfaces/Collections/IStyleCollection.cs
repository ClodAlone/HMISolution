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
  /// Style collection interface.
  /// </summary>
  public interface IStyleCollection
    : ICollectionBase
  {
    /// <summary>
    /// Get style by index
    /// </summary>
    new IStyle this[ int index ] { get; }
    /// <summary>
    /// Adds new style
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    int Add( IStyle style );
    /// <summary>
    /// Finds style by style name
    /// </summary>
    /// <param name="name"></param>
    IStyle FindByName( string name );
  }
}