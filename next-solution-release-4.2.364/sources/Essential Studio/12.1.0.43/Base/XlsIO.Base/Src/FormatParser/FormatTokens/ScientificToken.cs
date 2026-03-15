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
  /// Class used for Scientific Token.
  /// </summary>
  public class ScientificToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// One of the possible formats.
    /// </summary>
    private static readonly string[] PossibleFormats = new string[]
    {
      "E+",
      "E-",
    };
    /// <summary>
    /// Short form without plus sign.
    /// </summary>
    private const string DEF_SHORT_FORM = "E";
    #endregion

    #region Class members
    /// <summary>
    /// Used format index.
    /// </summary>
    private int m_iFormatIndex = -1;
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
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty" );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 and greater than than format length - 1." );

      m_iFormatIndex = FindString( PossibleFormats, strFormat, iIndex, false );

      if( m_iFormatIndex < 0 ) return iIndex;

      return iIndex + PossibleFormats[ m_iFormatIndex ].Length;
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
      if( m_iFormatIndex == 0 && value >= 0 )
      {
        return PossibleFormats[ 0 ];
      }
      else
      {
        return DEF_SHORT_FORM;
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
        return TokenType.Scientific;
      }
    }
    #endregion
  }
}
