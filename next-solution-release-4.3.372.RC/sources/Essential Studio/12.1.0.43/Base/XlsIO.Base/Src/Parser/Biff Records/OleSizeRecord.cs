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

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the size of an embedded OLE object
  /// (when Microsoft Excel is a server).
  /// </summary>
  [ Biff( TBIFFRecord.OleSize ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class OleSizeRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usReserved = 0;
    /// <summary>
    /// First row of the object.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFirstRow;
    /// <summary>
    /// Last row of the object.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usLastRow;
    /// <summary>
    /// First column of the object.
    /// </summary>
    [ BiffRecordPos( 6, 1 ) ]
    private byte m_FirstColumn;
    /// <summary>
    /// Last column of the object.
    /// </summary>
    [ BiffRecordPos( 7, 1 ) ]
    private byte m_LastColumn;
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Get reserved field value.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
    }
    /// <summary>
    /// First row of the object.
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
    /// Last row of the object.
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
    /// First column of the object.
    /// </summary>
    public byte FirstColumn
    {
      get
      {
        return m_FirstColumn;
      }
      set
      {
        m_FirstColumn = value;
      }
    }
    /// <summary>
    /// Last column of the object.
    /// </summary>
    public byte LastColumn
    {
      get
      {
        return m_LastColumn;
      }
      set
      {
        m_LastColumn = value;
      }
    }
    /// <summary>
    /// Read-only. Return minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 8;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return 8;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, sets all fields' default values.
    /// </summary>
    public  OleSizeRecord()
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
    public  OleSizeRecord( Stream stream, out int itemSize )
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
    public  OleSizeRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usReserved = provider.ReadUInt16( iOffset + 0 );
      m_usFirstRow = provider.ReadUInt16( iOffset + 2 );
      m_usLastRow = provider.ReadUInt16( iOffset + 4 );
      m_FirstColumn = provider.ReadByte( iOffset + 6 );
      m_LastColumn = provider.ReadByte( iOffset + 7 );
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
      provider.WriteUInt16( iOffset + 0, m_usReserved );
      provider.WriteUInt16( iOffset + 2, m_usFirstRow );
      provider.WriteUInt16( iOffset + 4, m_usLastRow );
      provider.WriteByte( iOffset + 6, m_FirstColumn );
      provider.WriteByte( iOffset + 7, m_LastColumn );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
