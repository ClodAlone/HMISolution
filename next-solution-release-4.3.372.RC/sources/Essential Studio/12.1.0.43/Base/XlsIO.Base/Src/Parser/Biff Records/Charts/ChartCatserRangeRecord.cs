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
  /// This record defines the scaling options for a category or series axis.
  /// </summary>
  [ Biff( TBIFFRecord.ChartCatserRange ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartCatserRangeRecord
    : BiffRecordRaw
    , IMaxCross
  {
    #region Class constants
    /// <summary>
    /// Minimum value for CrossingPoint, LabelsFrequency, and TickMarksFrequency properties.
    /// </summary>
    private const int DEF_MIN_CROSSPOINT = 1;
    /// <summary>
    /// Maximum value for CrossingPoint, LabelsFrequency, and TickMarksFrequency properties.
    /// </summary>
    private const int DEF_MAX_CROSSPOINT = 31999;
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Value axis / category crossing point (2D charts only).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usCrossingPoint = 1;
    /// <summary>
    /// Frequency of labels.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLabelsFrequency = 1;
    /// <summary>
    /// Frequency of tick marks.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usTickMarksFrequency = 1;
    /// <summary>
    /// Record flags holder.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions = 1;
    /// <summary>
    /// Value axis crossing:
    /// False if axis crosses mid-category.
    /// True if axis crosses between categories.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bValueAxisCrossing = true;
    /// <summary>
    /// Value axis crosses at the far right category (in a line, bar, 
    /// column, scatter, or area chart; 2D charts only).
    /// </summary>
    [ BiffRecordPos( 6, 1, TFieldType.Bit ) ]
    private bool m_bMaxCross;
    /// <summary>
    /// Display categories in reverse order.
    /// </summary>
    [ BiffRecordPos( 6, 2, TFieldType.Bit ) ]
    private bool m_bReverse;
    #endregion

    #region Class properties
    /// <summary>
    /// Value axis / category crossing point (2D charts only).
    /// </summary>
    public ushort CrossingPoint
    {
      get
      {
        return m_usCrossingPoint;
      }
      set
      {
        //if( value < DEF_MIN_CROSSPOINT || value > DEF_MAX_CROSSPOINT )
        //  throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 31999" );

        if( value != m_usCrossingPoint )
        {
          m_usCrossingPoint = value;
        }
      }
    }

    /// <summary>
    /// Frequency of labels.
    /// </summary>
    public ushort LabelsFrequency
    {
      get
      {
        return m_usLabelsFrequency;
      }
      set
      {
        if( value < DEF_MIN_CROSSPOINT || value > DEF_MAX_CROSSPOINT )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 31999" );

        if( value != m_usLabelsFrequency )
        {
          m_usLabelsFrequency = value;
        }
      }
    }

    /// <summary>
    /// Frequency of tick marks.
    /// </summary>
    public ushort TickMarksFrequency
    {
      get
      {
        return m_usTickMarksFrequency;
      }
      set
      {
        if( value < DEF_MIN_CROSSPOINT || value > DEF_MAX_CROSSPOINT )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 31999" );

        if( value != m_usTickMarksFrequency )
        {
          m_usTickMarksFrequency = value;
        }
      }
    }
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Value axis crossing:
    /// False if axis crosses mid-category.
    /// True if axis crosses between categories.
    /// </summary>
    public bool   IsBetween
    {
      get
      {
        return m_bValueAxisCrossing;
      }
      set
      {
        m_bValueAxisCrossing = value;
      }
    }
    /// <summary>
    /// Value axis crosses at the far right category (in a line, bar, 
    /// column, scatter, or area chart; 2D charts only).
    /// </summary>
    public bool   IsMaxCross
    {
      get
      {
        return m_bMaxCross;
      }
      set
      {
        m_bMaxCross = value;
      }
    }
    /// <summary>
    /// Display categories in reverse order.
    /// </summary>
    public bool   IsReverse
    {
      get
      {
        return m_bReverse;
      }
      set
      {
        m_bReverse = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartCatserRangeRecord()
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
    public  ChartCatserRangeRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartCatserRangeRecord( int iReserve )
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

      m_usCrossingPoint = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLabelsFrequency = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usTickMarksFrequency = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bValueAxisCrossing = provider.ReadBit( iOffset, 0 );
      m_bMaxCross = provider.ReadBit( iOffset, 1 );
      m_bReverse = provider.ReadBit( iOffset, 2 );
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
      //m_usOptions &= 0x7;

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usCrossingPoint );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLabelsFrequency );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usTickMarksFrequency );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bValueAxisCrossing, 0 );
      provider.WriteBit( iOffset, m_bMaxCross, 1 );
      provider.WriteBit( iOffset, m_bReverse, 2 );
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
