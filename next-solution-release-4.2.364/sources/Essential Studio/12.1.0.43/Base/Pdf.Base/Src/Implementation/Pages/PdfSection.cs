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
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;


namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents a section entity. A section it's a set of the pages with similar page settings.
    /// </summary>
    public class PdfSection :
        IPdfWrapper,
        IEnumerable
    {
        #region Fields
        private List<PdfPage> m_pages = new List<PdfPage>();
        private PdfArray m_pagesReferences;
        private PdfDictionary m_section;
        private PdfNumber m_count;
        private PdfSectionCollection m_parent;
        private PdfDictionary m_resources;
        /// <summary>
        /// Page settings of the pages in the section.
        /// </summary>
        private PdfPageSettings m_settings;
        /// <summary>
        /// Page template for the section.
        /// </summary>
        private PdfSectionTemplate m_pageTemplate;
        /// <summary>
        /// An instance of the class manipulating with a page label.
        /// </summary>
        private PdfPageLabel m_pageLabel;
        /// <summary>
        /// Indicates if the progress is turned on.
        /// </summary>
        private bool m_isProgressOn;
        /// <summary>
        /// Internal variable to store initial page settings.
        /// </summary>
        private PdfPageSettings m_initialSettings;
        /// <summary>
        /// Internal variable to store cached saved settings.
        /// </summary>
        private PdfPageTransition m_savedTransition;
        /// <summary>
        /// Internal variable to store whether transition has been saved already.
        /// </summary>
        private bool m_isTransitionSaved = false;
        /// <summary>
        /// A virtual collection of pages.
        /// </summary>
        private PdfSectionPageCollection m_pagesCollection;
        internal PdfDocumentBase m_document;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the pages.
        /// </summary>
        public PdfSectionPageCollection Pages
        {
            get
            {
                if (m_pagesCollection == null)
                {
                    m_pagesCollection = new PdfSectionPageCollection(this);
                }

                return m_pagesCollection;
            }
        }

        /// <summary>
        /// Gets or sets the page label.
        /// </summary>
        public PdfPageLabel PageLabel
        {
            get
            {
                return m_pageLabel;
            }
            set
            {
                if (value != null)
                {
                    Parent.PageLabelsSet();
                }
                m_pageLabel = value;
            }
        }

        /// <summary>
        /// Gets or sets page settings of the section.
        /// </summary>
        public PdfPageSettings PageSettings
        {
            get
            {
                return m_settings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("PageSettings");

                m_settings = value;
            }
        }

        /// <summary>
        /// Gets or sets a template for the pages in the section.
        /// </summary>
        public PdfSectionTemplate Template
        {
            get
            {
                if (m_pageTemplate == null)
                {
                    m_pageTemplate = new PdfSectionTemplate();
                }

                return m_pageTemplate;
            }
            set
            {
                m_pageTemplate = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:PdfPage"/> at the specified index.
        /// </summary>
        internal PdfPage this[int index]
        {
            get
            {
                if (0 > index || Count <= index)
                    throw new ArgumentOutOfRangeException("index", 
                        "The index can't be less then zero or greater then Count.");

                PdfPage page = (m_pages[index]) as PdfPage;

                return page;
            }
        }

        /// <summary>
        /// Gets the count of the pages in the section.
        /// </summary>
        internal int Count
        {
            get
            {
                return m_pagesReferences.Count;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal PdfSectionCollection Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;

                if (value != null)
                {
                    m_section[DictionaryProperties.Parent] = new PdfReferenceHolder(value);
                }
                else
                {
                    m_section.Remove(DictionaryProperties.Parent);
                }
            }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        internal PdfDictionary Resources
        {
            get
            {
                if (m_resources == null)
                {
                    m_resources = new PdfDictionary();
                    m_section[DictionaryProperties.Resources] = m_resources;
                }

                return m_resources;
            }
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        internal PdfDocument Document
        {
            get
            {
                return m_parent.Document;
            }
        }

        /// <summary>
        /// Gets the parent document.
        /// </summary>
        internal PdfDocumentBase ParentDocument
        {
            get
            {
                return m_document;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event rises when the new page has been added
        /// </summary>
        public event PageAddedEventHandler PageAdded;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSection"/> class.
        /// </summary>
        private PdfSection()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSection"/> class.
        /// </summary>
        /// <param name="pageSettings">The page settings.</param>
        internal PdfSection(PdfDocumentBase document, PdfPageSettings pageSettings)
            : this()
        {
            m_document = document;
            m_settings = (PdfPageSettings)pageSettings.Clone();
            m_initialSettings = (PdfPageSettings)m_settings.Clone();
        }

        /// <summary>
        ///	Creates a new instance of PDfSection class.
        /// </summary>
        /// <param name="document">Parent document for the section.</param>
        internal PdfSection(PdfDocument document)
            : this(document, document.PageSettings)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a page and adds it to the collection.
        /// </summary>
        /// <returns>Created page.</returns>
        internal PdfPage Add()
        {
            PdfPage page = new PdfPage();

            Add(page);
            return page;
        }

        /// <summary>
        /// Adds the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void Add(PdfPage page)
        {
            PdfReferenceHolder r = CheckPresence(page);

            m_pages.Add(page);
            m_pagesReferences.Add(r);
            page.SetSection(this);

            if (m_isProgressOn)
            {
                page.SetProgress();
            }
            else
            {
                page.ResetProgress();
            }

            PageAddedMethod(page);
        }

        /// <summary>
        /// Inserts a page at the specified index.
        /// </summary>
        /// <param name="index">The index of the page in the section.</param>
        /// <param name="page">The page.</param>
        internal void Insert(int index, PdfPage page)
        {
            PdfReferenceHolder r = CheckPresence(page);

            m_pages.Insert(index, page);
            m_pagesReferences.Insert(index, r);
            page.SetSection(this);

            if (m_isProgressOn)
            {
                page.SetProgress();
            }
            else
            {
                page.ResetProgress();
            }

            PageAddedMethod(page);
        }

        /// <summary>
        /// Get the index of the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The index of the page.</returns>
        internal int IndexOf(PdfPage page)
        {
            if (m_pages.Contains(page))
                return m_pages.IndexOf(page);

            PdfReferenceHolder r = new PdfReferenceHolder(page);

            return m_pagesReferences.IndexOf(r);
        }

        /// <summary>
        /// Determines whether the page in within the section.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// 	<c>true</c> if the specified page is within the section; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(PdfPage page)
        {
            int index = IndexOf(page);

            return (0 <= index);
        }

        /// <summary>
        /// Removes the page from the section.
        /// </summary>
        /// <param name="page">The page that should be removed from the section.</param>
        internal void Remove(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfReferenceHolder r = new PdfReferenceHolder(page);

            m_pagesReferences.Remove(r);
            m_pages.Remove(page);
            //page.SetSection(null);
        }

        /// <summary>
        /// Removes the page by its index in the section.
        /// </summary>
        /// <param name="index">Zero-based index of the page in the section.</param>
        internal void RemoveAt(int index)
        {
            PdfPage page = this[index];
            m_pagesReferences.RemoveAt(index);
            m_pages.RemoveAt(index);

            if (page != null)
            {
                page.SetSection(null);
            }
        }

        /// <summary>
        /// Removes all the pages from the section.
        /// </summary>
        /// <remarks>If the document contains one section only, this section should contain at least one page.</remarks>
        internal void Clear()
        {
            for (int i = m_pages.Count - 1; i > -1; i--)
            {
                PdfPage page = m_pages[i];
                Remove(page);
                page.Clear();
                page = null;
            }

            if (m_pages != null)
                m_pages.Clear();

            if (m_pagesReferences != null)
                m_pagesReferences.Clear();

            if (m_resources != null)
                m_resources.Clear();

            if (m_section != null)
                m_section.Clear();

            if (m_pagesCollection != null)
                m_pagesCollection.Clear();

            while (Count > 0)
            {
                RemoveAt(Count - 1);
            }

            m_pages = null;
            m_resources = null;
            m_pagesReferences = null;
            m_initialSettings = null;
            m_settings = null;
            m_parent = null;
            m_document = null;
            m_pagesCollection = null;
            m_pageTemplate = null;
            m_section = null;
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new PdfPageEnumerator(this);
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_section;
            }
        }
        #endregion

        #region Template methods
        /// <summary>
        /// Checks whether any template should be printed on this layer.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">The parent page.</param>
        /// <param name="foreground">Layer z-order.</param>
        /// <returns>True - if some content should be printed on the layer, False otherwise.</returns>
        internal bool ContainsTemplates(PdfDocument document, PdfPage page, bool foreground)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            PdfPageTemplateElement[] documentHeaders = GetDocumentTemplates(document, page, true, foreground);
            PdfPageTemplateElement[] documentTemplates = GetDocumentTemplates(document, page, false, foreground);
            PdfPageTemplateElement[] sectionHeaders = GetSectionTemplates(page, true, foreground);
            PdfPageTemplateElement[] sectionTemplates = GetSectionTemplates(page, false, foreground);

            bool contains = (documentHeaders.Length > 0 || documentTemplates.Length > 0 ||
                sectionHeaders.Length > 0 || sectionTemplates.Length > 0);

            return contains;
        }

        /// <summary>
        /// draws page templates on the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="layer">Page layer where the templates should be drawn.</param>
        /// <param name="document">Parent document.</param>
        /// <param name="foreground">Foreground layer if True, False otherwise.</param>
        internal void DrawTemplates(PdfPage page, PdfPageLayer layer, PdfDocument document, bool foreground)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");
            if (document == null)
                throw new ArgumentNullException("document");

            PdfPageTemplateElement[] documentHeaders = GetDocumentTemplates(document, page, true, foreground);
            PdfPageTemplateElement[] documentTemplates = GetDocumentTemplates(document, page, false, foreground);
            PdfPageTemplateElement[] sectionHeaders = GetSectionTemplates(page, true, foreground);
            PdfPageTemplateElement[] sectionTemplates = GetSectionTemplates(page, false, foreground);

            if (foreground)
            {
                DrawTemplates(layer, document, sectionHeaders);
                DrawTemplates(layer, document, sectionTemplates);

                DrawTemplates(layer, document, documentHeaders);
                DrawTemplates(layer, document, documentTemplates);
            }
            else
            {
                DrawTemplates(layer, document, documentHeaders);
                DrawTemplates(layer, document, documentTemplates);

                DrawTemplates(layer, document, sectionHeaders);
                DrawTemplates(layer, document, sectionTemplates);
            }
        }

        /// <summary>
        /// Calculates actual bounds of the page.
        /// </summary>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Actual bounds of the page.</returns>
        internal RectangleF GetActualBounds(PdfPage page, bool includeMargins)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            RectangleF result;

            if (Parent != null)
            {
                PdfDocument document = Parent.Document;

                if (document == null)
                    throw new PdfDocumentException("The section should be added to the section collection before this operation");

                result = GetActualBounds(document, page, includeMargins);
            }
            else
            {
                SizeF size = (includeMargins) ? PageSettings.GetActualSize() : PageSettings.Size;
                float left = (includeMargins) ? PageSettings.Margins.Left : 0;
                float top = (includeMargins) ? PageSettings.Margins.Top : 0;
                PointF location = new PointF(left, top);

                result = new RectangleF(location, size);
            }

            return result;
        }

        /// <summary>
        /// Calculates actual bounds of the page.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Actual bounds of the page.</returns>
        internal RectangleF GetActualBounds(PdfDocument document, PdfPage page, bool includeMargins)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            RectangleF bounds = RectangleF.Empty;

            bounds.Size = (includeMargins) ? PageSettings.Size : PageSettings.GetActualSize();

            float left = GetLeftIndentWidth(document, page, includeMargins);
            float top = GetTopIndentHeight(document, page, includeMargins);
            float right = GetRightIndentWidth(document, page, includeMargins);
            float bottom = GetBottomIndentHeight(document, page, includeMargins);

            bounds.X += left;
            bounds.Y += top;

            bounds.Width -= (left + right);
            bounds.Height -= (top + bottom);

            return bounds;
        }

        /// <summary>
        /// Calculates width of the left indent.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Width of the left indent.</returns>
        internal float GetLeftIndentWidth(PdfDocument document, PdfPage page, bool includeMargins)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            float value = (includeMargins) ? PageSettings.Margins.Left : 0f;

            float templateWidth = (Template.GetLeft(page) != null) ? Template.GetLeft(page).Width : 0f;
            float docTemplateWidth = (document.Template.GetLeft(page) != null) ? document.Template.GetLeft(page).Width : 0f;

            value += (Template.ApplyDocumentLeftTemplate) ?
                Math.Max(templateWidth, docTemplateWidth) : templateWidth;

            return value;
        }

        /// <summary>
        /// Calculates Height of the top indent.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Height of the top indent.</returns>
        internal float GetTopIndentHeight(PdfDocument document, PdfPage page, bool includeMargins)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            float value = (includeMargins) ? PageSettings.Margins.Top : 0f;

            float templateHeight = (Template.GetTop(page) != null) ? Template.GetTop(page).Height : 0f;
            float docTemplateHeight = (document.Template.GetTop(page) != null) ? document.Template.GetTop(page).Height : 0f;

            value += (Template.ApplyDocumentTopTemplate) ?
                Math.Max(templateHeight, docTemplateHeight) : templateHeight;

            return value;
        }

        /// <summary>
        /// Calculates width of the right indent.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Width of the right indent.</returns>
        internal float GetRightIndentWidth(PdfDocument document, PdfPage page, bool includeMargins)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            float value = (includeMargins) ? PageSettings.Margins.Right : 0f;

            float templateWidth = (Template.GetRight(page) != null) ? Template.GetRight(page).Width : 0f;
            float docTemplateWidth = (document.Template.GetRight(page) != null) ? document.Template.GetRight(page).Width : 0f;

            value += (Template.ApplyDocumentRightTemplate) ?
                Math.Max(templateWidth, docTemplateWidth) : templateWidth;

            return value;
        }

        /// <summary>
        /// Calculates Height of the bottom indent.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">Page where the bounds should be calculated.</param>
        /// <param name="includeMargins">If true - take into consideration Margins.</param>
        /// <returns>Height of the top indent.</returns>
        internal float GetBottomIndentHeight(PdfDocument document, PdfPage page, bool includeMargins)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            float value = (includeMargins) ? PageSettings.Margins.Bottom : 0f;

            float templateHeight = (Template.GetBottom(page) != null) ? Template.GetBottom(page).Height : 0f;
            float docTemplateHeight = (document.Template.GetBottom(page) != null) ? document.Template.GetBottom(page).Height : 0f;

            value += (Template.ApplyDocumentBottomTemplate) ?
                Math.Max(templateHeight, docTemplateHeight) : templateHeight;

            return value;
        }

        /// <summary>
        /// Translates point into native coordinates of the page.
        /// </summary>
        /// <param name="page">The parent page.</param>
        /// <param name="point">Point to translate.</param>
        /// <returns>Point in native page coordinates.</returns>
        internal PointF PointToNativePdf(PdfPage page, PointF point)
        {
            RectangleF bounds = GetActualBounds(page, true);

            point.X += bounds.Left;
            point.Y = PageSettings.Height - (bounds.Top + point.Y);

            return point;
        }

        /// <summary>
        /// Draws an array of the templates.
        /// </summary>
        /// <param name="layer">Parent layer.</param>
        /// <param name="document">PArent document.</param>
        /// <param name="templates">Array of templates.</param>
        private void DrawTemplates(PdfPageLayer layer, PdfDocument document, PdfPageTemplateElement[] templates)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            if (document == null)
                throw new ArgumentNullException("document");

            if (templates != null && templates.Length > 0)
            {
                for (int i = 0, len = templates.Length; i < len; i++)
                {
                    PdfPageTemplateElement template = templates[i];
                    template.Draw(layer, document);
                }
            }
        }

        /// <summary>
        /// Returns array of the document templates.
        /// </summary>
        /// <param name="document">Parent document.</param>
        /// <param name="page">the parent page.</param>
        /// <param name="headers">If true - return headers/footers, if false - return simple templates.</param>
        /// <param name="foreground">If true - return foreground templates, if false - return background templates.</param>
        /// <returns>Returns array of the document templates.</returns>
        private PdfPageTemplateElement[] GetDocumentTemplates(PdfDocument document, PdfPage page, bool headers, bool foreground)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (page == null)
                throw new ArgumentNullException("page");

            List<PdfPageTemplateElement> templates = new List<PdfPageTemplateElement>();

            if (headers)
            {
                if (Template.ApplyDocumentTopTemplate && document.Template.GetTop(page) != null &&
                    document.Template.GetTop(page).Foreground == foreground)
                {
                    templates.Add(document.Template.GetTop(page));
                }

                if (Template.ApplyDocumentBottomTemplate && document.Template.GetBottom(page) != null &&
                    document.Template.GetBottom(page).Foreground == foreground)
                {
                    templates.Add(document.Template.GetBottom(page));
                }

                if (Template.ApplyDocumentLeftTemplate && document.Template.GetLeft(page) != null &&
                    document.Template.GetLeft(page).Foreground == foreground)
                {
                    templates.Add(document.Template.GetLeft(page));
                }

                if (Template.ApplyDocumentRightTemplate && document.Template.GetRight(page) != null &&
                    document.Template.GetRight(page).Foreground == foreground)
                {
                    templates.Add(document.Template.GetRight(page));
                }
            }
            else if (Template.ApplyDocumentStamps)
            {
                for (int i = 0, len = document.Template.Stamps.Count; i < len; i++)
                {
                    PdfPageTemplateElement template = document.Template.Stamps[i];

                    if (template.Foreground == foreground)
                    {
                        templates.Add(template);
                    }
                }
            }

            return templates.ToArray();
        }

        /// <summary>
        /// Returns array of the section templates.
        /// </summary>
        /// <param name="page">The parent page.</param>
        /// <param name="headers">If true - return headers/footers, if false - return simple templates.</param>
        /// <param name="foreground">If true - return foreground templates, if false - return background templates.</param>
        /// <returns>Returns array of the document templates.</returns>
        private PdfPageTemplateElement[] GetSectionTemplates(PdfPage page, bool headers, bool foreground)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            List<PdfPageTemplateElement> templates = new List<PdfPageTemplateElement>();

            if (headers)
            {
                if (Template.GetTop(page) != null && Template.GetTop(page).Foreground == foreground)
                {
                    templates.Add(Template.GetTop(page));
                }

                if (Template.GetBottom(page) != null && Template.GetBottom(page).Foreground == foreground)
                {
                    templates.Add(Template.GetBottom(page));
                }

                if (Template.GetLeft(page) != null && Template.GetLeft(page).Foreground == foreground)
                {
                    templates.Add(Template.GetLeft(page));
                }

                if (Template.GetRight(page) != null && Template.GetRight(page).Foreground == foreground)
                {
                    templates.Add(Template.GetRight(page));
                }
            }
            else
            {
                for (int i = 0, len = Template.Stamps.Count; i < len; i++)
                {
                    PdfPageTemplateElement template = Template.Stamps[i];

                    if (template.Foreground == foreground)
                    {
                        templates.Add(template);
                    }
                }
            }

            return templates.ToArray();
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Called when the page has been added
        /// </summary>
        /// <param name="args">Event arguments.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected virtual void OnPageAdded(PageAddedEventArgs args)
        {
            if (PageAdded != null)
            {
                PageAdded(this, args);
            }
        }

        /// <summary>
        /// Sets the progress.
        /// </summary>
        internal void SetProgress()
        {
            if (!m_isProgressOn)
            {
                foreach (PdfPage page in this)
                {
                    page.SetProgress();
                }

                m_isProgressOn = true;
            }
        }

        /// <summary>
        /// Resets the progress.
        /// </summary>
        internal void ResetProgress()
        {
            if (m_isProgressOn)
            {
                foreach (PdfPage page in this)
                {
                    page.ResetProgress();
                }

                m_isProgressOn = false;
            }
        }

        /// <summary>
        /// Called when a page is being saved.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void OnPageSaving(PdfPage page)
        {
            Parent.OnPageSaving(page);
        }

        /// <summary>
        /// Checks the presence.
        /// </summary>
        /// <param name="page">The page.</param>
        private PdfReferenceHolder CheckPresence(PdfPage page)
        {
            PdfReferenceHolder rh = new PdfReferenceHolder(page);
            bool contains = false;

            if (m_parent != null)
            {
                PdfSectionCollection sc = Parent;

                foreach (PdfSection section in sc)
                {
                    contains |= section.Contains(page);

                    if (contains) break;
                }
            }
            else
            {
                contains = m_pagesReferences.Contains(rh);
            }

            if (contains)
                throw new ArgumentException("The page already exists in some section, it can't be contained by several sections", "page");

            return rh;
        }

        /// <summary>
        /// Infills dictionary by the data from Page settings.
        /// </summary>
        /// <param name="container">Pdf container of the data.</param>
        /// <param name="parentSettings">Parent page settings.</param>
        private void SetPageSettings(PdfDictionary container, PdfPageSettings parentSettings)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            //if( parentSettings == null )
            //  throw new ArgumentNullException( "parentSettings" );

            if (parentSettings == null || PageSettings.Size != parentSettings.Size)
            {
                RectangleF bounds = new RectangleF(PageSettings.Origin, PageSettings.Size);
                container[DictionaryProperties.MediaBox] = PdfArray.FromRectangle(bounds);
            }

            if (parentSettings == null || PageSettings.Rotate != parentSettings.Rotate)
            {
                int rotate = PdfSectionCollection.RotateFactor * (int)PageSettings.Rotate;
                PdfNumber angle = new PdfNumber(rotate);

                if(angle.IntValue!=0)
                container[DictionaryProperties.Rotate] = angle;
            }

            if (parentSettings == null || PageSettings.Unit != parentSettings.Unit)
            {
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                float unit = convertor.ConvertUnits(1f, PageSettings.Unit, PdfGraphicsUnit.Point);
                container[DictionaryProperties.UserUnit] = new PdfNumber(unit);
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Initialize()
        {
            m_pagesReferences = new PdfArray();
            m_section = new PdfDictionary();
            m_section.BeginSave += new SavePdfPrimitiveEventHandler(BeginSave);
            m_section.EndSave += new SavePdfPrimitiveEventHandler(EndSave);

            m_count = new PdfNumber(0);
            m_section[DictionaryProperties.Count] = m_count;
            m_section[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Pages);
            m_section[DictionaryProperties.Kids] = m_pagesReferences;
        }

        /// <summary>
        /// Gets the transition settings.
        /// </summary>
        /// <returns>Transition settings of the section.</returns>
        internal PdfPageTransition GetTransitionSettings()
        {
            if (!m_isTransitionSaved)
            {
                if (m_settings.GetTransition() == null)
                {
                    if (Document.PageSettings.GetTransition() != null)
                    {
                        m_savedTransition = (PdfPageTransition)Document.PageSettings.Transition.Clone();
                    }
                }
                else
                {
                    if (Document.PageSettings.GetTransition() == null)
                    {
                        m_savedTransition = (PdfPageTransition)m_settings.Transition.Clone();
                    }
                    else
                    {
                        m_savedTransition = new PdfPageTransition();

                        PdfPageTransition initialTransition = m_initialSettings.Transition;
                        PdfPageTransition documentTransition = Document.PageSettings.Transition;

                        // Store PageDuration.
                        bool sectionModified = initialTransition.PageDuration == m_settings.Transition.PageDuration;
                        bool documentModified = documentTransition.PageDuration == m_settings.Transition.PageDuration;

                        m_savedTransition.PageDuration = (documentModified && (!sectionModified)) ?
                            documentTransition.PageDuration : initialTransition.PageDuration;

                        // Store Dimension.
                        sectionModified = initialTransition.Dimension == m_settings.Transition.Dimension;
                        documentModified = documentTransition.Dimension == m_settings.Transition.Dimension;

                        m_savedTransition.Dimension = (documentModified && (!sectionModified)) ?
                            documentTransition.Dimension : initialTransition.Dimension;

                        // Store Direction.
                        sectionModified = initialTransition.Direction == m_settings.Transition.Direction;
                        documentModified = documentTransition.Direction == m_settings.Transition.Direction;

                        m_savedTransition.Direction = (documentModified && (!sectionModified)) ?
                            documentTransition.Direction : initialTransition.Direction;

                        // Store Motion.
                        sectionModified = initialTransition.Motion == m_settings.Transition.Motion;
                        documentModified = documentTransition.Motion == m_settings.Transition.Motion;

                        m_savedTransition.Motion = (documentModified && (!sectionModified)) ?
                            documentTransition.Motion : initialTransition.Motion;

                        // Store Scale.
                        sectionModified = initialTransition.Scale == m_settings.Transition.Scale;
                        documentModified = documentTransition.Scale == m_settings.Transition.Scale;

                        m_savedTransition.Scale = (documentModified && (!sectionModified)) ?
                            documentTransition.Scale : initialTransition.Scale;

                        // Store Style.
                        sectionModified = initialTransition.Style == m_settings.Transition.Style;
                        documentModified = documentTransition.Style == m_settings.Transition.Style;

                        m_savedTransition.Style = (documentModified && (!sectionModified)) ?
                            documentTransition.Style : initialTransition.Style;

                        // Store Duration.
                        sectionModified = initialTransition.Duration == m_settings.Transition.Duration;
                        documentModified = documentTransition.Duration == m_settings.Transition.Duration;

                        m_savedTransition.Duration = (documentModified && (!sectionModified)) ?
                            documentTransition.Duration : initialTransition.Duration;
                    }
                }
            }

            return m_savedTransition;
        }

        /// <summary>
        /// Resets crop box to the default one.
        /// </summary>
        internal void DropCropBox()
        {
            SetPageSettings(m_section, null);
            m_section[DictionaryProperties.CropBox] = m_section[DictionaryProperties.MediaBox];
        }

        /// <summary>
        /// Call two event's methods
        /// </summary>
        /// <param name="page">Added page</param>
        private void PageAddedMethod(PdfPage page)
        {
            //Create event's arguments
            PageAddedEventArgs args = new PageAddedEventArgs(page);

            OnPageAdded(args);

            PdfSectionCollection parent = Parent;

            if (parent != null)
            {
                parent.Document.Pages.OnPageAdded(args);
            }

            m_count.IntValue = Count;
        }
        #endregion

        #region Internals
        private struct PdfPageEnumerator : IEnumerator
        {
            #region Fields
            private PdfSection m_section;
            private int m_index;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Enumerator"/> class.
            /// </summary>
            /// <param name="section">The section.</param>
            internal PdfPageEnumerator(PdfSection section)
            {
                if (section == null)
                    throw new ArgumentNullException("section");

                m_section = section;
                m_index = -1;
            }
            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the current.
            /// </summary>
            public object Current
            {
                get
                {
                    CheckIndex();
                    return m_section[m_index];
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                ++m_index;

                return (m_index < m_section.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position,
            /// which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_index = -1;
            }

            #endregion

            #region Helper methods
            /// <summary>
            /// Checks the index.
            /// </summary>
            private void CheckIndex()
            {
                if (m_index < 0 || m_index >= m_section.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion

        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Catches the Save event of the dictionary.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BeginSave(object sender, SavePdfPrimitiveEventArgs e)
        {
            m_count.IntValue = Count;

            PdfDocument doc = (e.Writer.Document as PdfDocument);

            if (doc != null)
            {
                SetPageSettings(m_section, doc.PageSettings);
            }
            else
            {
                SetPageSettings(m_section, null);
            }
        }

        /// <summary>
        /// End save event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void EndSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            m_savedTransition = null;
        }
        #endregion
    }
}
