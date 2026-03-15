#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellSpanInfoCollection<T> : IList<T> where T : GraphicCellSpanInfo
    {
        List<T> inner = new List<T>();

        #region IList Members

        public int IndexOf(T item)
        {
            if (item == null)
                return -1;

            return inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException();

            inner.Insert(index, item);
            ItemAdded(item);
        }

        public void RemoveAt(int index)
        {
            T item = this[index];
            inner.RemoveAt(index);
            ItemRemoved(item);
        }

        public T this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                inner[index] = value;
            }
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException();
            inner.Add(item);
            ItemAdded(item);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(T item)
        {
            return item != null && inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (!Contains(item))
                return false;

            bool result = inner.Remove(item);
            if (result)
                ItemRemoved(item);
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)inner).GetEnumerator();
        }

        #endregion

        protected virtual void ItemAdded(T item)
        {

        }

        protected virtual void ItemRemoved(T item)
        {

        }
    }
}
