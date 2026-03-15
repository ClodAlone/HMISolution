#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Supports cloning, which creates a new instance of a class
  /// with the same value as an existing instance.
  /// </summary>
  public interface ICloneParent
  {
    #region Methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    object Clone( object parent );
    #endregion
  }
}
