#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents LogFont structure that defines the attributes of a font. 
  /// </summary>
#if !(WINRT )
  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
#endif
  public class LOGFONT
  {
    #region Class members
    /// <summary>
    /// Specifies the height, in logical units, of the font's character cell or character.
    ///  The character height value (also known as the em height) is the character cell
    ///  height value minus the internal-leading value.
    /// </summary>
    public int lfHeight;
    /// <summary>
    /// Specifies the average width, in logical units, of characters in the font.
    /// If lfWidth is zero, the aspect ratio of the device is matched against the
    /// digitalization aspect ratio of the available fonts to find the closest match,
    /// determined by the absolute value of the difference.
    /// </summary>
    public int lfWidth;
    /// <summary>
    /// Specifies the angle, in tenths of degrees, between the escapement vector
    /// and the x-axis of the device. The escapement vector is parallel
    /// to the base line of a row of text. 
    /// </summary>
    public int lfEscapement;
    /// <summary>
    /// Specifies the angle, in tenths of degrees, between each character's base
    /// line and the x-axis of the device.
    /// </summary>
    public int lfOrientation;
    /// <summary>
    /// Specifies the weight of the font in the range 0 through 1000.
    /// For example, 400 is normal and 700 is bold. If this value is zero,
    /// a default weight is used.
    /// </summary>
    public int lfWeight;
    /// <summary>
    /// Specifies an italic font if set to TRUE.
    /// </summary>
    public byte lfItalic;
    /// <summary>
    /// Specifies an underlined font if set to TRUE.
    /// </summary>
    public byte lfUnderline;
    /// <summary>
    /// Specifies a strikeout font if set to TRUE.
    /// </summary>
    public byte lfStrikeOut;
    /// <summary>
    /// Specifies the character set. The following values are predefined.
    /// </summary>
    public byte lfCharSet;
    /// <summary>
    /// Specifies the output precision. The output precision defines how closely
    /// the output must match the requested font's height, width, character
    /// orientation, escapement, pitch, and font type.
    /// </summary>
    public byte lfOutPrecision;
    /// <summary>
    /// Specifies the clipping precision. The clipping precision defines how
    /// to clip characters that are partially outside the clipping region.
    /// </summary>
    public byte lfClipPrecision;
    /// <summary>
    /// Specifies the output quality. The output quality defines how carefully
    /// the graphics device interface (GDI) must attempt to match the logical-font
    /// attributes to those of an actual physical font.
    /// </summary>
    public byte lfQuality;
    /// <summary>
    /// Specifies the pitch and family of the font. The two low-order bits
    /// specify the pitch of the font. Font families describe the look of
    /// a font in a general way. They are intended for specifying fonts when
    /// the exact typeface desired is not available.
    /// </summary>
    public byte lfPitchAndFamily;
    /// <summary>
    /// A null-terminated string that specifies the typeface name of the font.
    /// The length of this string must not exceed 32 characters, including
    /// the terminating null character. The EnumFontFamiliesEx function can
    /// be used to enumerate the typeface names of all currently available fonts.
    /// If lfFaceName is an empty string, GDI uses the first font that matches
    /// the other specified attributes.
    /// </summary>
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
    public byte[] lfFaceName = new byte[ 32 ];
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public LOGFONT()
    {
    }
    #endregion
  }

  /// <summary>
  /// Contains information about an enumerated font.
  /// </summary>
#if !(WINRT )
  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
#endif
  public class ENUMLOGFONTEX
  {
    #region Class members
    /// <summary>
    /// Specifies a LOGFONT structure that contains values defining the font attributes.
    /// </summary>
    public LOGFONT m_logFont;
    /// <summary>
    /// Specifies a null-terminated string specifying the unique name of the font.
    /// For example, ABC Font Company TrueType Bold Italic Sans Serif.
    /// </summary>
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 64 )]
    private byte[] m_arrFullName;
    /// <summary>
    /// Specifies a null-terminated string specifying the style of the font.
    /// For example, Bold Italic.
    /// </summary>
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
    private byte[] m_arrStyle;
    /// <summary>
    /// Specifies a null-terminated string specifying the script, that is,
    /// the character set, of the font. For example, Cyrillic. 
    /// </summary>
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
    public byte[] m_arrScript;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns font full name without ending zero character. Read-only.
    /// </summary>
    public string FullName
    {
      get
      {
        return GetZeroTerminatedString( m_arrFullName );
      }
    }

    /// <summary>
    /// Returns style name without ending zero character. Read-only.
    /// </summary>
    public string Style
    {
      get
      {
        return GetZeroTerminatedString( m_arrStyle );
      }
    }
    /// <summary>
    /// Returns a LOGFONT structure that contains values defining the font attributes. Read-only.
    /// </summary>
    public LOGFONT LogFont
    {
      get
      {
        return m_logFont;
      }
    }

#if DEBUG
    /// <summary>
    /// Gets / sets array with full name.
    /// </summary>
    public byte[] FullNameArray
    {
      get
      {
        return m_arrFullName;
      }
      set
      {
        m_arrFullName = value;
      }
    }
    /// <summary>
    /// Gets / sets array with full style.
    /// </summary>
    public byte[] StyleArray
    {
      get
      {
        return m_arrStyle;
      }
      set
      {
        m_arrStyle = value;
      }
    }
#endif
    #endregion

    #region Class helper methods
    /// <summary>
    /// Converts byte array with zero terminated string data into string without ending zero character.
    /// </summary>
    /// <param name="arrData">String data.</param>
    /// <returns>Converted string.</returns>
    private string GetZeroTerminatedString( byte[] arrData )
    {
      if( arrData != null )
      {
        int i = 0;

        for( int len = arrData.Length; i < len; i++ )
        {
          if( arrData[ i ] == '\0' )
            break;
        }

        Encoding encoding =
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.Default;
#else
        Encoding.UTF8;
#endif

        return encoding.GetString( arrData, 0, i );
      }
      else
      {
        return null;
      }
    }
    #endregion
  }
  ///<exclude/>
  /// <summary>
  /// The EnumFontFamExProc function is an application definedcallback function
  /// used with the EnumFontFamiliesEx function. It is used to process the fonts.
  /// It is called once for each enumerated font. The FONTENUMPROC type defines
  /// a pointer to this callback function. EnumFontFamExProc is a placeholder
  /// for the application definedfunction name. 
  /// </summary>
  public delegate int EnumFontFamExProc(
    ENUMLOGFONTEX lpelf,    // logical-font data
    IntPtr lpntm,  // physical-font data
    int FontType,        // type of font
    ref object objData     // application-defined data
  );
  /// <summary>
  /// Contains some native32 functions used by XlsIO.
  /// </summary>
  public sealed class API
  {
#if !(WINRT )
    /// <summary>
    /// The EnumFontFamiliesEx function enumerates all fonts in the system that
    /// match the font characteristics specified by the LOGFONT structure.
    /// EnumFontFamiliesEx enumerates fonts based on typeface name, character set, or both. 
    /// </summary>
    /// <param name="hdc">Handle to the device context.</param>
    /// <param name="lpLogfont">
    /// Pointer to a LOGFONT structure that contains information about the fonts
    /// to enumerate. The function examines the following members.
    /// </param>
    /// <param name="lpEnumFontFamExProc">
    /// Pointer to the application definedcallback function.
    /// </param>
    /// <param name="objData"></param>
    /// <param name="dwFlags"></param>
    /// <returns></returns>
    [DllImport( "gdi32.dll" )]
    public static extern int EnumFontFamiliesEx(
      IntPtr hdc,                          // handle to DC
      LOGFONT lpLogfont,              // font information
      EnumFontFamExProc lpEnumFontFamExProc, // callback function
      ref object objData,                    // additional data
      int dwFlags                     // not used; must be 0
      );
#endif
  }
  /// <summary>
  /// This class contains native heap functions.
  /// </summary>
  public sealed class Heap
  {
    #region Heap functions
    /// <summary>
    /// This function allocates a block of memory from a heap. The allocated memory is not movable.
    /// </summary>
    /// <param name="hHeap">Handle to the heap from which the memory will be allocated.</param>
    /// <param name="dwFlags">Heap allocation options.</param>
    /// <param name="dwBytes">Number of bytes to be allocated. If the heap specified by
    /// the hHeap parameter is a nongrowable heap, dwBytes must be less than 0x7FFF8.
    /// You create a nongrowable heap by calling the HeapCreate function with a nonzero value.</param>
    /// <returns></returns>
    [DllImport( "kernel32" )]
    public static extern IntPtr HeapAlloc( IntPtr hHeap, int dwFlags, int dwBytes );
#if !(WINRT )
    /// <summary>
    /// Creates a heap object that can be used by the calling process. The function reserves
    /// space in the virtual address space of the process and allocates physical storage for
    /// a specified initial portion of this block.
    /// </summary>
    /// <param name="flOptions">The heap allocation options.</param>
    /// <param name="dwInitialSize">The initial size of the heap, in bytes. This value
    /// determines the initial amount of memory that is committed for the heap.
    /// The value is rounded up to the next page boundary. The value must be smaller
    /// than dwMaximumSize. If this parameter is 0, the function commits one page.</param>
    /// <param name="dwMaximumSize">The maximum size of the heap, in bytes.</param>
    /// <returns></returns>
    [DllImport( "kernel32" )]
    public static extern IntPtr HeapCreate( int flOptions, int dwInitialSize, int dwMaximumSize );
    /// <summary>
    /// Destroys the specified heap object. It decommits and releases all the
    /// pages of a private heap object, and it invalidates the handle to the heap.
    /// </summary>
    /// <param name="hHeap">A handle to the heap to be destroyed.</param>
    /// <returns>If the function succeeds, the return value is nonzero.</returns>
    [DllImport( "kernel32" )]
    public static extern int HeapDestroy( IntPtr hHeap );
#endif
    /// <summary>
    /// Frees a memory block allocated from a heap by the HeapAlloc or HeapReAlloc function.
    /// </summary>
    /// <param name="hHeap">A handle to the heap whose memory block is to be freed.</param>
    /// <param name="dwFlags">The heap free options.</param>
    /// <param name="lpMem">A pointer to the memory block to be freed.</param>
    /// <returns>If the function succeeds, the return value is nonzero.</returns>
    [DllImport( "kernel32" )]
    public static extern int HeapFree( IntPtr hHeap, int dwFlags, IntPtr lpMem );
    /// <summary>
    /// Reallocates a block of memory from a heap. This function enables you to
    /// resize a memory block and change other memory block properties.
    /// The allocated memory is not movable. 
    /// </summary>
    /// <param name="hHeap">A handle to the heap from which the memory is to be reallocated.</param>
    /// <param name="dwFlags">The heap reallocation options.</param>
    /// <param name="lpMem">A pointer to the block of memory that the function reallocates.</param>
    /// <param name="dwBytes">The new size of the memory block, in bytes. A memory
    /// block's size can be increased or decreased by using this function.</param>
    /// <returns>If the function succeeds, the return value is a pointer to
    /// the reallocated memory block.</returns>
    [DllImport( "kernel32" )]
    public static extern IntPtr HeapReAlloc( IntPtr hHeap, int dwFlags, IntPtr lpMem, int dwBytes );
    #endregion
  }
  /// <summary>
  /// This class contains some Win32 API functions for memory operations.
  /// </summary>
  public sealed class Memory
  {
#if !(WINRT )
    /// <summary>
    /// The MoveMemory function moves a block of memory from one location to another.
    /// </summary>
    /// <param name="ptrDest">Pointer to the starting address of the move destination.</param>
    /// <param name="ptrSource">Pointer to the starting address of the block of memory to be moved.</param>
    /// <param name="iSize">Size of the block of memory to move, in bytes.</param>
    [DllImport( "kernel32.dll" )]
    public static extern void RtlMoveMemory( IntPtr ptrDest, IntPtr ptrSource, int iSize );
    /// <summary>
    /// The RtlZeroMemory routine fills a block of memory with zeros, given a pointer
    /// to the block and the length, in bytes, to be filled.
    /// </summary>
    /// <param name="ptrDest">Pointer to the memory to be filled with zeros.</param>
    /// <param name="iSize">Specifies the number of bytes to be zeroed.</param>
    [DllImport( "kernel32.dll" )]
    public static extern void RtlZeroMemory( IntPtr ptrDest, int iSize );
    /// <summary>
    /// The CopyMemory function copies a block of memory from one location to another.
    /// </summary>
    /// <param name="ptrDest">Pointer to the starting address of the copied block's destination.</param>
    /// <param name="ptrSource">Pointer to the starting address of the block of memory to copy.</param>
    /// <param name="iSize">Size of the block of memory to copy, in bytes.</param>
    [DllImport( "kernel32.dll" )]
    public static extern void CopyMemory( IntPtr ptrDest, IntPtr ptrSource, int iSize );
#endif
#if AllowUnsafeCode
    /// <summary>
    /// The CopyMemory function copies a block of memory from one location to another.
    /// </summary>
    /// <param name="ptrDest">Pointer to the starting address of the copied block's destination.</param>
    /// <param name="ptrSource">Pointer to the starting address of the block of memory to copy.</param>
    /// <param name="iSize">Size of the block of memory to copy, in bytes.</param>
    [DllImport( "kernel32.dll" )]
    [ CLSCompliant( false ) ]
    unsafe public static extern void CopyMemory( byte* ptrDest, byte* ptrSource, int iSize );
#endif
  }
}
