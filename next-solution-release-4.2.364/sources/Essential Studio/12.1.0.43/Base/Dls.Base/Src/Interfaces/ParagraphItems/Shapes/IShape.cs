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
  /// Represents a graphic shape object.
  /// </summary>
  public interface IShape
  {
    /// <summary>
    /// Gets name of style attached to the shape.
    /// </summary>
    string StyleName { get; }
    /// <summary>
    /// Attaches style to the shape.
    /// </summary>
    /// <param name="styleName">Style to be attached to the shape.</param>
    void ApplyStyle( string styleName );
  }
}