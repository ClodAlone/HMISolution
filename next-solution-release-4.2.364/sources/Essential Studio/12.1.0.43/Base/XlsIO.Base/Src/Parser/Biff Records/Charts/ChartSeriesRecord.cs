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
  /// This record describes the series of the chart and contains the 
  /// type of data and number of data fields that make up the series. 
  /// Series can contain 4000 points in Microsoft Excel version 5.  The 
  /// sdtX and sdtY fields define the type of data that is contained in 
  /// this series. At present, the two types of data used in the Microsoft 
  /// Excel chart series are numeric and text (date and sequence 
  /// information is not used). The cValx and cValy fields contain the 
  /// number of cell records in the series.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSeries ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartSeriesRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Type of data.
    /// </summary>
    public enum DataType : int
    {
      /// <summary>
      /// Represents the Date data format.
      /// </summary>
      Date      = 0,
      /// <summary>
      /// Represents the Numeric data format.
      /// </summary>
      Numeric   = 1,
      /// <summary>
      /// Represents the Sequence data format.
      /// </summary>
      Sequence  = 2,
      /// <summary>
      /// Represents the Text data format.
      /// </summary>
      Text      = 3
    }
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 12;
    #endregion

    #region Class members
    /// <summary>
    /// Type of data in categories.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usStdX;
    /// <summary>
    /// Type of data in values.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usStdY;
    /// <summary>
    /// Count of categories.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usCatCount;
    /// <summary>
    /// Count of values.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usValCount;
    /// <summary>
    /// Type of data in Bubble size series.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usBubbleDataType;
    /// <summary>
    /// Count of Bubble series values.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usBubbleSeriesCount;
    #endregion

    #region Class properties
    /// <summary>
    /// Type of data in categories.
    /// </summary>
    public DataType StdX
    {
      get
      {
        return ( DataType )m_usStdX;
      }
      set
      {
        m_usStdX = ( ushort )value;
      }
    }
    /// <summary>
    /// Type of data in values.
    /// </summary>
    public DataType StdY
    {
      get
      {
        return ( DataType )m_usStdY;
      }
      set
      {
        m_usStdY = ( ushort )value;
      }
    }
    /// <summary>
    /// Count of categories.
    /// </summary>
    public ushort CategoriesCount
    {
      get
      {
        return m_usCatCount;
      }
      set
      {
        m_usCatCount = value;
      }
    }
    /// <summary>
    /// Count of values.
    /// </summary>
    public ushort ValuesCount
    {
      get
      {
        return m_usValCount;
      }
      set
      {
        m_usValCount = value;
      }
    }
    /// <summary>
    /// Type of data in Bubble size series.
    /// </summary>
    public DataType BubbleDataType
    {
      get
      {
        return ( DataType )m_usBubbleDataType;
      }
      set
      {
        m_usBubbleDataType = ( ushort )value;
      }
    }
    /// <summary>
    /// Count of Bubble series values.
    /// </summary>
    public ushort BubbleSeriesCount
    {
      get
      {
        return m_usBubbleSeriesCount;
      }
      set
      {
        m_usBubbleSeriesCount = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartSeriesRecord()
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
    public  ChartSeriesRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSeriesRecord( int iReserve )
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

      m_usStdX = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usStdY = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usCatCount = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usValCount = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usBubbleDataType = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usBubbleSeriesCount = provider.ReadUInt16( iOffset );
      //iOffset += 2;
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
      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usStdX );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usStdY );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usCatCount );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usValCount );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBubbleDataType );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBubbleSeriesCount );
      //iOffset += 2;
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
