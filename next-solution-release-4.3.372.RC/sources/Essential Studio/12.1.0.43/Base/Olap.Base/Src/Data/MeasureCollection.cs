//-------------------------------------------------------------------------------------------------
// <copyright file="MeasureCollection.cs" company="syncfusion">
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
    /// Collection of measure objects
    /// </summary>
    [Serializable]
    public class MeasureCollection : CollectionBase, IEnumerable<Measure>
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Collection of measure objects
    /// </summary>
    [CollectionDataContract]
    public class MeasureCollection : Collection<Measure>
    {
#endif
        #region Private Variables
        CubeSchema _parentCubeSchema;
        #endregion

        #region  Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureCollection"/> class.
        /// </summary>
        /// <param name="parentCubeSchema">The parent cube schema.</param>
        public MeasureCollection(CubeSchema parentCubeSchema)
        {
            this._parentCubeSchema = parentCubeSchema;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureCollection"/> class.
        /// </summary>
        public MeasureCollection()
        {
        }
        #endregion

        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Measure"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Measure this[int index]
        {
            get { return (Measure)base.List[index]; }
            set { base.List[index] = value; }
        }

        /// <summary>
        /// Adds the Measure passed
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <returns>index of the measure added</returns>
        public int Add(Measure measure)
        {
            return base.List.Add(measure);
        }

        /// <summary>
        /// Determines whether [contains] [the specified measure].
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified measure]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Measure measure)
        {
            return base.List.Contains(measure);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="measureArray">The measure array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Measure[] measureArray, int index)
        {
            base.List.CopyTo(measureArray, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Measure> GetEnumerator()
        {
            foreach (Measure measure in base.List)
            {
                yield return measure;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <returns>index of the measure object in the collection</returns>
        public int IndexOf(Measure measure)
        {
            return base.List.IndexOf(measure);
        }

        /// <summary>
        /// Inserts the  measure to the specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="measure">The measure.</param>
        public void Insert(int index, Measure measure)
        {
            base.List.Insert(index, measure);
        }

        /// <summary>
        /// Removes the specified measure.
        /// </summary>
        /// <param name="measure">The measure.</param>
        public void Remove(Measure measure)
        {
            base.List.Remove(measure);
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
        /// Finds the Measure but its name
        /// </summary>
        /// <param name="name">The name of the Measure Object</param>
        /// <returns>Measure object if found else returns null</returns>
        public Measure FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (Measure measure in base.List)
            {
#else
            foreach (Measure measure in base.Items)
            {
#endif
                if (measure.Name == name)
                {
                    return measure;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Measure by its unique name.
        /// </summary>
        /// <param name="uniqueName">Measure uniqe name</param>
        /// <returns>Measure object if found else returns null</returns>
        public Measure FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (Measure measure in base.List)
            {
#else
            foreach (Measure measure in base.Items)
            {
#endif
                if (measure.UniqueName == uniqueName)
                {
                    return measure;
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
        protected override void InsertItem(int index, Measure item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, Measure item)
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
        /// <param name="measureObj">The measure obj.</param>
        void UpdateParent(object measureObj)
        {
            if (measureObj is Measure)
            {
                Measure measure = (Measure)measureObj;
                measure.ParentCubeSchema = this._parentCubeSchema;
            }
        }
        #endregion
    }
}
