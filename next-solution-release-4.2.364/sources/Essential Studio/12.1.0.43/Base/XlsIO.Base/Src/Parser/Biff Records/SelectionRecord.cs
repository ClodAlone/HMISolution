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
using System.IO;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;

#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Shows the user's selection on the sheet
  /// for write set num refs to 0.
  /// </summary>
  [ Biff( TBIFFRecord.Selection ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class SelectionRecord
    : BiffRecordRaw
    , ICloneable
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 9;
    /// <summary>
    /// Subitem size.
    /// </summary>
    private const int DEF_SUB_ITEM_SIZE = 6;
    #endregion

    #region Internal classes
    /// <summary>
    /// Each cell range address (called an ADDR structure) contains 4 16-bit values.
    /// Cell range address, BIFF8.
    /// </summary>
    public struct TAddr
    {
      #region Class members
      /// <summary>
      /// Index to first row.
      /// </summary>
      public ushort m_usFirstRow;
      /// <summary>
      /// Index to last row.
      /// </summary>
      public ushort m_usLastRow;
      /// <summary>
      /// Index to first column.
      /// </summary>
      public byte   m_FirstCol;
      /// <summary>
      /// Index to last column.
      /// </summary>
      public byte   m_LastCol;
      #endregion

      #region Class constructor
      /// <summary>
      /// Creates TAddr by specified first and last rows and first and last columns.
      /// </summary>
      /// <param name="FirstRow">First row of the cell range.</param>
      /// <param name="LastRow">Last row of the cell range.</param>
      /// <param name="FirstCol">First column of the cell range.</param>
      /// <param name="LastCol">Last column of the cell range.</param>
      public TAddr( ushort FirstRow, ushort LastRow, byte FirstCol, byte LastCol )
      {
        m_usFirstRow = FirstRow;
        m_usLastRow = LastRow;
        m_FirstCol = FirstCol;
        m_LastCol = LastCol;
      }
      #endregion

      /// <summary>
      /// Converts object to the string.
      /// </summary>
      /// <returns>String representation of the object.</returns>
      public override string ToString()
      {
        return string.Format( "firstRow: {0}, lastRow: {1}, firstColumn: {2}, lastColumn: {3}", 
          m_usFirstRow, m_usLastRow, m_FirstCol, m_LastCol );
      }
    }
    #endregion

    #region Class members
    /// <summary>
    /// The window pane for the record.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_Pane = 3;

    /// <summary>
    /// The active cell's row.
    /// </summary>
    [ BiffRecordPos( 1, 2 ) ]
    private ushort m_usRowActiveCell = 0;

    /// <summary>
    /// The active cell's column.
    /// </summary>
    [ BiffRecordPos( 3, 2 ) ]
    private ushort m_usColActiveCell = 0;

    /// <summary>
    /// The active cell's reference number.
    /// </summary>
    [ BiffRecordPos( 5, 2 ) ]
    private ushort m_usRefActiveCell = 0;

    /// <summary>
    /// The number of cell refs.
    /// </summary>
    [ BiffRecordPos( 7, 2 ) ]
    private ushort m_usNumRefs = 1;

    /// <summary>
    /// List of ADDR structures.
    /// </summary>
    private List<TAddr> m_arrAddr = new List<TAddr>( new TAddr[]{ new TAddr()} );
    #endregion

    #region Class properties
    /// <summary>
    /// The window pane for the record.
    /// </summary>
    public byte Pane
    {
      get
      {
        return m_Pane;
      }
      set
      {
        m_Pane = value;
      }
    }

    /// <summary>
    /// The active cell's row.
    /// </summary>
    public ushort RowActiveCell
    {
      get
      {
        return m_usRowActiveCell;
      }
      set
      {
        m_usRowActiveCell = value;
      }
    }

    /// <summary>
    /// The active cell's column.
    /// </summary>
    public ushort ColumnActiveCell
    {
      get
      {
        return m_usColActiveCell;
      }
      set
      {
        m_usColActiveCell = value;
      }
    }

    /// <summary>
    /// The active cell's reference number.
    /// </summary>
    public ushort RefActiveCell
    {
      get
      {
        return m_usRefActiveCell;
      }
      set
      {
        m_usRefActiveCell = value;
      }
    }

    /// <summary>
    /// The number of cell refs.
    /// </summary>
    public ushort NumRefs
    {
      get
      {
        return m_usNumRefs;
      }
//      set
//      {
//        m_usNumRefs = value;
//      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 9;
      }
    }

    /// <summary>
    /// List of ADDR structures.
    /// </summary>
    public TAddr[] Addr
    {
      get
      {
        //return m_arrAddr;
        return ( TAddr[] )m_arrAddr.ToArray();
      }
      set
      {
        m_arrAddr = new List<TAddr>( value );
        m_usNumRefs = ( ushort )m_arrAddr.Count;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets selection range.
    /// </summary>
    /// <param name="iIndex">Ref index.</param>
    /// <param name="addr">Addr to set.</param>
    public void SetSelection( int iIndex, TAddr addr )
    {
      if( iIndex >= NumRefs || iIndex < 0 )
        throw new ArgumentOutOfRangeException( "iIndex" );

      m_arrAddr[ iIndex ] = addr;
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  SelectionRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  SelectionRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  SelectionRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_Pane = provider.ReadByte( iOffset + 0 );
      m_usRowActiveCell = provider.ReadUInt16( iOffset + 1 );
      m_usColActiveCell = provider.ReadUInt16( iOffset + 3 );
      m_usRefActiveCell = provider.ReadUInt16( iOffset + 5 );
      m_usNumRefs = provider.ReadUInt16( iOffset + 7 );

      // NOTE: we have commented out != sign because for some reason
      // MS Excel 2007 Beta 2 adds 2 bytes to this record and after
      // that it was impossible to parse such files.
      if( m_iLength /*!=*/< 9 + m_usNumRefs * 6 )
      {
        throw new WrongBiffRecordDataException
          ( "Data length does not fit to number of refernces." );
      }

      TAddr addr = new TAddr();
      int iCurOffset = 9;
      m_arrAddr.Clear();
      for( int i = 0; i < m_usNumRefs; i++, iCurOffset += 6 )
      {
        addr.m_usFirstRow = provider.ReadUInt16( iOffset + iCurOffset );
        addr.m_usLastRow = provider.ReadUInt16( iOffset + iCurOffset + 2 );
        addr.m_FirstCol = provider.ReadByte( iOffset + iCurOffset + 4 );
        addr.m_LastCol = provider.ReadByte( iOffset + iCurOffset + 5 );
        m_arrAddr.Add( addr );
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = 9;
      provider.WriteByte( iOffset + 0, m_Pane );
      provider.WriteUInt16( iOffset + 1, m_usRowActiveCell );
      provider.WriteUInt16( iOffset + 3, m_usColActiveCell );
      provider.WriteUInt16( iOffset + 5, m_usRefActiveCell );
      provider.WriteUInt16( iOffset + 7, m_usNumRefs );

      TAddr addr;
      for( int i = 0; i < m_usNumRefs; i++, m_iLength += 6 )
      {
        addr = ( TAddr ) m_arrAddr[ i ];
        provider.WriteUInt16( iOffset + m_iLength,     addr.m_usFirstRow );
        provider.WriteUInt16( iOffset + m_iLength + 2, addr.m_usLastRow );
        provider.WriteByte( iOffset + m_iLength + 4, addr.m_FirstCol );
        provider.WriteByte( iOffset + m_iLength + 5, addr.m_LastCol );
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_SIZE + m_arrAddr.Count * DEF_SUB_ITEM_SIZE;
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    new public object Clone()
    {
      SelectionRecord result = ( SelectionRecord )base.MemberwiseClone();
      result.m_arrAddr = new List<TAddr>( m_arrAddr );
      return result;
    }
    #endregion
  }
}
