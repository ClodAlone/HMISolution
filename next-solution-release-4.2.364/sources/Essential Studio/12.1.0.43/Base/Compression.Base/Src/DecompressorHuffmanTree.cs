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
using System.Diagnostics;
#endregion

namespace Syncfusion.Compression
{
  /// <summary>
  /// Huffman tree, used for decompression.
  /// </summary>
  public class DecompressorHuffmanTree
  {
    #region Class constants
    /// <summary>
    /// Maximum count of bits.
    /// </summary>
    private static int MAX_BITLEN = 15;
    #endregion

    #region Class members
    /// <summary>
    /// Build huffman tree.
    /// </summary>
    private short[] m_Tree;
    #endregion

    #region Class Static Members
    /// <summary>
    /// Huffman tree for encoding and decoding lengths.
    /// </summary>
    private static DecompressorHuffmanTree m_LengthTree;

    /// <summary>
    /// huffman tree for encoding and decoding distances.
    /// </summary>
    private static DecompressorHuffmanTree m_DistanceTree;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Generates fixed huffman trees.
    /// </summary>
    static DecompressorHuffmanTree()
    {
      try
      {
        byte[] lengths;
        int index;

        // Generate huffman tree for lengths.
        lengths = new byte[ 288 ];
        index = 0;

        while( index < 144 )
        {
          lengths[ index++ ] = 8;
        }

        while( index < 256 )
        {
          lengths[ index++ ] = 9;
        }

        while( index < 280 )
        {
          lengths[ index++ ] = 7;
        }

        while( index < 288 )
        {
          lengths[ index++ ] = 8;
        }

        m_LengthTree = new DecompressorHuffmanTree( lengths );

        // Generate huffman tree for distances.
        lengths = new byte[ 32 ];
        index = 0;

        while( index < 32 )
        {
          lengths[ index++ ] = 5;
        }

        m_DistanceTree = new DecompressorHuffmanTree( lengths );
      }
      catch( Exception ex )
      {
        throw new Exception( "DecompressorHuffmanTree: fixed trees generation failed", ex );
      }
    }

    /// <summary>
    /// Creates huffman tree.
    /// </summary>
    /// <param name="codeLengths"></param>
    public DecompressorHuffmanTree( byte[] codeLengths )
    {
      BuildTree( codeLengths );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Prepares data for generating huffman tree.
    /// </summary>
    /// <param name="blCount">Array of counts of each code length.</param>
    /// <param name="nextCode">Numerical values of the smallest code for each code length.</param>
    /// <param name="lengths">Array of code lengths.</param>
    /// <param name="treeSize">Calculated tree size.</param>
    /// <returns>Code.</returns>
    private int PrepareData( int[] blCount, int[] nextCode, byte[] lengths, out int treeSize )
    {
      int code = 0;
      treeSize = 512;

      // Count number of codes for each code length.
      for( int i = 0; i < lengths.Length; i++ )
      {
        int length = lengths[ i ];

        if( length > 0 )
        {
          blCount[ length ]++;
        }
      }

      for( int bits = 1; bits <= MAX_BITLEN; bits++ )
      {
        nextCode[ bits ] = code;
        code += blCount[ bits ] << ( 16 - bits );

        if( bits >= 10 )
        {
          int start = nextCode[ bits ] & 0x1ff80;
          int end = code & 0x1ff80;
          treeSize += ( end - start ) >> ( 16 - bits );
        }
      }

      /*      if( code != 65536 )
        throw new ZipException( "Code lengths don't add up properly." );*/

      return code;
    }

    /// <summary>
    /// Generates huffman tree.
    /// </summary>
    /// <param name="blCount">Array of counts of each code length.</param>
    /// <param name="nextCode">Numerical values of the smallest code for each code length.</param>
    /// <param name="code">Precalculated code.</param>
    /// <param name="lengths">Array of code lengths.</param>
    /// <param name="treeSize">Calculated size of the tree.</param>
    /// <returns>Generated tree.</returns>
    private short[] TreeFromData( int[] blCount, int[] nextCode, byte[] lengths, int code, int treeSize )
    {
      short[] tree = new short[ treeSize ];
      int pointer = 512;
      int increment = 1 << 7;

      for( int bits = MAX_BITLEN; bits >= 10; bits-- )
      {
        int end = code & 0x1ff80;
        code -= blCount[ bits ] << ( 16 - bits );
        int start = code & 0x1ff80;

        for( int i = start; i < end; i += increment )
        {
          tree[ Utils.BitReverse( i ) ] = ( short )( ( -pointer << 4 ) | bits );
          pointer += 1 << ( bits - 9 );
        }
      }

      for( int i = 0; i < lengths.Length; i++ )
      {
        int bits = lengths[ i ];

        if( bits == 0 )
        {
          continue;
        }

        code = nextCode[ bits ];
        int revcode = Utils.BitReverse( code );

        if( bits <= 9 )
        {
          do
          {
            tree[ revcode ] = ( short )( ( i << 4 ) | bits );
            revcode += 1 << bits;
          }
          while( revcode < 512 );
        }
        else
        {
          int subTree = tree[ revcode & 511 ];
          int treeLen = 1 << ( subTree & 15 );
          subTree = -( subTree >> 4 );

          do
          {
            tree[ subTree | ( revcode >> 9 ) ] = ( short )( ( i << 4 ) | bits );
            revcode += 1 << bits;
          }
          while( revcode < treeLen );
        }

        nextCode[ bits ] = code + ( 1 << ( 16 - bits ) );
      }

      return tree;
    }

    /// <summary>
    /// Builds huffman tree from array of code lengths.
    /// </summary>
    /// <param name="lengths">Array of code lengths.</param>
    private void BuildTree( byte[] lengths )
    {
      // Count of codes for each code length.
      int[] blCount = new int[ MAX_BITLEN + 1 ];

      // Numerical value of the smallest code for each code length.
      int[] nextCode = new int[ MAX_BITLEN + 1 ];

      int treeSize;
      int code = PrepareData( blCount, nextCode, lengths, out treeSize );

      m_Tree = TreeFromData( blCount, nextCode, lengths, code, treeSize );
    }

    /// <summary>
    /// Reads and decompresses one symbol.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public int UnpackSymbol( CompressedStreamReader input )
    {
      int lookahead, symbol;

      if( ( lookahead = input.PeekBits( 9 ) ) >= 0 )
      {
        if( ( symbol = m_Tree[ lookahead ] ) >= 0 )
        {
          input.SkipBits( symbol & 15 );
          return symbol >> 4;
        }

        int subtree = -( symbol >> 4 );
        int bitlen = symbol & 15;

        if( ( lookahead = input.PeekBits( bitlen ) ) >= 0 )
        {
          symbol = m_Tree[ subtree | ( lookahead >> 9 ) ];
          input.SkipBits( symbol & 15 );
          return symbol >> 4;
        }
        else
        {
          int bits = input.AvailableBits;
          lookahead = input.PeekBits( bits );
          symbol = m_Tree[ subtree | ( lookahead >> 9 ) ];

          if( ( symbol & 15 ) <= bits )
          {
            input.SkipBits( symbol & 15 );
            return symbol >> 4;
          }
          else
          {
            return -1;
          }
        }
      }
      else
      {
        int bits = input.AvailableBits;
        lookahead = input.PeekBits( bits );
        symbol = m_Tree[ lookahead ];

        if( symbol >= 0 && ( symbol & 15 ) <= bits )
        {
          input.SkipBits( symbol & 15 );
          return symbol >> 4;
        }
        else
        {
          return -1;
        }
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// GET huffman tree for encoding and decoding lengths.
    /// </summary>
    public static DecompressorHuffmanTree LengthTree
    {
      get
      {
        return m_LengthTree;
      }
    }

    /// <summary>
    /// GET huffman tree for encoding and decoding distances.
    /// </summary>
    public static DecompressorHuffmanTree DistanceTree
    {
      get
      {
        return m_DistanceTree;
      }
    }
    #endregion
  }
}