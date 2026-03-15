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
  /// This record defines a line chart group.
  /// </summary>
  [ Biff( TBIFFRecord.ChartLine ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartLineRecord
    : BiffRecordRaw
    , IChartType
  {
    #region Constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DefaultRecordSize = ExcelConstants.ShortSize;
    #endregion

    #region Class members
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Stack the displayed values.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bStackValues;
    /// <summary>
    /// Each category is broken down as a percentage.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bAsPercents;
    /// <summary>
    /// True if this line has a shadow.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bHasShadow;
    #endregion

    #region Class properties
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
    /// Stack the displayed values.
    /// </summary>
    public bool StackValues
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
    /// Each category is broken down as a percentage.
    /// </summary>
    public bool ShowAsPercents
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
    /// True if this line has a shadow.
    /// </summary>
    public bool HasShadow
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
    public  ChartLineRecord()
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
    public  ChartLineRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartLineRecord( int iReserve )
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
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bStackValues = provider.ReadBit( iOffset + 0, 0 );
      m_bAsPercents = provider.ReadBit( iOffset + 0, 1 );
      m_bHasShadow = provider.ReadBit( iOffset + 0, 2 );
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
      m_usOptions &= 0x07;

      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bStackValues, 0 );
      provider.WriteBit( iOffset + 0, m_bAsPercents, 1 );
      provider.WriteBit( iOffset + 0, m_bHasShadow, 2 );
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