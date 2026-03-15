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
using System;
using System.Globalization;
using System.Text.RegularExpressions;
#endregion

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for second total token.
  /// </summary>
  public class SecondTotalToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Regular expression for hours part of the format:
    /// </summary>
    private static readonly Regex HourRegex = new Regex( "\\[[sS]+\\]", DEF_OPTIONS );
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the SecondTotalToken class.
    /// </summary>
    public SecondTotalToken()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Tries to parse format string.
    /// </summary>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iIndex">Position to start parsing at.</param>
    /// <returns>Position after parsed block.</returns>
    public override int TryParse( string strFormat, int iIndex )
    {
      return TryParseRegex( HourRegex, strFormat, iIndex );
    }

    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <param name="culture">Culture used to convert value into text.</param>
    /// <param name="section">Parent section.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( ref double value, bool bShowHiddenSymbols,
      CultureInfo culture, FormatSection section )
    {
      double dSeconds = value * FormatConstants.SecondsInDay;
      return ( ( int )dSeconds ).ToString();
    }

    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( string value, bool bShowHiddenSymbols )
    {
      return string.Empty;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns type of the token. Read-only.
    /// </summary>
    public override TokenType TokenType
    {
      get
      {
        return TokenType.SecondTotal;
      }
    }

    #endregion
  }
}
