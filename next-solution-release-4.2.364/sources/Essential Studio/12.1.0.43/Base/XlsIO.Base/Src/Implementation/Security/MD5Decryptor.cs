#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.IO;

//using Syncfusion.XlsIO.IO.Stream.Win32;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation.Security
{
  /// <summary>
  /// Represents an MD5Decryptor.
  /// </summary>
  [CLSCompliant( false )]
  public class MD5Decryptor
    : IDecryptor
    , IEncryptor
  {
    #region Class constants
    private const int DEF_READ_LENGTH = 16;
    private const int DEF_PAS_LEN = 64;
    private const int DEF_BLOCK_SIZE = 1024;
    private const int DEF_START_POS = 0;
    private const int DEF_INC_BYTE_MAXVAL = 256;
    #endregion

    #region Class members
    /// <summary>
    ///
    /// </summary>
    private byte[] m_baDocumentID;// = new byte[ DEF_READ_LENGTH ];
    /// <summary>
    ///
    /// </summary>
    private byte[] m_baPoint;// = new byte[ DEF_PAS_LEN ];
    /// <summary>
    ///
    /// </summary>
    private byte[] m_baHash;// = new byte[ DEF_READ_LENGTH ];
    /// <summary>
    ///
    /// </summary>
    private byte[] m_baPassword = new byte[ DEF_PAS_LEN ];
    /// <summary>
    ///
    /// </summary>
    private MD5Context m_valContext;
    /// <summary>
    /// Last stream position.
    /// </summary>
    private long m_lLastStreamPosition = 0;
    /// <summary>
    /// Data provider used to encrypt/decrypt byte arrays.
    /// </summary>
    private ByteArrayDataProvider m_provider;
//    /// <summary>
//    /// 
//    /// </summary>
//    private WordKey m_key = null;
//    /// <summary>
//    /// 
//    /// </summary>
//    private int m_iLastBlock = -1;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MD5Decryptor"/> class.
    /// </summary>
    public MD5Decryptor()
    {
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Checks the password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public bool CheckPassword( string password )
    {
      PreparePassword( password );
      bool isCorrectPass = VerifyPassword();
      return isCorrectPass;
    }
    /// <summary>
    /// Decrypts the stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns></returns>
    public MemoryStream Decrypt( Stream stream )
    {
      if( stream == null ) return null;

      MemoryStream retStream = new MemoryStream();
      byte[] temp = new byte[ DEF_READ_LENGTH ];
      ByteArrayDataProvider provider = new ByteArrayDataProvider( temp );
      long lEnd = stream.Length;

      if( lEnd == 0 ) return retStream;

      long j = stream.Position;
      uint uiBlockIndex = 0;
      WordKey key = new WordKey();
      MakeKey( key, uiBlockIndex, m_valContext );

      while( j < lEnd )
      {
        int count = stream.Read( temp, 0, DEF_READ_LENGTH );

        for( int i = count; i < DEF_READ_LENGTH; i++ )
          temp[ i ] = 1;

        DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );
        retStream.Write( temp, 0, DEF_READ_LENGTH );
        j += DEF_READ_LENGTH;

        if( j % DEF_BLOCK_SIZE == 0 )
        {
          uiBlockIndex++;
          MakeKey( key, uiBlockIndex, m_valContext );
        }

      }

      retStream.Position = 0;
      return retStream;
    }
    /// <summary>
    /// Decrypts data provider and writes data back into it.
    /// </summary>
    /// <param name="provider">Provider to decrypt.</param>
    /// <param name="offset">Offset to the data.</param>
    /// <param name="length">Size of the data to decrypt.</param>
    /// <param name="streamPos">Record position in the stream position.</param>
    public void Decrypt( DataProvider provider, int offset, int length, long streamPos )
    {
      if( provider == null ) return;

      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length" );

      CheckPrepared();

      int iEnd = offset + length;
      long iCurrentBlocksCount = streamPos / DEF_BLOCK_SIZE;
      long iCurrentBlockStart = iCurrentBlocksCount * DEF_BLOCK_SIZE;
      int iCurrentBlockRemaining = ( int )( DEF_BLOCK_SIZE - streamPos + iCurrentBlockStart );

      int iCurPos = offset;
      int iBytesToWrite = Math.Min( iCurrentBlockRemaining, length );

      //uint uiBlockIndex = 0;
      //uint uiBlockIndex = ( uint )iStartBlockIndex;
      while( iBytesToWrite > 0 )
      {
        WordKey key = PrepareKey( streamPos );
        //PrepareKey( streamPos );
        //WordKey key = new WordKey();
        //MakeKey( key, uiBlockIndex, m_valContext );
        DataProvider curProvider = provider;

        DecryptBuffer( curProvider, iCurPos, iBytesToWrite, key );

        iCurPos += iBytesToWrite;
        length -= iBytesToWrite;
        streamPos += iBytesToWrite;

        iBytesToWrite = Math.Min( DEF_BLOCK_SIZE, length );
      }
    }

    /// <summary>
    /// Decrypts buffer and puts resulting data back into it.
    /// </summary>
    /// <param name="buffer">Buffer to decrypt.</param>
    /// <param name="offset">Offset to the data.</param>
    /// <param name="length">Size of the data to decrypt.</param>
    public void Decrypt( byte[] buffer, int offset, int length )
    {
      throw new NotImplementedException();
      //      if( buffer == null ) return;
      //
      //      int iEnd = buffer.Length;
      //
      //      if( iEnd == 0 ) return;
      //
      //      if( offset < 0 )
      //        throw new ArgumentOutOfRangeException( "offset" );
      //
      //      if( length < 0 )
      //        throw new ArgumentOutOfRangeException( "length" );
      //
      //      CheckPrepared();
      //
      //      iEnd = Math.Min( iEnd, offset + length );
      //
      //      byte[] temp = new byte[ DEF_READ_LENGTH ];
      //      uint uiBlockIndex = 0;
      //      WordKey key = new WordKey();
      //      MakeKey( key, uiBlockIndex, m_valContext );
      //      int iCurPos = offset;
      //      byte[] arrBuffer = buffer;
      //
      //      while( iCurPos < iEnd )
      //      {
      //        int iBytesLeft = iEnd - iCurPos;
      //        bool bUseTemp = ( iBytesLeft < DEF_READ_LENGTH );
      //        int iDecryptOffset = iCurPos;
      //
      //        if( bUseTemp )
      //        {
      //          Buffer.BlockCopy( buffer, iCurPos, temp, 0, iBytesLeft );
      //          SetByte( temp, iBytesLeft, DEF_READ_LENGTH, 1 );
      //          arrBuffer = temp;
      //          iDecryptOffset = 0;
      //        }
      //
      //        DecryptBuffer( arrBuffer, iDecryptOffset, DEF_READ_LENGTH, key );
      //
      //        if( bUseTemp )
      //        {
      //          Buffer.BlockCopy( temp, iDecryptOffset, buffer, iCurPos, iBytesLeft );
      //        }
      //
      //        iCurPos += DEF_READ_LENGTH;
      //
      //        if( iCurPos % DEF_BLOCK_SIZE == 0 )
      //        {
      //          uiBlockIndex++;
      //          MakeKey( key, uiBlockIndex, m_valContext );
      //        }
      //      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Prepares password.
    /// </summary>
    /// <param name="password">Password to convert.</param>
    private void PreparePassword( string password )
    {
      int i = 0;
      int iLength = password.Length;

      while( i < DEF_READ_LENGTH && i < iLength )
      {
        ushort usValue = ( ushort )password[ i ];
        m_baPassword[ 2 * i ] = ( byte )( usValue & Byte.MaxValue );
        m_baPassword[ 2 * i + 1 ] = ( byte )( ( usValue >> 8 ) & Byte.MaxValue );
        i++;
      }

      m_baPassword[ 2 * i ] = 0x80;
      m_baPassword[ 56 ] = ( byte )( i << 4 );
    }
    /// <summary>
    /// Swaps a and b.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">b.</param>
    private void Swap( ref byte a, ref byte b )
    {
      byte temp = a;
      a = b;
      b = temp;
    }

    /// <summary>
    /// Prepares the key.
    /// </summary>
    /// <param name="key">Key to prepare.</param>
    /// <param name="data">The data.</param>
    /// <param name="length">The length.</param>
    private void PrepareKey( WordKey key, byte[] data, byte length )
    {
      byte index1 = 0;
      byte index2 = 0;
      byte[] state = key.Status;

      for( int i = 0; i < DEF_INC_BYTE_MAXVAL; i++ )
        state[ i ] = ( byte )i;

      for( int i = 0; i < DEF_INC_BYTE_MAXVAL; i++ )
      {
        index2 = ( byte )( ( data[ index1 ] + state[ i ] + index2 ) % DEF_INC_BYTE_MAXVAL );
        Swap( ref state[ i ], ref state[ index2 ] );
        index1 = ( byte )( ( index1 + 1 ) % length );
      }
    }

    /// <summary>
    /// Makes the key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="block">The block.</param>
    /// <param name="valContext">The val context.</param>
    private void MakeKey( WordKey key, uint block, MD5Context valContext )
    {
      MD5Context mdContext = new MD5Context();
      byte[] pwarray = new byte[ DEF_PAS_LEN ];
      //API.CopyMemory(pwarray, valContext.Digest, 5);
      Buffer.BlockCopy( valContext.Digest, 0, pwarray, 0, 5 );

      pwarray[ 5 ] = ( byte )( block & Byte.MaxValue );
      pwarray[ 6 ] = ( byte )( ( block >> 8 ) & Byte.MaxValue );
      pwarray[ 7 ] = ( byte )( ( block >> 16 ) & Byte.MaxValue );
      pwarray[ 8 ] = ( byte )( ( block >> 24 ) & Byte.MaxValue );

      pwarray[ 9 ] = 0x80;
      pwarray[ 56 ] = 0x48;
      mdContext.Update( pwarray, DEF_PAS_LEN );
      mdContext.StoreDigest();

      PrepareKey( key, mdContext.Digest, DEF_READ_LENGTH );
    }

    /// <summary>
    /// Compares memory blocks
    /// </summary>
    /// <param name="block1">The block1.</param>
    /// <param name="block2">The block2.</param>
    /// <param name="length">The length.</param>
    /// <returns></returns>
    private bool CompareMemory( byte[] block1, byte[] block2, int length )
    {
      for( int i = 0; i < length; i++ )
      {
        if( block1[ i ] != block2[ i ] )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Verifies the password.
    /// </summary>
    /// <returns>True if password was verified.</returns>
    private bool VerifyPassword()
    {
      PrepareValContext();

      WordKey key = new WordKey();
      MakeKey( key, 0, m_valContext );

      ByteArrayDataProvider provider = new ByteArrayDataProvider( m_baPoint );
      DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );

      provider.SetBuffer( m_baHash );
      DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );

      m_baPoint[ 16 ] = 0x80;
      SetByte( m_baPoint, 17, 47, 0 );
      m_baPoint[ 56 ] = 0x80;
      MD5Context mdContext2 = new MD5Context();
      mdContext2.Update( m_baPoint, DEF_PAS_LEN );
      mdContext2.StoreDigest();

      return CompareMemory( mdContext2.Digest, m_baHash, DEF_READ_LENGTH );
    }
    /// <summary>
    /// 
    /// </summary>
    private void PrepareValContext()
    {
      MD5Context mdContext = new MD5Context();
      mdContext.Update( m_baPassword, DEF_PAS_LEN );
      mdContext.StoreDigest();

      m_valContext = new MD5Context();
      int iOffset = 0;
      int iKeyOffset = 0;
      int iCopySize = 5;

      while( iOffset != DEF_READ_LENGTH )
      {
        if( DEF_PAS_LEN - iOffset < 5 )
        {
          iCopySize = DEF_PAS_LEN - iOffset;
        }

        Buffer.BlockCopy( mdContext.Digest, iKeyOffset, m_baPassword, iOffset, iCopySize );
        iOffset += iCopySize;

        if( iOffset == DEF_PAS_LEN )
        {
          m_valContext.Update( m_baPassword, DEF_PAS_LEN );
          iKeyOffset = iCopySize;
          iCopySize = 5 - iCopySize;
          iOffset = 0;
          continue;
        }

        iKeyOffset = 0;
        iCopySize = 5;
        Buffer.BlockCopy( m_baDocumentID, 0, m_baPassword, iOffset, DEF_READ_LENGTH );
        iOffset += DEF_READ_LENGTH;
      }

      m_baPassword[ 16 ] = 0x80;
      Array.Clear( m_baPassword, 17, 47 );
      m_baPassword[ 56 ] = 0x80;
      m_baPassword[ 57 ] = 0x0A;
      m_valContext.Update( m_baPassword, DEF_PAS_LEN );
      m_valContext.StoreDigest();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    /// <param name="iOffset"></param>
    /// <param name="iLength"></param>
    /// <param name="btValue"></param>
    private static void SetByte( byte[] arrData, int iOffset, int iLength, byte btValue )
    {
      if( arrData == null )
        throw new ArgumentNullException();

      for( int i = iOffset; i < iLength; i++ )
      {
        arrData[ i ] = btValue;
      }
    }
    /// <summary>
    /// Decrypts buffer
    /// </summary>
    /// <param name="provider">An object that gives access to the data storage.</param>
    /// <param name="startOffset">Start offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="key">The key.</param>
    private void DecryptBuffer( DataProvider provider, int startOffset, int length, WordKey key )
    {
      byte x = key.X;
      byte y = key.Y;
      byte[] state = key.Status;
      byte xorIndex;

      for( int i = 0; i < length; i++, startOffset++ )
      {
        x = ( byte )( ( x + 1 ) % DEF_INC_BYTE_MAXVAL );
        y = ( byte )( ( state[ x ] + y ) % DEF_INC_BYTE_MAXVAL );
        Swap( ref state[ x ], ref state[ y ] );
        xorIndex = ( byte )( ( state[ x ] + state[ y ] ) % DEF_INC_BYTE_MAXVAL );
        //data[ startOffset ] ^= state[ xorIndex ];

        if( provider != null )
        {
          byte btCurValue = provider.ReadByte( startOffset );
          btCurValue ^= state[ xorIndex ];
          provider.WriteByte( startOffset, btCurValue );
        }
      }

      key.Status = state;
      key.X = x;
      key.Y = y;
    }

    /// <summary>
    /// Checks whether decryption information was prepared.
    /// Throws ApplicationExcepion if it wasn't.
    /// </summary>
    private void CheckPrepared()
    {
      if( m_baDocumentID == null )
        throw new ApplicationException( "Decryption wasn't prepared." );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private WordKey PrepareKey( long position )
    {
//      if( m_key == null )
//      {
//        m_key = new WordKey();
//      }

      WordKey result = new WordKey();
      int count = ( int )( position - m_lLastStreamPosition );

      int iCurrentBlock = ( int )( position / DEF_BLOCK_SIZE );
      //if( iCurrentBlock != m_iLastBlock )
      {
        //m_iLastBlock = iCurrentBlock;
        MakeKey( result, ( uint )iCurrentBlock, m_valContext );
        count = ( int )( position % DEF_BLOCK_SIZE );

      }

      DecryptBuffer( null, 0, count, result );
      m_lLastStreamPosition = position;
      return result;
    }
    #endregion

    #region IDecryptor Members
    /// <summary>
    /// Sets information required to decrypt.
    /// </summary>
    /// <param name="docId">Unique document id.</param>
    /// <param name="encryptedDocId">Encrypted document id.</param>
    /// <param name="digest">Digest used to verify the entered password.</param>
    /// <param name="password">Password to use for verification and decryption.</param>
    public bool SetDecryptionInfo( byte[] docId, byte[] encryptedDocId,
      byte[] digest, string password )
    {
      // TODO:  Add MD5Decryptor.SetDecryptionInfo implementation
      if( docId == null )
        throw new ArgumentNullException( "docId" );

      if( encryptedDocId == null )
        throw new ArgumentNullException( "encryptedDocId" );

      if( digest == null )
        throw new ArgumentNullException( "digest" );

      if( password == null )
        throw new ArgumentNullException( "password" );

      if( DEF_READ_LENGTH != docId.Length )
        throw new ArgumentOutOfRangeException( "docId" );

      if( DEF_READ_LENGTH != encryptedDocId.Length )
        throw new ArgumentOutOfRangeException( "encryptedDocId" );

      if( DEF_READ_LENGTH != digest.Length )
        throw new ArgumentOutOfRangeException( "digest" );

      m_valContext = new MD5Context();
      m_baDocumentID = new byte[ DEF_READ_LENGTH ];
      m_baPoint = new byte[ DEF_PAS_LEN ];
      m_baHash = new byte[ DEF_READ_LENGTH ];

      Buffer.BlockCopy( docId, 0, m_baDocumentID, 0, DEF_READ_LENGTH );
      Buffer.BlockCopy( encryptedDocId, 0, m_baPoint, 0, DEF_READ_LENGTH );
      Buffer.BlockCopy( digest, 0, m_baHash, 0, DEF_READ_LENGTH );

      return CheckPassword( password );
//      if( !CheckPassword( password ) )
//        throw new ArgumentOutOfRangeException( "password", "Wrong password." );
    }

    #endregion

    #region IEncryptor Members
    /// <summary>
    /// Sets information required to encrypt the document.
    /// </summary>
    /// <param name="docId">Unique document id.</param>
    /// <param name="password">Encryption password.</param>
    /// <returns>True if password was verified and method succeeded.</returns>
    public void SetEncryptionInfo( byte[] docId, string password )
    {
      if( password == null )
        throw new ArgumentNullException( "password" );

      if( docId == null || docId.Length != DEF_READ_LENGTH )
        throw new ArgumentOutOfRangeException( "docId" );

//      docId = new byte[] { 0xD3, 0xA6, 0x87, 0xB3, 0x52, 0xE9, 0x34, 0x4D, 0xB2, 0x70, 0xD3, 0x74, 0x1C, 0x37, 0x40, 0x41 };
//      byte[] encryptedDocId = new byte[]{ 0xEE, 0xA6, 0x94, 0x9D, 0x39, 0x2F, 0x72, 0x10, 0xAA, 0xAB, 0x5A, 0x1D, 0xA5, 0x6D, 0x7F, 0xC1 };
//      byte[] digest = new byte[] { 0x87, 0xBF, 0xA0, 0x57, 0x4B, 0xE2, 0xCF, 0x10, 0xFB, 0x36, 0xE2, 0x32, 0xC5, 0xAA, 0xD4, 0x2B };
//      SetDecryptionInfo( docId, encryptedDocId, digest, password );
      m_valContext = new MD5Context();
      m_baDocumentID = docId;
      PreparePassword( password );
      PrepareValContext();

      m_baPoint = new byte[ DEF_PAS_LEN ];
      Buffer.BlockCopy( docId, 0, m_baPoint, 0, DEF_READ_LENGTH );

      m_baPoint[ 16 ] = 0x80;
      SetByte( m_baPoint, 17, 47, 0 );
      m_baPoint[ 56 ] = 0x80;

      MD5Context mdContext2 = new MD5Context();
      mdContext2.Update( m_baPoint, DEF_PAS_LEN );
      mdContext2.StoreDigest();

      m_baHash = new byte[ DEF_READ_LENGTH ];
      Buffer.BlockCopy( mdContext2.Digest, 0, m_baHash, 0, DEF_READ_LENGTH );

      WordKey key = new WordKey();
      MakeKey( key, 0, m_valContext );

      ByteArrayDataProvider provider = new ByteArrayDataProvider( m_baPoint );
      DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );

      provider.SetBuffer( m_baHash );
      DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );
    }

    /// <summary>
    /// Encrypts DataProvider and writes result back into it.
    /// </summary>
    /// <param name="provider">Provider to encrypt.</param>
    /// <param name="offset">Offset to the first byte to encrypt.</param>
    /// <param name="length">Number of bytes to encrypt.</param>
    /// <param name="streamPosition">Position of the data in the stream.</param>
    public void Encrypt( DataProvider provider, int offset, int length, long streamPosition )
    {
      Decrypt( provider, offset, length, streamPosition );
    }

    /// <summary>
    /// Encrypts DataProvider and writes result back into it.
    /// </summary>
    /// <param name="data">Data to encrypt.</param>
    /// <param name="offset">Offset to the first byte to encrypt.</param>
    /// <param name="length">Number of bytes to encrypt.</param>
    /// <param name="streamPosition">Position of the data in the stream.</param>
    public void Encrypt( byte[] data, int offset, int length, long streamPosition )
    {
      if( m_provider == null )
      {
        m_provider = new ByteArrayDataProvider( data );
      }
      else
      {
        m_provider.SetBuffer( data );
      }

      Encrypt( m_provider, offset, length, streamPosition );
    }

    /// <summary>
    /// Creates FilPassRecord that corresponds to this document.
    /// This method can only be called after encryption info was set.
    /// </summary>
    /// <returns>Created record.</returns>
    public FilePassRecord GetFilePassRecord()
    {
      FilePassRecord result = ( FilePassRecord )BiffRecordFactory.GetRecord( TBIFFRecord.FilePass );
      result.IsWeakEncryption = false;
      result.Key = result.Hash = FilePassRecord.DEF_STANDARD_HASH;
      result.CreateStandardBlock();
      FilePassStandardBlock standardBlock = result.StandardBlock;

      Buffer.BlockCopy( m_baDocumentID, 0, standardBlock.DocumentID, 0, DEF_READ_LENGTH );
      Buffer.BlockCopy( m_baPoint, 0, standardBlock.EncyptedDocumentID, 0, DEF_READ_LENGTH );
      Buffer.BlockCopy( m_baHash, 0, standardBlock.Digest, 0, DEF_READ_LENGTH );

//      byte[] docId = new byte[] { 0xD3, 0xA6, 0x87, 0xB3, 0x52, 0xE9, 0x34, 0x4D, 0xB2, 0x70, 0xD3, 0x74, 0x1C, 0x37, 0x40, 0x41 };
//      byte[] encryptedDocId = new byte[]{ 0xEE, 0xA6, 0x94, 0x9D, 0x39, 0x2F, 0x72, 0x10, 0xAA, 0xAB, 0x5A, 0x1D, 0xA5, 0x6D, 0x7F, 0xC1 };
//      byte[] digest = new byte[] { 0x87, 0xBF, 0xA0, 0x57, 0x4B, 0xE2, 0xCF, 0x10, 0xFB, 0x36, 0xE2, 0x32, 0xC5, 0xAA, 0xD4, 0x2B };
//
//      Buffer.BlockCopy( docId, 0, standardBlock.DocumentID, 0, DEF_READ_LENGTH );
//      Buffer.BlockCopy( encryptedDocId, 0, standardBlock.EncyptedDocumentID, 0, DEF_READ_LENGTH );
//      Buffer.BlockCopy( digest, 0, standardBlock.Digest, 0, DEF_READ_LENGTH );

      return result;
    }
    #endregion
  }
}
