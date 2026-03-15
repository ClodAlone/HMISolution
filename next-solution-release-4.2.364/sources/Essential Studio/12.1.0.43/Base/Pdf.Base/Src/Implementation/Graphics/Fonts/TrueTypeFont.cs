#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;
using System.IO;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Graphics.Fonts
{
#if !SILVERLIGHT && !NETFX_CORE && !WP
    /// <summary>
    /// Creator of TrueType type font.
    /// </summary>
    internal class TrueTypeFont : ITrueTypeFont
    {
    #region Constants
        /// <summary>
        /// Suffix for bold font name.
        /// </summary>
        private const string c_boldSuffix = ",Bold";

        /// <summary>
        /// Suffix for bold italic font name.
        /// </summary>
        private const string c_boldItalicSuffix = ",BoldItalic";

        /// <summary>
        /// Suffix for italic font name.
        /// </summary>
        private const string c_italicSuffix = ",Italic";

        /// <summary>
        /// Coefficient for calculating font size.
        /// </summary>
        private const float c_fontSizeMultiplier = 72000f;

        /// <summary>
        /// Display driver name.
        /// </summary>
        private const string c_driverName = "DISPLAY";
        /// <summary>
        /// String for generating font name.
        /// </summary>
        private const string c_nameString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether truetypefont has to be emdeded
        /// </summary>
        private bool m_embed = false;
        /// <summary>
        /// ttf reader object.
        /// </summary>
        private TtfReader m_ttfReader;
        /// <summary>
        /// Ttf metrics structure.
        /// </summary>
        internal TtfMetrics m_ttfMetrics;
        /// <summary>
        /// System font.
        /// </summary>
        private Font m_font;

        /// <summary>
        /// Size of the font.
        /// </summary>
        private float m_size;

        /// <summary>
        /// Array of used chars.
        /// </summary>
        private Dictionary<char, char> m_usedChars;
        /// <summary>
        /// Pdf primitive describing the font.
        /// </summary>
        private PdfDictionary m_fontDictionary;

        /// <summary>
        /// Font program.
        /// </summary>
        private PdfStream m_fontProgram;
        /// <summary>
        /// Holds font-specific info such as first char/last char, etc.
        /// </summary>
        private OUTLINETEXTMETRIC m_nativeMetrics;

        /// <summary>
        /// Font metrics.
        /// </summary>
        private PdfFontMetrics m_metrics;
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="size">The size.</param>
        public TrueTypeFont(Font font, float size)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            m_font = font;
            m_size = size;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="size">The size.</param>
        /// <param name="embed">if set to <c>true</c> [embed].</param>
        public TrueTypeFont(Font font, float size, bool embed)
        {
            if (font == null)
            {
                throw new ArgumentException("font");
            }
            m_font = font;
            m_size = size;
            m_embed = embed;
            // We should dispose reader after each operation.
            using (BinaryReader reader = GetFontData())
            {
                m_ttfReader = new TtfReader(reader, m_font);
                m_ttfMetrics = m_ttfReader.Metrics;
                m_ttfReader.TrueTypeSubset = true;
            }           
        
        }
        #endregion

        #region ITrueTypeFont implementation
        /// <summary>
        /// Creates font program.
        /// </summary>
        /// <returns>Font program.</returns>
        private void CreateFontProgram()
        {
            m_fontProgram.BeginSave += new SavePdfPrimitiveEventHandler(FontProgramBeginSave);
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
            else
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");                    
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

            if (PdfDocument.Cache.FontData.ContainsKey(font))
            {
                byte[] buffer = PdfDocument.Cache.FontData[font];
                return new MemoryStream(buffer);
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

            if (numBytes == WinGdiConst.GDI_ERROR)
            {
                uint code = KernelApi.GetLastError();
                Debug.WriteLine("Can't create font, error code: " + code);

                throw new PdfException("Can't parse the font");
            }
            GdiApi.SelectObject(hDC, oldObj);
            GdiApi.DeleteObject(hFont);
            GdiApi.DeleteDC(hDC);

            //Add font data to cache.
            PdfDocument.Cache.FontData.Add(font, buff);

            return new MemoryStream(buff, 0, buff.Length, false);
        }
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

            if (trueTypeFont != null && !trueTypeFont.Unicode && trueTypeFont.InternalFont.Font != null && m_font != null)
            {
                bool equalName = (m_font.Name.Equals(trueTypeFont.InternalFont.Metrics.Name));
                bool equalStyle = (m_font.Style & (~(FontStyle.Underline | FontStyle.Strikeout))) ==
                    (trueTypeFont.InternalFont.Font.Style & (~(FontStyle.Underline | FontStyle.Strikeout)));

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
            m_metrics = new PdfFontMetrics();
            m_fontProgram = new PdfStream();
            // Parse font.
            RetrieveFontData();
            PdfDictionary fontDescriptor = CreateDescriptor();
            CreateFontDictionary(fontDescriptor);
            if (m_embed)
            {
                
                m_ttfReader.Reader = GetFontData();

                m_ttfReader.CreateInternals();
                m_ttfMetrics = m_ttfReader.Metrics;               

                CreateFontProgram();
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
        /// Generates font program.
        /// </summary>
        private void GenerateFontProgram()
        {
            byte[] fontProgram = null;
            m_usedChars = (m_usedChars == null) ? new Dictionary<char, char>() : m_usedChars;            
            m_ttfReader.InternalReader.Seek(0);
            fontProgram = m_ttfReader.ReadFontProgram(m_usedChars);
            m_fontProgram["Length1"] = new PdfNumber(fontProgram.Length);
            m_fontProgram.Write(fontProgram);
            fontProgram = null;    
        }

        /// <summary>
        /// Stores used symbols.
        /// </summary>
        /// <param name="text">String text.</param>
        public void SetSymbols(string text)
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
        }   
        
        /// <summary>
        /// Returns width of the char symbol.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <returns>Width of the char symbol in universal units.</returns>
        int ITrueTypeFont.GetCharWidth(char charCode)
        {
            int iCode = (int)charCode;
            if (iCode > Byte.MaxValue)
                throw new PdfException("Couldn't find information about the character. Unicode is not supported by this font.");

            int code = (int)charCode - m_nativeMetrics.otmTextMetrics.tmFirstChar;
            int codeWidth = 0;
            WidthTable widthTable = m_metrics.WidthTable;

            if (code >= 0 && code < (widthTable as StandardWidthTable).Length)
            {
                codeWidth = widthTable[code];
            }
            else
            {
                codeWidth = m_nativeMetrics.otmTextMetrics.tmAveCharWidth;
            }

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
            {
                throw new ArgumentNullException("line");

            }

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
            if (m_fontProgram != null)
            {
                m_fontProgram.Clear();
                m_fontProgram = null;
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
            m_font = null;
            m_metrics = null;
        }
        #endregion

    #region Implementation
        /// <summary>
        /// Creates font dictionary.
        /// </summary>
        /// <param name="fontDescriptor">Font descriptor.</param>
        private void CreateFontDictionary(PdfDictionary fontDescriptor)
        {
            if (fontDescriptor == null)
                throw new ArgumentNullException("fontDescriptor");

            m_fontDictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Font);
            m_fontDictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.TrueType);
            m_fontDictionary[DictionaryProperties.BaseFont] = new PdfName(GetFontName());
            m_fontDictionary[DictionaryProperties.FontDescriptor] = new PdfReferenceHolder(fontDescriptor);
            m_fontDictionary[DictionaryProperties.FirstChar] = new PdfNumber(m_metrics.FirstChar);

            int lastChar = (IsFixedPitch()) ? m_metrics.FirstChar : m_metrics.LastChar;

            m_fontDictionary[DictionaryProperties.LastChar] = new PdfNumber(lastChar);
            m_fontDictionary[DictionaryProperties.Widths] = m_metrics.WidthTable.ToArray();
            //new PdfArray( m_metrics.WidthTable );

            // NOTE: This can probably be wrong. NOTE 2: The code is commented due to issue with Tagged PDF.
            //if (m_font.GdiCharSet != (byte)GDI_CHARSET.SYMBOL_CHARSET)
            {
                string encoding = FontEncoding.WinAnsiEncoding.ToString();

                m_fontDictionary[DictionaryProperties.Encoding] = new PdfName(encoding);
            }
        }

        /// <summary>
        /// Creates font descriptor.
        /// </summary>
        /// <returns>Font descriptor object.</returns>
        private PdfDictionary CreateDescriptor()
        {
            PdfDictionary fontDescriptor = new PdfDictionary();
            TEXTMETRIC metrics = m_nativeMetrics.otmTextMetrics;

            fontDescriptor[DictionaryProperties.Type] = new PdfName(DictionaryProperties.FontDescriptor);
            fontDescriptor[DictionaryProperties.FontName] = new PdfName(GetFontName());
            fontDescriptor[DictionaryProperties.Flags] = new PdfNumber(GetDescriptorFlags());
            fontDescriptor[DictionaryProperties.FontBBox] = PdfArray.FromRectangle(GetBoundBox());
            fontDescriptor[DictionaryProperties.MissingWidth] = new PdfNumber(metrics.tmAveCharWidth);
            fontDescriptor[DictionaryProperties.StemV] = new PdfNumber((m_font.Bold) ? 144 : 72);
            fontDescriptor[DictionaryProperties.StemH] = new PdfNumber((m_font.Bold) ? 144 : 72);
            fontDescriptor[DictionaryProperties.ItalicAngle] = new PdfNumber((m_font.Italic) ?
                (m_nativeMetrics.otmItalicAngle / 10) : 0);
            if (m_embed)
            {
                fontDescriptor[DictionaryProperties.FontFile2] = new PdfReferenceHolder(m_fontProgram);
            }
            fontDescriptor[DictionaryProperties.CapHeight] = new PdfNumber(m_nativeMetrics.otmsCapEmHeight);
            fontDescriptor[DictionaryProperties.XHeight] = new PdfNumber((int)m_nativeMetrics.otmsXHeight);
            fontDescriptor[DictionaryProperties.Ascent] = new PdfNumber(m_nativeMetrics.otmAscent);
            fontDescriptor[DictionaryProperties.Descent] = new PdfNumber(m_nativeMetrics.otmDescent);
            fontDescriptor[DictionaryProperties.Leading] = new PdfNumber(m_nativeMetrics.otmMacAscent -
                m_nativeMetrics.otmMacDescent + (int)m_nativeMetrics.otmMacLineGap);

            fontDescriptor[DictionaryProperties.MaxWidth] = new PdfNumber(metrics.tmMaxCharWidth);
            fontDescriptor[DictionaryProperties.AvgWidth] = new PdfNumber(metrics.tmAveCharWidth);

            return fontDescriptor;
        }

        /// <summary>
        /// Retrieves data from the font.
        /// </summary>
        private void RetrieveFontData()
        {
            m_nativeMetrics = new OUTLINETEXTMETRIC();
            m_nativeMetrics.otmSize = (uint)Marshal.SizeOf(typeof(OUTLINETEXTMETRIC));

            using (Bitmap bmp = new Bitmap(1, 1))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                // We need to get metrics of the font in standard dimensions.
                float fontSize = c_fontSizeMultiplier / g.DpiX;
                Font sampleFont = new Font(m_font.Name, fontSize, m_font.Style,
                    GraphicsUnit.Point);

                IntPtr graphicsDC = g.GetHdc();
                IntPtr fontDC = sampleFont.ToHfont();
                IntPtr prevObj = GdiApi.SelectObject(graphicsDC, fontDC);

                // Retrieve font-specific info.
                int result = GdiApi.GetOutlineTextMetrics(graphicsDC, (int)m_nativeMetrics.otmSize, ref m_nativeMetrics);

                if (result != 0)
                {
                    CreateFontMetrics(graphicsDC);
                }
                else
                {
                    string message = GetErrorMessage();

                    Debug.WriteLine("Error: " + message);
                }

                GdiApi.SelectObject(graphicsDC, prevObj);
                GdiApi.DeleteObject(fontDC);
                g.ReleaseHdc(graphicsDC);
                sampleFont.Dispose();

                // Set default symbol width as whitespace width.
                // NOTE: This is a workaround for supporting Tab and other nonprintable symbols.
                m_nativeMetrics.otmTextMetrics.tmAveCharWidth = ((ITrueTypeFont)this).GetCharWidth(StringTokenizer.WhiteSpace);
            }
        }

        /// <summary>
        /// Calculates BoundBox of the descriptor.
        /// </summary>
        /// <returns>BoundBox of the descriptor.</returns>
        private RectangleF GetBoundBox()
        {
            int left = m_nativeMetrics.otmrcFontBox.left;
            int right = m_nativeMetrics.otmrcFontBox.right;
            int bottom = m_nativeMetrics.otmMacDescent;
            int top = m_nativeMetrics.otmMacAscent +
                (int)m_nativeMetrics.otmMacLineGap;

            RectangleF rect = new RectangleF(left, bottom, right - left, top - bottom);

            return rect;
        }

        /// <summary>
        /// Calculates flags for the font descriptor.
        /// </summary>
        /// <returns>Flags for the font descriptor.</returns>
        private int GetDescriptorFlags()
        {
            int flags = 0;

            if (IsFixedPitch())
            {
                flags |= (int)FontDescriptorFlags.FixedPitch;
            }
            if (!IsSerif())
            {
                flags |= (int)FontDescriptorFlags.Serif;
            }
            if (IsSymbolic())
            {
                flags |= (int)FontDescriptorFlags.Symbolic;
            }
            if (IsScript())
            {
                flags |= (int)FontDescriptorFlags.Script;
            }
            if (!IsSymbolic())
            {
                flags |= (int)FontDescriptorFlags.Nonsymbolic;
            }
            if (m_font.Italic)
            {
                flags |= (int)FontDescriptorFlags.Italic;
            }

            return flags;
        }

        /// <summary>
        /// Infills font metrics.
        /// </summary>
        /// <param name="graphicsDC">Graphics DC.</param>
        private void CreateFontMetrics(IntPtr graphicsDC)
        {
            TEXTMETRIC textMetrics = m_nativeMetrics.otmTextMetrics;

            m_metrics.Ascent = m_nativeMetrics.otmMacAscent;
            m_metrics.Descent = m_nativeMetrics.otmMacDescent;
            m_metrics.FirstChar = textMetrics.tmFirstChar;
            m_metrics.LastChar = textMetrics.tmLastChar;
            m_metrics.Height = (m_nativeMetrics.otmMacAscent - m_nativeMetrics.otmMacDescent +
                (int)m_nativeMetrics.otmMacLineGap);
            m_metrics.LineGap = (int)m_nativeMetrics.otmMacLineGap;

            m_metrics.Size = m_size;
            m_metrics.WidthTable = new StandardWidthTable(CreateWidthTable(graphicsDC));
            m_metrics.PostScriptName = GetFontName();
            m_metrics.Name = m_font.Name;
            m_metrics.SubScriptSizeFactor = (float)m_nativeMetrics.otmEMSquare /
                (float)(m_nativeMetrics.otmptSubscriptSize.x + m_nativeMetrics.otmptSubscriptSize.y);
            m_metrics.SuperscriptSizeFactor = (float)m_nativeMetrics.otmEMSquare /
                (float)(m_nativeMetrics.otmptSuperscriptSize.x + m_nativeMetrics.otmptSuperscriptSize.y);
        }

        /// <summary>
        /// Creates width table.
        /// </summary>
        /// <param name="graphicsDC">Graphics DC.</param>
        /// <returns>Width table.</returns>
        private int[] CreateWidthTable(IntPtr graphicsDC)
        {
            // If font is fixed size, we can use width table from one element only.
            int firstChar = m_metrics.FirstChar;
            int lastChar = (IsFixedPitch()) ? m_metrics.FirstChar : m_metrics.LastChar;

            int widthLen = lastChar - firstChar + 1;
            int[] widthTable = new int[widthLen];

            // Only W2000 and later supports GetCharWidth32.
            if (Environment.OSVersion.Platform >= PlatformID.Win32NT)
            {
                GdiApi.GetCharWidth(graphicsDC, firstChar, lastChar, widthTable);
            }
            else
            {
                // GetCharWidth32. Windows 95/98/Me: Unsupported.
                Size size = Size.Empty;

                // for each of the symbol set its width
                for (int i = 0, len = widthTable.Length; i < len; i++)
                {
                    string str = (firstChar + i).ToString();

                    GdiApi.GetTextExtentPoint(graphicsDC, str, str.Length, ref size);
                    widthTable[i] = size.Width;
                }
            }

            return widthTable;
        }

        /// <summary>
        /// Returns error message.
        /// </summary>
        /// <returns>Returns error message.</returns>
        private string GetErrorMessage()
        {
            IntPtr lpBuffer = Marshal.AllocHGlobal(4);
            uint error = KernelApi.GetLastError();
            uint result = KernelApi.FormatMessage(
                FormatMessageFlags.AllocateBuffer | FormatMessageFlags.FromSystem,
                (IntPtr)0, error, 0, lpBuffer, 4, (IntPtr)0);

            byte[] data = new byte[4];

            Marshal.Copy(lpBuffer, data, 0, 4);

            int pointer = BitConverter.ToInt32(data, 0);

            Marshal.FreeHGlobal(lpBuffer);

            lpBuffer = new IntPtr(pointer);
            data = new byte[result];
            Marshal.Copy(lpBuffer, data, 0, (int)result);
            Marshal.FreeHGlobal(lpBuffer);

            string errorString = Encoding.UTF8.GetString(data);

            return errorString;
        }


        /// <summary>
        /// Gets the name of the font.
        /// </summary>
        /// <returns>string</returns>
        private string GetFontName()
        {
            StringBuilder builder = new StringBuilder();
            
            if (m_embed)
            {
                Random random = new Random();
                string name;
                for (int i = 0; i < 6; i++)
                {
                    int index = random.Next(c_nameString.Length);

                    builder.Append(c_nameString[index]);
                }

                builder.Append('+');
            }

            builder.Append(FormatName(m_font.Name));

            if (m_font.Bold && m_font.Italic)
            {
                builder.Append(c_boldItalicSuffix);
            }
            else if (m_font.Bold)
            {
                builder.Append(c_boldSuffix);
            }
            else if (m_font.Italic)
            {
                builder.Append(c_italicSuffix);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats name
        /// </summary>
        private string FormatName(string fontName)
        {
            if (fontName == null)
                throw new ArgumentNullException("fontName");

            StringBuilder builder = new StringBuilder();

            byte[] bytes = PdfTrueTypeFont.Encoding.GetBytes(fontName);

            for (int i = 0, len = bytes.Length; i < len; ++i)
            {
                byte byteToTest = bytes[i];

                if (IsWordSymbol(byteToTest))
                {
                    builder.Append(((char)byteToTest));
                }
                else
                {
                    builder.AppendFormat("#{0:X2}", byteToTest);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether byte of font name needs special formatting.
        /// </summary>
        private bool IsWordSymbol(char chToTest)
        {
            return (char.IsLetter(chToTest) || char.IsNumber(chToTest));
        }

        /// <summary>
        /// Indicates whether byte of font name needs special formatting.
        /// </summary>
        private bool IsWordSymbol(byte byteToTest)
        {
            char chToTest = (char)byteToTest;

            return IsWordSymbol(chToTest);
        }

        /// <summary>
        /// Gets a value indicating whether font is symbolic
        /// </summary>
        private bool IsSymbolic()
        {
            return (m_nativeMetrics.otmTextMetrics.tmCharSet == (byte)GDI_CHARSET.SYMBOL_CHARSET);
        }

        /// <summary>
        /// Gets a value indicating whether font is fixed pitch
        /// </summary>
        private bool IsFixedPitch()
        {
            return ((m_nativeMetrics.otmTextMetrics.tmPitchAndFamily &
                (byte)GDI_PITCH_AND_FAMILY.TMPF_FIXED_PITCH) != (int)GDI_PITCH_AND_FAMILY.TMPF_FIXED_PITCH);
        }

        /// <summary>
        /// Gets a value indicating whether font is script
        /// </summary>
        private bool IsScript()
        {
            return ((m_nativeMetrics.otmTextMetrics.tmPitchAndFamily &
                (byte)GDI_PITCH_AND_FAMILY.FF_SCRIPT) == (int)GDI_PITCH_AND_FAMILY.FF_SCRIPT);
        }

        /// <summary>
        /// Gets a value indicating whether font is serif
        /// </summary>
        private bool IsSerif()
        {
            return ((m_nativeMetrics.otmTextMetrics.tmPitchAndFamily &
                (byte)GDI_PITCH_AND_FAMILY.FF_SWISS) == (int)GDI_PITCH_AND_FAMILY.FF_SWISS);
        }
        #endregion
    }
#endif
}
