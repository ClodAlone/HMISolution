#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
#if NETFX_CORE || WP
using System.Threading.Tasks;
using Windows.Storage;
#endif
#if !SILVERLIGHT && !NETFX_CORE && !WP
using System.Drawing.Text;
#endif


/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents a logic to create Pdf document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font with Bold font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
    /// //Save the document.
    /// document.Save("Document.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font with Bold font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Draw text in the new page.
    /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
    /// 'Save the document.
    /// document.Save("Document.pdf");
    /// </code>
    /// </example>    
    /// <seealso cref="PdfFont"/> Class    
    /// <seealso cref="PdfPage"/> Class   
    /// <seealso cref="PdfDocumentBase"/> Class   
    /// <seealso cref="PdfLoadedDocument"/> Class    
    public class PdfDocument : PdfDocumentBase
    {
        #region Constants
        /// <summary>
        /// Default margin value.
        /// </summary>
        internal const float DefaultMargin = 40f;

        /// <summary>
        /// Font used in complex objects to draw strings and text when it is not defined explicitly.
        /// </summary>
        private static PdfFont s_defaultFont = null;
        #endregion

        #region Fields
        /// <summary>
        /// Cache of the objects.
        /// </summary>
        private static readonly PdfCacheCollection s_cache;

        /// <summary>
        /// Helps to lock s_cache to avoid race conditions.
        /// </summary>
        private static object s_cacheLock = new object();

        /// <summary>
        /// Internal variable to store template which is applied to each page of the document.
        /// </summary>
        private PdfDocumentTemplate m_pageTemplate;

        /// <summary>
        /// Internal variable to store document's collection of attachments.
        /// </summary>
        private PdfAttachmentCollection m_attachments;

        /// <summary>
        /// Internal variable to store document's collection of pages.
        /// </summary>
        private PdfDocumentPageCollection m_pages;

        /// <summary>
        /// Indicates whether the document was Pdf Viewer document.
        /// </summary>
        private bool m_isPdfViewerDocumentDisable = true;

        /// <summary>
        /// Internal variable to store document's collection of sections.
        /// </summary>
        private PdfSectionCollection m_sections;

        /// <summary>
        /// Default page settings.
        /// </summary>
        private PdfPageSettings m_settings;

        /// <summary>
        /// Root outline.
        /// </summary>
        private PdfBookmarkBase m_outlines;

        /// <summary>
        /// Indicates if the page labels were set.
        /// </summary>
        private bool m_bPageLabels = false;

        /// <summary>
        /// Indicates whether the document was encrypted or not.
        /// </summary>
        private bool m_bWasEncrypted = false;

        /// <summary>
        /// Internal variable to store additional document's actions.
        /// </summary>
        private PdfDocumentActions m_actions = null;

        /// <summary>
        /// Defines the color space of the document
        /// </summary>
        private Syncfusion.Pdf.Graphics.PdfColorSpace m_colorSpace;

        /// <summary>
        /// The delegade of the progress event handler.
        /// </summary>
        private ProgressEventHandler m_progressDelegade;

        /// <summary>
        /// Local Variable to store the Conformance Level.
        /// </summary>
        internal static PdfConformanceLevel ConformanceLevel;
        /// <summary>
        /// Internal variable to store OCG groups.
        /// </summary>
        internal PdfArray primitive = new PdfArray();
        /// <summary>
        /// Internal variable to store position.
        /// </summary>
        internal int m_positon = 0;
        /// <summary>
        /// Internal variable to store order position.
        /// </summary>
        internal int m_orderposition = 0;
        /// <summary>
        /// Internal variable to store  on position.
        /// </summary>
        internal int m_onpositon = 0;
        /// <summary>
        /// Internal variable to store  off position.
        /// </summary>
        internal int m_offpositon = 0;
        /// <summary>
        /// Internal variable to store layer order.
        /// </summary>
        internal PdfArray m_order = new PdfArray();
        /// <summary>
        /// Internal variable to store visible layers.
        /// </summary>
        internal PdfArray m_on = new PdfArray();
        /// <summary>
        /// Internal variable to store invisible layers.
        /// </summary>
        internal PdfArray m_off = new PdfArray();
        /// <summary>
        /// Internal variable to store Sub Layers.
        /// </summary>
        internal PdfArray m_sublayer = new PdfArray();
        /// <summary>
        /// Internal variable to store Sub Layers Position.
        /// </summary>
        internal int m_sublayerposition;

        /// <summary>
        /// Indicates whether enable cache or not
        /// </summary>
        private static bool m_enableCache=true;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Internal variable to store the private font collection.
        /// </summary>
        internal static PrivateFontCollection m_privateFonts;
#endif

        #endregion

        #region Constructors
        static PdfDocument()
        {
            s_cache = new PdfCacheCollection();
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_privateFonts = new PrivateFontCollection();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocument"/> class.
        /// </summary>        
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font with Bold font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// //Save the document.
        /// document.Save("Document.pdf");;
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font with Bold font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// 'Save the document.
        /// document.Save("Document.pdf");
        /// </code>
        /// </example>    
        /// <seealso cref="PdfFont"/> Class    
        /// <seealso cref="PdfPage"/> Class                   
        public PdfDocument()
            :this(false)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocument"/> class.
        /// </summary>
        /// <param name="isMerging"></param>
        internal PdfDocument(bool isMerging)
        {
            if (PdfDocument.IsSecurityGranted)
            {
                PdfDocument.ValidateLicense();
            }

            PdfMainObjectCollection objects = new PdfMainObjectCollection();
            SetMainObjectCollection(objects);

            PdfCrossTable crossTable = new PdfCrossTable();
            crossTable.IsMerging = isMerging;
            crossTable.Document = this;
            SetCrossTable(crossTable);

            PdfCatalog catalog = new PdfCatalog();
            SetCatalog(catalog);
            objects.Add(catalog);

            if (!isMerging)
                catalog.Position = -1;

            m_sections = new PdfSectionCollection(this);
            m_pages = new PdfDocumentPageCollection(this);
            catalog.Pages = m_sections;
        }


#if !SILVERLIGHT && !NETFX_CORE && !WP
#if AllowUnsafeCode
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocument"/> class.
        /// </summary>
        /// <param name="conformance">The conformance level.</param>
        /// <remarks>Not Supported under Medium Trust environment.</remarks>        
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A1B);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfTrueTypeFont(new Font(FontFamily.GenericMonospace, 12f, FontStyle.Bold));
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// //Saves the document
        /// document.Save("Document.pdf");;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument(PdfConformanceLevel.Pdf_A1B)
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfTrueTypeFont(New Font(FontFamily.GenericMonospace, 12f, FontStyle.Bold))
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// 'Saves the document
        /// document.Save("Document.pdf");
        /// </code>
        /// </example>
        /// <seealso cref="PdfConformanceLevel"/> Enumeration
        public PdfDocument(PdfConformanceLevel conformance)
            : this()
#else
        internal PdfDocument(PdfConformanceLevel conformance) : this()
#endif
        {
            ConformanceLevel = conformance;
            if (Conformance == PdfConformanceLevel.Pdf_A1B)
            {
                //Note : Activate XMP - Explicit Activation Needed.
                //base.DocumentInformation.XmpMetadata.ToString();

                //Note : PDF/A is based on Pdf 1.4.  Hence it does not support cross reference
                //stream which is an Pdf 1.5 feature.
                base.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceTable;
                base.FileStructure.Version = PdfVersion.Version1_4;

                //Embed the ColorProfie
                SetDocumentColorProfile();
            }
            else if (conformance == PdfConformanceLevel.Pdf_X1A2001)
            {
                //Note : Activate XMP - Explicit Activation Needed.
                base.FileStructure.Version = PdfVersion.Version1_3;
                base.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceTable;

                base.DocumentInformation.XmpMetadata.ToString();
                base.DocumentInformation.ApplyPdfXConformance();

                Catalog.ApplyPdfXConformance();
            }
        }
#endif
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate for the <see cref="ProgressEventHandler"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arguments">The arguments.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Add new pages to the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// document.SaveProgress += new PdfDocument.ProgressEventHandler(document_SaveProgress);
        /// //Saves the document
        /// document.Save("Document.pdf");;
        /// //  Handles the event
        /// void document_SaveProgress(object sender, ProgressEventArgs arguments)
        /// {
        ///   MessageBox.Show(String.Format("Current: {0}, Progress: {1}, Total {2}", arguments.Current, arguments.Progress, arguments.Total));
        /// }
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Private document As PdfDocument = New PdfDocument()
        /// 'Add new pages to the document.
        /// Private page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Private font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// AddHandler document.SaveProgress, AddressOf document_SaveProgress
        /// 'Saves the document
        /// document.Save("Document.pdf");
        /// '  Handles the event
        /// Private Sub document_SaveProgress(ByVal sender As Object, ByVal arguments As ProgressEventArgs)
        ///  MessageBox.Show(String.Format("Current: {0}, Progress: {1}, Total {2}", arguments.Current, arguments.Progress, arguments.Total))
        /// End Sub
        /// </code>
        /// </example>
        public delegate void ProgressEventHandler(object sender, ProgressEventArgs arguments);
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the document is being saved.
        /// </summary>
        /// <remarks>
        /// This event raised on saving the document. It will keep track of the save progress of the document.
        /// </remarks> 
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Add new pages to the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, new PointF(10, 10));
        /// document.SaveProgress += new PdfDocument.ProgressEventHandler(document_SaveProgress);
        /// //Saves the document
        /// document.Save("Document.pdf");;
        /// // Event handler for PageAdded event
        /// void document_SaveProgress(object sender, ProgressEventArgs arguments)
        /// {
        ///   MessageBox.Show(String.Format("Current: {0}, Progress: {1}, Total {2}", arguments.Current, arguments.Progress, arguments.Total));
        /// }
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Private document As PdfDocument = New PdfDocument()
        /// 'Add new pages to the document.
        /// Private page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Private font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", font, PdfBrushes.Black, New PointF(10, 10))
        /// AddHandler document.SaveProgress, AddressOf document_SaveProgress
        /// 'Saves the document
        /// document.Save("Document.pdf");
        /// ' Event handler for PageAdded event
        /// Private Sub document_SaveProgress(ByVal sender As Object, ByVal arguments As ProgressEventArgs)
        ///  MessageBox.Show(String.Format("Current: {0}, Progress: {1}, Total {2}", arguments.Current, arguments.Progress, arguments.Total))
        /// End Sub
        /// </code>
        /// </example>
        /// <seealso cref="ProgressEventHandler"/> Delegate
        public event ProgressEventHandler SaveProgress
        {
            add
            {
                m_progressDelegade = Delegate.Combine(m_progressDelegade, value) as ProgressEventHandler;

                if (m_progressDelegade != null)
                {
                    SetProgress();
                }
            }

            remove
            {
                m_progressDelegade = Delegate.Remove(m_progressDelegade, value) as ProgressEventHandler;

                if (m_progressDelegade == null)
                {
                    ResetProgress();
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a template that is applied to all pages in the document.
        /// </summary>        
        /// <example>
        /// <code lang="C#">
        /// //Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// RectangleF rect = new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height);
        /// //Creates a new page and adds it as the last page of the document template
        /// PdfPageTemplateElement footer = new PdfPageTemplateElement(rect);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Gray);
        /// //Create page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);
        /// //Create page count field
        /// PdfPageCountField count = new PdfPageCountField(font, brush);
        /// PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);
        /// compositeField.Bounds = footer.Bounds;
        /// compositeField.Draw(footer.Graphics, new PointF(40, footer.Height - 50));          
        /// //Add the footer template at the bottom
        /// doc.Template.Bottom = footer;
        /// doc.Save("Template.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim rect As RectangleF = New RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height)
        /// 'Create a page template
        /// Dim footer As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Gray)
        /// 'Create page number field
        /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font, brush)
        /// 'Create page count field
        /// Dim count As PdfPageCountField = New PdfPageCountField(font, brush)
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count)
        /// compositeField.Bounds = footer.Bounds
        /// compositeField.Draw(footer.Graphics, New PointF(40, footer.Height - 50))
        /// 'Add the footer template at the bottom
        /// doc.Template.Bottom = footer
        /// doc.Save("Template.pdf")
        /// </code>
        /// </example>
        /// <value>The <see cref="PdfDocumentTemplate"/> specifying the default template for the document.</value>
        /// <seealso cref="PdfTemplate"/> Class
        public PdfDocumentTemplate Template
        {
            get
            {
                if (m_pageTemplate == null)
                {
                    m_pageTemplate = new PdfDocumentTemplate();
                }

                return m_pageTemplate;
            }

            set
            {
                m_pageTemplate = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document was PDF viewer document.
        /// </summary>
        internal override bool IsPdfViewerDocumentDisable
        {
            get
            {
                return m_isPdfViewerDocumentDisable;
            }
            set
            {
                m_isPdfViewerDocumentDisable = value;
            }
        }
        /// <summary>
        /// Gets the additional document's actions.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document
        /// PdfDocument document = new PdfDocument();
        /// //Create and add new launch Action to the document
        /// PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        /// document.Actions.AfterOpen = action;
        /// //Save the document
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create and add new launch Action to the document.
        /// Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        /// document.Actions.AfterOpen = action
        /// 'Save the document
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        /// <value>The <see cref="PdfDocumentActions"/> specifying the document action.</value>
        /// <seealso cref="PdfLaunchAction"/> Class
        public PdfDocumentActions Actions
        {
            get
            {
                if (m_actions == null)
                {
                    m_actions = new PdfDocumentActions(Catalog);
                    Catalog[DictionaryProperties.AA] = (m_actions as IPdfWrapper).Element;
                }

                return m_actions;
            }
        }

        /// <summary>
        /// Gets the collection of the pages in the document.
        /// </summary>
        /// <value>A <see cref="PdfDocumentPageCollection"/> object containing the list of document's pages. </value>
        /// <remarks>This collection is exposed for usability purposes only.
        /// The pages are contained in the sections. So, sections should be used for pages manipulating.</remarks>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument(); 
        /// // Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Page collection
        /// PdfDocumentPageCollection pageCollection = doc.Pages;
        /// PdfFont standardFont = new PdfStandardFont(PdfFontFamily.TimesRoman,10);  
        /// // Drawing string on first page
        /// pageCollection[0].Graphics.DrawString("FirstPage", standardFont, PdfBrushes.Black, new PointF(10, 10));
        /// doc.Save("Pages.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Page collection
        /// Dim pageCollection As PdfDocumentPageCollection = doc.Pages
        /// Dim standardFont As PdfFont = New PdfStandardFont(PdfFontFamily.TimesRoman,10)
        /// ' Drawing string on first page
        /// pageCollection(0).Graphics.DrawString("FirstPage", standardFont, PdfBrushes.Black, New PointF(10, 10))
        /// doc.Save("Pages.pdf")
        /// </code>
        /// </example>        
        public PdfDocumentPageCollection Pages
        {
            get
            {
                return m_pages;
            }
        }

        /// <summary>
        /// Gets the collection of the sections in the document.
        /// </summary>  
        /// <value>A <see cref="PdfSectionCollection"/> object containing the list of document's sections.
        /// <example>
        /// <code lang="C#">
        /// //Create a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// // Create a new section
        /// PdfSection mySection = doc.Sections.Add();
        /// //Creates a new page and adds it as the last page of the section
        /// mySection.Pages.Add();
        /// // Gets the section collection
        /// PdfSectionCollection sectionCollection = doc.Sections;
        /// // Gets the first page from first section
        /// PdfPage page = sectionCollection[0].Pages[0];
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
        /// // Draw the string in first page of the section
        /// page.Graphics.DrawString("Hello World", font, PdfBrushes.Black, new Point(100, 100));
        /// doc.Save("Sections.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a new section
        /// Dim mySection As PdfSection = doc.Sections.Add()
        /// 'Creates a new page and adds it as the last page of the section
        /// mySection.Pages.Add()
        /// ' Gets the section collection
        /// Dim sectionCollection As PdfSectionCollection = doc.Sections
        /// ' Gets the first page from first section
        /// Dim page As PdfPage = sectionCollection(0).Pages(0)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12)
        /// ' Draw the string in first page of the section
        /// page.Graphics.DrawString("Hello World", font, PdfBrushes.Black, New Point(100, 100))
        /// doc.Save("Sections.pdf")
        /// </code>
        /// </example>      
        public PdfSectionCollection Sections
        {
            get
            {
                return m_sections;
            }
        }

        /// <summary>
        /// Gets or sets page settings of the document's sections.
        /// </summary>
        /// <example>
        /// <value>A <see cref="PdfPageSettings"/> object containing the setting values for a document`s pages.</value>
        /// <code lang="C#">
        /// // Create a new document class object.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set page size
        /// doc.PageSettings.Size = PdfPageSize.A6;
        /// //Set page orientation
        /// doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
        /// doc.Save("PageSettings.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document class object.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set page size
        /// doc.PageSettings.Size = PdfPageSize.A6
        /// 'Set page orientation
        /// doc.PageSettings.Orientation = PdfPageOrientation.Landscape
        /// doc.Save("PageSettings.pdf")
        /// </code>
        /// </example>
        /// <remarks>The changing of the settings doesn't take any effect on the already existing pages.</remarks>
        public PdfPageSettings PageSettings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = new PdfPageSettings(DefaultMargin);
                }

                return m_settings;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("PageSettings");
                }

                m_settings = value;
            }
        }

        /// <summary>
        /// Gets the root of the bookmark tree in the document.
        /// </summary>
        /// <value>A <see cref="PdfBookmarkBase"/> object specifying the document's bookmarks. </value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Create outline
        /// PdfBookmark bookMark = document.Bookmarks.Add("InteractiveFeature");
        /// bookMark.Color = Color.DarkBlue;
        /// bookMark.TextStyle = PdfTextStyle.Bold;
        /// bookMark.Title = "Interactive Feature";
        /// bookMark.Destination = new PdfDestination(page);
        /// doc.Save("Bookmarks.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Create outline
        /// Dim bookMark As PdfBookmark = document.Bookmarks.Add("InteractiveFeature")
        /// bookMark.Color = Color.DarkBlue
        /// bookMark.TextStyle = PdfTextStyle.Bold
        /// bookMark.Title = "Interactive Feature"
        /// bookMark.Destination = New PdfDestination(page)
        /// doc.Save("Bookmarks.pdf")
        /// </code>
        /// </example>
        /// <remarks>Creates an bookmark root instance
        /// if it's called for first time.</remarks>
        public override PdfBookmarkBase Bookmarks
        {
            get
            {
                if (m_outlines == null)
                {
                    m_outlines = new PdfBookmarkBase();
                    Catalog[DictionaryProperties.Outlines] = new PdfReferenceHolder(m_outlines);
                }

                return m_outlines;
            }
        }

        /// <summary>
        /// Gets the attachments of the document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// // Creates a new page in document
        /// PdfPage page = doc.Pages.Add();
        /// // Adding an image as attachment
        /// PdfAttachment attachment = new PdfAttachment("Logo.jpg");
        /// attachment.Description = "Syncfusion Logo";
        /// attachment.MimeType = "application/jpeg";
        /// // Add the attachment in document
        /// doc.Attachments.Add(attachment);
        /// doc.Save("Attachment.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Creates a new page in document
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Adding an image as attachment
        /// Dim attachment As PdfAttachment = New PdfAttachment("Logo.jpg")
        /// attachment.Description = "Syncfusion Logo"
        /// attachment.MimeType = "application/jpeg"
        /// ' Add the attachment in document
        /// doc.Attachments.Add(attachment)
        /// doc.Save("Attachment.pdf")
        /// </code>
        /// </example>
        /// <value>The <see cref="PdfAttachmentCollection"/> object contains list of files which are attached in the PDF document.</value>
        /// <seealso cref="PdfAttachment"/> Class.
        public PdfAttachmentCollection Attachments
        {
            get
            {
                if (m_attachments == null)
                {
                    m_attachments = new PdfAttachmentCollection();
                    Catalog.Names.EmbeddedFiles = m_attachments;
                }

                return m_attachments;
            }
        }

        /// <summary>
        /// Gets the interactive form of the document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Create a document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");                    
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;            
        /// //Add the textbox in form
        /// document.Form.Fields.Add(firstNameTextBox);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")        
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// 'Add the textbox in form
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <value>The <see cref="PdfForm"/> object contains the list of form elements of the document.</value>
        public PdfForm Form
        {
            get
            {
                if (Catalog.Form == null)
                {
                    Catalog.Form = new PdfForm();
                }

                return Catalog.Form;
            }
        }

        /// <summary>
        /// Gets or sets the color space of the document.
        /// </summary>
        /// <remarks>This property has impact on the new created pages only.
        /// If a page was created it remains its colour space obliviously
        /// to this property changes.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //  Set the document`s color spaces as GrayScale 
        /// doc.ColorSpace = PdfColorSpace.GrayScale;
        /// doc.Save("ColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s color spaces as GrayScale 
        /// doc.ColorSpace = PdfColorSpace.GrayScale
        /// doc.Save("ColorSpace.pdf")
        /// </code>
        /// </example>
        /// <value>The <see cref="PdfColorSpace"/> of the document.</value>
        public Syncfusion.Pdf.Graphics.PdfColorSpace ColorSpace
        {
            get
            {
                if ((m_colorSpace == PdfColorSpace.RGB) || ((m_colorSpace == PdfColorSpace.CMYK) || (m_colorSpace == PdfColorSpace.GrayScale)))
                {
                    return m_colorSpace;
                }
                else
                {
                    return PdfColorSpace.RGB;
                }
            }

            set
            {
                if ((value == PdfColorSpace.RGB) || ((value == PdfColorSpace.CMYK) || (value == PdfColorSpace.GrayScale)))
                {
                    m_colorSpace = value;
                }
                else
                {
                    m_colorSpace = PdfColorSpace.RGB;
                }
            }
        }

        /// <summary>
        /// Gets collection of the cached objects.
        /// </summary>
        internal static PdfCacheCollection Cache
        {
            get
            {
                lock (s_cacheLock)
                {
                    if (s_cache == null)
                    {
                        return new PdfCacheCollection();
                        //throw new Exception();
                    }

                    return s_cache;
                }
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the Private Font Collection
        /// </summary>
        internal static PrivateFontCollection PrivateFonts
        {
            get
            {
                if (m_privateFonts == null)
                {
                    m_privateFonts = new PrivateFontCollection();
                }
                return m_privateFonts;
            }
        }
#endif
        /// <summary>
        /// Gets the default font. It is used for complex objects when font is 
        /// not explicitly defined.
        /// </summary>
        /// <value>The default font.</value>
        internal static PdfFont DefaultFont
        {
            get
            {
                lock (s_cacheLock)
                {
                    if (s_defaultFont == null)
                    {
                        s_defaultFont = //new PdfTrueTypeFont( new Font( "Times New Roman", 12 ), true );
                            new PdfStandardFont(PdfFontFamily.Helvetica, 8);
                    }
                }

                return s_defaultFont;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document was encrypted.
        /// </summary>
        /// <value><c>true</c> if the document was encrypted; otherwise, <c>false</c>.</value>
        internal override bool WasEncrypted
        {
            get
            {
                return m_bWasEncrypted;
            }
        }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
# if NETFX_CORE || WP
        public override int PageCount
#else
        internal override int PageCount
#endif
        {
            get { return Pages.Count; }
        }

        /// <summary>
        /// Gets or Sets the Pdf Conformance level.
        /// Supported : PDF/A-1b - Level B compliance in Part 1
        /// </summary>
        /// <value>The <see cref="PdfConformanceLevel"/>.</value>
        /// <remarks>Not Supported under Medium Trust environment.</remarks>
        /// <example>
        /// <remarks>Default value is None.</remarks>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A1B);
        /// //Creates a new page and adds it as the last page of the document to the document.
        /// PdfPage page = document.Pages.Add();
        /// // Create a 'Times New Roman' font
        /// Font font = new Font("Times New Roman", 10);
        /// // Create font with bold font style.
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", pdfFont, PdfBrushes.Black, new PointF(10, 10));
        /// //Save document to disk.
        /// document.Save("ConformanceLevel.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument(PdfConformanceLevel.Pdf_A1B)
        /// ' Create a page to the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// ' Create a 'Times New Roman' font
        /// Dim font As Font = New Font("Times New Roman", 10)
        /// ' Create font with bold font style.
        /// Dim pdfFont As PdfFont = New PdfTrueTypeFont(font, False)
        /// 'Draw text in the new page.
        /// page.Graphics.DrawString("Essential PDF", pdfFont, PdfBrushes.Black, New PointF(10, 10))
        /// 'Save document to disk.
        /// document.Save("ConformanceLevel.pdf")
        /// </code>
        /// </example>
#if AllowUnsafeCode
        public PdfConformanceLevel Conformance
#else
        internal PdfConformanceLevel Conformance
#endif
        {
            get
            {
                return ConformanceLevel;
            }
        }
        #endregion

        #region Public methods

#if NETFX_CORE || WP

        /// <summary>
        /// Save the document to a stream in Asynchronous mode.
        /// </summary>
        /// <param name="stream">Stream for saving the PDF document.</param>
        /// <returns></returns>
        public async Task<bool> SaveAsync(Stream stream)
        {

            // Check if the storage file is valid.
            if (stream == null)
            {
                throw new ArgumentNullException("InvalidFile");
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    Save(stream);
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

#endif
        /// <summary>
        /// Saves the document to the specified stream.
        /// </summary>
        /// <param name="stream">The stream object where PDF document will be saved.</param>    
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Create Pdf graphics for the page
        /// PdfGraphics g = page.Graphics;
        /// // Loads an Image            
        /// PdfImage pdfImg = new PdfBitmap(Image.FromFile("Logo.png")); 
        /// //Draw the image
        /// g.DrawImage(pdfImg, 20, 20, 100, 200);
        /// // Save the document as a stream
        /// MemoryStream stream = new MemoryStream();
        /// doc.Save(stream);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Create Pdf graphics for the page
        /// Dim g As PdfGraphics = page.Graphics
        /// ' Loads an Image            
        /// Dim pdfImg As PdfImage = New PdfBitmap(Image.FromFile("Logo.png"))
        /// 'Draw the image
        /// g.DrawImage(pdfImg, 20, 20, 100, 200)
        /// ' Save the document as a stream
        /// Dim stream As MemoryStream = New MemoryStream()
        /// doc.Save(stream)
        /// </code>
        /// </example>
        public override void Save(Stream stream)
        {

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            CheckPagesPresence();

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (Conformance == PdfConformanceLevel.Pdf_A1B)
                base.DocumentInformation.XmpMetadata.ToString();
#endif
            PdfWriter writer = new PdfWriter(stream);
            writer.Document = this;

            // Clean out outlines if there were none.
            if (m_outlines != null && m_outlines.Count < 1)
            {
                Catalog.Remove(DictionaryProperties.Outlines);
            }

            // If PDF has been marked as tagged, few more entries should be added to the dictionary.
            if (FileStructure.TaggedPdf)
            {
                Catalog[DictionaryProperties.Lang] = new PdfString("en");

                PdfDictionary dic = null;
                if (!Catalog.ContainsKey(DictionaryProperties.MarkInfo))
                    Catalog[DictionaryProperties.MarkInfo] = new PdfDictionary();

                dic = Catalog[DictionaryProperties.MarkInfo] as PdfDictionary;
                dic[DictionaryProperties.Marked] = new PdfBoolean(true);
            }

            ProcessPageLabels();

            CrossTable.Save(writer);

            if (m_progressDelegade != null)
            {
                int count = Pages.Count;
                ProgressEventArgs arg = new ProgressEventArgs(count, count);
                OnSaveProgress(arg);
            }

            DocumentSavedEventArgs argsSaved = new DocumentSavedEventArgs(writer);
            OnDocumentSaved(argsSaved);

            PdfDocument.ConformanceLevel = PdfConformanceLevel.None;

            writer.Close();
        }

#if _MVC
        /// <summary>
        /// Saves as action result.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="response">The response.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public PdfResult SaveAsActionResult(string fileName, System.Web.HttpResponse response, HttpReadType type)
        {
            return new PdfResult(this, fileName, response, type);
        }
#endif

        /// <summary>
        /// Closes the document.
        /// </summary>
        /// <param name="completely">if set to <c>true</c> the document should be disposed completely.</param>
        /// <remarks>The document is disposed after calling the Close method. So, the document can't be saved if Close method was invoked.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();            
        /// //Create Pdf graphics for the page
        /// PdfGraphics g = page.Graphics;                        
        /// //Create a solid brush
        /// PdfBrush brush = new PdfSolidBrush(Color.Black);          
        /// float fontSize = 20f;
        /// //Set the font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);            
        /// //Draw the text
        /// g.DrawString("Hello world!", font, brush,new PointF(20,20));           
        /// doc.Save("Sample.pdf");
        /// // Closes the document.
        /// doc.Close(true);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Create Pdf graphics for the page
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a solid brush
        /// Dim brush As PdfBrush = New PdfSolidBrush(Color.Black)
        /// Dim fontSize As Single = 20f
        /// 'Set the font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
        /// 'Draw the text
        /// g.DrawString("Hello world!", font, brush,New PointF(20,20))
        /// doc.Save("Sample.pdf")
        /// ' Closes the document.
        /// doc.Close(True)
        /// </code>
        /// </example>                
        public override void Close(bool completely)
        {
            if (completely && Form != null && EnableMemoryOptimization)
                Form.Clear();

            if (completely && EnableMemoryOptimization)
            {
                m_off = null;
                m_on = null;
                m_order = null;
                if (m_outlines != null)
                    m_outlines.Clear();
                m_progressDelegade = null;
                m_sublayer = null;

                if (m_pages != null)
                    m_pages.Clear();

                if (m_sections != null)
                    m_sections.Clear();

                s_defaultFont = null;
            }

            base.Close(completely);

            // Close all associated non-memory resources.
            PdfDocument.ConformanceLevel = PdfConformanceLevel.None;
            m_pageTemplate = null;
            m_attachments = null;
            m_pages = null;
            m_sections = null;
            m_settings = null;
            m_outlines = null;
            m_bPageLabels = false;
            m_bWasEncrypted = false;
            m_actions = null;
            //GC.Collect();

            //PdfDocument.Cache.Clear();

            //GC.Collect();

#if !SILVERLIGHT && ! NETFX_CORE && !WP
            m_privateFonts = null;
# endif

            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <value>A new object that is a copy of this instance.</value>        
        /// <remarks>The resulting clone must be of the same type as or a compatible type to the original instance.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();            
        /// //Create Pdf graphics for the page
        /// PdfGraphics g = page.Graphics;                        
        /// //Create a solid brush
        /// PdfBrush brush = new PdfSolidBrush(Color.Black);          
        /// float fontSize = 20f;
        /// //Set the font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);            
        /// //Draw the text
        /// g.DrawString("Hello world!", font, brush,new PointF(20,20));           
        /// // Cloning the document
        /// PdfDocument cloneDoc = doc.Clone();
        /// cloneDoc.Save("Clone.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Create Pdf graphics for the page
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a solid brush
        /// Dim brush As PdfBrush = New PdfSolidBrush(Color.Black)
        /// Dim fontSize As Single = 20f
        /// 'Set the font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
        /// 'Draw the text
        /// g.DrawString("Hello world!", font, brush,New PointF(20,20))
        /// ' Cloning the document
        /// Dim cloneDoc As PdfDocument = doc.Clone()
        /// cloneDoc.Save("Clone.pdf")
        /// </code>
        /// </example>
        public object Clone()
        {
            PdfDictionary dic = CrossTable.EncryptorDictionary;
            if (dic != null)
            {
                throw new ArgumentException("Can't clone the Encrypted document");
            }

            return this.MemberwiseClone();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(PdfConfig));
#else
                new Syncfusion.Core.Licensing.LicensedWebComponent(typeof(PdfConfig));
#endif
            }
            finally
            {
                GC.Collect();
                GC.SuppressFinalize(new Syncfusion.Core.Licensing.LicensedComponent(typeof(PdfConfig)));
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif
        }

        /// <summary>
        /// Called when a page is saved.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void OnPageSave(PdfPage page)
        {
            if (m_progressDelegade != null)
            {
                int index = Pages.IndexOf(page);
                int count = Pages.Count;
                ProgressEventArgs pea = new ProgressEventArgs(index, count);

                OnSaveProgress(pea);
            }
        }

        /// <summary>
        /// Informs the document that the page labels were set.
        /// </summary>
        internal void PageLabelsSet()
        {
            m_bPageLabels = true;
        }

        /// <summary>
        /// Raises the <see cref="E:Progress"/> event.
        /// </summary>
        /// <param name="arguments">The <see cref="T:Syncfusion.Pdf.ProgressEventArgs"/>
        /// instance containing the event data.</param>
        protected virtual void OnSaveProgress(ProgressEventArgs arguments)
        {
            if (m_progressDelegade != null)
            {
                m_progressDelegade(this, arguments);
            }
        }

        /// <summary>
        /// Checks the pages presence.
        /// </summary>
        private void CheckPagesPresence()
        {
            if (Pages.Count == 0)
            {
                Pages.Add();
            }
        }

        public static bool EnableCache
        {
            get
            {
                return m_enableCache;
            }

            set
            {
                m_enableCache = value;
            }
        }
        /// <summary>
        /// Processes the page labels.
        /// </summary>
        private void ProcessPageLabels()
        {
            if (m_bPageLabels)
            {
                PdfDictionary labelsDic = Catalog[DictionaryProperties.PageLabels] as PdfDictionary;

                if (labelsDic == null)
                {
                    labelsDic = new PdfDictionary();
                    Catalog[DictionaryProperties.PageLabels] = labelsDic;
                }

                PdfArray labels = new PdfArray();
                labelsDic[DictionaryProperties.Nums] = labels;

                int pageIndex = 0;

                foreach (PdfSection section in Sections)
                {
                    PdfPageLabel label = section.PageLabel;

                    if (label == null)
                    {
                        label = new PdfPageLabel();
                    }

                    labels.Add(new PdfNumber(pageIndex));
                    labels.Add(((IPdfWrapper)label).Element);
                    pageIndex += section.Count;
                }
            }
        }

        /// <summary>
        /// Resets the progress mechanism.
        /// </summary>
        private void ResetProgress()
        {
            Sections.ResetProgress();
        }

        /// <summary>
        /// Sets the progress mechanism.
        /// </summary>
        private void SetProgress()
        {
            Sections.SetProgress();
        }

        /// <summary>
        /// Embeds the RGB ICC ColorProfile to the document to attain PDF/A conformance.
        /// </summary>
        private void SetDocumentColorProfile()
        {
            PdfDictionary dict = new PdfDictionary();
            dict["Info"] = new PdfString("sRGB IEC61966-2.1");
            dict["S"] = new PdfName("GTS_PDFA1");
            dict["OutputConditionIdentifier"] = new PdfString("custom");
            dict["Type"] = new PdfName("OutputIntent");
            dict["OutputCondition"] = new PdfString("");
            dict["RegistryName"] = new PdfString("");

            PdfICCColorProfile srgbProfile = new PdfICCColorProfile();
            dict["DestOutputProfile"] = new PdfReferenceHolder(srgbProfile);

            PdfArray outputIntent = new PdfArray();
            outputIntent.Add(dict);

            Catalog["OutputIntents"] = outputIntent;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <returns>The proper PdfForm instance.</returns>
        internal override PdfForm GetForm()
        {
            return Form;
        }

        /// <summary>
        /// Adds the fields connected to the page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="newPage">The new page.</param>
        /// <param name="fields">The lost of the fields.</param>
        internal override void AddFields(PdfLoadedDocument ldDoc, PdfPageBase newPage, List<PdfField> fields)
        {
            if (ldDoc.Catalog.ContainsKey(DictionaryProperties.AcroForm))
            {
                PdfReferenceHolder acroFormReference = null;
                PdfDictionary acroFormDictionary = null;
                if (ldDoc.Catalog[DictionaryProperties.AcroForm] is PdfReferenceHolder)
                {
                    acroFormReference = ldDoc.Catalog[DictionaryProperties.AcroForm] as PdfReferenceHolder;
                    acroFormDictionary = acroFormReference.Object as PdfDictionary;
                }
                else if (ldDoc.Catalog[DictionaryProperties.AcroForm] is PdfDictionary)
                {
                    acroFormDictionary = ldDoc.Catalog[DictionaryProperties.AcroForm] as PdfDictionary;
                }

                if (acroFormDictionary != null)
                {
                    if (acroFormDictionary.ContainsKey(DictionaryProperties.DR))
                    {
                        PdfDictionary drDictionary = acroFormDictionary[DictionaryProperties.DR] as PdfDictionary;
                        if ((drDictionary != null) && (drDictionary.ContainsKey(DictionaryProperties.Font)))
                        {
                            PdfDictionary fontDictionary = drDictionary[DictionaryProperties.Font] as PdfDictionary;
                            if (Form.Dictionary != null)
                            {
                                PdfDictionary newDRDictionary = null;
                                if (Form.Dictionary.ContainsKey(DictionaryProperties.DR) == true)
                                {
                                    PdfDictionary drDic = Form.Dictionary[DictionaryProperties.DR] as PdfDictionary;
                                    PdfDictionary fontDic = drDic[DictionaryProperties.Font] as PdfDictionary;
                                    newDRDictionary = EnableMemoryOptimization ? fontDictionary.Clone(CrossTable) as PdfDictionary : fontDictionary;

                                    foreach (KeyValuePair<PdfName, IPdfPrimitive> item in newDRDictionary.Items)
                                    {
                                        if (!fontDic.Items.ContainsKey(item.Key))
                                            fontDic.Items.Add(item.Key, item.Value);
                                    }
                                    fontDic.Modify();
                                }
                                else
                                {
                                    if (EnableMemoryOptimization)
                                        newDRDictionary = drDictionary.Clone(CrossTable) as PdfDictionary;
                                    else
                                        newDRDictionary = drDictionary;
                                    PdfResources res = new PdfResources(newDRDictionary);
                                    Form.Resources = res;
                                    Form.Dictionary.SetProperty(DictionaryProperties.DR, res);
                                    Form.Dictionary.Modify();
                                }
                            }
                        }
                        else if (acroFormDictionary[DictionaryProperties.DR] is PdfReferenceHolder)
                        {
                            drDictionary = (acroFormDictionary[DictionaryProperties.DR] as PdfReferenceHolder).Object as PdfDictionary;
                            if ((drDictionary != null) && (drDictionary.ContainsKey(DictionaryProperties.Font)))
                            {
                                PdfDictionary fontDictionary = drDictionary[DictionaryProperties.Font] as PdfDictionary;
                                Dictionary<PdfName, IPdfPrimitive> items = fontDictionary.Items;
                                if (Form.Dictionary != null)
                                {
                                    if (Form.Dictionary.ContainsKey(DictionaryProperties.DR) == true)
                                    {
                                        PdfDictionary drDic = Form.Dictionary[DictionaryProperties.DR] as PdfDictionary;
                                        PdfDictionary fontDic = drDic[DictionaryProperties.Font] as PdfDictionary;

                                        foreach (KeyValuePair<PdfName, IPdfPrimitive> item in items)
                                        {
                                            if (!fontDic.Items.ContainsKey(item.Key))
                                                fontDic.Items.Add(item.Key, item.Value);
                                        }
                                        fontDic.Modify();
                                    }
                                    else
                                    {
                                        PdfResources res = new PdfResources(drDictionary);
                                        Form.Resources = res;
                                        Form.Dictionary.SetProperty(DictionaryProperties.DR, res);
                                        Form.Dictionary.Modify();
                                    }
                                }
                            }
                        }
                    }
                    Form.SetAppearanceDictionary = ldDoc.Form.SetAppearanceDictionary;
                    Form.NeedAppearances = ldDoc.Form.NeedAppearances;
                }
            }

            for (int i = 0, count = fields.Count; i < count; ++i)
            {
                if (!EnableMemoryOptimization)
                {
                    if (fields[i].Dictionary.ContainsKey(DictionaryProperties.P))
                        fields[i].Dictionary.Remove(DictionaryProperties.P);
                }

                Form.Fields.Add(fields[i], newPage);
            }

            if (EnableMemoryOptimization && Form != null && Form.Fields.Count > 0 && ldDoc.Form != null)
            {
                PdfReferenceHolder formRef = ldDoc.Catalog[DictionaryProperties.AcroForm] as PdfReferenceHolder;
                if (formRef != null && !ldDoc.CrossTable.PageCorrespondance.ContainsKey(formRef.Reference))
                {
                    PdfReference reference = CrossTable.GetReference(Form.Dictionary);
                    ldDoc.CrossTable.PageCorrespondance.Add(formRef.Reference, reference);
                }
            }
        }

        /// <summary>
        /// Clones pages and their resource dictionaries and adds them into the document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page being cloned.</param>
        /// <param name="destinations">The destinations.</param>
        /// <returns>page/returns>
        internal override PdfPageBase ClonePage(PdfLoadedDocument ldDoc, PdfPageBase page,
            List<PdfArray> destinations)
        {
            PdfPageBase p = Pages.Add(ldDoc, page, destinations);

            return p;
        }
        #endregion
    }

    #region Internals
    /// <summary>
    /// Shows the saving progress.
    /// </summary>
    public class ProgressEventArgs
    {
        #region Fields
        /// <summary>
        /// The Total
        /// </summary>
        private int m_total;

        /// <summary>
        /// The Current object.
        /// </summary>
        private int m_current;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ProgressEventArgs"/> class.
        /// </summary>
        /// <param name="current">The current index.</param>
        /// <param name="total">The total number.</param>
        internal ProgressEventArgs(int current, int total)
        {
            if (total <= 0)
            {
                throw new ArgumentOutOfRangeException("total", "Total is less then or equal to zero.");
            }

            if (current < 0)
            {
                throw new ArgumentOutOfRangeException("current", "Current can't be less then zero.");
            }

            m_current = current;
            m_total = total;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressEventArgs"/> class.
        /// </summary>
        private ProgressEventArgs()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the total number of the elements (pages) that need to be saved.
        /// </summary>     
        public int Total
        {
            get
            {
                return m_total;
            }
        }

        /// <summary>
        /// Gets the current element (page) index that just was saved.
        /// </summary>
        /// <remarks>The index value increases constantly from 0 to Total.</remarks>
        public int Current
        {
            get
            {
                return m_current;
            }
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <remarks>Progress constantly increases from 0.0 to 1.0.
        /// 1.0 value means that entire document has been saved.</remarks>
        public float Progress
        {
            get
            {
                float progress = (float)Current / (float)Total;

                return progress;
            }
        }
        #endregion
    }

    /// <summary>
    /// Arguments for event raised after document saving.
    /// </summary>
    internal class DocumentSavedEventArgs : EventArgs
    {
        #region Fields
        /// <summary>
        /// Document's destination stream.
        /// </summary>
        private PdfWriter m_writer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSavedEventArgs"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal DocumentSavedEventArgs(PdfWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            m_writer = writer;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets document's destination stream.
        /// </summary>
        internal PdfWriter Writer
        {
            get
            {
                return m_writer;
            }
        }
        #endregion
    }
    #endregion
}
