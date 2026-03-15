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
  /// This record defines the axis type.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAxis ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAxisRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 18;
    /// <summary>
    /// Represents the Chart axis type.
    /// </summary>
    public enum ChartAxisType
    {
      /// <summary>
      /// Represents the CategoryAxis chart axis type.
      /// </summary>
      CategoryAxis = 0,
      /// <summary>
      /// Represents the ValueAxis chart axis type.
      /// </summary>
      ValueAxis = 1,
      /// <summary>
      /// Represents the SeriesAxis chart axis type.
      /// </summary>
      SeriesAxis = 2
    }
    #endregion

    #region Class members
    /// <summary>
    /// Axis type:
    /// 0 = category axis or x axis on a scatter chart
    /// 1 = value axis
    /// 2 = series axis
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usAxisType;
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    [ BiffRecordPos( 2, 4, true ) ]
    private int m_Reserved0;
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_Reserved1;
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    [ BiffRecordPos( 10, 4, true ) ]
    private int m_Reserved2;
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    [ BiffRecordPos( 14, 4, true ) ]
    private int m_Reserved3;
    #endregion

    #region Class properties
    /// <summary>
    /// Axis type:
    /// 0 = category axis or x axis on a scatter chart
    /// 1 = value axis
    /// 2 = series axis
    /// </summary>
    public ChartAxisType AxisType
    {
      get
      {
        return ( ChartAxisType )m_usAxisType;
      }
      set
      {
        m_usAxisType = ( ushort )value;
      }
    }

    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    public int Reserved0
    {
      get
      {
        return m_Reserved0;
      }
    }
    
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    public int Reserved1
    {
      get
      {
        return m_Reserved1;
      }
    }
    
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    public int Reserved2
    {
      get
      {
        return m_Reserved2;
      }
    }
    
    /// <summary>
    /// Field reserved by Microsoft. Read-only.
    /// </summary>
    public int Reserved3
    {
      get
      {
        return m_Reserved3;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAxisRecord()
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
    public  ChartAxisRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAxisRecord( int iReserve )
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

      m_usAxisType = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_Reserved0 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_Reserved1 = provider.ReadInt32( iOffset );
      iOffset += 4;
    
      m_Reserved2 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_Reserved3 = provider.ReadInt32( iOffset );
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
      // reserved values must be set to zero
      m_Reserved0 = m_Reserved1 = m_Reserved2 = m_Reserved3 = 0;

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usAxisType );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_Reserved0 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_Reserved1 );
      iOffset += 4;
    
      provider.WriteInt32( iOffset, m_Reserved2 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_Reserved3 );
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