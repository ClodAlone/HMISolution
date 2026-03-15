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
  /// Compression level.
  /// </summary>
  public enum CompressionLevel
  {
    /// <summary>
    /// Pack without compression
    /// </summary>
    NoCompression = 0,
    /// <summary>
    /// Use high speed compression, reduce of data size is low
    /// </summary>
    BestSpeed = 1,
    /// <summary>
    /// Something middle between normal and BestSpeed compressions
    /// </summary>
    BelowNormal = 3,
    /// <summary>
    /// Use normal compression, middle between speed and size
    /// </summary>
    Normal = 5,
    /// <summary>
    /// Pack better but require a little more time
    /// </summary>
    AboveNormal = 7,
    /// <summary>
    /// Use best compression, slow enough
    /// </summary>
    Best = 9
  }

  /// <summary>
  /// Represents the compressed stream writer
  /// </summary>
  public class CompressedStreamWriter
  {
    #region Private Enums
    /// <summary>
    /// Type of the block.
    /// </summary>
    private enum BlockType
    {
      /// <summary>
      /// Data simply stored as is
      /// </summary>
      Stored = 0,
      /// <summary>
      /// An option to use Fixed Huffman tree codes
      /// </summary>
      FixedHuffmanCodes = 1,
      /// <summary>
      /// An option to use Dynamically built Huffman codes
      /// </summary>
      DynamicHuffmanCodes = 2
    }
    #endregion

    #region Class constants
    /// <summary>
    /// Start template of the zlib header.
    /// </summary>                               
    private const int DEF_ZLIB_HEADER_TEMPLATE = ( 8 + ( 7 << 4 ) ) << 8;
    /// <summary>
    /// Memory usage level.
    /// </summary>
    private const int DEFAULT_MEM_LEVEL = 8;
    /// <summary>
    /// Size of the pending buffer.
    /// </summary>
    private const int DEF_PENDING_BUFFER_SIZE = 1 << ( DEFAULT_MEM_LEVEL + 8 );
    /// <summary>
    /// Size of the buffer for the huffman encoding.
    /// </summary>
    private const int DEF_HUFFMAN_BUFFER_SIZE = 1 << ( DEFAULT_MEM_LEVEL + 6 );

    /// <summary>
    /// Length of the literal alphabet(literal+lengths).
    /// </summary>
    private const int DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH = 286;
    /// <summary>
    /// Distances alphabet length.
    /// </summary>
    private const int DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH = 30;
    /// <summary>
    /// Length of the code-lengths tree.
    /// </summary>
    private const int DEF_HUFFMAN_BITLEN_TREE_LENGTH = 19;
    /// <summary>
    /// Code of the symbol, than means the end of the block.
    /// </summary>
    private const int DEF_HUFFMAN_ENDBLOCK_SYMBOL = 256;

    private const int TOO_FAR = 4096;
    /// <summary>
    /// Maximum window size.
    /// </summary>		
    private const int WSIZE = 1 << 15;
    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int WMASK = WSIZE - 1;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int HASH_BITS = DEFAULT_MEM_LEVEL + 7;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int HASH_SIZE = 1 << HASH_BITS;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int HASH_MASK = HASH_SIZE - 1;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int MAX_MATCH = 258;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int MIN_MATCH = 3;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int HASH_SHIFT = ( HASH_BITS + MIN_MATCH - 1 ) / MIN_MATCH;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int MIN_LOOKAHEAD = MAX_MATCH + MIN_MATCH + 1;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public const int MAX_DIST = WSIZE - MIN_LOOKAHEAD;

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int[] GOOD_LENGTH = {0, 4, 4, 4, 4, 8, 8, 8, 32, 32};

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int[] MAX_LAZY = {0, 4, 5, 6, 4, 16, 16, 32, 128, 258};

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int[] NICE_LENGTH = {0, 8, 16, 32, 16, 32, 128, 128, 258, 258};

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int[] MAX_CHAIN = {0, 4, 8, 32, 16, 32, 128, 256, 1024, 4096};

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int[] COMPR_FUNC = {0, 1, 1, 1, 1, 2, 2, 2, 2, 2};

    /// <summary>
    /// Internal compression engine constant
    /// </summary>		
    public static int MAX_BLOCK_SIZE = Math.Min( 65535, DEF_PENDING_BUFFER_SIZE - 5 );
    #endregion

    #region Class members
    /// <summary>
    /// Output stream.
    /// </summary>
    private Stream m_stream;

    /// <summary>
    /// Pending buffer for writing.
    /// </summary>
    private byte[] m_PendingBuffer = new byte[ DEF_PENDING_BUFFER_SIZE ];

    /// <summary>
    /// Length of the unflushed data.
    /// </summary>
    private int m_PendingBufferLength;

    /// <summary>
    /// Bits cache for pending buffer.
    /// </summary>
    private uint m_PendingBufferBitsCache;

    /// <summary>
    /// Count of bits in pending buffer cache.
    /// </summary>
    private int m_PendingBufferBitsInCache;

    /// <summary>
    /// If true, no zlib header will be written to the stream.
    /// </summary>
    private bool m_bNoWrap;

    /// <summary>
    /// Current checksum.
    /// </summary>
    private long m_CheckSum = 1;

    /// <summary>
    /// Current compression level.
    /// </summary>
    private CompressionLevel m_Level;

    /// <summary>
    /// Current tree for literals.
    /// </summary>
    private CompressorHuffmanTree m_treeLiteral;

    /// <summary>
    /// Current tree for distances.
    /// </summary>
    private CompressorHuffmanTree m_treeDistances;

    /// <summary>
    /// Current tree for code lengths.
    /// </summary>
    private CompressorHuffmanTree m_treeCodeLengths;

    /// <summary>
    /// Current position in literals and distances buffer.
    /// </summary>
    private int m_iBufferPosition;

    /// <summary>
    /// Recorded literals buffer.
    /// </summary>
    private byte[] m_arrLiteralsBuffer;

    /// <summary>
    /// Recorded distances buffer.
    /// </summary>
    private short[] m_arrDistancesBuffer;

    /// <summary>
    /// Count of the extra bits.
    /// </summary>
    private int m_iExtraBits;

    /// <summary>
    /// Static array of the literal codes.
    /// </summary>
    private static short[] m_arrLiteralCodes;

    /// <summary>
    /// Static array of the lengths of the literal codes.
    /// </summary>
    private static byte[] m_arrLiteralLengths;

    /// <summary>
    /// Static array of the distance codes.
    /// </summary>
    private static short[] m_arrDistanceCodes;

    /// <summary>
    /// Static array of the lengths of the distance codes.
    /// </summary>
    private static byte[] m_arrDistanceLengths;

    /// <summary>
    /// If true, no futher writings can be performed.
    /// </summary>
    private bool m_bStreamClosed;

    /// <summary>
    /// Current hash.
    /// </summary>
    private int m_CurrentHash;

    /// <summary>
    /// Hash m_HashHead.
    /// </summary>
    private short[] m_HashHead;

    /// <summary>
    /// Previous hashes.
    /// </summary>
    private short[] m_HashPrevious;

    /// <summary>
    /// Start of the matched part.
    /// </summary>
    private int m_MatchStart;

    /// <summary>
    /// Length of the matched part.
    /// </summary>
    private int m_MatchLength;

    /// <summary>
    /// Previous match available.
    /// </summary>
    private bool m_MatchPreviousAvailable;

    /// <summary>
    /// Start of the data window.
    /// </summary>
    private int m_BlockStart;

    /// <summary>
    /// String start in data window.
    /// </summary>
    private int m_StringStart;

    /// <summary>
    /// Lookahead.
    /// </summary>
    private int m_LookAhead;

    /// <summary>
    /// Data window.
    /// </summary>
    private byte[] m_DataWindow;

    /// <summary>
    /// Maximum chain length.
    /// </summary>
    private int m_MaximumChainLength;

    /// <summary>
    /// Maximum distance of the search with "lazy" algotithm.
    /// </summary>
    private int m_MaximumLazySearch;

    /// <summary>
    /// Nice length of the block.
    /// </summary>
    private int m_NiceLength;

    /// <summary>
    /// Good length of the block.
    /// </summary>
    private int m_GoodLength;

    /// <summary>
    /// Current compression function.
    /// </summary>
    private int m_CompressionFunction;

    /// <summary>
    /// Current block of the data to be compressed.
    /// </summary>
    private byte[] m_InputBuffer;

    /// <summary>
    /// Total count of bytes, that were compressed.
    /// </summary>
    private int m_TotalBytesIn;

    /// <summary>
    /// Offset in the input buffer, where input starts.
    /// </summary>
    private int m_InputOffset;

    /// <summary>
    /// Offset in the input buffer, where input ends.
    /// </summary>
    private int m_InputEnd;

    /// <summary>
    /// If true, stream will be closed after the last block.
    /// </summary>
    private bool m_bCloseStream;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes statical data for huffman compression.
    /// </summary>
    static CompressedStreamWriter()
    {
      // Create and fill arrays with literal codes and their lengths.
      m_arrLiteralCodes  = new short[ DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH ];
      m_arrLiteralLengths = new byte[ DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH ];
      int i = 0;

      while( i < 144 )
      {
        m_arrLiteralCodes[ i ] = Utils.BitReverse( ( 0x030 + i ) << 8 );
        m_arrLiteralLengths[ i++ ] = 8;
      }

      while( i < 256 )
      {
        m_arrLiteralCodes[ i ] = Utils.BitReverse( ( 0x190 - 144 + i ) << 7 );
        m_arrLiteralLengths[ i++ ] = 9;
      }

      while( i < 280 )
      {
        m_arrLiteralCodes[ i ] = Utils.BitReverse( ( 0x000 - 256 + i ) << 9 );
        m_arrLiteralLengths[ i++ ] = 7;
      }

      while( i < DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH )
      {
        m_arrLiteralCodes[ i ] = Utils.BitReverse( ( 0x0c0 - 280 + i ) << 8 );
        m_arrLiteralLengths[ i++ ] = 8;
      }

      // Create and fill arrays with distance codes and their lengths.
      m_arrDistanceCodes = new short[ DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH ];
      m_arrDistanceLengths = new byte[ DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH ];

      for( i = 0; i < DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH; i++ )
      {
        m_arrDistanceCodes[ i ] = Utils.BitReverse( i << 11 );
        m_arrDistanceLengths[ i ] = 5;
      }
    }

    /// <summary>
    /// Initializes compressor and writes ZLib header if needed.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="bNoWrap">If true, ZLib header and checksum will not be written.</param>
    /// <param name="level">Compression level.</param>
    /// <param name="bCloseStream">If true, output stream will be closed after the last block has been written.</param>
    public CompressedStreamWriter( Stream outputStream, bool bNoWrap, CompressionLevel level, bool bCloseStream )
    {
      if( outputStream == null )
        throw new ArgumentNullException( "outputStream" );

      if( !outputStream.CanWrite )
        throw new ArgumentException( "Output stream does not support writing.", "outputStream" );

      m_treeLiteral = new CompressorHuffmanTree( this, DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH, 257, 15 );
      m_treeDistances = new CompressorHuffmanTree( this, DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH, 1, 15 );
      m_treeCodeLengths = new CompressorHuffmanTree( this, DEF_HUFFMAN_BITLEN_TREE_LENGTH, 4, 7 );

      m_arrDistancesBuffer = new short[ DEF_HUFFMAN_BUFFER_SIZE ];
      m_arrLiteralsBuffer = new byte[ DEF_HUFFMAN_BUFFER_SIZE ];

      m_stream = outputStream;
      m_Level = level;
      m_bNoWrap = bNoWrap;
      m_bCloseStream = bCloseStream;

      m_DataWindow = new byte[ 2 * WSIZE ];
      m_HashHead = new short[ HASH_SIZE ];
      m_HashPrevious = new short[ WSIZE ];
      m_BlockStart = m_StringStart = 1;

      m_GoodLength = GOOD_LENGTH[ ( int )level ];
      m_MaximumLazySearch = MAX_LAZY[ ( int )level ];
      m_NiceLength = NICE_LENGTH[ ( int )level ];
      m_MaximumChainLength = MAX_CHAIN[ ( int )level ];
      m_CompressionFunction = COMPR_FUNC[ ( int )level ];

      if( !bNoWrap )
      {
        WriteZLIBHeader();
      }
    }

    /// <summary>
    /// Initializes compressor and writes ZLib header if needed.
    /// Compression level is set to normal.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="bNoWrap">If true, ZLib header and checksum will not be written.</param>
    /// <param name="bCloseStream">If true, output stream will be closed after the last block has been written.</param>
    public CompressedStreamWriter( Stream outputStream, bool bNoWrap, bool bCloseStream )
      : this( outputStream, bNoWrap, CompressionLevel.Normal, bCloseStream )
    {
    }

    /// <summary>
    /// Initializes compressor and writes ZLib header.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="level">Compression level.</param>
    /// <param name="bCloseStream">If true, output stream will be closed after the last block has been written.</param>
    public CompressedStreamWriter( Stream outputStream, CompressionLevel level, bool bCloseStream )
      : this( outputStream, false, level, bCloseStream )
    {
    }

    /// <summary>
    /// Initializes compressor and writes ZLib header.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="bCloseStream">If true, output stream will be closed after the last block has been written.</param>
    public CompressedStreamWriter( Stream outputStream, bool bCloseStream )
      : this( outputStream, false, CompressionLevel.Normal, bCloseStream )
    {
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Compresses data and writes it to the stream.
    /// </summary>
    /// <param name="data">Data to compress</param>
    /// <param name="offset">offset in data array</param>
    /// <param name="length">length of data to compress</param>
    /// <param name="bCloseAfterWrite">True - write last compress block in stream, 
    /// otherwise False</param>
    public void Write( byte[] data, int offset, int length, bool bCloseAfterWrite )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      int end = offset + length;

      if( 0 > offset || offset > end || end > data.Length )
        throw new ArgumentOutOfRangeException( "Offset or length is incorrect." );

      m_InputBuffer = data;
      m_InputOffset = offset;
      m_InputEnd = end;

      if( length == 0 ) return;

      if( m_bStreamClosed )
        throw new IOException( "Stream was closed." );

      ChecksumCalculator.ChecksumUpdate( ref m_CheckSum, m_InputBuffer, m_InputOffset, length );

      while( !NeedsInput || !PendingBufferIsFlushed )
      {
        PendingBufferFlush();

        if( !CompressData( bCloseAfterWrite ) && bCloseAfterWrite )
        {
          PendingBufferFlush();
          PendingBufferAlignToByte();

          if( !m_bNoWrap )
          {
            PendingBufferWriteShortMSB( ( int )( m_CheckSum >> 16 ) );
            PendingBufferWriteShortMSB( ( int )( m_CheckSum & 0xffff ) );
          }

          PendingBufferFlush();
          m_bStreamClosed = true;

          if( m_bCloseStream )
          {
            m_stream.Close();
          }
        }
      }
    }
    public void Close()
    {
      if( m_bStreamClosed )
        return;

      //while( !NeedsInput || !PendingBufferIsFlushed )
      //{
      //  PendingBufferFlush();

        //if( !CompressData( true ) )
      //while( !NeedsInput || !PendingBufferIsFlushed )
      do
      {
        PendingBufferFlush();
        if( !CompressData( true ) )
        {
          PendingBufferFlush();
          PendingBufferAlignToByte();

          if( !m_bNoWrap )
          {
            PendingBufferWriteShortMSB( ( int )( m_CheckSum >> 16 ) );
            PendingBufferWriteShortMSB( ( int )( m_CheckSum & 0xffff ) );
          }

          PendingBufferFlush();
        }
      }
      while( !NeedsInput || !PendingBufferIsFlushed );
      //else
      //{
      //  PendingBufferFlush();
      //  PendingBufferAlignToByte();

      //  if( !m_bNoWrap )
      //  {
      //    PendingBufferWriteShortMSB( ( int )( m_CheckSum >> 16 ) );
      //    PendingBufferWriteShortMSB( ( int )( m_CheckSum & 0xffff ) );
      //  }
      //}

      m_bStreamClosed = true;

      if( m_bCloseStream )
      {
        m_stream.Close();
      }

      //CompressData( true );

      //if( !PendingBufferIsFlushed o
      //  PendingBufferFlush();

      //m_stream.Flush();

      //if( m_bCloseStream )
      //  m_stream.Close();
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Total data processed.
    /// </summary>		
    public int TotalIn
    {
      get
      {
        return m_TotalBytesIn;
      }
    }

    /// <summary>
    /// Return true if input is needed
    /// </summary>		
    private bool NeedsInput
    {
      get
      {
        return m_InputEnd == m_InputOffset;
      }
    }

    /// <summary>
    /// Checks, wheather huffman compression buffer is full.
    /// </summary>
    /// <returns>True if buffer is full.</returns>
    private bool HuffmanIsFull
    {
      get
      {
        return m_iBufferPosition >= DEF_HUFFMAN_BUFFER_SIZE;
      }
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Writes ZLib header to stream.
    /// </summary>
    private void WriteZLIBHeader()
    {
      // Initialize header.
      int iHeaderData = DEF_ZLIB_HEADER_TEMPLATE;

      // Save compression level.
      iHeaderData |= ( ( ( int )m_Level >> 2 ) & 3 ) << 6;

      // Align header.
      iHeaderData += 31 - ( iHeaderData % 31 );

      // Write header to stream.
      PendingBufferWriteShortMSB( iHeaderData );
    }

    /// <summary>
    /// Fill the window
    /// </summary>
    private void FillWindow()
    {
      if( m_StringStart >= WSIZE + MAX_DIST )
      {
        SlideWindow();
      }

      while( m_LookAhead < MIN_LOOKAHEAD && m_InputOffset < m_InputEnd )
      {
        int more = 2 * WSIZE - m_LookAhead - m_StringStart;

        if( more > m_InputEnd - m_InputOffset )
        {
          more = m_InputEnd - m_InputOffset;
        }

        Array.Copy( m_InputBuffer, m_InputOffset, m_DataWindow, m_StringStart + m_LookAhead, more );

        m_InputOffset += more;
        m_TotalBytesIn += more;
        m_LookAhead += more;
      }

      if( m_LookAhead >= MIN_MATCH )
      {
        UpdateHash();
      }
    }

    /// <summary>
    /// Slides current window, and data, associated with it.
    /// </summary>
    private void SlideWindow()
    {
      Array.Copy( m_DataWindow, WSIZE, m_DataWindow, 0, WSIZE );
      m_MatchStart -= WSIZE;
      m_StringStart -= WSIZE;
      m_BlockStart -= WSIZE;

      for( int i = 0; i < HASH_SIZE; ++i )
      {
        int m = m_HashHead[ i ] & 0xffff;
        m_HashHead[ i ] = ( short )( ( m >= WSIZE ) ? ( m - WSIZE ) : 0 );
      }

      for( int i = 0; i < WSIZE; i++ )
      {
        int m = m_HashPrevious[ i ] & 0xffff;
        m_HashPrevious[ i ] = ( short )( ( m >= WSIZE ) ? ( m - WSIZE ) : 0 );
      }
    }

    /// <summary>
    /// Updates hash.
    /// </summary>
    private void UpdateHash()
    {
      m_CurrentHash = ( m_DataWindow[ m_StringStart ] << HASH_SHIFT ) ^ m_DataWindow[ m_StringStart + 1 ];
    }

    /// <summary>
    /// Inserts string to the hash.
    /// </summary>
    /// <returns></returns>
    private int InsertString()
    {
      short match;
      int hash = ( ( m_CurrentHash << HASH_SHIFT ) ^ m_DataWindow[ m_StringStart + ( MIN_MATCH - 1 ) ] ) & HASH_MASK;

      m_HashPrevious[ m_StringStart & WMASK ] = match = m_HashHead[ hash ];
      m_HashHead[ hash ] = ( short )m_StringStart;
      m_CurrentHash = hash;

      return match & 0xffff;
    }

    /// <summary>
    /// Searches for the longest match.
    /// </summary>
    /// <param name="curMatch"></param>
    /// <returns></returns>
    private bool FindLongestMatch( int curMatch )
    {
      int chainLength = this.m_MaximumChainLength;
      int m_NiceLength = this.m_NiceLength;
      short[] m_HashPrevious = this.m_HashPrevious;
      int scan = this.m_StringStart;
      int match;
      int best_end = this.m_StringStart + m_MatchLength;
      int best_len = Math.Max( m_MatchLength, MIN_MATCH - 1 );

      int limit = Math.Max( m_StringStart - MAX_DIST, 0 );

      int strend = m_StringStart + MAX_MATCH - 1;
      byte scan_end1 = m_DataWindow[ best_end - 1 ];
      byte scan_end = m_DataWindow[ best_end ];

      /* Do not waste too much time if we already have a good match: */
      if( best_len >= this.m_GoodLength )
      {
        chainLength >>= 2;
      }

      /* Do not look for matches beyond the end of the input. This is necessary
      * to make deflate deterministic.
      */
      if( m_NiceLength > m_LookAhead )
      {
        m_NiceLength = m_LookAhead;
      }

      do
      {
        if( m_DataWindow[ curMatch + best_len ] != scan_end ||
          m_DataWindow[ curMatch + best_len - 1 ] != scan_end1 ||
          m_DataWindow[ curMatch ] != m_DataWindow[ scan ] ||
          m_DataWindow[ curMatch + 1 ] != m_DataWindow[ scan + 1 ] )
        {
          continue;
        }

        match = curMatch + 2;
        scan += 2;

        /* We check for insufficient m_LookAhead only every 8th comparison;
        * the 256th check will be made at m_StringStart + 258.
        */
        while( m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] &&
          m_DataWindow[ ++scan ] == m_DataWindow[ ++match ] && scan < strend )
        {
          ;
        }

        if( scan > best_end )
        {
          m_MatchStart = curMatch;
          best_end = scan;
          best_len = scan - m_StringStart;

          if( best_len >= m_NiceLength )
          {
            break;
          }

          scan_end1 = m_DataWindow[ best_end - 1 ];
          scan_end = m_DataWindow[ best_end ];
        }
        scan = m_StringStart;
      }
      while( ( curMatch = ( m_HashPrevious[ curMatch & WMASK ] & 0xffff ) ) > limit && --chainLength != 0 );

      m_MatchLength = Math.Min( best_len, m_LookAhead );

      return m_MatchLength >= MIN_MATCH;
    }

    /// <summary>
    /// Store data without compression.
    /// </summary>
    /// <param name="flush"></param>
    /// <param name="finish"></param>
    /// <returns></returns>
    private bool SaveStored( bool flush, bool finish )
    {
      if( !flush && m_LookAhead == 0 )
      {
        return false;
      }

      m_StringStart += m_LookAhead;
      m_LookAhead = 0;

      int storedLen = m_StringStart - m_BlockStart;

      if( ( storedLen >= MAX_BLOCK_SIZE ) ||
        ( m_BlockStart < WSIZE && storedLen >= MAX_DIST ) ||
        flush )
      {
        bool lastBlock = finish;

        if( storedLen > MAX_BLOCK_SIZE )
        {
          storedLen = MAX_BLOCK_SIZE;
          lastBlock = false;
        }

        HuffmanFlushStoredBlock( m_DataWindow, m_BlockStart, storedLen, lastBlock );
        m_BlockStart += storedLen;

        return !lastBlock;
      }
      return true;
    }

    /// <summary>
    /// Compress with a maximum speed.
    /// </summary>
    /// <param name="flush"></param>
    /// <param name="finish"></param>
    /// <returns></returns>
    private bool CompressFast( bool flush, bool finish )
    {
      if( m_LookAhead < MIN_LOOKAHEAD && !flush )
      {
        return false;
      }

      while( m_LookAhead >= MIN_LOOKAHEAD || flush )
      {
        if( m_LookAhead == 0 )
        {
          HuffmanFlushBlock( m_DataWindow, m_BlockStart, m_StringStart - m_BlockStart, finish );
          m_BlockStart = m_StringStart;
          return false;
        }

        if( m_StringStart > 2 * WSIZE - MIN_LOOKAHEAD )
        {
          SlideWindow();
        }

        int hashHead;

        if( m_LookAhead >= MIN_MATCH &&
          ( hashHead = InsertString() ) != 0 &&
          m_StringStart - hashHead <= MAX_DIST &&
          FindLongestMatch( hashHead ) )
        {
          if( HuffmanTallyDist( m_StringStart - m_MatchStart, m_MatchLength ) )
          {
            bool lastBlock = finish && m_LookAhead == 0;
            HuffmanFlushBlock( m_DataWindow, m_BlockStart, m_StringStart - m_BlockStart, lastBlock );
            m_BlockStart = m_StringStart;
          }

          m_LookAhead -= m_MatchLength;

          if( m_MatchLength <= m_MaximumLazySearch && m_LookAhead >= MIN_MATCH )
          {
            while( --m_MatchLength > 0 )
            {
              ++m_StringStart;
              InsertString();
            }

            ++m_StringStart;
          }
          else
          {
            m_StringStart += m_MatchLength;

            if( m_LookAhead >= MIN_MATCH - 1 )
            {
              UpdateHash();
            }
          }

          m_MatchLength = MIN_MATCH - 1;

          continue;
        }
        else
        {
          HuffmanTallyLit( m_DataWindow[ m_StringStart ] & 0xff );
          ++m_StringStart;
          --m_LookAhead;
        }

        if( HuffmanIsFull )
        {
          bool lastBlock = ( finish && m_LookAhead == 0 );
          HuffmanFlushBlock( m_DataWindow, m_BlockStart, m_StringStart - m_BlockStart, lastBlock );
          m_BlockStart = m_StringStart;

          return !lastBlock;
        }
      }
      return true;
    }

    /// <summary>
    /// Compress, using maximum compression level.
    /// </summary>
    /// <param name="flush"></param>
    /// <param name="finish"></param>
    /// <returns></returns>
    private bool CompressSlow( bool flush, bool finish )
    {
      if( m_LookAhead < MIN_LOOKAHEAD && !flush )
      {
        return false;
      }

      while( m_LookAhead >= MIN_LOOKAHEAD || flush )
      {
        if( m_LookAhead == 0 )
        {
          if( m_MatchPreviousAvailable )
          {
            HuffmanTallyLit( m_DataWindow[ m_StringStart - 1 ] & 0xff );
          }

          m_MatchPreviousAvailable = false;

          HuffmanFlushBlock( m_DataWindow, m_BlockStart, m_StringStart - m_BlockStart, finish );

          m_BlockStart = m_StringStart;

          return false;
        }

        if( m_StringStart >= 2 * WSIZE - MIN_LOOKAHEAD )
        {
          SlideWindow();
        }

        int prevMatch = m_MatchStart;
        int prevLen = m_MatchLength;

        if( m_LookAhead >= MIN_MATCH )
        {
          int hashHead = InsertString();

          if( hashHead != 0 && m_StringStart - hashHead <= MAX_DIST && FindLongestMatch( hashHead ) )
          {
            // Discard match if too small and too far away.
            if( m_MatchLength <= 5 && ( m_MatchLength == MIN_MATCH && m_StringStart - m_MatchStart > TOO_FAR ) )
            {
              m_MatchLength = MIN_MATCH - 1;
            }
          }
        }

        // Previous match was better.
        if( prevLen >= MIN_MATCH && m_MatchLength <= prevLen )
        {
          HuffmanTallyDist( m_StringStart - 1 - prevMatch, prevLen );
          prevLen -= 2;

          do
          {
            m_StringStart++;
            m_LookAhead--;

            if( m_LookAhead >= MIN_MATCH )
            {
              InsertString();
            }
          }
          while( --prevLen > 0 );

          m_StringStart++;
          m_LookAhead--;
          m_MatchPreviousAvailable = false;
          m_MatchLength = MIN_MATCH - 1;
        }
        else
        {
          if( m_MatchPreviousAvailable )
          {
            HuffmanTallyLit( m_DataWindow[ m_StringStart - 1 ] & 0xff );
          }

          m_MatchPreviousAvailable = true;
          m_StringStart++;
          m_LookAhead--;
        }

        if( HuffmanIsFull )
        {
          int len = m_StringStart - m_BlockStart;

          if( m_MatchPreviousAvailable )
          {
            len--;
          }

          bool lastBlock = ( finish && m_LookAhead == 0 && !m_MatchPreviousAvailable );
          HuffmanFlushBlock( m_DataWindow, m_BlockStart, len, lastBlock );
          m_BlockStart += len;

          return !lastBlock;
        }
      }

      return true;
    }

    /// <summary>
    /// CompressData drives actual compression of data
    /// </summary>
    private bool CompressData( bool finish )
    {
      bool success;

      do
      {
        FillWindow();

        bool canFlush = ( finish && NeedsInput );

        switch( m_CompressionFunction )
        {
          case 0:
            success = SaveStored( canFlush, finish );
            break;

          case 1:
            success = CompressFast( canFlush, finish );
            break;

          case 2:
            success = CompressSlow( canFlush, finish );
            break;

          default:
            throw new InvalidOperationException( "unknown m_CompressionFunction" );
        }
      }
      while( PendingBufferIsFlushed && success ); /* repeat while we have no pending output and success was made */

      return success;
    }

    /// <summary>
    /// Reset internal state
    /// </summary>		
    private void HuffmanReset()
    {
      m_iBufferPosition = 0;
      m_iExtraBits = 0;
      m_treeLiteral.Reset();
      m_treeDistances.Reset();
      m_treeCodeLengths.Reset();
    }

    /// <summary>
    /// Calculates length code from length.
    /// </summary>
    /// <param name="len">Length.</param>
    /// <returns>Length code.</returns>
    private int HuffmanLengthCode( int len )
    {
      if( len == 255 )
      {
        return 285;
      }

      int code = 257;

      while( len >= 8 )
      {
        code += 4;
        len >>= 1;
      }

      return code + len;
    }

    /// <summary>
    /// Calculates distance code from distance.
    /// </summary>
    /// <param name="distance">Distance.</param>
    /// <returns>Distance code.</returns>
    private int HuffmanDistanceCode( int distance )
    {
      int code = 0;

      while( distance >= 4 )
      {
        code += 2;
        distance >>= 1;
      }
      return code + distance;
    }

    /// <summary>
    /// Write all trees to pending buffer
    /// </summary>		
    private void HuffmanSendAllTrees( int blTreeCodes )
    {
      m_treeCodeLengths.BuildCodes();
      m_treeLiteral.BuildCodes();
      m_treeDistances.BuildCodes();
      PendingBufferWriteBits( m_treeLiteral.TreeLength - 257, 5 );
      PendingBufferWriteBits( m_treeDistances.TreeLength - 1, 5 );
      PendingBufferWriteBits( blTreeCodes - 4, 4 );

      for( int rank = 0; rank < blTreeCodes; rank++ )
      {
        PendingBufferWriteBits(
          m_treeCodeLengths.CodeLengths[ Utils.DEF_HUFFMAN_DYNTREE_CODELENGTHS_ORDER[ rank ] ]
          , 3 );
      }

      m_treeLiteral.WriteTree( m_treeCodeLengths );
      m_treeDistances.WriteTree( m_treeCodeLengths );
    }

    /// <summary>
    /// Compress current buffer writing data to pending buffer
    /// </summary>
    private void HuffmanCompressBlock()
    {
      for( int i = 0; i < m_iBufferPosition; i++ )
      {
        int litlen = m_arrLiteralsBuffer[ i ] & 255;
        int dist = m_arrDistancesBuffer[ i ];

        if( dist-- != 0 )
        {
          int lc = HuffmanLengthCode( litlen );
          m_treeLiteral.WriteCodeToStream( lc );

          int bits = ( lc - 261 ) / 4;
          if( bits > 0 && bits <= 5 )
          {
            PendingBufferWriteBits( litlen & ( ( 1 << bits ) - 1 ), bits );
          }

          int dc = HuffmanDistanceCode( dist );
          m_treeDistances.WriteCodeToStream( dc );

          bits = dc / 2 - 1;
          if( bits > 0 )
          {
            PendingBufferWriteBits( dist & ( ( 1 << bits ) - 1 ), bits );
          }
        }
        else
        {
          m_treeLiteral.WriteCodeToStream( litlen );
        }
      }

      m_treeLiteral.WriteCodeToStream( DEF_HUFFMAN_ENDBLOCK_SYMBOL );
    }

    /// <summary>
    /// Flush block to output with no compression
    /// </summary>
    /// <param name="stored">Data to write</param>
    /// <param name="storedOffset">Index of first byte to write</param>
    /// <param name="storedLength">Count of bytes to write</param>
    /// <param name="lastBlock">True if this is the last block</param>
    private void HuffmanFlushStoredBlock( byte[] stored, int storedOffset, int storedLength, bool lastBlock )
    {
      PendingBufferWriteBits( ( ( int )BlockType.Stored << 1 ) + ( lastBlock ? 1 : 0 ), 3 );
      PendingBufferAlignToByte();
      PendingBufferWriteShort( storedLength );
      PendingBufferWriteShort( ~storedLength );
      PendingBufferWriteByteBlock( stored, storedOffset, storedLength );
      HuffmanReset();
    }

    /// <summary>
    /// Flush block to output with compression
    /// </summary>		
    /// <param name="stored">Data to flush</param>
    /// <param name="storedOffset">Index of first byte to flush</param>
    /// <param name="storedLength">Count of bytes to flush</param>
    /// <param name="lastBlock">True if this is the last block</param>
    private void HuffmanFlushBlock( byte[] stored, int storedOffset, int storedLength, bool lastBlock )
    {
      m_treeLiteral.CodeFrequences[ DEF_HUFFMAN_ENDBLOCK_SYMBOL ]++;

      // Build trees.
      m_treeLiteral.BuildTree();
      m_treeDistances.BuildTree();

      // Calculate bitlen frequency.
      m_treeLiteral.CalcBLFreq( m_treeCodeLengths );
      m_treeDistances.CalcBLFreq( m_treeCodeLengths );

      // Build bitlen tree.
      m_treeCodeLengths.BuildTree();

      int blTreeCodes = 4;
      for( int i = 18; i > blTreeCodes; i-- )
      {
        if( m_treeCodeLengths.CodeLengths[ Utils.DEF_HUFFMAN_DYNTREE_CODELENGTHS_ORDER[ i ] ] > 0 )
        {
          blTreeCodes = i + 1;
        }
      }
      int opt_len = 14 + blTreeCodes * 3 + m_treeCodeLengths.GetEncodedLength() +
        m_treeLiteral.GetEncodedLength() + m_treeDistances.GetEncodedLength() +
        m_iExtraBits;

      int static_len = m_iExtraBits;
      for( int i = 0; i < DEF_HUFFMAN_LITERAL_ALPHABET_LENGTH; i++ )
      {
        static_len += m_treeLiteral.CodeFrequences[ i ] * m_arrLiteralLengths[ i ];
      }
      for( int i = 0; i < DEF_HUFFMAN_DISTANCES_ALPHABET_LENGTH; i++ )
      {
        static_len += m_treeDistances.CodeFrequences[ i ] * m_arrDistanceLengths[ i ];
      }
      if( opt_len >= static_len )
      {
        // Force static trees.
        opt_len = static_len;
      }

      if( storedOffset >= 0 && storedLength + 4 < opt_len >> 3 )
      {
        HuffmanFlushStoredBlock( stored, storedOffset, storedLength, lastBlock );
      }
      else if( opt_len == static_len )
      {
        // Encode with static tree.
        PendingBufferWriteBits( ( ( int )BlockType.FixedHuffmanCodes << 1 ) + ( lastBlock ? 1 : 0 ), 3 );
        m_treeLiteral.SetStaticCodes( m_arrLiteralCodes, m_arrLiteralLengths );
        m_treeDistances.SetStaticCodes( m_arrDistanceCodes, m_arrDistanceLengths );
        HuffmanCompressBlock();
        HuffmanReset();
      }
      else
      {
        // Encode with dynamic tree.
        PendingBufferWriteBits( ( ( int )BlockType.DynamicHuffmanCodes << 1 ) + ( lastBlock ? 1 : 0 ), 3 );
        HuffmanSendAllTrees( blTreeCodes );
        HuffmanCompressBlock();
        HuffmanReset();
      }
    }

    /// <summary>
    /// Add literal to buffer.
    /// </summary>
    /// <param name="literal"></param>
    /// <returns>Value indicating internal buffer is full</returns>
    private bool HuffmanTallyLit( int literal )
    {
      m_arrDistancesBuffer[ m_iBufferPosition ] = 0;
      m_arrLiteralsBuffer[ m_iBufferPosition++ ] = ( byte )literal;
      m_treeLiteral.CodeFrequences[ literal ]++;
      return HuffmanIsFull;
    }

    /// <summary>
    /// Add distance code and length to literal and distance trees
    /// </summary>
    /// <param name="dist">Distance code</param>
    /// <param name="len">Length</param>
    /// <returns>Value indicating if internal buffer is full</returns>
    private bool HuffmanTallyDist( int dist, int len )
    {
      m_arrDistancesBuffer[ m_iBufferPosition ] = ( short )dist;
      m_arrLiteralsBuffer[ m_iBufferPosition++ ] = ( byte )( len - 3 );

      int lc = HuffmanLengthCode( len - 3 );
      m_treeLiteral.CodeFrequences[ lc ]++;
      if( lc >= 265 && lc < 285 )
      {
        m_iExtraBits += ( lc - 261 ) / 4;
      }

      int dc = HuffmanDistanceCode( dist - 1 );
      m_treeDistances.CodeFrequences[ dc ]++;
      if( dc >= 4 )
      {
        m_iExtraBits += dc / 2 - 1;
      }
      return HuffmanIsFull;
    }

    /// <summary>
    /// write a byte to buffer
    /// </summary>
    /// <param name="b">
    /// value to write
    /// </param>
    internal void PendingBufferWriteByte( int b )
    {
      Debug.Assert( m_PendingBufferBitsInCache == 0, "There is data in bits buffer." );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )b;
    }

    /// <summary>
    /// Write a short value to buffer LSB first
    /// </summary>
    /// <param name="s">
    /// value to write
    /// </param>
    internal void PendingBufferWriteShort( int s )
    {
      Debug.Assert( m_PendingBufferBitsInCache == 0, "There is data in bits buffer." );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )s;
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( s >> 8 );
    }

    /// <summary>
    /// write an integer LSB first
    /// </summary>
    /// <param name="s">value to write</param>
    internal void PendingBufferWriteInt( int s )
    {
      Debug.Assert( m_PendingBufferBitsInCache == 0, "There is data in bits buffer." );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )s;
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( s >> 8 );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( s >> 16 );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( s >> 24 );
    }

    /// <summary>
    /// Write a block of data to buffer
    /// </summary>
    /// <param name="data">data to write</param>
    /// <param name="offset">offset of first byte to write</param>
    /// <param name="length">number of bytes to write</param>
    internal void PendingBufferWriteByteBlock( byte[] data, int offset, int length )
    {
      Debug.Assert( m_PendingBufferBitsInCache == 0, "There is data in bits buffer." );
      Array.Copy( data, offset, m_PendingBuffer, m_PendingBufferLength, length );
      m_PendingBufferLength += length;
    }

    /// <summary>
    /// The number of bits written to the buffer
    /// </summary>
    internal int PendingBufferBitCount
    {
      get
      {
        return m_PendingBufferBitsInCache;
      }
    }

    /// <summary>
    /// Align internal buffer on a byte boundary
    /// </summary>
    internal void PendingBufferAlignToByte()
    {
      if( m_PendingBufferBitsInCache > 0 )
      {
        m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )m_PendingBufferBitsCache;

        if( m_PendingBufferBitsInCache > 8 )
        {
          m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( m_PendingBufferBitsCache >> 8 );
        }
      }

      m_PendingBufferBitsCache = 0;
      m_PendingBufferBitsInCache = 0;
    }

    /// <summary>
    /// Write bits to internal buffer
    /// </summary>
    /// <param name="b">source of bits</param>
    /// <param name="count">number of bits to write</param>
    internal void PendingBufferWriteBits( int b, int count )
    {
      m_PendingBufferBitsCache |= ( uint )( b << m_PendingBufferBitsInCache );
      m_PendingBufferBitsInCache += count;

      PendingBufferFlushBits();
    }

    /// <summary>
    /// Write a short value to internal buffer most significant byte first
    /// </summary>
    /// <param name="s">value to write</param>
    internal void PendingBufferWriteShortMSB( int s )
    {
      Debug.Assert( m_PendingBufferBitsInCache == 0, "There is data in bits buffer." );

      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )( s >> 8 );
      m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )s;
    }

    /// <summary>
    /// Indicates if buffer has been flushed
    /// </summary>
    internal bool PendingBufferIsFlushed
    {
      get
      {
        return m_PendingBufferLength == 0;
      }
    }

    /// <summary>
    /// Flushes the pending buffer into the given output array.  If the
    /// output array is to small, only a partial flush is done.
    /// </summary>
    internal void PendingBufferFlush()
    {
      PendingBufferFlushBits();

      m_stream.Write( m_PendingBuffer, 0, m_PendingBufferLength );
      m_PendingBufferLength = 0;
      m_stream.Flush();
    }

    /// <summary>
    /// Flushes fully recorded bytes to buffer array.
    /// </summary>
    /// <returns>Count of bytes, added to buffer.</returns>
    internal int PendingBufferFlushBits()
    {
      int result = 0;

      while( m_PendingBufferBitsInCache >= 8 && m_PendingBufferLength < DEF_PENDING_BUFFER_SIZE )
      {
        m_PendingBuffer[ m_PendingBufferLength++ ] = ( byte )m_PendingBufferBitsCache;
        m_PendingBufferBitsCache >>= 8;
        m_PendingBufferBitsInCache -= 8;
        result++;
      }

      return result;
    }

    /// <summary>
    /// Convert internal buffer to byte array.
    /// Buffer is empty on completion
    /// </summary>
    /// <returns>
    /// converted buffer contents contents
    /// </returns>
    internal byte[] PendingBufferToByteArray()
    {
      byte[] result = new byte[ m_PendingBufferLength - 0 ];
      Array.Copy( m_PendingBuffer, 0, result, 0, result.Length );
      m_PendingBufferLength = 0;

      return result;
    }
    #endregion
  }
}