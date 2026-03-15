#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This class represents function token of a formula.
  /// </summary>
  [ Token ( FormulaToken.tFunction1 ) ]
  [ Token ( FormulaToken.tFunction2 ) ]
  [ Token ( FormulaToken.tFunction3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class FunctionPtg : OperationPtg
  {
    #region Class members
    /// <summary>
    /// Index to built-in sheet function.
    /// </summary>
    private ExcelFunction m_FunctionIndex = ExcelFunction.NONE;
    /// <summary>
    /// Number of arguments.
    /// </summary>
    private byte m_ArgumentsNumber;
    #endregion

    #region Class static members
    /// <summary>
    /// Delimiter between function arguments.
    /// </summary>
    public const string OperandsDelimiter = ",";
//    /// <summary>
//    /// Array with all TokenAttributes.
//    /// </summary>
//    private static readonly TokenAttribute[] s_arrAttributes;
    #endregion

    #region Class constructors
//    /// <summary>
//    /// Static constructor.
//    /// </summary>
//    static FunctionPtg()
//    {
//      s_arrAttributes = ( TokenAttribute[] )
//        typeof( UnaryOperationPtg ).GetCustomAttributes( typeof( TokenAttribute ), false );
//    }
    /// <summary>
    /// Default constructor
    /// </summary>
    public FunctionPtg()
    {
      this.TokenCode = FormulaToken.tFunction2;
    }
    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public FunctionPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Constructs token by function index.
    /// </summary>
    /// <param name="index">Function index.</param>
    public FunctionPtg( ExcelFunction index )
    {
      m_FunctionIndex = index;
      TokenCode = FormulaToken.tFunction2;
      int iParamCount;

      if( FormulaUtil.FunctionIdToParamCount.TryGetValue( index, out iParamCount ) )
        m_ArgumentsNumber = ( byte )iParamCount;
    }
    /// <summary>
    /// Constructs function token by function name.
    /// </summary>
    /// <param name="strFunctionName">Valid function name.</param>
    public FunctionPtg( string strFunctionName )
      : this( ( ExcelFunction )FormulaUtil.FunctionAliasToId[ strFunctionName ] )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns built-in function index.
    /// </summary>
    public ExcelFunction FunctionIndex
    {
      get
      {
        return m_FunctionIndex;
      }
      set
      {
        m_FunctionIndex = value;
      }
    }
    /// <summary>
    /// Number of function arguments.
    /// </summary>
    public byte NumberOfArguments
    {
      get
      {
        return m_ArgumentsNumber;
      }
      set
      {
        m_ArgumentsNumber = value;
      }
    }
    /// <summary>
    /// Read-only. Type of operation (TYPE_FUNCTION for all functions).
    /// </summary>
    public override TOperation OperationType
    {
      get
      {
        return TOperation.TYPE_FUNCTION;
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
        return null;//s_arrAttributes;
      }
    }  
    #endregion

    #region Class methods
    /// <summary>
    /// Gets operands from string and returns array of string representation of the operands.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index to built-in function.</param>
    /// <param name="checkParamCount">True if function should check operands count.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings that contain function operands.</returns>
    /// <exception cref="System.ArgumentException">
    ///   if checkParamCount = True and actual parameter count does not equal expected
    /// </exception>
    protected string[] GetOperands( string strFormula, ref int index, bool checkParamCount,
      FormulaUtil formulaParser )
    {
      List<string> result = new List<string>();
      int count = 0;
      strFormula = strFormula.Substring( index + 1, strFormula.Length - index - 2 );
      index = -1;

      if( strFormula.Length > 0 )
      {
        while( index < strFormula.Length )
        {
          string operand = formulaParser.GetFunctionOperand( strFormula, index );
          result.Add( operand );
          index += operand.Length + 1;
          count++;
        }
      }

      if( checkParamCount && count != m_ArgumentsNumber )
        throw new ArgumentException( "Too many or not enough arguments." );

      return result.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static FormulaToken IndexToCode( int index )
    {
      return Ptg.IndexToCode( FormulaToken.tFunction1, index );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 3;
    }

    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn,
      bool bR1C1,
      NumberFormatInfo numberFormat,
      bool isForSerialization )
    {
      string result;

      if( !FormulaUtil.FunctionIdToAlias.TryGetValue( m_FunctionIndex, out result ) )
        result = ( ( Excel2007Function )m_FunctionIndex ).ToString();

      if (isForSerialization && (FormulaUtil.IsExcel2010Function(m_FunctionIndex) || FormulaUtil.IsExcel2013Function(m_FunctionIndex)))
      {
          result = FormulaUtil.Excel2010FunctionPrefix + result;
      }

      return result;
    }
    /// <summary>
    /// Converts the operation and its operands to the string.
    /// Gets all needed operands from the Stack parameter and pushes the result into the Stack.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and receives result of the operation.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      if( operands.Count < m_ArgumentsNumber )
      {
        throw new ArgumentOutOfRangeException( "Not enough arguments." );
      }

      string strFunctionName = ToString( formulaUtil, 0, 0, false, null, isForSerialization );
      // We use this function because there can be space in the stack.
      FormulaUtil.PushOperandToStack( operands, strFunctionName );
      strFunctionName = ( string )operands.Pop();

      string result = ( m_ArgumentsNumber > 0 ) ?
        operands.Pop().ToString() : string.Empty;

      string strDelimeter = formulaUtil.OperandsSeparator;//GetOperandsSeparator( parent );

      // TODO: here we can add StringBuilder.
      for( int i = 1; i < m_ArgumentsNumber; i++ )
      {
        result = operands.Pop().ToString() + strDelimeter + result;
      }

      string strValue = strFunctionName + "(" + result + ")"; 
      FormulaUtil.PushOperandToStack( operands, strValue );
    }
    /// <summary>
    /// Gets operands from string and returns array of string representation of the operands.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index of the function in the formula string.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings that contains function operands.</returns>
    public override string[] GetOperands(string strFormula, ref int index, FormulaUtil formulaParser )
    {
      return GetOperands( strFormula, ref index, true, formulaParser );
    }

    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      result[ 0 ] = ( byte )TokenCode;
      BitConverter.GetBytes( ( ushort ) m_FunctionIndex ).CopyTo( result, 1 );

      return result;
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
      m_FunctionIndex = ( ExcelFunction )provider.ReadUInt16( offset );
      string name;

      if( !FormulaUtil.FunctionIdToAlias.TryGetValue( m_FunctionIndex, out name ) )
        throw new ArgumentNullException( "Unknown function" );

      int value; ;

      if( FormulaUtil.FunctionIdToParamCount.TryGetValue( m_FunctionIndex, out value ) )
        m_ArgumentsNumber = ( byte )value;

      offset += 2;
    }
    #endregion
  }
}
