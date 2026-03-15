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
  /// Class used for describing DecimalSeparatorToken.
  /// </summary>
  public class DecimalPointToken : SingleCharToken
  {
    #region Class constants
    /// <summary>
    /// Format character.
    /// </summary>
    private const char DEF_FORMAT = '.';
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the DecimalPointToken class.
    /// </summary>
    public DecimalPointToken()
    {
    }
    #endregion

    #region Class overrides

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
        if (m_strFormat == null)
            return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;// DEF_FORMAT.ToString();
        else
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
      return string.Empty;
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Format character. Read-only.
    /// </summary>
    public override char FormatChar
    {
      get
      {
        return DEF_FORMAT;
      }
    }

    /// <summary>
    /// Returns type of the token. Read-only.
    /// </summary>
    public override TokenType TokenType
    {
      get
      {
        return TokenType.DecimalPoint;
      }
    }

    #endregion
  }
}
