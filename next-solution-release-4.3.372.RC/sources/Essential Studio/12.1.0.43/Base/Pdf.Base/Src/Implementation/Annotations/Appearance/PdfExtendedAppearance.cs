#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents extended appearance of the annotation. It has two states such as On state and Off state.
    /// </summary>
    public class PdfExtendedAppearance : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store normal appearance.
        /// </summary>
        private PdfAppearanceState m_normal = null;

        /// <summary>
        /// Internal variable to store appearance for pressed state.
        /// </summary>
        private PdfAppearanceState m_pressed = null;

        /// <summary>
        /// Internal variable to store appearance for state when mouse is hovered.
        /// </summary>
        private PdfAppearanceState m_mouseHover = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the normal appearance of the annotation.
        /// </summary>
        /// <value>The <see cref="PdfAppearanceState"/> object specifies the normal appearance of the annotation.</value>
        public PdfAppearanceState Normal
        {
            get
            {
                if (this.m_normal == null)
                {
                    this.m_normal = new PdfAppearanceState();
                    this.m_dictionary.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(this.m_normal));
                }

                return this.m_normal;
            }
        }

        /// <summary>
        /// Gets the appearance when mouse is hovered.
        /// </summary>
        /// <value>The <see cref="PdfAppearanceState"/> object specifies the annotation appearance when the mouse is hovered on it.</value>
        public PdfAppearanceState MouseHover
        {
            get
            {
                if (this.m_mouseHover == null)
                {
                    this.m_mouseHover = new PdfAppearanceState();
                    this.m_dictionary.SetProperty(DictionaryProperties.R, new PdfReferenceHolder(this.m_mouseHover));
                }

                return this.m_mouseHover;
            }
        }

        /// <summary>
        /// Gets the pressed state annotation.
        /// </summary>
        /// <value>The appearance in pressed state.</value>
        public PdfAppearanceState Pressed
        {
            get
            {
                if (this.m_pressed == null)
                {
                    this.m_pressed = new PdfAppearanceState();
                    this.m_dictionary.SetProperty(DictionaryProperties.D, new PdfReferenceHolder(this.m_pressed));
                }

                return this.m_pressed;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfExtendedAppearance"/> class.
        /// </summary>
        public PdfExtendedAppearance()
            : base()
        {
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
