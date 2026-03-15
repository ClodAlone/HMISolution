#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores information about multiple-consolidation PivotTable source data.
  /// </summary>
  [ Biff( TBIFFRecord.PivotSourceInfo ) ]
  [ CLSCompliant( false ) ]
  public class PivotSourceInfoRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for PageCount property.
    /// </summary>
    private const ushort DEF_BITMASK_PAGECOUNT = 0x07FFF;
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Count (1-based) of DCONREF or DCONNAME records that follow the SXTBL record.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRefCount;
    /// <summary>
    /// Count (1-based) of SXTBPG records that follow the DCONREF or DCONNAME records.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usPageItemCount;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Indicates whether the user selected the Create A Single Page Field For Me
    /// option in PivotTable Wizard dialog box.
    /// </summary>
    [ BiffRecordPos( 5, 7, TFieldType.Bit ) ]
    private bool m_bAutoPage;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotSourceInfoRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  PivotSourceInfoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotSourceInfoRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Count (1-based) of DCONREF or DCONNAME records that follow the SXTBL record.
    /// </summary>
    public ushort RefCount
    {
      get
      {
        return m_usRefCount;
      }
      set
      {
        m_usRefCount = value;
      }
    }
    /// <summary>
    /// Count (1-based) of SXTBPG records that follow the DCONREF or DCONNAME records.
    /// </summary>
    public ushort PageItemCount
    {
      get
      {
        return m_usPageItemCount;
      }
      set
      {
        m_usPageItemCount = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    /// <summary>
    /// Indicates whether the user selected the Create A Single Page Field For Me
    /// option in PivotTable Wizard dialog box.
    /// </summary>
    public bool IsAutoPage
    {
      get
      {
        return m_bAutoPage;
      }
      set
      {
        m_bAutoPage = value;
      }
    }
    /// <summary>
    /// Count (1-based) of page fields.
    /// </summary>
    public ushort PageCount
    {
      get
      {
        return GetUInt16BitsByMask( m_usOptions, DEF_BITMASK_PAGECOUNT );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, DEF_BITMASK_PAGECOUNT, value );
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
      m_usRefCount = provider.ReadUInt16( iOffset + 0 );
      m_usPageItemCount = provider.ReadUInt16( iOffset + 2 );
      m_usOptions = provider.ReadUInt16( iOffset + 4 );
      m_bAutoPage = provider.ReadBit( iOffset + 5, 7 );
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
      provider.WriteUInt16( iOffset + 0, m_usRefCount );
      provider.WriteUInt16( iOffset + 2, m_usPageItemCount );
      provider.WriteUInt16( iOffset + 4, m_usOptions );
      provider.WriteBit( iOffset + 5, m_bAutoPage, 7 );
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
