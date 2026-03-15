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
  /// Summary description for PivotNamePairRecord.
  /// </summary>
  [ Biff( TBIFFRecord.PivotNamePair ) ]
  [ CLSCompliant( false ) ]
  public class PivotNamePairRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Field.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usField;
    /// <summary>
    /// Index of item in field.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usCache;
    /// <summary>
    /// Reserved, should be set to zero.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usReserved;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bCalculatedItem;
    /// <summary>
    /// Indicates whether item is referred to by position (physical)
    /// rather than by name (logical).
    /// </summary>
    [ BiffRecordPos( 6, 3, TFieldType.Bit ) ]
    private bool m_bPhysical;
    /// <summary>
    /// If m_bPhysical is true, then item is referred to using relative
    /// references rather than absolute references.
    /// </summary>
    [ BiffRecordPos( 6, 4, TFieldType.Bit ) ]
    private bool m_bRelative;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotNamePairRecord()
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
    public  PivotNamePairRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotNamePairRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Field.
    /// </summary>
    public ushort Field
    {
      get
      {
        return m_usField;
      }
      set
      {
        m_usField = value;
      }
    }
    /// <summary>
    /// Index of item in field.
    /// </summary>
    public ushort Cache
    {
      get
      {
        return m_usCache;
      }
      set
      {
        m_usCache = value;
      }
    }
    /// <summary>
    /// Reserved, should be set to zero. Read-only.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
#if DEBUG
      set
      {
        m_usReserved = value;
      }
#endif
    }
    /// <summary>
    /// Option flags.
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
    /// 
    /// </summary>
    public bool IsCalculatedItem
    {
      get
      {
        return m_bCalculatedItem;
      }
      set
      {
        m_bCalculatedItem = value;
      }
    }
    /// <summary>
    /// Indicates whether item is referred to by position (physical)
    /// rather than by name (logical).
    /// </summary>
    public bool IsPhysical
    {
      get
      {
        return m_bPhysical;
      }
      set
      {
        m_bPhysical = value;
      }
    }
    /// <summary>
    /// Indicates whether item is referred to using relative
    /// references rather than absolute references.
    /// </summary>
    public bool IsRelative
    {
      get
      {
        return m_bRelative;
      }
      set
      {
        m_bRelative = value;
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
      m_usField = provider.ReadUInt16( iOffset + 0 );
      m_usCache = provider.ReadUInt16( iOffset + 2 );
      m_usReserved = provider.ReadUInt16( iOffset + 4 );
      m_usOptions = provider.ReadUInt16( iOffset + 6 );
      m_bCalculatedItem = provider.ReadBit( iOffset + 6, 0 );
      m_bPhysical = provider.ReadBit( iOffset + 6, 3 );
      m_bRelative = provider.ReadBit( iOffset + 6, 4 );
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
      provider.WriteUInt16( iOffset + 0, m_usField );
      provider.WriteUInt16( iOffset + 2, m_usCache );
      provider.WriteUInt16( iOffset + 4, m_usReserved );
      provider.WriteUInt16( iOffset + 6, m_usOptions );
      provider.WriteBit( iOffset + 6, m_bCalculatedItem, 0 );
      provider.WriteBit( iOffset + 6, m_bPhysical, 3 );
      provider.WriteBit( iOffset + 6, m_bRelative, 4 );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
