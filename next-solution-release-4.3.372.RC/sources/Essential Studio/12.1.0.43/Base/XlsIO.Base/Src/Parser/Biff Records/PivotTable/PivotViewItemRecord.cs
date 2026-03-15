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
  /// This record contains information about a PivotTable item.
  /// </summary>
  [ Biff( TBIFFRecord.PivotViewItem ) ]
  [ CLSCompliant( false ) ]
  public class PivotViewItemRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Offset to the name data.
    /// </summary>
    private const int DEF_NAME_OFFSET = 8;
    /// <summary>
    /// Possible item types.
    /// </summary>
    public enum ItemTypes
    {
      /// <summary>
      /// Represents the  item type.
      /// </summary>
      Page        = 0xFE,
      /// <summary>
      /// Represents the  item type.
      /// </summary>
      Null        = 0xFF,
      /// <summary>
      /// Represents the Data item type.
      /// </summary>
      Data        = 0,
      /// <summary>
      /// Represents the Default item type.
      /// </summary>
      Default     = 1,
      /// <summary>
      /// Represents the Sum item type.
      /// </summary>
      Sum         = 2,
      /// <summary>
      /// Represents the Counta item type.
      /// </summary>
      Counta      = 3,
      /// <summary>
      /// Represents the Count item type.
      /// </summary>
      Count       = 4,
      /// <summary>
      /// Represents the Average item type.
      /// </summary>
      Average     = 5,
      /// <summary>
      /// Represents the Max item type.
      /// </summary>
      Max         = 6,
      /// <summary>
      /// Represents the Min item type.
      /// </summary>
      Min         = 7,
      /// <summary>
      /// Represents the Product item type.
      /// </summary>
      Product     = 8,
      /// <summary>
      /// Represents the Stdev item type.
      /// </summary>
      Stdev       = 9,
      /// <summary>
      /// Represents the Stdevp item type.
      /// </summary>
      Stdevp      = 10,
      /// <summary>
      /// Represents the Var item type.
      /// </summary>
      Var         = 11,
      /// <summary>
      /// Represents the Varp item type.
      /// </summary>
      Varp        = 12,
      /// <summary>
      /// Represents the GrandTotal item type.
      /// </summary>
      GrandTotal  = 13,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Item type.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usItemType;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions;

    /// <summary>
    /// Indicates whether the item is hidden.
    /// </summary>
    [ BiffRecordPos( 2, 0, TFieldType.Bit ) ]
    private bool m_bHidden;
    /// <summary>
    /// Indicates whether detail is hidden.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.Bit ) ]
    private bool m_bHideDetail;
    /// <summary>
    /// Indicates whether the item is a calculated item.
    /// </summary>
    [ BiffRecordPos( 2, 2, TFieldType.Bit ) ]
    private bool m_bFormula;
    /// <summary>
    /// Indicates whether item is an item that does not exist in any records.
    /// </summary>
    [ BiffRecordPos( 2, 3, TFieldType.Bit ) ]
    private bool m_bMissing;

    /// <summary>
    /// Index to the PivotTable cache.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usCache;
    /// <summary>
    /// Length of the name. If it is equal to 0xFFFF, then name is null
    /// and the name in the cache is used.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usNameLength;
    /// <summary>
    /// Name.
    /// </summary>
    private string m_strName;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotViewItemRecord()
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
    public  PivotViewItemRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotViewItemRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Item type.
    /// </summary>
    public ItemTypes ItemType
    {
      get
      {
        return ( ItemTypes )m_usItemType;
      }
      set
      {
        m_usItemType = ( ushort )value;
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
    /// Indicates whether the item is hidden.
    /// </summary>
    public bool IsHidden
    {
      get
      {
        return m_bHidden;
      }
      set
      {
        m_bHidden = value;
      }
    }
    /// <summary>
    /// Indicates whether detail is hidden.
    /// </summary>
    public bool IsHideDetail
    {
      get
      {
        return m_bHideDetail;
      }
      set
      {
        m_bHideDetail = value;
      }
    }
    /// <summary>
    /// Indicates whether the item is calculated item.
    /// </summary>
    public bool IsFormula
    {
      get
      {
        return m_bFormula;
      }
      set
      {
        m_bFormula = value;
      }
    }
    /// <summary>
    /// Indicates whether item is an item that does not exist in any records.
    /// </summary>
    public bool IsMissing
    {
      get
      {
        return m_bMissing;
      }
      set
      {
        m_bMissing = value;
      }
    }

    /// <summary>
    /// Index to the PivotTable cache.
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
    /// Length of the name. If it is equal to 0xFFFF, then name is null
    /// and the name in the cache is used. Read-only.
    /// </summary>
    public ushort NameLength
    {
      get
      {
        return m_usNameLength;
      }
#if DEBUG
      set
      {
        m_usNameLength = value;
      }
#endif
    }
    /// <summary>
    /// Name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null )
        {
          m_usNameLength = DataItemRecord.DEF_NULL_NAME_LENGTH;
        }
        else
        {
          if( value.Length > DataItemRecord.DEF_NULL_NAME_LENGTH )
            throw new ArgumentOutOfRangeException( "value.Length", "Value cannot be greater DataItemRecord.DEF_NULL_NAME_LENGTH" );

          m_usNameLength = ( ushort )value.Length;
        }

        m_strName = value;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_usItemType = GetUInt16( 0 );
      m_usOptions = GetUInt16( 2 );
      m_bHidden = GetBit( 2, 0 );
      m_bHideDetail = GetBit( 2, 1 );
      m_bFormula = GetBit( 2, 2 );
      m_bMissing = GetBit( 2, 3 );
      m_usCache = GetUInt16( 4 );
      m_usNameLength = GetUInt16( 6 );

      m_strName = null;

      if( m_usNameLength != DataItemRecord.DEF_NULL_NAME_LENGTH )
      {
        m_strName = GetString( DEF_NAME_OFFSET, m_usNameLength );
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = DEF_NAME_OFFSET;
      m_data = new byte[ m_iLength ];

      SetUInt16( 0, m_usItemType );
      SetUInt16( 2, m_usOptions );
      SetBit( 2, m_bHidden, 0 );
      SetBit( 2, m_bHideDetail, 1 );
      SetBit( 2, m_bFormula, 2 );
      SetBit( 2, m_bMissing, 3 );
      SetUInt16( 4, m_usCache );
      SetUInt16( 6, m_usNameLength );

      if( m_strName != null )
      {
        m_iLength += SetStringNoLen( DEF_NAME_OFFSET, m_strName );
      }
    }
    #endregion
  }
}
