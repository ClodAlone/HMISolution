#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.DataVisualization.Models.Collections
{
    [Serializable]
    public class Collection :IList 
    {
        #region Members
        private ArrayList _members;
        private object _owner;
        #endregion

        #region Initialization
        public Collection()
        {
            _members = new ArrayList();
        }

        #endregion

        #region Public Properties
        public ArrayList Members
        {
            get
            {
                return _members;
            }
            set
            {
                if (value != _members)
                    _members = value;
            }
        }
        public int Count
        {
            get { return this._members.Count; }
        }

        public object this[int index]
        {
            get
            {
                return this._members[index];
            }
            set
            {
                Set(index, value);
            }
        }
        #endregion

        #region IList
        public void Add(object value)
        {
            if (!this._members.Contains(value))
                this._members.Add(value);
        } 

        public bool Contains(object value)
        {
            return this._members.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this._members.IndexOf(value);
        }

        public void Remove(object value)
        {
            if (this._members.Contains(value))
                this._members.Remove(value);
        }

        public void RemoveAt(int index)
        {
            if ((0 > index) || (index > (this._members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");
            this._members.RemoveAt(index);
        }

        public void Clear()
        {
            this._members.Clear();
        }

        public void RemoveRange(ICollection values)
        {
            IEnumerator enumerator = values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                int index = this._members.IndexOf(enumerator.Current);
                this._members.RemoveAt(index);
            }
        }

        protected void Set(int index, object value)
        {
            if ((0 > index) && (index > (this._members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");

            this._members[index] = value;
        }

        public IEnumerator GetEnumerator()
        {
            return new CollectionEnumerator(this);
        }
        #endregion





        #region INodeEnumerator class
        sealed class CollectionEnumerator : IEnumerator
        {
            #region Members
            private int _iIndex;
            private int _iCount;
            private Collection _iItems;
            #endregion

            #region Initialization
            internal CollectionEnumerator(Collection iItems)
            {
                _iItems = iItems;
                _iIndex = -1;
                _iCount = iItems.Count;
            }
            #endregion
            #region IEnumerator
            public object Current
            {
                get { return this._iItems[_iIndex]; }
            }

            public bool MoveNext()
            {
                if (_iItems.Count != _iCount)
                    throw new InvalidOperationException("collection was modified");

                return ++_iIndex < _iItems.Count;
            }

            public void Reset()
            {
                _iIndex = -1;
            }
            #endregion
        }
        #endregion

        int IList.Add(object value)
        {
            return AddValue(value);
        }

        public void Insert(int index, object value)
        {
            this.InsertValue(index, value);
        }

        public bool IsFixedSize
        {
            get { return this._members.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return this._members.IsReadOnly; }
        }

        public void CopyTo(Array array, int index)
        {
            this._members.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return this._members.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return this._members.SyncRoot; }
        }

        protected int AddValue(object value)
        {
            if (!this._members.Contains(value))
                return this._members.Add(value);
            return -1;
        }

        protected void InsertValue(int index, object value)
        {
            if ((0 > index) && (index < (this._members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");
            this._members.Insert(index, value);
        }
    }
}
