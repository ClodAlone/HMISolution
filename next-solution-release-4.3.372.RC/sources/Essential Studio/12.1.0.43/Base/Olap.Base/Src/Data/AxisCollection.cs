//-------------------------------------------------------------------------------------------------
// <copyright file="AxisCollection.cs" company="syncfusion">
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// The AxisCollection is used by a CellSet to represent the axes returned by a query. 
    /// Each Axis contained by an AxisCollection represents a set of congruent tuples organized 
    /// along one or more hierarchies. 
    /// </summary>
    /// <remarks>
    /// An AxisCollection is created in the constructor of Cellset and it is used to hold all
    /// the axis information of the cellset.  Axis are added in AxisCollection, in UpdateAxis method
    /// of AdomdDataProvider class.
    /// </remarks>
    [Serializable]
    public class AxisCollection : CollectionBase
    {
        #region Private Variables
        /// <summary>
        /// The Parent Cellset of the Axiscollection.
        /// </summary>
        [NonSerialized]
        private CellSet _parentCellset;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisCollection"/> class.
        /// </summary>
        /// <param name="parentCellset">The parent cellset.</param>
        public AxisCollection(CellSet parentCellset)
        {
            this._parentCellset = parentCellset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisCollection"/> class.
        /// </summary>
        public AxisCollection()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Axis"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the Axis</param>
        /// <value>Axis</value>
        [Description("Axis collection indexer.")]
        public Axis this[int index]
        {
            get
            {
                return (Axis)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the axis passed
        /// </summary>
        /// <param name="axis">Column or Row Axis</param>
        /// <returns>Axis added index</returns>
        public int Add(Axis axis)
        {
            return base.List.Add(axis);
        }

        /// <summary>
        /// Insets the specified index.
        /// </summary>
        /// <param name="index">insertion index of the Axis</param>
        /// <param name="axis">Column or Row axis</param>
        public void Insret(int index, Axis axis)
        {
            base.List.Insert(index, axis);
        }

        /// <summary>
        /// Removes the specified axis.
        /// </summary>
        /// <param name="axis">Column or Row axis</param>
        public void Remove(Axis axis)
        {
            base.List.Remove(axis);
        }
        #endregion
    }
}
