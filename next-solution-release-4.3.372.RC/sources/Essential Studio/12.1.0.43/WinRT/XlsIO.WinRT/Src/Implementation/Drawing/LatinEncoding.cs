#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;

using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class emulates latin1 encoding based on Unicode encoding.
  /// </summary>
  public class LatinEncoding : Encoding
  {
    /// <summary>
    /// Calculates the number of bytes produced by encoding a set of characters. 
    /// </summary>
    /// <param name="chars">The character array containing the set of characters to encode.</param>
    /// <param name="index">The index of the first character to encode.</param>
    /// <param name="count">The number of characters to encode.</param>
    /// <returns>The number of bytes produced by encoding the specified characters.</returns>
    public override int GetByteCount( char[] chars, int index, int count )
    {
      return count;
    }
    /// <summary>
    /// Encodes a set of characters from the specified character array into the specified byte array.
    /// </summary>
    /// <param name="chars">The character array containing the set of characters to encode.</param>
    /// <param name="charIndex">The index of the first character to encode.</param>
    /// <param name="charCount">The number of characters to encode.</param>
    /// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
    /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
    /// <returns>The actual number of bytes written into bytes.</returns>
    public override int GetBytes( char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex )
    {
      //byte[] result = new byte[ charCount * 2 ];
      for( int i = 0; i < charCount; i++, charIndex++, byteIndex++ )
      {
        bytes[ byteIndex ] = ( byte )( chars[ charIndex ] & 0xFF );
      }

      return charCount;
    }
    /// <summary>
    /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
    /// </summary>
    /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
    /// <param name="index">The index of the first byte to decode.</param>
    /// <param name="count">The number of bytes to decode.</param>
    /// <returns>The number of characters produced by decoding the specified sequence of bytes.</returns>
    public override int GetCharCount( byte[] bytes, int index, int count )
    {
      return count;
    }
    /// <summary>
    /// Decodes a sequence of bytes from the specified byte array into the specified character array.
    /// </summary>
    /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
    /// <param name="byteIndex">The index of the first byte to decode.</param>
    /// <param name="byteCount">The number of bytes to decode.</param>
    /// <param name="chars">The character array to contain the resulting set of characters.</param>
    /// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
    /// <returns>The actual number of characters written into chars</returns>
    public override int GetChars( byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex )
    {
      int iRealByteCount = byteCount * 2;
      byte[] arrRealBytes = new byte[ iRealByteCount ];

      for( int i = byteIndex, last = byteIndex + byteCount, offset = 0; i < last; i++, offset += 2 )
      {
        arrRealBytes[ offset ] = bytes[ i ];
        arrRealBytes[ offset + 1 ] = 0;
      }

      return Encoding.Unicode.GetChars( arrRealBytes, 0, iRealByteCount, chars, 0 );
    }
    /// <summary>
    /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
    /// </summary>
    /// <param name="charCount">The number of characters to encode.</param>
    /// <returns>The maximum number of bytes produced by encoding the specified number of characters.</returns>
    public override int GetMaxByteCount( int charCount )
    {
      return charCount;
    }
    /// <summary>
    /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
    /// </summary>
    /// <param name="byteCount">The number of bytes to decode.</param>
    /// <returns>The maximum number of characters produced by decoding the specified number of bytes.</returns>
    public override int GetMaxCharCount( int byteCount )
    {
      return byteCount;
    }
  }
}
