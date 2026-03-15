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
	/// Utility Class
	/// </summary>
	public class Utils
	{
    #region Class constants
    /// <summary>
    /// Bit-indexes for reversing.
    /// </summary>
    private static readonly byte[] DEF_REVERSE_BITS =
    {
      0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15
    };
    
    /// <summary>
    /// Code lengths for the code length alphabet.
    /// </summary>
    public static readonly int[] DEF_HUFFMAN_DYNTREE_CODELENGTHS_ORDER =
    {
      16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
    };
    #endregion

    /// <summary>
    /// Reverses bit.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static short BitReverse( int value )
    {
      return ( short ) ( DEF_REVERSE_BITS[ value & 15 ] << 12
        | DEF_REVERSE_BITS[ ( value >> 4 ) & 15 ] << 8
        | DEF_REVERSE_BITS[ ( value >> 8 ) & 15 ] << 4
        | DEF_REVERSE_BITS[ value >> 12 ] );
    }
  }
}
