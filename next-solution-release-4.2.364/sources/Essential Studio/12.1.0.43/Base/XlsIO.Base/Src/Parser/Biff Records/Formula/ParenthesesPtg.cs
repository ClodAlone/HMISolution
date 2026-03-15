#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// Parentheses. This token is for display purposes only, it does not affect the result
  /// of the token array. If it follows an operator, the parentheses will enclose
  /// the operator and its operand(s), which is the result of the enclosed
  /// operation. This operator does not modify the token class of its operand.
  /// </summary>
  [ Token ( FormulaToken.tParentheses, "(" ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ParenthesesPtg : UnaryOperationPtg
  {
    #region Class static constants
    /// <summary>
    /// Contains all token attributes.
    /// </summary>
    private static readonly TokenAttribute[] s_arrAttributes;
    #endregion

    #region Class constructors
    /// <summary>
    /// Static constructor.
    /// </summary>
    static ParenthesesPtg()
    {
      //Type curType = typeof( ParenthesesPtg );

      //s_arrAttributes = ( TokenAttribute[] )curType.GetCustomAttributes(
      //  typeof( TokenAttribute ), false );
      s_arrAttributes = new TokenAttribute[]
      {
        new TokenAttribute( FormulaToken.tParentheses, "(" ),
      };
    }
    /// <summary>
    /// Default constructor
    /// </summary>
    public ParenthesesPtg() : base( "(" )
    {
      this.TokenCode = FormulaToken.tParentheses;
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public ParenthesesPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Creates token by string representation.
    /// </summary>
    /// <param name="strFormula">String should be equal to "(".</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When strFormula is not equal to "(".
    /// </exception>
    public ParenthesesPtg( string strFormula ) : base( "(" )
    {
      if( strFormula != "(" )
        throw new ArgumentOutOfRangeException( "strFormula" );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
      return "()";
    }
    /// <summary>
    /// Gets all needed operands from the Stack parameter and pushes result into the Stack.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and will receive operation result as string.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      object operand = operands.Pop();
      object spaces = operand as AttrPtg;

      if( spaces != null )
      {
        operand = operands.Pop();
      }
      else
      {
        spaces = string.Empty;
      }


      string strResult = spaces.ToString() + "(" + operand.ToString() + ")";

      operands.Push( strResult );
    }

    /// <summary>
    ///  Returns array of string where each member corresponds to one argument.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index of opening bracket.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings that contain operation parameters.</returns>
    public override string[] GetOperands( string strFormula, ref int index,
      FormulaUtil formulaParser )
    {
      int BracketPair = FormulaUtil.FindCorrespondingBracket( strFormula, index );
      index = BracketPair + 1;

      string operand = strFormula.Substring( 1, BracketPair - 1 );

      return new string[]{ operand };
    }

    /// <summary>
    /// Updates parse formula options if necessary.
    /// </summary>
    /// <param name="options">Options to update.</param>
    /// <returns>Updated value.</returns>
    public override ExcelParseFormulaOptions UpdateParseOptions(ExcelParseFormulaOptions options)
    {
      return options;
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns all TokenAttributes applied to the token. Read-only.
    /// </summary>
    protected override TokenAttribute[] Attributes
    {
      get
      {
        return s_arrAttributes;
      }
    }

    #endregion
  }
}
