//-------------------------------------------------------------------------------------------------
// <copyright file="HierarchyElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the collection of <see cref="HierarchyElement"/>.
    /// </summary>
    [Serializable]
    public class HierarchyElementCollection : CollectionBase, ICloneable<HierarchyElementCollection>
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Reports
{
    [CollectionDataContract]
    public class HierarchyElementCollection : Collection<HierarchyElement>
#endif
    {
        #region Private variables
        private DimensionElement _parentDimension;
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyElementCollection"/> class.
        /// </summary>
        /// <param name="parentDimesionElement">The parent dimesion element.</param>
        public HierarchyElementCollection(DimensionElement parentDimesionElement)
        {
            this._parentDimension = parentDimesionElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyElementCollection"/> class.
        /// </summary>
        public HierarchyElementCollection()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.HierarchyElement"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="HierarchyElement"/> object.</value>
        public HierarchyElement this[int index]
        {
            get
            {
                return (HierarchyElement)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified hierarchy element.
        /// </summary>
        /// <param name="hierarchyElement">The hierarchy element.</param>
        /// <returns> The position into which the new element was inserted, or -1 to indicate that
        /// the item was not inserted into the collection.</returns>
        public int Add(HierarchyElement hierarchyElement)
        {
            return base.List.Add(hierarchyElement);
        }

        /// <summary>
        /// Removes the specified hierarchy element.
        /// </summary>
        /// <param name="hierarchyElement">The hierarchy element.</param>
        public void Remove(HierarchyElement hierarchyElement)
        {
            base.List.Remove(hierarchyElement);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="HierarchyElementCollection"/>.</returns>
        public HierarchyElementCollection Clone()
        {
            HierarchyElementCollection hierarchyElementCollection = new HierarchyElementCollection();
            foreach (HierarchyElement hierarchyElement in base.List)
            {
                hierarchyElementCollection.Add(hierarchyElement.Clone());
            }

            return hierarchyElementCollection;
        }
#endif
        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Reports.HierarchyElement"/> with the specified name.
        /// </summary>
        /// <value><see cref="HierarchyElement"/></value>
        public HierarchyElement this[string name]
        {
            get
            {
                return this.FindHierarchyElementByName(name);
            }
        }

        /// <summary>
        /// Finds the name of the hierarchy element by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Object of type <see cref="HierarchyElement"/>.</returns>
        public HierarchyElement FindHierarchyElementByName(string name)
        {
#if !SILVERLIGHT
            foreach (HierarchyElement hierarchyElement in this.List)
#else
            foreach (HierarchyElement hierarchyElement in this.Items)
#endif 
            {
                if (hierarchyElement.Name == name)
                {
                    return hierarchyElement;
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
        public void Add(HierarchyElement hierarchyElement)
        {
             base.Items.Add(hierarchyElement);
             this.UpdateParent(hierarchyElement);
        }

        protected override void InsertItem(int index, HierarchyElement item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, HierarchyElement item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }
#endif
        #endregion


        #region Private Methods
        void UpdateParent(object hierarchyObj)
        {
            if (hierarchyObj is HierarchyElement)
            {
                HierarchyElement hierarchyElement = (HierarchyElement)hierarchyObj;
                hierarchyElement.ParentDimension = _parentDimension;
            }
        }
        #endregion
    }
}
