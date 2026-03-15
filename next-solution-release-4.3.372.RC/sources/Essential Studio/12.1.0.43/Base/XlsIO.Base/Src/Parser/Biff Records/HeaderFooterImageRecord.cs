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
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for HeaderFooterImageRecord.
  /// </summary>
  [ Biff( TBIFFRecord.HeaderFooterImage ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class HeaderFooterImageRecord
    : MSODrawingGroupRecord
    , ILengthSetter
  {
    #region Class constants
    /// <summary>
    /// Record header in workbook part.
    /// </summary>
    internal static readonly byte[] DEF_RECORD_START = new byte[]
    {
      0x66, 0x08, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
    };
    /// <summary>
    /// Record header in worksheet part.
    /// </summary>
    internal static readonly byte[] DEF_WORKSHEET_RECORD_START = new byte[]
    {
      0x66, 0x08, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
    };
    /// <summary>
    /// Record header in workbook when it is not first record
    /// (when it is used instead of Continue record).
    /// </summary>
    internal static readonly byte[] DEF_CONTINUE_START = new byte[]
    {
      0x66, 0x08, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0,
    };
    /// <summary>
    /// Data offset.
    /// </summary>
    internal static readonly int DEF_DATA_OFFSET = DEF_RECORD_START.Length;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  HeaderFooterImageRecord()
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
    public  HeaderFooterImageRecord( Stream stream, out int itemSize )
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
    public  HeaderFooterImageRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Offset to the structures data.
    /// </summary>
    protected override int StructuresOffset
    {
      get
      {
        return DEF_DATA_OFFSET;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates List for all data.
    /// </summary>
    /// <param name="iStartIndex">First free index in the resulting List.</param>
    /// <returns>Created List.</returns>
    protected override Stream CreateDataList( out int iStartIndex )
    {
      int iCount = m_arrStructures.Count;
      iStartIndex = 1;
      MemoryStream result = new MemoryStream();
      result.Write( DEF_RECORD_START, 0, DEF_RECORD_START.Length );
      return result;
    }
    /// <summary>
    /// Adds record data from single record to the List.
    /// </summary>
    /// <param name="arrRecords">List to add data to.</param>
    /// <param name="record">Record to add data from.</param>
    /// <returns>Size of the added data.</returns>
    protected override int AddRecordData( List<byte[]> arrRecords, BiffRecordRaw record )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      byte[] arrData = record.Data;
      // Here we have to skip header of HeaderFooterImageRecord.
      int iLength = arrData.Length;
      int iNewLength = iLength - DEF_DATA_OFFSET;

      if( iNewLength > 0 )
      {
        byte[] arrNewData = new byte[ iNewLength ];
        iLength = iNewLength;
        Buffer.BlockCopy( arrData, DEF_DATA_OFFSET, arrNewData, 0, iNewLength );
        arrData = arrNewData;
      }

      arrRecords.Add( arrData );
      return iLength;
    }
    /// <summary>
    /// Creates continue record builder.
    /// </summary>
    /// <returns>Created builder.</returns>
    protected override ContinueRecordBuilder CreateBuilder()
    {
      ContinueRecordBuilder builder = new HeaderContinueRecordBuilder( this );
      builder.OnFirstContinue += new EventHandler( builder_OnFirstContinue );
      return builder;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets length of the internal data.
    /// </summary>
    /// <param name="iLength">Length to set.</param>
    public void SetLength( int iLength )
    {
      m_iLength = iLength;
    }
    #endregion
  }

  /// <summary>
  /// Special class for building Continue Records.
  /// </summary>
  [ CLSCompliant( false ) ]
  public class HeaderContinueRecordBuilder : ContinueRecordBuilder
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public HeaderContinueRecordBuilder( BiffContinueRecordRaw parent )
      : base( parent )
    {
      ContinueType = TBIFFRecord.HeaderFooterImage;
      FirstContinueType = TBIFFRecord.HeaderFooterImage;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Maximum size of the continue record data.
    /// </summary>
    public override int MaximumSize
    {
      get
      {
        return BiffRecordRaw.DEF_RECORD_MAX_SIZE - HeaderFooterImageRecord.DEF_DATA_OFFSET;
      }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Write array of data into output stream.
    /// </summary>
    /// <param name="data">Array of data.</param>
    /// <param name="start">Start index of an array.</param>
    /// <param name="length">Length of data to copy.</param>
    /// <returns>Quantity of created Continue Records.</returns>
    public override int AppendBytes( byte[] data, int start, int length )
    {
      int counter = 0;

      // If data array is too large, then save it by parts.
      if( CheckIfSpaceNeeded( length ) )
      {
        int endPoint = start + length;
        int i = start;

        for( ; i < endPoint; i += m_iMax )
        {
          UpdateContinueRecordSize();
          StartContinueRecord();
          counter++;

          m_parent.SetBytes( m_iPos, HeaderFooterImageRecord.DEF_CONTINUE_START,
            0, HeaderFooterImageRecord.DEF_DATA_OFFSET );
          UpdateCounters( HeaderFooterImageRecord.DEF_DATA_OFFSET );

          int iLen = ( endPoint - i < m_iMax ) ? endPoint - i : m_iMax;
          m_parent.SetBytes( m_iPos, data, i, iLen );
          UpdateCounters( iLen );
        }
      }
      else
      {
        m_parent.SetBytes( m_iPos, data, start, length );
        UpdateCounters( length );
      }

      UpdateContinueRecordSize();

      return counter;
    }

    #endregion

    #region Class Helper Methods
//    /// <summary>
//    /// Method that checks if Continue Record is needed.
//    /// </summary>
//    /// <param name="length">Length of data that needs to be stored.</param>
//    /// <returns>True if Continue Record will be needed for data storage;
//    /// otherwise False.</returns>
//    public bool CheckIfSpaceNeeded( int length )
//    {
//      return ( m_iContinueSize + length > m_iMax );
//    }
    #endregion
  }

}
