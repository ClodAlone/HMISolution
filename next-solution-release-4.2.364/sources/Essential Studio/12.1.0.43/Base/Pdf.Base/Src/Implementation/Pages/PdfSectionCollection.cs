#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;


namespace Syncfusion.Pdf
{
    /// <summary>
    /// The collection of the sections.
    /// </summary>
    public class PdfSectionCollection :
        IPdfWrapper,
        IEnumerable
    {
        #region Constants
        /// <summary>
        /// Rotate factor for page rotation.
        /// </summary>
        internal const int RotateFactor = 90;
        #endregion

        #region Fields
        private PdfArray m_sectionCollection;
        private List<PdfSection> m_sections = new List<PdfSection>();
        private PdfDictionary m_pages;
        private PdfNumber m_count;
        private PdfDocument m_document;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:PdfSection"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfSection this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (m_sections[index] as PdfSection);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_sections.Count;
            }
        }

        /// <summary>
        /// Gets a parent document.
        /// </summary>
        internal PdfDocument Document
        {
            get
            {
                return m_document;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSectionCollection"/> class.
        /// </summary>
        internal PdfSectionCollection(PdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
            Initialize();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a section and adds it to the collection.
        /// </summary>
        /// <returns>Created section object.</returns>
        public PdfSection Add()
        {
            PdfSection section = new PdfSection(m_document);
            Add(section);

            return section;
        }

        /// <summary>
        /// Determines the index of the section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>The index of the section.</returns>
        public int IndexOf(PdfSection section)
        {
            PdfReferenceHolder r = new PdfReferenceHolder(section);
            int index = m_sectionCollection.IndexOf(r);
            return index;
        }

        /// <summary>
        /// Inserts the section at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="section">The section.</param>
        public void Insert(int index, PdfSection section)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            PdfReferenceHolder r = CheckSection(section);
            m_sectionCollection.Insert(index, r);
        }

        /// <summary>
        /// Checks whether the collection contains the section.
        /// </summary>
        /// <param name="section">The section object.</param>
        /// <returns>True - if the sections belongs to the collection, False otherwise.</returns>
        public bool Contains(PdfSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            int index = IndexOf(section);

            return (index >= 0);
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new PdfSectionEnumerator(this);
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_pages;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Informs the section collection that the page labels were set.
        /// </summary>
        internal void PageLabelsSet()
        {
            Document.PageLabelsSet();
        }

        /// <summary>
        /// Resets the progress.
        /// </summary>
        internal void ResetProgress()
        {
            foreach (PdfSection section in this)
            {
                section.ResetProgress();
            }
        }

        /// <summary>
        /// Sets the progress.
        /// </summary>
        internal void SetProgress()
        {
            foreach (PdfSection section in this)
            {
                section.SetProgress();
            }
        }

        /// <summary>
        /// Called when a page is saving.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void OnPageSaving(PdfPage page)
        {
            Document.OnPageSave(page);
        }

        /// <summary>
        /// Infills dictionary by the data from Page settings.
        /// </summary>
        /// <param name="container">Pdf container of the data.</param>
        /// <param name="pageSettings">Page settings.</param>
        private void SetPageSettings(PdfDictionary container, PdfPageSettings pageSettings)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (pageSettings == null)
                throw new ArgumentNullException("pageSettings");

            RectangleF bounds = new RectangleF(PointF.Empty, pageSettings.Size);

            //With PDF/X, We apply trim and media box at the page dictionary.
            if (PdfDocument.ConformanceLevel != PdfConformanceLevel.Pdf_X1A2001)
                container[DictionaryProperties.MediaBox] = PdfArray.FromRectangle(bounds);

            if (pageSettings.Rotate != PdfPageRotateAngle.RotateAngle0)
            {
                int rotate = RotateFactor * (int)pageSettings.Rotate;
                PdfNumber angle = new PdfNumber(rotate);
                container[DictionaryProperties.Rotate] = angle;
            }

            if (pageSettings.Unit != PdfGraphicsUnit.Point)
            {
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                float unit = convertor.ConvertUnits(1f, pageSettings.Unit, PdfGraphicsUnit.Point);
                container[DictionaryProperties.UserUnit] = new PdfNumber(unit);
            }
        }

        /// <summary>
        /// Checks if the section is within the collection.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>The reference holder of the section.</returns>
        /// <exception cref="T:ArgumentException">Throws ArgumentException 
        /// if the section is within the collection.</exception>
        private PdfReferenceHolder CheckSection(PdfSection section)
        {
            PdfReferenceHolder r = new PdfReferenceHolder(section);
            bool contains = m_sectionCollection.Contains(r);

            if (contains)
                throw new ArgumentException("The object can't be added twice to the collection.", "section");

            return r;
        }

        /// <summary>
        /// Counts the pages.
        /// </summary>
        /// <returns></returns>
        private int CountPages()
        {
            int count = 0;

            foreach (PdfSection s in this)
            {
                count += s.Count;
            }

            return count;
        }

        /// <summary>
        /// Adds the specified section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>Index of the section in the collection.</returns>
        private int Add(PdfSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            PdfReferenceHolder r = CheckSection(section);
            m_sections.Add(section);
            section.Parent = this;
            m_sectionCollection.Add(r);

            return m_sections.IndexOf(section);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Initialize()
        {
            m_count = new PdfNumber(0);
            m_sectionCollection = new PdfArray();
            m_pages = new PdfDictionary();
            m_pages.BeginSave += new SavePdfPrimitiveEventHandler(BeginSave);
            m_pages[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Pages);
            m_pages[DictionaryProperties.Kids] = m_sectionCollection;
            m_pages[DictionaryProperties.Count] = m_count;
            m_pages[DictionaryProperties.Resources] = new PdfDictionary();

            SetPageSettings(m_pages, m_document.PageSettings);
        }

        /// <summary>
        /// Clears section collection of the document.
        /// </summary>
        internal void Clear()
        {
            foreach(PdfSection section in this)
                section.Clear();

            if (m_pages != null)
                m_pages.Clear();

            if (m_sectionCollection != null)
                m_sectionCollection.Clear();

            if (m_sections != null)
                m_sections.Clear();

            m_pages = null;
            m_sectionCollection = null;
            m_sections = null;
            m_document = null;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Catches the Save event of the dictionary.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BeginSave(object sender, SavePdfPrimitiveEventArgs e)
        {
            m_count.IntValue = CountPages();

            SetPageSettings(m_pages, m_document.PageSettings);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Section collection enumerator.
        /// </summary>
        private struct PdfSectionEnumerator : IEnumerator
        {
            #region Fields
            private PdfSectionCollection m_sectionCollection;
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Enumerator"/> class.
            /// </summary>
            /// <param name="sectionCollection">The section collection.</param>
            internal PdfSectionEnumerator(PdfSectionCollection sectionCollection)
            {
                if (sectionCollection == null)
                    throw new ArgumentNullException("sectionCollection");

                m_sectionCollection = sectionCollection;
                m_currentIndex = -1;
            }
            #endregion

            #region IEnumerator Members
            /// <summary>
            /// Gets the current section.
            /// </summary>
            public object Current
            {
                get
                {
                    CheckIndex();
                    return m_sectionCollection[m_currentIndex];
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element;
            /// false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                ++m_currentIndex;

                return (m_currentIndex < m_sectionCollection.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_currentIndex = -1;
            }
            #endregion

            #region Helper methods
            /// <summary>
            /// Checks the index.
            /// </summary>
            private void CheckIndex()
            {
                if (m_currentIndex < 0 || m_currentIndex >= m_sectionCollection.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
