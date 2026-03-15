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

#if !DOCIO && (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  public static class StreamHelper
  {
    #region Constants
    /// <summary>
    /// Size of the Int32 in bytes.
    /// </summary>
    public const int IntSize = 4;
    /// <summary>
    /// Size of the Int162 in bytes.
    /// </summary>
    private const int ShortSize = 2;
    /// <summary>
    /// Size of the Double in bytes.
    /// </summary>
    private const int DoubleSize = 8;
    #endregion

    #region Methods
    /// <summary>
    /// Reads Int16 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
    /// <returns>Extracted Int32 value.</returns>
    public static short ReadInt16( Stream stream, byte[] buffer )
    {
      if( stream.Read( buffer, 0, ShortSize ) != ShortSize )
        throw new Exception( "Invalid Data" );

      return BitConverter.ToInt16( buffer, 0 );
    }
    /// <summary>
    /// Reads Int32 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
    /// <returns>Extracted Int32 value.</returns>
    public static int ReadInt32( Stream stream, byte[] buffer )
    {
      if( stream.Read( buffer, 0, IntSize ) != IntSize )
        throw new Exception( "Invalid data" );

      return BitConverter.ToInt32( buffer, 0 );
    }
    /// <summary>
    /// Reads Double value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
    /// <returns>Extracted Double value.</returns>
    public static double ReadDouble( Stream stream, byte[] buffer )
    {
      if( stream.Read( buffer, 0, DoubleSize ) != DoubleSize )
        throw new Exception( "Invalid data" );

      return BitConverter.ToDouble( buffer, 0 );
    }
    /// <summary>
    /// Writes Int16 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>Size of the written data.</returns>
    public static int WriteInt16( Stream stream, short value )
    {
      byte[] buffer = BitConverter.GetBytes( value );
      stream.Write( buffer, 0, ShortSize );

      return ShortSize;
    }
    /// <summary>
    /// Writes Int32 value from the stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>Size of the written data.</returns>
    public static int WriteInt32( Stream stream, int value )
    {
      byte[] buffer = BitConverter.GetBytes( value );
      stream.Write( buffer, 0, IntSize );
      return IntSize;
    }
    /// <summary>
    /// Reads Double value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
    /// <returns>Extracted Double value.</returns>
    public static int WriteDouble( Stream stream, double value )
    {
      byte[] buffer = BitConverter.GetBytes( value );
      stream.Write( buffer, 0, DoubleSize );
      return DoubleSize;
    }
    /// <summary>
    /// Gets ASCII string from the stream starting from the current position.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="roundedSize">Approximate string size.</param>
    /// <returns>Extracted string.</returns>
    public static string GetAsciiString( Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 4 ];
      int iStringSize = StreamHelper.ReadInt32( stream, buffer );
      byte[] arrStringData = new byte[ iStringSize ];

      if( stream.Read( arrStringData, 0, iStringSize ) != iStringSize )
        throw new IOException();

      for( int i = 0; i < iStringSize; i++ )
      {
        if( arrStringData[ i ] == 0 )
        {
          roundedSize = i;
          break;
        }
      }

      Encoding encoding =
#if !SILVERLIGHT && !WINRT && !WP
 Encoding.Default;
#else
      Encoding.UTF8;
#endif

      string result = encoding.GetString( arrStringData, 0, roundedSize );
      return RemoveLastZero( result );
    }
    /// <summary>
    /// Extracts unicode string from the stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="roundedSize"></param>
    /// <returns></returns>
    public static string GetUnicodeString( Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 4 ];
      int iStringSize = StreamHelper.ReadInt32( stream, buffer ) * 2;
      byte[] arrStringData = new byte[ iStringSize ];

      if( stream.Read( arrStringData, 0, iStringSize ) != iStringSize )
        throw new IOException();

      //for( int i = 0; i < roundedSize; i++ )
      //{
      //  if( arrStringData[ i ] == 0 )
      //  {
      //    roundedSize = i;
      //    break;
      //  }
      //}

      Encoding encoding = Encoding.Unicode;
      string result = encoding.GetString( arrStringData, 0, iStringSize );
      result = RemoveLastZero( result );

      int mod = iStringSize % 4;

      if( mod != 0 )
        stream.Position += 4 - mod;

      return result;
    }
    /// <summary>
    /// Gets ASCII string from the stream starting from the current position.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="roundedSize">Approximate string size.</param>
    /// <returns>Extracted string.</returns>
    public static int WriteAsciiString( Stream stream, string value, bool align )
    {
      Encoding encoding =
#if !SILVERLIGHT && !WINRT && !WP
 Encoding.Default;
#else
        Encoding.UTF8;
#endif
      return WriteString( stream, value, encoding, align );
    }
    /// <summary>
    /// Writes unicode string into steram.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>Size of the written data in bytes.</returns>
    public static int WriteUnicodeString( Stream stream, string value )
    {
      return WriteString( stream, value, Encoding.Unicode, true );
    }
    /// <summary>
    /// Writes string into stream using specified encoding.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="encoding">Encoding to use.</param>
    /// <returns>Size of the written data in bytes.</returns>
    public static int WriteString( Stream stream, string value, Encoding encoding, bool align )
    {
      if( String.IsNullOrEmpty( value ) )
      {
        value = "\0";
      }
      else if( value[ value.Length - 1 ] != '\0' )
      {
        value += "\0";
      }

      byte[] arrStringData = encoding.GetBytes( value );
      int iStringDataSize = arrStringData.Length;
      int iStringLength = value.Length;
      byte[] buffer = BitConverter.GetBytes( iStringLength );

      stream.Write( buffer, 0, buffer.Length );
      stream.Write( arrStringData, 0, iStringDataSize );

      int iResult = StreamHelper.IntSize + iStringDataSize;

      if( align )
      {
        int mod = ( int )( /*stream.Position*/iResult % 4 );

        if( mod != 0 )
        {
          for( int i = 0, len = 4 - mod; i < len; i++ )
          {
            stream.WriteByte( 0 );
          }

          iResult += 4 - mod;
        }
      }

      return iResult;
    }
    /// <summary>
    /// Adds padding if necessary.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="iWrittenSize"></param>
    public static void AddPadding( Stream stream, ref int iWrittenSize )
    {
      int iMod = iWrittenSize % 4;

      if( iMod != 0 )
      {
        for( int i = 0, len = 4 - iMod; i < len; i++, iWrittenSize++ )
        {
          stream.WriteByte( 0 );
        }
      }
    }
    /// <summary>
    /// Removes last zero character from the string if it is present.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>String after removal.</returns>
    private static string RemoveLastZero( string value )
    {
      int iLength = ( value != null ) ? value.Length : 0;

      if( iLength > 0 && value[ iLength - 1 ] == '\0' )
        value = value.Substring( 0, iLength - 1 );

      return value;
    }
    #endregion
  }
}
