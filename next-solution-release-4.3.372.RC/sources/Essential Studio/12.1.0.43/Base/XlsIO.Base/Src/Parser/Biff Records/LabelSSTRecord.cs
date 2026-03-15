#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Represents a cell that contains a string.
  /// Refers to a string in the shared string table and is a column value.
  /// </summary>
  [ Biff( TBIFFRecord.LabelSST ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class LabelSSTRecord
    : CellPositionBase
    , ICloneable
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 10;
    /// <summary>
    /// Index offset.
    /// </summary>
    internal const int DEF_INDEX_OFFSET = 6;
    #endregion

    #region Class members
    /// <summary>
    /// Index into SST record.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_iSSTIndex = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Index into SST record.
    /// </summary>
    public int    SSTIndex
    {
      get
      {
        return m_iSSTIndex;
      }
      set
      {
        m_iSSTIndex = value;
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

#if DELAYED_PARSING
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

#endif
    /// <summary>
    /// 
    /// </summary>
    public override int MaximumMemorySize
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
    public  LabelSSTRecord()
      : base()
    {
#if USE_STRUCTURES
      m_iLength = DEF_RECORD_SIZE;
#elif DELAYED_PARSING
      m_data = new byte[ DEF_RECORD_SIZE ];
      //      AutoGrowData = true;
#endif
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void ParseCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iSSTIndex = provider.ReadInt32( iOffset );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void InfillCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteInt32( iOffset, m_iSSTIndex );
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DEF_RECORD_SIZE;

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets new index.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the start of the record data.</param>
    /// <param name="iNewIndex">New SST index.</param>
    /// <param name="version">Excel version of the used data storage.</param>
    public static void SetSSTIndex( DataProvider provider, int iOffset, int iNewIndex, ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      iOffset += DEF_INDEX_OFFSET + DEF_HEADER_SIZE;

      if( version != ExcelVersion.Excel97to2003 )
      {
        iOffset += 4;
      }

      provider.WriteInt32( iOffset, iNewIndex );
    }
    /// <summary>
    /// Sets new index.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the start of the record data.</param>
    /// <param name="version">Excel version of the used data storage.</param>
    /// <returns>Index in the SST table.</returns>
    public static int GetSSTIndex( DataProvider provider, int iOffset, ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      iOffset += DEF_INDEX_OFFSET + DEF_HEADER_SIZE;

      if( version != ExcelVersion.Excel97to2003 )
      {
        iOffset += 4;
      }

      return provider.ReadInt32( iOffset );
    }
    #endregion
  }
}
