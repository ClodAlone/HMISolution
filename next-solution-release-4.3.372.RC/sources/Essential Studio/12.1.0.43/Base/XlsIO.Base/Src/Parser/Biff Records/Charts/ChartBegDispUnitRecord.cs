#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.IO;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
	/// <summary>
	/// Represents begin display axis unit label record.
	/// </summary>
  [ Biff( TBIFFRecord.ChartBegDispUnit ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
	public class ChartBegDispUnitRecord : BiffRecordRaw
	{
    #region Class constants
    /// <summary>
    /// Record size constant.
    /// </summary>
    public const int DEF_RECORD_SIZE = 12;
    #endregion

    #region Class members
    /// <summary>
    /// Represents display units in axis.
    /// </summary>
    [ BiffRecordPos( 4, 4, TFieldType.Bit ) ]
    private bool m_bIsShowLabel;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  ChartBegDispUnitRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  ChartBegDispUnitRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  ChartBegDispUnitRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates is show display unit label.
    /// </summary>
    public bool IsShowLabels
    {
      get
      {
        return m_bIsShowLabel;
      }
      set
      {
        m_bIsShowLabel = value;
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
      iOffset += 4;

      m_bIsShowLabel = provider.ReadBit( iOffset, 4 );
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
      provider.WriteUInt16( iOffset, ( ushort )TypeCode );
      iOffset += 2;

      provider.WriteUInt16( iOffset, ( ushort )0 );
      iOffset += 2;

      int iOffsetValue = iOffset;

      provider.WriteUInt32( iOffset, 0 );
      iOffset += 4;

      provider.WriteUInt32( iOffset, 0 );
      iOffset += 4;

      provider.WriteBit( iOffsetValue, m_bIsShowLabel, 4 );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
	}
}
