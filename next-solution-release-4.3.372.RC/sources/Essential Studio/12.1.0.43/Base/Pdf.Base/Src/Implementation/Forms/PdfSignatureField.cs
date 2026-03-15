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

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents signature field in the PDF Form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create signature field
    /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
    /// sign.Bounds = new RectangleF(100, 420, 100, 50);
    /// document.Form.Fields.Add(sign);           
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create signature field
    /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
    /// sign.Bounds = New RectangleF(100, 420, 100, 50)
    /// document.Form.Fields.Add(sign)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfSignatureAppearanceField"/> Class  
    /// <seealso cref="PdfDocument"/> Class 
    /// <seealso cref="PdfPage"/> Class  
#if AllowUnsafeCode
    public class PdfSignatureField : PdfSignatureAppearanceField
#else
    internal class PdfSignatureField : PdfSignatureAppearanceField
#endif
    {
#region Fields
        /// <summary>
        /// Internal variable to store the signature.
        /// </summary>
        private PdfSignature m_signature;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureField"/> class.
        /// </summary>
        /// <param name="page">Page which the field to be placed on.</param>
        /// <param name="name">The name of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create signature field
        /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
        /// sign.Bounds = new RectangleF(100, 420, 100, 50);
        /// document.Form.Fields.Add(sign);           
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create signature field
        /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
        /// sign.Bounds = New RectangleF(100, 420, 100, 50)
        /// document.Form.Fields.Add(sign)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>        
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfPage"/> Class
        public PdfSignatureField(PdfPageBase page, string name)
            : base(page, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureField"/> class.
        /// </summary>
        internal PdfSignatureField()
        {
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets the visual appearance of this field. 
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create signature field
        /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
        /// sign.Bounds = new RectangleF(100, 420, 100, 50);
        /// // Gets the signature appearance
        /// PdfAppearance appearance = sign.Appearance;  
        /// document.Form.Fields.Add(sign);           
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create signature field
        /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
        /// ' Gets the signature appearance
        /// Dim appearance As PdfAppearance = sign.Appearance
        /// sign.Bounds = New RectangleF(100, 420, 100, 50)
        /// document.Form.Fields.Add(sign)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfPage"/> Class
        public PdfAppearance Appearance
        {
            get
            {
                return Widget.Appearance;
            }
        }

        /// <summary>
        /// Gets or sets the digital signature for signing the field. 
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create signature field
        /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
        /// sign.Signature = new PdfSignature(page, new PdfCertificate("PDF.pfx", "Syncfusion"), "Signature");
        /// sign.Bounds = new RectangleF(100, 420, 100, 50);
        /// // Gets the signature appearance
        /// PdfAppearance appearance = sign.Appearance;  
        /// document.Form.Fields.Add(sign);           
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create signature field
        /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
        /// sign.Signature = New PdfSignature(page, New PdfCertificate("PDF.pfx", "Syncfusion"), "Signature")
        /// sign.Bounds = New RectangleF(100, 420, 100, 50)
        /// document.Form.Fields.Add(sign)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfSignature"/> Class
        public PdfSignature Signature
        {
            get
            {
                return m_signature;
            }

            set
            {
                m_signature = value;
            }
        }

        #endregion

#region Implementation
        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Sig));
        }

        /// <summary>
        /// Saves the signature.
        /// </summary>
        internal override void Save()
        {
            base.Save();
            if (m_signature != null)
            {
                PdfSignatureDictionary sigDic = new PdfSignatureDictionary(((PdfPage)Page).Document, m_signature, m_signature.Certificate);
                Dictionary[DictionaryProperties.V] = new PdfReferenceHolder(sigDic);
            }
        }

        /// <summary>
        /// Draws the field.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
            if (Widget.GetAppearance() != null)
            {
                Page.Graphics.DrawPdfTemplate(Appearance.Normal, Location);
            }
        }

        /// <summary>
        /// Draws the appearance.
        /// </summary>
        /// <param name="template">The template.</param>
        protected override void DrawAppearance(PdfTemplate template)
        {
            base.DrawAppearance(template);

            if (m_signature != null && m_signature.DrawFieldAppearance)
            {
                PaintParams paintParams = new PaintParams(
                    new RectangleF(PointF.Empty, Size), BackBrush, null,
                    BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

                FieldPainter.DrawSignature(template.Graphics, paintParams);
            }
        }
        #endregion
    }
}
#endif