#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
{
    #region Description
    /*
	 * HANDLE, PSTR - IntPtr
	 * BYTE, TCHAR - Byte
	 * SHORT - Int16
	 * WORD - UInt16
	 * INT - Int32
	 * UINT - UInt32
	 * Int32 - Int32
	 * BOOL - Int32
	 * DWORD - UInt32
	 * LONG - Int32
	 * ULONG - UInt32
	 * CHAR - Char
	 * LPSTR - String
	 * FLOAT - Single
	 * DOUBLE - Double
	*/
    #endregion

    #region EMR_SETMITERLIMIT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETMITERLIMIT
    {
        /// <summary>
        /// New miter limit.
        /// </summary>
        public int eMiterLimit;
    }
    #endregion

    #region EMR_MODIFYWORLDTRANSFORM
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_MODIFYWORLDTRANSFORM
    {
        public XFORM xform;
        public MWT_DATA iMode;
    }
    #endregion

    #region XFORM
    /// <summary>
    /// The XFORM structure specifies a world-space to page-space transformation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
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

        public override string ToString()
        {
            return eM11 + " " + eM12 + " " + eM21 + " " + eM22 + " " + eDx + " " + eDy;
        }

    }
    #endregion

    #region EMR_EXTCREATEPEN
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_EXTCREATEPEN
    {
        public int ihPen;
        public int offBmi;
        public int cbBmi;
        public int offBits;
        public int cbBits;
        public int elpPenStyle;
        public int elpWidth;
        public uint elpBrushStyle;
        public int elpColor;
        public IntPtr elpHatch;
        public int elpNumEntries;
        public int[] elpStyleEntry;
    }
    #endregion

    #region POINT
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;

        public POINT(int X, int Y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POINT"/> struct.
        /// </summary>
        /// <param name="lParam">lParam's data for initialing point structure.</param>
        public POINT(int lParam)
        {
            x = (lParam & 0xffff);
            y = (lParam >> 16);
        }

        public static implicit operator Point(POINT p)
        {
            return new Point(p.x, p.y);
        }

        public static implicit operator PointF(POINT p)
        {
            return new PointF(p.x, p.y);
        }

        public static implicit operator POINT(Point p)
        {
            return new POINT(p.X, p.Y);
        }

    }
    #endregion

    #region POINTS
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINTS
    {
        public short x;
        public short y;

        public POINTS(short X, short Y)
        {
            x = X;
            y = Y;
        }

        public static implicit operator Point(POINTS p)
        {
            return new Point(p.x, p.y);
        }

        public static implicit operator PointF(POINTS p)
        {
            return new PointF(p.x, p.y);
        }

        public static implicit operator POINTS(Point p)
        {
            return new POINTS((short)p.X, (short)p.Y);
        }

        public static implicit operator POINTS(PointF p)
        {
            return new POINTS((short)p.X, (short)p.Y);
        }

    }
    #endregion

    #region LOGBRUSH32
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct LOGBRUSH32
    {
        public BS_BRUSH_STYLE lbStyle;
        public int lbColor;
        public uint lbHatch;
    }
    #endregion

    #region EMR_CREATEBRUSHINDIRECT
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_CREATEBRUSHINDIRECT
    {
        public int ihBrush;
        public LOGBRUSH32 lb;
    }
    #endregion

    #region EMR_SELECTCLIPPATH
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SELECTCLIPPATH
    {
        public int iMode;
    }
    #endregion

    #region RECT
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int x1, int y1, int x2, int y2)
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
                return new Point(left, top);
            }
        }

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", TopLeft, Size);
        }

        public static implicit operator System.Drawing.Rectangle(RECT rect)
        {
            return System.Drawing.Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }
        public static implicit operator RectangleF(RECT rect)
        {
            return RectangleF.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }
        public static implicit operator Size(RECT rect)
        {
            return new Size(rect.right - rect.left, rect.bottom - rect.top);
        }

        public static explicit operator RECT(System.Drawing.Rectangle rect)
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
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        public int cx;
        public int cy;

        public static implicit operator SizeF(SIZE rect)
        {
            return new SizeF(rect.cx, rect.cy);
        }
    }
    #endregion

    #region ABC
    /// <summary>
    /// ABC structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ABC
    {
        public int abcA;
        public int abcB;
        public int abcC;
    }
    #endregion

    #region ColorDataEx
    /// <summary>
    /// Structure for 32 bit images saving.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorDataEx
    {
        /// <summary>
        /// Value of Blue channel.
        /// </summary>
        public byte Blue;
        /// <summary>
        /// Value of Green channel.
        /// </summary>
        public byte Green;
        /// <summary>
        /// Value of Red channel.
        /// </summary>
        public byte Red;
        /// <summary>
        /// Value of Alpha channel.
        /// </summary>
        public byte Alpha;
    }
    #endregion

    #region ColorData
    /// <summary>
    /// Structure for 24 bit images saving.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorData
    {
        /// <summary>
        /// Value of Blue channel.
        /// </summary>
        public byte Blue;
        /// <summary>
        /// Value of Green channel.
        /// </summary>
        public byte Green;
        /// <summary>
        /// Value of Red channel.
        /// </summary>
        public byte Red;
    }
    #endregion

    #region ColorData
    /// <summary>
    /// Structure for 24 bit images saving.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorData16
    {
        /// <summary>
        /// Value of Blue channel.
        /// </summary>
        public short Blue;
        /// <summary>
        /// Value of Green channel.
        /// </summary>
        public short Green;
        /// <summary>
        /// Value of Red channel.
        /// </summary>
        public short Red;
    }
    #endregion

    #region EMR_POLYPOLYLINE16
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_POLYPOLYLINE16
    {
        public RECT rclBounds;
        public uint nPolys;
        public uint cpts;
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysUInt)]
        public uint[] aPolyCounts;
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1)]
        public POINTS[] apts;
    }
    #endregion

    #region EMR_POLYPOLYLINE
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_POLYPOLYLINE
    {
        public RECT rclBounds;
        public uint nPolys;
        public uint cpts;
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysUInt)]
        public uint[] aPolyCounts;
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1)]
        public POINT[] apts;
    }
    #endregion

    #region EMR_SETVIEWPORTEXTEX
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETVIEWPORTEXTEX
    {
        public SIZE szlExtent;
    }
    #endregion

    #region EMR_SETVIEWPORTORGEX
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETVIEWPORTORGEX
    {
        public POINT ptlOrigin;
    }
    #endregion

    #region EMR_SCALEVIEWPORTEXTEX
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SCALEVIEWPORTEXTEX
    {
        public int xNum;
        public int xDenom;
        public int yNum;
        public int yDenom;
    }
    #endregion

    #region EMR_LINETO
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_LINETO
    {
        public POINT ptl;
    }
    #endregion

    #region EMR_POLYLINE16
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_POLYLINE16
    {
        public RECT rclBounds;
        public uint cpts;
        public POINTS[] apts;
    }
    #endregion

    #region EMR_POLYLINE
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_POLYLINE
    {
        public RECT rclBounds;
        public uint cpts;
        public POINT[] apts;
    }
    #endregion

    #region EMR_ALPHABLEND
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_ALPHABLEND
    {
        public RECT rclBounds;
        public int xDest;
        public int yDest;
        public int cxDest;
        public int cyDest;
        public int dwRop;
        public int xSrc;
        public int ySrc;
        public XFORM xformSrc;
        public int crBkColorSrc;
        public int iUsageSrc;
        public int offBmiSrc;
        public int cbBmiSrc;
        public int offBitsSrc;
        public int cbBitsSrc;
        public int cxSrc;
        public int cySrc;
    }
    #endregion

    #region METAFILEPICT
    [StructLayout(LayoutKind.Sequential)]
    internal struct METAFILEPICT
    {
        public int mm;
        public int xExt;
        public int yExt;
        public IntPtr hMF;
    }
    #endregion

    #region EMR_OFFSETCLIPRGN
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_OFFSETCLIPRGN
    {
        public POINT ptlOffset;
    }
    #endregion

    #region EMR_EXCLUDECLIPRECT
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_EXCLUDECLIPRECT
    {
        public RECT rclClip;
    }
    #endregion

    #region EMR_SETARCDIRECTION
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETARCDIRECTION
    {
        public int iArcDirection;
    }
    #endregion

    #region EMR_FILLRGN
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_FILLRGN
    {
        public RECT rclBounds;
        public int cbRgnData;
        public int ihBrush;
        public RGNDATA RgnData;
    }
    #endregion

    #region RGNDATA
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RGNDATA
    {
        public RGNDATAHEADER rdh;
    }
    #endregion

    #region RGNDATAHEADER
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RGNDATAHEADER
    {
        public int dwSize;
        public int iType;
        public int nCount;
        public int nRgnSize;
        public RECT rcBound;
    }
    #endregion

    #region EMR_INVERTRGN
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_INVERTRGN
    {
        public RECT rclBounds;
        public int cbRgnData;
        public RGNDATA RgnData;
    }
    #endregion

    #region EMR_EXTSELECTCLIPRGN
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_EXTSELECTCLIPRGN
    {
        public int cbRgnData;
        public int iMode;
        public RGNDATA RgnData;
    }
    #endregion

    #region EMR_SETTEXTCOLOR
    /// <summary>
    /// Windows structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETTEXTCOLOR
    {
        public int crColor;
    }
    #endregion

    #region EMR_SETWORLDTRANSFORM
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETWORLDTRANSFORM
    {
        public XFORM xform;
    }
    #endregion

    #region EMR_CREATEPEN
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_CREATEPEN
    {
        public int ihPen;
        public LOGPEN lopn;
    }
    #endregion

    #region LOGPEN
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct LOGPEN
    {
        public uint lopnStyle;
        public POINT lopnWidth;
        public int lopnColor;
    }
    #endregion

    #region EMR_ANGLEARC
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_ANGLEARC
    {
        public POINT ptlCenter;
        public int nRadius;
        public float eStartAngle;
        public float eSweepAngle;
    }
    #endregion

    #region EMR_RECTANGLE
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_RECTANGLE
    {
        public RECT rclBox;
    }
    #endregion

    #region EMR_ROUNDRECT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_ROUNDRECT
    {
        public RECT rclBox;
        public SIZE szlCorner;
    }
    #endregion

    #region EMR_ARC
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_ARC
    {
        public RECT rclBox;
        public POINT ptlStart;
        public POINT ptlEnd;
    }
    #endregion

    #region EMR_FILLPATH
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_FILLPATH
    {
        public RECT rclBounds;
    }
    #endregion

    #region BITMAPINFOHEADER
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public DIB_COMPRESSION biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }
    #endregion

    #region BITMAPINFO
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        // public byte[] bmiColors;
        public IntPtr bmiColors;
    }
    #endregion

    #region BITMAP
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public int bmPlanes;
        public int bmBitsPixel;
        public byte[] bmBits;
    }
    #endregion

    #region EMR_STRETCHDIBITS
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_STRETCHDIBITS
    {
        public RECT rclBounds;
        public int xDest;
        public int yDest;
        public int xSrc;
        public int ySrc;
        public int cxSrc;
        public int cySrc;
        public int offBmiSrc;
        public int cbBmiSrc;
        public int offBitsSrc;
        public uint cbBitsSrc;
        public int iUsageSrc;
        public uint dwRop;
        public int cxDest;
        public int cyDest;
    }
    #endregion

    #region EMR_BITBLT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_BITBLT
    {
        public RECT rclBounds;
        public int xDest;
        public int yDest;
        public int cxDest;
        public int cyDest;
        public RASTER_CODE dwRop;
        public int xSrc;
        public int ySrc;
        public XFORM xformSrc;
        public int crBkColorSrc;
        public int iUsageSrc;
        public int offBmiSrc;
        public int cbBmiSrc;
        public int offBitsSrc;
        public uint cbBitsSrc;
    }
    #endregion

    #region EMR_STRETCHBLT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_STRETCHBLT
    {
        public RECT rclBounds;
        public int xDest;
        public int yDest;
        public int cxDest;
        public int cyDest;
        public RASTER_CODE dwRop;
        public int xSrc;
        public int ySrc;
        public XFORM xformSrc;
        public int crBkColorSrc;
        public int iUsageSrc;
        public int offBmiSrc;
        public int cbBmiSrc;
        public int offBitsSrc;
        public uint cbBitsSrc;
        public int cxSrc;
        public int cySrc;
    }
    #endregion

    #region EMR_MASKBLT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_MASKBLT
    {
        public RECT rclBounds;
        public int xDest;
        public int yDest;
        public int cxDest;
        public int cyDest;
        public RASTER_CODE dwRop;
        public int xSrc;
        public int ySrc;
        public XFORM xformSrc;
        public int crBkColorSrc;
        public int iUsageSrc;
        public int offBmiSrc;
        public int cbBmiSrc;
        public int offBitsSrc;
        public uint cbBitsSrc;
        public int xMask;
        public int yMask;
        public int iUsageMask;
        public int offBmiMask;
        public int cbBmiMask;
        public int offBitsMask;
        public uint cbBitsMask;
    }
    #endregion

    #region LOGFONT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class LOGFONT
    {
        public int lfHeight = 0;
        public int lfWidth = 0;
        public int lfEscapement = 0;
        public int lfOrientation = 0;
        public FW_FONT_WEIGHT lfWeight = FW_FONT_WEIGHT.FW_NORMAL;
        [MarshalAs(UnmanagedType.U1)]
        public bool lfItalic = false;
        [MarshalAs(UnmanagedType.U1)]
        public bool lfUnderline = false;
        [MarshalAs(UnmanagedType.U1)]
        public bool lfStrikeOut = false;
        public byte lfCharSet = 0;
        public byte lfOutPrecision = 0;
        public byte lfClipPrecision = 0;
        public byte lfQuality = 0;
        public byte lfPitchAndFamily = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WinGdiConst.LF_FACESIZE)]
        public string lfFaceName = null;
    }
    #endregion

    #region EMR_EXTCREATEFONTINDIRECTW
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_EXTCREATEFONTINDIRECTW
    {
        public int ihFonts;
        public LOGFONT elfw;
    }
    #endregion

    #region EMR_EXTTEXTOUTA
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_EXTTEXTOUTA
    {
        public RECT rclBounds;
        public int iGraphicsMode;
        public float exScale;
        public float eyScale;
        public EMR_TEXT emrtext;
    }
    #endregion

    #region EMR_TEXT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_TEXT
    {
        public POINT ptlReference;
        public int nChars;
        public int offString;
        public int fOptions;
        public RECT rcl;
        public int offDx;
    }
    #endregion

    #region EMR_CREATEDIBPATTERNBRUSHPT
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_CREATEDIBPATTERNBRUSHPT
    {
        public int ihBrush;
        public int iUsage;
        public int offBmi;
        public int cbBmi;
        public int offBits;
        public uint cbBits;
    }
    #endregion

    #region EMR_SETPIXELV
    /// <summary>
    /// Record of Emf metafile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMR_SETPIXELV
    {
        public POINT ptlPixel;
        public int crColor;
    }
    #endregion

    #region OUTLINETEXTMETRIC
    /// <summary>
    /// Structure for information about font.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
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

    #region WinGdi constants

    #region Simple constants
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
        public const int DIB_PAL_COLORS = 1;
        public const int LF_FULLFACESIZE = 64;
        public const int LF_FACESIZE = 32;
        public const int ELF_VENDOR_SIZE = 4;
        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;
        public const int CBM_INIT = 0x04;
        public const uint GDI_ERROR = 0xFFFFFFFF;
    }
    #endregion

    /// <summary>
    /// ExtTextOut options constants.
    /// </summary>
    [Flags]
    internal enum ETO
    {
        /// <summary>
        /// The current background color should be used to fill the rectangle.
        /// </summary>
        OPAQUE = 0x0002,
        /// <summary>
        /// The text will be clipped to the rectangle.
        /// </summary>
        CLIPPED = 0x0004,
        /// <summary>
        /// Windows 95 and Windows NT 4.0 and later:
        /// The lpString array refers to an array returned from 
        /// GetCharacterPlacement and should be parsed directly by GDI
        /// as no further language-specific processing is required.
        /// Glyph indexing only applies to TrueType fonts, but the flag
        /// can be used for bitmap and vector fonts to indicate that no further
        /// language processing is necessary and GDI should process the string
        /// directly.
        /// 
        /// Note that all glyph indexes are 16-bit values even though the string is
        /// assumed to be an array of 8-bit values for raster fonts. 
        /// 
        /// For ExtTextOutW, the glyph indexes are saved to a metafile.
        /// However, to display the correct characters the metafile must be
        /// played back using the same font. For ExtTextOutA, the glyph indexes are not saved.
        /// </summary>
        GLYPH_INDEX = 0x0010,
        /// <summary>
        /// Windows 95 and Windows NT 4.0 and later: To display numbers, use European digits.
        /// </summary>
        NUMERICSLATIN = 0x0800,
        /// <summary>
        /// Windows 95 and Windows NT 4.0 and later: To display numbers, use digits appropriate to the locale.
        /// </summary>
        NUMERICSLOCAL = 0x0400,
        /// <summary>
        /// Windows 95 and Windows NT 4.0 and later for Middle East
        /// language edition of Windows: If this value is specified
        /// and a Hebrew or Arabic font is selected into the device
        /// context, the string is output using right-to-left reading
        /// order. If this value is not specified, the string is output
        /// in left-to-right order. The same effect can be achieved
        /// by setting the TA_RTLREADING value in SetTextAlign. This
        /// value is preserved for backward compatibility. 
        /// </summary>
        RTLREADING = 0x0080,
        /// <summary>
        /// Windows NT 4.0 and later: Reserved for system use.
        /// If an application sets this flag, it loses international scripting
        /// support and in some cases it may display no text at all. 
        /// </summary>
        IGNORELANGUAGE = 0x1000,
        /// <summary>
        /// Windows 2000/XP: When this is set, the array pointed to by lpDx
        /// contains pairs of values. The first value of each pair is,
        /// as usual, the distance between origins of adjacent character cells,
        /// but the second value is the displacement along the vertical direction of the font.
        /// </summary>
        PDY = 0x2000,
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum MWT_DATA
    {
        MWT_IDENTITY = 1,
        MWT_LEFTMULTIPLY = 2,
        MWT_RIGHTMULTIPLY = 3
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum PS_PEN_STYLE
    {
        PS_SOLID = 0,
        PS_DASH = 1,
        PS_DOT = 2,
        PS_DASHDOT = 3,
        PS_DASHDOTDOT = 4,
        PS_NULL = 5,
        PS_INSIDEFRAME = 6,
        PS_USERSTYLE = 7,
        PS_ALTERNATE = 8,
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum PS_PEN_CAP_STYLE
    {
        PS_ENDCAP_ROUND = 0x00000000,
        PS_ENDCAP_SQUARE = 0x00000100,
        PS_ENDCAP_FLAT = 0x00000200
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum PS_PEN_JOIN_STYLE
    {
        PS_JOIN_ROUND = 0x00000000,
        PS_JOIN_BEVEL = 0x00001000,
        PS_JOIN_MITER = 0x00002000
    }
    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum PS_PEN_TYPE
    {
        PS_COSMETIC = 0x00000000,
        PS_GEOMETRIC = 0x00010000,
    }
    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum BS_BRUSH_STYLE
    {
        BS_SOLID = 0,
        BS_NULL = 1,
        BS_HOLLOW = BS_NULL,
        BS_HATCHED = 2,
        BS_PATTERN = 3,
        BS_INDEXED = 4,
        BS_DIBPATTERN = 5,
        BS_DIBPATTERNPT = 6,
        BS_PATTERN8X8 = 7,
        BS_DIBPATTERN8X8 = 8,
        BS_MONOPATTERN = 9
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum GM_GraphicsMode
    {
        GM_COMPATIBLE = 1,
        GM_ADVANCED = 2
    }
    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum MAPPING_MODE
    {
        MM_TEXT = 1,
        MM_LOMETRIC = 2,
        MM_HIMETRIC = 3,
        MM_LOENGLISH = 4,
        MM_HIENGLISH = 5,
        MM_TWIPS = 6,
        MM_ISOTROPIC = 7,
        MM_ANISOTROPIC = 8
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    [Flags]
    internal enum PT_POINT_TYPE : byte
    {
        PT_CLOSEFIGURE = 0x01,
        PT_LINETO = 0x02,
        PT_BEZIERTO = 0x04,
        PT_MOVETO = 0x06
    }
    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum AD_ANGLEDIRECTION
    {
        AD_COUNTERCLOCKWISE = 1,
        AD_CLOCKWISE = 2
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    [Flags]
    internal enum TA_TEXT_ALIGN
    {
        TA_NOUPDATECP = 0,
        TA_UPDATECP = 1,

        TA_LEFT = 0,
        TA_RIGHT = 2,
        TA_CENTER = 6,

        TA_TOP = 0,
        TA_BOTTOM = 8,
        TA_BASELINE = 24,

        TA_RTLREADING = 256
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    [Flags]
    internal enum RASTER_CODE : uint
    {
        SRCCOPY = (uint)0x00CC0020,
        SRCPAINT = (uint)0x00EE0086,
        SRCAND = (uint)0x008800C6,
        SRCINVERT = (uint)0x00660046,
        SRCERASE = (uint)0x00440328,
        NOTSRCCOPY = (uint)0x00330008,
        NOTSRCERASE = (uint)0x001100A6,
        MERGECOPY = (uint)0x00C000CA,
        MERGEPAINT = (uint)0x00BB0226,
        PATCOPY = (uint)0x00F00021,
        PATPAINT = (uint)0x00FB0A09,
        PATINVERT = (uint)0x005A0049,
        DSTINVERT = (uint)0x00550009,
        BLACKNESS = (uint)0x00000042,
        WHITENESS = (uint)0x00FF0062
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
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum STOCK : int
    {
        WHITE_BRUSH = 0,
        LTGRAY_BRUSH = 1,
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = NULL_BRUSH,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,

        DEFAULT_GUI_FONT = 17,
        DC_BRUSH = 18,
        DC_PEN = 19
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum COMBINE_RGN
    {
        RGN_AND = 1,
        RGN_OR = 2,
        RGN_XOR = 3,
        RGN_DIFF = 4,
        RGN_COPY = 5,
        RGN_MIN = RGN_AND,
        RGN_MAX = RGN_COPY
    }

    /// <summary>
    /// Constants from WinGdi.h file.
    /// </summary>
    internal enum DIB_COMPRESSION
    {
        BI_RGB = 0,
        BI_RLE8 = 1,
        BI_RLE4 = 2,
        BI_BITFIELDS = 3,
        BI_JPEG = 4,
        BI_PNG = 5
    }
    #endregion

    #region CryptoApi structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct SYSTEMTIME
    {
        public UInt16 wYear;
        public UInt16 wMonth;
        public UInt16 wDayOfWeek;
        public UInt16 wDay;
        public UInt16 wHour;
        public UInt16 wMinute;
        public UInt16 wSecond;
        public UInt16 wMilliseconds;
    }
    #endregion
}