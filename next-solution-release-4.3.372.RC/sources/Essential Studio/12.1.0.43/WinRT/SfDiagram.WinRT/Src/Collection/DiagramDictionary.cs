#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public class DiagramDictionary<TValue> : IList, IList<object>, ICollection, IEnumerable, ISharedData
    {
        private readonly IDictionary<object, DiagramKeyValue<TValue>> _mDictionary
            = new Dictionary<object, DiagramKeyValue<TValue>>();
        private readonly IDictionary<object, DiagramKeyValue<TValue>> _mVertesDictionary
            = new Dictionary<object, DiagramKeyValue<TValue>>();
        private readonly IDictionary<object, DiagramKeyValue<TValue>> _mConnectorDictionary
            = new Dictionary<object, DiagramKeyValue<TValue>>();
        private readonly IDictionary<object, DiagramKeyValue<TValue>> _mGroupDictionary
            = new Dictionary<object, DiagramKeyValue<TValue>>();

        private EventAggregartor _mEventAggregartor;
        private QuickViewAddedEvent _mQuickViewAddedEvent;
        
        void ISharedData.Init(SharedData shared)
        {
            _mEventAggregartor = shared.EventAggregator;
            _mQuickViewAddedEvent = _mEventAggregartor.GetEvent<QuickViewAddedEvent>();
        }

        //public TValue this[object key]
        //{
        //    get
        //    {
        //        return _mDictionary[key].Value;
        //    }
        //}

        internal TValue this[object key, ItemType itemType]
        {
            get
            {
                switch (itemType)
                {
                    case ItemType.None:
                        return _mDictionary[key].Value;
                    case ItemType.Group:
                        return _mGroupDictionary[key].Value;
                    case ItemType.Node:
                        return _mVertesDictionary[key].Value;
                    case ItemType.Connector:
                        return _mConnectorDictionary[key].Value;
                    default:
                        return default(TValue);
                }
            }
        }

        public int Add(object newItem)
        {
            DiagramKeyValue<TValue> item = newItem as DiagramKeyValue<TValue>;
            if (item.quickView is IGroup)
            {
                _mGroupDictionary.Add(item.Key, item);
            }
            else if (item.quickView is INode)
            {
                _mVertesDictionary.Add(item.Key, item);
            }
            else if (item.quickView is IConnector)
            {
                _mConnectorDictionary.Add(item.Key, item);
            }
            else
            {
                _mDictionary.Add(item.Key, item);
            }
            if (item.quickView != null)
            {
                _mQuickViewAddedEvent.Publish(item.quickView);
            }
            return 0;
        }

        internal void Add(DiagramKeyValue<TValue> item, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Group:
                    _mGroupDictionary.Add(item.Key, item);
                    break;
                case ItemType.Node:
                    _mVertesDictionary.Add(item.Key, item);
                    break;
                case ItemType.Connector:
                    _mConnectorDictionary.Add(item.Key, item);
                    break;
                case ItemType.None:
                    _mDictionary.Add(item.Key, item);
                    break;
            }
        }

        public void Clear()
        {
            _mDictionary.Clear();
        }

        public bool Contains(DiagramKeyValue<TValue> item)
        {
            return _mDictionary.ContainsKey(item.Key);
        }

        public bool Contains(object item)
        {
            return _mDictionary.ContainsKey(item) ||
                   _mGroupDictionary.ContainsKey(item) ||
                   _mVertesDictionary.ContainsKey(item) ||
                   _mConnectorDictionary.ContainsKey(item);
        }

        internal bool Contains(object item, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Group:
                    return _mGroupDictionary.ContainsKey(item);
                case ItemType.Node:
                    return _mVertesDictionary.ContainsKey(item);
                case ItemType.Connector:
                    return _mConnectorDictionary.ContainsKey(item);
                case ItemType.None:
                default:
                    return _mDictionary.ContainsKey(item);
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _mDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _mDictionary.IsReadOnly; }
        }

        public void Remove(object item)
        {
            _mDictionary.Remove((item as DiagramKeyValue<TValue>).Key);
        }

        //public IEnumerator<DiagramKeyValue<TValue>> GetEnumerator()
        //{
        //    foreach (var item in _mDictionary)
        //    {
        //        yield return item.Value;
        //    }
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _mDictionary)
            {
                yield return item.Value;
            }
        }

        public int IndexOf(object item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
        }


        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }


        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        void ICollection<object>.Add(object item)
        {
            this.Add(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            this.CopyTo((Array)array.ToArray(), arrayIndex);
        }

        bool ICollection<object>.Remove(object item)
        {
            this.Remove(item);
            return true;
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (var item in _mDictionary)
            {
                yield return item.Value;
            }
        }
    }

    internal enum ItemType
    {
        None,
        Node,
        Connector,
        Group
    }
}
