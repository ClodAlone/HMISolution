#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Xml;

using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

#if !SILVERLIGHT && !NETFX_CORE && !WP
using Syncfusion.Pdf.Xmp;
#endif

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents internal catalog of the Pdf document.
    /// </summary>
    /// <seealso cref="PdfDictionary"/> Class    
    internal class PdfCatalog : PdfDictionary
    {
        #region Fields
        /// <summary>
        /// Internal variable to store collection of sections.
        /// </summary>
        private PdfSectionCollection m_sections = null;

        /// <summary>
        /// Internal variable to store collection of attachments
        /// </summary>
        private PdfAttachmentCollection m_attachment = null;

        /// <summary>
        /// Internal variable to store viewer's preferences.
        /// </summary>
        private PdfViewerPreferences m_viewerPreferences = null;

        /// <summary>
        /// Internal variable to store catalog's names.
        /// </summary>
        private PdfCatalogNames m_names = null;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// The metadata of the stream.
        /// </summary>
        private XmpMetadata m_metadata = null;
#endif
        /// <summary>
        /// Internal variable to store form.
        /// </summary>
        private PdfForm m_form = null;

        /// <summary>
        /// Loaded form.
        /// </summary>
        private PdfLoadedForm m_loadedForm = null;

        /// <summary>
        /// Loaded document.
        /// </summary>
        private PdfLoadedDocument m_loadedDocument;
        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        /// <summary>
        /// Internal variable to store portfolio
        /// </summary>
        private PdfPortfolioInformation m_pdfPortfolio = null;

        /// <summary>
        /// No Names
        /// </summary>
        private bool m_noNames;

#if !SILVERLIGHT
        /// <summary>
        /// Internal variable to store StructTreeRoot.
        /// </summary>
        private static PdfStructTreeRoot m_structTreeRoot;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCatalog"/> class.
        /// </summary>
        internal PdfCatalog()
        {
            this[DictionaryProperties.Type] = new PdfName("Catalog");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCatalog"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="catalog">The catalog dictionary.</param>
        internal PdfCatalog(PdfLoadedDocument document, PdfDictionary catalog)
            : base(catalog)
        {
            m_loadedDocument = document;

            PdfDictionary dic = PdfCrossTable.Dereference(this[DictionaryProperties.Names]) as PdfDictionary;

            if (dic != null)
            {
                m_names = new PdfCatalogNames(dic);
            }
            else
            {
                m_noNames = true;
            }

#if !SILVERLIGHT && !NETFX_CORE && !WP
            ReadMetadata();
#endif
            FreezeChanges(this);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the viewer preferences.
        /// </summary>
        /// <value>The viewer preferences.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference. 
        /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments;
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.None;
        /// doc.ViewerPreferences.FitWindow = true;
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage;
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.None
        /// doc.ViewerPreferences.FitWindow = True
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public PdfViewerPreferences ViewerPreferences
        {
            get
            {
                return m_viewerPreferences;
            }

            set
            {
                if (m_viewerPreferences != value)
                {
                    m_viewerPreferences = value;


                    if (this[DictionaryProperties.ViewerPreferences] != null && LoadedDocument != null)
                    {
                        m_dictionary = this[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        PdfReferenceHolder tempReference = this[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        this[DictionaryProperties.ViewerPreferences] = new PdfReferenceHolder(value);
                        if (m_dictionary != null)
                        {
                            this.SetProperty(DictionaryProperties.ViewerPreferences, new PdfDictionary(m_dictionary));
                        }
                        else
                        {
                            this.SetProperty(DictionaryProperties.ViewerPreferences, tempReference);
                        }
                    }
                    else
                        this[DictionaryProperties.ViewerPreferences] = new PdfReferenceHolder(value);
                }
            }
        }

        /// <summary>
        /// Gets or setsthe Pdfportfolio
        /// </summary>
        internal PdfPortfolioInformation PdfPortfolio
        {

            get
            {
                return m_pdfPortfolio;
            }
            set
            {
                m_pdfPortfolio = value;
                this[DictionaryProperties.Collection] = new PdfReferenceHolder(m_pdfPortfolio);
            }
        }

# if !SILVERLIGHT
        /// <summary>
        /// Returns StructTreeRoot associated with the document.
        /// </summary>
        public static PdfStructTreeRoot StructTreeRoot
        {
            get
            {
                return m_structTreeRoot;
            }
        }
# endif

        /// <summary>
        /// Gets or sets the interactive form.
        /// </summary>
        /// <value>The form.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// // Creates a form
        /// PdfForm form = document.Form;
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;
        /// //Add the items to the list box
        /// listBox.Items.Add(new PdfListFieldItem("English", "English"));
        /// listBox.Items.Add(new PdfListFieldItem("French", "French"));
        /// listBox.Items.Add(new PdfListFieldItem("German", "German"));
        /// //Select the item
        /// listBox.SelectedIndex = 2;
        /// //Set the multiselect option
        /// listBox.MultiSelect = true;                    
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// ' Creates a form
        /// Dim form As PdfForm = document.Form
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// 'Add the items to the list box
        /// listBox.Items.Add(New PdfListFieldItem("English", "English"))
        /// listBox.Items.Add(New PdfListFieldItem("French", "French"))
        /// listBox.Items.Add(New PdfListFieldItem("German", "German"))
        /// 'Select the item
        /// listBox.SelectedIndex = 2
        /// 'Set the multiselect option
        /// listBox.MultiSelect = True
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        public PdfForm Form
        {
            get
            {
                return m_form;
            }

            set
            {
                if (m_form != value)
                {
                    m_form = value;
                    this[DictionaryProperties.AcroForm] = new PdfReferenceHolder(m_form);
                }
            }
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        public PdfCatalogNames Names
        {
            get
            {
                if (!m_noNames)
                {
                    CreateNamesIfNone();
                }

                return m_names;
            }
        }

        /// <summary>
        /// Gets the named destinations' dictionary.
        /// </summary>
        internal PdfDictionary Destinations
        {
            get
            {
                PdfDictionary dests = null;

                if (ContainsKey(DictionaryProperties.Dests))
                {
                    dests = PdfCrossTable.Dereference(this[DictionaryProperties.Dests]) as PdfDictionary;
                }

                return dests;
            }
        }

        /// <summary>
        /// Gets or sets the loaded form.
        /// </summary>
        internal PdfLoadedForm LoadedForm
        {
            get
            {
                return m_loadedForm;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("LoadedForm");
                }

                m_loadedForm = value;
            }
        }
        /// <summary>
        /// Gets or sets the loaded Document.
        /// </summary>
        internal PdfLoadedDocument LoadedDocument
        {
            get
            {
                return m_loadedDocument;
            }
        }
        /// <summary>
        /// Gets or sets the sections, which contain pages.
        /// </summary>
        internal PdfSectionCollection Pages
        {
            get
            {
                return m_sections;
            }

            set
            {
                if (m_sections != value)
                {
                    m_sections = value;
                    this[DictionaryProperties.Pages] = new PdfReferenceHolder(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the loaded form.
        /// </summary>
        internal PdfAttachmentCollection Attachments
        {
            get
            {
                return m_attachment;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("LoadedForm");
                }

                m_attachment = value;
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        internal XmpMetadata Metadata
        {
            get
            {
                return m_metadata;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Metadata");
                }

                m_metadata = value;
            }
        }
#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the names if there is none.
        /// </summary>
        internal void CreateNamesIfNone()
        {
            if (m_names == null)
            {
                m_names = new PdfCatalogNames();
                PdfReferenceHolder reference = new PdfReferenceHolder(m_names);
                this[DictionaryProperties.Names] = reference;
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes struct tree root.
        /// </summary>
        internal void InitializeStructTreeRoot()
        {
            if (m_structTreeRoot == null)
            {
                m_structTreeRoot = new PdfStructTreeRoot();
                this[DictionaryProperties.StructTreeRoot] = new PdfReferenceHolder(m_structTreeRoot);
            }
        }

        /// <summary>
        /// Reads Xmp from the document.
        /// </summary>
        private void ReadMetadata()
        {
            //Read metadata if present.
            IPdfPrimitive rhMetadata = this[DictionaryProperties.Metadata];

            PdfStream xmpStream = PdfCrossTable.Dereference(rhMetadata) as PdfStream;

            if (xmpStream != null)
            {
                if (xmpStream.Compress)
                {
                    xmpStream.Decompress();
                }

                MemoryStream ms = new MemoryStream(xmpStream.Data);

                XmlDocument xmp = new XmlDocument();
                try
                {
                    xmp.Load(ms);
                }
                catch (XmlException e)
                {
                    xmpStream.Decompress();
                    ms = new MemoryStream(xmpStream.Data);
                    try
                    {
                        xmp.Load(ms);
                    }
                    catch(XmlException e1)
                    {
                        return;
                    }
                }

                m_metadata = new XmpMetadata(xmp);
                this.LoadedDocument.DublinSchema = m_metadata.DublinCoreSchema;
                m_metadata.isLoadedDocument = true;
            }
        }
#endif

        /// <summary>
        /// Applies the PDF/X Conformance attributes to the document.
        /// </summary>
        internal void ApplyPdfXConformance()
        {
            PdfDictionary outputIntents = new PdfDictionary();
            outputIntents[DictionaryProperties.S] = new PdfName(DictionaryProperties.GTS_PDFX);
            outputIntents[DictionaryProperties.OutputConditionIdentifier] = new PdfString("CGATS TR 001");
            outputIntents[DictionaryProperties.Info] = new PdfString(string.Empty);
            outputIntents[DictionaryProperties.OutputCondition] = new PdfString("SWOP CGATS TR 001-1995");
            outputIntents[DictionaryProperties.Type] = new PdfName(DictionaryProperties.OutputIntent);
            outputIntents[DictionaryProperties.RegistryName] = new PdfString("http://www.color.org");

            PdfArray array = new PdfArray();
            array.Insert(0, outputIntents);

            this[DictionaryProperties.OutputIntents] = array;
        }

        /// <summary>
        /// Clear PdfCatalog
        /// </summary>
        internal void Clear()
        {
            if (m_names != null)
            {
                m_names.Clear();
                m_names = null;
            }
            if (m_viewerPreferences != null)
            {
                m_viewerPreferences = null;
                m_viewerPreferences = null;
            }
            if (m_attachment != null)
            {
                m_attachment.Clear();
                m_attachment = null;
            }
#if !SILVERLIGHT
            if (m_structTreeRoot != null)
            {
                m_structTreeRoot.Clear();
                m_structTreeRoot = null;
            }
# endif

            m_form = null;
            m_loadedDocument = null;
            m_loadedForm = null;
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_metadata = null;
# endif
            m_sections = null;

            base.Clear();
        }
        #endregion
    }
}
