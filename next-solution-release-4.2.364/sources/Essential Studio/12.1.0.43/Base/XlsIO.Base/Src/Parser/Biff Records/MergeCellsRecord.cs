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
using Syncfusion.XlsIO.Implementation.Exceptions;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains all merged cell ranges of the current sheet.
  /// Optional record defining a square area of cells to "merge" into one cell.
  /// </summary>
  [ Biff( TBIFFRecord.MergeCells ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MergeCellsRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Maximum possible number of regions in the single MergeCells record.
    /// </summary>
    public const int DEF_MAXIMUM_REGIONS = 1027;
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 2;
    /// <summary>
    /// Subitem size.
    /// </summary>
    private const int DEF_SUB_ITEM_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Number of ranges.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usNumber = 0;
    /// <summary>
    /// All merged cell ranges of the current sheet.
    /// </summary>
    private MergedRegion[] m_arrRegions = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns number of ranges.
    /// </summary>
    public ushort RangesNumber
    {
      get
      {
        return m_usNumber;
      }
    }
    /// <summary>
    /// All merged cell ranges of the current sheet.
    /// </summary>
    public MergedRegion[] Regions
    {
      get
      {
        return m_arrRegions;
      }
      set
      {
        m_arrRegions = value;
        m_usNumber = (ushort)m_arrRegions.Length;
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
        return 2;
      }
    }
    #endregion

    #region Internal Classes
    /// <summary>
    /// This class contains information about the region of merged cells.
    /// </summary>
    [ CLSCompliant( false ) ]
    public class MergedRegion : ICloneable
    {
      #region Class members
      /// <summary>
      /// First row of the region.
      /// </summary>
      private int m_iRowFrom;
      /// <summary>
      /// Last row of the region.
      /// </summary>
      private int m_iRowTo;
      /// <summary>
      /// First column of the region.
      /// </summary>
      private int m_iColFrom;
      /// <summary>
      /// Last column of the region.
      /// </summary>
      private int m_iColTo;
      #endregion

      #region Class Initialize methods
      /// <summary>
      /// To prevent creation without parameters.
      /// </summary>
      private MergedRegion()
      {
      }
      /// <summary>
      /// Creates copy of region.
      /// </summary>
      /// <param name="region">Region to copy.</param>
      public MergedRegion( MergedRegion region )
        : this( region.RowFrom, region.RowTo, region.ColumnFrom, region.ColumnTo )
      {
      }
      /// <summary>
      /// Creates region by specified first and last rows and first and last columns.
      /// </summary>
      /// <param name="rowFrom">First row of the region.</param>
      /// <param name="rowTo">Last row of the region.</param>
      /// <param name="colFrom">First column of the region.</param>
      /// <param name="colTo">Last row of the region.</param>
      public MergedRegion( int rowFrom, int rowTo, int colFrom, int colTo )
      {
        m_iRowFrom = rowFrom;
        m_iRowTo = rowTo;
        m_iColFrom = colFrom;
        m_iColTo = colTo;
      }
      #endregion

      #region Class properties
      /// <summary>
      /// Read-only. First row of the region.
      /// </summary>
      public int RowFrom
      {
        get
        {
          return m_iRowFrom;
        }
          set
          {
              m_iRowFrom = value;
          }
      }
      /// <summary>
      /// Gets/sets last row of the region.
      /// </summary>
      public int RowTo
      {
        get
        {
          return m_iRowTo;
        }
        set
        {
          m_iRowTo = value;
        }
      }
      /// <summary>
      /// Read-only. First column of the region.
      /// </summary>
      public int ColumnFrom
      {
        get
        {
          return m_iColFrom;
        }
      }
      /// <summary>
      /// Gets/sets last column of the region.
      /// </summary>
      public int ColumnTo
      {
        get
        {
          return m_iColTo;
        }
        set
        {
          m_iColTo = value;
        }
      }
      /// <summary>
      /// Returns number of cells used by 
      /// </summary>
      public int CellsCount
      {
        get
        {
          return ( m_iRowTo - m_iRowFrom + 1 ) * ( m_iColTo - m_iColFrom + 1 );
        }
      }
      #endregion

      #region Class methods
      /// <summary>
      /// Moves region.
      /// </summary>
      /// <param name="iRowDelta">Row delta.</param>
      /// <param name="iColDelta">Column delta.</param>
      public void MoveRegion( int iRowDelta, int iColDelta )
      {
        m_iRowTo += iRowDelta;
        m_iRowFrom += iRowDelta;
        m_iColFrom += iColDelta;
        m_iColTo += iColDelta;
      }
      /// <summary>
      /// Converts region into Rectangle.
      /// </summary>
      /// <returns>Rectangle corresponding to this region.</returns>
      internal Rectangle GetRectangle()
      {
        return Rectangle.FromLTRB( m_iColFrom, m_iRowFrom, m_iColTo, m_iRowTo );
      }
      #endregion

      #region ICloneable Members
      /// <summary>
      /// Creates a copy of the current object.
      /// </summary>
      /// <returns>A copy of the current object.</returns>
      public object Clone()
      {
        return MemberwiseClone();
      }

      #endregion

      #region Class overrides
      /// <summary>
      /// Compares two merged regions.
      /// </summary>
      /// <param name="region1">First region to compare.</param>
      /// <param name="region2">Second region to compare.</param>
      /// <returns></returns>
      public static bool Equals( MergedRegion region1, MergedRegion region2 )
      {
        if( region1 == null && region2 == null )
          return true;

        if( region1 == null || region2 == null )
          return false;

        return region1.Equals( region2 );
      }
      /// <summary>
      /// Determines whether the specified object is equal to the current object.
      /// </summary>
      /// <param name="obj">The object to compare with the current object.</param>
      /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
      public override bool Equals( object obj )
      {
        if( obj == null )
          return false;

        MergedRegion region = obj as MergedRegion;

        if( region == null )
          throw new ArgumentException( "obj" );

        return m_iColFrom == region.m_iColFrom && m_iColTo == region.m_iColTo &&
          m_iRowFrom == region.m_iRowFrom && m_iRowTo == region.m_iRowTo;
      }
      /// <summary>
      /// Serves as a hash function for a MergedRegion object.
      /// </summary>
      /// <returns>A hash code for the current object.</returns>
      public override int GetHashCode()
      {
        return m_iColFrom.GetHashCode() | m_iColTo.GetHashCode() | m_iRowTo.GetHashCode() | m_iRowFrom.GetHashCode();
      }
      #endregion

    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  MergeCellsRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  MergeCellsRecord( Stream stream, out int itemSize )
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
    public  MergeCellsRecord( int iReserve )
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
      m_usNumber = provider.ReadUInt16( iOffset );
      iOffset += 2;
      m_arrRegions = new MergedRegion[ m_usNumber ];

      InternalDataIntegrityCheck();
      for( int i = 0; i < m_usNumber; i++, iOffset += 8 )
      {
        m_arrRegions[ i ] = new MergedRegion(
          provider.ReadUInt16( iOffset ),
          provider.ReadUInt16( iOffset + 2 ),
          provider.ReadUInt16( iOffset + 4 ),
          provider.ReadUInt16( iOffset + 6 ) );
      }
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
      provider.WriteUInt16( iOffset, m_usNumber );
      m_iLength = GetStoreSize( version );
      iOffset += 2;

      for( int i = 0; i < m_usNumber; i++, iOffset += 8 )
      {
          provider.WriteUInt16( iOffset,     ( ushort )m_arrRegions[ i ].RowFrom );
          provider.WriteUInt16( iOffset + 2, ( ushort )m_arrRegions[ i ].RowTo );
          provider.WriteUInt16( iOffset + 4, ( ushort )m_arrRegions[ i ].ColumnFrom );
          provider.WriteUInt16( iOffset + 6, ( ushort )m_arrRegions[ i ].ColumnTo );
      }
    }

    /// <summary>
    /// This method checks a record's internal data array for integrity.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    private void InternalDataIntegrityCheck()
    {
      if( m_iLength != m_usNumber * 8 + 2 || ( m_iLength - 2 ) % 8 != 0 )
        throw new WrongBiffRecordDataException( "MergeCellsRecord" );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_SIZE + m_arrRegions.Length * DEF_SUB_ITEM_SIZE;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Copies regions to the internal array.
    /// </summary>
    /// <param name="iStartIndex">First region to copy.</param>
    /// <param name="iCount">Regions count.</param>
    /// <param name="arrRegions">Array with regions to copy.</param>
    public void SetRegions( int iStartIndex, int iCount, MergedRegion[] arrRegions )
    {
      if( arrRegions == null )
        throw new ArgumentNullException( "arrRegions" );

      int iRegionsCount = arrRegions.Length;

      if( iStartIndex < 0 )
        throw new ArgumentOutOfRangeException( "iStartIndex" );

      if( iCount < 0 || iStartIndex + iCount > iRegionsCount )
        throw new ArgumentOutOfRangeException( "iRegionsCount" );

      if( m_usNumber != iCount )
      {
        m_arrRegions = new MergedRegion[ iCount ];
        m_usNumber = ( ushort )iCount;
      }

      Array.Copy( arrRegions, iStartIndex, m_arrRegions, 0, iCount );
    }
    #endregion
  }
}
