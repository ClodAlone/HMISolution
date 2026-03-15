#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Specifies length of the encryption key for encryption.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// // Set the documents permission settings
    /// doc.Security.KeySize = PdfEncryptionKeySize.Key128Bit;
    /// doc.Security.OwnerPassword = "Syncfusion";
    /// doc.Security.Permissions = PdfPermissionsFlags.EditAnnotations;
    /// doc.Security.UserPassword = "123";
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
    /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, new PointF(10,10));
    /// doc.Save("Security.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Set the documents permission settings
    /// doc.Security.KeySize = PdfEncryptionKeySize.Key128Bit
    /// doc.Security.OwnerPassword = "Syncfusion"
    /// doc.Security.Permissions = PdfPermissionsFlags.EditAnnotations
    /// doc.Security.UserPassword = "123"
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 10)
    /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, New PointF(10,10))
    /// doc.Save("Security.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfSecurity"/> Class
    /// <seealso cref="PdfPage"/> Class
    /// <seealso cref="PdfFont"/> Class
    public enum PdfEncryptionKeySize
    {
        /// <summary>
        /// The key is 40 bit long.
        /// </summary>
        Key40Bit = 1,
        /// <summary>
        /// The key is 128 bit long.
        /// </summary>
        Key128Bit = 2,
        /// <summary>
        /// The key is 256 bit long.
        /// </summary>
        Key256Bit = 3
    }

    /// <summary>
    /// Specifies the type of encryption algorithm used.
    /// </summary>
    public enum PdfEncryptionAlgorithm
    {
		/// <summary>
        /// The encryption algorithm is RC4.
        /// </summary>
        RC4 = 1,
		/// <summary>
        /// The encryption algorithm is AES.
        /// </summary>
        AES = 2
    }
    /// <summary>
    /// Specifies the available permissions set for the signature.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// // Set the documents permission settings
    /// doc.Security.KeySize = PdfEncryptionKeySize.Key128Bit;
    /// doc.Security.OwnerPassword = "Syncfusion";
    /// doc.Security.Permissions = PdfPermissionsFlags.EditAnnotations;
    /// doc.Security.UserPassword = "123";
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
    /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, new PointF(10,10));
    /// doc.Save("Security.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Set the documents permission settings
    /// doc.Security.KeySize = PdfEncryptionKeySize.Key128Bit
    /// doc.Security.OwnerPassword = "Syncfusion"
    /// doc.Security.Permissions = PdfPermissionsFlags.EditAnnotations
    /// doc.Security.UserPassword = "123"
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 10)
    /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, New PointF(10,10))
    /// doc.Save("Security.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfSecurity"/> Class
    /// <seealso cref="PdfPage"/> Class
    /// <seealso cref="PdfFont"/> Class
    [Flags]
    public enum PdfPermissionsFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Default = 0x000000,//-4,
        /// <summary>
        /// Print the document.
        /// </summary>
        Print = 0x000004,
        /// <summary>
        /// Edit content.
        /// </summary>
        EditContent = 0x000008,
        /// <summary>
        /// Copy content.
        /// </summary>
        CopyContent = 0x000010,
        /// <summary>
        /// Add or modify text annotations, fill in interactive form fields.
        /// </summary>
        EditAnnotations = 0x000020,
        /// <summary>
        /// Fill form fields. (Only for 128 bits key).
        /// </summary>
        FillFields = 0x000100,
        /// <summary>
        /// Copy accessibility content.
        /// </summary>
        AccessibilityCopyContent = 0x000200,
        /// <summary>
        /// Assemble document permission. (Only for 128 bits key).
        /// </summary>
        AssembleDocument = 0x000400,
        /// <summary>
        /// Full quality print.
        /// </summary>
        FullQualityPrint = 0x000800
    }

    /// <summary>
    /// Enumerator that implements possible security handlers.
    /// </summary>
    internal enum SecurityHandlers
    {
        /// <summary>
        /// The built-in password-based security handler.
        /// </summary>
        Standard,
    }

    /// <summary>
    /// Specifies the naming a system store.
    /// </summary>
    public enum StoreType
    {
        /// <summary>
        /// A certificate store that holds certificates with associated private keys.
        /// </summary>
        MY,
        /// <summary>
        /// Root certificates.
        /// </summary>
        ROOT,
        /// <summary>
        /// Certification authority certificates.
        /// </summary>
        CA,
        /// <summary>
        /// Software Publisher Certificate.
        /// </summary>
        SPC
    }

    /// <summary>
    /// Specifies the available permissions on certificated document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
    /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
    /// signature.DocumentPermissions = PdfCertificationFlags.AllowComments;
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
    /// doc.Save("SignedPdfSample.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfGraphics"/> Class
    /// <seealso cref="PdfFont"/> Class
    /// <seealso cref="PdfCertificate"/> Class
    public enum PdfCertificationFlags
    {
        /// <summary>
        /// Disallow any changes to the document.
        /// </summary>
        ForbidChanges = 1,
        /// <summary>
        /// Only allow form fill-in actions on this document.
        /// </summary>
        AllowFormFill = 2,
        /// <summary>
        /// Only allow commenting and form fill-in actions on this document.
        /// </summary>
        AllowComments = 3
    }

    /// <summary>
    /// Enumeration of signature flags.
    /// </summary>
    [Flags]
    internal enum SignatureFlags
    {
        /// <summary>
        /// No flags specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// If set, the document contains at least one signature field. This flag allows a viewer 
        /// application to enable user interface items (such as menu items or pushbuttons) related 
        /// to signature processing without having to scan the entire document for the presence 
        /// of signature fields.
        /// </summary>
        SignaturesExists = 1,
        /// <summary>
        /// If set, the document contains signatures that may be invalidated if the file is saved 
        /// (written) in a way that alters its previous contents, as opposed to an incremental 
        /// update. Merely updating the file by appending new information to the end of the 
        /// previous version is safe. Viewer applications can use this flag to present 
        /// a user requesting a full save with an additional alert box warning that signatures 
        /// will be invalidated and requiring explicit confirmation before continuing with the operation.
        /// </summary>
        AppendOnly = 2
    }
}