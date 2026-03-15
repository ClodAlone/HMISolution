#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains an array constant. The values of the array constant do not follow
  /// the token identifier but are stored behind the complete token array.
  /// </summary>
  [ Token ( FormulaToken.tArray1 ) ]
  [ Token ( FormulaToken.tArray2 ) ]
  [ Token ( FormulaToken.tArray3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ArrayPtg
    : Ptg
    , IAdditionalData
    , ICloneable
  {
    #region Class constants
    /// <summary>
    /// Constant that indicates that there is double value in the array.
    /// </summary>
    public const byte DOUBLEVALUE     = 0x01;
    /// <summary>
    /// Constant that indicates that there is string value in the array.
    /// </summary>
    public const byte STRINGVALUE     = 0x02;
    /// <summary>
    /// Constant that indicates that there is boolean value in the array.
    /// </summary>
    public const byte BOOLEANVALUE    = 0x04;
    /// <summary>
    /// Constant that indicates that there is error code in the array.
    /// </summary>
    public const byte ERRORCODEVALUE  = 0x10;
    /// <summary>
    /// Separators between rows of the array.
    /// </summary>
    public static readonly string RowSeparator = ";";
    /// <summary>
    /// Separators between columns of the array.
    /// </summary>
    public static readonly string ColSeparator = ",";
    /// <summary>
    /// Constant that indicates that there is null code in the array.
    /// </summary>
    public const byte NilValue = 0;
    #endregion

    #region Class members
    /// <summary>
    /// Number of columns decreased by 1.
    /// </summary>
    private byte m_ColumnNumber;
    /// <summary>
    /// Number of rows decreased by 1.
    /// </summary>
    private ushort m_usRowNumber;
    /// <summary>
    /// Array of cached values.
    /// </summary>
    private object[,] m_arrCachedValue;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public ArrayPtg()
    {
    }
    /// <summary>
    /// Constructs array token using string representation.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="formulaParser">Formula parser.</param>
    public ArrayPtg( string strFormula, FormulaUtil formulaParser )
    {
      strFormula = strFormula.Substring( 1, strFormula.Length - 2 );
      List<string> arrRows = formulaParser.SplitArray( strFormula, formulaParser.ArrayRowSeparator );
      List<string>[] arrValues = new List<string>[ arrRows.Count ];

      for( int i = 0, len = arrRows.Count; i < len; i++ )
      {
        string strRow = arrRows[ i ];
        arrValues[ i ] = formulaParser.SplitArray( strRow, formulaParser.OperandsSeparator );

        if( i > 0 && arrValues[ i ].Count != arrValues[ i - 1 ].Count )
          throw new ArgumentException( "Each row in the tArray must have the same column number." );
      }

      FillList( arrValues, formulaParser );

      SetReferenceIndex( FormulaUtil.DEF_ARRAY_INDEX );
    }
    /// <summary>
    /// Constructs array but does not read any data. ReadArray should be called for
    /// this purpose because array data is placed just after all other tokens.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public ArrayPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int AdditionalDataSize
    {
      get
      {
        int iResult = 0;

        if( m_arrCachedValue != null )
        {
          iResult += 3; // number of rows and columns 2 + 1 bytes.
          foreach( object item in m_arrCachedValue )
          {
            if( item is double || item is bool || item is byte || item ==null)
            {
              iResult += 9;
            }
            else if( item is string )
            {
              iResult += Encoding.Unicode.GetByteCount( ( string )item ) + 4;
            }
            else
            {
              throw new ArrayTypeMismatchException( "Unexpected type in tArray." );
            }
          }
        }

        return iResult;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Reads array from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Starting position of array data.</param>
    /// <returns>Offset if the first byte after array.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is too small for tArray data.
    /// </exception>
    public int ReadArray( DataProvider provider, int offset )
    {
      m_ColumnNumber = provider.ReadByte( offset );
      m_usRowNumber = ( ushort )provider.ReadInt16( offset + 1 );

      return FillList( provider, offset + 3, m_ColumnNumber + 1, m_usRowNumber + 1 );
    }
    /// <summary>
    /// Fills tArray token with data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset in data array.</param>
    /// <param name="ColumnNumber">Number of columns in this array.</param>
    /// <param name="RowNumber">Number of rows in this array.</param>
    /// <returns>Returns final offset in data array.</returns>
    private int FillList( DataProvider provider, int offset, int ColumnNumber, int RowNumber )
    {
      double dValue;
      string strValue;
      bool bValue;
      byte CachedType;
      int strLen;
      m_arrCachedValue = new object[ RowNumber, ColumnNumber ];

      for( int i = 0; i < RowNumber; i++ )
      {
        for( int j = 0; j < ColumnNumber; j++ )
        {
          CachedType = provider.ReadByte( offset++ );

          switch( CachedType )
          {
            case DOUBLEVALUE: // double value
              dValue = provider.ReadDouble( offset );
              m_arrCachedValue[ i, j ] = dValue;
              offset += 8;
              break;

            case STRINGVALUE: // string value
              strValue = provider.ReadString16Bit( offset, out strLen );
              m_arrCachedValue[ i, j ] = strValue;
              offset += strLen;
              break;

            case BOOLEANVALUE: // boolean value
              bValue = provider.ReadBoolean( offset );
              m_arrCachedValue[ i, j ] = bValue;
              offset += 8;
              break;

            case ERRORCODEVALUE: // error code
              m_arrCachedValue[ i, j ] = provider.ReadByte( offset );
              offset += 8;
              break;
            
              case NilValue:
              m_arrCachedValue[i, j] = null;
              offset += 8;
              break;

            default:
              throw new ArgumentOutOfRangeException( "Unknown type in tArray: " + CachedType );

          }
        }
      }
      return offset;
    }
    /// <summary>
    /// Fills tArray token with data from byte array.
    /// </summary>
    /// <param name="data">Data array.</param>
    /// <param name="offset">Offset in data array.</param>
    /// <param name="ColumnNumber">Number of columns in this array.</param>
    /// <param name="RowNumber">Number of rows in this array.</param>
    /// <returns>Returns final offset in data array.</returns>
    private int FillList( byte[] data, int offset, int ColumnNumber, int RowNumber )
    {
      double dValue;
      string strValue;
      bool bValue;
      byte CachedType;
      int strLen;
      m_arrCachedValue = new object[ RowNumber, ColumnNumber ];

      for( int i = 0; i < RowNumber; i++ )
      {
        for( int j = 0; j < ColumnNumber; j++ )
        {
          CachedType = data[ offset++ ];

          switch( CachedType )
          {
            case DOUBLEVALUE: // double value
              if( offset + 8 > data.Length )
                throw new ArgumentOutOfRangeException( "FillList: data array too small." );

              dValue = BitConverter.ToDouble( data, offset );
              m_arrCachedValue[ i, j ] = dValue;
              offset += 8;
              break;

            case STRINGVALUE: // string value
              strValue = GetString16Bit( data, offset, out strLen );
              m_arrCachedValue[ i, j ] = strValue;
              offset += strLen;
              break;

            case BOOLEANVALUE: // boolean value
              if( offset + 8 > data.Length )
                throw new ArgumentOutOfRangeException( "FillList: data array too small." );

              bValue = BitConverter.ToBoolean( data, offset );
              m_arrCachedValue[ i, j ] = bValue;
              offset += 8;
              break;

            case ERRORCODEVALUE: // error code
              if( offset + 8 > data.Length )
                throw new ArgumentOutOfRangeException( "FillList: data array too small." );

              m_arrCachedValue[ i, j ] = data[ offset ];
              offset += 8;
              break;

            default:
              throw new ArgumentOutOfRangeException( "Unknown type in tArray: " + CachedType );

          }
        }
      }
      return offset;
    }
    /// <summary>
    /// Fills tArray token getting data from array of strings.
    /// </summary>
    /// <param name="arrValues">Array of string that contains array data.</param>
    /// <param name="formulaParser">Formula parser object that should assist in array items parsing.</param>
    /// <exception cref="System.ArgumentException">
    /// When one of the specified arguments is empty.
    /// </exception>
    private void FillList( List<string>[] arrValues, FormulaUtil formulaParser )
    {
      int iRowNumber = arrValues.Length;
      int iColNumber = arrValues[ 0 ].Count;
      m_ColumnNumber = ( byte ) ( iColNumber - 1 );
      m_usRowNumber = ( ushort ) ( iRowNumber - 1 );
      m_arrCachedValue = new object[ iRowNumber, iColNumber ];

      for( int i = 0; i < iRowNumber; i++ )
      {
        if( arrValues[ i ].Count != iColNumber )
        {
          m_arrCachedValue = null;
          throw new ArgumentException( "Each row in the tArray must have same number of columns." );
        }

        for( int j = 0; j < iColNumber; j++ )
        {
          m_arrCachedValue[ i, j ] = ParseConstant( arrValues[ i ][ j ], formulaParser );
        }
      }
    }
    /// <summary>
    /// Tries to parse string value as double, bool, or string and returns resulting object.
    /// </summary>
    /// <param name="value">String constant that will be parsed.</param>
    /// <param name="formulaParser">Formula parser object that should assist in array items parsing.</param>
    /// <returns>Parsed constant (double, bool, or string).</returns>
    private object ParseConstant( string value, FormulaUtil formulaParser )
    {
      if( value.Length == 0 ) throw new ArgumentException( "Constant string can't be empty." );
      // Let's check if it is double.
      double dValue;

      if( double.TryParse( value, System.Globalization.NumberStyles.Any, formulaParser.NumberFormat, out dValue ) )
      {
        return dValue;
      }

      // Now let's check if it is boolean.
      try
      {
        bool bValue = bool.Parse( value.ToLower() );
        return bValue;
      }
      catch( System.FormatException )
      {
      }

      // Now let's check if it is string.
      if( value[ 0 ] == '"' && '"' == value[ value.Length - 1 ] )
      {
        return value.Substring( 1, value.Length - 2 );
      }
      else
      {
        Ptg[] arrTokens = formulaParser.ParseString( value );

        if( arrTokens != null && arrTokens.Length == 1 )
        {
          ErrorPtg errorToken = arrTokens[ 0 ] as ErrorPtg;

          if( errorToken != null )
            return errorToken.ErrorCode;
        }
      }

      // unknown type
      return null;
    }
    /// <summary>
    /// Returns all values in data array. Must be written after formula data.
    /// </summary>
    /// <returns>Array of bytes of all constants stored in the array.</returns>
    public BytesList GetListBytes()
    {
      BytesList result = new BytesList( false );

      result.Add( m_ColumnNumber );
      result.AddRange( BitConverter.GetBytes( m_usRowNumber ) );

      foreach( object item in m_arrCachedValue )
      {
        if( item is double )
        {
          result.Add( ( byte ) DOUBLEVALUE );
          result.AddRange( GetDoubleBytes( ( double )item ) );
        }
        else if( item is string )
        {
          result.Add( ( byte ) STRINGVALUE );
          result.AddRange( GetStringBytes( ( string )item ) );
        }
        else if( item is bool )
        {
          result.Add( ( byte ) BOOLEANVALUE );
          result.AddRange( GetBoolBytes( ( bool )item ) );
        }
        else if( item is byte )
        {
          result.Add( ( byte ) ERRORCODEVALUE );
          result.AddRange( GetErrorCodeBytes( ( byte )item ) );
        }
        else if (item == null)
        {
            result.Add((byte)0);
            result.AddRange(GetNilBytes());
        }
        else
        {
          throw new ArrayTypeMismatchException( "Unexpected type in tArray." );
        }
      }

      //return ( byte[] ) result.ToArray( typeof( byte ) );
      return result;
    }

    /// <summary>
    /// Returns an array representation of boolean value.
    /// </summary>
    /// <param name="value">Boolean value that will be converted.</param>
    /// <returns>Array of bytes representing boolean value.</returns>
    private byte[] GetBoolBytes( bool value )
    {
      byte[] result = new byte[ 8 ]{ 0, 0, 0, 0, 0, 0, 0, 0 };

      if( value ) result[ 0 ] = 1;

      return result;
    }

    /// <summary>
    /// Returns an array representation of the error code value.
    /// </summary>
    /// <param name="value">Error code.</param>
    /// <returns>Array of bytes representing the error code.</returns>
    private byte[] GetErrorCodeBytes( byte value )
    {
      byte[] result = new byte[ 8 ]{ value, 0, 0, 0, 0, 0, 0, 0 };

      return result;
    }
    /// <summary>
    /// Returns an array representation of the null code value.
    /// </summary>
    /// <returns>Array of bytes representing the null code.</returns>
    private byte[] GetNilBytes()
    {
        byte[] result = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        return result;
    }
    /// <summary>
    /// Returns an array representation of double value.
    /// </summary>
    /// <param name="value">Double value that will be converted.</param>
    /// <returns>Array of bytes representing double value.</returns>
    private byte[] GetDoubleBytes( double value )
    {
      return BitConverter.GetBytes( value );
    }
    /// <summary>
    /// Returns an array representation of the string value.
    /// </summary>
    /// <param name="value">String value that will be converted.</param>
    /// <returns>Array of bytes representing the string value.</returns>
    private byte[] GetStringBytes( string value )
    {
      byte[] buffer = Encoding.Unicode.GetBytes( value );
      byte[] result = new byte[ buffer.Length + 3 ];
      buffer.CopyTo( result, 3 );
      BitConverter.GetBytes( ( ushort ) value.Length ).CopyTo( result, 0 );
      result[ 2 ] = 1;

      return result;
    }
    /// <summary>
    /// Sets token code in accordance with referenceIndex.
    /// </summary>
    /// <param name="referenceIndex">Index that corresponds to the token code.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or larger than 3.
    /// </exception>
    private void SetReferenceIndex( int referenceIndex )
    {
      switch( referenceIndex )
      {
        case 1:
          TokenCode = FormulaToken.tArray1;
          break;

        case 2:
          TokenCode = FormulaToken.tArray2;
          break;

        case 3:
          TokenCode = FormulaToken.tArray3;
          break;

        default:
          throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the array token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 8;
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
      string result = string.Empty;

      result += "{";
      int j;

      string strColumnSeparator = ColSeparator;
      string strRowSeparator = RowSeparator;

      if( formulaUtil != null )
      {
        strColumnSeparator = formulaUtil.OperandsSeparator;
        strRowSeparator = formulaUtil.ArrayRowSeparator;
      }

      for( int i = 0; i <= m_usRowNumber; i++ )
      {
        for( j = 0; j < m_ColumnNumber; j++ )
        {
          result += ( m_arrCachedValue[ i, j ] is string )
            ? '"' + m_arrCachedValue[ i, j ].ToString() + '"'
            : m_arrCachedValue[ i, j ];
          result += strColumnSeparator;
        }

        result += ( m_arrCachedValue[ i, j ] is string )
          ? '"' + m_arrCachedValue[ i, j ].ToString() + '"'
          : m_arrCachedValue[ i, m_ColumnNumber ];

        if( i != m_usRowNumber ) result += strRowSeparator;
      }

      return result + "}";
    }

    /// <summary>
    /// Converts tArray token to byte array.
    /// </summary>
    /// <param name="version">Excel version - defines resulting array format and size.</param>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      result[ 1 ] = 0;

      return result;
    }

    #endregion

    #region Class static methods
    /// <summary>
    /// Returns token code by index.
    /// </summary>
    /// <param name="index">Index of the needed token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or larger than 3.
    /// </exception>
    public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tArray1;
        case 2: return FormulaToken.tArray2;
        case 3: return FormulaToken.tArray3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    new public object Clone()
    {
      object result = base.Clone();
      object[,] arrOldValues = m_arrCachedValue;
      
      if( m_arrCachedValue != null )
      {
        int iRows = arrOldValues.GetLength( 0 );
        int iCols = arrOldValues.GetLength( 1 );

        for( int i = 0; i < iRows; i++ )
        {
          for( int j = 0; j < iCols; j++ )
          {
            m_arrCachedValue[ i, j ] = arrOldValues[ i, j ];
          }
        }
      }

      return result;
    }

    #endregion

    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public override void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
      offset += GetSize( version ) - 1;
    }
  }
}
