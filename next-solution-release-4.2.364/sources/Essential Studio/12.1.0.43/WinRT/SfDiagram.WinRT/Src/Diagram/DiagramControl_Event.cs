#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.System;
using Windows.Foundation; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class SfDiagramWrapper
    {
        public event SelectedEventHandler ItemSelectedEvent;
        public event UnSelectedEventHandler ItemUnSelectedEvent;

        public event ItemTappedEventHandler ItemTappedEvent;
        public event ItemDoubleTappedEventHandler ItemDoubleTappedEvent;

        private event NodeChangedEventHandler _mNodeChangedEvent;
        public event NodeChangedEventHandler NodeChangedEvent
        {
            add
            {
                if (_mNodeChangedEvent == null)
                {
                    _mNodeChangedEvent += value;
                    CheckNodeTransformEvent();
                }
                else
                {
                    _mNodeChangedEvent += value;
                }
            }
            remove
            {
                _mNodeChangedEvent -= value;
                if (_mNodeChangedEvent == null)
                {
                    CheckNodeTransformEvent();
                }
            }
        }

        private event ItemAddedEventHandler _mItemAdded;
        public event ItemAddedEventHandler ItemAdded
        {
            add
            {
                if (_mItemAdded == null)
                {
                    _mItemAdded += value;
                    CheckItemAdded();
                }
                else
                {
                    _mItemAdded += value;
                }
            }
            remove
            {
                _mItemAdded -= value;
                if (_mItemAdded == null)
                {
                    CheckItemAdded();
                }
            }
        }

        private event ItemDeletedEventHandler _mItemDeleted;
        public event ItemDeletedEventHandler ItemDeleted
        {
            add
            {
                if (_mItemDeleted == null)
                {
                    _mItemDeleted += value;
                    CheckItemDeleted();
                }
                else
                {
                    _mItemDeleted += value;
                }
            }
            remove
            {
                _mItemDeleted -= value;
                if (_mItemDeleted == null)
                {
                    CheckItemDeleted();
                }
            }
        }
        
        private event ConnectorSourceChangedEventHandler _mConnectorSourceChangedEvent;
        public event ConnectorSourceChangedEventHandler ConnectorSourceChangedEvent
        {
            add
            {
                if (_mConnectorSourceChangedEvent == null)
                {
                    _mConnectorSourceChangedEvent += value;
                    CheckSourceChangedEventNeed();
                }
                else
                {
                    _mConnectorSourceChangedEvent += value;
                }
            }
            remove
            {
                _mConnectorSourceChangedEvent -= value;
                if (_mConnectorSourceChangedEvent == null)
                {
                    CheckSourceChangedEventNeed();
                }
            }
        }
        
        private event ConnectorTargetChangedEventHandler _mConnectorTargetChangedEvent;
        public event ConnectorTargetChangedEventHandler ConnectorTargetChangedEvent
        {
            add
            {
                if (_mConnectorTargetChangedEvent == null)
                {
                    _mConnectorTargetChangedEvent += value;
                    CheckTargetChangedEventNeed();
                }
                else
                {
                    _mConnectorTargetChangedEvent += value;
                }
            }
            remove
            {
                _mConnectorTargetChangedEvent -= value;
                if (_mConnectorTargetChangedEvent == null)
                {
                    CheckTargetChangedEventNeed();
                }
            }
        }

        private event ViewPortChangedEventHandler _mViewPortChangedEvent;
        public event ViewPortChangedEventHandler ViewPortChangedEvent
        {
            add
            {
                if (_mViewPortChangedEvent == null)
                {
                    _mViewPortChangedEvent += value;
                    CheckViewportChangedEvent();
                }
                else
                {
                    _mViewPortChangedEvent += value;
                }
            }
            remove
            {
                _mViewPortChangedEvent -= value;
                if (_mViewPortChangedEvent == null)
                {
                    CheckViewportChangedEvent();
                }
            }
        }

      
        private event SelectingEventHandler _mItemSelectingEvent;
        private event UnSelectingEventHandler _mItemUnSelectingEvent;

        public event SelectingEventHandler ItemSelectingEvent
        {
            add
            {
                if (_mItemSelectingEvent == null)
                {
                    _mItemSelectingEvent += value;
                  
                }
                else
                {
                    _mItemSelectingEvent += value;
                }
            }
            remove
            {
               
                if (_mItemSelectingEvent == null)
                {
                    _mItemSelectingEvent -= value;
                }
            }
        }

        public event UnSelectingEventHandler ItemUnSelectingEvent
        {
            add
            {
                if (_mItemUnSelectingEvent == null)
                {
                    _mItemUnSelectingEvent += value;

                }
                else
                {
                    _mItemUnSelectingEvent += value;
                }
            }
            remove
            {

                if (_mItemUnSelectingEvent == null)
                {
                    _mItemUnSelectingEvent -= value;
                }
            }
        }

        private event ItemDeletingEventHandler _mItemDeletingEvent;
        public event ItemDeletingEventHandler ItemDeletingEvent
        {
            add
            {
                if (_mItemDeletingEvent == null)
                {
                    _mItemDeletingEvent += value;
                    CheckItemDeleted();
                }
                else
                {
                    _mItemDeletingEvent += value;
                }
            }
            remove
            {
                _mItemDeletingEvent -= value;
                if (_mItemDeletingEvent == null)
                {
                    CheckItemDeleted();
                }
            }
        }    

        private event SymbolDroppingEventHandler _mSymbolDroppingEvent;
        public event SymbolDroppingEventHandler SymbolDroppingEvent
        {
            add
            {
                if (_mSymbolDroppingEvent == null)
                {
                    _mSymbolDroppingEvent += value;
                }
                else
                {
                    _mSymbolDroppingEvent += value;
                }
            }
            remove
            {
                _mSymbolDroppingEvent -= value;
            }
        }

        private void CheckNodeTransformEvent()
        {
            if (_mNodeChangedEvent != null &&
                SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedNodeChangedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedNodeChangedEvent = false;
            }
        }

        private void CheckItemAdded()
        {
            if (_mItemAdded != null &&
                SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedItemAddedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedItemAddedEvent = false;
            }
        }

        private void CheckItemDeleted()
        {
            if (_mItemDeleted != null &&
                SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedItemDeletedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedItemDeletedEvent = false;
            }
        }
        
        private void CheckSourceChangedEventNeed()
        {
            if (_mConnectorSourceChangedEvent != null &&
                SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedSourceChangedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedSourceChangedEvent = false;
            }
        }

        private void CheckTargetChangedEventNeed()
        {
            if (_mConnectorTargetChangedEvent != null &&
                SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedTargetChangedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedTargetChangedEvent = false;
            }
        }
        private void CheckViewportChangedEvent()
        {
            if (_mViewPortChangedEvent != null && SharedData.Graph.Constraints.Contains(GraphConstraints.Events))
            {
                SharedData.NeededEvents.NeedViewportChangedEvent = true;
            }
            else
            {
                SharedData.NeededEvents.NeedViewportChangedEvent = false;
            }
        }
        public void OnItemSelectedEvent(DiagramEventArgs args)
        {
            SelectedEventHandler handler = ItemSelectedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemUnSelectedEvent(DiagramEventArgs args)
        {
            UnSelectedEventHandler handler = ItemUnSelectedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemTappedEvent(DiagramEventArgs args)
        {
            ItemTappedEventHandler handler = ItemTappedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemDoubleTappedEvent(DiagramEventArgs args)
        {
            ItemDoubleTappedEventHandler handler = ItemDoubleTappedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnNodeChangedEvent(ChangeEventArgs<object, NodeChangedEventArgs> args)
        {
            NodeChangedEventHandler handler = _mNodeChangedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemAdded(ItemAddedEventArgs args)
        {
            ItemAddedEventHandler handler = _mItemAdded;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemDeleted(DiagramEventArgs args)
        {
            ItemDeletedEventHandler handler = _mItemDeleted;
            if (handler != null) handler(_graphView, args);
        }
        public void OnConnectorSourceChangedEvent(ChangeEventArgs<object, ConnectorChangedEventArgs> args)
        {
            ConnectorSourceChangedEventHandler handler = _mConnectorSourceChangedEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnConnectorTargetChangedEvent(ChangeEventArgs<object, ConnectorChangedEventArgs> args)
        {
            ConnectorTargetChangedEventHandler handler = _mConnectorTargetChangedEvent;
            if (handler != null) handler(_graphView, args);
        }

        public void OnViewPortChangedEvent(ChangeEventArgs<object, ScrollChanged> args)
        {
            ViewPortChangedEventHandler handler = _mViewPortChangedEvent;
            if (handler != null) handler(_graphView, args);
        }

        public void OnSymbolDroppingEvent(SymbolDroppingEventArgs args)
        {
            SymbolDroppingEventHandler handler = _mSymbolDroppingEvent;
            if (handler != null) handler(_graphView,args);
        }

        public void OnItemSelectingEvent(DiagramPreviewEventArgs args)
        {
            SelectingEventHandler handler = _mItemSelectingEvent;
            if (handler != null) handler(_graphView, args);
        }
        public void OnItemUnSelectingEvent(DiagramPreviewEventArgs args)
        {
            UnSelectingEventHandler handler = _mItemUnSelectingEvent;
            if (handler != null) handler(_graphView, args);
        }

        public void OnItemDeletingEvent(DiagramPreviewEventArgs args)
        {
            ItemDeletingEventHandler handler = _mItemDeletingEvent;
            if (handler != null) handler(_graphView, args);
        }
        
        //public event SelectedEventHandler GroupSelectedEvent;
        //public event UnSelectedEventHandler GroupUnSelectedEvent;
        //public event ItemTappedEventHandler GroupTappedEvent;
        //public event ItemDoubleTappedEventHandler GroupDoubleTappedEvent;
        //public event TransformStartingEventHandler GroupTransformStartingEvent;
        //public event TransformStartedEventHandler GroupTransformStartedEvent;
        //public event TransformDeltaEventHandler GroupTransformDeltaEvent;
        //public event TransformCompletedEventHandler GroupTransformCompletedEvent;
        //public event TransformCanceledEventHandler GroupTransformCanceledEvent;
        //public event TransformedEventHandler GroupTransformedEvent;

        //public event ItemAddedEventHandler GroupAdded;
        //public event ItemDeletedEventHandler GroupDeleted;
        //void IGraphInternal.OnGroupTransformedEvent(TransformEventArgs args)
        //{
        //    TransformedEventHandler handler = GroupTransformedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupUnSelectedEvent(DiagramEventArgs args)
        //{
        //    UnSelectedEventHandler handler = GroupUnSelectedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTappedEvent(DiagramEventArgs args)
        //{
        //    ItemTappedEventHandler handler = GroupTappedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupDoubleTappedEvent(DiagramEventArgs args)
        //{
        //    ItemDoubleTappedEventHandler handler = GroupDoubleTappedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTransformStartingEvent(TransformStartingEventArgs args)
        //{
        //    TransformStartingEventHandler handler = GroupTransformStartingEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTransformStartedEvent(TransformStartedEventArgs args)
        //{
        //    TransformStartedEventHandler handler = GroupTransformStartedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTransformDeltaEvent(TransformDeltaEventArgs args)
        //{
        //    TransformDeltaEventHandler handler = GroupTransformDeltaEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTransformCompletedEvent(TransformCompletedEventArgs args)
        //{
        //    TransformCompletedEventHandler handler = GroupTransformCompletedEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupTransformCanceledEvent(TransformCanceledEventArgs args)
        //{
        //    TransformCanceledEventHandler handler = GroupTransformCanceledEvent;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupAdded(DiagramEventArgs args)
        //{
        //    ItemAddedEventHandler handler = GroupAdded;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupDeleted(DiagramEventArgs args)
        //{
        //    ItemDeletedEventHandler handler = GroupDeleted;
        //    if (handler != null) handler(this, args);
        //}
        //void IGraphInternal.OnGroupSelectedEvent(DiagramEventArgs args)
        //{
        //    SelectedEventHandler handler = GroupSelectedEvent;
        //    if (handler != null) handler(this, args);
        //}
    }
}
