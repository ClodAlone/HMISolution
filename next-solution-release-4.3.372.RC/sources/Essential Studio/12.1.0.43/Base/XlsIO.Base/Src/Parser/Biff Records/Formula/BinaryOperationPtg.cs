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

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  ///<exclude/>
  /// <summary>
  /// This class represents every binary operation.
  /// </summary>
  [ Token( FormulaToken.tAdd,                    "+" ) ]
  [ Token( FormulaToken.tDiv,                    "/" ) ]
  [ Token( FormulaToken.tMul,                    "*" ) ]
  [ Token( FormulaToken.tSub,                    "-" ) ]
  [ Token( FormulaToken.tPower,                  "^" ) ]
  [ Token( FormulaToken.tConcat,                 "&" ) ]
  [ Token( FormulaToken.tLessThan,               "<" ) ]
  [ Token( FormulaToken.tLessEqual,              "<=" ) ]
  [ Token( FormulaToken.tEqual,                  "=" ) ]
  [ Token( FormulaToken.tNotEqual,               "<>" ) ]
  [ Token( FormulaToken.tGreater,                ">" ) ]
  [ Token( FormulaToken.tGreaterEqual,           ">=" ) ]
  [ Token( FormulaToken.tCellRangeIntersection,  " " ) ]
  [ Token( FormulaToken.tCellRange,              ":" ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class BinaryOperationPtg : OperationPtg
  {
    #region Class static constants
    /// <summary>
    /// Dictionary that allows to convert operation name to the token code.
    /// </summary>
    private static readonly Dictionary<string, FormulaToken> NameToId = new Dictionary<string, FormulaToken>( 16 );
    /// <summary>
    /// Dictionary that allows to convert operation token code to its string representation.
    /// </summary>
    private static readonly Dictionary<FormulaToken, string> IdToName = new Dictionary<FormulaToken, string>( 16 );
    /// <summary>
    /// Contains all token attributes.
    /// </summary>
    private static readonly TokenAttribute[] s_arrAttributes;
    #endregion

    #region Class static constructor
    /// <summary>
    /// Static constructor. Fills hashtable that allows us to get
    /// token code by token string.
    /// </summary>
    static BinaryOperationPtg()
    {
      //Type curType = typeof( BinaryOperationPtg );
      //s_arrAttributes = ( TokenAttribute[] )curType.GetCustomAttributes(
      //  typeof( TokenAttribute ), false );

      s_arrAttributes = new TokenAttribute[]
      {
        new TokenAttribute( FormulaToken.tAdd,                    "+" ),
        new TokenAttribute( FormulaToken.tDiv,                    "/" ),
        new TokenAttribute( FormulaToken.tMul,                    "*" ),
        new TokenAttribute( FormulaToken.tSub,                    "-" ),
        new TokenAttribute( FormulaToken.tPower,                  "^" ),
        new TokenAttribute( FormulaToken.tConcat,                 "&" ),
        new TokenAttribute( FormulaToken.tLessThan,               "<" ),
        new TokenAttribute( FormulaToken.tLessEqual,              "<=" ),
        new TokenAttribute( FormulaToken.tEqual,                  "=" ),
        new TokenAttribute( FormulaToken.tNotEqual,               "<>" ),
        new TokenAttribute( FormulaToken.tGreater,                ">" ),
        new TokenAttribute( FormulaToken.tGreaterEqual,           ">=" ),
        new TokenAttribute( FormulaToken.tCellRangeIntersection,  " " ),
        new TokenAttribute( FormulaToken.tCellRange,              ":" ),
      };

      for( int i = 0, len = s_arrAttributes.Length; i < len; i++ )
      {
        TokenAttribute attribute = s_arrAttributes[ i ];
        FormulaToken token = attribute.FormulaType;
        string strOperation = attribute.OperationSymbol;

        NameToId.Add( strOperation, token );
        IdToName.Add( token, strOperation );
      }
    }
    /// <summary>
    /// Returns token code by string representation of the operation.
    /// </summary>
    /// <param name="operationSign">String representation of the operation.</param>
    /// <returns>Token code.</returns>
    public static FormulaToken GetTokenId( string operationSign )
    {
      if( operationSign == null )
        throw new ArgumentNullException( "operationSign" );

      if( operationSign.Length == 0 )
        throw new ArgumentException( "operationSign - string cannot be empty" );

      FormulaToken tokenId = NameToId[ operationSign ];
      return tokenId;
    }
    /// <summary>
    /// Returns token code by string representation of the operation.
    /// </summary>
    /// <param name="token">Token to get string representation for.</param>
    /// <returns>String representation of the token.</returns>
    public static string GetTokenString( FormulaToken token )
    {
      string strTokenValue = IdToName[ token ];
      return strTokenValue;
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BinaryOperationPtg()
    {
    }
    /// <summary>
    /// Constructs BinaryOperation using string that contains the sign of operation.
    /// </summary>
    /// <param name="operation">String representation of the operation.</param>
    public BinaryOperationPtg( string operation )
    {
      if( operation == null )
        throw new ArgumentNullException( "operation" );

      if( operation.Length == 0 )
        throw new ArgumentException( "operation - string cannot be empty" );

      if( !NameToId.ContainsKey( operation ) )
        throw new ArgumentException( "operation", "Unknown operation symbol" );

      OperationSymbol = operation;
      TokenCode = GetTokenId( operation );
    }
    /// <summary>
    /// Constructs BinaryOperation using string that contains the sign of operation.
    /// </summary>
    /// <param name="operation">Token code of the operation to create.</param>
    public BinaryOperationPtg( FormulaToken operation )
    {
      if( !IdToName.ContainsKey( operation ) )
        throw new ArgumentException( "operation", "Unknown operation symbol" );

      OperationSymbol = GetTokenString( operation );
      TokenCode = operation;
    }
    /// <summary>
    /// Constructs token using array with data and offset.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public BinaryOperationPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Number of operands (always 2 for binary operations).
    /// </summary>
    public override int NumberOfOperands
    {
      get
      {
        return 2;
      }
    }
    /// <summary>
    /// Read-only. Type of the operation ( TYPE_BINARY for binary operations).
    /// </summary>
    public override TOperation OperationType
    {
      get
      {
        return TOperation.TYPE_BINARY;
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

    #region Class methods
    /// <summary>
    /// Takes all needed operands from the stack and pushes the result of the operation.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and will receive result of the operation.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      FormulaUtil.PushOperandToStack( operands, ToString( formulaUtil ) );
      string operation = ( string )operands.Pop();
      string operand2 = ( string )operands.Pop();
      string operand1 = ( string )operands.Pop();
      operands.Push( operand1 + operation + operand2 );
    }

    /// <summary>
    /// Gets operands from string and returns array of string representation of the operands.
    /// </summary>
    /// <param name="strFormula">String representation of the formula.</param>
    /// <param name="index">Index of operation in the string.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings with operation operands.</returns>
    public override string[] GetOperands( string strFormula, ref int index,
      FormulaUtil formulaParser )
    {
      index += OperationSymbol.Length;
      
      string operand = formulaParser.GetRightBinaryOperand( strFormula,
        index, ToString() );
      
      index += operand.Length;

      return new string[]{ operand };
    }

    /// <summary>
    /// 
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 1;
    }

    #endregion
  }
}