#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    #region ChartLegendsListEditor
    /// <summary>
    /// Represents the GUI editor for <see cref="ChartLegendsList"/>.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    class ChartLegendsListEditor : CollectionEditor
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendsListEditor"/> class.
        /// </summary>
        public ChartLegendsListEditor()
            : base(typeof(ChartLegendsList))
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether original members of the collection can be removed.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>
        /// true if it is permissible to remove this value from the collection; otherwise, false. The default implementation always returns true.
        /// </returns>
        protected override bool CanRemoveInstance(object value)
        {
            ChartLegend legend = value as ChartLegend;

            return legend.Name != ChartLegend.DefaultName;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// A collection of <see cref="ChartLegend"/> items.
    /// </summary>
    [Editor(typeof(ChartLegendsListEditor), typeof(UITypeEditor))]
    public class ChartLegendsList : ChartBaseList
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Chart.ChartLegend"/> at the specified index.
        /// </summary>
        /// <value>The ChartLegends Indexer.</value>
        public ChartLegend this[int index]
        {
            get
            {
                return List[index] as ChartLegend;
            }

            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Gets <see cref="ChartLegend"/> by the name.
        /// </summary>
        /// <value></value>
        public ChartLegend this[string name]
        {
            get
            {
                ChartLegend result = null;

                foreach (ChartLegend legend in this)
                {
                    if (legend.Name == name)
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
        /// Initializes a new instance of the <see cref="ChartLegendsList"/> class.
        /// </summary>
        public ChartLegendsList()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified <see cref="ChartLegend"/>.
        /// </summary>
        /// <param name="value">The <see cref="ChartLegend"/>.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(ChartLegend value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Determines whether collection contains the specified <see cref="ChartLegend"/>.
        /// </summary>
        /// <param name="value">The <see cref="ChartLegend"/>.</param>
        /// <returns>
        ///     <c>true</c> if collection contains the specified <see cref="ChartLegend"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartLegend value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified <see cref="ChartLegend"/>.
        /// </summary>
        /// <param name="value">The <see cref="ChartLegend"/>.</param>
        public void Remove(ChartLegend value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Returns index of specified <see cref="ChartLegend"/>
        /// </summary>
        /// <param name="value">The <see cref="ChartLegend"/>.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(ChartLegend value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts <see cref="ChartLegend"/> to the collection by the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The <see cref="ChartLegend"/>.</param>
        public void Insert(int index, ChartLegend value)
        {
            List.Insert(index, value);
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
            ChartLegend legend = obj as ChartLegend;

            return (legend != null) && (!this.Contains(legend));
        }
        #endregion
    }
}
