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
using System.IO;
#if !NETFX_CORE && !WP
using System.Security.Permissions;
#endif
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;
#if NETFX_CORE || WP
using Windows.Storage;
using System.Threading.Tasks;
#endif


/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represent common properties of PdfDocument and PdfLoadedDocument classes.
    /// </summary>
    public abstract class PdfDocumentBase
    {
        #region Fields
        /// <summary>
        /// Collection of the main objects.
        /// </summary>
        private PdfMainObjectCollection m_objects;

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Security properties.
        /// </summary>
        private PdfSecurity m_security;
#endif
        /// <summary>
        /// Object that is saving currently.
        /// </summary>
        private PdfReference m_currentSavingObj;

        /// <summary>
        /// Document catlog.
        /// </summary>
        private PdfCatalog m_catalog;

        /// <summary>
        /// Cross table.
        /// </summary>
        private PdfCrossTable m_crossTable;

        /// <summary>
        /// Document information and properties.
        /// </summary>
        private PdfDocumentInformation m_documentInfo;

        /// <summary>
        /// String contain either user or owner password.
        /// </summary>
        private string m_password;

        /// <summary>
        /// Desired level of the new stream compression.
        /// </summary>
        private PdfCompressionLevel m_compression =
#if DEBUG
 PdfCompressionLevel.None;
#else
 PdfCompressionLevel.Normal;
#endif

        /// <summary>
        /// Specifies file structure.
        /// </summary>
        private PdfFileStructure m_fileStructure;

        /// <summary>
        /// A list of the objects that have to be disposed after document closing.
        /// </summary>
        private List<IDisposable> m_disposeObjects;

        /// <summary>
        /// Internal variable to store if memory optimization should be done.
        /// </summary>
        private bool m_enableMemoryOptimization;

        /// <summary>
        /// Internal varible to store portfolio
        /// </summary>
        private PdfPortfolioInformation m_portfolio;

        #endregion

        #region Delegates
        /// <summary>
        /// Delegate. Is used for raising events after document saving.
        /// </summary>
        internal delegate void DocumentSavedEventHandler(object sender, DocumentSavedEventArgs args);
        #endregion

        #region Events
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event. Rises when the document has been saved.
        /// </summary>
        internal event DocumentSavedEventHandler DocumentSaved;
        #endregion

        #region Properties
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the security parameters of the document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Reading security settings of the document
        /// PdfSecurity security = doc.Security;
        /// doc.Save("Security.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Reading security settings of the document
        /// Dim security As PdfSecurity = doc.Security
        /// doc.Save("Security.pdf")
        /// </code>
        /// </example>
        public PdfSecurity Security
        {
            get
            {
                if (m_security == null)
                {
                    m_security = new PdfSecurity();
                }

                return m_security;
            }
        }
#endif
        /// <summary>
        /// Gets a value indicating whether this instance is security granted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is security granted; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsSecurityGranted
        {
            get
            {
                bool bResult = false;
#if !SILVERLIGHT && !NETFX_CORE && !WP
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);

                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (System.Security.SecurityException)
                {
                }
#endif
                return bResult;
            }
        }

        /// <summary>
        /// Gets or sets document's information and properties.
        /// </summary>
        virtual public PdfDocumentInformation DocumentInformation
        {
            get
            {
                if (m_documentInfo == null)
                {
                    // This object needs Catalog since Xmp is stored there.
                    // Instead of passing this object there, we can subscribe it on BeforeSave of the catalog.
                    m_documentInfo = new PdfDocumentInformation(Catalog);
                    CrossTable.Trailer[DictionaryProperties.Info] = new PdfReferenceHolder(m_documentInfo);
                }

                return m_documentInfo;
            }
        }

        /// <summary>
        /// Gets or sets a viewer preferences object controlling the way the document is to be 
        /// presented on the screen or in print.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set the document`s viewer preference
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.TwoPageLeft;
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.AppDefault;
        /// doc.ViewerPreferences.PageMode = PdfPageMode.FullScreen;
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set the document`s viewer preference
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.TwoPageLeft
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.AppDefault
        /// doc.ViewerPreferences.PageMode = PdfPageMode.FullScreen
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public PdfViewerPreferences ViewerPreferences
        {
            get
            {
                if (m_catalog.ViewerPreferences == null)
                {
                    m_catalog.ViewerPreferences = new PdfViewerPreferences(m_catalog);
                }

                return m_catalog.ViewerPreferences;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ViewerPreferences");
                }

                m_catalog.ViewerPreferences = value;
            }
        }

        /// <summary>
        /// Gets or sets the desired level of stream compression.
        /// </summary>
        /// <remarks>All new objects should be compressed with this level of the compression.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// // Set the document`s compression level
        /// doc.Compression = PdfCompressionLevel.Best;
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// doc.Save("Compression.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// '  Set the document`s compression level
        /// doc.Compression = PdfCompressionLevel.Best
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// doc.Save("Compression.pdf")
        /// </code>
        /// </example>
        public PdfCompressionLevel Compression
        {
            get
            {
                return m_compression;
            }

            set
            {
                m_compression = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal structure of the PDF file.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s cross reference Type
        /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream;
        /// // Set the pdf version
        /// doc.FileStructure.Version = PdfVersion.Version1_6;
        /// // Save the document
        /// doc.Save("FileStructure.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// '  Set the document`s cross reference Type
        /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream
        /// ' set the pdf version
        /// doc.FileStructure.Version = PdfVersion.Version1_6
        /// ' Save the document
        /// doc.Save("FileStructure.pdf")
        /// </code>
        /// </example>
        public PdfFileStructure FileStructure
        {
            get
            {
                if (m_fileStructure == null)
                {
                    m_fileStructure = new PdfFileStructure();
                    m_fileStructure.TaggedPdfChanged += new EventHandler(m_fileStructure_TaggedPdfChanged);
                }

                return m_fileStructure;
            }

            set
            {
                m_fileStructure = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the pdf Portfolio to the Document.
        /// </summary>
        public PdfPortfolioInformation PortfolioInformation
        {

            get
            {
                return m_portfolio;
            }
            set
            {
                m_portfolio = value;
                m_catalog.PdfPortfolio = m_portfolio;
            }
        }

        /// <summary>
        /// If PDF is set as tagged, initializes StructTree in the catalog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_fileStructure_TaggedPdfChanged(object sender, EventArgs e)
        {
# if !SILVERLIGHT && !NETFX_CORE && !WP
            if (m_fileStructure.TaggedPdf)
                Catalog.InitializeStructTreeRoot();
# endif
        }

        /// <summary>
        /// Gets the bookmarks.
        /// </summary>
        public abstract PdfBookmarkBase Bookmarks { get; }

        /// <summary>
        /// Gets a value indicating whether the document was encrypted.
        /// </summary>
        internal abstract bool WasEncrypted
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the document was Pdf viewer document.
        /// </summary>
        internal abstract bool IsPdfViewerDocumentDisable
        {
            get;
            set;
        }
        //internal bool IsPdfViewerDocumentDisable = true;
        /// <summary>
        /// Gets the PDF objects collection, which stores all objects and references to it..
        /// </summary>
        internal PdfMainObjectCollection PdfObjects
        {
            get
            {
                return m_objects;
            }
        }

        /// <summary>
        /// Gets or sets the current saving object number.
        /// </summary>
        internal PdfReference CurrentSavingObj
        {
            get
            {
                return m_currentSavingObj;
            }

            set
            {
                m_currentSavingObj = value;
            }
        }

        /// <summary>
        /// Gets the cross-reference table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Gets the PDF document catalog.
        /// </summary>
        internal PdfCatalog Catalog
        {
            get
            {
                return m_catalog;
            }
        }

        /// <summary>
        /// Gets a list of the objects that have to be disposed after document closing.
        /// </summary>
        internal List<IDisposable> DisposeObjects
        {
            get
            {
                if (m_disposeObjects == null)
                {
                    m_disposeObjects = new List<IDisposable>();
                }

                return m_disposeObjects;
            }
        }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
#if NETFX_CORE || WP
        public abstract int PageCount
#else
        internal abstract int PageCount
#endif
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether to optimize memory.
        /// </summary>
        /// <remarks>Optimization will be effective only with merge, append and import functions. 
        /// Only memory will be optimized, different in time occur based on the document size.
        /// </remarks>
        public bool EnableMemoryOptimization
        {
            get
            {
                return m_enableMemoryOptimization;
            }
            set
            {
                m_enableMemoryOptimization = value;
            }
        }

        #endregion

        #region Public Static Methods
#if !NETFX_CORE && !WP
        /// <summary>
        /// Merges the specified source documents and return destination document.
        /// </summary>
        /// <param name="dest">The destination document, where the other documents are merged into.
        /// If it's null a new document object will be created.</param>
        /// <param name="sourceDocuments">The source documents.</param>
        /// <returns>The document containing merged documents.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Destination PDF document
        /// PdfDocument destDoc = new PdfDocument();
        /// // Source PDF documents
        /// string[] source = { "Src1.pdf", "Src2.pdf" };
        /// //Merge the source pdf document.
        /// PdfDocumentBase.Merge(destDoc, source);
        /// destDoc.Save("Merge.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Destination PDF document
        /// Dim destDoc As PdfDocument = New PdfDocument()
        /// ' Source PDF documents
        /// Dim source() As String = { "Src1.pdf", "Src2.pdf" }
        /// 'Merge the source pdf document.
        /// PdfDocumentBase.Merge(destDoc, source)
        /// destDoc.Save("Merge.pdf")
        /// </code>
        /// </example>
        public static PdfDocumentBase Merge(PdfDocumentBase dest, params object[] sourceDocuments)
        {
            if (dest == null)
            {
                dest = new PdfDocument(true);
            }
            else
                dest.CrossTable.IsMerging = true;

            for (int i = 0, count = sourceDocuments.Length; i < count; ++i)
            {
                object obj = sourceDocuments[i];
                string path = obj as string;
                Stream stream = obj as Stream;
                byte[] data = obj as byte[];
                PdfLoadedDocument ldDoc = obj as PdfLoadedDocument;
                bool closeDocument = true;

                if (path != null)
                {
                    ldDoc = new PdfLoadedDocument(path);
                }
                else if (stream != null)
                {
                    ldDoc = new PdfLoadedDocument(stream);
                }
                else if (data != null)
                {
                    ldDoc = new PdfLoadedDocument(data);
                }
                else if (ldDoc != null)
                {
                    // Just do nothing. Prevents exception throwing.
                    closeDocument = false;
                }
                else
                {
                    throw new ArgumentException("Unsupported argument type: " + obj.GetType());
                }

                dest.Append(ldDoc);

                if (dest is PdfDocument)
                {
                    if ((dest as PdfDocument).Form != null)
                    {
                        if (ldDoc.Form != null)
                        {
                            if (ldDoc.Form.IsXFAForm == true)
                            {
                                (dest as PdfDocument).Form.NeedAppearances = true;
                                (dest as PdfDocument).Form.IsXFA = true;

                            }
                            else if (ldDoc.IsXFAForm == true)
                            {
                                (dest as PdfDocument).Form.NeedAppearances = true;
                                (dest as PdfDocument).Form.IsXFA = true;
                            }
                        }
                    }
                }

                // We shouldn't close documents that we haven't created.
                if (closeDocument && dest.EnableMemoryOptimization)
                {
                    // The document should be disposed by GC.
                    ldDoc.Close(true);
                }
            }

            return dest;
        }

        /// <summary>
        /// Merges the PDF documents specified by the paths.
        /// </summary>
        /// <param name="paths">The array of string paths.</param>
        /// <returns>A new PDF document containing all merged documents.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Destination PDF document
        /// PdfDocument destDoc = new PdfDocument();
        /// // Source PDF documents
        /// string[] source = { "Src1.pdf", "Src2.pdf" };
        /// //Merge the source pdf document.
        /// destDoc = PdfDocument.Merge(source);
        /// destDoc.Save("Merge.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Destination PDF document
        /// Dim destDoc As PdfDocument = New PdfDocument()
        /// ' Source PDF documents
        /// Dim source() As String = { "Src1.pdf", "Src2.pdf" }
        /// 'Merge the source pdf document.
        /// destDoc = PdfDocument.Merge(source)
        /// destDoc.Save("Merge.pdf")
        /// </code>
        /// </example>
        public static PdfDocument Merge(string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }

            PdfDocument dest = new PdfDocument(true);
            dest.EnableMemoryOptimization = true;

            bool xfaform = false;
            foreach (string path in paths)
            {
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }

                PdfLoadedDocument src = new PdfLoadedDocument(path);
                if (src.IsXFAForm == true)
                {
                    xfaform = true;
                    dest.Form.IsXFA = true;
                }
                else if (src.Form != null)
                {
                    if (src.Form.IsXFAForm == true)
                    {
                        xfaform = true;
                        dest.Form.IsXFA = true;
                    }
                }

                dest.Append(src);

                src.Close(true);
                src = null;
            }

            if (dest is PdfDocument)
            {
                if ((dest as PdfDocument).Form != null)
                {
                    if (xfaform == true)
                    {
                        (dest as PdfDocument).Form.NeedAppearances = true;
                    }
                }
            }

            return dest;
        }

#endif
        /// <summary>
        /// Merges the specified dest.
        /// </summary>
        /// <param name="dest">The destination document.</param>
        /// <param name="src">The source document.</param>
        /// <returns>The merged document</returns>
        /// <example>
        /// <code lang="C#">
        /// // Source document.
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("Src1.pdf");
        /// // Destination PDF document
        /// PdfDocument destDoc = new PdfDocument();          
        /// //Merge the source pdf document.
        /// PdfDocumentBase.Merge(destDoc, srcDoc);
        /// destDoc.Save("Merge.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Source document.
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("Src1.pdf")
        /// ' Destination PDF document
        /// Dim destDoc As PdfDocument = New PdfDocument()
        /// 'Merge the source pdf document.
        /// PdfDocumentBase.Merge(destDoc, srcDoc)
        /// destDoc.Save("Merge.pdf")
        /// </code>
        /// </example>
        public static PdfDocumentBase Merge(PdfDocumentBase dest, PdfLoadedDocument src)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                dest = new PdfDocument(true);
            }
            else
                dest.CrossTable.IsMerging = true;
            dest.Append(src);

            if (dest is PdfDocument)
            {
                if ((dest as PdfDocument).Form != null)
                {
                    if (src.IsXFAForm == true)
                    {
                        (dest as PdfDocument).Form.NeedAppearances = true;
                        (dest as PdfDocument).Form.IsXFA = true;
                    }
                    else if (src.Form != null)
                    {
                        if (src.Form.IsXFAForm == true)
                        {
                            (dest as PdfDocument).Form.NeedAppearances = true;
                            (dest as PdfDocument).Form.IsXFA = true;
                        }
                    }
                }
            }

            return dest;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an object to a collection of the objects that will be disposed during document closing.
        /// </summary>
        /// <param name="obj">The object that will be disposed during document closing.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();               
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Create Pdf graphics for the page
        /// PdfGraphics g = page.Graphics;
        /// // Loads an Image
        /// Image img = Image.FromFile("Logo.png");
        /// PdfImage pdfImg = new PdfBitmap(img);
        /// //Draw the image
        /// g.DrawImage(pdfImg,20, 20, 100,200);            
        /// doc.Save("DisposeOnClose.pdf");        
        /// // Dispose the Img object along with the document.
        /// doc.DisposeOnClose(img);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Create Pdf graphics for the page
        /// Dim g As PdfGraphics = page.Graphics
        /// ' Loads an Image
        /// Dim img As Image = Image.FromFile("Logo.png")
        /// Dim pdfImg As PdfImage = New PdfBitmap(img)
        /// 'Draw the image
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// doc.Save("DisposeOnClose.pdf")
        /// ' Dispose the Img object along with the document.
        /// doc.DisposeOnClose(img)
        /// </code>
        /// </example>
        public void DisposeOnClose(IDisposable obj)
        {
            if (obj != null)
            {
                DisposeObjects.Add(obj);
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Saves the document to the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
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
        /// g.DrawImage(pdfImg,20, 20, 100,200);            
        /// doc.Save("SaveExample.pdf"); 
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
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// doc.Save("SaveExample.pdf")
        /// </code>
        /// </example>
#if NETFX_CORE || WP
        /// <summary>
        /// Saves the Pdf document to a storage file
        /// </summary>
        /// <param name="stFile">Storage file to save the PDF document.</param>
        /// <returns></returns>
        public async Task<bool> SaveAsync(StorageFile stFile)
        {
            // Check if the storage file is valid.
            if (stFile == null)
            {
                throw new ArgumentNullException("InvalidFile");
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    Stream fileStream = stFile.OpenStreamForWriteAsync().Result;
                    fileStream.SetLength(0);
                    MemoryStream tempStream = new MemoryStream();
                    Save(tempStream);
                    fileStream.Write(tempStream.ToArray(), 0, (int)tempStream.Length);
                    fileStream.Flush();
                    fileStream.Dispose();
                    tempStream.Dispose();
                    tcs.SetResult(true);
                }

                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }
       
#else 
 public void Save(string filename)
        {
            // Check if there is the specified path.
            if (filename == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (filename.Length == 0)
            {
                throw new ArgumentException("fileName - string can not be empty");
            }

            string full = System.IO.Path.GetFullPath(filename);
            string dir = System.IO.Path.GetDirectoryName(full);

            // Create directory for output file if it does not exist yet.
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(full))
            {
                FileAttributes attrib = File.GetAttributes(full);

                if ((attrib & FileAttributes.ReadOnly) != 0)
                {
                    throw new ArgumentException("File attributes set to Read-only state. File Name: " + full);
                }
            }

            // Create file stream.
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                // Save the document to the filestream.
                Save(fs);
            }
        }
#endif
#if !ClientProfile && !NETFX_CORE && !WP
        /// <summary>
        /// Saves the document into a HTTP response stream.
        /// </summary>
        /// <param name="fileName">The name of the document.</param>
        /// <param name="response">The HTTP response stream object.</param>
        /// <param name="type">The type of the reading document.</param>
        /// <remarks>
        /// If a document containing digital signature needs to be saved then
        /// the destination stream must support seeking, otherwise an exception will be raised.
        /// Since the HTTP response stream does not support seeking please write the document to a memory stream first and then flush it to the destination stream to avoid raising an exception.
        /// </remarks>
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
        /// g.DrawImage(pdfImg,20, 20, 100,200);            
        /// doc.Save("SaveExample.pdf", Response, HttpReadType.Open);    
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Create Pdf graphics for the page
        /// Dim g As PdfGraphics = page.Graphics
        /// ' Loads an Image            
        /// Dim pdfImg As PdfImage = New PdfBitmap(Image.FromFile("Logo.png"))  
        /// 'Draw the image
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// doc.Save("SaveExample.pdf", Response, HttpReadType.Open)
        /// </code>
        /// </example>
        public void Save(string fileName, System.Web.HttpResponse response, HttpReadType type)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.ClearContent();
            response.Expires = 0;
            response.Buffer = true;
            string disposition = "content-disposition";

            if (type == HttpReadType.Open)
            {
                response.AddHeader(disposition, "inline; filename=" + fileName);
            }
            else if (type == HttpReadType.Save)
            {
                response.AddHeader(disposition, "attachment; filename=" + fileName);
            }

            response.AddHeader("Content-Type", "application/pdf");

            response.Clear();

            Save(response.OutputStream);

            if (PdfDocumentBase.IsSecurityGranted)
            {
                response.Flush();
                response.End();
            }
            else
            {
                response.End();
            }
        }


        /// <summary>
        /// Saves the document into a HTTP response stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="response">The HTTP response stream object.</param>
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
        /// g.DrawImage(pdfImg,20, 20, 100,200);
        /// MemoryStream stream = new MemoryStream();
        /// // Save the document as a stream
        /// doc.Save(stream, Response);  
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
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// Dim stream As MemoryStream = New MemoryStream()
        /// ' Save the document as a stream
        /// doc.Save(stream, Response)
        /// </code>
        /// </example>
        public void Save(Stream stream, System.Web.HttpContext response)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            Save(stream);
            stream.Position = 0;

            //clear the reponse and send the report!
            response.Response.Clear();
            response.Response.ClearContent();
            response.Response.ClearHeaders();
            response.Response.ContentType = "application/pdf";

            for (int i = 0; i < stream.Length; i++)
            {
                int b = stream.ReadByte();
                if (b > -1) response.Response.OutputStream.WriteByte((byte)b);
            }

            response.Response.Flush();
            response.Response.Close();
            stream.Close();
        }
#endif
#endif

        /// <summary>
        /// Closes the document. Releases all common resources.
        /// </summary>
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
        /// g.DrawImage(pdfImg,20, 20, 100,200);
        /// //Save the document
        /// doc.Save("Close.pdf");
        /// // Closes the document
        /// doc.Close();
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
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// 'Save the document
        /// doc.Save("Close.pdf")
        /// ' Closes the document
        /// doc.Close()
        /// </code>
        /// </example>
        public void Close()
        {
            Close(false);
        }

        /// <summary> 
        /// Closes the document.
        /// </summary>
        /// <param name="completely">if set to <c>true</c> the document should close its stream as well.</param>
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
        /// g.DrawImage(pdfImg,20, 20, 100,200);
        /// //Save the document
        /// doc.Save("Close.pdf");
        /// // Closes the document completely.
        /// doc.Close(true);
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
        /// g.DrawImage(pdfImg,20, 20, 100,200)
        /// 'Save the document
        /// doc.Save("Close.pdf")
        /// ' Closes the document completely.
        /// doc.Close(True)
        /// </code>
        /// </example>
        public virtual void Close(bool completely)
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_security = null;
#endif
            m_objects = null;
            m_currentSavingObj = null;

            if (m_catalog != null && completely && EnableMemoryOptimization)
            {
                m_catalog.Clear();
                m_catalog = null;
            }

            if (EnableMemoryOptimization)
            { 
                if(m_crossTable!=null)
                m_crossTable.Close(true);
            }
            else if (completely && m_crossTable != null)
                m_crossTable.Dispose();

            m_crossTable = null;

            m_documentInfo = null;
            m_compression = PdfCompressionLevel.Normal;

            // Dispose all pending objects.
            if (m_disposeObjects != null)
            {
                for (int i = 0, len = m_disposeObjects.Count; i < len; i++)
                {
                    IDisposable obj = m_disposeObjects[i];
                    if (obj != null)
                    {
                        obj.Dispose();
                    }
                }

                m_disposeObjects.Clear();
                m_disposeObjects = null;
            }

            //GC.Collect();

            PdfDocument.Cache.Clear();

            //GC.Collect();
        }

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
        public abstract void Save(Stream stream);

        /// <summary>
        /// Imports a page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <returns>The page in the target document.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Source document
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("SrcDocument.pdf");
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// // Importing pages from source document.
        /// doc.ImportPage(srcDoc, srcDoc.Pages[0]);
        /// doc.Save("ImportPages.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Source document
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("SrcDocument.pdf")
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Importing pages from source document.
        /// doc.ImportPage(srcDoc, srcDoc.Pages(0))
        /// doc.Save("ImportPages.pdf")
        /// </code>
        /// </example>
        public PdfPageBase ImportPage(PdfLoadedDocument ldDoc, PdfPageBase page)
        {
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }

            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            int index = ldDoc.Pages.IndexOf(page);

            return ImportPage(ldDoc, index);
        }

        /// <summary>
        /// Imports a page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>The page in the target document.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Source document
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("SrcDocument.pdf");
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// // Importing pages from source document.
        /// doc.ImportPage(srcDoc, 0);
        /// doc.Save("ImportPages.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Source document
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("SrcDocument.pdf")
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Importing pages from source document.
        /// doc.ImportPage(srcDoc, 0)
        /// doc.Save("ImportPages.pdf")
        /// </code>
        /// </example>
        public PdfPageBase ImportPage(PdfLoadedDocument ldDoc, int pageIndex)
        {
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }

            if (pageIndex < 0 || pageIndex >= ldDoc.Pages.Count)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            return ImportPageRange(ldDoc, pageIndex, pageIndex);
        }

        /// <summary>
        /// Imports a page range from a loaded document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="startIndex">The start page index.</param>
        /// <param name="endIndex">The end page index.</param>
        /// <returns>The last created page in the target document.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Source document
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("SrcDocument.pdf");
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// // Importing pages from source document.
        /// doc.ImportPageRange(srcDoc, 0, 2);
        /// doc.Save("ImportPages.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Source document
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("SrcDocument.pdf")
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Importing pages from source document.
        /// doc.ImportPageRange(srcDoc, 0, 2)
        /// doc.Save("ImportPages.pdf")
        /// </code>
        /// </example>
        public PdfPageBase ImportPageRange(PdfLoadedDocument ldDoc, int startIndex, int endIndex)
        {
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }

            if (startIndex > endIndex)
            {
                throw new ArgumentException("The start index is greater then the end index, which might indicate the error in the program.");
            }

            PdfPageBase lastPage = null;
            PdfLoadedPageCollection pages = ldDoc.Pages;

            if (this is PdfLoadedDocument)   
            {
                PdfLoadedDocument doc = this as PdfLoadedDocument;
                foreach (PdfPageBase page in pages)
                {
                    if (doc.Pages.IndexOf(page) >= 0)
                    {
                        return null;
                    }
                }
            }

            if (ldDoc.CrossTable.DocumentCatalog.ContainsKey(DictionaryProperties.Pages))
            {
                PdfReferenceHolder pagesRef = ldDoc.CrossTable.DocumentCatalog[DictionaryProperties.Pages] as PdfReferenceHolder;
                PdfDictionary pagesKids = (pagesRef.Object) as PdfDictionary;
                PdfReferenceHolder kids = pagesKids[DictionaryProperties.Kids] as PdfReferenceHolder;
                PdfArray kidsArray;
                if (kids != null)
                {
                    kidsArray = (kids.Object) as PdfArray;
                }
                else
                {
                    kidsArray = pagesKids[DictionaryProperties.Kids] as PdfArray;
                }
                if (kidsArray.Count != pages.Count)
                {
                    foreach (PdfLoadedPage page in pages)
                    {
                        if (page.Contents.Count == 0)
                        {
                            PdfDictionary dic = (page as IPdfWrapper).Element as PdfDictionary;
                            if (dic.ContainsKey(DictionaryProperties.Contents))
                            {
                                PdfArray content = dic[DictionaryProperties.Contents] as PdfArray;
                                if (content == null)
                                {
                                    PdfArray temp = ((dic[DictionaryProperties.Parent] as PdfReferenceHolder).Object as PdfDictionary)[DictionaryProperties.Kids] as PdfArray;
                                    temp.Remove(new PdfReferenceHolder(dic));
                                }
                            }

                        }
                    }
                }
            }

            if (endIndex >= pages.Count || startIndex >= pages.Count)
            {
                throw new ArgumentException("Either or both indices are out of range", "endIndex, startIndex");
            }

            List<PdfField> fields = new List<PdfField>();
            List<PdfBookmarkBase> bookmarks = new List<PdfBookmarkBase>();
            List<PdfArray> destinations = new List<PdfArray>();
            Dictionary<IPdfPrimitive, Object> pageCorrespondance = ldDoc.CrossTable.PageCorrespondance;// = new Dictionary<IPdfPrimitive, PdfPageBase>(endIndex - startIndex);
            Dictionary<PdfPageBase, object> pageToBookmarkDic = ldDoc.CreateBookmarkDestinationDictionary();
            bool bookmarkPresent = (pageToBookmarkDic != null && pageToBookmarkDic.Count > 0);
            int correspondancePageCount = 0;

            for (int i = startIndex; i <= endIndex; ++i)
            {
                PdfPageBase page = pages[i];

                PdfPageBase newPage = ClonePage(ldDoc, page, destinations);
                newPage.Imported = true;
                pageCorrespondance[(page as IPdfWrapper).Element] = newPage;
                correspondancePageCount++;

                if (bookmarkPresent)
                {
                    List<object> pageBookmarDic = pageToBookmarkDic.ContainsKey(page) ?
                        pageToBookmarkDic[page] as List<object> : null;
                    if (pageBookmarDic != null)
                        MarkBookmarks(pageBookmarDic, bookmarks);
                }

                if (page.Dictionary.ContainsKey(DictionaryProperties.Resources))
                {
                    lastPage = newPage;
                }
                else
                if (page.Dictionary.ContainsKey(DictionaryProperties.Parent))
                {
                    PdfDictionary pageSection = ((page.Dictionary[DictionaryProperties.Parent] as PdfReferenceHolder)
                                                    .Object as PdfDictionary);

                    if (pageSection.ContainsKey(DictionaryProperties.Resources))
                    {
                        PdfResources sectionResource = null;
                        if (pageSection[DictionaryProperties.Resources] is PdfReferenceHolder)
                        {
                            if ((pageSection[DictionaryProperties.Resources] as PdfReferenceHolder).Object is PdfDictionary)
                            {
                                sectionResource = new PdfResources(
                                                                (pageSection[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary);
                            }
                        }
                        else
                        {
                            sectionResource = new PdfResources(pageSection[DictionaryProperties.Resources] as PdfDictionary);
                        }
                            if (sectionResource != null)
                            {
                                if ((newPage as PdfPage).Dictionary.ContainsKey(DictionaryProperties.Resources))
                                {
                                    (newPage as PdfPage).Dictionary.Remove(DictionaryProperties.Resources);
                                    PdfDictionary dict = null;
                                    if (pageSection[DictionaryProperties.Resources] is PdfReferenceHolder)
                                        dict = (pageSection[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary;
                                    else
                                        dict = pageSection[DictionaryProperties.Resources] as PdfDictionary;
                                    if (dict != null)
                                    {
                                        PdfDictionary resDict = EnableMemoryOptimization ? dict.Clone(CrossTable) as PdfDictionary : dict;
                                        (newPage as PdfPage).Dictionary[DictionaryProperties.Resources] = new PdfReferenceHolder(resDict);
                                    }
                                    newPage.Contents.Clear();
                                    foreach (IPdfPrimitive obj in page.Contents)
                                    {
                                        if (EnableMemoryOptimization)
                                            newPage.Contents.Add(obj.Clone(m_crossTable));
                                        else
                                            newPage.Contents.Add(obj);
                                    }
                                    (newPage as PdfPage).Dictionary.Modify();
                                }
                            }
                    }
                }
            }

            for (int i = startIndex; i <= endIndex; ++i)
            {
                fields = new List<PdfField>();
                PdfPageBase page = ldDoc.Pages[i];

                PdfPageBase newPage = (pageCorrespondance[(page as IPdfWrapper).Element] as PdfPageBase);

                CheckFields(ldDoc, page, fields);

                if (fields.Count > 0)
                {
                    AddFields(ldDoc, newPage, fields);

                    fields.Clear();

                    PdfForm form = GetForm();

                    if (form != null && !(form.m_pageMap.ContainsKey(page.Dictionary)))
                        form.m_pageMap.Add(page.Dictionary, newPage);
                }

                if (EnableMemoryOptimization)
                    (newPage as PdfPage).ImportAnnotations(ldDoc, page, destinations);
            }

            FixDestinations(pageCorrespondance, destinations);

            if (bookmarkPresent)
            {
                ExportBookmarks(ldDoc, bookmarks, correspondancePageCount, pageToBookmarkDic);
                this.Bookmarks.CrossTable.Document = this;
            }

            bookmarks.Clear();
            destinations.Clear();
            pageCorrespondance = null;
            CrossTable.PrevReference = null;

            return lastPage;
        }

        /// <summary>
        /// Appends the specified loaded document to this one.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <example>
        /// <code lang="C#">
        /// // Source document
        /// PdfLoadedDocument srcDoc = new PdfLoadedDocument("SrcDocument.pdf");
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();          
        /// // Appending the document with source document.
        /// doc.Append(srcDoc);
        /// // Save the document.
        /// doc.Save("Append.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Source document
        /// Dim srcDoc As PdfLoadedDocument = New PdfLoadedDocument("SrcDocument.pdf")
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Appending the document with source document.
        /// doc.Append(srcDoc)
        /// ' Save the document.
        /// doc.Save("Append.pdf")
        /// </code>
        /// </example>
        public void Append(PdfLoadedDocument ldDoc)
        {
            if (this is PdfDocument)
                CrossTable.IsMerging = true;
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }
            if (ldDoc.IsXFAForm == true && this is PdfDocument)
                ((PdfDocument)this).Form.IsXFA = true;
            bool m_encrypt = false;
            //changed revision 97323
            bool xfaform = false;
            if (ldDoc.IsXFAForm == true)
            {
                xfaform = true;
            }
            else if (ldDoc.Form != null)
            {
                if (ldDoc.Form.IsXFAForm == true)
                {
                    xfaform = true;
                }
            }
            int startIndex = 0;
            int endIndex = ldDoc.Pages.Count - 1;
            if (!this.EnableMemoryOptimization)
            {
                this.EnableMemoryOptimization = true;
            }

            ImportPageRange(ldDoc, startIndex, endIndex);
            MergeAttachments(ldDoc);
        }

#if NETFX_CORE || WP
        /// <summary>
        /// Imports the page in asynchronous mode
        /// </summary>
        /// <param name="ldDoc">Source document</param>
        /// <param name="page">Source page</param>
        /// <returns>Imported page</returns>
        public async Task<PdfPageBase> ImportPageAsync(PdfLoadedDocument ldDoc, PdfPageBase page)
        {
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }

            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            TaskCompletionSource<PdfPageBase> tcs = new TaskCompletionSource<PdfPageBase>();

            await Task.Run(() =>
                {
                    try
                    {
                        int index = ldDoc.Pages.IndexOf(page);
                        PdfPageBase resPage = ImportPage(ldDoc, index);
                        tcs.SetResult(resPage);
                    }
                    catch(Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                     
                });

            return await tcs.Task; 
        }
        
        /// <summary>
        /// Import page in asynchronous mode
        /// </summary>
        /// <param name="ldDoc">Source document</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Imported page</returns>
        public async Task<PdfPageBase> ImportPageAsync(PdfLoadedDocument ldDoc, int pageIndex)
        {
            if (ldDoc == null)
            {
                throw new ArgumentNullException("ldDoc");
            }

            if (pageIndex < 0 || pageIndex >= ldDoc.Pages.Count)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            TaskCompletionSource<PdfPageBase> tcs = new TaskCompletionSource<PdfPageBase>();

            await Task.Run(() =>
            {
                try
                {
                    PdfPageBase resPage = ImportPageRange(ldDoc, pageIndex, pageIndex);
                    tcs.SetResult(resPage);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            return await tcs.Task; 

        }

        /// <summary>
        /// Import pages in asynchronous mode
        /// </summary>
        /// <param name="ldDoc">Source document</param>
        /// <param name="startIndex">First page index</param>
        /// <param name="endIndex">Last page index</param>
        /// <returns>Imported page</returns>
        public async Task<PdfPageBase> ImportPageRangeAsync(PdfLoadedDocument ldDoc, int startIndex, int endIndex)
        {
            TaskCompletionSource<PdfPageBase> tcs = new TaskCompletionSource<PdfPageBase>();

            await Task.Run(() =>
            {
                try
                {
                    PdfPageBase resPage = ImportPageRange(ldDoc, startIndex, endIndex);
                    tcs.SetResult(resPage);
                }

                catch(Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Append document in asynchronous mode
        /// </summary>
        /// <param name="ldDoc">Source document</param>
        /// <returns>Task returning the status</returns>
        public async Task<bool> AppendAsync(PdfLoadedDocument ldDoc)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            await Task.Run(() =>
            {
                try
                {
                    Append(ldDoc);
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
        /// Merge documents in asynchronous mode
        /// </summary>
        /// <param name="dest">Destination document</param>
        /// <param name="src">Source document</param>
        /// <returns></returns>
        public async static Task<PdfDocumentBase> MergeAsync(PdfDocumentBase dest, PdfLoadedDocument src)
        {
            TaskCompletionSource<PdfDocumentBase> tcs = new TaskCompletionSource<PdfDocumentBase>();

            await Task.Run(() =>
                {
                    try
                    {
                        tcs.SetResult(Merge(dest,src));
                    }
                    catch(Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });

            return await tcs.Task;
        }
        

#endif

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Checks whether the PDF document was encrypted.
        /// </summary>
        /// <returns>True if the document was encrypted.</returns>
        private bool CheckEncryption(PdfLoadedDocument ldoc)
        {
            bool wasEncrypted = false;

            // Read Security if present.
            PdfDictionary trailer = ldoc.CrossTable.Trailer;

            IPdfPrimitive obj;
            PdfDictionary encDic = ldoc.CrossTable.EncryptorDictionary;
            m_password = ldoc.Password;
            bool isEncrpt = true;
            if (encDic != null && encDic.ContainsKey(DictionaryProperties.EncryptMetadata))
                isEncrpt = (encDic[DictionaryProperties.EncryptMetadata] as PdfBoolean).Value;

            if (encDic != null && isEncrpt) // Encryption dictionary have been found.
            {
                if (m_password == null)
                {
                    m_password = string.Empty;
                }

                obj = trailer[DictionaryProperties.ID];

                if (obj == null)
                {
                    throw new PdfDocumentException("Unable to decrypt document without ID.");
                }

                PdfArray id = obj as PdfArray;

                PdfString key = id[0] as PdfString;

                PdfEncryptor encryptor = new PdfEncryptor();

                encryptor.ReadFromDictionary(encDic);

                if (!encryptor.CheckPassword(m_password, key))
                {
                    this.Close(true);
                    throw new PdfDocumentException("Can't open an encrypted document. The password is invalid.");
                }

                encDic.Encrypt = false;

                PdfSecurity security = new PdfSecurity();
                if (this.Security.Encryptor.Encrypt == false)
                {
                    security.Encryptor = encryptor;
                    SetSecurity(security);
                    wasEncrypted = true;

                    this.Security.Encryptor = encryptor;
                }
            }

            return wasEncrypted;
        }
#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <returns>The proper PdfForm instance.</returns>
        internal abstract PdfForm GetForm();

        /// <summary>
        /// Sets the main object collection.
        /// </summary>
        /// <param name="moc">The main object collection.</param>
        /// <remarks>Allows to use null values as the parameter,
        /// which causes assigning null value to the variable. That's used for clearing.</remarks>
        internal void SetMainObjectCollection(PdfMainObjectCollection moc)
        {
            if (moc == null)
            {
                throw new ArgumentNullException("moc");
            }

            m_objects = moc;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Sets the security object.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <remarks>Allows to set null.</remarks>
        internal void SetSecurity(PdfSecurity security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            m_security = security;
        }
#endif
        /// <summary>
        /// Sets the cross table.
        /// </summary>
        /// <param name="cTable">The cross table.</param>
        internal void SetCrossTable(PdfCrossTable cTable)
        {
            if (cTable == null)
            {
                throw new ArgumentNullException("cTable");
            }

            m_crossTable = cTable;
        }

        /// <summary>
        /// Sets the catalog.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        internal void SetCatalog(PdfCatalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

            m_catalog = catalog;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises DocumentSaved event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        internal void OnDocumentSaved(DocumentSavedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (DocumentSaved != null)
            {
                DocumentSaved(this, args);
            }
        }

        /// <summary>
        /// Adds the fields connected to the page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="newPage">The new page.</param>
        /// <param name="fields">The lost of the fields.</param>
        internal abstract void AddFields(PdfLoadedDocument ldDoc, PdfPageBase newPage, List<PdfField> fields);

        /// <summary>
        /// Clones pages and their resource dictionaries and adds them into the document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page being cloned.</param>
        /// <param name="destinations">The destinations.</param>
        /// <returns>page</returns>
        internal abstract PdfPageBase ClonePage(PdfLoadedDocument ldDoc, PdfPageBase page,
            List<PdfArray> destinations);

        /// <summary>
        /// Checks what fields are connected with the page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <param name="fields">An array where the fields connected to the page are stored.</param>
        protected virtual void CheckFields(PdfLoadedDocument ldDoc, PdfPageBase page, List<PdfField> fields)
        {
            PdfArray annots = page.GetAnnots();
            PdfLoadedForm form = ldDoc.Form;

            PdfName kids = new PdfName(DictionaryProperties.Kids);
            PdfCollection collection = null;

            if (annots != null && form != null)
            {
                for (int i = 0, count = form.Fields.Count; i < count; ++i)
                {
                    PdfField field = form.Fields[i];

                    if (EnableMemoryOptimization)
                    {
                        bool signatureField = false;
#if !SILVERLIGHT && !NETFX_CORE && !WP
                        if (field is PdfLoadedSignatureField)
                            signatureField = true;
# endif

                        if (field.Dictionary.ContainsKey(kids) && (field.Dictionary[kids] as PdfArray).Count > 0 && !signatureField)
                        {
                            if (field is PdfLoadedButtonField && (field as PdfLoadedButtonField).Items.Count > 0)
                                collection = (field as PdfLoadedButtonField).Items;
                            else if (field is PdfLoadedCheckBoxField && (field as PdfLoadedCheckBoxField).Items.Count > 0)
                                collection = (field as PdfLoadedCheckBoxField).Items;
                            else if (field is PdfLoadedComboBoxField && (field as PdfLoadedComboBoxField).Items.Count > 0)
                                collection = (field as PdfLoadedComboBoxField).Items;
                            else if (field is PdfLoadedListBoxField && (field as PdfLoadedListBoxField).Items.Count > 0)
                                collection = (field as PdfLoadedListBoxField).Items;
                            else if (field is PdfLoadedRadioButtonListField && (field as PdfLoadedRadioButtonListField).Items.Count > 0)
                                collection = (field as PdfLoadedRadioButtonListField).Items;
                            else if (field is PdfLoadedTextBoxField && (field as PdfLoadedTextBoxField).Items.Count > 0)
                                collection = (field as PdfLoadedTextBoxField).Items;

                            foreach (PdfLoadedFieldItem item in collection)
                            {
                                if (item.Page == page)
                                {
                                    fields.Add(field);
                                    break;
                                }
                            }
                        }
                        else if (field.Page == page)
                            fields.Add(field);
                    }
                    else if (field.Page == page)
                        fields.Add(field);
                }
            }
        }

        /// <summary>
        /// Merges the attachments.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <remarks>This method merges only named attachments.</remarks>
        private void MergeAttachments(PdfLoadedDocument ldDoc)
        {
            PdfCatalogNames names = ldDoc.Catalog.Names;

            if (names != null)
            {
                Catalog.CreateNamesIfNone();
                if (EnableMemoryOptimization)
                    Catalog.Names.MergeEmbedded(names, m_crossTable);
                else
                    Catalog.Names.MergeEmbedded(names, null);
            }
        }

        /// <summary>
        /// Exports the bookmarks to the new document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="bookmarks">The bookmarks.</param>
        /// <param name="pageCorrespondance">The page correspondance dictionary.</param>
        private void ExportBookmarks(PdfLoadedDocument ldDoc, List<PdfBookmarkBase> bookmarks, int pageCount, Dictionary<PdfPageBase, object> bookmarkshash)
        {
            PdfBookmarkBase currentBase = Bookmarks;
            PdfBookmarkBase current = ldDoc.Bookmarks;
            List<String> bkCollection = null;

            if (current != null)// && bookmarks.Count > 0)
            {
                if (currentBase == null)
                {
                    currentBase = (this as PdfLoadedDocument).CreateBookmarkRoot();
                }

                Stack<NodeInfo> stack = new Stack<NodeInfo>();
                NodeInfo ni = new NodeInfo(currentBase, current.List);

                if (ldDoc.Pages.Count != pageCount)
                {
                    ni = new NodeInfo(currentBase, bookmarks);
                    bkCollection = new List<String>();
                }

                do
                {
                    for (; ni.Index < ni.Kids.Count; )
                    {
                        current = ni.Kids[ni.Index];

                        // Check node.
                        if (bookmarks.Contains(current) && bkCollection != null && !bkCollection.Contains((current as PdfBookmark).Title))
                        {
                            // Do exporting.
                            PdfBookmark bm = current as PdfBookmark;
                            PdfBookmark newBm = currentBase.Add(bm.Title);

                            newBm.TextStyle = bm.TextStyle;
                            newBm.Color = bm.Color;

                            PdfDestination dest = bm.Destination;
                            PdfDestination newDest = null;
                            PdfPageBase newPage = null;
                            PdfPageBase page = null;

                            // Get destination reference.
                            if (dest != null && EnableMemoryOptimization)
                            {
                                page = dest.Page;
                                if (ldDoc.CrossTable.PageCorrespondance.ContainsKey(page.Dictionary) && ldDoc.CrossTable.PageCorrespondance[page.Dictionary] != null)
                                {
                                    newPage = ldDoc.CrossTable.PageCorrespondance[page.Dictionary] as PdfPageBase;
                                    if (newPage != null)
                                    {
                                        newDest = new PdfDestination(newPage, dest.Location);
                                        newBm.Destination = newDest;
                                    }
                                }
                                else
                                    newBm.Dictionary.Remove(DictionaryProperties.A);
                            }
                            else
                            {
                                page = dest.Page;
                                newPage = ldDoc.CrossTable.PageCorrespondance[(page as IPdfWrapper).Element] as PdfPageBase;
                                newDest = new PdfDestination(newPage, dest.Location);

                                newBm.Destination = newDest;
                            }
                            currentBase = newBm;
                            bkCollection.Add(newBm.Title);
                        }
                        else
                        {
                            // Do exporting.
                            PdfBookmark bm = current as PdfBookmark;
                            // Get destination reference.
                            PdfDestination dest = bm.Destination;
                            PdfDestination newDest = null;
                            PdfPageBase newPage = null;
                            PdfPageBase page = null;

                            if (ldDoc.Pages.Count == pageCount)
                            {
                                PdfBookmark newBm = currentBase.Add(bm.Title);
                                if (!EnableMemoryOptimization && bm.Dictionary.ContainsKey(DictionaryProperties.A))
                                    newBm.Dictionary.SetProperty(DictionaryProperties.A, bm.Dictionary[DictionaryProperties.A]);
                                newBm.TextStyle = bm.TextStyle;
                                newBm.Color = bm.Color;
                                if (dest != null)
                                {
                                    page = dest.Page;
                                    if (ldDoc.CrossTable.PageCorrespondance.ContainsKey(page.Dictionary) && ldDoc.CrossTable.PageCorrespondance[page.Dictionary] != null)
                                    {
                                        newPage = ldDoc.CrossTable.PageCorrespondance[page.Dictionary] as PdfPageBase;
                                        if (newPage != null)
                                        {
                                            newDest = new PdfDestination(newPage, dest.Location);
                                            newBm.Destination = newDest;
                                        }
                                    }
                                    else
                                        newBm.Dictionary.Remove(DictionaryProperties.A);
                                }
                                currentBase = newBm;
                            }
                            else if (dest != null && dest.Page!= null)
                            {
                                if (ldDoc.Pages.IndexOf(dest.Page) < pageCount && ldDoc.CrossTable.PageCorrespondance.ContainsKey(dest.Page.Dictionary) && ldDoc.CrossTable.PageCorrespondance[dest.Page.Dictionary] != null)
                                {
                                    page = dest.Page;
                                    newPage = ldDoc.CrossTable.PageCorrespondance[dest.Page.Dictionary] as PdfPageBase;
                                    PdfBookmark newBm = currentBase.Add(bm.Title);
                                    if (bm.Dictionary.ContainsKey(DictionaryProperties.A))
                                    {
                                        if (EnableMemoryOptimization)
                                        {
                                            IPdfPrimitive obj = bm.Dictionary[DictionaryProperties.A].Clone(m_crossTable);
                                            newBm.Dictionary.SetProperty(DictionaryProperties.A, obj);
                                        }
                                        else
                                            newBm.Dictionary.SetProperty(DictionaryProperties.A, bm.Dictionary[DictionaryProperties.A]);
                                    }
                                    if (newPage != null)
                                    {
                                        newBm.TextStyle = bm.TextStyle;
                                        newBm.Color = bm.Color;
                                        newDest = new PdfDestination(newPage, dest.Location);
                                        newBm.Destination = newDest;
                                        currentBase = newBm;
                                    }
                                }
                            }
                        }

                        ++ni.Index;

                        // Go deeper.
                        if (current.Count > 0)
                        {
                            stack.Push(ni);
                            ni = new NodeInfo(currentBase, current.List);
                        }
                        else
                        {
                            currentBase = ni.Base;
                        }
                    }

                    if (stack.Count > 0)
                    {
                        ni = stack.Pop();
                        while ((ni.Index == ni.Kids.Count) && (stack.Count > 0))
                        {
                            ni = stack.Pop();
                        }
                        currentBase = ni.Base;
                    }
                } while (ni.Index < ni.Kids.Count);

                if (bkCollection != null)
                    bkCollection.Clear();
            }
        }

        /// <summary>
        /// Marks the bookmarks pointing to the page for exporting.
        /// </summary>
        /// <param name="pageBookmarks">The page bookmarks.</param>
        /// <param name="bookmarks">The bookmarks.</param>
        private void MarkBookmarks(List<object> pageBookmarks, List<PdfBookmarkBase> bookmarks)
        {
            if (pageBookmarks != null)
            {
                foreach (object var in pageBookmarks)
                {
                    if ((var as PdfBookmarkBase) == null)
                        throw new Exception("Type not specified properly");

                    bookmarks.Add((var as PdfBookmarkBase));
                }
            }
        }

        /// <summary>
        /// Marks the bookmarks pointing to the page for exporting.
        /// </summary>
        /// <param name="bookmarkBase">The page bookmarks.</param>
        /// <param name="bookmarks">The bookmarks.</param>

        private void MarkBookmarks(PdfBookmarkBase bookmarkBase, List<PdfBookmarkBase> bookmarks)
        {

            bookmarks.Add(bookmarkBase);

        }

        /// <summary>
        /// Fixes the destinations.
        /// </summary>
        /// <param name="pageCorrespondance">The page correspondance.</param>
        /// <param name="destinations">The destinations.</param>
        private void FixDestinations(Dictionary<IPdfPrimitive,object> pageCorrespondance, List<PdfArray> destinations)
        {
            PdfNull nullObj = new PdfNull();

            for (int i = 0, count = destinations.Count; i < count; ++i)
            {
                PdfArray dest = destinations[i] as PdfArray;

                if (dest != null)
                {
                    PdfReferenceHolder rh = dest[0] as PdfReferenceHolder;

                    if (rh != null)
                    {
                        PdfDictionary dict = rh.Object as PdfDictionary;
                        if (dict != null && pageCorrespondance.ContainsKey(dict) && pageCorrespondance[dict] != null)
                        {
                            PdfPageBase page = pageCorrespondance[dict] as PdfPageBase;

                            dest.RemoveAt(0);

                            if (page != null)
                            {
                                rh = new PdfReferenceHolder(page);
                                dest.Insert(0, rh);
                            }
                            else
                            {
                                dest.Insert(0, nullObj);
                            }
                        }
                        else if (pageCorrespondance.ContainsKey(dict) && pageCorrespondance[dict] == null)
                        {
                            dest.RemoveAt(0);
                            dest.Insert(0, nullObj);
                        }
                    }
                }
            }
        }

        #endregion

        #region Internals
        /// <summary>
        /// Holds info about current node.
        /// </summary>
        private class NodeInfo
        {
            #region Members
            /// <summary>
            /// Internal variable to store index value.
            /// </summary>
            public int Index;

            /// <summary>
            /// Internal variable to store Bookmark.
            /// </summary>
            public PdfBookmarkBase Base;

            /// <summary>
            /// Internal variable to store ArrayList.
            /// </summary>
            public List<PdfBookmarkBase> Kids;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="NodeInfo"/> class.
            /// </summary>
            /// <param name="bookmarkBase">The bookmark base.</param>
            /// <param name="kids">The kids.</param>
            public NodeInfo(PdfBookmarkBase bookmarkBase, List<PdfBookmarkBase> kids)
            {
                if (bookmarkBase == null)
                {
                    throw new ArgumentNullException("bookmarkBase");
                }

                if (kids == null)
                {
                    throw new ArgumentNullException("kids");
                }

                Base = bookmarkBase;
                Kids = kids;
            }
            #endregion
        }
        #endregion
    }
}
