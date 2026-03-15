#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;
#if NETFX_CORE || WP
using Windows.Storage;
using System.Threading.Tasks;
using System.Security;
#endif
# if !SILVERLIGHT  && !NETFX_CORE && !WP
using Syncfusion.Pdf.Xmp;
using System.Xml;
# endif

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
	/// <summary>
	/// Represents a logic to handle existing PDF documents.
	/// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Loads an existing document
    /// PdfLoadedDocument lDoc = new PdfLoadedDocument("sourceDoc.pdf");
    /// // Sets the PDF version as 1.6
    /// lDoc.FileStructure.Version = PdfVersion.Version1_6;
    /// lDoc.Save("Sample.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Loads an existing document
    /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
    /// ' Sets the PDF version as 1.6
    /// lDoc.FileStructure.Version = PdfVersion.Version1_6
    /// lDoc.Save("Sample.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocumentBase"/> Class
    /// <seealso cref="PdfFileStructure"/> Class
	public class PdfLoadedDocument :
		PdfDocumentBase,
		IDisposable
	{
		#region Fields
		/// <summary>
		/// String contain either user or owner password.
		/// </summary>
		private string m_password;

		/// <summary>
		/// The stream with the document data.
		/// </summary>
		private Stream m_stream;

		/// <summary>
		/// Indicates whether the document was encrypted.
		/// </summary>
		private bool m_bWasEncrypted;
        /// <summary>
		/// Indicates whether the document was Pdf Viewer document.
		/// </summary>
        private bool m_isPdfViewerDocumentDisable = true;
        
		/// <summary>
		/// Stores loaded form.
		/// </summary>
		private PdfLoadedForm m_form;

		/// <summary>
		/// Collection of loaded and created pages.
		/// </summary>
		private PdfLoadedPageCollection m_pages;

		/// <summary>
		/// Bookmarks of the document.
		/// </summary>
		private PdfBookmarkBase m_bookmark;

		/// <summary>
		/// Indicates whether the stream should be closed on dispose.
		/// </summary>
		private bool m_bCloseStream;

		/// <summary>
		/// Indicates whether the object was disposed.
		/// </summary>
		private bool m_isDisposed;

		private PdfDocumentInformation m_documentInfo;
        /// <summary>
        /// Internal stream
        /// </summary>
        private MemoryStream m_internalStream = new MemoryStream();
        /// <summary>
        /// Defines the color space of the document
        /// </summary>
        private Syncfusion.Pdf.Graphics.PdfColorSpace m_colorSpace;
		/// <summary>
        /// Defines the attachment collection of the document
		/// </summary>		
        private PdfAttachmentCollection m_attachments = null;
        /// <summary>
        /// String contain either user or owner password.
        /// </summary>
        private string password;
        /// <summary>
        /// Defines the Pdf Page Label.
        /// </summary>
        private PdfPageLabel m_pageLabel;
        /// <summary>
        /// Defined the Pdf Loaded Page Label Collection
        /// </summary>
        private PdfLoadedPageLabelCollection m_pageLabelCollection;
        /// <summary>
        /// Check whether the Page Label Enabled or not.
        /// </summary>
        private bool isPageLabel = false;

        /// <summary>
        /// Indicates is xfa form or not.
        /// </summary>
        private bool m_isXFAForm = false;

        /// <summary>
        /// Holds the file name for file saving operation.
        /// </summary>
        private string m_fileName;

        /// <summary>
        /// Holds the conformance level of the loaded document.
        /// </summary>
        private PdfConformanceLevel m_conformance;

        /// <summary>
        /// Check whether the document is linearized or not.
        /// </summary>
        private bool isLinearized;

        private bool isPortfolio;

       /// <summary>
       /// Private variable to store the pdf portfolio
       /// </summary>
        private PdfPortfolioInformation m_portfolio;

        private static Stream m_openStream;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Holds the dublin core values of the loaded document.
        /// </summary>
        private DublinCoreSchema m_dublinschema;
        /// <summary>
        /// Internal variable used to store the font information
        /// </summary>
        private List<PdfUsedFont> m_usedFonts;
#endif
        private Dictionary<PdfPageBase, object> m_bookmarkHashtable = null;

#if NETFX_CORE || WP
        private StorageFile m_stFile;
#endif
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this form is XFA Form or AcroForm.
        /// </summary>
        internal bool IsXFAForm
        {
            get
            {
                return m_isXFAForm;
            }

            set
            {
                m_isXFAForm = value;
            }
        }
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets or sets a value of the metadata dublin core values.
        /// </summary>
        internal DublinCoreSchema DublinSchema
        {
            get
            {
                return m_dublinschema;
            }
            set
            {
                m_dublinschema = value;
            }
        }
#endif
        /// <summary>
        /// Get and Set the PdfPageLabel.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("sourceDoc.pdf");
        /// // Create page label with upper case roman letters and starts with 3
        /// PdfPageLabel label = new PdfPageLabel();
        /// label.NumberStyle = PdfNumberStyle.UpperRoman;
        /// label.StartNumber = 3;
        /// lDoc.LoadedPageLabel = label;
        /// lDoc.Save("PageLabel.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
        /// ' Create page label with upper case roman letters and starts with 3
        /// Dim label As PdfPageLabel = New PdfPageLabel()
        /// label.NumberStyle = PdfNumberStyle.UpperRoman
        /// label.StartNumber = 3
        /// lDoc.LoadedPageLabel = label
        /// lDoc.Save("PageLabel.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        public PdfPageLabel LoadedPageLabel
        {
            get
            {
                return m_pageLabel;
            }
            set
            {
                if (m_pageLabelCollection == null)
                {
                    m_pageLabelCollection = new PdfLoadedPageLabelCollection();
                }
                isPageLabel = true;
                m_pageLabelCollection.Add(value);
            }
        }
        /// <summary>
        /// Get and Set the Password.
        /// </summary>
        internal string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;

            }
        }

		/// <summary>
        /// Gets the collection of document attachments displayed on a PDF page.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("sourceDoc.pdf");
        /// // Gets the collection of attachments displayed on a PDF page.
        /// PdfAttachmentCollection collection = lDoc.Attachments;
        /// // Creating an attachment
        /// PdfAttachment attachment = new PdfAttachment("logo.jpeg");
        /// attachment.FileName = "Syncfusion Logo";
        /// // Adding attachments to an existing document
        /// collection.Add(attachment);
        /// lDoc.Save("Attachment.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
        /// ' Gets the collection of attachments displayed on a PDF page.
        /// Dim collection As PdfAttachmentCollection = lDoc.Attachments
        /// ' Creating an attachment
        /// Dim attachment As PdfAttachment = New PdfAttachment("logo.jpeg")
        /// attachment.FileName = "Syncfusion Logo"
        /// ' Adding attachments to an existing document
        /// collection.Add(attachment)
        /// lDoc.Save("Attachment.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfAttachment"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
		 public PdfAttachmentCollection Attachments
        {
            get
            {
                if (m_attachments == null)
                {
                    PdfDictionary attachmentDictionary = GetAttachmentDictionary();

                    if (attachmentDictionary != null)
                    {
                        m_attachments = new PdfAttachmentCollection(attachmentDictionary, CrossTable);

                        if (m_attachments != null)
                        {
                            Catalog.Attachments = m_attachments;                        
                        }
                    }                   
                     
                }               
                return m_attachments;
            }
        }

        /// <summary>
        /// Get the portfolio from the document
        /// </summary>
         public PdfPortfolioInformation PortfolioInformation
         {
             set
             {
                 Catalog.PdfPortfolio = value;
             }
             
             get
             {
                 if (m_portfolio == null)
                 {
                     PdfDictionary portfolioDictionary = GetPortfolioDictionary();
                     if (portfolioDictionary != null)
                     {
                         m_portfolio = new PdfPortfolioInformation(portfolioDictionary);
                     }
                 }

                 return m_portfolio;
             }
         }
		// Pages
		// Form
		// Outlines
		// Security

        /// <summary>
        /// Gets or sets the color space for page that will be created.
        /// </summary>
        /// <remarks>This property has impact on the new created pages only.
        /// If a page was created it remains its colour space obliviously
        /// to this property changes.</remarks>
         /// <example>
         /// <code lang="C#">
         /// // Loads an existing document
         /// PdfLoadedDocument lDoc = new PdfLoadedDocument("sourceDoc.pdf");
         /// // Sets the documents colorSpace as GrayScale
         /// lDoc.ColorSpace = PdfColorSpace.GrayScale;
         /// lDoc.Save("ColorSpace.pdf");
         /// </code>
         /// <code lang="VB">
         /// ' Loads an existing document
         /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
         /// 'Sets the documents colorSpace as GrayScale
         /// lDoc.ColorSpace = PdfColorSpace.GrayScale
         /// lDoc.Save("ColorSpace.pdf")
         /// </code>
         /// </example>
         /// <seealso cref="PdfLoadedDocument"/> Class
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
		/// Gets the loaded form.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Load the PDF form
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// // Gets the form from the existing document
        /// PdfLoadedForm form = lDoc.Form;
        /// // Reading field element
        /// PdfLoadedTextBoxField textField = form[0] as PdfLoadedTextBoxField;
        /// textField.Text = "Syncfusion";
        /// lDoc.Save("LoadedForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// ' Gets the form from the existing document
        /// Dim form As PdfLoadedForm = lDoc.Form
        /// ' Reading field element
        /// Dim textField As PdfLoadedTextBoxField = TryCast(form(0), PdfLoadedTextBoxField)
        /// textField.Text = "Syncfusion"
        /// lDoc.Save("LoadedForm.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfForm"/> Class    
        /// <seealso cref="PdfLoadedDocument"/> Class
		public PdfLoadedForm Form
		{
			get
			{
				if( m_form == null )
				{
					PdfDictionary formDictionary = GetFormDictionary();

					if( formDictionary != null )
					{
						m_form = new PdfLoadedForm( formDictionary, CrossTable );

						if( m_form != null )
						{
							Catalog.LoadedForm = m_form;
                            //if (m_form.Flatten)
                            //{
                            if (!PdfLoadedPage.m_annotChanged)
                            {
                                for (int i = 0; i < (m_form.CrossTable.Document as PdfLoadedDocument).Pages.Count; i++)
                                {
                                    if ((m_form.CrossTable.Document as PdfLoadedDocument).Pages[i] is PdfLoadedPage)
                                        ((m_form.CrossTable.Document as PdfLoadedDocument).Pages[i] as PdfLoadedPage).CreateAnnotations();
                                }
                            }
                            //}
						}
					}
				}
				return m_form;
			}
		}

		/// <summary>
		/// Gets the pages.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// // Reading page collection from an existing document
        /// PdfLoadedPageCollection pageCollection = lDoc.Pages;
        /// //Creates a new page and adds it as the last page of the document 
        /// pageCollection.Add();            
        /// lDoc.Save("Pages.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// ' Reading page collection from an existing document
        /// Dim pageCollection As PdfLoadedPageCollection = lDoc.Pages
        /// ' Create a page 
        /// pageCollection.Add()
        /// lDoc.Save("Pages.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedPage"/> Class   
        /// <seealso cref="PdfLoadedDocument"/> Class
		public PdfLoadedPageCollection Pages
		{
			get
			{
				if( m_pages == null )
				{
					m_pages = new PdfLoadedPageCollection( this, CrossTable );
				}
				return m_pages;
			}
		}

		/// <summary>
		/// Gets the bookmarks.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("sourceDoc.pdf");
        /// // Reading bookmark collection from an existing document
        /// PdfBookmarkBase bm =  lDoc.Bookmarks;
        /// // Creates a new bookmark
        /// PdfBookmark newbm = bm.Add("Chapter1");
        /// newbm.Color = Color.DarkBlue;
        /// newbm.TextStyle = PdfTextStyle.Bold;            
        /// newbm.Destination = new PdfDestination( lDoc.Pages[0]);
        /// lDoc.Save("BookMark.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("sourceDoc.pdf")
        /// ' Reading bookmark collection from an existing document
        /// Dim bm As PdfBookmarkBase = lDoc.Bookmarks
        /// ' Creates a new bookmark
        /// Dim newbm As PdfBookmark = bm.Add("Chapter1")
        /// newbm.Color = Color.DarkBlue
        /// newbm.TextStyle = PdfTextStyle.Bold
        /// newbm.Destination = New PdfDestination(lDoc.Pages(0))
        /// lDoc.Save("BookMark.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class  
        /// <seealso cref="PdfBookmark"/> Class  
		public override PdfBookmarkBase Bookmarks
		{
			get
			{
				if( Catalog.ContainsKey( DictionaryProperties.Outlines ) && ( m_bookmark == null ) )
				{
					PdfDictionary outlines = PdfCrossTable.Dereference( Catalog[ DictionaryProperties.Outlines ] ) as PdfDictionary;

					m_bookmark = new PdfBookmarkBase( outlines, CrossTable );
					m_bookmark.ReproduceTree();
				}
                else if (m_bookmark == null)
                    m_bookmark = CreateBookmarkRoot();

				return m_bookmark;
			}
		}

        /// <summary>
        /// Gets number of pages.
        /// </summary>
# if NETFX_CORE || WP
        public override int PageCount
#else
        internal override int PageCount
#endif
        {
            get { return Pages.Count; }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP

#if AllowUnsafeCode
        /// <summary>
        /// Gets the conformance level applied in the loaded document.
        /// </summary>
        /// <remarks>Returns only levels supported by PdfConformanceLevel enum, otherwise None.</remarks>
        public PdfConformanceLevel Conformance
        {
            get
            {
                if (m_conformance == PdfConformanceLevel.None)
                {
                    PdfArray outputIntents = Catalog[DictionaryProperties.OutputIntents] as PdfArray;

                    if (outputIntents != null)
                    {
                        for (int i = 0; i < outputIntents.Count; i++)
                        {
                            PdfDictionary dict = outputIntents[i] as PdfDictionary;
                            if (dict != null)
                            {
                                PdfName conformance = dict[DictionaryProperties.S] as PdfName;
                                if (conformance.Value == "GTS_PDFA1")
                                {
                                    m_conformance = PdfConformanceLevel.Pdf_A1B;
                                    break;
                                }
                                else if (conformance.Value == DictionaryProperties.GTS_PDFX)
                                {
                                    if (DocumentInformation.Dictionary.ContainsKey(DictionaryProperties.GTS_PDFXConformance))
                                    {
                                        PdfString level = DocumentInformation.Dictionary[DictionaryProperties.GTS_PDFXConformance] as PdfString;
                                        if (level.Value == "PDF/X-1a:2001")
                                        {
                                            m_conformance = PdfConformanceLevel.Pdf_X1A2001;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (outputIntents == null | m_conformance == PdfConformanceLevel.Pdf_A1B)
                    {
                        string part = "pdfaid:part", conformance = "pdfaid:conformance";
                        XmlElement element = DocumentInformation.XmpMetadata.Xmpmeta;

                        bool found = false;

                        foreach (XmlNode parentNode in element.ChildNodes)
                        {
                            foreach (XmlNode childNode in parentNode.ChildNodes)
                            {
                                XmlAttribute attr1 = childNode.Attributes[part];
                                XmlAttribute attr2 = childNode.Attributes[conformance];
                                if (attr1 != null && attr2 != null && attr1.Value == "1" && attr2.Value == "B")
                                {
                                    m_conformance = PdfConformanceLevel.Pdf_A1B;
                                    found = true;
                                    break;
                                }
                                if (childNode.InnerXml.Contains("pdfaid"))
                                {
                                    if (childNode[part].InnerText == "1" && childNode[conformance].InnerText == "B")
                                    {
                                        m_conformance = PdfConformanceLevel.Pdf_A1B;
                                        found = true;
                                        break;
                                    }
                                }
                            }

                            if (found)
                                break;
                        }

                        if (!found)
                            m_conformance = PdfConformanceLevel.None;
                    }
                }
                return m_conformance;
            }
        }
# endif

        /// <summary>
        /// Gets the fonts which are available in the PDF document.
        /// </summary>
        /// <value>Retruns the fonts which are used in the PDF document.</value>
        public PdfUsedFont[] UsedFonts
        {
            get
            {
                if (m_usedFonts == null)
                    m_usedFonts = ExtractFonts();

                return m_usedFonts.ToArray();
            }
        }
#endif
        /// <summary>
        /// Used to check document is linearized or not
        /// </summary>
        public bool IsLinearized
        {
            get
            {
                isLinearized = CheckLinearization();
                return isLinearized;
            }
        }
        
        
        /// <summary>
        /// used to check the portfolio in the document
        /// </summary>
        public bool IsPortfolio
        {
            get
            {
                PdfDictionary portfolioDictionary = GetPortfolioDictionary();
                if (portfolioDictionary != null)
                {
                    isPortfolio = true;
                    return isPortfolio;
                }

                isPortfolio = false;
                return isPortfolio;
                
            }
        }
		#endregion

		#region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
        /// </summary>
#endif
       
#if NETFX_CORE || WP
        public PdfLoadedDocument()
        {
        }
#else
         /// <param name="filename">The path to source pdf file.</param>
        /// <remarks>This constructor imports an existing pdf file into the document object. It automatically populates the Pages collection with the pages of the given document. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing PDF Document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("Sample.pdf");
        /// // Save the document to a disk
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing PDF Document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("Sample.pdf")
        /// ' Save the document to a disk
        /// doc.Save("Samplepdf.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
		public PdfLoadedDocument( string filename )
			: this( CreateStream( filename ) )
		{
			m_bCloseStream = true;
            m_fileName = filename;
		}
#endif
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
        /// </summary>
		/// <param name="filename">The path to source PDF document.</param>
		/// <param name="password">The password (user or owner) of the encrypted document.</param>
        /// <example>
        /// <code lang="C#">
        /// // Load the PDF document with password.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("Sample.pdf","password");
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
        ///  ' Load the PDF document with password.
        ///  PdfLoadedDocument doc = new PdfLoadedDocument("Sample.pdf","password")
        ///  doc.Save("Samplepdf.pdf")
        /// </code>
        /// </example>
  		public PdfLoadedDocument( string filename, string password )
			: this( CreateStream( filename ), password )
		{
			m_bCloseStream = true;
            Password = password;
            m_fileName = filename;
        }
#endif
        /// <summary>
		/// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
		/// </summary>
		/// <param name="file">The byte array with the file content.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read);
        /// // Create a byte array of file stream length  
        /// byte[] pdfData = new byte[file2.Length];   
        /// //Read block of bytes from stream into the byte array   
        /// file2.Read(pdfData,0,System.Convert.ToInt32(pdfData.Length));
        /// // Load the byte array 
        /// PdfLoadedDocument doc = new PdfLoadedDocument(pdfData);
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
	    /// Dim file2 As Stream = New FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read)
        /// ' Create a byte array of file stream length  
        /// Dim pdfData() As Byte = New Byte(file2.Length){}
        /// 'Read block of bytes From stream Into the Byte array 
        /// file2.Read(pdfData,0,System.Convert.ToInt32(pdfData.Length))
        /// ' Load the byte array 
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument(pdfData)
        /// doc.Save("Samplepdf.pdf")
		/// </code>
        /// </example>
		public PdfLoadedDocument( byte[] file )
			: this( CreateStream( file ) )
		{
			m_bCloseStream = true;
		}

#if !SILVERLIGHT
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
		/// </summary>
		/// <param name="file">The byte array with the file content.</param>
		/// <param name="password">The password (user or owner) of the encrypted document.</param>
        /// <example>
        /// <code lang="C#">
        /// // Load the byte array with password
        /// PdfLoadedDocument doc = new PdfLoadedDocument(pdfData, "password");
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Load the byte array with password
        /// PdfLoadedDocument doc = new PdfLoadedDocument(pdfData, "password")
        /// doc.Save("Samplepdf.pdf")
        /// </code>
        /// </example>
		public PdfLoadedDocument( byte[] file, string password )
			: this( CreateStream( file ), password )
		{
            Password = password;
			m_bCloseStream = true;
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
		/// </summary>
		/// <param name="file">The stream with the file.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read);    
        /// // Load the stream
        /// PdfLoadedDocument doc = new PdfLoadedDocument(file2);
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim file2 As Stream = New FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read)
        /// ' Load the stream
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument(file2)
        /// doc.Save("Samplepdf.pdf")
        /// </code>
        /// </example>
		public PdfLoadedDocument( Stream file )
		{
			if( file == null )
				throw new ArgumentNullException( "file" );

            Stream pdfStream = CheckIfValid(file);
            LoadDocument(pdfStream);
		}

#if !SILVERLIGHT
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PdfLoadedDocument"/> class.
		/// </summary>
		/// <param name="file">The stream with the file.</param>
		/// <param name="password">The password (user or owner) of the encrypted document.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read);    
        /// // Load the stream
        /// PdfLoadedDocument doc = new PdfLoadedDocument(file2, "password");
        /// doc.Save("Samplepdf.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim file2 As Stream = New FileStream("sample.pdf", FileMode.Open, FileAccess.Read, FileShare.Read)
        /// ' Load the stream
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument(file2, "password")
        /// doc.Save("Samplepdf.pdf")
        /// </code>
        /// </example>
		public PdfLoadedDocument( Stream file, string password )
		{
			if( file == null )
				throw new ArgumentNullException( "file" );

			if( password == null )
				throw new ArgumentNullException( "password" );

			m_password = password;

			LoadDocument( file );
		}
#endif
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="PdfLoadedDocument"/> is reclaimed by garbage collection.
		/// </summary>
		~PdfLoadedDocument()
		{
			Dispose( false );
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Creates the stream.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>The proper file stream.</returns>
        #if NETFX_CORE || WP
		private static async void CreateStream( string filename )
		{

			if( filename == null )
				throw new ArgumentNullException( "filename" );


            StorageFolder folder = Windows.Storage.KnownFolders.DocumentsLibrary;
            StorageFile stFile = await folder.GetFileAsync(filename);
            Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
            m_openStream = fileStream.AsStream();
		}
    #else

        private static Stream CreateStream(string filename)
        {

            if (filename == null)
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new ArgumentException("File doesn't exist", "filename");

            FileInfo fi = new FileInfo(filename);

            MemoryStream ms = null;
            Stream fs;
            byte[] filedata = new byte[] { };
            
			bool IsReadOnly = ( ( fi.Attributes & FileAttributes.ReadOnly ) != 0 );

            if (IsReadOnly)
            {
                using (fs = fi.OpenRead())
                {
                    filedata = new byte[fs.Length];
                    fs.Read(filedata, 0, filedata.Length);
                }
            }
            else
            {
                try
                {
                    using (fs = fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        filedata = new byte[fs.Length];
                        fs.Read(filedata, 0, filedata.Length);
                    }
                }
                catch (IOException exception)
                {
                    using (fs = fi.OpenRead())
                    {
                        filedata = new byte[fs.Length];
                        fs.Read(filedata, 0, filedata.Length);
                    }
                }
                catch (SystemException exception)
                {
                    using (fs = fi.OpenRead())
                    {
                        filedata = new byte[fs.Length];
                        fs.Read(filedata, 0, filedata.Length);
                    }
                }
            }
            ms = new MemoryStream(filedata);
            return ms;
		}

       #endif

		/// <summary>
		/// Creates the stream.
		/// </summary>
		/// <param name="file">The file content.</param>
		/// <returns>The proper memory stream.</returns>
		private static Stream CreateStream( byte[] file )
		{
			if( file == null )
				throw new ArgumentNullException( "file" );

			MemoryStream ms = new MemoryStream( file );

			return ms;
		}
		#endregion

		#region Public Methods

#if NETFX_CORE || WP
        /// <summary>
        /// Opens an exiting Pdf document
        /// </summary>
        /// <param name="stFile">Storage File</param>
        /// <returns> Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(StorageFile stFile)
        {
            // Check if the storage file is valid.
            if (stFile == null)
            {
                throw new ArgumentNullException("InvalidFile");
            }

           
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            m_stFile = stFile;
            await Task.Run(() =>
            {
                try
                {
                    Stream fileStream = stFile.OpenStreamForReadAsync().Result;
                    byte[] bytes = new byte[fileStream.Length];                        
                    fileStream.Read(bytes, 0, bytes.Length);
                    LoadDocument(new MemoryStream(bytes));
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Opens an exising Pdf document
        /// </summary>
        /// <param name="stFile">Storage File</param>
        /// <param name="password">Password for opening the document</param>
        /// <returns>Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(StorageFile stFile, string password)
        {
            
            // Check if the storage file is valid.
            if (stFile == null)
            {
                throw new ArgumentNullException("InvalidFile");
            }

            if (password == null)
            {
                throw new ArgumentException("password");
            }

            m_password = password;
            m_stFile = stFile;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    Stream fileStream = stFile.OpenStreamForReadAsync().Result;
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    LoadDocument(new MemoryStream(bytes));
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Opens an existing Pdf document
        /// </summary>
        /// <param name="bytes">Pdf document in bytes</param>
        /// <returns>Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentException("Invalid bytes");
            }
           
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    LoadDocument(new MemoryStream(bytes));
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Opens an existing Pdf document
        /// </summary>
        /// <param name="bytes">Pdf document in bytes</param>
        /// <param name="password">Password for opening the document</param>
        /// <returns>Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(byte[] bytes, string password)
        {
            if (bytes == null)
            {
                throw new ArgumentException("Invalid bytes");
            }

            if (password == null)
            {
                throw new ArgumentException("password");
            }

            m_password = password;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {                    
                    LoadDocument(new MemoryStream(bytes));
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Opens an existing Pdf document
        /// </summary>
        /// <param name="stream">Pdf document stream</param>
        /// <returns>Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentException("Invalid bytes");
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    LoadDocument(stream);
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;


        }

        /// <summary>
        /// Opens an existing Pdf document
        /// </summary>
        /// <param name="stream">Pdf document stream</param>
        /// <param name="password">Password for opening the document</param>
        /// <returns>Task returning true if open succeeded</returns>
        public async Task<bool> OpenAsync(Stream stream, string password)
        {
            if (stream == null)
            {
                throw new ArgumentException("Invalid stream");
            }

            if (password == null)
            {
                throw new ArgumentException("password");
            }

            m_password = password;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    LoadDocument(stream);
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
#if !SILVERLIGHT
		/// <summary>
		/// Saves the document into the same stream or file.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourcePDF.pdf");
        /// ldoc.FileStructure.Version = PdfVersion.Version1_6;
        /// // Save the changes in the same document.
        /// ldoc.Save();
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourcePDF.pdf")
        /// ldoc.FileStructure.Version = PdfVersion.Version1_6
        /// ' Save the changes in the same document.
        /// ldoc.Save()
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        #if NETFX_CORE || WP
        /// <summary>
        /// Saves the document to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public async Task<bool> SaveAsync(Stream stream)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    using (PdfWriter writer = new PdfWriter(stream))
                    {
                        if (this.FileStructure.IncrementalUpdate)
                        {
                            CopyOldStream(writer);
                            AppendDocument(writer);
                        }
                        else
                        {
                            //this.FileStructure.Version = PdfVersion.Version1_4;

                            PdfCrossTable ctable = CrossTable;
                            SetCrossTable(new PdfCrossTable((int)ctable.Count, ctable.EncryptorDictionary));
                            CrossTable.Document = this;

                            //CrossTable.ForceNew();

                            try
                            {
                                AppendDocument(writer);
                            }
                            finally
                            {
                                SetCrossTable(ctable);
                            }
                        }
                    }

                    tcs.SetResult(true);
                }
                catch(Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Saves the modified document
        /// </summary>
        /// <returns>Task returning true if open succeeded</returns>
		public async Task<bool> Save()
		{

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    //Must save to intermediate stream
                    Save(m_internalStream);

                    if (!m_stream.CanWrite)
                        throw new PdfException("Unable to save to the specified file or stream, because it is being used by another process. Use Save(filename) or Save(stream) instead and specify different filename or stream.");

                    if (m_stFile != null)
                    {
                        Stream fileStream = m_stFile.OpenStreamForWriteAsync().Result;
                        MemoryStream tempStream = new MemoryStream();
                        Save(tempStream);
                        fileStream.Write(tempStream.ToArray(), 0, (int)tempStream.Length);
                        fileStream.Flush();
                        fileStream.Dispose();
                        tempStream.Dispose();
                    }
                }
                catch
                {
                }
            });

            return await tcs.Task;
		}
                #else
        public void Save()
		{
            //Must save to intermediate stream
            Save(m_internalStream);

            if (!m_stream.CanWrite)
                throw new PdfException("Unable to save to the specified file or stream, because it is being used by another process. Use Save(filename) or Save(stream) instead and specify different filename or stream.");

            if (!string.IsNullOrEmpty(m_fileName))
            {
                using (FileStream stream = new FileStream(m_fileName, FileMode.Create, FileAccess.Write))
                {
                    Save(stream);
                }
            }
		}
        #endif
#endif
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

#if !SILVERLIGHT && !NETFX_CORE && !WP
		/// <summary>
		/// Splits a PDF file to many PDF files, each of them consists of one page from the source file.
		/// </summary>
		/// <param name="destFilePattern">Template for destination file names.</param>
		/// <remarks>
		/// Each destination file will have 'destFileName{0***}' name,
		/// where *** is an optional format string for the number of the
		/// page inside of the source document.
		/// </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("Form.pdf");
        /// // Splits the source document 
        /// ldoc.Split("pdfDoc.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("Form.pdf")
        /// ' Splits the source document 
        /// ldoc.Split("pdfDoc.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
		public void Split( string destFilePattern )
		{
			Split( destFilePattern, 0 );
		}

		/// <summary>
		/// Splits a PDF file to many PDF files, each of them consists of
		/// one page from the source file.
		/// </summary>
		/// <param name="destFilePattern">Template for destination file
		/// names.</param>
		/// <param name="startNumber">The number that is use as a start
		/// point for the page numbering.</param>
		/// <remarks>
		/// Each destination file will have 'destFileName{0***}' name,
		/// where *** is an optional format string for the number of the
		/// page inside of the source document.
		/// </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("Form.pdf");
        /// // Splits the source document 
        /// ldoc.Split("pdfDoc.pdf", 1);
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("Form.pdf")
        /// ' Splits the source document 
        /// ldoc.Split("pdfDoc.pdf", 1)
        /// </code>
        /// </example>
		public void Split( string destFilePattern, int startNumber )
		{
			if( destFilePattern == null )
				throw new ArgumentNullException( "destFileName" );

			Regex regex = new Regex( @"\w*\{0.*\}\w*", RegexOptions.None );

			if( !regex.Match( destFilePattern ).Success )
			{
				int dotIndex = destFilePattern.LastIndexOf( '.' );

				if( dotIndex < 0 )
				{
					destFilePattern = string.Format( "{0}{1}", destFilePattern, "{0}.pdf" );
				}
				else
				{
					destFilePattern = string.Format( "{0}{1}{2}",
						destFilePattern.Substring( 0, dotIndex ),
						"{0}",
						destFilePattern.Substring( dotIndex, destFilePattern.Length - dotIndex ) );
				}
			}

			for( int i = 0, count = Pages.Count; i < count; ++i )
			{
				PdfDocument doc = new PdfDocument();

				doc.ImportPage( this, i );
				doc.Save( string.Format( destFilePattern, i + startNumber ) );
                doc.Close();
			}
		}
#endif

		/// <summary>
		/// Creates new form.
		/// </summary>
		public void CreateForm()
		{
			if( m_form == null )
			{
				m_form = new PdfLoadedForm( CrossTable );
				Catalog.SetProperty( DictionaryProperties.AcroForm, new PdfReferenceHolder( m_form ) );
				Catalog.LoadedForm = m_form;
			}
		}

        /// <summary>
        /// Creates the pdf attachment
        /// </summary>
        /// <returns>The root collection of attachment.</returns>
        public PdfAttachmentCollection CreateAttachment()
        {
            m_attachments = new PdfAttachmentCollection();        
            Catalog.CreateNamesIfNone();           
            Catalog.Names.EmbeddedFiles = m_attachments;
            return m_attachments;
        }

		/// <summary>
		/// Creates the bookmark root.
		/// </summary>
		/// <returns>The root collection of bookmarks.</returns>
		public PdfBookmarkBase CreateBookmarkRoot()
		{
			m_bookmark = new PdfBookmarkBase();
			Catalog.SetProperty( DictionaryProperties.Outlines, new PdfReferenceHolder( m_bookmark ) );

			return m_bookmark;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Saves the document to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public override void Save( Stream stream )
		{
			using( PdfWriter writer = new PdfWriter( stream ) )
			{
#if SILVERLIGHT || NETFX_CORE || WP
                if(this.FileStructure.IncrementalUpdate)
#else
                if (Security.Enabled == m_bWasEncrypted && !(m_bWasEncrypted && Security.Encryptor.Changed)  && this.FileStructure.IncrementalUpdate)
#endif
				{                  
					CopyOldStream( writer );
					AppendDocument( writer );
				}
				else
				{
                    //this.FileStructure.Version = PdfVersion.Version1_4;

					PdfCrossTable ctable = CrossTable;
					SetCrossTable( new PdfCrossTable( ( int )ctable.Count, ctable.EncryptorDictionary ) );
					CrossTable.Document = this;
                    if (this.DocumentInformation != null)
                    {
                        CrossTable.Trailer[DictionaryProperties.Info] = new PdfReferenceHolder(this.DocumentInformation);
                    }

					//CrossTable.ForceNew();

					try
					{
						AppendDocument( writer );
					}
					finally
					{
						//SetCrossTable( ctable );
					}
				}
			}
		}

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Extracts fonts from the given PDF document.
        /// </summary>
        /// <returns>Returns the extracted fonts as array.</returns>
        private List<PdfUsedFont> ExtractFonts()
        {
            List<PdfUsedFont> usedFonts = new List<PdfUsedFont>();

            PdfLoadedPageCollection pages = Pages;           
            ArrayList fontCollection = new ArrayList();
            foreach (PdfLoadedPage loadedPage in pages)
            {
                PdfFont[] extractedFonts = loadedPage.ExtractFonts();

                foreach (PdfFont font in extractedFonts)
                {
                    usedFonts.Add(new PdfUsedFont(font, loadedPage));
                }
            }
            PdfFont[] fonts = new PdfFont[fontCollection.Count];
            int i=0;
            foreach (Syncfusion.Pdf.Graphics.PdfFont font in fontCollection)
            {
                fonts[i++] = font;
            }
            return usedFonts;
        }
#endif

        /// <summary>
		/// Adds the fields connected to the page.
		/// </summary>
		/// <param name="ldDoc">The loaded document.</param>
		/// <param name="newPage">The new page.</param>
		/// <param name="fields">The lost of the fields.</param>
		internal override void AddFields( PdfLoadedDocument ldDoc, PdfPageBase newPage, List<PdfField> fields )
		{
			if( fields.Count > 0 && Form == null )
			{
				CreateForm();
			}

			for( int i = 0, count = fields.Count; i < count; ++i )
			{
				PdfField field = fields[ i ];

				Form.Fields.Add( field, newPage );
			}
		}

		/// <summary>
		/// Clones pages and their resource dictionaries and adds them into the document.
		/// </summary>
		/// <param name="ldDoc">The loaded document.</param>
		/// <param name="page">The page being cloned.</param>
		/// <param name="destinations">The destinations.</param>
		/// <returns></returns>
		internal override PdfPageBase ClonePage( PdfLoadedDocument ldDoc, PdfPageBase page,
			List<PdfArray> destinations )
		{
			PdfPageBase p = Pages.Add( ldDoc, page, destinations );

			return p;
		}

		/// <summary>
		/// Gets or sets document's information and properties.
		/// </summary>
		public override PdfDocumentInformation DocumentInformation
		{
			get
			{
				if( m_documentInfo == null )
				{
					PdfDictionary trailer = CrossTable.Trailer;
					PdfDictionary info = PdfCrossTable.Dereference( trailer[ DictionaryProperties.Info ] ) as PdfDictionary;

					if( info != null )
					{
						m_documentInfo = new PdfDocumentInformation( info, Catalog );
					}
					else
					{
						m_documentInfo = base.DocumentInformation;
					}
				}

				return m_documentInfo;
			}
		}

		/// <summary>
		/// Gets the form.
		/// </summary>
		/// <returns>The proper PdfForm instance.</returns>
		internal override PdfForm GetForm()
		{
			PdfLoadedForm form = Form;

			if( form == null )
			{
				CreateForm();
			}

			return Form;
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        ///  //Load an existing document
        ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceDoc.pdf");
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPageBase page = doc.Pages.Add();            
        ///  //Create Pdf graphics for the page
        ///  PdfGraphics g = page.Graphics;                        
        ///  //Create a solid brush
        ///  PdfBrush brush = new PdfSolidBrush(Color.Black);          
        ///  float fontSize = 8f;
        ///  //Set the font
        ///  PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, fontSize);            
        ///  //Draw the text
        ///  g.DrawString("HelloWorld", font, brush, new RectangleF(47.835f, 236.835f, 564.165f, 553.937f));           
        ///  doc.Save("Dispose.pdf");
        ///  // Dispose the object
        ///  doc.Dispose();
        /// </code>
        /// <code lang="VB">
        ///  'Load an existing document
        ///  Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        ///  'Create a page
        ///  Dim page As PdfPageBase = doc.Pages.Add()
        ///  'Create Pdf graphics for the page
        ///  Dim g As PdfGraphics = page.Graphics
        ///  'Create a solid brush
        ///  Dim brush As PdfBrush = New PdfSolidBrush(Color.Black)
        ///  Dim fontSize As Single = 8f
        ///  'Set the font
        ///  Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.TimesRoman, fontSize)
        ///  'Draw the text
        ///  g.DrawString("HelloWorld", font, brush, New RectangleF(47.835f, 236.835f, 564.165f, 553.937f))
        ///  doc.Save("Dispose.pdf")
        ///  ' Dispose the object
        ///  doc.Dispose()
        /// </code>
        /// </example>
		public void Dispose()
		{
            if (EnableMemoryOptimization)
                Close(true);
            else
                Dispose(true);
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="dispose"><c>true</c> to release both managed
		/// and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose( bool dispose )
		{
			if( !m_isDisposed )
			{
				m_isDisposed = true;
				//if( m_bCloseStream )
				//{
				//  m_stream.Close();
				//}

                if (dispose && EnableMemoryOptimization)
                {
                    // There is no resources requred explicit disposing or inheriting from IDisposable.
                    if (m_bookmark != null)
                        m_bookmark.Clear();
                    if (m_bookmarkHashtable != null)
                        m_bookmarkHashtable.Clear();
                    m_documentInfo = null;
                    m_form = null;
                    m_internalStream = null;
                    m_openStream = null;
                    m_pageLabel = null;
                    m_pageLabelCollection = null;
#if !NETFX_CORE && !WP
                    if (m_stream != null)
                        m_stream.Close();
# endif

#if !SILVERLIGHT && !NETFX_CORE && !WP
                    m_dublinschema = null;
                    if (m_usedFonts != null)
                        m_usedFonts.Clear();
# endif
                }

				m_stream = null;
				m_form = null;
				m_pages = null;
				m_bookmark = null;
			}
		}

		/// <summary>
		/// Closes the document.
		/// </summary>
		/// <param name="completely">if set to <c>true</c> the document should
		/// close its stream as well.</param>
		public override void Close( bool completely )
		{
            if(completely && m_pages != null && EnableMemoryOptimization)
                m_pages.Clear();

			base.Close( completely );
            if (EnableMemoryOptimization)
                Dispose(completely);
            else
                Dispose();
		}

        /// <summary>
        /// Creates a shallow copy of the current document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceDoc.pdf");
        /// // Clone the existing the document
        /// PdfLoadedDocument doc1 = doc.Clone() as PdfLoadedDocument;
        /// // Save the cloned document to a disk
        /// doc1.Save("ClonedPDF.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// ' Clone the existing the document
        /// Dim doc1 As PdfLoadedDocument = TryCast(doc.Clone(), PdfLoadedDocument)
        /// ' Save the cloned document to a disk
        /// doc1.Save("ClonedPDF.pdf")
        /// </code>
        /// </example>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
		#endregion

		#region Implementation

        /// <summary>
        /// Set the Page Label
        /// </summary>
        internal void PageLabel()
        {
            PdfDictionary labelsDic = Catalog[DictionaryProperties.PageLabels] as PdfDictionary;

            if (labelsDic == null)
            {
                labelsDic = new PdfDictionary();
                Catalog[DictionaryProperties.PageLabels] = labelsDic;
            }

            PdfArray labels = new PdfArray();
            labelsDic[DictionaryProperties.Nums] = labels;


            IPdfPrimitive obj = Catalog[DictionaryProperties.Pages];
            PdfDictionary node = CrossTable.GetObject(obj) as PdfDictionary;
            PdfArray m_sectionarray = node[DictionaryProperties.Kids] as PdfArray;

            int pageIndex = 0;
            for (int i = 0; i < m_sectionarray.Count; i++)
            {
                PdfPageLabel label = m_pageLabelCollection[i] as PdfPageLabel;

                if (label == null)
                {
                    label = new PdfPageLabel();
                }
                labels.Add(new PdfNumber(pageIndex));
                PdfReferenceHolder kidrefHold = m_sectionarray[i] as PdfReferenceHolder;
                PdfDictionary kidsDic = CrossTable.GetObject(kidrefHold) as PdfDictionary;
                PdfArray kidsarray = kidsDic[DictionaryProperties.Kids] as PdfArray;
                pageIndex = pageIndex + kidsarray.Count;
                labels.Add(((IPdfWrapper)label).Element);
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
		/// Gets a value indicating whether the document was encrypted.
		/// </summary>
		internal override bool WasEncrypted
		{
			get
			{
				return m_bWasEncrypted;
			}
		}
        public bool IsEncrypted
        {
            get
            {
                return m_bWasEncrypted;
            }
        }
		/// <summary>
		/// Creates the bookmark destination dictionary.
		/// </summary>
		/// <returns>Hashtable that uses destination as a key and bookmark list as a value.</returns>
        internal Dictionary<PdfPageBase, object> CreateBookmarkDestinationDictionary()
		{
            //Dictionary<PdfPageBase, object> table = null;
			PdfBookmarkBase current = Bookmarks;

            if (m_bookmarkHashtable == null && current != null)
            {
                m_bookmarkHashtable = new Dictionary<PdfPageBase, object>();
                Stack<CurrentNodeInfo> stack = new Stack<CurrentNodeInfo>();
                CurrentNodeInfo ni = new CurrentNodeInfo(current.List);

                do
                {
                    for (; ni.Index < ni.Kids.Count; )
                    {
                        current = ni.Kids[ni.Index] as PdfBookmarkBase;

                        // Get destination reference and put in into the table.
                        PdfDestination dest = (current as PdfBookmark).Destination;

                        if (dest!=null)
                        {
                            PdfPageBase page = dest.Page;

                            List<object> list = m_bookmarkHashtable.ContainsKey(page) ?
                                m_bookmarkHashtable[page] as List<object> : null;


                            if (list == null)
                            {
                                list = new List<object>();
                                m_bookmarkHashtable[page] = list;
                            }

                            list.Add(current);
                        }

                        ++ni.Index;

                        // Go deeper.
                        if (current.Count > 0)
                        {
                            stack.Push(ni);
                            ni = new CurrentNodeInfo(current.List);
                            continue;
                        }
                    }

                    if (stack.Count > 0)
                    {
                        ni = stack.Pop();
                        while ((ni.Index == ni.Kids.Count) && (stack.Count > 0))
                        {
                            ni = stack.Pop();

                        }
                    }
                } while (ni.Index < ni.Kids.Count);
            }

			return m_bookmarkHashtable;
		}

		/// <summary>
		/// Gets the named destination.
		/// </summary>
		/// <param name="name">The name of the destination.</param>
		/// <returns>The direct destination.</returns>
		internal PdfArray GetNamedDestination( PdfName name )
		{
			PdfDictionary destinations = Catalog.Destinations;
			IPdfPrimitive obj = destinations[ name ];
			PdfArray destination = ExtractDestination( obj );

			return destination;
		}

		/// <summary>
		/// Gets the named destination.
		/// </summary>
		/// <param name="name">The name of the destination.</param>
		/// <returns>A direct destination.</returns>
		internal PdfArray GetNamedDestination( PdfString name )
		{
			PdfCatalogNames names = Catalog.Names;
			PdfArray destination = null;

			if( name != null )
			{
				PdfDictionary destinations = names.Destinations;
				IPdfPrimitive obj = names.GetNamedObjectFromTree( destinations, name );

				destination = ExtractDestination( obj );
			}
			return destination;
		}

		/// <summary>
		/// Extracts the destination from dictionary or returns the object.
		/// </summary>
		/// <param name="obj">The destination object.</param>
		/// <returns>The destination array.</returns>
		private static PdfArray ExtractDestination( IPdfPrimitive obj )
		{
			PdfDictionary dic = obj as PdfDictionary;
			PdfArray destination = obj as PdfArray;

			if( dic != null )
			{
				obj = PdfCrossTable.Dereference( dic[ DictionaryProperties.D ] );
				destination = obj as PdfArray;
			}
			return destination;
		}

		/// <summary>
		/// Loads the document.
		/// </summary>
		/// <param name="file">The file.</param>
		private void LoadDocument( Stream file )
		{
			if( !file.CanRead || !file.CanSeek )
				throw new ArgumentException( "Can't use the specified stream.", "file" );

			m_stream = file;

			PdfMainObjectCollection objects = new PdfMainObjectCollection();
			SetMainObjectCollection( objects );

			PdfCrossTable crossTable = new PdfCrossTable( file );
			crossTable.Document = this;
            if (crossTable.StructureAltered)
                crossTable.Document.FileStructure.IncrementalUpdate = false;
            SetCrossTable( crossTable );

			// Check if the document is encrypted.
			m_bWasEncrypted = CheckEncryption();
			// Create catalog
			SetCatalog( GetCatalog());
			// Create user preferences.

			// Read document's info dictionary if present.
			ReadDocumentInfo();

            // Read the document`s version
            ReadFileVersion();

            // Check whether the loaded PDF is tagged one.
            CheckIfTagged();
		}

        /// <summary>
        /// Check whether the loaded PDF is tagged one.
        /// </summary>
        private void CheckIfTagged()
        {
                PdfDictionary dic = CrossTable.DocumentCatalog[DictionaryProperties.MarkInfo] as PdfDictionary;
                if (dic != null)
                {
                    if (dic.ContainsKey(DictionaryProperties.Marked))
                    {
                        PdfBoolean isTagged = dic[DictionaryProperties.Marked] as PdfBoolean;
                        this.FileStructure.TaggedPdf = isTagged.Value;
                    }
                } 
        }

        /// <summary>
        /// Gets the PDF version
        /// </summary>
        private void ReadFileVersion()
        {
            PdfReader m_reader = new PdfReader(m_stream);            

            m_reader.Position = 0;
            string token = m_reader.GetNextToken();
            if (token.StartsWith("%"))
            {
                token = m_reader.GetNextToken();

                if (token != null)
                {
                    switch (token)
                    {
                        case "PDF-1.4" :
                            this.FileStructure.Version = PdfVersion.Version1_4;
                            break;
                        case "PDF-1.0":
                            this.FileStructure.Version = PdfVersion.Version1_0;
                            this.FileStructure.IncrementalUpdate = false;
                            break;
                        case "PDF-1.1":
                            this.FileStructure.Version = PdfVersion.Version1_1;
                            this.FileStructure.IncrementalUpdate = false;
                            break;
                        case "PDF-1.2":
                            this.FileStructure.Version = PdfVersion.Version1_2;
                            this.FileStructure.IncrementalUpdate = false;
                            break;
                        case "PDF-1.3":
                            this.FileStructure.Version = PdfVersion.Version1_3;
                            this.FileStructure.IncrementalUpdate = false;
                            break;
                        case "PDF-1.5":
                            this.FileStructure.Version = PdfVersion.Version1_5;
                            break;
                        case "PDF-1.6":
                            this.FileStructure.Version = PdfVersion.Version1_6;
                            break;                      
                        case "PDF-1.7":
                            this.FileStructure.Version = PdfVersion.Version1_7;
                            break;
                      
                    }
                }
            }
        }

		/// <summary>
		/// Gets the catalog of the loaded document.
		/// </summary>
		/// <returns>The PdfCatalog instance.</returns>
		private PdfCatalog GetCatalog()
		{
			PdfCatalog catalog = new PdfCatalog( this, CrossTable.DocumentCatalog );
			PdfObjects.ReregisterReference( CrossTable.DocumentCatalog, catalog );
            if (!CrossTable.IsMerging)
                catalog.Position = -1;
            PdfDictionary dic = catalog as PdfDictionary;
            if (dic != null)
            {
                CheckNeedAppearence(dic);
            }
			return catalog;
		}

        /// <summary>
        /// To check the Need Appearance.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        private void CheckNeedAppearence(PdfDictionary dictionary)
        {
            if (dictionary.ContainsKey(DictionaryProperties.AcroForm))
            {
                if (dictionary[DictionaryProperties.AcroForm] is PdfReferenceHolder)
                {
                    PdfReferenceHolder acroform = dictionary[DictionaryProperties.AcroForm] as PdfReferenceHolder;
                    PdfDictionary acroformdicionary = acroform.Object as PdfDictionary;
                    if (acroformdicionary!=null && acroformdicionary.ContainsKey(DictionaryProperties.XFA))
                    {
                        this.IsXFAForm = true;
                    }
                    else if (acroformdicionary!=null && acroformdicionary.ContainsKey(DictionaryProperties.NeedAppearances))
                    {
                        this.IsXFAForm = true;
                    }
                }
                else if (dictionary[DictionaryProperties.AcroForm] is PdfDictionary)
                {
                    PdfDictionary acroformdicionary = dictionary[DictionaryProperties.AcroForm] as PdfDictionary;
                    if (acroformdicionary.ContainsKey(DictionaryProperties.XFA))
                    {
                        this.IsXFAForm = true;
                    }
                    else if (acroformdicionary.ContainsKey(DictionaryProperties.NeedAppearances))
                    {
                        this.IsXFAForm = true;
                    }
                }
            }
        }

		/// <summary>
		/// Read and parse the document's info dictionary.
		/// </summary>
		private void ReadDocumentInfo()
		{
			// Read document's info if present.
			PdfDictionary info = PdfCrossTable.Dereference( CrossTable.Trailer[ DictionaryProperties.Info ] ) as PdfDictionary;

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (info != null && this.m_bWasEncrypted && this.Catalog.Metadata != null)
            {
                XmpMetadata xmp = this.Catalog.Metadata;
                if (info.ContainsKey("Producer") && (xmp.PDFSchema != null && xmp.PDFSchema.Producer != string.Empty))
                {
                    if (xmp.PDFSchema.Producer != (info["Producer"] as PdfString).Value)
                    {
                        info["Producer"] = new PdfString(xmp.PDFSchema.Producer);
                    }
                }
                if (info.ContainsKey("Author") && (DublinSchema.Creator.Items != null && (DublinSchema.Creator.Items[0] != string.Empty)))
                {
                    if (xmp.DublinCoreSchema.Creator.Items[0] != (info["Author"] as PdfString).Value)
                    {
                        info["Author"] = new PdfString(DublinSchema.Creator.Items[0]);
                    }
                }
				if(xmp.XmlData.InnerText.Contains("Title"))
                if (info.ContainsKey("Title") && (DublinSchema != null && (DublinSchema.Title.DefaultText != string.Empty)))
                {
                    if (xmp.DublinCoreSchema.Title.DefaultText != (info["Title"] as PdfString).Value)
                    {
                        info["Title"] = new PdfString(DublinSchema.Title.DefaultText);
                    }
                }
                if (info.ContainsKey("Creator") && (xmp.BasicSchema != null && (xmp.BasicSchema.CreatorTool!= string.Empty)))
                {
                    if (xmp.BasicSchema.CreatorTool != (info["Creator"] as PdfString).Value)
                    {
                        info["Creator"] = new PdfString(xmp.BasicSchema.CreatorTool);
                    }
                }
                if (info.ContainsKey("CreationDate") && (xmp.BasicSchema != null && (xmp.BasicSchema.CreateDate.ToString() != string.Empty)))
                {
                    if (xmp.BasicSchema.CreateDate.ToString() != (info["CreationDate"] as PdfString).Value)
                    {
                        info["CreationDate"] = new PdfString(xmp.BasicSchema.CreateDate.ToString("yyyyMMddHHmmss"));
                    }
                }
                if (info.ContainsKey("ModDate") && (xmp.BasicSchema != null && (xmp.BasicSchema.ModifyDate.ToString() != string.Empty)))
                {
                    if (xmp.BasicSchema.ModifyDate.ToString() != (info["ModDate"] as PdfString).Value)
                    {
                        info["ModDate"] = new PdfString(xmp.BasicSchema.ModifyDate.ToString("yyyyMMddHHmmss"));
                    }
                }
            }
#endif
			if( info != null )
			{
				m_documentInfo = new PdfDocumentInformation( info, Catalog );
				PdfObjects.ReregisterReference( info, ( m_documentInfo as IPdfWrapper ).Element );
                (m_documentInfo as IPdfWrapper).Element.Position = -1;
			}
		}

		/// <summary>
		/// Checks whether the PDF document was encrypted.
		/// </summary>
		/// <returns>True if the document was encrypted.</returns>
		private bool CheckEncryption()
		{
			bool wasEncrypted = false;

			// Read Security if present.
			PdfDictionary trailer = CrossTable.Trailer;

			IPdfPrimitive obj;
			PdfDictionary encDic = CrossTable.EncryptorDictionary;

            bool isEncrpt = true;
            //if(encDic != null && encDic.ContainsKey( DictionaryProperties.EncryptMetadata ))
            //    isEncrpt = (encDic[DictionaryProperties.EncryptMetadata] as PdfBoolean).Value;


            if (encDic != null && isEncrpt) // Encryption dictionary have been found.
			{
				if( m_password == null )
					m_password = string.Empty;

				obj = trailer[ DictionaryProperties.ID ];

				if( obj == null )
					throw new PdfDocumentException(
						"Unable to decrypt document without ID." );

				PdfArray id = obj as PdfArray;
                // The first element of ID is used in the decryption algorithm.
				PdfString key = id[ 0 ] as PdfString;
                wasEncrypted = true;

#if !SILVERLIGHT && !WP
				PdfEncryptor encryptor = new PdfEncryptor();// this, encDic, m_password, key );
                if (encDic != null && encDic.ContainsKey(DictionaryProperties.EncryptMetadata))
                    encryptor.EncryptMetaData = (encDic[DictionaryProperties.EncryptMetadata] as PdfBoolean).Value;



				encryptor.ReadFromDictionary( encDic );

				if( !encryptor.CheckPassword( m_password, key ) )
				{
                    this.Close(true);
					throw new PdfDocumentException( "Can't open an encrypted document. The password is invalid." );
				}

				encDic.Encrypt = false;

				PdfSecurity security = new PdfSecurity();

				security.Encryptor = encryptor;
				SetSecurity( security );
				

				CrossTable.Encryptor = encryptor;
#endif
            }

			return wasEncrypted;
		}

		/// <summary>
		/// Gets the form dictionary.
		/// </summary>
		/// <returns>The form dictionary.</returns>
		private PdfDictionary GetFormDictionary()
		{
			PdfCatalog catalog = Catalog;
			PdfDictionary formDic = PdfCrossTable.Dereference(
				catalog[ DictionaryProperties.AcroForm ] ) as PdfDictionary;

			return formDic;
		}


        /// <summary>
        /// Gets the form dictionary.
        /// </summary>
        /// <returns>The form dictionary.</returns>
        private PdfDictionary GetAttachmentDictionary()
        {
            PdfCatalog catalog = Catalog;
            PdfDictionary attachmentDic = PdfCrossTable.Dereference(
                catalog[DictionaryProperties.Names]) as PdfDictionary;

            return attachmentDic;
        }

        /// <summary>
        /// Get the collection dictionary
        /// </summary>
        /// <returns></returns>
        private PdfDictionary GetPortfolioDictionary()
        {
            PdfCatalog catalog = Catalog;
            PdfDictionary portfolioDictionary = PdfCrossTable.Dereference(catalog[DictionaryProperties.Collection]) as PdfDictionary;

            return portfolioDictionary;
        }

		/// <summary>
		/// Appends the new document data.
		/// </summary>
		/// <param name="writer">The writer.</param>
		private void AppendDocument( PdfWriter writer )
		{
			writer.Document = this;
            if (isPageLabel == true)
            {
                PageLabel();
            }
			CrossTable.Save( writer );

			DocumentSavedEventArgs argsSaved = new DocumentSavedEventArgs( writer );
			OnDocumentSaved( argsSaved );
		}

		/// <summary>
		/// Copies the old stream.
		/// </summary>
		/// <param name="writer">The writer.</param>
		private void CopyOldStream( PdfWriter writer )
		{
			long length = m_stream.Length;
			byte[] data = new byte[ length ];

			m_stream.Position = 0;
			m_stream.Read( data, 0, ( int )length );

			writer.Write( data );
		}

        private Stream CheckIfValid(Stream file)
        {
            file.Position = file.Length - 1;
            int last = file.ReadByte();
            if (last == 0)
            {
                byte[] data = new byte[file.Length];
                file.Position = 0;
                file.Read(data, 0, data.Length);

                //find the first index of the null byte
                int index = data.Length - 1;
                while (data[index] == 0)

                    --index;
                byte[] trimmedData = new byte[index + 1];
                Array.Copy(data, trimmedData, index + 1);
                //if (data.Length != trimmedData.Length)
                //{
                MemoryStream stream = new MemoryStream();
                stream.Write(trimmedData, 0, trimmedData.Length);
                file.Dispose();

                return stream;
            }
            else
                file.Position = 0;
            return file;
        }

        private bool CheckLinearization()
        {
            bool linear = false;
            long position=0;

            PdfReader reader = new PdfReader(this.m_stream);
            string token = " ";
            try
            {
                position = reader.SearchForward("Linearized");
            }

            catch(Exception e)
            {
                if(e.Message.Equals("Invalid/Unknown/Unsupported format"))
                return linear;
            }

            if (position != 0)
            {
                while (true)
                {
                    token = reader.GetNextToken();
                    if (token.Equals("L"))
                    {
                        token = reader.GetNextToken();
                        long length = this.m_stream.Length;
                        if (token.Equals(length.ToString()))
                        {
                            linear = true;
                            break;
                        }
                        else
                        {
                            break;
                        }

                    }

                }
            }

            return linear;
        }
		#endregion

		#region Internals
		/// <summary>
		/// Stores info about current node.
		/// </summary>
		private class CurrentNodeInfo
		{
			#region Members
            /// <summary>
            /// Internal variable used to store Kids values.
            /// </summary>
			public List<PdfBookmarkBase> Kids;
            /// <summary>
            ///  Internal variable used to store index value.
            /// </summary>
			public int Index;
			#endregion

			#region Constructors
			/// <summary>
			/// Initializes a new instance of the <see cref="CurrentNodeInfo"/> class.
			/// </summary>
			/// <param name="kids">The kids.</param>
            public CurrentNodeInfo(List<PdfBookmarkBase> kids)
			{
				Kids = kids;
				Index = 0;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="CurrentNodeInfo"/> class.
			/// </summary>
			/// <param name="kids">The kids.</param>
			/// <param name="index">The index.</param>
            public CurrentNodeInfo(List<PdfBookmarkBase> kids, int index)
				: this( kids )
			{
				Index = index;
			}
			#endregion
		}
		#endregion
	}
}
