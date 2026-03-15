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
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Collection of <see cref="IChartLegend"/> instances.
    /// </summary>
    [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
    [TypeConverter(typeof(CollectionConverter))]
    public class ChartLegendCollection : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Gets or sets <see cref="IChartLegend"/> by the index.
        /// </summary>
        /// <param name="index">Index of <see cref="IChartLegend"/>.</param>
        public IChartLegend this[int index]
        {
            get
            {
                return List[index] as IChartLegend;
            }

            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Gets <see cref="IChartLegend"/> by the name.
        /// </summary>
        /// <param name="name">Name of <see cref="IChartLegend"/>.</param>
        public IChartLegend this[string name]
        {
            get
            {
                IChartLegend result = null;

                foreach (IChartLegend legend in this)
                {
                    if (legend.Name != name)
                    {
                        result = legend;
                    }
                }

                return result;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendCollection"/> class.
        /// </summary>
        public ChartLegendCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds <see cref="IChartLegend"/> to collection.
        /// </summary>
        /// <param name="value">Instance of <see cref="IChartLegend"/> class.</param>
        /// <returns>Index of item.</returns>
        public int Add(IChartLegend value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Determines whether the <see cref="ChartLegendCollection"/> contains a specific item.
        /// </summary>
        /// <param name="value">Instance of <see cref="IChartLegend"/> class.</param>
        /// <returns>true if the <see cref="IChartLegend"/> is found in the collection; otherwise, false.</returns>
        public bool Contains(IChartLegend value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specific <see cref="IChartLegend"/> from the collection.
        /// </summary>
        /// <param name="value">Instance of <see cref="IChartLegend"/> class.</param>
        public void Remove(IChartLegend value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="value">Instance of <see cref="IChartLegend"/> class.</param>
        /// <returns>Index of specified <see cref="IChartLegend"/>.</returns>
        public int IndexOf(IChartLegend value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="IChartLegend"/> should be inserted.</param>
        /// <param name="value">Instance of <see cref="IChartLegend"/> class.</param>
        public void Insert(int index, IChartLegend value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="obj">An <see cref="Object"/> to validation.</param>
        /// <returns>true if specified <see cref="Object"/> is <see cref="IChartLegend"/> and collection doesn't contain it; otherwise, false.</returns>
        protected override bool Validate(object obj)
        {
            IChartLegend legend = obj as IChartLegend;
            return (legend != null) && !this.Contains(legend);
        }
        #endregion
    }
}
