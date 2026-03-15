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
    /// Represents the widget annotation.
    /// </summary>
    internal class WidgetAnnotation : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal variable to store parent field.
        /// </summary>
        private PdfField m_parent = null;

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
        /// Internal variable to store alignment.
        /// </summary>
        private PdfTextAlignment m_alignment = PdfTextAlignment.Left;

        /// <summary>
        /// Internal variable to store annotation's actions.
        /// </summary>
        private PdfAnnotationActions m_actions = null;

        /// <summary>
        /// Annotation's appearance.
        /// </summary>
        private PdfAppearance m_appearance = null;

        /// <summary>
        /// Internal variable to store default appearance state value.
        /// </summary>
        private string m_appearanceState = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public PdfField Parent
        {
            get
            {
                return this.m_parent;
            }

            set
            {
                if (this.m_parent != value)
                {
                    this.m_parent = value;

                    if (this.m_parent != null)
                    {
                        Dictionary.SetProperty(DictionaryProperties.Parent, new PdfReferenceHolder(this.m_parent));
                    }
                    else
                    {
                        Dictionary.Remove(DictionaryProperties.Parent);
                    }
                }
            }
        }

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
            }
        }

        /// <summary>
        /// Gets the default appearance.
        /// </summary>
        /// <value>The default appearance.</value>
        public PdfDefaultAppearance DefaultAppearance
        {
            get
            {
                if (this.m_defaultAppearance == null)
                {
                    this.m_defaultAppearance = new PdfDefaultAppearance();
                }

                return this.m_defaultAppearance;
            }
        }

        /// <summary>
        /// Gets or sets annotation's border.
        /// </summary>
        /// <value>The widget border.</value>
        public WidgetBorder WidgetBorder
        {
            get
            {
                return this.m_border;
            }
        }

        /// <summary>
        /// Gets the widget appearance.
        /// </summary>
        /// <value>The widget appearance.</value>
        public WidgetAppearance WidgetAppearance
        {
            get
            {
                return this.m_widgetAppearance;
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
                if (this.m_highlightMode != value)
                {
                    this.m_highlightMode = value;
                    Dictionary.SetName(DictionaryProperties.H, this.HighlightModeToString(this.m_highlightMode));
                }
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
                    Dictionary.SetProperty(DictionaryProperties.AA, this.m_actions);
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

        #region Events
        /// <summary>
        /// Raise before object saves.
        /// </summary>
        internal event EventHandler BeginSave;
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            AnnotationFlags |= PdfAnnotationFlags.Print;
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Widget));
            Dictionary.SetProperty(DictionaryProperties.BS, this.m_border);
        }

        /// <summary>
        /// Raises the <see cref="E:Save"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnBeginSave(EventArgs args)
        {
            if (this.BeginSave != null)
            {
                this.BeginSave(this, args);
            }
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();

            this.OnBeginSave(new EventArgs());

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

            if (this.m_defaultAppearance != null)
            {
                Dictionary.SetProperty(DictionaryProperties.DA, new PdfString(this.m_defaultAppearance.ToString()));
            }
        }

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

        /// <summary>
        /// Gets the appearance.
        /// </summary>
        /// <returns></returns>
        internal PdfAppearance GetAppearance()
        {
            return this.m_appearance;
        }
        #endregion
    }
}