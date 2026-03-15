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
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.FormatParser
{
  /// <summary>
  /// Represents common fraction. Used to apply number format to the value.
  /// </summary>
  public class Fraction
  {
    #region Class constants
    /// <summary>
    /// Maximum number of digits.
    /// </summary>
    private const int DEF_MAX_DIGITS = 9;
    /// <summary>
    /// Maximum accuracy.
    /// </summary>
    private const double DEF_EPS = 1e-9;
    #endregion

    #region Class members
    /// <summary>
    /// Fraction numerator.
    /// </summary>
    private double m_dNumerator;
    /// <summary>
    /// Fraction denumerator.
    /// </summary>
    private double m_dDenumerator;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the Fraction class.
    /// </summary>
    /// <param name="dNumerator">Represents numerator of the fraction.</param>
    /// <param name="dDenumerator">Represents denominator of the fraction.</param>
    public Fraction( double dNumerator, double dDenumerator )
    {
      if( dDenumerator == 0 )
        throw new ArgumentOutOfRangeException( "dDenumerator" );

      m_dNumerator = dNumerator;
      m_dDenumerator = dDenumerator;
    }
    /// <summary>
    /// Initializes a new instance of the Fraction class.
    /// </summary>
    /// <param name="dNumerator">Represents numerator of the fraction.</param>
    public Fraction( double dNumerator )
      : this( dNumerator, 1 )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the fraction numerator.
    /// </summary>
    public double Numerator
    {
      get
      {
        return m_dNumerator;
      }
      set
      {
        m_dNumerator = value;
      }
    }
    /// <summary>
    /// Gets or sets the fraction denumerator.
    /// </summary>
    public double Denumerator
    {
      get
      {
        return m_dDenumerator;
      }
      set
      {
        m_dDenumerator = value;
      }
    }
    /// <summary>
    /// Gets the number of digits in the denumerator.
    /// </summary>
    public int DenumeratorLen
    {
      get
      {
        return ( int )Math.Log10( Denumerator ) + 1;
      }
    }
    #endregion

    #region Class operators
    /// <summary>
    /// Initializes a new instance of the Fraction class.
    /// </summary>
    /// <param name="term1">Represents first term of the fraction.</param>
    /// <param name="term2">Represents second term of the fraction.</param>
    /// <returns>Fraction of the numerator and denominator.</returns>
    public static Fraction operator+( Fraction term1, Fraction term2 )
    {
      double dNumerator = term1.Numerator * term2.Denumerator + term1.Denumerator * term2.Numerator;
      double dDenumerator = term2.Denumerator * term1.Denumerator;

      double lMCD = GetMaximumCommonDevisor( dNumerator, dDenumerator );
      dNumerator /= lMCD;
      dDenumerator /= lMCD;

      return new Fraction( dNumerator, dDenumerator );
    }
    /// <summary>
    /// Converts fraction to double value.
    /// </summary>
    /// <param name="fraction">Fraction to convert.</param>
    /// <returns>Double value equal to the fraction.</returns>
    public static explicit operator double( Fraction fraction )
    {
      return fraction.Numerator / /*( double )*/fraction.Denumerator;
    }
    /// <summary>
    /// Converts List into fraction.
    /// </summary>
    /// <param name="arrFraction">List to convert.</param>
    /// <returns>Converted value.</returns>
    public static explicit operator Fraction( List<double> arrFraction )
    {
      if( arrFraction == null )
        throw new ArgumentNullException( "arrFraction" );

      int iCount = arrFraction.Count;
      Fraction result = null;

      if( iCount > 0 )
      {
        double dValue = arrFraction[ iCount - 1 ];
        result = new Fraction( dValue, 1 );
 
        for( int i = iCount - 2; i >= 0; i-- )
        {
          dValue = arrFraction[ i ];
          result = result.Reverse() + ( Fraction )dValue;
        }
      }

      return result;
    }
    /// <summary>
    /// Converts long into fraction.
    /// </summary>
    /// <param name="dValue">Value to convert.</param>
    /// <returns>Converted value.</returns>
    public static explicit operator Fraction( double dValue )
    {
      return new Fraction( dValue );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Swaps numerator and denumerator.
    /// </summary>
    /// <returns>Current fraction after reverse.</returns>
    public Fraction Reverse()
    {
      double dTemp = m_dNumerator;
      m_dNumerator = m_dDenumerator;
      m_dDenumerator = dTemp;

      return this;
    }
    /// <summary>
    /// Converts number to fraction using required digits number.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="iDigitsNumber">Maximum number of digits.</param>
    /// <returns>Optimal fraction.</returns>
    public static Fraction ConvertToFraction( double value, int iDigitsNumber )
    {
      if( iDigitsNumber < 1 )
        throw new ArgumentOutOfRangeException( "iDigitsNumber" );

      iDigitsNumber = Math.Min( iDigitsNumber, DEF_MAX_DIGITS );
      List<double> arrFraction = new List<double>();
      double dLeft = value;

      // First step initialization.
      dLeft = AddNextNumber( arrFraction, dLeft );
      Fraction minFraction = ( Fraction )arrFraction;
      double delta = GetDelta( minFraction, value );
      Fraction curFraction = minFraction;

      while( Math.Abs( dLeft ) > DEF_EPS )
      {
        dLeft = AddNextNumber( arrFraction, dLeft );
        curFraction = ( Fraction )arrFraction;

        if( curFraction.DenumeratorLen <= iDigitsNumber )
        {
          double dCurDelta = GetDelta( curFraction, value );

          if( dCurDelta < delta )
          {
            minFraction = curFraction;
            delta = dCurDelta;
          }
        }
        else
        {
          break;
        }
      }

      return minFraction;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns maximum common divisor of two numbers using Euclid method.
    /// </summary>
    /// <param name="dNumerator">First number.</param>
    /// <param name="dDenumerator">Second number.</param>
    /// <returns>Maximum common divisor of two numbers.</returns>
    private static double GetMaximumCommonDevisor( double dNumerator, double dDenumerator )
    {
      double a = Math.Round( Math.Max( dNumerator, dDenumerator ) );
      double b = Math.Round( Math.Min( dNumerator, dDenumerator ) );
      double mod = a % b;

      if( b == 0 ) return 1;

      while( mod != 0 )
      {
        a = b;
        b = mod;
        mod = Math.Round( a % b );
      }

      return b;
    }
    /// <summary>
    /// Returns absolute difference between fraction and double.
    /// </summary>
    /// <param name="fraction">Fraction for difference evaluation.</param>
    /// <param name="value">Double value for difference evaluation.</param>
    /// <returns>Evaluated difference.</returns>
    private static double GetDelta( Fraction fraction, double value )
    {
      return Math.Abs( ( double )fraction - value );
    }
    /// <summary>
    /// Adds next number to List with continued fraction.
    /// </summary>
    /// <param name="arrFraction">List with continued fraction.</param>
    /// <param name="dLeft">Remainder of the fraction.</param>
    /// <returns>Remainder after adding next number to the fraction.</returns>
    private static double AddNextNumber( List<double> arrFraction, double dLeft )
    {
      if( Math.Abs( dLeft ) < DEF_EPS )
        return 0;

      int iCount = arrFraction.Count;
      double dValue;

      if( iCount != 0 )
      {
        dValue = 1 / dLeft;
        dLeft = dValue;
      }

      dValue = Math.Floor( dLeft );
      arrFraction.Add( dValue );
      return dLeft - dValue;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Returns string representation of the Fraction.
    /// </summary>
    /// <returns>String representation of the Fraction.</returns>
    public override string ToString()
    {
      return m_dNumerator.ToString() + " / " + m_dDenumerator.ToString();
    }

    #endregion
  }
}
