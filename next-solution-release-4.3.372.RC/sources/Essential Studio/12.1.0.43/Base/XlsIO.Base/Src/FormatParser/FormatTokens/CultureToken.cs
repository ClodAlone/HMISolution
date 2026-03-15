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
  /// Class used for Culture Tokens.
  /// </summary>
  public class CultureToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Group in regular expression with locale id.
    /// </summary>
    private const string DEF_LOCALE_GROUP = "LocaleID";
    /// <summary>
    /// Group in regular expression with characters.
    /// </summary>
    private const string DEF_CHAR_GROUP = "Character";
    /// <summary>
    /// LocaleId value that indicates that we should use system settings instead of provided number format.
    /// </summary>
    private const int SystemSettingsLocaleId = 0xF800;
    /// <summary>
    /// Regular expression for hours part of the format:
    /// </summary>
    //private static readonly Regex CultureRegex = new Regex( "\\[\\$\\-(?<"
    //  + DEF_LOCALE_GROUP + ">[0-9A-Za-z]+)\\]", DEF_OPTIONS );
    private static readonly Regex CultureRegex = new Regex( "\\[\\$(?<" + DEF_CHAR_GROUP + ">.?)\\-(?<"
      + DEF_LOCALE_GROUP + ">[0-9A-Za-z]+)\\]", DEF_OPTIONS );
    #endregion

    #region Class members
    /// <summary>
    /// Locale id of the desired culture.
    /// </summary>
    private int m_iLocaleId;
    /// <summary>
    /// Character specifying the culture.
    /// </summary>
    private string m_strCharacter;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the CultureToken class.
    /// </summary>
    public CultureToken()
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
      Match m;
      int iResult = TryParseRegex( CultureRegex, strFormat, iIndex, out m );

      if( iResult != iIndex )
      {
        string strLocale = m.Groups[ DEF_LOCALE_GROUP ].Value;
        m_iLocaleId = int.Parse( strLocale, NumberStyles.HexNumber );

        m_strCharacter = m.Groups[ DEF_CHAR_GROUP ].Value;
      }

      return iResult;
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
      return ( m_strCharacter != null ) ?
        m_strCharacter :
        string.Empty;
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat(string value, bool bShowHiddenSymbols)
    {
      return ( m_strCharacter != null ) ?
        m_strCharacter :
        string.Empty;
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
        return TokenType.Culture;
      }
    }

    /// <summary>
    /// Gets the culture info. Read-only.
    /// </summary>
    public CultureInfo Culture
    {
      get
      {
#if !SILVERLIGHT && !WINRT && !WP
          try
          {
              return new CultureInfo(m_iLocaleId);
          }
          catch (Exception exception)
          {
              return CultureInfo.CurrentCulture;
          }
#else
        return CultureInfo.CurrentCulture;
#endif
      }
    }
    /// <summary>
    /// Gets a value indicating whether we should ignore all following format tokens and use system date format. Read-only.
    /// </summary>
    public bool UseSystemSettings
    {
      get
      {
        return m_iLocaleId == SystemSettingsLocaleId;
      }
    }
    #endregion
  }
}
