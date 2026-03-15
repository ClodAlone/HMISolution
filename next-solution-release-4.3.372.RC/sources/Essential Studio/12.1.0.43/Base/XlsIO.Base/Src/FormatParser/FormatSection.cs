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
using System.Collections;
using System.Globalization;

using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.FormatParser
{
  /// <summary>
  /// Class used for Format Section.
  /// </summary>
  public class FormatSection
    : CommonObject
  {
    #region Class constants

    #region DEF_POSSIBLE_TOKENS
    /// <summary>
    /// Table for token type detection. Value in TokenType arrays must be sorted.
    /// </summary>
    private static readonly object[] DEF_POSSIBLE_TOKENS = new object[]
    {
       new TokenType[]
      {
        TokenType.Unknown,
        TokenType.String,
        TokenType.ReservedPlace,
        TokenType.Character,
        TokenType.Color,
      },
      ExcelFormatType.Unknown,

      new TokenType[]{ TokenType.General, TokenType.Culture }, ExcelFormatType.General,

      new TokenType[]
      {
        TokenType.Unknown,
        TokenType.String,
        TokenType.ReservedPlace,
        TokenType.Character,
        TokenType.Color,
        TokenType.Condition,
        TokenType.Text,
        TokenType.Asterix,
        TokenType.Culture,
      }, 
      ExcelFormatType.Text,

      new TokenType[]
      {
        TokenType.Unknown,        
        TokenType.String,
        TokenType.ReservedPlace,
        TokenType.Character,
        TokenType.Color,
        TokenType.Condition,
        TokenType.SignificantDigit,
        TokenType.InsignificantDigit,
        TokenType.PlaceReservedDigit,
        TokenType.Percent,
        TokenType.Scientific,
        TokenType.ThousandsSeparator,
        TokenType.DecimalPoint,
        TokenType.Asterix,
        TokenType.Fraction,
        TokenType.Culture,
      }, ExcelFormatType.Number,

      new TokenType[]
      {
        TokenType.Unknown,
        TokenType.Day,
        TokenType.String,
        TokenType.ReservedPlace,
        TokenType.Character,
        TokenType.Color,
        TokenType.Condition,
        TokenType.SignificantDigit,
        TokenType.InsignificantDigit,
        TokenType.PlaceReservedDigit,
        TokenType.Percent,
        TokenType.Scientific,
        TokenType.ThousandsSeparator,
        TokenType.DecimalPoint,
        TokenType.Asterix,
        TokenType.Fraction,
        TokenType.Culture,
      }, ExcelFormatType.Number,

      new TokenType[]
      { 
        TokenType.Unknown,
        TokenType.Hour,
        TokenType.Hour24,
        TokenType.Minute,
        TokenType.MinuteTotal,
        TokenType.Second,
        TokenType.SecondTotal,
        TokenType.Year,
        TokenType.Month,
        TokenType.Day,
        TokenType.String,
        TokenType.ReservedPlace,
        TokenType.Character,
        TokenType.AmPm,
        TokenType.Color,
        TokenType.Condition,
        //TokenType.InsignificantDigit,
        TokenType.SignificantDigit,
        TokenType.DecimalPoint,
        TokenType.Asterix,
        TokenType.Fraction,
        TokenType.Culture,
      }, ExcelFormatType.DateTime,

     
    };
    #endregion

    /// <summary>
    /// Break tokens when locating hour token.
    /// </summary>
    private static readonly TokenType[] DEF_BREAK_HOUR = new TokenType[]
    {
      TokenType.Minute,
    };
    /// <summary>
    /// Break tokens when locating second token.
    /// </summary>
    private static readonly TokenType[] DEF_BREAK_SECOND = new TokenType[]
    {
      TokenType.Minute,
      TokenType.Hour,
      TokenType.Day,
      TokenType.Month,
      TokenType.Year,
    };

    /// <summary>
    /// Return this value when element wasn't found.
    /// </summary>
    private const int DEF_NOTFOUND_INDEX = -1;
    /// <summary>
    /// Possible digit tokens in the millisecond token.
    /// </summary>
    private static readonly TokenType[] DEF_MILLISECONDTOKENS = new TokenType[]
    {
      //TokenType.InsignificantDigit,
      TokenType.SignificantDigit,
    };

    /// <summary>
    /// Thousand separator.
    /// </summary>
    private const string DEF_THOUSAND_SEPARATOR = ",";
    /// <summary>
    /// Minus sign.
    /// </summary>
    private const string DEF_MINUS = "-";
    private static readonly TokenType[] NotTimeTokens =
    {
      TokenType.Day,
      TokenType.Month,
      TokenType.Year,
    };
    #endregion

    #region Class members
    /// <summary>
    /// Array of tokens.
    /// </summary>
    private List<FormatTokenBase> m_arrTokens;
    /// <summary>
    /// Indicates whether format is prepared.
    /// </summary>
    private bool m_bFormatPrepared;
    /// <summary>
    /// Position of decimal separator.
    /// </summary>
    private int m_iDecimalPointPos = -1;
    /// <summary>
    /// Position of E/E+ or E- signs in format string.
    /// </summary>
    private int m_iScientificPos = -1;
    /// <summary>
    /// Last digit.
    /// </summary>
    private int m_iLastDigit = -1;
    /// <summary>
    /// Indicates whether there are groups after last digit.
    /// </summary>
    private bool m_bLastGroups = false;
    /// <summary>
    /// Number of digits in decimal fraction part.
    /// </summary>
    private int m_iNumberOfFractionDigits = 0;
    /// <summary>
    /// Number of digits in integer part of number.
    /// </summary>
    private int m_iNumberOfIntegerDigits = 0;
    /// <summary>
    /// Position where fraction sign '/' was met for the first time.
    /// </summary>
    private int m_iFractionPos = -1;
    /// <summary>
    /// Indicates whether number format contains fraction sign.
    /// </summary>
    private bool m_bFraction;
    /// <summary>
    /// Start of the fraction numerator group.
    /// </summary>
    private int m_iFractionStart;
    /// <summary>
    /// End of the fraction denumerator group.
    /// </summary>
    private int m_iFractionEnd;
    /// <summary>
    /// Length of the denumerator.
    /// </summary>
    private int m_iDenumaratorLen;
    /// <summary>
    /// End position of the integer value.
    /// </summary>
    private int m_iIntegerEnd = -1;
    /// <summary>
    /// End position of the decimal value.
    /// </summary>
    private int m_iDecimalEnd = -1;
    /// <summary>
    /// Condition token.
    /// </summary>
    private ConditionToken m_condition;
    /// <summary>
    /// Culture token. Used to display values.
    /// </summary>
    private CultureToken m_culture;
    /// <summary>
    /// Section format type.
    /// </summary>
    private ExcelFormatType m_formatType = ExcelFormatType.Unknown;
    /// <summary>
    /// Indicates whether we digits must be grouped.
    /// </summary>
    private bool m_bGroupDigits;
    /// <summary>
    /// Indicates whether more than one decimal point was met in the format string.
    /// </summary>
    private bool m_bMultiplePoints;
    /// <summary>
    /// Indicates whether we should use system 
    /// </summary>
    private bool m_bUseSystemDateFormat;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the FormatSection class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    private FormatSection( IApplication application, object parent )
      : base ( application, parent )
    {
    }
    /// <summary>
    /// Initializes a new instance of the FormatSection class based on array of tokens.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="arrTokens">Array of section's tokens.</param>
    public FormatSection( IApplication application, object parent, List<FormatTokenBase> arrTokens )
      : base ( application, parent )
    {
      if( arrTokens == null )
        throw new ArgumentNullException( "arrTokens" );

      m_arrTokens = new List<FormatTokenBase>( arrTokens );
      PrepareFormat();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Prepares format if necessary.
    /// </summary>
    public void PrepareFormat()
    {
      if( m_bFormatPrepared ) return;

      PreparePositions();

      m_iLastDigit = LocateLastFractionDigit();
      m_bLastGroups = LocateLastGroups( m_iLastDigit + 1 );

      if( FormatType == ExcelFormatType.Number )
      {
        m_iNumberOfFractionDigits = CalculateFractionDigits();
        m_iNumberOfIntegerDigits = CalculateIntegerDigits();
        LocateFractionParts();
      }
      else if( FormatType == ExcelFormatType.DateTime )
      {
        SetRoundSeconds();
        m_iDecimalPointPos = -1;
        m_bFraction = false;
      }
      else
      {
        m_iNumberOfFractionDigits = -1;
        m_iNumberOfIntegerDigits = -1;
      }

      int iCount = Count;
      m_iDecimalEnd = iCount - 1;
      m_iIntegerEnd = iCount - 1;

      if( m_iScientificPos > 0 )
      {
        m_iIntegerEnd = m_iDecimalEnd = m_iScientificPos - 1;
      }
      else if( m_bFraction )
      {
        LocateFractionParts();
        m_iIntegerEnd = m_iFractionStart - 1;
      }

      if( m_iDecimalPointPos > 0 )
      {
        m_iIntegerEnd = m_iDecimalPointPos - 1;
      }

      if( FormatType == ExcelFormatType.Number )
      {
        m_bGroupDigits = CheckGroupDigits();
      }

      m_bFormatPrepared = true;

      if( m_arrTokens.Count > 0 )
      {
        CultureToken culture = m_arrTokens[ 0 ] as CultureToken;
        
        if( culture != null )
        {
          m_bUseSystemDateFormat = culture.UseSystemSettings;
        }
      }
    }
    /// <summary>
    /// Checks whether digits must be grouped.
    /// </summary>
    /// <returns>True if digits must be grouped, otherwise returns false.</returns>
    private bool CheckGroupDigits()
    {
      for( int i = 0, len = m_iIntegerEnd - 1; i <= len; i++ )
      {
        if( this[ i ].TokenType == TokenType.ThousandsSeparator 
          && this[ i - 1 ] is DigitToken && this[ i + 1 ] is DigitToken )
        {
          return true;
        }
      }

      return false;
    }
    /// <summary>
    /// Prepares tokens and sets iternal position pointers.
    /// </summary>
    private void PreparePositions()
    {
      bool bDigit = false;
      m_bMultiplePoints = false;

      for( int i = 0, len = Count; i < len; i++ )
      {
        FormatTokenBase token = this[ i ];

        switch( token.TokenType )
        {
          case TokenType.AmPm:
            HourToken hour = FindCorrespondingHourSection( i );
            if( hour != null ) hour.IsAmPm = true;
            break;

          case TokenType.Minute:
            CheckMinuteToken( i );
            break;

          case TokenType.DecimalPoint:
            if( m_iDecimalPointPos < 0 )
            {
              AssignPosition( ref m_iDecimalPointPos, i );
            }
            else
            {
              m_bMultiplePoints = true;
            }
            break;

          case TokenType.Scientific:
            AssignPosition( ref m_iScientificPos, i );
            break;

          case TokenType.SignificantDigit:
          case TokenType.InsignificantDigit:
          case TokenType.PlaceReservedDigit:
            DigitToken digit = ( DigitToken )token;

            if( !bDigit )
            {
              digit.IsLastDigit = true;
              bDigit = true;
            }
            break;

          case TokenType.Fraction:
            if( m_iFractionPos < 0 )
            {
              m_iFractionPos = i;
              m_bFraction = true;
            }
            else
            {
              m_bFraction = false;
            }
            break;

          case TokenType.Condition:
            if( m_condition != null )
              throw new FormatException( "More than one condition was found in the section." );

            m_condition = ( ConditionToken )token;
            break;

          case TokenType.Culture:
            if( m_culture != null )
              throw new FormatException( "More than one culture information was found in the section" );

            m_culture = ( CultureToken )token;
            break;
        }
      }

      PrepareInsignificantDigits();
    }
    /// <summary>
    /// Prepares insignificant digits that are present in decimal fraction part of this section.
    /// </summary>
    private void PrepareInsignificantDigits()
    {
      if( m_iDecimalPointPos >= 0 )
      {
        for( int i = Count - 1; i > m_iDecimalPointPos; i-- )
        {
          DigitToken digit = m_arrTokens[ i ] as DigitToken;

          if( digit != null )
          {
            InsignificantDigitToken insignificantDigit = digit as InsignificantDigitToken;

            if( insignificantDigit != null )
            {
              insignificantDigit.HideIfZero = true;
            }
            else
            {
              break;
            }
          }
        }
      }
    }
    /// <summary>
    /// Searches for corresponding hour token.
    /// </summary>
    /// <param name="index">Start index to search.</param>
    /// <returns>Corresponding hour token.</returns>
    public HourToken FindCorrespondingHourSection( int index )
    {
      int i = index;

      do
      {
        i--;
        if( i < 0 ) i += Count;

        FormatTokenBase token = this[ i ];

        if( token.TokenType == TokenType.Hour )
        {
          return ( HourToken )token;
        }
      }
      while( i != index );

      return null;
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( double value )
    {
      return ApplyFormat( value, false );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( string value )
    {
      return ApplyFormat( value, false );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( double value, bool bShowReservedSymbols )
    {
      PrepareFormat();
      PrepareValue( ref value, bShowReservedSymbols );

      if( m_bUseSystemDateFormat )
        return 
#if ( WINRT )
            DateTimeExtension.FromOADate(value).ToString(CultureInfo.InvariantCulture);
#else
            DateTime.FromOADate( value ).ToLongDateString();
#endif

      int iCount = Count;

      double dFractionValue = 0;
      double dLog10 = value != 0 ? Math.Floor(Math.Log10(Math.Abs(value))) : value;
      double dWithoutE = value / Math.Pow( 10, dLog10 );

      if( m_iScientificPos > 0 )
      {
        int iPower = m_iNumberOfIntegerDigits - 1;
        dWithoutE *= Math.Pow( 10, iPower );
        dLog10 -= iPower;
        value = dWithoutE;
      }

      bool bAddNegative = value < 0;

      if( m_formatType == ExcelFormatType.Number && !m_bFraction )//m_iDecimalPointPos > 0 )
      {
        value = SplitValue( value, out dFractionValue );

        double dFractionSize = Math.Pow( 10, m_iNumberOfFractionDigits );
        dFractionValue *= dFractionSize;
        dFractionValue = Round( dFractionValue );

        if( dFractionValue >= dFractionSize )
        {
          dFractionValue -= dFractionSize;
          if( bAddNegative )
          {
            value--;
          }
          else
          {
            value++;
          }
        }
      }

      if( value == 0 )
        bAddNegative &= dFractionValue > 0;

      string strResult = ApplyFormat( value, bShowReservedSymbols, 0, m_iIntegerEnd, false, m_bGroupDigits, bAddNegative );

      if( m_iDecimalPointPos > 0 )
      {
        strResult += ApplyFormat( dFractionValue, bShowReservedSymbols,
          m_iDecimalPointPos, m_iDecimalEnd, false );
      }

      if( m_iScientificPos > 0 )
      {
        strResult += ApplyFormat( dLog10, bShowReservedSymbols, m_iDecimalEnd + 1, iCount - 1, false, false, dLog10 < 0 );
      }
      else if( m_bFraction )
      {
        dFractionValue = value;

        if( IsAnyDigit( 0, m_iIntegerEnd ) )
        {
          dFractionValue -= ( value > 0 ) ? Math.Floor( value ) : Math.Ceiling( value );
          dFractionValue = Math.Abs( dFractionValue );
        }

        if (dFractionValue != 0)
        {
            Fraction fraction = Fraction.ConvertToFraction(dFractionValue, m_iDenumaratorLen);
            long lNumerator = (long)fraction.Numerator;
            long lDenumerator = (long)fraction.Denumerator;

            strResult += ApplyFormat(lNumerator, bShowReservedSymbols, m_iIntegerEnd + 1, m_iFractionPos, false);
            strResult += ApplyFormat(lDenumerator, bShowReservedSymbols, m_iFractionPos + 1, m_iFractionEnd, false);
            strResult += ApplyFormat(0, bShowReservedSymbols, m_iFractionEnd + 1, iCount - 1, false);
        }
      }

      return strResult;
      // TODO: finish format apply.
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( string value, bool bShowReservedSymbols )
    {
      PrepareFormat();

      if( m_formatType != ExcelFormatType.Text && m_formatType != ExcelFormatType.General || m_bUseSystemDateFormat )
      {
        return value;
      }

      int iCount = m_arrTokens.Count;
      string strResult = string.Empty;
      FormatTokenBase token;

      if( iCount > 1 )
      {
        StringBuilder builder = new StringBuilder();

        for( int i = iCount - 1; i >= 0; i-- )
        {
          token = m_arrTokens[ i ];
          string strTokenResult = token.ApplyFormat( value, bShowReservedSymbols );
          builder.Insert( 0, strTokenResult );
        }

        strResult = builder.ToString();
      }
      else if( iCount == 1 )
      {
        token = m_arrTokens[ 0 ];
        strResult = token.ApplyFormat( value, bShowReservedSymbols );
      }

      return strResult;
    }

    /// <summary>
    /// Assigns position to the variable and checks if it wasn't assigned
    ///  before (throws ForamtException if it was).
    /// </summary>
    /// <param name="iToAssign">Variable to assign.</param>
    /// <param name="iCurrentPos">Current position.</param>
    private void AssignPosition( ref int iToAssign, int iCurrentPos )
    {
      if( iToAssign >= 0 )
        throw new FormatException();

      iToAssign = iCurrentPos;
    }
    /// <summary>
    /// Applies part of the format tokens to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <param name="iStartToken">Start format token.</param>
    /// <param name="iEndToken">End format token.</param>
    /// <param name="bForward">Indicates whether token index should be increased after each step.</param>
    /// <returns>String representation of the value.</returns>
    private string ApplyFormat( double value, bool bShowReservedSymbols,
      int iStartToken, int iEndToken, bool bForward )
    {
      return ApplyFormat( value, bShowReservedSymbols, iStartToken, iEndToken, bForward, false, false );
    }
    /// <summary>
    /// Applies part of the format tokens to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <param name="iStartToken">Start format token.</param>
    /// <param name="iEndToken">End format token.</param>
    /// <param name="bForward">Indicates whether token index should be increased after each step.</param>
    /// <param name="bGroupDigits">Indicates whether to group digit tokens.</param>
    /// <param name="bAddNegativeSign">Indicates whether we should add negative sign before first digit token.</param>
    /// <returns>String representation of the value.</returns>
    private string ApplyFormat( double value, bool bShowReservedSymbols,
      int iStartToken, int iEndToken, bool bForward, bool bGroupDigits, bool bAddNegativeSign )
    {
      StringBuilder builder = new StringBuilder();
      int iDelta = bForward ? 1 : -1;
      int iStart = bForward ? iStartToken : iEndToken;
      int iEnd = bForward ? iEndToken : iStartToken;
      int iDigitCounter = 0;
      CultureInfo culture = Culture;
      int iLastDigit = -1;
      double originalValue = value;

      for( int i = iStart; CheckCondition( iEnd, bForward, i ); i += iDelta )
      {
        FormatTokenBase token = m_arrTokens[ i ];
        DigitToken digit = token as DigitToken;

        if( digit != null )
        {
          digit.OriginalValue = originalValue;
          iDigitCounter = ApplyDigit( digit, i, iStart, ref value, iDigitCounter,
            builder, bForward, bShowReservedSymbols, bGroupDigits );

          if( bForward )
          {
            if( bAddNegativeSign )
            {
              AddToBuilder( builder, bForward, "-" );
              bAddNegativeSign = false;
            }
          }
          else
          {
            iLastDigit = builder.Length;
          }
        }
        else
        {
          double tempValue = originalValue;
          string strTokenResult = token.ApplyFormat( ref tempValue, bShowReservedSymbols, culture, this );
          if (strTokenResult != null)
              AddToBuilder( builder, bForward, strTokenResult );
        }
      }

      if( iLastDigit >= 0 && bAddNegativeSign )
      {
        //AddToBuilder( builder, bForward, "-" );
        builder.Insert( builder.Length - iLastDigit, "-" );
      }

      return builder.ToString();
    }
    /// <summary>
    /// Applies digit token to the value.
    /// </summary>
    /// <param name="digit">Represents a digit token to apply.</param>
    /// <param name="iIndex">Digit index.</param>
    /// <param name="iStart">Represents Start index of the range.</param>
    /// <param name="value">Represents Value to apply digit token to.</param>
    /// <param name="iDigitCounter">Represents digit counter.</param>
    /// <param name="builder">Represents the StringBuilder to add value to.</param>
    /// <param name="bForward">Boolean value indicating direction of the apply cycle.</param>
    /// <param name="bShowHiddenSymbols">Boolean value indicating whether we should show hidden symbols.</param>
    /// <param name="bGroupDigits">Boolean value indicating whether digits are grouped.</param>
    /// <returns>Value of the digit count.</returns>
    private int ApplyDigit( DigitToken digit, int iIndex, int iStart, ref double value,
      int iDigitCounter, StringBuilder builder, bool bForward, bool bShowHiddenSymbols, bool bGroupDigits )
    {
      if( digit == null )
        throw new ArgumentNullException( "digit" );

      string strTokenResult;
      CultureInfo culture = Culture;

      if( digit.IsLastDigit )
      {
        bool bMinus = ( value < 0 );
        value = Math.Abs( value );

        do
        {
//          int iDigit = digit.GetDigit( ref value );
//          strDigit = digit.GetDigitString( iDigit, bShowHiddenSymbols );
//          localBuilder.Insert( 0, strDigit );
          strTokenResult = digit.ApplyFormat( ref value, bShowHiddenSymbols, culture, this );
          iDigitCounter = ApplySingleDigit( iIndex, iStart, iDigitCounter,
            strTokenResult, builder, bForward, bGroupDigits );
        }
        while( value >= 1 );

        if( bMinus )
        {
          //AddToBuilder( builder, bForward, DEF_MINUS );
          value = -value;
        }
      }
      else
      {
        strTokenResult = digit.ApplyFormat( ref value, bShowHiddenSymbols, culture, this );
        iDigitCounter = ApplySingleDigit( iIndex, iStart, iDigitCounter,
          strTokenResult, builder, bForward, bGroupDigits );
      }

      return iDigitCounter;
    }
    /// <summary>
    /// Applies digit token to the value.
    /// </summary>
    /// <param name="iIndex">Digit index.</param>
    /// <param name="iStart">Represents start index.</param>
    /// <param name="iDigitCounter">Represents digit counter.</param>
    /// <param name="strTokenResult">Represents result of token string.</param>
    /// <param name="builder">Represents the StringBuilder to add value to.</param>
    /// <param name="bForward">Boolean value indicating direction of the apply cycle.</param>
    /// <param name="bGroupDigits">Boolean value indicating whether digits are grouped.</param>
    /// <returns>Number of digits.</returns>
    private int ApplySingleDigit( int iIndex, int iStart, int iDigitCounter,
      string strTokenResult, StringBuilder builder, bool bForward, bool bGroupDigits )
    {
      if( strTokenResult == null )
        throw new ArgumentNullException( "strTokenResult" );

      int iDelta = bForward ? 1 : -1;
      iDigitCounter++;

      if( bGroupDigits && strTokenResult.Length > 0 && iIndex != iStart && iDigitCounter == 4 )
      {
          AddToBuilder(builder, bForward, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);// DEF_THOUSAND_SEPARATOR );
        iDigitCounter = 1;
      }

      AddToBuilder( builder, bForward, strTokenResult );
      return iDigitCounter;
    }
    /// <summary>
    /// Adds value to a StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to add value to.</param>
    /// <param name="bForward">Indicates whether we should call append method or insert.</param>
    /// <param name="strValue">Value to add.</param>
    private void AddToBuilder( StringBuilder builder, bool bForward, string strValue )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );

      if( strValue == null )
        throw new ArgumentNullException( "strValue" );

      if( bForward )
      {
        builder.Append( strValue );
      }
      else
      {
        builder.Insert( 0, strValue );
      }
    }
    /// <summary>
    /// Checks whether iPos is inside range of correct values.
    /// </summary>
    /// <param name="iEndToken">End token index.</param>
    /// <param name="bForward">Indicates whether position is increasing each cycle.</param>
    /// <param name="iPos">Current position.</param>
    /// <returns>Value indicating whether iPos in inside of range of valid values.</returns>
    private bool CheckCondition( int iEndToken, bool bForward, int iPos )
    {
      return bForward ? iPos <= iEndToken : iPos >= iEndToken;
    }
    /// <summary>
    /// Locates last digit of the decimal fraction.
    /// </summary>
    /// <returns>Value of last decimal digit.</returns>
    private int LocateLastFractionDigit()
    {
      int iResult = -1;
      int iCount = Count;

      //if( m_iDecimalPointPos > 0 )
      {
        int iStart = ( m_iScientificPos > 0 ) ? m_iScientificPos - 1 : iCount - 1;

        for( int i = iStart; i >= 0;/*m_iDecimalPointPos;*/ i-- )
        {
          FormatTokenBase token = this[ i ];
          DigitToken digit = token as DigitToken;

          if( digit != null )
          {
            if( m_iDecimalPointPos < i )
            {
              //digit.IsLastDigit = true;
            }

            iResult = i;
            break;
          }
        }
      }

      if( m_iScientificPos > 0 )
      {
        for( int i = m_iScientificPos; i < iCount; i++ )
        {
          DigitToken token = this[ i ] as DigitToken;

          if( token != null )
          {
            token.IsLastDigit = true;
            break;
          }
        }
      }

      return iResult;
    }
    /// <summary>
    /// Locates last group symbols.
    /// </summary>
    /// <param name="iStartIndex">Start index to search.</param>
    /// <returns>Value indicating whether groups are present after last decimal digit.</returns>
    private bool LocateLastGroups( int iStartIndex )
    {
      iStartIndex = Math.Max( m_iDecimalPointPos, iStartIndex );
      iStartIndex = Math.Max( 0, iStartIndex );

      int iCount = Count;

      if( iStartIndex >= iCount )
      {
        return false;
      }

      ThousandsSeparatorToken token = m_arrTokens[ iStartIndex ] as ThousandsSeparatorToken;
      bool bResult = ( token != null );

      while( token != null )
      {
        token.IsAfterDigits = true;
        iStartIndex++;

        if( iStartIndex >= iCount ) break;

        token = m_arrTokens[ iStartIndex ] as ThousandsSeparatorToken;
      }

      return bResult;
    }
    /// <summary>
    /// Applies last groups tokens.
    /// </summary>
    /// <param name="value">Value to apply to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show hidden symbols (reserved place).</param>
    private void ApplyLastGroups( ref double value, bool bShowReservedSymbols )
    {
      if( !m_bLastGroups ) return;
      int iCurIndex = m_iLastDigit + 1;

      int iCount = Count;

      while( iCurIndex < iCount )
      {
        ThousandsSeparatorToken token = m_arrTokens[ iCurIndex ] as ThousandsSeparatorToken;

        if( token == null ) break;

        value = token.PreprocessValue( value );
        iCurIndex++;
      }
      
    }

    /// <summary>
    /// Prepares value for format application.
    /// </summary>
    /// <param name="value">Value to apply to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show hidden symbols (reserved place).</param>
    private void PrepareValue( ref double value, bool bShowReservedSymbols )
    {
      ApplyLastGroups( ref value, bShowReservedSymbols );

      for( int i = 0, len = Count; i < len; i++ )
      {
        FormatTokenBase token = this[ i ];

        if( token.TokenType == TokenType.Percent )
        {
          value *= 100;
        }
      }
    }
    /// <summary>
    /// Calculates number of fraction digits.
    /// </summary>
    /// <returns>Number of fraction digits.</returns>
    private int CalculateFractionDigits()
    {
      int iResult = 0;

      if( m_iDecimalPointPos > 0 )
      {
        return CalculateDigits( m_iDecimalPointPos, m_iLastDigit );
      }

      return iResult;
    }
    /// <summary>
    /// Calculates number of digits in the integer part of format string.
    /// </summary>
    /// <returns>Number of digits in the integer part of format string.</returns>
    private int CalculateIntegerDigits()
    {
      int iEndIndex = Count - 1;

      if( m_iDecimalPointPos > 0 )
      {
        iEndIndex = m_iDecimalPointPos;
      }
      else if( m_iScientificPos > 0 )
      {
        iEndIndex = m_iScientificPos;
      }

      return CalculateDigits( 0, iEndIndex );
    }
    /// <summary>
    /// Calculates number of digits in the specified range.
    /// </summary>
    /// <param name="iStartIndex">Start index of the range.</param>
    /// <param name="iEndIndex">End index of the range.</param>
    /// <returns>Number of digits in the specified range.</returns>
    private int CalculateDigits( int iStartIndex, int iEndIndex )
    {
      int iCount = Count;

      if( iStartIndex < 0 || iStartIndex > iCount )
        throw new ArgumentOutOfRangeException( "iStartIndex", "Value cannot be less than 0 and greater than iCount." );

      if( iEndIndex < 0 || iEndIndex > iCount )
        throw new ArgumentOutOfRangeException( "iEndIndex", "Value cannot be less than 0 and greater than iCount." );

      int iResult = 0;

      for( int i = iStartIndex; i <= iEndIndex; i++ )
      {
        if( m_arrTokens[ i ] is DigitToken )
        {
          iResult++;
        }
      }

      return iResult;
    }
    /// <summary>
    /// Searches for block of digits that correspond to the fraction.
    /// </summary>
    private void LocateFractionParts()
    {
      if( !m_bFraction ) return;

      // TODO: optimize len evaluation.
      m_iFractionStart = GetDigitGroupStart( m_iFractionPos, false );
      m_iFractionEnd = GetDigitGroupStart( m_iFractionPos, true );
      m_iDenumaratorLen = m_iFractionEnd - GetDigitGroupStart( m_iFractionEnd, false ) + 1;

      if( FormatType != ExcelFormatType.Number ) return;

      if( m_iFractionStart >= 0 )
      {
        DigitToken token = ( DigitToken )m_arrTokens[ m_iFractionStart ];
        token.IsLastDigit = true;
      }
      else
      {
        throw new ArgumentException( "Can't locate fraction digits" );
      }
    }
    /// <summary>
    /// Searches for start of the group of digits.
    /// </summary>
    /// <param name="iStartPos">Start position to search.</param>
    /// <param name="bForward">Direction of the search: true - from left to right, false - from right to left.</param>
    /// <returns>Position of the start of the group.</returns>
    private int GetDigitGroupStart( int iStartPos, bool bForward )
    {
      int iDelta = bForward ? 1 : -1;
      int i = iStartPos;
      int iCount = Count;
      bool bDigitFound = false;

      for( ; i >= 0 && i < iCount; i += iDelta )
      {
        if( m_arrTokens[ i ] is DigitToken )
        {
          bDigitFound = true;
          break;
        }
      }

      if( !bDigitFound ) return -1;

      i += iDelta;

      for( ;i >= 0 && i < iCount && m_arrTokens[ i ] is DigitToken; i += iDelta )
        ;

      return i - iDelta;
    }
    /// <summary>
    /// Indicates whether range contains any digits.
    /// </summary>
    /// <param name="iStartIndex">Start range index.</param>
    /// <param name="iEndIndex">End range index.</param>
    /// <returns>Value indicating whether range contains any digits.</returns>
    private bool IsAnyDigit( int iStartIndex, int iEndIndex )
    {
      int iCount = Count;

      iStartIndex = Math.Max( iStartIndex, 0 );
      iEndIndex = Math.Min( iEndIndex, iCount - 1 );

      for( int i = iStartIndex; i < iEndIndex; i++ )
      {
        if( m_arrTokens[ i ] is DigitToken )
        {
          return true;
        }
      }

      return false;
    }
    /// <summary>
    /// Checks whether value value meets the condition.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if value meets the condition; otherwise returns False.</returns>
    public bool CheckCondition( double value )
    {
      if( HasCondition )
      {
        return m_condition.CheckCondition( value );
      }

      return false;
    }
    /// <summary>
    /// Tries to detect format type.
    /// </summary>
    private void DetectFormatType()
    {
      m_formatType = ExcelFormatType.Unknown;

      for( int i = 0, len = DEF_POSSIBLE_TOKENS.Length; i < len; i += 2 )
      {
        TokenType[] arrPossibleTokens = ( TokenType[] )DEF_POSSIBLE_TOKENS[ i ];
        ExcelFormatType formatType = ( ExcelFormatType )DEF_POSSIBLE_TOKENS[ i + 1 ];

        if( formatType == ExcelFormatType.Number && m_bMultiplePoints )
        {
          continue;
        }

        if( CheckTokenTypes( arrPossibleTokens ) )
        {
          m_formatType = formatType;
          break;
        }
      }
    }
    /// <summary>
    /// Checks whether section contains only specified token types.
    /// </summary>
    /// <param name="arrPossibleTokens">Array with possible tokens.</param>
    /// <returns>Value indicating whether the section contains only specified token types.</returns>
    private bool CheckTokenTypes( TokenType[] arrPossibleTokens )
    {
      if( arrPossibleTokens == null )
        throw new ArgumentNullException( "arrPossibleTokens" );

      int iCount = Count;

      if( iCount == 0 && arrPossibleTokens.Length == 0 ) return true;

      for( int i = 0, len = iCount; i < len; i++ )
      {
        FormatTokenBase token = this[ i ];

        if( !ContainsIn( arrPossibleTokens, token.TokenType ) )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Check whether this token is really minute token and substitutes it by Month if necessary.
    /// </summary>
    /// <param name="iTokenIndex">Token index to check.</param>
    private void CheckMinuteToken( int iTokenIndex )
    {
      // Here we should check whether this token is really minute token
      // or it is month token. It can be minute token if it has hour
      // section before it or second section after it.
      FormatTokenBase token = this[ iTokenIndex ];

      if( token.TokenType != TokenType.Minute )
        throw new ArgumentException( "Wrong token type." );

      bool bMinute = ( FindTimeToken( iTokenIndex - 1, DEF_BREAK_HOUR, false, TokenType.Hour, TokenType.Hour24 ) != -1 )
        || ( FindTimeToken( iTokenIndex + 1, DEF_BREAK_SECOND, true, TokenType.Second, TokenType.SecondTotal ) != -1 );

      if( !bMinute )
      {
        MonthToken month = new MonthToken();
        month.Format = token.Format;
        m_arrTokens[ iTokenIndex ] = month;
      }
    }
    /// <summary>
    /// Searches for required time token.
    /// </summary>
    /// <param name="iTokenIndex">Start token index.</param>
    /// <param name="arrBreakTypes">Types of tokens that could stop search process.</param>
    /// <param name="bForward">Indicates whether we have to increase token index on each iteration.</param>
    /// <param name="arrTypes">Types of token to search.</param>
    /// <returns>Index of the found token or -1 if token was not found.</returns>
    private int FindTimeToken( int iTokenIndex, TokenType[] arrBreakTypes, bool bForward, params TokenType[] arrTypes )
    {
      int iCount = Count;
      int iDelta = bForward ? 1 : -1;

      while( iTokenIndex >= 0 && iTokenIndex < iCount )
      {
        FormatTokenBase token = this[ iTokenIndex ];
        TokenType tokenType = token.TokenType;

        if( Array.IndexOf( arrBreakTypes, tokenType ) != -1 ) break;

        if( Array.IndexOf( arrTypes, tokenType ) != -1 ) return iTokenIndex;

        iTokenIndex += iDelta;
      }

      return DEF_NOTFOUND_INDEX;
    }

    /// <summary>
    /// Sets to all second tokens.
    /// </summary>
    private void SetRoundSeconds()
    {
      bool bRound = true;
      int iCount = Count;

      for( int i = 0; i < iCount; i++ )
      {
        FormatTokenBase token = this[ i ];

        if( token.TokenType == TokenType.DecimalPoint )
        {
          int iStartIndex = i;
          string strFormat = string.Empty;

          i++;
          while( i < iCount && ( Array.IndexOf( DEF_MILLISECONDTOKENS,
            this[ i ].TokenType ) != -1 ) )
          {
            strFormat += this[ i ].Format;
            i++;
          }

          if( i != iStartIndex + 1 )
          {
            MilliSecondToken milli = new MilliSecondToken();
            milli.Format = strFormat;
            int iRemoveCount = i - iStartIndex;
            m_arrTokens.RemoveRange( iStartIndex, iRemoveCount );
            m_arrTokens.Insert( iStartIndex, milli );
            iCount -= iRemoveCount - 1;
            bRound = false;
          }
        }
      }

      if( bRound ) return;

      for( int i = 0; i < iCount; i++ )
      {
        FormatTokenBase token = this[ i ];

        if( token.TokenType == TokenType.Second )
        {
          ( ( SecondToken )token ).RoundValue = false;
        }
      }
    }
    public bool IsTimeFormat
    {
      get
      {
        if( FormatType != ExcelFormatType.DateTime )
          return false;

        bool result = true;

        foreach( FormatTokenBase tokenBase in m_arrTokens )
        {
          if( Array.IndexOf( NotTimeTokens, tokenBase.TokenType ) >= 0 )
          {
            result = false;
            break;
          }
        }

        return result;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single token from the section. Read-only.
    /// </summary>
    public FormatTokenBase this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count - 1 )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1." );

        return ( FormatTokenBase )m_arrTokens[ index ];
      }
    }

    /// <summary>
    /// Gets the number of tokens in the section.
    /// </summary>
    public int Count
    {
      get
      {
        return m_arrTokens.Count;
      }
    }
    /// <summary>
    /// Gets a value indicating whether section contains condition. Read-only.
    /// </summary>
    public bool HasCondition
    {
      get
      {
        return m_condition != null;
      }
    }
    /// <summary>
    /// Gets the section type.
    /// </summary>
    public ExcelFormatType FormatType
    {
      get
      {
        if( m_formatType == ExcelFormatType.Unknown )
        {
          DetectFormatType();
        }

        return m_formatType;
      }
    }
    /// <summary>
    /// Gets the culture used for conversion. Read-only.
    /// </summary>
    public CultureInfo Culture
    {
      get
      {
        return ( m_culture != null )
          ? m_culture.Culture
          : CultureInfo.CurrentCulture;
      }
    }

    /// <summary>
    /// Gets a value indicating whether number format contains fraction sign. Read-only.
    /// </summary>
    public bool IsFraction
    {
      get
      {
        return m_bFraction;
      }
    }
    /// <summary>
    /// Gets a value indicating whether section contains E/E+ or E- signs in format string.
    /// </summary>
    public bool IsScientific
    {
      get
      {
        return m_iScientificPos >= 0;
      }
    }
    /// <summary>
    /// Gets a value indicating whether thousand separator is present in number format. Read-only.
    /// </summary>
    public bool IsThousandSeparator
    {
      get
      {
        return m_bGroupDigits;
      }
    }
    /// <summary>
    /// Gets the number of digits after "." sign. Read-only.
    /// </summary>
    public int DecimalNumber
    {
      get
      {
        return m_iNumberOfFractionDigits;
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Indicates whether type of specified token is in the array of tokens.
    /// </summary>
    /// <param name="arrPossibleTokens">Array of tokens to check.</param>
    /// <param name="token">Token type to locate.</param>
    /// <returns>True if token type is in the array of possible tokens.</returns>
    private static bool ContainsIn( TokenType[] arrPossibleTokens, TokenType token )
    {
      if( arrPossibleTokens == null )
        throw new ArgumentNullException( "arrPossibleTokens" );

      // Here we are implementing our version of binary search,
      // instead of standard, to increase performance.
      int iFirstIndex = 0;
      int iLastIndex = arrPossibleTokens.Length - 1;

      while( iLastIndex != iFirstIndex )
      {
        int iMiddleIndex = ( iLastIndex + iFirstIndex ) / 2;
        TokenType curToken = arrPossibleTokens[ iMiddleIndex ];

        if( curToken >= token )
        {
          if( iLastIndex == iMiddleIndex ) break;

          iLastIndex = iMiddleIndex;
        }
        else if( curToken < token )
        {
          if( iFirstIndex == iMiddleIndex ) break;

          iFirstIndex = iMiddleIndex;
        }
      }

      return ( arrPossibleTokens[ iFirstIndex ] == token
        || arrPossibleTokens[ iLastIndex ] == token );
    }
    /// <summary>
    /// Splits value into integer and decimal parts.
    /// </summary>
    /// <param name="value">Value to split.</param>
    /// <param name="dFraction">Return fraction value.</param>
    /// <returns>Integer value.</returns>
    private static double SplitValue( double value, out double dFraction )
    {
      bool bPositive = value > 0;
      int orginalLength = value.ToString().Length;
      dFraction = Math.Abs( value - ( bPositive ? Math.Floor( value ) : Math.Ceiling( value ) ) );
      double dResult = Math.Abs( value ) - dFraction;
      int fractionLength = orginalLength - dResult.ToString().Length + 1;
      if (fractionLength < orginalLength && fractionLength<15)
          dFraction = Math.Round(dFraction, fractionLength);
      return bPositive ? dResult : -dResult;
    }
    /// <summary>
    /// Rounds value.
    /// </summary>
    /// <param name="value">Represents value to be rounded.</param>
    /// <returns>Rounded Value.</returns>
    internal static double Round( double value )
    {
      bool bLargerThanZero = value >= 0;
      double dIntPart = ( bLargerThanZero )
        ? Math.Floor( value )
        : Math.Ceiling( value );

      int iSign = Math.Sign( value );
      double dFloatPart = ( bLargerThanZero )
        ? value - dIntPart
        : dIntPart - value;

      if( dFloatPart >= 0.49999999999995 ) dIntPart += iSign;

      return dIntPart;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Represents parent object.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      FormatSection result = ( FormatSection )MemberwiseClone();
      result.SetParent( parent );
      result.m_arrTokens = new List<FormatTokenBase>( m_arrTokens.Count );

      for( int i = 0, len = m_arrTokens.Count; i < len; i++ )
      {
        FormatTokenBase token = ( FormatTokenBase )m_arrTokens[ i ];
        token = ( FormatTokenBase )token.Clone();
        result.m_arrTokens.Add( token );

        switch( token.TokenType )
        {
          case TokenType.Condition:
            result.m_condition = ( ConditionToken )token;
            break;

          case TokenType.Culture:
            result.m_culture = ( CultureToken )token;
            break;
        }
      }

      return result;
    }

    #endregion

    internal void Clear()
    {
        m_arrTokens.Clear();
        m_arrTokens = null;
        this.Dispose();
    }
  }
}
