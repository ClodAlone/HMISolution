//-------------------------------------------------------------------------------------------------
// <copyright file="DimensionCollection.cs" company="syncfusion">
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
    /// A Collection of Dimension objects
    /// </summary>
    /// <remarks>
    /// A DimensionCollection is created in CubeSchema to hold all the 
    /// Dimension objects of the Current Cube.  It is associated only with CubeSchema.
    /// </remarks>
    [Serializable]
    public class DimensionCollection : CollectionBase, IEnumerable<Dimension>
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A Collection of Dimension objects
    /// </summary>
    /// <remarks>
    /// A DimensionCollection is created in CubeSchema to hold all the 
    /// Dimension objects of the Current Cube.  It is associated only with CubeSchema.
    /// </remarks>
    [CollectionDataContract]
    public class DimensionCollection : Collection<Dimension>
    {
#endif
        #region Private Variables

        CubeSchema _parentCubeSchema;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionCollection"/> class.
        /// </summary>
        /// <param name="parentCubeSchema">The parent cube schema.</param>
        public DimensionCollection(CubeSchema parentCubeSchema)
        {
            _parentCubeSchema = parentCubeSchema;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionCollection"/> class.
        /// </summary>
        public DimensionCollection()
        {
        }

        #endregion

        #region Public Properties
       
 #if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Dimension"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Dimension this[int index]
        {
            get { return (Dimension)base.List[index]; }
            set { base.List[index] = value; }
        }

        /// <summary>
        /// Adds the dimension passed
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>index of the dimension added</returns>

        public int Add(Dimension dimension)
        {
            return base.List.Add(dimension);
        }

        /// <summary>
        /// Determines whether [contains] [the specified dimension].
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified dimension]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Dimension dimension)
        {
            return base.List.Contains(dimension);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="dimensionArray">The dimension array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Dimension[] dimensionArray, int index)
        {
            base.List.CopyTo(dimensionArray, index);
        }

        

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Dimension> GetEnumerator()
        {
            foreach (Dimension dimension in base.List)
            {
                yield return dimension;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>returns the index in the dimension collection</returns>
        public int IndexOf(Dimension dimension)
        {
            return base.List.IndexOf(dimension);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="dimension">The dimension.</param>
        public void Insert(int index, Dimension dimension)
        {
            base.List.Insert(index, dimension);
        }

        /// <summary>
        /// Removes the specified dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        public void Remove(Dimension dimension)
        {
            base.List.Remove(dimension);
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
        /// Finds the Dimension by name
        /// </summary>
        /// <param name="name">Dimension name of type string.</param>
        /// <returns>Dimension object</returns>
        public Dimension FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (Dimension dimension in base.List)
            {
#else
            foreach (Dimension dimension in base.Items)
            {
#endif
                if (dimension.Name == name)
                {
                    return dimension;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Dimension by its unique name
        /// </summary>
        /// <param name="uniqueName">Dimension unique name</param>
        /// <returns>Dimension object</returns>
        public Dimension FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (Dimension dimension in base.List)
            {
#else
            foreach (Dimension dimension in base.Items)
            {
#endif
                if (dimension.UniqueName == uniqueName)
                {
                    return dimension;
                }
            }

            return null;
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
        protected override void InsertItem(int index, Dimension item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, Dimension item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }
#endif
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the parent.
        /// </summary>
        /// <param name="dimensionObj">The dimension obj.</param>
        void UpdateParent(object dimensionObj)
        {
            if (dimensionObj is Dimension)
            {
                Dimension dimension = (Dimension)dimensionObj;
                dimension.ParentCubeSchema = this._parentCubeSchema;
            }
        }
        #endregion

    }
}
