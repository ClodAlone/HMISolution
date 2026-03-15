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
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
	/// <summary>
	/// This class wraps single record. Used in charts for DataLabels settings.
	/// </summary>
	[ Biff( TBIFFRecord.ChartWrapper ) ]
  [ CLSCompliant( false ) ]
	public class ChartWrapperRecord
    : BiffRecordRaw
    , ICloneable
	{
    #region Class constants
    /// <summary>
    /// Offset to the wrapped record data.
    /// </summary>
    private const int DEF_RECORD_OFFSET = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Wrapped record.
    /// </summary>
    private BiffRecordRaw m_record;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartWrapperRecord()
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
    public  ChartWrapperRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartWrapperRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets wrapped record. Read-only.
    /// </summary>
    public BiffRecordRaw Record
    {
      get
      {
        return m_record;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_record = value;
      }
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
      m_record = BiffRecordFactory.GetRecord( provider, iOffset + DEF_RECORD_OFFSET, version );
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
      int iRecordLength = m_record.GetStoreSize( version );

      m_iLength = DEF_RECORD_OFFSET + iRecordLength + BiffRecordRaw.DEF_HEADER_SIZE;

      provider.WriteUInt16( iOffset + DEF_RECORD_OFFSET, ( ushort )m_record.TypeCode );
      provider.WriteUInt16( iOffset + DEF_RECORD_OFFSET + 2, ( ushort )iRecordLength );

      //if( iLength > 0 )
      {
        m_record.InfillInternalData( provider, iOffset + DEF_RECORD_OFFSET
          + BiffRecordRaw.DEF_HEADER_SIZE, version );
      }

      provider.WriteUInt16( iOffset, ( ushort )TypeCode );
      provider.WriteUInt16( iOffset + 2, 0 );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_OFFSET + BiffRecordRaw.DEF_HEADER_SIZE + m_record.GetStoreSize( version );
    }
    #endregion

    #region ICloneable Members

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    new public object Clone()
    {
      object result = base.Clone();

      if( m_record != null )
      {
        ChartWrapperRecord destRecord = ( ChartWrapperRecord )result;
        destRecord.m_record = ( BiffRecordRaw )m_record.Clone();
      }

      return result;
    }

    #endregion
  }
}
