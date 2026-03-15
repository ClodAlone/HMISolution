#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    #region Native Types
    /*
		 * BYTE - byte: 8-bit unsigned integer.
		 * CHAR - byte: 8-bit signed integer.
		 * USHORT - ushort: 16-bit unsigned integer.
		 * SHORT	- short: 16-bit signed integer.
		 * ULONG -uint: 32-bit unsigned integer.
		 * LONG - int: 32-bit signed integer.
		 * FIXED - int: 32-bit signed fixed-point number (16.16)
		 * FUNIT - ?: Smallest measurable distance in the em space.
		 * FWORD - short: 16-bit signed integer (SHORT) that describes a quantity in FUnits.
		 * UFWORD - ushort: Unsigned 16-bit integer (USHORT) that describes a quantity in FUnits. 
		 * F2DOT14: short: 16-bit signed fixed number with the low 14 bits of fraction (2.14).
		 */
    #endregion

    /// <summary>
    /// Holds offset for TTF table from beginning of TrueType font file.
    /// </summary>
    internal struct TtfTableInfo
    {
        #region Fields
        /// <summary>
        /// Gets or sets ofset from beginning of TrueType font file.
        /// </summary>
        public int Offset;

        /// <summary>
        /// Gets or sets length of this table.
        /// </summary>
        public int Length;

        /// <summary>
        /// Gets or sets table checksum.
        /// </summary>
        public int Checksum;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="TtfTableInfo"/> is empty.
        /// </summary>
        /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
        public bool Empty
        {
            get
            {
                bool empty = (Offset == Length && Length == Checksum && Checksum == 0);
                return empty;
            }
        }
        #endregion
    }

    /// <summary>
    /// ttf metrics.
    /// </summary>
    internal struct TtfMetrics
    {
        #region Fields
        /// <summary>
        /// Typographic line gap.
        /// Negative LineGap values are treated as DEF_TABLE_CHECKSUM.
        /// </summary>
        public int LineGap;

        /// <summary>
        /// Gets or sets contains CFF.
        /// </summary>
        public bool ContainsCFF;

        /// <summary>
        /// Gets or sets value indicating if Symbol font is used.
        /// </summary>
        public bool IsSymbol;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public RECT FontBox;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public bool IsFixedPitch;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float ItalicAngle;

        /// <summary>
        /// Gets or sets post-script font name.
        /// </summary>
        public string PostScriptName;

        /// <summary>
        /// Gets or sets font family name.
        /// </summary>
        public string FontFamily;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float CapHeight;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float Leading;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float MacAscent;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float MacDescent;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float WinDescent;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float WinAscent;

        /// <summary>
        /// Gets or sets description font item.
        /// </summary>
        public float StemV;

        /// <summary>
        /// Gets or sets widths table for the font.
        /// </summary>
        public int[] WidthTable;

        /// <summary>
        /// Regular: 0
        /// Bold: 1
        /// Italic: 2
        /// Bold Italic: 3
        /// Bit 0- bold (if set to 1)
        /// Bit 1- italic (if set to 1)
        /// Bits 2-15- reserved (set to 0).
        /// NOTE:
        /// Note that macStyle bits must agree with the 'OS/2' table fsSelection bits.
        /// The fsSelection bits are used over the macStyle bits in Microsoft Windows.
        /// The PANOSE values and 'post' table values are ignored for determining bold or italic fonts.
        /// </summary>
        public int MacStyle;

        /// <summary>
        /// Subscript size factor.
        /// </summary>
        public float SubScriptSizeFactor;

        /// <summary>
        /// Superscript size factor.
        /// </summary>
        public float SuperscriptSizeFactor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is italic.
        /// </summary>
        /// <value><c>true</c> if this instance is italic; otherwise, <c>false</c>.</value>
        public bool IsItalic
        {
            get
            {
                return ((MacStyle & 2) != 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bold.
        /// </summary>
        /// <value><c>true</c> if this instance is bold; otherwise, <c>false</c>.</value>
        public bool IsBold
        {
            get
            {
                return ((MacStyle & 1) != 0);
            }
        }
        #endregion
    }

    /// <summary>
    /// name ttf table.
    /// </summary>
    internal struct TtfNameTable
    {
        #region Fields
        /// <summary>
        /// Local variable to store Format Selector.
        /// </summary>
        public ushort FormatSelector;

        /// <summary>
        /// Local variable to store Records Count.
        /// </summary>
        public ushort RecordsCount;

        /// <summary>
        /// Local variable to store Offset.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// Local variable to store Name Records.
        /// </summary>
        public TtfNameRecord[] NameRecords;
        #endregion
    }

    /// <summary>
    /// Name record.
    /// </summary>
    internal struct TtfNameRecord
    {
        #region Fields

        /// <summary>
        /// The PlatformID.
        /// </summary>
        public ushort PlatformID;

        /// <summary>
        /// The EncodingID.
        /// </summary>
        public ushort EncodingID;

        /// <summary>
        /// The PlatformIDLanguageID
        /// </summary>
        public ushort LanguageID;

        /// <summary>
        /// The NameID.
        /// </summary>
        public ushort NameID;

        /// <summary>
        /// The Length.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The Offset.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// The Name.
        /// </summary>
        public string Name;
        #endregion
    }

    /// <summary>
    /// Names of the tables.
    /// </summary>
    internal struct TtfTableNames
    {
        #region Constants
        /// <summary>
        /// The cmap.
        /// </summary>
        public const string cmap = "cmap";

        /// <summary>
        /// The glyf.
        /// </summary>
        public const string glyf = "glyf";

        /// <summary>
        /// The head.
        /// </summary>
        public const string head = "head";

        /// <summary>
        /// The hhea.
        /// </summary>
        public const string hhea = "hhea";

        /// <summary>
        /// The cmap.
        /// </summary>
        public const string hmtx = "hmtx";

        /// <summary>
        /// The loca.
        /// </summary>
        public const string loca = "loca";


        /// <summary>
        /// The maxp.
        /// </summary>
        public const string maxp = "maxp";

        /// <summary>
        /// The cmap.
        /// </summary>
        public const string name = "name";

        /// <summary>
        /// The post.
        /// </summary>
        public const string post = "post";

        /// <summary>
        /// The OS2.
        /// </summary>
        public const string OS2 = "OS/2";

        /// <summary>
        /// The CFF.
        /// </summary>
        public const string CFF = "CFF ";

        /// <summary>
        /// The cvt.
        /// </summary>
        public const string cvt = "cvt ";


        /// <summary>
        /// The fpgm.
        /// </summary>
        public const string fpgm = "fpgm";

        /// <summary>
        /// The prep.
        /// </summary>
        public const string prep = "prep";
        #endregion
    }

    /// <summary>
    /// Head table.
    /// </summary>
    internal struct TtfHeadTable
    {
        #region Fields
        /// <summary>
        /// Modified: International date (8-byte field).
        /// </summary>
        public long Modified;

        /// <summary>
        /// Created: International date (8-byte field).
        /// </summary>
        public long Created;

        /// <summary>
        /// MagicNumber: Set to 0x5F0F3CF5.
        /// </summary>
        public uint MagicNumber;

        /// <summary>
        /// CheckSumAdjustment: To compute: set it to 0, sum the entire font as ULONG,
        /// then store 0xB1B0AFBA - sum.
        /// </summary>
        public uint CheckSumAdjustment;

        /// <summary>
        /// FontRevision: Set by font manufacturer.
        /// </summary>
        public float FontRevision;

        /// <summary>
        /// Table version number: 0x00010000 for version 1.0.
        /// </summary>
        public float Version;

        /// <summary>
        /// Minimum x for all glyph bounding boxes.
        /// </summary>
        public short XMin;

        /// <summary>
        /// Minimum y for all glyph bounding boxes.
        /// </summary>
        public short YMin;

        /// <summary>
        /// Valid range is from 16 to 16384.
        /// </summary>
        public ushort UnitsPerEm;

        /// <summary>
        /// Maximum y for all glyph bounding boxes.
        /// </summary>
        public short YMax;

        /// <summary>
        /// Maximum x for all glyph bounding boxes.
        /// </summary>
        public short XMax;

        /// <summary>
        /// Regular: 0
        /// Bold: 1
        /// Italic: 2
        /// Bold Italic: 3
        /// Bit 0 - bold (if set to 1)
        /// Bit 1 - italic (if set to 1)
        /// Bits 2-15 - reserved (set to 0)
        /// NOTE:
        /// Note that macStyle bits must agree with the 'OS/2' table fsSelection bits.
        /// The fsSelection bits are used over the macStyle bits in Microsoft Windows.
        /// The PANOSE values and 'post' table values are ignored for determining bold or italic fonts.
        /// </summary>
        public ushort MacStyle;

        /// <summary>
        /// Bit 0 - baseline for font at y=0
        /// Bit 1 - left SideBearing at x=0
        ///	Bit 2 - instructions may depend on point size
        ///	Bit 3 - force ppem to integer values for all private scaler math; may use fractional ppem sizes if this bit is clear
        ///	Bit 4 - instructions may alter advance width (the advance widths might not scale linearly)
        ///	Note: All other bits must be zero.
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// LowestRecPPEM: Smallest readable size in pixels.
        /// </summary>
        public ushort LowestRecPPEM;

        /// <summary>
        /// FontDirectionHint:
        /// 0   Fully mixed directional glyphs
        /// 1   Only strongly left to right
        /// 2   Like 1 but also contains neutrals
        /// -1   Only strongly right to left
        /// -2   Like -1 but also contains neutrals.
        /// </summary>
        public short FontDirectionHint;

        /// <summary>
        /// 0 for short offsets, 1 for long.
        /// </summary>
        public short IndexToLocFormat;

        /// <summary>
        /// 0 for current format.
        /// </summary>
        public short GlyphDataFormat;
        #endregion
    }

    /// <summary>
    /// This table contains information for horizontal layout.
    /// The values in the minRightSidebearing, minLeftSideBearing, and xMaxExtent should be computed
    /// using only glyphs that have contours.
    /// Glyphs with no contours should be ignored for the purpose of these calculations.
    /// All reserved areas must be set to 0.
    /// </summary>
    internal struct TtfHorizontalHeaderTable
    {
        #region Fields
        /// <summary>
        /// Version.
        /// </summary>
        public float Version;

        /// <summary>
        /// Typographic ascent.
        /// </summary>
        public short Ascender;

        /// <summary>
        /// Maximum advance width value in HTML table.
        /// </summary>
        public ushort AdvanceWidthMax;

        /// <summary>
        /// Typographic descent.
        /// </summary>
        public short Descender;

        /// <summary>
        /// Number of hMetric entries in HTML table;
        /// may be smaller than the total number of glyphs in the font.
        /// </summary>
        public ushort NumberOfHMetrics;

        /// <summary>
        /// Typographic line gap. Negative LineGap values are treated as DEF_TABLE_CHECKSUM
        /// in Windows 3.1, System 6, and System 7.
        /// </summary>
        public short LineGap;

        /// <summary>
        /// Minimum left SideBearing value in HTML table.
        /// </summary>
        public short MinLeftSideBearing;

        /// <summary>
        /// Minimum right SideBearing value; calculated as Min(aw - lsb - (xMax - xMin)).
        /// </summary>
        public short MinRightSideBearing;

        /// <summary>
        /// Max(lsb + (xMax - xMin)).
        /// </summary>
        public short XMaxExtent;

        /// <summary>
        /// Used to calculate the slope of the cursor (rise/run); 1 for vertical.
        /// </summary>
        public short CaretSlopeRise;

        /// <summary>
        /// 0 for vertical.
        /// </summary>
        public short CaretSlopeRun;

        /// <summary>
        /// 0 for current format.
        /// </summary>
        public short MetricDataFormat;
        #endregion
    }

    /// <summary>
    /// The OS/2 table consists of a set of metrics that are required by Windows and OS/2.
    /// </summary>
    internal struct TtfOS2Table
    {
        #region Fields
        /// <summary>
        /// Struct field.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// The Average Character Width parameter specifies
        /// the arithmetic average of the escapement (width)
        /// of all of the 26 lowercase letters a through z of the Latin alphabet
        /// and the space character. If any of the 26 lowercase letters are not present,
        /// this parameter should equal the weighted average of all glyphs in the font.
        /// For non-UGL (platform 3, encoding 0) fonts, use the unweighted average.
        /// </summary>
        public short XAvgCharWidth;

        /// <summary>
        /// Indicates the visual weight (degree of blackness or thickness of strokes)
        /// of the characters in the font.
        /// </summary>
        public ushort UsWeightClass;

        /// <summary>
        /// Indicates a relative change from the normal aspect ratio (width to height ratio)
        /// as specified by a font designer for the glyphs in a font.
        /// </summary>
        public ushort UsWidthClass;

        /// <summary>
        /// Indicates font embedding licensing rights for the font.
        /// Embeddable fonts may be stored in a document.
        /// When a document with embedded fonts is opened on a system that does not have the font installed
        /// (the remote system), the embedded font may be loaded for temporary (and in some cases, permanent)
        /// use on that system by an embedding-aware application.
        /// Embedding licensing rights are granted by the vendor of the font.
        /// </summary>
        public short FsType;

        /// <summary>
        /// The recommended horizontal size in font design units for subscripts for this font.
        /// </summary>
        public short YSubscriptXSize;

        /// <summary>
        /// The recommended vertical size in font design units for subscripts for this font.
        /// </summary>
        public short YSubscriptYSize;

        /// <summary>
        /// The recommended horizontal offset in font design units for subscripts for this font.
        /// </summary>
        public short YSubscriptXOffset;

        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for subscripts for this font.
        /// </summary>
        public short YSubscriptYOffset;

        /// <summary>
        /// The recommended horizontal size in font design units for superscripts for this font.
        /// </summary>
        public short ySuperscriptXSize;

        /// <summary>
        /// The recommended vertical size in font design units for superscripts for this font.
        /// </summary>
        public short YSuperscriptYSize;

        /// <summary>
        /// The recommended horizontal offset in font design units for superscripts for this font.
        /// </summary>
        public short YSuperscriptXOffset;

        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for superscripts for this font.
        /// </summary>
        public short YSuperscriptYOffset;

        /// <summary>
        /// Width of the strikeout stroke in font design units.
        /// </summary>
        public short YStrikeoutSize;

        /// <summary>
        /// The position of the strikeout stroke relative to the baseline in font design units.
        /// </summary>
        public short YStrikeoutPosition;

        /// <summary>
        /// This parameter is a classification of font-family design.
        /// </summary>
        public short SFamilyClass;

        /// <summary>
        /// This 10 byte series of numbers are used to describe the visual characteristics
        /// of a given typeface.  These characteristics are then used to associate the font with
        /// other fonts of similar appearance having different names. The variables for each digit are listed below.
        /// The specifications for each variable can be obtained in the specification
        /// PANOSE v2.0 Numerical Evaluation from Microsoft or Elseware Corporation.
        /// </summary>
        public byte[] Panose;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint UlUnicodeRange1;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint UlUnicodeRange2;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint UlUnicodeRange3;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint UlUnicodeRange4;

        /// <summary>
        /// The four character identifier for the vendor of the given type face.
        /// </summary>
        public byte[] AchVendID;

        /// <summary>
        /// Information concerning the nature of the font patterns.
        /// </summary>
        public ushort FsSelection;

        /// <summary>
        /// The minimum Unicode index (character code) in this font,
        /// according to the cmap subtable for platform ID 3 and encoding ID 0 or 1.
        /// For most fonts supporting Win-ANSI or other character sets, this value would be 0x0020.
        /// </summary>
        public ushort UsFirstCharIndex;

        /// <summary>
        /// usLastCharIndex: The maximum Unicode index (character code) in this font,
        /// according to the cmap subtable for platform ID 3 and encoding ID 0 or 1.
        /// This value depends on which character sets the font supports.
        /// </summary>
        public ushort UsLastCharIndex;

        /// <summary>
        /// The typographic ascender for this font.
        /// Remember that this is not the same as the Ascender value in the 'hhea' table,
        /// which Apple defines in a far different manner.
        /// DEF_TABLE_OFFSET good source for usTypoAscender is the Ascender value from an AFM file.
        /// </summary>
        public short STypoAscender;

        /// <summary>
        /// The typographic descender for this font.
        /// Remember that this is not the same as the Descender value in the 'hhea' table,
        /// which Apple defines in a far different manner.
        /// DEF_TABLE_OFFSET good source for usTypoDescender is the Descender value from an AFM file.
        /// </summary>
        public short STypoDescender;

        /// <summary>
        /// The typographic line gap for this font.
        /// Remember that this is not the same as the LineGap value in the 'hhea' table,
        /// which Apple defines in a far different manner.
        /// </summary>
        public short STypoLineGap;

        /// <summary>
        /// The ascender metric for Windows.
        /// This too is distinct from Apple's Ascender value and from the usTypoAscender values.
        /// usWinAscent is computed as the yMax for all characters in the Windows ANSI character set.
        /// usTypoAscent is used to compute the Windows font height and default line spacing.
        /// For platform 3 encoding 0 fonts, it is the same as yMax.
        /// </summary>
        public ushort UsWinAscent;

        /// <summary>
        /// The descender metric for Windows.
        /// This too is distinct from Apple's Descender value and from the usTypoDescender values.
        /// usWinDescent is computed as the -yMin for all characters in the Windows ANSI character set.
        /// usTypoAscent is used to compute the Windows font height and default line spacing.
        /// For platform 3 encoding 0 fonts, it is the same as -yMin.
        /// </summary>
        public ushort UsWinDescent;

        /// <summary>
        /// This field is used to specify the code pages encompassed
        /// by the font file in the 'cmap' subtable for platform 3, encoding ID 1 (Microsoft platform).
        /// If the font file is encoding ID 0, then the Symbol Character Set bit should be set.
        /// If the bit is set (1) then the code page is considered functional.
        /// If the bit is clear (0) then the code page is not considered functional.
        /// Each of the bits is treated as an independent flag and the bits can be set in any combination.
        /// The determination of "functional" is left up to the font designer,
        /// although character set selection should attempt to be functional by code pages if at all possible.
        /// </summary>
        public uint UlCodePageRange1;

        /// <summary>
        /// This field is used to specify the code pages encompassed
        /// by the font file in the 'cmap' subtable for platform 3, encoding ID 1 (Microsoft platform).
        /// If the font file is encoding ID 0, then the Symbol Character Set bit should be set.
        /// If the bit is set (1) then the code page is considered functional.
        /// If the bit is clear (0) then the code page is not considered functional.
        /// Each of the bits is treated as an independent flag and the bits can be set in any combination.
        /// The determination of "functional" is left up to the font designer,
        /// although character set selection should attempt to be functional by code pages if at all possible.
        /// </summary>
        public uint UlCodePageRange2;

        /// <summary>
        /// Struct field.
        /// </summary>
        public short SxHeight;

        /// <summary>
        /// Struct field.
        /// </summary>
        public short SCapHeight;

        /// <summary>
        /// Struct field.
        /// </summary>
        public ushort UsDefaultChar;

        /// <summary>
        /// Struct field.
        /// </summary>
        public ushort UsBreakChar;

        /// <summary>
        /// Struct field.
        /// </summary>
        public ushort UsMaxContext;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfPostTable
    {
        #region Fields
        /// <summary>
        /// Struct field.
        /// </summary>
        public float FormatType;

        /// <summary>
        /// Struct field.
        /// </summary>
        public float ItalicAngle;

        /// <summary>
        /// Struct field.
        /// </summary>
        public short UnderlinePosition;

        /// <summary>
        /// Struct field.
        /// </summary>
        public short UnderlineThickness;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint IsFixedPitch;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint MinMemType42;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint MaxMemType42;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint MinMemType1;

        /// <summary>
        /// Struct field.
        /// </summary>
        public uint MaxMemType1;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfLongHorMertric
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort AdvanceWidth;

        /// <summary>
        /// Structure field.
        /// </summary>
        public short Lsb;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfCmapTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort TablesCount;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfCmapSubTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort PlatformID;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort EncodingID;

        /// <summary>
        /// Structure field.
        /// </summary>
        public uint Offset;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfAppleCmapSubTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Format;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Version;

        ///// <summary>
        ///// Structure field.
        ///// </summary>
        //public byte[] GlyphID;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfTrimmedCmapSubTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Format;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort FirstCode;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort EntryCount;

        ///// <summary>
        ///// Structure field.
        ///// </summary>
        //public ushort[] GlyphID;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfMicrosoftCmapSubTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Format;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort SegCountX2;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort SearchRange;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort EntrySelector;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort RangeShift;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort[] EndCount;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort ReservedPad;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort[] StartCount;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort[] IdDelta;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort[] IdRangeOffset;

        /// <summary>
        /// Structure field.
        /// </summary>
        public ushort[] GlyphID;
        #endregion
    }

    /// <summary>
    /// Holds glyph info and its width of character.
    /// </summary>
    internal struct TtfGlyphInfo : IComparable
    {
        #region Fields
        /// <summary>
        /// Holds glyph index.
        /// </summary>
        public int Index;

        /// <summary>
        /// Holds character's width.
        /// </summary>
        public int Width;

        /// <summary>
        /// Code of the char symbol.
        /// </summary>
        public int CharCode;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="TtfGlyphInfo"/> is empty.
        /// </summary>
        /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
        public bool Empty
        {
            get
            {
                bool empty = (Index == Width && Width == CharCode && CharCode == 0);
                return empty;
            }
        }
        #endregion

        #region IComparable implementation
        /// <summary>
        /// Compares two WidthDescriptor objects.
        /// </summary>
        /// <param name="obj">Another object for comparing.</param>
        /// <returns>A signed integer that indicates the relative order of this instance and value.</returns>
        public int CompareTo(object obj)
        {
            TtfGlyphInfo glyph = (TtfGlyphInfo)obj;

            return Index - glyph.Index;
        }
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfLocaTable
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public uint[] Offsets;
        #endregion
    }

    /// <summary>
    /// Ttf structure.
    /// </summary>
    internal struct TtfGlyphHeader
    {
        #region Fields
        /// <summary>
        /// Structure field.
        /// </summary>
        public short numberOfContours;

        /// <summary>
        /// Structure field.
        /// </summary>
        public short XMin;

        /// <summary>
        /// Structure field.
        /// </summary>
        public short YMin;

        /// <summary>
        /// Structure field.
        /// </summary>
        public short XMax;

        /// <summary>
        /// Structure field.
        /// </summary>
        public short YMax;
        #endregion
    }
}
