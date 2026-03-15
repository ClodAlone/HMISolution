#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the border style of the widget annotation.
    /// </summary>
    internal class WidgetBorder : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store border width.
        /// </summary>
        private int m_width = 1;

        /// <summary>
        /// Internal variable to store border style;
        /// </summary>
        private PdfBorderStyle m_style = PdfBorderStyle.Solid;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return this.m_width;
            }

            set
            {
                this.m_width = value;
                this.m_dictionary.SetNumber(DictionaryProperties.W, this.m_width);
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public PdfBorderStyle Style
        {
            get
            {
                return this.m_style;
            }

            set
            {
                this.m_style = value;
                this.m_dictionary.SetName(DictionaryProperties.S, this.StyleToString(this.m_style));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetBorder"/> class.
        /// </summary>
        public WidgetBorder()
            : base()
        {
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Border));
            this.m_dictionary.SetName(DictionaryProperties.S, this.StyleToString(this.m_style));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts border style to string.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private string StyleToString(PdfBorderStyle style)
        {
            switch (style)
            {
                case PdfBorderStyle.Solid:
                default:
                    return "S";

                case PdfBorderStyle.Beveled:
                    return "B";

                case PdfBorderStyle.Dashed:
                    return "D";

                case PdfBorderStyle.Inset:
                    return "I";

                case PdfBorderStyle.Underline:
                    return "U";
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_dictionary;
            }
        }
        #endregion
    }
}
