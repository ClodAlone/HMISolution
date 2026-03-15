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
  /// This record stores the colour of the tab below the sheet containing the sheet name.
  /// </summary>
  [ Biff( TBIFFRecord.SheetLayout ) ]
  [ CLSCompliant( false ) ]
  public class SheetLayoutRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Record size.
    /// </summary>
    public const int DefaultRecordSize = 20;
    #endregion

    #region Class members
    /// <summary>
    /// Repeated record identifier.
    /// </summary>
    [ BiffRecordPos( 0, 2, true ) ]
    private short m_id;
    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 2, 4, true ) ]
    private int m_iReserved1;
    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_iReserved2;
    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 10, 2, true ) ]
    private short m_sReserved3;
    /// <summary>
    /// Unknown data.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iUnknown = 0x00000014;
    /// <summary>
    /// Color index for sheet name tab.
    /// </summary>
    [ BiffRecordPos( 16, 4, true ) ]
    private int m_iColorIndex;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  SheetLayoutRecord()
      : base()
    {
      m_id = ( short )TypeCode;
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
    public  SheetLayoutRecord( Stream stream, out int itemSize )
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
    public  SheetLayoutRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Repeated record identifier.
    /// </summary>
    public short Id
    {
      get
      {
        return m_id;
      }
      set
      {
        m_id = value;
      }
    }
    /// <summary>
    /// Reserved.
    /// </summary>
    public int Reserved1
    {
      get
      {
        return m_iReserved1;
      }
      set
      {
        m_iReserved1 = value;
      }
    }
    /// <summary>
    /// Reserved.
    /// </summary>
    public int Reserved2
    {
      get
      {
        return m_iReserved2;
      }
      set
      {
        m_iReserved2 = value;
      }
    }
    /// <summary>
    /// Reserved.
    /// </summary>
    public short Reserved3
    {
      get
      {
        return m_sReserved3;
      }
      set
      {
        m_sReserved3 = value;
      }
    }
    /// <summary>
    /// Unknown data.
    /// </summary>
    public int Unknown
    {
      get
      {
        return m_iUnknown;
      }
      set
      {
        m_iUnknown = value;
      }
    }
    /// <summary>
    /// Color index for sheet name tab.
    /// </summary>
    public int ColorIndex
    {
      get
      {
        return m_iColorIndex;
      }
      set
      {
        m_iColorIndex = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }

    /// <summary>
    /// Maximum memory size for internal buffer.
    /// </summary>
    public override int MaximumMemorySize
    {
      get
      {
        return DefaultRecordSize;
      }
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
      m_id = provider.ReadInt16( iOffset + 0 );
      m_iReserved1 = provider.ReadInt32( iOffset + 2 );
      m_iReserved2 = provider.ReadInt32( iOffset + 6 );
      m_sReserved3 = provider.ReadInt16( iOffset + 10 );
      m_iUnknown = provider.ReadInt32( iOffset + 12 );
      m_iColorIndex = provider.ReadInt32( iOffset + 16 );
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
      provider.WriteInt16( iOffset + 0, m_id );
      provider.WriteInt32( iOffset + 2, m_iReserved1 );
      provider.WriteInt32( iOffset + 6, m_iReserved2 );
      provider.WriteInt16( iOffset + 10, m_sReserved3 );
      provider.WriteInt32( iOffset + 12, m_iUnknown );
      provider.WriteInt32( iOffset + 16, m_iColorIndex );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Size of the required storage space.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
