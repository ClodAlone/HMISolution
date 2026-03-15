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

using System;
using System.Collections;
using System.ComponentModel;


namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// A collection of <see cref="ChartLegendItem"/>s.
    /// </summary>
    [TypeConverter(typeof(CollectionConverter))]
    public class ChartLegendItemsCollection : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Looks up the collection and returns the legend item stored in the specified index.
        /// </summary>
        public ChartLegendItem this[int index]
        {
            get
            {
                return List[index] as ChartLegendItem;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendItemsCollection"/> class.
        /// </summary>
        public ChartLegendItemsCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified legend item to the collection.
        /// </summary>
        /// <param name="value">The item to add.</param>
        /// <returns> The position into which the new element was inserted.</returns>
        public int Add(ChartLegendItem value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Adds item array to the collection.
        /// </summary>
        /// <param name="range">The array of items to add.</param>
        public void AddRange(ChartLegendItem[] range)
        {
            foreach (ChartLegendItem item in range)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Removes the specified legend item from the collection.
        /// </summary>
        /// <param name="value">
        /// The legend item to be removed.
        /// </param>
        public void Remove(ChartLegendItem value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Inserts the specified legend item in the specified index.
        /// </summary>
        /// <param name="index">
        /// The index value where the legend item is to be inserted.
        /// </param>
        /// <param name="value">
        /// The legend item that is to be inserted.
        /// </param>
        public void Insert(int index, ChartLegendItem value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Returns the index value of the specified legend item.
        /// </summary>
        /// <param name="value">The legend item to look for.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(ChartLegendItem value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Copies the elements of the array.
        /// </summary>
        /// <returns>Returns array of ChartLegendItem.</returns>
        new public ChartLegendItem[] ToArray()
        {
            return this.ToArray(typeof(ChartLegendItem)) as ChartLegendItem[];
        }

        /// <summary>
        /// Indicates whether the specified item is in the list.
        /// </summary>
        /// <param name="value">The System.Object to locate in the System.Collections.IList.</param>
        /// <returns>true if the ChartLegendItem is found in the List; otherwise, false.</returns>
        public bool Contains(ChartLegendItem value)
        {
            return List.Contains(value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs additional custom processes when validating a value
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>If is true, value is approved.</returns>
        protected override bool Validate(object obj)
        {
            return obj != null;
        }
        #endregion
    }
}