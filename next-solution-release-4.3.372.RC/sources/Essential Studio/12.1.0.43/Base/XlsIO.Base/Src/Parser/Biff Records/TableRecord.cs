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

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores information about a multiple operation
  /// table in the sheet. It follows the first FORMULA record
  /// of the cell range containing the operation table.
  /// </summary>
  [ Biff( TBIFFRecord.Table ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class TableRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for the operation mode.
    /// </summary>
    public const ushort OperationModeBitMask  = 0x000C;
    /// <summary>
    /// First bit of the operation mode.
    /// </summary>
    public const int    OperationModeStartBit = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Index to first row of the multiple operation table range.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFirstRow;
    /// <summary>
    /// Index to last row of the multiple operation table range.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usLastRow;
    /// <summary>
    /// Index to first column of the multiple operation table range.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte m_FirstCol;
    /// <summary>
    /// Index to last column of the multiple operation table range.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private byte m_LastCol;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions;

    #region Options bit fields

    /// <summary>
    /// True to always recalculate array formula.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bRecalculate = false;

    /// <summary>
    /// True to calculate array formula on open.
    /// </summary>
    [ BiffRecordPos( 6, 1, TFieldType.Bit ) ]
    private bool m_bCalculateOnOpen = false;

    #endregion

    /// <summary>
    /// Index to row of input cell (in mode 1x2 index to row of
    /// input cell for row input).
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usInputCellRow;
    /// <summary>
    /// Index to column of input cell (in mode 1x2 index to column
    /// of input cell for row input).
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usInputCellCol;
    /// <summary>
    /// In mode 1x2 index to row of input cell for column input;
    /// else not used.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usInputCellRowForCol;
    /// <summary>
    /// In mode 1x2 index to column of input cell for column input;
    /// else not used.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usInputCellColForCol;
    #endregion

    #region Class properties
    /// <summary>
    /// Index to first row of the multiple operation table range.
    /// </summary>
    public ushort FirstRow
    {
      get
      {
        return m_usFirstRow;
      }
      set
      {
        m_usFirstRow = value;
      }
    }

    /// <summary>
    /// Index to last row of the multiple operation table range.
    /// </summary>
    public ushort LastRow
    {
      get
      {
        return m_usLastRow;
      }
      set
      {
        m_usLastRow = value;
      }
    }

    /// <summary>
    /// Index to first column of the multiple operation table range.
    /// </summary>
    public byte   FirstCol
    {
      get
      {
        return m_FirstCol;
      }
      set
      {
        m_FirstCol = value;
      }
    }

    /// <summary>
    /// Index to last column of the multiple operation table range.
    /// </summary>
    public byte   LastCol
    {
      get
      {
        return m_LastCol;
      }
      set
      {
        m_LastCol = value;
      }
    }

    /// <summary>
    /// True to always recalculate array formula.
    /// </summary>
    public bool   IsRecalculate
    {
      get
      {
        return m_bRecalculate;
      }
      set
      {
        m_bRecalculate = value;
      }
    }

    /// <summary>
    /// True to calculate array formula on open.
    /// </summary>
    public bool   IsCalculateOnOpen
    {
      get
      {
        return m_bCalculateOnOpen;
      }
      set
      {
        m_bCalculateOnOpen = value;
      }
    }

    /// <summary>
    /// Whether to display outline symbols (in the gutters).
    /// Changes bits of m_usOptions.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When value is more than 4.
    /// </exception>
    public ushort OperationMode
    {
      get
      {
        return (ushort) ( GetUInt16BitsByMask( m_usOptions, OperationModeBitMask ) >> OperationModeStartBit );
      }
      set
      {
        if( value > 4 )
        {
          throw new ArgumentOutOfRangeException();
        }
        SetUInt16BitsByMask( ref m_usOptions, OperationModeBitMask, (ushort) ( value << OperationModeStartBit ) );
      }
    }
    /// <summary>
    /// Index to row of input cell (in mode 1x2 index to row of
    /// input cell for row input).
    /// </summary>
    public ushort InputCellRow
    {
      get
      {
        return m_usInputCellRow;
      }
      set
      {
        m_usInputCellRow = value;
      }
    }
    /// <summary>
    /// Index to column of input cell (in mode 1x2 index to column
    /// of input cell for row input).
    /// </summary>
    public ushort InputCellColumn
    {
      get
      {
        return m_usInputCellCol;
      }
      set
      {
        m_usInputCellCol = value;
      }
    }
    /// <summary>
    /// In mode 1x2 index to row of input cell for column input;
    /// else not used.
    /// </summary>
    public ushort InputCellRowForColumn
    {
      get
      {
        return m_usInputCellRowForCol;
      }
      set
      {
        m_usInputCellRowForCol = value;
      }
    }
    /// <summary>
    /// In mode 1x2 index to column of input cell for column input;
    /// else not used.
    /// </summary>
    public ushort InputCellColumnForColumn
    {
      get
      {
        return m_usInputCellColForCol;
      }
      set
      {
        m_usInputCellColForCol = value;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 16;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return 16;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  TableRecord()
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
    public  TableRecord( Stream stream, out int itemSize )
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
    public  TableRecord( int iReserve )
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
      m_usFirstRow = provider.ReadUInt16( iOffset + 0 );
      m_usLastRow = provider.ReadUInt16( iOffset + 2 );
      m_FirstCol = provider.ReadByte( iOffset + 4 );
      m_LastCol = provider.ReadByte( iOffset + 5 );
      m_usOptions = provider.ReadUInt16( iOffset + 6 );
      m_bRecalculate = provider.ReadBit( iOffset + 6, 0 );
      m_bCalculateOnOpen = provider.ReadBit( iOffset + 6, 1 );
      m_usInputCellRow = provider.ReadUInt16( iOffset + 8 );
      m_usInputCellCol = provider.ReadUInt16( iOffset + 10 );
      m_usInputCellRowForCol = provider.ReadUInt16( iOffset + 12 );
      m_usInputCellColForCol = provider.ReadUInt16( iOffset + 14 );
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
      provider.WriteUInt16( iOffset + 0, m_usFirstRow );
      provider.WriteUInt16( iOffset + 2, m_usLastRow );
      provider.WriteByte( iOffset + 4, m_FirstCol );
      provider.WriteByte( iOffset + 5, m_LastCol );
      provider.WriteUInt16( iOffset + 6, m_usOptions );
      provider.WriteBit( iOffset + 6, m_bRecalculate, 0 );
      provider.WriteBit( iOffset + 6, m_bCalculateOnOpen, 1 );
      provider.WriteUInt16( iOffset + 8, m_usInputCellRow );
      provider.WriteUInt16( iOffset + 10, m_usInputCellCol );
      provider.WriteUInt16( iOffset + 12, m_usInputCellRowForCol );
      provider.WriteUInt16( iOffset + 14, m_usInputCellColForCol );
      m_iLength = MinimumRecordSize;
    }

    #endregion
  }
}
