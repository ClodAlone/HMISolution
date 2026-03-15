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
  /// Summary description for DataItem.
  /// </summary>
  [ Biff( TBIFFRecord.DataItem ) ]
  [ CLSCompliant( false ) ]
  public class DataItemRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Value for name length indicating that name is Null.
    /// </summary>
    internal const ushort DEF_NULL_NAME_LENGTH = 0xFFFF;
    /// <summary>
    /// Offset to the Name property.
    /// </summary>
    private const int DEF_STRING_OFFSET = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Field that this data item is based on.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usField;
    /// <summary>
    /// Index to the aggregation function:
    /// 0 - Sum,
    /// 1 - Count,
    /// 2 - Average,
    /// 3 - Max,
    /// 4 - Min,
    /// 5 - Product,
    /// 6 - Count Nums,
    /// 7 - StdDev,
    /// 8 - StdDevp,
    /// 9 - Var,
    /// 10 - Varp,
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFunctionIndex;
    /// <summary>
    /// Data display format:
    /// 0 - Normal,
    /// 1 - Difference from,
    /// 2 - Percentage of,
    /// 3 - Percentage difference from,
    /// 4 - Running total in,
    /// 5 - Percentage of row,
    /// 6 - Percentage of column,
    /// 7 - Percentage of total,
    /// 8 - Index.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usDisplayFormat;
    /// <summary>
    /// Index to the SXVD record used by the data display format.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usViewFieldIndex;
    /// <summary>
    /// Index to the SXVI record used by the data display format.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usViewItemIndex;
    /// <summary>
    /// Index to the format table for this item.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usFormatTableIndex;
    /// <summary>
    /// Length of the name; if it is equal to 0xFFFF, then name is null
    /// and the name in the PivotTable cache storage is used.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usNameLength = DEF_NULL_NAME_LENGTH;
    /// <summary>
    /// Name of the item.
    /// </summary>
    private string m_strName;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  DataItemRecord()
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
    public  DataItemRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DataItemRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Field that this data item is based on.
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
    /// Index to the aggregation function:
    /// 0 - Sum,
    /// 1 - Count,
    /// 2 - Average,
    /// 3 - Max,
    /// 4 - Min,
    /// 5 - Product,
    /// 6 - Count Nums,
    /// 7 - StdDev,
    /// 8 - StdDevp,
    /// 9 - Var,
    /// 10 - Varp,
    /// </summary>
    public ushort FunctionIndex
    {
      get
      {
        return m_usFunctionIndex;
      }
      set
      {
        m_usFunctionIndex = value;
      }
    }
    /// <summary>
    /// Data display format:
    /// 0 - Normal,
    /// 1 - Diffrence from,
    /// 2 - Percentage of,
    /// 3 - Percentage difference from,
    /// 4 - Running total in,
    /// 5 - Percentage of row,
    /// 6 - Percentage of column,
    /// 7 - Percentage of total,
    /// 8 - Index.
    /// </summary>
    public ushort DisplayFormat
    {
      get
      {
        return m_usDisplayFormat;
      }
      set
      {
        m_usDisplayFormat = value;
      }
    }
    /// <summary>
    /// Index to the SXVD record used by the data display format.
    /// </summary>
    public ushort ViewFieldIndex
    {
      get
      {
        return m_usViewFieldIndex;
      }
      set
      {
        m_usViewFieldIndex = value;
      }
    }
    /// <summary>
    /// Index to the SXVI record used by the data display format.
    /// </summary>
    public ushort ViewItemIndex
    {
      get
      {
        return m_usViewItemIndex;
      }
      set
      {
        m_usViewItemIndex = value;
      }
    }
    /// <summary>
    /// Index to the format table for this item.
    /// </summary>
    public ushort FormatTableIndex
    {
      get
      {
        return m_usFormatTableIndex;
      }
      set
      {
        m_usFormatTableIndex = value;
      }
    }
    /// <summary>
    /// Length of the name; if it is equal to 0xFFFF, then name is null
    /// and the name in the PivotTable cache storage is used. Read-only.
    /// </summary>
    public ushort NameLength
    {
      get
      {
        return m_usNameLength;
      }
    }
    /// <summary>
    /// Name of the item.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;

        if( value == null )
        {
          m_usNameLength = DEF_NULL_NAME_LENGTH;
        }
        else
        {
          m_usNameLength = ( ushort )value.Length;
        }
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
      m_usField = GetUInt16( 0 );
      m_usFunctionIndex = GetUInt16( 2 );
      m_usDisplayFormat = GetUInt16( 4 );
      m_usViewFieldIndex = GetUInt16( 6 );
      m_usViewItemIndex = GetUInt16( 8 );
      m_usFormatTableIndex = GetUInt16( 10 );
      m_usNameLength = GetUInt16( 12 );

      if( m_usNameLength == DEF_NULL_NAME_LENGTH )
      {
        m_strName = null;
      }
      else
      {
        m_strName = GetString( DEF_STRING_OFFSET, m_usNameLength );
      }
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = 14;
      m_data = new byte[ m_iLength ];
      SetUInt16( 0, m_usField );
      SetUInt16( 2, m_usFunctionIndex );
      SetUInt16( 4, m_usDisplayFormat );
      SetUInt16( 6, m_usViewFieldIndex );
      SetUInt16( 8, m_usViewItemIndex );
      SetUInt16( 10, m_usFormatTableIndex );
      SetUInt16( 12, m_usNameLength );

      if( m_strName != null )
      {
        AutoGrowData = true;
        bool bCompressed = IsAsciiString(m_strName);
        m_iLength += SetStringNoLen( m_iLength, m_strName, false, bCompressed );
      }
    }

    #endregion
  }
}
