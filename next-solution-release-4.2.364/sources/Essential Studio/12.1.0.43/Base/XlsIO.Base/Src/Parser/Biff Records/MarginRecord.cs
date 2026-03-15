#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record is part of the Page Settings block.
  /// It contains the margin of the current worksheet.
  /// </summary>
  [ Biff( TBIFFRecord.TopMargin ) ]
  [ Biff( TBIFFRecord.LeftMargin ) ]
  [ Biff( TBIFFRecord.BottomMargin ) ]
  [ Biff( TBIFFRecord.RightMargin ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MarginRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Default value of top margin.
    /// </summary>
    public const double DEFAULT_VALUE = 0.0;
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members

    /// <summary>
    /// It contains the top page margin of the current worksheet
    /// (IEEE floating-point value).
    /// </summary>
    [ BiffRecordPos( 0, 8, TFieldType.Float ) ]
    private double m_dbMargin = DEFAULT_VALUE;
    #endregion

    #region Class properties

    /// <summary>
    /// Maximum change in iteration (IEEE floating-point value).
    /// </summary>
    public double Margin
    {
      get
      {
        return m_dbMargin;
      }
      set
      {
        m_dbMargin = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  MarginRecord()
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
    public  MarginRecord( Stream stream, out int itemSize )
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
    public  MarginRecord( int iReserve )
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
      //AutoExtractFields();
      m_dbMargin = provider.ReadDouble( iOffset );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = DEF_RECORD_SIZE;
      provider.WriteDouble( iOffset, m_dbMargin );
    }

    #endregion
  }
}
