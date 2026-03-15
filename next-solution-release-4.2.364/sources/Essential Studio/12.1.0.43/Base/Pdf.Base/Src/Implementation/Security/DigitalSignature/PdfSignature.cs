#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Represents a digital signature used for signing a PDF document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
    /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
    /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
    /// doc.Save("SignedPdfSample.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
    /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
    /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
    /// doc.Save("SignedPdfSample.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfGraphics"/> Class
    /// <seealso cref="PdfFont"/> Class
    /// <seealso cref="PdfCertificate"/> Class
#if AllowUnsafeCode
    public class PdfSignature
#else
    internal class PdfSignature
#endif
    {
#region	Fields
        /// <summary>
        /// Holds signature dictionary.
        /// </summary>
        private PdfSignatureDictionary m_signatureDictionary;

        /// <summary>
        /// Holds pdf signature field.
        /// </summary>
        private PdfSignatureField m_field;

        /// <summary>
        /// Holds pdf Loaded signature field.
        /// </summary>
        private PdfLoadedSignatureField m_sigField;

        /// <summary>
        /// Holds pdf certificate.
        /// </summary>
        private PdfCertificate m_pdfCert;

        /// <summary>
        /// Reason of signing.
        /// </summary>
        private string m_reason;

        /// <summary>
        /// Page on which signature field is initialized.
        /// </summary>
        private PdfPageBase m_page;

        /// <summary>
        /// The CPU host name or physical location of the signing.
        /// </summary>
        private string m_location;

        /// <summary>
        /// Information provided by the signer to enable a recipient to contact
        /// the signer to verify the signature; for example, a phone number.
        /// </summary>
        private string m_contactInfo;

        /// <summary>
        /// Holds a value which indicates certefication of the document.
        /// </summary>
        private bool m_certeficated;

        /// <summary>
        /// Permissions of the certificated document.
        /// </summary>
        private PdfCertificationFlags m_docPermission = PdfCertificationFlags.ForbidChanges;

        /// <summary>
        /// Holds timestamping server
        /// </summary>
        private TimeStampServer m_tsrsrv;

        /// <summary>
        /// Document that holds page and this signature. That document should be signed.
        /// </summary>
        private PdfDocumentBase m_doc;
        /// <summary>
        /// Indicates whether the signature corresponds to signature field or not.
        /// Note : We draw appearance in the case of signature field.
        /// </summary>
        private bool m_drawSignatureAppearance;
        #endregion

#region Properties
        /// <summary>
        /// Gets the signature Appearance.
        /// </summary>
        /// <value>A <see cref="PdfAppearance"/> object defines signature`s appearance.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// PdfAppearance appearnce = signature.Appearence;
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// Dim appearnce As PdfAppearance = signature.Appearence
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class       
        public PdfAppearance Appearence
        {
            get
            {
               return m_field.Appearance;                
            }
        }

        /// <summary>
        /// Gets or sets signature location on the page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Location = new PointF(100, 200);
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Location = New PointF(100, 200)
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class          
        public PointF Location
        {
            get
            {
                return m_field.Location;
            }
            set
            {
                m_field.Location = value;
            }
        }

        /// <summary>
        /// Gets or sets bounds of signature.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Location = new PointF(100, 200);
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Location = New PointF(100, 200)
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class       
        public RectangleF Bounds
        {
            get
            {
                return m_field.Bounds;
            }
            set
            {
                m_field.Bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets information provided by the signer to enable a recipient to contact
        /// the signer to verify the signature; for example, a phone number.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.ContactInfo = "Syncfusion";
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.ContactInfo = "Syncfusion"
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class                     
        public string ContactInfo
        {
            get
            {
                return m_contactInfo;
            }
            set
            {
                m_contactInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets reason of signing.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Reason = "PDF is signed";
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Reason = "PDF is signed"
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class            
        public string Reason
        {
            get
            {
                return m_reason;
            }
            set
            {
                m_reason = value;
            }
        }

        /// <summary>
        /// Gets or sets the physical location of the signing.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.LocationInfo = "US";
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.LocationInfo = "US"
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class     
        public string LocationInfo
        {
            get
            {
                return m_location;
            }
            set
            {
                m_location = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating certificate document or not.
        /// NOTE: Works only with Adobe Reader 7.0.8 or higher.
        /// </summary>
        /// <value>certificate document if true.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Certificated = true;
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Certificated = True
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class            
        public bool Certificated
        {
            get
            {
                return m_certeficated;
            }
            set
            {
                PdfDictionary perms = PdfCrossTable.Dereference(m_doc.Catalog[DictionaryProperties.Perms]) as PdfDictionary;

                if (perms != null && perms.ContainsKey(DictionaryProperties.DocMDP))
                {
                    throw new ArgumentException("The document may contain at most one author signature!");
                }
                m_certeficated = value;
            }
        }

        /// <summary>
        /// Gets or sets the permission for certificated document.
        /// </summary>
        /// <value>The document permission.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.DocumentPermissions = PdfCertificationFlags.AllowComments;
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.DocumentPermissions = PdfCertificationFlags.AllowComments
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class 
        public PdfCertificationFlags DocumentPermissions
        {
            get
            {
                return m_docPermission;
            }
            set
            {
                m_docPermission = value;
            }
        }

        /// <summary>
        /// Gets signing certificate.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.ContactInfo = "Syncfusion";
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.ContactInfo = "Syncfusion"
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class  
        public PdfCertificate Certificate
        {
            get
            {
                return m_pdfCert;
            }
            set
            {
                m_pdfCert = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether signature visible or not.
        /// </summary>
        /// <remarks>Signature can be set as invisible when its <see cref="Bounds"/> size is set to empty.</remarks>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Visible = false;
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Visible = False
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class         
        public bool Visible
        {
            get
            {
                SizeF size = m_field.Size;

                if (size.Height == 0 && size.Width == 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets time stamping server unique resource identifier. 
        /// </summary>
        public TimeStampServer TimeStampServer
        {
            get
            {
                return m_tsrsrv;
            }
            set
            {
                m_tsrsrv = value;
            }
        }
        /// <summary>
        /// Gets pdf signature field.
        /// </summary>
        internal PdfField Field
        {
            get
            {
                if(m_field == null)
                    return m_sigField;
                return m_field;
            }
        }

        /// <summary>
        /// Gets whether to draw signature appearance or not.
        /// </summary>
        internal bool DrawFieldAppearance
        {
            get
            {
                return m_drawSignatureAppearance;
            }
        }
        #endregion

#region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignature"/> class.
        /// </summary>
        public PdfSignature()
        {
            m_drawSignatureAppearance = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignature"/> class.
        /// </summary>
        /// <param name="page">The current pdf page where signature will be replaced.</param>
        /// <param name="cert">The pdf certificate.</param>
        /// <param name="signatureName">Name of the signature.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class
        public PdfSignature(PdfPage page, PdfCertificate cert, string signatureName)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (cert == null)
                throw new ArgumentNullException("cert");

            m_page = page;
            m_pdfCert = cert;
            
            m_field = new PdfSignatureField(page, signatureName);

            PdfDocument doc = page.Document;
            m_doc = doc;

            doc.Form.Fields.Add(m_field);
            doc.Form.SignatureFlags = SignatureFlags.SignaturesExists | SignatureFlags.AppendOnly;

            doc.Catalog.BeginSave += new SavePdfPrimitiveEventHandler(Catalog_BeginSave);
            m_field.Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);

            m_signatureDictionary = new PdfSignatureDictionary(doc, this, cert);
            doc.PdfObjects.Add(((IPdfWrapper)m_signatureDictionary).Element);
            if (!doc.CrossTable.IsMerging)
                ((IPdfWrapper)m_signatureDictionary).Element.Position = -1;
            m_field.Dictionary.SetProperty(DictionaryProperties.V, new PdfReferenceHolder(m_signatureDictionary));
            m_field.Dictionary.SetProperty(DictionaryProperties.FieldFlags, new PdfNumber(0));
            m_signatureDictionary.Archive = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignature"/> class.
        /// </summary>
        /// <param name="document">The document, which has the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="signatureName">The name of the signature.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5),new SizeF(100,200));            
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Bounds = New RectangleF(New PointF(5, 5),New SizeF(100,200))
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class
        /// 


        public PdfSignature(PdfDocumentBase document, PdfPageBase page,
            PdfCertificate certificate, string signatureName)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (page == null)
                throw new ArgumentNullException("page");

            if (certificate == null)
                throw new ArgumentNullException("certificate");

            m_page = page;
            m_pdfCert = certificate;
            m_doc = document;
            m_field = new PdfSignatureField(page, signatureName);
            PdfForm form = document.GetForm();

            PdfLoadedForm lForm = form as PdfLoadedForm;

            if (lForm != null)
            {
                lForm.Fields.Add(m_field);
            }
            else
            {
                form.Fields.Add(m_field);
            }

            form.SignatureFlags = SignatureFlags.SignaturesExists | SignatureFlags.AppendOnly;

            document.Catalog.BeginSave += new SavePdfPrimitiveEventHandler(Catalog_BeginSave);
            m_field.Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);

            m_signatureDictionary = new PdfSignatureDictionary(document, this, certificate);
            document.PdfObjects.Add(((IPdfWrapper)m_signatureDictionary).Element);
            if (!document.CrossTable.IsMerging)
                ((IPdfWrapper)m_signatureDictionary).Element.Position = -1;
            m_field.Dictionary.SetProperty(DictionaryProperties.V, new PdfReferenceHolder(m_signatureDictionary));
            m_field.Dictionary.SetProperty(DictionaryProperties.FieldFlags, new PdfNumber(0));
            m_signatureDictionary.Archive = false;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignature"/> class.
        /// </summary>
        /// <param name="document">The loaded document, which has the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="signatureName">The name of the signature.</param>
        /// <param name="loadedField">The name of the loaded signature field</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfLoadedDocument doc = new PdfLoadedDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfLoadedSignatureField signatureField = loadedDocument.Form.Fields["Signature"] as PdfLoadedSignatureField;
        ///  PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature",signatureField);                  
        /// doc.Save("SignedPdfSample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signatureField as PdfLoadedSignatureField = TryCast(loadedDocument.Form.Fields["Signature"],PdfLoadedSignature)
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature",signatureField)         
        /// doc.Save("SignedPdfSample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        /// <seealso cref="PdfCertificate"/> Class
        /// 

        public PdfSignature(PdfDocumentBase document, PdfPageBase page,
            PdfCertificate certificate, string signatureName, PdfLoadedSignatureField loadedField)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (page == null)
                throw new ArgumentNullException("page");

            if (certificate == null)
                throw new ArgumentNullException("certificate");

            m_page = page;
            m_pdfCert = certificate;
            m_doc = document;
            m_sigField = loadedField;
            PdfLoadedForm form = document.GetForm() as PdfLoadedForm;

            if (form != null && m_sigField.Form == null)
                form.Fields.Add(m_sigField);

            form.SignatureFlags = SignatureFlags.SignaturesExists | SignatureFlags.AppendOnly;

            document.Catalog.BeginSave += new SavePdfPrimitiveEventHandler(Catalog_BeginSave);
            m_sigField.Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);

            m_signatureDictionary = new PdfSignatureDictionary(document, this, certificate);
            document.PdfObjects.Add(((IPdfWrapper)m_signatureDictionary).Element);
            if (!document.CrossTable.IsMerging)
                ((IPdfWrapper)m_signatureDictionary).Element.Position = -1;
            m_sigField.Dictionary.SetProperty(DictionaryProperties.V, new PdfReferenceHolder(m_signatureDictionary));
            m_sigField.Dictionary.SetProperty(DictionaryProperties.FieldFlags, new PdfNumber(0));
            m_signatureDictionary.Archive = false;
        }
        #endregion

#region Overrides
        /// <summary>
        /// Handles the BeginSave event of the catalog document.
        /// NOTE: Needed for certifying pdf document.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Catalog_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if (m_certeficated)
            {
                PdfDictionary permission = PdfCrossTable.Dereference(m_doc.Catalog[DictionaryProperties.Perms]) as PdfDictionary;

                if (permission == null)
                {
                    permission = new PdfDictionary();
                    permission[DictionaryProperties.DocMDP] = new PdfReferenceHolder(m_signatureDictionary);
                    m_doc.Catalog[DictionaryProperties.Perms] = permission;
                }
                else if (!permission.ContainsKey(DictionaryProperties.DocMDP))
                {
                    permission.SetProperty(DictionaryProperties.DocMDP, new PdfReferenceHolder(m_signatureDictionary));
                }
            }
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if(m_field!=null)
                m_field.Dictionary.Encrypt = m_doc.Security.Enabled;

            else
                m_sigField.Dictionary.Encrypt = m_doc.Security.Enabled;
        }
        #endregion
    }
}
#endif