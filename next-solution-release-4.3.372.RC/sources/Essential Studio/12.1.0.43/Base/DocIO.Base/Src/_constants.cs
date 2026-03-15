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
#endregion

namespace Syncfusion.DocIO
{
    /// <summary>
    /// ControlChar class represents control characters which are used in binary MS Word document.
    /// </summary>
    public class ControlChar
    {

        #region Fields / public

        /// <summary>
        /// Carriage return character: "\x000d" or "\r". Same as ParagraphBreak. 
        /// </summary>
        public static readonly string CarriegeReturn = ('\r').ToString();

        /// <summary>
        /// Carriage return followed by line feed character: "\x000d\x000a" or "\r\n". 
        /// Not used as such in Microsoft Word documents, but commonly used in text files for 
        /// paragraph breaks. 
        /// </summary>
        public static readonly string CrLf = "\r\n";

        /// <summary>
        /// This is the "o" string used as a default value in text input form fields. 
        /// </summary>
        public static readonly string DefaultTextInput = ((char)8194).ToString();

        /// <summary>
        /// This is the 'o' character used as a default value in text input form fields. 
        /// </summary>
        public static readonly char DefaultTextInputChar = (char)8194;

        /// <summary>
        /// Line break string: "\x000b" or "\v". 
        /// </summary>
        public static readonly string LineBreak = "\v";

        /// <summary>
        /// Line break character: (char)11. 
        /// </summary>
        public static readonly char LineBreakChar = '\v';

        /// <summary>
        /// Line feed string: "\x000a" or "\n". Same as Line feed. 
        /// </summary>
        public static readonly string LineFeed = ('\n').ToString();

        /// <summary>
        /// Line feed character: (char)10. 
        /// </summary>
        public static readonly char LineFeedChar = '\n';

        /// <summary>
        /// Non-breaking space string: "\x00a0". 
        /// </summary>
        public static readonly string NonBreakingSpace = ('\x00a0').ToString();

        /// <summary>
        /// Non-breaking space character: (char)160. 
        /// </summary>
        public static readonly char NonBreakingSpaceChar = '\x00a0';

        /// <summary>
        /// Tab string: "\x0009" or "\t". 
        /// </summary>
        public static readonly string Tab = "\t";

        /// <summary>
        /// Tab character: (char)9 or "\t". 
        /// </summary>
        public static readonly char TabChar = (char)9;

        /// <summary>
        /// Optional hyphen string.
        /// </summary>
        public static readonly string Hyphen = ((char)31).ToString();

        /// <summary>
        /// Optional hyphen character.
        /// </summary>
        public static readonly char HyphenChar = (char)31;

        /// <summary>
        /// Space string.
        /// </summary>
        public static readonly string Space = ((char)32).ToString();
        /// <summary>
        /// Space character
        /// </summary>
        public static readonly char SpaceChar = (char)32;

        /// <summary>
        /// Double Quote
        /// </summary>
        public static readonly char DoubleQuote = (char)34;
        /// <summary>
        /// Left Double Quote
        /// </summary>
        public static readonly char LeftDoubleQuote = (char)8220;
        /// <summary>
        /// Right Double Quote
        /// </summary>
        public static readonly char RightDoubleQuote = (char)8221;
        /// <summary>
        /// Double low Quote
        /// </summary>
        public static readonly char DoubleLowQuote = (char)8222;
        /// <summary>
        /// Non-breaking hyphen string.
        /// </summary>
        public static readonly string NonBreakingHyphen = ((char)30).ToString();

        /// <summary>
        /// Non-breaking hyphen character.
        /// </summary>    
        public static readonly char NonBreakingHyphenChar = (char)30;
        #endregion

        #region Fields / internal
        /// <summary>
        /// End of non-nested table cell or row character: "\x0007" or "\a"
        /// </summary>
        internal static readonly string Cell = ('\a').ToString();

        /// <summary>
        /// End of non-nested table cell or row character: (char)7 or "\a". 
        /// </summary>
        internal const char CellChar = '\a';

        /// <summary>
        /// End of column character: "\x000e". 
        /// </summary>
        internal static readonly string ColumnBreak = ('\x000e').ToString();

        /// <summary>
        /// End of column character: (char)14. 
        /// </summary>
        internal const char ColumnBreakChar = '\x000e';

        /// <summary>
        /// End of MS Word field character: (char)21. 
        /// </summary>
        internal const char FieldEndChar = '\x0015';

        /// <summary>
        /// Field separator character separates field code from field value. Optional in some fields. 
        /// Value: (char)20. 
        /// </summary>
        internal const char FieldSeparatorChar = '\x0014';

        /// <summary>
        /// Start of MS Word field character: (char)19. 
        /// </summary>
        internal const char FieldStartChar = '\x0013';

        /// <summary>
        /// Page break character: "\x000c" or "\f". Note it has the same value as SectionBreak. 
        /// </summary>
        internal static readonly string PageBreak = ('\f').ToString();

        /// <summary>
        /// Page break character: (char)12 or "\f". 
        /// </summary>
        internal const char PageBreakChar = '\f';

        /// <summary>
        /// End of paragraph character: "\x000d" or "\r". Same as Carriage return.
        /// </summary>
        internal static readonly string ParagraphBreak = ('\r').ToString();

        /// <summary>
        /// End of paragraph character: (char)13. 
        /// </summary>
        internal const char ParagraphBreakChar = '\r';

        /// <summary>
        /// End of section character: "\x000c" or "\f". Note it has the same value as PageBreak. 
        /// </summary>
        internal static readonly string SectionBreak = ('\f').ToString();

        /// <summary>
        /// End of section character: (char)12 or "\f". 
        /// </summary>
        internal const char SectionBreakChar = '\f';
        #endregion
    }

    /// <summary>
    /// Summary description for _constants.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class DLSConstants
    {
        /// <summary>
        /// Specifies SaclingFactor of an Image.
        /// </summary>
        public const int ImageScalingFactor = 10;

        /// <summary>
        /// Specifies BitmapScaleFactor.
        /// </summary>
        public const int BitmapScaleFactor = 15;

        /// <summary>
        /// Specifies MetafileScaleFactor.
        /// </summary>
        public const int MetafileScaleFactor = 20;

        /// <summary>
        /// width of a single line in 1/8 pt, max of 32 pt.
        /// </summary>
        public const int BorderLineFactor = 8;

        /// <summary>
        /// Number Twips in 1 Point
        /// </summary>
        public const int TwipsInOnePoint = 20;

        /// <summary>
        /// Specifies PercentageFactor.
        /// </summary>
        public const int PercentageFactor = 50;

        /// <summary>
        /// EMUs in one point
        /// </summary>
        public const int EmusPerPoint = 12700;

        /// <summary>
        /// EMUs in one point
        /// </summary>
        public const int EmusPerPointMetafile = 16891;

        /// <summary>
        /// Hundredths unit
        /// </summary>
        internal const int HundredthsUnit = 100;

        /// <summary>
        /// Thousandths of a percentage.
        /// </summary>
        internal const int ThousandthsUnit = 1000;

        /// <summary>
        /// Positive Fixed Angle 60000ths of a degree.
        /// </summary>
        internal const int SixtyThousandthsUnit = 60000;

        /// <summary>
        /// Maximum opaque limit (100%) and 0% transparent.
        /// </summary>
        internal const int FixedPointsUnit = 65536;

        /// <summary>
        /// Size of the Int32 value.
        /// </summary>
        internal const int IntSize = 4;
        /// <summary>
        /// Size of the Int16 value.
        /// </summary>
        internal const int ShortSize = 2;
        /// <summary>
        /// Size of the Int64 value.
        /// </summary>
        internal const int LongSize = 8;
        /// <summary>
        /// Number of bits inside single short value.
        /// </summary>
        internal const int BitsInShort = 16;
        /// <summary>
        /// Number of bits inside single byte value.
        /// </summary>
        internal const int BitsInByte = 8;
        /// <summary>
        /// Size of the Double value in bytes.
        /// </summary>
        internal const int DoubleSize = 8;
        /// <summary>
        /// Microsoft Windows code page
        /// </summary>
        internal const string WindowsCodePage = "Windows-1252";
    }

    /// <summary>
    ///  Contains Ascii-codes of some special characters
    /// </summary>
    internal class SpecialCharacters
    {
        /// <summary>
        /// End of paragraph character: 0x000d.
        /// </summary>
        public const char ParagraphEnd = (char)13;

        /// <summary>
        /// End of the page character: 0x000c.
        /// </summary>
        public const char PageBreak = (char)12;

        /// <summary>
        /// End of the column character:0x000e.
        /// </summary>
        public const char ColumnBreak = (char)14;

        /// <summary>
        /// Specifies charcter for Table: 0x0007;    
        /// </summary>
        public const char TableAscii = (char)7;

        /// <summary>
        /// Specifies charcter for Image: 0x0001;  
        /// </summary>
        public const char ImageAscii = (char)1;

        /// <summary>
        /// Specifies character for Shape: 0x0008.
        /// </summary>
        public const char ShapeAscii = (char)8;

        /// <summary>
        /// Specifies character value for FootNote:0x0002.
        /// </summary>
        public const char FootnoteAscii = (char)2;

        /// <summary>
        /// Begining of Field Mark character: 0x0013.
        /// </summary>
        public const char FieldBeginMark = (char)19;

        /// <summary>
        /// End of Field Mark character: 0x0015.
        /// </summary>
        public const char FieldEndMark = (char)21;

        /// <summary>
        /// Specifies character value for FieldSeparator: 0x0014.
        /// </summary>
        public const char FieldSeparator = (char)20;

        /// <summary>
        /// Specifies character value for Tab: 0x0009.
        /// </summary>
        public const char TabAscii = (char)9;

        /// <summary>
        /// Specifies character value for line break: 0x000b.
        /// </summary>
        public const char LineBreakAscii = (char)11;

        /// <summary>
        /// Specifies character value for symbol: 0x0028.
        /// </summary>
        public const char SymbolAscii = (char)40;

        /// <summary>
        /// Specifies value for Annotation: 0x0005.
        /// </summary>
        public const char AnnotationAscii = (char)5;

        /// <summary>
        /// Specifies character value for page number: 0x0000.
        /// </summary>
        public const char CurrPageNumber = (char)0;

        /// <summary>
        /// Specifies <see cref="FootnoteAscii"/> value as string.
        /// </summary>
        public static readonly string FootnoteAsciiStr = FootnoteAscii.ToString();

        /// <summary>
        /// Specifies <see cref="PageBreak"/> value as string.
        /// </summary>
        public static readonly string PageBreakStr = PageBreak.ToString();

        internal const char NonBreakingHyphen = (char)0x1E;
        internal const char SoftHyphen = (char)0x1F;
        internal const char NonBreakingSpace = (char)0xA0;
        /// <summary>
        /// Specifies the Footnotes/Endnotes separator character.
        /// </summary>
        internal const char Separator = (char)0x03;
        /// <summary>
        /// Specifies the Footnotes/Endnotes continuation separator character.
        /// </summary>
        internal const char ContinuationSeparator = (char)0x04;
        /// <summary>
        /// Array of special symbols.
        /// </summary>
        public static readonly char[] SpecialSymbolArr =
          new char[] { ParagraphEnd, PageBreak, ColumnBreak, ImageAscii, TableAscii, FootnoteAscii, TabAscii, FieldBeginMark, FieldSeparator, FieldEndMark, LineBreakAscii, ShapeAscii, AnnotationAscii };
    }
}