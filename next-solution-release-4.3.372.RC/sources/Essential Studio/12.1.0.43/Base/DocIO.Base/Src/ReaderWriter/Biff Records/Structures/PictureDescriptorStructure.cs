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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
  /// <summary>
  /// Summary description for PictureDescriptorStructure.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  [ CLSCompliant( false ) ]
  public class PictureDescriptorStructure
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_LENGTH = 68;
    #endregion

    #region Class members
    /// <summary>
    /// number of bytes in the PIC structure plus size of following 
    /// picture data 
    /// </summary>
    //    [ FieldOffset( 0 ) ]
    private uint m_lcb;
    /// <summary>
    /// number of bytes in the PIC 
    /// </summary>
    //    [ FieldOffset (4)]
    private ushort m_cbHeader;
    //	  [ FieldOffset( 6 ) ]
    private short m_mm;
    /// <summary>
    /// When the PIC describes a bitmap, xExt is the width 
    /// of the bitmap in pixels and yExt is the height of the bitmap 
    /// in pixels.
    /// </summary>
    //    [ FieldOffset( 8 ) ]
    private short m_xExt;
    //   [ FieldOffset( 10 ) ]
    private short m_yExt;
    //    [ FieldOffset( 12 ) ]
    private short m_hMF;
    /// <summary>
    /// Window's bitmap structure when PIC describes a BITMAP
    /// </summary>
    //    [ FieldOffset( 14 ) ]
    [ MarshalAs( UnmanagedType.ByValArray, SizeConst = 14 ) ]
    private byte[] m_bmpStruct = new byte[ 14 ];
    /// <summary>
    /// horizontal measurement in twips of the rectangle the 
    /// picture should be imaged within
    /// </summary>
    //    [ FieldOffset( 28 ) ]
    private short m_dxGoal;
    /// <summary>
    /// vertical measurement in twips of the rectangle 
    /// the picture should be imaged within.
    /// </summary>
    ///    [ FieldOffset( 30 ) ]
    private short m_dyGoal;
    /// <summary>
    /// horizontal scaling factor supplied by user expressed in .001% units.
    /// </summary>
    //    [ FieldOffset( 32 ) ]
    private ushort m_horScaling;
    /// <summary>
    /// vertical scaling factor supplied by user expressed in .001% units.
    /// </summary>
    //   [ FieldOffset( 34 ) ]
    private ushort m_verScaling;
    /// <summary>
    /// the amount the picture has been cropped on the left in twips.
    /// </summary>
    //    [ FieldOffset( 36 ) ]
    private short m_dxCropLeft;
    /// <summary>
    /// the amount the picture has been cropped on the top in twips.
    /// </summary>
    //   [ FieldOffset( 38 ) ]
    private short m_dxCropTop;
    /// <summary>
    /// the amount the picture has been cropped on the right in twips.
    /// </summary>
    //    [ FieldOffset( 40 ) ]
    private short m_dxCropRight;
    /// <summary>
    /// the amount the picture has been cropped on the bottom in twips.
    /// </summary>
    //    [ FieldOffset( 42 ) ]
    private short m_dxCropBottom;
    /// <summary>
    /// brcl short :4 000F Obsolete, superseded by brcTop, etc. In WinWord 1.x, it was the type of border to place around picture
    ///        0 single
    ///        1 thick
    ///        2 double
    ///        3 shadow 
    /// fFrameEmpty short :1  0010 picture consists of a single frame 
    /// fBitmap short :1      0020 ==1, when picture is just a bitmap 
    /// fDrawHatch short :1   0040 ==1, when picture is an active OLE object 
    /// fError short :1       0080 ==1, when picture is just an error message 
    /// bpp short :8  bits per pixel
    ///                   0 unknown
    ///                   1 monochrome
    ///                   4 VGA 
    /// </summary>
    //	  [ FieldOffset( 44 ) ]
    private short m_pictureInfo;
    /// <summary>
    /// specification for border above picture
    /// </summary>
    //    [ FieldOffset( 46 ) ]
    private uint m_brcTop;
    /// <summary>
    /// specification for border to the left of
    /// </summary>
    //    [ FieldOffset( 50 ) ]
    private uint m_brcLeft;
    /// <summary>
    /// specification for border below picture
    /// </summary>
    //    [ FieldOffset( 54 ) ]
    private uint m_brcBottom;
    /// <summary>
    /// specification for border to the right of
    /// </summary>
    //    [ FieldOffset( 58 ) ]
    private uint m_brcRight;
    /// <summary>
    /// horizontal offset of hand annotation origin
    /// </summary>
    //    [ FieldOffset( 62 ) ]
    private short m_dxaOrigin;
    /// <summary>
    /// vertical offset of hand annotation origin
    /// </summary>
    //    [ FieldOffset( 64 ) ]
    private short m_dyaOrigin;
    /// <summary>
    /// unused
    /// </summary>
    //    [ FieldOffset( 66 ) ]
    private short m_cProps;
    [ MarshalAs( UnmanagedType.ByValArray, SizeConst = 2 ) ]
    private byte[] m_rgb = new byte[ 2 ];
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int Length
    {
      get
      {
        return DEF_LENGTH;
      }
    }
    /// <summary>
    /// number of bytes in the PIC structure plus size of following picture data
    /// </summary>
    public uint SumSize
    {
      get
      {
        return m_lcb;
      }
      set
      {
        m_lcb = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort StructSize
    {
      get
      {
        return m_cbHeader;
      }
      set
      {
        m_cbHeader = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short MfpMM
    {
      get
      {
        return m_mm;
      }
      set
      {
        m_mm = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short MpfxExt
    {
      get
      {
        return m_xExt;
      }
      set
      {
        m_xExt = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short MpfyExt
    {
      get
      {
        return m_yExt;
      }
      set
      {
        m_yExt = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short MpfhMF
    {
      get
      {
        return m_hMF;
      }
      set
      {
        m_hMF = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public byte[ ] ArrBITMAP
    {
      get
      {
        return m_bmpStruct;
      }
      set
      {
        m_bmpStruct = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short XMeasurement
    {
      get
      {
        return m_dxGoal;
      }
      set
      {
        m_dxGoal = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short YMeasurement
    {
      get
      {
        return m_dyGoal;
      }
      set
      {
        m_dyGoal = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort XScale
    {
      get
      {
        return m_horScaling;
      }
      set
      {
        m_horScaling = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort YScale
    {
      get
      {
        return m_verScaling;
      }
      set
      {
        m_verScaling = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short CropLeft
    {
      get
      {
        return m_dxCropLeft;
      }
      set
      {
        m_dxCropLeft = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short CropRight
    {
      get
      {
        return m_dxCropRight;
      }
      set
      {
        m_dxCropRight = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short CropTop
    {
      get
      {
        return m_dxCropTop;
      }
      set
      {
        m_dxCropTop = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short CropBottom
    {
      get
      {
        return m_dxCropBottom;
      }
      set
      {
        m_dxCropBottom = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short PicInfo
    {
      get
      {
        return m_pictureInfo;
      }
      set
      {
        m_pictureInfo = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint BorderLeft
    {
      get
      {
        return m_brcLeft;
      }
      set
      {
        m_brcLeft = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint BorderRight
    {
      get
      {
        return m_brcRight;
      }
      set
      {
        m_brcRight = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint BorderTop
    {
      get
      {
        return m_brcTop;
      }
      set
      {
        m_brcTop = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint BorderBottom
    {
      get
      {
        return m_brcBottom;
      }
      set
      {
        m_brcBottom = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short XOrigin
    {
      get
      {
        return m_dxaOrigin;
      }
      set
      {
        m_dxaOrigin = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public short YOrigin
    {
      get
      {
        return m_dyaOrigin;
      }
      set
      {
        m_dyaOrigin = value;
      }
    }
    /// <summary>
    /// unused
    /// </summary>
    public short Props
    {
      get
      {
        return m_cProps;
      }
      set
      {
        m_cProps = value;
      }
    }
    /// <summary>
    /// unused
    /// </summary>
    public byte[ ] RGB
    {
      get
      {
        return m_rgb;
      }
      set
      {
        m_rgb = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal PictureDescriptorStructure Clone()
    {
      PictureDescriptorStructure pictStruct = ( PictureDescriptorStructure )this.MemberwiseClone();
      m_rgb.CopyTo( pictStruct.m_rgb, 0 );
      m_bmpStruct.CopyTo( pictStruct.m_bmpStruct, 0 );
      return  pictStruct;
    }
    #endregion
  }
}