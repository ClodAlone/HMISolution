#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif


#if  (SILVERLIGHT) || (WINRT) || (WP)
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

namespace Syncfusion.XlsIO.Implementation.Security
{
  /// <summary>
  /// This class contains utility methods used by Excel 2007 security implementation.
  /// </summary>
  sealed class SecurityHelper
  {
    #region Constants
      /// <summary>
      /// Represents the Excel 2010 encrypted version.
      /// </summary>
      internal const int Excel2010Version = 262148;
    /// <summary>
    /// Number of iterations used for key generation.
    /// </summary>
    private const int PasswordIterationCount = 50000;
    /// <summary>
    /// Name of encryption info stream.
    /// </summary>
    internal const string EncryptionInfoStream = "EncryptionInfo";
    /// <summary>
    /// Name of dataspaces storage.
    /// </summary>
    internal const string DataSpacesStorage = "\x0006DataSpaces";
    /// <summary>
    /// Name of dataspace map stream.
    /// </summary>
    internal const string DataSpaceMapStream = "DataSpaceMap";
    /// <summary>
    /// Name of the transform primary stream.
    /// </summary>
    internal const string TransformPrimaryStream = "\x06Primary";
    /// <summary>
    /// Name of dataspace info storage.
    /// </summary>
    internal const string DataSpaceInfoStorage = "DataSpaceInfo";
    /// <summary>
    /// Name of transform info storage.
    /// </summary>
    internal const string TransformInfoStorage = "TransformInfo";
    /// <summary>
    /// Name of encrypted package stream.
    /// </summary>
    internal const string EncryptedPackageStream = "EncryptedPackage";
    internal const string StrongEncryptionDataSpaceStream = "StrongEncryptionDataSpace";
    internal const string StrongEncryptionTransformStream = "StrongEncryptionTransform";
    internal const string VersionStream = "Version";
    #endregion

    #region Methods
    /// <summary>
    /// Reads Int32 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
    /// <returns>Extracted Int32 value.</returns>
    public static int ReadInt32( Stream stream, byte[] buffer )
    {
      if( stream.Read( buffer, 0, ExcelConstants.IntSize ) != ExcelConstants.IntSize )
        throw new InvalidDataException();

      return BitConverter.ToInt32( buffer, 0 );
    }
    /// <summary>
    /// Extracts padded unicode string from a stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <returns>Extracted string.</returns>
    public static string ReadUnicodeStringP4( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      int iLength = ReadInt32( stream, arrBuffer );
      arrBuffer = new byte[ iLength ];

      if( stream.Read( arrBuffer, 0, iLength ) != iLength )
        throw new InvalidDataException();

      string result = Encoding.Unicode.GetString( arrBuffer, 0, arrBuffer.Length );

      int iPadding = iLength % 4;

      if( iPadding != 0 )
        stream.Position += 4 - iPadding;

      return result;
    }
    /// <summary>
    /// Read zero-terminated string from the stream.
    /// </summary>
    /// <param name="stream">Stream to get string from.</param>
    /// <returns>Extracted string (without trailing zero).</returns>
    public static string ReadUnicodeStringZero( Stream stream )
    {
      StringBuilder builder = new StringBuilder();
      byte[] arrCharacter = new byte[ 2 ];

      while( stream.Read( arrCharacter, 0, 2 ) > 0 )
      {
        string chr = Encoding.Unicode.GetString( arrCharacter, 0, arrCharacter.Length );

        if( chr[ 0 ] != '\0' )
        {
          builder.Append( chr );
        }
        else
        {
          break;
        }
      }

      return builder.ToString();
    }
    /// <summary>
    /// Writes Int32 value into the stream.
    /// </summary>
    /// <param name="stream">Stream to put data into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteInt32( Stream stream, int value )
    {
      byte[] arrData = BitConverter.GetBytes( value );
      stream.Write( arrData, 0, ExcelConstants.IntSize );
    }
    /// <summary>
    /// Writes padded unicode string from a stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteUnicodeStringP4( Stream stream, string value )
    {
      byte[] arrBuffer = Encoding.Unicode.GetBytes( value );
      int iLength = arrBuffer.Length;
      WriteInt32( stream, iLength );
      stream.Write( arrBuffer, 0, iLength );

      int iPadding = iLength % 4;

      if( iPadding != 0 )
      {
        for( int i = 0, len = 4 - iPadding; i < len; i++ )
          stream.WriteByte( 0 );
      }
    }
    /// <summary>
    /// Writes zero-terminated string into the stream.
    /// </summary>
    /// <param name="stream">Stream to put string into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteUnicodeStringZero( Stream stream, string value )
    {
      int iStringLength = value.Length;

      //( iStringLength > 0 && value[ iStringLength - 1 ] != '\0' )

      byte[] arrData = Encoding.Unicode.GetBytes( value );
      stream.Write( arrData, 0, arrData.Length );

      if( iStringLength == 0 || value[ iStringLength - 1 ] != '\0' )
      {
        stream.WriteByte( 0 );
        stream.WriteByte( 0 );
      }
    }
    /// <summary>
    /// Creates key object based on the salt and password.
    /// </summary>
    /// <param name="password">Password to use.</param>
    /// <param name="salt">Salt to use.</param>
    /// <param name="keyLength">Required key length.</param>
    /// <returns>Array with created key.</returns>
    internal static byte[] CreateKey( string password, byte[] salt, int keyLength )
    {
#if !SILVERLIGHT && !WINRT && !WP
      SHA1CryptoServiceProvider
#else
      SHA1Managed
#endif
        sha1 = new 
#if !SILVERLIGHT && !WINRT && !WP
          SHA1CryptoServiceProvider();
#else
          SHA1Managed();
#endif
      byte[] arrPassword = Encoding.Unicode.GetBytes( password );
      byte[] saltPassword = new byte[ salt.Length + arrPassword.Length ];
      Buffer.BlockCopy( salt, 0, saltPassword, 0, salt.Length );
      Buffer.BlockCopy( arrPassword, 0, saltPassword, salt.Length, arrPassword.Length );

      byte[] H0 = sha1.ComputeHash( saltPassword );
      byte[] inputData = new byte[ H0.Length + 4 ];
      byte[] Hi = H0;
      byte[] iterator;

      for( int i = 0; i < PasswordIterationCount; i++ )
      {
        iterator = BitConverter.GetBytes( i );
        Buffer.BlockCopy( iterator, 0, inputData, 0, iterator.Length );
        Buffer.BlockCopy( Hi, 0, inputData, iterator.Length, Hi.Length );
        Hi = sha1.ComputeHash( inputData );
      }

      iterator = BitConverter.GetBytes( 0 );
      Buffer.BlockCopy( Hi, 0, inputData, 0, Hi.Length );
      Buffer.BlockCopy( iterator, 0, inputData, Hi.Length, iterator.Length );
      byte[] HFinal = sha1.ComputeHash( inputData );

      byte[] vector64 = new byte[ 64 ];

      for( int i = 0; i < 64; i++ )
        vector64[ i ] = 0x36;

      for( int i = 0, len = HFinal.Length; i < len; i++ )
        vector64[ i ] ^= HFinal[ i ];

      byte[] x1 = sha1.ComputeHash( vector64 );

      for( int i = 0; i < 64; i++ )
        vector64[ i ] = 0x5C;

      for( int i = 0, len = HFinal.Length; i < len; i++ )
        vector64[ i ] ^= HFinal[ i ];

      byte[] x2 = sha1.ComputeHash( vector64 );

      if( keyLength <= x1.Length )
      {
        byte[] arrResult = new byte[ keyLength ];
        Buffer.BlockCopy( x1, 0, arrResult, 0, keyLength );
        return arrResult;
        //return Hi;
      }
      else
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Encrypts/decrypts buffer with specified method.
    /// </summary>
    /// <param name="data">Data to process.</param>
    /// <param name="method">Method to use.</param>
    /// <param name="blockSize">Size of the encryption block.</param>
    /// <returns>Modified (encrypted/decrypted) data.</returns>
    internal static byte[] EncryptDecrypt( byte[] data, EncryptionMethod method, int blockSize )
    {
      int iLength = data.Length;
      byte[] result = new byte[ iLength ];
      byte[] arrBuffer = new byte[ blockSize ];
      byte[] arrTemp2 = new byte[ blockSize ];

      int iOffset = 0;
      while( iOffset < iLength )
      {
        int iDataLeft = iLength - iOffset;
        int iDataSize = Math.Min( iDataLeft, blockSize );
        Buffer.BlockCopy( data, iOffset, arrBuffer, 0, iDataSize );
        method( arrBuffer, arrTemp2 );
        Buffer.BlockCopy( arrTemp2, 0, result, iOffset, iDataSize );
        iOffset += blockSize;
      }

      return result;
    }
    /// <summary>
    /// Combines two arrays into one.
    /// </summary>
    /// <param name="buffer1">The first buffer to combine.</param>
    /// <param name="buffer2">The second buffer to combine.</param>
    /// <returns>Combined array.</returns>
    public static byte[] CombineArray( byte[] buffer1, byte[] buffer2 )
    {
      int iLength1 = buffer1.Length;
      int iLength2 = buffer2.Length;
      int iCombinedLength = iLength1 + iLength2;
      byte[] arrResult = new byte[ iCombinedLength ];

      Buffer.BlockCopy( buffer1, 0, arrResult, 0, iLength1 );
      Buffer.BlockCopy( buffer2, 0, arrResult, iLength1, iLength2 );

      return arrResult;
    }
    internal delegate void EncryptionMethod( byte[] buffer1, byte[] buffer2 );
    #endregion
  }
}
