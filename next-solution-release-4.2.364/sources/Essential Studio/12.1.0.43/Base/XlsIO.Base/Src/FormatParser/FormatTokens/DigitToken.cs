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
using System.Globalization;
#endregion

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for describing Digit Tokens.
  /// </summary>
  public abstract class DigitToken : SingleCharToken
  {
    #region Class members
    /// <summary>
    /// Indicates whether this digit is last in the sequence of digits
    /// and all significant numbers should be displayed.
    /// </summary>
    private bool m_bLastDigit;
    private bool m_bCenterDigit;
    private double m_originalValue;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the DigitToken class.
    /// </summary>
    public DigitToken()
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
      int iDigit;

      iDigit = GetDigit( ref value );
      m_bCenterDigit = false;
      GetIndexOfZero(OriginalValue, iDigit);
      if (iDigit == byte.MaxValue)
          iDigit = 0;
      return GetDigitString( value, iDigit, bShowHiddenSymbols );
    }

    private void GetIndexOfZero(double value,int iDigit)
    {
        if (iDigit == 0)
        {
            int startIndex = value.ToString().IndexOf(iDigit.ToString());
            int lastIndex = value.ToString().LastIndexOf(iDigit.ToString());
            if ((startIndex != value.ToString().Length - 1) && (startIndex != 0) && (startIndex != -1) && CheckIsZeroes(startIndex, lastIndex, value.ToString()))
                m_bCenterDigit = true;
        }
    }

    private bool CheckIsZeroes(int startIndex, int lastIndex, string p)
    {
        bool zeroCheck = true;

        if (startIndex == lastIndex)
            return zeroCheck;

        for (int index = startIndex; index<= lastIndex; index++)
        {
            if (p[index] == '0')
            {
                zeroCheck = false;
            }
            else
            {
                zeroCheck = true;
                break;
            }
        }
        return zeroCheck;
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( string value, bool bShowHiddenSymbols )
    {
      return value;
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets a value indicating whether this digit is last in the sequence of digits
    /// and all significant numbers should be displayed. Read-only.
    /// </summary>
    public bool IsLastDigit
    {
      get
      {
        return m_bLastDigit;
      }
      set
      {
        m_bLastDigit = value;
      }
    }
    public bool IsCenterDigit
    {
        get
        {
            return m_bCenterDigit;
        }
    }
    internal double OriginalValue
    {
        get
        {
            return m_originalValue;
        }
        set
        {
            m_originalValue = value;
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Gets single digit from number and updates number.
    /// </summary>
    /// <param name="value">Number to get digit from.</param>
    /// <returns>Extracted digit.</returns>
    internal protected int GetDigit( ref double value )
    {
      int iDigit;

      value = FormatSection.Round( value );//Math.Round( value );

      iDigit = ( int )( value % 10 );
      value /= 10;
      if (iDigit==0 && value == 0)
          return byte.MaxValue;
      int iSign = Math.Sign( value );
      value = ( value > 0 ) ? Math.Floor( value ) : Math.Ceiling( value );

      // We need to have some decimal fraction part in order to maintain sign,
      // but we also need to avoid rounding, so we can add 0.1.
      value += iSign * 0.1;

      return iDigit;
    }
    /// <summary>
    /// Returns string representation according to the current format and digit value.
    /// </summary>
    /// <param name="value">Value after removing current digit.</param>
    /// <param name="iDigit">Digit to convert into string.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to show hidden symbols.</param>
    /// <returns>Converted value.</returns>
    internal protected virtual string GetDigitString( double value, int iDigit, bool bShowHiddenSymbols )
    {
      iDigit = Math.Abs( iDigit );
      if( /*iDigit < 0 ||*/ iDigit > 9 )
        throw new ArgumentOutOfRangeException( "iDigit", "Value cannot be less than -9 and greater than than 9." );

      return iDigit.ToString();
    }
    #endregion
  }
}
