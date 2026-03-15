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
  /// This class is the base class for all operation tokens in the formula.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public abstract class OperationPtg : Ptg
  {
    #region Class constants
    /// <summary>
    /// Default arguments separator.
    /// </summary>
    private const string DEFAULT_ARGUMENTS_SEPARATOR = ",";
    #endregion

    #region Class members
    /// <summary>
    /// String representation of the operation.
    /// </summary>
    private string m_strOperationSymbol = string.Empty;
    /// <summary>
    /// Position of the operand (before (if it is set to False) or after (if it is set to True) operand) for unary operations.
    /// </summary>
    private bool m_bPlaceAfter = false;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public OperationPtg()
    {
    }

    /// <summary>
    /// Creates operation from byte array and offset in this array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    protected OperationPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. True if this class represents operation (always True for this class).
    /// </summary>
    override public bool IsOperation
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Read-only. Type of the operation.
    /// </summary>
    public abstract TOperation OperationType{ get; }
    /// <summary>
    /// Read-only. Number of operands this operation needs.
    /// </summary>
    public virtual int NumberOfOperands
    {
      get
      {
        switch( OperationType )
        {
          case TOperation.TYPE_BINARY:
            return 1;

          case TOperation.TYPE_UNARY:
            return 2;

          default:
            return 0;
        }
      }
    }
    /// <summary>
    /// Gets / sets string representation of the operation.
    /// </summary>
    public string OperationSymbol
    {
      get
      {
        return m_strOperationSymbol;
      }
      set
      {
        m_strOperationSymbol = value;
      }
    }
    /// <summary>
    /// Gets / sets True if operation sign should be placed after operand and False otherwise.
    /// </summary>
    public bool IsPlaceAfter
    {
      get
      {
        return m_bPlaceAfter;
      }
      set
      {
        m_bPlaceAfter = value;
      }
    }
    /// <summary>
    /// Array of all token attributes applied to the object.
    /// This property is used to increase performance.
    /// </summary>
    protected abstract TokenAttribute[] Attributes { get; }  
    #endregion

    #region Class methods
    /// <summary>
    /// Converts the operation and its operands to the string.
    /// Gets all needed operands from the Stack parameter and pushes the result into the Stack.
    /// </summary>
    /// <param name="operands">
    /// Stack that contains all operands and receives result of the operation.
    /// </param>
    public virtual void PushResultToStack( Stack<object> operands )
    {
      PushResultToStack( null, operands, false );
    }
    /// <summary>
    /// Converts the operation and its operands to the string.
    /// Gets all needed operands from the Stack parameter and pushes the result into the Stack.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands and receives result of the operation.
    /// </param>
    public abstract void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization );
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat,
      bool isForSerialization )
    {
      return m_strOperationSymbol;
    }

    /// <summary>
    /// Returns array of string where each member corresponds to one argument.
    /// </summary>
    /// <param name="strFormula">String representation of the formula.</param>
    /// <param name="index">Index of the operation.</param>
    /// <param name="formulaParser">Formula parser.</param>
    /// <returns>Array of strings containing operation operands.</returns>
    public abstract string[] GetOperands( string strFormula, ref int index, FormulaUtil formulaParser );
    /// <summary>
    /// Updates parse formula options if necessary.
    /// </summary>
    /// <param name="options">Options to update.</param>
    /// <returns>Updated value.</returns>
    public virtual ExcelParseFormulaOptions UpdateParseOptions( ExcelParseFormulaOptions options )
    {
      return options | ExcelParseFormulaOptions.ParseComplexOperand;
    }
    /// <summary>
    /// Returns arguments separator.
    /// </summary>
    /// <param name="formulaUtil">FormulaUtil object, to get separator from.</param>
    /// <returns>Arguments separator.</returns>
    protected string GetOperandsSeparator( FormulaUtil formulaUtil )
    {
      if( formulaUtil != null )
      {
        return formulaUtil.OperandsSeparator;
      }

      return DEFAULT_ARGUMENTS_SEPARATOR;
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
      TokenAttribute[] allAttributes = Attributes;

      if( allAttributes == null ) return;

      for( int i = 0, len = allAttributes.Length; i < len; i++ )
      {
        TokenAttribute attr = allAttributes[ i ];

        if( attr.FormulaType == TokenCode )
        {
          m_strOperationSymbol = attr.OperationSymbol;
          m_bPlaceAfter = attr.IsPlaceAfter;
          break;
        }
      }
    }
    #endregion
  }
}
