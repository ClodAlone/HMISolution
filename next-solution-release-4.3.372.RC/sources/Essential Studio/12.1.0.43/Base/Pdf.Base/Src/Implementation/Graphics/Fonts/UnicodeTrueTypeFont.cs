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
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// Creator of Unicode TrueType type font.
    /// </summary>
    internal class UnicodeTrueTypeFont :
        ITrueTypeFont
    {
        #region Constants
        /// <summary>
        /// Display driver name.
        /// </summary>
        private const string c_driverName = "DISPLAY";
        /// <summary>
        /// String for generating font name.
        /// </summary>
        private const string c_nameString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// Cmap table's start prefix.
        /// </summary>
        private const string c_cmapPrefix =
            "/CIDInit /ProcSet findresource begin\n12 dict begin\nbegincmap" + Operators.NewLine +
            "/CIDSystemInfo << /Registry (Adobe)/Ordering (UCS)/Supplement 0>> def\n/CMapName " +
            "/Adobe-Identity-UCS def\n/CMapType 2 def\n1 begincodespacerange" + Operators.NewLine;
        /// <summary>
        /// Cmap table's start suffix.
        /// </summary>
        private const string c_cmapEndCodespaceRange = "endcodespacerange" + Operators.NewLine;
        /// <summary>
        /// Cmap table's end
        /// </summary>
        private const string c_cmapSuffix = "endbfrange\nendcmap\nCMapName currentdict " +
            "/CMap defineresource pop\nend end" + Operators.NewLine;
        /// <summary>
        /// Cmap's begin range marker.
        /// </summary>
        private const string c_cmapBeginRange = "beginbfrange" + Operators.NewLine;
        /// <summary>
        /// Cmap's end range marker.
        /// </summary>
        private const string c_cmapEndRange = "endbfrange" + Operators.NewLine;
        /// <summary>
        /// Cmap's next range default value
        /// </summary>
        private const int c_cmapNextRangeValue = 100;
        /// <summary>
        /// Default registry's value
        /// </summary>
        private const string c_registry = "Adobe";
        /// <summary>
        /// Index of the default symbol.
        /// </summary>
        private const int c_defWidthIndex = 32;
        /// <summary>
        /// Length of Cid Stream
        /// </summary>
        private const int c_cidStreamLength = 11;
        #endregion

        #region Fields
        private static object s_syncLock = new object();
        /// <summary>
        /// 
        /// </summary>
        private Stream m_fontStream;
        /// <summary>
        /// System font.
        /// </summary>
#if !SILVERLIGHT && !NETFX_CORE && !WP
        private Font m_font;
#endif
        /// <summary>
        /// Path to ttf file.
        /// </summary>
        private string m_filePath;
        /// <summary>
        /// Size of the font.
        /// </summary>
        private float m_size;
        /// <summary>
        /// Font metrics.
        /// </summary>
        private PdfFontMetrics m_metrics;
        /// <summary>
        /// Pdf primitive describing the font.
        /// </summary>
        private PdfDictionary m_fontDictionary;
        /// <summary>
        /// Descendant font.
        /// </summary>
        private PdfDictionary m_descendantFont;
        /// <summary>
        /// Font program.
        /// </summary>
        private PdfStream m_fontProgram;
        /// <summary>
        /// Cmap stream.
        /// </summary>
        private PdfStream m_cmap;
        /// <summary>
        /// ttf reader object.
        /// </summary>
        private TtfReader m_ttfReader;
        /// <summary>
        /// Array of used chars.
        /// </summary>
        private Dictionary<char, char> m_usedChars;
        /// <summary>
        /// Name of the font subset.
        /// </summary>
        private string m_subsetName;
        /// <summary>
        /// Ttf metrics structure.
        /// </summary>
        internal TtfMetrics m_ttfMetrics;
        /// <summary>
        /// Specifies the composite font types.
        /// </summary>
        private CompositeFontType m_type;
        #endregion

        #region Properties
        /// <summary>
        /// Gets size of the font.
        /// </summary>
        float ITrueTypeFont.Size
        {
            get
            {
                return m_size;
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets system font.
        /// </summary>
        Font ITrueTypeFont.Font
        {
            get
            {
                return m_font;
            }
        }
#endif

        /// <summary>
        /// Gets font metrics.
        /// </summary>
        PdfFontMetrics ITrueTypeFont.Metrics
        {
            get
            {
                return m_metrics;
            }
        }

        /// <summary>
        /// Gets ttf reader.
        /// </summary>
        internal TtfReader TtfReader
        {
            get
            {
                return m_ttfReader;
            }
        }

        /// <summary>
        /// Gets path to the font file if the font was created from a file.
        /// </summary>
        internal string FontFile
        {
            get
            {
                return m_filePath;
            }
        }
        /// <summary>
        /// Gets TtfMetrics structure.
        /// </summary>
        internal TtfMetrics TtfMetrics
        {
            get
            {
                return m_ttfMetrics;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        internal CompositeFontType FontType
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        #endregion

        #region Constructors
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// creates a new object.
        /// </summary>
        /// <param name="font">Font object.</param>
        /// <param name="size">Font size.</param>
        public UnicodeTrueTypeFont(Font font, float size, CompositeFontType type)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_font = font;
            m_size = size;
            m_type = type;

            Initialize();
        }

        /// <summary>
        /// creates a new object.
        /// </summary>
        /// <param name="filePath">Path to ttf file.</param>
        /// <param name="size">Font size.</param>
        public UnicodeTrueTypeFont(string filePath, float size, CompositeFontType type)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            if (filePath.Length == 0)
                throw new ArgumentException("filePath - string can not be empty");

            m_filePath = filePath;
            m_size = size;
            m_type = type;

            Initialize();
        }
#endif
        /// <summary>
        /// creates a new object.
        /// </summary>
        /// <param name="font">Font object.</param>
        /// <param name="size">Font size.</param>
        public UnicodeTrueTypeFont(Stream font, float size, CompositeFontType type)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_fontStream = font;
            m_size = size;
            m_type = type;
            byte[] buffer = new byte[font.Length];
            font.Read(buffer, 0, buffer.Length);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Creates a new object from a prototype object.
        /// </summary>
        /// <param name="prototype">Prototype object.</param>
        public UnicodeTrueTypeFont(UnicodeTrueTypeFont prototype)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");

            m_ttfMetrics = prototype.TtfMetrics;
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_font = ((ITrueTypeFont)prototype).Font;
#endif
            m_filePath = prototype.FontFile;
            m_size = ((ITrueTypeFont)prototype).Size;
        }


        #endregion

        #region Public methods
        /// <summary>
        /// Stores used symbols.
        /// </summary>
        /// <param name="text">String text.</param>
        public void SetSymbols(string text)
        {
            lock (PdfDocument.Cache)
            {
                if (text == null)
                    throw new ArgumentNullException("text");

                if (m_usedChars == null)
                {
                    m_usedChars = new Dictionary<char, char>();
                }

                for (int i = 0; i < text.Length; i++)
                {
                    char ch = text[i];

                    m_usedChars[ch] = char.MinValue;
                }

                GetDescendantWidth();
            }
        }

        /// <summary>
        /// Stores used symbols.
        /// </summary>
        /// <param name="glyphs">Glyphs, used by the line of the text.</param>
        public void SetSymbols(UInt16[] glyphs)
        {
            if (glyphs == null)
                throw new ArgumentNullException("glyphs");

            if (m_usedChars == null)
            {
                m_usedChars = new Dictionary<char, char>();
            }

            for (int i = 0; i < glyphs.Length; i++)
            {
                int glyphIndex = glyphs[i];
                TtfGlyphInfo glyph = m_ttfReader.GetGlyph(glyphIndex);

                if (!glyph.Empty)
                {
                    char ch = (char)glyph.CharCode;

                    m_usedChars[ch] = char.MinValue;
                }
            }
            GetDescendantWidth();
        }
        #endregion

        #region ITrueTypeFont implementation
        /// <summary>
        /// Gets Pdf primitive reprsenting font.
        /// </summary>
        IPdfPrimitive ITrueTypeFont.GetInternals()
        {
            return m_fontDictionary;
        }

        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>True if fonts are equal, False otherwise.</returns>
        bool ITrueTypeFont.EqualsToFont(PdfFont font)
        {
            bool equal = false;
            PdfTrueTypeFont trueTypeFont = font as PdfTrueTypeFont;

            if (trueTypeFont != null && trueTypeFont.Unicode)
            {
                bool equalName = false;
                bool equalStyle = false;

#if !SILVERLIGHT && !NETFX_CORE && !WP
                if (m_font != null && trueTypeFont.InternalFont.Font != null)
                {
                    equalName = (m_font.Name.Equals(trueTypeFont.InternalFont.Font.Name));
                    equalStyle = (m_font.Style == trueTypeFont.InternalFont.Font.Style);
                }
                else
#endif
                {
                    UnicodeTrueTypeFont internalFont = (UnicodeTrueTypeFont)trueTypeFont.InternalFont;

                    equalName = (m_ttfMetrics.FontFamily.Equals(internalFont.m_ttfMetrics.FontFamily));
                    equalStyle = (m_ttfMetrics.MacStyle == internalFont.m_ttfMetrics.MacStyle);
                }

                equal = (equalName && equalStyle);
            }

            return equal;
        }

        /// <summary>
        /// Creates font internals.
        /// </summary>
        void ITrueTypeFont.CreateInternals()
        {
            m_fontDictionary = new PdfDictionary();
            m_fontProgram = new PdfStream();
            m_cmap = new PdfStream();
            m_descendantFont = new PdfDictionary();
            m_metrics = new PdfFontMetrics();

            // We keep reader until Font is disposed.
            // In this case we can dispose Font or don't care about font file.
#if SILVERLIGHT || NETFX_CORE || WP
            m_ttfReader.Reader = new BinaryReader(m_fontStream, TtfReader.Encoding);
#else
            m_ttfReader.Reader = GetFontData();
#endif
            m_ttfReader.CreateInternals();
            m_ttfMetrics = m_ttfReader.Metrics;
            InitializeMetrics();

            // Create all the dictionaries of the font.
            m_subsetName = GetFontName();
            CreateDescendantFont();
            CreateCmap();
            CreateFontDictionary();
            CreateFontProgram();
        }

        /// <summary>
        /// Returns width of the char symbol.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <returns>Width of the char symbol in universal units.</returns>
        int ITrueTypeFont.GetCharWidth(char charCode)
        {
            int codeWidth = m_ttfReader.GetCharWidth(charCode);

            return codeWidth;
        }

        /// <summary>
        /// Returns width of the text line.
        /// </summary>
        /// <param name="line">String line.</param>
        /// <returns>Width of the char symbol in universal units.</returns>
        int ITrueTypeFont.GetLineWidth(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            int width = 0;

            for (int i = 0, len = line.Length; i < len; i++)
            {
                char ch = line[i];
                int charWidth = ((ITrueTypeFont)this).GetCharWidth(ch);

                width += charWidth;
            }

            return width;
        }

        /// <summary>
        /// All resources are being to be closed.
        /// </summary>
        void ITrueTypeFont.Close()
        {
            if (m_fontDictionary != null)
            {
                m_fontDictionary.Clear();
                m_fontDictionary = null;
            }

            if (m_descendantFont != null)
            {
                m_descendantFont.Clear();
                m_descendantFont = null;
            }

            if (m_fontProgram != null)
            {
                m_fontProgram.Clear();
                m_fontProgram = null;
            }

            if (m_cmap != null)
            {
                m_cmap.Clear();
                m_cmap = null;
            }

            if (m_ttfReader != null)
            {
                m_ttfReader.Close();
                m_ttfReader = null;
            }

            if (m_usedChars != null)
            {
                m_usedChars.Clear();
                m_usedChars = null;
            }
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_font = null;
#endif
            m_filePath = null;
            m_metrics = null;
            m_subsetName = null;
        }
        #endregion

        #region Implementation

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Initialize()
        {
            // We should dispose reader after each operation.
            using (BinaryReader reader = GetFontData())
            {
                m_ttfReader = new TtfReader(reader, m_font);
                m_ttfMetrics = m_ttfReader.Metrics;
            }
        }

        /// <summary>
        /// Returns binary reader of the font's data.
        /// </summary>
        /// <returns>Returns binary reader of the font's data.</returns>
        private BinaryReader GetFontData()
        {
            Stream stream = null;

            if (m_font != null)
            {
                stream = GetFontData(m_font);
            }
            else if (m_fontStream != null)
            {
                stream = m_fontStream;
                if (stream.CanRead)
                    stream.Position = 0;
            }
            else
            {
                try
                {
                    stream = new FileStream(m_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");
                    throw new Exception("Cannot open file: " + m_filePath + " for reading.");
                }
            }

            BinaryReader reader = new BinaryReader(stream, TtfReader.Encoding);
            return reader;
        }

        /// <summary>
        /// Gets binary data of font.
        /// </summary>
        /// <param name="font">Font object.</param>
        /// <returns>Bimary data from font file.</returns>
        private Stream GetFontData(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (PdfDocument.EnableCache)
            {
                if (PdfDocument.Cache.FontData.ContainsKey(font))
                {
                    byte[] buffer = PdfDocument.Cache.FontData[font];
                    return new MemoryStream(buffer);
                }
            }

            IntPtr hDC = GdiApi.CreateDC(c_driverName, null, null, IntPtr.Zero);
            IntPtr hFont = font.ToHfont();
            IntPtr oldObj = GdiApi.SelectObject(hDC, hFont);
            uint numBytes = GdiApi.GetFontData(hDC, 0, 0, null, 0);

            if (numBytes == WinGdiConst.GDI_ERROR)
            {
                uint code = KernelApi.GetLastError();
                Debug.WriteLine("Can't create font, error code: " + code);

                throw new PdfException("Can't parse the font");
            }

            byte[] buff = new byte[numBytes];
            numBytes = GdiApi.GetFontData(hDC, 0, 0, buff, numBytes);

            // Debug purposes.
            //uint name = 0x70616D63; //0x636D6170;//'cmap';
            //uint num = GdiApi.GetFontData( hDC, name, 0, null, 0 );
            //byte[] cmap = new byte[ num ];
            //num = GdiApi.GetFontData( hDC, name, 0, cmap, num );

            //FileStream f = new FileStream("c:/temp/fontdata.dat", FileMode.Create);
            //f.Write(buff, 0, buff.Length);
            //f.Close();

            if (numBytes == WinGdiConst.GDI_ERROR)
            {
                uint code = KernelApi.GetLastError();
                Debug.WriteLine("Can't create font, error code: " + code);

                throw new PdfException("Can't parse the font");
            }
            GdiApi.SelectObject(hDC, oldObj);
            GdiApi.DeleteObject(hFont);
            GdiApi.DeleteDC(hDC);

            if (PdfDocument.EnableCache)
            {
                lock (PdfDocument.Cache)
                {
                    if (!PdfDocument.Cache.FontData.ContainsKey(font))
                    {
                        //Add font data to cache.
                        PdfDocument.Cache.FontData.Add(font, buff);
                    }
                }
            }
            return new MemoryStream(buff, 0, buff.Length, false);
        }
#endif
        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Initialize(Stream font)
        {
            // We should dispose reader after each operation.
            using (BinaryReader reader = new BinaryReader(font, TtfReader.Encoding))
            {
                m_ttfReader = new TtfReader(reader);
                m_ttfMetrics = m_ttfReader.Metrics;
            }
        }

        /// <summary>
        /// Initializes metrics.
        /// </summary>
        private void InitializeMetrics()
        {
            TtfMetrics ttfMetrics = m_ttfReader.Metrics;

            m_metrics.Ascent = ttfMetrics.MacAscent;
            m_metrics.Descent = ttfMetrics.MacDescent;
            m_metrics.Height = ttfMetrics.MacAscent - ttfMetrics.MacDescent + ttfMetrics.LineGap;
            m_metrics.Name = ttfMetrics.FontFamily;
            m_metrics.PostScriptName = ttfMetrics.PostScriptName;
            m_metrics.Size = m_size;
            m_metrics.WidthTable = new StandardWidthTable(ttfMetrics.WidthTable);
            m_metrics.LineGap = ttfMetrics.LineGap;
            m_metrics.SubScriptSizeFactor = ttfMetrics.SubScriptSizeFactor;
            m_metrics.SuperscriptSizeFactor = ttfMetrics.SuperscriptSizeFactor;
        }

        /// <summary>
        /// Creates font program.
        /// </summary>
        /// <returns>Font program.</returns>
        private void CreateFontProgram()
        {
            m_fontProgram.BeginSave += new SavePdfPrimitiveEventHandler(FontProgramBeginSave);
        }

        /// <summary>
        /// Generates font program.
        /// </summary>
        private void GenerateFontProgram()
        {
            byte[] fontProgram = null;
            m_usedChars = (m_usedChars == null) ? new Dictionary<char, char>() : m_usedChars;
            m_ttfReader.InternalReader.Seek(0);

#if SILVERLIGHT || NETFX_CORE || WP
            if (m_type == CompositeFontType.Type0)
            {

#else
            if (m_type == CompositeFontType.Type0 && m_ttfReader.Font != null)
            {
#endif
                fontProgram = m_ttfReader.ReadFontProgram(m_usedChars);
            }
            else
            {

#if SILVERLIGHT || NETFX_CORE || WP
                Stream fontStream = m_fontStream;
#else
                Stream fontStream = GetFontData().BaseStream;
#endif
                fontProgram = new byte[fontStream.Length];
                m_fontProgram["Length1"] = new PdfNumber(fontProgram.Length);
                fontStream.Read(fontProgram, 0, (int)fontStream.Length - 1);
                fontStream.Dispose();
            }

            m_fontProgram.Clear();
            m_fontProgram.Write(fontProgram);
        }

        /// <summary>
        /// Generates font dictionary.
        /// </summary>
        private void CreateFontDictionary()
        {
            m_fontDictionary.BeginSave += new SavePdfPrimitiveEventHandler(FontDictionaryBeginSave);

            m_fontDictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Font);
            m_fontDictionary[DictionaryProperties.BaseFont] = new PdfName(m_subsetName);

            if (m_type == CompositeFontType.Type0)
            {
                m_fontDictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.Type0);
                m_fontDictionary[DictionaryProperties.Encoding] = new PdfName(DictionaryProperties.IdentityH);

                PdfArray descFonts = new PdfArray();
                PdfReferenceHolder reference = new PdfReferenceHolder(m_descendantFont);

                descFonts.Add(reference);
                m_fontDictionary[DictionaryProperties.DescendantFonts] = descFonts;
            }
            else
            {
                m_fontDictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.TrueType);
                m_fontDictionary[DictionaryProperties.Encoding] = new PdfName(DictionaryProperties.WinAnsiEncoding);
                m_fontDictionary[DictionaryProperties.Widths] = new PdfArray(m_ttfMetrics.WidthTable);
                m_fontDictionary[DictionaryProperties.FirstChar] = new PdfNumber(0);
                m_fontDictionary[DictionaryProperties.LastChar] = new PdfNumber(255);
                IPdfPrimitive fontDescriptor = CreateFontDescriptor();
                m_fontDictionary[DictionaryProperties.FontDescriptor] = new PdfReferenceHolder(fontDescriptor);
            }
        }

        /// <summary>
        /// Creates descendant font.
        /// </summary>
        private void CreateDescendantFont()
        {
            m_descendantFont.BeginSave += new SavePdfPrimitiveEventHandler(DescendantFontBeginSave);

            m_descendantFont[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Font);
            m_descendantFont[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.CIDFontType2);
            m_descendantFont[DictionaryProperties.BaseFont] = new PdfName(m_subsetName);
            m_descendantFont[DictionaryProperties.CIDToGIDMap] = new PdfName(DictionaryProperties.Identity);
            m_descendantFont[DictionaryProperties.DW] = new PdfNumber(TtfReader.WidthMultiplier);

            IPdfPrimitive fontDescriptor = CreateFontDescriptor();

            Byte[] cidBytes = new byte[c_cidStreamLength];
            PdfStream cidStream = new PdfStream();
            cidStream.Write(cidBytes);
            cidStream.SetProperty(DictionaryProperties.Filter, new PdfName(DictionaryProperties.FlateDecode));
            (fontDescriptor as PdfDictionary).Items.Add(new PdfName(DictionaryProperties.CidSet), new PdfReferenceHolder(cidStream));


            m_descendantFont[DictionaryProperties.FontDescriptor] = new PdfReferenceHolder(fontDescriptor);

            IPdfPrimitive systemInfo = CreateSystemInfo();

            m_descendantFont[DictionaryProperties.CIDSystemInfo] = systemInfo;
        }

        /// <summary>
        /// Creates cmap.
        /// </summary>
        private void CreateCmap()
        {
            m_cmap.BeginSave += new SavePdfPrimitiveEventHandler(CmapBeginSave);
        }

        /// <summary>
        /// Creates cmap.
        /// </summary>
        private void GenerateCmap()
        {
            if (m_usedChars != null && m_usedChars.Count > 0)
            {
                Dictionary<int, int> glyphChars = m_ttfReader.GetGlyphChars(m_usedChars);

                if (glyphChars.Count > 0)
                {
                    int[] keys = new int[glyphChars.Count];
                    glyphChars.Keys.CopyTo(keys, 0);
                    Array.Sort(keys);

                    // Sort glyph indexes to get first and last.
                    List<int> glyphIndices = new List<int>(glyphChars.Keys.Count);

                    glyphIndices.AddRange(glyphChars.Keys);

                    Array sortedArray = glyphIndices.ToArray();
                    Array.Sort(sortedArray);

                    // add first and last glyph indexes
                    int first = keys[0];
                    int last = keys[keys.Length - 1];
                    string middlePart = ToHexString(first) + ToHexString(last) + Operators.NewLine;

                    StringBuilder builder = new StringBuilder();

                    builder.Append(c_cmapPrefix);
                    builder.Append(middlePart);
                    builder.Append(c_cmapEndCodespaceRange);

                    int nextRange = 0;

                    for (int i = 0, len = keys.Length; i < len; i++)
                    {
                        if (nextRange == 0)
                        {
                            if (i != 0)
                            {
                                builder.Append(c_cmapEndRange);
                            }
                            nextRange = Math.Min(c_cmapNextRangeValue, keys.Length - i);
                            builder.Append(nextRange);
                            builder.Append(Operators.WhiteSpace);
                            builder.Append(c_cmapBeginRange);
                        }
                        nextRange -= 1;

                        int key = keys[i];

                        builder.AppendFormat("<{0:X04}><{0:X04}><{1:X04}>\n", key, (int)glyphChars[key]);
                    }

                    builder.Append(c_cmapSuffix);

                    m_cmap.Clear();
                    m_cmap.Write(builder.ToString());
                }
            }
        }

        /// <summary>
        /// Creates system info dictionary for CID font.
        /// </summary>
        /// <returns>Pdf primitive.</returns>
        private IPdfPrimitive CreateSystemInfo()
        {
            PdfDictionary systemInfo = new PdfDictionary();

            systemInfo[DictionaryProperties.Registry] = new PdfString(c_registry);
            systemInfo[DictionaryProperties.Ordering] = new PdfString(DictionaryProperties.Identity);
            systemInfo[DictionaryProperties.Supplement] = new PdfNumber(0);

            return systemInfo;
        }

        /// <summary>
        /// Creates font descriptor.
        /// </summary>
        /// <returns>Pdf primitive.</returns>
        private IPdfPrimitive CreateFontDescriptor()
        {
            PdfDictionary descriptor = new PdfDictionary();
            TtfMetrics metrics = m_ttfReader.Metrics;

            descriptor[DictionaryProperties.Type] = new PdfName(DictionaryProperties.FontDescriptor);
            descriptor[DictionaryProperties.FontName] = new PdfName(m_subsetName);
            descriptor[DictionaryProperties.Flags] = new PdfNumber(GetDescriptorFlags());
            descriptor[DictionaryProperties.FontBBox] = PdfArray.FromRectangle(GetBoundBox());
            descriptor[DictionaryProperties.MissingWidth] = new PdfNumber(metrics.WidthTable[c_defWidthIndex]);
            descriptor[DictionaryProperties.StemV] = new PdfNumber((int)metrics.StemV);
            descriptor[DictionaryProperties.ItalicAngle] = new PdfNumber((int)metrics.ItalicAngle);
            descriptor[DictionaryProperties.CapHeight] = new PdfNumber((int)metrics.CapHeight);
            descriptor[DictionaryProperties.Ascent] = new PdfNumber((int)metrics.WinAscent);
            descriptor[DictionaryProperties.Descent] = new PdfNumber((int)metrics.WinDescent);
            descriptor[DictionaryProperties.Leading] = new PdfNumber((int)metrics.Leading);
            descriptor[DictionaryProperties.AvgWidth] = new PdfNumber(metrics.WidthTable[c_defWidthIndex]);
            descriptor[DictionaryProperties.FontFile2] = new PdfReferenceHolder(m_fontProgram);
            descriptor[DictionaryProperties.MaxWidth] = new PdfNumber(metrics.WidthTable[c_defWidthIndex]);
            descriptor[DictionaryProperties.XHeight] = new PdfNumber(0);
            descriptor[DictionaryProperties.StemH] = new PdfNumber(0);

            return descriptor;
        }

        /// <summary>
        /// Generates name of the font.
        /// </summary>
        private string FormatName(string fontName)
        {
            if (fontName == null)
                throw new ArgumentNullException("fontName");

            if (fontName == string.Empty)
                throw new ArgumentOutOfRangeException("fontName", "Parameter can not be empty");

            string ret = fontName.Replace("(", "#28");

            ret = ret.Replace(")", "#29");
            ret = ret.Replace("[", "#5B");
            ret = ret.Replace("]", "#5D");
            ret = ret.Replace("<", "#3C");
            ret = ret.Replace(">", "#3E");
            ret = ret.Replace("{", "#7B");
            ret = ret.Replace("}", "#7D");
            ret = ret.Replace("/", "#2F");
            ret = ret.Replace("%", "#25");

            return ret.Replace(" ", "#20");
        }

        /// <summary>
        /// Gets random string.
        /// </summary>
        private string GetFontName()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            string name;

            if (m_type == CompositeFontType.Type0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int index = random.Next(c_nameString.Length);

                    builder.Append(c_nameString[index]);
                }

                builder.Append('+');
                builder.Append(m_ttfReader.Metrics.PostScriptName);
            }
            else
            {
                builder.Append(m_ttfReader.Metrics.PostScriptName);
            }
            name = builder.ToString();

            name = FormatName(name);

            return name;
        }

        /// <summary>
        /// Gets width description pad array for cid font.
        /// </summary>
        /// <returns>Width description pad array for cid font.</returns>
        public PdfArray GetDescendantWidth()
        {
            lock (s_syncLock)
            {
                PdfArray array = null;

                if (m_usedChars != null && m_usedChars.Count > 0)
                {
                    array = new PdfArray();

                    List<TtfGlyphInfo> glyphInfo = new List<TtfGlyphInfo>();

                    foreach (KeyValuePair<char, char> entry in m_usedChars)
                    {
                        char chLen = (char)entry.Key;
                        TtfGlyphInfo glyph = m_ttfReader.GetGlyph(chLen);

                        if (glyph.Empty) continue;

                        glyphInfo.Add(glyph);
                    }

                    glyphInfo.Sort();

                    int firstGlyphIndex = 0;
                    int lastGlyphIndex = 0;
                    bool firstGlyphIndexWasSet = false;
                    PdfArray widthDetails = new PdfArray();

                    for (int i = 0, len = glyphInfo.Count; i < len; i++)
                    {
                        TtfGlyphInfo glyph = (TtfGlyphInfo)glyphInfo[i];

                        if (!firstGlyphIndexWasSet)
                        {
                            firstGlyphIndexWasSet = true;
                            firstGlyphIndex = glyph.Index;
                            lastGlyphIndex = glyph.Index - 1;
                        }

                        if ((lastGlyphIndex + 1 != glyph.Index || (i + 1 == len)) && len > 1)
                        {
                            // Add glyph index / width.
                            array.Add(new PdfNumber(firstGlyphIndex));

                            if (i != 0)
                            {
                                array.Add(widthDetails);
                            }

                            firstGlyphIndex = glyph.Index;
                            widthDetails = new PdfArray();
                        }

                        widthDetails.Add(new PdfNumber(glyph.Width));

                        if (i + 1 == len)
                        {
                            array.Add(new PdfNumber(firstGlyphIndex));
                            array.Add(widthDetails);
                        }

                        lastGlyphIndex = glyph.Index;
                    }
                }

                return array;
            }
        }

        /// <summary>
        /// Converts integer of decimal system to hex integer.
        /// </summary>
        /// <param name="n">Integer to be converted.</param>
        /// <returns>Hex string.</returns>
        private string ToHexString(int n)
        {
            string s = Convert.ToString(n, 16);

            return "<0000".Substring(0, 5 - s.Length) + s + ">";
        }

        /// <summary>
        /// Calculates flags for the font descriptor.
        /// </summary>
        /// <returns>Flags for the font descriptor.</returns>
        private int GetDescriptorFlags()
        {
            int flags = 0;
            TtfMetrics metrics = m_ttfReader.Metrics;

            if (metrics.IsFixedPitch)
            {
                flags |= (int)FontDescriptorFlags.FixedPitch;
            }
            if (metrics.IsSymbol)
            {
                flags |= (int)FontDescriptorFlags.Symbolic;
            }
            else
            {
                flags |= (int)FontDescriptorFlags.Nonsymbolic;
            }
            if (metrics.IsItalic)
            {
                flags |= (int)FontDescriptorFlags.Italic;
            }
            if (metrics.IsBold)
            {
                flags |= (int)FontDescriptorFlags.ForceBold;
            }

            return flags;
        }

        /// <summary>
        /// Calculates BoundBox of the descriptor.
        /// </summary>
        /// <returns>BoundBox of the descriptor.</returns>
        private RectangleF GetBoundBox()
        {

            RECT rect = m_ttfReader.Metrics.FontBox;
            int width = (int)Math.Abs(rect.right - rect.left);
            int height = (int)Math.Abs(rect.top - rect.bottom);
            RectangleF rectangle = new RectangleF(rect.left, rect.bottom, width, height);

            return rectangle;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Runs before font Dictionary will be saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event arguments.</param>
        private void FontDictionaryBeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if (m_usedChars != null && m_usedChars.Count > 0 &&
                !m_fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
            {
                m_fontDictionary[DictionaryProperties.ToUnicode] = new PdfReferenceHolder(m_cmap);
            }
        }

        /// <summary>
        /// Runs before font program stream save.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event arguments.</param>
        private void FontProgramBeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            GenerateFontProgram();
        }

        /// <summary>
        /// Runs before cmap will be saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event arguments.</param>
        private void CmapBeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            GenerateCmap();
        }

        /// <summary>
        /// Runs before font Dictionary will be saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event arguments.</param>
        private void DescendantFontBeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if (m_usedChars != null && m_usedChars.Count > 0)
            {
                PdfArray width = GetDescendantWidth();

                if (width != null)
                {
                    m_descendantFont[DictionaryProperties.W] = width;
                }
            }
        }
        #endregion
    }
}
