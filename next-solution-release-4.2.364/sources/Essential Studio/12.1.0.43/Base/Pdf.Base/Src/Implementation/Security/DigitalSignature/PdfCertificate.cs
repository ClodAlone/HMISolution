#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Syncfusion.Pdf.Native;

/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Represents the Certificate object.
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
    /// <seealso cref="PdfSignature"/> Class
#if AllowUnsafeCode
    public class PdfCertificate
#else
    internal class PdfCertificate
#endif
    {
#region Constants
        /// <summary>
        /// User's crypt key set.
        /// </summary>
        const uint CRYPT_USER_KEYSET = 0x00001000;

        /// <summary>
        /// The high word of the dwFlags parameter.
        /// </summary>
        private const uint CERT_SYSTEM_STORE_CURRENT_USER = 0x00010000;

        /// <summary>
        /// The high word of the dwFlags parameter.
        /// </summary>
        private const uint CERT_SYSTEM_STORE_LOCAL_MACHINE = 0x00020000;

        /// <summary>
        /// The high word of the dwFlags parameter 
        /// </summary>
        private const uint CERT_STORE_READONLY_FLAG = 0x00008000;

        /// <summary>
        /// The high word of the dwFlags parameter 
        /// </summary>
        private const uint CERT_STORE_OPEN_EXISTING_FLAG = 0x00004000;

        /// <summary>
        /// Encoding type.
        /// </summary>
        private const uint X509_ASN_ENCODING = 0x00000001;

        /// <summary>
        /// Encoding type.
        /// </summary>
        private const uint PKCS_7_ASN_ENCODING = 0x00010000;

        /// <summary>
        /// Provider being used.
        /// </summary>
        private const string STORE_TYPE = "MY";

        /// <summary>
        /// Provider being used.
        /// </summary>
        private const string STORE_PROVIDER = "System";

        /// <summary>
        /// Encoding type.
        /// </summary>
        private const uint ENCODING_TYPE = PKCS_7_ASN_ENCODING | X509_ASN_ENCODING;

        /// <summary>
        /// High-word and low-word values combined using a bitwise OR operation. 
        /// </summary>
        private const uint openflags = CERT_SYSTEM_STORE_CURRENT_USER |
            CERT_STORE_READONLY_FLAG |
            CERT_STORE_OPEN_EXISTING_FLAG;

        /// <summary>
        /// Structure type.
        /// </summary>
        private const int X509_NAME = 7;

        /// <summary>
        ///  CERT_RDN attribute.
        /// </summary>
        private const string szOID_COMMON_NAME = "2.5.4.3";
        #endregion

#region Fields
        /// <summary>
        /// Structure contains information for signing messages using a specified signing certificate context.
        /// </summary>
        private CRYPT_SIGN_MESSAGE_PARA m_signParams;

        /// <summary>
        /// Certificate's version.
        /// </summary>
        private int m_version;

        /// <summary>
        /// Certificate's serial number.
        /// </summary>
        private byte[] m_serialNumber;

        /// <summary>
        /// Certificate's issuer name.
        /// </summary>
        private string m_issuerName;

        /// <summary>
        /// Certificate's subject name.
        /// </summary>
        private string m_subjectName;

        /// <summary>
        /// Certificate's structure.
        /// </summary>
        private IntPtr m_certificate;

        /// <summary>
        /// Signature length.
        /// </summary>
        private uint m_signatureLength;

        /// <summary>
        /// Date and time before which the certificate is not valid.
        /// </summary>
        private DateTime m_validTo;

        /// <summary>
        /// Date and time after which the certificate is not valid.
        /// </summary>
        private DateTime m_validFrom;

        private X509Certificate2 m_x509Certificate;
        #endregion

#region Properties
        /// <summary>
        /// Certificate's version number.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument doc = new PdfDocument();
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(brush, 0.2f);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular);
        /// PdfCertificate pdfCert = new PdfCertificate(@"PDF.pfx", "syncfusion");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), page.Size);
        /// signature.ContactInfo = "johndoe@owned.us";
        /// signature.LocationInfo = "Honolulu, Hawaii";
        /// signature.Reason = "I am author of this document.";
        /// 
        /// PdfGraphics g = signature.Appearence.Normal.Graphics;
        /// string validto = "Version: " + signature.Certificate.Version.ToString();
        /// string validfrom = "Valid From: " + signature.Certificate.ValidFrom.ToString();
        /// 
        /// doc.Pages[0].Graphics.DrawString(validfrom, font, pen, brush, 0, 90);
        /// doc.Pages[0].Graphics.DrawString(validto, font, pen, brush, 0, 110);
        /// doc.Pages[0].Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130);
        /// doc.Pages[0].Graphics.DrawString("* To validate Signature click on the signature on this page \n * To check Document Status \n click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150);
        /// // Save the PDF file.
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Black)
        /// Dim pen As PdfPen = New PdfPen(brush, 0.2f)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular)
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("PDF.pfx", "syncfusion")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// 
        /// signature.Bounds = New RectangleF(New PointF(5, 5), page.Size)
        /// signature.ContactInfo = "johndoe@owned.us"
        /// signature.LocationInfo = "Honolulu, Hawaii"
        /// signature.Reason = "I am author of this document."
        /// 
        /// Dim g As PdfGraphics = signature.Appearence.Normal.Graphics
        /// Dim validto As String = "Version: " & signature.Certificate.Version.ToString()
        /// Dim validfrom As String = "Valid From: " & signature.Certificate.ValidFrom.ToString()
        /// 
        /// doc.Pages(0).Graphics.DrawString(validfrom, font, pen, brush, 0, 90)
        /// doc.Pages(0).Graphics.DrawString(validto, font, pen, brush, 0, 110)
        /// doc.Pages(0).Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130)
        /// doc.Pages(0).Graphics.DrawString("* To validate Signature click on the signature on this page " & Constants.vbLf & " * To check Document Status " & Constants.vbLf & " click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150)
        /// ' Save the PDF file.
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfSignature"/> Class
        /// <seealso cref="PdfFont"/> Class
        public int Version
        {
            get
            {
                return m_version;
            }
        }

        /// <summary>
        /// Gets the serial number of a certificate.
        /// </summary>
        /// <value>The serial number of the certificate.</value>
        public byte[] SerialNumber
        {
            get
            {
                return m_serialNumber;
            }
        }

        /// <summary>
        /// Gets the certificate issuer's name. 
        /// </summary>
        /// <value> The certificate issuer`s name.</value>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument doc = new PdfDocument();
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(brush, 0.2f);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular);
        /// PdfCertificate pdfCert = new PdfCertificate(@"PDF.pfx", "syncfusion");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), page.Size);
        /// signature.ContactInfo = "johndoe@owned.us";
        /// signature.LocationInfo = "Honolulu, Hawaii";
        /// signature.Reason = "I am author of this document.";
        /// 
        /// PdfGraphics g = signature.Appearence.Normal.Graphics;
        /// string validto  = "Issuer Name: " + signature.Certificate.IssuerName.ToString()
        /// string validfrom = "Valid From: " + signature.Certificate.ValidFrom.ToString();
        /// 
        /// doc.Pages[0].Graphics.DrawString(validfrom, font, pen, brush, 0, 90);
        /// doc.Pages[0].Graphics.DrawString(validto, font, pen, brush, 0, 110);
        /// doc.Pages[0].Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130);
        /// doc.Pages[0].Graphics.DrawString("* To validate Signature click on the signature on this page \n * To check Document Status \n click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150);
        /// // Save the PDF file.
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Black)
        /// Dim pen As PdfPen = New PdfPen(brush, 0.2f)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular)
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("PDF.pfx", "syncfusion")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// 
        /// signature.Bounds = New RectangleF(New PointF(5, 5), page.Size)
        /// signature.ContactInfo = "johndoe@owned.us"
        /// signature.LocationInfo = "Honolulu, Hawaii"
        /// signature.Reason = "I am author of this document."
        /// 
        /// Dim g As PdfGraphics = signature.Appearence.Normal.Graphics
        /// Dim validto As String = "Issuer Name: " + signature.Certificate.IssuerName.ToString()
        /// Dim validfrom As String = "Valid From: " & signature.Certificate.ValidFrom.ToString()
        /// 
        /// doc.Pages(0).Graphics.DrawString(validfrom, font, pen, brush, 0, 90)
        /// doc.Pages(0).Graphics.DrawString(validto, font, pen, brush, 0, 110)
        /// ' Save the PDF file.
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        public string IssuerName
        {
            get
            {
                return m_issuerName;
            }
        }

        /// <summary>
        /// Gets the certificate subject's name.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument doc = new PdfDocument();
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(brush, 0.2f);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular);
        /// PdfCertificate pdfCert = new PdfCertificate(@"PDF.pfx", "syncfusion");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), page.Size);
        /// signature.ContactInfo = "johndoe@owned.us";
        /// signature.LocationInfo = "Honolulu, Hawaii";
        /// signature.Reason = "I am author of this document.";
        /// 
        /// PdfGraphics g = signature.Appearence.Normal.Graphics;
        /// string validto  = "Issuer Name: " + signature.Certificate.IssuerName.ToString()
        /// string validfrom = "Valid From: " + signature.Certificate.ValidFrom.ToString();
        /// 
        /// doc.Pages[0].Graphics.DrawString(validfrom, font, pen, brush, 0, 90);
        /// doc.Pages[0].Graphics.DrawString(validto, font, pen, brush, 0, 110);
        /// doc.Pages[0].Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130);
        /// doc.Pages[0].Graphics.DrawString("* To validate Signature click on the signature on this page \n * To check Document Status \n click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150);
        /// // Save the PDF file.
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Black)
        /// Dim pen As PdfPen = New PdfPen(brush, 0.2f)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular)
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("PDF.pfx", "syncfusion")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// 
        /// signature.Bounds = New RectangleF(New PointF(5, 5), page.Size)
        /// signature.ContactInfo = "johndoe@owned.us"
        /// signature.LocationInfo = "Honolulu, Hawaii"
        /// signature.Reason = "I am author of this document."
        /// 
        /// Dim g As PdfGraphics = signature.Appearence.Normal.Graphics
        /// Dim validto As String = "Subject Name: " + signature.Certificate.SubjectName.ToString()
        /// Dim validfrom As String = "Valid From: " & signature.Certificate.ValidFrom.ToString()
        /// 
        /// doc.Pages(0).Graphics.DrawString(validfrom, font, pen, brush, 0, 90)
        /// doc.Pages(0).Graphics.DrawString(validto, font, pen, brush, 0, 110)
        /// ' Save the PDF file.
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        public string SubjectName
        {
            get
            {
                return m_subjectName;
            }
        }

        /// <summary>
        /// Date and time before which the certificate is not valid.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument doc = new PdfDocument();
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(brush, 0.2f);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular);
        /// PdfCertificate pdfCert = new PdfCertificate(@"PDF.pfx", "syncfusion");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), page.Size);
        /// signature.ContactInfo = "johndoe@owned.us";
        /// signature.LocationInfo = "Honolulu, Hawaii";
        /// signature.Reason = "I am author of this document.";
        /// 
        /// PdfGraphics g = signature.Appearence.Normal.Graphics;
        /// string validto  = "Version To: " + signature.Certificate.ValidTo.ToString()
        /// string validfrom = "Valid From: " + signature.Certificate.ValidFrom.ToString();
        /// 
        /// doc.Pages[0].Graphics.DrawString(validfrom, font, pen, brush, 0, 90);
        /// doc.Pages[0].Graphics.DrawString(validto, font, pen, brush, 0, 110);
        /// doc.Pages[0].Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130);
        /// doc.Pages[0].Graphics.DrawString("* To validate Signature click on the signature on this page \n * To check Document Status \n click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150);
        /// // Save the PDF file.
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Black)
        /// Dim pen As PdfPen = New PdfPen(brush, 0.2f)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular)
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("PDF.pfx", "syncfusion")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// 
        /// signature.Bounds = New RectangleF(New PointF(5, 5), page.Size)
        /// signature.ContactInfo = "johndoe@owned.us"
        /// signature.LocationInfo = "Honolulu, Hawaii"
        /// signature.Reason = "I am author of this document."
        /// 
        /// Dim g As PdfGraphics = signature.Appearence.Normal.Graphics
        /// Dim validto As String = "Version To: " + signature.Certificate.ValidTo.ToString()
        /// Dim validfrom As String = "Valid From: " & signature.Certificate.ValidFrom.ToString()
        /// 
        /// doc.Pages(0).Graphics.DrawString(validfrom, font, pen, brush, 0, 90)
        /// doc.Pages(0).Graphics.DrawString(validto, font, pen, brush, 0, 110)
        /// doc.Pages(0).Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130)
        /// doc.Pages(0).Graphics.DrawString("* To validate Signature click on the signature on this page " & Constants.vbLf & " * To check Document Status " & Constants.vbLf & " click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150)
        /// ' Save the PDF file.
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        public DateTime ValidTo
        {
            get
            {
                return m_validTo;
            }
        }

        /// <summary>
        /// Date and time after which the certificate is not valid.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument doc = new PdfDocument();
        /// PdfPage page = doc.Pages.Add();
        /// PdfSolidBrush brush = new PdfSolidBrush(Color.Black);
        /// PdfPen pen = new PdfPen(brush, 0.2f);
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular);
        /// PdfCertificate pdfCert = new PdfCertificate(@"PDF.pfx", "syncfusion");
        /// PdfSignature signature = new PdfSignature(page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), page.Size);
        /// signature.ContactInfo = "johndoe@owned.us";
        /// signature.LocationInfo = "Honolulu, Hawaii";
        /// signature.Reason = "I am author of this document.";
        /// 
        /// PdfGraphics g = signature.Appearence.Normal.Graphics;
        /// string validto  = "Version To: " + signature.Certificate.ValidTo.ToString()
        /// string validfrom = "Valid From: " + signature.Certificate.ValidFrom.ToString();
        /// 
        /// doc.Pages[0].Graphics.DrawString(validfrom, font, pen, brush, 0, 90);
        /// doc.Pages[0].Graphics.DrawString(validto, font, pen, brush, 0, 110);
        /// doc.Pages[0].Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130);
        /// doc.Pages[0].Graphics.DrawString("* To validate Signature click on the signature on this page \n * To check Document Status \n click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150);
        /// // Save the PDF file.
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Black)
        /// Dim pen As PdfPen = New PdfPen(brush, 0.2f)
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12, PdfFontStyle.Regular)
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("PDF.pfx", "syncfusion")
        /// Dim signature As PdfSignature = New PdfSignature(page, pdfCert, "Signature")
        /// 
        /// signature.Bounds = New RectangleF(New PointF(5, 5), page.Size)
        /// signature.ContactInfo = "johndoe@owned.us"
        /// signature.LocationInfo = "Honolulu, Hawaii"
        /// signature.Reason = "I am author of this document."
        /// 
        /// Dim g As PdfGraphics = signature.Appearence.Normal.Graphics
        /// Dim validto As String = "Version To: " + signature.Certificate.ValidTo.ToString()
        /// Dim validfrom As String = "Valid From: " & signature.Certificate.ValidFrom.ToString()
        /// 
        /// doc.Pages(0).Graphics.DrawString(validfrom, font, pen, brush, 0, 90)
        /// doc.Pages(0).Graphics.DrawString(validto, font, pen, brush, 0, 110)
        /// doc.Pages(0).Graphics.DrawString(" Protected Document. Digitally signed Document.", font, pen, brush, 0, 130)
        /// doc.Pages(0).Graphics.DrawString("* To validate Signature click on the signature on this page " & Constants.vbLf & " * To check Document Status " & Constants.vbLf & " click document status icon on the bottom left of the acrobat reader.", font, pen, brush, 0, 150)
        /// ' Save the PDF file.
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfGraphics"/> Class
        /// <seealso cref="PdfFont"/> Class
        public DateTime ValidFrom
        {
            get
            {
                return m_validFrom;
            }
        }

        /// <summary>
        /// Certificate's structure.
        /// </summary>
        internal IntPtr SysCertificate
        {
            get
            {
                return m_certificate;
            }
        }

        internal X509Certificate2 X509Certificate
        {
            get
            {
                return m_x509Certificate;
            }
        }
        #endregion

#region Constructor
        /// <summary>
        /// Creates new PdfCertificate from PFX file.
        /// </summary>
        /// <param name="pfxPath">The path to pfx file.</param>
        /// <param name="password">The password for pfx file.</param>
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
        public PdfCertificate(string pfxPath, string password)
        {
            if (pfxPath == null)
                throw new ArgumentNullException("pfxPath");

            if (password == null)
                throw new ArgumentNullException("password");

            Initialize(pfxPath, password);

            m_x509Certificate = new X509Certificate2(pfxPath, password);
        }

        /// <summary>
        /// Creates new pdf certificate object.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        internal PdfCertificate(IntPtr certificate)
        {
            if (certificate == IntPtr.Zero)
                throw new ArgumentNullException("certificate");

            Initialize(certificate);
        }
        #endregion

#region Implementation
        /// <summary>
        /// Gets the certificates in all storages.
        /// </summary>
        /// <returns>PdfCertificate array.</returns>
        public static PdfCertificate[] GetCertificates()
        {
            List<PdfCertificate> certCollection = new List<PdfCertificate>();

            GetCertificatesByType(StoreType.CA, certCollection);
            GetCertificatesByType(StoreType.MY, certCollection);
            GetCertificatesByType(StoreType.ROOT, certCollection);
            GetCertificatesByType(StoreType.SPC, certCollection);

            int count = certCollection.Count;

            if (count != 0)
            {
                PdfCertificate[] certificates = new PdfCertificate[count];

                for (int i = 0; i < count; i++)
                {
                    certificates[i] = certCollection[i] as PdfCertificate;
                }

                return certificates;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the certificates by.
        /// </summary>
        /// <param name="type">The storage type.</param>
        /// <param name="certList">The cert list.</param>
        private static void GetCertificatesByType(StoreType type, List<PdfCertificate> certList)
        {
            IntPtr certificateStorage = CryptoApi.CertOpenSystemStore(IntPtr.Zero, type.ToString());

            IntPtr hCertCntxt;
            IntPtr currentCertContext;
            currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, (IntPtr)0);

            while (currentCertContext != IntPtr.Zero)
            {
                hCertCntxt = CryptoApi.CertDuplicateCertificateContext(currentCertContext);
                PdfCertificate cert = new PdfCertificate(hCertCntxt);
                certList.Add(cert);

                currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, currentCertContext);
            }

            if (certificateStorage != IntPtr.Zero)
            {
                CryptoApi.CertCloseStore(certificateStorage, 0);
            }
        }

        /// <summary>
        /// Finds the certificate by subject.
        /// </summary>
        /// <param name="type">The store type.</param>
        /// <param name="subject">The certificate subject.</param>
        /// <returns></returns>
        public static PdfCertificate FindBySubject(StoreType type, string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            IntPtr certificateStorage = CryptoApi.CertOpenSystemStore(IntPtr.Zero, type.ToString());

            IntPtr hCertCntxt;
            IntPtr currentCertContext;
            currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, (IntPtr)0);

            while (currentCertContext != IntPtr.Zero)
            {
                hCertCntxt = CryptoApi.CertDuplicateCertificateContext(currentCertContext);


                string subjectName = CryptDecodeObjectEx(GetCertificateInfo(hCertCntxt).Subject);

                if (subject == subjectName)
                {
                    if (certificateStorage != IntPtr.Zero)
                    {
                        CryptoApi.CertCloseStore(certificateStorage, 0);
                    }

                    return new PdfCertificate(hCertCntxt);
                }
               
                CryptoApi.CertFreeCertificateContext(hCertCntxt);
                
                currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, currentCertContext);
            }

            if (certificateStorage != IntPtr.Zero)
            {
                CryptoApi.CertCloseStore(certificateStorage, 0);
            }

            return null;
        }

        /// <summary>
        /// Finds the certificate by issuer.
        /// </summary>
        /// <param name="type">The certification system store type.</param>
        /// <param name="issuer">The issuer.</param>
        /// <returns></returns>
        public static PdfCertificate FindByIssuer(StoreType type, string issuer)
        {
            if (issuer == null)
                throw new ArgumentNullException("issuer");

            IntPtr certificateStorage = CryptoApi.CertOpenSystemStore(IntPtr.Zero, type.ToString());

            IntPtr hCertCntxt;
            IntPtr currentCertContext;
            currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, (IntPtr)0);

            while (currentCertContext != IntPtr.Zero)
            {
                hCertCntxt = CryptoApi.CertDuplicateCertificateContext(currentCertContext);

                string issuerName = CryptDecodeObjectEx(GetCertificateInfo(hCertCntxt).Issuer);

                if (issuer == issuerName)
                {
                    if (certificateStorage != IntPtr.Zero)
                    {
                        CryptoApi.CertCloseStore(certificateStorage, 0);
                    }

                    return new PdfCertificate(hCertCntxt);
                }
                currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, currentCertContext);
            }

            if (certificateStorage != IntPtr.Zero)
            {
                CryptoApi.CertCloseStore(certificateStorage, 0);
            }

            return null;
        }

        /// <summary>
        /// Finds the certificate by serial number.
        /// </summary>
        /// <param name="type">The certification system store type.</param>
        /// <param name="certId">The certificate id.</param>
        /// <returns></returns>
        public static PdfCertificate FindBySerialId(StoreType type, byte[] certId)
        {
            if (certId == null)
                throw new ArgumentNullException("certId");

            IntPtr certificateStorage = CryptoApi.CertOpenSystemStore(IntPtr.Zero, type.ToString());

            IntPtr hCertCntxt;
            IntPtr currentCertContext;
            currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, (IntPtr)0);

            while (currentCertContext != IntPtr.Zero)
            {
                hCertCntxt = CryptoApi.CertDuplicateCertificateContext(currentCertContext);

                CERT_INFO ci = GetCertificateInfo(hCertCntxt);

                byte[] id = new byte[ci.SerialNumber.cbData];
                Marshal.Copy(ci.SerialNumber.pbData, id, 0, id.Length);

                if (Equals(id, certId))
                {
                    if (certificateStorage != IntPtr.Zero)
                    {
                        CryptoApi.CertCloseStore(certificateStorage, 0);
                    }

                    return new PdfCertificate(hCertCntxt);
                }
                currentCertContext = CryptoApi.CertEnumCertificatesInStore(certificateStorage, currentCertContext);
            }

            if (certificateStorage != IntPtr.Zero)
            {
                CryptoApi.CertCloseStore(certificateStorage, 0);
            }

            return null;
        }

        /// <summary>
        /// Initializes PdfCertificate object.
        /// </summary>
        private void Initialize(string pfxFileName, string password)
        {
            IntPtr hMemStore = IntPtr.Zero;

            if (!File.Exists(pfxFileName))
                throw new PdfException("File is not found");

            byte[] pfxdata = GetFileBytes(pfxFileName);

            CRYPT_DATA_BLOB ppfx = new CRYPT_DATA_BLOB();
            ppfx.cbData = pfxdata.Length;
            ppfx.pbData = Marshal.AllocHGlobal(pfxdata.Length);
            Marshal.Copy(pfxdata, 0, ppfx.pbData, pfxdata.Length);


            // make sure we have a valid pfx file.
            if (!CryptoApi.PFXIsPFXBlob(ref ppfx))
                throw new ArgumentException("File has wrong format");

            // try to import to memory store
            hMemStore = CryptoApi.PFXImportCertStore(ref ppfx, password, CRYPT_USER_KEYSET);

            if (hMemStore == IntPtr.Zero)
            {
                string errormessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new ArgumentException(errormessage);
            }

            Marshal.FreeHGlobal(ppfx.pbData);

            //m_certificates = new PDFCertificateCollection();

            IntPtr hCertCntxt;
            IntPtr currentCertContext;
            currentCertContext = CryptoApi.CertEnumCertificatesInStore(hMemStore, (IntPtr)0);

            while (currentCertContext != IntPtr.Zero)
            {
                hCertCntxt = CryptoApi.CertDuplicateCertificateContext(currentCertContext);
                Initialize(hCertCntxt);
                currentCertContext = CryptoApi.CertEnumCertificatesInStore(hMemStore, currentCertContext);
            }

            if (hMemStore != IntPtr.Zero)
            {
                CryptoApi.CertCloseStore(hMemStore, 0);
            }
        }

        /// <summary>
        /// Initializes PdfCertificate object.
        /// </summary>
        /// <param name="certificate">Certificate's structure.</param>
        private void Initialize(IntPtr certificate)
        {
            m_certificate = certificate;

            CERT_CONTEXT signerCert;
            signerCert = (CERT_CONTEXT)Marshal.PtrToStructure(certificate, typeof(CERT_CONTEXT));

            IntPtr ptrCertInfo = signerCert.pCertInfo;
            CERT_INFO certInfo = (CERT_INFO)Marshal.PtrToStructure(ptrCertInfo, typeof(CERT_INFO));

            m_version = certInfo.dwVersion;

            m_serialNumber = new byte[certInfo.SerialNumber.cbData];
            Marshal.Copy(certInfo.SerialNumber.pbData, m_serialNumber, 0, m_serialNumber.Length);

            string m_signatureAlgorithm = certInfo.SignatureAlgorithm.pszObjId;

            m_issuerName = CryptDecodeObjectEx(certInfo.Issuer);

            m_subjectName = CryptDecodeObjectEx(certInfo.Subject);

            m_validFrom = ConvertTime(certInfo.NotBefore);

            m_validTo = ConvertTime(certInfo.NotAfter);

            // Initialize signature params. 
            m_signParams = new CRYPT_SIGN_MESSAGE_PARA();
            m_signParams.cbSize = (uint)Marshal.SizeOf(typeof(CRYPT_SIGN_MESSAGE_PARA));
            m_signParams.dwMsgEncodingType = PKCS_7_ASN_ENCODING | X509_ASN_ENCODING;
            m_signParams.pSigningCert = m_certificate;
            m_signParams.HashAlgorithm.pszObjId = m_signatureAlgorithm;
            m_signParams.HashAlgorithm.Parameters.cbData = 0;
            m_signParams.pvHashAuxInfo = new IntPtr(0);
            m_signParams.cMsgCert = 1;
            m_signParams.rgpMsgCert = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.StructureToPtr(m_certificate, m_signParams.rgpMsgCert, true);
            m_signParams.cMsgCrl = 0;
            m_signParams.rgpMsgCrl = new IntPtr(0);
            m_signParams.cAuthAttr = 0;
            m_signParams.rgAuthAttr = new IntPtr(0);
            m_signParams.cUnauthAttr = 0;
            m_signParams.rgUnauthAttr = new IntPtr(0);
            m_signParams.dwFlags = 0;
            m_signParams.dwInnerContentType = 0;
        }

        /// <summary>
        /// Gets signature length.
        /// </summary>
        /// <returns>Signature length.</returns>
        internal uint GetSignatureLength()
        {
            if (m_signatureLength == 0)
            {
                // NOTE: We just have to pass in the method any string, it doesn't
                // matter which exactly.
                string text = Environment.CurrentDirectory;

                byte[] data = Encoding.UTF8.GetBytes(text);
                int[] lengthConvertedString = { data.Length };
                IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                IntPtr[] res = new IntPtr[] { ptr };

                bool failed = !CryptoApi.CryptSignMessage(ref m_signParams, true, 1, res, lengthConvertedString,
                    IntPtr.Zero, ref m_signatureLength);

                if (failed)
                {
                    uint error = KernelApi.GetLastError();
                    IntPtr lpBuffer = Marshal.AllocHGlobal(4);
                    uint result = KernelApi.FormatMessage(
                        FormatMessageFlags.AllocateBuffer | FormatMessageFlags.FromSystem,
                        (IntPtr)0, error, 0, lpBuffer, 4, (IntPtr)0);

                    byte[] errdata = new byte[4];
                    Marshal.Copy(lpBuffer, errdata, 0, 4);
                    int pointer = BitConverter.ToInt32(errdata, 0);
                    Marshal.FreeHGlobal(lpBuffer);

                    lpBuffer = new IntPtr(pointer);
                    errdata = new byte[result];
                    Marshal.Copy(lpBuffer, errdata, 0, (int)result);
                    Marshal.FreeHGlobal(lpBuffer);

                    string errorString = System.Text.Encoding.UTF8.GetString(errdata);
                    throw new Exception(errorString);
                }
            }

            return m_signatureLength;
        }

        /// <summary>
        /// Gets signature value.
        /// </summary>
        /// <param name="dataBlocks">Signature string.</param>
        /// <returns>Signature value.</returns>
        internal byte[] GetSignatureValue(byte[][] dataBlocks)
        {
            if (dataBlocks == null)
                throw new ArgumentNullException("dataBlocks");

            uint signatureLength = GetSignatureLength();
            int blocksCount = dataBlocks.Length;
            byte[] result = new byte[signatureLength];

            IntPtr[] res = new IntPtr[blocksCount];
            int[] rangeSize = new int[blocksCount];

            for (int i = 0; i < blocksCount; i++)
            {
                byte[] data = dataBlocks[i];
                IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                res[i] = ptr;
                rangeSize[i] = data.Length;
            }

            IntPtr pbSignature = Marshal.AllocCoTaskMem((int)signatureLength);

            CryptoApi.CryptSignMessage(ref m_signParams, true, (uint)blocksCount, res, rangeSize,
                pbSignature, ref signatureLength);

            Marshal.Copy(pbSignature, result, 0, result.Length);

            return result;
        }

        /// <summary>
        /// Retrieves data from the file.
        /// </summary>
        /// <param name="filename">Path to the file.</param>
        /// <returns>Data from the file if found, null otherwise.</returns>
        private static byte[] GetFileBytes(String filename)
        {
            if (!File.Exists(filename))
                return null;

            using (FileStream stream = File.OpenRead(filename))
            {
                int datalen = (int)stream.Length;
                byte[] filebytes = new byte[datalen];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(filebytes, 0, datalen);

                return filebytes;
            }
        }

        /// <summary>
        /// Converts FILETIME to DataTime.
        /// </summary>
        /// <param name="filetime">FILETIME struct.</param>
        /// <returns>DateTime struct.</returns>
        private DateTime ConvertTime(Syncfusion.Pdf.Native.FILETIME filetime)
        {
            SYSTEMTIME systime;
            systime = new SYSTEMTIME();

            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.StructureToPtr(filetime, ptr, true);
            KernelApi.FileTimeToSystemTime(ptr, ref systime);
            DateTime result = new DateTime(systime.wYear, systime.wMonth, systime.wDay,
                systime.wHour, systime.wMinute, systime.wSecond, systime.wMilliseconds);

            return result;
        }

        /// <summary>
        /// Decodes a structure.
        /// </summary>
        /// <param name="blob">Code structure.</param>
        /// <returns>Decode value.</returns>
        private static string CryptDecodeObjectEx(CRYPTOAPI_BLOB blob)
        {
            IntPtr refPtr = IntPtr.Zero;
            int size = 0;

            CryptoApi.CryptDecodeObject(ENCODING_TYPE, X509_NAME, blob.pbData, blob.cbData,
                0, refPtr, ref size);

            refPtr = Marshal.AllocHGlobal((int)size);

            CryptoApi.CryptDecodeObject(ENCODING_TYPE, X509_NAME, blob.pbData, blob.cbData,
                0, refPtr, ref size);

            CERT_NAME_INFO certInfo = (CERT_NAME_INFO)Marshal.PtrToStructure(refPtr,
                typeof(CERT_NAME_INFO));

            string commone_name = string.Empty;
            Marshal.FreeHGlobal(refPtr);
            refPtr = certInfo.rgRDN;
            CERT_RDN_ATTR cerATTR = new CERT_RDN_ATTR();

            int i = 0;

            while (szOID_COMMON_NAME != commone_name && certInfo.cRDN != i)
            {
                CERT_RDN certRDN = (CERT_RDN)Marshal.PtrToStructure(refPtr,
                    typeof(CERT_RDN));

                cerATTR = (CERT_RDN_ATTR)Marshal.PtrToStructure(certRDN.rgRDNAttr,
                    typeof(CERT_RDN_ATTR));

                commone_name = cerATTR.pszObjId;

                refPtr = new IntPtr(refPtr.ToInt32() + Marshal.SizeOf(certRDN));
                i++;
            }

            byte[] data = new byte[cerATTR.Value.cbData];

            if (data.Length == 0)
            {
                return null;
            }
            else
            {
                string result = null;

                Marshal.Copy(cerATTR.Value.pbData, data, 0, data.Length);

                if (cerATTR.dwValueType == 4 || cerATTR.dwValueType == 5)
                {
                    result = new string(Encoding.UTF8.GetChars(data));
                }
                else if (cerATTR.dwValueType == 12 || cerATTR.dwValueType == 13)
                {
                    result = Encoding.Unicode.GetString(data, 0, data.Length);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the certificate issuer.
        /// </summary>
        /// <param name="hCertCtx">The handle of certificate context.</param>
        /// <returns></returns>
        private static CERT_INFO GetCertificateInfo(IntPtr hCertCtx)
        {
            CERT_CONTEXT signerCert = (CERT_CONTEXT)Marshal.PtrToStructure(
                hCertCtx,
                typeof(CERT_CONTEXT));

            CERT_INFO certInfo = (CERT_INFO)Marshal.PtrToStructure(
                signerCert.pCertInfo,
                typeof(CERT_INFO));

            return certInfo;
        }

        /// <summary>
        /// Checks whether arrays of bytes are equal.
        /// </summary>
        /// <param name="arr1">First array.</param>
        /// <param name="arr2">Second array.</param>
        /// <returns>True if data are equal, False otherwise.</returns>
        private static bool Equals(byte[] arr1, byte[] arr2)
        {
            if (arr1 == null)
                throw new ArgumentNullException("arr1");

            if (arr2 == null)
                throw new ArgumentNullException("arr2");

            bool result = (arr1.Length == arr2.Length);

            if (result)
            {
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] != arr2[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
#endif