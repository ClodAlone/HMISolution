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
    /// A collection of stamps that are applied to the page templates.
    /// </summary>
    public class PdfStampCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets a stamp element by its index.
        /// </summary>
        public PdfPageTemplateElement this[int index]
        {
            get
            {
                PdfPageTemplateElement template = List[index] as PdfPageTemplateElement;
                return template;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new stamp collection.
        /// </summary>
        public PdfStampCollection()
            : base()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a stamp element to the collection.
        /// </summary>
        /// <param name="template">The stamp element.</param>
        /// <returns>The index of the stamp element.</returns>
        public int Add(PdfPageTemplateElement template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            int index = List.Add(template);

            return index;
        }

        /// <summary>
        /// Creates a stamp element and adds it to the collection.
        /// </summary>
        /// <param name="x">X co-ordinate of the stamp.</param>
        /// <param name="y">Y co-ordinate of the stamp.</param>
        /// <param name="width">Width of the stamp.</param>
        /// <param name="height">Height of the stamp.</param>
        /// <returns>The created stamp element.</returns>
        public PdfPageTemplateElement Add(float x, float y, float width, float height)
        {
            PdfPageTemplateElement template = new PdfPageTemplateElement(x, y, width, height);

            Add(template);

            return template;
        }

        /// <summary>
        /// Checks whether the stamp element exists in the collection.
        /// </summary>
        /// <param name="template">Stamp element.</param>
        /// <returns>True - if stamp element exists in the collection, False otherwise.</returns>
        public bool Contains(PdfPageTemplateElement template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            bool result = List.Contains(template);

            return result;
        }
        /// <summary>
        /// Inserts a stamp element to the collection at the specified position.
        /// </summary>
        /// <param name="index">The index of the stamp in the collection.</param>
        /// <param name="template">The stamp element.</param>
        public void Insert(int index, PdfPageTemplateElement template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            List.Insert(index, template);
        }
        /// <summary>
        /// Removes the stamp element from the collection.
        /// </summary>
        /// <param name="template">The stamp element.</param>
        public void Remove(PdfPageTemplateElement template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            List.Remove(template);
        }
        /// <summary>
        /// Removes a stamp element from the specified position in the collection.
        /// </summary>
        /// <param name="index">The index of the stamp in the collection.</param>
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }
        /// <summary>
        /// Cleares the collection.
        /// </summary>
        public void Clear()
        {
            List.Clear();
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        new public IEnumerator GetEnumerator()
        {
            return new PdfPageTemplateEnumerator(this);
        }
        #endregion

        #region Internals
        /// <summary>
        /// PdfPageTemplate enumerator.
        /// </summary>
        private struct PdfPageTemplateEnumerator : IEnumerator
        {
            #region Fields
            /// <summary>
            /// Stamps collection
            /// </summary>
            private PdfStampCollection m_stamps;
            /// <summary>
            /// Current index of the enumerator.
            /// </summary>
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Enumerator"/> class.
            /// </summary>
            /// <param name="stamps">The stanps collection.</param>
            internal PdfPageTemplateEnumerator(PdfStampCollection stamps)
            {
                if (stamps == null)
                    throw new ArgumentNullException("stamps");

                m_stamps = stamps;
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
                    return m_stamps[m_currentIndex];
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

                return (m_currentIndex < m_stamps.Count);
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
                if (m_currentIndex < 0 || m_currentIndex >= m_stamps.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
