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
  /// Class used for Hour Token.
  /// </summary>
  public class HourToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Regular expression for hours part of the format:
    /// </summary>
    private static readonly Regex HourRegex = new Regex( "[hH]+", DEF_OPTIONS );
    /// <summary>
    /// Long format.
    /// </summary>
    private const string DEF_FORMAT_LONG = "00";
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether token should be formatted using am/pm time format.
    /// </summary>
    private bool m_bAmPm;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the HourToken class.
    /// </summary>
    public HourToken()
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
      DateTime date =
#if ( WINRT )
 Syncfusion.XlsIO.Implementation.DateTimeExtension.FromOADate(value);
#else
          DateTime.FromOADate( value );
#endif
      int iHour = date.Hour;

      if( IsAmPm && iHour > 12 ) iHour -= 12;

      if( m_strFormat.Length > 1 )
      {
        return iHour.ToString( DEF_FORMAT_LONG );
      }
      else
      {
        return iHour.ToString();
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
        return TokenType.Hour;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether token should be formatted using am/pm time format.
    /// </summary>
    public bool IsAmPm
    {
      get
      {
        return m_bAmPm;
      }
      set
      {
        m_bAmPm = value;
      }
    }
    #endregion
  }
}
