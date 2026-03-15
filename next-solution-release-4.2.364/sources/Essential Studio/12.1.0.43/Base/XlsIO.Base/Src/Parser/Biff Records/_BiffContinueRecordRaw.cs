#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.IO;

using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// BifRecords container.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public abstract class BiffContinueRecordRaw : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    ///
    /// </summary>
    protected ContinueRecordExtractor m_extractor;
    /// <summary>
    /// 
    /// </summary>
    private ContinueRecordBuilder   m_builder;
    /// <summary>
    /// Array that contains positions of data of the continue records
    /// in the m_data array.
    /// </summary>
    protected internal List<int> m_arrContinuePos = new List<int>();
    /// <summary>
    ///
    /// </summary>
    private int m_iIntLen = -1;
//    /// <summary>
//    /// Object that should be used to parse encrypted records.
//    /// </summary>
//    private IDecryptor m_decryptor;
    #endregion

    #region Class Properties
    /// <summary>
    /// 
    /// </summary>
    protected ContinueRecordBuilder Builder
    {
      get
      {
        if( m_builder == null )
          throw new ArgumentNullException( "Builder", "Class does not call parent method InfillInternalData." );

        return m_builder;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    protected  BiffContinueRecordRaw()
      : base()
    {
    }

    /// <summary>
    /// Read / Initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    protected  BiffContinueRecordRaw( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    protected  BiffContinueRecordRaw( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// This method constitutes the main part of parsing with continue records.
    /// For inheritors this method must be called to get Continue records
    /// which is placed after this record in the input stream.
    /// </summary>
    public override void ParseStructure()
    {
      // Extract after that all continue records and attach them to 
      // main data buffer.
      ExtractContinueRecords();
//      if( ExtractContinueRecords( out arrData ) )
//      {
//        // Create new buffer with attached Continue record.
//        int iLength = m_data.Length;
//        int iCount = arrData.Length;
//        byte[] newArray = new byte[ iLength + iCount ];
//        Buffer.BlockCopy( m_data, 0, newArray, 0, iLength );
//        data.CopyTo( 0, newArray, iLength, iCount );
//
//        // Replace old buffer by new with attached continue record.
//        m_data = newArray;
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      AutoGrowData = true;
      m_builder = CreateBuilder();
    }

    /// <summary>
    /// Creates continue record builder.
    /// </summary>
    /// <returns>Created builder.</returns>
    protected virtual ContinueRecordBuilder CreateBuilder()
    {
      ContinueRecordBuilder builder = new ContinueRecordBuilder( this );
      builder.OnFirstContinue += new EventHandler( builder_OnFirstContinue );
      return builder;
    }

    /// <summary>
    /// Read from stream record data.
    /// </summary>
    /// <param name="reader">Stream with record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="decryptor">Object that should be used to parse encrypted records.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Size of the record data.</returns>
    /// <exception cref="System.ArgumentNullException">If reader is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    /// If stream is not big enough for data (end of stream
    /// reached and all data were not read).
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If record code is zero.
    /// </exception>
    public override int FillRecord( BinaryReader reader, DataProvider provider,
      IDecryptor decryptor, byte[] arrBuffer )
    {
      m_arrContinuePos = new List<int>();
      m_extractor = new ContinueRecordExtractor( reader, decryptor, provider );
      return base.FillRecord( reader, provider, decryptor, arrBuffer );
    }

    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream, used to reduce Flush
    /// calls of the writer.BaseStream.</param>
    /// <returns>Size of the record in the stream.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    /// If m_iLength of internal record data array is less than zero.
    /// </exception>
    public override int FillStream( BinaryWriter writer, IEncryptor encryptor, int streamPosition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      InfillInternalData( ExcelVersion.Excel97to2003 );

      if( m_iLength < 0 )
        throw new ApplicationException( "Wrong Record data infill." );

      writer.Write( ( ushort )m_iCode );

      if( m_iIntLen < 0 )
      {
        writer.Write( ( ushort )m_iLength );
      }
      else
      {
        writer.Write( ( ushort )m_iIntLen );
      }

      streamPosition += DEF_HEADER_SIZE;

      if( m_data.Length < m_iLength )
        throw new ApplicationException( "Length of data is greater than internal storage length." +
          "Object Type is " + this.GetType().Name );

      if( encryptor != null )
      {
        int iOffset = StartDecodingOffset;
        int iContinueCount = m_arrContinuePos.Count;
        ByteArrayDataProvider provider = new ByteArrayDataProvider( m_data );

        if( iContinueCount > 0 && !m_arrContinuePos.Contains(m_iLength))
        {            
           m_arrContinuePos.Add(m_iLength);
           iContinueCount++;
            
          int iStartPos = StartDecodingOffset;
          //long lStreamPos = writer.BaseStream.Position;

          for( int i = 0; i < iContinueCount; i++ )
          {
            int iEndPos = m_arrContinuePos[ i ];
            int iDataSize = iEndPos - iStartPos;

            encryptor.Encrypt( provider, iStartPos, iDataSize, streamPosition );

            streamPosition += iDataSize + DEF_HEADER_SIZE;
            iStartPos = iEndPos + DEF_HEADER_SIZE;
          }
        }
        else
        {
          encryptor.Encrypt( provider, iOffset, m_iLength - iOffset, streamPosition + iOffset );
        }
      }

      writer.Write( m_data, 0, m_iLength );
      return ( int )( m_iLength + 4 );
    }
    /// <summary>
    /// Extracts all continue records from the stream if needed and updates internal data array.
    /// </summary>
    /// <returns>True if there was extracted any continue record.</returns>
    protected virtual bool ExtractContinueRecords()
    {
      if( m_extractor == null )
        throw new ArgumentNullException( "m_extractor" );

      // Store stream position as a start point of parsing.
      m_extractor.StoreStreamPosition();

      int iLastPos = m_data.Length;

      m_arrContinuePos.Clear();
      m_arrContinuePos.Add( iLastPos );

      //data = new BytesList( true );
      ( ( IEnumerator )m_extractor ).Reset();

      int iFullLength;
      byte[] arrData;
      List<byte[]> arrRecords = CollectRecordsData( out iFullLength, ref iLastPos );
      int iCount = arrRecords.Count;

      if( iCount > 0 )
      {
        arrData = new byte[ iFullLength + m_iLength ];
        Buffer.BlockCopy( m_data, 0, arrData, 0, m_iLength );
        int iOffset = m_iLength;

        for( int i = 0; i < iCount; i++ )
        {
          byte[] arrCurrentData = arrRecords[ i ];
          int iLength = arrCurrentData.Length;

          Buffer.BlockCopy( arrCurrentData, 0, arrData, iOffset, iLength );
          iOffset += iLength;
        }

        m_data = arrData;
      }

      return ( iCount > 0 );
    }

    /// <summary>
    /// Returns List with byte arrays with continue record's data.
    /// </summary>
    /// <param name="iFullLength">Full length of the additional data.</param>
    /// <param name="iLastPos">Last position in the stream.</param>
    /// <returns>List with byte arrays with continue record's data.</returns>
    protected List<byte[]> CollectRecordsData( out int iFullLength, ref int iLastPos )
    {
      ( ( IEnumerator )m_extractor ).Reset();

      List<byte[]> arrRecords = new List<byte[]>();
      iFullLength = 0;

      while( ( ( IEnumerator )m_extractor ).MoveNext() )
      {
        int iLength = AddRecordData( arrRecords, m_extractor.Current );
        iLastPos += iLength;
        iFullLength += iLength;

        m_arrContinuePos.Add( iLastPos );
      }

      return arrRecords;
    }
    /// <summary>
    /// Adds record data from single record to the List.
    /// </summary>
    /// <param name="arrRecords">List to add data to.</param>
    /// <param name="record">Record to add data from.</param>
    /// <returns>Size of the added data.</returns>
    protected virtual int AddRecordData( List<byte[]> arrRecords, BiffRecordRaw record )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      byte[] arrData = record.Data;
      arrRecords.Add( arrData );

      return arrData.Length;
    }
    /// <summary>
    /// OnFirstContinue event handler.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void builder_OnFirstContinue( object sender, EventArgs e )
    {
      ContinueRecordBuilder builder = ( ContinueRecordBuilder )sender;
      builder.OnFirstContinue -= new EventHandler( builder_OnFirstContinue );

      m_iIntLen = builder.Position;
    }
    #endregion

    #region Class helper functions
    /// <summary>
    /// 
    /// </summary>
    /// <param name="recordType"></param>
    protected void AddContinueRecordType( TBIFFRecord recordType )
    {
      m_extractor.AddRecordType( recordType );
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public override object Clone()
    {
      BiffContinueRecordRaw result = ( BiffContinueRecordRaw )base.Clone();
      result.m_arrContinuePos = CloneUtils.CloneCloneable( m_arrContinuePos );

      return result;
    }

    #endregion
  }
}
