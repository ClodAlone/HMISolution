#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Native
{
    /// <summary>
    /// Specifies the type of character information the user wants to retrieve.
    /// </summary>
    internal enum StringInfoType : uint
    {
        /// <summary>
        /// Retrieves character type info
        /// </summary>
        CT_TYPE1 = 1,
        /// <summary>
        /// Retrieves bi-directional layout info
        /// </summary>
        CT_TYPE2 = 2,
        /// <summary>
        /// Retrieves text processing info
        /// </summary>
        CT_TYPE3 = 4
    }

    /// <summary>
    /// These types support ANSI C and POSIX (LC_CTYPE) character-typing functions.
    /// A combination of these values is returned in the array pointed to by the lpCharType parameter
    /// when the dwInfoType parameter is set to CT_CTYPE1.
    /// </summary>
    internal enum StringInfoCtype1 : ushort
    {
        /// <summary>
        /// Uppercase
        /// </summary>
        C1_UPPER = 0x0001,
        /// <summary>
        /// Lowercase
        /// </summary>
        C1_LOWER = 0x0002,
        /// <summary>
        /// Decimal digits
        /// </summary>
        C1_DIGIT = 0x0004,
        /// <summary>
        /// Space characters
        /// </summary>
        C1_SPACE = 0x0008,
        /// <summary>
        /// Punctuation
        /// </summary>
        C1_PUNCT = 0x0010,
        /// <summary>
        /// Control characters
        /// </summary>
        C1_CNTRL = 0x0020,
        /// <summary>
        /// Blank characters
        /// </summary>
        C1_BLANK = 0x0040,
        /// <summary>
        /// Hexadecimal digits
        /// </summary>
        C1_XDIGIT = 0x0080,
        /// <summary>
        /// Any linguistic character: alphabetic, syllabary, or ideographic
        /// </summary>
        C1_ALPHA = 0x0100,
    }

    /// <summary>
    /// These types support proper layout of Unicode text. The direction attributes are assigned
    /// so that the bidirectional layout algorithm standardized by Unicode produces accurate results.
    /// These types are mutually exclusive.
    /// </summary>
    internal enum StringInfoCtype2 : ushort
    {
        /// <summary>
        /// Left to right
        /// </summary>
        C2_LEFTTORIGHT = 0x0001,
        /// <summary>
        /// Right to left
        /// </summary>
        C2_RIGHTTOLEFT = 0x0002,
        /// <summary>
        /// European number, European digit
        /// </summary>
        C2_EUROPENUMBER = 0x0003,
        /// <summary>
        /// European numeric separator
        /// </summary>
        C2_EUROPESEPARATOR = 0x0004,
        /// <summary>
        /// European numeric terminator
        /// </summary>
        C2_EUROPETERMINATOR = 0x0005,
        /// <summary>
        /// Arabic number
        /// </summary>
        C2_ARABICNUMBER = 0x0006,
        /// <summary>
        /// Common numeric separator
        /// </summary>
        C2_COMMONSEPARATOR = 0x0007,
        /// <summary>
        /// Block separator
        /// </summary>
        C2_BLOCKSEPARATOR = 0x0008,
        /// <summary>
        /// Segment separator
        /// </summary>
        C2_SEGMENTSEPARATOR = 0x0009,
        /// <summary>
        /// White space
        /// </summary>
        C2_WHITESPACE = 0x000A,
        /// <summary>
        /// Other neutrals
        /// </summary>
        C2_OTHERNEUTRAL = 0x000B,
        /// <summary>
        /// No implicit directionality (for example, control codes)
        /// </summary>
        C2_NOTAPPLICABLE = 0x0000
    }

    /// <summary>
    /// These types are intended to be placeholders for extensions to the POSIX types
    /// required for general text processing or for the standard C library functions.
    ///  A combination of these values is returned when dwInfoType is set to CT_CTYPE3.
    /// </summary>
    internal enum StringInfoCtype3 : ushort
    {
        /// <summary>
        /// Diacritic nonspacing mark
        /// </summary>
        C3_DIACRITIC = 0x0002,
        /// <summary>
        /// Vowel nonspacing mark
        /// </summary>
        C3_VOWELMARK = 0x0004,
        /// <summary>
        /// Symbol
        /// </summary>
        C3_SYMBOL = 0x0008,
        /// <summary>
        /// Katakana character
        /// </summary>
        C3_KATAKANA = 0x0010,
        /// <summary>
        /// Hiragana character
        /// </summary>
        C3_HIRAGANA = 0x0020,
        /// <summary>
        /// Half-width (narrow) character
        /// </summary>
        C3_HALFWIDTH = 0x0040,
        /// <summary>
        /// Full-width (wide) character
        /// </summary>
        C3_FULLWIDTH = 0x0080,
        /// <summary>
        /// Ideographic character
        /// </summary>
        C3_IDEOGRAPH = 0x0100,
        /// <summary>
        /// Arabic Kashida character
        /// </summary>
        C3_KASHIDA = 0x0200,
        /// <summary>
        /// Punctuation which is counted as part of the word
        /// (Kashida, hyphen, feminine/masculine ordinal indicators, equal sign, and so forth)
        /// </summary>
        C3_LEXICAL = 0x0400,
        /// <summary>
        /// All linguistic characters (alphabetical, syllabary, and ideographic)
        /// </summary>
        C3_ALPHA = 0x8000,
        /// <summary>
        /// Not applicable
        /// </summary>
        C3_NOTAPPLICABLE = 0x0000
    }

    /// <summary>
    /// Native enum.
    /// </summary>
    [Flags]
    internal enum FormatMessageFlags
    {
        AllocateBuffer = 0x00000100,
        IgnoreInserts = 0x00000200,
        FromString = 0x00000400,
        FromHmodule = 0x00000800,
        FromSystem = 0x00001000,
        ArgumentArray = 0x00002000
    }
}
