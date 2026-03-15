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
    /// <summary>
    /// Represents form's field with style parameters.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create signature field
    /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
    /// sign.Bounds = new RectangleF(100, 420, 100, 50);
    /// document.Form.Fields.Add(sign);           
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create signature field
    /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
    /// sign.Bounds = New RectangleF(100, 420, 100, 50)
    /// document.Form.Fields.Add(sign)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfField"/> Class   
    /// <seealso cref="PdfGraphics"/> Class
    /// <seealso cref="PdfFont"/> Class
    /// <seealso cref="PdfPage"/> Class
    public abstract class PdfSignatureStyledField : PdfField
    {
        #region Constants
        /// <summary>
        /// Internal variable to store color shift value.
        /// </summary>
        private const byte ShadowShift = 64;
        #endregion

        #region Fields
        /// <summary>
        /// Internal variable to store widget of the field.
        /// </summary>
        private WidgetAnnotation m_widget;

        /// <summary>
        /// Internal variable to store actions of the field.
        /// </summary>
        private PdfFieldActions m_actions = null;

        /// <summary>
        /// Internal variable to store appearance template.
        /// </summary>
        private PdfTemplate m_appearanceTemplate = null;

        /// <summary>
        /// Internal variable to store back color.
        /// </summary>
        private PdfBrush m_backBrush = null;

        /// <summary>
        /// Internal variable to store border pen.
        /// </summary>
        private PdfPen m_borderPen = null;

        /// <summary>
        /// Internal variable to store shadow brush.
        /// </summary>
        private PdfBrush m_shadowBrush = null;

        /// <summary>
        /// Internal variable to store visibility of the field.
        /// </summary>
        private bool m_visible = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureStyledField"/> class.
        /// </summary>
        /// <param name="page">The page where the field should be placed.</param>
        /// <param name="name">The name.</param>
        public PdfSignatureStyledField(PdfPageBase page, string name)
            : base(page, name)
        {
            AddAnnotationToPage(page, Widget);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureStyledField"/> class.
        /// </summary>
        internal PdfSignatureStyledField()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public virtual RectangleF Bounds
        {
            get
            {
                RectangleF rect = m_widget.Bounds;

                return GetBoundsAtLoadedPage(Page, rect);
            }

            set
            {
                m_widget.Bounds = value;

                BoundsAtLoadedPage(Page, value);
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public PointF Location
        {
            get
            {
                return m_widget.Location;
            }

            set
            {
                m_widget.SetLocation(value);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public SizeF Size
        {
            get
            {
                return m_widget.Size;
            }

            set
            {
                m_widget.SetSize(value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public PdfColor BorderColor
        {
            get
            {
                return m_widget.WidgetAppearance.BorderColor;
            }

            set
            {
                m_widget.WidgetAppearance.BorderColor = value;
                CreateBorderPen();
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
                return m_widget.WidgetAppearance.BackColor;
            }

            set
            {
                m_widget.WidgetAppearance.BackColor = value;
                CreateBackBrush();
            }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>The width of the border.</value>
        public int BorderWidth
        {
            get
            {
                return m_widget.WidgetBorder.Width;
            }

            set
            {
                if (m_widget.WidgetBorder.Width != value)
                {
                    m_widget.WidgetBorder.Width = value;
                    CreateBorderPen();
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
                return m_widget.HighlightMode;
            }

            set
            {
                m_widget.HighlightMode = value;
            }
        }

        /// <summary>
        /// Gets the actions of the field.
        /// </summary>
        /// <value>The actions.</value>
        public PdfFieldActions Actions
        {
            get
            {
                if (m_actions == null)
                {
                    m_actions = new PdfFieldActions(Widget.Actions);
                    Dictionary.SetProperty(DictionaryProperties.AA, m_actions);
                }

                return m_actions;
            }
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <value>The border style.</value>
        public PdfBorderStyle BorderStyle
        {
            get
            {
                return Widget.WidgetBorder.Style;
            }

            set
            {
                Widget.WidgetBorder.Style = value;
                CreateBorderPen();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfSignatureStyledField"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get
            {
                return m_visible;
            }

            set
            {
                if (m_visible != value)
                {
                    if (!value)
                    {
                        m_visible = value;
                        m_widget.AnnotationFlags = PdfAnnotationFlags.Hidden;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the right bottom beveled Shadow brush.
        /// </summary>
        /// <value>The right bottom beveled Shadow brush.</value>
        internal PdfBrush ShadowBrush
        {
            get
            {
                return m_shadowBrush;
            }
        }

        /// <summary>
        /// Gets the widget.
        /// </summary>
        /// <value>The widget.</value>
        internal WidgetAnnotation Widget
        {
            get
            {
                return m_widget;
            }
        }

        /// <summary>
        /// Gets the appearance template.
        /// </summary>
        /// <value>The appearance template.</value>
        internal PdfTemplate AppearanceTemplate
        {
            get
            {
                return m_appearanceTemplate;
            }
        }

        /// <summary>
        /// Gets the back brush.
        /// </summary>
        /// <value>The back brush.</value>
        internal PdfBrush BackBrush
        {
            get
            {
                return m_backBrush;
            }
        }

        /// <summary>
        /// Gets the border pen.
        /// </summary>
        /// <value>The border pen.</value>
        internal PdfPen BorderPen
        {
            get
            {
                return m_borderPen;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            RemoveAnnoationFromPage(Page, Widget);
        }

        /// <summary>
        /// Removes the annotation from page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="widget">The widget.</param>
        internal void RemoveAnnoationFromPage(PdfPageBase page, PdfAnnotation widget)
        {
            PdfPage simplePage = page as PdfPage;

            if (simplePage != null)
            {
                simplePage.Annotations.Remove(widget);
            }
            else
            {
                PdfLoadedPage loadedPage = page as PdfLoadedPage;
                PdfDictionary pageDic = loadedPage.Dictionary;
                PdfArray annots = null;

                if (pageDic.ContainsKey(DictionaryProperties.Annots))
                {
                    annots = loadedPage.CrossTable.GetObject(pageDic[DictionaryProperties.Annots]) as PdfArray;
                }
                else
                {
                    annots = new PdfArray();
                }

                widget.Dictionary.SetProperty(DictionaryProperties.P, new PdfReferenceHolder(loadedPage));
                annots.Remove(new PdfReferenceHolder(widget));
                page.Dictionary.SetProperty(DictionaryProperties.Annots, annots);
            }
        }

        /// <summary>
        /// Adds the annotation to page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="widget">The widget.</param>
        internal void AddAnnotationToPage(PdfPageBase page, PdfAnnotation widget)
        {
            PdfPage simplePage = page as PdfPage;

            if (simplePage != null)
            {
                simplePage.Annotations.Add(widget);
            }
            else
            {
                PdfLoadedPage loadedPage = page as PdfLoadedPage;
                PdfDictionary pageDic = loadedPage.Dictionary;
                PdfArray annots = null;

                if (pageDic.ContainsKey(DictionaryProperties.Annots))
                {
                    annots = loadedPage.CrossTable.GetObject(pageDic[DictionaryProperties.Annots]) as PdfArray;
                }
                else
                {
                    annots = new PdfArray();
                }

                widget.Dictionary.SetProperty(DictionaryProperties.P, new PdfReferenceHolder(loadedPage));
                annots.Add(new PdfReferenceHolder(widget));
                page.Dictionary.SetProperty(DictionaryProperties.Annots, annots);
            }
        }

        /// <summary>
        /// Bounds at loaded page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="value">The value.</param>
        internal void BoundsAtLoadedPage(PdfPageBase page, RectangleF value)
        {
            if (page is PdfLoadedPage)
            {
                RectangleF rectangle = value;

                PointF point = new PointF(rectangle.X, page.Size.Height - (rectangle.Bottom + rectangle.Height));
                rectangle = new RectangleF(point, rectangle.Size);

                Widget.Bounds = rectangle;
            }
        }

        /// <summary>
        /// Gets the bounds at loaded page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="rect">The rect.</param>
        /// <returns>The bounds of field.</returns>
        internal RectangleF GetBoundsAtLoadedPage(PdfPageBase page, RectangleF rect)
        {
            if (page is PdfLoadedPage)
            {
                PointF point = new PointF(rect.X, page.Size.Height - (rect.Bottom + rect.Height));
                rect = new RectangleF(point, rect.Size);
            }

            return rect;
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            m_widget = new WidgetAnnotation();
            m_widget.Parent = this;

            CreateBorderPen();
            CreateBackBrush();

            PdfArray m_array = new PdfArray();
            m_array.Add(new PdfReferenceHolder(m_widget));
            Dictionary.SetProperty(DictionaryProperties.Kids, new PdfArray(m_array));

            Widget.DefaultAppearance.FontName = "TiRo";
        }

        /// <summary>
        /// Creates the border pen.
        /// </summary>
        private void CreateBorderPen()
        {
            float width = m_widget.WidgetBorder.Width;

            m_borderPen = new PdfPen(m_widget.WidgetAppearance.BorderColor, width);

            if (Widget.WidgetBorder.Style == PdfBorderStyle.Dashed)
            {
                m_borderPen.DashStyle = PdfDashStyle.Custom;
                m_borderPen.DashPattern = new float[] { 3 / width };
            }
        }

        /// <summary>
        /// Creates the back brush.
        /// </summary>
        private void CreateBackBrush()
        {
            m_backBrush = new PdfSolidBrush(m_widget.WidgetAppearance.BackColor);

            PdfColor color = new PdfColor(m_widget.WidgetAppearance.BackColor);

            color.R = (byte)(color.R - ShadowShift >= 0 ? color.R - ShadowShift : 0);
            color.G = (byte)(color.G - ShadowShift >= 0 ? color.G - ShadowShift : 0);
            color.B = (byte)(color.B - ShadowShift >= 0 ? color.B - ShadowShift : 0);

            m_shadowBrush = new PdfSolidBrush(color);
        }

        #endregion
    }
}
