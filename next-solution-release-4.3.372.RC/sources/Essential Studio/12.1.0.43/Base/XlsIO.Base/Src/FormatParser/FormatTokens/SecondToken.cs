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
  /// Class used for Seconds Token.
  /// </summary>
  public class SecondToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Regular expression for minutes part of the format:
    /// </summary>
    private static readonly Regex SecondRegex = new Regex( "[sS]+", DEF_OPTIONS );
    /// <summary>
    /// Long type of the format.
    /// </summary>
    private const string DEF_FORMAT_LONG = "00";
    /// <summary>
    /// Half of possible milliseconds.
    /// </summary>
    private const int DEF_MILLISECOND_HALF = 500;
    /// <summary>
    /// Default OleDateValue
    /// </summary>
    private const double DEF_OLE_DOUBLE = 2958465.9999999884;
    /// <summary>
    /// Maximum OleDateValue
    /// </summary>
    private const double DEF_MAX_DOUBLE = 2958466.0;
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether number of seconds must be rounded.
    /// </summary>
    private bool m_bRound = true;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the SecondToken class
    /// </summary>
    public SecondToken()
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
      return TryParseRegex( SecondRegex, strFormat, iIndex );
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
      int iSecond = date.Second;
      int iMilliSecond = date.Millisecond;

      if( m_bRound && iMilliSecond >= DEF_MILLISECOND_HALF ) iSecond++;

      if( m_strFormat.Length > 1 )
      {
        return iSecond.ToString( DEF_FORMAT_LONG );
      }
      else
      {
        return iSecond.ToString();
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
        return TokenType.Second;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether number of seconds must be rounded.
    /// </summary>
    public bool RoundValue
    {
      get
      {
        return m_bRound;
      }
      set
      {
        m_bRound = value;
      }
    }
    #endregion
  }
}
