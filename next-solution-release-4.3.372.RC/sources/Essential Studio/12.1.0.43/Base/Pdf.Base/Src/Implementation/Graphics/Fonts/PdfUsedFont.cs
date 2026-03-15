#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System.Collections.Generic;
using System.Drawing;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// Represents the used fonts in a PDF document.
    /// </summary>
    public class PdfUsedFont
    {
#region Fields
        private string m_name;
        private float m_size;
        private PdfFontStyle m_style;
        private PdfFontType m_type;
        private PdfFont m_internalFont;
        private PdfLoadedPage m_lpage;
        private string m_actualFontName;
        #endregion

#region Properties
        /// <summary>
        /// Gets the internal font.
        /// </summary>
        /// <value>The internal font.</value>
        internal PdfFont InternalFont
        {
            get
            {
                return m_internalFont;
            }
        }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return m_name;
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
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public PdfFontStyle Style
        {
            get
            {
                return m_style;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public PdfFontType Type
        {
            get
            {
                return m_type;
            }
        }

        /// <summary>
        /// Gets the actual name of the font.
        /// </summary>
        /// <value>The actual name of the font.</value>
        internal string ActualFontName
        {
            get
            {
                if (m_actualFontName == null)
                    m_actualFontName = GetActualFontName();


                return GetActualFontName();
            }
        }
        #endregion

#region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUsedFont"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfUsedFont(PdfFont font, PdfLoadedPage page)
        {
            InitializeInternals(font, page);
        }
        #endregion

#region Implementation
        /// <summary>
        /// Replaces the specified new font.
        /// </summary>
        /// <param name="newFont">The new font.</param>
        public void Replace(PdfFont fontToReplace)
        {
            CheckPreambula();

            PdfFont replaceFont = fontToReplace;
            PdfResources resources = m_lpage.GetResources();

            if (fontToReplace is PdfTrueTypeFont)
            {
                Font font = (fontToReplace as PdfTrueTypeFont).Font;
                string fontFile = (fontToReplace as PdfTrueTypeFont).FontFile;
                if (font == null && fontFile != null)
                {
                    replaceFont = new PdfTrueTypeFont(fontFile, fontToReplace.Size, true);
                }
                else
                {
                    replaceFont = new PdfTrueTypeFont(font, true, true);
                }
            }

            if (resources != null && replaceFont != null)
            {
                PdfName name = resources.GetName(ActualFontName);
                resources.RemoveFont(name.Value);
                resources.Add(replaceFont, name);
            }
           
        }
        #endregion

#region Helper Methods
        /// <summary>
        /// Initializes the internals.
        /// </summary>
        /// <param name="font">The font.</param>
        private void InitializeInternals(PdfFont font, PdfLoadedPage page)
        {
            m_lpage = page;
            m_internalFont = font;
            m_name = font.Name;
            m_size = font.Size;
            m_style = font.Style;

            if (font is PdfStandardFont)
                m_type = PdfFontType.Standard;
            else if (font is PdfTrueTypeFont && (font as PdfTrueTypeFont).Unicode == false)
                m_type = PdfFontType.TrueType;
            else
                m_type = PdfFontType.TrueTypeEmbedded;
        }

        /// <summary>
        /// Gets the actual name of the font.
        /// </summary>
        /// <returns></returns>
        private string GetActualFontName()
        {
            string actualFontName = string.Empty;

            PdfResources resources = m_lpage.GetResources();
            Dictionary<IPdfPrimitive, PdfName> names = resources.GetNames();
            foreach (KeyValuePair<IPdfPrimitive, PdfName> entry in names)
            {
                string baseFont = (entry.Key as PdfDictionary)["BaseFont"].ToString().TrimStart(new char[] { '/' });

                if (baseFont == this.Name || baseFont == this.InternalFont.InternalFontName)
                {
                    actualFontName = entry.Value.ToString();
                    break;
                }
            }

            return actualFontName.TrimStart(new char[] { '/' }); ;
        }

        /// <summary>
        /// Checks the preambula.
        /// </summary>
        private void CheckPreambula()
        {
            if (Type == PdfFontType.TrueTypeEmbedded)
                throw new PdfException("Can't replace font,  the font is already embedded");

        }
        #endregion

    }
}
#endif