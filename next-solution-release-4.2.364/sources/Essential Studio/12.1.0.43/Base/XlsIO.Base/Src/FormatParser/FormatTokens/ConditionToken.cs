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
  /// Class used for Condition Tokens.
  /// </summary>
  public class ConditionToken : InBracketToken
  {
    #region Class constants
    /// <summary>
    /// Possible compare operators.
    /// </summary>
    private enum CompareOperation
    {
      None = 0,
      Equal,
      NotEqual,
      GreaterEqual,
      LessEqual,
      Less,
      Greater,
    };
    /// <summary>
    /// All compare operations.
    /// </summary>
    private static readonly string[] CompareOperationStrings = new string[]
    {
      "=",
      "<>",
      ">=",
      "<=",
      "<",
      ">",
    };
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether token should be formatted using am/pm time format.
    /// </summary>
    private double m_dCompareNumber;
    /// <summary>
    /// Applied compare operation.
    /// </summary>
    private CompareOperation m_operation;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the ConditionToken class.
    /// </summary>
    public ConditionToken()
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

      int iOperationIndex = FindString( CompareOperationStrings, strFormat, iIndex, false );

      if( iOperationIndex < 0 ) return iStartIndex;

      string strOperation = CompareOperationStrings[ iOperationIndex ];
      iIndex += strOperation.Length;

      m_operation = ( CompareOperation )( iOperationIndex + 1 );

      string strNumber = strFormat.Substring( iIndex, iEndIndex - iIndex );
      double dResult;

      if( double.TryParse( strNumber, NumberStyles.Any, null, out dResult ) )
      {
        m_dCompareNumber = dResult;
        return iEndIndex + 1;
      }

      return iStartIndex;
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
        return TokenType.Condition;
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Checks value with the condition.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>Value that indicates whether the condition is true for the value.</returns>
    public bool CheckCondition( double value )
    {
      switch( m_operation )
      {
        case CompareOperation.Equal:
          return value == m_dCompareNumber;

        case CompareOperation.Greater:
          return value > m_dCompareNumber;

        case CompareOperation.GreaterEqual:
          return value >= m_dCompareNumber;

        case CompareOperation.Less:
          return value < m_dCompareNumber;

        case CompareOperation.LessEqual:
          return value <= m_dCompareNumber;

        case CompareOperation.NotEqual:
          return value != m_dCompareNumber;

        default:
          throw new ArgumentOutOfRangeException( "Compare operation" );
      }
    }
    #endregion
  }
}
