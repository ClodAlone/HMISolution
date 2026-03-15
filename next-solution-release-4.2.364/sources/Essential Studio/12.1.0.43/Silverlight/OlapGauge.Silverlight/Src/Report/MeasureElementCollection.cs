//-------------------------------------------------------------------------------------------------
// <copyright file="MeasureElementCollection.cs" company="syncfusion">
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
    /// Collection of Measure elements
    /// </summary>
    [CollectionDataContract]
    public class MeasureElementCollection : Collection<MeasureElement>
    {
        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.OlapSilverlight.Base.Report.MeasureElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public MeasureElement this[int index]
        {
            get
            {
                return (MeasureElement)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified measure element.
        /// </summary>
        /// <param name="measureElement">The measure element.</param>
        /// <returns>returns the index of the MeasureElement</returns>
        public void Add(MeasureElement measureElement)
        {
            base.Items.Add(measureElement);
        }

        /// <summary>
        /// Removes the specified measure element.
        /// </summary>
        /// <param name="measureElement">The measure element.</param>
        public void Remove(MeasureElement measureElement)
        {
            base.Items.Remove(measureElement);
        }
        #endregion
    }
}
