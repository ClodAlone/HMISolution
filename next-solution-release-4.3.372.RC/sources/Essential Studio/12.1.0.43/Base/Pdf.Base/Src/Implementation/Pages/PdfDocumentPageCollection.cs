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
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Implements a virtual collection of all pages in the document.
    /// </summary>
    public class PdfDocumentPageCollection : IEnumerable
    {
        #region Fields
        /// <summary>
        /// Parent document.
        /// </summary>
        private PdfDocument m_document;
        /// <summary>
        /// It holds the page collection with the index
        /// </summary>
        private Dictionary<PdfPage, int> m_pageCollectionIndex = new Dictionary<PdfPage, int>();
        /// <summary>
        /// It counts the index of the page
        /// </summary>
        internal int count = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the total number of the pages.
        /// </summary>
        public int Count
        {
            get
            {
                return CountPages();
            }
        }

        /// <summary>
        /// Gets a page by its index in the document.
        /// </summary>
        public PdfPage this[int index]
        {
            get
            {
                PdfPage page = GetPageByIndex(index);
                return page;
            }
        }

        /// <summary>
        /// Gets a page index from the document.
        /// </summary>
        internal Dictionary<PdfPage, int> PageCollectionIndex
        {
            get
            {
                return m_pageCollectionIndex;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Represents the  method that executes on a PdfDocument when a new page is created.
        /// </summary>
        public event PageAddedEventHandler PageAdded;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageCollection"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        internal PdfDocumentPageCollection(PdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a page and adds it to the last section in the document.
        /// </summary>
        /// <returns>Created page object.</returns>
        public PdfPage Add()
        {
            PdfPage page = new PdfPage();
            Add(page);

            return page;
        }

        /// <summary>
        /// Adds the specified page to the last section.
        /// </summary>
        /// <param name="page">The page.</param>i
        internal void Add(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfSection section = GetLastSection();
            if (GetLastSection().PageSettings.Orientation != this.m_document.PageSettings.Orientation)
            {
                section = this.m_document.Sections.Add();
                section.PageSettings.Orientation = this.m_document.PageSettings.Orientation;
            }
            if (!m_pageCollectionIndex.ContainsKey(page))
                m_pageCollectionIndex.Add(page, count++);
            section.Add(page);
        }

        /// <summary>
        /// Inserts a page at the specified index to the last section in the document.
        /// </summary>
        /// <param name="index">The index of the page in the section.</param>
        /// <param name="page">The page.</param>
        public void Insert(int index, PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            if (index > Count)
                throw new ArgumentOutOfRangeException("index", "Value can not be less 0, equal or more than number of pages in the document.");

            if (index == Count)
            {
                PdfSection section = GetLastSection();
                section.Add(page);
                return;
            }
            int numPages = 0;
            for (int i = 0, len = m_document.Sections.Count; i < len; i++)
            {
                PdfSection section = m_document.Sections[i];

                for (int j = 0; j < section.Pages.Count; j++)
                {
                    if (numPages == index)
                    {
                        section.Insert(j, page);
                        return;
                    }
                    else
                    {
                        numPages++;
                    }
                }
            }


        }

        /// <summary>
        /// Gets the index of the page in the document.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <returns>Index of the page in the document if exists, -1 otherwise.</returns>
        public int IndexOf(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            int index = -1;
            int numPages = 0;

            for (int i = 0, len = m_document.Sections.Count; i < len; i++)
            {
                PdfSection section = m_document.Sections[i];

                index = section.IndexOf(page);

                if (index >= 0)
                {
                    index += numPages;
                    break;
                }

                numPages += section.Count;
            }

            return index;
        }

        /// <summary>
        /// Removes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal PdfSection Remove(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            
            PdfSection section = null;
            for (int i = 0, len = m_document.Sections.Count; i < len; i++)
            {
                section = m_document.Sections[i];
                if (section.Pages.Contains(page))
                {
                    section.Pages.Remove(page);
                    break;
                }
            }

            return section;
        }

        /// <summary>
        /// Clears the page collection.
        /// </summary>
        internal void Clear()
        {
            foreach (PdfPage page in this)
            {
                Remove(page);
                m_pageCollectionIndex.Remove(page);
                page.Clear();
            }

            m_pageCollectionIndex.Clear();
            m_pageCollectionIndex = null;
            m_document = null;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Counts the pages.
        /// </summary>
        /// <returns>The total number of pages.</returns>
        private int CountPages()
        {
            PdfSectionCollection sc = m_document.Sections;
            int count = 0;

            foreach (PdfSection section in sc)
            {
                count += section.Count;
            }

            return count;
        }

        /// <summary>
        /// Searches a page by its index in the document.
        /// </summary>
        /// <param name="index">Zero-based index of the page.</param>
        /// <returns>Page by its index in the document.</returns>
        private PdfPage GetPageByIndex(int index)
        {
            if ((index < 0) || (index >= Count))
                throw new ArgumentOutOfRangeException("index", "Value can not be less 0, equal or more than number of pages in the document.");

            PdfPage page = null;
            int sectionStartIndex = 0;
            int sectionCount = 0;
            int pageIndex = 0;

            for (int i = 0, len = m_document.Sections.Count; i < len; i++)
            {
                PdfSection section = m_document.Sections[i];

                sectionCount = section.Count;

                pageIndex = index - sectionStartIndex;

                // We found a section containing the page.
                if ((index >= sectionStartIndex && pageIndex < sectionCount))
                {
                    page = section[pageIndex];
                    break;
                }
                sectionStartIndex += sectionCount;
            }

            return page;
        }

        /// <summary>
        /// Adds a loaded page to the last section in the document.
        /// </summary>
        /// <param name="page">The loaded page.</param>
        private void Add(PdfLoadedPage page)
        {
            throw new NotImplementedException();
            // TODO: Implement here.
        }

        /// <summary>
        /// Returns last section in the document.
        /// </summary>
        /// <returns>Returns last section in the document.</returns>
        private PdfSection GetLastSection()
        {
            PdfSectionCollection sc = m_document.Sections;

            if (sc.Count == 0)
            {
                sc.Add();
            }

            PdfSection section = sc[sc.Count - 1];
            return section;
        }

        /// <summary>
        /// Called when new page has been added
        /// </summary>
        /// <param name="args">Event arguments.</param>
        internal void OnPageAdded(PageAddedEventArgs args)
        {
            if (PageAdded != null)
            {
                PageAdded(this, args);
            }
        }

        /// <summary>
        /// Adds a cloned page from a loaded document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <param name="destinations">The destinations.</param>
        /// <returns></returns>
        internal PdfPageBase Add(PdfLoadedDocument ldDoc, PdfPageBase page, List<PdfArray> destinations)
        {
            if (ldDoc == null)
                throw new ArgumentNullException("ldDoc");

            if (page == null)
                throw new ArgumentNullException("page");

            // Get parameters of the page and determine if it can fit the last section.
            bool fitLastSection = CanPageFitLastSection(page);
            // Obtain the concatenated content streams as XObject.
            //PdfTemplate xobject = page.GetContent();

            PdfSection sec;
            // Create a new page and draw the XObject there.
            if (fitLastSection)
            {
                sec = GetLastSection();
            }
            else
            {
                sec = m_document.Sections.Add();

                PdfPageSettings ps = sec.PageSettings;

                ps.Size = page.Size;
                ps.Orientation = page.Orientation;
                ps.Rotate = page.Rotation;
                ps.Margins.All = 0;
                ps.Origin = page.Origin;
            }

            PdfPage newPage = sec.Add();
            m_pageCollectionIndex.Add(newPage, count++);
            //Set CropBox 
            PdfArray array = page.Dictionary[DictionaryProperties.CropBox] as PdfArray;

            if (array != null)
            {
                newPage.Dictionary.SetProperty(DictionaryProperties.CropBox, array);
            }

            SizeF size = newPage.Size;

            if (page.Dictionary.ContainsKey(DictionaryProperties.MediaBox))
            {
                //Set MedixBox 
                array = page.Dictionary[DictionaryProperties.MediaBox] as PdfArray;

                if (array != null)
                {
                    newPage.Dictionary.SetProperty(DictionaryProperties.MediaBox, array);
                    float width = (array[2] as PdfNumber).FloatValue;
                    float height = (array[3] as PdfNumber).FloatValue;
                    size = new SizeF(width, height);
                }
            }
          
            //// Obtain the concatenated content streams as XObject.
            //PdfTemplate xobject = page.ContentTemplate;

            //PdfStream contentStream = xobject.m_content;

            //if (contentStream.Data.Length > 0)
            //{
            //    if (xobject != null)
            //    {
            //        newPage.Graphics.DrawPdfTemplate(xobject, PointF.Empty, size.Height == 0 ? xobject.Size : size);
            //    }
            //}
            //else
            //{
                if (page.Contents.Count > 0)
                {
                    IPdfPrimitive content = null;
                    PdfResources resources = null;

                    foreach (IPdfPrimitive element in page.Contents)
                    {
                        if (m_document.EnableMemoryOptimization)
                            content = element.Clone(m_document.CrossTable);
                        else
                            content = element;
                        newPage.Contents.Add(content);
                    }
                    if (m_document.EnableMemoryOptimization)
                        resources = new PdfResources(page.GetResources().Clone(m_document.CrossTable) as PdfDictionary);
                    else
                        resources = page.GetResources();
                    newPage.Dictionary[DictionaryProperties.Resources] = resources;
                    newPage.SetResources(resources);
                }
                //else
                //{
                //    newPage.Graphics.DrawPdfTemplate(xobject, PointF.Empty, size.Height == 0 ? xobject.Size : size);
                //}
            //}          


            // Copy annotation dictionaries to the new page.
            if (!m_document.EnableMemoryOptimization)
                newPage.ImportAnnotations(ldDoc, page, destinations);
            return newPage;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Determines whether the page fit last section.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// 	<c>true</c> if the page fit last section; otherwise, <c>false</c>.
        /// </returns>
        private bool CanPageFitLastSection(PdfPageBase page)
        {
            // TODO: add real code.
            return false;
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
            return new PdfPageEnumerator(this);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Section collection enumerator.
        /// </summary>
        private struct PdfPageEnumerator : IEnumerator
        {
            #region Fields
            /// <summary>
            /// Parent page collection.
            /// </summary>
            private PdfDocumentPageCollection m_pageCollection;
            /// <summary>
            /// Current index of the enumerator.
            /// </summary>
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Enumerator"/> class.
            /// </summary>
            /// <param name="pageCollection">The section collection.</param>
            internal PdfPageEnumerator(PdfDocumentPageCollection pageCollection)
            {
                if (pageCollection == null)
                    throw new ArgumentNullException("pageCollection");

                m_pageCollection = pageCollection;
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
                    return m_pageCollection[m_currentIndex];
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

                return (m_currentIndex < m_pageCollection.Count);
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
                if (m_currentIndex < 0 || m_currentIndex >= m_pageCollection.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
