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

#if !SILVERLIGHT && !NETFX_CORE && !WP
using Syncfusion.Pdf.Xmp;
#endif

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    ///	A class containing the information about the document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document.
    /// PdfDocument pdfDoc= new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = pdfDoc.Pages.Add();           
    /// //Set the Document`s properties.
    /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
    /// pdfDoc.DocumentInformation.Author = "Syncfusion";
    /// pdfDoc.DocumentInformation.Keywords = "PDF";
    /// pdfDoc.DocumentInformation.Subject = "PDF demo";
    /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
    /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;
    /// //Save the document
    /// pdfDoc.Save("DocumentInformation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new document.
    /// Dim pdfDoc As PdfDocument= New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = pdfDoc.Pages.Add()
    /// 'set the Document`s properties.
    /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
    /// pdfDoc.DocumentInformation.Author = "Syncfusion"
    /// pdfDoc.DocumentInformation.Keywords = "PDF"
    /// pdfDoc.DocumentInformation.Subject = "PDF demo"
    /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
    /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now
    /// 'Save the document
    /// pdfDoc.Save("DocumentInformation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="IPdfWrapper"/> Interface   
    public class PdfDocumentInformation : IPdfWrapper
    {
        #region Fields
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Xmp metadata of the document.
        /// </summary>
        private XmpMetadata m_xmp;
#endif

        /// <summary>
        /// Parent catalog class.
        /// </summary>
        private PdfCatalog m_catalog;

        /// <summary>
        /// Author of the document.
        /// </summary>
        private string m_author;

        /// <summary>
        /// Title of the document.
        /// </summary>
        private string m_title;

        /// <summary>
        /// The subject of the document.
        /// </summary>
        private string m_subject;

        /// <summary>
        /// Keywords associated with the document.
        /// </summary>
        private string m_keywords;

        /// <summary>
        /// If the document was converted to PDF from another format, the name of 
        /// the application that created the original document from which it was converted.
        /// </summary>
        private string m_creator;

        /// <summary>
        /// If the document was converted to PDF from another format, the name of the 
        /// application that converted it to PDF.
        /// </summary>
        private string m_producer;

        /// <summary>
        /// The date and time the document was created.
        /// </summary>
        private DateTime m_creationDate = DateTime.Now;

        /// <summary>
        /// The date and time the document was modified.
        /// </summary>
        private DateTime m_modificationDate;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object.
        /// </summary>
        internal PdfDocumentInformation(PdfCatalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

            m_dictionary = new PdfDictionary();

            if (PdfDocument.ConformanceLevel != PdfConformanceLevel.Pdf_A1B)
            {
                m_dictionary.SetDateTime(DictionaryProperties.CreationDate, m_creationDate);
            }

            // This object needs Catalog since Xmp is stored there.
            // Instead of passing this object there, we can subscribe
            // it on BeforeSave of the catalog in PdfDocument class.
            m_catalog = catalog;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentInformation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="catalog">The catalog.</param>
        internal PdfDocumentInformation(PdfDictionary dictionary, PdfCatalog catalog)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

            m_dictionary = dictionary;
            m_catalog = catalog;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument pdfDoc= new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();           
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument= New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public DateTime CreationDate
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.CreationDate] as PdfString;
                if (result == null)
                {
                    return m_creationDate = DateTime.Now;
                }

                m_creationDate = m_dictionary.GetDateTime(result);
                return m_creationDate;
            }

            set
            {
                if (m_creationDate != value)
                {
                    m_creationDate = value;
                    m_dictionary.SetDateTime(DictionaryProperties.CreationDate, m_creationDate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>The modification date.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;
        /// //Sets the modification date
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now
        /// 'Sets the modification date
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public DateTime ModificationDate
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.ModificationDate] as PdfString;
                if (result == null)
                {
                    return m_creationDate = DateTime.Now;
                }

                m_modificationDate = m_dictionary.GetDateTime(result);
                return m_modificationDate;
            }

            set
            {
                if (m_modificationDate != value)
                {
                    m_modificationDate = value;
                    m_dictionary.SetDateTime(DictionaryProperties.ModificationDate, m_modificationDate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;       
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now        
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Title
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Title] as PdfString;

                if (result == null)
                {
                    return m_title = string.Empty;
                }

                m_title = result.Value;

                return m_title;
            }

            set
            {
                if (m_title != value)
                {
                    m_title = value;
                    m_dictionary.SetString(DictionaryProperties.Title, m_title);
                }
            }
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author of the document.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
        /// // Sets the author information
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;       
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
        /// ' Sets the author information
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now        
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Author
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Author] as PdfString;

                if (result == null)
                {
                    return m_author = string.Empty;
                }

                m_author = result.Value;

                return m_author;
            }

            set
            {
                if (m_author != value)
                {
                    m_author = value;
                    m_dictionary.SetString(DictionaryProperties.Author, m_author);
                }
            }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// // Sets the documents subject information
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;       
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// ' Sets the documents subject information
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now        
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Subject
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Subject] as PdfString;

                if (result == null)
                {
                    return m_subject = string.Empty;
                }

                m_subject = result.Value;

                return m_subject;
            }

            set
            {
                if (m_subject != value)
                {
                    m_subject = value;
                    m_dictionary.SetString(DictionaryProperties.Subject, m_subject);
                }
            }
        }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// // Sets the documents Keyword information
        /// pdfDoc.DocumentInformation.Keywords = "PDF";     
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;       
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// ' Sets the documents Keyword information
        /// pdfDoc.DocumentInformation.Keywords = "PDF"      
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now        
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Keywords
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Keywords] as PdfString;

                if (result == null)
                {
                    return m_keywords = string.Empty;
                }

                m_keywords = result.Value;

                return m_keywords;
            }

            set
            {
                if (value != m_keywords)
                {
                    m_keywords = value;
                    m_dictionary.SetString(DictionaryProperties.Keywords, m_keywords);
                }
            }
        }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// //Set the document`s information
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// pdfDoc.DocumentInformation.Keywords = "PDF";
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// // Sets the documents creator information
        /// pdfDoc.DocumentInformation.Creator = "Essential PDF";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set the document`s information.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// pdfDoc.DocumentInformation.Keywords = "PDF"
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// ' Sets the documents creator information
        /// pdfDoc.DocumentInformation.Creator = "Essential PDF"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now     
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Creator
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Creator] as PdfString;

                if (result == null)
                {
                    return m_creator = string.Empty;
                }

                m_creator = result.Value;

                return m_creator;
            }

            set
            {
                if (m_creator != value)
                {
                    m_creator = value;
                    m_dictionary.SetString(DictionaryProperties.Creator, m_creator);
                }
            }
        }

        /// <summary>
        /// If the document was converted to PDF from another format,
        /// the name of the application (for example, Acrobat Distiller)
        /// that converted it to PDF.
        /// </summary>
        /// <value>The producer of the document.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument pdfDoc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = pdfDoc.Pages.Add();
        /// // Set the various Document properties.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information";        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion";
        /// // Sets the documents Keyword information
        /// pdfDoc.DocumentInformation.Keywords = "PDF";     
        /// pdfDoc.DocumentInformation.Subject = "PDF demo";
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software";
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now;       
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now;
        /// //Save the document
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// 'Set various Document properties.
        /// pdfDoc.DocumentInformation.Title = "Document Properties Information"        
        /// pdfDoc.DocumentInformation.Author = "Syncfusion"
        /// ' Sets the documents Keyword information
        /// pdfDoc.DocumentInformation.Keywords = "PDF"      
        /// pdfDoc.DocumentInformation.Subject = "PDF demo"
        /// pdfDoc.DocumentInformation.Producer = "Syncfusion Software"
        /// pdfDoc.DocumentInformation.CreationDate = DateTime.Now        
        /// pdfDoc.DocumentInformation.ModificationDate = DateTime.Now
        /// 'Save the document
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public string Producer
        {
            get
            {
                PdfString result = m_dictionary[DictionaryProperties.Producer] as PdfString;

                if (result == null)
                {
                    return m_producer = string.Empty;
                }

                m_producer = result.Value;

                return m_producer;
            }

            set
            {
                if (m_producer != value)
                {
                    m_producer = value;
                    m_dictionary.SetString(DictionaryProperties.Producer, m_producer);
                }
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets Xmp metadata of the document.
        /// </summary>
        /// <value>Represents the document information in Xmp format.</value>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument pdfDoc = new PdfDocument();
        /// PdfPage page = pdfDoc.Pages.Add();
        /// // Get xmp object.
        /// XmpMetadata xmp = pdfDoc.DocumentInformation.XmpMetadata;
        /// // XMP Basic Schema.
        /// BasicSchema basic = xmp.BasicSchema;
        /// basic.Advisory.Add("advisory");
        /// basic.BaseURL = new Uri("http://google.com");
        /// basic.CreateDate = DateTime.Now;
        /// basic.CreatorTool = "creator tool";
        /// basic.Identifier.Add("identifier");
        /// basic.Label = "label";
        /// basic.MetadataDate = DateTime.Now;
        /// basic.ModifyDate = DateTime.Now;
        /// basic.Nickname = "nickname";
        /// basic.Rating.Add(-25);
        /// pdfDoc.Save("DocumentInformation.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim pdfDoc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = pdfDoc.Pages.Add()
        /// ' Get xmp object.
        /// Dim xmp As XmpMetadata = pdfDoc.DocumentInformation.XmpMetadata
        /// ' XMP Basic Schema.
        /// Dim basic As BasicSchema = xmp.BasicSchema
        /// basic.Advisory.Add("advisory")
        /// basic.BaseURL = New Uri("http://google.com")
        /// basic.CreateDate = DateTime.Now
        /// basic.CreatorTool = "creator tool"
        /// basic.Identifier.Add("identifier")
        /// basic.Label = "label"
        /// basic.MetadataDate = DateTime.Now
        /// basic.ModifyDate = DateTime.Now
        /// basic.Nickname = "nickname"
        /// basic.Rating.Add(-25)
        /// pdfDoc.Save("DocumentInformation.pdf")
        /// </code>
        /// </example>
        public XmpMetadata XmpMetadata
        {
            get
            {
                if (m_xmp == null)
                {
                    if (m_catalog.Metadata == null)
                    {
                        m_xmp = new XmpMetadata(m_catalog.Pages.Document.DocumentInformation);
                        m_catalog.SetProperty(DictionaryProperties.Metadata, new PdfReferenceHolder(m_xmp));
                    }
                    else
                    {
                        m_xmp = m_catalog.Metadata;
                    }
                }

                return m_xmp;
            }
        }
#endif

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Applies to attributes to attain PDF/X1a:2001 conformance.
        /// </summary>
        internal void ApplyPdfXConformance()
        {
            Dictionary[DictionaryProperties.GTS_PDFXConformance] = new PdfString("PDF/X-1a:2001");
            Dictionary[DictionaryProperties.Trapped] = new PdfName("False");
            Dictionary[DictionaryProperties.GTS_PDFXVersion] = new PdfString("PDF/X-1:2001");
            ModificationDate = DateTime.Now;
            if (Title == string.Empty)
            {
                Title = " ";
            }
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
                return m_dictionary;
            }
        }
        #endregion
    }
}
