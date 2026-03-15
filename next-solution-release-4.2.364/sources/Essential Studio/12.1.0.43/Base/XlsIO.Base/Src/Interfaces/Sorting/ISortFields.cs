#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;

namespace Syncfusion.XlsIO
{
    public interface ISortFields : IEnumerable
    {
        #region Properties
        /// <summary>
        /// Represents the field count.
        /// </summary>
        int Count { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the SortField in the collection.
        /// </summary>
        /// <param name="key">Column Index to sort the data.</param>
        /// <param name="sortBasedOn">To sort the data based on.</param>
        /// <param name="orderBy">To order the sorted data.</param>
        /// <returns>Returns the Added Sort Field.</returns>
        ISortField Add(int key, SortOn sortBasedOn, OrderBy orderBy);
        /// <summary>
        /// Removes the sortField in the collection with the Key.
        /// </summary>
        /// <param name="key">Sort field Key to remove.</param>
        void Remove(int key);
        /// <summary>
        /// Removes the sortField in the collection.
        /// </summary>
        /// <param name="sortField">Sort Field to remove from the collection.</param>
        void Remove(ISortField sortField);
        /// <summary>
        /// Returns single item from the collection.
        /// </summary>
        /// <param name="key">Gets the item based on the Key.</param>
        /// <returns>SortFiled with the given key.</returns>
        ISortField this[int index] { get; }
        #endregion
    }
}
