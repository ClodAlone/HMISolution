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
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
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
  /// Summary description for MsoPicture.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsoBitmapPicture
    : MsoBase
    , IDisposable
    , IPictureRecord
  {
    #region Class constants
    /// <summary>
    /// Size of the bitmap header.
    /// </summary>
    public const int DEF_DIB_HEADER_SIZE = 14;
    /// <summary>
    /// Dib identifier. ("BM").
    /// </summary>
    private static readonly byte[] DEF_DIB_ID = new byte[]{ 0x42, 0x4D };
    /// <summary>
    /// Reserved.
    /// </summary>
    private static readonly byte[] DEF_RESERVED = new byte[]{ 0, 0, 0, 0 };
    /// <summary>
    /// Number of used colors.
    /// </summary>
    public const int DEF_COLOR_USED_OFFSET = 32;
    /// <summary>
    /// Size of each color definition in the palette.
    /// </summary>
    private const uint DEF_COLOR_SIZE = 4;
    /// <summary>
    /// To specify two UIDs.
    /// </summary>
    internal const uint BlipDIBWithTwoUIDs = 0x7A9;
    /// <summary>
    /// To specify BlipPNG's two UIDs.
    /// </summary>
    internal const uint BlipPNGWithTwoUIDs = 0x6E1;
    /// <summary>
    /// To specify BlipJPEG's two UIDs.
    /// </summary>
    internal const uint BlipJPEGWithTwoUIDs = 0x543;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private byte[]  m_arrRgbUid = new byte[ 16 ];
    /// <summary>
    /// 
    /// </summary>
    private byte[]  m_arrRgbUidPrimary;
    /// <summary>
    /// 
    /// </summary>
    private byte    m_btTag = 255;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iPictureDataOffset;
    /// <summary>
    /// 
    /// </summary>
    private Image m_picture;

    private Stream m_pictStream;
    /// <summary>
    /// 
    /// </summary>
    private MemoryStream m_stream;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public MsoBitmapPicture( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }

    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Index of offset.</param>
    public MsoBitmapPicture( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="stream">Stream to get data from.</param>
    public MsoBitmapPicture( MsoBase parent, Stream stream )
      : base( parent, stream, null )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets picture of that is contained in the record.
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
          throw new ArgumentNullException( "value" );

        m_picture = value;
        EvaluateHash();
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
    /// <summary>
    /// 
    /// </summary>
    public byte Tag
    {
      get
      {
        return m_btTag;
      }
      set
      {
        m_btTag = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int PictureDataOffset
    {
      get
      {
        return m_iPictureDataOffset;
      }
      set
      {
        m_iPictureDataOffset = value;
      }
    }
    /// <summary>
    /// Indicates whether this is dib bitmap.
    /// </summary>
    public bool IsDib
    {
      get
      {
        MsofbtBSE bse = Parent as MsofbtBSE;
        return bse.BlipType == MsoBlipType.msoblipDIB;
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
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords )
    {
      //AutoGrowData = true;
      //      m_usVersionAndInst = 6;
      
      if( MsoRecordType == ( MsoRecords )0 )
      {
        // TODO: replace constant.
        MsoRecordType = ( MsoRecords )61470;
      }

      m_iLength = 0;
      stream.Write( m_arrRgbUid, 0, 16 );
      m_iLength += 16;
      if (HasTwoUIDs())
      {
          stream.Write(m_arrRgbUidPrimary, 0, 16);
          m_iLength += 16;
      }

      //SetByte( m_iLength, m_btTag );
      stream.WriteByte( m_btTag );
      m_iLength++;

      int iStart = IsDib ? DEF_DIB_HEADER_SIZE : 0;

      MemoryStream tempStream = new MemoryStream();
#if !SILVERLIGHT && !WINRT && !WP
      m_picture.Save( tempStream, MsofbtBSE.ConvertToImageFormat( ( Parent as MsofbtBSE ).BlipType ) );
#else
      byte[] data = m_picture.ImageData;
      tempStream.Write( data, 0, data.Length );
#endif
      tempStream.Position = 0;
      //tempStream.WriteTo( stream );
      int iPictureLength = ( int )tempStream.Length;

      const int BufferSize = 10240;
      byte[] arrBuffer = new byte[ BufferSize ];
      tempStream.Position = iStart;

      iOffset = iStart;
      while( iOffset < iPictureLength )
      {
        int iReadBytes = tempStream.Read( arrBuffer, 0, BufferSize );
        stream.Write( arrBuffer, 0, iReadBytes );
        iOffset += iReadBytes;
      }

      m_iLength += iPictureLength - iStart;

#if SHOW_PICTURES
      Form frm = new Form();
      frm.StartPosition = FormStartPosition.CenterParent;
      frm.BackgroundImage = m_picture;
      frm.ClientSize = new Size( m_picture.Width, m_picture.Height );
      frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      frm.Text = "InfillInternalData";
      frm.ShowDialog();
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      //AutoExtractFields();
      stream.Read( m_arrRgbUid, 0, 16 );
      //Array.Copy( m_data, 0, m_arrRgbUid, 0, 16 );

      int iOffset = LoadPrimaryUID( stream ) + 16;

      m_btTag = ( byte )stream.ReadByte();//GetByte( iOffset );
      iOffset++;

      m_iPictureDataOffset = iOffset;

      CreateImageStream( stream, iOffset );

#if SKV_DEBUG
//      using( FileStream pictureStream = new FileStream( "D:\\picture.bmp", FileMode.Create ) )
//      {
//        //pictureStream.Write( m_data, iOffset, m_data.Length - iOffset );
//        m_stream.WriteTo( pictureStream );
//        m_stream.Position = 0;
//      }
#endif

#if !SILVERLIGHT && !WINRT && !WP
      m_picture = Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateImage( m_stream );
#else
      m_picture = new Image( m_stream );
#endif

#if SHOW_PICTURES
      Form frm = new Form();
      frm.StartPosition = FormStartPosition.CenterParent;
      frm.BackgroundImage = m_picture;
      frm.ClientSize = new Size( m_picture.Width, m_picture.Height );
      frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      frm.Text = "ParseStructure";
      frm.ShowDialog();
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int LoadPrimaryUID( Stream stream )
    {
      int iResult = 0;
      
      if (HasTwoUIDs())
      {
        m_arrRgbUidPrimary = new byte[ 16 ];
        stream.Read( m_arrRgbUidPrimary, 0, 16 );
        iResult += 16;
      }

      return iResult;
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsoBitmapPicture instance = ( MsoBitmapPicture )base.InternalClone();

      if( m_arrRgbUid != null )
      {
        instance.m_arrRgbUid = CloneUtils.CloneByteArray( m_arrRgbUid );
      }

      if( m_stream != null )
      {
        instance.m_stream = UtilityMethods.CloneStream( m_stream );
      }

      if( m_picture != null )
      {
        instance.m_picture = ( m_stream != null ) ?
#if !SILVERLIGHT && !WINRT && !WP
          Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateImage( instance.m_stream ) :
#else
          instance.m_picture = new Image( instance.m_stream ) :
#endif
          ( Image )m_picture.Clone();
      }

      return instance;
    }
    /// <summary>
    /// Creates image stream and fills it with necessary data.
    /// </summary>
    /// <param name="stream">Stream to read image data from.</param>
    /// <param name="iOffset">Offset to the image data.</param>
    private void CreateImageStream( Stream stream, int iOffset )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( m_stream != null ) 
#if ( WINRT )
          m_stream.Dispose();
#else
              m_stream.Close();
#endif

      int iDataSize = m_iLength - iOffset;
      bool bDib = IsDib;
      int iFullSize = iDataSize + ( bDib ? DEF_DIB_HEADER_SIZE : 0 );
      m_stream = new MemoryStream( iDataSize + DEF_DIB_HEADER_SIZE );

      int iBytesToRead = iDataSize;

      if( bDib )
      {
        uint uiSize = ReadUInt32( stream );
        stream.Position -= 4;
        AddBitMapHeaderToStream( m_stream, iFullSize, uiSize, GetDibColorsCount( stream, iOffset ) );
      }

      const int BufferSize = 10240;
      byte[] arrBuffer = new byte[ BufferSize ];
      int iReadCount;

      while( ( iReadCount = stream.Read( arrBuffer, 0, Math.Min( BufferSize, iBytesToRead ) ) ) > 0 && iBytesToRead > 0 )
      {
        m_stream.Write( arrBuffer, 0, iReadCount );
        iBytesToRead -= iReadCount;
      }

      m_stream.Position = 0;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Indicates whether Art Blip contains Two Unique Id's.
    /// </summary>
    /// <returns></returns>
    private bool HasTwoUIDs()
    {
        int instance = Instance;
        return instance == BlipDIBWithTwoUIDs ||
            instance == BlipPNGWithTwoUIDs ||
            instance == BlipJPEGWithTwoUIDs;
    }
    /// <summary>
    /// Returns number of used colors in the dib image.
    /// </summary>
    /// <returns>Number of used colors in the dib image.</returns>
    private uint GetDibColorsCount( Stream stream, int iOffset )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      stream.Position += DEF_COLOR_USED_OFFSET;
      stream.Read( arrBuffer, 0, ExcelConstants.IntSize );
      stream.Position -= DEF_COLOR_USED_OFFSET + ExcelConstants.IntSize;

      return BitConverter.ToUInt32( arrBuffer, 0 );
    }
    /// <summary>
    /// Evaluates hash value for the stored picture.
    /// </summary>
    private void EvaluateHash()
    {
      MemoryStream stream = new MemoryStream();

#if !SILVERLIGHT && !WINRT && !WP
      m_picture.Save( stream, MsofbtBSE.ConvertToImageFormat( ( Parent as MsofbtBSE ).BlipType ) );
#else
      byte[] data = m_picture.ImageData;
      stream.Write( data, 0, data.Length );
#endif
      stream.Position = 0;
      //byte[] buffer = stream.GetBuffer();
      
      //Possible methods for FIPS enabled system
      // m_arrRgbUid = stream.GetBuffer();
      //(new SHA1CryptoServiceProvider().ComputeHash(stream)).CopyTo(m_arrRgbUid, 0);
      //(new HMACSHA1().ComputeHash(stream)).CopyTo(m_arrRgbUid, 0);
      try
      {
        byte[] hash = ( new SHA1Managed().ComputeHash( stream ) );
        Buffer.BlockCopy( hash, 0, m_arrRgbUid, 0, m_arrRgbUid.Length );
      }
      catch (InvalidOperationException)
      {
#if !SILVERLIGHT && !WINRT && !WP
        ( new MACTripleDES().ComputeHash( stream ) ).CopyTo( m_arrRgbUid, 0 );
#endif
      }
    }
    /// <summary>
    /// Adds bitmap header to stream.
    /// </summary>
    public static void AddBitMapHeaderToStream( MemoryStream ms, int iFullSize
      , uint uiSize, uint dibColorCount )
    {
      byte[] arrBuffer = BitConverter.GetBytes( iFullSize );

      ms.Write( DEF_DIB_ID, 0, DEF_DIB_ID.Length );
      ms.Write( arrBuffer, 0, arrBuffer.Length );
      ms.Write( DEF_RESERVED, 0, DEF_RESERVED.Length );

      uint uiDataOffset = uiSize + DEF_DIB_HEADER_SIZE + dibColorCount * DEF_COLOR_SIZE;

      arrBuffer = BitConverter.GetBytes( uiDataOffset );
      ms.Write( arrBuffer, 0, arrBuffer.Length );
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// 
    /// </summary>
    protected override void OnDispose()
    {
      if( m_stream != null )
      {
#if ( WINRT )
          m_stream.Dispose();
#else
        m_stream.Close();
        m_stream.Dispose();
#endif
        m_stream = null;
      }
    }

    #endregion
  }
}
