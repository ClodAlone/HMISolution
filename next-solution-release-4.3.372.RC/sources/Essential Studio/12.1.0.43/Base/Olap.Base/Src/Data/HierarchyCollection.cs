//-------------------------------------------------------------------------------------------------
// <copyright file="HierarchyCollection.cs" company="syncfusion">
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
    /// Collection of Hierarchy Objects
    /// </summary>
    [Serializable]
    public class HierarchyCollection : CollectionBase, IEnumerable<Hierarchy>
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Collection of Hierarchy Objects
    /// </summary>
    [CollectionDataContract]
    public class HierarchyCollection : Collection<Hierarchy>
    {
#endif
        #region Private Variables
        Dimension _parentDimension;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyCollection"/> class.
        /// </summary>
        public HierarchyCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyCollection"/> class.
        /// </summary>
        /// <param name="parentDimension">The parent dimension.</param>
        public HierarchyCollection(Dimension parentDimension)
        {
            _parentDimension = parentDimension;
        }
        #endregion

        #region Public Methods
        

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Hierarchy"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Hierarchy this[int index]
        {
            get { return (Hierarchy)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Adds the specified hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>index of the hierarchy added</returns>

        public int Add(Hierarchy hierarchy)
        {
            return base.List.Add(hierarchy);
        }

        /// <summary>
        /// Determines whether [contains] [the specified hierarchy].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified hierarchy]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Hierarchy hierarchy)
        {
            return base.List.Contains(hierarchy);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="hierarchyArray">The hierarchy array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Hierarchy[] hierarchyArray, int index)
        {
            base.List.CopyTo(hierarchyArray, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Hierarchy> GetEnumerator()
        {
            foreach (Hierarchy hierarchy in base.List)
            {
                yield return hierarchy;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>returns the index of Hierarchy Object in the current collection</returns>
        public int IndexOf(Hierarchy hierarchy)
        {
            return base.List.IndexOf(hierarchy);
        }

        /// <summary>
        /// Inserts the Hierarchy object in the specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        public void Insert(int index, Hierarchy hierarchy)
        {
            base.List.Insert(index, hierarchy);
        }

        /// <summary>
        /// Removes the specified hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public void Remove(Hierarchy hierarchy)
        {
            base.List.Remove(hierarchy);
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
        /// Finds the Hierarchy by its name
        /// </summary>
        /// <param name="name">The name of the Hierarchy</param>
        /// <returns>Hierarchy object</returns>
        public Hierarchy FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (Hierarchy hierarchy in base.List)
            {
#else
            foreach (Hierarchy hierarchy in base.Items)
            {
#endif
                if (hierarchy.Name == name)
                {
                    return hierarchy;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Hierarchy by its unique name
        /// </summary>
        /// <param name="uniqueName">Hierarchy unique name.</param>
        /// <returns>Hierarchy Objects</returns>
        public Hierarchy FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (Hierarchy hierarchy in base.List)
            {
#else
            foreach (Hierarchy hierarchy in base.Items)
            {
#endif
                if (hierarchy.UniqueName == uniqueName)
                {
                    return hierarchy;
                }
            }
            
            return null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the parent.
        /// </summary>
        /// <param name="hierarchyObj">The hierarchy obj.</param>
        void UpdateParent(object hierarchyObj)
        {
            if (hierarchyObj is Hierarchy)
            {
                Hierarchy hierarcy = (Hierarchy)hierarchyObj;
                hierarcy.ParentDimension = _parentDimension;
            }
        }
        #endregion

        #region Protected Methods
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

        protected override void InsertItem(int index, Hierarchy item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, Hierarchy item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }

#endif
        #endregion
    }
}
