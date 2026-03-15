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

#if !DOCIO && SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#endif

#if !DOCIO && WP
using Syncfusion.XlsIO.Implementation.WP;
#endif

#if DOCIO
#if (SILVERLIGHT) && !(WINRT ) 
using Syncfusion.DocIO.Implementation.Silverlight;
#elif WP
using Syncfusion.DocIO.Implementation.WP;
#endif
#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else



namespace Syncfusion.CompoundFile.XlsIO
#endif
{
  /// <summary>
  /// Property IDs for the SummaryInformation Property Set.
  /// </summary>
  public enum BuiltInProperty
  {
    /// <summary>
    /// Title document property Id.
    /// </summary>
    Title = 0x00000002,
    /// <summary>
    /// Subject document property Id.
    /// </summary>
    Subject,
    /// <summary>
    /// Author document property Id.
    /// </summary>
    Author,
    /// <summary>
    /// Keywords document property Id.
    /// </summary>
    Keywords,
    /// <summary>
    /// Comments document property Id.
    /// </summary>
    Comments,
    /// <summary>
    /// Template document property Id.
    /// </summary>
    Template,
    /// <summary>
    /// LastAuthor document property Id.
    /// </summary>
    LastAuthor,
    /// <summary>
    /// Revnumber document property Id.
    /// </summary>
    RevisionNumber,
    /// <summary>
    /// EditTime document property Id.
    /// </summary>
    EditTime,
    /// <summary>
    /// LastPrinted document property Id.
    /// </summary>
    LastPrinted,
    /// <summary>
    /// CreationDate document property Id.
    /// </summary>
    CreationDate,
    /// <summary>
    /// LastSaveDate document property Id.
    /// </summary>
    LastSaveDate,
    /// <summary>
    /// PageCount document property Id.
    /// </summary>
    PageCount,
    /// <summary>
    /// WordCount document property Id.
    /// </summary>
    WordCount,
    /// <summary>
    /// CharCount document property Id.
    /// </summary>
    CharCount,
    /// <summary>
    /// Thumbnail document property Id.
    /// </summary>
    Thumbnail,
    /// <summary>
    /// ApplicationName document property Id.
    /// </summary>
    ApplicationName,
    /// <summary>
    /// Ssecurity document property Id.
    /// </summary>
    Security,

    /// <summary>
    /// Category Id.
    /// </summary>
    Category = 1000,
    /// <summary>
    /// Target format for presentation (35mm, printer, video, and so on) id.
    /// </summary>
    PresentationTarget,
    /// <summary>
    /// ByteCount Id.
    /// </summary>
    ByteCount,
    /// <summary>
    /// LineCount Id.
    /// </summary>
    LineCount,
    /// <summary>
    /// ParCount Id.
    /// </summary>
    ParagraphCount,
    /// <summary>
    /// SlideCount Id.
    /// </summary>
    SlideCount,
    /// <summary>
    /// NoteCount Id.
    /// </summary>
    NoteCount,
    /// <summary>
    /// HiddenCount Id.
    /// </summary>
    HiddenCount,
    /// <summary>
    /// MmclipCount Id.
    /// </summary>
    MultimediaClipCount,
    /// <summary>
    /// ScaleCrop property Id.
    /// </summary>
    ScaleCrop,
    /// <summary>
    /// HeadingPair Id.
    /// </summary>
    HeadingPair,
    /// <summary>
    /// DocParts Id.
    /// </summary>
    DocParts,
    /// <summary>
    /// Manager Id.
    /// </summary>
    Manager,
    /// <summary>
    /// Company Id.
    /// </summary>
    Company,
    /// <summary>
    /// LinksDirty Id.
    /// </summary>
    LinksDirty,
  }
  /// <summary>
  /// Enumeration with all supported property types.
  /// </summary>
  [Flags]
  public enum PropertyType
  {
    /// <summary>
    /// Indicates a Boolean value.
    /// </summary>
    Bool = VarEnum.VT_BOOL,
    /// <summary>
    /// Indicates an integer value.
    /// </summary>
    Int = VarEnum.VT_INT,
    /// <summary>
    /// Indicates a 4-bytes signed integer value.
    /// </summary>
    Int32 = VarEnum.VT_I4,
    /// <summary>
    /// Indicates a 2-bytes signed interger value.
    /// </summary>
    Int16 = VarEnum.VT_I2,
    /// <summary>
    /// Indicates a 4-bytes unsigned interger value.
    /// </summary>
    UInt32 = VarEnum.VT_UI4,
    /// <summary>
    /// Indicates a wide string terminated by a null.
    /// </summary>
    String = VarEnum.VT_LPWSTR,
    /// <summary>
    /// Indicates a string terminated by a null.
    /// </summary>
    AsciiString = VarEnum.VT_LPSTR,
    /// <summary>
    /// Indicates a FILETIME value.
    /// </summary>
    DateTime = VarEnum.VT_FILETIME,
    /// <summary>
    /// Indicates length prefixed bytes.
    /// </summary>
    Blob = VarEnum.VT_BLOB,
    /// <summary>
    /// Indicates a simple, counted array.
    /// </summary>
    Vector = VarEnum.VT_VECTOR,
    /// <summary>
    /// Indicates an object.
    /// </summary>
    Object = VarEnum.VT_VARIANT,
    /// <summary>
    /// Indicates a double value.
    /// </summary>
    Double = VarEnum.VT_R8,
    /// <summary>
    /// Indicates an empty value.
    /// </summary>
    Empty = VarEnum.VT_EMPTY,
    /// <summary>
    /// Indicates null value.
    /// </summary>
    Null = VarEnum.VT_NULL,
    /// <summary>
    /// Indicates clipboard data.
    /// </summary>
    ClipboardData = VarEnum.VT_CF,

    /// <summary>
    /// Indicates an array of strings.
    /// </summary>
    AsciiStringArray = AsciiString | Vector,
    /// <summary>
    /// Indicates an array of strings.
    /// </summary>
    StringArray = String | Vector,
    /// <summary>
    /// Indicates an array of objects. Supported types are string and integer values.
    /// </summary>
    ObjectArray = Object | Vector,
  }
}
