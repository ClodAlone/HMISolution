#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the image class.
  /// </summary>
  public class Image : IDisposable
  {
    #region Constants
    private const string GifHeader = "GIF8";
    private const string BmpHeader = "BM";
    #endregion

    #region Fields
    private static byte[] m_pngHeader = { 137, 80, 78, 71, 13, 10, 26, 10 };
    private static byte[] m_jpegHeader = { 255, 216 };

    private Stream m_stream;
    private int m_height;
    private int m_width;
    private ImageFormat m_format;
    private byte[] m_imageData;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width
    {
      get
      {
        return m_width;
      }
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height
    {
      get
      {
        return m_height;
      }
    }

    /// <summary>
    /// Gets the image format.
    /// </summary>
    /// <value>The image format.</value>
    public ImageFormat Format
    {
      get
      {
        return m_format;
      }
    }

    /// <summary>
    /// Gets the image format.
    /// </summary>
    /// <value>The image format.</value>
    public ImageFormat RawFormat
    {
      get
      {
        return Format;
      }
    }

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public Size Size
    {
      get
      {
        return new Size( m_width, m_height );
      }
    }

    /// <summary>
    /// Gets the image data.
    /// </summary>
    /// <value>The image data.</value>
    public byte[] ImageData
    {
      get
      {
        return m_imageData;
      }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public Image( Stream stream )
    {
      if( !stream.CanRead || !stream.CanSeek )
        throw new ArgumentException( "Stream" );

      m_stream = stream;

      Initialize();


    }
    #endregion

    #region Implementation
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    private void Initialize()
    {
      if( CheckIfPng() )
      {
        m_format = ImageFormat.Png;
        ParsePngImage();
      }

      if( m_format == ImageFormat.Unknown
          && CheckIfJpeg() )
      {
        m_format = ImageFormat.Jpeg;
        ParseJpegImage();
      }

      if( m_format == ImageFormat.Unknown
          && CheckIfGif() )
      {
        m_format = ImageFormat.Gif;
        ParseGifImage();
      }

      if( m_format == ImageFormat.Unknown
          && CheckIfEmfOrWmf() )
      {
        ParseEmfOrWmfImage();
      }

      if( m_format == ImageFormat.Unknown
          && CheckIfIcon() )
      {
        m_format = ImageFormat.Icon;
        ParseIconImage();
      }

      if( m_format == ImageFormat.Unknown
        && CheckIfBmp() )
      {
        m_format = ImageFormat.Bmp;
        ParseBmpImage();
      }

        Reset();
        m_imageData = new byte[ m_stream.Length ];
        m_stream.Read( m_imageData, 0, m_imageData.Length );
    }

    /// <summary>
    /// Checks if icon.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfIcon()
    {
      Reset();

      int idReserved = ReadWord();
      int idType = ReadWord();

      if( idReserved == 0 && idType == 1 )
        return true;
      else
        return false;
    }

    /// <summary>
    /// Checks if PNG.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfPng()
    {
      Reset();

      for( int i = 0; i < m_pngHeader.Length; i++ )
      {
        if( m_pngHeader[ i ] != m_stream.ReadByte() )
          return false;
      }
      return true;
    }

    /// <summary>
    /// Checks if JPEG.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfJpeg()
    {
      Reset();

      for( int i = 0; i < m_jpegHeader.Length; i++ )
      {
        if( m_jpegHeader[ i ] != m_stream.ReadByte() )
          return false;
      }
      return true;
    }

    /// <summary>
    /// Checks if GIF.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfGif()
    {
      Reset();

      string header = ReadString( 6 );
      if( header.StartsWith( GifHeader ) )
        return true;

      return false;
    }

    /// <summary>
    /// Checks if Bmp.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfBmp()
    {
      Reset();

      string header = ReadString( 2 );
      if( header.StartsWith( BmpHeader ) )
        return true;

      return false;
    }

    /// <summary>
    /// Checks if EMF or WMF.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfEmfOrWmf()
    {
      Reset();

      if( ReadInt32() == 1 )
      {
        m_format = ImageFormat.Emf;
        return true;
      }
      else
      {
        Reset();

        if( ReadInt32() == unchecked( ( int )0x9AC6CDD7 ) )
        {
          m_format = ImageFormat.Wmf;
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Parses the PNG image.
    /// </summary>
    private void ParsePngImage()
    {
      byte[] temp;
      while( true )
      {
        int length = ReadUInt32();
        string record = ReadString( 4 );

        if( record.Equals( "IHDR" ) )
        {
          m_width = ReadUInt32();
          m_height = ReadUInt32();
          break;
        }
        else
        {
          temp = new byte[ length ];
          m_stream.Read( temp, 0, length );
        }
      }
    }

    /// <summary>
    /// Parses the JPEG image.
    /// </summary>
    private void ParseJpegImage()
    {
      Reset();

      byte[] imgData = new byte[ m_stream.Length ];
      m_stream.Read( imgData, 0, imgData.Length );
      long i = 4;

      // Check for valid JPEG header
      if( imgData[ i + 2 ] == 'J' && imgData[ i + 3 ] == 'F' &&
          imgData[ i + 4 ] == 'I' && imgData[ i + 5 ] == 'F' &&
          imgData[ i + 6 ] == 0 )
      {
        long length = imgData[ i ] * 256 + imgData[ i + 1 ];
        while( i < imgData.Length )
        {
          i += length;
          if( imgData[ i + 1 ] == 192 )
          {
            /*m_width*/m_height = imgData[ i + 5 ] * 256 + imgData[ i + 6 ];
            /*m_height*/m_width = imgData[ i + 7 ] * 256 + imgData[ i + 8 ];
            return;
          }
          else
          {
            i += 2;
            length = imgData[ i ] * 256 + imgData[ i + 1 ];
          }
        }
      }
    }

    /// <summary>
    /// Parses the GIF image.
    /// </summary>
    private void ParseGifImage()
    {
      m_width = ReadInt16();
      m_height = ReadInt16();
    }

    /// <summary>
    /// Parses the GIF image.
    /// </summary>
    private void ParseBmpImage()
    {
      m_stream.Position = 0x12;
      m_width = ReadInt32();
      m_height = ReadInt32();
    }

    /// <summary>
    /// Parses the icon image.
    /// </summary>
    private void ParseIconImage()
    {
      Reset();
      byte[] temp = new byte[ 6 ];
      m_stream.Read( temp, 0, temp.Length );

      m_width = m_stream.ReadByte();
      m_height = m_stream.ReadByte();
    }

    /// <summary>
    /// Parses the EMF or WMF image.
    /// </summary>
    private void ParseEmfOrWmfImage()
    {
      Reset();
      byte[] temp;

      if( m_format == ImageFormat.Emf )
      {
        temp = new byte[ 16 ];
        m_stream.Read( temp, 0, temp.Length );
        m_width = ReadInt32();
        m_height = ReadInt32();
      }
      else if( Format == ImageFormat.Wmf )
      {
        temp = new byte[ 10 ];
        m_stream.Read( temp, 0, temp.Length );

        m_width = ReadShortLE();
        m_height = ReadShortLE();
      }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Reads the Uint32.
    /// </summary>
    /// <returns></returns>
    private int ReadUInt32()
    {
      byte[] buffer = new byte[ 4 ];
      m_stream.Read( buffer, 0, 4 );

      return ( ( buffer[ 0 ] << 24 ) + ( buffer[ 1 ] << 16 ) + ( buffer[ 2 ] << 8 ) + buffer[ 3 ] );
    }

    /// <summary>
    /// Reads the int32.
    /// </summary>
    /// <returns></returns>
    private Int32 ReadInt32()
    {
      byte[] buffer = new byte[ 4 ];
      m_stream.Read( buffer, 0, 4 );

      return ( buffer[ 0 ] + ( buffer[ 1 ] << 8 ) + ( buffer[ 2 ] << 16 ) + ( buffer[ 3 ] << 24 ) );
    }

    /// <summary>
    /// Reads the Uint16.
    /// </summary>
    /// <returns></returns>
    private int ReadUInt16()
    {
      byte[] buffer = new byte[ 2 ];
      m_stream.Read( buffer, 0, 2 );

      return ( buffer[ 0 ] << 8 ) + buffer[ 1 ];
    }

    /// <summary>
    /// Reads the int16.
    /// </summary>
    /// <returns></returns>
    private int ReadInt16()
    {
      byte[] buffer = new byte[ 2 ];
      m_stream.Read( buffer, 0, 2 );

      return buffer[ 0 ] | ( buffer[ 1 ] << 8 );
    }

    /// <summary>
    /// Reads the word.
    /// </summary>
    /// <returns></returns>
    private int ReadWord()
    {
      int num = m_stream.ReadByte();
      return ( num + ( m_stream.ReadByte() << 8 ) ) & 0xffff;
    }

    /// <summary>
    /// Reads the short LE.
    /// </summary>
    /// <returns></returns>
    private int ReadShortLE()
    {
      int num = ReadWord();
      if( num > 0x7fff )
        num -= 0x10000;
      return num;
    }

    /// <summary>
    /// Reads the string.
    /// </summary>
    /// <param name="len">The len.</param>
    /// <returns></returns>
    private string ReadString( int len )
    {
      char[] chars = new char[ len ];

      for( int i = 0; i < len; i++ )
      {
        chars[ i ] = ( char )m_stream.ReadByte();
      }

      return new string( chars );
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    private void Reset()
    {
      m_stream.Position = 0;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      m_imageData = null;

      if( m_stream != null )
      {
        m_stream.Dispose();
        m_stream = null;
      }
    }
    #endregion

    public Image Clone()
    {
      Image result = ( Image )MemberwiseClone();
      //Stream m_stream;
      result.m_imageData = CloneUtils.CloneByteArray( m_imageData );
      return result;
    }
#if !WINRT
    public static Image FromFile( string fileName )
    {
      FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
      return new Image( stream );
    }
#endif

    public static Image FromStream( Stream stream )
    {
      return new Image( stream );
    }

    public static Image FromStream( Stream stream, bool p, bool p_3 )
    {
      return new Image( stream );
    }

    internal void Save( MemoryStream stream, ImageFormat imageFormat )
    {
      if( imageFormat != m_format )
        throw new NotSupportedException();

      stream.Write( m_imageData, 0, m_imageData.Length );
    }
  }

  /// <summary>
  /// Specifies the image format.
  /// </summary>
  public enum ImageFormat
  {
    Unknown,
    Bmp,
    Emf,
    Gif,
    Jpeg,
    Png,
    Wmf,
    Icon
  }
}
