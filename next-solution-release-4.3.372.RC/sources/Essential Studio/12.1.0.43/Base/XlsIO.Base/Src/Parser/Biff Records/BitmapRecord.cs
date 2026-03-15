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
using System.Runtime.InteropServices;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO;
#endif

#if  SILVERLIGHT
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

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
  /// This record is part of the Page Settings Block.
  /// It stores the background bitmap of a worksheet.
	/// </summary>
	[ Biff( TBIFFRecord.Bitmap ) ]
  [ CLSCompliant( false ) ]
	public class BitmapRecord : BiffContinueRecordRaw
	{
    #region Class constants
    /// <summary>
    /// Pixel rows in bitmap are aligned to this number of bytes.
    /// </summary>
    private const int DEF_ALIGN = 4;
    /// <summary>
    /// Header start.
    /// </summary>
    private const int DEF_HEADER_START = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Unknown value.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usUnknown = 9;
    /// <summary>
    /// Unknown value.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usUnknown2 = 1;
    /// <summary>
    /// Total size of the following record data, without this field (including CONTINUE records).
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iTotalSize;
    /// <summary>
    /// Header structure size.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iHeaderSize = 0x0C;
    /// <summary>
    /// Width of the picture, in pixel.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usWidth;
    /// <summary>
    /// Height of the picture, in pixel.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usHeight;
    /// <summary>
    /// Number of planes, must be 1.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usPlanes = 1;
    /// <summary>
    /// Color depth, must be 24 bit true-color.
    /// </summary>
    [ BiffRecordPos( 18, 2 ) ]
    private ushort m_usColorDepth = 24;
    /// <summary>
    /// Image contained in the record.
    /// </summary>
    private
#if !SILVERLIGHT && !WINRT && !WP
      Bitmap
#else
      Image
#endif
      m_bitmap;
    /// <summary>
    /// Allocated unmanaged memory.
    /// </summary>
    private IntPtr m_scan0;
    #endregion

    #region Class properties
    /// <summary>
    /// Unknown value.
    /// </summary>
    public ushort Unknown
    {
      get
      {
        return m_usUnknown;
      }
      set
      {
        m_usUnknown = value;
      }
    }
    /// <summary>
    /// Unknown value.
    /// </summary>
    public ushort Unknown2
    {
      get
      {
        return m_usUnknown2;
      }
      set
      {
        m_usUnknown2 = value;
      }
    }
    /// <summary>
    /// Total size of the following record data, without this field (including CONTINUE records).
    /// </summary>
    public int TotalSize
    {
      get
      {
        return m_iTotalSize;
      }
      set
      {
        m_iTotalSize = value;
      }
    }
    /// <summary>
    /// Header structure size.
    /// </summary>
    public int HeaderSize
    {
      get
      {
        return m_iHeaderSize;
      }
      set
      {
        m_iHeaderSize = value;
      }
    }
    /// <summary>
    /// Width of the picture, in pixel.
    /// </summary>
    public ushort Width
    {
      get
      {
        return m_usWidth;
      }
      set
      {
        m_usWidth = value;
      }
    }
    /// <summary>
    /// Height of the picture, in pixel.
    /// </summary>
    public ushort Height
    {
      get
      {
        return m_usHeight;
      }
      set
      {
        m_usHeight = value;
      }
    }
    /// <summary>
    /// Number of planes, must be 1.
    /// </summary>
    public ushort Planes
    {
      get
      {
        return m_usPlanes;
      }
      set
      {
        m_usPlanes = value;
      }
    }
    /// <summary>
    /// Color depth, must be 24 bit true-color.
    /// </summary>
    public ushort ColorDepth
    {
      get
      {
        return m_usColorDepth;
      }
      set
      {
        m_usColorDepth = value;
      }
    }
    /// <summary>
    /// Image.
    /// </summary>
    public
#if !SILVERLIGHT && !WINRT && !WP
      Bitmap
#else
      Image
#endif
      Picture
    {
      get
      {
        //if( m_bitmap == null )
        //{
        //  m_bitmap = new Bitmap( m_usWidth, m_usHeight, PixelFormat.Format24bppRgb );
        //}

        return m_bitmap;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

#if !SILVERLIGHT && !WINRT && !WP && !MediumTrust
        if( m_scan0.ToInt64() != 0 )
        {
          Marshal.FreeHGlobal( m_scan0 );
          m_scan0 = IntPtr.Zero;
        }
#endif

        m_bitmap = value;
        m_usWidth = ( ushort )Picture.Width;
        m_usHeight = ( ushort )Picture.Height;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default Constructor
    /// </summary>
    public  BitmapRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  BitmapRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for the record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  BitmapRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization

    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      // Call to extract Continue Records after this record until other 
      // records in stream.
      base.ParseStructure();
      m_usUnknown = GetUInt16( m_data, 0 );
      m_usUnknown2 = GetUInt16( m_data, 2 );
      m_iTotalSize = GetInt32( m_data, 4 );
      m_iHeaderSize = GetInt32( m_data, 8 );
      m_usWidth = GetUInt16( m_data, 12 );
      m_usHeight = GetUInt16( m_data, 14 );
      m_usPlanes = GetUInt16( m_data, 16 );
      m_usColorDepth = GetUInt16( m_data, 18 );
      //AutoExtractFields();

      int iStride = 3 * m_usWidth;
      int iMod = iStride % DEF_ALIGN;

      if( iMod != 0 )
      {
        iMod = DEF_ALIGN - iMod;
        iStride += iMod;
      }

#if !SILVERLIGHT && !WINRT && !WP && !MediumTrust
      int iSize = TotalSize - HeaderSize;
      IntPtr scan0 = Marshal.AllocHGlobal( iSize );
      IntPtr scanLast = ( IntPtr )( scan0.ToInt64() + iSize );
      //IntPtr scanLast = scan0;
      int iPictureOffset = DEF_HEADER_START + HeaderSize;
      Marshal.Copy( m_data, iPictureOffset, scan0, iSize );

      // -iStride to make bitmap having correct direction.
      m_bitmap = new Bitmap( m_usWidth, m_usHeight, -iStride, PixelFormat.Format24bppRgb, scanLast );

      m_scan0 = scan0;
#else
      int iSize = TotalSize - HeaderSize;
      byte[] scan0 = new byte[ iSize ];
      int iPictureOffset = DEF_HEADER_START + HeaderSize;
      Buffer.BlockCopy( m_data, iPictureOffset, scan0, 0, iSize );
      MemoryStream imageStream = new MemoryStream( scan0 );

      // -iStride to make bitmap having correct direction.
#if  (SILVERLIGHT) || ( WINRT ) || (WP)
      m_bitmap = new Image( imageStream );
#else
      m_bitmap = new Bitmap( imageStream );
#endif
#endif
      //InfillInternalDataTest();
      //Marshal.FreeHGlobal( scan0 );
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      byte[] arrData = GetImageData();
      AutoGrowData = true;
      // We must set Length property to value equal to this record only size.
      int iImageDataSize = arrData.Length;
      int iSize = m_iLength = iImageDataSize + HeaderSize + DEF_HEADER_START;

      m_iLength = ( iSize > this.MaximumRecordSize ) ? 
        this.MaximumRecordSize : iSize;
      int iStartOffset = ManualHeaderInfill();
      SetBytes( iStartOffset, arrData, 0, m_iLength - iStartOffset );

      // NOTE: Call to create Continue Builder. Continue Builder will start to
      // put data after this Record Position + MaximumRecordSize... that is why 
      // m_data must be filled first by record data and only after that we can 
      // add continue records.
      base.InfillInternalData( version );

      if( iSize > this.MaximumRecordSize )
      {
        int iPos = m_iLength - iStartOffset;
        int iLen = iImageDataSize - iPos;
        
        Builder.AppendBytes( arrData, iPos, iLen );
        m_iLength = Builder.Total;
      }
    }

    private byte[] GetImageData()
    {
#if !SILVERLIGHT && !WINRT && !WP
      BitmapData imageData = m_bitmap.LockBits( new Rectangle( 0, 0, m_bitmap.Width, m_bitmap.Height ),
        ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

      int iStride = imageData.Stride;
      int iAbsStride = Math.Abs( iStride );

      int iImageDataSize = Math.Abs( iStride ) * m_bitmap.Height;
      m_iTotalSize = iImageDataSize + DEF_HEADER_START;
      int iSize = m_iTotalSize + HeaderSize;
      int iPictureOffset = DEF_HEADER_START + HeaderSize;
      byte[] arrData = new byte[ iImageDataSize ];

      IntPtr scan = imageData.Scan0;

      if( iStride > 0 )
      {
        scan = ( IntPtr )( scan.ToInt64() + iImageDataSize - iStride );
        //scan0 = ( IntPtr )( ( int )scan0 - iImageDataSize );
        //scan = ( IntPtr )( scan.ToInt64() + iStride );
      }
      else
      {
        scan = ( IntPtr )( scan.ToInt64() - iImageDataSize );
      }


      for( int i = 0, iOffset = /*iImageDataSize - iAbsStride*/ 0;
        i < m_usHeight; i++, iOffset += iAbsStride )
      {
        Marshal.Copy( scan, arrData, iOffset, iAbsStride );
        scan = ( IntPtr )( scan.ToInt64() - iStride );
      }

      m_bitmap.UnlockBits( imageData );
#else
      byte[] sourceData = m_bitmap.ImageData;
      int iDataSize = sourceData.Length;
      int iDataOffset = 0;
      //const int BitmapDataStartOffset = 0x0A;
      //int iDataOffset = BitConverter.ToInt32( sourceData, BitmapDataStartOffset );
      //iDataSize -= iDataOffset;

      byte[] arrData = new byte[ iDataSize ];
      Buffer.BlockCopy( sourceData, iDataOffset, arrData, 0, iDataSize );
      m_iTotalSize = iDataSize + DEF_HEADER_START;
#endif

      return arrData;
    }

    /// <summary>
    /// Infills record header.
    /// </summary>
    /// <returns>Size of the record header.</returns>
    private int ManualHeaderInfill()
    {
      SetUInt16( 0, m_usUnknown );
      SetUInt16( 2, m_usUnknown2 );
      SetInt32( 4, m_iTotalSize );
      SetInt32( 8, m_iHeaderSize );
      SetUInt16( 12, m_usWidth );
      SetUInt16( 14, m_usHeight );
      SetUInt16( 16, m_usPlanes );
      SetUInt16( 18,  m_usColorDepth );
      return DEF_HEADER_START + HeaderSize;
    }
    #endregion
  }
}
