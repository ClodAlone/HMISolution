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
  /// This record is the list header of the Data Validity Table in the current sheet.
  /// </summary>
  [ Biff( TBIFFRecord.DVal ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DValRecord : BiffRecordRaw
  {
    #region Class members

    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions = 0;

    #region Option flags bit fields
    /// <summary>
    /// False if the prompt box is not visible.
    /// True if the prompt box is currently visible.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bPromtBoxVisible = false;

    /// <summary>
    /// False if the prompt box has a fixed position.
    /// True if the prompt box appears at a cell.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bPromtBoxPosFixed = false;

    /// <summary>
    /// True if cell validity data is cached in DV records.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bDataCached = false;
    #endregion

    /// <summary>
    /// Horizontal position of the prompt box, if it has a fixed position, in pixels.
    /// </summary>
    [ BiffRecordPos( 2, 4, true ) ]
    private int m_iPromtBoxHPos = 0;

    /// <summary>
    /// Vertical position of the prompt box, if it has a fixed position, in pixels.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_iPromtBoxVPos = 0;

    /// <summary>
    /// Object identifier of the drop-down arrow object for a list box,
    /// If a list box is visible at the current cursor position; otherwise FFFFFFFFH.
    /// </summary>
    [ BiffRecordPos( 10, 4 ) ]
    private uint m_uiObjectId = 0xFFFFFFFF;

    /// <summary>
    /// Number of DV records.
    /// </summary>
    [ BiffRecordPos( 14, 4 ) ]
    private uint m_uiDVNumber = 0;
    #endregion

    #region Class properties

    /// <summary>
    /// Read-only. Option flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }


    #region Option flags bit fields
    /// <summary>
    /// False if prompt box is invisible.
    /// True if prompt box is currently visible.
    /// </summary>
    public bool IsPromtBoxVisible
    {
      get
      {
        return m_bPromtBoxVisible;
      }
      set
      {
        m_bPromtBoxVisible = value;
      }
    }

    /// <summary>
    /// False if prompt box has fixed position.
    /// True if prompt box appears at cell.
    /// </summary>
    public bool IsPromtBoxPosFixed
    {
      get
      {
        return m_bPromtBoxPosFixed;
      }
      set
      {
        m_bPromtBoxPosFixed = value;
      }
    }

    /// <summary>
    /// True if cell validity data is cached in DV records.
    /// </summary>
    public bool IsDataCached
    {
      get
      {
        return m_bDataCached;
      }
      set
      {
        m_bDataCached = value;
      }
    }
    #endregion

    /// <summary>
    /// Horizontal position of the prompt box, if it has a fixed position, in pixels.
    /// </summary>
    public int PromtBoxHPos
    {
      get
      {
        return m_iPromtBoxHPos;
      }
      set
      {
        m_iPromtBoxHPos = value;
      }
    }

    /// <summary>
    /// Vertical position of the prompt box, if it has a fixed position, in pixels.
    /// </summary>
    public int PromtBoxVPos
    {
      get
      {
        return m_iPromtBoxVPos;
      }
      set
      {
        m_iPromtBoxVPos = value;
      }
    }

    /// <summary>
    /// Object identifier of the drop-down arrow object for a list box,
    /// if a list box is visible at the current cursor position; otherwise FFFFFFFFH.
    /// </summary>
    public uint ObjectId
    {
      get
      {
        return m_uiObjectId;
      }
      set
      {
        m_uiObjectId = value;
      }
    }

    /// <summary>
    /// Number of DV records.
    /// </summary>
    public uint DVNumber
    {
      get
      {
        return m_uiDVNumber;
      }
      set
      {
        m_uiDVNumber = value;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 18;
      }
    }

    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return 18;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  DValRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  DValRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DValRecord( int iReserve )
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
      m_usOptions = provider.ReadUInt16( iOffset );
      m_bPromtBoxVisible = provider.ReadBit( iOffset, 0 );
      m_bPromtBoxPosFixed = provider.ReadBit( iOffset, 1 );
      m_bDataCached = provider.ReadBit( iOffset, 2 );
      iOffset += 2;

      m_iPromtBoxHPos = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iPromtBoxVPos = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_uiObjectId = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiDVNumber = provider.ReadUInt32( iOffset );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bPromtBoxVisible, 0 );
      provider.WriteBit( iOffset, m_bPromtBoxPosFixed, 1 );
      provider.WriteBit( iOffset, m_bDataCached, 2 );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iPromtBoxHPos );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iPromtBoxVPos );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiObjectId );
      iOffset+= 4;

      provider.WriteUInt32( iOffset, m_uiDVNumber );
    }

    #endregion

    #region Class Overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      DValRecord dval = obj as DValRecord;
      
      if( dval == null ) return false;

      return ( dval.IsPromtBoxVisible == IsPromtBoxVisible
        && dval.IsPromtBoxPosFixed == IsPromtBoxPosFixed
        && dval.IsDataCached == IsDataCached
        && dval.PromtBoxHPos == PromtBoxHPos
        && dval.PromtBoxVPos == PromtBoxVPos
        && dval.ObjectId == ObjectId );

      // We shouldn't compare number of DVRecords only other fields.
        //&& dval.DVNumber == DVNumber );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return m_bDataCached.GetHashCode()
        + m_bPromtBoxPosFixed.GetHashCode()
        + m_bPromtBoxPosFixed.GetHashCode()
        + m_iPromtBoxHPos.GetHashCode()
        + m_iPromtBoxVPos.GetHashCode()
        + m_uiObjectId.GetHashCode();
    }

    #endregion
  }
}
