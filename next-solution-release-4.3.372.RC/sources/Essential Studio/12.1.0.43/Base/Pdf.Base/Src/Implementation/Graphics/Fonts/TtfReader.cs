#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Native;


namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Reader of the TTF data.
    /// </summary>
    internal class TtfReader
    {
        #region Constants
        /// <summary>
        /// Encoding class.
        /// </summary>
#if SILVERLIGHT        
        internal static readonly Encoding Encoding = Encoding.Unicode;
#else
        internal static readonly Encoding Encoding = Encoding.GetEncoding("windows-1252");
#endif
        /// <summary>
        /// Width multiplier.
        /// </summary>
        internal const int WidthMultiplier = 1000;
        /// <summary>
        /// Version of Ttf file.
        /// </summary>
        private const int c_ttfVersion1 = 0x10000;
        /// <summary>
        /// Version of Ttf file.
        /// </summary>
        private const int c_ttfVersion2 = 0x4f54544f;
        /// <summary>
        /// FP.
        /// </summary>
        private const int c_fp = 16;
        /// <summary>
        /// Aray of table names.
        /// </summary>
        private static readonly string[] s_tableNames;
        /// <summary>
        /// Array of table names.
        /// </summary>
        private static readonly string[] m_tableNames;
        /// <summary>
        /// Integer's table. One of this integer would be used as a key
        /// for writing some info in the header of the font program.
        /// </summary>
        private static readonly short[] s_entrySelectors;
        #endregion

        #region Fields
        /// <summary>
        /// Binary reader object.
        /// </summary>
        private BigEndianReader m_reader;
        /// <summary>
        /// The whole list of tables loaded from Ttf.
        /// </summary>
        private Dictionary<string, TtfTableInfo> m_tableDirectory;
        /// <summary>
        /// Ttf metrics.
        /// </summary>
        private TtfMetrics m_metrics;
        /// <summary>
        /// Width table.
        /// </summary>
        private int[] m_width;
        /// <summary>
        /// Glyphs for Macintosh or Symbol fonts (char - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> m_macintosh;
        /// <summary>
        /// Glyphs for Microsoft Unicode fonts (char - key, glyph - value)..
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> m_microsoft;
        /// <summary>
        /// Glyphs for Macintosh or Symbol fonts (glyph index - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> m_macintoshGlyphs;
        /// <summary>
        /// Glyphs for Microsoft Unicode fonts (glyph index - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> m_microsoftGlyphs;
        /// <summary>
        /// Indicates whether loca table is short.
        /// </summary>
        private bool m_bIsLocaShort;
        /// <summary>
        /// Indicates whether font is truetype subset
        /// </summary>
        private bool m_subset;
        
        private long m_lowestPosition;
        private int m_maxMacIndex;

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Indicates the current font which is currently under processing.
        /// </summary>
        private Font m_font = null;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets binary reader.
        /// </summary>
        public BinaryReader Reader
        {
            get
            {
                return m_reader.Reader;
            }
            set
            {
                m_reader.Reader = value;
            }
        }

        /// <summary>
        /// Gets BigEndian internal reader.
        /// </summary>
        public BigEndianReader InternalReader
        {
            get
            {
                return m_reader;
            }
        }

        /// <summary>
        /// Gets metrics of the font.
        /// </summary>
        public TtfMetrics Metrics
        {
            get
            {
                return m_metrics;
            }
        }

        /// <summary>
        /// The whole list of tables loaded from Ttf.
        /// </summary>
        private Dictionary<string, TtfTableInfo> TableDirectory
        {
            get
            {
                if (m_tableDirectory == null)
                {
                    m_tableDirectory = new Dictionary<string,TtfTableInfo>();
                }

                return m_tableDirectory;
            }
        }

        /// <summary>
        /// Gets glyphs for Macintosh or Symbol fonts (char - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> Macintosh
        {
            get
            {
                if (m_macintosh == null)
                {
                    m_macintosh = new Dictionary<int, TtfGlyphInfo>();
                }

                return m_macintosh;
            }
        }

        /// <summary>
        /// Gets glyphs for Microsoft Unicode fonts (char - key, glyph - value)..
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> Microsoft
        {
            get
            {
                if (m_microsoft == null)
                {
                    m_microsoft = new Dictionary<int, TtfGlyphInfo>();
                }

                return m_microsoft;
            }
        }

        /// <summary>
        /// Gets glyphs for Macintosh or Symbol fonts (glyph index - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> MacintoshGlyphs
        {
            get
            {
                if (m_macintoshGlyphs == null)
                {
                    m_macintoshGlyphs = new Dictionary<int, TtfGlyphInfo>();
                }

                return m_macintoshGlyphs;
            }
        }

        /// <summary>
        /// Gets glyphs for Microsoft Unicode fonts (glyph index - key, glyph - value).
        /// </summary>
        private Dictionary<int, TtfGlyphInfo> MicrosoftGlyphs
        {
            get
            {
                if (m_microsoftGlyphs == null)
                {
                    m_microsoftGlyphs = new Dictionary<int, TtfGlyphInfo>();
                }

                return m_microsoftGlyphs;
            }
        }
        /// /// <summary>
        /// Array of table names.
        /// </summary>
        /// <value>Table names</value>
        private string[] TableNames
        {
            get
            {
#if!SILVERLIGHT && !NETFX_CORE && !WP
                if (TrueTypeSubset)
                    return m_tableNames;
                else
#endif
                    return s_tableNames;
            }
        }
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <value>The font.</value>
        internal Font Font
        {
            get
            {
                return m_font;
            }
        }
        /// <summary>
        /// Indicates the truetypefont is subet
        /// </summary>
        /// <value><c>true</c> if embeded subset; otherwise, <c>false</c>.</value>
        internal bool TrueTypeSubset
        {
            get
            {
                return m_subset;
            }
            set
            {
                m_subset = value;
            }
        }       
        
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor.
        /// </summary>
        static TtfReader()
        {
            s_tableNames = new string[9];
            s_tableNames[0] = TtfTableNames.cvt;
            s_tableNames[1] = TtfTableNames.fpgm;
            s_tableNames[2] = TtfTableNames.glyf;
            s_tableNames[3] = TtfTableNames.head;
            s_tableNames[4] = TtfTableNames.hhea;
            s_tableNames[5] = TtfTableNames.hmtx;
            s_tableNames[6] = TtfTableNames.loca;
            s_tableNames[7] = TtfTableNames.maxp;
            s_tableNames[8] = TtfTableNames.prep;

            m_tableNames = new string[10];
            m_tableNames[0] = TtfTableNames.cmap;
            m_tableNames[1] = TtfTableNames.cvt;
            m_tableNames[2] = TtfTableNames.fpgm;
            m_tableNames[3] = TtfTableNames.glyf;
            m_tableNames[4] = TtfTableNames.head;
            m_tableNames[5] = TtfTableNames.hhea;
            m_tableNames[6] = TtfTableNames.hmtx;
            m_tableNames[7] = TtfTableNames.loca;
            m_tableNames[8] = TtfTableNames.maxp;
            m_tableNames[9] = TtfTableNames.prep;


            s_entrySelectors = new short[]
				{
					0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4
				};
        }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public TtfReader(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            m_reader = new BigEndianReader(reader);

            Initialize();
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public TtfReader(BinaryReader reader, Font font)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            m_reader = new BigEndianReader(reader);

            m_font = font;

            Initialize();
        }
#endif

        /// <summary>
        /// Closes all the resources.
        /// </summary>
        public void Close()
        {
            if (m_reader != null)
            {
                m_reader.Close();
                m_reader = null;
            }

            if (m_tableDirectory != null)
            {
                m_tableDirectory.Clear();
                m_tableDirectory = null;
            }

            if (m_macintosh != null)
            {
                m_macintosh.Clear();
                m_macintosh = null;
            }

            if (m_microsoft != null)
            {
                m_microsoft.Clear();
                m_microsoft = null;
            }

            if (m_macintoshGlyphs != null)
            {
                m_macintoshGlyphs.Clear();
                m_macintoshGlyphs = null;
            }

            if (m_microsoftGlyphs != null)
            {
                m_microsoftGlyphs.Clear();
                m_microsoftGlyphs = null;
            }

            m_width = null;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets glyph's info by char code.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        public TtfGlyphInfo GetGlyph(char charCode)
        {
            object obj = null;
            int code = (int)charCode;

            if (!m_metrics.IsSymbol && m_microsoft != null)
            {
#if !SILVERLIGHT && !NETFX_CORE && !WP
                if (Font != null && (Font.Name.ToLower() == "gautami" || Font.Name.ToLower() == "latha" || Font.Name.ToLower() == "shruti"
                   || Font.Name.ToLower() == "mangal" || Font.Name.ToLower() == "tunga" || Font.Name.ToLower() == "vrinda"))
                {
                    if (m_width.Length > code)
                    {
                        TtfGlyphInfo newGlyph = new TtfGlyphInfo();
                        newGlyph.CharCode = code;
                        newGlyph.Index = code;
                        newGlyph.Width = m_width[code];
                        return newGlyph;
                    }
                } 
#endif
                if (m_microsoft.ContainsKey(code))
                    obj = m_microsoft[code];
            }
            else if (m_metrics.IsSymbol && m_macintosh != null)
            {
                // NOTE: this code fixes char codes that extends 0x100. However, it might corrupt something.
                code %= m_maxMacIndex + 1;

                if(m_macintosh.ContainsKey(code))
                    obj = m_macintosh[code];

                if (obj == null && PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
                    obj = new TtfGlyphInfo();
            }

            // Fix for StackOverFlow exception in XPS to PDF converter
            if (charCode == StringTokenizer.WhiteSpace && obj == null)
                obj = new TtfGlyphInfo();

            TtfGlyphInfo glyph = (obj != null) ? (TtfGlyphInfo)obj : GetDefaultGlyph();

            return glyph;
        }

        /// <summary>
        /// Gets glyph's info by glyph index..
        /// </summary>
        /// <param name="glyphIndex">Glyph index.</param>
        public TtfGlyphInfo GetGlyph(int glyphIndex)
        {
            object obj = null;

            if (!m_metrics.IsSymbol && m_microsoftGlyphs != null)
            {
                if (m_microsoftGlyphs.ContainsKey(glyphIndex))
                    obj = m_microsoftGlyphs[glyphIndex];
            }
            else if (m_metrics.IsSymbol && m_macintoshGlyphs != null)
            {
                if (m_macintoshGlyphs.ContainsKey(glyphIndex))
                    obj = m_macintoshGlyphs[glyphIndex];
            }

            TtfGlyphInfo glyph = (obj != null) ? (TtfGlyphInfo)obj : GetDefaultGlyph();

            return glyph;
        }

        /// <summary>
        /// Creates fonts internals.
        /// </summary>
        public void CreateInternals()
        {
            ReadMetrics();
        }

        /// <summary>
        /// Reads a font's program.
        /// </summary>
        /// <param name="chars">Array of used chars.</param>
        /// <returns>Binary font data.</returns>
        public byte[] ReadFontProgram(Dictionary<char, char> chars)
        {
            Dictionary<int, int> glyphChars = GetGlyphChars(chars);
            TtfLocaTable locaTable = ReadLocaTable(m_bIsLocaShort);

            UpdateGlyphChars(glyphChars, locaTable);

            int[] newLocaTable = null;
            byte[] newGlyphTable = null;
            byte[] newLocaUpdated = null;
            uint glyphTableSize = GenerateGlyphTable(glyphChars, locaTable, out newLocaTable, out newGlyphTable);
            int newLocaSize = UpdateLocaTable(newLocaTable, m_bIsLocaShort, out newLocaUpdated);
            byte[] fontProgram = GetFontProgram(newLocaUpdated, newGlyphTable, glyphTableSize, (uint)newLocaSize);

            return fontProgram;
        }

        /// <summary>
        /// Reconverts string to be in proper format saved into PDF file.
        /// Return value would be in string.
        /// </summary>
        /// <param name="text">String to be reconverted.</param>
        /// <returns>Reconverted string.</returns>
        public string ConvertString(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            char[] glyph = new char[text.Length];
            int i = 0;

            for (int k = 0, len = text.Length; k < len; k++)
            {
                char ch = text[k];
                TtfGlyphInfo glyphInfo = GetGlyph(ch);

                //Debug.WriteLineIf(glyphInfo.Empty, text[k], "Char not found");

                if (!glyphInfo.Empty)
                {
                    glyph[i++] = (char)glyphInfo.Index;
                }
            }

            return new string(glyph, 0, i);
        }

        /// <summary>
        /// Gets char width.
        /// </summary>
        /// <param name="code">Char for which to measure the width.</param>
        /// <returns>Char width.</returns>
        public int GetCharWidth(char code)
        {
            TtfGlyphInfo glyphInfo = GetGlyph(code);

            glyphInfo = (!glyphInfo.Empty) ? glyphInfo : GetDefaultGlyph();

            int codeWidth = (!glyphInfo.Empty) ? glyphInfo.Width : 0;

            return codeWidth;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets hashtable with chars indexed by glyph index.
        /// </summary>
        /// <param name="chars">Chars that are used in destination output.</param>
        /// <returns>Hashtable with chars indexed by glyph index.</returns>
        internal Dictionary<int, int> GetGlyphChars(Dictionary<char, char> chars)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");

            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            foreach (KeyValuePair<char,char> entry in chars)
            {
                char ch = entry.Key;
                TtfGlyphInfo glyph = GetGlyph(ch);

                if (!glyph.Empty)
                {
                    dictionary[glyph.Index] = (int)ch;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Provides basic parsing required for font comparing (FontFamily and MacStyle).
        /// </summary>
        private void Initialize()
        {

            ReadFontDirectory();

            TtfNameTable nameTable = ReadNameTable();
            TtfHeadTable headTable = ReadHeadTable();

            InitializeFontName(nameTable);
            m_metrics.MacStyle = headTable.MacStyle;
        }

        /// <summary>
        /// Reads font directory.
        /// </summary>
        private void ReadFontDirectory()
        {
            m_reader.Seek(0);
            CheckPreambula();

            int numTables = m_reader.ReadInt16();

            int additionalOffset = 0;
            bool bWasChecked = false;

            m_reader.Skip(BigEndianReader.Int16Size * 3);

            // Retrieve tables.
            for (int i = 0; i < numTables; ++i)
            {
                TtfTableInfo table = new TtfTableInfo();
                string tableKey = m_reader.ReadString(BigEndianReader.Int32Size);

                table.Checksum = m_reader.ReadInt32();
                table.Offset = m_reader.ReadInt32();
                table.Length = m_reader.ReadInt32();

#if !SILVERLIGHT && !NETFX_CORE && !WP
                string fontName = (Font != null) ? Font.Name.ToLower()
                    : string.Empty;

                //NOTE: This code fixes the issues with chinese fonts.
                if (PdfDocument.EnableCache)
                {
                    lock (PdfDocument.Cache)
                    {
                        if (!bWasChecked && Font != null && PdfDocument.Cache.FontOffsetTable.ContainsKey(Font))
                        {
                            //Optimizing to avoid memory overhead with known offsets.
                            additionalOffset = (int)PdfDocument.Cache.FontOffsetTable[Font];
                            bWasChecked = true;
                        }
                        else if (!bWasChecked && Font != null && !PdfDocument.Cache.FontOffsetTable.ContainsKey(Font))
                        {
                            additionalOffset = NormalizeOffset(table, tableKey, m_reader);
                            PdfDocument.Cache.FontOffsetTable.Add(Font, additionalOffset);
                            bWasChecked = true;
                        }
                    }
                
                }
                else
                {
                    additionalOffset = NormalizeOffset(table, tableKey, m_reader);
                }


                if (additionalOffset != 0)
                {
                    table.Offset += additionalOffset;
                }
#endif
                TableDirectory[tableKey] = table;
            }

            if (!bWasChecked)
            {
                m_lowestPosition = m_reader.BaseStream.Position;
                FixOffsets();
            }
        }



        /// <summary>
        /// Fixes the offsets of the font tables.
        /// </summary>
        private void FixOffsets()
        {
            int minOffset = int.MaxValue;
            // Search for a smallest offset and compare it with the lowest position found.
            foreach (KeyValuePair<string, TtfTableInfo> pair in TableDirectory)
            {
                object value = pair.Value;
                int offset = ((TtfTableInfo)value).Offset;

                if (minOffset > offset)
                {
                    minOffset = offset;

                    if (minOffset <= m_lowestPosition) break;
                }
            }

            int shift = minOffset - (int)m_lowestPosition;

            if (shift != 0)
            {
                Dictionary<string, TtfTableInfo> table = new Dictionary<string, TtfTableInfo>();

                foreach (KeyValuePair<string, TtfTableInfo> kv in TableDirectory)
                {
                    TtfTableInfo ti = TableDirectory[kv.Key];
                    ti.Offset -= shift;
                    table[kv.Key] = ti;
                }

                m_tableDirectory = table;
            }
        }

        /// <summary>
        /// Reads font metrics.
        /// </summary>
        private void ReadMetrics()
        {
            m_metrics = new TtfMetrics();

            TtfNameTable nameTable = ReadNameTable();
            TtfHeadTable headTable = ReadHeadTable();

            m_bIsLocaShort = (headTable.IndexToLocFormat == 0);

            TtfHorizontalHeaderTable horizontalHeadTable = ReadHorizontalHeaderTable();
            TtfOS2Table os2Table = ReadOS2Table();
            TtfPostTable postTable = ReadPostTable();

            m_width = ReadWidthTable(horizontalHeadTable.NumberOfHMetrics, headTable.UnitsPerEm);

            TtfCmapSubTable[] subTables = ReadCmapTable();

            InitializeMetrics(nameTable, headTable, horizontalHeadTable, os2Table, postTable, subTables);
        }

        /// <summary>
        /// Initializes metrics.
        /// </summary>
        /// <param name="nameTable">Name table.</param>
        /// <param name="headTable">Head table.</param>
        /// <param name="horizontalHeadTable">Horizontal head table.</param>
        /// <param name="os2Table">OS/2 table.</param>
        /// <param name="postTable">Post table.</param>
        /// <param name="cmapTables">Cmap subtables.</param>
        private void InitializeMetrics(TtfNameTable nameTable,
            TtfHeadTable headTable, TtfHorizontalHeaderTable horizontalHeadTable,
            TtfOS2Table os2Table, TtfPostTable postTable, TtfCmapSubTable[] cmapTables)
        {
            if (cmapTables == null)
                throw new ArgumentNullException("cmapTables");

            InitializeFontName(nameTable);

            // Get font encoding.
            bool bSymbol = false;

            for (int i = 0; i < cmapTables.Length; i++)
            {
                TtfCmapSubTable subTable = cmapTables[i];
                TtfCmapEncoding encoding = GetCmapEncoding(subTable.PlatformID, subTable.EncodingID);

                if (encoding == TtfCmapEncoding.Symbol)
                {
                    bSymbol = true;
                    break;
                }
            }

            m_metrics.IsSymbol = bSymbol;
            m_metrics.MacStyle = headTable.MacStyle;
            m_metrics.IsFixedPitch = (postTable.IsFixedPitch != 0);
            m_metrics.ItalicAngle = postTable.ItalicAngle;

            float factor = (float)WidthMultiplier / (float)headTable.UnitsPerEm;

            m_metrics.WinAscent = os2Table.STypoAscender * factor;
            m_metrics.MacAscent = horizontalHeadTable.Ascender * factor;
            //m_metrics.MacAscent = os2Table.UsWinAscent * factor;

            // NOTE: This is stange workaround. The value is good if os2Table.SCapHeight != 0, otherwise it should be properly computed.
            m_metrics.CapHeight = (os2Table.SCapHeight != 0) ? os2Table.SCapHeight : 0.7f * headTable.UnitsPerEm * factor;

            m_metrics.WinDescent = os2Table.STypoDescender * factor;
            m_metrics.MacDescent = horizontalHeadTable.Descender * factor;
            //m_metrics.MacDescent = -os2Table.UsWinDescent * factor;

            m_metrics.Leading = (os2Table.STypoAscender - os2Table.STypoDescender + os2Table.STypoLineGap) * factor;
            m_metrics.LineGap = (int)Math.Ceiling(horizontalHeadTable.LineGap * factor);

            int left = (int)(headTable.XMin * factor);
            int top = (int)Math.Ceiling(m_metrics.MacAscent + m_metrics.LineGap);
            int right = (int)(headTable.XMax * factor);
            int bottom = (int)m_metrics.MacDescent;

            m_metrics.FontBox = new RECT(left, top, right, bottom);
            // NOTE: Strange!
            m_metrics.StemV = 80f;
            m_metrics.WidthTable = UpdateWidth();
            m_metrics.ContainsCFF = (TableDirectory.ContainsKey(TtfTableNames.CFF));

            m_metrics.SubScriptSizeFactor = (float)headTable.UnitsPerEm / (float)os2Table.YSubscriptYSize;
            m_metrics.SuperscriptSizeFactor = (float)headTable.UnitsPerEm / (float)os2Table.YSuperscriptYSize;
        }

        /// <summary>
        /// Reads name table.
        /// </summary>
        /// <returns>Name table.</returns>
        private TtfNameTable ReadNameTable()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.name);

            m_reader.Seek(tableInfo.Offset);

            TtfNameTable table = new TtfNameTable();

            table.FormatSelector = m_reader.ReadUInt16();
            table.RecordsCount = m_reader.ReadUInt16();
            table.Offset = m_reader.ReadUInt16();
            table.NameRecords = new TtfNameRecord[table.RecordsCount];

            long position = m_reader.BaseStream.Position;
            int recordSize = 12;// Marshal.SizeOf(typeof(TtfNameRecord)) - BigEndianReader.Int32Size;

            for (int i = 0, len = (int)table.RecordsCount; i < len; i++)
            {
                m_reader.Seek(position);

                TtfNameRecord record = new TtfNameRecord();

                record.PlatformID = m_reader.ReadUInt16();
                record.EncodingID = m_reader.ReadUInt16();
                record.LanguageID = m_reader.ReadUInt16();
                record.NameID = m_reader.ReadUInt16();
                record.Length = m_reader.ReadUInt16();
                record.Offset = m_reader.ReadUInt16();

                long offset = tableInfo.Offset + table.Offset + record.Offset;

                m_reader.Seek(offset);

                bool unicode = ((int)record.PlatformID == (int)TtfPlatformID.AppleUnicode ||
                    (int)record.PlatformID == (int)TtfPlatformID.Microsoft);

                record.Name = m_reader.ReadString(record.Length, unicode);

                table.NameRecords[i] = record;
                position += recordSize;
            }

            return table;
        }

        /// <summary>
        /// Reads head table.
        /// </summary>
        /// <returns>Head table.</returns>
        private TtfHeadTable ReadHeadTable()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.head);

            m_reader.Seek(tableInfo.Offset);

            TtfHeadTable table = new TtfHeadTable();

            table.Version = m_reader.ReadFixed();
            table.FontRevision = m_reader.ReadFixed();
            table.CheckSumAdjustment = m_reader.ReadUInt32();
            table.MagicNumber = m_reader.ReadUInt32();
            table.Flags = m_reader.ReadUInt16();
            table.UnitsPerEm = m_reader.ReadUInt16();
            table.Created = m_reader.ReadInt64();
            table.Modified = m_reader.ReadInt64();
            table.XMin = m_reader.ReadInt16();
            table.YMin = m_reader.ReadInt16();
            table.XMax = m_reader.ReadInt16();
            table.YMax = m_reader.ReadInt16();
            table.MacStyle = m_reader.ReadUInt16();
            table.LowestRecPPEM = m_reader.ReadUInt16();
            table.FontDirectionHint = m_reader.ReadInt16();
            table.IndexToLocFormat = m_reader.ReadInt16();
            table.GlyphDataFormat = m_reader.ReadInt16();

            return table;
        }

        /// <summary>
        /// Reads horizontal header table.
        /// </summary>
        /// <returns>Horizontal header table.</returns>
        private TtfHorizontalHeaderTable ReadHorizontalHeaderTable()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.hhea);

            m_reader.Seek(tableInfo.Offset);

            TtfHorizontalHeaderTable table = new TtfHorizontalHeaderTable();

            table.Version = m_reader.ReadFixed();
            table.Ascender = m_reader.ReadInt16();
            table.Descender = m_reader.ReadInt16();
            table.LineGap = m_reader.ReadInt16();
            table.AdvanceWidthMax = m_reader.ReadUInt16();
            table.MinLeftSideBearing = m_reader.ReadInt16();
            table.MinRightSideBearing = m_reader.ReadInt16();
            table.XMaxExtent = m_reader.ReadInt16();
            table.CaretSlopeRise = m_reader.ReadInt16();
            table.CaretSlopeRun = m_reader.ReadInt16();

            m_reader.Skip(BigEndianReader.Int16Size * 5);

            table.MetricDataFormat = m_reader.ReadInt16();
            table.NumberOfHMetrics = m_reader.ReadUInt16();

            return table;
        }

        /// <summary>
        /// Reads OS2 table.
        /// </summary>
        /// <returns></returns>
        private TtfOS2Table ReadOS2Table()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.OS2);

            m_reader.Seek(tableInfo.Offset);

            TtfOS2Table table = new TtfOS2Table();

            table.Version = m_reader.ReadUInt16();
            table.XAvgCharWidth = m_reader.ReadInt16();
            table.UsWeightClass = m_reader.ReadUInt16();
            table.UsWidthClass = m_reader.ReadUInt16();
            table.FsType = m_reader.ReadInt16();
            table.YSubscriptXSize = m_reader.ReadInt16();
            table.YSubscriptYSize = m_reader.ReadInt16();
            table.YSubscriptXOffset = m_reader.ReadInt16();
            table.YSubscriptYOffset = m_reader.ReadInt16();
            table.ySuperscriptXSize = m_reader.ReadInt16();
            table.YSuperscriptYSize = m_reader.ReadInt16();
            table.YSuperscriptXOffset = m_reader.ReadInt16();
            table.YSuperscriptYOffset = m_reader.ReadInt16();
            table.YStrikeoutSize = m_reader.ReadInt16();
            table.YStrikeoutPosition = m_reader.ReadInt16();
            table.SFamilyClass = m_reader.ReadInt16();
            table.Panose = m_reader.ReadBytes(10);
            table.UlUnicodeRange1 = m_reader.ReadUInt32();
            table.UlUnicodeRange2 = m_reader.ReadUInt32();
            table.UlUnicodeRange3 = m_reader.ReadUInt32();
            table.UlUnicodeRange4 = m_reader.ReadUInt32();
            table.AchVendID = m_reader.ReadBytes(4);
            table.FsSelection = m_reader.ReadUInt16();
            table.UsFirstCharIndex = m_reader.ReadUInt16();
            table.UsLastCharIndex = m_reader.ReadUInt16();
            table.STypoAscender = m_reader.ReadInt16();
            table.STypoDescender = m_reader.ReadInt16();
            table.STypoLineGap = m_reader.ReadInt16();
            table.UsWinAscent = m_reader.ReadUInt16();
            table.UsWinDescent = m_reader.ReadUInt16();
            table.UlCodePageRange1 = m_reader.ReadUInt32();
            table.UlCodePageRange2 = m_reader.ReadUInt32();

            if (table.Version > 1)
            {
                table.SxHeight = m_reader.ReadInt16();
                table.SCapHeight = m_reader.ReadInt16();
                table.UsDefaultChar = m_reader.ReadUInt16();
                table.UsBreakChar = m_reader.ReadUInt16();
                table.UsMaxContext = m_reader.ReadUInt16();
            }

            return table;
        }

        /// <summary>
        /// Reads post table.
        /// </summary>
        /// <returns></returns>
        private TtfPostTable ReadPostTable()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.post);

            m_reader.Seek(tableInfo.Offset);

            TtfPostTable table = new TtfPostTable();

            table.FormatType = m_reader.ReadFixed();
            table.ItalicAngle = m_reader.ReadFixed();
            table.UnderlinePosition = m_reader.ReadInt16();
            table.UnderlineThickness = m_reader.ReadInt16();
            table.IsFixedPitch = m_reader.ReadUInt32();
            table.MinMemType42 = m_reader.ReadUInt32();
            table.MaxMemType42 = m_reader.ReadUInt32();
            table.MinMemType1 = m_reader.ReadUInt32();
            table.MaxMemType1 = m_reader.ReadUInt32();

            return table;
        }

        /// <summary>
        /// Reads Width of the glyphs.
        /// </summary>
        /// <param name="glyphCount">Number of glyphs.</param>
        /// <param name="unitsPerEm">Power of 2.</param>
        /// <returns>Width of the glyphs.</returns>
        private int[] ReadWidthTable(int glyphCount, int unitsPerEm)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.hmtx);

            m_reader.Seek(tableInfo.Offset);

            int[] width = new int[glyphCount];

            for (int i = 0; i < glyphCount; i++)
            {
                TtfLongHorMertric glyph = new TtfLongHorMertric();

                glyph.AdvanceWidth = m_reader.ReadUInt16();
                glyph.Lsb = m_reader.ReadInt16();

                int glyphWidth = glyph.AdvanceWidth * WidthMultiplier / unitsPerEm;

                width[i] = glyphWidth;
            }

            return width;
        }

        /// <summary>
        /// Reads cmap table.
        /// </summary>
        /// <returns>Array of subtables.</returns>
        private TtfCmapSubTable[] ReadCmapTable()
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.cmap);

            m_reader.Seek(tableInfo.Offset);


            TtfCmapTable table = new TtfCmapTable();

            table.Version = m_reader.ReadUInt16();
            table.TablesCount = m_reader.ReadUInt16();

            long position = m_reader.BaseStream.Position;
            TtfCmapSubTable[] subTables = new TtfCmapSubTable[table.TablesCount];

            for (int i = 0; i < table.TablesCount; i++)
            {
                m_reader.Seek(position);

                TtfCmapSubTable subTable = new TtfCmapSubTable();

                subTable.PlatformID = m_reader.ReadUInt16();
                subTable.EncodingID = m_reader.ReadUInt16();
                subTable.Offset = m_reader.ReadUInt32();
                position = m_reader.BaseStream.Position;

                ReadCmapSubTable(subTable);
                subTables[i] = subTable;
            }

            return subTables;
        }

        /// <summary>
        /// Reads cmap subtables.
        /// </summary>
        /// <param name="subTable">Cmap subtable.</param>
        private void ReadCmapSubTable(TtfCmapSubTable subTable)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.cmap);

            m_reader.Seek(tableInfo.Offset + subTable.Offset);

            TtfCmapFormat format = (TtfCmapFormat)m_reader.ReadUInt16();
            TtfCmapEncoding encoding = GetCmapEncoding(subTable.PlatformID, subTable.EncodingID);
            TtfPlatformID platform = (encoding == TtfCmapEncoding.Macintosh) ?
                TtfPlatformID.Macintosh : TtfPlatformID.Microsoft;

            if (encoding != TtfCmapEncoding.Unknown)
            {
                switch (format)
                {
                    case TtfCmapFormat.Apple:
                        ReadAppleCmapTable(subTable, encoding);
                        break;

                    case TtfCmapFormat.Microsoft:
                        ReadMicrosoftCmapTable(subTable, encoding);
                        break;

                    case TtfCmapFormat.Trimmed:
                        ReadTrimmedCmapTable(subTable, encoding);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads Symbol cmap table.
        /// </summary>
        /// <param name="subTable">Cmap subtable.</param>
        /// <param name="encoding">Encoding ID.</param>
        private void ReadAppleCmapTable(TtfCmapSubTable subTable, TtfCmapEncoding encoding)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.cmap);

            m_reader.Seek(tableInfo.Offset + subTable.Offset);

            TtfAppleCmapSubTable table = new TtfAppleCmapSubTable();

            table.Format = m_reader.ReadUInt16();
            table.Length = m_reader.ReadUInt16();
            table.Version = m_reader.ReadUInt16();

            for (int i = 0, len = Byte.MaxValue + 1; i < len; ++i)
            {
                TtfGlyphInfo glyphInfo = new TtfGlyphInfo();

                glyphInfo.Index = (int)m_reader.ReadByte();
                glyphInfo.Width = GetWidth(glyphInfo.Index);
                glyphInfo.CharCode = i;

                Macintosh[i] = glyphInfo;
                AddGlyph(glyphInfo, encoding);
                // NOTE: this code fixes char codes that extends 0x100. However, it might corrupt something.
                m_maxMacIndex = Math.Max(i, m_maxMacIndex);
            }
        }

        /// <summary>
        /// Reads Symbol cmap table.
        /// </summary>
        /// <param name="subTable">Cmap subtable.</param>
        /// <param name="encoding">Encoding ID.</param>
        private void ReadMicrosoftCmapTable(TtfCmapSubTable subTable, TtfCmapEncoding encoding)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.cmap);

            m_reader.Seek(tableInfo.Offset + subTable.Offset);

            Dictionary<int, TtfGlyphInfo> collection = (encoding == TtfCmapEncoding.Unicode) ? Microsoft : Macintosh;
            TtfMicrosoftCmapSubTable table = new TtfMicrosoftCmapSubTable();

            table.Format = m_reader.ReadUInt16();
            table.Length = m_reader.ReadUInt16();
            table.Version = m_reader.ReadUInt16();
            table.SegCountX2 = m_reader.ReadUInt16();
            table.SearchRange = m_reader.ReadUInt16();
            table.EntrySelector = m_reader.ReadUInt16();
            table.RangeShift = m_reader.ReadUInt16();

            int segCount = table.SegCountX2 / 2;

            table.EndCount = ReadUshortArray(segCount);
            table.ReservedPad = m_reader.ReadUInt16();
            table.StartCount = ReadUshortArray(segCount);
            table.IdDelta = ReadUshortArray(segCount);
            table.IdRangeOffset = ReadUshortArray(segCount);

            int length = (table.Length / 2 - 8) - (segCount * 4);

            table.GlyphID = ReadUshortArray(length);

            // Process glyphIdArray array.
            int codeOffset = 0;
            int index = 0;

            for (int j = 0; j < segCount; j++)
            {
                for (int k = table.StartCount[j], len = table.EndCount[j];
                    k <= len && k != UInt16.MaxValue; k++)
                {
                    if (table.IdRangeOffset[j] == 0)
                    {
                        codeOffset = (k + table.IdDelta[j]) & UInt16.MaxValue;
                    }
                    else
                    {
                        index = j + table.IdRangeOffset[j] / 2 - segCount + k - table.StartCount[j];

                        if (index >= table.GlyphID.Length) continue;

                        codeOffset = (table.GlyphID[index] + table.IdDelta[j]) & UInt16.MaxValue;
                    }

                    TtfGlyphInfo glyph = new TtfGlyphInfo();

                    glyph.Index = codeOffset;
                    glyph.Width = GetWidth(glyph.Index);

                    int id = (encoding == TtfCmapEncoding.Symbol) ?
                        ((k & 0xff00) == 0xf000 ? k & 0xff : k) : k;

                    glyph.CharCode = id;

                    collection[id] = glyph;
                    AddGlyph(glyph, encoding);
                }
            }
        }

        /// <summary>
        /// Reads Trimed cmap table.
        /// </summary>
        /// <param name="subTable">Cmap subtable.</param>
        /// <param name="encoding">Encoding ID.</param>
        private void ReadTrimmedCmapTable(TtfCmapSubTable subTable, TtfCmapEncoding encoding)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.cmap);

            m_reader.Seek(tableInfo.Offset + subTable.Offset);

            TtfTrimmedCmapSubTable table = new TtfTrimmedCmapSubTable();

            table.Format = m_reader.ReadUInt16();
            table.Length = m_reader.ReadUInt16();
            table.Version = m_reader.ReadUInt16();
            table.FirstCode = m_reader.ReadUInt16();
            table.EntryCount = m_reader.ReadUInt16();

            for (int i = 0, len = table.EntryCount; i < len; ++i)
            {
                TtfGlyphInfo glyphInfo = new TtfGlyphInfo();

                glyphInfo.Index = (int)m_reader.ReadUInt16();
                glyphInfo.Width = GetWidth(glyphInfo.Index);
                glyphInfo.CharCode = i + table.FirstCode;

                Macintosh[i] = glyphInfo;
                AddGlyph(glyphInfo, encoding);
                // NOTE: this code fixes char codes that extends 0x100. However, it might corrupt something.
                m_maxMacIndex = Math.Max(i, m_maxMacIndex);
            }
        }

        /// <summary>
        /// Reads loca table.
        /// </summary>
        /// <param name="bShort">If True - table is int16, int32 otherwise.</param>
        /// <returns>Loca table.</returns>
        private TtfLocaTable ReadLocaTable(bool bShort)
        {
            TtfTableInfo tableInfo = GetTable(TtfTableNames.loca);

            m_reader.Seek(tableInfo.Offset);

            TtfLocaTable table = new TtfLocaTable();
            uint[] buffer = null;

            if (bShort)
            {
                int len = tableInfo.Length / BigEndianReader.Int16Size;

                buffer = new uint[len];

                for (int i = 0; i < len; i++)
                {
                    buffer[i] = (uint)m_reader.ReadUInt16() * 2;
                }
            }
            else
            {
                int len = tableInfo.Length / BigEndianReader.Int32Size;

                buffer = new uint[len];

                for (int i = 0; i < len; i++)
                {
                    buffer[i] = m_reader.ReadUInt32();
                }
            }

            table.Offsets = buffer;

            return table;
        }

        /// <summary>
        /// Reads ushort array.
        /// </summary>
        /// <param name="len">Length of the array.</param>
        /// <returns>Ushort array.</returns>
        private ushort[] ReadUshortArray(int len)
        {
            ushort[] buffer = new ushort[len];

            for (int i = 0; i < len; i++)
            {
                buffer[i] = m_reader.ReadUInt16();
            }

            return buffer;
        }

        /// <summary>
        /// Reads uint array.
        /// </summary>
        /// <param name="len">Length of the array.</param>
        /// <returns>Uint array.</returns>
        private uint[] ReadUintArray(int len)
        {
            uint[] buffer = new uint[len];

            for (int i = 0; i < len; i++)
            {
                buffer[i] = m_reader.ReadUInt32();
            }

            return buffer;
        }

        /// <summary>
        /// Adds glyph to the collection.
        /// </summary>
        /// <param name="glyph">Glyph info.</param>
        /// <param name="encoding">Encoding ID.</param>
        private void AddGlyph(TtfGlyphInfo glyph, TtfCmapEncoding encoding)
        {
            Dictionary<int, TtfGlyphInfo> collection = null;

            switch (encoding)
            {
                case TtfCmapEncoding.Unicode:
                    collection = MicrosoftGlyphs;
                    break;

                case TtfCmapEncoding.Macintosh:
                case TtfCmapEncoding.Symbol:
                    collection = MacintoshGlyphs;
                    break;
            }

            collection[glyph.Index] = glyph;
        }

        /// <summary>
        /// Returns width of the glyph.
        /// </summary>
        /// <param name="glyphCode">Code of the glyph.</param>
        /// <returns>Returns width of the glyph.</returns>
        private int GetWidth(int glyphCode)
        {
            glyphCode = (glyphCode < m_width.Length) ? glyphCode : m_width.Length - 1;

            return m_width[glyphCode];
        }

        /// <summary>
        /// Updates chars structure which is used in the case
        /// of ansi encoding (256 bytes).
        /// </summary>
        private int[] UpdateWidth()
        {
            int count = Byte.MaxValue + 1;
            int[] bytes = new int[count];

            if (m_metrics.IsSymbol)
            {
                for (int i = 0; i < count; i++)
                {
                    TtfGlyphInfo glyphInfo = GetGlyph((char)i);

                    bytes[i] = (glyphInfo.Empty) ? 0 : glyphInfo.Width;
                }
            }
            else
            {
                byte[] byteToProcess = new byte[1];
                char unknown = '?';
                char space = (char)32;

                for (int i = 0; i < count; i++)
                {
                    byteToProcess[0] = (byte)i;
#if SILVERLIGHT
                    string text = new Windows1252Encoding().GetString(byteToProcess, 0, byteToProcess.Length);
#else
                    string text = Encoding.GetString(byteToProcess, 0, byteToProcess.Length);
#endif
                    char ch = (text.Length > 0) ? text[0] : unknown;

                    TtfGlyphInfo glyphInfo = GetGlyph(ch);

                    if (!glyphInfo.Empty)
                    {
                        bytes[i] = glyphInfo.Width;
                    }
                    else
                    {
                        glyphInfo = GetGlyph(space);
                        bytes[i] = (glyphInfo.Empty) ? 0 : glyphInfo.Width;
                    }
                }
            }

            return bytes;
        }

        /// <summary>
        /// Indicates whether code is right Ttf preambula code:
        /// </summary>
        private void CheckPreambula()
        {
            int version = m_reader.ReadInt32();

            if (version != c_ttfVersion1 && version != c_ttfVersion2)
                throw new PdfException("Can't read TTF font data");
        }

        /// <summary>
        /// Gets CMAP encoding based on platform ID and encoding ID.
        /// </summary>
        private TtfCmapEncoding GetCmapEncoding(int platformID, int encodingID)
        {
            TtfCmapEncoding format = TtfCmapEncoding.Unknown;

            if (platformID == (int)TtfPlatformID.Microsoft &&
                encodingID == (int)TtfMicrosoftEncodingID.Undefined)
            {
                // When building a symbol font for Windows,
                // the platform ID should be 3 and the encoding ID should be 0.
                format = TtfCmapEncoding.Symbol;
            }
            else if (platformID == (int)TtfPlatformID.Microsoft &&
                encodingID == (int)TtfMicrosoftEncodingID.Unicode)
            {
                // When building a Unicode font for Windows,
                // the platform ID should be 3 and the encoding ID should be 1.
                format = TtfCmapEncoding.Unicode;
            }
            else if (platformID == (int)TtfPlatformID.Macintosh &&
                encodingID == (int)TtfMacintoshEncodingID.Roman)
            {
                // When building a font that will be used on the Macintosh,
                // the platform ID should be 1 and the encoding ID should be 0.
                format = TtfCmapEncoding.Macintosh;
            }

            return format;
        }

        /// <summary>
        /// Returns table.
        /// </summary>
        /// <param name="name">Name of the table.</param>
        /// <returns>Returns table.</returns>
        private TtfTableInfo GetTable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            object obj = null;
            TtfTableInfo table = new TtfTableInfo();
            if (TableDirectory.ContainsKey(name))
            {
               obj = TableDirectory[name];
            }

            if (obj != null)
            {
                table = (TtfTableInfo)obj;
            }

            return table;
        }

        /// <summary>
        /// Updates hashtable of used glyphs.
        /// </summary>
        /// <param name="glyphChars">Dictionary of used glyphs.</param>
        /// <param name="locaTable">Loca table.</param>
        private void UpdateGlyphChars(Dictionary<int, int> glyphChars, TtfLocaTable locaTable)
        {
            if (glyphChars == null)
                throw new ArgumentNullException("glyphChars");

            // Add zero key.
            if (!glyphChars.ContainsKey(0))
            {
                glyphChars.Add(0, 0);
            }

            Dictionary<int, int> clone = new Dictionary<int, int>(glyphChars.Count);
            foreach (KeyValuePair<int, int> kvp in glyphChars)
            {
                clone.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> entry in clone)
            {
                int nextKey = (int)entry.Key;

                ProcessCompositeGlyph(glyphChars, nextKey, locaTable);
            }
        }

        /// <summary>
        /// Checks if glyph is composite or not.
        /// If True, it provides additional work.
        /// </summary>
        /// <param name="glyphChars">Dictionary of glyphs.</param>
        /// <param name="glyph">Glyph index.</param>
        /// <param name="locaTable">Loca table.</param>
        private void ProcessCompositeGlyph(Dictionary<int, int> glyphChars, int glyph, TtfLocaTable locaTable)
        {
            if (glyphChars == null)
                throw new ArgumentNullException("glyphChars");

            // Is in range.
            if (glyph < locaTable.Offsets.Length - 1)
            {
                uint glyphOffset = locaTable.Offsets[glyph];

                if (glyphOffset != locaTable.Offsets[glyph + 1])
                {
                    TtfTableInfo tableInfo = GetTable(TtfTableNames.glyf);

                    m_reader.Seek(tableInfo.Offset + glyphOffset);

                    TtfGlyphHeader glyphHeader = new TtfGlyphHeader();

                    glyphHeader.numberOfContours = m_reader.ReadInt16();
                    glyphHeader.XMin = m_reader.ReadInt16();
                    glyphHeader.YMin = m_reader.ReadInt16();
                    glyphHeader.XMax = m_reader.ReadInt16();
                    glyphHeader.YMax = m_reader.ReadInt16();

                    // Glyph is composite.
                    if (glyphHeader.numberOfContours < 0)
                    {
                        int skipBytes = 0;

                        while (true)
                        {
                            ushort flags = m_reader.ReadUInt16();
                            int glyphIndex = m_reader.ReadUInt16();

                            if (!glyphChars.ContainsKey(glyphIndex))
                            {
                                glyphChars.Add(glyphIndex, 0);
                            }

                            if ((flags & (ushort)TtfCompositeGlyphFlags.MORE_COMPONENTS) == 0) break;

                            skipBytes = ((flags & (ushort)TtfCompositeGlyphFlags.ARG_1_AND_2_ARE_WORDS) != 0) ?
                                BigEndianReader.Int32Size : BigEndianReader.Int16Size;

                            if ((flags & (ushort)TtfCompositeGlyphFlags.WE_HAVE_A_SCALE) != 0)
                            {
                                skipBytes += BigEndianReader.Int16Size;
                            }
                            else if ((flags & (ushort)TtfCompositeGlyphFlags.WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                            {
                                skipBytes += BigEndianReader.Int32Size;
                            }
                            else if ((flags & (ushort)TtfCompositeGlyphFlags.WE_HAVE_A_TWO_BY_TWO) != 0)
                            {
                                skipBytes += 2 * BigEndianReader.Int32Size;
                            }

                            m_reader.Skip(skipBytes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates new glyph tables based on chars that are used for output.
        /// </summary>
        /// <param name="glyphChars">dictionary of glyphs.</param>
        /// <param name="locaTable">Loca table.</param>
        /// <param name="newLocaTable">The updated table that stores the offsets to the
        /// locations of the glyphs in the font.</param>
        /// <param name="newGlyphTable">The updated glyph table that holds only the glyphs that
        /// are used for destination output.</param>
        /// <returns>Glyph table size.</returns>
        private uint GenerateGlyphTable(Dictionary<int, int> glyphChars, TtfLocaTable locaTable,
            out int[] newLocaTable, out byte[] newGlyphTable)
        {
            if (glyphChars == null)
                throw new ArgumentNullException("glyphChars");

            newLocaTable = new int[locaTable.Offsets.Length];

            // Sorting used glyphs keys.
            List<int> tGlyphs = new List<int>(glyphChars.Keys);
            int[] activeGlyphs = tGlyphs.ToArray();

            Array.Sort(activeGlyphs);

            uint glyphSize = 0;

            for (int i = 0, len = activeGlyphs.Length; i < len; i++)
            {
                int glyphIndex = activeGlyphs[i];
                if(locaTable.Offsets.Length>0)
                glyphSize += locaTable.Offsets[glyphIndex + 1] - locaTable.Offsets[glyphIndex];
            }

            uint glyphSizeAligned = Align(glyphSize);

            newGlyphTable = new byte[glyphSizeAligned];

            int nextGlyphOffset = 0;
            int nextGlyphIndex = 0;
            TtfTableInfo table = GetTable(TtfTableNames.glyf);

            // Creating NewLocaTable - that would hold offsets for filtered glyphs.
            for (int i = 0, len = newLocaTable.Length; i < len; i++)
            {
                newLocaTable[i] = nextGlyphOffset;

                if (nextGlyphIndex < activeGlyphs.Length && activeGlyphs[nextGlyphIndex] == i)
                {
                    ++nextGlyphIndex;

                    newLocaTable[i] = nextGlyphOffset;

                    int oldGlyphOffset = (int)locaTable.Offsets[i];
                    int oldNextGlyphOffset =
                        (int)locaTable.Offsets[i + 1] - oldGlyphOffset;

                    if (oldNextGlyphOffset > 0)
                    {
                        m_reader.Seek(table.Offset + oldGlyphOffset);
                        m_reader.Read(newGlyphTable, nextGlyphOffset, oldNextGlyphOffset);
                        nextGlyphOffset += oldNextGlyphOffset;
                    }
                }
            }

            return glyphSize;
        }

        /// <summary>
        /// Updates new Loca table.
        /// </summary>
        /// <param name="newLocaTable">New Loca table.</param>
        /// <param name="bLocaIsShort">Indicates whether loca is short.</param>
        /// <param name="newLocaTableOut">Updated new loca table.</param>
        /// <returns>Updated loca table size.</returns>
        private int UpdateLocaTable(int[] newLocaTable, bool bLocaIsShort, out byte[] newLocaTableOut)
        {
            if (newLocaTable == null)
                throw new ArgumentNullException("newLocaTable");

            int size = (bLocaIsShort) ? newLocaTable.Length * BigEndianReader.Int16Size :
                newLocaTable.Length * BigEndianReader.Int32Size;

            int count = (int)Align((uint)size);
            BigEndianWriter writer = new BigEndianWriter(count);

            newLocaTableOut = writer.Data;

            for (int i = 0; i < newLocaTable.Length; i++)
            {
                int value = newLocaTable[i];

                if (bLocaIsShort)
                {
                    value /= 2;
                    writer.Write((short)value);
                }
                else
                {
                    writer.Write(value);
                }
            }

            return size;
        }

        /// <summary>
        /// Returns font program data.
        /// </summary>
        /// <param name="newLocaTableOut">New updated loca table.</param>
        /// <param name="newGlyphTable">New glyph table.</param>
        /// <param name="glyphTableSize">Size of glyph table.</param>
        /// <param name="locaTableSize">Size of loca table.</param>
        /// <returns>Font program data.</returns>
        private byte[] GetFontProgram(byte[] newLocaTableOut, byte[] newGlyphTable,
            uint glyphTableSize, uint locaTableSize)
        {
            if (newLocaTableOut == null)
                throw new ArgumentNullException("newLocaTableOut");

            if (newGlyphTable == null)
                throw new ArgumentNullException("newGlyphTable");

            string[] tableNames = TableNames;
            short numTables = 0;
            int fontProgramLength = GetFontProgramLength(newLocaTableOut, newGlyphTable, out numTables);
            BigEndianWriter writer = new BigEndianWriter(fontProgramLength);

            writer.Write(c_ttfVersion1);
            writer.Write(numTables);

            short entrySelector = s_entrySelectors[numTables];

            writer.Write((short)((1 << (entrySelector & 31)) * c_fp));
            writer.Write(entrySelector);
            writer.Write((short)((numTables - (1 << (entrySelector & 31))) * c_fp));

            // Writing to destination buffer - checksums && sizes of used tables.
            WriteCheckSums(writer, numTables, newLocaTableOut, newGlyphTable, glyphTableSize, locaTableSize);

            // Writing to destination buffer - used glyphs.
            WriteGlyphs(writer, newLocaTableOut, newGlyphTable);

            return writer.Data;
        }

        /// <summary>
        /// Calculate size of the font program.
        /// </summary>
        /// <param name="newLocaTableOut">Updated new loca table.</param>
        /// <param name="newGlyphTable">New glyph table.</param>
        /// <param name="numTables">Number of tables used.</param>
        /// <returns>Size of the font program.</returns>
        private int GetFontProgramLength(byte[] newLocaTableOut, byte[] newGlyphTable, out short numTables)
        {
            if (newLocaTableOut == null)
                throw new ArgumentNullException("newLocaTableOut");

            if (newGlyphTable == null)
                throw new ArgumentNullException("newGlyphTable");

            // glyf and loca are used by default;
            numTables = 2;

            string[] tableNames = TableNames;
            int fontProgramLength = 0;

            for (int i = 0, len = tableNames.Length; i < len; i++)
            {
                string tableName = tableNames[i];

                if (tableName != TtfTableNames.glyf && tableName != TtfTableNames.loca)
                {
                    TtfTableInfo table = GetTable(tableName);

                    if (!table.Empty)
                    {
                        ++numTables;
                        fontProgramLength += (int)Align((uint)table.Length);
                    }
                }
            }

            fontProgramLength += newLocaTableOut.Length;
            fontProgramLength += newGlyphTable.Length;

            int usedTablesSize = numTables * c_fp + (3 * BigEndianReader.Int32Size);

            fontProgramLength += usedTablesSize;

            return fontProgramLength;
        }

        /// <summary>
        /// Gets checksum from source buffer.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        private int CalculateCheckSum(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            int pos = 0;
            int byte1 = 0;
            int byte2 = 0;
            int byte3 = 0;
            int byte4 = 0;

            for (int i = 0, len = (bytes.Length + 1) / 4; i < len; i++)
            {
                byte4 += (bytes[pos++] & 255);
                byte3 += (bytes[pos++] & 255);
                byte2 += (bytes[pos++] & 255);
                byte1 += (bytes[pos++] & 255);
            }

            int result = byte1;

            result += (byte2 << 8);
            result += (byte3 << 16);
            result += (byte4 << 24);

            return result;
        }

        /// <summary>
        /// Writing to destination buffer - checksums and sizes of used tables.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="numTables">Number of tables.</param>
        /// <param name="newLocaTableOut">New updated loca table.</param>
        /// <param name="newGlyphTable">New glyph table.</param>
        /// <param name="glyphTableSize">Size of glyph table.</param>
        /// <param name="locaTableSize">Size of loca table.</param>
        private void WriteCheckSums(BigEndianWriter writer, short numTables,
            byte[] newLocaTableOut, byte[] newGlyphTable, uint glyphTableSize, uint locaTableSize)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (newLocaTableOut == null)
                throw new ArgumentNullException("newLocaTableOut");

            if (newGlyphTable == null)
                throw new ArgumentNullException("newGlyphTable");

            string[] tableNames = TableNames;
            uint usedTablesSize = (uint)(numTables * c_fp + (3 * BigEndianReader.Int32Size));
            uint nextTableSize = 0;

            for (int i = 0, len = tableNames.Length; i < len; i++)
            {
                string tableName = tableNames[i];
                TtfTableInfo tableInfo = GetTable(tableName);

                if (tableInfo.Empty) continue;

                writer.Write(tableName);

                if (tableName == TtfTableNames.glyf)
                {
                    int checksum = CalculateCheckSum(newGlyphTable);
                    writer.Write(checksum);
                    nextTableSize = glyphTableSize;
                }
                else if (tableName == TtfTableNames.loca)
                {
                    int checksum = CalculateCheckSum(newLocaTableOut);
                    writer.Write(checksum);
                    nextTableSize = locaTableSize;
                }
                else
                {
                    writer.Write(tableInfo.Checksum);
                    nextTableSize = (uint)tableInfo.Length;
                }

                writer.Write(usedTablesSize);
                writer.Write(nextTableSize);

                usedTablesSize += Align(nextTableSize);
            }
        }

        /// <summary>
        /// // Writing to destination buffer - used glyphs.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="newLocaTableOut">New updated loca table.</param>
        /// <param name="newGlyphTable">New glyph table.</param>
        private void WriteGlyphs(BigEndianWriter writer, byte[] newLocaTableOut, byte[] newGlyphTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (newLocaTableOut == null)
                throw new ArgumentNullException("newLocaTableOut");

            if (newGlyphTable == null)
                throw new ArgumentNullException("newGlyphTable");

            string[] tableNames = TableNames;

            for (int i = 0, len = tableNames.Length; i < len; i++)
            {
                string tableName = tableNames[i];
                TtfTableInfo tableInfo = GetTable(tableName);

                if (tableInfo.Empty) continue;

                if (tableName == TtfTableNames.glyf)
                {
                    writer.Write(newGlyphTable);
                }
                else if (tableName == TtfTableNames.loca)
                {
                    writer.Write(newLocaTableOut);
                }
                else
                {
                    int count = (int)Align((uint)tableInfo.Length);
                    byte[] buff = new byte[count];

                    m_reader.Seek(tableInfo.Offset);
                    m_reader.Read(buff, 0, tableInfo.Length);
                    writer.Write(buff);
                }
            }
        }

        /// <summary>
        /// Initializes font name.
        /// </summary>
        /// <param name="nameTable">Name table.</param>
        private void InitializeFontName(TtfNameTable nameTable)
        {
            // Get FontFamily and font Name.
            for (int i = 0; i < nameTable.RecordsCount; i++)
            {
                TtfNameRecord record = nameTable.NameRecords[i];

                if ((int)record.NameID == (int)TtfNameID.FontFamily)
                {
                    m_metrics.FontFamily = record.Name;
                }
                else if ((int)record.NameID == (int)TtfNameID.PostScriptName)
                {
                    m_metrics.PostScriptName = record.Name;
                }
                if (m_metrics.FontFamily != null && m_metrics.PostScriptName != null)
                {
                    break;
                }
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Reads structure from the binary reader.
        /// </summary>
        /// <param name="reader">Binary reader class.</param>
        /// <param name="type">Type of teh structure.</param>
        /// <returns>Structure object.</returns>
        private ValueType ReadStructure(BinaryReader reader, Type type)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (type == null)
                throw new ArgumentNullException("type");

            int size = Marshal.SizeOf(type);
            byte[] buffer = reader.ReadBytes(size);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, 0, ptr, size);
            ValueType structure = (ValueType)Marshal.PtrToStructure(ptr, type);
            Marshal.FreeHGlobal(ptr);

            return structure;
        }
#endif
        /// <summary>
        /// Aligns number to be divisible on 4.
        /// </summary>
        private uint Align(uint value)
        {
            return (uint)((value + 3) & (~3));
        }
        /// <summary>
        /// Returns default glyph.
        /// </summary>
        /// <returns>Returns default glyph.</returns>
        private TtfGlyphInfo GetDefaultGlyph()
        {
            TtfGlyphInfo glyph = GetGlyph(StringTokenizer.WhiteSpace);
            return glyph;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the font data.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private byte[] GetFontData(Font font, uint tableName)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            IntPtr hDC = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);
            IntPtr hFont = font.ToHfont();
            IntPtr oldObj = GdiApi.SelectObject(hDC, hFont);
            uint numBytes = GdiApi.GetFontData(hDC, tableName, 0, null, 0);

            if (numBytes == WinGdiConst.GDI_ERROR)
            {
                uint code = KernelApi.GetLastError();
                Debug.WriteLine("Can't create font, error code: " + code);

                throw new PdfException("Can't parse the font");
            }

            byte[] buff = new byte[numBytes];
            GdiApi.GetFontData(hDC, tableName, 0, buff, numBytes);
            GdiApi.SelectObject(hDC, oldObj);
            GdiApi.DeleteObject(hFont);
            GdiApi.DeleteDC(hFont);
            GdiApi.DeleteDC(hDC);

            return buff;
        }


        /// <summary>
        /// Checks for the additional offset of the tables.
        /// </summary>
        /// <param name="table">TTF table.</param>
        /// <param name="name">Name of the table.</param>
        /// <returns>Additional ofset of the tables.</returns>
        /// <param name="reader">Reader of the data.</param>
        private int NormalizeOffset(TtfTableInfo table, string name, BigEndianReader reader)
        {

            if (name == null)
                throw new ArgumentNullException("name");
            if (reader == null)
                throw new ArgumentNullException("reader");

            int offset = 0;

            if (Font != null)
            {
                uint tableCode = FormatTableName(name);
                byte[] buff = GetFontData(Font, tableCode);

                if (buff != null)
                {
                    int pos = (int)reader.BaseStream.Position;
                    int curPos = table.Offset;

                    for (int i = curPos; i >= 0; i = i - 4)
                    {
                        reader.BaseStream.Position = i;
                        byte[] possTableBuff = reader.ReadBytes(table.Length);

                        if (CompareArrays(buff, possTableBuff))
                        {
                            offset = i - table.Offset;
                            break;
                        }
                    }

                    reader.BaseStream.Position = pos;
                }
            }

            return offset;
        }
#endif
        /// <summary>
        /// Checks whether two arrays are equal.
        /// </summary>
        /// <param name="buff1">The first array.</param>
        /// <param name="buff2">The second array.</param>
        /// <returns>True if arrays are equal.</returns>
        private bool CompareArrays(byte[] buff1, byte[] buff2)
        {

            bool bEqual = false;
            if (buff1.Length == buff2.Length)
            {
                int i = 0;
                while ((i < buff2.Length) && (buff2[i] == buff1[i]))
                {
                    i += 1;
                }
                if (i == buff2.Length)
                {
                    bEqual = true;
                }
            }
            return bEqual;
        }

        /// <summary>
        /// Formats name of the table to int value.
        /// </summary>
        /// <param name="name">Name of the table.</param>
        /// <returns>Code of the table name.</returns>
        private uint FormatTableName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            byte[] buff = Encoding.UTF8.GetBytes(name);
            uint res = BitConverter.ToUInt32(buff, 0);
            uint result = (uint)((((buff[3] << 0x18) |
              (buff[2] << 0x10)) |
              (buff[1] << 8)) | buff[0]);

            return result;
        }
        #endregion

    }
}
