#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Collection which holds the carousel's child items
    /// </summary>
    public class ItemCollection : IList<Control>, IList
    {
        #region Constructor
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="container">The Control that this collection is associated with.</param>
        internal ItemCollection(Carousel container)
        {
            this.container = container;
            items = new List<Control>();
        }
        #endregion

        #region Fields
        /// <summary>
        /// The Control this collection is associated with.
        /// </summary>
        internal Carousel container;

        /// <summary>
        /// The list of items stored in this control.
        /// </summary>
        internal List<Control> items;

        #endregion

        #region Methods
        /// <summary>
        /// Sorts the items in the collection
        /// </summary>
        public void Sort()
        {
            items.Sort();
            container.Populate();
        }

        /// <summary>
        /// Sorts the items in the collection using the provided comparer.
        /// </summary>
        /// <param name="comparer">The comparer used to compare items.</param>
        public void Sort(IComparer<Control> comparer)
        {
            items.Sort(comparer);
            container.Populate();
        }
       
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<Control> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        List<Control> itemCollectionList = new List<Control>();
        public IEnumerable<Control> AsEnumerable()
        {
            foreach (Control item in this)
            {
                if (!itemCollectionList.Contains(item))
                    itemCollectionList.Add(item);
            }
            return itemCollectionList;
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        bool ICollection<Control>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Add(Control item)
        {
            items.Add(item);
            container.Controls.Add(item);
            item.Name = string.IsNullOrEmpty(item.Name) ? item.GetType().Name + (IndexOf(item) + 1).ToString() : item.Name;
            item.Text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
            container.Populate();
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (Control item in items)
                container.Controls.Remove(item);

            items.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>True if the item is found in the collection, otherwise false.</returns>
        public bool Contains(Control item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection<Control>.CopyTo(Control[] array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>True if the item was successfully removed from the colleection, otherwise false.  This method
        /// also returns false if the item is not found in the original collection.</returns>
        public bool Remove(Control item)
        {
            //item.SetOwner(null);
            bool rval = items.Remove(item);
            container.Controls.Remove(item);
            container.Populate();
            return rval;
        }
        #endregion

        #region ICollection Members
        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins</param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)items).CopyTo(array, index);
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region IList<T> Members
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public Control this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                if (items[index] != value)
                {
                    
                    items[index] = value;
                    
                    
                }
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The index of the item if found in the list, otherwise -1.</returns>
        public int IndexOf(Control item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the list.</param>
        public void Insert(int index, Control item)
        {
            
            items.Insert(index, item);
            container.Populate();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            container.Controls.RemoveAt(index);
            
            items.RemoveAt(index);
            container.Populate();
        }
        #endregion

        #region IList Members
        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="obj">The item to add to the list.</param>
        /// <returns>The position at which the item was inserted.</returns>
        int IList.Add(object obj)
        {
            this.Add((Control)obj);
            return this.Count - 1;
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="obj">The object to locate in the list.</param>
        /// <returns>True if an instance of the item was found in the list, otherwise false.</returns>
        bool IList.Contains(object obj)
        {
            return this.Contains((Control)obj);
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="obj">The object to locate in the list.</param>
        /// <returns>The index of the item if found in the list, otherwise -1.</returns>
        int IList.IndexOf(object obj)
        {
            return this.IndexOf((Control)obj);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="obj">The object to insert into the list.</param>
        void IList.Insert(int index, object obj)
        {
            this.Insert(index, (Control)obj);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        void IList.Remove(object obj)
        {
            this.Remove((Control)obj);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (Control)value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }
        #endregion
    }

    public class CarouselImageCollection : IList<CarouselImage>, IList
    {
        #region Constructor
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="container">The control that this collection is associated with.</param>
        internal CarouselImageCollection(Carousel container)
        {
            this.container = container;
            items = new List<CarouselImage>();
        }
        #endregion

        #region Fields
        /// <summary>
        /// The control this collection is associated with.
        /// </summary>
        internal Carousel container;

        /// <summary>
        /// The list of items stored in this control.
        /// </summary>
        internal List<CarouselImage> items;

        #endregion

        #region Methods
        /// <summary>
        /// Sorts the items in the collection 
        /// </summary>
        public void Sort()
        {
            items.Sort();
            container.Populate();
        }

        /// <summary>
        /// Sorts the items in the collection using the provided comparer.
        /// </summary>
        /// <param name="comparer">The comparer used to compare items.</param>
        public void Sort(IComparer<CarouselImage> comparer)
        {
            items.Sort(comparer);
            container.Populate();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<CarouselImage> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        List<CarouselImage> itemCollectionList = new List<CarouselImage>();
        public IEnumerable<CarouselImage> AsEnumerable()
        {
            foreach (CarouselImage item in this)
            {
                if (!itemCollectionList.Contains(item))
                    itemCollectionList.Add(item);
            }
            return itemCollectionList;
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        bool ICollection<CarouselImage>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Add(CarouselImage item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>True if the item is found in the collection, otherwise false.</returns>
        public bool Contains(CarouselImage item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection<CarouselImage>.CopyTo(CarouselImage[] array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>True if the item was successfully removed from the colleection, otherwise false.  This method
        /// also returns false if the item is not found in the original collection.</returns>
        public bool Remove(CarouselImage item)
        {
            //item.SetOwner(null);
            bool rval = items.Remove(item);
            //container.Controls.Remove(item);
            container.Populate();
            return rval;
        }
        #endregion

        #region ICollection Members
        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins</param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)items).CopyTo(array, index);
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region IList<T> Members
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public CarouselImage this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                if (items[index] != value)
                {

                    items[index] = value;


                }
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The index of the item if found in the list, otherwise -1.</returns>
        public int IndexOf(CarouselImage item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the list.</param>
        public void Insert(int index, CarouselImage item)
        {

            items.Insert(index, item);
            container.Populate();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            container.Controls.RemoveAt(index);

            items.RemoveAt(index);
            container.Populate();
        }
        #endregion

        #region IList Members
        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="obj">The item to add to the list.</param>
        /// <returns>The position at which the item was inserted.</returns>
        int IList.Add(object obj)
        {
            this.Add((CarouselImage)obj);
            return this.Count - 1;
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="obj">The object to locate in the list.</param>
        /// <returns>True if an instance of the item was found in the list, otherwise false.</returns>
        bool IList.Contains(object obj)
        {
            return this.Contains((CarouselImage)obj);
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="obj">The object to locate in the list.</param>
        /// <returns>The index of the item if found in the list, otherwise -1.</returns>
        int IList.IndexOf(object obj)
        {
            return this.IndexOf((CarouselImage)obj);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="obj">The object to insert into the list.</param>
        void IList.Insert(int index, object obj)
        {
            this.Insert(index, (CarouselImage)obj);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        void IList.Remove(object obj)
        {
            this.Remove((CarouselImage)obj);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (CarouselImage)value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}
