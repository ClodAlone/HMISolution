#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the font.
    /// </summary>
    public abstract class PdfFont :
        IPdfWrapper,
        IPdfCache
    {
        #region Constants
        /// <summary>
        /// Multiplier of the symbol width.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        internal const float CharSizeMultiplier = 0.001f;

        /// <summary>
        /// Synchronization object.
        /// </summary>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected static object s_syncObject = new object();
        #endregion

        #region Fields
        /// <summary>
        /// Size of the font.
        /// </summary>
        private float m_size;

        /// <summary>
        /// Style of the font.
        /// </summary>
        private PdfFontStyle m_style;

        /// <summary>
        /// Metrics of the font.
        /// </summary>
        private PdfFontMetrics m_fontMetrics;

        /// <summary>
        /// PDf primitive of the font.
        /// </summary>
        private IPdfPrimitive m_fontInternals;

        private string m_internalFontName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFont"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected PdfFont(float size)
        {
            m_size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFont"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected PdfFont(float size, PdfFontStyle style)
            : this(size)
        {
            SetStyle(style);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return Metrics.Name;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public float Size
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// Gets the height of the font in points.
        /// </summary>
        public float Height
        {
            get
            {
                return Metrics.GetHeight(null);
            }
        }

        /// <summary>
        /// Gets the style information for this font.
        /// </summary>
        public PdfFontStyle Style
        {
            get
            {
                return m_style;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfFont"/> is bold.
        /// </summary>
        /// <value><c>true</c> if bold; otherwise, <c>false</c>.</value>
        public bool Bold
        {
            get
            {
                return ((Style & PdfFontStyle.Bold) > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfFont"/> is italic.
        /// </summary>
        /// <value><c>true</c> if italic; otherwise, <c>false</c>.</value>
        public bool Italic
        {
            get
            {
                return ((Style & PdfFontStyle.Italic) > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfFont"/> is strikeout.
        /// </summary>
        /// <value><c>true</c> if strikeout; otherwise, <c>false</c>.</value>
        public bool Strikeout
        {
            get
            {
                return ((Style & PdfFontStyle.Strikeout) > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfFont"/> is underline.
        /// </summary>
        /// <value><c>true</c> if underline; otherwise, <c>false</c>.</value>
        public bool Underline
        {
            get
            {
                return ((Style & PdfFontStyle.Underline) > 0);
            }
        }

        /// <summary>
        /// Gets or sets the Metrics of the font.
        /// </summary>
        internal PdfFontMetrics Metrics
        {
            get
            {
                return m_fontMetrics;
            }

            set
            {
                m_fontMetrics = value;
            }
        }

        internal string InternalFontName
        {
            get
            {
                return m_internalFontName;
            }
            set
            {
                m_internalFontName = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text)
        {
            return MeasureString(text, null);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, PdfStringFormat format)
        {
            int charactersFitted = 0;
            int linesFilled = 0;
            return MeasureString(text, format, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <param name="charactersFitted">Number of characters in the string.</param>
        /// <param name="linesFilled">Number of text lines in the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, PdfStringFormat format, out int charactersFitted, out int linesFilled)
        {
            return MeasureString(text, 0, format, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="width">Maximum width of the string in points.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, float width)
        {
            return MeasureString(text, width, null);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="width">Maximum width of the string in points.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, float width, PdfStringFormat format)
        {
            int charactersFitted = 0;
            int linesFilled = 0;

            return MeasureString(text, width, format, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="width">Maximum width of the string in points.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <param name="charactersFitted">Number of characters in the string.</param>
        /// <param name="linesFilled">Number of text lines in the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, float width, PdfStringFormat format,
            out int charactersFitted, out int linesFilled)
        {
            SizeF layoutArea = new SizeF(width, 0);

            return MeasureString(text, layoutArea, format, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="layoutArea">SizeF structure that specifies the maximum layout area for the text in points.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, SizeF layoutArea)
        {
            return MeasureString(text, layoutArea, null);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="layoutArea">SizeF structure that specifies the maximum layout area for the text in points.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, SizeF layoutArea, PdfStringFormat format)
        {
            int charactersFitted = 0;
            int linesFilled = 0;

            return MeasureString(text, layoutArea, format, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// Measures a string by using this font.
        /// </summary>
        /// <param name="text">Text to be measured.</param>
        /// <param name="layoutArea">SizeF structure that specifies the maximum layout area for the text in points.</param>
        /// <param name="format">PdfStringFormat that represents formatting information, such as line spacing, for the string.</param>
        /// <param name="charactersFitted">Number of characters in the string.</param>
        /// <param name="linesFilled">Number of text lines in the string.</param>
        /// <returns>Size of the text.</returns>
        public SizeF MeasureString(string text, SizeF layoutArea, PdfStringFormat format,
            out int charactersFitted, out int linesFilled)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            PdfStringLayouter layouter = new PdfStringLayouter();
            PdfStringLayoutResult result = layouter.Layout(text, this, format, layoutArea);

            charactersFitted = (result.Remainder == null) ? text.Length : text.Length - result.Remainder.Length;
            linesFilled = (result.Empty) ? 0 : result.Lines.Length;

            return result.ActualSize;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets Pdf primitive representing the font.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_fontInternals;
            }
        }
        #endregion

        #region IPdfCache Members
        /// <summary>
        /// Checks whether the object is similar to another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True - if the objects have equal internals and can share them, False otherwise.</returns>
        bool IPdfCache.EqualsTo(IPdfCache obj)
        {
            bool result = EqualsToFont(obj as PdfFont);

            return result;
        }

        /// <summary>
        /// Returns internals of the object.
        /// </summary>
        /// <returns>Returns internals of the object.</returns>
        IPdfPrimitive IPdfCache.GetInternals()
        {
            return m_fontInternals;
        }

        /// <summary>
        /// Sets internals to the object.
        /// </summary>
        /// <param name="internals">Internals of the object.</param>
        void IPdfCache.SetInternals(IPdfPrimitive internals)
        {
            if (internals == null)
            {
                throw new ArgumentNullException("internals");
            }

            m_fontInternals = internals;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>True if fonts are equal, False otherwise.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected abstract bool EqualsToFont(PdfFont font);

        /// <summary>
        /// Returns width of the char.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the symbol.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal abstract float GetCharWidth(char charCode, PdfStringFormat format);

        /// <summary>
        /// Returns width of the line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the line.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal abstract float GetLineWidth(string line, PdfStringFormat format);

        /// <summary>
        /// Sets the style.
        /// </summary>
        /// <param name="style">The style.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected void SetStyle(PdfFontStyle style)
        {
            m_style = style;
        }

        /// <summary>
        /// Applies settings to the default line width.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="format">String format.</param>
        /// <param name="width">Default line width.</param>
        /// <returns>Line width with settings applied.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected float ApplyFormatSettings(string line, PdfStringFormat format, float width)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            float realWidth = width;

            if (format != null && width > 0)
            {
                // Space among characters is not default.
                if (format.CharacterSpacing != 0f)
                {
                    realWidth += (line.Length - 1) * format.CharacterSpacing;
                }

                // Space among words is not default.
                if (format.WordSpacing != 0f)
                {
                    char[] symbols = StringTokenizer.Spaces;
                    int whitespacesCount = StringTokenizer.GetCharsCount(line, symbols);
                    realWidth += whitespacesCount * format.WordSpacing;
                }
            }

            return realWidth;
        }
        #endregion
    }
}
