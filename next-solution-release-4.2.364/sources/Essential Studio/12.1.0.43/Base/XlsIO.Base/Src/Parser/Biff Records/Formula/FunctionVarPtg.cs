#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This class represents function with variable arguments number in a formula.
  /// </summary>
  [ Token( FormulaToken.tFunctionVar1 ) ]
  [ Token( FormulaToken.tFunctionVar2 ) ]
  [ Token( FormulaToken.tFunctionVar3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class FunctionVarPtg : FunctionPtg
  {
    #region Class constructors
    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public FunctionVarPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Constructs token by built-in function index.
    /// </summary>
    /// <param name="funcIndex">Built-in function index.</param>
    public FunctionVarPtg( ExcelFunction funcIndex ) : base( funcIndex )
    {
      TokenCode = FormulaToken.tFunctionVar2;
    }
    /// <summary>
    /// Constructs function token by function name.
    /// </summary>
    /// <param name="strFunctionName">Valid function name.</param>
    public FunctionVarPtg( string strFunctionName )
      : base( strFunctionName )
    {
      TokenCode = FormulaToken.tFunctionVar2;
    }
    /// <summary>
    /// Default constructor
    /// </summary>
    public FunctionVarPtg()
    {
      TokenCode = FormulaToken.tFunctionVar2;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return base.GetSize( version ) + 1;
    }
    /// <summary>
    /// Gets operands from string and returns array of string representation of the operands.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index of function in the string.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings containing function parameters.</returns>
    public override string[] GetOperands( string strFormula, ref int index,
      FormulaUtil formulaParser )
    {
      string[] result = GetOperands( strFormula, ref index ,false, formulaParser );

      if( FunctionIndex != ExcelFunction.CustomFunction )
      {
        NumberOfArguments = ( byte )result.Length;
      }
      else
      {
        NumberOfArguments = ( byte )( result.Length + 1 );
      }

      return result;
    }
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      result[ 1 ] = NumberOfArguments;
      BitConverter.GetBytes( ( ushort )FunctionIndex ).CopyTo( result, 2 );
      return result;
    }
    /// <summary>
    /// Takes all needed operands from the stack and pushes the result of the function.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and will receive result of the operation.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      if( operands == null )
        throw new ArgumentNullException( "operands" );

      if( operands.Count < NumberOfArguments )
        throw new ArgumentException( "Not enough elements in stack" );

      if( FunctionIndex == ExcelFunction.CustomFunction )
      {
        string strDelimeter = formulaUtil.OperandsSeparator;//GetOperandsSeparator( parent );
        StringBuilder builder = new StringBuilder();
        builder.Append( "(" );

        for( int i = 1, last = NumberOfArguments - 1; i <= last; i++ )
        {
          string strOperand = Convert.ToString(operands.Pop());
          builder.Insert( 1, strOperand );

          if( i != last ) builder.Insert( 1, strDelimeter );
        }

        builder.Append( ")" );

        string strFunctionName = ( string )operands.Pop();

        int iLength = strFunctionName.Length;

        if( strFunctionName[ iLength - 1 ] == '\'' )
        {
          int index = strFunctionName.LastIndexOf( '\'', iLength - 2 );

          if( index >= 0 )
          {
            strFunctionName = strFunctionName.Substring( index + 1, iLength - index - 2 );
          }
        }

        builder.Insert( 0, strFunctionName );

        string strResult = builder.ToString();

        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, strResult, "Custom function parsing result" );
        operands.Push( strResult );
      }
      else
      {
        base.PushResultToStack( formulaUtil, operands, isForSerialization );
      }
    }

    #endregion

    #region Class static methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new public static FormulaToken IndexToCode( int index )
    {
      return Ptg.IndexToCode( FormulaToken.tFunctionVar1, index );
    }
    #endregion

    #region Class infill methods
    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public override void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
      NumberOfArguments = provider.ReadByte( offset++ );
      FunctionIndex = ( ExcelFunction )provider.ReadUInt16( offset );
      offset += 2;
    }
    #endregion
  }
}
