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
  /// Contains the range address of the used area in the current sheet.
  /// </summary>
  [ Biff( TBIFFRecord.Dimensions ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DimensionsRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 14;
    #endregion

    #region Class members

    /// <summary>
    /// Index to first used row. Zero base.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iFirstRow = 0;

    /// <summary>
    /// Index to last used row. One base.
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iLastRow = 0;

    /// <summary>
    /// Index to first used column.  Zero base.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usFirstColumn = 0;

    /// <summary>
    /// Index to last used column. One base.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usLastColumn = 0;

    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usReserved = 0;

    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Not used.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
    }
    /// <summary>
    /// Index to first used row.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }

    /// <summary>
    /// Index to last used row.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }

    /// <summary>
    /// Index to first used column.
    /// </summary>
    public ushort FirstColumn
    {
      get
      {
        return m_usFirstColumn;
      }
      set
      {
        m_usFirstColumn = value;
      }
    }

    /// <summary>
    /// Index to last used column.
    /// </summary>
    public ushort LastColumn
    {
      get
      {
        return m_usLastColumn;
      }
      set
      {
        m_usLastColumn = value;
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
        return 14;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return 14;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  DimensionsRecord()
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
    public  DimensionsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DimensionsRecord( int iReserve )
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_iFirstRow = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iLastRow = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_usFirstColumn = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLastColumn = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usReserved = provider.ReadUInt16( iOffset );

      if( m_usLastColumn <= m_usFirstColumn ) m_usLastColumn = ( ushort )( m_usFirstColumn + 1 );
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
      m_iLength = DEF_RECORD_SIZE;

      provider.WriteInt32( iOffset, m_iFirstRow );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iLastRow );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usFirstColumn );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLastColumn );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usReserved );
    }

    #endregion
  }
}
