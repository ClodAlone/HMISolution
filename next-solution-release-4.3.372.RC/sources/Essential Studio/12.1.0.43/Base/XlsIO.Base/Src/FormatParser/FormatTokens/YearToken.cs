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
  /// Class used for YearToken.
  /// </summary>
  public class YearToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Regular expression for minutes part of the format:
    /// </summary>
    private static readonly Regex YearRegex = new Regex( "[yY]+", DEF_OPTIONS );
    /// <summary>
    /// Short year format.
    /// </summary>
    private const string DEF_FORMAT_SHORT = "00";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the YearToken class.
    /// </summary>
    public YearToken()
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
    public override int TryParse(string strFormat, int iIndex)
    {
      return TryParseRegex( YearRegex, strFormat, iIndex );
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

      DateTime date =
#if ( WINRT ) 
          Syncfusion.XlsIO.Implementation.DateTimeExtension.FromOADate(value);
#else
          DateTime.FromOADate( value );
#endif
      int iYear = date.Year;

      if( m_strFormat.Length > 2 )
      {
        return iYear.ToString();
      }
      else
      {
        return ( iYear % 100 ).ToString( DEF_FORMAT_SHORT );
      }
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
        return TokenType.Year;
      }
    }

    #endregion
  }
}
