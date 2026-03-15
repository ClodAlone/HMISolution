//-------------------------------------------------------------------------------------------------
// <copyright file="NamedSetCollection.cs" company="syncfusion">
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

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// A Collection of NamedSet objects
    /// </summary>
    [Serializable]
    public class NamedSetCollection : CollectionBase, IEnumerable<NamedSet>
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A Collection of NamedSet objects
    /// </summary>
    [CollectionDataContract]
    public class NamedSetCollection : Collection<NamedSet>
    {
#endif
        #region Private Variables 
        CubeSchema _parentCubeSchema;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedSetCollection"/> class.
        /// </summary>
        public NamedSetCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionCollection"/> class.
        /// </summary>
        /// <param name="parentCubeSchema">The parent cube schema.</param>
        public NamedSetCollection(CubeSchema parentCubeSchema)
        {
            _parentCubeSchema = parentCubeSchema;
        }
        #endregion

        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.NamedSet"/> at the specified index.
        /// </summary>
        /// <value></value>
        public NamedSet this[int index]
        {
            get { return (NamedSet)base.List[index]; }
            set { base.List[index] = value; }
        }

        /// <summary>
        /// Adds the NamedSet passed
        /// </summary>
        /// <param name="namedSet">The NamedSet object</param>
        /// <returns>index of the NamedSet added</returns>
        public int Add(NamedSet namedSet)
        {
            return base.List.Add(namedSet);
        }

        /// <summary>
        /// Determines whether [contains] [the specified NamedSet].
        /// </summary>
        /// <param name="namedSet">The NamedSet object</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified NamedSet]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(NamedSet namedSet)
        {
            return base.List.Contains(namedSet);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="namedSetArray">The NamedSet array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(NamedSet[] namedSetArray, int index)
        {
            base.List.CopyTo(namedSetArray, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<NamedSet> GetEnumerator()
        {
            foreach (NamedSet namedSet in base.List)
            {
                yield return namedSet;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="namedSet">The NamedSet Object</param>
        /// <returns>returns the index of the namedSet object in the current collection</returns>
        public int IndexOf(NamedSet namedSet)
        {
            return base.List.IndexOf(namedSet);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index to which the namedSet object should be inserted</param>
        /// <param name="namedSet">The NamedSet object which is to be inserted</param>
        public void Insert(int index, NamedSet namedSet)
        {
            base.List.Insert(index, namedSet);
        }

        /// <summary>
        /// Removes the specified dimension.
        /// </summary>
        /// <param name="namedSet">The NamedSet object which should be removed</param>
        public void Remove(NamedSet namedSet)
        {
            base.List.Remove(namedSet);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.CollectionBase"/> instance. This method is not overridable.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.CollectionBase.Count"/>.
        /// </exception>
        public new void RemoveAt(int index)
        {
            base.List.RemoveAt(index);
        }
#endif
        /// <summary>
        /// Finds the NamedSet by name
        /// </summary>
        /// <param name="name">NamedSet name of type string.</param>
        /// <returns>NamedSet object</returns>
        public NamedSet FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (NamedSet namedSet in base.List)
            {
#else
            foreach (NamedSet namedSet in base.Items)
            {
#endif
                if (namedSet.Name == name)
                {
                    return namedSet;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the NamedSet by its unique name
        /// </summary>
        /// <param name="uniqueName">NamedSet unique name</param>
        /// <returns>NamedSet object</returns>
        public NamedSet FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (NamedSet namedSet in base.List)
            {
#else
            foreach (NamedSet namedSet in base.Items)
            {
#endif         
               if (namedSet.UniqueName == uniqueName)
                {
                    return namedSet;
                }
            }

            return null;
        }

        #endregion

        #region Protected Memthods
#if !SILVERLIGHT
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnInsertComplete(int index, object value)
        {
            this.UpdateParent(value);
        }

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            this.UpdateParent(newValue);
        }
#else

        protected override void InsertItem(int index, NamedSet item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, NamedSet item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }

#endif
        #endregion

        #region Private Methods
        void UpdateParent(object namedSetObj)
        {
            if (namedSetObj is NamedSet)
            {
                NamedSet namedSet = (NamedSet)namedSetObj;
                namedSet.ParentCubeSchema = this._parentCubeSchema;
            }
        }
        #endregion
    }
}
