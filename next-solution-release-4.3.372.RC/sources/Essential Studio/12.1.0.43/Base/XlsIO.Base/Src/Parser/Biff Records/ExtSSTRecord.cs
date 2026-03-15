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

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  ///<exclude/>
  /// <summary>
  /// It is used by Excel to create a hash table with stream
  /// offsets to the SST record to optimize string search operations. Excel
  /// may not shorten this record if strings are deleted from the shared
  /// string table, so the last part might contain invalid data. The stream
  /// indexes in this record divide the SST into portions containing a
  /// constant number of strings.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.ExtSST ) ]
  //[ BiffOffsetsRecords( TBIFFRecord.SST ) ]
  [ CLSCompliant( false ) ]
  public class ExtSSTRecord  : BiffRecordRawWithArray
  {
    #region Class constants
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
    /// Number of strings in a portion, this number is >=8.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usStringPerBucket = 8;
    /// <summary>
    /// Array that contains all portions.
    /// </summary>
    private ExtSSTInfoSubRecord[] m_arrSSTInfo = null;
    /// <summary>
    /// Indicates and of workbook in some cases.
    /// </summary>
    private bool m_bIsEnd;
    /// <summary>
    /// Reference to the SST record.
    /// </summary>
    private SSTRecord m_sst;
    #endregion

    #region Class properties
    /// <summary>
    /// Number of strings in a portion, this number is >=8.
    /// </summary>
    public ushort StringPerBucket
    {
      get
      {
        return m_usStringPerBucket;
      }
      set
      {
        m_usStringPerBucket = value;
      }
    }

    /// <summary>
    /// Array that contains all portions.
    /// </summary>
    public ExtSSTInfoSubRecord[] SSTInfo
    {
      get
      {
        return m_arrSSTInfo;
      }
      set
      {
        m_arrSSTInfo = value;
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
        return 0;
      }
    }
    /// <summary>
    /// Indicates and of workbook in some cases. Read-only.
    /// </summary>
    public bool IsEnd
    {
      get
      {
        return m_bIsEnd;
      }
    }
    /// <summary>
    /// Gets / sets reference to the SST record.
    /// </summary>
    public SSTRecord SST
    {
      get
      {
        return m_sst;
      }
      set
      {
        m_sst = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  ExtSSTRecord()
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
    public  ExtSSTRecord( Stream stream, out int itemSize )
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
    public  ExtSSTRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns></returns>
    public override int FillStream( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      UpdateStringOffsets();
      return base.FillStream( writer, provider, encryptor, streamPosition );
    }
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_usStringPerBucket = GetUInt16( 0 );

      int iPortionsCount = ( m_iLength - 2 ) / 8;
      int iLength = ( m_iLength - 2 );

      if ((iLength) % 8 != 0)
      {
          if (iLength % 4 != 0)
          {
              int iValue = GetInt32(m_iLength - 4);
              m_bIsEnd = iValue == (int)TBIFFRecord.EOF;
              if(m_bIsEnd)
                    throw new WrongBiffRecordDataException("ExtSSTRecord's data size minus 2 must be divided by 8.");
          }
          else
          {
              int iValue = GetInt32(m_iLength - 4);
              m_bIsEnd = iValue == (int)TBIFFRecord.EOF;
          }
      }
      m_arrSSTInfo = new ExtSSTInfoSubRecord[ iPortionsCount ];

      int iOffset = 2;
      using( ByteArrayDataProvider dataProvider = new ByteArrayDataProvider( m_data ) )
      {
        for( int i = 0; i < iPortionsCount; i++, iOffset += 8 )
        {
          ExtSSTInfoSubRecord infoSubRecord = ( ExtSSTInfoSubRecord )BiffRecordFactory.GetRecord(
            TBIFFRecord.ExtSSTInfoSub );

          infoSubRecord.StreamPos = this.StreamPos + iOffset;
          //infoSubRecord.m_data = GetBytes( iOffset, 8 );
          infoSubRecord.ParseStructure( dataProvider, iOffset, 8, ExcelVersion.Excel97to2003 );
          m_arrSSTInfo[ i ] = infoSubRecord;
        }
      }
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_data = new byte[ GetStoreSize( ExcelVersion.Excel97to2003 ) ];
      SetUInt16( 0, m_usStringPerBucket );
      m_iLength = 2;

      if( m_arrSSTInfo != null )
      {
        for( int i = 0, len = m_arrSSTInfo.Length; i < len; i++, m_iLength += 8 )
        {
          m_arrSSTInfo[ i ].StreamPos = m_iLength;
          SetBytes( m_iLength, m_arrSSTInfo[ i ].Data, 0, 8 );
        }
      }
    }
    /// <summary>
    /// Method updates fields of record which must contain stream offset
    /// or other data. This method must be called before save operation
    /// when all records placed in array in its positions and offsets can
    /// be freely calculated.
    /// </summary>
    public void UpdateStringOffsets()
    {
      int position = ( int )m_sst.StreamPos;
      int iCount = ( int )m_sst.NumberOfUniqueStrings;

      if( iCount > 0 ) // if SST not empty
      {
        int[] arrStringsOffsets = m_sst.StringsOffsets;
        int[] arrStringsStreamPos = m_sst.StringsStreamPos;

        for( int i = 0, iPos = 0; i < iCount; i += StringPerBucket, iPos++ )
        {
          int offset = arrStringsOffsets[ i ];
          ExtSSTInfoSubRecord subRecord = m_arrSSTInfo[ iPos ];
          subRecord.StreamPosition = position + arrStringsStreamPos[ i ];
          subRecord.BucketSSTOffset = ( ushort )offset;
        }
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iLength = ( m_arrSSTInfo != null )
        ? m_arrSSTInfo.Length
        : 0;

      return DEF_FIXED_SIZE + iLength * DEF_SUB_ITEM_SIZE;
    }
    #endregion
  }
}