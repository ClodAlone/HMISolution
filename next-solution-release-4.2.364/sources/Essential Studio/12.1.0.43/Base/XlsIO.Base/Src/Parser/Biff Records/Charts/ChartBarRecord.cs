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

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record defines a bar or column chart group.
  /// </summary>
  [ Biff( TBIFFRecord.ChartBar ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartBarRecord
    : BiffRecordRaw
    , IChartType
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 6;
    #endregion

    #region Class members
    /// <summary>
    /// Space between bars (percent of bar width), default = 0.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOverlap;
    /// <summary>
    /// Space between categories (percent of bar width), default = 50.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usCategoriesSpace = 150;
    /// <summary>
    /// Holder of all flags.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True for horizontal bars (bar chart).
    /// False for vertical bars (column chart).
    /// </summary>
    [ BiffRecordPos( 4, 0, TFieldType.Bit ) ]
    private bool m_bIsHorizontal;
    /// <summary>
    /// Stack the displayed values.
    /// </summary>
    [ BiffRecordPos( 4, 1, TFieldType.Bit ) ]
    private bool m_bStackValues;
    /// <summary>
    /// Each category is displayed as a percentage.
    /// </summary>
    [ BiffRecordPos( 4, 2, TFieldType.Bit ) ]
    private bool m_bAsPercents;
    /// <summary>
    /// True if this bar has a shadow; otherwise False.
    /// </summary>
    [ BiffRecordPos( 4, 3, TFieldType.Bit ) ]
    private bool m_bHasShadow;
    #endregion

    #region Class properties
    /// <summary>
    /// Space between bars.
    /// </summary>
    public int Overlap
    {
      get
      {
        return -( short )m_usOverlap;
      }
      set
      {
        if( value != Overlap )
        {
          m_usOverlap = ( ushort )( -value );
        }
      }
    }

    /// <summary>
    /// Space between categories (percent of bar width), default = 50.
    /// </summary>
    public ushort CategoriesSpace
    {
      get
      {
        return m_usCategoriesSpace;
      }
      set
      {
        if( value != m_usCategoriesSpace )
        {
          m_usCategoriesSpace = value;
        }
      }
    }
    /// <summary>
    /// Holder of all flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True for horizontal bars (bar chart).
    /// False for vertical bars (column chart).
    /// </summary>
    public bool   IsHorizontalBar
    {
      get
      {
        return m_bIsHorizontal;
      }
      set
      {
        m_bIsHorizontal = value;
      }
    }
    /// <summary>
    /// Stack the displayed values.
    /// </summary>
    public bool   StackValues
    {
      get
      {
        return m_bStackValues;
      }
      set
      {
        m_bStackValues = value;
      }
    }
    /// <summary>
    /// Each category is displayed as a percentage.
    /// </summary>
    public bool   ShowAsPercents
    {
      get
      {
        return m_bAsPercents;
      }
      set
      {
        m_bAsPercents = value;
      }
    }
    /// <summary>
    /// True if this bar has a shadow; otherwise False.
    /// </summary>
    public bool   HasShadow
    {
      get
      {
        return m_bHasShadow;
      }
      set
      {
        m_bHasShadow = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartBarRecord()
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
    public  ChartBarRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartBarRecord( int iReserve )
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
      // TODO: check correctness of data

      m_usOverlap = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usCategoriesSpace = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bIsHorizontal = provider.ReadBit( iOffset, 0 );
      m_bStackValues = provider.ReadBit( iOffset, 1 );
      m_bAsPercents = provider.ReadBit( iOffset, 2 );
      m_bHasShadow = provider.ReadBit( iOffset, 3 );
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
      m_usOptions &= 0x0f;

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usOverlap );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usCategoriesSpace );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bIsHorizontal, 0 );
      provider.WriteBit( iOffset, m_bStackValues, 1 );
      provider.WriteBit( iOffset, m_bAsPercents, 2 );
      provider.WriteBit( iOffset, m_bHasShadow, 3 );
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion

  }
}