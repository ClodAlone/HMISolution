#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This class represents all unary operations.
  /// </summary>
  [ Token( FormulaToken.tUnaryMinus, "-" ) ]
  [ Token( FormulaToken.tUnaryPlus,  "+" ) ]
  [ Token( FormulaToken.tPercent,  "%", true ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class UnaryOperationPtg : OperationPtg
  {
    #region Class static constants
    /// <summary>
    /// Contains all token attributes.
    /// </summary>
    private static readonly TokenAttribute[] s_arrAttributes;
    /// <summary>
    /// Dictionary that allows to get token code by unary operation name.
    /// </summary>
    private static readonly Dictionary<string, TokenAttribute> NameToAttribute = new Dictionary<string, TokenAttribute>( 3 );
    #endregion

    #region Class static constructor
    /// <summary>
    /// Static constructor. Fills hashtable that allows us to get
    /// token code by token string.
    /// </summary>
    static UnaryOperationPtg()
    {
      //Type curType = typeof( UnaryOperationPtg );

      //s_arrAttributes = ( TokenAttribute[] )curType.GetCustomAttributes(
      //  typeof( TokenAttribute ), false );
      s_arrAttributes = new TokenAttribute[]
      {
        new TokenAttribute( FormulaToken.tUnaryMinus, "-" ),
        new TokenAttribute( FormulaToken.tUnaryPlus,  "+" ),
        new TokenAttribute( FormulaToken.tPercent,  "%", true ),
      };

      for( int i = 0, len = s_arrAttributes.Length; i < len; i++ )
      {
        TokenAttribute attribute = s_arrAttributes[ i ];
        NameToAttribute.Add( attribute.OperationSymbol, attribute );
      }
    }
    /// <summary>
    /// Gets token code using unary operation string representation.
    /// </summary>
    /// <param name="operationSign">String representation of the unary operation.</param>
    /// <returns>Token code.</returns>
    public static FormulaToken GetTokenId( string operationSign )
    {
      if( operationSign == null )
        throw new ArgumentNullException( "operationSign" );

      if( operationSign.Length == 0 )
        throw new ArgumentException( "operationSign - string cannot be empty" );

      TokenAttribute attribute = NameToAttribute[ operationSign ];
      return attribute.FormulaType;
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public UnaryOperationPtg()
    {
    }
    /// <summary>
    /// Constructs unary operation token by its string representation.
    /// </summary>
    /// <param name="strOperationSymbol">
    /// String representation of the operation that will be created.
    /// </param>
    public UnaryOperationPtg( string strOperationSymbol )
    {
      TokenAttribute curAttribute;

      if( !NameToAttribute.TryGetValue( strOperationSymbol, out curAttribute ) )
      {
        TokenAttribute[] arrAttributes = Attributes;

        if( arrAttributes == null )
          throw new ArgumentNullException( "Unknown operation" );

        int iLength = arrAttributes.Length;
        int i = 0;

        for( ; i < iLength; i++ )
        {
          curAttribute = arrAttributes[ i ];

          if( curAttribute.OperationSymbol == strOperationSymbol )
          {
            break;
          }
        }

        if( i == iLength )
          throw new ArgumentNullException( "Unknown operation." );
      }

      OperationSymbol = strOperationSymbol;
      TokenCode = curAttribute.FormulaType;
      IsPlaceAfter = curAttribute.IsPlaceAfter;
    }
    /// <summary>
    /// Creates unary operation from the data array and offset of the first byte in it.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public UnaryOperationPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Type of operation.
    /// </summary>
    public override TOperation OperationType
    {
      get
      {
        return TOperation.TYPE_UNARY;
      }
    }
    /// <summary>
    /// Array of all token attributes applied to the object.
    /// This property is used to increase performance.
    /// </summary>
    protected override TokenAttribute[] Attributes
    { 
      get
      {
        return s_arrAttributes;
      }
    }  
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 1;
    }

    /// <summary>
    /// Takes all needed operands from the stack and pushes the result of the operation.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and will receive the result of the operation.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      if( operands == null )
        throw new ArgumentNullException( "operands" );

      FormulaUtil.PushOperandToStack( operands, ToString() );
      string operation = ( string )operands.Pop();
      string operand1 = ( string )operands.Pop();

      if( this.IsPlaceAfter )
      {
        operands.Push( operand1 + operation );
      }
      else
        operands.Push( operation + operand1 );
    }

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
      return OperationSymbol;
    }
    /// <summary>
    /// Returns array of operands.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index of unary operation in the formula string.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings that contain unary operation operands.</returns>
    public override string[] GetOperands( string strFormula, ref int index, FormulaUtil formulaParser )
    {
      string[] result = new string[ 1 ];

      result[ 0 ] = formulaParser.GetRightUnaryOperand( strFormula, index );

      index += result[ 0 ].Length + this.ToString().Length;
      return result;
    }

    #endregion
  }
}
