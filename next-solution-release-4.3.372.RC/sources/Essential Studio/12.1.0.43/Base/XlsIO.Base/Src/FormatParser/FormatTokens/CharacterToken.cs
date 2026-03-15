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
  /// Class used for Character Token.
  /// </summary>
  public class CharacterToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Start of the token.
    /// </summary>
    private const char DEF_START = '\\';
    private const char DEF_FORMAT_CHAR = '@';
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the CharacterToken class
    /// </summary>
    public CharacterToken()
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
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      if( strFormat[ iIndex ] == DEF_START )
      {
        m_strFormat = strFormat[ iIndex + 1 ].ToString();
        if (m_strFormat != DEF_FORMAT_CHAR.ToString ())
            iIndex += 2;
        else
            m_strFormat = DEF_FORMAT_CHAR.ToString ();
      }
      else if( strFormat[ iIndex ] == '[' && strFormat[ iIndex + 2 ] == '$' )
      {
        m_strFormat = strFormat[ iIndex + 1 ].ToString();
        iIndex = strFormat.IndexOf( ']', iIndex + 3 ) + 1;
      }

      return iIndex;
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
      return m_strFormat;
    }

    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( string value, bool bShowHiddenSymbols )
    {
      return m_strFormat;
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
        return TokenType.Character;
      }
    }

    #endregion
  }
}