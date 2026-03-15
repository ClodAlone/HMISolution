#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    public class PdfFreeTextAnnotation : PdfAnnotation
    {
        #region Constants
        private const string c_annotationType = "FreeText";
        #endregion

        #region Fields
        private PdfLineEndingStyle m_lineEndingStyle;
        private PdfAnnotationIntent m_annotationIntent;
        private string m_markUpText;
        private float m_opacity = 0.9f;
        private PdfFont m_font;
        private PointF[] m_calloutLines;
        private PdfColor m_textMarkupColor;
        private WidgetAnnotation m_widgetAnnotation = new WidgetAnnotation();
        private PdfColor m_borderColor;
        #endregion

        #region Properties
        public PdfLineEndingStyle LineEndingStyle
        {
            get
            {
                return m_lineEndingStyle;
            }
            set
            {
                m_lineEndingStyle = value;
            }
        }

        public PdfAnnotationIntent AnnotationIntent
        {
            get
            {
                return m_annotationIntent;
            }
            set
            {
                m_annotationIntent = value;
            }
        }

        public string MarkupText
        {
            get
            {
                return m_markUpText;
            }
            set
            {
                m_markUpText = value;
            }
        }

        public float Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Valid value should be between 0 to 1.");

                m_opacity = value;
            }
        }

        public PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Font");

                m_font = value;
            }
        }

        public PointF[] CalloutLines
        {
            get
            {
                return m_calloutLines;
            }
            set
            {
                m_calloutLines = value;
            }
        }

        public PdfColor TextMarkupColor
        {
            get
            {
                return m_textMarkupColor;
            }
            set
            {
                m_textMarkupColor = value;
            }
        }

        public PdfColor BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;
            }
        }
        #endregion

        #region Constructors
        private PdfFreeTextAnnotation()
            : base()
        {
        }

        public PdfFreeTextAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
            base.Initialize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes Annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Saves an Text Markup Annotation .
        /// </summary>
        protected override void Save()
        {
            base.Save();

            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(c_annotationType));

            PdfArray textMarkupColor = new PdfArray();
            if (!this.Color.IsEmpty)
            {
                float red = this.Color.R / 255f;
                float green = this.Color.G / 255f;
                float blue = this.Color.B / 255f;
                textMarkupColor.Insert(0, new PdfNumber(red));
                textMarkupColor.Insert(1, new PdfNumber(green));
                textMarkupColor.Insert(2, new PdfNumber(blue));
            }

            Dictionary.SetProperty(DictionaryProperties.C, textMarkupColor);
            Dictionary.SetNumber(DictionaryProperties.CA, m_opacity);

            Dictionary.SetProperty(DictionaryProperties.T, new PdfString(m_markUpText));
            Dictionary.SetProperty(DictionaryProperties.Contents, new PdfString(m_markUpText));
            Dictionary.SetProperty(DictionaryProperties.IT, new PdfName(m_annotationIntent.ToString()));
            Dictionary.SetProperty(DictionaryProperties.LE, new PdfName(m_lineEndingStyle.ToString()));

#if NETFX_CORE || WP
#else
            System.Drawing.Color textColor = System.Drawing.Color.FromArgb(m_textMarkupColor.R, m_textMarkupColor.G, m_textMarkupColor.B);
            Dictionary.SetProperty(DictionaryProperties.DS,
                new PdfString(string.Format("font:{0} {1}pt; color:{2}", Font.Name, Font.Size,
                    System.Drawing.ColorTranslator.ToHtml(textColor))));
#endif
            string borderColor = string.Format("{0} {1} {2} rg ", m_borderColor.R / 255f, m_borderColor.G / 255f, m_borderColor.B / 255f);
            Dictionary.SetProperty(DictionaryProperties.DA, new PdfString(borderColor));

            if (m_calloutLines.Length >= 2)
            {
                PdfArray lines = new PdfArray();
                for (int i = 0; i < m_calloutLines.Length; i++)
                {
                    lines.Add(new PdfNumber(m_calloutLines[i].X));
                    lines.Add(new PdfNumber(m_calloutLines[i].Y));
                }
                Dictionary.SetProperty("CL", lines);
            }
        }
        #endregion
    }
}
