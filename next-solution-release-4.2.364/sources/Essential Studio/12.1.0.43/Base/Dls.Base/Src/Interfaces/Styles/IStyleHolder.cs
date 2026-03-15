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

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes base functionality for style holders
  /// </summary>
  public interface IStyleHolder
  {
    /// <summary>
    /// Gets style name.
    /// </summary>
    string StyleName { get; }
    /// <summary>
    /// Applies a new style.
    /// </summary>
    /// <param name="styleName"></param>
    void ApplyStyle( string styleName );
  }
}