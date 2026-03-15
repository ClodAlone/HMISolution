//-------------------------------------------------------------------------------------------------
// <copyright file="TupleCollection.cs" company="syncfusion">
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
using System.Collections.ObjectModel;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// This class will holds the collection of the Tuples.
    /// </summary>
    [Serializable]
    public class TupleCollection : Collection<Tuple>
    {
        #region Private Variables
        Axis _parentAxis;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TupleCollection"/> class.
        /// </summary>
        /// <param name="parentAxis">The parent axis.</param>
        public TupleCollection(Axis parentAxis)
        {
            _parentAxis = parentAxis;
            this.MaxLevel = new List<int>();
            this.MinLevel = new List<int>();
            this.valueSet = new List<bool>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleCollection"/> class.
        /// </summary>
        public TupleCollection()
        {
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void InsertItem(int index, Tuple item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item, index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void SetItem(int index, Tuple item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item, index);
        }

        /// <summary>
        /// Determines the Max Level of the Current Member in the TupleSet
        /// </summary>
        public List<int> MaxLevel;

        /// <summary>
        /// Determines the Min Level of the Current Member in the TupleSet
        /// </summary>
        public List<int> MinLevel;

        /// <summary>
        /// Determines whether Min Level has been already set or not.  Returns true if already set or else returns false.
        /// </summary>
        public List<bool> valueSet;
        #endregion

        #region Private Methods
        void UpdateParent(object tupleObj, int index)
        {
            if (tupleObj is Tuple)
            {
                Tuple tuple = (Tuple)tupleObj;
                tuple.parentAxis = this._parentAxis;
                tuple.OrdinalPosition = index;
            }
        }
        #endregion
    }
}
