#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Manipulates pages within a section.
    /// </summary>
    public class PdfSectionPageCollection : IEnumerable
    {
        #region Fields
        private PdfSection m_section;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:PdfPage"/> at the specified index.
        /// </summary>
        public PdfPage this[int index]
        {
            get
            {
                if (index < 0 && index > Count)
                    throw new ArgumentOutOfRangeException("index");

                return m_section[index];
            }
        }

        /// <summary>
        /// Gets the count of the pages.
        /// </summary>
        public int Count
        {
            get
            {
                return m_section.Count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSectionPageCollection"/> class.
        /// </summary>
        private PdfSectionPageCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSectionPageCollection"/> class.
        /// </summary>
        /// <param name="section">The section.</param>
        internal PdfSectionPageCollection(PdfSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            m_section = section;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new page and adds it into the collection.
        /// </summary>
        /// <returns>The new page.</returns>
        public PdfPage Add()
        {
            return m_section.Add();
        }

        /// <summary>
        /// Adds a page into collection.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Add(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_section.Add(page);
        }

        /// <summary>
        /// Inserts a page at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="page">The page.</param>
        public void Insert(int index, PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (index < 0 && index > Count)
                throw new ArgumentOutOfRangeException("index");

            m_section.Insert(index, page);
        }

        /// <summary>
        /// Returns the index of the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The index of the page.</returns>
        public int IndexOf(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            return m_section.IndexOf(page);
        }

        /// <summary>
        /// Determines whether the specified page is within the collection.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// 	<c>true</c> if the collection contains the specified page; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            return m_section.Contains(page);
        }

        /// <summary>
        /// Removes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Remove(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_section.Remove(page);
        }

        /// <summary>
        /// Removes a page at the index specified.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 && index > Count)
                throw new ArgumentOutOfRangeException("index");

            m_section.RemoveAt(index);
        }

        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear()
        {
            m_section = null;
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object
        /// that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_section).GetEnumerator();
        }
        #endregion
    }
}
