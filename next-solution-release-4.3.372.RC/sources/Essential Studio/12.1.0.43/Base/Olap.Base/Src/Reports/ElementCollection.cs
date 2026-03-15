//-------------------------------------------------------------------------------------------------
// <copyright file="ElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Collection of Filter elements
    /// </summary>
    [Serializable]
    public class ElementCollection : CollectionBase, ICloneable<ElementCollection>
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Collection of Filter elements
    /// </summary>
    [CollectionDataContract]
    public class ElementCollection : Collection<Element>
#endif
    {
        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.Element"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Element this[int index]
        {
            get
            {
                return (Element)base.List[index]; 
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified filter element.
        /// </summary>
        /// <param name="filterElement">The filter element.</param>
        /// <returns>Index of the filtered element.</returns>
        public int Add(Element filterElement)
        {
            return base.List.Add(filterElement);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns><see cref="ElementCollection"/></returns>
        public ElementCollection Clone()
        {
            ElementCollection filterElementCollection = new ElementCollection();
            foreach (Element filterElement in filterElementCollection)
            {
                filterElementCollection.Add(filterElement.Clone());
            }

            return filterElementCollection;
        }

        /// <summary>
        /// Removes the specified filter element.
        /// </summary>
        /// <param name="filterElement">The filter element.</param>
        public void Remove(Element filterElement)
        {
            base.List.Remove(filterElement);
        }
#endif
        #endregion
    }
}
