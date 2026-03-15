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
	/// Represents DisplayUnits option.
	/// </summary>
  [ Biff( TBIFFRecord.ChartAxisDisplayUnits ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
	public class ChartAxisDisplayUnitsRecord : BiffRecordRaw
	{
    #region Class constants
    /// <summary>
    /// Record size constant.
    /// </summary>
    public const int DEF_RECORD_SIZE = 16;
    #endregion

    #region Class members
    /// <summary>
    /// Represents display units in axis.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_displayUnit;
    /// <summary>
    /// Represents display units value.
    /// </summary>
    [ BiffRecordPos( 6, 8, TFieldType.Float ) ]
    private double m_displayUnitValue;
    /// <summary>
    /// Indicates is show display unit label.
    /// </summary>
    [ BiffRecordPos( 14, 1 ) ]
    private byte   m_isShowLabels;
    /// <summary>
    /// Reserved value.
    /// </summary>
    [ BiffRecordPos( 15, 1 ) ]
    private byte   m_reserved;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  ChartAxisDisplayUnitsRecord()
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
    public  ChartAxisDisplayUnitsRecord( Stream stream, out int itemSize )
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
    public  ChartAxisDisplayUnitsRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Represents display units in axis.
    /// </summary>
    public ExcelChartDisplayUnit DisplayUnit
    {
      get
      {
        return ( ExcelChartDisplayUnit )m_displayUnit;
      }
      set
      {
        m_displayUnit = ( ushort )value;
      }
    }
    /// <summary>
    /// Represents display units value.
    /// </summary>
    public double DisplayUnitValue
    {
      get
      {
        return m_displayUnitValue;
      }
      set
      {
        m_displayUnitValue = value;
      }
    }
    /// <summary>
    /// Indicates is show display unit label.
    /// </summary>
    public bool IsShowLabels
    {
      get
      {
        return m_isShowLabels == 3;
      }
      set
      {
        m_isShowLabels = ( value )
          ? ( byte )3
          : ( byte )1;
      }
    }
    /// <summary>
    /// Represents reserved byte.
    /// </summary>
    public byte Recerved
    {
      get
      {
        return m_reserved;
      }
      set
      {
        m_reserved = value;
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

      m_displayUnit = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_displayUnitValue = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_isShowLabels = provider.ReadByte( iOffset );
      iOffset++;

      m_reserved = provider.ReadByte( iOffset );
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

      provider.WriteUInt16( iOffset, m_displayUnit );
      iOffset += 2;

      provider.WriteDouble( iOffset, m_displayUnitValue );
      iOffset += 8;

      byte[] arr = { m_isShowLabels, m_reserved };

      provider.WriteByte( iOffset, m_isShowLabels );
      iOffset++;

      provider.WriteByte( iOffset, m_reserved );

      m_iLength = DEF_RECORD_SIZE;
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
