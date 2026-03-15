#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#if !WinRT
using System.ComponentModel;
#endif
#endif
namespace Syncfusion.Data
{
    //public class SortDescription
    //{
    //    private string _propertyName;
    //    private ListSortDirection _direction;
    //    public SortDescription(string propertyName, ListSortDirection direction)
    //    {
    //        if ((direction < ListSortDirection.Ascending) || (direction > ListSortDirection.Descending))
    //        {
    //            throw new ArgumentException("direction");
    //        }
    //        this._propertyName = propertyName;
    //        this._direction = direction;
    //    }

    //    public SortDescription()
    //    {
    //    }

    //    //Sorted Field Name
    //    public string PropertyName
    //    {
    //        get
    //        {
    //            return this._propertyName;
    //        }
    //        set
    //        {
    //            this._propertyName = value;
    //        }
    //    }

    //    //Sorting Direction
    //    public ListSortDirection Direction
    //    {
    //        get
    //        {
    //            return this._direction;
    //        }
    //        set
    //        {
    //            if ((value < ListSortDirection.Ascending) || (value > ListSortDirection.Descending))
    //            {
    //                throw new ArgumentException("value");
    //            }
    //            this._direction = value;
    //        }
    //    }
    //}

    public struct SortDescription
    {
        private string _propertyName;
        private ListSortDirection _direction;
        private bool _sealed;
        /// <summary>Gets or sets the property name being used as the sorting criteria.</summary>
        /// <returns>The default value is null.</returns>
        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
            set
            {
                if (this._sealed)
                {
                    throw new InvalidOperationException("CannotChangeAfterSealed");
                }
                this._propertyName = value;
            }
        }
        /// <summary>Gets or sets a value that indicates whether to sort in ascending or descending order.</summary>
        /// <returns>A <see cref="ListSortDirection" /> value to indicate whether to sort in ascending or descending order.</returns>
        public ListSortDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                if (this._sealed)
                {
                    throw new InvalidOperationException("CannotChangeAfterSealed");
                }
                if (value < ListSortDirection.Ascending || value > ListSortDirection.Descending)
                {
                    
                    throw new ArgumentException("Invalid enum");
                }
                this._direction = value;
            }
        }
        /// <summary>Gets a value that indicates whether this object is in an immutable state.</summary>
        /// <returns>true if this object is in use; otherwise, false.</returns>
        public bool IsSealed
        {
            get
            {
                return this._sealed;
            }
        }
        /// <summary>Initializes a new instance of the <see cref="SortDescription" /> structure.</summary>
        /// <param name="propertyName">The name of the property to sort the list by.</param>
        /// <param name="direction">The sort order.</param>
        public SortDescription(string propertyName, ListSortDirection direction)
        {
            if (direction != ListSortDirection.Ascending && direction != ListSortDirection.Descending)
            {
                throw new ArgumentException("Invalid Enum"); 
            }
            this._propertyName = propertyName;
            this._direction = direction;
            this._sealed = false;
        }
        /// <summary>Compares the specified instance and the current instance of <see cref="SortDescription" /> for value equality.</summary>
        /// <returns>true if <paramref name="obj" /> and this instance of <see cref="SortDescription" /> have the same values.</returns>
        /// <param name="obj">The <see cref="SortDescription" /> instance to compare.</param>
        public override bool Equals(object obj)
        {
            return obj is SortDescription && this == (SortDescription)obj;
        }
        /// <summary>Compares two <see cref="SortDescription" /> objects for value equality.</summary>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        /// <param name="sd1">The first instance to compare.</param>
        /// <param name="sd2">The second instance to compare.</param>
        public static bool operator ==(SortDescription sd1, SortDescription sd2)
        {
            return sd1.PropertyName == sd2.PropertyName && sd1.Direction == sd2.Direction;
        }
        /// <summary>Compares two <see cref="SortDescription" /> objects for value inequality.</summary>
        /// <returns>true if the values are not equal; otherwise, false.</returns>
        /// <param name="sd1">The first instance to compare.</param>
        /// <param name="sd2">The second instance to compare.</param>
        public static bool operator !=(SortDescription sd1, SortDescription sd2)
        {
            return !(sd1 == sd2);
        }
        /// <summary>Returns the hash code for this instance of <see cref="SortDescription" />.</summary>
        /// <returns>The hash code for this instance of <see cref="SortDescription" />.</returns>
        public override int GetHashCode()
        {
            int num = this.Direction.GetHashCode();
            if (this.PropertyName != null)
            {
                num = this.PropertyName.GetHashCode() + num;
            }
            return num;
        }
        internal void Seal()
        {
            this._sealed = true;
        }
    }

    public class SortDescriptionCollection : Collection<SortDescription>, INotifyCollectionChanged
    {
        private class EmptySortDescriptionCollection : SortDescriptionCollection, IList, ICollection, IEnumerable
        {
            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }
            bool IList.IsReadOnly
            {
                get
                {
                    return true;
                }
            }
            protected override void ClearItems()
            {
                throw new NotSupportedException();
            }
            protected override void RemoveItem(int index)
            {
                throw new NotSupportedException();
            }
            protected override void InsertItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }
            protected override void SetItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets an empty and non-modifiable instance of <see cref="SortDescriptionCollection" />. </summary>
        public static readonly SortDescriptionCollection Empty = new SortDescriptionCollection.EmptySortDescriptionCollection();
        /// <summary>Occurs when an item is added or removed.</summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                this.CollectionChanged += value;
            }
            remove
            {
                this.CollectionChanged -= value;
            }
        }
        /// <summary>Occurs when an item is added or removed.</summary>
        protected event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>Removes all items from the collection.</summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }
        /// <summary>Removes the item at the specified index in the collection.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            SortDescription sortDescription = base[index];
            base.RemoveItem(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, sortDescription, index);
        }
        /// <summary>Inserts an item into the collection at the specified index.</summary>
        /// <param name="index">The zero-based index where the <paramref name="item" /> is inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, SortDescription item)
        {
            item.Seal();
            base.InsertItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }
        /// <summary>Replaces the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, SortDescription item)
        {
            item.Seal();
            SortDescription sortDescription = base[index];
            base.SetItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, sortDescription, index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }
        /// <summary>Initializes a new instance of the <see cref="SortDescriptionCollection" /> class.</summary>
        public SortDescriptionCollection()
        {
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }
    }
}
