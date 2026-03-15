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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for DConRefRecord.
  /// </summary>
  [ Biff( TBIFFRecord.DCONRef ) ]
  [ CLSCompliant( false ) ]
  public class DConRefRecord : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// First row of the source area for consolidation.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFirstRow;
    /// <summary>
    /// Last row of the source area for consolidation.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLastRow;
    /// <summary>
    /// First column of the source area for consolidation.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte m_btFirstColumn;
    /// <summary>
    /// Last column of the source area for consolidation.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private byte m_btLastColumn;
    /// <summary>
    /// Workbook name.
    /// </summary>
    [ BiffRecordPos( 6, TFieldType.String16Bit ) ]
    private string m_strWorkbookName;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  DConRefRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  DConRefRecord( Stream stream, out int itemSize )
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
    public  DConRefRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// First row of the source area for consolidation.
    /// </summary>
    public ushort FirstRow
    {
      get
      {
        return m_usFirstRow;
      }
      set
      {
        m_usFirstRow = value;
      }
    }
    /// <summary>
    /// Last row of the source area for consolidation.
    /// </summary>
    public ushort LastRow
    {
      get
      {
        return m_usLastRow;
      }
      set
      {
        m_usLastRow = value;
      }
    }
    /// <summary>
    /// First column of the source area for consolidation.
    /// </summary>
    public byte FirstColumn
    {
      get
      {
        return m_btFirstColumn;
      }
      set
      {
        m_btFirstColumn = value;
      }
    }
    /// <summary>
    /// Last column of the source area for consolidation.
    /// </summary>
    public byte LastColumn
    {
      get
      {
        return m_btLastColumn;
      }
      set
      {
        m_btLastColumn = value;
      }
    }
    /// <summary>
    /// Workbook name.
    /// </summary>
    public string WorkbookName
    {
      get
      {
        return m_strWorkbookName;
      }
      set
      {
        m_strWorkbookName = value;
      }
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
      m_usFirstRow = provider.ReadUInt16( iOffset + 0 );
      m_usLastRow = provider.ReadUInt16( iOffset + 2 );
      m_btFirstColumn = provider.ReadByte( iOffset + 4 );
      m_btLastColumn = provider.ReadByte( iOffset + 5 );
      int iFullLength;
      m_strWorkbookName = provider.ReadString16Bit( iOffset + 6, out iFullLength );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteUInt16( iOffset + 0, m_usFirstRow );
      provider.WriteUInt16( iOffset + 2, m_usLastRow );
      provider.WriteByte( iOffset + 4, m_btFirstColumn );
      provider.WriteByte( iOffset + 5, m_btLastColumn );
      provider.WriteString16Bit( iOffset + 6, m_strWorkbookName );
      m_iLength = GetStoreSize( version );
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      const int FixedSize = 6;
      const int StringHeaderSize = 3;
      return FixedSize + StringHeaderSize + m_strWorkbookName.Length * 2;
    }
    #endregion
  }
}
