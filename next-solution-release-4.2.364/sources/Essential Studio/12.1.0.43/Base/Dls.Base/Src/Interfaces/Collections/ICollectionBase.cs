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
using System.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents base interface for dls collections.
  /// </summary>
  public interface ICollectionBase : IList
  {
    /// <summary>
    /// Gets index of specific item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf( IEntityBase item );
  }
}