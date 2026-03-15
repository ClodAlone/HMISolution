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
  /// Defines the formatting information for a range of columns including width,
  /// outline, and collapsed options.  
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.ColumnInfo ) ]
  [ CLSCompliant( false ) ]
  public class ColumnInfoRecord
    : BiffRecordRaw
    , IOutline
    , IComparable
  {
    #region Class constants
    /// <summary>
    /// Bit mask for the outlevel value.
    /// </summary>
    private const ushort OutlevelBitMask = 0x0700;
    /// <summary>
    /// Maximum and correct record size.
    /// </summary>
    private const int DEF_MAX_SIZE = 12;
    #endregion

    #region Class members

    /// <summary>
    /// Index of first column in the range.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFirstCol = 0;

    /// <summary>
    /// Index of the last column in the range.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLastCol = 0;

    /// <summary>
    /// Width of the columns in 1/256 of the width of zero character,
    /// using default font (first font record in the file).
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usColWidth = 2340;//2377;

    /// <summary>
    /// Index of extended format record for default column formatting.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usExtFormatIndex = 15;

    /// <summary>
    /// Options: Use bit fields instead of using this field.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usOptions = 0;

    #region Options bit fields

    /// <summary>
    /// Columns are hidden.
    /// </summary>
    [ BiffRecordPos( 8, 0, TFieldType.Bit ) ]
    private bool m_bHidden = false;

    [BiffRecordPos(8,2,TFieldType.Bit)]
    private bool m_bBestFit = false;
    [BiffRecordPos(8, 1, TFieldType.Bit)]
    private bool m_bUserSet = false;
    [BiffRecordPos(8,3,TFieldType.Bit)]
    private bool m_bPhonetic;
    /// <summary>
    /// Columns are collapsed.
    /// </summary>
    [ BiffRecordPos( 9, 4, TFieldType.Bit ) ]
    private bool m_bCollapsed = false;

    #endregion

    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usReserved = 4;
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Reserved value.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
    }
    /// <summary>
    /// Index of first column in the range.
    /// </summary>
    public ushort FirstColumn
    {
      get
      {
        return m_usFirstCol;
      }
      set
      {
        m_usFirstCol = value;
      }
    }

    /// <summary>
    /// Index of the last column in the range.
    /// </summary>
    public ushort LastColumn
    {
      get
      {
        return m_usLastCol;
      }
      set
      {
        m_usLastCol = value;
      }
    }

    /// <summary>
    /// Width of the columns in 1/256 of the width of the zero character,
    /// using default font (first font record in the file).
    /// </summary>
    public ushort ColumnWidth
    {
      get
      {
        return m_usColWidth;
      }
      set
      {
        m_usColWidth = value;
      }
    }

    /// <summary>
    /// Index of extended format record for default column formatting.
    /// </summary>
    public ushort ExtendedFormatIndex
    {
      get
      {
        return m_usExtFormatIndex;
      }
      set
      {
        m_usExtFormatIndex = value;
      }
    }

    /// <summary>
    /// Columns are hidden.
    /// </summary>
    public bool   IsHidden
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
    /// Gets or sets a value indicating whether this instance is best fit.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is best fit; otherwise, <c>false</c>.
    /// </value>
    internal bool IsBestFit
    {
        get
        {
            return m_bBestFit;
        }
        set
        {
            m_bBestFit = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is user set.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is user set; otherwise, <c>false</c>.
    /// </value>
    internal bool IsUserSet
    {
        get
        {
            return m_bUserSet;
        }
        set
        {
            m_bUserSet = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating if the phonetic information should be displayed by default for the affected column(s) of the worksheet.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is phenotic; otherwise, <c>false</c>.
    /// </value>
    internal bool IsPhenotic
    {
        get
        {
            return m_bPhonetic;
        }
        set
        {
            m_bPhonetic = value;
        }
    }

    /// <summary>
    /// Outline level of the columns (0 = no outline).
    /// This property changes bits of private m_usOptions field.
    /// Set method would raise ArgumentOutOfRange exception
    /// if value is more than 7.
    /// </summary>
    public ushort OutlineLevel
    {
      get
      {
        return ( ushort ) ( GetUInt16BitsByMask( m_usOptions, OutlevelBitMask ) >> 8 );
      }
      set
      {
        if( value > 7 )
          throw new ArgumentOutOfRangeException();
        SetUInt16BitsByMask( ref m_usOptions, OutlevelBitMask, ( ushort ) (value << 8 ) );
      }
    }

    /// <summary>
    /// If 1, then columns are collapsed.
    /// </summary>
    public bool   IsCollapsed
    {
      get
      {
        return m_bCollapsed;
      }
      set
      {
        m_bCollapsed = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 10;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return DEF_MAX_SIZE;
      }
    }
    /// <summary>
    /// Row or column index.
    /// </summary>
    ushort  IOutline.Index
    {
      get
      {
        return FirstColumn;
      }
      set
      {
        FirstColumn = value;
        LastColumn = value;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default Constructor
    /// </summary>
    public  ColumnInfoRecord()
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
    public  ColumnInfoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserves for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ColumnInfoRecord( int iReserve )
      : base( iReserve )
    {
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
      m_usFirstCol = provider.ReadUInt16( iOffset );
      m_usLastCol = provider.ReadUInt16( iOffset + 2 );
      m_usColWidth = provider.ReadUInt16( iOffset + 4 );
      m_usExtFormatIndex = provider.ReadUInt16( iOffset + 6 );
      m_usOptions = provider.ReadUInt16( iOffset + 8 );

      m_bHidden = provider.ReadBit( iOffset + 8, 0 );
      m_bUserSet = provider.ReadBit(iOffset + 8, 1);
      m_bBestFit = provider.ReadBit(iOffset + 8, 2);
      m_bPhonetic = provider.ReadBit(iOffset + 8, 3);
      m_bCollapsed = provider.ReadBit( iOffset + 9, 4 );

      if( iLength > 12 )
      {
        m_usReserved = provider.ReadUInt16( iOffset + 10 );
      }

      m_iLength = MinimumRecordSize;
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
      provider.WriteUInt16( iOffset, m_usFirstCol );
      provider.WriteUInt16( iOffset + 2, m_usLastCol );
      provider.WriteUInt16( iOffset + 4, m_usColWidth );
      provider.WriteUInt16( iOffset + 6, m_usExtFormatIndex );
      provider.WriteUInt16( iOffset + 8, m_usOptions );

      provider.WriteBit( iOffset + 8, m_bHidden, 0 );
      provider.WriteBit(iOffset + 8, m_bUserSet, 1);
      provider.WriteBit(iOffset + 8, m_bBestFit, 2);
      provider.WriteBit(iOffset + 8, m_bPhonetic, 3);
      provider.WriteBit( iOffset + 9, m_bCollapsed, 4 );

      provider.WriteUInt16( iOffset + 10, m_usReserved );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_MAX_SIZE;
    }
    #endregion

    #region IComparable members
    /// <summary>
    /// Compares this record with another ColumnInfoRecord.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns>0 if specified record is equal to the current record; otherwise -1.</returns>
    public int CompareTo( object obj )
    {
      int output = -1;

      if( obj is ColumnInfoRecord )
      {
        ColumnInfoRecord twin = ( ColumnInfoRecord )obj;

        if( (output = OutlineLevel.CompareTo( twin.OutlineLevel )) == 0 )
          if( (output = m_usExtFormatIndex.CompareTo( twin.m_usExtFormatIndex )) == 0 )
            if( (output = m_usColWidth.CompareTo( twin.m_usColWidth )) == 0 )
              if( (output = m_bHidden.CompareTo( twin.m_bHidden )) == 0 )
                if( (output = m_bCollapsed.CompareTo( twin.m_bCollapsed )) == 0 )
                  if( (output = m_usReserved.CompareTo( twin.m_usReserved )) == 0 )
                    return 0;
      }

      return output;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets options into default state without setting extended format index.
    /// </summary>
    public void SetDefaultOptions()
    {
      m_usColWidth = 2340;//2377;
      //m_usExtFormatIndex = 15;
      m_usOptions = 0;
      m_bHidden = false;
      m_bCollapsed = false;
      m_usReserved = 4;
    }
    #endregion
  }
}