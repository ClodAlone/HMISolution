#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;
using System.Text;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record contains extended PivotTable view fields information.
  /// </summary>
  [ Biff( TBIFFRecord.PivotViewFieldsEx ) ]
  [ CLSCompliant( false ) ]
  public class PivotViewFieldsExRecord : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private ushort m_usOptions;

    /// <summary>
    /// Show all items for this field.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bShowAllItems;
    /// <summary>
    /// User can drag field to row area.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bDragToRow;
    /// <summary>
    /// User can drag field to column area.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bDragToColumn;
    /// <summary>
    /// User can drag field to page area.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bDragToPage;
    /// <summary>
    /// User can remove field from view.
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bDragToHide;
    /// <summary>
    /// This field is a server-based field in the page area.
    /// </summary>
    [ BiffRecordPos( 0, 7, TFieldType.Bit ) ]
    private bool m_bServerBased;
    /// <summary>
    /// Autosort is enabled.
    /// </summary>
    [ BiffRecordPos( 1, 1, TFieldType.Bit ) ]
    private bool m_bAutoSort;
    /// <summary>
    /// Autosort ascending.
    /// </summary>
    [ BiffRecordPos( 1, 2, TFieldType.Bit ) ]
    private bool m_bAscendSort;
    /// <summary>
    /// Autoshow is enabled.
    /// </summary>
    [ BiffRecordPos( 1, 3, TFieldType.Bit ) ]
    private bool m_bAutoShow;
    /// <summary>
    /// Show top values.
    /// </summary>
    [ BiffRecordPos( 1, 4, TFieldType.Bit ) ]
    private bool m_bAscendShow;
    /// <summary>
    /// Calculated field.
    /// </summary>
    [ BiffRecordPos( 1, 5, TFieldType.Bit ) ]
    private bool m_bCalculateField;

    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte m_btReserved;
    /// <summary>
    /// Number of items to show for AutoShow, default is 10.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private byte m_btItemsNumber = 10;
    /// <summary>
    /// 0-based index of data field that AutoSort is based on or 0xFFFF for current field.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usSortIndex;
    /// <summary>
    /// 0-based index of data field that AutoShow is based on.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usShowIndex;
    /// <summary>
    /// Number format of field or 0 if none.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usFormat;
    /// <summary>
    /// Subtotal name.
    /// </summary>
    private string m_strSubTotalName;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotViewFieldsExRecord()
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
    public  PivotViewFieldsExRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotViewFieldsExRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
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
    /// Show all items for this field.
    /// </summary>
    public bool IsShowAllItems
    {
      get
      {
        return m_bShowAllItems;
      }
      set
      {
        m_bShowAllItems = value;
      }
    }
    /// <summary>
    /// User can drag field to row area.
    /// </summary>
    public bool IsDragToRow
    {
      get
      {
        return m_bDragToRow;
      }
      set
      {
        m_bDragToRow = value;
      }
    }
    /// <summary>
    /// User can drag field to column area.
    /// </summary>
    public bool IsDragToColumn
    {
      get
      {
        return m_bDragToColumn;
      }
      set
      {
        m_bDragToColumn = value;
      }
    }
    /// <summary>
    /// User can drag field to page area.
    /// </summary>
    public bool IsDragToPage
    {
      get
      {
        return m_bDragToPage;
      }
      set
      {
        m_bDragToPage = value;
      }
    }
    /// <summary>
    /// User can remove field from fiew.
    /// </summary>
    public bool IsDragToHide
    {
      get
      {
        return m_bDragToHide;
      }
      set
      {
        m_bDragToHide = value;
      }
    }
    /// <summary>
    /// This field is a server-based field in the page area.
    /// </summary>
    public bool IsServerBased
    {
      get
      {
        return m_bServerBased;
      }
      set
      {
        m_bServerBased = value;
      }
    }
    /// <summary>
    /// Autosort is enabled.
    /// </summary>
    public bool IsAutoSort
    {
      get
      {
        return m_bAutoSort;
      }
      set
      {
        m_bAutoSort = value;
      }
    }
    /// <summary>
    /// Autosort ascending.
    /// </summary>
    public bool IsAscendSort
    {
      get
      {
        return m_bAscendSort;
      }
      set
      {
        m_bAscendSort = value;
      }
    }
    /// <summary>
    /// Autoshow is enabled.
    /// </summary>
    public bool IsAutoShow
    {
      get
      {
        return m_bAutoShow;
      }
      set
      {
        m_bAutoShow = value;
      }
    }
    /// <summary>
    /// Show top values.
    /// </summary>
    public bool IsAscendShow
    {
      get
      {
        return m_bAscendShow;
      }
      set
      {
        m_bAscendShow = value;
      }
    }
    /// <summary>
    /// Calculated field.
    /// </summary>
    public bool IsCalculateField
    {
      get
      {
        return m_bCalculateField;
      }
      set
      {
        m_bCalculateField = value;
      }
    }

    /// <summary>
    /// Reserved. Read-only.
    /// </summary>
    public byte Reserved
    {
      get
      {
        return m_btReserved;
      }
#if DEBUG
      set
      {
        m_btReserved = value;
      }
#endif
    }
    /// <summary>
    /// Number of items to show for AutoShow, default is 10.
    /// </summary>
    public byte ItemsNumber
    {
      get
      {
        return m_btItemsNumber;
      }
      set
      {
        m_btItemsNumber = value;
      }
    }
    /// <summary>
    /// 0-based index of data field that AutoSort is based on or 0xFFFF for current field.
    /// </summary>
    public ushort SortIndex
    {
      get
      {
        return m_usSortIndex;
      }
      set
      {
        m_usSortIndex = value;
      }
    }
    /// <summary>
    /// 0-based index of data field that AutoShow is based on.
    /// </summary>
    public ushort ShowIndex
    {
      get
      {
        return m_usShowIndex;
      }
      set
      {
        m_usShowIndex = value;
      }
    }
    /// <summary>
    /// Number format of field or 0 if none.
    /// </summary>
    public ushort NumberFormat
    {
      get
      {
        return m_usFormat;
      }
      set
      {
        m_usFormat = value;
      }
    }
    /// <summary>
    /// Specifies the custom text that is displayed for the subtotals label.
    /// </summary>
    public string SubTotalName
    {
        get
        {
            return m_strSubTotalName;
        }
        set
        {
            m_strSubTotalName = value;
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
      m_usOptions = GetUInt16( 0 );
      m_bShowAllItems = GetBit( 0, 0 );
      m_bDragToRow = GetBit( 0, 1 );
      m_bDragToColumn = GetBit( 0, 2 );
      m_bDragToPage = GetBit( 0, 3 );
      m_bDragToHide = GetBit( 0, 4 );
      m_bServerBased = GetBit( 0, 7 );
      m_bAutoSort = GetBit( 1, 1 );
      m_bAscendSort = GetBit( 1, 2 );
      m_bAutoShow = GetBit( 1, 3 );
      m_bAscendShow = GetBit( 1, 4 );
      m_bCalculateField = GetBit( 1, 5 );
      m_btReserved = GetByte( 2 );
      m_btItemsNumber = GetByte( 3 );
      m_usSortIndex = GetUInt16( 4 );
      m_usShowIndex = GetUInt16( 6 );
      m_usFormat = GetUInt16( 8 );

      int iOffset = 10;
      if( m_iLength > iOffset )
      {
        int iSubTotalLength = GetInt16( iOffset );
        iOffset += 2;
        iOffset += 8; // skip reserved fields.
        int iSizeLeft = m_iLength - iOffset;

        if( iSubTotalLength > 0 )
        {
          byte btUnicode = m_data[ iOffset ];
          iOffset++;

          Encoding encoding = ( btUnicode != 0 ) ?
            Encoding.Unicode :
#if !SILVERLIGHT && !WINRT && !WP
            Encoding.Default;
#else
            Encoding.UTF8;
#endif
          if( btUnicode != 0 )
            iSubTotalLength *= 2;

          m_strSubTotalName = encoding.GetString( m_data, iOffset, iSubTotalLength );
        }
      }

    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = 10;
      m_data = new byte[ m_iLength ];
      SetUInt16( 0, m_usOptions );
      SetBit( 0, m_bShowAllItems, 0 );
      SetBit( 0, m_bDragToRow, 1 );
      SetBit( 0, m_bDragToColumn, 2 );
      SetBit( 0, m_bDragToPage, 3 );
      SetBit( 0, m_bDragToHide, 4 );
      SetBit( 0, m_bServerBased, 7 );
      SetBit( 1, m_bAutoSort, 1 );
      SetBit( 1, m_bAscendSort, 2 );
      SetBit( 1, m_bAutoShow, 3 );
      SetBit( 1, m_bAscendShow, 4 );
      SetBit( 1, m_bCalculateField, 5 );
      SetByte( 2, m_btReserved );
      SetByte( 3, m_btItemsNumber );
      SetUInt16( 4, m_usSortIndex );
      SetUInt16( 6, m_usShowIndex );
      SetUInt16( 8, m_usFormat );

      int iLength = ( m_strSubTotalName != null ) ?
        m_strSubTotalName.Length :
        0;
      AutoGrowData = true;
      if (iLength == 0)
          SetUInt16(m_iLength, ushort.MaxValue);
      else
          SetUInt16(m_iLength, (ushort)iLength);
      m_iLength += 2;

      SetInt32( m_iLength, 0 );
      m_iLength += 4;

      SetInt32( m_iLength, 0 );
      m_iLength += 4;

      if( m_strSubTotalName != null && m_strSubTotalName.Length > 0 )
      {
        SetByte( m_iLength, 1 );
        m_iLength++;

        byte[] arrData = Encoding.Unicode.GetBytes( m_strSubTotalName );
        SetBytes( m_iLength, arrData );
        m_iLength += arrData.Length;
      }
    }

    #endregion
  }
}
