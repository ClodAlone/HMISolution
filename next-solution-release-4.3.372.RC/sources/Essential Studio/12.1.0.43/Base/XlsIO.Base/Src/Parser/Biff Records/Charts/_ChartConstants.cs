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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// Rotation type.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TRotation : int
  {
    /// <summary>
    /// Represents the LeftToRight rotation type.
    /// </summary>
    LeftToRight = 0,
    /// <summary>
    /// Represents the TopToBottom rotation type.
    /// </summary>
    TopToBottom = 1,
    /// <summary>
    /// Represents the CounterClockwise rotation type.
    /// </summary>
    CounterClockwise = 2,
    /// <summary>
    /// Represents the Clockwise rotation type.
    /// </summary>
    Clockwise = 3,
  }

}
