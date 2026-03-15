#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Implements enumerator to the loaded page collection.
    /// </summary>
    public class PdfLoadedPageEnumerator : IEnumerator
    {
        #region Members
        private PdfLoadedPageCollection m_collection;
        private int m_index = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLoadedPageEnumerator"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public PdfLoadedPageEnumerator(PdfLoadedPageCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            m_collection = collection;
        }
        #endregion

        #region IEnumerator Members
        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <value></value>
        /// <returns>The current element in the collection.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The enumerator is positioned before the first element of the collection
        /// or after the last element. </exception>
        public object Current
        {
            get
            {
                if (m_index < 0 && m_index >= m_collection.Count)
                {
                    throw new InvalidOperationException("The index is out of range.");
                }

                return m_collection[m_index];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. </exception>
        public bool MoveNext()
        {
            ++m_index;

            bool result = !(m_index >= m_collection.Count);
            return result;
        }

        /// <summary>
        /// Sets the enumerator to its initial position,
        /// which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. </exception>
        public void Reset()
        {
            m_index = -1;
        }

        #endregion
    }
}
