#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Specifies style information applied to text.
    /// </summary>
    [Flags]
    public enum PdfFontStyle
    {
        /// <summary>
        /// Normal text.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold text.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Represents the underline text.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Strikeout text.
        /// </summary>
        Strikeout = 8
    }

    /// <summary>
    /// Indicates type of standard PDF fonts.
    /// </summary>
    public enum PdfFontFamily
    {
        /// <summary>
        /// Represents the Helvetica font.
        /// </summary>
        Helvetica,

        /// <summary>
        /// Represents the Courier font.
        /// </summary>
        Courier,

        /// <summary>
        /// Represents the Times Roman font.
        /// </summary>
        TimesRoman,

        /// <summary>
        /// Represents the Symbol font.
        /// </summary>
        Symbol,

        /// <summary>
        /// Represents the ZapfDingbats font.
        /// </summary>
        ZapfDingbats,
    }

    /// <summary>
    /// Specifies the type of CJK font.
    /// </summary>
    public enum PdfCjkFontFamily
    {
        /// <summary>
        /// Represents the Hanyang Systems Gothic Medium font.
        /// </summary>
        HanyangSystemsGothicMedium = 2,

        /// <summary>
        /// Represents the Hanyang Systems shin myeong Jo Medium font.
        /// </summary>
        HanyangSystemsShinMyeongJoMedium = 3,

        /// <summary>
        /// Represents the Heisei kaku GothicW5 font.
        /// </summary>
        HeiseiKakuGothicW5 = 0,

        /// <summary>
        /// Represents the Heisei MinchoW3 font.
        /// </summary>
        HeiseiMinchoW3 = 1,

        /// <summary>
        /// Represents the Monotype Hei Medium font.
        /// </summary>
        MonotypeHeiMedium = 4,

        /// <summary>
        /// Represents the monotype sung Light font.
        /// </summary>
        MonotypeSungLight = 5,

        /// <summary>
        /// Represents the sinotype song light font.
        /// </summary>
        SinoTypeSongLight = 6
    }

    /// <summary>
    /// Specifies the type of the font.
    /// </summary>
    public enum PdfFontType
    {
        /// <summary>
        /// Indicates the standard Adobe fonts.
        /// </summary>
        Standard,
        /// <summary>
        /// Indicates the non-embedded TrueType fonts.
        /// </summary>
        TrueType,
        /// <summary>
        /// Indicates the Embedded TrueType fonts.
        /// </summary>
        TrueTypeEmbedded
    }

    /// <summary>
    /// Specifies the types of text wrapping.
    /// </summary>
    public enum PdfWordWrapType
    {
        /// <summary>
        /// Text wrapping between lines when formatting within a rectangle is disabled.
        /// </summary>
        None,

        /// <summary>
        /// Text is wrapped by words. If there is a word that is longer than bounds' width, this word is wrapped by characters.
        /// </summary>
        Word,

        /// <summary>
        /// Text is wrapped by words. If there is a word that is longer than bounds' width, it won't be wrapped at all
        /// and the process will be finished.
        /// </summary>
        WordOnly,

        /// <summary>
        /// Text is wrapped by characters. In this case the word at the end of the text line can be split.
        /// </summary>
        Character
    }

    /// <summary>
    /// Specifies type of the SubSuperScript.
    /// </summary>
    public enum PdfSubSuperScript
    {
        /// <summary>
        /// Specifies no subscript or superscript.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies superscript format.
        /// </summary>
        SuperScript = 1,

        /// <summary>
        /// Specifies subscript format.
        /// </summary>
        SubScript = 2
    }

    /// <summary>
    /// Ttf platform ID.
    /// </summary>
    internal enum TtfPlatformID
    {
        /// <summary>
        /// Apple platform.
        /// </summary>
        AppleUnicode,

        /// <summary>
        /// Macintosh platform.
        /// </summary>
        Macintosh,

        /// <summary>
        /// Iso platform.
        /// </summary>
        Iso,

        /// <summary>
        /// Microsoft platform.
        /// </summary>
        Microsoft
    }

    /// <summary>
    /// Ttf Name ID.
    /// </summary>
    internal enum TtfNameID
    {
        /// <summary>
        /// The Copyright
        /// </summary>
        Copyright,

        /// <summary>
        /// The Font Family
        /// </summary>
        FontFamily,

        /// <summary>
        /// The Font Sub Family
        /// </summary>
        FontSubFamily,

        /// <summary>
        /// The Font Identifier
        /// </summary>
        FontIdentifier,

        /// <summary>
        /// The Font Name
        /// </summary>
        FontName,

        /// <summary>
        /// The Version
        /// </summary>
        Version,

        /// <summary>
        /// The PostScriptName
        /// </summary>
        PostScriptName,

        /// <summary>
        /// The Trademark
        /// </summary>
        Trademark
    }

    /// <summary>
    /// Enumerator that implements CMAP encodings.
    /// </summary>
    internal enum TtfCmapEncoding
    {
        /// <summary>
        /// Unknown encoding.
        /// </summary>
        Unknown,

        /// <summary>
        /// When building a symbol font for Windows.
        /// </summary>
        Symbol,

        /// <summary>
        /// When building a Unicode font for Windows.
        /// </summary>
        Unicode,

        /// <summary>
        /// For font that will be used on a Macintosh.
        /// </summary>
        Macintosh,
    }

    /// <summary>
    /// Microsoft encoding ID
    /// </summary>
    internal enum TtfMicrosoftEncodingID
    {
        /// <summary>
        /// Undefined encoding.
        /// </summary>
        Undefined,

        /// <summary>
        /// Unicode encoding.
        /// </summary>
        Unicode
    }

    /// <summary>
    /// Macintosh encoding ID.
    /// </summary>
    internal enum TtfMacintoshEncodingID
    {
        // Note: it's not all encoding, just firdt three.

        /// <summary>
        /// Roman encoding.
        /// </summary>
        Roman,

        /// <summary>
        /// Japanese encoding.
        /// </summary>
        Japanese,

        /// <summary>
        /// Chinese encoding.
        /// </summary>
        Chinese
    }

    /// <summary>
    /// Enumerator that implements CMAP formats.
    /// </summary>
    internal enum TtfCmapFormat
    {
        /// <summary>
        /// This is the Apple standard character to glyph index mapping table.
        /// </summary>
        Apple = 0,

        /// <summary>
        /// This is the Microsoft standard character to glyph index mapping table.
        /// </summary>
        Microsoft = 4,

        /// <summary>
        /// Format 6: Trimmed table mapping.
        /// </summary>
        Trimmed = 6,
    }

    /// <summary>
    /// ttf composite glyph flags.
    /// </summary>
    [Flags]
    internal enum TtfCompositeGlyphFlags : ushort
    {
        /// <summary>
        /// The ARG_1_AND_2_ARE_WORDS.
        /// </summary>
        ARG_1_AND_2_ARE_WORDS = 0x0001,

        /// <summary>
        /// The ARGS_ARE_XY_VALUES.
        /// </summary>
        ARGS_ARE_XY_VALUES = 0x0002,

        /// <summary>
        /// The ROUND_XY_TO_GRID.
        /// </summary>
        ROUND_XY_TO_GRID = 0x0004,

        /// <summary>
        /// The WE_HAVE_A_SCALE.
        /// </summary>
        WE_HAVE_A_SCALE = 0x0008,

        /// <summary>
        /// The RESERVED.
        /// </summary>
        RESERVED = 0x0010,

        /// <summary>
        /// The MORE_COMPONENTS.
        /// </summary>
        MORE_COMPONENTS = 0x0020,

        /// <summary>
        /// The WE_HAVE_AN_X_AND_Y_SCALE.
        /// </summary>
        WE_HAVE_AN_X_AND_Y_SCALE = 0x0040,

        /// <summary>
        /// The WE_HAVE_A_TWO_BY_TWO.
        /// </summary>
        WE_HAVE_A_TWO_BY_TWO = 0x0080,

        /// <summary>
        /// The WE_HAVE_INSTRUCTIONS.
        /// </summary>
        WE_HAVE_INSTRUCTIONS = 0x0100,

        /// <summary>
        /// The USE_MY_METRICS.
        /// </summary>
        USE_MY_METRICS = 0x0200,
    }

    /// <summary>
    /// Character set encoding type of the font.
    /// </summary>
    internal enum FontEncoding
    {
        /// <summary>
        /// Unknown encoding
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Adobe standard Latin-text encoding
        /// </summary>
        StandardEncoding,

        /// <summary>
        /// Mac OS standard encoding
        /// </summary>
        MacRomanEncoding,

        /// <summary>
        /// An encoding for use with expert fonts
        /// </summary>
        MacExpertEncoding,

        /// <summary>
        /// Windows Code Page 1252
        /// </summary>
        WinAnsiEncoding,

        /// <summary>
        /// Encoding for text strings in a PDF document outside the document's content streams.
        /// </summary>
        PDFDocEncoding,

        /// <summary>
        /// The horizontal identity mapping for 2-byte CIDs; may be used with CIDFonts using any
        /// Registry, Ordering, and Supplement values. It maps 2-byte character codes ranging from
        /// 0 to 65,535 to the same 2-byte CID value, interpreted high-order byte first.
        /// </summary>
        IdentityH
    }

    /// <summary>
    /// Enumerator that implements font descriptor flags.
    /// </summary>
    internal enum FontDescriptorFlags
    {
        /// <summary>
        /// All glyphs have the same width (as opposed to proportional or variable-pitch
        /// fonts, which have different widths).
        /// </summary>
        FixedPitch = 1,

        /// <summary>
        /// Glyphs have serifs, which are short strokes drawn at an angle on the top and
        /// bottom of glyph stems (as opposed to sans serif fonts, which do not).
        /// </summary>
        Serif = 2,

        /// <summary>
        /// Font contains glyphs outside the Adobe standard Latin character set. The
        /// flag and the nonsymbolic flag cannot both be set or both be clear.
        /// </summary>
        Symbolic = 4,

        /// <summary>
        /// Glyphs resemble cursive handwriting.
        /// </summary>
        Script = 8,

        /// <summary>
        /// Font uses the Adobe standard Latin character set or a subset of it.
        /// </summary>
        Nonsymbolic = 32,

        /// <summary>
        /// Glyphs have dominant vertical strokes that are slanted.
        /// </summary>
        Italic = 64,

        /// <summary>
        /// Bold font.
        /// </summary>
        ForceBold = 0x40000,
    }

    /// <summary>
    /// Specifies the composite font types.
    /// </summary>
    internal enum CompositeFontType
    {
        /// <summary>
        /// 
        /// </summary>
        Type0,
        /// <summary>
        /// 
        /// </summary>
        TrueType
    }
}
