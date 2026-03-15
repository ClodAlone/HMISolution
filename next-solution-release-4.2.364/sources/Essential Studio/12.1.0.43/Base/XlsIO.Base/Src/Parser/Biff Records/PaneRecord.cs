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
  /// 
  /// </summary>
  [ Biff( TBIFFRecord.Pane ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class PaneRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DefaultRecordSize = 10;
    #endregion

    #region Class members
    /// <summary>
    /// Position of the vertical split (px, 0 = No vertical split).
    /// Unfrozen pane: Width of the left pane(s) (in twips = 1/20 of a point).
    /// Frozen pane: Number of visible columns in left pane(s).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private int m_iVertSplit;
    /// <summary>
    /// Position of the horizontal split (py, 0 = No horizontal split).
    /// Unfrozen pane: Height of the top pane(s) (in twips = 1/20 of a point).
    /// Frozen pane: Number of visible rows in top pane(s).
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private int m_iHorizSplit;
    /// <summary>
    /// Index to first visible row in bottom pane(s).
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private int m_iFirstRow;
    /// <summary>
    /// Index to first visible column in right pane(s).
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private int m_iFirstColumn;
    /// <summary>
    /// Identifier of pane with active cell cursor (see below). The last field 
    /// specifying the active pane has a size of 1 byte in BIFF2-BIFF4 and 2 
    /// bytes in BIFF5-BIFF8. The correct identifiers for all possible 
    /// combinations of visible panes are shown in the following pictures:
    /// px=0, py=0              -> 3
    /// px=0, py>0              -> 3
    ///                            2
    /// px>0, py=0              -> 3 1
    /// px>0, py>0              -> 3 1
    ///                            2 0
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usActivePane;
    #endregion

    #region Class Properties
    /// <summary>
    /// Position of the vertical split (px, 0 = No vertical split).
    /// Unfrozen pane: Width of the left pane(s) (in twips = 1/20 of a point).
    /// Frozen pane: Number of visible columns in left pane(s).
    /// </summary>
    public int VerticalSplit
    {
      get
      {
        return m_iVertSplit;
      }
      set
      {
        m_iVertSplit = value;
      }
    }
    /// <summary>
    /// Position of the horizontal split (py, 0 = No horizontal split).
    /// Unfrozen pane: Height of the top pane(s) (in twips = 1/20 of a point).
    /// Frozen pane: Number of visible rows in top pane(s).
    /// </summary>
    public int HorizontalSplit
    {
      get
      {
        return m_iHorizSplit;
      }
      set
      {
        m_iHorizSplit = value;
      }
    }
    /// <summary>
    /// Index to first visible row in bottom pane(s).
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
    /// Index to first visible column in right pane(s).
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return m_iFirstColumn;
      }
      set
      {
        m_iFirstColumn = value;
      }
    }
    /// <summary>
    /// Identifier of pane with active cell cursor (see below). The last field 
    /// specifying the active pane has a size of 1 byte in BIFF2-BIFF4 and 2 
    /// bytes in BIFF5-BIFF8. The correct identifiers for all possible 
    /// combinations of visible panes are shown in the following pictures:
    /// px=0, py=0              -> 3
    /// px=0, py>0              -> 3
    ///                            2
    /// px>0, py=0              -> 3 1
    /// px>0, py>0              -> 3 1
    ///                            2 0
    /// </summary>
    public ushort ActivePane
    {
      get
      {
        return m_usActivePane;
      }
      set
      {
        m_usActivePane = value;
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
        return DefaultRecordSize;
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
        return DefaultRecordSize;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  PaneRecord()
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
    public  PaneRecord( Stream stream, out int itemSize )
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
    public  PaneRecord( int iReserve )
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
      m_iVertSplit = provider.ReadUInt16( iOffset + 0 );
      m_iHorizSplit = provider.ReadUInt16( iOffset + 2 );
      m_iFirstRow = provider.ReadUInt16( iOffset + 4 );
      m_iFirstColumn = provider.ReadUInt16( iOffset + 6 );
      m_usActivePane = provider.ReadUInt16( iOffset + 8 );
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
      provider.WriteUInt16( iOffset + 0, ( ushort )m_iVertSplit );
      provider.WriteUInt16( iOffset + 2, ( ushort )m_iHorizSplit );
      provider.WriteUInt16( iOffset + 4, ( ushort )m_iFirstRow );
      provider.WriteUInt16( iOffset + 6, ( ushort )m_iFirstColumn );
      provider.WriteUInt16( iOffset + 8, m_usActivePane );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}