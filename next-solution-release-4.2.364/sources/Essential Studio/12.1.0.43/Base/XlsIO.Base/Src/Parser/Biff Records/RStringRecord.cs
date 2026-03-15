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

using System;
using System.IO;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores a formatted text cell (Rich-Text).
  /// In BIFF8 it is replaced by the LABELSST record.
  /// Nevertheless, Excel uses this record if it copies formatted
  /// text cells to the clipboard.
  /// </summary>
  [ Biff( TBIFFRecord.RString ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class RStringRecord
    : BiffRecordRawWithArray
    , ICellPositionFormat
    , IStringValue
  {
    #region internal classes
    /// <summary>
    /// Rich text formatting run.
    /// </summary>
    [ CLSCompliant( false ) ]
    public struct TFormattingRun
    {
      /// <summary>
      /// First formatted character (zero-based).
      /// </summary>
      public ushort FirstChar;
      /// <summary>
      /// Index to FONT record.
      /// </summary>
      public ushort FormatIndex;
    }
    #endregion

    #region Class members

    /// <summary>
    /// Index to row.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private int m_iRow = 0;

    /// <summary>
    /// Index to column.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private int m_iColumn = 0;

    /// <summary>
    /// Index to XF (Extended Format) record.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usExtFormat = 0;

    /// <summary>
    /// Unformatted Unicode string, 16-bit string length.
    /// </summary>
    private string m_strValue = null;

    /// <summary>
    /// Number of rich text formatting runs.
    /// </summary>
    private ushort m_usFRunsNumber = 0;

    /// <summary>
    /// Array of formatting runs.
    /// </summary>
    private TFormattingRun[] m_arrFormattingRuns = null;
    #endregion

    #region Class Properties

    /// <summary>
    /// Index to row.
    /// </summary>
    public int Row
    {
      get
      {
        return m_iRow;
      }
      set
      {
        m_iRow = value;
      }
    }

    /// <summary>
    /// Index to column.
    /// </summary>
    public int Column
    {
      get
      {
        return m_iColumn;
      }
      set
      {
        m_iColumn = value;
      }
    }

    /// <summary>
    /// Index to XF (Extended Format) record.
    /// </summary>
    public ushort ExtendedFormatIndex
    {
      get
      {
        return m_usExtFormat;
      }
      set
      {
        m_usExtFormat = value;
      }
    }

    /// <summary>
    /// Unformatted Unicode string, 16-bit string length.
    /// </summary>
    public string Value
    {
      get
      {
        return m_strValue;
      }
      set
      {
        m_strValue = value;
      }
    }

    /// <summary>
    /// Array of formatting runs.
    /// </summary>
    public TFormattingRun[] FormattingRun
    {
      get
      {
        return m_arrFormattingRuns;
      }
      set
      {
        m_arrFormattingRuns = value;
        m_usFRunsNumber = ( value != null ) ? ( ushort ) value.Length : ( ushort ) 0;
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
        return 8;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  RStringRecord()
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
    public  RStringRecord( Stream stream, out int itemSize )
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
    public  RStringRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_iRow = GetUInt16( 0 );
      m_iColumn = GetUInt16( 2 );
      m_usExtFormat = GetUInt16( 4 );

      int offset = 6;
      m_strValue = GetString16BitUpdateOffset( ref offset );
      m_usFRunsNumber = GetUInt16( offset );
      offset += 2;
      m_arrFormattingRuns = new TFormattingRun[ m_usFRunsNumber ];

      for( int i = 0; i < m_usFRunsNumber; i++, offset += 4 )
      {
        m_arrFormattingRuns[ i ].FirstChar = GetUInt16( offset );
        m_arrFormattingRuns[ i ].FormatIndex = GetUInt16( offset + 2 );
      }
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( ExcelVersion version )
    {
      AutoGrowData = true;
      SetUInt16( 0, ( ushort )m_iRow );
      SetUInt16( 2, ( ushort )m_iColumn );
      SetUInt16( 4, m_usExtFormat );
      m_iLength = 6;

      SetString16BitUpdateOffset( ref m_iLength, m_strValue );
      SetUInt16( m_iLength, m_usFRunsNumber );
      m_iLength += 2;

      for( int i = 0; i < m_usFRunsNumber; i++, m_iLength += 4 )
      {
        SetUInt16( m_iLength    , m_arrFormattingRuns[ i ].FirstChar );
        SetUInt16( m_iLength + 2, m_arrFormattingRuns[ i ].FormatIndex );
      }
    }

    #endregion

    #region IStringValue Members
    /// <summary>
    /// Returns string value. Read-only.
    /// </summary>
    string IStringValue.StringValue
    {
      get
      {
        return Value;
      }
    }

    #endregion
  }
}
