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

using System;

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for placing reserved digit token.
  /// </summary>
  public class PlaceReservedDigitToken : DigitToken
  {
    #region Class constants
    /// <summary>
    /// Format character.
    /// </summary>
    private const char DEF_FORMAT_CHAR = '?';
    /// <summary>
    /// String to display when digit is 0 and ShowHiddenSymbols is set to False.
    /// </summary>
    private const string DEF_EMPTY_DIGIT = " ";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the PlaceReservedDigitToken class.
    /// </summary>
    public PlaceReservedDigitToken()
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
//      if( iDigit < 0 || iDigit > 9 )
//        throw new ArgumentOutOfRangeException( "iDigit", iDigit, "Value cannot be less than 0 and greater than than 9." );

      return ( iDigit == 0 && !bShowHiddenSymbols && value < 1 )
        ? DEF_EMPTY_DIGIT
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
        return TokenType.PlaceReservedDigit;
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
    #endregion
  }
}
