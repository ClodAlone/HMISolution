//-------------------------------------------------------------------------------------------------
// <copyright file="KPICollection.cs" company="syncfusion">
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
    /// A Collection of KPI objects
    /// </summary>
    [Serializable]
    public class KpiCollection : CollectionBase, IEnumerable<Kpi>
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A Collection of KPI objects
    /// </summary>
    [CollectionDataContract]
    public class KpiCollection : Collection<Kpi>
    {
#endif
        #region Private Variables 
        CubeSchema _parentCubeSchema;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="KpiCollection"/> class.
        /// </summary>
        public KpiCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionCollection"/> class.
        /// </summary>
        /// <param name="parentCubeSchema">The parent cube schema.</param>
        public KpiCollection(CubeSchema parentCubeSchema)
        {
            _parentCubeSchema = parentCubeSchema;
        }
        #endregion

        #region Public Methods
       
#if !SILVERLIGHT


        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Kpi"/> at the specified index.
        /// </summary>
        /// <value><see cref="Kpi"/></value>
        public Kpi this[int index]
        {
            get { return (Kpi)base.List[index]; }
            set { base.List[index] = value; }
        }

        /// <summary>
        /// Adds the KPI passed
        /// </summary>
        /// <param name="kpi">The KPI object</param>
        /// <returns>index of the KPI added</returns>
        public int Add(Kpi kpi)
        {
            return base.List.Add(kpi);
        }

        /// <summary>
        /// Determines whether [contains] [the specified KPI].
        /// </summary>
        /// <param name="kpi">The KPI object</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified KPI]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Kpi kpi)
        {
            return base.List.Contains(kpi);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="kpiArray">The KPI array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Kpi[] kpiArray, int index)
        {
            base.List.CopyTo(kpiArray, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Kpi> GetEnumerator()
        {
            foreach (Kpi kpi in base.List)
            {
                yield return kpi;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="kpi">The KPI Object</param>
        /// <returns>returns the index of the kpi object in the current collection</returns>
        public int IndexOf(Kpi kpi)
        {
            return base.List.IndexOf(kpi);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index to which the kpi object should be inserted</param>
        /// <param name="kpi">The KPI object which is to be inserted</param>
        public void Insert(int index, Kpi kpi)
        {
            base.List.Insert(index, kpi);
        }

        /// <summary>
        /// Removes the specified dimension.
        /// </summary>
        /// <param name="kpi">The KPI object which should be removed</param>
        public void Remove(Kpi kpi)
        {
            base.List.Remove(kpi);
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
        /// Finds the KPI by name
        /// </summary>
        /// <param name="name">KPI name of type string.</param>
        /// <returns>KPI object</returns>
        public Kpi FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (Kpi kpi in base.List)
            {
#else
            foreach (Kpi kpi in base.Items)
            {
#endif
                if (kpi.Name == name)
                {
                    return kpi;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the KPI by its unique name
        /// </summary>
        /// <param name="uniqueName">KPI unique name</param>
        /// <returns>KPI object</returns>
        public Kpi FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (Kpi kpi in base.List)
            {
#else
            foreach (Kpi kpi in base.Items)
            {
#endif
                if (kpi.UniqueName == uniqueName)
                {
                    return kpi;
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
        protected override void InsertItem(int index, Kpi item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, Kpi item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }
#endif
        #endregion

        #region Private Methods
        void UpdateParent(object kpiObj)
        {
            if (kpiObj is Kpi)
            {
                Kpi kpi = (Kpi)kpiObj;
                kpi.ParentCubeSchema = this._parentCubeSchema;
            }
        }
        #endregion
    }
}
