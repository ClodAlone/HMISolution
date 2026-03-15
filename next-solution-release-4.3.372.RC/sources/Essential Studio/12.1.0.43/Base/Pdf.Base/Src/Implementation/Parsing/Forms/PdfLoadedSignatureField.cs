#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the signature field of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Gets the signature field         
    /// PdfLoadedSignatureField signatureField = doc.Form.Fields["ManagerSignature"] as PdfLoadedSignatureField;
    /// signatureField.Flatten = true;            
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    ///  ' Gets the signature field         
    ///  Dim signatureField As PdfLoadedSignatureField = TryCast(doc.Form.Fields("ManagerSignature"), PdfLoadedSignatureField)
    ///  signatureField.Flatten = True
    ///  doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStyledField"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
#if AllowUnsafeCode
    public class PdfLoadedSignatureField : PdfLoadedStyledField
#else
    internal class PdfLoadedSignatureField : PdfLoadedStyledField
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
        /// Initializes a new instance of the <see cref="PdfLoadedSignatureField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedSignatureField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            if (dictionary.ContainsKey(DictionaryProperties.V))
                SetSignature(dictionary[DictionaryProperties.V]);
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the digital signature for signing the field. 
        /// </summary>
        /// <value>A <see cref="PdfSignature"/> object specifying the digital signature for signing the field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the first page
        /// PdfPageBase page = doc.Pages[0];
        /// // Gets the signature field                     
        /// PdfLoadedSignatureField signatureField = doc.Form.Fields["ManagerSignature"] as PdfLoadedSignatureField;
        /// // Create a new Signature
        /// PdfCertificate pdfCert = new PdfCertificate("Pdf.pfx", "123");
        /// PdfSignature signature = new PdfSignature(doc, page, pdfCert, "Signature");
        /// signature.Bounds = new RectangleF(new PointF(5, 5), new SizeF(100, 200));   
        /// // Set the signature of the field
        /// signatureField.Signature = signature;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the first page
        /// Dim page As PdfPageBase = doc.Pages(0)
        /// ' Gets the signature field                     
        /// Dim signatureField As PdfLoadedSignatureField = TryCast(doc.Form.Fields("ManagerSignature"), PdfLoadedSignatureField)
        /// ' Create a new Signature
        /// Dim pdfCert As PdfCertificate = New PdfCertificate("Pdf.pfx", "123")
        /// Dim signature As PdfSignature = New PdfSignature(doc, page, pdfCert, "Signature")
        /// signature.Bounds = New RectangleF(New PointF(5, 5), New SizeF(100, 200))
        /// ' Set the signature of the field
        /// signatureField.Signature = signature
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfSignature"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfCertificate"/> Class
        public PdfSignature Signature
        {
            get
            {
                return m_signature;
            }
            set
            {
               // m_signature = new PdfSignature();  
                m_signature = value;
                Changed = true;
            }
        }

        #endregion

#region Implementation
        /// <summary>
        /// Sets the signature properties 
        /// </summary>
        /// <param name="signature"></param>
        private void SetSignature(IPdfPrimitive signature)
        {
            if (signature is PdfReferenceHolder)
            {
                PdfDictionary signatureDictionary = (PdfDictionary)(signature as PdfReferenceHolder).Object;

                m_signature = new PdfSignature();
                if (signatureDictionary != null)
                {
                    if (signatureDictionary.ContainsKey(DictionaryProperties.Reason))
                        m_signature.Reason = (signatureDictionary[DictionaryProperties.Reason] as PdfString).Value;

                    if (signatureDictionary.ContainsKey(DictionaryProperties.Location))
                        m_signature.LocationInfo = (signatureDictionary[DictionaryProperties.Location] as PdfString).Value;

                    if (signatureDictionary.ContainsKey(DictionaryProperties.ContactInfo))
                        m_signature.ContactInfo = (signatureDictionary[DictionaryProperties.ContactInfo] as PdfString).Value;
                }
            }
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();

            if (m_signature != null)
            {
              
                PdfSignatureDictionary sigDic = new PdfSignatureDictionary(CrossTable.Document, m_signature, m_signature.Certificate);
                Dictionary[DictionaryProperties.V] = new PdfReferenceHolder(sigDic);                
            }
        }

        /// <summary>
        /// Creates a copy of PdfLoadedSignatureField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedSignatureField field = new PdfLoadedSignatureField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of loaded item.
        /// </summary>
        /// <param name="dictionary"></param>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            return base.CreateLoadedItem(dictionary);
        }
        #endregion
    }
}
#endif