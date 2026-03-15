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
  /// This record contains top-level PivotTable information.
  /// </summary>
  [ Biff( TBIFFRecord.PivotViewDefinition ) ]
  [ CLSCompliant( false ) ]
  public class PivotViewDefinitionRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Offset to the PivotTable name data.
    /// </summary>
    private const int DEF_TABLE_NAME_OFFSET = 44;
    #endregion

    #region Class members
    /// <summary>
    /// First row of the PivotTable.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFirstRow;
    /// <summary>
    /// Last row of the PivotTable.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLastRow;
    /// <summary>
    /// First column of the PivotTable.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usFirstColumn;
    /// <summary>
    /// Last column of the PivotTable.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usLastColumn;
    /// <summary>
    /// First row containing PivotTable headings.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usFirstHeadRow;
    /// <summary>
    /// First row containing PivotTable data.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usFirstDataRow;
    /// <summary>
    /// First column containing PivotTable data.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usFirstDataColumn;
    /// <summary>
    /// Index to the cache.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usCacheIndex;
    /// <summary>
    /// Reserved. Must be zero.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usReserved;
    /// <summary>
    /// Default axis for a data field.
    /// </summary>
    [ BiffRecordPos( 18, 2 ) ]
    private ushort m_usDataAxis;
    /// <summary>
    /// Default position for a data field.
    /// </summary>
    [ BiffRecordPos( 20, 2 ) ]
    private ushort m_usDataPos;
    /// <summary>
    /// Number of fields.
    /// </summary>
    [ BiffRecordPos( 22, 2 ) ]
    private ushort m_usFieldsNumber;
    /// <summary>
    /// Number of row fields.
    /// </summary>
    [ BiffRecordPos( 24, 2 ) ]
    private ushort m_usRowFieldsNumber;
    /// <summary>
    /// Number of column fields.
    /// </summary>
    [ BiffRecordPos( 26, 2 ) ]
    private ushort m_usColumnFieldsNumber;
    /// <summary>
    /// Number of page fields.
    /// </summary>
    [ BiffRecordPos( 28, 2 ) ]
    private ushort m_usPageFieldsNumber;
    /// <summary>
    /// Number of data fields.
    /// </summary>
    [ BiffRecordPos( 30, 2 ) ]
    private ushort m_usDataFieldsNumber;
    /// <summary>
    /// Number of data rows.
    /// </summary>
    [ BiffRecordPos( 32, 2 ) ]
    private ushort m_usDataRowsNumber;
    /// <summary>
    /// Number of data columns.
    /// </summary>
    [ BiffRecordPos( 34, 2 ) ]
    private ushort m_usDataColumnsNumber;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 36, 2 ) ]
    private ushort m_usOptions;

    /// <summary>
    /// Indicates whether the PivotTable contains grand totals for rows.
    /// </summary>
    [ BiffRecordPos( 36, 0, TFieldType.Bit ) ]
    private bool m_bRowGrand;
    /// <summary>
    /// Indicates whether the PivotTable contains grand totals for columns.
    /// </summary>
    [ BiffRecordPos( 36, 1, TFieldType.Bit ) ]
    private bool m_bColumnGrand;
    /// <summary>
    /// Indicates whether the PivotTable has an autoformat applied.
    /// </summary>
    [ BiffRecordPos( 36, 3, TFieldType.Bit ) ]
    private bool m_bAutoFormat;
    /// <summary>
    /// Indicates whether the width / height autoformat is applied.
    /// </summary>
    [ BiffRecordPos( 36, 4, TFieldType.Bit ) ]
    private bool m_bWHAutoFormat;
    /// <summary>
    /// Indicates whether the font autoformat is applied.
    /// </summary>
    [ BiffRecordPos( 36, 5, TFieldType.Bit ) ]
    private bool m_bFontAutoFormat;
    /// <summary>
    /// Indicates whether the alignment autoformat is applied.
    /// </summary>
    [ BiffRecordPos( 36, 6, TFieldType.Bit ) ]
    private bool m_bAlignAutoFormat;
    /// <summary>
    /// Indicates whether the Border autoformat is applied.
    /// </summary>
    [ BiffRecordPos( 36, 7, TFieldType.Bit ) ]
    private bool m_bBorderAutoFormat;
    /// <summary>
    /// Indicates whether the pattern autoformat is applied.
    /// </summary>
    [ BiffRecordPos( 37, 0, TFieldType.Bit ) ]
    private bool m_bPatternAutoFormat;
    /// <summary>
    /// Indicates whether the PivotTable has an autoformat applied.
    /// </summary>
    [ BiffRecordPos( 37, 1, TFieldType.Bit ) ]
    private bool m_bNumberAutoFormat;

    /// <summary>
    /// Index to the PivotTable autoformat.
    /// </summary>
    [ BiffRecordPos( 38, 2 ) ]
    private ushort m_usAutoFormatIndex;
    /// <summary>
    /// Length of the PivotTable name.
    /// </summary>
    [ BiffRecordPos( 40, 2 ) ]
    private ushort m_usTableNameLength;
    /// <summary>
    /// Length of the data field name.
    /// </summary>
    [ BiffRecordPos( 42, 2 ) ]
    private ushort m_usDataFieldNameLength;

    /// <summary>
    /// PivotTable name.
    /// </summary>
    private string m_strTableName;
    /// <summary>
    /// Name of the data field.
    /// </summary>
    private string m_strDataFieldName;

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotViewDefinitionRecord()
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
    public  PivotViewDefinitionRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotViewDefinitionRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// First row of the PivotTable.
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
    /// Last row of the PivotTable.
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
    /// First column of the PivotTable.
    /// </summary>
    public ushort FirstColumn
    {
      get
      {
        return m_usFirstColumn;
      }
      set
      {
        m_usFirstColumn = value;
      }
    }
    /// <summary>
    /// Last column of the PivotTable.
    /// </summary>
    public ushort LastColumn
    {
      get
      {
        return m_usLastColumn;
      }
      set
      {
        m_usLastColumn = value;
      }
    }
    /// <summary>
    /// First row containing PivotTable headings.
    /// </summary>
    public ushort FirstHeadRow
    {
      get
      {
        return m_usFirstHeadRow;
      }
      set
      {
        m_usFirstHeadRow = value;
      }
    }
    /// <summary>
    /// First row containing PivotTable data.
    /// </summary>
    public ushort FirstDataRow
    {
      get
      {
        return m_usFirstDataRow;
      }
      set
      {
        m_usFirstDataRow = value;
      }
    }
    /// <summary>
    /// First column containing PivotTable data.
    /// </summary>
    public ushort FirstDataColumn
    {
      get
      {
        return m_usFirstDataColumn;
      }
      set
      {
        m_usFirstDataColumn = value;
      }
    }
    /// <summary>
    /// Index to the cache.
    /// </summary>
    public ushort CacheIndex
    {
      get
      {
        return m_usCacheIndex;
      }
      set
      {
        m_usCacheIndex = value;
      }
    }
    /// <summary>
    /// Reserved. Must be zero. Read-only.
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
    /// Default axis for a data field.
    /// </summary>
    public ushort DataAxis
    {
      get
      {
        return m_usDataAxis;
      }
      set
      {
        m_usDataAxis = value;
      }
    }
    /// <summary>
    /// Default position for a data field.
    /// </summary>
    public ushort DataPos
    {
      get
      {
        return m_usDataPos;
      }
      set
      {
        m_usDataPos = value;
      }
    }
    /// <summary>
    /// Number of fields.
    /// </summary>
    public ushort FieldsNumber
    {
      get
      {
        return m_usFieldsNumber;
      }
      set
      {
        m_usFieldsNumber = value;
      }
    }
    /// <summary>
    /// Number of row fields.
    /// </summary>
    public ushort RowFieldsNumber
    {
      get
      {
        return m_usRowFieldsNumber;
      }
      set
      {
        m_usRowFieldsNumber = value;
      }
    }
    /// <summary>
    /// Number of column fields.
    /// </summary>
    public ushort ColumnFieldsNumber
    {
      get
      {
        return m_usColumnFieldsNumber;
      }
      set
      {
        m_usColumnFieldsNumber = value;
      }
    }
    /// <summary>
    /// Number of page fields.
    /// </summary>
    public ushort PageFieldsNumber
    {
      get
      {
        return m_usPageFieldsNumber;
      }
      set
      {
        m_usPageFieldsNumber = value;
      }
    }
    /// <summary>
    /// Number of data fields.
    /// </summary>
    public ushort DataFieldsNumber
    {
      get
      {
        return m_usDataFieldsNumber;
      }
      set
      {
        m_usDataFieldsNumber = value;
      }
    }
    /// <summary>
    /// Number of data rows.
    /// </summary>
    public ushort DataRowsNumber
    {
      get
      {
        return m_usDataRowsNumber;
      }
      set
      {
        m_usDataRowsNumber = value;
      }
    }
    /// <summary>
    /// Number of data columns.
    /// </summary>
    public ushort DataColumnsNumber
    {
      get
      {
        return m_usDataColumnsNumber;
      }
      set
      {
        m_usDataColumnsNumber = value;
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
    /// Indicates whether the PivotTable contains grand totals for rows.
    /// </summary>
    public bool IsRowGrand
    {
      get
      {
        return m_bRowGrand;
      }
      set
      {
        m_bRowGrand = value;
      }
    }
    /// <summary>
    /// Indicates whether the PivotTable contains grand totals for columns.
    /// </summary>
    public bool IsColumnGrand
    {
      get
      {
        return m_bColumnGrand;
      }
      set
      {
        m_bColumnGrand = value;
      }
    }
    /// <summary>
    /// Indicates whether the PivotTable has an autoformat applied.
    /// </summary>
    public bool IsAutoFormat
    {
      get
      {
        return m_bAutoFormat;
      }
      set
      {
        m_bAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the width / height autoformat is applied.
    /// </summary>
    public bool IsWHAutoFormat
    {
      get
      {
        return m_bWHAutoFormat;
      }
      set
      {
        m_bWHAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the font autoformat is applied.
    /// </summary>
    public bool IsFontAutoFormat
    {
      get
      {
        return m_bFontAutoFormat;
      }
      set
      {
        m_bFontAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the alignment autoformat is applied.
    /// </summary>
    public bool IsAlignAutoFormat
    {
      get
      {
        return m_bAlignAutoFormat;
      }
      set
      {
        m_bAlignAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the Border autoformat is applied.
    /// </summary>
    public bool IsBorderAutoFormat
    {
      get
      {
        return m_bBorderAutoFormat;
      }
      set
      {
        m_bBorderAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the pattern autoformat is applied.
    /// </summary>
    public bool IsPatternAutoFormat
    {
      get
      {
        return m_bPatternAutoFormat;
      }
      set
      {
        m_bPatternAutoFormat = value;
      }
    }
    /// <summary>
    /// Indicates whether the PivotTable has an autoformat applied.
    /// </summary>
    public bool IsNumberAutoFormat
    {
      get
      {
        return m_bNumberAutoFormat;
      }
      set
      {
        m_bNumberAutoFormat = value;
      }
    }

    /// <summary>
    /// Index to the PivotTable autoformat.
    /// </summary>
    public ushort AutoFormatIndex
    {
      get
      {
        return m_usAutoFormatIndex;
      }
      set
      {
        m_usAutoFormatIndex = value;
      }
    }
    /// <summary>
    /// Length of the PivotTable name. Read-only.
    /// </summary>
    public ushort TableNameLength
    {
      get
      {
        return m_usTableNameLength;
      }
#if DEBUG
      set
      {
        m_usTableNameLength = value;
      }
#endif
    }
    /// <summary>
    /// Length of the data field name. Read-only.
    /// </summary>
    public ushort DataFieldNameLength
    {
      get
      {
        return m_usDataFieldNameLength;
      }
#if DEBUG
      set
      {
        m_usDataFieldNameLength = value;
      }
#endif
    }

    /// <summary>
    /// PivotTable name.
    /// </summary>
    public string TableName
    {
      get
      {
        return m_strTableName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strTableName = value;
        m_usTableNameLength = ( ushort )value.Length;
      }
    }
    /// <summary>
    /// Name of the data field.
    /// </summary>
    public string DataFieldName
    {
      get
      {
        return m_strDataFieldName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strDataFieldName = value;
        m_usDataFieldNameLength = ( ushort )value.Length;
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
      m_usFirstRow = GetUInt16( 0 );
      m_usLastRow = GetUInt16( 2 );
      m_usFirstColumn = GetUInt16( 4 );
      m_usLastColumn = GetUInt16( 6 );
      m_usFirstHeadRow = GetUInt16( 8 );
      m_usFirstDataRow = GetUInt16( 10 );
      m_usFirstDataColumn = GetUInt16( 12 );
      m_usCacheIndex = GetUInt16( 14 );
      m_usReserved = GetUInt16( 16 );
      m_usDataAxis = GetUInt16( 18 );
      m_usDataPos = GetUInt16( 20 );
      m_usFieldsNumber = GetUInt16( 22 );
      m_usRowFieldsNumber = GetUInt16( 24 );
      m_usColumnFieldsNumber = GetUInt16( 26 );
      m_usPageFieldsNumber = GetUInt16( 28 );
      m_usDataFieldsNumber = GetUInt16( 30 );
      m_usDataRowsNumber = GetUInt16( 32 );
      m_usDataColumnsNumber = GetUInt16( 34 );
      m_usOptions = GetUInt16( 36 );
      m_bRowGrand = GetBit( 36, 0 );
      m_bColumnGrand = GetBit( 36, 1 );
      m_bAutoFormat = GetBit( 36, 3 );
      m_bWHAutoFormat = GetBit( 36, 4 );
      m_bFontAutoFormat = GetBit( 36, 5 );
      m_bAlignAutoFormat = GetBit( 36, 6 );
      m_bBorderAutoFormat = GetBit( 36, 7 );
      m_bPatternAutoFormat = GetBit( 37, 0 );
      m_bNumberAutoFormat = GetBit( 37, 1 );
      m_usAutoFormatIndex = GetUInt16( 38 );
      m_usTableNameLength = GetUInt16( 40 );
      m_usDataFieldNameLength = GetUInt16( 42 );

      int iOffset = DEF_TABLE_NAME_OFFSET;
      m_strTableName = GetStringUpdateOffset( ref iOffset, m_usTableNameLength );
      m_strDataFieldName = GetString( iOffset, m_usDataFieldNameLength );
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_data = new byte[ DEF_TABLE_NAME_OFFSET ];

      SetUInt16( 0, m_usFirstRow );
      SetUInt16( 2, m_usLastRow );
      SetUInt16( 4, m_usFirstColumn );
      SetUInt16( 6, m_usLastColumn );
      SetUInt16( 8, m_usFirstHeadRow );
      SetUInt16( 10, m_usFirstDataRow );
      SetUInt16( 12, m_usFirstDataColumn );
      SetUInt16( 14, m_usCacheIndex );
      SetUInt16( 16, m_usReserved );
      SetUInt16( 18, m_usDataAxis );
      SetUInt16( 20, m_usDataPos );
      SetUInt16( 22, m_usFieldsNumber );
      SetUInt16( 24, m_usRowFieldsNumber );
      SetUInt16( 26, m_usColumnFieldsNumber );
      SetUInt16( 28, m_usPageFieldsNumber );
      SetUInt16( 30, m_usDataFieldsNumber );
      SetUInt16( 32, m_usDataRowsNumber );
      SetUInt16( 34, m_usDataColumnsNumber );
      SetUInt16( 36, m_usOptions );
      SetBit( 36, m_bRowGrand, 0 );
      SetBit( 36, m_bColumnGrand, 1 );
      SetBit( 36, m_bAutoFormat, 3 );
      SetBit( 36, m_bWHAutoFormat, 4 );
      SetBit( 36, m_bFontAutoFormat, 5 );
      SetBit( 36, m_bAlignAutoFormat, 6 );
      SetBit( 36, m_bBorderAutoFormat, 7 );
      SetBit( 37, m_bPatternAutoFormat, 0 );
      SetBit( 37, m_bNumberAutoFormat, 1 );
      SetUInt16( 38, m_usAutoFormatIndex );
      SetUInt16( 40, m_usTableNameLength );
      SetUInt16( 42, m_usDataFieldNameLength );

      m_iLength = DEF_TABLE_NAME_OFFSET;

      m_iLength += SetStringNoLen( DEF_TABLE_NAME_OFFSET, m_strTableName, false, true );
      bool bCompressed = IsAsciiString(m_strDataFieldName);
      m_iLength += SetStringNoLen(m_iLength, m_strDataFieldName, false, bCompressed);
    }
    #endregion
  }
}
