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
using System.Collections;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.Compression;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if SHOW_PICTURES
using System.Windows.Forms;
#endif

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO;
using Syncfusion.Compression.Zip; 
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Drawing;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing;
using System.Drawing.Imaging;
#endif


#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Represents metafile picture in the workbook.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsoMetafilePicture
    : MsoBase
    , IDisposable
    , IPictureRecord
  {
    #region Class constants
    /// <summary>
    /// Default buffer size.
    /// </summary>
    private const int DEF_BUFFER_SIZE = 32 * 1024;
    /// <summary>
    /// Offset to UID.
    /// </summary>
    private const int DEF_UID_OFFSET = 0;
    /// <summary>
    /// Offset to metafile size.
    /// </summary>
    private const int DEF_METAFILE_SIZE_OFFSET = 16;
    /// <summary>
    /// Offset to compressed size.
    /// </summary>
    private const int DEF_COMPRESSED_SIZE_OFFSET = DEF_METAFILE_SIZE_OFFSET + 4 * 4 + 4 * 2;
    /// <summary>
    /// To specify BlipEMF's two UIDs.
    /// </summary>
    internal const uint BlipEMFWithTwoUIDs = 0x3D5;
    /// <summary>
    /// To specify BlipWMF's two UIDs.
    /// </summary>
    internal const uint BlipWMFWithTwoUIDs = 0x217;
    /// <summary>
    /// To specify BlipPICT's two UIDs.
    /// </summary>
    internal const uint BlipPICTWithTwoUIDs = 0x543;
    /// <summary>
    /// To specify BlipTIFF's two UIDs.
    /// </summary>
    internal const uint BlipTIFFWithTwoUIDs = 0x6E5;
    #endregion

    #region Class members
    /// <summary>
    /// Memory stream with picture data.
    /// </summary>
    private MemoryStream m_stream;
    /// <summary>
    /// Array with compressed picture.
    /// </summary>
    private byte[] m_arrCompressedPicture;
    /// <summary>
    /// UID of the picture.
    /// </summary>
    private byte[]  m_arrRgbUid = new byte[ 16 ];
    /// <summary>
    /// Primary UID.
    /// </summary>
    private byte[]  m_arrRgbUidPrimary;
    /// <summary>
    /// Cache of the metafile size.
    /// </summary>
    private int       m_iMetafileSize;
    /// <summary>
    /// Boundary of metafile drawing commands.
    /// </summary>
    private Rectangle m_rcBounds;
    /// <summary>
    /// Size of metafile in EMUs.
    /// </summary>
    private Point     m_ptSize;
    /// <summary>
    /// Cache of saved size (size of m_pvBits).
    /// </summary>
    private int       m_iSavedSize;
    /// <summary>
    /// Compression type.
    /// </summary>
    private MsoBlipCompression m_compression;
    /// <summary>
    /// Applied filter.
    /// </summary>
    private MsoBlipFilter m_filter = MsoBlipFilter.msofilterNone;
    /// <summary>
    /// Inner picture.
    /// </summary>
    private Image m_picture;
    private Stream m_pictStream;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the picture.
    /// </summary>
    /// <param name="parent">Parent record.</param>
    public MsoMetafilePicture( MsoBase parent )
      : base( parent )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsoMetafilePicture( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="stream">Stream to get data from.</param>
    public MsoMetafilePicture( MsoBase parent, Stream stream )
      : base( parent, stream, null )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Picture.
    /// </summary>
    public Image Picture
    {
      get
      {
        return m_picture;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Picture" );
        
        m_picture = value;

        MemoryStream metaFile = SerializeMetafile( m_picture );
        m_arrCompressedPicture = CompressMetafile( metaFile, 0 );
#if ( WINRT )
        metaFile.Dispose();
#else
        metaFile.Close();
#endif
      }
    }
    public Stream PictureStream
    {
        get
        {
            return m_pictStream;
        }
        set
        {
            if (value == null)
                throw new ArgumentNullException("value");
            m_pictStream = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public byte[] RgbUid
    {
      get
      {
        return m_arrRgbUid;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length != m_arrRgbUid.Length )
          throw new ArgumentOutOfRangeException( "value.Length" );

        m_arrRgbUid = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      m_rcBounds.Y = 0;
      m_rcBounds.X = 0;
      m_rcBounds.Width = m_picture.Width;
      m_rcBounds.Height = m_picture.Height;

      m_ptSize.X = ( int )ApplicationImpl.ConvertFromPixel( m_picture.Width,
        MeasureUnits.EMU );

      m_ptSize.Y = ( int )ApplicationImpl.ConvertFromPixel( m_picture.Height,
        MeasureUnits.EMU );

      if( MsoRecordType == ( MsoRecords )0 )
      {
        // TODO: replace constant.
        MsoRecordType = ( MsoRecords )61466;
        Instance = 980;
      }

      m_compression = MsoBlipCompression.msoCompressionDeflate;
      int iDataOffset = 0;
      
      stream.Write( m_arrRgbUid, 0, m_arrRgbUid.Length );
      iDataOffset += m_arrRgbUid.Length;

      if (HasTwoUIDs())
      {
          stream.Write(m_arrRgbUidPrimary, 0, m_arrRgbUidPrimary.Length);
          iDataOffset += m_arrRgbUidPrimary.Length;
      }

      WriteInt32( stream, m_iMetafileSize );
      iDataOffset += 4;

      WriteInt32( stream, m_rcBounds.Left );
      iDataOffset += 4;

      WriteInt32( stream, m_rcBounds.Top );
      iDataOffset += 4;

      WriteInt32( stream, m_rcBounds.Right );
      iDataOffset += 4;

      WriteInt32( stream, m_rcBounds.Bottom );
      iDataOffset += 4;

      WriteInt32( stream, m_ptSize.X );
      iDataOffset += 4;

      WriteInt32( stream, m_ptSize.Y );
      iDataOffset += 4;

      WriteInt32( stream, m_iSavedSize );
      iDataOffset += 4;

      stream.WriteByte( ( byte )m_compression );
      iDataOffset++;

      stream.WriteByte( ( byte )m_filter );
      iDataOffset++;

      stream.Write( m_arrCompressedPicture, 0, m_arrCompressedPicture.Length );
      iDataOffset += m_arrCompressedPicture.Length;

      m_iLength = iDataOffset;
    }
    /// <summary>
    /// Parses structure.
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      //AutoExtractFields();

      long lStartPosition = stream.Position;
      //Array.Copy( m_data, 0, m_arrRgbUid, 0, 16 );
      stream.Read( m_arrRgbUid, 0, 16 );

      LoadPrimaryUID( stream );

      m_iMetafileSize = ReadInt32( stream );

      int left = ReadInt32( stream );
      int top = ReadInt32( stream );
      int right = ReadInt32( stream );
      int bottom = ReadInt32( stream );

      m_rcBounds = Rectangle.FromLTRB( left, top, right, bottom );

      left = ReadInt32( stream );
      top = ReadInt32( stream );

      m_ptSize = new Point( left, top );

      m_iSavedSize = ReadInt32( stream );
      m_compression = ( MsoBlipCompression )stream.ReadByte();
      m_filter = ( MsoBlipFilter )stream.ReadByte();

      if( m_stream != null ) m_stream.Close();
      m_stream = new MemoryStream();

      long lDataSize = stream.Position - lStartPosition;
      int iPictureSize = ( int )( m_iLength - lDataSize );
      MemoryStream inputStream = new MemoryStream( iPictureSize );
      inputStream.SetLength( iPictureSize );
      stream.Read( inputStream.GetBuffer(), 0, iPictureSize );

#if SAVE_PICTURES
      FileStream fileStream = new FileStream( "D:\\test_open.emf", FileMode.Create );
      fileStream.Write( m_data, iOffset, m_data.Length - iOffset );
      fileStream.Close();
#endif

      m_arrCompressedPicture = new byte[ inputStream.Length ];
      inputStream.Read( m_arrCompressedPicture, 0, ( int )inputStream.Length );
      inputStream.Position = 0;
      #region TODO ( WINRT )
#if !(WINRT )
      if (m_compression == MsoBlipCompression.msoCompressionDeflate)
      {
          CompressedStreamReader extractor = new CompressedStreamReader(inputStream);

          byte[] buffer = new byte[DEF_BUFFER_SIZE];
          int len;

          while ((len = extractor.Read(buffer, 0, buffer.Length)) > 0)
          {
              m_stream.Write(buffer, 0, len);
          }
      }
      else
#endif
      #endregion
      {
        m_stream.Write( m_arrCompressedPicture, 0, m_arrCompressedPicture.Length );
      }

      m_stream.Position = 0;

#if !SILVERLIGHT && !WINRT && !WP
      m_picture = ApplicationImpl.CreateImage( m_stream );
#else
      m_picture = new Image( m_stream );
#endif

      inputStream.Close();
      
//
//#if SHOW_PICTURES
//      Form frm = new Form();
//      frm.StartPosition = FormStartPosition.CenterScreen;
//      frm.BackgroundImage = m_picture;
//      frm.ShowDialog();
//#endif
    }
    /// <summary>
    /// Indicates whether Art Blip contains Two Unique Id's.
    /// </summary>
    /// <returns></returns>
    private bool HasTwoUIDs()
    {
        int instance = Instance;
        return instance == BlipEMFWithTwoUIDs ||
            instance == BlipWMFWithTwoUIDs ||
            instance == BlipPICTWithTwoUIDs ||
            instance == BlipTIFFWithTwoUIDs;
    }
    /// <summary>
    /// Loads primary UID.
    /// </summary>
    /// <param name="stream">Stream to get primary UID from if necessary.</param>
    /// <returns>Offset after reading primary uid.</returns>
    private int LoadPrimaryUID( Stream stream )
    {
        int iResult = 0;

        if (HasTwoUIDs())
        {
            m_arrRgbUidPrimary = new byte[16];
            //Array.Copy( m_data, startOffset, m_arrRgbUidPrimary, 0, 16 );
            stream.Read(m_arrRgbUidPrimary, 0, 16);
            iResult += 16;
        }

        return iResult;
    }
    /// <summary>
    /// Saves metafile into MemoryStream.
    /// </summary>
    /// <param name="picture">Picture to serialize.</param>
    /// <returns>Memory stream with metafile data.</returns>
    public static MemoryStream SerializeMetafile( Image picture )
    {
      if( picture == null )
        return null;

      MemoryStream result = new MemoryStream();

      int iHeight = ( int )picture.Height;
      int iWidth = ( int )picture.Width;

#if !SILVERLIGHT && !WINRT && !WP
      using( Bitmap bmp = new Bitmap( iWidth + 1, iHeight + 1 ) )
      {
        using( Graphics g = Graphics.FromImage( bmp ) )
        {
          IntPtr hdc = g.GetHdc();

          GraphicsUnit unit = GraphicsUnit.Pixel;
          RectangleF rect = picture.GetBounds( ref unit );
          MetafileFrameUnit metaUnit = GetMetaUnit( unit );

          using( Metafile saveMeta = new Metafile( result, hdc, rect, metaUnit ) )//, EmfType.EmfOnly );
          {
            g.ReleaseHdc( hdc );
            using( Graphics gMeta = Graphics.FromImage( saveMeta ) )
            {
              //gMeta.DrawImageUnscaled( picture, 0, 0 );
              RectangleF rect2 = new RectangleF( rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
              gMeta.DrawImage( picture, rect2 );
            }

            //g.DrawImageUnscaled( saveMeta, 0, 0 );
            //bmp.Save( "d:\\test.bmp" );
          }
        }
      }
      //picture.Save( result, picture.RawFormat );
#else
      byte[] data = picture.ImageData;
      result.Write( data, 0, data.Length );
#endif

      result.Position = 0;
      return result;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Converts GraphicsUnit into MetafileFrameUnit.
    /// </summary>
    /// <param name="unit">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static MetafileFrameUnit GetMetaUnit( GraphicsUnit unit )
    {
      switch( unit )
      {
        case GraphicsUnit.Display:
          return MetafileFrameUnit.GdiCompatible;

        case GraphicsUnit.Document:
          return MetafileFrameUnit.Document;

        case GraphicsUnit.Inch:
          return MetafileFrameUnit.Inch;

        case GraphicsUnit.Millimeter:
          return MetafileFrameUnit.Millimeter;

        case GraphicsUnit.Pixel:
          return MetafileFrameUnit.Pixel;

        case GraphicsUnit.Point:
          return MetafileFrameUnit.Point;

        case GraphicsUnit.World:
          return MetafileFrameUnit.Pixel;
      }
      throw new Exception( "The method or operation is not implemented." );
    }
#endif
    /// <summary>
    /// Compresses metafile picture.
    /// </summary>
    /// <param name="metaFile">Stream with metafile.</param>
    /// <param name="iDataOffset">Offset to the metafile.</param>
    /// <returns>Compressed picture.</returns>
    private byte[] CompressMetafile( Stream metaFile, int iDataOffset )
    {
      MemoryStream outputStream = new MemoryStream();
      try
      {
#if !SILVERLIGHT && !WINRT && !WP
        ( new MD5CryptoServiceProvider().ComputeHash( metaFile ) ).CopyTo( m_arrRgbUid, 0 );
#else
        byte[] hash = ( new SHA1Managed().ComputeHash( metaFile ) );
        Buffer.BlockCopy( hash, 0, m_arrRgbUid, 0, m_arrRgbUid.Length );
#endif
      }
      catch (InvalidOperationException)
      {
#if !SILVERLIGHT && !WINRT && !WP
        ( new MACTripleDES().ComputeHash( metaFile ) ).CopyTo( m_arrRgbUid, 0 );
#endif
      }
      m_iMetafileSize = ( int )metaFile.Length;

      CompressedStreamWriter output = new CompressedStreamWriter( outputStream,
        CompressionLevel.Best, false );

      int len = 0;
      byte[] buffer = new byte[ DEF_BUFFER_SIZE ];
      metaFile.Position = 0;
      long lFullLen = metaFile.Length;

#if SKV_DEBUG
      using ( FileStream fs = new FileStream( "D:\\BadFile.dat", FileMode.Create ) )
      {
        while( ( len = metaFile.Read( buffer, 0, DEF_BUFFER_SIZE ) ) > 0 )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, metaFile.Position, "Current position" );
          fs.Write( buffer, 0, len );
        }
      }

      metaFile.Position = 0;
#endif

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, lFullLen, "Full length" );
      while( ( len = metaFile.Read( buffer, 0, DEF_BUFFER_SIZE ) ) > 0 )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, metaFile.Position, "Current position" );
        output.Write( buffer, 0, len, metaFile.Position + 1 >= lFullLen );
      }


      outputStream.Position = 0;
      m_iLength = iDataOffset;
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, outputStream.Length, "Compressed length" );

      m_iSavedSize = ( int )outputStream.Length;

      byte[] result = new byte[ outputStream.Length ];
      outputStream.Position = 0;
      outputStream.Read( result, 0, ( int )outputStream.Length );
      
#if SKV_DEBUG
      using( FileStream fs = new FileStream( "D:\\CompressedFile.zip", FileMode.Create ) )
      {
        fs.Write( result, 0, result.Length );
      }
#endif

      return result;
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsoMetafilePicture instance = ( MsoMetafilePicture )base.InternalClone();

      instance.m_arrCompressedPicture = CloneUtils.CloneByteArray( m_arrCompressedPicture );

      if( m_stream != null )
      {
        instance.m_stream = UtilityMethods.CloneStream( m_stream );
        instance.m_stream.Position = 0;//m_stream.Position;
      }

#if !SILVERLIGHT && !WINRT && !WP
      if( m_picture != null )
      {
        instance.m_picture = ApplicationImpl.CreateImage( instance.m_stream );
      }
#endif

      return instance;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Frees all unmanaged resources.
    /// </summary>
    protected override void OnDispose()
    {
      if( m_stream != null )
      {
        m_stream.Close();
        m_stream = null;
      }
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~MsoMetafilePicture()
    {
      if( m_stream != null ) Dispose();
    }

    #endregion
  }
}
