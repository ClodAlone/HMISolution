#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Base collection of the pdf objects.
    /// </summary>
    public class PdfCollection : IEnumerable
    {
        #region Fields
        /// <summary>
        /// List of the collection.
        /// </summary>
        private List<object> m_list;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCollection"/> class.
        /// </summary>
        public PdfCollection()
        {
            m_list = new List<object>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets number of the elements in the collection.
        /// </summary>
        /// <value>The total number of elements in the collection.</value>
        public int Count
        {
            get
            {
                return m_list.Count;
            }
        }

        /// <summary>
        /// Gets internal list of the collection.
        /// </summary>      
        protected IList List
        {
            get
            {
                return m_list;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        internal void CopyTo(IPdfWrapper[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            m_list.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>Returns an enumerator that iterates through a collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
        #endregion
    }
}
