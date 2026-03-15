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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// Collection of Filter elements
    /// </summary>
    [CollectionDataContract]
    public class ElementCollection : Collection<Element>
    {
        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.OlapSilverlight.Base.Report.FilterElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Element this[int index]
        {
            get
            {
                return (Element)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified filter element.
        /// </summary>
        /// <param name="filterElement">The filter element.</param>
        /// <returns>index of the filterelement</returns>
        public void Add(Element filterElement)
        {
            base.Items.Add(filterElement);
        }

        /// <summary>
        /// Removes the specified filter element.
        /// </summary>
        /// <param name="filterElement">The filter element.</param>
        public void Remove(Element filterElement)
        {
            base.Items.Remove(filterElement);
        }
        #endregion
    }
}
