#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    public class PdfLoadedWidgetAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal cross table
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal Annotation flags
        /// </summary>
        private PdfAnnotationFlags m_flags= PdfAnnotationFlags.Default;
        /// <summary>
        /// Internal variable to store extended appearance.
        /// </summary>
        private PdfExtendedAppearance m_extendedAppearance = null;
        /// <summary>
        /// Intrenal variable to store border parameters.
        /// </summary>
        private WidgetBorder m_border = new WidgetBorder();
        /// <summary>
        /// Internal variable to store appearance of the widget.
        /// </summary>
        private WidgetAppearance m_widgetAppearance = new WidgetAppearance();
        /// <summary>
        /// Internal variable to store highlighting mode.
        /// </summary>
        private PdfHighlightMode m_highlightMode = PdfHighlightMode.Invert;
        /// <summary>
        /// Internal variable to store default appearance.
        /// </summary>
        private PdfDefaultAppearance m_defaultAppearance = null;
        /// <summary>
        /// Internal variable to store annotation's actions.
        /// </summary>   
        private PdfAnnotationActions m_actions = null;
        /// <summary>
        /// Annotation's appearance.
        /// </summary>
        private PdfAppearance m_appearance = null;
        /// <summary>
        /// Internal variable to store alignment.
        /// </summary>
        private PdfTextAlignment m_alignment = PdfTextAlignment.Left;
        /// <summary>
        /// Internal variable to store default appearance state value.
        /// </summary>
        private string m_appearanceState = null;
        #endregion

        #region Properties        
        /// <summary>
        /// Gets or sets the extended appearance.
        /// </summary>
        /// <value>The extended appearance.</value>
        public PdfExtendedAppearance ExtendedAppearance
        {
            get
            {
                if (this.m_extendedAppearance == null)
                {
                    this.m_extendedAppearance = new PdfExtendedAppearance();
                }

                return this.m_extendedAppearance;
            }

            set
            {
                this.m_extendedAppearance = value;
                if (this.m_extendedAppearance != null)
                {
                    Dictionary.SetProperty(DictionaryProperties.AP, this.m_extendedAppearance);
                    Dictionary.SetProperty(DictionaryProperties.MK, null as IPdfPrimitive);
                }
                else
                {
                    if (this.m_appearance != null && this.m_appearance.GetNormalTemplate() != null)
                    {
                        Dictionary.SetProperty(DictionaryProperties.AP, this.m_appearance);
                    }
                    else
                    {
                        Dictionary.SetProperty(DictionaryProperties.AP, null as IPdfPrimitive);
                    }

                    Dictionary.SetProperty(DictionaryProperties.MK, this.m_widgetAppearance);
                    Dictionary.SetProperty(DictionaryProperties.AS, null as IPdfPrimitive);
                }
            }
        }

        /// <summary>
        /// Gets or sets the highlighting mode.
        /// </summary>
        /// <value>The highlighting mode.</value>
        public PdfHighlightMode HighlightMode
        {
            get
            {
                return this.m_highlightMode;
            }

            set
            {
                Dictionary.SetName(DictionaryProperties.H, this.HighlightModeToString(this.m_highlightMode));
                Dictionary.Modify();                
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        public PdfTextAlignment TextAlignment
        {
            get
            {
                return this.m_alignment;
            }

            set
            {
                if (this.m_alignment != value)
                {
                    this.m_alignment = value;
                    Dictionary.SetProperty(DictionaryProperties.Q, new PdfNumber((int)this.m_alignment));
                    Dictionary.Modify();
                }
            }
        }

        /// <summary>
        /// Gets the actions of the annotation.
        /// </summary>
        /// <value>The actions.</value>
        public PdfAnnotationActions Actions
        {
            get
            {
                if (this.m_actions == null)
                {
                    this.m_actions = new PdfAnnotationActions();
                    Dictionary.Remove(DictionaryProperties.AA);
                    Dictionary.SetProperty(DictionaryProperties.AA, this.m_actions);
                    Dictionary.Modify();
                }

                return m_actions;
            }
        }

        /// <summary>
        /// Gets or sets appearance of the annotation.
        /// </summary>
        public PdfAppearance Appearance
        {
            get
            {
                if (this.m_appearance == null)
                {
                    this.m_appearance = new PdfAppearance(this);
                }

                return this.m_appearance;
            }

            set
            {
                if (this.m_appearance != value)
                {
                    this.m_appearance = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets default appearance name.
        /// </summary>
        /// <value>The state of the appearance.</value>
        internal string AppearanceState
        {
            get
            {
                return this.m_appearanceState;
            }

            set
            {
                if (this.m_appearanceState != value)
                {
                    this.m_appearanceState = value;
                    Dictionary.SetName(DictionaryProperties.AS, value);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedWidgetAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="crossTable"></param>
        /// <param name="rectangle"></param>
        internal PdfLoadedWidgetAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle)
            : base(dictionary, crossTable)
        {
            Dictionary = dictionary;
            m_crossTable = crossTable;            
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Highlightings the mode to string.
        /// </summary>
        /// <param name="m_highlightingMode">The m_highlighting mode.</param>
        /// <returns>String representation of the highlighting mode in Pdf suiatable format.</returns>
        private string HighlightModeToString(PdfHighlightMode m_highlightingMode)
        {
            switch (m_highlightingMode)
            {
                case PdfHighlightMode.Invert:
                default:
                    return "I";

                case PdfHighlightMode.NoHighlighting:
                    return "N";

                case PdfHighlightMode.Outline:
                    return "O";

                case PdfHighlightMode.Push:
                    return "P";
            }
        }
        #endregion
    }
}
