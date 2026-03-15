#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;

using Syncfusion.DocIO.DLS.XML;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// This class implements the Collection interface.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class CollectionImpl : OwnerHolder
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private List<Object> m_innerList = new List<Object>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_innerList.Count;
            }
        }
        /// <summary>
        /// Gets the inner list.
        /// </summary>
        /// <value>The inner list.</value>
        internal IList InnerList
        {
            get
            {
                return m_innerList;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionImpl"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        protected CollectionImpl(WordDocument doc, OwnerHolder owner)
            : base(doc, owner)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return m_innerList.GetEnumerator();
        }
        #endregion
    }
}
