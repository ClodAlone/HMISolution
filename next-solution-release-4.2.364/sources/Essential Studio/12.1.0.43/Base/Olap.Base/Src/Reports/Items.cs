//-------------------------------------------------------------------------------------------------
// <copyright file="Items.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
#else
namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    using System;
    using System.Collections.Generic;

#if !SILVERLIGHT
    using Syncfusion.Olap.Common;

    /// <summary>
    /// Collection of item objects
    /// </summary>
    [Serializable]
    public class Items : ICloneable<Items>
#else
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using Syncfusion.OlapSilverlight.Common;
   
    public class Items 
#endif
    {
        #region Private Variables
        bool _isFilterOrSortOn;
        #endregion

        #region Constructor
#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="Items"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public Items(List<Item> items)
        {
            this.List = new List<Item>();
            this.AddRange(items);
            _isFilterOrSortOn = false;
        }
#else
        public Items(List<Item> items)
        {
            foreach (var item in items)
            {
                this.List.Add(item);
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Items"/> class.
        /// </summary>
        public Items()
        {
            this.List = new List<Item>();
            _isFilterOrSortOn = false;
        }
        #endregion

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public List<Item> List { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter or sort on.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter or sort on; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterOrSortOn
        {
            get
            {
                return _isFilterOrSortOn;
            }

            set
            {
                _isFilterOrSortOn = value;
                UpdateFilterOrSortingStatus(_isFilterOrSortOn);
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the sub set element.
        /// </summary>
        /// <value>The sub set element.</value>
        public SubsetElement SubSetElement 
        { 
            get; 
            set; 
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
               return this.List.Count;
            }

        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(Item item)
        {
            this.List.Add(item);
        }

        /// <summary>
        /// Adds the specified element value.
        /// </summary>
        /// <param name="elementValue">The element value.</param>
        public void Add(Element elementValue)
        {
            this.List.Add(new Item(elementValue));
        }

        /// <summary>
        /// Adds the specified dimension element.
        /// </summary>
        /// <param name="dimensionElement">The dimension element.</param>
        /// <param name="excludedDimensionElement">The excluded dimension element.</param>
        public void Add(Element dimensionElement, Element excludedDimensionElement)
        {
            this.List.Add(new Item(dimensionElement, excludedDimensionElement));
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(Items items)
        {
            foreach (var item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(List<Item> items)
        {
            foreach (var item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.List.Clear();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(Item item)
        {
            this.List.Remove(item);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void RemoveAll(AxisPosition axis)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                Item item = this.List[i] as Item;
                if (item != null && item.Axis == axis)
                {
                    this.List.Remove(item);
                    i--;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.Item"/> at the specified index.
        /// </summary>
        /// <value><see cref="Item"/></value>
        public Item this[int index]
        {
            get { return this.List[index] as Item; }
            set { this.List[index] = value; }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerated <see cref="Item"/>.</returns>
        public IEnumerator<Item> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }


#if !SILVERLIGHT

        /// <summary>
        /// Updates the filter or sorting status.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void UpdateFilterOrSortingStatus(bool value)
        {
            foreach (Item item in this)
            {
                item.IsFilterOrSortOn = value;
            }
        }


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Items Clone()
        {
            Items items = new Items();
            foreach (Item item in this.List)
            {
                this.List.Add(item.Clone());
            }

            items.IsFilterOrSortOn = this.IsFilterOrSortOn;
            return items;
        }


#endif
        #endregion

#if SILVERLIGHT
        #region Private Methods
        private void UpdateFilterOrSortingStatus(bool value)
        {
            foreach (Item item in this.List)
            {
                item.IsFilterOrSortOn = value;
            }
        }
        #endregion

#endif


    }
}
