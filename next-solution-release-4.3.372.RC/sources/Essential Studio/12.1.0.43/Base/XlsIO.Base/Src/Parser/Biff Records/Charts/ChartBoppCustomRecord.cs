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
  /// This record stores options for a custom bar of pie or pie of 
  /// pie chart; these are two of the pie chart subtypes.
  /// </summary>
  [ Biff( TBIFFRecord.ChartBoppCustom ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartBoppCustomRecord : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// Count of pie slices in the bar of pie or pie of pie chart.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usQuantity;
    /// <summary>
    /// Array of bytes; each byte contains a bit field that describes the 
    /// individual point positioning in the series. If a slice is on the 
    /// secondary pie or bar chart, the corresponding bit is set to 1 (one);
    /// otherwise the bit is 0 (zero).
    /// </summary>
    private byte[] m_bits;
    #endregion

    #region Class properties
    /// <summary>
    /// Count of pie slices in the bar of pie or pie of pie chart.
    /// </summary>
    public ushort Counter
    {
      get
      {
        return m_usQuantity;
      }
    }
    /// <summary>
    /// Array of bytes; each byte contains a bit field that describes the 
    /// individual point positioning in the series. If a slice is on the 
    /// secondary pie or bar chart, the corresponding bit is set to 1 (one);
    /// otherwise the bit is 0 (zero).
    /// </summary>
    public byte[] BitFields
    {
      get
      {
        return m_bits;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_bits = value;
        m_usQuantity = ( ushort )value.Length;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartBoppCustomRecord()
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
    public  ChartBoppCustomRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartBoppCustomRecord( int iReserve )
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
      m_usQuantity = provider.ReadUInt16( iOffset + 0 );
      m_bits = new byte[ m_usQuantity ];
      provider.ReadArray( iOffset + 2, m_bits );
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
      provider.WriteUInt16( iOffset + 0, m_usQuantity );
      provider.WriteBytes( iOffset + 2, m_bits, 0, m_bits.Length );
      m_iLength = m_bits.Length + 2;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ExcelConstants.ShortSize + m_usQuantity;
    }
    #endregion

    #region ICloneable method
    /// <summary>
    /// Clones current record.
    /// </summary>
    /// <returns>Returns cloned record.</returns>
    public override object Clone()
    {
      ChartBoppCustomRecord result = ( ChartBoppCustomRecord )base.Clone();

      result.m_bits = CloneUtils.CloneByteArray( m_bits );

      return result;
    }
    #endregion
  }
}
