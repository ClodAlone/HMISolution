#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents appearance of the widget annotation.
    /// </summary>
    internal class WidgetAppearance : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store border's color.
        /// </summary>
        private PdfColor m_borderColor = new PdfColor(0, 0, 0);

        /// <summary>
        /// Internal variable to store color of the background.
        /// </summary>
        private PdfColor m_backColor = new PdfColor(255, 255, 255);

        /// <summary>
        /// Internal variable to store normal cation text.
        /// </summary>
        private string m_normalCaption = String.Empty;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public PdfColor BorderColor
        {
            get
            {
                return this.m_borderColor;
            }

            set
            {
                if (this.m_borderColor != value)
                {
                    this.m_borderColor = value;
                    this.m_dictionary.SetProperty(DictionaryProperties.BC, this.m_borderColor.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public PdfColor BackColor
        {
            get
            {
                return this.m_backColor;
            }

            set
            {
                if (this.m_backColor != value)
                {
                    this.m_backColor = value;

                    if (m_backColor.A == 0)
                    {
                        this.m_dictionary.SetProperty(DictionaryProperties.BC, new PdfArray(new float[3]));
                        this.m_dictionary.Remove(DictionaryProperties.BG);
                    }
                    else
                        this.m_dictionary.SetProperty(DictionaryProperties.BG, this.m_backColor.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets or sets the normal caption.
        /// </summary>
        /// <value>The normal caption.</value>
        public string NormalCaption
        {
            get
            {
                return this.m_normalCaption;
            }

            set
            {
                if (this.m_normalCaption != value)
                {
                    this.m_normalCaption = value;
                    this.m_dictionary.SetString(DictionaryProperties.CA, this.m_normalCaption);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetAppearance"/> class.
        /// </summary>
        public WidgetAppearance()
            : base()
        {
            this.m_dictionary.SetProperty(DictionaryProperties.BC, this.m_borderColor.ToArray());
            this.m_dictionary.SetProperty(DictionaryProperties.BG, this.m_backColor.ToArray());
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
