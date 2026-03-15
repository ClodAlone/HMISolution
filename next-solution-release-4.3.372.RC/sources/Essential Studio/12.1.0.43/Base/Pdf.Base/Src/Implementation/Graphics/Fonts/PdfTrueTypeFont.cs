#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
#if !NETFX_CORE && !WP
using Microsoft.Win32;
#endif
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents TrueType font.
    /// </summary>
    ///[System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust" )]
#if AllowUnsafeCode || SILVERLIGHT || NETFX_CORE || WP
    public class PdfTrueTypeFont :
        PdfFont,
        IDisposable
#else
     internal class PdfTrueTypeFont :
		PdfFont,
		IDisposable
#endif
    {
        #region Constants
        /// <summary>
        /// Encoding for the font.
        /// </summary>
#if SILVERLIGHT || NETFX_CORE || WP
        internal static readonly Encoding Encoding = new Windows1252Encoding();
#else
        internal static readonly Encoding Encoding = Encoding.GetEncoding(c_codePage);
#endif
        /// <summary>
        /// Code page for the encoding.
        /// </summary>
        private const int c_codePage = 1252;
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether the font should be embeded.
        /// </summary>
        private bool m_embed = false;
        /// <summary>
        /// Indicates whether the font should use unicode symbols.
        /// </summary>
        private bool m_unicode = true;

        /// <summary>
        /// Internal font object.
        /// </summary>
        private ITrueTypeFont m_fontInternal;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bUseTrueType;
        #endregion

        #region Constructors
#if !SILVERLIGHT && !NETFX_CORE && !WP

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfTrueTypeFont(Font font)
            : this(font, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        public PdfTrueTypeFont(Font font, bool unicode)
            : this(font, font.SizeInPoints, unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        internal PdfTrueTypeFont(Font font, bool unicode, bool useTrueType)
            : this(font, font.SizeInPoints, unicode, useTrueType)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="size">The size.</param>
        public PdfTrueTypeFont(Font font, float size)
            : this(font, size, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="size">The size.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        public PdfTrueTypeFont(Font font, float size, bool unicode)
            : base(size, (PdfFontStyle)font.Style)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_unicode = unicode;

            CreateFontInternal(font);
        }

        internal PdfTrueTypeFont(Font font, float size, bool unicode, bool useTrueType)
            : base(size, (PdfFontStyle)font.Style)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_unicode = unicode;

            m_bUseTrueType = useTrueType;

            CreateFontInternal(font);
        }
        public PdfTrueTypeFont(Font font, FontStyle style, float size, bool unicode, bool embed)
            : base(size, (PdfFontStyle)style)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            font = new System.Drawing.Font(font.Name, font.Size, style);
            
            m_unicode = unicode;

            m_bUseTrueType = true;

            
            m_embed = embed;
            if (unicode == true && embed == false)
            {
                throw new Exception("Unicode font need to be embedded");
            }
            else
            CreateFontInternal(font);
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        public PdfTrueTypeFont(string fontFile, float size)
            : this(fontFile, size, PdfFontStyle.Regular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="isTrueType">Type of the is true.</param>
        internal PdfTrueTypeFont(string fontFile, float size, bool isTrueType)
            : this(fontFile, size, PdfFontStyle.Regular, isTrueType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfTrueTypeFont(string fontFile, float size, PdfFontStyle style)
            : base(size)
        {
            if (fontFile == null)
            {
                throw new ArgumentNullException("fontFile");
            }

            if (fontFile.Length == 0)
            {
                throw new ArgumentException("fontFile - string can not be empty");
            }

            m_unicode = true;

            CreateFontInternal(fontFile, style);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        internal PdfTrueTypeFont(string fontFile, float size, PdfFontStyle style, bool useTrueType)
            : base(size)
        {
            if (fontFile == null)
            {
                throw new ArgumentNullException("fontFile");
            }

            if (fontFile.Length == 0)
            {
                throw new ArgumentException("fontFile - string can not be empty");
            }

            m_bUseTrueType = useTrueType;

            m_unicode = true;

            CreateFontInternal(fontFile, style);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontStream">Font Stream.</param>
        /// <param name="size">Size of the font.</param>
        internal PdfTrueTypeFont(Stream fontStream, float size)
            : base(size)
        {
            m_unicode = true;

            CreateFontInternal(fontStream, PdfFontStyle.Regular);
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        public PdfTrueTypeFont(Stream fontStream, float size)
            : this(fontStream, size, PdfFontStyle.Regular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="isTrueType">Type of the is true.</param>
        internal PdfTrueTypeFont(Stream fontStream, float size, bool isTrueType)
            : this(fontStream, size, PdfFontStyle.Regular, isTrueType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfTrueTypeFont(Stream fontStream, float size, PdfFontStyle style)
            : base(size)
        {
            if (fontStream == null)
            {
                throw new ArgumentNullException("fontStream");
            }

            if (!fontStream.CanSeek || !fontStream.CanRead)
                throw new PdfException("Unable to parse the given font stream");

            fontStream.Seek(0, SeekOrigin.Begin);
            CreateFontInternal(fontStream, style);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="fontFile">The font file.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        internal PdfTrueTypeFont(Stream fontStream, float size, PdfFontStyle style, bool useTrueType)
            : base(size)
        {
            if (fontStream == null)
            {
                throw new ArgumentNullException("fontStream");
            }

            if (!fontStream.CanSeek || !fontStream.CanRead)
                throw new PdfException("Unable to parse the given font stream");

            m_bUseTrueType = useTrueType;

            m_unicode = true;

            CreateFontInternal(fontStream, style);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrueTypeFont"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size.</param>
        public PdfTrueTypeFont(PdfTrueTypeFont prototype, float size)
            : base(size, prototype.Style)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }

            m_unicode = prototype.Unicode;

            CreateFontInternal(prototype);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PdfTrueTypeFont"/> is reclaimed by garbage collection.
        /// </summary>
        ~PdfTrueTypeFont()
        {
            Dispose();
            //PdfDocument.Cache.Clear();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfTrueTypeFont"/> is unicode.
        /// </summary>
        /// <value><c>true</c> if unicode; otherwise, <c>false</c>.</value>
        public bool Unicode
        {
            get
            {
                return m_unicode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfTrueTypeFont"/> is embeded.
        /// </summary>
        /// <value><c>true</c> if embeded; otherwise, <c>false</c>.</value>
        internal bool Embed
        {
            get
            {
                return m_embed;
            }
        }
        /// <summary>
        /// Gets internals of the font.
        /// </summary>
        internal ITrueTypeFont InternalFont
        {
            get
            {
                return m_fontInternal;
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets font object of this font.
        /// </summary>
        internal Font Font
        {
            get
            {
                return InternalFont.Font;
            }
        }
#endif
        
        /// <summary>
        /// Gets path to the font file if the font was created from a file.
        /// </summary>
        internal string FontFile
        {
            get
            {
                string path = null;
                UnicodeTrueTypeFont unicodeFont = InternalFont as UnicodeTrueTypeFont;

                if (unicodeFont != null)
                {
                    path = unicodeFont.FontFile;
                }

                return path;
            }
        }
        #endregion

        #region IDisposable implementation
        /// <summary>
        /// Releases all resources of the font.
        /// </summary>
        /// <remarks>The font can be disposed when the document where this font was used is already closed.
        /// Don't dispose the font until the corresponding document is closed.</remarks>
        public void Dispose()
        {
            if (m_fontInternal != null)
            {
                lock (s_syncObject)
                {
                    int numberOfCached = 0;

                    if (PdfDocument.EnableCache)
                    {
                        PdfDocument.Cache.Remove(this);
                        numberOfCached = PdfDocument.Cache.GroupCount(this);

                        // The object was the last in the cache so we can release all the resources and it's safe
                        // because none of the fonts uses them.
                        if (numberOfCached == 0)
                        {
                            m_fontInternal.Close();
                        }

                        m_fontInternal = null;
                    }
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns width of the char.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the symbol.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal override float GetCharWidth(char charCode, PdfStringFormat format)
        {
            float codeWidth = InternalFont.GetCharWidth(charCode);
            float size = Metrics.GetSize(format);
            codeWidth *= (CharSizeMultiplier * size);

            return codeWidth;
        }

        /// <summary>
        /// Returns width of the line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the line.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected internal override float GetLineWidth(string line, PdfStringFormat format)
        {
            float width = 0f;

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (format == null || !format.RightToLeft || !Unicode)
            {
                width = InternalFont.GetLineWidth(line);
            }
            else
            {
                bool result = GetUnicodeLineWidth(line, out width);

                if (!result)
                {
                    width = InternalFont.GetLineWidth(line);
                }
            }
#else
            width = InternalFont.GetLineWidth(line);
#endif

            float size = Metrics.GetSize(format);
            width *= (CharSizeMultiplier * size);
            width = ApplyFormatSettings(line, format, width);

            return width;
        }

        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>True if fonts are equal, False otherwise.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected override bool EqualsToFont(PdfFont font)
        {
            bool result = m_fontInternal.EqualsToFont(font);

            return result;
        }

        /// <summary>
        /// Stores used symbols.
        /// </summary>
        /// <param name="text">String text.</param>
        internal void SetSymbols(string text)
        {
            if (m_fontInternal is UnicodeTrueTypeFont)
            {
                UnicodeTrueTypeFont internalFont = m_fontInternal as UnicodeTrueTypeFont;
                if (internalFont != null)
                {
                    internalFont.SetSymbols(text);
                }
            }
#if !SILVERLIGHT && !NETFX_CORE && !WP
            else
            {
                TrueTypeFont internalFont = m_fontInternal as TrueTypeFont;
                if (internalFont != null)
                {
                    internalFont.SetSymbols(text);
                }
            }
#endif
        }

        /// <summary>
        /// Stores used symbols.
        /// </summary>
        /// <param name="glyphs">Glyphs, used by the line of the text.</param>
        internal void SetSymbols(UInt16[] glyphs)
        {
            UnicodeTrueTypeFont internalFont = m_fontInternal as UnicodeTrueTypeFont;

            if (internalFont != null)
            {
                internalFont.SetSymbols(glyphs);
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Creates internal font object.
        /// </summary>
        /// <param name="font">System font.</param>
        private void CreateFontInternal(Font font)
        {
            if (!Unicode && m_bUseTrueType && Embed)
            {
                m_fontInternal = new TrueTypeFont(font, Size, true);
            }
            else
                if (Unicode && m_bUseTrueType && Embed)
                {
                    m_fontInternal = new UnicodeTrueTypeFont(font, Size, CompositeFontType.Type0);
                }
                else
                    if (Unicode && m_bUseTrueType)
                    {
                        m_fontInternal = new UnicodeTrueTypeFont(font, Size, CompositeFontType.TrueType);
                    }
                    else if (Unicode && !m_bUseTrueType)
                    {
                        m_fontInternal = new UnicodeTrueTypeFont(font, Size, CompositeFontType.Type0);
                    }
                    else
                    {
                        m_fontInternal = new TrueTypeFont(font, Size);
                    }

            InitializeInternals();
        }


        /// <summary>
        /// Creates internal font object.
        /// </summary>
        /// <param name="fontFile">Font file.</param>
        /// <param name="style">Suggested style of the font.</param>
        private void CreateFontInternal(string fontFile, PdfFontStyle style)
        {
            if (fontFile == null)
            {
                throw new ArgumentNullException("fontFile");
            }

            if (fontFile.Length == 0)
            {
                throw new ArgumentException("fontFile - string can not be empty");
            }

            if (!m_bUseTrueType)
            {
                m_fontInternal = new UnicodeTrueTypeFont(fontFile, Size, CompositeFontType.Type0);
            }
            else
            {
                m_fontInternal = new UnicodeTrueTypeFont(fontFile, Size, CompositeFontType.TrueType);
            }

            // Set style.
            CalculateStyle(style);
            InitializeInternals();
        }
#endif

        /// <summary>
        /// Creates a new font from a prototype font.
        /// </summary>
        /// <param name="prototype">Prototype object.</param>
        private void CreateFontInternal(PdfTrueTypeFont prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }

#if SILVERLIGHT || NETFX_CORE || WP
            m_fontInternal = new UnicodeTrueTypeFont(prototype.InternalFont as UnicodeTrueTypeFont);
#else
            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
            {
                m_unicode = true;
            }

            if (Unicode)
            {
                m_fontInternal = new UnicodeTrueTypeFont(prototype.InternalFont as UnicodeTrueTypeFont);
            }
            else
            {
                m_fontInternal = new TrueTypeFont(prototype.Font, Size);
            }
#endif
            InitializeInternals();
        }

        /// <summary>
        /// Creates internal font object.
        /// </summary>
        /// <param name="fontFile">Font file.</param>
        /// <param name="style">Suggested style of the font.</param>
        private void CreateFontInternal(Stream fontStream, PdfFontStyle style)
        {
            if (fontStream == null)
            {
                throw new ArgumentNullException("fontFile");
            }

            if (!fontStream.CanSeek || !fontStream.CanRead)
                throw new PdfException("Unable to parse the given font stream");

            if (!m_bUseTrueType)
            {
                m_fontInternal = new UnicodeTrueTypeFont(fontStream, Size, CompositeFontType.Type0);
            }
            else
            {
                m_fontInternal = new UnicodeTrueTypeFont(fontStream, Size, CompositeFontType.TrueType);
            }


            // Set style.
            CalculateStyle(style);
            InitializeInternals();
        }

        /// <summary>
        /// Initializes font internals.
        /// </summary>
        private void InitializeInternals()
        {
            //Lock this code to avoid corrupting a static cache by another threads.
            lock (s_syncObject)
            {
                IPdfCache equalFont = null;
                if (PdfDocument.EnableCache)
                {
                    // Search for the similar fonts.
                    equalFont = PdfDocument.Cache.Search(this);
                    
                }

                IPdfPrimitive internals = null;
                // There isn't equal font in the cache.
                if (equalFont != null)// || this.m_bUseTrueType)
                {
                    // Get the settings from the cached font.
                    internals = equalFont.GetInternals();

                    PdfFontMetrics metrics = ((PdfFont)equalFont).Metrics;
                    metrics = (PdfFontMetrics)metrics.Clone();
                    metrics.Size = Size;
                    Metrics = metrics;
                    m_fontInternal = ((PdfTrueTypeFont)equalFont).InternalFont;
                    
                }
                else
                {
                    if ((equalFont == null) || this.m_bUseTrueType)
                    {
                        if (PdfDocument.EnableCache)
                        {
                            if (this.m_bUseTrueType)
                                PdfDocument.Cache.Remove(equalFont);
                        }
                    m_fontInternal.CreateInternals();
                    internals = m_fontInternal.GetInternals();
                    Metrics = m_fontInternal.Metrics;
                    }
                }

                ((IPdfCache)this).SetInternals(internals);
            }
        }

        /// <summary>
        /// Sets the style of the font.
        /// </summary>
        /// <param name="style">Suggested style of the font.</param>
        private void CalculateStyle(PdfFontStyle style)
        {
            int iStyle = ((UnicodeTrueTypeFont)m_fontInternal).TtfMetrics.MacStyle;
            
            if ((style & PdfFontStyle.Underline) != 0)
            {
                iStyle |= (int)PdfFontStyle.Underline;
            }

            if ((style & PdfFontStyle.Strikeout) != 0)
            {
                iStyle |= (int)PdfFontStyle.Strikeout;
            }

            SetStyle((PdfFontStyle)style);
        }

        /// <summary>
        /// Calculates size of the symbol.
        /// </summary>
        /// <param name="ch">Symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Symbol size.</returns>
        private float GetSymbolSize(char ch, PdfStringFormat format)
        {
            float chSize = 0f;

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (format == null || !format.RightToLeft || !Unicode)
            {
                chSize = GetCharWidth(ch, format);
            }
            else
            {
                float size = Metrics.GetSize(format);
                GetUnicodeLineWidth(new string(ch, 1), out chSize);
                chSize *= (CharSizeMultiplier * size);
            }
#else
            chSize = GetCharWidth(ch, format);
#endif

            return chSize;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Calcuates width of the unicode line.
        /// </summary>
        /// <param name="line">String text.</param>
        /// <param name="width">Width of the line.</param>
        /// <returns>True if success, false otherwise.</returns>
        private bool GetUnicodeLineWidth(string line, out float width)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            width = 0f;

            UInt16[] glyphIndices = null;

            bool result = RtlRenderer.GetGlyphIndices(line, this, false, out glyphIndices);

            if (result && glyphIndices != null)
            {
                TtfReader ttfReader = (InternalFont as UnicodeTrueTypeFont).TtfReader;

                for (int i = 0, len = glyphIndices.Length; i < len; i++)
                {
                    int glyphIndex = glyphIndices[i];
                    TtfGlyphInfo glyph = ttfReader.GetGlyph(glyphIndex);

                    if (!glyph.Empty)
                    {
                        width += glyph.Width;
                    }
                }
            }

            return result;
        }
#endif
        #endregion
    }
}
