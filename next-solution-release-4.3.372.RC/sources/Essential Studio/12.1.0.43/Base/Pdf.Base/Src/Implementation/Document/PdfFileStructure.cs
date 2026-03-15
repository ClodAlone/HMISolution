#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.ComponentModel;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// This class represents a set of the properties that define the internal structure of PDF file.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Set the document`s cross reference type
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
    /// ' Set the document`s cross reference type
    /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream
    /// ' Set the pdf version
    /// doc.FileStructure.Version = PdfVersion.Version1_6
    /// ' Set the pdf version
    /// doc.Save("FileStructure.pdf")
    /// </code>
    /// </example>
    public class PdfFileStructure
    {
        #region Fields
        /// <summary>
        /// the version of the file.
        /// </summary>
        private PdfVersion m_version;

        /// <summary>
        /// The type of the cross-refrence.
        /// </summary>
        private PdfCrossReferenceType m_crossReferenceType;

        /// <summary>
        /// Indicates the file format.
        /// </summary>
        private PdfFileFormat m_fileformat;

        /// <summary>
        /// Incremental operator bool value
        /// </summary>
        private bool m_incrementalUpdate;

        /// <summary>
        /// Tagged bool value
        /// </summary>
        private bool m_taggedPdf;

        /// <summary>
        /// Notifies if TaggedPDF property is changed.
        /// </summary>
        internal event EventHandler TaggedPdfChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileStructure"/> class.
        /// </summary>    
        public PdfFileStructure()
        {
#if DEBUG
            m_version = PdfVersion.Version1_4;
            m_crossReferenceType = PdfCrossReferenceType.CrossReferenceTable;
#else
			m_version = PdfVersion.Version1_5;
			m_crossReferenceType = PdfCrossReferenceType.CrossReferenceStream;
#endif
            m_fileformat = PdfFileFormat.Plain;
            m_incrementalUpdate = true;
            m_taggedPdf = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the version of the PDF document.
        /// </summary>
        /// <value>The document version.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();    
        /// // Set the pdf version
        /// doc.FileStructure.Version = PdfVersion.Version1_6;
        /// // Save the document
        /// doc.Save("FileStructure.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the pdf version
        /// doc.FileStructure.Version = PdfVersion.Version1_6
        /// ' Save the document
        /// doc.Save("FileStructure.pdf")
        /// </code>
        /// </example>
        public PdfVersion Version
        {
            get
            {
                return m_version;
            }

            set
            {
                m_version = value;
                if (m_version <= PdfVersion.Version1_3)
                {
                    m_crossReferenceType = PdfCrossReferenceType.CrossReferenceTable;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [incremental update].
        /// </summary>
        /// <value><c>true</c> if [incremental update]; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// // Load an existing document
        /// PdfLoadedDocument lDoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// // Sets the incremental update as True
        /// lDoc.FileStructure.IncrementalUpdate = true;
        /// //Creates a new page and adds it as the last page of the document
        /// lDoc.Pages.Add();
        /// // Saves the document
        /// lDoc.Save("FileStructure.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Load an existing document
        /// Dim lDoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// ' Sets the incremental update as True
        /// lDoc.FileStructure.IncrementalUpdate = True
        /// ' Create a page
        /// lDoc.Pages.Add()
        /// ' Saves the document
        /// lDoc.Save("FileStructure.pdf")
        /// </code>
        /// </example>
        public bool IncrementalUpdate
        {
            get
            {
                return m_incrementalUpdate;
            }

            set
            {
                m_incrementalUpdate = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of PDF cross-reference.
        /// </summary>
        /// <remarks>Please see the description of <see cref="PdfCrossReferenceType"/> for more details.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the cross reference type
        /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream;  
        /// // Save the document
        /// doc.Save("FileStructure.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the cross reference type
        /// doc.FileStructure.CrossReferenceType = PdfCrossReferenceType.CrossReferenceStream    
        /// ' Save the document
        /// doc.Save("FileStructure.pdf")
        /// </code>
        /// </example>
        public PdfCrossReferenceType CrossReferenceType
        {
            get
            {
                return m_crossReferenceType;
            }

            set
            {
                m_crossReferenceType = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating the format of the file.
        /// </summary>
        internal PdfFileFormat FileFormat
        {
            get
            {
                return m_fileformat;
            }

            set
            {
                m_fileformat = value;
            }
        }

        /// <summary>
        /// Gets the value indicating whether the PDF document is tagged one or not.
        /// </summary>
        /// <value>If true PDF document is tagged, otherwise false.</value>
        public bool TaggedPdf
        {
            get
            {
                return m_taggedPdf;
            }
            internal set
            {
                if (m_taggedPdf != value)
                    m_taggedPdf = value;

                OnTaggedPdfChanged(new EventArgs());
            }
        }
        #endregion

        // Create the OnPropertyChanged method to raise the event
        protected void OnTaggedPdfChanged(EventArgs e)
        {
            EventHandler handler = TaggedPdfChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
