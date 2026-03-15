#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !WP

using System;
using System.Text;

using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Represents the security settings of the PDF document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfGraphics graphics = page.Graphics;
    /// PdfStandardFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 20f, PdfFontStyle.Bold);
    /// PdfBrush brush = PdfBrushes.Black;
    /// //Document security
    /// PdfSecurity security = document.Security;
    /// //use 128 bits key
    /// security.KeySize = PdfEncryptionKeySize.Key128Bit;
    /// security.OwnerPassword = "syncfusion";
    /// security.Permissions = PdfPermissionsFlags.Print | PdfPermissionsFlags.FullQualityPrint;
    /// security.UserPassword = "password";
    /// document.Save("Security.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim document As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim graphics As PdfGraphics = page.Graphics
    /// Dim font As PdfStandardFont = New PdfStandardFont(PdfFontFamily.TimesRoman, 20f, PdfFontStyle.Bold)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// 'Document security
    /// Dim security As PdfSecurity = document.Security
    /// 'use 128 bits key
    /// security.KeySize = PdfEncryptionKeySize.Key128Bit
    /// security.OwnerPassword = "syncfusion"
    /// security.Permissions = PdfPermissionsFlags.Print Or PdfPermissionsFlags.FullQualityPrint
    /// security.UserPassword = "password"
    /// document.Save("Security.pdf")
    /// </code>
    /// </example>
    public class PdfSecurity
    {
#region Fields
        /// <summary>
        /// Owner password value.
        /// </summary>
        private string m_ownerPassword;

        /// <summary>
        /// User password value.
        /// </summary>
        private string m_userPassword;

        /// <summary>
        /// Object encrypting the data.
        /// </summary>
        private PdfEncryptor m_encryptor;
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the owner password.
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
        public string OwnerPassword
        {
            get
            {
                return m_encryptor.OwnerPassword;
            }
            set
            {
                if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
                    throw new Exception("Document encryption is not allowed with PDF/A1-B Conformance documents.");

                m_encryptor.OwnerPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets the user password.
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
        public string UserPassword
        {
            get
            {
                return m_encryptor.UserPassword;
            }
            set
            {
                if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
                    throw new Exception("Document encryption is not allowed with PDF/A1-B Conformance documents.");

                m_encryptor.UserPassword = value;
            }
        }

        /// <summary>
        /// Permissions when the document is opened with user password.
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
        public PdfPermissionsFlags Permissions
        {
            get
            {
                return m_encryptor.Permissions;
            }
            set
            {
                if (m_encryptor.Permissions != value)
                {
                    m_encryptor.Permissions = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the encryptor.
        /// </summary>
        internal PdfEncryptor Encryptor
        {
            get
            {
                return m_encryptor;
            }
            set
            {
                m_encryptor = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the key.
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
        public PdfEncryptionKeySize KeySize
        {
            get
            {
                return m_encryptor.CryptographicAlgorithm;
            }
            set
            {
                m_encryptor.CryptographicAlgorithm = value;
            }
        }
		/// <summary>
        /// Gets or sets the type of encryption algorithm.
        /// </summary>
        public PdfEncryptionAlgorithm Algorithm
        {

            get
            {
                return m_encryptor.EncryptionAlgorithm;
            }
            set
            {
                m_encryptor.EncryptionAlgorithm = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PdfSecurity"/> is enabled.
        /// </summary>
        internal bool Enabled
        {
            get
            {
                return m_encryptor.Encrypt;
            }
            set
            {
                m_encryptor.Encrypt = value;
            }
        }

        #endregion

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSecurity"/> class.
        /// </summary>
        public PdfSecurity()
        {
            m_ownerPassword = string.Empty;
            m_userPassword = string.Empty;
            m_encryptor = new PdfEncryptor();
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Logically ORs flag and mask and return result.
        /// </summary>
        /// <param name="flags">The mask of set bit that should be set in the result.</param>
        /// <returns>The ORed value of flag and mask.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfSecurity security = doc.Security;
        /// security.SetPermissions(PdfPermissionsFlags.AssembleDocument);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, new PointF(10,10));
        /// doc.Save("Security.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Document security
        /// Dim security As PdfSecurity = doc.Security
        /// security.SetPermissions(PdfPermissionsFlags.AssembleDocument)
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
        public PdfPermissionsFlags SetPermissions(PdfPermissionsFlags flags)
        {

            Permissions |= flags;

            return Permissions;
        }

        /// <summary>
        /// Logically ANDs flag and inverted mask and return result.
        /// </summary>
        /// <param name="flags">The mask of set bit that should be cleared in the result.</param>
        /// <returns>The ANDed value of flag and inverted mask.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfSecurity security = doc.Security;
        /// security.ResetPermissions(PdfPermissionsFlags.AssembleDocument);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        /// page.Graphics.DrawString("Permission",font,PdfBrushes.Blue, new PointF(10,10));
        /// doc.Save("Security.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Document security
        /// Dim security As PdfSecurity = doc.Security
        /// security.ResetPermissions(PdfPermissionsFlags.AssembleDocument)
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
        public PdfPermissionsFlags ResetPermissions(PdfPermissionsFlags flags)
        {
            Permissions &= ~flags;

            return Permissions;
        }
        #endregion
    }
}
#endif