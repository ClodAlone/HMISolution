#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// Summary description for IToken3D.
  /// </summary>
  public interface IToken3D
  {
    /// <summary>
    /// Converts current token to the 3D token.
    /// </summary>
    /// <param name="iSheetReference">Reference to the worksheet.</param>
    /// <returns>Created token.</returns>
    Ptg Get3DToken( int iSheetReference );
  }
}
