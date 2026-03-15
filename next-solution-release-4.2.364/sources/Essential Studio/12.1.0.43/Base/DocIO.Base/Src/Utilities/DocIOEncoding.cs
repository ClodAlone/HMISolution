#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.DocIO.Utilities
{
  public abstract class DocIOEncoding
  {
      #region Members
      private static int[] m_charCodeTable = {
                                                 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                                                 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                                                 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 
                                                 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 
                                                 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 
                                                 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 
                                                 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 
                                                 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 
                                                 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 
                                                 122, 123, 124, 125, 126, 127, 8364, 65533, 8218, 402, 8222, 
                                                 8230, 8224, 8225, 710, 8240, 352, 8249, 338, 65533, 381, 
                                                 65533, 65533, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 
                                                 8482, 353, 8250, 339, 65533, 382, 376, 160, 161, 162, 163, 
                                                 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
                                                 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 
                                                 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 
                                                 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 
                                                 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
                                                 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 
                                                 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 
                                                 248, 249, 250, 251, 252, 253, 254, 255
                                             };
      #endregion

      #region Methods
      public static string GetString(byte[] bytes)
      {
          StringBuilder builder = new StringBuilder();
          for (int i = 0, cnt = bytes.Length; i < cnt; i++)
          {
              int index = bytes[i];
              short symbolCode = (short)m_charCodeTable[index];
              byte[] letterBytes = BitConverter.GetBytes(symbolCode);
#if SILVERLIGHT || WP
              string unicodeLetter = Encoding.Unicode.GetString(letterBytes, 0, letterBytes.Length);
#else
              string unicodeLetter = Encoding.Unicode.GetString( letterBytes );
#endif
              builder.Append(unicodeLetter);
          }
          return builder.ToString();
      }
      /*/// <summary>
    /// ASCII to UTF8 string.
    /// </summary>
    /// <param name="asciiBytes">The ASCII bytes.</param>
    /// <returns></returns>
    public static string AsciiToUnicodeString( byte[] asciiBytes )
    {
      string word = string.Empty;
      for( int i = 0, count = asciiBytes.Length; i < count; i++ )
      {
        word += AsciiToUnicodeChars( asciiBytes[ i ] );
      }

      return word;
    }
    /// <summary>
    /// ASCII to UTF8 Chars.
    /// </summary>
    /// <param name="letter">The ascii letter.</param>
    /// <param name="utf8Letters">The UTF8 letters array.</param>
    private static string AsciiToUnicodeChars( byte letter )
    {
      byte[] letterByte = new byte[ 2 ];

      if( letter < 128 )
      {
        letterByte[ 0 ] = letter;
      }
      else
      {
        letterByte[ 0 ] = 63;
      }

      letterByte[ 1 ] = 0;
#if SILVERLIGHT || WP
      string unicodeLetter = Encoding.Unicode.GetString( letterByte, 0, letterByte.Length );
#else
      string unicodeLetter = Encoding.Unicode.GetString( letterByte );
#endif
      return unicodeLetter;
    }*/
    #endregion
  }
}
