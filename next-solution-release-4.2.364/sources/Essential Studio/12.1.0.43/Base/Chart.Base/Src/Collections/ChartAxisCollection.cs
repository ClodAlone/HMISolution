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
using System.Drawing;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Collection of <see cref="ChartAxis">ChartAxis</see> instances.
    /// </summary>
    public class ChartAxisCollection : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Gets or sets <see cref="Syncfusion.Windows.Forms.Chart.ChartAxis"/> by index in collection.
        /// </summary>
        /// <value>The Chart Axis Object to add to the Chart Axis collection.</value>
        public ChartAxis this[int index]
        {
            get
            {
                return List[index] as ChartAxis;
            }

            set
            {
                List[index] = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisCollection"/> class.
        /// </summary>
        public ChartAxisCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The position into which the new axis element was inserted.</returns>
        public int Add(ChartAxis value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Adds the array of <see cref="ChartAxis"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Returns the count of axis collection.</returns>
        public int AddRange(ChartAxis[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                this.List.Add(values[i]);
            }

            return this.List.Count - 1;
        }

        /// <summary>
        /// Determines whether collection contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if collection contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartAxis value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(ChartAxis value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(ChartAxis value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, ChartAxis value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Performs additional custom processes when validating a value.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>If is true, value is approved.</returns>
        protected override bool Validate(object obj)
        {
            return (obj != null) && !this.List.Contains(obj);
        }
        #endregion
    }
}