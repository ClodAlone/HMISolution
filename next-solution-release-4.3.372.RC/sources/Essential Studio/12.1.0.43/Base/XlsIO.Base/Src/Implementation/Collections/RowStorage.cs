#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
//using Syncfusion.XlsIO.IO.Stream.Win32;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
using TRangeValueType = Syncfusion.XlsIO.Implementation.WorksheetImpl.TRangeValueType;
using OptionFlags = Syncfusion.XlsIO.Parser.Biff_Records.RowRecord.OptionFlags;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Stores single row data and gives functionality to access and modify it.
  /// </summary>
  public class RowStorage
    : IDisposable
    , IBiffStorage
    , ICloneable
    , IOutline
  {
    #region Class constants
    /// <summary>
    /// Memory block will be divisible of this value.
    /// </summary>
    private const int DEF_MEMORY_DELTA = 128;//512;
    /// <summary>
    /// Represents MULRK xf indexes period.
    /// </summary>
    private const int DEF_MULRK_PERIOD = 6;
    /// <summary>
    /// Represents MULBLANK xf indexes period.
    /// </summary>
    private const int DEF_MULBLANK_PERIOD = 2;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private const int DEF_ARRAY_CODE = ( int )TBIFFRecord.Array;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private const int DEF_STRING_CODE = ( int )TBIFFRecord.String;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private const int DEF_FORMULA_CODE = ( int )TBIFFRecord.Formula;
    /// <summary>
    /// Records that are not simply ICellPositionFormat records but that contains multiple subrecords.
    /// </summary>
    private static readonly short[] DEF_MULTIRECORDS = new short[]
    {
      ( short )TBIFFRecord.MulBlank,
      ( short )TBIFFRecord.MulRK,
    };
    /// <summary>
    /// First record type - record that can contain several sub records, second
    /// record type - type of the single record corresponding to the multi record type.
    /// </summary>
    private static readonly TBIFFRecord[] DEF_MULTIRECORDS_SUBTYPES = new TBIFFRecord[]
    {
      TBIFFRecord.MulBlank,
      TBIFFRecord.Blank,
      TBIFFRecord.MulRK,
      TBIFFRecord.RK,
    };
    /// <summary>
    /// Storage options.
    /// </summary>
    [ Flags ]
    private enum StorageOptions
    {
      /// <summary>
      /// None.
      /// </summary>
      None = 0,
      /// <summary>
      /// Indicates that storage contains (or can contain) some RK or Blank records.
      /// </summary>
      HasRKBlank = 1,
      /// <summary>
      /// Indicates that storage contains (or can contain) some MultiRK or MultiBlank records.
      /// </summary>
      HasMultiRKBlank = 2,
      /// <summary>
      /// Indicates that storage was disposed.
      /// </summary>
      Disposed = 4,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Zero-based index of the first column.
    /// </summary>
    private int m_iFirstColumn = -1;
    /// <summary>
    /// Zero-based index of the last column.
    /// </summary>
    private int m_iLastColumn = -1;
    /// <summary>
    /// Size of used data.
    /// </summary>
    private int m_iUsedSize = 0;
    /// <summary>
    /// Data provider.
    /// </summary>
    private DataProvider m_dataProvider;
    /// <summary>
    /// Storage options.
    /// </summary>
    private StorageOptions m_options;
    /// <summary>
    /// Column that is referenced by m_iCurrentOffset. -1 - means that we have no correct offset.
    /// </summary>
    private int m_iCurrentColumn = -1;
    /// <summary>
    /// Offset in the data storage to the record with m_iCurrentColumn.
    /// </summary>
    private int m_iCurrentOffset = -1;
    ///// <summary>
    ///// Size of the cell position in the cell record (4 - 2 bytes for row, 2 bytes for column,
    ///// 8 - 4 bytes for row, 4 bytes for column).
    ///// </summary>
    //private int m_iCellPositionSize = 4;
    private ExcelVersion m_version = ExcelVersion.Excel97to2003;
    /// <summary>
    /// Workbook object.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Represents the row index.
    /// </summary>
    private int m_row;
    /// <summary>
    /// Represents the cells contains WrapText
    /// </summary>
    private bool m_isWrapText;

    private string[] dateFormats = new string[]
        {
            "d",
            "dd",
            "m",
            "mm",
            "yy",
            "yyyy",
        };
    private bool m_hasRowHeight;

    #region RowRecord Fields
    //    /// <summary>
    //    /// Index of this row.
    //    /// </summary>
    //    private ushort m_usRowNumber;
    /// <summary>
    /// Height of the row, in twips = 1/20 of a point.
    /// </summary>
    private ushort m_usHeight;
    /// <summary>
    /// Options flag.
    /// </summary>
    private RowRecord.OptionFlags m_optionFlags = RowRecord.OptionFlags.ShowOutlineGroups;
    /// <summary>
    /// 
    /// </summary>
    private ushort m_usXFIndex;
    #endregion
    /// <summary>
    /// Indicates the dot symbol
    /// </summary>
    private const string DEF_DOT = ".";
    /// <summary>
    /// Preserves Table record for datatable support
    /// </summary>
    internal TableRecord m_tableRecord = null;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="iRowNumber"></param>
    /// <param name="height"></param>
    /// <param name="xfIndex"></param>
    public RowStorage( int iRowNumber, int height, int xfIndex )
    {
      //m_usRowNumber = ( ushort )iRowNumber;
      m_usHeight = ( ushort )height;
      ExtendedFormatIndex = ( ushort )xfIndex;
    }
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!Disposed)
            {
                if (m_dataProvider != null)
                {
                    // IntPtrDataProvider should free this memory block.
                    m_dataProvider.Dispose();
                    m_dataProvider = null;
                    m_iFirstColumn = -1;
                    m_iLastColumn = -1;
                    m_iUsedSize = -1;
                }

                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
    public void Dispose()
    {
        Dispose(true);
    }
    /// <summary>
    /// Disposes this object.
    /// </summary>
    ~RowStorage()
    {
      Dispose(false);
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Zero-based index of the first column. Read/write.
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
    /// Zero-based index of the last column. Read/write.
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
      set
      {
        m_iLastColumn = value;
      }
    }
    /// <summary>
    /// Size of used data. Read-only.
    /// </summary>
    public int UsedSize
    {
      get
      {
        return m_iUsedSize;
      }
#if DEBUG
      set
      {
        m_iUsedSize = value;
      }
#endif
    }
    /// <summary>
    /// Size of allocated data. Read-only.
    /// </summary>
    public int DataSize
    {
      get
      {
        return ( m_dataProvider != null )
          ? m_dataProvider.Capacity
          : 0;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether storage possibly contains Rk or Blank records.
    /// </summary>
    public bool HasRkBlank
    {
      get
      {
        return ( m_options & StorageOptions.HasRKBlank ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= StorageOptions.HasRKBlank;
        }
        else
        {
          m_options &= ~StorageOptions.HasRKBlank;
        }
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether storage possibly contains MultiRk or MultiBlank records.
    /// </summary>
    public bool HasMultiRkBlank
    {
      get
      {
        return ( m_options & StorageOptions.HasMultiRKBlank ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= StorageOptions.HasMultiRKBlank;
        }
        else
        {
          m_options &= ~StorageOptions.HasMultiRKBlank;
        }
      }
    }
    /// <summary>
    /// Indicates that storage was disposed. Read-only.
    /// </summary>
    private bool Disposed
    {
      get
      {
        return ( m_options & StorageOptions.Disposed ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= StorageOptions.Disposed;
        }
        else
        {
          m_options &= ~StorageOptions.Disposed;
        }
      }
    }
    /// <summary>
    /// Zero-based index of the first column. Read/write.
    /// </summary>
    public bool IsWrapText
    {
        get
        {
            return m_isWrapText;
        }
        set
        {
            m_isWrapText = value;
        }
    }
    /// <summary>
    /// Returns internal data provider. Read-only.
    /// </summary>
    public DataProvider Provider
    {
      get
      {
        return m_dataProvider;
      }
    }
    /// <summary>
    /// Returns size of the cell position block. Read-only.
    /// </summary>
    public int CellPositionSize
    {
      get
      {
        return ( m_version == ExcelVersion.Excel97to2003 ) ?
          4 :
          8;
          //m_iCellPositionSize;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelVersion Version
    {
      get
      {
        return m_version;
          //( m_iCellPositionSize == 4 ) ?
          //ExcelVersion.Excel97to2003 :
          //ExcelVersion.Excel2007;
      }
    }
    internal bool HasRowHeight
    {
        get
        {
            return m_hasRowHeight;
        }
        set
        {
            m_hasRowHeight = value;
        }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns row storage enumerator.
    /// </summary>
    /// <param name="recordExtractor">Record extractor to get Biff records from.</param>
    /// <returns></returns>
    public IEnumerator GetEnumerator( RecordExtractor recordExtractor )
    {
      if( recordExtractor == null )
        throw new ArgumentNullException( "recordExtractor" );

      return new RowStorageEnumerator( this, recordExtractor );
    }
    /// <summary>
    /// Sets cell style.
    /// </summary>
    /// <param name="iRow">Zero-based row index.</param>
    /// <param name="iColumn">Zero-based column index.</param>
    /// <param name="iXFIndex">Index of the extended format to set.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void SetCellStyle( int iRow, int iColumn, int iXFIndex, int iBlockSize )
    {
      bool bFound;
      int iOffset = LocateRecord( iColumn, out bFound );

      if( !bFound )
      {
        ICellPositionFormat cell = UtilityMethods.CreateCell( iRow, iColumn, TBIFFRecord.Blank );
        cell.ExtendedFormatIndex = ( ushort )iXFIndex;
        //SetCellRecord( iRow, iColumn, cell );
        SetRecord( iColumn, cell, iBlockSize );
      }
      else
      {
        //throw new NotImplementedException();
        TBIFFRecord recordCode = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( recordCode == TBIFFRecord.MulRK )
        {
          SetXFIndexMulti( iOffset, ( ushort )iXFIndex, iColumn, MulRKRecord.DEF_SUB_ITEM_SIZE );
        }
        else if( recordCode == TBIFFRecord.MulBlank )
        {
          SetXFIndexMulti( iOffset, ( ushort )iXFIndex, iColumn, MulBlankRecord.DEF_SUB_ITEM_SIZE );
        }
        else
        {
          SetXFIndex( iOffset, ( ushort )iXFIndex );
        }
      }
    }
    /// <summary>
    /// Creates new record based on the data at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the record.</param>
    /// <returns></returns>
    [ CLSCompliant( false ) ]
    public BiffRecordRaw GetRecordAtOffset( int iOffset )
    {
      if( iOffset < 0 || iOffset >= m_iUsedSize )
        throw new ArgumentOutOfRangeException( "iOffset" );

      return BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
    }
    /// <summary>
    /// Returns record corresponding to the specified cell.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <returns>Record corresponding to the specified cell.</returns>
    [CLSCompliant( false )]
    public ICellPositionFormat GetRecord( int iColumnIndex, int iBlockSize )
    {
#if DEBUG
      try
      {
#endif
        if( HasMultiRkBlank )
          Decompress( false, iBlockSize );

        if( m_iFirstColumn < 0 || iColumnIndex < m_iFirstColumn
          || iColumnIndex > m_iLastColumn )
        {
          return null;
        }

        bool bFound;
        //bool bMulti;

        int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );
        BiffRecordRaw record = null;

        if( bFound )
        {
          int iCode = m_dataProvider.ReadInt16( iOffset );
          int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

          record = BiffRecordFactory.GetRecord( iCode );
          record.Length = iLength;
          record.ParseStructure( m_dataProvider, iOffset + 4, iLength, Version );
        }

        return record as ICellPositionFormat;
#if DEBUG
      }
      catch( Exception ex )
      {
#if !(WINRT )
        using( StreamWriter writer = new StreamWriter( "D:\\RowStorage.log", true ) )
        {
          Dump( writer, ex );
        }
#endif
        throw;
      }
#endif
    }
    /// <summary>
    /// Returns record corresponding to the specified cell.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <param name="recordExtractor">Object used to extract records from internal data array.</param>
    /// <returns>Record corresponding to the specified cell.</returns>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat GetRecord( int iColumnIndex, int iBlockSize,
      RecordExtractor recordExtractor )
    {
#if DEBUG
      try
      {
#endif
        if( HasMultiRkBlank ) Decompress( false, iBlockSize );

        if( m_iFirstColumn < 0 || iColumnIndex < m_iFirstColumn
          || iColumnIndex > m_iLastColumn )
        {
          return null;
        }

        bool bFound;
        //bool bMulti;

        int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );
        BiffRecordRaw record = null;

        if( bFound )
        {
          int iCode = m_dataProvider.ReadInt16( iOffset );
          int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

          record = recordExtractor.GetRecord( iCode );
          record.Length = iLength;
          record.ParseStructure( m_dataProvider, iOffset + 4, iLength, Version );
        }

        return record as ICellPositionFormat;
#if DEBUG
      }
      catch( Exception ex )
      {
#if !(WINRT )
        using( StreamWriter writer = new StreamWriter( "D:\\RowStorage.log", true ) )
        {
          Dump( writer, ex );
        }
#endif
          throw;
        
      }
#endif
    }
    /// <summary>
    /// Sets cell data.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index of the cell.</param>
    /// <param name="cell">Cell to set.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    [ CLSCompliant( false ) ]
    public void SetRecord( int iColumnIndex, ICellPositionFormat cell, int iBlockSize )
    {
      //      TBIFFRecord code = ( cell == null )
      //        ? TBIFFRecord.Unknown
      //        : cell.TypeCode;
      //
      //      if( UsedSize > 0 && ( code == TBIFFRecord.RK || code == TBIFFRecord.Blank ) )
      //      {
      //        TrySetMultiRecord( iColumnIndex, cell );
      //      }
      //      else
      if( HasMultiRkBlank ) Decompress( false, iBlockSize );
      SetOrdinaryRecord( iColumnIndex, cell, iBlockSize );
    }

    /// <summary>
    /// Removes all data saving cells formatting.
    /// </summary>
    public void ClearData()
    {
      m_iFirstColumn = -1;
      m_iLastColumn = -1;
      m_iUsedSize = 0;
      ExtendedFormatIndex = 0;
    }
    /// <summary>
    /// Returns formula string value.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <param name="strValue">Value to set.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void SetFormulaStringValue( int iColumnIndex, string strValue, int iBlockSize )
    {
      int iFormulaOffset;
      int iOffset = RemoveFormulaStringValue( iColumnIndex, out iFormulaOffset );

      if( strValue == null )
      {
        return;
      }

      if( iOffset < 0 )
        throw new NotSupportedException( "Need formula cell to set FormulaStringValue." );

      FormulaRecord.SetStringValue( m_dataProvider, iFormulaOffset, Version );
      StringRecord stringRecord = ( StringRecord )BiffRecordFactory.GetRecord( TBIFFRecord.String );
      stringRecord.Value = strValue;
      int iRequiredSize = stringRecord.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;

      InsertRecordData( iOffset, 0, iRequiredSize, stringRecord, iBlockSize );
    }
    /// <summary>
    /// Sets array record.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <param name="array">Record to set.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    [ CLSCompliant( false ) ]
    public void SetArrayRecord( int iColumnIndex, ArrayRecord array, int iBlockSize )
    {
      bool bFound;
      //bool bMulti;

      int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );

      if( !bFound )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Cannot find record with specified index" );

      TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code != TBIFFRecord.Formula )
        throw new ArgumentOutOfRangeException( "RecordCode", "Cannot find FormulaRecord with specified column index" );

      iOffset = MoveNext( iOffset );

      if( iOffset < m_iUsedSize )
      {
        code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.Array )
        {
          RemoveRecord( iOffset );
        }
      }

      if( array != null )
      {
        int iRequiredSize = BiffRecordRaw.DEF_HEADER_SIZE + array.GetStoreSize( Version );
        InsertRecordData( iOffset, 0, iRequiredSize, array, iBlockSize );
      }
    }
    /// <summary>
    /// Returns array record.
    /// </summary>
    /// <param name="iOffset">Offset to the record.</param>
    /// <returns>Formula array record.</returns>
    [ CLSCompliant( false ) ]
    public ArrayRecord GetArrayRecordByOffset( int iOffset )
    {
      TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code != TBIFFRecord.Formula )
        return null;

      iOffset = MoveNext( iOffset );

      if( iOffset < m_iUsedSize )
      {
        code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.Array )
        {
          return ( ArrayRecord )BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
        }
      }

      return null;
    }
    /// <summary>
    /// Returns array record.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <returns>Corresponding array record.</returns>
    [ CLSCompliant( false ) ]
    public ArrayRecord GetArrayRecord( int iColumnIndex )
    {
      bool bFound;
      //bool bMulti;

      int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );

      if( !bFound )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Cannot find record with specified index" );

      return GetArrayRecordByOffset( iOffset );
    }
    /// <summary>
    /// Creates copy of the storage.
    /// </summary>
    /// <returns>RowStorage with copied data.</returns>
    public object Clone()
    {
      IntPtr heapHandle = GetHeapHandle();

      return Clone( heapHandle );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IntPtr GetHeapHandle()
    {
#if !SILVERLIGHT && !WINRT && !WP
      IntPtrDataProvider dataProvider =
        m_dataProvider as IntPtrDataProvider;

      return ( dataProvider != null ) ?
        dataProvider.HeapHandle :
        IntPtr.Zero;
#else
      return IntPtr.Zero;
#endif
    }
    /// <summary>
    /// Creates copy of the storage.
    /// </summary>
    /// <returns>RowStorage with copied data.</returns>
    public object Clone( IntPtr heapHandle )
    {
      RowStorage result = new RowStorage( 0, m_usHeight, m_usXFIndex );

      if( m_dataProvider != null && m_iUsedSize > 0 && m_iFirstColumn >= 0 )
      {
        result.m_dataProvider = ApplicationImpl.CreateDataProvider( heapHandle );
        result.EnsureSize( DataSize, 1 );
        m_dataProvider.CopyTo( 0, result.m_dataProvider, 0, m_iUsedSize );
      }

      result.m_iFirstColumn = m_iFirstColumn;
      result.m_iLastColumn = m_iLastColumn;
      result.m_iUsedSize = m_iUsedSize;
      result.m_options = m_options;
      //result.m_usHeight = m_usHeight;
      result.m_optionFlags = m_optionFlags;
      //result.m_iCellPositionSize = m_iCellPositionSize;
      result.m_version = m_version;
      result.m_usXFIndex = m_usXFIndex;

      return result;
    }
    /// <summary>
    /// Creates copy of the storage.
    /// </summary>
    /// <param name="iStartColumn">Zero-based index of the first column to copy.</param>
    /// <param name="iEndColumn">Zero-based index of the last column to copy.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <returns>RowStorage with copied data.</returns>
    public RowStorage Clone( int iStartColumn, int iEndColumn, int iBlockSize )
    {
      RowStorage result = new RowStorage( /*m_usRowNumber*/0, m_usHeight, m_usXFIndex );
      result.m_options = m_options;
      //result.m_usHeight = m_usHeight;
      result.m_optionFlags = m_optionFlags;
      //result.m_iCellPositionSize = m_iCellPositionSize;
      result.m_version = m_version;
      result.m_usXFIndex = m_usXFIndex;

      if( m_iUsedSize > 0 )
      {

        iStartColumn = Math.Max( m_iFirstColumn, iStartColumn );
        iEndColumn = Math.Min( m_iLastColumn, iEndColumn );

        if( iStartColumn > iEndColumn ) return null;

        Decompress( false, iBlockSize );
        Point pOffsets = GetOffsets( iStartColumn, iEndColumn, out iStartColumn, out iEndColumn );

        if( iStartColumn < 0 ) return null;

        int iStartOffset = pOffsets.X;
        int iEndOffset = pOffsets.Y;

        int iDataSize = iEndOffset - iStartOffset;

        if( iDataSize > 0 )
        {
          //DataProvider newProvider = m_dataProvider.CreateProvider();
          result.CreateDataProvider( GetHeapHandle() );
          result.EnsureSize( iDataSize, iBlockSize );
          //IntPtr ptrStart = ( IntPtr )( m_dataProvider.DataPointer.ToInt64() + iStartOffset );
          //Memory.CopyMemory( result.m_dataProvider.DataPointer, ptrStart, iDataSize );
          m_dataProvider.CopyTo( iStartOffset, result.m_dataProvider, 0, iDataSize );
        }

        result.m_iFirstColumn = iStartColumn;
        result.m_iLastColumn = iEndColumn;
        result.m_iUsedSize = iDataSize;
      }

      // TODO: find out whether we have to copy default row style here.

      result.m_iCurrentColumn = -1;
      result.m_iCurrentOffset = -1;

      return result;
    }
    /// <summary>
    /// Creates copy of the storage.
    /// </summary>
    /// <param name="sourceSST">Source strings table.</param>
    /// <param name="destSST">Destination strings table</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicNameIndexes">New name indexes.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>RowStorage with copied data.</returns>
    public RowStorage Clone( SSTDictionary sourceSST, SSTDictionary destSST,
      Dictionary<int, int> hashExtFormatIndexes, Dictionary<string, string> hashWorksheetNames,
      Dictionary<int, int> dicNameIndexes, Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dictExternSheets )
    {
      ApplicationImpl application = ( ApplicationImpl )destSST.Workbook.Application;
      RowStorage result = new RowStorage( /*m_usRowNumber*/0, application.StandardHeightInRowUnits, m_usXFIndex );

      result.m_iFirstColumn = m_iFirstColumn;
      result.m_iLastColumn = m_iLastColumn;
      result.m_iUsedSize = m_iUsedSize;
      result.m_options = m_options;
      result.m_usHeight = m_usHeight;
      result.m_optionFlags = m_optionFlags;
      //result.m_iCellPositionSize = m_iCellPositionSize;
      result.m_version = m_version;
      result.m_usXFIndex = m_usXFIndex;

      if( !( m_dataProvider == null || m_iUsedSize <= 0 || m_iFirstColumn < 0 ) )
      {
        result.m_dataProvider = ApplicationImpl.CreateDataProvider( destSST.Workbook.HeapHandle );
        result.m_dataProvider.EnsureCapacity( m_iUsedSize );
        //Memory.CopyMemory( result.m_dataProvider.DataPointer, m_dataProvider.DataPointer, m_iUsedSize );
        m_dataProvider.CopyTo( 0, result.m_dataProvider, 0, m_iUsedSize );

      }

      // Now we have to update indexes.
      result.UpdateRecordsAfterCopy( sourceSST, destSST, hashExtFormatIndexes,
        hashWorksheetNames, dicNameIndexes, dicFontIndexes, dictExternSheets );

      result.m_iCurrentColumn = -1;
      result.m_iCurrentOffset = -1;
      result.IsFormatted = IsFormatted;
      return result;
    }
    /// <summary>
    /// Creates copy of the storage.
    /// </summary>
    /// <param name="iStartColumn">Zero-based index of the first column to copy.</param>
    /// <param name="iEndColumn">Zero-based index of the last column to copy.</param>
    /// <param name="blockSize">Represents allocation block size.</param>
    public void Remove( int iStartColumn, int iEndColumn, int blockSize )
    {
      if( m_iFirstColumn < 0 ) return;

      iStartColumn = Math.Max( m_iFirstColumn, iStartColumn );
      iEndColumn = Math.Min( m_iLastColumn, iEndColumn );

      if( iStartColumn > iEndColumn ) return;

      Decompress( false, blockSize );
      Point pOffsets = GetOffsets( iStartColumn, iEndColumn, out iStartColumn, out iEndColumn );

      int iStartOffset = pOffsets.X;
      int iEndOffset = pOffsets.Y;
      int iSize = iEndOffset - iStartOffset;

      if( iSize <= 0 ) return;

      int iSizeToMove = m_iUsedSize - iEndOffset;

      if( iSizeToMove > 0 )
      {
        m_dataProvider.MoveMemory( iStartOffset, iEndOffset, iSizeToMove );
      }

      m_iUsedSize -= iSize;
      UpdateColumns();
    }
    /// <summary>
    /// Updates indexes in the array-entered formula.
    /// </summary>
    /// <param name="iColumn">Zero-based column index of the cell with array-entered formula.</param>
    /// <param name="iArrayRow">Zero-based row index of the array record.</param>
    /// <param name="iArrayColumn">Zero-based column index of the array record.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void SetArrayFormulaIndex( int iColumn, int iArrayRow, int iArrayColumn, int iBlockSize )
    {
      bool bFound;
      //bool bMulti;

      int iOffset = LocateRecord( iColumn, out bFound/*, out bMulti*/ );

      if( !bFound ) return;

      TBIFFRecord typeCode =( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( typeCode != TBIFFRecord.Formula ) return;

      FormulaRecord formula = BiffRecordFactory.GetRecord( m_dataProvider,
        iOffset, Version ) as FormulaRecord;

      Ptg[] arrPtgs = formula.ParsedExpression;

      if( arrPtgs == null || arrPtgs.Length == 0 ) return;

      ControlPtg control = arrPtgs[ 0 ] as ControlPtg;

      if( control == null ) return;

      control.RowIndex = iArrayRow;
      control.ColumnIndex = iArrayColumn;

      formula.ParsedExpression = arrPtgs;

      int iSize = formula.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      InsertRecordData( iOffset, iSize, iSize, formula, iBlockSize );
    }
    /// <summary>
    /// Indicates whether row contains record with specified index.
    /// </summary>
    /// <param name="iColumn">Zero-based column index to find.</param>
    /// <returns>True if required record was found.</returns>
    public bool Contains( int iColumn )
    {
      bool bFound;
      //bool bMulti;

      LocateRecord( iColumn, out bFound/*, out bMulti*/ );

      return bFound;
    }
    /// <summary>
    /// Inserts data from another row object.
    /// </summary>
    /// <param name="sourceRow">Source row data.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <param name="heapHandle">Heap handle that must be used to allocate memory.</param>
    public void InsertRowData( RowStorage sourceRow, int iBlockSize, IntPtr heapHandle )
    {
      if( sourceRow == null )
        throw new ArgumentNullException( "sourceRow" );

      if( sourceRow.m_iUsedSize <= 0 ) return;

      //m_iCellPositionSize = sourceRow.m_iCellPositionSize;
      m_version = sourceRow.m_version;
      int iStartColumn = sourceRow.FirstColumn;
      int iLastColumn = sourceRow.LastColumn;

      if( m_dataProvider == null || m_iUsedSize <= 0 )
      {
        m_iUsedSize = sourceRow.m_iUsedSize;
        CreateDataProvider( heapHandle );
        EnsureSize( m_iUsedSize, iBlockSize );
        //Memory.CopyMemory( m_dataProvider.DataPointer, sourceRow.m_dataProvider.DataPointer, m_iUsedSize );
        sourceRow.m_dataProvider.CopyTo( 0, m_dataProvider, 0, m_iUsedSize );
      }
      else
      {
        Remove( iStartColumn, iLastColumn, iBlockSize );

        bool bFound;
        //bool bMulti;
        int iOffset = LocateRecord( iStartColumn, out bFound/*, out bMulti*/ );
        int iSizeToMove = m_iUsedSize - iOffset;
        int iSizeToInsert = sourceRow.m_iUsedSize;

        EnsureSize( m_iUsedSize + iSizeToInsert, iBlockSize );

        //IntPtr ptrRowData = m_dataProvider.DataPointer;
        //IntPtr ptrSource = ( IntPtr )( ptrRowData.ToInt64() + iOffset );
        //IntPtr ptrDest = ( IntPtr )( ptrSource.ToInt64() + iSizeToInsert );

        if( iSizeToMove > 0 )
        {
          m_dataProvider.MoveMemory( iOffset + iSizeToInsert, iOffset, iSizeToMove );
          //API.RtlMoveMemory( ptrDest, ptrSource, iSizeToMove );
        }

        //Memory.CopyMemory( ptrSource, sourceRow.m_dataProvider.DataPointer, iSizeToInsert );
        sourceRow.m_dataProvider.CopyTo( 0, m_dataProvider, iOffset, iSizeToInsert );
        m_iUsedSize += iSizeToInsert;
      }

      if( m_iFirstColumn >= 0 )
      {
        m_iFirstColumn = Math.Min( m_iFirstColumn, iStartColumn );
        m_iLastColumn = Math.Max( m_iLastColumn, iLastColumn );
      }
      else
      {
        m_iFirstColumn = iStartColumn;
        m_iLastColumn = iLastColumn;
      }

      m_iCurrentOffset = -1;
      m_iCurrentColumn = -1;
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <param name="book">Parent workbook.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect,
      int iDestIndex, Rectangle destRect, int iBlockSize, WorkbookImpl book )
    {
      if( m_dataProvider == null || m_iUsedSize <= 0 || m_iFirstColumn < 0 ) return;

      int iOffset = 0;
      int iRowDelta = destRect.Top - sourceRect.Top;
      int iColDelta = destRect.Left - sourceRect.Left;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        // TODO: update code can be optimized.
        if( code == TBIFFRecord.Formula )
        {
          FormulaRecord formula = BiffRecordFactory.GetRecord( m_dataProvider,
            iOffset, Version ) as FormulaRecord;

          Ptg[] arrPtg = formula.ParsedExpression;
          formula.ParsedExpression = book.FormulaUtil.UpdateFormula( arrPtg, iCurIndex, iSourceIndex,
            sourceRect, iDestIndex, destRect, formula.Row + 1, formula.Column + 1 );

          //SetRecord( formula.Column, formula );
          InsertRecordData( iOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE,
            formula.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE, formula, iBlockSize );
          iLength = formula.GetStoreSize( Version );
        }
        else if( code == TBIFFRecord.Array )
        {
          ArrayRecord array = BiffRecordFactory.GetRecord( m_dataProvider,
            iOffset, Version ) as ArrayRecord;

          Ptg[] arrPtg = array.Formula;
          array.Formula = book.FormulaUtil.UpdateFormula( arrPtg, iCurIndex, iSourceIndex,
            sourceRect, iDestIndex, destRect, array.FirstRow + 1, array.FirstColumn + 1 );

          //SetArrayRecord( array.FirstCol, array );
          int iSize = iLength + BiffRecordRaw.DEF_HEADER_SIZE;
          InsertRecordData( iOffset, iSize, array.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE, array, iBlockSize );
          iLength = array.GetStoreSize( Version );
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Adds specified values to row and columns indexes.
    /// </summary>
    /// <param name="iDeltaRow">Value to add to row index.</param>
    /// <param name="iDeltaCol">Value to add to column index.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void RowColumnOffset( int iDeltaRow, int iDeltaCol, int iBlockSize )
    {
      int iOffset = 0;
      m_iFirstColumn += iDeltaCol;
      m_iLastColumn += iDeltaCol;
      //m_usRowNumber = ( ushort )( m_usRowNumber + iDeltaRow );

      while( iOffset < m_iUsedSize )
      {
        short sCode = m_dataProvider.ReadInt16( iOffset );
        TBIFFRecord code = ( TBIFFRecord )sCode;
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code == TBIFFRecord.Array )
        {
          ArrayRecord array = ( ArrayRecord )BiffRecordFactory.GetRecord(
            m_dataProvider, iOffset, Version );

          array.FirstColumn += iDeltaCol;
          array.LastColumn += iDeltaCol;
          array.FirstRow += iDeltaRow;
          array.LastRow += iDeltaRow;

          int iSize = iLength + BiffRecordRaw.DEF_HEADER_SIZE;
          InsertRecordData( iOffset, iSize, iSize, array, iBlockSize );
        }
        else if( code != TBIFFRecord.String )
        {
          int iRow = GetRow( iOffset );
          SetRow( iOffset, iRow + iDeltaRow );

          int iColumn = GetColumn( iOffset );
          SetColumn( iOffset, iColumn + iDeltaCol );

          //if( UtilityMethods.IndexOf( DEF_MULTIRECORDS, sCode ) != -1 )
          if( code == TBIFFRecord.MulBlank || code == TBIFFRecord.MulRK )
          {      
            MulBlankRecord.IncreaseLastColumn( m_dataProvider, iOffset, iLength, Version, iDeltaCol );
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="arrNewIndex">New indexes.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void UpdateNameIndexes( WorkbookImpl book, int[] arrNewIndex, int iBlockSize )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      int iOffset = 0;
      // TODO: this method can be optimized for large number of formula records -
      // we can use single instance of FormulaRecord instead of creating new each time.

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code == TBIFFRecord.Formula || code == TBIFFRecord.Array )
        {
          IFormulaRecord formula = ( IFormulaRecord )BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
          Ptg[] arrExpression = formula.Formula;

          if( book.FormulaUtil.UpdateNameIndex( arrExpression, arrNewIndex ) )
          {
            formula.Formula = arrExpression;
          }

          BiffRecordRaw record = ( BiffRecordRaw )formula;
          InsertRecordData( iOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE,
            record.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE, record, iBlockSize );

          iLength = record.GetStoreSize( Version );
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="dicNewIndex">New indexes.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void UpdateNameIndexes( WorkbookImpl book, IDictionary<int, int> dicNewIndex, int iBlockSize )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code == TBIFFRecord.Formula || code == TBIFFRecord.Array )
        {
          IFormulaRecord formula = ( IFormulaRecord )BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
          Ptg[] arrExpression = formula.Formula;

          if( book.FormulaUtil.UpdateNameIndex( arrExpression, dicNewIndex ) )
          {
            formula.Formula = arrExpression;
          }

          BiffRecordRaw record = ( BiffRecordRaw )formula;
          InsertRecordData( iOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE,
            record.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE, record, iBlockSize );

          iLength = record.GetStoreSize( Version );
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Replaces all shared formula with ordinary formula.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="shared">Shared formula that should be replaced.</param>
    [ CLSCompliant( false ) ]
    public void ReplaceSharedFormula( WorkbookImpl book, int row, int column, SharedFormulaRecord shared )
    {
      int iBlockSize = book.Application.RowStorageAllocationBlockSize;
      if( HasMultiRkBlank ) Decompress( false, iBlockSize );

      if( m_iFirstColumn > shared.LastColumn || m_iLastColumn < shared.FirstColumn )
        return;

      bool bFound;
      //bool bMulti;
      int iOffset = LocateRecord( shared.FirstColumn, out bFound/*, out bMulti*/ );

      //if( !bFound )
      //{
      //  return;
      //  //        if( shared.FirstCol > m_iLastColumn || shared.LastCol < m_iFirstColumn )
      //  //          return;
      //  //
      //  //        throw new ParseException( "Cannot parse shared formula - formula record not found" );
      //}

      int iCurColumn = -1;

      while( iOffset < m_iUsedSize )
      {
        //int iCode = m_dataProvider.ReadInt16( iOffset );
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
          //if( iCode != DEF_ARRAY_CODE && iCode != DEF_STRING_CODE )
        {
          iCurColumn = GetColumn( iOffset );

          if( iCurColumn > shared.LastColumn ) return;

          if( iCurColumn >= shared.FirstColumn && code == TBIFFRecord.Formula )
          {
            FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
            formula.CalculateOnOpen = true;
            formula.RecalculateAlways = true;
            formula.PartOfSharedFormula = false;

            Ptg[] arrPtg = formula.ParsedExpression;

            if( arrPtg != null &&
              arrPtg.Length == 1 &&
              arrPtg[ 0 ].TokenCode == FormulaToken.tExp )
            {
              ControlPtg token = ( ControlPtg )arrPtg[ 0 ];

              if( token.RowIndex == row && token.ColumnIndex == column )
              {
                arrPtg = FormulaUtil.ConvertSharedFormulaTokens( shared, book, formula.Row, iCurColumn );
                formula.ParsedExpression = arrPtg;
                int iNewLen = formula.GetStoreSize( Version );

                InsertRecordData( iOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE,
                  iNewLen + BiffRecordRaw.DEF_HEADER_SIZE, formula, iBlockSize );

                iLength = iNewLen;
              }
            }
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      if( arrNewIndexes == null )
        throw new ArgumentNullException( "arrNewIndexes" );

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code == TBIFFRecord.LabelSST )
        {
          int iSSTIndex = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );
          iSSTIndex = arrNewIndexes[ iSSTIndex ];
          LabelSSTRecord.SetSSTIndex( m_dataProvider, iOffset, iSSTIndex, Version );
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFirstColumn"></param>
    /// <param name="iLastColumn"></param>
    /// <param name="findValue"></param>
    /// <param name="flags"></param>
    /// <param name="iErrorCode"></param>
    /// <param name="bIsFindFirst"></param>
    /// <param name="book"></param>
    /// <returns></returns>
    public List<long> Find( int iFirstColumn, int iLastColumn, string findValue,
      ExcelFindType flags, int iErrorCode, bool bIsFindFirst, WorkbookImpl book )
    {
        return Find(iFirstColumn, iLastColumn, findValue, flags, ExcelFindOptions.None, iErrorCode, bIsFindFirst, book);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFirstColumn"></param>
    /// <param name="iLastColumn"></param>
    /// <param name="findValue"></param>
    /// <param name="flags"></param>
    /// <param name="iErrorCode"></param>
    /// <param name="bIsFindFirst"></param>
    /// <param name="book"></param>
    /// <returns></returns>
    public List<long> Find(int iFirstColumn, int iLastColumn, string findValue,
      ExcelFindType flags,ExcelFindOptions findOptions, int iErrorCode, bool bIsFindFirst, WorkbookImpl book)
    {
        //      bool bIsTextIsError = FormulaUtil.ErrorNameToCode.Contains( findValue );
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);
        bool bIsFormula = (flags & ExcelFindType.Formula) == ExcelFindType.Formula;
        bool bIsFormulaStringValue = (flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue;

        if (!(bIsText || bIsFormula || bIsError || bIsFormulaStringValue))
            throw new ArgumentException("Parameter flags is not valid.", "flags");

        List<long> result = new List<long>();

        if (m_dataProvider == null || m_iUsedSize <= 0
          || iLastColumn < m_iFirstColumn || iFirstColumn > m_iLastColumn)
        {
            return result;
        }
        
        bool bFound;
        //bool bMulti;
        int iOffset = LocateRecord(iFirstColumn, out bFound/*, out bMulti*/ );

        while (iOffset < m_iUsedSize)
        {
            TBIFFRecord code = (TBIFFRecord)m_dataProvider.ReadInt16(iOffset);
            int iLength = m_dataProvider.ReadInt16(iOffset + 2);
            int iRow = GetRow(iOffset);
            int iColumn = GetColumn(iOffset);
            int iRecordLastColumn = iColumn;
            bFound = false;

            if (iColumn > iLastColumn) break;

            switch (code)
            {
                case TBIFFRecord.Label:
                    if (bIsText)
                    {
                        LabelRecord label = (LabelRecord)BiffRecordFactory.GetRecord(
                          m_dataProvider, iOffset, Version);
                        if (findOptions == ExcelFindOptions.None)
                            bFound = (label.Label==findValue);
                        else
                            bFound = CheckStringValue(label.Label, findValue, findOptions,book);
                    }
                    break;

                case TBIFFRecord.LabelSST:
                    int iSSTIndex = LabelSSTRecord.GetSSTIndex(m_dataProvider, iOffset, Version);
                    string strText = book.InnerSST[iSSTIndex].Text;
                    if (findOptions == ExcelFindOptions.None)
                        bFound = (strText.ToLower().Contains(findValue.ToLower()));
                    else
                        bFound = CheckStringValue(strText, findValue, findOptions,book);
                    break;

                case TBIFFRecord.BoolErr:
                    if (bIsError)
                    {
                        BoolErrRecord boolErr = (BoolErrRecord)BiffRecordFactory.GetRecord(
                          m_dataProvider, iOffset, Version);

                        bFound = (boolErr.IsErrorCode && boolErr.BoolOrError == iErrorCode);
                    }
                    break;

                case TBIFFRecord.Formula:
                    if (bIsFormula)
                    {
                        FormulaRecord formula = (FormulaRecord)BiffRecordFactory.GetRecord(
                          m_dataProvider, iOffset, Version);
                        string strFormula = '=' + book.FormulaUtil.ParseFormulaRecord(formula);
                        if (findOptions == ExcelFindOptions.None)
                            bFound = strFormula.ToLower().Contains(findValue.ToLower());
                        else
                            bFound = CheckStringValue(strFormula, findValue, findOptions,book);
                    }

                    int iNewOffset = iOffset + iLength + BiffRecordRaw.DEF_HEADER_SIZE;

                    if (iNewOffset < m_iUsedSize)
                    {
                        code = (TBIFFRecord)m_dataProvider.ReadInt16(iNewOffset);

                        if (code == TBIFFRecord.Array)
                        {
                            iOffset = iNewOffset;
                            iNewOffset = MoveNext(iNewOffset);
                            code = (TBIFFRecord)m_dataProvider.ReadInt16(iNewOffset);
                        }

                        if (code == TBIFFRecord.String)
                        {
                            iOffset = iNewOffset;

                            if (bIsFormulaStringValue)
                            {
                                StringRecord stringRecord = (StringRecord)BiffRecordFactory.GetRecord(
                                  m_dataProvider, iNewOffset, Version);
                                if (findOptions == ExcelFindOptions.None)
                                    bFound = (stringRecord.Value==findValue);
                                else
                                    bFound = CheckStringValue(stringRecord.Value, findValue, findOptions,book);
                            }
                        }
                    }

                    break;
            }

            if (bFound)
            {
                long lCellIndex = RangeImpl.GetCellIndex(iColumn + 1, iRow + 1);
                result.Add(lCellIndex);

                if (bIsFindFirst) break;
            }

            iOffset = MoveNext(iOffset);
        }        
        return result;

    }
    /// <summary>
    /// Checks the string value.
    /// </summary>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <param name="options">The options.</param>
    /// <param name="book">The book.</param>
    /// <returns></returns>
    private bool CheckStringValue(string first, string second, ExcelFindOptions options,WorkbookImpl book)
    {
        bool result = false;
        StringComparison matchCase = (options & ExcelFindOptions.MatchCase) != 0 ? 
                                      StringComparison.CurrentCulture 
                                      :StringComparison.CurrentCultureIgnoreCase;
        bool matchWholeWord = (options & ExcelFindOptions.MatchEntireCellContent) != 0;
        if(book.IsStartsOrEndsWith !=null && !matchWholeWord)
        {
            return book.IsStartsOrEndsWith == true ?
                first.StartsWith(second, matchCase)
                : first.EndsWith(second, matchCase);
        }
        if (matchWholeWord)
            result = (first == second) ? true : false;
        else
            result = (first.IndexOf(second, 0, matchCase) == 0 ? true : false);

        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFirstColumn"></param>
    /// <param name="iLastColumn"></param>
    /// <param name="findValue"></param>
    /// <param name="flags"></param>
    /// <param name="bIsFindFirst"></param>
    /// <param name="book"></param>
    /// <returns></returns>
    public List<long> Find( int iFirstColumn, int iLastColumn, double findValue,
      ExcelFindType flags, bool bIsFindFirst, WorkbookImpl book )
    {
      bool bFormula = ( ( flags & ExcelFindType.FormulaValue ) != 0 );
      bool bNumber = ( ( flags & ExcelFindType.Number ) != 0 );

      if( !( bFormula || bNumber ) )
        throw new ArgumentException( "Parameter flags is not valid.", "flags" );

      List<long> result = new List<long>();

      if( m_dataProvider == null || m_iUsedSize <= 0
        || iLastColumn < m_iFirstColumn || iFirstColumn > m_iLastColumn )
      {
        return result;
      }

      // TODO: this can be optimized.
      Decompress( false, book.Application.RowStorageAllocationBlockSize );

      bool bFound;
      //bool bMulti;
      int iOffset = LocateRecord( iFirstColumn, out bFound/*, out bMulti*/ );

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        int iRow = GetRow( iOffset );
        int iColumn = GetColumn( iOffset );
        int iRecordLastColumn = iColumn;
        bFound = false;

        if( iColumn > iLastColumn ) break;

        switch( code )
        {
          case TBIFFRecord.RK:
          case TBIFFRecord.Number:
            if( bNumber )
            {
              IDoubleValue doubleValue = ( IDoubleValue )BiffRecordFactory.GetRecord(
                m_dataProvider, iOffset, Version );

              bFound = ( doubleValue.DoubleValue == findValue );
            }

            break;

          case TBIFFRecord.Formula:
            if( bFormula )
            {
              FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord(
                m_dataProvider, iOffset, Version );

              bFound = ( formula.Value == findValue );
            }

            int iNewOffset = iOffset + iLength + BiffRecordRaw.DEF_HEADER_SIZE;

            if( iNewOffset < m_iUsedSize )
            {
              code = ( TBIFFRecord )m_dataProvider.ReadInt16( iNewOffset );

              while( code == TBIFFRecord.Array || code == TBIFFRecord.String )
              {
                iOffset = iNewOffset;
                iNewOffset = MoveNext( iNewOffset );
                code = ( TBIFFRecord )m_dataProvider.ReadInt16( iNewOffset );
              }
            }

            break;
        }

        if( bFound )
        {
          long lCellIndex = RangeImpl.GetCellIndex( iColumn + 1, iRow + 1 );
          result.Add( lCellIndex );

          if( bIsFindFirst ) break;
        }

        iOffset = MoveNext( iOffset );
      }

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFirstColumn"></param>
    /// <param name="iLastColumn"></param>
    /// <param name="findValue"></param>
    /// <param name="bError"></param>
    /// <param name="bIsFindFirst"></param>
    /// <param name="book"></param>
    /// <returns></returns>
    public List<long> Find( int iFirstColumn, int iLastColumn, byte findValue,
      bool bError, bool bIsFindFirst, WorkbookImpl book )
    {
      List<long> result = new List<long>();

      if( m_dataProvider == null || m_iUsedSize <= 0
        || iLastColumn < m_iFirstColumn || iFirstColumn > m_iLastColumn )
      {
        return result;
      }

      bool bFound;
      //bool bMulti;
      int iOffset = LocateRecord( iFirstColumn, out bFound/*, out bMulti*/ );

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        int iRow = GetRow( iOffset );
        int iColumn = GetColumn( iOffset );
        int iRecordLastColumn = iColumn;
        bFound = false;

        if( iColumn > iLastColumn ) break;

        switch( code )
        {
          case TBIFFRecord.BoolErr:
            BoolErrRecord boolErr = ( BoolErrRecord )BiffRecordFactory.GetRecord(
              m_dataProvider, iOffset, Version );

            bFound = ( boolErr.IsErrorCode == bError && boolErr.BoolOrError == findValue );
            break;

          case TBIFFRecord.Formula:
            // TODO: here we can check also formula boolean value.
            int iNewOffset = iOffset + iLength + BiffRecordRaw.DEF_HEADER_SIZE;

            if( iNewOffset < m_iUsedSize )
            {
              code = ( TBIFFRecord )m_dataProvider.ReadInt16( iNewOffset );

              while( iNewOffset < m_iUsedSize && code == TBIFFRecord.Array || code == TBIFFRecord.String )
              {
                iOffset = iNewOffset;
                iNewOffset = MoveNext( iNewOffset );
                code = ( TBIFFRecord )m_dataProvider.ReadInt16( iNewOffset );
              }
            }

            break;
        }

        if( bFound )
        {
          long lCellIndex = RangeImpl.GetCellIndex( iColumn + 1, iRow + 1 );
          result.Add( lCellIndex );

          if( bIsFindFirst ) break;
        }

        iOffset = MoveNext( iOffset );
      }

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictIndexes"></param>
    /// <param name="arrRanges"></param>
    public void Find( Dictionary<int, object> dictIndexes, List<long> arrRanges )
    {
      if( dictIndexes == null || dictIndexes.Count == 0 || m_iUsedSize <= 0 )
        return;

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.LabelSST )
        {
          int iSSTIndex = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );

          if( dictIndexes.ContainsKey( iSSTIndex ) )
          {
            //IRange range = InnerGetCell( cell.Column + 1, cell.Row + 1 );
            //arrRanges.Add( range );
            int iRow = GetRow( iOffset ) + 1;
            int iColumn = GetColumn( iOffset ) + 1;
            arrRanges.Add( RangeImpl.GetCellIndex( iColumn, iRow ) );
          }
        }

        iOffset = MoveNext( iOffset );
      }
    }
    /// <summary>
    /// Moves pointer to the next record.
    /// </summary>
    /// <param name="iOffset">Offset of the current record.</param>
    /// <returns>Offset to the next record.</returns>
    public int MoveNextCell( int iOffset )
    {
      if( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = TBIFFRecord.Unknown;

        do
        {
          int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
          iOffset += BiffRecordRaw.DEF_HEADER_SIZE + iLength;

          if( iOffset >= m_iUsedSize )
            break;

          code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        }
        while( iOffset < m_iUsedSize && ( code == TBIFFRecord.Array || code == TBIFFRecord.String ) );
      }

      return iOffset;
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void UpdateExtendedFormatIndex( Dictionary<int, int> dictFormats, int iBlockSize )
    {
      if( m_iUsedSize <= 0 ) return;

      int iOffset = 0;
      int iNewIndex;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank )
        {
          // TODO: implement this
          throw new NotImplementedException();
        }
        else if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          int iIndex = GetXFIndex( iOffset, false );

          if( dictFormats.TryGetValue( iIndex, out iNewIndex ) )
          {
            SetXFIndex( iOffset, ( ushort )iNewIndex );
          }
        }

        iOffset = MoveNext( iOffset );
      }

      int iXFIndex = ExtendedFormatIndex;

      if( dictFormats.TryGetValue( iXFIndex, out iNewIndex ) )
      {
        ExtendedFormatIndex = ( ushort )iNewIndex;
      }
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="arrFormats">Array with updated extended formats.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void UpdateExtendedFormatIndex( int[] arrFormats, int iBlockSize )
    {
      if( m_iUsedSize <= 0 )
        return;

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank )
        {
          // TODO: implement this
          throw new NotImplementedException();
        }
        else if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          int iIndex = GetXFIndex( iOffset, false );
          iIndex = arrFormats[ iIndex ];
          SetXFIndex( iOffset, ( ushort )iIndex );
        }

        iOffset = MoveNext( iOffset );
      }

      int iXFIndex = ExtendedFormatIndex;
      ExtendedFormatIndex = ( ushort )arrFormats[ iXFIndex ];
    }
    /// <summary>
    /// This method updates indexes to the extended formats after version change.
    /// </summary>
    /// <param name="maxCount">New restriction for maximum possible XF index.</param>
    /// <param name="defaultXF">Index to the default extended format.</param>
    public void UpdateExtendedFormatIndex( int maxCount, int defaultXF )
    {
      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          ushort usCurrentIndex = GetXFIndex( iOffset , false);

          if( usCurrentIndex >= maxCount )
          {
            SetXFIndex( iOffset, ( ushort )defaultXF );
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Updates LabelSST indexes after SST record parsing.
    /// </summary>
    /// <param name="dictUpdatedIndexes">Dictionary with indexes to update, key - old index, value - new index.</param>
    public void UpdateLabelSSTIndexes( Dictionary<int, int> dictUpdatedIndexes, IncreaseIndex method )
    {
      if( dictUpdatedIndexes == null )
        throw new ArgumentNullException( "dictUpdatedIndexes" );

      if( dictUpdatedIndexes.Count == 0 )
        return;

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord recordType = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( recordType == TBIFFRecord.LabelSST )
        {
          int index = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );

          int newIndex;

          if(  dictUpdatedIndexes.TryGetValue( index, out newIndex ) )
          {
            index = newIndex;
            LabelSSTRecord.SetSSTIndex( m_dataProvider, iOffset, index, Version );
            method( index );
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Appends storage with record's data.
    /// </summary>
    /// <param name="type">Record type.</param>
    /// <param name="length">Data length.</param>
    /// <param name="data">Array with required data.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void AppendRecordData( short type, short length, BiffRecordRaw data, int iBlockSize )
    {
      EnsureSize( m_iUsedSize + BiffRecordRaw.DEF_HEADER_SIZE + length, iBlockSize );

      m_dataProvider.WriteInt16( m_iUsedSize, type );
      m_iUsedSize += 2;

      m_dataProvider.WriteInt16( m_iUsedSize, length );
      m_iUsedSize += 2;

      //m_dataProvider.WriteBytes( m_iUsedSize, data, 0, length );
      data.InfillInternalData( m_dataProvider, m_iUsedSize, Version );
      m_iUsedSize += length;
    }
    /// <summary>
    /// Appends storage with record's data.
    /// </summary>
    /// <param name="type">Record type.</param>
    /// <param name="length">Data length.</param>
    /// <param name="data">Array with required data.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    [ CLSCompliant( false ) ]
    public void AppendRecordData( short type, short length, byte[] data, int iBlockSize )
    {
      EnsureSize( m_iUsedSize + BiffRecordRaw.DEF_HEADER_SIZE + length, iBlockSize );

      m_dataProvider.WriteInt16( m_iUsedSize, type );
      m_iUsedSize += 2;

      m_dataProvider.WriteInt16( m_iUsedSize, length );
      m_iUsedSize += 2;

      m_dataProvider.WriteBytes( m_iUsedSize, data, 0, length );
      m_iUsedSize += length;
    }
    /// <summary>
    /// Appends storage with record's data.
    /// </summary>
    /// <param name="length">Data length.</param>
    /// <param name="data">Array with required data.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void AppendRecordData( int length, byte[] data, int iBlockSize )
    {
      EnsureSize( m_iUsedSize + length, iBlockSize );
      m_dataProvider.WriteBytes( m_iUsedSize, data, 0, length );
      m_iUsedSize += length;
    }
    /// <summary>
    /// Appends storage with record's data.
    /// </summary>
    /// <param name="columnIndex">Zero-based column index to insert data.</param>
    /// <param name="length">Data length.</param>
    /// <param name="data">Array with required data.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void InsertRecordData( int columnIndex, int length, byte[] data, int iBlockSize )
    {
      EnsureSize( m_iUsedSize + length, iBlockSize );
      bool bFound;
      int iOffset = LocateRecord( columnIndex, out bFound );

      if( bFound )
      {
        //throw new ArgumentOutOfRangeException( "columnIndex", columnIndex, "Cell with the same column index already exist" );
        RemoveRecord( iOffset );
        InsertRecordData( columnIndex, length, data, iBlockSize );
      }
      else
      {
        m_dataProvider.MoveMemory( iOffset + length, iOffset, m_iUsedSize - iOffset );
        m_dataProvider.WriteBytes( iOffset, data, 0, length );
        m_iUsedSize += length;
      }

      //m_iFirstColumn = Math.Min( columnIndex, m_iFirstColumn );
    }
    /// <summary>
    /// Appends storage with record's data.
    /// </summary>
    /// <param name="records">Records to append.</param>
    /// <param name="arrBuffer">Temporary buffer for record data.</param>
    /// <param name="bIgnoreStyles">Indicates whether we have to ignore styles.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    [ CLSCompliant( false ) ]
    public void AppendRecordData( BiffRecordRaw[] records, byte[] arrBuffer, bool bIgnoreStyles, int iBlockSize )
    {
      int iCount = records.Length;
      int iRequiredSize = 0;

      for( int i = 0; i < iCount; i++ )
      {
        BiffRecordRaw record = records[ i ];
        iRequiredSize += BiffRecordRaw.DEF_HEADER_SIZE + record.GetStoreSize( Version );
      }

      EnsureSize( m_iUsedSize + iRequiredSize, iBlockSize );

      for( int i = 0; i < iCount; i++ )
      {
        BiffRecordRaw record = records[ i ];
        AppendRecordData( ( short )record.RecordCode, ( short )record.GetStoreSize( Version ), record, iBlockSize );
      }
    }
    /// <summary>
    /// Converts all MultiRK and MultiBlank records into set of RK / Blank records.
    /// </summary>
    /// <param name="bIgnoreStyles">Indicates whether styles must be ignored.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    public void Decompress( bool bIgnoreStyles, int iBlockSize )
    {
      if( !HasMultiRkBlank ) return;

      // Step 1. Find all records that need decompression and evaluate required data storage size.
      MulBlankRecord mulBlank = ( MulBlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulBlank );
      MulRKRecord mulRK = ( MulRKRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulRK );
      int iSizeDelta;
      List<int> arrOffsets = GetMultiRecordsOffsets( mulBlank, mulRK, out iSizeDelta );

      // Step 2. Resize storage
      if( iSizeDelta == 0 ) return;

      EnsureSize( m_iUsedSize + iSizeDelta, iBlockSize );

      // Step 3. Decompress records.
      DecompressStorage( arrOffsets, iSizeDelta, mulBlank, mulRK, bIgnoreStyles );

      HasMultiRkBlank = false;
      HasRkBlank = true;

      m_iCurrentColumn = -1;
      m_iCurrentOffset = -1;
    }
    /// <summary>
    /// Compresses data storage - combines several rk/blank records into one MulRK/MulBlank records.
    /// </summary>
    public void Compress()
    {
      if( m_iUsedSize <= 0 || !HasRkBlank ) return;

      WriteData writeData = new WriteData();
      writeData.UsedSize = 0;//iUsedSize;
      //writeData.Writer = writer;
      //writeData.Buffer = arrBuffer;

      DefragmentHelper enumRk = new DefragmentHelper( CompressRKRecords );
      DefragmentHelper enumBlank = new DefragmentHelper( CompressBlankRecords );
      DefragmentHelper enumOrdinary = new DefragmentHelper( CompressRecord );

      DefragmentDataStorage( enumRk, enumBlank, enumOrdinary, writeData );
      m_iUsedSize = writeData.UsedSize;
      HasRkBlank = false;
    }
    /// <summary>
    /// This method prepares row data, it updates indexes inside SSTDictionary,
    /// sets flags for RK and MultiRK records.
    /// </summary>
    /// <param name="sst">SSTDictionary to update.</param>
    /// <param name="arrShared">List that will receive all found shared formula records.</param>
    public bool PrepareRowData( SSTDictionary sst, ref Dictionary<long, SharedFormulaRecord> arrShared )
    {
      if( sst == null )
        throw new ArgumentNullException( "sst" );
      int iRow = -1;
      int iColumn = -1;

      int iOffset = 0;
      List<int> arrSharedPos = new List<int>();
      bool bResult = true;
      //bool bSharedNull = arrShared == null;

      while( iOffset < m_iUsedSize )
      {
        if( iOffset < 0 )
        {
          bResult = false;
          break;
          //throw new ArgumentOutOfRangeException();
        }

        int iValue = m_dataProvider.ReadInt32( iOffset );
        TBIFFRecord code = ( TBIFFRecord )( iValue & ushort.MaxValue );
        int iLength = iValue >> ExcelConstants.BitsInShort;
       
        switch( code )
        {
          case TBIFFRecord.LabelSST:
            int iLabelIndex = m_dataProvider.ReadInt32( iOffset
              + LabelSSTRecord.DEF_INDEX_OFFSET + BiffRecordRaw.DEF_HEADER_SIZE );
            sst.AddIncrease( iLabelIndex );
            break;

          case TBIFFRecord.MulRK:
          case TBIFFRecord.MulBlank:
            m_options |= StorageOptions.HasMultiRKBlank;
            break;

          case TBIFFRecord.Formula:
            iRow = GetRow( iOffset );
            iColumn = GetColumn( iOffset );
            break;

          case TBIFFRecord.SharedFormula2:
            arrSharedPos.Add( iOffset );

            if( arrShared == null )
              arrShared = new Dictionary<long, SharedFormulaRecord>();

            SharedFormulaRecord shared = ( SharedFormulaRecord )BiffRecordFactory.GetRecord( m_dataProvider,
              iOffset, Version );
            long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
            arrShared.Add( lCellIndex, shared );
            break;
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }

      int iSharedCount = arrSharedPos.Count;

      if( iSharedCount > 0 )
      {
        for( int i = iSharedCount - 1; i >= 0; i-- )
        {
          iOffset = arrSharedPos[ i ];
          RemoveRecord( iOffset );
        }
      }

      return bResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="useFastParsing">
    /// Indicates whether fast parsing is turned on. If it is not then we don't
    /// need to update FirstColumn and LastColumn indexes.
    /// </param>
    [ CLSCompliant( false ) ]
    public void UpdateRowInfo( RowRecord row, bool useFastParsing )
    {
      if( useFastParsing )
      {
        m_iFirstColumn = row.FirstColumn;
        m_iLastColumn = row.LastColumn;
      }

      m_optionFlags = ( RowRecord.OptionFlags )row.Options;
      m_usHeight = row.Height;
      m_usXFIndex = row.ExtendedFormatIndex;
      //m_usRowNumber = row.RowNumber;
    }
    /// <summary>
    /// Creates RowRecord and copies all required data into it.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Created record.</returns>
    [ CLSCompliant( false ) ]
    public RowRecord CreateRowRecord( WorkbookImpl book )
    {
      RowRecord result = ( RowRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Row );
      //result.RowNumber = m_usRowNumber;
      result.FirstColumn = ( ushort )Math.Max( 0, m_iFirstColumn );
      result.LastColumn = ( ushort )Math.Max( 0, m_iLastColumn );
      result.Height = m_usHeight;
      result.Options = ( int )m_optionFlags;
      result.ExtendedFormatIndex = ( m_usXFIndex > book.MaxXFCount ) ?
        ( ushort )book.DefaultXFIndex :
        m_usXFIndex;

      return result;
    }
    /// <summary>
    /// Copies row settings (default style, height and some other settings) from another row storage.
    /// </summary>
    /// <param name="sourceRow">Storage to copy settings from.</param>
    public void CopyRowRecordFrom( RowStorage sourceRow )
    {
      m_usHeight = sourceRow.m_usHeight;
      m_optionFlags = sourceRow.m_optionFlags;
      m_usXFIndex = sourceRow.ExtendedFormatIndex;
    }
    /// <summary>
    /// Sets default row options.
    /// </summary>
    public void SetDefaultRowOptions()
    {
      m_optionFlags = RowRecord.OptionFlags.ShowOutlineGroups;
    }
       /// <summary>
    /// Updates column indexes of the specified row.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iLastColumnIndex">Last column index.</param>
    public void UpdateColumnIndexes( int iColumnIndex, int iLastColumnIndex )
    {
      m_iFirstColumn = ( m_iUsedSize == 0 || m_iFirstColumn < 0 ) ?
      iColumnIndex :
        m_iFirstColumn = Math.Min( m_iFirstColumn, iColumnIndex );

      m_iLastColumn = Math.Max( iLastColumnIndex, m_iLastColumn );
    }
#if DEBUG 
#if !(WINRT )
    /// <summary>
    /// 
    /// </summary>
    public void Dump()
    {
      using( StreamWriter writer = new StreamWriter( "d:\\rowStorage.log", true ) )
      {
        writer.WriteLine( "-------------------Error at {0}--------------------", DateTime.Now );

        writer.WriteLine( "Row data" );
        writer.WriteLine( "UsedSize = {0}", m_iUsedSize );
        writer.WriteLine( "DataSize = {0}", DataSize );
        writer.WriteLine( "FirstColumn = {0}", m_iFirstColumn );
        writer.WriteLine( "LastColumn = {0}", m_iLastColumn );

        int iOffset = 0;
        while( iOffset < m_iUsedSize )
        {
          int iLength = m_dataProvider.ReadUInt16( iOffset + 2 );
          writer.WriteLine( "{0} - {1} ", ( TBIFFRecord )m_dataProvider.ReadUInt16( iOffset ),
            iLength );

          iOffset += 4 + iLength;
        }

        for( int i = 0; i < m_iUsedSize; i++ )
        {
          writer.Write( "{0:X} ", m_dataProvider.ReadByte( i ) );
        }
        writer.WriteLine();
      }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="ex"></param>
    public void Dump( TextWriter writer, Exception ex )
    {
      writer.WriteLine( "-------------------Error at {0}--------------------", DateTime.Now );

      if( ex != null )
      {
        writer.WriteLine( "Exception {0}", ex.Message );
        writer.WriteLine( "Stack trace {0}", ex.StackTrace );
      }

      writer.WriteLine( "Row data" );
      writer.WriteLine( "UsedSize = {0}", m_iUsedSize );
      writer.WriteLine( "DataSize = {0}", DataSize );
      writer.WriteLine( "FirstColumn = {0}", m_iFirstColumn );
      writer.WriteLine( "LastColumn = {0}", m_iLastColumn );

      int iOffset = 0;
      while( iOffset < m_iUsedSize )
      {
        int iLength = m_dataProvider.ReadUInt16( iOffset + 2 );
        writer.WriteLine( "{0} - {1} ", ( TBIFFRecord )m_dataProvider.ReadUInt16( iOffset ),
          iLength );

        iOffset += 4 + iLength;
      }

      for( int i = 0; i < m_iUsedSize; i++ )
      {
        writer.Write( "{0:X} ", m_dataProvider.ReadByte( i ) );
      }
      writer.WriteLine();
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newSize"></param>
    /// <param name="iBlockSize">Memory allocation block size.</param>
    public void SetCellPositionSize( int newSize, int iBlockSize, ExcelVersion version )
    {
      if( CellPositionSize != newSize )
      {
        switch( newSize )
        {
          case 4:
            ShrinkDataStorage();
            break;

          case 8:
            ExtendDataStorage( iBlockSize );
            break;

          default:
            throw new NotSupportedException();
        }

        //m_iCellPositionSize = newSize;
        m_version = version;
      }
    }
    /// <summary>
    /// Returns extended format index for the specified column or int.MinValue if not found.
    /// </summary>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>Extended format index for the specified column or int.MinValue if not found.</returns>
    public int GetXFIndexByColumn( int column )
    {
      bool bFound;
      bool bMulti;
      int iOffset = LocateRecord( column, out bFound, out bMulti, true );

      return ( bFound ) ? GetXFIndex( iOffset, bMulti ) : int.MinValue;
    }
    /// <summary>
    /// Looks through all records and calls AddIncrease for each LabelSST record.
    /// </summary>
    /// <param name="sst">Dictionary to update.</param>
    public void ReAddAllStrings( SSTDictionary sst )
    {
      int iOffset = 0;
      int iLabelSSTCode = ( int )TBIFFRecord.LabelSST;

      while( iOffset < m_iUsedSize )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( iCode == iLabelSSTCode )
        {
          int iSSTIndex = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );
          sst.AddIncrease( iSSTIndex );
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="version"></param>
    /// <param name="iBlockSize"></param>
    public void SetVersion( ExcelVersion version, int iBlockSize )
    {
      if( Version != version )
      {
        switch( version )
        {
          case ExcelVersion.Excel97to2003:
            ShrinkDataStorage();
            break;

          case ExcelVersion.Excel2007:
          case ExcelVersion.Excel2010:
          case ExcelVersion.Excel2013:
            ExtendDataStorage( iBlockSize );
            break;

          default:
            throw new ArgumentOutOfRangeException( "version" );
        }

        m_version = version;
      }
    }
    /// <summary>
    /// Updates columns indexes.
    /// </summary>
    private void UpdateColumns()
    {
      m_iFirstColumn = -1;
      m_iLastColumn = -1;

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        short sCode = m_dataProvider.ReadInt16( iOffset );
        TBIFFRecord code = ( TBIFFRecord )sCode;//m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        int iColumnIndex = GetColumn( iOffset );

        if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          AccessColumn( iColumnIndex );

          // Trying to optimize.
          //if( UtilityMethods.IndexOf( DEF_MULTIRECORDS, sCode ) != -1 )
          if( code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank )
          {
            int iLastIndex = m_dataProvider.ReadInt16( iOffset + iLength
              + BiffRecordRaw.DEF_HEADER_SIZE -2 );

            AccessColumn( iLastIndex );
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Calls each cell.
    /// </summary>
    /// <param name="recordType">Record type to check.</param>
    /// <param name="offset">Offset to the record to check.</param>
    /// <param name="data">Data to be passed.</param>
    public delegate void CellMethod( TBIFFRecord recordType, int offset, object data );
    /// <summary>
    /// Iterates through all cells in the row.
    /// </summary>
    /// <param name="method">Method to call for each cell.</param>
    /// <param name="data">Data to pass to the calling method.</param>
    public void IterateCells( CellMethod method, object data )
    {
      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord recordType = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        method( recordType, iOffset, data );
        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="recordType">Record type to check.</param>
    /// <param name="offset">Offset to the record to check.</param>
    /// <param name="data">Array to mark used references in.</param>
    public void MarkCellUsedReferences( TBIFFRecord recordType, int offset, object data )
    {
      if( recordType == TBIFFRecord.Formula || recordType == TBIFFRecord.Array )
      {
        IFormulaRecord formula = BiffRecordFactory.GetRecord( m_dataProvider, offset, Version ) as IFormulaRecord;
        FormulaUtil.MarkUsedReferences( formula.Formula, ( bool[] )data );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="recordType">Represents Biff record type.</param>
    /// <param name="offset">Represents offset value.</param>
    /// <param name="data">Data to be updated.</param>
    public void UpdateReferenceIndexes( TBIFFRecord recordType, int offset, object data )
    {
      if( recordType == TBIFFRecord.Formula || recordType == TBIFFRecord.Array )
      {
        IFormulaRecord formula = BiffRecordFactory.GetRecord( m_dataProvider, offset, Version ) as IFormulaRecord;
        Ptg[] tokens = formula.Formula;
        if( FormulaUtil.UpdateReferenceIndexes( tokens, ( int[] )data ) )
        {
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="heapHandle"></param>
    internal void CreateDataProvider( IntPtr heapHandle )
    {
        if (m_dataProvider == null)
        {
            m_dataProvider = ApplicationImpl.CreateDataProvider(heapHandle);
            if (m_book != null && m_book.MaxImportColumns > 1)
                m_dataProvider.EnsureCapacity(18 * m_book.MaxImportColumns);
            else
                m_dataProvider.EnsureCapacity(18);
        }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Represents used items.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      IterateCells( MarkCellUsedReferences, usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      IterateCells( UpdateReferenceIndexes, arrUpdatedIndexes );
    }

    /// <summary>
    /// Searches for the record of specified type.
    /// </summary>
    /// <param name="recordType">Record type to look for.</param>
    /// <param name="startColumn">Zero-based index of the column to start looking at.</param>
    /// <param name="endColumn">Zero-based index of the column to end looking at.</param>
    /// <returns>Column index that contains record of the specified type or value beyond endColumn if not found.</returns>
    public int FindRecord( TBIFFRecord recordType, int startColumn, int endColumn )
    {
      if( startColumn > m_iLastColumn || endColumn > m_iLastColumn )
        return endColumn + 1;

      bool bFound;
      int iOffset = LocateRecord( startColumn, out bFound );

      if( !bFound && iOffset >= m_iUsedSize )
        return endColumn + 1;

      int iCurrentColumn = ( bFound ) ?
        startColumn :
        GetColumn( iOffset );

      if( iOffset >= m_iUsedSize )
        iCurrentColumn = endColumn + 1;

      TBIFFRecord record = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      while( iCurrentColumn <= endColumn )
      {
        if( record == recordType )
          break;

        iOffset = MoveNext( iOffset );

        if( iOffset >= m_iUsedSize )
        {
          iCurrentColumn = endColumn + 1;
          break;
        }

        record = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( record != TBIFFRecord.String && record != TBIFFRecord.Array )
          iCurrentColumn = GetColumn( iOffset );
      }

      if( iCurrentColumn <= endColumn && iOffset < m_iUsedSize )
      {
        m_iCurrentColumn = iCurrentColumn;
        m_iCurrentOffset = iOffset;
      }

      return iCurrentColumn;
    }
    /// <summary>
    /// Searches for the first cell in the specified range.
    /// </summary>
    /// <param name="startColumn">Zero-based column index to start search at.</param>
    /// <param name="endColumn">Zero-based column index to end search at.</param>
    /// <returns></returns>
    public int FindFirstCell( int startColumn, int endColumn )
    {
      bool bFound;
      int iOffset = LocateRecord( startColumn, out bFound );

      int iFoundColumn = ( iOffset < UsedSize ) ?
        GetColumn( iOffset ) :
        endColumn + 1;

      return iFoundColumn;
    }
    /// <summary>
    /// Gets indexes of the used named range objects.
    /// </summary>
    /// <param name="result">Collection to put named range indexes into.</param>
    internal void GetUsedNames( Dictionary<int, object> result )
    {
      if( result == null )
        throw new ArgumentNullException( "result" );

      int iOffset = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord cellType = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( cellType == TBIFFRecord.Formula || cellType == TBIFFRecord.Array )
        {
          IFormulaRecord formula = ( IFormulaRecord )BiffRecordFactory.GetRecord(
            m_dataProvider, iOffset, Version );
          Ptg[] tokens = formula.Formula;
          AddNamedRangeTokens( result, tokens );
        }

        iOffset = MoveNext( iOffset );
      }
    }
    /// <summary>
    /// Adds all named range indexes referenced by formula tokens into resulting dictionary.
    /// </summary>
    /// <param name="result">Dictionary that will get named range tokens.</param>
    /// <param name="tokens">Tokens to check.</param>
    private void AddNamedRangeTokens( Dictionary<int, object> result, Ptg[] tokens )
    {
      if( result == null )
        throw new ArgumentNullException( "result" );

      // No tokens to update?
      if( tokens == null )
        return;

      for( int i = tokens.Length - 1; i >= 0; i-- )
      {
        Ptg token = tokens[ i ];
        NamePtg name = token as NamePtg;

        if( name != null )
        {
          result[ name.ExternNameIndex - 1 ] = null;
        }

        // TODO: should we add namex objects?
      }
    }
    #endregion

    #region Class private methods
    /// <summary>
    /// Inserts records data into data storage.
    /// </summary>
    /// <param name="iOffset">Position to insert at.</param>
    /// <param name="records">Array of records to insert.</param>
    private void InsertRecordData( int iOffset, BiffRecordRaw[] records )
    {
      for( int i = 0, iCount = records.Length; i < iCount; i++ )
      {
        BiffRecordRaw record = records[ i ];
        m_dataProvider.WriteInt16( iOffset, ( short )record.RecordCode );
        iOffset += 2;

        int iLength = record.GetStoreSize( Version );
        m_dataProvider.WriteInt16( iOffset, ( short )iLength );
        iOffset += 2;

        record.InfillInternalData( m_dataProvider, iOffset, Version );
        iOffset += iLength;
      }
    }
    /// <summary>
    /// Sets cell position size to 4 and converts internal data storage.
    /// </summary>
    private void ShrinkDataStorage()
    {
      if( CellPositionSize == 4 | m_iUsedSize == 0 )
        return;

      int iReadOffset = 0;
      int iWriteOffset = 0;
      int iMaxRow = ushort.MaxValue;
      int iMaxColumn = byte.MaxValue;

      if( m_iFirstColumn > iMaxColumn )
      {
        m_iFirstColumn = 0;
        m_iLastColumn = 0;
        m_iUsedSize = iWriteOffset;
        //m_iCellPositionSize = 4;

        return;
      }
      
      while( iReadOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iReadOffset );
        iReadOffset += 2;

        int iLength = m_dataProvider.ReadInt16( iReadOffset );
        iReadOffset += 2;

        if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          int iRow = m_dataProvider.ReadInt32( iReadOffset );
          int iColumn = m_dataProvider.ReadInt32( iReadOffset + 4 );

          if( iColumn > iMaxColumn || iRow > iMaxRow )
            break;

          m_iLastColumn = iColumn;

          m_dataProvider.WriteInt16( iWriteOffset, ( short )code );
          iWriteOffset += 2;

          if( code != TBIFFRecord.Formula )
          {
              bool bMulti = (code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank);
              int iNewLength = iLength - 4;

              if (bMulti)
                  iNewLength -= 2;

              m_dataProvider.WriteInt16(iWriteOffset, (short)iNewLength);
              iWriteOffset += 2;

            m_dataProvider.WriteUInt16( iWriteOffset, ( ushort )iRow );
            iWriteOffset += 2;

            m_dataProvider.WriteInt16( iWriteOffset, ( short )iColumn );
            iWriteOffset += 2;

            m_dataProvider.CopyTo( iReadOffset + 8, m_dataProvider, iWriteOffset, iNewLength );
            iWriteOffset += iNewLength - 4;
          }
          else
          {
            // Here we have to convert tokens from one version into another.
            // The easiest way to do this is to use FormulaRecord for this purpose.
            FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord(
              m_dataProvider, iReadOffset - BiffRecordRaw.DEF_HEADER_SIZE, ExcelVersion.Excel2007 );

            int iNewLength = formula.GetStoreSize( ExcelVersion.Excel97to2003 );
            m_dataProvider.WriteInt16( iWriteOffset, ( short )iNewLength );
            iWriteOffset += 2;
            FormulaRecord.ConvertFormulaTokens( formula.ParsedExpression, true );
            formula.InfillInternalData( m_dataProvider, iWriteOffset, ExcelVersion.Excel97to2003 );
            iWriteOffset += iNewLength;
          }
        }
        else if( code == TBIFFRecord.Array )
        {
          BiffRecordRaw record = BiffRecordFactory.GetRecord( m_dataProvider,
            iReadOffset - BiffRecordRaw.DEF_HEADER_SIZE, ExcelVersion.Excel2007 );

          int iNewLength = record.GetStoreSize( ExcelVersion.Excel97to2003 );

          m_dataProvider.WriteInt16( iWriteOffset, ( short )code );
          iWriteOffset += 2;

          m_dataProvider.WriteInt16( iWriteOffset, ( short )iNewLength );
          iWriteOffset += 2;

          record.InfillInternalData( m_dataProvider, iWriteOffset, ExcelVersion.Excel97to2003 );
          iWriteOffset += iNewLength;
        }
        else
        {
          m_dataProvider.CopyTo( iReadOffset - BiffRecordRaw.DEF_HEADER_SIZE, m_dataProvider,
            iWriteOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE );

          iWriteOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
        }

        iReadOffset += iLength;
      }

      m_iUsedSize = iWriteOffset;
      //m_iCellPositionSize = 4;
    }
    /// <summary>
    /// Sets cell position size to 4 and converts internal data storage.
    /// </summary>
    private void ExtendDataStorage( int iBlockSize )
    {
      if( CellPositionSize == 8 || m_iUsedSize == 0 )
        return;

      //Decompress( false, 256 );
      int iReadOffset = 0;
      int iWriteOffset = 0;
      // TODO: change storage type
      DataProvider result = ApplicationImpl.CreateDataProvider( GetHeapHandle() );

      // TODO: change size - must be multiple of block size.
      List<FormulaRecord> arrFormulas = new List<FormulaRecord>();
      int iDataSize = GetEnlargedDataSize( arrFormulas );

      result.EnsureCapacity( ( iDataSize / iBlockSize + 1 ) * iBlockSize );
      int iFormulaIndex = 0;

      while( iReadOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iReadOffset );
        iReadOffset += 2;

        int iLength = m_dataProvider.ReadInt16( iReadOffset );
        iReadOffset += 2;

        if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
        {
          int iRow = m_dataProvider.ReadUInt16( iReadOffset );
          int iColumn = m_dataProvider.ReadUInt16( iReadOffset + 2 );

          result.WriteInt16( iWriteOffset, ( short )code );
          iWriteOffset += 2;

          if( code != TBIFFRecord.Formula )
          {
            EnlargeCellRecord( result, ref iWriteOffset, iReadOffset, code, iLength, iRow, iColumn );
          }
          else
          {
            // Here we have to convert tokens from one version into another.
            // The easiest way to do this is to use FormulaRecord for this purpose.
            FormulaRecord formula = arrFormulas[ iFormulaIndex ];
            FormulaRecord.ConvertFormulaTokens( formula.ParsedExpression, false );
            iFormulaIndex++;
            EnlargeFormulaRecord( result, ref iWriteOffset, formula );
          }
        }
        else if( code == TBIFFRecord.Array )
        {
          BiffRecordRaw record = BiffRecordFactory.GetRecord( m_dataProvider,
            iReadOffset - BiffRecordRaw.DEF_HEADER_SIZE, ExcelVersion.Excel97to2003 );

          int iNewLength = record.GetStoreSize( ExcelVersion.Excel2007 );

          result.WriteInt16( iWriteOffset, ( short )code );
          iWriteOffset += 2;

          result.WriteInt16( iWriteOffset, ( short )iNewLength );
          iWriteOffset += 2;

          record.InfillInternalData( result, iWriteOffset, ExcelVersion.Excel2007 );
          iWriteOffset += iNewLength;
        }
        else
        {
          int iFullLength = iLength + BiffRecordRaw.DEF_HEADER_SIZE;
          m_dataProvider.CopyTo( iReadOffset - BiffRecordRaw.DEF_HEADER_SIZE, result,
            iWriteOffset, iFullLength );

          iWriteOffset += iFullLength;
        }

        iReadOffset += iLength;
      }

      if( iDataSize != iWriteOffset )
        throw new InvalidOperationException( "Wrong offset" );

      m_iUsedSize = iWriteOffset;
      m_iCurrentColumn = -1;
      m_iCurrentOffset = -1;
      m_dataProvider.Dispose();
      //m_dataProvider = null;
      m_dataProvider = result;
      //m_iCellPositionSize = 8;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="iWriteOffset"></param>
    /// <param name="iReadOffset"></param>
    /// <param name="iLength"></param>
    /// <param name="code"></param>
    /// <param name="iRow"></param>
    /// <param name="iColumn"></param>
    private void EnlargeCellRecord( DataProvider result, ref int iWriteOffset,
      int iReadOffset, TBIFFRecord code, int iLength, int iRow, int iColumn  )
    {
      bool bMulti = ( code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank );
      int iNewLength = iLength + 4;

      if( bMulti )
        iNewLength += 2;

      result.EnsureCapacity( iWriteOffset + iNewLength + 2 );

      result.WriteInt16( iWriteOffset, ( short )iNewLength );
      iWriteOffset += 2;

      result.WriteInt32( iWriteOffset, iRow );
      iWriteOffset += 4;

      result.WriteInt32( iWriteOffset, iColumn );
      iWriteOffset += 4;

      m_dataProvider.CopyTo( iReadOffset + 4, result, iWriteOffset, iLength - 4 );
      iWriteOffset += iLength - 4;

      if( bMulti )
      {
        int iLastColumn = m_dataProvider.ReadInt16( iReadOffset + iLength - 2 );
        result.WriteInt32( iWriteOffset - 2, iLastColumn );
        iWriteOffset += 2;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="iWriteOffset"></param>
    /// <param name="formula"></param>
    private void EnlargeFormulaRecord( DataProvider result, ref int iWriteOffset, FormulaRecord formula )
    {
      int iNewLength = formula.GetStoreSize( ExcelVersion.Excel2007 );

      result.WriteInt16( iWriteOffset, ( short )iNewLength );
      iWriteOffset += 2;

      result.EnsureCapacity( iWriteOffset + iNewLength );
      formula.InfillInternalData( result, iWriteOffset, ExcelVersion.Excel2007 );
      iWriteOffset += iNewLength;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrFormulas"></param>
    /// <returns></returns>
    private int GetEnlargedDataSize( List<FormulaRecord> arrFormulas )
    {
      if( arrFormulas == null )
        throw new ArgumentNullException( "arrFormulas" );

      int iOffset = 0;
      int iRequiredSize = 0;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        switch( code )
        {
          case TBIFFRecord.String:
            iRequiredSize += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
            break;

          case TBIFFRecord.Array:
            ArrayRecord array = ( ArrayRecord )BiffRecordFactory.GetRecord(
              m_dataProvider, iOffset, Version );
            iRequiredSize += array.GetStoreSize( ExcelVersion.Excel2007 )
              + BiffRecordRaw.DEF_HEADER_SIZE;
            break;

          case TBIFFRecord.Formula:
            FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord(
              m_dataProvider, iOffset, Version );
            arrFormulas.Add( formula );
            iRequiredSize += formula.GetStoreSize( ExcelVersion.Excel2007 )
              + BiffRecordRaw.DEF_HEADER_SIZE;
            break;

          case TBIFFRecord.MulBlank:
          case TBIFFRecord.MulRK:
            iRequiredSize += iLength + BiffRecordRaw.DEF_HEADER_SIZE + 6;
            break;

          default:
            iRequiredSize += iLength + BiffRecordRaw.DEF_HEADER_SIZE + 4;
            break;
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }

      return iRequiredSize;
    }
    /// <summary>
    /// Evaluates number of records in this storage.
    /// </summary>
    /// <returns>Number of records.</returns>
    private int GetRecordCount()
    {
      int iOffset = 0;
      int iResult = 0;

      while( iOffset < m_iUsedSize )
      {
        int iRecordLen = m_dataProvider.ReadInt16( iOffset + 2 );
        iOffset += iRecordLen + BiffRecordRaw.DEF_HEADER_SIZE;
        iResult++;
      }

      return iResult;
    }
    /// <summary>
    /// Updates all required indexes after copying row into another worksheet.
    /// </summary>
    /// <param name="sourceSST">Source SST dictionary.</param>
    /// <param name="destSST">Destination SST dictionary.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicNameIndexes">Dictionary with new name indexes.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    private void UpdateRecordsAfterCopy( SSTDictionary sourceSST, SSTDictionary destSST,
      Dictionary<int, int> hashExtFormatIndexes, Dictionary<string, string> hashWorksheetNames,
      Dictionary<int, int> dicNameIndexes, Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dictExternSheets )
    {
      if( sourceSST == null )
        throw new ArgumentNullException( "sourceSST" );

      if( destSST == null )
        throw new ArgumentNullException( "destSST" );

      int iOffset = 0;
      int iOldXFIndex = ExtendedFormatIndex;
      bool bIsLocalCopy = sourceSST == destSST;
      WorkbookImpl destBook = destSST.Workbook;

      if( hashExtFormatIndexes.ContainsKey( iOldXFIndex ) )
      {
        ExtendedFormatIndex = ( IsFormatted )
          ? ( ushort )hashExtFormatIndexes[ iOldXFIndex ]
          : ( ushort )destBook.DefaultXFIndex;
      }

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

        if( code != TBIFFRecord.String && code != TBIFFRecord.Array )
        {
          if( !bIsLocalCopy )
          {
            if( hashExtFormatIndexes != null && hashExtFormatIndexes.Count > 0 )
            {
              if( code == TBIFFRecord.MulRK )
              {
                UpdateMulReference( hashExtFormatIndexes, iOffset, iLength, true );
              }
              else if( code == TBIFFRecord.MulBlank )
              {
                UpdateMulReference( hashExtFormatIndexes, iOffset, iLength, false );
              }
              else
              {
                int iXFIndex = GetXFIndex( iOffset, false );

                if( hashExtFormatIndexes.ContainsKey( iXFIndex ) )
                {
                  iXFIndex = hashExtFormatIndexes[ iXFIndex ];
                  SetXFIndex( iOffset, ( ushort )iXFIndex );
                }
              }
            }

            switch( code )
            {
              case TBIFFRecord.LabelSST:
                UpdateLabelSST( sourceSST, destSST, iOffset, false, dicFontIndexes );
                break;

              case TBIFFRecord.Formula:
                UpdateFormulaRefs( sourceSST, destSST, iOffset, hashWorksheetNames, dicNameIndexes,
                  iLength, dictExternSheets );
                break;
            }
          }
          else
          {
            if( code == TBIFFRecord.Formula )
            {
              UpdateFormulaRefs( sourceSST, destSST, iOffset, hashWorksheetNames, dicNameIndexes,
                iLength, dictExternSheets );
            }
            else if( code == TBIFFRecord.LabelSST )
            {
              UpdateLabelSST( sourceSST, destSST, iOffset, true, dicFontIndexes );
            }
          }
        }

        iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      }
    }
    /// <summary>
    /// Updates MulRK or MulBlank XF Indexes.
    /// </summary>
    /// <param name="hashXFIndexes">HashTable with new XF Indexes.</param>
    /// <param name="iOffset">Represents offset on start mull record.</param>
    /// <param name="iLength">Record length without header.</param>
    /// <param name="bIsMulRK">Indicates whether offset is offset to MulRK record (TRUE) or to MulBlank record (false).</param>
    private void UpdateMulReference( Dictionary<int, int> hashXFIndexes, int iOffset, int iLength, bool bIsMulRK )
    {
      iLength = iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iLength - 2;
      iOffset += BiffRecordRaw.DEF_HEADER_SIZE + 4;

      int iPeriod = ( bIsMulRK )
        ? DEF_MULRK_PERIOD
        : DEF_MULBLANK_PERIOD;

      for( int i = iOffset; i < iLength; i += iPeriod )
      {
        int iXFIndex = m_dataProvider.ReadInt16( i );

        if( hashXFIndexes.TryGetValue( iXFIndex, out iXFIndex ) )
        {
          m_dataProvider.WriteInt16( i, ( short )iXFIndex );
        }
      }
    }
    /// <summary>
    /// Updates sst records.
    /// </summary>
    /// <param name="sourceSST">Represents source sst.</param>
    /// <param name="destSST">Represents destination sst.</param>
    /// <param name="iOffset">Represents offset to add.</param>
    /// <param name="bIsLocal"></param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    private void UpdateLabelSST( SSTDictionary sourceSST, SSTDictionary destSST, int iOffset,
      bool bIsLocal, Dictionary<int, int> dicFontIndexes )
    {
      if( sourceSST == null )
        throw new ArgumentNullException( "sourceSST" );

      if( destSST == null )
        throw new ArgumentNullException( "destSST" );

      int iSSTIndex = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );

      if( bIsLocal )
      {
        destSST.AddIncrease( iSSTIndex );
      }
      else
      {
        TextWithFormat text = sourceSST[ iSSTIndex ];

        object forAdd = ( text.FormattingRunsCount == 0 )
          ? ( object )text.Text
          : ( object )text.Clone( dicFontIndexes );

        iSSTIndex = destSST.AddIncrease( forAdd );
        LabelSSTRecord.SetSSTIndex( m_dataProvider, iOffset, iSSTIndex, Version );
      }
    }
    /// <summary>
    /// Updates formula indexes.
    /// </summary>
    /// <param name="sourceSST">Represents source SST.</param>
    /// <param name="destSST">Represents destination sst.</param>
    /// <param name="iOffset">Represents offset.</param>
    /// <param name="hashWorksheetNames">Represents hash table with worksheet names.</param>
    /// <param name="dicNameIndexes">Represents dic name indexes.</param>
    /// <param name="iLength">Represents length</param>
    private void UpdateFormulaRefs( SSTDictionary sourceSST, SSTDictionary destSST, int iOffset
      , Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicNameIndexes,
      int iLength, Dictionary<int, int> dictExternSheets )
    {
      FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord(
        m_dataProvider, iOffset, Version );

      formula.ParseStructure( m_dataProvider, iOffset + 4, iLength, Version );
      WorkbookImpl destBook = destSST.Workbook;
      bool bUpdated = UpdateNameSheetReferences( formula, hashWorksheetNames, sourceSST.Workbook
        , destBook, dicNameIndexes, dictExternSheets );

      if( bUpdated )
      {
        formula.IsFillFromExpression = true;
        int iNewLen = formula.GetStoreSize( Version );
        InsertRecordData( iOffset, iLength + BiffRecordRaw.DEF_HEADER_SIZE,
          iNewLen + BiffRecordRaw.DEF_HEADER_SIZE, formula, destBook.Application.RowStorageAllocationBlockSize );
      }
    }
    /// <summary>
    /// Updates sheet references in the formula.
    /// </summary>
    /// <param name="formula">Formula to update.</param>
    /// <param name="dicSheetNames">Dictionary with new worksheet names.</param>
    /// <param name="sourceBook">Source workbook.</param>
    /// <param name="destBook">Destination workbook.</param>
    /// <returns>True if any changes were made.</returns>
    private bool UpdateSheetReferences( FormulaRecord formula, IDictionary dicSheetNames,
      WorkbookImpl sourceBook, WorkbookImpl destBook )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      if( sourceBook == null )
        throw new ArgumentNullException( "book" );

      Ptg[] arrPtg = formula.ParsedExpression;
      int iOldSize = formula.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      bool bChanged = false;

      for( int i = 0, len = arrPtg.Length; i < len; i++ )
      {
        Ptg token = arrPtg[ i ];

        if( token is ISheetReference )
        {
          ISheetReference reference = ( ISheetReference )token;
          ushort usOldRefIndex = reference.RefIndex;
          string strSheetName = sourceBook.GetSheetNameByReference( usOldRefIndex );

          if( dicSheetNames != null && dicSheetNames.Contains( strSheetName ) )
          {
            strSheetName = ( string )dicSheetNames[ strSheetName ];
          }

          int iNewReference = destBook.AddSheetReference( strSheetName );
          reference.RefIndex = ( ushort )iNewReference;
          bChanged = true;
        }
      }

      return bChanged;
    }
    /// <summary>
    /// Updates named range references in the formula.
    /// </summary>
    /// <param name="formula">Formula to update.</param>
    /// <param name="dicNameIndexes">Dictionary with new name indexes.</param>
    /// <returns>True if any changes were made.</returns>
    private bool UpdateNameReferences( FormulaRecord formula, Dictionary<int, int> dicNameIndexes )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      if( dicNameIndexes == null )
        throw new ArgumentNullException( "dicNameIndexes" );

      Ptg[] arrPtg = formula.ParsedExpression;
      int iOldSize = formula.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      bool bChanged = false;

      for( int i = 0, len = arrPtg.Length; i < len; i++ )
      {
        NamePtg name = arrPtg[ i ] as NamePtg;

        if( name != null )
        {
          int iOldIndex = name.ExternNameIndex - 1;
          int iNewIndex = dicNameIndexes.ContainsKey( iOldIndex )
            ? dicNameIndexes[ iOldIndex ]
            : iOldIndex;

          name.ExternNameIndex = ( ushort )( iNewIndex + 1 );
          bChanged = true;
        }
      }

      return bChanged;
    }
    /// <summary>
    /// Updates sheet references in the formula.
    /// </summary>
    /// <param name="formula">Formula to update.</param>
    /// <param name="dicSheetNames">Dictionary with new worksheet names.</param>
    /// <param name="sourceBook">Source workbook.</param>
    /// <param name="destBook">Destination workbook.</param>
    /// <param name="dicNameIndexes">Represents dictionary with name indexes.</param>
    /// <returns>True if any changes were made.</returns>
    private bool UpdateNameSheetReferences( FormulaRecord formula, Dictionary<string, string> dicSheetNames,
      WorkbookImpl sourceBook, WorkbookImpl destBook, Dictionary<int, int> dicNameIndexes,
      Dictionary<int, int> dictExternSheets )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      if( sourceBook == null )
        throw new ArgumentNullException( "book" );

      if( dicNameIndexes == null )
        throw new ArgumentNullException( "dicNameIndexes" );

      Ptg[] arrPtg = formula.ParsedExpression;
      int iOldSize = formula.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      bool bChanged = false;

      for( int i = 0, len = arrPtg.Length; i < len; i++ )
      {
        Ptg token = arrPtg[ i ];

        if( token is ISheetReference )
        {
          ISheetReference reference = ( ISheetReference )token;
          ushort usOldRefIndex = reference.RefIndex;

          if( !sourceBook.IsExternalReference( usOldRefIndex ) )
          {
            string strSheetName = sourceBook.GetSheetNameByReference( usOldRefIndex );

            if( dicSheetNames != null && dicSheetNames.ContainsKey( strSheetName ) )
            {
              strSheetName = dicSheetNames[ strSheetName ];
            }

            int iNewReference = destBook.AddSheetReference( strSheetName );
            reference.RefIndex = ( ushort )iNewReference;
            bChanged = true;
          }
          else
          {
            NameXPtg nameX = token as NameXPtg;

            if( nameX != null )
            {
              int iOldRefIndex = nameX.RefIndex;
              int iNewRefIndex = dictExternSheets.ContainsKey( iOldRefIndex ) ?
                dictExternSheets[ iOldRefIndex ] :
                iOldRefIndex;

              nameX.RefIndex = ( ushort )iNewRefIndex;
              bChanged = true;
            }
          }
        }
        else 
        {
          NamePtg name = token as NamePtg;

          if( name != null )
          {
            int iOldIndex = name.ExternNameIndex - 1;
            int iNewIndex = dicNameIndexes.ContainsKey( iOldIndex )
              ? dicNameIndexes[ iOldIndex ]
              : iOldIndex;

            name.ExternNameIndex = ( ushort )( iNewIndex + 1 );
            bChanged = true;
          }
        }
      }

      return bChanged;
    }
    /// <summary>
    /// Indicates whether multi- record contains records of the same type as specified one.
    /// </summary>
    /// <param name="cell">Cell type to check.</param>
    /// <param name="iOffset">Offset to the multi- record.</param>
    /// <returns>True if record types are compatible.</returns>
    private bool IsSameSubType( ICellPositionFormat cell, int iOffset )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );

      TBIFFRecord typeCode = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
      for( int i = 0, len = DEF_MULTIRECORDS_SUBTYPES.Length; i < len; i += 2 )
      {
        if( DEF_MULTIRECORDS_SUBTYPES[ i ] == typeCode )
        {
          return ( DEF_MULTIRECORDS_SUBTYPES[ i + 1 ] == cell.TypeCode );
        }
      }

      return false;
    }
    /// <summary>
    /// Splits record into two parts by removing subrecord with specified column index.
    /// </summary>
    /// <param name="iOffset">Offset to the record to split.</param>
    /// <param name="iColumnIndex">Column index to split by.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    /// <returns>Offset to the second part.</returns>
    private int SplitRecord( int iOffset, int iColumnIndex, int iBlockSize )
    {
      BiffRecordRaw record = BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );
      IMultiCellRecord multiCell = ( IMultiCellRecord )record;
      
      int iOldSize = record.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      ICellPositionFormat[] arrNewCells = multiCell.Split( iColumnIndex );
      int iNewCellCount = arrNewCells.Length;

      int iTotalSize = 0;

      for( int i = 0; i < iNewCellCount; i++ )
      {
        BiffRecordRaw newRecord = ( BiffRecordRaw )arrNewCells[ i ];

        if( record != null )
          iTotalSize += record.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      }

      // Number of additional memory (this value can be negative).
      int iDelta = iTotalSize - iOldSize;
      EnsureSize( m_iUsedSize + iDelta, iBlockSize );
      InsertRecordData( iOffset, iOldSize, 0, null, iBlockSize );

      for( int i = 0; i < iNewCellCount; i++ )
      {
        BiffRecordRaw newRecord = ( BiffRecordRaw )arrNewCells[ i ];

        if( newRecord != null )
        {
          int iStoreSize = newRecord.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
          InsertRecordData( iOffset, 0, iStoreSize, newRecord, iBlockSize );

          if( i == 0 )
            iOffset += iStoreSize;
        }
      }

      return iOffset;
    }
    /// <summary>
    /// Inserts record inside record that contains multiple values.
    /// </summary>
    /// <param name="iOffset">Offset to the record to insert into.</param>
    /// <param name="cell">Record to insert.</param>
    private void InsertIntoRecord( int iOffset, ICellPositionFormat cell )
    {
      IMultiCellRecord cellToInsertInto = ( IMultiCellRecord )
        BiffRecordFactory.GetRecord( m_dataProvider, iOffset, Version );

      cellToInsertInto.Insert( cell );
    }
    /// <summary>
    /// Inserts record data into storage.
    /// </summary>
    /// <param name="iOffset">Offset to the record.</param>
    /// <param name="iPreparedSize">Size that could be overwritten by new record.</param>
    /// <param name="iRequiredSize">Size required by new record.</param>
    /// <param name="record">Record to write.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    private void InsertRecordData( int iOffset, int iPreparedSize, int iRequiredSize,
      BiffRecordRaw record, int iBlockSize )
    {
      int iMemDelta = iRequiredSize - iPreparedSize;

      int iProposedSize = iRequiredSize / iBlockSize * iBlockSize + iBlockSize;
      
      if ((m_dataProvider.Capacity - UsedSize) < iRequiredSize)
      {
        EnsureSize( iProposedSize + m_iUsedSize, iBlockSize );
      }

      if( iPreparedSize != iRequiredSize )
      {
        int iSize = m_iUsedSize - iOffset - iPreparedSize;

        if( iSize > 0 )
        {
          //API.RtlMoveMemory( ptrDest, ptrSource, iSize );
          m_dataProvider.MoveMemory( iOffset + iRequiredSize, iOffset + iPreparedSize, iSize );
        }

        m_iUsedSize += iRequiredSize - iPreparedSize;
      }

      if( record != null )
      {
        m_dataProvider.WriteInt16( iOffset, ( short )record.TypeCode );
        ExcelVersion version = Version;
        m_dataProvider.WriteInt16( iOffset + 2, ( short )record.GetStoreSize( version ) );
        record.InfillInternalData( m_dataProvider, iOffset + 4, version );
      }
    }

    /// <summary>
    /// Inserts record data into storage.
    /// </summary>
    /// <param name="iOffset">Offset to the record.</param>
    /// <param name="iPreparedSize">Size that could be overwritten by new record.</param>
    /// <param name="arrRecords">Records to write.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    private void InsertRecordData( int iOffset, int iPreparedSize, IList arrRecords, int iBlockSize )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      int iRequiredSize = 0;
      int iCount = arrRecords.Count;
      int iMaxRecordSize = 0;

      for( int i = 0; i < iCount; i++ )
      {
        BiffRecordRaw record = ( BiffRecordRaw )arrRecords[ i ];
        int iStoreSize = record.GetStoreSize( Version );
        iRequiredSize += iStoreSize + BiffRecordRaw.DEF_HEADER_SIZE;
        iMaxRecordSize = Math.Max( iStoreSize, iMaxRecordSize );
      }

      int iMemDelta = iRequiredSize - iPreparedSize;

      if( iMemDelta > 0 )
      {
        EnsureSize( iMemDelta + m_iUsedSize, iBlockSize );
      }

      if( iPreparedSize != iRequiredSize )
      {
        //        long lDataStart = m_ptrRowData.ToInt64();
        //        IntPtr ptrSource = ( IntPtr )( lDataStart + iOffset + iPreparedSize );
        //        IntPtr ptrDest = ( IntPtr )( lDataStart + iOffset + iRequiredSize );

        int iSize = m_iUsedSize - iOffset - iPreparedSize;

        if( iSize > 0 )
        {
          m_dataProvider.MoveMemory( iOffset + iRequiredSize, iOffset + iPreparedSize, iSize );
          //API.RtlMoveMemory( ptrDest, ptrSource, iSize );
        }

        m_iUsedSize += iRequiredSize - iPreparedSize;
      }

      if( iCount > 0 )
      {
        for( int i = 0; i < iCount; i++ )
        {
          BiffRecordRaw record = ( BiffRecordRaw )arrRecords[ i ];
          m_dataProvider.WriteInt16( iOffset, ( short )record.TypeCode );
          m_dataProvider.WriteInt16( iOffset + 2, ( short )record.GetStoreSize( Version ) );
          record.InfillInternalData( m_dataProvider, iOffset, Version );
          iOffset += record.Length + BiffRecordRaw.DEF_HEADER_SIZE;
        }
      }
    }

    /// <summary>
    /// Locates record in the row.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index of the record to find.</param>
    /// <param name="bFound">Indicates whether record was found.</param>
    /// <returns>Offset where record should be placed.</returns>
    private int LocateRecord( int iColumnIndex, out bool bFound )
    {
      bool bMulty;

      return LocateRecord( iColumnIndex, out bFound, out bMulty, false );
    }
    /// <summary>
    /// Locates record in the row.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index of the record to find.</param>
    /// <param name="bFound">Indicates whether record was found.</param>
    /// <param name="bMul">Indicates is found multi record or not.</param>
    /// <param name="bGetRkOffset">If true returns offset on valid mull rk structure, by current column index.
    /// Otherwise on MulRk record.</param>
    /// <returns>Offset where record should be placed.</returns>
    private int LocateRecord( int iColumnIndex, out bool bFound, out bool bMul, bool bGetRkOffset )
    {
      bFound = false;
      bMul = false;

      if( iColumnIndex < m_iFirstColumn || m_iUsedSize <= 0 )
      {
        return 0;
      }

      int iOffset;
      int iCurrentColumn;

      /*if( iColumnIndex == m_iFirstColumn )
      {
        bFound = true;
        iCurrentColumn = m_iFirstColumn;

        if( bGetRkOffset )
        {
          if( m_dataProvider.ReadInt16( iOffset ) == ( int )TBIFFRecord.MulRK )
          {
            bMul = true;
            iOffset = 8;
          }
        }

        //return iOffset;
      }
      else*/
      if( iColumnIndex > m_iLastColumn )
      {
        iOffset = m_iUsedSize;
        iCurrentColumn = int.MaxValue;
      }
      else
      {
        iOffset = 0;
        bool bColumnAfter = ( iColumnIndex >= m_iCurrentColumn && m_iCurrentColumn >= 0 );
        int iLength = -BiffRecordRaw.DEF_HEADER_SIZE;
        //int iCurrentColumn = m_iFirstColumn;
        long lHeader;
        bool bIsMultyRK = HasMultiRkBlank;

        if( bColumnAfter )
        {
          iOffset = m_iCurrentOffset;
          iCurrentColumn = m_iCurrentColumn;
        }
        else
        {
          iCurrentColumn =  m_iFirstColumn;
        }

        // TODO: here we can also think about MulBlank and MulRK.

        do
        {
          iOffset += iLength + BiffRecordRaw.DEF_HEADER_SIZE;

          if( iOffset >= m_iUsedSize )
          {
            iCurrentColumn = int.MaxValue;
            break;
          }

          lHeader = m_dataProvider.ReadInt64( iOffset );
          TBIFFRecord biffCode = ( TBIFFRecord )( lHeader & ushort.MaxValue );
          lHeader >>= 16;
          iLength = ( int )( lHeader & ushort.MaxValue );

          if( biffCode == TBIFFRecord.String || biffCode == TBIFFRecord.Array ) continue;

          lHeader = ( lHeader >> 32 );

          // TODO: replace this with GetColumn method
          if( CellPositionSize == 4 )
          {
            iCurrentColumn = ( int )( lHeader & ushort.MaxValue );
          }
          else
          {
            iCurrentColumn = m_dataProvider.ReadInt32( iOffset + BiffRecordRaw.DEF_HEADER_SIZE + ExcelConstants.IntSize );
          }

          if( bIsMultyRK )
          {
            bool bMulti = ( biffCode == TBIFFRecord.MulRK || biffCode == TBIFFRecord.MulBlank );//UtilityMethods.IndexOf( DEF_MULTIRECORDS, sCode ) != -1;

            if( bMulti && GetOffsetToSubRecord( ref iOffset, iLength, iCurrentColumn,
              iColumnIndex, ref bMul, biffCode, bGetRkOffset ) )
            {
              break;
            }
          }
        }
        while( iCurrentColumn < iColumnIndex );
      }

      bFound = iCurrentColumn <= iColumnIndex;

      if( bFound && !bMul /*&& ( iColumnIndex > m_iCurrentColumn || m_iCurrentColumn < 0 )*/ )
      {
        m_iCurrentColumn = iCurrentColumn;
        m_iCurrentOffset = iOffset;
      }

      return iOffset;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOffset"></param>
    /// <param name="iLength"></param>
    /// <param name="iCurrentColumn"></param>
    /// <param name="iColumnIndex"></param>
    /// <param name="bMul"></param>
    /// <param name="biffCode"></param>
    /// <param name="bGetRkOffset"></param>
    /// <returns></returns>
    private bool GetOffsetToSubRecord( ref int iOffset, int iLength,
      int iCurrentColumn, int iColumnIndex, ref bool bMul, TBIFFRecord biffCode, bool bGetRkOffset )
    {
      int iCellPositionSize = CellPositionSize;
      int iLastColumnOffset = iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iLength - iCellPositionSize / 2;
      int iLastColumn = iCellPositionSize == 4 ?
        m_dataProvider.ReadInt16( iLastColumnOffset ) :
        m_dataProvider.ReadInt32( iLastColumnOffset );

      bool bResult = false;

      if( iCurrentColumn <= iColumnIndex && iLastColumn >= iColumnIndex )
      {
        bMul = true;

        int iSubRecordSize = ( biffCode == TBIFFRecord.MulRK ) ?
          MulRKRecord.DEF_SUB_ITEM_SIZE :
          MulBlankRecord.DEF_SUB_ITEM_SIZE;

        if( bGetRkOffset /*&& biffCode == TBIFFRecord.MulRK*/ )
        {
          m_iCurrentColumn = iCurrentColumn;
          m_iCurrentOffset = iOffset;

          int iCount = iColumnIndex - iCurrentColumn;
          iOffset = iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iCellPositionSize + iCount * iSubRecordSize;
        }

        bResult = true;
      }

      return bResult;
    }
    /// <summary>
    /// Ensures that internal data storage will be able to store specified number of bytes.
    /// </summary>
    /// <param name="iRequiredSize">Required data storage size.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    private void EnsureSize( int iRequiredSize, int iBlockSize )
    {
      if( m_dataProvider == null )
        throw new NotImplementedException();
        //m_dataProvider = ApplicationImpl.CreateDataProvider();

      int iProposedSize = iRequiredSize / iBlockSize * iBlockSize + iBlockSize;
      if (m_book != null)
          m_dataProvider.EnsureCapacity(iProposedSize, m_book.MaxImportColumns);
      else
          m_dataProvider.EnsureCapacity(iProposedSize);
    }
    /// <summary>
    /// Updates row dimension information.
    /// </summary>
    /// <param name="iColumnIndex">Accessed zero-based column index.</param>
    private void AccessColumn( int iColumnIndex )
    {
      if( iColumnIndex < 0 )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Column index cannot be less than 0" );

      m_iFirstColumn = ( m_iFirstColumn >= 0 )
        ? Math.Min( m_iFirstColumn, iColumnIndex )
        : iColumnIndex;

      m_iLastColumn = Math.Max( m_iLastColumn, iColumnIndex );
    }
    /// <summary>
    /// Updates row dimension information.
    /// </summary>
    /// <param name="iColumnIndex">Accessed zero-based column index.</param>
    /// <param name="cell">Cell that was set.</param>
    private void AccessColumn( int iColumnIndex, ICellPositionFormat cell )
    {
      if( iColumnIndex < 0 )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Column index cannot be less than 0" );

      if( cell != null )
      {
        m_iFirstColumn = ( m_iFirstColumn >= 0 )
          ? Math.Min( m_iFirstColumn, iColumnIndex )
          : iColumnIndex;

        m_iLastColumn = Math.Max( m_iLastColumn, iColumnIndex );
      }
      else
      {
        // Record was removed. Maybe we have to evaluate new dimensions.
        if( iColumnIndex == m_iFirstColumn )
        {
          if( iColumnIndex == m_iLastColumn )
          {
            m_iLastColumn = m_iFirstColumn = -1;
            m_iUsedSize = 0;
          }
          else
          {
            // Skip first record.
            m_iFirstColumn = GetColumn( 0 );
          }
        }
        else if( iColumnIndex == m_iLastColumn )
        {
          int iLastColumn = -1;
          int iOffset = 0;

          while( iOffset < m_iUsedSize )
          {
            short sCode = m_dataProvider.ReadInt16( iOffset );
            TBIFFRecord code = ( TBIFFRecord )sCode;
            int iLength = m_dataProvider.ReadInt16( iOffset + 2 );

            if( code != TBIFFRecord.Array && code != TBIFFRecord.String )
            {
              //if( UtilityMethods.IndexOf( DEF_MULTIRECORDS, sCode ) != -1 )
              if( code == TBIFFRecord.MulBlank || code == TBIFFRecord.MulRK )
              {
                iLastColumn = GetLastColumn( iOffset, iLength );
              }
              else
              {
                iLastColumn = GetColumn( iOffset );
              }
            }

            iOffset += BiffRecordRaw.DEF_HEADER_SIZE + iLength;
          }

          if( iLastColumn >= 0 )
          {
            m_iLastColumn = iLastColumn;
          }
          else
          {
            m_iFirstColumn = -1;
            m_iLastColumn = -1;
            m_iUsedSize = 0;
          }
          // TODO: update dimensions.
          //throw new NotImplementedException();
        }
      }
    }
    /// <summary>
    /// Removes formula string record.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <returns>Old offset to the string record.</returns>
    private int RemoveFormulaStringValue( int iColumnIndex )
    {
      int iFormulaRecordOffset;
      return RemoveFormulaStringValue( iColumnIndex, out iFormulaRecordOffset );
    }
    /// <summary>
    /// Removes formula string record.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <param name="iFormulaRecordOffset">Returns offset to the corresponding FormulaRecord.</param>
    /// <returns>Old offset to the string record.</returns>
    private int RemoveFormulaStringValue( int iColumnIndex, out int iFormulaRecordOffset )
    {
      iFormulaRecordOffset = -1;

      if( iColumnIndex < m_iFirstColumn || iColumnIndex > m_iLastColumn )
      {
        return -1;
      }

      bool bFound;
      //bool bMulti;
      int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );

      if( !bFound ) return -1;

      TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code != TBIFFRecord.Formula ) return -1;

      iFormulaRecordOffset = iOffset;
      iOffset = MoveNext( iOffset );

      if( iOffset >= m_iUsedSize ) return iOffset;

      code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code == TBIFFRecord.Array )
      {
        iOffset = MoveNext( iOffset );

        if( iOffset >= m_iUsedSize ) return iOffset;

        code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
      }

      if( code == TBIFFRecord.String )
        RemoveRecord( iOffset );

      return iOffset;
    }
    /// <summary>
    /// Removes record from the storage.
    /// </summary>
    /// <param name="iOffset">Offset to the record to remove.</param>
    private void RemoveRecord( int iOffset )
    {
      if( iOffset < m_iCurrentOffset )
      {
        m_iCurrentOffset = -1;
        m_iCurrentColumn = -1;
      }

      int iLength = m_dataProvider.ReadInt16( iOffset + 2 ) + BiffRecordRaw.DEF_HEADER_SIZE;

      //long lDataStart = m_ptrRowData.ToInt64() + iOffset;
      //IntPtr ptrSource = ( IntPtr )( lDataStart + iLength );
      //IntPtr ptrDest = ( IntPtr )lDataStart;

      int iSize = m_iUsedSize - iOffset - iLength;

      if( iSize > 0 )
      {
        //API.RtlMoveMemory( ptrDest, ptrSource, iSize );
        m_dataProvider.MoveMemory( iOffset, iOffset + iLength, iSize );
      }

      m_iUsedSize -= iLength;
    }
    /// <summary>
    /// Moves pointer to the next record.
    /// </summary>
    /// <param name="iOffset">Offset of the current record.</param>
    /// <returns>Offset to the next record.</returns>
    private int MoveNext( int iOffset )
    {
      int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
      return iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iLength;
    }
    /// <summary>
    /// Returns offsets, first one (X) is position before start column and the second one (Y) after end column.
    /// </summary>
    /// <param name="iStartColumn">Zero-based index of the start column.</param>
    /// <param name="iEndColumn">Zero-based index of the end column.</param>
    /// <param name="iRealStartColumn">Output start column index.</param>
    /// <param name="iRealEndColumn">Output end column index.</param>
    /// <returns>Point with evaluated offsets.</returns>
    private Point GetOffsets( int iStartColumn, int iEndColumn, out int iRealStartColumn, out int iRealEndColumn )
    {
      iRealStartColumn = -1;
      iRealEndColumn = -1;

      if( m_iFirstColumn < 0 ) return Point.Empty;

      iStartColumn = Math.Max( m_iFirstColumn, iStartColumn );
      iEndColumn = Math.Min( m_iLastColumn, iEndColumn );

      if( iStartColumn > iEndColumn ) return Point.Empty;

      if( iStartColumn == m_iFirstColumn && iEndColumn == m_iLastColumn )
      {
        iRealStartColumn = iStartColumn;
        iRealEndColumn = iEndColumn;
        return new Point( 0, m_iUsedSize );
      }

      bool bFound;
      //bool bMulti;

      int iStartOffset = LocateRecord( iStartColumn, out bFound/*, out bMulti*/ );
      //int iEndOffset = LocateRecord( iEndColumn, out bFound, out bMulti );

      if( iStartOffset >= m_iUsedSize ) return Point.Empty;

      iStartColumn = GetColumn( iStartOffset );
      iRealStartColumn = iStartColumn;
      iRealEndColumn = iStartColumn;
      int iEndOffset = iStartOffset;
      int iLength;

      if( iRealEndColumn == iEndColumn )
      {
        iEndOffset = MoveAfterRecord( iEndOffset );
      }
      else
      {
        while( iRealEndColumn < iEndColumn && iEndOffset < m_iUsedSize )
        {
          short sCode = m_dataProvider.ReadInt16( iEndOffset );
          TBIFFRecord code = ( TBIFFRecord )sCode;
          iLength = m_dataProvider.ReadInt16( iEndOffset + 2 );
          int iColumn = GetColumn( iEndOffset );

          if( iColumn > iEndColumn ) break;

          iEndOffset += BiffRecordRaw.DEF_HEADER_SIZE + iLength;

          bool bMultiRecord = code == TBIFFRecord.MulRK || code == TBIFFRecord.MulBlank;
          iRealEndColumn = bMultiRecord
            ? m_dataProvider.ReadInt16( iEndOffset - 2 )
            : iColumn;

          if( iEndOffset < m_iUsedSize )
          {
            code = ( TBIFFRecord )m_dataProvider.ReadInt16( iEndOffset );

            if( code == TBIFFRecord.Array )
            {
              iEndOffset = MoveNext( iEndOffset );

              if( iEndOffset < m_iUsedSize )
                code = ( TBIFFRecord )m_dataProvider.ReadInt16( iEndOffset );
            }

            if( code == TBIFFRecord.String )
            {
              iEndOffset = MoveNext( iEndOffset );
            }
          }
        }
      }

      return new Point( iStartOffset, iEndOffset );
    }
    /// <summary>
    /// Evaluates offset after specified record and satellite records.
    /// </summary>
    /// <param name="iOffset">Offset to the record.</param>
    /// <returns>Offset after specified record and satellite records.</returns>
    private int MoveAfterRecord( int iOffset )
    {
      if( iOffset < m_iUsedSize )
      {
        iOffset = MoveNext( iOffset );

        if( iOffset < m_iUsedSize )
        {
          TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

          if( code == TBIFFRecord.Array )
          {
            iOffset = MoveNext( iOffset );

            if( iOffset < m_iUsedSize )
              code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
          }

          if( code == TBIFFRecord.String )
          {
            iOffset = MoveNext( iOffset );
          }
        }
      }

      return iOffset;
    }
    /// <summary>
    /// Creates multi record.
    /// </summary>
    /// <param name="subCode">Type of sub record.</param>
    /// <returns>Created record.</returns>
    private IMultiCellRecord CreateMultiRecord( TBIFFRecord subCode )
    {
      TBIFFRecord code = ( subCode == TBIFFRecord.RK )
        ? TBIFFRecord.MulRK
        : TBIFFRecord.MulBlank;

      return ( IMultiCellRecord )BiffRecordFactory.GetRecord( code );
    }
    /// <summary>
    /// Tries to get record for the next column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="prevRecord">Records that corresponds to the column index.</param>
    /// <param name="iOffset">Current record offset.</param>
    /// <param name="bMulti">Indicates whether record is multi- record.</param>
    /// <returns>Record corresponding to the </returns>
    private ICellPositionFormat GetNextColumnRecord( int iColumnIndex, ICellPositionFormat prevRecord,
      ref int iOffset, bool bMulti )
    {
      int iLastColumn = iColumnIndex;
      ICellPositionFormat result = null;

      if( bMulti )
      {
        iLastColumn = GetLastColumnFromMultiRecord( iOffset );
      }

      if( iLastColumn >= iColumnIndex + 1 )
      {
        result = prevRecord;
      }
      else
      {
        if( prevRecord != null )
          iOffset += BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );

        if( iOffset < m_iUsedSize )
        {
          int iColumn = GetColumn( iOffset );

          if( iColumn == iColumnIndex + 1 )
          {
            result = ( ICellPositionFormat )GetRecordAtOffset( iOffset );
            //TBIFFRecord code = m_dataProvider.ReadInt16( iOffset );
            //bMulti = UtilityMethods.IndexOf( DEF_MULTIRECORDS, code ) != -1;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Returns last column index from the multi- record.
    /// </summary>
    /// <param name="iOffset">Offset to the record's start.</param>
    /// <returns>Last column index from the multi- record.</returns>
    private int GetLastColumnFromMultiRecord( int iOffset )
    {
      int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
      return m_dataProvider.ReadInt16( iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iLength - 2 );
    }
    /// <summary>
    /// Sets cell data without trying to create multi record.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index of the cell.</param>
    /// <param name="cell">Cell to set.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    private void SetOrdinaryRecord( int iColumnIndex, ICellPositionFormat cell, int iBlockSize )
    {
      if( cell != null && !HasRkBlank )
      {
        TBIFFRecord typeCode = cell.TypeCode;

        if( typeCode == TBIFFRecord.RK || typeCode == TBIFFRecord.Blank )
        {
          HasRkBlank = true;
        }
      }

      bool bFound;
      //bool bMulti;

      int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );
      int iPreparedSize = 0;

      if( bFound )
      {
        //        if( bMulti )
        //        {
        //          if( IsSameSubType( cell, iOffset ) )
        //          {
        //            InsertIntoRecord( iOffset, cell );
        //            return;
        //          }
        //          else
        //          {
        //            iOffset = SplitRecord( iOffset, cell.Column );
        //          }
        //        }
        //        else
      {
        iPreparedSize = m_dataProvider.ReadInt16( iOffset + 2 ) + BiffRecordRaw.DEF_HEADER_SIZE;
        int iEndOffset = iOffset + iPreparedSize;

        if( iEndOffset < m_iUsedSize )
        {
          TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iEndOffset );

          if( code == TBIFFRecord.Array )
          {
            int iLength = m_dataProvider.ReadInt16( iEndOffset + 2 );
            int iDelta = iLength + BiffRecordRaw.DEF_HEADER_SIZE;
            iEndOffset += iDelta;
            iPreparedSize += iDelta;

            if( iEndOffset < m_iUsedSize )
              code = ( TBIFFRecord )m_dataProvider.ReadInt16( iEndOffset );
          }

          if( code == TBIFFRecord.String && ( cell == null || cell.TypeCode != TBIFFRecord.Formula ) )
          {
            int iLength = m_dataProvider.ReadInt16( iEndOffset + 2 );
            iPreparedSize += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
          }
        }
      }
      }
      else
      {
        m_iCurrentOffset = -1;
        m_iCurrentColumn = -1;
      }

      BiffRecordRaw record = ( BiffRecordRaw )cell;
      int iRequiredSize = ( record != null )
        ? record.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE
        : 0;

      //EnsureSize( m_iUsedSize + iRequiredSize - iPreparedSize, iBlockSize );
      InsertRecordData( iOffset, iPreparedSize, iRequiredSize, record, iBlockSize );
      AccessColumn( iColumnIndex, cell );
    }

    /// <summary>
    /// Evaluates size of the data in the compressed state.
    /// </summary>
    /// <returns>Compressed data size.</returns>
    private int DefragmentDataStorage( DefragmentHelper rkRecordHelper,
      DefragmentHelper blankRecordHelper, DefragmentHelper ordinaryHelper, object userData )
    {
      int iOffset = 0;
      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        int iDelta = iLength + BiffRecordRaw.DEF_HEADER_SIZE;

        switch( code )
        {
          case TBIFFRecord.RK:
            iOffset = rkRecordHelper( userData );
            break;

          case TBIFFRecord.Blank:
            iOffset = blankRecordHelper( userData );
            break;

          case TBIFFRecord.Array:
          case TBIFFRecord.String:
          default:
            iOffset = ordinaryHelper( userData );
            break;
        }
      }

      return iOffset;
    }
    /// <summary>
    /// Simply skips record.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int SkipRecord( object userData )
    {
      OffsetData offsetData = ( OffsetData )userData;
      int iOffset = offsetData.StartOffset;

      int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
      int iDelta = iLength + BiffRecordRaw.DEF_HEADER_SIZE;

      offsetData.UsedSize += iDelta;
      iOffset += iDelta;
      offsetData.StartOffset = iOffset;

      return iOffset;
    }
    /// <summary>
    /// Skips RK records.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int SkipRKRecords( object userData )
    {
      OffsetData offsetData = ( OffsetData )userData;
      int iOffset = offsetData.StartOffset;
      int iStartColumn = GetColumn( iOffset );
      int iCurColumn = iStartColumn;
      int iFirstRecordSize = BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
      iOffset += iFirstRecordSize;
      int iUsedSize = offsetData.UsedSize;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code != TBIFFRecord.RK ) break;

        int iColumn = GetColumn( iOffset );

        if( iCurColumn + 1 != iColumn ) break;

        iOffset += BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
        iCurColumn = iColumn;
      }

      int iSubRecords = iCurColumn - iStartColumn + 1;

      if( iSubRecords > 1 )
      {
        iUsedSize += MulRKRecord.DEF_SUB_ITEM_SIZE * iSubRecords
          + MulRKRecord.DEF_FIXED_SIZE + BiffRecordRaw.DEF_HEADER_SIZE;
      }
      else
      {
        iUsedSize += iFirstRecordSize;
      }

      offsetData.UsedSize = iUsedSize;
      offsetData.StartOffset = iOffset;

      return iOffset;
    }
    /// <summary>
    /// Skips RK records.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int SkipBlankRecords( object userData )
    {
      OffsetData offsetData = ( OffsetData )userData;
      int iOffset = offsetData.StartOffset;
      int iStartColumn = GetColumn( iOffset );
      int iCurColumn = iStartColumn;
      int iFirstRecordSize = BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
      iOffset += iFirstRecordSize;
      int iUsedSize = offsetData.UsedSize;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code != TBIFFRecord.Blank ) break;

        int iColumn = GetColumn( iOffset );

        if( iCurColumn + 1 != iColumn ) break;

        iCurColumn = iColumn;
        iOffset += BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
      }

      int iSubRecords = iCurColumn - iStartColumn + 1;

      if( iSubRecords > 1 )
      {
        iUsedSize += MulBlankRecord.DEF_SUB_ITEM_SIZE * iSubRecords
          + MulBlankRecord.DEF_FIXED_SIZE + BiffRecordRaw.DEF_HEADER_SIZE;
      }
      else
      {
        iUsedSize += iFirstRecordSize;
      }

      offsetData.UsedSize = iUsedSize;
      offsetData.StartOffset = iOffset;

      return iOffset;
    }
    /// <summary>
    /// Simply skips record.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int CompressRecord( object userData )
    {
      WriteData writeData = ( WriteData )userData;
      int iOffset = writeData.Offset;

      int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
      int iDelta = iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      m_dataProvider.MoveMemory( writeData.UsedSize, iOffset, iDelta );

      //      IntPtr ptrSource = ( IntPtr )( m_ptrRowData.ToInt64() + iOffset );
      //      IntPtr ptrDest = ( IntPtr )( m_ptrRowData.ToInt64() + writeData.UsedSize );
      //      API.CopyMemory( ptrDest, ptrSource, iDelta );
      //m_dataProvider.WriteInto( writeData.Writer, iOffset, iDelta );
      //API.RtlMoveMemory( ptrDest, ptrSource, iDelta );

      writeData.UsedSize += iDelta;
      iOffset += iDelta;
      writeData.Offset = iOffset;

      return iOffset;
    }
    /// <summary>
    /// Writes RK records into data array.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int CompressRKRecords( object userData )
    {
      WriteData writeData = ( WriteData )userData;
      int iOffset = writeData.Offset;
      int iUsedSize = writeData.UsedSize;
      int iStartColumn = GetColumn( iOffset );
      int iCurColumn = iStartColumn;
      int iFirstRecordSize = BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
      iOffset += iFirstRecordSize;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code != TBIFFRecord.RK ) break;

        int iColumn = GetColumn( iOffset );

        if( iCurColumn + 1 != iColumn ) break;

        iOffset += BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
        iCurColumn = iColumn;
      }

      int iSubRecords = iCurColumn - iStartColumn + 1;

      if( iSubRecords > 1 )
      {
        int iLength = MulRKRecord.DEF_SUB_ITEM_SIZE * iSubRecords
          + MulRKRecord.DEF_FIXED_SIZE;
        
        CreateMulRKRecord( writeData, iSubRecords );
        HasMultiRkBlank = true;
      }
      else
      {
        iOffset = CompressRecord( userData );
      }

      return iOffset;
    }
    /// <summary>
    /// Writes Blank records into data array.
    /// </summary>
    /// <param name="userData">User data.</param>
    /// <returns>Offset after skipping record data.</returns>
    private int CompressBlankRecords( object userData )
    {
      WriteData writeData = ( WriteData )userData;
      int iOffset = writeData.Offset;
      int iUsedSize = writeData.UsedSize;
      int iStartColumn = GetColumn( iOffset );
      int iCurColumn = iStartColumn;
      int iFirstRecordSize = BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
      iOffset += iFirstRecordSize;

      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code != TBIFFRecord.Blank ) break;

        int iColumn = GetColumn( iOffset );

        if( iCurColumn + 1 != iColumn ) break;

        iOffset += BiffRecordRaw.DEF_HEADER_SIZE + m_dataProvider.ReadInt16( iOffset + 2 );
        iCurColumn = iColumn;
      }

      int iSubRecords = iCurColumn - iStartColumn + 1;

      if( iSubRecords > 1 )
      {
        int iLength = MulBlankRecord.DEF_SUB_ITEM_SIZE * iSubRecords
          + MulBlankRecord.DEF_FIXED_SIZE;
        
        BiffRecordRaw record = CreateMulBlankRecord( writeData.Offset, iSubRecords );
        //byte[] arrBuffer = writeData.Buffer;
        //record.InfillInternalData( arrBuffer, 4, 0 );
        //BitConverter.GetBytes( ( ushort )record.TypeCode ).CopyTo( arrBuffer, 0 );
        //BitConverter.GetBytes( ( ushort )iLength ).CopyTo( arrBuffer, 2 );
        m_dataProvider.WriteInt16( iUsedSize, ( short )record.RecordCode );
        iUsedSize += ExcelConstants.ShortSize;

        m_dataProvider.WriteInt16( iUsedSize, ( short )iLength );
        iUsedSize += ExcelConstants.ShortSize;

        record.InfillInternalData( m_dataProvider, iUsedSize, Version );
        iUsedSize += iLength;

        iLength += BiffRecordRaw.DEF_HEADER_SIZE;
        //m_dataProvider.WriteBytes( iUsedSize, arrBuffer, 0, iLength );
        writeData.UsedSize += iLength;
        writeData.Offset = iOffset;
        HasMultiRkBlank = true;
      }
      else
      {
        iOffset = CompressRecord( userData );
      }

      return iOffset;
    }
    /// <summary>
    /// Creates MulRK record based on set of RK records.
    /// </summary>
    /// <param name="writeData">
    /// Class that contains additional information needed to create
    /// MulRK record correctly.
    /// </param>
    /// <param name="iRecordsCount">Number of RK records.</param>
    /// <returns>Created record.</returns>
    private MulRKRecord CreateMulRKRecord( WriteData writeData, int iRecordsCount )
    {
      if( Version != ExcelVersion.Excel97to2003 )
        throw new NotSupportedException( "This method is supported only for Excel97-2003 file format" );

      int iOffset = writeData.Offset;
      int iUsedSize = writeData.UsedSize;

      if( iOffset < m_iCurrentOffset )
      {
        m_iCurrentOffset = -1;
        m_iCurrentColumn = -1;
      }

      m_dataProvider.WriteInt16( iUsedSize, ( short )TBIFFRecord.MulRK );
      iOffset += 2;
      iUsedSize += 2;

      int iRecordLength = MulRKRecord.DEF_FIXED_SIZE + MulRKRecord.DEF_SUB_ITEM_SIZE * iRecordsCount;
      m_dataProvider.WriteInt16( iUsedSize, ( short )iRecordLength );
      iOffset += 2;
      iUsedSize += 2;

      byte[] arrData = new byte[ 6 ];// usXF + iNumber
      m_dataProvider.ReadArray( iOffset, arrData, 4 );

      short sValue = m_dataProvider.ReadInt16( iOffset + 2 );
      short sLastColumn = ( short )( sValue + iRecordsCount - 1 );
      m_dataProvider.WriteBytes( iUsedSize, arrData, 0, 4 );
      iUsedSize += 4;

      iOffset = writeData.Offset + BiffRecordRaw.DEF_HEADER_SIZE + 4;
      for( int i = 0; i < iRecordsCount; i++  )
      {
        //ushort usXF = m_dataProvider.ReadUInt16( iOffset + DEF_XF_OFFSET );
        //int iNumber = m_dataProvider.ReadInt32( iOffset + RKRecord.DEF_HEADER_NUMBER_OFFSET );
        m_dataProvider.ReadArray( iOffset, arrData );
        m_dataProvider.WriteBytes( iUsedSize, arrData, 0, 6 );
        iOffset += RKRecord.DEF_RECORD_SIZE_WITH_HEADER;
        iUsedSize += MulRKRecord.DEF_SUB_ITEM_SIZE;
        //m_dataProvider.WriteUInt16( iUsedSize, usXF );
        //m_dataProvider.WriteInt32( iUsedSize + 2, iNumber );
      }

      m_dataProvider.WriteInt16( iUsedSize, sLastColumn );
      iUsedSize += 2;
      writeData.UsedSize = iUsedSize;
      writeData.Offset = iOffset - BiffRecordRaw.DEF_HEADER_SIZE - 4;
      return null;

      //      result.Records = arrRecords;
      //
      //      return result;
    }
    /// <summary>
    /// Creates MulRK record based on set of RK records.
    /// </summary>
    /// <param name="iOffset">Offset to the first RK record.</param>
    /// <param name="iRecordsCount">Number of RK records.</param>
    /// <returns>Created record.</returns>
    private MulRKRecord CreateMulRKRecord( int iOffset, int iRecordsCount )
    {
      if( iOffset < m_iCurrentOffset )
      {
        m_iCurrentOffset = -1;
        m_iCurrentColumn = -1;
      }

      MulRKRecord result = ( MulRKRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulRK );
      result.Row = GetRow( iOffset );
      result.FirstColumn = GetColumn( iOffset );
      result.LastColumn = result.FirstColumn + iRecordsCount - 1;

      List<MulRKRecord.RkRec> arrRecords = new List<MulRKRecord.RkRec>( iRecordsCount );
      
      for( int i = 0; i < iRecordsCount; i++ )
      {
        //RKRecord rk = ( RKRecord )BiffRecordFactory.GetRecord( m_dataProvider, iOffset );
        ushort usXF = GetXFIndex( iOffset, false );
        int iNumber = m_dataProvider.ReadInt32( iOffset + RKRecord.DEF_HEADER_NUMBER_OFFSET );
        MulRKRecord.RkRec rkRec = new MulRKRecord.RkRec( usXF, iNumber );
        arrRecords.Add( rkRec );
        iOffset += RKRecord.DEF_RECORD_SIZE_WITH_HEADER;//rk.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      }

      result.Records = arrRecords;

      return result;
    }
    /// <summary>
    /// Creates MulBlank record based on set of Blank records.
    /// </summary>
    /// <param name="iOffset">Offset to the first Blank record.</param>
    /// <param name="iRecordsCount">Number of Blank records.</param>
    /// <returns>Created record.</returns>
    private MulBlankRecord CreateMulBlankRecord( int iOffset, int iRecordsCount )
    {
      if( iOffset < m_iCurrentOffset )
      {
        m_iCurrentOffset = -1;
        m_iCurrentColumn = -1;
      }

      MulBlankRecord result = ( MulBlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulBlank );
      result.Row = GetRow( iOffset );
      result.FirstColumn = GetColumn( iOffset );
      result.LastColumn = result.FirstColumn + iRecordsCount - 1;

      List<ushort> arrIndexes = new List<ushort>( iRecordsCount );
      
      for( int i = 0; i < iRecordsCount; i++ )
      {
        ushort usXF = GetXFIndex( iOffset, false );
        arrIndexes.Add( usXF );
        iOffset += BlankRecord.DEF_RECORD_SIZE_WITH_HEADER;
      }

      result.ExtendedFormatIndexes = arrIndexes;

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mulBlank"></param>
    /// <param name="mulRK"></param>
    /// <param name="iSizeDelta"></param>
    /// <returns></returns>
    private List<int> GetMultiRecordsOffsets( MulBlankRecord mulBlank, MulRKRecord mulRK,
      out int iSizeDelta )
    {
      int iLength;
      iSizeDelta = 0;
      int iOffset = 0;
      List<int> arrOffsets = new List<int>();

      while( iOffset < m_iUsedSize )
      {
        IMultiCellRecord multiCell = CreateMultiCellRecord( iOffset, mulBlank, mulRK, out iLength );

        if( multiCell != null )
        {
          iSizeDelta += ( multiCell.LastColumn - multiCell.FirstColumn + 1 )
            * multiCell.GetSeparateSubRecordSize( Version ) - iLength;

          arrOffsets.Add( iOffset );
        }
        
        iOffset += iLength;
      }

      return arrOffsets;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrOffsets"></param>
    /// <param name="iSizeDelta"></param>
    /// <param name="mulBlank"></param>
    /// <param name="mulRK"></param>
    /// <param name="bIgnoreStyles"></param>
    private void DecompressStorage( List<int> arrOffsets, int iSizeDelta,
      MulBlankRecord mulBlank, MulRKRecord mulRK, bool bIgnoreStyles )
    {
      m_iUsedSize += iSizeDelta;
      int iNormalDataEnd = m_iUsedSize; // - starting from that offset we have decompressed data.

      int iLength;
      //IntPtr ptrRowData = m_dataProvider.DataPointer;
      //long lPtrData = ptrRowData.ToInt64();

      for( int i = arrOffsets.Count - 1; i >= 0; i-- )
      {
        int iStartOffset = arrOffsets[ i ]; // offset to the record that must be decompressed.
        IMultiCellRecord multiCell = CreateMultiCellRecord( iStartOffset, mulBlank, mulRK, out iLength );
        // Offset to the data that is not compressed and hasn't been copied yet.
        int iNormalDataStart = iStartOffset + iLength;
        // End of not compressed data
        int iBlockEnd = iNormalDataEnd - iSizeDelta;
        // Size of not compressed data
        int iBlockLen = iBlockEnd - iNormalDataStart;

        // Move normal data.
        if( iBlockLen > 0 )
        {
          //long lPtrDataStart = lPtrData + iNormalDataStart;
          //IntPtr ptrCorrectDataStart = ( IntPtr )lPtrDataStart;
          //IntPtr ptrDestination = ( IntPtr )( lPtrDataStart + iSizeDelta );
          //Memory.RtlMoveMemory( ptrDestination, ptrCorrectDataStart, iBlockLen );
          m_dataProvider.MoveMemory( iNormalDataStart + iSizeDelta, iNormalDataStart, iBlockLen );
        }

        // Decompress record data.
        int iDecompressedSize = ( multiCell.LastColumn - multiCell.FirstColumn + 1 )
          * multiCell.GetSeparateSubRecordSize( Version );

        // TODO: We can try to optimize this code - don't create so large number of records.
        BiffRecordRaw[] arrRecords = multiCell.Split( bIgnoreStyles );
        //iNormalDataEnd = iNormalDataStart + iSizeDelta - iDecompressedSize;
        InsertRecordData( iStartOffset, /*iNormalDataEnd,*/ arrRecords );

        // Reassign variables for next block.
        iNormalDataEnd = iNormalDataStart + iSizeDelta;
        iSizeDelta -= iDecompressedSize - iLength;
      }
    }
    /// <summary>
    /// Creates multi-cell record (MulRK or MulBlank) at the specified offset.
    /// </summary>
    /// <param name="iOffset">Offset to the record to create.</param>
    /// <param name="mulBlank">Instance of MulBlank record.</param>
    /// <param name="mulRK">Instance of MulRK record.</param>
    /// <param name="iLength">Record length including header.</param>
    /// <returns>Created record or null if there is incorrect record type.</returns>
    private IMultiCellRecord CreateMultiCellRecord( int iOffset,
      MulBlankRecord mulBlank, MulRKRecord mulRK, out int iLength )
    {
      IMultiCellRecord multiCell = null;
      TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );
      int iDataSize = m_dataProvider.ReadInt16( iOffset + 2 );
      iLength = iDataSize + BiffRecordRaw.DEF_HEADER_SIZE;;

      if( code == TBIFFRecord.MulRK )
      {
        mulRK.Length = iDataSize;
        mulRK.ParseStructure( m_dataProvider, iOffset + BiffRecordRaw.DEF_HEADER_SIZE, 0, Version );
        multiCell = mulRK;
      }
      else if( code == TBIFFRecord.MulBlank )
      {
        mulBlank.Length = iDataSize;
        mulBlank.ParseStructure( m_dataProvider, iOffset + BiffRecordRaw.DEF_HEADER_SIZE, 0, Version );
        multiCell = mulBlank;
      }

      return multiCell;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOffset"></param>
    /// <param name="multi"></param>
    /// <param name="bIgnoreStyles"></param>
    private void DecompressRecord( int iOffset, IMultiCellRecord multi, bool bIgnoreStyles )
    {
      BiffRecordRaw[] arrSubRecords = multi.Split( bIgnoreStyles );
      InsertRecordData( iOffset, arrSubRecords );
    }
    /// <summary>
    /// Gets row from the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <returns>Zero-based row index.</returns>
    public int GetRow( int recordStart )
    {
      switch( Version )
      {
        case ExcelVersion.Excel97to2003:
          return m_dataProvider.ReadUInt16( recordStart + BiffRecordRaw.DEF_HEADER_SIZE );

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return m_dataProvider.ReadInt32( recordStart + BiffRecordRaw.DEF_HEADER_SIZE );

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets row for the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="rowIndex">Zero-based row index to set.</param>
    private void SetRow( int recordStart, int rowIndex )
    {
      switch( Version )
      {
        case ExcelVersion.Excel97to2003:
          m_dataProvider.WriteUInt16( recordStart + BiffRecordRaw.DEF_HEADER_SIZE,
            ( ushort )rowIndex );
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          m_dataProvider.WriteInt32( recordStart + BiffRecordRaw.DEF_HEADER_SIZE,
            rowIndex );
          break;

        default:
          throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Gets column from the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <returns>Zero-based column index.</returns>
    public int GetColumn( int recordStart )
    {
      switch( Version )
      {
        case ExcelVersion.Excel97to2003:
          return m_dataProvider.ReadUInt16( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + ExcelConstants.ShortSize );

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return m_dataProvider.ReadInt32( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + ExcelConstants.IntSize );

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets column for the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="columnIndex">Zero-based column index to set.</param>
    private void SetColumn( int recordStart, int columnIndex )
    {
      switch( Version )
      {
        case ExcelVersion.Excel97to2003:
          m_dataProvider.WriteUInt16( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + ExcelConstants.ShortSize, ( ushort )columnIndex );
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          m_dataProvider.WriteInt32( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + ExcelConstants.IntSize, columnIndex );
          break;

        default:
          throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Gets last column index from the MulRk or MulBlank record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="iLength">Record length.</param>
    /// <returns>Zero-based column index.</returns>
    private int GetLastColumn( int recordStart, int iLength )
    {
      switch( Version )
      {
        case ExcelVersion.Excel97to2003:
          return m_dataProvider.ReadInt16( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + iLength - ExcelConstants.ShortSize );

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return m_dataProvider.ReadInt32( recordStart + BiffRecordRaw.DEF_HEADER_SIZE
            + iLength - ExcelConstants.IntSize );

        default:
          throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Gets XF index from the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="bMulti">Indicates whether record is multi record or not.</param>
    /// <returns>Zero-based index to the extended format record with style settings.</returns>
    [ CLSCompliant( false ) ]
    public ushort GetXFIndex( int recordStart, bool bMulti )
    {

      if( !bMulti )
      {
        recordStart += BiffRecordRaw.DEF_HEADER_SIZE + ExcelConstants.IntSize;

        ExcelVersion version = Version;

        if( version !=ExcelVersion.Excel97to2003)
        {
          recordStart += ExcelConstants.IntSize;
        }
      }

      return m_dataProvider.ReadUInt16( recordStart );
    }

    /// <summary>
    /// Sets XF index for the record.
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="xfIndex">Zero-based index to the extended format record with style settings to set.</param>
    private void SetXFIndex( int recordStart, ushort xfIndex )
    {
      int iOffset =  recordStart + BiffRecordRaw.DEF_HEADER_SIZE + ExcelConstants.IntSize;

      ExcelVersion version = Version;
      
      if( version !=ExcelVersion.Excel97to2003 )
      {
        iOffset += ExcelConstants.IntSize;
      }

      m_dataProvider.WriteUInt16( iOffset, xfIndex );
    }
    /// <summary>
    /// Sets XF index inside of "multi" record (MulRK or MulBlank).
    /// </summary>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="xfIndex">Zero-based index to the extended format record with style settings to set.</param>
    /// <param name="iColumnIndex">Zero-based column index of the cell to set style.</param>
    /// <param name="subRecordSize">Size of subrecord inside of multi record.</param>
    private void SetXFIndexMulti( int recordStart, ushort xfIndex, int iColumnIndex, int subRecordSize )
    {
      int iStartColumn = GetColumn( recordStart );

      recordStart += BiffRecordRaw.DEF_HEADER_SIZE + ExcelConstants.IntSize;

      ExcelVersion version = Version;

      if( version !=ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize;
      }

      recordStart += subRecordSize * ( iColumnIndex - iStartColumn );
      m_dataProvider.WriteUInt16( recordStart, xfIndex );
    }
    internal void UpdateFormulaFlags()
    {
      int iOffset = 0;
      while( iOffset < m_iUsedSize )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.Formula )
        {
          FormulaRecord.UpdateOptions( m_dataProvider, iOffset );
        }

        iOffset = MoveNext( iOffset );
      }

    }
    #endregion

    #region IBiffStorage Members
    /// <summary>
    /// Returns type code of the biff storage. Read-only.
    /// </summary>
    public TBIFFRecord TypeCode
    {
      get
      {
        return ( TBIFFRecord )(-1);
      }
    }
    /// <summary>
    /// Returns code of the biff storage. Read-only.
    /// </summary>
    public int RecordCode
    {
      get
      {
        return -1;
      }
    }
    /// <summary>
    /// Indicates whether data array is required by this record. Read-only.
    /// </summary>
    public bool NeedDataArray
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public long StreamPos
    {
      get
      {
        return -1;//m_lStreamPosition;
      }
      set
      {
        //m_lStreamPosition = value;
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public int GetStoreSize( ExcelVersion version )
    {
      Compress();
      return Math.Max( 0, m_iUsedSize - BiffRecordRaw.DEF_HEADER_SIZE );
    }
    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    public int FillStream( BinaryWriter writer, DataProvider provider, IEncryptor encryptor, int streamPosition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      //int iUsedSize;
      //Defragment( writer, arrBuffer, out iUsedSize );
      //Compress();

      ByteArrayDataProvider byteProvider = ( ByteArrayDataProvider )provider;
      byte[] arrBuffer = byteProvider.InternalBuffer;

      if( encryptor != null )
      {
        int iOffset = 0;
        //long lStreamPos = writer.BaseStream.Position;
        int iPosInBuffer = 0;
        int iBufferSize = arrBuffer.Length;

        while( iOffset < m_iUsedSize )
        {
          short sCode = m_dataProvider.ReadInt16( iOffset );
          int iLength = m_dataProvider.ReadUInt16( iOffset + 2 );
          streamPosition += BiffRecordRaw.DEF_HEADER_SIZE;
          iOffset += BiffRecordRaw.DEF_HEADER_SIZE;

          if( iBufferSize < iPosInBuffer + iLength + BiffRecordRaw.DEF_HEADER_SIZE )
          {
            byteProvider.WriteInto( writer, 0, iPosInBuffer, arrBuffer );
            iPosInBuffer = 0;
          }

          byteProvider.WriteInt16( iPosInBuffer, sCode );
          iPosInBuffer += 2;

          byteProvider.WriteInt16( iPosInBuffer, ( short )iLength );
          iPosInBuffer += 2;

          m_dataProvider.CopyTo( iOffset, arrBuffer, iPosInBuffer, iLength );

          encryptor.Encrypt( byteProvider, iPosInBuffer, iLength, streamPosition );
          iPosInBuffer += iLength;

          streamPosition += iLength;
          iOffset += iLength;
        }

        if( iPosInBuffer != 0 )
        {
          byteProvider.WriteInto( writer, 0, iPosInBuffer, arrBuffer );
        }
      }
      else
      {
        m_dataProvider.WriteInto( writer, 0, m_iUsedSize, arrBuffer );
      }

      return m_iUsedSize;

      //      IntPtr ptrDataToWrite = Defragment( out iUsedSize );
      //
      //      if( iUsedSize > 0 )
      //      {
      //        IntPtrDataProvider provider = new IntPtrDataProvider( ptrDataToWrite );
      //        provider.WriteInto( writer, 0, iUsedSize );
      //      }
      //
      //      if( iUsedSize != m_iUsedSize )
      //        Marshal.FreeHGlobal( ptrDataToWrite );
      //
      //      return iUsedSize;

      //      m_dataProvider.WriteInto( writer, 0, m_iUsedSize );
      //      return m_iUsedSize;
    }
    #endregion

    #region Internal classes
    /// <summary>
    /// Data for size evaluation.
    /// </summary>
    private class OffsetData
    {
      /// <summary>
      /// Start offset.
      /// </summary>
      public int StartOffset;
      /// <summary>
      /// Used size.
      /// </summary>
      public int UsedSize;
    }
    /// <summary>
    /// Data for writing defragmented data.
    /// </summary>
    private class WriteData
    {
      /// <summary>
      /// Offset to the record data in the old storage.
      /// </summary>
      public int Offset;
      //      /// <summary>
      //      /// Pointer to the data.
      //      /// </summary>
      //      public IntPtr PtrData;
      /// <summary>
      /// Size of the data.
      /// </summary>
      public int UsedSize;
      //      /// <summary>
      //      /// Binary writer.
      //      /// </summary>
      //      public BinaryWriter Writer;
      //      /// <summary>
      //      /// Data buffer.
      //      /// </summary>
      //      public byte[] Buffer;
    }
    /// <summary>
    /// Delegate used for defragmentation.
    /// </summary>
    private delegate int DefragmentHelper( object helperData );
    #endregion

    #region GetValue optimize methods
    /// <summary>
    /// Gets bool value by column index. Without check input parameters.
    /// </summary>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns boolean value; otherwise - 0.</returns>
    public int GetBoolValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.BoolErr )
        {
          int value = BoolErrRecord.ReadValue( m_dataProvider, iOffset, Version );

          if( ( value & 0xff00 ) == 0 )
            return value;
        }
      }

      return 0;
    }
    /// <summary>
    /// Gets formula bool value by column index. Without check input parameters.
    /// </summary>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns boolean value; otherwise - 0.</returns>
    public int GetFormulaBoolValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.Formula )
        {
          ulong value = ( ulong )FormulaRecord.ReadInt64Value( m_dataProvider, iOffset, Version );

          if( ( value & FormulaRecord.DEF_FIRST_MASK ) == FormulaRecord.DEF_BOOL_MASK )
            return ( int )( value & 0xff0000 );
        }
      }

      return 0;
    }
    /// <summary>
    /// Gets error value by column index. Without check input parameters.
    /// </summary>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns error value; otherwise - null.</returns>
    public string GetErrorValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.BoolErr )
        {
          int value = BoolErrRecord.ReadValue( m_dataProvider, iOffset, Version );

          if( ( value & 0xff00 ) != 0 )
          {
            return GetErrorString( value & 0xff );
          }
        }
      }

      return null;
    }
    /// <summary>
    /// Gets formula error value by column index. Without check input parameters.
    /// </summary>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns error value; otherwise - null.</returns>
    public string GetFormulaErrorValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.Formula )
        {
          ulong value = ( ulong )FormulaRecord.ReadInt64Value( m_dataProvider, iOffset, Version );

          if( ( value & FormulaRecord.DEF_FIRST_MASK ) == FormulaRecord.DEF_ERROR_MASK )
          {
            return GetErrorString( ( int )( ( value & 0xff0000 ) >> 16 ) );
          }
        }
      }

      return null;
    }
    /// <summary>
    /// Gets number value.
    /// </summary>
    /// <param name="iCol">Column index.</param>
    /// <returns>Returns number value or NaN.</returns>
    public double GetNumberValue( int iCol,int sheetIndex )
    {
      bool bFound;
      bool bMul;

      int iOffset = LocateRecord( iCol, out bFound, out bMul, true );

      if( bFound )
      {
        if( bMul )
        {
          return RKRecord.EncodeRK( m_dataProvider.ReadInt32( iOffset + 2 ) );
        }
        else
        {
          int iCode = m_dataProvider.ReadInt16( iOffset );

          if( iCode == ( int )TBIFFRecord.Number )
          {
            return NumberRecord.ReadValue( m_dataProvider, iOffset, Version );
          }
          else if( iCode == ( int )TBIFFRecord.RK )
          {
            int iValue = RKRecord.ReadValue( m_dataProvider, iOffset, Version );
            return RKRecord.EncodeRK( iValue );
          }
          else if (iCode == (int)TBIFFRecord.LabelSST)
          {
              int index = LabelSSTRecord.GetSSTIndex(m_dataProvider, iOffset, Version);
              string text = m_book.InnerSST[index].Text;
              if (m_book.ActiveSheet != null)
              {
                  DateTime value;
                  string format = m_book.Worksheets[sheetIndex][m_row, iCol + 1].NumberFormat;
                  RangeImpl range=m_book.Worksheets[sheetIndex][m_row, iCol + 1] as RangeImpl;
                  if (CheckFormat(format))
                  {
                      int charindex = format.IndexOf(';');
                      if (charindex != -1)
                      {
                          format = format.Remove(charindex, format.Length - charindex);
                      }
                      if (DateTime.TryParse(text,System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out value))
                      {
                          DateTime dtTmp ;
                          if (range != null && range.TryGetDateTimeByCulture(text, text.Contains(DEF_DOT), out dtTmp))
                              value = dtTmp;
                          return value.ToOADate();
                      }
                      else
                      {
                          DateTime dtTmp;
                          if (range != null && range.TryGetDateTimeByCulture(text, true, out dtTmp))
                          {
                              value = dtTmp;
                              return value.ToOADate();
                          }
                      }
                  }
              }
          }          
         
        }
      }

      return double.NaN;
    }
    /// <summary>
    /// Checks the format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    private bool CheckFormat(string format)
    {      
        int count = 0;
        foreach (string date in dateFormats)
        {
            if (format.Contains(date))
            {
                count++;
            }
        }
        if (count > 1) return true;
        else return false;
    }

    /// <summary>
    /// Gets formula number value.
    /// </summary>
    /// <param name="iCol">Column index.</param>
    /// <returns>Returns number value or NaN.</returns>
    public double GetFormulaNumberValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.Formula )
        {
          return FormulaRecord.ReadDoubleValue( m_dataProvider, iOffset, Version );
        }
      }

      return double.NaN;
    }
    /// <summary>
    /// Gets string value.
    /// </summary>
    /// <param name="iColumn">Column index.</param>
    /// <param name="sst">Represents sst dictionary.</param>
    /// <returns>Returns string value or null.</returns>
    public string GetStringValue( int iColumn, SSTDictionary sst )
    {
      bool bFound;

      int iOffset = LocateRecord( iColumn, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.LabelSST )
        {
          int iSSTIndex = LabelSSTRecord.GetSSTIndex( m_dataProvider, iOffset, Version );

          return sst[ iSSTIndex ];
        }
        else if( iCode == ( int )TBIFFRecord.Label || iCode == ( int )TBIFFRecord.RString )
        {
          // TODO: implement this - change offset to the correct one, depending on Excel version
          //throw new NotImplementedException();

          int iOffsetDelta = 10;

          if( Version != ExcelVersion.Excel97to2003 )
            iOffsetDelta += ExcelConstants.IntSize;

          int iLength;
          return m_dataProvider.ReadString16Bit( iOffset + iOffsetDelta, out iLength );
        }

        return null;
      }

      return null;
    }
    /// <summary>
    /// Returns formula string value.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    /// <returns>String result of the formula.</returns>
    public string GetFormulaStringValue( int iColumnIndex )
    {
      bool bFound;

      int iOffset = LocateRecord( iColumnIndex, out bFound/*, out bMulti*/ );

      if( !bFound ) return null;

      return GetFormulaStringValueByOffset( iOffset );
    }
    /// <summary>
    /// Returns formula string value.
    /// </summary>
    /// <param name="iOffset">Offset to record.</param>
    /// <returns>Formula string value.</returns>
    public string GetFormulaStringValueByOffset( int iOffset )
    {
      TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code != TBIFFRecord.Formula ) return null;

      int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
      iOffset += BiffRecordRaw.DEF_HEADER_SIZE + iLength;

      if( iOffset >= m_iUsedSize ) return null;

      code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

      if( code != TBIFFRecord.String ) return null;

      iOffset += 4;

      int iLen = m_dataProvider.ReadInt16( iOffset );
      int iByteCount;

      return  m_dataProvider.ReadString( iOffset + 2, iLen, out iByteCount, false );
    }
    /// <summary>
    /// Gets string value.
    /// </summary>
    /// <param name="iCol">Column index.</param>
    /// <returns>Returns ptg array or null.</returns>
    public Ptg[]  GetFormulaValue( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.Formula )
        {
          return FormulaRecord.ReadValue( m_dataProvider, iOffset, Version );
          //          int finalOffset;
          //          int iLength = m_dataProvider.ReadUInt16( iOffset + 24 );
          //
          //          return FormulaUtil.ParseExpression( m_dataProvider, iOffset + 26,
          //            iLength, out finalOffset, Version );
        }
      }

      return null;
    }
    /// <summary>
    /// Gets cell type from current column.
    /// </summary>
    /// <param name="iCol">Indicates column.</param>
    /// <param name="bNeedFormulaSubType">Indicates is need to indentify formula sub type.</param>
    /// <returns>Returns cell type.</returns>
    public TRangeValueType GetCellType( int iCol, bool bNeedFormulaSubType )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        switch( ( TBIFFRecord )iCode )
        {
          case TBIFFRecord.BoolErr:
            int iResult = BoolErrRecord.ReadValue( m_dataProvider, iOffset, Version );

            TRangeValueType result = ( ( iResult & 0xff00 ) == 0 )
              ? TRangeValueType.Boolean
              : TRangeValueType.Error;

            return result;

          case TBIFFRecord.MulRK:
          case TBIFFRecord.RK:
          case TBIFFRecord.Number:
            return TRangeValueType.Number;

          case TBIFFRecord.LabelSST:
          case TBIFFRecord.RString:
          case TBIFFRecord.Label:
            return TRangeValueType.String;

          case TBIFFRecord.Formula:

            TRangeValueType value = ( bNeedFormulaSubType )
              ? GetSubFormulaType( iOffset )
              : TRangeValueType.Formula;

            return value;

          default:
            return TRangeValueType.Blank;
        }
      }

      return TRangeValueType.Blank;
    }
    /// <summary>
    /// Gets sub formula type.
    /// </summary>
    /// <param name="iOffset">Represents offset on start formula record.</param>
    /// <returns></returns>
    private TRangeValueType GetSubFormulaType( int iOffset )
    {
      TRangeValueType result = TRangeValueType.Formula;

      ulong uBoolErrorValue = ( ulong )FormulaRecord.ReadInt64Value( m_dataProvider, iOffset, Version );
      ulong uBoolErrorMask = uBoolErrorValue & FormulaRecord.DEF_FIRST_MASK;

      if( uBoolErrorMask == FormulaRecord.DEF_BOOL_MASK )
      {
        result |= TRangeValueType.Boolean;
      }
      else if( uBoolErrorMask == FormulaRecord.DEF_ERROR_MASK )
      {
        result |= TRangeValueType.Error;
      }
      else if( uBoolErrorMask == FormulaRecord.DEF_BLANK_MASK )
      {
        result |= TRangeValueType.Blank;
      }
      else
      {
        int iLength = m_dataProvider.ReadInt16( iOffset + 2 );
        int iStringOffset = iOffset + BiffRecordRaw.DEF_HEADER_SIZE + iLength;
        bool bString = iStringOffset < m_iUsedSize;
        bString = bString && ( TBIFFRecord )m_dataProvider.ReadInt16( iStringOffset ) == TBIFFRecord.String;

        result |= ( bString )
          ? TRangeValueType.String
          : TRangeValueType.Number;
      }

      return result;
    }
    /// <summary>
    /// Indicates if there is formula record.
    /// </summary>
    /// <param name="iColumn">Zero based column index.</param>
    /// <returns>Indicates whether formula record is contained.</returns>
    public bool HasFormulaRecord( int iColumn )
    {
      bool bFound;
      
      int iOffset = LocateRecord( iColumn, out bFound );

      if( bFound )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.Formula )
          return true;
      }

      return false;
    }
    /// <summary>
    /// Indicates is contain formula array.
    /// </summary>
    /// <param name="iCol">Zero based column index.</param>
    /// <returns>If found return true; otherwise - false.</returns>
    public bool HasFormulaArrayRecord( int iCol )
    {
      bool bFound;

      int iOffset = LocateRecord( iCol, out bFound );

      if( bFound )
      {
        TBIFFRecord code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

        if( code == TBIFFRecord.Formula )
        {
          iOffset = MoveNext( iOffset );

          if( iOffset < m_iUsedSize )
          {
            code = ( TBIFFRecord )m_dataProvider.ReadInt16( iOffset );

            if( code == TBIFFRecord.Array )
              return true;
          }
        }
      }

      return false;
    }
    /// <summary>
    /// Gets error string by number.
    /// </summary>
    /// <param name="value">Represents number.</param>
    /// <returns>Returns error string or null.</returns>
    internal string GetErrorString( int value )
    {
      IDictionary dic = FormulaUtil.ErrorCodeToName;

      return ( dic.Contains( value ) )
        ? ( string )dic[ value ]
        : null;
    }
    /// <summary>
    /// Sets the workbook.
    /// </summary>
    /// <param name="book">The book.</param>
    /// <param name="iRow">The i row.</param>
    internal void SetWorkbook(WorkbookImpl book, int iRow)
    {
        m_book = book;
        m_row = iRow;
    }
    /// <summary>
    /// Sets formula value. Use for setting FormulaError, FormulaBoolean, FormulaNumber, FormulaString values.
    /// </summary>
    /// <param name="iColumn">Zero based column index.</param>
    /// <param name="value">Represents value for set.</param>
    /// <param name="strRecord">Represents string record as formula string value. Can be null.</param>
    /// <param name="iBlockSize">Memory allocation elementary block size.</param>
    [ CLSCompliant( false ) ]
    public void SetFormulaValue( int iColumn, double value, StringRecord strRecord, int iBlockSize )
    {
      bool bFound;
      bool isArray = false;

      int iOffset = LocateRecord( iColumn, out bFound );

      if( !bFound )
        throw new ApplicationException( "Cannot set formula number." );

      //m_dataProvider.WriteDouble( iOffset + 10, value );
      FormulaRecord.WriteDoubleValue( m_dataProvider, iOffset, Version, value );

      iOffset += m_dataProvider.ReadInt16( iOffset + 2 ) + BiffRecordRaw.DEF_HEADER_SIZE;

      if( iOffset < m_iUsedSize )
      {
        int iCode = m_dataProvider.ReadInt16( iOffset );

        if( iCode == ( int )TBIFFRecord.Array )
        {
          //m_dataProvider.MoveNe
            isArray = true;
            iOffset = MoveNext(iOffset);
            iCode = m_dataProvider.ReadInt16(iOffset);
        }

        if (iCode == (int)TBIFFRecord.String && !isArray)
          RemoveRecord( iOffset );
      }

      if( strRecord != null )
      {
        int iSize = strRecord.GetStoreSize( Version ) + BiffRecordRaw.DEF_HEADER_SIZE;
        InsertRecordData( iOffset, 0, iSize, strRecord, iBlockSize );
      }
    }
    #endregion

    #region IOutline Members
    //    /// <summary>
    //    /// Index of this row.
    //    /// </summary>
    //    [ CLSCompliant( false ) ]
    //    public ushort RowNumber
    //    {
    //      get
    //      {
    //        return m_usRowNumber;
    //      }
    //      set
    //      {
    //        m_usRowNumber = value;
    //      }
    //    }

    /// <summary>
    /// Height of the row, in twips = 1/20 of a point.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort Height
    {
      get
      {
        return m_usHeight;
      }
      set
      {
        if( value > RowRecord.DEF_MAX_HEIGHT * 20 )
            throw new ArgumentOutOfRangeException("Row Height should be less than " + RowRecord.DEF_MAX_HEIGHT);

        m_usHeight = value;
        //IsBadFontHeight = true;
      }
    }

    /// <summary>
    /// If the row is formatted, then this is the index to
    /// the extended format record.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort ExtendedFormatIndex
    {
      get
      {
        return m_usXFIndex;
        //return ( ushort )( ( ( int )m_optionFlags & 0xfff0000 ) >> 16 );
      }
      set
      {
        m_usXFIndex = value;

        //        int iOptionFlags = ( int )m_optionFlags;
        //        iOptionFlags &= ( ~0xfff0000 );
        //
        //        iOptionFlags |= ( ( value << 16 ) & 0xfff0000 );
        //        m_optionFlags = ( OptionFlags )iOptionFlags;

        if( value != RangeImpl.DEF_NORMAL_STYLE_INDEX )
        {
          IsFormatted = true;
        }

      }
    }

    /// <summary>
    /// The outline level of this row.
    /// Changes some bits of m_usOptionFlags private member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When value is more than 7.
    /// </exception>
    [ CLSCompliant( false ) ]
    public ushort OutlineLevel
    {
      get
      {
        return ( ushort )( ( int )m_optionFlags & RowRecord.DEF_OUTLINE_LEVEL_MASK );
      }
      set
      {
        if( value > RowRecord.DEF_OUTLINE_LEVEL_MASK )
          throw new ArgumentOutOfRangeException();

        int iOptionFlags = ( int )m_optionFlags;

        iOptionFlags &= ( ~RowRecord.DEF_OUTLINE_LEVEL_MASK );
        iOptionFlags |= ( value & RowRecord.DEF_OUTLINE_LEVEL_MASK );

        m_optionFlags = ( OptionFlags )iOptionFlags;
      }
    }

    /// <summary>
    /// Whether or not to collapse this row.
    /// </summary>
    public bool   IsCollapsed
    {
      get
      {
        return ( m_optionFlags & OptionFlags.Colapsed ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.Colapsed;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.Colapsed;
        }
      }
    }

    /// <summary>
    /// Whether or not to display this row with 0 height.
    /// </summary>
    public bool   IsHidden
    {
      get
      {
        return ( m_optionFlags & OptionFlags.ZeroHeight ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.ZeroHeight;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.ZeroHeight;
        }
      }
    }

    /// <summary>
    /// Whether the font and row height are not compatible.
    /// True if they aren't compatible.
    /// </summary>
    public bool   IsBadFontHeight
    {
      get
      {
        return ( m_optionFlags & OptionFlags.BadFontHeight ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.BadFontHeight;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.BadFontHeight;
        }
      }
    }

    /// <summary>
    /// Whether the row has been formatted (even if it has all blank cells).
    /// </summary>
    public bool   IsFormatted
    {
      get
      {
        return ( m_optionFlags & OptionFlags.Formatted ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.Formatted;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.Formatted;
        }
      }
    }
    /// <summary>
    /// True if there is additional space above the row.
    /// </summary>
    public bool   IsSpaceAboveRow
    {
      get
      {
        return ( m_optionFlags & OptionFlags.SpaceAbove ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.SpaceAbove;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.SpaceAbove;
        }
      }
    }
    /// <summary>
    /// True if there is additional space below the row.
    /// </summary>
    public bool   IsSpaceBelowRow
    {
      get
      {
        return ( m_optionFlags & OptionFlags.SpaceBelow ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.SpaceBelow;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.SpaceBelow;
        }
      }
    }
    /// <summary>
    /// Undocumented bit flag. If it is set to False, then Excel will
    /// not show row groups. Default value is True.
    /// </summary>
    public bool   IsGroupShown
    {
      get
      {
        return ( m_optionFlags & OptionFlags.ShowOutlineGroups ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.ShowOutlineGroups;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.ShowOutlineGroups;
        }
      }
    }
    /// <summary>
    /// Row or column index.
    /// </summary>
    ushort  IOutline.Index
    {
      get
      {
        throw new NotImplementedException();
        //return 0;//RowNumber;
      }
      set
      {
        throw new NotImplementedException();
        //RowNumber = value;
      }
    }

    #endregion
  }

  /// <summary>
  /// Represents enumerator for RowStorage
  /// </summary>
  public class RowStorageEnumerator : IEnumerator
  {
    #region IEnumerator Members
    /// <summary>
    /// RowStorage to enumerate.
    /// </summary>
    private RowStorage m_rowStorage;
    /// <summary>
    /// Offset to current cell.
    /// </summary>
    private int m_iOffset = -1;
    /// <summary>
    /// Record extractor.
    /// </summary>
    private RecordExtractor m_recordExtractor;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation instances of this class without arguments.
    /// </summary>
    private RowStorageEnumerator()
    {
    }
    /// <summary>
    /// Initializes new instance of the enumerator.
    /// </summary>
    /// <param name="row">RowStorage to enumerate.</param>
    /// <param name="recordExtractor">Record extractor to get Biff records from.</param>
    public RowStorageEnumerator( RowStorage row, RecordExtractor recordExtractor )
    {
      if( row == null )
        throw new ArgumentNullException( "row" );

      if( recordExtractor == null )
        throw new ArgumentNullException( "recordExtractor" );

      m_rowStorage = row;
      m_recordExtractor = recordExtractor;
    }
    #endregion

    #region IEnumerator Members
    /// <summary>
    /// Sets the enumerator to its initial position, which is before
    /// the first element in the collection.
    /// </summary>
    public void Reset()
    {
      m_iOffset = -1;
    }
    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
      bool bResult;

      if( m_rowStorage.UsedSize == 0 )
      {
        bResult = false;
      }
      else if( m_iOffset == -1 )
      {
        m_iOffset = 0;
        bResult = true;
      }
      else
      {
        int iNewOffset = m_rowStorage.MoveNextCell( m_iOffset );

        if( iNewOffset == m_rowStorage.UsedSize )
        {
          m_iOffset = -1;
          bResult = false;
        }
        else
        {
          m_iOffset = iNewOffset;
          bResult = true;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Gets the current element in the collection.
    /// </summary>
    public object Current
    {
      get
      {
        return m_recordExtractor.GetRecord( m_rowStorage.Provider, m_iOffset, m_rowStorage.Version );
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Returns formula array record from current cell if it exists.
    /// </summary>
    /// <returns>Formula array record; null - if doesn't exist.</returns>
    [ CLSCompliant( false ) ]
    public ArrayRecord GetArrayRecord()
    {
      if( m_iOffset == -1 )
        throw new InvalidOperationException( "Enumerator pointer is not set to an object instance" );

      return m_rowStorage.GetArrayRecordByOffset( m_iOffset );
    }
    /// <summary>
    /// Returns formula string value if formula contains it.
    /// </summary>
    /// <returns></returns>
    public string GetFormulaStringValue()
    {
      if( m_iOffset == -1 )
        throw new InvalidOperationException( "Enumerator pointer is not set to an object instance" );

      return m_rowStorage.GetFormulaStringValueByOffset( m_iOffset );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets current cell row.
    /// </summary>
    public int RowIndex
    {
      get
      {
        if( m_iOffset == -1 )
          throw new InvalidOperationException( "Enumerator pointer is not set to an object instance" );

        return m_rowStorage.GetRow( m_iOffset );
      }
    }
    /// <summary>
    /// Gets current cell column.
    /// </summary>
    public int ColumnIndex
    {
      get
      {
        if( m_iOffset == -1 )
          throw new InvalidOperationException( "Enumerator pointer is not set to an object instance" );

        return m_rowStorage.GetColumn( m_iOffset );
      }
    }
    /// <summary>
    /// Gets current cell extended format.
    /// </summary>
    public int XFIndex
    {
      get
      {
        if( m_iOffset == -1 )
          throw new InvalidOperationException( "Enumerator pointer is not set to an object instance" );

        return ( int )m_rowStorage.GetXFIndex( m_iOffset, false );
      }
    }
    #endregion
  }
}
