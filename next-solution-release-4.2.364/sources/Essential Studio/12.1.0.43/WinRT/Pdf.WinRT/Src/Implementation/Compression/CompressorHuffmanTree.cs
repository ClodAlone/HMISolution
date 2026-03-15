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
  /// Represents the Huffman Tree.
  /// </summary>
  public class CompressorHuffmanTree
  {
    #region Class members
    /// <summary>
    /// Frequences of the codes.
    /// </summary>
    private short[] m_CodeFrequences;

    /// <summary>
    /// Codes itself.
    /// </summary>
    private short[] m_Codes;

    /// <summary>
    /// Bit counts, needed to encode different codes.
    /// </summary>
    private byte[] m_CodeLengths;

    /// <summary>
    /// Count of codes with some lengths.
    /// Index - length, value - count.
    /// </summary>
    private int[] m_LengthCounts;

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    private int m_CodeMinimumCount;
    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    private int m_CodeCount;
    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    private int m_MaximumLength;

    /// <summary>
    /// Data compressor.
    /// </summary>
    private CompressedStreamWriter m_Writer;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Create a new Huffman tree
    /// </summary>
    /// <param name="writer"/>
    /// <param name="iElementsCount"/>
    /// <param name="iMinimumCodes"/>
    /// <param name="iMaximumLength"/>
    public CompressorHuffmanTree( CompressedStreamWriter writer, int iElementsCount,
                                  int iMinimumCodes, int iMaximumLength )
    {
      this.m_Writer = writer;
      this.m_CodeMinimumCount = iMinimumCodes;
      this.m_MaximumLength = iMaximumLength;
      m_CodeFrequences = new short[iElementsCount];
      m_LengthCounts = new int[iMaximumLength];
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Resets all code data in tree.
    /// </summary>
    public void Reset()
    {
      for( int i = 0 ; i < m_CodeFrequences.Length ; i++ )
      {
        m_CodeFrequences[i] = 0;
      }

      m_Codes = null;
      m_CodeLengths = null;
    }

    /// <summary>
    /// Writes code to the compressor output stream.
    /// </summary>
    /// <param name="code">Code to be written.</param>
    public void WriteCodeToStream( int code )
    {
      m_Writer.PendingBufferWriteBits( m_Codes[ code ] & 0xffff, m_CodeLengths[ code ] );
    }

    /// <summary>
    /// Checks wheather tree is empty.
    /// If tree is not empty, then exception will be raised.
    /// </summary>
    public void CheckEmpty()
    {
      for( int i = 0 ; i < m_CodeFrequences.Length ; i++ )
      {
        Debug.Assert( m_CodeFrequences[i] == 0, "Huffman compressor error");
      }
    }

    /// <summary>
    /// Specifies new arrays of codes and their lengths.
    /// </summary>
    /// <param name="codes">Array of codes.</param>
    /// <param name="lengths">Array of code lengths.</param>
    public void SetStaticCodes( short[] codes, byte[] lengths )
    {
      m_Codes = ( short[] )codes.Clone();
      m_CodeLengths = ( byte[] )lengths.Clone();
    }

    /// <summary>
    /// Calculates codes from their frequences.
    /// </summary>
    public void BuildCodes()
    {
      //Debug.Assert( m_CodeCount == m_CodeFrequences.Length, "Something wrong with count of codes." );

      int[] nextCode = new int[m_MaximumLength];
      m_Codes = new short[m_CodeCount];
      int code = 0;

      for( int bitsCount = 0 ; bitsCount < m_MaximumLength ; bitsCount++ )
      {
        nextCode[ bitsCount ] = code;
        code += m_LengthCounts[ bitsCount ] << ( 15 - bitsCount );
      }

      for( int i = 0 ; i < m_CodeCount ; i++ )
      {
        int bits = m_CodeLengths[ i ];

        if( bits > 0 )
        {
          m_Codes[ i ] = Utils.BitReverse( nextCode[ bits - 1 ] );
          nextCode[ bits - 1 ] += 1 << ( 16 - bits );
        }
      }
    }

    /// <summary>
    /// Build tree with lengths.
    /// </summary>
    /// <param name="childs"></param>
    private void BuildLength( int[] childs )
    {
      m_CodeLengths = new byte[m_CodeFrequences.Length];
      int numNodes = childs.Length / 2;
      int numLeafs = ( numNodes + 1 ) / 2;
      int overflow = 0;

      for( int i = 0 ; i < m_MaximumLength ; i++ )
      {
        m_LengthCounts[ i ] = 0;
      }

      // Calculating optimal code lengths
      int[] lengths = new int[numNodes];
      lengths[ numNodes - 1 ] = 0;

      for( int i = numNodes - 1 ; i >= 0 ; i-- )
      {
        int iChildIndex = 2 * i + 1;

        if( childs[ iChildIndex ] != -1 )
        {
          int bitLength = lengths[ i ] + 1;

          if( bitLength > m_MaximumLength )
          {
            bitLength = m_MaximumLength;
            overflow++;
          }

          lengths[ childs[ iChildIndex - 1 ] ] = lengths[ childs[ iChildIndex ] ] = bitLength;
        }
        else
        {
          int bitLength = lengths[ i ];
          m_LengthCounts[ bitLength - 1 ]++;
          m_CodeLengths[ childs[ iChildIndex - 1 ] ] = ( byte )lengths[ i ];
        }
      }

      if( overflow == 0 )
      {
        return;
      }

      int iIncreasableLength = m_MaximumLength - 1;

      do
      {
        // Find the first bit m_CodeLengths which could increase.
        while( m_LengthCounts[ --iIncreasableLength ] == 0 )
        {
          ;
        }

        do
        {
          m_LengthCounts[iIncreasableLength]--;
          m_LengthCounts[++iIncreasableLength]++;
          overflow -= ( 1 << ( m_MaximumLength - 1 - iIncreasableLength ) );
        }
        while( overflow > 0 && iIncreasableLength < m_MaximumLength - 1 );
      }
      while( overflow > 0 );

      m_LengthCounts[ m_MaximumLength - 1 ] += overflow;
      m_LengthCounts[ m_MaximumLength - 2 ] -= overflow;

      // Recreate tree.
      int nodePtr = 2 * numLeafs;

      for( int bits = m_MaximumLength ; bits != 0 ; bits-- )
      {
        int n = m_LengthCounts[ bits - 1 ];

        while( n > 0 )
        {
          int childPtr = 2 * childs[ nodePtr++ ];

          if( childs[ childPtr + 1 ] == -1 )
          {
            m_CodeLengths[childs[childPtr]] = ( byte )bits;
            n--;
          }
        }
      }
    }

    /// <summary>
    /// Builds tree.
    /// </summary>
    public void BuildTree()
    {
      int iCodesCount = m_CodeFrequences.Length;

      int[] arrTree = new int[iCodesCount];
      int iTreeLength = 0;
      int iMaxCode = 0;

      for( int n = 0 ; n < iCodesCount ; n++ )
      {
        int freq = m_CodeFrequences[ n ];

        if( freq != 0 )
        {
          int pos = iTreeLength++;
          int ppos;

          while( pos > 0 && m_CodeFrequences[ arrTree[ ppos = ( pos - 1 ) / 2 ] ] > freq )
          {
            arrTree[ pos ] = arrTree[ ppos ];
            pos = ppos;
          }

          arrTree[ pos ] = n;

          iMaxCode = n;
        }
      }

      while( iTreeLength < 2 )
      {
        arrTree[ iTreeLength++ ] =
          ( iMaxCode < 2 ) ? ++iMaxCode : 0;
      }

      m_CodeCount = Math.Max( iMaxCode + 1, m_CodeMinimumCount );

      int iLeafsCount = iTreeLength;
      int iNodesCount = iLeafsCount;
      int[] childs = new int[4 * iTreeLength - 2];
      int[] values = new int[2 * iTreeLength - 1];

      for( int i = 0 ; i < iTreeLength ; i++ )
      {
        int node = arrTree[ i ];
        int iIndex = 2 * i;
        childs[ iIndex ] = node;
        childs[ iIndex + 1] = -1;
        values[ i ] = ( m_CodeFrequences[ node ] << 8 );
        arrTree[ i ] = i;
      }

      do
      {
        int first = arrTree[ 0 ];
        int last = arrTree[ --iTreeLength ];
        int lastVal = values[ last ];

        int ppos = 0;
        int path = 1;

        while( path < iTreeLength )
        {
          if( path + 1 < iTreeLength &&
            values[ arrTree[ path ] ] > values[ arrTree[ path + 1 ] ] )
          {
            path++;
          }

          arrTree[ ppos ] = arrTree[ path ];
          ppos = path;
          path = ppos * 2 + 1;
        }

        while( ( path = ppos ) > 0 && values[ arrTree[ ppos = ( path - 1 ) / 2 ] ] > lastVal )
        {
          arrTree[ path ] = arrTree[ ppos ];
        }

        arrTree[ path ] = last;

        int second = arrTree[ 0 ];

        last = iNodesCount++;
        childs[ 2 * last ] = first;
        childs[ 2 * last + 1 ] = second;
        int mindepth = Math.Min( values[ first ] & 0xff, values[ second ] & 0xff );
        values[ last ] =
          lastVal =
            values[ first ] + values[ second ] - mindepth + 1;

        ppos = 0;
        path = 1;

        while( path < iTreeLength )
        {
          if( path + 1 < iTreeLength &&
            values[ arrTree[ path ] ] > values[ arrTree[ path + 1 ] ] )
          {
            path++;
          }

          arrTree[ ppos ] = arrTree[ path ];
          ppos = path;
          path = ppos * 2 + 1;
        }

        while( ( path = ppos ) > 0 &&
          values[ arrTree[ ppos = ( path - 1 ) / 2 ] ] > lastVal )
        {
          arrTree[ path ] = arrTree[ ppos ];
        }

        arrTree[ path ] = last;
      }
      while( iTreeLength > 1 );

      if( arrTree[ 0 ] != childs.Length / 2 - 1 )
      {
        throw new Exception( "Heap invariant violated" );
      }

      BuildLength( childs );
    }

    /// <summary>
    /// Calculates length of the compressed data.
    /// </summary>
    /// <returns>Count of bits, the data will occupy.</returns>
    public int GetEncodedLength()
    {
      int len = 0;

      for( int i = 0 ; i < m_CodeFrequences.Length ; i++ )
      {
        len += m_CodeFrequences[ i ] * m_CodeLengths[ i ];
      }

      return len;
    }

    /// <summary>
    /// Calculates code frequences.
    /// </summary>
    /// <param name="blTree">Tree.</param>
    public void CalcBLFreq( CompressorHuffmanTree blTree )
    {
      int max_count; /* max repeat count */
      int min_count; /* min repeat count */
      int count; /* repeat count of the current code */
      int curlen = -1; /* m_CodeLengths of current code */

      int i = 0;
      while( i < m_CodeCount )
      {
        count = 1;
        int nextlen = m_CodeLengths[i];
        if( nextlen == 0 )
        {
          max_count = 138;
          min_count = 3;
        }
        else
        {
          max_count = 6;
          min_count = 3;
          if( curlen != nextlen )
          {
            blTree.m_CodeFrequences[nextlen]++;
            count = 0;
          }
        }
        curlen = nextlen;
        i++;

        while( i < m_CodeCount && curlen == m_CodeLengths[i] )
        {
          i++;
          if( ++count >= max_count )
          {
            break;
          }
        }

        if( count < min_count )
        {
          blTree.m_CodeFrequences[curlen] += ( short )count;
        }
        else if( curlen != 0 )
        {
          blTree.m_CodeFrequences[ 16 ]++;
        }
        else if( count <= 10 )
        {
          blTree.m_CodeFrequences[ 17 ]++;
        }
        else
        {
          blTree.m_CodeFrequences[ 18 ]++;
        }
      }
    }

    /// <summary>
    /// Writes tree to output stream.
    /// </summary>
    /// <param name="blTree">Tree to be written.</param>
    public void WriteTree( CompressorHuffmanTree blTree )
    {
      int iMaxRepeatCount;
      int iMinRepeatCount;
      int iCurrentRepeatCount;
      int iCurrentCodeLength = -1;

      int i = 0;
      while( i < m_CodeCount )
      {
        iCurrentRepeatCount = 1;
        int nextlen = m_CodeLengths[ i ];

        if( nextlen == 0 )
        {
          iMaxRepeatCount = 138;
          iMinRepeatCount = 3;
        }
        else
        {
          iMaxRepeatCount = 6;
          iMinRepeatCount = 3;

          if( iCurrentCodeLength != nextlen )
          {
            blTree.WriteCodeToStream( nextlen );
            iCurrentRepeatCount = 0;
          }
        }

        iCurrentCodeLength = nextlen;
        i++;

        while( i < m_CodeCount && iCurrentCodeLength == m_CodeLengths[ i ] )
        {
          i++;

          if( ++iCurrentRepeatCount >= iMaxRepeatCount )
          {
            break;
          }
        }

        if( iCurrentRepeatCount < iMinRepeatCount )
        {
          while( iCurrentRepeatCount-- > 0 )
          {
            blTree.WriteCodeToStream( iCurrentCodeLength );
          }
        }
        else if( iCurrentCodeLength != 0 )
        {
          blTree.WriteCodeToStream( 16 );
          m_Writer.PendingBufferWriteBits( iCurrentRepeatCount - 3, 2 );
        }
        else if( iCurrentRepeatCount <= 10 )
        {
          blTree.WriteCodeToStream( 17 );
          m_Writer.PendingBufferWriteBits( iCurrentRepeatCount - 3, 3 );
        }
        else
        {
          blTree.WriteCodeToStream( 18 );
          m_Writer.PendingBufferWriteBits( iCurrentRepeatCount - 11, 7 );
        }
      }
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Length of the tree.
    /// </summary>
    public int TreeLength
    {
      get
      {
        return m_CodeCount;
      }
    }

    /// <summary>
    /// Lengths of codes in tree.
    /// </summary>
    public byte[] CodeLengths
    {
      get
      {
        return m_CodeLengths;
      }
    }

    /// <summary>
    /// Code frequences.
    /// </summary>
    public short[] CodeFrequences
    {
      get
      {
        return m_CodeFrequences;
      }
    }
    #endregion
  }
}