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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing.Imaging;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtBSE.
  /// </summary>
  [MsoDrawing( MsoRecords.msofbtBSE )]
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class MsofbtBSE : MsoBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_NAME_OFFSET = 36;
    /// <summary>
    /// 
    /// </summary>
    private static readonly MsoBlipType[] DEF_BITMAP_BLIPS = new MsoBlipType[]
    {
      MsoBlipType.msoblipDIB,
      MsoBlipType.msoblipPNG,
      MsoBlipType.msoblipJPEG,
    };
    /// <summary>
    /// 
    /// </summary>
    private static readonly MsoBlipType[] DEF_PICT_BLIPS = new MsoBlipType[]
    {
      MsoBlipType.msoblipEMF,
      MsoBlipType.msoblipPICT,
      MsoBlipType.msoblipWMF,
    };
    #endregion

    #region Class members
    /// <summary>
    /// Required type on Win32.
    /// </summary>
    [BiffRecordPos( 0, 1 )]
    private byte m_btReqWin32;
    /// <summary>
    /// Required type on Mac.
    /// </summary>
    [BiffRecordPos( 1, 1 )]
    private byte m_btReqMac;
    /// <summary>
    /// Blip size in stream.
    /// </summary>
    [BiffRecordPos( 20, 4 )]
    private uint m_uiSize;
    /// <summary>
    /// Reference count on the blip.
    /// </summary>
    [BiffRecordPos( 24, 4 )]
    private uint m_uiRefCount;
    /// <summary>
    /// File offset in the delay stream.
    /// </summary>
    [BiffRecordPos( 28, 4 )]
    private uint m_uiFileOffset;
    /// <summary>
    /// How this blip is used.
    /// </summary>
    [BiffRecordPos( 32, 1 )]
    private byte m_btUsage;
    /// <summary>
    /// Length of the blip name.
    /// </summary>
    [BiffRecordPos( 33, 1 )]
    private byte m_btNameLength;
    /// <summary>
    /// For the future.
    /// </summary>
    [BiffRecordPos( 34, 1 )]
    private byte m_btUnused1;
    /// <summary>
    /// For the future.
    /// </summary>
    [BiffRecordPos( 35, 1 )]
    private byte m_btUnused2;
    /// <summary>
    /// 
    /// </summary>
    private string m_strBlipName = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private MsoBase m_msoPicture;
    /// <summary>
    /// Index of the record in the collection.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Path to the picture item (used in Excel 2007).
    /// </summary>
    private string m_strPicturePath;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtBSE( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtBSE( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }

    protected override void OnDispose()
    {
        //TODO: Need to dispose m_msoPicture
        //if (m_msoPicture != null)
        //    m_msoPicture.Dispose();
        base.OnDispose();
    }
    public void Dispose()
    {
        if(m_msoPicture !=null )
        m_msoPicture.Dispose();

    }
    #endregion

    #region Class properties
    /// <summary>
    /// Required type on Win32.
    /// </summary>
    public byte RequiredWin32
    {
      get
      {
        return m_btReqWin32;
      }
      set
      {
        m_btReqWin32 = value;
      }
    }
    /// <summary>
    /// Required type on Mac.
    /// </summary>
    public byte RequiredMac
    {
      get
      {
        return m_btReqMac;
      }
      set
      {
        m_btReqMac = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string BlipName
    {
      get
      {
        return m_strBlipName;
      }
      set
      {
        m_strBlipName = value;
        m_btNameLength = ( value == null ) ? ( byte )0 : ( byte )m_strBlipName.Length;
      }
    }
    /// <summary>
    /// Blip size in stream.
    /// </summary>
    public uint SizeInStream
    {
      get
      {
        return m_uiSize;
      }
      set
      {
        m_uiSize = value;
      }
    }
    /// <summary>
    /// Reference count on the blip.
    /// </summary>
    public uint RefCount
    {
      get
      {
        return m_uiRefCount;
      }
      set
      {
        m_uiRefCount = value;
      }
    }
    /// <summary>
    /// File offset in the delay stream.
    /// </summary>
    public uint FileOffset
    {
      get
      {
        return m_uiFileOffset;
      }
      set
      {
        m_uiFileOffset = value;
      }
    }
    /// <summary>
    /// How this blip is used.
    /// </summary>
    public MsoBlipUsage BlipUsage
    {
      get
      {
        return ( MsoBlipUsage )m_btUsage;
      }
      set
      {
        m_btUsage = ( byte )value;
      }
    }
    /// <summary>
    /// Length of the blip name.
    /// </summary>
    public byte NameLength
    {
      get
      {
        return m_btNameLength;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public byte Unused1
    {
      get
      {
        return m_btUnused1;
      }
#if DEBUG
      set
      {
        m_btUnused1 = value;
      }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public byte Unused2
    {
      get
      {
        return m_btUnused2;
      }
#if DEBUG
      set
      {
        m_btUnused2 = value;
      }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBlipType BlipType
    {
      get
      {
        return ( MsoBlipType )this.Instance;
      }
      set
      {
        Instance = ( int )value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IPictureRecord PictureRecord
    {
      get
      {
        return m_msoPicture as IPictureRecord;
      }
      set
      {
        m_msoPicture = value as MsoBase;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets / sets index of the bse in the collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Gets / sets path to the picture zip item. Used in Excel 2007 format.
    /// </summary>
    public string PicturePath
    {
      get
      {
        return m_strPicturePath;
      }
      set
      {
        m_strPicturePath = value;
      }
    }

    #region /* comments */
#if DEBUG
    /// <summary>
    /// Returns Picture.
    /// </summary>
    public MsoBase[] PictureRec
    {
      get
      {
        return new MsoBase[] { m_msoPicture };
      }
    }
#endif
    //*/    
    #endregion

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

      m_uiSize = 0;
      m_btNameLength = 0;

      m_iLength = 36;
      stream.WriteByte( m_btReqWin32 );
      //[BiffRecordPos( 1, 1 )]
      stream.WriteByte( m_btReqMac );
      stream.Position += 18;
      //[BiffRecordPos( 20, 4 )]
      long lSizePosition = stream.Position;
      WriteUInt32( stream, m_uiSize );
      WriteUInt32( stream, m_uiRefCount );
      WriteUInt32( stream, m_uiFileOffset );
      stream.WriteByte( m_btUsage );
      stream.WriteByte( m_btNameLength );
      stream.WriteByte( m_btUnused1 );
      stream.WriteByte( m_btUnused2 );

      //AutoGrowData = true;

      if( m_btNameLength > 0 )
      {
        byte[] buffer = Encoding.Unicode.GetBytes( m_strBlipName );
        //SetBytes( m_iLength, buffer );
        int iCount = buffer.Length;
        stream.Write( buffer, 0, iCount );
        m_iLength += iCount;
      }

      if( m_msoPicture != null )
      {
        long lPosition = stream.Position;
        m_msoPicture.FillArray( stream );
        m_uiSize = ( uint )( stream.Position - lPosition );
        m_iLength += ( int )m_uiSize;

        lPosition = stream.Position;
        stream.Position = lSizePosition;
        WriteUInt32( stream, m_uiSize );
        stream.Position = lPosition;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      //AutoExtractFields();
      m_btReqWin32 = ( byte )stream.ReadByte();
      //[BiffRecordPos( 1, 1 )]
      m_btReqMac = ( byte )stream.ReadByte();
      stream.Position += 18;
      //[BiffRecordPos( 20, 4 )]
      m_uiSize = ReadUInt32( stream );
      m_uiRefCount = ReadUInt32( stream );
      m_uiFileOffset = ReadUInt32( stream );
      m_btUsage = ( byte )stream.ReadByte();
      m_btNameLength = ( byte )stream.ReadByte();
      m_btUnused1 = ( byte )stream.ReadByte();
      m_btUnused2 = ( byte )stream.ReadByte();

      int iPictureOffset = DEF_NAME_OFFSET;

      if( m_btNameLength > 0 )
      {
        byte[] arrData = new byte[ m_btNameLength ];
        stream.Read( arrData, 0, m_btNameLength );
        m_strBlipName = Encoding.Unicode.GetString( arrData, 0, arrData.Length );
        iPictureOffset += m_btNameLength;
      }

      if( iPictureOffset == m_iLength )
      {
        m_msoPicture = null;
        return;
      }
      else if( Array.IndexOf( DEF_BITMAP_BLIPS, BlipType ) != -1 )
      {
        m_msoPicture = new MsoBitmapPicture( this, stream );
      }
      else if( Array.IndexOf( DEF_PICT_BLIPS, BlipType ) != -1 )
      {
        m_msoPicture = new MsoMetafilePicture( this, stream );
      }

      #region /* Comment2 picture saving for debugging */
      /*
//      byte[] pictureData = GetBytes( iPictureOffset, Length - iPictureOffset );
//      int iOffset = 0;
//      m_msoPicture = MsoFactory.CreateMsoRecord( pictureData, ref iOffset );
//      MemoryStream stream = new MemoryStream( pictureData );
//      FileStream fileStream = new FileStream( "D:\\picture.bmp", FileMode.Create );
//      fileStream.Write( pictureData, 25, pictureData.Length - 25 );
//      fileStream.Close();
//      
//      fileStream = new FileStream( "D:\\pictureData.bmp", FileMode.Create );
//      fileStream.Write( pictureData, 0, pictureData.Length );
//      fileStream.Close();
//
//      stream.Close();
//      stream.Position = 0;

//      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( stream );
      */
      #endregion
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsofbtBSE instance = ( MsofbtBSE )base.InternalClone();

      if( m_msoPicture != null )
      {
        instance.m_msoPicture = m_msoPicture.Clone( instance );
      }

      return instance;
    }
    #endregion

    #region Class helper methods
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// 
    /// </summary>
    /// <param name="blipType"></param>
    /// <returns></returns>
    public static ImageFormat ConvertToImageFormat( MsoBlipType blipType )
    {
      switch( blipType )
      {
        case MsoBlipType.msoblipDIB:
          return ImageFormat.Bmp;

        case MsoBlipType.msoblipJPEG:
          return ImageFormat.Jpeg;

        case MsoBlipType.msoblipEMF:
          return ImageFormat.Emf;

        case MsoBlipType.msoblipWMF:
          return ImageFormat.Wmf;

        case MsoBlipType.msoblipPNG:
        default:
          return ImageFormat.Png;
      }
    }
#endif
    #endregion
  }
}
