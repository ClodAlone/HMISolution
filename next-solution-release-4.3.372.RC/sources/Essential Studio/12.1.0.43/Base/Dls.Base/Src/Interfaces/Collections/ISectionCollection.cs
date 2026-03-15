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
  /// Section collection interface.
  /// </summary>
  public interface ISectionCollection
    : ICollectionBase
  {
    /// <summary>
    /// Gets section by index
    /// </summary>
    new ISection this[ int index ] { get; }
    /// <summary>
    /// Adds new section
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    int Add( ISection section );
  }
}