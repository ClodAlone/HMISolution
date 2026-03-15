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
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class DataTemplateViewController<TView>
        : ISharedData
        where TView : class, IView
    {
        readonly List<TView> _mRealized;
        private DiagramDictionary<DiagramKeyValue<Stack<TView>>> _mVirtualized;
        private Dictionary<object, TView> _mQuickView;
        private Dictionary<object, TView> _mNodeQuickView;
        private Dictionary<object, TView> _mConnectorQuickView;
        private Dictionary<object, TView> _mGroupQuickView;
        readonly DataTemplateDictionary _mSource;
        private SharedData _mSharedData;

        public DataTemplateViewController(DataTemplateDictionary source)
        {
            _mSource = source;
            _mRealized = new List<TView>();
        }

        public void Init(SharedData shared)
        {
            _mQuickView = new Dictionary<object, TView>();
            _mGroupQuickView = new Dictionary<object, TView>();
            _mNodeQuickView = new Dictionary<object, TView>();
            _mConnectorQuickView = new Dictionary<object, TView>();
            _mSharedData = shared;
            _mVirtualized = new DiagramDictionary<DiagramKeyValue<Stack<TView>>>();
            (_mVirtualized as ISharedData).Init(shared);
            QuickViewAddedEvent quickViewAdded = _mSharedData.EventAggregator.GetEvent<QuickViewAddedEvent>();
            quickViewAdded.Subscribe(AddQuickView);
        }

        internal void AddQuickView(IDiagramElement newItem)
        {
            if (newItem is ISelector)
            {
                _mGroupQuickView.Add(newItem.Key, newItem as TView);
            }
            else if (newItem is IGroup)
            {
                _mGroupQuickView.Add(newItem.Key, newItem as TView);
            }
            else if (newItem is INode)
            {
                _mNodeQuickView.Add(newItem.Key, newItem as TView);
            }
            else if (newItem is IConnector)
            {
                _mConnectorQuickView.Add(newItem.Key, newItem as TView);
            }
            else
            {
                _mQuickView.Add(newItem.Key, newItem as TView);                
            }
            (newItem as UIElement).Opacity = 0;
            //_mSharedData.Graph.Page.Children.Add(newItem as UIElement);
        }

        public TView GetQuickView(object key, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Group:
                    return _mGroupQuickView[key];
                case ItemType.Node:
                    return _mNodeQuickView[key];
                case ItemType.Connector:
                    return _mConnectorQuickView[key];
                case ItemType.None:
                    return _mQuickView[key];
                default:
                    return default(TView);
            }
        }

        public TView Realize(object key, ItemType itemType, out bool isNew)
        {
            if (!_mVirtualized.Contains(key, itemType))
            {
                var k = new DiagramKeyValue<DiagramKeyValue<Stack<TView>>>()
                {
                    Key = key,
                    Value = new DiagramKeyValue<Stack<TView>>()
                    {
                        Key = _mSource[key, itemType],
                        Value = new Stack<TView>()
                    }
                };

                _mVirtualized.Add(k, itemType);
            }
            DiagramKeyValue<Stack<TView>> virtualized = _mVirtualized[key, itemType];
            if (virtualized.Value != null && virtualized.Value.Count > 0)
            {
                TView recycle = virtualized.Value.Pop();
                _mRealized.Add(recycle);
                isNew = false;
                return recycle;
            }
            else
            {
                TView newView = null;
                //if (virtualized.quickView != null)
                //{
                //    this._mQuickView.Add(key, virtualized.quickView as TView);
                //    virtualized.quickView = null;
                //}
                newView = (virtualized.Key as DataTemplate).LoadContent() as TView;
                newView.Key = key;
                _mRealized.Add(newView);
                isNew = true;
                return newView;
            }
        }

        internal int GetRealizedCount()
        {
            return _mRealized.Count;
        }

        public bool Virtualize(TView view)
        {
            object key = null;
            key = view.Key;
            if (key != null)
            {
                _mRealized.Remove(view);
                if (view is ISelector || view is IGroup)
                {
                    _mVirtualized[key, ItemType.Group].Value.Push(view);
                }
                else if (view is INode)
                {
                    _mVirtualized[key, ItemType.Node].Value.Push(view);
                }
                else if (view is IConnector)
                {
                    _mVirtualized[key, ItemType.Connector].Value.Push(view);
                }
                else
                {
                    _mVirtualized[key, ItemType.None].Value.Push(view);                    
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //internal int GetVirtualizedCount()
        //{
        //    int cnt = 0;
        //    foreach (DiagramKeyValue<Stack<TView>> item in _mVirtualized)
        //    {
        //        cnt += item.Value.Value.Count();
        //    }
        //    return cnt;
        //}


        public void Dispose()
        {
        }
    }
}
