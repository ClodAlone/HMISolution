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
  /// This class represents the unknown token of a formula.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class UnknownPtg : Ptg
  {
    #region Class constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public UnknownPtg()
    {
    }
    /// <summary>
    /// Creates token from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data in data array.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public UnknownPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
      offset++;
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 1;
    }

    /// <summary>
    /// Converts token to string.
    /// </summary>
    /// <returns>Converts token to the string.</returns>
    public override string ToString()
    {
      return "( not implemented or UNKNOWN " + this.TokenCode.ToString() + ")";
    }
    #endregion
  }
}
