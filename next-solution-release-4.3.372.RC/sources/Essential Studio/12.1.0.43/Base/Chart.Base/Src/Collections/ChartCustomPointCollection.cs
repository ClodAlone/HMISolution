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
using System.ComponentModel;
using Syncfusion.Documentation;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Collection of custom points that are registered for display.
    /// Custom points can be tied to specific positions on the chart or to specific points on any series.
    /// <seealso cref="ChartCustomPoint"/>
    /// </summary>
    [TypeConverter(typeof(CollectionConverter))]
    public class ChartCustomPointCollection : ChartBaseList
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Chart.ChartCustomPoint"/> at the specified index.
        /// </summary>                
        /// <value>Add the ChartCustomPoint.</value>        
        public ChartCustomPoint this[int index]
        {
            get
            {
                return List[index] as ChartCustomPoint;
            }

            set
            {
                List[index] = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(ChartCustomPoint value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Determines whether collection contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if collection contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartCustomPoint value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(ChartCustomPoint value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(ChartCustomPoint value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, ChartCustomPoint value)
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
            return (obj != null) && (obj is ChartCustomPoint) && !this.List.Contains(obj);
        }
        #endregion
    }
}