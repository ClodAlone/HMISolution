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
using System.Drawing;
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.Layouting.Native
{
  #region ABC
  /// <summary>
  /// ABC structure.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential )
    , CLSCompliant( false ) ]
  internal struct ABC
  {
    public int abcA;
    public int abcB;
    public int abcC;
  }
  #endregion

  #region POINT
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct POINT
  {
    public int x;
    public int y;
    public POINT( int X, int Y )
    {
      x = X;
      y = Y;
    }
    /// <summary>
    /// Point creation from lParam's data.
    /// </summary>
    /// <param name="lParam">lParam's data for initialing point structure.</param>
    public POINT( int lParam )
    {
      x = ( lParam & 0xffff );
      y = ( lParam >> 16 );
    }
    public static implicit operator Point( POINT p )
    {
      return new Point( p.x, p.y );
    }
    public static implicit operator PointF( POINT p )
    {
      return new PointF( p.x, p.y );
    }
    public static implicit operator POINT( Point p )
    {
      return new POINT( p.X, p.Y );
    }
  }
  #endregion

  #region POINTS
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct POINTS
  {
    public short x;
    public short y;
    public POINTS( short X, short Y )
    {
      x = X;
      y = Y;
    }
    public static implicit operator Point( POINTS p )
    {
      return new Point( p.x, p.y );
    }
    public static implicit operator PointF( POINTS p )
    {
      return new PointF( p.x, p.y );
    }
    public static implicit operator POINTS( Point p )
    {
      return new POINTS( ( short )p.X, ( short )p.Y );
    }
    public static implicit operator POINTS( PointF p )
    {
      return new POINTS( ( short )p.X, ( short )p.Y );
    }
  }
  #endregion
  
  #region RECT
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;

    public RECT( int x1, int y1, int x2, int y2 )
    {
      left = x1;
      top = y1;
      right = x2;
      bottom = y2;
    }

    public int Width
    {
      get
      {
        return right - left;
      }
    }

    public int Height
    {
      get
      {
        return bottom - top;
      }
    }

    public Point TopLeft
    {
      get
      {
        return new Point( left, top );
      }
    }

    public Size Size
    {
      get
      {
        return new Size( Width, Height );
      }
    }

    public override string ToString()
    {
      return string.Format( "{0}x{1}", TopLeft, Size );
    }

    public static implicit operator Rectangle( RECT rect )
    {
      return Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
    }
    public static implicit operator RectangleF( RECT rect )
    {
      return RectangleF.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
    }
    public static implicit operator Size( RECT rect )
    {
      return new Size( rect.right - rect.left, rect.bottom - rect.top );
    }

    public static explicit operator RECT( Rectangle rect )
    {
      RECT rc = new RECT();

      rc.left = rect.Left;
      rc.right = rect.Right;
      rc.top = rect.Top;
      rc.bottom = rect.Bottom;

      return rc;
    }
  }
  #endregion

  #region SIZE
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct SIZE
  {
    public int cx;
    public int cy;
    
    public static implicit operator SizeF( SIZE rect )
    {
      return new SizeF( rect.cx, rect.cy );
    }
  }
  #endregion
  
  #region XFORM
  /// <summary>
  /// The XFORM structure specifies a world-space to page-space transformation.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct XFORM
  {
    /// <summary>
    /// Specifies scaling/rotation/reflection
    /// </summary>
    public float eM11;
    /// <summary>
    /// Specified shear/rotation
    /// </summary>   
    public float eM12;
    /// <summary>
    /// Specified shear/rotation
    /// </summary>   
    public float eM21;
    /// <summary>
    /// Specifies scaling/rotation/reflection
    /// </summary>
    public float eM22;
    /// <summary>
    /// Specifies the horizontal translation component, in logical units.
    /// </summary>
    public float eDx;
    /// <summary>
    /// Specifies the vertical translation component, in logical units.
    /// </summary>
    public float eDy;    
  }
  #endregion

  #region METAFILEPICT
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct METAFILEPICT
  {
    public int mm;
    public int xExt;
    public int yExt;
    public IntPtr hMF;
  }
  #endregion
  
  #region BITMAPINFOHEADER
  /// <summary>
  /// Record of Emf metafile.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct BITMAPINFOHEADER
  {
    public int   biSize; 
    public int   biWidth; 
    public int   biHeight; 
    public short biPlanes; 
    public short biBitCount; 
    public int   biCompression; 
    public uint  biSizeImage; 
    public int   biXPelsPerMeter; 
    public int   biYPelsPerMeter; 
    public int   biClrUsed; 
    public int   biClrImportant; 
  }
  #endregion
  
  #region BITMAPINFO
  /// <summary>
  /// Record of Emf metafile.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct BITMAPINFO
  { 
    public BITMAPINFOHEADER bmiHeader; 
    //public RGBQUAD[] bmiColors;
    public byte[] bmiColors;
  }
  #endregion
  
  #region OUTLINETEXTMETRIC
  /// <summary>
  /// Structure for information about font.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential )
  , CLSCompliant( false ) ]
  internal struct OUTLINETEXTMETRIC
  {
    public uint otmSize;
    public TEXTMETRIC otmTextMetrics;
    public byte otmFiller;
    public PANOSE otmPanoseNumber;
    public uint otmfsSelection;
    public uint otmfsType;
    public int otmsCharSlopeRise;
    public int otmsCharSlopeRun;
    public int otmItalicAngle;
    public uint otmEMSquare;
    public int otmAscent;
    public int otmDescent;
    public uint otmLineGap;
    public uint otmsCapEmHeight;
    public uint otmsXHeight;
    public RECT otmrcFontBox;
    public int otmMacAscent;
    public int otmMacDescent;
    public uint otmMacLineGap;
    public uint otmusMinimumPPEM;
    public POINT otmptSubscriptSize;
    public POINT otmptSubscriptOffset;
    public POINT otmptSuperscriptSize;
    public POINT otmptSuperscriptOffset;
    public uint otmsStrikeoutSize;
    public int otmsStrikeoutPosition;
    public int otmsUnderscoreSize;
    public int otmsUnderscorePosition;
    public IntPtr otmpFamilyName;
    public IntPtr otmpFaceName;
    public IntPtr otmpStyleName;
    public IntPtr otmpFullName;
  }
  #endregion
  
  #region PANOSE
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct PANOSE
  {
    public byte bFamilyType;
    public byte bSerifStyle;
    public byte bWeight;
    public byte bProportion;
    public byte bContrast;
    public byte bStrokeVariation;
    public byte bArmStyle;
    public byte bLetterform;
    public byte bMidline;
    public byte bXHeight;
  }
  #endregion

  #region TEXTMETRIC
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct TEXTMETRIC
  {
    public Int32 tmHeight;
    public Int32 tmAscent;
    public Int32 tmDescent;
    public Int32 tmInternalLeading;
    public Int32 tmExternalLeading;
    public Int32 tmAveCharWidth;
    public Int32 tmMaxCharWidth;
    public Int32 tmWeight;
    public Int32 tmOverhang;
    public Int32 tmDigitizedAspectX;
    public Int32 tmDigitizedAspectY;
    public byte tmFirstChar;
    public byte tmLastChar;
    public byte tmDefaultChar;
    public byte tmBreakChar;
    public byte tmItalic;
    public byte tmUnderlined;
    public byte tmStruckOut;
    public byte tmPitchAndFamily;
    public byte tmCharSet;
  }
  #endregion
  
  #region LOGFONT
  /// <summary>
  /// Record of Emf metafile.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode ) ]
  internal class LOGFONT
  {
    public int lfHeight = 0;
    public int lfWidth = 0;
    public int lfEscapement = 0;
    public int lfOrientation = 0;
    public FW_FONT_WEIGHT lfWeight = FW_FONT_WEIGHT.FW_NORMAL;
    [ MarshalAs( UnmanagedType.U1 ) ]
    public bool lfItalic = false;
    [ MarshalAs( UnmanagedType.U1 ) ]
    public bool lfUnderline = false;
    [ MarshalAs( UnmanagedType.U1 ) ]
    public bool lfStrikeOut = false;
    public byte lfCharSet = 0;
    public byte lfOutPrecision = 0;
    public byte lfClipPrecision = 0;
    public byte lfQuality = 0;
    public byte lfPitchAndFamily = 0;
    [ MarshalAs( UnmanagedType.ByValTStr, SizeConst= WinGdiConst.LF_FACESIZE ) ]
    public string lfFaceName = null;
  }
  /// <summary>
  /// Constants from WinGdi.h file.
  /// </summary>
  internal enum FW_FONT_WEIGHT : int
  {
    FW_DONTCARE = 0,
    FW_THIN = 100,
    FW_EXTRALIGHT = 200,
    FW_LIGHT = 300,
    FW_NORMAL = 400,
    FW_MEDIUM = 500,
    FW_SEMIBOLD = 600,
    FW_BOLD = 700,
    FW_EXTRABOLD = 800,
    FW_HEAVY = 900,
  }
  /// <summary>
  /// Class with simple constants.
  /// </summary>
  internal class WinGdiConst
  {
    public const int PS_STYLE_MASK = 0x0000000F;
    public const int PS_ENDCAP_MASK = 0x00000F00;
    public const int PS_JOIN_MASK = 0x0000F000;
    public const int PS_TYPE_MASK = 0x000F0000;
    public const int DIB_RGB_COLORS = 0;
    public const int LF_FULLFACESIZE = 64;
    public const int LF_FACESIZE     = 32;
    public const int ELF_VENDOR_SIZE = 4;
    public const int LOGPIXELSX    = 88;
    public const int  LOGPIXELSY   = 90;
    public const int ETO_OPAQUE   = 0x0002;
    public const int ETO_CLIPPED  = 0x0004;
  }
  #endregion
}