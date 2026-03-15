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
#endregion

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for MilliSecond Token.
  /// </summary>
  public class MilliSecondToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Long format.
    /// </summary>
    private const string DEF_FORMAT_LONG = "000";
    /// <summary>
    /// Maximum format length.
    /// </summary>
    private static readonly int DEF_MAX_LEN = DEF_FORMAT_LONG.Length;
    /// <summary>
    /// Represents Dot.
    /// </summary>
    private const string DEF_DOT = ".";
    /// <summary>
    /// Default OleDateValue
    /// </summary>
    private const double DEF_OLE_DOUBLE = 2958465.9999999884;
    /// <summary>
    /// Maximum OleDateValue
    /// </summary>
    private const double DEF_MAX_DOUBLE = 2958466.0;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the MilliSecondToken class.
    /// </summary>
    public MilliSecondToken()
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
      throw new NotImplementedException();
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
      int iMilliSecond = date.Millisecond;
      int iFormatLen = m_strFormat.Length;
      string strNativeFormat = string.Empty;
      string strPostfix = string.Empty;

      if( iFormatLen < DEF_MAX_LEN )
      {
        int iPow = DEF_MAX_LEN - iFormatLen;
        iMilliSecond = ( int )FormatSection.Round( iMilliSecond / Math.Pow( 10, iPow ) );
        strNativeFormat = m_strFormat.Substring( 1, iFormatLen - 1 );
      }
      else
      {
        strNativeFormat = DEF_FORMAT_LONG;
        strPostfix = m_strFormat.Substring( DEF_MAX_LEN );
      }

      return DEF_DOT + iMilliSecond.ToString( strNativeFormat ) + strPostfix;
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
        return TokenType.MilliSecond;
      }
    }
    #endregion
  }
}
