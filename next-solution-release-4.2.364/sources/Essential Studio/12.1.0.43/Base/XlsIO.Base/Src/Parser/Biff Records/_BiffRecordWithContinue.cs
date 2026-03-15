#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.IO;

using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for BiffRecordWithContinue.
  /// </summary>
  [ CLSCompliant( false ) ]
  public abstract class BiffRecordWithContinue
    : BiffRecordRawWithDataProvider
  {
    #region Class constants
    /// <summary>
    /// Mask to get two bytes value.
    /// </summary>
    private int DEF_WORD_MASK = ushort.MaxValue;
    #endregion

    #region Class members
    /// <summary>
    /// Array that contains positions of data of the continue records
    /// in the m_data array.
    /// </summary>
    internal List<int> m_arrContinuePos = new List<int>();
    /// <summary>
    /// Size of the first record length.
    /// </summary>
    protected int m_iFirstLength = -1;
    #endregion

    #region Class initialize/finilize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BiffRecordWithContinue()
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Type of the first continue record. Read-only.
    /// </summary>
    public virtual TBIFFRecord FirstContinueType
    {
      get
      {
        return TBIFFRecord.Continue;
      }
    }
    /// <summary>
    /// Indicates whether we should add header of continue records to the internal data provider. Read-only.
    /// </summary>
    protected virtual bool AddHeaderToProvider
    {
      get
      {
        return false;
      }
    }
    #endregion

    #region Class overrides
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
    /// If stream is not be big enough for data (end of stream
    /// reached) and all data were not read.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If record code is zero.
    /// </exception>
    public override int FillRecord( BinaryReader reader, DataProvider provider,
      IDecryptor decryptor, byte[] arrBuffer )
    {
      m_arrContinuePos = new List<int>();
//      m_extractor = new ContinueRecordExtractor( reader, decryptor );
//      return base.FillRecord( reader, provider, decryptor );
      Stream stream = reader.BaseStream;
      long lStartPosition = stream.Position;
      provider.Read( reader, 0, DEF_HEADER_SIZE, arrBuffer );

      // We have to extract first record completely or at least remember its settings.
      // And check whether there are any continue records.
      int iValue = provider.ReadInt32( 0 );
      m_iCode = iValue & DEF_WORD_MASK;
      iValue >>= 16;
      m_iLength = iValue & DEF_WORD_MASK;

      int iLength = m_iLength;
      int iCode = 0;
      int iTotalLength = 0;
      int iRecordCount = 0;
      int firstContinue = ( int )FirstContinueType;
      //m_arrContinuePos.Clear();

      do
      {
        if( iRecordCount > 0 && AddHeaderToProvider )
        {
          iTotalLength += DEF_HEADER_SIZE;
        }

        stream.Position += iLength;
        iTotalLength += iLength;
        iRecordCount++;
        m_arrContinuePos.Add( iTotalLength );

        provider.Read( reader, 0, DEF_HEADER_SIZE, arrBuffer );
        iValue = provider.ReadInt32( 0 );
        iCode = iValue & DEF_WORD_MASK;
        iValue >>= 16;
        iLength = iValue & DEF_WORD_MASK;
//        iCode = provider.ReadInt16( 0 );
//        iLength = provider.ReadInt16( 2 );
      }
      while( iCode == ( int )TBIFFRecord.Continue || iCode == firstContinue );

      //if( AddHeaderToProvider )
      //{
      //  iTotalLength += ( iRecordCount - 1 ) * DEF_HEADER_SIZE;
      //}

      m_provider.EnsureCapacity( iTotalLength );
      stream.Position = lStartPosition;
      int iCurrentPosition = 0;

      if( AddHeaderToProvider )
      {
        stream.Position += 4;
        m_provider.Read( reader, 0, iTotalLength, arrBuffer );

        if( decryptor != null )
        {
          int iStartPos = 0;
          long lStreamPos = lStartPosition + DEF_HEADER_SIZE;

          for( int i = 0; i < iRecordCount; i++ )
          {
            int iEndPos = m_arrContinuePos[ i ];
            int iDataSize = iEndPos - iStartPos;

            decryptor.Decrypt( m_provider, iStartPos, iDataSize, lStreamPos );

            lStreamPos += iDataSize + DEF_HEADER_SIZE;
            iStartPos = iEndPos + DEF_HEADER_SIZE;
            //iStartPos = iEndPos;
          }
        }
      }
      else
      {
        for( int i = 0; i < iRecordCount; i++ )
        {
          provider.Read( reader, 0, DEF_HEADER_SIZE, arrBuffer );
          iLength = provider.ReadInt16( 2 );
          m_provider.Read( reader, iCurrentPosition, iLength, arrBuffer, decryptor );
          iCurrentPosition += iLength;
        }
      }

      m_iLength = iTotalLength;
      ParseStructure();

      return ( int )( stream.Position - lStartPosition );
      //return m_iLength;
    }
    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Represents data provider.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record in the stream.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    /// If m_iLength of internal record data array is less than zero.
    /// </exception>
    public override int FillStream( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( NeedInfill )
      {
        InfillInternalData( ExcelVersion.Excel97to2003 );
      }

      if( m_iLength < 0 )
        throw new ApplicationException( "Wrong Record data infill." );

      writer.Write( ( ushort )m_iCode );
      streamPosition += 2;

      if( m_iFirstLength < 0 )
      {
        writer.Write( ( ushort )m_iLength );
      }
      else
      {
        writer.Write( ( ushort )m_iFirstLength );
      }

      streamPosition += 2;
//      if( m_data.Length < m_iLength )
//        throw new ApplicationException( "Length of data is greater than internal storage length." +
//          "Object Type is " + this.GetType().Name );

      if( encryptor != null )
      {
        int iOffset = StartDecodingOffset;

        if( m_arrContinuePos.Count > 0 )
        {
          int iCount = m_arrContinuePos.Count;

          if( !AddHeaderToProvider && m_arrContinuePos[ iCount - 1 ] != m_iLength )
          {
            m_arrContinuePos.Add( m_iLength );
            iCount++;
          }

          int iStartPos = StartDecodingOffset;

          for( int i = 0, len = m_arrContinuePos.Count; i < len; i++ )
          {
            int iEndPos = m_arrContinuePos[ i ];
            int iDataSize = iEndPos - iStartPos;

            encryptor.Encrypt( m_provider, iStartPos, iDataSize, streamPosition );

            streamPosition += iDataSize + DEF_HEADER_SIZE;
            iStartPos = iEndPos + DEF_HEADER_SIZE;
          }
        }
        else
        {
          encryptor.Encrypt( m_provider, iOffset, m_iLength - iOffset, streamPosition + iOffset );
        }
      }

      byte[] arrBuffer = ( ( ByteArrayDataProvider )provider ).InternalBuffer;
      m_provider.WriteInto( writer, 0, m_iLength, arrBuffer );
      
      if( !NeedDataArray ) m_provider.Clear();

      NeedInfill = true;
      //writer.Write( m_data, 0, m_iLength );
      return ( int )( m_iLength + 4 );
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public override object Clone()
    {
      BiffRecordWithContinue result = ( BiffRecordWithContinue )base.Clone();
      result.m_arrContinuePos = CloneUtils.CloneCloneable( m_arrContinuePos );

      if( m_provider != null )
      {
        result.m_provider = Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateDataProvider();
        result.m_provider.EnsureCapacity( m_provider.Capacity );
        m_provider.CopyTo( 0, result.m_provider, 0, m_provider.Capacity );
      }

      return result;
    }

    #endregion
  }
}
