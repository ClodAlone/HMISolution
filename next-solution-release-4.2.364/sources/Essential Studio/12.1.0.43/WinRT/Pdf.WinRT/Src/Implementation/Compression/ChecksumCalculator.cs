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
#endregion

namespace Syncfusion.Compression
{
  /// <summary>
  /// Checksum calculator, based on Adler32 algorithm.
  /// </summary>
  public class ChecksumCalculator
  {
    #region Class constants
    /// <summary>
    /// Bits offset, used in adler checksum calculation.
    /// </summary>
    private const int DEF_CHECKSUM_BIT_OFFSET = 16;
    /// <summary>
    /// Lagrest prime, less than 65535
    /// </summary>
    private const int DEF_CHECKSUM_BASE = 65521;
    /// <summary>
    /// Count of iteration used in calculated of the adler checksumm.
    /// </summary>
    private const int DEF_CHECKSUM_ITERATIONSCOUNT = 3800;
    #endregion

    #region Class Static Methods
    /// <summary>
    /// Updates checksum by calculating checksum of the 
    /// given buffer and adding it to current value.
    /// </summary>
    /// <param name="checksum">Current checksum.</param>
    /// <param name="buffer">Data byte array.</param>
    /// <param name="offset">Offset in the buffer.</param>
    /// <param name="length">Length of data to be used from the stream.</param>
    public static void ChecksumUpdate( ref long checksum, byte[] buffer, int offset, int length )
    {
      uint checksum_uint = ( uint )checksum;
      uint s1 = checksum_uint & ushort.MaxValue;
      uint s2 = checksum_uint >> DEF_CHECKSUM_BIT_OFFSET;

      while( length > 0 )
      {
        int steps = Math.Min( length, DEF_CHECKSUM_ITERATIONSCOUNT );
        length -= steps;

        while( --steps >= 0 )
        {
          s1 = s1 + ( uint )( buffer[ offset++ ] & byte.MaxValue );
          s2 = s2 + s1;
        }

        s1 %= DEF_CHECKSUM_BASE;
        s2 %= DEF_CHECKSUM_BASE;
      }

      checksum_uint = ( s2 << DEF_CHECKSUM_BIT_OFFSET ) | s1;
      checksum = ( long )checksum_uint;
    }
    /// <summary>
    /// Generates checksum by calculating checksum of the 
    /// given buffer.
    /// </summary>
    /// <param name="buffer">Data byte array.</param>
    /// <param name="offset">Offset in the buffer.</param>
    /// <param name="length">Length of data to be used from the stream.</param>
    public static long ChecksumGenerate( byte[] buffer, int offset, int length )
    {
      long result = 1;
      ChecksumUpdate( ref result, buffer, offset, length );
      return result;
    }
    #endregion
  }
}