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

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Collection of Measure elements.
    /// </summary>
    [Serializable]
    public class MeasureElementCollection : CollectionBase, ICloneable<MeasureElementCollection>
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Collection of Measure elements
    /// </summary>
    [CollectionDataContract]
    public class MeasureElementCollection : Collection<MeasureElement>
#endif
    {
        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.MeasureElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public MeasureElement this[int index]
        {
            get
            {
                return (MeasureElement)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified measure element.
        /// </summary>
        /// <param name="measureElement">The measure element.</param>
        /// <returns>returns the index of the MeasureElement</returns>
        public int Add(MeasureElement measureElement)
        {
            return base.List.Add(measureElement);
        }

        /// <summary>
        /// Removes the specified measure element.
        /// </summary>
        /// <param name="measureElement">The measure element.</param>
        public void Remove(MeasureElement measureElement)
        {
            base.List.Remove(measureElement);
        }
#endif

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MeasureElementCollection"/>.</returns>
        public MeasureElementCollection Clone()
        {
            MeasureElementCollection measureElementCollection = new MeasureElementCollection();
            foreach (MeasureElement measureElement in measureElementCollection)
            {
                measureElementCollection.Add(measureElement.Clone());
            }

            return measureElementCollection;
        }

        /// <summary>
        /// Finds the name of the measure by.
        /// </summary>
        /// <param name="measureName">Name of the measure.</param>
        /// <returns>Measure Element if found</returns>
        public MeasureElement FindMeasureByName(string measureName)
        {
#if !SILVERLIGHT
            foreach (MeasureElement measureElement in base.List)
#else
            foreach (MeasureElement measureElement in base.Items)
#endif
            {
                if (measureElement.UniqueName.ToLower() == measureName.ToLower())
                {
                    return measureElement;
                }
            }
            return null;
        }
        #endregion
    }
}
