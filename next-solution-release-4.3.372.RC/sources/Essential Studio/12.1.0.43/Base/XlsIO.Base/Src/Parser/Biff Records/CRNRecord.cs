#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the contents of an external cell or cell range.
  /// An external cell range has only one row. If a cell range spans over more than
  /// one row, several CRN records will be created.
  /// </summary>
  [ Biff( TBIFFRecord.CRN ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class CRNRecord :
    BiffRecordRaw,
    ICloneable
  {
    #region Class constants
    /// <summary>
    /// Offset to the values.
    /// </summary>
    private const int DEF_VALUES_OFFSET = 4;
    /// <summary>
    /// Error message.
    /// </summary>
    private const string DEF_ERROR_MESSAGE = "Unknown data type";
    /// <summary>
    /// Unused bytes for boolean and error cell values.
    /// </summary>
    private static readonly byte[] DEF_RESERVED_BYTES = new byte[]{ 0, 0, 0, 0, 0, 0, 0 };
    /// <summary>
    /// 
    /// </summary>
    private enum CellValueType
    {
	Nil=0,
      Number = 1,
      String = 2,
      Boolean = 4,
      Error = 16,
    }
    /// <summary>
    /// Default subitem size (boolean, error and number).
    /// </summary>
    private const int DefaultSize = 8;
    #endregion

    #region Class members

    /// <summary>
    /// Index to last column inside of the referenced sheet.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte    m_btLastCol;

    /// <summary>
    /// Index to first column inside of the referenced sheet.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte    m_btFirstCol;

    /// <summary>
    /// Index to row inside of the referenced sheet.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort  m_usRow;
    /// <summary>
    /// Array of cell values.
    /// </summary>
    private List<object> m_arrValues = new List<object>();
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Index to last column inside of the referenced sheet.
    /// </summary>
    public byte   LastColumn
    {
      get
      {
        return m_btLastCol;
      }
      set
      {
        m_btLastCol = value;
      }
    }
    /// <summary>
    /// Index to first column inside of the referenced sheet.
    /// </summary>
    public byte   FirstColumn
    {
      get
      {
        return m_btFirstCol;
      }
      set
      {
        m_btFirstCol = value;
      }
    }

    /// <summary>
    /// Index to row inside of the referenced sheet.
    /// </summary>
    public ushort Row
    {
      get
      {
        return m_usRow;
      }
      set
      {
        m_usRow = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 4;
      }
    }

    /// <summary>
    /// Array of cell values.
    /// </summary>
    public List<object> Values
    {
      get
      {
        return m_arrValues;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  CRNRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CRNRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CRNRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization

    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Data provider that contains record's data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_btLastCol = provider.ReadByte( iOffset );
      m_btFirstCol = provider.ReadByte( iOffset + 1 );
      m_usRow = provider.ReadUInt16( iOffset + 2 );

      int iStartOffset = iOffset;
      iOffset += 4;
//      AutoExtractFields();
      m_arrValues.Clear();

      //for( int i = m_FirstCol; i <= m_LastCol; i++ )
      while( iOffset - iStartOffset < iLength )
      {
        object value = GetValue( provider, ref iOffset );
        m_arrValues.Add( value );
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );

      provider.WriteByte( iOffset, m_btLastCol );
      provider.WriteByte( iOffset + 1, m_btFirstCol );
      provider.WriteUInt16( iOffset + 2, m_usRow );
      iOffset += 4;

      for( int i = 0, len = m_arrValues.Count; i < len; i++ )
      {
        iOffset = SetValue( provider, iOffset, m_arrValues[ i ] );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOffset"></param>
    /// <returns></returns>
    private object GetValue( DataProvider provider, ref int iOffset )
    {
      object result = null;
      byte btCellType = provider.ReadByte( iOffset );
      iOffset++;

      switch( ( CellValueType )btCellType )
      {
        case CellValueType.Number:
          result = provider.ReadDouble( iOffset );
          iOffset += DefaultSize;
          break;

        case CellValueType.String:
          result = provider.ReadString16BitUpdateOffset( ref iOffset );
          break;

        case CellValueType.Boolean:
          result = provider.ReadBoolean( iOffset );
          iOffset += DefaultSize;
          break;

        case CellValueType.Error:
          result = provider.ReadByte( iOffset );
          iOffset += DefaultSize;
          break;
		  
       case CellValueType.Nil:
          iOffset += DefaultSize;
		  break;
        default:
          throw new ApplicationException( DEF_ERROR_MESSAGE );
      }

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOffset"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private int SetValue( DataProvider provider, int iOffset, object value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value is double )
      {
        provider.WriteByte( iOffset, ( byte )CellValueType.Number );
        iOffset++;
        provider.WriteDouble( iOffset, ( double )value );
        iOffset += ExcelConstants.DoubleSize;
      }
      else if( value is string )
      {
        provider.WriteByte( iOffset, ( byte )CellValueType.String );
        iOffset++;
        string strValue = value as string;
        provider.WriteString16BitUpdateOffset( ref iOffset, strValue );

        if( strValue.Length == 0 )
          provider.WriteByte( iOffset++, 0 );
      }
      else if( value is bool )
      {
        provider.WriteByte( iOffset, ( byte )CellValueType.Boolean );
        iOffset++;
        provider.WriteByte( iOffset++, ( byte )( ( bool )value ? 1 : 0 ) );
        provider.WriteBytes( iOffset, DEF_RESERVED_BYTES );
        iOffset += DEF_RESERVED_BYTES.Length;
      }
      else if( value is byte )
      {
        provider.WriteByte( iOffset++, ( byte )CellValueType.Error );
        provider.WriteByte( iOffset++, ( byte )value );
        provider.WriteBytes( iOffset, DEF_RESERVED_BYTES );
        iOffset += DEF_RESERVED_BYTES.Length;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "Wrong data type" );
      }

      return iOffset;
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      // Size of the header data
      int result = ExcelConstants.ShortSize + 2;

      for( int i = 0, len = m_arrValues.Count; i < len; i++ )
      {
        object value = m_arrValues[ i ];
        string strValue = value as string;

        if( strValue != null )
        {
          result += 4 + strValue.Length * 2;
        }
        else
        {
          result += DefaultSize + 1;
        }
      }

      return result;
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public override object Clone()
    {
      CRNRecord result = ( CRNRecord )base.Clone();
      result.m_arrValues = new List<object>( m_arrValues );
      return result;
    }
    #endregion
  }
}
