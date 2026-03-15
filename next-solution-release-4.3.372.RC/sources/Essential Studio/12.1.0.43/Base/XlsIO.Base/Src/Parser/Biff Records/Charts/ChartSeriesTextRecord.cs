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
  /// The value of the ID field determines the assignment of the text field.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSeriesText ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartSeriesTextRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Minimum size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 3;
    #endregion

    #region Class members
    /// <summary>
    /// Text identifier: 0 = series name or text.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usTextId;
    /// <summary>
    /// The series text string.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.String ) ]
    private string m_strText = string.Empty;
    #endregion

    #region Class properties
    /// <summary>
    /// Text identifier: 0 = series name or text.
    /// </summary>
    public ushort TextId
    {
      get
      {
        return m_usTextId;
      }
      set
      {
        m_usTextId = value;
      }
    }
    /// <summary>
    /// The series text string.
    /// </summary>
    public string Text
    {
      get
      {
        return m_strText;
      }
      set
      {
        m_strText = value;
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

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartSeriesTextRecord()
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
    public  ChartSeriesTextRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSeriesTextRecord( int iReserve )
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
      // TODO: check correctness of data

      m_usTextId = provider.ReadUInt16( iOffset + 0 );
      int iFullLength;
      m_strText = provider.ReadString8Bit( iOffset + 2, out iFullLength );
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
      provider.WriteUInt16( iOffset + 0, m_usTextId );
      int iStartOffset = iOffset;
      iOffset += 2;
      provider.WriteString8BitUpdateOffset( ref iOffset, m_strText );
      m_iLength = iOffset - iStartOffset;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ExcelConstants.ShortSize + ExcelConstants.ShortSize + m_strText.Length * 2;
    }
    #endregion
  }
}
