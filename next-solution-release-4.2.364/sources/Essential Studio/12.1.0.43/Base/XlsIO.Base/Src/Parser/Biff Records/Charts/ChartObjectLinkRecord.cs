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
  /// This record links a TEXT record to an object on
  /// the chart or to the entire chart.
  /// </summary>
  [ Biff( TBIFFRecord.ChartObjectLink ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartObjectLinkRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 6;
    #endregion

    #region Class members
    /// <summary>
    /// Object text is linked to.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usLinkObject;
    /// <summary>
    /// Link index 1, series number.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLinkIndex1;
    /// <summary>
    /// Link index 2, data point number.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usLinkIndex2;
    #endregion

    #region Class properties
    /// <summary>
    /// Object text is linked to.
    /// </summary>
    public ExcelObjectTextLink LinkObject
    {
      get
      {
        return (ExcelObjectTextLink) m_usLinkObject;
      }
      set
      {
        m_usLinkObject = (ushort) value;
      }
    }
    /// <summary>
    /// Link index 1, series number.
    /// </summary>
    public ushort SeriesNumber
    {
      get
      {
        return m_usLinkIndex1;
      }
      set
      {
        m_usLinkIndex1 = value;
      }
    }
    /// <summary>
    /// Link index 2, data point number.
    /// </summary>
    public ushort DataPointNumber
    {
      get
      {
        return m_usLinkIndex2;
      }
      set
      {
        m_usLinkIndex2 = value;
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
    public  ChartObjectLinkRecord()
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
    public  ChartObjectLinkRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartObjectLinkRecord( int iReserve )
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

      m_usLinkObject = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLinkIndex1 = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLinkIndex2 = provider.ReadUInt16( iOffset );
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

      provider.WriteUInt16( iOffset, m_usLinkObject );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLinkIndex1 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLinkIndex2 );
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
