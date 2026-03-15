#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Panels;

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class VirtualizingController : ISharedData
    {
        private SharedData _mSharedData;
        private PageBoundsChangedEvent _mPageBoundsChanged;
        //private Rect _mCurrentViewPort = Rect.Empty;
        private AddedEvent<CollectionArgs<IInternalNode>> nodeAdded;
        AddedEvent<CollectionArgs<IInternalConnector>> connectorAdded;
        AddedEvent<CollectionArgs<IInternalGroup>> groupAdded;
        public void Init(SharedData shared)
        {
            _mSharedData = shared;

            nodeAdded
               = _mSharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalNode>>>();
            DeletedEvent<CollectionArgs<IInternalNode>> nodeDeleted
                = _mSharedData.EventAggregator.GetEvent<DeletedEvent<CollectionArgs<IInternalNode>>>();


            connectorAdded
                 = _mSharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalConnector>>>();
            DeletedEvent<CollectionArgs<IInternalConnector>> connectorDeleted
                = _mSharedData.EventAggregator.GetEvent<DeletedEvent<CollectionArgs<IInternalConnector>>>();


            groupAdded
                = _mSharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalGroup>>>();
            DeletedEvent<CollectionArgs<IInternalGroup>> groupDeleted
                = _mSharedData.EventAggregator.GetEvent<DeletedEvent<CollectionArgs<IInternalGroup>>>();

            nodeAdded.Subscribe(NodeAdded);
            nodeDeleted.Subscribe(NodeDeleted);
            connectorAdded.Subscribe(ConnectorAdded);
            connectorDeleted.Subscribe(ConnectorDeleted);
            groupAdded.Subscribe(GroupAdded);
            groupDeleted.Subscribe(GroupDeleted);

            BoundsChangedEvent mBoundsChanged = _mSharedData.EventAggregator.GetEvent<BoundsChangedEvent>();
            _mPageBoundsChanged = _mSharedData.EventAggregator.GetEvent<PageBoundsChangedEvent>();
            ViewportChangedEvent mViewportChanged = _mSharedData.EventAggregator.GetEvent<ViewportChangedEvent>();

            //_mVirtualize = _mSharedData.EventAggregartor.GetEvent<VirtualizeEvent>();
            //_mRealize = _mSharedData.EventAggregartor.GetEvent<RealizeEvent>();

            mBoundsChanged.Subscribe(NodeBoundsChanged);
            mViewportChanged.Subscribe(ViewportChanged);

            _mSharedData.EventAggregator.GetEvent<SelectionRectangleChangedEvent>().Subscribe(SelectionRectangleChanged);
            _mSharedData.EventAggregator.GetEvent<MeasuringArrangingEvent>().Subscribe(UpdateVirtualizationState);
        }

        internal void SelectionRectangleChanged(Rect selectionRect)
        {
            if (!CodeSharingUtilities.IsControlKeyPressed())
            {
                DiagramPreviewEventArgs args = new DiagramPreviewEventArgs(_mSharedData.Graph.SelectedItems);
                _mSharedData.Graph.OnItemUnSelectingEvent(args);
                if (!args.Cancel)
                {
                    _mSharedData.Graph.InternalSelectedItems.ClearSelection();
                }
            }


            if (Rect.Empty == selectionRect || (selectionRect.Width == 0 && selectionRect.Height == 0))
            {
                return;
            }
            List<Quad> quads = _mSharedData.SpatialSearch.FindQuads(selectionRect);
            if (_mSharedData.Graph.MultipleSelectionMode.Contains(MultipleSelectionMode.RubberBandPartialIntersect))
            {
                foreach (Quad quad in quads)
                {
                    foreach (IInternalGroupable node in quad.objects)
                    {
                        if (node is IInternalConnector)
                        {
                            if ((node as ConnectorWrapper).IsIntersectsWith(selectionRect))
                            {
                                node.RubberbandSelect();
                            }
                        }
                        else
                        {
                            if (node.Bounds.IsIntersect(selectionRect))
                            {
                                node.RubberbandSelect();
                            }
                        }
                    }
                }
            }
            else if (_mSharedData.Graph.MultipleSelectionMode.Contains(MultipleSelectionMode.RubberBandCompleteIntersect))
            {
                foreach (Quad quad in quads)
                {
                    foreach (IInternalGroupable node in quad.objects)
                    {
                        if (selectionRect.Contains(node.Bounds))
                        {
                            node.RubberbandSelect();
                        }
                    }
                }
            }
        }

        internal void Virtualize(IInternalGroupable element, bool removeFromContainer = false, Panel container = null)
        {
            if (element.VirtualizationState == VirtualizationState.Realized ||
                element.VirtualizationState == VirtualizationState.Realizing)
            {
                element.SetVirtualizationState(VirtualizationState.Virtualizing);
                UIElement view = element.View as UIElement;
                if (VirtualizeWrapper(element))
                {
                    element.SetVirtualizationState(VirtualizationState.Virtualized);
                    if (removeFromContainer)
                    {
                        if (container == null)
                        {
                            _mSharedData.Graph.Page.Children.Remove(view);
                        }
                        else
                        {
                            container.Children.Remove(view);
                        }
                    }
                }
                else
                {
                    element.SetVirtualizationState(VirtualizationState.Realized);
                }
            }
        }

        private bool VirtualizeWrapper(IInternalGroupable wrapper)
        {
            if (wrapper.CanVirtualize)
            {
                IView view = wrapper.View;
                if (wrapper.Source is IView)
                {
                    view = wrapper.Source as IView;
                }
                if (view != null)
                {
                    //m_ViewManager.Virtualize(node);
                    if (_mSharedData.ViewManager.Virtualize(view))
                    {
                        wrapper.View = null;
                        //_mSharedData.Page.Children.Remove(view as UIElement);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        internal void Virtualize(Rect bounds)
        {
            foreach (var item in GetAllElements())
            {
                if (bounds.IsIntersect(item.Bounds))
                {
                    this.Virtualize(item);
                }
            }
        }
        internal void Realize(Rect bounds)
        {
            foreach (var item in GetAllElements())
            {
                if (item.VirtualizationState != VirtualizationState.Realized)
                {
                    if (bounds.IsIntersect(item.Bounds))
                    {
                        this.Realize(item);
                    }
                }
            }
        }

        internal void Realize(IInternalGroupable element, Panel container = null, int? index = null)
        {
            if (element.VirtualizationState == VirtualizationState.Virtualized ||
                element.VirtualizationState == VirtualizationState.Virtualizing)
            {
                element.SetVirtualizationState(VirtualizationState.Realizing);
                if (RealizeWrapper(element))
                {
                    if (container == null)
                    {
                        _mSharedData.Graph.Page.Children.Add(element.View as UIElement);
                    }
                    else
                    {
                        if (index != null)
                        {
                            container.Children.Insert(index.Value, element.View as UIElement);
                        }
                        else
                            container.Children.Add(element.View as UIElement);
                    }
                }
                element.SetVirtualizationState(VirtualizationState.Realized);
            }
            //if (element is NodeWrapper)
            //{
            //    RealizeNodeWrapper(element as NodeWrapper);
            //}
            ////System.Diagnostics.Debug.WriteLine("Total: " + InternalNodes.Count +
            ////                                    " Live: " + m_ViewManager.GetRealizedCount() +
            ////                                    " Virtual: " + m_ViewManager.GetVirtualizedCount());
        }

        //private void RealizeSize(IGroupable element)
        //{
        //    RealizeSizeWrapper(element as IInternalGroupable);
        //    //if (element is NodeWrapper)
        //    //{
        //    //    RealizeSize(element as NodeWrapper);
        //    //}
        //    //System.Diagnostics.Debug.WriteLine("Total: " + InternalNodes.Count +
        //    //                                    " Live: " + m_ViewManager.GetRealizedCount() +
        //    //                                    " Virtual: " + m_ViewManager.GetVirtualizedCount());
        //}

        //private void RealizeSizeWrapper(IInternalGroupable wrapper)
        //{
        //    if (!(wrapper is IInternalNode))
        //    {
        //        return;
        //    }
        //    if ((wrapper as IInternalNode).ActualWidth == 0 ||
        //        (wrapper as IInternalNode).ActualHeight == 0)
        //    {
        //        ItemType itemType = ItemType.None;
        //        if (wrapper is IGroup)
        //        {
        //            itemType = ItemType.Group;
        //        }
        //        else if (wrapper is INode)
        //        {
        //            itemType = ItemType.Node;
        //        }
        //        else if (wrapper is IConnector)
        //        {
        //            itemType = ItemType.Connector;
        //        }
        //        object key = _mSharedData.Graph.GetKey(wrapper, itemType);
        //        if (key != null) // && _mSharedData.Presenter != null)
        //        {
        //            IView view = null;
        //            view = _mSharedData.ViewManager.GetQuickView(key, itemType);
        //            if (view != null)
        //            {
        //                _mSharedData.Graph.PrepareElementForItemOverride(view, wrapper.Source);
        //                wrapper.View = view;
        //                view.SetBusinessObject(wrapper.Source);
        //                (view as UIElement).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //                (view as UIElement).Arrange(new Rect(new Point(0, 0), (view as UIElement).DesiredSize));
        //                view.SetBusinessObject(null);
        //                wrapper.View = null;
        //            }
        //        }
        //    }
        //    //bool isNew;
        //    //if (_mSharedData.Presenter != null)
        //    //{
        //    //    Node node = wrapper.Source as Node;
        //    //    if (node == null)
        //    //    {
        //    //        node = _mSharedData.Graph.InternalGetNodeForItemOverride(wrapper, out isNew);
        //    //        //_mSharedData.ViewManager.Virtualize(node);
        //    //    //return;
        //    //        if (isNew)
        //    //        {
        //    //            _mSharedData.Graph.PrepareNodeForItemOverride(node, wrapper.Source);
        //    //            node.SetBusinessObject(wrapper.Source);
        //    //            wrapper.View = node;
        //    //            _mSharedData.Presenter.Content = node;
        //    //            _mSharedData.Presenter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //    //            _mSharedData.Presenter.Arrange(new Rect(new Point(0, 0), _mSharedData.Presenter.DesiredSize));
        //    //            _mSharedData.Presenter.Content = null;
        //    //            wrapper.View = null;
        //    //            node.SetBusinessObject(null);
        //    //            _mSharedData.ViewManager.Virtualize(node);
        //    //        }
        //    //        else
        //    //        {
        //    //            _mSharedData.Graph.PrepareNodeForItemOverride(node, wrapper.Source);
        //    //            node.SetBusinessObject(wrapper.Source);
        //    //            wrapper.View = node;
        //    //            node.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //    //            node.Arrange(new Rect(new Point(0, 0), node.DesiredSize));
        //    //            wrapper.View = null;
        //    //            node.SetBusinessObject(null);
        //    //            _mSharedData.ViewManager.Virtualize(node);
        //    //        }
        //    //    }
        //    //}
        //}

        private bool RealizeWrapper(IInternalGroupable wrapper)
        {
            bool isNew = false;
            IView view = wrapper.Source as IView;
            if (view == null)
            {
                view = _mSharedData.Graph.InternalGetViewForItemOverride(wrapper, out isNew);
                //if (view is INodeView)
                {
                    _mSharedData.Graph.PrepareElementForItemOverride(view, wrapper.Source);
                }
                view.SetBusinessObject(wrapper.Source);
                wrapper.View = view;
                (view as UIElement).InvalidateMeasure();
                (view as UIElement).InvalidateArrange();
                //DiagramPage.SetA(view as UIElement, true);
                //DiagramPage.SetM(node, true);  
            }
            else
            {
                wrapper.View = view;
                isNew = true;
                _mSharedData.SpatialSearch.UpdateQuad(wrapper);
            }
            return isNew;
            //if (isNew)
            //{
            //    _mSharedData.Graph.Page.Children.Add(view as UIElement);
            //}
        }

        internal void ViewportChanged(ChangeArgs<Rect> element)
        {
            _mSharedData.Page.InvalidateArrange();
            //_mCurrentViewPort = element.NewValue;
            //Rect old = _mPageBounds;
            //_mPageBounds = Rect.Empty;
            //foreach (var item in GetElements())
            //{
            //    Invalidate(item);
            //}
            //IGraphInternal graph = _mSharedData.Graph;

            //if (graph.InternalSelectedItems != null)
            //{
            //    Invalidate(graph.InternalSelectedItems);
            //}
            //_mPageBoundsChanged.Publish(new ChangeArgs<Rect>(null, old, _mPageBounds));
        }

        internal void UpdateVirtualizationState(bool measure)
        {
            Rect viewport = _mSharedData.ScrollViewer.Viewport;
            if (viewport.Width == 0 || viewport.Height == 0)
            {
                return;
            }
            if (!measure)
            {
                //Rect old = _mPageBounds;
                // _mPageBounds = Rect.Empty;
                //_mWorks.Clear();
                if (!_mSharedData.Graph.CanVirtualize)
                {
                    foreach (var item in GetAllElements())
                    {
                        Realize(item);
                    }
                }
                else
                {
                    if (_mSharedData.Graph.InternalNodes != null)
                    {
                        if (!_mSharedData.InvalidateState.RecalculateVirtualization)
                        {
                            return;
                        }
                        _mSharedData.InvalidateState.RecalculateVirtualization = false;
                    }

                    foreach (FrameworkElement child in _mSharedData.Page.Children.OfType<INode>())
                    {
                        if (child.DataContext != null)
                        {
                            IInternalNode wrapper = _mSharedData.Graph.GetNodeWrapper(child.DataContext, false);
                            if (wrapper != null)
                            {
                                if (!(wrapper.Bounds.IsIntersect(_mSharedData.ScrollViewer.Viewport)))
                                {
                                    Virtualize(wrapper);
                                }
                            }
                        }
                    }
                    foreach (FrameworkElement child in _mSharedData.Page.Children.OfType<IConnector>())
                    {
                        if (child.DataContext != null)
                        {
                            IInternalConnector wrapper = _mSharedData.Graph.GetConnectorWrapper(child.DataContext, false);
                            if (wrapper != null)
                            {
                                if (!(wrapper.Bounds.IsIntersect(_mSharedData.ScrollViewer.Viewport)))
                                {
                                    Virtualize(wrapper);
                                }
                            }
                        }
                    }
                    foreach (var item in GetElements())
                    {
                        if (!item.CanVirtualize)
                        {
                        }
                        else
                        {
                            Realize(item);
                            //Invalidate(item, viewport);
                        }
                    }
                }
                IGraphInternal graph = _mSharedData.Graph;

                //if (graph.InternalSelectedItems != null)
                //{
                //    Realize(graph.InternalSelectedItems);
                //}
                //if (_recalcPageSize == true)
                //{
                //    _recalcPageSize = false;
                //    _mPageBoundsChanged.Publish(new ChangeArgs<Rect>(null, old, _mPageBounds));
                //}
            }
        }

        //private void InvalidateGroup(IInternalGroup<TID, TNode, TConnector, TGroup, TNode> group)
        //{
        //    if (group.InternalNodes != null)
        //    {
        //        foreach (IInternalGroupable node in group.InternalNodes)
        //        {
        //            Invalidate(node);
        //        }
        //    }

        //    if (group.InternalConnectors != null)
        //    {
        //        foreach (IInternalGroupable connector in group.InternalConnectors)
        //        {
        //            Invalidate(connector);
        //        }
        //    }

        //    if (group.InternalGroups != null)
        //    {
        //        foreach (IInternalGroup<TID, TNode, TConnector, TGroup, TNode> g in group.InternalGroups)
        //        {
        //            InvalidateGroup(g);
        //        }
        //    }
        //}

        private IEnumerable<IInternalGroupable> GetElements()
        {
            //foreach (var internalNode in _mSharedData.Graph.InternalNodes)
            //{
            //    yield return internalNode;
            //}
            //if (_mSharedData.Graph.InternalConnectors != null)
            //{
            //    foreach (var internalCon in _mSharedData.Graph.InternalConnectors)
            //    {
            //        yield return internalCon;
            //    }
            //}

            Rect viewport = _mSharedData.ScrollViewer.Viewport;
            double currentZoom = _mSharedData.ScrollViewer.CurrentZoom;
            viewport = new Rect(viewport.X / currentZoom,
                         viewport.Y / currentZoom,
                         viewport.Width / currentZoom,
                         viewport.Height / currentZoom);

            List<Quad> quads = _mSharedData.SpatialSearch.FindQuads(viewport);
            foreach (Quad quad in quads)
            {
                foreach (IInternalGroupable node in quad.objects)
                {
                    if (node.Bounds.IsIntersect(viewport))
                        yield return node;
                }
            }
        }

        private IEnumerable<IInternalGroupable> GetAllElements()
        {
            IGraphInternal graph = _mSharedData.Graph;

            if (graph != null && graph.InternalNodes != null)
            {
                foreach (IInternalGroupable node in graph.InternalNodes)
                {
                    yield return node;
                }
            }

            if (graph != null && graph.InternalConnectors != null)
            {
                foreach (IInternalGroupable connector in graph.InternalConnectors)
                {
                    yield return connector;
                }
            }
            if (graph != null && graph.InternalGroups != null)
            {
                foreach (IInternalGroup g in graph.InternalGroups)
                {
                    yield return g;
                    foreach (var subG in GetElements(g))
                    {
                        yield return subG;
                    }
                }
            }
        }

        private IEnumerable<IInternalGroupable> GetElements(IInternalGroup group)
        {
            if (group != null && group.InternalNodes != null)
            {
                foreach (IInternalGroupable node in group.InternalNodes)
                {
                    yield return node;
                }
            }

            if (group != null && group.InternalConnectors != null)
            {
                foreach (IInternalGroupable connector in group.InternalConnectors)
                {
                    yield return connector;
                }
            }

            if (group != null && group.InternalGroups != null)
            {
                foreach (IInternalGroup g in group.InternalGroups)
                {
                    yield return g;
                    foreach (var subG in GetElements(g))
                    {
                        yield return subG;
                    }
                }
            }
        }

        internal void NodeBoundsChanged(ChangeArgs<Rect> element)
        {
            Action work =
                () => Invalidate(element.Sender as IInternalGroupable, _mSharedData.ScrollViewer.Viewport);
            //if (!SfDiagram.Disp.HasThreadAccess)
            //{
            //SfDiagram.Disp.RunAsync(CoreDispatcherPriority.High,
            //                                                     new DispatchedHandler(work));
            //Do(work, SfDiagram.Disp);
            //}
            //else
            //{
            work();
            //}
        }

        private object workLock = new object();


#if WINRT
        private bool working = false;
        private async void Do(Action work, CoreDispatcher dispatcher, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (working)
            {
                _mWorks.Enqueue(work);
            }
            else
            {
                _mWorks.Enqueue(work);
                while (_mWorks.Count > 0)
                {
                    working = true;
                    Action action = null;
                    lock (workLock)
                    {
                        if (_mWorks.Count > 0)
                        {
                            action = _mWorks.Peek();
                        }
                    }
                    if (action != null)
                    {
                        await dispatcher.RunAsync(priority, new DispatchedHandler(action));
                    }
                    lock (workLock)
                    {
                        if (_mWorks.Count > 0)
                        {
                            _mWorks.Dequeue();
                        }
                    }
                }
                working = false;
            }
        }
#endif

        Queue<Action> _mWorks = new Queue<Action>();

        internal void NodeAdded(CollectionArgs<IInternalNode> newElement)
        {
            if (newElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    undoable.LogData(new AddDelete(newElement.Element, newElement.ElementType, false));
                }
            }
            if (newElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemAddedEvent)
            {
                _mSharedData.Graph.OnItemAdded(new ItemAddedEventArgs(newElement.Element.Source, newElement.ItemSource));
            }
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            if (_mSharedData.Graph.CanVirtualize && !newElement.Element.CanVirtualize)
            {
                Realize(newElement.Element);
            }
            // _mSharedData.SpatialSearch.AddIntoaQuad(newElement.Element);
            //NodeWrapper wrapper = newElement.Element as NodeWrapper;
            //Invalidate(wrapper);
        }

        internal void NodeDeleted(CollectionArgs<IInternalNode> oldElement)
        {
            if (oldElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    _mSharedData.UndoRedoController.BeginComposite(undoable);
                    undoable.LogData(new AddDelete(oldElement.Element, oldElement.ElementType, true));
                    oldElement.Element.Dispose();
                    _mSharedData.UndoRedoController.EndComposite(undoable);
                }
                else
                {
                    oldElement.Element.Dispose();
                }
                oldElement.Element.SetVirtualizationState(VirtualizationState.Virtualized);
            }
            if (oldElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemDeletedEvent)
            {
                _mSharedData.Graph.OnItemDeleted(new DiagramEventArgs(oldElement.Element.Source));
            }
            //NodeWrapper wrapper = oldElement.Element as NodeWrapper;
            RemoveChild(oldElement.Element.View as UIElement);
            _mSharedData.Graph.ClearElementForItemOverride(oldElement.Element.View, oldElement.Element.Source);
            if (oldElement.Element.View != null)
            {
                oldElement.Element.View.SetBusinessObject(null);
                oldElement.Element.View = null;
            }
            oldElement.Element.IsSelected = false;
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            _mSharedData.SpatialSearch.RemoveFromAQuad(oldElement.Element);
            if (_mSharedData.SpatialSearch.LeftElement == oldElement.Element ||
                _mSharedData.SpatialSearch.TopElement == oldElement.Element ||
                _mSharedData.SpatialSearch.RightElement == oldElement.Element ||
                _mSharedData.SpatialSearch.BottomElement == oldElement.Element)
            {
                _mSharedData.SpatialSearch.UpdateBounds(oldElement.Element);
            }

        }

        internal void ConnectorAdded(CollectionArgs<IInternalConnector> newElement)
        {
            if (newElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    undoable.LogData(new AddDelete(newElement.Element, newElement.ElementType,
                                                   false));
                }
            }
            if (newElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemAddedEvent)
            {
                _mSharedData.Graph.OnItemAdded(new ItemAddedEventArgs(newElement.Element.Source, newElement.ItemSource));
            }
            if (_mSharedData.Graph.CanVirtualize && !newElement.Element.CanVirtualize)
            {
                Realize(newElement.Element);
            }
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            //_mSharedData.SpatialSearch.AddIntoaQuad(newElement.Element);
        }

        internal void ConnectorDeleted(CollectionArgs<IInternalConnector> oldElement)
        {
            if (oldElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    _mSharedData.UndoRedoController.BeginComposite(undoable);
                    undoable.LogData(new AddDelete(oldElement.Element, oldElement.ElementType, true));
                    oldElement.Element.Dispose();
                    _mSharedData.UndoRedoController.EndComposite(undoable);
                }
                else
                {
                    oldElement.Element.Dispose();
                }
                oldElement.Element.SetVirtualizationState(VirtualizationState.Virtualized);
            }
            if (oldElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemDeletedEvent)
            {
                _mSharedData.Graph.OnItemDeleted(new DiagramEventArgs(oldElement.Element.Source));
            }
            //ConnectorWrapper wrapper = oldElement.Element as ConnectorWrapper;
            RemoveChild(oldElement.Element.View as UIElement);
            //IView connector = wrapper.View;
            //wrapper.View = null;
            _mSharedData.Graph.ClearElementForItemOverride(oldElement.Element.View, oldElement.Element.Source);
            if (oldElement.Element.View != null)
            {
                oldElement.Element.View.SetBusinessObject(null);
                oldElement.Element.View = null;
            }

            oldElement.Element.IsSelected = false;
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            _mSharedData.SpatialSearch.RemoveFromAQuad(oldElement.Element);
            if (_mSharedData.SpatialSearch.LeftElement == oldElement.Element ||
                _mSharedData.SpatialSearch.TopElement == oldElement.Element ||
                _mSharedData.SpatialSearch.RightElement == oldElement.Element ||
                _mSharedData.SpatialSearch.BottomElement == oldElement.Element)
            {
                _mSharedData.SpatialSearch.UpdateBounds(oldElement.Element);
            }
        }

        internal void GroupAdded(CollectionArgs<IInternalGroup> newElement)
        {
            if (newElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    undoable.LogData(new AddDelete(newElement.Element, newElement.ElementType,
                                                   false));
                }
            }
            if (newElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemAddedEvent)
            {
                _mSharedData.Graph.OnItemAdded(new ItemAddedEventArgs(newElement.Element.Source, newElement.ItemSource));
            }
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            //_mSharedData.SpatialSearch.ParentQuad.AddIntoaQuad(newElement.Element);
        }

        internal void GroupDeleted(CollectionArgs<IInternalGroup> oldElement)
        {
            if (oldElement.SourceType == SourceType.Graph)
            {
                IUndoable undoable = _mSharedData.Graph as IUndoable;
                if (undoable.CanLogData())
                {
                    undoable.LogData(new AddDelete(oldElement.Element, oldElement.ElementType, true));
                }
                oldElement.Element.SetVirtualizationState(VirtualizationState.Virtualized);
            }
            if (oldElement.SourceType != SourceType.Graph)
                return;
            if (_mSharedData.NeededEvents.NeedItemDeletedEvent)
            {
                _mSharedData.Graph.OnItemDeleted(new DiagramEventArgs(oldElement.Element.Source));
            }
            oldElement.Element.IsSelected = false;
            RemoveChild(oldElement.Element.View as UIElement);
            _mSharedData.InvalidateState.RecalculateVirtualization = true;
            _mSharedData.Page.InvalidateMeasure();
            _mSharedData.SpatialSearch.RemoveFromAQuad(oldElement.Element);
        }

        private void RemoveChild(UIElement child)
        {
            _mSharedData.Graph.Page.Children.Remove(child);
        }

        private void Invalidate(IInternalGroupable element, Rect viewport)
        {
            if (InView(element, viewport))
            {
                Realize(element);
            }
            else
            {
                Virtualize(element);
                //else
                //{
                //    //RealizeSize(element);
                //}
            }
        }

        private bool InView(IInternalGroupable element, Rect viewport)
        {
            Rect elementBound = element.Bounds;
            //return true;
            //if (_recalcPageSize == true)
            //{
            //    _mPageBounds.Union(elementBound);
            //}
            if (element is IGroup)
            {
                return false;
            }
            else if (element is IConnector && elementBound.Width == 0 && elementBound.Height == 0)
            {
                return false;
            }
            //else if (elementBound == Rect.Empty)
            //{
            //    if (element is IConnector)
            //    {
            //        return false;
            //    }
            //    (element as IProtectedNode).UpdateCompositeTrans();
            //    elementBound = element.Bounds;
            //}
            bool intersect = elementBound.IsIntersect(ref viewport);
            if (intersect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
        }

        //internal void UpdatePageBounds(Rect rect)
        //{
        //     Rect old = _mPageBounds;
        //    _mPageBounds = rect;
        //    //_mPageBounds.X = _mSharedData.SpatialSearch._pageLeft;
        //    //_mPageBounds.Y = _mSharedData.SpatialSearch._pageTop;
        //    //_mPageBounds.Width = _mSharedData.SpatialSearch._pageRight - _mSharedData.SpatialSearch._pageLeft;
        //    //_mPageBounds.Height = _mSharedData.SpatialSearch._pageBottom - _mSharedData.SpatialSearch._pageTop;
        //    _mPageBoundsChanged.Publish(new ChangeArgs<Rect>(null, old, _mPageBounds));
        //}
    }
}
