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
  /// Class used for Color Tokens.
  /// </summary>
  public class ColorToken : InBracketToken
  {
    #region Class constants
    /// <summary>
    /// Regex for color selecting.
    /// </summary>
    private static readonly Regex ColorRegex = new Regex( "[Color [0-9]+]" );
    /// <summary>
    /// Possible color values.
    /// </summary>
    private static readonly string[] DEF_KNOWN_COLORS = new string[]
    {
      "Black",
      "White",
      "Red",
      "Green",
      "Blue",
      "Yellow",
      "Magenta",
      "Cyan",
//      "Black",
//      "Blue",
//      "Cyan",
//      "Green",
//      "Magenta",
//      "Red",
//      "White",
//      "Yellow",
    };
    /// <summary>
    /// Reserved keyword for color definition in excel number format.
    /// </summary>
    private const string DEF_COLOR = "Color";
    /// <summary>
    /// Minimum possible color index.
    /// </summary>
    private const int DEF_MIN_COLOR_INDEX = 1;
    /// <summary>
    /// Maximum possible color index.
    /// </summary>
    private const int DEF_MAX_COLOR_INDEX = 56;
    /// <summary>
    /// Color increment.
    /// </summary>
    private const int DEF_COLOR_INCREMENT = 7;
    #endregion

    #region Class members
    /// <summary>
    /// Color index.
    /// </summary>
    private int m_iColorIndex = -1;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the ColorToken class
    /// </summary>
    public ColorToken()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Tries to parse format string.
    /// </summary>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iStartIndex">Position of the first bracket.</param>
    /// <param name="iIndex">Position to start parsing at.</param>
    /// <param name="iEndIndex">Position of the end bracket.</param>
    /// <returns>Position after parsed block.</returns>
    public override int TryParse( string strFormat, int iStartIndex, int iIndex, int iEndIndex )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 and greater than than format length - 1." );

      m_iColorIndex  = FindColor( strFormat, iIndex );

      if( m_iColorIndex < 0 )
      {
        m_iColorIndex = TryDetectColorNumber( strFormat, iIndex, iEndIndex );

        if( m_iColorIndex < 0 )return iStartIndex;

        m_iColorIndex += DEF_COLOR_INCREMENT;
        iIndex = iEndIndex + 1;
      }
      else
      {
        string strColor = DEF_KNOWN_COLORS[ m_iColorIndex ];
        iIndex += strColor.Length;
        m_strFormat = strColor;

        if( iIndex != iEndIndex ) return iStartIndex;

        iIndex++;
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
      return string.Empty;
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
        return TokenType.Color;
      }
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Searches for color in the array of known colors.
    /// </summary>
    /// <param name="strFormat">Format string.</param>
    /// <param name="iIndex">Start index.</param>
    /// <returns>Index in the array of known colors or -1 if color was not found.</returns>
    private int FindColor( string strFormat, int iIndex )
    {
      return FindString( DEF_KNOWN_COLORS, strFormat, iIndex, true );
    }
    /// <summary>
    /// Tries to get color number from format string.
    /// </summary>
    /// <param name="strFormat">Format string</param>
    /// <param name="iIndex">Index to start looking from.</param>
    /// <param name="iEndIndex">End index of the token.</param>
    /// <returns>Extracted color index or -1 if color was not found.</returns>
    private int TryDetectColorNumber( string strFormat, int iIndex, int iEndIndex )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 and greater than than format length - 1." );


      if( iEndIndex < 0 || iEndIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iEndIndex", "Value cannot be less than 0 and greater than than format length - 1." );

      int iColorLen = DEF_COLOR.Length;

      if( string.Compare( strFormat, iIndex, DEF_COLOR, 0, iColorLen, StringComparison.CurrentCultureIgnoreCase ) == 0 )
      {
        // Here we should extract color number from format.
        int iStartIndex = iIndex + iColorLen;
        string strNumber = strFormat.Substring( iStartIndex, iEndIndex - iStartIndex );

        double dResult;
        bool bResult = double.TryParse( strNumber, NumberStyles.Integer, null, out dResult );

        if( bResult )
        {
          int iColorIndex = ( int )dResult;

          if( iColorIndex < DEF_MIN_COLOR_INDEX || iColorIndex > DEF_MAX_COLOR_INDEX )
            throw new ArgumentOutOfRangeException( "Color index" );

          return iColorIndex;
        }
      }

      return -1;
    }
    #endregion
  }
}
