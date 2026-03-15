#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.Sorting
{
    /// <summary>
    /// Represents the sort Field collection.
    /// </summary>
    class SortFields :
       CollectionBaseEx<ISortField>,
       ISortFields
    {


        #region Intialization
        public SortFields(IApplication application, object parent)
            : base(application, parent)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the SortField in the collection.
        /// </summary>
        /// <param name="key">Column Index to sort the data.</param>
        /// <param name="sortBasedOn">To sort the data based on.</param>
        /// <param name="orderBy">To order the sorted data.</param>
        /// <returns>Added sort field.</returns>
        public ISortField Add(int key, SortOn sortBasedOn, OrderBy orderBy)
        {
            SortField sortField = new SortField(this);
            sortField.Key = key;
            sortField.SortOn = sortBasedOn;
            sortField.Order = orderBy;
            base.Add(sortField);
            return sortField;
        }
        /// <summary>
        /// Removes the sortField in the collection.
        /// </summary>
        /// <param name="sortField">Sort Field to remove from the collection.</param>
        public void Remove(ISortField sortField)
        {
            Remove(sortField.Key);
        }
        /// <summary>
        /// Remvoes the Sort Field in the collection.
        /// </summary>
        /// <param name="fieldIndex">Field index to remove.</param>
        public void Remove(int key)
        {
            int index = FindByKey(key);
            if (index == -1)
                throw new ArgumentOutOfRangeException("Key Not found");
            RemoveAt(index);
        }
        /// <summary>
        /// Sets the priority of the column to sort.
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="priority"></param>
        internal void SetPriority(SortField sortField, int priority)
        {
            int index = FindByKey(sortField.Key);
            if(index!=-1)
                RemoveAt(index);
            base.Insert(priority, sortField);
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Finds the SortField by Key.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>index of the SortField.</returns>
        internal  int FindByKey(int key)
        {
            int index = 0;
            foreach (ISortField sortField in this)
            {
                if (sortField.Key == key)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        #endregion
    }
}
