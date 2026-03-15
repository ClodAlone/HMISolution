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
  /// Summary description for DCON.
  /// </summary>
  [ Biff( TBIFFRecord.DCON ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DCONRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Index to the data consolidation function.
    /// </summary>
    [ BiffRecordPos( 0, 2, true ) ]
    private short m_sFuncIndex;
    /// <summary>
    /// 1 indicates that the left column option is turned on.
    /// </summary>
    [ BiffRecordPos( 2, 2, true ) ]
    private short m_sLeftColumn;
    /// <summary>
    /// 1 indicates that the top row option is turned on.
    /// </summary>
    [ BiffRecordPos( 4, 2, true ) ]
    private short m_sTopRow;
    /// <summary>
    /// 1 indicates that the create links to source data option is turned on.
    /// </summary>
    [ BiffRecordPos( 6, 2, true ) ]
    private short m_sLinkSource;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  DCONRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  DCONRecord( Stream stream, out int itemSize )
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
    public  DCONRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Index to the data consolidation function.
    /// </summary>
    public short FuncIndex
    {
      get
      {
        return m_sFuncIndex;
      }
      set
      {
        m_sFuncIndex = value;
      }
    }
    /// <summary>
    /// True means that the left column option is turned on.
    /// </summary>
    public bool IsLeftColumn
    {
      get
      {
        return ( m_sLeftColumn == 1 );
      }
      set
      {
        m_sLeftColumn = ( short ) ( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// True means that the top row option is turned on.
    /// </summary>
    public bool IsTopRow
    {
      get
      {
        return ( m_sTopRow == 1 );
      }
      set
      {
        m_sTopRow = ( short ) ( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// True means that the create links to source data option is turned on.
    /// </summary>
    public bool IsLinkSource
    {
      get
      {
        return ( m_sLinkSource == 1 );
      }
      set
      {
        m_sLinkSource = ( short ) ( value ? 1 : 0 );
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
        return DEF_RECORD_SIZE;
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
        return DEF_RECORD_SIZE;
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
      m_sFuncIndex = provider.ReadInt16( iOffset );
      m_sLeftColumn = provider.ReadInt16( iOffset + 2 );
      m_sTopRow = provider.ReadInt16( iOffset + 4 );
      m_sLinkSource = provider.ReadInt16( iOffset + 6 );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      provider.WriteInt16( iOffset, m_sFuncIndex );
      provider.WriteInt16( iOffset + 2, m_sLeftColumn );
      provider.WriteInt16( iOffset + 4, m_sTopRow );
      provider.WriteInt16( iOffset + 6, m_sLinkSource );
    }

    #endregion
  }
}
