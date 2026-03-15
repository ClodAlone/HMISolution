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
using System.IO;
#endregion

namespace Syncfusion.Compression
{
  /// <summary>
  /// Reader, that reads stream with compressed data
  /// </summary>
  public class CompressedStreamReader
  {
    #region Class constants
    /// <summary>
    /// Mask for compression method to be decoded from 16-bit header.
    /// </summary>
    private const int DEF_HEADER_METHOD_MASK = 15 << 8;
    /// <summary>
    /// Mask for compression info to be decoded from 16-bit header.
    /// </summary>
    private const int DEF_HEADER_INFO_MASK = 240 << 8;
    /// <summary>
    /// Mask for check bits to be decoded from 16-bit header.
    /// </summary>
    private const int DEF_HEADER_FLAGS_FCHECK = 31;
    /// <summary>
    /// Mask for dictionary presence to be decoded from 16-bit header.
    /// </summary>
    private const int DEF_HEADER_FLAGS_FDICT = 32;
    /// <summary>
    /// Mask for compression level to be decoded from 16-bit header.
    /// </summary>
    private const int DEF_HEADER_FLAGS_FLEVEL = 192;

    /// <summary>
    /// Minimum count of repetions.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_DYNTREE_REPEAT_MINIMUMS = { 3, 3, 11 };

    /// <summary>
    /// Bits, that responds for different repetion modes.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_DYNTREE_REPEAT_BITS = { 2, 3, 7 };

    /// <summary>
    /// Maximum size of the data window.
    /// </summary>
    private const int DEF_MAX_WINDOW_SIZE = UInt16.MaxValue;

    /// <summary>
    /// Length bases.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_REPEAT_LENGTH_BASE =
    {
      3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31,
      35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258
    };

    /// <summary>
    /// Length extended bits count.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_REPEAT_LENGTH_EXTENSION =
      {
        0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
        3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
      };

    /// <summary>
    /// Distance bases.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_REPEAT_DISTANCE_BASE =
      {
        1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193,
        257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145,
        8193, 12289, 16385, 24577
      };

    /// <summary>
    /// Distance extanded bits count.
    /// </summary>
    private static readonly int[] DEF_HUFFMAN_REPEAT_DISTANCE_EXTENSION =
      {
        0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6,
        7, 7, 8, 8, 9, 9, 10, 10, 11, 11,
        12, 12, 13, 13
      };

    /// <summary>
    /// Maximum length of the repeatable block.
    /// </summary>
    private const int DEF_HUFFMAN_REPEATE_MAX = 258;
    /// <summary>
    /// End of the block sign.
    /// </summary>
    private const int DEF_HUFFMAN_END_BLOCK = 256;
    /// <summary>
    /// Minimal length code.
    /// </summary>
    private const int DEF_HUFFMAN_LENGTH_MINIMUM_CODE = 257;
    /// <summary>
    /// Maximal length code.
    /// </summary>
    private const int DEF_HUFFMAN_LENGTH_MAXIMUM_CODE = 285;
    /// <summary>
    /// Maximal distance code.
    /// </summary>
    private const int DEF_HUFFMAN_DISTANCE_MAXIMUM_CODE = 29;
    #endregion

    #region Class Members
    /// <summary>
    /// Input stream.
    /// </summary>
    private Stream m_InputStream;

    /// <summary>
    /// Currently calculated checksum, 
    /// based on Adler32 algorithm.
    /// </summary>
    private long m_CheckSum = 1;

    /// <summary>
    /// Currently read 4 bytes.
    /// </summary>
    private uint m_Buffer;

    /// <summary>
    /// Count of bits that are in buffer.
    /// </summary>
    private int m_BufferedBits;

    /// <summary>
    /// Temporary buffer.
    /// </summary>
    private byte[] m_temp_buffer = new byte[ 4 ];

    /// <summary>
    /// 32k buffer for unpacked data.
    /// </summary>
    private byte[] m_Block_Buffer = new byte[ DEF_MAX_WINDOW_SIZE ];

    /// <summary>
    /// No wrap mode.
    /// </summary>
    private bool m_bNoWrap;

    /// <summary>
    /// Window size, can not be larger than 32k.
    /// </summary>
    private int m_WindowSize;

    /// <summary>
    /// Current position in output stream.
    /// Current in-block position can be extracted by applying Int16.MaxValue mask.
    /// </summary>
    private long m_CurrentPosition;

    /// <summary>
    /// Data length.
    /// Current in-block position can be extracted by applying Int16.MaxValue mask.
    /// </summary>
    private long m_DataLength;

    /// <summary>
    /// Sign of uncompressed data reading.
    /// </summary>
    private bool m_bReadingUncompressed;

    /// <summary>
    /// Size of the block with uncompressed data.
    /// </summary>
    private int m_UncompressedDataLength;

    /// <summary>
    /// Specifies wheather next block can to be read.
    /// Reading can be denied because the header of the last block have been read.
    /// </summary>
    private bool m_bCanReadNextBlock = true;

    /// <summary>
    /// Specifies wheather user can read more data from stream.
    /// </summary>
    private bool m_bCanReadMoreData = true;

    /// <summary>
    /// Current lengths huffman tree.
    /// </summary>
    private DecompressorHuffmanTree m_CurrentLengthTree;

    /// <summary>
    /// Current distances huffman tree.
    /// </summary>
    private DecompressorHuffmanTree m_CurrentDistanceTree;

    /// <summary>
    /// Specifies wheather checksum has been read.
    /// </summary>
    private bool m_bCheckSumRead;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    /// <param name="stream"/>
    public CompressedStreamReader( Stream stream )
      : this( stream, false )
    {
    }

    /// <summary>
    /// Creates new reader for streams with compressed data.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bNoWrap"></param>
    public CompressedStreamReader( Stream stream, bool bNoWrap )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( stream.Length == 0 )
        throw new ArgumentException( "stream - string can not be empty" );

      m_InputStream = stream;
      m_bNoWrap = bNoWrap;

      if( !m_bNoWrap )
      {
        ReadZLibHeader();
      }

      DecodeBlockHeader();
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// GET count of bits available
    /// </summary>
    protected internal int AvailableBits
    {
      get
      {
        return m_BufferedBits;
      }
    }

    /// <summary>
    /// Get count of full bytes available.
    /// </summary>
    protected internal long AvailableBytes
    {
      get
      {
        return m_InputStream.Length - m_InputStream.Position + m_BufferedBits >> 3;
      }
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Resets current checksum to 1.
    /// </summary>
    protected void ChecksumReset()
    {
      m_CheckSum = 1;
    }

    /// <summary>
    /// Updates checksum by calculating checksum of the 
    /// given buffer and adding it to current value.
    /// </summary>
    /// <param name="buffer">Data byte array.</param>
    /// <param name="offset">Offset in the buffer.</param>
    /// <param name="length">Length of data to be used from the stream.</param>
    protected void ChecksumUpdate( byte[] buffer, int offset, int length )
    {
      ChecksumCalculator.ChecksumUpdate( ref m_CheckSum, buffer, offset, length );
    }

    /// <summary>
    /// Discards left-most partially used byte.
    /// </summary>
    protected internal void SkipToBoundary()
    {
      m_Buffer >>= ( m_BufferedBits & 7 );
      m_BufferedBits &= ~7;
    }

    /// <summary>
    /// Reads array of bytes.
    /// </summary>
    /// <param name="buffer">Output buffer.</param>
    /// <param name="offset">Offset in output buffer.</param>
    /// <param name="length">Length of the data to be read.</param>
    /// <returns>Count of bytes actually read to the buffer.</returns>
    protected internal int ReadPackedBytes( byte[] buffer, int offset, int length )
    {
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( offset < 0 || offset > buffer.Length - 1 )
        throw new ArgumentOutOfRangeException( "offset", "Offset can not be less than zero or greater than buffer length - 1." );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length", "Length can not be less than zero." );

      if( length > buffer.Length - offset )
        throw new ArgumentOutOfRangeException( "length", "Length is too large." );

      if( ( m_BufferedBits & 7 ) != 0 )
        throw new NotSupportedException( "Reading of unalligned data is not supported." );

      if( length == 0 ) return 0;

      int result = 0;

      while( m_BufferedBits > 0 && length > 0 )
      {
        buffer[ offset++ ] = ( byte )( m_Buffer );
        m_BufferedBits -= 8;
        m_Buffer >>= 8;
        length--;
        result++;
      }

      if( length > 0 )
      {
        result += m_InputStream.Read( buffer, offset, length );
      }

      return result;
    }

    /// <summary>
    /// Fill`s empty parts of the buffer.
    /// </summary>
    protected void FillBuffer()
    {
      int length = 4 - ( m_BufferedBits >> 3 ) -
        ( ( ( m_BufferedBits & 7 ) != 0 ) ? 1 : 0 );

      if( length == 0 )
      {
        return;
      }

      int bytesRead = m_InputStream.Read( m_temp_buffer, 0, length );

      for( int i = 0; i < bytesRead; i++ )
      {
        m_Buffer |= ( ( uint )m_temp_buffer[ i ] << m_BufferedBits );
        m_BufferedBits += 8;
      }
    }

    /// <summary>
    /// Reads specified count of bits without adjusting position.
    /// </summary>
    /// <param name="count">Count of bits to be read.</param>
    /// <returns>Read value.</returns>
    protected internal int PeekBits( int count )
    {
      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count", "Bits count can not be less than zero." );

      if( count > 32 )
        throw new ArgumentOutOfRangeException( "count", "Count of bits is too large." );

      // If buffered data is not enough to give result, 
      // fill buffer.
      if( m_BufferedBits < count )
      {
        FillBuffer();
      }

      // If you want to read 4 bytes and there is partial data in
      // buffer, than you will fail.
      if( m_BufferedBits < count )
      {
        return -1;
      }

      // Create bitmask for reading of count bits
      uint bitMask = ~( uint.MaxValue << count );

      int result = ( int )( m_Buffer & bitMask );

      //Debug.WriteLine( /*new string( ' ', 32 - m_BufferedBits + (int)( ( 32 - m_BufferedBits ) / 8 ) ) + BitsToString( (int)m_Buffer, m_BufferedBits ) + "  " + */BitsToString( result, count ) );

      return result;
    }

    /// <summary>
    /// Skips specified count of bits.
    /// </summary>
    /// <param name="count">Count of bits to be skipped.</param>
    protected internal void SkipBits( int count )
    {
      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count", "Bits count can not be less than zero." );

      if( count == 0 )
        return;

      if( count >= m_BufferedBits )
      {
        count -= m_BufferedBits;
        m_BufferedBits = 0;
        m_Buffer = 0;

        // if something left, skip it.
        if( count > 0 )
        {
          // Skip entire bytes.
          m_InputStream.Position += ( count >> 3 );
          count &= 7;

          // Skip bits.
          if( count > 0 )
          {
            FillBuffer();
            m_BufferedBits -= count;
            m_Buffer >>= count;
          }
        }
      }
      else
      {
        m_BufferedBits -= count;
        m_Buffer >>= count;
      }
    }

    /// <summary>
    /// Reads specified count of bits from stream.
    /// </summary>
    /// <param name="count">Count of bits to be read.</param>
    /// <returns>
    /// TODO: place correct comment here
    /// </returns>
    protected internal int ReadBits( int count )
    {
      int result = PeekBits( count );

      if( result == -1 )
      {
        return -1;
      }

      m_BufferedBits -= count;
      m_Buffer >>= count;
      return result;
    }

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    /// <returns>
    /// TODO: place correct comment here
    /// </returns>
    protected internal int ReadInt16()
    {
      int result = ( ReadBits( 8 ) << 8 );
      result |= ReadBits( 8 );
      return result;
    }

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    /// <returns>
    /// TODO: place correct comment here
    /// </returns>
    protected internal int ReadInt16Inverted()
    {
      int result = ( ReadBits( 8 ) );
      result |= ReadBits( 8 ) << 8;
      return result;
    }

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    /// <returns>
    /// TODO: place correct comment here
    /// </returns>
    protected internal long ReadInt32()
    {
      long result = ( uint )( ReadBits( 8 ) << 24 );
      result |= ( uint )( ReadBits( 8 ) << 16 );
      result |= ( uint )( ReadBits( 8 ) << 8 );
      result |= ( uint )ReadBits( 8 );
      return result;
    }

    /// <summary>
    /// Reads ZLib header with compression method and flags.
    /// </summary>
    protected void ReadZLibHeader()
    {
      // first 8 bits - compression Method and flags
      // 8 other - flags 
      int header = ReadInt16();

      //Debug.WriteLine( BitsToString( header ) );

      if( header == -1 )
        throw new Exception( "Header of the stream can not be read." );

      if( header % 31 != 0 )
        throw new FormatException( "Header checksum illegal" );

      if( ( header & DEF_HEADER_METHOD_MASK ) != ( 8 << 8 ) )
        throw new FormatException( "Unsupported compression method." );

      m_WindowSize = ( int )Math.Pow( 2, ( ( header & DEF_HEADER_INFO_MASK ) >> 12 ) + 8 );

      if( m_WindowSize > UInt16.MaxValue )
        throw new FormatException( "Unsupported window size for deflate compression method." );

      if( ( header & DEF_HEADER_FLAGS_FDICT ) >> 5 == 1 )
      {
        // Get dictionary.
        throw new NotImplementedException( "Custom dictionary is not supported at the moment." );
      }

    }

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    /// <returns>
    /// TODO: place correct comment here
    /// </returns>
    /// <param name="bits"/>
    /// <param name="count"/>
    protected string BitsToString( int bits, int count )
    {
      string result = "";

      for( int i = 0; i < count; i++ )
      {
        if( ( i & 7 ) == 0 )
        {
          result = " " + result;
        }

        result = ( bits & 1 ).ToString() + result;
        bits >>= 1;
      }

      return result;
    }

    /// <summary>
    /// Reades dynamic huffman codes from block header.
    /// </summary>
    /// <param name="lengthTree">Literals/Lengths tree.</param>
    /// <param name="distanceTree">Distances tree.</param>
    protected void DecodeDynHeader( out DecompressorHuffmanTree lengthTree, out DecompressorHuffmanTree distanceTree )
    {
      byte[] arrDecoderCodeLengths;
      byte[] arrResultingCodeLengths;

      byte bLastSymbol = 0;
      int iLengthsCount = ReadBits( 5 );
      int iDistancesCount = ReadBits( 5 );
      int iCodeLengthsCount = ReadBits( 4 );

      if( iLengthsCount < 0 || iDistancesCount < 0 || iCodeLengthsCount < 0 )
        throw new FormatException( "Wrong dynamic huffman codes." );

      iLengthsCount += 257;
      iDistancesCount += 1;

      int iResultingCodeLengthsCount = iLengthsCount + iDistancesCount;
      arrResultingCodeLengths = new byte[ iResultingCodeLengthsCount ];
      arrDecoderCodeLengths = new byte[ 19 ];
      iCodeLengthsCount += 4;
      int iCurrentCode = 0;

      while( iCurrentCode < iCodeLengthsCount )
      {
        int len = ReadBits( 3 );

        if( len < 0 )
          throw new FormatException( "Wrong dynamic huffman codes." );

        arrDecoderCodeLengths[ Utils.DEF_HUFFMAN_DYNTREE_CODELENGTHS_ORDER[ iCurrentCode++ ] ] =
          ( byte )len;
      }

      DecompressorHuffmanTree treeInternalDecoder = new
        DecompressorHuffmanTree( arrDecoderCodeLengths );

      iCurrentCode = 0;

      for( ; ; )
      {
        int symbol;
        bool bNeedBreak = false;

        while( ( ( symbol = treeInternalDecoder.UnpackSymbol( this ) ) & ~15 ) == 0 )
        {
          arrResultingCodeLengths[ iCurrentCode++ ] = bLastSymbol = ( byte )symbol;

          if( iCurrentCode == iResultingCodeLengthsCount )
          {
            bNeedBreak = true;
            break;
          }
        }

        if( bNeedBreak ) break;

        if( symbol < 0 )
          throw new FormatException( "Wrong dynamic huffman codes." );

        if( symbol >= 17 )
        {
          bLastSymbol = 0;
        }
        else if( iCurrentCode == 0 )
        {
          throw new FormatException( "Wrong dynamic huffman codes." );
        }

        int m_iRepSymbol = symbol - 16;
        int bits = DEF_HUFFMAN_DYNTREE_REPEAT_BITS[ m_iRepSymbol ];

        int count = ReadBits( bits );

        if( count < 0 )
          throw new FormatException( "Wrong dynamic huffman codes." );

        count += DEF_HUFFMAN_DYNTREE_REPEAT_MINIMUMS[ m_iRepSymbol ];

        if( iCurrentCode + count > iResultingCodeLengthsCount )
          throw new FormatException( "Wrong dynamic huffman codes." );

        while( count-- > 0 )
        {
          arrResultingCodeLengths[ iCurrentCode++ ] = bLastSymbol;
        }

        if( iCurrentCode == iResultingCodeLengthsCount ) break;
      }

      byte[] tempArray = new byte[iLengthsCount];
      Array.Copy( arrResultingCodeLengths, 0, tempArray, 0, iLengthsCount );
      lengthTree = new DecompressorHuffmanTree( tempArray );

      tempArray = new byte[iDistancesCount];
      Array.Copy( arrResultingCodeLengths, iLengthsCount, tempArray, 0, iDistancesCount );
      distanceTree = new DecompressorHuffmanTree( tempArray );

    }

    /// <summary>
    /// Reads and decodes block of data.
    /// </summary>
    /// <returns>True if buffer was empty and new data was read, otherwise - False.</returns>
    protected bool DecodeBlockHeader()
    {
      if( !m_bCanReadNextBlock )
      {
        return false;
      }

      int bFinalBlock = ReadBits( 1 );
      if( bFinalBlock == -1 )
      {
        return false;
      }

      int blockType = ReadBits( 2 );
      if( blockType == -1 )
      {
        return false;
      }

      m_bCanReadNextBlock = ( bFinalBlock == 0 );
      //      ChecksumReset();

      switch( blockType )
      {
        case 0:
          // Uncompressed data
          m_bReadingUncompressed = true;

          SkipToBoundary();
          int length = ReadInt16Inverted();
          int lengthComplement = ReadInt16Inverted();

          if( length != ( lengthComplement ^ 0xffff ) )
            throw new FormatException( "Wrong block length." );

          if( length > UInt16.MaxValue )
            throw new FormatException( "Uncompressed block length can not be more than 65535." );

          m_UncompressedDataLength = length;
          m_CurrentLengthTree = null;
          m_CurrentDistanceTree = null;
          break;

        case 1:
          // Compressed data with fixed huffman codes.
          m_bReadingUncompressed = false;
          m_UncompressedDataLength = -1;
          m_CurrentLengthTree = DecompressorHuffmanTree.LengthTree;
          m_CurrentDistanceTree = DecompressorHuffmanTree.DistanceTree;
          break;

        case 2:
          // Compressed data with dynamic huffman codes.
          m_bReadingUncompressed = false;
          m_UncompressedDataLength = -1;
          DecodeDynHeader( out m_CurrentLengthTree, out m_CurrentDistanceTree );
          break;

        default:
          throw new FormatException( "Wrong block type." );
      }

      return true;
    }

    /// <summary>
    /// Decodes huffman codes.
    /// </summary>
    /// <returns>True if some data was read.</returns>
    private bool ReadHuffman()
    {
      int free = DEF_MAX_WINDOW_SIZE - ( int )( m_DataLength - m_CurrentPosition );
      bool dataRead = false;
      //long maxdistance = DEF_MAX_WINDOW_SIZE >> 1;

      // DEF_HUFFMAN_REPEATE_MAX - longest repeatable block, we should always reserve space for it because
      // if we should not, we will have buffer overrun.
      while( free >= DEF_HUFFMAN_REPEATE_MAX )
      {
        int symbol;

        // Only codes 0..255 are valid independent symbols.
        while( ( ( symbol = m_CurrentLengthTree.UnpackSymbol( this ) ) & ~0xff ) == 0 )
        {
          m_Block_Buffer[ m_DataLength++ % DEF_MAX_WINDOW_SIZE ] = ( byte )symbol;
          dataRead = true;

          if( --free < DEF_HUFFMAN_REPEATE_MAX )
          {
            return true;
          }

          //if( (m_DataLength - m_CurrentPosition ) < maxdistance ) return true;
        }

        if( symbol < DEF_HUFFMAN_LENGTH_MINIMUM_CODE )
        {
          if( symbol < DEF_HUFFMAN_END_BLOCK )
            throw new FormatException( "Illegal code." );

          return ( dataRead | ( m_bCanReadMoreData = DecodeBlockHeader() ) );
        }

        if( symbol > DEF_HUFFMAN_LENGTH_MAXIMUM_CODE )
          throw new FormatException( "Illegal repeat code length." );

        int iRepeatLength = DEF_HUFFMAN_REPEAT_LENGTH_BASE
          [ symbol - DEF_HUFFMAN_LENGTH_MINIMUM_CODE ];

        int iRepeatExtraBits = DEF_HUFFMAN_REPEAT_LENGTH_EXTENSION
          [ symbol - DEF_HUFFMAN_LENGTH_MINIMUM_CODE ];

        if( iRepeatExtraBits > 0 )
        {
          int extra = ReadBits( iRepeatExtraBits );

          if( extra < 0 )
            throw new FormatException( "Wrong data." );

          iRepeatLength += extra;
        }

        // Unpack repeat distance.
        symbol = m_CurrentDistanceTree.UnpackSymbol( this );

        if( symbol < 0 || symbol > DEF_HUFFMAN_REPEAT_DISTANCE_BASE.Length )
          throw new FormatException( "Wrong distance code." );

        int iRepeatDistance = DEF_HUFFMAN_REPEAT_DISTANCE_BASE[ symbol ];
        iRepeatExtraBits = DEF_HUFFMAN_REPEAT_DISTANCE_EXTENSION[ symbol ];

        if( iRepeatExtraBits > 0 )
        {
          int extra = ReadBits( iRepeatExtraBits );

          if( extra < 0 )
            throw new FormatException( "Wrong data." );

          iRepeatDistance += extra;
        }

        // Copy data in slow repeat mode
        for( int i = 0; i < iRepeatLength; i++ )
        {
          m_Block_Buffer[ m_DataLength % DEF_MAX_WINDOW_SIZE ] =
            m_Block_Buffer[ ( m_DataLength - ( long )iRepeatDistance ) % DEF_MAX_WINDOW_SIZE ];
          m_DataLength++;
          free--;
        }

        dataRead = true;

      }

      return dataRead;
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Reads data to buffer.
    /// </summary>
    /// <param name="buffer">Output buffer for data.</param>
    /// <param name="offset">Offset in output data.</param>
    /// <param name="length">Length of the data to be read.</param>
    /// <returns>Count of bytes actually read.</returns>
    public int Read( byte[] buffer, int offset, int length )
    {
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( offset < 0 || offset > buffer.Length - 1 )
        throw new ArgumentOutOfRangeException( "offset", "Offset does not belong to specified buffer." );

      if( length < 0 || length > buffer.Length - offset )
        throw new ArgumentOutOfRangeException( "length", "Length is illegal." );

      int initialLength = length;

      while( length > 0 )
      {
        // Read from internal buffer.
        if( m_CurrentPosition < m_DataLength )
        {
          // Position in buffer array.
          int inBlockPosition = ( int )( m_CurrentPosition % DEF_MAX_WINDOW_SIZE );
          // We can not read more than we have in buffer at once,
          // and we not read more than till the array end.
          int dataToCopy = Math.Min( DEF_MAX_WINDOW_SIZE - inBlockPosition, ( int )( m_DataLength - m_CurrentPosition ) );
          // Reading not more, than the rest of the buffer.
          dataToCopy = Math.Min( dataToCopy, length );

          // Copy data.
          Array.Copy( m_Block_Buffer, inBlockPosition, buffer, offset, dataToCopy );
          // Correct position, length, 
          m_CurrentPosition += ( long )dataToCopy;
          offset += dataToCopy;
          length -= dataToCopy;
        }
        else
        {
          Debug.Assert( m_CurrentPosition == m_DataLength, "Wrong position",
                        "Current position is lerger than data length, so something is wrong." );

          if( !m_bCanReadMoreData )
          {
            break;
          }

          long oldDataLength = m_DataLength;

          if( !m_bReadingUncompressed )
          {
            if( !ReadHuffman() )
            {
              break;
            }
          }
          else
          {
            if( m_UncompressedDataLength == 0 )
            {
              // If there is no more data in stream, just exit.
              if( !( m_bCanReadMoreData = DecodeBlockHeader() ) )
              {
                break;
              }
            }
            else
            {
              // Position of the data end in block buffer.
              int inBlockPosition = ( int )( m_DataLength % DEF_MAX_WINDOW_SIZE );
              int dataToRead = Math.Min( m_UncompressedDataLength, DEF_MAX_WINDOW_SIZE - inBlockPosition );
              int dataRead = ReadPackedBytes( m_Block_Buffer, inBlockPosition, dataToRead );

              if( dataToRead != dataRead )
                throw new FormatException( "Not enough data in stream." );

              m_UncompressedDataLength -= dataRead;

              m_DataLength += ( long )dataRead;
            }
          }

          Debug.Assert( ( m_DataLength - m_CurrentPosition ) <= DEF_MAX_WINDOW_SIZE, "Wrong position",
                        "Unread block is larger than 32k, this is wrong behaviour, because some data is lost." );

          if( oldDataLength < m_DataLength )
          {
            int start = ( int )( oldDataLength % DEF_MAX_WINDOW_SIZE );
            int end = ( int )( m_DataLength % DEF_MAX_WINDOW_SIZE );

            if( start < end )
            {
              ChecksumUpdate( m_Block_Buffer, start, end - start );
            }
            else
            {
              ChecksumUpdate( m_Block_Buffer, start, DEF_MAX_WINDOW_SIZE - start );

              if( end > 0 )
              {
                ChecksumUpdate( m_Block_Buffer, 0, end );
              }
            }
          }
        }
      }

      if( !m_bCanReadMoreData && !m_bCheckSumRead && !m_bNoWrap )
      {
        SkipToBoundary();
        long checkSum = ReadInt32();

        //Debug.Assert( checkSum == m_CheckSum, "" );
        if( checkSum != m_CheckSum )
          throw new Exception( "Checksum check failed." );

        m_bCheckSumRead = true;
      }

      return initialLength - length;
    }
    #endregion
  }
}