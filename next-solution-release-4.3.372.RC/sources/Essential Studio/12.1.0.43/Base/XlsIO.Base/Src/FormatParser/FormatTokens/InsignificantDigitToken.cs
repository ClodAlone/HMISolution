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
using System.Text;
#endregion

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for Significant Digit Tokens.
  /// </summary>
  public class InsignificantDigitToken : DigitToken
  {
    #region Class constants
    /// <summary>
    /// Format character.
    /// </summary>
    private const char DEF_FORMAT_CHAR = '#';
    #endregion

    #region Members
    /// <summary>
    /// Indicates that we shouldn't show string value if digit is zero.
    /// </summary>
    private bool m_bHideIfZero;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the InsignificantDigitToken class.
    /// </summary>
    public InsignificantDigitToken()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Returns string representation according to the current format and digit value.
    /// </summary>
    /// <param name="value">Value after removing specified digit.</param>
    /// <param name="iDigit">Digit to convert into string.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to show hidden symbols.</param>
    /// <returns>Converted value.</returns>
    internal protected override string GetDigitString( double value, int iDigit, bool bShowHiddenSymbols )
    {
      return (iDigit == 0 && (Math.Abs( value ) < 1 ||HideIfZero))
        ?(!IsCenterDigit)
        ?string.Empty:
         base.GetDigitString( value, iDigit, bShowHiddenSymbols )
        : base.GetDigitString( value, iDigit, bShowHiddenSymbols );
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
        return TokenType.InsignificantDigit;
      }
    }

    /// <summary>
    /// Format character. Read-only.
    /// </summary>
    public override char FormatChar
    {
      get
      {
        return DEF_FORMAT_CHAR;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show string value if digit is zero.
    /// </summary>
    public bool HideIfZero
    {
      get
      {
        return m_bHideIfZero;
      }
      set
      {
        m_bHideIfZero = value;
      }
    }
    #endregion
  }
}
