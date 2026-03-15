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
using System.Reflection;
using System.Text;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal interface IInternalCollection<T> : IEnumerable<T>
    {
        void Add(T item, ItemSource source);
    }

    internal class ObservableElements<TSource, TWrap> :
        IInternalCollection<TWrap>, 
        INotifyCollectionChanged 
        where TWrap : IWrapper
    {
        public Type ItemType { get; set; }
        private readonly Dictionary<TSource, TWrap> _mCopy = new Dictionary<TSource, TWrap>();
        //private readonly ICollection<TSource> _mSourceICollection;
        private MethodInfo _mAddMethod;
        private MethodInfo _mRemoveMethod;
        private MethodInfo _mInsertMethod;
        private readonly IEnumerable _mSource;
        bool _sourceCollectionChaning = false;
        bool _copyCollectionChaning = false;

        readonly AddedEvent<CollectionArgs<TWrap>> _mAddedEvent;

        readonly DeletedEvent<CollectionArgs<TWrap>> _mDeletedEvent;
        readonly ElementType _mElementType = ElementType.Connector;
        readonly SourceType _mSrcType = SourceType.Graph;

        public SourceType SourceType
        {
            get { return _mSrcType; }
        }

        public ElementType ElementType
        {
            get { return _mElementType; }
        }

        public IEnumerable Source
        {
            get { return _mSource; }
        }
        
        public ObservableElements(object source, ElementType type, SourceType srcType,
                                    EventAggregartor aggregator, Func<TSource, bool, TWrap> wrapper)
        {
            GetNewWrapper = wrapper;
            _mSrcType = srcType;
            Type collection = source.GetType();
            //_mSourceICollection = source as ICollection<TSource>;
            foreach (var inter in collection.GetTypeInfo().ImplementedInterfaces())
            {
                if (inter.Name == "ICollection`1")
                {
                    ItemType = inter.GenericTypeArguments()[0];
                    _mAddMethod = collection.GetRuntimeMethod("Add", new[] { ItemType });
                    _mRemoveMethod = collection.GetRuntimeMethod("Remove", new[] { ItemType });
                    _mInsertMethod = collection.GetRuntimeMethod("Insert", new[] {typeof(int), ItemType});
                }
            }
            _mSource = source as IEnumerable;
            _mElementType = type;
            _mAddedEvent = aggregator.GetEvent<AddedEvent<CollectionArgs<TWrap>>>();
            _mDeletedEvent = aggregator.GetEvent<DeletedEvent<CollectionArgs<TWrap>>>();
            if (source is INotifyCollectionChanged)
            {
                (source as INotifyCollectionChanged).CollectionChanged += source_CollectionChanged;
            }
            //Task t = new Task(() =>
            //    {
                    _sourceCollectionChaning = true;
                    foreach (TSource item in _mSource)
                    {
                        this.Add(GetNewWrapper(item, true), ItemSource.UnKnown);
                    }
                    _sourceCollectionChaning = false;
            //    });
            //t.Start();
        }

        public bool TryGetWrapper(TSource item, out TWrap wrap)
        {
            return _mCopy.TryGetValue(item, out wrap);
        }

        private TWrap GetWrapperInternal(TSource item)
        {
            if (_mCopy.ContainsKey(item))
            {
                return _mCopy[item];
            }
            else
            {
                return GetNewWrapper(item, true);
            }
        }

        public Func<TSource, bool, TWrap> GetNewWrapper;

        void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_copyCollectionChaning)
            {
                return;
            }
            _sourceCollectionChaning = true;
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged.Invoke(this, e);
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (TSource item in e.NewItems)
                        {
                           // this.Adding(GetNewWrapper(item, true), ItemSource.UnKnown);
                            this.Add(GetNewWrapper(item, true), ItemSource.UnKnown);
                        }
                    }                 
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (TSource item in e.OldItems)
                        {
                            this.Remove(GetWrapperInternal(item));
                        }
                    }
                    break;
#if WINRT
                case NotifyCollectionChangedAction.Move: 
#endif
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in _mCopy.Keys.Except(_mSource as IEnumerable<TSource>).ToList())
                    {
                        this.Remove(GetWrapperInternal(item));
                    }
                    foreach (TSource item in _mSource)
                    {
                        if (!this.Contains(item))
                        {
                            this.Add(GetNewWrapper(item, true), ItemSource.UnKnown);
                        }
                    }
                    break;
            }
            _sourceCollectionChaning = false;
        }

        public void Add(TWrap item, ItemSource source)
        {
           //if(!_mAddedEvent.IsSuspended) //lock (enumLock)
            {
                _copyCollectionChaning = true;
                _mCopy.Add((TSource) item.Source, item);

                if (_sourceCollectionChaning == false && _mAddMethod != null)
                {
                    _mAddMethod.Invoke(_mSource, new[] {item.Source});
                    //_mSourceICollection.Add((TSource)item.Source);
                }

                if (_mAddedEvent.HasSubscripitons &&
                    !_mAddedEvent.IsSuspended)
                {
                    CollectionArgs<TWrap> args = new CollectionArgs<TWrap>(this, item, _mElementType, _mSrcType, source);
                    _mAddedEvent.Publish(args);
                }
                if (Added != null)
                {
                    CollectionArgs<TWrap> args = new CollectionArgs<TWrap>(this, item, _mElementType, _mSrcType, source);
                    Added.Invoke(args);
                }

                _copyCollectionChaning = false;
            }
        }

        public void Insert(TWrap item, ItemSource source, int index)
        {
            _copyCollectionChaning = true;
            _mCopy.Add((TSource)item.Source,item);

            if (_sourceCollectionChaning == false && _mInsertMethod != null)
            {
                _mInsertMethod.Invoke(_mSource, new[] { index, item.Source });
            }
            if (_mAddedEvent.HasSubscripitons &&
                   !_mAddedEvent.IsSuspended)
            {
                CollectionArgs<TWrap> args = new CollectionArgs<TWrap>(this, item, _mElementType, _mSrcType, source);
                _mAddedEvent.Publish(args);
            }
            if (Added != null)
            {
                CollectionArgs<TWrap> args = new CollectionArgs<TWrap>(this, item, _mElementType, _mSrcType, source);
                Added.Invoke(args);
            }
            _copyCollectionChaning = false;
        }

        //public void AddIfDoesNotExist(TWrap item)
        //{
        //    if (!_mCopy.ContainsKey((TSource)item.Source))
        //    {
        //        Add(item, ItemSource.UnKnown);
        //    }
        //}

        public void Clear()
        {
            _copyCollectionChaning = true;
            while (_mCopy.Count != 0)
            {
                TWrap item = _mCopy.Values.First();
                Remove(item);
                if (_sourceCollectionChaning == false && _mRemoveMethod != null)
                {
                    _mRemoveMethod.Invoke(_mSource, new[] { item.Source });
                    //_mSourceICollection.Remove((TSource)item.Source);
                }
            } _copyCollectionChaning = false;
        }

        public bool Contains(TWrap item)
        {
            return _mCopy.ContainsValue(item);
        }

        public bool Contains(TSource item)
        {
            return _mCopy.ContainsKey(item);
        }

        public void CopyTo(TWrap[] array, int arrayIndex)
        {
            _mCopy.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _mCopy.Count; }
        }

        public bool IsReadOnly
        {
            get { return (bool)_mSource.GetType().GetRuntimeProperty("IsReadOnly").GetValue(_mSource); }
        }

        public bool Remove(TWrap item)
        {
            _copyCollectionChaning = true;
            if (_mCopy.Remove((TSource)item.Source))
            {
                if (_sourceCollectionChaning == false && _mRemoveMethod != null)
                {
                    _mRemoveMethod.Invoke(_mSource, new[] { item.Source });
                    //_mSourceICollection.Remove((TSource)item.Source);
                }
                CollectionArgs<TWrap> args = new CollectionArgs<TWrap>(this, item, _mElementType, _mSrcType, ItemSource.UnKnown);
                PublishDelete(args);
                _copyCollectionChaning = false;
                return true;
            }
            else
            {
                _copyCollectionChaning = false;
                return false;
            }
        }

        private object enumLock = new object();

        public IEnumerator<TWrap> GetEnumerator()
        {
            //lock (enumLock)
            {
                foreach (var item in _mCopy.Values)
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            yield return _mCopy.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event Action<CollectionArgs<TWrap>> Added;
        public event Action<CollectionArgs<TWrap>> Deleted;
        
        private void PublishDelete(CollectionArgs<TWrap> args)
        {
            if (_mDeletedEvent.HasSubscripitons &&
                !_mDeletedEvent.IsSuspended)
            {
                _mDeletedEvent.Publish(args);
            }
            if (Deleted != null)
            {
                Deleted.Invoke(args);
            }
        }
    }

    enum ElementType
    {
        Node,
        Connector,
        Group,
        Port,
        Annotation,
        Segment
    }

    enum SourceType
    {
        Graph,
        SelectionList,
        Group,
        Node,
        Segments
    }

}
