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
using System.Text;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// Summary description for ChartDataLabelsRecord.
  /// </summary>
  [ Biff( TBIFFRecord.ChartDataLabels ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartDataLabelsRecord
    : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int DEF_RECORD_SIZE = 12;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 12, 1 ) ]
    private byte m_Options;
    /// <summary>
    /// Indicates whether series name is in data labels.
    /// </summary>
    [ BiffRecordPos( 12, 0, TFieldType.Bit ) ]
    private bool m_bSeriesName;
    /// <summary>
    /// Indicates whether category name is in data labels.
    /// </summary>
    [ BiffRecordPos( 12, 1, TFieldType.Bit ) ]
    private bool m_bCategoryName;
    /// <summary>
    /// Indicates whether value is in data labels.
    /// </summary>
    [ BiffRecordPos( 12, 2, TFieldType.Bit ) ]
    private bool m_bValue;
    /// <summary>
    /// Indicates whether percentage is in data labels.
    /// </summary>
    [ BiffRecordPos( 12, 3, TFieldType.Bit ) ]
    private bool m_bPercentage;
    /// <summary>
    /// Indicates whether bubble size is in data labels.
    /// </summary>
    [ BiffRecordPos( 12, 4, TFieldType.Bit ) ]
    private bool m_bBubbleSize;
    /// <summary>
    /// Length of the delimiter.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usDelimLen;
    /// <summary>
    /// Delimiter.
    /// </summary>
    private string m_strDelimiter;
    #endregion

    #region Class properties
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public byte Options
    {
      get
      {
        return m_Options;
      }
#if DEBUG
      set
      {
        m_Options = value;
      }
#endif
    }
    /// <summary>
    /// Indicates whether series name is in data labels.
    /// </summary>
    public bool IsSeriesName
    {
      get
      {
        return m_bSeriesName;
      }
      set
      {
        m_bSeriesName = value;
      }
    }
    /// <summary>
    /// Indicates whether category name is in data labels.
    /// </summary>
    public bool IsCategoryName
    {
      get
      {
        return m_bCategoryName;
      }
      set
      {
        m_bCategoryName = value;
      }
    }
    /// <summary>
    /// Indicates whether value is in data labels.
    /// </summary>
    public bool IsValue
    {
      get
      {
        return m_bValue;
      }
      set
      {
        m_bValue = value;
      }
    }
    /// <summary>
    /// Indicates whether percentage is in data labels.
    /// </summary>
    public bool IsPercentage
    {
      get
      {
        return m_bPercentage;
      }
      set
      {
        m_bPercentage = value;
      }
    }
    /// <summary>
    /// Indicates whether bubble size is in data labels.
    /// </summary>
    public bool IsBubbleSize
    {
      get
      {
        return m_bBubbleSize;
      }
      set
      {
        m_bBubbleSize = value;
      }
    }
    /// <summary>
    /// Length of the delimiter.
    /// </summary>
    public int DelimiterLength
    {
      get
      {
        return m_usDelimLen;
      }
    }
    /// <summary>
    /// Delimiter.
    /// </summary>
    public string Delimiter
    {
      get
      {
        return m_strDelimiter;
      }
      set
      {
        m_strDelimiter = value;
        m_usDelimLen = (ushort) ( (value == null ) ? 0 : value.Length);
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartDataLabelsRecord()
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
    public  ChartDataLabelsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
  {
  }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartDataLabelsRecord( int iReserve )
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
      m_Options = provider.ReadByte( iOffset + 12 );
      m_bSeriesName = provider.ReadBit( iOffset + 12, 0 );
      m_bCategoryName = provider.ReadBit( iOffset + 12, 1 );
      m_bValue = provider.ReadBit( iOffset + 12, 2 );
      m_bPercentage = provider.ReadBit( iOffset + 12, 3 );
      m_bBubbleSize = provider.ReadBit( iOffset + 12, 4 );
      m_usDelimLen = provider.ReadUInt16( iOffset + 14 );

      if( m_usDelimLen > 0 )
      {
        int iSize;
        m_strDelimiter = provider.ReadString( iOffset + 16, m_usDelimLen, out iSize, false );
      }
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      int iStartOffset = iOffset;
      provider.WriteInt16( iOffset, 0x86B );

      for( int i = iOffset + ExcelConstants.ShortSize, len = iOffset + 12; i < len; i++ )
        provider.WriteByte( i, 0 );

      provider.WriteByte( iOffset + 12, m_Options );
      provider.WriteBit( iOffset + 12, m_bSeriesName, 0 );
      provider.WriteBit( iOffset + 12, m_bCategoryName, 1 );
      provider.WriteBit( iOffset + 12, m_bValue, 2 );
      provider.WriteBit( iOffset + 12, m_bPercentage, 3 );
      provider.WriteBit( iOffset + 12, m_bBubbleSize, 4 );
      provider.WriteByte(iOffset + 13, 0);
      provider.WriteUInt16( iOffset + 14, m_usDelimLen );

      if( m_usDelimLen > 0 )
        provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strDelimiter );
      
      m_iLength = iOffset - iStartOffset;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = 16; // size of the fixed part.

      if( m_usDelimLen > 0 )
      {
        iResult += 1 + Encoding.Unicode.GetByteCount( m_strDelimiter );
      }

      return iResult;
    }
    public static bool operator ==( ChartDataLabelsRecord record1, ChartDataLabelsRecord record2 )
    {
      bool bFirstNull = object.Equals( record1, null );
      bool bSecondNull = object.Equals( record2, null );

      if( bFirstNull && bSecondNull )
        return true;

      if( bFirstNull || bSecondNull )
        return false;

      return record1.m_bSeriesName == record2.m_bSeriesName &&
        record1.m_bCategoryName == record2.m_bCategoryName &&
        record1.m_bValue == record2.m_bValue &&
        record1.m_bPercentage == record2.m_bPercentage &&
        record1.m_bBubbleSize == record2.m_bBubbleSize;
    }
    public static bool operator !=( ChartDataLabelsRecord record1, ChartDataLabelsRecord record2 )
    {
      return !( record1 == record2 );
    }
    #endregion
  }
}
