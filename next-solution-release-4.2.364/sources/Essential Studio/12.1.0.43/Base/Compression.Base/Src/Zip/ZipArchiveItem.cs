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
using System.IO;
using System.Text;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 || (SILVERLIGHT) || WP)
using System.IO.Compression;
#endif

#if !(SILVERLIGHT || WP)
using System.Globalization;
#endif

#endregion

namespace Syncfusion.Compression.Zip
{
  /// <summary>
  /// Represents single item inside zip archive. It can be either folder or file.
  /// </summary>
  public class ZipArchiveItem : IDisposable
  {
    #region Members
    /// <summary>
    /// Name of the archive item.
    /// </summary>
    private string m_strItemName;
    /// <summary>
    /// Compression method.
    /// </summary>
    private CompressionMethod m_compressionMethod = CompressionMethod.Deflated;
    /// <summary>
    /// Compression level.
    /// </summary>
    private CompressionLevel m_compressionLevel = CompressionLevel.Normal;
    /// <summary>
    /// Crc.
    /// </summary>
    private uint m_uiCrc32;
    /// <summary>
    /// Stream with item's data.
    /// </summary>
    private Stream m_streamData;
    /// <summary>
    /// Compressed data size.
    /// </summary>
    private long m_lCompressedSize;
    /// <summary>
    /// Original (not compressed) data size.
    /// </summary>
    private long m_lOriginalSize;
    /// <summary>
    /// Indicates whether this item controls it's data stream.
    /// </summary>
    private bool m_bControlStream;
    /// <summary>
    /// Indicates whether internal stream contains compressed data.
    /// </summary>
    private bool m_bCompressed;
    /// <summary>
    /// Position of the size block inside local file header.
    /// </summary>
    private long m_lCrcPosition;
    /// <summary>
    /// Offset to the local header.
    /// </summary>
    private int m_iLocalHeaderOffset;
    /// <summary>
    /// General purpose bit flag.
    /// </summary>
    private GeneralPurposeBitFlags m_options;
    /// <summary>
    /// Item's external attributes.
    /// </summary>
    private int m_iExternalAttributes;
    /// <summary>
    /// Indicates whether we should check crc value after decompressing item's data.
    /// </summary>
    private bool m_bCheckCrc;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bOptimizedDecompress;
    private ZipArchive m_archive;
    private const int MaxAnsiCode = 255;
    #endregion

    #region Properties
    /// <summary>
    /// Name of the archive item.
    /// </summary>
    public string ItemName
    {
      get
      {
        return m_strItemName;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentOutOfRangeException( "ItemName" );

        m_strItemName = value;
      }
    }
    /// <summary>
    /// Compression method.
    /// </summary>
    public CompressionMethod CompressionMethod
    {
      get
      {
        return m_compressionMethod;
      }
      set
      {
          m_compressionMethod = value;
      }
    }
    /// <summary>
    /// Gets/sets item's compression level.
    /// </summary>
    public CompressionLevel CompressionLevel
    {
      get
      {
        return m_compressionLevel;
      }
      set
      {
        if( m_compressionLevel != value )
        {
          if( m_bCompressed )
          {
            DecompressData();
          }

          m_compressionLevel = value;
        }
      }
    }
    /// <summary>
    /// Crc.
    /// </summary>
    [CLSCompliant( false )]
    public uint Crc32
    {
      get
      {
        return m_uiCrc32;
      }
    }
    /// <summary>
    /// Stream with item's data.
    /// </summary>
    public Stream DataStream
    {
      get
      {
        if( m_bCompressed )
          DecompressData();

        return m_streamData;
      }
    }
    /// <summary>
    /// Compressed data size.
    /// </summary>
    public long CompressedSize
    {
      get
      {
        return m_lCompressedSize;
      }
    }
    /// <summary>
    /// Original (not compressed) data size.
    /// </summary>
    public long OriginalSize
    {
      get
      {
        return m_lOriginalSize;
      }
    }
    /// <summary>
    /// Indicates whether this item controls it's data stream.
    /// </summary>
    public bool ControlStream
    {
      get
      {
        return m_bControlStream;
      }
    }
    /// <summary>
    /// Indicates whether internal stream contains compressed data.
    /// </summary>
    public bool Compressed
    {
      get
      {
        return m_bCompressed;
      }
    }
    /// <summary>
    /// Gets / sets item's external attributes.
    /// </summary>
    public FileAttributes ExternalAttributes
    {
      get
      {
        return ( FileAttributes )m_iExternalAttributes;
      }
      set
      {
        m_iExternalAttributes = ( int )value;
      }
    }
    public bool OptimizedDecompress
    {
      get
      {
        return m_bOptimizedDecompress;
      }
      set
      {
        m_bOptimizedDecompress = value;
      }
    }
#if !(SILVERLIGHT || WP)
    /// <summary>
    /// Gets current OEM code page.
    /// </summary>
    public static int OemCodePage
    {
      get
      {
        CultureInfo info = CultureInfo.CurrentCulture;
        TextInfo textInfo = info.TextInfo;
        return textInfo.OEMCodePage;
      }
    }
#endif
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    internal ZipArchiveItem( ZipArchive archive )
    {
      m_archive = archive;
    }

    /// <summary>
    /// Creates new instance of the zip item.
    /// </summary>
    /// <param name="itemName">Name of the item (can be relative or absolute path).</param>
    /// <param name="streamData">Stream data.</param>
    /// <param name="controlStream">
    /// Indicates whether item controls stream and must close it when item finish its work.
    /// </param>
    /// <param name="attributes"></param>
    public ZipArchiveItem( ZipArchive archive, string itemName, Stream streamData, bool controlStream, FileAttributes attributes ):
      this( archive )
    {
      m_strItemName = itemName;
      m_bControlStream = controlStream;
      m_streamData = streamData;
      m_iExternalAttributes = ( int )attributes;

      if( CheckIsUnicode(m_strItemName))
        m_options |= GeneralPurposeBitFlags.Unicode;
    }
    #endregion

    #region Methods
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 || (SILVERLIGHT) )
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public void Update( ZippedContentStream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( m_streamData != null && m_bControlStream )
      {
        m_streamData.Close();
      }

      m_streamData = stream.ZippedContent;
      m_lCompressedSize = m_streamData.Length;
      m_lOriginalSize = stream.UnzippedSize;
      m_bCompressed = true;
      m_uiCrc32 = stream.Crc32;
      m_bControlStream = false;
    }
#endif
    /// <summary>
    /// Updates internal data stream.
    /// </summary>
    /// <param name="newDataStream">New stream to set.</param>
    /// <param name="controlStream">Indicates whether item should conrol new stream.</param>
    public void Update( Stream newDataStream, bool controlStream )
    {
        if (m_streamData != null && m_bControlStream)
        {
         if (m_streamData != newDataStream )   
            m_streamData.Close();
        }

      m_bControlStream = controlStream;
      m_streamData = newDataStream;
      ResetFlags();

      m_lOriginalSize = ( newDataStream != null )? newDataStream.Length : 0;
      //m_compressionMethod = CompressionMethod.Deflated;
    }
    /// <summary>
    /// 
    /// </summary>
    public void ResetFlags()
    {
      m_lCompressedSize = 0;
      m_lOriginalSize = 0;
      m_bCompressed = false;
      m_uiCrc32 = 0;
    }
    /// <summary>
    /// This method saves item inside stream.
    /// </summary>
    /// <param name="outputStream">Stream to save item into.</param>
    internal void Write( Stream outputStream )
    {
      if( m_streamData == null || m_streamData.Length == 0 )
      {
        m_compressionLevel = CompressionLevel.NoCompression;
        m_compressionMethod = CompressionMethod.Stored;
      }
//      else if( m_compressionLevel == CompressionLevel.Normal )
//      {
//        m_options = ( GeneralPurposeBitFlags )0;
//      }
//      else if( m_compressionLevel == CompressionLevel.Best )
//      {
//        m_options = ( GeneralPurposeBitFlags )2;
//      }
//      else if( m_compressionLevel == CompressionLevel.BestSpeed )
//      {
//        m_options = ( GeneralPurposeBitFlags )6;
//      }

      WriteHeader( outputStream );
      WriteZippedContent( outputStream );
      WriteFooter( outputStream );
    }
    /// <summary>
    /// Frees all internal resources and closes internal stream if necessary.
    /// </summary>
    internal void Close()
    {
      if( m_streamData != null )
      {
        m_streamData.Flush();

        if( m_bControlStream )
        {
          m_streamData.Close();
        }

        m_streamData = null;
        m_strItemName = null;
      }
    }
    /// <summary>
    /// This method writes file header into Central directory record.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    internal void WriteFileHeader( Stream stream )
    {
      //throw new Exception( "The method or operation is not implemented." );
      // TODO: this function can be optimized.
      stream.Write( BitConverter.GetBytes( Constants.CentralHeaderSignature ), 0, Constants.IntSize );
      stream.Write( BitConverter.GetBytes( ( short )Constants.VersionMadeBy ), 0, Constants.ShortSize );
      stream.Write( BitConverter.GetBytes( ( short )Constants.VersionNeededToExtract ), 0, Constants.ShortSize );

      // general purpose bit flag
      stream.Write( BitConverter.GetBytes( ( short )m_options ), 0, Constants.ShortSize );
      //stream.WriteByte( 0 );
      //stream.WriteByte( 0 );

      stream.Write( BitConverter.GetBytes( ( short )m_compressionMethod ),
        0, Constants.ShortSize );

      // writes last modified date and time of the file
      int lastModified = ConvertDateTime(DateTime.Now);

      byte[] date = new byte[4];

      date[0] = (byte)(lastModified & 0x000000FF);
      date[1] = (byte)((lastModified & 0x0000FF00) >> 8);
      date[2] = (byte)((lastModified & 0x00FF0000) >> 16);
      date[3] = (byte)((lastModified & 0xFF000000) >> 24);

      stream.Write(date, 0, Constants.IntSize);

      stream.Write( BitConverter.GetBytes( m_uiCrc32 ), 0, Constants.IntSize );
      stream.Write( BitConverter.GetBytes( ( int )m_lCompressedSize ), 0, Constants.IntSize );
      stream.Write( BitConverter.GetBytes( ( int )m_lOriginalSize ), 0, Constants.IntSize );

      Encoding encoding = ( ( m_options & GeneralPurposeBitFlags.Unicode ) != 0 ) ?
        Encoding.UTF8 :
#if !(SILVERLIGHT || WP)
        Encoding.GetEncoding( OemCodePage );
        //Encoding.GetEncoding( 1252 );
        //Encoding.Default;
#else
        new LatinEncoding();
#endif

      byte[] arrName = encoding.GetBytes( m_strItemName );
      int iNameLength = encoding.GetByteCount(m_strItemName);

      stream.Write( BitConverter.GetBytes( ( short )iNameLength ), 0, Constants.ShortSize );

      // extra field length
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // file comment length
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // disk number start
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // internal file attributes
      stream.WriteByte( 0 );
      stream.WriteByte( 0 );

      // external file attributes
      stream.Write( BitConverter.GetBytes( m_iExternalAttributes ), 0, Constants.IntSize );

      // relative offset of the local header
      stream.Write( BitConverter.GetBytes( m_iLocalHeaderOffset ), 0, Constants.IntSize );
      //stream.WriteByte( 0 );
      //stream.WriteByte( 0 );
      //stream.WriteByte( 0 );
      //stream.WriteByte( 0 );

      stream.Write( arrName, 0, arrName.Length );
    }
    /// <summary>
    /// Converts current datetime to Windows format.
    /// </summary>
    /// <param name="time">Current Date and time.</param>
    /// <returns>Value in Windows format.</returns>
    private Int32 ConvertDateTime(DateTime time)
    {
        time = time.ToLocalTime();

        UInt16 uDate = (UInt16)((time.Day & 0x0000001F) | ((time.Month << 5) & 0x000001E0) | (((time.Year - 1980) << 9) & 0x0000FE00));
        UInt16 uTime = (UInt16)((time.Second / 2 & 0x0000001F) | ((time.Minute << 5) & 0x000007E0) | ((time.Hour << 11) & 0x0000F800));

        Int32 result = (Int32)(((UInt32)(uDate << 16)) | uTime);
        return result;
    }
    /// <summary>
    /// Read data from the stream based on the central directory.
    /// </summary>
    /// <param name="stream">Stream to read data from, stream.Position must point at just after correct file header.</param>
    internal void ReadCentralDirectoryData( Stream stream )
    {
      // on the current moment we ignore "version made by" and "version needed to extract" fields.
      stream.Position += 4;

      m_options = ( GeneralPurposeBitFlags )ZipArchive.ReadInt16( stream );

      if (m_options != 0) m_options = 0;

      m_compressionMethod = ( CompressionMethod )ZipArchive.ReadInt16( stream );
      m_bCompressed = true;

      // on the current moment we ignore "last mod file time" and "last mod file date" fields.
      stream.Position += 4;

      m_uiCrc32 = ( uint )ZipArchive.ReadInt32( stream );
      m_lCompressedSize = ZipArchive.ReadInt32( stream );
      m_lOriginalSize = ZipArchive.ReadInt32( stream );

      int iFileNameLength = ZipArchive.ReadInt16( stream );
      int iExtraFieldLenth = ZipArchive.ReadInt16( stream );
      int iCommentLength = ZipArchive.ReadInt16( stream );

      // on the current moment we ignore and "disk number start" (2 bytes),
      // "internal file attributes" (2 bytes).
      stream.Position += 4;

      m_iExternalAttributes = ZipArchive.ReadInt32( stream );
      m_iLocalHeaderOffset = ZipArchive.ReadInt32( stream );

      byte[] arrBuffer = new byte[ iFileNameLength ];

      stream.Read( arrBuffer, 0, iFileNameLength );

      Encoding encoding = ( ( m_options & GeneralPurposeBitFlags.Unicode ) != 0 ) ?
        Encoding.UTF8 :
#if !(SILVERLIGHT || WP)
        Encoding.GetEncoding( OemCodePage );
        //Encoding.Default;
#else
        new LatinEncoding();
#endif

      m_strItemName = encoding.GetString( arrBuffer, 0, arrBuffer.Length );
     m_strItemName= m_strItemName.Replace("\\", "/");
      // we are not interested in other items, so we simply ignore them.
      stream.Position += iExtraFieldLenth + iCommentLength;
    }
    /// <summary>
    /// Reads zipped data from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="checkCrc">Indicates whether we should check crc value after data decompression.</param>
    internal void ReadData( Stream stream, bool checkCrc )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      stream.Position = m_iLocalHeaderOffset;
      m_bCheckCrc = checkCrc;

      ReadLocalHeader( stream );
      ReadCompressedData( stream );
    }
    /// <summary>
    /// Extracts compressed data from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    private void ReadCompressedData( Stream stream )
    {
      if( m_lCompressedSize > 0 )
      {
        MemoryStream dataStream = new MemoryStream();
        int iBytesLeft = ( int )m_lCompressedSize;
        dataStream.Capacity = iBytesLeft;

        byte[] arrBuffer = new byte[ Constants.BufferSize ];

        while( iBytesLeft > 0 )
        {
          int iBytesToRead = Math.Min( iBytesLeft, Constants.BufferSize );

          if( stream.Read( arrBuffer, 0, iBytesToRead ) != iBytesToRead )
            throw new ZipException( "End of file reached - wrong file format or file is corrupt." );

          dataStream.Write( arrBuffer, 0, iBytesToRead );
          iBytesLeft -= iBytesToRead;
        }

        m_streamData = dataStream;
        m_bControlStream = true;
      }
      else if(m_lCompressedSize<0) //If compression size is negative, then read until the next header signature reached.
      {
          MemoryStream dataStream = new MemoryStream();
          int bt = 0;
          bool proceed=true;
          while (proceed)
          {
              if ((bt = stream.ReadByte()) == Constants.HeaderSignatureStartByteValue)
              {
                  stream.Position -= 1;
                  int headerSignature = ZipArchive.ReadInt32(stream);
                  if (headerSignature==Constants.CentralHeaderSignature || headerSignature==Constants.CentralHeaderSignature)
                  {
                      proceed = false;
                   
                  }
                  stream.Position -= 3;
              }
              if (proceed)
                  dataStream.WriteByte((byte)bt);
          }
          m_streamData = dataStream;
          m_lCompressedSize = m_streamData.Length;
          m_bControlStream = true;
      }
    }
    /// <summary>
    /// Extracts local header from the stream.
    /// </summary>
    /// <param name="stream">Stream to read data from.</param>
    private void ReadLocalHeader( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( ZipArchive.ReadInt32( stream ) != Constants.HeaderSignature )
        throw new ZipException( "Can't find local header signature - wrong file format or file is corrupt." );

      // TODO: it is good to verify data read from the central directory record,
      // but on the current moment we simply skip it.
      stream.Position += 22;

      int iNameLength = ZipArchive.ReadInt16( stream );
      int iExtraLength = ZipArchive.ReadUInt16( stream );

      stream.Position += iNameLength + iExtraLength;
    }
    /// <summary>
    /// Decompressed internal data if necessary.
    /// </summary>
    private void DecompressData()
    {
      if( m_bCompressed )
      {
        if( m_compressionMethod == CompressionMethod.Deflated )
        {
          if( m_lOriginalSize > 0 )
          {
            m_streamData.Position = 0;
#if SyncfusionFramework1_0 || SyncfusionFramework1_1 || (SILVERLIGHT || WP)
            DecompressDataOld();
#else
            if( m_bOptimizedDecompress )
            {
              DecompressDataMemoryOptimized();
            }
            else
            {
              DecompressDataOrdinary();
            }
#endif
//#if SyncfusionFramework1_0 || SyncfusionFramework1_1
//            CompressedStreamReader reader = new CompressedStreamReader( m_streamData, true );
//#else
//            DeflateStream deflateStream = new DeflateStream( m_streamData, CompressionMode.Decompress, true );
//#endif

//            MemoryStream decompressedData = new MemoryStream();
//            decompressedData.Capacity = ( int )m_lOriginalSize;
//            byte[] arrBuffer = new byte[ Constants.BufferSize ];
//            int iReadBytes;

//#if SyncfusionFramework1_0 || SyncfusionFramework1_1
//            while( ( iReadBytes = reader.Read( arrBuffer, 0, Constants.BufferSize ) ) > 0 )
//#else
//            while( ( iReadBytes = deflateStream.Read( arrBuffer, 0, Constants.BufferSize ) ) > 0 )
//#endif
//            {
//              decompressedData.Write( arrBuffer, 0, iReadBytes );
//            }

//#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
//            deflateStream.Dispose();
//#endif

            //if( m_bControlStream )
            //{
            //  m_streamData.Close();
            //}

            //m_bControlStream = true;
            //m_streamData = decompressedData;
            //decompressedData.SetLength( m_lOriginalSize );
            //decompressedData.Capacity = ( int )m_lOriginalSize;

            //if( m_bCheckCrc )
            //{
            //  //CheckCrc();
            //  CheckCrc( decompressedData.GetBuffer() );
            //}

            //m_streamData.Position = 0;
          }
#if !(SILVERLIGHT || WP)
            else
            {
                m_streamData.Position = 0;
                DecompressDataOrdinary();
            }
#elif (SILVERLIGHT || WP)
          else
          {
              m_streamData.Position = 0;
              DecompressDataOld();
          }
#endif
            m_bCompressed = false;
        }
        else if( m_compressionMethod == CompressionMethod.Stored )
        {
          m_bCompressed = false;
        }
        else
        {
          throw new NotSupportedException( "Compression type: " + m_compressionMethod.ToString() + " is not supported" );
        }
      }
    }
    private void DecompressDataOld()
    {
      CompressedStreamReader reader = new CompressedStreamReader( m_streamData, true );

      MemoryStream decompressedData = new MemoryStream();
        if(m_lOriginalSize>0)
            decompressedData.Capacity = (int)m_lOriginalSize;
      byte[] arrBuffer = new byte[ Constants.BufferSize ];
      int iReadBytes;

      while( ( iReadBytes = reader.Read( arrBuffer, 0, Constants.BufferSize ) ) > 0 )
      {
        decompressedData.Write( arrBuffer, 0, iReadBytes );
      }

      if( m_bControlStream )
      {
        m_streamData.Close();
      }
      m_lOriginalSize = decompressedData.Length;
      m_bControlStream = true;
      m_streamData = decompressedData;
      decompressedData.SetLength( m_lOriginalSize );
      decompressedData.Capacity = ( int )m_lOriginalSize;

      if( m_bCheckCrc )
      {
        //CheckCrc();
        CheckCrc( decompressedData.GetBuffer() );
      }

      m_streamData.Position = 0;
    }
#if !(SILVERLIGHT || WP)
    private void DecompressDataMemoryOptimized()
    {
      m_streamData = new DeflateStream( m_streamData, CompressionMode.Decompress, m_bControlStream );

      //if( m_bCheckCrc )
      //{
      //  //CheckCrc();
      //  CheckCrc( decompressedData.GetBuffer() );
      //}

      //m_streamData.Position = 0;
    }
    private void DecompressDataOrdinary()
    {
      DeflateStream deflateStream = new DeflateStream( m_streamData, CompressionMode.Decompress, true );

      MemoryStream decompressedData = new MemoryStream();
      if (m_lOriginalSize > 0)
      {
          decompressedData.Capacity = (int)m_lOriginalSize;
      }
      byte[] arrBuffer = new byte[ Constants.BufferSize ];
      int iReadBytes;
      bool bReadSucceed = false;

      while( ( iReadBytes = deflateStream.Read( arrBuffer, 0, Constants.BufferSize ) ) > 0 )
      {
        bReadSucceed = true;
        decompressedData.Write( arrBuffer, 0, iReadBytes );
      }

      deflateStream.Dispose();

      if( !bReadSucceed )
      {
        m_streamData.Position = 0;
        DecompressDataOld();
      }
      else
      {
        if( m_bControlStream )
        {
          m_streamData.Close();
        }
        if (m_lOriginalSize < 0)
        {
            m_lOriginalSize = decompressedData.Length;
        }
        m_bControlStream = true;
        m_streamData = decompressedData;
        decompressedData.SetLength( m_lOriginalSize );
        decompressedData.Capacity = ( int )m_lOriginalSize;

        if( m_bCheckCrc )
        {
          //CheckCrc();
          CheckCrc( decompressedData.GetBuffer() );
        }

        m_streamData.Position = 0;
      }
    }
#endif
    /// <summary>
    /// Writes local file header.
    /// </summary>
    /// <param name="outputStream">Stream to write into.</param>
    private void WriteHeader( Stream outputStream )
    {
      // TODO: this method can be optimized.
      //throw new Exception( "The method or operation is not implemented." );
      m_iLocalHeaderOffset = ( int )outputStream.Position;
      outputStream.Write( BitConverter.GetBytes( Constants.HeaderSignature ), 0, Constants.HeaderSignatureBytes );
      outputStream.Write( BitConverter.GetBytes( Constants.VersionNeededToExtract ), 0, Constants.ShortSize );

      // general purpose bit flag
      outputStream.Write( BitConverter.GetBytes( ( short )m_options ), 0, Constants.ShortSize );
      //outputStream.WriteByte( 0 );
      //outputStream.WriteByte( 0 );

      outputStream.Write( BitConverter.GetBytes( ( short )m_compressionMethod ),
        0, Constants.ShortSize );

      // last mod file time
      outputStream.WriteByte( 0 );
      outputStream.WriteByte( 0 );

      // last mod file date
      outputStream.WriteByte( 33 );
      outputStream.WriteByte( 0 );

      m_lCrcPosition = outputStream.Position;
      outputStream.Write( BitConverter.GetBytes( m_uiCrc32 ), 0, Constants.IntSize );
      outputStream.Write( BitConverter.GetBytes( ( int )m_lCompressedSize ), 0, Constants.IntSize );
      outputStream.Write( BitConverter.GetBytes( ( int )m_lOriginalSize ), 0, Constants.IntSize );

      Encoding encoding = ( ( m_options & GeneralPurposeBitFlags.Unicode ) != 0 ) ?
        Encoding.UTF8 :
#if !(SILVERLIGHT || WP)
        Encoding.GetEncoding( OemCodePage );
        //Encoding.GetEncoding( 1252 );
        //Encoding.Default;
#else
        new LatinEncoding();
#endif

      int iCharCount = encoding.GetByteCount( m_strItemName );
      outputStream.Write( BitConverter.GetBytes( ( short )iCharCount ), 0, Constants.ShortSize );

      // extra field length
      outputStream.WriteByte( 0 );
      outputStream.WriteByte( 0 );

      byte[] arrName = encoding.GetBytes( m_strItemName );
      outputStream.Write( arrName, 0, arrName.Length );
    }
    /// <summary>
    /// Writes zipped content inside stream.
    /// </summary>
    /// <param name="outputStream">Stream to write into.</param>
    private void WriteZippedContent( Stream outputStream )
    {
      long lDataLength = ( m_streamData != null ) ? m_streamData.Length : 0L;

      if( lDataLength <= 0 )
      {
        return;
      }

      long lStartPosition = outputStream.Position;

      if( m_bCompressed || m_compressionMethod == CompressionMethod.Stored )
      {
        m_streamData.Position = 0;
        // TODO: use single buffer at least for the signle archive to reduce memory usage and work time.
        byte[] arrBuffer = new byte[ Constants.BufferSize ];

        while( lDataLength > 0 )
        {
          int iReadSize = m_streamData.Read( arrBuffer, 0, Constants.BufferSize );
          outputStream.Write( arrBuffer, 0, iReadSize );
          lDataLength -= iReadSize;
          if (m_compressionMethod == CompressionMethod.Stored && m_uiCrc32==0)
              m_uiCrc32 = ZipCrc32.ComputeCrc(arrBuffer, 0, iReadSize, m_uiCrc32);
        }
      }
      else if( m_compressionMethod == CompressionMethod.Deflated )
      {
        m_lOriginalSize = lDataLength;
        m_streamData.Position = 0;
        m_uiCrc32 = 0; //Constants.StartCrc;
        // TODO: use single buffer at least for the signle archive to reduce memory usage and work time.
        byte[] arrBuffer = new byte[ Constants.BufferSize ];
        Stream compressor = m_archive.CreateCompressor( outputStream );
//#if SyncfusionFramework1_0 || SyncfusionFramework1_1 || (SILVERLIGHT)
//        compressor = new NetCompressor( m_compressionLevel, outputStream );
//#else
//        compressor = new NativeCompressor( m_compressionLevel, outputStream );
//#endif

        while( lDataLength > 0 )
        {
          int iReadSize = m_streamData.Read( arrBuffer, 0, Constants.BufferSize );
          bool bClose = ( lDataLength <= Constants.BufferSize );
          compressor.Write( arrBuffer, 0, iReadSize );
          lDataLength -= iReadSize;
          m_uiCrc32 = ZipCrc32.ComputeCrc( arrBuffer, 0, iReadSize, m_uiCrc32 );
        }

        compressor.Close();
      }

      m_lCompressedSize = outputStream.Position - lStartPosition;
    }
    /// <summary>
    /// Writes local file footer into stream.
    /// </summary>
    /// <param name="outputStream">Stream to write into.</param>
    private void WriteFooter( Stream outputStream )
    {
      //throw new Exception( "The method or operation is not implemented." );
      if( outputStream == null )
      {
        throw new ArgumentNullException( "outputStream" );
      }

      long lCurrentPos = outputStream.Position;

      outputStream.Position = m_lCrcPosition;
      outputStream.Write( BitConverter.GetBytes( m_uiCrc32 ), 0, Constants.IntSize );
      outputStream.Write( BitConverter.GetBytes( ( int )m_lCompressedSize ), 0, Constants.IntSize );
      outputStream.Write( BitConverter.GetBytes( ( int )m_lOriginalSize ), 0, Constants.IntSize );

      outputStream.Position = lCurrentPos;
    }
    /// <summary>
    /// Checks whether Crc field and stream data corresponds each other.
    /// </summary>
    private void CheckCrc()
    {
      m_streamData.Position = 0;
      uint uiCrc = ZipCrc32.ComputeCrc( m_streamData, ( int )m_lOriginalSize );

      if( uiCrc != m_uiCrc32 )
        throw new ZipException( "Wrong Crc value." );
    }
    private void CheckCrc( byte[] arrData )
    {
      uint uiCrc = ZipCrc32.ComputeCrc( arrData, 0, ( int )m_lOriginalSize, 0 );

      if( uiCrc != m_uiCrc32 )
        throw new ZipException( "Wrong Crc value." );
    }
    internal ZipArchiveItem Clone()
    {
      ZipArchiveItem result = ( ZipArchiveItem )MemberwiseClone();
      result.m_streamData = CloneStream( m_streamData );
      return result;
    }
    /// <summary>
    /// Creates copy of the stream.
    /// </summary>
    /// <param name="stream">Stream to copy.</param>
    /// <returns>Created stream.</returns>
    public static Stream CloneStream( Stream stream )
    {
      if( stream == null )
        return null;

      long lStartPos = stream.Position;

      MemoryStream result = new MemoryStream( ( int )stream.Length );
      stream.Position = 0;
      const int BufferSize = 32768;
      byte[] arrBuffer = new byte[ BufferSize ];
      int iReadSize;

      while( ( iReadSize = stream.Read( arrBuffer, 0, BufferSize ) ) != 0 )
      {
        result.Write( arrBuffer, 0, iReadSize );
      }

      stream.Position = lStartPos;
      result.Position = lStartPos;

      return result;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// A method to release allocated unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_strItemName != null )
      {
        Close();
        m_strItemName = null;
        GC.SuppressFinalize( this );
      }
    }

    /// <summary>
    /// Finilizer.
    /// </summary>
    ~ZipArchiveItem()
    {
      Dispose();
    }
    #endregion

    #region HelperMethods
    /// <summary>
    /// Checks whether the file has unicode characters.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private bool CheckIsUnicode(string fileName)
    {
        if (fileName == null || fileName == string.Empty)
            throw new ArgumentException("fileName");

        char[] fileArray = fileName.ToCharArray();

        foreach (char ch in fileArray)
        {
            if ((int)ch > MaxAnsiCode)
                return true;
        }
        return false;
    }
      #endregion
  }
}
